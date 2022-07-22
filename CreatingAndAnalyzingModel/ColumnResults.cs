namespace CreatingAndAnalyzingModel
{
	/// <summary>
	/// Stores the results required for identifying and reporting critical column forces
	/// </summary>
	public class ColumnResults
	{
		#region Properties

		/// <summary>
		/// Gets or sets the member name
		/// </summary>
		public string MemberName { get; set; }

		/// <summary>
		/// Gets or sets the span number
		/// </summary>
		public int SpanNumber { get; set; }

		/// <summary>
		/// Gets or sets the span length (in [mm])
		/// </summary>
		public double SpanLength { get; set; }

		/// <summary>
		/// Gets or sets the name of the critical combination for major moment
		/// </summary>
		public string CriticalMajorMomentCombinationName { get; set; }

		/// <summary>
		/// Gets or sets the name of the critical combination for minor moment
		/// </summary>
		public string CriticalMinorMomentCombinationName { get; set; }

		/// <summary>
		/// Gets or sets the name of the critical combination for axial force
		/// </summary>
		public string CriticalAxialForceCombinationName { get; set; }

		/// <summary>
		/// Gets or sets the value of the maximum major moment (in [Nmm])
		/// </summary>
		public double MaximumMajorMomentValue { get; set; }

		/// <summary>
		/// Gets or sets the value of the maximum minor moment (in [Nmm])
		/// </summary>
		public double MaximumMinorMomentValue { get; set; }

		/// <summary>
		/// Gets or sets the value of the maximum axial force (in [N])
		/// </summary>
		public double MaximumAxialForceValue { get; set; }

		#endregion
	}
}
