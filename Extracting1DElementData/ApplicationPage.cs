using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using ClosedXML.Excel;
using TSD.API.Remoting.Sections;
using TSD.API.Remoting.Solver;

namespace TsdApiRemotingSample
{
	public partial class ApplicationPage : Form
	{
		#region Fields

		/// <summary>
		/// The TSD application object
		/// </summary>
		private readonly TSD.API.Remoting.IApplication _application;

		private const string Filter = "TSD Project file (*.tsmd)|*.tsmd";
		private const string DefaultExt = ".tsmd";
		private const string ExcelFilter = "Excel file (*.xlsx)|*.xlsx";
		private const string ExcelExt = "*.xlsx";

		private const string Fixed = "Fixed";
		private const string Free = "Free";

		#endregion

		public ApplicationPage( TSD.API.Remoting.IApplication application )
		{
			_application = application;
			InitializeComponent();
			
			// Connect to application events so we know if application is closed or the document or model change
			_application.Disconnected += OnApplicationDisconnected;
			_application.ModelClosed += OnApplicationModelChanged;
			_application.ModelOpened += OnApplicationModelChanged;
			
			TopLevel = false;
			Load += OnLoad;
		}

		#region Events

		public event EventHandler Disconnected;

		#endregion

		#region EventHandlers

		/// <summary>
		/// Initializes controls of this page before it is displayed for the first time
		/// </summary>
		private async void OnLoad( object sender, EventArgs e )
		{
			Dock = DockStyle.Fill;
			_versionLabel.Text = await _application.GetVersionStringAsync();
			await FillNameAsync();
			_connectionIndicator.Text = _application.Connected ? "CONNECTED" : "DISCONNECTED";
			_connectionIndicator.BackColor = _application.Connected ? Color.Green : Color.Red;
			await FillDocumentAsync();
		}

		/// <summary>
		/// Handles change of model in TSD
		/// </summary>
		private void OnApplicationModelChanged( object sender, TSD.API.Remoting.ModelEventArgs e )
		{
			BeginInvoke( new Action( async () => { await FillNameAsync(); await FillDocumentAsync(); _refreshSolverModels.PerformClick(); } ) );
		}

		/// <summary>
		/// Handles cases like network failure or application termination
		/// </summary>
		private void OnApplicationDisconnected( object sender, EventArgs e )
		{
			// Event is passed to MainForm.OnPageDisconnected
			BeginInvoke( new Action( () => Disconnected?.Invoke( this, EventArgs.Empty ) ) );
		}

		/// <summary>
		/// Closes the document open in TSD, if the document is dirty user is asked to save/discard the changes
		/// </summary>
		private async void OnCloseDocumentClick( object sender, EventArgs e )
		{
			SetDocumentButtonsState( false );
			var document = await _application.GetDocumentAsync();

			await document.CloseAsync( false );
			await FillDocumentAsync();
		}

		/// <summary>
		/// Closes the document open is TSD without user interaction, any changes are lost
		/// </summary>
		private async void OnForceCloseDocumentClick( object sender, EventArgs e )
		{
			SetDocumentButtonsState( false );
			var document = await _application.GetDocumentAsync();

			await document.CloseAsync( true );
			await FillDocumentAsync();
		}

		/// <summary>
		/// Saves the document open in TSD
		/// </summary>
		private async void OnSaveDocumentClick( object sender, EventArgs e )
		{
			SetDocumentButtonsState( false );
			var document = await _application.GetDocumentAsync();

			await document.SaveToAsync( document.Path, false );
			await FillDocumentAsync();
		}

		/// <summary>
		/// Saves the document open in TSD to a new file
		/// </summary>
		private async void OnSaveDocumentToClick( object sender, EventArgs e )
		{
			SetDocumentButtonsState( false );

			var document = await _application.GetDocumentAsync();

			using var fd = new SaveFileDialog
			{
				CheckPathExists = true,
				Filter = Filter,
				DefaultExt = DefaultExt,
				OverwritePrompt = true,
			};

			if( fd.ShowDialog() != DialogResult.OK )
				return;

			await document.SaveToAsync( fd.FileName, false );
			await FillDocumentAsync();
		}

		/// <summary>
		/// Saves only core data of the model open in TSD
		/// </summary>
		private async void SaveDocumentCoreToClick( object sender, EventArgs e )
		{
			SetDocumentButtonsState( false );
			var document = await _application.GetDocumentAsync();
			using var fd = new SaveFileDialog
			{
				CheckPathExists = true,
				Filter = Filter,
				DefaultExt = DefaultExt,
				OverwritePrompt = true,
			};
			if( fd.ShowDialog() != DialogResult.OK )
				return;

			await document.SaveToAsync( fd.FileName, true );
			await FillDocumentAsync();
		}

