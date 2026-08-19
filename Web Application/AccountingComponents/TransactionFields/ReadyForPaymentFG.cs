namespace TransactionFields
{
	using System.Web.UI;
	using FMBusinessObjects.DataObjects;

	public enum InvoiceStatus : short
	{
		NONE = 0,
		READY = 1,
		REJECTED = 2
	}

	public class ReadyForPaymentFG : CustomCheckBoxGenerator, IHeaderField
	{
		public ReadyForPaymentFG()
			: base(new Pair("onClick", "javascript:ProcessReadyForPayment();"))
		{
		}

		public override string FieldID
		{
			get
			{
				return "ReadyForPayment";
			}
		}

		public object GetDataValue(TransactionDO transaction)
		{
			if (null == transaction.Number05)
			{
				return false;
			}

			return transaction.Number05.Value == (double) InvoiceStatus.READY;
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
			if ((bool) newValue)
			{
				transaction.Number05 = (double) InvoiceStatus.READY;
			}
		}
	}

	public class RejectedForPaymentFG : CustomCheckBoxGenerator, IHeaderField
	{
		public RejectedForPaymentFG()
			: base(new Pair("onClick", "javascript:ProcessRejectedForPayment();"))
		{
		}

		public override string FieldID
		{
			get
			{
				return "RejectedForPayment";
			}
		}

		public object GetDataValue(TransactionDO transaction)
		{
			if (null == transaction.Number05)
			{
				return false;
			}

			return transaction.Number05.Value == (double) InvoiceStatus.REJECTED;
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
			if ((bool) newValue)
			{
				transaction.Number05 = (double) InvoiceStatus.REJECTED;
			}
		}
	}
}
