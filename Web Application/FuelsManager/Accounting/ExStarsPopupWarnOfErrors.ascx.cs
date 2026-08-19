namespace FuelsManager.Accounting
{
	using System;

	public partial class ExStarsPopupWarnOfErrors : System.Web.UI.UserControl
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			if (!this.Page.IsPostBack)
			{
				TextBox1.Text = "Errors were encountered when attempting to create the report.  "
				                + "No report was created."
				                + Environment.NewLine + Environment.NewLine
				                + "Please make corrections and try again.";
			}
		}

		protected void Button1_Click(object sender, EventArgs e)
		{
			this.Visible = false;
		}
	}
}