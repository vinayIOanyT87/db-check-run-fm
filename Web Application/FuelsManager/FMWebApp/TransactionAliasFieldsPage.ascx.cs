// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TransactionAliasFieldsPage.ascx.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the TransactionAliasFieldsPage type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FMWebApp
{
	using System;
	using System.Collections;
	using System.Data;
	using System.Web;
	using System.Web.UI.WebControls;
    using System.Collections.Generic;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;
    using FMBusinessObjects.Constants;

	using FuelsManager.FMWebApp;

	using Microsoft.Ajax.Utilities;

	using TransactionFields;

	/// <summary>
	/// Summary description for TransactionAliasFieldsPage.
	/// </summary>
	public partial class TransactionAliasFieldsPage : FMUserControlBase
	{
		#region Constants and Fields

		protected int PriorEditItemIndex = -1;

		#endregion

		#region Methods

		protected void Page_Load( object sender, EventArgs e )
		{
			try
			{
				if ( !this.Page.IsPostBack )
				{
					// Populate FieldTypeDropDownList
					for ( var fieldType = TransactionFieldType.Transaction; fieldType < TransactionFieldType.TransactionFieldTypeMax; fieldType++ )
					{
						var item = new ListItem( TransactionAliasFieldClass.TransactionFieldTypeID( fieldType ), ( (int) fieldType ).ToString() );
						this.FieldTypeDropDownList.Items.Add( item );
					}

					this.ViewState["CurrentFieldType"] = TransactionFieldType.Transaction;
                    this.SetFieldAccessibilityForChildRecordVersion();
                    this.rbStandardAlias.Checked = true;
					this.UpdateView();
				}
				else
				{
					this.UpdateGridValues();
				}
			}
			catch ( Exception except )
			{
				this.ErrorHandler( except );
			}
		}

		private void UpdateGridValues()
		{
			var currentFieldType = (TransactionFieldType)this.ViewState["CurrentFieldType"];

			var existingFields = this.GetExistingFields(currentFieldType);

			var gridItems = this.FieldDataGrid.Items;

			foreach ( DataGridItem item in gridItems )
			{
				if (item.ItemType == ListItemType.Item 
					|| item.ItemType == ListItemType.AlternatingItem)
				{
					// Determine if the field is displayed
					if (this.IsDisplayChecked(item))
					{
						var IdentityGuidLabel = (Label) item.FindControl( "IdentityGuidLabel" );
						var FieldNameLabel = (Label) item.FindControl( "FieldNameLabel" );
						var DisplayNameTextBox = (TextBox) item.FindControl( "DisplayNameTextBox" );
						var RequiredCheckBox = (CheckBox) item.FindControl( "RequiredCheckBox" );
						var UserGroupDropDownList = (DropDownList) item.FindControl( "UserGroupDropDownList" );
						var ClearOnNewCheckBox = (CheckBox) item.FindControl( "ClearOnNewCheckBox" );

						// Make sure the item is listed in the existing item list
						FieldClass field = this.FindExistingField(existingFields, item);
						if (field != null)
						{
							// Update the field values
							field.DisplayName = DisplayNameTextBox.Text;
							field.FieldRequired = RequiredCheckBox.Checked;
							field.DispatchField = this.rbDispatchAlias.Checked;
							field.UserGroupGuid = Guid.Parse(UserGroupDropDownList.SelectedItem.Value);
							field.UserGroupID = UserGroupDropDownList.SelectedItem.Text;
							field.ClearOnNew = ClearOnNewCheckBox.Checked;
						}
						else
						{
							// Add to existing field collection
							var newField = new TransactionAliasFieldClass
							               {
								               IdentityGuid = Guid.Parse(IdentityGuidLabel.Text),
								               DbName = FieldNameLabel.Text,
								               DisplayName = DisplayNameTextBox.Text,
								               Type = currentFieldType,
								               FieldRequired = RequiredCheckBox.Checked,
								               DispatchField = this.rbDispatchAlias.Checked,
								               UserGroupGuid =
									               Guid.Parse(UserGroupDropDownList.SelectedItem.Value),
								               UserGroupID = UserGroupDropDownList.SelectedItem.Text,
								               ClearOnNew = ClearOnNewCheckBox.Checked,
											   DisplayOrder = existingFields.Count,
											   VirtualField = TransactionAliasFieldClass.IsVirtual(FieldNameLabel.Text,currentFieldType)
							               };

							existingFields.Add(newField);
						}
					}
					else
					{
						// Make sure the item is removed from the existing item list
						this.RemoveItemFromList(existingFields, item);
					}
				}
			}

			var orderPage =
				(TransactionAliasFieldOrderPage)
					this.Page.FindControl("tcTransactionAliasTabs")
						.FindControl("tpFieldOrderPage")
						.FindControl("TransactionAliasFieldOrderPage");

			orderPage.ReloadSectionTypeDropDown();
		}

		private FieldClass FindExistingField(TransactionAliasFieldCollectionClass existingFields, DataGridItem item)
		{
			var fieldName = this.GetFieldName(item);

			foreach (var field in existingFields)
			{
				if (field.DbName.Equals(fieldName))
				{
					return field;
				}
			}

			return null;
		}

		private void RemoveItemFromList(TransactionAliasFieldCollectionClass existingFields, DataGridItem item)
		{
			var fieldName = this.GetFieldName(item);

			for (var index = 0; index < existingFields.Count; ++index)
			{
				var field = existingFields[index];

				if (fieldName.Equals(field.DbName))
				{
					existingFields.Remove(index);
					return;
				}
			}
		}

		protected string GetFieldName(DataGridItem item )
		{
			var label = (Label)item.FindControl("FieldNameLabel");
			return label.Text;
		}

		protected bool IsDisplayChecked(DataGridItem item)
		{
			var box = (CheckBox)item.FindControl("DisplayCheckBox");
			return box.Checked;
		}

		private ListItemCollection userGroups = null;

		protected ListItemCollection EnumerateUserGroups()
		{

			if (userGroups == null)
			{
				userGroups = new ListItemCollection();

				try
				{
					GroupCollectionClass groupCollection =
					    FMChannelHelper.MakeCall<IGroups, GroupCollectionClass>(groups => groups.Enumerate(this.Security));

					userGroups.Add(new ListItem(this.GetTranslatedText("{All}"), Guid.Empty.ToString()));

					foreach (GroupClass group in groupCollection)
					{
						var newUserGroupItem = new ListItem(group.ID, group.IdentityGuid.ToString());
						foreach (ListItem existingUserGroupItem in userGroups)
						{
							if (existingUserGroupItem.Text.CompareTo(newUserGroupItem.Text) > 0)
							{
								int idx = userGroups.IndexOf(existingUserGroupItem);
								userGroups.Insert(idx, newUserGroupItem);
								newUserGroupItem = null;
								break;
							}
						}

						if (newUserGroupItem != null)
						{
							userGroups.Add(newUserGroupItem);
						}
					}
				}
				catch (Exception except)
				{
					this.ErrorHandler(except);
				}
			}
			return userGroups;
		}

		protected void FieldDataGridItemDataBound(object sender, DataGridItemEventArgs e)
		{
			var SiteGuidLabel = (Label) e.Item.FindControl( "SiteGuidLabel" );

			if (SiteGuidLabel != null)
			{
				// CSI 5856 - disable buttons if user has no modify right.
				if (!this.Security.HasRight(RIGHT.MODIFY_TRANSACTION_ALIASES)
				    || this.Security.SiteGuid != Guid.Parse(SiteGuidLabel.Text))
				{
					e.Item.Enabled = false;
				}
			}

			if (e.Item.Enabled)
			{
				var transactionAlias = (TransactionAliasClass) this.Session["TransactionAlias"];
				var versionSpecificFields = this.Session[PageSessionKeyConstants.TRANS_ALIAS_VERSION_SPECIFIC_FIELDS] as List<string>;
				bool currentSiteOwnsRecordVersion = ( transactionAlias.SiteGuid == this.Security.SiteGuid );
				if (
					!((transactionAlias == null) || (transactionAlias.IdentityGuid.Equals(Guid.Empty))
					  || (currentSiteOwnsRecordVersion 
                            && transactionAlias.IdentityGuid.Equals(transactionAlias.MasterRecordGuid))))
				{
					e.Item.Enabled = (versionSpecificFields != null) && versionSpecificFields.Contains( "Fields" );
				}
			}

			var userGroupDropDownList = e.Item.FindControl( "UserGroupDropDownList" ) as DropDownList;
			if ( userGroupDropDownList != null )
			{
				var dataView = this.FieldDataGrid.DataSource as DataView;
				var userGroup = dataView[e.Item.ItemIndex].Row["UserGroup"] as string;
				ListItem item = userGroupDropDownList.Items.FindByText( userGroup );
				int index = userGroupDropDownList.Items.IndexOf( item );
				userGroupDropDownList.SelectedIndex = index;
			}

			var requiredEnabledCheckBox = e.Item.FindControl("RequiredEnabledCheckBox") as CheckBox;
			if (requiredEnabledCheckBox != null)
			{
				var requiredCheckBox = e.Item.FindControl("RequiredCheckBox") as CheckBox;
				if (requiredCheckBox != null)
				{
					requiredCheckBox.Enabled = requiredEnabledCheckBox.Checked;
				}
			}
		}

		protected void FieldTypeDropDownList_SelectedIndexChanged(object sender, EventArgs e)
		{
			try
			{
				this.ViewState["CurrentFieldType"] = 
					(TransactionFieldType)Enum.Parse(typeof(TransactionFieldType), this.FieldTypeDropDownList.SelectedValue);

				this.UpdateView();
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		/// <summary>
		///    Called when the alias type changes.
		/// </summary>
		/// <param name="sender">The sender object</param>
		/// <param name="e">The event arguments</param>
		protected void OnAliasTypeChanged(object sender, EventArgs e)
		{
			try
			{
				this.FieldDataGrid.CurrentPageIndex = 0;
				this.UpdateView();
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		/// <summary>
		///    This method enables/disables controls.
		/// </summary>
		/// <param name="enable"></param>
		private void EnableControls(bool enable)
		{
			this.FieldTypeDropDownList.Enabled = enable;

			// Call the main form to disable buttons and tabs.
			var transAliasForm = (TransactionAliasForm)this.Page;
			transAliasForm.EnableControls(enable);
		}

		private TransactionAliasFieldCollectionClass GetExistingFields(TransactionFieldType fieldType)
		{
			// Get the alias we are editing
			var transactionAlias = (TransactionAliasClass) this.Session["TransactionAlias"];

			switch ( fieldType )
			{
				case TransactionFieldType.Transaction:
					return this.rbStandardAlias.Checked
						? transactionAlias.TransactionFieldCollection
						: transactionAlias.DispatchTransactionFields;

				case TransactionFieldType.LineItem:
					return this.rbStandardAlias.Checked
						? transactionAlias.LineItemFieldCollection
						: transactionAlias.DispatchLineItemFields;

				case TransactionFieldType.WeightReading:
					return this.rbStandardAlias.Checked
						? transactionAlias.WeightReadingFieldCollection
						: transactionAlias.DispatchWeightReadingFields;
				
				case TransactionFieldType.Note:
					return this.rbStandardAlias.Checked 
						? transactionAlias.NoteFieldCollection 
						: transactionAlias.DispatchNoteFields;
				
				case TransactionFieldType.TransportInfo:
					return this.rbStandardAlias.Checked
						? transactionAlias.TransportLineItemFieldCollection
						: transactionAlias.DispatchTransportLineItemFields;
				
				case TransactionFieldType.ExportResult:
					return this.rbStandardAlias.Checked
						? transactionAlias.ExportResultDetailFieldCollection
						: transactionAlias.DispatchExportResultDetailFields;
				
				default:
					throw new Exception( "Undefined Transaction Field Type" );
			}
		}

		private ICollection EnumerateDataFields()
		{
			// Get the alias we are editing
			var transactionAlias = (TransactionAliasClass)this.Session["TransactionAlias"];

			// Create a data table to bind to the gird
			var fieldDataTable = new DataTable();

			// Add the columns for the grid.
			fieldDataTable.Columns.Add("SiteGuid", typeof(Guid));
			fieldDataTable.Columns.Add("IdentityGuid", typeof(Guid));
			fieldDataTable.Columns.Add("FieldName", typeof(string));
			fieldDataTable.Columns.Add("Display", typeof(bool));
			fieldDataTable.Columns.Add("DisplayName", typeof(string));
			fieldDataTable.Columns.Add("Required", typeof(bool));
			fieldDataTable.Columns.Add("UserGroup", typeof(string));
			fieldDataTable.Columns.Add( "ClearOnNew", typeof( bool ) );
			fieldDataTable.Columns.Add("RequiredEnabled", typeof(bool));

			var fieldType = (TransactionFieldType)Convert.ToByte(this.FieldTypeDropDownList.SelectedValue);

			// Get a list of the existing fields based on whether the user is editing a standard
			// set of fields or a dispatch set of fields.
			var existingFieldCollection = this.GetExistingFields(fieldType);

			// Now get a list of fields names in the specified table type.  This should be a list of the 
			// possible fields to include in the alias not the fields that are actually in the alias now.
			List<string> fieldNames = FMChannelHelper.MakeCall<ITransactionAliasFields, List<string>>(
				fields => fields.EnumerateFields(
					this.Security,
					fieldType: fieldType,
					transType: transactionAlias.TransTypeID));

			// Create a transaction context object so we can use it to determine if the required nature
			// of fields can actually be set by the user and then whether the field is actually
			// marked as required.
			var trans = new TransactionDO();
			var accountingSite = new AccountingSite();
			var txFieldGenerator = new TransactionFieldGenerator(null, null, null, null, null);
			var transContext = new TransactionContext(null, accountingSite, transactionAlias.ID, TransactionContext.Mode.View, false);
			trans.TransTypeID = transactionAlias.TransTypeID;

			var allText = HttpUtility.HtmlEncode( this.GetTranslatedText( "{All}" ) );

			// Go through all the possible fields to build a data source for the grid.  We want
			// to show them all.  If the field is currently in 
			// the alias, include all the settings for the field.
			foreach (string fieldName in fieldNames)
			{
				DataRow fieldDataRow = fieldDataTable.NewRow();
				fieldDataRow["SiteGuid"] = this.Security.SiteGuid;
				fieldDataRow["IdentityGuid"] = Guid.Empty;
				fieldDataRow["FieldName"] = fieldName;
				fieldDataRow["Display"] = false;
				fieldDataRow["DisplayName"] = fieldName;
				fieldDataRow["Required"] = false;
				fieldDataRow["UserGroup"] = allText;
				fieldDataRow["ClearOnNew"] = false;
				fieldDataRow["RequiredEnabled"] = true;

				// For each possible field check to see if the field has already been added to the alias.
				foreach (TransactionAliasFieldClass existingField in existingFieldCollection)
				{
					if (fieldName == existingField.DbName)
					{
						fieldDataRow["IdentityGuid"] = existingField.IdentityGuid;
						fieldDataRow["Display"] = true;
						fieldDataRow["DisplayName"] = existingField.DisplayName;
						fieldDataRow["ClearOnNew"] = existingField.ClearOnNew;

						// For fields that have unmodifiable Required field, set value to hardcoded value. (Do not use value from database)
						FieldGenerator fieldGenerator = txFieldGenerator.GetFieldGenerator(fieldName);
						if (fieldGenerator != null)
						{
							fieldGenerator.SetTransaction(trans);
							fieldGenerator.SetTransactionContext(transContext);
							fieldGenerator.Required = existingField.FieldRequired;
							fieldDataRow["Required"] = fieldGenerator.Required;

							bool oldRequiredValue = fieldGenerator.Required;

							//This checks if Required field can be modified. If modify fails
							//disable checkbox.
							fieldGenerator.Required = !oldRequiredValue;

							if ( fieldGenerator.Required == oldRequiredValue )
							{
								fieldDataRow["RequiredEnabled"] = false;
							}
						}
						else
						{
							fieldDataRow["Required"] = existingField.FieldRequired;
						}

						fieldDataRow["UserGroup"] =
							HttpUtility.HtmlEncode(
								(existingField.UserGroupGuid == Guid.Empty) ? allText : existingField.UserGroupID);

						break;
					}
				}

				fieldDataTable.Rows.Add(fieldDataRow);
			}

			return new DataView(fieldDataTable);
		}

		private void UpdateView()
		{
			// TODO: Determine if the field's required status can be set or not.

			ICollection fields = this.EnumerateDataFields();

			this.FieldDataGrid.DataSource = fields;
			this.FieldDataGrid.DataBind();
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
                this.FieldDataGrid.Enabled = (this.FieldDataGrid.Enabled && versionSpecificFields.Contains("Fields"));
                this.aliasTypePanel.Enabled = (this.aliasTypePanel.Enabled && versionSpecificFields.Contains("Fields"));
                this.rbDispatchAlias.Enabled = (this.rbDispatchAlias.Enabled && versionSpecificFields.Contains("Fields"));
                this.rbStandardAlias.Enabled = (this.rbStandardAlias.Enabled && versionSpecificFields.Contains("Fields"));
            }
        }

        #endregion
    }
}
