using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ServiceModel;
using System.Data;
using System.Collections;
using System.Data.SqlClient;
using System.Text;
using System.Diagnostics;
using System.Reflection;
using System.Security;
using System.Xml;

using FMBusinessObjects.DataObjects;
using FMBusinessObjects.BusinessInterfaces;
using FMBusinessObjects.ServiceRequests;
using FMBusinessServices.DataAccessLayer;
using FMBusinessObjects.Exceptions;
using FMBusinessServices.InternalClasses;
using FMBusinessObjects.UtilityObjects;

namespace FMBusinessServices.ServiceClasses
{
	[SecuritySafeCriticalAttribute]
	[ServiceBehavior(TransactionIsolationLevel = System.Transactions.IsolationLevel.ReadCommitted)]
	public class StandingOffersClass : IStandingOffers, IDependency
	{
		#region Protected data members
		private ConsolidatedDAClass consolidatedDA;
		#endregion

		#region Private data members
		private const string MsgSecurityNull = "Security is null";
		private const string MsgStandingOfferNull = "StandingOffer is null";
		private const string MsgStandingOfferExists = "Price List Entry Exists";
		private const string MsgStandingOfferNotFound = "Price List Entry Not Found";
		private const string MsgIDependencyObjectNull = "IDependency Object is null";
		private const string MsgStandingOfferExistsNoDelete = "Price List Entry record exists, cannot delete";
		private const string MsgMustSetSupplierAndProduct = "Must set Supplier and Product";
		#endregion

		#region Constructors
		/// <summary>
		/// This is the default constructor for the Standing Offers (aka Price List) Class.
		/// </summary>
		public StandingOffersClass()
		{
			this.consolidatedDA = new ConsolidatedDAClass();
		}
		#endregion

		#region Public methods
		/// <summary>
		/// This method will add a new price list entry (aka standing offer) record to the database. It will
		/// throw an exception on any error.
		/// </summary>
		/// <param name="security"></param>
		/// <param name="standingOffer"></param>
		/// <returns></returns>
		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public Guid Add(SecurityClass security, StandingOfferClass standingOffer)
		{
			// Validate the security and price list entry (aka standing offer) objects
			if (security == null)
			{
				throw new ArgumentNullException(StandingOffersClass.MsgSecurityNull);
			}

			if (standingOffer == null)
			{
				throw new ArgumentNullException(StandingOffersClass.MsgStandingOfferNull);
			}

			// Ensure that the user has the modify rights.
			if (security.HasRight(RIGHT.MODIFY_STANDING_OFFERS) == false)
			{
				throw new FMInsufficientRightsException();
			}

			standingOffer.ID = BuildIDUsingGuids(security,
															standingOffer.SupplierGuid,
															standingOffer.ProductGuid,
															standingOffer.LocationGuid,
															standingOffer.EffectiveDate,
															standingOffer.ExpirationDate,
															standingOffer.LowerBound,
															standingOffer.UpperBound);

			// Throw an exception if there is no ID.
			if (String.IsNullOrWhiteSpace(standingOffer.ID))
			{
				throw (new Exception("Price List Entry has no ID"));
			}

			// Throw an exception of the same record exists.
			if (this.GetIdentityGuid(security, standingOffer.ID) != Guid.Empty)
			{
				throw (new Exception(StandingOffersClass.MsgStandingOfferExists));
			}

			standingOffer.SiteGuid = security.SiteGuid;
			standingOffer.CreatedDate = DateTimeOffset.Now;
			standingOffer.CreatedBy = security.UserID;
			standingOffer.UpdatedDate = DateTimeOffset.Now;
			standingOffer.UpdatedBy = security.UserID;
			standingOffer.IdentityGuid = Guid.NewGuid();

			using (SqlCommand cmd = new SqlCommand())
			{
				standingOffer.InsertSQL(cmd);
				consolidatedDA.ExecuteQuery(security, cmd);
			}

			// Create Entity to Site Map
			EntityToSiteMaps EntityToSiteMaps = new EntityToSiteMaps();
			EntityToSiteMapClass EntityToSiteMap = new EntityToSiteMapClass(standingOffer);
			EntityToSiteMaps.Add(security, EntityToSiteMap, GetType().GUID);

			return standingOffer.IdentityGuid;
		}

