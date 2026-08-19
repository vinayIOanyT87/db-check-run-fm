// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Global.asax.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the Global ASAX type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Security.Claims;
using System.ServiceModel;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Helpers;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Security.Cryptography;
using System.Web.Security;

using FMBusinessObjects.BusinessInterfaces;
using FMBusinessObjects.ChannelFactories;
using FMBusinessObjects.Constants;
using FMBusinessObjects.DataObjects;
using FMBusinessObjects.Exceptions;
using FMBusinessObjects.UtilityObjects;

using FMCore;

using FuelsManager.Areas.App_Start;
using Unity;
using FuelsManager.Interfaces;
using FuelsManager.Services;
using FMDepedencyManager;
using FMWebAPIBusinessLogic.Interfaces.FMBusinessLogic;
using FMWebAPIBusinessLogic;

using FuelsManager.Areas.Controllers;
using System.Web.UI;
using System.Web.SessionState;
using System.Collections.Generic;
using Opc.Ua.Configuration;
using System.Configuration;
using System.Linq;
using System.IO;

public class Global : HttpApplication
{
   /// <summary>
   /// Required designer variable.
   /// </summary>
   private IContainer components = null;

   /// <summary>
   /// Used to keep track of the time a request began for performance logging
   /// </summary>
   private DateTime startRequestTime;

   /// <summary>
   /// Used to keep track of the time a request ended for performance logging
   /// </summary>
   private DateTime endRequestTime;

   static private string loadBalancerCertSubjectString = string.Empty;

   static public string LoadBalancerCertSubjectString
   {
      get
      {
         return loadBalancerCertSubjectString;
      }
   }
   static public bool IsFdsIM
   {
      get
      {
         bool? isFdsIM = AppDomain.CurrentDomain.GetData("IsFdsIM") as bool?;

         if (isFdsIM == null || isFdsIM.HasValue == false)
         {
            try
            {
               isFdsIM =  ApplicationInformation.IsFDSIM;              
               AppDomain.CurrentDomain.SetData("IsFdsIM", isFdsIM);
            }
            catch (Exception ex)
            {
               WriteToEventLog(ex.Message, EventLogEntryType.Error);
               WriteToEventLog(ex.StackTrace, EventLogEntryType.Error);
            }
            string message = $">>>>>>>>>>>>>>>>>>IsFdsIM =  {isFdsIM}";
            WriteToEventLog(message, EventLogEntryType.Information);
         }

         return isFdsIM.Value;
      }
   }

   static public bool AccessibilityEnabled
   {
      get
      {
         bool? accessibilityEnabled = AppDomain.CurrentDomain.GetData("AccessibilityEnabled") as bool?;

         if (accessibilityEnabled == null || accessibilityEnabled.HasValue == false)
         {

            string accessibilityEnabledStr = ConfigurationManager.AppSettings["AccessibilityEnabled"].DefaultIfNull("false").Trim();
            if (!string.IsNullOrEmpty(accessibilityEnabledStr))
            {
               if (accessibilityEnabledStr.Equals("false", StringComparison.OrdinalIgnoreCase) || accessibilityEnabledStr == "0")
               {
                  accessibilityEnabled = false;
               }
               else
               {
                  accessibilityEnabled = true;
               }
               AppDomain.CurrentDomain.SetData("AccessibilityEnabled", accessibilityEnabled);
            }
            string message = $">>>>>>>>>>>>>>>>>>AccessibilityEnabled =  {accessibilityEnabled}";
            WriteToEventLog(message, EventLogEntryType.Information);
         }

         return accessibilityEnabled.Value;
      }
   }

   static public string LinkAccessibilityCssUrl(HttpSessionStateBase sessionState)
   {
      if (AccessibilityEnabled)
      {
         UserAccessibilityDO ua = sessionState["Accessibility"] as FMBusinessObjects.DataObjects.UserAccessibilityDO;
         if (ua != null)
         {
            if (ua.Enabled)
            {
               return string.Format("<link href='{0}/css/accessibility.css' media='screen' rel='stylesheet' type='text/css' />", HttpRuntime.AppDomainAppVirtualPath);

            }
         }
      }
      return string.Empty;
   }
   static public string LinkAccessibilityCssUrl(HttpSessionState sessionState)
   {
      if (AccessibilityEnabled)
      {
         UserAccessibilityDO ua = sessionState["Accessibility"] as FMBusinessObjects.DataObjects.UserAccessibilityDO;
         if (ua != null)
         {
            if (ua.Enabled)
            {
               return string.Format("<link href='{0}/css/accessibility.css' media='screen' rel='stylesheet' type='text/css' />", HttpRuntime.AppDomainAppVirtualPath);

            }
         }
      }
      return string.Empty;
   }

   public Global()
   {
      this.InitializeComponent();
   }

   /// <summary>
   /// This is used by web pages.  Somehow codebehind code can do Trace statements without problems.
   /// But when it is calling from the web page, it is gone.
   /// If you need to write it from web page, that means code enclosed in <% %>.  You may consider using this.
   /// </summary>
   /// <param name="message"></param>
   /// <param name="level"></param>
   private static void MyTrace(string message, TraceLevel level)
   {
      try
      {
         switch (level)
         {
            case TraceLevel.Error:
               Trace.TraceError(message);
               break;

            case TraceLevel.Warning:
               Trace.TraceWarning(message);
               break;

            case TraceLevel.Info:
               Trace.TraceInformation(message);
               break;

            default:
               Trace.WriteLine(message);
               break;

         }
      }
      catch
      {
         ;
      }
   }

   /// <summary>
   /// Test Method, will remove if it doesn't work
   /// </summary>
   /// <param name="message"></param>
   /// <param name="entryType"></param>
   public static void WriteToEventLog(string message, EventLogEntryType entryType, int eventID = 0)
   {
      try
      {
         using (var eventLog = new EventLog("Application", ".", "FuelsManager"))
         {
            eventLog.WriteEntry("EventLog-" + message, entryType, eventID);
         }
      }
      catch
      {
         var level = TraceLevel.Verbose;
         switch (entryType)
         {
            case EventLogEntryType.Error:
               level = TraceLevel.Error;
               break;
            case EventLogEntryType.Warning:
               level = TraceLevel.Warning;
               break;
            case EventLogEntryType.Information:
            case EventLogEntryType.SuccessAudit:
            case EventLogEntryType.FailureAudit:
               level = TraceLevel.Info;
               break;
         }
         MyTrace("[EventLog fallback]" + message, level);
      }
   }

   /// <summary>
   /// This is just to make sure the diagnostics log is working. 
   /// </summary>
   private static void SetupDiagnostic()
   {
   }

   static public bool ComplianceFlag{
      get
      {
         var complianceScanFlag = AppDomain.CurrentDomain.GetData("ComplianceScanFlag") as bool?;

         if (complianceScanFlag == null || complianceScanFlag.Value == false)
         {
            return false;
         }
         return true;
      }
   }

