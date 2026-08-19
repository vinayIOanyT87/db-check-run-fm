namespace FMBusinessServices.ServiceClasses
{
	using System.Security;
	using System.ServiceModel;
	using System.Data.SqlClient;
	using System.Data;
	using System;

	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessServices.DataAccessLayer;
	using FMBusinessServices.InternalClasses;
    using System.Text.RegularExpressions;

    [SecuritySafeCriticalAttribute]
	[ServiceBehavior(TransactionIsolationLevel = System.Transactions.IsolationLevel.ReadCommitted)]
	public class WWIntegrationClass : IWWIntegrationClass
	{
		#region Private data members
		private readonly ConsolidatedDAClass consolidatedDA;
		private const string GuidRegEx = @"^[{]?[0-9a-fA-F]{8}-([0-9a-fA-F]{4}-){3}[0-9a-fA-F]{12}[}]?$";
		#endregion

		#region Constructors
		/// <summary>
		/// This is the default constructor for the Currencies class.
		/// </summary>
		public WWIntegrationClass()
		{
			this.consolidatedDA = new ConsolidatedDAClass ( );
		}
		#endregion

		#region Public methods
		/// <summary>
		/// This method returns a WWIntegration data object based on the
		/// site guid.
		/// </summary>
		/// <param name="security"></param>
		/// <param name="siteGuid"></param>
		/// <returns></returns>
		public WWIntegrationDO GetForSite ( SecurityClass security, Guid siteGuid )
		{
			WWIntegrationDO integration = null;

			try
			{
				var integrationDO = new WWIntegrationDO();

				using (var cmd = new SqlCommand())
				{
					integrationDO.SelectForSite(cmd, siteGuid);
					DataSet dataSet = this.consolidatedDA.GetDataSet(cmd, security);

					if (dataSet != null && dataSet.Tables.Count > 0)
					{
						DataTable dataTable = dataSet.Tables[0];

						if ((dataTable.Rows != null) && (dataTable.Rows.Count > 0))
						{
							// Populate the currencies collection
							foreach (DataRow dataRow in dataTable.Rows)
							{
								integration = new WWIntegrationDO();
								integration.Populate(dataTable.Rows[0]);
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				throw new Exception ( "An error occurred attempting to load WW Integrations from the database. " + ex.Message );
			}

			return integration;
		}

		/// <summary>
		/// This method returns a WWIntegration data object based on the site guid in the security object
		/// </summary>
		/// <param name="security"></param>
		/// <param name="siteGuid"></param>
		/// <returns></returns>
		public WWIntegrationDO Get(SecurityClass security)
		{
			WWIntegrationDO integration = null;

			try
			{
				var integrationDO = new WWIntegrationDO();

				using (var cmd = new SqlCommand())
				{
					integrationDO.SelectForSite(cmd, security.SiteGuid);
					DataSet dataSet = this.consolidatedDA.GetDataSet(cmd, security);

					if (dataSet != null && dataSet.Tables.Count > 0)
					{
						DataTable dataTable = dataSet.Tables[0];

						if ((dataTable.Rows != null) && (dataTable.Rows.Count > 0))
						{
							// Populate the currencies collection
							foreach (DataRow dataRow in dataTable.Rows)
							{
								integration = new WWIntegrationDO();
								integration.Populate(dataTable.Rows[0]);
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				throw new Exception("An error occurred attempting to load WW Integrations from the database. " + ex.Message);
			}

			return integration;
		}
		/// <summary>
		/// This method returns a collection of all integration data objects.
		/// </summary>
		/// <param name="security"></param>
		/// <returns></returns>
		public WWIntegrationDOCollectionClass GetIntegrations ( SecurityClass security )
		{
			var integrationCollection = new WWIntegrationDOCollectionClass( );

			try
			{
				var integrationDO = new WWIntegrationDO( );

				using (var cmd = new SqlCommand())
				{
					integrationDO.SelectIntegrations(cmd);

					DataSet dataSet = this.consolidatedDA.GetDataSet(cmd, security);

					if (dataSet != null && dataSet.Tables.Count > 0)
					{
						DataTable dataTable = dataSet.Tables[0];

						if (dataTable.Rows != null)
						{
							foreach (DataRow dataRow in dataTable.Rows)
							{
								var integration = new WWIntegrationDO();
								integration.Populate(dataRow);
								integrationCollection.Add(integration);
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				throw new Exception ( "An error occured attempting to load WW Integrations from the database. " + ex.Message );
			}

			return integrationCollection;
		}

		/// <summary>
		/// This method returns a WW Integration data object based on the integration guid.
		/// </summary>
		/// <param name="security"></param>
		/// <param name="integrationGuid"></param>
		/// <returns></returns>
		public WWIntegrationDO GetByIntegrationGuid( SecurityClass security, Guid integrationGuid)
		{
			WWIntegrationDO integration = null;

			try
			{
				var integrationDO = new WWIntegrationDO( );

				using (var cmd = new SqlCommand())
				{
					integrationDO.SelectByIntegrationGuid(cmd, integrationGuid);

					DataSet dataSet = this.consolidatedDA.GetDataSet(cmd, security);

					if (dataSet != null && dataSet.Tables.Count > 0)
					{
						DataTable dataTable = dataSet.Tables[0];

						if ((dataTable.Rows != null) && (dataTable.Rows.Count > 0))
						{
							integration = new WWIntegrationDO();
							integration.Populate(dataTable.Rows[0]);
						}
					}
				}
			}
			catch (Exception ex)
			{
				throw new Exception ( "An error occurred attempting to load a WW Integration from the database.  " + ex.Message );
			}

			return integration;
		}

		/// <summary>
		/// This method returns a collection of all WW Integration data objects.
		/// </summary>
		/// <param name="security"></param>
		/// <param name="stationIATACode"></param>
		/// <returns></returns>
		public WWIntegrationDOCollectionClass GetByStationIATACode(SecurityClass security, string stationIATACode)
		{
			var integrationCollection = new WWIntegrationDOCollectionClass();

			try
			{
				var integrationDO = new WWIntegrationDO();

				using (var cmd = new SqlCommand())
				{
					integrationDO.SelectByStationIATA(cmd, stationIATACode);

					DataSet dataSet = this.consolidatedDA.GetDataSet(cmd, security);

					if (dataSet != null && dataSet.Tables.Count > 0)
					{
						DataTable dataTable = dataSet.Tables[0];

						if (dataTable.Rows != null)
						{
							foreach (DataRow dataRow in dataTable.Rows)
							{
								var integration = new WWIntegrationDO();
								integration.Populate(dataRow);
								integrationCollection.Add(integration);
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				throw new Exception("An error occured attempting to load WW Integrations from the database. " + ex.Message);
			}

			return integrationCollection;
		}

		/// <summary>
		/// This method returns a collection of all WW Integration data objects.
		/// </summary>
		/// <param name="security"></param>
		/// <param name="vendor"></param>
		/// <returns></returns>
		public WWIntegrationDOCollectionClass GetByVendor(SecurityClass security, string vendor)
		{
			var integrationCollection = new WWIntegrationDOCollectionClass();

			try
			{
				var integrationDO = new WWIntegrationDO();

				using (var cmd = new SqlCommand())
				{
					integrationDO.SelectByVendor(cmd, vendor);

					DataSet dataSet = this.consolidatedDA.GetDataSet(cmd, security);

					if (dataSet != null && dataSet.Tables.Count > 0)
					{
						DataTable dataTable = dataSet.Tables[0];

						if (dataTable.Rows != null)
						{
							foreach (DataRow dataRow in dataTable.Rows)
							{
								var integration = new WWIntegrationDO();
								integration.Populate(dataRow);
								integrationCollection.Add(integration);
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				throw new Exception("An error occured attempting to load WW Integrations from the database. " + ex.Message);
			}

			return integrationCollection;
		}

		/// <summary>
		/// This method will insert a new WW Integration object into the database.
		/// </summary>
		/// <param name="security"></param>
		/// <param name="integration"></param>
		[OperationBehavior( TransactionScopeRequired = true, TransactionAutoComplete = true )]
		public void Add( SecurityClass security, WWIntegrationDO integration )
		{
			// Make sure all required fields are populated
			this.Validate (integration);

			if (this.IntegrationExists( security, integration))
			{
				throw new FMBusinessObjects.Exceptions.FinanceObjectExistsException ( "An Integration with that IntegrationGuid already exists." );
			}

			try
			{
				var integrationDO = new WWIntegrationDO( );

				using (var cmd = new SqlCommand())
				{
					integration.IdentityGuid = Guid.NewGuid();
					integrationDO.Insert(cmd, integration, security.UserID);

					this.consolidatedDA.ExecuteQuery(security, cmd);
				}
			}
			catch (Exception ex)
			{
				throw new Exception ( "An error occurred attempting to add a WW Integration to the database." + ex.Message );
			}
		}

		/// <summary>
		/// This method will update an existing integration object 
		/// </summary>
		/// <param name="integration"></param>
		/// <param name="security"></param>
		[OperationBehavior( TransactionScopeRequired = true, TransactionAutoComplete = true )]
		public void Save( SecurityClass security, WWIntegrationDO integration )
		{
			// Make sure required fields are populated
			this.Validate (integration);

			try
			{
				var integrationDO = new WWIntegrationDO( );

				using (var cmd = new SqlCommand())
				{
					integrationDO.Update(cmd, integration, security.UserID);

					// Update tblMobileDispatchSiteIntegrationInfo.
					this.consolidatedDA.ExecuteQuery(security, cmd);
				}
			}
			catch (Exception ex)
			{
				throw new Exception ( "An error occurred attempting to save a WW Integration object to the database.  " + ex.Message );
			}
		}

		/// <summary>
		/// This method will remove a integration object
		/// </summary>
		/// <param name="integrationGuid"></param>
		/// <param name="security"></param>
		[OperationBehavior( TransactionScopeRequired = true, TransactionAutoComplete = true )]
		public void Remove( SecurityClass security, Guid integrationGuid)
		{
			try
			{
				var integrationDO = new WWIntegrationDO();

				using (var cmd = new SqlCommand())
				{
					integrationDO.Delete(cmd, integrationGuid);
					this.consolidatedDA.ExecuteQuery(security, cmd);
				}
			}
			catch (Exception ex)
			{
				throw new Exception ( "An error occured attempting to delete a WW Integration object from the database.  " + ex.Message );
			}
		}
		#endregion

		#region Private methods
		/// <summary>
		/// This method will return true if the currency already exists.  Otherwise,
		/// it will return false.
		/// </summary>
		/// <param name="security"></param>
		/// <param name="integration"></param>
		/// <returns></returns>
		private bool IntegrationExists ( SecurityClass security, WWIntegrationDO integration )
		{
			bool exists = false;
			var integrationDO = new WWIntegrationDO( );

			using (var cmd = new SqlCommand())
			{
				integrationDO.Exists(cmd, integration, ContextUtil.IsInTransaction);

				try
				{
					DataSet dataSet = this.consolidatedDA.GetDataSet(cmd, security);

					if (dataSet != null  && dataSet.Tables.Count > 0)
					{
						DataTable dataTable = dataSet.Tables[0];
						exists = (dataTable.Rows.Count != 0);
					}
				}
				catch (Exception ex)
				{
					throw new ApplicationException("An error occurred attempting to see if an Integration exists.  " + ex.Message);
				}
			}

			return exists;
		}


		/// <summary>
		/// Checks to see all required fields are populated
		/// </summary>
		/// <param name="integration">The integration object to validate</param>
		private void Validate ( WWIntegrationDO integration )
		{
			Regex re = new Regex(GuidRegEx);
			if (!re.IsMatch(integration.IntegrationGuid.ToString()))
			{
				throw new ApplicationException("Integration GUID is required.");
			}

			if (string.IsNullOrEmpty(integration.API_Username.Trim ( )))
			{
				throw new ApplicationException ( "API Username is required." );
			}

			if (string.IsNullOrEmpty(integration.API_Password.Trim()))
			{
				throw new ApplicationException("API Password is required.");
			}

			if (string.IsNullOrEmpty(integration.StationIATACode.Trim()))
			{
				throw new ApplicationException("Station IATA code is required.");
			}

			if (string.IsNullOrEmpty(integration.Vendor.Trim()))
			{
				throw new ApplicationException("Vendor is required.");
			}
		}
		#endregion
	}
}