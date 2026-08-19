///***************************************************************************
/// Module Name:  TransactionSR.cs
/// Author:       
/// Copyright (c) Varec, Inc.  All rights reserved.
///***************************************************************************

namespace FMBusinessObjects.ServiceRequests
{
	using System;
	using System.Runtime.Serialization;
	using FMBusinessObjects.DataObjects;

	/// <summary>
	/// Service request for use with ITransactionsProcessor
	/// </summary>
    [Serializable]
	[DataContract]
	[KnownType(typeof(AccountingSite))]
	public class TransactionSR : AccountingServiceRequest
	{
		#region Constants and Fields

		[DataMember]
		public AccountingSite AccountingSite = null;

		[DataMember]
		public bool ConvertUnits;

		/// <summary>
		/// You may optionally look up the transaction using the TransactionGuid instead of the TransID.
		/// </summary>
		[DataMember]
		public Guid TransactionGuid { get; set; }

		[DataMember]
		private string transID;

		[DataMember]
		private TransactionDO transaction;

		#endregion

		#region Constructors and Destructors

		/// <summary>
		/// Initializes a new instance of the <see cref="TransactionSR"/> class.
		/// </summary>
		public TransactionSR()
		{
			this.ConvertUnits = true;
			this.TransactionGuid = Guid.Empty;
		    this.GetHeaderOnly = false;
		}

		#endregion

		#region Public Properties

		[DataMember]
		public bool AllowCrossSiteTransactions { get; set; }

		public string TransID
		{
			get
			{
				return this.transID;
			}

			set
			{
				this.transID = value;
			}
		}

		public TransactionDO Transaction
		{
			get
			{
				return this.transaction;
			}

			set
			{
				this.transaction = value;
			}
		}

        [DataMember]
        public bool GetHeaderOnly { get; set; }
		#endregion
	}
}