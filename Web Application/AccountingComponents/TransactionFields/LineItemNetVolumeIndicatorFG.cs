namespace TransactionFields
{
    using FMBusinessObjects.DataObjects;

    public class LineItemNetVolumeIndicatorFG : CheckBoxGenerator, ILineItemField
    {
		public LineItemNetVolumeIndicatorFG()
		{
		}

		public override string FieldID
		{
			get
			{
				return "LineItem NetVolumeIndicator";
			}
		}

		#region ILineItemField Members
		public object GetDataValue(LineItemDO inLineItem)
		{
			return inLineItem.NetVolumeIndicator;
		}

		public string GetDataText(LineItemDO inLineItem)
		{
			return inLineItem.NetVolumeIndicator.ToString();
		}

		public void SetDataValue(LineItemDO inLineItem, object newValue)
		{
			inLineItem.NetVolumeIndicator = (bool)newValue;
			OnFieldChanged();
		}
		#endregion
	}
}
