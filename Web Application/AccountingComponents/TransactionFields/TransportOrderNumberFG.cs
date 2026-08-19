namespace TransactionFields
{
	using FMBusinessObjects.DataObjects;

	public class TransportOrderNumberFG : TextFieldGenerator, ITransportLineItemField
	{
		#region Constructors
		/// <summary>
		/// This is the default constructor for the Transport order number Field Generator.
		/// </summary>
		public TransportOrderNumberFG()
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
				return "TransportLineItem TransportOrderNumber";
			}
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

		/// <summary>
		/// This property will return true meaning that the field is
		/// always required.
		/// </summary>
		public override bool Required
		{
			get
			{
				return true;
			}
		}
		#endregion

		#region ITransportLineItemField override methods
		/// <summary>
		/// This method will return the Order Number field value from the data object.
		/// </summary>
		/// <param name="transportLineItem"></param>
		/// <returns></returns>
		public object GetDataValue(TransportLineItemDO transportLineItem)
		{
			return transportLineItem.TransportOrderNumber;
		}

		/// <summary>
		/// This method will return the Order Number field text from the data object.
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
			transportLineItem.TransportOrderNumber = newValue as string;
			OnFieldChanged();
		}
		#endregion
	}
}
