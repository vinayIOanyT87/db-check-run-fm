
namespace FMBusinessServices.ServiceClasses
{
	using System;
	using System.Data;
	using System.Data.SqlClient;
	using System.Security;
	using System.ServiceModel;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.DataObjects;

	using FMBusinessServices.DataAccessLayer;

	public class OpcUAServersClass : FMServiceBase, IOpcUAServer
	{
		public ConsolidatedDAClass ConsolidatedDa = new ConsolidatedDAClass();

		public Guid Add(SecurityClass security, OpcUAServerClass opcUaServer)
		{
			using (var cmd = new SqlCommand())
			{
				opcUaServer.SetCreationStamp(security);

				opcUaServer.AutoGenerateInsertProcSQL(cmd, "gsp_OpcUaServerInsertByPK");

				cmd.Parameters["@OpcUaServerGuid"].Direction = ParameterDirection.Output;

				this.ConsolidatedDa.ExecuteQuery(security, cmd);

				opcUaServer.IdentityGuid = new Guid(cmd.Parameters["@OpcUaServerGuid"].Value.ToString());

				return opcUaServer.IdentityGuid;
			}
		}


		public void Modify(SecurityClass security, OpcUAServerClass opcUaServer)
		{
			using (var cmd = new SqlCommand())
			{
				opcUaServer.SetModifyStamp(security);
				opcUaServer.AutoGenerateModifyProcSQL(cmd, "gsp_OpcUaServerUpdateByPK");
				ConsolidatedDa.ExecuteQuery(security, cmd);
			}
		}


		public void Purge(SecurityClass security, Guid opcUAServerGuid)
		{
			var consolidatedDa = new ConsolidatedDAClass();

			using (var cmd = new SqlCommand())
			{
				cmd.CommandText = "IF 0 = (SELECT COUNT(*) FROM tblPointTag WHERE OpcUaServerGuid = @OpcUaServerGuid)"
										+ " DELETE FROM tblOpcUaServer WHERE OpcUaServerGuid = @OpcUaServerGuid";

				cmd.Parameters.AddWithValue("@OpcUaServerGuid", opcUAServerGuid);
				consolidatedDa.ExecuteQuery(security, cmd);
			}
		}


		public OpcUAServerClass Get(SecurityClass security, Guid opcUAServerGuid)
		{
			var consolidatedDa = new ConsolidatedDAClass();

			DataSet dataSet;

			using ( var cmd = new SqlCommand() )
			{
				cmd.CommandText = "SELECT * FROM tblOpcUaServer"
										+ " WHERE OpcUaServerGuid = @OpcUaServerGuid";

				cmd.Parameters.AddWithValue("@OpcUaServerGuid", opcUAServerGuid);
				dataSet = consolidatedDa.GetDataSet( cmd, security );
			}

			DataTable table = dataSet.Tables[0];

			if ( table.Rows.Count > 0 )
			{
				OpcUAServerClass opcUaServer = new OpcUAServerClass();
				opcUaServer.AutoLoad( table.Rows[0] );
				return opcUaServer;
			}

			return null;
		}


		public OpcUAServerClass GetByEndpoint(SecurityClass security, string endpoint)
		{
			var consolidatedDa = new ConsolidatedDAClass();

			DataSet dataSet;

			using (var cmd = new SqlCommand())
			{
				cmd.CommandText = "SELECT * FROM tblOpcUaServer"
										+ " WHERE SiteGuid = @SiteGuid AND ServerEndPoint = @ServerEndPoint";

				cmd.Parameters.AddWithValue("@SiteGuid", security.SiteGuid);
				cmd.Parameters.AddWithValue("@ServerEndPoint", endpoint);
				dataSet = consolidatedDa.GetDataSet(cmd, security);
			}

			DataTable table = dataSet.Tables[0];

			if (table.Rows.Count > 0)
			{
				OpcUAServerClass opcUaServer = new OpcUAServerClass();
				opcUaServer.AutoLoad(table.Rows[0]);
				return opcUaServer;
			}

			return null;
		}


		public OpcUAServerCollectionClass GetAll(SecurityClass security)
		{
			DataSet dataSet;
			using (var cmd = new SqlCommand())
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.CommandText = "usp_GetAllOpcUaServers";

				dataSet = this.ConsolidatedDa.GetDataSet(cmd, security);
			}
			DataTable table = dataSet.Tables[0];
			var ret = new OpcUAServerCollectionClass();
			foreach (DataRow row in table.Rows)
			{
				var serv = new OpcUAServerClass();

				serv.AutoLoad(row);
				ret.Add(serv);
			}

			return ret;
		}
	}
}