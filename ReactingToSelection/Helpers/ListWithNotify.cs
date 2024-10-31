using System.Collections.Specialized;
using System.Windows.Threading;

namespace ReactingToSelection.Helpers;

/// <summary>
/// Represents collection based on <see cref="List{T}"/> with rudimentary <see cref="INotifyCollectionChanged"/> implementation
/// </summary>
/// <typeparam name="TItem">The type of collection item</typeparam>
public class ListWithNotify<TItem> : List<TItem>
	, INotifyCollectionChanged
{
	#region Nested Types

	public enum NotificationType
	{
		None,
		Asynchronous,
		Synchronous,
	}

	#endregion

	#region Fields

	/// <summary>
	/// The dispatcher to use
	/// </summary>
	private readonly Dispatcher? _dispatcher = Dispatcher.FromThread( Thread.CurrentThread );

	#endregion

	#region Constructors

	/// <summary>
	/// Creates a new instance of <see cref="ListWithNotify{TItem}"/> class
	/// </summary>
	public ListWithNotify()
	{
		// Intentionally left blank
	}

	/// <summary>
	/// Creates a new instance of <see cref="ListWithNotify{TItem}"/> class
	/// </summary>
	/// <param name="items">The items to fill the list with</param>
	public ListWithNotify( IEnumerable<TItem> items )
		: base( items )
	{
		// Intentionally left blank
	}

	#endregion

	#region Methods

	/// <summary>
	/// Raises the <see cref="CollectionChanged"/> event
	/// </summary>
	/// <param name="notificationType">Value indicating how the notification should be raised</param>
	private void RaiseCollectionChanged( NotificationType notificationType )
	{
		switch( notificationType )
		{
			case NotificationType.Asynchronous:
				(_dispatcher ?? System.Windows.Application.Current.Dispatcher).InvokeAsync( () => RaiseCollectionChanged( NotificationType.Synchronous ) );

				break;

			case NotificationType.Synchronous:
				CollectionChanged?.Invoke( this, new NotifyCollectionChangedEventArgs( NotifyCollectionChangedAction.Reset ) );

				break;

			case NotificationType.None:
				break;

			default:
				throw new InvalidOperationException( "Unsupported notification type" );
		}
	}

	/// <summary>
	/// Replaces all items in collection with the provided items and raises <see cref="CollectionChanged"/> event
	/// </summary>
	/// <param name="newContent">Collection of items to replace the content of this collection</param>
	/// <param name="notificationType">Value indicating how the notification should be raised</param>
	public void ReplaceContentAndNotify( IEnumerable<TItem> newContent, NotificationType notificationType = NotificationType.Asynchronous )
	{
		Clear();
		AddRange( newContent );

		RaiseCollectionChanged( notificationType );
	}

	#endregion

	#region INotifyCollectionChanged

	public event NotifyCollectionChangedEventHandler? CollectionChanged;

	#endregion
}