		/// <summary>
		/// This method updates the price list entry (aka standing offer) information in the database.
		/// </summary>
		/// <param name="Security"></param>
		/// <param name="User"></param>
		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void Modify(SecurityClass security, StandingOfferClass standingOffer)
		{
			// Ensure the security and price list entry (aka standing offer) objects are valid.
			if (security == null)
			{
				throw new ArgumentNullException(StandingOffersClass.MsgSecurityNull);
			}

			if (standingOffer == null)
			{
				throw new ArgumentNullException(StandingOffersClass.MsgStandingOfferNull);
			}

			// Ensure user had modify rights.
			if (!security.HasRight(RIGHT.MODIFY_STANDING_OFFERS) && !security.HasRight(RIGHT.EXECUTE_IMPORT_EXPORT))
			{
				throw new FMInsufficientRightsException();
			}

			standingOffer.ID = BuildIDUsingGuids(security,
															standingOffer.SupplierGuid,
															standingOffer.ProductGuid,
															standingOffer.LocationGuid,
															standingOffer.EffectiveDate,
															standingOffer.ExpirationDate,
															standingOffer.LowerBound,
															standingOffer.UpperBound);

			// Ensure that the price list entry (aka standing offer) object does not update
			// another price list entry (aka standing offer) record.
			Guid identityGuid = GetIdentityGuid(security, standingOffer.ID);
			if ((identityGuid != Guid.Empty) && (identityGuid != standingOffer.IdentityGuid))
			{
				throw (new Exception(StandingOffersClass.MsgStandingOfferExists));
			}

			// Ensure that the existing price list entry (aka standing offer) exists.
			StandingOfferClass oldClass = Get(security, standingOffer.IdentityGuid);
			if (oldClass.IdentityGuid == Guid.Empty)
			{
				throw (new Exception(StandingOffersClass.MsgStandingOfferNotFound));
			}

			standingOffer.UpdatedDate = DateTimeOffset.Now;
			standingOffer.UpdatedBy = security.UserID;
			using (SqlCommand cmd = new SqlCommand())
			{
				standingOffer.UpdateSQL(cmd);
				this.consolidatedDA.ExecuteQuery(security, cmd);
			}

			// Get the entity to site map collection.
			EntityToSiteMaps entityToSiteMaps = new EntityToSiteMaps();
			EntityToSiteMapCollectionClass entityToSiteMapCollection =
										 entityToSiteMaps.EnumerateByTypeIDAndGuid(security,
																					standingOffer.EntityType,
																					standingOffer.IdentityGuid);

			// If the new updated price list entry (aka standing offer) does not match the previous one, the purge
			// from the entity to site map.
			if (standingOffer.SiteGuid != oldClass.SiteGuid)
			{
				// Purge from EntityToSiteMap
				foreach (EntityToSiteMapClass entityToSiteMap in entityToSiteMapCollection)
				{
					entityToSiteMap.ID = standingOffer.ID;
					entityToSiteMaps.Purge(security, entityToSiteMap);
				}

				// Create Entity to Site Map
				EntityToSiteMapClass newEntityToSiteMap = new EntityToSiteMapClass(standingOffer);
				entityToSiteMaps.Add(security, newEntityToSiteMap, GetType().GUID);
			}
			else
			{
				// Verify that new ID will not conflict with EntityToSiteMaps
				foreach (EntityToSiteMapClass entityToSiteMap in entityToSiteMapCollection)
				{
					Guid siteGuid = security.SiteGuid;
					security.SiteGuid = entityToSiteMap.SiteGuid;
					identityGuid = this.GetIdentityGuid(security, standingOffer.ID);
					security.SiteGuid = siteGuid;

					if ((identityGuid != Guid.Empty) && (identityGuid != entityToSiteMap.IdentityGuid))
					{
						throw (new Exception(StandingOffersClass.MsgStandingOfferExists));
					}
				}
			}
		}

		/// <summary>
		/// This method will return the standing offer (aka price list) class given the security object
		/// and identity Guid.
		/// </summary>
		/// <param name="Security"></param>
		/// <param name="standingOfferGuid"></param>
		/// <returns></returns>
		public StandingOfferClass Get(SecurityClass security, Guid standingOfferGuid)
		{
			if (security == null)
			{
				throw new ArgumentNullException(StandingOffersClass.MsgSecurityNull);
			}

			StandingOfferClass standingOffer = new StandingOfferClass();
			standingOffer.IdentityGuid = standingOfferGuid;
			using (SqlCommand cmd = new SqlCommand())
			{
				standingOffer.SelectSQL(cmd, ContextUtil.IsInTransaction);
				standingOffer.Load(this.consolidatedDA.GetDataSet(cmd, security));
			}

			return standingOffer;
		}

		/// <summary>
		/// This method will return the object's identity Guid given an ID. It will
		/// return an empty Guid if not found.
		/// </summary>
		/// <param name="security"></param>
		/// <param name="standingOfferID"></param>
		/// <returns></returns>
		public Guid GetIdentityGuid(SecurityClass security, string standingOfferID)
		{
			StandingOfferClass standingOffer = GetByID(security, standingOfferID);

			return standingOffer.IdentityGuid;
		}

