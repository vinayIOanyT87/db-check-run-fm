using System;
using FMBusinessObjects.DataObjects;

namespace TransactionFields
{
	/// <summary>
	/// Summary description for LineItemDate01FG.
	/// </summary>
	public class LineItemDate01FG : DateTimeGenerator, ILineItemField, ISublineItemField
	{
		public LineItemDate01FG()
		{
		}

		public override string FieldID
		{
			get
			{
				return "LineItem Date01";
			}
		}

		#region ILineItemField Members

		public object GetDataValue(LineItemDO lineItem)
		{
			return lineItem.Date01;
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
			lineItem.Date01 = newValue as DateTimeOffset?;
			OnFieldChanged();
		}

		#endregion

		#region ISublineItemField Members

		object TransactionFields.ISublineItemField.GetDataValue(SubLineItemDO sublineItem)
		{
			return sublineItem.Date01;
		}

		string TransactionFields.ISublineItemField.GetDataText(SubLineItemDO sublineItem)
		{
			return sublineItem.Date01.ToString();
		}

		void TransactionFields.ISublineItemField.SetDataValue(SubLineItemDO sublineItem, object newValue)
		{
			sublineItem.Date01 = newValue as DateTimeOffset?;
			OnFieldChanged();
		}

		#endregion
	}
}
