using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using FMBusinessObjects.DataObjects;

namespace FMBusinessObjects.ServiceRequests
{
    [Serializable]
    [DataContract]
	public class IncomingTruckGetTxSR : AccountingServiceRequest
	{
		#region Constructors
		/// <summary>
		/// This is the default constructor for the incoming truck get transaction
		/// service request class.
		/// </summary>
		public IncomingTruckGetTxSR ()
		{
		}
		#endregion

		#region Properties
		[DataMember]
		public Guid IATAGuid 
		{ 
			get; 
			set; 
		}
		#endregion

		#region Public methods
		public SqlCommand GetSQL ( )
		{
			const string PARAM_NAME_TRANSTYPEID = "@TransTypeID";
			const SqlDbType PARAM_TYPE_TRANSTYPEID = SqlDbType.SmallInt;
			const string PARAM_NAME_TRXSTATUS = "@TrxStatus";
			const string PARAM_NAME_LineItemSTATUS = "@LineItemStatus";
			const SqlDbType PARAM_TYPE_STATUS = SqlDbType.Int;
			
			const string PARAM_NAME_STATIONGUID = "@StationGuid";
			const SqlDbType PARAM_TYPE_STATIONGuid = SqlDbType.UniqueIdentifier;

			SqlCommand cmd = new SqlCommand();
			string sql = "SELECT * " +
								"FROM tblTransactions TH LEFT OUTER JOIN tblTransactionLineItems TL  ON TH.TransactionGuid = TL.TransactionGuid " +
								"WHERE " +
								" TH.DeleteFlag = CAST(0 as bit) " +
								DataObject.AddParameter(cmd, false, "TH.LookupTransTypeIndex", PARAM_NAME_TRANSTYPEID, PARAM_TYPE_TRANSTYPEID, TransactionTypes.T25_Shipment) +
								DataObject.AddParameter(cmd, "AND", "TL.LookupTransactionStatusIndex", "<>", PARAM_NAME_TRXSTATUS, PARAM_TYPE_STATUS, 0) +
								DataObject.AddParameter(cmd, "AND", "TH.LookupTransactionStatusIndex", "<>", PARAM_NAME_LineItemSTATUS, PARAM_TYPE_STATUS, 0) +
								DataObject.AddParameter(cmd, true, "TH.FinalStationIATAGuid", PARAM_NAME_STATIONGUID, PARAM_TYPE_STATIONGuid, IATAGuid);
			cmd.CommandText = sql;
			
			return cmd;
		#endregion
		}
	}
}
