using ReactingToSelection.Helpers;
using TSD.API.Remoting.Connections;

namespace ReactingToSelection.ViewModels;

public class BasePlateFourBoltLayout( IBasePlateFourBoltLayout layout ) : BasePlateBoltLayout
{
	#region Properties

	public string EdgeDistance { get; } = layout.EdgeDistance.GetDisplayString();

	public string EndDistance { get; } = layout.EndDistance.GetDisplayString();

	#endregion
}
