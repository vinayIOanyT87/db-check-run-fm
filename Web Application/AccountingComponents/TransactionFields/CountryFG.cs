namespace TransactionFields
{
	using FMBusinessObjects.DataObjects;

	/// <summary>
	/// Summary description for CountryFG.
	/// </summary>
	public class CountryFG : TextFieldGenerator, IHeaderField
	{
		public CountryFG()
		{
		}

		public override string FieldID
		{
			get
			{
				return "Country";
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
			return transaction.Country;
		}

		public string GetDataText(TransactionDO transaction)
		{
			return transaction.Country;
		}

		public void SetDataValue(TransactionDO transaction, object newValue)
		{
			transaction.Country = newValue as string;

			OnFieldChanged();
		}
		#endregion
	}
}
