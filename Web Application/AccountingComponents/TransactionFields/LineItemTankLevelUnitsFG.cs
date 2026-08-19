using System;
using FMBusinessObjects.DataObjects;

namespace TransactionFields
{
	/// <summary>
	/// Summary description for LineItemTankLevelUnitsFG.
	/// 05-22-2008 V. Thompson
	/// Line Item field added for ADF
	/// </summary>
	public class LineItemTankLevelUnitsFG : NumericTextFieldGenerator, ILineItemField
	{
		public LineItemTankLevelUnitsFG()
		{
		}

		public override string FieldID
		{
			get
			{
				return "LineItem TankLevelUnits";
			}
		}

		public override ENumericType NumericType
		{
			get
			{
				return ENumericType.Integer;
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
			if (lineItem.TankLevelUnits == null)
				return null;
			else
				return lineItem.TankLevelUnits.Value;
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
				lineItem.TankLevelUnits = null;
			else
				lineItem.TankLevelUnits = new int?((int) newValue);
			OnFieldChanged();
		}

		#endregion
	}
}
