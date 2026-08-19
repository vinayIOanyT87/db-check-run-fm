namespace DispatchPrototype
{
   partial class WarningBannerForm
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
         if (disposing && (components != null))
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
      private void InitializeComponent ()
      {
         System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager( typeof( WarningBannerForm ) );
         this.textBox1 = new System.Windows.Forms.TextBox();
         this.AcceptBtn = new System.Windows.Forms.Button();
         this.SuspendLayout();
         // 
         // textBox1
         // 
         this.textBox1.BackColor = System.Drawing.SystemColors.Control;
         this.textBox1.BorderStyle = System.Windows.Forms.BorderStyle.None;
         this.textBox1.Location = new System.Drawing.Point( 13, 13 );
         this.textBox1.Multiline = true;
         this.textBox1.Name = "textBox1";
         this.textBox1.ReadOnly = true;
         this.textBox1.Size = new System.Drawing.Size( 502, 301 );
         this.textBox1.TabIndex = 0;
         this.textBox1.TabStop = false;
         this.textBox1.Text = resources.GetString( "textBox1.Text" );
         // 
         // AcceptBtn
         // 
         this.AcceptBtn.Location = new System.Drawing.Point( 224, 323 );
         this.AcceptBtn.Name = "AcceptButton";
         this.AcceptBtn.Size = new System.Drawing.Size( 75, 23 );
         this.AcceptBtn.TabIndex = 1;
         this.AcceptBtn.Text = "Accept";
         this.AcceptBtn.UseVisualStyleBackColor = true;
         this.AcceptBtn.Click += new System.EventHandler( this.AcceptButton_Click );
         // 
         // WarningBannerForm
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF( 6F, 13F );
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.ClientSize = new System.Drawing.Size( 525, 358 );
         this.ControlBox = false;
         this.Controls.Add( this.AcceptBtn );
         this.Controls.Add( this.textBox1 );
         this.Name = "WarningBannerForm";
         this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
         this.Text = "FuelsManager Defense (FMD)";
         this.ResumeLayout( false );
         this.PerformLayout();

      }

      #endregion

      private System.Windows.Forms.TextBox textBox1;
      private System.Windows.Forms.Button AcceptBtn;
   }
}