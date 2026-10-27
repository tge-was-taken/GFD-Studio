using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.CommandLine;
using System.IO;
using System.Linq;
using System.Text.Json;
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

            var command = new Command( "roundtrip", "Load and save files unchanged, and report any that do not come back identical." )
            {
                path, results, games, sample, offset, workers,
            };

            command.SetAction( parse => CommandHelpers.Guard( () => Run(
                parse.GetValue( path ), parse.GetValue( results ), parse.GetValue( games ),
                parse.GetValue( sample ), parse.GetValue( offset ), parse.GetValue( workers ) ) ) );

            return command;
        }

        private static int Run( string path, string resultsPath, string games, int sample, int offset, int workers )
        {
            var entries = Collect( path, games, Math.Max( sample, 1 ), offset );

            if ( entries.Count == 0 )
                throw new CliException( $"Nothing to round trip under \"{path}\"." );

            var done = new ConcurrentBag<Result>();
            Parallel.ForEach( entries, new ParallelOptions { MaxDegreeOfParallelism = Math.Max( workers, 1 ) },
                              entry => done.Add( One( entry ) ) );

            var ordered = done.OrderBy( x => x.Name, StringComparer.Ordinal ).ToList();

            if ( !string.IsNullOrEmpty( resultsPath ) )
                File.WriteAllLines( resultsPath, ordered.Select( x => x.Detail.Length == 0
                                                                     ? $"{x.Status}|{x.Name}"
                                                                     : $"{x.Status}|{x.Name}|{x.Detail}" ) );

            Report( ordered );

            return ordered.Any( x => x.Status is not ("MATCH" or "KNOWN_BAD") ) ? 1 : 0;
        }

        private static Result One( Entry entry )
        {
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

            var differing = before.Zip( after ).Count( pair => pair.First != pair.Second )
                            + Math.Abs( before.Length - after.Length );

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

        private sealed record Entry( string Path, string Name, string KnownBad );

        private sealed record Result( string Status, string Name, string Detail );
    }
}
