namespace DispatchPrototype
{
	partial class CommentForm
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
			this.CancelCommentTextBox = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.okbutton = new System.Windows.Forms.Button();
			this.label2 = new System.Windows.Forms.Label();
			this.Fortextbox = new System.Windows.Forms.TextBox();
			this.SuspendLayout();
			// 
			// CancelCommentTextBox
			// 
			this.CancelCommentTextBox.Location = new System.Drawing.Point(14, 64);
			this.CancelCommentTextBox.Margin = new System.Windows.Forms.Padding(2);
			this.CancelCommentTextBox.Multiline = true;
			this.CancelCommentTextBox.Name = "CancelCommentTextBox";
			this.CancelCommentTextBox.Size = new System.Drawing.Size(593, 74);
			this.CancelCommentTextBox.TabIndex = 4;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(13, 46);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(51, 13);
			this.label1.TabIndex = 5;
			this.label1.Text = "Comment";
			// 
			// okbutton
			// 
			this.okbutton.Location = new System.Drawing.Point(273, 158);
			this.okbutton.Name = "okbutton";
			this.okbutton.Size = new System.Drawing.Size(75, 23);
			this.okbutton.TabIndex = 6;
			this.okbutton.Text = "OK";
			this.okbutton.UseVisualStyleBackColor = true;
			this.okbutton.Click += new System.EventHandler(this.okbutton_clicked);
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(13, 17);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(133, 13);
			this.label2.TabIndex = 7;
			this.label2.Text = "Cancellation Comment For:";
			// 
			// Fortextbox
			// 
			this.Fortextbox.Location = new System.Drawing.Point(156, 13);
			this.Fortextbox.Name = "Fortextbox";
			this.Fortextbox.ReadOnly = true;
			this.Fortextbox.Size = new System.Drawing.Size(362, 20);
			this.Fortextbox.TabIndex = 8;
			// 
			// CommentForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(620, 196);
			this.ControlBox = false;
			this.Controls.Add(this.Fortextbox);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.okbutton);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.CancelCommentTextBox);
			this.Name = "CommentForm";
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Cancellation Comment";
			this.Load += new System.EventHandler(this.CancelCommentForm_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TextBox CancelCommentTextBox;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Button okbutton;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox Fortextbox;
	}
}