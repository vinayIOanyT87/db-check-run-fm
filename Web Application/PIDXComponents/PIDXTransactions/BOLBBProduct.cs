using System;
using System.Collections;
using System.Text;

namespace PIDXTransactions
{
    public class BOLBBProduct : BOLProductBase
    {
        #region Constructors
        /// <summary>
        /// This is the default constructor for the BOL BB Product class.
        /// </summary>
        public BOLBBProduct()
        {
            this.Initialize();
        }
        #endregion

        #region Abstract method implementations
        /// <summary>
        /// This method implement that validation for the BOL BB product. It throws
        /// an exception if the validation fails.
        /// </summary>
        public override void ValidateProduct()
        {
            base.Validate();
        }
        #endregion

        #region Private methods
        /// <summary>
        /// This method initializes the object to its initial state.
        /// </summary>
        private void Initialize()
        {
        }
        #endregion
    }
}