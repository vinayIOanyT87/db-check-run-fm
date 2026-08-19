// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RomanJetFileForm.aspx.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the RomanJetFileForm type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FuelsManager.FinanceWebApp
{
    using System;
    using System.Collections;
    using System.Web.UI.WebControls;

    using Accounting;

    using FMBusinessObjects.BusinessInterfaces;
    using FMBusinessObjects.ChannelFactories;
    using FMBusinessObjects.DataObjects;
    using FMBusinessObjects.ReportExecutionSvr2005;

    using global::FinanceWebApp;

    public partial class RomanJetFileForm : AccountingWebFormView
	{
		#region Constants and Fields

		private const string JetReportName = "JournalEntryTransactionReport";

		private const string Msg001 = "Must select a Posting Period.";

		private const string Msg002 = "Must select a Group ID.";

		private const string Msg003 = "Jet Reference value entered is out of range.";

		private const string Msg004 = "Must enter a valid 4 digit Posting Year, eg 2008.";

		private const string RjfDocumentCompany = "RJFDocumentCompany";

		private const string RjfGroupID = "RJFGroupID";

		private const string RjfJournalType = "RJFJournalType";

		private const int RjfNumberOfReportParameters = 12;

/*
		private const string RjfPostingEndDate = "RJFPostingEndDate";
*/

		private const string RjfPostingPeriod = "RJFPostingPeriod";

		private const int RjfPostingPeriodMaxNum = 12;

/*
		private const string RjfPostingStartDate = "RJFPostingStartDate";
*/

		private const string RjfPostingYear = "RJFPostingYear";

		private const string RjfTransactionType = "RJFTransactionType";

		private AccountingSite accountingSite;

		#endregion

		#region Methods

		protected override void OnInit(EventArgs e)
		{
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
		    this.InitializeComponent();
			base.OnInit(e);
		}

		protected void Page_Load(object sender, EventArgs e)
		{
			this.accountingSite = FMChannelHelper.MakeCall<IAccountingSites, AccountingSite>(
																	 x =>
																	 x.LoadSiteInfo(this.security, this.security.SiteGuid)
																);

			if (this.Page.IsPostBack == false)
			{
				this.PopulatePostingPeriodDropdown();
				this.PopulateJournalTypeDropdown();
				this.PopulateDocumentCompanyDropdown();
				this.PopulateTransactionTypeDropdown();
				this.PopulateGroupIDDropdown();
				this.SetPostingYear();
				this.Session.Remove(RjfTransactionType);
			}

			// force JET reference label change
			this.GroupIDSelectionChange(sender, e);
		}

		/// <summary>
		///    This method handles the Document Company dropdown selection change event. It
		///    will save the selected index in session.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void DocumentCompanySelectionChange(object sender, EventArgs e)
		{
			this.Session.Add(RjfDocumentCompany, this.DocumentCompanyDropdown.SelectedValue);
		}

		/// <summary>
		///    This method handles the Export button event. It will ensure that the user
		///    has selected the mandatory report filters and will call the RenderJetReport method to
		///    render the report.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ExportBtnOnClick(object sender, EventArgs e)
		{
			string errorMessage = null;
			string postingPeriod = this.PostingPeriodDropdown.SelectedValue;
			if (postingPeriod == "-1")
			{
				errorMessage += "\n" + Msg001;
			}
            short postingYear;
			if (!short.TryParse(this.PostingYearTextBox.Text, out postingYear))
			{
				errorMessage += "\n" + Msg004;
			}
			else if (postingYear < 2000 || postingYear > 9999)
			{
				errorMessage += "\n" + Msg004;
			}
			string groupID = this.GroupIDDropdown.SelectedValue;
			if (groupID == "-1")
			{
				errorMessage += "\n" + Msg002;
			}
			if (this.JetReferenceTextBox.Text.Trim() != "")
			{
				try
				{
					int jetReferenceValue = Convert.ToInt32(this.JetReferenceTextBox.Text.Trim());
					if (groupID == "Air Force" && (jetReferenceValue < 14000000 || jetReferenceValue > 14999999)
						 || groupID == "Army" && (jetReferenceValue < 12000000 || jetReferenceValue > 12999999)
						 || groupID == "Navy" && (jetReferenceValue < 13000000 || jetReferenceValue > 13999999))
					{
						errorMessage += "\n" + Msg003;
					}
				}
				catch //Not a valid integer entered
				{
					errorMessage += "\n" + Msg003;
				}
			}
			if (errorMessage != null)
			{
				this.ErrorHandler(new Exception(errorMessage));
			}
			else
			{
				this.RenderJetReport();
			}
		}

        /// <summary>
        ///    This method will return the Posting Start date depending on the Posting Period option selected and
        ///    the Year entered.
        /// </summary>
        /// <param name="postingPeriod"></param>
        private DateTimeOffset GetPostingStartDate(string postingPeriod)
		{
			short postingMonth = Convert.ToInt16(postingPeriod);
			short postingYear = Convert.ToInt16(this.PostingYearTextBox.Text);
			if (postingMonth > 6)
			{
				postingMonth -= 6;
			}
			else
			{
				postingMonth += 6;
			}
			return new DateTimeOffset(postingYear, postingMonth, 1, 0, 0, 0, TimeSpan.Zero);
		}

		/// <summary>
		///    This method handles the Group ID dropdown selection change event. It
		///    will save the selected index in session.
		///    Depending on the selection, it will set/display the Jet Reference valid ranges in a text label
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void GroupIDSelectionChange(object sender, EventArgs e)
		{
			this.Session.Add(RjfGroupID, this.GroupIDDropdown.SelectedValue);
			if (this.GroupIDDropdown.SelectedValue == "Air Force")
			{
				this.JetReferenceRangeLabel.Text = "Valid Range : 14000000 - 14999999";
			}
			else if (this.GroupIDDropdown.SelectedValue == "Army")
			{
				this.JetReferenceRangeLabel.Text = "Valid Range : 12000000 - 12999999";
			}
			else if (this.GroupIDDropdown.SelectedValue == "Navy")
			{
				this.JetReferenceRangeLabel.Text = "Valid Range : 13000000 - 13999999";
			}
			else
			{
				this.JetReferenceRangeLabel.Text = "";
			}
		}

		/// <summary>
		///    Required method for Designer support - do not modify
		///    the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.PostingPeriodDropdown.SelectedIndexChanged += this.PostingPeriodSelectionChange;
			this.TransactionTypeDropdown.SelectedIndexChanged += this.TransactionTypeSelectionChange;
			this.GroupIDDropdown.SelectedIndexChanged += this.GroupIDSelectionChange;
			this.JournalTypeDropdown.SelectedIndexChanged += this.JournalTypeSelectionChange;
			this.DocumentCompanyDropdown.SelectedIndexChanged += this.DocumentCompanySelectionChange;
			this.ExportButton.Click += this.ExportBtnOnClick;
			this.PreRender += this.RomanJetFileFormPreRender;
		}

/*
		/// <summary>
		///    This method will check if date passed in is valid.
		/// </summary>
		/// <param name="date"></param>
		private bool IsValidDate(string date)
		{
			DateTimeOffset _date;
			return DateTimeOffset.TryParse(date, out _date);
		}
*/

		/// <summary>
		///    This method handles the Journal Type dropdown selection change event. It
		///    will save the selected index in session.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void JournalTypeSelectionChange(object sender, EventArgs e)
		{
			this.Session.Add(RjfJournalType, this.JournalTypeDropdown.SelectedValue);
		}

		/// <summary>
		///    This method will populate the Document Company drop down list.
		///    Note: The values here are "split" by ":" when placed on the ROMAN JET report.
		/// </summary>
		private void PopulateDocumentCompanyDropdown()
		{
		    var documentCompanyListing = new ArrayList();

		    var valuePair = new DropdownValuePairDO
		                    {
		                        Text = "1000 : Defence-Departmental",
		                        TextValue = "1000 : Defence-Departmental"
		                    };
		    documentCompanyListing.Add(valuePair);

		    valuePair = new DropdownValuePairDO
		                {
		                    Text = "2000 : Defence-Administered",
		                    TextValue = "2000 : Defence-Administered"
		                };
		    documentCompanyListing.Add(valuePair);

		    valuePair = new DropdownValuePairDO
		                {
		                    Text = "3000 : Defence-Spl Public Monies",
		                    TextValue = "3000 : Defence-Spl Public Monies"
		                };
		    documentCompanyListing.Add(valuePair);

		    valuePair = new DropdownValuePairDO { Text = "4100 : DMO-Departmental", TextValue = "4100 : DMO-Departmental" };
		    documentCompanyListing.Add(valuePair);

		    valuePair = new DropdownValuePairDO { Text = "4200 : DMO-Administered", TextValue = "4200 : DMO-Administered" };
		    documentCompanyListing.Add(valuePair);

		    valuePair = new DropdownValuePairDO
		                {
		                    Text = "4300 : DMO-Spl Public Monies",
		                    TextValue = "4300 : DMO-Spl Public Monies"
		                };
		    documentCompanyListing.Add(valuePair);

			this.DocumentCompanyDropdown.DataSource = documentCompanyListing;
			this.DocumentCompanyDropdown.DataTextField = "Text";
			this.DocumentCompanyDropdown.DataValueField = "TextValue";
			this.DocumentCompanyDropdown.DataBind();

			if (this.Session[RjfDocumentCompany] == null)
			{
				this.DocumentCompanyDropdown.SelectedIndex = 0;
				this.Session.Add(RjfDocumentCompany, this.DocumentCompanyDropdown.SelectedValue);
			}
			else
			{
			    var li = this.DocumentCompanyDropdown.Items.FindByValue((string)this.Session[RjfDocumentCompany]);
			    li.Selected = true;
			}
		}

		/// <summary>
		///    This method will populate the Group ID drop down list with the 3 Service Groups.
		///    Note: The values here must match match with Site UserData3 list values
		/// </summary>
		private void PopulateGroupIDDropdown()
		{
		    var groupIDListing = new ArrayList();

		    var valuePair = new DropdownValuePairDO { Text = "<Select Group>", TextValue = "-1" };
		    groupIDListing.Add(valuePair);

		    valuePair = new DropdownValuePairDO { Text = "Air Force", TextValue = "Air Force" };
		    groupIDListing.Add(valuePair);

		    valuePair = new DropdownValuePairDO { Text = "Army", TextValue = "Army" };
		    groupIDListing.Add(valuePair);

		    valuePair = new DropdownValuePairDO { Text = "Navy", TextValue = "Navy" };
		    groupIDListing.Add(valuePair);

			this.GroupIDDropdown.DataSource = groupIDListing;
			this.GroupIDDropdown.DataTextField = "Text";
			this.GroupIDDropdown.DataValueField = "TextValue";
			this.GroupIDDropdown.DataBind();

			if (this.Session[RjfGroupID] == null)
			{
				this.GroupIDDropdown.SelectedIndex = 0;
				this.Session.Add(RjfGroupID, this.GroupIDDropdown.SelectedValue);
			}
			else
			{
			    var li = this.GroupIDDropdown.Items.FindByValue((string)this.Session[RjfGroupID]);
			    li.Selected = true;
			}
		}

		/// <summary>
		///    This method will populate the Journal Type drop down list
		/// </summary>
		private void PopulateJournalTypeDropdown()
		{
		    var journalTypeListing = new ArrayList();

		    var valuePair = new DropdownValuePairDO { Text = "A: Accrual Reversing", TextValue = "A: Accrual Reversing" };
		    journalTypeListing.Add(valuePair);

		    valuePair = new DropdownValuePairDO { Text = "G: General Journal", TextValue = "G: General Journal" };
		    journalTypeListing.Add(valuePair);

		    valuePair = new DropdownValuePairDO { Text = "N: Non-reversing Accrual", TextValue = "N: Non-reversing Accrual" };
		    journalTypeListing.Add(valuePair);

			this.JournalTypeDropdown.DataSource = journalTypeListing;
			this.JournalTypeDropdown.DataTextField = "Text";
			this.JournalTypeDropdown.DataValueField = "TextValue";
			this.JournalTypeDropdown.DataBind();

			// Set the selection based on ROMAN Export Category selected.
			string exportCategory = this.Request.QueryString["ExportCat"];
			if (exportCategory == "Acceptance" || exportCategory == "IssueSell")
			{
			    var li = this.JournalTypeDropdown.Items.FindByValue("A: Accrual Reversing");
			    li.Selected = true;
			}
			else if (exportCategory == "Storage")
			{
			    var li = this.JournalTypeDropdown.Items.FindByValue("G: General Journal");
			    li.Selected = true;
			}
			else if (exportCategory == "Consumption")
			{
			    var li = this.JournalTypeDropdown.Items.FindByValue("N: Non-reversing Accrual");
			    li.Selected = true;
			}
			else
			{
				throw new Exception("Unknown ROMAN export Category selected");
			}
			this.Session.Add(RjfJournalType, this.JournalTypeDropdown.SelectedValue);
		}

		/// <summary>
		///    This method will populate the Posting Period drop down list
		/// </summary>
		private void PopulatePostingPeriodDropdown()
		{
		    var postingPeriodListing = new ArrayList();

		    var valuePair = new DropdownValuePairDO { Text = "<Select PP>", TextValue = "-1" };
		    postingPeriodListing.Add(valuePair);

			for (int i = 1; i <= RjfPostingPeriodMaxNum; i++)
			{
			    valuePair = new DropdownValuePairDO { Text = i.ToString(), TextValue = i.ToString() };
			    postingPeriodListing.Add(valuePair);
			}
			this.PostingPeriodDropdown.DataSource = postingPeriodListing;
			this.PostingPeriodDropdown.DataTextField = "Text";
			this.PostingPeriodDropdown.DataValueField = "TextValue";
			this.PostingPeriodDropdown.DataBind();

			if (this.Session[RjfPostingPeriod] == null)
			{
				this.PostingPeriodDropdown.SelectedIndex = 0;
				this.Session.Add(RjfPostingPeriod, this.PostingPeriodDropdown.SelectedValue);
			}
			else
			{
			    var li = this.PostingPeriodDropdown.Items.FindByValue((string)this.Session[RjfPostingPeriod]);
			    li.Selected = true;
			}
		}

		/// <summary>
		///    This method will populate the TransactionType drop down list
		/// </summary>
		private void PopulateTransactionTypeDropdown()
		{
			string exportCategory = this.Request.QueryString["ExportCat"];
			TransactionAliasCollectionClass[] aliasesCollections = null;

			// Acceptance = all Transaction Types 3, 4, 8
			if (exportCategory == "Acceptance")
			{
				aliasesCollections = new TransactionAliasCollectionClass[3];
				aliasesCollections[0] = FMChannelHelper.MakeCall<ITransactionAliases, TransactionAliasCollectionClass>(
																	 x =>
																	 x.EnumerateByTransTypeID(this.security, TransactionTypes.T4_SecondaryDefuel)
																);

				aliasesCollections[1] = FMChannelHelper.MakeCall<ITransactionAliases, TransactionAliasCollectionClass>(
																	 x =>
																	 x.EnumerateByTransTypeID(this.security, TransactionTypes.T8_Receipt)
																);

				aliasesCollections[2] = FMChannelHelper.MakeCall<ITransactionAliases, TransactionAliasCollectionClass>(
																	x =>
																	x.EnumerateByTransTypeID(this.security, TransactionTypes.T3_PrimaryDefuel)
															  );
			}

				// Storage including all Transaction Types 15 plus any transactions with Product "Waste". 
			// The Waste option is added manually after populating DropDown list.
			else if (exportCategory == "Storage")
			{
				aliasesCollections = new TransactionAliasCollectionClass[1];
				aliasesCollections[0] = FMChannelHelper.MakeCall<ITransactionAliases, TransactionAliasCollectionClass>(
																	 x =>
																	 x.EnumerateByTransTypeID(this.security, TransactionTypes.T15_PrimaryRegrade)
																);

			}

				// Issue/Sell contains all TransactionTypes "5" that have the text "Issue" or "Sale" in the alias name
			else if (exportCategory == "IssueSell")
			{
				aliasesCollections = new TransactionAliasCollectionClass[1];
				aliasesCollections[0] = new TransactionAliasCollectionClass();
			    var tempAliasCollection = FMChannelHelper.MakeCall<ITransactionAliases, TransactionAliasCollectionClass>(
			        x =>
			            x.EnumerateByTransTypeID(this.security, TransactionTypes.T5_PrimaryDisbursement)
			        );

				//Iterate through each Transcation alias and only include ones with "Issue" or "Sales" text
				foreach (TransactionAliasClass alias in tempAliasCollection)
				{
					if (alias.ID.ToUpper().IndexOf("ISSUE", StringComparison.Ordinal) >= 0 || alias.ID.ToUpper().IndexOf("SALE", StringComparison.Ordinal) >= 0)
					{
						aliasesCollections[0].Add(alias);
					}
				}
			}

				// Consumption includes aliases "Direct Fuel Purchase" or "Commercial" with TransactionTypes "12"
			else if (exportCategory == "Consumption")
			{
				aliasesCollections = new TransactionAliasCollectionClass[1];
				aliasesCollections[0] = new TransactionAliasCollectionClass();
			    var tempAliasCollection = FMChannelHelper.MakeCall<ITransactionAliases, TransactionAliasCollectionClass>(
			        x =>
			            x.EnumerateByTransTypeID(this.security, TransactionTypes.T12_InventoryNotAffected)
			        );

				foreach (TransactionAliasClass alias in tempAliasCollection)
				{
					if (alias.ID.ToUpper().IndexOf("DIRECT FUEL PURCHASE", StringComparison.Ordinal) >= 0 || alias.ID.ToUpper().IndexOf("COMMERCIAL", StringComparison.Ordinal) >= 0)
					{
						aliasesCollections[0].Add(alias);
					}
				}
			}

			DropdownValuePairDO valuePair;
			var transactionAliasesList = new ArrayList();
		    if (aliasesCollections != null)
		    {
		        foreach (TransactionAliasCollectionClass aliasesCollection in aliasesCollections)
		        {
		            foreach (TransactionAliasClass alias in aliasesCollection)
		            {
		                valuePair = new DropdownValuePairDO { Text = alias.ID, TextValue = alias.IdentityGuid.ToString() };
		                transactionAliasesList.Add(valuePair);
		            }
		        }
		    }
		    //Manually add to Dropdown "Waste" if category=Storage it is not a Transaction but a Product
			if (exportCategory == "Storage")
			{
			    valuePair = new DropdownValuePairDO { Text = "Waste", TextValue = "Waste" };
			    transactionAliasesList.Add(valuePair);
			}
			//Insert "All" transaction option
		    valuePair = new DropdownValuePairDO { Text = "{All}", TextValue = "All" + exportCategory };
		    transactionAliasesList.Insert(0, valuePair);

			this.TransactionTypeDropdown.DataSource = transactionAliasesList;
			this.TransactionTypeDropdown.DataTextField = "Text";
			this.TransactionTypeDropdown.DataValueField = "TextValue";
			this.TransactionTypeDropdown.DataBind();

			ListItem li = this.TransactionTypeDropdown.Items.FindByValue((string)this.Session[RjfTransactionType]);
			if (li == null)
			{
				this.TransactionTypeDropdown.SelectedIndex = 0;
				this.Session.Add(RjfTransactionType, this.TransactionTypeDropdown.SelectedValue);
			}
			else
			{
				li = this.TransactionTypeDropdown.Items.FindByValue((string)this.Session[RjfTransactionType]);
				li.Selected = true;
			}
		}

		/// <summary>
		///    This method handles the Posting Period dropdown selection change event. It
		///    will save the selected index in session.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void PostingPeriodSelectionChange(object sender, EventArgs e)
		{
			this.Session.Add(RjfPostingPeriod, this.PostingPeriodDropdown.SelectedValue);
		}

		/// <summary>
		///    This method will setup the report parameters and call the RenderReport object
		///    to render the Jet file report.
		/// </summary>
		private void RenderJetReport()
		{
			try
			{
				var renderReports = new RenderReports();

			    // Get system setting for the report URL, the report directory, and the report format.
				var systemSetting = FMChannelHelper.MakeCall<ISystemSettings, SystemSettingClass>(
				    x =>
				        x.Get(this.security)
				    );

				renderReports.ReportingServiceUrl = systemSetting.ReportServerUrl + "/ReportService.asmx";
				renderReports.ReportName = this.accountingSite.LoginSite.ReportDirectory + "/" + JetReportName;
				renderReports.ReportFormat = RenderReports.REPORT_FORMAT_EXCEL;

				// Set SimplePageHeaders to True so Headers and footers translate to real Excel Header and Footers
				renderReports.DeviceInfo =
					"<DeviceInfo><Toolbar>False</Toolbar><SimplePageHeaders>True</SimplePageHeaders></DeviceInfo>";

				// Create the report parameters for the Jet file report.
				var reportParameters = new ParameterValue[RjfNumberOfReportParameters];
				for (int i = 0; i < RjfNumberOfReportParameters; i++)
				{
					reportParameters[i] = new ParameterValue();
				}

				reportParameters[0].Name = "GroupID";
				reportParameters[0].Value = this.GroupIDDropdown.SelectedValue;

				reportParameters[1].Name = "PostingPeriod";
				reportParameters[1].Value = this.PostingPeriodDropdown.SelectedValue;

				reportParameters[2].Name = "PostingStartDate";
				DateTimeOffset postingStartDate = this.GetPostingStartDate(this.PostingPeriodDropdown.SelectedValue);
				reportParameters[2].Value = postingStartDate.Year + "-" + postingStartDate.Month + "-" + postingStartDate.Day;

				reportParameters[3].Name = "PostingEndDate";
				DateTimeOffset postingEndDate = postingStartDate.AddMonths(1);
				reportParameters[3].Value = postingEndDate.Year + "-" + postingEndDate.Month + "-" + postingEndDate.Day;

				reportParameters[4].Name = "JournalType";
				reportParameters[4].Value = this.JournalTypeDropdown.SelectedValue;

				reportParameters[5].Name = "LoginSiteGuid";
				reportParameters[5].Value = this.security.LoginSiteGuid.ToString();

				reportParameters[6].Name = "SiteGuid";
				reportParameters[6].Value = this.security.SiteGuid.ToString();

				reportParameters[7].Name = "UserGuid";
				reportParameters[7].Value = this.security.UserGuid.ToString();

				reportParameters[8].Name = "DocumentCompany";
				reportParameters[8].Value = this.DocumentCompanyDropdown.SelectedValue;

				reportParameters[9].Name = "JetReference";
				reportParameters[9].Value = this.JetReferenceTextBox.Text;

				reportParameters[10].Name = "TransactionType";
				reportParameters[10].Value = this.TransactionTypeDropdown.SelectedValue;

				reportParameters[11].Name = "JournalDescription";
				reportParameters[11].Value = this.JournalDescriptionTextBox.Text;

				renderReports.ReportParameters = reportParameters;

				// Render the Jet file report.
			    var renderedreport = renderReports.RenderReport(this.security);
				if (null != renderedreport)
				{
					this.Response.ClearContent();
					this.Response.ClearHeaders();
					this.Response.AddHeader("Content-Type", "application/xls");
					this.Response.AddHeader("Content-Disposition", "attachment; filename=JET.XLS");
					this.Response.BinaryWrite(renderedreport);
					this.Response.Flush();
					this.Response.SuppressContent = true;
				}
			}
			catch (Exception ex)
			{
				this.ErrorHandler(new Exception("Error rendering JET report. " + ex.Message));
			}
		}

		private void RomanJetFileFormPreRender(object sender, EventArgs e)
		{
			//        this.ExportButton.Visible = (PostingDate.Calendar.Visible) ? false : true;
		}

		/// <summary>
		///    This method will set the default Posting Year to today's year.
		/// </summary>
		private void SetPostingYear()
		{
			if (this.Session[RjfPostingYear] != null)
			{
				this.PostingYearTextBox.Text = (string)this.Session[RjfPostingYear];
			}
			else
			{
				DateTimeOffset dateNow = DateTimeOffset.Now;
				this.PostingYearTextBox.Text = dateNow.Year.ToString();
			}
		}

		/// <summary>
		///    This method handles the Transaction Type dropdown selection change event. It
		///    will save the selected index in session.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void TransactionTypeSelectionChange(object sender, EventArgs e)
		{
			this.Session.Add(RjfTransactionType, this.TransactionTypeDropdown.SelectedValue);
		}

		#endregion
	}
}