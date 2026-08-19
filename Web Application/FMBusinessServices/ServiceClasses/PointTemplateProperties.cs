
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
   using FMBusinessObjects.UtilityObjects;

   using FMCore;

   using InternalClasses;

   [SecuritySafeCritical]
   [ServiceBehavior(TransactionIsolationLevel = System.Transactions.IsolationLevel.ReadCommitted)]
   public class PointTemplateProperties : IPointTemplateProperties
   {
      public ConsolidatedDAClass ConsolidatedDa = new ConsolidatedDAClass();

      [OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
      public void AddProperties(SecurityClass security, List<PointTemplateProperty> pointTemplatePropertyList)
      {
         if (security == null)
         {
            throw new ArgumentNullException("security");
         }

         // TODO: Check security rights.
         using (var cmd = new SqlCommand())
         {
            foreach (var pointTemplateProperty in pointTemplatePropertyList)
            {
               pointTemplateProperty.SetCreationStamp(security);
               pointTemplateProperty.AutoGenerateInsertProcSQL(cmd, "usp_PointTemplatePropertyInsertByPK");
               cmd.Parameters["@PointTemplatePropertyGuid"].Direction = ParameterDirection.InputOutput;
               ConsolidatedDa.ExecuteQuery(security, cmd);
               pointTemplateProperty.IdentityGuid = new Guid(cmd.Parameters["@PointTemplatePropertyGuid"].Value.ToString());
            }
         }
      }

      public PointTemplateProperty Get(SecurityClass security, Guid pointTemplateGuid)
      {
         if (security == null)
         {
            throw new ArgumentNullException("security");
         }

         // TODO: Check security rights.

         var propertyTemplateProperty = new PointTemplateProperty();
         DataSet set = null;

         using (var cmd = new SqlCommand())
         {
            propertyTemplateProperty.GetSQL(cmd, pointTemplateGuid);
            set = ConsolidatedDa.GetDataSet(cmd, security);
         }

         DataTable table = set.Tables[0];
         if (table.Rows.Count > 0)
         {
            propertyTemplateProperty.AutoLoad(table.Rows[0]);
         }

         return propertyTemplateProperty;
      }

      public Dictionary<Guid, PointTemplateProperty> EnumerateByPointTemplateGuid(SecurityClass security, Guid pointTemplateGuid)
      {
         if (security == null)
         {
            throw new ArgumentNullException("security");
         }

         // TODO: Check security rights.

         var property = new PointTemplateProperty();
         DataSet set = null;

         using (var cmd = new SqlCommand())
         {
            property.EnumerateByPointTemplateSQL(cmd, pointTemplateGuid);
            set = ConsolidatedDa.GetDataSet(cmd, security);
         }
         return PopulateDictionary(set);
      }

      [OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
      public void ModifyPointTemplatePropertyValue(SecurityClass security, PointTemplateProperty pointTemplateProperty)
      {
         security.ThrowIfNull("security");
         // TODO: Check security rights.

         using (var cmd = new SqlCommand())
         {
            cmd.CommandText = "dbo.usp_PointTemplatePropertyDataUpdate";
            cmd.CommandType = CommandType.StoredProcedure;
            if (string.IsNullOrEmpty(pointTemplateProperty.ValueXml))
            {
               cmd.Parameters.AddWithValue("@Value", DBNull.Value);
            }
            else
            {
               cmd.Parameters.AddWithValue("@Value", pointTemplateProperty.ValueXml);
            }
            cmd.Parameters.AddWithValue("@PointTemplatePropertyGuid", pointTemplateProperty.PointTemplatePropertyGuid);
            cmd.Parameters.AddWithValue("@UpdatedBy", security.UserID);
            cmd.Parameters.AddWithValue("@UpdatedDate", DateTimeOffset.Now);

            ConsolidatedDa.ExecuteQuery(security, cmd);
         }
      }


      [OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
      public void UpdatePointTemplateProperties(SecurityClass security, Guid pointTemplateGuid, Dictionary<Guid, PointTemplateProperty> propertyList)
      {
         if (security == null)
         {
            throw new ArgumentNullException("security");
         }

         if (pointTemplateGuid == null)
         {
            throw new ArgumentNullException("pointTemplateGuid");
         }

         if (propertyList == null)
         {
            throw new ArgumentNullException("propertyList");
         }

         var existingPropertiesByGuid = this.EnumerateByPointTemplateGuid(security, pointTemplateGuid);

         var consolidatedDa = new ConsolidatedDAClass();

         var processedPropertyList = new List<Guid>();
         using (var cmd = new SqlCommand())
         {
            foreach (var property in propertyList.Values)
            {
               processedPropertyList.Add(property.PointTemplatePropertyGuid);

               PointTemplateProperty existingProperty;
               if (existingPropertiesByGuid.TryGetValue(property.PointTemplatePropertyGuid, out existingProperty)
                  && this.AreEquivalentForUpsert(property, existingProperty))
               {
                  continue;
               }

               property.SetModifyStamp(security);
               property.AutoGenerateModifyProcSQL(cmd, "usp_PointTemplatePropertyUpdateByPK");

               if (property.Value == null && cmd.Parameters.Contains("@Value"))
               {
                  cmd.Parameters["@Value"].Value = DBNull.Value;
                  cmd.Parameters.AddWithValue("@NullOverrideValue", 1);
               }

               consolidatedDa.ExecuteQuery(security, cmd);
            }
         }

         this.PurgeByPointTemplateGuidAndNotInList(security, pointTemplateGuid, processedPropertyList);
      }



      //Dictionary<PointTemplatePropertyGuid, PointTemplateProperty>
      protected Dictionary<Guid, PointTemplateProperty> PopulateDictionary(DataSet set)
      {
         var pointTemplatePropertyDictionary = new Dictionary<Guid, PointTemplateProperty>();

         DataTable table = set.Tables[0];

         foreach (DataRow row in table.Rows)
         {
            var property = new PointTemplateProperty();

            property.AutoLoad(row);
            pointTemplatePropertyDictionary.Add(property.PointTemplatePropertyGuid, property);
         }

         return pointTemplatePropertyDictionary;
      }

      private bool AreEquivalentForUpsert(PointTemplateProperty currentProperty, PointTemplateProperty existingProperty)
      {
         return currentProperty.PointTemplatePropertyGuid == existingProperty.PointTemplatePropertyGuid
            && currentProperty.PointTemplateGuid == existingProperty.PointTemplateGuid
            && currentProperty.SiteGuid == existingProperty.SiteGuid
            && string.Equals(currentProperty.ID, existingProperty.ID, StringComparison.Ordinal)
            && string.Equals(this.GetValueTypeStringSafe(currentProperty), this.GetValueTypeStringSafe(existingProperty), StringComparison.Ordinal)
            && string.Equals(currentProperty.ValueXml, existingProperty.ValueXml, StringComparison.Ordinal);
      }

      private string GetValueTypeStringSafe(PointTemplateProperty property)
      {
         return (property.ValueType == null) ? string.Empty : property.ValueType.ToString();
      }

      public void Purge(SecurityClass security, Guid pointTemplatePropertyGuid)
      {
         if (security == null)
         {
            throw new ArgumentNullException("security");
         }

         var pointTemplateProperty = this.Get(security, pointTemplatePropertyGuid);
         if (pointTemplateProperty.IdentityGuid == Guid.Empty)
         {
            throw new Exception("Point Template Property not found.");
         }


         var dependencies = new DependenciesClass(security);
         dependencies.Purge(security, pointTemplateProperty);

         using (var cmd = new SqlCommand())
         {
            cmd.CommandText = "dbo.gsp_PointTemplatePropertyDeleteByRowGuid";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@PointTemplatePropertyGuid", pointTemplatePropertyGuid);
            ConsolidatedDa.ExecuteQuery(security, cmd);
         }
      }

      public void PurgeByPointTemplateGuidAndNotInList(SecurityClass security, Guid pointTemplateGuid, List<Guid> propertyList)
      {
         if (security == null)
         {
            throw new ArgumentNullException("security");
         }

         if (!this.HasStalePointTemplateProperties(security, pointTemplateGuid, propertyList))
         {
            return;
         }

         var pointAccessGroupToExposedSettingMaps = new PointAccessGroupToExposedSettingMaps();
         pointAccessGroupToExposedSettingMaps.PurgeByPointTemplateGuidAndNotInList(security, pointTemplateGuid, propertyList);

         using (var cmd = new SqlCommand())
         {
            cmd.CommandText = "SET NOCOUNT ON"
                              + " DELETE FROM tblPointProperty WHERE PointGuid IN (SELECT PointGuid FROM tblPoint WHERE PointTemplateGuid = @PointTemplateGuid) AND PointTemplatePropertyGuid NOT IN (SELECT * FROM @PointTemplatePropertyGuidList)"
                              + " DELETE FROM tblPointTemplateProperty WHERE PointTemplateGuid = @PointTemplateGuid AND PointTemplatePropertyGuid NOT IN(SELECT * FROM @PointTemplatePropertyGuidList)";
            cmd.CommandType = CommandType.Text;

            using (var parameterTempTable = new DataTable())
            {
               parameterTempTable.Columns.Add("PointTemplatePropertyGuid", typeof(Guid));

               foreach (var pointTemplatePropertyGuid in propertyList)
               {
                  parameterTempTable.Rows.Add(pointTemplatePropertyGuid);
               }

               var pList = new SqlParameter("@PointTemplatePropertyGuidList", SqlDbType.Structured);
               pList.TypeName = "dbo.GuidListType";
               pList.Value = parameterTempTable;

               cmd.Parameters.AddWithValue("@PointTemplateGuid", pointTemplateGuid);
               cmd.Parameters.Add(pList);
               ConsolidatedDa.ExecuteQuery(security, cmd);
            }
         }
      }

      private bool HasStalePointTemplateProperties(SecurityClass security, Guid pointTemplateGuid, List<Guid> propertyList)
      {
         using (var cmd = new SqlCommand())
         {
            cmd.CommandText = "SELECT TOP (1) 1 FROM tblPointTemplateProperty WHERE PointTemplateGuid = @PointTemplateGuid AND PointTemplatePropertyGuid NOT IN (SELECT * FROM @PointTemplatePropertyGuidList)";
            cmd.CommandType = CommandType.Text;

            using (var parameterTempTable = new DataTable())
            {
               parameterTempTable.Columns.Add("PointTemplatePropertyGuid", typeof(Guid));

               foreach (var pointTemplatePropertyGuid in propertyList)
               {
                  parameterTempTable.Rows.Add(pointTemplatePropertyGuid);
               }

               var pList = new SqlParameter("@PointTemplatePropertyGuidList", SqlDbType.Structured);
               pList.TypeName = "dbo.GuidListType";
               pList.Value = parameterTempTable;

               cmd.Parameters.AddWithValue("@PointTemplateGuid", pointTemplateGuid);
               cmd.Parameters.Add(pList);

               var resultSet = ConsolidatedDa.GetDataSet(cmd, security);
               return resultSet.Tables.Count > 0 && resultSet.Tables[0].Rows.Count > 0;
            }
         }
      }
   }
}