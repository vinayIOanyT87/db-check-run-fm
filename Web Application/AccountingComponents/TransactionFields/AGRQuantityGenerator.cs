using System;
using FMBusinessObjects.DataObjects;

namespace TransactionFields
{
	/// <summary>
	/// Summary description for AGRQuantityGenerator.
	/// </summary>
	abstract public class AGRQuantityGenerator : NumericTextFieldGenerator
	{
		public AGRQuantityGenerator()
		{
		}

		public override ENumericType NumericType { get { return ENumericType.Double; } }
		public override SITE_VARIABLE_TYPE UnitType
		{
			get
			{
				return SITE_VARIABLE_TYPE.MASS;
			}
		}

		protected double ConvertFromEquipmentUnits(double x)
		{
			GaugeReadingsengineeringUnitser gruc = new GaugeReadingsengineeringUnitser();
			return gruc.ConvertGaugeReadingFromEquipmentUnits(x, ref trans, transContext);
		}

		protected double ConvertToEquipmentUnits(double x)
		{
			GaugeReadingsengineeringUnitser gruc = new GaugeReadingsengineeringUnitser();
			return gruc.ConvertGaugeReadingToEquipmentUnits(x, ref trans, transContext);
		}

	}
}
