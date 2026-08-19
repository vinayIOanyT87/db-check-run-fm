using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.ServiceModel;
using System.Web;
using System.Data.SqlClient;

using FMBusinessObjects.BusinessInterfaces;
using FMBusinessObjects.DataObjects;
using FMBusinessObjects.Constants;
using FMBusinessObjects.LogClient;
using FMBusinessObjects.ServiceRequests;

using FMBusinessServices.DataAccessLayer;

namespace FMBusinessServices.ServiceClasses
{
	public class TransactionPIDXProcessorClass : ITransactionPIDXProcessor
	{
		#region Private data members
		private const string TransactionPIDXProcessor_ERR_MSG_001 = "Could not update transactionPIDX record: ";
		private const string TransactionPIDXProcessor_ERR_MSG_002 = "Error in retrieving transaction PIDX queue information. ";
		private const string TransactionPIDXProcessor_ERR_MSG_003 = "Could not delete transaction PIDX record for TransID/Authorization: ";
		private ConsolidatedDAClass consolidatedDA;
		private Logger logger;
		#endregion

		#region Constructors
		public TransactionPIDXProcessorClass()
		{
			this.consolidatedDA = new ConsolidatedDAClass ( );
			this.logger = new Logger ( "Transaction PIDX Processor" );
		}
		#endregion

		/// <summary>
		/// This method is the entry point of the processor from the client.
		/// </summary>
		/// <param name="accountingSR"></param>
		/// <returns></returns>
		public TransactionPIDXCollectionDO Process( TransactionPIDXSR sr )
		{
			switch(sr.PIDXRequestType)
			{
				case TransactionPIDXSR.PIDX_REQUEST_TYPES.GET_PIDX_BOL:
					return this.GetPIDXBOLQueue(sr);

				case TransactionPIDXSR.PIDX_REQUEST_TYPES.GET_PIDX_TRANS:
					return this.GetPIDXTrans(sr);

				case TransactionPIDXSR.PIDX_REQUEST_TYPES.UPDATE_SENT:
					this.UpdateSentStatus(sr);
					return null;

				case TransactionPIDXSR.PIDX_REQUEST_TYPES.DELETE_PIDX:
					this.DeletePIDX(sr);
					return null;

				default:
					return null;
			}
		}

		/// <summary>
		/// This method will update the sent flag for each of the transaction PIDX authorization
		/// records that BOL were sent.
		/// </summary>
		/// <param name="sr"></param>
		private void UpdateSentStatus(TransactionPIDXSR sr)
		{
			foreach (TransactionPIDXDO pidxDO in sr.TransactionPidxDOCollection.TransactionPIDXDOList)
			{
				pidxDO.UpdatedBy = sr.Security.UserID;

				try
				{
					using (SqlCommand cmd = new SqlCommand())
					{
						pidxDO.UpdateSentStatusSQL(cmd);
						this.consolidatedDA.ExecuteQuery(sr.Security, cmd);
					}
				}
				catch (SqlException sqlException)
				{
					string msg = TransactionPIDXProcessor_ERR_MSG_001 + pidxDO.TransactionGuid + " : " + pidxDO.AuthorizationNumber +
								  ". " + sqlException.Message;
					throw new Exception(msg);
				}
			}
		}

		/// <summary>
		/// This method will delete a given transaction PIDX record from the database.
		/// </summary>
		/// <param name="sr"></param>
		private void DeletePIDX(TransactionPIDXSR sr)
		{
			TransactionPIDXDO pidxDO = sr.TransPIDXDO;

			if (pidxDO != null)
			{
				try
				{
					using (SqlCommand cmd = new SqlCommand())
					{
						pidxDO.DeletePIDXSQL(cmd);
						this.consolidatedDA.ExecuteQuery(sr.Security, cmd);
					}
				}
				catch (SqlException sqlException)
				{
					string msg = TransactionPIDXProcessor_ERR_MSG_003 + pidxDO.TransactionGuid + " : " + pidxDO.AuthorizationNumber +
								  ". " + sqlException.Message;
					throw new Exception(msg);
				}
			}
		}

		/// <summary>
		/// This method will return a data object that contains an array of
		/// transaction PIDX data object collection.
		/// </summary>
		/// <returns></returns>
		private TransactionPIDXCollectionDO GetPIDXBOLQueue( TransactionPIDXSR sr )
		{
			TransactionPIDXCollectionDO transPidxCollection = new TransactionPIDXCollectionDO();
			TransactionPIDXDO pidxDO = new TransactionPIDXDO();
			pidxDO.SiteGuid = sr.Security.SiteGuid;

			try
			{
				using (SqlCommand cmd = new SqlCommand())
				{
					pidxDO.GetNonSentRecordsSqlCmd(cmd);
					DataSet dataSet = this.consolidatedDA.GetDataSet(cmd, sr.Security);
					DataTable table = dataSet.Tables[0];

					foreach (System.Data.DataRow row in table.Rows)
					{
						TransactionPIDXDO transPidxDO = new TransactionPIDXDO();
						transPidxDO.LoadNonSentRecordsSQL(row);

						if (transPidxDO.BrokenBlend)
						{
							FMEventLog eventLog = new FMEventLog();
							eventLog.WriteEntry("Transaction PIDXDO has Broken Blend TransID = " + transPidxDO.TransactionGuid, FMEventLogEntryType.Error);
							continue;
						}

						transPidxCollection.Add(transPidxDO);
					}
				}
			}
			catch(Exception ex)
			{
				this.logger.Debug(TransactionPIDXProcessor_ERR_MSG_002 + ex);
				throw new Exception(TransactionPIDXProcessor_ERR_MSG_002 + ex);
			}

			return transPidxCollection;
		}

		/// <summary>
		/// This method will return a data object that contains an array of
		/// transaction PIDX data object for a requested TransactionGuid.
		/// </summary>
		/// <returns></returns>
		private TransactionPIDXCollectionDO GetPIDXTrans( TransactionPIDXSR sr )
		{
			TransactionPIDXCollectionDO transPidxCollection = new TransactionPIDXCollectionDO();
			TransactionPIDXDO pidxDO = new TransactionPIDXDO();
			pidxDO.SiteGuid = sr.Security.SiteGuid;
			pidxDO.TransactionGuid = sr.TransactionGuid;

			try
			{
				using (SqlCommand cmd = new SqlCommand())
				{
					pidxDO.GetTransRecordsSQL(cmd);
					DataSet dataSet = this.consolidatedDA.GetDataSet(cmd, sr.Security);
					System.Data.DataTable table = dataSet.Tables[0];

					foreach (System.Data.DataRow row in table.Rows)
					{
						TransactionPIDXDO transPidxDO = new TransactionPIDXDO();
						transPidxDO.LoadNonSentRecordsSQL(row);
						transPidxCollection.Add(transPidxDO);
					}
				}
			}
			catch(Exception ex)
			{
				this.logger.Debug(TransactionPIDXProcessor_ERR_MSG_002 + ex);
				throw new Exception(TransactionPIDXProcessor_ERR_MSG_002 + ex);
			}

			return transPidxCollection;
		}

	}

}
