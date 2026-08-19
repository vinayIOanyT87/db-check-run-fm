using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace FuelsManager
{
   public partial class UnhandledException : System.Web.UI.Page
   {
      protected void Page_Load(object sender, EventArgs e)
      {
         try
         {
            Exception except = this.Server.GetLastError();
            if (except != null)
            {
               Global.WriteToEventLog(except.Message, System.Diagnostics.EventLogEntryType.Error);
               Global.WriteToEventLog(except.StackTrace, System.Diagnostics.EventLogEntryType.Error);
               this.Server.ClearError();
            }
         }
         catch 
         {
            ;
         }
      }
   }
}