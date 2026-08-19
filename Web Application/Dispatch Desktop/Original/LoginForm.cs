/// <summary>
/// 
/// File name:	LoginForm.cs
/// 
/// Purpose:	
/// 
/// Comments:	Copyright (C) Varec, Inc. Norcross, GA, USA, 2009 
///            This file shall not be copied or reproduced in any form 
///				without the express written consent of Varec, Inc.
///				
/// Author(s):	
/// 
/// Modification History:
///	Date:			By:					Reason:
///	----------	----------------	-----------------------------------------------
///	2009-09-25	I.Orndorff			- Modified "loginButton_Click()" to catch all responses from
///											  "sites.Login()". This addresses bug #7148.
///											  
///	2009-11-1   S. Jiang				- Modified for CAC Enabling											  
///											
/// </summary>

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using Interop.FMInterfaces;

using FMBusinessObjects.ChannelFactories;
using FMBusinessObjects.BusinessInterfaces;
using FMBusinessObjects.DataObjects;
using FMBusinessObjects.ServiceRequests;
using FMBusinessObjects.Exceptions;

namespace DispatchPrototype
{
	public partial class LoginForm : FMBaseForm
	{
		public bool thinInterface = true;
		private bool CACEnable = false;
		static System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();

		public LoginForm()
		{
			InitializeComponent();
			CenterToScreen();
		}

		private void loginButton_Click(object sender, EventArgs e)
		{
			try
			{
				// For Dispatch we want to force a direct hardware key read before login proceeds
				FMChannelFactory<IHardwareKey> hardwareKeyClient = new FMChannelFactory<IHardwareKey>();
				IHardwareKey hardwareKey = hardwareKeyClient.CreateProxy();

				hardwareKey.ReadHardwareKey();
				hardwareKey.ValidateHardwareKey();

				Configuration config = ConfigurationManager.OpenExeConfiguration("App_Dispatch");
				string site = config.AppSettings.Settings["Site"] == null ? null : config.AppSettings.Settings["Site"].Value;
				string sessionRefreshPeriod = config.AppSettings.Settings["SessionRefreshPeriod"] == null ? null : config.AppSettings.Settings["SessionRefreshPeriod"].Value;

				if (site == null)
				{
					throw new Exception("Site not in Application Configuration");
				}

				if (string.IsNullOrEmpty(sessionRefreshPeriod))
				{
					sessionRefreshPeriod = "3";
				}

				AppDomain appDomain = AppDomain.CurrentDomain;

				// Read the target customer from the .config file (IGO 2010-Sep-13)
				string targetcustomer = config.AppSettings.Settings["TargetCustomer"] == null ? null : config.AppSettings.Settings["TargetCustomer"].Value;
				if (string.IsNullOrEmpty(targetcustomer))
				{
					throw new Exception("TargetCustomer not in Application Configuration");
				}

				appDomain.SetData("TargetCustomer", targetcustomer);

				bool changePassword;
				int daysUntilExpiration;

				DateTimeFormatInfo dtformatinfo;

				FMChannelFactory<ISites> sitesClient = new FMChannelFactory<ISites>();
				ISites sites = sitesClient.CreateProxy();

				SecurityLoginRequest sr = new SecurityLoginRequest()
				{
					SiteID = site,
					UserID = userNameTextBox.Text,
					Password = passwordTextBox.Text
				};

				SecurityClass security;

				string token = sites.Login(out changePassword, out daysUntilExpiration, out security, sr);

				// catch all invalid logins from "sites.Login()". (IGO 2009-Sep-25)
				if ((token != null) && ((token.StartsWith("User") == true) || (token.ToUpper().StartsWith("LOGIN FAILED") == true)))
				{
					throw new Exception(token);
				}

				appDomain.SetData("Token", token);
				appDomain.SetData("Security", security);

				// after login get the date/time format information related to the site and store in the current domain app data (IGO 2010-Aug-13)
				SiteClass currentsite = sites.Get(security, security.SiteGuid, getMemberSites: false, getSchedulesAndProcessVariables: false, bGetAssociatedAliases: false);
				dtformatinfo = currentsite.GetDateTimeFormatInfo();

				appDomain.SetData("SiteDateTimeFormatInfo", dtformatinfo);

				// check if the user has dispatch rights
				if (security.HasRight(RIGHT.VIEW_DISPATCH) == false)
					throw new Exception("User Not Authorized to run Dispatch");

				// check that the dispatch option is enabled in the key
				uint Options = hardwareKey.GetOptionsCell();

				if ((Options & 0x1000) == 0)
					throw new Exception("Dispatch Not Authorized For This Computer");
				DialogResult dialogResult = DialogResult.Cancel;

				if (!CACEnable && (daysUntilExpiration <= 7))
					dialogResult = MessageBox.Show(this, "Your password will expire in " + daysUntilExpiration.ToString() +
											 " days. Click OK to change your password now, or Cancel to continue.", "Change Password", MessageBoxButtons.OKCancel);

				if (!CACEnable && (changePassword || dialogResult == DialogResult.OK))
				{
					this.Hide();
					ChangePasswordForm changePasswordForm = new ChangePasswordForm();
					dialogResult = changePasswordForm.ShowDialog(this);
					changePasswordForm.Close();

					if (dialogResult == DialogResult.Cancel)
					{
						Close();
						return;
					}
				}

				this.Hide();

				DispatchContainerForm.UserID = userNameTextBox.Text;
				DispatchContainerForm container = new DispatchContainerForm();

				SessionClass session = new SessionClass();
				session.ID = token;
				session.SiteID = site;
				session.SiteGuid = security.SiteGuid;
				session.UpdatedDate = DateTimeOffset.Now;
				session.UpdatedBy = "Dispatch - " + security.UserID;
				appDomain.SetData("Session", session);


				/* Adds the event and the event handler for the method that will 
				   process the timer event to the timer. */
				timer.Tick += new EventHandler(TimerEventProcessor);

				// Sets the timer interval to 60 seconds.
				timer.Interval = System.Convert.ToInt32(sessionRefreshPeriod) * 60000;
				timer.Start();


				container.ShowDialog(this);
				container.Close();
			}
			catch (FMHardwareKeyInvalidException except)
			{
				ErrorHandler(except);
			}
			catch (Exception exception)
			{
				MessageBox.Show(this, exception.Message, this.Text);
			}

			Close();
		}

		private void LoginForm_Load(object sender, EventArgs e)
		{
			CACComponentsClass CAC = new CACComponentsClass();
			string szUserID;
			if (CAC.CACEnable(out szUserID) == true)
			{
				szUserID = szUserID.Replace(",", ""); // strip apostrophes from CAC login
				userNameTextBox.Text = szUserID.Replace("'", ""); // strip apostrophes from CAC login
				//   bCACenable = true;
				loginButton.PerformClick();
			}
		}

		// This is the method to run when the timer is raised.
		private static void TimerEventProcessor(Object myObject,
												EventArgs myEventArgs)
		{
			timer.Stop();
			try
			{
				AppDomain appDomain = AppDomain.CurrentDomain;
				SessionClass session = appDomain.GetData("Session") as SessionClass;
				SecurityClass security = appDomain.GetData("Security") as SecurityClass;

				session.UpdatedDate = DateTimeOffset.Now;

				FMChannelFactory<ISessions> sessionsClient = new FMChannelFactory<ISessions>();
				ISessions sessions = sessionsClient.CreateProxy();

				sessions.Modify(security, session);
			}
			catch
			{
			}

			timer.Enabled = true;

		}

	}
}
