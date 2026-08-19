// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CompanyRoleAssignmentForm.aspx.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the CompanyRoleAssignmentForm.aspx.cs type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FuelsManager.FMWebApp
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Drawing;
    using System.Globalization;
    using System.Web.UI.WebControls;

    using FMBusinessObjects.BusinessInterfaces;
    using FMBusinessObjects.ChannelFactories;
    using FMBusinessObjects.Constants;
    using FMBusinessObjects.DataObjects;

    using FMControls;

    public partial class CompanyRoleAssignmentForm : FMFormBase, IMenuDiscovery
	{
		#region Enums

		private enum CheckedAllStates { Checked, Unchecked }

		private bool hasModifyRight;

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
                if ((options & 0x80100) == 0)
                {
                    return null;
                }
            }

            if ((security.HasRight(RIGHT.VIEW_COMPANY_DATA) == false) && (security.HasRight(RIGHT.MODIFY_COMPANY_DATA) == false))
			{
				return null;
			}

			var menuItems = new List<FMMenuItem>();

			var menuItem = new FMMenuItem
				{
					MenuItemType = FMMenuItemType.ACCOUNTING_COMPANIES_COMPANY_ROLE_ASSIGNMENTS,
					RootMenuName = "Accounting",
					CategoryName = "Companies",
					ItemName = "Company Role Assignments",
					NavigateUrl = "CompanyRoleAssignmentForm.aspx",
					ApplyDataDictionary = FMWebApp.ApplyDataDictionary.Apply,
					SortOrder = 3
				};

			menuItems.Add(menuItem);

			return menuItems;
		}

		#endregion

		#region Methods

		/// <summary>
		///    This method will handle the bottom Apply button event. It will save all the changes to the
		///    database.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void ApplyBottomBtn_OnClick(object sender, EventArgs e)
		{
			this.ApplyBtn_Onclick(sender, e);
		}

		/// <summary>
		///    This method will handle the Apply button event. It will save all the changes to the
		///    database.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void ApplyBtn_Onclick(object sender, EventArgs e)
		{
			bool addingOwner = false;
			bool loginSiteIsSingleOwner = false;
			this.PersistFilters();

			DataGridItemCollection gridItems = this.CompanyRolesGrid.Items;

			SiteClass site = FMChannelHelper.MakeCall<ISites, SiteClass>(sites => sites.GetBasic(this.Security, this.Security.SiteGuid));

			if (site != null)
			{
				// set the single owner flag here so we do not have to interogate the database multiple times
				loginSiteIsSingleOwner = site.EnforceSingleOwner;
			}

			if (gridItems.Count > 0)
			{
				FMChannelHelper.MakeCall<ICompanyRoleMaps>(
					companyRoleMaps =>
						{
							foreach (DataGridItem item in gridItems)
							{
								addingOwner = this.UpdateValues(companyRoleMaps, item, addingOwner, loginSiteIsSingleOwner);
							}
						});
			}

			this.UpdateView();
		}

		private bool UpdateValues(ICompanyRoleMaps companyRoleMaps, DataGridItem item, bool addingOwner, bool loginSiteIsSingleOwner)
		{
			try
			{
				var checkBox = (FMCheckBox)item.FindControl("ManagerCheckbox");
				if (this.IsFirstRow(checkBox))
				{
					this.HandleAllRoleAssignment(checkBox, COMPANY_ROLE.MANAGER);
				}
				else
				{
					this.HandleRoleAssignmentUpdate( companyRoleMaps, checkBox, COMPANY_ROLE.MANAGER );
				}

				addingOwner = true;
				checkBox = (FMCheckBox)item.FindControl("OwnerCheckbox");
				if (this.IsFirstRow(checkBox))
				{
					this.HandleAllRoleAssignment(checkBox, COMPANY_ROLE.OWNER);
				}
				else
				{
					this.HandleRoleAssignmentUpdate( companyRoleMaps, checkBox, COMPANY_ROLE.OWNER );
				}

				addingOwner = false;

				checkBox = (FMCheckBox)item.FindControl("CarrierCheckbox");
				if (this.IsFirstRow(checkBox))
				{
					this.HandleAllRoleAssignment(checkBox, COMPANY_ROLE.CARRIER);
				}
				else
				{
					this.HandleRoleAssignmentUpdate( companyRoleMaps, checkBox, COMPANY_ROLE.CARRIER );
				}

				checkBox = (FMCheckBox)item.FindControl("ShipToCheckbox");
				if (this.IsFirstRow(checkBox))
				{
					this.HandleAllRoleAssignment(checkBox, COMPANY_ROLE.CUSTOMER_SHIPTO);
				}
				else
				{
					this.HandleRoleAssignmentUpdate( companyRoleMaps, checkBox, COMPANY_ROLE.CUSTOMER_SHIPTO );
				}

				checkBox = (FMCheckBox)item.FindControl("BillToCheckbox");
				if (this.IsFirstRow(checkBox))
				{
					this.HandleAllRoleAssignment(checkBox, COMPANY_ROLE.CUSTOMER_BILLTO);
				}
				else
				{
					this.HandleRoleAssignmentUpdate( companyRoleMaps, checkBox, COMPANY_ROLE.CUSTOMER_BILLTO );
				}

				checkBox = (FMCheckBox)item.FindControl("ShipperCheckbox");
				if (this.IsFirstRow(checkBox))
				{
					this.HandleAllRoleAssignment(checkBox, COMPANY_ROLE.SHIPPER);
				}
				else
				{
					this.HandleRoleAssignmentUpdate( companyRoleMaps, checkBox, COMPANY_ROLE.SHIPPER );
				}

				checkBox = (FMCheckBox)item.FindControl("SupplierCheckbox");
				if (this.IsFirstRow(checkBox))
				{
					this.HandleAllRoleAssignment(checkBox, COMPANY_ROLE.SUPPLIER);
				}
				else
				{
					this.HandleRoleAssignmentUpdate( companyRoleMaps, checkBox, COMPANY_ROLE.SUPPLIER );
				}
			}
			catch (Exception except)
			{
				if (loginSiteIsSingleOwner && addingOwner && this.GetTotalRoleCount(COMPANY_ROLE.OWNER) >= 1)
				{
					this.ErrorHandler(new Exception("Can Not Have Multiple Owners On Single Owner Site"));
				}
				else
				{
					this.ErrorHandler(except);
				}
			}
			return addingOwner;
		}

		/// <summary>
		///    This method will handle the page index change event.
		/// </summary>
		/// <param name="source"></param>
		/// <param name="e"></param>
		protected void CompanyRoleDataGridPageIndexChanged(object source, DataGridPageChangedEventArgs e)
		{
			try
			{
				this.PersistFilters();
				this.CompanyRolesGrid.CurrentPageIndex = e.NewPageIndex;
				this.UpdateView();
			}
			catch (Exception except)
			{
			    this.ErrorHandler(except);
			}
		}

		/// <summary>
		///    This method handles the company role selection change event. It will update
		///    the grid based on the filters.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void CompanyRoleSelectChange(object sender, EventArgs e)
		{
			this.RefreshGrid();
		}


		/// <summary>
		///    This method will handle the company role grid Item Data Bound event. It will
		///    load the value of each field into the control.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void CompanyRolesGridItemDataBound(object sender, DataGridItemEventArgs e)
		{
			try
			{
				if (e.Item.ItemIndex > -1)
				{
					// Get the correct index based on the page size, current page, and bound index.
					int nextIndex = this.GetNextRoleMapIndex(e.Item.ItemIndex);

					// Clear the second row which is the row after the Apply To All row. The row will be
					// used as a separator and it will be colored in blue.
					if (nextIndex == 1)
					{
						this.ClearSecondRow(e);
					}
					else
					{
						var companyRoleList = (List<CompanyRoleMapClass>)this.CompanyRolesGrid.DataSource;
						CompanyRoleMapClass companyRoleMap = companyRoleList[nextIndex];

						var companyID = (FMLabel)e.Item.FindControl("IDLabel");
						if (companyID != null)
						{
							companyID.Text = companyRoleMap.CompanyID;

							if (nextIndex == 0)
							{
								companyID.Font.Bold = true;
							}
						}

						var companyName = (FMLabel)e.Item.FindControl("NameLabel");
						if (companyName != null)
						{
							companyName.Text = companyRoleMap.CompanyName;
						}

						var companySite = (FMLabel)e.Item.FindControl("SiteLabel");
						if (companySite != null)
						{
							companySite.Text = companyRoleMap.SiteID;
						}

						var assigned = (FMCheckBox)e.Item.FindControl("ManagerCheckbox");
						this.SetCheckbox(assigned, companyRoleMap.HasManagerRole, COMPANY_ROLE.MANAGER, nextIndex, companyRoleList.Count);
                        assigned.Enabled = this.hasModifyRight;

						assigned = (FMCheckBox)e.Item.FindControl("OwnerCheckbox");
						this.SetCheckbox(assigned, companyRoleMap.HasOwnerRole, COMPANY_ROLE.OWNER, nextIndex, companyRoleList.Count);
                        assigned.Enabled = this.hasModifyRight;

                        assigned = (FMCheckBox)e.Item.FindControl("ShipperCheckbox");
						this.SetCheckbox(assigned, companyRoleMap.HasShipperRole, COMPANY_ROLE.SHIPPER, nextIndex, companyRoleList.Count);
                        assigned.Enabled = this.hasModifyRight;

                        assigned = (FMCheckBox)e.Item.FindControl("BillToCheckbox");
						this.SetCheckbox(assigned, companyRoleMap.HasBillToRole, COMPANY_ROLE.CUSTOMER_BILLTO, nextIndex, companyRoleList.Count);
                        assigned.Enabled = this.hasModifyRight;

                        assigned = (FMCheckBox)e.Item.FindControl("ShipToCheckbox");
						this.SetCheckbox(assigned, companyRoleMap.HasShipToRole, COMPANY_ROLE.CUSTOMER_SHIPTO, nextIndex, companyRoleList.Count);
                        assigned.Enabled = this.hasModifyRight;

                        assigned = (FMCheckBox)e.Item.FindControl("CarrierCheckbox");
						this.SetCheckbox(assigned, companyRoleMap.HasCarrierRole, COMPANY_ROLE.CARRIER, nextIndex, companyRoleList.Count);
                        assigned.Enabled = this.hasModifyRight;

                        assigned = (FMCheckBox)e.Item.FindControl("SupplierCheckbox");
						this.SetCheckbox(assigned, companyRoleMap.HasSupplierRole, COMPANY_ROLE.SUPPLIER, nextIndex, companyRoleList.Count);
                        assigned.Enabled = this.hasModifyRight;
                    }
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		/// <summary>
		///    This method will handle the sort command event. It will save the sort column in session.
		/// </summary>
		/// <param name="source"></param>
		/// <param name="e"></param>
		protected void CompanyRolesGridSortCommand(object source, DataGridSortCommandEventArgs e)
		{
			bool validSortKey = false;
			string sortExpression = e.SortExpression;

			// Can only sort on the ID, Name, or Site columns.
			if (sortExpression != null)
			{
				if (sortExpression.ToUpper().Equals("ID"))
				{
					validSortKey = true;
				}
				else if (sortExpression.ToUpper().Equals("NAME"))
				{
					validSortKey = true;
				}
				else if (sortExpression.ToUpper().Equals("SITE"))
				{
					validSortKey = true;
				}
			}

			if (validSortKey)
			{
				if (this.Page.Session[PageSessionKeyConstants.CRAF_SESSION_SORT_KEY] == null)
				{
					this.Page.Session.Add(PageSessionKeyConstants.CRAF_SESSION_SORT_KEY, sortExpression);
					this.PersistFilters();
					this.UpdateView();
				}
				else
				{
					this.Page.Session[PageSessionKeyConstants.CRAF_SESSION_SORT_KEY] = sortExpression;
					this.PersistFilters();
					this.UpdateView();
				}
			}
		}

		/// <summary>
		///    This method handles the Company Text Box on text change event. If the selection is
		///    set to "ALL", then the Site and Company Role dropdown lists are disabled.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void CompanySelectionOnTextChange(object sender, EventArgs e)
		{
			if ((this.CompanyTextBox.Text != null) && this.CompanyTextBox.Text.Equals("{All}"))
			{
				this.RoleDropDown.Enabled = true;
			}
			else
			{
				this.RoleDropDown.SelectedIndex = 0;
				this.RoleDropDown.Enabled = false;
			}

			// Refresh the data based on the filters.
			this.RefreshGrid();
		}

		/// <summary>
		///    This method handles the Find Button on click event. It will update
		///    the filters and update the view with new data.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void FindBtn_OnClick(object sender, EventArgs e)
		{
			this.PersistFilters();
			this.UpdateView();
		}

		/// <summary>
		///    This method handles the Include Member Site checkbox change event.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void IncludeMemberSiteChange(object sender, EventArgs e)
		{
			this.RefreshGrid();
		}

		/// <summary>
		///    This method handles the on initialize event from ASP.NET.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnInit(EventArgs e)
		{
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			this.InitializeComponent();
			base.OnInit(e);
		}

		protected void Page_Load(object sender, EventArgs e)
		{
			try
			{
				this.GetSecurity();

				this.hasModifyRight = this.Security.HasRight( RIGHT.MODIFY_COMPANY_DATA );

				// Disable the Apply buttons unless the user has modify company data rights. 
				this.ApplyBtnSecurityCheck();

				if (this.Page.IsPostBack == false)
				{
					this.ApplyDataDictionary();
					this.FindTextBox.Text = String.Empty;
					this.LoadSiteDropDown();
					this.LoadRoleDropDown();
					this.SetFilterFields();
					this.PersistFilters();
					this.CompanySelectionOnTextChange(null, null);
				}
			}
			catch (Exception except)
			{
			    this.ErrorHandler(except);
			}
		}

		/// <summary>
		///    This method refreshes the grid.
		/// </summary>
		protected void RefreshGrid()
		{
			this.PersistFilters();
			this.UpdateView();
		}

		/// <summary>
		///    This method handles the role grid sizing event. It will update the grid based
		///    on the requested size (show 10, 25, 50, 100, and ALL).
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void RoleSizeDropDownSelectedIndexChanged(object sender, EventArgs e)
		{
			this.PersistFilters();
			this.UpdateView();
		}

		/// <summary>
		///    This method handles the Show All Button on click event. It will update
		///    the filters and update the view with new data.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void ShowAllBtn_OnClick(object sender, EventArgs e)
		{
			this.FindTextBox.Text = String.Empty;
			this.PersistFilters();
			this.UpdateView();
		}

		/// <summary>
		///    This method handles the Site dropdown selection change. It disables the Include
		///    Member Site check box if the site selected is not a site group.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void SiteSelectionChange(object sender, EventArgs e)
		{

			try
			{
				Guid selectedSiteGuid = Guid.Parse(this.SiteDropDown.SelectedValue);


				if ( FMChannelHelper.MakeCall<ISites, Boolean>(x => x.IsSiteGroup(this.Security, selectedSiteGuid))== false)
				{
					this.IncludeMemberSitesCheckBox.Checked = false;
					this.IncludeMemberSitesCheckBox.Enabled = false;
				}
				else
				{
					this.IncludeMemberSitesCheckBox.Enabled = true;

					if (this.Page.Session[PageSessionKeyConstants.CRAF_SESSION_INCLUDE_MEMBERS] != null)
					{
						var isChecked = (bool)this.Page.Session[PageSessionKeyConstants.CRAF_SESSION_INCLUDE_MEMBERS];
						this.IncludeMemberSitesCheckBox.Checked = isChecked;
					}
				}

				// When the event is null, that means it was called on the initial page
				// load and there is no need to refresh the grid.
				if (e != null)
				{
					// Refresh the grid based on the filter settings.
					this.RefreshGrid();
				}
			}
			catch (Exception)
			{
				this.IncludeMemberSitesCheckBox.Checked = false;
				this.IncludeMemberSitesCheckBox.Enabled = false;
			}
		}

		/// <summary>
		///    This method will disable the Apply buttons if the user does not have
		///    modify company data rights.
		/// </summary>
		private void ApplyBtnSecurityCheck()
		{
			this.TopApplyButton.Enabled = false;
			this.BottomApplyButton.Enabled = false;

			if (this.Security.HasRight(RIGHT.MODIFY_COMPANY_DATA))
			{
				this.TopApplyButton.Enabled = true;
				this.BottomApplyButton.Enabled = true;
			}
		}

		/// <summary>
		///    This method will apply the data dictionary to the columns on the grid.
		/// </summary>
		private void ApplyDataDictionary()
		{
			for (int nextColumn = 0; nextColumn < 10; nextColumn++)
			{
				if ((this.CompanyRolesGrid.Columns[nextColumn] != null)
					 && (this.CompanyRolesGrid.Columns[nextColumn].HeaderText != null))
				{
					string newText = this.GetTranslatedText(this.CompanyRolesGrid.Columns[nextColumn].HeaderText);
					this.CompanyRolesGrid.Columns[nextColumn].HeaderText = newText;
				}
			}
		}


/*
		/// <summary>
		///    This method will disable the Checkboxes unless the user has modify company data
		///    rights. This method is called by the Item Data Bound event.
		/// </summary>
		/// <param name="checkBox"></param>
		private void CheckBoxSecurityCheck(FMCheckBox checkBox)
		{
			checkBox.Enabled = false;

			if (this.Security.HasRight(RIGHT.MODIFY_COMPANY_DATA))
			{
				checkBox.Enabled = true;
			}
		}
*/

		/// <summary>
		///    This method will clear the second row (use to separate the Apply To All row from the other
		///    rows. It will also fill the row in blue.
		/// </summary>
		/// <param name="e"></param>
		private void ClearSecondRow(DataGridItemEventArgs e)
		{
			string strColorHeaderBlue = ConfigurationManager.AppSettings["ColorHeaderBlue"];
			int nColorValue = Convert.ToInt32(strColorHeaderBlue);
			Color headerColor = Color.FromArgb(nColorValue);

			var controlLabelNames = new ArrayList();
			var controlCheckBoxNames = new ArrayList();

			controlLabelNames.Add("IDLabel");
			controlLabelNames.Add("NameLabel");
			controlLabelNames.Add("SiteLabel");

			controlCheckBoxNames.Add("ManagerCheckbox");
			controlCheckBoxNames.Add("OwnerCheckbox");
			controlCheckBoxNames.Add("ShipperCheckbox");
			controlCheckBoxNames.Add("BillToCheckbox");
			controlCheckBoxNames.Add("ShipToCheckbox");
			controlCheckBoxNames.Add("CarrierCheckbox");
			controlCheckBoxNames.Add("SupplierCheckbox");

			foreach (string controlName in controlLabelNames)
			{
				var labelID = (FMLabel)e.Item.FindControl(controlName);

				if (labelID != null)
				{
					labelID.Text = "A";
					var cell = labelID.Parent as TableCell;
					if (cell != null)
					{
						cell.BackColor = headerColor;
					}
					labelID.ForeColor = headerColor;
				}
			}

			foreach (string controlName in controlCheckBoxNames)
			{
				var checkboxID = (FMCheckBox)e.Item.FindControl(controlName);

				if (checkboxID != null)
				{
					checkboxID.Visible = false;
					var cell = checkboxID.Parent as TableCell;
					if (cell != null)
					{
						cell.BackColor = headerColor;
					}
				}
			}
		}

		/// <summary>
		///    This method will return the Company Guid for the selected company. It
		///    will return an empty guid if no selected company.
		/// </summary>
		/// <returns></returns>
		private Guid GetCompanyIdentityGuid()
		{
			Guid companyIdentityGuid = Guid.Empty; //was -9;
			string companySelection = this.CompanyTextBox.Text;

			if (!string.IsNullOrEmpty(companySelection))
			{
				companyIdentityGuid = FMChannelHelper.MakeCall<ICompanies, Guid>(
																	 x =>
																	 x.GetMasterRecordGuid(this.Security, companySelection)
																);
			}

			return companyIdentityGuid;
		}

		/// <summary>
		///    This method will return the next company role map index based on the item being
		///    bound, the page size, and the current page.
		/// </summary>
		/// <param name="baseIndex"></param>
		/// <returns></returns>
		private int GetNextRoleMapIndex(int baseIndex)
		{
			int nextIndex = baseIndex;
		    return nextIndex;
		}

		/// <summary>
		///    This method will return the selected company role value.
		/// </summary>
		/// <returns></returns>
		private COMPANY_ROLE GetRole()
		{
			var role = COMPANY_ROLE.NO_COMPANY_ROLE;

			try
			{
				int intRole = Convert.ToInt32(this.RoleDropDown.SelectedValue);

				switch (intRole)
				{
					case 0:
						role = COMPANY_ROLE.MANAGER;
						break;
					case 1:
						role = COMPANY_ROLE.OWNER;
						break;
					case 2:
						role = COMPANY_ROLE.SHIPPER;
						break;
					case 3:
						role = COMPANY_ROLE.CUSTOMER_BILLTO;
						break;
					case 4:
						role = COMPANY_ROLE.CUSTOMER_SHIPTO;
						break;
					case 5:
						role = COMPANY_ROLE.CARRIER;
						break;
					case 6:
						role = COMPANY_ROLE.SUPPLIER;
						break;
					case 7:
						role = COMPANY_ROLE.MAX_COMPANY_ROLE;
						break;
					case 8:
						role = COMPANY_ROLE.NO_COMPANY_ROLE;
						break;
				}
			}
			catch (Exception)
			{
				// Expected.
			}

			return role;
		}

		/// <summary>
		///    This method will return the Role count for a particular role that was
		///    saved in session.
		/// </summary>
		/// <param name="inRole"></param>
		/// <returns></returns>
		private int GetTotalRoleCount(COMPANY_ROLE inRole)
		{
			int roleCount = 0;

			switch (inRole)
			{
				case COMPANY_ROLE.MANAGER:
					if (this.Page.Session[PageSessionKeyConstants.CRAF_SESSION_ROLE_MANAGER_COUNT] != null)
					{
						roleCount = Convert.ToInt32(this.Page.Session[PageSessionKeyConstants.CRAF_SESSION_ROLE_MANAGER_COUNT].ToString());
					}
					break;
				case COMPANY_ROLE.CARRIER:
					if (this.Page.Session[PageSessionKeyConstants.CRAF_SESSION_ROLE_CARRIER_COUNT] != null)
					{
						roleCount = Convert.ToInt32(this.Page.Session[PageSessionKeyConstants.CRAF_SESSION_ROLE_CARRIER_COUNT].ToString());
					}
					break;
				case COMPANY_ROLE.CUSTOMER_BILLTO:
					if (this.Page.Session[PageSessionKeyConstants.CRAF_SESSION_ROLE_BILLTO_COUNT] != null)
					{
						roleCount = Convert.ToInt32(this.Page.Session[PageSessionKeyConstants.CRAF_SESSION_ROLE_BILLTO_COUNT].ToString());
					}
					break;
				case COMPANY_ROLE.CUSTOMER_SHIPTO:
					if (this.Page.Session[PageSessionKeyConstants.CRAF_SESSION_ROLE_SHIPTO_COUNT] != null)
					{
						roleCount = Convert.ToInt32(this.Page.Session[PageSessionKeyConstants.CRAF_SESSION_ROLE_SHIPTO_COUNT].ToString());
					}
					break;
				case COMPANY_ROLE.OWNER:
					if (this.Page.Session[PageSessionKeyConstants.CRAF_SESSION_ROLE_OWNER_COUNT] != null)
					{
						roleCount = Convert.ToInt32(this.Page.Session[PageSessionKeyConstants.CRAF_SESSION_ROLE_OWNER_COUNT].ToString());
					}
					break;
				case COMPANY_ROLE.SHIPPER:
					if (this.Page.Session[PageSessionKeyConstants.CRAF_SESSION_ROLE_SHIPPER_COUNT] != null)
					{
						roleCount = Convert.ToInt32(this.Page.Session[PageSessionKeyConstants.CRAF_SESSION_ROLE_SHIPPER_COUNT].ToString());
					}
					break;
				case COMPANY_ROLE.SUPPLIER:
					if (this.Page.Session[PageSessionKeyConstants.CRAF_SESSION_ROLE_SUPPLIER_COUNT] != null)
					{
						roleCount = Convert.ToInt32(
							this.Page.Session[PageSessionKeyConstants.CRAF_SESSION_ROLE_SUPPLIER_COUNT].ToString());
					}
					break;
			}

			return roleCount;
		}

		/// <summary>
		///    This method will determine if the All check box for a given company role is checked or
		///    not checked. If the the state has changed from the previous setting, then all the item
		///    will be updated with the change.
		/// </summary>
		/// <param name="inCheckBox"></param>
		/// <param name="inRole"></param>
		private void HandleAllRoleAssignment(FMCheckBox inCheckBox, COMPANY_ROLE inRole)
		{
			if (inCheckBox == null)
			{
				return;
			}

			var checkAllStateHshTbl = (Hashtable)this.Page.Session[PageSessionKeyConstants.CRAF_SESSION_APPLY_ALL_SETTING];

			if (checkAllStateHshTbl != null)
			{
				var isChecked = (bool)checkAllStateHshTbl[inRole];

				if (inCheckBox.Checked != isChecked)
				{
					var dataList = this.Page.Session[PageSessionKeyConstants.CRAF_SESSION_ROLE_DATA_LIST] as List<CompanyRoleMapClass>;

					if (inCheckBox.Checked)
					{
						this.UpdateAllRole(dataList, inRole, CheckedAllStates.Checked);
					}
					else
					{
						this.UpdateAllRole(dataList, inRole, CheckedAllStates.Unchecked);
					}
				}
			}
		}

		/// <summary>
		///    This method will update the role assignment for any of the role check box
		///    events. It is called by one of the check change event handlers.
		/// </summary>
		/// <param name="companyRoleMaps"></param>
		/// <param name="inCheckBox"></param>
		/// <param name="inRole"></param>
		private void HandleRoleAssignmentUpdate(ICompanyRoleMaps companyRoleMaps, FMCheckBox inCheckBox, COMPANY_ROLE inRole)
		{
		    var dataGridItem = inCheckBox?.NamingContainer as DataGridItem;
			if (dataGridItem != null)
			{
				int itemIndex = this.GetNextRoleMapIndex(dataGridItem.ItemIndex);

				var dataList = this.Page.Session[PageSessionKeyConstants.CRAF_SESSION_ROLE_DATA_LIST] as List<CompanyRoleMapClass>;

			    CompanyRoleMapClass companyRoleMap = dataList?[itemIndex];

			    if (companyRoleMap != null)
			    {
			        bool hasRoleX = false;

			        switch (inRole)
			        {
			            case COMPANY_ROLE.CARRIER:
			                hasRoleX = companyRoleMap.HasCarrierRole;
			                break;
			            case COMPANY_ROLE.CUSTOMER_BILLTO:
			                hasRoleX = companyRoleMap.HasBillToRole;
			                break;
			            case COMPANY_ROLE.CUSTOMER_SHIPTO:
			                hasRoleX = companyRoleMap.HasShipToRole;
			                break;
			            case COMPANY_ROLE.MANAGER:
			                hasRoleX = companyRoleMap.HasManagerRole;
			                break;
			            case COMPANY_ROLE.OWNER:
			                hasRoleX = companyRoleMap.HasOwnerRole;
			                break;
			            case COMPANY_ROLE.SHIPPER:
			                hasRoleX = companyRoleMap.HasShipperRole;
			                break;
			            case COMPANY_ROLE.SUPPLIER:
			                hasRoleX = companyRoleMap.HasSupplierRole;
			                break;
			        }

			        if (hasRoleX && (inCheckBox.Checked == false))
			        {
			            companyRoleMap.Role = inRole;
			            companyRoleMaps.Purge(this.Security, companyRoleMap);
			        }
			        else if ((hasRoleX == false) && inCheckBox.Checked)
			        {
			            companyRoleMap.Role = inRole;
			            companyRoleMaps.Add(this.Security, companyRoleMap);
			        }
			    }
			}
		}

		/// <summary>
		///    This method initialize and adds events to the page.
		/// </summary>
		private void InitializeComponent()
		{
			this.CompanyRolesGrid.ItemDataBound += this.CompanyRolesGridItemDataBound;
			this.CompanyRolesGrid.PageIndexChanged += this.CompanyRoleDataGridPageIndexChanged;
		}

		/// <summary>
		///    This method will return true if the item in the grid is the first overall item. It
		///    will otherwise return false.
		/// </summary>
		/// <param name="inCheckBox"></param>
		/// <returns></returns>
		private bool IsFirstRow(FMCheckBox inCheckBox)
		{
			bool isFirstRow = false;
			var dataGridItem = inCheckBox.NamingContainer as DataGridItem;
			if (dataGridItem != null)
			{
				int itemIndex = this.GetNextRoleMapIndex(dataGridItem.ItemIndex);

				if ((itemIndex == 0) && (this.Page.Session[PageSessionKeyConstants.CRAF_SESSION_APPLY_ALL_SETTING] != null))
				{
					isFirstRow = true;
				}
			}

			return isFirstRow;
		}

		/// <summary>
		///    This method will load the company role dropdown with the appropriate values.
		/// </summary>
		private void LoadRoleDropDown()
		{
			this.RoleDropDown.Items.Clear();

			var intRole = (int)COMPANY_ROLE.MAX_COMPANY_ROLE;
			var listItem = new ListItem(CompanyRoleMapClass.RoleID(COMPANY_ROLE.MAX_COMPANY_ROLE), intRole.ToString(CultureInfo.InvariantCulture));
			this.RoleDropDown.Items.Add(listItem);

			intRole = (int)COMPANY_ROLE.NO_COMPANY_ROLE;
			listItem = new ListItem(CompanyRoleMapClass.RoleID(COMPANY_ROLE.NO_COMPANY_ROLE), intRole.ToString(CultureInfo.InvariantCulture));
			this.RoleDropDown.Items.Add(listItem);

			intRole = (int)COMPANY_ROLE.CARRIER;
			listItem = new ListItem(CompanyRoleMapClass.RoleID(COMPANY_ROLE.CARRIER), intRole.ToString(CultureInfo.InvariantCulture));
			this.RoleDropDown.Items.Add(listItem);

			intRole = (int)COMPANY_ROLE.CUSTOMER_BILLTO;
			listItem = new ListItem(CompanyRoleMapClass.RoleID(COMPANY_ROLE.CUSTOMER_BILLTO), intRole.ToString(CultureInfo.InvariantCulture));
			this.RoleDropDown.Items.Add(listItem);

			intRole = (int)COMPANY_ROLE.CUSTOMER_SHIPTO;
			listItem = new ListItem(CompanyRoleMapClass.RoleID(COMPANY_ROLE.CUSTOMER_SHIPTO), intRole.ToString(CultureInfo.InvariantCulture));
			this.RoleDropDown.Items.Add(listItem);

			intRole = (int)COMPANY_ROLE.MANAGER;
			listItem = new ListItem(CompanyRoleMapClass.RoleID(COMPANY_ROLE.MANAGER), intRole.ToString(CultureInfo.InvariantCulture));
			this.RoleDropDown.Items.Add(listItem);

			intRole = (int)COMPANY_ROLE.OWNER;
			listItem = new ListItem(CompanyRoleMapClass.RoleID(COMPANY_ROLE.OWNER), intRole.ToString(CultureInfo.InvariantCulture));
			this.RoleDropDown.Items.Add(listItem);

			intRole = (int)COMPANY_ROLE.SHIPPER;
			listItem = new ListItem(CompanyRoleMapClass.RoleID(COMPANY_ROLE.SHIPPER), intRole.ToString(CultureInfo.InvariantCulture));
			this.RoleDropDown.Items.Add(listItem);

			intRole = (int)COMPANY_ROLE.SUPPLIER;
			listItem = new ListItem(CompanyRoleMapClass.RoleID(COMPANY_ROLE.SUPPLIER), intRole.ToString(CultureInfo.InvariantCulture));
			this.RoleDropDown.Items.Add(listItem);

			// Initially set the default to the first item in the list.
			this.RoleDropDown.SelectedIndex = 0;
		}

		/// <summary>
		///    This method will load the site dropdown list with a list of sites if the login site
		///    is a site group or just one site if not a site group.
		/// </summary>
		private void LoadSiteDropDown()
		{
			this.SiteDropDown.Items.Clear();

			SiteClass site = FMChannelHelper.MakeCall<ISites, SiteClass>(
				sites => sites.GetBasic(this.Security, this.Security.SiteGuid));

			if (site != null)
			{
				if (site.SiteGroup)
				{
					SiteCollectionClass siteCollection = FMChannelHelper.MakeCall<ISites, SiteCollectionClass>(
							x =>
							x.EnumerateLimitSiteMemberByParentSite(this.Security, this.Security.SiteGuid)
					);

					this.SiteDropDown.Items.Add(new ListItem(site.ID, this.Security.SiteGuid.ToString()));

					foreach (SiteClass nextSite in siteCollection)
					{
						this.SiteDropDown.Items.Add(new ListItem(nextSite.ID, nextSite.IdentityGuid.ToString()));
					}
				}
				else
				{
					this.SiteDropDown.Items.Add(new ListItem(site.ID, this.Security.SiteGuid.ToString()));
				}

				// Initially set the default to the first item in the list.
				this.SiteDropDown.SelectedIndex = 0;

				// This will set the Include member sites checkbox.
				this.SiteSelectionChange(null, null);
			}
		}

		/// <summary>
		///    This method will persist the company role page filters.
		/// </summary>
		private void PersistFilters()
		{
			if (this.Page.Session[PageSessionKeyConstants.CRAF_SESSION_COMPANY_SELECT] == null)
			{
				if (!string.IsNullOrEmpty(this.CompanyTextBox?.Text))
				{
					this.Page.Session.Add(PageSessionKeyConstants.CRAF_SESSION_COMPANY_SELECT, this.CompanyTextBox.Text);
				}
				else
				{
					this.Page.Session.Add(PageSessionKeyConstants.CRAF_SESSION_COMPANY_SELECT, "{All}");
				}
			}
			else
			{
				if (!string.IsNullOrEmpty(this.CompanyTextBox?.Text))
				{
					this.Page.Session[PageSessionKeyConstants.CRAF_SESSION_COMPANY_SELECT] = this.CompanyTextBox.Text;
				}
				else
				{
					this.Page.Session[PageSessionKeyConstants.CRAF_SESSION_COMPANY_SELECT] = "{All}";
				}
			}

			if (this.Page.Session[PageSessionKeyConstants.CRAF_SESSION_FIND_STRING] == null)
			{
				if (!string.IsNullOrEmpty(this.FindTextBox?.Text))
				{
					this.Page.Session.Add(PageSessionKeyConstants.CRAF_SESSION_FIND_STRING, this.FindTextBox.Text.ToUpper());
				}
				else
				{
					this.Page.Session.Add(PageSessionKeyConstants.CRAF_SESSION_FIND_STRING, string.Empty);
				}
			}
			else
			{
				if (!string.IsNullOrEmpty(this.FindTextBox?.Text))
				{
					this.Page.Session[PageSessionKeyConstants.CRAF_SESSION_FIND_STRING] = this.FindTextBox.Text.ToUpper();
				}
				else
				{
					this.Page.Session[PageSessionKeyConstants.CRAF_SESSION_FIND_STRING] = string.Empty;
				}
			}

			if (this.Page.Session[PageSessionKeyConstants.CRAF_SESSION_COMPANY_ROLE_SELECT] == null)
			{
				this.Page.Session.Add(
					PageSessionKeyConstants.CRAF_SESSION_COMPANY_ROLE_SELECT, this.RoleDropDown.SelectedIndex.ToString(CultureInfo.InvariantCulture));
			}
			else
			{
				this.Page.Session[PageSessionKeyConstants.CRAF_SESSION_COMPANY_ROLE_SELECT] = this.RoleDropDown.SelectedIndex.ToString(CultureInfo.InvariantCulture);
			}

			if (this.Page.Session[PageSessionKeyConstants.CRAF_SESSION_SITE_SELECT] == null)
			{
				this.Page.Session.Add(PageSessionKeyConstants.CRAF_SESSION_SITE_SELECT, this.SiteDropDown.SelectedIndex.ToString(CultureInfo.InvariantCulture));
			}
			else
			{
				this.Page.Session[PageSessionKeyConstants.CRAF_SESSION_SITE_SELECT] = this.SiteDropDown.SelectedIndex.ToString(CultureInfo.InvariantCulture);
			}

			if (this.Page.Session[PageSessionKeyConstants.CRAF_SESSION_INCLUDE_MEMBERS] == null)
			{
				this.Page.Session.Add(PageSessionKeyConstants.CRAF_SESSION_INCLUDE_MEMBERS, this.IncludeMemberSitesCheckBox.Checked);
			}
			else
			{
				this.Page.Session[PageSessionKeyConstants.CRAF_SESSION_INCLUDE_MEMBERS] = this.IncludeMemberSitesCheckBox.Checked;
			}
		}

		/// <summary>
		///    This method will check each Role (i.e. Manager, owner, etc) for each row in the collection
		///    and add up the totals. This will be used to set the Apply To All check box. The totals
		///    will be saved in session.
		/// </summary>
		/// <param name="list"></param>
		private void RoleCheckedCount(IEnumerable<CompanyRoleMapClass> list)
		{
			int rowCount = -1;
			int totalManagerCount = 0;
			int totalOwnerCount = 0;
			int totalCarrierCount = 0;
			int totalShipToCount = 0;
			int totalBillToCount = 0;
			int totalShipperCount = 0;
			int totalSupplierCount = 0;

			foreach (CompanyRoleMapClass companyRoleMap in list)
			{
				rowCount++;

				// The first row in the list is the Apply To All. The second row in the
				// list is the Apply To All separator.  Therefore, ignore for the counting.
				if ((rowCount == 0) || (rowCount == 1))
				{
					continue;
				}

				if (companyRoleMap.HasManagerRole)
				{
					totalManagerCount++;
				}
				if (companyRoleMap.HasOwnerRole)
				{
					totalOwnerCount++;
				}
				if (companyRoleMap.HasCarrierRole)
				{
					totalCarrierCount++;
				}
				if (companyRoleMap.HasShipToRole)
				{
					totalShipToCount++;
				}
				if (companyRoleMap.HasBillToRole)
				{
					totalBillToCount++;
				}
				if (companyRoleMap.HasShipperRole)
				{
					totalShipperCount++;
				}
				if (companyRoleMap.HasSupplierRole)
				{
					totalSupplierCount++;
				}
			}

			if (this.Page.Session[PageSessionKeyConstants.CRAF_SESSION_ROLE_MANAGER_COUNT] != null)
			{
				this.Page.Session.Remove(PageSessionKeyConstants.CRAF_SESSION_ROLE_MANAGER_COUNT);
			}

			if (this.Page.Session[PageSessionKeyConstants.CRAF_SESSION_ROLE_OWNER_COUNT] != null)
			{
				this.Page.Session.Remove(PageSessionKeyConstants.CRAF_SESSION_ROLE_OWNER_COUNT);
			}

			if (this.Page.Session[PageSessionKeyConstants.CRAF_SESSION_ROLE_SHIPTO_COUNT] != null)
			{
				this.Page.Session.Remove(PageSessionKeyConstants.CRAF_SESSION_ROLE_SHIPTO_COUNT);
			}

			if (this.Page.Session[PageSessionKeyConstants.CRAF_SESSION_ROLE_BILLTO_COUNT] != null)
			{
				this.Page.Session.Remove(PageSessionKeyConstants.CRAF_SESSION_ROLE_BILLTO_COUNT);
			}

			if (this.Page.Session[PageSessionKeyConstants.CRAF_SESSION_ROLE_CARRIER_COUNT] != null)
			{
				this.Page.Session.Remove(PageSessionKeyConstants.CRAF_SESSION_ROLE_CARRIER_COUNT);
			}

			if (this.Page.Session[PageSessionKeyConstants.CRAF_SESSION_ROLE_SHIPPER_COUNT] != null)
			{
				this.Page.Session.Remove(PageSessionKeyConstants.CRAF_SESSION_ROLE_SHIPPER_COUNT);
			}

			if (this.Page.Session[PageSessionKeyConstants.CRAF_SESSION_ROLE_SUPPLIER_COUNT] != null)
			{
				this.Page.Session.Remove(PageSessionKeyConstants.CRAF_SESSION_ROLE_SUPPLIER_COUNT);
			}

			this.Page.Session.Add(PageSessionKeyConstants.CRAF_SESSION_ROLE_MANAGER_COUNT, totalManagerCount.ToString(CultureInfo.InvariantCulture));
			this.Page.Session.Add(PageSessionKeyConstants.CRAF_SESSION_ROLE_OWNER_COUNT, totalOwnerCount.ToString(CultureInfo.InvariantCulture));
			this.Page.Session.Add(PageSessionKeyConstants.CRAF_SESSION_ROLE_SHIPTO_COUNT, totalShipToCount.ToString(CultureInfo.InvariantCulture));
			this.Page.Session.Add(PageSessionKeyConstants.CRAF_SESSION_ROLE_BILLTO_COUNT, totalBillToCount.ToString(CultureInfo.InvariantCulture));
			this.Page.Session.Add(PageSessionKeyConstants.CRAF_SESSION_ROLE_CARRIER_COUNT, totalCarrierCount.ToString(CultureInfo.InvariantCulture));
			this.Page.Session.Add(PageSessionKeyConstants.CRAF_SESSION_ROLE_SHIPPER_COUNT, totalShipperCount.ToString(CultureInfo.InvariantCulture));
			this.Page.Session.Add(PageSessionKeyConstants.CRAF_SESSION_ROLE_SUPPLIER_COUNT, totalSupplierCount.ToString(CultureInfo.InvariantCulture));
		}

		/// <summary>
		///    This method will set the Check Boxes to the appropriate state (Checked or Unchecked) based
		///    on the Role. If the first row (index set to zero) "Apply To All", the check if see if all
		///    the flags for a particular role are checked. If so, the check the role check box.
		/// </summary>
		/// <param name="checkBox"></param>
		/// <param name="hasRoleX"></param>
		/// <param name="inRole"></param>
		/// <param name="boundIndex"></param>
		/// <param name="listCount"></param>
		private void SetCheckbox(FMCheckBox checkBox, bool hasRoleX, COMPANY_ROLE inRole, int boundIndex, int listCount)
		{
			if (checkBox != null)
			{
				// If the first row "Apply To All", then determine if the role check box should be checked or not
				// based on the role count verses the total list count.
				if (boundIndex == 0)
				{
					// Do only one time. Manager role is always first.
					Hashtable applyAllHshTbl;
					if (inRole == COMPANY_ROLE.MANAGER)
					{
						applyAllHshTbl = new Hashtable();

						if (this.Page.Session[PageSessionKeyConstants.CRAF_SESSION_APPLY_ALL_SETTING] != null)
						{
							this.Page.Session.Remove(PageSessionKeyConstants.CRAF_SESSION_APPLY_ALL_SETTING);
						}

						this.Page.Session.Add(PageSessionKeyConstants.CRAF_SESSION_APPLY_ALL_SETTING, applyAllHshTbl);
					}
					else
					{
						applyAllHshTbl = (Hashtable)this.Page.Session[PageSessionKeyConstants.CRAF_SESSION_APPLY_ALL_SETTING];
					}

					// Remember that the first item in the list is the Apply To All row and the second
					// item in the list is the Apply To All separator.  Therefore, remove it from the count.
					int totalInList = listCount - 2;

					if (totalInList == this.GetTotalRoleCount(inRole))
					{
						checkBox.Checked = true;
					}
					else
					{
						checkBox.Checked = false;
					}

					if (applyAllHshTbl.Contains(inRole))
					{
						applyAllHshTbl[inRole] = checkBox.Checked;
					}
					else
					{
						applyAllHshTbl.Add(inRole, checkBox.Checked);
					}
				}
				else
				{
					// Not the first row, so set per company property.
					checkBox.Checked = hasRoleX;
				}
			}
		}

		/// <summary>
		///    This method will set all the Filters to their previous values.
		/// </summary>
		private void SetFilterFields()
		{
			if (this.Page.Session[PageSessionKeyConstants.CRAF_SESSION_COMPANY_SELECT] != null)
			{
				this.CompanyTextBox.Text = this.Page.Session[PageSessionKeyConstants.CRAF_SESSION_COMPANY_SELECT].ToString();
			}
			else
			{
				this.CompanyTextBox.Text = "{All}";
			}

			if (this.Page.Session[PageSessionKeyConstants.CRAF_SESSION_FIND_STRING] != null)
			{
				this.FindTextBox.Text = this.Page.Session[PageSessionKeyConstants.CRAF_SESSION_FIND_STRING].ToString();
			}

			if (this.Page.Session[PageSessionKeyConstants.CRAF_SESSION_COMPANY_ROLE_SELECT] != null)
			{
				int selectedIndex =
					Convert.ToInt32(this.Page.Session[PageSessionKeyConstants.CRAF_SESSION_COMPANY_ROLE_SELECT].ToString());
				this.RoleDropDown.SelectedIndex = selectedIndex;
			}

			if (this.Page.Session[PageSessionKeyConstants.CRAF_SESSION_SITE_SELECT] != null)
			{
				int selectedIndex = Convert.ToInt32(this.Page.Session[PageSessionKeyConstants.CRAF_SESSION_SITE_SELECT].ToString());
				this.SiteDropDown.SelectedIndex = selectedIndex;
			}

			if (this.Page.Session[PageSessionKeyConstants.CRAF_SESSION_INCLUDE_MEMBERS] != null)
			{
				this.IncludeMemberSitesCheckBox.Checked =
					(bool)this.Page.Session[PageSessionKeyConstants.CRAF_SESSION_INCLUDE_MEMBERS];
			}
		}

		/// <summary>
		///    This method will check or unchecked all the rows for a given company role.
		/// </summary>
		/// <param name="dataList"></param>
		/// <param name="inRole"></param>
		/// <param name="checkedState"></param>
		private void UpdateAllRole(IEnumerable<CompanyRoleMapClass> dataList, COMPANY_ROLE inRole, CheckedAllStates checkedState)
		{
			if (dataList != null)
			{
				foreach (CompanyRoleMapClass companyRoleMap in dataList)
				{
					// The "Apply To All" and blue border rows have an empty company guid and
					// are invalid for assignment.
					if (companyRoleMap.CompanyGuid == Guid.Empty)
					{
						continue;
					}

					bool hasRoleX = false;

					switch (inRole)
					{
						case COMPANY_ROLE.CARRIER:
							hasRoleX = companyRoleMap.HasCarrierRole;
							break;
						case COMPANY_ROLE.CUSTOMER_BILLTO:
							hasRoleX = companyRoleMap.HasBillToRole;
							break;
						case COMPANY_ROLE.CUSTOMER_SHIPTO:
							hasRoleX = companyRoleMap.HasShipToRole;
							break;
						case COMPANY_ROLE.MANAGER:
							hasRoleX = companyRoleMap.HasManagerRole;
							break;
						case COMPANY_ROLE.OWNER:
							hasRoleX = companyRoleMap.HasOwnerRole;
							break;
						case COMPANY_ROLE.SHIPPER:
							hasRoleX = companyRoleMap.HasShipperRole;
							break;
						case COMPANY_ROLE.SUPPLIER:
							hasRoleX = companyRoleMap.HasSupplierRole;
							break;
					}

					if (hasRoleX && (checkedState == CheckedAllStates.Unchecked))
					{
						companyRoleMap.Role = inRole;
						this.PurgeCompanyRoleMap(this.Security, companyRoleMap);
					}
					else if ((hasRoleX == false) && (checkedState == CheckedAllStates.Checked))
					{
						companyRoleMap.Role = inRole;

						try
						{
							this.AddCompanyRoleMap(this.Security, companyRoleMap);
						}
						catch (Exception)
						{
							if (inRole == COMPANY_ROLE.OWNER)
							{
								const string ErrMsg = "Single owner system, cannot have more than one owner role.";
								throw new Exception(ErrMsg);
							}
						}
					}
				}
			}
		}

		private void AddCompanyRoleMap(SecurityClass securityClass, CompanyRoleMapClass companyRoleMapClass)
		{
			FMChannelHelper.MakeCall<ICompanyRoleMaps>(
																	 x =>
																	 x.Add(securityClass, companyRoleMapClass)
																);
		}

		private void PurgeCompanyRoleMap(SecurityClass securityClass, CompanyRoleMapClass companyRoleMap)
		{
			FMChannelHelper.MakeCall<ICompanyRoleMaps>(
																	 x =>
																	 x.Purge(securityClass, companyRoleMap)
																);
		}

		/// <summary>
		///    This method will retrieve new company role data and bind the data to
		///    the company role grid.  The data will be retrieved based on the filting
		///    criterion.
		/// </summary>
		private void UpdateView()
		{
			Guid companyGuid = this.GetCompanyIdentityGuid();
			string findString = this.FindTextBox.Text;
			COMPANY_ROLE role = this.GetRole();
			Guid siteGuid = Guid.Parse(this.SiteDropDown.SelectedValue);
			bool includeMemberSites = this.IncludeMemberSitesCheckBox.Checked;
			string sortKey = "ID";

			if (this.IncludeMemberSitesCheckBox.Enabled == false)
			{
				includeMemberSites = false;
			}

			if (!string.IsNullOrEmpty(findString))
			{
				findString = findString.ToUpper();
			}

			if (this.Page.Session[PageSessionKeyConstants.CRAF_SESSION_SORT_KEY] != null)
			{
				sortKey = (string)this.Page.Session[PageSessionKeyConstants.CRAF_SESSION_SORT_KEY];
			}

			try
			{
				List<CompanyRoleMapClass> list =
				FMChannelHelper.MakeCall<ICompanyRoleMaps, List<CompanyRoleMapClass>>(
																	 x =>
																	 x.EnumerateByCriterion(this.Security, siteGuid, findString,
																	 companyGuid, role, includeMemberSites, sortKey)
																);

				if (this.Page.Session[PageSessionKeyConstants.CRAF_SESSION_ROLE_DATA_LIST] != null)
				{
					this.Page.Session.Remove(PageSessionKeyConstants.CRAF_SESSION_ROLE_DATA_LIST);
				}

				this.Page.Session.Add(PageSessionKeyConstants.CRAF_SESSION_ROLE_DATA_LIST, list);

				// Check each Role (i.e. Manager, owner, etc) for each row in the collection
				// and add up the totals. This will be used to set the Apply To All check box.
				this.RoleCheckedCount(list);

				// Bind the data to the grid.
				this.CompanyRolesGrid.DataSource = list;
				this.CompanyRolesGrid.DataBind();
			}
			catch (Exception)
			{
				const string ErrMsg = "Error retrieving Company Roles";
			    this.ErrorHandler(new Exception(ErrMsg));
			}
		}

		#endregion
	}
}