		/// <summary>
		/// Opens a new document in TSD, if the current document is dirty user is asked to save/discard the changes
		/// </summary>
		private async void OnOpenDocumentClick( object sender, EventArgs e ) => await OpenDocument( false );

		/// <summary>
		/// Opens a new document in TSD without user interaction, any changes in current document are lost
		/// </summary>
		private async void OnOpenDocumentForceClick( object sender, EventArgs e ) => await OpenDocument( true );

		/// <summary>
		/// Opens TSD document in this client and then passes it to TSD application via stream
		/// </summary>
		private async void OnOpenDocumentStreamClick( object sender, EventArgs e )
		{
			SetDocumentButtonsState( false );
			using var fd = new OpenFileDialog
			{
				CheckPathExists = true,
				Filter = Filter,
				DefaultExt = DefaultExt,
			};

			if( fd.ShowDialog() == DialogResult.OK )
			{
				using var stream = fd.OpenFile();
				await _application.OpenDocumentAsync( (System.IO.FileStream) stream, false );
			}

			await FillDocumentAsync();
		}

		/// <summary>
		/// Fetches solver models available to combo box _solverModels
		/// </summary>
		private async void OnRefreshSolverModelsClick( object sender, EventArgs e )
		{
			_solverModels.Enabled = _refreshSolverModels.Enabled = _exportSolverModel.Enabled = false;

			var oldSelection = _solverModels.SelectedItem as IModel;

			_solverModels.BeginUpdate();

			try
			{
				_solverModels.Items.Clear();

				var document = await _application.GetDocumentAsync();
				var model = await document.GetModelAsync();
				var solverModels = await model.GetSolverModelsAsync( ValidAnalysisTypes() );

				_solverModels.Items.AddRange( solverModels
					.Where( sm => sm != null ).Cast<object>()
					.ToArray() );

				_solverModels.SelectedIndex = 0;

				if( oldSelection != null )
				{
					for( int i = 0; i < _solverModels.Items.Count; i++ )
					{
						if( ((IModel) _solverModels.Items[i]).Id != oldSelection.Id )
							continue;

						_solverModels.SelectedIndex = i;
						break;
					}
				}
			}
			finally
			{
				_solverModels.EndUpdate();
			}

			_solverModels.Enabled = _refreshSolverModels.Enabled = _exportSolverModel.Enabled = true;

			static IEnumerable<AnalysisType> ValidAnalysisTypes() => new[]
			{
				AnalysisType.None,
				AnalysisType.FirstOrderLinear,
				AnalysisType.FirstOrderNonLinear,
				AnalysisType.SecondOrderLinear,
				AnalysisType.SecondOrderNonLinear
			};
		}

		/// <summary>
		/// Handles state of export button if selected solver model changes
		/// </summary>
		private void OnSolverModelsSelectedIndexChanged(object sender, EventArgs e)
		{
			if (!(_solverModels.SelectedItem is IModel solverModel))
			{
				_exportSolverModel.Enabled = false;
				return;
			}

			_exportSolverModel.Enabled = true;
		}

