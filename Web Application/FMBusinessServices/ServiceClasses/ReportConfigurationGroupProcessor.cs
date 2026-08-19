/// <summary>
/// File name:	ReportConfigurationGroupProcessor.cs
/// Purpose:	Handles the report configuration group data. It will retrieve, delete, get, and
///				get all requests.
///	Comments:	Copyright (C) E+H Systems & Gauging, Inc. Norcross, GA, USA, 
///				2000.  This file shall not be copied or reproduced in any form 
///				without the express written consent of Endress+Hauser.
///	Author(s):	Richard R. Panachida
///	Version:	1.0.0  Current version
///	
///	Modification History:
///		Date:			   By:						Reason:
///		----------		--------------------	----------------------------------
///		2009-03-05     Richard Panachida    Defect 877: Added code to handle if a user does not have finance rights.
///		
///		2012-02-07		Brian Main				Converted SQL statements to use SqlCommand object and parameters.
/// </summary>

namespace FMBusinessServices.ServiceClasses
{
	using System;
	using System.Collections.Generic;
	using System.Data;
	using System.Data.SqlClient;
	using System.ServiceModel;
	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.ServiceRequests;
	using FMBusinessServices.DataAccessLayer;
	using FMBusinessServices.InternalClasses;

	[ServiceBehavior(TransactionIsolationLevel = System.Transactions.IsolationLevel.ReadCommitted)]
	public class ReportConfigurationGroupProcessorClass : IReportConfigurationGroupProcessor, IDependency
	{
		#region Attributes

		private AccountingSite accountingSite;
		private ConsolidatedDAClass consolidatedDA;

		#endregion

		#region Constructors

		/// <summary>
		/// This is the default constructor for the report configuration group processor class.
		/// It must initialize the accounting service implementation class in the base class
		/// and set the request command with the service request string.
		/// </summary>
		public ReportConfigurationGroupProcessorClass()
		{
			this.accountingSite = new AccountingSite();
			this.consolidatedDA = new ConsolidatedDAClass();
		}

		#endregion

		#region Public Methods
		/// <summary>
		/// This method will delete one report group record from the database.  If there is an error,
		/// then the error object is set.
		/// </summary>
		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void Delete(ReportConfigurationGroupSR reportGroupSR)
		{
			string errMsg = "";
			ReportConfigurationGroupDO groupDO = reportGroupSR.ReportConfigurationGroupDO;

			if (groupDO == null)
			{
				errMsg = "The record to be deleted had a null object reference!";
				throw new Exception(errMsg);
			}
			else
			{
				// Remove the report group record from the database.
				try
				{
					// Set all report detail record group guid values that were linked to the 
					// report group being deleted to not being associated (Guid.empty).
					ReportConfigurationDetailDO detailDO = new ReportConfigurationDetailDO();
					
					using (SqlCommand cmd = new SqlCommand())
					{
						detailDO.SQLUpdateGroupGuid(cmd, groupDO.ReportGroupGuid, Guid.Empty);
						consolidatedDA.ExecuteQuery(reportGroupSR.Security, cmd);
					}
					
			using (SqlCommand cmd = new SqlCommand())
					{
						groupDO.SQLDeleteReportGroup(cmd);
						consolidatedDA.ExecuteQuery(reportGroupSR.Security, cmd);
					}
				}
				catch (Exception ex)
				{
                    //errMsg = "Could not delete record with group guid of " + groupDO.ReportGroupGuid.ToString() + "!  " + ex.ToString();
                    errMsg = "Entity is being referenced and cannot be deleted.";
                    throw new Exception(errMsg, ex);
                }
			}
		}

		/// <summary>
		/// This method will handle the retrieval of one report group configuration record from the database.
		/// The key used to retrieve the record is the group guid. This method will return either the report
		/// group configuration data object or the error object.
		/// </summary>
		/// <returns></returns>
		public ReportConfigurationGroupDO GetConfiguration(ReportConfigurationGroupSR reportGroupSR)
		{
			string errMsg = "";
			ReportConfigurationGroupDO groupDO = reportGroupSR.ReportConfigurationGroupDO;

			if (groupDO == null)
			{
				errMsg = "The record to be retrieved had a null object reference!";
				throw new Exception(errMsg);
			}
			else
			{
				try
				{
					DataSet dataSet = null;
					using (SqlCommand cmd = groupDO.SQLGetReportGroup(groupDO.ReportGroupGuid))
					{
						dataSet = this.consolidatedDA.GetDataSet(cmd, reportGroupSR.Security);
					}
					groupDO.SQLLoadReportGroup(dataSet, reportGroupSR.Security);
				}
				catch (Exception ex)
				{
					errMsg = "Could not retrieve record with group guid of " + groupDO.ReportGroupGuid.ToString() + "!  " + ex.ToString();
					throw new Exception(errMsg);
				}
			}

			return groupDO;
		}

