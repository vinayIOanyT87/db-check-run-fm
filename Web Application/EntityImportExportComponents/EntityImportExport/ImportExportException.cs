/******************************************************************************
	FILE NAME:	ImportExportException.cs
	PURPOSE:		ImportExportException

	COMMENTS:
		Copyright (C) Varec, Inc. Norcross, GA, USA, 2008
		This file shall not be copied or reproduced in any form without
		the express written consent of Varec.

	AUTHOR(S):	Richard Panachida
	VERSION:		1.0.0  Current version

	MODIFICATION HISTORY:
		Date:				By:					Reason:
		------------	-----------------	-------------------------------------------
		2008-03-12		B. Nelson			- Added personnel import messages.

		2008-03-27		I.Orndorff			- Added Equipment compartment import exception
													  messages.

		2008-03-28		I.Orndorff			- Added Equipment test inspections import exception
													  messages.
													- Added Equipment tag licenses import exception
													  messages.

		2008-04-03		B. Nelson			- Fixed information messages.
													- Added Product Additive Profile import messages.

		2008-04-03		B. Nelson			- Fixed bug in AppendMessage.

		2008-04-09		I.Orndorff			- Updated IMPORT_MSG_053 and IMPORT_MSG_062 to 
													  reflect "imported" instead of "added".

		2008-04-10		B. Nelson			- Added Company Hierarchy import messages.

		2008-04-10		I.Orndorff			- Added the following new exceptions:
													  IMPORT_MSG_063, IMPORT_MSG_079 and
													  IMPORT_MSG_080.

		2008-04-11		I.Orndorff			- Updated IMPORT_MSG_PROD_008 to 
													  reflect "imported" instead of "added".
													- Removed IMPORT_MSG_PROD_009.

		2008-04-14		I.Orndorff			- Updated IMPORT_MSG_063 to reflect 
													  sequence instead of ID.

		2008-04-14		B. Nelson			- Fixed the company hierarchy messages.
													  Added the product blend component messages.

		2008-04-15		I.Orndorff			- Updated IMPORT_MSG_PERS_008 to reflect
													  "imported" instead of "added".

		2008-04-16		B. Nelson			- Improved license & qualification messages.
  
		2008-11-18		A. Coker				- Added turnover period messages.
 
      2009-03-09     R. Panachida		Defect 1467: Updated to stop processing if the import site does not match
													import data site.
  
		2009-03-26		I.Orndorff			- Added IATA Code exception messages: IMPORT_MSG_IATA_CODES_001 to
													  IMPORT_MSG_IATA_CODES_008. This addresses change request 2434. 
 
      2009-06-02     A. Coker          - Made changes to accommodate import and export of personnel Access 
                                         Schedule.
 

*******************************************************************************/

using System;
using System.Collections;

namespace EntityImportExport
{
	public class ImportExportException : System.Exception
	{
		#region Public data members
		public enum EXCEPTION_TYPES { CRITICAL, ERROR, WARNING, INFO, NONE};

      public const string IMPORT_MSG_001 = "No header row in work sheet ";
      public const string IMPORT_MSG_002 = "No rows found in work sheet ";
		public const string IMPORT_MSG_006 = "Header row column count must match data row column count.";
		public const string IMPORT_MSG_007 = "Could not find worksheet nodes.";
		public const string IMPORT_MSG_008 = "Column header names cannot be null or empty.";
		public const string IMPORT_MSG_009 = "Cannot have two column headers with the same name.";

      // General import messages
      public const string IMPORT_MSG_GNRL_001 = "The import site does not match the current site.";
		#endregion

		#region Private data members
		private EXCEPTION_TYPES exceptionType;
		private string errorMessage;
		private string warningMessage;
		private string criticalMessage;
		private string infoMessage;
		private bool hasException;
		private Hashtable exceptionStringTypes;
      private bool isCritical;
		#endregion

		#region Constructors
		/// <summary>
		/// This is the default constructor for the import/export exception class.
		/// </summary>
		public ImportExportException()
		{
			this.Initialize(null, ImportExportException.EXCEPTION_TYPES.NONE);
		}

		/// <summary>
		/// This constructor sets the error message and defaults the type to error.
		/// </summary>
		/// <param name="errMessage"></param>
		public ImportExportException(string errMessage) : base (errMessage)
		{
			this.Initialize(errMessage, ImportExportException.EXCEPTION_TYPES.ERROR);
		}

		/// <summary>
		/// This constructor sets the error type to (critical, error, warning, info).
		/// </summary>
		/// <param name="errMessage"></param>
		/// <param name="errType"></param>
		public ImportExportException(string errMessage, ImportExportException.EXCEPTION_TYPES errType) : base (errMessage)
		{
			this.Initialize(errMessage, errType);
		}
		#endregion

		#region Properties
		public string CriticalMessage
		{
			get { return this.criticalMessage; }
		}

		public string ErrorMessage
		{
			get { return this.errorMessage; }
		}

		public string InfoMessage
		{
			get { return this.infoMessage; }
		}

		public string WarningMessage
		{
			get { return this.warningMessage; }
		}

		public ImportExportException.EXCEPTION_TYPES ExceptionType
		{
			get { return this.exceptionType; }
			set { this.exceptionType = value; }
		}

