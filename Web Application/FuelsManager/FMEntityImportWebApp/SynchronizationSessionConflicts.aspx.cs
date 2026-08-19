// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SynchronizationSessionConflicts.aspx.cs" company="Varec, Inc.">
//	Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//	Defines the SynchronizationSessionConflicts type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FuelsManager.FMEntityImportWebApp
{
	using System;
	using System.Data;
	using System.Globalization;
	using System.Web.UI.WebControls;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;

	using FMControls;

	using FMCore;

	using FuelsManager.FMWebApp;

	/// <summary>
	///	Conflicts description for SiteForm.
	/// </summary>
	public partial class SynchronizationSessionConflicts : SynchronizationSessionFormBase
	{
		#region Attributes
		private const string SESSION_STATUS_FILTER_INDEX = "SessionStatusFilterIndex";
		private const string SESSION_STATUS_IDENTITY_GUID = "SessionIdentityGuid";
		private const string SYNC_NODE_GUID = "SyncNodeGuid";
		private const string SYNC_CONFLICT_RESOLUTION_STATUS = "SyncConflictResolutionStatus"; 
		#endregion Attributes

		#region Properties
		/// <summary>
		/// Get or set the status filter selected dropdown index from Session.
		/// </summary>
		private int SessionStatusFilterIndex
		{
			get
			{
				if (this.Page.Session[SESSION_STATUS_FILTER_INDEX] != null && this.Page.Session[SESSION_STATUS_FILTER_INDEX] is int)
				{
					return (int) this.Page.Session[SESSION_STATUS_FILTER_INDEX];
				}
					
			return (0);
			}
			set
			{
				this.Page.Session.Add(SESSION_STATUS_FILTER_INDEX, value);
			}
		}
		#endregion Properties

		#region Methods and Operators
		/// <summary>
		/// Populate the fields on the screen with data
		/// </summary>
		protected override void UpdateView()
		{
			try
			{
				SyncRecordConflictCollection conflicts;

				if (this.Page.Session[SESSION_STATUS_IDENTITY_GUID] == null)
				{
					var syncNodeGuid = (Guid)this.Page.Session[SYNC_NODE_GUID];
					conflicts = FMChannelHelper.MakeCall<ISyncRecordConflicts, SyncRecordConflictCollection>(
										x => x.EnumerateUnresolved(this.Security, syncNodeGuid, null, 0));
				}
				else
				{
					var sessionIdentityGuid = (Guid)this.Page.Session[SESSION_STATUS_IDENTITY_GUID];
					
					if (this.StatusDropDownList.SelectedIndex == -1 || this.StatusDropDownList.SelectedValue == "-1")
					{
						conflicts = FMChannelHelper.MakeCall<ISyncRecordConflicts, SyncRecordConflictCollection>(
														x => x.EnumerateBySyncSessionLog(this.Security, sessionIdentityGuid, null, 0));
					}
					else
					{
						var status = (SYNCCONFLICTRESOLUTIONSTATUS)Convert.ToInt32(this.StatusDropDownList.SelectedValue);
						conflicts = FMChannelHelper.MakeCall<ISyncRecordConflicts, SyncRecordConflictCollection>(
												x => x.EnumerateByStatus(this.Security, status, sessionIdentityGuid));
					}
				}

				this.SyncSessionConflictDataGrid.DataSource = conflicts;
				this.SyncSessionConflictDataGrid.DataBind();
			}
			catch (Exception ex)
			{
				this.ErrorHandler(ex);
			}
		}
		#endregion Methods and Operators

		#region Page Events and Overrides
		protected override void OnInit(EventArgs e)
		{
			this.InitializeComponent();
			base.OnInit(e);
		}

		/// <summary>
		///	Required method for Designer support - do not modify
		///	the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.RefreshButton.Command += this.RefreshButtonCommand;
			this.SyncSessionConflictDataGrid.PageIndexChanged += this.SyncSessionConflictDataGrid_OnPageIndexChanged;
			this.SyncSessionConflictDataGrid.ItemDataBound += this.SyncSessionConflictDataGrid_ItemDataBound;
		}

		protected void Page_Load(object sender, EventArgs e)
		{
			try
			{
				this.GetSecurity();

				if (!this.Security.HasRight(RIGHT.VIEW_SYNC_CONFLICT_STATUS))
				{
					throw new Exception("Insufficient Rights");
				}

				if (!this.Page.IsPostBack)
				{
					if (this.Request.GetQueryOrFormValue("SessionGuid") != null)
					{
						this.Page.Session.Add(SESSION_STATUS_IDENTITY_GUID, Guid.Parse(this.Request.GetQueryOrFormValue("SessionGuid")));
						this.Page.Session.Remove(SYNC_NODE_GUID);
					}

				if (this.Request.GetQueryOrFormValue("SyncNodeGuid") != null)
				{
					this.Page.Session.Add(SYNC_NODE_GUID, Guid.Parse(this.Request.GetQueryOrFormValue("SyncNodeGuid")));
					this.Page.Session.Remove(SESSION_STATUS_IDENTITY_GUID);
					this.labSynchronizationSummary.Text = this.GetTranslatedText("Synchronization Unresolved Conflict / Error Summary");
					this.StatusDropDownList.Enabled = false;
					this.Page.Session.Remove(SYNC_CONFLICT_RESOLUTION_STATUS);
				}

	
				this.StatusDropDownList.Items.Add(new ListItem("{All}","-1"));
				for(SYNCCONFLICTRESOLUTIONSTATUS status = SYNCCONFLICTRESOLUTIONSTATUS.PENDING;status <= SYNCCONFLICTRESOLUTIONSTATUS.AUTORETRY;status ++)
				{
					this.StatusDropDownList.Items.Add(new ListItem(status.ToString(),((int)status).ToString(CultureInfo.InvariantCulture)));							
				}

				if (this.Page.Session[SYNC_CONFLICT_RESOLUTION_STATUS] is string)
				{
					this.StatusDropDownList.SelectedValue = this.Page.Session[SYNC_CONFLICT_RESOLUTION_STATUS] as string;
				}

					this.UpdateView();
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}
		#endregion Page Events and Overrides

		#region Control Events
		private void RefreshButtonCommand(object sender, CommandEventArgs e)
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

		/// <summary>
		/// This method is an event from the status filter dropdown being changed.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void StatusDropDownListSelectedIndexChanged(object sender, EventArgs e)
		{
			try
			{
				if (this.StatusDropDownList.SelectedIndex != -1 && this.StatusDropDownList.SelectedValue == "-1")
				{
				this.Page.Session.Remove(SYNC_CONFLICT_RESOLUTION_STATUS);
				}
				else
				{
				this.Page.Session[SYNC_CONFLICT_RESOLUTION_STATUS] = this.StatusDropDownList.SelectedValue;
				}
				this.UpdateView();
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		/// <summary>
		/// This method is an event from the row conflict status dropdown being changed.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void RowStatusDropDownListSelectedIndexChanged(object sender, EventArgs e)
		{
				// Need to filter the data grid
				//this.InitializeDataGrid();
		}
		#endregion Control Events

		#region Conflict Grid Methods
		/// <summary>
		/// This method will load the closeout grid with the data retrieve from the 
		/// database. It will contain the previous closeouts for a date range, manager
		/// and product.
		/// </summary>
		private void InitializeDataGrid()
		{
			this.SyncSessionConflictDataGrid.AllowSorting = false;
		}

		/// <summary>
		/// This event fires when a row is bound to the summary grid.
		/// We perform row-specific logic like highlighting rows that are out of tolerance
		/// and setting the error indicator
		/// </summary>
		/// <param name="sender">not used</param>
		/// <param name="e">Contains the row being bound</param>
		protected void SyncSessionConflictDataGrid_ItemDataBound(object sender, DataGridItemEventArgs e)
		{
			try
			{
				if (e.Item.ItemType == ListItemType.Item
				|| e.Item.ItemType == ListItemType.AlternatingItem)
				{
					var syncRecordConflictDO = ((SyncRecordConflictCollection)SyncSessionConflictDataGrid.DataSource)[SyncSessionConflictDataGrid.CurrentPageIndex * SyncSessionConflictDataGrid.PageSize + e.Item.ItemIndex];

					var linkButton = e.Item.FindControl("FMViewConflictLinkButton") as FMViewLinkButton;
					if (linkButton != null)
					{
						linkButton.Attributes.Add("onClick", "ViewConflict('" + syncRecordConflictDO.IdentityGuid.ToString() + "'); return false;");
					}
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}


		protected void SyncSessionConflictDataGrid_OnPageIndexChanged(object sender, DataGridPageChangedEventArgs e)
		{
			try
			{
				this.SyncSessionConflictDataGrid.CurrentPageIndex = e.NewPageIndex;
				UpdateView();
			}
			catch (Exception error)
			{
				ErrorHandler(error);
			}
		}

		#endregion Conflict Grid Methods
	}
}