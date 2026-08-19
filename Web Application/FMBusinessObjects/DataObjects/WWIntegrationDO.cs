/// <summary>
/// File name:	WWIntegrationDO.cs
/// Purpose:	To contain and load WWIntegrationDO data.
///</summary>
using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Data;
using System.Data.SqlClient;

namespace FMBusinessObjects.DataObjects
{
	#region WWIntegration DO Collection Class
	[Serializable]
   [CollectionDataContract]
	public class WWIntegrationDOCollectionClass : List<WWIntegrationDO> { }
	#endregion

	#region WWIntegration DO Class
	[DataContract]
    [Serializable]
	public class WWIntegrationDO : BaseDataObject
	{
		#region Protected data members
		[DataMember] protected Guid integrationGuid;
		[DataMember] protected string api_Username;
		[DataMember] protected string api_Password;
		[DataMember] protected string stationIATACode;
		[DataMember] protected string facility;
		[DataMember] protected string vendor;
		[DataMember] protected string baseURL;
		[DataMember] protected string requestedURL;
		[DataMember] protected Guid siteGuid;
		#endregion

		#region Constructors
		/// <summary>
		/// This is the default constructor for the WWIntegration Data Object class.
		/// </summary>
		public WWIntegrationDO()
		{
		}
		#endregion

		#region Properties
		public Guid IntegrationGuid
		{
			get { return integrationGuid; }
			set { integrationGuid = value; }
		}

		public string API_Username
		{
			get { return api_Username; }
			set { api_Username = value; }
		}

		public string API_Password
		{
			get { return api_Password; }
			set { api_Password = value; }
		}

		public string StationIATACode
		{
			get { return stationIATACode; }
			set { stationIATACode = value; }
		}

		public string Facility
		{
			get { return facility; }
			set { facility = value; }
		}

		public string Vendor
		{
			get { return vendor; }
			set { vendor = value; }
		}

		public string BaseURL
		{
			get { return baseURL; }
			set { vendor = value; }
		}

		public string RequestedURL
		{
			get { return requestedURL; }
			set { vendor = value; }
		}
		#endregion

		#region Public methods
		/// <summary>
		/// Populates the WW Integration object with data from a database
		/// </summary>
		/// <param name="dr">Contains the data used to populate the WW Integration object</param>
		public void Populate(DataRow dr)
		{
			base._IdentityGuid = DataObject.getValue<Guid>(dr["MobileDispatchSiteIntegrationInfoGuid"], Guid.Empty);
			this.integrationGuid = DataObject.getValue<Guid>(dr["IntegrationGuid"], Guid.Empty);
			this.api_Username = DataObject.getValue<string>(dr["API_Username"], "");
			this.api_Password = DataObject.getValue<string>(dr["API_Password"], "");
			this.stationIATACode = DataObject.getValue<string>(dr["StationIATA"], "");
			this.facility = DataObject.getValue<string>(dr["Facility"], "");
			this.vendor = DataObject.getValue<string>(dr["Vendor"], "");
			this.baseURL = DataObject.getValue<string>(dr["BaseURL"], "");
			this.requestedURL = DataObject.getValue<string>(dr["RequestedURL"], "");
			base._SiteGuid = DataObject.getValue<Guid>(dr["SiteGuid"], Guid.Empty);
			base._CreatedDate = DataObject.getValue<DateTimeOffset>(dr["CreatedDate"], DateTimeOffset.Now);
			base._CreatedBy = DataObject.getValue<string>(dr["CreatedBy"], ADMIN);
			base._UpdatedDate = DataObject.getValue<DateTimeOffset>(dr["UpdatedDate"], _CreatedDate);
			base._UpdatedBy = DataObject.getValue<string>(dr["UpdatedBy"], ADMIN);
		}
		#endregion

		#region Public SQL methods
		/// <summary>
		/// Returns the configured currencies for the passed site guid
		/// </summary>
		/// <param name="cmd">SQL command object</param>
		/// <param name="siteGuid">The guid of a site</param>
		public void SelectForSite(SqlCommand cmd, Guid siteGuid)
		{
			cmd.CommandText = ""
				+ "  SELECT MobileDispatchSiteIntegrationInfoGuid, "
				+ "			IntegrationGuid, "
				+ "			API_Username, "
				+ "			API_Password, "
				+ "			StationIATA, "
				+ "			Facility, "
				+ "			Vendor, "
				+ "			BaseURL, "
				+ "			RequestedURL, "
				+ "			SiteGuid, "
				+ "			CreatedBy, "
				+ "			CreatedDate, "
				+ "			UpdatedBy, "
				+ "			UpdatedDate "
				+ "	FROM tblMobileDispatchSiteIntegrationInfo "
				+ "	WHERE SiteGuid = @SiteGuid "
				+ " ORDER BY StationIATA ";

			SqlParameter guidParm = cmd.Parameters.Add("@SiteGuid", SqlDbType.UniqueIdentifier);
			guidParm.Value = siteGuid;
		}

