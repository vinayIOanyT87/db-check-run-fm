using System;
using FMBusinessObjects.DataObjects;

namespace TransactionFields
{
	/// <summary>
	/// Summary description for AGRQuantity.
	/// </summary>
	public class AGRBeginQuantity : AGRQuantityGenerator, IWeightReadingField
	{
		public AGRBeginQuantity()
		{

		}

		public override string FieldID { get { return "AGR BeginQuantityValue"; } }

		#region IGaugeReadingField Members

		public object GetDataValue(WeightReadingDO agr)
		{
			if(agr.BeginQuantity == null)
			{
				return null;
			}
			return  this.ConvertToEquipmentUnits(agr.BeginQuantity.Value);
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
				agr.BeginQuantity = null;
			}
			else
			{
				agr.BeginQuantity = this.ConvertFromEquipmentUnits((double) newValue);
			}
			OnFieldChanged();
		}

		#endregion
	}
}
