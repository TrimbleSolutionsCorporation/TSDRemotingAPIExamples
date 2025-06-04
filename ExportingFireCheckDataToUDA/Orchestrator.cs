using ExportingFireCheckDataToUDA.Extensions;
using TSD.API.Remoting;
using TSD.API.Remoting.Materials;
using TSD.API.Remoting.Structure;

namespace ExportingFireCheckDataToUDA;

/// <summary>
/// Orchestrates the whole export process
/// </summary>
internal static class Orchestrator
{
	#region Methods

	/// <summary>
	/// Exports the fire check data to UDA
	/// </summary>
	/// <param name="apiPort">The port TSD API is listening on (if <see langword="null"/> the first running application will be used)</param>
	public static async Task ExportDataToUdaAsync( int? apiPort )
	{
		var model = await GetModelAsync( apiPort );

		var eligibleMembers = (await model.GetMembersAsync())
			.Where( IsEligible )
			.ToList();

		Console.WriteLine( $"Found {eligibleMembers.Count} eligible members" );

		var spans = new System.Collections.Concurrent.ConcurrentBag<(IMember Member, IEnumerable<IMemberSpan> Spans)>();

		await Task.Run( () => Parallel.ForEachAsync( eligibleMembers, async ( m, cancellationToken ) =>
		{
			spans.Add( (Member: m, await m.GetSpanAsync( cancellationToken: cancellationToken )) );
		} ) );

		var fireCheckData = spans
			.SelectMany( tuple => tuple.Spans.Select( s => GetFireCheckData( tuple.Member, s ) ) )
			.ToList();

		Console.WriteLine( $"Found {fireCheckData.Count} eligible member spans" );

		var dataList = await DataCollector.PopulateFireCheckDataAsync( fireCheckData );

		await DataApplier.ApplyFireCheckDataAsync( model, dataList );
	}

	/// <summary>
	/// Returns a value indicating whether the given member is eligible for the fire check
	/// </summary>
	/// <param name="member">The member to check</param>
	private static bool IsEligible( IMember member )
	{
		if( member.Data.GetValueOrDefault() is not { } memberData )
			return false;

		if( memberData.MaterialType.GetValueOrDefault() is not MaterialType.Steel and not MaterialType.ColdFormed )
			return false;

		return true;
	}

	/// <summary>
	/// Returns a fire check data associated with the given member span
	/// </summary>
	/// <param name="member">The owning member of the span to check</param>
	/// <param name="memberSpan">The member span to check</param>
	/// <returns>Returns a fire check data associated with the given member span (or <see langword="null"/> in case the member span is not eligible)</returns>
	private static FireCheckData GetFireCheckData( IMember member, IMemberSpan memberSpan )
	{
		var fireCheckData = memberSpan.FireCheckData.GetValueOrDefault();

		return new FireCheckData
		{
			Member = member,
			MemberSpan = memberSpan,
			Exposure = fireCheckData?.ExposedSides.IsApplicable is true ? fireCheckData.ExposedSides.Value : null,
			NominalFireExposure = fireCheckData?.TimeOfFireExposure.IsApplicable is true ? fireCheckData.TimeOfFireExposure.Value : null,
		};
	}

	/// <summary>
	/// Returns the currently active model
	/// </summary>
	/// <param name="apiPort">The port TSD API is listening on (if <see langword="null"/> the first running application will be used)</param>
	private static async Task<IModel> GetModelAsync( int? apiPort )
	{
		var application = await ConnectAsync( apiPort );

		if( await application.GetDocumentAsync() is not { } document )
			throw new InvalidOperationException( "Could not obtain document!" );

		if( await document.GetModelAsync() is not { } model )
			throw new InvalidOperationException( "Could not obtain model!" );

		return model;
	}

	/// <summary>
	/// Connects to TSD
	/// </summary>
	/// <param name="apiPort">The port TSD API is listening on (if <see langword="null"/> the first running application will be used)</param>
	/// <returns>The connected application</returns>
	private static async Task<IApplication> ConnectAsync( int? apiPort )
	{
		if( apiPort is { } specifiedPort )
		{
			if( await ApplicationFactory.ConnectToRunningApplicationAsync( specifiedPort ) is not { } application )
				throw new InvalidOperationException( $"Could not connect to TSD API at port {specifiedPort}!" );

			return application;
		}
		else
		{
			if( await ApplicationFactory.GetFirstRunningApplicationAsync() is not { } application )
				throw new InvalidOperationException( "Could not connect to TSD API!" );

			return application;
		}
	}

	#endregion
}
