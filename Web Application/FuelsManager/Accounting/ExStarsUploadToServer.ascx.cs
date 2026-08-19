
namespace FuelsManager.Accounting
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
	using System.Web;
	using System.Web.UI;
	using System.Web.UI.WebControls;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;

	using FMWebApp;

	using FMControls;

	public partial class ExStarsUploadToServer : FMUserControlBase 
	{
		private SecurityClass security;
		protected void Page_Load(object sender, EventArgs e)
		{
			this.security = this.Session["Security"] as SecurityClass;
			if (!this.Page.IsPostBack)
			{
				LoadManager();
				if (security.HasRight(RIGHT.IRS_EXSTARS_MANAGER))
				{
					RadioReportType.Items.Add(new ListItem("Report Previously Submitted to the IRS", "StdMonthly"));
				}
			}
		}


		protected override void OnInit(EventArgs e)
		{
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			this.InitializeComponent();
			base.OnInit(e);
		}

		/// <summary>
		///    Required method for Designer support - do not modify
		///    the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
		}


		protected void btnUpLoad_Click(object sender, EventArgs e)
		{
			if (this.FileUpload1.HasFile)
			{
				string uploadPath = ExStarsConstants.PrependDefaultPathToFileName(this.FileUpload1.FileName);
				this.FileUpload1.SaveAs(uploadPath);
				string errorsAndWarnings = "";

				try
				{
					Guid mgrGuid = Guid.Parse(this.ddManager.SelectedValue);

						FMChannelHelper.MakeCall<IExStarsBusiness, string>(
					RunBusiness =>
					RunBusiness.UploadFile(
						security
						, out errorsAndWarnings
						, mgrGuid
						, uploadPath
						, RadioReportType.SelectedValue
						, true
						));
					tbErrorsAndWarnings.ValidateRequestMode = ValidateRequestMode.Disabled;
					tbErrorsAndWarnings.Text = errorsAndWarnings;
				}
				catch (Exception ex)
				{
					tbErrorsAndWarnings.ValidateRequestMode = ValidateRequestMode.Disabled;
					tbErrorsAndWarnings.Text = string.Format("{0}\n{1}", ex.Message, ex.StackTrace).Replace("\n", Environment.NewLine);
				}
			}
		}


		private void LoadManager()
		{
			var managers = FMChannelHelper.MakeCall<ICompanies, CompanyCollectionClass>(
								 companies => companies.EnumerateByRole(security, COMPANY_ROLE.MANAGER, false, false));

			this.ddManager.Items.Clear();

			foreach (CompanyClass manager in managers)
			{
				ListItem item = new ListItem();
				item.Text = manager.ID;
				item.Value = manager.MasterRecordGuid.ToString();
				ddManager.Items.Add(item);
			}
		}
	}
}