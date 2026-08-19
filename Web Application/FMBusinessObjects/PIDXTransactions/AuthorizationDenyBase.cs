namespace FMBusinessObjects.PIDXTransactions
{
    using System;
    using System.Collections;

    using FMBusinessObjects.Constants;
    using FMBusinessObjects.Exceptions;

	public abstract class AuthorizationDenyBase : PIDXAuthorizationBase
	{
		#region Private attributes
		private Hashtable denyReasonList;
		private string denyReasonCode;
		private string denyReason;

	    private string terminatingString;
		#endregion

		#region Constructor
		/// <summary>
		/// This is the default constructor for the authorization deny class.
		/// </summary>
		protected AuthorizationDenyBase ( )
		{
			this.Initialize ( );
		}
		#endregion

		#region Properties
		public string DenyReasonCode
		{
			get { return this.denyReasonCode; }
			set { this.denyReasonCode=value; }
		}

		public string DenyReason
		{
			get { return this.denyReason; }
			set { this.denyReason=value; }
		}

		public Hashtable DenyReasonList
		{
			get { return this.denyReasonList; }
			set { this.denyReasonList = value; }
		}


		public string SellerID { get; set; }

	    public string TerminatingString
		{
			get { return this.terminatingString; }
			set { this.terminatingString = value; }
		}
		#endregion

		#region Private methods
		/// <summary>
		/// This method initializes the object to its intial state.
		/// </summary>
		private void Initialize ( )
		{
			this.denyReasonCode = "";
			this.denyReason = "";
			this.terminatingString = null;

		    this.denyReasonList = new Hashtable
		                          {
		                              { "01", "Invalid Seller ID" },
		                              { "02", "Invalid Shipper ID" },
		                              { "03", "Invalid Truck Number" },
		                              { "04", "Invalid Seller/Terminal Combination" },
		                              { "05", "Invalid Customer ID" },
		                              { "06", "Deny-No Check (Option 2)" },
		                              { "07", "No Credit Record (Option 3 and 6)" },
		                              { "08", "No Product Record (Option 5 and 6)" },
		                              { "09", "Credit Limit Exceeded" },
		                              { "10", "Prouct Allocation Exhausted" },
		                              { "11", "Pre-authorization Already Used (Option 4)" },
		                              { "12", "Invalid Transaction" },
		                              { "13", "Invalid Transaction Length" },
		                              { "14", "Incorrect consignee on the CO record" },
		                              { "49", "Account credit Lockout - Toptech" },
		                              { "51", "Invalid order/order record not found" },
		                              { "52", "Order was cancelled" },
		                              { "53", "Order was already completed" },
		                              { "54", "Order is already active" },
		                              { "55", "Invalid order status" },
		                              { "59", "Customer credit risk lockout - Toptech" },
		                              { "69", "Stockholder credit risk lockout - Toptech" },
		                              { "79", "Consignee credit risk lockout - Toptech" },
		                              { "80", "Retransmission Count Exceeded" },
		                              { "81", "Session error count exceeded" },
		                              { "94", "Account locked out - Toptech" },
		                              { "95", "Customer locked out - Toptech" },
		                              { "96", "Stockholder locked out - Toptech" },
		                              { "97", "Consignee locked out - Toptech" },
		                              { "98", "System Temporary Unaviable" },
		                              { "99", "TABS System error" },
		                              { "001", "Invalid Seller ID" },
		                              { "002", "Invalid Shipper ID" },
		                              { "003", "Invalid Truck Number" },
		                              { "004", "Invalid Seller/Terminal Combination" },
		                              { "005", "Invalid Customer ID" },
		                              { "006", "Deny-No Check (Option 2)" },
		                              { "007", "No Credit Record (Option 3 and 6)" },
		                              { "008", "No Product Record (Option 5 and 6)" },
		                              { "009", "Credit Limit Exceeded" },
		                              { "010", "Prouct Allocation Exhausted" },
		                              { "011", "Pre-authorization Already Used (Option 4)" },
		                              { "012", "Invalid Transaction" },
		                              { "013", "Invalid Transaction Length" },
		                              { "014", "Incorrect consignee on the CO record" },
		                              { "049", "Account credit Lockout - Toptech" },
		                              { "051", "Invalid order/order record not found" },
		                              { "052", "Order was cancelled" },
		                              { "053", "Order was already completed" },
		                              { "054", "Order is already active" },
		                              { "055", "Invalid order status" },
		                              { "059", "Customer credit risk lockout - Toptech" },
		                              { "069", "Stockholder credit risk lockout - Toptech" },
		                              { "079", "Consignee credit risk lockout - Toptech" },
		                              { "080", "Retransmission Count Exceeded" },
		                              { "081", "Session error count exceeded" },
		                              { "094", "Account locked out - Toptech" },
		                              { "095", "Customer locked out - Toptech" },
		                              { "096", "Stockholder locked out - Toptech" },
		                              { "097", "Consignee locked out - Toptech" },
		                              { "098", "System Temporarily Unavailable" },
		                              { "099", "TABS System error" }
		                          };
		}
		#endregion

		#region abstract methods
		public abstract void Parse(string response);

		#endregion
	}
}