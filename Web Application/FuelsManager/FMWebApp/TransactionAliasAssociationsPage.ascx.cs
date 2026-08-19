// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TransactionAliasAssociationsPage.ascx.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the TransactionAliasAssociationsPage type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FuelsManager.FMWebApp
{
	using System;
	using System.Collections.Generic;
	using System.Web.UI.WebControls;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.Constants;
	using FMBusinessObjects.DataObjects;

	using global::FMWebApp;

	public partial class TransactionAliasAssociationsPage : FMUserControlBase
	{
		#region Constants and Fields
		protected TransactionAliasClass alias;
		#endregion

		#region Methods
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
				this.alias = (TransactionAliasClass)this.Session["TransactionAlias"];

				if (!this.IsPostBack)
				{
					// Enable/Disable associate/disassociate buttons based on rights
					if (!this.Security.HasRight(RIGHT.MODIFY_TRANSACTION_ALIASES))
					{
						this.btnAssign.Enabled = false;
						this.btnUnassign.Enabled = false;
						this.chkAggregate.Enabled = false;
					}
					else
					{
						// vthompson 9/24/2008
						// Only Supply Orders allow for aggregating transactions
						this.chkAggregate.Enabled = (this.alias._TransTypeID == TransactionTypes.T18_SupplyOrder
						                             || this.alias._TransTypeID == TransactionTypes.T21_AccountPayableInvoice
						                             || this.alias._TransTypeID == TransactionTypes.T22_AccountReceivableInvoice
						                             || this.alias._TransTypeID == TransactionTypes.T9_Request);

						if (FMChannelHelper.MakeCall<IHardwareKey, bool>(x => x.IsADFKey()))
						{
							this.chkAggregate.Enabled = this.alias._TransTypeID == TransactionTypes.T8_Receipt;
						}
					}

					this.chkQtyToleranceWarning.Checked = (this.alias._TransTypeID == TransactionTypes.T18_SupplyOrder);
					this.chkTotalQtyWarning.Checked = (this.alias._TransTypeID == TransactionTypes.T18_SupplyOrder);
					this.chkTotalValueWarning.Checked = (this.alias._TransTypeID == TransactionTypes.T18_SupplyOrder);
					this.chkValueToleranceWarning.Checked = (this.alias._TransTypeID == TransactionTypes.T18_SupplyOrder);

					this.PopulateListBoxes();
					this.chkAggregate.Checked = this.alias.AggregateAssociatedTransactions;
					this.chkQtyToleranceWarning.Checked = this.alias.EnableQtyToleranceExceededWarning;
					this.chkTotalQtyWarning.Checked = this.alias.EnableTotalQtyExceededWarning;
					this.chkTotalValueWarning.Checked = this.alias.EnableTotalValueExceededWarning;
					this.chkValueToleranceWarning.Checked = this.alias.EnableValueToleranceExceededWarning;
				}
				else
				{
					this.alias.AggregateAssociatedTransactions = this.chkAggregate.Checked;
					this.alias.EnableQtyToleranceExceededWarning = this.chkQtyToleranceWarning.Checked;
					this.alias.EnableTotalQtyExceededWarning = this.chkTotalQtyWarning.Checked;
					this.alias.EnableTotalValueExceededWarning = this.chkTotalValueWarning.Checked;
					this.alias.EnableValueToleranceExceededWarning = this.chkValueToleranceWarning.Checked;
				}

				if (this.alias._TransTypeID == TransactionTypes.T21_AccountPayableInvoice
				    || this.alias._TransTypeID == TransactionTypes.T22_AccountReceivableInvoice
				    || this.alias._TransTypeID == TransactionTypes.T9_Request)
				{
					this.chkQtyToleranceWarning.Enabled = false;
					this.chkTotalQtyWarning.Enabled = false;
					this.chkTotalValueWarning.Enabled = false;
					this.chkValueToleranceWarning.Enabled = false;
				}

                this.SetFieldAccessibilityForChildRecordVersion();
			}
			catch (Exception ex)
			{
				this.ErrorHandler(ex);
			}
		}

		protected void BtnAssignClick(object sender, EventArgs e)
		{
			// Get all the items selected from the available aliases list box
			var selectedItems = new ListItemCollection();

			foreach (ListItem li in this.lbxAvailable.Items)
			{
				if (li.Selected)
				{
					selectedItems.Add(li);
				}
			}

			// If none are selected return
			if (selectedItems.Count == 0)
			{
				return;
			}

			// Add each of the selected aliases to the associated transactions collection
			FMChannelHelper.MakeCall<ITransactionAliases>(aliases =>
					{
						foreach (ListItem li in selectedItems)
						{
							// Get the selected alias
							Guid aliasGuid = Guid.Parse(li.Value);
							TransactionAliasClass selectedAlias = aliases.Get(this.Security, aliasGuid, false);
							this.alias.AssociatedAliases.Add(selectedAlias);
						}
					});

			// Repopulate the list boxes
			this.PopulateListBoxes();
		}

		protected void BtnUnassignClick(object sender, EventArgs e)
		{
			// Get all the items selected from the associated aliases list box
			var selected = new ListItemCollection();
			foreach (ListItem li in this.lbxAssociated.Items)
			{
				if (li.Selected)
				{
					selected.Add(li);
				}
			}

			// If none are selected return
			if (selected.Count == 0)
			{
				return;
			}

			foreach (ListItem li in selected)
			{
				// Remove the item from the transaction alias
				foreach (TransactionAliasClass associated in this.alias.AssociatedAliases)
				{
					if (li.Value == associated.IdentityGuid.ToString())
					{
						this.alias.AssociatedAliases.Remove(associated);
						break;
					}
				}
			}

			// Repopulate the list boxes
			this.PopulateListBoxes();
		}

		/// <summary>
		///    Required method for Designer support - do not modify
		///    the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
		}

		private void PopulateListBoxes()
		{
			// Populate the assigned box first
			this.lbxAssociated.Items.Clear();

			foreach (TransactionAliasClass transAlias in this.alias.AssociatedAliases)
			{
				var li = new ListItem(transAlias.ID, transAlias.IdentityGuid.ToString());
				this.lbxAssociated.Items.Add(li);
			}

			// Populate the unassigned box
			this.lbxAvailable.Items.Clear();

			List<TransactionAliasNameClass> aliases =
				FMChannelHelper.MakeCall<ITransactionAliases, List<TransactionAliasNameClass>>(
					transactionAliases => transactionAliases.EnumerateNamesOnly(this.Security, false));

			foreach (TransactionAliasNameClass transactionAliasName in aliases)
			{
				bool contains = false;

				foreach (TransactionAliasClass associated in this.alias.AssociatedAliases)
				{
					if (transactionAliasName.IdentityGuid == associated.IdentityGuid)
					{
						contains = true;
						break;
					}
				}

				// If the alias was not found in the associated alias collection
				// add it to the list of available aliases
				if (!contains)
				{
					// Aliases cannot be associated with themselves so don't add
					// the alias that's being edited
					if (transactionAliasName.AliasName == this.alias.ID)
					{
						continue;
					}

					var li = new ListItem(transactionAliasName.AliasName, transactionAliasName.IdentityGuid.ToString());
					this.lbxAvailable.Items.Add(li);
				}
			}
		}


        private void SetFieldAccessibilityForChildRecordVersion()
        {
            var transactionAlias = (TransactionAliasClass)this.Session["TransactionAlias"];
            var versionSpecificFields = this.Session[PageSessionKeyConstants.TRANS_ALIAS_VERSION_SPECIFIC_FIELDS] as List<string>;
            bool currentSiteOwnsRecordVersion = (transactionAlias.SiteGuid == this.Security.SiteGuid);
           
			if (transactionAlias == null
				|| (transactionAlias.IdentityGuid.Equals(Guid.Empty)
                || (currentSiteOwnsRecordVersion && transactionAlias.IdentityGuid.Equals(transactionAlias.MasterRecordGuid))
                || (versionSpecificFields == null)))
            {
                return;
            }

            this.btnAssign.Enabled = (this.btnAssign.Enabled && versionSpecificFields.Contains("Associations"));
            this.btnUnassign.Enabled = (this.btnUnassign.Enabled && versionSpecificFields.Contains("Associations"));
            this.chkAggregate.Enabled = (this.chkAggregate.Enabled && versionSpecificFields.Contains("AggregateAssocTrans"));
            this.chkTotalQtyWarning.Enabled = (this.chkTotalQtyWarning.Enabled && versionSpecificFields.Contains("EnableTotalQuantityExceededWarning"));
            this.chkTotalValueWarning.Enabled = (this.chkTotalValueWarning.Enabled && versionSpecificFields.Contains("EnableTotalValueExceededWarning"));
            this.chkQtyToleranceWarning.Enabled = (this.chkQtyToleranceWarning.Enabled && versionSpecificFields.Contains("EnableQuantityToleranceExceededWarning"));
            this.chkValueToleranceWarning.Enabled = (this.chkValueToleranceWarning.Enabled && versionSpecificFields.Contains("EnableValueToleranceExceededWarning"));
        }
		#endregion
	}
}