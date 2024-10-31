using TSD.API.Remoting.Connections;
using static ReactingToSelection.Helpers.UiHelpers;

namespace ReactingToSelection.ViewModels;

public class BoltUs( IBoltUs bolt ) : Bolt( bolt )
{
	#region Properties

	public string Area { get; } = GetDisplayString( bolt.Area );

	public string BearingArea { get; } = GetDisplayString( bolt.BearingArea );

	#endregion
}
