namespace ReactingToSelection.Helpers;

/// <summary>
/// Contains extension methods for the <see cref="Task"/> class
/// </summary>
public static class TaskExtensions
{
	#region Methods

	/// <summary>
	/// Provides a safe way to call asynchronous method without waiting for it's result
	/// </summary>
	/// <param name="task">The task to be run</param>
	public static async void FireAndForgetSafeAsync( this Task task )
	{
		try
		{
			await task;
		}
		catch( Exception )
		{
			// Intentionally ignored
		}
	}

	#endregion
}
