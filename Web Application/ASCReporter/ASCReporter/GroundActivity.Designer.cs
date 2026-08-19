namespace ASCReporter
{
	partial class GroundActivity
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle8 = new System.Windows.Forms.DataGridViewCellStyle();
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle9 = new System.Windows.Forms.DataGridViewCellStyle();
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle7 = new System.Windows.Forms.DataGridViewCellStyle();
			this.labelExcludedDays = new System.Windows.Forms.Label();
			this.manualEntry = new System.Windows.Forms.CheckBox();
			this.quantityMax = new System.Windows.Forms.MaskedTextBox();
			this.labelMaxOneDay = new System.Windows.Forms.Label();
			this.excludedDaysGrid = new System.Windows.Forms.DataGridView();
			this.excludedDaysGridDate = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.excludedDaysGridRestore = new System.Windows.Forms.DataGridViewButtonColumn();
			this.labelPeakDays = new System.Windows.Forms.Label();
			this.peakDaysGrid = new System.Windows.Forms.DataGridView();
			this.peakDaysGridDate = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.peakDaysGridQuantity = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.peakDaysGridExclude = new System.Windows.Forms.DataGridViewButtonColumn();
			this.buttonAccept = new System.Windows.Forms.Button();
			this.buttonCancel = new System.Windows.Forms.Button();
			this.pleaseWait = new System.Windows.Forms.Label();
			((System.ComponentModel.ISupportInitialize)(this.excludedDaysGrid)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.peakDaysGrid)).BeginInit();
			this.SuspendLayout();
			// 
			// labelExcludedDays
			// 
			this.labelExcludedDays.AutoSize = true;
			this.labelExcludedDays.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.labelExcludedDays.Location = new System.Drawing.Point(95, 264);
			this.labelExcludedDays.Name = "labelExcludedDays";
			this.labelExcludedDays.Size = new System.Drawing.Size(78, 13);
			this.labelExcludedDays.TabIndex = 46;
			this.labelExcludedDays.Text = "Excluded Days";
			this.labelExcludedDays.Visible = false;
			// 
			// manualEntry
			// 
			this.manualEntry.AutoSize = true;
			this.manualEntry.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.manualEntry.Location = new System.Drawing.Point(282, 31);
			this.manualEntry.Name = "manualEntry";
			this.manualEntry.Size = new System.Drawing.Size(88, 17);
			this.manualEntry.TabIndex = 1;
			this.manualEntry.Text = "Manual Entry";
			this.manualEntry.UseVisualStyleBackColor = true;
			this.manualEntry.Visible = false;
			this.manualEntry.CheckedChanged += new System.EventHandler(this.manualEntry_CheckedChanged);
			// 
			// quantityMax
			// 
			this.quantityMax.HidePromptOnLeave = true;
			this.quantityMax.Location = new System.Drawing.Point(230, 29);
			this.quantityMax.Mask = "00000";
			this.quantityMax.Name = "quantityMax";
			this.quantityMax.ReadOnly = true;
			this.quantityMax.Size = new System.Drawing.Size(45, 20);
			this.quantityMax.TabIndex = 0;
			this.quantityMax.TabStop = false;
			this.quantityMax.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.quantityMax.TextMaskFormat = System.Windows.Forms.MaskFormat.ExcludePromptAndLiterals;
			this.quantityMax.Visible = false;
			this.quantityMax.TextChanged += new System.EventHandler(this.quantityMax_TextChanged);
			// 
			// labelMaxOneDay
			// 
			this.labelMaxOneDay.AutoSize = true;
			this.labelMaxOneDay.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.labelMaxOneDay.Location = new System.Drawing.Point(34, 22);
			this.labelMaxOneDay.Name = "labelMaxOneDay";
			this.labelMaxOneDay.Size = new System.Drawing.Size(167, 26);
			this.labelMaxOneDay.TabIndex = 43;
			this.labelMaxOneDay.Text = "Max 1 Day of Heating/Diesel Fuel\r\nGallon Amount";
			this.labelMaxOneDay.Visible = false;
			// 
			// excludedDaysGrid
			// 
			this.excludedDaysGrid.AllowUserToAddRows = false;
			this.excludedDaysGrid.AllowUserToDeleteRows = false;
			this.excludedDaysGrid.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this.excludedDaysGrid.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
			dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
			dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
			dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
			dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
			dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
			dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
			this.excludedDaysGrid.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
			this.excludedDaysGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.excludedDaysGrid.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.excludedDaysGridDate,
            this.excludedDaysGridRestore});
			dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
			dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Window;
			dataGridViewCellStyle3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.ControlText;
			dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
			dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
			dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
			this.excludedDaysGrid.DefaultCellStyle = dataGridViewCellStyle3;
			this.excludedDaysGrid.Location = new System.Drawing.Point(98, 280);
			this.excludedDaysGrid.Name = "excludedDaysGrid";
			dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
			dataGridViewCellStyle4.BackColor = System.Drawing.SystemColors.Control;
			dataGridViewCellStyle4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			dataGridViewCellStyle4.ForeColor = System.Drawing.SystemColors.WindowText;
			dataGridViewCellStyle4.SelectionBackColor = System.Drawing.SystemColors.Highlight;
			dataGridViewCellStyle4.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
			dataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
			this.excludedDaysGrid.RowHeadersDefaultCellStyle = dataGridViewCellStyle4;
			this.excludedDaysGrid.RowHeadersVisible = false;
			this.excludedDaysGrid.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.excludedDaysGrid.Size = new System.Drawing.Size(208, 103);
			this.excludedDaysGrid.StandardTab = true;
			this.excludedDaysGrid.TabIndex = 3;
			this.excludedDaysGrid.Visible = false;
			this.excludedDaysGrid.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.excludedDaysGrid_CellContentClick);
			// 
			// excludedDaysGridDate
			// 
			dataGridViewCellStyle2.Format = "d";
			this.excludedDaysGridDate.DefaultCellStyle = dataGridViewCellStyle2;
			this.excludedDaysGridDate.HeaderText = "Excluded Date";
			this.excludedDaysGridDate.Name = "excludedDaysGridDate";
			this.excludedDaysGridDate.ReadOnly = true;
			this.excludedDaysGridDate.Resizable = System.Windows.Forms.DataGridViewTriState.False;
			// 
			// excludedDaysGridRestore
			// 
			this.excludedDaysGridRestore.HeaderText = "";
			this.excludedDaysGridRestore.Name = "excludedDaysGridRestore";
			this.excludedDaysGridRestore.Resizable = System.Windows.Forms.DataGridViewTriState.False;
			this.excludedDaysGridRestore.Text = "Restore";
			this.excludedDaysGridRestore.UseColumnTextForButtonValue = true;
			// 
			// labelPeakDays
			// 
			this.labelPeakDays.AutoSize = true;
			this.labelPeakDays.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.labelPeakDays.Location = new System.Drawing.Point(38, 72);
			this.labelPeakDays.Name = "labelPeakDays";
			this.labelPeakDays.Size = new System.Drawing.Size(91, 13);
			this.labelPeakDays.TabIndex = 41;
			this.labelPeakDays.Text = "Peak Day Activity";
			this.labelPeakDays.Visible = false;
			// 
			// peakDaysGrid
			// 
			this.peakDaysGrid.AllowUserToAddRows = false;
			this.peakDaysGrid.AllowUserToDeleteRows = false;
			this.peakDaysGrid.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
			dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
			dataGridViewCellStyle5.BackColor = System.Drawing.SystemColors.Control;
			dataGridViewCellStyle5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			dataGridViewCellStyle5.ForeColor = System.Drawing.SystemColors.WindowText;
			dataGridViewCellStyle5.SelectionBackColor = System.Drawing.SystemColors.Highlight;
			dataGridViewCellStyle5.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
			dataGridViewCellStyle5.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
			this.peakDaysGrid.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle5;
			this.peakDaysGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.peakDaysGrid.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.peakDaysGridDate,
            this.peakDaysGridQuantity,
            this.peakDaysGridExclude});
			dataGridViewCellStyle8.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
			dataGridViewCellStyle8.BackColor = System.Drawing.SystemColors.Window;
			dataGridViewCellStyle8.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			dataGridViewCellStyle8.ForeColor = System.Drawing.SystemColors.ControlText;
			dataGridViewCellStyle8.SelectionBackColor = System.Drawing.SystemColors.Highlight;
			dataGridViewCellStyle8.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
			dataGridViewCellStyle8.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
			this.peakDaysGrid.DefaultCellStyle = dataGridViewCellStyle8;
			this.peakDaysGrid.Location = new System.Drawing.Point(37, 95);
			this.peakDaysGrid.Name = "peakDaysGrid";
			dataGridViewCellStyle9.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
			dataGridViewCellStyle9.BackColor = System.Drawing.SystemColors.Control;
			dataGridViewCellStyle9.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			dataGridViewCellStyle9.ForeColor = System.Drawing.SystemColors.WindowText;
			dataGridViewCellStyle9.SelectionBackColor = System.Drawing.SystemColors.Highlight;
			dataGridViewCellStyle9.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
			dataGridViewCellStyle9.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
			this.peakDaysGrid.RowHeadersDefaultCellStyle = dataGridViewCellStyle9;
			this.peakDaysGrid.RowHeadersVisible = false;
			this.peakDaysGrid.ScrollBars = System.Windows.Forms.ScrollBars.None;
			this.peakDaysGrid.Size = new System.Drawing.Size(333, 153);
			this.peakDaysGrid.StandardTab = true;
			this.peakDaysGrid.TabIndex = 2;
			this.peakDaysGrid.Visible = false;
			this.peakDaysGrid.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.peakDaysGrid_CellContentClick);
			// 
			// peakDaysGridDate
			// 
			dataGridViewCellStyle6.Format = "d";
			this.peakDaysGridDate.DefaultCellStyle = dataGridViewCellStyle6;
			this.peakDaysGridDate.HeaderText = "Date";
			this.peakDaysGridDate.Name = "peakDaysGridDate";
			this.peakDaysGridDate.ReadOnly = true;
			this.peakDaysGridDate.Resizable = System.Windows.Forms.DataGridViewTriState.False;
			// 
			// peakDaysGridQuantity
			// 
			dataGridViewCellStyle7.Format = "N0";
			dataGridViewCellStyle7.NullValue = null;
			this.peakDaysGridQuantity.DefaultCellStyle = dataGridViewCellStyle7;
			this.peakDaysGridQuantity.HeaderText = "Total Quantity";
			this.peakDaysGridQuantity.Name = "peakDaysGridQuantity";
			this.peakDaysGridQuantity.ReadOnly = true;
			this.peakDaysGridQuantity.Resizable = System.Windows.Forms.DataGridViewTriState.False;
			// 
			// peakDaysGridExclude
			// 
			this.peakDaysGridExclude.HeaderText = "";
			this.peakDaysGridExclude.Name = "peakDaysGridExclude";
			this.peakDaysGridExclude.Resizable = System.Windows.Forms.DataGridViewTriState.False;
			this.peakDaysGridExclude.Text = "Exclude";
			this.peakDaysGridExclude.UseColumnTextForButtonValue = true;
			// 
			// buttonAccept
			// 
			this.buttonAccept.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.buttonAccept.Enabled = false;
			this.buttonAccept.Location = new System.Drawing.Point(110, 401);
			this.buttonAccept.Name = "buttonAccept";
			this.buttonAccept.Size = new System.Drawing.Size(75, 23);
			this.buttonAccept.TabIndex = 4;
			this.buttonAccept.Text = "OK";
			this.buttonAccept.UseVisualStyleBackColor = true;
			// 
			// buttonCancel
			// 
			this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonCancel.Enabled = false;
			this.buttonCancel.Location = new System.Drawing.Point(230, 400);
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.Size = new System.Drawing.Size(75, 23);
			this.buttonCancel.TabIndex = 5;
			this.buttonCancel.Text = "Cancel";
			this.buttonCancel.UseVisualStyleBackColor = true;
			// 
			// pleaseWait
			// 
			this.pleaseWait.AutoSize = true;
			this.pleaseWait.Location = new System.Drawing.Point(165, 204);
			this.pleaseWait.Name = "pleaseWait";
			this.pleaseWait.Size = new System.Drawing.Size(82, 26);
			this.pleaseWait.TabIndex = 47;
			this.pleaseWait.Text = "Gathering Data.\r\nPlease wait.";
			this.pleaseWait.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// GroundActivity
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(412, 435);
			this.Controls.Add(this.pleaseWait);
			this.Controls.Add(this.buttonCancel);
			this.Controls.Add(this.buttonAccept);
			this.Controls.Add(this.labelExcludedDays);
			this.Controls.Add(this.manualEntry);
			this.Controls.Add(this.quantityMax);
			this.Controls.Add(this.labelMaxOneDay);
			this.Controls.Add(this.excludedDaysGrid);
			this.Controls.Add(this.labelPeakDays);
			this.Controls.Add(this.peakDaysGrid);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Name = "GroundActivity";
			this.ShowInTaskbar = false;
			this.Text = "Ground Fuel Activity";
			this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.GroundActivity_FormClosed);
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.GroundActivity_FormClosing);
			this.Load += new System.EventHandler(this.GroundActivity_Load);
			((System.ComponentModel.ISupportInitialize)(this.excludedDaysGrid)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.peakDaysGrid)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label labelExcludedDays;
		private System.Windows.Forms.CheckBox manualEntry;
		private System.Windows.Forms.MaskedTextBox quantityMax;
		private System.Windows.Forms.Label labelMaxOneDay;
		private System.Windows.Forms.DataGridView excludedDaysGrid;
		private System.Windows.Forms.Label labelPeakDays;
		private System.Windows.Forms.DataGridView peakDaysGrid;
		private System.Windows.Forms.Button buttonAccept;
		private System.Windows.Forms.Button buttonCancel;
		private System.Windows.Forms.DataGridViewTextBoxColumn excludedDaysGridDate;
		private System.Windows.Forms.DataGridViewButtonColumn excludedDaysGridRestore;
		private System.Windows.Forms.DataGridViewTextBoxColumn peakDaysGridDate;
		private System.Windows.Forms.DataGridViewTextBoxColumn peakDaysGridQuantity;
		private System.Windows.Forms.DataGridViewButtonColumn peakDaysGridExclude;
		private System.Windows.Forms.Label pleaseWait;
	}
}