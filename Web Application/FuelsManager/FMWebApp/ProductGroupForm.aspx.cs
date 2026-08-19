// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProductGroupForm.aspx.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the ProductGroupForm.aspx.cs type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FuelsManager.FMWebApp
{
	using System;
	using System.Globalization;
	using System.Web.UI.WebControls;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;

	using global::FMWebApp;

	/// <summary>
	///    Summary description for ProductGroupForm.
	/// </summary>
	public partial class ProductGroupForm : FMAutoSubmitFormBase
	{
		#region Public Methods and Operators

		public void UpdateData()
		{
			var productGroup = (ProductGroupClass)this.Session["ProductGroup"];

			ApplicationStringMapCollectionClass messageCollection = null;
			var stringMapType = STRING_MAP_TYPE.MAX_STRING_MAP_TYPE;

			if (Convert.ToInt32(this.TypeDropDownList.SelectedValue) == (int)STRING_TYPE.ENTRY_MESSAGE)
			{
				stringMapType = STRING_MAP_TYPE.ENTRY_MESSAGE;
				messageCollection = productGroup.EntryMessageCollection;
			}
			else if (Convert.ToInt32(this.TypeDropDownList.SelectedValue) == (int)STRING_TYPE.EXIT_MESSAGE)
			{
				stringMapType = STRING_MAP_TYPE.EXIT_MESSAGE;
				messageCollection = productGroup.ExitMessageCollection;
			}

			if (messageCollection != null)
			{
				messageCollection.Clear();

				for (int itemInt = 0; itemInt < this.AssignedMessagesListBox.Items.Count; itemInt++)
				{
					var message = new ApplicationStringMapClass
					              {
						              AssignedToGuid = productGroup.IdentityGuid,
						              ApplicationStringGuid = Guid.Parse(this.AssignedMessagesListBox.Items[itemInt].Value),
						              ID = this.AssignedMessagesListBox.Items[itemInt].Text,
						              Sequence = itemInt,
						              Type = stringMapType
					              };

					messageCollection.Add(message);
				}
			}
		}
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
				this.GetSecurity();

				if (!this.Page.IsPostBack)
				{
					ProductGroupClass productGroup;
					var identityGuid = this.Session["IdentityGuid"] as string;

					// Get IdentityGuid
					if (identityGuid != null)
					{
						// Get ProductGroup
						productGroup = FMChannelHelper.MakeCall<IProductGroups, ProductGroupClass>(
																	 x =>
																	 x.Get(this.Security, Guid.Parse(identityGuid))
																);
					}
					else
					{
						productGroup = new ProductGroupClass();
					}

					this.Session["ProductGroup"] = productGroup;
					this.Name.Text = productGroup.ID;

					// Populate AssignedProductsListBox
					foreach (ProductMapClass productMap in productGroup.ProductMapCollection)
					{
						var unassignedProductItem = new ListItem(productMap.AssignedID, productMap.AssignedGuid.ToString());

						foreach (ListItem assignedProductItem in this.AssignedProductsListBox.Items)
						{
							if (String.Compare(assignedProductItem.Text, unassignedProductItem.Text, StringComparison.Ordinal) > 0)
							{
								int index = this.AssignedProductsListBox.Items.IndexOf(assignedProductItem);
								this.AssignedProductsListBox.Items.Insert(index, unassignedProductItem);
								unassignedProductItem = null;
								break;
							}
						}

						if (unassignedProductItem != null)
						{
							this.AssignedProductsListBox.Items.Add(unassignedProductItem);
						}
					}

					// Populate UnassignedProductsListBox
					ProductCollectionClass productCollection = FMChannelHelper.MakeCall<IProducts, ProductCollectionClass>(
																	 x =>
																	 x.Enumerate(this.Security)
																);

					foreach (ProductClass product in productCollection)
					{
						if (product.ProductType == ProductType.AdditiveProduct)
						{
							continue;
						}

						if (null == this.AssignedProductsListBox.Items.FindByValue(product.MasterRecordGuid.ToString()))
						{
							var assignedProductItem = new ListItem(product.ID, product.MasterRecordGuid.ToString());

							foreach (ListItem unassignedProductItem in this.UnassignedProductsListBox.Items)
							{
								if (String.Compare(unassignedProductItem.Text, assignedProductItem.Text, StringComparison.Ordinal) > 0)
								{
									int index = this.UnassignedProductsListBox.Items.IndexOf(unassignedProductItem);
									this.UnassignedProductsListBox.Items.Insert(index, assignedProductItem);
									assignedProductItem = null;
									break;
								}
							}

							if (assignedProductItem != null)
							{
								this.UnassignedProductsListBox.Items.Add(assignedProductItem);
							}
						}
					}

					if (!this.Security.HasRight(RIGHT.MODIFY_PRODUCTS)
					    || (this.Security.SiteGuid != productGroup.SiteGuid && productGroup.IdentityGuid != Guid.Empty))
					{
						this.OK.Enabled = false;
						this.AssignMessagesButton.Enabled = false;
						this.UnassignMessagesButton.Enabled = false;
						this.AssignProductsButton.Enabled = false;
						this.UnassignProductsButton.Enabled = false;
						this.UpButton.Enabled = false;
						this.DownButton.Enabled = false;
					}

					//Set the title label with a key field from the bound object appended
					this.ProductGroupTitleLabel.Text = this.GetTitleLabelText(this.ProductGroupTitleLabel.Text, productGroup.ID);

					STRING_TYPE[] stringTypes = { STRING_TYPE.ENTRY_MESSAGE, STRING_TYPE.EXIT_MESSAGE };
					var applicationString = new ApplicationStringClass();
					
					foreach (STRING_TYPE stringType in stringTypes)
					{
						applicationString.Type = stringType;
						string entityTypeID = EntityToSiteMapClass.GetEntityTypeID(applicationString.EntityType);
						this.TypeDropDownList.Items.Add(new ListItem(entityTypeID, 
																	((int)applicationString.Type).ToString(CultureInfo.InvariantCulture)));
					}

					this.TypeDropDownListSelectedIndexChanged(null, null);
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		protected void TypeDropDownListSelectedIndexChanged(object sender, EventArgs e)
		{
			var productGroup = (ProductGroupClass)this.Session["ProductGroup"];

			ApplicationStringMapCollectionClass messageCollection = null;
			var stringType = STRING_TYPE.MAX_STRING_TYPE;

			if (Convert.ToInt32(this.TypeDropDownList.SelectedValue) == (int)STRING_TYPE.ENTRY_MESSAGE)
			{
				stringType = STRING_TYPE.ENTRY_MESSAGE;
				messageCollection = productGroup.EntryMessageCollection;
			}
			else if (Convert.ToInt32(this.TypeDropDownList.SelectedValue) == (int)STRING_TYPE.EXIT_MESSAGE)
			{
				stringType = STRING_TYPE.EXIT_MESSAGE;
				messageCollection = productGroup.ExitMessageCollection;
			}

			// Populate the AssignedMessagesListBox
			this.AssignedMessagesListBox.Items.Clear();

			if (messageCollection != null)
			{
				foreach (ApplicationStringMapClass message in messageCollection)
				{
					this.AssignedMessagesListBox.Items.Add(new ListItem(message.ID, message.ApplicationStringGuid.ToString()));
				}
			}

			// Populate the UnassignedMessagesListBox
			this.UnassignedMessagesListBox.Items.Clear();
			ApplicationStringCollectionClass unassignedMessageCollection = 
							FMChannelHelper.MakeCall<IApplicationStrings, ApplicationStringCollectionClass>(
																	 x =>
																	 x.EnumerateByType(this.Security, stringType)
																);

			foreach (ApplicationStringClass unassignedMessage in unassignedMessageCollection)
			{
				if (this.AssignedMessagesListBox.Items.FindByValue(unassignedMessage.IdentityGuid.ToString()) == null)
				{
					this.UnassignedMessagesListBox.Items.Add(
						new ListItem(unassignedMessage.ID, unassignedMessage.IdentityGuid.ToString()));
				}
			}
		}

		private void AssignMessagesButtonCommand(object sender, CommandEventArgs e)
		{
			ListItem messageItem;
			
			while ((messageItem = this.UnassignedMessagesListBox.SelectedItem) != null)
			{
				this.UnassignedMessagesListBox.Items.Remove(messageItem);
				messageItem.Selected = false;
				this.AssignedMessagesListBox.Items.Add(messageItem);
			}

			this.UpdateData();
		}

		private void AssignProductsButtonCommand(object sender, CommandEventArgs e)
		{
			ListItem unassignedProductItem;
			
			while ((unassignedProductItem = this.UnassignedProductsListBox.SelectedItem) != null)
			{
				this.UnassignedProductsListBox.Items.Remove(unassignedProductItem);
				unassignedProductItem.Selected = false;

				foreach (ListItem assignedProductItem in this.AssignedProductsListBox.Items)
				{
					if (String.Compare(assignedProductItem.Text, unassignedProductItem.Text, StringComparison.Ordinal) > 0)
					{
						int index = this.AssignedProductsListBox.Items.IndexOf(assignedProductItem);
						this.AssignedProductsListBox.Items.Insert(index, unassignedProductItem);
						unassignedProductItem = null;
						break;
					}
				}

				if (unassignedProductItem != null)
				{
					this.AssignedProductsListBox.Items.Add(unassignedProductItem);
				}
			}
		}

		private void CancelCommand(object sender, CommandEventArgs e)
		{
			this.Redirect("ProductGroupsForm.aspx");
		}

		private void DownButtonCommand(object sender, CommandEventArgs e)
		{
			int selectedInt = this.AssignedMessagesListBox.SelectedIndex;
			int countInt = this.AssignedMessagesListBox.Items.Count;
			
			if (selectedInt < countInt - 1)
			{
				for (int itemInt = countInt - 1; itemInt >= 0; itemInt--)
				{
					if (!this.AssignedMessagesListBox.Items[itemInt].Selected && itemInt > 0
					    && this.AssignedMessagesListBox.Items[itemInt - 1].Selected)
					{
						ListItem moveItem = this.AssignedMessagesListBox.Items[itemInt];
						this.AssignedMessagesListBox.Items.RemoveAt(itemInt);
						this.AssignedMessagesListBox.Items.Insert(itemInt - 1, moveItem);
					}
				}
			}

			this.UpdateData();
		}

		/// <summary>
		///    Required method for Designer support - do not modify
		///    the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.DownButton.Command				+= this.DownButtonCommand;
			this.UnassignMessagesButton.Command += this.UnassignMessagesButtonCommand;
			this.AssignMessagesButton.Command	+= this.AssignMessagesButtonCommand;
			this.UpButton.Command				+= this.UpButtonCommand;
			this.Cancel.Command					+= this.CancelCommand;
			this.OK.Command						+= this.OkCommand;
			this.UnassignProductsButton.Command += this.UnassignProductsButtonCommand;
			this.AssignProductsButton.Command	+= this.AssignProductsButtonCommand;
		}

		private void OkCommand(object sender, CommandEventArgs e)
		{
			try
			{
				var productGroup = (ProductGroupClass)this.Session["ProductGroup"];

				if (this.Name.Text == string.Empty)
				{
					var except = new Exception("ID is a required field.");
					this.ErrorHandler(except);
					return;
				}

				productGroup.ID = this.Name.Text;

				// Create an Assigned ProductMapCollection
				var productMapCollection = new ProductMapCollectionClass();
				
				foreach (ListItem assignedProductItem in this.AssignedProductsListBox.Items)
				{
					var productMap = new ProductMapClass
					                 {
						                 AssignedToGuid = productGroup.IdentityGuid,
						                 AssignedGuid = Guid.Parse(assignedProductItem.Value),
						                 AssignedID = assignedProductItem.Text,
						                 Type = PRODUCT_MAP_TYPE.PRODUCT_GROUP_MAP
					                 };
					productMapCollection.Add(productMap);
				}

				productGroup.ProductMapCollection = productMapCollection;

				if (productGroup.IdentityGuid != Guid.Empty)
				{
					FMChannelHelper.MakeCall<IProductGroups>(x => x.Modify(this.Security, productGroup));
				}
				else
				{
					FMChannelHelper.MakeCall<IProductGroups, Guid>(x => x.Add(this.Security, productGroup));
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
				return;
			}

			this.Redirect("ProductGroupsForm.aspx");
		}

		private void UnassignMessagesButtonCommand(object sender, CommandEventArgs e)
		{
			ListItem assignedItem;
			
			while ((assignedItem = this.AssignedMessagesListBox.SelectedItem) != null)
			{
				this.AssignedMessagesListBox.Items.Remove(assignedItem);
				assignedItem.Selected = false;

				foreach (ListItem unassignedItem in this.UnassignedMessagesListBox.Items)
				{
					if (String.Compare(unassignedItem.Text, assignedItem.Text, StringComparison.Ordinal) > 0)
					{
						int index = this.UnassignedMessagesListBox.Items.IndexOf(unassignedItem);
						this.UnassignedMessagesListBox.Items.Insert(index, assignedItem);
						assignedItem = null;
						break;
					}
				}

				if (assignedItem != null)
				{
					this.UnassignedMessagesListBox.Items.Add(assignedItem);
				}
			}

			this.UpdateData();
		}

		private void UnassignProductsButtonCommand(object sender, CommandEventArgs e)
		{
			ListItem assignedProductItem;
			
			while ((assignedProductItem = this.AssignedProductsListBox.SelectedItem) != null)
			{
				this.AssignedProductsListBox.Items.Remove(assignedProductItem);
				assignedProductItem.Selected = false;

				foreach (ListItem unassignedProductItem in this.UnassignedProductsListBox.Items)
				{
					if (String.Compare(unassignedProductItem.Text, assignedProductItem.Text, StringComparison.Ordinal) > 0)
					{
						int index = this.UnassignedProductsListBox.Items.IndexOf(unassignedProductItem);
						this.UnassignedProductsListBox.Items.Insert(index, assignedProductItem);
						assignedProductItem = null;
						break;
					}
				}

				if (assignedProductItem != null)
				{
					this.UnassignedProductsListBox.Items.Add(assignedProductItem);
				}
			}
		}

		private void UpButtonCommand(object sender, CommandEventArgs e)
		{
			int selectedInt = this.AssignedMessagesListBox.SelectedIndex;
			int countInt = this.AssignedMessagesListBox.Items.Count;
			
			if (selectedInt > 0)
			{
				for (int itemInt = selectedInt - 1; itemInt < countInt; itemInt++)
				{
					if (!this.AssignedMessagesListBox.Items[itemInt].Selected && itemInt < countInt - 1
					    && this.AssignedMessagesListBox.Items[itemInt + 1].Selected)
					{
						ListItem moveItem = this.AssignedMessagesListBox.Items[itemInt];
						this.AssignedMessagesListBox.Items.RemoveAt(itemInt);
						this.AssignedMessagesListBox.Items.Insert(itemInt + 1, moveItem);
					}
				}
			}

			this.UpdateData();
		}
		#endregion
	}
}