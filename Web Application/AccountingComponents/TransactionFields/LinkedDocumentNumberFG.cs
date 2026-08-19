namespace TransactionFields
{
	using FMBusinessObjects.DataObjects;

	/// <summary>
	/// Summary description for LinkedDocumentNumberFG.
	/// </summary>
	public class LinkedDocumentNumberFG : TextFieldGenerator, IHeaderField
	{
		public LinkedDocumentNumberFG()
		{

		}

		public override string FieldID
		{
			get
			{
				return "LinkedDocumentNumber";
			}
		}

		public override bool Editable
		{
			get
			{
				return false;
			}
		}

		/// <summary>
		/// This property will returned either a figured data length or the 
		/// default length of 64.
		/// </summary>
		protected override short MaxColumns
		{
			get
			{
				return this.GetFieldLength(FieldID, 64);
			}
		}

		#region IHeaderField Members
		public object GetDataValue(TransactionDO transaction)
		{
			return transaction.LinkedDocumentNumber;
		}

		public string GetDataText(TransactionDO transaction)
		{
			return GetDataValue(transaction).ToString();
		}

		void IHeaderField.SetDataValue(TransactionDO transaction, object newValue)
		{
		}
		#endregion
	}
}
