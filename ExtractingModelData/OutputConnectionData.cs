using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TSD.API.Remoting.Structure;

namespace ExtractingModelData
{
	internal class OutputConnectionData
	{
		#region Methods

		public static async Task CreateConnectionDataOutputAsync( IModel model, StringBuilder stringBuilder )
		{
			stringBuilder.AppendLine(
				"Connection name," +
				"Connection type," +
				"X (mm)," +
				"Y (mm)," +
				"Z (mm)," +
				"Connected members," );

			var constructionPoints = (await model.GetConstructionPointsAsync()).ToList();
			var connections = await model.GetConnectionsAsync();

			foreach( var connection in connections )
			{
				var constructionPoint = constructionPoints.FirstOrDefault( c => c.Index == connection.ConstructionPointIndex.Value );
				var connectedMembers = await connection.GetConnectedMembersAsync();

				stringBuilder.AppendLine(
					$"{connection.Name}," +
					$"{connection.ConnectionType.Value}," +
					$"{constructionPoint?.Coordinates.Value.X}," +
					$"{constructionPoint?.Coordinates.Value.Y}," +
					$"{constructionPoint?.Coordinates.Value.Z}," +
					$"{string.Join( ", ", connectedMembers.Select( m => m.Name ) )}," );
			}

			stringBuilder.AppendLine( "" );
		}

		#endregion
	}
}
