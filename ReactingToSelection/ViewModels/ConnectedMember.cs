using System.Diagnostics.CodeAnalysis;
using ReactingToSelection.Helpers;
using ReactingToSelection.Models;
using TSD.API.Remoting.Connections;
using TSD.API.Remoting.Loading;
using TSD.API.Remoting.Solver;
using TSD.API.Remoting.Structure;

namespace ReactingToSelection.ViewModels;

public class ConnectedMember : ViewModelBase
{
	#region Fields

	private IMember _member;
	private IConnection _connection;
	private SpanReleases? _spanReleases;
	private IForce3DLocal? _endForce;
	private AnalysisType _analysisType;
	private ICombination? _selectedCombination;

	#endregion

	#region Constructors

	public ConnectedMember( IMember member, IConnection connection )
	{
		SetSelectedMember( member, connection );

		AnalysisType = AnalysisTypes.First();

		GetCombinationsAsync().FireAndForgetSafeAsync();
	}

	#endregion

	#region Properties

	public string Name => _member.Name;

	public SpanReleases? SpanReleases
	{
		get => _spanReleases;
		set => SetProperty( nameof( SpanReleases ), ref _spanReleases, value );
	}

	public IForce3DLocal? EndForce
	{
		get => _endForce;
		set => SetProperty( nameof( EndForce ), ref _endForce, value );
	}

	public ICombination? SelectedCombination
	{
		get => _selectedCombination;
		set => SetProperty( nameof( SelectedCombination ), ref _selectedCombination, value, () => GetForceAndReleasesAsync().FireAndForgetSafeAsync() );
	}

	public ListWithNotify<ICombination> Combinations { get; } = [];

	public AnalysisType AnalysisType
	{
		get => _analysisType;
		set => SetProperty( nameof( AnalysisType ), ref _analysisType, value, () => GetForceAndReleasesAsync().FireAndForgetSafeAsync() );
	}

	public static AnalysisType[] AnalysisTypes => Enum.GetValues<AnalysisType>().Skip( 1 ).ToArray();

	#endregion

	#region Methods

	[MemberNotNull( nameof( _member ))]
	[MemberNotNull( nameof( _connection ) )]
	public void SetSelectedMember( IMember member, IConnection connection )
	{
		_member = member;
		_connection = connection;

		OnPropertyChanged( nameof( Name ) );
		GetForceAndReleasesAsync().FireAndForgetSafeAsync();
	}

	private async Task GetForceAndReleasesAsync()
	{
		if( SelectedCombination is null )
		{
			EndForce = null;
			SpanReleases = null;

			return;
		}

		var details = await MemberSpanFetcher.GetConnectedMemberDetailsAsync( _connection, _member, SelectedCombination, AnalysisType );

		if( details is null )
		{
			EndForce = null;
			SpanReleases = null;

			return;
		}

		EndForce = details.Value.EndForce;
		SpanReleases = new SpanReleases( details.Value.Releases );
	}

	private async Task GetCombinationsAsync()
	{
		var combinations = (await Model.Instance.GetCombinationsAsync()).ToList();

		Combinations.ReplaceContentAndNotify( combinations );

		SelectedCombination = combinations.FirstOrDefault();
	}

	#endregion
}
