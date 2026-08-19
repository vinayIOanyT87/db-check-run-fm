using System;
using System.Collections;
using System.Text;

namespace PIDXTransactions
{
    public class FinishedRecord : PIDXRecordBase
    {
        #region Private attributes
        #endregion

        #region Constructor
        /// <summary>
        /// This is the default constructor for the finished record.
        /// </summary>
        public FinishedRecord()
        {
            base.TransactionType = PIDXConstants.FINISHED_PROCESSING;
        }
        #endregion

        #region properties
        #endregion

        #region Abstract method implementations
        /// <summary>
        /// This method implement the building of the finished processing record.
        /// </summary>
        /// <returns></returns>
        public override string GetDataRecord()
        {
            string fpRecord = "";
            fpRecord = fpRecord +
                       base.TransactionType;
            GenerateCheckBit(fpRecord);
            fpRecord += base.CheckDigit;

            return fpRecord;
        }

        /// <summary>
        /// This method implement that validation for the finished processing record.  It throws
        /// an exception if the validation fails.
        /// </summary>
        public override void ValidateRecord()
        {
        }
        #endregion
    }
}
