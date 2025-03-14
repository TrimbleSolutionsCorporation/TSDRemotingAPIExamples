using ExportingFireCheckDataToUDA.Extensions;
using TSD.API.Remoting.Common;
using TSD.API.Remoting.Report;
using TSD.API.Remoting.Structure;

namespace ExportingFireCheckDataToUDA;

/// <summary>
/// Contains methods for collection fire check data
/// </summary>
internal static class DataCollector
{
	#region Constants

	/// <summary>
	/// The prefix of the name of the summary table node
	/// </summary>
	private const string SummaryTableNodeNamePrefix = "Summary";

	/// <summary>
	/// The name of the line with fire ambient temperature results
	/// </summary>
	private const string FireAmbientTemperatureLineName = "Fire: Ambient Temperature";

	/// <summary>
	/// The format of the name of the line with fire resistance check results
	/// </summary>
	private const string FireResistanceCheckLineNameFormat = "Fire: Resistance Check : Span - {0}";

	/// <summary>
	/// The name of the item with load reduction factor
	/// </summary>
	private const string LoadReductionFactorItemName = "Load reduction factor, \\eta_{fi}";

	#endregion

	#region Methods

	/// <summary>
	/// Populates the given collection of fire check data
	/// </summary>
	/// <param name="dataCollection">The collection of empty fire check data to populate with data</param>
	public static async Task<IReadOnlyList<FireCheckData>> PopulateFireCheckDataAsync( IReadOnlyCollection<FireCheckData> dataCollection )
	{
		var memberSpanCount = dataCollection.Count;
		var decimalPlaces = (int) Math.Ceiling( Math.Log10( memberSpanCount ) );

		Console.WriteLine( "Collecting fire check data" );

		var dataList = new System.Collections.Concurrent.ConcurrentBag<FireCheckData>();

		var groupedDataCollection = dataCollection
			.GroupBy( d => d.Member )
			.ToList();

		var processedTasks = 0;
		var @lock = new object();

		await Task.Run( () => Parallel.ForEachAsync( groupedDataCollection, async ( group, cancellationToken ) =>
		{
			var populatedMemberFireCheckDataCollection = await PopulateFireCheckDataAsync( group.Key, group.ToList(), cancellationToken );

			foreach( var fireCheckData in populatedMemberFireCheckDataCollection )
			{
				dataList.Add( fireCheckData );
			}

			var message = populatedMemberFireCheckDataCollection.FirstOrDefault() is { MemberSpan: { } memberSpan }
				? $"Collected data from {memberSpan.Name}"
				: "Failed to collect data!";

			lock( @lock )
			{
				Console.WriteLine( $"{ProgressIndicator.GetText( decimalPlaces, processedTasks, memberSpanCount )} {message}" );

				processedTasks++;
			}
		} ) );

		return dataList.ToList();
	}

	/// <summary>
	/// Returns populated version of the given fire check data
	/// </summary>
	/// <param name="member">The member to collect fire check data for</param>
	/// <param name="dataCollection">The collection fire check data to populate</param>
	/// <param name="cancellationToken">The cancellation token</param>
	private static async Task<IReadOnlyCollection<FireCheckData>> PopulateFireCheckDataAsync( IMember member, IReadOnlyCollection<FireCheckData> dataCollection, CancellationToken cancellationToken )
	{
		var rootNode = await member.GetCalculationsAsync( CheckResultType.Static, cancellationToken: cancellationToken );

		if( GetSummaryTable( rootNode ) is not { } summaryTable )
			return [];

		if( summaryTable.GetLine( FireAmbientTemperatureLineName ) is not { } fireAmbientTemperatureLine )
			return [];

		if( GetCombinationReferenceIndex( fireAmbientTemperatureLine ) is not { } combinationReferenceIndex )
			return [];

		if( GetAmbientUtilizationRatio( fireAmbientTemperatureLine ) is not { } ambientUtilizationRatio )
			return [];

		if( GetLoadReductionFactor( member, fireAmbientTemperatureLine.Table ) is not { } loadReductionFactor )
			return [];

		var populatedDataList = new List<FireCheckData>();

		foreach( var data in dataCollection )
		{
			double? criticalTemperature = null;

			if( summaryTable.GetLine( string.Format( FireResistanceCheckLineNameFormat, data.MemberSpan.Index + 1 ) ) is { } fireResistanceCheckLine )
			{
				criticalTemperature = GetCriticalTemperature( fireResistanceCheckLine );
			}

			var populatedData = data with
			{
				CriticalTemperature = criticalTemperature,
				LoadReductionFactor = loadReductionFactor,
				AmbientUtilizationRatio = ambientUtilizationRatio,
				CombinationReferenceIndex = combinationReferenceIndex,
			};

			populatedDataList.Add( populatedData );
		}

		return populatedDataList;
	}

