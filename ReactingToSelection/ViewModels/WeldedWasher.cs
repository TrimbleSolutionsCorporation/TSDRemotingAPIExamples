using ReactingToSelection.Helpers;
using TSD.API.Remoting.Connections;

namespace ReactingToSelection.ViewModels;

public class WeldedWasher( IWeldedWasher? weldedWasher ) : ViewModelBase
{
	#region Properties

	public bool UseWeldedWashers { get; } = (weldedWasher?.UseWeldedWashers).GetValueOrDefault();

	public string SideLength { get; } = (weldedWasher?.SideLength).GetDisplayString();

	public string HoleDiameter { get; } = (weldedWasher?.HoleDiameter).GetDisplayString();

	public string Thickness { get; } = (weldedWasher?.Thickness).GetDisplayString();

	public string Steel { get; } = weldedWasher?.Steel.GetValueOrDefault()?.Name ?? string.Empty;

	#endregion
}
