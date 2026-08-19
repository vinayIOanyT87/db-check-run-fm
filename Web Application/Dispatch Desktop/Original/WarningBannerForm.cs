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
   public partial class WarningBannerForm : FMBaseForm
   {
      public WarningBannerForm ()
      {
         InitializeComponent();
      }

      private void AcceptButton_Click ( object sender, EventArgs e )
      {
         try
         {
            this.Close();
         }
         catch (Exception except)
         {
            ErrorHandler( except );
         }
      }
   }
}
