// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UmnyangoForm.aspx.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Code behind for login form
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FuelsManager.FMWebApp
{
    using System;
    using System.Configuration;
    using System.Diagnostics;
    using System.Linq;
    using System.Reflection;
    using System.Web;
    using System.Web.UI.WebControls;

    using FMBusinessObjects.BusinessInterfaces;
    using FMBusinessObjects.ChannelFactories;
    using FMBusinessObjects.Constants;
    using FMBusinessObjects.DataObjects;
    using FMBusinessObjects.Exceptions;
    using FMBusinessObjects.ServiceRequests;
    using FMCore;
    using global::FMWebApp;

   /// <summary>
   /// Code behind class for login form
   /// </summary>
    public partial class UmnyangoForm : UmnyangoFormBase
	{
	#region Constants and Fields

		public static string CsSecurityLevel;

		public static string CsUserName;

      public string PwdHint;

		protected HyperLink AboutUs;

		protected HyperLink ContactUs;

		protected Image Image3;

		#endregion

		#region Properties
		/// <summary>
		/// Gets the assembly file version.
		/// </summary>
		private static string AssemblyFileVersion
		{
			get
			{
				var assembly = Assembly.GetExecutingAssembly();
				var fileVersionInfo = FileVersionInfo.GetVersionInfo(assembly.Location);
				return fileVersionInfo.FileVersion;
			}
		}

        protected string PasswordHint
        {
            get
            {
                return this.PwdHint;
            }
            set
            {
                this.PwdHint = value;
            }
        }
		public bool IsServiceDisrupted { get;set; } = false;
        #endregion

        #region Methods

        /// <summary>
        ///     Handles the Click event of the ForgotPasswordButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">
        ///     The <see cref="System.EventArgs" /> instance containing the event data.
        /// </param>
        protected void ForgotPasswordButtonClick(object sender, EventArgs e)
		{
			try
			{
				if (string.IsNullOrEmpty(this.UserNameTextBox.Text))
				{
					throw new ApplicationException("A User ID is required.");
				}

				this.RedirectWithoutSecurity("ForgotPasswordForm.aspx?userId=" + this.UserNameTextBox.Text + "&siteId=" + this.SiteTextBox.Text + "&ShowPasswordHintButton="+this.PasswordHintButton.Visible+"&currentUserHint="+this.ppPasswordHint.Value);
            }
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		/// <summary>
		/// Handles the password hint and forgot password div.
		/// </summary>
		/// <param name="loginRequest">The login request.</param>
		/// <param name="mySite">My site.</param>
		/// <param name="token">The token.</param>
		protected override void HandlePasswordHintAndForgotPasswordDiv(SecurityLoginRequest loginRequest, SiteClass mySite, string token)
		{
			var security = new SecurityClass
			{
			    SiteGuid = mySite.IdentityGuid          
			};
			security.AddRight(RIGHT.VIEW_SITES_AND_SITE_GROUPS);

			if (!string.IsNullOrEmpty(loginRequest.UserID))
			{
				try
				{
					// Throws indexoutofrangeexception
					this.PasswordHint = FMChannelHelper.MakeCall<ITempPasswordGenerator, string>(tempPasswordGeneratorChannel => tempPasswordGeneratorChannel.GetPasswordHint(security, loginRequest));
                    this.ppPasswordHint.Value = this.PasswordHint;
                }
				// ReSharper disable EmptyGeneralCatchClause
				catch
				// ReSharper restore EmptyGeneralCatchClause
				{
				}
			}
		    this.PasswordHintScript(this.PasswordHint);
            this.PasswordHintButton.Visible = mySite.EnablePasswordHint;
            this.ForgotPasswordButton.Visible = mySite.EnablePasswordReset;
            this.ErrorHandler(new Exception(token));
			this.ResetControls();
		}

		/// <summary>
		///     Raises the <see cref="OnInit" /> event.
		/// </summary>
		/// <param name="e">
		///     The <see cref="System.EventArgs" /> instance containing the event data.
		/// </param>
		protected override void OnInit(EventArgs e)
		{
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			this.InitializeComponent();
			base.OnInit(e);
		}

		/// <summary>
		/// Handles the Init event of the Page control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
		protected void Page_Init(object sender, EventArgs e)
		{
			if (this.IsPostBack == false)
			{
            // Make sure we came from the default page so the banner had a chance
            // to display.  If not, refer them back.          
            var urlReferrer = this.Request.UrlReferrer;

				int i = urlReferrer?.Segments.Length - 1 ?? 0;

				if (i > 1 && urlReferrer != null && urlReferrer.Segments[i] == "ChangePasswordForm.aspx"
					&& urlReferrer.Segments[0] == this.Request.Url.Segments[0]
					&& urlReferrer.Segments[1] == this.Request.Url.Segments[1]
					&& urlReferrer.Host == this.Request.Url.Host)
				{
					//coming from Change Password page. Display login without displaying warning banner.
					this.AcceptButtonClick(null, null);
				}
				else
				{
				    var ssoLogout = this.Session[LogoutForm.LogoutFormSessionLogoutKey] as string;

					// If session is active, it needs to be closed. This may happen
					// when user enters FuelsManager home URL without first logging out of 
					// the previous session.
				    Global.Logout(this.Context);

                    // Have to reset the SSO Logout session key because the Global.Logout clears
                    // all session keys.
				    if (string.IsNullOrEmpty(ssoLogout) == false && ssoLogout.Equals("TRUE"))
				    {
				        this.Session.Add(LogoutForm.LogoutFormSessionLogoutKey, "TRUE");
					}

					//Prepare to display warning banner.
					this.WarningPageInit(sender, e);
				}
			}
			else
			{
			    this.Session.Remove("Security");
			}
		}

		/// <summary>
		/// Handles the Load event of the Page control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
		/// <exception cref="System.Exception">Multiple Sites are not permitted</exception>
		protected void Page_Load(object sender, EventArgs e)
		{

			CheckForCSRF();

         try
			{            
				this.Session.Timeout = GetSessionTimeout();
				if (Convert.ToBoolean(this.Request.QueryString["ShowForgotPassword"]) == true)
				{
					this.DivPasswordForgotPassword.Visible = true;
					this.ForgotPasswordButton.Visible = true;
				}
				if (Convert.ToBoolean(this.Request.QueryString["ShowPasswordHint"]) == true)
				{
					this.PasswordHint = this.Request.QueryString["currentUserHint"];
					this.ppPasswordHint.Value = this.PasswordHint;
					this.PasswordHintScript(this.PasswordHint);
					this.PasswordHintButton.Visible = true;
				}
			}
			catch (Exception except)
			{
				if (except.Message == "Session Not Found" || except.Message == "Session Timeout")
				{
					this.Session.Remove("Token");
				}
				else
				{
					this.ErrorHandler(except);
				}
			}
		}

		protected void WarningPageInit(object sender, EventArgs e)
		{
			var token = this.Session["Token"] as string;
			if (token != null)
			{
				//If session is active, it needs to be closed. This may happen
				//when user enters FuelsManager home URL without first logging out of 
				//the previous session.l
				//            Session.Abandon();
				FMChannelHelper.MakeCall<ISites>(x => x.LogoutToken(token));
			}

			string backgroundColor = ConfigurationManager.AppSettings["LoginPageBackgroundColor"];
			if (string.IsNullOrEmpty(backgroundColor) == false)
			{
				this.PageBody.Style["Background-Color"] = backgroundColor;
			}

			string warningImage = ConfigurationManager.AppSettings["WarningPageImage"];
			if (string.IsNullOrEmpty(warningImage) == false)
			{
				this.warnDiv.Style["Background-Image"] = $"url({warningImage})";
			}

			string titleText = ConfigurationManager.AppSettings["WarningTitle"];
			if (String.IsNullOrEmpty(titleText) == false)
			{
				this.TitleLabel.Text = titleText;
			}

			string warningText = ConfigurationManager.AppSettings["WarningText"].DefaultIfNull(string.Empty);
			if (Global.IsFdsIM == false)
			{
				this.AcceptButtonClick(null, null);
			}
			else
			{
				string searchReplace = ConfigurationManager.AppSettings["WarningTextSearchAndReplace1"];
				if(!string.IsNullOrWhiteSpace(searchReplace))
				{
					string[] str = searchReplace.Split('~');
					if (str.Length >= 2) warningText = System.Text.RegularExpressions.Regex.Replace(warningText, str[0], str[1]);
				}

				warningText = warningText.Replace("\r\n", "<br><br>");

				searchReplace = ConfigurationManager.AppSettings["WarningTextSearchAndReplace2"];
				if (!string.IsNullOrWhiteSpace(searchReplace))
				{
					string[] str = searchReplace.Split('~');
					if (str.Length >= 2) warningText = System.Text.RegularExpressions.Regex.Replace(warningText, str[0], str[1]);
				}

				//warningText = warningText.Replace("-", "&bull; ");

				this.WarningLabel.InnerHtml = warningText;
			}
		}

		protected void InitializeLogin()
		{
			try
			{
				this.Session["CSRFToken"] = SecurityClass.GenerateCSRFToken(string.Empty);//generate new one for next response;
				bool useDictionary = true;

				if (this.Session["UseDataDictionary"] != null)
				{
					useDictionary = (bool)this.Session["UseDataDictionary"];
				}
				else
				{
					this.Session.Add("UseDataDictionary", true);
				}

				this.Session.Timeout = PageSessionKeyConstants.MAXIMUM_SESSION_TIMEOUT;

				this.PasswordHintButton.Attributes["OnClick"] = "return false;";
				this.DivPasswordForgotPassword.Visible = false;
				this.PasswordHintButton.Visible = false;
				this.ForgotPasswordButton.Visible = false;

				string splashImageUrl = ConfigurationManager.AppSettings["LoginPageSplashImage"];
				if (string.IsNullOrEmpty(splashImageUrl) == false)
				{
					this.splashDiv.Style["background-image"] = $"url({splashImageUrl})";
				}

				string backgroundColor = ConfigurationManager.AppSettings["LoginPageBackgroundColor"];
				if (string.IsNullOrEmpty(backgroundColor))
				{
					backgroundColor = "#006CB3";
				}

				this.PageBody.Style["Background-Color"] = backgroundColor;
				
				string welcomeTitle = ConfigurationManager.AppSettings["LoginPageWelcomeTitle"];
				this.PageTitleTb.Text = "EMPTY";

				if (string.IsNullOrEmpty(welcomeTitle) == false)
				{
					this.FMLabel1.Text = welcomeTitle;
					this.PageTitleTb.Text = welcomeTitle;
				}

				string surpressBottomLinks = ConfigurationManager.AppSettings["LoginPageSurpressBottomLinks"];
				this.SurpressLoginPageLinksTB.Text = "FALSE";

				if (string.IsNullOrEmpty(surpressBottomLinks) == false && surpressBottomLinks.ToUpper().Equals("TRUE"))
				{
					this.SurpressLoginPageLinksTB.Text = "TRUE";
				}

				if (Global.IsFdsIM)
				{
					string javascriptRunAfterAccept = ConfigurationManager.AppSettings["JavascriptRunAfterAccept"];
					if (string.IsNullOrEmpty(javascriptRunAfterAccept) == false)
					{
						this.ClientScript.RegisterClientScriptBlock(this.GetType(), "javascriptafterRun", javascriptRunAfterAccept, true);
					}
				}

				var security = new SecurityClass { SiteGuid = Guids.SiteAdminGuid };
				security.AddRight(RIGHT.VIEW_SITES_AND_SITE_GROUPS);
				security.AddRight(RIGHT.MODIFY_SITES_AND_SITE_GROUPS);
				security.AddRight(RIGHT.VIEW_USERS);
				security.AddRight(RIGHT.MODIFY_USERS);
				security.AddRight(RIGHT.VIEW_USER_GROUPS);
				security.AddRight(RIGHT.MODIFY_USER_GROUPS);
				security.AddRight(RIGHT.VIEW_EQUIPMENT_DATA);
				security.AddRight(RIGHT.MODIFY_EQUIPMENT_DATA);

				this.Session["SiteGuid"] = Guids.SiteAdminGuid;

                try
                {
                    this.SetCustomizedLinks(security);
                }
                catch
                {
                    this.IsServiceDisrupted = true;
					return;
                }

                string serviceLogin = FMChannelHelper.MakeCall<IDBAccess, string>(dbAccessChannel => dbAccessChannel.ServiceLogin(security));
				security.UserID = serviceLogin;

                bool multipleSiteKey = false;

				FMChannelHelper.MakeCall<IHardwareKey>(hardwareKeyChannel =>
				{
					multipleSiteKey = hardwareKeyChannel.IsMultipleSiteKey();
				});

				int siteCount = FMChannelHelper.MakeCall<ISites, int>(x => x.GetSiteCount(security));
				// Depends upon Multiple Site
				if (multipleSiteKey)
				{
					if (siteCount == 1)
					{
						this.SiteTextBox.Visible = false;
						this.SiteTextBox.Text = "SiteAdmin";
					}
				}
				else
				{
					SiteCollectionClass siteCollection = FMChannelHelper.MakeCall<ISites, SiteCollectionClass>(x => x.Enumerate(security));

					// Single Site Operation
					SiteClass defaultSite = null;
					SiteClass siteAdminSite = null;

					foreach (SiteClass site in siteCollection)
					{
						if (site.SiteGroup)
						{
							if (site.IdentityGuid == Guids.SiteAdminGuid)
							{
								siteAdminSite = site;
							}
							continue;
						}

						if (defaultSite == null)
						{
							defaultSite = site;
						}
						else
						{
							throw new Exception("Multiple Sites are not permitted");
						}
					}

					if (siteAdminSite == null)
					{
						throw new Exception("No SiteAdmin");
					}

					if (siteCount == 1)
					{
						this.SiteTextBox.Visible = false;
						this.SiteTextBox.Text = "SiteAdmin";
					}
					else
					{
						this.SiteTextBox.Visible = true;
						this.SiteTextBox.ReadOnly = true;
					}

					this.SiteTextBox.Text = (defaultSite == null) ? siteAdminSite.ID : defaultSite.ID;
				}

				// Apply the data dictionary to the non FMControl objects.
				this.ApplyDictionary(useDictionary);

				this.Page.ClientScript.RegisterStartupScript(
				this.GetType(),
				"sf",
				string.Format("<script language='if (javascript'>document.forms[0].{0} != undefined) javascript'>document.forms[0].{0}.focus();</script>", this.UserNameTextBox.ClientID ));

				var adUser = GetCurrentAdUser(security);
				if(adUser != null)
				{
                    this.UserNameTextBox.Visible = false;
                    this.PasswordTextBox.Visible = false;
					this.SiteTextBox.Visible = false;
					this.ChangePasswordButton.Visible = false;
                    this.SiteListDropDown.Visible = true;

                    var siteCollection = FMChannelHelper.MakeCall<ISites, SiteCollectionClass>(x => x.EnumerateByUser(this.Security, adUser.IdentityGuid));

					SiteListDropDown.Visible = true;
                    SiteListDropDown.DataTextField = "ID";
                    SiteListDropDown.DataValueField = "ID";
                    SiteListDropDown.DataSource = siteCollection;
                    SiteListDropDown.DataBind();

                    SiteClass userSite = null;
                    if (siteCollection.Count == 1)
					{
						userSite = siteCollection[0];
					}
					else if(!base.IsEnterprise)
					{
						var usersites = siteCollection.Where(x => !x.SiteGroup).ToList();
						if(usersites.Count == 1)
						{
                            userSite = usersites[0];
                        }
                    }

                    if (userSite != null)
					{
                        // if there is only one site for the user login immediately
                        // otherwise we will give them option to choose site
                        this.HandleSsoSignIn(security,adUser, userSite.ID);
                    }
                }

				string cacEnableString = FMChannelHelper.MakeCall<IConfigurationSettings, string>(x => x.GetKeyValueByKey(this.Security, ConfigurationSettingDOClass.Key_CAC_Enable));
				if (cacEnableString == "1")
				{
					HttpClientCertificate cs = this.Request.ClientCertificate;

					if (1 == cs.Flags)
					{
						this.UserNameTextBox.Visible = false;

						this.PasswordTextBox.Visible = false;

						if (this.SiteTextBox.Visible == false)
						{
							if (this.versionCheckFailed)
							{
								this.ResetControls();
							}
							else
							{
								this.Login(this.UserNameTextBox.Text, this.PasswordTextBox.Text, this.SiteTextBox.Text, false, false);
							}
						}
					}
				}

			}
			catch (Exception except)
			{
				if (except.Message == "Session Not Found" || except.Message == "Session Timeout")
				{
					this.Session.Remove("Token");
				}
				else
				{
					this.ErrorHandler(except);
				}
			}
			finally
			{
            this.GetVersionNumbersAndBuildDate();

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
			catch(Exception)
            {
				return isSsoMode;
			}

			return isSsoMode;
        }

		private UserClass GetCurrentAdUser(SecurityClass security)
		{
            UserClass AdUser = null;

            if (this.IsSsoMode(security))
            {
                var domainUserName = HttpContext.Current.User.Identity.Name;

                var fullName = System.Security.Principal.WindowsIdentity.GetCurrent().Name;

                if (!string.IsNullOrWhiteSpace(domainUserName) && domainUserName.Contains("\\"))
                {
                    var users = FMChannelHelper.MakeCall<IUsers, UserCollectionClass>(x => x.GetUsersByIDWithoutSite(security, domainUserName));
                    var user = users.FirstOrDefault();

                    if (user != null && user.IdentityGuid != Guid.Empty && user.ActiveDirectoryUser)
                    {
                        var isActiveDirectoryUser = FMChannelHelper.MakeCall<IActiveDirectoryService, bool>(x => x.ConfirmUser(domainUserName));

                        if (isActiveDirectoryUser)
                        {
							AdUser = user;
                        }
                    }
                }
            }
            return AdUser;
		}

		/// <summary>
		/// This method will handle the SSO sign in.
		/// </summary>
		/// <param name="security">The secruity object.</param>
		private void HandleSsoSignIn(SecurityClass security, UserClass user, string siteID)
		{
			// Get single sign on mode flag.
			bool ssoMode = this.IsSsoMode(security);

			string ssoLogout = this.Session[LogoutForm.LogoutFormSessionLogoutKey] as string;
			bool ssoLogoutFlag = (string.IsNullOrEmpty(ssoLogout) == false && ssoLogout.Equals("TRUE"));
			this.Session.Remove(LogoutForm.LogoutFormSessionLogoutKey);

			string fromChangePwForm = this.Session[ChangePasswordForm.FromChangePwFormKey] as string;
			bool fromChangePwFlag = (string.IsNullOrEmpty(fromChangePwForm) == false && fromChangePwForm.Equals("TRUE"));
			this.Session.Remove(ChangePasswordForm.FromChangePwFormKey);

			if (ssoMode && ssoLogoutFlag == false && fromChangePwFlag == false)
			{
				var domainUserName = HttpContext.Current.User.Identity.Name;

				if (!string.IsNullOrWhiteSpace(domainUserName) && domainUserName.Contains("\\"))
				{
					if (user != null && user.IdentityGuid != Guid.Empty && user.ActiveDirectoryUser && domainUserName.Equals(user.ID,StringComparison.InvariantCultureIgnoreCase))
					{
						var isActiveDirectoryUser = FMChannelHelper.MakeCall<IActiveDirectoryService, bool>(x => x.ConfirmUser(domainUserName));

						if (isActiveDirectoryUser)
						{
							this.Login(user.ID, null, siteID, false, true);
						}
                        else
						{
                            var message = "Could not confirm domain user: " + domainUserName;
                            using (var eventLog = new EventLog("Application", ".", "FuelsManager"))
                            {
                                eventLog.WriteEntry(message, EventLogEntryType.Error);
                            }
                        }
					}
					else
					{
                        var message = "User Id does not match domain UserName";
						if(!string.IsNullOrWhiteSpace(domainUserName) )
						{
							message += " domainUserName:" + domainUserName;
                        }
                        if (!string.IsNullOrWhiteSpace(user?.ID))
                        {
                            message += " ID:" + user?.ID;
                        }
                        using (var eventLog = new EventLog("Application", ".", "FuelsManager"))
                        {
                            eventLog.WriteEntry(message, EventLogEntryType.Error);
                        }
                    }
				}
				else
				{
                    var message = "Invalid domain user";
                    using (var eventLog = new EventLog("Application", ".", "FuelsManager"))
                    {
                        eventLog.WriteEntry(message, EventLogEntryType.Error);
					}
				}
			}
		}

		/// <summary>
		/// This method will retrieve the Site ID of the terminal site if the user is assigned to the
		/// terminal site.  This is only used for single sign on mode.
		/// </summary>
		/// <param name="security">The security object.</param>
		/// <param name="userGuid">The user Guid.</param>
		/// <returns>Returns the user's terminal site ID if found. Otherwise, returns an empty string.</returns>
		private string GetTerminalSiteId(SecurityClass security, Guid userGuid)
        {
			var siteCollection = FMChannelHelper.MakeCall<ISites, SiteCollectionClass>(x => x.EnumerateByUser(security, userGuid));

			if(siteCollection == null || siteCollection.Count == 0)
            {
				return string.Empty;
            }

			SiteClass terminalSite = siteCollection.Find(x => x.SiteGroup == false);

			if(terminalSite == null)
            {
				return string.Empty;
            }

			return terminalSite.ID;
        }

		/// <summary>
		/// Gets version numbers and build date, and assigns them to session variables.
		/// 
		/// </summary>
		private void GetVersionNumbersAndBuildDate()
		{
			
			string versionText = "Unable to Determine Build Version";

         try
			{
				// System Version and build date
				var versionInfo = FMChannelHelper.MakeCall<IDBAccess, VersionInfo>(x => x.GetVersion());
				versionText = versionInfo.ToString(4);			
			}
			catch
			{
            versionText = "Unable to Determine Build Version";
         }

			string businessServicesVersion = "Unable to Determine Build Version";

			try
			{
				businessServicesVersion = FMChannelHelper.MakeCall<IGeneralConfigProcessor, string>(x => x.GetAssemblyFileVersion());
			}
			catch
			{
            businessServicesVersion = "Unable to Determine Build Version";
         }

			this.Session["DatabaseVersion"] = versionText;
			this.Session["FuelsManagerVersion"] = AssemblyFileVersion;
			this.Session["FMBusinessServicesVersion"] = businessServicesVersion;

			this.Session["PrivacyPolicyPath"] = ConfigurationManager.AppSettings["PrivacyPolicyPath"];
			
			this.ppPath.Value = (string)this.Session["PrivacyPolicyPath"];

			this.FMLabelBuildVersion.Text = "Version: " + AssemblyFileVersion;

			this.FMLabelBuildVersion.ToolTip = $"FuelsManager Web: {AssemblyFileVersion}\nDatabase: {versionText}\nFMBusinessServices: {businessServicesVersion}";
		}

		/// <summary>
		///     This method will apply the data dictionary to the non FMControl objects.
		/// </summary>
		/// <param name="useDictionary">Flag to indicate whether to use the data dictionary.</param>
		private void ApplyDictionary(bool useDictionary)
		{
			if (useDictionary && this.Session["SiteGuid"] != null)
			{
				var siteGuid = (Guid)this.Session["SiteGuid"];

				string newText = this.GetDataDictionaryValueByKey(siteGuid, this.ContactUsHyperLink.Text);
				this.ContactUsHyperLink.Text = newText;

				newText = this.GetDataDictionaryValueByKey(siteGuid, this.SupportHyperLink.Text);
				this.SupportHyperLink.Text = newText;

				newText = this.GetDataDictionaryValueByKey(siteGuid, this.PrivacyHyperLink.Text);
				this.PrivacyHyperLink.Text = newText;

				newText = this.GetDataDictionaryValueByKey(siteGuid, this.CopyrightHyperLink.Text);
				this.CopyrightHyperLink.Text = newText;
			}
		}

		protected void AcceptButtonClick(object sender, EventArgs e)
		{
		    this.Session.Add("WARNINGACCEPTED", true);
		    this.warnDiv.Visible = false;
		    this.splashDiv.Visible = true;
		    this.InitializeLogin();
		}

		/// <summary>
		///     Required method for Designer support - do not modify
		///     the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.LoginButton.Command += this.LoginButtonCommand;
			this.AcceptButton.Command += this.AcceptButtonClick;
		}


        /// <summary>
        ///     Handles the Command event of the ChangePasswordButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">
        ///     The <see cref="System.Web.UI.WebControls.CommandEventArgs" /> instance containing the event data.
        /// </param>
        protected void ChangePasswordButtonCommand(object sender, EventArgs e)
		{
			if (this.versionCheckFailed)
			{
				this.ResetControls();
				return;
			}

			try
			{
				this.Login(this.UserNameTextBox.Text, this.PasswordTextBox.Text, this.SiteTextBox.Text, true, false);
			}
			catch (Exception exception)
			{
				this.ErrorHandler(exception);
				this.ResetControls();
			}
		}

		/// <summary>
		///     Handles the Command event of the LoginButton control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">
		///     The <see cref="System.Web.UI.WebControls.CommandEventArgs" /> instance containing the event data.
		/// </param>
		private void LoginButtonCommand(object sender, CommandEventArgs e)
		{
			if (this.versionCheckFailed)
			{
				this.ResetControls();
				return;
			}

         //Ensure login request is from same origin. 
         string requestSchemeAndHostname = $"{Request.Url.Scheme}://{Request.Url.DnsSafeHost}";

         string origin = this.Request.Headers["Origin"];
         if (!string.IsNullOrWhiteSpace(origin))
         {
            if (origin.NotEquals(requestSchemeAndHostname, StringComparison.OrdinalIgnoreCase))
            {
               Global.WriteToEventLog($"Login request is not from same origin. Login failed. Origin header={origin}   requested url scheme+host={requestSchemeAndHostname}  ", EventLogEntryType.Error);
               throw new Exception("Login failed");
            }
         }
         else if (this.Request.UrlReferrer != null && !string.IsNullOrWhiteSpace(this.Request.UrlReferrer.AbsolutePath))
         {
            string referer = $"{this.Request.UrlReferrer.Scheme}://{this.Request.UrlReferrer.DnsSafeHost}";
            if (referer.NotEquals(requestSchemeAndHostname, StringComparison.OrdinalIgnoreCase))
            {
               Global.WriteToEventLog($"Login request is not from same origin. Login failed. Referer header={referer}    requested scheme+host={requestSchemeAndHostname}  ", EventLogEntryType.Error);
               throw new Exception("Login failed");
            }
         }

         var security = new SecurityClass { SiteGuid = Guids.SiteAdminGuid };
         security.AddRight(RIGHT.VIEW_USERS);
         var adUser = GetCurrentAdUser(security);

			if(adUser != null)
			{
                // Handle the SSO sign in.
                this.HandleSsoSignIn(security, adUser, this.SiteListDropDown.SelectedValue);
			}
			else
			{
                try
                {
                    if( !string.IsNullOrEmpty( pointgroupreportgeneration.Text ))
                    {
                        this.Session.Add("pointgroupreportgeneration", pointgroupreportgeneration.Text);
                    }
                    this.Login(this.UserNameTextBox.Text, this.PasswordTextBox.Text, this.SiteTextBox.Text, false, false);

                    this.GetVersionNumbersAndBuildDate();
                }
                catch (Exception exception)
                {
					if(exception.InnerException != null && exception.InnerException.Message == FMLicenseException.LicenseHasExpired)
					{
                        this.ErrorHandler(exception.InnerException);
					}
					else
					{
                    this.ErrorHandler(exception);
                }
                    
                }
                finally
                {
                    this.ResetControls();
                }
            }
		}

		/// <summary>
		///     Resets the controls.
		/// </summary>
		private void ResetControls()
		{
			this.PasswordTextBox.Text = string.Empty;
			this.UserNameTextBox.Text = string.Empty;

			// do not clear the site text box
			// when in single site mode we need to restore the site name
			if (this.SiteTextBox.Visible && this.SiteTextBox.ReadOnly != true)
			{
				this.SiteTextBox.Text = string.Empty;
			}
		}

		/// <summary>
		///     Sets the customized links.
		/// </summary>
		private void SetCustomizedLinks(SecurityClass security)
		{

			var contactUsLink = new WebLink();
			var supportLink = new WebLink();

			FMChannelHelper.MakeCall<IWebLinks>(
				x =>
				{
					contactUsLink = x.GetByName(security, "Contact Us");
					supportLink = x.GetByName(security, "Support");
				});

			bool defenseKey = FMChannelHelper.MakeCall<IHardwareKey, bool>(x => x.IsDescKey());

			string linkText = contactUsLink == null ? "" : contactUsLink.LinkAddress;
			if (String.IsNullOrEmpty(linkText))
			{
				linkText = ConfigurationManager.AppSettings["ContactUsLink"];
			}
			if (string.IsNullOrEmpty(linkText) == false)
			{
				this.ContactUsHyperLink.NavigateUrl = linkText;
			}

			linkText = supportLink == null ? "" : supportLink.LinkAddress;
			if (String.IsNullOrEmpty(linkText))
			{
				linkText = ConfigurationManager.AppSettings["SupportLink"];
			}
			if (string.IsNullOrEmpty(linkText) == false)
			{
				this.SupportHyperLink.NavigateUrl = linkText;
			}

			if (defenseKey)
			{
				this.PrivacyHyperLink.Visible = false;
				this.PrivacySeparatorLabel.Visible = false;

				this.CopyrightHyperLink.Visible = false;
				this.CopyrightSeparatorLabel.Visible = false;

				this.DisplayDlaPrivacyPolicyLink();
			}
			else
			{
				linkText = ConfigurationManager.AppSettings["PrivacyLink"];
				if (string.IsNullOrEmpty(linkText) == false)
				{
					this.PrivacyHyperLink.NavigateUrl = linkText;
				}

				linkText = ConfigurationManager.AppSettings["CopyrightLink"];
				if (string.IsNullOrEmpty(linkText) == false)
				{
					this.CopyrightHyperLink.NavigateUrl = linkText;
				}
			}
		}

		private void DisplayDlaPrivacyPolicyLink()
		{
			var displayPrivacyPolicyLink = false;
			var privacyPolicyPath = ConfigurationManager.AppSettings["PrivacyPolicyPath"];
			if (!string.IsNullOrEmpty(privacyPolicyPath))
			{
				displayPrivacyPolicyLink = true;
			}
			
			if (displayPrivacyPolicyLink)
			{
				this.DlaPrivacyPolicyLink.Visible = true;
				this.DlaPrivacyPolicySeparatorLabel.Visible = true;
			}
		}

        ///<summary>
        /// Ajax script for password hint bubble
        /// </summary>
        protected void PasswordHintScript(string passWordHint)
        {
            //         string passWordHintScript = @"
            //<script type='text/javascript'>
            //<!--
            //         $(document).ready(function () 
            //         {
            //             //create a bubble popup for each DOM element with class attribute as 'text', 'button' or 'link' and LI, P, IMG elements.
            //             $('#PasswordHintButton').CreateBubblePopup(
            //             {
            //                 position: 'center',
            //                 align: 'center',
            //                 innerHtml: '";
            //                 passWordHintScript += passWordHint;
            //                 passWordHintScript += @"',

            //                 innerHtmlStyle: 
            //                 {
            //                     color: '#000000',
            //                     'text-align':'center',
            //                     'padding': '0'
            //                 },

            //                 themeName: 'blue',
            //                 themePath: '../jquerybubblepopup/themes'
            //             });
            //         });
            ////-->
            //</script>";

            this.PasswordHintButton.Attributes["title"] = passWordHint;

            //this.Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "PasswordHintScript", passWordHint);
            this.DivPasswordForgotPassword.Visible = true;
        }
		private void CheckForCSRF()
		{

			try
			{
				if (this.IsPostBack == false || string.IsNullOrWhiteSpace(Session["LoginCSRFToken"] as string) || string.IsNullOrEmpty(LoginCSRFToken.Value))
				{
					Session["LoginCSRFToken"] = SecurityClass.GenerateCSRFToken(null);
				}
				LoginCSRFToken.Value = Session["LoginCSRFToken"] as string;

				if (this.IsPostBack)
				{
					//CSRF validation
					string csrfToken = Request.Params["LoginCSRFToken"];
					string sessionCSRFToken = Session["LoginCSRFToken"] as string;
					if (string.IsNullOrEmpty(csrfToken)
						|| string.IsNullOrEmpty(sessionCSRFToken)
						|| sessionCSRFToken.NotEquals(csrfToken))
					{
						throw new Exception("Invalid CSRF token. Login failed.");
					}
				}
			}
			catch (Exception ex)
			{
				Global.WriteToEventLog(ex.Message, EventLogEntryType.Error);
				PasswordTextBox.Text = string.Empty;
				UserNameTextBox.Text = string.Empty;
			}
		}

      protected void Page_LoadComplete(object sender, EventArgs e) {
            ServiceInterruptionLabel.Visible = this.IsServiceDisrupted;
            this.FMLabel1.IsServiceDisrupted = this.IsServiceDisrupted;
            this.FMLabelBuildVersion.IsServiceDisrupted = this.IsServiceDisrupted;
            this.FMLoginLabel.IsServiceDisrupted = this.IsServiceDisrupted;
            this.AcceptButton.IsServiceDisrupted = this.IsServiceDisrupted;
            this.LoginButton.IsServiceDisrupted = this.IsServiceDisrupted;

        }
        #endregion
    }
}
