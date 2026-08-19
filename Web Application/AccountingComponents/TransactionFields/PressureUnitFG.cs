namespace TransactionFields
{
	using FMBusinessObjects.DataObjects;

	public class PressureUnitFG : EngUnitFG
	{
		public PressureUnitFG()
		{
		}

		public override string FieldID
		{
			get
			{
				return "PressureUnit";
			}
		}

		public override object GetDataValue(TransactionDO transaction)
		{
			return GetUnitAsAbbrevString(transaction.PressureUnits);
		}
	}
}
