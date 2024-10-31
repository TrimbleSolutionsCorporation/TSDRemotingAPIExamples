using ReactingToSelection.Helpers;
using TSD.API.Remoting.Connections;

namespace ReactingToSelection.ViewModels;

public class AnchorPlate( IAnchorPlate? anchorPlate ) : ViewModelBase
{
	#region Properties

	public string PlateType { get; } = (anchorPlate?.Type).GetDisplayString();

	public string Width { get; } = (anchorPlate?.Width).GetDisplayString();

	public string Thickness { get; } = (anchorPlate?.Thickness).GetDisplayString();

	public string Length { get; } = (anchorPlate?.Length).GetDisplayString();

	public string EdgeDistance { get; } = (anchorPlate?.EdgeDistance).GetDisplayString();

	#endregion
}
