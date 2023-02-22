using System.Collections.Generic;
using System.Linq;
using ClosedXML.Excel;
using TSD.API.Remoting.Materials;
using TSD.API.Remoting.Sections;
using TSD.API.Remoting.Solver;

namespace Extracting1DElementData.Exporters
{
	/// <summary>
	/// Contains methods for exporting 1D elements
	/// </summary>
	public static class Element1dExporter
	{
		#region Constants

		private const int LastColumnIndex = 12;

		#endregion

		#region Methods

		public static void Export( IXLWorkbook workbook, List<IElement1D> elements )
		{
			var workSheet = workbook.Worksheets.Add( "Elements 1D" );

			const int column = 1;
			var row = 1;

			ExportBaseInfo( workSheet, elements, ref row, column );
			ExportMaterialInfo( workSheet, elements, ref row, column );
			ExportElementSections( workSheet, elements, ref row, column );
			ExportSpringSections( workSheet, elements, ref row, column );
			ExportLinkElementSections( workSheet, elements, ref row, column );

			workSheet.Columns( 1, LastColumnIndex ).AdjustToContents();
		}

		private static void SetHeading( IXLWorksheet worksheet, int row, int column, int columnCount, string text )
		{
			worksheet.Cell( row, column ).Value = text;

			var range = worksheet.Range( row, column, row, column + columnCount - 1 );

			range.Merge();
			range.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
			range.Style.Font.Bold = true;
		}

		private static void ExportBaseInfo( IXLWorksheet worksheet, List<IElement1D> elements, ref int row, int column )
		{
			if( !elements.Any() )
				return;

			SetHeading( worksheet, row, column, 3, "Base Info" );

			row++;

			worksheet.Cell( row, column ).Value = "Element index";
			worksheet.Cell( row, column + 1 ).Value = "Start node index";
			worksheet.Cell( row, column + 2 ).Value = "End node index";

			row++;

			foreach( var element in elements )
			{
				worksheet.Cell( row, column ).Value = element.Index;
				worksheet.Cell( row, column + 1 ).Value = element.GetNodeIndex( 0 );
				worksheet.Cell( row, column + 2 ).Value = element.GetNodeIndex( 1 );

				row++;
			}

			row++;
		}

		private static void ExportMaterialInfo( IXLWorksheet worksheet, IEnumerable<IElement1D> elements, ref int row, int column )
		{
			var materials = GetMaterials();

			if( !materials.Any() )
				return;

			SetHeading( worksheet, row, column, 5, "Material Info" );

			row++;

			worksheet.Cell( row, column ).Value = "Element index";
			worksheet.Cell( row, column + 1 ).Value = "Material name";
			worksheet.Cell( row, column + 2 ).Value = "Shear modulus";
			worksheet.Cell( row, column + 3 ).Value = "Poisson's ratio";
			worksheet.Cell( row, column + 4 ).Value = "Thermal expansion coefficient";

			row++;

			foreach( var (index, material) in materials )
			{
				worksheet.Cell( row, column ).Value = index;
				worksheet.Cell( row, column + 1 ).Value = material.Name;
				worksheet.Cell( row, column + 2 ).Value = material.ShearModulus;
				worksheet.Cell( row, column + 3 ).Value = material.PoissonsRatio;
				worksheet.Cell( row, column + 4 ).Value = material.ThermalExpansionCoefficient;

				row++;
			}

			row++;

			List<(int, IMaterial)> GetMaterials() => elements
				.Select( element => (element.Index, element.Material) )
				.Where( tuple => tuple.Material != null )
				.ToList();
		}

