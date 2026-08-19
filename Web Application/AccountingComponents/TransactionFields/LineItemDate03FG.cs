using System;
using FMBusinessObjects.DataObjects;

namespace TransactionFields
{
	/// <summary>
	/// Summary description for LineItemDate03FG.
	/// </summary>
	public class LineItemDate03FG : DateTimeGenerator, ILineItemField, ISublineItemField
	{
		public LineItemDate03FG()
		{
		}

		public override string FieldID
		{
			get
			{
				return "LineItem Date03";
			}
		}

		#region ILineItemField Members

		public object GetDataValue(LineItemDO lineItem)
		{
			return lineItem.Date03;
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
			lineItem.Date03 = newValue as DateTimeOffset?;
			OnFieldChanged();
		}

		#endregion

		#region ISublineItemField Members

		object TransactionFields.ISublineItemField.GetDataValue(SubLineItemDO sublineItem)
		{
			return sublineItem.Date03;
		}

		string TransactionFields.ISublineItemField.GetDataText(SubLineItemDO sublineItem)
		{
			return sublineItem.Date03.ToString();
		}

		void TransactionFields.ISublineItemField.SetDataValue(SubLineItemDO sublineItem, object newValue)
		{
			sublineItem.Date03 = newValue as DateTimeOffset?;
			OnFieldChanged();
		}

		#endregion
	}
}
