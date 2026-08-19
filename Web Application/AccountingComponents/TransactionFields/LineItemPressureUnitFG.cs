namespace TransactionFields
{
	using FMBusinessObjects.DataObjects;

	public class LineItemPressureUnitFG : LineItemEngUnitFG
	{
		public LineItemPressureUnitFG()
		{
		}

		public override string FieldID
		{
			get
			{
				return "LineItem PressureUnit";
			}
		}

		public override object GetDataValue(LineItemDO inLineItem)
		{
			return GetUnitAsAbbrevString(inLineItem.PressureUnits);
		}

		public override object GetDataValue(SubLineItemDO inSubLineItem)
		{
			return GetUnitAsAbbrevString(inSubLineItem.PressureUnits);
		}
	}
}
