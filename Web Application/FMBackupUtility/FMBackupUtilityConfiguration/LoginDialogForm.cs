using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using FMBusinessObjects.DataObjects;
using FMBusinessObjects.BusinessInterfaces;
using FMBusinessObjects.ChannelFactories;
using FMBusinessObjects.ServiceRequests;

namespace FMBackupUtilityConfiguration
{
	public partial class LoginDialogForm : Form
	{
		private bool bLogin = false;
		public String strWhere;

		public LoginDialogForm ( )
		{
			InitializeComponent ( );
		}

		public bool IsLoggedIn
		{
			get { return bLogin; }
		}

		private void btOK_Click ( object sender, EventArgs e )
		{
			Login ( );

			if (bLogin == false)
			{
				this.tbUserName.Clear ( );
				this.tbPassword.Clear ( );
				this.tbUserName.Focus ( );
			}
		}

		private void Login ( )
		{
			try
			{

				bool changePassword = false;
				int daysUntilExpiration = 999;
				string token = null;

				var loginRequest = new SecurityLoginRequest ( );
				loginRequest.SiteID = "siteadmin";
				loginRequest.UserID = this.tbUserName.Text.Trim ( );
				loginRequest.Password = this.tbPassword.Text.Trim ( );


				SecurityLoginResponse loginResult =
					FMChannelHelper.MakeCall<ISites, SecurityLoginResponse>(
						x => x.Login2(loginRequest));

				if (loginResult != null)
				{
					if (loginResult.Result != null)
					{
						token = loginResult.Result;
					}

					changePassword = loginResult.ChangePassword;
					daysUntilExpiration = loginResult.DaysUntilExpiration;
				}

				// For invalid logins, the app must update the User table with the number of invalid
				// attempts. Therefore, in order to persist the update to the user table an exception
				// cannot be throw so the return value is set to error message which starts is "User".
				if (( token != null ) && ( ( token.StartsWith ( "User" ) == true ) || ( token.ToUpper ( ).StartsWith ( "LOGIN FAILED" ) == true ) ))
				{
					bLogin = false;
					// base.ErrorHandler(new Exception(token));
					MessageBox.Show ( token, "Login", MessageBoxButtons.OK, MessageBoxIcon.Error );
				}
				else
				{

					if (changePassword) //SJiang: Do not check if user login using CAC
					{
						String strMsg;
						strMsg = "Your password must be changed before it may be used. You must change your password in the FuelsManager Defense application. ";
						MessageBox.Show ( strMsg, "Login", MessageBoxButtons.OK, MessageBoxIcon.Error );
						bLogin = false;
					}
					else
					{
						// Display expiration warning to user if within 7 days. (IGO 2009-Aug-11)
						if (daysUntilExpiration <= 7)  //SJiang: Do not check if user login using CAC
						{
							//Get user confirmation if the transaction is associated to other transactions.
							String strMsg;
							strMsg = "Your Password will expire in " + daysUntilExpiration.ToString ( ) +
												  " days.\\n You can change your password in the FuelsManager Defense application. ";
							MessageBox.Show ( strMsg, "Login", MessageBoxButtons.OK, MessageBoxIcon.Information );
						}
						bLogin = true;
						this.Close ( );
					}
				}
			}
			catch (Exception exception)
			{
				//   ErrorHandler(exception);
				MessageBox.Show ( exception.Message, "Login", MessageBoxButtons.OK, MessageBoxIcon.Error );
				bLogin = false;
			}
		}

	}
}