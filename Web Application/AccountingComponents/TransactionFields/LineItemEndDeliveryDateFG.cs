namespace TransactionFields
{
	using System;
	using FMBusinessObjects.DataObjects;

	/// <summary>
	/// Summary description for LineItemEndDeliveryDateFG.
	/// </summary>
	public class LineItemEndDeliveryDateFG : DateTimeGenerator, ILineItemField
	{
		public LineItemEndDeliveryDateFG()
		{
		}

		public override string FieldID
		{
			get
			{
				return "LineItem EndDeliveryDate";
			}
		}

		#region ILineItemField Members

		public object GetDataValue(LineItemDO inLineItem)
		{
			if (inLineItem.EndDeliveryDate == null)
			{
				return null;
			}

			return inLineItem.EndDeliveryDate.Value;
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
			inLineItem.EndDeliveryDate = newValue as DateTimeOffset?;
			OnFieldChanged();
		}
		#endregion
	}
}
