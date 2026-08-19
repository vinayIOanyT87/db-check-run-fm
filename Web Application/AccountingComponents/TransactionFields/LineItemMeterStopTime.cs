using System;
using FMBusinessObjects.DataObjects;

namespace TransactionFields
{
	/// <summary>
	/// Summary description for LineItemMeterStopTimeFG.
	/// </summary>
	public class LineItemMeterStopTimeFG : DateTimeGenerator, ILineItemField, ISublineItemField
	{
		public LineItemMeterStopTimeFG()
		{

		}

		public override string FieldID { get { return "LineItem MeterStopDateTime"; } }

		#region ILineItemField Members

		public object GetDataValue(LineItemDO lineItem)
		{
			if (lineItem.MeterReading.StopDateTime == null)
			{
				return null;
			}
			return lineItem.MeterReading.StopDateTime.Value;
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
			lineItem.MeterReading.StopDateTime = newValue as DateTimeOffset?;
			OnFieldChanged();
		}

		#endregion

		#region ISublineItemField Members

		object TransactionFields.ISublineItemField.GetDataValue(SubLineItemDO sublineItem)
		{
			if (sublineItem.MeterReading.StopDateTime == null)
			{
				return null;
			}
			return sublineItem.MeterReading.StopDateTime.Value;
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
			sublineItem.MeterReading.StopDateTime = newValue as DateTimeOffset?;
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
