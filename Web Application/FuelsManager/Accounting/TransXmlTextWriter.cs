/******************************************************************************
	FILE NAME:		TransXmlTextWriter.cs

	PURPOSE:		Ax extension of class XmlTextWriter so that it can have some additional member functions

	COMMENTS:		Copyright (C) Varec, Inc. Norcross, GA, USA, 2005
					This file shall not be copied or reproduced in any form without
					the express written consent of Varec, Inc.
									
	AUTHOR(S):		Paul Carpenter

	VERSION:		$Header: /FuelsManager Aviation/Release v7.1 SP8/Core/FMAviationEnterpriseInterface/FMAEInterfaceClient/TransXmlTextWriter.cs 1     8/18/17 2:16p Ponnwitzb $
    LAST CHANGES:   3.24.16 PaulTreinis Added defaulValue param to  WriteElementDateTime().  bug #58031
*******************************************************************************/

using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Text;
using System.Data;
using System.Xml;

namespace FuelsManager.Accounting
{
    class TransXmlTextWriter : XmlTextWriter
    {
        static protected DateTime BeginningOfTime = new DateTime(2000, 1, 1);
        protected DateTime CurrentTime;
        protected TimeZone CurrentTimeZone;
        protected TimeSpan OffsetFromLocalTimeZoneToGMT;
        public bool CommentXmlClose = false;

        static Regex regExHMM = new Regex(@"^\d{3}$");
        static Regex regExHHMM = new Regex(@"^\d{4}$");
        static Regex regExHColonMM = new Regex(@"^\d:\d\d$");
        static Regex regExHHColonMM = new Regex(@"^\d\d:\d\d$");

        public TransXmlTextWriter(string filename, Encoding encoding)
            : base(filename, encoding)
        {
            this.CurrentTime = DateTime.Now;
            this.CurrentTimeZone = TimeZone.CurrentTimeZone;
            this.OffsetFromLocalTimeZoneToGMT = this.CurrentTimeZone.GetUtcOffset(this.CurrentTime);
#if DEBUG
            //CommentXmlClose = true;
#endif 
        }

        public void WriteEndElement(string expectedElementName)
        {
            if (this.CommentXmlClose)
            {
                this.WriteComment(expectedElementName);
            }
            base.WriteEndElement();
        }

        public void WriteIfNotNull(DataRow dbRow, string toColumn, string fromColumn)
        {
            object obj = dbRow[fromColumn];
            if (obj != null && obj != DBNull.Value)
            {
                this.WriteElementString(toColumn, obj.ToString());
            }
        }

        public void WriteNumberIfNotZero(DataRow dbRow, string toColumn, string fromColumn)
        {
            object obj = dbRow[fromColumn];
            if (obj == null || obj == DBNull.Value)
            {
                return;
            }
            if (obj is string)
            {
                int intValue;
                double doubleValue;
                if (   ( int.TryParse((string)obj, out intValue) && intValue != 0)
                    || (double.TryParse((string)obj, out doubleValue) && doubleValue != 0.0))
                {
                    this.WriteElementString(toColumn, (string)obj);
                }
            } 
            else if (   (obj is double && (double)obj != 0.0)
                     || (obj is int && (double)obj != 0))
            {
                this.WriteElementString(toColumn, obj.ToString());
            }
        }

        /// <summary>
        /// created a UserField element from a transaction column value within this there is an element named UserData# where # is the UserDataIndex
        /// </summary>
        /// <param name="dbRow"></param>
        /// <param name="fromColumn">column within the transaction where the input value is</param>
        /// <param name="userDataIndex">index of userdata 1..24</param>
        public void WriteUserDataElementWithValue(DataRow dbRow, string fromColumn, int userDataIndex)
        {
            if (!string.IsNullOrWhiteSpace(dbRow[fromColumn].ToString()))
            {
                string name1 = string.Format("UserData{0}", userDataIndex);
				this.WriteElementFromRowData("UserField", dbRow, fromColumn, name1, "Value");
            }
        }

        /// <summary>
        /// created a UserField element from a transaction column value within this there is an element named UserData# where # is the UserDataIndex
        /// </summary>
        /// <param name="dbRow">datarow</param>
        /// <param name="userDataIndex">index for UserData column, used for both source and dest</param>
        public void WriteUserDataElementWithValue(
             DataRow dbRow
            , int userDataIndex)
        {
            string fromColumn = string.Format("USERDATA_{0}", userDataIndex);
			this.WriteUserDataElementWithValue(dbRow, fromColumn, userDataIndex);
        }

