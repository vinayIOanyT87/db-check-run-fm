using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Windows.Forms;

namespace DispatchPrototype
{
   static class Program
   {
      /// <summary>
      /// The main entry point for the application.
      /// </summary>
      [STAThread]
      static void Main ()
      {
         try
         {
            // Remove "EXE" from default configuration file name per request of Varec Configuration Management.
            AppDomain.CurrentDomain.SetData( "APP_CONFIG_FILE", "dispatch.config" );


            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault( false );
            Application.Run( new WarningBannerForm() );
            Application.Run( new LoginForm() );
         }
         catch (Exception except)
         {
            MessageBox.Show( except.Message, "Dispatch" );
         }
      }
   }
}
