using System;
using FMBusinessObjects.DataObjects;

namespace TransactionFields
{
	/// <summary>
	/// Summary description for LineItemTankLevelUnitsFG.
	/// 05-22-2008 V. Thompson
	/// Line Item field added for ADF
	/// </summary>
	public class LineItemAlternativeNetVolumeFG : NumericTextFieldGenerator, ILineItemField
	{
		public LineItemAlternativeNetVolumeFG()
		{
		}

		public override string FieldID
		{
			get
			{
				return "LineItem AlternativeNetVolume";
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
			if (lineItem.AlternativeNetVolume == null)
				return null;
			else
				return lineItem.AlternativeNetVolume.Value;
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
				lineItem.AlternativeNetVolume = null;
			else
				lineItem.AlternativeNetVolume = new double?((double) newValue);
			OnFieldChanged();
		}

		#endregion
	}
}
