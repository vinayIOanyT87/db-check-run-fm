namespace TransactionFields
{
	using FMBusinessObjects.DataObjects;

	/// <summary>
	/// Summary description for AutoCompleteFG.
	/// </summary>
	public class AutoCompleteFG : CheckBoxGenerator, IHeaderField
	{
		public AutoCompleteFG()
		{
		}

		public override string FieldID
		{ get { return "AutoComplete"; } }

		#region IHeaderField Members

		public object GetDataValue(TransactionDO transaction)
		{
			return transaction.AutoComplete;
		}

		public string GetDataText(TransactionDO transaction)
		{
			if (GetDataValue(transaction) != null)
			{
				return GetDataValue(transaction).ToString();
			}

			return null;
		}

		public void SetDataValue(TransactionDO transaction, object newValue)
		{
			if (newValue is bool)
			{
				trans.AutoComplete = (bool)newValue;
				this.SetNewValue((bool)newValue);
				OnFieldChanged();
			}
		}
		#endregion
	}
}
