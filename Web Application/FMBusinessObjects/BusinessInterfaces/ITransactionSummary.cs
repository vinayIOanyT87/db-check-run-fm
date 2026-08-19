namespace FMBusinessObjects.BusinessInterfaces
{
	using System;
	using System.Collections.Generic;
	using System.ServiceModel;

	using FMBusinessObjects.DataObjects;

	[ServiceContract]
	public interface ITransactionSummary
	{
		/// <summary>
		/// Select the transaction summary matching the search criteria from the DB
		/// </summary>
		/// <param name="security">Contains security information.</param>
		/// <param name="beginDate">The beginning inventory date specified by the user</param>
		/// <param name="endDate">The ending inventory date specified by the user</param>
		/// <param name="aliasName">The alias name to match on specified by the user</param>
		/// <param name="searchString">Anything provided in the Find String/Search text box to match results on</param>
		/// <param name="pageStart">The first record to retrieve in the result set</param>
		/// <param name="pageLength">The number of records to retrieve from the result set</param>
		/// <param name="sortedColumns">The columns the user has sorted the results on</param>
		/// <param name="recordCount">The total number of records macthed by the sql - not the length of the paged result set.
		/// This is used to display the "Showing records x of y" information on the page.</param>
		[OperationContract]
		List<TransactionSummaryClass> Enumerate(
			SecurityClass security,
			DateTimeOffset beginDate,
			DateTimeOffset endDate,
			string aliasName,
			string searchString,
			int pageStart,
			int pageLength,
			List<DataTablesColumn> sortedColumns,
			out int recordCount);

		/// <summary>
		/// Get the configured transaction list view columns for the specified transaction alias
		/// </summary>
		/// <param name="security">Contains security information.</param>
		/// <param name="aliasName">The alias name to match on specified by the user</param>
		/// <returns>A dictionary collection of list view columns with key = DbName and value = DisplayName</returns>
		[OperationContract]
		Dictionary<string, string> GetListViewAssignedColumns(SecurityClass security, string aliasName);
	}
}
