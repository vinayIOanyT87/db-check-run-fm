using System;
using System.Collections.Generic;
using System.Collections;
using System.Data;
using System.Linq;
using System.ServiceModel;
using System.Web;
using System.Data.SqlClient;

using FMBusinessObjects.DataObjects;
using FMBusinessObjects.BusinessInterfaces;
using FMBusinessObjects.ServiceRequests;
using FMBusinessServices.DataAccessLayer;
using FMBusinessObjects.Exceptions;

namespace FMBusinessServices.ServiceClasses
{
	public class GetTransactionTypeProcessorClass : IGetTransactionTypeProcessor
	{
		#region Private data members
		private ConsolidatedDAClass consolidatedDA;
		#endregion

		#region Constructors
		public GetTransactionTypeProcessorClass ( )
		{
			this.consolidatedDA = new ConsolidatedDAClass ( );
		}
		#endregion

		#region Public methods
		public GetTransactionTypeDO Process ( GetTransactionTypeSR getTransactionTypeSR )
		{
			GetTransactionTypeDO getTransactionTypeDO = new GetTransactionTypeDO ( );

			this.GetTransactionTypeInfo ( getTransactionTypeSR.TransID, getTransactionTypeDO, getTransactionTypeSR.Security );

			return getTransactionTypeDO;
		}
		#endregion

		#region private methods
		private void GetTransactionTypeInfo ( string TransID, GetTransactionTypeDO getTransactionTypeDO, SecurityClass security )
		{
			const string PARAM_NAME_TRXID = "@TRXID";
			const SqlDbType PARAM_TYPE_TRXID = SqlDbType.NVarChar;
			const int PARAM_SIZE_TRXID = 64;


			DataSet dataSet = null;
			using (SqlCommand cmd = new SqlCommand())
			{
				cmd.CommandText = "SELECT LookupTransTypeIndex,DocumentNumber FROM tblTransactions " +
									DataObject.AddParameter(cmd, "WHERE", "TransID", "=", PARAM_NAME_TRXID, PARAM_TYPE_TRXID, PARAM_SIZE_TRXID, TransID);
				dataSet = this.consolidatedDA.GetDataSet( cmd, security);
			}

			if (dataSet.Tables[0].Rows.Count < 1)
			{
				throw new AccountingServicesException ( "TransactionID " + TransID + " does not exist" );
			}

			DataRow row = dataSet.Tables[0].Rows[0];

			getTransactionTypeDO.TransType = DataObject.getValue<TransactionTypes>(row["LookupTransTypeIndex"], TransactionTypes.T_Maximum);
			getTransactionTypeDO.DocumentNumber = DataObject.getValue<string>(row["DocumentNumber"], "");
			getTransactionTypeDO.TransID = TransID;
		}
		#endregion
	}
}