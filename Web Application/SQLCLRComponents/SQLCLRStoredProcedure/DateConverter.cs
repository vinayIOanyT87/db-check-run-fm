using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

class DateConverter
{
   public enum TimeTypes {START, END};
   #region Constructors
   /// <summary>
   /// This is the default constructor for the Date Converter class.
   /// </summary>
   public DateConverter()
   {
   }
   #endregion

   #region Public methods
   /// <summary>
   /// This method will convert the date to the following format:
   /// mm/dd/yyyy.
   /// </summary>
   public string ConvertToMonthDayYear(System.DateTime dateTime)
   {
      int month = dateTime.Month;
      int day   = dateTime.Day;
      int year  = dateTime.Year;

      string monthDayYearFormat = this.ZeroFill(month) + "/" + this.ZeroFill(day) + "/" + Convert.ToString(year);

      return monthDayYearFormat;
   }

   /// <summary>
   /// This method will convert the date time to the following format:
   /// yyyy-mm-dd hh:mm:ss.
   /// </summary>
   public string ConvertToYearMonthDayTime(System.DateTime dateTime)
   {
      int month  = dateTime.Month;
      int day    = dateTime.Day;
      int year   = dateTime.Year;
      int hour   = dateTime.Hour;
      int minute = dateTime.Minute;
      int second = dateTime.Second;

      string yearMonthDayTimeFormat = Convert.ToString(year) + "-" + this.ZeroFill(month) +
                                      "-" + this.ZeroFill(day) + " " + this.ZeroFill(hour) + ":" +
                                      this.ZeroFill(minute) + ":" + this.ZeroFill(second);

      return yearMonthDayTimeFormat;
   }

   /// <summary>
   /// This method will return the Datetime with the time portion set to either
   /// 00:00:00 or 23:59:59 depending on the Time Types (START or END).
   /// </summary>
   /// <param name="inDate"></param>
   /// <param name="timeTypes"></param>
   /// <returns></returns>
   public DateTime GetDateWithCorrectTimePortion(DateTime inDate, TimeTypes timeTypes)
   {
      DateTime fixedTime;

      switch (timeTypes)
      {
         case TimeTypes.START:
            fixedTime = new DateTime(inDate.Year, inDate.Month, inDate.Day, 0, 0, 0);
            break;
         default:
            fixedTime = new DateTime(inDate.Year, inDate.Month, inDate.Day, 23, 59, 59);
            break;
      }

      return fixedTime;
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
