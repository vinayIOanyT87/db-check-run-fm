namespace FuelsManager.FMReportWebMain
{
	using System;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.DataObjects;

	public class ManageSecurity
	{
		#region private attributes
		private bool securityValid;
		private SecurityClass security;
		private ISites sites;
		#endregion

		#region Constructors
		/// <summary>
		/// This is the default constructor for the ManageSecurity object.
		/// </summary>
		public ManageSecurity ( )
		{
			this.securityValid = true;
			this.security = null;
			this.sites = null;
		}
		#endregion

		#region Properties
		/// <summary>
		/// This property returns true if the security is valid. Otherwise,
		/// it returns false.
		/// </summary>
		public bool IsSecurityValid
		{
			get { return this.securityValid; }
		}

		/// <summary>
		/// This property returns the site Guid.
		/// </summary>
		public Guid SiteGuid
		{
			get { return this.security.SiteGuid; }
		}

		/// <summary>
		/// This property returns login site guid.
		/// </summary>
		public Guid LoginSiteGuid
		{
			get { return this.security.LoginSiteGuid; }
		}

		/// <summary>
		/// This property returns site ID string.
		/// </summary>
		public string SiteID
		{
			get { return this.security.SiteID; }
		}

		/// <summary>
		/// This property returns user Guid.
		/// </summary>
		public Guid UserGuid
		{
			get { return this.security.UserGuid; }
		}

		/// <summary>
		/// This property returns the security class.
		/// </summary>
		public SecurityClass Security
		{
			get { return this.security; }
		}

		/// <summary>
		/// This property returns the sites class.
		/// </summary>
		public ISites Sites
		{
			get { return this.sites; }
		}
		#endregion

		#region Public Methods
		/// <summary>
		/// This method extracts the security token from the request and creates
		/// a FM security object. If the security is not valid, then the security valid
		/// flag is set to false.
		/// </summary>
		/// <param name="request"></param>
		public void BuildSecurity ( SecurityClass security )
		{
			// Display an error and transfer control to the report error page if there
			// is not a valid token.
			if (security == null)
			{
				this.securityValid = false;
			}
			else
			{
				this.security = security;
			}
		}
		#endregion
	}
}
