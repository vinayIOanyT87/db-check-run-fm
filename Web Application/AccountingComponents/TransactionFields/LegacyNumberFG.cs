namespace TransactionFields
{
	using FMBusinessObjects.DataObjects;

	/// <summary>
	/// Summary description for LegacyNumberFG.
	/// </summary>
	public class LegacyNumberFG : TextFieldGenerator, IHeaderField
	{
		public LegacyNumberFG()
		{
		}

		public override string FieldID
		{
			get
			{
				return "LegacyNumber";
			}
		}

      /// <summary>
      /// This property will returned either a figured data length or the 
      /// default length of 50.
      /// </summary>
      protected override short MaxColumns
		{
			get
			{
				return this.GetFieldLength(FieldID, 50);
			}
		}


		#region IHeaderField Members
		public object GetDataValue(TransactionDO transaction)
		{
			return transaction.LegacyNumber;
		}

		public string GetDataText(TransactionDO transaction)
		{
			return transaction.LegacyNumber;
		}

		public void SetDataValue(TransactionDO transaction, object newValue)
		{
			transaction.LegacyNumber = newValue as string;
			OnFieldChanged();
		}
		#endregion
	}
}
