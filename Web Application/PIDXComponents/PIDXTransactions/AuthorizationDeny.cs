using System;
using System.Collections;
using System.Text;

namespace PIDXTransactions
{
    public class AuthorizationDeny : PIDXAuthorizationBase
    {
        #region Private attributes
        private Hashtable denyReasonList;
        private string    denyReasonCode;
        private string    denyReason;
        private string    sellerID;
        private string    terminatingString;
        #endregion

        #region Constructor
        /// <summary>
        /// This is the default constructor for the authorization deny class.
        /// </summary>
        public AuthorizationDeny()
        {
            this.Initialize();
        }
        #endregion

        #region Properties
        public string DenyReasonCode
        {
            get { return this.denyReasonCode; }
        }

        public string DenyReason
        {
            get { return this.denyReason; }
        }

        public string SellerID
        {
            get { return this.sellerID; }
        }

        public string TerminatingString
        {
            get { return this.terminatingString; }
            set { this.terminatingString = value; }
        }
        #endregion

        #region Public methods
        /// <summary>
        /// This method parses the deny response.
        /// </summary>
        /// <param name="response"></param>
        public void Parse(string response)
        {
            if ((response != null) && (response.IndexOf("E!") >= 0))
            {
                throw new PIDXException(PIDXConstants.ERR_MSG_025, PIDXException.ErrorTypes.WARNING);
            }

            if ((response != null) && (response.Length >= 14))
            {
                base.ResponseType   = response.Substring(0, 4);
                this.sellerID       = response.Substring(7, 3);
                this.denyReasonCode = response.Substring(11, 2);
                this.denyReason     = (string)this.denyReasonList[this.denyReasonCode];

                int termIndex = response.IndexOf("R?");

                if (termIndex >= 0)
                {
                    this.terminatingString = response.Substring(termIndex);
                }
            }
            else
            {
                throw new PIDXException(PIDXConstants.ERR_MSG_024);
            }
        }
        #endregion

        #region Private methods
        /// <summary>
        /// This method initializes the object to its intial state.
        /// </summary>
        private void Initialize()
        {
            this.denyReasonCode    = "";
            this.denyReason        = "";
            this.terminatingString = null;

            this.denyReasonList = new Hashtable();
            this.denyReasonList.Add("01", "Invalid Seller ID");
            this.denyReasonList.Add("02", "Invalid Shipper ID");
            this.denyReasonList.Add("03", "Invalid Truck Number");
            this.denyReasonList.Add("04", "Invalid Seller/Terminal Combination");
            this.denyReasonList.Add("05", "Invalid Customer ID");
            this.denyReasonList.Add("06", "Deny-No Check (Option 2)");
            this.denyReasonList.Add("07", "No Credit Record (Option 3 and 6)");
            this.denyReasonList.Add("08", "No Product Record (Option 5 and 6)");
            this.denyReasonList.Add("09", "Credit Limit Exceeded");
            this.denyReasonList.Add("10", "Prouct Allocation Exhausted");
            this.denyReasonList.Add("11", "Pre-authorization Already Used (Option 4)");
            this.denyReasonList.Add("12", "Invalid Transaction");
            this.denyReasonList.Add("13", "Invalid Transaction Length");
            this.denyReasonList.Add("14", "Incorrect consignee on the CO record");
            this.denyReasonList.Add("49", "Account credit Lockout - Toptech");
            this.denyReasonList.Add("51", "Invalid order/order record not found");
            this.denyReasonList.Add("52", "Order was cancelled");
            this.denyReasonList.Add("53", "Order was already completed");
            this.denyReasonList.Add("54", "Order is already active");
            this.denyReasonList.Add("55", "Invalid order status");
            this.denyReasonList.Add("59", "Customer credit risk lockout - Toptech");
            this.denyReasonList.Add("69", "Stockholder credit risk lockout - Toptech");
            this.denyReasonList.Add("79", "Consignee credit risk lockout - Toptech");
            this.denyReasonList.Add("80", "Retransmission Count Exceeded");
            this.denyReasonList.Add("81", "Session error count exceeded");
            this.denyReasonList.Add("94", "Account locked out - Toptech");
            this.denyReasonList.Add("95", "Customer locked out - Toptech");
            this.denyReasonList.Add("96", "Stockholder locked out - Toptech");
            this.denyReasonList.Add("97", "Consignee locked out - Toptech");
            this.denyReasonList.Add("98", "System Temporary Unaviable");
            this.denyReasonList.Add("99", "TABS System error");
        }
        #endregion
    }
}
