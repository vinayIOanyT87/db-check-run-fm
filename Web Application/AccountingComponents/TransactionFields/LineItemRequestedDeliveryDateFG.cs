using System;
using FMBusinessObjects.DataObjects;

namespace TransactionFields
{
	/// <summary>
	/// Summary description for LineItemRequestedDeliveryDateFG.
	/// </summary>
	public class LineItemRequestedDeliveryDateFG : DateTimeGenerator, ILineItemField
	{
		public LineItemRequestedDeliveryDateFG()
		{
		}

		public override string FieldID
		{
			get
			{
				return "LineItem RequestedDeliveryDate";
			}
		}

		#region ILineItemField Members

		public object GetDataValue(LineItemDO lineItem)
		{
			return lineItem.RequestedDeliveryDate;
		}

		public string GetDataText(LineItemDO lineItem)
		{
			if (GetDataValue(lineItem) != null)
			{
				return GetDataValue(lineItem).ToString();
			}
			else
			{
				return null;
			}
		}

		public void SetDataValue(LineItemDO lineItem, object newValue)
		{
			lineItem.RequestedDeliveryDate = newValue as DateTimeOffset?;

			OnFieldChanged();
		}

		#endregion
	}
}
