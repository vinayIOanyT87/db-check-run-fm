using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

using FMBusinessObjects.DataObjects;

namespace FMBusinessObjects.ServiceRequests
{
    [Serializable]
    [DataContract]
	public class TransactionPIDXSR : AccountingServiceRequest
	{
		#region public data members
		public enum PIDX_REQUEST_TYPES { GET_PIDX_BOL, UPDATE_SENT, DELETE_PIDX, GET_PIDX_TRANS, NONE };
		#endregion

		#region private data members
		[DataMember]
		private PIDX_REQUEST_TYPES pidxRequestType;
		[DataMember]
		private Guid transactionGuid;
		[DataMember]
		private TransactionPIDXCollectionDO transPidxDOList;
		[DataMember]
		private TransactionPIDXDO tranPidxDO;
		#endregion

		#region Constructors
		/// <summary>
		/// This is the default constructor for the transaction PIDX service request.
		/// </summary>
		public TransactionPIDXSR()
		{
			this.Initialize();
		}
		#endregion

		#region properties
		[DataMember]
		public TransactionPIDXCollectionDO TransactionPidxDOCollection
		{
			get { return this.transPidxDOList; }
			set { this.transPidxDOList = value; }
		}

		[DataMember]
		public TransactionPIDXSR.PIDX_REQUEST_TYPES PIDXRequestType
		{
			get { return this.pidxRequestType; }
			set { this.pidxRequestType = value; }
		}

		[DataMember]
		public TransactionPIDXDO TransPIDXDO
		{
			get { return this.tranPidxDO; }
			set { this.tranPidxDO = value; }
		}

		[DataMember]
		public Guid TransactionGuid
		{
			get { return this.transactionGuid; }
			set { this.transactionGuid = value; }
		}
		#endregion

		#region Private methods
		/// <summary>
		/// This method will initialize the object to initial state.
		/// </summary>
		private void Initialize()
		{
			this.pidxRequestType = TransactionPIDXSR.PIDX_REQUEST_TYPES.NONE;
			this.transPidxDOList = new TransactionPIDXCollectionDO();
			this.tranPidxDO      = null;
			this.transactionGuid		= Guid.Empty;
		}
		#endregion
	}
}
