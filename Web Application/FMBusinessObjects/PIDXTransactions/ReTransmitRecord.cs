namespace FMBusinessObjects.PIDXTransactions
{
    using FMBusinessObjects.Constants;
    using FMBusinessObjects.DataObjects;

    public class ReTransmitRecord : PIDXRecordBase
	{
		#region Private attributs
		#endregion

		#region Constructor
		/// <summary>
		/// This is the default constructor for the re-transmit record.
		/// </summary>
		public ReTransmitRecord ( )
		{
			this.TransactionType = PIDXConstants.RE_TRANSMIT;
		}
		#endregion

		#region properties
		#endregion

		#region Abstract method implementations
		/// <summary>
		/// This method implement the building of the re-transmit record.
		/// </summary>
		/// <returns></returns>
		public override string GetDataRecord (PIDXVersion version)
		{
			// Validate the fields to ensure that exist and are the appropriate length.
			string reRecord = "";
			reRecord = reRecord +
					   this.TransactionType;
		    this.GenerateCheckBit ( reRecord );
			reRecord += this.CheckDigit;

			return reRecord;
		}

		/// <summary>
		/// This method implement that validation for the re-transmit record.  It throws
		/// an exception if the validation fails.
		/// </summary>
		public override void ValidateRecord ( )
		{
		}
		#endregion
	}
}