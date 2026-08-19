namespace FMBusinessObjects.PIDXTransactions
{
    using FMBusinessObjects.Constants;

    // ReSharper disable once InconsistentNaming
    public class BOLBLRecordDTN : BOLBLRecord
	{
		#region Private attributes
		#endregion

		#region Constructors
		/// <summary>
		/// This is the default constructor for the BOL build record class.
		/// </summary>
		public BOLBLRecordDTN()
		{
			this.Initialize();
			this.TransactionType = PIDXConstants.BOL_BL;
		}
		#endregion

		#region Properties
		public override string ConsigneeNumber
		{
			get { return base.ConsigneeNumber.PadLeft(14, ' '); }
			set { base.ConsigneeNumber = value.Substring(0, (value.Length < 14) ? value.Length : 14); }
		}
		#endregion

		#region Private methods
		/// <summary>
		/// This method initializes the object to its initial state.
		/// </summary>
		private void Initialize()
		{
		}
		#endregion


	}
}
