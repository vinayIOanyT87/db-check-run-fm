// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UmnyangoFormBase.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the UmnyangoFormBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FuelsManager.FMWebApp
{
	using System;
    using System.Collections.Generic;
    using System.Configuration;
	using System.Diagnostics;
    using System.Globalization;
	using System.ServiceModel;
	using System.Web;
    using System.Web.Security;
    using System.Web.SessionState;
    using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.Constants;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.Exceptions;
	using FMBusinessObjects.ServiceRequests;

    /// <summary>
    ///    A base class for forms that handle logging in to the system, namely,
    ///    UmnyangoForm and ChangePasswordForm. The methods in this class are methods
    ///    that both forms need to call.
    /// </summary>
	public abstract class UmnyangoFormBase : FMFormBase
	{
		#region Methods

		/// <summary>
		/// Default implementation for this method. UmnyangoForm should override to provide the password hint.
		/// </summary>
		/// <param name="loginRequest">Information about login credentials</param>
		/// <param name="mySite">site</param>
		/// <param name="result">The result.</param>
		protected virtual void HandlePasswordHintAndForgotPasswordDiv(SecurityLoginRequest loginRequest, SiteClass mySite, string result)
		{
			this.ErrorHandler(new Exception(result));
		}

		/// <summary>
		///    Attempt to log in to FuelsManager with the given credentials. Will take the
		///    appropriate action based on the result, e.g., show an error message, go
		///    to the main page, go to the change password page. Note that exceptions must
		///    be handled by caller.
		/// </summary>
		/// <param name="userID">entered user id</param>
		/// <param name="password">entered password</param>
		/// <param name="siteID">entered site id</param>
		/// <param name="forcePasswordChange">flag to indicate if we are going to change the password</param>
        /// <param name="passthroughAuthorisation">If true, then AD user has already been authenticated by Windows and does not need to be authenticated by Active Directory again</param>
		protected void Login(string userID, string password, string siteID, bool forcePasswordChange, bool passthroughAuthorisation)
		{
            SecurityClass security = new SecurityClass { SiteGuid = Guids.SiteAdminGuid };
            security.AddRight(RIGHT.VIEW_SITES_AND_SITE_GROUPS);
            security.AddRight(RIGHT.MODIFY_SITES_AND_SITE_GROUPS);
            security.AddRight(RIGHT.VIEW_USERS);
            security.AddRight(RIGHT.MODIFY_USERS);
            security.AddRight(RIGHT.VIEW_USER_GROUPS);
            security.AddRight(RIGHT.MODIFY_USER_GROUPS);
            security.AddRight(RIGHT.VIEW_EQUIPMENT_DATA);
            security.AddRight(RIGHT.MODIFY_EQUIPMENT_DATA);
            security.AddRight(RIGHT.MODIFY_SYSTEM_SETTINGS);

            bool isDomainUser = userID.Contains("\\");

            // Get the single sign on mode flag.
            bool ssoMode = this.IsSsoMode(security);

            if (ssoMode && isDomainUser)
            {
                string message;
                var userSite = FMChannelHelper.MakeCall<ISites, SiteClass>(x => x.GetByID(security, siteID, false));

                if (userSite == null || userSite.SiteGuid == Guid.Empty)
                {
                    message = "Invalid user credentials. Could not find user's site:" + siteID;
                    Global.WriteToEventLog(message, EventLogEntryType.Error);
                    throw new Exception(message);
                }

                security.SiteGuid = userSite.SiteGuid;
                var user = FMChannelHelper.MakeCall<IUsers, UserClass>(x => x.GetByID(security, userID));

                if (user == null || user.IdentityGuid == Guid.Empty || !user.ActiveDirectoryUser)
                {
                    message = "Invalid user credentials. Could not find user:" + userID + " Site ID " + siteID;
                    Global.WriteToEventLog(message, EventLogEntryType.Error);

                    throw new Exception(message);
                }

                if (!passthroughAuthorisation)
                {
                    var authenticated = FMChannelHelper.MakeCall<IActiveDirectoryService, bool>(x => x.AuthenticateUser(userID, password));
                    if (authenticated == false)
                    {
                        message = "Could not authenticate domain user: " + userID + ". UserID in FuelsManager is " + user.ID;
                        Global.WriteToEventLog(message, EventLogEntryType.Error);
                        throw new Exception(message);
                    }

                    password = user.Password;
                }
            }

			bool cacEnable = false;
            string cacEnableString = FMChannelHelper.MakeCall<IConfigurationSettings, string>(x => x.GetKeyValueByKey(this.Security, ConfigurationSettingDOClass.Key_CAC_Enable));


            if (string.IsNullOrWhiteSpace( cacEnableString ) == false && cacEnableString.Trim() == "1")
            {
                HttpClientCertificate cs = this.Request.ClientCertificate;
                if (1 == cs.Flags)
                {
                    string strCn = cs.Subject;
                    string strCnFromHeader = this.Request.Headers["X-SSL-Client-Cert-Subject"];

                    // Load Balancer is appending client information to web request headers; use those if available
                    if (Global.IsFdsIM && string.IsNullOrWhiteSpace(strCnFromHeader) == false)
                    {

                        string message = string.Format("Request header X-SSL-Client-Cert-Subject detected. Value = {0}", strCnFromHeader);
                        Global.WriteToEventLog(message, EventLogEntryType.Information);

                        
                        //Verify certificate belongs to load balancer by checking the certificate subject against ones listed in confguration settings.

                        if (string.IsNullOrWhiteSpace(Global.LoadBalancerCertSubjectString) == false)
                        {

                            string[] loadBalancerCertSubjects = Global.LoadBalancerCertSubjectString.Split(new char[] { ';', '\n','\r' });
                            if (loadBalancerCertSubjects.Length > 0)
                            {
                                strCn = strCn.Trim().Replace(" ", string.Empty).ToUpper();
                                string allLoadBalancerSubjects = string.Empty;
                                string allLoadBalancerSubjectsTrimmed = string.Empty;

                                foreach (var loadBalancerCertSubject in loadBalancerCertSubjects)
                                {
                                    allLoadBalancerSubjects += string.Format("'{0}' ,", loadBalancerCertSubject);
                                    string loadBalancerCertSubjectStr = loadBalancerCertSubject.Trim().Replace(" ", string.Empty).ToUpper();
                                    allLoadBalancerSubjectsTrimmed += string.Format("'{0}' ,", loadBalancerCertSubjectStr);
                                    if (loadBalancerCertSubjectStr == strCn)
                                    {
                                        strCn = strCnFromHeader;
                                        cacEnable = true;
                                        break;
                                    }
                                }
                                if (!cacEnable)
                                {
                                    message = string.Format("Could not verify load balancer certificate.\r\nApplication is CAC enabled.\r\n Received request header [X-SSL-Client-Cert-Subject]='{0}'  '{1}'.\r\n Certificate Subject='{2}' '{3}'.\r\nAppSetting LoadBalancerCertificateSubject={4}  {5}.",
                                                            strCnFromHeader, strCn, cs.Subject, strCn, allLoadBalancerSubjects, allLoadBalancerSubjectsTrimmed);
                                    Global.WriteToEventLog(message, EventLogEntryType.Warning);

                                }
                            }
                        }
                        else
                        {
                            Global.WriteToEventLog("Load balancer certificate subject string not found in configuration file.", EventLogEntryType.Information);
                        }
                    }
                    else
                    {
                        cacEnable = true;
                    }
                    if (cacEnable)
                    {
                        if (strCn.IndexOf("CN=", StringComparison.Ordinal) != -1)
                        {
                            strCn = strCn.Substring(strCn.IndexOf("CN=", StringComparison.Ordinal) + 3);
                        }
                        int iIndex = strCn.IndexOf(',');
                        if (iIndex >= 0) strCn = strCn.Remove(iIndex);
                        strCn = strCn.Trim().Replace(",", ""); // remove comma from CAC login
                        userID = strCn.Trim().Replace("'", ""); // remove apostrophes from CAC login
                        string message = userID + " is attempting CAC login with client certificate.";
                        Global.WriteToEventLog(message, EventLogEntryType.Information);
                    }
                }
            }

			// if ForcePasswordChange is set to true then the user has pressed the change password button
			// treat this like the password is expired
			bool changePassword = false;
			int daysUntilExpiration = 0;
            var loginRequest = new SecurityLoginRequest
            {
                CACEnabled = cacEnable,
                UserID = userID,
                Password = password,
                SiteID = siteID,
                TimeOut = GetSessionTimeout()
            };

            string result = null;
            SecurityLoginResponse loginResponse;

            try
            {
                loginResponse =
                    FMChannelHelper.MakeCall<ISites, SecurityLoginResponse>(
                        x => x.Login2(loginRequest));
            }
            catch (Exception loginEx)
            {
					if (loginEx.Message == "Site Not Found."
					&& forcePasswordChange)
					{
						loginResponse = new SecurityLoginResponse() { Security = security, Result = "Login Failed" };
					}
					else
					{
						throw new Exception("Login Failed", loginEx);
					}
				}

            if (loginResponse != null)
            {
					if ((loginResponse.Result == "User locked out"
					|| loginResponse.Result == "Login Failed")
					&& forcePasswordChange)
					{
						result = "Please provide your credentials to change password";
					}
					else
					{
						result = loginResponse.Result;
					}
					security = loginResponse.Security;
					changePassword = loginResponse.ChangePassword;
					daysUntilExpiration = loginResponse.DaysUntilExpiration;
            }

            // For invalid logins, the app must update the User table with the number of invalid
            // attempts. Therefore, in order to persist the update to the user table an exception
            // cannot be throw so the return value is set to error message which starts is "User".
            if ((result != null)
				&& (result.StartsWith("User")
				|| result.ToUpper().StartsWith("LOGIN FAILED")
				|| result.ToUpper().StartsWith("PLEASE PROVIDE")
            || result.ToUpper().StartsWith("CORRUPTED")))
            {
                SiteClass mySite = FMChannelHelper.MakeCall<ISites, SiteClass>(
                                                                        x =>
                                                                        x.GetByID(security, loginRequest.SiteID, true)
                                                                );

                if (mySite.EnablePasswordHint || mySite.EnablePasswordReset)
                {
                    this.HandlePasswordHintAndForgotPasswordDiv(loginRequest, mySite, result);
                }
                else
                {
                    this.ErrorHandler(new Exception(result));
                }
            }
            else
            {
                string token = security.Token.ToString();
                this.Session.Add("Security", security);
                this.Session.Add("Token", token);
                this.Session.Add("ResetTabularViewSessionOperation", "UserLogin");
                this.Session.Remove("Result");
                this.Session["CSRFToken"] = security.CSRFToken;//generate new one for next response;
                this.Session["IsWebApp"] = "true";
                bool licenseExpired = FMChannelHelper.MakeCall<IHardwareKey, bool>(x => x.GetLicenseExpired());
                if (licenseExpired == false)
                {
                    this.Session["LicenseNotExpiredAtLogin"] = "true";
                }
                string newSessionId = RegenerateAspNetSessionID(security);

                Response.Cookies.Add(new HttpCookie("Token", token));

                string postLoginUrl = "FuelsManagerForm.aspx";
                //if (security.HasRight(RIGHT.ACCESS_ADMIN_DASHBOARD))
                //{
                //	postLoginUrl = "../FMEntityImportWebApp/SynchronizationDashboard.aspx";
                //}

                string postLogin = FMChannelHelper.MakeCall<IConfigurationSettings, string>(x => x.GetKeyValueByKey(security, "PostLoginURL"));
                if (!string.IsNullOrEmpty(postLogin))
                {
                    postLoginUrl = postLogin;
                }

                postLoginUrl = ResolveUrl(postLoginUrl);


                //FormsAuthentication.SetAuthCookie(security.UserID, false);

                // SJiang: Do not check if user login using CAC
                if (!cacEnable && !isDomainUser && changePassword)
                {
                    this.Session.Add("PWDCHG-" + security.Token, "TRUE");

                    // Display forced change notice before forwarding user to the change password page.  CHK 2009-09-28
                    this.Response.Write(string.Format(
                        "<script type=\"text/javascript\">\r\n<!--\r\nalert(\"Your password is either new or expired, and must be changed.\");window.location=\"ChangePasswordForm.aspx?{0}\";\r\n-->\r\n</script>", security.CSRFTokenWithParamName));
                }
                else if (!cacEnable && !isDomainUser && forcePasswordChange)
                {
                    this.Session.Add("PWDCHG-" + security.Token.ToString(), "TRUE");
                    this.Response.Write(
								"<script type=\"text/javascript\">\r\n<!--\r\nwindow.location=\"ChangePasswordForm.aspx?CSRFToken=" + security.CSRFToken + "\";\r\n-->\r\n</script>");
                }
                else
                {
                    // Display expiration warning to user if within 7 days. (IGO 2009-Aug-11)
                    // SJiang: Do not check if user login using CAC
                    if (!cacEnable && !isDomainUser && daysUntilExpiration <= 7)
                    {
                        // Get user confirmation if the transaction is associated to other transactions.
                        string message = "Your Password will expire in " + daysUntilExpiration.ToString(CultureInfo.InvariantCulture)
                                                + " days. Click OK to change your Password now, or Cancel to continue.";

                        // All this fancy scripting is just to POST the CSRF token rather than GET
                        // so that it doesn't show up on the URL address bar on the browser.
                        this.Page.ClientScript.RegisterStartupScript(this.GetType(), "CHANGEPWD_CONFIRMATION",
                                    "<script language='JavaScript'>\n\r" +
                                        "<!--\n\r" +
                                        "if (confirm('" + this.Page.Server.HtmlEncode(message) + "'))\n\r" +
                                        "	window.location='ChangePasswordForm.aspx?" + security.CSRFTokenWithParamName + "';\n\r" +
                                        "else{\n\r" +
                                        "	window.top.document.writeln('<'+'html><'+'body><'+ 'form method=\"post\" action=\"" + postLoginUrl + "\" >' +\n\r" +
                                        "	'<' + 'input type=\"hidden\" name=\"CSRFToken\" value=\"" + security.CSRFToken + "\"/>' +\n\r" +
                                        "	'<' + '/form><' + 'script>document.forms[0].submit();<' + '/script><' + '/body><' + '/html>');\n\r" +
                                        "}\n\r" +
                                        "-->\n\r" +
                                        "</script>");
                    }
                    else
                    {
                        this.CheckLogSizes(security);
                        
                        // Want menu info loaded from scratch
                        this.Session.Remove(PageSessionKeyConstants.FM_MENU_DATA);
                        // All this fancy scripting is just to POST the CSRF token rather than GET
                        // so that it doesn't show up on the URL address bar on the browser.
                        this.Response.Write("<script language='JavaScript'>\n\r" +
                                            "<!--\n\r" +
                                            "window.top.document.writeln('<'+'html><'+'body><'+ 'form method=\"post\"  action=\"" + postLoginUrl + "\" >' +\n\r" +
                                            "'<' + 'input type=\"hidden\" name=\"CSRFToken\" value=\"" + security.CSRFToken + "\" />' + \n\r" +
                                            "'<' + '/form><' + 'script>document.forms[0].submit();<' + '/script><' + '/body><' + '/html>');\n\r" +
                                            "-->\n\r" +
                                            "</script>");
                    }
                }
            }
		}

        /// <summary>
		/// This method gets the SSO mode.
		/// </summary>
		/// <param name="security">The security object.</param>
		/// <returns>Returns true if in SSO mode, otherwise false.</returns>
		private bool IsSsoMode(SecurityClass security)
        {
            bool isSsoMode = false;

            try
            {
                var configSetting = FMChannelHelper.MakeCall<IConfigurationSettings, ConfigurationSettingDOClass>
                                                        (x => x.GetByKey(security, ConfigurationSettingDOClass.Key_SingleSignOnMode));

                if (configSetting != null && string.IsNullOrEmpty(configSetting.SettingValue) == false && configSetting.SettingValue == "1")
                {
                    isSsoMode = true;
                }
            }
            catch (Exception)
            {
                return isSsoMode;
            }

            return isSsoMode;
        }

        /// <summary>
        /// Checks the log sizes.
        /// </summary>
        /// <param name="security">The security.</param>
        private void CheckLogSizes(SecurityClass security)
		{
			// Check the log sizes if necessary
			int? sizeLimitValue = null;
			int? thresholdLimitValue = null;
			int? shutdownIfThresholdExceededValue = null;

			FMChannelHelper.MakeCall<IConfigurationSettings>(
				x =>
				{
					ConfigurationSettingDOClass sizeLimitSetting = x.GetByKey(security, "MaximumNumberOfRowsForLogs");
					ConfigurationSettingDOClass thresholdLimitSetting = x.GetByKey(security, "ThreshholdPercentageForLogs");
					ConfigurationSettingDOClass shutdownIfThresholdExceededSetting = x.GetByKey(
						security,
						"ShutdownIfThresholdExceededForLogs");

					sizeLimitValue = sizeLimitSetting.GetIntegerValue();
					thresholdLimitValue = thresholdLimitSetting.GetIntegerValue();
					shutdownIfThresholdExceededValue = shutdownIfThresholdExceededSetting.GetIntegerValue();
				});


			int sizeLimit = sizeLimitValue.GetValueOrDefault(0);
			int thresholdLimit = thresholdLimitValue.GetValueOrDefault(0);

			if (sizeLimit > 0 && thresholdLimit > 0)
			{
				bool shutdownIfThresholdExceeded = Convert.ToBoolean(shutdownIfThresholdExceededValue.GetValueOrDefault(0));

				try
				{
					FMChannelHelper.MakeCall<IAlarmAndEventLogs>(alarmAndEventsChannel => alarmAndEventsChannel.CheckLogSize(security, sizeLimit, thresholdLimit));
				}
				catch (FaultException<FMRowCountThresholdException> except)
				{
					// If ShutdownIfThresholdExceededForLogs config value is set to true
					// then create an FMFatalErrorException to shut down FuelsManager
					Exception thresholdException;
					if (shutdownIfThresholdExceeded)
					{
						thresholdException = new FMFatalErrorException(-1, except.Detail.Message);
					}
					else
					{
						thresholdException = except;
					}
					this.ErrorHandler(thresholdException);
				}
				catch (Exception e)
				{
					Debug.WriteLine(e.ToString());
				}
			}
		}
		#endregion

        public string RegenerateAspNetSessionID(SecurityClass security)
        {
            string newSessionId = string.Empty;

            if (this.Session != null)
            {
                Guid token = security.Token;
                newSessionId = Session.SessionID;

                if (this.Session.IsCookieless)
                {
                    //temporary fix until a solution is found for cookiless version that would generate new session id when there is successful login.
                    return newSessionId;

                }

                Response.Cookies.Clear();
                Request.Cookies.Clear();
                this.Context.ApplicationInstance.CompleteRequest();

                SessionStateItemCollection stateItems = new SessionStateItemCollection(); 
                HttpSessionStateContainer stateProvider = (HttpSessionStateContainer)(SessionStateUtility.GetHttpSessionStateFromContext(this.Context));

                foreach (string key in Session.Keys)
                {
                    stateItems[key] = Session[key]; 
                }
                stateItems["CreatedDateTime"] = DateTime.UtcNow;

                //RaiseSessionEnd removes current session record from tblSessions table using the token.
                //To keep the entry in tblSessions table, set token to empty and then restore after RaiseSessionEnd call.
                security.Token = Guid.Empty;

                SessionStateUtility.RaiseSessionEnd(stateProvider, this, EventArgs.Empty);
                SessionStateUtility.RemoveHttpSessionStateFromContext(this.Context);

                this.Session.Abandon();

                SessionIDManager manager = new SessionIDManager();
                newSessionId = manager.CreateSessionID(this.Context);

               lock (Global.lockNewSessionStates)
               {
                  Dictionary<string, SessionStateItemCollection> newSessionStateItems = AppDomain.CurrentDomain.GetData("NewSessionStateItems") as Dictionary<string, SessionStateItemCollection>;
                  if (newSessionStateItems != null)
                  {
                     newSessionStateItems[newSessionId] = stateItems;
                  }
               }

                bool redirectedFlag;
                bool cookieAddedFlag;
                manager.SaveSessionID(this.Context, newSessionId, out redirectedFlag, out cookieAddedFlag);

                security.Token = token;
            }
            return newSessionId;
        }
    }
}