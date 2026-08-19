namespace TransactionFields
{
	using FMBusinessObjects.DataObjects;

	/// <summary>
	/// Summary description for ContactInfoFG.
	/// </summary>
	public class ContactInfoFG : TextFieldGenerator, IHeaderField
	{
		public ContactInfoFG()
		{
		}

		public override string FieldID
		{
			get
			{
				return "ContactInfo";
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
			return transaction.ContactInfo;
		}

		public string GetDataText(TransactionDO transaction)
		{
			return transaction.ContactInfo;
		}

		public void SetDataValue(TransactionDO transaction, object newValue)
		{
			transaction.ContactInfo = newValue as string;

			OnFieldChanged();
		}
		#endregion
	}
}
