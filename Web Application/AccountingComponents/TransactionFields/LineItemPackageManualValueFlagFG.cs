namespace TransactionFields
{
	using FMBusinessObjects.DataObjects;

	/// <summary>
	/// Summary description for LineItemPackageManualValueFlagFG.
	/// </summary>
	public class LineItemPackageManualValueFlagFG : CheckBoxGenerator, ILineItemField, ISublineItemField
	{
		public LineItemPackageManualValueFlagFG()
		{
		}

		public override string FieldID
		{
			get
			{
				return "LineItem PackageManualValueFlag";
			}
		}

		#region ILineItemField Members
		public object GetDataValue(LineItemDO inLineItem)
		{
			return inLineItem.Quantity.PackageManualValueFlag;
		}

		public string GetDataText(LineItemDO inLineItem)
		{
			return inLineItem.Quantity.PackageManualValueFlag.ToString();
		}

		public void SetDataValue(LineItemDO inLineItem, object newValue)
		{
			inLineItem.Quantity.PackageManualValueFlag = (bool) newValue;
			OnFieldChanged();
		}
		#endregion

		#region ISublineItemField Members
		object ISublineItemField.GetDataValue(SubLineItemDO inSublineItem)
		{
			return inSublineItem.Quantity.PackageManualValueFlag;
		}

		string ISublineItemField.GetDataText(SubLineItemDO inSublineItem)
		{
			return inSublineItem.Quantity.PackageManualValueFlag.ToString();
		}

		void ISublineItemField.SetDataValue(SubLineItemDO inSublineItem, object newValue)
		{
			inSublineItem.Quantity.PackageManualValueFlag = (bool) newValue;
			OnFieldChanged();
		}
		#endregion
	}
}
