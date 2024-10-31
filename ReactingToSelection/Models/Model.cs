using TSD.API.Remoting.Structure;

namespace ReactingToSelection.Models;

/// <summary>
/// Represents a class that holds reference to the currently active model
/// </summary>
public static class Model
{
	#region Fields

	/// <summary>
	/// The currently active model
	/// </summary>
	private static IModel? _instance;

	#endregion

	#region Properties

	/// <summary>
	/// Gets the currently active model
	/// </summary>
	/// <remarks>Throws <see cref="InvalidOperationException"/> in case the model is not fetched</remarks>
	public static IModel Instance => _instance ?? throw new InvalidOperationException( "There is no active model!" );

	#endregion

	#region Methods

	/// <summary>
	/// Fetches the currently opened model of given document
	/// </summary>
	/// <param name="document">The document to fetch currently opened model of</param>
	public static async Task FetchModelAsync( TSD.API.Remoting.Document.IDocument document )
	{
		var model = await document.GetModelAsync();

		_instance = model;
	}

	#endregion
}
