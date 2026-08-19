namespace Dispatch
{
   partial class ShutdownForm
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
         this.components = new System.ComponentModel.Container();
         this.ShutDownNowButton = new System.Windows.Forms.Button();
         this.ErrorMessageTextBox = new System.Windows.Forms.TextBox();
         this.CountDownLabel = new System.Windows.Forms.Label();
         this.timer1 = new System.Windows.Forms.Timer( this.components );
         this.SuspendLayout();
         // 
         // ShutDownNowButton
         // 
         this.ShutDownNowButton.Location = new System.Drawing.Point( 128, 153 );
         this.ShutDownNowButton.Name = "ShutDownNowButton";
         this.ShutDownNowButton.Size = new System.Drawing.Size( 96, 23 );
         this.ShutDownNowButton.TabIndex = 0;
         this.ShutDownNowButton.Text = "Shutdown &Now";
         this.ShutDownNowButton.UseVisualStyleBackColor = true;
         this.ShutDownNowButton.Click += new System.EventHandler( this.ShutDownNowButtonClick );
         // 
         // ErrorMessageTextBox
         // 
         this.ErrorMessageTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
         this.ErrorMessageTextBox.Font = new System.Drawing.Font( "Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)) );
         this.ErrorMessageTextBox.Location = new System.Drawing.Point( 13, 13 );
         this.ErrorMessageTextBox.Multiline = true;
         this.ErrorMessageTextBox.Name = "ErrorMessageTextBox";
         this.ErrorMessageTextBox.ReadOnly = true;
         this.ErrorMessageTextBox.Size = new System.Drawing.Size( 345, 93 );
         this.ErrorMessageTextBox.TabIndex = 1;
         this.ErrorMessageTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
         // 
         // CountDownLabel
         // 
         this.CountDownLabel.AutoSize = true;
         this.CountDownLabel.Font = new System.Drawing.Font( "Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)) );
         this.CountDownLabel.Location = new System.Drawing.Point( 12, 121 );
         this.CountDownLabel.Name = "CountDownLabel";
         this.CountDownLabel.Size = new System.Drawing.Size( 122, 16 );
         this.CountDownLabel.TabIndex = 2;
         this.CountDownLabel.Text = "Application will exit.";
         // 
         // timer1
         // 
         this.timer1.Interval = 1000;
         this.timer1.Tick += new System.EventHandler( this.Timer1Tick );
         // 
         // ShutdownForm
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF( 6F, 13F );
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.ClientSize = new System.Drawing.Size( 370, 192 );
         this.ControlBox = false;
         this.Controls.Add( this.CountDownLabel );
         this.Controls.Add( this.ErrorMessageTextBox );
         this.Controls.Add( this.ShutDownNowButton );
         this.Name = "ShutdownForm";
         this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
         this.Text = "ShutdownForm";
         this.Load += new System.EventHandler( this.ShutdownFormLoad );
         this.ResumeLayout( false );
         this.PerformLayout();

      }

      #endregion

      private System.Windows.Forms.Button ShutDownNowButton;
      private System.Windows.Forms.TextBox ErrorMessageTextBox;
      private System.Windows.Forms.Label CountDownLabel;
      private System.Windows.Forms.Timer timer1;
   }
}