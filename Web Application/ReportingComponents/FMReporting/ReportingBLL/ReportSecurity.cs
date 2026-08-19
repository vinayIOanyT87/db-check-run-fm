/// <summary> =================================================================
///
///	FILE NAME:	ReportSecurity.cs
///
///	PURPOSE:		Declaration of the ReportSecurity class
///
///		Copyright (C) 1999-2009	      Varec, Inc.          All Rights Reserved
///										      Norcross, GA, USA
///
///		This file shall not be copied or reproduced in any form without the
///		express written consent of Varec.
///
///		Date:			By:						Reason:
///		---------	-----------------		-----------------------------------------------------------------------------
///		2009-01-01	Richard R. Panachida	7.5.0.13		Initial Creation.
///		
///		2009-08-11	I.Orndorff				- Added DaysUntilExpiration parameter to "Sites.Login()". This addresses task #5267.
///
/// </summary> ================================================================

using ConsolidatedBLL;
using FMCommon;
using Microsoft.Win32;


namespace ReportingBLL
{
	public class ReportSecurity
	{
		#region Attributes
		private string securityToken;
		private int currentSiteIndex;
		private bool useDataDictionary;
		private const int EMPTY_STRING = 0;
		private SecurityClass security;
		#endregion

		#region Constructors
		/// <summary>
		/// This is the default constructor for the report security class.
		/// </summary>
		public ReportSecurity()
		{
			this.securityToken = "";
			this.currentSiteIndex = -1;

			this.BuildSecurity();
		}

		/// <summary>
		/// This is the recommended constructor to use.  It sets up the
		/// class to its initial state.
		/// </summary>
		/// <param name="securityToken"></param>
		/// <param name="siteIndex"></param>
		public ReportSecurity(string securityToken, int siteIndex, bool useDataDictionary)
		{
			if ((securityToken == null) || (securityToken.Length == EMPTY_STRING))
				this.securityToken = "";
			else
				this.securityToken = securityToken;

			if ((siteIndex == 0) || (siteIndex < -1) || (siteIndex > 30))
				this.currentSiteIndex = -1;
			else
				this.currentSiteIndex = siteIndex;

			// Data dictionary not being used at this time.
			this.useDataDictionary = useDataDictionary;

			this.BuildSecurity();
		}
		#endregion

		#region Private Methods
		/// <summary>
		/// This method will build the security information using the security
		/// token and the site index.
		/// </summary>
		private void BuildSecurity()
		{
			// Create a new site class that will be used to retrieve the security class.
			SitesClass sites = new SitesClass();
            SecurityClass localSecurity;

			if ((this.securityToken.Length == EMPTY_STRING) && (this.InDevMode() == true))
			{
				// ChangePassword is not used in this application.
				bool changePassword;
				int DaysUntilExpiration = 999; 
				this.securityToken = sites.Login("SiteAdmin", "Administrator", "marietta", out changePassword, out DaysUntilExpiration, out localSecurity);
                this.security = localSecurity;
			}

			if (this.securityToken.Length != EMPTY_STRING)
			{
				// Use the token retrieved from the cookie in order to retrieve 

				// the security class for a given site.  Add the security class 
				// to the session.
				this.security = sites.GetSecurity(this.securityToken);

				// Setup a default security object since the real one
				// could be found.
				if (this.security == null)
				{
					this.security = new SecurityClass();
					this.security.SiteIndex = this.currentSiteIndex;
				}

				this.currentSiteIndex = this.security.SiteIndex;
			}
		}

		/// <summary>
		/// This method returns true if the application is in development mode. Otherwise,
		/// it returns false.
		/// </summary>
		/// <returns></returns>
		private bool InDevMode()
		{
			bool devMode = false;
			string valueString = "Mode";
			string mode = "";
			bool writeable = false;

			System.Security.Permissions.RegistryPermission regPermissions = new
			System.Security.Permissions.RegistryPermission(System.Security.Permissions.RegistryPermissionAccess.AllAccess,
			"HKEY_LOCAL_MACHINE\\SOFTWARE\\FuelsManager\\Reporting");

			regPermissions.Assert();
			RegistryKey key = Registry.LocalMachine.OpenSubKey("Software\\FuelsManager\\Reporting", writeable);

			if (key != null)
				mode = (string) key.GetValue(valueString);

			System.Security.Permissions.RegistryPermission.RevertAssert();

			if ((mode != null) && (mode.Length > 0))
			{
				if (mode.ToUpper() == "DEV")
					devMode = true;
			}

			return devMode;
		}
		#endregion

		#region Properties
		/// <summary>
		/// This property gets the security object.
        /// 
        /// Set added 2009-09-29 CHK as it's needed for DESC security
		/// </summary>
		public SecurityClass Security
		{
			get { return this.security; }
            set { this.security = value; }
		}

		/// <summary>
		/// This property will retrieve the user's ID from the security object.
		/// </summary>
		public string UserID
		{
			get { return this.security.UserID; }
		}
		#endregion
	}
}
