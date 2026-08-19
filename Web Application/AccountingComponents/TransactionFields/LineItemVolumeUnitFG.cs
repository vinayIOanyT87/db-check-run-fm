namespace TransactionFields
{
	using FMBusinessObjects.DataObjects;


	public class LineItemVolumeUnitFG : LineItemEngUnitFG
	{
		public LineItemVolumeUnitFG()
		{
		}

		public override string FieldID
		{
			get
			{
				return "LineItem VolumeUnit";
			}
		}

		public override object GetDataValue(LineItemDO inLineItem)
		{
			return GetUnitAsAbbrevString(inLineItem.VolumeUnits);
		}

		public override object GetDataValue(SubLineItemDO inSubLineItem)
		{
			return GetUnitAsAbbrevString(inSubLineItem.VolumeUnits);
		}
	}
}