	/// <summary>
	/// Returns the summary table from the given node
	/// </summary>
	/// <param name="parentNode">The node to retrieve summary table from</param>
	private static IReportTable? GetSummaryTable( ICalculationsNode parentNode )
		=> parentNode.GetChildNode( n => n.Name.StartsWith( SummaryTableNodeNamePrefix ) )?.Items.OfType<IReportTable>().FirstOrDefault();

	/// <summary>
	/// Returns the combination reference index from the given line
	/// </summary>
	/// <param name="line">The line to retrieve combination reference index from</param>
	private static int? GetCombinationReferenceIndex( IReportTableLine line )
	{
		if( line.Items.Count < 2 )
			return null;

		if( line.Items[1] is not IReportTableLineItemText { Text: { } text } )
			return null;

		if( !int.TryParse( text, out var combinationReferenceIndex ) )
			return null;

		return combinationReferenceIndex;
	}

	/// <summary>
	/// Returns the ambient utilization ratio from the given line
	/// </summary>
	/// <param name="line">The line to retrieve ambient utilization ratio from</param>
	private static double? GetAmbientUtilizationRatio( IReportTableLine line )
	{
		if( line.Items.Count < 6 )
			return null;

		if( line.Items[5] is not IReportTableLineItemDoubleValue { Value: var ambientUtilizationRatio } )
			return null;

		return ambientUtilizationRatio;
	}

	/// <summary>
	/// Returns the load reduction factor from the given table
	/// </summary>
	/// <param name="member">The member to retrieve load reduction factor for</param>
	/// <param name="table">The table to retrieve load reduction factor from</param>
	private static double? GetLoadReductionFactor( IMember member, IReportTable? table )
	{
		if( table is null )
			return null;

		return member.Data.GetValueOrDefault()?.Construction.GetValueOrDefault() switch
		{
			MemberConstruction.SteelBeam => GetSteelBeamValue(),
			MemberConstruction.SteelColumn => GetSteelColumnValue(),
			_ => null
		};

		double? GetSteelBeamValue() => table.Lines
			.Select( l => l.Table )
			.OfType<IReportTable>()
			.Select( t => t.GetValue( LoadReductionFactorItemName ) )
			.FirstOrDefault( v => v is not null );

		double? GetSteelColumnValue()
		{
			if( table.GetValue( LoadReductionFactorItemName ) is { } directLoadReductionFactor )
				return directLoadReductionFactor;

			foreach( var directTable in table.Lines.Select( l => l.Table ).OfType<IReportTable>() )
			{
				if( directTable.GetValue( LoadReductionFactorItemName ) is { } shallowNestedLoadReductionFactor )
					return shallowNestedLoadReductionFactor;

				var deepNestedLoadReductionFactor = directTable.Lines
					.Select( l => l.Table )
					.OfType<IReportTable>()
					.Select( t => t.GetValue( LoadReductionFactorItemName ) )
					.FirstOrDefault( v => v is not null );

				if( deepNestedLoadReductionFactor is not null )
					return deepNestedLoadReductionFactor;
			}

			return null;
		}
	}

	/// <summary>
	/// Returns the critical temperature from the given line
	/// </summary>
	/// <param name="line">The line to retrieve critical temperature from</param>
	private static double? GetCriticalTemperature( IReportTableLine line )
	{
		if( line.Items.Count < 4 )
			return null;

		if( line.Items[3] is not IReportTableLineItemDoubleValue { Value: var criticalTemperature } )
			return null;

		return criticalTemperature;
	}

	#endregion
}
