namespace LedgerCore
{
	using System;

	class LRDateConverter
	{
		public enum TimeTypes { Start, End };

		#region Constructors
		/// <summary>
		/// This is the default constructor for the Date Converter class.
		/// </summary>
		public LRDateConverter()
		{
		}
		#endregion

		#region Public methods
		/// <summary>
		/// This method will convert the date to the following format:
		/// mm/dd/yyyy.
		/// </summary>
		public string ConvertToMonthDayYear(DateTime inDate)
		{
			int month = inDate.Month;
			int day = inDate.Day;
			int year = inDate.Year;

			string monthDayYearFormat = this.ZeroFill(month) + "/" + this.ZeroFill(day) + "/" + Convert.ToString(year);

			return monthDayYearFormat;
		}

		/// <summary>
		/// This method will convert the date time to the following format:
		/// yyyy-mm-dd hh:mm:ss.
		/// </summary>
		public string ConvertToYearMonthDayTime(DateTimeOffset inDate)
		{
			int month = inDate.Month;
			int day = inDate.Day;
			int year = inDate.Year;
			int hour = inDate.Hour;
			int minute = inDate.Minute;
			int second = inDate.Second;

			string yearMonthDayTimeFormat = Convert.ToString(year) + "-" + this.ZeroFill(month) +
													  "-" + this.ZeroFill(day) + " " + this.ZeroFill(hour) + ":" +
													  this.ZeroFill(minute) + ":" + this.ZeroFill(second);

			return yearMonthDayTimeFormat;
		}

		/// <summary>
		/// This method will return the DatetimeOffset with the time portion set to either
		/// 00:00:00 or 23:59:59 depending on the Time Types (START or END).
		/// </summary>
		/// <param name="inDate"></param>
		/// <param name="timeTypes"></param>
		/// <returns></returns>
		public DateTimeOffset GetDateWithCorrectTimePortion(DateTimeOffset inDate, TimeTypes timeTypes)
		{
		    var fixedTime = inDate;

			switch (timeTypes)
			{
				case TimeTypes.Start:
					fixedTime = LedgerTime.ToStartOfDay(fixedTime);
					break;
				default:
					fixedTime = LedgerTime.ToEndOfDay(fixedTime);
					break;
			}

			return fixedTime;
		}

        /// <summary>
        /// This method will return the DatetimeOffset with the time portion set to either
        /// 00:00:00 or 23:59:59 depending on the Time Types (START or END).
        /// </summary>
        /// <param name="inDate"></param>
        /// <param name="timeTypes"></param>
        /// <returns></returns>
        public DateTimeOffset GetDateWithCorrectTimePortion(DateTime inDate, TimeTypes timeTypes)
        {
            DateTimeOffset fixedTime = new DateTimeOffset(inDate);
            return GetDateWithCorrectTimePortion(fixedTime, timeTypes);
        }
		#endregion

		#region Private methods
		/// <summary>
		/// This method will convert a number to a string and if less
		/// than 10, it will prefix it with a zero.
		/// </summary>
		/// <param name="number"></param>
		/// <returns></returns>
		private string ZeroFill(int number)
		{
			string zeroFillNumber = Convert.ToString(number);

			if (number < 10)
			{
				zeroFillNumber = "0" + zeroFillNumber;
			}

			return zeroFillNumber;
		}
		#endregion
	}
}