        /// <summary>
        /// Create an XML element with sub-elements Name and Code
        /// </summary>
        /// <param name="elementName">the name of the XML element beign creatd</param>
        /// <param name="dbRow"></param>
        /// <param name="fromColumn">column name from the transaction table</param>
        public void WriteElementWithCode(string elementName, DataRow dbRow, string fromColumn)
        {
			this.WriteElementWithValue(elementName, dbRow, dbRow[fromColumn].ToString(), "Code", "");
        }

        /// <summary>
        /// Create an XML element with 2 sub-elements "Name" and a name passed in by paraemter fieldName2
        /// </summary>
        /// <param name="elementName"></param>
        /// <param name="dbRow"></param>
        /// <param name="name1">Value of sub-element "Name"</param>
        /// <param name="fieldName2">Name of the second sub-element</param>
        /// <param name="fieldValue2">Value of the second sub-element</param>
        public void WriteElementWithValue(string elementName, DataRow dbRow
            , string name1, string fieldName2, string fieldValue2)
        {
            this.WriteStartElement(elementName);
            this.WriteElementString("Name", name1);
            this.WriteElementString(fieldName2, fieldValue2);
            this.WriteEndElement(elementName);
        }        

        public void WriteElementFromRowData(string elementName, DataRow dbRow
            , string fromColumn, string name1, string fieldName2)
        {
			this.WriteElementWithValue(elementName, dbRow, name1, fieldName2, dbRow[fromColumn].ToString());
        }


        public void WriteElementWithValue(string elementName, DataRow dbRow, string name1, string value)
        {
            this.WriteStartElement(elementName);
            this.WriteElementString("Name", name1);
            this.WriteElementString("Value", value);
            this.WriteEndElement(elementName);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="elementName">Output XML element name</param>
        /// <param name="fromColumn">column name for a column with a type of DateTime, DateTimeOffset or String, assumes a limited number or formats for string</param>
        /// <param name="dbRow"></param>
        /// <param name="defColumn"></param> PGT 3.23.16 Default field to use in case fromColumn is empty. Must be the same field type as fromColumn field.
        public void WriteElementDateTime(string elementName, string fromColumn, DataRow dbRow, string defColumn="")
        {
            object dataValue = dbRow[fromColumn], defValue=null;
            if (dataValue != null && dataValue != DBNull.Value)
            {
                try
                {
                    if (dataValue is DateTime)
                    {
                        DateTime localTime = (DateTime)dataValue;
                        if (localTime < BeginningOfTime)// PGT 3.23.16
                        {
                            if (defColumn == "") return;                            
                            dataValue = dbRow[defColumn];
                            if (dataValue == null || dataValue == DBNull.Value || !(dataValue is DateTime)) return;  
                            if ((localTime = (DateTime)dataValue) <= BeginningOfTime) return;                            
                        }
						this.WriteElementDateTime(elementName, localTime.ToUniversalTime(), this.TimeZoneOffset(localTime));
                        return;
                    }
                    if (dataValue is DateTimeOffset)
                    {
                        DateTimeOffset dateTimeOffset = (DateTimeOffset)(dataValue);
                        DateTime tm = new DateTime(dateTimeOffset.Year, dateTimeOffset.Month, dateTimeOffset.Day, dateTimeOffset.Hour, dateTimeOffset.Minute, dateTimeOffset.Second);
                        if (tm <BeginningOfTime)
                        {    // PGT 3.23.16 added use of default value
                            if (tm == BeginningOfTime && defColumn != "" && (defValue = dbRow[defColumn]) != null
                                && defValue != DBNull.Value && defValue is DateTimeOffset)
                            {
                                DateTimeOffset defDateTimeOffset = (DateTimeOffset)(dataValue);
                                DateTime defTm = new DateTime(defDateTimeOffset.Year, defDateTimeOffset.Month, defDateTimeOffset.Day, defDateTimeOffset.Hour, defDateTimeOffset.Minute, defDateTimeOffset.Second);
                                if (defTm >= BeginningOfTime)
                                    tm = defTm;
                                else
                                    return;
                            }
                            else return;
                        }
						this.WriteElementDateTime(elementName, tm, dateTimeOffset.Offset);
                        return;
                    }
                    if (dataValue is string)
                    {
                        DateTime dateTimeUniversal;
                        TimeSpan tzOffset;
                        DateTime transDateTime = (DateTime)(dbRow["TRANSACTION_DATE"]);
                        if (StandardizeTime(dataValue as string, transDateTime, out dateTimeUniversal, out tzOffset))
                        {
							this.WriteElementDateTime(elementName, (DateTime)dateTimeUniversal, (TimeSpan)tzOffset);
                            return;
                        }
                    }
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.ToString());
                    // whatever was in that column was not a date, just leave it blank;
                }
            }
            //   No need to output an empty date time value
            //  WriteElementString(elementName, "");
        }


