namespace TransactionFields
{
	using FMBusinessObjects.DataObjects;

	public class VolumeUnitFG : EngUnitFG
	{
		public VolumeUnitFG()
		{
		}

		public override string FieldID
		{
			get
			{
				return "VolumeUnit";
			}
		}

		public override object GetDataValue(TransactionDO transaction)
		{
			return GetUnitAsAbbrevString(transaction.VolumeUnits);
		}
	}
}
