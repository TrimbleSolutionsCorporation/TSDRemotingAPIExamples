using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TSD.API.Remoting.Loading;
using TSD.API.Remoting.Solver;
using TSD.API.Remoting.Structure;

namespace CreatingAndAnalyzingModel
{
	/// <summary>
	/// Contains methods to output results
	/// </summary>
	public class ResultsOutput
	{
		#region Constants

		internal const string OutputFileName = "TSDRemotingAPIExampleOutput.csv";

		#endregion

		#region Methods

		public async Task OutputResults( TSD.API.Remoting.Structure.IModel model, IEnumerable<Guid> solvedLoading, AnalysisType requestedAnalysisType )
		{
			var members = await model.GetMembersAsync( null );

			// Get all loadcases in the model. Pass null as the parameter to get all loadcases; alternatively a sequence of indices can be passed to get specific loadcases
			var loadcases = await model.GetLoadcasesAsync( null );

			// Get the solved loadcases by cross referencing with the sequence of solved guids obtained from the solver model
			var solvedLoadcases = loadcases.Where( l => solvedLoading.Contains( l.Id ) ).ToList();

			var stringBuilder = new StringBuilder();

			await GetBeamDeflectionResults( model, requestedAnalysisType, members, stringBuilder );

			stringBuilder.AppendLine();

			await GetColumnResults( model, requestedAnalysisType, members, stringBuilder );

			stringBuilder.AppendLine();

			await GetMidSpanResults( model, requestedAnalysisType, members, stringBuilder );

			// Write the .csv file with the output data
			File.WriteAllText( Path.Combine( Environment.GetFolderPath( Environment.SpecialFolder.MyDocuments ), OutputFileName ), stringBuilder.ToString() );

			Console.WriteLine( "Results output complete" );
		}

