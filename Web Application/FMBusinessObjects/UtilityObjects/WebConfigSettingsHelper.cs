using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Collections.Specialized;
using System.Web.Configuration;

namespace FMBusinessObjects.UtilityObjects
{
	static public class WebConfigSettingsHelper
	{
		//static private bool ?secureCookieEnabled = null;

		/// <summary>
		/// Indicates if secure cookies can be used. 
		/// If sessionState  cookieless attribute is false and secure cookies are enabled (httpOnly and requireSSL for httpCookies are set to true), then it returns true.
		/// </summary>
		/// <returns>bool</returns>
		//static public bool CanUseSecureCookies
		//{
		//	get
		//	{
		//		try
		//		{
		//			if (secureCookieEnabled == null)
		//			{
		//				var httpCookiesSection = HttpCookies;
		//				var sessionStateSection = SessionState;
		//				secureCookieEnabled = (httpCookiesSection != null && httpCookiesSection.HttpOnlyCookies == true && httpCookiesSection.RequireSSL == true);
		//				if (sessionStateSection != null && sessionStateSection.Cookieless != System.Web.HttpCookieMode.UseCookies)
		//					secureCookieEnabled = false;

		//			}

		//		}
		//		catch
		//		{
		//			return false;

		//		}
		//		return (secureCookieEnabled == true);
		//	}
		//}

		/// <summary>
		/// Retrieves sessionState setting from fuelsmanager web.config.
		/// </summary>
		/// <returns>SessionStateSection</returns>
		static public SessionStateSection SessionState
		{
			get 
			{
				try
				{
					return ConfigurationManager.GetSection("system.web/sessionState") as SessionStateSection;

				}
				catch
				{
					return null;

				}
			}
		}

		/// <summary>
		/// Retrieves httpCookies setting from fuelsmanager web.config.
		/// </summary>
		/// <returns>HttpCookiesSection</returns>
		static public HttpCookiesSection HttpCookies
		{
			get 
			{
				try
				{
					return ConfigurationManager.GetSection("system.web/httpCookies") as HttpCookiesSection;

				}
				catch
				{
					return null;

				}
			}
		}
	}
}