		public ReportConfigurationGroupDO GetByName(ReportConfigurationGroupSR reportGroupSR)
		{
			string errMsg = "";
			ReportConfigurationGroupDO groupDO = reportGroupSR.ReportConfigurationGroupDO;

			if (groupDO == null)
			{
				errMsg = "The record to be retrieved had a null object reference!";
				throw new Exception(errMsg);
			}
			else
			{
				try
				{
					groupDO.SiteGuid = reportGroupSR.CurrentSiteGuid;
					DataSet dataSet = null;
					using (SqlCommand cmd = groupDO.SQLGetReportGroup(groupDO.GroupName))
					{
						dataSet = this.consolidatedDA.GetDataSet(cmd, reportGroupSR.Security);
					}
					groupDO.SQLLoadReportGroup(dataSet, reportGroupSR.Security);
				}
				catch (Exception ex)
				{
					errMsg = "Could not retrieve record with group name of " + groupDO.GroupName + "!  " + ex.ToString();
					throw new Exception(errMsg);
				}
			}

			return groupDO;
		}

		/// <summary>
		/// This method will retrieve all the report group configuration records for a given site. It will
		/// return either the report all group configuration object or the error object.
		/// </summary>
		/// <returns></returns>
		public ReportConfigurationGroupListDO GetAll(ReportConfigurationGroupSR reportGroupSR)
		{
			string errMsg = "";
			ReportConfigurationGroupListDO groupListDO = new ReportConfigurationGroupListDO();

			try
			{
				DataSet dataSet = null;
				using (SqlCommand cmd = groupListDO.SQLGetAllReportGroups(reportGroupSR.CurrentSiteGuid))
				{
					dataSet = this.consolidatedDA.GetDataSet(cmd, reportGroupSR.Security);
				}
				groupListDO.SQLLoadAllReportGroups(dataSet, reportGroupSR.Security);
			}
			catch (Exception ex)
			{
				errMsg = "Could not retrieve all report group records with site guid of " +
											 reportGroupSR.CurrentSiteGuid.ToString() + "!  " + ex.ToString();
				throw new Exception(errMsg);
			}

			return groupListDO;
		}

		/// <summary>
		/// This method will either insert or update a report group configuration record depending on the
		/// group guid value. If the group guid is Guid.Empty, then it will be an insert, else, it will be an
		/// update.
		/// </summary>
		/// <returns></returns>
		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void Save(ReportConfigurationGroupSR reportGroupSR)
		{
			string errMsg = "";
			ReportConfigurationGroupDO groupDO = reportGroupSR.ReportConfigurationGroupDO;

			if (groupDO == null)
			{
				errMsg = "The record to be inserted/updated had a null object reference!";
				throw new Exception(errMsg);
			}
			else
			{
				try
				{
					// Insure the Group Name isn't in use
					DataSet dataSet = null;
					using (SqlCommand cmd = groupDO.SQLGetReportGroup(groupDO.GroupName))
					{
						dataSet = this.consolidatedDA.GetDataSet(cmd, reportGroupSR.Security);
					}

					ReportConfigurationGroupDO existingGroupDO = new ReportConfigurationGroupDO();

					if ((existingGroupDO.SQLLoadReportGroup(dataSet, reportGroupSR.Security))
						&& (existingGroupDO.ReportGroupGuid != groupDO.ReportGroupGuid))
					{
						errMsg = "The record to be inserted/updated is a duplicate!";
						throw new Exception(errMsg);
					}

					// Perform an insert if the group guid is Guid.Empty Else, perform
					// an update.
					if (groupDO.ReportGroupGuid == Guid.Empty)
					{
						groupDO.CreatedBy = reportGroupSR.Security.UserID;
						
						using (SqlCommand cmd = new SqlCommand())
						{
							groupDO.SQLInsertReportGroup(cmd);
							consolidatedDA.ExecuteQuery(reportGroupSR.Security, cmd);
						}

					}
					else
					{
						groupDO.UpdatedBy = reportGroupSR.Security.UserID;
						
						using (SqlCommand cmd = new SqlCommand())
						{
							groupDO.SQLUpdateReportGroup(cmd);
							consolidatedDA.ExecuteQuery(reportGroupSR.Security, cmd);
						}
					}
				}
				catch (Exception ex)
				{
					errMsg = "Could not insert/update report group record with group guid of " +
												 groupDO.ReportGroupGuid.ToString() + "!  " + ex.ToString();
					throw new Exception(errMsg);
				}
			}
		}

