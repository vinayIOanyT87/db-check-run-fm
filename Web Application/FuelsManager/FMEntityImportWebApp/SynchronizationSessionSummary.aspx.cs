// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SynchronizationSessionSummary.aspx.cs" company="Varec, Inc.">
//	Copyright (c) Varec, Inc.	All rights reserved.
// </copyright>
// <summary>
//	Defines the SynchronizationSessionSummary type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FuelsManager.FMEntityImportWebApp
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Data;
	using System.Globalization;
	using System.Web.UI.WebControls;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.Exceptions;
	using FMBusinessObjects.UtilityObjects;

	using FMControls;

	using FuelsManager.FMWebApp;

	/// <summary>
	///	Summary description for SynchronizationSessionSummaryForm.
	/// </summary>
	public partial class SynchronizationSessionSummary : SynchronizationSessionFormBase
	{
			#region Attributes

		protected SiteClass CurrentSite;
		public const string SYNC_SESSION_SUMMARY_LOG = "SyncSessionSummaryLog";
			#endregion Attributes

			#region Properties
			/// <summary>
			/// Get or set the status filter selected drop down index from Session.
			/// </summary>
			private int SessionStatusFilterIndex
			{
				get
				{
					if (this.Session["SessionStatusFilterIndex"] != null && this.Session["SessionStatusFilterIndex"] is int)
					{
							return (int)this.Session["SessionStatusFilterIndex"];
					}
					else
					{
							return 0;
					}
				}

				set
				{
					this.Session.Add("SessionStatusFilterIndex", value);
				}
			}

			/// <summary>
			/// Gets or Sets the list of Synchronization Sessions.
			/// </summary>
			private SyncSessionLogCollection SyncSessionLogLogList
			{
				get
				{
					if (this.Session[SynchronizationSessionSummary.SYNC_SESSION_SUMMARY_LOG] != null && this.Session[SynchronizationSessionSummary.SYNC_SESSION_SUMMARY_LOG] is SyncSessionLogCollection)
					{
							return (SyncSessionLogCollection)this.Session[SynchronizationSessionSummary.SYNC_SESSION_SUMMARY_LOG];
					}
					else
					{
							return null;
					}
				}

				set
				{
					this.Session.Add(SynchronizationSessionSummary.SYNC_SESSION_SUMMARY_LOG, value);
				}
			}

			#endregion Properties

			#region Methods and Operators

			/// <summary>
			///	This method will either enable or disable controls.	It is called by
			///	the individual tabs associated to the site form.
			/// </summary>
			/// <param name="enable"></param>
			protected override void EnableControls(bool enable)
			{
				base.EnableControls(enable);
			}

			/// <summary>
			/// Update all object(s) in session with any data the user has entered on the page
			/// </summary>
			public void UpdateData()
			{
				// this.PeriodicSyncSettingsPage.UpdateData();
			}

			/// <summary>
			/// Populate the fields on the screen with data
			/// </summary>
			protected override void UpdateView()
			{
				try
			{
					var beginning = this.BeginningDateTime.CurrentValue;
				var ending = this.EndingDateTime.CurrentValue;
					bool? withConflicts = this.WithConflictsCheckbox.Checked;

				if (this.NodeDropDownList.SelectedIndex != -1)
				{
							this.SyncSessionLogLogList = FMChannelHelper.MakeCall<ISyncSessionLogs, SyncSessionLogCollection>(
																											x =>
																											x.Enumerate(this.Security, Guid.Parse(this.NodeDropDownList.SelectedValue), beginning, ending, withConflicts));
					this.UnresolvedButton.Enabled = true;
				}
				else
				{
					this.UnresolvedButton.Enabled = false;
				}


					var SummaryCollection = this.EnumerateSyncSessionLog();

					this.SyncSessionSummaryDataGrid.DataSource = SummaryCollection;
					this.SyncSessoinSummaryPageSizeDropDown.SetPageSize(this.SyncSessionSummaryDataGrid, SummaryCollection.Count);
					this.SyncSessionSummaryDataGrid.DataBind();
				}
				catch (Exception ex)
				{
					this.ErrorHandler(ex);
				}
			}

			private ICollection EnumerateSyncSessionLog()
			{


				var syncSessionLogDataTable = new DataTable();

				syncSessionLogDataTable.Columns.Add("SyncSessionLogGuid", typeof(Guid));
				syncSessionLogDataTable.Columns.Add("StartDate", typeof(DateTimeOffset));
				syncSessionLogDataTable.Columns.Add("EndDate", typeof(DateTimeOffset));
				syncSessionLogDataTable.Columns.Add("SourceNodeMachineName", typeof(string));
				syncSessionLogDataTable.Columns.Add("TransferTypeID", typeof(string));
				syncSessionLogDataTable.Columns.Add("SyncSessionStatusID", typeof(string));
				syncSessionLogDataTable.Columns.Add("Conflicts", typeof(int));

				if (null != this.SyncSessionLogLogList)
				{
					foreach (SyncSessionLogDO sessionEntry in this.SyncSessionLogLogList)
					{
						if (sessionEntry.SyncTransferTypeIndex.ToString() != this.TransferTypeDropDownList.SelectedValue)
						{
							continue;
						}

					DataRow syncSessionLogDataRow = syncSessionLogDataTable.NewRow();

							syncSessionLogDataRow["SyncSessionLogGuid"] = sessionEntry.IdentityGuid;
							syncSessionLogDataRow["StartDate"] = sessionEntry.StartDate.HasValue ? TimeZoneInfo.ConvertTime(sessionEntry.StartDate.Value, CurrentSite.GetTimeZoneInfo()) : (object)DBNull.Value;
							syncSessionLogDataRow["EndDate"] = sessionEntry.EndDate.HasValue ? TimeZoneInfo.ConvertTime(sessionEntry.EndDate.Value, CurrentSite.GetTimeZoneInfo()) : (object)DBNull.Value;
							syncSessionLogDataRow["SourceNodeMachineName"] = sessionEntry.RemoteNodeMachineName;
							syncSessionLogDataRow["TransferTypeID"] = (sessionEntry.SyncTransferTypeIndex == SYNCTRANSFERTYPE.OFFLINE) ? "Offline" : "Online";
							syncSessionLogDataRow["SyncSessionStatusID"] = SyncTypes.GetSyncSessionStatusString(sessionEntry.SyncSessionStatusIndex);
						syncSessionLogDataRow["Conflicts"] = sessionEntry.Conflicts;
							
							syncSessionLogDataTable.Rows.Add(syncSessionLogDataRow);
					}
				}

				var syncSessionLogDataView = new DataView(syncSessionLogDataTable);
				return syncSessionLogDataView;
			}

			#endregion Methods and Operators

			#region Page Events and Overrides

			protected override void OnInit(EventArgs e)
			{
				//
				// CODEGEN: This call is required by the ASP.NET Web Form Designer.
				//
				InitializeComponent();
				base.OnInit(e);
			}

			/// <summary>
			///	Required method for Designer support - do not modify
			///	the contents of this method with the code editor.
			/// </summary>
			private void InitializeComponent()
			{
				this.SyncSessionSummaryDataGrid.PageIndexChanged += this.SyncSessionSummaryDataGrid_OnPageIndexChanged;
				this.SyncSessionSummaryDataGrid.ItemDataBound += this.SyncSessionSummaryDataGrid_ItemDataBound;
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

					if (!this.Security.HasRight(RIGHT.PERFORM_SYNCHRONIZATION))
					{
							throw new FMInsufficientRightsException();
					}

					if (!this.Page.IsPostBack)
					{
							if(this.Request.Params["WithConflicts"] is string
							&& this.Request.Params["WithConflicts"] as string == "true")
							{
								Session["SyncSessionLogWithConflicts"] = true;

								var syncRecordConflictCount = FMChannelHelper.MakeCall<ISyncRecordConflicts, SyncRecordConflictCountDO>(x => x.GetUnresolvedConflictsCount(this.Security, null));

								if(syncRecordConflictCount != null)
								{
									var timeConverter = new SiteTimeConverter(this.CurrentSite);

									syncRecordConflictCount.OldestDate = timeConverter.ConvertToSiteTime(syncRecordConflictCount.OldestDate);

									this.Session["SyncSessionLogDateFormat"] = this.CurrentSite.GetDateTimeFormatInfo();
									this.Session["SyncSessionLogBeginningDateTime"] = syncRecordConflictCount.OldestDate.ToString(this.CurrentSite.GetDateTimeFormatInfo());
								}
							}

							DateTimeOffset today = TimeConverter.Today(this.CurrentSite);

							DateTimeFormatInfo dateFormat = this.Session["SyncSessionLogDateFormat"] as DateTimeFormatInfo;

							string beginningDateString = this.Session["SyncSessionLogBeginningDateTime"] as string;
							DateTimeOffset beginningDate;

							if (beginningDateString != null && dateFormat != null && DateTimeOffset.TryParse(beginningDateString, dateFormat, DateTimeStyles.None, out beginningDate))
							{
								this.BeginningDateTime.Text = beginningDate.ToString(this.CurrentSite.GetDateTimeFormatInfo());
							}
							else
							{
								this.BeginningDateTime.Text = today.ToString(this.CurrentSite.GetDateTimeFormatInfo());
							}

							string endingDateString = this.Session["SyncSessionLogEndingDateTime"] as string;
							DateTimeOffset endingDate;

							if (endingDateString != null && dateFormat != null && DateTimeOffset.TryParse(endingDateString, dateFormat, DateTimeStyles.None, out endingDate))
							{
								this.EndingDateTime.Text = endingDate.ToString(this.CurrentSite.GetDateTimeFormatInfo());
							}
							else
							{
								this.EndingDateTime.Text = today.AddDays(1).ToString(this.CurrentSite.GetDateTimeFormatInfo());
							}

							this.WithConflictsCheckbox.Checked = false;
							if(Session["SyncSessionLogWithConflicts"] != null)
							{
								WithConflictsCheckbox.Checked = (bool)Session["SyncSessionLogWithConflicts"];
							}

							this.LoadSyncTransferTypes();

							this.LoadNodeList();

							this.UpdateView();
					}

					if (!this.Security.HasRight(RIGHT.PERFORM_SYNCHRONIZATION))
					{
							//this.OK.Enabled = false;
					}
				}
				catch (Exception except)
				{
					this.ErrorHandler(except);
				}
			}

			#endregion Page Events and Overrides

			#region Control Events

			/// <summary>
			/// This method is an event from the status filter dropdown being changed.
			/// </summary>
			/// <param name="sender"></param>
			/// <param name="e"></param>
			protected void NodeDropDownList_SelectedIndexChanged(object sender, System.EventArgs e)
			{
				this.UpdateView();
			}

			/// <summary>
			/// This method is an event from the transfer type filter dropdown being changed.
			/// </summary>
			/// <param name="sender"></param>
			/// <param name="e"></param>
			protected void TransferTypeDropDownList_SelectedIndexChanged(object sender, System.EventArgs e)
			{
				this.UpdateView();
			}

			protected void WithConflicts_CheckBoxChanged(object sender, System.EventArgs e)
			{
				this.Session["SyncSessionLogWithConflicts"] = this.WithConflictsCheckbox.Checked;

				this.UpdateView();
			}

			public void RefreshButton_Click(object sender, EventArgs e)
		{
			try
			{
				//verify beginning date recent than end date
				if (DateTimeOffset.Parse(this.BeginningDateTime.Text, this.CurrentSite.GetDateTimeFormatInfo())
					> DateTimeOffset.Parse(this.EndingDateTime.Text, this.CurrentSite.GetDateTimeFormatInfo()))
				{
					throw new Exception("Ending Date must be more recent than Beginning Date");
				}

				this.Session["SyncSessionLogBeginningDateTime"] = this.BeginningDateTime.Text;
				this.Session["SyncSessionLogEndingDateTime"] = this.EndingDateTime.Text;
				this.Session["SyncSessionLogDateFormat"] = this.CurrentSite.GetDateTimeFormatInfo();

				this.SyncSessionSummaryDataGrid.CurrentPageIndex = 0;

				this.UpdateView();
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
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



			#endregion Control Events

			#region DataGrid Methods and Event Handlers

			/// <summary>
			/// This event fires when a row is bound to the summary grid.
			/// We perform row-specific logic like highlighting rows that are out of tolerance
			/// and setting the error indicator
			/// </summary>
			/// <param name="sender">not used</param>
			/// <param name="e">Contains the row being bound</param>
			protected void SyncSessionSummaryDataGrid_ItemDataBound(object sender, DataGridItemEventArgs e)
			{
				try
				{
					if (e.Item.ItemType == ListItemType.Item
					|| e.Item.ItemType == ListItemType.AlternatingItem)
					{
						DataRow row = ((DataView)SyncSessionSummaryDataGrid.DataSource).Table.Rows[SyncSessionSummaryDataGrid.CurrentPageIndex * SyncSessionSummaryDataGrid.PageSize + e.Item.ItemIndex];

						var linkButton = e.Item.FindControl("FMViewConflictsLinkButton") as FMViewLinkButton;
						if (linkButton != null)
						{
							linkButton.Attributes.Add("onClick", "ViewSessionConflicts('" + ((Guid)row["SyncSessionLogGuid"]).ToString() + "'); return false;");
						}
						e.Item.Cells[2].Text = ((DateTimeOffset)row["StartDate"]).ToString(CurrentSite.GetDateTimeFormatInfo());
						e.Item.Cells[3].Text = ((DateTimeOffset)row["EndDate"]).ToString(CurrentSite.GetDateTimeFormatInfo());
					}
				}
				catch (Exception except)
				{
					this.ErrorHandler(except);
				}
			}

			protected void SyncSessionSummaryDataGrid_OnPageIndexChanged(object sender, DataGridPageChangedEventArgs e)
			{
				try
				{
					this.SyncSessionSummaryDataGrid.CurrentPageIndex = e.NewPageIndex;
					UpdateView();
				}
				catch (Exception error)
				{
					ErrorHandler(error);
				}
			}

			#endregion DataGrid Methods and Event Handlers

			#region Dropdown Controls
			/// <summary>
			///	This method will load the transfer types into the transfer type
			///	dropdown list.
			/// </summary>
			private void LoadSyncTransferTypes()
			{
				var transferTypes = new ArrayList();

				transferTypes.Add(new ListItem() { Text = "Online", Value = "ONLINE" });
				transferTypes.Add(new ListItem() { Text = "Offline", Value = "OFFLINE" });

				this.TransferTypeDropDownList.DataSource = transferTypes;
				this.TransferTypeDropDownList.DataTextField = "Text";
				this.TransferTypeDropDownList.DataValueField = "Value";
				this.TransferTypeDropDownList.DataBind();
			}

			/// <summary>
			///	This method will get a list of distinct Remote Nodes that experienced conflicts and populate them into the 
			///	node dropdown list.
			/// </summary>
			private void LoadNodeList()
			{
				var nodes = new ArrayList();

				var nodeDictionary = FMChannelHelper.MakeCall<ISyncSessionLogs, Dictionary<Guid, string>>(x => x.GetRemoteNodes(this.Security));

				var isEnterprise = FMChannelHelper.MakeCall<IHardwareKey, bool>(x => x.IsEnterpriseKey());
				if(isEnterprise)
				{
					nodes.Add(new ListItem() { Text = "{All}", Value = Guid.Empty.ToString() });
				}


				foreach (var node in nodeDictionary)
				{
					nodes.Add(new ListItem() { Text = node.Value, Value = node.Key.ToString() });
				}

				this.NodeDropDownList.DataSource = nodes;
				this.NodeDropDownList.DataTextField = "Text";
				this.NodeDropDownList.DataValueField = "Value";
				this.NodeDropDownList.DataBind();
			}

			#endregion Dropdown Controls
	}
}