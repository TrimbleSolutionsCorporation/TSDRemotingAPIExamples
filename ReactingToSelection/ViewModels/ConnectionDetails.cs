using ReactingToSelection.Helpers;
using TSD.API.Remoting.Connections;
using TSD.API.Remoting.Loading;

namespace ReactingToSelection.ViewModels;

public abstract class ConnectionDetails : ViewModelBase
{
	#region Methods

	public static ConnectionDetails? Create( IConnection? connection, IForce3DGlobal supportReaction ) => connection switch
	{
		IBasePlate basePlate => new BasePlate( basePlate, supportReaction ),
		_ => null
	};

	#endregion
}
