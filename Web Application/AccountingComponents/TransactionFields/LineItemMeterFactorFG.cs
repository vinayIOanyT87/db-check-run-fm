using System;
using FMBusinessObjects.DataObjects;

namespace TransactionFields
{
	/// <summary>
	/// Summary description for LineItemMeterFactorFG.
	/// </summary>
	public class LineItemMeterFactorFG : NumericTextFieldGenerator, ILineItemField, ISublineItemField
	{
		public LineItemMeterFactorFG()
		{
			
		}

		public override string FieldID { get { return "LineItem MeterFactor"; } }
		public override ENumericType NumericType { get { return ENumericType.Double; } }
		public override SITE_VARIABLE_TYPE UnitType
		{ get { return SITE_VARIABLE_TYPE.DEFAULT; } }

		#region ILineItemField Members

		public object GetDataValue(LineItemDO lineItem)
		{
			if(lineItem.MeterReading.MeterFactor == null)
			{
				return null;
			}
			return lineItem.MeterReading.MeterFactor.Value;
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
			if(newValue == null)
			{
				lineItem.MeterReading.MeterFactor = null;
			}
			else
			{
				lineItem.MeterReading.MeterFactor = new double?((double) newValue);
			}
			OnFieldChanged();
		}

		#endregion

		#region ISublineItemField Members

		object TransactionFields.ISublineItemField.GetDataValue(
			SubLineItemDO sublineItem)
		{
			if(sublineItem.MeterReading.MeterFactor == null)
			{
				return null;
			}
			return sublineItem.MeterReading.MeterFactor.Value;
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

		void TransactionFields.ISublineItemField.SetDataValue(
			SubLineItemDO sublineItem, object newValue)
		{
			if(newValue == null)
			{
				sublineItem.MeterReading.MeterFactor = null;
			}
			else
			{
				sublineItem.MeterReading.MeterFactor = new double?((double) newValue);
			}
			OnFieldChanged();
		}

		#endregion
	}
}
