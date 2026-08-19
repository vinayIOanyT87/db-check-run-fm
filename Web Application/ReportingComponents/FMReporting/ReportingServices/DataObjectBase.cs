/// <summary>
/// File name:	DataObjectBase.cs
/// Purpose:	This is the base data objects class for all derived data objects.
///	Comments:	Copyright (C) E+H Systems & Gauging, Inc. Norcross, GA, USA, 
///				2000.  This file shall not be copied or reproduced in any form 
///				without the express written consent of Endress+Hauser.
///	Author(s):	Richard R. Panachida
///	Version:	1.0.0  Current version
///	
///	Modification History:
///		Date:			By:						Reason:
///		----------		--------------------	----------------------------------
///		
/// </summary>
/// 
using System;

namespace ReportingServices
{
	[System.Serializable]
	public abstract class DataObjectBase
	{
		#region Attributes
//		public enum UnitConversionTypes {TEMPERATURE, DENSITY, WEIGHT, VOLUME};
//		private   UnitConversion  unitConversion;
		#endregion

		#region Constructor
		/// <summary>
		/// This is the default constructor for the data object base class.
		/// </summary>
		public DataObjectBase()
		{
		}
		#endregion

		#region Protected Methods
		/// <summary>
		/// This method will return a DB null if the object is null.  The
		/// system cannot handle a regular null.
		/// </summary>
		/// <param name="inObject"></param>
		/// <returns></returns>
		protected bool isNull(object inObject)
		{
			return (inObject == System.DBNull.Value);
		}

		/// <summary>
		/// This method will determine if the row has a null value.  If so,
		/// then the method will return an empty string.
		/// </summary>
		/// <param name="row"></param>
		/// <returns></returns>
		protected string getString(object row)
		{
			if (isNull(row))
				return "";
			else
				return (string) row;
		}

		/// <summary>
		/// This method will determine if the row has a null value.  If so,
		/// then the method will return a regular null and not a DB Null.
		/// </summary>
		/// <param name="row"></param>
		/// <returns></returns>
		protected System.DateTime getDateTime(object row)
		{
			return (System.DateTime) row;
		}

		/// <summary>
		/// This method will determine if the row has a null value.  If so,
		/// then an empty string is returned.  Else, a string representing the
		/// date only is return.
		/// </summary>
		/// <param name="row"></param>
		/// <returns></returns>
		protected string getDateTimeString(object row)
		{
			if (isNull(row))
			{
				return "";
			}
			else
			{
				System.DateTime newDate = (System.DateTime) row;
				string dateStr = newDate.Year + "-" + newDate.Month + "-" + newDate.Day + 
					" " + newDate.Hour + ":" + newDate.Minute + ":" + newDate.Second;
				return dateStr;
			}
		}

		/// <summary>
		/// This method will determine if the row has a null value.  If so,
		/// then a double point zero is returned. Else, the double point
		/// is returned.
		/// </summary>
		/// <param name="row"></param>
		/// <returns></returns>
		protected double getFloat(object row)
		{
			if (isNull(row))
				return 0.0F;
			else
				return (float) row;
		}

		/// <summary>
		/// This method will determine if the row has a null value.  If so,
		/// then a double doubleing point zero is returned. Else, the double doubleing point
		/// is returned.
		/// </summary>
		/// <param name="row"></param>
		/// <returns></returns>
		protected double getDouble(object row)
		{
			if (isNull(row) == true)
				return 0.0;
			else
				return (double) row;
		}

		/// <summary>
		/// This method will determine if the row has a null value.  If so,
		/// then returns a boolean row.
		/// </summary>
		/// <param name="row"></param>
		/// <returns></returns>
		protected bool getBool(object row)
		{
			return ((!isNull(row)) && (bool) row);
		}

		/// <summary>
		/// This method will determine if the row has a null value.  If so,
		/// then an integer zero is returned. Else, the actual value is returned.
		/// </summary>
		/// <param name="row"></param>
		/// <returns></returns>
		protected int getInt(object row)
		{
			if (isNull(row) == true)
				return 0;
			else
				return (int) row;
		}

		/// <summary>
		/// This method will determine if the row has a null value.  If so,
		/// then a long zero is returned. Else, the actual value is returned.
		/// </summary>
		/// <param name="row"></param>
		/// <returns></returns>
		protected long getLong(object row)
		{
			if (isNull(row) == true)
				return 0;
			else
				return (long) row;
		}

		/// <summary>
		/// This method will determine if the date time is valid. It will return true
		/// if valid or false otherwise.
		/// </summary>
		/// <param name="row"></param>
		/// <returns></returns>
		protected bool IsDateValid(object row)
		{
			if (isNull(row) == true)
				return false;
			else
				return true;
		}
		#endregion

		#region Public Methods
//		public void setUnits(UnitConversion unitConv)
//		{
//			this.unitConversion = unitConv;
//		}

		public void load(System.Data.DataSet dataSet)
		{
		}

		#endregion

		#region ISerializable Members
		public void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
		{
			// TODO:  Add DataObject.GetObjectData implementation
		}

		#endregion
	}
}
