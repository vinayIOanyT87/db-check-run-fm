namespace FMBusinessServices.ServiceClasses
{
	using System;
	using System.Collections.Generic;
	using System.Data;
	using System.Data.SqlClient;
	using System.Security;
	using System.ServiceModel;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.DataObjects;

	using FMBusinessServices.DataAccessLayer;
	using FMBusinessServices.InternalClasses;

	/// <summary>
	/// Summary description for QualificationMapsClass.
	/// </summary>
	[SecuritySafeCriticalAttribute]
	[ServiceBehavior(TransactionIsolationLevel = System.Transactions.IsolationLevel.ReadCommitted)]
	public class QualificationMapsClass : IDependency, IQualificationMaps
	{
		internal ConsolidatedDAClass ConsolidatedDA = new ConsolidatedDAClass();

	    [OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void Add(SecurityClass security, QualificationMapClass qualificationMap)
		{
			if (security == null)
				throw new ArgumentNullException(nameof(security));

			if (qualificationMap == null)
				throw new ArgumentNullException(nameof(qualificationMap));

			qualificationMap.SiteGuid = security.SiteGuid;
			qualificationMap.CreatedDate = DateTimeOffset.Now;
			qualificationMap.CreatedBy = security.UserID;
			qualificationMap.UpdatedDate = qualificationMap.CreatedDate;
			qualificationMap.UpdatedBy = security.UserID;


			try
			{
				using (SqlCommand cmd = qualificationMap.InsertSQL_)
				{
				    this.ConsolidatedDA.ExecuteQuery(security, cmd);
				}
			}
			catch (SqlException e)
			{
				if (-1 != e.Message.IndexOf("CK_tblQualificationsMap_TWICConstraint", StringComparison.Ordinal))
					throw new Exception("Duplicate TWIC Card");
			}

		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void Modify(SecurityClass security, QualificationMapClass qualificationMap)
		{
			if (security == null)
				throw new ArgumentNullException(nameof(security));

			if (qualificationMap == null)
				throw new ArgumentNullException(nameof(qualificationMap));

			qualificationMap.CreatedBy = security.UserID;
			qualificationMap.UpdatedDate = qualificationMap.CreatedDate;

			try
			{
				using (SqlCommand cmd = qualificationMap.UpdateSQL)
				{
				    this.ConsolidatedDA.ExecuteQuery(security, cmd);
				}
			}
			catch (SqlException e)
			{
				if (-1 != e.Message.IndexOf("CK_tblQualificationsMap_TWICConstraint", StringComparison.Ordinal))
					throw new Exception("Duplicate TWIC Card");
			}

		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void PurgeByPrimaryKey(SecurityClass security, Guid primarykey, QUALIFICATION_MAP_TYPE qualificationMapType)
		{
			if (security == null)
			{
				throw new ArgumentNullException(nameof(security));
			}

			var qualificationMap = new QualificationMapClass
			{
				IdentityGuid = primarykey,
				Type = qualificationMapType
			};

			using (SqlCommand cmd = qualificationMap.PurgeSQL)
			{
			    this.ConsolidatedDA.ExecuteQuery(security, cmd);
			}
		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void Purge(SecurityClass security, Guid targetGuid, Guid newAssignedGuid, QUALIFICATION_MAP_TYPE type)
		{
			if (security == null)
				throw new ArgumentNullException(nameof(security));

		    QualificationMapClass qualificationMap = new QualificationMapClass
		                                             {
		                                                 IdentityGuid = targetGuid,
		                                                 AssignedGuid = newAssignedGuid,
		                                                 Type = type
		                                             };
		    using (SqlCommand cmd = qualificationMap.PurgeSQL)
			{
			    this.ConsolidatedDA.ExecuteQuery(security, cmd);
			}
		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void PurgeHistoricalRecord(SecurityClass security, Guid targetGuid, Guid newAssignedGuid, QUALIFICATION_MAP_TYPE type, DateTimeOffset updatedDate)
		{
			if (security == null)
				throw new ArgumentNullException(nameof(security));

		    // ReSharper disable once UnusedVariable
            // Waiting on below TODO:
		    QualificationMapClass qualificationMap = new QualificationMapClass
		                                             {
		                                                 IdentityGuid = targetGuid,
		                                                 AssignedGuid = newAssignedGuid,
		                                                 Type = type,
		                                                 UpdatedDate = updatedDate
		                                             };
		    // TODO: Find implementation of QualificationMapClass.PurgeHistoricalRecordSQL
			//using (SqlCommand cmd = QualificationMap.PurgeHistoricalRecordSQL)
			//{
			//	ConsolidatedDA.ExecuteQuery(security, cmd);
			//}
		}

		public QualificationMapClass Get(SecurityClass security, Guid targetGuid, Guid newAssignedGuid, QUALIFICATION_MAP_TYPE type)
		{
			if (security == null)
				throw new ArgumentNullException(nameof(security));

		    QualificationMapClass qualificationMap = new QualificationMapClass
		                                             {
		                                                 IdentityGuid = targetGuid,
		                                                 AssignedGuid = newAssignedGuid,
		                                                 Type = type
		                                             };

		    using (SqlCommand cmd = qualificationMap.SelectSQL(ContextUtil.IsInTransaction))
			{
				qualificationMap.Load(this.ConsolidatedDA.GetDataSet(cmd, security));
			}
			return qualificationMap;
		}

		/// <summary>
		/// The enumerate company certificate and permit.
		/// </summary>
		/// <param name="security">
		/// The security.
		/// </param>
		/// <param name="targetSiteGuid">
		/// The target site globally unique identifier.
		/// </param>
		/// <returns>
		/// The <see cref="QualificationMapCollectionClass"/>.
		/// </returns>
		/// <exception cref="ArgumentNullException">
		/// Security is null
		/// </exception>
		public QualificationMapCollectionClass EnumerateCompanyCertificateAndPermitForExport(SecurityClass security, Guid targetSiteGuid)
		{
			if (security == null)
			{
				throw new ArgumentNullException(nameof(security));
			}

			var qualificationMap = new QualificationMapClass();
			DataSet set;
			qualificationMap.Type = QUALIFICATION_MAP_TYPE.COMPANY_CERTIFICATE_AND_PERMIT_TO_COMPANY;
			qualificationMap.SiteGuid = security.SiteGuid;

			using (var cmd = new SqlCommand())
			{
				// Get the qualifications mapped to companies that belong to the provided site or are assigned to the provided site
				cmd.CommandText = "SELECT MapTable.*,  QTable.ID AS QualificationID, QTable.Reoccurrence as ReoccurrenceID "
				                  + "FROM map.tblQualificationCompanyCertificateAndPermitToCompany MapTable "
				                  + "INNER JOIN tblCompanies ON tblCompanies.CompanyGuid = MapTable.CompanyGuid "
				                  + "INNER JOIN tblQualifications QTable ON MapTable.QualificationGuid = QTable.QualificationGuid "
				                  + "WHERE (tblCompanies.SiteGuid = @TargetSiteGuid "
				                  + "OR EXISTS (SELECT * FROM map.tblEntityCompanyToSite "
				                  + "WHERE map.tblEntityCompanyToSite.CompanyGuid = tblCompanies.CompanyGuid AND SiteGuid = @TargetSiteGuid))";

				cmd.Parameters.Add("@TargetSiteGuid", SqlDbType.UniqueIdentifier);
				cmd.Parameters["@TargetSiteGuid"].Value = targetSiteGuid;
				set = this.ConsolidatedDA.GetDataSet(cmd, security);
			}
			var qualificationMapCollection = new QualificationMapCollectionClass();
			var table = set.Tables[0];
			while (table.Rows.Count != 0)
			{
				qualificationMap = new QualificationMapClass
									   {
										   Type =
											   QUALIFICATION_MAP_TYPE.COMPANY_CERTIFICATE_AND_PERMIT_TO_COMPANY
									   };
				qualificationMap.Load(set);
				qualificationMapCollection.Add(qualificationMap);
				table.Rows.RemoveAt(0);
			}

			return qualificationMapCollection;
		}


		public QualificationMapCollectionClass EnumerateByGuidAndType(SecurityClass security, Guid targetGuid, QUALIFICATION_MAP_TYPE type, bool getHistoricalData)
		{
			if (security == null)
				throw new ArgumentNullException(nameof(security));

		    QualificationMapClass qualificationMap = new QualificationMapClass
		                                             {
		                                                 AssigneeGuid = targetGuid,
		                                                 Type = type,
		                                                 SiteGuid = security.SiteGuid
		                                             };
		    DataSet set;

            if (getHistoricalData)
			{
				using (SqlCommand cmd = qualificationMap.EnumerateHistoricalRecordsByIndexAndTypeSQL)
				{
					set = this.ConsolidatedDA.GetDataSet(cmd, security);
				}
			}
			else
			{
				using (SqlCommand cmd = qualificationMap.EnumerateByGuidAndTypeSQL)
				{
					set = this.ConsolidatedDA.GetDataSet(cmd, security);
				}
			}
			QualificationMapCollectionClass qualificationMapCollection = new QualificationMapCollectionClass();

			DataTable table = set.Tables[0];
			while (table.Rows.Count != 0)
			{
			    qualificationMap = new QualificationMapClass { Type = type };
			    qualificationMap.Load(set);
				qualificationMapCollection.Add(qualificationMap);
				table.Rows.RemoveAt(0);
			}

			return qualificationMapCollection;
		}

		public QualificationMapCollectionClass EnumerateWhereQualificationOrTrainingIsUsed(SecurityClass security, Guid targetGuid)
		{
			if (security == null)
				throw new ArgumentNullException(nameof(security));

			QualificationMapClass qualificationMap = new QualificationMapClass();
			DataSet set;
			qualificationMap.IdentityGuid = targetGuid;
			qualificationMap.SiteGuid = security.SiteGuid;

			using (SqlCommand cmd = qualificationMap.EnumerateWhereQualificationTrainingIsUsedSQL)
			{
				set = this.ConsolidatedDA.GetDataSet(cmd, security);
			}

			QualificationMapCollectionClass qualificationMapCollection = new QualificationMapCollectionClass();

			DataTable table = set.Tables[0];
			while (table.Rows.Count != 0)
			{
				qualificationMap = new QualificationMapClass();
				qualificationMap.Load(set);
				qualificationMapCollection.Add(qualificationMap);
				table.Rows.RemoveAt(0);
			}

			return qualificationMapCollection;
		}

		public QualificationMapCollectionClass EnumerateByAssignedGuid(SecurityClass security, Guid targetGuid)
		{
			if (security == null)
				throw new ArgumentNullException(nameof(security));

		    QualificationMapClass qualificationMap = new QualificationMapClass
		                                             {
		                                                 AssignedGuid = targetGuid,
		                                                 SiteGuid = security.SiteGuid
		                                             };
		    DataSet set;
			using (SqlCommand cmd = qualificationMap.EnumerateByAssignedGuidSQL)
			{
				set = this.ConsolidatedDA.GetDataSet(cmd, security);
			}
			QualificationMapCollectionClass qualificationMapCollection = new QualificationMapCollectionClass();

			DataTable table = set.Tables[0];
			while (table.Rows.Count != 0)
			{
				qualificationMap = new QualificationMapClass();
				qualificationMap.Load(set);
				qualificationMapCollection.Add(qualificationMap);
				table.Rows.RemoveAt(0);
			}

			return qualificationMapCollection;
		}

		/// <summary>
		/// Examine the qualification maps present before the change and compare them to the ones after the change. 
		/// Determine which records need to be deleted, modified, or inserted
		/// </summary>
		/// <param name="security">Contains security information.</param>
		/// <param name="targetGuid">The guid of the entity the qualification map is assigned to</param>
		/// <param name="newQualificationMapCollection">The qualification maps after the modification</param>
		/// <param name="existingQualificationMapCollection">The qualification maps before the modification</param>
		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void ModifyCollection(
			SecurityClass security,
			Guid targetGuid,
			QualificationMapCollectionClass newQualificationMapCollection,
			QualificationMapCollectionClass existingQualificationMapCollection)
		{
			if (security == null)
			{
				throw new ArgumentNullException(nameof(security));
			}

			List<QualificationMapClass> recordsToDelete = new List<QualificationMapClass>();
			List<QualificationMapClass> recordsBeingReplaced = new List<QualificationMapClass>();

			// Any record that does not exist in the new collection with a matching AssignedGuid and Number (Number is the ID column in the database) should be deleted.
			// If either the number or the assigned guid is modified, we must delete the record before making any modifications or adding new records to avoid potentially violating unique constraints.
			// This is to prevent a situation where a user swaps the number assigned to two qualifications, 
			// if we modify them both then we end up violating the unique constraint on Number (ID in the database)
			// because at one given point in time we have two qualification maps with the same number in the db.
			if (existingQualificationMapCollection != null)
			{
				foreach (QualificationMapClass existingMap in existingQualificationMapCollection)
				{
					QualificationMapClass reusedMap = null;
					reusedMap = newQualificationMapCollection?.Find(newQualificationMap => newQualificationMap.IdentityGuid == existingMap.IdentityGuid);

					if (newQualificationMapCollection?.Find(newQualificationMap => newQualificationMap.AssignedGuid == existingMap.AssignedGuid
					                                                               && newQualificationMap.Number == existingMap.Number) == null)
					{
						recordsToDelete.Add(existingMap);

						// If an existing mapping record was updated with a new AssignedGuid and Number; we need to keep track of this and make sure 
						// we use a new IdentityGuid on the updated record that gets inserted later.  Re-using the same IdentityGuid after it's been deleted will cause
						// problems with synchronization tracking because it will detect that the record was deleted and we don't allow "undeleting" of records by simply 
						// re-inserting the same primary key.
						if (null != reusedMap)
						{
							recordsBeingReplaced.Add(reusedMap);
						}
					}
				}
			}

			// Delete records that we determined should be deleted
			foreach (QualificationMapClass recordToDelete in recordsToDelete)
			{
				this.Purge(security, recordToDelete.IdentityGuid, recordToDelete.AssignedGuid, recordToDelete.Type);

				// If it's deleted, it no longer exists. We should remove it from the collection so we know to insert the new record later. 
				// We can do the search based on the AssignedGuid since there should never be two items
				// in the collection assigned to the same qualification.
			    existingQualificationMapCollection?.RemoveAll(existingMap => existingMap.AssignedGuid == recordToDelete.AssignedGuid);
			}

			// Any record that is in the new collection could either require creating a new record or modifying an existing one.
			if (newQualificationMapCollection != null)
			{
				foreach (QualificationMapClass newQualificationMap in newQualificationMapCollection)
				{
					newQualificationMap.AssigneeGuid = targetGuid;

					// Determine if anything in the new collection was deleted above and we're attempting to re-use the underlying IdentityGuid.
					// If it was added to the recordsBeingReplaced collection; then it had an IdentityGuid that matched a new record but the AssignedGuid and Number were both changed.
					// These will end up being re-inserted into the database but before we do, generate a New IdentityGuid for it. 
					QualificationMapClass reusedMap = null;

					if (recordsBeingReplaced != null)
					{
						reusedMap = recordsBeingReplaced.Find(
								reusedMapping => newQualificationMap.IdentityGuid == reusedMapping.IdentityGuid);
					}

					if (null != reusedMap)
					{
						newQualificationMap.IdentityGuid = Guid.NewGuid();
					}

					// Determine if a matching record exists in the old collection.
					// We can do the search based on the AssignedGuid since there should never be two items
					// in the collection assigned to the same qualification.
					QualificationMapClass matchingMap = null;

					if (existingQualificationMapCollection != null)
					{
						matchingMap = existingQualificationMapCollection.Find(
								existingQualificationMap => newQualificationMap.AssignedGuid == existingQualificationMap.AssignedGuid);
					}
			
					if (matchingMap == null)
					{
						// If a matching record does not exist in the old collection we should create a new one.
					    if (newQualificationMap.IdentityGuid == Guid.Empty)
					    {
					        newQualificationMap.IdentityGuid = Guid.NewGuid();
					    }
						this.Add(security, newQualificationMap);
					}
					else
					{
						// A matching record exists, we should modify it.
						// First, check to see if we need to create an historical record for this item
						if (QualificationMapClass.IsHistoricalRecordType(matchingMap.Type)
							&& (matchingMap.Sequence != newQualificationMap.Sequence
								|| matchingMap.ExpirationDate.Value != newQualificationMap.ExpirationDate.Value
								|| matchingMap.DateCompleted.Value != newQualificationMap.DateCompleted.Value
								|| matchingMap.DateDue.Value != newQualificationMap.DateDue.Value
								|| matchingMap.ID != newQualificationMap.ID
								|| matchingMap.Number != newQualificationMap.Number
								|| matchingMap.Instructor != newQualificationMap.Instructor
								|| matchingMap.Rating != newQualificationMap.Rating))
						{
							matchingMap.UpdatedDate = DateTimeOffset.Now;
							matchingMap.HistoricalRecord = true;
						    matchingMap.IdentityGuid = Guid.NewGuid();
							this.Add(security, matchingMap);
						}

						if (matchingMap.Sequence != newQualificationMap.Sequence
							|| matchingMap.ExpirationDate.Value != newQualificationMap.ExpirationDate.Value
							|| matchingMap.DateCompleted.Value != newQualificationMap.DateCompleted.Value
							|| matchingMap.DateDue.Value != newQualificationMap.DateDue.Value
							|| matchingMap.ID != newQualificationMap.ID
							|| matchingMap.Number != newQualificationMap.Number
							|| matchingMap.Instructor != newQualificationMap.Instructor
							|| matchingMap.Rating != newQualificationMap.Rating)
						{
							// Modify the existing record
							this.Modify(security, newQualificationMap);
						}
					}
				}
			}
		}

		void IDependency.Insert(SecurityClass security, BaseDataObject obj, bool preOperation)
		{
			if (security == null)
				throw new ArgumentNullException(nameof(security));

			if (obj == null)
				throw new ArgumentNullException(nameof(obj));

		}

		void IDependency.Update(SecurityClass security, BaseDataObject obj)
		{
			if (security == null)
				throw new ArgumentNullException(nameof(security));

			if (obj == null)
				throw new ArgumentNullException(nameof(obj));
		}

		void IDependency.Purge(SecurityClass security, BaseDataObject obj)
		{
			if (security == null)
				throw new ArgumentNullException(nameof(security));

			if (obj == null)
				throw new ArgumentNullException(nameof(obj));

		    var qualification = obj as QualificationClass;
		    if (qualification != null)
			{
			    var qualificationMapCollection = this.EnumerateByAssignedGuid(security, qualification.IdentityGuid);
				foreach (QualificationMapClass qualificationMap in qualificationMapCollection)
				    this.Purge(security,
							qualificationMap.IdentityGuid,
							qualificationMap.AssignedGuid,
							qualificationMap.Type);

				return;
			}

		    var company = obj as CompanyClass;
		    if (company != null)
			{
				QUALIFICATION_MAP_TYPE[] type = { QUALIFICATION_MAP_TYPE.COMPANY_CERTIFICATE_AND_PERMIT_TO_COMPANY };
			    for (int iType = 0; iType < 1; iType++)
				{
				    var qualificationMapCollection = this.EnumerateByGuidAndType(security, company.IdentityGuid, type[iType], false);
				    foreach (QualificationMapClass qualificationMap in qualificationMapCollection)
					    this.Purge(security,
								qualificationMap.IdentityGuid,
								qualificationMap.AssignedGuid,
								qualificationMap.Type);
				}
			    return;
			}

		    var person = obj as PersonClass;
		    if (person != null)
			{
				QUALIFICATION_MAP_TYPE[] type ={	QUALIFICATION_MAP_TYPE.PERSON_LICENSE_TO_PERSON,
															QUALIFICATION_MAP_TYPE.PERSON_QUALIFICATION_TO_PERSON};

			    for (int iType = 0; iType < 2; iType++)
				{
				    var qualificationMapCollection = this.EnumerateByGuidAndType(security, person.IdentityGuid, type[iType], false);
				    foreach (QualificationMapClass qualificationMap in qualificationMapCollection)
					    this.Purge(security,
								qualificationMap.IdentityGuid,
								qualificationMap.AssignedGuid,
								qualificationMap.Type);
				}
			    return;
			}

		    var equipment = obj as EquipmentClass;
		    if (equipment != null)
			{
				QUALIFICATION_MAP_TYPE[] type ={	QUALIFICATION_MAP_TYPE.EQUIPMENT_TAG_AND_LICENSE_TO_EQUIPMENT,
															QUALIFICATION_MAP_TYPE.EQUIPMENT_TEST_AND_INSPECTION_TO_EQUIPMENT};

			    for (int iType = 0; iType < 2; iType++)
				{
				    var qualificationMapCollection = this.EnumerateByGuidAndType(security, equipment.MasterRecordGuid, type[iType], false);
				    foreach (QualificationMapClass qualificationMap in qualificationMapCollection)
					    this.Purge(security,
								qualificationMap.IdentityGuid,
								qualificationMap.AssignedGuid,
								qualificationMap.Type);
				}
			}
		}
	}
}
