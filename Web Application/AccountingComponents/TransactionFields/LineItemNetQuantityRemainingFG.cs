namespace TransactionFields
{
	using System;

	using FMBusinessObjects.DataObjects;

	/// <summary>
	/// Summary description for LineItemNetQuantityRemainingFG.
	/// </summary>
	public class LineItemNetQuantityRemainingFG : LineItemVolumeFG, ILineItemField
	{
		public LineItemNetQuantityRemainingFG()
		{
			virtualField = true;
		}

		public override string FieldID
		{
			get
			{
				return "LineItem NetQuantityRemaining";
			}
		}

		public override bool Required
		{
			get
			{
				return false;
			}
		}

		public override bool Editable
		{
			get
			{
				return false;
			}
		}

		public object GetDataValue(LineItemDO inLineItem)
		{
			return Math.Round(inLineItem.NetQuantityRemaining, inLineItem.VolumeDecimalPlaces, MidpointRounding.AwayFromZero);
		}

		public string GetDataText(LineItemDO inLineItem)
		{
			if (GetDataValue(inLineItem) != null)
			{
				return GetDataValue(inLineItem).ToString();
			}

			return null;
		}

		public void SetDataValue(LineItemDO inLineItem, object newValue)
		{
			// Calculated field value - not setable
			OnFieldChanged();
		}
	}
}
