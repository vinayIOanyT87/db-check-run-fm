namespace TransactionFields
{
	using System;

	using FMBusinessObjects.DataObjects;

	/// <summary>
	/// Summary description for LineItemGrossQuantityRemainingFG.
	/// </summary>
	public class LineItemGrossQuantityRemainingFG : LineItemVolumeFG, ILineItemField
	{
		public LineItemGrossQuantityRemainingFG()
		{
			virtualField = true;
		}

		public override string FieldID
		{
			get
			{
				return "LineItem GrossQuantityRemaining";
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
			return Math.Round(inLineItem.GrossQuantityRemaining, inLineItem.VolumeDecimalPlaces, MidpointRounding.AwayFromZero);
		}

		public string GetDataText(LineItemDO inLineItem)
		{
			return GetDataValue(inLineItem).ToString();
		}

		public void SetDataValue(LineItemDO inLineItem, object newValue)
		{
			// Calculated field value - not setable
			OnFieldChanged();
		}
	}
}
