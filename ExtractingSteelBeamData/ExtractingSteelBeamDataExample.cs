using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TSD.API.Remoting;
using TSD.API.Remoting.Loading;
using TSD.API.Remoting.Materials;
using TSD.API.Remoting.Solver;
using TSD.API.Remoting.Structure;

namespace ExtractingSteelBeamData
{
	/// <summary>
	/// Contains example code on how to extract steel beam data from a TSD document using the API
	/// </summary>
	internal static class ExtractingSteelBeamDataExample
	{
		#region Nested Types

		/// <summary>
		/// Represents example output data for a typical member, used to write data to a text file
		/// </summary>
		private struct OutputMemberData
		{
			#region Properties

			/// <summary>
			/// Gets or sets the member name
			/// </summary>
			public string MemberName { get; set; }

			/// <summary>
			/// Gets or sets the members total length (includes all spans)
			/// </summary>
			public double MemberTotalLength { get; set; }

			/// <summary>
			/// Gets or sets the name of the critical loadcase
			/// </summary>
			public string CriticalLoadcaseName { get; set; }

			/// <summary>
			/// Gets or sets the value of the maximum deflection
			/// </summary>
			public double MaximumDeflectionValue { get; set; }

			/// <summary>
			/// Gets or sets the span index at which the maximum deflection occurs
			/// </summary>
			public int MaximumDeflectionSpanIndex { get; set; }

			/// <summary>
			/// Gets or sets the position at which the maximum deflection occurs, relative to the appropriate span
			/// </summary>
			public double MaximumDeflectionPosition { get; set; }

			/// <summary>
			/// Gets or sets the value of the maximum moment
			/// </summary>
			public double MaximumMomentValue { get; set; }

			/// <summary>
			/// Gets or sets the span index at which the maximum moment occurs
			/// </summary>
			public int MaximumMomentSpanIndex { get; set; }

			/// <summary>
			/// Gets or sets the position at which the maximum moment occurs, relative to the appropriate span
			/// </summary>
			public double MaximumMomentPosition { get; set; }

			#endregion
		}

		#endregion

		#region Methods

		/// <summary>
		/// Main entry point to the application, executes on startup
		/// </summary>
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

			var connected = tsdInstance.Connected;

			// Get the active document from the running instance of TSD
			var version = await tsdInstance.GetVersionStringAsync();

			Console.WriteLine( version );

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

			// For this example we will just request the first order linear analysis type
			var requestedAnalysisType = AnalysisType.FirstOrderLinear;

			// Get the solver models for the requested analysis types
			var solverModels = await model.GetSolverModelsAsync( new[] { requestedAnalysisType } );

			if( !solverModels.Any() )
			{
				Console.WriteLine( "No solver models found!" );

				return;
			}

			// Get the first order linear solver model
			var firstOrderLinearSolverModel = solverModels.FirstOrDefault();

			if( firstOrderLinearSolverModel == null )
			{
				Console.WriteLine( "No solver models found for the first order linear analysis type!" );

				return;
			}

			// Get the results for the first order linear solver model
			var firstOrderLinearSolverResults = await firstOrderLinearSolverModel.GetResultsAsync();

			if( firstOrderLinearSolverResults == null )
			{
				Console.WriteLine( "No results found for the first order linear analysis type!" );

				return;
			}

			// Get the first order linear analysis results
			var analysis3DResults = await firstOrderLinearSolverResults.GetAnalysis3DAsync();

			if( analysis3DResults == null )
			{
				Console.WriteLine( "No analysis results found!" );

				return;
			}

			// Get the Guids of the solved loading
			var solvedLoadingGuids = await analysis3DResults.GetSolvedLoadingIdsAsync();

			if( !solvedLoadingGuids.Any() )
			{
				Console.WriteLine( "No solved loading guids found!" );

				return;
			}

