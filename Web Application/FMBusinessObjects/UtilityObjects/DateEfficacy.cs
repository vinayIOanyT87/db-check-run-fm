namespace FMBusinessObjects.DataObjects
{
    using System;

    using FMBusinessObjects.UtilityObjects;

    public class DateEfficacy
	{
		#region Public Attributes
		public enum CompareResults { LESS_THAN, GREATER_THAN, EQUAL_TO, NONE };
		#endregion

		#region Private Attributes
		private enum StartEndDateType { BEGIN, END };
		private const int EMPTY_STRING = 0;
		#endregion

		#region Constructors
		/// <summary>
		/// This is the default constructor for the date class.
		/// </summary>
		public DateEfficacy()
		{
		}
		#endregion

		#region Public Methods
		/// <summary>
		/// This method compares the first date with the second date. It will return an enum of less than,
		/// greater than, equal to, or none depending on the results. None means that the objects were invalid.
		/// </summary>
		/// <param name="date1"></param>
		/// <param name="date2"></param>
		/// <returns></returns>
		public static CompareResults CompareDate1ToDate2(DateTimeOffset date1, DateTimeOffset date2)
		{
			CompareResults compareResult = CompareResults.NONE;

			if ((((object)date1) != null) && (((object)date2) != null))
			{
				if ((date1.Year > 1) && (date2.Year > 1))
				{
					int results = date1.CompareTo(date2);

					if (results < 0)
						compareResult = CompareResults.LESS_THAN;

					if (results == 0)
						compareResult = CompareResults.EQUAL_TO;

					if (results > 0)
						compareResult = CompareResults.GREATER_THAN;
				}
			}

			return compareResult;
		}

		/// <summary>
		/// This method compares the first date with the second date. It will return an enum of less than,
		/// greater than, equal to, or none depending on the results. None means that the objects were invalid.
		/// The input string parameters must be of the following format: "dd-mm-yyyy".
		/// </summary>
		/// <param name="date1Str"></param>
		/// <param name="date2Str"></param>
		/// <returns></returns>
		public static CompareResults CompareDate1ToDate2(string date1Str, string date2Str)
		{
			CompareResults compareResult = CompareResults.NONE;

			if ((date1Str != null) && (date1Str.Length > EMPTY_STRING) &&
			(date2Str != null) && (date2Str.Length > EMPTY_STRING))
			{
				DateTimeOffset date1 = convertMonthDayYearToDateTime(date1Str);
				DateTimeOffset date2 = convertMonthDayYearToDateTime(date2Str);

				compareResult = CompareDate1ToDate2(date1, date2);
			}

			return compareResult;
		}

		/// <summary>
		/// This method will convert the "yyyy-mm-dd hh:mm:ss" or "yyyy-mm-dd" or
		/// "yyyy-m-dd" or "yyyy-mm-d" or "yyyy-m-d" date formats into a system date time.
		/// </summary>
		/// <param name="dateStr"></param>
		/// <returns></returns>
		public static DateTimeOffset convertYearMonthDayToDateTime(string dateStr)
		{
			string justTheDate = dateStr;
			int index = dateStr.IndexOf(" ");

			if (index > 0)
				justTheDate = dateStr.Substring(0, index);

			int firstDashIndex = justTheDate.IndexOf("-");
			int secondDashIndex = justTheDate.IndexOf("-", (firstDashIndex + 1));
			int monthLen = secondDashIndex - firstDashIndex - 1;
			int dayLen = justTheDate.Length - secondDashIndex - 1;

			DateTimeOffset dateTime = new DateTimeOffset(int.Parse(justTheDate.Substring(0, firstDashIndex)),
																		int.Parse(justTheDate.Substring((firstDashIndex + 1), monthLen)),
																		int.Parse(justTheDate.Substring((secondDashIndex + 1), dayLen)),
																		0, 0, 0, TimeSpan.Zero);

			return dateTime;
		}

		/// <summary>
		/// This method will convert the "mm-dd-yyyy" date format
		/// into a system date time.
		/// </summary>
		/// <param name="dateStr"></param>
		/// <returns></returns>
		public static DateTimeOffset convertMonthDayYearToDateTime(string dateStr)
		{
			int month = 1;
			int day = 1;
			int year = 1;
			int index1 = -1;
			int index2 = -1;
			string slash = "/";
			string dash = "-";
			string searchChar;
			DateTimeOffset dateTime;

			if ((dateStr != null) && (dateStr.Length > 0))
			{
				if (dateStr.IndexOf(slash) < 0)
					searchChar = dash;
				else
					searchChar = slash;

				index1 = dateStr.IndexOf(searchChar);
				index2 = dateStr.IndexOf(searchChar, index1 + 1);
			}

			if ((index1 >= 0) && (index2 > index1))
			{
				month = int.Parse(dateStr.Substring(0, index1));
				day = int.Parse(dateStr.Substring((index1 + 1), (index2 - index1 - 1)));
				year = int.Parse(dateStr.Substring((index2 + 1), (dateStr.Length - index2 - 1)));
				if (year < 50)
				{
					//map 0, 1, ..., 48, 49 to 2000, 2001, ..., 2048, 2049
					year += 2000;
				}
				else if (year < 100)
				{
					//map 50, 51, ..., 98, 99 to 1950, 1951, ..., 1998, 1999
					year += 1900;
				}
			}

			try
			{
				dateTime = new DateTimeOffset(year, month, day, 0, 0, 0, TimeSpan.Zero);
			}
			catch (Exception)
			{
				dateTime = TimeConverter.MinFMDate;
			}
			return dateTime;
		}

		/// <summary>
		/// This method will convert the date time to the following format:
		/// yyyy-mm-dd hh:mm:ss.
		/// </summary>
		public static string convertToYearMonthDayTime(DateTimeOffset dateTime)
		{
			int month = dateTime.Month;
			int day = dateTime.Day;
			int year = dateTime.Year;
			int hour = dateTime.Hour;
			int minute = dateTime.Minute;
			int second = dateTime.Second;

			string yearMonthDayTimeFormat = Convert.ToString(year) + "-" + zeroFill(month) +
			"-" + zeroFill(day) + " " + zeroFill(hour) + ":" +
			zeroFill(minute) + ":" + zeroFill(second);

			return yearMonthDayTimeFormat;
		}

		/// <summary>
		/// This method will convert the date time to the following format:
		/// yyyy-m-d h:m:s.
		/// </summary>
		public static string convertToYearMonthDayTimeNoneFill(DateTimeOffset dateTime)
		{
			int month = dateTime.Month;
			int day = dateTime.Day;
			int year = dateTime.Year;
			int hour = dateTime.Hour;
			int minute = dateTime.Minute;
			int second = dateTime.Second;

			string yearMonthDayTimeFormatNoneFill = Convert.ToString(year) + "-" + Convert.ToString(month) +
			"-" + Convert.ToString(day) + " " + Convert.ToString(hour) + ":" +
			Convert.ToString(minute) + ":" + Convert.ToString(second);

			return yearMonthDayTimeFormatNoneFill;
		}

		/// <summary>
		/// This method will convert the date to the following format:
		/// mm/dd/yyyy.
		/// </summary>
		public static string convertToMonthDayYear(DateTimeOffset dateTime)
		{
			int month = dateTime.Month;
			int day = dateTime.Day;
			int year = dateTime.Year;

			string monthDayYearFormat = zeroFill(month) + "/" + zeroFill(day) + "/" +
			Convert.ToString(year);

			return monthDayYearFormat;
		}

        /// <summary>
        /// This method will convert the date to the following database date format:
        /// yyyy-mm-dd.
        /// </summary>
        public static string convertToDatabaseDate(DateTime date)
        {
            int month = date.Month;
            int day = date.Day;
            int year = date.Year;

            string monthDayYearFormat = Convert.ToString(year) + "-" + zeroFill(month) + "-" + zeroFill(day);

            return monthDayYearFormat;
        }

		/// <summary>
		/// This method will convert yyyy-mm-dd hh:mm:ss formatted date string into
		/// mm/dd/yyyy formatted string.
		/// </summary>
		/// <param name="dateStr"></param>
		/// <returns></returns>
		public static string convertYearMonthDayToMonthDayYear(string dateStr)
		{
			string month = dateStr.Substring(5, 2);
			string day = dateStr.Substring(8, 2);
			string year = dateStr.Substring(0, 4);
			string convertedDate = month + "/" + day + "/" + year;

			return convertedDate;
		}

		/// <summary>
		/// This method will convert the date to the following format:
		/// m/d/yyyy.
		/// </summary>
		public static string convertToMonthDayYearNoneFill(DateTimeOffset dateTime)
		{
			int month = dateTime.Month;
			int day = dateTime.Day;
			int year = dateTime.Year;

			string monthDayYearFormatNoneFill = Convert.ToString(month) + "/" + Convert.ToString(day) + "/" +
			Convert.ToString(year);

			return monthDayYearFormatNoneFill;
		}

		/// <summary>
		/// This method will return the first day of the month in the following
		/// format: "yyyy-mm-dd hh:mm:ss" from the month/year (January 2004) format.
		/// </summary>
		/// <returns></returns>
		public static string getFirstDayOfMonth(string monthYear)
		{
			return convertMonthYear(monthYear, StartEndDateType.BEGIN);
		}

		/// <summary>
		/// This method will return the last day of the month in the following
		/// format: "yyyy-mm-dd hh:mm:ss" from the month/year (January 2004) format.
		/// </summary>
		/// <returns></returns>
		public static string getLastDayOfMonth(string monthYear)
		{
			return convertMonthYear(monthYear, StartEndDateType.END);
		}

		/// <summary>
		/// This method will return a month/year string in the following format:
		/// January 2006.
		/// </summary>
		/// <param name="dateTime"></param>
		/// <returns></returns>
		public static string ConvertToMonthAndYear(DateTimeOffset dateTime)
		{
            var dtf = System.Globalization.CultureInfo.InvariantCulture.DateTimeFormat;
            string monthYear = dtf.GetMonthName(dateTime.Month);
			monthYear = monthYear + " " + dateTime.Year.ToString();
			return monthYear;
		}

        /// <summary>
        /// This method will return a month/year string in the following format:
        /// January 2006.
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static string ConvertToMonthAndYear(DateTime dateTime)
        {
            var dtf = System.Globalization.CultureInfo.InvariantCulture.DateTimeFormat;
            string monthYear = dtf.GetMonthName(dateTime.Month);
            monthYear = monthYear + " " + dateTime.Year.ToString();
            return monthYear;
        }

        /// <summary>
        /// This method will return a month/year string in the following format:
        /// Avril 2006.
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static string ConvertToMonthAndYearCurrentCulture(DateTimeOffset dateTime)
        {
            var dtf = System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat;
            string monthYear = dtf.GetMonthName(dateTime.Month);
            monthYear = monthYear + dateTime.Year.ToString();
            return monthYear;
        }

        /// <summary>
        /// This method will return either the first or last of the month in the
        /// following format: yyyy-mm-dd hh:mm:ss
        /// from the month/year format (June 2004).
        /// </summary>
        /// <param name="monthYear"></param>
        /// <returns></returns>
        public static string convertMonthYearToCurrentCulture(string monthYear)
        {
            string dateStr = "";

            if ((monthYear != null) && (monthYear.Length != EMPTY_STRING))
            {
                string monthStr = monthYear;
                string yearStr = monthYear;

                // Parse the month portion of the string from the year portion. Build a
                // date string that has the following format: "2003-03-01 00:00:00"
                int spaceIndex = monthStr.IndexOf(" ", 0);
                monthStr = monthStr.Substring(0, spaceIndex);
                monthStr = monthStr.ToUpper();
                yearStr = yearStr.Substring(spaceIndex + 1, 4);
                dateStr = yearStr + "-" + monthStr + "-" + "01";
                DateTimeOffset workDateTimeOffset = DateTimeOffset.Parse(dateStr, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.AssumeUniversal);
                var dtf = System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat;
                string monthName = dtf.GetMonthName(workDateTimeOffset.Month).ToUpper();
                dateStr = monthName + " " + yearStr;
            }

            return dateStr;
        }

		#endregion

		#region Private Methods

		/// <summary>
		/// This method will return either the first or last of the month in the
		/// following format: yyyy-mm-dd hh:mm:ss
		/// from the month/year format (June 2004).
		/// </summary>
		/// <param name="monthYear"></param>
		/// <returns></returns>
		private static string convertMonthYear(string monthYear, StartEndDateType dateType)
		{
			string dateStr = "";

			if ((monthYear != null) && (monthYear.Length != EMPTY_STRING))
			{
				string monthStr = monthYear;
				string yearStr = monthYear;

				// Parse the month portion of the string from the year portion. Build a
				// date string that has the following format: "2003-03-01 00:00:00"
				int spaceIndex = monthStr.IndexOf(" ", 0);
				monthStr = monthStr.Substring(0, spaceIndex);
				monthStr = monthStr.ToUpper();
				yearStr = yearStr.Substring(spaceIndex + 1, 4);
                DateTimeOffset workDateTimeOffset = new DateTimeOffset();
                string tempDateStr = yearStr + "-" + monthStr + "-" + "01";
                int month = -1;
                if (!DateTimeOffset.TryParse(tempDateStr, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.AssumeUniversal, out workDateTimeOffset))
                {
                    DateTimeOffset.Parse(tempDateStr, System.Globalization.CultureInfo.CurrentCulture, System.Globalization.DateTimeStyles.AssumeUniversal);
                }
                month = workDateTimeOffset.Month;

				switch (dateType)
				{
					case StartEndDateType.BEGIN:
						{
                            dateStr = yearStr + "-" + zeroFill(month) + "-" + "01";
							break;
						}

					case StartEndDateType.END:
						{
                            int year = workDateTimeOffset.Year;
							int monthEndDay = DateTime.DaysInMonth(year, month);
                            dateStr = yearStr + "-" + zeroFill(month) + "-" + zeroFill(monthEndDay);
							break;
						}
				}
			}

			return dateStr;
		}

		/// <summary>
		/// This method will convert a number to a string and if less
		/// than 10, it will prefix it with a zero.
		/// </summary>
		/// <param name="number"></param>
		/// <returns></returns>
		private static string zeroFill(int number)
		{
			string zeroFillNumber = Convert.ToString(number);

			if (number < 10)
				zeroFillNumber = "0" + zeroFillNumber;

			return zeroFillNumber;
		}
		#endregion
	}
}
