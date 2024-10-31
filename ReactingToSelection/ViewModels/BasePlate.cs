using ReactingToSelection.Helpers;
using TSD.API.Remoting.Connections;
using TSD.API.Remoting.Loading;

namespace ReactingToSelection.ViewModels;

public class BasePlate( IBasePlate basePlate, IForce3DGlobal supportReaction ) : ConnectionDetails
{
	#region Properties

	public BasePlateProperties Properties { get; } = new( basePlate );

	public BasePlateConcreteData ConcreteData { get; } = new( basePlate.ConcreteData.GetValueOrDefault() );

	public BasePlateBoltData BoltData { get; } = new( basePlate.BoltData.GetValueOrDefault() );

	public BasePlateBoltLayout? BoltLayout { get; } = BasePlateBoltLayout.Create( basePlate.BoltLayout.GetValueOrDefault() );

	public BasePlateWelds Welds { get; } = new( basePlate.Welds.GetValueOrDefault() );

	public WeldedWasher WeldedWasher { get; } = new( basePlate.WeldedWasher.GetValueOrDefault() );

	public SupportReaction SupportReaction { get; } = new( supportReaction );

	#endregion
}
