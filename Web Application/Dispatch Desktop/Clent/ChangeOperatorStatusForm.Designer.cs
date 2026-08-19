namespace Dispatch
{
   partial class ChangeOperatorStatusForm
   {
      /// <summary>
      /// Required designer variable.
      /// </summary>
      private System.ComponentModel.IContainer components = null;

      /// <summary>
      /// Clean up any resources being used.
      /// </summary>
      /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
      protected override void Dispose ( bool disposing )
      {
         if (disposing && (this.components != null))
         {
            this.components.Dispose();
         }
         base.Dispose( disposing );
      }

      #region Windows Form Designer generated code

      /// <summary>
      /// Required method for Designer support - do not modify
      /// the contents of this method with the code editor.
      /// </summary>
      private void InitializeComponent ()
      {
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.StandByButton = new System.Windows.Forms.Button();
			this.OutButton = new System.Windows.Forms.Button();
			this.InButton = new System.Windows.Forms.Button();
			this.OperatorGrid = new System.Windows.Forms.DataGridView();
			this.OperatorName = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.StatusCode = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.Vehicle = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.OKButton = new System.Windows.Forms.Button();
			this.CancelBtn = new System.Windows.Forms.Button();
			this.groupBox1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.OperatorGrid)).BeginInit();
			this.SuspendLayout();
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.StandByButton);
			this.groupBox1.Controls.Add(this.OutButton);
			this.groupBox1.Controls.Add(this.InButton);
			this.groupBox1.Controls.Add(this.OperatorGrid);
			this.groupBox1.Location = new System.Drawing.Point(13,13);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(439,300);
			this.groupBox1.TabIndex = 0;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Operator Status";
			// 
			// StandByButton
			// 
			this.StandByButton.Location = new System.Drawing.Point(355,146);
			this.StandByButton.Name = "StandByButton";
			this.StandByButton.Size = new System.Drawing.Size(75,23);
			this.StandByButton.TabIndex = 3;
			this.StandByButton.Text = "&StandBy";
			this.StandByButton.UseVisualStyleBackColor = true;
			this.StandByButton.Click += new System.EventHandler(this.StandByButtonClick);
			// 
			// OutButton
			// 
			this.OutButton.Location = new System.Drawing.Point(355,101);
			this.OutButton.Name = "OutButton";
			this.OutButton.Size = new System.Drawing.Size(75,23);
			this.OutButton.TabIndex = 2;
			this.OutButton.Text = "&Out";
			this.OutButton.UseVisualStyleBackColor = true;
			this.OutButton.Click += new System.EventHandler(this.OutButtonClick);
			// 
			// InButton
			// 
			this.InButton.Location = new System.Drawing.Point(355,56);
			this.InButton.Name = "InButton";
			this.InButton.Size = new System.Drawing.Size(75,23);
			this.InButton.TabIndex = 1;
			this.InButton.Text = "&Home";
			this.InButton.UseVisualStyleBackColor = true;
			this.InButton.Click += new System.EventHandler(this.InButtonClick);
			// 
			// OperatorGrid
			// 
			this.OperatorGrid.AllowUserToAddRows = false;
			this.OperatorGrid.AllowUserToDeleteRows = false;
			this.OperatorGrid.AllowUserToResizeRows = false;
			this.OperatorGrid.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
			this.OperatorGrid.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
			dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
			dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
			dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif",8.25F,System.Drawing.FontStyle.Regular,System.Drawing.GraphicsUnit.Point,((byte)(0)));
			dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
			dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
			dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
			dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
			this.OperatorGrid.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
			this.OperatorGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.OperatorGrid.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.OperatorName,
            this.StatusCode,
            this.Vehicle});
			this.OperatorGrid.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
			this.OperatorGrid.Location = new System.Drawing.Point(7,20);
			this.OperatorGrid.Name = "OperatorGrid";
			this.OperatorGrid.ReadOnly = true;
			dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
			dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Control;
			dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif",8.25F,System.Drawing.FontStyle.Regular,System.Drawing.GraphicsUnit.Point,((byte)(0)));
			dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.WindowText;
			dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
			dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
			dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
			this.OperatorGrid.RowHeadersDefaultCellStyle = dataGridViewCellStyle2;
			this.OperatorGrid.RowHeadersVisible = false;
			this.OperatorGrid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
			this.OperatorGrid.ShowEditingIcon = false;
			this.OperatorGrid.Size = new System.Drawing.Size(342,274);
			this.OperatorGrid.TabIndex = 0;
			// 
			// OperatorName
			// 
			this.OperatorName.DataPropertyName = "FullName";
			this.OperatorName.HeaderText = "Operator Name";
			this.OperatorName.Name = "OperatorName";
			this.OperatorName.ReadOnly = true;
			this.OperatorName.Width = 104;
			// 
			// StatusCode
			// 
			this.StatusCode.DataPropertyName = "StatusText";
			this.StatusCode.HeaderText = "Status Code";
			this.StatusCode.Name = "StatusCode";
			this.StatusCode.ReadOnly = true;
			this.StatusCode.Width = 90;
			// 
			// Vehicle
			// 
			this.Vehicle.DataPropertyName = "AssignedEquipmentID";
			this.Vehicle.HeaderText = "Vehicle";
			this.Vehicle.Name = "Vehicle";
			this.Vehicle.ReadOnly = true;
			this.Vehicle.Width = 67;
			// 
			// OKButton
			// 
			this.OKButton.Location = new System.Drawing.Point(141,328);
			this.OKButton.Name = "OKButton";
			this.OKButton.Size = new System.Drawing.Size(75,23);
			this.OKButton.TabIndex = 1;
			this.OKButton.Text = "&OK";
			this.OKButton.UseVisualStyleBackColor = true;
			this.OKButton.Click += new System.EventHandler(this.OkButtonClick);
			// 
			// CancelBtn
			// 
			this.CancelBtn.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.CancelBtn.Location = new System.Drawing.Point(257,328);
			this.CancelBtn.Name = "CancelBtn";
			this.CancelBtn.Size = new System.Drawing.Size(75,23);
			this.CancelBtn.TabIndex = 2;
			this.CancelBtn.Text = "&Cancel";
			this.CancelBtn.UseVisualStyleBackColor = true;
			this.CancelBtn.Click += new System.EventHandler(this.CancelButtonClick);
			// 
			// ChangeOperatorStatusForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F,13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.CancelBtn;
			this.ClientSize = new System.Drawing.Size(453,353);
			this.ControlBox = false;
			this.Controls.Add(this.CancelBtn);
			this.Controls.Add(this.OKButton);
			this.Controls.Add(this.groupBox1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "ChangeOperatorStatusForm";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Change Operator Status";
			this.Load += new System.EventHandler(this.ChangeOperatorStatusFormLoad);
			this.groupBox1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.OperatorGrid)).EndInit();
			this.ResumeLayout(false);

      }

      #endregion

      private System.Windows.Forms.GroupBox groupBox1;
      private System.Windows.Forms.Button OKButton;
      private System.Windows.Forms.Button CancelBtn;
      private System.Windows.Forms.Button StandByButton;
      private System.Windows.Forms.Button OutButton;
      private System.Windows.Forms.Button InButton;
      private System.Windows.Forms.DataGridView OperatorGrid;
      private System.Windows.Forms.DataGridViewTextBoxColumn OperatorName;
      private System.Windows.Forms.DataGridViewTextBoxColumn StatusCode;
      private System.Windows.Forms.DataGridViewTextBoxColumn Vehicle;
   }
}