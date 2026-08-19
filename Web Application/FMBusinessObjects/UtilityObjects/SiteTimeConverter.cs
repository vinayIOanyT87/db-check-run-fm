using System;

using FMBusinessObjects.DataObjects;

namespace FMBusinessObjects.UtilityObjects
{
	/// <summary>
	/// Static class used to facilitate time zone conversions to and from a specified Site.
	/// </summary>
	public static class		TimeConverter
	{
		/// <summary>
		/// Minimum Date value, 1/01/1900, used in the FuelsManager application.  Contains a zero time component.
		/// </summary>
		public static readonly DateTimeOffset MinFMDate = new DateTimeOffset(1900, 1, 1, 0, 0, 0, TimeSpan.Zero);

		/// <summary>
		/// Maximum Date value, 1/01/2200, used in the FuelsManager application.  Contains a zero time component.
		/// </summary>
		public static readonly DateTimeOffset MaxFMDate = new DateTimeOffset(2200, 1, 1, 0, 0, 0, TimeSpan.Zero);

		/// <summary>
		/// Minimum Time of Day value, 00:00:00, used in the FuelsManager application.  Contains a MinFMDate date component.
		/// </summary>
		public static readonly DateTimeOffset MinFMTime = TimeConverter.MinFMDate;

		/// <summary>
		/// Maximum Time of Day value, 23:59:59, used in the FuelsManager application.  Contains a MinFMDate date component.
		/// </summary>
		public static readonly DateTimeOffset MaxFMTime = TimeConverter.MinFMDate + new TimeSpan(0, 23, 59, 59, 999);

		/// <summary>
		/// Default Start Time value, 00:00:00, used in the FuelsManager application.  Contains a MinFMDate date component.
		/// </summary>
		public static readonly DateTimeOffset DefaultFMStartTime = TimeConverter.MinFMDate + new TimeSpan(0, 0, 0);

		/// <summary>
		/// Default End Time value, 23:59:59, used in the FuelsManager application.  Contains a MinFMDate date component.
		/// </summary>
		public static readonly DateTimeOffset DefaultFMEndTime = TimeConverter.MinFMDate + new TimeSpan(23, 59, 59);

		/// <summary>
		/// To the site time.
		/// </summary>
		/// <param name="standardName">Name of the standard.</param>
		/// <param name="inTime">The in time.</param>
		/// <returns></returns>
		public static DateTimeOffset ToSiteTime(string standardName, DateTimeOffset inTime)
		{
			TimeZoneInfo timeZoneInfo;
			try
			{
				timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(standardName);
			}
			catch (Exception)
			{
				timeZoneInfo = TimeZoneInfo.Local;
			}

			return TimeZoneInfo.ConvertTime(inTime, timeZoneInfo);
		}

		/// <summary>
		/// Converts the specified DateTimeOffset object to the time zone indicated by the specified Site.  Uses the
		/// daylight savings time aware method TimeZoneInfo.ConvertTime() to perform the conversion and handle any
		/// necessary daylight savings time adjustments.
		/// </summary>
		/// <param name="site">The Site containing the time zone used to perform the conversion.</param>
		/// <param name="inTime">The DateTimeOffset object to convert to the indicated time zone.</param>
		/// <returns>The specified DateTimeOffset object converted to the indicated time zone.</returns>
		public static DateTimeOffset ToSiteTime(SiteClass site, DateTimeOffset inTime)
		{
			TimeZoneInfo timeZoneInfo;
			try
			{
				timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(site.TimeZone);
			}
			catch (Exception)
			{
				timeZoneInfo = TimeZoneInfo.Local;
			}

			return TimeZoneInfo.ConvertTime(inTime, timeZoneInfo);
		}

		/// <summary>
		/// Converts the inTime to the time zone specified by the timeZoneInfo parameter.
		/// </summary>
		/// <param name="timeZoneInfo">The time zone to use for conversion</param>
		/// <param name="inTime">The time object to convert.</param>
		/// <returns>A DateTimeOffset converted to the specified time zone.</returns>
		public static DateTimeOffset ToSiteTime(TimeZoneInfo timeZoneInfo, DateTimeOffset inTime)
		{
			return TimeZoneInfo.ConvertTime( inTime, timeZoneInfo );
		}

