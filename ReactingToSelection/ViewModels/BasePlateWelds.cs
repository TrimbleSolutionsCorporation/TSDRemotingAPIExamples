using ReactingToSelection.Helpers;
using TSD.API.Remoting.Common.Properties;
using TSD.API.Remoting.Connections;

namespace ReactingToSelection.ViewModels;

public class BasePlateWelds : ViewModelBase
{
	#region Nested Types

	public class WeldData( BasePlateWeldLocation location, IBasePlateWeldData? data ) : ViewModelBase
	{
		#region Properties

		public string Location { get; } = location.ToString();

		public string WeldType { get; } = (data?.Type).GetDisplayString();

		public string LegLength { get; } = (data?.LegLength).GetDisplayString();

		public bool IsFullLength { get; } = (data?.IsFullLength).GetValueOrDefault();

		public string PartialLength { get; } = (data?.PartialLength).GetDisplayString();

		#endregion
	}

	#endregion

	#region Constructors

	public BasePlateWelds( IEnumerable<IKeyValuePair<BasePlateWeldLocation, IReadOnlyProperty<IBasePlateWeldData>>>? weldData )
	{
		Data = new ListWithNotify<WeldData>( weldData?.Select( kvp => new WeldData( kvp.Key, kvp.Value.GetValueOrDefault() ) ) ?? [] );
	}

	#endregion

	#region Properties

	public ListWithNotify<WeldData> Data { get; }

	#endregion
}
