using System;
using FMBusinessObjects.DataObjects;

namespace TransactionFields
{
	/// <summary>
	/// Summary description for LineItemOnLocationTimeFG.
	/// </summary>
	public class LineItemOnLocationTimeFG : DateTimeGenerator, ILineItemField
	{
		public LineItemOnLocationTimeFG()
		{
		}

		public override string FieldID { get { return "LineItem OnLocationTime"; } }

		#region ILineItemField Members

		public object GetDataValue(LineItemDO lineItem)
		{
			return lineItem.OnLocationTime;
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
			lineItem.OnLocationTime = newValue as DateTimeOffset?;
			OnFieldChanged();
		}

		#endregion
	}
}
