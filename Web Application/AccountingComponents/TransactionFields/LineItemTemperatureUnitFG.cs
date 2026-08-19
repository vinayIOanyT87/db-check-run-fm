namespace TransactionFields
{
	using FMBusinessObjects.DataObjects;

	public class LineItemTemperatureUnitFG : LineItemEngUnitFG
	{
		public LineItemTemperatureUnitFG()
		{
		}

		public override string FieldID
		{
			get
			{
				return "LineItem TemperatureUnit";
			}
		}

		public override object GetDataValue(LineItemDO inLineItem)
		{
			return GetUnitAsAbbrevString(inLineItem.TemperatureUnits);
		}

		public override object GetDataValue(SubLineItemDO inSubLineItem)
		{
			return GetUnitAsAbbrevString(inSubLineItem.TemperatureUnits);
		}
	}
}
