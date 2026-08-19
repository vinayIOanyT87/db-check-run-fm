using System.Security;
using System.ServiceModel;
using System.Web;
using System.Data.SqlClient;
using System.Data;
using System;
using System.Collections.Generic;

using FMBusinessObjects.DataObjects;
using FMBusinessObjects.BusinessInterfaces;
using FMBusinessServices.DataAccessLayer;
using FMBusinessServices.InternalClasses;

namespace FMBusinessServices.ServiceClasses
{
	[ServiceBehavior(TransactionIsolationLevel = System.Transactions.IsolationLevel.ReadCommitted)]
	public class GoodsAndServicesClass : IGoodsAndServices
	{
		#region Private data members
		private ConsolidatedDAClass consolidatedDA;
		#endregion

		#region Constructors
		/// <summary>
		/// This is the default constructor for the GST class.
		/// </summary>
		public GoodsAndServicesClass()
		{
			this.consolidatedDA = new ConsolidatedDAClass();
		}
		#endregion

		#region Public methods
		/// <summary>
		/// Retrieves all the configured GST's from the database
		/// </summary>
		/// <returns>A collection of GST's</returns>
		public GoodsAndServicesTaxDOCollection GetAll(SecurityClass security)
		{
			var gsts = new GoodsAndServicesTaxDOCollection();

			try
			{
				var gstTaxDO = new GoodsAndServicesTaxDO();

				using (var cmd = new SqlCommand())
				{
					gstTaxDO.SelectGSTs(cmd);

					DataSet dataSet = this.consolidatedDA.GetDataSet(cmd, security);

					if ((dataSet != null) && (dataSet.Tables.Count > 0))
					{
						DataTable dataTable = dataSet.Tables[0];

						if ((dataTable.Rows != null) && (dataTable.Rows.Count > 0))
						{
							foreach (DataRow dataRow in dataTable.Rows)
							{
								var gst = new GoodsAndServicesTaxDO();
								gst.Populate(dataRow);
								gst.SiteGuid = security.SiteGuid;
								gst.SiteID = security.SiteID;
								gsts.Add(gst);
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				throw new Exception("An error occurred attempting to retrieve GST's from the database.  " + ex.Message);
			}

			return gsts;
		}

		/// <summary>
		/// Retrieves the configured GST's associated with the inventory date supplied.
		/// </summary>
		/// <returns>A GST Data Object</returns>
		public GoodsAndServicesTaxDO GetByDate(SecurityClass security, DateTimeOffset dtDate)
		{
			GoodsAndServicesTaxDO gst = new GoodsAndServicesTaxDO();

			try
			{
				GoodsAndServicesTaxDO gstDO = new GoodsAndServicesTaxDO();
				using (SqlCommand cmd = new SqlCommand())
				{
					gstDO.SelectGSTByDate(cmd, dtDate);

					DataSet dataSet = this.consolidatedDA.GetDataSet(cmd, security);

					if ((dataSet != null) && (dataSet.Tables != null) && (dataSet.Tables.Count > 0))
					{
						DataTable dataTable = dataSet.Tables[0];

						if ((dataTable.Rows != null) && (dataTable.Rows.Count > 0))
						{
							gst.Populate(dataTable.Rows[0]);
						}
					}
				}
			}
			catch (Exception ex)
			{
				throw new Exception("An error occurred attempting to retrieve GST's from the database.  " + ex.Message);
			}

			return gst;
		}

		/// <summary>
		/// Retrieves the configured GST's associated with the inventory date supplied and
		/// associated company.
		/// </summary>
		/// <returns>A GST Data Object</returns>
		public GoodsAndServicesTaxDO GetByDateAndCompany(SecurityClass security, DateTimeOffset dtDate, Guid companyGuid)
		{
			GoodsAndServicesTaxDO gst = new GoodsAndServicesTaxDO();

			try
			{
				GoodsAndServicesTaxDO gstDO = new GoodsAndServicesTaxDO();
				using (SqlCommand cmd = new SqlCommand())
				{
					gstDO.SelectGSTByDateAndCompany(cmd, dtDate, companyGuid);

					DataSet dataSet = this.consolidatedDA.GetDataSet(cmd, security);

					if ((dataSet != null) && (dataSet.Tables != null) && (dataSet.Tables.Count > 0))
					{
						DataTable dataTable = dataSet.Tables[0];

						if ((dataTable.Rows != null) && (dataTable.Rows.Count > 0))
						{
							gst.Populate(dataTable.Rows[0]);
						}
					}
				}
			}
			catch (Exception ex)
			{
				throw new Exception("An error occurred attempting to retrieve GST's from the database.  " + ex.Message);
			}

			return gst;
		}

		/// <summary>
		/// Retrieves the companies assigned to a GST
		/// </summary>
		/// <param name="gstDO"></param>
		/// <param name="security"></param>
		/// <returns></returns>
		public List<TaxCompanyMapDO> GetGSTCompanies(GoodsAndServicesTaxDO gstDO, SecurityClass security)
		{
			var companyMapList = new List<TaxCompanyMapDO>();

			// The GST DO must be a new GST. Therefore, there will be no
			// company mapping.
			if (gstDO.IdentityGuid == Guid.Empty)
			{
				return companyMapList;
			}

			try
			{
				var taxCompanyMap = new TaxCompanyMapClass();
				DataTable dataTable = taxCompanyMap.SelectGSTTaxMapCompanies(gstDO, security);

				foreach (DataRow row in dataTable.Rows)
				{
					var companyMap = new TaxCompanyMapDO
						                             {
							                             CompanyGuid = DataObject.getValue<Guid>(row["CompanyGuid"], Guid.Empty),
							                             CompanyID = DataObject.getValue<string>(row["ID"], string.Empty)
						                             };

					companyMapList.Add(companyMap);
				}
			}
			catch (Exception ex)
			{
				throw new Exception("An error occurred attempting to retrieve GST Companies from the database.  " + ex.Message);
			}

			companyMapList.Sort
			(
				delegate(TaxCompanyMapDO class1, TaxCompanyMapDO class2) 
				{
					return Comparer<string>.Default.Compare(class1.CompanyID, class2.CompanyID);
				}
			);

			return companyMapList;
		}

		/// <summary>
		/// Removes a GST from the database.
		/// </summary>
		/// <param name="gst">The GST to remove.</param>
		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void Remove(GoodsAndServicesTaxDO gst, SecurityClass security)
		{
			try
			{
				// Delete all the associated companies prior to deleting the GST entry.
				TaxCompanyMapClass taxCompanyMap = new TaxCompanyMapClass();
				taxCompanyMap.DeleteAllAssociatedCompanies(security, gst.IdentityGuid, TaxCompanyMapDO.TaxMapTypes.GST_MAP);

				GoodsAndServicesTaxDO gstDO = new GoodsAndServicesTaxDO();
				using (SqlCommand cmd = new SqlCommand())
				{
					gstDO.Delete(cmd, gst);

					this.consolidatedDA.ExecuteQuery(security, cmd);
				}
			}
			catch (Exception ex)
			{
				throw new Exception("An error occurred attempting to remove a GST. " + ex.Message);
			}
		}

		/// <summary>
		/// Saves an existing GST to the database with associated companies
		/// </summary>
		/// <param name="gst">The GST to save</param>
		/// <param name="security">Contains security credentials</param>
		/// <param name="companyList">A list of associated companies to add</param>
		/// <param name="deletedCompanyList">A list of associated companies to add remove</param>
		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void Save(GoodsAndServicesTaxDO gst,
							SecurityClass security,
							List<TaxCompanyMapDO> companyList,
							List<TaxCompanyMapDO> deletedCompanyList,
							List<TaxCompanyMapDO> completeCompanyList)
		{
			SitesClass sites = new SitesClass();
			SiteClass site = sites.GetByMemberAndProcessVariables(security, security.SiteGuid, false, false);

			if (this.UpdateGSTExists(security, gst) == true)
			{
				string strDate = (site == null) ? gst.GstDate.ToString("d") : gst.GstDate.ToString(site.ShortDatePattern);

				throw new FMBusinessObjects.Exceptions.FinanceObjectExistsException("A GST with the Date " + strDate + " already exists");
			}

			// Ensure the company associations are not mapped to another GST Code.
			List<TaxCompanyMapDO> CompanyListToCheck = null;
			TaxCompanyMapClass taxCompanyMap = new TaxCompanyMapClass();

			if (completeCompanyList != null)
			{
				CompanyListToCheck = completeCompanyList;
			}
			else
			{
				CompanyListToCheck = companyList;
			}

			List<string> companyExistList = taxCompanyMap.GSTOrganizationAlreadyMapped(security, gst, CompanyListToCheck);

			if (companyExistList.Count > 0)
			{
				string strDate = (site == null) ? gst.GstDate.ToString("d") : gst.GstDate.ToString(site.ShortDatePattern);
				string errorMsg = "The following company or companies are already associated to GST codes with the Date: " + strDate + " \n\r";

				foreach (string companyName in companyExistList)
				{
					errorMsg += (companyName + "\n");
				}

				throw new FMBusinessObjects.Exceptions.FinanceObjectExistsException(errorMsg);
			}

			try
			{
				GoodsAndServicesTaxDO gstDO = new GoodsAndServicesTaxDO();
				using (SqlCommand cmd = new SqlCommand())
				{
					gstDO.Update(cmd, gst, security.UserID);
					this.consolidatedDA.ExecuteQuery(security, cmd);

					if ((companyList != null) && (companyList.Count > 0))
					{
						taxCompanyMap.InsertGSTAssociatedCompanies(security, companyList, gst.IdentityGuid);
					}

					// Remove company associations
					if ((deletedCompanyList != null) && (deletedCompanyList.Count > 0))
					{
						taxCompanyMap.DeleteGSTAssociatedCompanies(security, deletedCompanyList, gst.IdentityGuid);
					}
				}
			}
			catch (FMBusinessObjects.Exceptions.FinanceObjectDoesNotExistException financeEx)
			{
				// Let the user interface handle this exception
				throw new Exception(financeEx.Message);
			}
			catch (FMBusinessObjects.Exceptions.FinanceObjectExistsException financeEx)
			{
				throw new Exception(financeEx.Message);
			}
			catch (Exception ex)
			{
				throw new Exception("An error occurred attempting to update a GST.  " + ex.Message);
			}
		}

		/// <summary>
		/// Adds a new GST to the database with an associated company list.
		/// </summary>
		/// <param name="gst">The GST to add</param>
		/// <param name="security">Contains security credentials</param>
		/// <param name="companyList">A list of associated companies to add</param>
		/// <returns>The guid of the newly added GST</returns>
		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public Guid Add(GoodsAndServicesTaxDO gst, SecurityClass security, List<TaxCompanyMapDO> companyList)
		{
			Guid gstGuid = Guid.Empty;
			SitesClass sites = new SitesClass();
			SiteClass site = sites.GetByMemberAndProcessVariables(security, security.SiteGuid, false, false);

			if (this.InsertGSTExists(security, gst) == true)
			{
				string strDate = (site == null) ? gst.GstDate.ToString("d") : gst.GstDate.ToString(site.ShortDatePattern);
				throw new FMBusinessObjects.Exceptions.FinanceObjectExistsException("A Goods and Services Tax with date " + strDate +
																					  " already exists.");
			}

			// Ensure the company associations are not mapped to another GST Code.
			TaxCompanyMapClass taxCompanyMap = new TaxCompanyMapClass();
			List<string> companyExistList = taxCompanyMap.GSTOrganizationAlreadyMapped(security, gst, companyList);

			if (companyExistList.Count > 0)
			{
				string strDate = (site == null) ? gst.GstDate.ToString("d") : gst.GstDate.ToString(site.ShortDatePattern);
				string errorMsg = "The following company or companies are already associated to GST codes with the Date: " + strDate + " \n\r";

				foreach (string companyName in companyExistList)
				{
					errorMsg += (companyName + "\n");
				}

				throw new FMBusinessObjects.Exceptions.FinanceObjectExistsException(errorMsg);
			}

			try
			{
				GoodsAndServicesTaxDO gstDO = new GoodsAndServicesTaxDO();
				using (SqlCommand cmd = new SqlCommand())
				{
					gstDO.IdentityGuid = Guid.NewGuid();
					gstDO.Insert(cmd, gst, security.UserID);
					this.consolidatedDA.ExecuteQuery(security, cmd);
					gstGuid = gstDO.IdentityGuid;
				}

				// Insert associated companies.
				if ((companyList != null) && (companyList.Count > 0) && (gstGuid != Guid.Empty))
				{
					taxCompanyMap.InsertGSTAssociatedCompanies(security, companyList, gstGuid);
				}
			}
			catch (FMBusinessObjects.Exceptions.FinanceObjectExistsException financeEx)
			{
				// Let the user interface handle this exception
				throw new Exception(financeEx.Message);
			}
			catch (Exception ex)
			{
				throw new Exception("An error occurred attempting to insert a GST  " + ex.Message);
			}

			return gstGuid;
		}
		#endregion

		#region Private methods
		/// <summary>
		/// Determines whether a similar GST exists in the database based on the GST Code
		/// and date combination. This is used for Updates.
		/// </summary>
		/// <param name="gst">A GST with a populated code value</param>
		/// <returns>True if a similar GST and date exists in the database.  False if otherwise.</returns>
		private bool UpdateGSTExists(SecurityClass security, GoodsAndServicesTaxDO gst)
		{
			bool exists = false;

			try
			{
				GoodsAndServicesTaxDO gstDO = new GoodsAndServicesTaxDO();
				using (SqlCommand cmd = new SqlCommand())
				{
					gstDO.UpdateGSTExists(cmd, gst, ContextUtil.IsInTransaction);
					DataSet dataSet = this.consolidatedDA.GetDataSet(cmd, security);

					if ((dataSet != null) && (dataSet.Tables.Count != 0) && (dataSet.Tables[0].Rows.Count != 0))
					{
						// If there are more than one results, then the GST/Date combination already exists.
						if (dataSet.Tables[0].Rows.Count > 1)
						{
							exists = true;
						}
						else
						{
							DataRow row = dataSet.Tables[0].Rows[0];
							Guid gstGuid = DataObject.getValue<Guid>(row["GSTGuid"], Guid.Empty);

							// If the retrieved GST guid does not match the GST guid being updated, then
							// that means another one exists.
							if (gstGuid != gst.IdentityGuid)
							{
								exists = true;
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				throw new Exception("Error determining if GST exists.  " + ex.Message);
			}

			return exists;
		}

		/// <summary>
		/// Determines whether a similar GST exists in the database based on the GST Code
		/// and date combination.
		/// </summary>
		/// <param name="gst">A GST with a populated code value</param>
		/// <returns>True if a similar GST and date exists in the database.  False if otherwise.</returns>
		private bool InsertGSTExists(SecurityClass security, GoodsAndServicesTaxDO gst)
		{
			bool exists = false;

			try
			{
				GoodsAndServicesTaxDO gstDO = new GoodsAndServicesTaxDO();
				using (SqlCommand cmd = new SqlCommand())
				{
					gstDO.InsertGSTExists(cmd, gst, ContextUtil.IsInTransaction);
					DataSet dataSet = this.consolidatedDA.GetDataSet(cmd, security);

					if ((dataSet != null) && (dataSet.Tables.Count != 0) && (dataSet.Tables[0].Rows.Count != 0))
					{
						if (dataSet.Tables[0].Rows.Count > 0)
						{
							exists = true;
						}
					}
				}
			}
			catch (Exception ex)
			{
				throw new Exception("Error determining if GST exists.  " + ex.Message);
			}

			return exists;
		}
		#endregion
	}
}
