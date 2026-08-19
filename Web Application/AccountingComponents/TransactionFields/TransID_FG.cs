namespace TransactionFields
{
	using FMBusinessObjects.DataObjects;

	/// <summary>
	/// Summary description for TransID_FG.
	/// </summary>
	public class TransID_FG : TextFieldGenerator, IHeaderField
	{
		public TransID_FG()
		{
		}

		public override string FieldID
		{
			get
			{
				return "TransID";
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
		/// default length of 40.
		/// </summary>
		protected override short MaxColumns
		{
			get
			{
				return this.GetFieldLength(FieldID, 40);
			}
		}

		#region IHeaderField Members
		public object GetDataValue(TransactionDO transaction)
		{
			return transaction.TransID;
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
			System.Diagnostics.Debug.Assert(false, "TransID_FG.SetDataValue() should never be called as it is not editable.");
		}
		#endregion
	}
}