		/// <summary>
		/// Exports some solver model data to an excel workbook
		/// </summary>
		private async void OnExportSolverModelClick(object sender, EventArgs e)
		{
			if (!(_solverModels.SelectedItem is IModel solverModel))
				return;

			try
			{
				_exportSolverModel.Enabled = false;

				using var fd = new SaveFileDialog
				{
					CheckPathExists = true,
					Filter = ExcelFilter,
					DefaultExt = ExcelExt,
					OverwritePrompt = true,
				};

				if (fd.ShowDialog() != DialogResult.OK)
					return;

				UseWaitCursor = true;

				// Get the document
				var document = await _application.GetDocumentAsync();
				// Get the model
				var model = await document.GetModelAsync();

				var workbook = new XLWorkbook();

				// Export solver nodes
				await ExportSolverNodes(workbook);
				// Export solver 1D elements
				await ExportElements1D(workbook);

				// Get analysis results
				var analysisResults = await solverModel.GetResultsAsync();
				// Check if they exist
				if (analysisResults != null)
				{
					// Get results of 3D analysis
					var results3D = await analysisResults.GetAnalysis3DAsync();

					if (results3D != null)
					{
						// Get ids of solved loading cases
						var solvedLoading = (await results3D.GetSolvedLoadingIdsAsync()).ToList();

						// And if there are any
						if (solvedLoading.Any())
						{
							// Collect loading cases (loadcases and combinations) from model
							var loadingCases = (await model.GetLoadcasesAsync(null)).Cast<TSD.API.Remoting.Loading.ILoadingCase>().ToList();
							loadingCases.AddRange((await model.GetCombinationsAsync(null)).Cast<TSD.API.Remoting.Loading.ILoadingCase>());

							// Get id of first solved loading case
							var loadingId = solvedLoading.First();

							// Get the model loading case so we can export its name
							var loadingCase = loadingCases.First(lc => lc.Id == loadingId);

							// Export element end forces
							await ExportElement1DEndForces(workbook, loadingCase, await analysisResults.GetAnalysis3DAsync());
						}
					}
				}

				workbook.SaveAs(fd.FileName);
				workbook.Dispose();
			}
			finally
			{
				UseWaitCursor = false;

				_exportSolverModel.Enabled = true;
			}

			async Task ExportSolverNodes(XLWorkbook wb)
			{
				var nodes = await solverModel.GetNodesAsync(null);

				var ws = wb.Worksheets.Add("Nodes");

				ws.Cell(1, 1).Value = "Node index";
				ws.Cell(1, 2).Value = "X-Coordinate";
				ws.Cell(1, 3).Value = "Y-Coordinate";
				ws.Cell(1, 4).Value = "Z-Coordinate";
				ws.Cell(1, 5).Value = "Fx";
				ws.Cell(1, 6).Value = "Fy";
				ws.Cell(1, 7).Value = "Fz";
				ws.Cell(1, 8).Value = "Mx";
				ws.Cell(1, 9).Value = "My";
				ws.Cell(1, 10).Value = "Mz";

				var row = 3;

				foreach (var node in nodes)
				{
					ws.Cell(row, 1).Value = node.Index;
					ws.Cell(row, 2).Value = node.Coordinates.X;
					ws.Cell(row, 3).Value = node.Coordinates.Y;
					ws.Cell(row, 4).Value = node.Coordinates.Z;
					ws.Cell(row, 5).Value = node.Dof.HasFlag(DegreeOfFreedom.Fx) ? Fixed : Free;
					ws.Cell(row, 6).Value = node.Dof.HasFlag(DegreeOfFreedom.Fy) ? Fixed : Free;
					ws.Cell(row, 7).Value = node.Dof.HasFlag(DegreeOfFreedom.Fz) ? Fixed : Free;
					ws.Cell(row, 8).Value = node.Dof.HasFlag(DegreeOfFreedom.Mx) ? Fixed : Free;
					ws.Cell(row, 9).Value = node.Dof.HasFlag(DegreeOfFreedom.My) ? Fixed : Free;
					ws.Cell(row, 10).Value = node.Dof.HasFlag(DegreeOfFreedom.Mz) ? Fixed : Free;

					row++;
				}

				for (var i = 1; i <= 10; i++)
					ws.Column(i).AdjustToContents();
			}

			async Task ExportElements1D(XLWorkbook wb)
			{
				var elements = await solverModel.GetElements1DAsync(null);

				var ws = wb.Worksheets.Add("Elements 1D");

				ws.Cell(1, 1).Value = "Element index";
				ws.Cell(1, 2).Value = "Start node index";
				ws.Cell(1, 3).Value = "End node index";
				ws.Cell(1, 4).Value = "Section name";
				ws.Cell(1, 5).Value = "Major axis 2nd moment of area";
				ws.Cell(1, 6).Value = "Minor axis 2nd moment of area";
				ws.Cell(1, 7).Value = "Torsion constant";
				ws.Cell(1, 8).Value = "Cross sectional area";
				ws.Cell(1, 9).Value = "Material name";
				ws.Cell(1, 10).Value = "Shear modulus";
				ws.Cell(1, 11).Value = "Poissons ratio";
				ws.Cell(1, 12).Value = "Thermal expansion coefficient";

				var row = 3;

				foreach (var element in elements)
				{
					var section = element.ElementSection as ISolverElementSection;

					ws.Cell(row, 1).Value = element.Index;
					ws.Cell(row, 2).Value = element.GetNodeIndex(0);
					ws.Cell(row, 3).Value = element.GetNodeIndex(1);
					ws.Cell(row, 4).Value = (element.ElementSection as IMemberSection)?.PhysicalSection.Value.LongName;
					ws.Cell(row, 5).Value = section?.MajorAxisSecondMomentOfArea.Value;
					ws.Cell(row, 6).Value = section?.MinorAxisSecondMomentOfArea.Value;
					ws.Cell(row, 7).Value = section?.TorsionConstant.Value;
					ws.Cell(row, 8).Value = section?.CrossSectionalArea.Value;
					ws.Cell(row, 9).Value = element.Material.Name;
					ws.Cell(row, 10).Value = element.Material.ShearModulus;
					ws.Cell(row, 11).Value = element.Material.PoissonsRatio;
					ws.Cell(row, 12).Value = element.Material.ThermalExpansionCoefficient;

					row++;
				}

				for (var i = 1; i <= 12; i++)
					ws.Column(i).AdjustToContents();
			}

			static async Task ExportElement1DEndForces(XLWorkbook wb, TSD.API.Remoting.Loading.ILoadingCase lc, IAnalysis3DResults results)
			{
				var endForces = (await results.GetEndForcesAsync(lc.Id, TSD.API.Remoting.Loading.LoadingResultType.Base, null)).OrderBy(ef => ef.ElementIndex).ToList();

				var ws = wb.Worksheets.Add("Element end forces");

				ws.Cell("A1").Value = "Loadcase: " + lc.Name;

				ws.Cell("A3").Value = "Element index";
				ws.Cell("B3").Value = "Start";
				ws.Range("B3:G3").Row(1).Merge();
				ws.Cell("H3").Value = "End";
				ws.Range("H3:M3").Row(1).Merge();
				ws.Cell(4, 2).Value = "Fx";
				ws.Cell(4, 3).Value = "Fy";
				ws.Cell(4, 4).Value = "Fz";
				ws.Cell(4, 5).Value = "Mx";
				ws.Cell(4, 6).Value = "My";
				ws.Cell(4, 7).Value = "Mz";
				ws.Cell(4, 8).Value = "Fx";
				ws.Cell(4, 9).Value = "Fy";
				ws.Cell(4, 10).Value = "Fz";
				ws.Cell(4, 11).Value = "Mx";
				ws.Cell(4, 12).Value = "My";
				ws.Cell(4, 13).Value = "Mz";

				var row = 5;

				foreach (var endForce in endForces)
				{
					ws.Cell(row, 1).Value = endForce.ElementIndex;

					var force = endForce.StartForce;

					ws.Cell(row, 2).Value = force.Fx;
					ws.Cell(row, 3).Value = force.Fy;
					ws.Cell(row, 4).Value = force.Fz;
					ws.Cell(row, 5).Value = force.Mx;
					ws.Cell(row, 6).Value = force.My;
					ws.Cell(row, 7).Value = force.Mz;

					force = endForce.EndForce;

					ws.Cell(row, 8).Value = force.Fx;
					ws.Cell(row, 9).Value = force.Fy;
					ws.Cell(row, 10).Value = force.Fz;
					ws.Cell(row, 11).Value = force.Mx;
					ws.Cell(row, 12).Value = force.My;
					ws.Cell(row, 13).Value = force.Mz;

					row++;
				}

				for (var i = 1; i <= 13; i++)
					ws.Column(i).AdjustToContents();
			}
		}

