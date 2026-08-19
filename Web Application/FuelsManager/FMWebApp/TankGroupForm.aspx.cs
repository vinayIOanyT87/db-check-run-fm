// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TankGroupForm.aspx.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the TankGroupForm type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FuelsManager.FMWebApp
{
	using System;
	using System.Net.Sockets;
	using System.Web.UI.WebControls;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.Interfaces;

	/// <summary>
	///    Summary description for TankGroupForm.
	/// </summary>
	public partial class TankGroupForm : FMAutoSubmitFormBase
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
				this.GetSecurity();

				if (!this.Page.IsPostBack)
				{
					TankGroupClass tankGroup;

					// Get IdentityGuid
					if (this.Session["IdentityGuid"] != null)
					{
						// Get TankGroup
						tankGroup = FMChannelHelper.MakeCall<ITankGroups, TankGroupClass>(
																	 x =>
																	 x.Get(this.Security, Guid.Parse(this.Session["IdentityGuid"] as string ?? Guid.Empty.ToString()))
																);
					}
					else
					{
						tankGroup = new TankGroupClass();
					}

					this.Session["TankGroup"] = tankGroup;

					this.Name.Text = tankGroup.ID;

					// Populate AssignedTanksListBox
					foreach (TankMapClass tankMap in tankGroup.TankMapCollection)
					{
						var unassignedTankItem = new ListItem(tankMap.AssignedID, tankMap.TankGuid.ToString());

						foreach (ListItem assignedTankItem in this.AssignedTanksListBox.Items)
						{
							if (string.Compare(assignedTankItem.Text, unassignedTankItem.Text, StringComparison.Ordinal) > 0)
							{
								int index = this.AssignedTanksListBox.Items.IndexOf(assignedTankItem);
								this.AssignedTanksListBox.Items.Insert(index, unassignedTankItem);
								unassignedTankItem = null;
								break;
							}
						}

						if (unassignedTankItem != null)
						{
							this.AssignedTanksListBox.Items.Add(unassignedTankItem);
						}
					}

					// Populate the ProductsDropDownListBox
				    ProductCollectionClass productCollection = FMChannelHelper.MakeCall<IProducts, ProductCollectionClass>(
																	 x =>
																	 x.EnumerateByType(this.Security, ProductType.ComponentProduct)
																);

					foreach (ProductClass product in productCollection)
					{
						var item = new ListItem(product.ID, product.MasterRecordGuid.ToString());
						this.ProductsDropDownList.Items.Add(item);
						if (product.MasterRecordGuid == tankGroup.ProductGuid)
						{
							this.ProductsDropDownList.SelectedIndex = this.ProductsDropDownList.Items.Count - 1;
						}
					}

					// Populate UnassignedTanksListBox
					if (tankGroup.ProductGuid != Guid.Empty)
					{
						this.PopulateUnassignedTanksListBox(tankGroup.ProductGuid);
					}
					else if (this.ProductsDropDownList.SelectedIndex != -1)
					{
						this.PopulateUnassignedTanksListBox(Guid.Parse(this.ProductsDropDownList.SelectedItem.Value));
					}

					if (!this.Security.HasRight(RIGHT.MODIFY_TANK_DATA)
					    || (this.Security.SiteGuid != tankGroup.SiteGuid && tankGroup.IdentityGuid != Guid.Empty))
					{
						this.OK.Enabled = false;
					}

					//Set the title label with a key field from the bound object appended
				    this.TankGroupTitleLabel.Text = this.GetTitleLabelText(this.TankGroupTitleLabel.Text, tankGroup.ID);
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		protected void PopulateUnassignedTanksListBox(Guid productMasterGuid)
		{
			TankCollectionClass tankCollection = FMChannelHelper.MakeCall<ITanks, TankCollectionClass>(
																	 x =>
																	 x.EnumerateByProduct(this.Security, productMasterGuid)
																);

			foreach (TankClass tank in tankCollection)
			{
				if (null == this.AssignedTanksListBox.Items.FindByValue(tank.IdentityGuid.ToString()))
				{
					var assignedTankItem = new ListItem(tank.ID, tank.IdentityGuid.ToString());

					foreach (ListItem unassignedTankItem in this.UnassignedTanksListBox.Items)
					{
						if (string.Compare(unassignedTankItem.Text, assignedTankItem.Text, StringComparison.Ordinal) > 0)
						{
							int index = this.UnassignedTanksListBox.Items.IndexOf(unassignedTankItem);
							this.UnassignedTanksListBox.Items.Insert(index, assignedTankItem);
							assignedTankItem = null;
							break;
						}
					}

					if (assignedTankItem != null)
					{
						this.UnassignedTanksListBox.Items.Add(assignedTankItem);
					}
				}
			}
		}

		protected void ProductsDropDownListSelectedIndexChanged(object sender, EventArgs e)
		{
			this.AssignedTanksListBox.Items.Clear();
			this.UnassignedTanksListBox.Items.Clear();

			Guid productGuid = FMChannelHelper.MakeCall<IProducts, Guid>(x => x.GetMasterRecordGuid(this.Security, Guid.Parse(this.ProductsDropDownList.SelectedItem.Value)));

			// Populate UnassignedTanksListBox
			if (productGuid != Guid.Empty)
			{
				this.PopulateUnassignedTanksListBox(productGuid);
			}
		}

		private void AssignTanksButtonCommand(object sender, CommandEventArgs e)
		{
			ListItem unassignedTankItem;
			while ((unassignedTankItem = this.UnassignedTanksListBox.SelectedItem) != null)
			{
				this.UnassignedTanksListBox.Items.Remove(unassignedTankItem);
				unassignedTankItem.Selected = false;

				foreach (ListItem assignedTankItem in this.AssignedTanksListBox.Items)
				{
					if (string.Compare(assignedTankItem.Text, unassignedTankItem.Text, StringComparison.Ordinal) > 0)
					{
						int index = this.AssignedTanksListBox.Items.IndexOf(assignedTankItem);
						this.AssignedTanksListBox.Items.Insert(index, unassignedTankItem);
						unassignedTankItem = null;
						break;
					}
				}

				if (unassignedTankItem != null)
				{
					this.AssignedTanksListBox.Items.Add(unassignedTankItem);
				}
			}
		}

		private void CancelCommand(object sender, CommandEventArgs e)
		{
			this.Redirect("TankGroupsForm.aspx");
		}

		/// <summary>
		///    Required method for Designer support - do not modify
		///    the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.Cancel.Command += this.CancelCommand;
			this.OK.Command += this.OkCommand;
			this.UnassignTanksButton.Command += this.UnassignTanksButtonCommand;
			this.AssignTanksButton.Command += this.AssignTanksButtonCommand;
		}

		private void OkCommand(object sender, CommandEventArgs e)
		{
			try
			{
				var tankGroup = (TankGroupClass)this.Session["TankGroup"];

				if (this.Name.Text == "")
				{
					var except = new Exception("ID is a required field.");
					this.ErrorHandler(except);
					return;
				}

				//if there are no items in the product drop down list, the selected item will be null
				//throw an exception to avoid a null reference exception when setting the ProductGuid
				if (this.ProductsDropDownList.SelectedItem == null)
				{
					var except = new Exception("You must select a product.");
					this.ErrorHandler(except);
					return;
				}

				tankGroup.ID = this.Name.Text;
                tankGroup.ProductGuid = FMChannelHelper.MakeCall<IProducts, Guid>(x => x.GetMasterRecordGuid(this.Security, Guid.Parse(this.ProductsDropDownList.SelectedItem.Value))); 
				tankGroup.ProductID = this.ProductsDropDownList.SelectedItem.Text;

				// Create an Assigned TankMapCollection
				var tankMapCollection = new TankMapCollectionClass();
				foreach (ListItem assignedTankItem in this.AssignedTanksListBox.Items)
				{
				    var tankMap = new TankMapClass
				                  {
				                      IdentityGuid = tankGroup.IdentityGuid,
				                      TankGuid = Guid.Parse(assignedTankItem.Value),
				                      AssignedID = assignedTankItem.Text
				                  };
				    tankMapCollection.Add(tankMap);
				}

				tankGroup.TankMapCollection = tankMapCollection;

				Guid identityGuid = tankGroup.IdentityGuid;
				if (tankGroup.IdentityGuid != Guid.Empty)
				{
					FMChannelHelper.MakeCall<ITankGroups>(
																	 x =>
																	 x.Modify(this.Security,tankGroup)
																);

				}
				else
				{
					tankGroup.IdentityGuid = FMChannelHelper.MakeCall<ITankGroups, Guid>(
																	 x =>
																	 x.Add(this.Security,tankGroup)
																);
				}

				try
				{
					ILoadRackManager loadRackManager = this.GetLoadRackManager();
					if (identityGuid != Guid.Empty)
					{
						loadRackManager.Modify(this.Security, typeof(TankGroupClass), tankGroup.IdentityGuid);
					}
					else
					{
						loadRackManager.Add(this.Security, typeof(TankGroupClass), tankGroup.IdentityGuid);
					}
				}
				catch (SocketException socketExcept)
				{
					if (socketExcept.ErrorCode != 10061)
					{
						throw;
					}
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
				return;
			}
			this.Redirect("TankGroupsForm.aspx");
		}

		private void UnassignTanksButtonCommand(object sender, CommandEventArgs e)
		{
			ListItem assignedTankItem;
			while ((assignedTankItem = this.AssignedTanksListBox.SelectedItem) != null)
			{
				this.AssignedTanksListBox.Items.Remove(assignedTankItem);
				assignedTankItem.Selected = false;

				foreach (ListItem unassignedTankItem in this.UnassignedTanksListBox.Items)
				{
					if (string.Compare(unassignedTankItem.Text, assignedTankItem.Text, StringComparison.Ordinal) > 0)
					{
						int index = this.UnassignedTanksListBox.Items.IndexOf(unassignedTankItem);
						this.UnassignedTanksListBox.Items.Insert(index, assignedTankItem);
						assignedTankItem = null;
						break;
					}
				}

				if (assignedTankItem != null)
				{
					this.UnassignedTanksListBox.Items.Add(assignedTankItem);
				}
			}
		}

		#endregion
	}
}