using System;
using FMBusinessObjects.DataObjects;

namespace TransactionFields
{
	/// <summary>
	/// Summary description for LineItemCompletionDateTimeFG.
	/// </summary>
	public class LineItemCompletionDateTimeFG : DateTimeGenerator, ILineItemField
	{
		public LineItemCompletionDateTimeFG()
		{

		}

		public override string FieldID { get { return "LineItem CompletionDateTime"; } }

		#region ILineItemField Members

		public object GetDataValue(LineItemDO lineItem)
		{
			if (lineItem.CompletionDateTime == null)
			{
				return null;
			}
			return lineItem.CompletionDateTime.Value;
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
			lineItem.CompletionDateTime = newValue as DateTimeOffset?;
			OnFieldChanged();
		}

		#endregion
	}
}
