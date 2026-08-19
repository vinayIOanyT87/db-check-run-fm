/// <summary>
/// File name:	ReportListProcessor.cs
/// Purpose:	Handles the report list service request to retrieve and apply business
///				logic to the data. It will return a data set object that contains
///				a view of the data.
///	Comments:	Copyright (C) E+H Systems & Gauging, Inc. Norcross, GA, USA, 
///				2000.  This file shall not be copied or reproduced in any form 
///				without the express written consent of Endress+Hauser.
///	Author(s):	Richard R. Panachida
///	Version:	1.0.0  Current version
///	
///	Modification History:
///		Date:			By:				Reason:
///		----------	-------------	-------------------------------------------
///		11-Nov-05	I.Orndorff		7.0.0.1 - Modified "RetrieveCompanyData() to add
///														 "All" when "reportListSR.HasAllItem" is
///														 set to true.
///		
/// </summary>
/// 
using System;
using System.Data;
using System.Collections;
using ReportingServices;
using ConsolidatedBLL;
using ConsolidatedDataObjects;
using FM7Accounting;
using LogClient;
using FMCommon;

namespace ReportingBLL
{
	/// <summary>
	/// Summary description for ReportListProcessor.
	/// </summary>
	public class ReportListProcessor : ReportRequestProcessor
	{
		#region Attributes
		private string       requestCommand;
		private ReportListSR reportListSR;
		private const int    EMPTY_STRING = 0;
		private Logger       logger;
		#endregion

		#region Constructors
		/// <summary>
		/// This is the default constructor for the report processor class.
		/// It must initialize the reporting service implemention class in the base class
		/// and set the request command with the service request string.
		/// </summary>
		/// <param name="reportingServiceImpl"></param>
		public ReportListProcessor(ReportServiceImpl reportingServerImpl) : base (reportingServerImpl)
		{
			this.requestCommand = typeof(ReportListSR).ToString();
		}
		#endregion

		#region Public Methods
		/// <summary>
		/// This method implements the base class get command method.  It will
		/// return the list service request command (class name string).
		/// This is used during the registrations of the processors in the accounting
		/// service object.
		/// </summary>
		/// <returns></returns>
		override public string GetCommand()
		{
			return requestCommand;
		}

		/// <summary>
		/// This method starts the processing of gathering all the data for the list
		/// base reports.
		/// </summary>
		/// <param name="request"></param>
		/// <returns></returns>
		override public System.Data.DataSet Process(ReportServiceRequest request)
		{
			DataSet dataSet = null;
			this.reportListSR = (ReportListSR) request;

			logger = new Logger("ReportingBLL");
			logger.Debug("Entered Process method.");

			// Create the security object that will be used to access the appropriate
			// data.  The request object should have the token, site index, and
			// whether or not to use the data dictionary.
			base.reportSecurity = new ReportSecurity(this.reportListSR.SecurityToken, 
												     this.reportListSR.CurrentSiteIndex, 
													 this.reportListSR.UseDataDictionary);

			switch (this.reportListSR.SubReportType)
			{
				case ReportListSR.SubReportTypes.PRODUCT_LIST:
					logger.Debug("Report subtype selected: Product List");
					dataSet = this.RetrieveProductData();
					break;

				case ReportListSR.SubReportTypes.BILLTO_LIST:
					logger.Debug("Report subtype selected: Product List");
					dataSet = this.RetrieveCompanyData(ConsolidatedDataObjects.COMPANY_ROLE.CUSTOMER_BILLTO);
					break;

				case ReportListSR.SubReportTypes.MANAGER_LIST:
					logger.Debug("Report subtype selected: Manager List");
					dataSet = this.RetrieveCompanyData(ConsolidatedDataObjects.COMPANY_ROLE.MANAGER);
					break;

				case ReportListSR.SubReportTypes.OWNER_LIST:
					logger.Debug("Report subtype selected: Owner List");
					dataSet = this.RetrieveCompanyData(ConsolidatedDataObjects.COMPANY_ROLE.OWNER);
					break;

				case ReportListSR.SubReportTypes.SHIPPER_LIST:
					logger.Debug("Report subtype selected: Shipper List");
					dataSet = this.RetrieveCompanyData(ConsolidatedDataObjects.COMPANY_ROLE.SHIPPER);
					break;

				case ReportListSR.SubReportTypes.SHIPTO_LIST:
					logger.Debug("Report subtype selected: ShipTo List");
					dataSet = this.RetrieveCompanyData(ConsolidatedDataObjects.COMPANY_ROLE.CUSTOMER_SHIPTO);
					break;

				case ReportListSR.SubReportTypes.SUPPLIER_LIST:
					logger.Debug("Report subtype selected: Supplier List");
					dataSet = this.RetrieveCompanyData(ConsolidatedDataObjects.COMPANY_ROLE.CARRIER); //NOTE: must change!!
					break;

				case ReportListSR.SubReportTypes.CARRIER_LIST:
					logger.Debug("Report subtype selected: Carrier List");
					dataSet = this.RetrieveCompanyData(ConsolidatedDataObjects.COMPANY_ROLE.CARRIER);
					break;

				case ReportListSR.SubReportTypes.MONTH_YEAR_LIST:
					logger.Debug("Report subtype selected: Month/Year List");
					dataSet = this.RetrieveMonthYearData(base.reportSecurity.Security);
					break;
			}
			
			logger.Debug("Finished processing, returing dataset.");
			return dataSet;
		}
		#endregion

