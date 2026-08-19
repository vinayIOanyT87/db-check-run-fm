using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Accounting;
using ADOFMSImport.Transformers.Interfaces;
using ADOFMSImport.DataObjects;
using ADOFMSImport.Parsers;
using FMBusinessObjects.DataObjects;
using FMBusinessObjects.BusinessInterfaces;
using FMBusinessObjects.ChannelFactories;
using FMBusinessObjects.ServiceRequests;

namespace ADOFMSImport.Transformers
{
	public class IssuesTransformer : Transformer, ITransformer
	{
		#region Attributes
		protected Defaults m_defaults;
		#endregion

		#region Construction
		public IssuesTransformer ( Defaults a_defaults )
			: base ( )
		{
			m_defaults = a_defaults;
		}
		#endregion // Construction

		#region ITransformer members
		public override bool Transform ( CSVObject a_csv )
		{
			bool result = true;

			IssuesObject issues = a_csv as IssuesObject;
			if (issues == null)
			{
				throw new Exception ( "Cannot transform issues because input object was of type " + a_csv.GetType ( ).ToString ( ) );
			}

			// prepare security
			SecurityClass security = new SecurityClass ( );
			foreach (RIGHT right in Enum.GetValues ( typeof ( RIGHT ) ))
			{
				security.RightCollection.Add ( right );
			}
			security.SiteID = "JFLA";
			security.SiteIndex = 3;
			security.UserID = "administrator";
			security.UserIndex = 1;

			// prepare common FM I/O
			Hashtable acctSiteTable = new Hashtable ( );

			// prepare lookup classes
			FMChannelFactory<ISites> sitesClient = new FMChannelFactory<ISites> ( );
			ISites siteLookup = sitesClient.CreateProxy ( );

			FMChannelFactory<ICompanies> companiesClient = new FMChannelFactory<ICompanies> ( );
			ICompanies companyLookup = companiesClient.CreateProxy ( );

			FMChannelFactory<IProducts> productsClient = new FMChannelFactory<IProducts> ( );
			IProducts productLookup = productsClient.CreateProxy ( );

			FMChannelFactory<IEquipments> equipClient = new FMChannelFactory<IEquipments> ( );
			IEquipments equipmentLookup = equipClient.CreateProxy ( );

			FMChannelFactory<ITransactionAliases> aliasesClient = new FMChannelFactory<ITransactionAliases> ( );
			ITransactionAliases aliasesLookup = aliasesClient.CreateProxy ( );

			// prepare reverse lookup
			ADOFMSReverseLookupSR sr = new ADOFMSReverseLookupSR ( );
			sr.Security = security;

			// reset transaction collection
			m_transactionCollection = new TransactionDOCollection ( );

			for (int i = 0; i < issues.Count; ++i)
			{
				try
				{
					TransactionDO trans = new TransactionDO ( );
					LineItemDO lineItem = new LineItemDO ( );

					trans.TransID = Guid.NewGuid ( ).ToString ( );

					// values which require lookups
					string receiver;
					string billto;
					string fuelType;
					string assetID;
					string site;

					// perform reverse lookups for name values
					StringDO match = null;

					FMChannelFactory<IADOFMSReverseLookupProcessor> adofmsClient = new FMChannelFactory<IADOFMSReverseLookupProcessor> ( );
					IADOFMSReverseLookupProcessor adofmsProcessor = adofmsClient.CreateProxy ( );

					// site lookup
					sr.EntityIdentifier = ADOFMSReverseLookupSR.EntityID.SITE;
					sr.EntityValue = issues.GetRowValue ( IssuesObject.COLUMN_SITE, i ).ToString ( );
					match = ( adofmsProcessor.Process ( sr ) as StringDO );
					if (match == null)
					{
						LoggerManager.LogError ( m_defaults.LoggerKey, "Lookup failed for site of " + issues.GetRowValue ( "ID", i ).ToString ( ) + ", skipping" );
						continue;
					}
					site = match.Value;

					// company lookup
					sr.EntityIdentifier = ADOFMSReverseLookupSR.EntityID.COMPANY;
					sr.EntityValue = issues.GetRowValue ( (int) IssuesObject.Columns.BILLTO, i ).ToString ( );
					match = ( adofmsProcessor.Process ( sr ) as StringDO );
					if (match == null)
					{
						LoggerManager.LogError ( m_defaults.LoggerKey, "Lookup failed for bill-to of " + issues.GetRowValue ( "ID", i ).ToString ( ) + ", skipping" );
						continue;
					}
					billto = match.Value;
					sr.EntityValue = issues.GetRowValue ( (int) IssuesObject.Columns.CUSTOMER, i ).ToString ( );
					match = ( adofmsProcessor.Process ( sr ) as StringDO );
					if (match == null)
					{
						LoggerManager.LogError ( m_defaults.LoggerKey, "Lookup failed for customer of " + issues.GetRowValue ( "ID", i ).ToString ( ) + ", skipping" );
						continue;
					}
					receiver = match.Value;

					// product lookup
					sr.EntityIdentifier = ADOFMSReverseLookupSR.EntityID.PRODUCT;
					sr.EntityValue = issues.GetRowValue ( (int) IssuesObject.Columns.PRODUCT, i ).ToString ( );
					match = ( adofmsProcessor.Process ( sr ) as StringDO );
					if (match == null)
					{
						LoggerManager.LogError ( m_defaults.LoggerKey, "Lookup failed for product of " + issues.GetRowValue ( "ID", i ).ToString ( ) + ", skipping" );
						continue;
					}
					fuelType = match.Value;

					// asset lookup
					sr.EntityIdentifier = ADOFMSReverseLookupSR.EntityID.EQUIPMENT;
					sr.EntityValue = issues.GetRowValue ( (int) IssuesObject.Columns.DEFENCE_ASSET_ID, i ).ToString ( );
					match = ( adofmsProcessor.Process ( sr ) as StringDO );
					if (match == null)
					{
						LoggerManager.LogError ( m_defaults.LoggerKey, "Lookup failed for asset ID of " + issues.GetRowValue ( "ID", i ).ToString ( ) + ", skipping" );
						continue;
					}
					assetID = match.Value;

					// other required fields
					double quantity = double.Parse ( issues.GetRowValue ( (int) IssuesObject.Columns.QUANTITY, i ).ToString ( ) );
					DateTime actualInventoryDate = DateTime.Parse ( issues.GetRowValue ( (int) IssuesObject.Columns.ACTUAL_INVENTORY_DATE, i ).ToString ( ) );
					string aliasName = issues.GetRowValue ( IssuesObject.COLUMN_FMTRANSNAME, i ).ToString ( );
					TransactionTypes transTypeID = (TransactionTypes) ( int.Parse ( issues.GetRowValue ( IssuesObject.COLUMN_FMTRANSTYPE, i ).ToString ( ) ) );

					// verify lookups
					if (actualInventoryDate == null || string.IsNullOrEmpty ( aliasName ))
					{
						LoggerManager.LogError ( m_defaults.LoggerKey, "Failed to parse Alias and/or Actual Inventory Date of " + issues.GetRowValue ( "ID", i ).ToString ( ) + ", skipping" );
						continue;
					}

					// fill in transaction header
					trans.ManagerID = Defaults.MANAGER;
					trans.ManagerCompanyGuid = BaseDataObject.DUMMY_GUID;	// 31;
					trans.OwnerID = Defaults.OWNER;
					trans.OwnerCompanyGuid = BaseDataObject.DUMMY_GUID;	// 31;

					// site
					trans.Site = site;
					trans.SiteGuid = siteLookup.GetIdentityGuid(security, site);

					// companies
					trans.ShipToID = receiver;
					trans.ShipToCompanyGuid = companyLookup.GetIdentityGuid ( security, receiver );
					trans.BillToID = billto;
					trans.BillToCompanyGuid = companyLookup.GetIdentityGuid ( security, billto );

					// product
					lineItem.Product = fuelType;
					lineItem.ProductGuid = productLookup.GetIdentityGuid ( security, fuelType );

					// asset
					trans.DestinationEQ1.RegistrationID = assetID;
					trans.DestinationEQ1.EquipmentGuid = equipmentLookup.GetIdentityGuid(security, assetID);

					// alias
					trans.Alias = aliasName;
					trans.TransactionAliasGuid = aliasesLookup.GetMasterRecordGuid(security, aliasName );

					// misc
					trans.Date03 = actualInventoryDate;
					trans.TransTypeID = transTypeID;
					lineItem.Quantity.Gross = lineItem.Quantity.Net = lineItem.Quantity.GrossInventoryChange = lineItem.Quantity.NetInventoryChange = quantity;
					trans.Notes = "From ADOFMS Import, ID " + issues.GetRowValue ( "ID", i ).ToString ( );
					trans.FuelCardID = issues.GetRowValue ( (int) IssuesObject.Columns.FUEL_CARD_ID, i ).ToString ( );

					// get units from accounting site
					if (!acctSiteTable.Contains ( site ))
					{
						FMChannelFactory<IAccountingSites> accountingSitesClient = new FMChannelFactory<IAccountingSites> ( );
						IAccountingSites accountingSites = accountingSitesClient.CreateProxy ( );

						AccountingSite acctSite = new AccountingSite ( );
						acctSite = accountingSites.LoadSiteInfo(security, trans.SiteGuid);

						acctSiteTable[trans.SiteGuid] = acctSite;
					}
					AccountingSite accountingSite = acctSiteTable[trans.SiteGuid] as AccountingSite;
					if (accountingSite != null)
					{
						trans.VolumeUnits = accountingSite.VolumeUnits;
						lineItem.VolumeUnits = accountingSite.VolumeUnits;
					}

					trans.LineItems.Add ( lineItem );

					// add it to the collection
					m_transactionCollection.Add ( trans );
				}
				catch (Exception e)
				{
					LoggerManager.LogError ( m_defaults.LoggerKey, "Failure transforming issue " + issues.GetRowValue ( "ID", i ).ToString ( ) + ", " + e.Message );
				}
			}

			return result;
		}

		public override Type GetTransformingType ( )
		{
			return typeof ( IssuesObject );
		}
		#endregion // ITransformer members
	}
}
