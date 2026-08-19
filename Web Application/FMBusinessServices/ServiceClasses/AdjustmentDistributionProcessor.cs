using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Collections;
using System.Data;
using System.Diagnostics;
using System.ServiceModel;
using FMBusinessObjects.LogClient;

using FMBusinessObjects.DataObjects;
using FMBusinessObjects.BusinessInterfaces;
using FMBusinessObjects.ServiceRequests;
using FMBusinessServices.DataAccessLayer;
using System.Data.SqlClient;

namespace FMBusinessServices.ServiceClasses
{
	public class AdjustmentDistributionProcessorClass : IAdjustmentDistributionProcessor
	{
		#region Private Attributes
		private AdjustmentDistributionSR adjustDistSR;
		private AdjustmentDistributionDO adjustDistDO;
		private const string USER_DATA_TYPE = "Alias";
		private ConsolidatedDAClass consolidatedDA;
		private Logger logger;
		#endregion

		#region Constructor
		/// <summary>
		/// This is the default constructor for the adjustment distribute processor class.
		/// It must initialize the accounting service implemention class in the base class
		/// and set the request command with the service request string.
		/// </summary>
		public AdjustmentDistributionProcessorClass ( )
		{
			this.consolidatedDA = new ConsolidatedDAClass ( );

			this.logger = new Logger ( "AdjustmentDistribution" );
		}
		#endregion

		#region Public Methods
		/// <summary>
		/// This method is an override method that all derived service request classes have
		/// to implement. It is the entry point for the adjustment distribution business
		/// logic layer.
		/// </summary>
		/// <param name="accountingSR"></param>
		/// <returns></returns>
		public AdjustmentDistributionDO Process ( AdjustmentDistributionSR inAdjustDistSR )
		{
			this.adjustDistSR = inAdjustDistSR;
			this.adjustDistDO = new AdjustmentDistributionDO ( );

			if (this.adjustDistSR != null)
			{
				switch (this.adjustDistSR.Subrequest)
				{
					case AdjustmentDistributionSR.RequestTypes.GET_LIST_DATA:
						this.GetHeaderLists ( );
						break;
					case AdjustmentDistributionSR.RequestTypes.GET_USER_DATA:
						this.GetUserData ( );
						break;
					case AdjustmentDistributionSR.RequestTypes.CREATE_ADJUSTMENTS:
						this.CreateAdjustment ( );
						break;
					case AdjustmentDistributionSR.RequestTypes.GET_CONFIGURATION_DATA:
						this.GetConfigurationData ( );
						break;
					case AdjustmentDistributionSR.RequestTypes.GET_OWNERS:
						this.GetOwners ( );
						break;
					case AdjustmentDistributionSR.RequestTypes.GET_TRANSACTIONS:
						this.GetTransactions ( );
						break;
				}
			}

			return adjustDistDO;
		}
		#endregion

		#region Private Methods
		/// <summary>
		/// This method will retrieve the owner list. If the consortium
		/// flag is set to true, then the list of companies will be only
		/// the companies that are part of the consortium. Else, the list 
		/// is all companies for the site.
		/// </summary>
		private void GetOwners ( )
		{
			ArrayList ownerList = new ArrayList ( );
			ProductCollectionClass productCollection;
			string productID = adjustDistSR.ProductID;
			Guid productIdentityGuid = Guid.Empty;

			// Get the list of products that are associated to the site and the user.
			ProductsClass products = new ProductsClass ( );
			productCollection = (ProductCollectionClass) products.Enumerate ( this.adjustDistSR.Security );

			// Find the product index in order to find the consortium 
			// owners.
			foreach (ProductClass product in productCollection)
			{
				if (adjustDistSR.ProductID == product.ID)
					productIdentityGuid = product.IdentityGuid;
			}

			// Get the list of consortium owners if flag is set. 
			if (this.adjustDistSR.IsConsortium == true)
			{
				CompanyGroupClass companyGroup;
				CompanyMapCollectionClass companyMaps;
				CompanyGroupsClass companyGroups = new CompanyGroupsClass ( );
				companyGroup = companyGroups.GetByProductIdentityGuid(this.adjustDistSR.Security, productIdentityGuid);

				companyMaps = companyGroup.AssignedCompanyCollection;
				foreach (CompanyMapClass companyMap in companyMaps)
				{
					string ids = companyMap.ID;
					ownerList.Add ( companyMap.AssignedID );
				}
			}
			else
			{
				// Retrieve the owners for this site.
				CompanyCollectionClass companyList;
				bool filterByUserAssociation = false;
				CompaniesClass companies = new CompaniesClass ( );

				companyList = companies.EnumerateByRole ( this.adjustDistSR.Security, COMPANY_ROLE.OWNER, filterByUserAssociation );

				foreach (CompanyClass company in companyList)
				{
					ownerList.Add ( company.ID );
				}
			}

			this.adjustDistDO.OwnerList.Clear();
			this.adjustDistDO.OwnerList.AddRange( ownerList );
		}

