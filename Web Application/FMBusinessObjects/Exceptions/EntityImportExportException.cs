using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;

namespace FMBusinessObjects.Exceptions
{
	public class EntityImportExportException : Exception
	{
		#region Public data members
		public enum EXCEPTION_TYPES { CRITICAL, ERROR, WARNING, INFO, NONE };

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
		public EntityImportExportException ( )
		{
			this.Initialize ( null, EntityImportExportException.EXCEPTION_TYPES.NONE );
		}

		/// <summary>
		/// This constructor sets the error message and defaults the type to error.
		/// </summary>
		/// <param name="errMessage"></param>
		public EntityImportExportException ( string errMessage ) : base ( errMessage )
		{
			this.Initialize ( errMessage, EntityImportExportException.EXCEPTION_TYPES.ERROR );
		}

		/// <summary>
		/// This constructor sets the error type to (critical, error, warning, info).
		/// </summary>
		/// <param name="errMessage"></param>
		/// <param name="errType"></param>
		public EntityImportExportException ( string errMessage, EntityImportExportException.EXCEPTION_TYPES errType ) : base ( errMessage )
		{
			this.Initialize ( errMessage, errType );
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

		public EntityImportExportException.EXCEPTION_TYPES ExceptionType
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
		public void AppendMessage ( string inMessage, EntityImportExportException.EXCEPTION_TYPES exceptionType )
		{
			string message = "";

			if (( inMessage != null ) && ( inMessage.Length > 0 ))
			{
				message = inMessage;
				this.hasException = true;
			}

			switch (exceptionType)
			{
				case EntityImportExportException.EXCEPTION_TYPES.CRITICAL:
					{
						if (( this.criticalMessage == null ) || ( this.criticalMessage.Length <= 0 ))
						{
							this.criticalMessage = ( (string) this.exceptionStringTypes[EntityImportExportException.EXCEPTION_TYPES.CRITICAL] )
										+ "\n" + message;
						}
						else
						{
							this.criticalMessage = this.criticalMessage + "\n" + message;
						}

						this.isCritical = true;
						break;
					}

				case EntityImportExportException.EXCEPTION_TYPES.ERROR:
					{
						if (( this.errorMessage == null ) || ( this.errorMessage.Length <= 0 ))
						{
							this.errorMessage = ( (string) this.exceptionStringTypes[EntityImportExportException.EXCEPTION_TYPES.ERROR] )
										+ "\n" + message;
						}
						else
						{
							this.errorMessage = this.errorMessage + "\n" + message;
						}
						break;
					}

				case EntityImportExportException.EXCEPTION_TYPES.INFO:
					{
						if (( this.infoMessage == null ) || ( this.infoMessage.Length <= 0 ))
						{
							this.infoMessage = ( (string) this.exceptionStringTypes[EntityImportExportException.EXCEPTION_TYPES.INFO] )
										+ "\n" + message;
						}
						else
						{
							this.infoMessage = this.infoMessage + "\n" + message;
						}
						break;
					}

				case EntityImportExportException.EXCEPTION_TYPES.WARNING:
					{
						if (( this.warningMessage == null ) || ( this.warningMessage.Length <= 0 ))
						{
							this.warningMessage = ( (string) this.exceptionStringTypes[EntityImportExportException.EXCEPTION_TYPES.WARNING] )
										+ "\n" + message;
						}
						else
						{
							this.warningMessage = this.warningMessage + "\n" + message;
						}
						break;
					}

				default:
					{
						if (( this.infoMessage == null ) || ( this.infoMessage.Length <= 0 ))
						{
							this.infoMessage = ( (string) this.exceptionStringTypes[EntityImportExportException.EXCEPTION_TYPES.INFO] )
										+ "\n" + message;
						}
						else
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
		public void ClearMessages ( )
		{
			this.criticalMessage = "";
			this.errorMessage = "";
			this.warningMessage = "";
			this.infoMessage = "";
			this.hasException = false;
		}
		#endregion

		#region Private methods
		/// <summary>
		/// This method initializes the import/export exception object to its
		/// initial state.
		/// </summary>
		private void Initialize ( string inMessage, EntityImportExportException.EXCEPTION_TYPES exceptionType )
		{
			this.exceptionType = exceptionType;
			this.hasException = false;
			string message = "";

			if (( inMessage != null ) && ( inMessage.Length > 0 ))
			{
				message = inMessage;
			}

			switch (exceptionType)
			{
				case EntityImportExportException.EXCEPTION_TYPES.CRITICAL:
					this.criticalMessage = message;
					this.isCritical = false;

					if (( message != null ) && ( message.Length > 0 ))
					{
						this.isCritical = true;
					}
					break;

				case EntityImportExportException.EXCEPTION_TYPES.ERROR:
					this.errorMessage = message;
					break;

				case EntityImportExportException.EXCEPTION_TYPES.INFO:
					this.infoMessage = message;
					break;

				case EntityImportExportException.EXCEPTION_TYPES.WARNING:
					this.warningMessage = message;
					break;

				default:
					this.infoMessage = message;
					break;
			}

			this.exceptionStringTypes = new Hashtable ( );
			this.exceptionStringTypes.Add ( EntityImportExportException.EXCEPTION_TYPES.CRITICAL, "*** Critical Messages ***" );
			this.exceptionStringTypes.Add ( EntityImportExportException.EXCEPTION_TYPES.ERROR, "*** Error Messages ***" );
			this.exceptionStringTypes.Add ( EntityImportExportException.EXCEPTION_TYPES.WARNING, "*** Warning Messages ***" );
			this.exceptionStringTypes.Add ( EntityImportExportException.EXCEPTION_TYPES.INFO, "*** Info Messages ***" );
		}
		#endregion
	}
}
