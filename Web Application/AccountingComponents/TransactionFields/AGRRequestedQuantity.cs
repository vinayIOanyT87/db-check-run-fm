using System;
using FMBusinessObjects.DataObjects;

namespace TransactionFields
{
	/// <summary>
	/// Summary description for AGRQuantity.
	/// </summary>
	public class AGRRequestedQuantity : AGRQuantityGenerator, IWeightReadingField
	{
		public AGRRequestedQuantity()
		{

		}

		public override string FieldID { get { return "AGR RequestedQuantityValue"; } }

		#region IGaugeReadingField Members

		public object GetDataValue(WeightReadingDO agr)
		{
			if(agr.RequestedQuantity == null)
			{
				return null;
			}
			return this.ConvertToEquipmentUnits(agr.RequestedQuantity.Value);
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
				agr.RequestedQuantity = null;
			}
			else
			{
				agr.RequestedQuantity = this.ConvertFromEquipmentUnits((double) newValue);
			}
			OnFieldChanged();
		}

		#endregion
	}
}
