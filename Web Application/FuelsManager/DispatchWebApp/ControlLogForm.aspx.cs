// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ControlLogForm.aspx.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the ControlLogForm type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FuelsManager.DispatchWebApp
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Data;
	using System.Web;
	using System.Web.UI;
	using System.Web.UI.WebControls;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.ServiceRequests;
	using FMBusinessObjects.UtilityObjects;
	using FMControls;

	using FuelsManager.FMWebApp;

	using global::FMWebApp;

	public partial class ControlLogForm : FMFormBase
	{
		#region Constants and Fields
		/* constants referenced in the html/menu */
		public const string ControllerLogGuidColumnName = "ControllersLogGuid";
		public const string IdentityGuidColumnName = "IdentityGuid";
		public const string SiteGuidColumnName = "SiteGuid";
		public const string EventTimeColumnName = "EventTime";
		public const string ControllerColumnName = "Controller";
		public const string MemoColumnName = "Memo";

		/* constants defined in html and used in the class*/
		private const string GuidControlID = "identityGuidLabel";
		private const string EventTimeControlID = "eventTimeTextBox";
		private const string ControllerControlID = "controllerColumnTextBox";
		private const string MemoControlID = "memoColumnTextBox";
		private const string DeleteButtonTagID = "FMDeleteLinkButton1";
		private const string EditButtonTagID = "FMEditLinkButton1";

		/* The following are referenced in the class only */
		public const string DataListSessionKey = "ControllerLogClassList";
		private const string SessionControlLogTransaction = "ControlLogTransaction";

		private bool hasModifyRight;
		private bool startDateControlWasClicked;
		#endregion constants and fields

		#region Properties
		/// <summary>
		/// Sets and returns Session[DataListSessionKey] as list of ControllerLogClass items
		/// </summary>
		private List<ControllerLogClass> MySessionDataList
		{
			get
			{
				return this.Session[DataListSessionKey] as List<ControllerLogClass>;
			}
			set
			{
				this.Session[DataListSessionKey] = value;
			}
		}

		/// <summary>
		/// Represents the transaction that we are viewing control log records for
		/// If we are viewing all records, as opposed to those for a specific transaction,
		/// this will be a new transaction object with a TransactionGuid == Guid.Empty
		/// </summary>
		public TransactionDO SessionTransaction
		{
			get
			{
				if (this.Session[SessionControlLogTransaction] is TransactionDO)
				{
					return this.Session[SessionControlLogTransaction] as TransactionDO;
				}

				return new TransactionDO { SubmittedToAccounting = false };
			}
			set
			{
				this.Session.Add(SessionControlLogTransaction, value);
			}
		}

		///<summary>
		/// Sets the TransactionId
		/// </summary>
		private string TransId
		{
			get
			{
				var transId = this.Request.QueryString["transId"];
				return transId;
			}
		}
		#endregion

		#region Page Events
		/// <summary>
		/// The Page_Init event
		/// </summary>
		/// <param name="sender">An object instance</param>
		/// <param name="e">An EventArgs instance</param>
		protected void Page_Init(object sender, EventArgs e)
		{
			this.StartDate.Calendar.SelectionChanged += this.StartDateSelectionChanged;
			this.StopDate.Calendar.SelectionChanged += this.StopDateSelectionChanged;

			if (!string.IsNullOrEmpty(this.Request.QueryString["NavigateAction"]))
			{
				this.Session["NavigateAction"] = this.Request.QueryString["NavigateAction"];
			}
		}

		/// <summary>
		/// The Start Date SelectionChanged event handler
		/// </summary>
		/// <param name="sender">An object instance</param>
		/// <param name="e">An EventArgs instance</param>
		protected void StartDateSelectionChanged(object sender, EventArgs e)
		{
			this.startDateControlWasClicked = true;
			this.ShowDeletedItemsCheckBoxOnCheckedChanged(sender, e);
		}

		/// <summary>
		/// The Stop Date SelectionChanged event handler
		/// </summary>
		/// <param name="sender">An object instance</param>
		/// <param name="e">An EventArgs instance</param>
		protected void StopDateSelectionChanged(object sender, EventArgs e)
		{
			this.startDateControlWasClicked = false;
			this.ShowDeletedItemsCheckBoxOnCheckedChanged(sender, e);
		}

		/// <summary>
		/// The Page_Load event
		/// </summary>
		/// <param name="sender">An object instance</param>
		/// <param name="e">An EventArgs instance</param>
		protected void Page_Load(object sender, EventArgs e)
		{
			try
			{
				this.GetSecurity();
				this.hasModifyRight = this.Security.HasRight(RIGHT.MODIFY_DISPATCH);

				if (!this.Page.IsPostBack)
				{
					// If the user provided a transaction ID, load the corresponding transaction so we can use it later when retrieving records
					if (!string.IsNullOrEmpty(this.TransId))
					{
						TransactionSR transSR = new TransactionSR
						                        {
							                        Security = this.Security,
							                        TransID = this.TransId,
							                        AllowCrossSiteTransactions = true
						                        };

						// We should allow cross-site transactions so the form can be viewed for transactions at the site group level.

						TransactionDO trans = FMChannelHelper.MakeCall<ITransactionProcessor, TransactionDO>(
							transactionProcessorService => transactionProcessorService.Process(transSR));

						this.StartDate.CurrentValue = trans.CreatedDate;

						this.SessionTransaction = trans;
					}
					else
					{
						this.SessionTransaction = new TransactionDO { SubmittedToAccounting = false };
					}

					this.Session["NavigateAction"] = this.Request.QueryString["navigateAction"];

					if (this.Session["ControlLogPageIndex"] != null)
					{
						this.mainDataGrid.CurrentPageIndex = (int)this.Session["ControlLogPageIndex"];
						this.Session.Remove("ControlLogPageIndex");
					}

					SiteClass site = FMChannelHelper.MakeCall<ISites, SiteClass>(
																	 x =>
																	 x.GetUsingGuid(this.Security, this.Security.SiteGuid)
																);

					DateTimeOffset siteTimeNow = TimeConverter.Now(site);

					this.StartDate.CurrentValue = siteTimeNow;
					this.StopDate.CurrentValue = siteTimeNow;

					this.EnableControls(true);
					this.RefreshGrid();
				}

				if (this.ShowDeletedItemsCheckBox.Checked)
				{
					this.mainDataGrid.Columns[8].HeaderText = "UnDelete";//bds
				}
				else
				{
					this.mainDataGrid.Columns[8].HeaderText = "Delete";//bds
				}
			}
			catch (Exception error)
			{
				this.ErrorHandler(error);
			}
		}

		/// <summary>
		/// This method handles the SelectedIndexChanged for the page size dropdownbox
		/// </summary>
		/// <param name="source">An object instance</param>
		/// <param name="e">A EventArgs instance</param>
		protected void PageSizeDropDown_SelectedIndexChanged(object source, EventArgs e)
		{
			try
			{
				this.UpdateView();
			}
			catch (Exception error)
			{
				this.ErrorHandler(error);
			}
		}

		/// <summary>
		/// Closes the form and redirects client to previous page or FuelsManager home page.
		/// If a close button click was used to navigate to this page then the FuelsManager
		/// home page will be displayed when this page is closed.  Otherwise the previous
		/// page will be displayed.
		/// </summary>
		/// <param name="sender">The sender object</param>
		/// <param name="e">The event arguments</param>
		protected void CloseButtonOnClick(object sender, EventArgs e)
		{
			try
			{
				// If the menu bar was used to navigate to this page then the URL of the previous
				// page will be stored in the PreviousMenuItemUrl property.  If an open button
				// click was used to navigate to this page then the URL of the previous page
				// will be stored in the CurrentMenuItemUrl property.  The navigate action is
				// only provided on open and close button clicks.  A null or empty navigate
				// action indicates the menu bar was used to navigate to this page.
				string navigateAction = this.Session["NavigateAction"] as string;
				string redirectPageUrl;
				if (string.IsNullOrEmpty(navigateAction))
				{
					redirectPageUrl = this.ucFMMenuBar.PreviousMenuItemUrl;
				}
				else if (navigateAction == "openClick")
				{
					redirectPageUrl = this.ucFMMenuBar.CurrentMenuItemUrl;
				}
				else
				{
					redirectPageUrl = FMMenuBar.FuelsManagerHomePageUrl;
				}

				this.Redirect(redirectPageUrl + "?navigateAction=closeClick");
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		/// <summary>
		/// his method handles the Click event of the refresh button
		/// </summary>
		/// <param name="sender">The sender object</param>
		/// <param name="e">The event arguments</param>
		protected void RefreshButtonClick(object sender, EventArgs e)
		{
			this.RefreshGrid();
		}

		/// <summary>
		/// This method handles the OnCheckedChanged event of the show deleted items checkbox
		/// </summary>
		/// <param name="sender">The sender object</param>
		/// <param name="e">The event arguments</param>
		protected void ShowDeletedItemsCheckBoxOnCheckedChanged(object sender, EventArgs e)
		{
			bool enable = !this.ShowDeletedItemsCheckBox.Checked;

			this.EnableAddButtons(enable);

			this.RefreshGrid();
		}
		#endregion Page Events

		#region Grid events
		/// <summary>
		/// This method handles the OnItemCreated event of the Datagrid
		/// </summary>
		/// <param name="sender">An object instance</param>
		/// <param name="eventArgs">An DataGridItemEventArgs instance</param>
		protected void DataGrid_OnItemCreated(object sender, DataGridItemEventArgs eventArgs)
		{
			//Set the memo textfield as the control with focus upon editing a row
			TextBox memoControl = (TextBox)eventArgs.Item.FindControl(MemoControlID);

			if (memoControl != null)
			{
				this.SetFocus(memoControl);
			}
		}

		/// <summary>
		/// This method handles the click event of the Add button
		/// </summary>
		/// <param name="sender">An object instance</param>
		/// <param name="eventArgs">An EventArgs instance</param>
		protected void AddButtonClick(object sender, EventArgs eventArgs)
		{
			try
			{
				ControllerLogClass controllerLog = new ControllerLogClass
				                                   {
					                                   SiteGuid = this.Security.SiteGuid,
					                                   Controller = this.Security.UserID
				                                   };

				List<ControllerLogClass> controllerLogList = this.MySessionDataList;
				controllerLogList.Add(controllerLog);

				this.mainDataGrid.CurrentPageIndex = (controllerLogList.Count - 1) / this.mainDataGrid.PageSize;
				this.mainDataGrid.EditItemIndex = (controllerLogList.Count - 1) % this.mainDataGrid.PageSize;

				this.EnableControls(false);
				this.UpdateView();
			}
			catch (Exception error)
			{
				this.ErrorHandler(error);
			}
		}

		/// <summary>
		/// This method handles the Edit Command event of the datagrid
		/// </summary>
		/// <param name="source">An object instance</param>
		/// <param name="eventArgs">An DataGridCommandEventArgs instance</param>
		protected void DataGridEditCommand(object source, DataGridCommandEventArgs eventArgs)
		{
			try
			{
				if (this.ShowDeletedItemsCheckBox.Checked != true)
				{
					this.EnableControls(false);
					this.mainDataGrid.EditItemIndex = eventArgs.Item.ItemIndex;
					this.RefreshGrid();
				}
			}
			catch (Exception error)
			{
				this.ErrorHandler(error);
			}
		}

		/// <summary>
		/// This method handles the Add/Update Command event of the datagrid
		/// </summary>
		/// <param name="source">An object instance</param>
		/// <param name="eventArgs">An DataGridCommandEventArgs instance</param>
		protected void DataGridUpdateCommand(object source, DataGridCommandEventArgs eventArgs)
		{
			try
			{
				ControllerLogClass controllerLog;

				if (this.FindCurrentControllerLog(eventArgs.Item, out controllerLog))
				{
					this.GetSecurity();
					TextBox eventTimeControl = (TextBox)eventArgs.Item.FindControl(EventTimeControlID);
					TextBox controllerControl = (TextBox)eventArgs.Item.FindControl(ControllerControlID);
					TextBox memoControl = (TextBox)eventArgs.Item.FindControl(MemoControlID);

					controllerLog.SiteGuid = this.Security.SiteGuid;
					controllerLog.EventTime = eventTimeControl.Text;
					controllerLog.Controller = controllerControl.Text;
					controllerLog.Memo = memoControl.Text;

					if (!string.IsNullOrEmpty(this.TransId))
					{
						Guid transactionGuid = this.SessionTransaction.TransactionGuid;

						if (controllerLog.IdentityGuid == Guid.Empty && transactionGuid != Guid.Empty)
						{
							var guidList =
							FMChannelHelper.MakeCall<IControllerLogs, List<Guid>>(
												x => x.AddControllerLogAndMapRecord(this.Security, controllerLog, transactionGuid));

							if (guidList != null && guidList.Count > 0)
							{
								controllerLog.IdentityGuid = guidList[0];
							}
						}
						else
						{
							FMChannelHelper.MakeCall<IControllerLogs>(x => x.Modify(this.Security, controllerLog));
						}
					}
					else
					{
						if (controllerLog.IdentityGuid == Guid.Empty)
						{
							var newControllerGuid = FMChannelHelper.MakeCall<IControllerLogs, Guid>(x => x.Add(this.Security, controllerLog));
							controllerLog.IdentityGuid = newControllerGuid;
						}
						else
						{
							FMChannelHelper.MakeCall<IControllerLogs>(x => x.Modify(this.Security, controllerLog));
						}
					}

					DataGrid tempDataGrid = (DataGrid)source;
					tempDataGrid.EditItemIndex = -1;

					this.EnableControls(true);
					this.RefreshGrid();
				}
			}
			catch (Exception error)
			{
				this.ErrorHandler(error);
			}
		}

		/// <summary>
		/// This method handles the Cancel Command event of the datagrid
		/// </summary>
		/// <param name="source">An object instance</param>
		/// <param name="eventArgs">An DataGridCommandEventArgs instance</param>
		protected void DataGridCancelCommand(object source, DataGridCommandEventArgs eventArgs)
		{
			try
			{
				ControllerLogClass currentControllerLog;

				if (this.FindCurrentControllerLog(eventArgs.Item, out currentControllerLog) &&
					currentControllerLog.IdentityGuid == Guid.Empty)
				{
					this.MySessionDataList.Remove(currentControllerLog);

					if (this.mainDataGrid.Items.Count == 1 && this.mainDataGrid.CurrentPageIndex > 0)
					{
						this.mainDataGrid.CurrentPageIndex--;
					}
				}

				this.mainDataGrid.EditItemIndex = -1;
				this.EnableControls(true);
				this.RefreshGrid();
			}
			catch (Exception error)
			{
				this.ErrorHandler(error);
			}
		}

		/// <summary>
		/// This method handles the Delete Command event of the datagrid
		/// </summary>
		/// <param name="source">An object instance</param>
		/// <param name="eventArgs">An DataGridCommandEventArgs instance</param>
		protected void DataGridDeleteCommand(object source, DataGridCommandEventArgs eventArgs)
		{
			try
			{
				ControllerLogClass currentControllerLog;

				if (this.FindCurrentControllerLog(eventArgs.Item, out currentControllerLog))
				{
					List<ControllerLogClass> controllerLogList = this.MySessionDataList;

					if (this.ShowDeletedItemsCheckBox.Checked != true)
					{
						if (this.mainDataGrid.EditItemIndex == eventArgs.Item.ItemIndex)
						{
							this.mainDataGrid.EditItemIndex = -1;
							this.EnableControls(true);
						}
						else if (this.mainDataGrid.EditItemIndex > eventArgs.Item.ItemIndex)
						{
							this.mainDataGrid.EditItemIndex--;
						}


						// Non empty indicates object has been committed to database
						if (currentControllerLog.IdentityGuid != Guid.Empty)
						{
							FMChannelHelper.MakeCall<IControllerLogs>(x => x.Purge(this.Security, currentControllerLog.IdentityGuid));
						}

						controllerLogList.RemoveAt(eventArgs.Item.DataSetIndex);

						if (this.mainDataGrid.Items.Count == 1
							&& this.mainDataGrid.CurrentPageIndex > 0)
						{
							this.mainDataGrid.CurrentPageIndex--;
						}
					}
					else
					{
						FMChannelHelper.MakeCall<IControllerLogs>(x => x.UnDeleteControllerLog(this.Security, currentControllerLog.IdentityGuid));
					}
					this.RefreshGrid();
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		/// <summary>
		/// This method handles the Page Index Changed event of the datagrid
		/// </summary>
		/// <param name="source">An object instance</param>
		/// <param name="eventArgs">An DataGridPageChangedEventArgs instance</param>
		protected void DataGridPageIndexChanged(object source, DataGridPageChangedEventArgs eventArgs)
		{
			try
			{
				// if we are editing do not allow a page change
				if (this.mainDataGrid.EditItemIndex > -1)
				{
					return;
				}

				this.mainDataGrid.CurrentPageIndex = eventArgs.NewPageIndex;
				this.UpdateView();
			}
			catch (Exception error)
			{
				this.ErrorHandler(error);
			}
		}

		/// <summary>
		/// This method handles the Datagrids ItemDataBound event 
		/// </summary>
		/// <param name="sender">An object instance</param>
		/// <param name="eventArgs">An DataGridItemEventArgs instance</param>
		protected void DataGridItemDataBound(object sender, DataGridItemEventArgs eventArgs)
		{
			DataView dv = this.mainDataGrid.DataSource as DataView;
			if (!dv.Table.Rows[0][0].ToString().Contains("No"))
			{
				// Need to disable the edit and delete buttons when the user does not
				// have the appropriate rights.
				if (eventArgs.Item.ItemIndex != -1)
				{
					Guid siteGuid = (Guid)((DataRowView)eventArgs.Item.DataItem).Row[SiteGuidColumnName];
					this.FindAndDisableLinkButton(eventArgs, DeleteButtonTagID, siteGuid);
					this.FindAndDisableLinkButton(eventArgs, EditButtonTagID, siteGuid);
				}
			}

			if (this.ShowDeletedItemsCheckBox.Checked)
			{
				FMEditLinkButton editButton = (FMEditLinkButton)eventArgs.Item.FindControl(EditButtonTagID);
				if (editButton != null)
				{
					editButton.Enabled = false;
				}

				FMDeleteLinkButton deleteButton = (FMDeleteLinkButton)eventArgs.Item.FindControl(DeleteButtonTagID);
				if (deleteButton != null)
				{
					//Get user confirmation if they want to undelete a controller log.
					string message = "Are you sure you want to undelete this controller log?\nPress OK to continue with undelete.";
					deleteButton.Attributes.Add("onClick", "if(disabled)return false; return confirm(" +
						HttpUtility.JavaScriptStringEncode(message, true) + ");");
				}
			}

			//Check for ItemType
			if (eventArgs.Item.ItemType == ListItemType.Item ||
				eventArgs.Item.ItemType == ListItemType.AlternatingItem)
			{
				if (!dv.Table.Rows[0][0].ToString().Contains("No"))
				{
					//Declare DateTime variable
					//Assign the relevant data to a variable
					//To display DateTime field in the format for the current site or "MM/dd/yyyy" 
					SiteClass site = FMChannelHelper.MakeCall<ISites, SiteClass>(
																	 x =>
																	 x.GetUsingGuid(this.Security, this.Security.SiteGuid)
																);

					string datePattern = (!string.IsNullOrEmpty(site.ShortDatePattern)) ? "{0:" + site.ShortDatePattern + "}"
						: "{0:MM/dd/yyyy HH:mm}";
					DateTime fieldDate = Convert.ToDateTime(DataBinder.Eval(eventArgs.Item.DataItem,
						EventTimeColumnName, datePattern));

					//To display the value in the proper cell of DataGrid
					eventArgs.Item.Cells[5].Text = fieldDate.ToString();//bds
				}
				else
				{	//remove these columns such that there's one column taking up the entire row
					eventArgs.Item.Cells.RemoveAt(8);//bds
					eventArgs.Item.Cells.RemoveAt(5);
					eventArgs.Item.Cells.RemoveAt(4);
					eventArgs.Item.Cells.RemoveAt(3);
					eventArgs.Item.Cells.RemoveAt(1);
					eventArgs.Item.Cells.RemoveAt(0);
					eventArgs.Item.Cells[0].ColumnSpan = 4;
				}
			}
			else if (eventArgs.Item.ItemType == ListItemType.Header)
			{
				//resize the columns widths to match widths when data is present.
				this.mainDataGrid.Columns[0].HeaderStyle.Width = Unit.Pixel(60);
				this.mainDataGrid.Columns[1].HeaderStyle.Width = Unit.Pixel(50);
				this.mainDataGrid.Columns[5].HeaderStyle.Width = Unit.Pixel(155);//bds
				this.mainDataGrid.Columns[6].HeaderStyle.Width = Unit.Pixel(155);//bds
				this.mainDataGrid.Columns[7].HeaderStyle.Width = Unit.Pixel(345);//bds
			}

		}
		#endregion grid events

		#region Public methods
		/// <summary>
		/// Used by the html to bind data column
		/// </summary>
		/// <param name="container">Data Grid containter/row</param>
		/// <param name="columnName">Name of the column</param>
		/// <returns>The column value</returns>
		public object BindColumn(object container, string columnName)
		{
			return DataBinder.Eval(container, "DataItem." + columnName);
		}
		#endregion public methods

		#region Private Methods

		/// <summary>
		/// Enables/Disables Add buttons based on rights, Enable/Disable Show ... dropdown
		/// </summary>
		/// <param name="toEnable">Enable or Disable the controls</param>
		private void EnableControls(bool toEnable)
		{
			this.EnableAddButtons(toEnable);
			this.pageSizeDropDown.Enabled = toEnable;	
			this.ShowDeletedItemsCheckBox.Enabled = toEnable;
			this.StartDate.Enabled = toEnable;
			this.StopDate.Enabled = toEnable;
		}

		/// <summary>
		/// Enable or disable the add buttons. Whether the add buttons are enabled also depends on the user having the right to modify records.
		/// Additionally, if we are viewing control log records for a particular transaction, your site must own the transaction you 
		/// are viewing records for
		/// </summary>
		/// <param name="enable">Whether to enable or disable the add buttons</param>
		private void EnableAddButtons(bool enable)
		{
			bool siteOwnsTransaction = (this.SessionTransaction.SiteGuid == Guid.Empty || this.Security.SiteGuid == this.SessionTransaction.SiteGuid);

			bool actualEnable = enable && this.hasModifyRight && siteOwnsTransaction;
			this.topAddButton.Enabled = actualEnable;
			this.bottomAddButton.Enabled = actualEnable;
		}

		/// <summary>
		/// No parameter wrapper to the UpdateView method
		/// </summary>
		private void UpdateView()
		{
			this.UpdateView(this.pageSizeDropDown);
		}

		/// <summary>
		/// Updates the grid
		/// </summary>
		/// <param name="localPageSizeDropDown">Page size control</param>
		private void UpdateView(FMPageSizeDropDown localPageSizeDropDown)
		{
			ICollection applicationStrings = this.EnumerateData();

			if (localPageSizeDropDown != null)
			{
				localPageSizeDropDown.SetPageSize(this.mainDataGrid, applicationStrings.Count);
			}

			this.mainDataGrid.DataSource = applicationStrings;
			this.mainDataGrid.DataBind();
		}

		/// <summary>
		/// Prepare datasource for the data grid
		/// </summary>
		/// <returns>Returns a list of controller logs</returns>
		private ICollection EnumerateData()
		{
			DataView retVal;
			List<ControllerLogClass> dataList = this.MySessionDataList;

			DataTable mapDataTable = new DataTable();
			DataColumnCollection dataColumnList = mapDataTable.Columns;
			dataColumnList.Add(SiteGuidColumnName, typeof(Guid));
			dataColumnList.Add(IdentityGuidColumnName, typeof(Guid));
			dataColumnList.Add(EventTimeColumnName, typeof(string));
			dataColumnList.Add(ControllerColumnName, typeof(string));
			dataColumnList.Add(MemoColumnName, typeof(string));

			if (dataList.Count > 0)
			{
				for (int iItem = 0; iItem < dataList.Count; iItem++)
				{
					DataRow mapDataRow = mapDataTable.NewRow();
					ControllerLogClass controllerLog = dataList[iItem];

					mapDataRow[SiteGuidColumnName] = controllerLog.SiteGuid;
					mapDataRow[IdentityGuidColumnName] = controllerLog.IdentityGuid;
					mapDataRow[EventTimeColumnName] = controllerLog.EventTime;
					mapDataRow[ControllerColumnName] = controllerLog.Controller;
					mapDataRow[MemoColumnName] = controllerLog.Memo;

					mapDataTable.Rows.Add(mapDataRow);
				}
			}
			else
			{
				dataColumnList.Clear();

				dataColumnList.Add(SiteGuidColumnName, typeof(string));
				dataColumnList.Add(IdentityGuidColumnName, typeof(string));
				dataColumnList.Add(EventTimeColumnName, typeof(string));
				dataColumnList.Add(ControllerColumnName, typeof(string));
				dataColumnList.Add(MemoColumnName, typeof(string));

				DataRow mapDataRow = mapDataTable.NewRow();
				mapDataRow[SiteGuidColumnName] = "No records in date range.";
				mapDataTable.Rows.Add(mapDataRow);
			}
			retVal = new DataView(mapDataTable);
			return retVal;
		}

		/// <summary>
		/// Disables Edit/Delete icons based on rights
		/// </summary>
		/// <param name="eventArgs">Data Grid Event Arguments</param>
		/// <param name="targetID">Target control to be find</param>
		/// <param name="siteGuid">Enable or disable the control</param>
		private void FindAndDisableLinkButton(DataGridItemEventArgs eventArgs, string targetID, Guid siteGuid)
		{
			LinkButton targetButton = (LinkButton)eventArgs.Item.FindControl(targetID);

			if ((targetButton != null)
				  &&
				  (
					!this.hasModifyRight ||
					(siteGuid != this.Security.SiteGuid)		// if not owned by this site, you can't edit it
				))
			{
				targetButton.Enabled = false;
			}
		}

		/// <summary>
		/// Finds the current item and returns its Guid
		/// </summary>
		/// <param name="currentItem">Current Data Guid Row</param>
		/// <param name="currentItemGuid">returns the Guid of the current row</param>
		/// <returns>True if the Guid of the current item is found</returns>
		private bool FindCurrentGuid(DataGridItem currentItem, out Guid currentItemGuid)
		{
			Label guidLabel = (Label)currentItem.FindControl(GuidControlID);
			currentItemGuid = Guid.Empty;
			bool found = guidLabel != null;

			if (found)
			{
				currentItemGuid = new Guid(guidLabel.Text);
			}

			return found;
		}

		/// <summary>
		/// Finds the current item and returns the object from the session list
		/// </summary>
		/// <param name="currentItem">Current DataGrid row</param>
		/// <param name="controlLog">The corresponding controller log object</param>
		/// <returns>True if found</returns>
		private bool FindCurrentControllerLog(DataGridItem currentItem, out ControllerLogClass controlLog)
		{
			Guid currentGuid;
			controlLog = null;
			bool found = this.FindCurrentGuid(currentItem, out currentGuid);

			if (found)
			{
				controlLog = this.MySessionDataList.FindByGuid(currentGuid);
			}

			return found;
		}

		/// <summary>
		/// This method is used to refresh the grid contents without needing a transId.
		/// </summary>
		private void RefreshGrid()
		{
			List<ControllerLogClass> controllerLogList;

			//Setup query parameters
			DateTimeOffset startDt;
			DateTimeOffset endDt;

			if (string.IsNullOrEmpty(this.StartDate.Text))
			{
				startDt = DateTimeOffset.Now.Subtract(new TimeSpan(36500, 0, 0, 0));
			}
			else
			{
				startDt = this.StartDate.CurrentValue;
			}

			if (string.IsNullOrEmpty(this.StopDate.Text))
			{
				endDt = DateTimeOffset.Now.AddDays(36500.00);
			}
			else
			{
				endDt = this.StopDate.CurrentValue;
			}

			TransactionDO trans = this.SessionTransaction;

			//Determine which query to use to draw results.
			if (trans.TransactionGuid == Guid.Empty)
			{
				controllerLogList = FMChannelHelper.MakeCall<IControllerLogs, List<ControllerLogClass>>(
												x =>
												x.EnumerateByStartStopDateAndDeleted(this.Security, startDt, endDt, this.ShowDeletedItemsCheckBox.Checked)
											);
			}
			else
			{
				controllerLogList = FMChannelHelper.MakeCall<IControllerLogs, List<ControllerLogClass>>(
												x =>
												x.EnumerateByStartStopTimeAndTransId(this.Security, trans.CreatedDate, endDt, this.ShowDeletedItemsCheckBox.Checked,
																									trans.TransactionGuid)
											);
			}

			this.MySessionDataList = controllerLogList;
			this.UpdateView();

			if (this.StartDate.CurrentValue > this.StopDate.CurrentValue && this.startDateControlWasClicked)
			{
				this.StopDate.CurrentValue = this.StartDate.CurrentValue;
			}
			else if (this.StopDate.CurrentValue < this.StartDate.CurrentValue)
			{
				this.StartDate.CurrentValue = this.StopDate.CurrentValue;
			}
		}
		#endregion
	}
}
