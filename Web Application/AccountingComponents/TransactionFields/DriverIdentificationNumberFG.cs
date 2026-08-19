namespace TransactionFields
{
	using FMBusinessObjects.DataObjects;

	/// <summary>
	/// Summary description for DriverIdentificationNumberFG.
	/// </summary>
	public class DriverIdentificationNumberFG : TextFieldGenerator, IHeaderField
	{
		public DriverIdentificationNumberFG()
		{
		}

		public override string FieldID
		{
			get { return "DriverIdentificationNumber"; }
		}

		/// <summary>
		/// This property will returned either a figured data length or the 
		/// default length of 50.
		/// </summary>
		protected override short MaxColumns
		{
			get { return this.GetFieldLength(FieldID, 50); }
		}

		#region IHeaderField Members

		object IHeaderField.GetDataValue(TransactionDO transaction)
		{
			return transaction.DriverIDNumber;
		}

		string IHeaderField.GetDataText(TransactionDO transaction)
		{
			if (((IHeaderField) this).GetDataValue(transaction) != null)
			{
				return ((IHeaderField) this).GetDataValue(transaction).ToString();
			}

			return null;
		}

		void IHeaderField.SetDataValue(TransactionDO transaction, object newValue)
		{
			transaction.DriverIDNumber = newValue as string;
			OnFieldChanged();
		}
		#endregion
	}
}
