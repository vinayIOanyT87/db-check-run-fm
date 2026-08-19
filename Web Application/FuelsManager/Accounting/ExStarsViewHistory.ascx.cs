
namespace FuelsManager.Accounting
{
	using System;
	using System.Web.UI.WebControls;
	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;
	using FMWebApp;

	public partial class ExStarsViewHistory : FMUserControlBase
	{
		private SecurityClass security;
		private ExStarsReportHistoryList historyList;

		protected void Page_Load(object sender, EventArgs e)
		{
			this.security = this.Session["Security"] as SecurityClass;
			if (!this.Page.IsPostBack)
			{
				LoadManager();
				// IN January, you want to look at the previous year
				DateTimeOffset lastDayOfReportingMonth = DateTimeOffset.Now.AddDays(-DateTimeOffset.Now.Day);
				DateTimeOffset beginningOfYear = new DateTimeOffset(lastDayOfReportingMonth.Year, 1, 1, 0, 0, 0, 0, lastDayOfReportingMonth.Offset);
				DateTimeOffset endOfYear = new DateTimeOffset(lastDayOfReportingMonth.Year, 12, 31, 0, 0, 0, 0, lastDayOfReportingMonth.Offset);
				dtStartDate.CurrentValue = beginningOfYear;
				dtEndDate.CurrentValue = endOfYear;
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


		protected void dataGrid_Command(object sender, DataGridCommandEventArgs e)
		{
			System.Diagnostics.Debug.WriteLine(e.ToString());
			string fmt = e.CommandName;
			string filingGuidAsStr = e.Item.Cells[14].Text;
	
			Guid mgrGuid = Guid.Parse(this.ddManager.SelectedValue);
			string defaultFileName = "";
			var errorsAndWarnings =
				FMChannelHelper.MakeCall<IExStarsBusiness, string>(
					RunBusiness =>
					RunBusiness.DownloadReportByGuid(
						security
						, security.SiteGuid
						, mgrGuid
						,filingGuidAsStr
						, fmt == "151" ? "errorReport" : fmt
						, out defaultFileName
						));

			if (string.IsNullOrEmpty(defaultFileName))
			{
				return;
			}

			System.IO.FileInfo file = new System.IO.FileInfo(defaultFileName);
			if (file.Exists)
			{
				Response.ClearContent();
				Response.ClearHeaders();
				Response.AddHeader("Content-Disposition", "attachment; filename=" + file.Name);
				Response.AddHeader("Content-Length", file.Length.ToString());
				Response.ContentType = "text/plain";
				Response.TransmitFile(file.FullName);
				Response.End();
			}
		}


		protected void btnViewHistory_Click(object sender, EventArgs e)
		{
			Guid mgrGuid = Guid.Parse(this.ddManager.SelectedValue);
			historyList = new ExStarsReportHistoryList();
			DateTime startTime = dtStartDate.DateTimeValue;
			DateTime endTime = dtEndDate.DateTimeValue;
			
			string rowCount = FMChannelHelper.MakeCall<IExStarsBusiness, string>(
			RunBusiness =>
			RunBusiness.ViewHistory(
				security
				, mgrGuid
				, startTime
				, endTime
				, out historyList
				));

			lblClickToView.Text =  (historyList.Count == 0)
				? "No records found"
				: "Click To View";
			lblClickToView.Visible = true;

			dataGrid.DataSource = historyList;
			dataGrid.DataBind();		
		}


		public object BindColumn(object container, string columnName)
		{
			return System.Web.UI.DataBinder.Eval(container, "DataItem." + columnName);
		}
	}
}