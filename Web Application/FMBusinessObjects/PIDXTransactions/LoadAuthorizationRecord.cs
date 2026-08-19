#pragma warning disable 1587
/// <summary>
/// File name:	LoadAuthorizationRecord.cs
/// Purpose:	
///	Comments:	Copyright (C) Varec, Inc. Norcross, GA, USA, 
///				2011.  This file shall not be copied or reproduced in any form 
///				without the express written consent of Varec, Inc.
///	Author(s):	Warren Gray
///	Version:	1.0.0  Current version
///	
///	Modification History:
///		Date:		By:				Reason:
///		----------	-------------	-------------------------------------------
///		
/// </summary>
/// 
#pragma warning restore 1587

namespace FMBusinessObjects.PIDXTransactions
{
    using System.Diagnostics.CodeAnalysis;

    using FMBusinessObjects.Constants;
    using FMBusinessObjects.DataObjects;
    using FMBusinessObjects.Exceptions;

    public class LoadAuthorizationRecord : PIDXRecordBase
	{
		#region Constructors
		/// <summary>
		/// Initializes a new instance of the <see cref="LoadAuthorizationRecord"/> class. 
		/// This is the default constructor for the credit record class.
		/// </summary>
		public LoadAuthorizationRecord()
		{
			this.TransactionType = PIDXConstants.LOAD_AUTHORIZATION;
		}
		#endregion

		#region Abstract method implementations
		/// <summary>
		/// This method implement the building of the credit authorization record.
		/// </summary>
		/// <param name="version">
		/// PIDX version to build the record for.
		/// </param>
		/// <returns>
		/// Properly formatted PIDX record
		/// </returns>
		[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1305:FieldNamesMustNotUseHungarianNotation", Justification = "Reviewed. Suppression is OK here.")]
		public override string GetDataRecord(PIDXVersion version)
		{
			// Validate the fields to ensure that exist and are the appropriate length.
			this.ValidateRecord();

		    string caRecord = this.TransactionType +
		                      PIDXProfileClass.VersionID(version) +
		                      this.SPLCCode.Substring(0, 6) +
		                      this.TerminalOperator.Substring(0, 3) +
		                      this.SellerID.Substring(0, 3) +
		                      this.ConsigneeNumber.Substring(0, 14) +
		                      this.FinalShipperID.Substring(0, 3) +
		                      this.CarrierID.Substring(0, 8) +
		                      this.RackDriverID.Substring(0, 20) +
		                      this.TerminalControlNumber.Substring(0, 9) +
		                      this.ReleaseOrderNumber.Substring(0, 16);

			this.GenerateCheckBit(caRecord);
			caRecord += this.CheckDigit;

            // We are only supposed to send either CheckDigit37 or CheckDigit16,
            // not both. We'll send only CheckDigit37
            // Per TDS 4/27/2016, the CheckDigit16, if not used, should be space-filled
            //// caRecord += CRC16(caRecord,caRecord.Length-1);
            caRecord += "    ";

			return caRecord;
		}

		/// <summary>
		/// This method implement that validation for the check and order record.  It throws
		/// an exception if the validation fails.
		/// </summary>
		public override void ValidateRecord()
		{
			this.Validate();

			if (string.IsNullOrEmpty(this.ConsigneeNumber) ||
				 (this.ConsigneeNumber.Length != PIDXConstants.CONSIGNEE_ID_LENGTH))
			{
				throw new PIDXException(PIDXConstants.ERR_MSG_005);
			}
		}
		#endregion
	}
}
