using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TSD.API.Remoting;
using TSD.API.Remoting.Solver;

namespace CreatingAndAnalyzingModel
{
	class Program
	{
		public static async Task Main()
		{
			// Get all instances of TSD running on the local machine
			var tsdRunningInstances = await ApplicationFactory.GetRunningApplicationsAsync();

			if( !tsdRunningInstances.Any() )
			{
				Console.WriteLine( "No running instances of TSD found!" );

				return;
			}

			// Get the first running TSD instance found
			var tsdInstance = tsdRunningInstances.First();

			if( !await tsdInstance.InitializationTask )
			{
				Console.WriteLine( "Could not initialize TSD instance" );
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

			var createModelObjects = new CreateModelObjects();
			await createModelObjects.Create( model );

			var createLoading = new CreateLoading();
			await createLoading.Create( model );

			await RunValidation( model );

			var requestedAnalysisType = AnalysisType.FirstOrderLinear;

			var solvedLoading = await RunAnalysis( model, requestedAnalysisType );

			var resultsOutput = new ResultsOutput();
			await resultsOutput.OutputResults( model, solvedLoading, requestedAnalysisType );
		}

		private static async Task RunValidation( TSD.API.Remoting.Structure.IModel model )
		{
			await model.ValidateAsync();

			var validationData = await model.GetValidationDataAsync();

			Console.WriteLine( validationData.OverallStatus.Value.ToString() );
		}

		private static async Task<IEnumerable<Guid>> RunAnalysis( TSD.API.Remoting.Structure.IModel model, AnalysisType requestedAnalysisType )
		{
			var analysedLoading = await model.RunAnalysisAsync( requestedAnalysisType );

			Console.WriteLine( requestedAnalysisType.ToString() + " analysis complete" );

			return analysedLoading;
		}
	}
}
