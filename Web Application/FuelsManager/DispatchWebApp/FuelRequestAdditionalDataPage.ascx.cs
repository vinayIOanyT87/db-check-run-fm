// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FuelRequestAdditionalDataPage.ascx.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
// Represents the Additional Data tab on the Fuel Request Form. 
// The Additional Data tab contains the line item user data fields 
// configured for the transaction alias which corresponds to the type of 
// request. The fields display as either text boxes or combo boxes depending
// on the user data field configuration.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FuelsManager.DispatchWebApp
{
	using System;
	using System.Collections.Generic;
	using System.Globalization;
	using System.Web.UI.WebControls;

	using FMBusinessObjects.DataObjects;

	using FMControls;

	/// <summary>
	/// <para>
	/// Represents the Additional Data tab on the Fuel Request Form. 
	/// The Additional Data tab contains the line item user data fields 
	/// configured for the transaction alias which corresponds to the type of 
	/// request. The fields display as either text boxes or combo boxes depending
	/// on the user data field configuration.
	/// </para> 
	/// <para>
	/// The tab also contains some other tidbits of data, like the ID of the FuelsManager
	/// transaction record.
	/// </para>
	/// </summary>
	public partial class FuelRequestAdditionalDataPage : FuelRequestFormPageBase
	{
		#region Page Properties

		/// <summary>
		/// The issue point currently displayed on the Additional Data tab
		/// </summary>
		public string IssuePoint
		{
			get
			{
				return this.IssuePointNumberTextBox.Text;
			}

			set
			{
				this.IssuePointNumberTextBox.Text = value;
			}
		}

		/// <summary>
		/// The issue point number currently displayed on the Additional data
		/// </summary>
		public string IssuePointNumber
		{
			get
			{
				return this.IssuePointTextBox.Text;
			}

			set
			{
				this.IssuePointTextBox.Text = value;
			}
		}

		/// <summary>
		/// Represents the 24 sets of user data controls displayed on the tab page
		/// </summary>
		private List<UserDataControlSet> UserDataControls { get; set; }

		#endregion

		#region Page Methods

		/// <summary>
		/// Use the transaction alias line item user data field configuration to determine whether to and how to display
		/// the user data controls on the form. A user data control can be displayed as text box or combo box. 
		/// </summary>
		public void SetUserDataControls()
		{
			if (this.UserDataControls != null)
			{
				// Disable all of the user data controls
				foreach (UserDataControlSet userDataControlSet in this.UserDataControls)
				{
					userDataControlSet.Disable();
				}

				// Get the line item user data fields configured for the transaction alias which corresponds to the type of request
				UserDataFieldCollectionClass userDataFields = FuelRequestFormSession.SessionTransactionAlias.LineItemUserDataFieldCollection;

				// If there are no line item user data fields configured for the transaction alias, show a 
				// label that indicates that to the user
				this.NoUserDataFieldsLabel.Visible = userDataFields.Count == 0;

				foreach (UserDataFieldClass lineItemUserDataField in userDataFields)
				{
					// Set each user data control set to be either a text box or a combo box depending on the user data field configuration
					if (lineItemUserDataField.UserDataType == USER_DATA_TYPE.TEXT)
					{
						this.UserDataControls[lineItemUserDataField.Number].SetToTextBoxMode(lineItemUserDataField.DisplayName);
					}
					else
					{
						this.UserDataControls[lineItemUserDataField.Number].SetToComboBoxMode(lineItemUserDataField.DisplayName, lineItemUserDataField.UserDataListValueCollection);
					}
				}
			}
		}

		#endregion

		#region Page Events

		/// <summary>
		/// Fires when the page loads. If it's not a post back, display the transaction
		/// </summary>
		/// <param name="sender">The parameter is not used.</param>
		/// <param name="e">The parameter is not used.</param>
		protected void Page_Load(object sender, EventArgs e)
		{
			try
			{
				// Set all of our user data control groups so we can more easily manipulate them
				this.UserDataControls = new List<UserDataControlSet>()
					{
						new UserDataControlSet(this.UserData1Label, this.UserData1TextBox, this.UserData1ComboBoxPanel, this.UserData1ComboBox),
						new UserDataControlSet(this.UserData2Label, this.UserData2TextBox, this.UserData2ComboBoxPanel, this.UserData2ComboBox),
						new UserDataControlSet(this.UserData3Label, this.UserData3TextBox, this.UserData3ComboBoxPanel, this.UserData3ComboBox),
						new UserDataControlSet(this.UserData4Label, this.UserData4TextBox, this.UserData4ComboBoxPanel, this.UserData4ComboBox),
						new UserDataControlSet(this.UserData5Label, this.UserData5TextBox, this.UserData5ComboBoxPanel, this.UserData5ComboBox),
						new UserDataControlSet(this.UserData6Label, this.UserData6TextBox, this.UserData6ComboBoxPanel, this.UserData6ComboBox),
						new UserDataControlSet(this.UserData7Label, this.UserData7TextBox, this.UserData7ComboBoxPanel, this.UserData7ComboBox),
						new UserDataControlSet(this.UserData8Label, this.UserData8TextBox, this.UserData8ComboBoxPanel, this.UserData8ComboBox),
						new UserDataControlSet(this.UserData9Label, this.UserData9TextBox, this.UserData9ComboBoxPanel, this.UserData9ComboBox),
						new UserDataControlSet(this.UserData10Label, this.UserData10TextBox, this.UserData10ComboBoxPanel, this.UserData10ComboBox),
						new UserDataControlSet(this.UserData11Label, this.UserData11TextBox, this.UserData11ComboBoxPanel, this.UserData11ComboBox),
						new UserDataControlSet(this.UserData12Label, this.UserData12TextBox, this.UserData12ComboBoxPanel, this.UserData12ComboBox),
						new UserDataControlSet(this.UserData13Label, this.UserData13TextBox, this.UserData13ComboBoxPanel, this.UserData13ComboBox),
						new UserDataControlSet(this.UserData14Label, this.UserData14TextBox, this.UserData14ComboBoxPanel, this.UserData14ComboBox),
						new UserDataControlSet(this.UserData15Label, this.UserData15TextBox, this.UserData15ComboBoxPanel, this.UserData15ComboBox),
						new UserDataControlSet(this.UserData16Label, this.UserData16TextBox, this.UserData16ComboBoxPanel, this.UserData16ComboBox),
						new UserDataControlSet(this.UserData17Label, this.UserData17TextBox, this.UserData17ComboBoxPanel, this.UserData17ComboBox),
						new UserDataControlSet(this.UserData18Label, this.UserData18TextBox, this.UserData18ComboBoxPanel, this.UserData18ComboBox),
						new UserDataControlSet(this.UserData19Label, this.UserData19TextBox, this.UserData19ComboBoxPanel, this.UserData19ComboBox),
						new UserDataControlSet(this.UserData20Label, this.UserData20TextBox, this.UserData20ComboBoxPanel, this.UserData20ComboBox),
						new UserDataControlSet(this.UserData21Label, this.UserData21TextBox, this.UserData21ComboBoxPanel, this.UserData21ComboBox),
						new UserDataControlSet(this.UserData22Label, this.UserData22TextBox, this.UserData22ComboBoxPanel, this.UserData22ComboBox),
						new UserDataControlSet(this.UserData23Label, this.UserData23TextBox, this.UserData23ComboBoxPanel, this.UserData23ComboBox),
						new UserDataControlSet(this.UserData24Label, this.UserData24TextBox, this.UserData24ComboBoxPanel, this.UserData24ComboBox)
					};

				if (!this.Page.IsPostBack)
				{
					this.SetUserDataControls();
					this.DisplayTransaction(FuelRequestFormSession.SessionTransaction);
				}
			}
			catch (Exception ex)
			{
				this.ErrorHandler(ex);
			}
		}

		#endregion

		#region Transaction Record Display and Creation

		/// <summary>
		/// Use the controls on the form to display data from a FuelsManager transaction record.
		/// </summary>
		/// <param name="transaction">The transaction record to display</param>
		public void DisplayTransaction(TransactionDO transaction)
		{
			LineItemDO lineItem = transaction.LineItems.Find((matchingLineItem) => matchingLineItem.DeleteFlag == false);

			if (lineItem != null)
			{
				foreach (UserDataFieldClass lineItemUserDataField in FuelRequestFormSession.SessionTransactionAlias.LineItemUserDataFieldCollection)
				{
					string userDataValue = string.Empty;
					lineItem.UserData.TryGetValue(BaseTransactionLineItemDO.UserDataLineItemKeyPrefix + (lineItemUserDataField.Number + 1).ToString(), out userDataValue);
					this.UserDataControls[lineItemUserDataField.Number].DisplayValue(userDataValue);
				}
			}

			this.TransIDTextBox.Text = transaction.TransID;

			string serialNumberText = string.Empty;
			transaction.UserData.TryGetValue(TransactionDO.USER_DATA_KEY_04, out serialNumberText);
			this.SerialNumberTextBox.Text = serialNumberText;

			this.GrossGalTextBox.Text = transaction.Number03.HasValue ? transaction.Number03.ToString() : string.Empty;
			this.IssuePointTextBox.Text = transaction.IssuePoint;
			this.IssuePointNumberTextBox.Text = transaction.IssuePointNumber;
		}

		/// <summary>
		/// Set values in a FuelsManager transaction record using data from the controls on the page
		/// </summary>
		/// <param name="transaction">The FuelsManager Transaction record to populate with data</param>
		public void SaveTransactionData(TransactionDO transaction)
		{
			LineItemDO transactionLineItem = transaction.LineItems.Find((lineItem) => lineItem.DeleteFlag == false);

			for (int userDataFieldNumber = 0; userDataFieldNumber < this.UserDataControls.Count; userDataFieldNumber++)
			{
				if (this.UserDataControls[userDataFieldNumber].Enabled)
				{
					transactionLineItem.UserData[BaseTransactionLineItemDO.UserDataLineItemKeyPrefix + (userDataFieldNumber + 1).ToString(CultureInfo.InvariantCulture)] = this.UserDataControls[userDataFieldNumber].GetValue();
				}
			}

			double convertedGrossGallons = 0;

			if (double.TryParse(this.GrossGalTextBox.Text, out convertedGrossGallons))
			{
				transaction.Number03 = convertedGrossGallons;
			}
			else
			{
				transaction.Number03 = null;
			}

			transaction.UserData4 = this.SerialNumberTextBox.Text;
			transaction.IssuePoint = this.IssuePointTextBox.Text;
			transaction.IssuePointNumber = this.IssuePointNumberTextBox.Text;
		}

		#endregion

		/// <summary>
		/// Represents a related group of controls that are used to display a user data field 
		/// </summary>
		private class UserDataControlSet
		{
			/// <summary>
			/// The label, which displays the name assigned to the user data field
			/// </summary>
			private readonly FMLabel LabelControl = null;

			/// <summary>
			/// When the user data field is a text field, the text box displays the value of the field
			/// </summary>
			private readonly FMTextBox TextBoxControl = null;

			/// <summary>
			/// The panel which contains the combo box, we either hide or display the panel and the combo box
			/// depending on the type of user data field
			/// </summary>
			private readonly Panel ComboBoxPanel = null;

			/// <summary>
			/// When the user data field is a list field, the combo box displays the values that can be assigned to the field
			/// </summary>
			private readonly FMComboBox ComboBox = null;

			/// <summary>
			/// Create a set of user data controls using the specified label, text box, and combo box.
			/// </summary>
			/// <param name="label">The label control</param>
			/// <param name="textBox">The text box control</param>
			/// <param name="panel">The panel control which contains the combo box</param>
			/// <param name="comboBox">The combo box control</param>
			public UserDataControlSet(FMLabel label, FMTextBox textBox, Panel panel, FMComboBox comboBox)
			{
				this.LabelControl = label;
				this.TextBoxControl = textBox;
				this.ComboBoxPanel = panel;
				this.ComboBox = comboBox;
			}

			/// <summary>
			/// True if either the combo box or text box is being displayed
			/// </summary>
			public bool Enabled
			{
				get
				{
					return this.TextBoxControl.Visible || this.ComboBox.Visible;
				}
			}

			/// <summary>
			/// Display a configured user data field as a text box.
			/// </summary>
			/// <param name="displayName">The text to display on the label, i.e, the name of the user data field</param>
			public void SetToTextBoxMode(string displayName)
			{
				this.LabelControl.Text = displayName + ":";

				this.LabelControl.Visible = true;
				this.TextBoxControl.Visible = true;
				this.ComboBoxPanel.Visible = false;
			}

			/// <summary>
			/// Display a configured user data field as a combo box. The combo box will contain the list values configured
			/// for the user data field
			/// </summary>
			/// <param name="displayName">The text to display on the label, i.e, the name of the user data field</param>
			/// <param name="userDataListValues">The values that should be displayed in the combo box</param>
			public void SetToComboBoxMode(string displayName, UserDataListValueCollectionClass userDataListValues)
			{
				this.LabelControl.Text = displayName + ":";

				this.LabelControl.Visible = true;
				this.TextBoxControl.Visible = false;
				this.ComboBoxPanel.Visible = true;

				string previouslySelectedComboBoxText = string.Empty;

				// If a value is selected, remember the value selected before we reload the records to display 
				// in the list so that we can select the same value again.
				if (this.ComboBox.SelectedItem != null && !string.IsNullOrEmpty(this.ComboBox.SelectedItem.Text))
				{
					previouslySelectedComboBoxText = this.ComboBox.SelectedItem.Text;
				}

				this.ComboBox.SelectedIndex = -1;
				this.ComboBox.DataSource = userDataListValues;
				this.ComboBox.DataBind();

				// Add a blank value to the list to represent no selection
				this.ComboBox.Items.Insert(0, new ListItem(string.Empty, string.Empty));

				if (!string.IsNullOrEmpty(previouslySelectedComboBoxText))
				{
					this.ComboBox.SelectByText(previouslySelectedComboBoxText);
				}
				else
				{
					this.ComboBox.SelectedValue = string.Empty;
				}
			}

			/// <summary>
			/// Disable a user data field, i.e. don't display the controls.
			/// We do this when there is no user data field configured for the transaction alias 
			/// </summary>
			public void Disable()
			{
				this.LabelControl.Visible = false;
				this.TextBoxControl.Visible = false;
				this.ComboBoxPanel.Visible = false;
			}

			/// <summary>
			/// Display the specified value in the user data control
			/// </summary>
			/// <param name="valueToDisplay">The value to display</param>
			public void DisplayValue(string valueToDisplay)
			{
				if (this.TextBoxControl.Visible)
				{
					this.TextBoxControl.Text = valueToDisplay;
				}
				else
				{
					this.ComboBox.SelectedValue = valueToDisplay;
				}
			}

			/// <summary>
			/// Retrieve the value currently displayed in the user data control
			/// </summary>
			/// <returns>The value currently displayed in the user data control</returns>
			public string GetValue()
			{
				if (this.TextBoxControl.Visible)
				{
					return this.TextBoxControl.Text;
				}
				else
				{
					if (this.ComboBox.SelectedItem != null)
					{
						return this.ComboBox.SelectedItem.Value;
					}
					else
					{
						return string.Empty;
					}
				}
			}
		}
	}
}