using System;
using FMBusinessObjects.DataObjects;

namespace TransactionFields
{
	/// <summary>
	/// Summary description for LineItemRequestedDateTimeFG.
	/// </summary>
	public class LineItemRequestedDateTimeFG : DateTimeGenerator, ILineItemField
	{
		public LineItemRequestedDateTimeFG()
		{
		}

		public override string FieldID { get { return "LineItem ItemRequestedDateTime"; } }

		#region ILineItemField Members

		public object GetDataValue(LineItemDO lineItem)
		{
			return lineItem.RequestedDateTime;
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
			lineItem.RequestedDateTime = newValue as DateTimeOffset?;
			OnFieldChanged();
		}

		#endregion
	}
}
