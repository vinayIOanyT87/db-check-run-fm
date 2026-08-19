using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Xml.Serialization;

namespace FMBusinessObjects.DataObjects
{
	[KnownType(typeof(GregorianCalendar))]
	[Serializable]
	[DataContract]
	public class Time
	{
		#region Constants
		protected const string TimeFormat = "yyyy'-'MM'-'dd'T'HH':'mm':'sszzz";
		#endregion

		[DataMember]
		[XmlIgnore]
		public DateTimeOffset Value;
		[DataMember]
		public string amDesignator = "am";
		[DataMember]
		public string pmDesignator = "pm";
		[DataMember]
		public string timePattern = "hh:mm";
		[DataMember]
		public string timeSeparator = ":";

		public Time()
			: this((SiteClass) null)
		{
		}

		public Time(DateTimeFormatInfo dateTimeFormatInfo)
		{
			if (dateTimeFormatInfo == null)
			{
				this.Format = DateTimeFormatInfo.CurrentInfo;
			}
			else
			{
				this.Format = dateTimeFormatInfo;
			}
		}

		public Time(SiteClass Site)
		{
			if (Site == null)
			{
				this.Format = DateTimeFormatInfo.CurrentInfo;
			}
			else
			{
				this.Format = Site.GetDateTimeFormatInfo();
			}
		}

		[XmlElement("Value")]
		public string TimeString { get { return Value.ToString(TimeFormat); } set { Value = DateTimeOffset.ParseExact(value, TimeFormat, null); } }

		public override bool Equals(object obj)
		{
			if (!typeof(Time).IsInstanceOfType(obj))
			{
				return false;
			}

			return Value.Equals(((Time)obj).Value);
		}

		public override int GetHashCode()
		{
			return Value.GetHashCode();
		}

		public override string ToString()
		{
			return Value.ToString("t", Format);
		}

		[XmlIgnore]
		public DateTimeFormatInfo Format
		{
			get
			{
				DateTimeFormatInfo format = new DateTimeFormatInfo();
				format.AMDesignator = this.amDesignator;
				format.PMDesignator = this.pmDesignator;
				format.LongTimePattern = this.timePattern;
				format.ShortTimePattern = this.timePattern;
				format.TimeSeparator = this.timeSeparator;

				return format;
			}
			set
			{
				this.pmDesignator = value.PMDesignator;
				this.amDesignator = value.AMDesignator;
				this.timePattern = value.ShortTimePattern;
				this.timeSeparator = value.TimeSeparator;
			}
		}
	}
}
