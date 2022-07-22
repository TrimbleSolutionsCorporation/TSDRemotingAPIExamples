using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TSD.API.Remoting.Common;
using TSD.API.Remoting.Loading;
using TSD.API.Remoting.Loading.Create;
using TSD.API.Remoting.Structure;

namespace CreatingAndAnalyzingModel
{
	/// <summary>
	/// Contains methods to create the model loading
	/// </summary>
	public class CreateLoading
	{
		#region Methods

		public async Task Create( IModel model )
		{
			await CreateLoads( model );

			await CreateCombinations( model );

			Console.WriteLine( "Loading created" );
		}

		private static async Task CreateLoads( IModel model )
		{
			var loadcases = (await model.GetLoadcasesAsync()).ToList();

			var createLoadParams = new List<LoadParams>();

			var levels = (await model.GetLevelsAsync()).ToList();

			// Create some level loading for each loadcase
			foreach( var loadcase in loadcases )
			{
				var load = 0.0;
				if( loadcase.Name.Contains( "Dead" ) )
				{
					load = 0.004; // 0.004 N/mm^2 = 4kN/m^2
				}
				else if( loadcase.Name.Contains( "Services" ) )
				{
					load = 0.003; // 0.003 N/mm^2 = 3kN/m^2
				}
				else if( loadcase.Name.Contains( "Imposed" ) )
				{
					load = 0.005; // 0.005 N/mm^2 = 5kN/m^2
				}
				else
				{
					continue;
				}

				foreach( var level in levels.Where( l => l.Level.Value > 0.001 ) ) // ignore base level
				{
					var planeLoad = new ConstructionPlaneLoadParams( level, load );
					createLoadParams.Add( planeLoad );
				}

				await loadcase.CreateLoadAsync( createLoadParams );
				createLoadParams.Clear();
			}

			// Create a lateral load on the structure for the imposed load case
			var imposedLoadcase = loadcases.First( l => l.Type.Value == LoadcaseType.Imposed );

			var members = await model.GetMembersAsync();
			var constructionPoints = await model.GetConstructionPointsAsync();

			// Find edge beam points where X co-ordinate is 0 at first floor
			var edgeBeamPoints = constructionPoints.Where( c => Math.Abs( c.Coordinates.Value.X ) < 0.001 && Math.Abs( c.Coordinates.Value.Z - 3000.0 ) < 0.001 );

			foreach( var member in members.Where( m => m.MemberType.Value == MemberType.Beam ) )
			{
				foreach( var span in await member.GetSpanAsync( new[] { 0, member.SpanCount.Value - 1 } ) )
				{
					if( edgeBeamPoints.Select( p => p.Index ).ToList().Contains( span.EndMemberNode.ConstructionPointIndex.Value ) )
					{
						var memberSpanInfo = new MemberSpanInfo( member.Id, span.Id );
						var memberLateralLoad = new MemberFullUniformlyDistributedLoadParams( memberSpanInfo, 10.0, MemberLoadParams.LoadDirectionLocal.Y );
						createLoadParams.Add( memberLateralLoad );
					}
				}
			}

			await imposedLoadcase.CreateLoadAsync( createLoadParams );

			createLoadParams.Clear();

			// Create a snow load
			var loadcaseParams = new List<EntityParams>()
			{
				new LoadcaseParams(),
			};

			await model.CreateEntityAsync( loadcaseParams );

			var snowLoadcase = (await model.GetLoadcasesAsync()).Last();

			await snowLoadcase.UserName.SetValueAndUpdateAsync( "Snow" );
			await snowLoadcase.Type.SetValueAndUpdateAsync( LoadcaseType.Snow );

			var roofSlab = (await model.GetSlabsAsync()).Last();
			var slabSnowLoad = new SlabLoadParams( roofSlab, 0.00063 );
			createLoadParams.Add( slabSnowLoad );
			await snowLoadcase.CreateLoadAsync( createLoadParams );
		}

		private static async Task CreateCombinations( IModel model )
		{
			var loadcases = (await model.GetLoadcasesAsync()).ToList();

			var cbParams = new List<EntityParams>()
			{
				new CombinationParams(),
			};

			// Create 4 combinations
			for( int i = 0; i < 4; i++ )
			{
				await model.CreateEntityAsync( cbParams );
			}

			var combinations = (await model.GetCombinationsAsync()).ToList();

			foreach( var combination in combinations )
			{
				switch( combination.Index )
				{
					case 1:
						await combination.UserName.SetValueAndUpdateAsync( "Dead + Imposed" );
						break;
					case 2:
						await combination.UserName.SetValueAndUpdateAsync( "Dead only" );
						break;
					case 3:
						await combination.UserName.SetValueAndUpdateAsync( "Imposed only" );
						break;
					case 4:
						await combination.UserName.SetValueAndUpdateAsync( "Dead + Imposed + Snow" );
						break;
				}

				// Add each loadcase to the combination
				foreach( var loadcase in loadcases )
				{
					if( AddLoadcaseToCombination( combination.Index, loadcase.Type.Value ) )
						await combination.AddLoadcaseAsync( loadcase );
				}
			}

			// Create an envelope of all combinations
			var envParams = new List<EntityParams>()
			{
				new EnvelopeParams(),
			};

			await model.CreateEntityAsync( envParams );

			var envelope = (await model.GetEnvelopesAsync()).ToList().FirstOrDefault();

			if( envelope == null )
				return;

			foreach( var combination in combinations )
			{
				await envelope.AddCombinationAsync( combination );
			}

			static bool AddLoadcaseToCombination( int combinationIndex, LoadcaseType loadcaseType )
			{
				if( combinationIndex == 1 && loadcaseType is LoadcaseType.Dead or LoadcaseType.Imposed )
					return true;

				if( combinationIndex == 2 && loadcaseType is LoadcaseType.Dead )
					return true;

				if( combinationIndex == 3 && loadcaseType is LoadcaseType.Imposed )
					return true;

				if( combinationIndex == 4 && loadcaseType is LoadcaseType.Dead or LoadcaseType.Imposed or LoadcaseType.Snow )
					return true;

				return false;
			}
		}

		#endregion
	}
}
