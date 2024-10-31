using ReactingToSelection.Helpers;
using TSD.API.Remoting.Connections;

namespace ReactingToSelection.ViewModels;

public class BasePlateSymmetricBoltLayout( IBasePlateSymmetricBoltLayout layout ) : BasePlateBoltLayout
{
	#region Properties

	public bool Row2Enabled { get; } = layout.Row2Enabled.GetValueOrDefault();

	public string BoltCountRow1 { get; } = layout.BoltCountRow1.GetDisplayString();

	public string BoltCountRow2 { get; } = layout.BoltCountRow2.GetDisplayString();

	public string EndDistance { get; } = layout.EndDistance.GetDisplayString();

	public string FixedDistance { get; } = layout.FixedDistance.GetDisplayString();

	public string EdgeDistance { get; } = layout.EdgeDistance.GetDisplayString();

	public string EdgeDistanceRow1 { get; } = layout.EdgeDistanceRow1.GetDisplayString();

	public string EdgeDistanceRow2 { get; } = layout.EdgeDistanceRow2.GetDisplayString();

	public string Pitch { get; } = layout.Pitch.GetDisplayString();

	public string PitchRow1 { get; } = layout.PitchRow1.GetDisplayString();

	public string PitchRow2 { get; } = layout.PitchRow2.GetDisplayString();

	public string Gauge { get; } = layout.Gauge.GetDisplayString();

	#endregion
}
