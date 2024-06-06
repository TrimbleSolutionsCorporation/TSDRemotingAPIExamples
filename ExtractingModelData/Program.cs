using System;
using System.Text;
using System.Threading.Tasks;
using TSD.API.Remoting;

namespace ExtractingModelData
{
	internal static class Program
	{
		public static async Task Main()
		{
			using( var tsdInstance = await ApplicationFactory.GetFirstRunningApplicationAsync() )
			{
				if( tsdInstance == null )
				{
					Console.WriteLine( "No running instances of TSD found!" );

					return;
				}

				Console.WriteLine( await tsdInstance.GetVersionStringAsync() );

				var document = await tsdInstance.GetDocumentAsync();

				if( document == null )
				{
					Console.WriteLine( "No document was found in the TSD instance!" );

					return;
				}

				var model = await document.GetModelAsync();

				var stringBuilder = new StringBuilder();

				await OutputMemberData.CreateMemberDataOutputAsync( model, stringBuilder );
				await OutputWallData.CreateWallDataOutputAsync( model, stringBuilder );
				await OutputSlabData.CreateSlabDataOutputAsync( model, stringBuilder );
				await OutputConnectionData.CreateConnectionDataOutputAsync( model, stringBuilder );
				await DataOutputFileCreator.OutputDataAsync( stringBuilder );
			}
		}
	}
}
