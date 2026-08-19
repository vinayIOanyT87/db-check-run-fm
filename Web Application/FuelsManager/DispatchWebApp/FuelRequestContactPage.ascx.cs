// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FuelRequestContactPage.ascx.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
// Represents the Contact tab on the Fuel Request Form. The Contact tab 
// contains information about the purchaser of the fuel and is only used for 
// Refuel and Defuel requests - it is not displayed for Fill Stand requests.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FuelsManager.DispatchWebApp
{
	using System;

	using FMBusinessObjects.DataObjects;

	/// <summary>
	/// Represents the Contact tab on the Fuel Request Form. The Contact tab 
	/// contains information about the purchaser of the fuel and is only used for 
	/// Refuel and Defuel requests - it is not displayed for Fill Stand requests.
	/// </summary>
	public partial class FuelRequestContactPage : FuelRequestFormPageBase
	{
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
				if (!this.Page.IsPostBack)
				{
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
		/// Use the controls on the page to display data from a FuelsManager transaction record.
		/// </summary>
		/// <param name="transaction">The transaction record to display</param>
		public void DisplayTransaction(TransactionDO transaction)
		{
			this.ContactTextBox.Text = transaction.ContactFirstName;
			this.PhoneTextBox.Text = transaction.ContactInfo;

			string addressText;
			transaction.UserData.TryGetValue(TransactionDO.USER_DATA_KEY_11, out addressText);
			this.AddressTextBox.Text = addressText;

			string emailText;
			transaction.UserData.TryGetValue(TransactionDO.USER_DATA_KEY_12, out emailText);
			this.EmailTextBox.Text = emailText;

			string cityText;
			transaction.UserData.TryGetValue(TransactionDO.USER_DATA_KEY_15, out cityText);
			this.CityTextBox.Text = cityText;

			string stateText;
			transaction.UserData.TryGetValue(TransactionDO.USER_DATA_KEY_17, out stateText);
			this.StateTextBox.Text = stateText;

			string zipText;
			transaction.UserData.TryGetValue(TransactionDO.USER_DATA_KEY_22, out zipText);
			this.ZipTextBox.Text = zipText;

			string memoText;
			transaction.UserData.TryGetValue(TransactionDO.USER_DATA_KEY_23, out memoText);
			this.MemoTextBox.Text = memoText;

			string faxText;
			transaction.UserData.TryGetValue(TransactionDO.USER_DATA_KEY_24, out faxText);
			this.FaxTextBox.Text = faxText;
		}

		/// <summary>
		/// Set values in a FuelsManager transaction record using data from the controls on the page
		/// </summary>
		/// <param name="transaction">The FuelsManager Transaction record to populate with data</param>
		public void SaveTransactionData(TransactionDO transaction)
		{
			transaction.ContactFirstName = this.ContactTextBox.Text;
			transaction.ContactInfo = this.PhoneTextBox.Text;
			transaction.UserData11 = this.AddressTextBox.Text;
			transaction.UserData12 = this.EmailTextBox.Text;
			transaction.UserData15 = this.CityTextBox.Text;
			transaction.UserData17 = this.StateTextBox.Text;
			transaction.UserData22 = this.ZipTextBox.Text;
			transaction.UserData23 = this.MemoTextBox.Text;
			transaction.UserData24 = this.FaxTextBox.Text;
		}

		#endregion
	}
}