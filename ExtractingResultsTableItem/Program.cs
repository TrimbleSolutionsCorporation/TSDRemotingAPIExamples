using System;
using System.Linq;
using System.Threading.Tasks;
using TSD.API.Remoting;
using TSD.API.Remoting.Structure;

namespace ExtractingResultsTableItem
{
	static class Program
	{
		[STAThread]
		public static async Task Main()
		{
			// Get the first running TSD instance found
			using( var tsdInstance = await ApplicationFactory.GetFirstRunningApplicationAsync() )
			{
				if( tsdInstance == null )
				{
					Console.WriteLine( "No running instances of TSD found!" );

					return;
				}

				// Output the version number of TSD
				Console.WriteLine( await tsdInstance.GetVersionStringAsync() );

				// Get the active document from the running instance of TSD
				var document = await tsdInstance.GetDocumentAsync();

				if( document == null )
				{
					Console.WriteLine( "No document was found in the TSD instance!" );

					return;
				}

				// Get the model from the document
				var model = await document.GetModelAsync();

				if( model == null )
				{
					Console.WriteLine( "No model was found in the document!" );

					return;
				}

				var member = await GetMemberForOutput( model );

				if( member == null )
				{
					Console.WriteLine( "No member found." );

					return;
				}

				var lineItemString = GetLineItemStringFromUserInput();

				var resultValuesOutputter = new ResultsValuesOutputter( member, lineItemString, tsdInstance );
				await resultValuesOutputter.Output();

				Console.ReadLine();
			}
		}

		private static async Task<IMember> GetMemberForOutput( IModel model )
		{
			var members = await model.GetMembersAsync();

			IMember member = null;

			do
			{
				Console.WriteLine( "Enter member reference:" );

				string memberName = Console.ReadLine();

				member = members.FirstOrDefault( m => m.Name == memberName );

				if( member == null )
				{
					Console.WriteLine( "Member named " + memberName + " not found" );
				}
			} while( member == null );

			return member;
		}

		private static string GetLineItemStringFromUserInput()
		{
			string lineItemString;

			do
			{
				Console.WriteLine( "Enter string to use for results table search:" );
				lineItemString = Console.ReadLine();
			} while( lineItemString.Length == 0 );

			return lineItemString;
		}
	}
}
