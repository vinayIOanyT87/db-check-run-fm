namespace TransactionFields
{
	using FMBusinessObjects.DataObjects;

	/// <summary>
	/// Summary description for LineItemGrossManualValueFlagFG.
	/// </summary>
	public class LineItemGrossManualValueFlagFG : CheckBoxGenerator, ILineItemField, ISublineItemField
	{
		public LineItemGrossManualValueFlagFG()
		{
		}

		public override string FieldID
		{
			get
			{
				return "LineItem GrossManualValueFlag";
			}
		}

		#region ILineItemField Members
		public object GetDataValue(LineItemDO inLineItem)
		{
			return inLineItem.Quantity.GrossManualValueFlag;
		}

		public string GetDataText(LineItemDO inLineItem)
		{
			return inLineItem.Quantity.GrossManualValueFlag.ToString();
		}

		public void SetDataValue(LineItemDO inLineItem, object newValue)
		{
			inLineItem.Quantity.GrossManualValueFlag = (bool) newValue;
			OnFieldChanged();
		}
		#endregion

		#region ISublineItemField Members
		object ISublineItemField.GetDataValue(SubLineItemDO inSublineItem)
		{
			return inSublineItem.Quantity.GrossManualValueFlag;
		}

		string ISublineItemField.GetDataText(SubLineItemDO inSublineItem)
		{
			return inSublineItem.Quantity.GrossManualValueFlag.ToString();
		}

		void ISublineItemField.SetDataValue(SubLineItemDO inSublineItem, object newValue)
		{
			inSublineItem.Quantity.GrossManualValueFlag = (bool) newValue;
			OnFieldChanged();
		}
		#endregion
	}
}
