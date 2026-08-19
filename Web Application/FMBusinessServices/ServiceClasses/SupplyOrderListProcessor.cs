 #pragma warning disable 1587
/// <summary>
/// SupplyOrderListProcessor
///
/// Original Author: Ivan Orndorff
/// Revisions: See source control comments
///
/// (C) Copyright 2007 by Varec, Inc.  All rights reserved.
///
///	MODIFICATION HISTORY:
///		Date:			By:					Reason:
///		----------	-----------------	-------------------------------------------
///		2007-09-24	I. Orndorff			- Initial Revision based off v7.3.0.0 of
///												OrderListProcessor.
///	
///		2007-10-10	I. Orndorff			- Added code to get Confirmation Number and
///												Standing Offer Reference in "GetDetail()".
///
///		2007-11-08	I. Orndorff			- Removed standing offer reference. No longer
///												part of List.
///
///		2007-11-20	I. Orndorff			- Modified "GetDetail()" to get "InventoryDate",
///												"Manager", "Owner", "BillToID", "ShipperID",
///												"ShipToID" and "CarrierID".
///
///		08/27/2008	W.Gray				7.4.5.9 - Change to GetDetail to convert times to UTC (CSI 6114)
///
///		05/19/2009	A. Coker				Fixed defect 3611. Rearranged site list in query to fit string passed into sql execute.
/// 
///      2009-06-23  Richard Panachida WI#4092: The date filter was converting to UTC and it was already converted. Removed
///                                    the conversion.
///                                    
///		2009-06-24	I. Orndorff			- Modified "IncludeDeleted()" to use "ShowDeletedTransactions"
///												  in the GeneralConfigDO.
/// </summary>
#pragma warning restore 1587
namespace FMBusinessServices.ServiceClasses
{
    using System;
    using System.Collections;
    using System.Data;
    using System.Data.SqlClient;

    using FMBusinessObjects.BusinessInterfaces;
    using FMBusinessObjects.DataObjects;
    using FMBusinessObjects.LogClient;
    using FMBusinessObjects.ServiceRequests;

    using FMBusinessServices.DataAccessLayer;

    public class SupplyOrderListProcessorClass : ISupplyOrderListProcessor
	{
		#region Private data members
		private readonly ConsolidatedDAClass consolidatedDA;
		private SupplyOrderListSR supplyorderListSR;
		private SupplyOrderListDO supplyorderListDO;
		#endregion

		#region Constructors
		public SupplyOrderListProcessorClass ( )
		{
			this.consolidatedDA		= new ConsolidatedDAClass ( );
			this.supplyorderListDO	= null;
			this.supplyorderListSR	= null;	
		}
		#endregion

		public SupplyOrderListDO Process ( SupplyOrderListSR supplyorderListSRParam )
		{
			this.supplyorderListSR = supplyorderListSRParam;
			this.supplyorderListDO = new SupplyOrderListDO ( );

			switch (this.supplyorderListSR.SubRequest)
			{
				case SupplyOrderListSR.RequestTypes.GET_HEADER_DATA:
					this.GetHeaderData ( );
					break;

				case SupplyOrderListSR.RequestTypes.GET_DETAIL:
					this.GetDetail ( );
					break;
			}

			return this.supplyorderListDO;
		}

		private void GetHeaderData ( )
		{
			StopWatch timer = new StopWatch ( StopWatch.Appnames.OrderEntry, "SupplyOrderListProcessor.GetHeaderData()" );

			this.GetProductList ( );
			this.GetOrderTypeList ( );
			this.GetOrderStatusList ( );

			timer.Stop ( );
		}

		private void GetOrderTypeList ()
		{
			this.supplyorderListDO.OrderTypeList.Add ( this.supplyorderListSR.AllText );

			var aliases = new TransactionAliasesClass();
			var aliasNames = aliases.EnumerateNamesOnly( this.supplyorderListSR.Security, true );

			foreach ( var alias in aliasNames )
			{
				if ( alias.TransTypeID == TransactionTypes.T18_SupplyOrder )
				{
					this.supplyorderListDO.OrderTypeList.Add( alias.AliasName );
				}
			}
		}


		private void GetOrderStatusList ( )
		{
			string[] names = Enum.GetNames ( typeof ( TransactionStatus ) );

			foreach (string status in names)
			{
				this.supplyorderListDO.OrderStatusList.Add ( status );
			}

			this.InsertAllOption ( this.supplyorderListDO.OrderStatusList );
		}


