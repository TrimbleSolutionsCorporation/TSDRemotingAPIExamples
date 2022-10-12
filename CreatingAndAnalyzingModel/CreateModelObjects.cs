using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using TSD.API.Remoting.Common;
using TSD.API.Remoting.Foundations;
using TSD.API.Remoting.Foundations.Create;
using TSD.API.Remoting.Geometry;
using TSD.API.Remoting.Materials;
using TSD.API.Remoting.Sections;
using TSD.API.Remoting.Solver;
using TSD.API.Remoting.Structure;
using TSD.API.Remoting.Structure.Create;

namespace CreatingAndAnalyzingModel
{
	/// <summary>
	/// Contains methods to create the model objects
	/// </summary>
	public class CreateModelObjects
	{
		#region Methods

		public async Task Create( TSD.API.Remoting.Structure.IModel model )
		{
			await CreateLevels( model );

			await CreateGrids( model );

			var constructionPoints = await CreateConstructionPoints( model );

			await CreateConcreteBeams( model, constructionPoints );

			await CreateSteelBeams( model, constructionPoints );

			await CreateStructuralWalls( model, constructionPoints );

			await CreateConcreteColumns( model, constructionPoints );

			await CreateSteelColumns( model, constructionPoints );

			await CreateSlabItems( model, constructionPoints );

			await CreateIsolatedFoundations( model );

			await EditSteelBeamSections( model, constructionPoints );

			await EditConcreteColumnSections( model, constructionPoints );
		}

		private static async Task CreateLevels( TSD.API.Remoting.Structure.IModel model )
		{
			var levelParams = new List<HorizontalConstructionPlaneParams>();

			// 2 levels will be created at 3m spacing
			for( var i = 1; i < 3; i++ )
			{
				levelParams.Add( new HorizontalConstructionPlaneParams( i * 3000.0 ) );
			}

			var levels = (await model.CreateEntityAsync( levelParams )).Cast<IHorizontalConstructionPlane>().ToList();

			for( var i = 0; i < levels.Count; i++ )
			{
				var level = levels[i];

				level.ShortReference.Value = $"{i + 1}";
				level.LongReference.Value = $"{i + 1}";
				level.IsFloor.Value = true;
				level.Level.Value = (i + 1) * 3000.0;
				level.SourcePlaneId.Value = Guid.Empty;
			}

			await model.ApplyEntityAsync( levels );

			Console.WriteLine( "Levels created" );
		}

		private static async Task CreateGrids( TSD.API.Remoting.Structure.IModel model )
		{
			var ec = await model.GetEntityCollectionAsync( EntityType.HorizontalConstructionPlane );

			var gridParams = new List<EntityParams>();

			// Creates architectural grids: 3 bays at 4m by 2 bays at 6m
			var grid = new ParallelogramGridParams(
				ec.ValidIndices.Select( index => new EntityInfo { Index = index, Type = EntityType.HorizontalConstructionPlane } ),
				new ParallelogramGridParams.DirectionExtents( 3, 4000.0, GridLineNameType.Numeric ),
				new ParallelogramGridParams.DirectionExtents( 2, 6000.0, GridLineNameType.Alphanumeric ) )
			{
				Color = Color.OrangeRed,
				Style = ConstructionHelperLineType.DashDot,
			};

			gridParams.Add( grid );

			var entities = (await model.CreateEntityAsync( gridParams )).ToList();

			Console.WriteLine( "Grids created" );
		}

		private static async Task<List<List<List<IConstructionPoint>>>> CreateConstructionPoints( TSD.API.Remoting.Structure.IModel model )
		{
			var coords = new List<ConstructionPointParams>();

			const int xCount = 4; // Number of points in the X direction
			const int yCount = 3; // Number of points in the Y direction
			const int zCount = 3; // Number of points in the Z direction

			for( var x = 0; x < xCount; x++ )
			{
				for( var y = 0; y < yCount; y++ )
				{
					for( var z = 0; z < zCount; z++ )
					{
						coords.Add( new ConstructionPointParams( new Point3D( x * 4000.0, y * 6000.0, z * 3000.0 ) ) );
					}
				}
			}

			var points = (await model.CreateEntityAsync( coords )).Cast<IConstructionPoint>().ToList();

			var listX = new List<List<List<IConstructionPoint>>>();

			var index = 0;
			for( var x = 0; x < xCount; x++ )
			{
				var listY = new List<List<IConstructionPoint>>();

				for( var y = 0; y < yCount; y++ )
				{
					listY.Add( points.GetRange( index, zCount ).ToList() );
					index += zCount;
				}

				listX.Add( listY );
			}

			return listX;
		}

