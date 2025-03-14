using System.CommandLine;

namespace ExportingFireCheckDataToUDA;

/// <summary>
/// Contains entry point of the application
/// </summary>
internal static class Program
{
	#region Methods

	/// <summary>
	/// Executes the application logic
	/// </summary>
	/// <param name="args">The command line arguments</param>
	/// <returns>The exit code indicating how the application terminated</returns>
	private static int Main( string[] args ) => Commands.RootCommand.Invoke( args );

	#endregion
}