   protected void Application_Start(Object sender, EventArgs e)
   {
      var complianceScanFlag = AppDomain.CurrentDomain.GetData("ComplianceScanFlag") as bool?;
      if (complianceScanFlag == null)
      {
         complianceScanFlag = new bool?(FMBusinessObjects.UtilityObjects.AppSettingsHelper.GetKeyValue("ComplianceScanFlag", false));

         AppDomain.CurrentDomain.SetData("ComplianceScanFlag", complianceScanFlag);
      }

      if (complianceScanFlag.Value)
      {
         AntiForgeryConfig.SuppressXFrameOptionsHeader = true;
      }
      WriteToEventLog(string.Format("AntiForgeryConfig.SuppressXFrameOptionsHeader = {0}", AntiForgeryConfig.SuppressXFrameOptionsHeader), EventLogEntryType.Information);

      SetupDiagnostic();

      AreaRegistration.RegisterAllAreas();

      WebApiConfig.Register(GlobalConfiguration.Configuration);
      FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
      RouteConfig.RegisterRoutes(RouteTable.Routes);
      BundleConfig.RegisterBundles(BundleTable.Bundles);

      AntiForgeryConfig.UniqueClaimTypeIdentifier = ClaimTypes.NameIdentifier;

      FMServiceLocator.Container = new UnityContainer();
      FMServiceLocator.Container.RegisterFMCoreServices();
      FMServiceLocator.Container.RegisterFuelManagerWebAPIBusinessServices();
      FMServiceLocator.Container.RegisterType<IFuelManagerConfigurationFactory, FuelManagerConfigurationFactory>();
      FMServiceLocator.Container.RegisterType<AngularJavaScriptToPageService>();
      DependencyResolver.SetResolver(new UnityResolver(FMServiceLocator.Container));


      string JQueryVer = "2.2.1";
      ScriptManager.ScriptResourceMapping.AddDefinition("jquery", new ScriptResourceDefinition
      {
         Path = "~/Scripts/jquery-" + JQueryVer + ".min.js",
         DebugPath = "~/Scripts/jquery-" + JQueryVer + ".js",
         LoadSuccessExpression = "window.jQuery"
      });

      GlobalConfiguration.Configuration.EnsureInitialized();

      if (IsFdsIM)
      {
         WriteToEventLog("FDS-IM web application started.", EventLogEntryType.Information);
         loadBalancerCertSubjectString = ConfigurationManager.AppSettings["LoadBalancerCertificateSubject"].DefaultIfNull(string.Empty);
         string message = string.Format("Load balancer certificate subject string = {0}", loadBalancerCertSubjectString);
         WriteToEventLog(message, EventLogEntryType.Information);
      }
   }

   protected void Session_Start(Object sender, EventArgs e)
   {
      if (AppDomain.CurrentDomain.GetData("EnterpriseConfigCheck") == null)
      {
         try
         {
            FMChannelHelper.MakeCall<IConfigurationSettings>(x => x.UpdateIsEnterpriseSetting());
         }
         catch
         {
            // if the database is down this throws an un-captured exception
         }
         AppDomain.CurrentDomain.SetData("EnterpriseConfigCheck", "Just has to exist");
      }
      if (this.Context != null && this.Context.Handler != null)
      {
         Type obj = this.Context.Handler.GetType();
         if (obj.FullName == "System.Web.Optimization.BundleHandler")
         {
            //No need to check bundles
            return;
         }
      }
      InitializeNewSession();
   }
   static public object lockNewSessionStates = new object();
   /*
    *  When new session is created adds session variables that were created for new session id during successful login such as token, csrfToken, security, etc.
    *  Removes session items of sessions that have been stored more than 2 hours. Most likely abandoned by expired sessions.
    */
   protected void InitializeNewSession()
   {
      lock (lockNewSessionStates)
      {
         Dictionary<string, SessionStateItemCollection> newSessionStates = AppDomain.CurrentDomain.GetData("NewSessionStateItems") as Dictionary<string, SessionStateItemCollection>;
         if (newSessionStates == null)
         {
            newSessionStates = new Dictionary<string, SessionStateItemCollection>();
            AppDomain.CurrentDomain.SetData("NewSessionStateItems", newSessionStates);
         }
         SessionStateItemCollection sessionStateItems = null;

         if (newSessionStates.ContainsKey(this.Session.SessionID))
         {
            //Add session variables created during successful login to current session
            sessionStateItems = newSessionStates[this.Session.SessionID];
            if (sessionStateItems != null)
            {
               foreach (string key in sessionStateItems)
               {
                  this.Context.Session[key] = sessionStateItems[key];
               }
               newSessionStates.Remove(this.Context.Session.SessionID);
            }
         }
         //Remove any items belonging to old sessions which may have failed to remove them before.
         List<string> expiredSessionStateItems = new List<string>();
         foreach (string sessionid in newSessionStates.Keys)
         {
            SessionStateItemCollection stateItems = newSessionStates[sessionid];
            if (stateItems["CreatedDateTime"] != null)
            {
               DateTime t0 = (DateTime)stateItems["CreatedDateTime"];
               DateTime t1 = DateTime.UtcNow;
               TimeSpan dt = t1 - t0;
               if (dt.Hours > 2)
               {
                  expiredSessionStateItems.Add(sessionid);
               }
            }
         }
         if (expiredSessionStateItems.Count > 0)
         {
            foreach (string sessionid in expiredSessionStateItems)
            {
               newSessionStates.Remove(sessionid);
            }
            expiredSessionStateItems.Clear();
         }
         //WriteToEventLog(string.Format("newSessionStates.Count={0}", newSessionStates.Count ), EventLogEntryType.Information);
      }
   }

   protected void Application_BeginRequest(Object sender, EventArgs e)
   {
      if (this.Request.Url.AbsolutePath.IndexOf("/fmwebapp/umnyangoform.aspx", StringComparison.OrdinalIgnoreCase) > 0 &&
          this.Request.QueryString.AllKeys.Contains("healthcheck"))
      {
         try
         {
            var versionInfo = FMChannelHelper.MakeCall<IDBAccess, VersionInfo>(x => x.GetVersion());
         }
         catch
         {
            this.Response.Write("Service Status Down.");
            this.Response.Flush();
            this.Context.ApplicationInstance.CompleteRequest();
            return;
         }
      }

      if (this.Request.ContentType.Contains("application/json"))
      {
         //This is to catch any HTTP 500 code responses when handling REST web services
         var filter = new ResponseFilterClass(this.Response.Filter)
         {
            Session = null,
            Response = this.Response
         };

         this.Response.Filter = filter;
      }

      if (ComplianceFlag == false)
      {
         //Enable HSTS (HTTP Strict Transport Security)
         switch (Request.Url.Scheme)
         {
            case "https":
               Response.AppendHeader("Strict-Transport-Security", "max-age=31536000; includeSubDomains; preload");
               break;
            case "http":
               var path = "https://" + Request.Url.Host + Request.Url.PathAndQuery;
               Response.Status = "301 Moved Permanently";
               Response.AppendHeader("Location", path);
               break;
         }
      }

      HttpContext.Current.Response.Cache.SetRevalidation(HttpCacheRevalidation.AllCaches);
      HttpContext.Current.Response.Cache.SetCacheability(HttpCacheability.Private);

      // Bug fix for MS SSRS Blank.gif 500 server error missing parameter IterationId
      // https://connect.microsoft.com/VisualStudio/feedback/details/556989/
      if (HttpContext.Current.Request.Url.PathAndQuery.Contains("/Reserved.ReportViewerWebControl.axd")
      && !String.IsNullOrEmpty(HttpContext.Current.Request.QueryString["ResourceStreamID"])
      && HttpContext.Current.Request.QueryString["ResourceStreamID"].ToLower().Equals("blank.gif"))
      {
         this.Context.RewritePath(String.Concat(HttpContext.Current.Request.Url.PathAndQuery, "&IterationId=0"));
      }

      this.startRequestTime = DateTime.Now;
   }

