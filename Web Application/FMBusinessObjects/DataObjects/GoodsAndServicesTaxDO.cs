/// <summary>
/// File name:	GoodsAndServicesTaxDO.cs
/// Purpose:	To contain and load goods & services tax data.
/// 
///	Comments:	Copyright (C) E+H Systems & Gauging, Inc. Norcross, GA, USA, 
///				2000. This file shall not be copied or reproduced in any form 
///				without the express written consent of Endress+Hauser.
///				
///	Author(s):	Van Thompson
///	Version:	1.0.0 Current version
///	
///	Modification History:
///		Date:			By:						Reason:
///		----------		--------------------	----------------------------------
///		yyyy-mm-dd		Developer's name		Reason for the changes
///		
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
	#region Goods And Services Tax DO Collection class
	/// <summary>
	/// A typed collection of GoodsAndServicesTaxDO objects
	/// </summary>
   [Serializable]
   [CollectionDataContract]
	public class GoodsAndServicesTaxDOCollection : List<GoodsAndServicesTaxDO>
	{
		public void RemoveByIdentityGuid(GoodsAndServicesTaxDO gst)
		{
			int idx = 0;
			foreach (GoodsAndServicesTaxDO item in this)
			{
				if (item.IdentityGuid == gst.IdentityGuid)
				{
					this.RemoveAt(idx);
					return;
				}
				idx++;
			}
		}
	}
	#endregion

	#region Goods And Services Tax DO class
	/// <summary>
	/// Summary description for GoodsAndServicesTaxDO.
	/// </summary>
	[DataContract]
   [Serializable]
	public class GoodsAndServicesTaxDO : BaseDataObject, IComparable
	{
		#region Protected data members
		[DataMember]
		protected string gstCode;
		[DataMember]
		protected double gstValue;
		[DataMember]
		protected DateTimeOffset gstDate;
		#endregion

		#region Constructors
		/// <summary>
		/// This is the default constructor for the GST Tax Data Object class.
		/// </summary>
		public GoodsAndServicesTaxDO()
		{
			this.gstCode = "";
			this.gstValue = 0.0;
			this.gstDate = DateTimeOffset.MinValue;
		}
		#endregion

		#region Properties
		public string GstCode
		{
			get { return this.gstCode; }
			set { this.gstCode = value; }
		}

		public double GstValue
		{
			get { return gstValue; }
			set { gstValue = value; }
		}

		public DateTimeOffset GstDate
		{
			get { return gstDate; }
			set { gstDate = value; }
		}
		#endregion

		#region Public methods
		public void Populate(System.Data.DataRow dr)
		{
			base._IdentityGuid = DataObject.getValue<Guid>(dr["GSTGuid"], Guid.Empty);
			this.GstCode = DataObject.getValue<string>(dr["GSTCode"], "");
			this.GstValue = DataObject.getValue<double>(dr["GSTValue"], 0.0);
			this.GstDate = DataObject.getValue<DateTimeOffset>(dr["GSTDate"], DateTimeOffset.MinValue);
		}
		#endregion

		#region SQL Methods

		/// <summary>
		/// Returns companies assigned to the passed GST SQL
		/// </summary>
		/// <param name="cmd">SQL command object</param>
		/// <param name="gstDO"></param>
		public void SelectGSTCompaniesSQL(SqlCommand cmd, GoodsAndServicesTaxDO gstDO)
		{
			cmd.CommandText = "SELECT m.CompanyGuid, c.ID " +
						 "FROM map.tblGSTToCompany m JOIN (select * from tblCompanies where tblCompanies.CompanyGuid IN (SELECT CompanyGuid FROM [erv].[udf_GetCompanyRecordVersions](@TargetSiteGuid))) c ON m.CompanyGuid = c._MasterRecordGuid " +
						 "WHERE m.GSTGuid = @GSTGuid";

			cmd.Parameters.Add("@GSTGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters[0].Value = gstDO.IdentityGuid;
			cmd.Parameters.Add("@TargetSiteGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters["@TargetSiteGuid"].Value = gstDO._SiteGuid;

		}

		/// <summary>
		/// Retrieves all configured GST's SQL
		/// </summary>
		/// <param name="cmd">SQL command object</param>
		public void SelectGSTs(SqlCommand cmd)
		{
			cmd.CommandText = "SELECT GSTGuid, GSTCode, GSTValue, GSTDate " +
						  "FROM tblGST " +
						  "ORDER BY GSTCode";
		}

		/// <summary>
		/// Retrieves a GST for a specific Date
		/// </summary>
		/// <param name="cmd">SQL command object</param>
		/// <param name="InventoryDate"></param>
		public void SelectGSTByDate(SqlCommand cmd, DateTimeOffset InventoryDate)
		{
			cmd.CommandText =
				"SELECT TOP 1 " +
				"GSTGuid, GSTCode, GSTValue, GSTDate " +
				"FROM " +
				"tblGST " +
				"WHERE " +
				"GSTDate <= @gstDate " +
				"ORDER BY GSTDate DESC";

			cmd.Parameters.Add("@gstDate", SqlDbType.DateTimeOffset);
			cmd.Parameters[0].Value = InventoryDate;
		}

		/// <summary>
		/// This method will return a data row that contains the GST value for a given 
		/// company guid and most recent inventory date.
		/// </summary>
		/// <param name="cmd">SQL command object</param>
		/// <param name="inventoryDate"></param>
		/// <param name="companyGuid"></param>
		public void SelectGSTByDateAndCompany(SqlCommand cmd, DateTimeOffset inventoryDate, Guid companyGuid)
		{
			cmd.CommandText = "SELECT TOP 1 g.GSTGuid, g.GSTCode, g.GSTValue, g.GSTDate " +
						 "FROM tblGST g LEFT OUTER JOIN map.tblGSTToCompany m on g.GSTGuid = m.GSTGuid " +
						 "WHERE g.GSTDate <= @GstDate AND m.CompanyGuid = @CompanyGuid " +
						 "ORDER BY GSTDate DESC ";

			cmd.Parameters.Add("@GstDate", SqlDbType.DateTimeOffset);
			cmd.Parameters[0].Value = inventoryDate;

			cmd.Parameters.Add("@CompanyGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters[1].Value = companyGuid;
		}

		/// <summary>
		/// Saves changes to a GoodsAndServicesTaxDO object in the database
		/// </summary>
		/// <param name="cmd">SQL command object</param>
		/// <param name="gst">The GoodsAndServicesTaxDO object to save</param>
		/// <param name="userID"></param>
		public void Update(SqlCommand cmd,
									GoodsAndServicesTaxDO gst,
									string userID)
		{
			cmd.CommandText = "UPDATE tblGST " +
						  "SET  GSTCode = @gstCode,  GSTValue = @gstValue, " +
								 "GSTDate = @gstDate,  UpdatedBy = @updatedBy,  UpdatedDate = @updatedDate " +
						  "WHERE  GSTGuid = @GSTGuid";

			cmd.Parameters.Add("@gstCode", SqlDbType.NVarChar, 50);
			cmd.Parameters.Add("@gstValue", SqlDbType.Float);
			cmd.Parameters.Add("@gstDate", SqlDbType.DateTimeOffset);
			cmd.Parameters.Add("@updatedBy", SqlDbType.NVarChar, 50);
			cmd.Parameters.Add("@updatedDate", SqlDbType.DateTimeOffset);
			cmd.Parameters.Add("@GSTGuid", SqlDbType.UniqueIdentifier);

			cmd.Parameters["@gstCode"].Value = gst.GstCode;
			cmd.Parameters["@gstValue"].Value = gst.GstValue;
			cmd.Parameters["@gstDate"].Value = gst.GstDate;
			cmd.Parameters["@updatedBy"].Value = userID;
			cmd.Parameters["@updatedDate"].Value = DateTimeOffset.Now;
			cmd.Parameters["@GSTGuid"].Value = gst.IdentityGuid;
		}

		/// <summary>
		/// Inserts the newly created GST
		/// </summary>
		/// <param name="cmd">SQL command object</param>
		/// <param name="gst">The GST to insert</param>
		/// <param name="userID"></param>
		public void Insert(SqlCommand cmd, GoodsAndServicesTaxDO gst, string userID)
		{
			cmd.CommandText = "INSERT INTO tblGST " +
							"(GSTCode, GSTValue,  GSTDate,  CreatedBy, CreatedDate, UpdatedBy, UpdatedDate, GSTGuid) " +
							"VALUES " +
							"(@gstCode, @gstValue, @gstDate, @createdBy, @createdDate,  @updatedBy, @updatedDate, @GSTGuid)";

			cmd.Parameters.Add("@gstCode", SqlDbType.NVarChar, 50);
			cmd.Parameters.Add("@gstValue", SqlDbType.Float);
			cmd.Parameters.Add("@gstDate", SqlDbType.DateTimeOffset);
			cmd.Parameters.Add("@createdBy", SqlDbType.NVarChar, 50);
			cmd.Parameters.Add("@createdDate", SqlDbType.DateTimeOffset);
			cmd.Parameters.Add("@updatedBy", SqlDbType.NVarChar, 50);
			cmd.Parameters.Add("@updatedDate", SqlDbType.DateTimeOffset);
			cmd.Parameters.Add("@GSTGuid", SqlDbType.UniqueIdentifier);

			cmd.Parameters["@gstCode"].Value = gst.GstCode;
			cmd.Parameters["@gstValue"].Value = gst.GstValue;
			cmd.Parameters["@gstDate"].Value = gst.GstDate;
			cmd.Parameters["@createdBy"].Value = userID;
			cmd.Parameters["@createdDate"].Value = DateTimeOffset.Now;
			cmd.Parameters["@updatedBy"].Value = userID;
			cmd.Parameters["@updatedDate"].Value = DateTimeOffset.Now;
			cmd.Parameters["@GSTGuid"].Value = _IdentityGuid;
		}

		/// <summary>
		/// Removes a GST from the database.
		/// </summary>
		/// <param name="gst">The GST to remove</param>
		public void Delete(SqlCommand cmd, GoodsAndServicesTaxDO gst)
		{
			cmd.CommandText = "DELETE FROM tblGST " +
						  "WHERE GSTGuid = @GstGuid";

			cmd.Parameters.Add("@GstGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters["@GstGuid"].Value = gst.IdentityGuid;
		}

		/// <summary>
		/// Determines whether a similar GST exists in the database based on the GST Code
		/// and date combination.
		/// </summary>
		/// <param name="cmd">SQL command object</param>
		/// <param name="gst">A GST with a populated code value</param>
		/// <param name="isInTransaction"></param>
		public void InsertGSTExists(SqlCommand cmd, GoodsAndServicesTaxDO gst, bool isInTransaction)
		{
            // 9/8/2016 - TLH - removing all UPLOCK hints, allowing SQL Server to determine best lock.
            cmd.CommandText = "SELECT  GSTGuid " +
						 "FROM  tblGST " + 
						 "WHERE  GSTDate = @gstDate  AND GSTCode = UPPER(@gstCode) ";

			cmd.Parameters.Add("@gstDate", SqlDbType.DateTimeOffset);
			cmd.Parameters[0].Value = gst.GstDate;

			cmd.Parameters.Add("@gstCode", SqlDbType.NVarChar, 50);
			cmd.Parameters[1].Value = gst.GstCode.ToUpper();
		}

		/// <summary>
		/// Determines whether a similar GST exists in the database based on the GST Code
		/// and date combination. This is used for Updates.
		/// </summary>
		/// <param name="cmd">SQL command object</param>
		/// <param name="gst">A GST with a populated code value</param>
		/// <param name="isInTransaction"></param>
		public void UpdateGSTExists(SqlCommand cmd, GoodsAndServicesTaxDO gst, bool isInTransaction)
		{
            // 9/8/2016 - TLH - removing all UPLOCK hints, allowing SQL Server to determine best lock.
            cmd.CommandText = "SELECT  GSTGuid " +
						 "FROM  tblGST " + 
						 "WHERE  GSTDate = @gstDate  AND GSTCode = UPPER(@gstCode) ";

			cmd.Parameters.Add("@gstDate", SqlDbType.DateTimeOffset);
			cmd.Parameters[0].Value = gst.GstDate;

			cmd.Parameters.Add("@gstCode", SqlDbType.NVarChar, 50);
			cmd.Parameters[1].Value = gst.GstCode.ToUpper();
		}
		#endregion

		#region IComparable Members
		/// <summary>
		/// Provides sorting capabilities based on the GSTCode
		/// </summary>
		/// <param name="obj">The GST that will be compared to this GST</param>
		/// <returns>-1 if less than, 0 if equal to, 1 if greater than</returns>
		public int CompareTo(object obj)
		{
			if (obj == null)
			{
				return 1;
			}

			GoodsAndServicesTaxDO gst = (GoodsAndServicesTaxDO)obj;
			return this.GstCode.CompareTo(gst.GstCode);
		}
		#endregion
	}
	#endregion
}
