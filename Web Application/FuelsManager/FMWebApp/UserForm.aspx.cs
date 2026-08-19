/******************************************************************************
	FILE NAME:		UserForm.aspx.cs
	PURPOSE:		Implementation of UserForm

	COMMENTS:
		Copyright (C) E+H Systems & Gauging, Inc. Norcross, GA, USA, 2002
		This file shall not be copied or reproduced in any form without
		the express written consent of Endress+Hauser.


	AUTHOR(S):	W. Gray
	VERSION:	1.0.0  Current version

	MODIFICATION HISTORY:
		Date:			By:					Reason:
		----------	-----------------	-------------------------------------------
		08/15/2005	W.Gray				7.0.0.30 - Changed to populate UnassignedGroupsListBox
										if user has View Users or View User Groups right 
		2007-08-20	Richard Panachida	CSI 5063 - Updated to handle the new Password security.
		2007-09-07	Richard Panachida	CSI 5098 - Add new Password security for inactivity usage.
		03-05-2008	B. Schaal			CSI 5515 - Added code that will set the value PasswordLockoutCount at 9999
												when the user is manually lock out. Modified the lockout check code to return true
												if a value of 9999 is passed in.
		03-12-2009	B. Schaal			Changed error message for the strong Password failure since the message was incomplete.

		2009-04-03  G.Kendall			WI# 2733 Updated checks to make strong Password check 
												independent of other Password settings
		2009-07-10  A. Coker			Replaced 9999 used for indicating manual lock out with UserClass.MANUAL_LOCKOUT. 

		2009-09-17	I.Orndorff			- Modified "OK_Command()" to rehash user password before adding it to session.
 * 
 *		20009-09-30 C. Knight			- Modified OK_Command to no longer hash user password - WI 6214
		2009-12-02	S.Jiang				Update for Dormant Accounts Management
 *********************************************************************************/

namespace FuelsManager.FMWebApp
{
	using System;
	using System.Configuration;
	using System.Web.UI.WebControls;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.UtilityObjects;

	using FMControls;

	/// <summary>
	/// Summary description for UserForm.
	/// </summary>
	public partial class UserForm : FMAutoSubmitFormBase
	{
		public static readonly string SessionKeyOldPassword = "OldPassword";
		public static readonly string SessionKeyUserGuid = "UserGuid";
		private static readonly string SessionKeyUserEdit = "UserEditObject";

		public static string UserFormUrl
		{
				get
				{
					string userFormURL = ConfigurationManager.AppSettings["UserFormURL"];
					if (string.IsNullOrEmpty(userFormURL))
					{
						userFormURL = "FMWebApp/UserForm.aspx";
					}
					return "../" + userFormURL;
				}
		}

		protected Image Image2;

		protected Image Image3;

		public UserClass FMUser { get; set; }

		public FMButton OkButton
		{
			get
			{
				return this.OK;
			}
		}

