using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Security;
using System.ServiceModel;
using System.Web;
using System.Collections;

using FMBusinessServices.DataAccessLayer;
using FMBusinessObjects.DataObjects;
using FMBusinessObjects.BusinessInterfaces;
using FMBusinessServices.InternalClasses;

namespace FMBusinessServices.ServiceClasses
{
	/// <summary>
	/// Summary description for FieldLevelConfigMapsClass.
	/// </summary>
	[SecuritySafeCriticalAttribute]
	[ServiceBehavior(TransactionIsolationLevel = System.Transactions.IsolationLevel.ReadCommitted)]
	public class FieldLevelConfigMapsClass : IFieldLevelConfigMaps
	{
		internal ConsolidatedDAClass ConsolidatedDA = new ConsolidatedDAClass();

		public FieldLevelConfigMapsClass()
		{
		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
        public void Update(SecurityClass security, FieldLevelConfigCollectionClass flcCollection, Guid targetSitegroupGuid)
		{
			if (security == null)
				throw new ArgumentNullException("Security");

			if (flcCollection == null)
				throw new ArgumentNullException("FieldLevelConfigMap");

            DataTable dtRecordsToUpdate = new DataTable();
            dtRecordsToUpdate.Columns.Add("FieldConfigGuid", typeof(Guid));
            dtRecordsToUpdate.Columns.Add("EntitySegmentTemplateGuid", typeof(Guid));
            dtRecordsToUpdate.Columns.Add("EntityTypeId", typeof(string));
            dtRecordsToUpdate.Columns.Add("SiteGroupGuid", typeof(Guid));
            dtRecordsToUpdate.Columns.Add("FilterFieldName", typeof(string));
            dtRecordsToUpdate.Columns.Add("FilterValueGuid", typeof(Guid));
            dtRecordsToUpdate.Columns.Add("FilterValueName", typeof(string));
            dtRecordsToUpdate.Columns.Add("TargetField", typeof(string));
            dtRecordsToUpdate.Columns.Add("IsExternalAttribute", typeof(bool));
            dtRecordsToUpdate.Columns.Add("InternalField", typeof(string));
            dtRecordsToUpdate.Columns.Add("InheritedControlMode", typeof(string));
            dtRecordsToUpdate.Columns.Add("ForwardControlMode", typeof(string));
            dtRecordsToUpdate.Columns.Add("HierarchyLevel", typeof(int));

            FieldLevelConfigClass flc = null;
            string inheritedControlModeStr = null;
            string forwardControlModeStr = null;
            for (int i = 0; i < flcCollection.Count; i++)
            {
                flc = flcCollection[i];
                if (flc.InheritedControlMode != FieldLevelConfigClass.FIELD_CONTROL_MODE.Unknown)
                    inheritedControlModeStr = flc.InheritedControlMode.ToString();
                if (flc.ForwardControlMode != FieldLevelConfigClass.FIELD_CONTROL_MODE.Unknown)
                    forwardControlModeStr = flc.ForwardControlMode.ToString();
                dtRecordsToUpdate.Rows.Add(flc.IdentityGuid, flc.EntitySegmentTemplateGuid, flc.EntityTypeId, flc.SiteGroupGuid, flc.FilterFieldName, flc.FilterValueGuid, flc.FilterValueName, flc.TargetField, flc.IsExternalAttribute, flc.InternalFieldName, inheritedControlModeStr, forwardControlModeStr, DBNull.Value);
            }            

			using (SqlCommand cmd = new SqlCommand())
			{
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "erv.usp_UpdateFLCForwardControlMode";
                cmd.CommandTimeout = 0;
                SqlParameter sqlParamRecordsToUpdate = cmd.Parameters.AddWithValue("@FieldLevelConfigParamTable", dtRecordsToUpdate);
                sqlParamRecordsToUpdate.SqlDbType = SqlDbType.Structured;
                sqlParamRecordsToUpdate.TypeName = "erv.utt_FieldLevelConfig";
                cmd.Parameters.Add("@SiteGroupGuid", SqlDbType.UniqueIdentifier);
                cmd.Parameters["@SiteGroupGuid"].Value = targetSitegroupGuid;
                cmd.Parameters.Add("@UserId", SqlDbType.NVarChar, 100);
                cmd.Parameters["@UserId"].Value = security.UserID;
                ConsolidatedDA.ExecuteQuery(security, cmd);
			}
		}


        public FieldLevelConfigCollectionClass GetFieldLevelConfigMatrix(SecurityClass security, string entityTypeId, Guid sitegroupGuid, string filterFieldName, bool ignoreFilterValues, Guid filterValueGuid, string targetField, FieldLevelConfigClass.FIELD_CONTROL_MODE controlMode, bool includeChildrenSiteGroups)
		{
			if (security == null)
				throw new ArgumentNullException("Security");

            DataSet Set = null;
            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "erv.usp_GetFieldLevelConfigMatrix";
                cmd.Parameters.Add("@EntityTypeId", SqlDbType.NVarChar, 100);
                if (entityTypeId == null)
                    cmd.Parameters["@EntityTypeId"].Value = DBNull.Value;
                else
                    cmd.Parameters["@EntityTypeId"].Value = entityTypeId;                
                cmd.Parameters.Add("@SiteGroupGuid", SqlDbType.UniqueIdentifier);
                if (sitegroupGuid == Guid.Empty)
                    cmd.Parameters["@SiteGroupGuid"].Value = DBNull.Value;
                else
                    cmd.Parameters["@SiteGroupGuid"].Value = sitegroupGuid;
                cmd.Parameters.Add("@FilterFieldName", SqlDbType.NVarChar, 100);
                if (filterFieldName == null)
                    cmd.Parameters["@FilterFieldName"].Value = DBNull.Value;
                else
                    cmd.Parameters["@FilterFieldName"].Value = filterFieldName;
                cmd.Parameters.Add("@FilterValueGuid", SqlDbType.UniqueIdentifier);
                if (ignoreFilterValues)
                    cmd.Parameters["@FilterValueGuid"].Value = DBNull.Value;
                else
                    cmd.Parameters["@FilterValueGuid"].Value = filterValueGuid;
                cmd.Parameters.Add("@TargetField", SqlDbType.NVarChar, 100);
                if (targetField == null)
                    cmd.Parameters["@TargetField"].Value = DBNull.Value;
                else
                    cmd.Parameters["@TargetField"].Value = targetField;
                cmd.Parameters.Add("@ControlMode", SqlDbType.NVarChar, 20);
                if (controlMode == FieldLevelConfigClass.FIELD_CONTROL_MODE.Unknown)
                    cmd.Parameters["@ControlMode"].Value = DBNull.Value;
                else
                    cmd.Parameters["@ControlMode"].Value = controlMode.ToString();
                cmd.Parameters.Add("@IncludeChildrenSiteGroups", SqlDbType.Bit);
                cmd.Parameters["@IncludeChildrenSiteGroups"].Value = includeChildrenSiteGroups;
                Set = ConsolidatedDA.GetDataSet(cmd, security);
            }

            FieldLevelConfigCollectionClass flcCollection = new FieldLevelConfigCollectionClass();

            DataTable Table = Set.Tables[0];
            for (int i = 0; i < Table.Rows.Count; i++)
            {
                FieldLevelConfigClass flc = new FieldLevelConfigClass();
                flc.Load(Table.Rows[i]);
                flc.FieldLevelConfigMatrixIndex = i;
                flcCollection.Add(flc);
            }
            return flcCollection;
		}


