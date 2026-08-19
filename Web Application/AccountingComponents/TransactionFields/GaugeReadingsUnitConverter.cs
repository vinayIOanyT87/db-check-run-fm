namespace TransactionFields
{
    using System;
    using System.Globalization;

    using FMBusinessObjects.BusinessInterfaces;
    using FMBusinessObjects.ChannelFactories;
    using FMBusinessObjects.DataObjects;

    using Varec.CommonComponents.EngineeringUnitsLibrary;

    /// <summary>
	/// Summary description for GaugeReadingsengineeringUnitser.
	/// </summary>
	public class GaugeReadingsengineeringUnitser
	{
        public double ConvertGaugeReadingFromEquipmentUnits ( double x, ref TransactionDO trans, TransactionContext context )
		{
			return this.ConvertGaugeReading ( x, true, ref trans, context );
		}

		public double ConvertGaugeReadingToEquipmentUnits ( double x, ref TransactionDO trans, TransactionContext context )
		{
			return this.ConvertGaugeReading ( x, false, ref trans, context );
		}

		public double ConvertGaugeReading ( double x,
											bool toSiteUnits,
											ref TransactionDO trans,
											TransactionContext context )
		{
			EquipmentClass eq = null;
			var transTemp = trans;

			// Determine the first Destination Equipment with compartments
			// It can only be equipment 1 or equipment 2.
			if (trans.DestinationEQ1.EquipmentGuid != Guid.Empty
				&& ( trans.DestinationEQ1.EquipmentType == EquipmentTypeClass.TypeID ( EQUIPMENT_TYPE.AIRCRAFT_TYPE )
				|| trans.DestinationEQ1.EquipmentType == EquipmentTypeClass.TypeID ( EQUIPMENT_TYPE.BARGE_TYPE )
				|| trans.DestinationEQ1.EquipmentType == EquipmentTypeClass.TypeID ( EQUIPMENT_TYPE.RAILCAR_TYPE )
				|| trans.DestinationEQ1.EquipmentType == EquipmentTypeClass.TypeID ( EQUIPMENT_TYPE.SHIP_TYPE )
				|| trans.DestinationEQ1.EquipmentType == EquipmentTypeClass.TypeID ( EQUIPMENT_TYPE.TANKER_TYPE )
				|| trans.DestinationEQ1.EquipmentType == EquipmentTypeClass.TypeID ( EQUIPMENT_TYPE.TRAILER_TYPE ) ))
			{
				eq = FMChannelHelper.MakeCall<IEquipments, EquipmentClass>(
																	 r =>
																	 r.Get(context.security, transTemp.DestinationEQ1.EquipmentGuid)
																);
				trans = transTemp;
			}

			else if (trans.DestinationEQ2.EquipmentGuid != Guid.Empty
				&& ( trans.DestinationEQ2.EquipmentType == EquipmentTypeClass.TypeID ( EQUIPMENT_TYPE.AIRCRAFT_TYPE )
				|| trans.DestinationEQ2.EquipmentType == EquipmentTypeClass.TypeID ( EQUIPMENT_TYPE.BARGE_TYPE )
				|| trans.DestinationEQ2.EquipmentType == EquipmentTypeClass.TypeID ( EQUIPMENT_TYPE.RAILCAR_TYPE )
				|| trans.DestinationEQ2.EquipmentType == EquipmentTypeClass.TypeID ( EQUIPMENT_TYPE.SHIP_TYPE )
				|| trans.DestinationEQ2.EquipmentType == EquipmentTypeClass.TypeID ( EQUIPMENT_TYPE.TANKER_TYPE )
				|| trans.DestinationEQ2.EquipmentType == EquipmentTypeClass.TypeID ( EQUIPMENT_TYPE.TRAILER_TYPE ) ))
			{
				eq = FMChannelHelper.MakeCall<IEquipments, EquipmentClass>(
																	 r =>
																	 r.Get(context.security, transTemp.DestinationEQ2.EquipmentGuid)
																);
				trans = transTemp;
			}


			if (eq == null)
			{
				return x;
			}

			EngineeringUnit eqUnit = eq.MassUnits;
			EngineeringUnit siteUnit = context.accountingSite.CurrentSite.MassUnits;

			if (siteUnit == eqUnit)
			{
				return x;
			}

			EngineeringUnit toUnit;
			EngineeringUnit fromUnit;
			if (toSiteUnits)
			{
				toUnit = siteUnit;
				fromUnit = eqUnit;
			}
			else
			{
				toUnit = eqUnit;
				fromUnit = siteUnit;
			}

			//If the equipment units are different than the site units, 
			//convert the entered value from equipment units to site units.
			NumberFormatInfo format = context.accountingSite.CurrentSite.GetNumberFormatInfo ( SITE_VARIABLE_TYPE.MASS );
			double dSpecial = 0;
			return EngineeringUnits.Convert ( x, fromUnit, toUnit, dSpecial );
		}
	}
}