		/// <summary>
		/// This method will retrieve the Manager, Product, and Transaction Type lists to be
		/// displayed on the adjustment page.
		/// </summary>
		/// <returns></returns>
		private void GetHeaderLists ( )
		{
			CompanyCollectionClass companyCollection;
			ProductCollectionClass productCollection;

			// Get the list of companies that are associated to the site and the user.
			CompaniesClass companies = new CompaniesClass ( );
			companyCollection = (CompanyCollectionClass) companies.EnumerateByRole ( this.adjustDistSR.Security, COMPANY_ROLE.MANAGER, false );

			ArrayList managerList = new ArrayList ( );
			foreach (CompanyClass company in companyCollection)
			{
				managerList.Add ( company.ID );
			}

			// Get the list of products that are associated to the site and the user.
			ProductsClass products = new ProductsClass ( );
			productCollection = (ProductCollectionClass) products.Enumerate ( this.adjustDistSR.Security );

			ArrayList productList = new ArrayList ( );
			foreach (ProductClass product in productCollection)
			{
				productList.Add ( product.ID );
			}

			this.adjustDistDO.ManagerList.Clear();
			this.adjustDistDO.ManagerList.AddRange( managerList );

			this.adjustDistDO.ProductList.Clear();
			this.adjustDistDO.ProductList.AddRange( productList );

			this.adjustDistDO.TransactionAliasList.Clear();
			this.adjustDistDO.TransactionAliasList.AddRange( this.GetTransactionAliases() );
		}

		/// <summary>
		/// This method will retrieve the transaction aliases from the database for a
		/// given site.  It will return an array list of the transaction alias names.
		/// <returns></returns>
		private ArrayList GetTransactionAliases ( )
		{
			TransactionAliasesClass transAliasesObj;
			TransactionAliasCollectionClass transAliasList;
			ArrayList transList = new ArrayList ( );

			try
			{
				transAliasesObj = new TransactionAliasesClass ( );
				transAliasList = transAliasesObj.Enumerate ( this.adjustDistSR.Security );

				// Loop through all the transactions and retrieve the alias name and ID.
				// Place in dropdown value pair for binding in the GUI. Only save the adjustment
				// type transactions.
				foreach (TransactionAliasClass transAliasObj in transAliasList)
				{
					if (( transAliasObj.TransTypeID == TransactionTypes.T1_PrimaryAdjustment ) ||
						( transAliasObj.TransTypeID == TransactionTypes.T2_SecondaryAdjustment ))
					{
						DropdownValuePairDO valuePair	= new DropdownValuePairDO ( );
						valuePair.Text					= transAliasObj.ID;
						valuePair.TextValue = transAliasObj.MasterRecordGuid.ToString();
						transList.Add ( valuePair );
					}
				}
			}
			catch (Exception ex)
			{
				Debug.WriteLine ( "Error in retrieving Transaction Aliases: " + ex );
			}

			return transList;
		}