        public Hashtable GetEntityTypes(SecurityClass security)
        {
            if (security == null)
                throw new ArgumentNullException("Security");
            Hashtable htResult = new Hashtable();
            DataSet Set = null;
            DataRow Row = null;
            string entityTypeKey = null;
            string entityTypeValue = null;
            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "erv.usp_GetFLCEntityTypes";
                Set = ConsolidatedDA.GetDataSet(cmd, security);
            }
            DataTable Table = Set.Tables[0];
            for (int i = 0; i < Table.Rows.Count; i++)
            {
                Row = Table.Rows[i];
                entityTypeKey = DataObject.getValue<string>(Row["EntityTypeId"], null);
                entityTypeValue = DataObject.getValue<string>(Row["EntityTypeDisplayName"], null);
                htResult.Add(entityTypeKey, entityTypeValue);
            }
            return htResult;
        }


        /// <summary>
        /// Retrieves the site hierarchy below a given sitegroup. Includes the given sitegroup as well.
        /// </summary>
        /// <param name="security"></param>
        /// <param name="siteGuid">Guid of the site or sitegroup for which the lower hierarchy is to be retrieved. If a site guid is provided (instead of a sitegroup guid) then the query only returns the given site itself.</param>
        /// <param name="maxHierarchyDepth">Maximum hierarchy levels for which to retrieve children nodes. NULL: No hierarchy limit. Retrieved all children nodes. 0: Returns only the given siteGuid provided. 1: Returns the given siteGuid, and all its immediate children nodes. </param>
        /// <param name="siteGroupsOnly">True: Search limited to sitegrouops only. False: Search extended to sites as well.</param>
        /// <returns></returns>
        public SortedList GetSiteHierarchy(SecurityClass security, Guid siteGuid, int? maxHierarchyDepth, bool siteGroupsOnly)
        {
            if (security == null)
                throw new ArgumentNullException("Security");
            SortedList slResult = new SortedList();
            DataSet Set = null;
            DataRow Row = null;
            Guid siteKey = Guid.Empty;
            string siteValue = null;
            int hierarchylevel = 0;
            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "erv.usp_GetFLCSiteHierarchy";                
                cmd.Parameters.Add("@SiteGuid", SqlDbType.UniqueIdentifier);
                if (siteGuid == Guid.Empty)
                    cmd.Parameters["@SiteGuid"].Value = DBNull.Value;
                else
                    cmd.Parameters["@SiteGuid"].Value = siteGuid;
                cmd.Parameters.Add("@SiteGroupsOnly", SqlDbType.Bit);
                cmd.Parameters["@SiteGroupsOnly"].Value = siteGroupsOnly;
                Set = ConsolidatedDA.GetDataSet(cmd, security);
            }
            DataTable Table = Set.Tables[0];            
            for (int i = 0; i < Table.Rows.Count; i++)
            {
                Row = Table.Rows[i];
                siteKey = DataObject.getValue<Guid>(Row["SiteGuid"], Guid.Empty);
                siteValue = DataObject.getValue<string>(Row["SiteId"], null);
                hierarchylevel = DataObject.getValue<int>(Row["HierarchyLevel"], 0);
                if ((maxHierarchyDepth == null) || (hierarchylevel <= maxHierarchyDepth))
                    slResult.Add(siteValue, siteKey);
            }
            return slResult;
        }


        public Hashtable GetFilters(SecurityClass security, string entityTypeId)
        {
            if (security == null)
                throw new ArgumentNullException("Security");
            Hashtable htResult = new Hashtable();
            DataSet Set = null;
            DataRow Row = null;
            string filterKey = null;
            string filterValue = null;
            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "erv.usp_GetFLCFilters";
                cmd.Parameters.Add("@EntityTypeId", SqlDbType.NVarChar, 100);
                cmd.Parameters["@EntityTypeId"].Value = entityTypeId;
                Set = ConsolidatedDA.GetDataSet(cmd, security);
            }
            DataTable Table = Set.Tables[0];
            for (int i = 0; i < Table.Rows.Count; i++)
            {
                Row = Table.Rows[i];
                filterKey = DataObject.getValue<string>(Row["FilterFieldName"], null);
                filterValue = DataObject.getValue<string>(Row["FilterDisplayName"], null);
                htResult.Add(filterKey, filterValue);
            }
            return htResult;
        }


        public Hashtable GetFilterValues(SecurityClass security, string entityTypeId, Guid siteGuid, string filterFieldName)
        {
            if (security == null)
                throw new ArgumentNullException("Security");
            Hashtable htResult = new Hashtable();
            DataSet Set = null;
            DataRow Row = null;
            Guid filterKey = Guid.Empty;
            string filterValue = null;
            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "erv.usp_GetFLCFilterValues";
                cmd.Parameters.Add("@EntityTypeId", SqlDbType.NVarChar, 100);
                cmd.Parameters["@EntityTypeId"].Value = entityTypeId;
                cmd.Parameters.Add("@SiteGuid", SqlDbType.UniqueIdentifier);
                if (siteGuid == Guid.Empty)
                    cmd.Parameters["@SiteGuid"].Value = DBNull.Value;
                else
                    cmd.Parameters["@SiteGuid"].Value = siteGuid;
                cmd.Parameters.Add("@FilterFieldName", SqlDbType.NVarChar, 100);
                cmd.Parameters["@FilterFieldName"].Value = filterFieldName;
                Set = ConsolidatedDA.GetDataSet(cmd, security);
            }
            DataTable Table = Set.Tables[0];
            for (int i = 0; i < Table.Rows.Count; i++)
            {
                Row = Table.Rows[i];
                filterKey = DataObject.getValue<Guid>(Row["FilterValueGuid"], Guid.Empty);
                filterValue = DataObject.getValue<string>(Row["FilterValueName"], null);
                htResult.Add(filterKey, filterValue);
            }
            return htResult;
        }




        public Hashtable GetTargetFields(SecurityClass security, string entityTypeId)
        {
            if (security == null)
                throw new ArgumentNullException("Security");
            Hashtable htResult = new Hashtable();
            DataSet Set = null;
            DataRow Row = null;
            string fieldKey = null;
            string fieldValue = null;
            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "erv.usp_GetFLCTargetFields";
                cmd.Parameters.Add("@EntityTypeId", SqlDbType.NVarChar, 100);
                cmd.Parameters["@EntityTypeId"].Value = entityTypeId;
                Set = ConsolidatedDA.GetDataSet(cmd, security);
            }
            DataTable Table = Set.Tables[0];
            for (int i = 0; i < Table.Rows.Count; i++)
            {
                Row = Table.Rows[i];
                fieldKey = DataObject.getValue<string>(Row["TargetField"], null);
                fieldValue = fieldKey;
                htResult.Add(fieldKey, fieldValue);
            }
            return htResult;
        }


        private Guid GetAssignedFromSite(SecurityClass security, string entityTypeId, Guid recordGuid, Guid ownerSiteGuid)
        {
            Object result = null;
            if (security == null)
                throw new ArgumentNullException("Security");
            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "erv.udf_GetEntityAssignedFromSite";
                cmd.Parameters.Add("@EntityTypeId", SqlDbType.NVarChar, 100);
                cmd.Parameters["@EntityTypeId"].Value = entityTypeId;
                cmd.Parameters.Add("@EntityGuid", SqlDbType.UniqueIdentifier);
                cmd.Parameters["@EntityGuid"].Value = recordGuid;
                cmd.Parameters.Add("@AssignedToSiteGuid", SqlDbType.UniqueIdentifier);
                cmd.Parameters["@AssignedToSiteGuid"].Value = ownerSiteGuid;                
                SqlParameter sqlParamResult = cmd.Parameters.Add("@Result", SqlDbType.UniqueIdentifier);
                sqlParamResult.Direction = ParameterDirection.ReturnValue;                
                result = ConsolidatedDA.ExecuteScalar(cmd, security);                                    
            }
            if (result != null)
                return (Guid)result;
            return Guid.Empty;
        }


        public bool  IsFieldRecordVersionSpecific(SecurityClass security, string entityTypeId, Guid recordGuid, Guid masterRecordGuid, Guid ownerSiteGuid, string targetField)
        {
            if (security == null)
                throw new ArgumentNullException("Security");
            if (recordGuid == masterRecordGuid)
                return true;

            Guid assignedFromSiteGroupGuid = GetAssignedFromSite(security, entityTypeId, recordGuid, ownerSiteGuid);

            DataSet Set = null;
            DataRow Row = null;
            string field = null;
            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "erv.usp_GetRecordVersioningFields";
                cmd.Parameters.Add("@EntityTypeId", SqlDbType.NVarChar, 100);
                cmd.Parameters["@EntityTypeId"].Value = entityTypeId;
                cmd.Parameters.Add("@EntityMasterRecGuid", SqlDbType.UniqueIdentifier);
                cmd.Parameters["@EntityMasterRecGuid"].Value = masterRecordGuid;
                cmd.Parameters.Add("@AssignedFromSiteGuid", SqlDbType.UniqueIdentifier);
                cmd.Parameters["@AssignedFromSiteGuid"].Value = assignedFromSiteGroupGuid;
                cmd.Parameters.Add("@FieldLevelControlMode", SqlDbType.NVarChar, 40);
                cmd.Parameters["@FieldLevelControlMode"].Value = FieldLevelConfigClass.FLCModeVSandGS;

                Set = ConsolidatedDA.GetDataSet(cmd, security);
            }
            DataTable Table = Set.Tables[0];
            for (int i = 0; i < Table.Rows.Count; i++)
            {
                Row = Table.Rows[i];
                field = DataObject.getValue<string>(Row["TargetField"], null);
                if (field == targetField)
                    return true;
            }
            return false;
        }



        public void ProcessSiteAssignmentChange(SecurityClass security, Guid targetSiteGroupGuid, SiteToSiteMapCollectionClass siteAssignmentsBefore, SiteToSiteMapCollectionClass siteAssignmentsAfter)
        {
            if (security == null)
                throw new ArgumentNullException("Security");

            DataTable dtSiteAssignmentsBefore = new DataTable();
            dtSiteAssignmentsBefore.Columns.Add("SiteGuid", typeof(Guid));

            DataTable dtSiteAssignmentsAfter = new DataTable();
            dtSiteAssignmentsAfter.Columns.Add("SiteGuid", typeof(Guid));

            for (int i = 0; i < siteAssignmentsBefore.Count; i++)
            {
                dtSiteAssignmentsBefore.Rows.Add(siteAssignmentsBefore[i].ChildSiteGuid);
            }
            for (int i = 0; i < siteAssignmentsAfter.Count; i++)
            {
                dtSiteAssignmentsAfter.Rows.Add(siteAssignmentsAfter[i].ChildSiteGuid);
            }

            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "erv.usp_ProcessSiteAssignmentChange";
                cmd.CommandTimeout = 30 * (dtSiteAssignmentsAfter.Rows.Count + dtSiteAssignmentsBefore.Rows.Count + 1);
                cmd.Parameters.Add("@TargetSiteGroupGuid", SqlDbType.UniqueIdentifier);
                cmd.Parameters["@TargetSiteGroupGuid"].Value = targetSiteGroupGuid;
                SqlParameter sqlParamAssignmentsBefore = cmd.Parameters.AddWithValue("@SiteAssignmentsBefore", dtSiteAssignmentsBefore);
                sqlParamAssignmentsBefore.SqlDbType = SqlDbType.Structured;
                sqlParamAssignmentsBefore.TypeName = "erv.utt_SiteList";
                SqlParameter sqlParamAssignmentsAfter = cmd.Parameters.AddWithValue("@SiteAssignmentsAfter", dtSiteAssignmentsAfter);
                sqlParamAssignmentsAfter.SqlDbType = SqlDbType.Structured;
                sqlParamAssignmentsAfter.TypeName = "erv.utt_SiteList";

                ConsolidatedDA.ExecuteQuery(security, cmd);
            }
        }


        /// <summary>
        /// Get the Record Version Guid (Child Record Version or Master Record Version) that is applicable for a record at a given site.
        /// </summary>
        /// <param name="security"></param>
        /// <param name="entityTypeId">Entity Type of the record</param>
        /// <param name="recordGuid">Guid of the record (Master Record Guid or Child Record Guid)</param>
        /// <param name="targetSiteGuid">Site for which the applicable Record Version Guid is to be fetched</param>
        /// <returns></returns>
        public Guid GetRecordVersionGuid(SecurityClass security, string entityTypeId, Guid recordGuid, Guid targetSiteGuid)
        {
            Object result = null;
            if (security == null)
                throw new ArgumentNullException("Security");
            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "erv.udf_GetFirstParentRecordVersionGuid";
                cmd.Parameters.Add("@EntityTypeId", SqlDbType.NVarChar, 100);
                cmd.Parameters["@EntityTypeId"].Value = entityTypeId;
                cmd.Parameters.Add("@EntityRecGuid", SqlDbType.UniqueIdentifier);
                cmd.Parameters["@EntityRecGuid"].Value = recordGuid;
                cmd.Parameters.Add("@StartSiteIndex", SqlDbType.UniqueIdentifier);
                cmd.Parameters["@StartSiteIndex"].Value = targetSiteGuid;
                SqlParameter sqlParamResult = cmd.Parameters.Add("@Result", SqlDbType.UniqueIdentifier);
                sqlParamResult.Direction = ParameterDirection.ReturnValue;
                ConsolidatedDA.ExecuteScalar(cmd, security);
                result = DataObject.getValue<Guid>(cmd.Parameters["@Result"].Value, Guid.Empty);
            }
            if (result != null)
                return (Guid)result;
            return Guid.Empty;
        }



        public ERVProcessSettingsClass GetProcessSettings(SecurityClass security)
        {
            if (security == null)
            {
                throw new ArgumentNullException(nameof(security));
            }

            ERVProcessSettingsClass ervProcessSettings = new ERVProcessSettingsClass();

            DataSet set;
            using (var cmd = new SqlCommand())
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "erv.usp_GetProcessSettings";
                set = this.ConsolidatedDA.GetDataSet(cmd, security);
            }
            ervProcessSettings.Load(set);
            return ervProcessSettings;
        }



        public void SetGlobalFieldsProcessingInhibitFlag(SecurityClass security, bool inhibitFlag, string userId)
        {
            if (security == null)
                throw new ArgumentNullException("Security");
            
            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "erv.usp_SetProcessSettingsInhibitGlobalFieldsProcessing";
                cmd.Parameters.Add("@InhibitGlobalFieldsProcessing", SqlDbType.Bit);
                cmd.Parameters["@InhibitGlobalFieldsProcessing"].Value = inhibitFlag;
                cmd.Parameters.Add("@UserId", SqlDbType.NVarChar, 100);
                cmd.Parameters["@UserId"].Value = userId;

                ConsolidatedDA.ExecuteQuery(security, cmd);
            }
        }


    }

}
