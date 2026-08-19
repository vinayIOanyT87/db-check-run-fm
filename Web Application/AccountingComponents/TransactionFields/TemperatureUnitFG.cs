namespace TransactionFields
{
	using FMBusinessObjects.DataObjects;

	public class TemperatureUnitFG : EngUnitFG
	{
		public TemperatureUnitFG()
		{
		}

		public override string FieldID
		{
			get
			{
				return "TemperatureUnit";
			}
		}

		public override object GetDataValue(TransactionDO transaction)
		{
			return GetUnitAsAbbrevString(transaction.TemperatureUnits);
		}
	}
}
