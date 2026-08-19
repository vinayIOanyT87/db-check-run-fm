namespace DataObjects.DataObjects
{
	using System;

	public class EpochDate
	{
		/// <summary>
		/// This method will take a date time in a string format and convert 
		/// epoch date time.
		/// </summary>
		/// <param name="dateStr">String date in yyyy/mm/dd hh:mm:ss format.</param>
		/// <returns>Returns the epoch date time or null if failed.</returns>
		public uint? ConvertToEpochDate(string dateStr)
		{
			DateTime dateTime;
			uint? epochDateTime = null;

			if (DateTime.TryParse(dateStr, out dateTime))
			{
				var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
				epochDateTime = Convert.ToUInt32((dateTime - epoch).TotalSeconds);
			}

			return epochDateTime;
		}

		/// <summary>
		/// This method will take a date time in a string format and convert 
		/// epoch date time.
		/// </summary>
		/// <param name="dateTime">Date time to convert to Epoch.</param>
		/// <returns>Returns the epoch date time or null if failed.</returns>
		public uint? ConvertToEpochDate(DateTime dateTime)
		{
			var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
			uint? epochDateTime = Convert.ToUInt32((dateTime - epoch).TotalSeconds);

			return epochDateTime;
		}


		/// <summary>
		/// This method will convert an epoch date time to a regular
		/// date time in string format.
		/// </summary>
		/// <param name="epoch">Epoch date time</param>
		/// <returns>Return a date time in yyyy/mm/dd hh:mm:ss format.</returns>
		public string ConvertFromEpochDate(uint epoch)
		{
			var epochDateTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
			DateTime convertedDateTime = epochDateTime.AddSeconds(epoch);

			int year	= convertedDateTime.Year;
			int month	= convertedDateTime.Month;
			int day		= convertedDateTime.Day;
			int hour	= convertedDateTime.Hour;
			int minute	= convertedDateTime.Minute;
			int second	= convertedDateTime.Second;


			string convertedDateTimeStr = year + "/";

			if (month < 10)
			{
				convertedDateTimeStr = convertedDateTimeStr + "0" + month + "/";
			}
			else
			{
				convertedDateTimeStr = convertedDateTimeStr + month + "/";
			}

			if (day < 10)
			{
				convertedDateTimeStr = convertedDateTimeStr + "0" + day + " ";
			}
			else
			{
				convertedDateTimeStr = convertedDateTimeStr + day + " ";
			}

			if (hour < 10)
			{
				convertedDateTimeStr = convertedDateTimeStr + "0" + hour + ":";
			}
			else
			{
				convertedDateTimeStr = convertedDateTimeStr + hour + ":";
			}

			if (minute < 10)
			{
				convertedDateTimeStr = convertedDateTimeStr + "0" + minute + ":";
			}
			else
			{
				convertedDateTimeStr = convertedDateTimeStr + minute + ":";
			}

			if (second < 10)
			{
				convertedDateTimeStr = convertedDateTimeStr + "0" + second;
			}
			else
			{
				convertedDateTimeStr = convertedDateTimeStr + second;
			}

			return convertedDateTimeStr;
		}
	}
}
