using System;
using FMBusinessObjects.DataObjects;

namespace TransactionFields
{
	/// <summary>
	/// Summary description for CreditAmountFG.
	/// </summary>
	public class CreditAmountFG : NumericTextFieldGenerator, IHeaderField
	{
		public CreditAmountFG ( )
		{

		}

		public override string FieldID { get { return "CreditAmount"; } }
		public override ENumericType NumericType { get { return ENumericType.Double; } }
		public override SITE_VARIABLE_TYPE UnitType
		{ get { return SITE_VARIABLE_TYPE.DEFAULT; } }

		#region IHeaderField Members

		public object GetDataValue ( TransactionDO transaction )
		{
			if (transaction.PaymentInfo.CreditCardAmount == null)
			{
				return null;
			}
			return transaction.PaymentInfo.CreditCardAmount.Value;
		}

		public string GetDataText ( TransactionDO transaction )
		{
			if (GetDataValue(transaction) != null)
			{
				return GetDataValue(transaction).ToString();
			}
			else
			{
				return null;
			}
		}

		public void SetDataValue ( TransactionDO transaction, object newValue )
		{
			if (newValue == null)
			{
				transaction.PaymentInfo.CreditCardAmount = null;
			}
			else
			{
				transaction.PaymentInfo.CreditCardAmount = newValue as double?;
			}

			OnFieldChanged ( );
		}

		#endregion
	}
}