		/// <summary>
		/// If the specified DateTimeOffset object has a zero time component then the time zone offset is cleared.
		/// The assumption is that a value with a zero time component is intended to be a date.  It is also assumed
		/// that date values should be the same across all time zones.  Otherwise the specified DateTimeOffset object
		/// is converted to the time zone indicated by the specified site.  Uses the daylight savings time aware method
		/// TimeZoneInfo.ConvertTime() to perform the conversion and handle any necessary daylight savings time adjustments.
		/// </summary>
		/// <param name="site">The Site containing the time zone used to perform the conversion.</param>
		/// <param name="inTime">The DateTimeOffset object to convert to the indicated time zone.</param>
		/// <returns>
		/// The specified DateTimeOffset object converted to the indicated time zone unless the time component is zero.
		/// Values with a zero time component are assumed to be dates and returned with the time zone offset cleared.
		/// </returns>
		public static DateTimeOffset ToSiteTimeOrDate(SiteClass site, DateTimeOffset inTime)
		{
			TimeZoneInfo timeZoneInfo;
			try
			{
				timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(site.TimeZone);
			}
			catch (Exception)
			{
				timeZoneInfo = TimeZoneInfo.Local;
			}

			if (inTime.TimeOfDay == TimeSpan.Zero)
			{
				return ToDate(inTime);
			}

			return TimeZoneInfo.ConvertTime(inTime, timeZoneInfo);
		}

		public static DateTimeOffset Now(string standardName)
		{
			return ToSiteTime(standardName, DateTimeOffset.Now);
		}

		/// <summary>
		/// Converts the current time to the time zone indicated by the specified Site.
		/// </summary>
		/// <param name="site">The Site containing the time zone used to perform the conversion.</param>
		/// <returns>The current time converted to the time zone indicated by the specified Site.</returns>
		public static DateTimeOffset Now(SiteClass site)
		{
			return ToSiteTime(site, DateTimeOffset.Now);
		}

		public static DateTimeOffset Today(string standardName)
		{
			return ToDate(ToSiteTime(standardName, DateTimeOffset.Now));
		}

		/// <summary>
		/// Converts the current time to the time zone indicated by the specified Site.  Then converts that
		/// value to a date by zeroing out the time component.
		/// </summary>
		/// <param name="site">The Site containing the time zone used to perform the conversion.</param>
		/// <returns>The current time converted to the time zone indicated by the specified Site and then
		/// converted to a date with a zero time component.
		/// </returns>
		public static DateTimeOffset Today(SiteClass site)
		{
			return ToDate(ToSiteTime(site, DateTimeOffset.Now));
		}

		/// <summary>
		/// Converts the current time to a date by zeroing out the time component.
		/// </summary>
		/// <returns>The current time converted to a date with a zero time component.</returns>
		public static DateTimeOffset Today()
		{
			return ToDate(DateTimeOffset.Now);
		}

		/// <summary>
		/// Converts the specified DateTimeOffset object to a date by zeroing out the time component.
		/// </summary>
		/// <param name="inTime">The DateTimeOffset object to convert to a date.</param>
		/// <returns>The specified DateTimeOffset object converted to a date with a zero time component.</returns>
		public static DateTimeOffset ToDate(DateTimeOffset inTime)
		{
			return new DateTimeOffset(inTime.Year, inTime.Month, inTime.Day, 0, 0, 0, 0, inTime.Offset);
		}

		/// <summary>
		/// Converts the specified DateTimeOffset object to the UTC time zone.  Uses the daylight savings
		/// time aware method TimeZoneInfo.ConvertTime() to perform the conversion and handle any necessary
		/// daylight savings time adjustments.
		/// </summary>
		/// <param name="inTime">The DateTimeOffset object to convert to the UTC time zone.</param>
		/// <returns>The specified DateTimeOffset object converted to the UTC time zone.</returns>
		public static DateTimeOffset ToUTCTime(DateTimeOffset inTime)
		{
			return TimeZoneInfo.ConvertTime(inTime, TimeZoneInfo.Utc);
		}

		/// <summary>
		/// Converts the specified DateTimeOffset object to the local time zone.  Uses the daylight savings
		/// time aware method TimeZoneInfo.ConvertTime() to perform the conversion and handle any necessary
		/// daylight savings time adjustments.
		/// </summary>
		/// <param name="inTime">The DateTimeOffset object to convert to the local time zone.</param>
		/// <returns>The specified DateTimeOffset object converted to the local time zone.</returns>
		public static DateTimeOffset ToLocalTime(DateTimeOffset inTime)
		{
			return TimeZoneInfo.ConvertTime(inTime, TimeZoneInfo.Local);
		}

