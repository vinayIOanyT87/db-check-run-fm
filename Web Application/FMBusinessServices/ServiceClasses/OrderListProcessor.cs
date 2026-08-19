using System.Linq;
using System.Web;
using System.ServiceModel;
using System.Data;
using System.Collections;
using System.Data.SqlClient;
using System.Text;
using System.Diagnostics;
using System.Reflection;
using System.Globalization;
using System;

using FMBusinessObjects.LogClient;
using FMBusinessObjects.DataObjects;
using FMBusinessObjects.BusinessInterfaces;
using FMBusinessObjects.ServiceRequests;
using FMBusinessServices.DataAccessLayer;
using FMBusinessObjects.Exceptions;
using FMBusinessServices.InternalClasses;
using FMBusinessObjects.UtilityObjects;

namespace FMBusinessServices.ServiceClasses
{
	public class OrderListProcessorClass : IOrderListProcessor
	{
		#region Private data members
		private OrderListSR orderListSR;
		private OrderListDO orderListDO;
		private ConsolidatedDAClass consolidatedDA;
		#endregion

		#region Constructors
		public OrderListProcessorClass()
		{
			this.consolidatedDA = new ConsolidatedDAClass();
		}
		#endregion

		public OrderListDO Process(OrderListSR inOrderListSR)
		{
			this.orderListSR = inOrderListSR;
			this.orderListDO = new OrderListDO();

			switch (this.orderListSR.SubRequest)
			{
				case OrderListSR.RequestTypes.GET_HEADER_DATA:
					GetHeaderData();
					break;

				case OrderListSR.RequestTypes.GET_DETAIL:
					GetDetail();
					break;
			}

			return this.orderListDO;
		}

		private void GetHeaderData()
		{
			StopWatch timer = new StopWatch(StopWatch.Appnames.OrderEntry, "OrderListProcessor.GetHeaderData()");

			this.GetProductList();
			this.GetLocationList();
			this.GetOrderTypeList();
			this.GetOrderStatusList();

			timer.Stop();
		}

		private void GetOrderTypeList()
		{
			this.orderListDO.OrderTypeList.Add( this.orderListSR.AllText );

			var aliases = new TransactionAliasesClass();
			var aliasNames = aliases.EnumerateNamesOnly( this.orderListSR.Security, true );

			foreach ( var alias in aliasNames )
			{
				if ( alias.TransTypeID == TransactionTypes.T17_Order )
				{
					this.orderListDO.OrderTypeList.Add( alias.AliasName );
				}
			}
		}

		private void GetOrderStatusList()
		{
			string[] names = System.Enum.GetNames(typeof(TransactionStatus));

			foreach (string Status in names)
			{
				this.orderListDO.OrderStatusList.Add(Status);
			}

			this.InsertAllOption(this.orderListDO.OrderStatusList);
		}

		private void GetLocationList()
		{
			StationsClass stations = new StationsClass();
			StationCollectionClass stationCollection = (StationCollectionClass)stations.Enumerate(this.orderListSR.Security);

			foreach (BaseDataObject item in stationCollection)
			{
				this.orderListDO.LocationList.Add(item.ID);
			}

			this.InsertAllOption(this.orderListDO.LocationList);
		}

		private void GetProductList()
		{
			// Get the list of products that are associated to the site and the user.
			ProductsClass products = new ProductsClass();
			ProductCollectionClass productCollection = (ProductCollectionClass)products.EnumerateByFilterAndLocalize(this.orderListSR.Security, null, false);

			foreach (BaseDataObject item in productCollection)
			{
				this.orderListDO.ProductList.Add(item.ID);
			}

			this.InsertAllOption(this.orderListDO.ProductList);
		}

		private void InsertAllOption(ArrayList array)
		{
			array.Insert(0, this.orderListSR.AllText);
		}

		private bool IncludeDeleted()
		{
			StopWatch timer = new StopWatch(StopWatch.Appnames.OrderEntry, "OrderListProcessor.IncludeDeleted()");

			GeneralConfigSR genConfigSR = new GeneralConfigSR();
			genConfigSR.Security = this.orderListSR.Security;
			genConfigSR.Request = GeneralConfigSR.GeneralConfigurationRequests.GET_CONFIGURATION;

			GeneralConfigProcessorClass services = new GeneralConfigProcessorClass();
			GeneralConfigDO genConfigDO = services.Get(genConfigSR);

			timer.Stop();

			return genConfigDO.ShowDeletedTransactions;
		}

