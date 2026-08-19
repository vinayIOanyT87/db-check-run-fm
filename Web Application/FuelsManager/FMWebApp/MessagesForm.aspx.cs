// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MessagesForm.aspx.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the MessagesForm type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FuelsManager.FMWebApp
{
	using System;
	using System.Collections.Generic;
	using System.Globalization;
	using System.Linq;
	using System.Web.UI.WebControls;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.UtilityObjects;

	using global::FMWebApp;

	/// <summary>
	///    Summary description for MessagesForm.
	/// </summary>
	public partial class MessagesForm : FMAutoSubmitFormBase, IMenuDiscovery
	{
		#region Constants and Fields
		protected Guid CarrierMasterGuid = Guid.Empty;
		protected Guid CarrierGuid = Guid.Empty;
		protected string CarrierID = "{All}";
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
                    return null;
            }
            else
            {
                // Depends Upon Load Rack
                if ((options & 0x8000) == 0)
                {
                    return null;
                }
            }

            var items = new List<FMMenuItem>();

			if (siteGroup)
			{
				return null;
			}

			if (!security.HasRight(RIGHT.VIEW_LOAD_RACK_DATA) && !security.HasRight(RIGHT.MODIFY_LOAD_RACK_DATA))
			{
				return null;
			}

			items.Add(
				new FMMenuItem
					{
						MenuItemType = FMMenuItemType.OPERATIONS_LOAD_RACK_MESSAGES,
						RootMenuName = "Operations",
						CategoryName = "Load Rack",
						ItemName = "Messages",
						NavigateUrl = "MessagesForm.aspx",
						ApplyDataDictionary = ApplyDataDictionary.Apply
					});

			return items;
		}

		public void ItemCarrierTextBoxTextChanged(object sender, EventArgs e)
		{
			var messageCollection = (MessageCollectionClass)this.Session["MessageCollection"];

			DataGridItem item = this.MessageDataGrid.Items[this.MessageDataGrid.EditItemIndex];
			var idTextBox = (TextBox)item.FindControl("IDTextBox");

			int index = this.MessageDataGrid.CurrentPageIndex * this.MessageDataGrid.PageSize
							+ this.MessageDataGrid.EditItemIndex;
			MessageClass message = messageCollection[index];
			message.ID = idTextBox.Text;

			var locationTypeDropDownList = (DropDownList)item.FindControl("LocationTypeDropDownList");
			message._LocationType = (MessageLocationType)Convert.ToInt32(locationTypeDropDownList.SelectedValue);

			var frequencyTypeDropDownList = (DropDownList)item.FindControl("FrequencyTypeDropDownList");
			message._FrequencyType = (MessageFrequencyType)Convert.ToInt32(frequencyTypeDropDownList.SelectedValue);

			var itemCarrierTextBox = (TextBox)sender;

			this.CarrierID = itemCarrierTextBox.Text;
			this.CarrierMasterGuid = FMChannelHelper.MakeCall<ICompanies, Guid>(
																	 x =>
																	 x.GetMasterRecordGuid(this.Security, this.CarrierID)
																);
            this.CarrierGuid = FMChannelHelper.MakeCall<ICompanies, Guid>(
                                                                     x =>
                                                                     x.GetIdentityGuid(this.Security, this.CarrierID)
                                                                );
            this.UpdateView();
		}
		#endregion


		#region Methods
		protected void AddButtonCommand(object sender, CommandEventArgs e)
		{
			var messageCollection = (MessageCollectionClass)this.Session["MessageCollection"];
			var message = new MessageClass();
			messageCollection.Add(message);
			this.MessageDataGrid.CurrentPageIndex = (messageCollection.Count - 1) / this.MessageDataGrid.PageSize;
			this.MessageDataGrid.EditItemIndex = (messageCollection.Count - 1) % this.MessageDataGrid.PageSize;
			this.EnableControls(false);
			this.UpdateView();
		}

		protected void CarrierTextBoxTextChanged(object sender, EventArgs e)
		{
			this.Session.Remove("MessageCollection");

			this.CarrierMasterGuid = FMChannelHelper.MakeCall<ICompanies, Guid>(
																	 x =>
																	 x.GetMasterRecordGuid(this.Security, this.CarrierTextBox.Text)
																);
            this.CarrierGuid = FMChannelHelper.MakeCall<ICompanies, Guid>(
                                                                     x =>
                                                                     x.GetIdentityGuid(this.Security, this.CarrierTextBox.Text)
                                                                );

            this.Session["CarrierMasterGuid"] = this.CarrierMasterGuid;
            this.Session["CarrierGuid"] = this.CarrierGuid;
            this.DriversDropDownList.Items.Clear();

			if (!this.CarrierMasterGuid.IsEmpty())
			{
				ListItemCollection drivers = this.EnumerateDrivers();

				this.DriversDropDownList.DataSource = drivers;
				this.DriversDropDownList.DataBind();

				if (this.Session["DriverGuid"] != null)
				{
					ListItem item = this.DriversDropDownList.Items.FindByValue(((Guid)this.Session["DriverGuid"]).ToString());

					if (item != null)
					{
						this.DriversDropDownList.SelectedIndex = this.DriversDropDownList.Items.IndexOf(item);
					}
				}

				this.Session["DriverGuid"] = Guid.Parse(this.DriversDropDownList.SelectedValue);
			}

			else
			{
				this.DriversDropDownList.Items.Add(new ListItem(this.GetTranslatedText("{All}"), Guid.Empty.ToString()));
			}

			this.UpdateView();
		}

		protected void DriversDropDownListSelectedIndexChanged(object sender, EventArgs e)
		{
			this.Session["DriverGuid"] = Guid.Parse(this.DriversDropDownList.SelectedValue);
			this.Session.Remove("MessageCollection");
			this.UpdateView();
		}

		protected void EnableControls(bool enable)
		{
			this.AddButton.Enabled = enable;
			this.AddButton2.Enabled = enable;
			this.DriversDropDownList.Enabled = enable;
			this.CarrierTextBox.Enabled = enable;

			this.LRMessagesFormPageSizeDropDown.Enabled = enable;
		}

		protected ListItemCollection EnumerateCarriers()
		{
			var carrierItems = new ListItemCollection();

			CompanyCollectionClass carrierCollection = FMChannelHelper.MakeCall<ICompanies, CompanyCollectionClass>(
																	 x =>
																	 x.EnumerateByRole(this.Security, COMPANY_ROLE.CARRIER, false, true)
																);

			var newItem = new ListItem(this.GetTranslatedText("{All}"), Guid.Empty.ToString());
			carrierItems.Add(newItem);
			
			foreach (CompanyClass carrier in carrierCollection)
			{
				if (carrier.LockedOut)
				{
					continue;
				}

				newItem = new ListItem(carrier.ID, carrier.MasterRecordGuid.ToString());

				foreach (ListItem existingItem in carrierItems)
				{
					if (existingItem.Text.CompareTo(newItem.Text) > 0)
					{
						int insert = carrierItems.IndexOf(existingItem);
						carrierItems.Insert(insert, newItem);
						newItem = null;
						break;
					}
				}

				if (newItem != null)
				{
					carrierItems.Add(newItem);
				}
			}

			return carrierItems;
		}

		protected ListItemCollection EnumerateDrivers()
		{
			var driverItems = new ListItemCollection();

			PersonCollectionClass driverCollection = new PersonCollectionClass();
            PersonCollectionClass loaderCollection = FMChannelHelper.MakeCall<IPersonnel, PersonCollectionClass>(
																	 x =>
																	 x.EnumerateByRoleAndCompanyGuid(this.Security, PERSON_ROLE.LOADER_ROLE, this.CarrierGuid)
																);
            PersonCollectionClass offloaderCollection = FMChannelHelper.MakeCall<IPersonnel, PersonCollectionClass>(
                                                                     x =>
                                                                     x.EnumerateByRoleAndCompanyGuid(this.Security, PERSON_ROLE.OFFLOADER_ROLE, this.CarrierGuid)
                                                                );
            loaderCollection.Union(offloaderCollection).ToList().ForEach(x => driverCollection.Add(x));

            var newItem = new ListItem(this.GetTranslatedText("{All}"), Guid.Empty.ToString());
			driverItems.Add(newItem);

            foreach (PersonClass driver in driverCollection)
            {
                if (driver.LockedOut)
                {
                    continue;
                }

				newItem = new ListItem(driver.ID, driver.MasterRecordGuid.ToString());

				foreach (ListItem existingItem in driverItems)
				{
					if (existingItem.Text.CompareTo(newItem.Text) > 0)
					{
						int insert = driverItems.IndexOf(existingItem);
						driverItems.Insert(insert, newItem);
						newItem = null;
						break;
					}else if(existingItem.Text.CompareTo(newItem.Text) == 0)
                    {
                        newItem = null;
                        break;
                    }
				}
                
				if (newItem != null)
				{
					driverItems.Add(newItem);
				}               
			}

			return driverItems;
		}

		protected ListItemCollection EnumerateFrequencyTypes()
		{
			var typeItems = new ListItemCollection();

			for (var type = MessageFrequencyType.Always; type < MessageFrequencyType.MaxType; type++)
			{
				string frequencyID = this.GetTranslatedText(MessageClass.FrequencyTypeID(type));
				var newTypeItem = new ListItem(frequencyID, ((int)type).ToString(CultureInfo.InvariantCulture));
				typeItems.Add(newTypeItem);
			}

			return typeItems;
		}

		protected ListItemCollection EnumerateLocationTypes()
		{
			var typeItems = new ListItemCollection();

			for (var messageLocationType = MessageLocationType.Gate; messageLocationType < MessageLocationType.MaxType; messageLocationType++)
			{
				string locationID = this.GetTranslatedText(MessageClass.LocationTypeID(messageLocationType));
				var newTypeItem = new ListItem(locationID, ((int)messageLocationType).ToString(CultureInfo.InvariantCulture));
				typeItems.Add(newTypeItem);
			}

			return typeItems;
		}

		protected void MessageDataGridCancelCommand(object source, DataGridCommandEventArgs e)
		{
			var messageCollection = (MessageCollectionClass)this.Session["MessageCollection"];
			int index = this.MessageDataGrid.CurrentPageIndex * this.MessageDataGrid.PageSize + e.Item.ItemIndex;
			MessageClass message = messageCollection[index];

			if (message.IdentityGuid == Guid.Empty)
			{
				messageCollection.RemoveAt(index);

				if (this.MessageDataGrid.Items.Count == 1 && this.MessageDataGrid.CurrentPageIndex > 0)
				{
					this.MessageDataGrid.CurrentPageIndex--;
				}
			}

			this.MessageDataGrid.EditItemIndex = -1;
			this.EnableControls(true);
			this.Session.Remove("MessageCollection");
			this.UpdateView();
		}

		protected void MessageDataGridDeleteCommand(object source, DataGridCommandEventArgs e)
		{
			try
			{
				var messageCollection = (MessageCollectionClass)this.Session["MessageCollection"];

				int index = this.MessageDataGrid.CurrentPageIndex * this.MessageDataGrid.PageSize + e.Item.ItemIndex;
				MessageClass message = messageCollection[index];

				if (this.MessageDataGrid.EditItemIndex == e.Item.ItemIndex)
				{
					this.MessageDataGrid.EditItemIndex = -1;
					this.EnableControls(true);
				}
				else if (this.MessageDataGrid.EditItemIndex > e.Item.ItemIndex)
				{
					this.MessageDataGrid.EditItemIndex--;
				}

				// Non Zero identity guid indicates Message has been committed to database
				if (message.IdentityGuid != Guid.Empty)
				{
					FMChannelHelper.MakeCall<IMessages>(x => x.Purge(this.Security, message.IdentityGuid));
				}

				messageCollection.RemoveAt(index);

				if (this.MessageDataGrid.Items.Count == 1 && this.MessageDataGrid.CurrentPageIndex > 0)
				{
					this.MessageDataGrid.CurrentPageIndex--;
				}

				this.UpdateView();
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		protected void MessageDataGridEditCommand(object source, DataGridCommandEventArgs e)
		{
			this.MessageDataGrid.EditItemIndex = e.Item.ItemIndex;
			var messageCollection = (MessageCollectionClass)this.Session["MessageCollection"];
			int index = this.MessageDataGrid.CurrentPageIndex * this.MessageDataGrid.PageSize
							+ this.MessageDataGrid.EditItemIndex;
			MessageClass message = messageCollection[index];
			this.CarrierMasterGuid = message.CompanyGuid;
			CompanyClass carrier = FMChannelHelper.MakeCall<ICompanies, CompanyClass>(x => x.Get(this.Security, this.CarrierMasterGuid, false, false));
			this.CarrierGuid = carrier.IdentityGuid;
			this.CarrierID = message.CompanyID;

			// These items should be disabled during edit
			this.EnableControls(false);
			this.UpdateView();
		}

		protected void MessageDataGridPageIndexChanged(object source, DataGridPageChangedEventArgs e)
		{
			// if we are editing do not allow a page change
			if (this.MessageDataGrid.EditItemIndex > -1)
			{
				return;
			}

			this.MessageDataGrid.CurrentPageIndex = e.NewPageIndex;
			this.UpdateView();
		}

		protected void MessageDataGridUpdateCommand(object source, DataGridCommandEventArgs e)
		{
			try
			{
				var messageCollection = (MessageCollectionClass)this.Session["MessageCollection"];

				var idTextBox = (TextBox)e.Item.FindControl("IDTextBox");

				int index = this.MessageDataGrid.CurrentPageIndex * this.MessageDataGrid.PageSize
								+ this.MessageDataGrid.EditItemIndex;
				MessageClass message = messageCollection[index];
				message.ID = idTextBox.Text;

				var locationTypeDropDownList = (DropDownList)e.Item.FindControl("LocationTypeDropDownList");
				message._LocationType = (MessageLocationType)Convert.ToInt32(locationTypeDropDownList.SelectedValue);

				var frequencyTypeDropDownList = (DropDownList)e.Item.FindControl("FrequencyTypeDropDownList");
				message._FrequencyType = (MessageFrequencyType)Convert.ToInt32(frequencyTypeDropDownList.SelectedValue);

				var carrierTextBox = (TextBox)e.Item.FindControl("ItemCarrierTextBox");

				message.CompanyGuid = FMChannelHelper.MakeCall<ICompanies, Guid>(
																	 x =>
																	 x.GetMasterRecordGuid(this.Security, carrierTextBox.Text)
																);

				message.CompanyID = message.CompanyGuid == Guid.Empty ? "{All}" : carrierTextBox.Text;

                var driverDropDownList = (DropDownList)e.Item.FindControl("DriverDropDownList");
				message.PersonnelGuid = Guid.Parse(driverDropDownList.SelectedValue);

				message.PersonID = message.PersonnelGuid == Guid.Empty ? "{All}" : driverDropDownList.SelectedItem.Text;

                if (message.IdentityGuid == Guid.Empty)
				{
					message.IdentityGuid = FMChannelHelper.MakeCall<IMessages, Guid>(
																	 x =>
																	 x.Add(this.Security, message)
																);

				}
				else
				{
					FMChannelHelper.MakeCall<IMessages>(x => x.Modify(this.Security, message));
				}

				this.EnableControls(true);
				this.MessageDataGrid.EditItemIndex = -1;
				this.Session.Remove("MessageCollection");
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}

			try
			{
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

		protected void PageSizeDropDownSelectedIndexChanged(object source, EventArgs e)
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
					if (!this.Security.HasRight(RIGHT.MODIFY_LOAD_RACK_DATA))
					{
						this.AddButton.Enabled = false;
						this.AddButton2.Enabled = false;
					}

					if (this.Session["CarrierGuid"] != null && (Guid)this.Session["CarrierGuid"] != Guid.Empty)
					{
						CompanyClass company = FMChannelHelper.MakeCall<ICompanies, CompanyClass>(
																	 companies => companies.Get(this.Security, (Guid)this.Session["CarrierGuid"], false));
						this.CarrierTextBox.Text = company.ID;
					}
					else
					{
						this.Session["CarrierGuid"] = Guid.Empty;
						this.CarrierTextBox.Text = this.CarrierID;
					}

					this.CarrierTextBoxTextChanged(null, null);
					this.UpdateView();
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		/// <summary>
		///    Required method for Designer support - do not modify
		///    the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.AddButton2.Command					+= this.AddButtonCommand;
			this.MessageDataGrid.EditCommand		+= this.MessageDataGridEditCommand;
			this.MessageDataGrid.PageIndexChanged	+= this.MessageDataGridPageIndexChanged;
			this.MessageDataGrid.CancelCommand		+= this.MessageDataGridCancelCommand;
			this.MessageDataGrid.UpdateCommand		+= this.MessageDataGridUpdateCommand;
			this.MessageDataGrid.DeleteCommand		+= this.MessageDataGridDeleteCommand;
			this.MessageDataGrid.ItemDataBound		+= this.MessageDataGridItemDataBound;
			this.AddButton.Command					+= this.AddButtonCommand;
		}

		private void MessageDataGridItemDataBound(object sender, DataGridItemEventArgs e)
		{
			if (e.Item.ItemIndex != -1)
			{
				var editButton = (LinkButton)e.Item.FindControl("EditButton");
				var deleteButton = (LinkButton)e.Item.FindControl("DeleteButton");

				if (editButton != null && deleteButton != null)
				{
					editButton.Enabled = this.Security.HasRight(RIGHT.MODIFY_LOAD_RACK_DATA);
					deleteButton.Enabled = this.Security.HasRight(RIGHT.MODIFY_LOAD_RACK_DATA);
				}

				var messageCollection = (MessageCollectionClass)this.Session["MessageCollection"];
				int index = this.MessageDataGrid.CurrentPageIndex * this.MessageDataGrid.PageSize + e.Item.ItemIndex;
				MessageClass message = messageCollection[index];

				if (this.MessageDataGrid.EditItemIndex == e.Item.ItemIndex)
				{
					var locationTypeDropDownList = (DropDownList)e.Item.FindControl("LocationTypeDropDownList");
					ListItem item = locationTypeDropDownList.Items.FindByValue(((int)message._LocationType).ToString(CultureInfo.InvariantCulture));
					locationTypeDropDownList.SelectedIndex = locationTypeDropDownList.Items.IndexOf(item);

					var frequencyTypeDropDownList = (DropDownList)e.Item.FindControl("FrequencyTypeDropDownList");
					item = frequencyTypeDropDownList.Items.FindByValue(((int)message._FrequencyType).ToString(CultureInfo.InvariantCulture));
					frequencyTypeDropDownList.SelectedIndex = frequencyTypeDropDownList.Items.IndexOf(item);

					var itemCarrierTextBox = (TextBox)e.Item.FindControl("ItemCarrierTextBox");
					itemCarrierTextBox.Text = this.CarrierID;

					var driverDropDownList = (DropDownList)e.Item.FindControl("DriverDropDownList");
					item = driverDropDownList.Items.FindByValue(message.PersonnelGuid.ToString());
					driverDropDownList.SelectedIndex = driverDropDownList.Items.IndexOf(item);
				}
				else
				{
					var carrierLabel = (Label)e.Item.FindControl("CarrierLabel");
					carrierLabel.Text = message.CompanyID;
					carrierLabel.ToolTip = message.CompanyToolTip;
					var driverLabel = (Label)e.Item.FindControl("DriverLabel");
					driverLabel.Text = message.PersonID;
					driverLabel.ToolTip = message.PersonToolTip;

					if (this.Session["UseDataDictionary"] == null || (bool)this.Session["UseDataDictionary"])
					{
						carrierLabel.Text = FMChannelHelper.MakeCall<IDataDictionariesClass, string>(
																	 x =>
																	 x.Get(this.Security.LoginSiteGuid, carrierLabel.Text)
																);

						driverLabel.Text = FMChannelHelper.MakeCall<IDataDictionariesClass, string>(
																	 x =>
																	 x.Get(this.Security.LoginSiteGuid, driverLabel.Text)
																);
					}

					carrierLabel.Text = carrierLabel.Text.Replace("&", "&amp");
					carrierLabel.Text = carrierLabel.Text.Replace(">", "&gt");
					carrierLabel.Text = carrierLabel.Text.Replace("<", "&lt");
					carrierLabel.Text = carrierLabel.Text.Replace("'", "&apos");
					carrierLabel.Text = carrierLabel.Text.Replace("\"", "&quot");

					driverLabel.Text = driverLabel.Text.Replace("&", "&amp");
					driverLabel.Text = driverLabel.Text.Replace(">", "&gt");
					driverLabel.Text = driverLabel.Text.Replace("<", "&lt");
					driverLabel.Text = driverLabel.Text.Replace("'", "&apos");
					driverLabel.Text = driverLabel.Text.Replace("\"", "&quot");
				}
			}
		}

		private void UpdateView()
		{
			try
			{
				if (this.Session["MessageCollection"] == null)
				{
					if ((Guid)this.Session["CarrierGuid"] == Guid.Empty)
					{
						this.Session["MessageCollection"] = FMChannelHelper.MakeCall<IMessages, MessageCollectionClass>(
																	 x =>
																	 x.Enumerate(this.Security)
																);

					}
					else if ((Guid)this.Session["DriverGuid"] == Guid.Empty)
					{
						this.Session["MessageCollection"] = FMChannelHelper.MakeCall<IMessages, MessageCollectionClass>(
								x =>
								x.EnumerateByCompany(this.Security, (Guid)this.Session["CarrierGuid"])
						);

					}
					else
					{
						this.Session["MessageCollection"] = FMChannelHelper.MakeCall<IMessages, MessageCollectionClass>(
								x =>
								x.EnumerateByGuids(this.Security, (Guid)this.Session["CarrierGuid"], (Guid)this.Session["DriverGuid"])
						);
					}
				}

				var messageCollection = (MessageCollectionClass)this.Session["MessageCollection"];

				this.LRMessagesFormPageSizeDropDown.SetPageSize(this.MessageDataGrid, messageCollection.Count);

				this.MessageDataGrid.DataSource = messageCollection;
				this.MessageDataGrid.DataBind();
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}
		#endregion
	}
}