		/// <summary>
		/// This method will retrieve the adjustment distribution configuration data.
		/// </summary>
		private void GetConfigurationData ( )
		{
			DataSet dataSet = null;
			Guid siteGuid	= adjustDistSR.Security.SiteGuid;


			AdjustmentDistributionConfigurationDO adjustConfigDO = new AdjustmentDistributionConfigurationDO ( );

			try
			{
				using (SqlCommand cmd = adjustConfigDO.RetrieveAdjustmentConfigurationSQL(siteGuid))
				{
					dataSet = this.consolidatedDA.GetDataSet ( cmd, this.adjustDistSR.Security);
				}
				adjustConfigDO.LoadAdjustmentConfigurationSQL ( dataSet );
			}
			catch (Exception ex)
			{
				Debug.WriteLine ( "Error in retrieving Transaction Aliases: " + ex );
			}

			this.adjustDistDO.AdjustConfigurationDO = adjustConfigDO;
		}

		/// <summary>
		/// This method will set the array list of user data fields in the adjustment distribution
		/// data object.
		/// </summary>
		private void GetUserData ( )
		{
			TransactionAliasClass transAliasObj = new TransactionAliasClass ( );
			UserDataFieldCollectionClass userDataFields = null;
			UserDataFieldsClass userDataFieldsObj = new UserDataFieldsClass ( );
			userDataFields = userDataFieldsObj.EnumerateByEntityType(this.adjustDistSR.Security,
																	transAliasObj.EntityType,
																	this.adjustDistSR.TransactionAliasGuid,
																	false,
																	false);
			ArrayList userFields = new ArrayList ( );

			foreach (UserDataFieldClass userDataField in userDataFields)
			{
				userFields.Add ( userDataField );
			}

			this.adjustDistDO.UserFields.Clear();
			this.adjustDistDO.UserFields.AddRange( userFields );
		}


		/// <summary>
		/// This method will retrieve the transactions from the last closeout
		/// period until the requested inventory date.
		/// </summary>
		private void GetTransactions ( )
		{
			

			try
			{
				DataSet dataSet = null;
				using (SqlCommand cmd = this.adjustDistDO.GetTransactionsForDateRangeSQL(this.adjustDistSR, this.GetCloseoutDate()))
				{
					dataSet = this.consolidatedDA.GetDataSet( cmd, this.adjustDistSR.Security);
				}

				if (dataSet != null)
				{
					this.adjustDistDO.LoadTransactionsForDateRangeSQL ( dataSet );
				}
			}
			catch (Exception ex)
			{
				this.logger.Debug ( Resource1.ErrorRetrievingTransQuantities + ex );
				throw;
			}
		}

		/// <summary>
		/// This method will retrieve the closeout date to be used in getting the 
		/// list of transactions for calculating throughput.
		/// </summary>
		/// <returns></returns>
		private string GetCloseoutDate ( )
		{
			string closeoutDateStr = "";
			
			try
			{
				DataSet dataSet = null;
				using (SqlCommand cmd = this.adjustDistDO.GetLatestCloseoutDateSelectSQL(this.adjustDistSR))
				{
					dataSet = this.consolidatedDA.GetDataSet ( cmd, this.adjustDistSR.Security);
				}

				if (dataSet != null)
				{
					closeoutDateStr = this.adjustDistDO.LoadLatestCloseoutDate ( dataSet );
				}
			}
			catch (Exception ex)
			{
				this.logger.Debug ( Resource1.ErrorRetrievingCloseoutDate + ex );
				throw;
			}

			return closeoutDateStr;
		}

