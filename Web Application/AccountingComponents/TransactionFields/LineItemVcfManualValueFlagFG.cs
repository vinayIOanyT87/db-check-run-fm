namespace TransactionFields
{
	using FMBusinessObjects.DataObjects;

	/// <summary>
	/// Summary description for LineItemVcfManualValueFlagFG.
	/// </summary>
	public class LineItemVcfManualValueFlagFG : CheckBoxGenerator, ILineItemField, ISublineItemField
	{
		public LineItemVcfManualValueFlagFG()
		{
		}

		public override string FieldID
		{
			get
			{
				return "LineItem VcfManualValueFlag";
			}
		}

		#region ILineItemField Members
		public object GetDataValue(LineItemDO inLineItem)
		{
			return inLineItem.Quantity.VcfManualValueFlag;
		}

		public string GetDataText(LineItemDO inLineItem)
		{
			return inLineItem.Quantity.VcfManualValueFlag.ToString();
		}

		public void SetDataValue(LineItemDO inLineItem, object newValue)
		{
			inLineItem.Quantity.VcfManualValueFlag = (bool) newValue;
			OnFieldChanged();
		}
		#endregion

		#region ISublineItemField Members
		object ISublineItemField.GetDataValue(SubLineItemDO inSublineItem)
		{
			return inSublineItem.Quantity.VcfManualValueFlag;
		}

		string ISublineItemField.GetDataText(SubLineItemDO inSublineItem)
		{
			return inSublineItem.Quantity.VcfManualValueFlag.ToString();
		}

		void ISublineItemField.SetDataValue(SubLineItemDO inSublineItem, object newValue)
		{
			inSublineItem.Quantity.VcfManualValueFlag = (bool) newValue;
			OnFieldChanged();
		}
		#endregion
	}
}
