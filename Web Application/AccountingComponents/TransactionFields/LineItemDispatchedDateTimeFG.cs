using System;
using FMBusinessObjects.DataObjects;

namespace TransactionFields
{
	/// <summary>
	/// Summary description for LineItemDispatchedDateTimeFG.
	/// </summary>
	public class LineItemDispatchedDateTimeFG : DateTimeGenerator, ILineItemField
	{
		public LineItemDispatchedDateTimeFG()
		{
		}

		public override string FieldID { get { return "LineItem ItemDispatchedDateTime"; } }

		#region ILineItemField Members

		public object GetDataValue(LineItemDO lineItem)
		{
			if (lineItem.DispatchedDateTime == null)
			{
				return null;
			}
			return lineItem.DispatchedDateTime.Value;
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
			lineItem.DispatchedDateTime = newValue as DateTimeOffset?;
			OnFieldChanged();
		}

		#endregion

	}
}
