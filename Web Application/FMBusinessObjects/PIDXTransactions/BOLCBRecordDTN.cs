 #pragma warning disable 1587
/// <summary>
/// File name:	BOLCBRecordDTN.cs
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
#pragma warning restore 1587

namespace FMBusinessObjects.PIDXTransactions
{
    using FMBusinessObjects.Constants;
    using FMBusinessObjects.Exceptions;
    using FMBusinessObjects.DataObjects;

    // ReSharper disable once InconsistentNaming
    public class BOLCBRecordDTN : BOLCBRecord
	{
		#region Private attributes
		#endregion

		#region Constructors
		/// <summary>
		/// This is the default constructor for the BOL complete record class.
		/// </summary>
		public BOLCBRecordDTN ( )
		{
			this.Initialize ( );
			this.TransactionType = PIDXConstants.COMPLETED_BOL;
		}
		#endregion

		#region Properties
		public override string ConsigneeNumber
		{
			get { return base.ConsigneeNumber.PadLeft(14, ' '); }
			set { base.ConsigneeNumber = value.Substring(0, (value.Length < 14) ? value.Length : 14); }
		}
		#endregion

		#region Abstract method implementations
		/// <summary>
		/// This method implement the building of the BOL build record.
		/// </summary>
		/// <returns></returns>
		public override string GetDataRecord(PIDXVersion version)
		{
			// Validate the fields to ensure that exist and are the appropriate length.
			this.ValidateRecord();

			string bolRecord = "";
			bolRecord = bolRecord + this.TransactionType + this.SPLCCode + this.TerminalOperator + this.SellerID + this.ConsigneeNumber + this.FinalShipperID + this.CarrierID + this.TruckNumber + this.BOLNumber + this.ShippedDate;

			// Add each defined bolbbproduct to the BB
		    // ReSharper disable once ForCanBeConvertedToForeach
			for (int count = 0; count < this.ProductArrayList.Count; count++)
			{
				BOLCBProduct cbproduct = (BOLCBProduct)this.ProductArrayList[count];

				bolRecord +=
				cbproduct.ProductCode +
				cbproduct.BlendOrAlterationIndicator +
				cbproduct.Gross +
				cbproduct.NetTemperature +
				cbproduct.NetTemperatureFlag +
				cbproduct.CreditIndicator;
			}

		    this.GenerateCheckBit ( bolRecord );
			bolRecord += this.CheckDigit;

			return bolRecord;
		}

		/// <summary>
		/// This method implement that validation for the BOL Build record.  It throws
		/// an exception if the validation fails.
		/// </summary>
		public override void ValidateRecord ( )
		{
			this.ValidateSpecific ( );

			if (( string.IsNullOrEmpty(base.ConsigneeNumber) ) ||
				( base.ConsigneeNumber.Length > PIDXConstants.CONSIGNEE_ID_LENGTH ) ||
				( base.ConsigneeNumber.Length < 1 ))
			{
				throw new PIDXException ( PIDXConstants.ERR_MSG_008 );
			}
		}
		#endregion

		#region Private methods
		/// <summary>
		/// This method initializes the object to its initial state.
		/// </summary>
		private void Initialize ( )
		{
		}
		#endregion
	}
}