        /// <summary>
        /// Return the offset to GMT, considers time zone and dates of Day Light Savings
        /// </summary>
        /// <param name="dateTime">time/date in local timezone</param>
        /// <returns></returns>
        protected TimeSpan TimeZoneOffset(DateTime dateTime)
        {
            bool isCurrentlyDayLightSavings = this.CurrentTime.IsDaylightSavingTime();
            bool parameterIsDayLightSavings = dateTime.IsDaylightSavingTime();

            if (isCurrentlyDayLightSavings == parameterIsDayLightSavings)
            {
                return this.OffsetFromLocalTimeZoneToGMT;
            }

            TimeSpan retVal = new TimeSpan(this.OffsetFromLocalTimeZoneToGMT.Hours - (isCurrentlyDayLightSavings ? 1 : 0) + (parameterIsDayLightSavings ? 1 : 0), 0, 0);
            return retVal;
        }

        /// <summary>
        /// Formats and write XML element for DateTime elements
        /// </summary>
        /// <param name="elementName">XML element name</param>
        /// <param name="dateTime">value of element</param>
        /// <param name="timeZoneOffset">offset to GMT written as an attribute</param>
        protected void WriteElementDateTime(string elementName, DateTime dateTime, TimeSpan timeZoneOffset)
        {
            this.WriteStartElement(elementName);
            if (dateTime >= BeginningOfTime)
            {
                // for TimeSpan lowercase hh is 00-23
                //  timezoneOffset.ToString() does not follow spec: https://msdn.microsoft.com/en-us/library/1ecy8h51(v=vs.110).aspx
                string val = string.Format("{0:00}:{1:00}", timeZoneOffset.Hours, timeZoneOffset.Minutes);
                this.WriteAttributeString("TimeZoneOffset", val);
                string formattedTime = dateTime.ToString("yyyy-MM-dd HH:mm:ss") + "Z";
                this.WriteString(formattedTime);
                //Debug.WriteLine(string.Format("TimeZoneOffset=\"{0}\" {1}", val, formattedTime));
            }
            this.WriteEndElement(elementName);
        }

        /// <summary>
        /// Parse out a string with a time and/or date from selected formats
        /// </summary>
        /// <param name="timeString">INput time</param>
        /// <param name="defaultDate">When timeString does not contain a date, use the date portion of this as a default</param>
        /// <param name="timezoneOffset">Time zone offset betweeen return value and GMT</param>
        /// <returns>True if timeString was successfully parsed</returns>

        internal static bool StandardizeTime(string timeString, DateTime defaultDate, out  DateTime dateTimeInGMT, out TimeSpan timezoneOffset)
        {
            dateTimeInGMT = DateTime.MinValue;
            timezoneOffset = TimeSpan.MinValue;
            timeString = timeString.Trim();

            // Is timeString a date and time or only a time?  
            // To be treated as a date and time it has to have date on or after the beginning of time and a colon
            if (timeString.IndexOf(':') > 0 && DateTime.TryParse(timeString, out dateTimeInGMT) && dateTimeInGMT >= TransXmlTextWriter.BeginningOfTime)
            {
                // no conversion needed
            }
            else if (regExHHMM.IsMatch(timeString))
            {
                dateTimeInGMT = new DateTime(defaultDate.Year
                    , defaultDate.Month
                    , defaultDate.Day
                    , int.Parse(timeString.Substring(0, 2))
                    , int.Parse(timeString.Substring(2, 2))
                    , 0);
            }
            else if (regExHColonMM.IsMatch(timeString))
            {
                dateTimeInGMT = new DateTime(defaultDate.Year
                    , defaultDate.Month
                    , defaultDate.Day
                    , int.Parse(timeString.Substring(0, 1))
                    , int.Parse(timeString.Substring(2, 2))
                    , 0);
            }
            else if (regExHHColonMM.IsMatch(timeString))
            {
                dateTimeInGMT = new DateTime(defaultDate.Year
                    , defaultDate.Month
                    , defaultDate.Day
                    , int.Parse(timeString.Substring(0, 2))
                    , int.Parse(timeString.Substring(3, 2))
                    , 0);
            }
            else if (regExHMM.IsMatch(timeString))
            {
                dateTimeInGMT = new DateTime(defaultDate.Year
                    , defaultDate.Month
                    , defaultDate.Day
                    , int.Parse(timeString.Substring(0, 1))
                    , int.Parse(timeString.Substring(1, 2))
                    , 0);
            }
            else
            {
                return false;
            }

            timezoneOffset = TimeZone.CurrentTimeZone.GetUtcOffset(dateTimeInGMT);
            dateTimeInGMT = dateTimeInGMT.ToUniversalTime();
            return true;
        }
    }

}