		public bool HasException
		{
			get { return this.hasException; }
		}

      public bool IsCritical
      {
         get { return this.isCritical; }
      }
		#endregion

		#region Public methods
		/// <summary>
		/// This method will append to the message text.
		/// </summary>
		/// <param name="message"></param>
		/// <param name="exceptType"></param>
		public void AppendMessage(string inMessage, ImportExportException.EXCEPTION_TYPES exceptionType)
		{
			string message = "";

			if ((inMessage != null) && (inMessage.Length > 0))
			{
				message = inMessage;
				this.hasException = true;
			}

			switch (exceptionType)
			{
				case ImportExportException.EXCEPTION_TYPES.CRITICAL:
				{
					if ((this.criticalMessage == null) || (this.criticalMessage.Length <= 0))
					{
						this.criticalMessage = ((string) this.exceptionStringTypes[ImportExportException.EXCEPTION_TYPES.CRITICAL])
									+ "\n" + message;
					}
					else if (!this.criticalMessage.Contains(message))
						{
						this.criticalMessage = this.criticalMessage + "\n" + message;
					}

               this.isCritical = true;
					break;
				}

				case ImportExportException.EXCEPTION_TYPES.ERROR:
				{
					if ((this.errorMessage == null) || (this.errorMessage.Length <= 0))
					{
						this.errorMessage = ((string) this.exceptionStringTypes[ImportExportException.EXCEPTION_TYPES.ERROR])
									+ "\n" + message;
					}
					// Dont add duplicate messages 
					else if (!this.errorMessage.Contains(message))
					{
					this.errorMessage = this.errorMessage + "\n" + message;
					}
					break;
				}

				case ImportExportException.EXCEPTION_TYPES.INFO:
				{
					if ((this.infoMessage == null) || (this.infoMessage.Length <= 0))
					{
						this.infoMessage = ((string) this.exceptionStringTypes[ImportExportException.EXCEPTION_TYPES.INFO])
									+ "\n" + message;
					}
					// Dont add duplicate messages 
					else if (!this.infoMessage.Contains(message))
					{
						this.infoMessage = this.infoMessage + "\n" + message;
					}
					break;
				}

				case ImportExportException.EXCEPTION_TYPES.WARNING:
				{
					if ((this.warningMessage == null) || (this.warningMessage.Length <= 0))
					{
						this.warningMessage = ((string) this.exceptionStringTypes[ImportExportException.EXCEPTION_TYPES.WARNING])
									+ "\n" + message;
					}
					// Dont add duplicate messages
					else if (!this.warningMessage.Contains(message))
					{
						this.warningMessage = this.warningMessage + "\n" + message;
					}
					break;
				}

				default:
				{
					if ((this.infoMessage == null) || (this.infoMessage.Length <= 0))
					{
						this.infoMessage = ((string) this.exceptionStringTypes[ImportExportException.EXCEPTION_TYPES.INFO])
									+ "\n" + message;
					}
					// Dont add duplicate messages
					else if (!this.infoMessage.Contains(message))
					{
						this.infoMessage = this.infoMessage + "\n" + message;
					}
					break;
				}
			}
		}

		/// <summary>
		/// This method will clear the messages.
		/// </summary>
		public void ClearMessages()
		{
			this.criticalMessage = "";
			this.errorMessage    = "";
			this.warningMessage  = "";
			this.infoMessage     = "";
			this.hasException    = false;
		}
		#endregion

		#region Private methods
		/// <summary>
		/// This method initializes the import/export exception object to its
		/// initial state.
		/// </summary>
		private void Initialize(string inMessage, ImportExportException.EXCEPTION_TYPES exceptionType)
		{
			this.exceptionType = exceptionType;
			this.hasException  = false;
			string message     = "";

			if ((inMessage != null) && (inMessage.Length > 0))
			{
				message = inMessage;
			}

			switch (exceptionType)
			{
				case ImportExportException.EXCEPTION_TYPES.CRITICAL:
					this.criticalMessage = message;
               this.isCritical = false;

               if ((message != null) && (message.Length > 0))
               {
                  this.isCritical = true;
               }
					break;

				case ImportExportException.EXCEPTION_TYPES.ERROR:
					this.errorMessage = message;
					break;

				case ImportExportException.EXCEPTION_TYPES.INFO:
					this.infoMessage = message;
					break;

				case ImportExportException.EXCEPTION_TYPES.WARNING:
					this.warningMessage = message;
					break;

				default:
					this.infoMessage = message;
					break;
			}

			this.exceptionStringTypes = new Hashtable();
			this.exceptionStringTypes.Add(ImportExportException.EXCEPTION_TYPES.CRITICAL, "*** Critical Messages ***");
			this.exceptionStringTypes.Add(ImportExportException.EXCEPTION_TYPES.ERROR,    "*** Error Messages ***");
			this.exceptionStringTypes.Add(ImportExportException.EXCEPTION_TYPES.WARNING,  "*** Warning Messages ***");
			this.exceptionStringTypes.Add(ImportExportException.EXCEPTION_TYPES.INFO,     "*** Info Messages ***");
		}
		#endregion
	}
}