			// Get all loadcases in the model. Pass null as the parameter to get all loadcases; alternatively a sequence of indices can be passed to get specific loadcases
			var loadcases = await model.GetLoadcasesAsync( null );

			// Get the solved loadcases by cross referencing with the sequence of solved guids obtained from the solver model
			var solvedLoadcases = loadcases.Where( l => solvedLoadingGuids.Contains( l.Id ) ).ToList();

			if( !solvedLoadcases.Any() )
			{
				Console.WriteLine( "No solved loadcases found!" );

				return;
			}

			// Get all members in the model. Pass null as the parameter to get all members; alternatively a sequence of indices can be passed to get specific members
			var members = await model.GetMembersAsync( null );

			if( !members.Any() )
			{
				Console.WriteLine( "No members found in the model!" );

				return;
			}

			// Specify the loading value options, these will determine the type of data we are requesting. Please see the constructor documentation for additional options.
			// We do this before looping through all the members to avoid re-instantiating the classes on every iteration
			var momentValueOption = new LoadingValueOptions( LoadingValueType.Moment, LoadingDirection.Major );
			var deflectionValueOption = new LoadingValueOptions( LoadingValueType.Deflection, LoadingDirection.Major );

			// Instantiate a new string builder, we will use this to help write our output .csv file
			var stringBuilder = new StringBuilder();

			// Append the first line which will contain the column headers
			stringBuilder.AppendLine(
					$"{nameof( OutputMemberData.MemberName )}," +
					$"{nameof( OutputMemberData.MemberTotalLength )}," +
					$"{nameof( OutputMemberData.CriticalLoadcaseName )}," +
					$"{nameof( OutputMemberData.MaximumDeflectionValue )}," +
					$"{nameof( OutputMemberData.MaximumDeflectionSpanIndex )}," +
					$"{nameof( OutputMemberData.MaximumDeflectionPosition )}," +
					$"{nameof( OutputMemberData.MaximumMomentValue )}," +
					$"{nameof( OutputMemberData.MaximumMomentSpanIndex )}," +
					$"{nameof( OutputMemberData.MaximumMomentPosition )}," );

