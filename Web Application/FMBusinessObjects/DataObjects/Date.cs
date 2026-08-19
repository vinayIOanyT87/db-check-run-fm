using System;
using System.Globalization;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using FMBusinessObjects.UtilityObjects;

namespace FMBusinessObjects.DataObjects
{
	[Serializable]
	[DataContract]
	public class Date
	{
		#region Constants	
		protected const string TimeFormat = "yyyy'-'MM'-'dd'T'HH':'mm':'sszzz";
		#endregion

		#region Public data members


		[DataMember]
		public string StandardName;

		[DataMember]
		public string shortDatePattern;

		[DataMember]
		public string dateSeparator;
		#endregion

		[XmlIgnore]
		[DataMember]
		private DateTimeOffset dateOnlyValue;

		#region Constructors
		/// <summary>
		/// This is the default constructor for the Date Class.
		/// </summary>
		public Date()
			: this((string) null, (DateTimeFormatInfo) null)
		{
		}


		/// <summary>
		/// This constructor will initialize the Date Class to the given date time format
		/// info object.
		/// </summary>
		/// <param name="dateTimeFormatInfo"></param>
		public Date(string standardName, DateTimeFormatInfo dateTimeFormatInfo)
		{
			if (dateTimeFormatInfo == null)
			{
				this.Format = DateTimeFormatInfo.CurrentInfo;
				this.StandardName = TimeZone.CurrentTimeZone.StandardName;
			}
			else
			{
				this.Format = dateTimeFormatInfo;
				this.StandardName = standardName;
			}
		}

		/// <summary>
		/// This constructor will initialize the Date Class to the site settings.
		/// </summary>
		/// <param name="Site"></param>
		public Date(SiteClass site)
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

		#endregion

		[XmlElement("Value")]
		public string TimeString
		{
			get
			{
				return Value.ToString(TimeFormat);
			}
			set
			{
				Value = DateTimeOffset.ParseExact(value, TimeFormat, null);
			}
		}

		public bool IsToday
		{
			get
			{
				return TimeConverter.ToDate(Value) == TimeConverter.Today(StandardName);
			}
		}

		public bool IsTodayOrBefore
		{
			get
			{
				return TimeConverter.ToDate(Value) <= TimeConverter.Today(StandardName);
			}
		}

		public bool IsTodayOrAfter
		{
			get
			{
				return TimeConverter.ToDate(Value) >= TimeConverter.Today(StandardName);
			}
		}

		public override bool Equals(object obj)
		{
			if (!typeof(Date).IsInstanceOfType(obj))
			{
				return false;
			}

			return Value.Equals(((Date)obj).Value);
		}

		public override int GetHashCode()
		{
			return Value.GetHashCode();
		}

		public override string ToString()
		{
			return Value.ToString("d", Format);
		}

		
		[XmlIgnore]
		public DateTimeOffset Value
		{
			get
			{

				return (DateTimeOffset.MinValue == dateOnlyValue) ? DateTimeOffset.MinValue : dateOnlyValue.Date;
			}
			set
			{
				dateOnlyValue = (DateTimeOffset.MinValue == value) ? DateTimeOffset.MinValue : value.Date;
			}
		}


		[XmlIgnore]
		public DateTimeFormatInfo Format
		{
			get
			{
				DateTimeFormatInfo format = new DateTimeFormatInfo();
				format.ShortDatePattern = this.shortDatePattern;
				format.DateSeparator = this.dateSeparator;
				return format;
			}
			set
			{
				this.shortDatePattern = value.ShortDatePattern;
				this.dateSeparator = value.DateSeparator;
			}
		}
	}
}
