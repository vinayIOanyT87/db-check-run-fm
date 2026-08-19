namespace FMBusinessServices.ServiceClasses
{
	using System;
	using System.Linq;
	using System.Collections.Generic;
	using System.Text;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.Exceptions;


	public class ExStarsTotalsElement :IComparable
	{
		public CompanyClass ManagingCompany { get; protected set; }
		public EnumExStarsTrxType TrxType { get; protected set; }
		public string TaxCode { get; protected set; }
		/// <summary>
		/// Product is here as a visual reference, but it is the taxcode that is the unifying field
		/// </summary>
		public ProductClass  Product { get; protected set; }
		public double GrossVolume { get; set; }
		public double NetVolume { get; set; }
		public int Count { get; set; }
		public string DisplayKey { get {  return string.Format("{0};{1};", this.ManagingCompany.ID, this.TaxCode);} }

		public ExStarsTotalsElement(CompanyClass managingCompany, ProductClass product, EnumExStarsTrxType trxType)
		{
			this.ManagingCompany = managingCompany;
			this.TrxType = trxType;
			this.Product = product;
			this.TaxCode = product.TaxCode;
			this.GrossVolume = 0.0;
			this.NetVolume = 0.0;
			this.Count = 0;
		}

		public override int GetHashCode()
		{
			return this.ManagingCompany.MasterRecordGuid.GetHashCode() ^ TrxType.GetHashCode() ^ TaxCode.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			ExStarsTotalsElement compareTo = obj as ExStarsTotalsElement;
			return compareTo != null
				&& this.ManagingCompany.MasterRecordGuid.Equals(compareTo.ManagingCompany.MasterRecordGuid)
				&& this.TrxType.Equals(compareTo.TrxType)
				&& this.TaxCode.Equals(compareTo.TaxCode);
		}
		

		public override string ToString()
		{
			return string.Format("{0};{1};{2}", this.ManagingCompany.ID, Product.ID, this.TrxType);
		}

		public int CompareTo(object obj)
		{
			ExStarsTotalsElement compareTo = obj as ExStarsTotalsElement;
			if (compareTo == null)
			{
				throw new ExStarsBusinessException("ExStarsTotalsElement.CompareTo(null)");
			}
			int retVal1 = this.ManagingCompany.ID.CompareTo(compareTo.ManagingCompany.ID);
			int retVal2 = this.Product.ID.CompareTo(compareTo.Product.ID);
			int retVal3 = this.TrxType.CompareTo(compareTo.TrxType);

			return retVal1 != 0
				       ? retVal1
				       : retVal2 != 0
					         ? retVal2
					         : retVal3;
		}		
	}



	/// <summary>
	/// A collection of gross and net totals for all transaction types (or at least as many as the IRS is interested in)
	/// By manager, product, transaction type
	/// </summary>
	[Serializable]
	public class ExStarsManagerTotals
	{
		public SortedList<ExStarsTotalsElement, ExStarsTotalsElement> Totals { get; protected set; }
		private readonly ExStarsSiteConfigExpanded config;

		public ExStarsManagerTotals()
		{
			this.config = null;
			Totals = new SortedList<ExStarsTotalsElement, ExStarsTotalsElement>();
		}


		public  ExStarsManagerTotals( ExStarsSiteConfigExpanded config)
		{
			this.config = config;
			Totals = new SortedList<ExStarsTotalsElement, ExStarsTotalsElement>();
		}

		public int Count { get {  return Totals.Count;} }

		public ExStarsTotalsElement Get(ExStarsTotalsElement keyByElement, EnumExStarsTrxType trxType)
		{
			ExStarsTotalsElement key = new ExStarsTotalsElement(keyByElement.ManagingCompany, keyByElement.Product, trxType);
			if (! Totals.ContainsKey(key))
			{
				// return the key with gross and net values of 0.0
				return key;
			}
			return Totals[key];
		}

		public double TotalNetVolume()
		{
			// issues are negative amounts
			var totalNet = (from element in Totals.Values 
							where 
								element.TrxType == EnumExStarsTrxType.Issue
								|| element.TrxType == EnumExStarsTrxType.BulkIssue
								|| element.TrxType == EnumExStarsTrxType.Defuel
							select element.NetVolume).Sum();
			return totalNet;
		}

		public void Reset()
		{
			Totals.Clear();
		}

		public void AddtoSum(EnumExStarsTrxType trxType, Guid managingCompanyGuid, Guid productGuid, double grossVolume, double netVolume, int recordCount = 1)
		{
			ProductClass product = config.LookUpProduct(productGuid);
			AddtoSum(trxType, managingCompanyGuid, product, grossVolume, netVolume, recordCount);
		}

		/// <summary>
		/// Add to gross and net sums according to transaction type
		/// </summary>
		/// <param name="trxType">transaction type of the source</param>
		/// <param name="managingCompanyGuid">In reality this is always the same manager, but does could support multiple managers</param>
		/// <param name="product">totals are segregated by product</param>
		/// <param name="grossVolume">gallons amount</param>
		/// <param name="netVolume">gallons amount</param>
		/// /// <param name="recordCount">how many rows in the database</param>
		public void AddtoSum(EnumExStarsTrxType trxType, Guid managingCompanyGuid, ProductClass product, double grossVolume, double netVolume, int recordCount = 1)
		{
			// C_ExSTARS_X12_Schedule_Detail::Generate_Book_Adjustments ~ 917
			// for positive adjustments, summarize as receipts, for negative as issues.
			if (trxType == EnumExStarsTrxType.Adjustment)
			{
				trxType = netVolume >= 0 ? EnumExStarsTrxType.Receipt : EnumExStarsTrxType.Issue;
			}
			CompanyClass managingCompany = config.LookUpCompany(managingCompanyGuid, false, trxType.ToString(), "AddtoSum");
			ExStarsTotalsElement key = new ExStarsTotalsElement(managingCompany, product, trxType);
			if (Totals.ContainsKey(key))
			{
				Totals[key].GrossVolume += grossVolume;
				Totals[key].NetVolume += netVolume;
				Totals[key].Count += recordCount;
			}
			else
			{
				key.GrossVolume = grossVolume;
				key.NetVolume = netVolume;
				key.Count = recordCount;
				Totals.Add(key, key);
			}
		}

		public string ReportTotals()
		{
			StringBuilder report = new StringBuilder(100 * (this.Count + 5));
			String lastDisplayKey = "";
			foreach (ExStarsTotalsElement mgrTotal in this.Totals.Values)
			{
				if (lastDisplayKey == mgrTotal.DisplayKey)
				{
					continue;
				}
				lastDisplayKey = mgrTotal.DisplayKey;

				ExStarsTotalsElement beginningInventory = this.Get(mgrTotal, EnumExStarsTrxType.BeginningInventory);
				ExStarsTotalsElement receipts = this.Get(mgrTotal, EnumExStarsTrxType.Receipt);
				ExStarsTotalsElement issues = this.Get(mgrTotal, EnumExStarsTrxType.Issue);
				ExStarsTotalsElement defuel = this.Get(mgrTotal, EnumExStarsTrxType.Defuel);
				ExStarsTotalsElement bulkIssue = this.Get(mgrTotal, EnumExStarsTrxType.BulkIssue);
				ExStarsTotalsElement brokerReceipt = this.Get(mgrTotal, EnumExStarsTrxType.BrokerReceipt);
				ExStarsTotalsElement brokerDisbursement = this.Get(mgrTotal, EnumExStarsTrxType.BrokerDisbursement);
				ExStarsTotalsElement endingInventory = this.Get(mgrTotal, EnumExStarsTrxType.EndingInventory);
				ExStarsTotalsElement adjustments = this.Get(mgrTotal, EnumExStarsTrxType.Adjustment);

				// Issues have negative values
				double totalIssueGross = issues.GrossVolume + bulkIssue.GrossVolume + defuel.GrossVolume;
				double totalIssueNet = issues.NetVolume + bulkIssue.NetVolume + defuel.NetVolume;
				int totalIssueCount = issues.Count + bulkIssue.Count + defuel.Count;
				//\FuelsManager Aviation\Release v7.1 SP5\Core\ExSTARS Reporting Utility\common_utilities.h ~ 162
				//  dGetTotalGrossBookInventory()
				double calculatedEndingGross = beginningInventory.GrossVolume + receipts.GrossVolume + brokerReceipt.GrossVolume - brokerDisbursement.GrossVolume + totalIssueGross + adjustments.GrossVolume;
				double calculatedEndingNet = beginningInventory.NetVolume + receipts.NetVolume + brokerReceipt.NetVolume - brokerDisbursement.NetVolume + totalIssueNet + adjustments.NetVolume;
				int calculatedEndingCount = beginningInventory.Count + receipts.Count + brokerReceipt.Count + brokerDisbursement.Count + totalIssueCount + adjustments.Count;
				double gainLossGross = endingInventory.GrossVolume - calculatedEndingGross;
				double gainLossNet = endingInventory.NetVolume - calculatedEndingNet;

				report.AppendLine();
				report.AppendFormat("-------- TOTALS for \"{0}\"::{1} (IRS Mode:{2}) -------------------\n", mgrTotal.ManagingCompany.ID, mgrTotal.Product.ID, mgrTotal.Product.TaxCode);
				report.AppendFormat("                            {0,12}{1,12}{2,20}\n", "GROSS", "NET","TRANSACTION COUNT");
				report.AppendFormat("Beginning Inventory:        {0,12}{1,12}{2,20}\n", beginningInventory.GrossVolume, beginningInventory.NetVolume, beginningInventory.Count);
				report.AppendFormat("Total Receipts:             {0,12}{1,12}{2,20}\n", receipts.GrossVolume, receipts.NetVolume, receipts.Count);
				report.AppendFormat(" Issues:                    {0,12}{1,12}{2,20}\n", -issues.GrossVolume, -issues.NetVolume, issues.Count);
				report.AppendFormat(" Bulk Issues:               {0,12}{1,12}{2,20}\n", -bulkIssue.GrossVolume, -bulkIssue.NetVolume, bulkIssue.Count);
				report.AppendFormat(" Defuel:                    {0,12}{1,12}{2,20}\n", -defuel.GrossVolume, -defuel.NetVolume, defuel.Count);
				report.AppendFormat("Total Deliveries:           {0,12}{1,12}{2,20}\n", -totalIssueGross, -totalIssueNet, totalIssueCount);
				report.AppendFormat("Total Broker Receipts:      {0,12}{1,12}{2,20}\n", brokerReceipt.GrossVolume, brokerReceipt.NetVolume, brokerReceipt.Count);
				report.AppendFormat("Total Broker Issues:        {0,12}{1,12}{2,20}\n", brokerDisbursement.GrossVolume, brokerDisbursement.NetVolume, brokerDisbursement.Count);
				report.AppendFormat(" Adjustments:               {0,12}{1,12}{2,20}\n", adjustments.GrossVolume, adjustments.NetVolume, adjustments.Count);
				report.AppendFormat("CALCULATED ENDING Inventory:{0,12}{1,12}{2,20}\n", calculatedEndingGross, calculatedEndingNet, calculatedEndingCount);
				report.AppendFormat("Reported Ending Inventory:  {0,12}{1,12}{2,20}\n", endingInventory.GrossVolume, endingInventory.NetVolume, endingInventory.Count);
				report.AppendFormat("Gain or Losses (Calculated):{0,12}{1,12}\n", gainLossGross, gainLossNet);
				report.AppendLine();
			}

			return report.ToString();
		}


	}
}
