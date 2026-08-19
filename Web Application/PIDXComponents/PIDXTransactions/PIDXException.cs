using System;
using System.Collections;
using System.Text;

namespace PIDXTransactions
{
    public class PIDXException : System.Exception
    {
        #region public attributes
        public enum ErrorTypes { CRITICAL, WARNING }
        #endregion

        #region private attributes
        private string errorMessage;
        private PIDXException.ErrorTypes errorType;
        #endregion

        #region Constructors
        /// <summary>
        /// This is the default constructor for the PIDX Exception class.
        /// </summary>
        /// <param name="errorMsg"></param>
        public PIDXException(string errorMsg)
        {
            this.errorMessage = errorMsg;
            this.errorType = PIDXException.ErrorTypes.CRITICAL;
        }

        /// <summary>
        /// This constructor allows the user to set the error type (critical or warning).
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="errType"></param>
        public PIDXException(string errorMsg, PIDXException.ErrorTypes errType)
        {
            this.errorMessage = errorMsg;
            this.errorType = errType;
        }
        #endregion

        #region properties
        public string ErrorMessage
        {
            get { return this.errorMessage; }
        }

        public PIDXException.ErrorTypes ErrorType
        {
            get { return this.errorType; }
        }
        #endregion
    }
}
