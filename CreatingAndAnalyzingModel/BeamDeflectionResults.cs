namespace CreatingAndAnalyzingModel
{
	/// <summary>
	/// Stores the results required for identifying and reporting critical beam deflection
	/// </summary>
	public class BeamDeflectionResults
	{
		#region Properties

		/// <summary>
		/// Gets or sets the member name
		/// </summary>
		public string MemberName { get; set; }

		/// <summary>
		/// Gets or sets the members total length (includes all spans) (in [mm])
		/// </summary>
		public double MemberTotalLength { get; set; }

		/// <summary>
		/// Gets or sets the name of the critical combination
		/// </summary>
		public string CriticalCombinationName { get; set; }

		/// <summary>
		/// Gets or sets the value of the maximum deflection (in [mm])
		/// </summary>
		public double MaximumDeflectionValue { get; set; }

		/// <summary>
		/// Gets or sets the span index at which the maximum deflection occurs
		/// </summary>
		public int MaximumDeflectionSpanIndex { get; set; }

		/// <summary>
		/// Gets or sets the position at which the maximum deflection occurs, relative to the appropriate span (in [mm])
		/// </summary>
		public double MaximumDeflectionPosition { get; set; }

		/// <summary>
		/// Gets or sets the value of the maximum moment (in [Nmm])
		/// </summary>
		public double MaximumMomentValue { get; set; }

		/// <summary>
		/// Gets or sets the span index at which the maximum moment occurs
		/// </summary>
		public int MaximumMomentSpanIndex { get; set; }

		/// <summary>
		/// Gets or sets the position at which the maximum moment occurs, relative to the appropriate span (in [mm])
		/// </summary>
		public double MaximumMomentPosition { get; set; }

		#endregion
	}
}