		/// <summary>
		/// Constructs an ID based on supplier Guid, product Guid, location Guid,
		/// effective date and expiration date. 
		/// </summary>
		/// <param name="supplierGuid"></param>
		/// <param name="productGuid"></param>
		/// <param name="locationGuid"></param>
		/// <param name="effectiveDate"></param>
		/// <param name="expirationDate"></param>
		/// <param name="lowerBound"></param>
		/// <param name="upperBound"></param>
		/// <returns></returns>
		public string BuildIDUsingGuids(SecurityClass security,
												Guid supplierGuid,
												Guid productGuid,
												Guid locationGuid,
												DateTimeOffset effectiveDate,
												DateTimeOffset expirationDate,
												int lowerBound,
												int upperBound)
		{
			if ((supplierGuid == Guid.Empty) || (productGuid == Guid.Empty))
			{
				throw new Exception(StandingOffersClass.MsgMustSetSupplierAndProduct);
			}

			StandingOfferClass standingOffer = new StandingOfferClass();
			standingOffer.SiteGuid = security.SiteGuid;
			standingOffer.SupplierGuid = supplierGuid;
			standingOffer.ProductGuid = productGuid;
			standingOffer.LocationGuid = locationGuid;
			standingOffer.EffectiveDate = effectiveDate;
			standingOffer.ExpirationDate = expirationDate;
			standingOffer.LowerBound = lowerBound;
			standingOffer.UpperBound = upperBound;

			ConsolidatedDAClass dal = new ConsolidatedDAClass();
			using (SqlCommand cmd = new SqlCommand())
			{
				standingOffer.BuildIDSQLUsingGuids(cmd);
				DataSet dataSet = dal.GetDataSet(cmd, security);

				if (dataSet == null)
				{
					return "unassigned";
				}
				else
				{
					DataTable table = dataSet.Tables[0];

					if (table.Rows.Count == 0)
					{
						return "unassigned";
					}
					DataRow row = table.Rows[0];
					return row["ID"] as string;
				}
			}
		}

		/// <summary>
		/// Constructs an ID based on site Guid, supplier ID, product ID, location ID,
		/// effective date and expiration date. 
		/// </summary>
		/// <param name="security"></param>
		/// <param name="supplierID"></param>
		/// <param name="productID"></param>
		/// <param name="locationID"></param>
		/// <param name="effectiveDate"></param>
		/// <param name="expirationDate"></param>
		/// <param name="lowerBound"></param>
		/// <param name="upperBound"></param>
		/// <returns></returns>
		public string BuildIDUsingIDs(SecurityClass security,
										string supplierID,
										string productID,
										string locationID,
										DateTimeOffset effectiveDate,
										DateTimeOffset expirationDate,
										int lowerBound,
										int upperBound)
		{
			if (String.IsNullOrWhiteSpace(supplierID) || String.IsNullOrWhiteSpace(productID))
			{
				throw new Exception(StandingOffersClass.MsgMustSetSupplierAndProduct);
			}

			StandingOfferClass standingOffer = new StandingOfferClass();
			standingOffer.SiteGuid = security.SiteGuid;
			standingOffer.SupplierID = supplierID;
			standingOffer.ProductID = productID;
			standingOffer.LocationID = locationID;
			standingOffer.EffectiveDate = effectiveDate;
			standingOffer.ExpirationDate = expirationDate;
			standingOffer.LowerBound = lowerBound;
			standingOffer.UpperBound = upperBound;

			ConsolidatedDAClass consolidatedDA = new ConsolidatedDAClass();
			using (SqlCommand cmd = new SqlCommand())
			{
				standingOffer.BuildIDSQLUsingIDs(cmd);
				DataSet dataSet = consolidatedDA.GetDataSet(cmd, security);

				if (dataSet == null)
				{
					return "unassigned";
				}
				else
				{
					DataTable table = dataSet.Tables[0];

					if (table.Rows.Count == 0)
					{
						return "unassigned";
					}

					DataRow row = table.Rows[0];
					return row["ID"] as string;
				}
			}
		}