		private static void ExportElementSections( IXLWorksheet worksheet, IEnumerable<IElement1D> elements, ref int row, int column )
		{
			var solverElementSections = GetSolverElementSections();

			if( !solverElementSections.Any() )
				return;

			SetHeading( worksheet, row, column, 8, "Element Sections" );

			row++;

			worksheet.Cell( row, column ).Value = "Element index";
			worksheet.Cell( row, column + 1 ).Value = "Torsion constant";
			worksheet.Cell( row, column + 2 ).Value = "Major axis 2nd moment of area";
			worksheet.Cell( row, column + 3 ).Value = "Minor axis 2nd moment of area";
			worksheet.Cell( row, column + 4 ).Value = "Cross sectional area";
			worksheet.Cell( row, column + 5 ).Value = "Shear area loaded parallel to major axis";
			worksheet.Cell( row, column + 6 ).Value = "Shear area loaded parallel to minor axis";
			worksheet.Cell( row, column + 7 ).Value = "Section name";

			row++;

			foreach( var (index, section) in solverElementSections )
			{
				worksheet.Cell( row, column ).Value = index;
				worksheet.Cell( row, column + 1 ).Value = section.TorsionConstant.Value;
				worksheet.Cell( row, column + 2 ).Value = section.MajorAxisSecondMomentOfArea.Value;
				worksheet.Cell( row, column + 3 ).Value = section.MinorAxisSecondMomentOfArea.Value;
				worksheet.Cell( row, column + 4 ).Value = section.CrossSectionalArea.Value;
				worksheet.Cell( row, column + 5 ).Value = section.ShearAreaLoadedParallelToMajorAxis.Value;
				worksheet.Cell( row, column + 6 ).Value = section.ShearAreaLoadedParallelToMinorAxis.Value;
				worksheet.Cell( row, column + 7 ).Value = (section as IMemberSection)?.PhysicalSection.Value.LongName;

				row++;
			}

			row++;

			List<(int, ISolverElementSection)> GetSolverElementSections() => elements
				.Select( element => (element.Index, SolverElementSection: element.ElementSection as ISolverElementSection) )
				.Where( tuple => tuple.SolverElementSection != null )
				.Cast<(int, ISolverElementSection)>()
				.ToList();
		}

		private static void ExportSpringSections( IXLWorksheet worksheet, IEnumerable<IElement1D> elements, ref int row, int column )
		{
			var springSections = GetSpringSections();

			if( !springSections.Any() )
				return;

			SetHeading( worksheet, row, column, 11, "Spring Sections" );

			row++;

			worksheet.Cell( row, column ).Value = "Element index";
			worksheet.Cell( row, column + 1 ).Value = "Transitional stiffness";
			worksheet.Cell( row, column + 2 ).Value = "Rotational stiffness";
			worksheet.Cell( row, column + 3 ).Value = "Axial stiffness K tension";
			worksheet.Cell( row, column + 4 ).Value = "Axial F max tension";
			worksheet.Cell( row, column + 5 ).Value = "Axial stiffness K compression";
			worksheet.Cell( row, column + 6 ).Value = "Axial F max compression";
			worksheet.Cell( row, column + 7 ).Value = "Rotational stiffness K tension";
			worksheet.Cell( row, column + 8 ).Value = "Rotational F max tension";
			worksheet.Cell( row, column + 9 ).Value = "Rotational stiffness K compression";
			worksheet.Cell( row, column + 10 ).Value = "Rotational F max compression";

			row++;

			foreach( var (index, section) in springSections )
			{
				worksheet.Cell( row, column ).Value = index;

				ExportSpringSection( worksheet, section, row, column + 1 );

				row++;
			}

			row++;

			List<(int, ISpringSection)> GetSpringSections() => elements
				.Select( element => (element.Index, SpringSection: element.ElementSection as ISpringSection) )
				.Where( tuple => tuple.SpringSection != null )
				.Cast<(int, ISpringSection)>()
				.ToList();
		}

