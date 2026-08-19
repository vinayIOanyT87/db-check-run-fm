using System;
using FMBusinessObjects.DataObjects;

namespace TransactionFields
{
	/// <summary>
	/// Summary description for CashAmountFG.
	/// </summary>
	public class CashAmountFG : NumericTextFieldGenerator, IHeaderField
	{
		public CashAmountFG()
		{
			
		}
		public override string FieldID { get { return "CashAmount"; } }
		public override ENumericType NumericType { get { return ENumericType.Double; } }
		public override SITE_VARIABLE_TYPE UnitType
		{ get { return SITE_VARIABLE_TYPE.DEFAULT; } }

		#region IHeaderField Members

		public object GetDataValue(TransactionDO transaction)
		{
			if(transaction.PaymentInfo.CashAmount == null)
			{
				return null;
			}
			return transaction.PaymentInfo.CashAmount.Value;
		}

		public string GetDataText(TransactionDO transaction)
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

		public void SetDataValue(TransactionDO transaction, object newValue)
		{
			if(newValue == null)
			{
				transaction.PaymentInfo.CashAmount = null;
			}
			else
			{
				transaction.PaymentInfo.CashAmount = newValue as double?;
			}

			OnFieldChanged();
		}

		#endregion
	}
}
