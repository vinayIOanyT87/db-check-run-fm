namespace FuelsManager.Areas.AccountingArea.ViewModels
{
	using System.Collections.Generic;
	using System.Linq;
	using System.Web.Mvc;

	using FMBusinessObjects.DataObjects;

	public class TransactionSummaryViewModel
	{
		public string FindText { get; set; }

		public List<TransactionSummaryClass> Transactions { get; set; }

		public TransactionAliasNameCollectionClass TransactionAliasNames { get; set; }

		public Dictionary<string, string> ListViewAliasColumnNames { get; set; }

		public string ColumnDisplayNames { get; set; }

		public string SelectedAlias { get; set; }

		public bool DeleteEnabled { get; set; }

		public string ShortDatePattern { get; set; }

		public string TimePattern { get; set; }

		public byte VolumeDecimalPlaces { get; set; }

		public byte MassDecimalPlaces { get; set; }

		public string NowText { get; set; }

		public string BeginDate { get; set; }

		public string EndDate { get; set; }

		public int RecordCount { get; set; }

		public string AllOptionText { get; set; }

		public IEnumerable<SelectListItem> AliasNames
		{
			get
			{
				var allNames =
					this.TransactionAliasNames.Select(name => new SelectListItem() { Value = name.AliasName, Text = name.AliasName });

				return DefaultAliasName.Concat(allNames);
			}
		}

		public IEnumerable<SelectListItem> DefaultAliasName
		{
			get
			{
				return Enumerable.Repeat(new SelectListItem { Value = string.Empty, Text = AllOptionText }, count: 1);
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="TransactionSummaryViewModel"/> class.
		/// </summary>
		public TransactionSummaryViewModel()
		{
			this.Transactions = new List<TransactionSummaryClass>();
			this.TransactionAliasNames = new TransactionAliasNameCollectionClass();
			this.ListViewAliasColumnNames = new Dictionary<string, string>();
			this.ColumnDisplayNames = string.Empty;
			this.DeleteEnabled = false;
			this.AllOptionText = "{All}";
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="TransactionSummaryViewModel"/> class.
		/// </summary>
		/// <param name="context">The context to use to populate initial values.</param>
		public TransactionSummaryViewModel(TransactionSummaryFilterContext context)
		{
			this.Transactions = new List<TransactionSummaryClass>();
			this.TransactionAliasNames = new TransactionAliasNameCollectionClass();
			this.ListViewAliasColumnNames = new Dictionary<string, string>();
			this.ColumnDisplayNames = string.Empty;
			this.AllOptionText = "{All}";

			if (context != null)
			{
				this.BeginDate = context.BeginDate;
				this.EndDate = context.EndDate;
				this.FindText = context.FindText;
				this.ShortDatePattern = context.ShortDatePattern;
				this.SelectedAlias = context.SelectedAlias;
			}
		}
	}
}