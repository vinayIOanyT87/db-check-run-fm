// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FuelCards.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the FuelCardsDataDictionaryClass type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FMBusinessServices.ServiceClasses
{
	using System;
	using System.Data;
	using System.Data.SqlClient;
	using System.Security;
	using System.ServiceModel;
	using System.Text.RegularExpressions;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.Exceptions;

	using FMBusinessServices.DataAccessLayer;
	using FMBusinessServices.InternalClasses;


	/// <summary>
	/// Summary description for FuelCards.
	/// </summary>
	[SecuritySafeCriticalAttribute]
	[ServiceBehavior(TransactionIsolationLevel = System.Transactions.IsolationLevel.ReadCommitted)]
	public class FuelCardsClass : FMServiceBase, IDependency, IFuelCards
	{
		#region Protected data members
		/// <summary>
		/// The consolidated data access.
		/// </summary>
		internal ConsolidatedDAClass ConsolidatedDa = new ConsolidatedDAClass ( );
		#endregion

		#region Private data members
		public const string MSG001 = "Error newing ConsolidatedDAClass";
		public const string MSG002 = "Security is null";
		public const string MSG003 = "Fuel Card is null";
		public const string MSG005 = "Fuel Card ID is a required field";
		public const string MSG006 = "Fuel Card Exists";
		public const string MSG007 = "Fuel Card Not Found";
		public const string MSG008 = "IDependency Object is null";
		public const string MSG009 = "Fuel Card record exist, cannot delete";
		public const string MSG010 = "Could not update associated transactions";
		public const string MSG011 = "Fuel Card ID must be an alphanumeric value which does not have any spaces.";
		#endregion

		#region Constructors
		/// <summary>
		/// Initializes a new instance of the <see cref="FuelCardsClass"/> class. 
		/// This is the default constructor for the Fuel Card Class.
		/// </summary>
		public FuelCardsClass ( )
		{
		}
		#endregion

		#region Public methods
		/// <summary>
		/// This method will add a new fuel card record to the database. It will
		/// throw an exception on any error.
		/// </summary>
		/// <param name="security"></param>
		/// <param name="FuelCard"></param>
		/// <returns></returns>
		[OperationBehavior( TransactionScopeRequired = true, TransactionAutoComplete = true )]
		public Guid Add( SecurityClass security, FuelCardClass fuelCard )
		{
			var entityToSiteMaps = new EntityToSiteMaps ( );
			var equipments = new EquipmentsClass ( );

			// Validate the security and Fuel Card objects
			if (security == null)
			{
				throw new ArgumentNullException ( MSG002 );
			}

			if (fuelCard == null)
			{
				throw new ArgumentNullException ( MSG003 );
			}

			// Ensure that the user has the modify rights.
			if (security.HasRight ( RIGHT.MODIFY_FUEL_CARD_DATA ) == false
					&& security.HasRight ( RIGHT.IMPORT_ENTERPRISE_DATA ) == false)
			{
				throw new FMInsufficientRightsException();
			}

			fuelCard.ID = string.IsNullOrEmpty(fuelCard.ID) ? string.Empty : fuelCard.ID.Trim();
			this.Validate(security, fuelCard);

			// Throw an exception if a record with the same ID exists.
			if (this.GetIdentityGuid(security, fuelCard.ID) != Guid.Empty)
			{
				throw new Exception ( MSG006 );
			}

			// Set UserData(list type) to defaults if they are blanks
			UserDataFieldsClass.SetDefaults(security, fuelCard.UserData, ENTITY_TYPE.FUEL_CARD);

			fuelCard.SiteGuid			= security.SiteGuid;
			fuelCard.CreatedBy			= security.UserID;
			fuelCard.UpdatedBy			= security.UserID;
			fuelCard.StatusModifiedBy	= security.UserID;
			fuelCard.IdentityGuid		= Guid.NewGuid();

			using (var cmd = new SqlCommand())
			{
				fuelCard.InsertSQL(cmd);
				this.ConsolidatedDa.ExecuteQuery(security, cmd);
			}

            var flc = new FieldLevelConfigMapsClass();

			foreach (EquipmentClass equipment in fuelCard.EquipmentCollection)
			{
				EquipmentClass detailedEquipment = equipments.Get ( security, equipment.IdentityGuid );
                bool isFuelCardEditable = flc.IsFieldRecordVersionSpecific(security, ENTITY_TYPE.EQUIPMENT.ToString(), equipment.IdentityGuid, equipment.MasterRecordGuid, equipment.SiteGuid, "Fuel Card");

				if (isFuelCardEditable && (detailedEquipment.IdentityGuid != Guid.Empty))
				{
					detailedEquipment.FuelCardGuid	= fuelCard.IdentityGuid;
					detailedEquipment.FuelCardID	= fuelCard.ID;
					equipments.Modify ( security, detailedEquipment );
				}
			}

			// Create Entity to Site Map
			var entityToSiteMap = new EntityToSiteMapClass(fuelCard);
			entityToSiteMaps.Add ( security, entityToSiteMap, GetType().GUID );

			// Add the fuel card to fuel card limit mapping.
			this.AddFuelCardToFuelCardLimitMapping(security, fuelCard);

			return fuelCard.IdentityGuid;
		}

		/// <summary>
		/// This method updates the fuel card information in the database.
		/// </summary>
		/// <param name="security">Security object.</param>
		/// <param name="fuelCard">Fuel Card object.</param>
		[OperationBehavior( TransactionScopeRequired = true, TransactionAutoComplete = true )]
		public void Modify( SecurityClass security, FuelCardClass fuelCard )
		{
			var entityToSiteMaps = new EntityToSiteMaps ( );
			var equipments = new EquipmentsClass ( );

			// Ensure the security and Fuel Card objects are valid.
			if (security == null)
			{
				throw new ArgumentNullException ( MSG002 );
			}

			if (fuelCard == null)
			{
				throw new ArgumentNullException ( MSG003 );
			}

			// Ensure user had modify rights.
			if (!security.HasRight ( RIGHT.MODIFY_FUEL_CARD_DATA ) &&
					!security.HasRight ( RIGHT.EXECUTE_IMPORT_EXPORT ) &&
					!security.HasRight ( RIGHT.IMPORT_ENTERPRISE_DATA ))
			{
				throw new FMInsufficientRightsException();
			}

			fuelCard.ID = string.IsNullOrEmpty( fuelCard.ID ) ? string.Empty : fuelCard.ID.Trim();
			this.Validate ( security, fuelCard );

			// Ensure that the Fuel Card object does not update
			// another Fuel Card record.
			Guid identityGuid = this.GetIdentityGuid(security, fuelCard.ID);

			if (identityGuid != Guid.Empty && identityGuid != fuelCard.IdentityGuid)
			{
				throw new Exception ( MSG006 );
			}

			// Ensure that the existing Fuel Card exists.
			FuelCardClass oldClass = this.Get(security, fuelCard.IdentityGuid, true);

			if (oldClass.IdentityGuid == Guid.Empty)
			{
				throw new Exception ( MSG007 );
			}


          // Set UserData(list type) to defaults if they are blanks
          UserDataFieldsClass.SetDefaults(security, fuelCard.UserData, ENTITY_TYPE.FUEL_CARD);
			
			fuelCard.UpdatedBy = security.UserID;

			if (fuelCard.Status != oldClass.Status)
			{
				fuelCard.StatusModifiedBy = security.UserID;
			}
			else if (string.IsNullOrEmpty(fuelCard.StatusModifiedBy))
			{
				if (string.IsNullOrEmpty(oldClass.StatusModifiedBy))
				{
					fuelCard.StatusModifiedBy = security.UserID;
				}
				else
				{
					fuelCard.StatusModifiedBy = oldClass.StatusModifiedBy;
				}
			}

			using (var cmd = new SqlCommand())
			{
				fuelCard.UpdateSQL(cmd);
				this.ConsolidatedDa.ExecuteQuery(security, cmd);
			}

			foreach (EquipmentClass equipment in oldClass.EquipmentCollection)
			{
				if (fuelCard.EquipmentCollection.Find(x => x.IdentityGuid == equipment.IdentityGuid) != null)
				{
					continue;
				}

				EquipmentClass equipmentDetail = equipments.Get(security, equipment.IdentityGuid);
				equipmentDetail.FuelCardGuid = Guid.Empty;
				equipmentDetail.FuelCardID = string.Empty;
				equipments.Modify ( security, equipmentDetail );
			}

			foreach (EquipmentClass equipment in fuelCard.EquipmentCollection)
			{
				if (oldClass.EquipmentCollection.Find(x => x.IdentityGuid == equipment.IdentityGuid) != null)
				{
					continue;
				}

				EquipmentClass equipmentDetail = equipments.Get(security, equipment.IdentityGuid);
				equipmentDetail.FuelCardGuid = fuelCard.IdentityGuid;
				equipmentDetail.FuelCardID = fuelCard.ID;
				equipments.Modify ( security, equipmentDetail );
			}

			// Get the entity to site map collection.
			EntityToSiteMapCollectionClass entityToSiteMapCollection = entityToSiteMaps.EnumerateByTypeIDAndGuid(
																						security, fuelCard.EntityType, fuelCard.IdentityGuid );

			// If the new updated Fuel Card does not match the previous one, then purge
			// from the EntityToSiteMap.
			if (fuelCard.SiteGuid != oldClass.SiteGuid)
			{
				// Purge from EntityToSiteMap
				foreach (EntityToSiteMapClass entityToSiteMap in entityToSiteMapCollection)
				{
					entityToSiteMap.ID = fuelCard.ID;
					entityToSiteMaps.Purge ( security, entityToSiteMap );
				}

				// Create Entity to Site Map
				var newEntityToSiteMap = new EntityToSiteMapClass ( fuelCard );
				entityToSiteMaps.Add ( security, newEntityToSiteMap, GetType().GUID );
			}

			// Update the existing fuel card to fuel card limit mapping.
			this.ModifyFuelCardToFuelCardLimitMapping(security, oldClass, fuelCard);

			// TODO: Temporary commented out so that QA does not test change queue features.
			// ChangeQueueRecordsClass.ProcessChangeQueueRecords ( security, ChangeQueueEventType.Modify, FuelCard );
		}

		/// <summary>
		/// This method will return the Fuel Card class given the security object
		/// and identity GUID.
		/// </summary>
		/// <param name="security">Security object</param>
		/// <param name="identityGuid">Identity GUID</param>
		/// <param name="getExtendedInfo">When true, populates the equipment collection</param>
		/// <returns>Fuel Card object</returns>
		public FuelCardClass Get ( SecurityClass security, Guid identityGuid, bool getExtendedInfo )
		{
			if (security == null)
			{
				throw new ArgumentNullException ( MSG002 );
			}

			if (!security.HasRight ( RIGHT.VIEW_FUEL_CARD_DATA ) &&
				!security.HasRight ( RIGHT.VIEW_TRANSACTION_DATA ) &&
				!security.HasRight ( RIGHT.MODIFY_TRANSACTION_DATA ) &&
				!security.HasRight ( RIGHT.EXECUTE_IMPORT_EXPORT ) &&
				!security.HasRight ( RIGHT.MODIFY_FUEL_CARD_DATA ) &&
				!security.HasRight ( RIGHT.MODIFY_DISPATCH ) &&
				!security.HasRight ( RIGHT.VIEW_DISPATCH ) &&
				!security.HasRight(RIGHT.MODIFY_EQUIPMENT_DATA) &&
				!security.HasRight(RIGHT.IMPORT_ENTERPRISE_DATA))
			{
				throw new FMInsufficientRightsException();
			}

		    var sites = new SitesClass();
		    var site = sites.Get(
		        security,
		        security.SiteGuid,
		        bGetMemberSites: false,
		        getSchedulesAndProcessVariables: false,
		        bGetAssociatedAliases: false);

			var fuelCard = new FuelCardClass(site) { IdentityGuid = identityGuid };

			using (var cmd = new SqlCommand())
			{
				fuelCard.SelectSQL(cmd, ContextUtil.IsInTransaction);
				fuelCard.Load(this.ConsolidatedDa.GetDataSet(cmd, security));
			}

			if (getExtendedInfo)
			{
				var equipments = new EquipmentsClass();
				var fuelCardLimits = new FuelCardLimits();

				fuelCard.EquipmentCollection = equipments.EnumerateByFuelCard(security, identityGuid);
				fuelCard.FuelCardLimit = fuelCardLimits.EnumerateFuelCardLimitMappingsByFuelCardGuid(security, identityGuid);
			}

			return fuelCard;
		}

        /// <summary>
		/// This method will return the Fuel Card Identity GUID given the security object and
		/// the Fuel Card ID.
		/// </summary>
		/// <param name="security">Security object</param>
		/// <param name="fuelCardId">Fuel Card ID</param>
		/// <returns>A Fuel Card GUID.</returns>
		public Guid GetIdentityGuid ( SecurityClass security, string fuelCardId )
		{
			if (security == null)
			{
				throw new ArgumentNullException ( MSG002 );
			}

			if (!security.HasRight ( RIGHT.VIEW_FUEL_CARD_DATA ) 
				&& !security.HasRight ( RIGHT.MODIFY_FUEL_CARD_DATA ) 
				&& !security.HasRight ( RIGHT.EXECUTE_IMPORT_EXPORT )
				&& !security.HasRight ( RIGHT.MODIFY_DISPATCH )
				&& !security.HasRight ( RIGHT.VIEW_DISPATCH )
				&& !security.HasRight ( RIGHT.IMPORT_ENTERPRISE_DATA )
				&& !security.HasRight(RIGHT.MODIFY_EQUIPMENT_DATA))
			{
				throw new FMInsufficientRightsException();
			}

			if (fuelCardId != null && (fuelCardId == "{Unassigned}" || fuelCardId == "{None}" || fuelCardId == "{All}"))
			{
				return Guid.Empty;
			}

            SitesClass sites = new SitesClass();
            var site = sites.Get(
                security,
                security.SiteGuid,
                bGetMemberSites: false,
                getSchedulesAndProcessVariables: false,
                bGetAssociatedAliases: false);

            var fuelCard = new FuelCardClass(site) { ID = fuelCardId };
			DataSet dataSet;

			using (var cmd = new SqlCommand())
			{
				fuelCard.SelectIdentityGuidSQL(cmd, security, ContextUtil.IsInTransaction);
				dataSet = this.ConsolidatedDa.GetDataSet(cmd, security);
			}

			if (dataSet.Tables.Count == 1 
				&& dataSet.Tables[0].Columns.Contains("FuelCardGuid")
				&& dataSet.Tables[0].Rows.Count == 1 
				&& !dataSet.Tables[0].Rows[0].IsNull("FuelCardGuid"))
			{
				return (Guid)dataSet.Tables[0].Rows[0]["FuelCardGuid"];
			}

			return Guid.Empty;
		}

		/// <summary>
		/// This method will purge a Fuel Card record from the database.
		/// </summary>
		/// <param name="security">Security object.</param>
		/// <param name="fuelCardGuid">Fuel Card object.</param>
		[OperationBehavior( TransactionScopeRequired = true, TransactionAutoComplete = true )]
		public void Purge(SecurityClass security, Guid fuelCardGuid)
		{
			var equipments = new EquipmentsClass ( );
			var entityToSiteMaps = new EntityToSiteMaps ( );

			if (security == null)
			{
				throw new ArgumentNullException ( FuelCardsClass.MSG002 );
			}

			if (security.HasRight ( RIGHT.MODIFY_FUEL_CARD_DATA ) == false
				&& security.HasRight ( RIGHT.IMPORT_ENTERPRISE_DATA ) == false
				&& !security.HasRight(RIGHT.MODIFY_SITES_AND_SITE_GROUPS))
			{
				throw new FMInsufficientRightsException();
			}

			FuelCardClass fuelCard = Get(security, fuelCardGuid, true);

			if (fuelCard.IdentityGuid == Guid.Empty)
			{
				throw new Exception ( MSG007 );
			}

			// Purge from EntityToSiteMap
			EntityToSiteMapCollectionClass entityToSiteMapCollection =
				entityToSiteMaps.EnumerateByTypeIDAndGuid(security, fuelCard.EntityType, fuelCardGuid);

			foreach (EntityToSiteMapClass entityToSiteMap in entityToSiteMapCollection)
			{
				entityToSiteMap.ID = fuelCard.ID;
				entityToSiteMaps.Purge ( security, entityToSiteMap );
			}

			foreach (EquipmentClass equipment in fuelCard.EquipmentCollection)
			{
				EquipmentClass equipmentDetail = equipments.Get(security, equipment.IdentityGuid);
				equipmentDetail.FuelCardGuid = Guid.Empty;
				equipmentDetail.FuelCardID = string.Empty;
				equipments.Modify ( security, equipmentDetail );
			}

			using (var cmd = new SqlCommand())
			{
				fuelCard.PurgeSQL(cmd);
				this.ConsolidatedDa.ExecuteQuery(security, cmd);
			}
		}

	    /// <summary>
	    /// This method will return a list of Fuel Card objects.
	    /// </summary>
	    /// <param name="security">Security object.</param>
	    /// <param name="hideHiddenFuelCards">If true, only fuel cards not marked as hidden will be returned</param>
	    /// <returns>A collection of Fuel Card objects.</returns>
	    public FuelCardCollectionClass EnumerateFuelCards ( SecurityClass security, bool hideHiddenFuelCards = false )
		{
			return this.EnumerateFuelCardsByCompanyAndFilter(security, Guid.Empty, Guid.Empty, Guid.Empty, Guid.Empty, Guid.Empty, null, hideHiddenFuelCards: hideHiddenFuelCards);
		}

	    /// <summary>
	    /// This method returns a dataset of fuel card records.
	    /// </summary>
	    /// <param name="security">Security object.</param>
	    /// <param name="hideHiddenFuelCards">If true, only fuel cards not marked as hidden will be returned</param>
	    /// <returns>Dataset of fuel card records.</returns>
	    public DataSet EnumerateFuelCardsForAutoComplete(SecurityClass security, bool hideHiddenFuelCards = false)
		{
			if (security == null)
			{
				throw new ArgumentNullException(MSG002);
			}

			if (!security.HasRight(RIGHT.VIEW_FUEL_CARD_DATA) &&
				!security.HasRight(RIGHT.MODIFY_FUEL_CARD_DATA) &&
				!security.HasRight(RIGHT.MODIFY_TRANSACTION_DATA) &&
				!security.HasRight(RIGHT.VIEW_TRANSACTION_DATA) &&
				!security.HasRight(RIGHT.EXECUTE_IMPORT_EXPORT) &&
				!security.HasRight(RIGHT.MODIFY_DISPATCH) &&
				!security.HasRight(RIGHT.VIEW_DISPATCH) &&
				!security.HasRight(RIGHT.MODIFY_EQUIPMENT_DATA) &&
				!security.HasRight(RIGHT.MODIFY_SITES_AND_SITE_GROUPS) &&
				!security.HasRight(RIGHT.EXPORT_ENTERPRISE_DATA))
			{
				throw new FMInsufficientRightsException();
			}

			DataSet dataSet;

			using (var cmd = new SqlCommand())
			{
				FuelCardClass.EnumerateForAutoCompleteSQL(cmd, security, hideHiddenFuelCards: hideHiddenFuelCards);
				dataSet = this.ConsolidatedDa.GetDataSet(cmd, security);
			}

			return dataSet;
		}

	    /// <summary>
	    /// This method will return a list of Fuel Card objects that matches the filter and
	    /// site.
	    /// </summary>
	    /// <param name="security">
	    /// Security object.
	    /// </param>
	    /// <param name="managerGuid">
	    /// The manager GUID.
	    /// </param>
	    /// <param name="ownerGuid">
	    /// The owner GUID.
	    /// </param>
	    /// <param name="shipperGuid">
	    /// The shipper GUID.
	    /// </param>
	    /// <param name="billToGuid">
	    /// The bill To GUID.
	    /// </param>
	    /// <param name="shipToGuid">
	    /// The ship To GUID.
	    /// </param>
	    /// <param name="filterList">
	    /// The filter List.
	    /// </param>
	    /// <param name="hideHiddenFuelCards">If true, only fuel cards not marked as hidden will be returned</param>
	    /// <returns>
	    /// A collection of Fuel Card objects.
	    /// </returns>
	    public FuelCardCollectionClass EnumerateFuelCardsByCompanyAndFilter(SecurityClass security, Guid managerGuid, Guid ownerGuid, Guid shipperGuid, Guid billToGuid, Guid shipToGuid, string filterList, bool hideHiddenFuelCards = false)
		{
			if (security == null)
			{
				throw new ArgumentNullException ( MSG002 );
			}

			if (!security.HasRight ( RIGHT.VIEW_FUEL_CARD_DATA ) &&
				!security.HasRight ( RIGHT.MODIFY_FUEL_CARD_DATA ) &&
				!security.HasRight ( RIGHT.MODIFY_TRANSACTION_DATA ) &&
				!security.HasRight ( RIGHT.VIEW_TRANSACTION_DATA ) &&
				!security.HasRight ( RIGHT.EXECUTE_IMPORT_EXPORT ) &&
				!security.HasRight ( RIGHT.MODIFY_DISPATCH ) &&
				!security.HasRight ( RIGHT.VIEW_DISPATCH ) &&
				!security.HasRight(RIGHT.MODIFY_EQUIPMENT_DATA) && 
				!security.HasRight(RIGHT.MODIFY_SITES_AND_SITE_GROUPS) &&
				!security.HasRight(RIGHT.EXPORT_ENTERPRISE_DATA))
			{
				throw new FMInsufficientRightsException();
			}

			DataSet dataSet;

			using (var cmd = new SqlCommand())
			{
				FuelCardClass.EnumerateSQL(cmd, security, managerGuid, ownerGuid, shipperGuid, billToGuid, shipToGuid, Guid.Empty, filterList, hideHiddenFuelCards: hideHiddenFuelCards);
				dataSet = this.ConsolidatedDa.GetDataSet(cmd, security);
			}

			var fuelCardCollection = new FuelCardCollectionClass ( );
			DataTable table = dataSet.Tables[0];

			foreach (DataRow row in table.Rows)
			{
				var fuelCard = new FuelCardClass ( );
				fuelCard.Load ( row );
				fuelCardCollection.Add ( fuelCard );
			}

			return fuelCardCollection;
		}

		/// <summary>
		/// This method will return a list of Fuel Card objects along with the associated fuel card
		/// limit and equipment.  It is used for the entity export.
		/// </summary>
		/// <param name="security">The security object.</param>
		/// <returns>A collection of fuel cards.</returns>
		public FuelCardCollectionClass EnumerateFuelCardsForEntityExport(SecurityClass security)
		{
			if (security == null)
			{
				throw new ArgumentNullException(MSG002);
			}

			if (!security.HasRight(RIGHT.VIEW_FUEL_CARD_DATA) &&
				!security.HasRight(RIGHT.MODIFY_FUEL_CARD_DATA) &&
				!security.HasRight(RIGHT.MODIFY_TRANSACTION_DATA) &&
				!security.HasRight(RIGHT.VIEW_TRANSACTION_DATA) &&
				!security.HasRight(RIGHT.EXECUTE_IMPORT_EXPORT) &&
				!security.HasRight(RIGHT.MODIFY_DISPATCH) &&
				!security.HasRight(RIGHT.VIEW_DISPATCH) &&
				!security.HasRight(RIGHT.MODIFY_EQUIPMENT_DATA) &&
				!security.HasRight(RIGHT.MODIFY_SITES_AND_SITE_GROUPS) &&
				!security.HasRight(RIGHT.EXPORT_ENTERPRISE_DATA))
			{
				throw new FMInsufficientRightsException();
			}

			DataSet dataSet;

			using (var cmd = new SqlCommand())
			{
				FuelCardClass.EnumerateSQL(cmd, security, Guid.Empty, Guid.Empty, Guid.Empty, Guid.Empty, Guid.Empty, Guid.Empty, null);
				dataSet = this.ConsolidatedDa.GetDataSet(cmd, security);
			}

			var fuelCardCollection = new FuelCardCollectionClass();
			DataTable table = dataSet.Tables[0];

			var fuelCardLimits = new FuelCardLimits();

			foreach (DataRow row in table.Rows)
			{
				var fuelCard = new FuelCardClass();
				fuelCard.Load(row);
				fuelCardCollection.Add(fuelCard);

				fuelCard.EquipmentCollection = this.GetAssociatedEquipmentForFuelCardExport(security, fuelCard.IdentityGuid);
				fuelCard.FuelCardLimit = fuelCardLimits.EnumerateFuelCardLimitMappingsByFuelCardGuid(security, fuelCard.IdentityGuid);
			}

			return fuelCardCollection;
		}

	    /// <summary>
	    /// This method will return a list of Fuel Card objects that matches the filter and
	    /// site.
	    /// </summary>
	    /// <param name="security">
	    /// Security object.
	    /// </param>
	    /// <param name="managerGuid">
	    /// The manager GUID.
	    /// </param>
	    /// <param name="ownerGuid">
	    /// The owner GUID.
	    /// </param>
	    /// <param name="shipperGuid">
	    /// The shipper GUID.
	    /// </param>
	    /// <param name="billToGuid">
	    /// The bill To GUID.
	    /// </param>
	    /// <param name="shipToGuid">
	    /// The ship To GUID.
	    /// </param>
	    /// <param name="fuelCardTypeApplicationStringGuid">The fuel card type to search for, or guid.empty to show all types</param>
	    /// <param name="filterList">
	    /// The filter List.
	    /// </param>
	    /// <param name="transientFlag">
	    /// Transient flag to filter on.</param>
	    /// <param name="hideHiddenFuelCards">If true, only fuel cards not marked as hidden will be returned</param>
	    /// <returns>
	    /// A collection of Fuel Card objects.
	    /// </returns>
	    public DataSet EnumerateFuelCardsForSummary(
												SecurityClass security, 
												Guid managerGuid, 
												Guid ownerGuid, 
												Guid shipperGuid, 
												Guid billToGuid, 
												Guid shipToGuid,
                                                Guid fuelCardTypeApplicationStringGuid,
												string filterList,
												bool transientFlag, 
                                                bool hideHiddenFuelCards = false)
		{
			if (security == null)
			{
				throw new ArgumentNullException(MSG002);
			}

			if (!security.HasRight(RIGHT.VIEW_FUEL_CARD_DATA) &&
				!security.HasRight(RIGHT.MODIFY_FUEL_CARD_DATA) &&
				!security.HasRight(RIGHT.MODIFY_TRANSACTION_DATA) &&
				!security.HasRight(RIGHT.VIEW_TRANSACTION_DATA) &&
				!security.HasRight(RIGHT.EXECUTE_IMPORT_EXPORT) &&
				!security.HasRight(RIGHT.MODIFY_DISPATCH) &&
				!security.HasRight(RIGHT.VIEW_DISPATCH) &&
				!security.HasRight(RIGHT.MODIFY_EQUIPMENT_DATA) &&
				!security.HasRight(RIGHT.MODIFY_SITES_AND_SITE_GROUPS) &&
				!security.HasRight(RIGHT.EXPORT_ENTERPRISE_DATA))
			{
				throw new FMInsufficientRightsException();
			}

			DataSet dataSet;

			using (var cmd = new SqlCommand())
			{
				FuelCardClass.EnumerateForSummarySql(
													cmd, 
													security, 
													managerGuid, 
													ownerGuid, 
													shipperGuid, 
													billToGuid, 
													shipToGuid, 
                                                    fuelCardTypeApplicationStringGuid,
													filterList, 
													transientFlag,
                                                    hideHiddenFuelCards: hideHiddenFuelCards);

				dataSet = this.ConsolidatedDa.GetDataSet(cmd, security);
			}

			return dataSet;
		}

        /// <summary>
        /// Enumerate all fuel cards not assigned to a fuel card limit owned or assigned to the current site.
        /// Optionally limit the fuel cards returned to those with an ID containing the provided searchFilter
        /// </summary>
        /// <param name="security">Contains security information</param>
        /// <param name="fuelCardLimitGuid">Fuel cards assigned to this limit will be returned.</param>
        /// <param name="searchFilter">If provided, limits the fuel cards returned to those containing the value provided in the ID field</param>
        /// <returns>All fuel cards not assigned to a fuel card limit owned or assigned to the current site.</returns>
        public FuelCardCollectionClass EnumerateNotAssignedToFuelCardLimit(SecurityClass security, Guid fuelCardLimitGuid, string searchFilter)
        {
            if (security == null)
            {
                throw new ArgumentNullException(MSG002);
            }

            if (!security.HasRight(RIGHT.VIEW_FUEL_CARD_DATA) &&
                !security.HasRight(RIGHT.MODIFY_FUEL_CARD_DATA) &&
                !security.HasRight(RIGHT.MODIFY_TRANSACTION_DATA) &&
                !security.HasRight(RIGHT.VIEW_FUEL_CARD_LIMIT) && 
                !security.HasRight(RIGHT.MODIFY_FUEL_CARD_LIMIT))
            {
                throw new FMInsufficientRightsException();
            }

            DataSet dataSet;

            using (var cmd = new SqlCommand())
            {
                FuelCardClass.EnumerateNotAssignedToFuelCardLimitSQL(cmd, security, fuelCardLimitGuid, searchFilter);
                dataSet = this.ConsolidatedDa.GetDataSet(cmd, security);
            }

            var fuelCardCollection = new FuelCardCollectionClass();
            DataTable table = dataSet.Tables[0];

            foreach (DataRow row in table.Rows)
            {
                var fuelCard = new FuelCardClass
                                   {
                                       ID = DataObject.getValue(row["ID"], string.Empty),
                                       IdentityGuid = DataObject.getValue(row["FuelCardGuid"], Guid.Empty),
                                       SiteGuid = DataObject.getValue(row["SiteGuid"], Guid.Empty),
                                       ManagerID = DataObject.getValue(row["ManagerID"], string.Empty),
                                       BillToID = DataObject.getValue(row["BillToID"], string.Empty)
                                   };

                fuelCardCollection.Add(fuelCard);
            }

            return fuelCardCollection;
        }

		/// <summary>
		/// The enumerate fuel cards by company.
		/// </summary>
		/// <param name="security">
		/// The security.
		/// </param>
		/// <param name="managerGuid">
		/// The manager GUID.
		/// </param>
		/// <param name="ownerGuid">
		/// The owner GUID.
		/// </param>
		/// <param name="shipperGuid">
		/// The shipper GUID.
		/// </param>
		/// <param name="billToGuid">
		/// The bill to GUID.
		/// </param>
		/// <param name="shipToGuid">
		/// The ship to GUID.
		/// </param>
		/// <returns>
		/// The <see cref="FuelCardCollectionClass"/>.
		/// </returns>
		public FuelCardCollectionClass EnumerateFuelCardsByCompany(SecurityClass security, Guid managerGuid, Guid ownerGuid, Guid shipperGuid, Guid billToGuid, Guid shipToGuid)
		{
			return this.EnumerateFuelCardsByCompanyAndFilter(security, managerGuid, ownerGuid, shipperGuid, billToGuid, shipToGuid, null);
		}
		#endregion

		[OperationBehavior( TransactionScopeRequired = true, TransactionAutoComplete = true )]
		public void Import( SecurityClass inSecurity, FuelCardClass fuelCard )
		{
			if (inSecurity == null)
			{
				throw new ArgumentNullException("inSecurity");
			}

			if (fuelCard == null)
			{
				throw new ArgumentNullException("fuelCard");
			}

			SecurityClass security = inSecurity.Clone();

			var companies = new CompaniesClass ( );
			var equipments = new EquipmentsClass ( );
			var companyRoleMaps = new CompanyRoleMapsClass ( );		

			try
			{
				fuelCard.IdentityGuid = GetIdentityGuid(security, fuelCard.ID);

				// If the entity exists and is not owned by this site, do not update it.
				if (fuelCard.IdentityGuid != Guid.Empty 
					&& Get(security, fuelCard.IdentityGuid, false).SiteGuid != security.SiteGuid)
				{
					return;
				}

				if (fuelCard.ShipToID != "{Unassigned}" && fuelCard.ShipToID != string.Empty)
				{
					fuelCard.ShipToGuid = companies.GetIdentityGuid(security, fuelCard.ShipToID);

					if (fuelCard.ShipToGuid == Guid.Empty)
					{
						var company = new CompanyClass
						{
							ID = fuelCard.ShipToID,
							Code = fuelCard.ShipToCode
						};

						fuelCard.ShipToGuid = companies.Add(security, company);
					}

					var companyRoleMap = new CompanyRoleMapClass
					                     {
						                     Role = COMPANY_ROLE.CUSTOMER_SHIPTO,
											 CompanyGuid = fuelCard.ShipToGuid
					                     };
					companyRoleMaps.Add ( security, companyRoleMap );
				}

				if (fuelCard.BillToID != "{Unassigned}" && fuelCard.BillToID != string.Empty)
				{
					fuelCard.BillToGuid = companies.GetIdentityGuid(security, fuelCard.BillToID);

					if (fuelCard.BillToGuid == Guid.Empty)
					{
						var company = new CompanyClass
						{
							ID = fuelCard.BillToID,
							Code = fuelCard.BillToCode
						};

						fuelCard.BillToGuid = companies.Add(security, company);
					}

					var companyRoleMap = new CompanyRoleMapClass
					                     {
						                     Role = COMPANY_ROLE.CUSTOMER_BILLTO,
											 CompanyGuid = fuelCard.BillToGuid
					                     };

					companyRoleMaps.Add ( security, companyRoleMap );
				}

				if (fuelCard.ShipperID != "{Unassigned}" && fuelCard.ShipperID != string.Empty)
				{
					fuelCard.ShipperGuid = companies.GetIdentityGuid(security, fuelCard.ShipperID);

					if (fuelCard.ShipperGuid == Guid.Empty)
					{
						var company = new CompanyClass
						{
							ID = fuelCard.ShipperID,
							Code = fuelCard.ShipperCode
						};

						fuelCard.ShipperGuid = companies.Add(security, company);
					}

					var companyRoleMap = new CompanyRoleMapClass
					{
						Role = COMPANY_ROLE.SHIPPER,
						CompanyGuid = fuelCard.ShipperGuid
					};

					companyRoleMaps.Add ( security, companyRoleMap );
				}

				if (fuelCard.OwnerID != "{Unassigned}" && fuelCard.OwnerID != string.Empty)
				{
					fuelCard.OwnerGuid = companies.GetIdentityGuid(security, fuelCard.OwnerID);

					if (fuelCard.OwnerGuid == Guid.Empty)
					{
						var company = new CompanyClass
						{
							ID = fuelCard.OwnerID,
							Code = fuelCard.OwnerCode
						};

						fuelCard.OwnerGuid = companies.Add(security, company);
					}

					var companyRoleMap = new CompanyRoleMapClass
					{
						Role = COMPANY_ROLE.OWNER,
						CompanyGuid = fuelCard.OwnerGuid
					};

					companyRoleMaps.Add ( security, companyRoleMap );
				}

				if (fuelCard.ManagerID != "{Unassigned}" && fuelCard.ManagerID != string.Empty)
				{
					fuelCard.ManagerGuid = companies.GetIdentityGuid(security, fuelCard.ManagerID);

					if (fuelCard.ManagerGuid == Guid.Empty)
					{
						var company = new CompanyClass
						{
							ID = fuelCard.ManagerID,
							Code = fuelCard.ManagerCode
						};

						fuelCard.ManagerGuid = companies.Add(security, company);
					}

					var companyRoleMap = new CompanyRoleMapClass
					{
						Role = COMPANY_ROLE.MANAGER,
						CompanyGuid = fuelCard.ManagerGuid
					};

					companyRoleMaps.Add ( security, companyRoleMap );
				}

				foreach (EquipmentClass equipment in fuelCard.EquipmentCollection)
				{
					equipment.IdentityGuid = equipments.GetIdentityGuid(security, equipment.ID);

					if (equipment.IdentityGuid == Guid.Empty)
					{
						equipment.IdentityGuid = equipments.Add(security, equipment);
					}
				}

				if (fuelCard.IdentityGuid == Guid.Empty)
				{
					// The add handles the fuel card limit assignment.
					this.Add(security, fuelCard);
				}
				else
				{
					// The modify handles the fuel card limit assignment.
					this.Modify(security, fuelCard);
				}
			}
			catch (Exception ex)
			{
				throw new Exception("[Fuel Card Import Error ID] : " + fuelCard.ID + ", " + ex.Message);
			}
		}

		/// <summary>
		/// This method will add the fuel card to fuel card limit mapping.
		/// </summary>
		/// <param name="security">The security object.</param>
		/// <param name="fuelCard">The fuel card object which contains the fuel card limit.</param>
		private void AddFuelCardToFuelCardLimitMapping(SecurityClass security, FuelCardClass fuelCard)
		{
			if (fuelCard.FuelCardLimit != null && fuelCard.FuelCardLimit.IdentityGuid != Guid.Empty)
			{
				var fuelCardLimits = new FuelCardLimits();

				// Insert new fuel card to fuel card limit mapping.
				fuelCardLimits.AddFuelCardToFuelCardLimitMapping(security, fuelCard);
			}
		}

		/// <summary>
		/// This method will update an existing fuel card to fuel card limit mapping.
		/// </summary>
		/// <param name="security">The security object.</param>
		/// <param name="oldFuelCard">The original fuel card object.</param>
		/// <param name="fuelCard">The fuel card object which contains the fuel card limit.</param>
		private void ModifyFuelCardToFuelCardLimitMapping(SecurityClass security, FuelCardClass oldFuelCard, FuelCardClass fuelCard)
		{
			if (oldFuelCard.FuelCardLimit == null && fuelCard.FuelCardLimit == null)
			{
				return;
			}

			if (oldFuelCard.FuelCardLimit == null && fuelCard.FuelCardLimit != null)
			{
				this.AddFuelCardToFuelCardLimitMapping(security, fuelCard);
				return;
			}

			if (oldFuelCard.FuelCardLimit != null && fuelCard.FuelCardLimit == null)
			{
				var fuelCardLimits = new FuelCardLimits();
				fuelCardLimits.ModifyFuelCardToFuelCardLimitMapping(security, oldFuelCard.FuelCardLimit.IdentityGuid, fuelCard);
				return;
			}

			if (oldFuelCard.FuelCardLimit != null && fuelCard.FuelCardLimit != null)
			{
				if (oldFuelCard.FuelCardLimit.ID == fuelCard.FuelCardLimit.ID)
				{
					return;
				}

				var fuelCardLimits = new FuelCardLimits();
				fuelCardLimits.ModifyFuelCardToFuelCardLimitMapping(security, oldFuelCard.FuelCardLimit.IdentityGuid, fuelCard);
			}
		}

		/// <summary>
		/// The validate.
		/// </summary>
		/// <param name="security">Security object.</param>
		/// <param name="fuelCard">
		/// The fuel card.
		/// </param>
		/// <exception cref="Exception">
		/// Fuel Card invalid exception.
		/// </exception>
		private void Validate ( SecurityClass security, FuelCardClass fuelCard )
		{
			var objAlphaNumeric = new Regex ( "[^a-zA-Z0-9]" );

			// Throw an exception if there is no ID.
			if (string.IsNullOrEmpty(fuelCard.ID) || ( fuelCard.ID.Length <= 0 ))
			{
				throw new Exception ( MSG005 );
			}

			if (objAlphaNumeric.IsMatch ( fuelCard.ID ))
			{
				throw new Exception ( MSG011 );
			}

			this.ValidateUserData(security, fuelCard);
		}

		/// <summary>
		/// This method gets the associated equipment for the entity export of a 
		/// fuel card only.
		/// </summary>
		/// <param name="security">The security object.</param>
		/// <param name="fuelCardGuid">The fuel card GUID</param>
		/// <returns>Equipment Collections that only has IDs.</returns>
		private EquipmentCollectionClass GetAssociatedEquipmentForFuelCardExport(SecurityClass security, Guid fuelCardGuid)
		{
			const string Select = "SELECT e.ID, e.SiteGuid ";
			const string From	= "FROM tblEquipment e ";
			const string Where	= "WHERE e.FuelCardGuid = @FuelCardGuid ";

			var equipmentCollection = new EquipmentCollectionClass();

			using (var sqlCommand = new SqlCommand())
			{
				sqlCommand.CommandText = Select + From + Where;

				var parm = new SqlParameter("@FuelCardGuid", SqlDbType.UniqueIdentifier)
				{
					Value = fuelCardGuid
				};
				sqlCommand.Parameters.Add(parm);

				DataSet dataSet = this.ConsolidatedDa.GetDataSet(sqlCommand, security);

				if (dataSet != null && dataSet.Tables.Count > 0 && dataSet.Tables[0].Rows.Count > 0)
				{
					DataTable table = dataSet.Tables[0];

					foreach (DataRow row in table.Rows)
					{
						string id = row.IsNull("ID") ? string.Empty : (string) row["ID"];
						Guid siteGuid = row.IsNull("SiteGuid") ? Guid.Empty : (Guid)row["SiteGuid"];

						if (string.IsNullOrEmpty(id) == false && siteGuid != Guid.Empty)
						{
							var equipment = new EquipmentClass
							{
								ID = id,
								SiteGuid = siteGuid
							};
							equipmentCollection.Add(equipment);
						}
					}
				}
			}

			return equipmentCollection;
		}

		#region Handle dependencies
		/// <summary>
		/// This method will insert a dependency for the Fuel Card object.
		/// </summary>
		/// <param name="security"></param>
		/// <param name="inObject"></param>
		void IDependency.Insert ( SecurityClass security, BaseDataObject inObject, bool preOperation )
		{
			if (security == null)
			{
				throw new ArgumentNullException ( FuelCardsClass.MSG002 );
			}

			if (inObject == null)
			{
				throw new ArgumentNullException ( FuelCardsClass.MSG008 );
			}
		}

		/// <summary>
		/// This method will update a dependency on for the fuel card object.
		/// </summary>
		/// <param name="security"></param>
		/// <param name="inObject"></param>
		void IDependency.Update ( SecurityClass security, BaseDataObject inObject )
		{
			if (security == null)
			{
				throw new ArgumentNullException ( FuelCardsClass.MSG002 );
			}

			if (inObject == null)
			{
				throw new ArgumentNullException ( FuelCardsClass.MSG008 );
			}

			var siteObject = inObject as SiteClass;

			if (siteObject != null)
			{
				SiteClass site = siteObject;
				FuelCardCollectionClass fuelCardCollection = this.EnumerateFuelCards ( security );
				var entityToSiteMaps = new EntityToSiteMaps ( );

				foreach (FuelCardClass fuelCard in fuelCardCollection)
				{
					if (site.SiteGuid == fuelCard.SiteGuid)
					{
						EntityToSiteMapCollectionClass entityToSiteMapCollection =
							entityToSiteMaps.EnumerateByTypeIDAndGuid(security,
							fuelCard.EntityType,
							fuelCard.IdentityGuid );

						foreach (EntityToSiteMapClass entityToSiteMap in entityToSiteMapCollection)
						{
							if (entityToSiteMap.SiteGuid != site.SiteGuid)
							{
								entityToSiteMaps.Purge ( security, entityToSiteMap );
							}
						}
					}
				}
			}
		}

		/// <summary>
		/// This method will purge a dependency on for the fuel card object.
		/// </summary>
		/// <param name="security"></param>
		/// <param name="inObject"></param>
		void IDependency.Purge ( SecurityClass security, BaseDataObject inObject )
		{
			if (security == null)
			{
				throw new ArgumentNullException ( FuelCardsClass.MSG002 );
			}

			if (inObject == null)
			{
				throw new ArgumentNullException ( FuelCardsClass.MSG008 );
			}


			// Throw an exception if a fuel card record has an identityGuid that matches the
			// the site being deleted.
			var siteObject = inObject as SiteClass;

			if (siteObject != null)
			{
				SiteClass site = siteObject;
				var entityToSiteMaps = new EntityToSiteMaps ( );
				FuelCardCollectionClass fuelCardCollection = this.EnumerateFuelCards ( security );

				foreach (FuelCardClass fuelCard in fuelCardCollection)
				{
					if (site.IdentityGuid == fuelCard.SiteGuid)
						Purge(security, fuelCard.IdentityGuid);
					else
					{
						var entityToSiteMap = new EntityToSiteMapClass
						                      {
							                      TypeID = fuelCard.EntityType,
							                      SiteGuid = site.SiteGuid,
							                      IdentityGuid = fuelCard.IdentityGuid
						                      };
						entityToSiteMaps.Purge ( security, entityToSiteMap );
					}
				}
				return;
			}

			var companyRoleMapObject = inObject as CompanyRoleMapClass;

			if (companyRoleMapObject != null)
			{
				CompanyRoleMapClass companyRoleMap = companyRoleMapObject;
				Guid siteGuid = security.SiteGuid;

				try
				{
					security.SiteGuid = companyRoleMap.SiteGuid;
					FuelCardCollectionClass fuelCardCollection;

					switch (companyRoleMap.Role)
					{
						case COMPANY_ROLE.MANAGER:
							fuelCardCollection = EnumerateFuelCardsByCompany(security, companyRoleMap.CompanyGuid, Guid.Empty, Guid.Empty, Guid.Empty, Guid.Empty);
							break;
						case COMPANY_ROLE.OWNER:
							fuelCardCollection = EnumerateFuelCardsByCompany(security, Guid.Empty, companyRoleMap.CompanyGuid, Guid.Empty, Guid.Empty, Guid.Empty);
							break;
						case COMPANY_ROLE.SHIPPER:
							fuelCardCollection = EnumerateFuelCardsByCompany(security, Guid.Empty, Guid.Empty, companyRoleMap.CompanyGuid, Guid.Empty, Guid.Empty);
							break;
						case COMPANY_ROLE.CUSTOMER_BILLTO:
							fuelCardCollection = EnumerateFuelCardsByCompany(security, Guid.Empty, Guid.Empty, Guid.Empty, companyRoleMap.CompanyGuid, Guid.Empty);
							break;
						case COMPANY_ROLE.CUSTOMER_SHIPTO:
							fuelCardCollection = EnumerateFuelCardsByCompany(security, Guid.Empty, Guid.Empty, Guid.Empty, Guid.Empty, companyRoleMap.CompanyGuid);
							break;
						default:
							return;
					}

					foreach (FuelCardClass fuelCard in fuelCardCollection)
					{
						switch (companyRoleMap.Role)
						{
							case COMPANY_ROLE.MANAGER:
								fuelCard.ManagerGuid = Guid.Empty;
								break;
							case COMPANY_ROLE.OWNER:
								fuelCard.OwnerGuid = Guid.Empty;
								break;
							case COMPANY_ROLE.SHIPPER:
								fuelCard.ShipperGuid = Guid.Empty;
								break;
							case COMPANY_ROLE.CUSTOMER_BILLTO:
								fuelCard.BillToGuid = Guid.Empty;
								break;
							case COMPANY_ROLE.CUSTOMER_SHIPTO:
								fuelCard.ShipToGuid = Guid.Empty;
								break;
							default:
								return;
						}

						fuelCard.UpdatedDate = DateTimeOffset.Now;
						fuelCard.UpdatedBy = security.UserID;

						using (var cmd = new SqlCommand())
						{
							fuelCard.UpdateSQL(cmd);
							this.ConsolidatedDa.ExecuteQuery(security, cmd);
						}
					}
				}
				finally
				{
					security.SiteGuid = siteGuid;
				}
			}
		}
		#endregion
	}
}