   protected void Application_EndRequest(Object sender, EventArgs e)
   {
      this.endRequestTime = DateTime.Now;

      // If the application setting to enable performance logging is set to true, log the total time the request took,
      // but only if it's greater than a configurable threshold specified in milliseconds 
      if (AppSettingsHelper.GetKeyValue("EnablePerformanceLogging", false))
      {
         TimeSpan elapsed = this.endRequestTime - this.startRequestTime;

         if (elapsed.TotalMilliseconds > AppSettingsHelper.GetKeyValue("PerformanceLoggingThresholdMilliseconds", 0))
         {
            string logEntry = string.Format(
                "Performance Log IP: {0} Start: {1} End: {2} Elapsed: {3} URL: {4}",
                this.Request.ServerVariables["REMOTE_ADDR"],
                this.startRequestTime.ToString("hh:mm:ss.fff tt"),
                this.endRequestTime.ToString("hh:mm:ss.fff tt"),
                elapsed.TotalMilliseconds,
                this.Request.Url);

            Trace.TraceInformation(logEntry);
         }
      }
   }

   protected void Application_AuthenticateRequest(Object sender, EventArgs e)
   {
      //if (!Request.IsAuthenticated) Response.Redirect("UmnyangoForm.aspx");
      var request = Request;
   }
   protected void Application_PostAuthenticateRequest(Object sender, EventArgs e)
   {

   }

   protected void Application_AuthorizeRequest(Object sender, EventArgs e)
   {

   }

   protected void Application_PostAcquireRequestState(Object sender, EventArgs e)
   {

   }
   protected void Application_ResolveRequestCache(Object sender, EventArgs e)
   {

   }
   protected void Application_PostAuthorizeRequest(Object sender, EventArgs e)
   {
      if (IsWebApiRequest())
      {
         //setup session for web api
         HttpContext.Current.SetSessionStateBehavior(System.Web.SessionState.SessionStateBehavior.Required);
      }
   }
    protected void Application_Error(Object sender, EventArgs e)
    {
        Exception except = this.Server.GetLastError();

        if (except != null)
        {
            while (except.InnerException != null)
            {
                Trace.TraceError(except.ToString());
                except = except.InnerException;
            }

            // Process unhandled FMFatalErrorException type and if FuelsManager has been
            // shut down as a result then notify the user and stop all processing

            var fatalErrorEx = except as FMFatalErrorException;
            if (fatalErrorEx == null)
            {
                var fatalErr2 = except as FaultException<FMFatalErrorException>;
                if (fatalErr2 != null)
                {
                    fatalErrorEx = fatalErr2.Detail;
                }
            }

            if (fatalErrorEx != null)
            {
                var security = (SecurityClass)Session["Security"];
                bool shutdownFuelsManager = FMChannelHelper.MakeCall<IFMFatalErrorHandler, bool>(x => x.ProcessFatalError(security, fatalErrorEx));
                if (shutdownFuelsManager)
                {
                    Response.Clear();
                    this.Response.AppendHeader("Cache-Control", "private");
                    Response.Write(FMFatalErrorHandlerClass.Header);
                    var notificationMessage = string.Format(FMFatalErrorHandlerClass.NotificationFormatter, fatalErrorEx.Message);
                    Response.Write(notificationMessage);
                    Response.Write(FMFatalErrorHandlerClass.Footer);
                    Response.Flush();
                    Server.ClearError();
                    return;
                }
                WriteToEventLog(fatalErrorEx.Message, EventLogEntryType.Error);
            }

            string message = this.StripReturns(except.Message);

            message = message.Replace("--->", "");

            if (message != "Response is not available in this context.")
            {
                bool errMsgLogged = false;
                if (message.ToLower().Contains("at fuelsmanager") || message.ToLower().Contains("at system"))
                {
                    if (message.ToLower().Contains("exception")
                        || message.ToLower().Contains("trace")
                        || message.ToLower().Contains(":line "))
                    {
                        //Generate hash to cross reference displayed message with one in event log.
                        MD5CryptoServiceProvider md5Provider = new MD5CryptoServiceProvider();
                        var encoding = new System.Text.ASCIIEncoding();
                        var exceptionBytes = encoding.GetBytes(message);
                        var hashCode = md5Provider.ComputeHash(exceptionBytes);
                        var base64Hash = Convert.ToBase64String(hashCode);//Cross reference displayed error with message that is logged.
                        try
                        {
                            FMChannelHelper.MakeCall<IFMEventLog>(x => x.WriteEntry(string.Format("(ref:{0}):\n{1}", base64Hash, message), FMEventLogEntryType.Error));
                            FMChannelHelper.MakeCall<IFMEventLog>(x => x.WriteEntry(string.Format("(ref:{0}):\n{1}", base64Hash, except.StackTrace.ToString()), FMEventLogEntryType.Error));
                            message = string.Format("Error occured while processing request. See event log for details. (ref:{0})", base64Hash);
                            errMsgLogged = true;
                        }
                        catch
                        {
                            ;
                        }
                    }

                }
                if (errMsgLogged == false)
                {
                    try
                    {
                        FMChannelHelper.MakeCall<IFMEventLog>(x => x.WriteEntry(string.Format("{0}", message), FMEventLogEntryType.Error));
                        FMChannelHelper.MakeCall<IFMEventLog>(x => x.WriteEntry(string.Format("{0}", except.StackTrace.ToString()), FMEventLogEntryType.Error));
                    }
                    catch
                    {
                        ;
                    }
                    message = string.Format("Error: {0}.\n\rSee event log for details. ", message);

                }
                Trace.TraceError(except.ToString());
               if (this.Context.Session != null && this.Context.Session["Security"] != null)
               {
                  SecurityClass security = this.Context.Session["Security"] as SecurityClass;
                  string statusError = this.Session["Status"] as string;
                  if (this.Context.Handler is MvcHandler && string.IsNullOrWhiteSpace(statusError) == false)
                  {
                     //from MVC error and exception handler.
                     if ((this.Request.AppRelativeCurrentExecutionFilePath.IndexOf("Operate/UpdateValues", StringComparison.OrdinalIgnoreCase) < 0)
                        && (this.Request.AppRelativeCurrentExecutionFilePath.IndexOf("AlarmSummary2/AlarmNotifications", StringComparison.OrdinalIgnoreCase) < 0)
                        && (this.Request.AppRelativeCurrentExecutionFilePath.IndexOf("AlarmSummary2/SyncUnresolvedConflictsCount", StringComparison.OrdinalIgnoreCase) < 0)
                        && (this.Request.AppRelativeCurrentExecutionFilePath.IndexOf("AlarmSummary2/ControllerPingMechanism", StringComparison.OrdinalIgnoreCase) < 0)
                        && (this.Request.AppRelativeCurrentExecutionFilePath.IndexOf("AlarmSummary2/AlarmNotificationsForMenu", StringComparison.OrdinalIgnoreCase) < 0)
								&& (this.Request.AppRelativeCurrentExecutionFilePath.IndexOf("Operate/SetOperateActive", StringComparison.OrdinalIgnoreCase) < 0))
                     {
                        string path = string.Format("~/FMWebApp/FuelsManagerForm.aspx?{0}", security.CSRFTokenWithParamName);
                        path = Context.Response.ApplyAppPathModifier(path);
                        Session["PreviousUrl"] = path;
                     }
                     else
                     {
                        return;
                     }
                  }

                  this.Response.Clear();
                  this.Response.Cache.SetCacheability(HttpCacheability.Private);
                  this.Response.AppendHeader("Cache-Control", "private");
                  if (Request.RawUrl.IndexOf("/FMWebApp/RedirectOnError.aspx", StringComparison.OrdinalIgnoreCase) < 0)
                  {
                     //Using redirect instead of writing script directly to HttpResponse because
                     //when there is a server side error during asynchronous client-side panel update, 
                     //client cannot handle the response and result in client side exception. 
                     this.Context.Session["ErrorMessage"] = HttpUtility.JavaScriptStringEncode(message, true);
                     string path = string.Format("~/FMWebApp/RedirectOnError.aspx?{0}", security.CSRFTokenWithParamName);
                     path = Context.Response.ApplyAppPathModifier(path);
                     this.Response.Redirect(path, true);
                  }
                  else
                  {
                     this.Response.StatusCode = 503;
                  }
                  this.Context.ApplicationInstance.CompleteRequest();

               }
            }
            this.Server.ClearError();
        }
    }
   