		#region Private Load/Create Month/Year data
		/// <summary>
		/// This method will gather all the month/year data for the range of transactions in the 
		/// database. It will return a dataset of that data.
		/// </summary>
		/// <returns></returns>
		private System.Data.DataSet RetrieveMonthYearData(SecurityClass security)
		{
			// Create a new dataset and table to store the month/year data.
			DataSet   dataSet   = new DataSet();
			DataTable dataTable = new DataTable("MonthYear");
			dataTable.Columns.Add("MonthYearID", typeof(string));

			// Retrieve from accounting the list of month/year data for the range of dates
			// that the transactions incompass.
			AccountingService accountingService = new AccountingService(new AccountingServiceImpl());
            MonthYearSR monthYearSR = new MonthYearSR();
            monthYearSR.Security = security;
			MonthYearDO monthYearDO = (MonthYearDO) accountingService.request(new MonthYearSR());
			ArrayList monthList = monthYearDO.MonthList;
			ArrayList yearList = monthYearDO.YearList;

			if ((monthList == null) || (monthList.Count == 0) || (yearList == null) || (yearList.Count == 0))
			{
				DataRow dataRow = dataTable.NewRow();
				dataRow[0] = "January 2005";
				dataTable.Rows.Add(dataRow);
			}
			else
			{
				// Loop through all the month/year data and create a row for each in the
				// dataset.
				for (int nextDate = 0; nextDate < monthList.Count; nextDate++)
				{
					DataRow dataRow = dataTable.NewRow();
					dataRow[0] = monthList[nextDate] + " " + yearList[nextDate];
					dataTable.Rows.Add(dataRow);
				}
			}

			// Add the dataset to the table and return the dataset.
			dataSet.Tables.Add(dataTable);
			return dataSet;
		}
		#endregion

		#region Private load/create company data
		/// <summary>
		/// This method will gather all the company data for a given role and return
		/// a data set.  The data set will be empty if nothing was retrieved.
		/// </summary>
		/// <returns></returns>
		private DataSet RetrieveCompanyData(ConsolidatedDataObjects.COMPANY_ROLE role)
		{
			bool filterByAssociatedCompanies = false;
			CompanyCollectionClass companyCollection;
			DataSet   dataSet   = new DataSet();
			DataTable dataTable = new DataTable("Companies");
			this.CreateCompanyTableColumns(dataTable);

			// Retrieve company list from shared components for the specified role.
			CompaniesClass companies = new CompaniesClass();
			companyCollection = (CompanyCollectionClass) companies.EnumerateByRole(base.reportSecurity.Security, 
																				   role, filterByAssociatedCompanies);
			
			// If HasAllItem set in the ServiceRequset all to the data set returned.
			if( this.reportListSR.HasAllItem )
			{
				DataRow dataRow = dataTable.NewRow();
				dataRow[0] = "<All>";
				dataRow[1] = "<All>";
				dataTable.Rows.Add(dataRow);
			}

			foreach (CompanyClass company in companyCollection)
			{
				this.LoadCompanyDataSet(company, dataTable);
			}

			dataSet.Tables.Add(dataTable);
			return dataSet;
		}

		/// <summary>
		/// This method will load the company data into one row of the given table.
		/// </summary>
		/// <param name="company"></param>
		/// <param name="dataTable"></param>
		private void LoadCompanyDataSet(CompanyClass company, DataTable dataTable)
		{
			DataRow dataRow = dataTable.NewRow();

			dataRow[0] = company.ID;
			dataRow[1] = company.Code;

			dataTable.Rows.Add(dataRow);
		}

		/// <summary>
		/// This method will create company data columns for the different company roles.
		/// </summary>
		/// <param name="inDataTable"></param>
		private void CreateCompanyTableColumns(DataTable inDataTable)
		{
			logger.Debug("Creating columns for company roles.");
			inDataTable.Columns.Add("CompanyName",   typeof(string));
			inDataTable.Columns.Add("CompanyCode",   typeof(string));
		}
		#endregion

		#region Private Product Data Methods
		/// <summary>
		/// This method will gather all the product data and returns
		/// a data set.  The data set will be empty if nothing was 
		/// retrieved.
		/// </summary>
		/// <returns></returns>
		private DataSet RetrieveProductData()
		{
			DataSet   dataSet   = new DataSet();
			DataTable dataTable = new DataTable("Products");
			this.CreateProductTableColumns(dataTable);

			// Retrieve product list from shared components
			ProductsClass products = new ProductsClass();
			ProductCollectionClass productCollection = (ProductCollectionClass) 
															products.Enumerate(base.reportSecurity.Security);

			foreach (ProductClass product in productCollection)
			{
				this.LoadProductIntoDataSet(product, dataTable);
			}

			dataSet.Tables.Add(dataTable);
			return dataSet;
		}

		/// <summary>
		/// This method will load the product data into one row of the given table.
		/// </summary>
		/// <param name="product"></param>
		/// <param name="dataTable"></param>
		private void LoadProductIntoDataSet(ProductClass product, DataTable dataTable)
		{
			DataRow dataRow = dataTable.NewRow();

			dataRow[0] = product.Description;
			dataRow[1] = product.Code;
			dataRow[2] = product.ID;

			dataTable.Rows.Add(dataRow);
		}

		/// <summary>
		/// This method will create all the columns in the product table.
		/// </summary>
		/// <param name="inDataTable"></param>
		private void CreateProductTableColumns(DataTable inDataTable)
		{
			logger.Debug("Creating columns for product tables.");
			inDataTable.Columns.Add("Description", typeof(string));
			inDataTable.Columns.Add("Code",        typeof(string));
			inDataTable.Columns.Add("ProductID",   typeof(string));
		}
		#endregion
	}
}
