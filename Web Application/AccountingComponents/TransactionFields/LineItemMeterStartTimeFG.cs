using System;
using FMBusinessObjects.DataObjects;

namespace TransactionFields
{
	/// <summary>
	/// Summary description for LineItemMeterStartTimeFG.
	/// </summary>
	public class LineItemMeterStartTimeFG : DateTimeGenerator, ILineItemField, ISublineItemField
	{
		public LineItemMeterStartTimeFG()
		{

		}

		public override string FieldID { get { return "LineItem MeterStartDateTime"; } }

		#region ILineItemField Members

		public object GetDataValue(LineItemDO lineItem)
		{
			if (lineItem.MeterReading.StartDateTime == null)
			{
				return null;
			}
			return lineItem.MeterReading.StartDateTime.Value;
		}

		public string GetDataText(LineItemDO lineItem)
		{
			if (GetDataValue(lineItem) != null)
			{
				return GetDataValue(lineItem).ToString();
			}
			else
			{
				return null;
			}
		}

		public void SetDataValue(LineItemDO lineItem, object newValue)
		{
			lineItem.MeterReading.StartDateTime = newValue as DateTimeOffset?;
			OnFieldChanged();
		}

		#endregion

		#region ISublineItemField Members

		object TransactionFields.ISublineItemField.GetDataValue(SubLineItemDO sublineItem)
		{
			if (sublineItem.MeterReading.StartDateTime == null)
			{
				return null;
			}
			return sublineItem.MeterReading.StartDateTime.Value;
		}

		string TransactionFields.ISublineItemField.GetDataText(SubLineItemDO sublineItem)
		{
			if (((ISublineItemField)this).GetDataValue(sublineItem) != null)
			{
				return ((ISublineItemField)this).GetDataValue(sublineItem).ToString();
			}
			else
			{
				return null;
			}
		}

		void TransactionFields.ISublineItemField.SetDataValue(SubLineItemDO sublineItem, object newValue)
		{
			sublineItem.MeterReading.StartDateTime = newValue as DateTimeOffset?;
			OnFieldChanged();
		}

		public override string GetFormattedValue()
		{
			object dateValue = GetDataValue();
			if (dateValue == null)
			{
				return string.Empty;
			}
			if (dateValue is DateTimeOffset)
			{
				var date = (DateTimeOffset)dateValue;
				return this.fieldGenerator.accountingSite.FormatDateTime(date);
			}

			return null;
		}
		#endregion
	}
}
