using System;
using FMBusinessObjects.DataObjects;

namespace TransactionFields
{
	/// <summary>
	/// Summary description for LineItemOdometerHoursFG.
	/// </summary>
	public class LineItemOdometerHoursFG : NumericTextFieldGenerator, ILineItemField
	{
		public LineItemOdometerHoursFG()
		{
		}

		public override string FieldID
		{
			get
			{
				return "LineItem OdometerHours";
			}
		}

		public override ENumericType NumericType
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
			if (lineItem.OdometerHours == null)
				return null;
			else
				return lineItem.OdometerHours.Value;
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
				lineItem.OdometerHours = null;
			else
				lineItem.OdometerHours = new double?((double)newValue);
			OnFieldChanged();
		}

		#endregion
	}
}
