namespace FuelsManager.Accounting
{
	using System;
	using System.Collections;
	using System.Globalization;
	using System.Web.UI.WebControls;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.Constants;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.ServiceRequests;

	public partial class GeneralConfiguration : AccountingAutoSubmitWebFormView
	{
		#region private attributes
		private GeneralConfigDO generalConfigDO;
		private GeneralConfigSR generalConfigSR;
		#endregion

		#region Page load methods.
		/// <summary>
		/// This is the default constructor for the general configuration page code behind.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void Page_Load(object sender, EventArgs e)
		{
			try
			{
				if (this.Page.IsPostBack == false)
				{
					this.PerformDataDictionary();
					bool successful = this.LoadConfigurationData();

					// Ensure that the data was retrieved successfully.
					if (successful)
					{
						this.Session.Add(PageSessionKeyConstants.ACCOUNTING_GENERAL_PAGE_DATA_OBJECT, this.generalConfigDO);
						this.BindModes();
						this.BindForcedCloseout();
						this.BindExstars();
						this.BindAdjustment();
					}

					if (FMChannelHelper.MakeCall<IHardwareKey, bool>(x => x.IsADFKey()))
					{
						this.ReverseTrxDateLabel.Visible = false;
						this.ReverseTrxDateRadioButtonList.Visible = false;
					}

					if (FMChannelHelper.MakeCall<IHardwareKey, bool>(x => x.IsDescKey()))
					{
						this.ReverseTrxDateLabel.Visible = false;
						this.ReverseTrxDateRadioButtonList.Visible = false;
					}

				}
				else
				{
					this.generalConfigDO = this.Page.Session[PageSessionKeyConstants.ACCOUNTING_GENERAL_PAGE_DATA_OBJECT] as GeneralConfigDO;

					if (this.generalConfigDO == null)
					{
						const string ErrMsg = "Cannot find GeneralConfigDO in session.";
						this.ErrorHandler(new Exception(ErrMsg));
					}
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}
		#endregion

		#region Load data methods
		/// <summary>
		/// This method will set the data dictionary for all labels, buttons, checkboxes, and radio controls.
		/// </summary>
		private void PerformDataDictionary()
		{
			// Data Dictionary the labels.
			string newText = GetDataDictionaryValueByKey(this.security.SiteGuid, this.AdjustmentLabel.Text);

			this.AdjustmentLabel.Text = newText;

			string stripped = this.AdjustmentMethodsLabel.Text.Remove(this.AdjustmentMethodsLabel.Text.Length - 1, 1);
			newText = GetDataDictionaryValueByKey(this.security.SiteGuid, stripped);
			this.AdjustmentMethodsLabel.Text = newText + ":";

			newText = GetDataDictionaryValueByKey(this.security.SiteGuid, this.AssignedLabel.Text);
			this.AssignedLabel.Text = newText;

			stripped = this.AuthorizationCodeLabel.Text.Remove(this.AuthorizationCodeLabel.Text.Length - 1, 1);
			newText = GetDataDictionaryValueByKey(this.security.SiteGuid, stripped);
			this.AuthorizationCodeLabel.Text = newText + ":";

			stripped = this.ReverseTrxDateLabel.Text.Remove(this.ReverseTrxDateLabel.Text.Length - 1, 1);
			newText = GetDataDictionaryValueByKey(this.security.SiteGuid, stripped);
			this.ReverseTrxDateLabel.Text = newText + ":";

			newText = GetDataDictionaryValueByKey(this.security.SiteGuid, this.ExstarsLabel.Text);
			this.ExstarsLabel.Text = newText;

			stripped = this.ForceDateLabel.Text.Remove(this.ForceDateLabel.Text.Length - 1, 1);
			newText = GetDataDictionaryValueByKey(this.security.SiteGuid, stripped);
			this.ForceDateLabel.Text = newText + ":";

			newText = GetDataDictionaryValueByKey(this.security.SiteGuid, this.GeneralConfigTitleLabel.Text);
			this.GeneralConfigTitleLabel.Text = newText;

			newText = GetDataDictionaryValueByKey(this.security.SiteGuid, this.GeneralLabel.Text);
			this.GeneralLabel.Text = newText;

			stripped = this.SecurityCodeLabel.Text.Remove(this.SecurityCodeLabel.Text.Length - 1, 1);
			newText = GetDataDictionaryValueByKey(this.security.SiteGuid, stripped);
			this.SecurityCodeLabel.Text = newText + ":";

			newText = GetDataDictionaryValueByKey(this.security.SiteGuid, this.UnassignedLabel.Text);
			this.UnassignedLabel.Text = newText;

			// Data Dictionary the buttons.
			newText = GetDataDictionaryValueByKey(this.security.SiteGuid, this.AssignButton.Text);
			this.AssignButton.Text = newText;

			newText = GetDataDictionaryValueByKey(this.security.SiteGuid, this.UnassignButton.Text);
			this.UnassignButton.Text = newText;

			newText = GetDataDictionaryValueByKey(this.security.SiteGuid, this.OK.Text);
			this.OK.Text = newText;

			newText = GetDataDictionaryValueByKey(this.security.SiteGuid, this.CancelButton.Text);
			this.CancelButton.Text = newText;

			// Data Dictionary the checkboxes.
			newText = GetDataDictionaryValueByKey(this.security.SiteGuid, this.SetBeginInvCheckBox.Text);
			this.SetBeginInvCheckBox.Text = newText;

			// Data Dictionary the checkboxes.
			stripped = this.ShowDeletedCheckBox.Text.Remove(this.ShowDeletedCheckBox.Text.Length - 1, 1);
			newText = GetDataDictionaryValueByKey(this.security.SiteGuid, stripped);
			this.ShowDeletedCheckBox.Text = newText + "?";

			stripped = this.ConsortiumCheckBox.Text.Remove(this.ConsortiumCheckBox.Text.Length - 1, 1);
			newText = GetDataDictionaryValueByKey(this.security.SiteGuid, stripped);
			this.ConsortiumCheckBox.Text = newText + "?";

			// Data dictionary the radio buttons.
			DataDictionaryRadioList(this.ReverseTrxDateRadioButtonList);
			DataDictionaryRadioList(this.MethodsRadioButtonList);

		}


		private void DataDictionaryRadioList(RadioButtonList rdoButtonList)
		{
			ListItemCollection listItems = rdoButtonList.Items;
			foreach (ListItem listItem in listItems)
			{
				listItem.Text = GetDataDictionaryValueByKey(this.security.SiteGuid, listItem.Text);
			}
		}


		/// <summary>
		/// This method will retrieve the general configuration data from the database for a given site.
		/// </summary>
		private bool LoadConfigurationData()
		{
			bool successful = true;

			try
			{
				this.generalConfigSR = new GeneralConfigSR
				{
					Request = GeneralConfigSR.GeneralConfigurationRequests.GET_CONFIGURATION,
					Security = this.security,
					SiteGuid = this.security.SiteGuid
				};

				this.generalConfigDO = FMChannelHelper.MakeCall<IGeneralConfigProcessor, GeneralConfigDO>(x => x.Get(this.generalConfigSR));

				// If there were no configuration entries in the database for this site, then
				// the site Guid in the DO will be empty. Initialize it to the current site.
				if (this.generalConfigDO.SiteGuid == Guid.Empty)
				{
					this.generalConfigDO.SiteGuid = this.security.SiteGuid;
				}
			}
			catch (Exception ex)
			{
				successful = false;
				this.DisplayErrorDialog("Error in retrieving general configuration data! " + ex.Message);
			}

			return successful;
		}
		#endregion

		#region Bind Methods
		/// <summary>
		/// This method will bind the modes to the configured data in the
		/// database.
		/// </summary>
		private void BindModes()
		{
			this.ShowDeletedCheckBox.Checked = this.generalConfigDO.ShowDeletedTransactions;
			this.SetBeginInvCheckBox.Checked = this.generalConfigDO.SetBeginInventoryToZeroFlag;

			// Ensure there is a valid value for the current session mode.
			if (!string.IsNullOrEmpty(this.generalConfigDO.ReverseTransactionDateMode))
			{
				if ((this.generalConfigDO.ReverseTransactionDateMode == "Current") ||
					(this.generalConfigDO.ReverseTransactionDateMode == "Original"))
				{
					this.ReverseTrxDateRadioButtonList.SelectedValue = this.generalConfigDO.ReverseTransactionDateMode;
				}
			}
		}

		/// <summary>
		/// This method will bind the force closeout days and set the selection
		/// to the configured value in the database.
		/// </summary>
		private void BindForcedCloseout()
		{
			ArrayList numberOfDays = new ArrayList();

			string disabledText = GetDataDictionaryValueByKey(this.security.SiteGuid, "Disabled");

			var item = new ListItem(disabledText, "0");
			numberOfDays.Add(item);

			for (int days = 1; days <= 180; days++)
			{
				string aDay;
				if (days == 1)
				{
					aDay = "  1 day";
				}
				else
				{
					aDay = string.Format("{0,3} days", days);
				}

				numberOfDays.Add(new ListItem(aDay, days.ToString()));
			}

			this.NumOfDaysDropdown.DataSource = numberOfDays;
			this.NumOfDaysDropdown.DataBind();

			string selectedValue = FMChannelHelper.MakeCall<IDataDictionariesClass, string>(
				x =>
				x.Get(this.security.SiteGuid, this.generalConfigDO.ForceCloseoutString)
			);

			this.NumOfDaysDropdown.SelectedValue = selectedValue;
		}

		/// <summary>
		/// This method will bind the ExSTARS configuration from the database.
		/// </summary>
		private void BindExstars()
		{
			this.SecurityCodeTextBox.Text = this.generalConfigDO.SecurityCode;
			this.AuthorizationCodeTextBox.Text = this.generalConfigDO.AuthorizationCode;
		}

		/// <summary>
		/// This method will bind all the data associated to the adjustment distribution
		/// configuration.
		/// </summary>
		private void BindAdjustment()
		{
			this.ConsortiumCheckBox.Checked = this.generalConfigDO.UseConsortium;
			this.MethodsRadioButtonList.SelectedValue = this.generalConfigDO.AdjustmentMethodString;

			// Build an array of value pair of the alias name and transaction type IDs. Use
			// this array to bind to the assigned list box.
			var bindingObjects = new ArrayList();

			foreach (GeneralConfigAlias genConfigAlias in this.generalConfigDO.AdjustmentAliasList)
			{
				var pair = new DropdownValuePairDO
				{
					Text = genConfigAlias.AliasName,
					TextValue = genConfigAlias.TransactionAliasGuid.ToString()
				};
				bindingObjects.Add(pair);
			}

			// Bind to the assigned list box.
			this.AssignedListBox.DataSource = bindingObjects;
			this.AssignedListBox.DataTextField = "Text";
			this.AssignedListBox.DataValueField = "TextValue";
			this.AssignedListBox.DataBind();

			// Bind to the unassigned list box.
			this.UnassignedListBox.DataSource = this.generalConfigDO.UnassignedAliasList;
			this.UnassignedListBox.DataTextField = "Text";
			this.UnassignedListBox.DataValueField = "TextValue";
			this.UnassignedListBox.DataBind();

			// Disable assigned/unassigned list boxes and buttons if the method is not throughput.
			if (this.generalConfigDO.AdjustmentMethod == GeneralConfigSR.GeneralConfigAdjustMethod.THROUGHPUT)
			{
				this.AssignedListBox.Enabled = true;
				this.UnassignedListBox.Enabled = true;
				this.AssignButton.Enabled = true;
				this.UnassignButton.Enabled = true;
				this.ConsortiumCheckBox.Enabled = true;
			}
			else
			{
				this.AssignedListBox.Enabled = false;
				this.UnassignedListBox.Enabled = false;
				this.AssignButton.Enabled = false;
				this.UnassignButton.Enabled = false;
				this.ConsortiumCheckBox.Checked = false;
				this.ConsortiumCheckBox.Enabled = false;
			}
		}
		#endregion

		#region Event Methods
		/// <summary>
		/// This method handles the Okay button on click event. It will retrieve all the data
		/// from the page and update the general configuration data object with the new data.
		/// It will then attempt to either create a new entry or update an existing one. There
		/// should only be one record per site.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void OkBtnOnClick(object sender, EventArgs e)
		{
			this.generalConfigDO = (GeneralConfigDO)this.Session[PageSessionKeyConstants.ACCOUNTING_GENERAL_PAGE_DATA_OBJECT];

			// There should always be a general configuration data object present. If not, then
			// display an error dialog.
			if (this.generalConfigDO == null)
			{
				this.DisplayErrorDialog("Data Object is missing. Cannot Save!");
			}
			else
			{
				this.generalConfigDO.ShowDeletedTransactions = this.ShowDeletedCheckBox.Checked;
				this.generalConfigDO.SetBeginInventoryToZeroFlag = this.SetBeginInvCheckBox.Checked;
				this.generalConfigDO.ReverseTransactionDateMode = this.ReverseTrxDateRadioButtonList.SelectedValue;
				this.generalConfigDO.ForceCloseoutString = this.NumOfDaysDropdown.SelectedValue;
				this.generalConfigDO.SecurityCode = this.SecurityCodeTextBox.Text;
				this.generalConfigDO.AuthorizationCode = this.AuthorizationCodeTextBox.Text;
				this.generalConfigDO.UseConsortium = this.ConsortiumCheckBox.Checked;
				this.generalConfigDO.AdjustmentMethodString = this.MethodsRadioButtonList.SelectedValue;
				this.generalConfigDO.UpdatedBy = this.security.UserID;

				string value = GetDataDictionaryValueByKey(this.security.SiteGuid, "Disabled");

				if (this.generalConfigDO.ForceCloseoutString == value)
				{
					this.generalConfigDO.ForceCloseoutString = "Disabled";
				}

				this.HandleListBoxData();

				// Create request to save the data and attempt to save.
				try
				{
					this.generalConfigSR = new GeneralConfigSR
					{
						Security = this.security,
						GeneralConfigurationDO = this.generalConfigDO,
						Request = GeneralConfigSR.GeneralConfigurationRequests.SAVE_CONFIGURATION,
						SiteGuid = this.security.SiteGuid
					};

					FMChannelHelper.MakeCall<IGeneralConfigProcessor>(x => x.Save(this.generalConfigSR));

					if (this.LoadConfigurationData())
					{
						this.Session.Add(PageSessionKeyConstants.ACCOUNTING_GENERAL_PAGE_DATA_OBJECT, this.generalConfigDO);
					}
				}
				catch (Exception ex)
				{
					this.DisplayErrorDialog("Error saving General Configuration data! " + ex.Message);
				}
			}
		}

		/// <summary>
		/// This method handles the new addition to the assigned list box and the removal of items
		/// from the list box. It updates the general configuration alias data objects appropriately.
		/// </summary>
		private void HandleListBoxData()
		{
			// This loop detects unassigned items
			foreach (GeneralConfigAlias genConfigAliasDO in this.generalConfigDO.AdjustmentAliasList)
			{
				bool found = false;
				Guid origAliasGuid = genConfigAliasDO.TransactionAliasGuid;

				foreach (ListItem assignedItem in this.AssignedListBox.Items)
				{
					Guid newAliasGuid = Guid.Parse(assignedItem.Value);

					if (origAliasGuid == newAliasGuid)
					{
						found = true;
						break;
					}
				}

				if (found == false)
				{
					genConfigAliasDO.DeleteFlag = true;
				}
				else
				{
					genConfigAliasDO.UpdatedBy = this.security.UserID;
				}
			}

			// This loop detects newly assigned items
			foreach (ListItem assignedItem in this.AssignedListBox.Items)
			{
				bool found = false;
				Guid newAliasGuid = Guid.Parse(assignedItem.Value);

				foreach (GeneralConfigAlias genConfigAliasDO in this.generalConfigDO.AdjustmentAliasList)
				{
					Guid origAliasGuid = genConfigAliasDO.TransactionAliasGuid;

					if (origAliasGuid == newAliasGuid)
					{
						found = true;
						break;
					}
				}

				if (found == false)
				{
					var newGenConfigDO = new GeneralConfigAlias
					{
						GeneralConfigurationGuid = this.generalConfigDO.GeneralConfigurationGuid,
						UpdatedBy = this.security.UserID,
						CreatedBy = this.security.UserID,
						TransactionAliasGuid = newAliasGuid
					};

					this.generalConfigDO.AdjustmentAliasList.Add(newGenConfigDO);
				}
			}
		}

		/// <summary>
		/// This method handles the cancel button on click event.  It transfers control
		/// to the configuration splash page.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void CancelBtnOnClick(object sender, EventArgs e)
		{
			this.Redirect("../FMWebApp/ConfigurationForm.aspx");
		}

		/// <summary>
		/// This method handles the assign button on click event. It will move a selected item
		/// from the unassigned list box to the assigned list box.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void AssignBtnOnClick(object sender, EventArgs e)
		{
			// Ensure that an item is selected. If not, then display a message.
			if (string.IsNullOrEmpty(this.UnassignedListBox.SelectedValue))
			{
				this.DisplayErrorDialog("Must select an item in the unassigned list!");
			}
			else
			{
				// Get the selected item information from the unassigned list box.
				ListItem unassignedListItem = this.UnassignedListBox.SelectedItem;
				var unassignedCollection = this.UnassignedListBox.Items;
				int selectedIndex = this.UnassignedListBox.SelectedIndex;

				// Add the unassigned list item to the assigned collection and remove the unassigned from
				// the unassigned list box.
				ListItemCollection assignedCollection = this.AssignedListBox.Items;
				assignedCollection.Add(unassignedListItem);
				unassignedCollection.RemoveAt(selectedIndex);

				// Clear the previous selections.
				this.UnassignedListBox.ClearSelection();
				this.AssignedListBox.ClearSelection();
			}
		}

		/// <summary>
		/// This method handles the unassign button on click event. It will move a selected item
		/// from the assigned list box to the unassigned list box.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void UnassignBtnOnClick(object sender, EventArgs e)
		{
			// Ensure that an item is selected. If not, then display a message.
			if (string.IsNullOrEmpty(this.AssignedListBox.SelectedValue))
			{
				this.DisplayErrorDialog("Must select an item in the assigned list!");
			}
			else
			{
				// Get the selected item information from the assigned list box.
				ListItem assignedListItem = this.AssignedListBox.SelectedItem;
				ListItemCollection assignedCollection = this.AssignedListBox.Items;
				int selectedIndex = this.AssignedListBox.SelectedIndex;

				// Add the assigned list item to the unassigned collection and remove the assigned from
				// the assigned list box.
				ListItemCollection unassignedCollection = this.UnassignedListBox.Items;
				unassignedCollection.Add(assignedListItem);
				assignedCollection.RemoveAt(selectedIndex);

				// Clear the previous selections.
				this.AssignedListBox.ClearSelection();
				this.UnassignedListBox.ClearSelection();
			}
		}

		/// <summary>
		/// This method handles the adjustment distribution method radio buttons being selected.
		/// If the selection is "throughput" then the assigned/unassigned list boxes and buttons
		/// should be available.  Otherwise, they should not.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void AdjMethodsOnClick(object sender, EventArgs e)
		{
			string selectedValue = this.MethodsRadioButtonList.SelectedValue;

			this.AssignedListBox.Enabled = false;
			this.UnassignedListBox.Enabled = false;
			this.AssignButton.Enabled = false;
			this.UnassignButton.Enabled = false;
			this.ConsortiumCheckBox.Checked = false;
			this.ConsortiumCheckBox.Enabled = false;

			if ((!string.IsNullOrEmpty(selectedValue)) && (selectedValue.ToUpper() == "THROUGHPUT"))
			{
				this.AssignedListBox.Enabled = true;
				this.UnassignedListBox.Enabled = true;
				this.AssignButton.Enabled = true;
				this.UnassignButton.Enabled = true;
				this.ConsortiumCheckBox.Enabled = true;
			}
		}
		#endregion

		#region Private Popup Error diaglog
		/// <summary>
		/// This method will display an error dialog informing the user of an error.
		/// </summary>
		/// <param name="errorMessage"></param>
		private void DisplayErrorDialog(string errorMessage)
		{
			string errMsg = "An Error has occurred!";

			if (!string.IsNullOrEmpty(errorMessage))
			{
				errMsg = errorMessage;
			}

			this.RenderErrorMessage(errMsg);
		}
		#endregion

		#region Web Form Designer generated code
		override protected void OnInit(EventArgs e)
		{
			base.CurrentSiteGuid = Guids.SiteAdminGuid;
			base.Initialize();
			base.OnInit(e);
		}
		#endregion
	}
}