   // Called on timeout and when user clicks logout in left tree view.
   // Never call Response or Request objects when Session_End event is activated. 
   // For some reason we have to have an empty Session to call ExecuteQuery().  
   // So I gave it and empty SecurityClass object too.
   protected void Session_End(Object sender, EventArgs e)
   {
      if (this.Context != null && this.Context.Handler != null)
      {
         Type obj = this.Context.Handler.GetType();
         if (obj.FullName == "System.Web.Optimization.BundleHandler")
         {
            //No need to check bundles
            return;
         }
      }
      try
      {
         //Global.MyEventLog("Session Ended.", System.Diagnostics.EventLogEntryType.Warning);
         if (Context != null && Context.Session != null)
         {
            if (Context.Session["IsWebApp"] != null)
            {
               //Remove session only if it is created for a web application. 
               //This session variable is added after successful login (UmnyangoForm.aspx.cs).
               //Sessions created for Dispatch should skip this.			
               Logout(Context);
            }
         }
         else
         {
            ;// Global.MyEventLog("Session is null.", System.Diagnostics.EventLogEntryType.Warning);

         }
      }
      catch { };
   }

   protected void Application_End(Object sender, EventArgs e)
   {
   }

   protected void FormsAuthentication_OnAuthenticate(Object sender, FormsAuthenticationEventArgs e)
   {
      if (FormsAuthentication.CookiesSupported && Request.Cookies[FormsAuthentication.FormsCookieName] != null)
      {
         try
         {
            string userName = FormsAuthentication.Decrypt(Request.Cookies[FormsAuthentication.FormsCookieName].Value).Name;
            string roles = "FMUser";

            e.User = new System.Security.Principal.GenericPrincipal(new System.Security.Principal.GenericIdentity(userName, "Forms"), roles.Split(';'));
         }
         catch (Exception ex)
         {
            this.Context.Session["ErrorMessage"] = HttpUtility.JavaScriptStringEncode(ex.Message, true);
         }
      }
   }

   private bool IsWebApiRequest()
   {
      return HttpContext.Current.Request.AppRelativeCurrentExecutionFilePath.StartsWith(WebApiConfig.UrlPrefixRelative);
   }

   #region Web Form Designer generated code

   /// <summary>
   /// Required method for Designer support - do not modify
   /// the contents of this method with the code editor.
   /// </summary>
   private void InitializeComponent()
   {
      // 
      // Global
      // 
      this.PreRequestHandlerExecute += new System.EventHandler(this.Global_PreRequestHandlerExecute);
      this.PostRequestHandlerExecute += new System.EventHandler(this.Global_PostRequestHandlerExecute);

      this.components = new System.ComponentModel.Container();
   }

   #endregion


   #region Private methods
   /// <summary>
   /// This method will display an alert dialog along with the error
   /// message.
   /// </summary>
   /// <param name="inMessage"></param>
   private void DisplayError(string inMessage, string redirectPath = "../")
   {
      try
      {
         string message = this.StripReturns(inMessage);

         string alertString = "<script type=\"text/javascript\">\r\n<!--\r\nalert(" +
                             HttpUtility.JavaScriptStringEncode(message, true) + ");\r\n-->\r\n</script>";


         this.Response.AppendHeader("Cache-Control", "private");
         this.Response.Write(alertString);
         this.Response.Write(string.Format("<script src=\"{0}/Javascripts/cfs.js\" type=\"text/javascript\" defer></script><script type=\"text/javascript\">\r\n<!--\r\nwindow.top.location=\"{1}\";\r\n-->\r\n</script>", this.Request.ApplicationPath, redirectPath));
         this.Response.Flush();
         this.CompleteRequest();

      }
      catch (Exception ex)
      {
         WriteToEventLog(ex.Message, EventLogEntryType.Error);
      }

   }

