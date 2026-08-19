namespace FMBusinessServices.DataAccessLayer
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using System.Linq;

    using FMBusinessObjects.DataObjects;

	internal static class TransactionSummaryDAO
	{
        /// <summary>
        /// A list of columns that the user can potentially order by.
        /// Because the order by clause is dynamically built,
        /// it is possible that a malicious user could perform a SQL Injection attack.
        /// This serves as a whitelist of column names we expect to see in the order by.
        /// </summary>
	    private static readonly List<string> SupportedOrderByColumns = new List<string>
        {
	        "InventoryDate",
            "TransDateTime",
            "DocumentNumber",
            "AliasName",
            "TransactionStatus",
            "OwnerID",
            "ManagerID",
            "ShipToID",
            "Product",
            "GrossQuantity",
            "NetQuantity"
	    };

        /// <summary>
        /// Build the SQL to select the transaction records matching the search criteria from the DB
        /// </summary>
        /// <param name="summary">This is an extension method for the TransactionSummary class.</param>
        /// <param name="security">Contains security information.</param>
        /// <param name="cmd">The SQL command to add SQL to</param>
        /// <param name="beginDate">The beginning inventory date specified by the user</param>
        /// <param name="endDate">The ending inventory date specified by the user</param>
        /// <param name="aliasName">The alias name to match on specified by the user</param>
        /// <param name="findText">Anything provided in the Find String/Search text box to match results on</param>
        /// <param name="pageStart">The first record to retrieve in the result set</param>
        /// <param name="pageLength">The number of records to retrieve from the result set</param>
        /// <param name="sortedColumns">The columns the user has sorted the results on</param>
        internal static void EnumerateSql(
            this TransactionSummaryClass summary,
            SecurityClass security,
            SqlCommand cmd,
            DateTimeOffset beginDate,
            DateTimeOffset endDate,
            string aliasName,
            string findText,
            int pageStart,
            int pageLength,
            List<DataTablesColumn> sortedColumns)
        {
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "usp_TransactionSummary";

            cmd.Parameters.Add("@LoginSiteGuid", SqlDbType.UniqueIdentifier).Value = security.LoginSiteGuid;
            cmd.Parameters.Add("@SiteGuid", SqlDbType.UniqueIdentifier).Value = security.SiteGuid;
            cmd.Parameters.Add("@UserGuid", SqlDbType.UniqueIdentifier).Value = security.UserGuid;
            cmd.Parameters.Add("@BeginDate", SqlDbType.Date).Value = beginDate.Date;        
            cmd.Parameters.Add("@EndDate", SqlDbType.Date).Value = endDate.Date;

            if (!string.IsNullOrEmpty(aliasName))
            {             
                cmd.Parameters.Add("@AliasName", SqlDbType.NVarChar, 32).Value = aliasName;
            }

            if (!string.IsNullOrEmpty(findText))
            {              
                // The FindText is going to be searched using LIKE in the stored procedure, so add percent signs to it
                cmd.Parameters.Add("@FindText", SqlDbType.NVarChar, 22).Value = "%" + findText + "%";
            }

            cmd.Parameters.Add("@PageStart", SqlDbType.Int).Value = pageStart;
            cmd.Parameters.Add("@PageLength", SqlDbType.Int).Value = pageLength;            

            // Order the results - either by the default (InventoryDate) or the sort order specified by the user
            cmd.Parameters.Add("@OrderBy", SqlDbType.NVarChar, 1000).Value = BuildOrderBy(sortedColumns);
        }

        /// <summary>
        /// Create an ORDER BY clause to use. If the user has not specified a sort order, order by InventoryDate.
        /// Otherwise, order by the columns specified by the user
        /// </summary>
        /// <param name="sortedColumns">Columns to sort on specified by the user, in the order to sort on</param>
        /// <returns>An ORDER BY clause based on the columns the user sorted by</returns>
	    private static string BuildOrderBy(IReadOnlyCollection<DataTablesColumn> sortedColumns)
	    {
            // If no sort order was specified, order by inventory date. We have to order by something to page the results.
	        if (sortedColumns.Count == 0)
	        {
                return "InventoryDate ";
	        }

            string orderByColumns = string.Join(",", sortedColumns.Select(sortedColumn => 
                (string.IsNullOrEmpty(sortedColumn.Name) ? CheckOrderByWhitelist(sortedColumn.Data) : CheckOrderByWhitelist(sortedColumn.Name)) 
                + " " + (sortedColumn.SortDirection == DataTablesColumn.OrderDirection.Descendant ? "DESC " : string.Empty)));

            return orderByColumns;
	    }

        /// <summary>
        /// Check the whitelist of supported columns to make sure some nefarious user isn't trying to inject SQL.
        /// </summary>
        /// <param name="orderByColumnName">The order by column to check</param>
        /// <returns>If the value is in the whitelist, return the value. Otherwise, throw an exception</returns>
	    private static string CheckOrderByWhitelist(string orderByColumnName)
	    {
	        if (SupportedOrderByColumns.Find(
	                orderByColumn => orderByColumn.Equals(orderByColumnName, StringComparison.OrdinalIgnoreCase)) != null)
	        {
	            return orderByColumnName;
	        }

	        throw new Exception(orderByColumnName + " is not a supported field");
	    }
	}
}