		#endregion

		#region Methods

		private async Task FillNameAsync()
		{
			_title.Text = await _application.GetApplicationTitleAsync();
		}

		private async Task FillDocumentAsync()
		{
			var document = await _application.GetDocumentAsync();
			_documentIdLabel.Text = document.ModelId.ToString();
			_documentPathLabel.Text = document.Path;
			SetDocumentButtonsState( document.ModelId != Guid.Empty );
			_saveDocument.Enabled = !string.IsNullOrWhiteSpace( document.Path );
			_openDocument.Enabled = true;
			_openDocumentForce.Enabled = true;
			_openDocumentStream.Enabled = true;
		}

		private void SetDocumentButtonsState( bool enabled )
		{
			var controls = new Control[]
			{
				_solverModels, _refreshSolverModels, _exportSolverModel,
				_openDocumentStream, _openDocumentForce, _openDocument, _saveDocumentCoreTo, _saveDocumentTo, _saveDocument, _closeDocument, _forceCloseDocument,
			};

			foreach( var control in controls )
				control.Enabled = enabled;

			_exportSolverModel.Enabled &= _solverModels.SelectedItem is IModel;
		}

		private async Task OpenDocument( bool force )
		{
			SetDocumentButtonsState( false );
			using var fd = new OpenFileDialog
			{
				CheckPathExists = true,
				Filter = Filter,
				DefaultExt = DefaultExt,
			};
			if( fd.ShowDialog() == DialogResult.OK )
			{
				await _application.OpenDocumentAsync( fd.FileName, !force );
			}
			await FillDocumentAsync();
		}

		#endregion
	}
}