   private void CheckCSRF()
   {
      if (ComplianceFlag == true)
      {
         return;
      }

      var security = this.Context.Session["Security"] as SecurityClass;

      string sessionCsrfToken = (security == null ?
          this.Session["CSRFToken"] as string : security.CSRFToken);
      string requestCSRFToken = this.Request.GetQueryOrFormValue("CSRFToken");

      if (!string.IsNullOrEmpty(requestCSRFToken))
      {
         string[] requestCSRFTokens = requestCSRFToken.Split(new char[] { ',' });
         if (requestCSRFTokens.Length > 0)
         {
            requestCSRFToken = requestCSRFTokens[0];
         }
      }

      //generate new CSRF Token for next response
      this.Context.Session["CSRFToken"] = SecurityClass.GenerateCSRFToken(sessionCsrfToken);


      if (string.IsNullOrEmpty(requestCSRFToken) && this.Context.Session["Security"] == null)
      {
         ////Not logged in yet. First time user visiting login page.
      }
      else if (!SecurityClass.ValidatedCSRFToken(requestCSRFToken, sessionCsrfToken))
      {
         if (!this.Request.Url.AbsolutePath.Contains("ReportViewerWebControl.axd")
             && !this.Request.Url.AbsolutePath.Contains("WebResource.axd")
             && !this.Request.Url.AbsolutePath.Contains("ScriptResource.axd")
             && !this.Request.Url.AbsolutePath.Contains("LogoutForm.aspx"))
         {
            try
            {
               if (this.Request.Url.AbsolutePath.Contains("UmnyangoForm.aspx"))
               {
                  this.Response.Redirect(Context.Response.ApplyAppPathModifier("~/FMWebApp/LogoutForm.aspx"));
               }
               else
               {
                  WriteToEventLog("Invalid Session (Possible Cross-site Request Forgery detected) - " + Request.Url.AbsolutePath, EventLogEntryType.Error);
                  this.DisplayError("Invalid Session", Context.Response.ApplyAppPathModifier("~/FMWebApp/LogoutForm.aspx"));
               }
            }
            finally
            {
               ;
            }
         }
      }
   }
   static public void Logout(HttpContext httpContext)
   {
      if (httpContext == null)
      {
         return;
      }

      System.Web.SessionState.HttpSessionState session = httpContext.Session;

      try
      {
         if (session == null)
         {
            return;
         }
         var security = session["Security"] as SecurityClass;
         if (security != null)
         {
            FMChannelHelper.MakeCall<ISites>(
                                                            x =>
                                                            x.Logout(security)
                                                        );
            session.RemoveAll();
            session.Abandon();
            httpContext.Response.Cookies.Clear();
         }
         else
         {
            string token = session["Token"] as string;
            if (!string.IsNullOrEmpty(token) && !session.IsNewSession)
            {
               FMChannelHelper.MakeCall<ISites>(
                                                               x =>
                                                               x.LogoutToken(token)
                                                           );
               session.RemoveAll();
               session.Abandon();
               httpContext.Response.Cookies.Clear();
            }
         }
      }
      finally
      {
         ;
      }
   }

   private void SetHttpResponseFilter()
   {
      try
      {
         var rndTokenStr = this.Context.Session["CSRFToken"] as string;

         var security = this.Context.Session["Security"] as SecurityClass;
         if (security != null)
         {
            rndTokenStr = security.CSRFToken;
         }
         else
         {
            rndTokenStr = SecurityClass.GenerateCSRFToken(null);
         }
         this.Context.Session["CSRFToken"] = rndTokenStr;



      }
      catch
      {
      }
   }


   private void Global_PostRequestHandlerExecute(object sender, EventArgs e)
   {
      Type obj = this.Context.Handler.GetType();
      if (obj.FullName == "System.Web.Optimization.BundleHandler")
      {
         //No need to check bundles
         return;
      }
      if (this.Context.Session != null)
      {
         if (Request.AppRelativeCurrentExecutionFilePath.IndexOf("FMWebApp/RedirectOnError.aspx", StringComparison.OrdinalIgnoreCase) < 0 &&
             !(this.Context.Handler is MvcHandler)
             )
         {
            this.Context.Session["PreviousUrl"] = this.Request.Url.PathAndQuery;
         }
         if(this.Context.Session.IsCookieless
         && this.Response.Cookies["Token"] != null)
         {
            HttpCookie cookie = this.Response.Cookies["Token"];
            this.Response.Cookies.Remove("Token");
            cookie.Name = this.Context.Session.SessionID + "_Token";
            this.Response.Cookies.Add(cookie);
         }
      }
   }

