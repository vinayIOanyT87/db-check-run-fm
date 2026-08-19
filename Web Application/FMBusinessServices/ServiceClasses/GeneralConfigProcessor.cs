using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.ServiceModel;
using System.Web;

using FMBusinessObjects.DataObjects;
using FMBusinessObjects.BusinessInterfaces;
using FMBusinessObjects.ServiceRequests;
using FMBusinessServices.DataAccessLayer;
using FMBusinessObjects.Exceptions;
using FMBusinessObjects.LogClient;

namespace FMBusinessServices.ServiceClasses
{
	using System.Diagnostics;
	using System.Reflection;

	[ServiceBehavior(TransactionIsolationLevel = System.Transactions.IsolationLevel.ReadCommitted)]
	public class GeneralConfigProcessorClass : IGeneralConfigProcessor
	{
		#region Private data members
		private Logger logger;
		private ConsolidatedDAClass consolidatedDA;
		#endregion

		#region Constructors
		/// <summary>
		/// This is the default constructor for the General Configuration Processor Class.
		/// </summary>
		public GeneralConfigProcessorClass()
		{
			this.consolidatedDA = new ConsolidatedDAClass();
			this.logger = new Logger("General Configuration Processor");
		}
		#endregion

		/// <summary>
		/// This method retrieves the general configuration data.
		/// </summary>
		/// <param name="generalConfigSR"></param>
		/// <returns></returns>
		public GeneralConfigDO Get(GeneralConfigSR generalConfigSR)
		{
			GeneralConfigDO generalConfigDO = null;

			switch (generalConfigSR.Request)
			{
				case GeneralConfigSR.GeneralConfigurationRequests.GET_CONFIGURATION:
					generalConfigDO = this.GetConfiguration(generalConfigSR);
					break;

				case GeneralConfigSR.GeneralConfigurationRequests.GET_CONFIGURATION_EXCLUDE_ALIASES:
					generalConfigDO = this.GetConfiguration(generalConfigSR);
					break;


				default:
					throw new AccountingServicesException("GeneralConfigSR.Request is not specified.");
			}

			return generalConfigDO;
		}

		/// <summary>
		/// This method saves the general configuration data.
		/// </summary>
		/// <param name="generalConfigSR"></param>
		/// <returns></returns>
		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void Save(GeneralConfigSR generalConfigSR)
		{
			switch (generalConfigSR.Request)
			{
				case GeneralConfigSR.GeneralConfigurationRequests.SAVE_CONFIGURATION:
					this.SaveConfiguration(generalConfigSR);
					break;

				default:
					throw new AccountingServicesException("GeneralConfigSR.Request is not specified.");
			}
		}

		/// <summary>
		/// This method saves the general configuration data.
		/// </summary>
		/// <param name="generalConfigSR"></param>
		/// <returns></returns>
		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void Purge(GeneralConfigSR generalConfigSR)
		{
			switch (generalConfigSR.Request)
			{
				case GeneralConfigSR.GeneralConfigurationRequests.PURGE:
					this.PurgeConfiguration(generalConfigSR);
					break;

				default:
					throw new AccountingServicesException("GeneralConfigSR.Request is not specified.");
			}
		}




		/// <summary>
		/// The get assembly version.
		/// </summary>
		/// <returns>
		/// The <see cref="string"/>.
		/// </returns>
		public string GetAssemblyFileVersion()
		{
			var assembly = Assembly.GetExecutingAssembly();
			var fileVersionInfo = FileVersionInfo.GetVersionInfo(assembly.Location);
			return fileVersionInfo.FileVersion;
		}

		#region Private Methods
		/// <summary>
		/// This method will determine whether to perform an insert or update.  If the
		/// GeneralConfigurationGuid is empty, then this indicates an insert.
		/// </summary>
		private void SaveConfiguration(GeneralConfigSR generalConfigSR)
		{
			GeneralConfigDO generalConfigDO = generalConfigSR.GeneralConfigurationDO;

			if (generalConfigDO.GeneralConfigurationGuid == Guid.Empty)
			{
				this.InsertConfiguration(generalConfigSR.Security, generalConfigDO, generalConfigSR);
			}
			else
			{
				this.UpdateConfiguration(generalConfigSR.Security, generalConfigDO, generalConfigSR);
			}
		}

