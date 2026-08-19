using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Collections;

using FMBusinessObjects.DataObjects;

namespace FMBusinessObjects.UtilityObjects
{
	public class AdjustmentDistributorClass
	{
		#region Private Attributes
		private AdjustmentDistributionConfigurationDO.AdjustmentDistributionMethods adjustmentMethod;
		private ArrayList ownerAdjustmentList;
		private double totalGainLossGross;
		private double totalGainLossNet;
		private double totalVarianceGross;
		private double totalVarianceNet;
		private ArrayList companyList;
		private Hashtable transactionList;
		private const int TRANS_TYPE_4 = 4;
		private const int TRANS_TYPE_5 = 5;
		private const int TRANS_TYPE_6 = 6;
		private AccountingSite accountingSite;
		#endregion

		#region Constructors
		public AdjustmentDistributorClass ( AdjustmentDistributionConfigurationDO.AdjustmentDistributionMethods adjustMethod )
		{
			this.Init ( adjustMethod );
		}
		#endregion

		#region Properties
		/// <summary>
		/// This property will get and set the adjustment distribution method.
		/// </summary>
		public AdjustmentDistributionConfigurationDO.AdjustmentDistributionMethods AdjustmentMethod
		{
			get { return this.adjustmentMethod; }
			set { this.adjustmentMethod = value; }
		}

		/// <summary>
		/// This property will get the owner adjustment distribution list attribute.
		/// </summary>
		public ArrayList OwnerAdjustmentDistributions
		{
			get { return this.ownerAdjustmentList; }
		}

		/// <summary>
		/// This property will get the total gain/loss gross attribute.
		/// </summary>
		public double TotalGainLossGross
		{
			get { return this.totalGainLossGross; }
		}

		/// <summary>
		/// This property will get the total gain/loss net attribute.
		/// </summary>
		public double TotalGainLossNet
		{
			get { return this.totalGainLossNet; }
		}

		/// <summary>
		/// This property will get or set the total variance attribute.
		/// </summary>
		public double TotalVarianceNet
		{
			get { return this.totalVarianceNet; }
			set { this.totalVarianceNet = value; }
		}

		/// <summary>
		/// This property will get or set the total variance attribute.
		/// </summary>
		public double TotalVarianceGross
		{
			get { return this.totalVarianceGross; }
			set { this.totalVarianceGross = value; }
		}

		/// <summary>
		/// This property will get or set the owner list attribute.
		/// </summary>
		public ArrayList OwnerList
		{
			get { return this.companyList; }
			set { this.companyList = value; }
		}

		/// <summary>
		/// This property will get and set the transaction list hash table attribute.
		/// </summary>
		public Hashtable TransactionList
		{
			get { return this.transactionList; }
			set { this.transactionList = value; }
		}
		#endregion

		#region Public Methods
		/// <summary>
		/// This method will determine the type of adjustment method to be used and it will
		/// call the appropriate method to calculate the distribution.
		/// </summary>
		/// <returns></returns>
		public ArrayList PerformDistribution ( AccountingSite accountingSite )
		{
			this.accountingSite = accountingSite;

			switch (this.adjustmentMethod)
			{
				case AdjustmentDistributionConfigurationDO.AdjustmentDistributionMethods.ALLOCATION:
					this.CalculateAllocations ( );
					break;
				case AdjustmentDistributionConfigurationDO.AdjustmentDistributionMethods.THROUGHPUT:
					this.CalculateThroughput ( );
					break;
				case AdjustmentDistributionConfigurationDO.AdjustmentDistributionMethods.MANUAL:
					this.ownerAdjustmentList = new ArrayList ( );
					break;
				default:
					this.ownerAdjustmentList = new ArrayList ( );
					break;
			}

			return this.ownerAdjustmentList;
		}
		#endregion

		#region Private Methods
		/// <summary>
		/// This method will calculate the adjustment distribution evenly for all companies.
		/// The alogrithm used is: total gain loss * (1 / total number of companies) for gross
		/// and net.
		/// </summary>
		private void CalculateAllocations ( )
		{
			double percentPerCompany = 100.0;
			this.ownerAdjustmentList = new ArrayList ( );
			this.totalGainLossGross = 0.0;
			this.totalGainLossNet = 0.0;
			int decimalPlaces = (int) this.accountingSite.LoginSite._VolumeDecimalPlaces;

			// Calculate the percent to be used to determine the adjustment
			// distribution for each owner.
			if (this.companyList.Count > 0)
				percentPerCompany = 1.0 / System.Convert.ToDouble ( this.companyList.Count );

			// Calculate each owners adjustments and the total gain/loss.
			for (int nextCompany = 0; nextCompany < this.companyList.Count; nextCompany++)
			{
				AdjustmentOwnerRecord ownerRecord = new AdjustmentOwnerRecord ( );

				ownerRecord.OwnerName = (string) this.companyList[nextCompany];
				ownerRecord.GrossValue = Math.Round ( ( this.totalVarianceGross * percentPerCompany ), decimalPlaces, MidpointRounding.AwayFromZero );
				ownerRecord.NetValue = Math.Round ( ( this.totalVarianceNet * percentPerCompany ), decimalPlaces, MidpointRounding.AwayFromZero );

				this.totalGainLossGross = this.totalGainLossGross + ownerRecord.GrossValue;
				this.totalGainLossNet = this.totalGainLossNet + ownerRecord.NetValue;

				this.ownerAdjustmentList.Add ( ownerRecord );
			}
		}

