using System;

namespace StandardXMLImportExport
{
	/// <summary>
	/// Summary description for PrimaryRegradePopulater.
	/// </summary>
	public class PrimaryRegradePopulater : TransactionPopulater
	{
		public PrimaryRegradePopulater()
		{
		
		}

		protected override string TransactionTypeID
		{
			get
			{
				return "PrimaryRegrade";
			}
		}

		protected override void Populate()
		{
			this.SetConjoinedTransID();
		}

		protected override void PopulateLineItem()
		{

		}
	}
}
