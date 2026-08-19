using System;
using FMBusinessObjects.DataObjects;

namespace TransactionFields
{
	/// <summary>
	/// Summary description for AGRQuantity.
	/// </summary>
	public class AGRFinalQuantity : AGRQuantityGenerator, IWeightReadingField
	{
		public AGRFinalQuantity()
		{

		}

		public override string FieldID { get { return "AGR FinalQuantityValue"; } }

		#region IGaugeReadingField Members

		public object GetDataValue(WeightReadingDO agr)
		{
			if(agr.FinalQuantity == null)
			{
				return null;
			}
			return  this.ConvertToEquipmentUnits(agr.FinalQuantity.Value);
		}

		public string GetDataText(WeightReadingDO agr)
		{
			if (GetDataValue(agr) != null)
			{
				return GetDataValue(agr).ToString();
			}
			else
			{
				return null;
			}
		}

		public void SetDataValue(WeightReadingDO agr, object newValue)
		{
			if(newValue == null)
			{
				agr.FinalQuantity = null;
			}
			else
			{
				agr.FinalQuantity = this.ConvertFromEquipmentUnits((double) newValue);
			}
			OnFieldChanged();
		}

		#endregion
	}
}
