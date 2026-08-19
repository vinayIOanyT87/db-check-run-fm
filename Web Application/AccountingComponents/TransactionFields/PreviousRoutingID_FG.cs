namespace TransactionFields
{
	using FMBusinessObjects.DataObjects;

	/// <summary>
	/// Summary description for PreviousRoutingID_FG.
	/// </summary>
	public class PreviousRoutingID_FG : TextFieldGenerator, IHeaderField
	{
		public PreviousRoutingID_FG()
		{
		}

		public override string FieldID
		{
			get
			{
				return "PreviousRoutingID";
			}
		}

		public object GetDataValue(TransactionDO transaction)
		{
			return transaction.RouteInfo.PreviousRoutingID;
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
			transaction.RouteInfo.PreviousRoutingID = newValue as string;
			OnFieldChanged();
		}

		/// <summary>
		/// This property will returned either a figured data length or the 
		/// default length of 30.
		/// </summary>
		protected override short MaxColumns
		{
			get
			{
				return this.GetFieldLength(FieldID, 30);
			}
		}
	}
}
