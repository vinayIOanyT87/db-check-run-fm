using System;
using System.Collections;
using System.ComponentModel;
using System.Web;
using System.Web.SessionState;

using FMCommon;
using ConsolidatedDataObjects;
using ConsolidatedBLL;
using FM7Accounting;

namespace StandardXMLImportExport 
{
	/// <summary>
	/// Summary description for Global.
	/// </summary>
	public class Global : System.Web.HttpApplication
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		public Global()
		{
			InitializeComponent();
		}	
		
		protected void Application_Start(Object sender, EventArgs e)
		{

		}
 
		protected void Session_Start(Object sender, EventArgs e)
		{
		}

		protected void Application_BeginRequest(Object sender, EventArgs e)
		{

		}

		protected void Application_EndRequest(Object sender, EventArgs e)
		{

		}

		protected void Application_AuthenticateRequest(Object sender, EventArgs e)
		{

		}

		protected void Application_Error(Object sender, EventArgs e)
		{

		}

		protected void Session_End(Object sender, EventArgs e)
		{

		}

		protected void Application_End(Object sender, EventArgs e)
		{

		}
			
		SecurityClass GetSecurity()
		{
			int currentSiteIndex   = -1;


			// Retrieve the site index from the HTTP cookie.  This index is used to set
			// the site index within the security object.
			if (Session["SiteIndex"] != null)
			{
				string siteIndexStr = Session["SiteIndex"].ToString();

				if ((siteIndexStr != null) && (siteIndexStr.Length > 0))
					currentSiteIndex = Convert.ToInt32(siteIndexStr);
			}

			// Create a new site class that will be used to retrieve the security class.
			SitesClass sites = new SitesClass();

	
			// Use the token retrieved from the cookie in order to retrieve 
			// the security class for a given site.  Add the security class 
			// to the session.
			SecurityClass security = sites.GetSecurity(Session["Token"] as string);

			// Setup a default security object since the real one
			// could not be found.
			if (security == null)
				security = new SecurityClass();

			security.SiteIndex = currentSiteIndex;
			Session.Add("Security", security);
			return security;
		}
		
		#region Web Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{    
			this.components = new System.ComponentModel.Container();
		}
		#endregion
	}
}

