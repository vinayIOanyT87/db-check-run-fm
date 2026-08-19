namespace FuelsManager.Accounting
{
	using System;
	using System.Collections;
	using System.Data;
	using System.Web.UI.WebControls;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.Interfaces;
	using FMBusinessObjects.UtilityObjects;

	using FMCore;

	//using FuelsManager.Areas.Controllers;

	public partial class AutoDistributionReasonCodesForm : AccountingWebFormView, IEntityDiscovery
	{
		#region constants and fields
		/* constants referenced in the html/menu */
		public const string PageTitle = "Automatic Distribution Reason Codes Configuration";

		public const string MenuName = "Reason Codes";
		public const string ReasonCodeColumnHeader = "Reason Code";
		public const string DescriptionColumnHeader = "Description";

		public const string ReasonCodeGuidColumnName = "IdentityGuid";
		public const string SiteGuidColumnName = "SiteGuid";
		public const string ReasonCodeColumnName = "ReasonCode";
		public const string DescriptionColumnName = "Description";

		/* constants defined in html and used in the class*/
		private const string GuidControlID = "identityGuidLabel";
		private const string ReasonCodeControlID = "reasonCodeTextBox";
		private const string DescriptionControlID = "descriptionTextBox";
		private const string DeleteButtonTagID = "DeleteButton";
		private const string EditButtonTagID = "EditButton";

		/* The following are referenced in the class only */
		private const string DataListSessionKey = "AutoDistributionReasonCodeList";
		private bool hasModifyRight;
		#endregion constants and fields

		#region Properties
		/// <summary>
		/// Sets and returns Session[AutoDistributionReasonCodeList] as AutoDistributionReasonCodeCollectionClass
		/// </summary>
		private AutoDistributionReasonCodeCollectionClass MySessionDataList
		{
			get
			{
				return this.Session[DataListSessionKey] as AutoDistributionReasonCodeCollectionClass;
			}
			set
			{
				this.Session[DataListSessionKey] = value;
			}
		}
		#endregion

		#region Page Events
		protected void Page_Load(object sender, EventArgs e)
		{
			try
			{
				System.Diagnostics.Trace.TraceInformation("AutoDistributionReasonCodesForm loading...");
				this.GetSecurity();
				this.hasModifyRight = this.Security.HasRight(RIGHT.MODIFY_AUTO_DISTRIBUTION_CONFIGURATION);

				if (!this.Page.IsPostBack)
				{
					this.EnableControls(true);
					AutoDistributionReasonCodeCollectionClass reasonCodeList = 
						FMChannelHelper.MakeCall<IAutoDistributionReasonCodes, AutoDistributionReasonCodeCollectionClass>(
						x => 
						x.Enumerate(this.Security)
					);

					this.MySessionDataList = reasonCodeList;
					this.UpdateView();
				}

			}
			catch (Exception error)
			{
				this.ErrorHandler(error);
			}
		}

		protected void PageSizeDropDownSelectedIndexChanged(object source, EventArgs e)
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
		#endregion Page Events

		#region grid events
		protected void AddButtonClick(object sender, EventArgs eventArgs)
		{
			try
			{
			    AutoDistributionReasonCodeClass reasonCode = new AutoDistributionReasonCodeClass
			                                                 {
			                                                     SiteGuid =
			                                                         this.Security.SiteGuid
			                                                 };
			    AutoDistributionReasonCodeCollectionClass resonCodeList = this.MySessionDataList;
				resonCodeList.Add(reasonCode);
				this.mainDataGrid.CurrentPageIndex = (resonCodeList.Count - 1) / this.mainDataGrid.PageSize;
				this.mainDataGrid.EditItemIndex = (resonCodeList.Count - 1) % this.mainDataGrid.PageSize;
				this.EnableControls(false);
				this.UpdateView();
			}
			catch (Exception error)
			{
				this.ErrorHandler(error);
			}
		}

		protected void DataGridEditCommand(object source, DataGridCommandEventArgs eventArgs)
		{
			try
			{
				this.EnableControls(false);
				this.mainDataGrid.EditItemIndex = eventArgs.Item.ItemIndex;
				this.UpdateView();
			}
			catch (Exception error)
			{
				this.ErrorHandler(error);
			}
		}

		protected void DataGridUpdateCommand(object source, DataGridCommandEventArgs eventArgs)
		{
			try
			{
				AutoDistributionReasonCodeClass currentReasonCode;

				if (this.FindCurrentReasonCode(eventArgs.Item, out currentReasonCode))
				{
					this.GetSecurity();
					TextBox reasonCodeControl = (TextBox)eventArgs.Item.FindControl(ReasonCodeControlID);
					TextBox descriptionControl = (TextBox)eventArgs.Item.FindControl(DescriptionControlID);
					currentReasonCode.SiteGuid = this.Security.SiteGuid;
					currentReasonCode.Code = reasonCodeControl.Text;
					currentReasonCode.Description = descriptionControl.Text;

					if (currentReasonCode.IdentityGuid == Guid.Empty)
					{
						currentReasonCode.IdentityGuid = FMChannelHelper.MakeCall<IAutoDistributionReasonCodes,Guid>(
							x => 
							x.Add(this.Security, currentReasonCode)
						);
					}
					else
					{
						FMChannelHelper.MakeCall<IAutoDistributionReasonCodes>(x => x.Modify(this.Security, currentReasonCode));
					}

					DataGrid tempDataGrid = (DataGrid)source;
					tempDataGrid.EditItemIndex = -1;
					this.EnableControls(true);
					this.UpdateView();
				}

			}
			catch (Exception error)
			{
				this.ErrorHandler(error);
			}
		}

		protected void DataGridCancelCommand(object source, DataGridCommandEventArgs eventArgs)
		{
			try
			{
				AutoDistributionReasonCodeClass currentReasonCode;

				if (this.FindCurrentReasonCode(eventArgs.Item, out currentReasonCode) &&
					currentReasonCode.IdentityGuid.IsEmpty())
				{
					this.MySessionDataList.Remove(currentReasonCode);

					if (this.mainDataGrid.Items.Count == 1 && this.mainDataGrid.CurrentPageIndex > 0)
					{
						this.mainDataGrid.CurrentPageIndex--;
					}
				}

				this.mainDataGrid.EditItemIndex = -1;
				this.EnableControls(true);
				this.UpdateView();
			}
			catch (Exception error)
			{
				this.ErrorHandler(error);
			}
		}

		protected void DataGridDeleteCommand(object source, DataGridCommandEventArgs eventArgs)
		{
			try
			{
				AutoDistributionReasonCodeClass currentReasonCode;

				if (this.FindCurrentReasonCode(eventArgs.Item, out currentReasonCode))
				{
					AutoDistributionReasonCodeCollectionClass reasonCodeList = this.MySessionDataList;

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
					if (!currentReasonCode.IdentityGuid.IsEmpty())
					{
						FMChannelHelper.MakeCall<IAutoDistributionReasonCodes>(x => x.Purge(this.Security, currentReasonCode.IdentityGuid));
					}

					reasonCodeList.RemoveAt(eventArgs.Item.DataSetIndex);

					if (this.mainDataGrid.Items.Count == 1
						&& this.mainDataGrid.CurrentPageIndex > 0)
					{
						this.mainDataGrid.CurrentPageIndex--;
					}

					this.UpdateView();
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

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

		protected void DataGridItemDataBound(object sender, DataGridItemEventArgs eventArgs)
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
		#endregion grid events

		#region public methods
		/// <summary>
		/// Used by the html to bind data column
		/// </summary>
		/// <param name="container">Data Grid containter/row</param>
		/// <param name="columnName">Name of the column</param>
		/// <returns>The column value</returns>
		public object BindColumn(object container, string columnName)
		{
			return System.Web.UI.DataBinder.Eval(container, "DataItem." + columnName);
		}

		/// <summary>
		/// returns a list of reason codes
		/// </summary>
		/// <param name="mySecurity">Security object used by FM</param>
		/// <returns>The current reason code list used</returns>
		public static AutoDistributionReasonCodeCollectionClass GetReasonCodeList(SecurityClass mySecurity)
		{
            mySecurity.ThrowIfNull("mySecurity");

			return FMChannelHelper.MakeCall<IAutoDistributionReasonCodes, AutoDistributionReasonCodeCollectionClass>(
				x =>
				x.Enumerate(mySecurity)
			);
		}
		#endregion public methods
		#region Private Methods

		/// <summary>
		/// Enables/Disables Add buttons based on rights, Enable/Disable Show ... dropdown
		/// </summary>
		/// <param name="toEnable">Enable or Disable the controls</param>
		private void EnableControls(bool toEnable)
		{
			bool actualEnable = toEnable && this.hasModifyRight;
			this.topAddButton.Enabled = actualEnable;
			this.bottomAddButton.Enabled = actualEnable;
			this.pageSizeDropDown.Enabled = toEnable;	// indenpendt of Modify Rights
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
		/// <param name="pageSizeDropDownParam">Page size control</param>
		private void UpdateView(FMControls.FMPageSizeDropDown pageSizeDropDownParam)
		{
			ICollection applicationStrings = this.EnumerateData();

		    pageSizeDropDownParam?.SetPageSize(this.mainDataGrid, applicationStrings.Count);

		    this.mainDataGrid.DataSource = applicationStrings;
			this.mainDataGrid.DataBind();
		}

		/// <summary>
		/// Prepare datasource for the data grid
		/// </summary>
		/// <returns>Returns a list of reason codes</returns>
		private ICollection EnumerateData()
		{
			AutoDistributionReasonCodeCollectionClass dataList = this.MySessionDataList;

			DataTable mapDataTable = new DataTable();
			DataColumnCollection dataColumnList = mapDataTable.Columns;
			dataColumnList.Add(ReasonCodeGuidColumnName, typeof(Guid));
			dataColumnList.Add(SiteGuidColumnName, typeof(Guid));
			dataColumnList.Add(ReasonCodeColumnName, typeof(string));
			dataColumnList.Add(DescriptionColumnName, typeof(string));
			foreach (AutoDistributionReasonCodeClass t in dataList)
			{
			    DataRow mapDataRow = mapDataTable.NewRow();
			    AutoDistributionReasonCodeClass reasonCode = t;
			    mapDataRow[ReasonCodeGuidColumnName] = reasonCode.IdentityGuid;
			    mapDataRow[SiteGuidColumnName] = reasonCode.SiteGuid;
			    mapDataRow[ReasonCodeColumnName] = reasonCode.Code;
			    mapDataRow[DescriptionColumnName] = reasonCode.Description;
			    mapDataTable.Rows.Add(mapDataRow);
			}
			DataView newDataView = new DataView(mapDataTable);
			return newDataView;
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
		/// <param name="reasonCode">The corresponding reason code object</param>
		/// <returns>True if found</returns>
		private bool FindCurrentReasonCode(DataGridItem currentItem, out AutoDistributionReasonCodeClass reasonCode)
		{
			Guid currentGuid;
			reasonCode = null;
			bool found = this.FindCurrentGuid(currentItem, out currentGuid);

			if (found)
			{
				reasonCode = this.MySessionDataList[currentGuid];
			}

			return found;
		}

		#endregion
		#region IEntityDiscovery interface
		ENTITY_TYPE IEntityDiscovery.EntityType => ENTITY_TYPE.AUTODISTRIBUTION_REASONCODE;

	    EntityToSiteMapCollectionClass IEntityDiscovery.EnumerateEntityMaps(SecurityClass securityParam, ENTITY_ASSIGNMENT_TYPE type)
		{
			AutoDistributionReasonCodeCollectionClass reasonCodeList = GetReasonCodeList(securityParam);
			EntityToSiteMapCollectionClass reasonCodeToSiteMapList = new EntityToSiteMapCollectionClass();
			foreach (AutoDistributionReasonCodeClass reasonCode in reasonCodeList)
			{
				
				if (type == ENTITY_ASSIGNMENT_TYPE.ASSIGNED)
				{
					if ((securityParam.SiteGuid == reasonCode.SiteGuid)
						|| (securityParam.LoginSiteGuid != reasonCode.SiteGuid))
					{
						continue;
					}
				}
				else
				{
					if (securityParam.SiteGuid != reasonCode.SiteGuid)
					{
						continue;
					}
				}

				reasonCodeToSiteMapList.Add(new EntityToSiteMapClass(reasonCode));
			}
			return reasonCodeToSiteMapList;
		}

		Type IEntityDiscovery.EntityEngineType => typeof(IAutoDistributionReasonCodes);

	    void IEntityDiscovery.SetSiteGuid(SecurityClass securityParam, Guid reasonCodeGuid, Guid siteGuid)
		{
			AutoDistributionReasonCodeClass reasonCode = FMChannelHelper.MakeCall<IAutoDistributionReasonCodes, AutoDistributionReasonCodeClass>(
				x =>
				x.Get(securityParam, reasonCodeGuid)
			);

			reasonCode.SiteGuid = siteGuid;

			FMChannelHelper.MakeCall<IAutoDistributionReasonCodes>(
				x =>
				x.Modify(securityParam, reasonCode)
			);
		}

		Guid IEntityDiscovery.GetIdentityGuid(SecurityClass securityParam, string reasonCode)
		{
			return FMChannelHelper.MakeCall<IAutoDistributionReasonCodes, Guid>(x => x.GetIdentityGuid(securityParam, reasonCode));
		}

		bool IEntityDiscovery.EntityAssignable => true;

	    #endregion
	}
}