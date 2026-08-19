using FMBusinessObjects.DataObjects;


namespace TransactionFields
{
	public class LineItemCleanLineDeductItemFG : CheckBoxGenerator, ILineItemField, ISublineItemField
	{
		public LineItemCleanLineDeductItemFG()
		{
		}

		public override string FieldID
		{
			get
			{
				return "LineItem CleanLineDeductItem";
			}
		}

		#region ILineItemField Members

		public object GetDataValue(LineItemDO lineItem)
		{
			return lineItem.CleanLineDeductProduct;
		}

		public string GetDataText(LineItemDO lineItem)
		{
			return lineItem.CleanLineDeductProduct.ToString();
		}

		public void SetDataValue(LineItemDO lineItem, object newValue)
		{
			lineItem.CleanLineDeductProduct = (bool)newValue;
			OnFieldChanged();
		}

		#endregion

		#region ISublineItemField Members

		object TransactionFields.ISublineItemField.GetDataValue(SubLineItemDO sublineItem)
		{
			return sublineItem.CleanLineDeductProduct;
		}

		string TransactionFields.ISublineItemField.GetDataText(SubLineItemDO sublineItem)
		{
			return sublineItem.CleanLineDeductProduct.ToString();
		}

		void TransactionFields.ISublineItemField.SetDataValue(SubLineItemDO sublineItem, object newValue)
		{
			sublineItem.CleanLineDeductProduct = (bool) newValue;
			OnFieldChanged();
		}

		#endregion
	}
}
