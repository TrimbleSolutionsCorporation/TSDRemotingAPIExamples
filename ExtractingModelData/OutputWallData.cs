using System.Text;
using System.Threading.Tasks;
using TSD.API.Remoting.Common;
using TSD.API.Remoting.Structure;

namespace ExtractingModelData
{
	internal static class OutputWallData
	{
		#region Methods

		public static async Task CreateWallDataOutputAsync( IModel model, StringBuilder stringBuilder )
		{
			var walls = await model.GetStructuralWallsAsync();

			foreach( var wall in walls )
			{
				stringBuilder.AppendLine( $"Wall name:, {wall.Name}," );

				stringBuilder.AppendLine(
					"Panel number," +
					"Thickness (mm)," +
					"Material," +
					"Ratio (Static)," +
					"Status (Static)," );

				var panels = await wall.GetSpanAsync();

				foreach( var panel in panels )
				{
					stringBuilder.AppendLine(
						$"{panel.Index + 1}," +
						$"{panel.WallPanelData.Value.Thickness.Value}," +
						$"{panel.WallPanelData.Value.Material.Value.Name}," +
						$"{panel.CheckResults[CheckResultType.Static].Value.UtilizationRatio.Value}," +
						$"{panel.CheckResults[CheckResultType.Static].Value.CheckStatus.Value}," );
				}

				stringBuilder.AppendLine();
			}
		}

		#endregion
	}
}
