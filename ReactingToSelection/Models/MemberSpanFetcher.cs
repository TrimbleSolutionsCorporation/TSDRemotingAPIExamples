using TSD.API.Remoting.Connections;
using TSD.API.Remoting.Loading;
using TSD.API.Remoting.Solver;
using TSD.API.Remoting.Structure;

namespace ReactingToSelection.Models;

/// <summary>
/// Represents a class that fetches member span details
/// </summary>
public static class MemberSpanFetcher
{
	#region Methods

	/// <summary>
	/// Returns the connected member details
	/// </summary>
	/// <param name="connection">The connection to fetch connected member details of</param>
	/// <param name="member">The member to fetch connected member details of</param>
	/// <param name="loadingCase">The loading case to fetch connected member details of</param>
	/// <param name="analysisType">The analysis type to fetch connected member details of</param>
	public static async Task<(ISpanReleases Releases, IForce3DLocal? EndForce)?> GetConnectedMemberDetailsAsync( IConnection connection, IMember member,
		ILoadingCase loadingCase, AnalysisType analysisType )
	{
		int connectionConstructionPointIndex = connection.ConstructionPointIndex.Value;
		var spans = await member.GetSpanAsync();

		foreach( var span in spans )
		{
			int startConstructionPointIndex = span.StartMemberNode.ConstructionPointIndex.Value;
			int endConstructionPointIndex = span.EndMemberNode.ConstructionPointIndex.Value;

			if( connectionConstructionPointIndex == startConstructionPointIndex )
				return (span.StartReleases.Value, await span.GetEndForceAsync( 0, analysisType, loadingCase.Id, LoadingResultType.Base ));

			if( connectionConstructionPointIndex == endConstructionPointIndex )
				return (span.EndReleases.Value, await span.GetEndForceAsync( 1, analysisType, loadingCase.Id, LoadingResultType.Base ));

			var constructionPoints = await Model.Instance.GetConstructionPointsAsync( [connectionConstructionPointIndex, startConstructionPointIndex, endConstructionPointIndex] );
			var groupIndices = constructionPoints.Select( cp => cp.GroupIndex ).ToList();

			if( groupIndices[0].Value == groupIndices[1].Value )
				return (span.StartReleases.Value, await span.GetEndForceAsync( 0, analysisType, loadingCase.Id, LoadingResultType.Base ));

			if( groupIndices[0].Value == groupIndices[2].Value )
				return (span.EndReleases.Value, await span.GetEndForceAsync( 1, analysisType, loadingCase.Id, LoadingResultType.Base ));
		}

		return null;
	}

	#endregion
}
