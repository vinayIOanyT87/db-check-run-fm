namespace TransactionFields
{
	using System;
	using FMBusinessObjects.DataObjects;

	/// <summary>
	/// Summary description for LineItemPresetAmountFG.
	/// </summary>
	public class LineItemPresetAmountFG : LineItemVolumeFG, ILineItemField, ISublineItemField
	{
		public LineItemPresetAmountFG()
		{
		}

		override public string FieldID
		{
			get
			{
				return "LineItem PresetAmount";
			}
		}

		#region ILineItemField Members
		public object GetDataValue(LineItemDO inLineItem)
		{
			if (inLineItem.PresetAmount == null)
			{
				return null;
			}

			return Math.Round(inLineItem.PresetAmount.Value, inLineItem.VolumeDecimalPlaces, MidpointRounding.AwayFromZero);
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
			if (newValue == null)
			{
				inLineItem.PresetAmount = null;
			}
			else
			{
				inLineItem.PresetAmount = (double) newValue;
			}

			OnFieldChanged();
		}
		#endregion

		#region ISublineItemField Members
		object ISublineItemField.GetDataValue(SubLineItemDO inSublineItem)
		{
			if (inSublineItem.PresetAmount == null)
			{
				return null;
			}

			return Math.Round(inSublineItem.PresetAmount.Value, inSublineItem.VolumeDecimalPlaces, MidpointRounding.AwayFromZero);
		}

		string ISublineItemField.GetDataText(SubLineItemDO inSublineItem)
		{
			if (((ISublineItemField) this).GetDataValue(inSublineItem) != null)
			{
				return ((ISublineItemField) this).GetDataValue(inSublineItem).ToString();
			}
			
			return null;
		}

		void ISublineItemField.SetDataValue(SubLineItemDO inSublineItem, object newValue)
		{
			if (newValue == null)
			{
				inSublineItem.PresetAmount = null;
			}
			else
			{
				inSublineItem.PresetAmount = (double) newValue;
			}

			OnFieldChanged();
		}
		#endregion
	}
}
