using ReactingToSelection.Helpers;
using TSD.API.Remoting.Connections;

namespace ReactingToSelection.ViewModels;

public class BasePlateAsymmetricBoltLayout( IBasePlateAsymmetricBoltLayout layout ) : BasePlateBoltLayout
{
	#region Properties

	public BasePlateBoltLayout? FlangeA { get; } = Create( layout.FlangeA.GetValueOrDefault() );

	public BasePlateBoltLayout? FlangeC { get; } = Create( layout.FlangeC.GetValueOrDefault() );

	#endregion
}
