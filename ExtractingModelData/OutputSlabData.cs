using System.Text;
using System.Threading.Tasks;
using TSD.API.Remoting.Structure;

namespace ExtractingModelData
{
	internal class OutputSlabData
	{
		#region Methods

		public static async Task CreateSlabDataOutputAsync( IModel model, StringBuilder stringBuilder )
		{
			var slabs = await model.GetSlabsAsync();
			var slabItems = await model.GetSlabItemsAsync();

			stringBuilder.AppendLine(
				"Slab name," +
				"Slab type," +
				"Material," );

			foreach( var slab in slabs )
			{
				stringBuilder.AppendLine(
					$"{slab.Name}," +
					$"{slab.SlabData.Value.SlabType.Value}," +
					$"{slab.SlabData.Value.Material.Value.Name}," );
			}

			stringBuilder.AppendLine();

			stringBuilder.AppendLine(
				"Slab item name," +
				"Slab depth (mm)," );

			foreach( var slabItem in slabItems )
			{
				stringBuilder.AppendLine(
					$"{slabItem.Name}," +
					$"{slabItem.SlabItemData.Value.Depth.Value}," );
			}

			stringBuilder.AppendLine();
		}

		#endregion
	}
}
