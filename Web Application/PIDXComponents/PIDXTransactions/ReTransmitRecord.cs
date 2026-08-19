using System;
using System.Collections;
using System.Text;

namespace PIDXTransactions
{
    public class ReTransmitRecord : PIDXRecordBase
    {
        #region Private attributs
        #endregion

        #region Constructor
        /// <summary>
        /// This is the default constructor for the re-transmit record.
        /// </summary>
        public ReTransmitRecord ()
        {
            base.TransactionType = PIDXConstants.RE_TRANSMIT;
        }
        #endregion

        #region properties
        #endregion

        #region Abstract method implementations
        /// <summary>
        /// This method implement the building of the re-transmit record.
        /// </summary>
        /// <returns></returns>
        public override string GetDataRecord()
        {
            // Validate the fields to ensure that exist and are the appropriate length.
            string reRecord = "";
            reRecord = reRecord +
                       base.TransactionType;
            GenerateCheckBit(reRecord);
            reRecord += base.CheckDigit;

            return reRecord;
        }

        /// <summary>
        /// This method implement that validation for the re-transmit record.  It throws
        /// an exception if the validation fails.
        /// </summary>
        public override void ValidateRecord()
        {
        }
        #endregion
    }
}