		private static void ExportLinkElementSections( IXLWorksheet worksheet, IEnumerable<IElement1D> elements, ref int row, int column )
		{
			var linkElementSections = GetLinkElementSections();

			if( !linkElementSections.Any() )
				return;

			SetHeading( worksheet, row, column, 12, "Link Element Sections" );

			row++;

			worksheet.Cell( row, column ).Value = "Element index";
			worksheet.Cell( row, column + 1 ).Value = "Direction";
			worksheet.Cell( row, column + 2 ).Value = "Transitional stiffness";
			worksheet.Cell( row, column + 3 ).Value = "Rotational stiffness";
			worksheet.Cell( row, column + 4 ).Value = "Axial stiffness K tension";
			worksheet.Cell( row, column + 5 ).Value = "Axial F max tension";
			worksheet.Cell( row, column + 6 ).Value = "Axial stiffness K compression";
			worksheet.Cell( row, column + 7 ).Value = "Axial F max compression";
			worksheet.Cell( row, column + 8 ).Value = "Rotational stiffness K tension";
			worksheet.Cell( row, column + 9 ).Value = "Rotational F max tension";
			worksheet.Cell( row, column + 10 ).Value = "Rotational stiffness K compression";
			worksheet.Cell( row, column + 11 ).Value = "Rotational F max compression";

			foreach( var (index, section) in linkElementSections )
			{
				worksheet.Cell( row, column ).Value = index;

				worksheet.Cell( row, column + 1 ).Value = "X";
				ExportSpringSection( worksheet, section.SpringX.Value, row, column + 2 );

				row++;

				worksheet.Cell( row, column + 1 ).Value = "Y";
				ExportSpringSection( worksheet, section.SpringY.Value, row, column + 2 );

				row++;

				worksheet.Cell( row, column + 1 ).Value = "Z";
				ExportSpringSection( worksheet, section.SpringZ.Value, row, column + 2 );

				row++;
			}

			row++;

			List<(int, ILinkElementSection)> GetLinkElementSections() => elements
				.Select( element => (element.Index, LinkElementSection: element.ElementSection as ILinkElementSection) )
				.Where( tuple => tuple.LinkElementSection != null )
				.Cast<(int, ILinkElementSection)>()
				.ToList();
		}

		private static void ExportSpringSection( IXLWorksheet worksheet, ISpringSection section, int row, int column )
		{
			if( section is ILinearAxialSpringSection linearAxialSpringSection )
			{
				worksheet.Cell( row, column ).Value = linearAxialSpringSection.TransitionalStiffness.Value;
			}

			if( section is ILinearTorsionalSpringSection linearTorsionalSpringSection )
			{
				worksheet.Cell( row, column + 1 ).Value = linearTorsionalSpringSection.RotationalStiffness.Value;
			}

			if( section is INonLinearAxialSpringSection nonLinearAxialSpringSection )
			{
				worksheet.Cell( row, column + 2 ).Value = nonLinearAxialSpringSection.AxialStiffnessKTension.Value;
				worksheet.Cell( row, column + 3 ).Value = nonLinearAxialSpringSection.AxialFMaxTension.Value;
				worksheet.Cell( row, column + 4 ).Value = nonLinearAxialSpringSection.AxialStiffnessKCompression.Value;
				worksheet.Cell( row, column + 5 ).Value = nonLinearAxialSpringSection.AxialFMaxCompression.Value;
			}

			if( section is INonLinearTorsionalSpringSection nonLinearTorsionalSpringSection )
			{
				worksheet.Cell( row, column + 6 ).Value = nonLinearTorsionalSpringSection.TorsionalStiffnessKTension.Value;
				worksheet.Cell( row, column + 7 ).Value = nonLinearTorsionalSpringSection.TorsionalFMaxTension.Value;
				worksheet.Cell( row, column + 8 ).Value = nonLinearTorsionalSpringSection.TorsionalStiffnessKCompression.Value;
				worksheet.Cell( row, column + 9 ).Value = nonLinearTorsionalSpringSection.TorsionalFMaxCompression.Value;
			}
		}

		#endregion
	}
}
