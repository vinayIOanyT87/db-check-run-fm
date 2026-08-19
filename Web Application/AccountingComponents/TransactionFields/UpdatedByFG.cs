namespace TransactionFields
{
	using FMBusinessObjects.DataObjects;

	public class UpdatedByFG : TextFieldGenerator, IHeaderField
	{
		#region Constructors
		/// <summary>
		/// This is the default constructor for the Updated By field generator.
		/// </summary>
		public UpdatedByFG()
		{
		}
		#endregion

		#region Properties
		/// <summary>
		/// This property will return the Field ID for the Updated By field.
		/// </summary>
		public override string FieldID
		{
			get
			{
				return "UpdatedBy";
			}
		}

		/// <summary>
		/// This property will returned either a configured data length or the 
		/// default length of 30.
		/// </summary>
		protected override short MaxColumns
		{
			get
			{
				return this.GetFieldLength(FieldID, 30);
			}
		}

		/// <summary>
		/// This property will return false indicating that the field is always
		/// read only.
		/// </summary>
		public override bool Editable
		{
			get
			{
				return false;
			}
		}
		#endregion

		#region Public methods
		/// <summary>
		/// This method will return the value of the Updated By field.
		/// </summary>
		/// <param name="transaction"></param>
		/// <returns></returns>
		public object GetDataValue(TransactionDO transaction)
		{
			return transaction.UpdatedBy;
		}

		/// <summary>
		/// This method will return the value in a string format of the Updated By field.
		/// </summary>
		/// <param name="transaction"></param>
		/// <returns></returns>
		public string GetDataText(TransactionDO transaction)
		{
			if (GetDataValue(transaction) != null)
			{
				return GetDataValue(transaction).ToString();
			}
			
			return null;
		}

		/// <summary>
		/// This method will set the value in the data object.
		/// </summary>
		/// <param name="transaction"></param>
		/// <param name="newValue"></param>
		public void SetDataValue(TransactionDO transaction, object newValue)
		{
			transaction.UpdatedBy = newValue as string;
			OnFieldChanged();
		}
		#endregion
	}
}