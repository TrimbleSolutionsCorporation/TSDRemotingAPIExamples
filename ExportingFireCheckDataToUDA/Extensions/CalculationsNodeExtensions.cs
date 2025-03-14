using TSD.API.Remoting.Report;

namespace ExportingFireCheckDataToUDA.Extensions;

/// <summary>
/// Contains extension methods for <see cref="ICalculationsNode"/>
/// </summary>
internal static class CalculationsNodeExtensions
{
	#region Methods

	/// <summary>
	/// Returns a child node of the given node with the specified name
	/// </summary>
	/// <param name="node">The node to query the child nodes of</param>
	/// <param name="name">The name of the node to look for</param>
	/// <returns>Returns a child node of the given node with the specified name (or <see langword="null"/> if no such exists)</returns>
	public static ICalculationsNode? GetChildNode( this ICalculationsNode node, string name )
		=> node.Nodes.SingleOrDefault( n => n.Name.Equals( name, StringComparison.InvariantCultureIgnoreCase ) );

	/// <summary>
	/// Returns a child node of the given node that matches the predicate
	/// </summary>
	/// <param name="node">The node to query the child nodes of</param>
	/// <param name="predicate">The predicate to use to look for the child node</param>
	/// <returns>Returns a child node of the given node that matches the predicate (or <see langword="null"/> if no such exists)</returns>
	public static ICalculationsNode? GetChildNode( this ICalculationsNode node, Func<ICalculationsNode, bool> predicate )
		=> node.Nodes.SingleOrDefault( predicate );

	#endregion
}
