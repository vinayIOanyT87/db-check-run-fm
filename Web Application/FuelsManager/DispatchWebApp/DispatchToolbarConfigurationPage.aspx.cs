// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DispatchToolbarConfigurationPage.aspx.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the DispatchToolbarConfigurationPage type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FuelsManager.DispatchWebApp
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Web.UI.WebControls;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;

	using FuelsManager.FMWebApp;

	using global::FMWebApp;

	/// <summary>
	///    Partial definition of the DispatchToolbarConfigurationPage class.  Provides functionality for the
	///    Dispatch Toolbar Configuration web page.
	/// </summary>
	public partial class DispatchToolbarConfigurationPage : FMFormBase
	{
		#region Constants and Fields

		/// <summary>
		///    Separator used to combine multiple values into a single string
		/// </summary>
		private const char StringSeparator = '|';

		#endregion


		#region Methods

		private bool SameConfigAsDefault()
		{
			char[] seperator = { StringSeparator };
			string stringToolbarType = this.toolbarTypeDropDownList.SelectedItem.Value.Split(seperator)[1];
			var defaultCommands =
				FMChannelHelper.MakeCall<ICustomToolbarCommands, List<CustomToolbarCommandType>>(
					customToolbarCommands =>
						customToolbarCommands.EnumerateDefaultCommandTypes(this.Security, Convert.ToInt32(stringToolbarType)));

			bool sameAsDefault = defaultCommands.Count == selectedCommandsListBox.Items.Count;

			int columnOrder = 0;
			
			foreach (ListItem selectedItem in this.selectedCommandsListBox.Items)
			{

				// Extract the ID and ToolbarCommandType from the combined list item value field
				string[] values = selectedItem.Value.Split(seperator);
				sameAsDefault = sameAsDefault && (values[0] == defaultCommands[columnOrder].Id);
				columnOrder++;
			}
			return sameAsDefault;
		}

		/// <summary>
		///    Saves the current custom toolbar configuration to the database for the selected toolbar type.
		/// </summary>
		/// <param name="sender">The sender object</param>
		/// <param name="e">The event arguments</param>
		protected void ApplyButtonOnClick(object sender, EventArgs e)
		{
			try
			{

				char[] seperator = { StringSeparator };
				
				// Populate the Custom Toolbar Command List with the selected commands
				var toolbarCommandList = new CustomToolbarCommandCollectionClass();
				int columnOrder = 0;
				foreach (ListItem selectedItem in this.selectedCommandsListBox.Items)
				{
					var selectedCommand = new CustomToolbarCommandClass();

					// Extract the ID and ToolbarCommandType from the combined list item value field
					string[] values = selectedItem.Value.Split(seperator);
					selectedCommand.ID = values[0];
					selectedCommand.ToolbarCommandType = Convert.ToInt32(values[1]);

					// Extract the TransactionAliasGuid from the combined list item value field
					if (selectedCommand.ToolbarCommandType == CustomToolbarCommandType.TransactionAliasCommandType)
					{
						selectedCommand.TransactionAliasGuid = Guid.Parse(values[2]);
					}

					selectedCommand.ColumnOrder = columnOrder++;
					toolbarCommandList.Add(selectedCommand);

				}

				if (this.SameConfigAsDefault())
				{
					toolbarCommandList.Clear(); //this will save it as the default config
				}

				var dispatchConfigGuid = (Guid)this.Session["DispatchConfigGuid"];

				// Create a dispatch configuration if one doesn't exist and custom toolbar commands have been selected
				if ((dispatchConfigGuid == Guid.Empty) && (toolbarCommandList.Count > 0))
				{
					dispatchConfigGuid = FMChannelHelper.MakeCall<IDispatchConfigurations, Guid>(
						dispatchConfigs => dispatchConfigs.Add(this.Security, new DispatchConfigurationClass()));
					this.Session["DispatchConfigGuid"] = dispatchConfigGuid;
				}

				// Save the selected custom toolbar commands
				var customToolbar = (CustomToolbarClass)this.Session["CustomToolbar"];
				customToolbar.DispatchConfigurationGuid = dispatchConfigGuid;
				customToolbar.ToolbarCommandList = toolbarCommandList;

				FMChannelHelper.MakeCall<ICustomToolbars>(
					customToolbars =>
					{
						if (customToolbar.ToolbarCommandList.Count == 0)
						{
							if (customToolbar.IdentityGuid != Guid.Empty)
							{
								customToolbars.Purge(this.Security, customToolbar.IdentityGuid);
								customToolbar.IdentityGuid = Guid.Empty;
							}
						}
						else
						{
							if (customToolbar.IdentityGuid == Guid.Empty)
							{
								customToolbar.IdentityGuid = customToolbars.Add(this.Security, customToolbar);
							}
							else
							{
								customToolbars.Modify(this.Security, customToolbar);
							}

							this.Session["CustomToolbar"] = customToolbars.Get(this.Security, customToolbar.IdentityGuid);
						}
					});
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		/// <summary>
		///    Moves the selected list of commands down one row in the selected commands list box.
		/// </summary>
		/// <param name="sender">The sender object</param>
		/// <param name="e">The event arguments</param>
		protected void DownButtonOnClick(object sender, EventArgs e)
		{
			try
			{
				int itemIndex = this.selectedCommandsListBox.Items.Count - 1;
				int beginIndex = 0;
				int endIndex = 0;
				bool beginFound = false;
				bool endFound = false;
				while (itemIndex >= 0)
				{
					if (!beginFound && this.selectedCommandsListBox.Items[itemIndex].Selected)
					{
						beginIndex = itemIndex;
						beginFound = true;
					}

					if (!endFound && beginFound)
					{
						if (!this.selectedCommandsListBox.Items[itemIndex].Selected)
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
						if (beginIndex < this.selectedCommandsListBox.Items.Count - 1)
						{
							var endItem = new ListItem(
								this.selectedCommandsListBox.Items[beginIndex + 1].Text,
								this.selectedCommandsListBox.Items[beginIndex + 1].Value);
							for (int index = beginIndex; index >= endIndex; index--)
							{
								this.selectedCommandsListBox.Items[index + 1].Text = this.selectedCommandsListBox.Items[index].Text;
								this.selectedCommandsListBox.Items[index + 1].Value = this.selectedCommandsListBox.Items[index].Value;
								this.selectedCommandsListBox.Items[index + 1].Selected = this.selectedCommandsListBox.Items[index].Selected;
							}

							this.selectedCommandsListBox.Items[endIndex].Text = endItem.Text;
							this.selectedCommandsListBox.Items[endIndex].Value = endItem.Value;
							this.selectedCommandsListBox.Items[endIndex].Selected = endItem.Selected;
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
		///    Executes when the page is loaded.  Disables command buttons if security requirements are not satisfied.
		///    Populates the toolbar type dropdown list with the available custom toolbar types.
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

					if (entityAssigned || !this.Security.HasRight(RIGHT.MODIFY_SITES_AND_SITE_GROUPS))
					{
						this.EnableControls(false);
					}

					// Populate the ToolbarTypeDropDownList
					var customToolbarTypes = FMChannelHelper.MakeCall<ICustomToolbars, List<CustomToolbarType>>(
						customToolbars => customToolbars.EnumerateToolbarTypes(this.Security));

					foreach (CustomToolbarType toolbarType in customToolbarTypes)
					{
						string combinedValue = toolbarType.Id + StringSeparator + toolbarType.LookupIndex.ToString();
						var item = new ListItem(this.GetTranslatedText(toolbarType.Id), combinedValue);
						this.toolbarTypeDropDownList.Items.Add(item);
					}

					this.ToolbarTypeDropDownListSelectedIndexChanged(null, null);
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		/// <summary>
		///    Moves the selected list of commands from the available commands list box to the selected commands list box.
		/// </summary>
		/// <param name="sender">The sender object</param>
		/// <param name="e">The event arguments</param>
		protected void SelectCommandButtonOnClick(object sender, EventArgs e)
		{
			try
			{
				ListItem availableCommandItem;
				while ((availableCommandItem = this.availableCommandsListBox.SelectedItem) != null)
				{
					this.availableCommandsListBox.Items.Remove(availableCommandItem);
					availableCommandItem.Selected = false;
					this.selectedCommandsListBox.Items.Add(availableCommandItem);
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		/// <summary>
		///    Executed when the toolbar type dropdown list selection changes.  Populates the available and selected command list
		///    boxes based on the saved configuration of the selected custom toolbar type.  If no saved configuration exist then
		///    the selected command list box will be empty and the available command list box will contain all available commands.
		/// </summary>
		/// <param name="sender">The sender object</param>
		/// <param name="e">The event arguments</param>
		protected void ToolbarTypeDropDownListSelectedIndexChanged(object sender, EventArgs e)
		{
			try
			{
				this.selectedCommandsListBox.Items.Clear();
				this.availableCommandsListBox.Items.Clear();

				if (this.toolbarTypeDropDownList.SelectedIndex == -1)
				{
					return;
				}

				var dispatchConfigGuid = (Guid)this.Session["DispatchConfigGuid"];
				var customToolbar = new CustomToolbarClass { DispatchConfigurationGuid = dispatchConfigGuid };

				// Get the Custom Toolbar associated with the selected toolbar type
				// Extract the ID and ToolbarType from the combined list item value field
				char[] seperator = { StringSeparator };
				string[] values = this.toolbarTypeDropDownList.SelectedItem.Value.Split(seperator);
				customToolbar.ID = values[0];
				customToolbar.ToolbarType = Convert.ToInt32(values[1]);
				
				FMChannelHelper.MakeCall<ICustomToolbars>(
					customToolbars =>
					{
						Guid customToolbarGuid = customToolbars.GetIdentityGuidById(
							this.Security, customToolbar.ID, customToolbar.DispatchConfigurationGuid);

						if (customToolbarGuid != Guid.Empty)
						{
							customToolbar = customToolbars.Get(this.Security, customToolbarGuid);
						}
					});

				this.Session["CustomToolbar"] = customToolbar;

				// Populate the Available Commands list box with the full set of toolbar commands
				// First populate with the commands associated with the toolbar type
				var toolbarCommandTypes = FMChannelHelper.MakeCall<ICustomToolbarCommands, List<CustomToolbarCommandType>>(
					customToolbarCommands => customToolbarCommands.EnumerateCommandTypes(this.Security, customToolbar.ToolbarType));



				if (customToolbar.ToolbarCommandList.Count == 0)
				{
					//get sorted defaut commands
					var defaultCommands =
						toolbarCommandTypes.FindAll(x => x.IsDefault).OrderBy(x => x.DefaultOrder.HasValue ? x.DefaultOrder.Value : -1);


					//get the defaults
					customToolbar.ToolbarCommandList = new CustomToolbarCommandCollectionClass();
					customToolbar.ToolbarCommandList.AddRange(
						defaultCommands.Select(x => new CustomToolbarCommandClass() { ID = x.Id, ToolbarCommandType = x.LookupIndex }).ToList());
				}

				foreach (CustomToolbarCommandType commandType in toolbarCommandTypes)
				{
					string combinedValue = commandType.Id + StringSeparator + commandType.LookupIndex;
					var item = new ListItem(this.GetTranslatedText(commandType.Id), combinedValue);
					this.availableCommandsListBox.Items.Add(item);
				}

				// Also populate with the dispatch transaction aliases
				TransactionAliasNameCollectionClass aliasNames =
					FMChannelHelper.MakeCall<ITransactionAliases, TransactionAliasNameCollectionClass>(
							x =>
							x.EnumerateDispatchAliasNames(this.Security)
					);

				string transactionAliasCommandType = CustomToolbarCommandType.TransactionAliasCommandType.ToString();

				foreach (TransactionAliasNameClass aliasName in aliasNames)
				{
					string combinedValue = aliasName.AliasName + StringSeparator + transactionAliasCommandType + StringSeparator
											+ aliasName.MasterRecordGuid.ToString();
					var item = new ListItem(aliasName.AliasName + CustomToolbarCommandClass.TransactionAliasDesignator, combinedValue);
					this.availableCommandsListBox.Items.Add(item);
				}

				// Move the selected toolbar commands to the Selected Commands list box
				// The toolbar commands are ordered correctly when retrieved from the database
				foreach (CustomToolbarCommandClass selectedCommand in customToolbar.ToolbarCommandList)
				{
					string toolbarCommandType = selectedCommand.ToolbarCommandType.ToString();
					string combinedValue = selectedCommand.ID + StringSeparator + toolbarCommandType;
					string itemText;
					if (toolbarCommandType == transactionAliasCommandType)
					{
						combinedValue += StringSeparator + selectedCommand.TransactionAliasGuid.ToString();
						itemText = selectedCommand.ID + CustomToolbarCommandClass.TransactionAliasDesignator;
					}
					else
					{
						itemText = this.GetTranslatedText(selectedCommand.ID);
					}

					var selectedItem = new ListItem(itemText, combinedValue);
					this.availableCommandsListBox.Items.Remove(selectedItem);
					this.selectedCommandsListBox.Items.Add(selectedItem);
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		/// <summary>
		///    Moves the selected list of commands from the selected commands list box to the available commands list box.
		/// </summary>
		/// <param name="sender">The sender object</param>
		/// <param name="e">The event arguments</param>
		protected void UnselectCommandButtonOnClick(object sender, EventArgs e)
		{
			try
			{
				ListItem selectedCommandItem;
				while ((selectedCommandItem = this.selectedCommandsListBox.SelectedItem) != null)
				{
					this.selectedCommandsListBox.Items.Remove(selectedCommandItem);
					selectedCommandItem.Selected = false;

					// Insert the unselected item into the Available Commands List alphabetically
					bool itemInserted = false;
					foreach (ListItem availableItem in this.availableCommandsListBox.Items)
					{
						if (availableItem.Text.CompareTo(selectedCommandItem.Text) > 0)
						{
							int index = this.availableCommandsListBox.Items.IndexOf(availableItem);
							this.availableCommandsListBox.Items.Insert(index, selectedCommandItem);
							itemInserted = true;
							break;
						}
					}

					if (!itemInserted)
					{
						this.availableCommandsListBox.Items.Add(selectedCommandItem);
					}
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		/// <summary>
		///    Moves the selected list of commands up one row in the selected commands list box.
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
				while (itemIndex < this.selectedCommandsListBox.Items.Count)
				{
					if (!beginFound && this.selectedCommandsListBox.Items[itemIndex].Selected)
					{
						beginIndex = itemIndex;
						beginFound = true;
					}

					if (!endFound && beginFound)
					{
						if (!this.selectedCommandsListBox.Items[itemIndex].Selected)
						{
							endIndex = itemIndex - 1;
							endFound = true;
						}
						else if (itemIndex == this.selectedCommandsListBox.Items.Count - 1)
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
								this.selectedCommandsListBox.Items[beginIndex - 1].Text,
								this.selectedCommandsListBox.Items[beginIndex - 1].Value);
							for (int index = beginIndex; index <= endIndex; index++)
							{
								this.selectedCommandsListBox.Items[index - 1].Text = this.selectedCommandsListBox.Items[index].Text;
								this.selectedCommandsListBox.Items[index - 1].Value = this.selectedCommandsListBox.Items[index].Value;
								this.selectedCommandsListBox.Items[index - 1].Selected = this.selectedCommandsListBox.Items[index].Selected;
							}

							this.selectedCommandsListBox.Items[endIndex].Text = endItem.Text;
							this.selectedCommandsListBox.Items[endIndex].Value = endItem.Value;
							this.selectedCommandsListBox.Items[endIndex].Selected = endItem.Selected;
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
			this.selectCommandButton.Enabled = enable;
			this.unselectCommandButton.Enabled = enable;
			this.applyButton.Enabled = enable;
		}

		#endregion
	}
}