using System.Windows.Controls;
using ReactingToSelection.ViewModels;

namespace ReactingToSelection.Views;

public partial class MainWindow
{
	#region Constructors

	public MainWindow()
	{
		InitializeComponent();

		DataContext = new MainWindowViewModel();
	}

	#endregion

	#region Event Handlers

	private void OnDataGridAutoGeneratingColumn( object sender, DataGridAutoGeneratingColumnEventArgs e )
	{
		e.Column = new DataGridTextColumn
		{
			Header = ConvertPropertyNameToHeader( e.PropertyName ),
			Binding = new System.Windows.Data.Binding( $"{e.PropertyName}.Value" )
		};

		static string ConvertPropertyNameToHeader( string propertyName ) => System.Text.RegularExpressions.Regex.Replace( propertyName, "([a-z])([A-Z])", "$1 $2" );
	}

	#endregion
}
