using System.Globalization;
using ExportingFireCheckDataToUDA.Extensions;
using TSD.API.Remoting.Loading;
using TSD.API.Remoting.Structure;
using TSD.API.Remoting.UserDefinedAttributes;
using TSD.API.Remoting.UserDefinedAttributes.Create;

namespace ExportingFireCheckDataToUDA;

/// <summary>
/// Contains methods for applying fire check data
/// </summary>
internal static class DataApplier
{
	#region Constants

	/// <summary>
	/// The name of the user defined attribute with exposure
	/// </summary>
	private const string ExposureUdaName = "Exposure";

	/// <summary>
	/// The name of the user defined attribute with nominal fire exposure
	/// </summary>
	private const string NominalFireExposureUdaName = "Nominal fire exposure";

	/// <summary>
	/// The name of the user defined attribute with ambient utilization ratio
	/// </summary>
	private const string AmbientUtilizationRatioUdaName = "Ambient UR (u0)";

	/// <summary>
	/// The name of the user defined attribute with load reduction factor
	/// </summary>
	private const string LoadReductionFactorUdaName = "Load reduction factor (nfi)";

	/// <summary>
	/// The name of the user defined attribute with critical temperature
	/// </summary>
	private const string CriticalTemperatureUdaName = "Critical temperature";

	/// <summary>
	/// The name of the user defined attribute with imposed loading category
	/// </summary>
	private const string ImposedLoadingCategoryUdaName = "Imposed loading category";

	#endregion

	#region Methods

	/// <summary>
	/// Applies the given collection of fire check data
	/// </summary>
	/// <param name="model">The current model</param>
	/// <param name="dataList">The collection of fire check data to apply</param>
	public static async Task ApplyFireCheckDataAsync( IModel model, IReadOnlyList<FireCheckData> dataList )
	{
		var combinations = (await model.GetCombinationsAsync()).ToList();
		var dominantImposedLoadcases = await GetDominantImposedLoadcaseByCombination( model, combinations );

		var existingDefinitions = (await model.UserDefinedAttributesManager.GetAttributeDefinitionsByNamesAsync()).ToList();

		var exposureUda = await GetOrCreateAttributeDefinitionAsync( ExposureUdaName );
		var nominalFireExposureUda = await GetOrCreateAttributeDefinitionAsync( NominalFireExposureUdaName );
		var ambientUtilizationRatioUda = await GetOrCreateAttributeDefinitionAsync( AmbientUtilizationRatioUdaName );
		var loadReductionFactorUda = await GetOrCreateAttributeDefinitionAsync( LoadReductionFactorUdaName );
		var criticalTemperatureUda = await GetOrCreateAttributeDefinitionAsync( CriticalTemperatureUdaName );
		var imposedLoadingCategoryUda = await GetOrCreateAttributeDefinitionAsync( ImposedLoadingCategoryUdaName );

		var groupedChunks = dataList
			.GroupBy( d => d.MemberSpan )
			.ToList();

		Console.WriteLine( "Applying fire check data" );

		await model.UserDefinedAttributesManager.SetUserDefinedAttributesAsync( CreateSetAttributeParameters() );

		async Task<IAttributeDefinition> GetOrCreateAttributeDefinitionAsync( string name )
		{
			if( existingDefinitions.FirstOrDefault( d => d.Name.Equals( name, StringComparison.OrdinalIgnoreCase ) ) is { } existingDefinition )
				return existingDefinition;

			var parameters = CreateAttributeDefinitionParams.Create( name, AttributeValueType.String, AttributeValueSource.UserDefined );

			if( await model.UserDefinedAttributesManager.CreateAttributeDefinitionAsync( parameters ) is not { } definition )
				throw new InvalidOperationException( $"Could not create attribute {name}!" );

			return definition;
		}

		IEnumerable<ISetAttributeParams> CreateSetAttributeParameters()
		{
			foreach( var group in groupedChunks )
			{
				var fireCheckData = group.MaxBy( d => d.AmbientUtilizationRatio );

				IHaveUserDefinedAttributes? udaOwner = fireCheckData.MemberSpan;

				yield return udaOwner.CreateSetAttributeParams( exposureUda, FormatEnum( fireCheckData.Exposure ) );
				yield return udaOwner.CreateSetAttributeParams( nominalFireExposureUda, FormatEnum( fireCheckData.NominalFireExposure ) );
				yield return udaOwner.CreateSetAttributeParams( ambientUtilizationRatioUda, FormatDouble( fireCheckData.AmbientUtilizationRatio ) );
				yield return udaOwner.CreateSetAttributeParams( loadReductionFactorUda, FormatDouble( fireCheckData.LoadReductionFactor ) );
				yield return udaOwner.CreateSetAttributeParams( criticalTemperatureUda, FormatDouble( fireCheckData.CriticalTemperature ) );

				yield return udaOwner.CreateSetAttributeParams( imposedLoadingCategoryUda,
					FormatEnum( GetDominantImposedLoadcase( fireCheckData.CombinationReferenceIndex )?.Ec3LoadingCategory.GetValueOrDefault() ) );
			}
		}

		ILoadcase? GetDominantImposedLoadcase( int? combinationReferenceIndex )
		{
			if( combinations.FirstOrDefault( c => c.ReferenceIndex == combinationReferenceIndex ) is not { } combination )
				return null;

			if( !dominantImposedLoadcases.TryGetValue( combination, out var dominantImposedLoadcase ) )
				return null;

			return dominantImposedLoadcase;
		}

		static string FormatEnum<T>( T? value )
			where T : struct, Enum
			=> value?.ToString() ?? string.Empty;

		static string FormatDouble( double? value ) => value is not null ? string.Format( CultureInfo.InvariantCulture, "{0:F3}", value ) : string.Empty;
	}

	/// <summary>
	/// Returns a dictionary of dominant imposed loadcases indexed by the combination they dominate
	/// </summary>
	/// <param name="model">The current model</param>
	/// <param name="combinations">The collection of all combinations in the model</param>
	private static async Task<IDictionary<ICombination, ILoadcase?>> GetDominantImposedLoadcaseByCombination( IModel model, IReadOnlyList<ICombination> combinations )
	{
		var loadcases = (await model.GetLoadcasesAsync()).ToList();

		return combinations.ToDictionary( c => c, GetDominantImposedLoadcase );

		ILoadcase? GetDominantImposedLoadcase( ICombination combination )
		{
			if( combination.LoadcaseFactors.GetValueOrDefault() is not { } loadcaseFactors )
				return null;

			return loadcaseFactors
				.Select( lf => loadcases.FirstOrDefault( l => l.Id == lf.GetValueOrDefault()?.LoadcaseId.GetValueOrDefault() ) )
				.OfType<ILoadcase>()
				.Where( IsImposed )
				.MaxBy( GetPsi1Factor );
		}

		static double GetPsi1Factor( ILoadcase loadcase )
		{
			if( loadcase.EcCombinationFactors.GetValueOrDefault() is { } ecCombinationFactors )
				return ecCombinationFactors.Psi1.GetValueOrDefault( double.MinValue );

			if( loadcase.AustralianCombinationFactors.GetValueOrDefault() is { } australianCombinationFactors )
				return australianCombinationFactors.PsiLong.GetValueOrDefault( double.MinValue );

			return double.MinValue;
		}

		static bool IsImposed( ILoadcase loadcase ) => loadcase.Type.GetValueOrDefault() is LoadcaseType.Imposed;
	}

	#endregion
}
