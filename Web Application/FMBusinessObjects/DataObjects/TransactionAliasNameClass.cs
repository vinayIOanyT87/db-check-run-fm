// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TransactionAliasNameClass.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the TransactionAliasNameCollectionClass type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FMBusinessObjects.DataObjects
{
	using System;
	using System.Collections.Generic;
	using System.Data;
	using System.Data.SqlClient;
	using System.Linq;
	using System.Runtime.Serialization;
	using System.Xml.Serialization;

	/// <summary>
	/// Definition of the TransactionAliasNameClass.
	/// </summary>
	[DataContract]
	[Serializable]
	public class TransactionAliasNameClass : BaseDataObject
	{
		/// <summary>
		/// Gets or sets the alias name.
		/// </summary>
		[DataMember]
		public string AliasName { get; set; }

		/// <summary>
		/// Gets or sets the trans type id.
		/// </summary>
		[DataMember]
		public TransactionTypes TransTypeID { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether include in dispatch.
		/// </summary>
		[DataMember]
		public bool IncludeInDispatch { get; set; }

		[DataMember]
		public Guid MasterRecordGuid { get; set; }

		/// <summary>
		/// Gets the entity type.
		/// </summary>
		[XmlIgnoreAttribute]
		public override ENTITY_TYPE EntityType
		{
			get { return ENTITY_TYPE.TRANSACTION_ALIAS_NAME; }
		}

		/// <summary>
		/// Gets the parent entity type.
		/// </summary>
		[XmlIgnoreAttribute]
		public override ENTITY_TYPE ParentEntityType
		{
			get { return ENTITY_TYPE.NONE; }
		}

		/// <summary>
		/// Resets the Transaction Alias Name Class object to its initial state.
		/// </summary>
		public override void Reset()
		{
			base.Reset();
			this.AliasName = string.Empty;
			this.TransTypeID = TransactionTypes.T1_PrimaryAdjustment;
			this.IncludeInDispatch = false;
			this.MasterRecordGuid = Guid.Empty;
		}

		/// <summary>
		/// Loads the Transaction Alias Name Class data retrieved from the database.
		/// </summary>
		/// <param name="dataSet">The data set retrieved from the database</param>
		public void Load(DataSet dataSet)
		{
			if (dataSet == null)
			{
				throw new ArgumentNullException("dataSet");
			}

			this.Reset();

			DataTable table = dataSet.Tables[0];
			if (table.Rows.Count == 0)
			{
				return;
			}

			DataRow row = table.Rows[0];

			this.AliasName = DataObject.getValue<string>(row["AliasName"], string.Empty);
			this.TransTypeID = DataObject.getValue<TransactionTypes>(row["LookupTransTypeIndex"], TransactionTypes.T1_PrimaryAdjustment);
			this.IdentityGuid = DataObject.getValue<Guid>(row["TransactionAliasGuid"], Guid.Empty);
			this.MasterRecordGuid = DataObject.getValue<Guid>(row["_MasterRecordGuid"], Guid.Empty);
			this.IncludeInDispatch = DataObject.getValue<bool>(row["IncludeInDispatch"], false);
			this.SiteGuid = DataObject.getValue(row["SiteGuid"], Guid.Empty);
		}

		/// <summary>
		/// Generates the dynamic SQL to select a list of TransactionAliasNameClass objects from the database
		/// </summary>
		/// <param name="cmd">The SqlCommand object</param>
		/// <param name="security">The security object</param>
		public void EnumerateSql(SqlCommand cmd, SecurityClass security)
		{
			cmd.CommandText = "SELECT AliasName, LookupTransTypeIndex, TransactionAliasGuid, IncludeInDispatch, _MasterRecordGuid, SiteGuid" +
				" FROM tblTransactionAliases" +
				" WHERE" + this.AppendSiteWhereClause(cmd, security, "tblTransactionAliases", "TransactionAliasGuid") +
				" ORDER BY tblTransactionAliases.AliasName";
			cmd.Parameters.Add("@TargetSiteGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters["@TargetSiteGuid"].Value = security.SiteGuid;
		}

		/// <summary>
		/// Generates the dynamic SQL to select a list of TransactionAliasNameClass objects from the database
		/// that have the "IncludeInDispatch" flag set to true.
		/// </summary>
		/// <param name="cmd">The SqlCommand object</param>
		/// <param name="site">The site object</param>
		public void EnumerateForDispatchSql(SqlCommand cmd, SiteClass site)
		{
			var memberSiteGuids = site.SiteToSiteMapCollection.Select(x => x.ChildSiteGuid.ToString()).ToArray();
			string memberSites = string.Empty;

			if (memberSiteGuids.Length > 0)
			{
				memberSites += ",'" + String.Join("','", memberSiteGuids) + "'";
			}

			cmd.CommandText = "SELECT DISTINCT ta.AliasName, ta.LookupTransTypeIndex, ta.TransactionAliasGuid, ta.IncludeInDispatch, "
								+ "ta._MasterRecordGuid, ta.SiteGuid " 
								+ "FROM tblTransactionAliases ta LEFT OUTER JOIN "
								+ "map.tblEntityTransactionAliasToSite tas ON ta.TransactionAliasGuid = tas.TransactionAliasGuid " 
								+ "WHERE ta.IncludeInDispatch = 1 " 
								+ "AND (ta.SiteGuid IN (@SiteGuid" + String.Format("{0})", memberSites)
								+ " OR tas.SiteGuid = @SiteGuid) "
								+ "ORDER BY ta.AliasName";

			cmd.Parameters.AddWithValue("@SiteGuid", site.SiteGuid);
		}

		/// <summary>
		/// Override the AppendSiteWhereClause to support TransactionAlias RecordVersioning
		/// </summary>
		/// <param name="cmd"></param>
		/// <param name="security"></param>
		/// <param name="entityTable"></param>
		/// <param name="entityGuidColumn"></param>
		public override string AppendSiteWhereClause(SqlCommand cmd, SecurityClass security, string entityTable, string entityGuidColumn)
		{
			string sql = " (" + entityTable + "." + entityGuidColumn + " IN (SELECT " + entityGuidColumn + " FROM [erv].[udf_GetTransactionAliasRecordVersions](@TargetSiteGuid)" + "))";
			return sql;
		}
	}

	/// <summary>
	/// Defines a list of TransactionAliasNameClass objects.
	/// </summary>
	[CollectionDataContract]
	[Serializable]
	public class TransactionAliasNameCollectionClass : List<TransactionAliasNameClass>
	{
	}
}