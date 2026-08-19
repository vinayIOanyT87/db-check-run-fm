namespace FMBusinessServices.ServiceClasses
{
    using System;
    using System.Data;
    using System.Data.SqlClient;

    using FMBusinessObjects.BusinessInterfaces;
    using FMBusinessObjects.DataObjects;
    using FMBusinessObjects.ServiceRequests;

    using DataAccessLayer;

    using Varec.CommonComponents.EngineeringUnitsLibrary;

    public class CloseoutListProcessorClass : ICloseoutListProcessor
	{
		#region Private Data members
		private ConsolidatedDAClass consolidatedDA;
		#endregion
		public CloseoutListDO Process( CloseoutListSR sr )
		{
			if (sr == null)
			{
				throw new ArgumentNullException(nameof(sr));
			}

			if (sr.ProductGuid == Guid.Empty)
			{
				throw new Exception("Product is required.");
			}

		    CloseoutListDO closeoutList = new CloseoutListDO
		                                      {
		                                          SiteGuid = sr.CurrentSiteGuid,
		                                          ManagerGuid = sr.ManagerGuid,
		                                          ProductGuid = sr.ProductGuid,
		                                          FromDate = sr.StartDate,
		                                          ToDate = sr.EndDate
		                                      };


		    this.consolidatedDA = new ConsolidatedDAClass();
	
			DataSet ds;
			
			using (SqlCommand cmd = new SqlCommand())
			{
				closeoutList.GetSelectCommand(cmd);
				ds = this.consolidatedDA.GetDataSet(cmd, sr.Security);
			}

			closeoutList.Load( ds );

			if (sr.GetPreviousAndSubsequentCloseouts)
			{
				using (SqlCommand cmd = new SqlCommand())
				{
					closeoutList.GetPreviousSelectCommand(cmd);
					ds = this.consolidatedDA.GetDataSet(cmd, sr.Security);
				}

				closeoutList.LoadPrevious(ds);

				using (SqlCommand cmd = new SqlCommand())
				{
					closeoutList.GetSubsequentSelectCommand(cmd);
					ds = this.consolidatedDA.GetDataSet(cmd, sr.Security);
				}

				closeoutList.LoadSubsequent(ds);
			}

			// Only do further processing if there is something to process
			if (closeoutList.CloseoutList.Count > 0 && sr.ConvertUnits)
			{
				ProductsClass products = new ProductsClass ( );
				ProductClass product = products.GetByInfoAuthorizedCompanies ( sr.Security, sr.ProductGuid, true, false, false );

				SitesClass sites = new SitesClass ( );
				SiteClass site = sites.GetByMemberAndProcessVariables ( sr.Security, sites.GetIdentityGuid( sr.Security, sr.Site ), false, false );

				EngineeringUnit volumeUnits;
				EngineeringUnit massUnits;
				int volumeDecimalPlaces;
				int massDecimalPlaces;

				if (product.VolumeUnits != 0)
				{
					volumeUnits = product.VolumeUnits;
					volumeDecimalPlaces = product.VolumeDecimalPlaces;
				}
				else if (product.ProductType == ProductType.AdditiveProduct)
				{
					volumeUnits = site.AdditiveVolumeUnits;
					volumeDecimalPlaces = site._AdditiveVolumeDecimalPlaces;
				}
				else
				{
					volumeUnits = site.VolumeUnits;
					volumeDecimalPlaces = site._VolumeDecimalPlaces;
				}

				if (product.MassUnits != 0)
				{
					massUnits = product.MassUnits;
					massDecimalPlaces = product.MassDecimalPlaces;
				}
				else
				{
					massUnits = site.MassUnits;
					massDecimalPlaces = site._MassDecimalPlaces;
				}


				double volumeConversionFactor = 0.0;
				EngineeringUnits.Convert(1, EngineeringUnit.FmvMeter3, ref volumeConversionFactor, volumeUnits, 0.0);

				double massConversionFactor = 0.0;
				EngineeringUnits.Convert( 1, EngineeringUnit.FmmKg, ref massConversionFactor, massUnits, 0.0 );

				CloseoutDO lastCloseoutRecord = null;
				for (int i = (closeoutList.CloseoutList.Count - 1); i >= 0; i--)
				{
					CloseoutDO closeoutRecord = closeoutList.CloseoutList[i] as CloseoutDO;
					if (closeoutRecord == null)
					{
						continue;
					}

					closeoutRecord.BookInventory.GrossInventoryChange = Math.Round(volumeConversionFactor * closeoutRecord.BookInventory.GrossInventoryChange, volumeDecimalPlaces, MidpointRounding.AwayFromZero);
					closeoutRecord.BookInventory.NetInventoryChange= Math.Round(volumeConversionFactor * closeoutRecord.BookInventory.NetInventoryChange, volumeDecimalPlaces, MidpointRounding.AwayFromZero);
					closeoutRecord.BookInventory.MassInventoryChange = Math.Round(massConversionFactor * closeoutRecord.BookInventory.MassInventoryChange, massDecimalPlaces, MidpointRounding.AwayFromZero);
					closeoutRecord.TotalPhysicalInventory.GrossInventoryChange = Math.Round(volumeConversionFactor *closeoutRecord.TotalPhysicalInventory.GrossInventoryChange, volumeDecimalPlaces, MidpointRounding.AwayFromZero);
					closeoutRecord.TotalPhysicalInventory.NetInventoryChange = Math.Round(volumeConversionFactor * closeoutRecord.TotalPhysicalInventory.NetInventoryChange, volumeDecimalPlaces, MidpointRounding.AwayFromZero);
					closeoutRecord.TotalPhysicalInventory.MassInventoryChange = Math.Round(massConversionFactor * closeoutRecord.TotalPhysicalInventory.MassInventoryChange, massDecimalPlaces, MidpointRounding.AwayFromZero);
					closeoutRecord.TotalVariance.GrossInventoryChange = closeoutRecord.Variance.GrossInventoryChange;
					closeoutRecord.TotalVariance.NetInventoryChange = closeoutRecord.Variance.NetInventoryChange;
					closeoutRecord.TotalVariance.MassInventoryChange = closeoutRecord.Variance.MassInventoryChange;

					if (lastCloseoutRecord != null
					&& lastCloseoutRecord.CloseoutDate.Month == closeoutRecord.CloseoutDate.Month
					&& lastCloseoutRecord.CloseoutDate.Year == closeoutRecord.CloseoutDate.Year)
					{
						closeoutRecord.TotalVariance.GrossInventoryChange += lastCloseoutRecord.TotalVariance.GrossInventoryChange;
						closeoutRecord.TotalVariance.NetInventoryChange += lastCloseoutRecord.TotalVariance.NetInventoryChange;
						closeoutRecord.TotalVariance.MassInventoryChange += lastCloseoutRecord.TotalVariance.MassInventoryChange;
					}

				    // ReSharper disable CompareOfFloatsByEqualityOperator
					if (product.LoadByWeight && product._MassPackageSize.Value != 0)
					{
						closeoutRecord.BookInventory.Package			= closeoutRecord.BookInventory.Mass / product._MassPackageSize.Value;
						closeoutRecord.TotalPhysicalInventory.Package	= closeoutRecord.TotalPhysicalInventory.Mass / product._MassPackageSize.Value;
						closeoutRecord.TotalVariance.Package			= closeoutRecord.TotalVariance.MassInventoryChange / product._MassPackageSize.Value;
					}
					else if (product._VolumePackageSize.Value != 0)
					{
						closeoutRecord.BookInventory.Package			= closeoutRecord.BookInventory.Net / product._VolumePackageSize.Value;
						closeoutRecord.TotalPhysicalInventory.Package	= closeoutRecord.TotalPhysicalInventory.Net / product._VolumePackageSize.Value;
						closeoutRecord.TotalVariance.Package			= closeoutRecord.TotalVariance.NetInventoryChange / product._VolumePackageSize.Value;
					}
                    // ReSharper restore CompareOfFloatsByEqualityOperator

                    lastCloseoutRecord = closeoutRecord;
				}
			}

			return closeoutList;
		}
	}
}
