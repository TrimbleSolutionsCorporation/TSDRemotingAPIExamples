namespace TsdApiRemotingSample
{
	partial class MainForm
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
				Clear();
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
			this._refreshButton = new System.Windows.Forms.Button();
			this._statusLabel = new System.Windows.Forms.Label();
			this._table = new System.Windows.Forms.TableLayoutPanel();
			this._directConnect = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// _refreshButton
			// 
			this._refreshButton.Location = new System.Drawing.Point(12, 12);
			this._refreshButton.Name = "_refreshButton";
			this._refreshButton.Size = new System.Drawing.Size(75, 23);
			this._refreshButton.TabIndex = 1;
			this._refreshButton.Text = "Refresh";
			this._refreshButton.UseVisualStyleBackColor = true;
			this._refreshButton.Click += new System.EventHandler(this.OnRefreshButtonClick);
			// 
			// _statusLabel
			// 
			this._statusLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._statusLabel.AutoSize = true;
			this._statusLabel.Location = new System.Drawing.Point(218, 17);
			this._statusLabel.Name = "_statusLabel";
			this._statusLabel.Size = new System.Drawing.Size(35, 13);
			this._statusLabel.TabIndex = 2;
			this._statusLabel.Text = "label1";
			// 
			// _table
			// 
			this._table.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._table.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.InsetDouble;
			this._table.ColumnCount = 2;
			this._table.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this._table.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this._table.Location = new System.Drawing.Point(12, 41);
			this._table.Name = "_table";
			this._table.RowCount = 2;
			this._table.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this._table.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this._table.Size = new System.Drawing.Size(733, 427);
			this._table.TabIndex = 3;
			// 
			// _directConnect
			// 
			this._directConnect.Location = new System.Drawing.Point(93, 12);
			this._directConnect.Name = "_directConnect";
			this._directConnect.Size = new System.Drawing.Size(119, 23);
			this._directConnect.TabIndex = 4;
			this._directConnect.Text = "Direct connection...";
			this._directConnect.UseVisualStyleBackColor = true;
			this._directConnect.Click += new System.EventHandler(this.OnDirectConnectClick);
			// 
			// MainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(757, 480);
			this.Controls.Add(this._directConnect);
			this.Controls.Add(this._table);
			this.Controls.Add(this._statusLabel);
			this.Controls.Add(this._refreshButton);
			this.Name = "MainForm";
			this.Text = "TSD API 2 Sample Application";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion
		private System.Windows.Forms.Button _refreshButton;
		private System.Windows.Forms.Label _statusLabel;
		private System.Windows.Forms.TableLayoutPanel _table;
		private System.Windows.Forms.Button _directConnect;
	}
}

