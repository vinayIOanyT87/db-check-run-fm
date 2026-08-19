namespace MirgrationToolProcessing.Mapping
{
    using BusinessObjects.MigrationObjects;
    using MigrationToolDataAccessLayer;
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;

    public class SourceDbEquipTypesMapping753ToV12 : SourceDbEquipTypesMappingBase
    {
        #region Constructors
        /// <summary>
        /// This is the default constructor
        /// </summary>
        public SourceDbEquipTypesMapping753ToV12()
        {
            base.Init();
        }
        #endregion

        #region Properties
        #endregion

        #region Public methods
        /// <summary>
        /// This method retrieves the equipment types from the source database.
        /// </summary>
        /// <param name="migrationDa">The data access object.</param>
        public override void GetSourceEquipmentTypeMaps(MigrationDatabaseDAClass migrationDa, string sourceDbName)
        {
            base.EquipmentTypesBaseList = new List<EquipmentTypeBaseDo>();

            using (var command = new SqlCommand())
            {
                var equipmentTypes = new EquipmentType753ToV12Do(sourceDbName, null);
                equipmentTypes.EnumerateEquipmentTypesSql(command);
                DataSet dataSet = null;

                try
                {
                    dataSet = migrationDa.GetDataSet(command);
                }
                catch (Exception ex)
                {
                    base.MessageFlag = true;
                    base.Message = base.Message + Environment.NewLine + "Error: Retrieving Equipment Types from source database. " + ex.Message;
                }

                if (dataSet == null || dataSet.Tables.Count == 0)
                {
                    base.MessageFlag = true;
                    base.Message = base.Message + Environment.NewLine + "Info: No Equipment Types in source database.";
                    return;
                }

                DataTable table = dataSet.Tables[0];

                foreach (DataRow row in table.Rows)
                {
                    equipmentTypes = new EquipmentType753ToV12Do();
                    equipmentTypes.Load(row);

                    base.EquipmentTypesBaseList.Add(equipmentTypes);
                }
            }
        }
        #endregion
    }
}
