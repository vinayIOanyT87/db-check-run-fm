// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ReportConfigurationGroupPage.aspx.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the ReportConfigurationGroupPage type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FuelsManager.FMReportWebMain
{
	using System;
	using System.Collections.Generic;
	using System.Data;
	using System.Web.UI.WebControls;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.ServiceRequests;

	using FuelsManager.FMWebApp;

	/// <summary>
	///    Summary description for ReportConfigurationGroupPage.
	/// </summary>
	public partial class ReportConfigurationGroupPage : FMFormBase
	{
		#region Constants and Fields

		private const string SessionGroupListDO = "ReportConfigurationGroupPage.GroupListDO";

		private bool editOnAddFlag;

		private string errorMsg001 = "Invalid entry";
		private string errorMsg002 = "Report Detail/Group is null";
		private string errorMsg003 = "Group name is blank/max length (30) exceeded";
		private string errorMsg004 = "Business objects not available";

		private bool illegalEntry;

		private string reportUrl;

		#endregion

		#region Methods

		/// <summary>
		///    This method will add a new row to the group grid. It will also add the row to the
		///    database.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void AddGroupButtonOnClick(object sender, EventArgs e)
		{
			try
			{
				var groupListDO = (ReportConfigurationGroupListDO)this.Session[SessionGroupListDO];
				var groupDO = new ReportConfigurationGroupDO { GroupName = "", SiteGuid = this.Security.SiteGuid };
				groupListDO.ReportGroupDOList.Add(groupDO);

				// The edit on add flag indicates that a new item has been added and to place
				// it in edit mode.  True indicates that the item should be placed in edit mode.
				this.editOnAddFlag = true;
				// Disable the add and close buttons while editing.
				this.EnableControls(false);

				DataView view = this.BuildGridData(groupListDO.ReportGroupDOList);

				this.ReportGroupsFormPageSizeDropDown2.SetPageSize(this.GroupDataGrid, view.Count);

				this.GroupDataGrid.DataSource = view;
				this.GroupDataGrid.DataBind();
			}
			catch (Exception ex)
			{
				this.HandleErrorCondition(ex.Message + " " + this.errorMsg004 + "!");
			}
		}

		/// <summary>
		///    This method will perform the close opertions on this page. Control will be
		///    transfered to the report configuration assignment page.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void CloseButtonOnClick(object sender, EventArgs e)
		{
			this.Session.Remove(SessionGroupListDO);
			this.Redirect(this.reportUrl + "ReportConfigurationSettingsPage.aspx");
		}

		/// <summary>
		///    This method will capture the move event and reorder the groups according to
		///    the request. Only one item is moved at a time.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void GridMoveCommand(object sender, EventArgs e)
		{
			try
			{
				var groupDataGrid = (DataGrid)sender;
				int itemIndex = groupDataGrid.SelectedIndex;

				ReportConfigurationGroupListDO groupListDO = this.GetAllGroups(false);
				List<ReportConfigurationGroupDO> groupList = groupListDO.ReportGroupDOList;

				// Ensure that the item selected for editing is within bounds.
				if ((itemIndex >= 0) && (itemIndex < groupList.Count))
				{
					// Retrieve the group that is being moved and remove it from the list.
					ReportConfigurationGroupDO groupDO = groupList[itemIndex];
					groupList.RemoveAt(itemIndex);

					// If the group being moved is the first one, then add it to the bottom
					// of the list. Otherwise, insert it before the previous one.
					if (itemIndex == 0)
					{
						groupList.Add(groupDO);
					}
					else
					{
						groupList.Insert(itemIndex - 1, groupDO);
					}

					// Starting at the being of the group list, renumber the order starting at 1.
					int orderNumber = 1;
					foreach (ReportConfigurationGroupDO tempGroupDO in groupList)
					{
						tempGroupDO.OrderNumber = orderNumber;
						orderNumber++;
					}

					// Just same the updates to the order number.
					var groupSR = new ReportConfigurationGroupSR
					              {
						              RequestType = ReportConfigurationGroupSR.RequestTypes.UPDATE_ORDER,
						              CurrentSiteGuid = this.Security.SiteGuid,
						              ReportGroupList = groupList,
						              Security = this.Security
					              };

					try
					{
						FMChannelHelper.MakeCall<IReportConfigurationGroupProcessor>(
																	 x =>
																	 x.UpdateOrder(groupSR)
																);

						// Refresh the menu bar since the order has changed and the menu honors the order specified here
						this.ucFMMenuBar.Refresh();
					}
					catch (Exception ex)
					{
						this.HandleErrorCondition(ex.Message);
					}
				}

				this.EnableControls(true);
				this.GroupDataGrid.SelectedIndex = -1;
				this.LoadPageData(true);
			}
			catch (Exception ex)
			{
				this.HandleErrorCondition(ex.Message + " " + this.errorMsg004 + "!");
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

		protected void PageSizeDropDownSelectedIndexChanged(object source, EventArgs e)
		{
			this.LoadPageData(false);
		}

		/// <summary>
		///    This is the main entry point for the report group configuration page.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void Page_Load(object sender, EventArgs e)
		{
			try
			{
				this.reportUrl = "../FMReportWebMain/";

				this.GetSecurity();

				// Perform security checks
				if (this.Security == null)
				{
					this.illegalEntry = true;
				}
				else
				{
					this.illegalEntry = false;
				}

				// Display an error dialog if an illegal entry occurred.
				if (this.illegalEntry)
				{
					this.HandleErrorCondition(this.errorMsg001 + "!");
				}
				else
				{
					// Apply the data dictionary to this page.
					this.ApplyDataDictionary();

					// This flag is used to set the edit for the new added item
					// in the list. False indicates not to edit.
					this.editOnAddFlag = false;

					if (this.Page.IsPostBack == false)
					{
						// Load the contains of this page.
						this.LoadPageData(true);
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
			string newText = this.GetTranslatedText(this.GroupLabel.Text);
			this.GroupLabel.Text = newText;

			newText = this.GetTranslatedText(this.errorMsg001);
			this.errorMsg001 = newText;

			newText = this.GetTranslatedText(this.errorMsg002);
			this.errorMsg002 = newText;

			newText = this.GetTranslatedText(this.errorMsg003);
			this.errorMsg003 = newText;

			newText = this.GetTranslatedText(this.errorMsg004);
			this.errorMsg004 = newText;

			// Apply data dictionary to the Edit column.
			DataGridColumnCollection columns = this.GroupDataGrid.Columns;
			newText = this.GetTranslatedText(columns[0].HeaderText);
			columns[0].HeaderText = newText;

			// Apply data dictionary to the Delete column.
			newText = this.GetTranslatedText(columns[5].HeaderText);//bds
			columns[5].HeaderText = newText;//bds

			// Apply data dictionary to the Groups column.
			newText = this.GetTranslatedText(columns[2].HeaderText);//bds
			columns[2].HeaderText = newText;//bds

			// Apply data dictionary to the Order column.
			newText = this.GetTranslatedText(columns[3].HeaderText);//bds
			columns[3].HeaderText = newText;//bds
		}

		/// <summary>
		///    This method will build the data view that the report group data grid can bind to.
		///    It will return a data view object.
		/// </summary>
		/// <param name="reportGroupDOList"></param>
		/// <returns></returns>
		private DataView BuildGridData(IEnumerable<ReportConfigurationGroupDO> reportGroupDOList)
		{
			var dataTable = new DataTable();

			dataTable.Columns.Add("GroupGuid", typeof(Guid));
			dataTable.Columns.Add("GroupName", typeof(string));
			dataTable.Columns.Add("SiteGuid", typeof(string));

			// The edit item index is used to indicate the item that may have to
			// be placed in edit mode.  The count index keeps up with the current item.
			int countIndex = 0;
			int editItemIndex = 0;

			foreach (ReportConfigurationGroupDO reportGroupDO in reportGroupDOList)
			{
				DataRow dataRow = dataTable.NewRow();
				dataRow["GroupGuid"] = reportGroupDO.ReportGroupGuid;
				dataRow["GroupName"] = reportGroupDO.GroupName;
				dataRow["SiteGuid"] = reportGroupDO.SiteGuid.ToString();
				dataTable.Rows.Add(dataRow);

				// If the group name is equal to a blank, then this indicates
				// that this item is probably a newly added item.  Therefore,
				// save the index so that it can be placed in edit mode.
				if (reportGroupDO.GroupName == "")
				{
					editItemIndex = countIndex;
				}

				countIndex++;
			}

			// Determine if the user added a new item to the list. If so,
			// then place that item in edit mode. True indicates the add
			// button was pressed.
			if (this.editOnAddFlag)
			{
				this.GroupDataGrid.CurrentPageIndex = editItemIndex / this.GroupDataGrid.PageSize;
				this.GroupDataGrid.EditItemIndex = editItemIndex
															  - (this.GroupDataGrid.CurrentPageIndex * this.GroupDataGrid.PageSize);
			}

			var dataView = new DataView(dataTable);
			return dataView;
		}

		/// <summary>
		///    This method will return true if the string is with bounds.  Otherwise,
		///    it return false.
		/// </summary>
		/// <param name="strValue"></param>
		/// <returns></returns>
		private bool CheckLength(string strValue)
		{
			bool okay = false;

			if (!string.IsNullOrEmpty(strValue))
			{
				if (strValue.Length <= 30)
				{
					okay = true;
				}
			}

			return okay;
		}

		/// <summary>
		///    This method will enable and disable controls.
		/// </summary>
		/// <param name="enable"></param>
		private void EnableControls(bool enable)
		{
			this.AddGroupButton.Enabled = enable;
			this.CloseButton.Enabled = enable;
			this.AddGroupButton2.Enabled = enable;
			this.CloseButton2.Enabled = enable;
			this.ReportGroupsFormPageSizeDropDown2.Enabled = enable;
		}

		/// <summary>
		///    This method will return the report configuration group list DOs in the database for a
		///    given site.
		/// </summary>
		/// <returns></returns>
		private ReportConfigurationGroupListDO GetAllGroups(bool fetchFromDb)
		{
			ReportConfigurationGroupListDO groupListDO = null;

			// Get the report group list object from the session if requested to. If it is
			// not there, then set flag to retrieve from database.
			if (fetchFromDb == false)
			{
				groupListDO = (ReportConfigurationGroupListDO)this.Session[SessionGroupListDO];

				if (groupListDO == null)
				{
					fetchFromDb = true;
				}
			}

			if (fetchFromDb)
			{
				try
				{
					// Setup the group service request.
					var groupSR = new ReportConfigurationGroupSR
					              {
						              RequestType = ReportConfigurationGroupSR.RequestTypes.GET_ALL,
						              CurrentSiteGuid = this.Security.SiteGuid,
						              Security = this.Security
					              };

					// Send request to get all the group DOs from the database. Check for errors,
					// no errors then continue.
					try
					{
						groupListDO = FMChannelHelper.MakeCall<IReportConfigurationGroupProcessor, ReportConfigurationGroupListDO>(
																	 x =>
																	 x.GetAll(groupSR)
																);
						this.Session.Add(SessionGroupListDO, groupListDO);
					}
					catch (Exception ex)
					{
						this.HandleErrorCondition(ex.Message);
					}
				}
				catch (Exception ex)
				{
					this.HandleErrorCondition(ex.Message + " " + this.errorMsg004 + "!");
				}
			}

			return groupListDO;
		}


		/// <summary>
		///    This method is called by the grid when the user cancels the edit command.  It will set the
		///    page index.
		/// </summary>
		/// <param name="source"></param>
		/// <param name="eventArgs"></param>
		private void GridCancelCommand(object source, DataGridCommandEventArgs eventArgs)
		{
			this.EnableControls(true);
			this.GroupDataGrid.EditItemIndex = -1;
			this.LoadPageData(true);
		}

		/// <summary>
		///    This method will remove the select item from the group list and remove it from the
		///    database.
		/// </summary>
		/// <param name="source"></param>
		/// <param name="eventArgs"></param>
		private void GridDeleteCommand(object source, DataGridCommandEventArgs eventArgs)
		{
			try
			{
				// Get the list data.
				ReportConfigurationGroupListDO groupListDO = this.GetAllGroups(false);
				List<ReportConfigurationGroupDO> groupList = groupListDO.ReportGroupDOList;

				// Find the index to be deleted.
				int itemIndex = (this.GroupDataGrid.CurrentPageIndex * this.GroupDataGrid.PageSize) + eventArgs.Item.ItemIndex;

				// Ensure that the index is within bounds. If so, then remove the item from the 
				// database.
				if ((itemIndex >= 0) && (itemIndex < groupList.Count))
				{
					ReportConfigurationGroupDO groupDO = groupList[itemIndex];

					var groupSR = new ReportConfigurationGroupSR
					              {
						              RequestType = ReportConfigurationGroupSR.RequestTypes.DELETE,
						              CurrentSiteGuid = this.Security.SiteGuid,
						              Security = this.Security,
						              ReportConfigurationGroupDO = groupDO
					              };

					try
					{
						FMChannelHelper.MakeCall<IReportConfigurationGroupProcessor>(
																	 x =>
																	 x.Delete(groupSR)
																);
					}
					catch (Exception ex)
					{
						this.HandleErrorCondition(ex.Message);
					}

					// Set the page index to the first page.
					this.GroupDataGrid.CurrentPageIndex = 0;
				}

				this.EnableControls(true);
				this.ucFMMenuBar.Refresh();
				this.LoadPageData(true);
			}
			catch (Exception ex)
			{
				this.HandleErrorCondition(ex.Message + " " + this.errorMsg004 + "!");
			}
		}

		/// <summary>
		///    This method is called by the grid when the user selects the edit command. It will set the
		///    item index to be edited.
		/// </summary>
		/// <param name="source"></param>
		/// <param name="eventArgs"></param>
		private void GridEditCommand(object source, DataGridCommandEventArgs eventArgs)
		{
			this.EnableControls(false);
			this.GroupDataGrid.EditItemIndex = eventArgs.Item.ItemIndex;
			this.LoadPageData(false);
		}

		/// <summary>
		///    This method is called when the user selects another page.
		/// </summary>
		/// <param name="source"></param>
		/// <param name="eventArgs"></param>
		private void GridPageIndexCommand(object source, DataGridPageChangedEventArgs eventArgs)
		{
			this.GroupDataGrid.CurrentPageIndex = eventArgs.NewPageIndex;
			this.LoadPageData(true);
		}

		/// <summary>
		///    This method will update the database with the item that was selected in the group list.
		/// </summary>
		/// <param name="source"></param>
		/// <param name="eventArgs"></param>
		private void GridUpdateCommand(object source, DataGridCommandEventArgs eventArgs)
		{
			try
			{
				ReportConfigurationGroupListDO groupListDO = this.GetAllGroups(false);
				List<ReportConfigurationGroupDO> groupList = groupListDO.ReportGroupDOList;

				var groupGuidLabel = (Label)eventArgs.Item.FindControl("IdentityGuidLabel");
				var groupNameTextBox = (TextBox)eventArgs.Item.FindControl("GroupNameTextBox");

				int itemIndex = (this.GroupDataGrid.CurrentPageIndex * this.GroupDataGrid.PageSize)
									 + this.GroupDataGrid.EditItemIndex;

				// Ensure that the item selected for editing is within bounds.
				if ((itemIndex >= 0) && (itemIndex < groupList.Count))
				{
					ReportConfigurationGroupDO groupDO = groupList[itemIndex];

					// Make sure that we have the correct group data object by checking the group Guid value.
					// Update the record in the database if Guid matches.
					if (groupDO.ReportGroupGuid == Guid.Parse(groupGuidLabel.Text))
					{
						if (this.CheckLength(groupNameTextBox.Text))
						{
							groupDO.GroupName = groupNameTextBox.Text;

							var groupSR = new ReportConfigurationGroupSR
							              {
								              RequestType = ReportConfigurationGroupSR.RequestTypes.SAVE,
								              CurrentSiteGuid = this.Security.SiteGuid,
								              Security = this.Security,
								              ReportConfigurationGroupDO = groupDO
							              };

							try
							{
								FMChannelHelper.MakeCall<IReportConfigurationGroupProcessor>(
																	 x =>
																	 x.Save(groupSR)
																);
							}
							catch (Exception ex)
							{
								this.HandleErrorCondition(ex.Message);
							}
						}
						else
						{
							string message = this.errorMsg003 + "!";
							this.RenderErrorMessage(message);
						}
					}
				}

				this.GroupDataGrid.EditItemIndex = -1;
				this.EnableControls(true);
				this.ucFMMenuBar.Refresh();
				this.LoadPageData(true);
			}
			catch (Exception ex)
			{
				this.HandleErrorCondition(ex.Message + " " + this.errorMsg004 + "!");
			}
		}

		private void GroupDataGridItemDataBound(object sender, DataGridItemEventArgs e)
		{
			var deleteButton = (LinkButton)e.Item.FindControl("DeleteButton");
			var editButton = (LinkButton)e.Item.FindControl("EditButton");

			if (deleteButton != null && editButton != null)
			{
				TableCell siteGuidCell = e.Item.Cells[4];//bds

				if (!this.Security.HasRight(RIGHT.MODIFY_REPORTS) || this.Security.SiteGuid != Guid.Parse(siteGuidCell.Text))
				{
					deleteButton.Enabled = false;
					editButton.Enabled = false;
				}
			}
		}

		/// <summary>
		///    This method will check to see if there is an error, if so, then it will display an
		///    error dialog.
		/// </summary>
		/// <param name="errMsg"></param>
		private void HandleErrorCondition(string errMsg)
		{
			if (string.IsNullOrEmpty(errMsg) == false)
			{
				errMsg = errMsg.Replace(Environment.NewLine, " ");
				this.SaveControlState();
				this.RenderErrorMessage(errMsg);
			}
		}

		/// <summary>
		///    Required method for Designer support - do not modify
		///    the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.GroupDataGrid.PageIndexChanged += this.GridPageIndexCommand;
			this.GroupDataGrid.CancelCommand += this.GridCancelCommand;
			this.GroupDataGrid.EditCommand += this.GridEditCommand;
			this.GroupDataGrid.UpdateCommand += this.GridUpdateCommand;
			this.GroupDataGrid.DeleteCommand += this.GridDeleteCommand;
			this.GroupDataGrid.ItemDataBound += this.GroupDataGridItemDataBound;
		}

		/// <summary>
		///    This method will load group data into the grid on the group configuration page.
		/// </summary>
		private void LoadPageData(bool fetchFromDb)
		{
			// Get all the groups.
			ReportConfigurationGroupListDO reportGroupListDO = this.GetAllGroups(fetchFromDb);

			// If the report group list data object exist, which it should, then bind the data the
			// assignment data grid.
			if (reportGroupListDO != null)
			{
				try
				{
					DataView view = this.BuildGridData(reportGroupListDO.ReportGroupDOList);

					this.ReportGroupsFormPageSizeDropDown2.SetPageSize(this.GroupDataGrid, view.Count);

					this.GroupDataGrid.DataSource = view;
					this.GroupDataGrid.DataBind();
				}
				catch (Exception ex)
				{
					this.HandleErrorCondition(ex.Message + " " + this.errorMsg004 + "!");
				}
			}
			else
			{
				// Display error message and transfer control to report error page.
				this.HandleErrorCondition(this.errorMsg002 + "!");
			}
		}

		#endregion
	}
}