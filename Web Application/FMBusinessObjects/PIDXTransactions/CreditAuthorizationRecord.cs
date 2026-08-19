namespace FMBusinessObjects.PIDXTransactions
{
    using FMBusinessObjects.Constants;
    using FMBusinessObjects.DataObjects;
    using FMBusinessObjects.Exceptions;

    public class CreditAuthorizationRecord : PIDXRecordBase
	{
		#region Constructors
		/// <summary>
		/// This is the default constructor for the credit record class.
		/// </summary>
		public CreditAuthorizationRecord ( )
		{
			this.TransactionType = PIDXConstants.CREDIT_AUTHORIZATION;
		}
		#endregion

		#region Abstract method implementations
		/// <summary>
		/// This method implement the building of the credit authorization record.
		/// </summary>
		/// <returns></returns>
		public override string GetDataRecord (PIDXVersion version)
		{
			// Validate the fields to ensure that exist and are the appropriate length.
			this.ValidateRecord ( );

			string caRecord = "";
			caRecord = caRecord +
					   this.TransactionType +
					   this.SPLCCode +
					   this.TerminalOperator +
					   this.SellerID +
					   this.ConsigneeNumber +
					   this.FinalShipperID +
					   this.CarrierID +
					   this.TruckNumber;
		    this.GenerateCheckBit ( caRecord );
			caRecord += this.CheckDigit;

			return caRecord;
		}

		/// <summary>
		/// This method implement that validation for the check and order record.  It throws
		/// an exception if the validation fails.
		/// </summary>
		public override void ValidateRecord ( )
		{
			this.Validate ( );

			if (( string.IsNullOrEmpty(this.ConsigneeNumber) ) ||
				( this.ConsigneeNumber.Length != PIDXConstants.CONSIGNEE_ID_LENGTH ))
			{
				throw new PIDXException ( PIDXConstants.ERR_MSG_005 );
			}
		}
		#endregion
	}
}