		private void GetDetail()
		{
			StopWatch timer = new StopWatch(StopWatch.Appnames.OrderEntry, "OrderListProcessor.GetDetail()");

			// Determine the sites needed for filtering since Order Summary should show not only the
			// current site, but Orders from and groups this site is a member of.
			this.orderListSR.Criteria.SiteList = DetermineSites();

			// Should we include deletes?
			this.orderListSR.Criteria.ShowDeleted = this.IncludeDeleted();

			// Convert times which are in Site Time to UTC Time
			this.orderListSR.Criteria.StartDate = TimeConverter.ToUTCTime(this.orderListSR.Criteria.StartDate);
			this.orderListSR.Criteria.EndDate = TimeConverter.ToUTCTime(this.orderListSR.Criteria.EndDate);

			// Read order list from the database
			DataSet ds = null;
			using (SqlCommand cmd = this.orderListDO.getSelectCommand(this.orderListSR.Criteria))
			{
				ds = this.consolidatedDA.GetDataSet(cmd, this.orderListSR.Security);
			}
			// Load the data object line items
			foreach (DataRow Row in ds.Tables[0].Rows)
			{
				// Create a new line item object
				OrderListLineItemDO lineItem = new OrderListLineItemDO();

				// Load the line item object
				lineItem.TransactionID = Row["TransactionID"].ToString();
				lineItem.TransactionAlias = Row["TransactionAlias"].ToString();
				lineItem.OrderStatus = Row["LookupTransactionStatusIndex"].ToString();

				lineItem.TransactionDate = Row["TransactionDate"].ToString();

				if (Row["TransactionDate"].GetType() == typeof(DateTimeOffset))
				{
					/* Preserve the DateTimeOffset object */
					lineItem.TransactionDateTime = DataObject.getValue<DateTimeOffset>(Row["TransactionDate"], TimeConverter.Today());
				}
				else
				{
					lineItem.TransactionDateTime = new DateTimeOffset();
				}

				if (Row["InventoryDate"] is DateTime)
				{
					lineItem.InventoryDateTime = (DateTime)Row["InventoryDate"];
				}
				else
				{
					lineItem.InventoryDateTime = new DateTime();
				}
				lineItem.InventoryDate = DateEfficacy.convertToMonthDayYear(lineItem.InventoryDateTime);

				lineItem.SupplierID = Row["SupplierID"].ToString();
				lineItem.Manager = Row["ManagerID"].ToString();
				lineItem.Owner = Row["OwnerID"].ToString();
				lineItem.BillToID = Row["BillToID"].ToString();
				lineItem.ShipperID = Row["ShipperID"].ToString();
				lineItem.ShipToID = Row["ShipToID"].ToString();
				lineItem.CarrierID = Row["CarrierID"].ToString();
				lineItem.DocumentNumber = Row["DocumentNumber"].ToString();
				lineItem.PONumber = Row["PONumber"].ToString();

				lineItem.ScheduledDate = Row["ScheduledDate"].ToString();

				if (Row["ScheduledDate"].GetType() == typeof(DateTimeOffset))
				{
					lineItem.ScheduledDateTime = DataObject.getValue<DateTimeOffset>(Row["ScheduledDate"], TimeConverter.Today());
				}
				else
				{
					lineItem.ScheduledDateTime = new DateTimeOffset();
				}

				lineItem.TransactionStatus = (TransactionStatus)Row["TransStatus"];

				lineItem.BillToName = Row["BillToName"] == null ? "" : Row["BillToName"].ToString();
				lineItem.BillToAddress = Row["BillToAddress"] == null ? "" : Row["BillToAddress"].ToString();
				lineItem.BillToCity = Row["BillToCity"] == null ? "" : Row["BillToCity"].ToString();
				lineItem.BillToState = Row["BillToState"] == null ? "" : Row["BillToState"].ToString();

				lineItem.ShipToName = Row["ShipToName"] == null ? "" : Row["ShipToName"].ToString();
				lineItem.ShipToAddress = Row["ShipToAddress"] == null ? "" : Row["ShipToAddress"].ToString();
				lineItem.ShipToCity = Row["ShipToCity"] == null ? "" : Row["ShipToCity"].ToString();
				lineItem.ShipToState = Row["ShipToState"] == null ? "" : Row["ShipToState"].ToString();

				lineItem.CarrierName = Row["CarrierName"] == null ? "" : Row["CarrierName"].ToString();
				lineItem.CarrierAddress = Row["CarrierAddress"] == null ? "" : Row["CarrierAddress"].ToString();
				lineItem.CarrierCity = Row["CarrierCity"] == null ? "" : Row["CarrierCity"].ToString();
				lineItem.CarrierState = Row["CarrierState"] == null ? "" : Row["CarrierState"].ToString();

				// Effective Date
				lineItem.EffectiveDate = Row["EffectiveDate"].ToString();

				if (Row["EffectiveDate"].GetType() == typeof(DateTimeOffset))
				{
					/* Preserve the DateTimeOffset object */
					lineItem.EffectiveDateTime = DataObject.getValue<DateTimeOffset>(Row["EffectiveDate"], TimeConverter.Today());
				}
				else
				{
					lineItem.EffectiveDateTime = new DateTimeOffset();
				}

				// Expiration Date
				lineItem.ExpirationDate = Row["ExpirationDate"].ToString();

				if (Row["ExpirationDate"].GetType() == typeof(DateTimeOffset))
				{
					/* Preserve the DateTimeOffset object */
					lineItem.ExpirationDateTime = DataObject.getValue<DateTimeOffset>(Row["ExpirationDate"], TimeConverter.Today());
				}
				else
				{
					lineItem.ExpirationDateTime = new DateTimeOffset();
				}

				// ETA
				lineItem.ETA = Row["ETA"].ToString();

				if (Row["ETA"].GetType() == typeof(DateTimeOffset))
				{
					lineItem.ETADateTime = DataObject.getValue<DateTimeOffset>(Row["ETA"], TimeConverter.Today());
				}
				else
				{
					lineItem.ETADateTime = new DateTimeOffset();
				}

				// Requested Delivery Date. Added 2008-Feb-14 IGO.
				lineItem.RequestedDeliveryDate = Row["RequestedDeliveryDate"].ToString();
				if (Row["RequestedDeliveryDate"].GetType() == typeof(DateTimeOffset))
				{
					lineItem.RequestedDeliveryDateTime = DataObject.getValue<DateTimeOffset>(Row["RequestedDeliveryDate"], TimeConverter.Today());
				}
				else
				{
					lineItem.RequestedDeliveryDateTime = new DateTimeOffset();
				}

				lineItem.ShipmentNumber = Row["ShipmentNumber"] == null ? "" : Row["ShipmentNumber"].ToString();
				lineItem.OperatorID = Row["OperatorID"] == null ? "" : Row["OperatorID"].ToString();
				lineItem.DestRegistrationID1 = Row["DestinationRegistrationID1"] == null ? "" : Row["DestinationRegistrationID1"].ToString();
				lineItem.DestRegistrationID2 = Row["DestinationRegistrationID2"] == null ? "" : Row["DestinationRegistrationID2"].ToString();
				lineItem.DestRegistrationID3 = Row["DestinationRegistrationID3"] == null ? "" : Row["DestinationRegistrationID3"].ToString();
				lineItem.UserData1 = Row["UserData1"] == null ? "" : Row["UserData1"].ToString();
				lineItem.UserData2 = Row["UserData2"] == null ? "" : Row["UserData2"].ToString();
				lineItem.UserData3 = Row["UserData3"] == null ? "" : Row["UserData3"].ToString();
				lineItem.UserData4 = Row["UserData4"] == null ? "" : Row["UserData4"].ToString();
				lineItem.UserData5 = Row["UserData5"] == null ? "" : Row["UserData5"].ToString();
				lineItem.UserData6 = Row["UserData6"] == null ? "" : Row["UserData6"].ToString();
				lineItem.UserData7 = Row["UserData7"] == null ? "" : Row["UserData7"].ToString();
				lineItem.UserData8 = Row["UserData8"] == null ? "" : Row["UserData8"].ToString();
				lineItem.UserData9 = Row["UserData9"] == null ? "" : Row["UserData9"].ToString();
				lineItem.UserData10 = Row["UserData10"] == null ? "" : Row["UserData10"].ToString();
				lineItem.UserData11 = Row["UserData11"] == null ? "" : Row["UserData11"].ToString();
				lineItem.UserData12 = Row["UserData12"] == null ? "" : Row["UserData12"].ToString();
				lineItem.UserData13 = Row["UserData13"] == null ? "" : Row["UserData13"].ToString();
				lineItem.UserData14 = Row["UserData14"] == null ? "" : Row["UserData14"].ToString();
				lineItem.UserData15 = Row["UserData15"] == null ? "" : Row["UserData15"].ToString();
				lineItem.UserData16 = Row["UserData16"] == null ? "" : Row["UserData16"].ToString();
				lineItem.UserData17 = Row["UserData17"] == null ? "" : Row["UserData17"].ToString();
				lineItem.UserData18 = Row["UserData18"] == null ? "" : Row["UserData18"].ToString();
				lineItem.UserData19 = Row["UserData19"] == null ? "" : Row["UserData19"].ToString();
				lineItem.UserData20 = Row["UserData20"] == null ? "" : Row["UserData20"].ToString();
				lineItem.UserData21 = Row["UserData21"] == null ? "" : Row["UserData21"].ToString();
				lineItem.UserData22 = Row["UserData22"] == null ? "" : Row["UserData22"].ToString();
				lineItem.UserData23 = Row["UserData23"] == null ? "" : Row["UserData23"].ToString();
				lineItem.UserData24 = Row["UserData24"] == null ? "" : Row["UserData24"].ToString();

				// Add it to the order list data object
				this.orderListDO.LineItems.Add(lineItem);
			}

			timer.Stop();
		}

