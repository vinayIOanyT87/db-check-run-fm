namespace LedgerCore
{
	public class LRTransactionErrorDO
	{
		#region Private Attributes
		private string aliasName;
		private string inventoryDate;
		private int errorStatus;
		#endregion

		#region Constructors
		/// <summary>
		/// This is the default constructor for the Transaction Error Data Object class.
		/// </summary>
		public LRTransactionErrorDO()
		{
			this.aliasName = string.Empty;
			this.inventoryDate = string.Empty;
			this.errorStatus = 0;
		}
		#endregion

		#region Properties
		/// <summary>
		/// This properties gets and sets the alias name.
		/// </summary>
		public string AliasName
		{
			get { return this.aliasName; }
			set { this.aliasName = value; }
		}
		/// <summary>
		/// This properties gets and sets the inventory date.
		/// </summary>
		public string InventoryDate
		{
			get { return this.inventoryDate; }
			set { this.inventoryDate = value; }
		}
		/// <summary>
		/// This properties gets and sets the error status.
		/// </summary>
		public int ErrorStatus
		{
			get { return this.errorStatus; }
			set { this.errorStatus = value; }
		}
		#endregion
	}
}