		/// <summary>
		/// Retrieves all integrations from the database
		/// </summary>
		/// <param name="cmd">SQL command object</param>
		public void SelectIntegrations(SqlCommand cmd)
		{
			cmd.CommandText = "SELECT " +
						 "MobileDispatchSiteIntegrationInfoGuid, IntegrationGuid, API_Username, API_Password, StationIATA, Facility, Vendor, BaseURL, RequestedURL, SiteGuid, " +
						 "CreatedBy, CreatedDate, UpdatedBy, UpdatedDate " +
						 "FROM tblMobileDispatchSiteIntegrationInfo " +
						 "ORDER BY StationIATA";
		}

		/// <summary>
		/// This method will return the sql command that retrieves WW Integration data
		/// based on a MobileDispatchSiteIntegrationInfoGuid guid.
		/// </summary>
		/// <param name="cmd">SQL command object</param>
		/// <param name="MobileDispatchSiteIntegrationInfoGuid"></param>
		public void Select(SqlCommand cmd, Guid MobileDispatchSiteIntegrationInfoGuid)
		{
			cmd.CommandText = "SELECT MobileDispatchSiteIntegrationInfoGuid, IntegrationGuid, API_Username, API_Password, StationIATA, Facility, Vendor, BaseURL, RequestedURL, SiteGuid, " +
						 "CreatedBy, CreatedDate, UpdatedBy, UpdatedDate " +
						 "FROM tblMobileDispatchSiteIntegrationInfo " +
						 "WHERE MobileDispatchSiteIntegrationInfoGuid = @MobileDispatchSiteIntegrationInfoGuid";

			SqlParameter guidParm = cmd.Parameters.Add("@MobileDispatchSiteIntegrationInfoGuid", SqlDbType.UniqueIdentifier);
			guidParm.Value = MobileDispatchSiteIntegrationInfoGuid;
		}

		/// <summary>
		/// This method will return the sql command that retrieves WW Integration data
		/// based on a IntegrationGuid guid.
		/// </summary>
		/// <param name="cmd">SQL command object</param>
		/// <param name="IntegrationGuid"></param>
		public void SelectByIntegrationGuid(SqlCommand cmd, Guid IntegrationGuid)
		{
			cmd.CommandText = "SELECT MobileDispatchSiteIntegrationInfoGuid, IntegrationGuid, API_Username, API_Password, StationIATA, Facility, Vendor, BaseURL, RequestedURL, SiteGuid, " +
						 "CreatedBy, CreatedDate, UpdatedBy, UpdatedDate " +
						 "FROM tblMobileDispatchSiteIntegrationInfo " +
						 "WHERE IntegrationGuid = @IntegrationGuid";

			SqlParameter guidParm = cmd.Parameters.Add("@IntegrationGuid", SqlDbType.UniqueIdentifier);
			guidParm.Value = IntegrationGuid;
		}

		/// <summary>
		/// This method will return one row that matches the Unit Index.
		/// </summary>
		/// <param name="cmd">SQL command object</param>
		/// <param name="stationIATA"></param>
		public void SelectByStationIATA(SqlCommand cmd, string stationIATA)
		{
			cmd.CommandText = "SELECT MobileDispatchSiteIntegrationInfoGuid, IntegrationGuid, API_Username, API_Password, StationIATA, Facility, Vendor, BaseURL, RequestedURL, SiteGuid, " +
						 "CreatedBy, CreatedDate, UpdatedBy, UpdatedDate " +
						 "FROM tblMobileDispatchSiteIntegrationInfo " +
						 "WHERE StationIATA = @StationIATA";

			SqlParameter stationParm = cmd.Parameters.Add("@StationIATA", SqlDbType.Int);
			stationParm.Value = stationIATA;
		}

		/// <summary>
		/// This method will return one row that matches the Unit Index.
		/// </summary>
		/// <param name="cmd">SQL command object</param>
		/// <param name="Vendor"></param>
		public void SelectByVendor(SqlCommand cmd, string vendor)
		{
			cmd.CommandText = "SELECT MobileDispatchSiteIntegrationInfoGuid, IntegrationGuid, API_Username, API_Password, StationIATA, Facility, Vendor, BaseURL, RequestedURL, SiteGuid, " +
						 "CreatedBy, CreatedDate, UpdatedBy, UpdatedDate " +
						 "FROM tblMobileDispatchSiteIntegrationInfo " +
						 "WHERE Vendor = @Vendor";

			SqlParameter vendorParm = cmd.Parameters.Add("@Vendor", SqlDbType.Int);
			vendorParm.Value = vendor;
		}

