// --------------------------------------------------------------------------------------------------------------------
// <copyright file="QueriesClass.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the QueriesClass type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FMBusinessServices.ServiceClasses
{
	using System;
	using System.Collections.Generic;
	using System.Data;
	using System.Data.SqlClient;
	using System.Globalization;
	using System.Linq;
	using System.Reflection;
	using System.Security;
	using System.ServiceModel;
	using System.Web.UI.WebControls;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.Exceptions;
	using FMBusinessObjects.LogClient;
	using FMBusinessObjects.UtilityObjects;

	using FMBusinessServices.DataAccessLayer;
	using FMBusinessServices.InternalClasses;

    using FMCore;

    /// <summary>
	/// The QueriesClass service class allows get and set of queries.
	/// </summary>
	[SecuritySafeCritical]
	[ServiceBehavior(TransactionIsolationLevel = System.Transactions.IsolationLevel.ReadCommitted)]
	public class QueriesClass : IQueries
	{
		#region Constants and Fields

		/// <summary>
		/// Provides access to the database.
		/// </summary>
		private readonly ConsolidatedDAClass consolidatedDA = new ConsolidatedDAClass();

        private const int DefaultCommandTimeout = 30;
        #endregion

		#region Public Methods and Operators

		/// <summary>
		/// Validates the query.
		/// </summary>
		/// <param name="security">The FuelsManager security object.</param>
		/// <param name="query">The query to validate.</param>
		private static void ValidateQuery( SecurityClass security, QueryClass query )
		{
			if (security == null)
			{
				throw new ArgumentNullException( "security" );
			}

			if (!security.HasRight( RIGHT.VIEW_QUERIES ) && !security.HasRight( RIGHT.MODIFY_QUERIES ))
			{
				throw new FMInsufficientRightsException();
			}

			if (query == null)
			{
				throw new ApplicationException("Query object cannot be null.");
			}

			if (query.Fields == null || query.Fields.Count == 0)
			{
				throw new ApplicationException( "No fields configured for query results." );
			}
		}

		/// <summary>
		/// Applies translations, if any, to the fields in the provided collection.
		/// </summary>
		/// <param name="security">
		/// The FuelsManager fuelsManagerSecurityObject object.
		/// </param>
		/// <param name="fieldCollection">
		/// A collection of Query Writer Fields.
		/// </param>
		public static void ApplyDataDictionary(SecurityClass security, QueryWriterFieldCollection fieldCollection)
		{
			var dictionaries = new DataDictionariesClass();

			foreach (var field in fieldCollection)
			{
				string translation = dictionaries.Get(security.SiteGuid, field.DisplayName);
				if (string.IsNullOrEmpty(translation) == false)
				{
					field.DisplayName = translation;
				}
			}
		}

		/// <summary>
		/// Adds a query object to the database.
		/// </summary>
		/// <param name="security">
		/// The FuelsManager fuelsManagerSecurityObject object.
		/// </param>
		/// <param name="query">
		/// The query to add.
		/// </param>
		/// <returns>
		///	The identity guid for the newly added query.
		/// </returns>
		/// <exception cref="ArgumentNullException">
		/// The fuelsManagerSecurityObject object cannot be null.
		/// </exception>
		/// <exception cref="ArgumentNullException">
		/// The query object cannot be null.
		/// </exception>
		/// <exception cref="Exception">
		/// The user must have certain rights.
		/// </exception>
		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public Guid Add(SecurityClass security, QueryClass query)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (query == null)
			{
				throw new ArgumentNullException("query");
			}

			if (!security.HasRight(RIGHT.MODIFY_QUERIES))
			{
				throw new FMInsufficientRightsException();
			}

			Validate(query);

			query.SiteGuid = security.SiteGuid;
			query.CreatedDate = DateTimeOffset.Now;
			query.CreatedBy = security.UserID;
			query.UpdatedDate = query.CreatedDate;
			query.UpdatedBy = security.UserID;
			query.OwnerUserGuid = security.UserGuid;

			using (var cmd = new SqlCommand())
			{
				query.IdentityGuid = Guid.NewGuid();
				query.InsertSQL(cmd);
				this.consolidatedDA.ExecuteQuery(security, cmd);
			}

			var groups = new GroupsClass();

			// Query Group Maps
			var queryGroupMaps = new QueryGroupMapsClass();
			foreach (GroupClass group in query.AssignedGroups)
			{
				// If the group identity guid is invalid, try to look it up
				// Some groups might make it here without a valid identity guid if the query was imported
				// or if the group was deleted before the query could be saved
				if (group.IdentityGuid.IsEmpty())
				{
					group.IdentityGuid = groups.GetIdentityGuid(security, group.ID);
				}

				// Only add the group if the identity guid is valid
				if (group.IdentityGuid.IsEmpty() == false)
				{
					var map = new QueryGroupMapClass
						{
							QueryStorageGuid = query.IdentityGuid, 
							GroupGuid = @group.IdentityGuid
						};

					queryGroupMaps.Add(security, map);
				}
			}

			return query.IdentityGuid;
		}


		public QueryCollectionClass EnumerateQueryNodes(SecurityClass security)
		{
			QueryCollectionClass queries = this.Enumerate(security, true);
			var queryNodes = new QueryCollectionClass();
			foreach (QueryClass query in queries)
			{
				if (string.IsNullOrEmpty(query.NavNodePath) == false)
				{
					queryNodes.Add(query);
				}
			}

			return queryNodes;
		}


		/// <summary>
		/// Enumerates the queries in the system according to the provided fuelsManagerSecurityObject context.
		/// </summary>
		/// <param name="security">
		/// The FuelsManager fuelsManagerSecurityObject object.
		/// </param>
		/// <param name="isQuickLoad">
		/// Specifies whether a quick load of the Query objects in the returned list is desired.  
		/// Quick load skips loading much of the detailed configuration information.
		/// </param>
		/// <returns>
		/// A collection of query objects.
		/// </returns>
		/// <exception cref="ArgumentNullException">
		/// THe fuelsManagerSecurityObject object cannot be null.
		/// </exception>
		/// <exception cref="ApplicationException">
		/// Access to this method is governed by certain fuelsManagerSecurityObject rights.
		/// </exception>
		public QueryCollectionClass Enumerate(SecurityClass security, bool isQuickLoad)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (!security.HasRight(RIGHT.VIEW_QUERIES) && !security.HasRight(RIGHT.MODIFY_QUERIES))
			{
				throw new FMInsufficientRightsException();
			}

			var queryCollection = new QueryCollectionClass();

			var query = new QueryClass { SiteGuid = security.SiteGuid };

			DataSet set;
			using (var cmd = new SqlCommand())
			{
				query.EnumerateSQL(cmd, security);
				set = this.consolidatedDA.GetDataSet(cmd, security);
			}

			DataTable table = set.Tables[0];

			string queryAssemblies = null;

			if (!isQuickLoad)
			{
				queryAssemblies = GetQueryAssemblyList(security);
			}

			while (table.Rows.Count != 0)
			{
				query = new QueryClass();

				query.Load(security, set, isQuickLoad, queryAssemblies);
				queryCollection.AddDistinct(security, query);

				table.Rows.RemoveAt(0);
			}

			return queryCollection;
		}

		public QueryCollectionClass EnumerateForUserPurge(SecurityClass security, Guid userGuid)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (!security.HasRight(RIGHT.MODIFY_USERS))
			{
				throw new FMInsufficientRightsException();
			}

			var queryCollection = new QueryCollectionClass();

			var query = new QueryClass { SiteGuid = security.SiteGuid, OwnerUserGuid = userGuid};

			DataSet set;
			using (var cmd = new SqlCommand())
			{
				query.EnumerateForUserPurgeSQL(cmd);
				set = this.consolidatedDA.GetDataSet(cmd, security);
			}

			DataTable table = set.Tables[0];

			while (table.Rows.Count != 0)
			{
				query = new QueryClass();
				query.Load(security, set, true, null);
				queryCollection.AddDistinct(security, query);

				table.Rows.RemoveAt(0);
			}

			return queryCollection;
		}

		/// <summary>
		/// Gets the query object specified by the queryGuid or null if not found.
		/// </summary>
		/// <param name="security">The FuelsManager fuelsManagerSecurityObject object.</param>
		/// <param name="queryGuid">The query GUID to find.</param>
		/// <returns>The query object specified by the queryGuid identifier or null if not found.</returns>
		public QueryClass Get(SecurityClass security, Guid queryGuid)
		{
			return this.GetByQuickLoad(security, queryGuid, false);
		}

		/// <summary>
		/// Gets the name of the by query.
		/// </summary>
		/// <param name="security">The FuelsManager security object.</param>
		/// <param name="queryName">Name of the query.</param>
		/// <returns>The query specified by the name or null if not found.</returns>
		public QueryClass GetByQueryName(SecurityClass security, string queryName)
		{
			var query = new QueryClass();

			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (!security.HasRight(RIGHT.VIEW_QUERIES) && !security.HasRight(RIGHT.MODIFY_QUERIES))
			{
				throw new FMInsufficientRightsException();
			}

			query.QueryName = queryName;
			query.SiteGuid = security.SiteGuid;

			using (var cmd = new SqlCommand())
			{
				query.SelectByNameSQL(cmd, security, ContextUtil.IsInTransaction);
				query.Load(security, this.consolidatedDA.GetDataSet(cmd, security), false,  GetQueryAssemblyList(security));
			}

			return query;
		}

		public QueryClass GetByNodePath(SecurityClass security, string queryNodePath)
		{
			var query = new QueryClass();

			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (!security.HasRight(RIGHT.VIEW_QUERIES) && !security.HasRight(RIGHT.MODIFY_QUERIES))
			{
				throw new FMInsufficientRightsException();
			}

			query.NavNodePath = queryNodePath;
			query.SiteGuid = security.SiteGuid;

			using (var cmd = new SqlCommand())
			{
				query.SelectByNodePathSQL(cmd, security, ContextUtil.IsInTransaction);
				query.Load(security, this.consolidatedDA.GetDataSet(cmd, security), false, GetQueryAssemblyList(security));
			}

			return query;
		}

		/// <summary>
		/// Gets a query with optional quick load.
		/// </summary>
		/// <param name="security">The FuelsManager security object.</param>
		/// <param name="identityGuid">The identity GUID of the query to get.</param>
		/// <param name="isQuickLoad">if set to <c>true</c> only basic information is retrieved for the query (no xml parsing done).</param>
		/// <returns>A query object if it is found, null otherwise.</returns>
		public QueryClass GetByQuickLoad(SecurityClass security, Guid identityGuid, bool isQuickLoad)
		{
			var query = new QueryClass();

			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (!security.HasRight(RIGHT.VIEW_QUERIES) && !security.HasRight(RIGHT.MODIFY_QUERIES))
			{
				throw new FMInsufficientRightsException();
			}

			query.IdentityGuid = identityGuid;
			query.SiteGuid = security.SiteGuid;

			string queryAssemblies = null;

			if (!isQuickLoad)
			{
				queryAssemblies = GetQueryAssemblyList(security);
			}

			using (var cmd = new SqlCommand())
			{
				query.SelectSQL(cmd, ContextUtil.IsInTransaction);
				query.Load(security, this.consolidatedDA.GetDataSet(cmd, security), isQuickLoad, queryAssemblies);
			}

			return query;
		}

		/// <summary>
		/// Gets the query results.  This is the main query results generation.
		/// </summary>
		/// <param name="security">The FuelsManager security object.</param>
		/// <param name="query">The query to run.</param>
		/// <param name="pageFilters">The page filters to use.</param>
		/// <returns>A dataset containing the results of the query.</returns>
		public DataSet GetQueryResults(SecurityClass security, QueryClass query, QueryCriteriaPhraseCollection pageFilters)
		{
			ValidateQuery(security, query);

			// Get an object of the type in the query
			MethodInfo sqlMethod = query.Topic.ObjectType.GetMethod("QueryWriterSQL");

			if (sqlMethod == null)
			{
				throw new ApplicationException("Query method not found.");
			}

			// Get an object of the right type
			object mainObject = Activator.CreateInstance(query.Topic.ObjectType);

			if (mainObject == null)
			{
				throw new ApplicationException("Unable to create data object type");
			}

			string databaseName = query.QueryOnArchiveData ? this.consolidatedDA.ArchiveDatabaseName : this.consolidatedDA.DatabaseName;

		    using (var cmd = new SqlCommand())
		    {
		        cmd.CommandTimeout = this.QueryWriterCommandTimeout;
			    try
				{
					object[] parameters = { cmd, security, query.SelectStatement(security), databaseName };
					sqlMethod.Invoke(mainObject, parameters);
				}
				catch (TargetParameterCountException)
				{
					object[] parameters = { cmd, security, query.SelectStatement(security) };
					sqlMethod.Invoke(mainObject, parameters);
				}

				// Now build the filter criteria
				int filterCritFieldIndex = 0;

				//this is for transaction querys to filter by type
				if (query.TransactionAliasGuids.Count > 0 && (false == query.TransactionAliasGuids.Contains(new QueryWriterAliasGuid(Guid.Empty))))
				{
					var aliasCriteriaCol = new QueryCriteriaPhraseCollection();
					aliasCriteriaCol.Add(new QueryCriteriaPhrase(){Type = QueryCriteriaType.StartGroup});
					
					foreach (QueryWriterAliasGuid aliasGuid in query.TransactionAliasGuids)
					{
						var phrase = new QueryCriteriaPhrase();
						const string DbFieldName = "tblTransactions.TransactionAliasGuid";
						const string DisplayName = "TransactionAliasGuid";
						const bool GenerateSelect = true;                        

						phrase.Field = new QueryWriterField(DisplayName, DbFieldName, GenerateSelect) { Topic = query.Topic };
						phrase.Operator = QueryOperator.Equals;
						phrase.Value = aliasGuid.ToString();
						phrase.Field.FieldType = Type.GetType("System.Guid");
						phrase.Conjunction = QueryAndOr.OR;

						aliasCriteriaCol.Add(phrase);
					}

					aliasCriteriaCol.Add(new QueryCriteriaPhrase() { Type = QueryCriteriaType.EndGroup });
					
					this.BuildFilterCriteria(cmd, ref filterCritFieldIndex, query.Topic, aliasCriteriaCol, security);
				}

				// Now build the Phrase filter criteria
				this.BuildFilterCriteria(cmd, ref filterCritFieldIndex, query.Topic, query.Criterion, security);
				this.BuildFilterCriteria(cmd, ref filterCritFieldIndex, query.Topic, pageFilters, security);

				cmd.CommandText += query.DataGroupStatement;

				// Execute the query
				DataSet set = this.consolidatedDA.GetDataSet(cmd, security);

				this.ProcessEnumerations(query, set);

				// Convert the date and time to the site's local date and time.
				this.ProcessDateTimes(query, set, security, mainObject);

				// Call the pre process if there is one defined
				MethodInfo preProcess = query.Topic.ObjectType.GetMethod("QueryWriterPreProcess");

				if (preProcess != null)
				{
					object[] parameters2 = { security, set };
					preProcess.Invoke(mainObject, parameters2);
				}

				if (query.TotalAllFields)
				{

					// Call the totals filter if there is one defined
					PropertyInfo totalsFilter = query.Topic.ObjectType.GetProperty("QueryWriterTotalsFilter");
					string totalFilter = string.Empty;

					if (totalsFilter != null)
					{
						object[] parameters2 = { security, set };
						totalFilter = (string)totalsFilter.GetValue(mainObject, null);
					}

					this.ProduceTotals(query, set, totalFilter);
				}

				// Call the post process if there is one defined
                MethodInfo postProcess = query.Topic.ObjectType.GetMethod("QueryWriterPostProcess");

                if (postProcess != null)
                {
                    object[] parameters2 = { security, set };
                    postProcess.Invoke(mainObject, parameters2);
                }

				return set;
			}
		}

		/// <summary>
		/// Saves an existing query object in the database.
		/// </summary>
		/// <param name="security">The FuelsManager fuelsManagerSecurityObject object.</param>
		/// <param name="query">The query to save.</param>
		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void Modify(SecurityClass security, QueryClass query)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (query == null)
			{
				throw new ArgumentNullException("query");
			}

			if (!security.HasRight(RIGHT.MODIFY_QUERIES))
			{
				throw new FMInsufficientRightsException();
			}

			Validate(query);
		
			QueryClass oldQuery = this.GetByQueryName(security, query.QueryName);

			// Verify that the old query exists
			if (oldQuery.IdentityGuid == Guid.Empty)
			{
				throw new Exception("Query Not Found");
			}

			// Verify that the ID is not already in use
			if (oldQuery.IdentityGuid != query.IdentityGuid)
			{
				throw new Exception("Query Exists");
			}

			query.UpdatedDate = DateTimeOffset.Now;
			query.UpdatedBy = security.UserID;

			using (var cmd = new SqlCommand())
			{
				query.UpdateSQL(cmd);
				this.consolidatedDA.ExecuteQuery(security, cmd);
			}

			var groups = new GroupsClass();

			// Delete old group maps
			var queryGroupMaps = new QueryGroupMapsClass();
			foreach (GroupClass group in oldQuery.AssignedGroups)
			{
				// If the group identity guid is invalid, try to look it up
				if ( group.IdentityGuid.IsEmpty() )
				{
					group.IdentityGuid = groups.GetIdentityGuid(security, group.ID);
				}

				if ( group.IdentityGuid.IsEmpty() == false )
				{
					queryGroupMaps.Purge(security, oldQuery.IdentityGuid, group.IdentityGuid);
				}
			}

			// Add new ones
			foreach (GroupClass group in query.AssignedGroups)
			{
				// If the group identity guid is invalid, try to look it up
				if (group.IdentityGuid.IsEmpty())
				{
					group.IdentityGuid = groups.GetIdentityGuid(security, group.ID);
				}

				// Some groups might make it here without a valid IdentityGuid if the query was imported
				// or if the group was deleted before the query could be saved
				if ( group.IdentityGuid.IsEmpty() == false )
				{
					var map = new QueryGroupMapClass
						{
							QueryStorageGuid = query.IdentityGuid, 
							GroupGuid = @group.IdentityGuid
						};

					queryGroupMaps.Add(security, map);
				}
			}
		}

		/// <summary>
		/// Creates a new query object.
		/// </summary>
		/// <param name="security">The FuelsManager fuelsManagerSecurityObject object.</param>
		/// <param name="topic">The query topic to use for the new query.</param>
		/// <returns>A new query object</returns>
		public QueryClass NewQuery(SecurityClass security, QueryWriterTopic topic)
		{
			var query = new QueryClass { Topic = topic };

			// Get the query writer field attributes
			QueryWriterFieldCollection fields = topic.GetFields(security, true);

			// Set the default fields
			var defaults = new QueryDefaultFieldsClass();
			QueryDefaultFieldCollectionClass defaultFields = defaults.Enumerate(security);

			foreach (var field in defaultFields)
			{
				if (field.Topic != topic.ObjectType.ToString())
				{
					continue;
				}

				var fieldAttribute = fields.FirstOrDefault(x => x.ID == field.ID);

				if (fieldAttribute != null)
				{
					query.Fields.Add(fieldAttribute);
				}
			}

			return query;
		}

		/// <summary>
		/// Purges the specified query object from the database.
		/// </summary>
		/// <param name="fuelsManagerSecurityObject">The FuelsManager security object.</param>
		/// <param name="query">The query to purge.</param>
		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void Purge(SecurityClass fuelsManagerSecurityObject, QueryClass query)
		{
			this.PurgeByIdentityGuid(fuelsManagerSecurityObject, query.IdentityGuid);
		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void PurgeByUser(SecurityClass fuelsManagerSecurityObject, Guid userGuid)
		{
			QueryCollectionClass queries = this.EnumerateForUserPurge(fuelsManagerSecurityObject, userGuid);

			foreach (QueryClass query in queries)
			{
				this.PurgeByIdentityGuid(fuelsManagerSecurityObject, query.IdentityGuid);
			}
		}

		/// <summary>
		/// Purges the query by identity GUID.
		/// </summary>
		/// <param name="security">The FuelsManager security object.</param>
		/// <param name="identityGuid">The GUID that identifies the query to purge..</param>
		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void PurgeByIdentityGuid(SecurityClass security, Guid identityGuid)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (!security.HasRight(RIGHT.MODIFY_QUERIES) && !security.HasRight(RIGHT.MODIFY_USERS))
			{
				throw new FMInsufficientRightsException();
			}

			QueryClass query = this.GetByQuickLoad(security, identityGuid, false);
			if (query.IdentityGuid == Guid.Empty)
			{
				throw new ApplicationException("Query Not Found");
			}

			var groups = new GroupsClass();

			// Delete old group maps
			var queryGroupMaps = new QueryGroupMapsClass();
			foreach (GroupClass group in query.AssignedGroups)
			{
				Guid groupGuid = groups.GetIdentityGuid(security, group.ID);
				queryGroupMaps.Purge(security, query.IdentityGuid, groupGuid);
			}

			using (var cmd = new SqlCommand())
			{
				query.PurgeSQL(cmd);
				this.consolidatedDA.ExecuteQuery(security, cmd);
			}
		}

		/// <summary>
		/// Validates the specified query.
		/// </summary>
		/// <param name="query">The query to validate.</param>
		private static void Validate(QueryClass query)
		{
			if (query == null)
			{
				throw new ApplicationException("Query object cannot be null.");
			}
			
			if (string.IsNullOrEmpty(query.QueryName))
			{
				throw new ApplicationException("Query name cannot be blank");
			}
		}

		#endregion

		#region Methods

		private void BuildFilterCriteria(
										SqlCommand cmd, 
										ref int filterCritFieldIndex, 
										QueryWriterTopic topic, 
										QueryCriteriaPhraseCollection criterion, 
										SecurityClass security)
		{
			if (criterion.Count > 0)
			{
				string filterSQL = " AND (";

				string conjunction = string.Empty;

				foreach (QueryCriteriaPhrase criteria in criterion)
				{
					if (criteria.Type == QueryCriteriaType.StartGroup)
					{
						filterSQL += " " + conjunction + " ";
						filterSQL += "(";
						conjunction = string.Empty;
					}
					else if (criteria.Type == QueryCriteriaType.Phrase)
					{
						filterSQL += " " + conjunction + " ";

                        bool hasSecondaryField = string.IsNullOrEmpty(criteria.Field.SecondaryDBFieldName) == false;
                        
                        // Determine field name - do special processing if there is a post-query alias name
						string databaseName = criteria.Field.DBFieldName;
					    string secondaryDatabaseName = criteria.Field.SecondaryDBFieldName;
						if (string.IsNullOrEmpty(topic.PostQueryAliasName) == false)
						{
							// A PostQueryAliasName is used when a queried object must wrap a sub-query
							// so that translated values still work.  At this point the query fields are using
							// the post-query alias name so we need to use them for the filter criteria.
							databaseName = topic.PostQueryAliasName + ".[" + databaseName + "]";
						    if (hasSecondaryField)
						    {
						        secondaryDatabaseName = topic.PostQueryAliasName + ".[" + secondaryDatabaseName + "]";
						    }
						}

						string value = criteria.Value.DefaultIfNull(string.Empty); // .Replace( "'", "''" );
						string value2 = criteria.Value2.DefaultIfNull(string.Empty); // .Replace( "'", "''" );

						// If the field is an enumeration type, we need to convert the filter value entered by the 
						// user into the equivalent enumeration value
						if (criteria.Field.FieldType.BaseType != null && criteria.Field.FieldType.BaseType == typeof(Enum))
						{
							// If the value the user specified is defined in the enum, get the
							// equivalent integer value; otherwise, we need to set the comparison
							// value to -1 so it does not match any saved enum value.
							if (Enum.IsDefined(criteria.Field.FieldType, value))
							{
								value = ((int)Enum.Parse(criteria.Field.FieldType, value, false)).ToString(CultureInfo.InvariantCulture);
							}
							else
							{
								value = "-1";
							}
						}

                        if (value != String.Empty && criteria.Field.FieldType.FullName == "FMBusinessObjects.DataObjects.Date")
                        {
                            var sites = new SitesClass();
                            SiteClass currentSite = sites.Get(security, security.SiteGuid, false, false, false);

                            DateTimeFormatInfo dateTimeFormatInfo = currentSite.GetDateTimeFormatInfo();

                            var strArrFormats = new string[2];
                            strArrFormats[0] = dateTimeFormatInfo.ShortDatePattern;
                            strArrFormats[1] = dateTimeFormatInfo.LongDatePattern;

	                        string[] vals;

	                        if (criteria.Operator == QueryOperator.IN)
	                        {
		                        vals = value.Split(',');
	                        }
	                        else
	                        {
		                        vals = new string[] { value };
	                        }

	                        value = string.Empty;

	                        foreach (string val in vals)
	                        {
								var newDateTime = new DateTime();

								if (!DateTime.TryParseExact(val, strArrFormats, null, DateTimeStyles.AllowWhiteSpaces, out newDateTime))
								{
									string msg = "Invalid Date Format! Must have the following format: "
												 + strArrFormats[0] + " or " + strArrFormats[1];
									throw new ApplicationException(msg);
								}

		                        value += DateEfficacy.convertToDatabaseDate(newDateTime) + ",";
	                        }

	                        value = value.TrimEnd(',');

                            // if querying BETWEEN two values, then be sure to convert the TO date to a DatabaseDate
                            if (string.IsNullOrWhiteSpace(value2) == false)
                            {
                                var newDateTime = new DateTime();

                                if (!DateTime.TryParseExact(value2, strArrFormats, null, DateTimeStyles.AllowWhiteSpaces, out newDateTime))
                                {
                                    string msg = "Invalid Date Format! Must have the following format: "
                                                 + strArrFormats[0] + " or " + strArrFormats[1];
                                    throw new ApplicationException(msg);
                                }

                                value2 = DateEfficacy.convertToDatabaseDate(newDateTime);
                            }

                        }

                        if (value != String.Empty &&
							(criteria.Field.FieldType == typeof(DateTimeOffset) ||
							 criteria.Field.FieldType == typeof(DateTimeOffset?)))
						{
							var sites = new SitesClass();
							SiteClass currentSite = sites.Get(security, security.SiteGuid, false, false, false);

							DateTimeFormatInfo dateTimeFormatInfo = currentSite.GetDateTimeFormatInfo();


							string[] vals;

	                        if (criteria.Operator == QueryOperator.IN)
	                        {
		                        vals = value.Split(',');
	                        }
	                        else
	                        {
		                        vals = new string[] { value };
	                        }

	                        value = string.Empty;

							foreach (string val in vals)
							{

								DateTimeOffset newDateTime;
								TimeSpan offsetTimeSpan;

								try
								{
									newDateTime = DateTimeOffset.Parse(val, dateTimeFormatInfo);

									var siteTimeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(currentSite.TimeZone);
									offsetTimeSpan = siteTimeZoneInfo.GetUtcOffset(newDateTime.DateTime);
								}
								catch (Exception)
								{
									string msg = "Invalid Date Time Format! Must have the following format: " + dateTimeFormatInfo.ShortDatePattern
									             + " or " + dateTimeFormatInfo.LongDatePattern + " or " + dateTimeFormatInfo.ShortDatePattern + " "
									             + dateTimeFormatInfo.ShortTimePattern + " or " + dateTimeFormatInfo.LongDatePattern + " "
									             + dateTimeFormatInfo.LongTimePattern;
									throw new ApplicationException(msg);
								}

								string day = newDateTime.Day.ToString(CultureInfo.InvariantCulture);
								string month = newDateTime.Month.ToString(CultureInfo.InvariantCulture);
								string year = newDateTime.Year.ToString(CultureInfo.InvariantCulture);
								string hour = newDateTime.Hour.ToString(CultureInfo.InvariantCulture);
								string minute = newDateTime.Minute.ToString(CultureInfo.InvariantCulture);
								string second = newDateTime.Second.ToString(CultureInfo.InvariantCulture);

								string offset = offsetTimeSpan.Hours < 0 ? "-" : "+";
								offset = offset + Math.Abs(offsetTimeSpan.Hours) + ":" + offsetTimeSpan.Minutes;

								value += year + "-" + month + "-" + day + " " + hour + ":" + minute + ":" + second + " " + offset + ",";
							}

							value = value.TrimEnd(',');
						}

						string paramName;

						switch (criteria.Operator)
						{
							case QueryOperator.Equals:
								paramName = "@filterCritParm" + filterCritFieldIndex.ToString(CultureInfo.InvariantCulture);
								filterCritFieldIndex++;
								cmd.Parameters.AddWithValue(paramName, value);
						        if (hasSecondaryField)
						        {
                                    filterSQL += "(" + databaseName + "=" + paramName + " OR " + 
                                                        secondaryDatabaseName + "=" + paramName + " ) " ;
                                }
						        else
						        {
						            filterSQL += databaseName + "=" + paramName;
						        }
								break;

							case QueryOperator.GreaterThan:
								paramName = "@filterCritParm" + filterCritFieldIndex.ToString(CultureInfo.InvariantCulture);
								filterCritFieldIndex++;
								cmd.Parameters.AddWithValue(paramName, value);
						        if (hasSecondaryField)
						        {
                                    filterSQL += "(" + databaseName + ">" + paramName + " OR " + 
                                                        secondaryDatabaseName + ">" + paramName + " ) " ;
                                }
						        else
						        {
                                    filterSQL += databaseName + ">" + paramName;
                                }
								break;

							case QueryOperator.GreaterThanEqual:
								paramName = "@filterCritParm" + filterCritFieldIndex.ToString(CultureInfo.InvariantCulture);
								filterCritFieldIndex++;
								cmd.Parameters.AddWithValue(paramName, value);
						        if (hasSecondaryField)
						        {
                                    filterSQL += "(" + databaseName + ">=" + paramName + " OR " + 
                                                        secondaryDatabaseName + ">=" + paramName + " ) " ;
                                }
						        else
						        {
                                    filterSQL += databaseName + ">=" + paramName;
                                }
								break;

							case QueryOperator.LessThan:
								paramName = "@filterCritParm" + filterCritFieldIndex.ToString(CultureInfo.InvariantCulture);
								filterCritFieldIndex++;
								cmd.Parameters.AddWithValue(paramName, value);
						        if (hasSecondaryField)
						        {
                                    filterSQL += "(" + databaseName + "<" + paramName + " OR " + 
                                                        secondaryDatabaseName + "<" + paramName + " ) " ;
                                }
						        else
						        {
                                    filterSQL += databaseName + "<" + paramName;
                                }
								break;

							case QueryOperator.LessThanEqual:
								paramName = "@filterCritParm" + filterCritFieldIndex.ToString(CultureInfo.InvariantCulture);
								filterCritFieldIndex++;
								cmd.Parameters.AddWithValue(paramName, value);
						        if (hasSecondaryField)
						        {
                                    filterSQL += "(" + databaseName + "<=" + paramName + " OR " + 
                                                        secondaryDatabaseName + "<=" + paramName + " ) " ;
                                }
						        else
						        {
                                    filterSQL += databaseName + "<=" + paramName;
                                }
								break;

							case QueryOperator.NotEqual:
								paramName = "@filterCritParm" + filterCritFieldIndex.ToString(CultureInfo.InvariantCulture);
								filterCritFieldIndex++;
								cmd.Parameters.AddWithValue(paramName, value);
						        if (hasSecondaryField)
						        {
                                    filterSQL += "(" + databaseName + "<>" + paramName + " OR " + 
                                                        secondaryDatabaseName + "<>" + paramName + " ) " ;
                                }
						        else
						        {
                                    filterSQL += databaseName + "<>" + paramName;
                                }
								break;

							case QueryOperator.Like:
								paramName = "@filterCritParm" + filterCritFieldIndex.ToString(CultureInfo.InvariantCulture);
								filterCritFieldIndex++;
								cmd.Parameters.AddWithValue(paramName, value);
						        if (hasSecondaryField)
						        {
                                    filterSQL += "(" + databaseName + " LIKE " + paramName + " OR " + 
                                                        secondaryDatabaseName + " LIKE " + paramName + " ) " ;
                                }
						        else
						        {
                                    filterSQL += databaseName + " LIKE " + paramName;
                                }
								break;

							case QueryOperator.Contains:
								paramName = "@filterCritParm" + filterCritFieldIndex.ToString(CultureInfo.InvariantCulture);
								filterCritFieldIndex++;
								cmd.Parameters.AddWithValue(paramName, "%" + value + "%");
						        if (hasSecondaryField)
						        {
                                    filterSQL += "(" + databaseName + " LIKE " + paramName + " OR " + 
                                                        secondaryDatabaseName + " LIKE " + paramName + " ) " ;
                                }
						        else
						        {
                                    filterSQL += databaseName + " LIKE " + paramName;
                                }
								break;

							case QueryOperator.IN:
								paramName = "@filterCritParm" + filterCritFieldIndex.ToString(CultureInfo.InvariantCulture);
								filterCritFieldIndex++;
						        if (hasSecondaryField)
						        {
                                    filterSQL += "(" + databaseName + " IN " + this.ProcessINOperatorValue(cmd.Parameters, paramName, value) + " OR " +
                                                        secondaryDatabaseName + " IN " + this.ProcessINOperatorValue(cmd.Parameters, paramName, value) + " ) ";
                                }
						        else
						        {
                                    filterSQL += databaseName + " IN " + this.ProcessINOperatorValue(cmd.Parameters, paramName, value);
                                }
								break;

							case QueryOperator.NotLike:
								paramName = "@filterCritParm" + filterCritFieldIndex.ToString(CultureInfo.InvariantCulture);
								filterCritFieldIndex++;
								cmd.Parameters.AddWithValue(paramName, value);
						        if (hasSecondaryField)
						        {
                                    filterSQL += "(" + databaseName + " NOT LIKE " + paramName + " OR " +
                                                        secondaryDatabaseName + " NOT LIKE " + paramName + " ) ";
                                }
						        else
						        {
                                    filterSQL += databaseName + " NOT LIKE " + paramName;
                                }
								break;

							case QueryOperator.Between:
								paramName = "@filterCritParm" + filterCritFieldIndex.ToString(CultureInfo.InvariantCulture);
								filterCritFieldIndex++;
								cmd.Parameters.AddWithValue(paramName, value);
								string param2Name = "@filterCritParm" + filterCritFieldIndex.ToString(CultureInfo.InvariantCulture);
								filterCritFieldIndex++;
								cmd.Parameters.AddWithValue(param2Name, value2);
						        if (hasSecondaryField)
						        {
                                    filterSQL += "(" + databaseName + " BETWEEN " + paramName + " AND " + param2Name + " OR " +
                                                        secondaryDatabaseName + " BETWEEN " + paramName + " AND " + param2Name + " ) ";
                                }
						        else
						        {
                                    filterSQL += databaseName + " BETWEEN " + paramName + " AND " + param2Name;
                                }
								break;

							case QueryOperator.NullOrEmpty:
						        if (hasSecondaryField)
						        {
                                    filterSQL += "(" + string.Format(" ({0} IS NULL OR {0} = '')", databaseName) + " OR " +
                                                        string.Format(" ({0} IS NULL OR {0} = '')", secondaryDatabaseName) + " ) ";
                                }
						        else
						        {
                                    filterSQL += string.Format(" ({0} IS NULL OR {0} = '')", databaseName);
                                }
								break;

							case QueryOperator.NotNullOrEmpty:
						        if (hasSecondaryField)
						        {
                                    filterSQL += "(" + string.Format(" ({0} IS NOT NULL AND {0} <> '')", databaseName) + " OR " +
                                                        string.Format(" ({0} IS NOT NULL AND {0} <> '')", secondaryDatabaseName) + " ) ";
                                }
						        else
						        {
                                    filterSQL += string.Format(" ({0} IS NOT NULL AND {0} <> '')", databaseName);
                                }
								break;
						}

						conjunction = criteria.Conjunction.ToString();
					}
					else if (criteria.Type == QueryCriteriaType.EndGroup)
					{
						if (filterSQL[filterSQL.Length - 1].Equals('('))
						{
							filterSQL += "1=1";
						}

						filterSQL += ")";
						conjunction = criteria.Conjunction.ToString();
					}
				}

				filterSQL += ")";

				cmd.CommandText += filterSQL;
			}
		}

		/// <summary>
		/// Creates the group row.
		/// </summary>
		/// <param name="table">The table of the current result set.</param>
		/// <param name="rowType">Type of the row.</param>
		/// <param name="groupValue">The group value.</param>
		/// <returns>A new group row.</returns>
		private DataRow CreateGroupRow(DataTable table, QueryRowType rowType, string groupValue)
		{
			DataRow row = table.NewRow();

			row[QueryClass.ROW_TYPE] = rowType;
			row["Internal_GroupName"] = groupValue;

			return row;
		}


		/// <summary>
		/// Is the given field an enum field?
		/// </summary>
		/// <param name="theField"></param>
		/// <returns></returns>
		private static bool IsEnumField(QueryWriterField theField)
		{
			if (theField == null)
			{
				return false;
			}

			Type fieldType = theField.FieldType.BaseType;
			return fieldType != null && fieldType.Equals(typeof(Enum));
		}

		/// <summary>
		/// Returns true if the given field/column is suitable for totalling
		/// </summary>
		/// <param name="theField"></param>
		/// <param name="theColumn"></param>
		/// <returns></returns>
		private static bool IsFieldGoodForTotalling(QueryWriterField theField, DataColumn theColumn)
		{
			Type dataType = theColumn.DataType;
			if (dataType == typeof(Boolean)
			   || dataType == typeof(Byte)
			   || dataType == typeof(Char)
			   || dataType == typeof(Int16)
			   || dataType == typeof(DateTime)
			   || dataType == typeof(DateTimeOffset)
			   || dataType == typeof(String)
			   || dataType == typeof(TimeSpan)
			   || dataType == typeof(Guid)
			   || IsEnumField(theField)
			   || theColumn.ColumnName.Equals(QueryClass.LINE_NUMBER))
			{
				return false;
			}

			return true;

		}

		/// <summary>
		/// Populates the total columns.
		/// </summary>
		/// <param name="table">The table to use for determining column types.</param>
		/// <param name="row">The row to total..</param>
		/// <param name="whereClause">The where clause to use for filtering computed results..</param>
		/// <param name="excludedTotalFields">Fields to exclude from the total calculation that are specific to the type of query (Transaction, product, etc)</param>
		private void PopulateTotalColumns(QueryClass query, DataTable table, DataRow row, string whereClause, List<string> excludedTotalFields)
		{
			foreach (DataColumn column in table.Columns)
			{
				if (!IsFieldGoodForTotalling(null, column))
				{
					if (whereClause != string.Empty && column.AllowDBNull == false)
					{
						column.AllowDBNull = true;
					}
				}
			}

			foreach (QueryWriterField Field in query.Fields)
			{
				DataColumn Column = table.Columns[Field.DBFieldName];

				if (IsFieldGoodForTotalling(Field, Column) && excludedTotalFields.FindIndex(matchingColumn => matchingColumn == Column.ColumnName) < 0)
				{
					try
					{
						row[Column.ColumnName] = table.Compute("sum([" + Column.ColumnName + "])", whereClause);
					}
					catch (Exception error)
					{
						// We're most likely trying to sum like a int and cause overflow error.
						// Rather than throwing an error, we just suppress it.  If they really really need to, they can export to excel and sum it there.
						string msg = "Error Calculating the sum for the column " + Column.ColumnName + ". " + error.Message;
						var myLogger = new Logger("Accounting");
						myLogger.Error(msg);
					}
				}
			}
		}

		/// <summary>
		/// This method will convert date/time to the site's local date and time.
		/// </summary>
		/// <param name="query">
		/// The query object for which to process datatimes.
		/// </param>
		/// <param name="dataSet">
		/// The data set with results to translate.
		/// </param>
		/// <param name="security">
		/// The current FuelsManager security object.
		/// </param>
		private void ProcessDateTimes(QueryClass query, DataSet dataSet, SecurityClass security, object topicObject)
		{
			var sites = new SitesClass();
			
			// In general we will use the current site, but for transactions we will convert 
			// the time to the zone of the owner site of the transaction.  This means
			// we will have to look up the site for each row in the results below for conversion.
			SiteClass site = sites.Get(
				security,
				security.SiteGuid,
				bGetMemberSites: false,
				getSchedulesAndProcessVariables: false,
				bGetAssociatedAliases: false);

			TimeZoneInfo timeZoneInfo = site.GetTimeZoneInfo();

			// Call the pre process if there is one defined
			MethodInfo checkField = query.Topic.ObjectType.GetMethod("QueryWriterShouldConvertDateOnField");

			var timeZoneInfoCache = new Dictionary<Guid, TimeZoneInfo>
			                        {
				                        { security.SiteGuid, timeZoneInfo }
			                        };

			foreach (QueryWriterField field in query.Fields)
			{
				if ((field.FieldType == typeof(DateTimeOffset)) || (field.FieldType == typeof(DateAndTime)) || (field.FieldType == typeof(DateTimeOffset?)))
				{
					//call to see if the field should have the date converted, some clients may not want some of the dates converted
					if (checkField != null && !(bool)checkField.Invoke(topicObject, new[] { field }))
					{
						continue;
					}

					var siteGuidExists = dataSet.Tables[0].Columns.Contains("InternalSiteGuidTimeZone");

					foreach (DataRow row in dataSet.Tables[0].Rows)
					{
						object value = row[field.DBFieldName];

						if ((value != DBNull.Value) && (value is DateTimeOffset))
						{
							if ( siteGuidExists )
							{
								Guid rowSiteGuid = row.IsNull("InternalSiteGuidTimeZone") ? Guid.Empty : (Guid) row["InternalSiteGuidTimeZone"];

								// If we find an InternalSiteGuid, use it for time conversion.
								if ( rowSiteGuid != Guid.Empty )
								{
									// Look it up in the cache first.
									if ( timeZoneInfoCache.ContainsKey( rowSiteGuid ) )
									{
										timeZoneInfo = timeZoneInfoCache[rowSiteGuid];
									}
									else
									{
										if ( rowSiteGuid != Guid.Empty )
										{
											site = sites.Get(
												security,
												rowSiteGuid,
												bGetMemberSites: false,
												getSchedulesAndProcessVariables: false,
												bGetAssociatedAliases: false );
										}

										timeZoneInfo = site.GetTimeZoneInfo();
										timeZoneInfoCache.Add( rowSiteGuid, timeZoneInfo );
									}
								}
							}

							var dateTimeOffset = (DateTimeOffset)value;
							DateTimeOffset siteDateTime = TimeConverter.ToSiteTime( timeZoneInfo, dateTimeOffset );

							row[field.DBFieldName] = siteDateTime;
						}
					}
				}
			}
		}

		/// <summary>
		/// Processes enumerations in the query during result generation.
		/// </summary>
		/// <param name="query">
		/// The query.
		/// </param>
		/// <param name="set">
		/// The result set.
		/// </param>
		private void ProcessEnumerations(QueryClass query, DataSet set)
		{
			var enumerations = new QueryWriterFieldCollection();

			foreach (QueryWriterField field in query.Fields)
			{
				if (field.FieldType.BaseType == typeof(Enum))
				{
					enumerations.Add(field);
					
					// ReSharper disable AssignNullToNotNullAttribute
					var enumColumn = new DataColumn(field.EnumFieldName, Type.GetType("System.String"));
					// ReSharper restore AssignNullToNotNullAttribute
					set.Tables[0].Columns.Add(enumColumn);
				}
			}

			if (enumerations.Count > 0)
			{
				foreach (DataRow row in set.Tables[0].Rows)
				{
					foreach (QueryWriterField field in enumerations)
					{
						object value = row[field.DBFieldName];

						if (value != DBNull.Value)
						{
							// Some enums are stored in text fields in the database for some reason and must be
							// converted to integers before enumeration translation
							if (value is string)
							{
								value = Convert.ToInt32(value);
							}

							row[field.EnumFieldName] = Enum.GetName(field.FieldType, value);
						}
					}
				}
			}
		}

		/// <summary>
		/// Processes the use of the "IN" operator.
		/// </summary>
		/// <param name="sqlParams">
		/// The sql params.
		/// </param>
		/// <param name="baseParamName">
		/// The base param name.
		/// </param>
		/// <param name="value">
		/// The value to check for.
		/// </param>
		/// <returns>
		/// A parameter list set to use for searching a set in SQL.
		/// </returns>
		private string ProcessINOperatorValue(SqlParameterCollection sqlParams, string baseParamName, string value)
		{
			int inParamIndex = 0;
			string paramList = string.Empty;

			foreach (string inVal in value.Split(','))
			{
				if (inVal.Trim().Length > 0)
				{
					string paramName = baseParamName + "_" + inParamIndex.ToString(CultureInfo.InvariantCulture);
					inParamIndex++;
					paramList += paramName + ",";
					sqlParams.AddWithValue(paramName, inVal.Trim());
				}
			}

			if (paramList.Length > 0)
			{
				return "(" + paramList.Substring(0, paramList.Length - 1) + ")";
			}

			return string.Empty;
		}

		/// <summary>
		/// Produces the totals.
		/// </summary>
		/// <param name="query">The query to use for reference.</param>
		/// <param name="set">The data set for which to produce totals.</param>
		private void ProduceTotals(QueryClass query, DataSet set, string totalsFilter)
		{
			DataTable table = set.Tables[0];
		    AllowNullsInTable(table);

			DataTable summaryTable = null;
			DataRow grandTotalRow;
			if (query.ShowSummaryLinesOnly)
			{
				summaryTable = table.Clone();
				grandTotalRow = summaryTable.NewRow();
			}
			else
			{
				grandTotalRow = table.NewRow();
			}

            // Get the names fields that we shouldn't try to total
            List<string> excludedTotalFields = new List<string>();
            PropertyInfo excludedFieldsProperty = query.Topic.ObjectType.GetProperty("QueryWriterExcludedTotalFields");

            if (excludedFieldsProperty != null)
            {
                excludedTotalFields = excludedFieldsProperty.GetValue(null) as List<string>;
            }

			// Set initial value
			grandTotalRow[QueryClass.ROW_TYPE] = QueryRowType.Total;
            this.PopulateTotalColumns(query, table, grandTotalRow, string.Empty, excludedTotalFields);

			// Now do the grouping totals if they are configured ( we only total the first group )
			string groupValue = null;
			if (query.HasGroups)
			{
				QueryWriterField group1Field = query.DataGroups[0];
				string group1FieldName = query.ProcessFieldNameWithoutAS(group1Field.DBFieldName);

				if (group1Field.FieldType == Type.GetType("System.String"))
				{
					grandTotalRow[group1Field.DBFieldName] = "Total";
				}

				for (int index = table.Rows.Count - 1; index >= 0; --index)
				{
					DataRow row = table.Rows[index];

					bool isNewGroup = string.IsNullOrEmpty(groupValue) || groupValue.NotEquals(row[group1Field.DBFieldName].ToString());

					if (isNewGroup)
					{
						DataRow subTotalRow;

						if (summaryTable != null)
						{
							subTotalRow = summaryTable.NewRow();                           
							summaryTable.Rows.InsertAt(subTotalRow, 0);
						}
						else
						{
							subTotalRow = table.NewRow();
							table.Rows.InsertAt(subTotalRow, index + 1);
						}

						groupValue = row[group1Field.DBFieldName].ToString();
                        if (table.Columns[QueryClass.ROW_TYPE].MaxLength < QueryRowType.Subtotal.ToString().Length)
                            table.Columns[QueryClass.ROW_TYPE].MaxLength = QueryRowType.Subtotal.ToString().Length;
                        subTotalRow[QueryClass.ROW_TYPE] = QueryRowType.Subtotal.ToString();
						
						if (query.ShowSummaryLinesOnly)
						{
							subTotalRow[group1Field.DBFieldName] = groupValue;
						}
						else
						{
							if (group1Field.FieldType == Type.GetType("System.String"))
							{
								subTotalRow[group1Field.DBFieldName] = "Sub-Total";
							}
						}

						string whereClause = group1FieldName + "='" + groupValue + "'";

                        this.PopulateTotalColumns(query, table, subTotalRow, whereClause, excludedTotalFields);
					}
				}
			}

			if (summaryTable != null)
			{
				summaryTable.Rows.Add(grandTotalRow);
				set.Tables.Remove(table);
				set.Tables.Add(summaryTable);
			}
			else
			{
				table.Rows.Add(grandTotalRow);
			}
		}

        /// <summary>
        /// When asking for totals, non-summable columns may end up with null values.
        /// setting column.AllowDBNull to true prevents this from being an error condition.
        /// </summary>
        /// <param name="table">the table that you want to allow nulls on all columns</param>
	    private void AllowNullsInTable(DataTable table)
	    {
	        foreach (DataColumn column in table.Columns)
	        {
	            column.AllowDBNull = true;
	        }
	    }

        /// <summary>
        /// Validate a FuelsManager Date field and convert the value to a DB-friendly string.
        /// </summary>
        /// <param name="security">Contains security information</param>
        /// <param name="value">The date value to validate and convert</param>
	    private void ParseDate(SecurityClass security, ref string value)
	    {
            var sites = new SitesClass();
            SiteClass currentSite = sites.Get(security, security.SiteGuid, false, false, false);

            DateTimeFormatInfo dateTimeFormatInfo = currentSite.GetDateTimeFormatInfo();

            var strArrFormats = new string[2];
            strArrFormats[0] = dateTimeFormatInfo.ShortDatePattern;
            strArrFormats[1] = dateTimeFormatInfo.LongDatePattern;

            var newDateTime = new DateTime();

            if (!DateTime.TryParseExact(value, strArrFormats, null, DateTimeStyles.AllowWhiteSpaces, out newDateTime))
            {
                string msg = "Invalid Date Format! Must have the following format: "
                             + strArrFormats[0] + " or " + strArrFormats[1];

                throw new ApplicationException(msg);
            }

            value = DateEfficacy.convertToDatabaseDate(newDateTime);
	    }

        /// <summary>
        /// Validate a DateTimeOffset field and convert the value to a DB-friendly string.
        /// </summary>
        /// <param name="security">Contains security information</param>
        /// <param name="value">The DateTimeOffset value to validate and convert</param>
	    private void ParseDateTimeOffset(SecurityClass security, ref string value)
	    {
            var sites = new SitesClass();
            SiteClass currentSite = sites.Get(security, security.SiteGuid, false, false, false);

            DateTimeFormatInfo dateTimeFormatInfo = currentSite.GetDateTimeFormatInfo();
            DateTimeOffset newDateTime;
            TimeSpan offsetTimeSpan;

            try
            {
                newDateTime = DateTimeOffset.Parse(value, dateTimeFormatInfo);

                var siteTimeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(currentSite.TimeZone);
                offsetTimeSpan = siteTimeZoneInfo.GetUtcOffset(newDateTime.DateTime);
            }
            catch (Exception)
            {
                string msg = "Invalid Date Time Format! Must have the following format: "
                            + dateTimeFormatInfo.ShortDatePattern
                            + " or " + dateTimeFormatInfo.LongDatePattern
                            + " or " + dateTimeFormatInfo.ShortDatePattern + " " + dateTimeFormatInfo.ShortTimePattern
                            + " or " + dateTimeFormatInfo.LongDatePattern + " " + dateTimeFormatInfo.LongTimePattern;

                throw new ApplicationException(msg);
            }

            string day = newDateTime.Day.ToString();
            string month = newDateTime.Month.ToString();
            string year = newDateTime.Year.ToString();
            string hour = newDateTime.Hour.ToString();
            string minute = newDateTime.Minute.ToString();
            string second = newDateTime.Second.ToString();

            string offset = offsetTimeSpan.Hours < 0 ? "-" : "+";
            offset = offset + offsetTimeSpan.Hours + ":" + offsetTimeSpan.Minutes;

            value = year + "-" + month + "-" + day + " " + hour + ":" + minute + ":" + second + " " + offset;
	    }

        private int QueryWriterCommandTimeout
        {
            get
            {
                int timeout = AppSettingsHelper.GetKeyValue<int>("QueryWriterCommandTimeout", DefaultCommandTimeout);
                return timeout;
            }
        }

		private string GetQueryAssemblyList(SecurityClass security)
		{
			ConfigurationSettingsClass config = new ConfigurationSettingsClass();
			ConfigurationSettingDOClass setting = config.GetByKey(security, QueryWriterTopics.QUERYWRITER_SETTING);

			return setting.SettingValue;
		}
	    #endregion
	}
}