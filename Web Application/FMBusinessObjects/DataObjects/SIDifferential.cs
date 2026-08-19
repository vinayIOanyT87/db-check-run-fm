namespace FMBusinessObjects.DataObjects
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.Security;
    using System.Xml.Serialization;

    using Varec.CommonComponents.EngineeringUnitsLibrary;

    [Serializable]
	public class SIDifferential : SIDouble
    {
        public SIDifferential()
        {
        }

        public SIDifferential( EngineeringUnit units,
                                        NumberFormatInfo format,
                                        double value )
        {
            this.Units=units;
            this.Format=format;
            this.SIValue=value;
        }

		[XmlIgnore]
		[SuppressMessage("ReSharper", "CompareOfFloatsByEqualityOperator")]
		public override double Value
        {
            [SecuritySafeCritical]
            get
            {
                double result=0.0;

                // The following deals with an error in UnitConvert which
                // is not catching divide by 0 
                if (this.Units == EngineeringUnit.FmdDegApi
					&& this.SIValue == 0.0)
                    return 1e20;

                double input= this.SIValue;

                if (this.Units != 0)
                {
                    try
                    {
							  EngineeringUnits.Convert(input,
                                                SIUnits(this.Units ),
                                                ref result, this.Units, this.ReferenceTemperature );
                    }
                    catch
                    {
                        string abbrev = EngineeringUnits.GetUnitAbbreviation(this.Units);
                        throw new Exception( "Invalid value conversion from SI units : SIValue = " + this.SIValue.ToString(CultureInfo.CurrentCulture) + " Units=" + abbrev );
                    }

                }
                else
                {
                    result = this.SIValue;
                }

                if (this.Units == EngineeringUnit.FmtDegF)
                    result-=32;


                if (this.Units != EngineeringUnit.FmlFtIn16Th
				&& this.Units != EngineeringUnit.FmlFtIn8Th)
                    result = Math.Round( result, this.Format.NumberDecimalDigits, MidpointRounding.AwayFromZero );

                return result;
            }
            [SecuritySafeCritical]
            set
            {
                if (this.Units == EngineeringUnit.FmtDegF)
                    value+=32;

                if (this.Units == EngineeringUnit.FmdDegApi && value >= 1e20)
                {
                    this.SIValue = 0;
                    return;
                }

                if (this.Units != 0)
                {
                    try
                    {
							  EngineeringUnits.Convert(value, this.Units,
                                                ref this.SIValue,
                                                SIUnits(this.Units ), this.ReferenceTemperature );
                    }
                    catch
                    {
                        string abbrev = EngineeringUnits.GetUnitAbbreviation(this.Units);
                        throw new Exception( "Invalid value conversion to SI units: SIValue = " + value.ToString() + " Units=" + abbrev );
                    }
                }
                else
                {
                    this.SIValue = value;
                }

                if (this.Units == EngineeringUnit.FmdDegApi) this.SIValue=1075-value * this.SIValue * 1000.0/131.5;

            }
        }

        public override int GetHashCode()
        {
            return this.SIValue.GetHashCode();
        }

        public override bool Equals( object obj )
        {
            if (!typeof( SIDifferential ).IsInstanceOfType( obj ))
                return false;

            return this.Value.Equals( ((SIDifferential)obj).Value );
        }

    }

}