		private void GetProductList ( )
		{
			// Get the list of products that are associated to the site and the user.
			ProductsClass products = new ProductsClass ( );
			ProductCollectionClass productCollection = products.EnumerateByFilterAndLocalize ( this.supplyorderListSR.Security, null, false );

			foreach (ProductClass item in productCollection)
			{
				this.supplyorderListDO.ProductList.Add ( item.ID );
			}

			this.InsertAllOption ( this.supplyorderListDO.ProductList );
		}


		private void InsertAllOption ( ArrayList array )
		{
			array.Insert ( 0, this.supplyorderListSR.AllText );
		}

		private bool IncludeDeleted ( )
		{
			StopWatch timer = new StopWatch ( StopWatch.Appnames.OrderEntry, "SupplyOrderListProcessor.IncludeDeleted()" );

		    GeneralConfigSR genConfigSR = new GeneralConfigSR
		                                  {
		                                      Security = this.supplyorderListSR.Security,
		                                      Request =
		                                          GeneralConfigSR.GeneralConfigurationRequests
		                                          .GET_CONFIGURATION
		                                  };

		    GeneralConfigProcessorClass proc = new GeneralConfigProcessorClass();
			GeneralConfigDO genConfigDO = proc.Get ( genConfigSR );

			timer.Stop ( );

			return genConfigDO.ShowDeletedTransactions;
		}


		private void GetDetail ( )
		{
			StopWatch timer = new StopWatch ( StopWatch.Appnames.OrderEntry, "SupplyOrderListProcessor.GetDetail()" );

			// Determine the sites needed for filtering since Order Summary should show not only the
			// current site, but Orders from and groups this site is a member of.
			this.supplyorderListSR.Criteria.SiteList = this.DetermineSites ( );

			// Should we include deletes?
			this.supplyorderListSR.Criteria.ShowDeleted = this.IncludeDeleted ( );

			// Get SQL Command
			using (SqlCommand cmd = new SqlCommand())
			{
				this.supplyorderListDO.GetSelectCommand(cmd, this.supplyorderListSR.Criteria);

				// Read order list from the database
				DataSet ds = this.consolidatedDA.GetDataSet(cmd, this.supplyorderListSR.Security);

				// Load the data object line items
				foreach (DataRow row in ds.Tables[0].Rows)
				{
					// Create a new line item object
				    SupplyOrderListLineItemDO lineItem = new SupplyOrderListLineItemDO
				                                         {
				                                             TransactionID =
				                                                 row.IsNull("TransactionID") ? null : row["TransactionID"].ToString(),
				                                             TransactionAlias = row.IsNull("TransactionAlias") ? null : row["TransactionAlias"].ToString(),
				                                             OrderStatus = row.IsNull("LookupTransactionStatusIndex") ? null : row["LookupTransactionStatusIndex"].ToString(),
				                                             TransactionDate = row["TransactionDate"].ToString()
				                                         };

				    // Load the line item object

				    if (row["TransactionDate"] is DateTimeOffset)
					{
						lineItem.TransactionDateTime = (DateTimeOffset)row["TransactionDate"];
					}
					else
					{
						lineItem.TransactionDateTime = new DateTimeOffset();
					}

					if (row["InventoryDate"] is DateTime)
					{
						lineItem.InventoryDateTime = (DateTime)row["InventoryDate"];
					}
					else
					{
						lineItem.InventoryDateTime = new DateTime();
					}
					lineItem.InventoryDate = DateEfficacy.convertToMonthDayYear(lineItem.InventoryDateTime);

					lineItem.DocumentNumber = row.IsNull("DocumentNumber") ? null : row["DocumentNumber"].ToString();
					lineItem.ConfirmationNumber = row.IsNull("ConfirmationNumber") ? null : row["ConfirmationNumber"].ToString();
					lineItem.PONumber = row.IsNull("PONumber") ? null : row["PONumber"].ToString();

					lineItem.RequiredDeliveryDate = row.IsNull("RequiredDeliveryDate") ? null : row["RequiredDeliveryDate"].ToString();

					if (row["RequiredDeliveryDate"] is DateTimeOffset)
					{
						lineItem.RequiredDeliveryDateTime = (DateTimeOffset)row["RequiredDeliveryDate"];
					}
					else
					{
						lineItem.RequiredDeliveryDateTime = new DateTimeOffset();
					}

					// Estimated Date To
					lineItem.EstimatedDeliveryDateTo = row["EstimatedDeliveryDateTo"].ToString();

					if (row["EstimatedDeliveryDateTo"] is DateTimeOffset)
					{
						/* Preserve the DateTimeOffset object */
						lineItem.EstimatedDeliveryDateToTime = (DateTimeOffset)row["EstimatedDeliveryDateTo"];
					}
					else
					{
						lineItem.EstimatedDeliveryDateToTime = new DateTimeOffset();
					}

					// Estimated Date From
					lineItem.EstimatedDeliveryDateFrom = row.IsNull("EstimatedDeliveryDateFrom") ? null : row["EstimatedDeliveryDateFrom"].ToString();

					if (row["EstimatedDeliveryDateFrom"] is DateTimeOffset)
					{
						/* Preserve the DateTimeOffset object */
						lineItem.EstimatedDeliveryDateFromTime = (DateTimeOffset)row["EstimatedDeliveryDateFrom"];
					}
					else
					{
						lineItem.EstimatedDeliveryDateFromTime = new DateTimeOffset();
					}

					lineItem.TransactionStatus = row.IsNull("SupplierID") ? TransactionStatus.InProgress : (TransactionStatus)row["TransStatus"];
					lineItem.SupplierID = row.IsNull("SupplierID") ? null : row["SupplierID"].ToString();
					lineItem.Manager = row.IsNull("ManagerID") ? null : row["ManagerID"].ToString();
					lineItem.Owner = row.IsNull("OwnerID") ? null : row["OwnerID"].ToString();
					lineItem.BillToID = row.IsNull("BillToID") ? null : row["BillToID"].ToString();
					lineItem.ShipperID = row.IsNull("ShipperID") ? null : row["ShipperID"].ToString();
					lineItem.ShipToID = row.IsNull("ShipToID") ? null : row["ShipToID"].ToString();
					lineItem.CarrierID = row.IsNull("CarrierID") ? null : row["CarrierID"].ToString();

					// Add it to the order list data object
					this.supplyorderListDO.LineItems.Add(lineItem);
				}
			}

			timer.Stop ( );
		}

