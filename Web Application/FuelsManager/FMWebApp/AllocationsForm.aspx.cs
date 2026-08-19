// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AllocationsForm.aspx.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the AllocationsForm type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FuelsManager.FMWebApp
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Data;
	using System.Web.UI.WebControls;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.Constants;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.Interfaces;

	using FMCore;
	using System.Diagnostics.CodeAnalysis;

	/// <summary>
	///    Summary description for AllocationsForm.
	/// </summary>
	public partial class AllocationsForm : FMAutoSubmitFormBase, IEntityDiscovery, IMenuDiscovery
	{
      #region Explicit Interface Properties

      bool IEntityDiscovery.EntityAssignable
      {
         get
         {
            return false;
         }
      }

      Type IEntityDiscovery.EntityEngineType
      {
         get
         {
            return typeof(IAllocations);
         }
      }

      ENTITY_TYPE IEntityDiscovery.EntityType
      {
         get
         {
            return ENTITY_TYPE.ALLOCATION;
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
                if ((word2 & 0x01) != 0x01)
            {
               return null;
            }
         }
            else
            {
                // Depends Upon Load Rack Service
                if ((options & 0x8000) == 0)
                {
                    return null;
                }
            }

            if (siteGroup)
			{
				return null;
			}

			if (!security.HasRight(RIGHT.VIEW_ALLOCATIONS) && !security.HasRight(RIGHT.MODIFY_ALLOCATIONS))
			{
				return null;
			}

			var menuItems = new List<FMMenuItem>();

			var menuItem = new FMMenuItem
				{
					MenuItemType = FMMenuItemType.OPERATIONS_LOAD_RACK_ALLOCATIONS,
					RootMenuName = "Operations",
					CategoryName = "Load Rack",
					ItemName = "Allocations",
					NavigateUrl = "AllocationsForm.aspx",
					ApplyDataDictionary = ApplyDataDictionary.Apply
				};

			menuItems.Add(menuItem);

			return menuItems;
		}

		#endregion

		#region Explicit Interface Methods

		EntityToSiteMapCollectionClass IEntityDiscovery.EnumerateEntityMaps(
			SecurityClass security, ENTITY_ASSIGNMENT_TYPE type)
		{
			return null;
		}

		Guid IEntityDiscovery.GetIdentityGuid(SecurityClass security, string id)
		{
			return Guid.Empty;
		}

		void IEntityDiscovery.SetSiteGuid(SecurityClass security, Guid guid, Guid siteGuid)
		{
		}

		#endregion

		#region Methods

	    // ReSharper disable once InconsistentNaming
		protected void AllocationGroupsDropDownList_SelectedIndexChanged(object sender, EventArgs e)
		{
			try
			{
				this.Session["AllocationGroupGuid"] = Guid.Parse(this.AllocationGroupsDropDownList.SelectedItem.Value);
				this.AllocationsDataGrid.CurrentPageIndex = 0;
				this.UpdateView();
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

	    // ReSharper disable once InconsistentNaming
		protected void CompanyRoleDropDownList_SelectedIndexChanged(object sender, EventArgs e)
		{
			try
			{
				this.Session["CompanyMapType"] =
					(COMPANY_MAP_TYPE)Convert.ToInt32(this.CompanyMapTypeDropDownList.SelectedItem.Value);
				this.AllocationsDataGrid.CurrentPageIndex = 0;
				this.UpdateView();
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

	    // ReSharper disable once InconsistentNaming
		protected void CompanyTextBox_TextChanged(object sender, EventArgs e)
		{
			try
			{
				this.Session["AllocationsCompany"] = this.CompanyTextBox.Text;
				this.AllocationsDataGrid.CurrentPageIndex = 0;

				if (this.CompanyTextBox.Text == this.GetTranslatedText("{All}"))
				{
					this.AllocationGroupsDropDownList.Enabled = true;
					this.CompanyMapTypeDropDownList.Enabled = true;
				}
				else
				{
					this.AllocationGroupsDropDownList.Enabled = false;
					this.CompanyMapTypeDropDownList.Enabled = false;
				}

				this.UpdateView();
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
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

	    // ReSharper disable once InconsistentNaming
		protected void PageSizeDropDown_SelectedIndexChanged(object source, EventArgs e)
		{
			this.UpdateView();
		}

		protected void Page_Load(object sender, EventArgs e)
		{
			try
			{
				this.GetSecurity();

				if (!this.Page.IsPostBack)
				{
					if (!this.Security.HasRight(RIGHT.MODIFY_ALLOCATIONS))
					{
						this.AddButton.Enabled = false;
						this.AddButton2.Enabled = false;
					}

					// CompanyMapTypeDropDownList
					COMPANY_MAP_TYPE[] types =
						{
							COMPANY_MAP_TYPE.LOAD_OWNER_MANAGER_MAP, COMPANY_MAP_TYPE.SHIPPER_OWNER_MAP,
							COMPANY_MAP_TYPE.BILLTO_SHIPPER_MAP, COMPANY_MAP_TYPE.SHIPTO_BILLTO_MAP
						};

					foreach (COMPANY_MAP_TYPE type in types)
					{
						var newTypeItem = new ListItem(CompanyMapClass.TypeID(type), ((int)type).ToString());

						this.CompanyMapTypeDropDownList.Items.Add(newTypeItem);
						if ((this.Session["CompanyMapType"] != null && (COMPANY_MAP_TYPE)this.Session["CompanyMapType"] == type)
							 || (this.Session["CompanyMapType"] == null && COMPANY_MAP_TYPE.SHIPTO_BILLTO_MAP == type))
						{
							this.CompanyMapTypeDropDownList.SelectedIndex = this.CompanyMapTypeDropDownList.Items.Count - 1;
						}
					}

					this.Session["CompanyMapType"] =
						(COMPANY_MAP_TYPE)Convert.ToInt32(this.CompanyMapTypeDropDownList.SelectedItem.Value);

					// AllocationGroupDropDownList
					ApplicationStringCollectionClass allocationGroups =
						FMChannelHelper.MakeCall<IApplicationStrings, ApplicationStringCollectionClass>(
							x => x.EnumerateByType(this.Security, STRING_TYPE.ALLOCATION_GROUP));

                    var newAllocationGroupItem = new ListItem();
                    // ReSharper disable once ForCanBeConvertedToForeach
                    for (int iItem = 0; iItem < allocationGroups.Count; iItem++)
					{
						ApplicationStringClass allocationGroup = allocationGroups[iItem];

						newAllocationGroupItem = new ListItem(allocationGroup.ID, allocationGroup.IdentityGuid.ToString());

						foreach (ListItem existingAllocationGroupItem in this.AllocationGroupsDropDownList.Items)
						{
							if (string.Compare(existingAllocationGroupItem.Text, newAllocationGroupItem.Text, StringComparison.Ordinal) > 0)
							{
								int index = this.AllocationGroupsDropDownList.Items.IndexOf(existingAllocationGroupItem);
								this.AllocationGroupsDropDownList.Items.Insert(index, newAllocationGroupItem);
								if (this.Session["AllocationGroupGuid"] != null
									 && (Guid)this.Session["AllocationGroupGuid"] == allocationGroup.IdentityGuid)
								{
									this.AllocationGroupsDropDownList.SelectedIndex = index;
								}
								newAllocationGroupItem = null;
								break;
							}
						}

						if (newAllocationGroupItem != null)
						{
							this.AllocationGroupsDropDownList.Items.Add(newAllocationGroupItem);
							if (this.Session["AllocationGroupGuid"] != null
								 && (Guid)this.Session["AllocationGroupGuid"] == allocationGroup.IdentityGuid)
							{
								this.AllocationGroupsDropDownList.SelectedIndex = this.AllocationGroupsDropDownList.Items.Count - 1;
							}
						}
					}

                    newAllocationGroupItem = new ListItem(this.GetTranslatedText("{All}"), Guids.AllFilterGuid.ToString());
                    this.AllocationGroupsDropDownList.Items.Insert(0, newAllocationGroupItem);

                    newAllocationGroupItem = new ListItem(this.GetTranslatedText("{None}"), Guid.Empty.ToString());
                    this.AllocationGroupsDropDownList.Items.Insert(1, newAllocationGroupItem);

                    if (this.AllocationGroupsDropDownList.SelectedIndex != -1)
					{
						this.Session["AllocationGroupGuid"] = Guid.Parse(this.AllocationGroupsDropDownList.SelectedValue);
					}

					if (this.Session["AllocationsPage"] != null)
					{
						this.AllocationsDataGrid.CurrentPageIndex = (int)this.Session["AllocationsPage"];
						this.Session.Remove("AllocationsPage");
					}

					if (this.Session["AllocationsCompany"] == null)
					{
						this.Session["AllocationsCompany"] = this.GetTranslatedText("{All}");
					}

					this.CompanyTextBox.Text = this.Session["AllocationsCompany"] as string;

					if (this.CompanyTextBox.Text == this.GetTranslatedText("{All}"))
					{
						this.AllocationGroupsDropDownList.Enabled = true;
						this.CompanyMapTypeDropDownList.Enabled = true;
					}
					else
					{
						this.AllocationGroupsDropDownList.Enabled = false;
						this.CompanyMapTypeDropDownList.Enabled = false;
					}

					this.UpdateView();

					var param = this.Request.GetQueryOrFormValue( "Error" );
					if ( string.IsNullOrEmpty( param ) == false && param.Equals( "NoHierarchies" ) )
					{
						throw new Exception( "No available Company Hierarchies" );
					}
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		private void AddAssociatedAllocations(CompanyMapClass companyMap, AllocationCollectionClass allocationCollection)
		{
			AllocationCollectionClass associatedAllocationCollection = FMChannelHelper.MakeCall<IAllocations, AllocationCollectionClass>
																		(x => x.EnumerateByCompanyMapGuid(this.Security, companyMap.IdentityGuid, companyMap.Type));

			// Get the user group allocation status.
			var userGroupAllocationStatus = FMChannelHelper.MakeCall<IAllocations, AllocationClass.UserAllocationStatus>
													(x => x.UserHasAllocationRightsAndCompanyMapCollection(this.Security));

			// Get the groups associated to the user.
			List<GroupClass> userGroupList = FMChannelHelper.MakeCall<IAllocations, List<GroupClass>>(x => x.GetUserGroups(this.Security));

			foreach (AllocationClass associatedAllocation in associatedAllocationCollection)
			{
				bool found = false;

				if(userGroupAllocationStatus == AllocationClass.UserAllocationStatus.DoesNotHaveAllocationRights)
                {
					continue;
                }

				if (userGroupAllocationStatus == AllocationClass.UserAllocationStatus.HasGroupMappingToSome)
				{
					CompanyMapClass associatedCompanyMap = FMChannelHelper.MakeCall<ICompanyMaps, CompanyMapClass>
															(x => x.Get(this.Security, associatedAllocation.CompanyMapGuid, associatedAllocation.CompanyMapType));

					bool canViewAlloc = FMChannelHelper.MakeCall<IAllocations, bool>
											(x => x.CanViewAllocation(this.Security, associatedCompanyMap, associatedAllocation.CompanyMapType, userGroupList));

					if (canViewAlloc == false)
					{
						continue;
					}
				}

				foreach (AllocationClass existingAllocation in allocationCollection)
				{
					if (associatedAllocation.IdentityGuid == existingAllocation.IdentityGuid)
					{
						found = true;
						break;
					}
				}

				if (found == false)
				{
					allocationCollection.Add(associatedAllocation);
				}
			}

			COMPANY_MAP_TYPE type;
			switch (companyMap.Type)
			{
				case COMPANY_MAP_TYPE.SHIPTO_BILLTO_MAP:
					type = COMPANY_MAP_TYPE.BILLTO_SHIPPER_MAP;
					break;

				case COMPANY_MAP_TYPE.BILLTO_SHIPPER_MAP:
					type = COMPANY_MAP_TYPE.SHIPPER_OWNER_MAP;
					break;

				case COMPANY_MAP_TYPE.SHIPPER_OWNER_MAP:
					type = COMPANY_MAP_TYPE.LOAD_OWNER_MANAGER_MAP;
					break;

				default:
					return;
			}

			CompanyMapClass assignedToCompanyMap = FMChannelHelper.MakeCall<ICompanyMaps, CompanyMapClass>
															(x => x.Get(this.Security, companyMap.AssignedToGuid, type));


			this.AddAssociatedAllocations(assignedToCompanyMap, allocationCollection);
		}

	    // ReSharper disable once InconsistentNaming
		private void AddButton_Command(object sender, CommandEventArgs e)
		{
			this.Session.Remove("IdentityGuid");
			this.Session["AllocationsPage"] = this.AllocationsDataGrid.CurrentPageIndex;
			this.Redirect("AllocationForm.aspx");
		}

	    // ReSharper disable once InconsistentNaming
		private void AllocationsDataGrid_DeleteCommand(object source, DataGridCommandEventArgs e)
		{
			try
			{
				// Get IdentityGuid
				TableCell identityGuidCell = e.Item.Cells[2];//bds

				FMChannelHelper.MakeCall<IAllocations>(x => x.Purge(this.Security, Guid.Parse(identityGuidCell.Text)));

				this.AllocationsDataGrid.SelectedIndex = -1;
				this.Session.Remove("IdentityGuid");
				if (this.AllocationsDataGrid.Items.Count == 1 && this.AllocationsDataGrid.CurrentPageIndex > 0)
				{
					this.AllocationsDataGrid.CurrentPageIndex--;
				}
				this.UpdateView();
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

	    // ReSharper disable once InconsistentNaming
		private void AllocationsDataGrid_EditCommand(object source, DataGridCommandEventArgs e)
		{
			this.Session.Remove("Product");
			TableCell identityGuidCell = e.Item.Cells[2];//bds
			this.Session["IdentityGuid"] = identityGuidCell.Text;
			this.Session["AllocationsPage"] = this.AllocationsDataGrid.CurrentPageIndex;
			this.Redirect("AllocationForm.aspx");
		}

	    // ReSharper disable once InconsistentNaming
		private void AllocationsDataGrid_ItemDataBound(object sender, DataGridItemEventArgs e)
		{
			var deleteButton = (LinkButton)e.Item.FindControl("DeleteButton");
			if (deleteButton != null)
			{
				deleteButton.Enabled = this.Security.HasRight(RIGHT.MODIFY_ALLOCATIONS);
			}
		}

	    // ReSharper disable once InconsistentNaming
		private void AllocationsDataGrid_PageIndexChanged(object source, DataGridPageChangedEventArgs e)
		{
			try
			{
				// if we are editing do not allow a page change
				if (this.AllocationsDataGrid.EditItemIndex > -1)
				{
					return;
				}
				this.AllocationsDataGrid.CurrentPageIndex = e.NewPageIndex;
				this.UpdateView();
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

      private ICollection EnumerateAllocations()
		{

		    // ReSharper disable once InconsistentNaming
			var AllocationCollection = new AllocationCollectionClass();

			if (this.Session["AllocationsCompany"] as string != this.GetTranslatedText("{All}"))
			{
				Guid companyMasterGuid =
					FMChannelHelper.MakeCall<ICompanies, Guid>(
						x => x.GetMasterRecordGuid(this.Security, this.Session["AllocationsCompany"] as string));

				if (companyMasterGuid != Guid.Empty)
				{
				    // ReSharper disable once InconsistentNaming
					CompanyRoleMapCollectionClass RoleMapCollection =
						FMChannelHelper.MakeCall<ICompanyRoleMaps, CompanyRoleMapCollectionClass>(
							x => x.EnumerateByCompany(this.Security, companyMasterGuid));

				    // ReSharper disable once InconsistentNaming
					foreach (CompanyRoleMapClass RoleMap in RoleMapCollection)
					{
						COMPANY_MAP_TYPE type;
						switch (RoleMap.Role)
						{
							case COMPANY_ROLE.CUSTOMER_SHIPTO:
								type = COMPANY_MAP_TYPE.SHIPTO_BILLTO_MAP;
								break;

							case COMPANY_ROLE.CUSTOMER_BILLTO:
								type = COMPANY_MAP_TYPE.BILLTO_SHIPPER_MAP;
								break;

							case COMPANY_ROLE.SHIPPER:
								type = COMPANY_MAP_TYPE.SHIPPER_OWNER_MAP;
								break;

							case COMPANY_ROLE.OWNER:
								type = COMPANY_MAP_TYPE.LOAD_OWNER_MANAGER_MAP;
								break;

							case COMPANY_ROLE.CARRIER:
							case COMPANY_ROLE.SUPPLIER:
							case COMPANY_ROLE.MANAGER:
							default:
								continue;
						}

						CompanyMapCollectionClass companyMapCollection = FMChannelHelper.MakeCall<ICompanyMaps, CompanyMapCollectionClass>(
																	 x =>
																	 x.EnumerateByAssignedGuidAndType(this.Security, companyMasterGuid, type)
																);

						foreach (CompanyMapClass companyMap in companyMapCollection)
						{
							this.AddAssociatedAllocations(companyMap, AllocationCollection);
						}
					}
				}
			}
			else
			{
				var type = (COMPANY_MAP_TYPE)Convert.ToInt32(this.CompanyMapTypeDropDownList.SelectedValue);
				AllocationCollection = FMChannelHelper.MakeCall<IAllocations, AllocationCollectionClass>(
																	 x =>
																	 x.EnumerateByCompanyMapType(this.Security, type)
																);

			}

			var allocationDataTable = new DataTable();

	        allocationDataTable.Columns.Add("SiteGuid", typeof(Guid));
			allocationDataTable.Columns.Add("IdentityGuid", typeof(Guid));
			allocationDataTable.Columns.Add("CompanyMapAssignedID", typeof(string));
			allocationDataTable.Columns.Add("CompanyMapAssignedToID", typeof(string));
			allocationDataTable.Columns.Add("EffectiveDate", typeof(string));
			allocationDataTable.Columns.Add("ExpirationDate", typeof(string));
			allocationDataTable.Columns.Add("AllocationGroupID", typeof(string));

			Guid allocationGroupGuid = Guids.AllFilterGuid; //was -1

			if (this.AllocationGroupsDropDownList.Enabled && this.Session["AllocationGroupGuid"] != null)
			{
				allocationGroupGuid = (Guid)this.Session["AllocationGroupGuid"];
			}

			foreach (AllocationClass allocation in AllocationCollection)
			{
				if (allocationGroupGuid != Guids.AllFilterGuid && allocationGroupGuid != allocation.AllocationGroupGuid)
				{
					continue;
				}

				var allocationDataRow = allocationDataTable.NewRow();

				int indexOfLastDelimeter = allocation.ID.LastIndexOfAny(new[] { '>' });

				allocationDataRow["SiteGuid"] = allocation.SiteGuid;
				allocationDataRow["IdentityGuid"] = allocation.IdentityGuid;
				allocationDataRow["CompanyMapAssignedID"] = allocation.ID.Substring(indexOfLastDelimeter + 1);
				allocationDataRow["CompanyMapAssignedToID"] = allocation.ID.Substring(0, indexOfLastDelimeter - 1);
				allocationDataRow["EffectiveDate"] = allocation.EffectiveDate;
				allocationDataRow["ExpirationDate"] = allocation.ExpirationDate;
				allocationDataRow["AllocationGroupID"] = allocation.AllocationGroupID;

				allocationDataTable.Rows.Add(allocationDataRow);
			}

			var allocationDataView = new DataView(allocationDataTable);
			return allocationDataView;
		}

		/// <summary>
		///    Required method for Designer support - do not modify
		///    the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.AddButton2.Command += this.AddButton_Command;
			this.AllocationsDataGrid.EditCommand += this.AllocationsDataGrid_EditCommand;
			this.AllocationsDataGrid.PageIndexChanged += this.AllocationsDataGrid_PageIndexChanged;
			this.AllocationsDataGrid.DeleteCommand += this.AllocationsDataGrid_DeleteCommand;
			this.AllocationsDataGrid.ItemDataBound += this.AllocationsDataGrid_ItemDataBound;
			this.AddButton.Command += this.AddButton_Command;
		}

		private void UpdateView()
		{
			ICollection allocations = this.EnumerateAllocations();

			this.AllocationsFormPageSizeDropDown.SetPageSize(this.AllocationsDataGrid, allocations.Count);

			this.AllocationsDataGrid.DataSource = allocations;
			this.AllocationsDataGrid.DataBind();
		}

		#endregion
	}
}