namespace ASCReporter
{
	partial class HSVWorksheet
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(HSVWorksheet));
			this.buttonOK = new System.Windows.Forms.Button();
			this.buttonCancel = new System.Windows.Forms.Button();
			this.hsvBOIA = new System.Windows.Forms.MaskedTextBox();
			this.hsvBOIB = new System.Windows.Forms.MaskedTextBox();
			this.hsvBOIC = new System.Windows.Forms.MaskedTextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// buttonOK
			// 
			this.buttonOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.buttonOK.Location = new System.Drawing.Point(108, 274);
			this.buttonOK.Name = "buttonOK";
			this.buttonOK.Size = new System.Drawing.Size(75, 23);
			this.buttonOK.TabIndex = 0;
			this.buttonOK.Text = "OK";
			this.buttonOK.UseVisualStyleBackColor = true;
			// 
			// buttonCancel
			// 
			this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonCancel.Location = new System.Drawing.Point(189, 274);
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.Size = new System.Drawing.Size(75, 23);
			this.buttonCancel.TabIndex = 1;
			this.buttonCancel.Text = "Cancel";
			this.buttonCancel.UseVisualStyleBackColor = true;
			// 
			// hsvBOIA
			// 
			this.hsvBOIA.HidePromptOnLeave = true;
			this.hsvBOIA.Location = new System.Drawing.Point(326, 93);
			this.hsvBOIA.Mask = "00";
			this.hsvBOIA.Name = "hsvBOIA";
			this.hsvBOIA.Size = new System.Drawing.Size(23, 20);
			this.hsvBOIA.TabIndex = 2;
			this.hsvBOIA.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.hsvBOIA.TextMaskFormat = System.Windows.Forms.MaskFormat.ExcludePromptAndLiterals;
			// 
			// hsvBOIB
			// 
			this.hsvBOIB.HidePromptOnLeave = true;
			this.hsvBOIB.Location = new System.Drawing.Point(326, 179);
			this.hsvBOIB.Mask = "00";
			this.hsvBOIB.Name = "hsvBOIB";
			this.hsvBOIB.Size = new System.Drawing.Size(23, 20);
			this.hsvBOIB.TabIndex = 3;
			this.hsvBOIB.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.hsvBOIB.TextMaskFormat = System.Windows.Forms.MaskFormat.ExcludePromptAndLiterals;
			// 
			// hsvBOIC
			// 
			this.hsvBOIC.HidePromptOnLeave = true;
			this.hsvBOIC.Location = new System.Drawing.Point(326, 229);
			this.hsvBOIC.Mask = "00";
			this.hsvBOIC.Name = "hsvBOIC";
			this.hsvBOIC.Size = new System.Drawing.Size(23, 20);
			this.hsvBOIC.TabIndex = 4;
			this.hsvBOIC.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.hsvBOIC.TextMaskFormat = System.Windows.Forms.MaskFormat.ExcludePromptAndLiterals;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(13, 35);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(279, 78);
			this.label1.TabIndex = 5;
			this.label1.Text = resources.GetString("label1.Text");
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(12, 134);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(287, 65);
			this.label2.TabIndex = 6;
			this.label2.Text = resources.GetString("label2.Text");
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(13, 223);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(289, 26);
			this.label3.TabIndex = 7;
			this.label3.Text = "Enter 1 per AFB when BOI \"A\" or \"B\" Authorizations are not\r\nsufficient to cover d" +
				 "efuel requirements.";
			// 
			// HSVWorksheet
			// 
			this.AcceptButton = this.buttonOK;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.buttonCancel;
			this.ClientSize = new System.Drawing.Size(373, 314);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.hsvBOIC);
			this.Controls.Add(this.hsvBOIB);
			this.Controls.Add(this.hsvBOIA);
			this.Controls.Add(this.buttonCancel);
			this.Controls.Add(this.buttonOK);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Name = "HSVWorksheet";
			this.Text = "Hydrant Servicing Vehicle Authorization Worksheet";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button buttonOK;
		private System.Windows.Forms.Button buttonCancel;
		private System.Windows.Forms.MaskedTextBox hsvBOIA;
		private System.Windows.Forms.MaskedTextBox hsvBOIB;
		private System.Windows.Forms.MaskedTextBox hsvBOIC;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
	}
}