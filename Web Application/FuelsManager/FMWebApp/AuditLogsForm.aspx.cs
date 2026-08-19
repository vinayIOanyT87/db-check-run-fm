// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AuditLogsForm.aspx.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the AuditLogsForm type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FuelsManager.FMWebApp
{
	using System;
	using System.Collections.Generic;
	using System.Data;
	using System.Globalization;
	using System.Text;
	using System.Web.UI.WebControls;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.UtilityObjects;

	using global::FMWebApp;

	public partial class AuditLogsForm : FMAutoSubmitFormBase, IMenuDiscovery
	{
		#region Constants and Fields

		protected SiteClass CurrentSite;

		private const string AllText = "{All}";

        private string [] TypeIDs = {
					"Additive Profile - Additive",
					"Additive Profiles",
					"Alarm And Events",
					"Alarm Priorities",
					"Alarm Template",
					"Alarm Test Template",
					"Allocation - LineItem",
					"Allocations",
					"Application Strings",
					"Archived Users",
					"Asset Tracking Detail",
					"Asset Tracking Device",
					"Asset Tracking Icon Configuration",
					"Asset Tracking Map Configuration",
					"Auto Distribution Reason Code",
					"Auto Distribution Rule",
					"Auto Distribution Rule - Manager",
					"Auto Distribution Rule - Manager Group",
					"Auto Distribution Rule - Owner",
					"Auto Distribution Rule - Owner Group",
					"Auto Distribution Rule - Product",
					"Auto Distribution Rule - Product Group",
					"Auto Distribution Rule - Transaction Alias",
					"Closeout",
					"Companies",
					"Company - Authorized Carrier",
					"Company - Certificate and Permit",
					"Company - Product",
					"Company - Role",
					"Company - Schedule",
					"Company - Unavailable Inventory",
					"Company Group - Company",
					"Company Group - Product",
					"Configuration Settings",
					"Controller - Memo",
					"Data Dictionary",
					"Data Exchange Profiles",
					"Data Exchange Profiles - Ship To - Load ID",
					"Delivery Locations",
					"Dispatch Configuration",
					"Dispatch Grid",
					"Dispatch Grid - Column",
					"Dispatch Toolbar",
					"Dispatch Toolbar - Command",
					"E-mail Group - Category",
					"E-mail Group - E-mail Address",
					"E-mail Group - Priority",
					"E-mail Groups",
					"Enterprise Export/Import Settings",
					"Equipment",
					"Equipment - Maintenance",
					"Equipment - Process Variable",
					"Equipment - Quality Tag",
					"Equipment - Tag and License",
					"Equipment - Test and Inspection",
					"Equipment Appointment",
					"Equipment Type",
					"Equipment Type - Aircraft Tank",
					"Equipment Type - Required Qualifications",
					"Equipment Type - Required Training",
					"Export Requests",
					"External Station",
					"External Station General Configuration",
					"External Station - Product",
					"FCE Device",
					"FCEE Mapping",
					"Field Level Configuration",
					"Fuel Card",
					"Gates",
					"General Configuration",
					"House Cards",
					"Ledger Aggregate Column",
					"Ledger View - Product",
					"Ledger View - User Group",
					"List View - List View Field",
					"List Views",
					"Load Arm - Arm Permissive",
					"Load Arm - Component",
					"Load Arm - Component Permissive",
					"Load Arm - External Component",
					"Load Arm - External Component Blend Percentage",
					"Load Arm - External Component Permissive",
					"Load Arm - Injector",
					"Load Arm - Injector Permissive",
					"Load Arm - No Additive Permissive",
					"Load Arm - Process Variable",
					"Load Arm - Recipe",
					"Load Arm - Recipe Permissive",
					"Loading Hierarchy",
					"Loading Hierarchy - Load ID",
					"Maintenance Reasons",
					"Message",
					"Meters",
					"Movement History",
					"Movement Summary",
					"Movement Summary Columns",
					"Movement Summary Rows",
					"Notes",
					"Off-Loading Hierarchy",
					"Off-Loading Hierarchy - Load ID",
					"OPC Connections",
					"Owner Closeout",
					"Person - Schedule",
					"Personnel",
					"Personnel - License",
					"Personnel - Qualifications",
					"Personnel - Role",
					"Personnel - Training",
					"Personnel Appointment",
					"Point",
					"Point Access Group",
					"Point Group",
					"PointTemplate",
					"Product - Dot Hazardous Message",
					"Product - Footnote",
					"Product - Product Message",
					"Product Blend - Component",
					"Product Group - Entry Message",
					"Product Group - Exit Message",
					"Product Group - Product",
					"Products",
					"Qualifications",
					"Quality Tags",
					"Query Default Fields",
					"Query Settings",
					"Query Storage",
					"Query Storage - User Group",
					"Report - User Group",
					"Report Assignment",
					"Report Groups",
					"Reserve Level",
					"Ship To - Footnote",
					"Shipper - Footnote",
					"Site - Additive Profile",
					"Site - Alarm & Events",
					"Site - Alarm Event Category",
					"Site - Alarm Priority",
					"Site - All Report Configuration",
					"Site - Allocation Group",
					"Site - Auto Distribution Reason Code",
					"Site - Auto Distribution Rule",
					"Site - Company",
					"Site - Company Certificate And Permit",
					"Site - Company Group",
					"Site - Company Type",
					"Site - Data Dictionary",
					"Site - Delivery Location",
					"Site - Dispatch Configuration",
					"Site - Dot Hazardous Message",
					"Site - E-mail Address",
					"Site - E-mail Group",
					"Site - Entry Message",
					"Site - Equipment",
					"Site - Equipment Appointment",
					"Site - Equipment Tag and License",
					"Site - Equipment Test and Inspection",
					"Site - Equipment Type",
					"Site - Exit Message",
					"Site - Footnote",
					"Site - Fuel Card",
					"Site - Holiday",
					"Site - Ledger Aggregate Column",
					"Site - Ledger View",
					"Site - List View",
					"Site - Person",
					"Site - Personnel Appointment",
					"Site - Personnel License",
					"Site - Personnel Qualification",
					"Site - Personnel Training",
					"Site - Process Variable",
					"Site - Process Variable Message",
					"Site - Product",
					"Site - Product Group",
					"Site - Product Message",
					"Site - Quality Tag",
					"Site - Query Settings",
					"Site - Schedule",
					"Site - Site",
					"Site - Tank Appointment",
					"Site - Test",
					"Site - Test Set",
					"Site - Transaction Alias",
					"Site - User",
					"Site - User Data Configuration",
					"Site - User Group",
					"Site Ancillary Data",
					"Sites",
					"State - Footnote",
					"Station - Load Arm",
					"Station - Permissive",
					"Station - Process Variable",
					"Station - Required Qualifications",
					"Station - Required Training",
					"Stations",
					"Synchronization Settings",
					"System Settings",
					"Tank - Maintenance",
					"Tank - Meter",
					"Tank - Process Variable",
					"Tank - Quality Tag",
					"Tank Appointment",
					"Tank Group - Tanks",
					"Tank Groups",
					"Tanks",
					"Test",
					"Test - Equipment",
					"Test - Tank",
					"Test Set",
					"Test Set - Equipment",
					"Test Set - Tank",
					"Test Set - Test",
					"Transaction Alias - Associated Alias",
					"Transaction Alias - Fields",
					"Transaction Alias - Fields Placement",
					"Transaction Alias - Line Item User Data",
					"Transaction Alias - Line Item User Data Fields",
					"Transaction Alias - Product Exclusion",
					"Transaction Alias - Status",
					"Transaction Alias - User Data",
					"Transaction Alias - User Data Fields",
					"Transaction Alias - User Group",
					"Transaction Alias User Data",
					"Transaction Aliases",
					"Transaction Line Item User Data",
					"Transaction Line Items",
					"Transaction Notes",
					"Transaction PIDX",
					"Transaction Signature",
					"Transaction Sub Line Items",
					"Transaction Transport Line Items",
					"Transactions",
					"User - Menu Favorite",
					"User Data",
					"User Group - Company",
					"User Group - Right",
					"User Group - User",
					"User Groups",
					"Users"
            };


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

         if (security.HasRight(RIGHT.VIEW_AUDIT_LOGS) == false)
			{
				return null;
			}

			var menuItems = new List<FMMenuItem>();

			var menuItem = new FMMenuItem
				{
					MenuItemType = FMMenuItemType.OPERATIONS_SYSTEM_LOGS_AUDIT_LOG,
					RootMenuName = "Operations",
					CategoryName = "System Logs",
					ItemName = "Audit Log",
					NavigateUrl = "AuditLogsForm.aspx",
					ApplyDataDictionary = ApplyDataDictionary.Apply
				};

			menuItems.Add(menuItem);

			return menuItems;
		}

		#endregion

		#region Methods

		protected void ActionIDDropDownList_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (this.ActionIDDropDownList.SelectedValue == AllText)
			{
				this.Session.Remove("AuditLogActionID");
			}
			else
			{
				this.Session["AuditLogActionID"] = this.ActionIDDropDownList.SelectedValue;
			}
         this.Session.Remove("AuditLogID");
         Populate_IDDropDownList();
		}

		protected void IDDropDownList_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (this.IDDropDownList.SelectedValue == AllText)
			{
				this.Session.Remove("AuditLogID");
			}
			else
			{
				this.Session["AuditLogID"] = this.IDDropDownList.SelectedValue;
			}
		}

		protected override void OnInit(EventArgs e)
		{
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			this.InitializeComponent();
			this.IgnoreInputDisable = true;
			base.OnInit(e);
		}

		/// <summary>
		/// Handles the SelectedIndexChanged event of the PageSizeDropDown control.
		/// </summary>
		/// <param name="source">The source of the event.</param>
		/// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
		protected void PageSizeDropDownSelectedIndexChanged(object source, EventArgs e)
		{
			try
			{
				this.UpdateView();
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		protected void PageSizeDropDown_SelectedIndexChanged(object source, EventArgs e)
		{
			this.UpdateView();
		}

		protected void Page_Load(object sender, EventArgs e)
		{
			try
			{
				this.GetSecurity();
				this.CurrentSite = FMChannelHelper.MakeCall<ISites, SiteClass>(
																	 x =>
																	 x.Get(this.Security, this.Security.SiteGuid, true, false, false)
																);
				if (this.Page.IsPostBack == false)
				{
					DateTimeOffset today = TimeConverter.Today(this.CurrentSite);

               DateTimeFormatInfo dateFormat = this.Session["AuditLogDateFormat"] as DateTimeFormatInfo;

               string beginningDateString = this.Session["AuditLogBeginningDateTime"] as string;
               DateTimeOffset beginningDate;

               if (beginningDateString != null && dateFormat != null && DateTimeOffset.TryParse(beginningDateString, dateFormat, DateTimeStyles.None, out beginningDate))
					{
                  this.BeginningDateTime.Text = beginningDate.ToString(this.CurrentSite.GetDateTimeFormatInfo());			
					}
					else
					{
						this.BeginningDateTime.Text = today.ToString(this.CurrentSite.GetDateTimeFormatInfo());
					}

               string endingDateString = this.Session["AuditLogEndingDateTime"] as string;
               DateTimeOffset endingDate;

               if (endingDateString != null && dateFormat != null && DateTimeOffset.TryParse(endingDateString, dateFormat, DateTimeStyles.None, out endingDate))
					{
                  this.EndingDateTime.Text = endingDate.ToString(this.CurrentSite.GetDateTimeFormatInfo());			
					}
					else
					{
						this.EndingDateTime.Text = today.AddDays(1).ToString(this.CurrentSite.GetDateTimeFormatInfo());
					}

					// Populate ActionIDDropDownList
					string[] ActionIDs = { AllText, "Add", "Modify", "Purge" };
					ListItem Item = null;

					foreach (string ActionID in ActionIDs)
					{
						Item = new ListItem(this.GetTranslatedText(ActionID), ActionID);
						this.ActionIDDropDownList.Items.Add(Item);
					}

					// Populate TypeIDDropDownList
					var NewItem = new ListItem(this.GetTranslatedText(AllText), AllText);
					this.TypeIDDropDownList.Items.Add(NewItem);

               foreach (string TypeID in this.TypeIDs)
               {
						NewItem = new ListItem(this.GetTranslatedText(TypeID), TypeID);

						this.TypeIDDropDownList.Items.Add(NewItem);
						if ((this.Session["AuditLogTypeID"] != null) && (NewItem.Text == (string)this.Session["AuditLogTypeID"]))
    					{
							this.TypeIDDropDownList.SelectedIndex = this.TypeIDDropDownList.Items.Count - 1;
						}
					}

					// Populate UserDropDownList
					UserCollectionClass UserCollection = FMChannelHelper.MakeCall<IUsers, UserCollectionClass>(
																			 x =>
																			 x.Enumerate(this.Security)
																		);

					NewItem = new ListItem(this.GetTranslatedText(AllText), AllText);
					this.UserDropDownList.Items.Add(NewItem);
					int Index = 0;

					foreach (UserClass User in UserCollection)
					{
						NewItem = new ListItem(this.GetTranslatedText(User.ID), User.ID);
						foreach (ListItem ExistingItem in this.UserDropDownList.Items)
						{
							if (ExistingItem.Text.CompareTo(NewItem.Text) > 0)
							{
								Index = this.UserDropDownList.Items.IndexOf(ExistingItem);
								this.UserDropDownList.Items.Insert(Index, NewItem);

								if ((this.Session["AuditLogUser"] != null) && (NewItem.Text == (string)this.Session["AuditLogUser"]))
								{
									this.UserDropDownList.SelectedIndex = Index;
								}

								NewItem = null;
								break;
							}
						}

						if (NewItem != null)
						{
							this.UserDropDownList.Items.Add(NewItem);
							if (this.Session["AuditLogUser"] != null && NewItem.Text == (string)this.Session["AuditLogUser"])
							{
								this.UserDropDownList.SelectedIndex = this.UserDropDownList.Items.Count - 1;
							}
						}
					}

					// Populate SiteDropDownList
					ListItem newItem;

					if (this.CurrentSite.SiteGroup)
					{
						newItem = new ListItem("{All}", string.Empty);
						this.SiteDropDownList.Items.Add(newItem);
						newItem = new ListItem("{" + this.CurrentSite.ID + "}", this.CurrentSite.SiteGuid.ToString());
						this.SiteDropDownList.Items.Add(newItem);

						if ((this.Session["AuditLogSite"] != null) && (newItem.Text == (string)this.Session["AuditLogSite"]))
						{
							this.SiteDropDownList.SelectedIndex = this.SiteDropDownList.Items.Count - 1;
						}

						foreach (SiteToSiteMapClass childSiteMap in this.CurrentSite.SiteToSiteMapCollection)
						{
							newItem = new ListItem(childSiteMap.ChildSiteID, childSiteMap.ChildSiteGuid.ToString());
							this.SiteDropDownList.Items.Add(newItem);

							if ((this.Session["AuditLogSite"] != null) &&
							(newItem.Text == (string)this.Session["AuditLogSite"]))
								this.SiteDropDownList.SelectedIndex = this.SiteDropDownList.Items.Count - 1;
						}
					}
					else
					{
						newItem = new ListItem(this.CurrentSite.ID, this.CurrentSite.SiteGuid.ToString());
						this.SiteDropDownList.Items.Add(newItem);
					}

               // are we looking at archive data?
               if (this.Session["AuditLogUseArchiveData"] != null)
               {
                  this.ArchiveCheckBox.Checked = (bool)this.Session["AuditLogUseArchiveData"];
               }

               this.Session["AuditLogSite"] = this.SiteDropDownList.SelectedItem.Text;

					// Populate IDDropDownList
					//this.TypeIDDropDownList_SelectedIndexChanged(null, null);
					Populate_IDDropDownList();

               this.UpdateView();
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		protected void SiteDropDownList_SelectedIndexChanged(object sender, EventArgs e)
		{
         this.Session.Remove("AuditLogID");
         Populate_IDDropDownList();

		}

		protected void TypeIDDropDownList_SelectedIndexChanged(object sender, EventArgs e)
		{
         this.Session.Remove("AuditLogID");
         Populate_IDDropDownList();

		}

		protected void Populate_IDDropDownList()
		{
			try
			{
				this.IDDropDownList.Items.Clear();
				var NewItem = new ListItem(this.GetTranslatedText(AllText), AllText);
				this.IDDropDownList.Items.Add(NewItem);

            if (this.TypeIDDropDownList.SelectedValue == AllText)
            {
               this.Session.Remove("AuditLogTypeID");
            }
            else
            {
               this.Session["AuditLogTypeID"] = this.TypeIDDropDownList.SelectedValue; 
               var beginning = DateTimeOffset.Parse(this.BeginningDateTime.Text, this.CurrentSite.GetDateTimeFormatInfo());
               var ending = DateTimeOffset.Parse(this.EndingDateTime.Text, this.CurrentSite.GetDateTimeFormatInfo());

               var beginningDateAndTime = new DateAndTime(this.CurrentSite);
               var endingDateAndTime = new DateAndTime(this.CurrentSite);

               beginningDateAndTime.Value = beginning;
               endingDateAndTime.Value = ending;

               string actionID;
               if (this.Session["AuditLogActionID"] != null)
               {
                  actionID = (string)this.Session["AuditLogActionID"];
               }
               else
               {
                  actionID = "";
               }

               string typeID;

               if (this.Session["AuditLogTypeID"] != null)
               {
                  typeID = (string)this.Session["AuditLogTypeID"];
               }
               else
               {
                  typeID = "";
               }

               bool includeMemberSites = false;
					Guid siteGuid = this.Security.SiteGuid;
				
					if (this.SiteDropDownList.SelectedItem.Text == this.GetTranslatedText("{All}"))
					{
						includeMemberSites = true;
					}
					else
					{
						siteGuid = new Guid(this.SiteDropDownList.SelectedValue);
					}

               DataSet idDataSet;

					idDataSet = FMChannelHelper.MakeCall<IAuditLogs, DataSet>(
					auditLogs =>
					auditLogs.EnumerateIDs(
					this.Security, siteGuid, beginningDateAndTime.Value, endingDateAndTime.Value, actionID, typeID, includeMemberSites));

               if(idDataSet.Tables.Count == 1)
               {
                  foreach(DataRow row in idDataSet.Tables[0].Rows)
                  {
                        NewItem = new ListItem(row["ID"] as string, row["ID"] as string);
                        this.IDDropDownList.Items.Add(NewItem);
                  }
               }
            }
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		protected void UserDropDownList_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (this.UserDropDownList.SelectedValue == AllText)
			{
				this.Session.Remove("AuditLogUserID");
			}
			else
			{
				this.Session["AuditLogUserID"] = this.UserDropDownList.SelectedValue;
			}
		}

		private void AuditLogsDataGridPageIndexChanged(object source, DataGridPageChangedEventArgs e)
		{
			// if we are editing do not allow a page change
			if (this.AuditLogsDataGrid.EditItemIndex > -1)
			{
				return;
			}
			this.AuditLogsDataGrid.CurrentPageIndex = e.NewPageIndex;
			this.UpdateView();
		}

		private void AuditLogsDataGridItemDataBound(object source, DataGridItemEventArgs e)
		{
			if (e.Item.ItemIndex > -1)
			{
				DataRow row = ((DataView)this.AuditLogsDataGrid.DataSource).Table.Rows[(this.AuditLogsDataGrid.CurrentPageIndex * this.AuditLogsDataGrid.PageSize) + e.Item.ItemIndex];
				e.Item.Cells[0].Text = ((DateTimeOffset) row["CreatedDate"]).ToString("G", this.CurrentSite.GetDateTimeFormatInfo());
			}
		}


		/// <summary>
		///    This method will enumerate a list of audit log entries based on a filter and return
		///    a data view object.
		/// </summary>
		/// <returns></returns>
		private DataView EnumerateAuditLogs()
		{
			var beginning = this.BeginningDateTime.CurrentValue;
			var ending = this.EndingDateTime.CurrentValue;

			var beginningDateAndTime = new DateAndTime(this.CurrentSite);
			var endingDateAndTime = new DateAndTime(this.CurrentSite);

			beginningDateAndTime.Value = beginning;
			endingDateAndTime.Value = ending;


			string actionID;
			if (this.Session["AuditLogActionID"] != null)
			{
				actionID = (string)this.Session["AuditLogActionID"];
            this.ActionIDDropDownList.SelectedValue = actionID;
         }
			else
			{
				actionID = "";
			}

			string typeID;

			if (this.Session["AuditLogTypeID"] != null)
			{
				typeID = (string)this.Session["AuditLogTypeID"];
            Populate_IDDropDownList();
            this.TypeIDDropDownList.SelectedValue = typeID;
         }
			else
			{
				typeID = "";
			}

			string ID;

			if (this.Session["AuditLogID"] != null)
			{
				ID = (string)this.Session["AuditLogID"];
				this.IDDropDownList.SelectedValue = ID;
			}
			else
			{
				ID = "";
			}

			string userID;

			if (this.Session["AuditLogUserID"] != null)
			{
				userID = (string)this.Session["AuditLogUserID"];
				this.UserDropDownList.SelectedValue = userID;
			}
			else
			{
				userID = "";
			}

			Guid siteGuid = this.Security.SiteGuid;
			bool includeMemberSites = false;
         bool includeGlobalSites = false;
         if (this.SiteDropDownList.SelectedItem.Text == this.GetTranslatedText("{All}"))
			{
				includeMemberSites = true;
            includeGlobalSites = true;
         }
			else
			{
				siteGuid = new Guid(this.SiteDropDownList.SelectedValue);
			}

			this.Session["AuditLogSite"] = this.SiteDropDownList.SelectedItem.Text;

			Guid currentSiteGuid = this.Security.SiteGuid;

			this.Security.SiteGuid = siteGuid;

         bool useArchiveData = false;
         try
         {
               useArchiveData = (bool)this.Session["AuditLogUseArchiveData"];
         }
         catch (Exception)
         {
               useArchiveData = false;
         }


         DataSet auditLogDataSet;
			try
			{
				auditLogDataSet = FMChannelHelper.MakeCall<IAuditLogs, DataSet>(
					auditLogs => auditLogs.EnumerateForAuditLogPage(
						this.Security, beginningDateAndTime.Value, endingDateAndTime.Value, actionID, typeID, ID, userID,
						this.SourceFilterTextBox.Text, this.useDataDictionary, includeMemberSites, useArchiveData, includeGlobalSites));
			}
			finally
			{
				this.Security.SiteGuid = currentSiteGuid;
			}

			var auditLogDataView = new DataView(auditLogDataSet.Tables[0]);
			return auditLogDataView;
		}

		/// <summary>
		///    Required method for Designer support - do not modify
		///    the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.ExportButton.Command += new System.Web.UI.WebControls.CommandEventHandler(this.ExportButton_Command);
			this.RefreshButton.Command += new System.Web.UI.WebControls.CommandEventHandler(this.RefreshButton_Command);
			this.AuditLogsDataGrid.PageIndexChanged += new System.Web.UI.WebControls.DataGridPageChangedEventHandler(this.AuditLogsDataGridPageIndexChanged);
			this.AuditLogsDataGrid.ItemDataBound += new System.Web.UI.WebControls.DataGridItemEventHandler(this.AuditLogsDataGridItemDataBound);
		}

		private DataSet ConvertDisplayDataToDataSet(DataTable dataTable)
		{
		
			// The incoming dataTable is the n-number of rows and ~370 columns of underlying ledger data.

			// Dict holds the header text from the display grid and index of the column from ledger data table.
			var columnDictionary = new Dictionary<string, int>();

			// New table holds the pared-down set of data.  Only export what appears in the transaction list grid.
			var exportDataTable = new DataTable();

			foreach (DataGridColumn column in this.AuditLogsDataGrid.Columns)
			{
				var headerText = column.HeaderText;
				var columnVisible = column.Visible;

				if (headerText == "Edit" || !columnVisible)
					continue;

				var boundColumn = column as BoundColumn;
				if (boundColumn == null)
					continue;

				var columnName = boundColumn.DataField;  // The underlying name of the column in the gridview datatable.
				if (!string.IsNullOrEmpty(columnName) && dataTable.Columns[columnName] != null)
				{
					var columnIndex = dataTable.Columns[columnName].Ordinal;
					var dataType = dataTable.Columns[columnIndex].DataType;

					exportDataTable.Columns.Add(new DataColumn(headerText, dataType));

					columnDictionary.Add(headerText, columnIndex);
				}
			}

			foreach (DataRow row in dataTable.Rows)
			{
				var rowValues = new List<object>();
				foreach (var keyValuePair in columnDictionary)
				{
					var columnIndex = keyValuePair.Value;
					var columnValue = row[columnIndex];
					rowValues.Add(columnValue);
				}
				exportDataTable.Rows.Add(rowValues.ToArray());
			}

			var exportDataSet = new DataSet();
			exportDataSet.Tables.Add(exportDataTable);
			return exportDataSet;
		}
/*		protected void AcceptButton_Click(object sender, EventArgs e)
		{
			try
			{
				DataView logs = this.EnumerateAuditLogs();

				if (logs == null)
				{
					return;
				}

				var dataSet = ConvertDisplayDataToDataSet(logs.Table);
				if (dataSet == null)
				{
					return;
				}
				const string ReportName = "Audit Log";


				var exportHelper = new DataTableExportHelper(this.Response, dataSet, this.exportpassword.Text);
				exportHelper.ExportData("PDF", ReportName);
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}
		protected void CancelButton_Click(object sender, EventArgs e)
		{
		}*/
		
		private void ExportButton_Command(object sender, CommandEventArgs e)
		{
			try
			{
				this.Response.ClearContent();
				this.Response.ClearHeaders();
				this.Response.ContentType = "text/csv";
				this.Response.AddHeader("Content-Disposition", "attachment; filename=AuditLog.csv");

				DataView logs = this.EnumerateAuditLogs();
				byte[] output;
				byte[] crlf = Encoding.UTF8.GetBytes("\r\n");
				int columnCount = 0;
				foreach (BoundColumn column in AuditLogsDataGrid.Columns)
				{

					output = Encoding.UTF8.GetBytes(column.HeaderText + ((++columnCount == AuditLogsDataGrid.Columns.Count) ? "" : CurrentSite.ListSeparator));
					this.Response.OutputStream.Write(output,0,output.Length);
				}

				this.Response.OutputStream.Write(crlf, 0, crlf.Length);

				foreach (DataRow row in logs.Table.Rows)
				{
					columnCount = 0;
					foreach (BoundColumn column in AuditLogsDataGrid.Columns)
					{
						if (row[column.DataField] is string)
						{
							output =
								Encoding.UTF8.GetBytes(
									row[column.DataField] as string
									+ ((++columnCount == AuditLogsDataGrid.Columns.Count) ? "" : CurrentSite.ListSeparator));
						}

						else if (row[column.DataField] is DateTimeOffset)
						{
							output =
								Encoding.UTF8.GetBytes(
									((DateTimeOffset)row[column.DataField]).ToString(CurrentSite.GetDateTimeFormatInfo())
									+ ((++columnCount == AuditLogsDataGrid.Columns.Count) ? "" : CurrentSite.ListSeparator));
						}

						else
						{
							output =
								Encoding.UTF8.GetBytes("Unsupported Data Type"
									+ ((++columnCount == AuditLogsDataGrid.Columns.Count) ? "" : CurrentSite.ListSeparator));
						}

						this.Response.OutputStream.Write(output, 0, output.Length);
					}

					this.Response.OutputStream.Write(crlf, 0, crlf.Length);
				}

				this.Response.Flush();
				this.Response.SuppressContent = true;
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		private void RefreshButton_Command(object sender, CommandEventArgs e)
		{
			try
			{
				//verify beginning date recent than end date
				if (DateTimeOffset.Parse(this.BeginningDateTime.Text, this.CurrentSite.GetDateTimeFormatInfo())
				    > DateTimeOffset.Parse(this.EndingDateTime.Text, this.CurrentSite.GetDateTimeFormatInfo()))
				{
					throw new Exception("Ending Date must be more recent than Beginning Date");
				}

				this.Session["AuditLogBeginningDateTime"] = this.BeginningDateTime.Text;
				this.Session["AuditLogEndingDateTime"] = this.EndingDateTime.Text;
			   this.Session["AuditLogDateFormat"] = this.CurrentSite.GetDateTimeFormatInfo();
            this.Session["AuditLogUseArchiveData"] = this.ArchiveCheckBox.Checked;

            this.AuditLogsDataGrid.CurrentPageIndex = 0;
				this.UpdateView();
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		/// <summary>
		///    This method will update the grid view with new audit log information
		///    based on the filters.
		/// </summary>
		private void UpdateView()
		{
			DataView logs = this.EnumerateAuditLogs();
			this.AuditLogsPageSizeDropDown.SetPageSize(this.AuditLogsDataGrid, logs.Count);
			this.AuditLogsDataGrid.DataSource = logs;
			this.AuditLogsDataGrid.DataBind();
		}

		#endregion
	}
}