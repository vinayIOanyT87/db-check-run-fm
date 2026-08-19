using System;
using FMBusinessObjects.DataObjects;

namespace TransactionFields
{
	/// <summary>
	/// Summary description for CardExpirationFG.
	/// </summary>
	internal class CardExpirationFG : DateGenerator, IHeaderField
	{
		#region Constructors
		/// <summary>
		/// This is the default constructor for the Card Expiration Field Control.
		/// </summary>
		public CardExpirationFG()
		{
		}
		#endregion

		#region Properties
		/// <summary>
		/// Gets the Field ID.
		/// </summary>
		public override string FieldID
		{
			get { return "CardExpiration"; }
		}
		#endregion

		#region IHeaderField Members
		public object GetDataValue(TransactionDO transaction)
		{
			return transaction.PaymentInfo.CreditCardExpiration;
		}

		public string GetDataText(TransactionDO transaction)
		{
			if (GetDataValue(transaction) != null)
			{
				return GetDataValue(transaction).ToString();
			}

			return null;
		}

		public void SetDataValue(TransactionDO transaction, object newValue)
		{
			transaction.PaymentInfo.CreditCardExpiration = newValue as DateTimeOffset?;

			OnFieldChanged();
		}
		#endregion
	}
}
