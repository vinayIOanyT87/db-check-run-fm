using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace FMBusinessObjects.DataObjects
{
	[DataContract]
   [Serializable]
    public class FMDecimal
    {
		[DataMember]
		public Decimal Value;
		[DataMember]
		public int numberDecimalDigits = 0;
		[DataMember]
		public int[] numberGroupSizes = { 3 };
		[DataMember]
		public string numberDecimalSeparator = ".";
		[DataMember]
		public string numberGroupSeparator = ",";

        public FMDecimal()
        {
            this.Format=NumberFormatInfo.CurrentInfo;
        }

        public FMDecimal( NumberFormatInfo Format )
        {
            this.Format=Format;
        }

		[XmlIgnore]
		public NumberFormatInfo Format
		{
			get
			{
				NumberFormatInfo format = new NumberFormatInfo();

				format.NumberDecimalDigits = numberDecimalDigits;
				format.NumberGroupSizes = numberGroupSizes;
				format.NumberDecimalSeparator = numberDecimalSeparator;
				format.NumberGroupSeparator = numberGroupSeparator;

				return format;
			}
			set
			{
				numberDecimalDigits = value.NumberDecimalDigits;
				numberGroupSizes = value.NumberGroupSizes;
				numberDecimalSeparator = value.NumberDecimalSeparator;
				numberGroupSeparator = value.NumberGroupSeparator;
			}
		}

        public override string ToString()
        {
            return Value.ToString( "N", Format );
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        public override bool Equals( object obj )
        {
            if (!typeof( FMDecimal ).IsInstanceOfType( obj ))
                return false;

            return Value.Equals( ((FMDecimal)obj).Value );
        }

    }
}
