using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TSD.API.Remoting.Common;
using TSD.API.Remoting.Materials;
using TSD.API.Remoting.Sections;
using TSD.API.Remoting.Structure;

namespace ExtractingModelData
{
	internal class OutputMemberData
	{
		#region Methods

		public static async Task CreateMemberDataOutputAsync( IModel model, StringBuilder stringBuilder )
		{
			var members = (await model.GetMembersAsync()).ToList();

			await OutputMemberDataForMaterialAsync( model, members, stringBuilder, MaterialType.Steel );
			await OutputMemberDataForMaterialAsync( model, members, stringBuilder, MaterialType.Concrete );
			await OutputMemberDataForMaterialAsync( model, members, stringBuilder, MaterialType.Timber );
		}

		public static async Task OutputMemberDataForMaterialAsync( IModel model, IEnumerable<IMember> members, StringBuilder stringBuilder, MaterialType materialType )
		{
			stringBuilder.AppendLine( $"{materialType} members" );

			stringBuilder.AppendLine(
				"Span name," +
				"Section," +
				"Material," +
				"Start X (mm)," +
				"Start Y (mm)," +
				"Start Z (mm)," +
				"End X (mm)," +
				"End Y (mm)," +
				"End Z (mm)," +
				"Ratio (Static)," +
				"Status (Static)," );

			var constructionPoints = (await model.GetConstructionPointsAsync()).ToList();

			var membersToReport = members.Where( m => m.Data.Value.MaterialType.Value == materialType && IsOutputMemberType( m.Data.Value.MemberType.Value ) );

			foreach( var member in membersToReport )
			{
				foreach( var span in await member.GetSpanAsync( new[] { 0, member.SpanCount.Value - 1 } ) )
				{
					var section = span.ElementSection.Value as IMemberSection;
					var startPoint = constructionPoints.First( c => c.Index == span.StartMemberNode.ConstructionPointIndex.Value );
					var endPoint = constructionPoints.First( c => c.Index == span.EndMemberNode.ConstructionPointIndex.Value );

					stringBuilder.AppendLine(
						$"{span.Name}," +
						$"{section?.PhysicalSection.Value.LongName}," +
						$"{span.Material.Value.Name}," +
						$"{startPoint.Coordinates.Value.X}," +
						$"{startPoint.Coordinates.Value.Y}," +
						$"{startPoint.Coordinates.Value.Z}," +
						$"{endPoint.Coordinates.Value.X}," +
						$"{endPoint.Coordinates.Value.Y}," +
						$"{endPoint.Coordinates.Value.Z}," +
						$"{span.CheckResults[CheckResultType.Static].Value.UtilizationRatio.Value}," +
						$"{span.CheckResults[CheckResultType.Static].Value.CheckStatus.Value}," );
				}
			}

			stringBuilder.AppendLine( string.Empty );
		}

		public static bool IsOutputMemberType( MemberType memberType ) => memberType != MemberType.WallBeamElement
			&& memberType != MemberType.WallColumnElement
			&& memberType != MemberType.WallMeshBeamElement
			&& memberType != MemberType.BearingWallBeam
			&& memberType != MemberType.BearingWallColumn;

		#endregion
	}
}