		private ArrayList DetermineSites ( )
		{
			StopWatch timer = new StopWatch ( StopWatch.Appnames.OrderEntry, "SupplyOrderListProcessor.DetermineSites()" );

			// Get the sites info list and time it
			StopWatch timer3 = new StopWatch ( StopWatch.Appnames.OrderEntry, "** SitesInfo" );
			SitesInfoClass infoList = new SitesInfoClass ( );
			SiteInfoDO siteInfoDO = infoList.RefreshSiteInfo ( this.supplyorderListSR.Security );
			timer3.Stop ( );

		    ArrayList siteArray = new ArrayList { this.supplyorderListSR.Security.SiteGuid };

		    // Add the current site

		    // If the base Site is a group, enumerate all the member sites
			SiteCollectionClass siteCollection = siteInfoDO.EnumerateByParentSite ( this.supplyorderListSR.Security.SiteGuid );

			foreach (SiteClass site in siteCollection)
			{
				siteArray.Add ( site.SiteGuid );
			}

			// Now enumerate the parent sites
			ArrayList groupsEvaluated = new ArrayList ( );

			//SiteCollection = Sites.Enumerate( this.supplyorderListSR.Security );
			siteCollection = siteInfoDO.SiteCollection;

			foreach (SiteClass checkSite in siteCollection)
			{
				this.EnumerateParentSites ( siteInfoDO, checkSite, groupsEvaluated, siteArray );
			}

			timer.Stop ( );

			return siteArray;
		}

		private void EnumerateParentSites ( SiteInfoDO siteInfoDO, SiteClass site, ArrayList groupsEvaluated, ArrayList finalSiteList )
		{
			// Is the group we are looking at a Site Group?
			if (site.SiteGroup)
			{
				// Do we need to evaluate this group?
				foreach (Guid siteID in groupsEvaluated)
				{
					if (siteID == site.SiteGuid)
					{
						return;
					}
				}

				// Mark this one as having been evaluated
				groupsEvaluated.Add(site.SiteGuid);

				// Get the member sites of this group
				SiteCollectionClass children = siteInfoDO.EnumerateByParentSite(site.SiteGuid);

				// Is our site a member of this group?
				foreach (SiteClass child in children)
				{
					// Is this a group?
					if (child.SiteGroup)
					{
					    this.EnumerateParentSites ( siteInfoDO, child, groupsEvaluated, finalSiteList );
					}

					if (child.SiteGuid == this.supplyorderListSR.Security.SiteGuid)
					{
						finalSiteList.Add(site.SiteGuid);
					}
				}
			}
		}
	}
}