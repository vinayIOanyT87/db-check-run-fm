namespace TransactionFields
{
	using FMBusinessObjects.DataObjects;

	/// <summary>
	/// Summary description for LineItemDeliveredGrossManualValueFlagFG.
	/// </summary>
	public class LineItemDeliveredGrossManualValueFlagFG : CheckBoxGenerator, ILineItemField, ISublineItemField
	{
		public LineItemDeliveredGrossManualValueFlagFG()
		{
		}

		public override string FieldID
		{
			get
			{
				return "LineItem DeliveredGrossManualValueFlag";
			}
		}

		#region ILineItemField Members
		public object GetDataValue(LineItemDO inLineItem)
		{
			return inLineItem.Quantity.DeliveredGrossManualValueFlag;
		}

		public string GetDataText(LineItemDO inLineItem)
		{
			return inLineItem.Quantity.DeliveredGrossManualValueFlag.ToString();
		}

		public void SetDataValue(LineItemDO inLineItem, object newValue)
		{
			inLineItem.Quantity.DeliveredGrossManualValueFlag = (bool) newValue;
			OnFieldChanged();
		}
		#endregion

		#region ISublineItemField Members
		object ISublineItemField.GetDataValue(SubLineItemDO inSublineItem)
		{
			return inSublineItem.Quantity.DeliveredGrossManualValueFlag;
		}

		string ISublineItemField.GetDataText(SubLineItemDO inSublineItem)
		{
			return inSublineItem.Quantity.DeliveredGrossManualValueFlag.ToString();
		}

		void ISublineItemField.SetDataValue(SubLineItemDO inSublineItem, object newValue)
		{
			inSublineItem.Quantity.DeliveredGrossManualValueFlag = (bool) newValue;
			OnFieldChanged();
		}
		#endregion
	}
}