		/// <summary>
		/// This method will return the price list entry (aka standing offer) object for either the product and 
		/// current period combination or just the product. It will return null if no
		/// price list entry (aka standing offer) is found.
		/// </summary>
		/// <param name="security"></param>
		/// <param name="productGuid"></param>
		/// <returns></returns>
		public StandingOfferClass GetUsingProduct(SecurityClass security, Guid productGuid, DateTimeOffset currentPeriod)
		{
			bool found = true;

			StandingOfferClass standingOffer = new StandingOfferClass();
			using (SqlCommand cmd = new SqlCommand())
			{
				standingOffer.GetIdentityGuidSQL(cmd, security, productGuid, currentPeriod);

				DataSet dataSet = this.consolidatedDA.GetDataSet(cmd, security);
				DataTable table = null;

				if (dataSet == null)
				{
					found = false;
				}
				else
				{
					table = dataSet.Tables[0];

					if (table.Rows.Count == 0)
					{
						found = false;
					}
				}

				if (found == true)
				{
					DataRow row = table.Rows[0];
					Guid standingOfferGuid = DataObject.getValue<Guid>(row["StandingOfferGuid"], Guid.Empty);

					standingOffer = this.Get(security, standingOfferGuid);
				}
				else
				{
					using (SqlCommand cmd2 = new SqlCommand())
					{
						standingOffer.GetIdentityGuidSQL(cmd2, security, productGuid, null);
						dataSet = this.consolidatedDA.GetDataSet(cmd2, security);

						if (dataSet == null)
						{
							return null;
						}

						table = dataSet.Tables[0];

						if (table.Rows.Count == 0)
						{
							return null;
						}

						DataRow row = table.Rows[0];
						Guid standingOfferGuid = DataObject.getValue<Guid>(row["StandingOfferGuid"], Guid.Empty);

						standingOffer = this.Get(security, standingOfferGuid);
					}
				}

				return standingOffer;
			}
		}

		/// <summary>
		/// This method will return the object's identity guid given an ID. It will return
		/// an empty Guid if not found.
		/// </summary>
		/// <param name="security"></param>
		/// <param name="supplierID"></param>
		/// <param name="productID"></param>
		/// <returns></returns>
		public Guid GetIdentityGuidUsingProduct(SecurityClass security, Guid supplierGuid, Guid productGuid)
		{
			return this.GetIdentityGuidUsingLocation(security, supplierGuid, productGuid, Guid.Empty);
		}

		/// <summary>
		/// This method will return the object's identity Guid given supplier Guid, 
		/// product Guid, and location Guid . It will return
		/// an empty Guid if not found.
		/// </summary>
		/// <param name="security"></param>
		/// <param name="supplierGuid"></param>
		/// <param name="productGuid"></param>
		/// <param name="locationGuid"></param>
		/// <returns></returns>
		public Guid GetIdentityGuidUsingLocation(SecurityClass security, Guid supplierGuid, Guid productGuid, Guid locationGuid)
		{
			StandingOfferClass standingOffer = new StandingOfferClass();
			using (SqlCommand cmd = new SqlCommand())
			{
				standingOffer.GetIdentityGuidSQL(cmd, security, supplierGuid, productGuid, locationGuid, null);

				DataSet dataSet = this.consolidatedDA.GetDataSet(cmd, security);

				if (dataSet == null)
				{
					return Guid.Empty;
				}

				DataTable table = dataSet.Tables[0];

				if (table.Rows.Count == 0)
				{
					return Guid.Empty;
				}

				DataRow row = table.Rows[0];
				Guid standingOfferGuid = DataObject.getValue<Guid>(row["StandingOfferGuid"], Guid.Empty);

				return standingOfferGuid;
			}
		}

		/// <summary>
		/// This method will return the price list entry (aka standing offer) Guid based on the supplier Guid,
		/// product Guid, and current period. It will return an empty Guid if not found.
		/// </summary>
		/// <param name="security"></param>
		/// <param name="standingOfferID"></param>
		/// <returns></returns>
		/// 
		public Guid GetIdentityGuidUsingPeriod(SecurityClass security, Guid supplierGuid, Guid productGuid, DateTimeOffset currentPeriod)
		{
			Guid standingOfferGuid = this.GetIdentityGuidUsingLocationPeriod(security, supplierGuid, productGuid, Guid.Empty, currentPeriod);
			return standingOfferGuid;
		}

		/// <summary>
		/// This method will return the price list entry (aka standing offer) Guid based on the supplier Guid,
		/// product Guid, location Guid, and current period. It will return an empty Guid
		/// if not found.
		/// </summary>
		/// <param name="security"></param>
		/// <param name="supplierGuid"></param>
		/// <param name="productGuid"></param>
		/// <param name="locationGuid"></param>
		/// <param name="currentPeriod"></param>
		/// <returns></returns>
		public Guid GetIdentityGuidUsingLocationPeriod(SecurityClass security, Guid supplierGuid, Guid productGuid,
																		Guid locationGuid, DateTimeOffset currentPeriod)
		{
			bool mostRecent = false;
			double? quantity = null;

			Guid standingOfferGuid = this.GetIdentityGuidUsingMostRecent(security, supplierGuid, productGuid, locationGuid,
																								currentPeriod, quantity, mostRecent);
			return standingOfferGuid;
		}