		private static async Task CreateStructuralWalls( TSD.API.Remoting.Structure.IModel model, IReadOnlyList<List<List<IConstructionPoint>>> constructionPoints )
		{
			using var attrSet1 = await model.CreateStructuralWallAttributeSetAsync();

			// Set initial properties for a wall
			await attrSet1.StructuralWallData.Value.MaterialType.SetValueAndUpdateAsync( MaterialType.Concrete );
			await attrSet1.StructuralWallData.Value.Fabrication.SetValueAndUpdateAsync( MemberFabrication.Reinforced );
			await attrSet1.StructuralWallData.Value.SupportData.Value.DegreeOfFreedom.SetValueAndUpdateAsync( DegreeOfFreedom.Fy | DegreeOfFreedom.Fz | DegreeOfFreedom.Mz );
			await attrSet1.StructuralWallPanelData.Value.Thickness.SetValueAndUpdateAsync( 300.0 );
			await attrSet1.StructuralWallPanelData.Value.Alignment.SetValueAndUpdateAsync( WallPanelAlignment.Front );
			await attrSet1.StructuralWallPanelData.Value.Material.SetValueAndUpdateAsync( attrSet1.StructuralWallPanelData.Value.Material.ValidValues.FirstOrDefault( item => item.Name == "C35/45" ) );
			await attrSet1.StructuralWallPanelData.Value.ExtensionLeftEnd.SetValueAndUpdateAsync( 100.0 );

			var createParams = new List<StructuralWallParams>();

			// These points define the outline of the wall
			var points = new List<IConstructionPoint>
			{
				constructionPoints[2][0][0],
				constructionPoints[3][0][0],
				constructionPoints[3][0][2],
				constructionPoints[2][0][2],
			};

			createParams.Add( new StructuralWallParams( points, attrSet1 ) );

			using var attrSet2 = await model.CreateStructuralWallAttributeSetAsync();

			await attrSet2.StructuralWallData.Value.MaterialType.SetValueAndUpdateAsync( MaterialType.Concrete );
			await attrSet2.StructuralWallData.Value.Fabrication.SetValueAndUpdateAsync( MemberFabrication.Reinforced );
			await attrSet2.StructuralWallData.Value.SupportData.Value.DegreeOfFreedom.SetValueAndUpdateAsync( DegreeOfFreedom.Fy | DegreeOfFreedom.Fz | DegreeOfFreedom.Mz );
			await attrSet2.StructuralWallPanelData.Value.Thickness.SetValueAndUpdateAsync( 400.0 );
			await attrSet2.StructuralWallPanelData.Value.Alignment.SetValueAndUpdateAsync( WallPanelAlignment.User );
			await attrSet2.StructuralWallPanelData.Value.Material.SetValueAndUpdateAsync( attrSet2.StructuralWallPanelData.Value.Material.ValidValues.FirstOrDefault( item => item.Name == "C35/45" ) );
			await attrSet2.StructuralWallPanelData.Value.AlignmentOffset.SetValueAndUpdateAsync( 100.0 );
			await attrSet2.StructuralWallPanelData.Value.ExtensionRightEnd.SetValueAndUpdateAsync( 100.0 );

			points = new List<IConstructionPoint>
			{
				constructionPoints[3][0][0],
				constructionPoints[3][1][0],
				constructionPoints[3][1][2],
				constructionPoints[3][0][2],
			};

			createParams.Add( new StructuralWallParams( points, attrSet2 ) );

			await model.CreateEntityAsync( createParams );

			Console.WriteLine( "Structural walls created" );
		}

