namespace DispatchPrototype
{
   partial class EmbeddedBrowser
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
         this.webBrowser1 = new ExtendedWebBrowser();
         this.webBrowser1.Quit += new System.EventHandler( EmbeddedBrowser_Quit );
         this.panel1 = new System.Windows.Forms.Panel();
         this.panel1.SuspendLayout();
         this.SuspendLayout();
         // 
         // webBrowser1
         // 
         this.webBrowser1.Location = new System.Drawing.Point( 0, 0 );
         this.webBrowser1.MinimumSize = new System.Drawing.Size( 20, 20 );
         this.webBrowser1.Name = "webBrowser1";
         this.webBrowser1.Size = new System.Drawing.Size( 1137, 600 );
         this.webBrowser1.TabIndex = 0;
         // 
         // panel1
         // 
         this.panel1.BackColor = System.Drawing.Color.White;
         this.panel1.Controls.Add( this.webBrowser1 );
         this.panel1.Location = new System.Drawing.Point( -1, 0 );
         this.panel1.Name = "panel1";
         this.panel1.Size = new System.Drawing.Size( 1165, 779 );
         this.panel1.TabIndex = 0;
         // 
         // EmbeddedBrowser
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF( 6F, 13F );
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.ClientSize = new System.Drawing.Size( 1164, 778 );
         this.Controls.Add( this.panel1 );
         this.MaximizeBox = false;
         this.MaximumSize = new System.Drawing.Size( 1172, 812 );
         this.MinimizeBox = false;
         this.Name = "EmbeddedBrowser";
         this.ShowIcon = false;
         this.ShowInTaskbar = false;
         this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
         this.Text = "FuelsManager";
         this.panel1.ResumeLayout( false );
         this.ResumeLayout( false );

      }

      #endregion

		private ExtendedWebBrowser webBrowser1;
		private System.Windows.Forms.Panel panel1;
   }
}