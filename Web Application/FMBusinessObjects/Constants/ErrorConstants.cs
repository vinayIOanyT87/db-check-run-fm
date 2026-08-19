// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ErrorConstants.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Contains error-related constants and strings (exposed from resources)
//   Instances of this class allow for aggregation of error messages for
//   a detailed summary of a failure
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FMBusinessObjects.Constants
{
	/// <summary>
	/// Contains error-related constants and strings (exposed from resources)
	/// Instances of this class allow for aggregation of error messages for 
	/// a detailed summary of a failure
	/// </summary>
	public class ErrorConstants
	{
		#region Attributes
		/// <summary>
		/// The error message.
		/// </summary>
		private string errorMessage;

		/// <summary>
		/// The error flag.
		/// </summary>
		private bool errorFlag;
		#endregion

		#region Constructors
		/// <summary>
		/// Initializes a new instance of the <see cref="ErrorConstants"/> class.
		/// </summary>
		public ErrorConstants( )
		{
			this.errorMessage = string.Empty;
			this.errorFlag = false;
		}
		#endregion

		#region Errors for Data Retriever
		/// <summary>
		/// Gets error - Could not retrieve equipment!
		/// </summary>
		public string ERR_MSG_01001
		{
			get { return ErrorStrings.ERR_MSG_01001; }
		}

		/// <summary>
		/// Gets error - Could not retrieve equipment types!
		/// </summary>
		public string ERR_MSG_01002
		{
			get { return ErrorStrings.ERR_MSG_01002; }
		}

		/// <summary>
		/// Gets error - Could not retrieve employees!
		/// </summary>
		public string ERR_MSG_01003
		{
			get { return ErrorStrings.ERR_MSG_01003; }
		}

		/// <summary>
		/// Gets error - Could not retrieve users!
		/// </summary>
		public string ERR_MSG_01004
		{
			get { return ErrorStrings.ERR_MSG_01004; }
		}

		/// <summary>
		/// Gets error - Could not retrieve products!
		/// </summary>
		public string ERR_MSG_01005
		{
			get { return ErrorStrings.ERR_MSG_01005; }
		}

		/// <summary>
		/// Gets error - Could not retrieve sites!
		/// </summary>
		public string ERR_MSG_01006
		{
			get { return ErrorStrings.ERR_MSG_01006; }
		}

		/// <summary>
		/// Gets error - Null hash table entered.
		/// </summary>
		public string ERR_MSG_01007
		{
			get { return ErrorStrings.ERR_MSG_01007; }
		}
		#endregion

		#region Errors for XML Generator
		/// <summary>
		/// Gets error - Could not find xsd path in the registry!
		/// </summary>
		public string ERR_MSG_02001
		{
			get { return ErrorStrings.ERR_MSG_02001; }
		}

		/// <summary>
		/// Gets error - Could not access the registry!
		/// </summary>
		public string ERR_MSG_02002
		{
			get { return ErrorStrings.ERR_MSG_02002; }
		}

		/// <summary>
		/// Gets error - Could not build access data XML schema!
		/// </summary>
		public string ERR_MSG_02003
		{
			get { return ErrorStrings.ERR_MSG_02003; }
		}

		/// <summary>
		/// Gets error - Could not perform XML validation!
		/// </summary>
		public string ERR_MSG_02004
		{
			get { return ErrorStrings.ERR_MSG_02004; }
		}

		/// <summary>
		/// Gets error - Validation errors:
		/// </summary>
		public string ERR_MSG_02005
		{
			get { return ErrorStrings.ERR_MSG_02005; }
		}

		/// <summary>
		/// Gets error - Field type mismatch with the xsd!
		/// </summary>
		public string ERR_MSG_02006
		{
			get { return ErrorStrings.ERR_MSG_02006; }
		}
		#endregion

		#region Errors for Accounting data retriever
		/// <summary>
		/// Gets error - Accounting file download path configured in registry does not exist.
		/// </summary>
		public string ERR_MSG_03001
		{
			get { return ErrorStrings.ERR_MSG_03001; }
		}

		/// <summary>
		/// Gets error - Unable to retrieve accounting file download path from the registry\nReason:
		/// </summary>
		public string ERR_MSG_03002
		{
			get { return ErrorStrings.ERR_MSG_03002; }
		}

		/// <summary>
		/// Gets error - No accounting Data Available to Send
		/// </summary>
		public string ERR_MSG_03003
		{
			get { return ErrorStrings.ERR_MSG_03003; }
		}

		/// <summary>
		/// Gets error - Unable to Retrieve Accouting Data:
		/// </summary>
		public string ERR_MSG_03004
		{
			get { return ErrorStrings.ERR_MSG_03004; }
		}
		#endregion

		#region Errors for the GUI
		/// <summary>
		/// Gets error - An error occurred while generating the secure file.  Error:
		/// </summary>
		public string ERR_MSG_04001
		{
			get { return ErrorStrings.ERR_MSG_04001; }
		}

		/// <summary>
		/// Gets error - XML document is empty!
		/// </summary>
		public string ERR_MSG_04002
		{
			get { return ErrorStrings.ERR_MSG_04002; }
		}

		/// <summary>
		/// Gets error - Could not compress and encrypt enterprise data!
		/// </summary>
		public string ERR_MSG_04003
		{
			get { return ErrorStrings.ERR_MSG_04003; }
		}

		/// <summary>
		/// Gets error - Could not retrieve data!
		/// </summary>
		public string ERR_MSG_04004
		{
			get { return ErrorStrings.ERR_MSG_04004; }
		}

		/// <summary>Could not save data!</summary>
		public string ERR_MSG_04005
		{
			get { return ErrorStrings.ERR_MSG_04005; }
		}

		/// <summary>
		/// Gets error - An Error has occurred!
		/// </summary>
		public string ERR_MSG_04006
		{
			get { return ErrorStrings.ERR_MSG_04006; }
		}

		/// <summary>
		/// Gets error - Could not access the WFL.
		/// </summary>
		public string ERR_MSG_04007
		{
			get { return ErrorStrings.ERR_MSG_04007; }
		}

		/// <summary>
		/// Gets error - Could not send data to the Enterprise.
		/// </summary>
		public string ERR_MSG_04008
		{
			get { return ErrorStrings.ERR_MSG_04008; }
		}

		/// <summary>
		/// Gets error - Looks up a localized string similar to An error occurred while generating the secure file.  Error:.
		/// </summary>
		public string ERR_MSG_04009
		{
			get { return ErrorStrings.ERR_MSG_04009; }
		}
		#endregion

		#region Errors for IM Retriever
		/// <summary>
		/// Gets error - No DataManager configured.
		/// </summary>
		public string ERR_MSG_05001
		{
			get { return ErrorStrings.ERR_MSG_05001; }
		}

		/// <summary>
		/// Gets error - DataManager not found or error accessing DataManager.
		/// </summary>
		public string ERR_MSG_05002
		{
			get { return ErrorStrings.ERR_MSG_05002; }
		}

		/// <summary>
		/// Gets error - DataManager not running.
		/// </summary>
		public string ERR_MSG_05003
		{
			get { return ErrorStrings.ERR_MSG_05003; }
		}

		/// <summary>
		/// Gets error - Unable to read point value from tank 
		/// </summary>
		public string ERR_MSG_05004
		{
			get { return ErrorStrings.ERR_MSG_05004; }
		}
		#endregion

		#region Errors for the Express upload page (local base).
		/// <summary>
		/// Gets error - Invalid user!
		/// </summary>
		public string ERR_MSG_06001
		{
			get { return ErrorStrings.ERR_MSG_06001; }
		}

		/// <summary>
		/// Gets error - Could not find the enterprise secure file!
		/// </summary>
		public string ERR_MSG_06002
		{
			get { return ErrorStrings.ERR_MSG_06002; }
		}

		/// <summary>
		/// Gets error - Invalid Session information!
		/// </summary>
		public string ERR_MSG_06003
		{
			get { return ErrorStrings.ERR_MSG_06003; }
		}

		/// <summary>
		/// Gets error - Could not decrypt and/or decompress the stream!
		/// </summary>
		public string ERR_MSG_06004
		{
			get { return ErrorStrings.ERR_MSG_06004; }
		}

		/// <summary>
		/// Gets error - The enterprise decrypted/decompressed data is empty!
		/// </summary>
		public string ERR_MSG_06005
		{
			get { return ErrorStrings.ERR_MSG_06005; }
		}

		/// <summary>
		/// Gets error - The enterprise data string is empty!
		/// </summary>
		public string ERR_MSG_06006
		{
			get { return ErrorStrings.ERR_MSG_06006; }
		}

		/// <summary>
		/// Gets error - Error in saving enterprise data, rolling back!
		/// </summary>
		public string ERR_MSG_06007
		{
			get { return ErrorStrings.ERR_MSG_06007; }
		}

		/// <summary>
		/// Gets error - Could not find the Document DODAAC number!
		/// </summary>
		public string ERR_MSG_06008
		{
			get { return ErrorStrings.ERR_MSG_06008; }
		}

		/// <summary>
		/// Gets error - The document DODAAC does not match the site DODACC!
		/// </summary>
		public string ERR_MSG_06009
		{
			get { return ErrorStrings.ERR_MSG_06009; }
		}

		/// <summary>
		/// Gets error - Accounting transactions not set.
		/// </summary>
		public string ERR_MSG_06010
		{
			get { return ErrorStrings.ERR_MSG_06010; }
		}

		/// <summary>
		/// Gets error - Bad AccountSND header line.
		/// </summary>
		public string ERR_MSG_06011
		{
			get { return ErrorStrings.ERR_MSG_06011; }
		}

		/// <summary>
		/// Gets error - Error in opening stream!
		/// </summary>
		public string ERR_MSG_07001
		{
			get { return ErrorStrings.ERR_MSG_07001; }
		}

		/// <summary>
		/// Gets error - Error in reading stream!
		/// </summary>
		public string ERR_MSG_07002
		{
			get { return ErrorStrings.ERR_MSG_07002; }
		}

		/// <summary>
		/// Gets error - Corrupted file!
		/// </summary>
		public string ERR_MSG_07003
		{
			get { return ErrorStrings.ERR_MSG_07003; }
		}

		/// <summary>
		/// Gets error - This property returns the error message;
		/// </summary>
		public string ErrorMessage
		{
			get { return this.errorMessage; }
		}

		/// <summary>
		/// Gets error - This property sets or gets the error flag value (true or false).
		/// </summary>
		public bool ErrorFlag
		{
			get { return this.errorFlag; }
			set { this.errorFlag = value; }
		}
		#endregion

        #region Errors for the Synchronization Engine.
        /// <summary>
        /// Gets error - BindingType for enterprise synchronization service not found in configuration
        /// </summary>
        public static string SYNC_ERR_MSG_08001
        {
            get { return ErrorStrings.ERR_MSG_08001; }
        }

        /// <summary>
        /// Gets error - Client synchronization settings missing
        /// </summary>
        public static string SYNC_ERR_MSG_08002
        {
            get { return ErrorStrings.ERR_MSG_08002; }
        }

        /// <summary>
        /// Gets error - Root Site / SiteGroup ID not specified in client synchronization settings
        /// </summary>
        public static string SYNC_ERR_MSG_08003
        {
            get { return ErrorStrings.ERR_MSG_08003; }
        }

        /// <summary>
        /// Gets error - Enterprise synchronization URL not specified in client synchronization settings
        /// </summary>
        public static string SYNC_ERR_MSG_08004
        {
            get { return ErrorStrings.ERR_MSG_08004; }
        }

        /// <summary>
        /// Gets error - Synchronization currently disabled on this client
        /// </summary>
        public static string SYNC_ERR_MSG_08005
        {
            get { return ErrorStrings.ERR_MSG_08005; }
        }

        /// <summary>
        /// Gets error - Cannot synchronize with same system
        /// </summary>
        public static string SYNC_ERR_MSG_08006
        {
            get { return ErrorStrings.ERR_MSG_08006; }
        }

        /// <summary>
        /// Gets error - Cannot locate local synchronization node id.
        /// </summary>
        public static string SYNC_ERR_MSG_08007
        {
            get { return ErrorStrings.ERR_MSG_08007; }
        }

        /// <summary>
        /// Gets error - Cannot locate enterprise synchronization node id.
        /// </summary>
        public static string SYNC_ERR_MSG_08008
        {
            get { return ErrorStrings.ERR_MSG_08008; }
        }

        /// <summary>
        /// Gets error - Enterprise synchronization server configuration missing
        /// </summary>
        public static string SYNC_ERR_MSG_08009
        {
            get { return ErrorStrings.ERR_MSG_08009; }
        }

        /// <summary>
        /// Gets error - Enterprise server currently not accepting requests
        /// </summary>
        public static string SYNC_ERR_MSG_08010
        {
            get { return ErrorStrings.ERR_MSG_08010; }
        }

        /// <summary>
        /// Gets error - Enterprise server FuelsManager authentication configuration error
        /// </summary>
        public static string SYNC_ERR_MSG_08011
        {
            get { return ErrorStrings.ERR_MSG_08011; }
        }

        /// <summary>
        /// Gets error - Missing Client Authentication Certificate
        /// </summary>
        public static string SYNC_ERR_MSG_08012
        {
            get { return ErrorStrings.ERR_MSG_08012; }
        }

        /// <summary>
        /// Gets error - Missing Client Authentication Credentials
        /// </summary>
        public static string SYNC_ERR_MSG_08013
        {
            get { return ErrorStrings.ERR_MSG_08013; }
        }

        /// <summary>
        /// Gets error - FuelsManager User Account was unable to login to the Enterprise Server.
        /// </summary>
        public static string SYNC_ERR_MSG_08014
        {
            get { return ErrorStrings.ERR_MSG_08014; }
        }

        /// <summary>
        /// Gets error - Unable to create system mutex representing an active synchronization controller.
        /// </summary>
        public static string SYNC_ERR_MSG_08015
        {
            get { return ErrorStrings.ERR_MSG_08015; }
        }

        /// <summary>
        /// Gets error - Unable to create system mutex, an active synchronization controller already exists.
        /// </summary>
        public static string SYNC_ERR_MSG_08016
        {
            get { return ErrorStrings.ERR_MSG_08016; }
        }

		/// <summary>
		/// Gets error - Synchronization currently disabled on this site/sitegroup
		/// </summary>
		public static string SYNC_ERR_MSG_08017
		{
			get { return ErrorStrings.ERR_MSG_08017; }
		}



		#endregion Errors for the Synchronization Engine.

		#region Methods
		/// <summary>
		/// This method will append error messages to the existing error messages.
		/// It will also set the error flag to true.
		/// </summary>
		/// <param name="error">
		/// The error.
		/// </param>
		public void AppendErrors(string error)
		{
			if (string.IsNullOrEmpty(error) == false)
			{
				this.errorMessage = this.errorMessage + error;
				this.errorFlag = true;
			}
		}

		/// <summary>
		/// This method will clear the error message text and set the error
		/// flag to false.
		/// </summary>
		public void ClearErrors( )
		{
			this.errorFlag = false;
			this.errorMessage = string.Empty;
		}
		#endregion
	}
}
