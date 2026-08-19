using System;
using FMBusinessObjects.DataObjects;

namespace TransactionFields
{
	/// <summary>
	/// Summary description for IWeightReadingField.
	/// </summary>
	public interface IWeightReadingField
	{
		object GetDataValue(WeightReadingDO agr);
		string GetDataText(WeightReadingDO agr);
		void SetDataValue(WeightReadingDO agr, object newValue);
	}
}
