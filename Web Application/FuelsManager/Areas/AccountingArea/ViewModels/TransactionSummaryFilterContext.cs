namespace FuelsManager.Areas.AccountingArea.ViewModels
{
	using System;

	[Serializable]
	public class TransactionSummaryFilterContext
	{
		public string BeginDate { get; set; }

		public string EndDate { get; set; }

		public string FindText { get; set; }

		public string ShortDatePattern { get; set; }

		public string SelectedAlias { get; set; }

		public TransactionSummaryFilterContext(TransactionSummaryViewModel model)
		{
			this.BeginDate = model.BeginDate;
			this.EndDate = model.EndDate;
			this.FindText = model.FindText;
			this.ShortDatePattern = model.ShortDatePattern;
			this.SelectedAlias = model.SelectedAlias;
		}
	}
}
