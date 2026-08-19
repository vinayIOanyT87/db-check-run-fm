namespace TransactionFields
{
	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;

	/// <summary>
	/// Summary description for LineItemReceiptVarianceFG.
	/// </summary>
	public class LineItemReceiptVarianceFG : LineItemVolumeFG, ILineItemField
	{
		public LineItemReceiptVarianceFG()
		{
		}

		public override string FieldID
		{
			get
			{
				return "LineItem ReceiptVariance";
			}
		}

		#region ILineItemField Members
		public object GetDataValue(LineItemDO inLineItem)
		{
			if (FMChannelHelper.MakeCall<IHardwareKey, bool>(x => x.IsADFKey()))
			{
				// if you change this, change all places marked with [ReceiptPriceQuantity]
				double recVariance = inLineItem.Quantity.NetInventoryChange -
													(inLineItem.AlternativeNetVolume == null ? 0.0 : inLineItem.AlternativeNetVolume.Value);
				inLineItem.ReceiptVariance = recVariance;
			}

			return inLineItem.ReceiptVariance;
		}

		public string GetDataText(LineItemDO inLineItem)
		{
			if (GetDataValue(inLineItem) != null)
			{
				return GetDataValue(inLineItem).ToString();
			}

			return null;
		}

		public void SetDataValue(LineItemDO inLineItem, object newValue)
		{
			if (FMChannelHelper.MakeCall<IHardwareKey, bool>(x => x.IsADFKey()))
			{
				double recVariance = (inLineItem.AlternativeNetVolume == null ? 0.0 : inLineItem.AlternativeNetVolume.Value);
				inLineItem.ReceiptVariance = recVariance;
			}
			else if ((newValue == null) || (newValue.Equals(string.Empty)))
			{
				inLineItem.ReceiptVariance = null;
			}
			else
			{
				inLineItem.ReceiptVariance = (double) newValue;
			}

			OnFieldChanged();
		}
		#endregion
	}
}
