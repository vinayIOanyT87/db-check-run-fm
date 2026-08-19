/// <summary>
/// File name:	TaxCompanyMapDO.cs
/// Purpose:	To contain and load Tax Company Mapping data.
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
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Data.SqlClient;
using System.Data;

namespace FMBusinessObjects.DataObjects
{
   [Serializable]
   [DataContract]
	public class TaxCompanyMapDO : IEquatable<TaxCompanyMapDO>
	{
		#region Public data members
		public enum TaxMapTypes { GST_MAP, EXCISE_MAP, MARKUP_MAP };
		#endregion

		#region Private data members
		[DataMember]
		private Guid companyGuid;
		[DataMember]
		private string companyID;
		#endregion

		#region Constructors
		/// <summary>
		/// This is the default constructor for the Goods And Services Company Map DO class
		/// </summary>
		public TaxCompanyMapDO()
		{
			this.companyID = "";
			this.companyGuid = Guid.Empty;
		}
		#endregion

		#region Properties
		/// <summary>
		/// This property returns and sets the mapped Company ID
		/// </summary>
		public string CompanyID
		{
			get { return this.companyID; }
			set { this.companyID = value; }
		}

		/// <summary>
		/// This property returns and sets the mapped Company Guid
		/// </summary>
		public Guid CompanyGuid
		{
			get { return this.companyGuid; }
			set { this.companyGuid = value; }
		}
		#endregion

		#region Public SQL Methods

		/// <summary>
		/// This method will return a list of companies that have already been mapped to a 
		/// GST entry. If none have been mapped, then an empty list is returned.
		/// Changed to look at the configured date and return the companies that are configured for the same
		/// </summary>
		/// <param name="cmd"></param>
		/// <param name="gst"></param>
		/// <param name="companyGuid"></param>
		public void GSTOrganizationAlreadyMappedSQL(SqlCommand cmd, GoodsAndServicesTaxDO gst, Guid companyGuid)
		{
			cmd.CommandText = "SELECT g.GSTGuid, g.GSTCode, g.GSTValue, g.GSTDate " +
						 "FROM tblGST g LEFT OUTER JOIN map.tblGSTToCompany m on g.GSTGuid = m.GSTGuid " +
						 "WHERE g.GSTDate = @GstDate AND m.CompanyGuid = @CompanyGuid ";

			cmd.Parameters.Add("@GstDate", SqlDbType.DateTimeOffset);
			cmd.Parameters.Add("@CompanyGuid", SqlDbType.UniqueIdentifier);

			cmd.Parameters["@GstDate"].Value = gst.GstDate;
			cmd.Parameters["@CompanyGuid"].Value = companyGuid;
		}

