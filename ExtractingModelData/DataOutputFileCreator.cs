using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace ExtractingModelData
{
	internal static class DataOutputFileCreator
	{
		#region Constants

		internal const string OutputFileName = "TsdRemotingApiModelDataOutput.csv";

		#endregion

		#region Methods

		public static async Task OutputDataAsync( StringBuilder stringBuilder )
		{
			// Write the .csv file with the output data
			await File.WriteAllTextAsync( Path.Combine( Environment.GetFolderPath( Environment.SpecialFolder.MyDocuments ), OutputFileName ), stringBuilder.ToString() );

			Console.WriteLine( "Results output complete" );
		}

		#endregion
	}
}
