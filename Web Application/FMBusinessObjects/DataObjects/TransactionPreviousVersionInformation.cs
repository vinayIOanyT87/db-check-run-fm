namespace FMBusinessObjects.DataObjects
{
	using System;
	using System.Collections.Generic;
    using System.Runtime.Serialization;

    /// <summary>
    /// Represents the previous (existing) version of the transaction in the database.
    /// This object only contains information needed by the save transactions processor when it is 
    /// modifying transactions. 
    /// </summary>
    [DataContract]
    public class TransactionPreviousVersionInformation
    {
        /// <summary>
        /// Does the existing version of the transaction have any weight readings?
        /// </summary>
        [DataMember]
        public bool HasWeightReadings { get; set; }

        /// <summary>
        /// Is the existing transaction deleted?
        /// </summary>
        [DataMember]
        public bool DeleteFlag { get; set; }

        /// <summary>
        /// The status of the existing version of the transaction
        /// </summary>
        [DataMember]
        public TransactionStatus Status { get; set; }

        /// <summary>
        /// The TransVersion (an automatically incremented number) of the existing transaction
        /// </summary>
        [DataMember]
        public long TransVersion { get; set; }

		/// <summary>
		/// The transaction inventory date.
		/// </summary>
		[DataMember]
		public DateTime InventoryDate { get; set; }

        /// <summary>
        /// The transaction links of the existing transaction. The existing links are needed by the save transactions processor
        /// when saving transaction links to determine whether it needs to create or delete links.
        /// </summary>
        [DataMember]
        public List<AssociatedTxDO> AssociatedTransactions { get; set; }
    }
}
