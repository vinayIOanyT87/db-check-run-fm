namespace TransactionFields
{
	using FMBusinessObjects.DataObjects;

	/// <summary>
	/// Summary description for RoutingID_FG.
	/// </summary>
	public class RoutingID_FG : TextFieldGenerator, IHeaderField
	{
		public RoutingID_FG()
		{
		}

		public override string FieldID
		{
			get
			{
				return "RoutingID";
			}
		}

		public object GetDataValue(TransactionDO transaction)
		{
			return transaction.RouteInfo.RoutingID;
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
			transaction.RouteInfo.RoutingID = newValue as string;
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
