/// <summary>
/// File name:	MarkupDO.cs
/// Purpose:	To contain and load markup tax data.
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
	#region Markup DO Collection Class
   [Serializable]
   [CollectionDataContract]
	public class MarkupDOCollection : List<MarkupDO>
	{
		public void RemoveByIdentityGuid(MarkupDO markup)
		{
			int idx = 0;
			foreach (MarkupDO item in this)
			{
				if (item.IdentityGuid == markup.IdentityGuid)
				{
					this.RemoveAt(idx);
					return;
				}
				idx++;
			}
		}
	}
	#endregion

	#region Markup DO Class
	/// <summary>
	/// Summary description for MarkupDO.
	/// </summary>
   [Serializable]
   [DataContract]
	public class MarkupDO : BaseDataObject, IComparable
	{
		#region Private data members
		[DataMember]
		private string purchasingEntity;
		[DataMember]
		private double markupRate;
		[DataMember]
		private double quantities;
		[DataMember]
		private Hashtable companies;
		#endregion

		#region Constructors
		/// <summary>
		/// This is the default constructor for the Markup Data Object class.
		/// </summary>
		public MarkupDO()
		{
		}
		#endregion

		#region Properties
		public string PurchasingEntity
		{
			get { return purchasingEntity; }
			set { purchasingEntity = value; }
		}

		public double MarkupRate
		{
			get { return markupRate; }
			set { markupRate = value; }
		}

		/// <summary>
		/// Quantities apply to the Navy service type
		/// </summary>
		public double Quantities
		{
			get { return quantities; }
			set { quantities = value; }
		}

		public Hashtable Companies
		{
			get { return companies; }
			set { companies = value; }
		}
		#endregion

		#region Public methods
		/// <summary>
		/// This method will populate the object.
		/// </summary>
		/// <param name="dr"></param>
		public void Populate(System.Data.DataRow dr)
		{
			base._IdentityGuid = DataObject.getValue<Guid>(dr["MarkupGuid"], Guid.Empty);
			this.PurchasingEntity = DataObject.getValue<string>(dr["PurchasingEntity"], "");
			this.MarkupRate = DataObject.getValue<double>(dr["MarkupRate"], 0.0);
		}
		#endregion

		#region SQL Methods
		/// <summary>
		/// Retrieves all Markups sorted by Purchasing entity
		/// </summary>
		/// <param name="security">Contains security credentials</param>
		/// <returns>SQL Command object</returns>
		public void SelectMarkups(SqlCommand cmd)
		{
			cmd.CommandText = "SELECT MarkupGuid, LookupServiceTypeIndex, PurchasingEntity, MarkupRate " +
						  "FROM tblMarkup " +
						  "ORDER BY PurchasingEntity";
		}

		/// <summary>
		/// Returns all the Markups for a given service type
		/// </summary>
		/// <param name="security">Contains security credentials</param>
		/// <param name="serviceType">The service type for which the markups are returned</param>
		/// <returns>SQL command object</returns>
		public void SelectMarkupsForServiceType(SqlCommand cmd, ServiceTypes serviceType)
		{
			cmd.CommandText = "SELECT MarkupGuid, LookupServiceTypeIndex, PurchasingEntity, MarkupRate " +
						  "FROM tblMarkup " +
						  "WHERE LookupServiceTypeIndex = @LookupServiceTypeIndex " +
						  "ORDER BY PurchasingEntity";

			cmd.Parameters.Add("@LookupServiceTypeIndex", SqlDbType.Int);
			cmd.Parameters[0].Value = (int)serviceType == 0 ? (object)DBNull.Value : (object)((int)serviceType);
		}

		/// <summary>
		/// Returns companies assigned to the passed Markup
		/// </summary>
		/// <param name="markup">A Markup object</param>
		/// <param name="security">Contains security credentials</param>
		/// <returns>SQL Command object</returns>
		public void SelectMarkupCompanies(SqlCommand cmd, MarkupDO markup)
		{
			cmd.CommandText = "SELECT  a.CompanyGuid, b.ID " +
						  "FROM  map.tblMarkupToCompany a  JOIN (select * from tblCompanies where tblCompanies.CompanyGuid IN (SELECT CompanyGuid FROM [erv].[udf_GetCompanyRecordVersions](@TargetSiteGuid))) b ON a.CompanyGuid = b._MasterRecordGuid " +
						  "WHERE  a.MarkupGuid = @MarkupGuid";

			cmd.Parameters.Add("@MarkupGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters[0].Value = markup.IdentityGuid;
			cmd.Parameters.Add("@TargetSiteGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters["@TargetSiteGuid"].Value = markup._SiteGuid;

		}

		/// <summary>
		/// Saves a new markup to the database
		/// </summary>
		/// <param name="markup">The markup to save</param>
		/// <param name="userID">Contains ID for user making the change</param>
		/// <returns>SQL command object</returns>
		public void Insert(SqlCommand cmd, MarkupDO markup, string userID)
		{
			cmd.CommandText = "INSERT INTO tblMarkup " +
							"(PurchasingEntity, MarkupRate, CreatedBy,  CreatedDate, UpdatedBy, UpdatedDate, MarkupGuid) " +
							"VALUES " +
							"(@purchasingEntity, @rate, @createdBy,  @createdDate, @updatedBy, @updatedDate, @MarkupGuid)";

			cmd.Parameters.Add("@purchasingEntity", SqlDbType.NVarChar, 50);
			cmd.Parameters.Add("@rate", SqlDbType.Float);
			cmd.Parameters.Add("@createdBy", SqlDbType.NVarChar, 50);
			cmd.Parameters.Add("@createdDate", SqlDbType.DateTimeOffset);
			cmd.Parameters.Add("@updatedBy", SqlDbType.NVarChar, 50);
			cmd.Parameters.Add("@updatedDate", SqlDbType.DateTimeOffset);
			cmd.Parameters.Add("@MarkupGuid", SqlDbType.UniqueIdentifier);

			cmd.Parameters["@purchasingEntity"].Value = markup.PurchasingEntity;
			cmd.Parameters["@rate"].Value = markup.MarkupRate;
			cmd.Parameters["@createdBy"].Value = userID;
			cmd.Parameters["@createdDate"].Value = DateTimeOffset.Now;
			cmd.Parameters["@updatedBy"].Value = userID;
			cmd.Parameters["@updatedDate"].Value = DateTimeOffset.Now;
			cmd.Parameters["@MarkupGuid"].Value = _IdentityGuid;
		}

		/// <summary>
		/// Inserts a company for the passed markup into the Markup Company table
		/// </summary>
		/// <param name="markup">The Markup for which the company will be assigned</param>
		/// <param name="companyGuid">The guid of the company to assign</param>
		/// <param name="security">Contains security credentials</param>
		public void InsertMarkupCompany(SqlCommand cmd, MarkupDO markup, Guid companyGuid, string userID)
		{
			cmd.CommandText = "INSERT INTO map.tblMarkupToCompany " +
							"(MarkupGuid, CompanyGuid, CreatedBy, CreatedDate, UpdatedBy, UpdatedDate) " +
						  "VALUES " +
							"(@markupGuid, @companyGuid, @createdBy, @createdDate, @updatedBy,  @updatedDate)";

			cmd.Parameters.Add("@markupGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters.Add("@companyGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters.Add("@createdBy", SqlDbType.NVarChar, 50);
			cmd.Parameters.Add("@createdDate", SqlDbType.DateTimeOffset);
			cmd.Parameters.Add("@updatedBy", SqlDbType.NVarChar, 50);
			cmd.Parameters.Add("@updatedDate", SqlDbType.DateTimeOffset);

			cmd.Parameters["@markupGuid"].Value = markup.IdentityGuid;
			cmd.Parameters["@companyGuid"].Value = companyGuid;
			cmd.Parameters["@createdBy"].Value = userID;
			cmd.Parameters["@createdDate"].Value = DateTimeOffset.Now;
			cmd.Parameters["@updatedBy"].Value = userID;
			cmd.Parameters["@updatedDate"].Value = DateTimeOffset.Now;
		}

		/// <summary>
		/// Saves changes to the passed Markup in the database
		/// </summary>
		/// <param name="markup">The Markup to update</param>
		/// <param name="security">Contains security credentials</param>
		public void Update(SqlCommand cmd, MarkupDO markup, string userID)
		{
			cmd.CommandText = "UPDATE tblMarkup  SET " +
							"PurchasingEntity = @purchasingEntity, " +
							"MarkupRate = @rate, " +
							"UpdatedBy = @updatedBy, " +
							"UpdatedDate = @updatedDate " +
						  "WHERE  MarkupGuid = @MarkupGuid";

			cmd.Parameters.Add("@purchasingEntity", SqlDbType.NVarChar, 50);
			cmd.Parameters.Add("@rate", SqlDbType.Float);
			cmd.Parameters.Add("@updatedBy", SqlDbType.NVarChar, 50);
			cmd.Parameters.Add("@updatedDate", SqlDbType.DateTimeOffset);
			cmd.Parameters.Add("@MarkupGuid", SqlDbType.UniqueIdentifier);

			cmd.Parameters["@purchasingEntity"].Value = markup.PurchasingEntity;
			cmd.Parameters["@rate"].Value = markup.MarkupRate;
			cmd.Parameters["@updatedBy"].Value = userID;
			cmd.Parameters["@updatedDate"].Value = DateTimeOffset.Now;
			cmd.Parameters["@MarkupGuid"].Value = markup.IdentityGuid;
		}

		/// <summary>
		/// Removes a Markup from the database
		/// </summary>
		/// <param name="markup">The Markup to remove</param>
		public void Delete(SqlCommand cmd, MarkupDO markup)
		{
			cmd.CommandText = "DELETE tblMarkup WHERE MarkupGuid = @MarkupGuid";

			cmd.Parameters.Add("@MarkupGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters[0].Value = markup.IdentityGuid;
		}

		/// <summary>
		/// This method returns True if the company assignment already exists.
		/// </summary>
		/// <param name="markup"></param>
		/// <param name="companyGuid"></param>
		/// <returns></returns>
		public void CompanyAssignmentExists(SqlCommand cmd, MarkupDO markup, Guid companyGuid)
		{
			cmd.CommandText = "SELECT MarkupToCompanyGuid " +
						 "FROM map.tblMarkupToCompany " +
						 "WHERE MarkupGuid = @MarkupGuid AND CompanyGuid = @CompanyGuid";

			cmd.Parameters.Add("@MarkupGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters.Add("@CompanyGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters[0].Value = markup.IdentityGuid;
			cmd.Parameters[1].Value = companyGuid;
		}
		#endregion

		#region IComparable Members
		/// <summary>
		/// Used in collections to sort by Purchasing Entity
		/// </summary>
		/// <param name="obj">The Markup that will be compared to this markup</param>
		/// <returns></returns>
		public int CompareTo(object obj)
		{
			if (obj == null)
			{
				return 1;
			}

			MarkupDO markup = (MarkupDO)obj;
			return this.PurchasingEntity.CompareTo(markup.PurchasingEntity);
		}
		#endregion
	}
	#endregion
}
