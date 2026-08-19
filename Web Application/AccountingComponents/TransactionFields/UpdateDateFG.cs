namespace TransactionFields
{
	using System;
	using FMBusinessObjects.DataObjects;

	public class UpdatedDateFG : DateTimeGenerator, IHeaderField
	{
		#region Constructors
		/// <summary>
		/// This is the default constructor for the Updated Date field generator.
		/// </summary>
		public UpdatedDateFG()
		{
		}
		#endregion

		#region Properties
		/// <summary>
		/// This property will return the Field ID for the Updated Date field.
		/// </summary>
		public override string FieldID
		{
			get
			{
				return "UpdatedDate";
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

		#region IHeaderField Members
		/// <summary>
		/// This method will return the value of the Updated Date field.
		/// </summary>
		/// <param name="transaction"></param>
		/// <returns></returns>
		public object GetDataValue(TransactionDO transaction)
		{
			return transaction.UpdatedDate;
		}

		/// <summary>
		/// This method will return the value in a string format of the Updated Date field.
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
		/// This method will set the value in the data object. Since the updated date
		/// is read only, then this method will do nothing.
		/// </summary>
		/// <param name="transaction"></param>
		/// <param name="newValue"></param>
		public void SetDataValue(TransactionDO transaction, object newValue)
		{
			if (newValue != null)
			{
				transaction.UpdatedDate = (DateTimeOffset) newValue;
				OnFieldChanged();
			}
		}
		#endregion
	}
}