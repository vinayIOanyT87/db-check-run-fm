using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace DispatchPrototype
{
   public partial class ShutdownForm : FMBaseForm
   {
      private int countDown = 10;

      public string ErrorMessage
      {
         set
         {
            ErrorMessageTextBox.Text = value;
         }
      }

      public ShutdownForm ()
      {
         InitializeComponent();
      }

      private void timer1_Tick ( object sender, EventArgs e )
      {
         try
         {
            --countDown;
            ShowCountDownMessage();

            if (countDown == 0)
            {
               this.Close();
            }

         }
         catch (Exception except)
         {
            ErrorHandler( except );
         }
      }

      private void ShowCountDownMessage ()
      {
         CountDownLabel.Text = string.Format( "Application will exit in {0} second(s)", countDown );
      }

      private void ShutdownForm_Load ( object sender, EventArgs e )
      {
         try
         {
            ShowCountDownMessage();
            timer1.Enabled = true;
         }
         catch (Exception except)
         {
            ErrorHandler( except );
         }
      }

      private void ShutDownNowButton_Click ( object sender, EventArgs e )
      {
         this.Close();
      }
   }
}