		private ArrayList DetermineSites()
		{
			StopWatch timer = new StopWatch(StopWatch.Appnames.OrderEntry, "OrderListProcessor.DetermineSites()");

			// Get the sites info list and time it
			StopWatch timer3 = new StopWatch(StopWatch.Appnames.OrderEntry, "** SitesInfo");
			SitesInfoClass infoList = new SitesInfoClass();
			SiteInfoDO siteInfoDO = infoList.RefreshSiteInfo(this.orderListSR.Security);
			timer3.Stop();

			ArrayList siteArray = new ArrayList();

			// Add the current site
			siteArray.Add(this.orderListSR.Security.SiteGuid);

			// If the base Site is a group, enumerate all the member sites
			SiteCollectionClass siteCollection = siteInfoDO.EnumerateByParentSite(this.orderListSR.Security.SiteGuid);

			foreach (SiteClass site in siteCollection)
			{
				siteArray.Add(site.SiteGuid);
			}

			// Now enumerate the parent sites
			ArrayList groupsEvaluated = new ArrayList();

			//SiteCollection = Sites.Enumerate( this.orderListSR.Security );
			siteCollection = siteInfoDO.SiteCollection;

			foreach (SiteClass checkSite in siteCollection)
			{
				EnumerateParentSites(siteInfoDO, checkSite, groupsEvaluated, siteArray);
			}

			timer.Stop();

			return siteArray;
		}

