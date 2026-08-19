// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TaxCompanyMapClass.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   ENTER FILE SUMMARY HERE
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace FMBusinessServices.InternalClasses
{
	using System;
	using System.Collections.Generic;
	using System.Data;
	using System.Data.SqlClient;

	using FMBusinessObjects.DataObjects;

	using FMBusinessServices.DataAccessLayer;

	public class TaxCompanyMapClass
	{
		#region Constants and Fields

		private readonly ConsolidatedDAClass consolidatedDA;

		#endregion

		#region Constructors and Destructors

		/// <summary>
		/// Initializes a new instance of the <see cref="TaxCompanyMapClass"/> class. 
		///     This is the default constructor for the Tax Company Map Class.
		/// </summary>
		public TaxCompanyMapClass()
		{
			this.consolidatedDA = new ConsolidatedDAClass();
		}

		#endregion

		#region Public Methods and Operators

		/// <summary>
		/// This method will delete all associated companies to either the tblGSTCompanyMap,
		///     tblExciseCompanyMap, or tblMarkupCompanyMap tables.
		/// </summary>
		/// <param name="security">
		/// </param>
		/// <param name="taxGuid">
		/// </param>
		/// <param name="mapType">
		/// </param>
		public void DeleteAllAssociatedCompanies(SecurityClass security, Guid taxGuid, TaxCompanyMapDO.TaxMapTypes mapType)
		{
			try
			{
				var taxCompanyMapDO = new TaxCompanyMapDO();
				using (var cmd = new SqlCommand())
				{
					taxCompanyMapDO.DeleteAllAssociatedCompaniesSQL(cmd, taxGuid, mapType);
					this.consolidatedDA.ExecuteQuery(security, cmd);
				}
			}
			catch (Exception ex)
			{
				throw new Exception("Error deleting all associated companies.  " + ex.Message);
			}
		}

		/// <summary>
		/// This method will delete associated companies to the tblExciseCompanyMap table.
		/// </summary>
		/// <param name="security">
		/// </param>
		/// <param name="deletedCompanyList">
		/// </param>
		/// <param name="exciseGuid">
		/// </param>
		public void DeleteExciseAssociatedCompanies(
			SecurityClass security, List<TaxCompanyMapDO> deletedCompanyList, Guid exciseGuid)
		{
			// Do nothing if the deleted company list is empty or the Excise index is zero or less.
			if ((deletedCompanyList == null) || (deletedCompanyList.Count <= 0) || (exciseGuid == Guid.Empty))
			{
				return;
			}

			try
			{
				var taxCompanyMapDO = new TaxCompanyMapDO();

				foreach (TaxCompanyMapDO mappedDO in deletedCompanyList)
				{
					using (var cmd = new SqlCommand())
					{
						taxCompanyMapDO.DeleteExciseAssociatedCompaniesSQL(cmd, mappedDO.CompanyGuid, exciseGuid);
						this.consolidatedDA.ExecuteQuery(security, cmd);
					}
				}
			}
			catch (Exception ex)
			{
				throw new Exception("Error deleting Excise associated companies  " + ex.Message);
			}
		}

		/// <summary>
		/// This method will delete associated companies to the tblGSTCompanyMap table.
		/// </summary>
		/// <param name="security">
		/// </param>
		/// <param name="deletedCompanyList">
		/// </param>
		/// <param name="gstGuid">
		/// </param>
		public void DeleteGSTAssociatedCompanies(
			SecurityClass security, List<TaxCompanyMapDO> deletedCompanyList, Guid gstGuid)
		{
			// Do nothing if the deleted company list is empty or the GST index is zero or less.
			if ((deletedCompanyList == null) || (deletedCompanyList.Count <= 0) || (gstGuid == Guid.Empty))
			{
				return;
			}

			try
			{
				var taxCompanyMapDO = new TaxCompanyMapDO();

				foreach (TaxCompanyMapDO mappedDO in deletedCompanyList)
				{
					using (var cmd = new SqlCommand())
					{
						taxCompanyMapDO.DeleteGSTAssociatedCompaniesSQL(cmd, mappedDO.CompanyGuid, gstGuid);
						this.consolidatedDA.ExecuteQuery(security, cmd);
					}
				}
			}
			catch (Exception ex)
			{
				throw new Exception("Error deleting GST associated companies  " + ex.Message);
			}
		}

		/// <summary>
		/// This method will delete associated companies to the tblMarkupCompanyMap table.
		/// </summary>
		/// <param name="security">
		/// </param>
		/// <param name="deletedCompanyList">
		/// </param>
		/// <param name="markupGuid">
		/// </param>
		public void DeleteMarkupAssociatedCompanies(
			SecurityClass security, List<TaxCompanyMapDO> deletedCompanyList, Guid markupGuid)
		{
			// Do nothing if the deleted company list is empty or the Markup index is zero or less.
			if ((deletedCompanyList == null) || (deletedCompanyList.Count <= 0) || (markupGuid == Guid.Empty))
			{
				return;
			}

			try
			{
				var taxCompanyMapDO = new TaxCompanyMapDO();

				foreach (TaxCompanyMapDO mappedDO in deletedCompanyList)
				{
					using (var cmd = new SqlCommand())
					{
						taxCompanyMapDO.DeleteMarkupAssociatedCompaniesSQL(cmd, mappedDO.CompanyGuid, markupGuid);
						this.consolidatedDA.ExecuteQuery(security, cmd);
					}
				}
			}
			catch (Exception ex)
			{
				throw new Exception("Error deleting Markup associated companies  " + ex.Message);
			}
		}

		/// <summary>
		/// This method will return a list of companies that have already been mapped to a
		///     GST entry. If none have been mapped, then an empty list is returned.
		///     Changed to look at the configured date and return the companies that are configured for the same
		/// </summary>
		/// <param name="security">
		/// </param>
		/// <param name="gst">
		/// </param>
		/// <param name="companyList">
		/// </param>
		public List<string> GSTOrganizationAlreadyMapped(
			SecurityClass security, GoodsAndServicesTaxDO gst, List<TaxCompanyMapDO> companyList)
		{
			var companyMappingExists = new List<string>();

			// Do nothing if the company list is empty or the GST index is zero or less.
			if ((companyList == null) || (companyList.Count <= 0))
			{
				return companyMappingExists;
			}

			try
			{
				foreach (TaxCompanyMapDO companyMapDO in companyList)
				{
					var taxCompanyMapDO = new TaxCompanyMapDO();
					using (var cmd = new SqlCommand())
					{
						taxCompanyMapDO.GSTOrganizationAlreadyMappedSQL(cmd, gst, companyMapDO.CompanyGuid);
						DataSet dataSet = this.consolidatedDA.GetDataSet(cmd, security);

						if ((dataSet != null) && (dataSet.Tables.Count > 0))
						{
							DataTable dataTable = dataSet.Tables[0];

							if ((dataTable.Rows != null) && (dataTable.Rows.Count > 0))
							{
								companyMappingExists.Add(companyMapDO.CompanyID);
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				throw new Exception("Error retrieving Good & Service tax information.  " + ex.Message);
			}

			return companyMappingExists;
		}

		/// <summary>
		/// This method will insert associated companies to the tblExciseCompanyMap table.
		/// </summary>
		/// <param name="security">
		/// </param>
		/// <param name="companyList">
		/// </param>
		/// <param name="exciseGuid">
		/// </param>
		public void InsertExciseAssociatedCompanies(
			SecurityClass security, List<TaxCompanyMapDO> companyList, Guid exciseGuid)
		{
			// Do nothing if the company list is empty or the Excise index is zero or less.
			if ((companyList == null) || (companyList.Count <= 0) || (exciseGuid == Guid.Empty))
			{
				return;
			}

			try
			{
				var taxCompanyMapDO = new TaxCompanyMapDO();

				foreach (TaxCompanyMapDO mappedDO in companyList)
				{
					using (var cmd = new SqlCommand())
					{
						taxCompanyMapDO.InsertExciseAssociatedCompaniesSQL(cmd, security.UserID, mappedDO.CompanyGuid, exciseGuid);
						this.consolidatedDA.ExecuteQuery(security, cmd);
					}
				}
			}
			catch (Exception ex)
			{
				throw new Exception("Error inserting Excise associated companies.  " + ex.Message);
			}
		}

		/// <summary>
		/// This method will insert associated companies to the tblGSTCompanyMap table.
		/// </summary>
		/// <param name="security">
		/// </param>
		/// <param name="companyList">
		/// </param>
		/// <param name="gstGuid">
		/// </param>
		public void InsertGSTAssociatedCompanies(SecurityClass security, List<TaxCompanyMapDO> companyList, Guid gstGuid)
		{
			// Do nothing if the company list is empty or the GST index is zero or less.
			if ((companyList == null) || (companyList.Count <= 0) || (gstGuid == Guid.Empty))
			{
				return;
			}

			try
			{
				var taxCompanyMapDO = new TaxCompanyMapDO();

				foreach (TaxCompanyMapDO mappedDO in companyList)
				{
					using (var cmd = new SqlCommand())
					{
						taxCompanyMapDO.InsertGSTAssociatedCompaniesSQL(cmd, mappedDO.CompanyGuid, gstGuid, security.UserID);
						this.consolidatedDA.ExecuteQuery(security, cmd);
					}
				}
			}
			catch (Exception ex)
			{
				throw new Exception("Error inserting GST associated companies.  " + ex.Message);
			}
		}

		/// <summary>
		/// This method will insert associated companies to the tblMarkupCompanyMap table.
		/// </summary>
		/// <param name="security">
		/// </param>
		/// <param name="companyList">
		/// </param>
		/// <param name="markupGuid">
		/// </param>
		public void InsertMarkupAssociatedCompanies(
			SecurityClass security, List<TaxCompanyMapDO> companyList, Guid markupGuid)
		{
			// Do nothing if the company list is empty or the Markup index is zero or less.
			if ((companyList == null) || (companyList.Count <= 0) || (markupGuid == Guid.Empty))
			{
				return;
			}

			try
			{
				var taxCompanyMapDO = new TaxCompanyMapDO();

				foreach (TaxCompanyMapDO mappedDO in companyList)
				{
					using (var cmd = new SqlCommand())
					{
						taxCompanyMapDO.InsertMarkupAssociatedCompaniesSQL(cmd, security.UserID, mappedDO.CompanyGuid, markupGuid);
						this.consolidatedDA.ExecuteQuery(security, cmd);
					}
				}
			}
			catch (Exception ex)
			{
				throw new Exception("Error inserting Markup associated companies.  " + ex.Message);
			}
		}

		/// <summary>
		/// Returns companies assigned to the passed Excise
		/// </summary>
		/// <param name="exciseDO">
		/// The excise DO.
		/// </param>
		/// <param name="security">
		/// </param>
		/// <returns>
		/// The <see cref="DataTable"/>.
		/// </returns>
		public DataTable SelectExciseTaxMapCompanies(ExciseTaxDO exciseDO, SecurityClass security)
		{
			DataTable dataTable = null;

			try
			{
				var taxCompanyMapDO = new TaxCompanyMapDO();
				using (var cmd = new SqlCommand())
				{
					taxCompanyMapDO.SelectExciseTaxMapCompaniesSQL(cmd, exciseDO);
					DataSet dataSet = this.consolidatedDA.GetDataSet(cmd, security);

					if ((dataSet != null) && (dataSet.Tables.Count > 0))
					{
						dataTable = dataSet.Tables[0];
					}
				}
			}
			catch (Exception ex)
			{
				throw new Exception("Error retrieving Excise mapped companies.  " + ex.Message);
			}

			return dataTable;
		}

		/// <summary>
		/// Returns companies assigned to the passed GST
		/// </summary>
		/// <param name="gstDO">
		/// The gst DO.
		/// </param>
		/// <param name="security">
		/// </param>
		/// <returns>
		/// The <see cref="DataTable"/>.
		/// </returns>
		public DataTable SelectGSTTaxMapCompanies(GoodsAndServicesTaxDO gstDO, SecurityClass security)
		{
			DataTable dataTable = null;

			try
			{
				var taxCompanyMapDO = new TaxCompanyMapDO();
				using (var cmd = new SqlCommand())
				{
					taxCompanyMapDO.SelectGSTTaxMapCompaniesSQL(cmd, gstDO);
					DataSet dataSet = this.consolidatedDA.GetDataSet(cmd, security);

					if ((dataSet != null) && (dataSet.Tables.Count > 0))
					{
						dataTable = dataSet.Tables[0];
					}
				}
			}
			catch (Exception ex)
			{
				throw new Exception("Error retrieving GST mapped companies.  " + ex.Message);
			}

			return dataTable;
		}

		/// <summary>
		/// Returns companies assigned to the passed Markup
		/// </summary>
		/// <param name="markupDO">
		/// The markup DO.
		/// </param>
		/// <param name="security">
		/// </param>
		/// <returns>
		/// The <see cref="DataTable"/>.
		/// </returns>
		public DataTable SelectMarkupTaxMapCompanies(MarkupDO markupDO, SecurityClass security)
		{
			DataTable dataTable = null;

			try
			{
				var taxCompanyMapDO = new TaxCompanyMapDO();
				using (var cmd = new SqlCommand())
				{
					taxCompanyMapDO.SelectMarkupTaxMapCompaniesSQL(cmd, markupDO);
					DataSet dataSet = this.consolidatedDA.GetDataSet(cmd, security);

					if ((dataSet != null) && (dataSet.Tables.Count > 0))
					{
						dataTable = dataSet.Tables[0];
					}
				}
			}
			catch (Exception ex)
			{
				throw new Exception("Error retrieving Markup mapped companies.  " + ex.Message);
			}

			return dataTable;
		}

		#endregion
	}
}