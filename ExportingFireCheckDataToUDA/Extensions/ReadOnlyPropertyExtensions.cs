using TSD.API.Remoting.Common.Properties;

namespace ExportingFireCheckDataToUDA.Extensions;

/// <summary>
/// Contains extension methods for the <see cref="IReadOnlyProperty{TValue}"/> interface
/// </summary>
internal static class ReadOnlyPropertyExtensions
{
	#region Methods

	/// <summary>
	/// Returns the value of this property or the given default value in case the property is not applicable
	/// </summary>
	/// <param name="property">The property to get value of</param>
	/// <param name="defaultValue">The default value to use in case the property is not applicable</param>
	/// <typeparam name="TValue">The type of value stored in the property</typeparam>
	public static TValue? GetValueOrDefault<TValue>( this IReadOnlyProperty<TValue> property, TValue? defaultValue = default )
		=> property is { IsApplicable: true } ? property.Value : defaultValue;

	#endregion
}
