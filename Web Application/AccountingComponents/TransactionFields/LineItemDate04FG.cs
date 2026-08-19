using System;
using FMBusinessObjects.DataObjects;

namespace TransactionFields
{
	/// <summary>
	/// Summary description for LineItemDate04FG.
	/// </summary>
	public class LineItemDate04FG : DateTimeGenerator, ILineItemField, ISublineItemField
	{
		public LineItemDate04FG()
		{
		}

		public override string FieldID
		{
			get
			{
				return "LineItem Date04";
			}
		}

		#region ILineItemField Members

		public object GetDataValue(LineItemDO lineItem)
		{
			return lineItem.Date04;
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
			lineItem.Date04 = newValue as DateTimeOffset?;
			OnFieldChanged();
		}

		#endregion

		#region ISublineItemField Members

		object TransactionFields.ISublineItemField.GetDataValue(SubLineItemDO sublineItem)
		{
			return sublineItem.Date04;
		}

		string TransactionFields.ISublineItemField.GetDataText(SubLineItemDO sublineItem)
		{
			return sublineItem.Date04.ToString();
		}

		void TransactionFields.ISublineItemField.SetDataValue(SubLineItemDO sublineItem, object newValue)
		{
			sublineItem.Date04 = newValue as DateTimeOffset?;
			OnFieldChanged();
		}

		#endregion
	}
}