		protected void Page_Load(object sender, EventArgs e)
		{
			try
			{
				this.GetSecurity();

				UserClass user;

				if (!this.Page.IsPostBack)
				{

					// Get IdentityGuid
					if (this.Session[SessionKeyUserGuid] is Guid)
					{
						user = FMChannelHelper.MakeCall<IUsers, UserClass>(x => x.Get(this.Security, (Guid)this.Session[SessionKeyUserGuid]));
					}
					else
					{
						user = new UserClass();
					}

					FMUser = user;

					this.Session[SessionKeyUserEdit] = FMUser;
					this.Session[SessionKeyOldPassword] = FMUser.Password;
				}
				else
				{
					if (this.Session[SessionKeyUserEdit] is UserClass)
					{
						FMUser = this.Session[SessionKeyUserEdit] as UserClass;
					}
					else
					{ 
						throw new Exception("User not in Session");
					}
				}

				//Set the title label with a key field from the bound object appended
				this.UserTitleLabel.Text = this.GetTitleLabelText(this.UserTitleLabel.Text, FMUser.ID);

				if (!this.Security.HasRight(RIGHT.MODIFY_USERS) || FMUser.SiteGuid.IsNotEmptyAndNotEqualTo(this.Security.SiteGuid))
				{
					this.OK.Enabled = false;
				}

				this.tpUserUserDataPage.HeaderText = this.GetTranslatedText("User Data");
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		#region Web Form Designer generated code
		override protected void OnInit(EventArgs e)
		{
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			this.InitializeComponent();
			base.OnInit(e);
		}

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.OK.Command += new System.Web.UI.WebControls.CommandEventHandler(this.OkCommand);
			this.Cancel.Command += new System.Web.UI.WebControls.CommandEventHandler(this.CancelCommand);
		}

		#endregion

		/// <summary>
		/// This method handles the OK button event. It will update the user's configuration items.
		/// In addition, it will perform validation on the Password to ensure that it meets the 
		/// security settings.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OkCommand(object sender, CommandEventArgs e)
		{
			if (this.SaveUser())
			{
				this.TransferToOriginatingForm();
			}
		}

		public bool SaveUser()
		{
			try
			{
				bool newUser = false;

				if (this.Session[SessionKeyUserGuid] != null)
				{
					this.FMUser = FMChannelHelper.MakeCall<IUsers, UserClass>(
												x =>
												x.Get(this.Security, (Guid)this.Session[SessionKeyUserGuid])
									);
				}
				else
				{
					this.FMUser = new UserClass();
					newUser = true;
				}

				this.UserGeneralPage.UpdateData();
				this.UserUserDataPage.UpdateData();

				if (newUser == false)
				{
					// Ignore the Password history update if the Password text box has not been changed.
					// Otherwise, update the history since the user entered in a Password.
					if (UserGeneralPage.PasswordNotChanged)
					{
						FMChannelHelper.MakeCall<IUsers>(x => x.Modify(this.Security, this.FMUser));
					}
					else
					{
						string oldPassword = this.Session[SessionKeyOldPassword] as string;
						if (oldPassword == null)
						{
							oldPassword = string.Empty;
						}
						FMChannelHelper.MakeCall<IUsers>(x => x.ModifyWithPasswordHistory(this.Security, this.FMUser, oldPassword));

						if ((this.Security.UserID == this.FMUser.ID) && (this.Security.SiteGuid == this.FMUser.SiteGuid))
						{
							this.Security.Password = this.FMUser.Password;
							this.Session["Security"] = this.Security;
						}
					}

					if (UserGeneralPage.UserGroupChanged)
					{
						this.Session["Security"] = FMChannelHelper.MakeCall<ISites, SecurityClass>(x => x.GetSecurity(this.Security.Token.ToString()));
					}
				}
				else
				{
					Guid newUserGuid = FMChannelHelper.MakeCall<IUsers, Guid>(x => x.Add(this.Security, this.FMUser));
					this.Session[SessionKeyUserGuid] = newUserGuid;
				}

				return true;
			}
			catch (Exception except)
			{
				this.UserGeneralPage.ClearPassword();


				this.ErrorHandler(except);
				return false;
			}
		}

		private void TransferToOriginatingForm()
		{
			if (this.IsFromQueryWriter)
			{
				this.Redirect("..\\QueryWriterWebApp\\QueryResultsForm.aspx?Mode=Returning");
			}
			else if (this.IsFromDispatch)
			{
				// Return to dispatching view
				this.Redirect("../DispatchWebApp/DispatchingView.aspx");
			}
			else if (this.Session["UserSelectSearchString"] == null)
			{
				this.Redirect("UsersForm.aspx");
			}
			else
			{
				string transferString = "UserSelectForm.aspx?";
				var userSelectSearchString = (string)this.Session["UserSelectSearchString"];

				if (userSelectSearchString != null)//&& UserSelectSearchString != String.Empty)
				{
					transferString += "SearchString=" + userSelectSearchString + "&";
				}

				this.Redirect(transferString);
			}
		}


		private void CancelCommand(object sender, CommandEventArgs e)
		{
			this.TransferToOriginatingForm();
		}
	}
}
