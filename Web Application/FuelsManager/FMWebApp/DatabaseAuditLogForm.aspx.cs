// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DatabaseAuditLogForm.aspx.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the DatabaseAuditLogForm type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FuelsManager.FMWebApp
{
	using System;
	using System.Collections.Generic;
	using System.Web.UI.WebControls;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.UtilityObjects;

	using FuelsManager.FMReportWebMain;
    using FMCore;
	using global::FMWebApp;

	using Microsoft.Reporting.WebForms;

	/// <summary>
	///    Summary description for DatabaseAuditLogForm.
	/// </summary>
	public partial class DatabaseAuditLogForm : FMAutoSubmitFormBase, IMenuDiscovery
	{
		#region Constants and Fields

		protected SiteClass LoginSite;

		private string selectThisItemText = String.Empty;

		private ManageSecurity manageSecurity;

		#endregion

		//RequestParser requestParser = null;
		//ParameterParser parmParser = null;

		#region Public Properties

		public string SelectThisItemText
		{
			get
			{
				return this.selectThisItemText;
			}
		}

		#endregion

		#region Public Methods and Operators

		/// <summary>
		///    Gets a list of menu items that should be displayed for the current user.
		/// </summary>
		/// <param name="security">The security object of the current session</param>
		/// <param name="siteGroup">Whether the current logged-in site is a site group</param>
		/// <param name="options">Hardware key options</param>
		/// <returns>
		///    List of menu items to be displayed
		/// </returns>
		public List<FMMenuItem> GetMenuItems(SecurityClass security, bool siteGroup, ushort word1,ushort word2, ushort useNewLicenseKey, uint options)
		{
            if (useNewLicenseKey == 1)
            {

            }
            else
            {
                // Depends Upon Shared Components Config
                if ((options & 0x4000) == 0)
                {
                    return null;
                }
            }

            if (security.HasRight(RIGHT.VIEW_DATABASE_AUDIT_LOG) == false)
			{
				return null;
			}


			var menuItems = new List<FMMenuItem>();

			var menuItem = new FMMenuItem
				{
					MenuItemType = FMMenuItemType.OPERATIONS_SYSTEM_LOGS_DATABASE_AUDIT,
					RootMenuName = "Operations",
					CategoryName = "System Logs",
					ItemName = "Database Audit Log",
					NavigateUrl = "DatabaseAuditLogForm.aspx",
					ApplyDataDictionary = ApplyDataDictionary.Apply
				};

			menuItems.Add(menuItem);

			return menuItems;
		}

		#endregion

		#region Methods

		protected void DisplayErrorDialog(string errorMessage)
		{
			string errMsg = "An Error has occurred!";
			
			if (!string.IsNullOrEmpty(errorMessage))
			{
				errMsg = errorMessage;
			}

			this.RenderErrorMessage(errMsg);
		}

		protected ListItemCollection EnumerateUserNames()
		{
			var loginNames = new ListItemCollection { new ListItem("{All}", Guid.Empty.ToString()) };
			UserCollectionClass userCollectionClass = FMChannelHelper.MakeCall<IUsers, UserCollectionClass>(x => x.Enumerate(this.Security));

			foreach (UserClass user in userCollectionClass)
			{
				var listItem = new ListItem(user.ID, user.IdentityGuid.ToString());
				loginNames.Add(listItem);
			}
			return loginNames;
		}

		protected override void OnInit(EventArgs e)
		{
			this.InitializeComponent();
			base.OnInit(e);
			GetSecurity();

			if (this.Page.IsPostBack)
			{
			}
		}

		protected void OnViewReportClick(object sender, EventArgs e)
		{
			try
			{
				string file = this.Request.GetQueryOrFormValue("file");
				if (string.IsNullOrEmpty(file))
				{
					throw new Exception("Trace file name is missing.");
				}
			}
			catch (Exception ex)
			{
				this.ErrorHandler(new Exception("Error retrieving trace data.", ex));
				return;
			}

			this.RptViewer.Visible = true;

			// Build the security object to ensure that the request is coming
			// from the FuelsManager application.
			this.manageSecurity = new ManageSecurity();
			this.manageSecurity.BuildSecurity(Security);

			// If the request is valid, then parse the request and create the report viewer parameters.
			if (this.manageSecurity.IsSecurityValid)
			{
				var rptLocation = new ReportLocation(this.manageSecurity);
				this.RptViewer.ProcessingMode = ProcessingMode.Remote;

				try
				{
					this.RptViewer.ServerReport.ReportServerUrl = new Uri(rptLocation.ReportServerUri);
					this.RptViewer.ServerReport.ReportPath = rptLocation.ReportPath + "/BSM-E AuditLog Report";

					if (FMChannelHelper.MakeCall<IHardwareKey, bool>(x => x.IsDescKey()))
					{
						var credentialsList = new DataSourceCredentials[1];
						credentialsList[0] = new DataSourceCredentials();
						credentialsList[0].Password =
							FMChannelHelper.MakeCall<IDBAccess, string>(x => x.GetDBPassword(base.Security.Password));

						credentialsList[0].UserId = base.Security.UserID;
						credentialsList[0].Name = "ConsolidatedDBDataSource";
						this.RptViewer.ServerReport.SetDataSourceCredentials(credentialsList);
					}

					var parameters = new ReportParameter[2];
					parameters[0] = new ReportParameter("BeginDate", this.BeginningDate.CurrentValue.ToString("d"));
					parameters[1] = new ReportParameter("EndDate", this.EndingDate.CurrentValue.ToString("d"));
					this.RptViewer.ServerReport.SetParameters(parameters);
					this.RptViewer.ShowParameterPrompts = true;
					this.RptViewer.ShowPromptAreaButton = true;
					this.RptViewer.ZoomMode = ZoomMode.Percent;
					this.RptViewer.ZoomPercent = 100;
					this.RptViewer.Visible = true;
				}
					// ReSharper disable once EmptyGeneralCatchClause
				catch
				{
					//nothing
				}
			}
			else
			{
				this.RptViewer.Visible = false;
				this.ErrorLabel.Visible = true;
				this.ErrorLabel.Text = "Error in render report.";
			}
		}

		/// <summary>
		///    This event handles the loading of the page.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void Page_Load(object sender, EventArgs e)
		{
			try
			{
				this.GetSecurity();

				this.selectThisItemText = this.GetTranslatedText("Select This Item");

				if (this.Page.IsPostBack == false)
				{
					this.LoginNameDropDownList.DataBind();
				}

				// Put user code to initialize the page here
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		/// <summary>
		///    Required method for Designer support - do not modify
		///    the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
		}

		private bool ValidateDates()
		{
			bool breturn = false;

			try
			{
				// Check for empty dates
				if (0 == this.BeginningDate.Text.Length || 0 == this.EndingDate.Text.Length)
				{
					this.DisplayErrorDialog("Cannot have empty date(s).");
				}
				else
				{
					// Gather and Validate Begin and End dates

					DateTimeOffset begindate = DateTimeOffset.Parse(this.BeginningDate.Text, this.LoginSite.GetDateTimeFormatInfo());
					DateTimeOffset enddate = DateTimeOffset.Parse(this.EndingDate.Text, this.LoginSite.GetDateTimeFormatInfo());
					if (begindate > enddate)
					{
						this.DisplayErrorDialog("Beginning date greater than ending date.");
					}
					else
					{
						breturn = true;
					}
				}
			}
			catch (FormatException fe)
			{
				this.DisplayErrorDialog(fe.Message);
			}
			return breturn;
		}

		#endregion
	}

	public class DatabaseAuditLogContext
	{
		#region Constants and Fields

		public string AuditLogFileName;

		public DateTimeOffset Beginning;

		public DateTimeOffset Ending;

		public int EventIndex;

		public int LoginNameIndex;

		public int ResultIndex;

		#endregion

		#region Constructors and Destructors

		public DatabaseAuditLogContext(SiteClass site)
		{
			DateTimeOffset today = TimeConverter.Today(site);

			this.LoginNameIndex = 0;
			this.Beginning = today;
			this.Ending = today.AddDays(1);
			this.ResultIndex = 0;
			this.EventIndex = 0;
			this.AuditLogFileName = "";
		}

		#endregion
	}
}