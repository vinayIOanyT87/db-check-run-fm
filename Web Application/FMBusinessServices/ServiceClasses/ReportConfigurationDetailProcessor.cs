/// <summary>
/// File name:	ReportConfigurationDetailProcessor.cs
/// Purpose:	Handles the report configuration detail data. It will retrieve, delete, get, and
///				get all requests.
///	Comments:	Copyright (C) E+H Systems & Gauging, Inc. Norcross, GA, USA, 
///				2000.  This file shall not be copied or reproduced in any form 
///				without the express written consent of Endress+Hauser.
///	Author(s):	Richard R. Panachida
///	Version:	1.0.0  Current version
///	
///	Modification History:
///		Date:			By:						Reason:
///		----------		--------------------	----------------------------------
///		2005/7/15		Richard Panachida		Added feature to auto print reports
///		
///		2012-02-07		Brian Main				Converted SQL statements to SqlCommand objects and parameters.
/// </summary>

namespace FMBusinessServices.ServiceClasses
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Data;
	using System.Data.SqlClient;
	using System.ServiceModel;
	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;
    using FMBusinessObjects.Exceptions;
    using FMBusinessObjects.ServiceRequests;
	using FMBusinessServices.DataAccessLayer;
	using FMBusinessServices.InternalClasses;

	[ServiceBehavior(TransactionIsolationLevel = System.Transactions.IsolationLevel.ReadCommitted)]
	public class ReportConfigurationDetailProcessorClass : IReportConfigurationDetailProcessor, IDependency
	{
		#region Attributes

		private AccountingSite accountingSite;
		private ConsolidatedDAClass consolidatedDA;

		#endregion

		#region Constructors
		/// <summary>
		/// This is the default constructor for the report configuration detail processor class.
		/// It must initialize the accounting service implemention class in the base class
		/// and set the request command with the service request string.
		/// </summary>
		public ReportConfigurationDetailProcessorClass ( )
		{
			this.accountingSite = new AccountingSite ( );
			this.consolidatedDA = new ConsolidatedDAClass ( );
		}
		#endregion

		#region Public Methods
		/// <summary>
		/// This method will delete one report detail record from the database.  If there is an error,
		/// then the error object is set.
		/// </summary>
		[OperationBehavior ( TransactionScopeRequired = true, TransactionAutoComplete = true )]
		public void Delete ( ReportConfigurationDetailSR rptDetailSR )
		{
			string errMsg = "";
			ReportConfigurationDetailDO detailDO = rptDetailSR.ReportConfigurationDetailDO;

			if (detailDO == null)
			{
				errMsg = "The record to be deleted had a null object reference!";
				throw new Exception ( errMsg );
			}
			else
			{
				// Remove the report detail record from the database.
				try
				{

					using (SqlCommand cmd = ReportDetailUserGroupMapDO.SQLEnumerate ( detailDO.ReportGuid ) )
					{
						detailDO.UserGroupMap = ReportDetailUserGroupMapDO.SQLLoadReportDetailUserGroupMap ( this.consolidatedDA.GetDataSet( cmd, rptDetailSR.Security) );
					}

					foreach (ReportDetailUserGroupMapDO reportDetailUserGroupMapDO in detailDO.UserGroupMap)
					{
						using (SqlCommand cmd = new SqlCommand())
						{
							reportDetailUserGroupMapDO.SQLDelete(cmd, detailDO.ReportGuid);
							consolidatedDA.ExecuteQuery(rptDetailSR.Security, cmd);
						}
					
					}

					using (SqlCommand cmd = new SqlCommand())
					{
						detailDO.SQLDeleteReportDetail(cmd);
						consolidatedDA.ExecuteQuery(rptDetailSR.Security, cmd);
					}
				}
				catch (Exception ex)
				{
					errMsg = "Could not delete record with detail guid of " + detailDO.ReportGuid.ToString() + "!  " + ex.ToString();
					throw new Exception ( errMsg );
				}
			}
		}

		/// <summary>
		/// This method will handle the retrieval of one report detail configuration record for a print type of
		/// report from the database.  The key used to retrieve the record is the detail report name. This method 
		/// will return either the report detail configuration data object or the error object.
		/// </summary>
		/// <returns></returns>
		public ReportConfigurationDetailDO GetPrintType ( ReportConfigurationDetailSR rptDetailSR )
		{
			string errMsg = "";
			ReportConfigurationDetailDO detailDO = rptDetailSR.ReportConfigurationDetailDO;

			if (detailDO == null)
			{
				errMsg = "The record to be retrieved had a null object reference!";
				throw new Exception ( errMsg );
			}
			else
			{
				try
				{
					DataSet dataSet = null;
					using (SqlCommand cmd = detailDO.SQLGetPrintReportDetail ( detailDO.ReportName ))
					{
						dataSet = this.consolidatedDA.GetDataSet( cmd, rptDetailSR.Security);
					}
					detailDO.SQLLoadReportDetail ( dataSet );
				}
				catch (Exception ex)
				{
					errMsg = "Could not retrieve record with detail guid of " + detailDO.ReportGuid.ToString() + "!  " + ex.ToString();
					throw new Exception ( errMsg );
				}
			}

			return detailDO;
		}

		/// <summary>
		/// This method will handle the retrieval of one report detail configuration record from the database.
		/// The key used to retrieve the record is the detail guid. This method will return the report
		/// detail configuration data object.
		/// </summary>
		/// <returns></returns>
		public ReportConfigurationDetailDO GetConfiguration ( ReportConfigurationDetailSR rptDetailSR )
		{
			string errMsg = "";
			ReportConfigurationDetailDO detailDO = rptDetailSR.ReportConfigurationDetailDO;

			if (detailDO == null)
			{
				errMsg = "The record to be retrieved had a null object reference!";
				throw new Exception ( errMsg );
			}
			else
			{
				try
				{
					DataSet dataSet = null;
					using (SqlCommand cmd = detailDO.SQLGetReportDetail( detailDO.ReportGuid))
					{
						dataSet = this.consolidatedDA.GetDataSet ( cmd, rptDetailSR.Security);
					}
					detailDO.SQLLoadReportDetail ( dataSet );
					using (SqlCommand cmd = ReportDetailUserGroupMapDO.SQLEnumerate(detailDO.ReportGuid))
					{
						detailDO.UserGroupMap = ReportDetailUserGroupMapDO.SQLLoadReportDetailUserGroupMap ( this.consolidatedDA.GetDataSet ( cmd, rptDetailSR.Security ) );
					}
				}
				catch (Exception ex)
				{
					errMsg = "Could not retrieve record with detail guid of " + detailDO.ReportGuid.ToString() + "!  " + ex.ToString();
					throw new Exception ( errMsg );
				}
			}

			return detailDO;
		}

		/// <summary>
		/// This method will retrieve all the report detail configuration records for a given site. It will
		/// return the report all detail configuration object.
		/// </summary>
		/// <returns></returns>
		public ReportConfigurationDetailListDO GetAll ( ReportConfigurationDetailSR rptDetailSR)
		{
			string errMsg = "";
			ReportConfigurationDetailListDO detailListDO = new ReportConfigurationDetailListDO ( );

			try
			{
				DataSet dataSet = null;
                switch (rptDetailSR.RequestType)
                {
                    case ReportConfigurationDetailSR.RequestTypes.GET_PRINT_AT_END_OF_DAY_TYPE:
						using (SqlCommand cmd = detailListDO.SQLGetAllPrintAtEndOfDayReportDetails(rptDetailSR.CurrentSiteGuid))
						{
							dataSet = this.consolidatedDA.GetDataSet(cmd, rptDetailSR.Security);
							detailListDO.SQLLoadAllReportDetails(dataSet);
						}
						break;
                    case ReportConfigurationDetailSR.RequestTypes.GET_PRINT_AT_END_OF_MONTH_TYPE:
						using (SqlCommand cmd = detailListDO.SQLGetAllPrintAtEndOfMonthReportDetails(rptDetailSR.CurrentSiteGuid))
						{
							dataSet = this.consolidatedDA.GetDataSet(cmd, rptDetailSR.Security);
							detailListDO.SQLLoadAllReportDetails(dataSet);
						}
						break;
					default:
						using (SqlCommand cmd = detailListDO.SQLGetAllReportDetails(rptDetailSR.CurrentSiteGuid))
						{
							dataSet = this.consolidatedDA.GetDataSet(cmd, rptDetailSR.Security);
							detailListDO.SQLLoadAllReportDetails(dataSet);
						}
						break;
                }

			}
			catch (Exception ex)
			{
				errMsg = "Could not retrieve all report detail records with site guid of " +
							rptDetailSR.CurrentSiteGuid.ToString() + "!  " + ex.ToString();
				throw new Exception ( errMsg );
			}

			return detailListDO;
		}

		/// <summary>
		/// This method will retrieve all the report detail configuration non-print records for a given site. It will
		/// return the report all detail configuration object.
		/// </summary>
		/// <returns></returns>
		public ReportConfigurationDetailListDO GetAllNonPrint ( ReportConfigurationDetailSR rptDetailSR )
		{
			string errMsg = "";
			ReportConfigurationDetailListDO detailListDO = new ReportConfigurationDetailListDO ( );

			try
			{
				DataSet dataSet = null;
				using (SqlCommand cmd = detailListDO.SQLGetAllNonPrintReportDetails( rptDetailSR.CurrentSiteGuid, rptDetailSR.Security.UserGuid))
				{
					dataSet = this.consolidatedDA.GetDataSet( cmd, rptDetailSR.Security);
				}
				detailListDO.SQLLoadAllNonPrintReportDetails ( dataSet );
			}
			catch (Exception ex)
			{
				errMsg	= "Could not retrieve all the non-print type report detail records with site guid of " +
							rptDetailSR.CurrentSiteGuid.ToString() + "!  " + ex.ToString ( );
				throw new Exception ( errMsg );
			}

			return detailListDO;
		}

		/// <summary>
		/// This method will retrieve all the report detail configuration print at EndOfDay records for a given site. It will
		/// return either the report all detail configuration object or the error object.
		/// </summary>
		/// <returns></returns>
		public ReportConfigurationDetailListDO GetPrintAtEndOfDay ( ReportConfigurationDetailSR rptDetailSR )
		{
			string errMsg = "";
			ReportConfigurationDetailListDO detailListDO = new ReportConfigurationDetailListDO ( );

			try
			{
				DataSet dataSet = null;
				using (SqlCommand cmd = detailListDO.SQLGetAllPrintAtEndOfDayReportDetails ( rptDetailSR.CurrentSiteGuid ) )
				{
					dataSet = this.consolidatedDA.GetDataSet(cmd, rptDetailSR.Security);
				}													
				detailListDO.SQLLoadAllNonPrintReportDetails ( dataSet );
			}
			catch (Exception ex)
			{
				errMsg	= "Could not retrieve all the print at EndOfDay type report detail records with site guid of " +
							rptDetailSR.CurrentSiteGuid.ToString() + "!  " + ex.ToString();
				throw new Exception ( errMsg );
			}

			return detailListDO;
		}


		/// <summary>
		/// This method will retrieve all the report detail configuration print at EndOfMonth records for a given site. It will
		/// return either the report all detail configuration object or the error object.
		/// </summary>
		/// <returns></returns>
		public ReportConfigurationDetailListDO GetPrintAtEndOfMonth ( ReportConfigurationDetailSR rptDetailSR )
		{
			string errMsg = "";
			ReportConfigurationDetailListDO detailListDO = new ReportConfigurationDetailListDO ( );

			try
			{
				DataSet dataSet = null;
				using (SqlCommand cmd = detailListDO.SQLGetAllPrintAtEndOfMonthReportDetails(rptDetailSR.CurrentSiteGuid))
				{
					dataSet = this.consolidatedDA.GetDataSet( cmd, rptDetailSR.Security);
				}
				detailListDO.SQLLoadAllNonPrintReportDetails ( dataSet );
			}
			catch (Exception ex)
			{
				errMsg	= "Could not retrieve all the print at EndOfDay type report detail records with site guid of " +
							rptDetailSR.CurrentSiteGuid.ToString() + "!  " + ex.ToString ( );
				throw new Exception ( errMsg );
			}

			return detailListDO;
		}

		/// <summary>
		/// This method will either insert or update a report detail configuration record depending on the
		/// detail guid value. If the detail guid is Guid.empty then it will be an insert, else, it will be an
		/// update.
		/// </summary>
		/// <returns></returns>
		[OperationBehavior ( TransactionScopeRequired = true, TransactionAutoComplete = true )]
		public void Save ( ReportConfigurationDetailSR rptDetailSR )
		{
			string errMsg = "";
			ReportConfigurationDetailDO detailDO = rptDetailSR.ReportConfigurationDetailDO;

			if (detailDO == null)
			{
				errMsg = "The record to be inserted/updated had a null object reference!";
				throw new Exception ( errMsg );
			}
			else
			{
				try
				{
					// Insure the Detail Name isn't in use
					detailDO.SiteGuid = rptDetailSR.Security.SiteGuid;
					DataSet dataSet = null;
					using (SqlCommand cmd = detailDO.SQLGetReportDetail(detailDO.ReportName))
					{
						dataSet = this.consolidatedDA.GetDataSet( cmd, rptDetailSR.Security);
					}
					ReportConfigurationDetailDO existingDetailDO = new ReportConfigurationDetailDO ( );

					if (existingDetailDO.SQLLoadReportDetail(dataSet) && existingDetailDO.ReportGuid != detailDO.ReportGuid)
					{
						errMsg	= "The record to be inserted/updated is a duplicate!";
						return;
					}

					// Perform an insert if the group guid is Guid.empty. Else, perform
					// an update.
					if (detailDO.ReportGuid == Guid.Empty)
					{
						detailDO.CreatedBy = rptDetailSR.Security.UserID;
						using (SqlCommand cmd = detailDO.SQLCmdInsertReportDetail())
						{
							this.consolidatedDA.ExecuteQuery(rptDetailSR.Security, cmd);
						}
						DataSet ds = null;
						using (SqlCommand cmd = detailDO.SQLGetReportDetail(detailDO.ReportName))
						{
							ds = this.consolidatedDA.GetDataSet( cmd, rptDetailSR.Security);
						}

						if (ds.Tables.Count == 1 && ds.Tables[0].Rows.Count == 1)
						{
							detailDO.ReportGuid = (Guid)ds.Tables[0].Rows[0][0];

							foreach (ReportDetailUserGroupMapDO reportDetailUserGroupMapDO in detailDO.UserGroupMap)
							{
								using (SqlCommand cmd = new SqlCommand())
								{
									reportDetailUserGroupMapDO.SQLInsert(cmd, detailDO.ReportGuid, rptDetailSR.Security.UserID);
									consolidatedDA.ExecuteQuery(rptDetailSR.Security, cmd);
								}
							
							}
						}
					}
					else
					{
						detailDO.UpdatedBy = rptDetailSR.Security.UserID;
						
						using (SqlCommand cmd = new SqlCommand())
						{
							detailDO.SQLUpdateReportDetail(cmd);
							consolidatedDA.ExecuteQuery(rptDetailSR.Security, cmd);
						}


						ArrayList existingUserGroupMap = null;
						using (SqlCommand cmd = ReportDetailUserGroupMapDO.SQLEnumerate(detailDO.ReportGuid))
						{

							// Get Existing UserGroupMap
							existingUserGroupMap = ReportDetailUserGroupMapDO.SQLLoadReportDetailUserGroupMap(this.consolidatedDA.GetDataSet(cmd, rptDetailSR.Security));
						}

						// Insert new UserGroupMap
						foreach (ReportDetailUserGroupMapDO reportDetailUserGroupMapDO in detailDO.UserGroupMap)
						{
							bool Found = false;
							foreach (ReportDetailUserGroupMapDO existingReportDetailUserGroupMapDO in existingUserGroupMap)
							{
								if (existingReportDetailUserGroupMapDO.GroupGuid == reportDetailUserGroupMapDO.GroupGuid)
								{
									existingUserGroupMap.Remove ( existingReportDetailUserGroupMapDO );
									Found = true;
									break;
								}
							}

							if (!Found)
							{
								using (SqlCommand cmd = new SqlCommand())
								{
									reportDetailUserGroupMapDO.SQLInsert(cmd, detailDO.ReportGuid, rptDetailSR.Security.UserID);
									consolidatedDA.ExecuteQuery(rptDetailSR.Security, cmd);
								}
							}
						}

						// remove Deleted UserGroupMap
						foreach (ReportDetailUserGroupMapDO existingReportDetailUserGroupMapDO in existingUserGroupMap)
						{
							using (SqlCommand cmd = new SqlCommand())
							{
								existingReportDetailUserGroupMapDO.SQLDelete(cmd, detailDO.ReportGuid);
								consolidatedDA.ExecuteQuery(rptDetailSR.Security, cmd);
							}
						}

					}
				}
				catch (Exception ex)
				{
					errMsg	= "Could not insert/update report detail record with detail guid of " +
								detailDO.ReportGuid.ToString() + "!  " + ex.ToString();
					throw new Exception ( errMsg );
				}
			}
		}

		/// <summary>
		/// This method will update the order numbers for all report detail records.
		/// </summary>
		/// <returns></returns>
		[OperationBehavior ( TransactionScopeRequired = true, TransactionAutoComplete = true )]
		public void UpdateOrder ( ReportConfigurationDetailSR rptDetailSR )
		{
			string errMsg = "";
			List<ReportConfigurationDetailDO> detailList = rptDetailSR.ReportConfigurationDetailList;

			// Loop through all of the report detail data objects and update their order
			// number.
			foreach (ReportConfigurationDetailDO detailDO in detailList)
			{
				// If the detail data object is null, create an error object and quit.
				if (detailDO == null)
				{
					errMsg = "The record to be updated had a null object reference!";
					throw new Exception ( errMsg );
				}
				else
				{
					try
					{
						detailDO.UpdatedBy = rptDetailSR.Security.UserID;

						using (SqlCommand cmd = new SqlCommand())
						{
							detailDO.SQLUpdateDetailOrder(cmd, detailDO.ReportGuid, detailDO.OrderNumber);
							consolidatedDA.ExecuteQuery(rptDetailSR.Security, cmd);
						}
					
					}
					catch (Exception ex)
					{
						errMsg = "Could not update report detail order number with detail guid of " +
								detailDO.ReportGuid.ToString() + "! " + ex.ToString();
						throw new Exception ( errMsg );
					}
				}
			}
		}

		public void CreateDefaultReportAssignments(SecurityClass security)
		{
			string returnMsg1 = String.Empty;
			string returnMsg2 = String.Empty;
			if (security == null)
			{
				throw new ArgumentNullException(nameof(security));
			}

			if (!security.HasRight(RIGHT.MODIFY_REPORTS))
			{
				throw new FMInsufficientRightsException();
			}

			ListViewClass listView = new ListViewClass { SiteGuid = security.SiteGuid };
			try
			{
				using (SqlCommand cmd = new SqlCommand())
				{
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.CommandText = "[dbo].[usp_AddDefaultReportConfiguration]";
					cmd.CommandTimeout = 0;
					cmd.Parameters.Add("@SiteGuid", SqlDbType.UniqueIdentifier);
					cmd.Parameters["@SiteGuid"].Value = security.SiteGuid;

					consolidatedDA.ExecuteQuery(security, cmd);
				}
			}
			catch (Exception)
			{
				throw new Exception("Cannot create the default report assignments" );
			}
			return;
		}

		#endregion

		#region IDependency

		/// <summary>
		/// When an entity to site map record is created for report configuration during entity assignment,
		/// check to see if any reports are configured for the site.
		/// If reports are already configured for the site then you may not assign the report configuration (groups + reports) to the site
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

			// Check to see if any reports are configured for the site
			ReportConfigurationDetailSR reportDetailSR = new ReportConfigurationDetailSR
			{
				CurrentSiteGuid = security.SiteGuid,
				RequestType = ReportConfigurationDetailSR.RequestTypes.GET_ALL,
				Security = security
			};

			ReportConfigurationDetailListDO reportDetailListDO =
				FMChannelHelper.MakeCall<IReportConfigurationDetailProcessor, ReportConfigurationDetailListDO>(reports => reports.GetAll(reportDetailSR));

			if (reportDetailListDO.ReportDetailDOList.Count > 0)
			{
				throw new Exception("You may not assign reports to the site because reports are already configured for the site.");
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
			if(dataObject is SiteClass)
            {
				var site = dataObject as SiteClass;

				var entityToSiteMaps = new EntityToSiteMaps();
				var entityToSiteMapCollection = entityToSiteMaps.EnumerateByTypeIDAndSiteGuid(security, ENTITY_TYPE.REPORT_CONFIGURATION_SETTINGS, site.SiteGuid);

				if (entityToSiteMapCollection != null
				&& entityToSiteMapCollection.Count > 0)
				{
					foreach (var entityToSiteMap in entityToSiteMapCollection)
					{
						entityToSiteMaps.Purge(security, entityToSiteMap);
					}
				}

				else
				{
					// Check to see if any reports are configured for the site
					var reportDetailSR = new ReportConfigurationDetailSR
					{
						CurrentSiteGuid = security.SiteGuid,
						RequestType = ReportConfigurationDetailSR.RequestTypes.GET_ALL,
						Security = security
					};

					var reportDetailListDO = this.GetAll(reportDetailSR);

					var reportGroupSR = new ReportConfigurationGroupSR
					{
						CurrentSiteGuid = security.SiteGuid,
						RequestType = ReportConfigurationGroupSR.RequestTypes.GET_ALL,
						Security = security
					};


					var reportGroups = new ReportConfigurationGroupProcessorClass();
					var reportConfigurationGroupListDO = reportGroups.GetAll(reportGroupSR);


					if (reportDetailListDO.ReportDetailDOList.Count > 0
					|| reportConfigurationGroupListDO.ReportGroupDOList.Count > 0)
					{
						reportDetailSR.RequestType = ReportConfigurationDetailSR.RequestTypes.DELETE;
						foreach (var reportDetailDO in reportDetailListDO.ReportDetailDOList)
						{
							reportDetailSR.ReportConfigurationDetailDO = reportDetailDO;
							this.Delete(reportDetailSR);
						}

						reportGroupSR.RequestType = ReportConfigurationGroupSR.RequestTypes.DELETE;
						foreach (var reportGroupDO in reportConfigurationGroupListDO.ReportGroupDOList)
						{
							reportGroupSR.ReportConfigurationGroupDO = reportGroupDO;
							reportGroups.Delete(reportGroupSR);
						}
					}
				}
			}
		}

		#endregion
	}
}