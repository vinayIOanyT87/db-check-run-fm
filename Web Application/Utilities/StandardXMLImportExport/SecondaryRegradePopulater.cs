using System;

namespace StandardXMLImportExport
{
	/// <summary>
	/// Summary description for SecondaryRegradePopulater.
	/// </summary>
	public class SecondaryRegradePopulater : TransactionPopulater
	{
		public SecondaryRegradePopulater()
		{
			
		}

		protected override string TransactionTypeID
		{
			get
			{
				return "SecondaryRegrade";
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
