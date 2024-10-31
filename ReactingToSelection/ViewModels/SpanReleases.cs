using ReactingToSelection.Helpers;
using TSD.API.Remoting.Structure;

namespace ReactingToSelection.ViewModels;

public class SpanReleases( ISpanReleases? spanReleases ) : ViewModelBase
{
	#region Properties

	public string DegreeOfFreedom => (spanReleases?.DegreeOfFreedom).GetDisplayString();

	public string Cantilever => (spanReleases?.Cantilever).GetDisplayString();

	public string AxialRelease => (spanReleases?.AxialRelease).GetDisplayString();

	public string TorsionalRelease => (spanReleases?.TorsionalRelease).GetDisplayString();

	#endregion
}
