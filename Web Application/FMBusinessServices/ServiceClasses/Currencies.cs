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

	[SecuritySafeCriticalAttribute]
	[ServiceBehavior(TransactionIsolationLevel = System.Transactions.IsolationLevel.ReadCommitted)]
	public class CurrenciesClass : ICurrencies
	{
		#region Private data members
		private readonly ConsolidatedDAClass consolidatedDA;
		#endregion

		#region Constructors
		/// <summary>
		/// This is the default constructor for the Currencies class.
		/// </summary>
		public CurrenciesClass()
		{
			this.consolidatedDA = new ConsolidatedDAClass ( );
		}
		#endregion

		#region Public methods
		/// <summary>
		/// This method returns a collection of currency data objects based on the
		/// site guid.
		/// </summary>
		/// <param name="security"></param>
		/// <param name="siteGuid"></param>
		/// <returns></returns>
		public CurrencyDOCollectionClass GetForSite ( SecurityClass security, Guid siteGuid )
		{
			var currencyCollection = new CurrencyDOCollectionClass ( );

			try
			{
				var currencyDO = new CurrencyDO ( );

				using (var cmd = new SqlCommand())
				{
					currencyDO.SelectForSite(cmd, siteGuid);
					DataSet dataSet = this.consolidatedDA.GetDataSet(cmd, security);

					if (dataSet != null && dataSet.Tables.Count > 0)
					{
						DataTable dataTable = dataSet.Tables[0];

						if (dataTable.Rows != null)
						{
							// Populate the currencies collection
							foreach (DataRow dataRow in dataTable.Rows)
							{
								var currency = new CurrencyDO();
								currency.Populate(dataRow);
								currencyCollection.Add(currency);
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				throw new Exception ( "An error occurred attempting to load Currencies from the database. " + ex.Message );
			}

			return currencyCollection;
		}

		/// <summary>
		/// This method returns a collection of all currency data objects.
		/// </summary>
		/// <param name="security"></param>
		/// <returns></returns>
		public CurrencyDOCollectionClass GetCurrencies ( SecurityClass security )
		{
			var currencyCollection = new CurrencyDOCollectionClass ( );

			try
			{
				var currencyDO = new CurrencyDO ( );

				using (var cmd = new SqlCommand())
				{
					currencyDO.SelectCurrencies(cmd);

					DataSet dataSet = this.consolidatedDA.GetDataSet(cmd, security);

					if (dataSet != null && dataSet.Tables.Count > 0)
					{
						DataTable dataTable = dataSet.Tables[0];

						if (dataTable.Rows != null)
						{
							foreach (DataRow dataRow in dataTable.Rows)
							{
								var currency = new CurrencyDO();
								currency.Populate(dataRow);
								currencyCollection.Add(currency);
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				throw new Exception ( "An error occured attempting to load Currencies from the database. " + ex.Message );
			}

			return currencyCollection;
		}

		/// <summary>
		/// This method returns a collection of currency unit data objects.
		/// </summary>
		/// <param name="security"></param>
		/// <returns></returns>
		public CurrencyUnitDOCollectionClass GetCurrencyUnits ( SecurityClass security )
		{
			var unitCollection = new CurrencyUnitDOCollectionClass ( );

			try
			{
				var currencyDO = new CurrencyDO ( );
				using (var cmd = new SqlCommand())
				{
					currencyDO.SelectCurrencyUnits(cmd);

					DataSet dataSet = this.consolidatedDA.GetDataSet(cmd, security);

					if (dataSet != null && dataSet.Tables.Count > 0)
					{
						DataTable dataTable = dataSet.Tables[0];

						if (dataTable.Rows != null)
						{
							// Populate the collection
							foreach (DataRow dataRow in dataTable.Rows)
							{
								var unit = new CurrencyUnitDO();
								unit.Populate(dataRow);
								unitCollection.Add(unit);
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				throw new Exception ( "An error occurred attempting to load Currency Units from the database.  " + ex.Message );
			}

			return unitCollection;
		}

		/// <summary>
		/// This method returns a currency data object based on the currency guid.
		/// </summary>
		/// <param name="security"></param>
		/// <param name="currencyGuid"></param>
		/// <returns></returns>
		public CurrencyDO Get ( SecurityClass security, Guid currencyGuid )
		{
			CurrencyDO currency = null;

			try
			{
				var currencyDO = new CurrencyDO ( );

				using (var cmd = new SqlCommand())
				{
					currencyDO.Select(cmd, currencyGuid);

					DataSet dataSet = this.consolidatedDA.GetDataSet(cmd, security);

					if (dataSet != null && dataSet.Tables.Count > 0)
					{
						DataTable dataTable = dataSet.Tables[0];

						if ((dataTable.Rows != null) && (dataTable.Rows.Count > 0))
						{
							currency = new CurrencyDO();
							currency.Populate(dataTable.Rows[0]);

							using (var cmd2 = new SqlCommand())
							{
								currencyDO.SelectLineItemsForCurrency(cmd2, currencyGuid);
								dataSet = this.consolidatedDA.GetDataSet(cmd2, security);

								if (dataSet != null && dataSet.Tables.Count > 0)
								{
									dataTable = dataSet.Tables[0];

									if (dataTable.Rows != null)
									{
										foreach (DataRow row in dataTable.Rows)
										{
											var lineItem = new CurrencyLineItemDO();
											lineItem.Populate(row);
											currency.LineItems.Add(lineItem);
										}
									}
								}
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				throw new Exception ( "An error occurred attempting to load a Currency from the database.  " + ex.Message );
			}

			return currency;
		}

		/// <summary>
		/// This method returns a currency data object based on the unit index.
		/// </summary>
		/// <param name="security"></param>
		/// <param name="unitIndex"></param>
		/// <returns></returns>
		public CurrencyDO GetByUnitIndex ( SecurityClass security, int unitIndex )
		{
			CurrencyDO currency = null;

			try
			{
				var currencyDO = new CurrencyDO ( );

				using (var cmd = new SqlCommand())
				{
					currencyDO.SelectByUnitIndex(cmd, unitIndex);

					DataSet dataSet = this.consolidatedDA.GetDataSet(cmd, security);

					if (dataSet != null  && dataSet.Tables.Count > 0)
					{
						DataTable dataTable = dataSet.Tables[0];

						if ((dataTable.Rows != null) && (dataTable.Rows.Count > 0))
						{
							currency = new CurrencyDO();
							currency.Populate(dataTable.Rows[0]);

							using (var cmd2 = new SqlCommand())
							{
								currencyDO.SelectLineItemsForCurrency(cmd2, currency.IdentityGuid);
								dataSet = this.consolidatedDA.GetDataSet(cmd2, security);

								if (dataSet != null  && dataSet.Tables.Count > 0)
								{
									if (dataTable.Rows != null)
									{
										foreach (DataRow row in dataTable.Rows)
										{
											var lineItem = new CurrencyLineItemDO();
											lineItem.Populate(row);
											currency.LineItems.Add(lineItem);
										}
									}
								}
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				throw new Exception ( "An error occurred attempting to load a Currency from the database.  " + ex.Message );
			}

			return currency;
		}

		/// <summary>
		/// This method will insert a new currency object into the database.
		/// </summary>
		/// <param name="security"></param>
		/// <param name="currency"></param>
		[OperationBehavior( TransactionScopeRequired = true, TransactionAutoComplete = true )]
		public void Add( SecurityClass security, CurrencyDO currency )
		{
			// Make sure all required fields are populated
			this.Validate ( currency );

			if (this.CurrencyExists ( security, currency ))
			{
				throw new FMBusinessObjects.Exceptions.FinanceObjectExistsException ( "A Currency with that Name already exists." );
			}

			try
			{
				var currencyDO = new CurrencyDO ( );

				using (var cmd = new SqlCommand())
				{
					currency.IdentityGuid = Guid.NewGuid();
					currencyDO.Insert(cmd, currency, security.UserID);

					this.consolidatedDA.ExecuteQuery(security, cmd);

					// Now save the line items
					foreach (CurrencyLineItemDO lineItem in currency.LineItems)
					{
						try
						{
							lineItem.CurrencyGuid = currency.IdentityGuid;

							using (var cmd2 = new SqlCommand())
							{
								lineItem.IdentityGuid = Guid.NewGuid();
								currencyDO.InsertLineItem(cmd2, lineItem, security.UserID);
								this.consolidatedDA.ExecuteQuery(security, cmd2);
							}
						}
						catch (Exception ex)
						{
							throw new ApplicationException("An error occurred attempting to insert a currency " +
															"line item.", ex);
						}
					}
				}
			}
			catch (Exception ex)
			{
				throw new Exception ( "An error occurred attempting to add a Currency to the database." + ex.Message );
			}
		}

		/// <summary>
		/// This method will update an existing currency object and its
		/// line items.
		/// </summary>
		/// <param name="currency"></param>
		/// <param name="security"></param>
		[OperationBehavior( TransactionScopeRequired = true, TransactionAutoComplete = true )]
		public void Save( SecurityClass security, CurrencyDO currency )
		{
			// Make sure required fields are populated
			this.Validate ( currency );

			try
			{
				var currencyDO = new CurrencyDO ( );

				using (var cmd = new SqlCommand())
				{
					currencyDO.Update(cmd, currency, security.UserID);

					// Update the currency.
					this.consolidatedDA.ExecuteQuery(security, cmd);
				}

				// Insert any new line items, update any dirty line items, and prepare a string
				// that will be used to remove from the db any deleted line items
				string deleteSql = string.Empty;

				foreach (CurrencyLineItemDO lineItem in currency.LineItems)
				{
					if (lineItem.IdentityGuid == Guid.Empty)
					{
						// This is a new line item so insert it
						try
						{
							lineItem.CurrencyGuid = currency.IdentityGuid;

							using (var cmd = new SqlCommand())
							{
								lineItem.IdentityGuid = Guid.NewGuid();
								currencyDO.InsertLineItem(cmd, lineItem, security.UserID);
								this.consolidatedDA.ExecuteQuery(security, cmd);

								deleteSql += "'" + lineItem.IdentityGuid + "',";
							}
						}
						catch (Exception ex)
						{
							throw new ApplicationException ( "An error occurred attempting to insert a currency " +
															"line item.  " + ex.Message );
						}

						continue;
					}

					// Update the line item
					if (lineItem.IsDirty)
					{
						try
						{
							using (var cmd = new SqlCommand())
							{
								currencyDO.UpdateLineItem(cmd, lineItem, security.UserID);
								this.consolidatedDA.ExecuteQuery(security, cmd);
							}

							deleteSql += "'" + lineItem.IdentityGuid + "',";
						}
						catch (Exception ex)
						{
							throw new ApplicationException ( "An error occurred attempting to update a currency line item.  " + ex.Message );
						}
						continue;
					}

					deleteSql += "'" + lineItem.IdentityGuid + "',";
				}

				// Now remove any deleted line items
				if ( string.IsNullOrEmpty(deleteSql) == false || currency.LineItems.Count == 0)
				{
					if (string.IsNullOrEmpty(deleteSql) == false)
					{
						// Get rid of the last character which should be a comma
						deleteSql = deleteSql.Substring ( 0, deleteSql.Length - 1 );

						try
						{
							using (var cmd = new SqlCommand())
							{
								currencyDO.DeleteAllCurrencyLineItemsBut(cmd, deleteSql, currency.IdentityGuid);
								this.consolidatedDA.ExecuteQuery(security, cmd);
							}
						}
						catch (Exception ex)
						{
							throw new ApplicationException ( "An error occurred attempting to delete currency line items.  " + ex.Message );
						}
					}

					if (currency.LineItems.Count == 0)
					{
						try
						{
							using (var cmd = new SqlCommand())
							{
								currencyDO.DeleteCurrencyLineItems(cmd, currency.IdentityGuid);
								this.consolidatedDA.ExecuteQuery(security, cmd);
							}
						}
						catch (Exception ex)
						{
							throw new ApplicationException ( "An error occurred attempting to delete currency line items.  " + ex.Message );
						}
					}
				}
			}
			catch (Exception ex)
			{
				throw new Exception ( "An error occurred attempting to save a Currency object to the database.  " + ex.Message );
			}
		}

		/// <summary>
		/// This method will remove a currency object along with its
		/// line items from the database.
		/// </summary>
		/// <param name="currencyGuid"></param>
		/// <param name="security"></param>
		[OperationBehavior( TransactionScopeRequired = true, TransactionAutoComplete = true )]
		public void Remove( SecurityClass security, Guid currencyGuid )
		{
			try
			{
				var currencyDO = new CurrencyDO();

				using (var cmd = new SqlCommand())
				{
					currencyDO.DeleteCurrencyLineItems(cmd, currencyGuid);
					this.consolidatedDA.ExecuteQuery(security, cmd);
				}

				using (var cmd = new SqlCommand())
				{
					currencyDO.Delete(cmd, currencyGuid);
					this.consolidatedDA.ExecuteQuery(security, cmd);
				}
			}
			catch (Exception ex)
			{
				throw new Exception ( "An error occured attempting to delete a Currency object from the database.  " + ex.Message );
			}
		}
		#endregion

		#region Private methods
		/// <summary>
		/// This method will return true if the currency already exists.  Otherwise,
		/// it will return false.
		/// </summary>
		/// <param name="security"></param>
		/// <param name="currency"></param>
		/// <returns></returns>
		private bool CurrencyExists ( SecurityClass security, CurrencyDO currency )
		{
			bool exists = false;
			var currencyDO = new CurrencyDO ( );

			using (var cmd = new SqlCommand())
			{
				currencyDO.Exists(cmd, currency, ContextUtil.IsInTransaction);

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
					throw new ApplicationException("An error occurred attempting to see if a Currency  exists.  " + ex.Message);
				}
			}

			return exists;
		}


		/// <summary>
		/// Checks to see all required fields are populated
		/// </summary>
		/// <param name="currency">The currency object to validate</param>
		private void Validate ( CurrencyDO currency )
		{
			if (string.IsNullOrEmpty(currency.UnitDisplayName.Trim ( )))
			{
				throw new ApplicationException ( "Currency Name is required." );
			}

			// WI#1327 (Kendall) - Enforce that at least one rate must be configured for a valid currency
			if (currency.LineItems.Count == 0)
			{
				throw new ApplicationException ( "Currency must have at least one configured rate." );
			}
		}
		#endregion
	}
}