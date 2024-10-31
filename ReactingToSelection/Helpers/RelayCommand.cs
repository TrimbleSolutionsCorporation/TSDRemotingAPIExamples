using System.Windows.Input;

namespace ReactingToSelection.Helpers;

/// <summary>
/// Represents a command
/// </summary>
/// <param name="action">The action to invoke</param>
internal class RelayCommand( Action action ) : ICommand
{
	#region ICommand

	public bool CanExecute( object? parameter ) => true;

	public void Execute( object? parameter )
	{
		action();
	}

	event EventHandler? ICommand.CanExecuteChanged
	{
		add
		{
			// Intentionally left blank
		}
		remove
		{
			// Intentionally left blank
		}
	}

	#endregion
}
