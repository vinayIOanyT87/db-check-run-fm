namespace FuelsManager.Areas.AccountingArea.ViewModels
{
	using System.Collections.Generic;

	using FMBusinessObjects.DataObjects;

	public class TransactionEditorViewModel
	{
		public string AliasName { get; set; }

		public List<TransactionAliasFieldClass> Fields { get; set; }

		public bool IsCombineAvailable { get; set; }

		public bool IsDeleteAvailable { get; set; }

		public bool IsReverseAvailable { get; set; }

		public bool IsReverseUpdateAvailable { get; set; }

		public bool IsViewPrintableAvailable { get; set; }

		public string ShortDatePattern { get; set; }

		public string TimePattern { get; set; }

		public TransactionDO Transaction { get; set; }

		public LineItemDO LineItem { get; set; }

		public string TransactionId
		{
			get
			{
				if (this.Transaction != null)
				{
					return this.Transaction.TransID;
				}

				return string.Empty;
			}
		}

		public byte VolumeDecimalPlaces { get; set; }
	}
}
