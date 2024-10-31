using TSD.API.Remoting.Common;
using TSD.API.Remoting.Common.Interfaces;
using TSD.API.Remoting.Connections;
using TSD.API.Remoting.Loading;
using TSD.API.Remoting.Solver;
using TSD.API.Remoting.Structure;

namespace ReactingToSelection.Models;

/// <summary>
/// Represents a class that fetches connection details
/// </summary>
public static class ConnectionFetcher
{
	#region Methods

	/// <summary>
	/// Returns the selected connections
	/// </summary>
	public static async Task<IEnumerable<IConnection>> GetSelectedConnectionsAsync()
	{
		var selection = await Model.Instance.GetSelectedEntitiesAsync();

		var connectionIndices = selection
			.OfType<ISelectedEntity>()
			.Where( x => x.Entity.Type == EntityType.Connection )
			.Select( e => e.Entity.Index )
			.ToList();

		if( connectionIndices.Count == 0 )
			return Enumerable.Empty<IConnection>();

		return await Model.Instance.GetConnectionsAsync( connectionIndices );
	}

	/// <summary>
	/// Returns the connected members of the connection
	/// </summary>
	/// <param name="connection">The connection to fetch connected members of</param>
	public static Task<IEnumerable<IMember>> GetConnectedMembersAsync( IConnection connection ) => connection.GetConnectedMembersAsync();

	/// <summary>
	/// Returns the support reaction of the connection
	/// </summary>
	/// <param name="connection">The connection to fetch support reaction of</param>
	public static async Task<IForce3DGlobal> GetSupportReactionAsync( IConnection connection )
	{
		var combination = (await Model.Instance.GetCombinationsAsync()).First();
		var constructionPoint = (await Model.Instance.GetConstructionPointsAsync( [connection.ConstructionPointIndex.Value] )).First();
		int solverNodeIndex = (await Model.Instance.GetConstructionPointGroupsAsync( [constructionPoint.GroupIndex.Value] )).First().SolverNodeIndex.Value;
		var solverNode = (await (await Model.Instance.GetSolverModelsAsync( [AnalysisType.FirstOrderLinear] )).First().GetNodesAsync( [solverNodeIndex] )).First();

		return await solverNode.GetSupportReactionAsync( combination.Id, reduceAxialLoad: false );
	}

	#endregion
}
