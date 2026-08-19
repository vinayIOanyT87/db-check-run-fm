// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RadioFieldForm.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Code behind class for radio field form
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Dispatch
{
	using System;

	using FMBusinessObjects.DataObjects;

	/// <summary>
	/// Code behind class for radio field form
	/// </summary>
	public partial class RadioFieldForm : FMBaseForm
	{
		/// <summary>
		/// The transaction whose radio field will be edited/set.
		/// </summary>
		private readonly TransactionDO transaction;

		/// <summary>
		/// Initializes a new instance of the <see cref="RadioFieldForm"/> class.
		/// </summary>
		/// <param name="transactionGuid">The transaction GUID.</param>
		public RadioFieldForm(Guid transactionGuid)
		{
			try
			{
				this.GetSecurity();
				this.InitializeComponent();

				this.transaction = this.GetTransaction(transactionGuid);

				if (this.transaction.UserData.ContainsKey("TAUD8"))
				{
					this.RadioNumberTextBox.Text = this.transaction.UserData["TAUD8"];
				}

				this.RadioNumberTextBox.Focus();
				this.RadioNumberTextBox.SelectAll();
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		/// <summary>
		/// Handles the Click event of the OKButton control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
		private void OkButtonClick(object sender, EventArgs e)
		{
			try
			{
				this.transaction.UserData["TAUD8"] = this.RadioNumberTextBox.Text;
				this.SaveTransaction(this.transaction);
				this.Close();
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		/// <summary>
		/// Handles the Click event of the CancelBtn control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
		private void CancelBtnClick(object sender, EventArgs e)
		{
			this.Close();
		}

		/// <summary>
		/// Handles the Load event of the RadioFieldForm control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
		private void RadioFieldFormLoad(object sender, EventArgs e)
		{
			this.OKButton.Enabled = this.Security.HasRight(RIGHT.MODIFY_DISPATCH);
		}
	}
}
