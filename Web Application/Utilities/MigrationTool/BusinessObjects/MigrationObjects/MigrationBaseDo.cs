namespace BusinessObjects.MigrationObjects
{
    using System.Data;
    using System.Data.SqlClient;

    public class MigrationBaseDo
    {
        #region Constructors
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        public MigrationBaseDo()
        {
        }
        #endregion

        /// <summary>
        /// This method will populate the the SQL to get the site index. 
        /// </summary>
        /// <param name="command">The SQL Command</param>
        /// <param name="siteId">The site ID to get the index.</param>
        public virtual void GetSiteIndexByIdSql(SqlCommand command, string siteId)
        {
            command.CommandText = "SELECT SiteIndex FROM tblSites WHERE ID = '" + siteId + "' ";
        }

        /// <summary>
        /// This method will load the dataset and return the Site Index.  Returns
        /// -99 if not found.
        /// </summary>
        /// <param name="dataSet">The dataset containing the site index.</param>
        /// <returns>Returns the site index or null if not found.</returns>
        public int? GetSiteIndex(DataSet dataSet)
        {
            if (dataSet == null || dataSet.Tables.Count == 0 || dataSet.Tables[0].Rows.Count == 0)
            {
                return null;
            }

            DataTable table = dataSet.Tables[0];
            DataRow row = table.Rows[0];

            int? siteIndex = row.IsNull("SiteIndex") ? null : (int?)row["SiteIndex"];
            return siteIndex;
        }
    }
}
