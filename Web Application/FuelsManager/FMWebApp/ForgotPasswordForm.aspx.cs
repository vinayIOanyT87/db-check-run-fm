// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ForgotPasswordForm.aspx.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the ForgotPasswordForm type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FuelsManager.FMWebApp
{
    using System;
    using System.Net.Mail;
    using FMBusinessObjects.BusinessInterfaces;
    using FMBusinessObjects.ChannelFactories;
    using FMBusinessObjects.Constants;
    using FMBusinessObjects.DataObjects;
    using System.Web;
    using System.Web.UI;

	using global::FMWebApp;

	/// <summary>
	///     This class is responsible for helping a user reset their password by providing
	///     a Reset Password button event handler.
	/// </summary>
	public partial class ForgotPasswordForm : FMFormBase
	{
        #region Attributes
        private string username = null;
        //private string siteId = null;
        private UserClass user = null;
        private bool showPasswordHintButton;

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Identifies the data dictionary keys needed for this page.
        /// </summary>
        /// <param name="security">
        ///     The current security object.
        /// </param>
        /// <returns>
        ///     An array of data dictionary keys.
        /// </returns>
        public string[] Keys(SecurityClass security)
		{
			string[] keys = { "Reset Password" };

			return keys;
		}

		#endregion

		#region Methods

		/// <summary>
		///     This method acts as the event-handler for clicking the Cancel button.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">
		///     The <see cref="System.EventArgs" /> instance containing the event data.
		/// </param>
		protected void CancelButton_Click(object sender, EventArgs e)
		{
            //this.Session["ForgotUserPaswordUserName"] = this.Request.QueryString["userId"];
            this.RedirectWithoutSecurity("UmnyangoForm.aspx?ShowForgotPassword=true&ShowPasswordHint=" + this.showPasswordHintButton + "&currentUserHint=" + this.Request.QueryString["currentUserHint"]);
		}

		/// <summary>
		///     This method is one in the chain of ASP.Net methods that are called during the page lifecycle.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void Page_Load(object sender, EventArgs e)
		{
		    try
		    {
                this.username = this.Request.QueryString["userId"];
                //this.siteId = this.Request.QueryString["siteId"];
                this.showPasswordHintButton = Convert.ToBoolean(this.Request.QueryString["ShowPasswordHintButton"]);

		        if (!string.IsNullOrEmpty(this.username))
		        {
		            //Setup security
		            this.Security = new SecurityClass();
		            this.Security.SiteGuid = Guids.SiteAdminGuid;
		            this.Security.AddRight(RIGHT.VIEW_SITES_AND_SITE_GROUPS);
		            this.Security.AddRight(RIGHT.MODIFY_SITES_AND_SITE_GROUPS);
		            this.Security.AddRight(RIGHT.VIEW_USERS);
		            this.Security.AddRight(RIGHT.MODIFY_USERS);
		            this.Security.AddRight(RIGHT.VIEW_USER_GROUPS);
		            this.Security.AddRight(RIGHT.MODIFY_USER_GROUPS);
		            this.Security.AddRight(RIGHT.VIEW_EQUIPMENT_DATA);
		            this.Security.AddRight(RIGHT.MODIFY_EQUIPMENT_DATA);

		            string serviceLogin = FMChannelHelper.MakeCall<IDBAccess, string>(x => x.ServiceLogin(this.Security));

		            this.Security.UserID = serviceLogin;
		            
		            UserCollectionClass users = null;
		            users =
		                FMChannelHelper.MakeCall<IUsers, UserCollectionClass>(
		                    x => x.GetUsersByIDWithoutSite(this.Security, this.username));

		            if (users.Count == 0)
		            {
		                var message =
		                    "User Name cannot be found in the system, please contact your FuelsManager administrator for help.";
		                this.PopupAlert(message, this.showPasswordHintButton);
		            }
		            else if (users.Count > 1)
		            {
		                var message = "User account is not unique, please contact your FuelsManager administrator for help.";
		                this.PopupAlert(message, this.showPasswordHintButton);
		            }
		            else
		            {
		                this.user = users[0];

		                if (string.IsNullOrEmpty(this.user.EmailAddress))
		                {
		                    var message = "No email address is attached to this user account, "
		                                  + "please contact your FuelsManager administrator for help.";
		                    this.PopupAlert(message, this.showPasswordHintButton);
		                }		                
		            }
		        }
		    }
		    catch (Exception ex)
		    {		        
		        this.ErrorHandler(ex);
		    }
        }

		/// <summary>
		///     This method acts as the event-handler for the Reset Password button.  It is the starting point for resetting a
		///     user's password.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void ResetPasswordButton_Click(object sender, EventArgs e)
		{
		    try
		    {
                //Generate a temporary password
                //At least 8 characters long, One upper case character, One number, One symbol
                string tempPassword =
                    FMChannelHelper.MakeCall<ITempPasswordGenerator, string>(x => x.GenerateTemporaryPassword(this.Security));
                //Send temporary password to user via email
                string mailMessage =
                    string.Format(
                        "A request was made to reset your FuelsManager password.  "
                        + "Your password has been reset to the following temporary password: \"{0}\".  You will be required "
                        + "to change your password at your next login. {1}{1}"
                        + "If you did not make this password reset request, please contact your FuelsManager administrator.",
                        tempPassword,
                        Environment.NewLine);

                //find a related site that could send email
                if (this.user != null)
		        {
		            SiteCollectionClass sites = null;
		            sites = FMChannelHelper.MakeCall<ISites, SiteCollectionClass>(
		                        x => x.EnumerateByUser(this.Security, this.user.IdentityGuid));

		            if (sites.Count == 0)
		            {
		                return;
		            }

		            bool sentMailSuccessfully = false;
		            bool mailServerConfigurationCorrect = false;
		            foreach (SiteClass site in sites)
		            {
		                if (!string.IsNullOrEmpty(site.MailServer) && !string.IsNullOrEmpty(site.MailFrom))
		                {
                            mailServerConfigurationCorrect = true;
                            this.Security.SiteGuid = site.SiteGuid;
                            //this user has all the attach userGroup functions, etc.
                            this.user =
                                FMChannelHelper.MakeCall<IUsers, UserClass>(
                                    x => x.Get(this.Security, this.user.IdentityGuid));

                            var mailClient = new SmtpClient(site.MailServer);
                            var mail = new MailMessage(
                                site.MailFrom,
                                this.user.EmailAddress,
                                "FuelsManager Temporary Password",
                                mailMessage);
		                    try
		                    {
		                        mailClient.Send(mail);
		                    }
		                    catch (Exception)
		                    {
		                        continue;
		                    }
                            //Force the user to reenter a new password upon next login
                            this.user.Password = tempPassword;
                            this.user.ChangePassword = true;
                            FMChannelHelper.MakeCall<IUsers>(x => x.Modify(this.Security, this.user));
                            this.PopupMessageBoxOnSucceed("Your temporary password has been sent successfully!");
                            sentMailSuccessfully = true;
		                    break;
		                }
		            }

		            if (mailServerConfigurationCorrect == false)
		            {
                        var message = "Sending mail failed due to System Configuration, "
                                    + "please contact your FuelsManager administrator for help.";
                        this.PopupAlert(message, this.showPasswordHintButton);
                    }

		            if (sentMailSuccessfully == false)
		            {
                        string errorMessage = "Sending mail failed, please contact your FuelsManager administrator for help.";
                        this.PopupAlert(errorMessage, this.showPasswordHintButton);
                    }
		        }                                            		                    	        
		    }
		    catch (Exception ex)
		    {
		        this.ErrorHandler(ex);
		    }
        }

        /// <summary>
		///  Popup error message and redirect back to the login page.
		/// </summary>
		/// <param name="message"></param>
		/// <param name="ifShowPasswordHint"></param>
        public void PopupAlert(string message, bool ifShowPasswordHint)
        {           
            string alertString = "<script type=\"text/javascript\">\r\n<!--\r\n";
            alertString += "alert(\"" + HttpUtility.JavaScriptStringEncode(message) + "\");";
            alertString += "window.location ='UmnyangoForm.aspx?ShowForgotPassword=true&ShowPasswordHint="+ifShowPasswordHint+"'; ";
            alertString += "\r\n--></script>";

            ScriptManager.RegisterClientScriptBlock(
                this.Page,
                this.GetType(),
                "SendingTempPasswordEmailError",
                alertString,
                false);
        }

        /// <summary>
		///  Popup message box and redirect back to the login page.
		/// </summary>
		/// <param name="message"></param>
        public void PopupMessageBoxOnSucceed(string message)
        {
            string infoString = "<script type=\"text/javascript\">\r\n<!--\r\n";
            infoString += "alert(\"" + HttpUtility.JavaScriptStringEncode(message) + "\");";
            infoString += "window.location ='UmnyangoForm.aspx'; ";
            infoString += "\r\n--></script>";

            ScriptManager.RegisterClientScriptBlock(
                this.Page,
                this.GetType(),
                "SendingTempPasswordEmailError",
                infoString,
                false);
        }
        #endregion
    }
}