		public Guid GetIdentityGuidUsingQuantity(SecurityClass security, Guid supplierGuid, Guid productGuid, Guid locationGuid,
																DateTimeOffset currentPeriod, double? quantity)
		{
			bool mostRecent = false;
			Guid standingOfferGuid = this.GetIdentityGuidUsingMostRecent(security, supplierGuid, productGuid, locationGuid,
																								currentPeriod, quantity, mostRecent);
			return standingOfferGuid;
		}

		/// <summary>
		/// This method will return the price list entry (aka standing offer) Guid based on the supplier Guid,
		/// product Guid, location Guid, current period, and quanity. It will return an empty Guid
		/// if not found.
		/// </summary>
		/// <param name="security"></param>
		/// <param name="supplierGuid"></param>
		/// <param name="productGuid"></param>
		/// <param name="locationGuid"></param>
		/// <param name="currentPeriod"></param>
		/// <param name="quantity"></param>
		/// <param name="mostRecent"></param>
		/// <returns></returns>
		public Guid GetIdentityGuidUsingMostRecent(SecurityClass security, Guid supplierGuid, Guid productGuid, Guid locationGuid,
																	DateTimeOffset currentPeriod, double? quantity, bool mostRecent)
		{
			StandingOfferClass standingOffer = new StandingOfferClass();

			using (SqlCommand cmd = new SqlCommand())
			{
				standingOffer.GetIdentityGuidSQL(cmd,
													security,
													supplierGuid,
													productGuid,
													locationGuid,
													currentPeriod,
													quantity,
													mostRecent);

				DataSet dataSet = this.consolidatedDA.GetDataSet(cmd, security);

				if (dataSet == null)
				{
					return Guid.Empty;
				}

				DataTable table = dataSet.Tables[0];

				if (table.Rows.Count == 0)
				{
					return Guid.Empty;
				}

				DataRow row = table.Rows[0];
				Guid standingOfferGuid = DataObject.getValue<Guid>(row["StandingOfferGuid"], Guid.Empty);

				return standingOfferGuid;
			}
		}

		/// <summary>
		/// This method will return the price list entry (aka standing offer) object given the security object and
		/// the price list entry (aka standing offer) ID.
		/// </summary>
		/// <param name="security"></param>
		/// <param name="standingOfferID"></param>
		/// <returns></returns>
		public StandingOfferClass GetByID(SecurityClass security, string standingOfferID)
		{
			if (security == null)
			{
				throw new ArgumentNullException(StandingOffersClass.MsgSecurityNull);
			}

			StandingOfferClass standingOffer = new StandingOfferClass();

			standingOffer.ID = standingOfferID;
			standingOffer.SiteGuid = security.SiteGuid;

			using (SqlCommand cmd = new SqlCommand())
			{
				standingOffer.SelectByIDSQL(cmd, security, ContextUtil.IsInTransaction);
				standingOffer.Load(this.consolidatedDA.GetDataSet(cmd, security));

				return standingOffer;
			}
		}

		/// <summary>
		/// This method will return true if the price list entry (aka standing offer) is overlapping an existing price list entry (aka standing offer).
		/// Otherwise, it will return false.
		/// </summary>
		/// <param name="security"></param>
		/// <param name="standingOffer"></param>
		/// <returns></returns>
		public bool IsStandingOfferOverlapping(SecurityClass security, StandingOfferClass standingOffer)
		{
			bool isOverlapping = false;

			using (SqlCommand cmd = new SqlCommand())
			{
				standingOffer.SelectOverlapSQL(cmd, false);
				DataSet dataSet = this.consolidatedDA.GetDataSet(cmd, security);
				isOverlapping = standingOffer.LoadOverlap(dataSet);

				return isOverlapping;
			}
		}

