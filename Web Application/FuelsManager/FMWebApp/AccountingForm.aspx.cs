namespace FuelsManager.FMWebApp
{
	using System;

	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.Exceptions;

	using global::FMWebApp;

	/// <summary>
	/// Summary description for AccountingForm.
	/// </summary>
	public partial class AccountingForm : FMFormBase
	{
	
		protected void Page_Load(object sender, EventArgs e)
		{
			try
			{
				this.GetSecurity();
			}
			catch (FMSessionInvalidException ex)
			{
				this.ErrorHandler(ex);
			}
		}

		#region Web Form Designer generated code
		override protected void OnInit(EventArgs e)
		{
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			this.InitializeComponent();
			base.OnInit(e);
		}
		
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{    

		}
		#endregion
	}
}
