namespace ExportingFireCheckDataToUDA;

/// <summary>
/// Contains methods for indicating progress
/// </summary>
internal static class ProgressIndicator
{
	#region Methods

	/// <summary>
	/// Returns the text indicating the current progress
	/// </summary>
	/// <param name="decimalPlaces">The number of decimal places of the total number of data chunks</param>
	/// <param name="currentChunkIndex">The index of the currently processed data chunk</param>
	/// <param name="totalChunkCount">The total number of data chunks</param>
	public static string GetText( int decimalPlaces, int currentChunkIndex, int totalChunkCount )
		=> $"{(currentChunkIndex + 1).ToString().PadLeft( decimalPlaces )}/{totalChunkCount}";

	#endregion
}
