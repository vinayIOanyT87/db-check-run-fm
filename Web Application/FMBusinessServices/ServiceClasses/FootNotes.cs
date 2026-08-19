// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FootNotes.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Summary description for FootNotes.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FMBusinessServices.ServiceClasses
{
	using System;
	using System.Data;
	using System.Data.SqlClient;
	using System.Security;
	using System.ServiceModel;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.Exceptions;

	using FMBusinessServices.DataAccessLayer;
	using FMBusinessServices.InternalClasses;

	/// <summary>
	/// Service class for FootNotes
	/// </summary>
	[SecuritySafeCritical]
	[ServiceBehavior(TransactionIsolationLevel = System.Transactions.IsolationLevel.ReadCommitted)]
	public class FootNotes : IDependency, IFootNotes
	{
		#region Constants and Fields

		/// <summary>
		/// Database access object
		/// </summary>
		private readonly ConsolidatedDAClass consolidatedDA = new ConsolidatedDAClass();

		#endregion

		#region Public Methods and Operators

		/// <summary>
		/// Adds the specified footnote.
		/// </summary>
		/// <param name="security">The security object.</param>
		/// <param name="footNote">The foot note to add.</param>
		/// <returns>The identity Guid of the newly added footnote.</returns>
		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public Guid Add(SecurityClass security, FootNoteClass footNote)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (footNote == null)
			{
				throw new ArgumentNullException("footNote");
			}

			if (!security.HasRight(RIGHT.MODIFY_PRODUCTS))
			{
				throw new FMInsufficientRightsException();
			}

			this.Validate(footNote);

			if (this.GetIdentityGuid(security, footNote.ID) != Guid.Empty)
			{
				throw new Exception("Footnote Exists");
			}

			footNote.SiteGuid = security.SiteGuid;
			footNote.CreatedDate = DateTimeOffset.Now;
			footNote.CreatedBy = security.UserID;
			footNote.UpdatedDate = footNote.CreatedDate;
			footNote.UpdatedBy = security.UserID;
			footNote.Deleted = false;
			footNote.IdentityGuid = Guid.NewGuid();

			using (var cmd = new SqlCommand())
			{
				footNote.InsertSQL(cmd);
				consolidatedDA.ExecuteQuery(security, cmd);
			}

			// Create Entity to Site Map
			var entityToSiteMaps = new EntityToSiteMaps();
			var entityToSiteMap = new EntityToSiteMapClass(footNote);
			entityToSiteMaps.Add(security, entityToSiteMap, this.GetType().GUID);

			var applicationStringMaps = new ApplicationStringMapsClass();
			applicationStringMaps.ModifyCollection(security, footNote.IdentityGuid, footNote.FootNoteShipToMapCollection, null);
			applicationStringMaps.ModifyCollection(security, footNote.IdentityGuid, footNote.FootNoteShipperMapCollection, null);
			applicationStringMaps.ModifyCollection(
				security, footNote.IdentityGuid, footNote.FootNoteShipToStateMapCollection, null);
			applicationStringMaps.ModifyCollection(security, footNote.IdentityGuid, footNote.FootNoteProductMapCollection, null);
            applicationStringMaps.ModifyCollection(security, footNote.IdentityGuid, footNote.FootNoteAdditiveProfileMapCollection, null);

			return footNote.IdentityGuid;
		}

		/// <summary>
		/// Enumerates footnotes.
		/// </summary>
		/// <param name="security">
		/// The security object.
		/// </param>
		/// <returns>
		/// A collection of enumerated footnotes.
		/// </returns>
		/// <exception cref="ArgumentNullException">
		/// Thrown if the security object is invalid.
		/// </exception>
		/// <exception cref="Exception">
		/// Access denied.
		/// </exception>
		public FootNoteCollectionClass Enumerate(SecurityClass security)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (!security.HasRight(RIGHT.VIEW_PRODUCTS) && !security.HasRight(RIGHT.MODIFY_PRODUCTS) && !security.HasRight(RIGHT.MODIFY_SITES_AND_SITE_GROUPS))
			{
				throw new FMInsufficientRightsException();
			}

			var footNote = new FootNoteClass();
			DataSet set;

			using (var cmd = new SqlCommand())
			{
				footNote.EnumerateSQL(cmd, security);
				set = this.consolidatedDA.GetDataSet(cmd, security);
			}

			var footNoteCollection = new FootNoteCollectionClass();

			var table = set.Tables[0];
			while (table.Rows.Count != 0)
			{
				footNote = new FootNoteClass();
				footNote.Load(set);
				footNoteCollection.Add(footNote);
				table.Rows.RemoveAt(0);
			}

			return footNoteCollection;
		}

		/// <summary>
		/// Gets the specified footnote.
		/// </summary>
		/// <param name="security">The security object.</param>
		/// <param name="identityGuid">The identity GUID of the footnote to get.</param>
		/// <returns>The requested footnote or null if not found.</returns>
		public FootNoteClass Get(SecurityClass security, Guid identityGuid)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (!security.HasRight(RIGHT.VIEW_PRODUCTS) && !security.HasRight(RIGHT.MODIFY_PRODUCTS))
			{
				throw new FMInsufficientRightsException();
			}

			var footNote = new FootNoteClass { IdentityGuid = identityGuid };

			using (var cmd = new SqlCommand())
			{
				footNote.SelectSQL(cmd, ContextUtil.IsInTransaction);
				footNote.Load(this.consolidatedDA.GetDataSet(cmd, security));
			}

			var applicationStringMaps = new ApplicationStringMapsClass();
			footNote.FootNoteShipToMapCollection = applicationStringMaps.EnumerateByApplicationStringGuidAndType(
				security, footNote.IdentityGuid, STRING_MAP_TYPE.FOOT_NOTE_SHIPTO);
			footNote.FootNoteShipperMapCollection = applicationStringMaps.EnumerateByApplicationStringGuidAndType(
				security, footNote.IdentityGuid, STRING_MAP_TYPE.FOOT_NOTE_SHIPPER);
			footNote.FootNoteShipToStateMapCollection = applicationStringMaps.EnumerateByApplicationStringGuidAndType(
				security, footNote.IdentityGuid, STRING_MAP_TYPE.FOOT_NOTE_SHIPTO_STATE);
			footNote.FootNoteProductMapCollection = applicationStringMaps.EnumerateByApplicationStringGuidAndType(
				security, footNote.IdentityGuid, STRING_MAP_TYPE.FOOT_NOTE_PRODUCT);
            footNote.FootNoteAdditiveProfileMapCollection = applicationStringMaps.EnumerateByApplicationStringGuidAndType(
                security, footNote.IdentityGuid, STRING_MAP_TYPE.FOOT_NOTE_ADDITIVE_PROFILE);

			return footNote;
		}

		/// <summary>
		/// Gets the identity GUID of the footnote with the specified ID.
		/// </summary>
		/// <param name="security">The security object.</param>
		/// <param name="id">The id of the footnote to find.</param>
		/// <returns>The identity Guid of the specified footnote.</returns>
		public Guid GetIdentityGuid(SecurityClass security, string id)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (!security.HasRight(RIGHT.VIEW_PRODUCTS) && !security.HasRight(RIGHT.MODIFY_PRODUCTS))
			{
				throw new FMInsufficientRightsException();
			}

			var footNote = new FootNoteClass { ID = id, SiteGuid = security.SiteGuid };

			using (var cmd = new SqlCommand())
			{
				footNote.SelectByIDSQL(cmd, security, ContextUtil.IsInTransaction);
				footNote.Load(this.consolidatedDA.GetDataSet(cmd, security));
			}

			return footNote.IdentityGuid;
		}

		/// <summary>
		/// Modifies the specified footnote.
		/// </summary>
		/// <param name="security">The security object.</param>
		/// <param name="footNote">The foot note to save.</param>
		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void Modify(SecurityClass security, FootNoteClass footNote)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (footNote == null)
			{
				throw new ArgumentNullException("footNote");
			}

			if (!security.HasRight(RIGHT.MODIFY_PRODUCTS))
			{
				throw new FMInsufficientRightsException();
			}

			this.Validate(footNote);

			Guid identityGuid = this.GetIdentityGuid(security, footNote.ID);
			if (identityGuid != Guid.Empty && identityGuid != footNote.IdentityGuid)
			{
				throw new Exception("Footnote Exists");
			}

			FootNoteClass oldFootNote = this.Get(security, footNote.IdentityGuid);

 
         if (oldFootNote.IdentityGuid == Guid.Empty)
			{
				throw new Exception("Footnote Not Found");
			}

			footNote.UpdatedDate = DateTimeOffset.Now;
			footNote.UpdatedBy = security.UserID;

			using (var cmd = new SqlCommand())
			{
				footNote.UpdateSQL(cmd);
				this.consolidatedDA.ExecuteQuery(security, cmd);
			}

			var entityToSiteMaps = new EntityToSiteMaps();
			EntityToSiteMapCollectionClass entityToSiteMapCollection = entityToSiteMaps.EnumerateByTypeIDAndGuid(
				security, footNote.EntityType, footNote.IdentityGuid);

			if (footNote.SiteGuid != oldFootNote.SiteGuid)
			{
				// Purge from EntityToSiteMap
				foreach (EntityToSiteMapClass entityToSiteMap in entityToSiteMapCollection)
				{
					entityToSiteMap.ID = footNote.ID;
					entityToSiteMaps.Purge(security, entityToSiteMap);
				}

				// Create Entity to Site Map
				var newEntityToSiteMap = new EntityToSiteMapClass(footNote);
				entityToSiteMaps.Add(security, newEntityToSiteMap, this.GetType().GUID);
			}

			var applicationStringMaps = new ApplicationStringMapsClass();
			applicationStringMaps.ModifyCollection(
				security, footNote.IdentityGuid, footNote.FootNoteShipToMapCollection, oldFootNote.FootNoteShipToMapCollection);
			applicationStringMaps.ModifyCollection(
				security, footNote.IdentityGuid, footNote.FootNoteShipperMapCollection, oldFootNote.FootNoteShipperMapCollection);
			applicationStringMaps.ModifyCollection(
				security, 
				footNote.IdentityGuid, 
				footNote.FootNoteShipToStateMapCollection, 
				oldFootNote.FootNoteShipToStateMapCollection);
			applicationStringMaps.ModifyCollection(
				security, footNote.IdentityGuid, footNote.FootNoteProductMapCollection, oldFootNote.FootNoteProductMapCollection);
            applicationStringMaps.ModifyCollection(
                security, footNote.IdentityGuid, footNote.FootNoteAdditiveProfileMapCollection, oldFootNote.FootNoteAdditiveProfileMapCollection);
        }

		/// <summary>
		/// Purges the specified footnote.
		/// </summary>
		/// <param name="security">The security object.</param>
		/// <param name="identityGuid">The identity GUID of the footnote to purge.</param>
		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void Purge(SecurityClass security, Guid identityGuid)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (!security.HasRight(RIGHT.MODIFY_PRODUCTS) && !security.HasRight(RIGHT.MODIFY_SITES_AND_SITE_GROUPS))
			{
				throw new FMInsufficientRightsException();
			}

			FootNoteClass footNote = this.Get(security, identityGuid);
			if (footNote.IdentityGuid == Guid.Empty)
			{
				throw new Exception("Footnote Not Found");
			}

			var applicationStringMaps = new ApplicationStringMapsClass();
			applicationStringMaps.ModifyCollection(security, footNote.IdentityGuid, null, footNote.FootNoteShipToMapCollection);
			applicationStringMaps.ModifyCollection(security, footNote.IdentityGuid, null, footNote.FootNoteShipperMapCollection);
			applicationStringMaps.ModifyCollection(
				security, footNote.IdentityGuid, null, footNote.FootNoteShipToStateMapCollection);
			applicationStringMaps.ModifyCollection(security, footNote.IdentityGuid, null, footNote.FootNoteProductMapCollection);
            applicationStringMaps.ModifyCollection(security, footNote.IdentityGuid, null, footNote.FootNoteAdditiveProfileMapCollection);

            // Purge from EntityToSiteMap
            var entityToSiteMaps = new EntityToSiteMaps();

			var entityToSiteMapCollection = entityToSiteMaps.EnumerateByTypeIDAndGuid(
				security, footNote.EntityType, identityGuid);

			foreach (EntityToSiteMapClass entityToSiteMap in entityToSiteMapCollection)
			{
				entityToSiteMap.ID = footNote.ID;
				entityToSiteMaps.Purge(security, entityToSiteMap);
			}

			using (var cmd = new SqlCommand())
			{
				footNote.PurgeSQL(cmd);
				this.consolidatedDA.ExecuteQuery(security, cmd);
			}
		}

		#endregion

		#region Explicit Interface Methods

		/// <summary>
		/// Implementation of IDependency Insert.
		/// </summary>
		/// <param name="security">The security object.</param>
		/// <param name="Object">The object being inserted.</param>
		void IDependency.Insert(SecurityClass security, BaseDataObject Object, bool preOperation)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (Object == null)
			{
				throw new ArgumentNullException("Object");
			}
		}

		/// <summary>
		/// Implementation of IDependency Purge.
		/// </summary>
		/// <param name="security">The security object.</param>
		/// <param name="Object">The object being purged.</param>
		void IDependency.Purge(SecurityClass security, BaseDataObject Object)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (Object == null)
			{
				throw new ArgumentNullException("Object");
			}

			// Purge FootNotes
			if (Object is SiteClass)
			{
				var site = (SiteClass)Object;
				FootNoteCollectionClass footNoteCollection = this.Enumerate(security);
				var entityToSiteMaps = new EntityToSiteMaps();
				foreach (FootNoteClass footNote in footNoteCollection)
				{
					if (site.SiteGuid == footNote.SiteGuid)
					{
						this.Purge(security, footNote.IdentityGuid);
					}
					else
					{
						var entityToSiteMap = new EntityToSiteMapClass
							{
								TypeID = footNote.EntityType, 
								SiteGuid = site.SiteGuid, 
								IdentityGuid = footNote.IdentityGuid
							};

						entityToSiteMaps.Purge(security, entityToSiteMap);
					}
				}
			}
			else if (Object is CompanyClass)
			{
				var company = (CompanyClass)Object;

				var applicationStringMaps = new ApplicationStringMapsClass();

				ApplicationStringMapCollectionClass messageCollection = applicationStringMaps.EnumerateByAssignedToGuidAndType(
					security, company.MasterRecordGuid, STRING_MAP_TYPE.FOOT_NOTE_SHIPPER);

				foreach (ApplicationStringMapClass message in messageCollection)
				{
					applicationStringMaps.Purge(security, message.IdentityGuid, message.Type);
				}

				messageCollection = applicationStringMaps.EnumerateByAssignedToGuidAndType(
					security, company.MasterRecordGuid, STRING_MAP_TYPE.FOOT_NOTE_SHIPTO);

				foreach (ApplicationStringMapClass message in messageCollection)
				{
					applicationStringMaps.Purge(security, message.IdentityGuid, message.Type);
				}

				var applicationStrings = new ApplicationStringsClass();
				Guid identityGuid = applicationStrings.GetIdentityGuid(security, STRING_TYPE.SHIPTO_STATE, company.State);
				if (identityGuid != Guid.Empty)
				{
					messageCollection = applicationStringMaps.EnumerateByAssignedToGuidAndType(
						security, identityGuid, STRING_MAP_TYPE.FOOT_NOTE_SHIPTO_STATE);

					foreach (ApplicationStringMapClass message in messageCollection)
					{
						applicationStringMaps.Purge(security, message.IdentityGuid, message.Type);
					}
				}
			}
		}

		/// <summary>
		/// Implementation of IDependency Update.
		/// </summary>
		/// <param name="security">The security object.</param>
		/// <param name="Object">The object being updated.</param>
		void IDependency.Update(SecurityClass security, BaseDataObject Object)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (Object == null)
			{
				throw new ArgumentNullException("Object");
			}

			var site = Object as SiteClass;
			if (site != null)
			{
				FootNoteCollectionClass footNoteCollection = this.Enumerate(security);
				var entityToSiteMaps = new EntityToSiteMaps();
				foreach (FootNoteClass footNote in footNoteCollection)
				{
					if (site.SiteGuid == footNote.SiteGuid)
					{
						EntityToSiteMapCollectionClass entityToSiteMapCollection = entityToSiteMaps.EnumerateByTypeIDAndGuid(
							security, footNote.EntityType, footNote.IdentityGuid);
						foreach (EntityToSiteMapClass entityToSiteMap in entityToSiteMapCollection)
						{
							if (entityToSiteMap.SiteGuid != site.SiteGuid)
							{
								entityToSiteMap.ID = footNote.ID;
								entityToSiteMaps.Purge(security, entityToSiteMap);
							}
						}
					}
				}
			}
		}

		#endregion

		#region Methods

		/// <summary>
		/// Validates the specified foot note.
		/// </summary>
		/// <param name="footNote">The foot note to validate.</param>
		private void Validate(FootNoteClass footNote)
		{
			if (footNote.ID == string.Empty)
			{
				throw new Exception("ID Required");
			}

			if (footNote.ID == "{None}" || footNote.ID == "{Unassigned}" || footNote.ID == "{All}")
			{
				throw new Exception("ID is reserved key word " + footNote.ID);
			}
		}

		#endregion
	}
}