		private static async Task CreateConcreteColumns( TSD.API.Remoting.Structure.IModel model, IReadOnlyList<List<List<IConstructionPoint>>> constructionPoints )
		{
			using var memberAttributeSet = await model.CreateMemberAttributeSetAsync();

			// Set initial properties for the columns
			await memberAttributeSet.MemberType.SetValueAndUpdateAsync( MemberType.Column );
			await memberAttributeSet.MaterialType.SetValueAndUpdateAsync( MaterialType.Concrete );
			await memberAttributeSet.Fabrication.SetValueAndUpdateAsync( MemberFabrication.Reinforced );
			await memberAttributeSet.AutoDesign.SetValueAndUpdateAsync( true );
			await memberAttributeSet.AutoDesignOption.SetValueAndUpdateAsync( AutoDesignOption.StartingFromMinima );
			await memberAttributeSet.RotationOption.SetValueAndUpdateAsync( RotationOption.Degrees0 );
			await memberAttributeSet.Material.SetValueAndUpdateAsync( memberAttributeSet.Material.ValidValues.FirstOrDefault( item => item.Name == "C35/45" ) );
			await memberAttributeSet.AutoAlign.SetValueAndUpdateAsync( false );
			await memberAttributeSet.Alignment.Value.SnapLevelMajor.SetValueAndUpdateAsync( SectionSnapLevel.Center );
			await memberAttributeSet.Alignment.Value.SnapLevelMinor.SetValueAndUpdateAsync( SectionSnapLevel.Center );

			var points = new List<IConstructionPoint>();

			for( var z = 0; z < 3; z++ )
			{
				points.Add( constructionPoints[2][1][z] );
			}

			var memberParams = new List<MemberParams>()
			{
				new MemberParams( points, memberAttributeSet ),
			};

			await model.CreateEntityAsync( memberParams );

			// Change the initial parameters for the next column so that the outside of the column aligns with the gridline
			await memberAttributeSet.Alignment.Value.SnapLevelMajor.SetValueAndUpdateAsync( SectionSnapLevel.Outline1 );

			points = new List<IConstructionPoint>();

			for( var z = 0; z < 3; z++ )
			{
				points.Add( constructionPoints[2][2][z] );
			}

			memberParams = new List<MemberParams>()
			{
				new MemberParams( points, memberAttributeSet ),
			};

			await model.CreateEntityAsync( memberParams );

			// Change the initial parameters for the next column so that the outsides of the column align with the gridlines
			await memberAttributeSet.Alignment.Value.SnapLevelMajor.SetValueAndUpdateAsync( SectionSnapLevel.Outline1 );
			await memberAttributeSet.Alignment.Value.SnapLevelMinor.SetValueAndUpdateAsync( SectionSnapLevel.Outline1 );

			points = new List<IConstructionPoint>();

			for( var z = 0; z < 3; z++ )
			{
				points.Add( constructionPoints[3][2][z] );
			}

			memberParams = new List<MemberParams>()
			{
				new MemberParams( points, memberAttributeSet ),
			};

			await model.CreateEntityAsync( memberParams );

			Console.WriteLine( "Concrete columns created" );
		}

		private static async Task CreateSteelColumns( TSD.API.Remoting.Structure.IModel model, IReadOnlyList<List<List<IConstructionPoint>>> constructionPoints )
		{
			using var memberAttributeSet = await model.CreateMemberAttributeSetAsync();

			// Set initial properties for the columns
			await memberAttributeSet.MemberType.SetValueAndUpdateAsync( MemberType.Column );
			await memberAttributeSet.MaterialType.SetValueAndUpdateAsync( MaterialType.Steel );
			await memberAttributeSet.Construction.SetValueAndUpdateAsync( MemberConstruction.SteelColumn );
			await memberAttributeSet.Fabrication.SetValueAndUpdateAsync( MemberFabrication.Rolled );
			await memberAttributeSet.AutoDesign.SetValueAndUpdateAsync( true );
			await memberAttributeSet.RotationOption.SetValueAndUpdateAsync( RotationOption.Degrees0 );
			await memberAttributeSet.Material.SetValueAndUpdateAsync( memberAttributeSet.Material.ValidValues.FirstOrDefault( item => item.Name == "S355" ) );

			var memberParams = new List<MemberParams>();

			for( var x = 0; x < 2; x++ )
			{
				for( var y = 0; y < 3; y++ )
				{
					var points = new List<IConstructionPoint>();

					for( var z = 0; z < 3; z++ )
					{
						points.Add( constructionPoints[x][y][z] );
					}

					memberParams.Add( new MemberParams( points, memberAttributeSet ) );
				}
			}

			await model.CreateEntityAsync( memberParams );

			Console.WriteLine( "Steel columns created" );
		}

