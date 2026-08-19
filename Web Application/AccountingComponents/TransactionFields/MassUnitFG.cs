namespace TransactionFields
{
	using FMBusinessObjects.DataObjects;

	public class MassUnitFG : EngUnitFG
	{
		public MassUnitFG()
		{
		}

		public override string FieldID
		{
			get
			{
				return "MassUnit";
			}
		}

		public override object GetDataValue(TransactionDO transaction)
		{
			return GetUnitAsAbbrevString(transaction.MassUnits);
		}
	}
}
