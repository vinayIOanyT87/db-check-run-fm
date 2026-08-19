namespace TransactionFields
{
	using FMBusinessObjects.DataObjects;

	/// <summary>
	/// Summary description for AGR_CompartmentID.
	/// </summary>
	public class AGR_CompartmentID : TextFieldGenerator, IWeightReadingField
	{
		public AGR_CompartmentID()
		{
		}

		public override string FieldID
		{
			get
			{
				return "AGR CompartmentID";
			}
		}

		/// <summary>
		/// This property will returned either a figured data length or the 
		/// default length of 30.
		/// </summary>
		protected override short MaxColumns
		{
			get { return this.GetFieldLength(FieldID, 30); }
		}

		#region IWeightReadingField Members
		public object GetDataValue(WeightReadingDO agr)
		{
			return agr.CompartmentName;
		}

		public string GetDataText(WeightReadingDO agr)
		{
			if (GetDataValue(agr) != null)
			{
				return GetDataValue(agr).ToString();
			}

			return null;
		}

		public void SetDataValue(WeightReadingDO agr, object newValue)
		{
			agr.CompartmentName = newValue as string;
			OnFieldChanged();
		}
		#endregion
	}
}
