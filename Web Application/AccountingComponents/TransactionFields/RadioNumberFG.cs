namespace TransactionFields
{
	using FMBusinessObjects.DataObjects;

	/// <summary>
	/// Summary description for RadioNumberFG.
	/// </summary>
	public class RadioNumberFG : TextFieldGenerator, IHeaderField
	{
		public override string FieldID { get { return "RadioNumber"; } }
		public object GetDataValue( TransactionDO transaction )
		{
			return transaction.RadioNumber;
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
			transaction.RadioNumber = newValue as string;

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
