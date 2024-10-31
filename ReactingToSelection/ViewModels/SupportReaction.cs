using ReactingToSelection.Helpers;
using TSD.API.Remoting.Loading;

namespace ReactingToSelection.ViewModels;

public class SupportReaction( IForce3DGlobal? supportReaction ) : ViewModelBase
{
	#region Properties

	public string Fx { get; } = UiHelpers.GetDisplayString( supportReaction?.Fx );

	public string Fy { get; } = UiHelpers.GetDisplayString( supportReaction?.Fy  );

	public string Fz { get; } = UiHelpers.GetDisplayString( supportReaction?.Fz );

	public string Mx { get; } = UiHelpers.GetDisplayString( supportReaction?.Mx );

	public string My { get; } = UiHelpers.GetDisplayString( supportReaction?.My );

	public string Mz { get; } = UiHelpers.GetDisplayString( supportReaction?.Mz );

	#endregion
}
