// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Varec, Inc." file="FMFormBaseAjax.cs">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the FMFormBaseAjax type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FuelsManager.FMWebApp
{
	using System;
	using System.Web;
	using System.Web.UI;

	/// <summary>
	/// Base page to support pages that use ajax.
	/// </summary>
	public class FMFormBaseAjax : FMFormBase
	{
		#region Methods

		/// <summary>
		/// Redirects navigation back to the login page after session timeout.
		/// </summary>
		protected override void RedirectAfterSessionTimeout()
		{
			if (ScriptManager.GetCurrent(this) != null)
			{
				this.Redirect("../FMWebApp/SessionTimeout.htm");
				this.Context.ApplicationInstance.CompleteRequest();
			}
			else
			{
				base.RedirectAfterSessionTimeout();
			}
		}

		/// <summary>
		/// Renders the error message.
		/// </summary>
		/// <param name="errorMessage">The error message.</param>
		protected override void RenderErrorMessage(string errorMessage)
		{
			if (ScriptManager.GetCurrent(this) != null)
			{
				Guid guid = Guid.NewGuid();
				string message = "alert(\"" + HttpUtility.JavaScriptStringEncode(errorMessage) + "\");";

				ScriptManager.RegisterStartupScript(this, this.GetType(), guid.ToString(), message, true);
			}
			else
			{
				base.RenderErrorMessage(errorMessage);
			}
		}

		#endregion
	}
}
