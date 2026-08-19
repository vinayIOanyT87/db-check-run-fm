// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SaveTransactionsSR.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the SaveTransactionsSR type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FMBusinessObjects.ServiceRequests
{
    using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Globalization;
	using System.Runtime.Serialization;

	using FMBusinessObjects.DataObjects;

	/// <summary>
	/// The save transactions Service Request.
	/// </summary>
    [Serializable]
    [DataContract]
	[KnownType(typeof(TransactionDO))]
	[KnownType(typeof(LineItemDO))]
	[KnownType(typeof(GregorianCalendar))]
	public class SaveTransactionsSR : AccountingServiceRequest
	{
		/// <summary>
		/// The save transaction sub type.
		/// </summary>
		public enum SaveTransactionSubType
		{
			SaveTransactions,
			SaveTranactionFlagsAndStatus
		};

		/// <summary>
		/// Gets or sets the operator.
		/// </summary>
		[DataMember]
		public PersonClass Operator { get; set; }

		/// <summary>
		/// Gets or sets the transactions.
		/// </summary>
		[DataMember]
		public List<TransactionDO> Transactions { get; set; }

		/// <summary>
		/// Gets or sets the transactions.  This property may be populated with old versions of transactions to bypass the 
		/// transaction lookup that is performed whenever we save a transaction.
		/// You should only use this after considering whether you can be sure 
		/// that the version of the transaction you're providing is the most recent version.
		/// For example, it would probably not be wise to use this from the transaction detail form 
		/// as someone may update the transaction while you are viewing it on the transaction detail form,
		/// and then what you think is the most recent version of the transaction is really not the most recent version.
		/// </summary>
		[DataMember]
		public List<TransactionDO> OldTransactions { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether convert units.
		/// </summary>
		[DataMember]
		public bool ConvertUnits { get; set; }

		/// <summary>
		/// Gets or sets a value. This property was added to replace some logic which 
		/// used the Individual Database Transaction property, which was removed. 
		/// The auto complete methods in Save Transactions Processor returned null if 
		/// Individual Database Transaction was false. If you want to use autocomplete and not have the
		/// methods in Save Transaction sProcessor return null, set this property to true
		/// </summary>
		[DataMember]
		public bool UseAutoComplete { get; set; }

		/// <summary>
		/// Gets or sets the accounting site.
		/// </summary>
		[DataMember]
		public AccountingSite AccountingSite { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether individual database transaction.
		/// </summary>
		[DataMember]
		public bool IndividualDbTransaction { get; set; }

		/// <summary>
		/// If true, transaction validation will be bypassed
		/// </summary>
		[DataMember]
		public bool BypassValidation { get; set; }

		/// <summary>
		/// Gets or sets the sub type.
		/// </summary>
		[DataMember]
		public SaveTransactionSubType SubType { get; set; }

		/// <summary>
		/// Gets or sets the Force Trans Version Update flag.
		/// </summary>
		[DataMember]
		public bool ForceTransVersionUpdate { get; set; }

		/// <summary>
		/// The trans flags and status collection.
		/// </summary>
		[DataMember]
		private List<TransactionFlagsAndStatusDO> transFlagsAndStatusCollection;

        /// <summary>
        /// Gets or sets the Force New PIDX record flag.
        /// 
        /// Needed because merge on TransactionPIDX table will find the existing record if one without an
        /// authorization number exists.  We need to explicitly force a new insert.
        /// </summary>
        [DataMember]
        public bool ForceNewPidxRecord { get; set; }

        /// <summary>
        /// Gets or sets the Create Missing Reversal Pieces
        /// 
        /// When true, the save transaction processor will create or update the transactions
        /// related to this one via the reversed transaction id.  This includes the update, the original,
        /// and the reversal or reversal/update
        /// </summary>
        [DataMember]
        public bool CreateMissingReversalPieces { get; set; }

        /// <summary>
        /// Indicate if load is from Load Rack
        /// 
        /// This flag is created to determine if Alarm and Event Log update event will be triggered based on
        /// from load rack or web/manual BOL
        /// </summary>
        [DataMember]
        public bool BOLFromLoadRackFlag { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SaveTransactionsSR"/> class.
        /// </summary>
        public SaveTransactionsSR()
		{
			this.Transactions					= new List<TransactionDO>();
			this.OldTransactions				= new List<TransactionDO>();
			this.ConvertUnits					= true;
			this.UseAutoComplete				= false;
			this.AccountingSite					= null;
			this.IndividualDbTransaction		= false;
			this.SubType						= SaveTransactionSubType.SaveTransactions;
			this.transFlagsAndStatusCollection	= new List<TransactionFlagsAndStatusDO>();
			this.BypassValidation				= false;
			this.ForceTransVersionUpdate		= false;
            this.ForceNewPidxRecord             = false;
            this.CreateMissingReversalPieces    = true;
            this.BOLFromLoadRackFlag            = false;
		}

		/// <summary>
		/// Gets or sets the trans flags and status collection.
		/// </summary>
		public List<TransactionFlagsAndStatusDO> TransFlagsAndStatusCollection => this.transFlagsAndStatusCollection;
	}
}
