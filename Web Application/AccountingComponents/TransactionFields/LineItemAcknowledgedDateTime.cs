using System;
using FMBusinessObjects.DataObjects;

namespace TransactionFields
{
	/// <summary>
	/// Summary description for LineItemAcknowledgedDateTime.
	/// </summary>
	public class LineItemAcknowledgedDateTime : DateTimeGenerator, ILineItemField
	{
		public LineItemAcknowledgedDateTime()
		{

		}

		public override string FieldID { get { return "LineItem AcknowledgedDateTime"; } }

		#region ILineItemField Members

		public object GetDataValue(LineItemDO lineItem)
		{
			return lineItem.AcknowledgedDateTime;
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
			lineItem.AcknowledgedDateTime = newValue as DateTimeOffset?;
			OnFieldChanged();
		}

		#endregion
	}
}
