namespace TransactionFields
{
	using FMBusinessObjects.DataObjects;
	using System;

	/// <summary>
	/// Summary description for LineItemGrossQuantityReceivedFG.
	/// </summary>
	internal class LineItemGrossQuantityReceivedFG : QuantityReceivedFG, ILineItemField
	{
		public LineItemGrossQuantityReceivedFG()
		{
			virtualField = true;
		}

		public override string FieldID
		{
			get
			{
				return "LineItem GrossQuantityReceived";
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
				LineItemDO localLineItem = this.trans.LineItems[0];
				return localLineItem.TransactionLineItemGuid != Guid.Empty;
			}
		}

		public object GetDataValue(LineItemDO inLineItem)
		{
			return Math.Round(inLineItem.GrossQuantityReceived, inLineItem.VolumeDecimalPlaces, MidpointRounding.AwayFromZero);
		}

		public string GetDataText(LineItemDO inLineItem)
		{
			return GetDataValue(inLineItem).ToString();
		}

		public void SetDataValue(LineItemDO inLineItem, object newValue)
		{
			OnFieldChanged();
		}
	}
}
