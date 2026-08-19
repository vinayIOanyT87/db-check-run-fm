namespace TransactionFields
{
	using FMBusinessObjects.DataObjects;

	/// <summary>
	/// Summary description for DocumentNumberFG.
	/// </summary>
	public class InvoiceQueryFG : CustomTextButtonGenerator, IHeaderField
	{
		#region Constructors
		public InvoiceQueryFG()
			: base(FMControls.CustomTextBoxType.INVOICE_QUERY)
		{
			virtualField = true;
		}
		#endregion // Constructors

		public override string FieldID
		{
			get
			{
				return "InvoiceQuery";
			}
		}

		public object GetDataValue(TransactionDO transaction)
		{
			if (null == transaction.Number04)
			{
				return 0;
			}

			return transaction.Number04.Value;
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
			var stringTemp = newValue as string;

			if (stringTemp != null)
			{
				transaction.Number04 = double.Parse(stringTemp.Trim());
			}
		}

		protected override short MaxColumns
		{
			get { return ProductTextButtonGenerator.FieldLength; }
		}
	}
}
