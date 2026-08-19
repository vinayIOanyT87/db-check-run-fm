namespace LedgerCore
{
	using System;

	[System.Serializable]
	public class LRLedgerLineItemCollection : LRBaseCollections
	{
		#region Attributes
		private Guid siteGuid;
		private Guid productGuid;
		private Guid tankGuid;
		#endregion

		#region Constructors
		/// <summary>
		/// This is the default constructor for the Ledger Line Item
		/// Collection class.
		/// </summary>
		public LRLedgerLineItemCollection()
		{
			this.siteGuid = Guid.Empty;
			this.productGuid = Guid.Empty;
			this.tankGuid = Guid.Empty;
		}
		#endregion

		#region Public Properties
		/// <summary>
		/// This property gets and sets the Site Guid data
		/// member.
		/// </summary>
		public Guid SiteGuid
		{
			get { return this.siteGuid; }
			set { this.siteGuid = value; }
		}

		/// <summary>
		/// This property gets and sets the Product Guid data
		/// member.
		/// </summary>
		public Guid ProductGuid
		{
			get { return this.productGuid; }
			set { this.productGuid = value; }
		}

		/// <summary>
		/// This property gets and sets the Tank Guid data
		/// member.
		/// </summary>
		public Guid TankGuid
		{
			get { return this.tankGuid; }
			set { this.tankGuid = value; }
		}
		#endregion
	}
}