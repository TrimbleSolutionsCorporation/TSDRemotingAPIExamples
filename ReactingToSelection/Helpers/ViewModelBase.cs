using System.ComponentModel;

namespace ReactingToSelection.Helpers;

/// <summary>
/// Contains the base implementation of the <see cref="INotifyPropertyChanged"/> interface
/// </summary>
public abstract class ViewModelBase : INotifyPropertyChanged
{
	#region Methods

	/// <summary>
	/// Sets the property value and raises <see cref="PropertyChanged"/> event (if the new value differs from the old one)
	/// </summary>
	/// <typeparam name="T">The type of the property</typeparam>
	/// <param name="name">The name of the property to raise the notification event for</param>
	/// <param name="field">The backing field for the property</param>
	/// <param name="newValue">The new value to set</param>
	/// <param name="action">An optional action to perform after raising the notification event</param>
	/// <returns><see langword="true"/> when <paramref name="newValue"/> was different from the <paramref name="field"/></returns>
	protected void SetProperty<T>( string name, ref T field, T newValue, Action? action = null )
		=> SetPropertyInternal( name, ref field, newValue, OnPropertyChanged, action );

	/// <summary>
	/// Sets the property value and raises <see cref="PropertyChanged"/> event (if the new value differs from the old one)
	/// </summary>
	/// <typeparam name="T">The type of the property</typeparam>
	/// <param name="name">The name of the property to raise the notification event for</param>
	/// <param name="field">The backing field for the property</param>
	/// <param name="newValue">The new value to set</param>
	/// <param name="propertyChangedNotifier">The action to use for raising the <see cref="PropertyChanged"/> event</param>
	/// <param name="action">An optional action to perform after raising the notification event</param>
	/// <returns><see langword="true"/> when <paramref name="newValue"/> was different from the <paramref name="field"/></returns>
	private static void SetPropertyInternal<T>( string name, ref T field, T newValue, Action<string> propertyChangedNotifier, Action? action = null )
	{
		if( Equals( field, newValue ) )
			return;

		field = newValue;

		propertyChangedNotifier.Invoke( name );

		action?.Invoke();
	}

	/// <summary>
	/// Reacts to change of given property
	/// </summary>
	/// <param name="name">The name of the property that was changed</param>
	protected void OnPropertyChanged( string name )
	{
		PropertyChanged?.Invoke( this, new PropertyChangedEventArgs( name ) );
	}

	#endregion

	#region INotifyPropertyChanged

	public event PropertyChangedEventHandler? PropertyChanged;

	#endregion
}