		/// <summary>
		/// This method will purge a price list entry (aka standing offer) record from the database.
		/// </summary>
		/// <param name="security"></param>
		/// <param name="standingOfferGuid"></param>
		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void Purge(SecurityClass security, Guid standingOfferGuid)
		{
			if (security == null)
			{
				throw new ArgumentNullException(StandingOffersClass.MsgSecurityNull);
			}

			if (security.HasRight(RIGHT.MODIFY_STANDING_OFFERS) == false)
			{
				throw new FMInsufficientRightsException();
			}

			StandingOfferClass standingOffer = Get(security, standingOfferGuid);
			if (standingOffer.IdentityGuid == Guid.Empty)
			{
				throw new Exception(StandingOffersClass.MsgStandingOfferNotFound);
			}

			// Purge from EntityToSiteMap
			EntityToSiteMaps entityToSiteMaps = new EntityToSiteMaps();
			EntityToSiteMapCollectionClass entityToSiteMapCollection =
							entityToSiteMaps.EnumerateByTypeIDAndGuid(security, standingOffer.EntityType, standingOfferGuid);

			foreach (EntityToSiteMapClass entityToSiteMap in entityToSiteMapCollection)
			{
				entityToSiteMaps.Purge(security, entityToSiteMap);
			}

			using (SqlCommand cmd = new SqlCommand())
			{
				standingOffer.PurgeSQL(cmd);
				this.consolidatedDA.ExecuteQuery(security, cmd);
			}
		}

		/// <summary>
		/// This method will return a list of price list entry (aka standing offer) objects.
		/// </summary>
		/// <param name="security"></param>
		/// <returns></returns>
		public StandingOfferCollectionClass Enumerate(SecurityClass security)
		{
			if (security == null)
			{
				throw new ArgumentNullException(StandingOffersClass.MsgSecurityNull);
			}

			if (!security.HasRight(RIGHT.VIEW_STANDING_OFFERS) &&
				!security.HasRight(RIGHT.MODIFY_STANDING_OFFERS) &&
				!security.HasRight(RIGHT.MODIFY_PRODUCTS) &&
				!security.HasRight(RIGHT.MODIFY_SITES_AND_SITE_GROUPS) &&
				!security.HasRight(RIGHT.MODIFY_COMPANY_DATA) &&
				!security.HasRight(RIGHT.IMPORT_ENTERPRISE_DATA) &&
				!security.HasRight(RIGHT.EXECUTE_IMPORT_EXPORT))
			{
				throw new FMInsufficientRightsException();
			}

			StandingOfferClass standingOffer = new StandingOfferClass();
			using (SqlCommand cmd = new SqlCommand())
			{
				standingOffer.EnumerateSQL(cmd, security);
				DataSet dataSet = this.consolidatedDA.GetDataSet(cmd, security);
				StandingOfferCollectionClass standingOfferCollection = new StandingOfferCollectionClass();

				DataTable table = dataSet.Tables[0];
				while (table.Rows.Count != 0)
				{
					standingOffer = new StandingOfferClass();
					standingOffer.Load(dataSet);
					standingOfferCollection.Add(standingOffer);
					table.Rows.RemoveAt(0);
				}

				return standingOfferCollection;
			}
		}

