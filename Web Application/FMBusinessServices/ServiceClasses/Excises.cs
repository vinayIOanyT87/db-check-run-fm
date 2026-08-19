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
	[SecuritySafeCriticalAttribute]
	[ServiceBehavior(TransactionIsolationLevel = System.Transactions.IsolationLevel.ReadCommitted)]
	public class ExcisesClass : IExcises
	{
		#region Private data members
		private ConsolidatedDAClass consolidatedDA;
		#endregion

		#region Constructors
		/// <summary>
		/// This is the default constructor for the Excises class.
		/// </summary>
		public ExcisesClass()
		{
			this.consolidatedDA = new ConsolidatedDAClass();
		}
		#endregion

		#region Public methods
		/// <summary>
		/// Retrieves all configured Excise Taxes
		/// </summary>
		/// <returns>A collection of Excise Taxes</returns>
		public ExciseTaxDOCollection GetAll(SecurityClass security)
		{
			DataSet dataSet = null;

			try
			{
				ExciseTaxDO exciseDO = new ExciseTaxDO();
				using (SqlCommand cmd = new SqlCommand())
				{
					exciseDO.SelectExciseTaxes(cmd);

					dataSet = this.consolidatedDA.GetDataSet(cmd, security);
				}
			}
			catch (Exception ex)
			{
				throw new Exception("An error occurred attempting to retrieve Excise Taxes from the database.  " + ex.Message);
			}

			ExciseTaxDOCollection exciseTaxes = new ExciseTaxDOCollection();

			if ((dataSet != null) && (dataSet.Tables != null) && (dataSet.Tables.Count > 0))
			{
				DataTable dataTable = dataSet.Tables[0];

				if (dataTable.Rows != null)
				{
					foreach (DataRow dataRow in dataTable.Rows)
					{
						ExciseTaxDO excise = new ExciseTaxDO();
						excise.Populate(dataRow);
						exciseTaxes.Add(excise);
					}
				}
			}

			return exciseTaxes;
		}

		/// <summary>
		/// Returns configured Excise Taxes for the passed product and excise code.
		/// </summary>
		/// <param name="productId">A product ID</param>
		/// <param name="exciseCode">An excise code</param>
		/// <returns>A collection of Excise Taxes</returns>
		public ExciseTaxDOCollection GetForProductAndCode(string productId, string exciseCode, SecurityClass security)
		{
			DataSet dataSet = null;

			try
			{
				ExciseTaxDO exciseDO = new ExciseTaxDO();
				using (SqlCommand cmd = new SqlCommand())
				{
					exciseDO.SelectForProductAndCode(cmd, productId, exciseCode);

					dataSet = this.consolidatedDA.GetDataSet(cmd, security);
				}
			}
			catch (Exception ex)
			{
				throw new Exception("An error occurred attempting to retrieve Excise Taxes from the database.  " + ex.Message);
			}

			ExciseTaxDOCollection exciseTaxes = new ExciseTaxDOCollection();

			if ((dataSet != null) && (dataSet.Tables != null) && (dataSet.Tables.Count > 0))
			{
				DataTable dataTable = dataSet.Tables[0];

				if (dataTable.Rows != null)
				{
					foreach (DataRow dataRow in dataTable.Rows)
					{
						ExciseTaxDO excise = new ExciseTaxDO();
						excise.Populate(dataRow);
						exciseTaxes.Add(excise);
					}
				}
			}

			return exciseTaxes;
		}

		/// <summary>
		/// Returns configured Excise Taxes for the passed product ID
		/// </summary>
		/// <param name="productId">A product ID</param>
		/// <returns>A collection of Excise Taxes</returns>
		public ExciseTaxDOCollection GetForProduct(string productId, SecurityClass security)
		{
			DataSet dataSet = null;

			try
			{
				ExciseTaxDO exciseDO = new ExciseTaxDO();
				using (SqlCommand cmd = new SqlCommand())
				{
					exciseDO.SelectForProduct(cmd, productId);

					dataSet = this.consolidatedDA.GetDataSet(cmd, security);
				}
			}
			catch (Exception ex)
			{
				throw new Exception("An error occurred attempting to retrieve Excise Taxes from the database.  " + ex.Message);
			}

			ExciseTaxDOCollection exciseTaxes = new ExciseTaxDOCollection();

			if ((dataSet != null) && (dataSet.Tables != null) && (dataSet.Tables.Count > 0))
			{
				DataTable dataTable = dataSet.Tables[0];

				if (dataTable.Rows != null)
				{
					foreach (DataRow dataRow in dataTable.Rows)
					{
						ExciseTaxDO excise = new ExciseTaxDO();
						excise.Populate(dataRow);
						exciseTaxes.Add(excise);
					}
				}
			}

			return exciseTaxes;
		}

		/// <summary>
		/// Retrieves the configured Excise's associated with the inventory date and product ID supplied.
		/// </summary>
		/// <returns>A Excise Data Object</returns>
		public ExciseTaxDO GetForProductAndDate(Guid productGuid, DateTimeOffset dtDate, SecurityClass security)
		{
			DataSet dataSet = null;
			ExciseTaxDO excise = null;

			try
			{
				ExciseTaxDO exciseDO = new ExciseTaxDO();
				using (SqlCommand cmd = new SqlCommand())
				{
					exciseDO.SelectForProductAndDate(cmd, productGuid, dtDate);

					dataSet = this.consolidatedDA.GetDataSet(cmd, security);
				}
			}
			catch (Exception ex)
			{
				throw new Exception("An error occurred attempting to retrieve excise tax record from the database.  " + ex.Message);
			}

			if ((dataSet != null) && (dataSet.Tables != null) && (dataSet.Tables.Count > 0))
			{
				DataTable dataTable = dataSet.Tables[0];

				if ((dataTable.Rows != null) && (dataTable.Rows.Count > 0))
				{
					excise = new ExciseTaxDO();
					excise.Populate(dataTable.Rows[0]);
				}
			}

			return excise;
		}

		/// <summary>
		/// This method will return an Excise Tax data object based on the product, company, and date.
		/// </summary>
		/// <param name="productGuid"></param>
		/// <param name="dtDate"></param>
		/// <param name="companyGuid"></param>
		/// <param name="security"></param>
		/// <returns></returns>
		public ExciseTaxDO GetForProductCompanyAndDate(Guid productGuid, DateTimeOffset dtDate, Guid companyGuid, SecurityClass security)
		{
			DataSet dataSet = null;
			ExciseTaxDO excise = null;

			try
			{
				ExciseTaxDO exciseDO = new ExciseTaxDO();
				using (SqlCommand cmd = new SqlCommand())
				{
					exciseDO.SelectForProductCompanyAndDate(cmd, productGuid, dtDate, companyGuid);

					dataSet = this.consolidatedDA.GetDataSet(cmd, security);
				}
			}
			catch (Exception ex)
			{
				throw new Exception("An error occurred attempting to retrieve excise tax record from the database.  " + ex.Message);
			}

			if ((dataSet != null) && (dataSet.Tables != null) && (dataSet.Tables.Count > 0))
			{
				DataTable dataTable = dataSet.Tables[0];

				if ((dataTable.Rows != null) && (dataTable.Rows.Count > 0))
				{
					excise = new ExciseTaxDO();
					excise.Populate(dataTable.Rows[0]);
				}
			}

			return excise;
		}

		/// <summary>
		/// Returns configured Excise Taxes for the passed product ID and excise code.
		/// </summary>
		/// <param name="productId">A product ID</param>
		/// <param name="dtStart">Begin Date of Search</param>
		/// <param name="dtEnd">End Date of the Search</param>
		/// <param name="security">Security context object</param>
		/// <returns>A collection of Excise Taxes</returns>
		public ExciseTaxDOCollection GetForProductAndDateRange(string productId, DateTimeOffset dtStart, DateTimeOffset dtEnd, SecurityClass security)
		{
			DataSet dataSet = null;

			try
			{
				ExciseTaxDO exciseDO = new ExciseTaxDO();
				using (SqlCommand cmd = new SqlCommand())
				{
					exciseDO.SelectForProductAndDateRange(cmd, productId, dtStart, dtEnd);

					dataSet = this.consolidatedDA.GetDataSet(cmd, security);
				}
			}
			catch (Exception ex)
			{
				throw new Exception("An error occurred attempting to retrieve Excise Taxes from the database.  " + ex.Message);
			}

			ExciseTaxDOCollection exciseTaxes = new ExciseTaxDOCollection();

			if ((dataSet != null) && (dataSet.Tables != null) && (dataSet.Tables.Count > 0))
			{
				DataTable dataTable = dataSet.Tables[0];

				if (dataTable.Rows != null)
				{
					foreach (DataRow dataRow in dataTable.Rows)
					{
						ExciseTaxDO excise = new ExciseTaxDO();
						excise.Populate(dataRow);
						exciseTaxes.Add(excise);
					}
				}
			}

			return exciseTaxes;
		}

		/// <summary>
		/// Returns a list of Excise Codes
		/// </summary>
		/// <returns>A list of Excise Codes</returns>
		public DataTable GetExciseCodes(SecurityClass security)
		{
			DataSet dataSet = null;
			DataTable dataTable = null;

			try
			{
				ExciseTaxDO exciseDO = new ExciseTaxDO();
				using (SqlCommand cmd = new SqlCommand())
				{
					exciseDO.SelectExciseCodes(cmd);

					dataSet = this.consolidatedDA.GetDataSet(cmd, security);
				}
			}
			catch (Exception ex)
			{
				throw new Exception("An error occurred retrieving Excise Codes.  " + ex.Message);
			}

			if ((dataSet != null) && (dataSet.Tables != null) && (dataSet.Tables.Count > 0))
			{
				dataTable = dataSet.Tables[0];
			}

			return dataTable;
		}

		/// <summary>
		/// Retrieves the companies assigned to a Excise
		/// </summary>
		/// <param name="gstDO"></param>
		/// <param name="security"></param>
		/// <returns></returns>
		public List<TaxCompanyMapDO> GetExciseCompanies(ExciseTaxDO exciseDO, SecurityClass security)
		{
			List<TaxCompanyMapDO> companyMapList = new List<TaxCompanyMapDO>();
			TaxCompanyMapDO companyMap = null;

			try
			{
				TaxCompanyMapClass mapClass = new TaxCompanyMapClass();
				DataTable dataTable = mapClass.SelectExciseTaxMapCompanies(exciseDO, security);

				foreach (DataRow row in dataTable.Rows)
				{
					companyMap = new TaxCompanyMapDO();
					companyMap.CompanyGuid = DataObject.getValue<Guid>(row["CompanyGuid"], Guid.Empty);
					companyMap.CompanyID = DataObject.getValue<string>(row["ID"], "");

					companyMapList.Add(companyMap);
				}
			}
			catch (Exception ex)
			{
				throw new Exception("An error occurred attempting to retrieve Excise Companies from the database.  " + ex.Message);
			}

			companyMapList.Sort
			(
				delegate(TaxCompanyMapDO class1, TaxCompanyMapDO class2)
				{
					return (Comparer<string>.Default.Compare(class1.CompanyID, class2.CompanyID));
				}
			);

			return companyMapList;
		}

		/// <summary>
		/// Removes an Excise Tax from the database
		/// </summary>
		/// <param name="excise">The Excise Tax to remove</param>
		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void Remove(ExciseTaxDO excise, SecurityClass security)
		{
			try
			{
				ExciseTaxDO exciseDO = new ExciseTaxDO();
				using (SqlCommand cmd = new SqlCommand())
				{
					exciseDO.Delete(cmd, excise);

					this.consolidatedDA.ExecuteQuery(security, cmd);
				}
			}
			catch (Exception ex)
			{
				throw new Exception("An error occurred attempting to remove an Excise Tax.  " + ex.Message);
			}
		}

		/// <summary>
		/// Adds a new Excise Tax to the database with associated companies.
		/// </summary>
		/// <param name="excise">The Excise Tax to add</param>
		/// <param name="security">Contains security credentials</param>
		/// <returns>The Guid of the newly added Excise Tax</returns>
		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public Guid Add(ExciseTaxDO excise, SecurityClass security, List<TaxCompanyMapDO> companyList)
		{
			Guid exciseGuid = Guid.Empty;

			if (this.ExciseAndCompanyExists(excise, security, companyList) == true)
			{
				SitesClass sites = new SitesClass();
				SiteClass site = sites.GetByMemberAndProcessVariables(security, security.SiteGuid, false, false);
				string strDate = (site == null) ? excise.ExciseDate.ToString("d") : excise.ExciseDate.ToString(site.ShortDatePattern);

				throw new FMBusinessObjects.Exceptions.FinanceObjectExistsException(
														"An Excise Tax with Date: " + strDate + ", Product: " + excise.Product +
														", and an associated company combination already exists.");
			}

			try
			{
				TaxCompanyMapClass taxCompanyMap = new TaxCompanyMapClass();
				ExciseTaxDO exciseDO = new ExciseTaxDO();

				// Insert a Excise information.
				using (SqlCommand cmd = new SqlCommand())
				{
					exciseDO.IdentityGuid = Guid.NewGuid();
					excise.IdentityGuid = exciseDO.IdentityGuid;
					exciseDO.Insert(cmd, excise, security.UserID);
					this.consolidatedDA.ExecuteQuery(security, cmd);
					exciseGuid = exciseDO.IdentityGuid;

					// Insert associated excise tax companies.
					if ((companyList != null) && (companyList.Count > 0) && (exciseGuid != Guid.Empty))
					{
						taxCompanyMap.InsertExciseAssociatedCompanies(security, companyList, exciseGuid);
					}
				}
			}
			catch (FMBusinessObjects.Exceptions.FinanceObjectExistsException financeEx)
			{
				// Let the UI handle this exception
				throw new Exception(financeEx.Message);
			}
			catch (Exception ex)
			{
				throw new Exception("An error occurred attempting to add an Excise Tax.  " + ex.Message);
			}

			return exciseGuid;
		}

		/// <summary>
		/// Saves an Excise Tax with associated companies.
		/// </summary>
		/// <param name="excise">The Excise Tax to save</param>
		/// <param name="security">Contains security credentials</param>
		/// <param name="companyList">A list of associated companies to add</param>
		/// <param name="deletedCompanyList">A list of associated companies to delete</param>
		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void Save(ExciseTaxDO excise, SecurityClass security, List<TaxCompanyMapDO> companyList, List<TaxCompanyMapDO> deletedCompanyList)
		{
			// Modify the Excise Date to the the site setting.
			SitesClass sites = new SitesClass();
			SiteClass site = sites.GetByMemberAndProcessVariables(security, security.SiteGuid, false, false);
			string strDate = (site == null) ? excise.ExciseDate.ToString("d") : excise.ExciseDate.ToString(site.ShortDatePattern);

			// Throw exception if the Excise Tax with date and product combination exists.
			if (this.Exists(security, excise) == false)
			{
				throw new FMBusinessObjects.Exceptions.FinanceObjectDoesNotExistException("An Excise Tax with Date: " + strDate +
																							" and Product: " + excise.Product +
																							" combination  does not exist.");
			}
			// Throw exception if the Excise Tax with date and product combination contains
			// a company association that matches another excise tax item.
			else if (this.ExciseAndCompanyExists(excise, security, companyList) == true)
			{
				throw new FMBusinessObjects.Exceptions.FinanceObjectExistsException("An Excise Tax with Date: " + strDate +
																	" and Product: " + excise.Product +
																	" contains a company association that matches another excise tax item.");
			}

			try
			{
				ExciseTaxDO exciseDO = new ExciseTaxDO();
				TaxCompanyMapClass taxCompanyMap = new TaxCompanyMapClass();

				// Update the Excise tax information
				using (SqlCommand cmd = new SqlCommand())
				{
					exciseDO.Update(cmd, excise, security.UserID);
					this.consolidatedDA.ExecuteQuery(security, cmd);

					// Add any new company association
					if ((companyList != null) && (companyList.Count > 0))
					{
						taxCompanyMap.InsertExciseAssociatedCompanies(security, companyList, excise.IdentityGuid);
					}

					// Remove any newly deleted company association
					if ((deletedCompanyList != null) && (deletedCompanyList.Count > 0))
					{
						taxCompanyMap.DeleteExciseAssociatedCompanies(security, deletedCompanyList, excise.IdentityGuid);
					}
				}
			}
			catch (FMBusinessObjects.Exceptions.FinanceObjectDoesNotExistException)
			{
				// Let the UI handle this exception
				throw;
			}
			catch (FMBusinessObjects.Exceptions.FinanceObjectExistsException financeEx)
			{
				// Let the UI handle this exception
				throw new Exception(financeEx.Message);
			}
			catch (Exception ex)
			{
				throw new Exception("An error occurred attempting to save the Excise Tax.  " + ex.Message);
			}
		}
		#endregion

		#region Private methods
		/// <summary>
		/// Checks to see if an Excise Tax exists in the database
		/// </summary>
		/// <param name="excise">The Excise Tax to search for</param>
		/// <returns>True if it exists.  False if it doesn't</returns>
		/// <remarks>This will search on Product Guid and Excise Code</remarks>
		private bool Exists(SecurityClass security, ExciseTaxDO excise)
		{
			bool exists = false;

			try
			{
				ExciseTaxDO exciseDO = new ExciseTaxDO();
				using (SqlCommand cmd = new SqlCommand())
				{
					exciseDO.Exists(cmd, excise, ContextUtil.IsInTransaction);
					DataSet dataSet = consolidatedDA.GetDataSet(cmd, security);

					if ((dataSet != null) && (dataSet.Tables != null) && (dataSet.Tables.Count > 0))
					{
						DataTable dataTable = dataSet.Tables[0];

						if ((dataTable.Rows != null) && (dataTable.Rows.Count > 0))
						{
							exists = true;
						}
					}
				}
			}
			catch (Exception ex)
			{
				throw new Exception("Error determining Excise existence.  " + ex.Message);
			}

			return exists;
		}

		/// <summary>
		/// This method will return true if the Excise / Company combination exists.
		/// Otherwise, it returns false.
		/// </summary>
		/// <param name="excise"></param>
		/// <param name="security"></param>
		/// <param name="companyList"></param>
		/// <returns></returns>
		private bool ExciseAndCompanyExists(ExciseTaxDO excise, SecurityClass security, List<TaxCompanyMapDO> companyList)
		{
			bool exists = false;

			try
			{
				ExciseTaxDO exciseDO = new ExciseTaxDO();
				using (SqlCommand cmd = new SqlCommand())
				{
					exciseDO.ExciseAndCompanyExists(cmd, excise, companyList, ContextUtil.IsInTransaction);
					DataSet dataSet = this.consolidatedDA.GetDataSet(cmd, security);

					if ((dataSet != null) && (dataSet.Tables != null) && (dataSet.Tables.Count > 0))
					{
						DataTable dataTable = dataSet.Tables[0];

						if ((dataTable.Rows != null) && (dataTable.Rows.Count > 0))
						{
							exists = true;
						}
					}
				}
			}
			catch (Exception ex)
			{
				throw new Exception("Error determining if Excise/Company combination exists.  " + ex.Message);
			}

			return exists;
		}
		#endregion
	}
}
