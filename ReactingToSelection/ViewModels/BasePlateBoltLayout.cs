using ReactingToSelection.Helpers;
using TSD.API.Remoting.Connections;

namespace ReactingToSelection.ViewModels;

public abstract class BasePlateBoltLayout : ViewModelBase
{
	#region Methods

	public static BasePlateBoltLayout? Create( IBasePlateBoltLayout? layout ) => layout switch
	{
		IBasePlateFourBoltLayout fourBoltLayout => new BasePlateFourBoltLayout( fourBoltLayout ),
		IBasePlateSymmetricBoltLayout symmetricBoltLayout => new BasePlateSymmetricBoltLayout( symmetricBoltLayout ),
		IBasePlateAsymmetricBoltLayout asymmetricBoltLayout => new BasePlateAsymmetricBoltLayout( asymmetricBoltLayout ),
		null => null,
		_ => throw new InvalidOperationException( $"Unexpected type of base plate bolt layout {layout.GetType().Name} encountered!" )
	};

	#endregion
}
