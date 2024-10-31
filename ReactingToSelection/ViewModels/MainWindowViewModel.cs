using System.Windows.Input;
using ReactingToSelection.Helpers;
using ReactingToSelection.Models;
using TSD.API.Remoting;
using TSD.API.Remoting.Connections;

namespace ReactingToSelection.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
	#region Fields

	private IApplication? _application;
	private Connection? _connection;
	private IConnection? _selectedConnection;

	#endregion

	#region Constructors

	public MainWindowViewModel()
	{
		RefreshCommand = new RelayCommand( RefreshAsync );

		async void RefreshAsync()
		{
			if( _application is not null )
			{
				_application.SelectionChanged -= OnSelectionChanged;
			}

			_application = (await ApplicationFactory.GetRunningApplicationsAsync()).FirstOrDefault();

			if( _application is null )
				return;

			_application.SelectionChanged += OnSelectionChanged;

			if( await _application.GetDocumentAsync() is not { } document )
				return;

			await Model.FetchModelAsync( document );

			OnSelectionChanged( null, EventArgs.Empty );
		}
	}

	#endregion

	#region Properties

	public Connection? Connection
	{
		get => _connection;
		set => SetProperty( nameof( Connection ), ref _connection, value );
	}

	public ListWithNotify<IConnection> Connections { get; } = [];

	public IConnection? SelectedConnection
	{
		get => _selectedConnection;
		set => SetProperty( nameof( SelectedConnection ), ref _selectedConnection, value, () => Connection = new Connection( value ?? Connections.FirstOrDefault() ) );
	}

	public ICommand RefreshCommand { get; }

	#endregion

	#region Event Handlers

	private void OnSelectionChanged( object? sender, EventArgs e )
	{
		GetConnectionDetailsForSelectionAsync().FireAndForgetSafeAsync();

		async Task GetConnectionDetailsForSelectionAsync()
		{
			Connections.ReplaceContentAndNotify( await ConnectionFetcher.GetSelectedConnectionsAsync() );

			SelectedConnection = Connections.FirstOrDefault();
		}
	}

	#endregion
}
