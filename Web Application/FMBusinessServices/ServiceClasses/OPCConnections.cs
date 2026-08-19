using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Security;
using System.ServiceModel;
using System.Web;

using FMBusinessObjects.DataObjects;
using FMBusinessObjects.BusinessInterfaces;
using FMBusinessServices.DataAccessLayer;
using FMBusinessServices.InternalClasses;
using FMBusinessObjects.UtilityObjects;

namespace FMBusinessServices.ServiceClasses
{
	using FMBusinessObjects.Exceptions;

	/// <summary>
	/// Summary description for OPCConnections.
	/// </summary>
	[SecuritySafeCriticalAttribute]
	[ServiceBehavior(TransactionIsolationLevel = System.Transactions.IsolationLevel.ReadCommitted)]
	public class OPCConnectionsClass : IOPCConnections
	{
		internal ConsolidatedDAClass ConsolidatedDA = new ConsolidatedDAClass();

		public OPCConnectionsClass()
		{
		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public Guid Add(SecurityClass security, OPCConnectionClass OPCConnection)
		{
			if (security == null)
				throw new ArgumentNullException("Security");

			if (OPCConnection == null)
				throw new ArgumentNullException("OPCConnection");

			if (!security.HasRight(RIGHT.MODIFY_SITES_AND_SITE_GROUPS))
				throw new FMInsufficientRightsException();

			if (!GetIdentityGuid(security, OPCConnection.URL).IsEmpty())
				throw (new Exception("OPC Connection Exists"));

			OPCConnection.CreatedDate = DateTimeOffset.Now;
			OPCConnection.CreatedBy = security.UserID;

			using (SqlCommand cmd = OPCConnection.InsertSQLCmd_)
			{
				ConsolidatedDA.ExecuteQuery(security, cmd);
			}

			return GetIdentityGuid(security, OPCConnection.URL);
		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void Purge(SecurityClass security, Guid opcGuid)
		{
			if (security == null)
				throw new ArgumentNullException("Security");

			if (!security.HasRight(RIGHT.MODIFY_SITES_AND_SITE_GROUPS))
				throw new FMInsufficientRightsException();

			OPCConnectionClass OPCConnection = Get(security, opcGuid);
			if (OPCConnection == null)
			{
				throw (new Exception("OPC Connection does not exist."));
			}

			using (SqlCommand cmd = OPCConnection.PurgeSQLCmd)
			{
				ConsolidatedDA.ExecuteQuery(security, cmd);
			}

		}

		public OPCConnectionClass Get(SecurityClass security, Guid opcGuid)
		{
			if (security == null)
				throw new ArgumentNullException("Security");

			if (!security.HasRight(RIGHT.VIEW_SITES_AND_SITE_GROUPS)
			&& !security.HasRight(RIGHT.MODIFY_SITES_AND_SITE_GROUPS))
				throw new FMInsufficientRightsException();

			OPCConnectionClass OPCConnection = new OPCConnectionClass();
			OPCConnection.IdentityGuid = opcGuid;
			using (SqlCommand cmd = OPCConnection.SelectSQLCmd)
			{
				OPCConnection.Load(ConsolidatedDA.GetDataSet(cmd, security));
			}
			return OPCConnection;
		}


		public Guid GetIdentityGuid(SecurityClass security, string URL)
		{
			if (security == null)
				throw new ArgumentNullException("Security");

			if (!security.HasRight(RIGHT.VIEW_SITES_AND_SITE_GROUPS)
			&& !security.HasRight(RIGHT.MODIFY_SITES_AND_SITE_GROUPS)
			&& !security.HasRight(RIGHT.MODIFY_LOAD_RACK_DATA)
			&& !security.HasRight(RIGHT.MODIFY_TRANSACTION_DATA)
			&& !security.HasRight(RIGHT.ENABLEDISABLE_STATIONS)
			&& !security.HasRight(RIGHT.MODIFY_DISPATCH)
			&& !security.HasRight(RIGHT.VIEW_DISPATCH))
				throw new FMInsufficientRightsException();

			OPCConnectionClass OPCConnection = new OPCConnectionClass();
			OPCConnection.URL = URL;
			using (SqlCommand cmd = OPCConnection.SelectByIDSQLCmd(ContextUtil.IsInTransaction))
			{
				OPCConnection.Load(ConsolidatedDA.GetDataSet(cmd, security));
			}
			return OPCConnection.IdentityGuid;
		}


		public OPCConnectionCollectionClass Enumerate(SecurityClass security)
		{
			if (security == null)
				throw new ArgumentNullException("Security");

			if (!security.HasRight(RIGHT.VIEW_SITES_AND_SITE_GROUPS)
			&& !security.HasRight(RIGHT.MODIFY_SITES_AND_SITE_GROUPS))
				throw new FMInsufficientRightsException();

			OPCConnectionClass OPCConnection = new OPCConnectionClass();
			DataSet Set = null;
			using (SqlCommand cmd = OPCConnection.EnumerateSQLCmd)
			{
				ConsolidatedDA.GetDataSet(cmd, security);
			}
			OPCConnectionCollectionClass OPCConnectionCollection = new OPCConnectionCollectionClass();

			DataTable Table = Set.Tables[0];
			while (Table.Rows.Count != 0)
			{
				OPCConnection = new OPCConnectionClass();
				OPCConnection.Load(Set);
				OPCConnectionCollection.Add(OPCConnection);
				Table.Rows.RemoveAt(0);
			}

			return OPCConnectionCollection;
		}
	}
}
