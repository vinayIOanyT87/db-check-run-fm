/// <summary>
/// File name:	CreditAuthorizationRecordDTN.cs
/// Purpose:	
///	Comments:	Copyright (C) Varec, Inc. Norcross, GA, USA, 
///				2008.  This file shall not be copied or reproduced in any form 
///				without the express written consent of Varec, Inc.
///	Author(s):	Ivan Orndorff
///	Version:	1.0.0  Current version
///	
///	Modification History:
///		Date:		By:				Reason:
///		----------	-------------	-------------------------------------------
///		19-Mar-08	I.Orndorff		1.0.0 - Initial Revision.
///		
/// </summary>
/// 

using System;
using System.Collections;
using System.Text;

namespace PIDXTransactions
{
    public class CreditAuthorizationRecordDTN : PIDXRecordBase
    {
        #region Constructors
        /// <summary>
        /// This is the default constructor for the credit record class.
        /// </summary>
        public CreditAuthorizationRecordDTN()
        {
            base.TransactionType = PIDXConstants.CREDIT_AUTHORIZATION;
        }
        #endregion

		#region Properties
		public override string ConsigneeNumber
		{
			get { return base.ConsigneeNumber; }
			set { base.ConsigneeNumber = value.PadLeft(14,' '); }
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
