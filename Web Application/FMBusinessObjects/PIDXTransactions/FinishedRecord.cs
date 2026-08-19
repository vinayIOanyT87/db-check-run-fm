namespace FMBusinessObjects.PIDXTransactions
{
    using FMBusinessObjects.Constants;
    using FMBusinessObjects.DataObjects;

    public class FinishedRecord : PIDXRecordBase
	{
		#region Private attributes
		#endregion

		#region Constructor
		/// <summary>
		/// This is the default constructor for the finished record.
		/// </summary>
		public FinishedRecord ( )
		{
			this.TransactionType = PIDXConstants.FINISHED_PROCESSING;
		}
		#endregion

		#region properties
		#endregion

		#region Abstract method implementations
		/// <summary>
		/// This method implement the building of the finished processing record.
		/// </summary>
		/// <returns></returns>
		public override string GetDataRecord (PIDXVersion version)
		{
			string fpRecord = "";
			fpRecord = fpRecord +
					   this.TransactionType;
		    this.GenerateCheckBit ( fpRecord );
			fpRecord += this.CheckDigit;

			return fpRecord;
		}

		/// <summary>
		/// This method implement that validation for the finished processing record.  It throws
		/// an exception if the validation fails.
		/// </summary>
		public override void ValidateRecord ( )
		{
		}
		#endregion
	}
}