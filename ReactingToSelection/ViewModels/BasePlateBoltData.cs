using ReactingToSelection.Helpers;
using TSD.API.Remoting.Connections;

namespace ReactingToSelection.ViewModels;

public class BasePlateBoltData( IBasePlateBoltData? data ) : ViewModelBase
{
	#region Properties

	public Bolt? Bolt { get; } = Bolt.Create( data?.Bolt.GetValueOrDefault() );

	public string BoltGrade { get; } = (data?.Grade).GetDisplayString();

	public string Length { get; } = (data?.Length).GetDisplayString();

	public string ProjectionAbovePlate { get; } = (data?.ProjectionAbovePlate).GetDisplayString();

	public string EmbeddedDepth { get; } = (data?.EmbeddedDepth).GetDisplayString();

	public AnchorPlate AnchorPlate { get; } = new( data?.AnchorPlate.GetValueOrDefault() );

	#endregion
}