		private static async Task GetBeamDeflectionResults( TSD.API.Remoting.Structure.IModel model, AnalysisType requestedAnalysisType, IEnumerable<IMember> members, StringBuilder stringBuilder )
		{
			// Specify the loading value options, these will determine the type of data we are requesting. Please see the constructor documentation for additional options.
			var momentValueOption = new LoadingValueOptions( LoadingValueType.Moment, LoadingDirection.Major );
			var deflectionValueOption = new LoadingValueOptions( LoadingValueType.Deflection, LoadingDirection.Major );

			var combinations = await model.GetCombinationsAsync();

			// Append the first line which will contain the column headers
			stringBuilder.AppendLine( "Maximum deflection by beam with critical combination with maximum moment in critical combination" );
			stringBuilder.AppendLine(
					$"{nameof( BeamDeflectionResults.MemberName )}," +
					$"{nameof( BeamDeflectionResults.MemberTotalLength )}," +
					$"{nameof( BeamDeflectionResults.CriticalCombinationName )}," +
					$"{nameof( BeamDeflectionResults.MaximumDeflectionValue )}," +
					$"{nameof( BeamDeflectionResults.MaximumDeflectionSpanIndex )}," +
					$"{nameof( BeamDeflectionResults.MaximumDeflectionPosition )}," +
					$"{nameof( BeamDeflectionResults.MaximumMomentValue )}," +
					$"{nameof( BeamDeflectionResults.MaximumMomentSpanIndex )}," +
					$"{nameof( BeamDeflectionResults.MaximumMomentPosition )}," );

			// Loop through the beams
			foreach( var member in members.Where( m => m.MemberType.Value == MemberType.Beam ) )
			{
				double totalLength = 0.0;

				foreach( var span in await member.GetSpanAsync( new[] { 0, member.SpanCount.Value - 1 } ) )
				{
					totalLength += span.Length.Value;
				}

				// Create the output data object and set some member properties
				var outputMemberData = new BeamDeflectionResults
				{
					MemberName = member.Name,
					MemberTotalLength = totalLength,
				};

				// We will define the critical member loading as the solved loadcase which causes the greatest maximum deflection, considering all spans of the member
				IMemberLoading criticalMemberLoading = null;

				foreach( var combination in combinations )
				{
					// Get the member loading for the combination
					var memberLoading = await member.GetLoadingAsync( combination.Id, requestedAnalysisType, LoadingResultType.Base );

					if( memberLoading == null )
					{
						Console.WriteLine( "The member loading could not be found!" );

						continue;
					}

					// Get the interest points at which the maximum deflection in the major axis occurs
					var maximumDeflectionPointsOfInterest = await memberLoading.GetPointsOfInterest( deflectionValueOption, PointOfInterestType.Maximum );

					// Loop through the points of interest
					foreach( var maximumDeflectionPointOfInterest in maximumDeflectionPointsOfInterest )
					{
						// Get the loading values for deflection at the position and span defined by point of interest
						var maximumDeflectionLoadingValues = await memberLoading.GetValueAsync( deflectionValueOption, maximumDeflectionPointOfInterest.SpanIndex, maximumDeflectionPointOfInterest.Position );

						// Get the maximum of all loading values at this point of interest
						double spanMaximumDeflection = maximumDeflectionLoadingValues.Max( lv => lv.Value );

						// If this combination/span has a greater maximum deflection than any of the previous combinations, it becomes the critical combination/span
						if( spanMaximumDeflection > outputMemberData.MaximumDeflectionValue )
						{
							criticalMemberLoading = memberLoading;
							outputMemberData.CriticalCombinationName = combination.Name;
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

				// Get the interest points at which the maximum moment in the major axis occurs for the critical combination
				var maximumMomentPointsOfInterest = await criticalMemberLoading.GetPointsOfInterest( momentValueOption, PointOfInterestType.Maximum );

				// Loop through the points of interest
				foreach( var maximumMomentPointOfInterest in maximumMomentPointsOfInterest )
				{
					// Get the loading values for moment at the position and span defined by point of interest
					var maximumMomentLoadingValues = await criticalMemberLoading.GetValueAsync( momentValueOption, maximumMomentPointOfInterest.SpanIndex, maximumMomentPointOfInterest.Position );

					// Get the maximum of all loading values at this point of interest
					double loadcaseMaximumMoment = maximumMomentLoadingValues.Max( lv => lv.Value );

					if( loadcaseMaximumMoment > outputMemberData.MaximumMomentValue )
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
					$"{outputMemberData.CriticalCombinationName}," +
					$"{outputMemberData.MaximumDeflectionValue}," +
					$"{outputMemberData.MaximumDeflectionSpanIndex}," +
					$"{outputMemberData.MaximumDeflectionPosition}," +
					$"{outputMemberData.MaximumMomentValue}," +
					$"{outputMemberData.MaximumMomentSpanIndex}," +
					$"{outputMemberData.MaximumMomentPosition}," );
			}
		}

		private static async Task GetColumnResults( TSD.API.Remoting.Structure.IModel model, AnalysisType requestedAnalysisType, IEnumerable<IMember> members, StringBuilder stringBuilder )
		{
			var combinations = await model.GetCombinationsAsync();

			// Append the first line which will contain the column headers
			stringBuilder.AppendLine( "Maximum major moment minor moment and axial force in each column stack" );
			stringBuilder.AppendLine(
					$"{nameof( ColumnResults.MemberName )}," +
					$"{nameof( ColumnResults.SpanNumber )}," +
					$"{nameof( ColumnResults.SpanLength )}," +
					$"{nameof( ColumnResults.CriticalMajorMomentCombinationName )}," +
					$"{nameof( ColumnResults.MaximumMajorMomentValue )}," +
					$"{nameof( ColumnResults.CriticalMinorMomentCombinationName )}," +
					$"{nameof( ColumnResults.MaximumMinorMomentValue )}," +
					$"{nameof( ColumnResults.CriticalAxialForceCombinationName )}," +
					$"{nameof( ColumnResults.MaximumAxialForceValue )}," );

			// Loop through the columns
			foreach( var member in members.Where( m => m.MemberType.Value == MemberType.Column ) )
			{
				foreach( var span in await member.GetSpanAsync( new[] { 0, member.SpanCount.Value - 1 } ) )
				{
					// Create the output data object and set some member properties
					var outputMemberData = new ColumnResults
					{
						MemberName = member.Name,
						SpanNumber = span.Index + 1,
						SpanLength = span.Length.Value,
					};

					foreach( var combination in combinations )
					{
						// Get the member loading for the solved loadcase
						var memberLoading = await member.GetLoadingAsync( combination.Id, requestedAnalysisType, LoadingResultType.Base );

						if( memberLoading == null )
						{
							Console.WriteLine( member.Name.ToString() + " - " + (span.Index + 1).ToString() + " The member loading could not be found!" );

							continue;
						}

						var majorMomentValueOption = new LoadingValueOptions( LoadingValueType.Moment, LoadingDirection.Major );

						// Get the interest points at which the maximum major moment occurs
						var maximumMajorMomentPointsOfInterest = (await memberLoading.GetPointsOfInterest( majorMomentValueOption, PointOfInterestType.Maximum )).ToList();
						maximumMajorMomentPointsOfInterest.AddRange( await memberLoading.GetPointsOfInterest( majorMomentValueOption, PointOfInterestType.Minimum ) );

						// Loop through the points of interest
						foreach( var maximumMajorMomentPointOfInterest in maximumMajorMomentPointsOfInterest.Where( p => p.SpanIndex == span.Index ) )
						{
							// Get the loading values for moment at the position and span defined by point of interest
							var maximumMomentLoadingValues = await memberLoading.GetValueAsync( majorMomentValueOption, span.Index, maximumMajorMomentPointOfInterest.Position );

							// Get the maximum (numerically) of all loading values at this point of interest
							double loadcaseMaximumMajorMoment = maximumMomentLoadingValues.OrderByDescending( lv => Math.Abs( lv.Value ) ).FirstOrDefault().Value;

							if( Math.Abs( loadcaseMaximumMajorMoment ) > Math.Abs( outputMemberData.MaximumMajorMomentValue ) )
							{
								// Set the moment output data
								outputMemberData.MaximumMajorMomentValue = loadcaseMaximumMajorMoment;
								outputMemberData.CriticalMajorMomentCombinationName = combination.Name;
							}
						}

						var minorMomentValueOption = new LoadingValueOptions( LoadingValueType.Moment, LoadingDirection.Minor );

						// Get the interest points at which the maximum minor moment occurs
						var maximumMinorMomentPointsOfInterest = (await memberLoading.GetPointsOfInterest( minorMomentValueOption, PointOfInterestType.Maximum )).ToList();
						maximumMinorMomentPointsOfInterest.AddRange( await memberLoading.GetPointsOfInterest( minorMomentValueOption, PointOfInterestType.Minimum ) );

						// Loop through the points of interest
						foreach( var maximumMinorMomentPointOfInterest in maximumMinorMomentPointsOfInterest.Where( p => p.SpanIndex == span.Index ) )
						{
							// Get the loading values for moment at the position and span defined by point of interest
							var maximumMomentLoadingValues = await memberLoading.GetValueAsync( minorMomentValueOption, span.Index, maximumMinorMomentPointOfInterest.Position );

							// Get the maximum (numerically) of all loading values at this point of interest
							double loadcaseMaximumMinorMoment = maximumMomentLoadingValues.OrderByDescending( lv => Math.Abs( lv.Value ) ).FirstOrDefault().Value;

							if( Math.Abs( loadcaseMaximumMinorMoment ) > Math.Abs( outputMemberData.MaximumMinorMomentValue ) )
							{
								// Set the moment output data
								outputMemberData.MaximumMinorMomentValue = loadcaseMaximumMinorMoment;
								outputMemberData.CriticalMinorMomentCombinationName = combination.Name;
							}
						}

						var axialForceValueOption = new LoadingValueOptions( LoadingValueType.Force, LoadingDirection.Axial );

						// Get the interest points at which the maximum axial force occurs
						var maximumAxialForcePointsOfInterest = (await memberLoading.GetPointsOfInterest( axialForceValueOption, PointOfInterestType.Maximum )).ToList();
						maximumAxialForcePointsOfInterest.AddRange( await memberLoading.GetPointsOfInterest( axialForceValueOption, PointOfInterestType.Minimum ) );

						// Loop through the points of interest
						foreach( var maximumAxialForcePointOfInterest in maximumAxialForcePointsOfInterest.Where( p => p.SpanIndex == span.Index ) )
						{
							// Get the loading values for force at the position and span defined by point of interest
							var maximumAxialForceLoadingValues = await memberLoading.GetValueAsync( axialForceValueOption, span.Index, maximumAxialForcePointOfInterest.Position );

							// Get the maximum (numerically) of all loading values at this point of interest
							double loadcaseMaximumAxialForce = maximumAxialForceLoadingValues.OrderByDescending( lv => Math.Abs( lv.Value ) ).FirstOrDefault().Value;

							if( Math.Abs( loadcaseMaximumAxialForce ) > Math.Abs( outputMemberData.MaximumAxialForceValue ) )
							{
								// Set the axial force output data
								outputMemberData.MaximumAxialForceValue = loadcaseMaximumAxialForce;
								outputMemberData.CriticalAxialForceCombinationName = combination.Name;
							}
						}
					}

					// Add a new line containing the data for this member to the output file
					stringBuilder.AppendLine(
						$"{outputMemberData.MemberName}," +
						$"{outputMemberData.SpanNumber}," +
						$"{outputMemberData.SpanLength}," +
						$"{outputMemberData.CriticalMajorMomentCombinationName}," +
						$"{outputMemberData.MaximumMajorMomentValue}," +
						$"{outputMemberData.CriticalMinorMomentCombinationName}," +
						$"{outputMemberData.MaximumMinorMomentValue}," +
						$"{outputMemberData.CriticalAxialForceCombinationName}," +
						$"{outputMemberData.MaximumAxialForceValue}," );
				}
			}
		}

		private static async Task GetMidSpanResults( TSD.API.Remoting.Structure.IModel model, AnalysisType requestedAnalysisType, IEnumerable<IMember> members, StringBuilder stringBuilder )
		{
			// Specify the loading value options, these will determine the type of data we are requesting. Please see the constructor documentation for additional options.
			var momentValueOption = new LoadingValueOptions( LoadingValueType.Moment, LoadingDirection.Major );
			var shearValueOption = new LoadingValueOptions( LoadingValueType.Force, LoadingDirection.Major );
			var axialValueOption = new LoadingValueOptions( LoadingValueType.Force, LoadingDirection.Axial );
			var torsionValueOption = new LoadingValueOptions( LoadingValueType.Moment, LoadingDirection.Axial );
			var deflectionValueOption = new LoadingValueOptions( LoadingValueType.Deflection, LoadingDirection.Major );

			var combinations = await model.GetCombinationsAsync();

			// Append the first line which will contain the column headers
			stringBuilder.AppendLine( "Forces at mid-span" );
			stringBuilder.AppendLine(
					$"{nameof( MidSpanResults.MemberName )}," +
					$"{nameof( MidSpanResults.MidSpanPosition )}," +
					$"{nameof( MidSpanResults.CombinationName )}," +
					$"{nameof( MidSpanResults.MidSpanMajorMomentValue )}," +
					$"{nameof( MidSpanResults.MidSpanMajorShearValue )}," +
					$"{nameof( MidSpanResults.MidSpanAxialValue )}," +
					$"{nameof( MidSpanResults.MidSpanTorsionValue )}," +
					$"{nameof( MidSpanResults.MidSpanDeflectionValue )}," );

			// Loop through the beams and columns
			foreach( var member in members.Where( m => m.MemberType.Value is MemberType.Beam or MemberType.Column ) )
			{
				foreach( var span in await member.GetSpanAsync( new[] { 0, member.SpanCount.Value - 1 } ) )
				{
					double midSpanPosition = span.DesignSegment.Value.Magnitude.Value / 2;

					foreach( var combination in combinations )
					{
						// Get the member loading for the combination
						var memberLoading = await member.GetLoadingAsync( combination.Id, requestedAnalysisType, LoadingResultType.Base );

						if( memberLoading == null )
						{
							Console.WriteLine( "The member loading could not be found!" );

							continue;
						}

						var midSpanMajorMoment = await memberLoading.GetValueAsync( momentValueOption, span.Index, midSpanPosition );
						var midSpanMajorShear = await memberLoading.GetValueAsync( shearValueOption, span.Index, midSpanPosition );
						var midSpanAxialForce = await memberLoading.GetValueAsync( axialValueOption, span.Index, midSpanPosition );
						var midSpanTorsion = await memberLoading.GetValueAsync( torsionValueOption, span.Index, midSpanPosition );
						var midSpanDeflection = await memberLoading.GetValueAsync( deflectionValueOption, span.Index, midSpanPosition );

						// Create the output data object and set some member properties
						var outputMemberData = new MidSpanResults
						{
							MemberName = member.Name,
							SpanIndex = span.Index,
							MidSpanPosition = midSpanPosition,
							CombinationName = combination.UserName.Value,
							MidSpanMajorMomentValue = midSpanMajorMoment.OrderByDescending( m => Math.Abs( m.Value ) ).FirstOrDefault().Value,
							MidSpanMajorShearValue = midSpanMajorShear.OrderByDescending( s => Math.Abs( s.Value ) ).FirstOrDefault().Value,
							MidSpanAxialValue = midSpanAxialForce.OrderByDescending( f => Math.Abs( f.Value ) ).FirstOrDefault().Value,
							MidSpanTorsionValue = midSpanTorsion.OrderByDescending( t => Math.Abs( t.Value ) ).FirstOrDefault().Value,
							MidSpanDeflectionValue = midSpanDeflection.OrderByDescending( d => Math.Abs( d.Value ) ).FirstOrDefault().Value,
						};

						// Add a new line containing the data for this member to the output file
						stringBuilder.AppendLine(
							$"{outputMemberData.MemberName} - {outputMemberData.SpanIndex + 1}," +
							$"{outputMemberData.MidSpanPosition}," +
							$"{outputMemberData.CombinationName}," +
							$"{outputMemberData.MidSpanMajorMomentValue}," +
							$"{outputMemberData.MidSpanMajorShearValue}," +
							$"{outputMemberData.MidSpanAxialValue}," +
							$"{outputMemberData.MidSpanTorsionValue}," +
							$"{outputMemberData.MidSpanDeflectionValue}," );
					}
				}
			}
		}

		#endregion
	}
}
