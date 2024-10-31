using ReactingToSelection.Helpers;
using TSD.API.Remoting.Connections;

namespace ReactingToSelection.ViewModels;

public class BasePlateConcreteData( IBasePlateConcreteData? data ) : ViewModelBase
{
	#region Properties

	public string Width { get; } = (data?.Width).GetDisplayString();

	public string Length { get; } = (data?.Length).GetDisplayString();

	public string Depth { get; } = (data?.Depth).GetDisplayString();

	public string Material { get; } = (data?.Material).GetDisplayString();

	public string GroutSpace { get; } = (data?.GroutSpace).GetDisplayString();

	#endregion
}
