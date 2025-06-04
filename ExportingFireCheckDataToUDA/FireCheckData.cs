using TSD.API.Remoting.Structure;

namespace ExportingFireCheckDataToUDA;

/// <summary>
/// Contains the fire check data
/// </summary>
/// <param name="Member">The owning member of the span the fire check data belong to</param>
/// <param name="MemberSpan">The member span the fire check data belong to</param>
/// <param name="Exposure">The fire exposure of the member span</param>
/// <param name="NominalFireExposure">The time of fire exposure of the member span</param>
/// <param name="CriticalTemperature">The critical temperature for the member span</param>
/// <param name="AmbientUtilizationRatio">The ambient utilization ratio for the member span</param>
/// <param name="LoadReductionFactor">The load reduction factor for the member span</param>
/// <param name="CombinationReferenceIndex">The reference index of the combination</param>
internal readonly record struct FireCheckData(
	IMember Member,
	IMemberSpan MemberSpan,
	ExposedSides? Exposure,
	TimeOfFireExposure? NominalFireExposure,
	double? CriticalTemperature,
	double? AmbientUtilizationRatio,
	double? LoadReductionFactor,
	int? CombinationReferenceIndex
);