		/// <summary>
		/// This method will calculate the adjustment gross/net for each company and total the
		/// total gain/loss. There are no return values, but the method sets the member variables
		/// for the total gain/loss and member variable that contains the list of each owners gain
		/// loss.
		/// </summary>
		private void CalculateThroughput ( )
		{
			double totalThroughputGross = 0.0;
			double totalThroughputNet = 0.0;
			this.totalGainLossGross = 0.0;
			this.totalGainLossNet = 0.0;

			// Get the volume conversion factor for the given site.
			double volConvFactor = this.accountingSite.GetVolumeConversionFactor ( );
			int decimalPlaces = (int) this.accountingSite.LoginSite._VolumeDecimalPlaces;

			// Add volumes for companies that do not have transactions.
			this.AddVolumesForCompanies ( );

			// Calculate the total usage for all owners both gross and net.
			IDictionaryEnumerator transEnumerator = this.transactionList.GetEnumerator ( );
			while (transEnumerator.MoveNext ( ) == true)
			{
				QuantityDO volume = (QuantityDO) transEnumerator.Value;

				// Convert the volume to the correct units and precision. Then sum up the throughput.
				totalThroughputGross = totalThroughputGross + Math.Round ( ( volume.Gross * volConvFactor ), decimalPlaces, MidpointRounding.AwayFromZero );
				totalThroughputNet = totalThroughputNet + Math.Round ( ( volume.Net * volConvFactor ), decimalPlaces, MidpointRounding.AwayFromZero );
			}

			// Calculate the percent usage for each owner both gross and net.
			transEnumerator = this.transactionList.GetEnumerator ( );
			while (transEnumerator.MoveNext ( ) == true)
			{
				QuantityDO volume = (QuantityDO) transEnumerator.Value;

				AdjustmentOwnerRecord adjustmentPerOwner = new AdjustmentOwnerRecord ( );
				double percentUsageGross = 100.0;
				double percentUsageNet = 100.0;

				// Calculate the percent to adjust for each owner
				if (totalThroughputGross != 0)
					percentUsageGross = ( Math.Round ( ( volume.Gross * volConvFactor ), decimalPlaces, MidpointRounding.AwayFromZero ) ) / totalThroughputGross;

				if (totalThroughputNet != 0)
					percentUsageNet = ( Math.Round ( ( volume.Net * volConvFactor ), decimalPlaces, MidpointRounding.AwayFromZero ) ) / totalThroughputNet;

				// Calculate the adjustment gross and net for each owner.
				adjustmentPerOwner.OwnerName = (string) transEnumerator.Key;
				adjustmentPerOwner.GrossValue = percentUsageGross * this.totalVarianceGross;
				adjustmentPerOwner.NetValue = percentUsageNet * this.totalVarianceNet;

				// Calculate the total gain/loss for all the owners.
				this.totalGainLossGross = this.totalGainLossGross + adjustmentPerOwner.GrossValue;
				this.totalGainLossNet = this.totalGainLossNet + adjustmentPerOwner.NetValue;

				// Add the individual owner adjustment to the list.
				this.ownerAdjustmentList.Add ( adjustmentPerOwner );
			}
		}

		/// <summary>
		/// This method will add volumes for companies that do not have any transactions.  The volumes
		/// will be set to zero.
		/// </summary>
		private void AddVolumesForCompanies ( )
		{
			foreach (string ownerName in this.OwnerList)
			{
				if (this.transactionList.Contains ( ownerName ) == false)
				{
					QuantityDO quantity = new QuantityDO ( 0.0, 0.0, 0.0, 0.0 );
					this.transactionList.Add ( ownerName, quantity );
				}
			}
		}

		/// <summary>
		/// This method initializes the adjustment distributor object to its initial state.
		/// </summary>
		private void Init ( AdjustmentDistributionConfigurationDO.AdjustmentDistributionMethods adjustMethod)
		{
			this.adjustmentMethod		= adjustMethod;
			this.ownerAdjustmentList	= new ArrayList ( );
			this.totalGainLossGross		= 0.0;
			this.totalGainLossNet		= 0.0;
			this.totalVarianceGross		= 0.0;
			this.totalVarianceNet		= 0.0;
			this.companyList			= new ArrayList ( );
			this.transactionList		= new Hashtable ( );
		}
		#endregion
	}
}