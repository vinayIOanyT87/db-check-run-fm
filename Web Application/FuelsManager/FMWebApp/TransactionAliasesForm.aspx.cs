// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TransactionAliasesForm.aspx.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   ENTER FILE SUMMARY HERE
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace FuelsManager.FMWebApp
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Net.Sockets;
	using System.Web.UI.WebControls;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.Interfaces;

	public enum TransactionAliasDefaultsType
	{
		Aviation = 0,
		TerminalAutomation = 1
	}

	public class TransactionAliasDefaultsSelection
	{
		private TransactionAliasDefaultsType defaultSelectionId;
		public string DefaultSelectionName { get; set; }
		
		public TransactionAliasDefaultsType DefaultSelectionId
		{ get => this.defaultSelectionId;
			set {
				this.defaultSelectionId = value;
				this.DefaultSelectionName = value.ToString();
			}
		}
	}

	/// <summary>
	/// Code behind for TransactionAliasesForm.
	/// </summary>
	public partial class TransactionAliasesForm : FMFormBase, IEntityDiscovery, IMenuDiscovery
	{
		#region Explicit Interface Properties
		bool IEntityDiscovery.EntityAssignable
		{
			get { return true; }
		}

		Type IEntityDiscovery.EntityEngineType
		{
			get { return typeof(ITransactionAliases); }
		}

		ENTITY_TYPE IEntityDiscovery.EntityType
		{
			get { return ENTITY_TYPE.TRANSACTION_ALIAS; }
		}
		#endregion

		#region Public Methods and Operators
		/// <summary>
		/// Gets a list of menu items that should be displayed for the current user.
		/// </summary>
		/// <param name="security">
		/// The security object of the current session
		/// </param>
		/// <param name="siteGroup">
		/// Whether the current logged-in site is a site group
		/// </param>
		/// <param name="options">
		/// Hardware key options
		/// </param>
		/// <returns>
		/// List of menu items to be displayed
		/// </returns>
		public List<FMMenuItem> GetMenuItems(SecurityClass security, bool siteGroup, ushort word1,ushort word2, ushort useNewLicenseKey, uint options)
		{
				if (useNewLicenseKey == 1)
				{
					 if ((word1 & 0x10) != 0x10)
						  return null;
				}
				else
				{
					 // Depends Upon Accounting
					 if ((options & 0x80100) == 0)
					 {
						  return null;
					 }
				}

				var items = new List<FMMenuItem>();

			if (!security.HasRight(RIGHT.VIEW_TRANSACTION_ALIASES) && !security.HasRight(RIGHT.MODIFY_TRANSACTION_ALIASES))
			{
				return null;
			}

			items.Add(
				new FMMenuItem
					{
						MenuItemType = FMMenuItemType.CONFIG_ACCOUNTING_TRANSACTION_ALIASES, 
						RootMenuName = "Configuration", 
						CategoryName = "Accounting", 
						ItemName = "Transaction Aliases", 
						NavigateUrl = "TransactionAliasesForm.aspx", 
						ApplyDataDictionary = ApplyDataDictionary.Apply
					});

			return items;
		}
		#endregion

		#region Explicit Interface Methods

		EntityToSiteMapCollectionClass IEntityDiscovery.EnumerateEntityMaps(SecurityClass security, ENTITY_ASSIGNMENT_TYPE entityAssignmentType)
		{
				TransactionAliasCollectionClass transactionAliasCollection = entityAssignmentType == ENTITY_ASSIGNMENT_TYPE.UNDELEGATED
					? FMChannelHelper.MakeCall<ITransactionAliases, TransactionAliasCollectionClass>(x => x.EnumerateUndelegated(security))
					: FMChannelHelper.MakeCall<ITransactionAliases, TransactionAliasCollectionClass>(x => x.Enumerate(security));
			var entityToSiteMapCollection = new EntityToSiteMapCollectionClass();

			foreach (TransactionAliasClass transactionAlias in transactionAliasCollection)
			{
				if (entityAssignmentType == ENTITY_ASSIGNMENT_TYPE.ASSIGNED)
				{
					if (security.SiteGuid == transactionAlias.SiteGuid)
					{
						continue;
					}

					if (security.LoginSiteGuid != transactionAlias.SiteGuid)
					{
						continue;
					}
				}
				else
				{
						  //For entity types supporting Record Versioning, assignments can be cascaded, irrespective of whether Record Versioning is turned on or off.
					if (security.SiteGuid != transactionAlias.SiteGuid 
						&& security.SiteGuid != transactionAlias.AssignedToSiteGuid)
						{
								continue;
						}
				}

				// The EntityToSiteMap references TransactionAlias records by their MasterRecordGuids instead of their actual TransactionAliasGuids.
				var entityToSiteMap = new EntityToSiteMapClass(transactionAlias)
											{
												IdentityGuid = transactionAlias.MasterRecordGuid
											};

				entityToSiteMapCollection.Add(entityToSiteMap);
			}

			return entityToSiteMapCollection;
		}

		Guid IEntityDiscovery.GetIdentityGuid(SecurityClass security, string id)
		{
			return FMChannelHelper.MakeCall<ITransactionAliases, Guid>(x => x.GetIdentityGuid(security, id));
		}

		void IEntityDiscovery.SetSiteGuid(SecurityClass security, Guid guid, Guid siteGuid)
		{
			FMChannelHelper.MakeCall<ITransactionAliases>(
				aliases =>
					{
						TransactionAliasClass transactionAlias = aliases.Get( security, guid, false );
						transactionAlias.SiteGuid = siteGuid;
						aliases.Modify( security, transactionAlias );
					});
		}
		#endregion

		#region Methods

		/// <summary>
		/// Raises the <see cref="OnInit" /> event.
		/// </summary>
		/// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
		protected override void OnInit(EventArgs e)
		{
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			this.InitializeComponent();
			base.OnInit(e);
		}

        protected void PageSizeDropDown_SelectedIndexChanged(object source, EventArgs e)
        {
            this.UpdateView();
        }

        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        protected void Page_Load(object sender, EventArgs e)
		{
			try
			{
				this.GetSecurity();
				string useNewScreenStr = FMChannelHelper.MakeCall<IConfigurationSettings, string>(
						x => x.GetKeyValueByKey(this.Security, ConfigurationSettingDOClass.Key_UseNewTransactionAliasScreen));

				AddButton.Attributes.Add("data-editormode", useNewScreenStr);
				if (!this.Page.IsPostBack)
				{
					if (this.Security.HasRight(RIGHT.MODIFY_TRANSACTION_ALIASES) == false)
					{
						this.AddButton.Enabled = false;
						this.AddButton2.Enabled = false;
						this.CreateDefaultButton.Enabled = false;
					}

					if (this.Session["TransactionAliasesPage"] != null)
					{
						this.AliasesDataGrid.CurrentPageIndex = (int)this.Session["TransactionAliasesPage"];
						this.Session.Remove("TransactionAliasesPage");
					}

					this.UpdateView();
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		/// <summary>
		/// Handles the Command event of the AddButton control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="CommandEventArgs" /> instance containing the event data.</param>
		private void AddButtonCommand(object sender, CommandEventArgs e)
		{

			this.Session.Remove("IdentityGuid");
			this.Session["TransactionAliasesPage"] = this.AliasesDataGrid.CurrentPageIndex;
			this.Session.Remove("TransactionAlias");

			this.Redirect("TransactionAliasForm.aspx");

		}

		/// <summary>
		/// This method will handle the delete event from the transaction alias grid.
		/// </summary>
		/// <param name="source">
		/// </param>
		/// <param name="e">
		/// </param>
		private void AliasesDataGridDeleteCommand(object source, DataGridCommandEventArgs e)
		{
			try
			{
				// Get guid
				TableCell identityGuidCell = e.Item.Cells[2];//bds

				FMChannelHelper.MakeCall<ITransactionAliases>(x => x.Purge(this.Security, Guid.Parse(identityGuidCell.Text)));

				try
				{
					if (UsingLoadRack)
					{
						ILoadRackManager loadRackManager = this.GetLoadRackManager();
						loadRackManager.Purge(this.Security, typeof(TransactionAliasClass), Guid.Parse(identityGuidCell.Text));
					}
				}
				catch (SocketException socketExcept)
				{
					if (socketExcept.ErrorCode != 10061)
					{
						throw socketExcept;
					}
				}

				this.AliasesDataGrid.SelectedIndex = -1;
				this.Session.Remove("IdentityGuid");

				if (this.AliasesDataGrid.Items.Count == 1 && this.AliasesDataGrid.CurrentPageIndex > 0)
				{
					this.AliasesDataGrid.CurrentPageIndex--;
				}

				this.UpdateView();
				this.RefreshLeftTreeView();
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		private void AliasesDataGridEditCommand(object source, DataGridCommandEventArgs e)
		{
			TableCell identityGuidCell = e.Item.Cells[2];//bds
			this.Session["IdentityGuid"] = identityGuidCell.Text;
			this.Session["TransactionAliasesPage"] = this.AliasesDataGrid.CurrentPageIndex;
			this.Session.Remove("TransactionAlias");

			string useNewScreenStr = FMChannelHelper.MakeCall<IConfigurationSettings, string>(
					x => x.GetKeyValueByKey(this.Security, ConfigurationSettingDOClass.Key_UseNewTransactionAliasScreen));

			if (string.IsNullOrEmpty(useNewScreenStr) == false && Int32.Parse( useNewScreenStr ) == 1)
			{
				this.Redirect("../MenuBar/FMMenuBar.aspx?target=../AccountingArea/TransactionAlias/TransactionAliasDetail/" + identityGuidCell.Text);
			} else {
				this.Redirect("TransactionAliasForm.aspx");
			}
		}

		private void AliasesDataGridItemDataBound(object sender, DataGridItemEventArgs e)
		{
			var deleteButton = (LinkButton)e.Item.FindControl("DeleteButton");

			if (deleteButton != null)
			{
				TableCell siteGuidCell = e.Item.Cells[1];//bds

				if (!this.Security.HasRight(RIGHT.MODIFY_TRANSACTION_ALIASES)
					 || this.Security.SiteGuid != Guid.Parse(siteGuidCell.Text))
				{
					deleteButton.Enabled = false;
				}
					 //Child record versions cannot be created or deleted directly. Their lifetime is controlled by the Entity-To-Site assignment only.
					 if (deleteButton.Enabled)
					 {
						  int index = this.AliasesDataGrid.CurrentPageIndex * this.AliasesDataGrid.PageSize + e.Item.ItemIndex;

					var lstTransAlias = (List<TransactionAliasNameClass>)this.AliasesDataGrid.DataSource;
						 
					if (lstTransAlias[index].IdentityGuid != lstTransAlias[index].MasterRecordGuid)
						 {
							 deleteButton.Enabled = false;
						 }
					 }
			}
		}

		private void AliasesDataGridPageIndexChanged(object source, DataGridPageChangedEventArgs e)
		{
			// if we are editing do not allow a page change
			if (this.AliasesDataGrid.EditItemIndex > -1)
			{
				return;
			}

			this.AliasesDataGrid.CurrentPageIndex = e.NewPageIndex;
			this.UpdateView();
		}

		private void CreateDefaultButtonCommand(object sender, CommandEventArgs e)
		{
			try
			{
				TransactionAliasDefaultsType defaultsSelection;

				if (!Enum.IsDefined(typeof(TransactionAliasDefaultsType), Request["__EVENTARGUMENT"]))
				{
					// Transaction Alias defaults selection not given or not supported
					return;
				}

				defaultsSelection = (TransactionAliasDefaultsType)Enum.Parse(typeof(TransactionAliasDefaultsType), Request["__EVENTARGUMENT"]);

				var transactionAlias = new TransactionAliasClass();

				// By default, all statuses are assigned to new aliases
				transactionAlias.AssignedStatuses.AddRange(Enum.GetValues(typeof(TransactionStatus)).Cast<int>().ToList());

				switch (defaultsSelection)
				{
					case TransactionAliasDefaultsType.Aviation:
						FMChannelHelper.MakeCall<ITransactionAliases>(
							transactionAliases =>
							{
								transactionAlias.ID = "Adjustment";
								if (transactionAliases.GetIdentityGuid(this.Security, transactionAlias.ID) == Guid.Empty)
								{
									transactionAlias.TransTypeID = TransactionTypes.T1_PrimaryAdjustment;
									transactionAlias.LookupDefaultStatusIndex = 0;
									transactionAlias.SetEquipmentTypes(true, 1, new EQUIPMENT_TYPE[] { });
									transactionAlias.SetEquipmentTypes(true, 2, new EQUIPMENT_TYPE[] { });
									transactionAliases.Add(this.Security, transactionAlias);
								}
								transactionAlias.ID = "24 Hour Closeout";
								if (transactionAliases.GetIdentityGuid(this.Security, transactionAlias.ID) == Guid.Empty)
								{
									transactionAlias.TransTypeID = TransactionTypes.T12_InventoryNotAffected;
									transactionAlias.MeterCloseout = true;
									transactionAlias.LookupDefaultStatusIndex = 0;
									transactionAliases.Add(this.Security, transactionAlias);
								}
								transactionAlias.ID = "Physical Inventory";
								if (transactionAliases.GetIdentityGuid(this.Security, transactionAlias.ID) == Guid.Empty)
								{
									transactionAlias.TransTypeID = TransactionTypes.T14_PhysicalInventory;
									transactionAlias.LookupDefaultStatusIndex = 0;
									transactionAliases.Add(this.Security, transactionAlias);
								}
								transactionAlias.ID = "Receipt";
								if (transactionAliases.GetIdentityGuid(this.Security, transactionAlias.ID) == Guid.Empty)
								{
									transactionAlias.TransTypeID = TransactionTypes.T8_Receipt;
									transactionAlias.LookupDefaultStatusIndex = 0;
									transactionAliases.Add(this.Security, transactionAlias);
								}
								transactionAlias.ID = "Transfer";
								if (transactionAliases.GetIdentityGuid(this.Security, transactionAlias.ID) == Guid.Empty)
								{
									transactionAlias.TransTypeID = TransactionTypes.T13_OwnerTransfer;
									transactionAlias.LookupDefaultStatusIndex = 0;
									transactionAliases.Add(this.Security, transactionAlias);
								}
								transactionAlias.ID = "Defuel";
								if (transactionAliases.GetIdentityGuid(this.Security, transactionAlias.ID) == Guid.Empty)
								{
									transactionAlias.TransTypeID = TransactionTypes.T4_SecondaryDefuel;
									transactionAlias.MultipleLineItems = false;
									transactionAlias.SetEquipmentTypes(true, 1, new EQUIPMENT_TYPE[] { });
									transactionAlias.SetEquipmentTypes(true, 2, new EQUIPMENT_TYPE[] { });
									transactionAliases.Add(this.Security, transactionAlias);
								}
								transactionAlias.ID = "LR Receipt";
								if (transactionAliases.GetIdentityGuid(this.Security, transactionAlias.ID) == Guid.Empty)
								{
									transactionAlias.TransTypeID = TransactionTypes.T4_SecondaryDefuel;
									transactionAlias.MultipleLineItems = false;
									transactionAlias.SetEquipmentTypes(true, 1, new EQUIPMENT_TYPE[] { });
									transactionAlias.SetEquipmentTypes(true, 2, new EQUIPMENT_TYPE[] { });
									transactionAliases.Add(this.Security, transactionAlias);
								}
								transactionAlias.ID = "Bulk Issue";
								if (transactionAliases.GetIdentityGuid(this.Security, transactionAlias.ID) == Guid.Empty)
								{
									transactionAlias.TransTypeID = TransactionTypes.T5_PrimaryDisbursement;
									transactionAliases.Add(this.Security, transactionAlias);
								}
								transactionAlias.ID = "Issue";
								if (transactionAliases.GetIdentityGuid(this.Security, transactionAlias.ID) == Guid.Empty)
								{
									transactionAlias.TransTypeID = TransactionTypes.T5_PrimaryDisbursement;
									transactionAliases.Add(this.Security, transactionAlias);
								}
								transactionAlias.ID = "Load Rack";
								if (transactionAliases.GetIdentityGuid(this.Security, transactionAlias.ID) == Guid.Empty)
								{
									transactionAlias.TransTypeID = TransactionTypes.T7_FillStand;
									transactionAliases.Add(this.Security, transactionAlias);
								}
								transactionAlias.ID = "Rotation";
								if (transactionAliases.GetIdentityGuid(this.Security, transactionAlias.ID) == Guid.Empty)
								{
									transactionAlias.TransTypeID = TransactionTypes.T12_InventoryNotAffected;
									transactionAliases.Add(this.Security, transactionAlias);
								}
							});
						break;
					case TransactionAliasDefaultsType.TerminalAutomation:
						FMChannelHelper.MakeCall<ITransactionAliases>(
							transactionAliases =>
							{
								transactionAlias.ID = "Adjustment";
								if (transactionAliases.GetIdentityGuid(this.Security, transactionAlias.ID) == Guid.Empty)
								{
									transactionAlias.TransTypeID = TransactionTypes.T1_PrimaryAdjustment;
									transactionAlias.LookupDefaultStatusIndex = 0;
									transactionAlias.SetEquipmentTypes(true, 1, new EQUIPMENT_TYPE[] { });
									transactionAlias.SetEquipmentTypes(true, 2, new EQUIPMENT_TYPE[] { });
									transactionAliases.Add(this.Security, transactionAlias);
								}

								transactionAlias.ID = "BOL";
								if (transactionAliases.GetIdentityGuid(this.Security, transactionAlias.ID) == Guid.Empty)
								{
									transactionAlias.TransTypeID = TransactionTypes.T5_PrimaryDisbursement;
									transactionAlias.ShowCompanyName = TRANSACTION_SHOW_COMPANY_NAME.SHOW_NAME_AND_ID;
									transactionAlias.LimitSelectionsBasedOnHierarchy = true;
									transactionAlias.LookupDefaultStatusIndex = 0;
									transactionAliases.Add(this.Security, transactionAlias);
								}

								transactionAlias.ID = "Meter Closeout";
								if (transactionAliases.GetIdentityGuid(this.Security, transactionAlias.ID) == Guid.Empty)
								{
									transactionAlias.TransTypeID = TransactionTypes.T12_InventoryNotAffected;
									transactionAlias.MeterCloseout = true;
									transactionAlias.LookupDefaultStatusIndex = 0;
									transactionAliases.Add(this.Security, transactionAlias);
								}

								transactionAlias.ID = "Physical Inventory";
								if (transactionAliases.GetIdentityGuid(this.Security, transactionAlias.ID) == Guid.Empty)
								{
									transactionAlias.TransTypeID = TransactionTypes.T14_PhysicalInventory;
									transactionAlias.LookupDefaultStatusIndex = 0;
									transactionAliases.Add(this.Security, transactionAlias);
								}

								transactionAlias.ID = "Receipt";
								if (transactionAliases.GetIdentityGuid(this.Security, transactionAlias.ID) == Guid.Empty)
								{
									transactionAlias.TransTypeID = TransactionTypes.T8_Receipt;
									transactionAlias.LookupDefaultStatusIndex = 0;
									transactionAliases.Add(this.Security, transactionAlias);
								}

								transactionAlias.ID = "Regrade";
								if (transactionAliases.GetIdentityGuid(this.Security, transactionAlias.ID) == Guid.Empty)
								{
									transactionAlias.TransTypeID = TransactionTypes.T15_PrimaryRegrade;
									transactionAlias.LookupDefaultStatusIndex = 0;
									transactionAliases.Add(this.Security, transactionAlias);
								}

								transactionAlias.ID = "Shipment";
								if (transactionAliases.GetIdentityGuid(this.Security, transactionAlias.ID) == Guid.Empty)
								{
									transactionAlias.TransTypeID = TransactionTypes.T6_SecondaryDisbursement;
									transactionAlias.DistributedImpact = true;
									transactionAlias.LookupDefaultStatusIndex = 0;
									transactionAliases.Add(this.Security, transactionAlias);
								}

								transactionAlias.ID = "Supply Order";
								if (transactionAliases.GetIdentityGuid(this.Security, transactionAlias.ID) == Guid.Empty)
								{
									transactionAlias.TransTypeID = TransactionTypes.T18_SupplyOrder;
									transactionAlias.LookupDefaultStatusIndex = 0;
									transactionAliases.Add(this.Security, transactionAlias);
								}

								transactionAlias.ID = "Transfer";
								if (transactionAliases.GetIdentityGuid(this.Security, transactionAlias.ID) == Guid.Empty)
								{
									transactionAlias.TransTypeID = TransactionTypes.T13_OwnerTransfer;
									transactionAlias.LookupDefaultStatusIndex = 0;
									transactionAliases.Add(this.Security, transactionAlias);
								}
								transactionAlias.ID = "LR Receipt";
								if (transactionAliases.GetIdentityGuid(this.Security, transactionAlias.ID) == Guid.Empty)
								{
									transactionAlias.TransTypeID = TransactionTypes.T4_SecondaryDefuel;
									transactionAlias.MultipleLineItems = false;
									transactionAlias.SetEquipmentTypes(true, 1, new EQUIPMENT_TYPE[] { });
									transactionAlias.SetEquipmentTypes(true, 2, new EQUIPMENT_TYPE[] { });
									transactionAliases.Add(this.Security, transactionAlias);
								}
								transactionAlias.ID = "Bulk Issue";
								if (transactionAliases.GetIdentityGuid(this.Security, transactionAlias.ID) == Guid.Empty)
								{
									transactionAlias.TransTypeID = TransactionTypes.T5_PrimaryDisbursement;
									transactionAliases.Add(this.Security, transactionAlias);
								}
								transactionAlias.ID = "Issue";
								if (transactionAliases.GetIdentityGuid(this.Security, transactionAlias.ID) == Guid.Empty)
								{
									transactionAlias.TransTypeID = TransactionTypes.T6_SecondaryDisbursement;
									transactionAliases.Add(this.Security, transactionAlias);
								}
								transactionAlias.ID = "Load Rack";
								if (transactionAliases.GetIdentityGuid(this.Security, transactionAlias.ID) == Guid.Empty)
								{
									transactionAlias.TransTypeID = TransactionTypes.T7_FillStand;
									transactionAliases.Add(this.Security, transactionAlias);
								}
								transactionAlias.ID = "Rotation";
								if (transactionAliases.GetIdentityGuid(this.Security, transactionAlias.ID) == Guid.Empty)
								{
									transactionAlias.TransTypeID = TransactionTypes.T12_InventoryNotAffected;
									transactionAliases.Add(this.Security, transactionAlias);
								}
								transactionAlias.ID = "Order";
								if (transactionAliases.GetIdentityGuid(this.Security, transactionAlias.ID) == Guid.Empty)
								{
									transactionAlias.TransTypeID = TransactionTypes.T17_Order;
									transactionAliases.Add(this.Security, transactionAlias);
								}
								transactionAlias.ID = "Accounts Payable Invoice";
								if (transactionAliases.GetIdentityGuid(this.Security, transactionAlias.ID) == Guid.Empty)
								{
									transactionAlias.TransTypeID = TransactionTypes.T21_AccountPayableInvoice;
									transactionAliases.Add(this.Security, transactionAlias);
								}
								transactionAlias.ID = "Accounts Receivable Invoice";
								if (transactionAliases.GetIdentityGuid(this.Security, transactionAlias.ID) == Guid.Empty)
								{
									transactionAlias.TransTypeID = TransactionTypes.T22_AccountReceivableInvoice;
									transactionAliases.Add(this.Security, transactionAlias);
								}
							});
						break;
				}

				this.UpdateView();
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		/// <summary>
		///     Required method for Designer support - do not modify
		///     the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.AddButton2.Command					+= this.AddButtonCommand;
			this.AliasesDataGrid.EditCommand		+= this.AliasesDataGridEditCommand;
			this.AliasesDataGrid.PageIndexChanged	+= this.AliasesDataGridPageIndexChanged;
			this.AliasesDataGrid.DeleteCommand		+= this.AliasesDataGridDeleteCommand;
			this.AliasesDataGrid.ItemDataBound		+= this.AliasesDataGridItemDataBound;
			this.AddButton.Command					+= this.AddButtonCommand;
			this.CreateDefaultButton.Command		+= this.CreateDefaultButtonCommand;
		}

		/// <summary>
		///     This method will refresh the left tree view. It is called by the delete command.
		/// </summary>
		private void RefreshLeftTreeView()
		{
			this.ucFMMenuBar.Refresh();
		}

		/// <summary>
		/// Updates the view.
		/// </summary>
		private void UpdateView()
		{
			List<TransactionAliasNameClass> aliasesNames = 
				FMChannelHelper.MakeCall<ITransactionAliases, List<TransactionAliasNameClass>>(
							transactionAliases => transactionAliases.EnumerateNamesOnly(this.Security, false));
            
			this.AliasesFormPageSizeDropDown.SetPageSize(this.AliasesDataGrid, aliasesNames.Count);

            this.AliasesDataGrid.DataSource = aliasesNames;
			this.AliasesDataGrid.DataBind();
		}
		#endregion
	}
}