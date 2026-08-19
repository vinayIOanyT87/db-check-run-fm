// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FMFormBase.Redirect.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the FMFormBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

// --------------------------------------------------------------------------------------------------------------------
// This file is marked as skip in ReSharper.  Be careful making changes.  
// Also, do not add additional methods here.
// --------------------------------------------------------------------------------------------------------------------

namespace FuelsManager.FMWebApp
{
	using System;

	public partial class FMFormBase
	{
        /// <summary>
		/// This method handles redirection without Security, designed for Reset Password.
		/// Currently in FuelsManager, only 5 pages allows to bypass security, see Global.asax.cs
		/// </summary>
		/// <param name="url">The url to which to redirect.</param>
	    public void RedirectWithoutSecurity(string url)
	    {
            try
            {
                this.Response.Redirect(url, endResponse: false);
                this.Context.ApplicationInstance.CompleteRequest();
            }
            catch (Exception ex)
            {
                LogErrorMessage(ex.Message);
            };                     
        }
		/// <summary>
		/// This method handles redirection properly to avoid non-obvious problems
		/// we have encountered with redirection on FuelsManager Pages.
		/// </summary>
		/// <param name="url">The url to which to redirect.</param>
		public void Redirect( string url )
		{
			try
			{
				if (url.Contains("CSRFToken=") == false)
				{
					FMBusinessObjects.DataObjects.SecurityClass security = (FMBusinessObjects.DataObjects.SecurityClass)this.Session["Security"]; 
					if (url.Contains("?"))
					{
						url += "&" + security.CSRFTokenWithParamName;
					}
					else
					{
						url += "?" + security.CSRFTokenWithParamName;

					}
				}

				if (url.Contains("ClientDispatch=") == false && IsFromClientDispatch)
				{
					if (url.Contains("?"))
					{
						url += "&ClientDispatch=true";
					}
					else
					{
						url += "?ClientDispatch=true";

					}
				}
				this.Server.ClearError();
				this.Response.Redirect( url, endResponse: false );
				this.Context.ApplicationInstance.CompleteRequest();
			}
			catch ( Exception ex )
			{
				LogErrorMessage( ex.Message );
			}
		}
	}
}
