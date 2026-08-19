using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ADOFMSImport.DataObjects;
using ADOFMSImport.Parsers;
using ADOFMSImport.Transformers;
using Accounting;
using FMBusinessObjects.DataObjects;
using FMBusinessObjects.BusinessInterfaces;
using FMBusinessObjects.ChannelFactories;
using FMBusinessObjects.ServiceRequests;
using FMBusinessObjects.UtilityObjects;

namespace ADOFMSImport
{
	public class Runner
	{
		#region Constants
		public const string MESSAGE_CRITICAL = "Critical error, cannot continue.";
		public const string MESSAGE_SUCCESSFUL = "Finished successfully.";
		public const string MESSAGE_WITHERRORS = "Finished with errors.";
		#endregion // Constants

		public bool Run ( Defaults a_defaults )
		{
			bool result = false;

			try
			{
				TransactionDOCollection transCollection = new TransactionDOCollection ( );

				// read the file into a CSV file
				using (Parser fileReader = new CSVMultiParser
				   (
					  new IssuesObject ( a_defaults ),
					  new SalesObject ( a_defaults )
				   ))
				{
					// prepare security (move this when performing optimizations)
					SecurityClass security = new SecurityClass ( );
					foreach (RIGHT right in Enum.GetValues ( typeof ( RIGHT ) ))
					{
						security.RightCollection.Add ( right );
					}
					security.SiteID = "JFLA";
					security.SiteIndex = 3;
					security.UserID = "administrator";
					security.UserIndex = 1;
					security.LoginSiteGuid = BaseDataObject.DUMMY_GUID; //was "3"
					security.LoginSiteID = "JFLA";

					TransformerManager manager = new TransformerManager ( a_defaults );

					fileReader.Read ( a_defaults.InputFile );

					for (int i = 0; i < fileReader.GetDataObjectCount ( ); ++i)
					{
						using (CSVObject csvObject = fileReader.GetDataObject ( i ) as CSVObject)
						{
							// transform the csv object into transactions
							TransactionDOCollection localCollection = manager.Transform ( csvObject );
							if (localCollection != null)
							{
								foreach (TransactionDO trans in localCollection)
								{
									transCollection.Add ( trans );
								}
							}
						}
					}

					// write the transactions
					FMChannelFactory<ISites> sitesClient = new FMChannelFactory<ISites> ( );
					ISites siteLookup = sitesClient.CreateProxy ( );

					SaveTransactionsSR saveTransSr = new SaveTransactionsSR ( );
					saveTransSr.Security = security;

					// to ensure we have transaction/inventory dates as close to write date as possible,
					// we perform wac processing one by one (this is how saving lists of transactions 
					// work anyway)

					FMChannelFactory<IPriceCalculatorInvoker> invokerClient = new FMChannelFactory<IPriceCalculatorInvoker> ( );
					IPriceCalculatorInvoker invoker = invokerClient.CreateProxy ( );

					foreach (TransactionDO trans in transCollection)
					{
						// extra ID name
						int idIndex = trans.Notes.IndexOf ( "ID " ) + 3;
						string seg = trans.Notes.Substring ( idIndex );
						string idString = seg.Split ( ' ' )[0];

						try
						{
							security.SiteGuid = trans.SiteGuid;
							security.SiteID = trans.Site;
							SiteClass site = siteLookup.Get(security, trans.SiteGuid, false, false, false);
							SiteTimeConverter timeConverter = new SiteTimeConverter ( site );

							DateTime siteTime = timeConverter.ConvertToSiteTime ( DateTime.UtcNow );
							trans.TransactionDateTime = siteTime;
							trans.InventoryDate = siteTime;
							trans.SetVolumeSigns ( false );

							invoker.CalculateWithLineItems (security, trans, trans.LineItems);

							saveTransSr.Transactions.Clear ( );
							saveTransSr.Transactions.Add ( trans );
							saveTransSr.CurrentSiteGuid = trans.SiteGuid;

							FMChannelFactory<ISaveTransactionsProcessor> saveTxClient = new FMChannelFactory<ISaveTransactionsProcessor> ( );
							ISaveTransactionsProcessor saveTxProcessor = saveTxClient.CreateProxy ( );

							SaveTransactionsResultDO saveResult = saveTxProcessor.SaveTransactions ( saveTransSr );

							LoggerManager.LogProgress ( a_defaults.LoggerKey, "SUCCESS[" + idString + "] Write transaction for " + trans.Alias );
						}
						catch (Exception e)
						{
							LoggerManager.LogError ( a_defaults.LoggerKey, "FAILURE[" + idString + "] Write transaction for " + trans.Alias + " because: " + e.Message );
						}
					}

				}

				// finally write the transaction
			}
			catch (Exception e)
			{
				LoggerManager.LogError ( a_defaults.LoggerKey, e.Message );
				LoggerManager.LogError ( a_defaults.LoggerKey, MESSAGE_CRITICAL );
			}

			return result;
		}
	}
}
