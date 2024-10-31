using ReactingToSelection.Helpers;
using ReactingToSelection.Models;
using TSD.API.Remoting.Common;
using TSD.API.Remoting.Common.Properties;
using TSD.API.Remoting.Connections;
using TSD.API.Remoting.Structure;

namespace ReactingToSelection.ViewModels;

public class Connection : ViewModelBase
{
	#region Fields

	private IConnection? _connection;
	private ConnectionDetails? _connectionDetails;
	private IMember? _selectedConnectedMember;
	private ConnectedMember? _connectedMember;

	#endregion

	#region Constructors

	public Connection( IConnection? connection )
	{
		SetSelectedConnection( connection );

		FetchConnectedMembersAsync().FireAndForgetSafeAsync();
		FetchConnectionDetailsAsync().FireAndForgetSafeAsync();
	}

	#endregion

	#region Properties

	public string Name => _connection?.Name ?? string.Empty;

	public string ConnectionType => _connection?.ConnectionType.Value.ToString() ?? string.Empty;

	public string ConstructionPointIndex => _connection?.ConstructionPointIndex.Value.ToString() ?? string.Empty;

	public IProperty<double>? UtilizationRatio => _connection?.UtilizationRatio;

	public IPropertyWithValidValues<CheckStatus>? CheckStatus => _connection?.CheckStatus;

	public ListWithNotify<IMember> ConnectedMembers { get; } = [];

	public ConnectionDetails? ConnectionDetails
	{
		get => _connectionDetails;
		private set => SetProperty( nameof( ConnectionDetails ), ref _connectionDetails, value );
	}

	public IMember? SelectedConnectedMember
	{
		get => _selectedConnectedMember;
		set
		{
			if( _selectedConnectedMember?.Name == value?.Name )
				return;

			SetProperty( nameof( SelectedConnectedMember ), ref _selectedConnectedMember, value );

			if( _selectedConnectedMember is not { } selectedConnectedMember )
				return;

			if( _connection is not { } connection )
				return;

			ConnectedMember ??= new ConnectedMember( selectedConnectedMember, connection );
			ConnectedMember.SetSelectedMember( selectedConnectedMember, connection );
		}
	}

	public ConnectedMember? ConnectedMember
	{
		get => _connectedMember;
		private set => SetProperty( nameof( ConnectedMember ), ref _connectedMember, value );
	}

	#endregion

	#region Methods

	private void SetSelectedConnection( IConnection? connection )
	{
		if( connection == _connection )
			return;

		_connection = connection;

		OnPropertyChanged( nameof( Name ) );
		OnPropertyChanged( nameof( ConnectionType ) );
		OnPropertyChanged( nameof( ConstructionPointIndex ) );
		OnPropertyChanged( nameof( UtilizationRatio ) );
		OnPropertyChanged( nameof( CheckStatus ) );
	}

	private async Task FetchConnectedMembersAsync()
	{
		if( _connection is null )
			return;

		var connectedMembers = (await ConnectionFetcher.GetConnectedMembersAsync( _connection )).ToList();

		ConnectedMembers.ReplaceContentAndNotify( connectedMembers );

		SelectedConnectedMember = connectedMembers.FirstOrDefault();
	}

	private async Task FetchConnectionDetailsAsync()
	{
		if( _connection is null )
			return;

		var supportReaction = await ConnectionFetcher.GetSupportReactionAsync( _connection );

		ConnectionDetails = ConnectionDetails.Create( _connection, supportReaction );
	}

	#endregion
}