		/// <summary>
		/// This method will update all report group configuration order number records with the new
		/// order number.
		/// </summary>
		/// <returns></returns>
		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void UpdateOrder(ReportConfigurationGroupSR reportGroupSR)
		{
			string errMsg = "";
			List<ReportConfigurationGroupDO> groupList = reportGroupSR.ReportGroupList;

			// Loop through all the groups and update their order number.
			foreach (ReportConfigurationGroupDO groupDO in groupList)
			{
				if (groupDO == null)
				{
					errMsg = "The record to be inserted/updated had a null object reference!";
					throw new Exception(errMsg);
				}
				else
				{
					try
					{
						groupDO.UpdatedBy = reportGroupSR.Security.UserID;
						
						using (SqlCommand cmd = new SqlCommand())
						{
							groupDO.SQLUpdateReportGroupOrder(cmd, groupDO.ReportGroupGuid, groupDO.OrderNumber);
							consolidatedDA.ExecuteQuery(reportGroupSR.Security, cmd);
						}
					
					}
					catch (Exception ex)
					{
						errMsg = "Could not update report group order number with group guid of " +
													 groupDO.ReportGroupGuid.ToString() + "!  " + ex.ToString();
						throw new Exception(errMsg);
					}
				}
			}
		}
		#endregion

		#region IDependency

		/// <summary>
		/// When an entity to site map record is created for report configuration during entity assignment,
		/// check to see if any report groups are configured for the site.
		/// If report groups are already configured for the site then you may not assign the report configuration (groups + reports) to the site
		/// </summary>
		/// <param name="security">Contains security information</param>
		/// <param name="dataObject">The object being inserted, in our case we only care about EntityToSiteMap records</param>
		/// <param name="preOperation">True if this method is being called before the insert of the entityToSiteMap record.</param>
		void IDependency.Insert(SecurityClass security, BaseDataObject dataObject, bool preOperation)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (dataObject == null)
			{
				throw new ArgumentNullException("dataObject");
			}

			// We only apply the checks when this is occuring before the insert of an EntityToSiteMap record
			if (!preOperation || !(dataObject is EntityToSiteMapClass))
			{
				return;
			}

			EntityToSiteMapClass entityToSiteMap = dataObject as EntityToSiteMapClass;

			// The EntityToSiteMap record must be the REPORT_CONFIGURATION_SETTINGS type, otherwise, we don't need to do anything 
			if (entityToSiteMap.TypeID != ENTITY_TYPE.REPORT_CONFIGURATION_SETTINGS)
			{
				return;
			}

			// Check to see if any report groups are configured for the site
			ReportConfigurationGroupSR reportGroupSR = new ReportConfigurationGroupSR
			{
				CurrentSiteGuid = security.SiteGuid,
				RequestType = ReportConfigurationGroupSR.RequestTypes.GET_ALL,
				Security = security
			};

			ReportConfigurationGroupListDO reportGroupList =
				FMChannelHelper.MakeCall<IReportConfigurationGroupProcessor, ReportConfigurationGroupListDO>(reportGroups => reportGroups.GetAll(reportGroupSR));

			if (reportGroupList.ReportGroupDOList.Count > 0)
			{
				throw new Exception("You may not assign report groups to the site because report groups are already configured for the site.");
			}
		}

		/// <summary>
		/// This method is implemented because we implement IDependency
		/// </summary>
		/// <param name="security">The parameter is not used.</param>
		/// <param name="dataObject">The parameter is not used.</param>
		void IDependency.Update(SecurityClass security, BaseDataObject dataObject)
		{
		}

		/// <summary>
		/// This method is implemented because we implement IDependency
		/// </summary>
		/// <param name="security">The parameter is not used.</param>
		/// <param name="dataObject">The parameter is not used.</param>
		void IDependency.Purge(SecurityClass security, BaseDataObject dataObject)
		{
		}

		#endregion
	}
}