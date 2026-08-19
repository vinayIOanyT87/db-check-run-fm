using System;
using FMBusinessObjects.DataObjects;

namespace TransactionFields
{
	/// <summary>
	/// Summary description for LineItemDate02FG.
	/// </summary>
	public class LineItemDate02FG : DateTimeGenerator, ILineItemField, ISublineItemField
	{
		public LineItemDate02FG()
		{
		}

		public override string FieldID
		{
			get
			{
				return "LineItem Date02";
			}
		}

		#region ILineItemField Members

		public object GetDataValue(LineItemDO lineItem)
		{
			return lineItem.Date02;
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
			lineItem.Date02 = newValue as DateTimeOffset?;
			OnFieldChanged();
		}

		#endregion

		#region ISublineItemField Members

		object TransactionFields.ISublineItemField.GetDataValue(SubLineItemDO sublineItem)
		{
			return sublineItem.Date02;
		}

		string TransactionFields.ISublineItemField.GetDataText(SubLineItemDO sublineItem)
		{
			return sublineItem.Date02.ToString();
		}

		void TransactionFields.ISublineItemField.SetDataValue(SubLineItemDO sublineItem, object newValue)
		{
			sublineItem.Date02 = newValue as DateTimeOffset?;
			OnFieldChanged();
		}

		#endregion
	}
}
