// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UserDataForm.aspx.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the UserDataForm type.
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
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.Interfaces;

	using FMControls;

	using global::FMWebApp;

	/// <summary>
	/// Code behind for UserDataForm form.
	/// </summary>
	public partial class UserDataForm : FMFormBase, IEntityDiscovery, IMenuDiscovery
	{
		#region Constants and Fields

		protected string DisplayName;

		protected string Type;

		protected string[] ValueList;

		private const string ExistingEntitiesWarning =
			"Warning: Existing entities that do not have a value for this required user data field may cause errors during the use of FuelsManager until the field is populated.";

		#endregion

		#region Explicit Interface Properties

		bool IEntityDiscovery.EntityAssignable
		{
			get
			{
				return true;
			}
		}

		Type IEntityDiscovery.EntityEngineType
		{
			get
			{
				return typeof(IUserDataFields);
			}
		}

		ENTITY_TYPE IEntityDiscovery.EntityType
		{
			get
			{
				return ENTITY_TYPE.USER_DATA_FIELD;
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

            }
            else
            {
                // Depends Upon Shared Components Config
                if ((options & 0x4000) == 0)
                {
                    return null;
                }
            }
            var items = new List<FMMenuItem>();

			if (!security.HasRight(RIGHT.VIEW_SITES_AND_SITE_GROUPS) && !security.HasRight(RIGHT.MODIFY_SITES_AND_SITE_GROUPS))
			{
				return null;
			}

			items.Add(
				new FMMenuItem
					{
						MenuItemType = FMMenuItemType.CONFIG_SYSTEM_USER_DATA,
						RootMenuName = "Configuration",
						CategoryName = "System",
						ItemName = "User Data",
						NavigateUrl = "UserDataForm.aspx",
						ApplyDataDictionary = ApplyDataDictionary.Apply
					});

			return items;
		}

		public void TypeDropDownList_SelectedIndexChanged(Object sender, EventArgs e)
		{
			try
			{
				var TypeDropDownList = (FMDropDownList)sender;
				this.Type = TypeDropDownList.SelectedItem.Text;
				this.UpdateView();
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		#endregion

		#region Explicit Interface Methods

		EntityToSiteMapCollectionClass IEntityDiscovery.EnumerateEntityMaps(
			SecurityClass Security, ENTITY_ASSIGNMENT_TYPE Type)
		{
			var EntityToSiteMapCollection = new EntityToSiteMapCollectionClass();

			if (Type == ENTITY_ASSIGNMENT_TYPE.OWNED)
			{
			}
			else
			{
				EntityToSiteMapClass EntityToSiteMap = FMChannelHelper.MakeCall<IEntityToSiteMaps, EntityToSiteMapClass>(
																		 x =>
																		 x.Get(Security, ((IEntityDiscovery)this).EntityType, Security.LoginSiteGuid)
																	);

				if (Type == ENTITY_ASSIGNMENT_TYPE.ASSIGNED)
				{
					if (Security.LoginSiteGuid == EntityToSiteMap.IdentityGuid)
					{
						EntityToSiteMap.ID = "All User Data Configuration";
						EntityToSiteMapCollection.Add(EntityToSiteMap);
					}
				}
				else
				{
					if (EntityToSiteMap.IdentityGuid == Guid.Empty)
					{
						EntityToSiteMap = new EntityToSiteMapClass();
						EntityToSiteMap.SiteGuid = Guid.Empty;
						EntityToSiteMap.ID = "All User Data Configuration";
						EntityToSiteMap.TypeID = ((IEntityDiscovery)this).EntityType;
						EntityToSiteMap.IdentityGuid = Security.SiteGuid;
						EntityToSiteMapCollection.Add(EntityToSiteMap);
					}
				}
			}
			return EntityToSiteMapCollection;
		}

		Guid IEntityDiscovery.GetIdentityGuid(SecurityClass security, string ID)
		{
			EntityToSiteMapClass EntityToSiteMap = 
			FMChannelHelper.MakeCall<IEntityToSiteMaps, EntityToSiteMapClass>(
																	 x =>
																	 x.Get(security, ((IEntityDiscovery)this).EntityType, security.LoginSiteGuid)
																);

			return (EntityToSiteMap.IdentityGuid == Guid.Empty) ? security.SiteGuid : EntityToSiteMap.IdentityGuid;
		}

		void IEntityDiscovery.SetSiteGuid(SecurityClass security, Guid guid, Guid SiteGuid)
		{
			try
			{
				ArrayList userDataFieldEntityTypes = UserDataFieldClass.GetUserDataEntityTypes();

				foreach (ENTITY_TYPE userDataFieldEntityType in userDataFieldEntityTypes)
				{
					UserDataFieldCollectionClass UserDataFieldCollection = this.EnumerateUserDataFields(security, userDataFieldEntityType);
					foreach (UserDataFieldClass UserDataField in UserDataFieldCollection)
					{
						if (UserDataField.SiteGuid == security.SiteGuid)
						{
							UserDataField.SiteGuid = SiteGuid;
							this.ModifyUserDataField(security, UserDataField);
						}
					}
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		private void ModifyUserDataField(SecurityClass security, UserDataFieldClass UserDataField)
		{
			FMChannelHelper.MakeCall<IUserDataFields>(
																	 x =>
																	 x.Modify(security, UserDataField)
																);
		}

		private UserDataFieldCollectionClass EnumerateUserDataFields(SecurityClass security, ENTITY_TYPE userDataFieldEntityType)
		{
			return FMChannelHelper.MakeCall<IUserDataFields, UserDataFieldCollectionClass>(
																	 x =>
																	 x.Enumerate(security, userDataFieldEntityType)
																);
		}

		#endregion

		#region Methods

		protected void EntityTypeDropDownList_SelectedIndexChanged(object sender, EventArgs e)
		{
			try
			{
				this.Session["UserDataEntityType"] = Enum.Parse(typeof(ENTITY_TYPE), this.EntityTypeDropDownList.SelectedItem.Value);

				//set the edit item index to -1 so if the user is editing something while changing the entity type the edit is cancelled.
				this.UserDataFieldDataGrid.EditItemIndex = -1;
				this.UpdateView();
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		protected ListItemCollection EnumerateUserDataTypes()
		{
			var ListItems = new ListItemCollection();
			var UserDataField = new UserDataFieldClass();
			USER_DATA_TYPE[] Types = { USER_DATA_TYPE.TEXT, USER_DATA_TYPE.LIST };
			foreach (USER_DATA_TYPE Type in Types)
			{
				var Item = new ListItem(this.GetTranslatedText(UserDataFieldClass.TypeID(Type)), ((int)Type).ToString());
				ListItems.Add(Item);
			}
			return ListItems;
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

				if (this.Page.IsPostBack == false)
				{
					ENTITY_TYPE[] entityTypes =
						{
							ENTITY_TYPE.COMPANY, ENTITY_TYPE.PRODUCT, ENTITY_TYPE.SITE, ENTITY_TYPE.PERSONNEL,
							ENTITY_TYPE.FUEL_CARD, ENTITY_TYPE.EQUIPMENT, ENTITY_TYPE.USER, ENTITY_TYPE.IATA_CODE
                        };
					int index = 0;
					foreach (ENTITY_TYPE entityType in entityTypes)
					{
						var Item = new ListItem(EntityToSiteMapClass.GetEntityTypeID(entityType), entityType.ToString());
						this.EntityTypeDropDownList.Items.Add(Item);

						if ((this.Session["UserDataEntityType"] != null)
						    && ((ENTITY_TYPE)this.Session["UserDataEntityType"] == entityType))
						{
							this.EntityTypeDropDownList.SelectedIndex = index;
						}
						index++;
					}

					this.EntityTypeDropDownList_SelectedIndexChanged(null, null);
				}
				else
				{
					if (this.UserDataFieldDataGrid.EditItemIndex != -1
					    && this.UserDataFieldDataGrid.EditItemIndex < this.UserDataFieldDataGrid.Items.Count)
					{
						var DisplayNameTextBox =
							(TextBox)
							this.UserDataFieldDataGrid.Items[this.UserDataFieldDataGrid.EditItemIndex].FindControl("DisplayNameTextBox");

						if (DisplayNameTextBox != null)
						{
							this.DisplayName = DisplayNameTextBox.Text;
						}

						var ValueListTextBox =
							(TextBox)
							this.UserDataFieldDataGrid.Items[this.UserDataFieldDataGrid.EditItemIndex].FindControl("ValueListTextBox");

						if (ValueListTextBox != null)
						{
							char[] Separators = { '\r', '\n' };
							this.ValueList = ValueListTextBox.Text.Split(Separators);
						}
					}
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		private ICollection EnumerateUserDataFields()
		{
			var entityType = ENTITY_TYPE.UNKNOWN;
			if (this.Session["UserDataEntityType"] != null)
			{
				entityType = (ENTITY_TYPE)this.Session["UserDataEntityType"];
			}

			UserDataFieldCollectionClass UserDataFieldCollection = FMChannelHelper.MakeCall<IUserDataFields, UserDataFieldCollectionClass>(
																							x =>
																							x.EnumerateByEntityType(this.Security, entityType, Guid.Empty, false, false)
																						);

			EntityToSiteMapCollectionClass EntityToSiteMapCollection = FMChannelHelper.MakeCall<IEntityToSiteMaps, EntityToSiteMapCollectionClass>(
																	 x =>
																	 x.EnumerateByTypeIDAndSiteGuid(this.Security, ENTITY_TYPE.USER_DATA_FIELD, this.Security.SiteGuid)
																);

			var UserDataFieldDataTable = new DataTable();
			DataRow UserDataFieldDataRow;

			UserDataFieldDataTable.Columns.Add("SiteGuid", typeof(Guid));
			UserDataFieldDataTable.Columns.Add("IdentityGuid", typeof(Guid));
			UserDataFieldDataTable.Columns.Add("Number", typeof(int));
			UserDataFieldDataTable.Columns.Add("DisplayName", typeof(string));
			UserDataFieldDataTable.Columns.Add("Type", typeof(string));
			UserDataFieldDataTable.Columns.Add("ValueList", typeof(string));
			UserDataFieldDataTable.Columns.Add( "Required", typeof( bool ) );
			UserDataFieldDataTable.Columns.Add( "OriginalRequired", typeof( bool ) );

			for (byte Number = 0; Number < this.NumberOfUserDataItems(entityType); Number++)
			{
				UserDataFieldDataRow = UserDataFieldDataTable.NewRow();

				if (EntityToSiteMapCollection.Count != 0)
				{
					UserDataFieldDataRow["SiteGuid"] = EntityToSiteMapCollection[0].IdentityGuid;
				}
				else
				{
					UserDataFieldDataRow["SiteGuid"] = this.Security.SiteGuid;
				}

				UserDataFieldDataRow["IdentityGuid"] = Guid.Empty;
				UserDataFieldDataRow["Number"] = Number + 1;
				UserDataFieldDataRow["DisplayName"] = "";
				UserDataFieldDataRow["Type"] = "";
				UserDataFieldDataRow["ValueList"] = "";
				UserDataFieldDataRow["Required"] = false;
				UserDataFieldDataRow["OriginalRequired"] = false;

				foreach (UserDataFieldClass UserDataField in UserDataFieldCollection)
				{
					if (UserDataField.Number == Number)
					{
						UserDataFieldDataRow["IdentityGuid"] = UserDataField.IdentityGuid;
						UserDataFieldDataRow["DisplayName"] = UserDataField.DisplayName;
						UserDataFieldDataRow["Type"] = this.GetTranslatedText(UserDataFieldClass.TypeID(UserDataField.UserDataType));

						if (UserDataField.UserDataType == USER_DATA_TYPE.LIST)
						{
							foreach (UserDataListValueClass UserDataListValue in UserDataField.UserDataListValueCollection)
							{
								UserDataFieldDataRow["ValueList"] += UserDataListValue.ID + ";";
							}
						}

						UserDataFieldDataRow["Required"] = UserDataField.FieldRequired;
						UserDataFieldDataRow["OriginalRequired"] = UserDataField.FieldRequired;
						break;
					}
				}

				UserDataFieldDataTable.Rows.Add(UserDataFieldDataRow);
			}

			var UserDataFieldDataView = new DataView(UserDataFieldDataTable);
			return UserDataFieldDataView;
		}

		/// <summary>
		///    Required method for Designer support - do not modify
		///    the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.UserDataFieldDataGrid.EditCommand +=
				new System.Web.UI.WebControls.DataGridCommandEventHandler(this.UserDataFieldDataGrid_EditCommand);
			this.UserDataFieldDataGrid.CancelCommand +=
				new System.Web.UI.WebControls.DataGridCommandEventHandler(this.UserDataFieldDataGrid_CancelCommand);
			this.UserDataFieldDataGrid.UpdateCommand +=
				new System.Web.UI.WebControls.DataGridCommandEventHandler(this.UserDataFieldDataGrid_UpdateCommand);
			this.UserDataFieldDataGrid.ItemDataBound +=
				new System.Web.UI.WebControls.DataGridItemEventHandler(this.UserDataFieldDataGrid_ItemDataBound);
		}

		private byte NumberOfUserDataItems(ENTITY_TYPE entityType)
		{
			if (entityType == ENTITY_TYPE.COMPANY || entityType == ENTITY_TYPE.PRODUCT || entityType == ENTITY_TYPE.SITE
				|| entityType == ENTITY_TYPE.FUEL_CARD || entityType == ENTITY_TYPE.USER || entityType == ENTITY_TYPE.IATA_CODE)
			{
				return 8;
			}
			else if (entityType == ENTITY_TYPE.PERSONNEL || entityType == ENTITY_TYPE.EQUIPMENT  )
			{
				return 24;
			}
			else
			{
				throw new Exception(
					this.GetTranslatedText("Unsupported User Data Entity Type") + " "
					+ EntityToSiteMapClass.GetEntityTypeID(entityType));
			}
		}

		private void UpdateView()
		{
			this.UserDataFieldDataGrid.DataSource = this.EnumerateUserDataFields();
			this.UserDataFieldDataGrid.DataBind();
		}

		private void UserDataFieldDataGrid_CancelCommand(object source, DataGridCommandEventArgs e)
		{
			try
			{
				this.UserDataFieldDataGrid.EditItemIndex = -1;
				this.EntityTypeDropDownList.Enabled = true;
				this.UpdateView();
			}
			catch (Exception except)
			{
				this.EntityTypeDropDownList.Enabled = true;
				this.ErrorHandler(except);
			}
		}

		private void UserDataFieldDataGrid_EditCommand(object source, DataGridCommandEventArgs e)
		{
			try
			{
				this.UserDataFieldDataGrid.EditItemIndex = e.Item.ItemIndex;
				var DisplayNameLabel = (Label)e.Item.FindControl("DisplayNameLabel");
				if (DisplayNameLabel != null)
				{
					this.DisplayName = DisplayNameLabel.Text;
				}
				var TypeLabel = (Label)e.Item.FindControl("TypeLabel");
				if (TypeLabel != null)
				{
					this.Type = TypeLabel.Text;
				}
				var ValueListLabel = (Label)e.Item.FindControl("ValueListLabel");
				if (ValueListLabel != null)
				{
					char[] Separators = { ';' };
					this.ValueList = ValueListLabel.Text.Split(Separators);
				}

				// The entity dropdown list should be disabled during editing.
				this.EntityTypeDropDownList.Enabled = false;
				this.UpdateView();
			}
			catch (Exception except)
			{
				this.EntityTypeDropDownList.Enabled = true;
				this.ErrorHandler(except);
			}
		}

		private void UserDataFieldDataGrid_ItemDataBound(object sender, DataGridItemEventArgs e)
		{
			if (this.UserDataFieldDataGrid.EditItemIndex != -1 && e.Item.ItemIndex == this.UserDataFieldDataGrid.EditItemIndex)
			{
				var DisplayNameTextBox = (TextBox)e.Item.FindControl("DisplayNameTextBox");
				if (DisplayNameTextBox != null)
				{
					DisplayNameTextBox.Text = this.DisplayName;
				}

				var TypeDropDownList = (FMDropDownList)e.Item.FindControl("TypeDropDownList");
				if (TypeDropDownList != null)
				{
					TypeDropDownList.SelectedIndex = TypeDropDownList.Items.IndexOf(TypeDropDownList.Items.FindByText(this.Type));

					var ValueListTextBox = (TextBox)e.Item.FindControl("ValueListTextBox");
					if (ValueListTextBox != null)
					{
						if (TypeDropDownList.SelectedValue == "0")
						{
							ValueListTextBox.Visible = false;
						}
						else
						{
							foreach (string Value in this.ValueList)
							{
								if (Value != "")
								{
									ValueListTextBox.Text += Value + "\r\n";
								}
							}
						}
					}
				}
			}

			var EditButton = (LinkButton)e.Item.FindControl("EditButton");
			if (EditButton != null)
			{
				var SiteGuidLabel = (Label)e.Item.FindControl("SiteGuidLabel");

				if (!this.Security.HasRight(RIGHT.MODIFY_SITES_AND_SITE_GROUPS)
				    || this.Security.SiteGuid != Guid.Parse(SiteGuidLabel.Text))
				{
					EditButton.Enabled = false;
					EditButton.Text = "<img src=Images/Edit_un.gif border=0 align=absmiddle alt='Edit this item'>";
				}
			}
		}

		private void UserDataFieldDataGrid_UpdateCommand(object source, DataGridCommandEventArgs e)
		{
			try
			{
				if ((this.UserDataFieldDataGrid.EditItemIndex != -1)
				    && (e.Item.ItemIndex == this.UserDataFieldDataGrid.EditItemIndex))
				{
					var IdentityGuidLabel = (Label)e.Item.FindControl("IdentityGuidLabel");
					var NumberLabel = (Label)e.Item.FindControl("NumberLabel");
					var DisplayNameTextBox = (TextBox)e.Item.FindControl("DisplayNameTextBox");
					var TypeDropDownList = (DropDownList)e.Item.FindControl("TypeDropDownList");
					CheckBox RequiredCheckBox = (CheckBox)e.Item.FindControl("RequiredCheckBox");
					CheckBox originalRequiredCheckBox = (CheckBox)e.Item.FindControl("OriginalCheckBox");

					var UserDataField = new UserDataFieldClass();
					UserDataField.IdentityGuid = Guid.Parse(IdentityGuidLabel.Text);
					UserDataField.SiteGuid = this.Security.SiteGuid;

					ArrayList userDataEntityTypes = UserDataFieldClass.GetUserDataEntityTypes();

					foreach (ENTITY_TYPE userDataEntityType in userDataEntityTypes)
					{
						if (this.EntityTypeDropDownList.SelectedItem.Value == userDataEntityType.ToString())
						{
							UserDataField.UserDataEntityType = userDataEntityType;
						}
					}

					UserDataField.Number = Convert.ToByte(NumberLabel.Text);
					UserDataField.DisplayName = DisplayNameTextBox.Text;
					UserDataField.UserDataType = (USER_DATA_TYPE)Convert.ToByte(TypeDropDownList.SelectedValue);
					UserDataField.Number--;
					UserDataField.FieldRequired = RequiredCheckBox.Checked;

					if (UserDataField.UserDataType == USER_DATA_TYPE.LIST)
					{
						var ValueListTextBox = (TextBox)e.Item.FindControl("ValueListTextBox");
						char[] Seperators = { '\r', '\n', ';' };
						string[] Values = ValueListTextBox.Text.Split(Seperators);

						foreach (string Value in Values)
						{
							if (Value != "")
							{
								var UserDataListValue = new UserDataListValueClass();
								UserDataListValue.ID = Value;
								UserDataField.UserDataListValueCollection.Add(UserDataListValue);
							}
						}
					}

					if (UserDataField.DisplayName == "")
					{
						if (UserDataField.IdentityGuid != Guid.Empty)
						{
							FMChannelHelper.MakeCall<IUserDataFields>(
																	 x =>
																	 x.Purge(this.Security, UserDataField.IdentityGuid, UserDataField.UserDataEntityType)
																);
						}
					}
					else
					{
						if (UserDataField.IdentityGuid != Guid.Empty)
						{
							FMChannelHelper.MakeCall<IUserDataFields>(
								x =>
								{
									x.Modify(this.Security, UserDataField);

									// Check to see if the field required setting changed.
									if ( UserDataField.FieldRequired 
										&& originalRequiredCheckBox != null 
										&& originalRequiredCheckBox.Checked == false)
									{
										throw new Exception(ExistingEntitiesWarning);
									}
								});
						}
						else
						{
							FMChannelHelper.MakeCall<IUserDataFields>(
																	 x =>
																	 x.Add(this.Security, UserDataField)
																);

							if (UserDataField.FieldRequired)
							{
								throw new Exception(ExistingEntitiesWarning);
							}

						}
					}

					// Since the editing is complete, enable the entity dropdown list.
					this.EntityTypeDropDownList.Enabled = true;
					this.UserDataFieldDataGrid.EditItemIndex = -1;
					this.UpdateView();
				}
			}
			catch (Exception except)
			{
				this.EntityTypeDropDownList.Enabled = true;
				this.UserDataFieldDataGrid.EditItemIndex = -1;
				base.ErrorHandler(except);
				this.UpdateView();
			}
		}

		#endregion
	}
}