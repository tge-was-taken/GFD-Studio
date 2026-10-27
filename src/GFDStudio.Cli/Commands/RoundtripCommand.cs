using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.CommandLine;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using GFDLibrary;

namespace GFDStudio.Cli.Commands
{
    /// <summary>
    /// Reads a file and writes it straight back, then compares the bytes. A difference means the
    /// library lost or changed something it did not understand.
    /// </summary>
    public static class RoundtripCommand
    {
        public static Command Create()
        {
            var path = new Argument<string>( "path" )
            {
                Description = "A file, a directory to walk, or a corpus checkout holding manifest.json.",
            };

            var results = CommandHelpers.Text( "--results", "Write one status line per file here." );
            var games = CommandHelpers.Text( "--games", "Comma separated game tags from the manifest, default all." );
            var sample = new Option<int>( "--sample" ) { Description = "Take every Nth file.", DefaultValueFactory = _ => 1 };
            var offset = new Option<int>( "--offset" ) { Description = "Which of the N slices to take.", DefaultValueFactory = _ => 0 };
            var workers = new Option<int>( "--workers" ) { Description = "How many files to do at once.", DefaultValueFactory = _ => Environment.ProcessorCount };
            var budget = new Option<int>( "--memory-budget" )
            {
                Description = "Megabytes of loaded files to hold at once. 0 lifts the limit.",
                DefaultValueFactory = _ => DefaultBudgetMegabytes(),
            };

            var command = new Command( "roundtrip", "Load and save files unchanged, and report any that do not come back identical." )
            {
                path, results, games, sample, offset, workers, budget,
            };

            command.SetAction( parse => CommandHelpers.Guard( () => Run(
                parse.GetValue( path ), parse.GetValue( results ), parse.GetValue( games ),
                parse.GetValue( sample ), parse.GetValue( offset ), parse.GetValue( workers ),
                parse.GetValue( budget ) ) ) );

            return command;
        }

        private static int Run( string path, string resultsPath, string games, int sample, int offset, int workers,
                                int budgetMegabytes )
        {
            var entries = Collect( path, games, Math.Max( sample, 1 ), offset );

            if ( entries.Count == 0 )
                throw new CliException( $"Nothing to round trip under \"{path}\"." );

            // Biggest first, so the files that need most of the budget get it while the run is
            // young rather than queueing behind a tail of small ones.
            var queue = entries.OrderByDescending( Size ).ToList();
            var budget = new MemoryBudget( (long)Math.Max( budgetMegabytes, 0 ) * 1024 * 1024 );

            var done = new ConcurrentBag<Result>();
            Parallel.ForEach( queue, new ParallelOptions { MaxDegreeOfParallelism = Math.Max( workers, 1 ) },
                              entry => done.Add( One( entry, budget ) ) );

            var ordered = done.OrderBy( x => x.Name, StringComparer.Ordinal ).ToList();

            if ( !string.IsNullOrEmpty( resultsPath ) )
                File.WriteAllLines( resultsPath, ordered.Select( x => x.Detail.Length == 0
                                                                     ? $"{x.Status}|{x.Name}"
                                                                     : $"{x.Status}|{x.Name}|{x.Detail}" ) );

            Report( ordered );

            return ordered.Any( x => x.Status is not ("MATCH" or "KNOWN_BAD") ) ? 1 : 0;
        }

        private static Result One( Entry entry, MemoryBudget budget )
        {
            using var reservation = budget.Reserve( Size( entry ) );

            byte[] before;

            try
            {
                before = File.ReadAllBytes( entry.Path );
            }
            catch ( Exception exception )
            {
                return new Result( "READ_FAIL", entry.Name, exception.Message );
            }

            if ( entry.KnownBad != null )
                return new Result( "KNOWN_BAD", entry.Name, entry.KnownBad );

            Resource resource;

            try
            {
                using var input = new MemoryStream( before );
                resource = Resource.Load( input, false );
            }
            catch ( Exception exception )
            {
                return new Result( "LOAD_FAIL", entry.Name, Describe( exception ) );
            }

            byte[] after;

            try
            {
                using var output = new MemoryStream( before.Length );
                resource.Save( output, true );
                after = output.ToArray();
            }
            catch ( Exception exception )
            {
                return new Result( "SAVE_FAIL", entry.Name, Describe( exception ) );
            }

            if ( before.AsSpan().SequenceEqual( after ) )
                return new Result( "MATCH", entry.Name, "" );

            var shared = Math.Min( before.Length, after.Length );
            var differing = Math.Abs( before.Length - after.Length );

            for ( var i = 0; i < shared; i++ )
                if ( before[i] != after[i] )
                    differing++;

            return new Result( "DIFF", entry.Name, $"size {before.Length} vs {after.Length}, {differing} bytes differ" );
        }

        private static string Describe( Exception exception )
            => $"{exception.GetType().Name}: {exception.Message.ReplaceLineEndings( " " )}";

