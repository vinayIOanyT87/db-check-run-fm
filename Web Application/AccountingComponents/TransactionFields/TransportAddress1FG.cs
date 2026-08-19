namespace TransactionFields
{
	using FMBusinessObjects.DataObjects;

	public class TransportAddress1FG : TextFieldGenerator, ITransportLineItemField
	{
		#region Constructors
		/// <summary>
		/// This is the default constructor for the Transport Address 1 Field Generator.
		/// </summary>
		public TransportAddress1FG()
		{
		}
		#endregion

		#region Properties
		/// <summary>
		/// This property returns the field identification of the field.
		/// </summary>
		public override string FieldID
		{
			get
			{
				return "TransportLineItem Address1";
			}
		}

		/// <summary>
		/// This property will returned either a figured data length or the 
		/// default length of 60.
		/// </summary>
		protected override short MaxColumns
		{
			get
			{
				return this.GetFieldLength(FieldID, 60);
			}
		}
		#endregion

		#region ITransportLineItemField override methods
		/// <summary>
		/// This method will return the Address 1 field value from the data object.
		/// </summary>
		/// <param name="transportLineItem"></param>
		/// <returns></returns>
		public object GetDataValue(TransportLineItemDO transportLineItem)
		{
			return transportLineItem.Address1;
		}

		/// <summary>
		/// This method will return the Address 1 field text from the data object.
		/// </summary>
		/// <param name="transportLineItem"></param>
		/// <returns></returns>
		public string GetDataText(TransportLineItemDO transportLineItem)
		{
			if (GetDataValue(transportLineItem) != null)
			{
				return GetDataValue(transportLineItem).ToString();
			}
			
			return null;
		}

		/// <summary>
		/// This method will set the value from the page into the data object.
		/// </summary>
		/// <param name="transportLineItem"></param>
		/// <param name="newValue"></param>
		public void SetDataValue(TransportLineItemDO transportLineItem, object newValue)
		{
			transportLineItem.Address1 = newValue as string;
			OnFieldChanged();
		}
		#endregion
	}
}
