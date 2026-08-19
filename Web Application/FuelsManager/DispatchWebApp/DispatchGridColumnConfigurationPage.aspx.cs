// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DispatchGridColumnConfigurationPage.aspx.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the DispatchGridColumnConfigurationPage type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FuelsManager.DispatchWebApp
{
	using System;
	using System.Collections.Generic;
	using System.Globalization;
	using System.Web.UI.WebControls;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;

	using FuelsManager.FMWebApp;

	/// <summary>
	///    Partial definition of the DispatchGridColumnConfigurationPage class.  Provides functionality for the
	///    Dispatch Grid Column Configuration web page.
	/// </summary>
	public partial class DispatchGridColumnConfigurationPage : FMFormBase
	{
		#region Constants and Fields
		/// <summary>
		///    Separator used to combine multiple values into a single string
		/// </summary>
		private const char StringSeparator = '|';
		#endregion

		#region Methods
		/// <summary>
		///    Saves the current dispatch grid column configuration to the database for the selected grid type.
		/// </summary>
		/// <param name="sender">The sender object</param>
		/// <param name="e">The event arguments</param>
		protected void ApplyButtonOnClick(object sender, EventArgs e)
		{
			try
			{
				// Populate the dispatch grid column list with the selected columns
				var dispatchGridColumnList = new DispatchGridColumnCollectionClass();
				int columnOrder = 0;
				foreach (ListItem selectedItem in this.selectedColumnsListBox.Items)
				{
					var selectedColumn = new DispatchGridColumnClass { UserGuid = this.Security.UserGuid };

					// Extract the ID and column type from the combined list item value field
					char[] seperator = { StringSeparator };
					string[] values = selectedItem.Value.Split(seperator);
					selectedColumn.ID = values[0];
					selectedColumn.GridColumnType = Convert.ToInt32(values[1]);

					// Extract the User Data Field Guid, Alias Name, and User Data Number from the combined list item value field
					if (selectedColumn.GridColumnType == DispatchGridColumnType.TransactionAliasUserDataColumnType)
					{
						selectedColumn.UserDataFieldTransactionAliasGuid = Guid.Parse(values[2]);
						selectedColumn.AliasName = values[3];
						selectedColumn.UserDataNumber = Convert.ToInt32(values[4]);
					}
					else if (selectedColumn.GridColumnType == DispatchGridColumnType.TransactionAliasLineItemUserDataColumnType)
					{
						selectedColumn.UserDataFieldTransactionAliasLineItemGuid = Guid.Parse(values[2]);
						selectedColumn.AliasName = values[3];
						selectedColumn.UserDataNumber = Convert.ToInt32(values[4]);
					}

					selectedColumn.ColumnOrder = columnOrder++;
					dispatchGridColumnList.Add(selectedColumn);
				}

				var dispatchConfigGuid = (Guid)this.Session["DispatchConfigGuid"];

				// Create a dispatch configuration if one doesn't exist and dispatch grid columns have been selected
				if ((dispatchConfigGuid == Guid.Empty) && (dispatchGridColumnList.Count > 0))
				{
					dispatchConfigGuid = FMChannelHelper.MakeCall<IDispatchConfigurations, Guid>(
						dispatchConfigs => dispatchConfigs.Add(this.Security, new DispatchConfigurationClass()));
					this.Session["DispatchConfigGuid"] = dispatchConfigGuid;
				}

				// Save the selected dispatch grid columns
				var dispatchGrid = (DispatchGridClass)this.Session["DispatchGrid"];
				dispatchGrid.DispatchConfigurationGuid = dispatchConfigGuid;
				dispatchGrid.GridColumnList = dispatchGridColumnList;

				FMChannelHelper.MakeCall<IDispatchGrids>(
					dispatchGrids =>
					{
						if (dispatchGrid.IdentityGuid == Guid.Empty)
						{
							dispatchGrid.IdentityGuid = dispatchGrids.Add(this.Security, dispatchGrid);
						}
						else
						{
							dispatchGrids.Modify(this.Security, dispatchGrid);
						}

						this.Session["DispatchGrid"] = dispatchGrids.Get(this.Security, dispatchGrid.IdentityGuid);
					});
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		/// <summary>
		///    Moves the selected list of columns down one row in the selected columns list box.
		/// </summary>
		/// <param name="sender">The sender object</param>
		/// <param name="e">The event arguments</param>
		protected void DownButtonOnClick(object sender, EventArgs e)
		{
			try
			{
				int itemIndex = this.selectedColumnsListBox.Items.Count - 1;
				int beginIndex = 0;
				int endIndex = 0;
				bool beginFound = false;
				bool endFound = false;
				while (itemIndex >= 0)
				{
					if (!beginFound && this.selectedColumnsListBox.Items[itemIndex].Selected)
					{
						beginIndex = itemIndex;
						beginFound = true;
					}

					if (beginFound)
					{
						if (!this.selectedColumnsListBox.Items[itemIndex].Selected)
						{
							endIndex = itemIndex + 1;
							endFound = true;
						}
						else if (itemIndex == 0)
						{
							endIndex = itemIndex;
							endFound = true;
						}
					}

					--itemIndex;

					if (beginFound && endFound)
					{
						if (beginIndex < this.selectedColumnsListBox.Items.Count - 1)
						{
							var endItem = new ListItem(
								this.selectedColumnsListBox.Items[beginIndex + 1].Text, this.selectedColumnsListBox.Items[beginIndex + 1].Value);
							for (int index = beginIndex; index >= endIndex; index--)
							{
								this.selectedColumnsListBox.Items[index + 1].Text = this.selectedColumnsListBox.Items[index].Text;
								this.selectedColumnsListBox.Items[index + 1].Value = this.selectedColumnsListBox.Items[index].Value;
								this.selectedColumnsListBox.Items[index + 1].Selected = this.selectedColumnsListBox.Items[index].Selected;
							}

							this.selectedColumnsListBox.Items[endIndex].Text = endItem.Text;
							this.selectedColumnsListBox.Items[endIndex].Value = endItem.Value;
							this.selectedColumnsListBox.Items[endIndex].Selected = endItem.Selected;
						}

						beginFound = false;
						endFound = false;
					}
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		/// <summary>
		///    Executed when the grid type dropdown list selection changes.  Populates the available and selected column list boxes
		///    based on the saved configuration of the selected dispatch grid type.  If no saved configuration exist then the
		///    selected column list box will be empty and the available column list box will contain all available columns.
		/// </summary>
		/// <param name="sender">The sender object</param>
		/// <param name="e">The event arguments</param>
		protected void GridTypeDropDownListSelectedIndexChanged(object sender, EventArgs e)
		{
			try
			{
				this.selectedColumnsListBox.Items.Clear();
				this.availableColumnsListBox.Items.Clear();

				if (this.gridTypeDropDownList.SelectedIndex == -1)
				{
					return;
				}

				var dispatchConfigGuid = (Guid)this.Session["DispatchConfigGuid"];
				var dispatchGrid = new DispatchGridClass { DispatchConfigurationGuid = dispatchConfigGuid };

				// Get the dispatch grid associated with the selected grid type
				// Extract the ID and grid type from the combined list item value field
				char[] seperator = { StringSeparator };
				string[] values = this.gridTypeDropDownList.SelectedItem.Value.Split(seperator);
				dispatchGrid.ID = values[0];
				dispatchGrid.GridType = Convert.ToInt32(values[1]);
				FMChannelHelper.MakeCall<IDispatchGrids>(
					dispatchGrids =>
					{
						Guid dispatchGridGuid = dispatchGrids.GetIdentityGuidById(
							this.Security, dispatchGrid.ID, dispatchGrid.DispatchConfigurationGuid);

						if (dispatchGridGuid != Guid.Empty)
						{
							dispatchGrid = dispatchGrids.Get(this.Security, dispatchGridGuid);
						}
					});

				this.Session["DispatchGrid"] = dispatchGrid;

				// Populate the Available Columns list box with the full set of dispatch grid columns
				// First populate with the commands associated with the grid type
				var gridColumnTypes = FMChannelHelper.MakeCall<IDispatchGridColumns, List<DispatchGridColumnType>>(
					gridColumns => gridColumns.EnumerateColumnTypes(this.Security, dispatchGrid.GridType, false));

				foreach (DispatchGridColumnType columnType in gridColumnTypes)
				{
					string combinedValue = columnType.DisplayName + StringSeparator + columnType.LookupIndex.ToString();
					var item = new ListItem(this.GetTranslatedText(columnType.DisplayName), combinedValue);
					this.availableColumnsListBox.Items.Add(item);
				}

				// Also populate with the dispatch transaction alias user data and transaction alias line item user data fields
				TransactionAliasNameCollectionClass aliasNames =
					FMChannelHelper.MakeCall<ITransactionAliases, TransactionAliasNameCollectionClass>(
							transactionAliases => transactionAliases.EnumerateDispatchAliasNames(this.Security)
					);

				string transactionAliasUserDataColumnType = DispatchGridColumnType.TransactionAliasUserDataColumnType.ToString();
				string transactionAliasLineItemUserDataColumnType = DispatchGridColumnType.TransactionAliasLineItemUserDataColumnType.ToString();

				foreach (TransactionAliasNameClass aliasName in aliasNames)
				{
					string id = aliasName.AliasName;
					Guid identityGuid = FMChannelHelper.MakeCall<ITransactionAliases, Guid>(
											transactionAliases => transactionAliases.GetIdentityGuid(this.Security, id));

					var userDataFields = FMChannelHelper.MakeCall<IUserDataFields, UserDataFieldCollectionClass>(
											transactionAliases => transactionAliases.EnumerateByEntityType(
												this.Security, ENTITY_TYPE.TRANSACTION_ALIAS, identityGuid, false, true));

					foreach (var fieldClass in userDataFields)
					{
						var userDataField = (UserDataFieldClass)fieldClass;
						string combinedValue = userDataField.DisplayName + StringSeparator + transactionAliasUserDataColumnType
												+ StringSeparator + userDataField.IdentityGuid + StringSeparator + id
												+ StringSeparator + userDataField.Number;
						var item = new ListItem(userDataField.DisplayName + '(' + id + ')', combinedValue);
						this.availableColumnsListBox.Items.Add(item);
					}

					var userDataLineItemFields = FMChannelHelper.MakeCall<IUserDataFields, UserDataFieldCollectionClass>(
											transactionAliases => transactionAliases.EnumerateByEntityType(
												this.Security, ENTITY_TYPE.TRANSACTION_ALIAS_LINE_ITEM, identityGuid, false, true));

					foreach (var fieldClass in userDataLineItemFields)
					{
						var userDataLineItemField = (UserDataFieldClass)fieldClass;
						string combinedValue = userDataLineItemField.DisplayName + StringSeparator + transactionAliasLineItemUserDataColumnType
												+ StringSeparator + userDataLineItemField.IdentityGuid + StringSeparator + id
												+ StringSeparator + userDataLineItemField.Number;
						var item = new ListItem(userDataLineItemField.DisplayName + '(' + id + ')', combinedValue);
						this.availableColumnsListBox.Items.Add(item);
					}
				}

				// Move the selected dispatch grid columns to the Selected Columns list box
				// The dispatch grid columns are ordered correctly when retrieved from the database
				foreach (DispatchGridColumnClass selectedColumn in dispatchGrid.GridColumnList)
				{
					string gridColumnType = selectedColumn.GridColumnType.ToString();
					string combinedValue = selectedColumn.ID + StringSeparator + gridColumnType;
					string itemText;
					if (gridColumnType == transactionAliasUserDataColumnType)
					{
						combinedValue += StringSeparator + selectedColumn.UserDataFieldTransactionAliasGuid.ToString() +
											StringSeparator + selectedColumn.AliasName + StringSeparator + selectedColumn.UserDataNumber;
						itemText = selectedColumn.ID + '(' + selectedColumn.AliasName + ')';
					}
					else if (gridColumnType == transactionAliasLineItemUserDataColumnType)
					{
						combinedValue += StringSeparator + selectedColumn.UserDataFieldTransactionAliasLineItemGuid.ToString() +
											StringSeparator + selectedColumn.AliasName + StringSeparator + selectedColumn.UserDataNumber;
						itemText = selectedColumn.ID + '(' + selectedColumn.AliasName + ')';
					}
					else
					{
						itemText = this.GetTranslatedText(selectedColumn.ID);
					}

					var selectedItem = new ListItem(itemText, combinedValue);
					this.availableColumnsListBox.Items.Remove(selectedItem);
					this.selectedColumnsListBox.Items.Add(selectedItem);
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		/// <summary>
		///    Executes when the page is loaded.  Disables command buttons if security requirements are not satisfied.
		///    Populates the grid type dropdown list with the available dispatch grid types.
		/// </summary>
		/// <param name="sender">The sender object</param>
		/// <param name="e">The event arguments</param>
		protected void Page_Load(object sender, EventArgs e)
		{
			try
			{
				this.GetSecurity();

				if (!this.Page.IsPostBack)
				{
					// Retrieve the current Dispatch Configuration Guid from the database
					bool entityAssigned = false;
					Guid dispatchConfigGuid = FMChannelHelper.MakeCall<IDispatchConfigurations, Guid>(
						dispatchConfigs => dispatchConfigs.GetIdentityGuidBySiteIdAndAssigned(
						this.Security, this.Security.SiteGuid, DispatchConfigurationClass.DefaultId, true, out entityAssigned));

					this.Session["DispatchConfigGuid"] = dispatchConfigGuid;

					if (entityAssigned || !this.Security.HasRight(RIGHT.MODIFY_DISPATCH))
					{
						this.EnableControls(false);
					}

					// Populate the grid type dropdown list
					var gridTypes = FMChannelHelper.MakeCall<IDispatchGrids, List<DispatchGridType>>(
						dispatchGrids => dispatchGrids.EnumerateGridTypes(this.Security));

					foreach (DispatchGridType gridType in gridTypes)
					{
						string combinedValue = gridType.Id + StringSeparator + gridType.LookupIndex.ToString(CultureInfo.InvariantCulture);
						var item = new ListItem(this.GetTranslatedText(gridType.Id), combinedValue);
						this.gridTypeDropDownList.Items.Add(item);
					}

					this.GridTypeDropDownListSelectedIndexChanged(null, null);
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		/// <summary>
		///    Moves the selected list of columns from the available columns list box to the selected columns list box.
		/// </summary>
		/// <param name="sender">The sender object</param>
		/// <param name="e">The event arguments</param>
		protected void SelectColumnsButtonOnClick(object sender, EventArgs e)
		{
			try
			{
				ListItem availableColumnItem;
				while ((availableColumnItem = this.availableColumnsListBox.SelectedItem) != null)
				{
					this.availableColumnsListBox.Items.Remove(availableColumnItem);
					availableColumnItem.Selected = false;
					this.selectedColumnsListBox.Items.Add(availableColumnItem);
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		/// <summary>
		///    Moves the selected list of columns from the selected columns list box to the available columns list box.
		/// </summary>
		/// <param name="sender">The sender object</param>
		/// <param name="e">The event arguments</param>
		protected void UnselectColumnsButtonOnClick(object sender, EventArgs e)
		{
			try
			{
				ListItem selectedColumnItem;
				while ((selectedColumnItem = this.selectedColumnsListBox.SelectedItem) != null)
				{
					this.selectedColumnsListBox.Items.Remove(selectedColumnItem);
					selectedColumnItem.Selected = false;

					// Insert the unselected item into the available columns List alphabetically
					bool itemInserted = false;
					foreach (ListItem availableItem in this.availableColumnsListBox.Items)
					{
						if (availableItem.Text.CompareTo(selectedColumnItem.Text) > 0)
						{
							int index = this.availableColumnsListBox.Items.IndexOf(availableItem);
							this.availableColumnsListBox.Items.Insert(index, selectedColumnItem);
							itemInserted = true;
							break;
						}
					}

					if (!itemInserted)
					{
						this.availableColumnsListBox.Items.Add(selectedColumnItem);
					}
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		/// <summary>
		///    Moves the selected list of columns up one row in the selected columns list box.
		/// </summary>
		/// <param name="sender">The sender object</param>
		/// <param name="e">The event arguments</param>
		protected void UpButtonOnClick(object sender, EventArgs e)
		{
			try
			{
				int itemIndex = 0;
				int beginIndex = 0;
				int endIndex = 0;
				bool beginFound = false;
				bool endFound = false;

				while (itemIndex < this.selectedColumnsListBox.Items.Count)
				{
					if (!beginFound && this.selectedColumnsListBox.Items[itemIndex].Selected)
					{
						beginIndex = itemIndex;
						beginFound = true;
					}

					if (beginFound)
					{
						if (!this.selectedColumnsListBox.Items[itemIndex].Selected)
						{
							endIndex = itemIndex - 1;
							endFound = true;
						}
						else if (itemIndex == this.selectedColumnsListBox.Items.Count - 1)
						{
							endIndex = itemIndex;
							endFound = true;
						}
					}

					++itemIndex;

					if (beginFound && endFound)
					{
						if (beginIndex > 0)
						{
							var endItem = new ListItem(
								this.selectedColumnsListBox.Items[beginIndex - 1].Text, this.selectedColumnsListBox.Items[beginIndex - 1].Value);
							for (int index = beginIndex; index <= endIndex; index++)
							{
								this.selectedColumnsListBox.Items[index - 1].Text = this.selectedColumnsListBox.Items[index].Text;
								this.selectedColumnsListBox.Items[index - 1].Value = this.selectedColumnsListBox.Items[index].Value;
								this.selectedColumnsListBox.Items[index - 1].Selected = this.selectedColumnsListBox.Items[index].Selected;
							}

							this.selectedColumnsListBox.Items[endIndex].Text = endItem.Text;
							this.selectedColumnsListBox.Items[endIndex].Value = endItem.Value;
							this.selectedColumnsListBox.Items[endIndex].Selected = endItem.Selected;
						}

						beginFound = false;
						endFound = false;
					}
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		/// <summary>
		///    Enables or disables all the data modification controls.
		/// </summary>
		/// <param name="enable">If true controls are enabled otherwise they are disabled.</param>
		private void EnableControls(bool enable)
		{
			this.downButton.Enabled = enable;
			this.upButton.Enabled = enable;
			this.selectColumnsButton.Enabled = enable;
			this.unselectColumnsButton.Enabled = enable;
			this.applyButton.Enabled = enable;
		}
		#endregion
	}
}