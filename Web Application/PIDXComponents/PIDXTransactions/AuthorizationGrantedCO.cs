using System;
using System.Collections;
using System.Text;

namespace PIDXTransactions
{
    public class AuthorizationGrantedCO : AuthorizationGrantedBase
    {
        #region Private attributes
        private Hashtable productLiftAmountList;
        private ArrayList productGroupList;
        #endregion

        #region Constructor
        /// <summary>
        /// This is the default constructor for the authorization granted CO class.
        /// </summary>
        public AuthorizationGrantedCO()
        {
        }
        #endregion

        #region Properties
        public ArrayList ProductGroupList
        {
            get { return this.productGroupList; }
        }

        public bool HasProductGroups
        {
            get
            {
                if ((this.productGroupList == null) || (this.productGroupList.Count <= 0))
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }

        public Hashtable ProductLiftAmountList
        {
            get { return this.productLiftAmountList; }
        }

        public bool HasProductLiftAmounts
        {
            get
            {
                if ((this.productLiftAmountList == null) || (this.productLiftAmountList.Count <= 0))
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
        /// This method parses the CO response.
        /// </summary>
        /// <param name="response"></param>
        public override void Parse(string response)
        {
            int headerLength = base.ParseHeader(response);
            this.productGroupList = new ArrayList();
            this.productLiftAmountList = new Hashtable();

            int productLength = 113;

            if ((headerLength + productLength) < response.Length)
            {
                string productStr = response.Substring(headerLength, productLength);
                this.ParseProducts(productStr);
            }
            // raw response needed to check validity against check digit
            int respEnd = response.IndexOf("R?") - 1;
            ResponseNoCheckDigit = response.Substring(0, respEnd);

            this.ParseProductGroups(response);
        }
        #endregion

        #region Private methods
        /// <summary>
        /// This method parses the product groups from the response.
        /// </summary>
        /// <param name="response"></param>
        private void ParseProductGroups(string response)
        {
            int prodGrpStart = 152;
            int prodGrpEnd = response.IndexOf("R?") - 1;

            for (int nextProdGrp = 0; nextProdGrp < 12; nextProdGrp++)
            {
                if (prodGrpStart > prodGrpEnd)
                {
                    nextProdGrp = 13;
                    break;
                }

                string prodGrp = response.Substring(prodGrpStart, 1);
                this.productGroupList.Add(prodGrp);
                prodGrpStart += 2;
            }
        }

        /// <summary>
        /// This method will parse the product and product lift amounts.
        /// </summary>
        /// <param name="productStr"></param>
        private void ParseProducts(string productStr)
        {
            int prodStart = 31;
            int liftStart = 34;

            for (int nextProd = 0; nextProd < 12; nextProd++)
            {
                string productID  = productStr.Substring(prodStart, 3).Replace(" ", "");
                string liftAmount = productStr.Substring(liftStart, 6).Replace(" ", "");

                if ((productID.Length > 0) && (liftAmount.Length > 0))
                {
                    if (this.productLiftAmountList.Contains(productID) == false)
                    {
                        try
                        {
                            double prodLiftAmount = System.Convert.ToDouble(liftAmount);
                            this.productLiftAmountList.Add(productID, prodLiftAmount);
                        }
                        catch (System.InvalidCastException)
                        {
                            throw new PIDXException(PIDXConstants.ERR_MSG_023);
                        }
                    }
                }

                prodStart += 10;
                liftStart += 10;

                if (liftStart > productStr.Length)
                {
                    break;
                }
            }
        }
        #endregion
    }
}
