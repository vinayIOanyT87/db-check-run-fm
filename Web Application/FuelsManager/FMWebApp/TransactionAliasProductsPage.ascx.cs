// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TransactionAliasProductsPage.ascx.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the TransactionAliasProductsPage type.
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

	/// <summary>
	///    Summary description for TransactionAliasProductsPage.
	/// </summary>
	public partial class TransactionAliasProductsPage : FMUserControlBase
	{
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
				var TransactionAlias = (TransactionAliasClass)this.Session["TransactionAlias"];

				if (!this.Page.IsPostBack)
				{
					// CSI 5856 - disable buttons if user has no modify right.
					if (!this.Security.HasRight(RIGHT.MODIFY_TRANSACTION_ALIASES))
					{
						this.ExcludeProductsButton.Enabled = false;
						this.UnexcludeProductsButton.Enabled = false;
					}

					// Populate ExcludedProductsListBox
					foreach (ProductMapClass ProductMap in TransactionAlias.ExcludedProductCollection)
					{
						var Item = new ListItem(ProductMap.AssignedID, ProductMap.AssignedGuid.ToString());
						this.ExcludedProductsListBox.Items.Add(Item);
					}

					// Populate the NonexcludedProductsListBox
					var productCollection = FMChannelHelper.MakeCall<IProducts, ProductCollectionClass>(
						x => x.Enumerate(this.Security));

					foreach (ProductClass product in productCollection)
					{
						if (this.ExcludedProductsListBox.Items.FindByText(product.ID) != null)
						{
							continue;
						}

						var Item = new ListItem(product.ID, product.IdentityGuid.ToString());
						this.UnexcludedProductsListBox.Items.Add(Item);
					}
                    this.SetFieldAccessibilityForChildRecordVersion();
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		private void ExcludeProductsButton_Command(object sender, CommandEventArgs e)
		{
			ListItem UnexcludedProductItem;
			while ((UnexcludedProductItem = this.UnexcludedProductsListBox.SelectedItem) != null)
			{
				this.UnexcludedProductsListBox.Items.Remove(UnexcludedProductItem);
				UnexcludedProductItem.Selected = false;

				foreach (ListItem ExcludedProductItem in this.ExcludedProductsListBox.Items)
				{
					if (ExcludedProductItem.Text.CompareTo(UnexcludedProductItem.Text) > 0)
					{
						int idx = this.ExcludedProductsListBox.Items.IndexOf(ExcludedProductItem);
						this.ExcludedProductsListBox.Items.Insert(idx, UnexcludedProductItem);
						UnexcludedProductItem = null;
						break;
					}
				}

				if (UnexcludedProductItem != null)
				{
					this.ExcludedProductsListBox.Items.Add(UnexcludedProductItem);
				}
			}

			this.UpdateExcludedProducts();
		}

		/// <summary>
		///    Required method for Designer support - do not modify
		///    the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.ExcludeProductsButton.Command +=
				new System.Web.UI.WebControls.CommandEventHandler(this.ExcludeProductsButton_Command);
			this.UnexcludeProductsButton.Command +=
				new System.Web.UI.WebControls.CommandEventHandler(this.UnexcludeProductsButton_Command);
		}

		private void UnexcludeProductsButton_Command(object sender, CommandEventArgs e)
		{
			ListItem ExcludedProductItem;
			while ((ExcludedProductItem = this.ExcludedProductsListBox.SelectedItem) != null)
			{
				this.ExcludedProductsListBox.Items.Remove(ExcludedProductItem);
				ExcludedProductItem.Selected = false;

				foreach (ListItem UnexcludedProductItem in this.UnexcludedProductsListBox.Items)
				{
					if (UnexcludedProductItem.Text.CompareTo(ExcludedProductItem.Text) > 0)
					{
						int idx = this.UnexcludedProductsListBox.Items.IndexOf(UnexcludedProductItem);
						this.UnexcludedProductsListBox.Items.Insert(idx, ExcludedProductItem);
						ExcludedProductItem = null;
						break;
					}
				}

				if (ExcludedProductItem != null)
				{
					this.UnexcludedProductsListBox.Items.Add(ExcludedProductItem);
				}
			}

			this.UpdateExcludedProducts();
		}

		private void UpdateExcludedProducts()
		{
			var TransactionAlias = (TransactionAliasClass)this.Session["TransactionAlias"];
			var ExcludedProductCollection = new ProductMapCollectionClass();

			foreach (ListItem ExcludedItem in this.ExcludedProductsListBox.Items)
			{
				var ExcludedProductMap = new ProductMapClass();
				ExcludedProductMap.AssignedGuid = Guid.Parse(ExcludedItem.Value);
				ExcludedProductMap.Type = PRODUCT_MAP_TYPE.TRANSACTION_ALIAS_EXCLUSION_MAP;
				ExcludedProductMap.AssignedID = ExcludedItem.Text; //CSI 4693 Log Id is improperly
				ExcludedProductCollection.Add(ExcludedProductMap);
			}

			TransactionAlias.ExcludedProductCollection = ExcludedProductCollection;
		}


        private void SetFieldAccessibilityForChildRecordVersion()
        {
            var transactionAlias = (TransactionAliasClass)this.Session["TransactionAlias"];
            var versionSpecificFields = this.Session[PageSessionKeyConstants.TRANS_ALIAS_VERSION_SPECIFIC_FIELDS] as List<string>;
            bool currentSiteOwnsRecordVersion = (transactionAlias.SiteGuid == this.Security.SiteGuid);

            if (versionSpecificFields != null && (transactionAlias.IdentityGuid.Equals(Guid.Empty)
                                              || (currentSiteOwnsRecordVersion && transactionAlias.IdentityGuid.Equals(transactionAlias.MasterRecordGuid))))
            {
                return;
            }

            if (versionSpecificFields != null)
            {
                this.ExcludeProductsButton.Enabled = (this.ExcludeProductsButton.Enabled && versionSpecificFields.Contains("Products"));
                this.UnexcludeProductsButton.Enabled = (this.UnexcludeProductsButton.Enabled && versionSpecificFields.Contains("Products"));
                this.ExcludedProductsListBox.Enabled = (this.ExcludedProductsListBox.Enabled && versionSpecificFields.Contains("Products"));
                this.UnexcludedProductsListBox.Enabled = (this.UnexcludedProductsListBox.Enabled && versionSpecificFields.Contains("Products"));
            }
        }
		#endregion
	}
}