using System;
using System.Globalization;
using System.Xml.Serialization;
using System.Runtime.Serialization;

namespace FMBusinessObjects.DataObjects
{
	[Serializable()]
	public class DateAndTime
	{
		#region Constants
		protected const string TimeFormat = "yyyy'-'MM'-'dd'T'HH':'mm':'sszzz";
		#endregion

		[DataMember]
		[XmlIgnore]
		public DateTimeOffset Value;
		[DataMember]
		public string StandardName;
		[DataMember]
		public string amDesignator = "am";
		[DataMember]
		public string pmDesignator = "pm";
		[DataMember]
		public string timePattern = "hh:mm";
		[DataMember]
		public string timeSeparator = ":";
		[DataMember]
		public string shortDatePattern = "MM/dd/yy";
		[DataMember]
		public string dateSeparator = "/";

		public DateAndTime()
			: this(null)
		{
		}

		public DateAndTime(SiteClass site)
		{
			if (site == null)
			{
				this.Format = DateTimeFormatInfo.CurrentInfo;
				this.StandardName = TimeZone.CurrentTimeZone.StandardName;
			}
			else
			{
				this.Format = site.GetDateTimeFormatInfo();
				this.StandardName = site.TimeZone;
			}
		}

		[XmlElement("Value")]
		public string TimeString { get { return Value.ToString(TimeFormat); } set { Value = DateTimeOffset.ParseExact(value, TimeFormat, null); } }

		public override bool Equals(object obj)
		{
			if (!typeof(DateAndTime).IsInstanceOfType(obj))
			{
				return false;
			}

			return Value.Equals(((DateAndTime)obj).Value);
		}

		public override int GetHashCode()
		{
			return Value.GetHashCode();
		}

		public override string ToString()
		{
			return Value.ToString("G",Format);
		}

		[XmlIgnore]
		public DateTimeFormatInfo Format
		{
			get
			{
				DateTimeFormatInfo format = new DateTimeFormatInfo();
				format.AMDesignator = amDesignator;
				format.PMDesignator = pmDesignator;
				format.LongTimePattern = timePattern;
				format.ShortTimePattern = timePattern;
				format.TimeSeparator = timeSeparator;
				format.ShortDatePattern = shortDatePattern;
				format.DateSeparator = dateSeparator;
				return format;
			}
			set
			{
				amDesignator = value.AMDesignator;
				pmDesignator = value.PMDesignator;
				timePattern = value.ShortTimePattern;
				timeSeparator = value.TimeSeparator;
				shortDatePattern = value.ShortDatePattern;
				dateSeparator = value.DateSeparator;
			}
		}

		public DateTimeOffset UTCValue
		{
			get
			{
				return this.Value.UtcDateTime;
			}

			set
			{
				this.Value = value;
			}
		}
	}
}