		private static async Task CreateConcreteBeams( TSD.API.Remoting.Structure.IModel model, IReadOnlyList<List<List<IConstructionPoint>>> constructionPoints )
		{
			using var memberAttributeSet = await model.CreateMemberAttributeSetAsync();

			// Set initial properties for the beams
			await memberAttributeSet.MemberType.SetValueAndUpdateAsync( MemberType.Beam );
			await memberAttributeSet.MaterialType.SetValueAndUpdateAsync( MaterialType.Concrete );
			await memberAttributeSet.Construction.SetValueAndUpdateAsync( MemberConstruction.ConcreteBeam );
			await memberAttributeSet.Fabrication.SetValueAndUpdateAsync( MemberFabrication.Reinforced );
			await memberAttributeSet.AutoDesign.SetValueAndUpdateAsync( true );
			await memberAttributeSet.AutoDesignOption.SetValueAndUpdateAsync( AutoDesignOption.StartingFromMinima );
			await memberAttributeSet.GravityOnly.SetValueAndUpdateAsync( true );
			await memberAttributeSet.Material.SetValueAndUpdateAsync( memberAttributeSet.Material.ValidValues.FirstOrDefault( item => item.Name == "C35/45" ) );
			await memberAttributeSet.StartReleases.Value.DegreeOfFreedom.SetValueAndUpdateAsync( DegreeOfFreedom.Fx | DegreeOfFreedom.Fy | DegreeOfFreedom.Fz | DegreeOfFreedom.Mx | DegreeOfFreedom.My );
			await memberAttributeSet.StartReleases.Value.MajorRotationalStiffness.Value.Type.SetValueAndUpdateAsync( SpringStiffness.Fixed );
			await memberAttributeSet.EndReleases.Value.DegreeOfFreedom.SetValueAndUpdateAsync( DegreeOfFreedom.Fx | DegreeOfFreedom.Fy | DegreeOfFreedom.Fz | DegreeOfFreedom.Mx | DegreeOfFreedom.Mz );
			await memberAttributeSet.EndReleases.Value.MinorRotationalStiffness.Value.Type.SetValueAndUpdateAsync( SpringStiffness.Fixed );
			await memberAttributeSet.AutoAlign.SetValueAndUpdateAsync( false );
			await memberAttributeSet.ConsiderFlanges.SetValueAndUpdateAsync( true );
			await memberAttributeSet.IncreaseReinforcementIfDeflectionCheckFails.SetValueAndUpdateAsync( true );
			await memberAttributeSet.IncludeFlangesInAnalysis.SetValueAndUpdateAsync( true );

			var memberParams = new List<MemberParams>();

			// Four concrete beams will be created
			for( var z = 1; z < 3; z++ )
			{
				var points = new List<IConstructionPoint>();

				for( var x = 2; x < 4; x++ )
				{
					points.Add( constructionPoints[x][1][z] );
				}

				memberParams.Add( new MemberParams( points, memberAttributeSet ) );

				points = new List<IConstructionPoint>();

				for( var y = 0; y < 3; y++ )
				{
					points.Add( constructionPoints[2][y][z] );
				}

				memberParams.Add( new MemberParams( points, memberAttributeSet ) );

				points = new List<IConstructionPoint>();

				for( var x = 2; x < 4; x++ )
				{
					points.Add( constructionPoints[x][2][z] );
				}

				memberParams.Add( new MemberParams( points, memberAttributeSet ) );

				points = new List<IConstructionPoint>();

				for( var y = 1; y < 3; y++ )
				{
					points.Add( constructionPoints[3][y][z] );
				}

				memberParams.Add( new MemberParams( points, memberAttributeSet ) );
			}

			var members = (await model.CreateEntityAsync( memberParams )).Cast<IMember>().ToList();

			// Alignments will be changed for the exterior beams so that the outside edges align with the gridlines
			var alignmentChangeMembers = new List<IMember>()
			{
				members[2],
				members[6],
			};

			var collectableEntities = new List<IEntity>( alignmentChangeMembers );
			var collectableEntitySpans = new List<ISubEntity>();

			foreach( var member in alignmentChangeMembers )
			{
				var memberSpans = (await member.GetSpanAsync( new[] { 0, member.SpanCount.Value - 1 } )).ToList();

				collectableEntitySpans.AddRange( memberSpans );

				foreach( var memberSpan in memberSpans )
				{
					memberSpan.Alignment.Value.SnapLevelMinor.Value = SectionSnapLevel.Outline0;
				}
			}

			alignmentChangeMembers = new List<IMember>()
			{
				members[3],
				members[7],
			};

			collectableEntities.AddRange( alignmentChangeMembers );

			foreach( var member in alignmentChangeMembers )
			{
				var memberSpans = (await member.GetSpanAsync( new[] { 0, member.SpanCount.Value - 1 } )).ToList();

				collectableEntitySpans.AddRange( memberSpans );

				foreach( var memberSpan in memberSpans )
				{
					memberSpan.Alignment.Value.SnapLevelMinor.Value = SectionSnapLevel.Outline1;
				}
			}

			await model.CreateEntityCollector( collectableEntities ).ApplyEntityAsync();
			await model.CreateSubEntityCollector( collectableEntitySpans ).ApplySubEntityAsync();

			Console.WriteLine( "Concrete beams created" );
		}