		private void EnumerateParentSites(SiteInfoDO siteInfoDO, SiteClass site, ArrayList groupsEvaluated, ArrayList finalSiteList)
		{
			// Is the group we are looking at a Site Group?
			if (site.SiteGroup == true)
			{
				// Do we need to evaluate this group?
				foreach (Guid siteGuid in groupsEvaluated)
				{
					if (siteGuid == site.SiteGuid)
					{
						return;
					}
				}

				// Mark this one as having been evaluated
				groupsEvaluated.Add(site.SiteGuid);

				// Get the member sites of this group
				SiteCollectionClass Children = siteInfoDO.EnumerateByParentSite(site.SiteGuid);

				// Is our site a member of this group?
				foreach (SiteClass Child in Children)
				{
					// Is this a group?
					if (Child.SiteGroup == true)
					{
						this.EnumerateParentSites(siteInfoDO, Child, groupsEvaluated, finalSiteList);
					}

					if (Child.SiteGuid == this.orderListSR.Security.SiteGuid)
					{
						finalSiteList.Add(site.SiteGuid);
					}
				}
			}
		}

		private DataObject view()
		{
			return null;
		}

		private DataObject add()
		{
			return null;
		}

		private DataObject delete()
		{
			return null;
		}

		private DataObject modify()
		{
			return null;
		}
	}
}
