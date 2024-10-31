using ReactingToSelection.Helpers;
using TSD.API.Remoting.Connections;
using static ReactingToSelection.Helpers.UiHelpers;

namespace ReactingToSelection.ViewModels;

public abstract class Bolt( IBolt bolt ) : ViewModelBase
{
	#region Properties

	public string HeadCode { get; } = bolt.HeadCode.ToString();

	public string SystemType { get; } = bolt.SystemType.ToString();

	public string Grade { get; } = bolt.Grade;

	public string Diameter { get; } = GetDisplayString( bolt.Diameter );

	public string TensileStrength { get; } = GetDisplayString( bolt.TensileStrength );

	public string WidthAcrossNutFlats { get; } = GetDisplayString( bolt.WidthAcrossNutFlats );

	public string WidthAcrossNutPoints { get; } = GetDisplayString( bolt.WidthAcrossNutPoints );

	public string NutThickness { get; } = GetDisplayString( bolt.NutThickness );

	public string WasherOutsideDiameter { get; } = GetDisplayString( bolt.WasherOutsideDiameter );

	public string WasherThickness { get; } = GetDisplayString( bolt.WasherThickness );

	#endregion

	#region Methods

	public static Bolt? Create( IBolt? bolt ) => bolt switch
	{
		IBoltEc ecBolt => new BoltEc( ecBolt ),
		IBoltUs usBolt => new BoltUs( usBolt ),
		null => null,
		_ => throw new InvalidOperationException( $"Unexpected type of bolt {bolt.GetType().Name} encountered!" )
	};

	#endregion
}