		/// <summary>
		/// This method will return a SQL command that updates the tblMobileDispatchSiteIntegrationInfo
		/// table.
		/// </summary>
		/// <param name="cmd">SQL command object</param>
		/// <param name="WWIntegrationDO"></param>
		/// <param name="userID"></param>
		public void Update(SqlCommand cmd, WWIntegrationDO integrationDO, string userID)
		{
			// First, update the WWIntegrationDO object
			cmd.CommandText = "UPDATE tblMobileDispatchSiteIntegrationInfo " +
						 "SET " +
						 "IntegrationGuid = @IntegrationGuid, " +
						 "API_Username = @API_Username, " +
						 "API_Password = @API_Password, " +
						 "StationIATA = @StationIATA, " +
						 "Facility = @Facility, " +
						 "Vendor = @Vendor, " +
						 "BaseURL = @BaseURL, " +
						 "RequestedURL = @RequestedURL, " +
						 "SiteGuid = @SiteGuid, " +
						 "CreatedBy = @createdBy, " +
						 "CreatedDate = @createdDate, " +
						 "UpdatedBy = @updatedBy, " +
						 "UpdatedDate = @updatedDate " +
						 "WHERE MobileDispatchSiteIntegrationInfoGuid = @MobileDispatchSiteIntegrationInfoGuid";

			// Add parameters
			int i = 0;
			cmd.Parameters.Add("@IntegrationGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters[i++].Value = integrationDO.IntegrationGuid;
			cmd.Parameters.Add("@API_Username", SqlDbType.NVarChar, 255);
			cmd.Parameters[i++].Value = integrationDO.API_Username;
			cmd.Parameters.Add("@API_Password", SqlDbType.NVarChar, 255);
			cmd.Parameters[i++].Value = integrationDO.API_Password;
			cmd.Parameters.Add("@StationIATA", SqlDbType.NVarChar, 3);
			cmd.Parameters[i++].Value = integrationDO.StationIATACode;
			cmd.Parameters.Add("@Facility", SqlDbType.NVarChar, 100);
			cmd.Parameters[i++].Value = integrationDO.Facility;
			cmd.Parameters.Add("@Vendor", SqlDbType.NVarChar, 100);
			cmd.Parameters[i++].Value = integrationDO.Vendor;
			cmd.Parameters.Add("@BaseURL", SqlDbType.NVarChar, 512);
			cmd.Parameters[i++].Value = integrationDO.BaseURL;
			cmd.Parameters.Add("@RequestedURL", SqlDbType.NVarChar, 512);
			cmd.Parameters[i++].Value = integrationDO.RequestedURL;
			cmd.Parameters.Add("@SiteGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters[i++].Value = integrationDO.SiteGuid;
			cmd.Parameters.Add("@createdBy", SqlDbType.NVarChar, 100);
			cmd.Parameters[i++].Value = userID;
			cmd.Parameters.Add("@createdDate", SqlDbType.DateTimeOffset);
			cmd.Parameters[i++].Value = DateTimeOffset.Now;
			cmd.Parameters.Add("@updatedBy", SqlDbType.NVarChar, 100);
			cmd.Parameters[i++].Value = userID;
			cmd.Parameters.Add("@updatedDate", SqlDbType.DateTimeOffset);
			cmd.Parameters[i++].Value = DateTimeOffset.Now;
			cmd.Parameters.Add("@MobileDispatchSiteIntegrationInfoGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters[i++].Value = integrationDO.IdentityGuid;
		}

		/// <summary>
		/// This method will return a sql command to insert a WW Integration row.
		/// </summary>
		/// <param name="cmd">SQL command object</param>
		/// <param name="WWIntegrationDO"></param>
		/// <param name="userID"></param>
		public void Insert(SqlCommand cmd, WWIntegrationDO integrationDO, string userID)
		{
			cmd.CommandText = "INSERT INTO tblMobileDispatchSiteIntegrationInfo (" +
						 "IntegrationGuid, API_Username, API_Password, StationIATA, Facility, Vendor, BaseURL, RequestedURL, SiteGuid, " +
						 "CreatedBy, CreatedDate, UpdatedBy, UpdatedDate, MobileDispatchSiteIntegrationInfoGuid) " +
						 "VALUES (" +
						 "@IntegrationGuid, @API_Username, @API_Password, @StationIATA, @Facility, @Vendor, @BaseURL, @RequestedURL, @SiteGuid, " +
						 "@createdBy, @createdDate, @updatedBy, @updatedDate, @MobileDispatchSiteIntegrationInfoGuid)";

			// Prepare the command
			int i = 0;
			cmd.Parameters.Add("@IntegrationGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters[i++].Value = integrationDO.IntegrationGuid;
			cmd.Parameters.Add("@API_Username", SqlDbType.NVarChar, 255);
			cmd.Parameters[i++].Value = integrationDO.API_Username;
			cmd.Parameters.Add("@API_Password", SqlDbType.NVarChar, 255);
			cmd.Parameters[i++].Value = integrationDO.API_Password;
			cmd.Parameters.Add("@StationIATA", SqlDbType.NVarChar, 3);
			cmd.Parameters[i++].Value = integrationDO.StationIATACode;
			cmd.Parameters.Add("@Facility", SqlDbType.NVarChar, 100);
			cmd.Parameters[i++].Value = integrationDO.Facility;
			cmd.Parameters.Add("@Vendor", SqlDbType.NVarChar, 100);
			cmd.Parameters[i++].Value = integrationDO.Vendor;
			cmd.Parameters.Add("@BaseURL", SqlDbType.NVarChar, 512);
			cmd.Parameters[i++].Value = integrationDO.BaseURL;
			cmd.Parameters.Add("@RequestedURL", SqlDbType.NVarChar, 512);
			cmd.Parameters[i++].Value = integrationDO.RequestedURL;
			cmd.Parameters.Add("@SiteGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters[i++].Value = integrationDO.SiteGuid;
			cmd.Parameters.Add("@createdBy", SqlDbType.NVarChar, 100);
			cmd.Parameters[i++].Value = userID;
			cmd.Parameters.Add("@createdDate", SqlDbType.DateTimeOffset);
			cmd.Parameters[i++].Value = DateTimeOffset.Now;
			cmd.Parameters.Add("@updatedBy", SqlDbType.NVarChar, 100);
			cmd.Parameters[i++].Value = userID;
			cmd.Parameters.Add("@updatedDate", SqlDbType.DateTimeOffset);
			cmd.Parameters[i++].Value = DateTimeOffset.Now;
			cmd.Parameters.Add("@MobileDispatchSiteIntegrationInfoGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters[i++].Value = integrationDO.IdentityGuid;
		}

		/// <summary>
		/// Populates a SqlCommand object to checks to see if a WW Integration with the passed
		/// integration's integrationGuid exists in the database
		/// </summary>
		/// <param name="cmd">SQL command object</param>
		/// <param name="integration">The Integration to check</param>
		/// <param name="inTransaction">True if in DB transaction</param>
		public void Exists(SqlCommand cmd, WWIntegrationDO integration, bool inTransaction)
		{
			cmd.CommandText = "SELECT MobileDispatchSiteIntegrationInfoGuid " +
						 "FROM tblMobileDispatchSiteIntegrationInfo " +
						 "WHERE IntegrationGuid = @IntegrationGuid";

			cmd.Parameters.Add("@IntegrationGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters[0].Value = integration.IntegrationGuid;
		}

		/// <summary>
		/// Removes the passed Integration from the database.
		/// </summary>
		/// <param name="cmd">SQL command object</param>
		/// <param name="MobileDispatchSiteIntegrationInfoGuid">The guid of the integration to remove.</param>
		public void Delete(SqlCommand cmd, Guid MobileDispatchSiteIntegrationInfoGuid)
		{
			cmd.CommandText = "DELETE tblMobileDispatchSiteIntegrationInfo " +
						 "WHERE  MobileDispatchSiteIntegrationInfoGuid = @MobileDispatchSiteIntegrationInfoGuid";

			// Prepare the command to delete the WWIntegration
			cmd.Parameters.Add("@MobileDispatchSiteIntegrationInfoGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters[0].Value = MobileDispatchSiteIntegrationInfoGuid;
		}

		/// <summary>
		/// Removes the passed Integration from the database.
		/// </summary>
		/// <param name="cmd">SQL command object</param>
		/// <param name="IntegrationGuid">The guid of the integration to remove.</param>
		public void DeleteByIntegrationGuid(SqlCommand cmd, Guid MobileDispatchSiteIntegrationInfoGuid)
		{
			cmd.CommandText = "DELETE tblMobileDispatchSiteIntegrationInfo " +
						 "WHERE  IntegrationGuid = @IntegrationGuid";

			// Prepare the command to delete the WWIntegration
			cmd.Parameters.Add("@IntegrationGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters[0].Value = IntegrationGuid;
		}

		#endregion
	}
	#endregion
}
