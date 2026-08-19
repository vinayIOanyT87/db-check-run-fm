// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TransactionImportSR.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the TransactionImportSR type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FMBusinessObjects.ServiceRequests
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;

	using FMBusinessObjects.DataObjects;

	/// <summary>
	/// The transaction import Service Request.
	/// </summary>
    [Serializable]
    [DataContract]
    public class TransactionImportSR : AccountingServiceRequest
	{
		[DataMember] private AccountingSite accountingSite;
		[DataMember] private TransactionDO transactionDo;
		[DataMember] private bool convertUnits;
		[DataMember] private SecurityClass passedSecurity;
		[DataMember] private List<TransactionDO> transactionCollection;
		[DataMember] private bool individualDbTransaction;

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
        /// Initializes a new instance of the <see cref="TransactionImportSR"/> class.
        /// </summary>
        /// <param name="security">
        /// The security.
        /// </param>
        /// <param name="transactiondo">
        /// The transaction data object.
        /// </param>
        public TransactionImportSR( SecurityClass security, TransactionDO transactiondo )
        {
            this.passedSecurity = security;
            this.transactionDo = transactiondo;
			this.TransactionCollection = null;
            this.CreateMissingReversalPieces = true;
			this.individualDbTransaction	= false;
        }

		/// <summary>
		/// Initializes a new instance of the <see cref="TransactionImportSR"/> class.
		/// </summary>
		/// <param name="security">
		/// The security.
		/// </param>
		/// <param name="transactionDo">
		/// The transaction data object.
		/// </param>
		/// <param name="accountingSite">
		/// The accounting site.
		/// </param>
		/// <param name="convertUnits">
		/// The convert units.
		/// </param>
		public TransactionImportSR(SecurityClass security, TransactionDO transactionDo, AccountingSite accountingSite, bool convertUnits)
		{
			this.Security				= security;
			this.transactionDo			= transactionDo;
			this.accountingSite			= accountingSite;
			this.convertUnits			= convertUnits;
			this.TransactionCollection	= null;
		    this.CreateMissingReversalPieces = true;
		    this.BypassValidation = false;
			this.individualDbTransaction = false;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="TransactionImportSR"/> class.
		/// </summary>
		/// <param name="security">
		/// The security.
		/// </param>
		/// <param name="transList">
		/// The trans list.
		/// </param>
		/// <param name="accountingSite">
		/// The accounting site.
		/// </param>
		/// <param name="convertUnits">
		/// The convert units.
		/// </param>
		public TransactionImportSR(SecurityClass security, List<TransactionDO> transList, AccountingSite accountingSite, bool convertUnits)
		{
			this.Security				= security;
			this.transactionDo			= null;
			this.TransactionCollection	= transList;
			this.accountingSite			= accountingSite;
			this.convertUnits			= convertUnits;
		    this.CreateMissingReversalPieces = true;
		    this.BypassValidation = false;
			this.individualDbTransaction = false;
		}


		/// <summary>
		/// Gets or sets the transaction collection.
		/// </summary>
		public List<TransactionDO> TransactionCollection
		{
			get { return this.transactionCollection; }
			set { this.transactionCollection = value; }
		}

		/// <summary>
		/// Gets or sets the passed security.
		/// </summary>
		public SecurityClass PassedSecurity 
		{
			get { return this.passedSecurity; }
			set { this.passedSecurity = value; }
		}

		/// <summary>
		/// Gets or sets the accounting site.
		/// </summary>
		public AccountingSite AccountingSite
		{
			get { return this.accountingSite; }
			set { this.accountingSite = value; }
		}

		/// <summary>
		/// Gets or sets the transaction data object.
		/// </summary>
		public TransactionDO TransactionDO
		{
			get { return this.transactionDo; }
			set { this.transactionDo = value; }
		}

		/// <summary>
		/// Gets or sets a value indicating whether convert units.
		/// </summary>
		public bool ConvertUnits
		{
			get { return this.convertUnits; }
			set { this.convertUnits = value; }
		}

        [DataMember]
        public bool BypassValidation { get; set; }
		
		/// <summary>
		/// Gets or sets a value indicating whether convert units.
		/// </summary>
		public bool IndividualDbTransaction
		{
			get { return this.individualDbTransaction; }
			set { this.individualDbTransaction = value; }
		}
	}
}
