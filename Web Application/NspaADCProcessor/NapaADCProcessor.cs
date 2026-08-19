namespace Nspa
{
	using System;
	using System.Diagnostics;
	using System.IO;
	using System.Linq;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.Interfaces;

	using ADC.Nspa.General;

	public class NspaADCProcessor : IFMDataExchangeProcessor
	{
	    #region Private Attributes
		private string interfacePath;
		#endregion

		#region Public IFMDataExchangeProcessor Methods
		public string InterfaceID
		{
			get { return "A89AF85B-3846-4145-B647-44AABC910D52"; }
		}

		public string InterfacePath
		{
			get
			{
				return this.interfacePath;
			}
			set
			{
				this.interfacePath = value.TrimEnd("\\".ToCharArray());
			}
		}

		public string ProcessData(SecurityClass security, string exchangeData)
		{
			var logMessage = string.Empty;
			var retVal = string.Empty;
			RequestProcessorBase processor = null;
            ExchangeType exchangeType = ExchangeType.None;

			try
			{
				if (security == null)
				{
                    Helper.NspaADCEventLog.WriteEntry("Security parameter is empty.", EventLogEntryType.Error);
					throw new ArgumentNullException("security");
				}

				if (String.IsNullOrEmpty(exchangeData))
				{
                    Helper.NspaADCEventLog.WriteEntry("ExchangeData parameter is empty.", EventLogEntryType.Error);
                    throw new ArgumentNullException("exchangeData");
				}

                if (exchangeData.IndexOf("DownloadReferenceDataRequest", 0, 100) != -1)
				{
					exchangeType = ExchangeType.DownloadReferenceData;
					processor = new DownloadReferenceDataRequestProcessor();
				}

				else if (exchangeData.IndexOf("UploadLogFileRequest", 0, 100) != -1)
				{
					exchangeType = ExchangeType.UploadLogFile;
					processor = new UploadLogFileRequestProcessor();
				}

				else if (exchangeData.IndexOf("UploadTransactionsRequest", 0, 100) != -1)
				{
					exchangeType = ExchangeType.UploadTransactions;
					processor = new UploadTransactionsRequestProcessor();
				}

				if (processor == null)
				{
					throw new Exception("Unhandled Exchange Type");
				}
				try
				{
					processor.Process(security, exchangeData);
					retVal = processor.BaseResponse.ToString();
				}
				catch (Exception ex)
				{
					var errorMessage = string.Format("An error occurred while {0}: {1}: {2}", processor.OperationDescription, ex.Message, ex.StackTrace);
                    Helper.NspaADCEventLog.WriteEntry(errorMessage, EventLogEntryType.Error);
                    processor.AddResponseError("ProcessData", errorMessage);
					retVal = processor.BaseResponse.ToString();
				}
			}
			finally
			{
				if (processor != null)
				{
					if (processor.BaseRequest != null)
					{
                        var fullMessage = string.Format(
							"{0} {1}, user={2}",
							exchangeType,
							processor.BaseResponse.Success ? "success" : "failure",
							security.UserID ?? "[Unknown]");
					    if (processor.BaseResponse.Success)
					    {
                            Helper.LogFmEventSuccess(security, processor.BaseRequest.ClientHostName, fullMessage);
                        }
                        else
					    {
                            Helper.LogFmEventFailure(security, processor.BaseRequest.ClientHostName, fullMessage);
                        }
                    }
					processor.Dispose();
				}
			}

			return retVal;
		}

        private string GenerateFileName(string clientHostName)
        {
            return String.Format("ADC_{0}_RawTransactionData_{1:yyyyMMdd_hhmmss}.xml", clientHostName, DateTime.Now);
        }

		public bool Authenticate
		{
			get
			{
				return true;
			}
		}
		#endregion
		#region Public Properties
		#endregion

		/// <summary>
		/// These are the transactions that Nspa can create
		/// </summary>
		/// <returns></returns>
		internal static bool IsTransactionAliasIncluded(TransactionTypes aliasType)
		{
			bool isIncluded;
			switch (aliasType)
			{
				case TransactionTypes.T4_SecondaryDefuel:		// defuel
				case TransactionTypes.T6_SecondaryDisbursement: // Sale
				case TransactionTypes.T8_Receipt:
				case TransactionTypes.T12_InventoryNotAffected: // Recirculation
					isIncluded = true;
					break;
				default:
					isIncluded = false;
					break;
			}
			return isIncluded;
		}

        internal static CompanyClass FindCompanyWithDefault(SecurityClass security, COMPANY_ROLE theRole, string defaultCompanyId = "")
        {
            var companyList = FMChannelHelper.MakeCall<ICompanies, CompanyCollectionClass>(
                companyService => companyService.EnumerateByRoleGetIDCodeTypesIdentityGuidOnly(security,
                    new COMPANY_ROLE[] { theRole }));

            if (companyList.Count == 0)
            {
                var message = string.Format("No {0} configured for site.", CompanyRoleMapClass.RoleID(theRole));
                Helper.NspaADCEventLog.WriteEntry(message, EventLogEntryType.Warning);
                throw new Exception(message);
            }

            CompanyClass theCompany = null;
            if (string.IsNullOrWhiteSpace(defaultCompanyId))
            {
                theCompany = companyList[0];
            }
            else
            {
                try
                {
                    theCompany = companyList.First(c => c.ID == defaultCompanyId);
                }
                catch (Exception)
                {
                    // we don't mind if there is no company found... fall through with default company=null
                }

                if (theCompany == null)
                {
                    var message = string.Format("Site '{2}' default company '{0}' is not properly configured with role '{1}'.", defaultCompanyId, CompanyRoleMapClass.RoleID(theRole), security.SiteID);
                    Helper.NspaADCEventLog.WriteEntry(message, EventLogEntryType.Warning);
                    throw new Exception(message);
                }
            }

            return theCompany;
        }
    }
}
