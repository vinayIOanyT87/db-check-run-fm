// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TransactionAliasUserDataPage.ascx.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the TransactionAliasUserDataPage type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FMWebApp
{
	using System;
	using System.Collections;
	using System.Data;
	using System.Web;
	using System.Web.UI;
	using System.Web.UI.WebControls;
    using System.Collections.Generic;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;
    using FMBusinessObjects.Constants;

	using FMControls;

	using FuelsManager.FMWebApp;

	/// <summary>
	///    Summary description for TransactionAliasUserDataPage.
	/// </summary>
	public partial class TransactionAliasUserDataPage : FMUserControlBase
	{
		#region Constants and Fields

		protected string DisplayName;

		protected string Type;

		protected string[] ValueList;

		#endregion

		#region Public Methods and Operators

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

		/// <summary>
		/// Called when the alias type changes.
		/// </summary>
		/// <param name="sender">The sender object</param>
		/// <param name="e">The event arguments</param>
		protected void OnAliasTypeChanged(object sender, EventArgs e)
		{
			try
			{
				this.UserDataFieldDataGrid.CurrentPageIndex = 0;
				this.UpdateView();
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		#endregion

		#region Methods

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

		protected ListItemCollection EnumerateUserGroups()
		{
			var userGroupItems = new ListItemCollection();

			try
			{
				GroupCollectionClass groupCollection =
					FMChannelHelper.MakeCall<IGroups, GroupCollectionClass>(x => x.Enumerate(this.Security));

				userGroupItems.Add(new ListItem(this.GetTranslatedText("{All}"), Guid.Empty.ToString()));

				foreach (GroupClass group in groupCollection)
				{
					var newUserGroupItem = new ListItem(group.ID, group.IdentityGuid.ToString());
					foreach (ListItem existingUserGroupItem in userGroupItems)
					{
						if (existingUserGroupItem.Text.CompareTo(newUserGroupItem.Text) > 0)
						{
							int idx = userGroupItems.IndexOf(existingUserGroupItem);
							userGroupItems.Insert(idx, newUserGroupItem);
							newUserGroupItem = null;
							break;
						}
					}

					if (newUserGroupItem != null)
					{
						userGroupItems.Add(newUserGroupItem);
					}
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
			return userGroupItems;
		}

		protected override void OnInit(EventArgs e)
		{
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			InitializeComponent();
			base.OnInit(e);
		}

		protected void Page_Load(object sender, EventArgs e)
		{
			try
			{
				if (!this.Page.IsPostBack)
				{
					this.PopulateFieldTypeDropDown();
					this.rbStandardAlias.Checked = true;
					this.UpdateView();
                    this.SetFieldAccessibilityForChildRecordVersion();
                }

				else
				{
					if (this.UserDataFieldDataGrid.EditItemIndex != -1)
					{
						var DisplayCheckBox =
							(CheckBox)
							this.UserDataFieldDataGrid.Items[this.UserDataFieldDataGrid.EditItemIndex].FindControl("DisplayCheckBox");
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

		/// <summary>
		///    Populates the field type drop down list.
		/// </summary>
		/// <remarks>
		///    The items in the drop down are similar to the transaction alias
		///    field types.  That enumeration was not used for two reasons.  First,
		///    simplicity.  Second, only two of the types were needed.
		/// </remarks>
		protected void PopulateFieldTypeDropDown()
		{
			this.ddlFieldType.Items.Add(new ListItem("Transaction", "Transaction"));
			this.ddlFieldType.Items.Add(new ListItem("Line Item", "Line Item"));
		}

		protected void ddlFieldType_SelectedIndexChanged(object sender, EventArgs e)
		{
			this.UpdateView();
		}

		/// <summary>
		///    This method enables/disables controls.
		/// </summary>
		/// <param name="enable"></param>
		private void EnableControls(bool enable)
		{
			// Call the main form to disable buttons and tabs.
			var transAliasForm = (TransactionAliasForm)this.Page;
			transAliasForm.EnableControls(enable);
		}

		private ICollection EnumerateUserDataFields()
		{
			var TransactionAlias = (TransactionAliasClass)this.Session["TransactionAlias"];

			// Determine whether to populate transaction alias user fields or line item user fields
			UserDataFieldCollectionClass userDataFields;
			if (this.ddlFieldType.SelectedItem.Value == "Line Item")
			{
				userDataFields = this.rbStandardAlias.Checked
									? TransactionAlias.LineItemUserDataFieldCollection
									: TransactionAlias.DispatchLineItemUserDataFields;
			}
			else
			{
				userDataFields = this.rbStandardAlias.Checked
									? TransactionAlias.UserDataFieldCollection
									: TransactionAlias.DispatchUserDataFields;
			}

			var UserDataFieldDataTable = new DataTable();
			DataRow UserDataFieldDataRow;

			UserDataFieldDataTable.Columns.Add("SiteGuid", typeof(Guid));
			UserDataFieldDataTable.Columns.Add("UserDataFieldGuid", typeof(Guid));
			UserDataFieldDataTable.Columns.Add("Number", typeof(int));
			UserDataFieldDataTable.Columns.Add("DisplayName", typeof(string));
			UserDataFieldDataTable.Columns.Add("Type", typeof(string));
			UserDataFieldDataTable.Columns.Add("ValueList", typeof(string));
			UserDataFieldDataTable.Columns.Add("Required", typeof(bool));
			UserDataFieldDataTable.Columns.Add("UserGroup", typeof(string));
			UserDataFieldDataTable.Columns.Add("ClearOnNew", typeof(bool));

			for (byte Number = 0; Number < 24; Number++)
			{
				UserDataFieldDataRow = UserDataFieldDataTable.NewRow();
				UserDataFieldDataRow["SiteGuid"] = this.Security.SiteGuid;
				UserDataFieldDataRow["UserDataFieldGuid"] = Guid.Empty;
				UserDataFieldDataRow["Number"] = Number + 1;
				UserDataFieldDataRow["DisplayName"] = "";
				UserDataFieldDataRow["Type"] = "";
				UserDataFieldDataRow["ValueList"] = "";
				UserDataFieldDataRow["Required"] = false;
				UserDataFieldDataRow["UserGroup"] = HttpUtility.HtmlEncode(this.GetTranslatedText("{All}"));
				UserDataFieldDataRow["ClearOnNew"] = false;

				foreach (UserDataFieldClass UserDataField in userDataFields)
				{
					if (UserDataField.Number == Number)
					{
						UserDataFieldDataRow["UserDataFieldGuid"] = UserDataField.IdentityGuid;
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
						UserDataFieldDataRow["UserGroup"] =
							HttpUtility.HtmlEncode(
								(UserDataField.UserGroupGuid == Guid.Empty) ? this.GetTranslatedText("{All}") : UserDataField.UserGroupID);
						UserDataFieldDataRow["ClearOnNew"] = UserDataField.ClearOnNew;
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
			this.UserDataFieldDataGrid.PageIndexChanged +=
				new System.Web.UI.WebControls.DataGridPageChangedEventHandler(this.UserDataFieldDataGrid_PageIndexChanged);
			this.UserDataFieldDataGrid.CancelCommand +=
				new System.Web.UI.WebControls.DataGridCommandEventHandler(this.UserDataFieldDataGrid_CancelCommand);
			this.UserDataFieldDataGrid.UpdateCommand +=
				new System.Web.UI.WebControls.DataGridCommandEventHandler(this.UserDataFieldDataGrid_UpdateCommand);
			this.UserDataFieldDataGrid.ItemDataBound +=
				new System.Web.UI.WebControls.DataGridItemEventHandler(this.UserDataFieldDataGrid_ItemDataBound);
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
				this.EnableControls(true);

				this.UpdateView();
			}
			catch (Exception except)
			{
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

				this.EnableControls(false);

				this.UpdateView();
			}
			catch (Exception except)
			{
				this.EnableControls(true);
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
/*
					string script = @"<script language='javascript'> document.getElementById('{0}').scrollIntoView(); </script>";
					ScriptManager.RegisterStartupScript(
						this.Page, this.GetType(), "page_set_focus", string.Format(script, DisplayNameTextBox.ClientID), false);
*/				}

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

							ValueListTextBox.MaxLength = ValueListTextBox.Text.Length + 1000;
							if (ValueListTextBox.MaxLength > 20000)
							{
								//Limit the maximum number of characters that list text box can hold.
								ValueListTextBox.MaxLength = 20000;
							}
						}
					}
				}

				var UserGroupDropDownList = e.Item.FindControl("UserGroupDropDownList") as DropDownList;
				if (UserGroupDropDownList != null)
				{
					var dataView = this.UserDataFieldDataGrid.DataSource as DataView;
					var UserGroup =
						dataView[this.UserDataFieldDataGrid.PageSize * this.UserDataFieldDataGrid.CurrentPageIndex + e.Item.ItemIndex].Row
							["UserGroup"] as string;
					ListItem Item = UserGroupDropDownList.Items.FindByText(UserGroup);
					int idx = UserGroupDropDownList.Items.IndexOf(Item);
					UserGroupDropDownList.SelectedIndex = idx;
				}
			}

			var EditButton = (LinkButton)e.Item.FindControl("EditButton");
			if (EditButton != null)
			{
				var SiteGuidLabel = (Label)e.Item.FindControl("SiteGuidLabel");

				// CSI 5856 - disable buttons if user has no modify right.
				if (!this.Security.HasRight(RIGHT.MODIFY_TRANSACTION_ALIASES)
				    || this.Security.SiteGuid != Guid.Parse(SiteGuidLabel.Text))
				{
					EditButton.Enabled = false;
				}
                TransactionAliasClass transactionAlias = (TransactionAliasClass)this.Session["TransactionAlias"];
                List<string> versionSpecificFields = this.Session[PageSessionKeyConstants.TRANS_ALIAS_VERSION_SPECIFIC_FIELDS] as List<string>;
                bool currentSiteOwnsRecordVersion = (transactionAlias.SiteGuid == this.Security.SiteGuid);
                if (EditButton.Enabled)
                {
                    if (!(transactionAlias.Equals(null) 
                            || transactionAlias.IdentityGuid.Equals(Guid.Empty) 
                            || (currentSiteOwnsRecordVersion && transactionAlias.IdentityGuid.Equals(transactionAlias.MasterRecordGuid))))
                    {
                        EditButton.Enabled = (versionSpecificFields != null) && versionSpecificFields.Contains("UserData");                        
                    }
                }
			}           
		}

		private void UserDataFieldDataGrid_PageIndexChanged(object source, DataGridPageChangedEventArgs e)
		{
			// if we are editing do not allow a page change
			if (this.UserDataFieldDataGrid.EditItemIndex > -1)
			{
				return;
			}
			this.UserDataFieldDataGrid.CurrentPageIndex = e.NewPageIndex;
			this.UpdateView();
		}

		/// <summary>
		/// This method handles the update command on the user data field record.
		/// </summary>
		/// <param name="source">Source</param>
		/// <param name="e">Event arguments</param>
		private void UserDataFieldDataGrid_UpdateCommand(object source, DataGridCommandEventArgs e)
		{
			try
			{
				if (this.UserDataFieldDataGrid.EditItemIndex != -1 && e.Item.ItemIndex == this.UserDataFieldDataGrid.EditItemIndex)
				{
					// Check if user fields apply to transaction or line item
					bool isTransaction = (this.ddlFieldType.SelectedValue == "Transaction");
					var transactionAlias = (TransactionAliasClass)this.Session["TransactionAlias"];

					var userDataFieldGuidLabel	= (Label)e.Item.FindControl("UserDataFieldGuidLabel");
					var numberLabel				= (Label)e.Item.FindControl("NumberLabel");
					var displayNameTextBox		= (TextBox)e.Item.FindControl("DisplayNameTextBox");
					var typeDropDownList		= (DropDownList)e.Item.FindControl("TypeDropDownList");
					var requiredCheckBox		= (CheckBox)e.Item.FindControl("RequiredCheckBox");
					var userGroupDropDownList	= (DropDownList)e.Item.FindControl("UserGroupDropDownList");
					var clearOnNewCheckBox		= (CheckBox)e.Item.FindControl("ClearOnNewCheckBox");

					FieldClass[] fields;

					// Either pull transaction fields or line item fields
					if (isTransaction)
					{
						fields = this.rbStandardAlias.Checked
									? transactionAlias.DisplayOrder(TRANSACTION_SECTION_TYPE.BODY)
									: transactionAlias.DispatchDisplayOrder(TRANSACTION_SECTION_TYPE.BODY);
					}
					else
					{
						fields = this.rbStandardAlias.Checked
									? transactionAlias.DisplayOrder(TRANSACTION_SECTION_TYPE.LINE_ITEMS)
									: transactionAlias.DispatchDisplayOrder(TRANSACTION_SECTION_TYPE.LINE_ITEMS);
					}

					var userDataField = new UserDataFieldClass
					                    {
						                    IdentityGuid = Guid.Parse(userDataFieldGuidLabel.Text),
						                    SiteGuid = this.Security.SiteGuid
					                    };

					// If this is line item user data then set the EntityTypeID to reflect this
					if (isTransaction)
					{
						userDataField.UserDataEntityType = ENTITY_TYPE.TRANSACTION_ALIAS;
					}
					else
					{
						userDataField.UserDataEntityType = ENTITY_TYPE.TRANSACTION_ALIAS_LINE_ITEM;
					}

					userDataField.Number = Convert.ToByte(numberLabel.Text);
					userDataField.Number--;

					userDataField.DisplayOrder	= fields.Length;
					userDataField.DisplayName	= displayNameTextBox.Text;
					userDataField.UserDataType	= (USER_DATA_TYPE)Convert.ToByte(typeDropDownList.SelectedValue);
					userDataField.FieldRequired = requiredCheckBox.Checked;
					userDataField.DispatchField = this.rbDispatchAlias.Checked;
					userDataField.UserGroupGuid = Guid.Parse(userGroupDropDownList.SelectedItem.Value);
					userDataField.UserGroupID	= userGroupDropDownList.SelectedItem.Text;
					userDataField.ClearOnNew	= clearOnNewCheckBox.Checked;

					if (userDataField.UserDataType == USER_DATA_TYPE.LIST)
					{
						var valueListTextBox = (TextBox)e.Item.FindControl("ValueListTextBox");
						char[] seperators = { '\r', '\n', ';' };
						string[] values = valueListTextBox.Text.Split(seperators);

						foreach (string stringValue in values)
						{
							if (stringValue != "")
							{
								var userDataListValue = new UserDataListValueClass { ID = stringValue };
								userDataField.UserDataListValueCollection.Add(userDataListValue);
							}
						}
					}

					var userDataFieldCollection = new UserDataFieldCollectionClass();
					UserDataFieldCollectionClass fieldsToIterate;

					// Choose the correct collection to iterate
					if (isTransaction)
					{
						fieldsToIterate = this.rbStandardAlias.Checked
											? transactionAlias.UserDataFieldCollection
											: transactionAlias.DispatchUserDataFields;
					}
					else
					{
						fieldsToIterate = this.rbStandardAlias.Checked
											? transactionAlias.LineItemUserDataFieldCollection
											: transactionAlias.DispatchLineItemUserDataFields;
					}

					foreach (var fieldClass in fieldsToIterate)
					{
						var existingUserDataField = (UserDataFieldClass)fieldClass;

						if (userDataField != null)
						{
							if (existingUserDataField.Number == userDataField.Number)
							{
								if (userDataField.DisplayName != "")
								{
									userDataField.DisplayOrder = existingUserDataField.DisplayOrder;
									userDataFieldCollection.Add(userDataField);
									userDataField = null;
								}
							}
							else
							{
								userDataFieldCollection.Add(existingUserDataField);
							}
						}
						else
						{
							userDataFieldCollection.Add(existingUserDataField);
						}
					}

					if (userDataField != null && userDataField.DisplayName != "")
					{
						userDataFieldCollection.Add(userDataField);
					}

					// Choose the correct collection to update
					if (this.rbStandardAlias.Checked)
					{
						if (isTransaction)
						{
							transactionAlias.UserDataFieldCollection = userDataFieldCollection;
						}
						else
						{
							transactionAlias.LineItemUserDataFieldCollection = userDataFieldCollection;
						}
					}
					else
					{
						if (isTransaction)
						{
							transactionAlias.DispatchUserDataFields = userDataFieldCollection;
						}
						else
						{
							transactionAlias.DispatchLineItemUserDataFields = userDataFieldCollection;
						}
					}

					this.UserDataFieldDataGrid.EditItemIndex = -1;
					this.EnableControls(true);

					this.UpdateView();

					var orderPage =
						(TransactionAliasFieldOrderPage)
						this.Page.FindControl("tcTransactionAliasTabs")
						    .FindControl("tpFieldOrderPage")
						    .FindControl("TransactionAliasFieldOrderPage");
					orderPage.ReloadSectionTypeDropDown();
				}
			}
			catch (Exception except)
			{
				this.UserDataFieldDataGrid.EditItemIndex = -1;
				this.EnableControls(true);
				this.ErrorHandler(except);
				this.UpdateView();
			}
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
                this.UserDataFieldDataGrid.Enabled = (this.UserDataFieldDataGrid.Enabled && versionSpecificFields.Contains("UserData"));
            }
        }
        #endregion
    }       
}