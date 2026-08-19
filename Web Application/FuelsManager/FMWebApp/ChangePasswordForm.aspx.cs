// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ChangePasswordForm.aspx.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the ChangePasswordForm type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FMWebApp
{
	using System;
	using System.Collections.Generic;
	using System.Drawing;
	using System.Web.UI.WebControls;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.Constants;
	using FMBusinessObjects.DataObjects;

	using FMCore;

	using FuelsManager.FMWebApp;
   using Opc.Hda;

   /// <summary>
   ///    Summary description for ChangePasswordForm.
   /// </summary>
	public partial class ChangePasswordForm : UmnyangoFormBase, IMenuDiscovery
	{
        #region Data Members
        public const string FromChangePwFormKey = "ChangePasswordForm.RedirectFromChangePwForm";
        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///    Gets a list of menu items that should be displayed for the current user.
        /// </summary>
        /// <param name="security">The security object of the current session</param>
        /// <param name="siteGroup">Whether the current logged-in site is a site group</param>
        /// <param name="options">Hardware key options</param>
        /// <returns>
        ///    List of menu items to be displayed
        /// </returns>
        public List<FMMenuItem> GetMenuItems(SecurityClass security, bool siteGroup, ushort word1,ushort word2, ushort useNewLicenseKey, uint options)
		{
            if (useNewLicenseKey == 1)
            {

            }
            else
            {
                // Depends Upon Shared Components Config
                if ((options & 0x4000) == 0)
                {
                    return null;
                }
            }
            var menuItems = new List<FMMenuItem>();

			var menuItem = new FMMenuItem
				{
					MenuItemType = FMMenuItemType.ADMIN_SECURITY_CHANGE_PASSWORD,
					RootMenuName = "Administration",
					CategoryName = "Security",
					ItemName = "Change Password",
					NavigateUrl = "ChangePasswordForm.aspx?FromApplication=true&FromOperate=false",
					ApplyDataDictionary = ApplyDataDictionary.Apply
				};

			menuItems.Add(menuItem);

			return menuItems;
		}

		#endregion

		#region Methods

		protected override void OnInit(EventArgs e)
		{
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			InitializeComponent();
			base.OnInit(e);
		}

		protected void Page_Load(object sender, EventArgs e)
		{
			try
			{
				this.GetSecurity();
				this.PasswordPopupBubble();

				this.Form.DefaultButton = "OK";
				this.ErrorMsgLabel.Text = string.Empty;

				if (!this.Page.IsPostBack)
				{
					bool fromapplication = Convert.ToBoolean(this.Request.GetQueryOrFormValue("FromApplication"));
					bool fromoperate = Convert.ToBoolean(this.Request.GetQueryOrFormValue("FromOperate"));
					if (null == this.Session["FromApplication"])
					{
						this.Session.Add("FromApplication", fromapplication);
					}
					else
					{
						this.Session["FromApplication"] = fromapplication;
					}

					// set the password page as loading. This is done so we can track it in view operate only mode
					if (null == this.Session["ChangePassword"])
					{
						this.Session.Add("ChangePassword", true);
					}
					else
					{
						this.Session["ChangePassword"] = true;
					}

					if (null == this.Session["FromOperate"])
					{
						this.Session.Add("FromOperate", fromoperate);
					}
					else
					{
						this.Session["FromOperate"] = fromoperate;
					}


					// Do not show menu if we just came from login screen
					if (!((bool)this.Session["FromApplication"]) ||
					    ((bool)this.Session["FromOperate"]))
					{
						this.ucFMMenuBar.Visible = false;
					}
				}

			    if (this.IsSsoMode() && this.Security.ActiveDirectoryUser)
			    {
			        this.CurrentPasswordTextBox.Enabled = false;
			        this.NewPasswordTextBox.Enabled     = false;
			        this.ReenterPasswordTextBox.Enabled = false;
			        this.OK.Enabled                     = false;
			    }
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		/// <summary>
		/// This method handles the Cancel button event.
		/// </summary>
		/// <param name="sender">The sending object.</param>
		/// <param name="e">Command event arguments.</param>
		protected void CancelCommand(object sender, CommandEventArgs e)
		{
			this.Session["ChangePassword"] = false;

			if (((bool)this.Session["FromApplication"]) ||
			    ((bool)this.Session["FromOperate"]))
			{
				if ((bool)this.Session["FromOperate"])
				{
					string url = ResolveUrl("~/InventoryManagement/Operate/OperateIndex");
					this.Redirect(url);
				}
				else
				{
					this.Redirect(ResolveUrl("~/FMWebApp/FuelsManagerForm.aspx"));
				}
			}
			else
			{
                // This is so that the Login page will not try and auto login the domain user.
			    if (this.IsSsoMode())
			    {
			        this.Session.Add(FromChangePwFormKey, "TRUE");
			    }

				this.Redirect(ResolveUrl("~/FMWebApp/UmnyangoForm.aspx"));
			}
		}

	    private bool IsSsoMode()
	    {
            SecurityClass localSecurity = new SecurityClass { SiteGuid = Guids.SiteAdminGuid };
            localSecurity.AddRight(RIGHT.MODIFY_SYSTEM_SETTINGS);

			try
			{
				var configSetting = FMChannelHelper.MakeCall<IConfigurationSettings, ConfigurationSettingDOClass>
													(x => x.GetByKey(localSecurity, ConfigurationSettingDOClass.Key_SingleSignOnMode));

				// This is so that the Login page will not try and auto login the domain user.
				if (configSetting != null && string.IsNullOrEmpty(configSetting.SettingValue) == false && configSetting.SettingValue == "1")
				{
					return true;
				}
			}
			catch(Exception)
            {
				return false;
            }

	        return false;
	    }

		/// <summary>
		///    Required method for Designer support - do not modify
		///    the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.Cancel.Command += this.CancelCommand;
			this.OK.Command += this.OkCommand;
		}

		/// <summary>
		///    This method handles the new Password change event. It perform validations to ensure
		///    the Password meets the new security standards.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OkCommand(object sender, CommandEventArgs e)
		{
			try
			{
				if (this.NewPasswordTextBox.Text != this.ReenterPasswordTextBox.Text)
				{
					throw new Exception("New Password vs. Re-enter Password does not match");
				}

				UserClass user = FMChannelHelper.MakeCall<IUsers, UserClass>(
																	 x =>
																	 x.Get(this.Security, this.Security.UserGuid));

				if (user.Password == this.NewPasswordTextBox.Text)
				{
					throw new ApplicationException("New Password must be different from Current Password");
				}

				if (FMChannelHelper.MakeCall<ISites, Boolean>(
                                                    x =>
                                                    x.CheckCurrentPassword(user, this.CurrentPasswordTextBox.Text) 
                                                ) == false)
				{
					throw new ApplicationException("Current Password entered incorrectly");
				}

				string oldPassword		= user.Password;
				user.Password			= this.NewPasswordTextBox.Text;
				user.ChangePassword		= false;

				FMChannelHelper.MakeCall<IUsers>(x => x.ModifyWithPasswordHistory(this.Security, user, oldPassword));

				this.Session["ChangePassword"] = false;
				this.Security.Password = user.Password;
				this.Security.ForcePasswordUpdate = false;
				this.Security.SkipSessionTimeUpdate = false;
				if (((bool)this.Session["FromApplication"]) ||
						((bool)this.Session["FromOperate"]))
				{

               if ((bool)this.Session["FromOperate"])
               {
                  string url = ResolveUrl("~/InventoryManagement/Operate/OperateIndex");
                  this.Redirect(url);
               }
               else
               {
                  this.Redirect(ResolveUrl("~/FMWebApp/FuelsManagerForm.aspx"));
               }
            }
				else
				{

					this.Session.Remove(FMBusinessObjects.Constants.PageSessionKeyConstants.FM_MENU_DATA);
					this.Response.Write("<script type='text/javascript'>\n\r" +
					   "<!--\n\r" +
					   "window.top.document.writeln('<'+'html><'+'body><'+ 'form method=\"post\"  action=\"FuelsManagerForm.aspx\" >' +\n\r" +
					   "'<' + 'input type=\"hidden\" name=\"CSRFToken\" value=\"" + Security.CSRFToken + "\" />' + \n\r" +
					   "'<' + '/form><' + 'script>document.forms[0].submit();<' + '/script><' + '/body><' + '/html>');\n\r" +
					   "//-->\n\r" +
					   "</script>");
				}
			}
			catch (Exception except)
			{
				this.NewPasswordTextBox.Text = string.Empty;
				this.ReenterPasswordTextBox.Text = string.Empty;

				// When the menu does not exist the error message is not displayed.
				// Present the error message in a label.
				if (((bool) this.Session["FromApplication"]))
				{
					this.ErrorHandler(except);
				}
				else
				{
					this.ErrorMsgLabel.Text = "Error: " + except.Message;
					this.ErrorMsgLabel.ForeColor = Color.Red;
				}
			}
		}
        #endregion
        /// <summary>
        /// Displays the password policy in a popup bubble.
        /// </summary>
        protected void PasswordPopupBubble()
        {
			Guid siteGuid = this.Security.SiteGuid;
			SiteClass site = FMChannelHelper.MakeCall<ISites, SiteClass>(sites => sites.Get(this.Security, siteGuid, false, false, false));
			string passwordPopupMessageEnhanced = "Must be at least " + site.MinPasswordCharacterLength + " characters long and contain at least two lower case letters, two upper case letters, two digits, and two special characters (@#$%^&+.,!=).";
			string passwordPopupMessageStrong = "Must be at least " + site.MinPasswordCharacterLength + " characters long and contain at least one lower case letter, one upper case letter, one digit, and one special character (@#$%^&+.,!=).";
			string passwordPopupMessageRegular = "Must be at least " + site.MinPasswordCharacterLength + " characters long.";
			
			string passwordPopupMessage = string.Empty;
			switch ((StrongPasswordUsage)site.StrongPasswordUse)
			{
				case StrongPasswordUsage.Enhanced:
					passwordPopupMessage = passwordPopupMessageEnhanced;
					break;
				case StrongPasswordUsage.Strong:
					passwordPopupMessage = passwordPopupMessageStrong;
					break;
				case StrongPasswordUsage.None:
					passwordPopupMessage = passwordPopupMessageRegular;
					break;
			}

			string passwordPopupBubbleScript =$@"
			<script type='text/javascript'>
            $(document).ready(function ()
                {{
                    $(""#PasswordPopupBubbleLabel"").attr(""title"",""{passwordPopupMessage}"");

							//debugger;

                    //create a bubble popup for each DOM element with class attribute as 'text', 'button' or 'link' and LI, P, IMG elements.
                    //$(""#PasswordPopupBubbleLabel"").tooltip(
                    //    {{
                    //        position: {{ my: ""center bottom"", at: ""center top"" }},
                    //        tooltipClass: ""PasswordPopupStyle"",
                    //        innerHtmlStyle: 
                    //        {{
                    //            color: '#000000',
                    //            'text-align':'center'
                    //        }},
                    //    }});
                }});
			</script>";
            this.Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "PasswordPopupBubble", passwordPopupBubbleScript);
        }
    }
}