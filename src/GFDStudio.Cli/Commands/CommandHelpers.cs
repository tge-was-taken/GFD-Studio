using System;
using System.CommandLine;
using System.Reflection;

namespace GFDStudio.Cli.Commands
{
    internal static class CommandHelpers
    {
        /// <summary>
        /// A question the core asks that has to be answered one way or the other, taken from a
        /// pair of switches. Null when neither was given, which the dialogs turn into an error
        /// naming them.
        /// </summary>
        public static bool? Toggle( bool yes, bool no, string yesFlag = "--yes", string noFlag = "--no" )
        {
            if ( yes && no )
                throw new CliException( $"\"{yesFlag}\" and \"{noFlag}\" contradict each other." );

            if ( yes )
                return true;

            return no ? false : null;
        }

        public static Option<string> Text( string name, string description, bool required = false )
            => new( name ) { Description = description, Required = required };

        public static Option<bool> Switch( string name, string description )
            => new( name ) { Description = description };

        /// <summary>
        /// Runs the body of a command, turning anything it throws into one line on standard
        /// error. The parser runs actions itself and reports what escapes them with a stack
        /// trace, which is no use to someone who mistyped a path.
        /// </summary>
        public static int Guard( Func<int> body )
        {
            try
            {
                return body();
            }
            catch ( CliException exception )
            {
                Console.Error.WriteLine( $"gfdstudio: {exception.Message}" );
                return 1;
            }
            catch ( Exception exception )
            {
                Console.Error.WriteLine( $"gfdstudio: {Unwrap( exception ).Message}" );

                if ( Environment.GetEnvironmentVariable( "GFDSTUDIO_TRACE" ) != null )
                    Console.Error.WriteLine( exception );

                return 2;
            }
        }

        /// <summary>
        /// The message worth showing is the innermost one; a reflection or task wrapper only
        /// says that something inside it went wrong.
        /// </summary>
        private static Exception Unwrap( Exception exception )
        {
            while ( exception is TargetInvocationException or AggregateException && exception.InnerException != null )
                exception = exception.InnerException;

            return exception;
        }
    }
}
