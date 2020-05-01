namespace TsdApiRemotingSample
{
	partial class ConnectionForm
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose( bool disposing )
		{
			if( disposing && (components != null) )
			{
				components.Dispose();
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			System.Windows.Forms.Label label1;
			System.Windows.Forms.Label label2;
			this._address = new System.Windows.Forms.TextBox();
			this._port = new System.Windows.Forms.TextBox();
			this._cancel = new System.Windows.Forms.Button();
			this._ok = new System.Windows.Forms.Button();
			label1 = new System.Windows.Forms.Label();
			label2 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// label1
			// 
			label1.AutoSize = true;
			label1.Location = new System.Drawing.Point(12, 9);
			label1.Name = "label1";
			label1.Size = new System.Drawing.Size(51, 13);
			label1.TabIndex = 0;
			label1.Text = "Address :";
			// 
			// label2
			// 
			label2.AutoSize = true;
			label2.Location = new System.Drawing.Point(12, 35);
			label2.Name = "label2";
			label2.Size = new System.Drawing.Size(32, 13);
			label2.TabIndex = 1;
			label2.Text = "Port :";
			// 
			// _address
			// 
			this._address.Location = new System.Drawing.Point(68, 6);
			this._address.Name = "_address";
			this._address.Size = new System.Drawing.Size(112, 20);
			this._address.TabIndex = 1;
			this._address.Text = "localhost";
			// 
			// _port
			// 
			this._port.Location = new System.Drawing.Point(68, 32);
			this._port.Name = "_port";
			this._port.Size = new System.Drawing.Size(112, 20);
			this._port.TabIndex = 0;
			this._port.TextChanged += new System.EventHandler(this.OnPortTextChanged);
			// 
			// _cancel
			// 
			this._cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this._cancel.Location = new System.Drawing.Point(15, 58);
			this._cancel.Name = "_cancel";
			this._cancel.Size = new System.Drawing.Size(75, 23);
			this._cancel.TabIndex = 3;
			this._cancel.Text = "Cancel";
			this._cancel.UseVisualStyleBackColor = true;
			// 
			// _ok
			// 
			this._ok.DialogResult = System.Windows.Forms.DialogResult.OK;
			this._ok.Location = new System.Drawing.Point(105, 58);
			this._ok.Name = "_ok";
			this._ok.Size = new System.Drawing.Size(75, 23);
			this._ok.TabIndex = 2;
			this._ok.Text = "OK";
			this._ok.UseVisualStyleBackColor = true;
			// 
			// ConnectionForm
			// 
			this.AcceptButton = this._ok;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this._cancel;
			this.ClientSize = new System.Drawing.Size(194, 93);
			this.ControlBox = false;
			this.Controls.Add(this._ok);
			this.Controls.Add(this._cancel);
			this.Controls.Add(this._port);
			this.Controls.Add(this._address);
			this.Controls.Add(label2);
			this.Controls.Add(label1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Name = "ConnectionForm";
			this.ShowInTaskbar = false;
			this.Text = "Connect to...";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TextBox _address;
		private System.Windows.Forms.TextBox _port;
		private System.Windows.Forms.Button _cancel;
		private System.Windows.Forms.Button _ok;
	}
}