

namespace FMBusinessObjects.DataObjects
{
	using System.Collections.Generic;

	/// <summary>
	/// Data to populate TIA segments, sorted by date and BillOfLadingNumber
	/// </summary>
	public class ExStarsTaxInfoTransactionSumCollection : SortedDictionary<string, ExStarsTaxInfoSum>
	{
		/// <summary>
		/// If this key is new to the collection, insert the a copy of the transaction,  else add the gross and net values
		/// to the existing element.  
		/// </summary>
		/// <param name="key">The key format will vary depending on the transaction type</param>
		/// <param name="taxInfoSum">the object will be cloned so prevent leaking changes</param>
		public new void Add(string key, ExStarsTaxInfoSum taxInfoSum)
		{
			if (!this.ContainsKey(key))
			{
				base.Add(key, taxInfoSum.Clone());
				return;
			}
			this[key].NetReceiptVolume += taxInfoSum.NetReceiptVolume;
			this[key].GrossReceiptVolume += taxInfoSum.GrossReceiptVolume;
		}
	}

	/// <summary>
	/// The summed gross and total for a single day and BillOfLadingNumber
	/// </summary>
	public class ExStarsTaxInfoSum
	{
		public int ReportYear { get; protected set; }
		public int ReportMonth { get; protected set; }
		public int ReportDay { get; protected set; }
		public string BillOfLadingNumber { get; protected set; }
		public double GrossReceiptVolume { get; set; }
		public double NetReceiptVolume { get; set; }
		public double GrossHydrantDisbursement { get; set; }
		public double NetHydrantDisbursement { get; set; }
		public double GrossBrokerReceipts { get; set; }
		public double NetBrokerReceipts { get; set; }
		public double GrossBrokerIssues { get; set; }
		public double NetBrokerIssues { get; set; }

		public ExStarsTaxInfoSum(ExStarsBrokerTransferClass trx) 
			: this( trx.ReportYear, trx.ReportMonth, trx.ReportDay, "", trx.GrossVolume, trx.NetVolume){}

		public ExStarsTaxInfoSum(ExStarsTransactionClass trx, bool setNegative)
			: this(trx.ReportYear
			, trx.ReportMonth
			, trx.ReportDay
			, trx.BillOfLadingNumber
			, setNegative ? -trx.GrossVolume : trx.GrossVolume
			, setNegative ? -trx.NetVolume : trx.NetVolume) 
		{ }


		public ExStarsTaxInfoSum(int year, int month, int day, double grossVolume, double netVolume)
			: this(year, month, day, "", grossVolume, netVolume) { }

		public ExStarsTaxInfoSum(int year, int month, int day, string billOfLadingNumber, double grossReceiptVolume, double netReceiptVolume)
		{
			this.ReportYear = year;
			this.ReportMonth = month;
			this.ReportDay = day;
			this.BillOfLadingNumber = billOfLadingNumber;
			this.GrossReceiptVolume = grossReceiptVolume;
			this.NetReceiptVolume = netReceiptVolume;
			this.GrossHydrantDisbursement = 0.0;
			this.NetHydrantDisbursement = 0.0;
			this.NetBrokerReceipts = 0.0;
			this.GrossBrokerReceipts = 0.0;
		}


		public ExStarsTaxInfoSum Clone()
		{
			return this.MemberwiseClone() as ExStarsTaxInfoSum;
		}
	}
	
}
