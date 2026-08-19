using System;
using System.Collections;
using System.Text;

namespace PIDXTransactions
{
    public class AuthorizationGrantedCA : AuthorizationGrantedBase
    {
        #region Private attributes
        private ArrayList productIdentifierList;
        #endregion

        #region Constructor
        /// <summary>
        /// This is the default constructor for the authorization granted CA class.
        /// </summary>
        public AuthorizationGrantedCA()
        {
        }
        #endregion

        #region Properties
        public ArrayList ProductIdentifierList
        {
            get { return this.productIdentifierList; }
        }

        public bool HasProductIdentifiers
        {
            get
            {
                if ((this.productIdentifierList == null) || (this.productIdentifierList.Count <= 0))
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }
        #endregion

        #region Public override methods
        /// <summary>
        /// This method parses the CA response.
        /// </summary>
        /// <param name="response"></param>
        public override void Parse(string response)
        {
            int headerLength = base.ParseHeader(response);
            this.productIdentifierList = new ArrayList();

            int respEnd = response.IndexOf("R?") - 1;
            int productLength = respEnd - headerLength;

            if (productLength > 0)
            {
                string productIdentifiers = response.Substring(31, productLength);
                this.FindProductIndentifiers(productIdentifiers);
            }
            // raw response needed to check validity against check digit
            ResponseNoCheckDigit = response.Substring(0, respEnd);
        }
        #endregion

        #region Private methods
        /// <summary>
        /// This method will parse out the product identifiers
        /// </summary>
        /// <param name="productStr"></param>
        private void FindProductIndentifiers(string productStr)
        {
            string productIdent = productStr.Replace(" ", "");

            if ((productIdent != null) && (productIdent.Length > 0))
            {
                for (int nextChar = 0; nextChar < productIdent.Length; nextChar++)
                {
                    this.productIdentifierList.Add(productIdent.Substring(nextChar, 1));
                }
            }
        }
        #endregion
    }
}