		/// <summary>
		/// This method will insert general configuration data and the associated assigned
		/// adjustment aliases.
		/// </summary>
		private void InsertConfiguration(SecurityClass security, GeneralConfigDO generalConfigDO, GeneralConfigSR generalConfigSR)
		{
			generalConfigDO.CreatedBy = security.UserID;
			generalConfigDO.UpdatedBy = security.UserID;
			generalConfigDO.GeneralConfigurationGuid = Guid.NewGuid();

			try
			{
				using (SqlCommand cmd = new SqlCommand())
				{
					generalConfigDO.InsertGeneralConfigSQL(cmd);
					consolidatedDA.ExecuteQuery(security, cmd);
				}

				try
				{
					// Insert the associated assigned adjustment aliases.
					foreach (GeneralConfigAlias generalConfigAliasDO in generalConfigDO.AdjustmentAliasList)
					{
						generalConfigAliasDO.GeneralConfigurationGuid = generalConfigDO.GeneralConfigurationGuid;

						using (SqlCommand cmd = new SqlCommand())
						{
							generalConfigAliasDO.InsertGeneralConfigAliasSQL(cmd);
							this.consolidatedDA.ExecuteQuery(security, cmd);
						}
					}
				}
				catch (Exception ex)
				{
					this.logger.Debug("Error in inserting General Configuration assigned Alias Data. " + ex);
					throw ex;
				}
			}
			catch (Exception ex)
			{
				this.logger.Debug("Error in inserting General Configuration Data. " + ex);
				throw ex;
			}
		}

		/// <summary>
		/// This method will update the general configuration data and the associated assigned
		/// adjustment aliases.
		/// </summary>
		private void UpdateConfiguration(SecurityClass security, GeneralConfigDO generalConfigDO, GeneralConfigSR generalConfigSR)
		{
			generalConfigDO.UpdatedBy = security.UserID;

			try
			{
				using (SqlCommand cmd = new SqlCommand())
				{
					generalConfigDO.UpdatedGeneralConfigSQL(cmd);
					this.consolidatedDA.ExecuteQuery(security, cmd);
				}

				try
				{
					// Insert the associated assigned adjustment aliases.
					foreach (GeneralConfigAlias generalConfigAliasDO in generalConfigDO.AdjustmentAliasList)
					{
						// Delete the assignment if the delete flag is set to true.
						// Else, update the data.
						if (generalConfigAliasDO.DeleteFlag == true)
						{
							using (SqlCommand cmd = new SqlCommand())
							{
								generalConfigAliasDO.DeleteGeneralConfigAliasSQL(cmd);
								this.consolidatedDA.ExecuteQuery(security, cmd);
							}
						}
						else
						{
							// Insert a new aliase if the GeneralConfigurationAliasGuid is empty.
							// Else, udpate the record.
							if (generalConfigAliasDO.GeneralConfigurationAliasGuid == Guid.Empty)
							{
								generalConfigAliasDO.GeneralConfigurationGuid = generalConfigDO.GeneralConfigurationGuid;

								using (SqlCommand cmd = new SqlCommand())
								{
									generalConfigAliasDO.InsertGeneralConfigAliasSQL(cmd);
									this.consolidatedDA.ExecuteQuery(security, cmd);
								}
							}
							else
							{
								using (SqlCommand cmd = new SqlCommand())
								{
									generalConfigAliasDO.UpdateGeneralConfigAliasSQL(cmd);
									this.consolidatedDA.ExecuteQuery(security, cmd);
								}
							}
						}
					}
				}
				catch (Exception ex)
				{
					this.logger.Debug("Error in inserting General Configuration assigned Alias Data. " + ex);
					throw ex;
				}
			}
			catch (Exception ex)
			{
				this.logger.Debug("Error in inserting General Configuration Data. " + ex);
				throw ex;
			}
		}

