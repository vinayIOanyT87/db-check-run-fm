namespace TransactionFields
{
	using FMBusinessObjects.DataObjects;

	/// <summary>
	/// Summary description for ContactFirstNameFG.
	/// Author: Van Thompson
	/// 
	/// ContactFirstName field generator requested by ADF
	/// </summary>
	public class ContactFirstNameFG : TextFieldGenerator, IHeaderField
	{
		public ContactFirstNameFG()
		{
		}

		public override string FieldID
		{
			get
			{
				return "ContactFirstName";
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
			return transaction.ContactFirstName;
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
			transaction.ContactFirstName = newValue as string;

			OnFieldChanged();
		}
		#endregion
	}
}
