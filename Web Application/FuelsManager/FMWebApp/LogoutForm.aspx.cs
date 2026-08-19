// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LogoutForm.aspx.cs" company="Varec, Inc.">
//	Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//	Defines the LogoutForm type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FuelsManager.FMWebApp
{
	using System;
	using System.Globalization;
	using System.Web;
	using System.Web.UI;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.Constants;
	using FMBusinessObjects.DataObjects;

	using global::FMWebApp;

	/// <summary>
	/// Logout form code behind
	/// </summary>
	public partial class LogoutForm : FMFormBase
	{
	    public const string LogoutFormSessionLogoutKey = "LogoutForm.LogoutKey";
      protected string SessionTimedOut = "false";
      protected string InvalidSession = "false";
      protected string commercialReturnToLoginStr = "false";

      #region Methods

      /// <summary>
      /// Handles the Load event of the Page control.
      /// </summary>
      /// <param name="sender">The source of the event.</param>
      /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
      protected void Page_Load(object sender, EventArgs e)
		{
         Global.Logout(this.Context);

			if (this.Session != null)
			{
				this.Session.Clear();
				Session["IsWebApp"] = "true";
         }
			foreach(string k in Request.QueryString.Keys)
			{
				if(k == "SessionTimedOut")
				{
               SessionTimedOut = "true";
					break;

				}
            if (k == "InvalidSession")
            {
               InvalidSession = "true";
               break;

            }
         }

            var commercialReturnToLogin = FMBusinessObjects.UtilityObjects.AppSettingsHelper.GetKeyValue("CommercialReturnToLogin", false);

            // Check for single signon mode and set session key.
            this.SetSsoLogout();

			foreach(string key in Request.Params.Keys)
			{
				//If logging out due to bad path, then write message to event log.
				if (string.IsNullOrWhiteSpace(key) == false && key.Left(4) == "404;")
				{
					string p = key.Right(key.Length - 4);
					if (p.Length > 0)
					{
						string msg = "Path not found : " + p + ".\nLogging out.";
						Global.WriteToEventLog(msg, System.Diagnostics.EventLogEntryType.Warning);
						break;
					}


                }
			}
			if (commercialReturnToLogin)
            {
				commercialReturnToLoginStr = "true";
   //             // Do not use FMFormBase.Redirect here, as the session has already been logged out and 
   //             // FMFormBase.Redirect will throw here trying to get the CSRF token
   //             this.Response.Redirect("../", endResponse: false);
   //             this.Context.ApplicationInstance.CompleteRequest();
   //             return;
            }


      }

        /// <summary>
        /// This method will check for SSO mode and if so, will set a logout session
        /// key for the login page.
        /// </summary>
	    private void SetSsoLogout()
	    {
            SecurityClass localSecurity = new SecurityClass { SiteGuid = Guids.SiteAdminGuid };
            localSecurity.AddRight(RIGHT.MODIFY_SYSTEM_SETTINGS);

			try
			{
				var configSetting = FMChannelHelper.MakeCall<IConfigurationSettings, ConfigurationSettingDOClass>
													(x => x.GetByKey(localSecurity, ConfigurationSettingDOClass.Key_SingleSignOnMode));

				// This is so that the Login page will not try and auto login the domain user.
				if (configSetting != null
					&& string.IsNullOrEmpty(configSetting.SettingValue) == false
					&& configSetting.SettingValue == "1")
				{
					this.Session.Add(LogoutFormSessionLogoutKey, "TRUE");
				}
			}
			catch(Exception)
            {
				// Ignore
            }
	    }
		#endregion
	}
}