   // Called before each request.
   private void Global_PreRequestHandlerExecute(object sender, EventArgs e)
   {
#if DEBUG
      // Visual Studio has a feature call browser links which connect browsers to the visual studio debugging session.  During debugging
      // these browser links will trigger http exceptions when attempting to execute the code in this method.  So...don't execute this
      // method if we're in debug and the request is a browser link.
      Regex visualStudioBrowserLinks = new Regex("__browserLink/requestData/.*", RegexOptions.Compiled);

      Match match = visualStudioBrowserLinks.Match(HttpContext.Current.Request.Path);

      if (match.Success) return;
#endif

      if (HttpContext.Current == null || HttpContext.Current.Session == null)
         return;
      long daysLeft = 0;
      DateTime expirationDate = DateTime.MinValue;
      try
      {
         daysLeft = FMChannelHelper.MakeCall<IHardwareKey, long>(x => x.GetLicenseDaysLeftToExpire());
         expirationDate = FMChannelHelper.MakeCall<IHardwareKey, DateTime>(x => x.GetLicenseExpirationDate());
      }
      catch(Exception ex)
      {
         Exception x = ex;
         WriteToEventLog(x.Message, EventLogEntryType.Error);
         while(x.InnerException != null)
         {
            x = x.InnerException;
            WriteToEventLog(x.Message, EventLogEntryType.Error);
         }
         this.DisplayError("Error reading hardware key. See event logs for details.");
         return;
      }

      this.Context.Session["LicenseDaysLeftToExpire"] = daysLeft;
      this.Context.Session["LicenseExpirationDate"] = expirationDate;


      string token = null;
      HttpCookie cookie = null;
      Type obj = this.Context.Handler.GetType();
      if (obj.FullName == "System.Web.Optimization.BundleHandler")
      {
         //No need to check bundles
         return;
      }

      if (HttpContext.Current != null
       && HttpContext.Current.Session != null
       && this.Context.Session.IsCookieless
       && this.Request.Cookies[this.Context.Session.SessionID + "_Token"] != null)
      {
         cookie = this.Request.Cookies[this.Context.Session.SessionID + "_Token"];
         this.Request.Cookies.Remove("Token");
         cookie.Name = "Token";
         this.Request.Cookies.Add(cookie);
      }
      else
      {
         cookie = this.Request.Cookies["Token"];
      }

      if (cookie != null)
      {
         token = cookie.Value;
      }
      else if (HttpContext.Current != null && HttpContext.Current.Session != null)
      {
         token = this.Context.Session["Token"] as String;
      }
      SecurityClass security = this.Context.Session["Security"] as SecurityClass;
      if (string.IsNullOrWhiteSpace(token) == false &&
          security != null &&
          this.Request.Url.AbsolutePath.IndexOf("/MainArea/SessionInvalid/SessionInvalidIndex", StringComparison.OrdinalIgnoreCase) < 0 &&
          this.Request.Url.AbsolutePath.IndexOf("/FMWebApp/LogoutForm.aspx", StringComparison.OrdinalIgnoreCase) < 0 &&
          this.Request.Url.AbsolutePath.IndexOf("/FMWebApp/RedirectOnError.aspx", StringComparison.OrdinalIgnoreCase) < 0 &&
          this.Request.Url.AbsolutePath.IndexOf("/FMWebApp/UmnyangoForm.aspx", StringComparison.OrdinalIgnoreCase) < 0)
      {
         try
         {
            if (security.Token == Guid.Empty)
            {
               throw new FMSessionInvalidException("Invalid Token");

            }
            string securityToken = security.Token.ToString();
            if (token.NotEquals(securityToken, StringComparison.OrdinalIgnoreCase))
            {
               throw new FMSessionInvalidException("Invalid Token");
            }
         }
         catch (FMSessionInvalidException ex)
         {
            string message = ex.Message + Environment.NewLine + this.Request.RawUrl;
            WriteToEventLog(message, EventLogEntryType.Error);

            try
            {
               FMChannelHelper.MakeCall<IFMEventLog>(x => x.WriteEntry(message, FMEventLogEntryType.Error));
            }
            catch (Exception ex2)
            {
               WriteToEventLog(ex2.Message, EventLogEntryType.Error);
            }
            this.Response.Redirect(Context.Response.ApplyAppPathModifier("~/FMWebApp/LogoutForm.aspx"));
            return;
         }

      }
      // The FMBaseController needs to ignore processing when the request contains the
      // Account/Login URL until the user is logged in and a security object is created.
      // This occurs when IIS is set to Window Authentication is enabled.
      if (this.Request.Url.AbsolutePath.IndexOf("Account/Login", StringComparison.OrdinalIgnoreCase) >= 0)
      {
         this.Context.Session.Add(FMBaseController.IgnoreOnServiceAccountKey, "TRUE");
      }

      if (this.Request.Url.AbsolutePath.IndexOf("UmnyangoForm.aspx", StringComparison.OrdinalIgnoreCase) >= 0)
      {
         this.CheckCSRF();
      }

      if (token != null && !token.ToUpper().StartsWith("LOGIN FAILED"))
      {
         string pwdChanged = null;

         try
         {
            if (this.Context.Session != null)
            {
               pwdChanged = (string)this.Context.Session["PWDCHG-" + token];
            }
         }
         catch (Exception)
         {
         }

         if (pwdChanged != null)
         {
            this.Context.Session.Remove("PWDCHG-" + token);
         }
         else
         {
            pwdChanged = "FALSE";
         }
         if ((this.Request.Url.AbsolutePath.IndexOf("UmnyangoForm.aspx", StringComparison.OrdinalIgnoreCase) < 0)
             && (Request.Url.AbsolutePath.IndexOf("LogoutForm.aspx", StringComparison.OrdinalIgnoreCase) < 0)
             && (Request.Url.AbsolutePath.IndexOf("LicenseExpiredForm.aspx", StringComparison.OrdinalIgnoreCase) < 0)
             && (Request.Url.AbsolutePath.IndexOf("RedirectOnError.aspx", StringComparison.OrdinalIgnoreCase) < 0)
             && (this.Request.AppRelativeCurrentExecutionFilePath.IndexOf("Operate/UpdateValues", StringComparison.OrdinalIgnoreCase) < 0)
             && (this.Request.AppRelativeCurrentExecutionFilePath.IndexOf("AlarmSummary2/AlarmNotifications", StringComparison.OrdinalIgnoreCase) < 0)
             && (this.Request.AppRelativeCurrentExecutionFilePath.IndexOf("AlarmSummary2/SyncUnresolvedConflictsCount", StringComparison.OrdinalIgnoreCase) < 0)
             && (this.Request.AppRelativeCurrentExecutionFilePath.IndexOf("AlarmSummary2/ControllerPingMechanism", StringComparison.OrdinalIgnoreCase) < 0)
             && (this.Request.AppRelativeCurrentExecutionFilePath.IndexOf("AlarmSummary2/AlarmNotificationsForMenu", StringComparison.OrdinalIgnoreCase) < 0)

             )
         {
            //bool licenseExpired = FMChannelHelper.MakeCall<IHardwareKey, bool>(x => x.GetLicenseExpired());
            bool licenseNotExpiredAtLogin = (this.Session["LicenseNotExpiredAtLogin"] as string != null && this.Session["LicenseNotExpiredAtLogin"] as string == "true");
            if (licenseNotExpiredAtLogin == false)
            {
               bool licenseExpired = FMChannelHelper.MakeCall<IHardwareKey, bool>(x => x.GetLicenseExpired());
               if (licenseExpired == true)
               {
                  Context.ApplicationInstance.CompleteRequest();
                  this.Response.Redirect(Context.Response.ApplyAppPathModifier("~/FMWebApp/LicenseExpiredForm.aspx"));
               }
            }
         }

         // If not one of these forms, we do create a SecurityClass object.
         if ((this.Request.Url.AbsolutePath.IndexOf("UmnyangoForm.aspx", StringComparison.OrdinalIgnoreCase) < 0)
            && (Request.Url.AbsolutePath.IndexOf("LicenseExpiredForm.aspx", StringComparison.OrdinalIgnoreCase) < 0)
            && (Request.Url.AbsolutePath.IndexOf("default.aspx", StringComparison.OrdinalIgnoreCase) < 0)
            && (this.Request.Url.AbsolutePath.IndexOf("LogoutForm.aspx", StringComparison.OrdinalIgnoreCase) < 0)
            && (this.Request.Url.AbsolutePath.IndexOf("ForgotPasswordForm.aspx", StringComparison.OrdinalIgnoreCase) < 0)
            && (this.Request.Url.AbsolutePath.IndexOf("FuelsManagerForm.aspx", StringComparison.OrdinalIgnoreCase) < 0)
            && (this.Request.RawUrl.IndexOf("DisplayImage.ashx") < 0)
            && (this.Request.RawUrl.IndexOf("/bundles/") < 0)
            && (this.Request.RawUrl.IndexOf("/Content/") < 0)
            && (this.Request.Url.AbsolutePath.IndexOf("/MainArea/SessionInvalid/SessionInvalidIndex", StringComparison.OrdinalIgnoreCase) < 0)
            && (this.Request.AppRelativeCurrentExecutionFilePath.IndexOf("Operate/UpdateValues", StringComparison.OrdinalIgnoreCase) < 0)
            && (this.Request.AppRelativeCurrentExecutionFilePath.IndexOf("AlarmSummary2/AlarmNotifications", StringComparison.OrdinalIgnoreCase) < 0)
            && (this.Request.AppRelativeCurrentExecutionFilePath.IndexOf("AlarmSummary2/SyncUnresolvedConflictsCount", StringComparison.OrdinalIgnoreCase) < 0)
            && (this.Request.AppRelativeCurrentExecutionFilePath.IndexOf("AlarmSummary2/ControllerPingMechanism", StringComparison.OrdinalIgnoreCase) < 0)
            && (this.Request.AppRelativeCurrentExecutionFilePath.IndexOf("AlarmSummary2/AlarmNotificationsForMenu", StringComparison.OrdinalIgnoreCase) < 0)
            && this.Context.Session != null)
         {
            try
            {
               // This should be the only call to GetSecurity.  All others should
               // say:	Security = (SecurityClass) Session["Security"];
               // SecurityClass security = sites.GetSecurity(token);
               // We now get security from the session here too.  We set security during login
               // process.
               // If, however, the request comes from Dispatch, the token and user password will
               // be stored in cookies and the security object will be null.  We need to create 
               // a security object in that case.
               if (security == null)
               {
                  if (this.Context.Session["IsWebApp"] == null && string.IsNullOrWhiteSpace(token) == false)
                  {
                     //May mean request is from dispatch
                     try
                     {
                        security = FMChannelHelper.MakeCall<ISites, SecurityClass>(sitesChannel =>
                        {
                           return sitesChannel.GetSecurity(token);
                        });

                        security.SkipSessionTimeUpdate = false;
                        this.Context.Session["Security"] = security;
                        this.Context.Session["Token"] = token;
                     }
                     catch(FMSessionInvalidException ex)
                     {
                        this.Context.Session.Clear();
                        WriteToEventLog(ex.Message + Environment.NewLine +this.Request.RawUrl, EventLogEntryType.Error);
                        return;
                     }
                     catch ( Exception ex)
                     {
                        this.Context.Session.Clear();
                        WriteToEventLog(ex.Message + Environment.NewLine + this.Request.RawUrl, EventLogEntryType.Error);
                        return;
                     }
                  }
                  else
                  {
                     this.DisplayError("Invalid Session", Context.Response.ApplyAppPathModifier("~/FMWebApp/LogoutForm.aspx"));
                     return;
                  }

               }
               else if (security.Token != Guid.Empty)
               {
                  security.SkipSessionTimeUpdate = false;
                  FMChannelHelper.MakeCall<ISessions>(x => x.PingSession(security));
               }

               var requestContext = FMWebAPIServiceLocator.GetInstance<ICurrentRequestContext>();
               requestContext.SetCurrentSecurityContext(security);

               if (this.IsAjaxCallback() == false && !(this.Context.Handler is MvcHandler))
               {
                  this.CheckCSRF();
               }

               // New code for "Change Log".  Populate the Security object with the values
               // for these new fields.  Yes, GetHostByAddress() is obsolete, but, depending on 
               // the server's setting, GetHostEntry() will return the IP address, not the client
               // machine name.
               // 
               // Post hoc - As it turns out, using NetBIOS for reverse DNS lookup has its own issues,
               // So, we just rely on the IP address, which meets the requirement.  Tasks 1873 and 1906.
               // System.Net.IPHostEntry host = System.Net.Dns.GetHostByAddress(Request.ServerVariables["REMOTE_HOST"]);
               string[] saDomainAndUser = this.Request.LogonUserIdentity.Name.Split('\\');

               security.ASPSessionID = this.Context.Session.SessionID;
               security.ClientDomain = (0 < saDomainAndUser.Length) ? saDomainAndUser[0] : "Unknown";
               security.ClientUserName = (1 < saDomainAndUser.Length) ? saDomainAndUser[1] : "Unknown";

               security.ClientIpAddress = Request.UserHostAddress;
               if (security.ClientIpAddress != null)
                  security.ClientIpAddress = GetSessionsIP4Address(security.ClientIpAddress);
               else
                  security.ClientIpAddress = "";

               if (security.ClientIpAddress == this.Request.UserHostName)
               {
                  security.Workstation = security.ClientIpAddress;
               }
               else
               {
                  security.Workstation = this.Request.UserHostName;  // Gets DNS name of remote client (but usually returns the IP address).
               }

               security.WebServerIpAddress = this.Request.ServerVariables["LOCAL_ADDR"];
               if (security.WebServerIpAddress != null)
                  security.WebServerIpAddress = GetSessionsIP4Address(security.WebServerIpAddress);
               else
                  security.WebServerIpAddress = "";


               this.Context.Session["Security"] = security;
            }
            catch (SqlException ex)
            {
               string message = ex.Message;
               WriteToEventLog(message, EventLogEntryType.Error);

               if (message.StartsWith("General network error") == true)
               {
                  this.DisplayError("SQL Server does not exist or access is denied.");
               }
               return;
            }
            catch (HttpRequestValidationException except)
            {
               WriteToEventLog(except.Message, EventLogEntryType.Error);

               try
               {
                  FMChannelHelper.MakeCall<IFMEventLog>(x => x.WriteEntry(except.ToString(), FMEventLogEntryType.Error));
               }
               catch (Exception ex2)
               {
                  WriteToEventLog(ex2.Message, EventLogEntryType.Error);
               }
               this.DisplayError(except.Message);
            }
            catch (FMSessionInvalidException ex)
            {
               string message = ex.Message;
               WriteToEventLog(message, EventLogEntryType.Error);

               try
               {
                  FMChannelHelper.MakeCall<IFMEventLog>(x => x.WriteEntry(message, FMEventLogEntryType.Error));
               }
               catch (Exception ex2)
               {
                  WriteToEventLog(ex2.Message, EventLogEntryType.Error);
               }

               if (this.Context != null)
               {
                  Logout(this.Context);
                  if (message == FMSessionInvalidException.SessionTimedOutExceptionMessage)
                  {
                     Response.Redirect(Context.Response.ApplyAppPathModifier("~/FMWebApp/LogoutForm.aspx?SessionTimedOut=true"));
                  }
                  else
                  {
                     Response.Redirect(Context.Response.ApplyAppPathModifier("~/FMWebApp/LogoutForm.aspx?InvalidSession=true"));

                  }
                  this.Context.ApplicationInstance.CompleteRequest();
               }

               return;
            }
            catch (Exception ex)
            {
               string message = ex.Message;
               WriteToEventLog(message, EventLogEntryType.Error);
               try
               {
                  FMChannelHelper.MakeCall<IFMEventLog>(x => x.WriteEntry(message, FMEventLogEntryType.Error));
               }
               catch (Exception ex2)
               {
                  WriteToEventLog(ex2.Message, EventLogEntryType.Error);
               }
               if (message == FMSessionInvalidException.SessionNotFoundExceptionMessage
                  || message == FMSessionInvalidException.SessionTimedOutExceptionMessage)
               {

                  if (this.Context != null)
                  {
                     WriteToEventLog("Trying to redirect to logout form", EventLogEntryType.Error);
                     Logout(this.Context);
                     if (message == FMSessionInvalidException.SessionTimedOutExceptionMessage)
                     {
                        Response.Redirect(Context.Response.ApplyAppPathModifier("~/FMWebApp/LogoutForm.aspx?SessionTimedOut=true"));
                     }
                     else
                     {
                        Response.Redirect(Context.Response.ApplyAppPathModifier("~/FMWebApp/LogoutForm.aspx?InvalidSession=true"));

                     }
                     this.Context.ApplicationInstance.CompleteRequest();

                  }
             
               }


               return;
            }
         }
         else
         {
            if (this.Request.Url.AbsolutePath.IndexOf("LicenseExpiredForm.aspx", StringComparison.OrdinalIgnoreCase) > 0)
            {

            }
            else if (this.Context.Handler is MvcHandler
                    && this.Context.Session != null)
            {
               if (this.Request.AppRelativeCurrentExecutionFilePath.IndexOf("Operate/UpdateValues", StringComparison.OrdinalIgnoreCase) >= 0 ||
                   this.Request.AppRelativeCurrentExecutionFilePath.IndexOf("AlarmSummary2/AlarmNotifications", StringComparison.OrdinalIgnoreCase) >= 0 ||
                   this.Request.AppRelativeCurrentExecutionFilePath.IndexOf("AlarmSummary2/ControllerPingMechanism", StringComparison.OrdinalIgnoreCase) >= 0 ||
                   this.Request.AppRelativeCurrentExecutionFilePath.IndexOf("AlarmSummary2/SyncUnresolvedConflictsCount", StringComparison.OrdinalIgnoreCase) >= 0 ||
                   this.Request.AppRelativeCurrentExecutionFilePath.IndexOf("AlarmSummary2/AlarmNotificationsForMenu", StringComparison.OrdinalIgnoreCase) >= 0 ||
                   this.Request.AppRelativeCurrentExecutionFilePath.IndexOf("Operate/SetOperateActive", StringComparison.OrdinalIgnoreCase) >= 0)
               {
                  if (security == null)
                  {
                     //Session must have terminated (due to user logging out, idle session, or exception)
                     //WriteToEventLog(string.Format("Code 403: Missing security object while handling {0}.", this.Request.AppRelativeCurrentExecutionFilePath), EventLogEntryType.Warning);
                     this.Response.Cookies.Clear();
                     this.Response.StatusCode = 403;
                     this.Context.ApplicationInstance.CompleteRequest();
                     return;
                  }
                  security.SkipSessionTimeUpdate = true;
               }
            }
         }
      }
      else
      {
         if (this.Request.Url.AbsolutePath.IndexOf("UmnyangoForm.aspx", StringComparison.OrdinalIgnoreCase) < 0
             && this.Request.Url.AbsolutePath.IndexOf("LogoutForm.aspx", StringComparison.OrdinalIgnoreCase) < 0)
         {
            FMChannelHelper.MakeCall<IFMEventLog>(fmEventLogChannel => { fmEventLogChannel.WriteEntry("Invalid Page Request - " + this.Request.RawUrl, FMEventLogEntryType.Error); });
         }
      }
      this.SetHttpResponseFilter();
      if(Context.Session != null && Context.Session["Security"] != null && Context.Session["Token"] != null)
      {
         security = Context.Session["Security"] as SecurityClass;
         if (security != null && security.UserGuid != Guid.Empty && Context.Session["Accessibility"] == null) 
         {
            var ac = new UserAccessibilityDO(security, security.UserGuid);
            this.Session["Accessibility"] = ac;
         }
      }
   }
   private bool IsAjaxCallback()
   {
      return string.Equals(
          "XMLHttpRequest",
          this.Context.Request.Headers["x-requested-with"],
          StringComparison.OrdinalIgnoreCase);
   }

