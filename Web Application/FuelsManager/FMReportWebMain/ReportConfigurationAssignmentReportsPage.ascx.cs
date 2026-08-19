// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ReportConfigurationAssignmentReportsPage.ascx.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the ReportConfigurationAssignmentReportsPage type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FuelsManager.FMReportWebMain
{
	using System;
	using System.Collections.Generic;
	using System.Data;
    using System.Web;
    using System.Web.UI;
    using System.Web.UI.WebControls;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.ServiceRequests;

    using FMCore;

	using FuelsManager.FMWebApp;

	public partial class ReportConfigurationAssignmentReportsPage : FMUserControlBase
	{
		#region Constants and Fields

		private string errorMsg001 = "Invalid entry";

		private string errorMsg002 = "Business objects not available";

		private string errorMsg003 = "Report Detail/Group is null";

		private string reportUrl;

		#endregion

		#region Methods

		/// <summary>
		///    This method will transfer control to the report configuration group page that will
		///    allow the user to add/modify/delete a new report group item.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void AddGroupButtonOnClick(object sender, EventArgs e)
		{
			this.Redirect(this.reportUrl + "ReportConfigurationGroupPage.aspx");
		}

		/// <summary>
		///    This method will transfer control to the report configuration detail page that will
		///    allow the user to add a new report detail item.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void AddReportButtonOnClick(object sender, EventArgs e)
		{
			try
			{
				// Create a new report detail data object to store the new row.
				var reportDetailDO = new ReportConfigurationDetailDO();
				this.Session.Add("ReportConfigurationDetailDO", reportDetailDO);

				// Create a request to retrieve all the report group data from the database.
				var reportGroupSR = new ReportConfigurationGroupSR
				                    {
					                    CurrentSiteGuid = this.Security.SiteGuid,
					                    RequestType = ReportConfigurationGroupSR.RequestTypes.GET_ALL,
					                    Security = this.Security
				                    };

				ReportConfigurationGroupListDO reportGroupListDO =
					FMChannelHelper.MakeCall<IReportConfigurationGroupProcessor, ReportConfigurationGroupListDO>(
							x =>
							x.GetAll(reportGroupSR)
					);

				this.Session.Add("ReportConfigurationGroupListDO", reportGroupListDO);

				// Transfer control to the report configuration detail page.
				this.Redirect(this.reportUrl + "ReportConfigurationDetailPage.aspx");
			}
			catch (Exception exception)
			{
				string msg = exception.Message;

				if (msg.StartsWith("Thread was being aborted.") == false)
				{
					this.HandleErrorCondition(exception.Message);
				}
			}
		}

		protected void CreateDefaultReportsAssignmentButtonOnClick(object sender, EventArgs e)
		{
			try
			{
				FMChannelHelper.MakeCall<IReportConfigurationDetailProcessor>(
																x =>
																x.CreateDefaultReportAssignments(this.Security)
													);

				// Refresh the menu since the list of reports was updated
				if (this.Page is ReportConfigurationSettingsPage)
				{
					var parentForm = this.Page as ReportConfigurationSettingsPage;
					parentForm.MenuBar.Refresh();
				}

				this.LoadPageData();
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		protected void AssignmentDataGridItemDataBound(object sender, DataGridItemEventArgs e)
		{
			var deleteButton = (LinkButton)e.Item.FindControl("DeleteButton");

			if (deleteButton != null)
			{
				TableCell siteGuidCell = e.Item.Cells[5];//bds

				if (!this.Security.HasRight(RIGHT.MODIFY_REPORTS) || this.Security.SiteGuid != Guid.Parse(siteGuidCell.Text))
				{
					deleteButton.Enabled = false;
				}
			}
		}

		/// <summary>
		///    This method is called when the user wants to re-order an item. It will move the item up
		///    and then save it to the database.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void GridMoveItemCommand(object sender, EventArgs e)
		{
			var assignmentDataGrid = (DataGrid)sender;
			int itemIndex = (assignmentDataGrid.CurrentPageIndex * assignmentDataGrid.PageSize)
								 + assignmentDataGrid.SelectedIndex;

			try
			{
				// Get the list data.
				ReportConfigurationDetailListDO detailListDO = this.GetAllReportDetails();
				List<ReportConfigurationDetailDO> detailList = detailListDO.ReportDetailDOList;

				// Ensure that the index is within bounds. If so, then remove the item from the 
				// database.
				if ((itemIndex >= 0) && (itemIndex < detailList.Count))
				{
					// Retrieve the detail that is being moved and remove it from the list.
					ReportConfigurationDetailDO detailDO = detailList[itemIndex];
					detailList.RemoveAt(itemIndex);

					// If the detail being moved is the first one, then add it to the bottom
					// of the list. Otherwise, insert it before the previous one.
					if (itemIndex == 0)
					{
						detailList.Add(detailDO);
					}
					else
					{
						detailList.Insert(itemIndex - 1, detailDO);
					}

					// Starting at the being of the group list, renumber the order starting at 1.
					int orderNumber = 1;
					foreach (ReportConfigurationDetailDO tempDetailDO in detailList)
					{
						tempDetailDO.OrderNumber = orderNumber;
						orderNumber++;
					}

					var detailSR = new ReportConfigurationDetailSR
					               {
						               RequestType = ReportConfigurationDetailSR.RequestTypes.UPDATE_ORDER,
						               CurrentSiteGuid = this.Security.SiteGuid,
						               ReportConfigurationDetailList = detailList,
						               Security = this.Security
					               };

					FMChannelHelper.MakeCall<IReportConfigurationDetailProcessor>(
																	 x =>
																	 x.UpdateOrder(detailSR)
																);

					// Refresh the menu since changing the order here affects how reports appear in the menu
					if (this.Page is ReportConfigurationSettingsPage)
					{
						var parentForm = this.Page as ReportConfigurationSettingsPage;
						parentForm.MenuBar.Refresh();
					}
				}

				this.AssignmentDataGrid.SelectedIndex = -1;
				this.LoadPageData();
			}
			catch (Exception)
			{
				this.HandleErrorCondition(this.errorMsg002 + "!");
			}
		}

		protected override void OnInit(EventArgs e)
		{
			this.InitializeComponent();
			base.OnInit(e);
		}

		protected void PageSizeDropDownSelectedIndexChanged(object source, EventArgs e)
		{
			this.LoadPageData();
		}

		/// <summary>
		///    This is the main entry point to the report configuration assignment page.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void Page_Load(object sender, EventArgs e)
		{
			try
			{
				this.reportUrl = "../FMReportWebMain/";

				if (this.Page.IsPostBack)
				{
					// Apply the data dictionary to the page.
					this.ApplyDataDictionary();
				}
				else
				{
					try
					{
						// Are we going to the group page?
						string sValue = this.Request.GetQueryOrFormValue("Group");
						if (sValue == "Yes")
						{
							this.Redirect("..\\FMReportWebMain\\ReportConfigurationGroupPage.aspx");
						}

						// Apply the data dictionary to the page.
						this.ApplyDataDictionary();

						// Load the contains of this page.
						this.LoadPageData();

						// Disable the add buttons if the user does not have modify permissions.
						this.CheckPriviledges();
					}
					catch (Exception exception)
					{
						string msg = exception.Message;

						if (msg.StartsWith("Thread was being aborted.") == false)
						{
							this.HandleErrorCondition(this.errorMsg002 + "!");
						}
					}
				}
			}
			catch (Exception ex)
			{
				this.ErrorHandler(ex);
			}
		}

		/// <summary>
		///    This method will apply the data dictionary to this page.  If the data dictionary
		///    use flag is set to true, then it will apply data dictionary.
		/// </summary>
		private void ApplyDataDictionary()
		{
			string newText = this.GetTranslatedText(this.errorMsg001);
			this.errorMsg001 = newText;

			newText = this.GetTranslatedText(this.errorMsg002);
			this.errorMsg002 = newText;

			newText = this.GetTranslatedText(this.errorMsg003);
			this.errorMsg003 = newText;

			// Apply data dictionary to the Edit column.
			DataGridColumnCollection columns = this.AssignmentDataGrid.Columns;
			newText = this.GetTranslatedText(columns[0].HeaderText);
			columns[0].HeaderText = newText;

			// Apply data dictionary to the Delete column.
			newText = this.GetTranslatedText(columns[6].HeaderText);//bds
			columns[6].HeaderText = newText;//bds

			// Apply data dictionary to the Reports column.
			newText = this.GetTranslatedText(columns[2].HeaderText);//bds
			columns[2].HeaderText = newText;//bds

			// Apply data dictionary to the Groups column.
			newText = this.GetTranslatedText(columns[3].HeaderText);//bds
			columns[3].HeaderText = newText;//bds

			// Apply data dictionary to the Order column.
			newText = this.GetTranslatedText(columns[4].HeaderText);//bds
			columns[4].HeaderText = newText;//bds
		}

		/// <summary>
		///    This method will build the data view that the report group data grid can bind to.
		///    It will return a data view object.
		/// </summary>
		/// <param name="reportGroupDOList"></param>
		/// <param name="reportDetailDOList"></param>
		/// <returns></returns>
		private DataView BuildGridData(List<ReportConfigurationGroupDO> reportGroupDOList, List<ReportConfigurationDetailDO> reportDetailDOList)
		{
			var dataTable = new DataTable();

			dataTable.Columns.Add("ReportGuid", typeof(Guid));
			dataTable.Columns.Add("ReportName", typeof(string));
			dataTable.Columns.Add("GroupName", typeof(string));
			dataTable.Columns.Add("SiteGuid", typeof(Guid));

			// If the list is empty, then we want to create an empty row for display nicely.
			// Else, create the normal way.
			if (reportDetailDOList.Count != 0)
			{
				foreach (ReportConfigurationDetailDO reportDetailDO in reportDetailDOList)
				{
					DataRow dataRow = dataTable.NewRow();

					dataRow["ReportGuid"] = reportDetailDO.ReportGuid;
					dataRow["ReportName"] = reportDetailDO.ReportName;
					dataRow["SiteGuid"] = reportDetailDO.SiteGuid;

					foreach (ReportConfigurationGroupDO reportGroupDO in reportGroupDOList)
					{
						if (reportDetailDO.ReportGroupGuid == reportGroupDO.ReportGroupGuid)
						{
							dataRow["GroupName"] = reportGroupDO.GroupName;
							break;
						}
					}

					dataTable.Rows.Add(dataRow);
				}
			}

			var dataView = new DataView(dataTable);
			return dataView;
		}

		/// <summary>
		///    This method will check the security privileges and disable edit type functionality.
		/// </summary>
		private void CheckPriviledges()
		{
			if (this.Security.HasRight(RIGHT.MODIFY_REPORTS) == false)
			{
				this.AddReportButton.Enabled = false;
				this.AddReportButton2.Enabled = false;

				this.AddGroupButton.Enabled = false;
				this.AddGroupButton2.Enabled = false;

				this.CreateDefaultReportsAssignmentButton.Enabled = false;

			}
		}

		/// <summary>
		///    This method will return the report configuration detail list DOs in the database for a
		///    given site.
		/// </summary>
		/// <returns></returns>
		private ReportConfigurationDetailListDO GetAllReportDetails()
		{
			// Setup the report detail service request.
			var detailSR = new ReportConfigurationDetailSR
			               {
				               RequestType = ReportConfigurationDetailSR.RequestTypes.GET_ALL,
				               CurrentSiteGuid = this.Security.SiteGuid,
				               Security = this.Security
			               };

			// Send request to get all the group DOs from the database.
			ReportConfigurationDetailListDO reportDetailListDO = null;

			try
			{
				reportDetailListDO = FMChannelHelper.MakeCall<IReportConfigurationDetailProcessor, ReportConfigurationDetailListDO>(
					reportConfigurationProcessor => reportConfigurationProcessor.GetAll(detailSR));
			}
			catch (Exception ex)
			{
				this.HandleErrorCondition(ex.Message);
			}

			return reportDetailListDO;
		}

		/// <summary>
		///    This method will return the report configuration detail in the database for a
		///    given guid.
		/// </summary>
		/// <returns></returns>
		private ReportConfigurationDetailDO GetReportDetails(Guid reportGuid)
		{
			// Setup the report detail service request.
			var detailSR = new ReportConfigurationDetailSR
			               {
				               RequestType = ReportConfigurationDetailSR.RequestTypes.GET,
				               CurrentSiteGuid = this.Security.SiteGuid,
				               ReportConfigurationDetailDO =
					               new ReportConfigurationDetailDO { ReportGuid = reportGuid },
				               Security = this.Security
			               };

			// Send request to get all the group DOs from the database.
			ReportConfigurationDetailDO reportDetailDO = null;

			try
			{
				reportDetailDO = FMChannelHelper.MakeCall<IReportConfigurationDetailProcessor, ReportConfigurationDetailDO>(
																	 x =>
																	 x.GetConfiguration(detailSR)
																);
			}
			catch (Exception ex)
			{
				this.HandleErrorCondition(ex.Message);
			}

			return reportDetailDO;
		}

		/// <summary>
		///    This method will remove the select item from the report detail list and remove it from the
		///    database.
		/// </summary>
		/// <param name="source"></param>
		/// <param name="e"></param>
		private void GridDeleteCommand(object source, DataGridCommandEventArgs eventArgs)
		{
			try
			{
				// Find the identity guid to be deleted.
				TableCell identityGuidCell = eventArgs.Item.Cells[1];//bds
				Guid reportGuid = Guid.Parse(identityGuidCell.Text);

				var detailSR = new ReportConfigurationDetailSR
				               {
					               RequestType = ReportConfigurationDetailSR.RequestTypes.DELETE,
					               CurrentSiteGuid = this.Security.SiteGuid,
					               ReportConfigurationDetailDO = new ReportConfigurationDetailDO { ReportGuid = reportGuid },
								   Security = this.Security
				               };

				FMChannelHelper.MakeCall<IReportConfigurationDetailProcessor>(
																	 x =>
																	 x.Delete(detailSR)
																);

				// Refresh the menu since a report was deleted
				if (this.Page is ReportConfigurationSettingsPage)
				{
					var parentForm = this.Page as ReportConfigurationSettingsPage;
					parentForm.MenuBar.Refresh();
				}

				this.LoadPageData();
			}
			catch (Exception)
			{
				this.HandleErrorCondition(this.errorMsg002 + "!");
			}
		}

		/// <summary>
		///    This method will find the selected item to be edited and transfer control to the report
		///    configuration detail page for editing.
		/// </summary>
		/// <param name="source"></param>
		/// <param name="eventArgs"></param>
		private void GridEditCommand(object source, DataGridCommandEventArgs eventArgs)
		{
			try
			{
				// Find the guid to be deleted.
				TableCell identityGuidCell = eventArgs.Item.Cells[1];//bds
				Guid reportGuid = Guid.Parse(identityGuidCell.Text);

				// Add the select report detail data object to the session.
				ReportConfigurationDetailDO detailDO = this.GetReportDetails(reportGuid);
				this.Session.Add("ReportConfigurationDetailDO", detailDO);

				// Setup the retrieval request of all the report groups for this site.
				var groupSR = new ReportConfigurationGroupSR
				              {
					              RequestType = ReportConfigurationGroupSR.RequestTypes.GET_ALL,
					              CurrentSiteGuid = this.Security.SiteGuid,
					              Security = this.Security
				              };

				// Retrieve all the report groups from this site and place into the session.
				var reportGroupListDO = FMChannelHelper.MakeCall<IReportConfigurationGroupProcessor, ReportConfigurationGroupListDO>(
																		 x =>
																		 x.GetAll(groupSR)
																	);

				this.Session.Add("ReportConfigurationGroupListDO", reportGroupListDO);

				// Transfer control to the report detail page for editing.
				this.Redirect(this.reportUrl + "ReportConfigurationDetailPage.aspx");
			}
			catch (Exception exception)
			{
				string msg = exception.Message;

				if (msg.StartsWith("Thread was being aborted.") == false)
				{
					this.HandleErrorCondition(this.errorMsg002 + "!");
				}
			}
		}

		/// <summary>
		///    This method is called when the user selects another page.
		/// </summary>
		/// <param name="source"></param>
		/// <param name="eventArgs"></param>
		private void GridPageIndexCommand(object source, DataGridPageChangedEventArgs eventArgs)
		{
			this.AssignmentDataGrid.CurrentPageIndex = eventArgs.NewPageIndex;
			this.LoadPageData();
		}

		/// <summary>
		///    This method will check to see if there is an error, if so, then it will display an
		///    error dialog and transfer control to the error page.
		/// </summary>
		/// <param name="erroMsg"></param>
		private void HandleErrorCondition(string errMsg)
		{
			if (string.IsNullOrEmpty(errMsg) == false)
			{
				((FMFormBase)this.Page).ErrorHandler("FuelsManager", errMsg);
			}
		}

		/// <summary>
		///    Required method for Designer support - do not modify
		///    the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.AssignmentDataGrid.PageIndexChanged += this.GridPageIndexCommand;
			this.AssignmentDataGrid.EditCommand += this.GridEditCommand;
			this.AssignmentDataGrid.DeleteCommand += this.GridDeleteCommand;
		}

		/// <summary>
		///    This method will load group data into the grid on the group configuration page.
		/// </summary>
		private void LoadPageData()
		{
			var reportDetailSR = new ReportConfigurationDetailSR();
			var reportGroupSR = new ReportConfigurationGroupSR();

			reportDetailSR.CurrentSiteGuid = this.Security.SiteGuid;
			reportDetailSR.RequestType = ReportConfigurationDetailSR.RequestTypes.GET_ALL;
			reportDetailSR.Security = this.Security;

			reportGroupSR.CurrentSiteGuid = this.Security.SiteGuid;
			reportGroupSR.RequestType = ReportConfigurationGroupSR.RequestTypes.GET_ALL;
			reportGroupSR.Security = this.Security;

			ReportConfigurationGroupListDO reportGroupListDO = null;
			ReportConfigurationDetailListDO reportDetailListDO = null;

			try
			{
				reportGroupListDO = FMChannelHelper.MakeCall<IReportConfigurationGroupProcessor, ReportConfigurationGroupListDO>(
																	 x =>
																	 x.GetAll(reportGroupSR)
																);

				reportDetailListDO = FMChannelHelper.MakeCall<IReportConfigurationDetailProcessor, ReportConfigurationDetailListDO>(
																	 x =>
																	 x.GetAll(reportDetailSR)
																);

			}
			catch (Exception ex)
			{
				this.HandleErrorCondition(ex.Message);
			}

			// If the report group list data object exist, which it should, then bind the data the
			// assignment data grid.
			if ((reportGroupListDO != null) && (reportDetailListDO != null))
			{
				DataView view = this.BuildGridData(reportGroupListDO.ReportGroupDOList, reportDetailListDO.ReportDetailDOList);

				this.AssignmentDataGrid.DataSource = view;
				this.ReportGroupsFormPageSizeDropDown.SetPageSize(this.AssignmentDataGrid, view.Count);
				this.AssignmentDataGrid.DataBind();
			}
			else
			{
				this.HandleErrorCondition(this.errorMsg003 + "!");
			}
		}

		#endregion
	}
}