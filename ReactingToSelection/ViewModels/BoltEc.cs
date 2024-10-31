using TSD.API.Remoting.Connections;
using static ReactingToSelection.Helpers.UiHelpers;

namespace ReactingToSelection.ViewModels;

public class BoltEc( IBoltEc bolt ) : Bolt( bolt )
{
	#region Properties

	public string TensileStressArea { get; } = GetDisplayString( bolt.TensileStressArea );

	public string YieldStrength { get; } = GetDisplayString( bolt.YieldStrength );

	public string HeadThickness { get; } = GetDisplayString( bolt.HeadThickness );

	#endregion
}
