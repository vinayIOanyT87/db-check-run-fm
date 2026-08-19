namespace FuelsManager.Areas.AccountingArea.ViewModels
{
	using System;
	using System.Collections.Generic;

	using FMBusinessObjects.DataObjects;

	[Serializable]
	public class TransactionEditorContext
	{
		public static string SessionKey = "TransactionEditorContext";

		public TransactionDO Transaction { get; set; }

		public List<TransactionAliasFieldClass> Fields { get; set; }

		public TransactionEditorContext(TransactionEditorViewModel model)
		{
			if (model == null)
			{
				throw new ArgumentNullException("model");
			}

			this.Transaction = model.Transaction;
			this.Fields = model.Fields;
		}
	}
}
