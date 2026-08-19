namespace TransactionFields
{
	using FMBusinessObjects.DataObjects;

	/// <summary>
	/// Summary description for IssuePointNumberFG.
	/// </summary>
	public class IssuePointNumberFG : TextFieldGenerator, IHeaderField
	{
		public override string FieldID { get { return "IssuePointNumber"; } }
		public object GetDataValue( TransactionDO transaction )
		{
			return transaction.IssuePointNumber;
		}

		public string GetDataText( TransactionDO transaction )
		{
			if ( GetDataValue( transaction ) != null )
			{
				return GetDataValue( transaction ).ToString();
			}
			else
			{
				return null;
			}
		}

		public void SetDataValue( TransactionDO transaction, object newValue )
		{
			transaction.IssuePointNumber = newValue as string;

			OnFieldChanged();
		}

		/// <summary>
		/// This property will returned either a figured data length or the 
		/// default length of 30.
		/// </summary>
		protected override short MaxColumns
		{
			get { return this.GetFieldLength( FieldID, 30 ); }
		}
	}
}