   // Task 3589.  Returns an IPv4 address whether IPv6 is running or not.
   static private string GetSessionsIP4Address(string sUserHostAddress)
   {
      string sIP4Address = String.Empty;

      try
      {
         foreach (IPAddress IPA in Dns.GetHostAddresses(sUserHostAddress))
         {
            if (IPA.AddressFamily.ToString() == "InterNetwork")
            {
               sIP4Address = IPA.ToString();
               break;
            }
         }

         if (sIP4Address != String.Empty)
         {
            return sIP4Address;
         }

         foreach (IPAddress IPA in Dns.GetHostAddresses(Dns.GetHostName()))
         {
            if (IPA.AddressFamily.ToString() == "InterNetwork")
            {
               sIP4Address = IPA.ToString();
               break;
            }
         }
      }
      catch (ArgumentNullException e)                 // hostNameOrAddress is null.
      {
         FMChannelHelper.MakeCall<IFMEventLog>(fmEventLogChannel => { fmEventLogChannel.WriteEntry(e.ToString(), FMEventLogEntryType.Error); });
      }
      catch (ArgumentOutOfRangeException e)           //  The length of hostNameOrAddress is greater than 126 characters.
      {
         FMChannelHelper.MakeCall<IFMEventLog>(fmEventLogChannel => { fmEventLogChannel.WriteEntry(e.ToString(), FMEventLogEntryType.Error); });
      }
      catch (ArgumentException e)                     //  hostNameOrAddress is an invalid IP address.
      {
         FMChannelHelper.MakeCall<IFMEventLog>(fmEventLogChannel => { fmEventLogChannel.WriteEntry(e.ToString(), FMEventLogEntryType.Error); });
      }
      catch (SocketException e)           //  No such host is known (not running IPv6, but sent an IPv6 address?).
      {
         FMChannelHelper.MakeCall<IFMEventLog>(fmEventLogChannel => { fmEventLogChannel.WriteEntry(e.ToString(), FMEventLogEntryType.Error); });
      }

      return sIP4Address;
   }

   /// <summary>
   /// This method will strip out all carriage returns and line feeds from
   /// the string. It will return the string without carriage returns and
   /// line feeds.
   /// </summary>
   /// <param name="inStr"></param>
   /// <returns></returns>
   private string StripReturns(string inStr)
   {
      string outStr = inStr;
      char carriageReturn = '\r';
      char lineFeed = '\n';

      if ((inStr != null) && (inStr.Length > 0))
      {
         int index = -1;
         bool containsCarriageReturns = true;
         bool containsLineFeeds = true;

         while (containsCarriageReturns == true)
         {
            index = outStr.IndexOf(carriageReturn);

            if (index > -1)
            {
               outStr = outStr.Remove(index, 1);
            }
            else
            {
               containsCarriageReturns = false;
            }
         }

         while (containsLineFeeds == true)
         {
            index = outStr.IndexOf(lineFeed);

            if (index > -1)
            {
               outStr = outStr.Remove(index, 1);
            }
            else
            {
               containsLineFeeds = false;
            }
         }
      }

      return outStr;
   }
   #endregion
}
