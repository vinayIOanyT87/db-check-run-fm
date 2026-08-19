
using FMBusinessObjects.Exceptions;
using System.Web.Mvc;


namespace FuelsManager.Areas.Controllers
{
   [SessionState(System.Web.SessionState.SessionStateBehavior.ReadOnly)]
   public class FileNotFoundController : FMBaseController
   {


      [HttpGet]
      public ActionResult NotFound()
      {
         try
         {

            int p = this.Request.Url.AbsolutePath.ToLower().IndexOf("bundles/");

            if (p > -1)
            {
               p = this.Request.Url.AbsolutePath.ToLower().IndexOf(".map;", p + 8);
               if (p > -1)
               {
                  //.map files allow a browser to download a full version of the minified JS.
                  //It is for debugging purposes. Missing .map file shouldn't be an issue.
                  return null;
               }
            }
            string msg = "Path not found : " + this.Request.Url.AbsolutePath;
            var session = System.Web.HttpContext.Current.Session;

            string[] parts = this.Request.Url.AbsolutePath.Split('/');
            if (parts.Length > 2)
            {

               if (parts[parts.Length - 2] == "images")
               {
                  string resourceName = parts[parts.Length - 1];
                  p = resourceName.LastIndexOf(".");
                  if (p == -1 || p == resourceName.Length - 1)
                  {
                     if (session != null)
                     {
                        session["Status"] = "Error";
                     }
                     Global.WriteToEventLog(msg + ".\nLogging out.", System.Diagnostics.EventLogEntryType.Error);
                     return Redirect("~/FMWebApp/LogoutForm.aspx?InvalidSession=true");
                  }
                  string suf = resourceName.Substring(p + 1).ToUpper();
                  if (suf == "PNG" || suf == "JPG" || suf == "JPEG"
                     || suf == "ICO" || suf == "SVG" || suf == "GIF"
                     || suf == "CUR")
                  {
                     Global.WriteToEventLog(msg, System.Diagnostics.EventLogEntryType.Warning);
                     return null;
                  }
               }

               if (parts.Length == 5 && parts[2].ToLower() == "fmwebapp" && parts[3].ToLower() == "sounds")
               {
                  msg = "Path not found : " + this.Request.Url.AbsolutePath + ".\nConfigured alarm priority audio file not found. Please check your configuration.";
                  Global.WriteToEventLog(msg, System.Diagnostics.EventLogEntryType.Warning);
                  return null;
               }
            }

            if (session != null)
            {
               session["Status"] = "Error";
            }
            Global.WriteToEventLog(msg + ".\nLogging out.", System.Diagnostics.EventLogEntryType.Error);
            return Redirect("~/FMWebApp/LogoutForm.aspx?InvalidSession=true");

         }
         catch (System.Exception ex)
         {
            Global.WriteToEventLog(ex.Message, System.Diagnostics.EventLogEntryType.Error);
            Global.WriteToEventLog(ex.StackTrace, System.Diagnostics.EventLogEntryType.Error);
         }
         return null;
      }
   }

}