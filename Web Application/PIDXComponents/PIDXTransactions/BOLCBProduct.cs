using System;
using System.Collections;
using System.Text;

namespace PIDXTransactions
{
    public class BOLCBProduct : BOLProductBase
    {
        #region Private attributes
        private int creditIndicator;
        #endregion

        #region Constructors
        /// <summary>
        /// This is the default constructor for the BOL CB product class.
        /// </summary>
        public BOLCBProduct()
        {
            this.Initialize();
        }
        #endregion

        #region Properties
        public int CreditIndicatorDigit
        {
            get { return this.creditIndicator;  }
            set { this.creditIndicator = Math.Abs(value); }
        }

        public string CreditIndicator
        {
            get { return this.creditIndicator.ToString(); }
        }
        #endregion

        #region Abstract method implementations
        /// <summary>
        /// This method implement that validation for the BOL CB product. It throws
        /// an exception if the validation fails.
        /// </summary>
        public override void ValidateProduct()
        {
            base.Validate();

            if (this.creditIndicator == -99)
            {
                throw new PIDXException(PIDXConstants.ERR_MSG_018);
            }
        }
        #endregion

        #region Private methods
        /// <summary>
        /// This method initializes the object to its initial state.
        /// </summary>
        private void Initialize()
        {
            this.creditIndicator = -99;
        }
        #endregion
    }
}