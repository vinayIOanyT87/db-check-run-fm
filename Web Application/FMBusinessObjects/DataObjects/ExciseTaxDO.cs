/// <summary>
/// File name:	ExciseTaxDO.cs
/// Purpose:	To contain and load excise tax data.
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
	#region Excise Tax DO Collection Class
   [Serializable]
   [CollectionDataContract]
	public class ExciseTaxDOCollection : List<ExciseTaxDO>
	{
		public void RemoveByIdentityGuid(ExciseTaxDO excise)
		{
			int idx = 0;

			foreach (ExciseTaxDO item in this)
			{
				if (item.IdentityGuid == excise.IdentityGuid)
				{
					this.RemoveAt(idx);
					return;
				}

				idx++;
			}
		}
	}
	#endregion

	#region Excise Tax DO class
	[DataContract]
   [Serializable]
	public class ExciseTaxDO : BaseDataObject, IComparable
	{
		#region Protected data members
		[DataMember]
		protected Guid productGuid;
		[DataMember]
		protected string product;
		[DataMember]
		protected string exciseCode;
		[DataMember]
		protected double exciseRate;
		[DataMember]
		protected DateTimeOffset exciseDate;
		#endregion

		#region Constructors
		/// <summary>
		/// This is the default constructor for the Excise Tax Data Object class.
		/// </summary>
		public ExciseTaxDO()
		{
			productGuid = Guid.Empty;
			product = "";
			exciseCode = "";
			exciseRate = 0.0;
			exciseDate = DateTimeOffset.MinValue;
		}
		#endregion

		#region Properties
		public Guid ProductGuid
		{
			get { return productGuid; }
			set { productGuid = value; }
		}

		public string Product
		{
			get { return product; }
			set { product = value; }
		}

		public string ExciseCode
		{
			get { return exciseCode; }
			set { exciseCode = value; }
		}

		public double ExciseRate
		{
			get { return exciseRate; }
			set { exciseRate = value; }
		}

		/// <summary>
		/// This property will return the Excise Rate in the following string
		/// format: #,###.#####
		/// </summary>
		public string ExciseRateStr
		{
			get { return this.exciseRate.ToString("#,###.00000"); }
		}

		//Eric Simmons (10-1-2008) Added to support CSI #6153
		//Change is result of email sent by R. Panachida on Mon 9/29/2008 @ 9:59 AM
		//Email is attached to CSI and is entitled "Excise Code configuration changes....htm"
		public DateTimeOffset ExciseDate
		{
			get { return exciseDate; }
			set { exciseDate = value; }
		}
		#endregion

		#region Public methods
		/// <summary>
		/// This method will load the data object with data.
		/// </summary>
		/// <param name="dr"></param>
		public void Populate(System.Data.DataRow dataRow)
		{
			base._IdentityGuid = DataObject.getValue<Guid>(dataRow["ExciseGuid"], Guid.Empty);
			this.ProductGuid = DataObject.getValue<Guid>(dataRow["ProductGuid"], Guid.Empty);
			this.Product = DataObject.getValue<string>(dataRow["ProductID"], "");
			this.ExciseCode = DataObject.getValue<string>(dataRow["ExciseCode"], "");
			this.ExciseRate = DataObject.getValue<double>(dataRow["ExciseRate"], 0.0);
			this.ExciseDate = DataObject.getValue<DateTimeOffset>(dataRow["ExciseDate"], DateTimeOffset.MinValue);
		}
		#endregion

		#region Public SQL methods
		/// <summary>
		/// Retrieves all the configured Excise Taxes from the database
		/// </summary>
		/// <param name="cmd"></param>
		public void SelectExciseTaxes(SqlCommand cmd)
		{
			cmd.CommandText = "SELECT e.ExciseGuid, e.ProductGuid, p.ProductID, e.ExciseRate, e.ExciseCode, e.ExciseDate " +
						 "FROM  tblExcise e JOIN tblProducts p ON e.ProductGuid = p.ProductGuid " +
						 "ORDER BY  e.ExciseDate";
		}

		/// <summary>
		/// Retrieves the SQL command for configured Excise Taxes based on
		/// the passed product and code.
		/// </summary>
		/// <param name="cmd">SQL command object</param>
		/// <param name="productId">A product ID</param>
		/// <param name="exciseCode">An Excise code</param>
		public void SelectForProductAndCode(SqlCommand cmd, string productId, string exciseCode)
		{
			cmd.CommandText = "SELECT e.ExciseGuid, e.ProductGuid, p.ProductID, e.ExciseRate, e.ExciseCode, e.ExciseDate " +
						 "FROM tblExcise e JOIN tblProducts p ON e.ProductGuid = p.ProductGuid " +
						 "WHERE ((p.ProductID = @productId) OR (@productId IS NULL)) AND " +
						 "((e.ExciseCode = @code) OR (@code IS NULL)) " +
						 "ORDER BY  e.ExciseCode";

			cmd.Parameters.Add("@productId", SqlDbType.NVarChar, 30);
			cmd.Parameters.Add("@code", SqlDbType.NVarChar, 50);

			int i = 0;
			cmd.Parameters[i++].Value = productId;
			cmd.Parameters[i++].Value = exciseCode;

			// Have to convert any null values to DbNull
			foreach (SqlParameter parm in cmd.Parameters)
			{
				if (parm.Value == null)
				{
					parm.Value = DBNull.Value;
				}
			}
		}

		/// <summary>
		/// Retrieves an SQL command for configured Excise Taxes based on
		/// the passed product.
		/// </summary>
		/// <param name="cmd">SQL command object</param>
		/// <param name="productId">A product id</param>
		public void SelectForProduct(SqlCommand cmd, string productId)
		{
			cmd.CommandText = "SELECT  e.ExciseGuid, e.ProductGuid, p.ProductID, e.ExciseRate, e.ExciseCode, e.ExciseDate " +
						 "FROM  tblExcise e JOIN tblProducts p ON e.ProductGuid = p.ProductGuid " +
						 "WHERE  ((p.ProductID = @productId) OR (@productId IS NULL)) " +
						 "ORDER BY p.ProductID";

			cmd.Parameters.Add("@productId", SqlDbType.NVarChar, 30);
			cmd.Parameters["@productId"].Value = productId;

			// Have to convert any null values to DbNull
			foreach (SqlParameter parm in cmd.Parameters)
			{
				if (parm.Value == null)
				{
					parm.Value = DBNull.Value;
				}
			}
		}

		/// <summary>
		/// Retrieves an SQL command for configured Excise Taxes based on
		/// the passed product, code, and date range.
		/// </summary>
		/// <param name="cmd">SQL command object</param>
		/// <param name="productId">A product id</param>
		/// <param name="dtStart">Start of the Search Range</param>
		/// <param name="dtEnd">End of the Search Range</param>
		public void SelectForProductAndDateRange(SqlCommand cmd, string productId, DateTimeOffset dtStart, DateTimeOffset dtEnd)
		{
			bool bUseProductFilter = !(productId == null || productId.Length == 0 || productId == "{All}");
			string productFilter = (bUseProductFilter) ? "(p.ProductID = @productId) AND " : "";

			cmd.CommandText = "SELECT  e.ExciseGuid, e.ProductGuid, p.ProductID, e.ExciseRate, e.ExciseCode, e.ExciseDate " +
						  "FROM  tblExcise e JOIN tblProducts p ON e.ProductGuid = p.ProductGuid " +
						  "WHERE " + productFilter + "(e.ExciseDate between @start and @end) " +
						  "ORDER BY e.ExciseDate";

			if (bUseProductFilter)
			{
				cmd.Parameters.Add("@productId", SqlDbType.NVarChar, 30);
			}

			cmd.Parameters.Add("@start", SqlDbType.DateTimeOffset);
			cmd.Parameters.Add("@end", SqlDbType.DateTimeOffset);

			if (bUseProductFilter)
			{
				cmd.Parameters["@productId"].Value = productId;
			}

			cmd.Parameters["@start"].Value = dtStart;
			cmd.Parameters["@end"].Value = dtEnd;

			// Have to convert any null values to DbNull
			foreach (SqlParameter parm in cmd.Parameters)
			{
				if (parm.Value == null)
				{
					parm.Value = DBNull.Value;
				}
			}
		}

		/// <summary>
		/// Retrieves an SQL command for configured Excise Taxes based on
		/// the passed product and date.
		/// </summary>
		/// <param name="cmd">SQL command object</param>
		/// <param name="productGuid">A product guid</param>
		/// <param name="dtDate">An Excise Date</param>
		public void SelectForProductAndDate(SqlCommand cmd, Guid productGuid, DateTimeOffset dtDate)
		{
			cmd.CommandText = "SELECT TOP 1 " +
						 "e.ExciseGuid, e.ProductGuid, e.ExciseRate, e.ExciseCode, e.ExciseDate, p.ProductID " +
						 "FROM tblExcise e JOIN tblProducts p ON e.ProductGuid = p.ProductGuid " +
						 "WHERE e.ProductGuid = @ProductGuid " +
						 "ORDER BY e.ExciseDate DESC";

			cmd.Parameters.Add("@ProductGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters.Add("@exciseDate", SqlDbType.DateTimeOffset);

			int i = 0;
			cmd.Parameters[i++].Value = productGuid;
			cmd.Parameters[i++].Value = dtDate;

			// Have to convert any null values to DbNull
			foreach (SqlParameter parm in cmd.Parameters)
			{
				if (parm.Value == null)
				{
					parm.Value = DBNull.Value;
				}
			}
		}

		/// <summary>
		/// Retrieves an SQL command that will get a row that contains the Excise information 
		/// based on the product, company, and date.
		/// </summary>
		/// <param name="cmd">SQL command object</param>
		/// <param name="productGuid"></param>
		/// <param name="dtDate"></param>
		/// <param name="companyGuid"></param>
		public void SelectForProductCompanyAndDate(SqlCommand cmd, Guid productGuid, DateTimeOffset dtDate, Guid companyGuid)
		{
			cmd.CommandText = "SELECT TOP 1 e.ExciseGuid, e.ProductGuid, e.ExciseRate, e.ExciseCode, e.ExciseDate, NULL AS ProductID " +
						 "FROM tblExcise e LEFT OUTER JOIN map.tblExciseToCompany m ON e.ExciseGuid = m.ExciseGuid " +
						 "WHERE e.ProductGuid = @ProductGuid AND m.CompanyGuid = @CompanyGuid AND e.ExciseDate <= @ExciseDate " +
						 "ORDER BY  e.ExciseDate DESC ";

			cmd.Parameters.Add("@ProductGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters.Add("@ExciseDate", SqlDbType.DateTimeOffset);
			cmd.Parameters.Add("@CompanyGuid", SqlDbType.UniqueIdentifier);

			int i = 0;
			cmd.Parameters[i++].Value = productGuid;
			cmd.Parameters[i++].Value = dtDate;
			cmd.Parameters[i++].Value = companyGuid;

			// Have to convert any null values to DbNull
			foreach (SqlParameter parm in cmd.Parameters)
			{
				if (parm.Value == null)
				{
					parm.Value = DBNull.Value;
				}
			}
		}

		/// <summary>
		/// Retrieves distinct Excise Codes for populating dropdowns.
		/// </summary>
		/// <param name="cmd">SQL command object</param>
		public void SelectExciseCodes(SqlCommand cmd)
		{
			cmd.CommandText = "SELECT  DISTINCT ExciseCode " +
						  "FROM  tblExcise " +
						  "ORDER BY ExciseCode";
		}

		/// <summary>
		/// Returns an SQL command that inserts a new Excise Tax in the database
		/// </summary>
		/// <param name="cmd">SQL command object</param>
		/// <param name="excise">The Excise Tax to insert</param>
		/// <param name="userID">User ID that is inserting</param>
		public void Insert(SqlCommand cmd, ExciseTaxDO excise, string userID)
		{
			cmd.CommandText = "INSERT INTO tblExcise " +
						 "(ProductGuid, ExciseRate, ExciseCode, ExciseDate, CreatedBy, CreatedDate, UpdatedBy,  UpdatedDate, ExciseGuid) " +
						 "VALUES " +
						 "(@ProductGuid, @exciseRate, @exciseCode, @exciseDate, @createdBy, @createdDate,  @UpdatedBy, @UpdatedDate, @ExciseGuid)";

			cmd.Parameters.Add("@ProductGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters.Add("@exciseRate", SqlDbType.Float);
			cmd.Parameters.Add("@exciseCode", SqlDbType.NVarChar, 50);
			cmd.Parameters.Add("@exciseDate", SqlDbType.DateTimeOffset);
			cmd.Parameters.Add("@createdBy", SqlDbType.NVarChar, 50);
			cmd.Parameters.Add("@createdDate", SqlDbType.DateTimeOffset);
			cmd.Parameters.Add("@updatedBy", SqlDbType.NVarChar, 50);
			cmd.Parameters.Add("@updatedDate", SqlDbType.DateTimeOffset);
			cmd.Parameters.Add("@ExciseGuid", SqlDbType.UniqueIdentifier);

			cmd.Parameters["@ProductGuid"].Value = excise.ProductGuid;
			cmd.Parameters["@exciseRate"].Value = excise.ExciseRate;
			cmd.Parameters["@exciseCode"].Value = excise.ExciseCode;
			cmd.Parameters["@exciseDate"].Value = excise.ExciseDate;
			cmd.Parameters["@createdBy"].Value = userID;
			cmd.Parameters["@createdDate"].Value = DateTimeOffset.Now;
			cmd.Parameters["@updatedBy"].Value = userID;
			cmd.Parameters["@updatedDate"].Value = DateTimeOffset.Now;
			cmd.Parameters["@ExciseGuid"].Value = excise._IdentityGuid;
		}

		/// <summary>
		/// Saves changes to the passed Excise Tax in the database with associated companies.
		/// </summary>
		/// <param name="cmd">SQL command object</param>
		/// <param name="excise">The Excise Tax to update</param>
		/// <param name="userID">ID of the user that is making the change</param>
		public void Update(SqlCommand cmd, ExciseTaxDO excise, string userID)
		{
			cmd.CommandText = "UPDATE tblExcise  SET " +
							"ProductGuid = @ProductGuid, " +
							"ExciseRate = @rate, " +
							"ExciseCode = @code, " +
							"ExciseDate = @exciseDate, " +
							"UpdatedBy = @updatedBy, " +
							"UpdatedDate = @updatedDate " +
						  "WHERE  ExciseGuid = @ExciseGuid";

			cmd.Parameters.Add("@ProductGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters.Add("@rate", SqlDbType.Float);
			cmd.Parameters.Add("@code", SqlDbType.NVarChar, 50);
			cmd.Parameters.Add("@exciseDate", SqlDbType.DateTimeOffset);
			cmd.Parameters.Add("@updatedBy", SqlDbType.NVarChar, 50);
			cmd.Parameters.Add("@updatedDate", SqlDbType.DateTimeOffset);
			cmd.Parameters.Add("@ExciseGuid", SqlDbType.UniqueIdentifier);

			cmd.Parameters["@ProductGuid"].Value = excise.ProductGuid;
			cmd.Parameters["@rate"].Value = excise.ExciseRate;
			cmd.Parameters["@code"].Value = excise.ExciseCode;
			cmd.Parameters["@exciseDate"].Value = excise.ExciseDate;
			cmd.Parameters["@updatedBy"].Value = userID;
			cmd.Parameters["@updatedDate"].Value = DateTimeOffset.Now;
			cmd.Parameters["@ExciseGuid"].Value = excise.IdentityGuid;
		}

		/// <summary>
		/// Removes an Excise Tax from the database
		/// </summary>
		/// <param name="cmd">SQL command object</param>
		/// <param name="excise">The Excise Tax to remove</param>
		public void Delete(SqlCommand cmd, ExciseTaxDO excise)
		{
			// Delete all the associated companies prior to deleting the Excise entry.
			//TaxCompanyMapDA taxCompanyMapDA = new TaxCompanyMapDA ( );
			//taxCompanyMapDA.DeleteAllAssociatedCompanies ( security, excise.Index, TaxCompanyMapDA.TaxMapTypes.EXCISE_MAP );

			cmd.CommandText = "DELETE tblExcise  WHERE  ExciseGuid = @ExciseGuid";

			cmd.Parameters.Add("@ExciseGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters[0].Value = excise.IdentityGuid;
		}

		/// <summary>
		/// Checks to see if an Excise Tax exists in the database based
		/// on Product Guid and Excise Code
		/// </summary>
		/// <param name="cmd">SQL command object</param>
		/// <param name="excise">The Excise Tax to search for</param>
		/// <param name="isInTransaction">True if in DB transaction</param>
		public void Exists(SqlCommand cmd, ExciseTaxDO excise, bool isInTransaction)
		{
            // 9/8/2016 - TLH - removing all UPLOCK hints, allowing SQL Server to determine best lock.
            cmd.CommandText = "SELECT  ExciseGuid " +
						  "FROM  tblExcise " + 
						  "WHERE  (ProductGuid = @ProductGuid AND  ExciseDate = @exciseDate) OR " +
									"ExciseGuid = @ExciseGuid";

			cmd.Parameters.Add("@ProductGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters.Add("@exciseDate", SqlDbType.DateTimeOffset);
			cmd.Parameters.Add("@ExciseGuid", SqlDbType.UniqueIdentifier);

			cmd.Parameters["@ProductGuid"].Value = excise.ProductGuid;
			cmd.Parameters["@exciseDate"].Value = excise.ExciseDate;
			cmd.Parameters["@ExciseGuid"].Value = excise.IdentityGuid;
		}

		/// <summary>
		/// This method return true if the product, excise date, and company association already
		/// exists. If not, then false is returned.
		/// </summary>
		/// <param name="cmd">SQL command object</param>
		/// <param name="excise"></param>
		/// <param name="companyList"></param>
		/// <param name="isInTransaction"></param>
		public void ExciseAndCompanyExists(SqlCommand cmd, ExciseTaxDO excise, List<TaxCompanyMapDO> companyList, bool isInTransaction)
		{
            // 9/8/2016 - TLH - removing all UPLOCK hints, allowing SQL Server to determine best lock.
            string sql = "SELECT ExciseToCompanyGuid " +
						 "FROM map.tblExciseToCompany " + 
						 "WHERE ExciseGuid IN (SELECT ExciseGuid FROM tblExcise " + 
												"WHERE ProductGuid = @ProductGuid " +
														"AND ExciseDate = @ExciseDate AND ExciseGuid <> @ExciseGuid) ";

			cmd.Parameters.Add("@ProductGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters.Add("@ExciseDate", SqlDbType.DateTimeOffset);
			cmd.Parameters.Add("@ExciseGuid", SqlDbType.UniqueIdentifier);

			cmd.Parameters["@ProductGuid"].Value = excise.ProductGuid;
			cmd.Parameters["@ExciseDate"].Value = excise.ExciseDate;
			cmd.Parameters["@ExciseGuid"].Value = excise.IdentityGuid;

			string inClause = "";
			string companyGuidParm = "";
			int parmCount = 0;

			if ((companyList != null) && (companyList.Count > 0))
			{
				inClause = "AND CompanyGuid IN (";

				foreach (TaxCompanyMapDO companyMapDO in companyList)
				{
					parmCount++;
					companyGuidParm = "@CompanyGuid" + parmCount.ToString();
					inClause = inClause + companyGuidParm + ", ";

					cmd.Parameters.Add(companyGuidParm, SqlDbType.UniqueIdentifier);
					cmd.Parameters[companyGuidParm].Value = companyMapDO.CompanyGuid;
				}

				int lastComma = inClause.LastIndexOf(", ");
				inClause = inClause.Substring(0, lastComma);
				inClause += ") ";
			}

			cmd.CommandText = sql + inClause;
		}
		#endregion

		#region IComparable Members
		/// <summary>
		/// Provides sorting capabilities based on the ExciseCode
		/// </summary>
		/// <param name="obj">The ExciseTax that will be compared to this ExciseTax</param>
		/// <returns>-1 if less than, 0 if equal to, 1 if greater than</returns>
		public int CompareTo(object obj)
		{
			if (obj == null)
			{
				return 1;
			}

			ExciseTaxDO excise = (ExciseTaxDO)obj;
			return this.ExciseCode.CompareTo(excise.ExciseCode);
		}
		#endregion
	}
	#endregion
}
