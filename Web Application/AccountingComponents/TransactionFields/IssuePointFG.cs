namespace TransactionFields
{
	using FMBusinessObjects.DataObjects;

	/// <summary>
	/// Summary description for IssuePointFG.
	/// </summary>
	public class IssuePointFG : TextFieldGenerator, IHeaderField
	{
		public override string FieldID { get { return "IssuePoint"; } }
		public object GetDataValue( TransactionDO transaction )
		{
			return transaction.IssuePoint;
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
			transaction.IssuePoint = newValue as string;

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
