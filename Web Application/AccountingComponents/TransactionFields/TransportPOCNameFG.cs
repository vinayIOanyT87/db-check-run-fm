namespace TransactionFields
{
	using FMBusinessObjects.DataObjects;

	public class TransportPOCNameFG : TextFieldGenerator, ITransportLineItemField
	{
		#region Constructors
		/// <summary>
		/// This is the default constructor for the Transport POC Name Field Generator.
		/// </summary>
		public TransportPOCNameFG()
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
				return "TransportLineItem POCName";
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
		#endregion

		#region ITransportLineItemField override methods
		/// <summary>
		/// This method will return the POC Name field value from the data object.
		/// </summary>
		/// <param name="transportLineItem"></param>
		/// <returns></returns>
		public object GetDataValue(TransportLineItemDO transportLineItem)
		{
			return transportLineItem.POCName;
		}

		/// <summary>
		/// This method will return the POC Name field text from the data object.
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
			transportLineItem.POCName = newValue as string;
			OnFieldChanged();
		}
		#endregion
	}
}

