using System;
using System.Collections;
using System.Text;

namespace PIDXTransactions
{
    public class CreditAuthorizationRecord : PIDXRecordBase
    {
        #region Constructors
        /// <summary>
        /// This is the default constructor for the credit record class.
        /// </summary>
        public CreditAuthorizationRecord()
        {
            base.TransactionType = PIDXConstants.CREDIT_AUTHORIZATION;
        }
        #endregion

        #region Abstract method implementations
        /// <summary>
        /// This method implement the building of the credit authorization record.
        /// </summary>
        /// <returns></returns>
        public override string GetDataRecord()
        {
            // Validate the fields to ensure that exist and are the appropriate length.
            this.ValidateRecord();

            string CARecord = "";
            CARecord = CARecord +
                       base.TransactionType +
                       base.SPLCCode +
                       base.TerminalOperator +
                       base.SellerID +
                       base.ConsigneeNumber +
                       base.FinalShipperID +
                       base.CarrierID +
                       base.TruckNumber;
            GenerateCheckBit(CARecord);
            CARecord += base.CheckDigit;

            return CARecord;
        }

        /// <summary>
        /// This method implement that validation for the check and order record.  It throws
        /// an exception if the validation fails.
        /// </summary>
        public override void ValidateRecord()
        {
            base.Validate();

            if ((base.ConsigneeNumber == null) ||
                (base.ConsigneeNumber.Length != PIDXConstants.CONSIGNEE_ID_LENGTH))
            {
                throw new PIDXException(PIDXConstants.ERR_MSG_005);
            }
        }
        #endregion
    }
}
