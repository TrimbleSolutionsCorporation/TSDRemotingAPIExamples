using TSD.API.Remoting.Common;
using TSD.API.Remoting.Common.Properties;

namespace ReactingToSelection.Helpers;

/// <summary>
/// Contains helper methods for UI
/// </summary>
internal static class UiHelpers
{
	#region Methods

	/// <summary>
	/// Returns the display string for the given property
	/// </summary>
	/// <param name="property">The property to get display string of.</param>
	/// <typeparam name="T">The type of the property value</typeparam>
	/// <returns>The display string for the given property</returns>
	internal static string GetDisplayString<T>( this IReadOnlyProperty<T>? property )
	{
		if( property is not { IsApplicable: true } )
			return "-";

		return property.Value switch
		{
			int integer => integer.ToString(),
			double number => GetDisplayString( number ),
			string text => text,
			IHaveName namedObject => namedObject.Name,
			_ => property.Value?.ToString() ?? "null"
		};
	}

	/// <summary>
	/// Returns the display string for the given double value
	/// </summary>
	/// <param name="value">The value to get display string of</param>
	/// <returns>The display string for the given double value</returns>
	public static string GetDisplayString( double? value ) => value?.ToString( "F2" ) ?? string.Empty;

	#endregion
}
