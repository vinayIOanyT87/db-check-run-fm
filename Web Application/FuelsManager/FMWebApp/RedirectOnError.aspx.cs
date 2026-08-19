using FMBusinessObjects.DataObjects;
using FMCore;
using Interop.DataObjects;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace FuelsManager
{
   public partial class WebForm1 : System.Web.UI.Page
   {
      protected string transferToPage = "/fuelsmanager";
      private const string logoutUrl = "~/FMWebApp/LogoutForm.aspx";

      protected void Page_Load(object sender, EventArgs e)
      {
         transferToPage = ResolveUrl(logoutUrl);


         if (this.Session != null && this.Session["Security"] != null)
         {
            string errorDetails = Session["ErrorMessage"] as string;

            if (string.IsNullOrWhiteSpace(errorDetails))
            {
               errorDetails = "Unknow error";
            }
            else
            {
               errorDetails = errorDetails.Trim();
            }
            Global.WriteToEventLog(errorDetails, System.Diagnostics.EventLogEntryType.Error);
            Session.Remove("ErrorMessage");
            if (errorDetails.IndexOf("Invalid Session", StringComparison.OrdinalIgnoreCase) != -1
            || (errorDetails.IndexOf("The controller for path ", StringComparison.OrdinalIgnoreCase) != -1 && errorDetails.IndexOf(" was not found or does not implement IController", StringComparison.OrdinalIgnoreCase) != -1))
            {
               return;
            }
         }
         SetTransferToPage();
      }
      void SetTransferToPage()
      {
         if (Session == null)
         {
            return;
         }
 
         if (this.Session["Security"] != null)
         {

            string previousUrl = Session["PreviousUrl"] as string;
            if (string.IsNullOrWhiteSpace(previousUrl))
            {
               previousUrl = ResolveUrl(logoutUrl);
            }

            if (previousUrl.IndexOf("RedirectOnError.aspx", StringComparison.OrdinalIgnoreCase) < 0)
            {
               transferToPage = previousUrl;
            }
            Session.Remove("PreviousUrl");
            Session.Remove("TransferToPage");

            if (transferToPage.IndexOf("AlarmSummary2/ControllerPingMechanism", StringComparison.OrdinalIgnoreCase) >= 0 ||
               transferToPage.IndexOf("AlarmSummary2/SyncUnresolvedConflictsCount", StringComparison.OrdinalIgnoreCase) >= 0 ||
               transferToPage.IndexOf("AlarmSummary2/AlarmNotificationsForMenu", StringComparison.OrdinalIgnoreCase) >= 0)
            {
               string token = Session["Token"] as string;
               if (token == null)
               {
                  transferToPage = ResolveUrl(logoutUrl);
               }
            }

         }
         if (Session.IsCookieless)
         {
            string embeddedSessionID = string.Format("/(S({0}))/", Session.SessionID);
            int p = transferToPage.IndexOf(embeddedSessionID);
            if (p == -1)
            {
               transferToPage = transferToPage.Replace(this.Request.ApplicationPath, "~");
            }

         }
         transferToPage = ResolveUrl(transferToPage);

         if (transferToPage.Equals(this.Request.Url.PathAndQuery, StringComparison.OrdinalIgnoreCase))
         {
            //Avoid a loop, where control returns to the page that generated the error.
            //Return to FuelsManagerForm instead.
            SecurityClass security = Session["Security"] as SecurityClass;
            if (security != null)
            {
               string fuelsManagerForm = "~/FMWebApp/FuelsManagerForm.aspx?E";
               if (transferToPage.IndexOf(fuelsManagerForm) < 0)
               {
                  transferToPage = string.Format("{0}&{1}", fuelsManagerForm, security.CSRFTokenWithParamName);
               }
               else
               {
                  //error seems to be reoccurring in FuelsManagerForm. Don't want to get stuck in FuelsManagerForm. Logout.
                  transferToPage = logoutUrl;
               }
               transferToPage = ResolveUrl(transferToPage);
            }
         }
      }

   }
}