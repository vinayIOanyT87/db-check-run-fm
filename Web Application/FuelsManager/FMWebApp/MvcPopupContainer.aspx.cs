namespace FuelsManager.FMWebApp
{
	using System;
	using System.Web.UI;

	using FMCore;

	using global::FMWebApp;

	public partial class MvcPopupContainer :  FMFormBase
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			try
			{
				var target = this.Request.GetQueryOrFormValue("target");

				if (string.IsNullOrEmpty(target))
				{
					throw new ArgumentNullException("target");
				}

				string parmSymbol = "?";

				if (target.IndexOf("?", StringComparison.Ordinal) >= 0)
				{
					parmSymbol = "&";
				}

				target = target + parmSymbol + "CSRFToken=" + this.Session["CSRFToken"];
				var iframe = string.Format("<iframe id='iframeContent' src='{0}' style='border: none; overflow-x:hidden;' title='iframe' onload='iframeLoaded()'></iframe>", target);

				this.content.Controls.Add(new LiteralControl(iframe));
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}
	}
}