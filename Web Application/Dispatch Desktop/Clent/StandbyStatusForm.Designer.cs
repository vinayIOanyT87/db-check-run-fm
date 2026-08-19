namespace Dispatch
{
	partial class StandbyStatusForm
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
			if (disposing && (this.components != null))
			{
				this.components.Dispose();
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
         this.StandbyStatuslistView = new System.Windows.Forms.ListView();
         this.Closebutton = new System.Windows.Forms.Button();
         this.Dispatchbutton = new System.Windows.Forms.Button();
         this.SuspendLayout();
         // 
         // StandbyStatuslistView
         // 
         this.StandbyStatuslistView.FullRowSelect = true;
         this.StandbyStatuslistView.Location = new System.Drawing.Point( 12, 12 );
         this.StandbyStatuslistView.MultiSelect = false;
         this.StandbyStatuslistView.Name = "StandbyStatuslistView";
         this.StandbyStatuslistView.Size = new System.Drawing.Size( 307, 130 );
         this.StandbyStatuslistView.TabIndex = 0;
         this.StandbyStatuslistView.UseCompatibleStateImageBehavior = false;
         this.StandbyStatuslistView.View = System.Windows.Forms.View.List;
         this.StandbyStatuslistView.SelectedIndexChanged += new System.EventHandler( this.OnListViewSelectedIndexChanged );
         // 
         // Closebutton
         // 
         this.Closebutton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
         this.Closebutton.Location = new System.Drawing.Point( 177, 157 );
         this.Closebutton.Name = "Closebutton";
         this.Closebutton.Size = new System.Drawing.Size( 75, 23 );
         this.Closebutton.TabIndex = 1;
         this.Closebutton.Text = "&Close";
         this.Closebutton.UseVisualStyleBackColor = true;
         this.Closebutton.Click += new System.EventHandler( this.ClosebuttonClick );
         // 
         // Dispatchbutton
         // 
         this.Dispatchbutton.Location = new System.Drawing.Point( 76, 157 );
         this.Dispatchbutton.Name = "Dispatchbutton";
         this.Dispatchbutton.Size = new System.Drawing.Size( 75, 23 );
         this.Dispatchbutton.TabIndex = 2;
         this.Dispatchbutton.Text = "&Dispatch";
         this.Dispatchbutton.UseVisualStyleBackColor = true;
         this.Dispatchbutton.Click += new System.EventHandler( this.DispatchbuttonClick );
         // 
         // StandbyStatusForm
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF( 6F, 13F );
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.CancelButton = this.Closebutton;
         this.ClientSize = new System.Drawing.Size( 328, 192 );
         this.ControlBox = false;
         this.Controls.Add( this.Dispatchbutton );
         this.Controls.Add( this.Closebutton );
         this.Controls.Add( this.StandbyStatuslistView );
         this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
         this.MaximizeBox = false;
         this.MinimizeBox = false;
         this.Name = "StandbyStatusForm";
         this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
         this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
         this.Text = "Standby Status Board";
         this.Load += new System.EventHandler( this.StandbyFormLoad );
         this.ResumeLayout( false );

		}

		#endregion

		private System.Windows.Forms.ListView StandbyStatuslistView;
		private System.Windows.Forms.Button Closebutton;
		private System.Windows.Forms.Button Dispatchbutton;
	}
}