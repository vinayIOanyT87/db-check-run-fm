namespace TransactionFields
{
	using FMBusinessObjects.DataObjects;

	public class TransportZipFG : TextFieldGenerator, ITransportLineItemField
	{
		#region Constructors
		/// <summary>
		/// This is the default constructor for the Transport ZIP Field Generator.
		/// </summary>
		public TransportZipFG()
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
				return "TransportLineItem Zip";
			}
		}

		/// <summary>
		/// This property will returned either a figured data length or the 
		/// default length of 11.
		/// </summary>
		protected override short MaxColumns
		{
			get
			{
				return this.GetFieldLength(FieldID, 11);
			}
		}
		#endregion

		#region ITransportLineItemField override methods
		/// <summary>
		/// This method will return the ZIP field value from the data object.
		/// </summary>
		/// <param name="transportLineItem"></param>
		/// <returns></returns>
		public object GetDataValue(TransportLineItemDO transportLineItem)
		{
			return transportLineItem.Zip;
		}

		/// <summary>
		/// This method will return the ZIP field text from the data object.
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
			transportLineItem.Zip = newValue as string;
			OnFieldChanged();
		}
		#endregion
	}
}

