namespace Dispatch
{
	using System;
	using System.Configuration;
	using System.Globalization;
	using System.Windows.Forms;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.Exceptions;
	using FMBusinessObjects.ServiceRequests;

	using FMDispatchBusinessObjects.BusinessInterfaces;
	using FMDispatchBusinessObjects.ChannelFactories;

	using Interop.FMInterfaces;

	public partial class LoginForm : FMBaseForm
	{
		public bool ThinInterface = true;
		private bool cacEnable;
		static readonly Timer Timer = new Timer();

		public LoginForm()
		{
			this.InitializeComponent();
			this.CenterToScreen();
		}

		private void LoginButtonClick(object sender, EventArgs e)
		{
			try
			{
				// For Dispatch we want to force a direct hardware key read before login proceeds
				FMChannelHelper.MakeCall<IClientDispatchService>(x => x.ReadHardwareKey());

				string site = ConfigurationManager.AppSettings["Site"];

				if (string.IsNullOrEmpty(site))
				{
					throw new Exception("Site not in Application Configuration");
				}

				string sessionRefreshPeriod = ConfigurationManager.AppSettings["SessionRefreshPeriod"];

				if (string.IsNullOrEmpty(sessionRefreshPeriod))
				{
					sessionRefreshPeriod = "3";
				}

				AppDomain appDomain = AppDomain.CurrentDomain;

				bool changePassword = false;
				int daysUntilExpiration = 0;
				string token = "";
				SecurityClass security = null;

				var sr = new SecurityLoginRequest
							 {
								 CACEnabled = this.cacEnable,
								 Password = this.passwordTextBox.Text,
								 SiteID = site,
								 TimeOut = Convert.ToInt32(sessionRefreshPeriod) + 1,
								 UserID = this.userNameTextBox.Text
							 };

				SecurityLoginResponse loginResult =
					FMChannelHelper.MakeCall<IClientDispatchService, SecurityLoginResponse>(
						x => x.Login(sr));

				// catch all invalid logins from "sites.Login()". (IGO 2009-Sep-25)
				if ((loginResult != null && loginResult.Result != null)
					&& (loginResult.Result.StartsWith("User") || loginResult.Result.ToUpper().StartsWith("LOGIN FAILED")))
				{
					throw new Exception(loginResult.Result);
				}

				if (loginResult.Security == null)
				{
					throw new Exception(loginResult.Result);
				}

				if (string.IsNullOrEmpty(loginResult.Result))
				{
					token = loginResult.Security.Token.ToString();
				}


				security = loginResult.Security;
				daysUntilExpiration = loginResult.DaysUntilExpiration;
				changePassword = loginResult.ChangePassword;

				appDomain.SetData("Security", security);
				appDomain.SetData("Token", token);

				var dialogResult = DialogResult.Cancel;

				if (!this.cacEnable)
				{

					if (!changePassword && (daysUntilExpiration <= 7))
						dialogResult = MessageBox.Show(this, "Your password will expire in " + daysUntilExpiration +
														 " days. Click OK to change your password now, or Cancel to continue.", 
														 "Change Password", MessageBoxButtons.OKCancel);

					if (changePassword || dialogResult == DialogResult.OK)
					{
						this.Hide();

						var changePasswordForm = new ChangePasswordForm();
						dialogResult = changePasswordForm.ShowDialog(this);
						string strNewPassword = changePasswordForm.newPasswordTextBox.Text;
						changePasswordForm.Close();

						if (dialogResult == DialogResult.Cancel)
						{
							this.Close();
							return;
						}

						// security, changepassword has to be reset
						security.Password = strNewPassword;
						appDomain.SetData("Security", security);
					}
				}

				this.Hide();

				// after login get the date/time format information related to the site and store in the current domain app data (IGO 2010-Aug-13)
				SiteClass currentsite =
					FMChannelHelper.MakeCall<IClientDispatchService, SiteClass>(x => x.GetSite(security, security.SiteGuid));

				DateTimeFormatInfo dtformatinfo = currentsite.GetDateTimeFormatInfo();
				appDomain.SetData("SiteDateTimeFormatInfo", dtformatinfo);

				// check if the user has dispatch rights
				if (security.HasRight(RIGHT.VIEW_DISPATCH) == false)
				{
					throw new Exception("User Not Authorized to run Dispatch");
				}

				DispatchContainerForm.UserID = this.userNameTextBox.Text;
				var container = new DispatchContainerForm();

				/* Adds the event and the event handler for the method that will 
				   process the timer event to the timer. */
				Timer.Tick += TimerEventProcessor;

				// Sets the timer interval to 60 seconds.
				Timer.Interval = Convert.ToInt32(sessionRefreshPeriod) * 60000;
				Timer.Start();

				container.ShowDialog(this);
				container.Close();
			}
			catch (FMHardwareKeyInvalidException except)
			{
				this.ErrorHandler(except);
			}
			catch (Exception exception)
			{
				MessageBox.Show(this, exception.Message, this.Text);
			}

			this.Close();
		}

		private void LoginFormLoad(object sender, EventArgs e)
		{
			var cac = new CACComponentsClass();
			string szUserID;

			if (cac.CACEnable(out szUserID))
			{
				szUserID = szUserID.Replace(",", string.Empty); // strip apostrophes from CAC login
				this.userNameTextBox.Text = szUserID.Replace("'",string.Empty); // strip apostrophes from CAC login

				//   bCACenable = true;
				this.cacEnable = true;
				this.loginButton.PerformClick();
			}
		}

		// This is the method to run when the timer is raised.
		private static void TimerEventProcessor(Object myObject,
												EventArgs myEventArgs)
		{
			Timer.Stop();
			UpdateSession();
			Timer.Enabled = true;
		}

		public static void UpdateSession()
		{
			try
			{
				AppDomain appDomain = AppDomain.CurrentDomain;
				var security = appDomain.GetData("Security") as SecurityClass;

				FMChannelHelper.MakeCall<IClientDispatchService>(x => x.PingSession(security));
			}
			catch
			{
			}
		}
	}
}
