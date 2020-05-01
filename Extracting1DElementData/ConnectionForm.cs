using System;
using System.Windows.Forms;

namespace TsdApiRemotingSample
{
	public partial class ConnectionForm : Form
	{
		public ConnectionForm()
		{
			InitializeComponent();
		}

		private void OnPortTextChanged( object sender, EventArgs e )
		{
			_ok.Enabled = _port.Text.Length > 0 && int.TryParse( _port.Text, out _ );
		}

		public string Host => _address.Text;

		public int Port => int.Parse( _port.Text );
	}
}