		/// <summary>
		/// Returns companies assigned to the passed GST
		/// </summary>
		/// <param name="cmd"></param>
		/// <param name="gstDO"></param>
		public void SelectGSTTaxMapCompaniesSQL(SqlCommand cmd, GoodsAndServicesTaxDO gstDO)
		{
			cmd.CommandText = "SELECT m.CompanyGuid, c.ID " +
						 "FROM map.tblGSTToCompany m JOIN (select * from tblCompanies where tblCompanies.CompanyGuid IN (SELECT CompanyGuid FROM [erv].[udf_GetCompanyRecordVersions](@SiteGuid))) c ON m.CompanyGuid = c._MasterRecordGuid " +
						 "WHERE m.GSTGuid = @GSTGuid";

			cmd.Parameters.Add("@GSTGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters["@GSTGuid"].Value = gstDO.IdentityGuid;
			cmd.Parameters.Add("@SiteGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters["@SiteGuid"].Value = gstDO.SiteGuid;
		}

		/// <summary>
		/// Returns companies assigned to the passed Excise
		/// </summary>
		/// <param name="cmd"></param>
		/// <param name="exciseDO"></param>
		public void SelectExciseTaxMapCompaniesSQL(SqlCommand cmd, ExciseTaxDO exciseDO)
		{
			cmd.CommandText = "SELECT m.CompanyGuid, c.ID " +
						 "FROM map.tblExciseToCompany m JOIN (select * from tblCompanies where tblCompanies.CompanyGuid IN (SELECT CompanyGuid FROM [erv].[udf_GetCompanyRecordVersions](@SiteGuid))) c ON m.CompanyGuid = c._MasterRecordGuid " +
						 "WHERE m.ExciseGuid = @ExciseGuid";

			cmd.Parameters.Add("@ExciseGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters["@ExciseGuid"].Value = exciseDO.IdentityGuid;
			cmd.Parameters.Add("@SiteGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters["@SiteGuid"].Value = exciseDO.SiteGuid;
		}

		/// <summary>
		/// Returns companies assigned to the passed Markup
		/// </summary>
		/// <param name="cmd"></param>
		/// <param name="markupDO"></param>
		public void SelectMarkupTaxMapCompaniesSQL(SqlCommand cmd, MarkupDO markupDO)
		{
			cmd.CommandText = "SELECT m.CompanyGuid, c.ID " +
						 "FROM map.tblMarkupToCompany m JOIN (select * from tblCompanies where tblCompanies.CompanyGuid IN (SELECT CompanyGuid FROM [erv].[udf_GetCompanyRecordVersions](@SiteGuid))) c ON m.CompanyGuid = c._MasterRecordGuid " +
						 "WHERE m.MarkupGuid = @MarkupGuid";

			cmd.Parameters.Add("@MarkupGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters["@MarkupGuid"].Value = markupDO.IdentityGuid;
			cmd.Parameters.Add("@SiteGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters["@SiteGuid"].Value = markupDO.SiteGuid;
		}

		/// <summary>
		/// This method will insert associated companies to the map.tblGSTToCompany table.
		/// </summary>
		/// <param name="cmd"></param>
		/// <param name="companyGuid"></param>
		/// <param name="gstGuid"></param>
		/// <param name="userID"></param>
		public void InsertGSTAssociatedCompaniesSQL(SqlCommand cmd, Guid companyGuid, Guid gstGuid, string userID)
		{
			cmd.CommandText = "INSERT INTO map.tblGSTToCompany " +
						 "(GSTGuid, CompanyGuid, CreatedBy, CreatedDate, UpdatedBy, UpdatedDate)" +
						 "VALUES ( @GstGuid, @CompanyGuid, @CreatedBy, @CreatedDate, @UpdatedBy, @UpdatedDate)";

			cmd.Parameters.Add("@GstGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters.Add("@CompanyGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters.Add("@CreatedBy", SqlDbType.NVarChar, 50);
			cmd.Parameters.Add("@CreatedDate", SqlDbType.DateTimeOffset);
			cmd.Parameters.Add("@UpdatedBy", SqlDbType.NVarChar, 50);
			cmd.Parameters.Add("@UpdatedDate", SqlDbType.DateTimeOffset);

			cmd.Parameters["@GstGuid"].Value = gstGuid;
			cmd.Parameters["@CompanyGuid"].Value = companyGuid;
			cmd.Parameters["@CreatedBy"].Value = userID;
			cmd.Parameters["@CreatedDate"].Value = DateTimeOffset.Now;
			cmd.Parameters["@UpdatedBy"].Value = userID;
			cmd.Parameters["@UpdatedDate"].Value = DateTimeOffset.Now;
		}

		/// <summary>
		/// This method will insert associated companies to the map.tblExciseToCompany table.
		/// </summary>
		/// <param name="cmd"></param>
		/// <param name="userID"></param>
		/// <param name="companyGuid"></param>
		/// <param name="exciseGuid"></param>
		public void InsertExciseAssociatedCompaniesSQL(SqlCommand cmd, string userID, Guid companyGuid, Guid exciseGuid)
		{
			cmd.CommandText = "INSERT INTO map.tblExciseToCompany " +
						 "(ExciseGuid, CompanyGuid, CreatedBy, CreatedDate, UpdatedBy, UpdatedDate)" +
						 "VALUES ( @ExciseGuid, @CompanyGuid, @CreatedBy, @CreatedDate, @UpdatedBy, @UpdatedDate)";

			cmd.Parameters.Add("@ExciseGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters.Add("@CompanyGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters.Add("@CreatedBy", SqlDbType.NVarChar, 50);
			cmd.Parameters.Add("@CreatedDate", SqlDbType.DateTimeOffset);
			cmd.Parameters.Add("@UpdatedBy", SqlDbType.NVarChar, 50);
			cmd.Parameters.Add("@UpdatedDate", SqlDbType.DateTimeOffset);

			cmd.Parameters["@ExciseGuid"].Value = exciseGuid;
			cmd.Parameters["@CompanyGuid"].Value = companyGuid;
			cmd.Parameters["@CreatedBy"].Value = userID;
			cmd.Parameters["@CreatedDate"].Value = DateTimeOffset.Now;
			cmd.Parameters["@UpdatedBy"].Value = userID;
			cmd.Parameters["@UpdatedDate"].Value = DateTimeOffset.Now;
		}

		/// <summary>
		/// This method will insert associated companies to the map.tblMarkupToCompany table.
		/// </summary>
		/// <param name="cmd"></param>
		/// <param name="userID"></param>
		/// <param name="companyGuid"></param>
		/// <param name="markupGuid"></param>
		public void InsertMarkupAssociatedCompaniesSQL(SqlCommand cmd, string userID, Guid companyGuid, Guid markupGuid)
		{
			cmd.CommandText = "INSERT INTO map.tblMarkupToCompany " +
						 "(MarkupGuid, CompanyGuid, CreatedBy, CreatedDate, UpdatedBy, UpdatedDate)" +
						 "VALUES ( @MarkupGuid, @CompanyGuid, @CreatedBy, @CreatedDate, @UpdatedBy, @UpdatedDate)";

			cmd.Parameters.Add("@MarkupGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters.Add("@CompanyGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters.Add("@CreatedBy", SqlDbType.NVarChar, 50);
			cmd.Parameters.Add("@CreatedDate", SqlDbType.DateTimeOffset);
			cmd.Parameters.Add("@UpdatedBy", SqlDbType.NVarChar, 50);
			cmd.Parameters.Add("@UpdatedDate", SqlDbType.DateTimeOffset);

			cmd.Parameters["@MarkupGuid"].Value = markupGuid;
			cmd.Parameters["@CompanyGuid"].Value = companyGuid;
			cmd.Parameters["@CreatedBy"].Value = userID;
			cmd.Parameters["@CreatedDate"].Value = DateTimeOffset.Now;
			cmd.Parameters["@UpdatedBy"].Value = userID;
			cmd.Parameters["@UpdatedDate"].Value = DateTimeOffset.Now;
		}

		/// <summary>
		/// This method will delete associated companies to the map.tblGSTToCompany table.
		/// </summary>
		/// <param name="cmd"></param>
		/// <param name="deletedCompanyGuid"></param>
		/// <param name="gstGuid"></param>
		public void DeleteGSTAssociatedCompaniesSQL(SqlCommand cmd, Guid deletedCompanyGuid, Guid gstGuid)
		{
			cmd.CommandText = "DELETE FROM map.tblGSTToCompany " +
						 "WHERE CompanyGuid = @CompanyGuid AND GSTGuid = @GstGuid";

			cmd.Parameters.Add("@GstGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters.Add("@CompanyGuid", SqlDbType.UniqueIdentifier);

			cmd.Parameters["@GstGuid"].Value = gstGuid;
			cmd.Parameters["@CompanyGuid"].Value = deletedCompanyGuid;
		}

		/// <summary>
		/// This method will delete associated companies to the map.tblExciseToCompany table.
		/// </summary>
		/// <param name="cmd"></param>
		/// <param name="deletedCompanyGuid"></param>
		/// <param name="exciseGuid"></param>
		public void DeleteExciseAssociatedCompaniesSQL(SqlCommand cmd, Guid deletedCompanyGuid, Guid exciseGuid)
		{
			cmd.CommandText = "DELETE FROM map.tblExciseToCompany " +
						 "WHERE CompanyGuid = @CompanyGuid AND ExciseGuid = @ExciseGuid";

			cmd.Parameters.Add("@ExciseGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters.Add("@CompanyGuid", SqlDbType.UniqueIdentifier);

			cmd.Parameters["@ExciseGuid"].Value = exciseGuid;
			cmd.Parameters["@CompanyGuid"].Value = deletedCompanyGuid;
		}

		/// <summary>
		/// This method will delete associated companies to the map.tblMarkupToCompany table.
		/// </summary>
		/// <param name="cmd"></param>
		/// <param name="deletedCompanyGuid"></param>
		/// <param name="markupGuid"></param>
		public void DeleteMarkupAssociatedCompaniesSQL(SqlCommand cmd, Guid deletedCompanyGuid, Guid markupGuid)
		{
			cmd.CommandText = "DELETE FROM map.tblMarkupToCompany " +
						 "WHERE CompanyGuid = @CompanyGuid AND MarkupGuid = @MarkupGuid";

			cmd.Parameters.Add("@MarkupGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters.Add("@CompanyGuid", SqlDbType.UniqueIdentifier);

			cmd.Parameters["@MarkupGuid"].Value = markupGuid;
			cmd.Parameters["@CompanyGuid"].Value = deletedCompanyGuid;
		}

		/// <summary>
		/// This method will delete all associated companies to either the map.tblGSTToCompany,
		/// map.tblExciseToCompany, or map.tblMarkupToCompany tables.
		/// </summary>
		/// <param name="cmd"></param>
		/// <param name="taxGuid"></param>
		/// <param name="mapType"></param>
		public void DeleteAllAssociatedCompaniesSQL(SqlCommand cmd, Guid taxGuid, TaxMapTypes mapType)
		{
			switch (mapType)
			{
				case TaxMapTypes.GST_MAP:
					cmd.CommandText = "DELETE FROM map.tblGSTToCompany WHERE GSTGuid = @GstGuid";
					cmd.Parameters.Add("@GstGuid", SqlDbType.UniqueIdentifier);
					cmd.Parameters["@GstGuid"].Value = taxGuid;
					break;

				case TaxMapTypes.EXCISE_MAP:
					cmd.CommandText = "DELETE FROM map.tblExciseToCompany WHERE ExciseGuid = @ExciseGuid";
					cmd.Parameters.Add("@ExciseGuid", SqlDbType.UniqueIdentifier);
					cmd.Parameters["@ExciseGuid"].Value = taxGuid;
					break;

				case TaxMapTypes.MARKUP_MAP:
					cmd.CommandText = "DELETE FROM map.tblMarkupToCompany WHERE MarkupGuid = @MarkupGuid";
					cmd.Parameters.Add("@MarkupGuid", SqlDbType.UniqueIdentifier);
					cmd.Parameters["@MarkupGuid"].Value = taxGuid;
					break;
			}
		}
		#endregion

		#region IEquatable Members
		/// <summary>
		/// This method will handle the "Contains" method for a List.
		/// </summary>
		/// <param name="other"></param>
		/// <returns></returns>
		public bool Equals(TaxCompanyMapDO other)
		{
			if (other == null)
			{
				return false;
			}
			else
			{
				if (other.companyGuid == this.companyGuid)
				{
					return true;
				}
				else
				{
					return false;
				}
			}
		}
		#endregion
	}
}