		/// <summary>
		/// Converts the specified DateTimeOffset object to the start of the day by zeroing out the time component.
		/// </summary>
		/// <param name="inTime">The DateTimeOffset object to convert to the start of the day.</param>
		/// <returns>The specified DateTimeOffset object converted to a date with a zero time component.</returns>
		public static DateTimeOffset ToStartOfDay(DateTimeOffset inTime)
		{
			return ToDate(inTime);
		}

		/// <summary>
		/// Converts the specified DateTimeOffset object to the end of the day by setting the time component to
		/// the maximum number of hours, minutes, seconds, and milliseconds in a day.
		/// </summary>
		/// <param name="inTime">The DateTimeOffset object to convert to the end of the day.</param>
		/// <returns>The specified DateTimeOffset object converted to a date with the time component set to
		/// the maximum number of hours, minutes, seconds, and milliseconds in a day.
		/// </returns>
		public static DateTimeOffset ToEndOfDay(DateTimeOffset inTime)
		{
			return new DateTimeOffset(inTime.Year, inTime.Month, inTime.Day, 23, 59, 59, 999, TimeSpan.Zero);
		}

		/// <summary>
		/// Converts the specified DateTimeOffset object to a FuelsManager time by setting the date component
		/// to TimeConverter.MinFMDate.
		/// </summary>
		/// <param name="inTime">The DateTimeOffset object to convert to a FuelsManager time.</param>
		/// <returns>The specified DateTimeOffset object converted to a FuelsManager time with the date
		/// component set to TimeConverter.MinFMDate.
		/// </returns>
		public static DateTimeOffset ToFMTime(DateTimeOffset inTime)
		{
			return TimeConverter.MinFMDate + inTime.TimeOfDay;
		}
	}

	/// <summary>
	/// The SiteTimeConverter class is used to convert times between the UTC and specified Site time zones.
	/// </summary>
	public class SiteTimeConverter
	{
		private readonly SiteClass site;

		/// <summary>
		/// Constructor used to initialize the Site member variable
		/// </summary>
		/// <param name="site">SiteClass object containing site time zone info</param>
		public SiteTimeConverter(SiteClass site)
		{
			this.site = site;
		}

		/// <summary>
		/// Converts the specified DateTimeOffset object to the time zone indicated by the Site member variable.
		/// Indirectly uses the daylight savings time aware method TimeZoneInfo.ConvertTime() to perform the
		/// conversion and handle any necessary daylight savings time adjustments.
		/// </summary>
		/// <param name="inTime">The DateTimeOffset object to convert to the Site time zone.</param>
		/// <returns>The specified DateTimeOffset object converted to the Site time zone.</returns>
		public DateTimeOffset ConvertToSiteTime(DateTimeOffset inTime)
		{
			return TimeConverter.ToSiteTime(this.site, inTime);
		}

		/// <summary>
		/// Converts the current time to the time zone indicated by the Site member variable.
		/// </summary>
		/// <returns>The current time converted to the Site time zone.</returns>
		public DateTimeOffset Now()
		{
			return TimeConverter.Now(this.site);
		}

		/// <summary>
		/// Converts the current time to the time zone indicated by the Site member variable.  Then
		/// converts that value to a date by zeroing out the time component.
		/// </summary>
		/// <returns>The current time converted to the time zone indicated by the Site member variable
		/// and then converted to a date with a zero time component.
		/// </returns>
		public DateTimeOffset Today()
		{
			return TimeConverter.Today(this.site);
		}

		/// <summary>
		/// Converts the specified DateTimeOffset object to the UTC time zone.
		/// </summary>
		/// <param name="inTime">The DateTimeOffset object to convert to the UTC time zone.</param>
		/// <returns>The specified DateTimeOffset object converted to the UTC time zone.</returns>
		public DateTimeOffset ConvertToUtcTime(DateTimeOffset inTime)
		{
			return TimeConverter.ToUTCTime(inTime);
		}

		public DateTimeOffset ConvertFromSiteTime( DateTimeOffset inTime )
		{
			DateAndTime converter = new DateAndTime( this.site );
			converter.Value = inTime;
			return converter.UTCValue;
		}
	}
}
