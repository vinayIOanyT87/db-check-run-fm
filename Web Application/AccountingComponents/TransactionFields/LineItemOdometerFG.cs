using System;
using FMBusinessObjects.DataObjects;

namespace TransactionFields
{
	/// <summary>
	/// Summary description for LineItemOdometerFG.
	/// </summary>
	public class LineItemOdometerFG : NumericTextFieldGenerator, ILineItemField
	{
		public LineItemOdometerFG()
		{
		}

		public override string FieldID
		{
			get
			{
				return "LineItem Odometer";
			}
		}

		public override TransactionFields.NumericTextFieldGenerator.ENumericType NumericType
		{
			get
			{
				return ENumericType.Double;
			}
		}

		/// <summary>
		/// This property will return the unit type which is set to default.
		/// </summary>
		public override SITE_VARIABLE_TYPE UnitType
		{
			get { return SITE_VARIABLE_TYPE.DEFAULT; }
		}

		#region ILineItemField Members

		public object GetDataValue(LineItemDO lineItem)
		{
			if (lineItem.Odometer == null)
				return null;
			else
				return lineItem.Odometer.Value;
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
			if (newValue == null)
				lineItem.Odometer = null;
			else
				lineItem.Odometer = new double?((double)newValue);
			OnFieldChanged();
		}

		#endregion
	}
}
