namespace MirgrationToolProcessing.Mapping
{
    using BusinessObjects.MigrationObjects;
    using MigrationToolDataAccessLayer;
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;

    public class SourceDbQualificationMapping753ToV12 : SourceDbQualificationMappingBase
    {
        #region Constructors
        /// <summary>
        /// This is the default constructor
        /// </summary>
        public SourceDbQualificationMapping753ToV12()
        {
            base.Init();
        }
        #endregion

        #region Properties
        #endregion

        #region Public methods
        /// <summary>
        /// This method retrieves the Qualification maps from the source database.
        /// </summary>
        /// <param name="migrationDa">The data access object.</param>
        public override void GetSourceQualificationMaps(MigrationDatabaseDAClass migrationDa)
        {
            base.QualificationMapsBaseList = new List<QualificationMapsBaseDo>();

            using(var command = new SqlCommand())
            {
                var qualificationMaps = new QualificationMaps753ToV12Do();
                qualificationMaps.EnumerateQuantityMapsSql(command);
                DataSet dataSet = null;

                try
                {
                    dataSet = migrationDa.GetDataSet(command);
                }
                catch(Exception ex)
                {
                    base.MessageFlag = true;
                    base.Message = base.Message + Environment.NewLine + "Error: Retrieving Qualification Maps from source database. " + ex.Message;
                }

                if(dataSet == null || dataSet.Tables.Count == 0)
                {
                    base.MessageFlag = true;
                    base.Message = base.Message + Environment.NewLine + "Info: No Qualification Maps in source database.";
                    return;
                }

                DataTable table = dataSet.Tables[0];

                foreach(DataRow row in table.Rows)
                {
                    qualificationMaps = new QualificationMaps753ToV12Do();
                    qualificationMaps.Load(row);

                    base.QualificationMapsBaseList.Add((QualificationMapsBaseDo)qualificationMaps);
                }
            }
        }
        #endregion

        #region Private methods
        #endregion
    }
}
