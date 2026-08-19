using FMBusinessObjects.DataObjects;

namespace TransactionFields
{
	public class LineItemCleanLineItemFG : CheckBoxGenerator, ILineItemField, ISublineItemField
	{
		public LineItemCleanLineItemFG()
		{
		}

		public override string FieldID
		{
			get
			{
				return "LineItem CleanLineItem";
			}
		}

		#region ILineItemField Members

		public object GetDataValue(LineItemDO lineItem)
		{
			return lineItem.CleanLineProduct;
		}

		public string GetDataText(LineItemDO lineItem)
		{
			return lineItem.CleanLineProduct.ToString();
		}

		public void SetDataValue(LineItemDO lineItem, object newValue)
		{
			lineItem.CleanLineProduct = (bool) newValue;
			OnFieldChanged();
		}

		#endregion

		#region ISublineItemField Members

		object TransactionFields.ISublineItemField.GetDataValue(SubLineItemDO sublineItem)
		{
			return sublineItem.CleanLineProduct;
		}

		string TransactionFields.ISublineItemField.GetDataText(SubLineItemDO sublineItem)
		{
			return sublineItem.CleanLineProduct.ToString();
		}

		void TransactionFields.ISublineItemField.SetDataValue(SubLineItemDO sublineItem, object newValue)
		{
			sublineItem.CleanLineProduct = (bool) newValue;
			OnFieldChanged();
		}

		#endregion
	}
}