			// Loop through the collection of members where the member is a steel beam
			foreach( var member in members.Where( IsSteelBeam ) )
			{
				// Define the total length as 0.0 initially
				double totalLength = 0.0;

				foreach( var span in await member.GetSpanAsync( new[] { 0, member.SpanCount.Value - 1 } ) )
				{
					// Cumulatively add in the length of each individual span within the member
					totalLength += span.Length.Value;
				}

				// Create the output data object and set some member properties
				var outputMemberData = new OutputMemberData
				{
					MemberName = member.Name,
					MemberTotalLength = totalLength,
				};

				// We will define the critical member loading as the solved loadcase which causes the greatest maximum deflection, considering all spans of the member
				IMemberLoading criticalMemberLoading = null;

				// We will keep track of the maximum deflection in order to determine which loadcase/span has the greatest maximum deflection and is therefore critical
				double maximumDeflection = 0.0;

				// For each member, loop through the collection of solved loadcases
				foreach( var solvedLoadcase in solvedLoadcases )
				{
					// Get the member loading for the solved loadcase
					var memberLoading = await member.GetLoadingAsync( solvedLoadcase.Id, requestedAnalysisType, LoadingResultType.Base );

					if( memberLoading == null )
					{
						Console.WriteLine( "The member loading could not be found!" );

						continue;
					}

					// Get the interest points at which the maximum deflection in the major axis occurs (1 point of interest per span)
					var maximumDeflectionPointsOfInterest = await memberLoading.GetPointsOfInterest( deflectionValueOption, PointOfInterestType.Maximum );

					// For each solved loadcase, loop through the points of interest
					foreach( var maximumDeflectionPointOfInterest in maximumDeflectionPointsOfInterest )
					{
						// Get the loading values for deflection at the position and span defined by point of interest
						var maximumDeflectionLoadingValues = await memberLoading.GetValueAsync( deflectionValueOption, maximumDeflectionPointOfInterest.SpanIndex, maximumDeflectionPointOfInterest.Position );

						// Get the maximum of all loading values at this point of interest
						double spanMaximumDeflection = maximumDeflectionLoadingValues.Max( lv => lv.Value );

						// If this loadcase/span has a greater maximum deflection than any of the previous loadcases, it becomes the critical loadcase/span. 
						// For this example we will ignore the situation where the values spanMaximumDeflection and maximumDeflection are equal.
						if( spanMaximumDeflection > maximumDeflection )
						{
							// If this loadcase is the critical one, update the critical values
							criticalMemberLoading = memberLoading;
							maximumDeflection = spanMaximumDeflection;

							// Set critical loadcase and deflection output data
							outputMemberData.CriticalLoadcaseName = solvedLoadcase.Name;
							outputMemberData.MaximumDeflectionValue = spanMaximumDeflection;
							outputMemberData.MaximumDeflectionSpanIndex = maximumDeflectionPointOfInterest.SpanIndex;
							outputMemberData.MaximumDeflectionPosition = maximumDeflectionPointOfInterest.Position;
						}
					}
				}

				if( criticalMemberLoading == null )
				{
					Console.WriteLine( "No critical loadcase could be determined for this member!" );

					continue;
				}

				// Get the interest points at which the maximum moment in the major axis occurs for the critical loadcase (1 point of interest per span)
				var maximumMomentPointsOfInterest = await criticalMemberLoading.GetPointsOfInterest( momentValueOption, PointOfInterestType.Maximum );

				// We will keep track of the maximum moment in order to determine which span has the greatest maximum moment
				var maximumMoment = 0.0;

				// Loop through the points of interest
				foreach( var maximumMomentPointOfInterest in maximumMomentPointsOfInterest )
				{
					// Get the loading values for moment at the position and span defined by point of interest
					var maximumMomentLoadingValues = await criticalMemberLoading.GetValueAsync( momentValueOption, maximumMomentPointOfInterest.SpanIndex, maximumMomentPointOfInterest.Position );

					// Get the maximum of all loading values at this point of interest
					double loadcaseMaximumMoment = maximumMomentLoadingValues.Max( lv => Math.Abs( lv.Value ) );

					if( loadcaseMaximumMoment > maximumMoment )
					{
						// Set the moment output data
						outputMemberData.MaximumMomentValue = loadcaseMaximumMoment;
						outputMemberData.MaximumMomentSpanIndex = maximumMomentPointOfInterest.SpanIndex;
						outputMemberData.MaximumMomentPosition = maximumMomentPointOfInterest.Position;
					}
				}

				// Add a new line containing the data for this member to the output file
				stringBuilder.AppendLine(
					$"{outputMemberData.MemberName}," +
					$"{outputMemberData.MemberTotalLength}," +
					$"{outputMemberData.CriticalLoadcaseName}," +
					$"{outputMemberData.MaximumDeflectionValue}," +
					$"{outputMemberData.MaximumDeflectionSpanIndex}," +
					$"{outputMemberData.MaximumDeflectionPosition}," +
					$"{outputMemberData.MaximumMomentValue}," +
					$"{outputMemberData.MaximumMomentSpanIndex}," +
					$"{outputMemberData.MaximumMomentPosition}," );
			}

			// Write the .csv file with the output data to the same directory as the model
			File.WriteAllText( $"{Path.GetDirectoryName( document.Path )}/SteelBeamDataExampleOutput.csv", stringBuilder.ToString() );
		}

		/// <summary>
		/// Checks whether the given member is a steel beam
		/// </summary>
		/// <param name="member">The member to check</param>
		/// <returns>True if the member is a steel beam; false otherwise</returns>
		private static bool IsSteelBeam( IMember member ) => member.MemberType.Value == MemberType.Beam && member.MaterialType.Value == MaterialType.Steel;

		#endregion
	}
}
