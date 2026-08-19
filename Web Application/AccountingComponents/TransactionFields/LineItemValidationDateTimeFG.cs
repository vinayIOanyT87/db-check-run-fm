using System;
using FMBusinessObjects.DataObjects;

namespace TransactionFields
{
	/// <summary>
	/// Summary description for LineItemValidationDateTimeFG.
	/// </summary>
	public class LineItemValidationDateTimeFG : DateTimeGenerator, ILineItemField
	{
		public LineItemValidationDateTimeFG()
		{

		}

		public override string FieldID { get { return "LineItem ValidationDateTime"; } }

		#region ILineItemField Members

		public object GetDataValue(LineItemDO lineItem)
		{
			if (lineItem.ValidationDateTime == null)
			{
				return null;
			}
			return lineItem.ValidationDateTime.Value;
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
			lineItem.ValidationDateTime = newValue as DateTimeOffset?;
			OnFieldChanged();
		}

		#endregion
	}
}
