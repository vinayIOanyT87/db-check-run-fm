// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CardNumberFG.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Summary description for CardNumberFG.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TransactionFields
{
	using System.Web.UI;
	using System.Web.UI.WebControls;

	using FMBusinessObjects.DataObjects;

	/// <summary>
	/// Summary description for Card Number Field Generator.
	/// </summary>
	public class CardNumberFG : TextFieldGenerator, IHeaderField
	{
		/// <summary>
		/// The client side script card number.
		/// </summary>
		public const string CLIENT_SIDE_SCRIPT_CARD_NUMBER = "CLIENT_SIDE_SCRIPT_CARD_NUMBER";

		/// <summary>
		/// The client side key card number.
		/// </summary>
		public const string CLIENT_SIDE_KEY_CARD_NUMBER = "CLIENT_SIDE_KEY_CARD_NUMBER";

		/// <summary>
		/// Initializes a new instance of the <see cref="CardNumberFG"/> class.
		/// </summary>
		public CardNumberFG()
		{
		}

		/// <summary>
		/// Gets the field ID.
		/// </summary>
		public override string FieldID
		{
			get { return "CardNumber"; }
		}

		/// <summary>
		/// This property will returned either a figured data length or the 
		/// default length of 30.
		/// </summary>
		protected override short MaxColumns
		{
			get { return this.GetFieldLength(this.FieldID, 30); }
		}

		#region override methods
		/// <summary>
		/// This method handles special ASP control functions such as client side scripting.
		/// </summary>
		/// <param name="control">
		/// The control.
		/// </param>
		protected override void SpecializeControl(WebControl control)
		{
			base.SpecializeControl(control);

			TextBox textBox = null;
			var updatePanel = control.Controls[0] as UpdatePanel;

			if (updatePanel != null)
			{
				textBox = updatePanel.ContentTemplateContainer.Controls[0] as TextBox;
				
				if(textBox != null)
				{

					// Register client scripts for this control if the custom client script registered is registered.
					var customClientScript = control.Page.Session[CUSTOM_CLIENT_SCRIPT_NAME] as string;

					if (!string.IsNullOrEmpty(customClientScript))
					{
						// Delay client side scripting until page pre-render event in case user clicks edit button of a
						// line item while editing another line item. Such situation causes this method to be called 
						// twice, once for for each line item. Since client side script is  allowed only once to be registered,
						// later line item's client script is ignored, which is the one we actually want.
						if (textBox != null)
						{
							textBox.Page.Session[CLIENT_SIDE_SCRIPT_CARD_NUMBER] =
								"<script language=\"javascript\" type=\"text/javascript\"><!--\n" +
								"var oCardNumberTextBox  = document.getElementById('" + textBox.ClientID + "');\n " +
								"\n//--></script>";

							textBox.Attributes.Add("onChange", "javascript:try{MasterOnChange('" + this.FieldID + "');}catch(err){;}");
						}
					}
				}
			}
		}
		#endregion

		#region IHeaderField Members
		/// <summary>
		/// The get data value.
		/// </summary>
		/// <param name="transaction">
		/// The transaction.
		/// </param>
		/// <returns>
		/// The <see cref="object"/>.
		/// </returns>
		public object GetDataValue(TransactionDO transaction)
		{
			return this.GetDataValueOverride(transaction);
		}

		/// <summary>
		/// The get data text.
		/// </summary>
		/// <param name="transaction">
		/// The transaction.
		/// </param>
		/// <returns>
		/// The <see cref="string"/>.
		/// </returns>
		public string GetDataText(TransactionDO transaction)
		{
			return this.GetDataTextOverride(transaction);
		}

		/// <summary>
		/// The set data value.
		/// </summary>
		/// <param name="transaction">
		/// The transaction.
		/// </param>
		/// <param name="newValue">
		/// The new value.
		/// </param>
		public void SetDataValue(TransactionDO transaction, object newValue)
		{
			this.SetDataValueOverride(transaction, newValue);
		}

		/// <summary>
		/// This method is an virtual method so that the derive class can
		/// modify the data that is returning.  You cannot have virtuals
		/// in an interface.
		/// </summary>
		/// <param name="inTrans">
		/// The Transaction.
		/// </param>
		/// <returns>
		/// The <see cref="object"/>.
		/// </returns>
		protected virtual object GetDataValueOverride(TransactionDO inTrans)
		{
			return inTrans.PaymentInfo.CreditCardNumber;
		}

		/// <summary>
		/// This method is an virtual method so that the derive class can
		/// modify the data that is returning.  You cannot have virtuals
		/// in an interface.
		/// </summary>
		/// <param name="inTrans">
		/// The Transaction.
		/// </param>
		/// <returns>
		/// The <see cref="string"/>.
		/// </returns>
		protected virtual string GetDataTextOverride(TransactionDO inTrans)
		{
			return this.GetDataValueOverride(inTrans).ToString();
		}

		/// <summary>
		/// The set data value override.
		/// </summary>
		/// <param name="inTrans">
		/// The transaction.
		/// </param>
		/// <param name="newValue">
		/// The new value.
		/// </param>
		protected virtual void SetDataValueOverride(TransactionDO inTrans, object newValue)
		{
			inTrans.PaymentInfo.CreditCardNumber = newValue as string;

			this.OnFieldChanged();
		}
		#endregion
	}
}
