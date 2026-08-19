namespace TransactionFields
{
	using FMBusinessObjects.DataObjects;

	/// <summary>
	/// Summary description for LineItemMassManualValueFlagFG.
	/// </summary>
	public class LineItemMassManualValueFlagFG : CheckBoxGenerator, ILineItemField, ISublineItemField
	{
		public LineItemMassManualValueFlagFG()
		{
		}

		public override string FieldID
		{
			get
			{
				return "LineItem MassManualValueFlag";
			}
		}

		#region ILineItemField Members
		public object GetDataValue(LineItemDO inLineItem)
		{
			return inLineItem.Quantity.MassManualValueFlag;
		}

		public string GetDataText(LineItemDO inLineItem)
		{
			return inLineItem.Quantity.MassManualValueFlag.ToString();
		}

		public void SetDataValue(LineItemDO inLineItem, object newValue)
		{
			inLineItem.Quantity.MassManualValueFlag = (bool) newValue;
			OnFieldChanged();
		}
		#endregion

		#region ISublineItemField Members
		object ISublineItemField.GetDataValue(SubLineItemDO inSublineItem)
		{
			return inSublineItem.Quantity.MassManualValueFlag;
		}

		string ISublineItemField.GetDataText(SubLineItemDO inSublineItem)
		{
			return inSublineItem.Quantity.MassManualValueFlag.ToString();
		}

		void ISublineItemField.SetDataValue(SubLineItemDO inSublineItem, object newValue)
		{
			inSublineItem.Quantity.MassManualValueFlag = (bool) newValue;
			OnFieldChanged();
		}
		#endregion
	}
}
