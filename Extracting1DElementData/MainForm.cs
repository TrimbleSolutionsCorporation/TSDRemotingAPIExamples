using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using TSD.API.Remoting;

namespace Extracting1DElementData
{
	public partial class MainForm : Form
	{
		#region Fields

		private List<ApplicationPage> _pages = new List<ApplicationPage>();

		#endregion

		#region Constructors

		public MainForm()
		{
			InitializeComponent();
			UpdateStatus();
		}

		#endregion

		#region Overrides

		protected override void OnLoad( EventArgs e )
		{
			Clear();
			base.OnLoad( e );
		}

		#endregion

		#region Methods

		private void Clear()
		{
			foreach( var page in _pages.ToArray() )
			{
				RemovePage( page );
			}

			_pages.Clear();
			_table.RowCount = 0;
			_table.ColumnCount = 0;
		}

		private void UpdateStatus()
		{
			_statusLabel.Text = $"Found {_pages.Count} running TSD applications";
		}

		/// <summary>
		/// Enumerates running TSD instances and for each one adds application page
		/// </summary>
		/// <returns></returns>
		private async Task FindApplicationsAsync()
		{
			foreach( var page in (await ApplicationFactory.GetRunningApplicationsAsync()).Select( app => new ApplicationPage( app ) ) )
				AddPage( page );

			UpdateStatus();
		}

		private void AddPage( ApplicationPage page )
		{
			_pages.Add( page );
			FillTable();
			page.Show();
			page.Disconnected += OnPageDisconnected;
		}

		private void RemovePage( ApplicationPage page )
		{
			page.Disconnected -= OnPageDisconnected;
			page.Dispose();
			_pages.Remove( page );
			FillTable();
		}

		private void FillTable()
		{
			_table.SuspendLayout();
			_table.ResumeLayout();

			int toPlace = Math.Min( 8, _pages.Count );
			_table.RowCount = toPlace > 2 ? 2 : 1;
			_table.ColumnCount = (int) Math.Ceiling( (double) toPlace / _table.RowCount );

			_table.Controls.Clear();

			for( int i = 0; i < toPlace; i++ )
				_table.Controls.Add( _pages[i] );

			_table.PerformLayout();
		}

		#endregion

		#region Event Handlers

		private async void OnRefreshButtonClick( object sender, EventArgs e )
		{
			_refreshButton.Enabled = false;

			Clear();
			await FindApplicationsAsync();

			_refreshButton.Enabled = true;
		}

		private async void OnDirectConnectClick( object sender, EventArgs e )
		{
			_directConnect.Enabled = false;

			using( var form = new ConnectionForm() )
			{
				if( form.ShowDialog() == DialogResult.OK )
				{
					var app = await ApplicationFactory.ConnectToRunningApplicationAsync( form.Host, form.Port );

					if( app != null )
					{
						AddPage( new ApplicationPage( app ) );
						UpdateStatus();
					}
				}
			}

			_directConnect.Enabled = true;
		}

		/// <summary>
		/// Removes disconnected page
		/// </summary>
		private void OnPageDisconnected( object sender, EventArgs e )
		{
			RemovePage( (ApplicationPage) sender );
			UpdateStatus();
		}

		#endregion
	}
}
