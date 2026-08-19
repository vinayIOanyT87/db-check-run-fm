namespace TransactionFields
{
	using FMBusinessObjects.DataObjects;

	/// <summary>
	/// Summary description for InternationalRouteFG.
	/// </summary>
	public class InternationalRouteFG : CheckBoxGenerator, IHeaderField
	{
		public InternationalRouteFG()
		{
		}

		public override string FieldID
		{
			get { return "InternationalRouteIndicator"; }
		}

		#region IHeaderField Members
		public object GetDataValue(TransactionDO transaction)
		{
			return transaction.RouteInfo.InternationalRouteIndicator;
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
				transaction.RouteInfo.InternationalRouteIndicator = (bool) newValue;
				this.SetNewValue((bool?)newValue);
				OnFieldChanged();
			}
		}
		#endregion
	}
}