		private static async Task CreateSteelBeams( TSD.API.Remoting.Structure.IModel model, IReadOnlyList<List<List<IConstructionPoint>>> constructionPoints )
		{
			using var memberAttributeSet = await model.CreateMemberAttributeSetAsync();

			// Set initial properties for the beams
			await memberAttributeSet.MemberType.SetValueAndUpdateAsync( MemberType.Beam );
			await memberAttributeSet.MaterialType.SetValueAndUpdateAsync( MaterialType.Steel );
			await memberAttributeSet.Construction.SetValueAndUpdateAsync( MemberConstruction.SteelBeam );
			await memberAttributeSet.Fabrication.SetValueAndUpdateAsync( MemberFabrication.Rolled );
			await memberAttributeSet.AutoDesign.SetValueAndUpdateAsync( true );
			await memberAttributeSet.GravityOnly.SetValueAndUpdateAsync( true );
			await memberAttributeSet.Material.SetValueAndUpdateAsync( memberAttributeSet.Material.ValidValues.FirstOrDefault( item => item.Name == "S355" ) );
			await memberAttributeSet.StartReleases.Value.DegreeOfFreedom.SetValueAndUpdateAsync( DegreeOfFreedom.Fx | DegreeOfFreedom.Fy | DegreeOfFreedom.Fz | DegreeOfFreedom.Mx | DegreeOfFreedom.My );
			await memberAttributeSet.StartReleases.Value.MajorRotationalStiffness.Value.Type.SetValueAndUpdateAsync( SpringStiffness.Fixed );
			await memberAttributeSet.EndReleases.Value.DegreeOfFreedom.SetValueAndUpdateAsync( DegreeOfFreedom.Fx | DegreeOfFreedom.Fy | DegreeOfFreedom.Fz | DegreeOfFreedom.Mx | DegreeOfFreedom.Mz );
			await memberAttributeSet.EndReleases.Value.MinorRotationalStiffness.Value.Type.SetValueAndUpdateAsync( SpringStiffness.Fixed );

			var memberParams = new List<MemberParams>();

			for( var z = 1; z < 3; z++ )
			{
				for( var y = 0; y < 3; y++ )
				{
					var points = new List<IConstructionPoint>();

					for( var x = 0; x < 3; x++ )
					{
						points.Add( constructionPoints[x][y][z] );
					}

					memberParams.Add( new MemberParams( points, memberAttributeSet ) );
				}

				for( var x = 0; x < 2; x++ )
				{
					var points = new List<IConstructionPoint>();

					for( var y = 0; y < 3; y++ )
					{
						points.Add( constructionPoints[x][y][z] );
					}

					memberParams.Add( new MemberParams( points, memberAttributeSet ) );
				}
			}

			var members = (await model.CreateEntityAsync( memberParams )).Cast<IMember>().ToList();

			Console.WriteLine( "Steel beams created" );
		}

