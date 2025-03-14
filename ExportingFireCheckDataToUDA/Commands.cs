using System.CommandLine;

namespace ExportingFireCheckDataToUDA;

/// <summary>
/// Contains command line commands
/// </summary>
internal static class Commands
{
	#region Fields

	/// <summary>
	/// The option for API port
	/// </summary>
	private static readonly Option<int?> _apiPortOption = new( "--api-port", "The port TSD API is listening on." );

	#endregion

	#region Properties

	/// <summary>
	/// Gets the root command
	/// </summary>
	public static RootCommand RootCommand
	{
		get
		{
			var command = new RootCommand
			{
				_apiPortOption,
			};

			command.SetHandler<int?>( Orchestrator.ExportDataToUdaAsync, _apiPortOption );

			return command;
		}
	}

	#endregion
}
