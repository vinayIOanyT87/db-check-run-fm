namespace FuelsManager.Areas.AccountingArea.ViewModels
{
	using FMBusinessObjects.DataObjects;

	public class TransactionFieldInfo
	{
		public string DisplayName { get; set; }

		public bool FieldRequired { get; set; }
		
		public bool ClearOnNew { get; set; }

		public bool ReadOnly { get; set; }

		public TransactionFieldInfo()
		{
		}

		public TransactionFieldInfo(FieldClass field)
		{
			this.DisplayName = field.DisplayName;
			this.ClearOnNew = field.ClearOnNew;
			this.FieldRequired = field.FieldRequired;
			this.ReadOnly = true;
		}
	}
}
