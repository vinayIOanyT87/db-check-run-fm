namespace LedgerCore
{
	using System;

	public class LedgerTime
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
	}
}
