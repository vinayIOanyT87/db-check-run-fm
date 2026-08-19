namespace TransactionFields
{
	using FMBusinessObjects.DataObjects;

	public class LevelUnitFG : EngUnitFG
	{
		public LevelUnitFG()
		{
		}

		public override string FieldID
		{
			get { return "LevelUnit"; }
		}

		public override object GetDataValue(TransactionDO transaction)
		{
			return GetUnitAsAbbrevString(transaction.LevelUnits);
		}
	}
}