		/// <summary>
		/// This method will get the general configuration data from the database.  It will load the
		/// assigned adjustment aliases also.
		/// </summary>
		private GeneralConfigDO GetConfiguration(GeneralConfigSR generalConfigSR)
		{
			GeneralConfigDO generalConfigDO = new GeneralConfigDO();
			Guid siteGuid;

			if (generalConfigSR.SiteGuid != Guid.Empty)
			{
				siteGuid = generalConfigSR.SiteGuid;
			}
			else
			{
				siteGuid = generalConfigSR.Security.SiteGuid;
			}

			try
			{

				DataSet dataSet = null;

				using (SqlCommand cmd = new SqlCommand())
				{
					generalConfigDO.GetGeneralConfigSQL(cmd, siteGuid);
					dataSet = this.consolidatedDA.GetDataSet(cmd, generalConfigSR.Security);
				}

				generalConfigDO.LoadGeneralConfigSQL(dataSet);

				HardwareKeyClass hardwareKeyClass = new HardwareKeyClass();
				if (hardwareKeyClass.IsDescKey())
					generalConfigDO.ReverseTransactionDateMode = "Original";


				if (generalConfigSR.Request == GeneralConfigSR.GeneralConfigurationRequests.GET_CONFIGURATION)
				{
					GeneralConfigAlias generalConfigAliasDO = new GeneralConfigAlias();

					try
					{
						using (SqlCommand cmd = new SqlCommand())
						{
							generalConfigAliasDO.GetGeneralConfigAliasSQL(cmd, generalConfigDO.GeneralConfigurationGuid);
							dataSet = this.consolidatedDA.GetDataSet(cmd, generalConfigSR.Security);
						}

						generalConfigDO.LoadGeneralConfigurationAlias(dataSet);

						// Retrieve the available transaction aliases so that the user will have a 
						// list to choose from.
						this.GetAvailableTransactionAliases(generalConfigSR.Security, generalConfigDO);
					}
					catch (Exception ex)
					{
						this.logger.Debug("Error in retrieving General Configuration aliases. " + ex);
						throw ex;
					}
				}
			}
			catch (Exception ex)
			{
				this.logger.Debug("Error in retrieving General Configuration Data. " + ex);
				throw ex;
			}

			return generalConfigDO;
		}

		/// <summary>
		/// This method will retrieve all the possible aliases for the given site. It will
		/// not add to the list any aliases that have been assigned.
		/// </summary>
		private void GetAvailableTransactionAliases(SecurityClass security, GeneralConfigDO generalConfigDO)
		{
			TransactionAliasesClass aliasesProcessor = new TransactionAliasesClass();
			List<TransactionAliasClass> transAliasList = aliasesProcessor.Enumerate(security);

			foreach (TransactionAliasClass transAlias in transAliasList)
			{
				bool addToList = true;

				foreach (GeneralConfigAlias genConfigAliasDO in generalConfigDO.AdjustmentAliasList)
				{
					if (transAlias.IdentityGuid == genConfigAliasDO.TransactionAliasGuid)
					{
						addToList = false;
						break;
					}
				}

				if (addToList == true)
				{
					DropdownValuePairDO pair = new DropdownValuePairDO();
					pair.Text = transAlias.ID;
					pair.TextValue = transAlias.MasterRecordGuid.ToString();
					generalConfigDO.UnassignedAliasList.Add(pair);
				}
			}
		}

		/// <summary>
		/// This method will perform a purge.  If the
		/// </summary>
		private void PurgeConfiguration(GeneralConfigSR generalConfigSR)
		{
			GeneralConfigDO generalConfigDO = generalConfigSR.GeneralConfigurationDO;

			generalConfigDO.UpdatedBy = generalConfigSR.Security.UserID;

			try
			{
				using (SqlCommand cmd = new SqlCommand())
				{
					generalConfigDO.PurgeGeneralConfigSQL(cmd);
					this.consolidatedDA.ExecuteQuery(generalConfigSR.Security, cmd);
				}
			}
			catch (Exception ex)
			{
				this.logger.Debug("Error in Purge General Configuration. " + ex);
				throw ex;
			}
		}




		#endregion
	}

}