        private static void Report( IReadOnlyList<Result> results )
        {
            var counts = results.GroupBy( x => x.Status )
                                .ToDictionary( x => x.Key, x => x.Count() );

            Program.Output.WriteLine( $"{results.Count} files" );

            foreach ( var status in counts.OrderByDescending( x => x.Value ) )
                Program.Output.WriteLine( $"  {status.Value,6} {status.Key}" );

            var bad = results.Where( x => x.Status is not ("MATCH" or "KNOWN_BAD") ).ToList();

            foreach ( var result in bad.Take( 50 ) )
                Console.Error.WriteLine( $"{result.Status} {result.Name} {result.Detail}" );

            if ( bad.Count > 50 )
                Console.Error.WriteLine( $"and {bad.Count - 50} more" );
        }

        /// <summary>
        /// The files to try, from a manifest where there is one and from the directory tree
        /// otherwise. Slicing rotates with the offset so repeated partial runs cover everything.
        /// </summary>
        private static List<Entry> Collect( string path, string games, int sample, int offset )
        {
            var entries = Enumerate( path, games ).OrderBy( x => x.Name, StringComparer.Ordinal ).ToList();

            return sample <= 1 ? entries : entries.Where( ( _, i ) => i % sample == offset % sample ).ToList();
        }

        private static IEnumerable<Entry> Enumerate( string path, string games )
        {
            if ( File.Exists( path ) )
                return new[] { new Entry( path, Path.GetFileName( path ), null ) };

            if ( !Directory.Exists( path ) )
                throw new CliException( $"No file or directory at \"{path}\"." );

            var manifest = Path.Combine( path, "manifest.json" );

            return File.Exists( manifest ) ? FromManifest( path, manifest, games ) : FromTree( path );
        }

        private static IEnumerable<Entry> FromManifest( string root, string manifest, string games )
        {
            var wanted = string.IsNullOrEmpty( games )
                ? null
                : new HashSet<string>( games.Split( ',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries ),
                                       StringComparer.OrdinalIgnoreCase );

            using var file = File.OpenRead( manifest );

            foreach ( var element in JsonDocument.Parse( file ).RootElement.EnumerateArray() )
            {
                var name = element.GetProperty( "corpus" ).GetString();

                if ( wanted != null && !wanted.Contains( Game( element ) ) )
                    continue;

                yield return new Entry( Path.Combine( root, name ), name,
                                        element.TryGetProperty( "known_bad", out var reason ) ? reason.GetString() : null );
            }
        }

        private static string Game( JsonElement element )
            => element.TryGetProperty( "game", out var game ) ? game.GetString() : "";

        private static IEnumerable<Entry> FromTree( string root )
            => Directory.EnumerateFiles( root, "*", SearchOption.AllDirectories )
                        .Where( x => Readable.Contains( Path.GetExtension( x ) ) )
                        .Select( x => new Entry( x, Path.GetRelativePath( root, x ), null ) );

        private static readonly HashSet<string> Readable = new( StringComparer.OrdinalIgnoreCase )
        {
            ".gmd", ".gfs", ".gap", ".gmdl", ".gac", ".ganm", ".gnf", ".epl",
        };

        private static long Size( Entry entry )
        {
            try
            {
                return new FileInfo( entry.Path ).Length;
            }
            catch
            {
                return 0;
            }
        }

        /// <summary>
        /// A quarter of what the machine has, and never more than 4 GB. Loading a model costs
        /// several times the file on disk, so the budget is deliberately well under the total.
        /// </summary>
        private static int DefaultBudgetMegabytes()
        {
            var total = GC.GetGCMemoryInfo().TotalAvailableMemoryBytes;
            var quarter = total > 0 ? total / 4 : 2L * 1024 * 1024 * 1024;

            return (int)( Math.Min( quarter, 4L * 1024 * 1024 * 1024 ) / ( 1024 * 1024 ) );
        }

        /// <summary>
        /// Keeps the loaded files from piling up. A model expands well beyond its size on disk,
        /// so each file is charged a multiple of that and waits for room before it is read. One
        /// file always gets through, however large, so a single outsized file cannot deadlock.
        /// </summary>
        private sealed class MemoryBudget
        {
            /// <summary>
            /// What a file costs in memory against its size on disk. Measured on model packs,
            /// where the vertex and key arrays become objects.
            /// </summary>
            private const long Expansion = 12;

            private readonly long mLimit;
            private long mInFlight;

            public MemoryBudget( long limit ) => mLimit = limit;

            public IDisposable Reserve( long size )
            {
                if ( mLimit <= 0 )
                    return new Reservation( this, 0 );

                var cost = Math.Max( size, 1 ) * Expansion;

                lock ( this )
                {
                    while ( mInFlight > 0 && mInFlight + cost > mLimit )
                        Monitor.Wait( this );

                    mInFlight += cost;
                }

                return new Reservation( this, cost );
            }

            private void Release( long cost )
            {
                lock ( this )
                {
                    mInFlight -= cost;
                    Monitor.PulseAll( this );
                }
            }

            private sealed class Reservation : IDisposable
            {
                private readonly MemoryBudget mBudget;
                private readonly long mCost;

                public Reservation( MemoryBudget budget, long cost )
                {
                    mBudget = budget;
                    mCost = cost;
                }

                public void Dispose()
                {
                    if ( mCost == 0 )
                        return;

                    // The reservation covers the loaded object graph, which is only actually
                    // gone once it has been collected.
                    GC.Collect();
                    mBudget.Release( mCost );
                }
            }
        }

        private sealed record Entry( string Path, string Name, string KnownBad );

        private sealed record Result( string Status, string Name, string Detail );
    }
}
