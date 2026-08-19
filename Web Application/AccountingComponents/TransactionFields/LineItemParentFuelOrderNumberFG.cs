namespace TransactionFields
{
	using FMBusinessObjects.DataObjects;

	public class LineItemParentFuelOrderNumberFG : LineItemParentDocumentNumberFG
	{
		public LineItemParentFuelOrderNumberFG()
		{
			this.m_parentType = TransactionTypes.T18_SupplyOrder;
			virtualField = true;
		}

		public override string FieldID
		{
			get
			{
				return "LineItem ParentFuelOrderNumber";
			}
		}

		public override bool Editable
		{
			get
			{
				return false;
			}
		}
	}
}
