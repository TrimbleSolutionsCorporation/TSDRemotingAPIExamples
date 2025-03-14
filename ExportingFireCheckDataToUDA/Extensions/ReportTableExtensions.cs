using TSD.API.Remoting.Report;

namespace ExportingFireCheckDataToUDA.Extensions;

/// <summary>
/// Contains extension methods for <see cref="IReportTable"/>
/// </summary>
internal static class ReportTableExtensions
{
	#region Methods

	/// <summary>
	/// Returns a value associated with item with the given name from the specified table
	/// </summary>
	/// <param name="table">The table to retrieve value from</param>
	/// <param name="name">The name of the item to retrieve value of</param>
	public static double? GetValue( this IReportTable table, string name )
	{
		if( table.GetLine( name ) is not { } line )
			return null;

		var itemWithValue = line.Items
			.OfType<IReportTableLineItemCombined>()
			.FirstOrDefault();

		return itemWithValue is { Value: var value } ? value : null;
	}

	/// <summary>
	/// Returns a line containing an item with the given name from the specified table
	/// </summary>
	/// <param name="table">The table to retrieve line from</param>
	/// <param name="name">The name of the item to retrieve the associated line</param>
	public static IReportTableLine? GetLine( this IReportTable table, string name )
		=> table.Lines.FirstOrDefault( l => l.Items.OfType<IReportTableLineItemText>().Any( i => i.Text.Equals( name ) ) );

	#endregion
}
