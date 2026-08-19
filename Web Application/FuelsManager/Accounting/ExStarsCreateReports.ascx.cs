namespace FuelsManager.Accounting
{
	using System;
	using System.Collections.Generic;
	using System.Web.UI;
	using System.Web.UI.WebControls;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;

	using FuelsManager.FMWebApp;

	public partial class ExStarsCreateReports : FMUserControlBase
	{
		public class MonthAsInt
		{
			public int MonthNumber { get; protected set; }
			public string MonthName { get; protected set; }

			public MonthAsInt(int monthNumber, string monthName)
			{
				this.MonthNumber = monthNumber;
				this.MonthName = monthName;
			}

			public override string ToString()
			{
				return this.MonthName;
			}
		}
		public bool IsStdMonthly { get{ return  this.ddReportType.SelectedIndex == 0; }}
		private readonly string[] reportTypeDesc = new string[]
				{
					"Note: This report type should be used to generate a standard monthly IRS report where the transactional data in the report will correspond to the Month,Year, and Manager selected below. DO NOT use this report type  if there was an inventory manager hand-over within the month and year selected."
					,"Note: Only the OUTGOING inventory manager should use this report type  to generate an IRS report that will contain activity data from the first day of the month associated with the Inventory Hand-over Date up to and including the Inventory Hand-over Date."
					,"Note: Only the INCOMING inventory manager should use this report type  to generate an IRS report that will contain the physical inventory reading on the Inventory Hand-over Date along with all remaining activity data up to and including the last day of the month associated with the day."
				};
		private SecurityClass security;

		protected readonly Dictionary<string, string> reportTypeDictionary = new Dictionary<string, string>();
		protected readonly List<MonthAsInt>  MonthList =  new List<MonthAsInt>();

		protected void Page_Load(object sender, EventArgs e)
		{
			this.security = this.Session["Security"] as SecurityClass;
			if (!this.Page.IsPostBack)
			{
				this.ddModifier.Items.Add("Original");
				this.ddModifier.Items.Add("Replacement");
				this.ddModifier.Items.Add("Supplemental");
				this.LoadReportType();
				this.LoadManager();
				this.LoadMonthAndYear();
				this.chkTest.Visible = this.security.HasRight(RIGHT.IRS_EXSTARS_MANAGER);
				this.Panel1.Visible = false;
				this.tbErrorsAndWarnings.Text = "";			
			}
		}

		private void LoadMonthAndYear()
		{
			if (this.MonthDropDownLst.Items.Count > 0)
			{
				return;
			}
			if (this.MonthList.Count == 0) 
			{
				this.MonthList.Add(new MonthAsInt(1, "January"));
				this.MonthList.Add(new MonthAsInt(2, "February"));
				this.MonthList.Add(new MonthAsInt(3, "March"));
				this.MonthList.Add(new MonthAsInt(4, "April"));
				this.MonthList.Add(new MonthAsInt(5, "May"));
				this.MonthList.Add(new MonthAsInt(6, "June"));
				this.MonthList.Add(new MonthAsInt(7, "July"));
				this.MonthList.Add(new MonthAsInt(8, "August"));
				this.MonthList.Add(new MonthAsInt(9, "September"));
				this.MonthList.Add(new MonthAsInt(10, "October"));
				this.MonthList.Add(new MonthAsInt(11, "November"));
				this.MonthList.Add(new MonthAsInt(12, "December"));				
			}
			this.MonthDropDownLst.DataSource = this.MonthList;
			this.MonthDropDownLst.DataTextField = "MonthName";
			this.MonthDropDownLst.DataValueField = "MonthNumber"; 
			this.MonthDropDownLst.DataBind();

			// put last 15 years into YearList
			int currentYear = DateTime.Now.Date.Year;
			for (int count = 0; count < 15; count++)
			{
				this.YearDropDownList.Items.Add( (currentYear--).ToString());
			}

			// set default to the month which most recently  was the end of the month
			DateTime lastDayOfMonth = DateTime.Now.Date.AddDays(1);
			if (lastDayOfMonth.Day == 1)
			{
				// today is the last day of the month
				lastDayOfMonth = DateTime.Now.Date;
			}
			else
			{
				//  the day prior to the first of the month
				lastDayOfMonth = new DateTime(DateTime.Now.Date.Year, DateTime.Now.Date.Month, 1).AddDays(-1);
			}
			this.YearDropDownList.SelectByText(lastDayOfMonth.Year.ToString());
			this.MonthDropDownLst.SelectedIndex = lastDayOfMonth.Month - 1;
		}

		private void LoadReportType()
		{
			this.reportTypeDictionary.Add("Standard Monthly", "StdMonthly");
			this.reportTypeDictionary.Add("Outgoing Manager", "OutgoingManger");
			this.reportTypeDictionary.Add("Incoming Manager", "IncomingManager");
			this.ddReportType.DataSource = this.reportTypeDictionary;
			this.ddReportType.DataTextField = "key";
			this.ddReportType.DataValueField = "value";
			this.ddReportType.DataBind();
			this.ddReportType.SelectedIndex = 0;
			this.ddReportType_SelectedIndexChanged(null, null);
		}

		private void LoadManager()
		{
			var managers = FMChannelHelper.MakeCall<ICompanies, CompanyCollectionClass>(
								 companies => companies.EnumerateByRole(this.security, COMPANY_ROLE.MANAGER, false, false));

			this.ddManager.Items.Clear();

			foreach (CompanyClass manager in managers)
			{
				ListItem item = new ListItem();
				item.Text = manager.ID;
				item.Value = manager.MasterRecordGuid.ToString();
				this.ddManager.Items.Add(item);
			}
		}

		/// <summary>
		/// The on init.
		/// </summary>
		/// <param name="e">
		/// The e.
		/// </param>
		protected override void OnInit(EventArgs e)
		{
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			this.InitializeComponent();
			this.ddReportType.TextChanged += this.ddReportType_SelectedIndexChanged;
			this.ddReportType.SelectedIndexChanged += this.ddReportType_SelectedIndexChanged;
			base.OnInit(e);
		}

		protected void ddReportType_SelectedIndexChanged(object sender, EventArgs e)
		{
			DateTime lastDayOfReportingMonth = DateTime.Now.AddDays(-DateTime.Now.Day);
			this.tbDescription.Text = this.reportTypeDesc[this.ddReportType.SelectedIndex];
			this.tbDescription.Style.Remove("background-color");
			this.tbDescription.Style.Add("background-color",  this.IsStdMonthly? "transparent" : "yellow");
			switch (this.ddReportType.SelectedIndex)
			{
				case 1:
					if (string.IsNullOrEmpty(this.tbTurnOverDay.Text))
					{
						this.tbTurnOverDay.Text = lastDayOfReportingMonth.Day.ToString();
					}
					break;
				case 2:
					if (string.IsNullOrEmpty(this.tbTurnOverDay.Text))
					{
						this.tbTurnOverDay.Text = "1";
					}
					break;
				default: break;
			}
			this.panTurnOver.Visible = !this.IsStdMonthly;
			if (!this.IsStdMonthly)
			{
				this.DateChanged();
			}
			else
			{
				this.EnableButtons(true);
			}
		}

		/// <summary>
		///    Required method for Designer support - do not modify
		///    the contents of this method with the code editor.
		/// </summary>
		public void InitializeComponent()
		{
			this.tbErrorsAndWarnings.CausesValidation = false;
			this.tbErrorsAndWarnings.ValidateRequestMode = ValidateRequestMode.Disabled;
			this.tbReportTypeWarning.ValidateRequestMode = ValidateRequestMode.Disabled;
			this.tbReportTypeWarning.CausesValidation = false;
			this.tbReportTypeWarning.Text="Outgoing and Incoming manager reports should only be created when the company "
			+"managing the fuel is changing. They must always be created in pairs, with the outgoing report created first.  "
			+"In the month where these reports are used, there will be no Standard Monthly report."
			+ Environment.NewLine + Environment.NewLine + "Do you wish to create this report?";
		}

		protected void SelectedDateChanged(object sender, EventArgs e)
		{
			this.DateChanged();
		}

		protected void DateChanged()
		{
			this.tbErrorsAndWarnings.Text = "";
			DateTime endReportDate = this.ParseReportDate().AddMonths(1).AddDays(-1);
			if (endReportDate > DateTime.Now)
			{
				this.EnableButtons(false);
				return;
			}
			if (this.IsStdMonthly)
			{
				this.panTurnOver.Visible = false;
			}
			else
			{
				this.panTurnOver.Visible = true;
				this.tbTurnOverMonth.Text = this.MonthDropDownLst.SelectedItem.Text;
				this.tbTurnOverYear.Text = this.YearDropDownList.Text;
				int day;
				bool enable = int.TryParse(this.tbTurnOverDay.Text, out day) && day >= 0 && day <= endReportDate.Day;
				this.EnableButtons(enable);
				return;
			}			
			this.EnableButtons(true);
		}


		protected void EnableButtons(bool enable)
		{
			this.btnCreateReport.Enabled = enable;
			this.btnDownLoadEDI.Enabled = enable;
			this.btnDownLoadEasyRead.Enabled = enable;
		}


		protected DateTime ParseReportDate()
		{
			return new DateTime(int.Parse(this.YearDropDownList.Text), int.Parse(this.MonthDropDownLst.SelectedItem.Value), 1);
		}


		protected void btnInOutOk_Click(object sender, EventArgs e)
		{
			this.tbErrorsAndWarnings.Text = "";
			this.Panel1.Visible = false;
			this.CreateReport();
		}


		protected void btnInOutProhbited_Click(object sender, EventArgs e)
		{
			this.Panel1.Visible = false;
		}


		protected void btnCreateReportStdMonthly_Click(object sender, EventArgs e)
		{
			this.tbErrorsAndWarnings.Text = "";
			if (this.IsStdMonthly)
			{
				this.CreateReport();
			}
			else
			{
				this.Panel1.Visible = true;
			}
		}


		protected void btnCreateReportInOutMgr_Click(object sender, EventArgs e)
		{
			this.tbErrorsAndWarnings.Text = "";
			this.Panel1.Visible = false;
			this.CreateReport();
		}

		protected void CreateReport()
		{
			DateTime endDate;
			Guid mgrGuid;
			var startDate = this.StartDate(out endDate, out mgrGuid);
			bool reportCreated = false;
			string userErrors;
			try
			{
				var errorsAndWarnings =
					FMChannelHelper.MakeCall<IExStarsBusiness, string>(
				RunBusiness =>
				RunBusiness.CreateExStarsReport( 
					this.security
					,this.security.SiteGuid
					,mgrGuid
					,this.chkTest.Checked
					,startDate
					,endDate
                    , true
					,this.ddReportType.SelectedItem.Value
					,this.ddModifier.SelectedItem.Text
					,out userErrors
					, out reportCreated
					));
				this.tbErrorsAndWarnings.ValidateRequestMode = ValidateRequestMode.Disabled;
				this.tbErrorsAndWarnings.Text = errorsAndWarnings;
			}
			catch (Exception ex)
			{
				this.tbErrorsAndWarnings.ValidateRequestMode = ValidateRequestMode.Disabled;
				this.tbErrorsAndWarnings.Text = ex.Message.Replace("\n", Environment.NewLine);
			}
			if (!reportCreated)
			{
				this.warningpopup.Visible = true;
			}
		}


		protected void btnDownLoadEDI_Click(object sender, EventArgs e)
		{
			this.DownLoadFile(false);
		}


		protected void btnDownLoadEasyRead_Click(object sender, EventArgs e)
		{
			this.DownLoadFile(true);
		}


		protected void DownLoadFile( bool easyRead)
		{
			this.tbErrorsAndWarnings.Text = "";
			Guid mgrGuid = Guid.Parse(this.ddManager.SelectedValue);
			var transactionMonthAndYear = this.ParseReportDate();
			string defaultFileName = "";
			try
			{
				var errorsAndWarnings =
					FMChannelHelper.MakeCall<IExStarsBusiness, string>(
						RunBusiness =>
						RunBusiness.DownloadReport(
							this.security
							, this.security.SiteGuid
							, mgrGuid
							, transactionMonthAndYear
							, this.ddReportType.SelectedItem.Value
							, easyRead ? "easyread" : "edi"
							, out defaultFileName
							));
			}
			catch (Exception ex)
			{
				this.tbErrorsAndWarnings.ValidateRequestMode = ValidateRequestMode.Disabled;
				this.tbErrorsAndWarnings.Text = ex.Message.Replace("\n", Environment.NewLine);
			}

			if (string.IsNullOrEmpty(defaultFileName))
			{
				return;
			}
				
			System.IO.FileInfo file = new System.IO.FileInfo(defaultFileName);
			if (file.Exists)
			{
				this.Response.ClearContent();
				this.Response.ClearHeaders();
				this.Response.AddHeader("Content-Disposition", "attachment; filename=" + file.Name);
				this.Response.AddHeader("Content-Length", file.Length.ToString());
				this.Response.ContentType = "text/plain";
				this.Response.TransmitFile(file.FullName);
				this.Response.End();
			}
		}


		private DateTime StartDate(out DateTime endDate, out Guid mgrGuid)
		{
			DateTime startDate = DateTime.MinValue;
			endDate = DateTime.MinValue;
			DateTime firstOfMonth = this.ParseReportDate();
			switch (this.ddReportType.SelectedItem.Value)
			{
				case "StdMonthly":
					startDate = firstOfMonth;
					endDate = firstOfMonth.AddMonths(1).AddDays(-1);
					break;
				case "OutgoingManger":
					startDate = firstOfMonth;
					endDate = new DateTime(startDate.Year, startDate.Month, int.Parse(this.tbTurnOverDay.Text));
					break;
				case "IncomingManager":
					startDate = new DateTime(startDate.Year, startDate.Month, int.Parse(this.tbTurnOverDay.Text));
					endDate = firstOfMonth.AddMonths(1).AddDays(-1);
					break;
			}

			mgrGuid = Guid.Parse(this.ddManager.SelectedValue);
			return startDate;
		}	
	}
}