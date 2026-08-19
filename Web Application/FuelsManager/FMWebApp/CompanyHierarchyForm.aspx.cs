// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CompanyHierarchyForm.aspx.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the CompanyHierarchyForm type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FuelsManager.FMWebApp
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data;
    using System.Drawing;
    using System.Web.UI.WebControls;

    using FMBusinessObjects.BusinessInterfaces;
    using FMBusinessObjects.ChannelFactories;
    using FMBusinessObjects.DataObjects;

    /// <summary>
	///    Summary description for CompanyHierarchyForm.
	/// </summary>
	public partial class CompanyHierarchyForm : FMFormBase, IMenuDiscovery
	{
		#region Public Methods and Operators

		public ListItemCollection EnumerateDrivers()
		{
			var driverItems = new ListItemCollection();

			try
			{
				CompanyMapCollectionClass loadIDs;
                PersonCollectionClass driverCollection;
				if (this.OffLoadingRadioButton.Checked)
				{
					loadIDs = (CompanyMapCollectionClass)this.Session["Off-LoadIDs"];
                    driverCollection = FMChannelHelper.MakeCall<IPersonnel, PersonCollectionClass>(
                                                                         x =>
                                                                         x.EnumerateByRole(this.Security, PERSON_ROLE.OFFLOADER_ROLE)
                                                                    );
                }
                else
				{
					loadIDs = (CompanyMapCollectionClass)this.Session["LoadIDs"];
                    driverCollection = FMChannelHelper.MakeCall<IPersonnel, PersonCollectionClass>(
                                                                         x =>
                                                                         x.EnumerateByRole(this.Security, PERSON_ROLE.LOADER_ROLE)
                                                                    );
                }

				foreach (PersonClass driver in driverCollection)
				{
					bool found = false;
					int item = 0;
					foreach (CompanyMapClass loadID in loadIDs)
					{
						if (item
							 != this.LoadIDDataGrid.EditItemIndex + this.LoadIDDataGrid.CurrentPageIndex * this.LoadIDDataGrid.PageSize
							 && driver.MasterRecordGuid == loadID.AssignedGuid)
						{
							found = true;
							break;
						}
						item++;
					}

					if (found)
					{
						continue;
					}

					var newDriverItem = new ListItem(driver.ID, driver.MasterRecordGuid.ToString());
					foreach (ListItem existingDriverItem in driverItems)
					{
						if (string.Compare(existingDriverItem.Text, newDriverItem.Text, StringComparison.Ordinal) > 0)
						{
							int index = driverItems.IndexOf(existingDriverItem);
							driverItems.Insert(index, newDriverItem);
							newDriverItem = null;
							break;
						}
					}

					if (newDriverItem != null)
					{
						driverItems.Add(newDriverItem);
					}
				}

                //Add "{All}" into the list and put on the top
                var newDriverItemAll = new ListItem("{All}", Guid.Empty.ToString());
                driverItems.Insert(0, newDriverItemAll);
            }
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}

			if (driverItems.Count == 0)
			{
				throw new Exception("No Drivers Available");
			}

			return driverItems;
		}

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
                // Depends Upon Accounting
                if ((options & 0x80100) == 0)
                {
                    return null;
                }
            }

            if (siteGroup)
			{
				return null;
			}

			if (!security.HasRight(RIGHT.VIEW_COMPANY_DATA) && !security.HasRight(RIGHT.MODIFY_COMPANY_DATA))
			{
				return null;
			}

			var menuItems = new List<FMMenuItem>();

			var menuItem = new FMMenuItem
				{
					MenuItemType = FMMenuItemType.ACCOUNTING_COMPANIES_HIERARCHY,
					RootMenuName = "Accounting",
					CategoryName = "Companies",
					ItemName = "Hierarchy",
					NavigateUrl = "CompanyHierarchyForm.aspx",
					ApplyDataDictionary = ApplyDataDictionary.Apply
				};

			menuItems.Add(menuItem);

			return menuItems;
		}

		#endregion

		#region Methods

        // ReSharper disable once InconsistentNaming
		protected void LoadingRadioButton_CheckChanged(object sender, EventArgs e)
		{
			try
			{
                this.UpdateHeirarchyTreeControl();                
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

        // ReSharper disable once InconsistentNaming
		protected void OffLoadingRadioButton_CheckChanged(object sender, EventArgs e)
		{
			try
			{
				this.UpdateHeirarchyTreeControl();                
			}
			catch ( Exception except )
			{
				this.ErrorHandler( except );
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

		protected void Page_Load(object sender, EventArgs e)
		{
			try
			{
				this.GetSecurity();
				if (!this.Page.IsPostBack)
				{
					if (this.Security.HasRight(RIGHT.MODIFY_COMPANY_DATA) == false)
					{
						this.AssignButton.Enabled = false;
						this.UnassignButton.Enabled = false;
					}

					this.LoadingRadioButton.Checked = true;
					this.OffLoadingRadioButton.Checked = false;

					// update the tree control based on the selected type
					this.UpdateHeirarchyTreeControl();

					this.LoadIDDataGrid.Visible = false;
					this.AddButton.Visible = false;
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		protected void UpdateHeirarchyTreeControl()
		{
			// remove any current nodes in the tree
			this.HierarchyTreeView.Nodes.Clear();
			this.HierarchyTreeView.AutoGenerateDataBindings = false;
			this.HierarchyTreeView.SelectedNodeStyle.BackColor = Color.LightGray;
			this.HierarchyTreeView.NodeIndent = 60;            
	
			// Add the Manager Nodes    
			var managers = FMChannelHelper.MakeCall<ICompanies, CompanyCollectionClass>(
																	 x =>
																	 x.EnumerateByRole(this.Security, COMPANY_ROLE.MANAGER, false, true));

			var mapType = this.OffLoadingRadioButton.Checked
				? COMPANY_MAP_TYPE.OFFLOAD_OWNER_MANAGER_MAP
				: COMPANY_MAP_TYPE.LOAD_OWNER_MANAGER_MAP;

			var allMaps =
				FMChannelHelper.MakeCall<ICompanyMaps, CompanyMapCollectionClass>(
					maps => maps.EnumerateByType(this.Security, mapType));
			
			foreach (CompanyClass manager in managers)
			{
			    var newNode = new TreeNode
			                  {
			                      Expanded = false,
			                      SelectAction = TreeNodeSelectAction.Select,
			                      Text = manager.ID,
			                      Value = manager.MasterRecordGuid.ToString(),
			                      ToolTip = manager.CompanyToolTip,
			                      ImageUrl = "images\\manager.ico"
			                  };

			    if (this.CheckCount(allMaps, manager))
				{
					var childItemTreeNode = new TreeNode();
					newNode.Expanded = false;
					newNode.SelectAction = TreeNodeSelectAction.Select;
					newNode.ChildNodes.Add(childItemTreeNode);
				}

				foreach (TreeNode existingNode in this.HierarchyTreeView.Nodes)
				{
					if (string.Compare(existingNode.Text, newNode.Text, StringComparison.Ordinal) > 0)
					{
						int index = this.HierarchyTreeView.Nodes.IndexOf(existingNode);
						this.HierarchyTreeView.Nodes.AddAt(index, newNode);
						newNode = null;
						break;
					}
				}

				if (newNode != null)
				{
					this.HierarchyTreeView.Nodes.Add(newNode);
				}
			}

            if (this.HierarchyTreeView.Nodes.Count != 0)
            {
                this.HierarchyTreeView.Nodes[0].Select();
                EventArgs e = new EventArgs();
                this.HierarchyTreeView_SelectedNodeChange(this.HierarchyTreeView, e);
            }
            
			this.FunctionLabel.Text = "Unassigned Owners";
			this.CompanyRoleLabel.Text = "Manager";
			this.CompanyRoleLabel.Text = this.GetTranslatedText(this.CompanyRoleLabel.Text) + ":";
			this.FunctionLabel.Text = this.GetTranslatedText(this.FunctionLabel.Text);
		}

		/// <summary>
		/// Checks to see if the count of maps for the specified manager is greater than zero.
		/// </summary>
		/// <param name="allMaps">All the maps of the correct type.</param>
		/// <param name="manager">The manager to check.</param>
		/// <returns>True if the manager has one or more maps assigned to it.</returns>
		private bool CheckCount(CompanyMapCollectionClass allMaps, CompanyClass manager)
		{
			foreach (CompanyMapClass map in allMaps)
			{
				if (map.AssignedToGuid == manager.MasterRecordGuid)
				{
					return true;
				}
			}

			return false;
		}

        // ReSharper disable once InconsistentNaming
		private void AddButton_Command(object sender, CommandEventArgs e)
		{
			TreeNode node = this.HierarchyTreeView.SelectedNode;
			Guid identityGuid = Guid.Parse(node.Value);
			CompanyMapCollectionClass loadIDs;
		    CompanyMapClass loadID;
			if (this.OffLoadingRadioButton.Checked)
			{
				loadIDs = (CompanyMapCollectionClass)this.Session["Off-LoadIDs"];
                loadID = CompanyMapClass.CreateCompanyMap(COMPANY_MAP_TYPE.OFFLOADID_SUPPLIER_MAP);
				loadID.AssignedToGuid = identityGuid;
			}
			else
			{
				loadIDs = (CompanyMapCollectionClass)this.Session["LoadIDs"];
                loadID = CompanyMapClass.CreateCompanyMap(COMPANY_MAP_TYPE.LOADID_SHIPTO_MAP);
				loadID.AssignedToGuid = identityGuid;
			}
			loadIDs.Add(loadID);
			this.LoadIDDataGrid.CurrentPageIndex = (loadIDs.Count - 1) / this.LoadIDDataGrid.PageSize;
			this.LoadIDDataGrid.EditItemIndex = (loadIDs.Count - 1) % this.LoadIDDataGrid.PageSize;
			this.AddButton.Enabled = false;
			this.HierarchyTreeView.Enabled = false;
			try
			{
				this.UpdateLoadIDView();
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
				loadIDs.Remove(loadIDs.Count - 1);
				if (this.LoadIDDataGrid.CurrentPageIndex > 0 && this.LoadIDDataGrid.EditItemIndex == 0)
				{
					this.LoadIDDataGrid.CurrentPageIndex--;
				}
				this.LoadIDDataGrid.EditItemIndex = -1;
				this.AddButton.Enabled = true;
				this.HierarchyTreeView.Enabled = true;
				this.UpdateLoadIDView();
			}
		}

        // ReSharper disable once InconsistentNaming
		private void AssignButton_Command(object sender, CommandEventArgs e)
		{
			try
			{
				TreeNode node = this.HierarchyTreeView.SelectedNode;
				ListItem unassignedCompanyItem;

				while ((unassignedCompanyItem = this.UnassignedCompanyListBox.SelectedItem) != null)
				{
					this.UnassignedCompanyListBox.Items.Remove(unassignedCompanyItem);

                    CompanyMapClass companyMap = CompanyMapClass.CreateCompanyMap(this.GetMapType(node.Depth));
					companyMap.AssignedToGuid = Guid.Parse(node.Value);
					object parent = node.Parent;
					companyMap.AssignedToID = node.Text;
					while (parent != null)
					{
						companyMap.AssignedToID = ((TreeNode)parent).Text + "->" + companyMap.AssignedToID;
						parent = ((TreeNode) parent).Parent;
					}

					companyMap.AssignedGuid = Guid.Parse(unassignedCompanyItem.Value);
					companyMap.AssignedID = unassignedCompanyItem.Text;
					companyMap.IdentityGuid = FMChannelHelper.MakeCall<ICompanyMaps, Guid>(
																	 x =>
																	 x.Add(this.Security, companyMap)
																);


				    var newNode = new TreeNode
				                  {
				                      Expanded = false,
				                      SelectAction = TreeNodeSelectAction.Select,
				                      Text = unassignedCompanyItem.Text,
				                      Value = companyMap.IdentityGuid.ToString(),
				                      ImageUrl = this.GetImageUrl(node.Depth)
				                  };
				    newNode.ImageToolTip = newNode.Text;

					foreach (TreeNode existingNode in node.ChildNodes)
					{
						if (string.Compare(existingNode.Text, newNode.Text, StringComparison.Ordinal) > 0)
						{
							int index = node.ChildNodes.IndexOf(existingNode);
							node.ChildNodes.AddAt(index, newNode);
							newNode = null;
							break;
						}
					}

					if (newNode != null)
					{
						node.ChildNodes.Add(newNode);
					}
				}

				if (node.Expanded == null
				|| !(bool) node.Expanded)
				{
					node.Expanded = true;
					var eventArgs = new TreeNodeEventArgs(node);
					this.HierarchyTreeView_Expand(this.HierarchyTreeView, eventArgs);
				}

				this.AssignButton.Enabled = (this.UnassignedCompanyListBox.Items.Count != 0);
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		private ICollection EnumerateLoadIDs()
		{
			var loadIDDataTable = new DataTable();

		    loadIDDataTable.Columns.Add("Index", typeof(int));
			loadIDDataTable.Columns.Add("LoadID", typeof(string));
			loadIDDataTable.Columns.Add("PersonID", typeof(string));

			int item = 0;

			if ((this.Session["LoadIDs"] != null && this.OffLoadingRadioButton.Checked == false)
				 || (this.Session["Off-LoadIDs"] != null && this.OffLoadingRadioButton.Checked))
			{
				CompanyMapCollectionClass loadIDs;
				if (this.OffLoadingRadioButton.Checked)
				{
					loadIDs = (CompanyMapCollectionClass)this.Session["Off-LoadIDs"];
				}
				else
				{
					loadIDs = (CompanyMapCollectionClass)this.Session["LoadIDs"];
				}
			    if (loadIDs != null)
			    {
			        foreach (CompanyMapClass loadID in loadIDs)
			        {
			            var loadIDDataRow = loadIDDataTable.NewRow();

			            loadIDDataRow["Index"] = item;
			            loadIDDataRow["LoadID"] = loadID.MapID;
			            loadIDDataRow["PersonID"] = (loadID.AssignedGuid != Guid.Empty) ? loadID.AssignedID : "{All}";

			            // Sort the DataTable by LoadID except for an Added entry
			            // which has a zero Index, that messes up the sort order
			            // relative to the EditItemIndex
			            if (loadID.IdentityGuid != Guid.Empty)
			            {
			                int iRow = 0;
			                foreach (DataRow row in loadIDDataTable.Rows)
			                {
			                    if (string.Compare(((string)row["LoadID"]), (string)loadIDDataRow["LoadID"], StringComparison.Ordinal) > 0)
			                    {
			                        loadIDDataTable.Rows.InsertAt(loadIDDataRow, iRow);
			                        loadIDDataRow = null;
			                        break;
			                    }
			                    iRow++;
			                }
			            }

			            if (loadIDDataRow != null)
			            {
			                loadIDDataTable.Rows.Add(loadIDDataRow);
			            }

			            item++;
			        }
			    }
			}

			var loadIDDataView = new DataView(loadIDDataTable);
			return loadIDDataView;
		}

		private COMPANY_MAP_TYPE GetMapType(int nodeDepth)
		{
			if (this.OffLoadingRadioButton.Checked)
			{
				switch (nodeDepth)
				{
					case 0:
						return COMPANY_MAP_TYPE.OFFLOAD_OWNER_MANAGER_MAP;
					case 1:
						return COMPANY_MAP_TYPE.SUPPLIER_OWNER_MAP;
					default:
						return COMPANY_MAP_TYPE.OFFLOAD_MAX_COMPANY_MAP_TYPE;
				}
			}

			switch (nodeDepth)
			{
				case 0:
					return COMPANY_MAP_TYPE.LOAD_OWNER_MANAGER_MAP;
				case 1:
					return COMPANY_MAP_TYPE.SHIPPER_OWNER_MAP;
				case 2:
					return COMPANY_MAP_TYPE.BILLTO_SHIPPER_MAP;
				case 3:
					return COMPANY_MAP_TYPE.SHIPTO_BILLTO_MAP;
				default:
					return COMPANY_MAP_TYPE.LOAD_MAX_COMPANY_MAP_TYPE;
			}
		}

		private string GetImageUrl(int nodeDepth)
		{
			if (this.OffLoadingRadioButton.Checked)
			{
				switch (nodeDepth)
				{
					case 0:
						return "images\\owner.ico";
					default:
						return "images\\supplier.ico";
				}
			}

			switch (nodeDepth)
			{
				case 0:
					return "images\\owner.ico";
				case 1:
					return "images\\shipper.ico";
				case 2:
					return "images\\custbill.ico";
				default:
					return "images\\custship.ico";
			}
		}

        // ReSharper disable once InconsistentNaming
		private void HierarchyTreeView_Expand(object sender, TreeNodeEventArgs e)
		{
			try
			{
				TreeNode node = e.Node;

				// Dummy node indicating presence of a list
				if (string.IsNullOrEmpty(node.Value))
				{
					return;
				}

				if (node.ChildNodes.Count > 0 && !string.IsNullOrEmpty(node.ChildNodes[0].Value))
				{
					return;
				}

				node.ChildNodes.Clear();

				CompanyMapCollectionClass maps = FMChannelHelper.MakeCall<ICompanyMaps, CompanyMapCollectionClass>(
																	 x =>
																	 x.EnumerateByAssignedToGuidAndType(
																		this.Security,
																		Guid.Parse(node.Value),
																		this.GetMapType(node.Depth)));

				foreach (CompanyMapClass map in maps)
				{
				    var newNode = new TreeNode
				                  {
				                      Expanded = false,
				                      SelectAction = TreeNodeSelectAction.Select,
				                      Text = map.AssignedID,
				                      Value = map.IdentityGuid.ToString(),
				                      ToolTip = map.AssignedCompanyToolTip,
				                      ImageUrl = this.GetImageUrl(node.Depth)
				                  };
				    newNode.ImageToolTip = newNode.Text;

				    if (this.GetMapType(node.Depth + 1) != COMPANY_MAP_TYPE.LOAD_MAX_COMPANY_MAP_TYPE
				        && this.GetMapType(node.Depth + 1) != COMPANY_MAP_TYPE.OFFLOAD_MAX_COMPANY_MAP_TYPE)
				    {
				        var map1 = map;
				        var subMaps = FMChannelHelper.MakeCall<ICompanyMaps, CompanyMapCollectionClass>(
				            x =>
				                x.EnumerateByAssignedToGuidAndType(
				                    this.Security,
				                    map1.IdentityGuid,
				                    this.GetMapType(node.Depth + 1)));

				        if (subMaps.Count != 0)
				        {
				            var childItemTreeNode = new TreeNode();
				            newNode.Expanded = false;
				            newNode.SelectAction = TreeNodeSelectAction.Select;
				            newNode.ChildNodes.Add(childItemTreeNode);
				        }
				    }

				    foreach (TreeNode existingNode in node.ChildNodes)
				    {
				        if (string.Compare(existingNode.Text, newNode.Text, StringComparison.Ordinal) > 0)
				        {
				            int index = node.ChildNodes.IndexOf(existingNode);
				            node.ChildNodes.AddAt(index, newNode);
				            newNode = null;
				            break;
				        }
				    }

				    if (newNode != null)
				    {
				        node.ChildNodes.Add(newNode);
				    }
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

        // ReSharper disable once InconsistentNaming
		private void HierarchyTreeView_SelectedNodeChange(object sender, EventArgs e)
		{
			this.Session.Remove("LoadIDs");
			this.AssignButton.Visible = true;
			this.AssignButton.Enabled = false;
			this.UnassignButton.Visible = true;
			this.UnassignButton.Enabled = false;
			this.UnassignedCompanyListBox.Visible = true;
			this.AddButton.Visible = false;
			this.LoadIDDataGrid.Visible = false;

			this.UnassignedCompanyListBox.Items.Clear();

			TreeNode node = this.HierarchyTreeView.SelectedNode;
			if (node == null)
			{
				return;
			}

			COMPANY_ROLE role;

			if (this.OffLoadingRadioButton.Checked)
			{
				switch (this.GetMapType(node.Depth))
				{
					case COMPANY_MAP_TYPE.OFFLOAD_OWNER_MANAGER_MAP:
						role = COMPANY_ROLE.OWNER;
						this.FunctionLabel.Text = "Unassigned Owners";
						this.CompanyRoleLabel.Text = "Manager";
						break;
					case COMPANY_MAP_TYPE.SUPPLIER_OWNER_MAP:
						role = COMPANY_ROLE.SUPPLIER;
						this.FunctionLabel.Text = "Unassigned Suppliers";
						this.CompanyRoleLabel.Text = "Owner";
						break;
					default:
					{
						this.AssignButton.Visible = false;
						this.UnassignedCompanyListBox.Visible = false;
						this.AddButton.Visible = true;
						this.LoadIDDataGrid.Visible = true;

						// Ensure that the if the user does not have modify rights that the 
						// assign and unassign buttons are disabled.
						if (this.Security.HasRight(RIGHT.MODIFY_COMPANY_DATA) == false)
						{
							this.AssignButton.Enabled = false;
							this.UnassignButton.Enabled = false;
							this.AddButton.Enabled = false;
						}
						else
						{
							this.AssignButton.Enabled = false;
							this.UnassignButton.Enabled = true;
							this.AddButton.Enabled = true;
						}

						this.CompanyRoleLabel.Text = "Supplier";
						this.FunctionLabel.Text = "Off-Load IDs";

						Guid identityGuid = Guid.Parse(node.Value);
						this.Session["Off-LoadIDs"] = FMChannelHelper.MakeCall<ICompanyMaps, CompanyMapCollectionClass>(
																	x =>
																	x.EnumerateByAssignedToGuidAndType(this.Security,
																	identityGuid, COMPANY_MAP_TYPE.OFFLOADID_SUPPLIER_MAP));

						if (this.Security.HasRight(RIGHT.MODIFY_COMPANY_DATA))
						{
							this.AddButton.Enabled = true;
						}

						this.LoadIDDataGrid.CurrentPageIndex = 0;
						this.UpdateLoadIDView();

						this.CompanyRoleLabel.Text = this.GetTranslatedText(this.CompanyRoleLabel.Text) + ":";
						this.FunctionLabel.Text = this.GetTranslatedText(this.FunctionLabel.Text);

						return;
					}
				}
			}
			else
			{
				switch (this.GetMapType(node.Depth))
				{
					case COMPANY_MAP_TYPE.LOAD_OWNER_MANAGER_MAP:
						role = COMPANY_ROLE.OWNER;
						this.FunctionLabel.Text = "Unassigned Owners";
						this.CompanyRoleLabel.Text = "Manager";
						break;
					case COMPANY_MAP_TYPE.SHIPPER_OWNER_MAP:
						role = COMPANY_ROLE.SHIPPER;
						this.FunctionLabel.Text = "Unassigned Shippers";
						this.CompanyRoleLabel.Text = "Owner";
						break;
					case COMPANY_MAP_TYPE.BILLTO_SHIPPER_MAP:
						role = COMPANY_ROLE.CUSTOMER_BILLTO;
						this.FunctionLabel.Text = "Unassigned Bill To";
						this.CompanyRoleLabel.Text = "Shipper";
						break;
					case COMPANY_MAP_TYPE.SHIPTO_BILLTO_MAP:
						role = COMPANY_ROLE.CUSTOMER_SHIPTO;
						this.FunctionLabel.Text = "Unassigned Ship To";
						this.CompanyRoleLabel.Text = "Bill To";
						break;
					default:
					{
						this.AssignButton.Visible = false;
						this.UnassignedCompanyListBox.Visible = false;
						this.AddButton.Visible = true;
						this.LoadIDDataGrid.Visible = true;

						// Ensure that the if the user does not have modify rights that the 
						// assign and unassign buttons are disabled.
						if (this.Security.HasRight(RIGHT.MODIFY_COMPANY_DATA) == false)
						{
							this.AssignButton.Enabled = false;
							this.UnassignButton.Enabled = false;
							this.AddButton.Enabled = false;
						}
						else
						{
							this.AssignButton.Enabled = false;
							this.UnassignButton.Enabled = true;
							this.AddButton.Enabled = true;
						}

						this.FunctionLabel.Text = "Load IDs";
						this.CompanyRoleLabel.Text = "Ship To";

						Guid identityGuid = Guid.Parse(node.Value);
						this.Session["LoadIDs"] = FMChannelHelper.MakeCall<ICompanyMaps, CompanyMapCollectionClass>(
																	x =>
																	x.EnumerateByAssignedToGuidAndType(this.Security,
																	identityGuid, COMPANY_MAP_TYPE.LOADID_SHIPTO_MAP));

						if (this.Security.HasRight(RIGHT.MODIFY_COMPANY_DATA))
						{
							this.AddButton.Enabled = true;
						}

						this.LoadIDDataGrid.CurrentPageIndex = 0;
						this.UpdateLoadIDView();

						this.CompanyRoleLabel.Text = this.GetTranslatedText(this.CompanyRoleLabel.Text) + ":";
						this.FunctionLabel.Text = this.GetTranslatedText(this.FunctionLabel.Text);


						return;
					}
				}
			}

			this.CompanyRoleLabel.Text = this.GetTranslatedText(this.CompanyRoleLabel.Text) + ":";
			this.FunctionLabel.Text = this.GetTranslatedText(this.FunctionLabel.Text);

			CompanyCollectionClass companyCollection = FMChannelHelper.MakeCall<ICompanies, CompanyCollectionClass>(
																	 x =>
																	 x.EnumerateByRole(this.Security, role, false, true));

			CompanyMapCollectionClass maps = FMChannelHelper.MakeCall<ICompanyMaps, CompanyMapCollectionClass>(
																	 x =>
																	 x.EnumerateByAssignedToGuidAndType(
																		this.Security,
																		Guid.Parse(node.Value),
																		this.GetMapType(node.Depth)));

		    // ReSharper disable once ForCanBeConvertedToForeach
			for (int companyItem = 0; companyItem < companyCollection.Count; companyItem++)
			{
				CompanyClass company = companyCollection[companyItem];
				foreach (CompanyMapClass companyMap in maps)
				{
				    if (companyMap.AssignedGuid == company.MasterRecordGuid)
				    {
				        company = null;
				        break;
				    }
				}

				if (company == null)
				{
					continue;
				}

				var newItem = new ListItem(company.ID, company.MasterRecordGuid.ToString());
				foreach (ListItem existingItem in this.UnassignedCompanyListBox.Items)
				{
					if (string.Compare(existingItem.Text, newItem.Text, StringComparison.Ordinal) > 0)
					{
						int index = this.UnassignedCompanyListBox.Items.IndexOf(existingItem);
						this.UnassignedCompanyListBox.Items.Insert(index, newItem);
						newItem = null;
						break;
					}
				}

				if (newItem != null)
				{
					this.UnassignedCompanyListBox.Items.Add(newItem);
				}
			}

			// Ensure that the if the user does not have modify rights that the 
			// assign and unassign buttons are disabled.
			if (this.Security.HasRight(RIGHT.MODIFY_COMPANY_DATA) == false)
			{
				this.AssignButton.Enabled = false;
				this.UnassignButton.Enabled = false;
			}
			else
			{
				this.AssignButton.Enabled = (this.UnassignedCompanyListBox.Items.Count != 0);
				if (this.OffLoadingRadioButton.Checked)
				{
					this.UnassignButton.Enabled = (this.GetMapType(node.Depth)
					                               != COMPANY_MAP_TYPE.OFFLOAD_OWNER_MANAGER_MAP);
				}
				else
				{
					this.UnassignButton.Enabled = (this.GetMapType(node.Depth)
					                               != COMPANY_MAP_TYPE.LOAD_OWNER_MANAGER_MAP);
				}
			}
		}

		/// <summary>
		///    Required method for Designer support - do not modify
		///    the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.UnassignButton.Command += this.UnassignButton_Command;
			this.OffLoadingRadioButton.CheckedChanged += this.OffLoadingRadioButton_CheckChanged;
			this.LoadingRadioButton.CheckedChanged += this.LoadingRadioButton_CheckChanged;
			this.HierarchyTreeView.TreeNodeExpanded += this.HierarchyTreeView_Expand;
			this.HierarchyTreeView.SelectedNodeChanged += this.HierarchyTreeView_SelectedNodeChange;
			this.AssignButton.Command += this.AssignButton_Command;
			this.LoadIDDataGrid.EditCommand += this.LoadIDDataGrid_EditCommand;
			this.LoadIDDataGrid.PageIndexChanged += this.LoadIDDataGrid_PageIndexChanged;
			this.LoadIDDataGrid.CancelCommand += this.LoadIDDataGrid_CancelCommand;
			this.LoadIDDataGrid.UpdateCommand += this.LoadIDDataGrid_UpdateCommand;
			this.LoadIDDataGrid.DeleteCommand += this.LoadIDDataGrid_DeleteCommand;
			this.LoadIDDataGrid.ItemDataBound += this.LoadIDDataGrid_ItemDataBound;
			this.AddButton.Command += this.AddButton_Command;
		}

        // ReSharper disable once InconsistentNaming
		private void LoadIDDataGrid_CancelCommand(object source, DataGridCommandEventArgs e)
		{
			var indexLabel = (Label)e.Item.FindControl("IndexLabel");
			if (indexLabel != null)
			{
				CompanyMapCollectionClass loadIDs;
				if (this.OffLoadingRadioButton.Checked)
				{
					loadIDs = (CompanyMapCollectionClass)this.Session["Off-LoadIDs"];
				}
				else
				{
					loadIDs = (CompanyMapCollectionClass)this.Session["LoadIDs"];
				}
				CompanyMapClass loadID = loadIDs[Convert.ToInt32(indexLabel.Text)];
				if (loadID.IdentityGuid == Guid.Empty)
				{
					loadIDs.Remove(Convert.ToInt32(indexLabel.Text));
					if (this.LoadIDDataGrid.Items.Count == 1 && this.LoadIDDataGrid.CurrentPageIndex > 0)
					{
						this.LoadIDDataGrid.CurrentPageIndex--;
					}
				}
				this.LoadIDDataGrid.EditItemIndex = -1;
				this.AddButton.Enabled = true;
				this.HierarchyTreeView.Enabled = true;
				this.UpdateLoadIDView();
			}
		}

        // ReSharper disable once InconsistentNaming
		private void LoadIDDataGrid_DeleteCommand(object source, DataGridCommandEventArgs e)
		{
			try
			{
				var indexLabel = (Label)e.Item.FindControl("IndexLabel");
				if (indexLabel != null)
				{
					CompanyMapCollectionClass loadIDs;
					if (this.OffLoadingRadioButton.Checked)
					{
						loadIDs = (CompanyMapCollectionClass)this.Session["Off-LoadIDs"];
					}
					else
					{
						loadIDs = (CompanyMapCollectionClass)this.Session["LoadIDs"];
					}
					CompanyMapClass loadID = loadIDs[Convert.ToInt32(indexLabel.Text)];

					if (this.LoadIDDataGrid.EditItemIndex == e.Item.ItemIndex)
					{
						this.LoadIDDataGrid.EditItemIndex = -1;
						this.AddButton.Enabled = true;
						this.HierarchyTreeView.Enabled = true;
					}
					else if (this.LoadIDDataGrid.EditItemIndex > e.Item.ItemIndex)
					{
						this.LoadIDDataGrid.EditItemIndex--;
					}

					// Non empty identity guid indicates LoadID has been committed to database
					if (this.OffLoadingRadioButton.Checked)
					{
						if (loadID.IdentityGuid != Guid.Empty)
						{
							FMChannelHelper.MakeCall<ICompanyMaps>(
																	 x =>
																	 x.Purge(this.Security, loadID.IdentityGuid, COMPANY_MAP_TYPE.OFFLOADID_SUPPLIER_MAP)
																);
						}
					}
					else
					{
						if (loadID.IdentityGuid != Guid.Empty)
						{
							FMChannelHelper.MakeCall<ICompanyMaps>(
																	 x =>
																	 x.Purge(this.Security, loadID.IdentityGuid, COMPANY_MAP_TYPE.LOADID_SHIPTO_MAP)
																);
						}
					}

					loadIDs.Remove(Convert.ToInt32(indexLabel.Text));
					if (this.LoadIDDataGrid.Items.Count == 1 && this.LoadIDDataGrid.CurrentPageIndex > 0)
					{
						this.LoadIDDataGrid.CurrentPageIndex--;
					}
					this.UpdateLoadIDView();
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

        // ReSharper disable once InconsistentNaming
		private void LoadIDDataGrid_EditCommand(object source, DataGridCommandEventArgs e)
		{
			try
			{
				this.LoadIDDataGrid.EditItemIndex = e.Item.ItemIndex;
				this.AddButton.Enabled = false;
				this.HierarchyTreeView.Enabled = false;
				this.UpdateLoadIDView();
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
				this.LoadIDDataGrid.EditItemIndex = -1;
				this.AddButton.Enabled = true;
				this.HierarchyTreeView.Enabled = true;
				this.UpdateLoadIDView();
			}
		}

        // ReSharper disable once InconsistentNaming
		private void LoadIDDataGrid_ItemDataBound(object sender, DataGridItemEventArgs e)
		{
			if (e.Item.ItemType == ListItemType.Header)
			{
				if (this.OffLoadingRadioButton.Checked)
				{
					e.Item.Cells[2].Text = this.GetTranslatedText("Off Load ID");//bds
				}
				foreach (TableCell tc in e.Item.Cells)
				{
					tc.Text = this.GetTranslatedText(tc.Text);
				}
			}

			// Disable the edit and delete icons if the user does not have
			// the modify company data right.
			bool bEnabled = this.Security.HasRight(RIGHT.MODIFY_COMPANY_DATA);

			var editButton = (LinkButton)e.Item.FindControl("EditButton");
			if (editButton != null)
			{
				editButton.Enabled = bEnabled;
			}

			var deleteButton = (LinkButton)e.Item.FindControl("DeleteButton");
			if (deleteButton != null)
			{
				deleteButton.Enabled = bEnabled;
			}

			var indexLabel = (Label)e.Item.FindControl("IndexLabel");
			if (indexLabel != null)
			{
				var driverDropDownList = (DropDownList)e.Item.FindControl("DriverDropDownList");

				if (driverDropDownList != null)
				{
					CompanyMapCollectionClass loadIDs;
					if (this.OffLoadingRadioButton.Checked)
					{
						loadIDs = (CompanyMapCollectionClass)this.Session["Off-LoadIDs"];
					}
					else
					{
						loadIDs = (CompanyMapCollectionClass)this.Session["LoadIDs"];
					}
					CompanyMapClass loadID = loadIDs[Convert.ToInt32(indexLabel.Text)];

					if (loadID.IdentityGuid != Guid.Empty)
					{
						var item = new ListItem(
							(loadID.AssignedGuid != Guid.Empty) ? loadID.AssignedID : "{All}", loadID.AssignedGuid.ToString());
						int index = driverDropDownList.Items.IndexOf(item);

						if (index != -1)
						{
							driverDropDownList.SelectedIndex = index;
						}
					}
                    else
                    {
                        var item = driverDropDownList.Items.FindByText("{All}");
                        if(item != null)
                            item.Selected = true;
                    }
				}
			}
		}

        // ReSharper disable once InconsistentNaming
		private void LoadIDDataGrid_PageIndexChanged(object source, DataGridPageChangedEventArgs e)
		{
			// if we are editing do not allow a page change
			if (this.LoadIDDataGrid.EditItemIndex > -1)
			{
				return;
			}
			this.LoadIDDataGrid.CurrentPageIndex = e.NewPageIndex;
			this.UpdateLoadIDView();
		}

        // ReSharper disable once InconsistentNaming
		private void LoadIDDataGrid_UpdateCommand(object source, DataGridCommandEventArgs e)
		{
			try
			{
			    // ReSharper disable once InconsistentNaming
			    // ReSharper disable once InconsistentNaming
				var indexLabel = (Label)e.Item.FindControl("IndexLabel");
				if (indexLabel != null)
				{
				    // ReSharper disable once InconsistentNaming
				    // ReSharper disable once InconsistentNaming
					CompanyMapCollectionClass loadIDs;
					if (this.OffLoadingRadioButton.Checked)
					{
						loadIDs = (CompanyMapCollectionClass)this.Session["Off-LoadIDs"];
					}
					else
					{
						loadIDs = (CompanyMapCollectionClass)this.Session["LoadIDs"];
					}
				    // ReSharper disable once InconsistentNaming
				    // ReSharper disable once InconsistentNaming
					CompanyMapClass loadID = loadIDs[Convert.ToInt32(indexLabel.Text)];

				    // ReSharper disable once InconsistentNaming
					var idTextBox = (TextBox)e.Item.FindControl("LoadIDTextBox");
					loadID.MapID = idTextBox.Text.Trim();
                    loadID.AssignedToID = idTextBox.Text.Trim();

					var driversDropDownList = (DropDownList)e.Item.FindControl("DriverDropDownList");
					loadID.AssignedGuid = Guid.Parse(driversDropDownList.SelectedValue);
					loadID.AssignedID = driversDropDownList.SelectedItem.Text;

					if (loadID.IdentityGuid == Guid.Empty)
					{
						loadID.IdentityGuid = FMChannelHelper.MakeCall<ICompanyMaps, Guid>(
																	 x =>
																	 x.Add(this.Security, loadID)
																);

					}
					else
					{
						FMChannelHelper.MakeCall<ICompanyMaps>(
																	 x =>
																	 x.Modify(this.Security, loadID)
																);
					}

					this.LoadIDDataGrid.EditItemIndex = -1;
					this.AddButton.Enabled = true;
					this.HierarchyTreeView.Enabled = true;
					this.UpdateLoadIDView();
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

        // ReSharper disable once InconsistentNaming
		private void UnassignButton_Command(object sender, CommandEventArgs e)
		{
			try
			{
				TreeNode node = this.HierarchyTreeView.SelectedNode;

				if (node.Parent != null)
				{
					var type = this.GetMapType(node.Depth);

					switch (type)
					{
						case COMPANY_MAP_TYPE.LOAD_OWNER_MANAGER_MAP:
							return;
						case COMPANY_MAP_TYPE.SHIPPER_OWNER_MAP:
							type = COMPANY_MAP_TYPE.LOAD_OWNER_MANAGER_MAP;
							break;
						case COMPANY_MAP_TYPE.BILLTO_SHIPPER_MAP:
							type = COMPANY_MAP_TYPE.SHIPPER_OWNER_MAP;
							break;
						case COMPANY_MAP_TYPE.SHIPTO_BILLTO_MAP:
							type = COMPANY_MAP_TYPE.BILLTO_SHIPPER_MAP;
							break;
						case COMPANY_MAP_TYPE.LOAD_MAX_COMPANY_MAP_TYPE:
							type = COMPANY_MAP_TYPE.SHIPTO_BILLTO_MAP;
							break;
						case COMPANY_MAP_TYPE.OFFLOAD_OWNER_MANAGER_MAP:
							return;
						case COMPANY_MAP_TYPE.SUPPLIER_OWNER_MAP:
							type = COMPANY_MAP_TYPE.OFFLOAD_OWNER_MANAGER_MAP;
							break;
						case COMPANY_MAP_TYPE.OFFLOAD_MAX_COMPANY_MAP_TYPE:
							type = COMPANY_MAP_TYPE.SUPPLIER_OWNER_MAP;
							break;
						default:
							throw new Exception("Invalid Map Type");
					}

					FMChannelHelper.MakeCall<ICompanyMaps>(x => x.Purge(this.Security, Guid.Parse(node.Value), type));

					int index = node.Parent.ChildNodes.IndexOf(node);
					if (index < node.Parent.ChildNodes.Count - 1)
					{
						node.Parent.ChildNodes[index + 1].Selected = true;
					}
					else if (node.Parent.ChildNodes.Count > 1)
					{
						node.Parent.ChildNodes[index - 1].Selected = true;
					}
					else
					{
						node.Parent.Selected = true;
					}
		
					node.Parent.ChildNodes.Remove(node);


					var eventArgs = new EventArgs();
					this.HierarchyTreeView_SelectedNodeChange(this.HierarchyTreeView, eventArgs);
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		private void UpdateLoadIDView()
		{
			this.LoadIDDataGrid.DataSource = this.EnumerateLoadIDs();
			this.LoadIDDataGrid.DataBind();
		}

		#endregion
	}
}