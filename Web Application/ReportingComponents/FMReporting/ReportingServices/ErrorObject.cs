/// <summary>
/// File name:	ErrorObject.cs
/// Purpose:	The purpose of this object is to contain an error message, exception
///				error message and a flag to indicate if there are any errors.
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
	public class ErrorObject : DataObjectBase
	{
		#region Public Attributes
		public enum ErrorLevels {CRITICAL, ERROR, WARNING, NONE};
		#endregion

		#region Private Attributes
		private ErrorLevels errorLevel; 
		private string errorMsg;
		private string exceptionMsg;
		private bool errorFlag;
		#endregion

		#region Constructors
		/// <summary>
		/// This is the default constructor for the error object.
		/// </summary>
		public ErrorObject()
		{
			this.ClearErrors();
		}
		#endregion

		#region Properties
		/// <summary>
		/// This property will get and set the error level.
		/// </summary>
		public ErrorLevels ErrorLevel
		{
			get { return this.errorLevel; }
			set { this.errorLevel = value; }
		}

		/// <summary>
		/// This property will get and set the error flag. True indicates that
		/// there is an error and false indicates no error.
		/// </summary>
		public bool HasErrors
		{
			get { return this.errorFlag; }
			set { this.errorFlag = value; }
		}

		/// <summary>
		/// This property will get and set the error message.
		/// </summary>
		public string ErrorMessage
		{
			get { return this.errorMsg; }
			set 
			{ 
				string errorMessage = value;

				switch (this.errorLevel)
				{
					case ErrorObject.ErrorLevels.CRITICAL:
						this.errorFlag = true;
						this.errorMsg = "Critical: " + errorMessage;
						break;

					case ErrorObject.ErrorLevels.ERROR:
						this.errorFlag = true;
						this.errorMsg = "Error: " + errorMessage;
						break;
					case ErrorObject.ErrorLevels.WARNING:
						this.errorMsg = "Warning: " + errorMessage;
						break;

					default:
						this.errorFlag = true;
						this.errorMsg = "Error: " + errorMessage;
						break;
				}
			}
		}

		/// <summary>
		/// This property will get and set the exception error message.
		/// </summary>
		public string ExceptionError
		{
			get { return this.exceptionMsg; }
			set 
			{ 
				this.exceptionMsg = value; 
				this.errorFlag = true;
			}
		}
		#endregion

		#region Public Methods
		/// <summary>
		/// This method will clear all the error messages and flags.
		/// </summary>
		public void ClearErrors()
		{
			this.errorMsg = "";
			this.exceptionMsg = "";
			this.errorFlag = false;
			this.errorLevel = ErrorLevels.NONE;
		}
		#endregion
	}
}
