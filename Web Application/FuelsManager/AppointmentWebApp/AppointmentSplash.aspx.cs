using System;

using FuelsManager.FMWebApp;

namespace FuelsManager.AppointmentWebApp
{
	/// <summary>
	/// Summary description for QualityControlSplash.
	/// </summary>
	public partial class AppointmentSplashForm : FMFormBase
	{
	
		protected void Page_Load(object sender, System.EventArgs e)
		{
			// Put user code to initialize the page here
		}

		#region Web Form Designer generated code
		override protected void OnInit(EventArgs e)
		{
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			InitializeComponent();
			base.OnInit(e);
		}
		
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.ID = "AppointmentSplashForm";

		}
		#endregion
	}
}