		private static async Task CreateSlabItems( TSD.API.Remoting.Structure.IModel model, IReadOnlyList<List<List<IConstructionPoint>>> constructionPoints )
		{
			using var attrSet = await model.CreateSlabItemAttributeSetAsync( SlabItemAttributeSetParams.General() );

			// Set initial properties for the slabs
			await attrSet.SlabData.Value.SlabType.SetValueAndUpdateAsync( SlabType.OnBeams );
			await attrSet.SlabData.Value.ConcreteType.SetValueAndUpdateAsync( ConcreteType.Normal );
			await attrSet.SlabData.Value.Material.SetValueAndUpdateAsync( attrSet.SlabData.Value.Material.ValidValues.FirstOrDefault( item => item.Name == "C35/45" ) );
			await attrSet.SlabData.Value.Depth.SetValueAndUpdateAsync( 200.0 );

			var createParams = new List<SlabItemParams>();

			var entities = new List<ISlabItem>();

			// A slab item will be created within each bay on the first and second levels
			for( int z = 1; z < 3; z++ )
			{
				bool firstSlabItemOnLevel = true;

				for( int y = 0; y < 2; y++ )
				{
					for( int x = 0; x < 3; x++ )
					{
						var points = new List<IConstructionPoint>
						{
							constructionPoints[x][y][z],
							constructionPoints[(x + 1) % 4][y][z],
							constructionPoints[(x + 1) % 4][(y + 1) % 3][z],
							constructionPoints[x][(y + 1) % 3][z],
						};

						if( firstSlabItemOnLevel )
						{
							// First slab item at this level - this will create a new Slab
							createParams.Add( SlabItemParams.SearchForSlab( points, attrSet ) );
						}
						else
						{
							// Add slab item to existing slab on this level
							int slabIndex = entities.LastOrDefault()?.SlabIndex.Value ?? 0;
							createParams.Add( SlabItemParams.AddToSlab( points, attrSet, slabIndex ) );
						}

						entities.AddRange( (await model.CreateEntityAsync( createParams )).Cast<ISlabItem>() );

						createParams.Clear();
						firstSlabItemOnLevel = false;
					}
				}
			}

			Console.WriteLine( "Slab items created" );
		}

		private static async Task CreateIsolatedFoundations( TSD.API.Remoting.Structure.IModel model )
		{
			using var padBaseAttrSet = await model.CreatePadBaseAttributeSetAsync();

			var padBaseEntities = new List<IEntity>();

			var columns = (await model.GetMembersAsync()).Where( item => item.MemberType.Value == MemberType.Column ).ToList();

			// Pad bases will be added beneath all concrete columns and walls
			padBaseEntities.AddRange( columns.Where( c => c.MaterialType.Value == MaterialType.Concrete ) );

			var structuralWalls = (await model.GetStructuralWallsAsync()).ToList();
			padBaseEntities.AddRange( structuralWalls );

			var createParams = padBaseEntities.Select( e => new IsolatedFoundationParams( e, padBaseAttrSet ) ).ToList();

			var entities = (await model.CreateEntityAsync( createParams )).Cast<IIsolatedFoundation>().ToList();

			var changeEntities = new List<IEntity>();

			// Edit the depth of all pad bases, and the length of bases under walls
			foreach( var padBase in entities )
			{
				padBase.IsolatedFoundationData.Value.Depth.Value = 500.0;

				if( padBase.SupportedMemberType.Value == SupportedMemberType.Wall )
				{
					var wall = (await model.GetStructuralWallsAsync( Enumerable.Repeat( padBase.SupportedEntityInfo.Value.Index, 1 ) )).First();
					var firstPanel = (await wall.GetSpanAsync( new[] { 0 } )).First();

					// Set the length of the foundation to extend 1m beyond the bottom panel ends
					padBase.IsolatedFoundationData.Value.LengthDir1.Value = firstPanel.BottomSegment.Value.Magnitude.Value + firstPanel.WallPanelData.Value.ExtensionLeftEnd.Value
						+ firstPanel.WallPanelData.Value.ExtensionRightEnd.Value + 2000.0;
				}

				changeEntities.Add( padBase );
			}

			await model.CreateEntityCollector( changeEntities ).ApplyEntityAsync();

			// A pile type must be defined in the model to allow creation of pile caps
			var pileType1 = await model.CreatePileTypeAsync( new PileTypeParams( "PileType1" )
			{
				PileTypeInstallationType = PileTypeInstallationType.Driven,
				PileTypeShape = PileTypeShape.Circular,
				Dimension = 400.0,
				Length = 12.0 * 1.0E3,
				Embedment = 120.0,
				AxialCompressiveResistance = 500.0 * 1.0E03,
				AxialTensileResistance = 500.0 * 1.0E03,
				AxialCompressiveResistanceEcStr = 500.0 * 1.0E03,
				AxialTensileResistanceEcStr = 500.0 * 1.0E03,
				Linearity = SpringStiffness.SpringNonLinear,
				CompressionStiffnessVertical = 12000.0,
				TensionStiffnessVertical = 15000.0,
				CompressionLimitVertical = 1200.0 * 1.0E3,
				TensionLimitVertical = 1500.0 * 1.0E3,
				HorizontalRestraint = SpringStiffness.Fixed,
				LoadTransferType = PileTypeLoadTransferType.EndBearing,
				LateralResistance = 20.0 * 1.0E03,
				LateralResistanceStr = 20.0 * 1.0E03,
				VolumeAdjustmentPercentage = 1.0,
			} );

			using var pileCapAttrSet = await model.CreatePileCapAttributeSetAsync( new PileCapAttributeSetParams( pileType1 ) );

			var pileCapEntities = new List<IEntity>();

			// Pile caps will be added beneath all steel columns
			pileCapEntities.AddRange( columns.Where( c => c.MaterialType.Value == MaterialType.Steel ) );

			createParams = pileCapEntities.Select( e => new IsolatedFoundationParams( e, pileCapAttrSet ) ).ToList();

			entities = (await model.CreateEntityAsync( createParams )).Cast<IIsolatedFoundation>().ToList();

			// Edit the depth and number of piles for each pile cap
			foreach( var pileCap in entities )
			{
				await pileCap.IsolatedFoundationData.Value.Depth.SetValueAndUpdateAsync( 800.0 );
				await ((IPileCapData) pileCap.IsolatedFoundationData.Value).NumberOfPiles.SetValueAndUpdateAsync( 3 );
			}

			Console.WriteLine( "Isolated foundations created" );
		}