		/// <summary>
		/// This method will return a list of price list entry (aka standing offer) objects that matches the filter and
		/// site.
		/// </summary>
		/// <param name="security"></param>
		/// <returns></returns>
		public StandingOfferCollectionClass EnumerateWithFilter(SecurityClass security, StandingOfferFilterClass filter)
		{
			if (security == null)
			{
				throw new ArgumentNullException(StandingOffersClass.MsgSecurityNull);
			}

			if (!security.HasRight(RIGHT.VIEW_STANDING_OFFERS) &&
				!security.HasRight(RIGHT.MODIFY_STANDING_OFFERS) &&
				!security.HasRight(RIGHT.MODIFY_FINANCIAL_DATA) &&
				!security.HasRight(RIGHT.VIEW_FINANCIAL_DATA))
			{
				throw new FMInsufficientRightsException();
			}

			StandingOfferClass standingOffer = new StandingOfferClass();
			EnumerationLimits limits = new EnumerationLimits();
			int limit = limits.GetLimit(EnumerationLimits.EnumerationOptions.STANDING_OFFER);

			using (SqlCommand cmd = new SqlCommand())
			{
				standingOffer.EnumerateSQLWithFilter(cmd, security, filter, limit);
				DataSet dataSet = this.consolidatedDA.GetDataSet(cmd, security);
				StandingOfferCollectionClass standingOfferCollection = new StandingOfferCollectionClass();

				DataTable table = dataSet.Tables[0];
				while (table.Rows.Count != 0)
				{
					standingOffer = new StandingOfferClass();
					standingOffer.Load(dataSet);
					standingOfferCollection.Add(standingOffer);
					table.Rows.RemoveAt(0);
				}

				return standingOfferCollection;
			}
		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void ImportWithXML(SecurityClass security, string Xml)
		{
			bool getSchedulesFlag = false;

			if (security == null)
			{
				throw new ArgumentNullException(StandingOffersClass.MsgSecurityNull);
			}

			if (Xml == null)
			{
				throw new ArgumentNullException("Xml");
			}

			if (!security.HasRight(RIGHT.MODIFY_STANDING_OFFERS))
			{
				throw new FMInsufficientRightsException();
			}

			XmlDocument Document = new XmlDocument();
			Document.LoadXml(Xml);

			SitesClass Sites = new SitesClass();
			SiteClass Site = Sites.Get(security, security.SiteGuid, getSchedulesFlag);

			XmlNode standingOfferNode = Document.ChildNodes[0];

			if (standingOfferNode != null && standingOfferNode.Name == "StandingOffer")
			{
				StandingOfferClass standingOffer = new StandingOfferClass();

				try
				{
					standingOffer.Load(standingOfferNode);
					this.ImportWithStandingOffer(security, standingOffer);
				}
				catch
				{
				}
			}
		}

		/// <summary>
		/// Import price list entry (aka standing offer) data from excel worksheet.
		/// </summary>
		/// <param name="Security"></param>
		/// <param name="standingOffer"></param>
		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void ImportWithStandingOffer(SecurityClass security, StandingOfferClass standingOffer)
		{
			if (security == null)
			{
				throw new ArgumentNullException(StandingOffersClass.MsgSecurityNull);
			}

			if (standingOffer == null)
			{
				throw new ArgumentNullException(StandingOffersClass.MsgStandingOfferNull);
			}


			standingOffer.ID = BuildIDUsingIDs(security,
												standingOffer.SupplierID,
												standingOffer.ProductID,
												standingOffer.LocationID,
												standingOffer.EffectiveDate,
												standingOffer.ExpirationDate,
												standingOffer.LowerBound,
												standingOffer.UpperBound);


			standingOffer.IdentityGuid = this.GetByID(security, standingOffer.ID).IdentityGuid;

			if (standingOffer.IdentityGuid != Guid.Empty && Get(security, standingOffer.IdentityGuid).SiteGuid != security.SiteGuid)
			{
				return;
			}

			if (standingOffer.SupplierID != "{Unassigned}" && standingOffer.SupplierID != "")
			{
				if (standingOffer.SupplierGuid == Guid.Empty)
				{
					CompaniesClass companies = new CompaniesClass();
					standingOffer.SupplierGuid = companies.GetIdentityGuid(security, standingOffer.SupplierID);
				}
			}

			if (standingOffer.ProductID != "{Unassigned}" && standingOffer.ProductID != "")
			{
				if (standingOffer.ProductGuid == Guid.Empty)
				{
					ProductsClass products = new ProductsClass();
					standingOffer.ProductGuid = products.GetMasterRecordGuidFromID(security, standingOffer.ProductID);
				}
			}

			if (standingOffer.LocationID != "{Unassigned}" && standingOffer.LocationID != "")
			{
				if (standingOffer.LocationGuid == Guid.Empty)
				{
					IATACodesClass locations = new IATACodesClass();
					standingOffer.LocationGuid = locations.GetIdentityGuid(security, standingOffer.LocationID);
				}
			}

			if (standingOffer.IdentityGuid != Guid.Empty)
			{
				this.Modify(security, standingOffer);
			}
			else
			{
				this.Add(security, standingOffer);
			}
		}
		#endregion

		#region Handle dependencies
		/// <summary>
		/// This method will insert a dependency on for the price list entry (aka standing offer) object.
		/// </summary>
		/// <param name="security"></param>
		/// <param name="inObject"></param>
		void IDependency.Insert(SecurityClass security, BaseDataObject inObject, bool preOperation)
		{
			if (security == null)
			{
				throw new ArgumentNullException(StandingOffersClass.MsgSecurityNull);
			}

			if (inObject == null)
			{
				throw new ArgumentNullException(StandingOffersClass.MsgIDependencyObjectNull);
			}

			if (preOperation && typeof(EntityToSiteMapClass).IsInstanceOfType(inObject) == true)
			{
				EntityToSiteMapClass entityToSiteMap = (EntityToSiteMapClass)inObject;

				if ( entityToSiteMap.TypeID != ENTITY_TYPE.STANDING_OFFER )
				{
					return;
				}

				if (GetIdentityGuid(security, entityToSiteMap.ID) != Guid.Empty)
				{
					throw (new Exception(StandingOffersClass.MsgStandingOfferExists));
				}
			}
		}

		/// <summary>
		/// This method will update a dependency on for the price list entry (aka standing offer) object.
		/// </summary>
		/// <param name="security"></param>
		/// <param name="inObject"></param>
		void IDependency.Update(SecurityClass security, BaseDataObject inObject)
		{
			if (security == null)
			{
				throw new ArgumentNullException(StandingOffersClass.MsgSecurityNull);
			}

			if (inObject == null)
			{
				throw new ArgumentNullException(StandingOffersClass.MsgIDependencyObjectNull);
			}

			if (typeof(SiteClass).IsInstanceOfType(inObject) == true)
			{
				SiteClass site = (SiteClass)inObject;
				StandingOfferCollectionClass standingOfferCollection = this.Enumerate(security);
				EntityToSiteMaps entityToSiteMaps = new EntityToSiteMaps();

				foreach (StandingOfferClass standingOffer in standingOfferCollection)
				{
					if (site.SiteGuid == standingOffer.SiteGuid)
					{
						EntityToSiteMapCollectionClass entityToSiteMapCollection =
												 entityToSiteMaps.EnumerateByTypeIDAndGuid(security,
																							standingOffer.EntityType,
																							standingOffer.IdentityGuid);

						foreach (EntityToSiteMapClass entityToSiteMap in entityToSiteMapCollection)
						{
							if (entityToSiteMap.SiteGuid != site.SiteGuid)
							{
								entityToSiteMaps.Purge(security, entityToSiteMap);
							}
						}
					}
				}
			}
		}

		/// <summary>
		/// This method will purge a dependency on for the price list entry (aka standing offer) object.
		/// </summary>
		/// <param name="security"></param>
		/// <param name="inObject"></param>
		void IDependency.Purge(SecurityClass security, BaseDataObject inObject)
		{
			if (security == null)
			{
				throw new ArgumentNullException(StandingOffersClass.MsgSecurityNull);
			}

			if (inObject == null)
			{
				throw new ArgumentNullException(StandingOffersClass.MsgIDependencyObjectNull);
			}

			// Purge Standing Offers (aka Price List)
			if (typeof(SiteClass).IsInstanceOfType(inObject) == true)
			{
				SiteClass site = (SiteClass)inObject;
				StandingOfferCollectionClass standingOfferCollection = this.Enumerate(security);
				EntityToSiteMaps entityToSiteMaps = new EntityToSiteMaps();

				foreach (StandingOfferClass standingOffer in standingOfferCollection)
				{
					if (site.SiteGuid == standingOffer.SiteGuid)
					{
						this.Purge(security, standingOffer.IdentityGuid);
					}
					else
					{
						EntityToSiteMapClass entityToSiteMap = new EntityToSiteMapClass(standingOffer);
						entityToSiteMap.SiteGuid = site.SiteGuid;
						entityToSiteMaps.Purge(security, entityToSiteMap);
					}
				}
			}

			// Throw an exception if a price list entry (aka standing offer) record has an identity Guid that matches the
			// the company entity being deleted.
			if (typeof(CompanyClass).IsInstanceOfType(inObject) == true)
			{
				CompanyClass company = (CompanyClass)inObject;
				StandingOfferCollectionClass standingOfferCollection = this.Enumerate(security);

				foreach (StandingOfferClass standingOffer in standingOfferCollection)
				{
					if ((standingOffer.SupplierGuid == company.MasterRecordGuid) &&
							(standingOffer.SiteGuid == company.SiteGuid))
					{
						throw new Exception(StandingOffersClass.MsgStandingOfferExistsNoDelete + " ID:" + standingOffer.StandingOfferID);
					}
				}
			}


			// Throw an exception if a price list entry (aka standing offer) record has an identity Guid that matches the
			// the product entity being deleted.
			if (typeof(ProductClass).IsInstanceOfType(inObject) == true)
			{
				ProductClass product = (ProductClass)inObject;
				StandingOfferCollectionClass standingOfferCollection = this.Enumerate(security);

				foreach (StandingOfferClass standingOffer in standingOfferCollection)
				{
					if ((standingOffer.ProductGuid == product.MasterRecordGuid) &&
						(standingOffer.SiteGuid == product.SiteGuid))
					{
						throw new Exception(StandingOffersClass.MsgStandingOfferExistsNoDelete + " ID:" + standingOffer.StandingOfferID);
					}
				}
			}

			// Throw an exception if a price list entry (aka standing offer) record has an identity Guid that matches the
			// the IATA entity being deleted.
			if (typeof(IATACodeClass).IsInstanceOfType(inObject) == true)
			{
				IATACodeClass iata = (IATACodeClass)inObject;
				StandingOfferCollectionClass standingOfferCollection = this.Enumerate(security);

				foreach (StandingOfferClass standingOffer in standingOfferCollection)
				{
					if ((standingOffer.LocationGuid == iata.IdentityGuid) &&
						(standingOffer.SiteGuid == iata.SiteGuid))
					{
						throw new Exception(StandingOffersClass.MsgStandingOfferExistsNoDelete + " ID:" + standingOffer.StandingOfferID);
					}
				}
			}
		}
		#endregion
	}
}