		/// <summary>
		/// This method will create an adjustment transaction for each owner.
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage( "Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId="FMBusinessObjects.LogClient.Logger.Debug(System.String)" )]
		private void CreateAdjustment ( )
		{
			TransactionDO transactionDO		= null;
			LineItemDO lineItemDO				= null;
			QuantityDO quantity					= null;
			List<TransactionDO> transList		= null;
			List<LineItemDO> lineItemList		= null;

			// If there is owner information to be create, then start adding transactions for each owner.
			if (( this.adjustDistSR.AdjustmentOwnerRecordList != null ) && ( this.adjustDistSR.AdjustmentOwnerRecordList.Count > 0 ))
			{
				transList = new List<TransactionDO>();
				SaveTransactionsSR saveTransSR = new SaveTransactionsSR ( );

				foreach (AdjustmentOwnerRecord adjOwnerRecord in this.adjustDistSR.AdjustmentOwnerRecordList)
				{
					// Create a new transaction DO and populate with the header information.
					transactionDO				= new TransactionDO ( );
					transactionDO.Site			= this.adjustDistSR.Security.SiteID;
					transactionDO.SiteGuid		= this.adjustDistSR.Security.SiteGuid;
					transactionDO.TransTypeID	= this.GetAdjustmentAliasTransType ( );
					transactionDO.Alias			= this.GetAdjustmentAliasName ( );
					transactionDO.TransactionAliasGuid	= this.adjustDistSR.TransactionAliasGuid;
					transactionDO.ManagerID		= this.adjustDistSR.ManagerID;
					transactionDO.ManagerCode	= this.GetCompanyCode ( COMPANY_ROLE.MANAGER, this.adjustDistSR.ManagerID );
					transactionDO.OwnerID		= adjOwnerRecord.OwnerName;
					transactionDO.OwnerCode		= this.GetCompanyCode ( COMPANY_ROLE.OWNER, adjOwnerRecord.OwnerName );
					transactionDO.InventoryDate = this.adjustDistSR.InventoryDate.Date;

					// Add notes if info is present.
					if (string.IsNullOrEmpty ( adjustDistSR.Notes ) == false)
					{
						transactionDO.Notes = this.adjustDistSR.Notes;
					}

					// Create and populate the line item and add to the transaction DO.
					lineItemDO					= new LineItemDO ( );
					quantity						= new QuantityDO ( adjOwnerRecord.GrossValue, adjOwnerRecord.NetValue, adjOwnerRecord.MassValue, 0 );
					lineItemDO.Quantity		= quantity;
					lineItemDO.Product		= this.adjustDistSR.ProductID;
					lineItemDO.ProductCode	= this.GetProductCode ( this.adjustDistSR.ProductID );
					lineItemList				= new List<LineItemDO> ( );

					lineItemList.Add ( lineItemDO );
					transactionDO.LineItems = lineItemList;

					// Get the user data list.
					this.FindUserDataAssociation ( transactionDO );

					// Create a list of transactions
					transList.Add ( transactionDO );
				}

				// Save the transactions
				try
				{
					saveTransSR.Transactions = transList;
					saveTransSR.Security = this.adjustDistSR.Security;
					saveTransSR.UseAutoComplete = true;

					//No conversion required. SI units assumed. See AdjustmentDistribution.aspx.cs CreatAdjustmentOnClick method.
					saveTransSR.ConvertUnits = false;

					SaveTransactionsProcessor saveTransProcessor = new SaveTransactionsProcessor ( );
					SaveTransactionsResultDO transResults = saveTransProcessor.SaveTransactions ( saveTransSR );
				}
				catch (Exception ex)
				{
					string message = string.Format( "{0} - {1}", Resource1.ErrorSavingAdjustmentTransaction, ex );
					this.logger.Debug ( message );
					throw;
				}
			}

		}

		/// <summary>
		/// This method will will retrieve the user data list from the database and associate the 
		/// user data database names to the user data fields retrieved from the adjustment page.
		/// In order to save the user data, the hash table must have the keys as the database names
		/// and not the display name. This method will populate the transaction DO's hash table with
		/// the appropriate keys and the values from the GUI.
		/// </summary>
		/// <param name="transDO"></param>
		private void FindUserDataAssociation ( TransactionDO transDO )
		{
			Hashtable guiUserData = this.adjustDistSR.UserDataList;

			// Get the user data fields from the database.
			this.GetUserData ( );
			ArrayList dbUserDataList = this.adjustDistDO.UserFields;

			if (( dbUserDataList != null ) && ( guiUserData != null ))
			{
				// Match the display name from the GUI with the display name of the
				// user data field from the database. Build a new hash table with the
				// database user data names with the values retrieved from the GUI.
				foreach (UserDataFieldClass dbUserDataField in dbUserDataList)
				{
					if (guiUserData.Contains ( dbUserDataField.DisplayName ) == true)
					{
						string userDataValue = guiUserData[dbUserDataField.DisplayName] as string;
						transDO.UserData.Add ( dbUserDataField.ID, userDataValue );
					}
					else if (guiUserData.Contains ( dbUserDataField.ID ) == true)
					{
						string userDataValue = guiUserData[dbUserDataField.ID] as string;
						transDO.UserData.Add ( dbUserDataField.ID, userDataValue );
					}
				}
			}
		}

		/// <summary>
		/// This method will return the aliasname for an adjustment distribution type transaction.
		/// </summary>
		/// <returns></returns>
		[System.Diagnostics.CodeAnalysis.SuppressMessage( "Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId="FMBusinessObjects.LogClient.Logger.Debug(System.String)" )]
		private string GetAdjustmentAliasName ( )
		{
			string aliasName = "";
			TransactionAliasesClass transAliasesObj;
			TransactionAliasCollectionClass transAliasList;

			try
			{
				transAliasesObj = new TransactionAliasesClass ( );
				transAliasList = transAliasesObj.Enumerate ( this.adjustDistSR.Security );

				// Find the alias name for the adjustment type transaction.
				foreach (TransactionAliasClass transAliasObj in transAliasList)
				{
					if (transAliasObj.IdentityGuid == this.adjustDistSR.TransactionAliasGuid)
					{
						aliasName = transAliasObj.ID;
						break;
					}
				}
			}
			catch (Exception ex)
			{
				string message = string.Format( "{0} - {1}", Resource1.ErrorRetrievingTransactionAliases, ex );
				this.logger.Debug ( message );
			}

			return aliasName;
		}

		/// <summary>
		/// This method will return the alias transaction type for an adjustment distribution type transaction.
		/// The default is set to primary adjustment.
		/// </summary>
		/// <returns></returns>
		private TransactionTypes GetAdjustmentAliasTransType ( )
		{
			TransactionTypes transType = TransactionTypes.T1_PrimaryAdjustment;
			TransactionAliasesClass transAliasesObj;
			TransactionAliasCollectionClass transAliasList;

			try
			{
				transAliasesObj = new TransactionAliasesClass ( );
				transAliasList = transAliasesObj.Enumerate ( this.adjustDistSR.Security );

				// Find the alias name for the adjustment type transaction.
				foreach (TransactionAliasClass transAliasObj in transAliasList)
				{
					if (transAliasObj.IdentityGuid == this.adjustDistSR.TransactionAliasGuid)
					{
						transType = transAliasObj.TransTypeID;
						break;
					}
				}
			}
			catch (Exception ex)
			{
				string message = string.Format( "{0} - {1}", Resource1.ErrorRetrievingTransactionAliases, ex );
				this.logger.Debug( message );
			}

			return transType;
		}

		/// <summary>
		/// This method will return either a blank code or the code for a given company role and
		/// company name.
		/// </summary>
		/// <param name="role"></param>
		/// <param name="companyName"></param>
		/// <returns></returns>
		private string GetCompanyCode ( COMPANY_ROLE role, string companyName )
		{
			string code = "";
			CompaniesClass companyObj = new CompaniesClass ( );
			CompanyCollectionClass companies = null;

			if (( companyName != null ) && ( companyName.Length > 0 ))
			{
				try
				{
					companies = companyObj.EnumerateByRole ( this.adjustDistSR.Security, role, false );

					foreach (CompanyClass company in companies)
					{
						if (companyName == company.ID)
						{
							code = company.Code;
							break;
						}
					}
				}
				catch (Exception ex)
				{
					this.logger.Debug ( Resource1.ErrorRetrievingCompanyObjects + ex );
				}
			}

			return code;
		}

		/// <summary>
		/// This method will return either a blank code or the code for a given product name.
		/// </summary>
		/// <param name="productName"></param>
		/// <returns></returns>
		private string GetProductCode ( string productName )
		{
			string code = "";
			ProductsClass productObj = new ProductsClass ( );
			ProductCollectionClass products = null;

			if (( productName != null ) && ( productName.Length > 0 ))
			{
				try
				{
					products = productObj.Enumerate ( this.adjustDistSR.Security );

					foreach (ProductClass product in products)
					{
						if (productName == product.ID)
						{
							code = product.Code;
							break;
						}
					}
				}
				catch (Exception ex)
				{
					this.logger.Debug ( Resource1.ErrorRetrievingProductObjects + ex );
				}
			}

			return code;
		}
		#endregion
	}
}
