namespace TransactionFields
{
	using FMBusinessObjects.DataObjects;

	public class TransportLocationNameFG : TextFieldGenerator, ITransportLineItemField
	{
		#region Contructors
		/// <summary>
		/// This is the default constructor for the Location Name field generator class.
		/// </summary>
		public TransportLocationNameFG()
		{
		}
		#endregion

		#region Override Properties
		/// <summary>
		/// This property returns the field ID
		/// </summary>
		public override string FieldID
		{
			get
			{
				return "TransportLineItem LocationName";
			}
		}

		/// <summary>
		/// This property returns the field's maximum column width.
		/// </summary>
		protected override short MaxColumns
		{
			get
			{
				return this.GetFieldLength(FieldID, CompanyTextButtonGenerator.FIELD_LENGTH);
			}
		}

		/// <summary>
		/// This property will return true if the location name is required.
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
		/// This method will return the Location Name field value from the data object.
		/// </summary>
		/// <param name="transportLineItem"></param>
		/// <returns></returns>
		public object GetDataValue(TransportLineItemDO transportLineItem)
		{
			return transportLineItem.LocationName;
		}

		/// <summary>
		/// This method will return the Location Name field text from the data object.
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
			transportLineItem.LocationName = newValue as string;
			OnFieldChanged();
		}
		#endregion
	}
}
