namespace TransactionFields
{
	using FMBusinessObjects.DataObjects;

	/// <summary>
	/// Summary description for ReversedTransID_FG.
	/// </summary>
	public class ReversedTransID_FG : TextFieldGenerator, IHeaderField
	{
		public ReversedTransID_FG()
		{
		}

		public override string FieldID
		{
			get
			{
				return "ReversedTransID";
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
			return transaction.ReversedTransID;
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
			System.Diagnostics.Debug.Assert(false, "ReversedTransID_FG.SetDataValue() should never be called.");
		}
		#endregion
	}
}
