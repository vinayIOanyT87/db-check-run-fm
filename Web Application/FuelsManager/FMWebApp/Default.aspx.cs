namespace FuelsManager.FMWebApp
{
	using System;
	using System.Web;

	public partial class _default : System.Web.UI.Page
	{

		protected void Page_Load(object sender, EventArgs e)
		{
			var p = HttpRuntime.AppDomainAppVirtualPath + "/FMWebApp/FuelsManagerForm.aspx";
			Response.Redirect(p, false);
		}

	}
}