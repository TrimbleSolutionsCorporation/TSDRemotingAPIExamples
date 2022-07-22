namespace CreatingAndAnalyzingModel
{
	/// <summary>
	/// Stores the results required for identifying and reporting mid span forces
	/// </summary>
	public class MidSpanResults
	{
		#region Properties

		/// <summary>
		/// Gets or sets the member name
		/// </summary>
		public string MemberName { get; set; }

		/// <summary>
		/// Gets or sets the span index
		/// </summary>
		public int SpanIndex { get; set; }

		/// <summary>
		/// Gets or sets the position of the mid span (in [mm])
		/// </summary>
		public double MidSpanPosition { get; set; }

		/// <summary>
		/// Gets or sets the name of the combination
		/// </summary>
		public string CombinationName { get; set; }

		/// <summary>
		/// Gets or sets the value of the major moment at the mid span (in [Nmm])
		/// </summary>
		public double MidSpanMajorMomentValue { get; set; }

		/// <summary>
		/// Gets or sets the value of the major shear at the mid span (in [N])
		/// </summary>
		public double MidSpanMajorShearValue { get; set; }

		/// <summary>
		/// Gets or sets the value of the axial force at the mid span (in [N])
		/// </summary>
		public double MidSpanAxialValue { get; set; }

		/// <summary>
		/// Gets or sets the value of the torsion at the mid span (in [Nmm])
		/// </summary>
		public double MidSpanTorsionValue { get; set; }

		/// <summary>
		/// Gets or sets the value of the deflection at the mid span (in [mm])
		/// </summary>
		public double MidSpanDeflectionValue { get; set; }

		#endregion
	}
}
