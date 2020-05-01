namespace TsdApiRemotingSample
{
	partial class ApplicationPage
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

			if( disposing )
			{
				_application.Disconnected -= OnApplicationDisconnected;
				_application.ModelClosed -= OnApplicationModelChanged;
				_application.ModelOpened -= OnApplicationModelChanged;
				_application.Dispose();
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
            System.Windows.Forms.GroupBox groupBox1;
            System.Windows.Forms.Label label4;
            System.Windows.Forms.Label label3;
            System.Windows.Forms.Label label5;
            this._openDocumentStream = new System.Windows.Forms.Button();
            this._openDocumentForce = new System.Windows.Forms.Button();
            this._openDocument = new System.Windows.Forms.Button();
            this._saveDocumentCoreTo = new System.Windows.Forms.Button();
            this._saveDocumentTo = new System.Windows.Forms.Button();
            this._saveDocument = new System.Windows.Forms.Button();
            this._forceCloseDocument = new System.Windows.Forms.Button();
            this._closeDocument = new System.Windows.Forms.Button();
            this._documentIdLabel = new System.Windows.Forms.Label();
            this._documentPathLabel = new System.Windows.Forms.Label();
            this._versionLabel = new System.Windows.Forms.Label();
            this._title = new System.Windows.Forms.TextBox();
            this._connectionIndicator = new System.Windows.Forms.Label();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this._exportSolverModel = new System.Windows.Forms.Button();
            this._refreshSolverModels = new System.Windows.Forms.Button();
            this._solverModels = new System.Windows.Forms.ComboBox();
            label1 = new System.Windows.Forms.Label();
            label2 = new System.Windows.Forms.Label();
            groupBox1 = new System.Windows.Forms.GroupBox();
            label4 = new System.Windows.Forms.Label();
            label3 = new System.Windows.Forms.Label();
            label5 = new System.Windows.Forms.Label();
            groupBox1.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(12, 31);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(30, 13);
            label1.TabIndex = 0;
            label1.Text = "Title:";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new System.Drawing.Point(12, 9);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(45, 13);
            label2.TabIndex = 1;
            label2.Text = "Version:";
            // 
            // groupBox1
            // 
            groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            groupBox1.Controls.Add(this._openDocumentStream);
            groupBox1.Controls.Add(this._openDocumentForce);
            groupBox1.Controls.Add(this._openDocument);
            groupBox1.Controls.Add(this._saveDocumentCoreTo);
            groupBox1.Controls.Add(this._saveDocumentTo);
            groupBox1.Controls.Add(this._saveDocument);
            groupBox1.Controls.Add(this._forceCloseDocument);
            groupBox1.Controls.Add(this._closeDocument);
            groupBox1.Controls.Add(this._documentIdLabel);
            groupBox1.Controls.Add(this._documentPathLabel);
            groupBox1.Controls.Add(label4);
            groupBox1.Controls.Add(label3);
            groupBox1.Location = new System.Drawing.Point(15, 98);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new System.Drawing.Size(773, 81);
            groupBox1.TabIndex = 6;
            groupBox1.TabStop = false;
            groupBox1.Text = "Document";
            // 
            // _openDocumentStream
            // 
            this._openDocumentStream.Location = new System.Drawing.Point(622, 45);
            this._openDocumentStream.Name = "_openDocumentStream";
            this._openDocumentStream.Size = new System.Drawing.Size(90, 23);
            this._openDocumentStream.TabIndex = 13;
            this._openDocumentStream.Text = "Open... (stream)";
            this._openDocumentStream.UseVisualStyleBackColor = true;
            this._openDocumentStream.Click += new System.EventHandler(this.OnOpenDocumentStreamClick);
            // 
            // _openDocumentForce
            // 
            this._openDocumentForce.Location = new System.Drawing.Point(526, 45);
            this._openDocumentForce.Name = "_openDocumentForce";
            this._openDocumentForce.Size = new System.Drawing.Size(90, 23);
            this._openDocumentForce.TabIndex = 12;
            this._openDocumentForce.Text = "Open... (force)";
            this._openDocumentForce.UseVisualStyleBackColor = true;
            this._openDocumentForce.Click += new System.EventHandler(this.OnOpenDocumentForceClick);
            // 
            // _openDocument
            // 
            this._openDocument.Location = new System.Drawing.Point(445, 45);
            this._openDocument.Name = "_openDocument";
            this._openDocument.Size = new System.Drawing.Size(75, 23);
            this._openDocument.TabIndex = 11;
            this._openDocument.Text = "Open...";
            this._openDocument.UseVisualStyleBackColor = true;
            this._openDocument.Click += new System.EventHandler(this.OnOpenDocumentClick);
            // 
            // _saveDocumentCoreTo
            // 
            this._saveDocumentCoreTo.Location = new System.Drawing.Point(349, 45);
            this._saveDocumentCoreTo.Name = "_saveDocumentCoreTo";
            this._saveDocumentCoreTo.Size = new System.Drawing.Size(90, 23);
            this._saveDocumentCoreTo.TabIndex = 10;
            this._saveDocumentCoreTo.Text = "Save Core To...";
            this._saveDocumentCoreTo.UseVisualStyleBackColor = true;
            this._saveDocumentCoreTo.Click += new System.EventHandler(this.SaveDocumentCoreToClick);
            // 
            // _saveDocumentTo
            // 
            this._saveDocumentTo.Location = new System.Drawing.Point(268, 45);
            this._saveDocumentTo.Name = "_saveDocumentTo";
            this._saveDocumentTo.Size = new System.Drawing.Size(75, 23);
            this._saveDocumentTo.TabIndex = 9;
            this._saveDocumentTo.Text = "Save To...";
            this._saveDocumentTo.UseVisualStyleBackColor = true;
            this._saveDocumentTo.Click += new System.EventHandler(this.OnSaveDocumentToClick);
            // 
            // _saveDocument
            // 
            this._saveDocument.Location = new System.Drawing.Point(187, 45);
            this._saveDocument.Name = "_saveDocument";
            this._saveDocument.Size = new System.Drawing.Size(75, 23);
            this._saveDocument.TabIndex = 8;
            this._saveDocument.Text = "Save";
            this._saveDocument.UseVisualStyleBackColor = true;
            this._saveDocument.Click += new System.EventHandler(this.OnSaveDocumentClick);
            // 
            // _forceCloseDocument
            // 
            this._forceCloseDocument.Location = new System.Drawing.Point(89, 45);
            this._forceCloseDocument.Name = "_forceCloseDocument";
            this._forceCloseDocument.Size = new System.Drawing.Size(92, 23);
            this._forceCloseDocument.TabIndex = 7;
            this._forceCloseDocument.Text = "Close (Force)";
            this._forceCloseDocument.UseVisualStyleBackColor = true;
            this._forceCloseDocument.Click += new System.EventHandler(this.OnForceCloseDocumentClick);
            // 
            // _closeDocument
            // 
            this._closeDocument.Location = new System.Drawing.Point(8, 45);
            this._closeDocument.Name = "_closeDocument";
            this._closeDocument.Size = new System.Drawing.Size(75, 23);
            this._closeDocument.TabIndex = 4;
            this._closeDocument.Text = "Close";
            this._closeDocument.UseVisualStyleBackColor = true;
            this._closeDocument.Click += new System.EventHandler(this.OnCloseDocumentClick);
            // 
            // _documentIdLabel
            // 
            this._documentIdLabel.AutoSize = true;
            this._documentIdLabel.Location = new System.Drawing.Point(65, 29);
            this._documentIdLabel.Name = "_documentIdLabel";
            this._documentIdLabel.Size = new System.Drawing.Size(35, 13);
            this._documentIdLabel.TabIndex = 3;
            this._documentIdLabel.Text = "label5";
            // 
            // _documentPathLabel
            // 
            this._documentPathLabel.AutoSize = true;
            this._documentPathLabel.Location = new System.Drawing.Point(65, 16);
            this._documentPathLabel.Name = "_documentPathLabel";
            this._documentPathLabel.Size = new System.Drawing.Size(35, 13);
            this._documentPathLabel.TabIndex = 2;
            this._documentPathLabel.Text = "label5";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new System.Drawing.Point(6, 29);
            label4.Name = "label4";
            label4.Size = new System.Drawing.Size(53, 13);
            label4.TabIndex = 1;
            label4.Text = "Model ID:";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new System.Drawing.Point(6, 16);
            label3.Name = "label3";
            label3.Size = new System.Drawing.Size(32, 13);
            label3.TabIndex = 0;
            label3.Text = "Path:";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new System.Drawing.Point(6, 22);
            label5.Name = "label5";
            label5.Size = new System.Drawing.Size(71, 13);
            label5.TabIndex = 1;
            label5.Text = "Analysis type:";
            // 
            // _versionLabel
            // 
            this._versionLabel.AutoSize = true;
            this._versionLabel.Location = new System.Drawing.Point(63, 9);
            this._versionLabel.Name = "_versionLabel";
            this._versionLabel.Size = new System.Drawing.Size(35, 13);
            this._versionLabel.TabIndex = 2;
            this._versionLabel.Text = "label3";
            // 
            // _title
            // 
            this._title.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._title.Location = new System.Drawing.Point(48, 28);
            this._title.Multiline = true;
            this._title.Name = "_title";
            this._title.ReadOnly = true;
            this._title.Size = new System.Drawing.Size(740, 64);
            this._title.TabIndex = 4;
            // 
            // _connectionIndicator
            // 
            this._connectionIndicator.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._connectionIndicator.Location = new System.Drawing.Point(672, 9);
            this._connectionIndicator.Name = "_connectionIndicator";
            this._connectionIndicator.Size = new System.Drawing.Size(116, 13);
            this._connectionIndicator.TabIndex = 5;
            this._connectionIndicator.Text = "CONNECTED";
            this._connectionIndicator.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Controls.Add(this.groupBox2, 0, 0);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(15, 185);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(773, 253);
            this.tableLayoutPanel1.TabIndex = 7;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this._exportSolverModel);
            this.groupBox2.Controls.Add(this._refreshSolverModels);
            this.groupBox2.Controls.Add(label5);
            this.groupBox2.Controls.Add(this._solverModels);
            this.groupBox2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox2.Location = new System.Drawing.Point(3, 3);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(380, 247);
            this.groupBox2.TabIndex = 0;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Solver";
            // 
            // _exportSolverModel
            // 
            this._exportSolverModel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._exportSolverModel.Enabled = false;
            this._exportSolverModel.Location = new System.Drawing.Point(299, 46);
            this._exportSolverModel.Name = "_exportSolverModel";
            this._exportSolverModel.Size = new System.Drawing.Size(75, 23);
            this._exportSolverModel.TabIndex = 13;
            this._exportSolverModel.Text = "Export...";
            this._exportSolverModel.UseVisualStyleBackColor = true;
            this._exportSolverModel.Click += new System.EventHandler(this.OnExportSolverModelClick);
            // 
            // _refreshSolverModels
            // 
            this._refreshSolverModels.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._refreshSolverModels.Location = new System.Drawing.Point(299, 17);
            this._refreshSolverModels.Name = "_refreshSolverModels";
            this._refreshSolverModels.Size = new System.Drawing.Size(75, 23);
            this._refreshSolverModels.TabIndex = 2;
            this._refreshSolverModels.Text = "Refresh";
            this._refreshSolverModels.UseVisualStyleBackColor = true;
            this._refreshSolverModels.Click += new System.EventHandler(this.OnRefreshSolverModelsClick);
            // 
            // _solverModels
            // 
            this._solverModels.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._solverModels.FormattingEnabled = true;
            this._solverModels.Location = new System.Drawing.Point(83, 19);
            this._solverModels.Name = "_solverModels";
            this._solverModels.Size = new System.Drawing.Size(210, 21);
            this._solverModels.TabIndex = 0;
            this._solverModels.SelectedIndexChanged += new System.EventHandler(this.OnSolverModelsSelectedIndexChanged);
            // 
            // ApplicationPage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Controls.Add(groupBox1);
            this.Controls.Add(this._connectionIndicator);
            this.Controls.Add(this._title);
            this.Controls.Add(this._versionLabel);
            this.Controls.Add(label2);
            this.Controls.Add(label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "ApplicationPage";
            this.ShowInTaskbar = false;
            this.Text = "ApplicationPage";
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label _versionLabel;
		private System.Windows.Forms.TextBox _title;
		private System.Windows.Forms.Label _connectionIndicator;
		private System.Windows.Forms.Label _documentIdLabel;
		private System.Windows.Forms.Label _documentPathLabel;
		private System.Windows.Forms.Button _closeDocument;
		private System.Windows.Forms.Button _forceCloseDocument;
		private System.Windows.Forms.Button _saveDocument;
		private System.Windows.Forms.Button _saveDocumentTo;
		private System.Windows.Forms.Button _saveDocumentCoreTo;
		private System.Windows.Forms.Button _openDocument;
		private System.Windows.Forms.Button _openDocumentForce;
		private System.Windows.Forms.Button _openDocumentStream;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.ComboBox _solverModels;
		private System.Windows.Forms.Button _refreshSolverModels;
        private System.Windows.Forms.Button _exportSolverModel;
    }
}
