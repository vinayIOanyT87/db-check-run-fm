namespace FMBusinessObjects.ServiceRequests
{
    using System;
    using System.Collections;
    using System.Runtime.Serialization;

    using UtilityObjects;

    [Serializable]
    [DataContract]
	public class TransactionListSR : AccountingServiceRequest
	{
		#region Attributes
		[DataMember]
		private DateTime transactionDate;
		[DataMember]
		private string manager;
		[DataMember]
		private string product;
		[DataMember]
		private string owner;
		[DataMember]
		private bool showDeletedTrx;
		[DataMember]
		private ArrayList aliasNames;
		[DataMember] private string nominationKey;
		[DataMember] private BsmeLedgerDateType.DateProcessTypes dateType;
		#endregion

		#region Constructor
		/// <summary>
		/// This is the default constructor for the transaction list SR.
		/// </summary>
		public TransactionListSR()
		{
			this.aliasNames = new ArrayList();
			this.dateType = BsmeLedgerDateType.DateProcessTypes.ByInventoryDate;
		}
		#endregion

		#region Properties

		public ArrayList AliasNames => this.aliasNames;

        public DateTime TransactionDate
		{
			get { return this.transactionDate; }
			set {
			    this.transactionDate = value; }
		}

		public string Manager
		{
			get { return this.manager; }
			set {
			    this.manager = value; }
		}

		public string Product
		{
			get { return this.product; }
			set {
			    this.product = value; }
		}

		public string Owner
		{
			get { return this.owner; }
			set {
			    this.owner = value; }
		}

		public bool ShowDeletedTransactions
		{
			get { return this.showDeletedTrx; }
			set {
			    this.showDeletedTrx = value; }
		}

		public string NominationKey
		{
			get { return this.nominationKey; }
			set { this.nominationKey = value; }
		}

		public BsmeLedgerDateType.DateProcessTypes DateType
		{
			get { return this.dateType; }
			set { this.dateType = value; }
		}
		#endregion

		#region Public methods
		/// <summary>
		/// This method will clear the AliasNames array list.
		/// </summary>
		public void ClearAliasNames()
		{
		    this.aliasNames?.Clear();
		}

        /// <summary>
		/// This method will add alias names to the array list.
		/// </summary>
		/// <param name="aliasName"></param>
		public void AddAliasNames(string aliasName)
		{
			if (!string.IsNullOrEmpty(aliasName))
			{
				this.aliasNames.Add(aliasName);
			}
		}
		#endregion
	}
}