		private static async Task EditSteelBeamSections( TSD.API.Remoting.Structure.IModel model, IReadOnlyList<List<List<IConstructionPoint>>> constructionPoints )
		{
			var members = await model.GetMembersAsync();

			// Get the list of valid sections from the database
			var sectionFactory = model.SectionFactory;
			var sections = await sectionFactory.GetNonParametricSections( TSD.API.Remoting.HeadCode.EC, Country.UK, SystemType.Metric, MaterialType.Steel, SectionGeometry.ISymmetric,
				SectionType.UniversalBeam );

			// Select the new section size from the list
			var newSection = sections.First( s => s.LongName.Contains( "610x229x101" ) );

			// Identify the start and end positions of the beams on gridline 1 above ground level
			var startPointsIndices = constructionPoints[0][0].Where( c => c.Coordinates.Value.Z > 0.1 ).Select( p => p.Index ).ToList();
			var endPointsIndices = constructionPoints[0][2].Where( c => c.Coordinates.Value.Z > 0.1 ).Select( p => p.Index ).ToList();

			var steelBeams = members.Where( m => m.MemberType.Value is MemberType.Beam && m.MaterialType.Value is MaterialType.Steel );

			var beamsToChange = steelBeams.Where( ShouldBeamChange );

			foreach( var beam in beamsToChange )
			{
				foreach( var span in await beam.GetSpanAsync() )
				{
					await (span.ElementSection.Value as IMemberSection)?.PhysicalSection.SetValueAndUpdateAsync( newSection );
				}
			}

			Console.WriteLine( "Steel beam sections edited" );

			bool ShouldBeamChange( IMember member ) => startPointsIndices.Contains( member.MemberNodes.Value.First().Value.ConstructionPointIndex.Value )
				&& endPointsIndices.Contains( member.MemberNodes.Value.Last().Value.ConstructionPointIndex.Value );
		}

		private static async Task EditConcreteColumnSections( TSD.API.Remoting.Structure.IModel model, IReadOnlyList<List<List<IConstructionPoint>>> constructionPoints )
		{
			var members = await model.GetMembersAsync();

			// Create the new section
			var sectionFactory = model.SectionFactory;
			var newSection = await sectionFactory.GetParametricRectangularSection( MaterialType.Concrete, 500.0, 500.0 );

			// Identify the column at grid intersection 4C
			var startPoint = constructionPoints[3][2][0];
			var endPoint = constructionPoints[3][2][2];

			var columnToChange = members.First( ShouldColumnChange );

			foreach( var span in await columnToChange.GetSpanAsync() )
			{
				await (span.ElementSection.Value as IMemberSection)?.PhysicalSection.SetValueAndUpdateAsync( newSection );
			}

			Console.WriteLine( "Concrete column section edited" );

			bool ShouldColumnChange( IMember column ) => startPoint.Index == column.MemberNodes.Value.First().Value.ConstructionPointIndex.Value
				&& endPoint.Index == column.MemberNodes.Value.Last().Value.ConstructionPointIndex.Value;
		}

		#endregion
	}
}
