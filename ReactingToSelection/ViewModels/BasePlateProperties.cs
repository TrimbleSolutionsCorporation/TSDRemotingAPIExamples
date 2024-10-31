using ReactingToSelection.Helpers;
using TSD.API.Remoting.Connections;

namespace ReactingToSelection.ViewModels;

public class BasePlateProperties( IBasePlate basePlate ) : ViewModelBase
{
	#region Properties

	public string Length { get; } = basePlate.Length.GetDisplayString();

	public string Width { get; } = basePlate.Width.GetDisplayString();

	public string Thickness { get; } = basePlate.Thickness.GetDisplayString();

	public string Steel { get; } = basePlate.Steel.GetDisplayString();

	#endregion
}
