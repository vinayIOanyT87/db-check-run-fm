namespace FMBusinessServices.ServiceClasses
{
	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.Constants;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.DataObjects.CodedVariables;
	using FMBusinessObjects.ReportSvr2005;
	using FMBusinessObjects.UtilityObjects;
	using FMBusinessServices.DataAccessLayer;
	using System;
	using System.Collections.Generic;
	using System.Data;
	using System.Data.SqlClient;
	using System.Security;
	using System.ServiceModel;
	using System.Diagnostics;
	using Varec.CommonComponents.EngineeringUnitsLibrary;
	using System.IO;

	[SecuritySafeCriticalAttribute]
	[ServiceBehavior(TransactionIsolationLevel = System.Transactions.IsolationLevel.ReadCommitted)]
	public class MovementHistories : IMovementHistories
	{
		#region Data members
		internal ConsolidatedDAClass consolidatedDA = new ConsolidatedDAClass();
		const string MovementHistoryPrefix = "MovementHistoryService - ";
		private FMEventLog eventLog;
		#endregion

		#region Constructors
		/// <summary>
		/// This is the default constructor
		/// </summary>
		public MovementHistories()
		{
			this.Init();
		}
		#endregion

		#region Properties
		#endregion

		#region Public methods
		/// <summary>
		/// This method adds a new movement history record to the history table using the
		/// movement data.
		/// </summary>
		/// <param name="security">The security object.</param>
		/// <param name="movementData">The movement data from the points.</param>
		public Guid Add(SecurityClass security, MovementData movementData)
		{
			if (security == null)
			{
				throw new ArgumentNullException("Security");
			}

			if (movementData == null)
			{
				throw new ArgumentNullException("MovementData");
			}

			Guid movementHistoryGuid = Guid.Empty;

			// Map the data from the movement data to a DO for saving.
			List<MovementHistoryDO> movementHistoryDoList = this.MapData(ref movementData, security);

			if (movementHistoryDoList.Count > 0)
			{
				this.AddByList(security, movementHistoryDoList);
				movementHistoryGuid = movementHistoryDoList[0].MovementHistoryGuid;
			}

			return movementHistoryGuid;
		}

		/// <summary>
		/// This method adds a new movement history record to the history table.
		/// </summary>
		/// <param name="security">The security object.</param>
		/// <param name="movementHistoryDoList">The movement history records to save.</param>
		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void AddByList(SecurityClass security, List<MovementHistoryDO> movementHistoryDoList)
		{
			if (security == null)
			{
				throw new ArgumentNullException("Security");
			}

			if (movementHistoryDoList == null)
			{
				throw new ArgumentNullException("MovementHistoryDoList");
			}

			using (SqlCommand cmd = new SqlCommand())
			{
				foreach (MovementHistoryDO movementHistoryDo in movementHistoryDoList)
				{
					movementHistoryDo.SaveMovementHistorySql(cmd);
					this.consolidatedDA.ExecuteQuery(security, cmd);
					cmd.CommandText = string.Empty;
					cmd.Parameters.Clear();
				}
			}
		}

		/// <summary>
		/// This method updates a movement history record to the history table.
		/// </summary>
		/// <param name="security">The security object.</param>
		/// <param name="movementHistoryDo">The movement history record to save.</param>
		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void Modify(SecurityClass security, MovementHistoryDO movementHistoryDo)
		{
			if (security == null)
			{
				throw new ArgumentNullException("Security");
			}

			if (movementHistoryDo == null)
			{
				throw new ArgumentNullException("MovementHistoryDo");
			}

			if(movementHistoryDo.MovementHistoryGuid == Guid.Empty || movementHistoryDo.MovementHistoryGuid == Guid.Empty)
			{
				throw new ArgumentOutOfRangeException("Movement or Movement History Guid are empty.");
			}

			using (SqlCommand cmd = new SqlCommand())
			{
				movementHistoryDo.SiteGuid = security.SiteGuid;

				movementHistoryDo.SaveMovementHistorySql(cmd);
				this.consolidatedDA.ExecuteQuery(security, cmd);
			}
		}

		/// <summary>
		/// This method will update the hand gauge record and if the user indicated, the final
		/// record.
		/// </summary>
		/// <param name="security">The security object.</param>
		/// <param name="movementHistoryDo">The movement history record to update.</param>
		/// <param name="updateFinalRecord">Flag indicating whether to update the final record.</param>
		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void UpdateHandgaugeFromHistory(SecurityClass security, MovementHistoryDO movementHistoryDo, bool updateFinalRecord)
		{
			if (security == null)
			{
				throw new ArgumentNullException("Security");
			}

			if (movementHistoryDo == null)
			{
				throw new ArgumentNullException("MovementHistoryDo");
			}

			if (movementHistoryDo.MovementHistoryGuid == null || movementHistoryDo.MovementHistoryGuid == Guid.Empty)
			{
				throw new ArgumentOutOfRangeException("Movement History Guid is empty.");
			}

			using (SqlCommand cmd = new SqlCommand())
			{
				movementHistoryDo.SiteGuid = security.SiteGuid;

				movementHistoryDo.SaveMovementHistorySql(cmd);
				this.consolidatedDA.ExecuteQuery(security, cmd);
			}

			// Update the final record based on the user's request.
			if(updateFinalRecord)
			{
				DataSet dataSet = null;
				using (SqlCommand cmd = new SqlCommand())
				{
					movementHistoryDo.GetFinalRecordInfoAssociatedToHandgaugeSql(cmd, movementHistoryDo.RootParentGuid, movementHistoryDo.ParentGuid);
					dataSet = this.consolidatedDA.GetDataSet(cmd, security);
				}

				if(dataSet == null || dataSet.Tables.Count == 0 || dataSet.Tables[0].Rows.Count == 0)
				{
					string msg = MovementHistoryPrefix + "Could not Final record associated to Handgauge (" + movementHistoryDo.MovementHistoryGuid.ToString() + ").";
					this.eventLog.WriteEntry(msg, FMEventLogEntryType.Warning);
					return;
				}

				DataRow row                 = dataSet.Tables[0].Rows[0];
				Guid finalRecordGuid        = row.IsNull("MovementHistoryGuid") ? Guid.Empty : (Guid)row["MovementHistoryGuid"];
				int finalRecordSeq          = row.IsNull("RecordSeq") ? -99 : (int)row["RecordSeq"];
				string finalRecordNodeName  = row.IsNull("Node") ? string.Empty : (string)row["Node"];

				if (finalRecordGuid == Guid.Empty || finalRecordSeq == -99 || string.IsNullOrEmpty(finalRecordNodeName))
				{
					string msg = MovementHistoryPrefix + "Could not Final record associated to Handgauge (" + movementHistoryDo.MovementHistoryGuid.ToString() + ").";
					this.eventLog.WriteEntry(msg, FMEventLogEntryType.Warning);
					return;
				}

				movementHistoryDo.MovementHistoryGuid = finalRecordGuid;
				movementHistoryDo.RecordSeq = finalRecordSeq;
				movementHistoryDo.Node = finalRecordNodeName;
				movementHistoryDo.RecordType = MovementHistoryDO.MovementRecordTypes.Final;

				// Update the Final Record.
				using (SqlCommand cmd = new SqlCommand())
				{
					movementHistoryDo.SaveMovementHistorySql(cmd);
					this.consolidatedDA.ExecuteQuery(security, cmd);
				}
			}
		}

		/// <summary>
		/// This method will update the final record.
		/// </summary>
		/// <param name="security">The security object.</param>
		/// <param name="movementHistoryDo">The movement history record to update.</param>
		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void UpdateNodeDataToFinalRecord(SecurityClass security, MovementHistoryDO movementHistoryDo)
		{
			if (security == null)
			{
				throw new ArgumentNullException("Security");
			}

			if (movementHistoryDo == null)
			{
				throw new ArgumentNullException("MovementHistoryDo");
			}

			if (movementHistoryDo.MovementHistoryGuid == null || movementHistoryDo.MovementHistoryGuid == Guid.Empty)
			{
				throw new ArgumentOutOfRangeException("Movement History Guid is empty.");
			}

			using (SqlCommand cmd = new SqlCommand())
			{
				movementHistoryDo.SiteGuid = security.SiteGuid;

				movementHistoryDo.SaveMovementHistorySql(cmd);
				this.consolidatedDA.ExecuteQuery(security, cmd);
			}
		}

		/// <summary>
		/// This method will delete a movement by Movement Name and Site.
		/// </summary>
		/// <param name="security">The security object.</param>
		/// <param name="movementName">The movement to delete.</param>
		/// <param name="movementHistoryGuid">The movement history to delete.</param>
		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void DeleteByMovementName(SecurityClass security, Guid movementHistoryGuid, string movementName)
		{
			if (security == null)
			{
				throw new ArgumentNullException("Security");
			}

			if (movementHistoryGuid == null || movementHistoryGuid == Guid.Empty)
			{
				throw new ArgumentNullException("MovementHistoryGuid");
			}

			if (string.IsNullOrEmpty(movementName))
			{
				throw new ArgumentNullException("movementName");
			}

			var movementHistoryDo = new MovementHistoryDO();

			using (SqlCommand cmd = new SqlCommand())
			{
				movementHistoryDo.DeleteMovementHistoryByMovementNameSql(cmd, movementHistoryGuid, movementName, security.SiteGuid);
				this.consolidatedDA.ExecuteQuery(security, cmd);
			}
		}

		/// <summary>
		/// This method updates a movement history comment.
		/// </summary>
		/// <param name="security">The security object.</param>
		/// <param name="movementHistoryDo">The movement history record to update.</param>
		/// <param name="comment">The movement history comment to update.</param>
		/// <param name="commentUserId">The movement history comment user to update.</param>
		/// <param name="commentDateTime">The movement history comment date time to update.</param>
		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void UpdateComment(SecurityClass security, Guid movementHistoryGuid, string comment, string commentUserId, DateTime commentDateTime)
		{
			if (security == null)
			{
				throw new ArgumentNullException("Security");
			}

			using (SqlCommand cmd = new SqlCommand())
			{
				var movementHistoryDo = new MovementHistoryDO();
				movementHistoryDo.UpdateMovementHistoryCommentSql(cmd, movementHistoryGuid, comment, commentUserId, commentDateTime);
				this.consolidatedDA.ExecuteQuery(security, cmd);
			}
		}

		/// <summary>
		/// This method will retrieve the movement by the movement Guid and Site.
		/// </summary>
		/// <param name="security">The security object</param>
		/// <param name="movementName">The movement name to retrieve.</param>
		/// <returns>Returns a list of movement history records.</returns>
		public List<MovementHistoryDO> GetMovementByMovementName(SecurityClass security, string movementName)
		{
			if (security == null)
			{
				throw new ArgumentNullException("Security");
			}

			if (string.IsNullOrEmpty(movementName))
			{
				throw new ArgumentNullException("movementName");
			}

			var movementHistoryDoList = new List<MovementHistoryDO>();
			var movementHistoryDo = new MovementHistoryDO();
			DataSet dataSet = null;

			using (SqlCommand cmd = new SqlCommand())
			{
				movementHistoryDo.GetMovementByMovementNameSql(cmd, movementName, security.SiteGuid);
				dataSet = this.consolidatedDA.GetDataSet(cmd, security);
			}

			if(dataSet == null || dataSet.Tables.Count == 0 || dataSet.Tables[0].Rows.Count == 0)
			{
				return movementHistoryDoList;
			}

			foreach(DataRow row in dataSet.Tables[0].Rows)
			{
				movementHistoryDo = new MovementHistoryDO();
				movementHistoryDo.Load(row);
				movementHistoryDoList.Add(movementHistoryDo);
			}

			return movementHistoryDoList;
		}

		/// <summary>
		/// This method will retrieve all the movement by the Site.
		/// </summary>
		/// <param name="security">The security object</param>
		/// <param name="siteGuid">The site Guid to retrieve.</param>
		/// <param name="startTime">The start date/time to retrieve.</param>
		/// <param name="endTime">The end date/time to retrieve.</param>
		/// <param name="orderColumnName">The column to order by.</param>
		/// <param name="orderDirection">The order direction.</param>
		/// <returns>Returns a list of movement history records.</returns>
		public List<MovementHistoryDO> GetAllMovementsBySiteGuid(SecurityClass security
																	, Guid siteGuid
																	, DateTime startTime
																	, DateTime endTime
																	, bool autoGauge
																	, bool handGauge
																	, bool midnightRecord
																	, string orderColumnName
																	, string orderDirection)
		{
			if (security == null)
			{
				throw new ArgumentNullException("Security");
			}

			if (siteGuid == null || siteGuid == Guid.Empty)
			{
				throw new ArgumentNullException("SiteGuid");
			}

			var movementHistoryDoList = new List<MovementHistoryDO>();
			var movementHistoryDo = new MovementHistoryDO();
			DataSet dataSet = null;

			using (SqlCommand cmd = new SqlCommand())
			{
				movementHistoryDo.GetAllMovementsBySiteGuidSql(cmd, security.SiteGuid, startTime, endTime, autoGauge, handGauge, midnightRecord, orderColumnName, orderDirection);
				dataSet = this.consolidatedDA.GetDataSet(cmd, security);
			}

			if (dataSet == null || dataSet.Tables.Count == 0 || dataSet.Tables[0].Rows.Count == 0)
			{
				return movementHistoryDoList;
			}

			foreach (DataRow row in dataSet.Tables[0].Rows)
			{
				movementHistoryDo = new MovementHistoryDO();
				movementHistoryDo.Load(row);
				movementHistoryDoList.Add(movementHistoryDo);
			}

			return movementHistoryDoList;
		}

		/// <summary>
		/// This method will retrieve all the movement by the Site.
		/// </summary>
		/// <param name="security">The security object</param>
		/// <param name="siteGuid">The site Guid to retrieve.</param>
		/// <param name="initialLoadCount">The initial load count to retrieve.</param>
		/// <returns>Returns a list of movement history records.</returns>
		public List<MovementHistoryDO> GetMovementsByInitialLoadRequest(SecurityClass security, Guid siteGuid, int initialLoadCount)
		{
			if (security == null)
			{
				throw new ArgumentNullException("Security");
			}

			if (siteGuid == null || siteGuid == Guid.Empty)
			{
				throw new ArgumentNullException("SiteGuid");
			}

			var movementHistoryDoList = new List<MovementHistoryDO>();
			var movementHistoryDo = new MovementHistoryDO();
			DataSet dataSet = null;

			using (SqlCommand cmd = new SqlCommand())
			{
				movementHistoryDo.GetMovementsByInitialLoadRequestSql(cmd, security.SiteGuid, initialLoadCount);
				dataSet = this.consolidatedDA.GetDataSet(cmd, security);
			}

			if (dataSet == null || dataSet.Tables.Count == 0 || dataSet.Tables[0].Rows.Count == 0)
			{
				return movementHistoryDoList;
			}

			foreach (DataRow row in dataSet.Tables[0].Rows)
			{
				movementHistoryDo = new MovementHistoryDO();
				movementHistoryDo.Load(row);
				movementHistoryDoList.Add(movementHistoryDo);
			}

			return movementHistoryDoList;
		}

		/// <summary>
		/// This method will retrieve the movement record by the movement history Guid and Site.
		/// </summary>
		/// <param name="security">The security object</param>
		/// <param name="movementHistoryGuid">The movement history Guid to retrieve.</param>
		/// <returns>Returns a movement history record.</returns>
		public MovementHistoryDO GetMovementRecordByGuid(SecurityClass security, Guid movementHistoryGuid)
		{
			if (security == null)
			{
				throw new ArgumentNullException("Security");
			}

			if (movementHistoryGuid == null || movementHistoryGuid == Guid.Empty)
			{
				throw new ArgumentNullException("movementHistoryGuid");
			}

			var movementHistoryDo = new MovementHistoryDO();
			DataSet dataSet = null;

			using (SqlCommand cmd = new SqlCommand())
			{
				movementHistoryDo.GetMovementRecordByGuidSql(cmd, movementHistoryGuid, security.SiteGuid);
				dataSet = this.consolidatedDA.GetDataSet(cmd, security);
			}

			if (dataSet == null || dataSet.Tables.Count == 0 || dataSet.Tables[0].Rows.Count == 0)
			{
				return new MovementHistoryDO();
			}

			DataRow row = dataSet.Tables[0].Rows[0];
			movementHistoryDo = new MovementHistoryDO();
			movementHistoryDo.Load(row);

			return movementHistoryDo;
		}

		/// <summary>
		/// This method will print the movement record by the movement history Guid and Site.
		/// </summary>
		/// <param name="security">The security object</param>
		/// <param name="movementHistoryGuid">The movement history Guid to retrieve.</param>
		/// <returns></returns>
		public void PrintMovementTicket(SecurityClass security, Guid movementHistoryGuid, bool automatic)
		{
			if (security == null)
			{
				throw new ArgumentNullException("Security");
			}

			if (movementHistoryGuid == null || movementHistoryGuid == Guid.Empty)
			{
				throw new ArgumentNullException("movementHistoryGuid");
			}

			EventLog eventLog = new EventLog("Application", ".", "FuelsManager"); ;

			var sites = new SitesClass();
			var site = sites.Get(security, security.SiteGuid, false, false, false);

			if (!site.EnableAutomaticMovementTicketPrinting
			&& automatic)
			{
				return;
			}

			string reportName = site.MovementTicketReportName;
			string printerName = site.MovementTicketPrinter;

			if (!string.IsNullOrEmpty(reportName) && !string.IsNullOrEmpty(printerName) && !printerName.Equals("{None}"))
			{
				var systemSettings = new SystemSettingsClass();
				var systemSetting = systemSettings.Get(security);

				ParameterValue[] parameterValues = new ParameterValue[1];

				parameterValues[0] = new ParameterValue
				{
					Name = "movementGuid",
					Value = movementHistoryGuid.ToString()
				};

				string rptDir = sites.GetReportDirectory(security, reportName);
				ReportServicePrintService printService = new ReportServicePrintService(eventLog)
				{
					PrinterName = printerName,
					ReportingServiceUrl = systemSetting.ReportServerUrl,
					ReportName = rptDir + "/" + reportName,
					ParameterValues = parameterValues,
					Security = security,
					EnableBOLPDFArchiving = false,
				};

				printService.PrintReport();
			}
		}

		/// <summary>
		/// This method will archive the movement record by the movement history Guid and Site.
		/// </summary>
		/// <param name="security">The security object</param>
		/// <param name="movementHistoryGuid">The movement history Guid to retrieve.</param>
		/// <returns></returns>
		public void ArchiveMovementTicket(SecurityClass security, Guid movementHistoryGuid, string movementID)
		{
			if (security == null)
			{
				throw new ArgumentNullException("Security");
			}

			if (movementHistoryGuid == null || movementHistoryGuid == Guid.Empty)
			{
				throw new ArgumentNullException("movementHistoryGuid");
			}

			EventLog eventLog = new EventLog("Application", ".", "FuelsManager");

			var sites = new SitesClass();
			var site = sites.Get(security, security.SiteGuid, false, false, false);

			if (!site.EnableMovementTicketPDFArchiving)
			{
				return;
			}

			string reportName = site.MovementTicketReportName;
			string archiveDirectory = site.MovementTicketFileExportDirectory;
			string fileName = site.MovementTicketExportFileName;

			if (!string.IsNullOrEmpty(reportName) && !string.IsNullOrEmpty(archiveDirectory) && !string.IsNullOrEmpty(fileName))
			{
				var systemSettings = new SystemSettingsClass();
				var systemSetting = systemSettings.Get(security);

				ParameterValue[] parameterValues = new ParameterValue[1];

				parameterValues[0] = new ParameterValue
				{
					Name = "movementGuid",
					Value = movementHistoryGuid.ToString()
				};

				if (fileName.Contains("%SiteID%"))
					fileName = fileName.Replace("%SiteID%", site.SiteID);

				if (fileName.Contains("%MovementID%") && !string.IsNullOrEmpty(movementID))
					fileName = fileName.Replace("%MovementID%", movementID);

				var siteTimeConverter = new SiteTimeConverter(site);
				string dateTimePrefix = siteTimeConverter.ConvertToSiteTime(DateTime.UtcNow).ToString("_yyyyMMdd-HHmmss");

				string extension = Path.GetExtension(fileName);
				if (string.IsNullOrWhiteSpace(extension) || (extension.ToUpper() != ".PDF")) extension = ".pdf";

				fileName += dateTimePrefix + extension;

				string rptDir = sites.GetReportDirectory(security, reportName);
				ReportServicePrintService printService = new ReportServicePrintService(eventLog)
				{
					ReportingServiceUrl = systemSetting.ReportServerUrl,
					ReportName = rptDir + "/" + reportName,
					ParameterValues = parameterValues,
					Security = security,
					EnableBOLPDFArchiving = true,
					BOLPDFArchivingPath = archiveDirectory,
					BOLPDFArchivingFileName = fileName,
				};

				printService.ArchiveReport();
			}
			else
			{
				if(string.IsNullOrEmpty(reportName))
				{
					eventLog.WriteEntry("Failed to archive Movement Ticket due to missing report name (Hint: Site configuration -> Reports).", EventLogEntryType.Error);
				}
				else if (string.IsNullOrEmpty(archiveDirectory))
				{
					eventLog.WriteEntry("Failed to archive Movement Ticket due to missing archive directory (Hint: Site configuration -> Reports).", EventLogEntryType.Error);
				}
				else if (string.IsNullOrEmpty(fileName))
				{
					eventLog.WriteEntry("Failed to archive Movement Ticket due to missing archive file name (Hint: Site configuration -> Reports).", EventLogEntryType.Error);
				}
			}
		}

		#endregion

		#region Private methods
		/// <summary>
		/// This method maps the movement data into the movement history object for saving.
		/// </summary>
		/// <param name="movementData">The movement data to record.</param>
		/// <returns>Returns the movement history records to be archived.</returns>
		private List<MovementHistoryDO> MapData(ref MovementData movementData, SecurityClass security)
		{
				var movementHistoryList = new List<MovementHistoryDO>();

				// NOTE: The CreateBy property always contains the movement and nodes.
				if(movementData == null || movementData.CreatedBy == null || movementData.CreatedBy.Count == 0)
				{
					this.eventLog.WriteEntry(MovementHistoryPrefix + "Movement Data is null or doesn't have any items.", FMEventLogEntryType.Warning);
					return movementHistoryList;
				}

				int pointValueCount = movementData.CreatedBy.Count;

				// Gets the movement ID which is in the first element of the point value list (index = 0).
				this.GetMovementId(movementData.CreatedBy, out string movementId);

				if (string.IsNullOrEmpty(movementId))
				{
					this.eventLog.WriteEntry(MovementHistoryPrefix + "Movement ID is null or empty.", FMEventLogEntryType.Error);
					return movementHistoryList;
				}

				// Get the Site ID and Guid from the point value.
				this.GetSiteIdAndGuid(movementData.CreatedBy, out string siteId, out Guid siteGuid);

				if(siteGuid == null || siteGuid == Guid.Empty)
				{
					this.eventLog.WriteEntry(MovementHistoryPrefix + "Site GUID is null or empty.", FMEventLogEntryType.Error);
					return movementHistoryList;
				}

				long initiationCount = 0;
				Guid rootParentGuid = Guid.Empty;
				DateTime timeStampUtc = DateTime.UtcNow;
				bool successful;
				int recordSeq = 0;
				bool hasHandgauge = false;
				bool isMidnightRecord = false;

				// This loop contains one movement with X number of nodes.
				for (int index = 0; index < pointValueCount; index++)
				{
					// The first record contains the movement record.
					if(index == 0)
					{
						// Check for a midnight record.
						isMidnightRecord = this.IsMidnightRecord(movementData.Status);

						MovementHistoryDO movementHistoryDo = this.CreateNewMovementHistoryDo(siteGuid, siteId, movementId, timeStampUtc, security, recordSeq, isMidnightRecord);
						rootParentGuid = movementHistoryDo.MovementHistoryGuid;
						successful = this.MapMovementRecord(ref movementHistoryDo, ref movementData, initiationCount, index);

						if (successful)
						{
								hasHandgauge = this.IsHandgaugeSet(security, movementHistoryDo.PointGuid);
								initiationCount = movementHistoryDo.InitiationCount.Value;
								movementHistoryList.Add(movementHistoryDo);
						}
						else
						{
								return new List<MovementHistoryDO>();
						}
					}

					// The movement info is in the first row of the list. Node data is in the subsequent rows.
					if (index > 0)
					{
						Guid parentGuid;
						recordSeq++;

						// Create the movement node record.
						MovementHistoryDO movementHistoryDo = this.CreateNewMovementHistoryDo(siteGuid, siteId, movementId, timeStampUtc, security, recordSeq, isMidnightRecord);
						this.GetNodeId(movementData.CreatedBy, index, out string nodeId);

						// Must have a node Guid.
						if (string.IsNullOrEmpty(nodeId))
						{
								this.eventLog.WriteEntry(MovementHistoryPrefix + "Node Node ID is null or empty.", FMEventLogEntryType.Error);
								continue;
						}

						movementHistoryDo.Node = nodeId + " - AG";
						movementHistoryDo.RecordType = MovementHistoryDO.MovementRecordTypes.Node;
						movementHistoryDo.RootParentGuid = rootParentGuid;
						movementHistoryDo.ParentGuid = rootParentGuid;

						successful = this.MapMovementRecord(ref movementHistoryDo, ref movementData, initiationCount, index);

						if (successful)
						{
								parentGuid = movementHistoryDo.MovementHistoryGuid;
								movementHistoryList.Add(movementHistoryDo);
						}
						else
						{
								return new List<MovementHistoryDO>();
						}

						// Create the final record which is based on the current node information
						recordSeq++;
						movementHistoryDo = this.CreateNewMovementHistoryDo(siteGuid, siteId, movementId, timeStampUtc, security, recordSeq, isMidnightRecord);
						movementHistoryDo.Node = nodeId;
						movementHistoryDo.RecordType = MovementHistoryDO.MovementRecordTypes.Final;
						movementHistoryDo.RootParentGuid = rootParentGuid;
						movementHistoryDo.ParentGuid = parentGuid;

						successful = this.MapMovementRecord(ref movementHistoryDo, ref movementData, initiationCount, index);

						if (successful)
						{
								movementHistoryList.Add(movementHistoryDo);
						}
						else
						{
								return new List<MovementHistoryDO>();
						}

						// Create the handgauge record which is based on the current node information.
						// Only create this record if the handgauge is set in the configuration.
						if (hasHandgauge)
						{
								recordSeq++;
								movementHistoryDo = this.CreateNewMovementHistoryDo(siteGuid, siteId, movementId, timeStampUtc, security, recordSeq, isMidnightRecord);
								movementHistoryDo.Node = nodeId + " - HG";
								movementHistoryDo.RecordType = MovementHistoryDO.MovementRecordTypes.Handgauge;
								movementHistoryDo.RootParentGuid = rootParentGuid;
								movementHistoryDo.ParentGuid = parentGuid;

								successful = this.MapMovementRecord(ref movementHistoryDo, ref movementData, initiationCount, index);

								if (successful)
								{
									movementHistoryList.Add(movementHistoryDo);
								}
								else
								{
									return new List<MovementHistoryDO>();
								}
						}
					}
				}

				return movementHistoryList;
		}

		/// <summary>
		/// This method will create a movement history data object with its intial information.
		/// </summary>
		/// <param name="siteGuid">The site guid.</param>
		/// <param name="siteId">The site ID.</param>
		/// <param name="movementId">The movement name.</param>
		/// <param name="timeStampUtc">The UTC data time to record the record</param>
		/// <param name="security">The security object.</param>
		/// <param name="recordSeq">The record sequence number for the movement records.</param>
		/// <param name="midnightRecord">Indicate if the record is a midnight record.</param>
		/// <returns>Returns a movement history data object.</returns>
		private MovementHistoryDO CreateNewMovementHistoryDo(Guid siteGuid
																				, string siteId
																				, string movementId
																				, DateTime timeStampUtc
																				, SecurityClass security
																				, int recordSeq
																				, bool midnightRecord)
		{
				MovementHistoryDO movementHistoryDo = new MovementHistoryDO
				{
					MovementHistoryGuid = Guid.NewGuid(),
					SiteGuid				= siteGuid,
					SiteID				= siteId,
					Name					= movementId + (midnightRecord ? " - M" : string.Empty),
					RecordType			= MovementHistoryDO.MovementRecordTypes.Movement,
					TimeStamp			= timeStampUtc,
					ParentGuid			= Guid.Empty,
					RootParentGuid		= Guid.Empty,
					RecordSeq			= recordSeq,
					MidnightRecord		= midnightRecord,
					CreatedBy			= string.IsNullOrEmpty(security.UserID) ? "MovementProcessor" : security.UserID,
					UpdatedBy			= string.IsNullOrEmpty(security.UserID) ? "MovementProcessor" : security.UserID
				};

				return movementHistoryDo;
		}

		/// <summary>
		/// This method will map the movement data to the movement history data object. 
		/// </summary>
		/// <param name="movementHistoryDo">The movement history object being mapped.</param>
		/// <param name="movementData">The movement data to be mapped.</param>
		/// <param name="initiationCount">The initiation count.</param>
		/// <param name="index">The movement data array index.</param>
		/// <returns>Returns true if successful, otherwise it returns false.</returns>
		private bool MapMovementRecord(ref MovementHistoryDO movementHistoryDo
													, ref MovementData movementData
													, long initiationCount
													, int index)
		{
		string processingItem = string.Empty;
		bool successful = true;
		int? unitVolumeIndex = null;
		int? unitLevelProductIndex = null;
		int? unitTemperatureProductIndex = null;
		int? unitDensityProductObservedIndex = null;
		int? unitDensityProductStandardIndex = null;
		int? unitTemperatureDensityIndex = null;
		int? unitTemperatureAmbientIndex = null;
		int? unitMassIndex = null;
		int? decimalPlacesVolume = null;
		int? decimalPlacesLevel = null;
		int? decimalPlacesDensity = null;
		int? decimalPlacesTemperature = null;
		try
		{
			PointValue pointValue;

			// Only the Movement record has the initiation count field which is at index 0.
			if (index == 0)
			{
				pointValue = this.GetNextPointValue(movementData.InitiationCount, index);

				if (pointValue != null && pointValue.Value != null)
				{
						processingItem = "InitiationCount";
						movementHistoryDo.InitiationCount = (short)pointValue.Value;
						movementHistoryDo.PointGuid = pointValue.PointGuid;
				}
			}
			else
			{
				movementHistoryDo.InitiationCount = initiationCount;

				pointValue = this.GetNextPointValue(movementData.PointId, index);
				if (pointValue != null && pointValue.Value != null)
				{
						processingItem = "PointId";
						movementHistoryDo.PointGuid = pointValue.PointGuid;
				}

				pointValue = this.GetNextPointValue(movementData.TransferDirection, index);
				if (pointValue != null && pointValue.Value != null)
				{
						processingItem = "TransferDirection";
						movementHistoryDo.TransferDirection = (TransferDirection)pointValue.Value == TransferDirection.Destination ? "Destination" : "Source";
				}
			}

			///////////////////////////////////////////////////////
			// Transfer mappings
			//////////////////////////////////////////////////////
			pointValue = this.GetNextPointValue(movementData.TransferMode, index);
				if (pointValue != null && pointValue.Value != null)
				{
					processingItem = "TransferMode";
					if (pointValue.Value is TankTransferMode)
					{
						switch ((TankTransferMode)pointValue.Value)
						{
							case TankTransferMode.Inactive:
								movementHistoryDo.TransferMode = (int)TransferModes.Inactive;
								break;
							case TankTransferMode.Batch:
								movementHistoryDo.TransferMode = (int)TransferModes.Batch;
								break;
							case TankTransferMode.Level:
								movementHistoryDo.TransferMode = (int)TransferModes.Level;
								break;
							default:
								break;
						}

					}
					if (pointValue.Value is VolumeTransferMode)
					{
						switch ((VolumeTransferMode)pointValue.Value)
						{
							case VolumeTransferMode.Inactive:
								movementHistoryDo.TransferMode = (int)TransferModes.Inactive;
								break;
							case VolumeTransferMode.Batch:
								movementHistoryDo.TransferMode = (int)TransferModes.Batch;
								break;
							default:
								break;
						}
					}

					if (pointValue.Value is TransferModes)
					{
						movementHistoryDo.TransferMode = (int)(TransferModes)pointValue.Value;
					}
				}

				pointValue = this.GetNextPointValue(movementData.TransferredGOV, index);
				if (pointValue != null && pointValue.Value != null)
				{
					processingItem = "TransferredGOV";
					movementHistoryDo.CloseoutTransferGov = (double)pointValue.Value;
				}

				pointValue = this.GetNextPointValue(movementData.TransferredNSV, index);
				if (pointValue != null && pointValue.Value != null)
				{
					processingItem = "TransferredNSV";
					movementHistoryDo.CloseoutTransferNsv = (double)pointValue.Value;
				}

				pointValue = this.GetNextPointValue(movementData.TransferStartTime, index);
				if (pointValue != null && pointValue.Value != null)
				{
					processingItem = "TransferStartTime";
					movementHistoryDo.StartTime = (DateTimeOffset)pointValue.Value;
				}

				pointValue = this.GetNextPointValue(movementData.TransferStopTime, index);
				if (pointValue != null && pointValue.Value != null)
				{
					processingItem = "TransferStopTime";
					movementHistoryDo.StopTime = (DateTimeOffset)pointValue.Value;
				}

				pointValue = this.GetNextPointValue(movementData.TransferStatus, index);
				if (pointValue != null && pointValue.Value != null)
				{
					processingItem = "TransferStatus";
					movementHistoryDo.TransferStatus = (int)(TransferStatuses)pointValue.Value;
				}

				pointValue = this.GetNextPointValue(movementData.Status, index);
				if (pointValue != null && pointValue.Value != null)
				{
					processingItem = "Status";
					movementHistoryDo.Status = (int)(MovementStatus)pointValue.Value;
				}

				pointValue = this.GetNextPointValue(movementData.VolumeWater, index);
				if (pointValue != null && pointValue.Value != null)
				{
					processingItem = "VolumeWater";
					movementHistoryDo.VolumeWater = (double)pointValue.Value;
				}

				pointValue = this.GetNextPointValue(movementData.TransferredVolumeWater, index);
				if (pointValue != null && pointValue.Value != null)
				{
					processingItem = "TransferredVolumeWater";
					movementHistoryDo.TransferredVolumeWater = (double)pointValue.Value;
				}

				pointValue = this.GetNextPointValue(movementData.TransferredVolume, index);
				if (pointValue != null && pointValue.Value != null)
				{
					processingItem = "TransferredVolume";
					movementHistoryDo.TransferredVolume = (double)pointValue.Value;
				}

				pointValue = this.GetNextPointValue(movementData.LevelProduct, index);
				if (pointValue != null && pointValue.Value != null)
				{
					processingItem = "LevelProduct";
					movementHistoryDo.LevelProduct = (double)pointValue.Value;
					if (unitLevelProductIndex == null) unitLevelProductIndex = (int)pointValue.Units;
				}

				pointValue = this.GetNextPointValue(movementData.TransferStartLevel, index);
				if (pointValue != null && pointValue.Value != null)
				{
					processingItem = "StartLevelProduct";
					movementHistoryDo.StartLevelProduct = (double)pointValue.Value;
				}

				pointValue = this.GetNextPointValue(movementData.TransferStartGOV, index);
				if (pointValue != null && pointValue.Value != null)
				{
					processingItem = "StartVolumeGrossObserved";
					movementHistoryDo.StartVolumeGrossObserved = (double)pointValue.Value;
				}

				pointValue = this.GetNextPointValue(movementData.TransferStartNSV, index);
				if (pointValue != null && pointValue.Value != null)
				{
					processingItem = "StartVolumeNetStandard";
					movementHistoryDo.StartVolumeNetStandard = (double)pointValue.Value;
				}

				pointValue = this.GetNextPointValue(movementData.TransferStartVolume, index);
				if (pointValue != null && pointValue.Value != null)
				{
					processingItem = "StartVolume";
					movementHistoryDo.StartVolume = (double)pointValue.Value;
				}

				pointValue = this.GetNextPointValue(movementData.TransferStartWaterVolume, index);
				if (pointValue != null && pointValue.Value != null)
				{
					processingItem = "StartVolumeWater";
					movementHistoryDo.StartVolumeWater = (double)pointValue.Value;
				}

				pointValue = this.GetNextPointValue(movementData.TransferTarget, index);
				if (pointValue != null && pointValue.Value != null)
				{
					processingItem = "TransferTarget";
					movementHistoryDo.TransferTarget = (double)pointValue.Value;
				}

				pointValue = this.GetNextPointValue(movementData.TransferTarget, index);
				if (pointValue != null && pointValue.Value != null)
				{
					processingItem = "TransferTargetUnitsIndex";
					movementHistoryDo.TransferTargetUnitsIndex = (int) pointValue.Units;
				}

				pointValue = this.GetNextPointValue(movementData.TransferLevelTarget, index);
				if (pointValue != null && pointValue.Value != null)
				{
					processingItem = "TransferLevelTarget";
					movementHistoryDo.TransferLevelTarget = (double)pointValue.Value;
				}

				pointValue = this.GetNextPointValue(movementData.TransferVolumeTarget, index);
				if (pointValue != null && pointValue.Value != null)
				{
					processingItem = "TransferVolumeTarget";
					movementHistoryDo.TransferVolumeTarget = (double)pointValue.Value;
				}

				pointValue = this.GetNextPointValue(movementData.TransferTimeRemaining, index);
				if (pointValue != null && pointValue.Value != null)
				{
					processingItem = "TransferTimeRemaining";
					movementHistoryDo.TransferTimeRemaining = ((TimeSpan)pointValue.Value).Ticks;
				}

				pointValue = this.GetNextPointValue(movementData.Deviation, index);
				if (pointValue != null && pointValue.Value != null)
				{
					processingItem = "Deviation";
					movementHistoryDo.TransferDeviation = (double)pointValue.Value;
				}

				pointValue = this.GetNextPointValue(movementData.PercentDeviation, index);
				if (pointValue != null && pointValue.Value != null)
				{
					processingItem = "PercentDeviation";
					movementHistoryDo.TransferPercentDeviation = (double)pointValue.Value;
					movementHistoryDo.DecimalPlacesPercent = (int)pointValue.DecimalPlaces;
				}

				pointValue = this.GetNextPointValue(movementData.Product, index);
				if (pointValue != null && pointValue.Value != null)
				{
					processingItem = "Product";
					movementHistoryDo.Product = (string)pointValue.Value;
				}


				/////////////////////////////////////////////////////////////////
				// User Data mappings
				////////////////////////////////////////////////////////////////
				pointValue = this.GetNextPointValue(movementData.UserData01, index);
				if (pointValue != null && pointValue.Value != null)
				{
					processingItem = "UserData01";
					movementHistoryDo.UserData01 = (string)pointValue.Value;
				}

				pointValue = this.GetNextPointValue(movementData.UserData01, index);
				if (pointValue != null && pointValue.Value != null)
				{
					processingItem = "UserData02";
					movementHistoryDo.UserData02 = (string)pointValue.Value;
				}

				pointValue = this.GetNextPointValue(movementData.UserData01, index);
				if (pointValue != null && pointValue.Value != null)
				{
					processingItem = "UserData03";
					movementHistoryDo.UserData03 = (string)pointValue.Value;
				}

				pointValue = this.GetNextPointValue(movementData.UserData01, index);
				if (pointValue != null && pointValue.Value != null)
				{
					processingItem = "UserData04";
					movementHistoryDo.UserData04 = (string)pointValue.Value;
				}

				pointValue = this.GetNextPointValue(movementData.UserData01, index);
				if (pointValue != null && pointValue.Value != null)
				{
					processingItem = "UserData05";
					movementHistoryDo.UserData05 = (string)pointValue.Value;
				}

				pointValue = this.GetNextPointValue(movementData.UserData01, index);
				if (pointValue != null && pointValue.Value != null)
				{
					processingItem = "UserData06";
					movementHistoryDo.UserData06 = (string)pointValue.Value;
				}

				pointValue = this.GetNextPointValue(movementData.UserData01, index);
				if (pointValue != null && pointValue.Value != null)
				{
					processingItem = "UserData07";
					movementHistoryDo.UserData07 = (string)pointValue.Value;
				}

				pointValue = this.GetNextPointValue(movementData.UserData01, index);
				if (pointValue != null && pointValue.Value != null)
				{
					processingItem = "UserData08";
					movementHistoryDo.UserData08 = (string)pointValue.Value;
				}

				pointValue = this.GetNextPointValue(movementData.UserData01, index);
				if (pointValue != null && pointValue.Value != null)
				{
					processingItem = "UserData09";
					movementHistoryDo.UserData09 = (string)pointValue.Value;
				}

				pointValue = this.GetNextPointValue(movementData.UserData01, index);
				if (pointValue != null && pointValue.Value != null)
				{
					processingItem = "UserData10";
					movementHistoryDo.UserData10 = (string)pointValue.Value;
				}

				////////////////////////////////////////////////////////////////////////
				// Closeout mappings from Openings
				///////////////////////////////////////////////////////////////////////
				pointValue = this.GetNextPointValue(movementData.OpeningTemperatureAmbient, index);
				if (pointValue != null && pointValue.Value != null)
				{
					processingItem = "OpeningTemperatureAmbient";
					movementHistoryDo.CloseoutTemperatureAmbient = (double)pointValue.Value;
					if (unitTemperatureAmbientIndex == null) unitTemperatureAmbientIndex = (int)pointValue.Units;
					if (decimalPlacesTemperature == null) decimalPlacesTemperature = (int)pointValue.DecimalPlaces;
				}

				pointValue = this.GetNextPointValue(movementData.OpeningDensityProductObserved, index);
				if (pointValue != null && pointValue.Value != null)
				{
					processingItem = "OpeningDensityProductObserved";
					movementHistoryDo.CloseoutDensityProductObserved = (double)pointValue.Value;
				}

				pointValue = this.GetNextPointValue(movementData.OpeningDensityProductinAir, index);
				if (pointValue != null && pointValue.Value != null)
				{
					processingItem = "OpeningDensityProductinAir";
					movementHistoryDo.CloseoutDensityProductInAir = (double)pointValue.Value;
				}

				pointValue = this.GetNextPointValue(movementData.OpeningDensityProductStandard, index);
				if (pointValue != null && pointValue.Value != null)
				{
					processingItem = "OpeningDensityProductStandard";
					movementHistoryDo.CloseoutDensityProductStandard = (double)pointValue.Value;
				}

				pointValue = this.GetNextPointValue(movementData.OpeningDensityProductStandardinAir, index);
				if (pointValue != null && pointValue.Value != null)
				{
					processingItem = "OpeningDensityProductStandardInAir";
					movementHistoryDo.CloseoutDensityProductInAir = (double)pointValue.Value;
				}

				pointValue = this.GetNextPointValue(movementData.OpeningLevelProduct, index);
				if (pointValue != null && pointValue.Value != null)
				{
					processingItem = "OpeningLevelProduct";
					movementHistoryDo.CloseoutLevelProduct = (double)pointValue.Value;
					if (unitLevelProductIndex == null) unitLevelProductIndex = (int)pointValue.Units;
				}

				pointValue = this.GetNextPointValue(movementData.OpeningLevelWater, index);
				if (pointValue != null && pointValue.Value != null)
				{
					processingItem = "OpeningLevelWater";
					movementHistoryDo.CloseoutLevelWater = (double)pointValue.Value;
				}

				pointValue = this.GetNextPointValue(movementData.OpeningMassLiquid, index);
				if (pointValue != null && pointValue.Value != null)
				{
					processingItem = "OpeningMassLiquid";
					movementHistoryDo.CloseoutMassLiquid = (double)pointValue.Value;
					if (unitMassIndex == null) unitMassIndex = (int)pointValue.Units;
				}

                pointValue = this.GetNextPointValue(movementData.OpeningPercentBsw, index);
                if (pointValue != null && pointValue.Value != null)
                {
                    processingItem = "OpeningPercentBsw";
                    movementHistoryDo.CloseoutPercentBsw = (double)pointValue.Value;
                }

                pointValue = this.GetNextPointValue(movementData.OpeningTankShellCorrection, index);
				if (pointValue != null && pointValue.Value != null)
				{
					processingItem = "OpeningTankShellCorrection";
					movementHistoryDo.CloseoutTankShellCorrection = (double)pointValue.Value;
				}

				pointValue = this.GetNextPointValue(movementData.OpeningTemperatureDensity, index);
				if (pointValue != null && pointValue.Value != null)
				{
					processingItem = "OpeningTemperatureDensity";
					movementHistoryDo.CloseoutTemperatureDensity = (double)pointValue.Value;
					if (unitTemperatureDensityIndex == null) unitTemperatureDensityIndex = (int)pointValue.Units;
				}

				pointValue = this.GetNextPointValue(movementData.OpeningTemperatureProduct, index);
				if (pointValue != null && pointValue.Value != null)
				{
					processingItem = "OpeningTemperatureProduct";
					movementHistoryDo.CloseoutTemperatureProduct = (double)pointValue.Value;
					if (unitTemperatureProductIndex == null) unitTemperatureProductIndex = (int)pointValue.Units;
					if (decimalPlacesTemperature == null) decimalPlacesTemperature = (int)pointValue.DecimalPlaces;
				}

                pointValue = this.GetNextPointValue(movementData.OpeningVolumeBsw, index);
                if (pointValue != null && pointValue.Value != null)
                {
                    processingItem = "OpeningVolumeBsw";
                    movementHistoryDo.CloseoutVolumeBsw = (double)pointValue.Value;
                    if (unitVolumeIndex == null) unitVolumeIndex = (int)pointValue.Units;
					if (decimalPlacesVolume == null) decimalPlacesVolume = (int)pointValue.DecimalPlaces;
                }

                pointValue = this.GetNextPointValue(movementData.OpeningVolumeCorrectionFactor, index);
				if (pointValue != null && pointValue.Value != null)
				{
					processingItem = "OpeningVolumeCorrectionFactor";
					movementHistoryDo.CloseoutVolumeCorrectionFactor = (double)pointValue.Value;
				}

				pointValue = this.GetNextPointValue(movementData.OpeningVolumeGrossObserved, index);
				if (pointValue != null && pointValue.Value != null)
				{
					processingItem = "OpeningVolumeGrossObserved";
					movementHistoryDo.CloseoutVolumeGrossObserved = (double)pointValue.Value;
					if (unitVolumeIndex == null) unitVolumeIndex = (int)pointValue.Units;
				}

				pointValue = this.GetNextPointValue(movementData.OpeningVolumeGrossStandard, index);
				if (pointValue != null && pointValue.Value != null)
				{
					processingItem = "OpeningVolumeGrossStandard";
					movementHistoryDo.CloseoutVolumeGrossStandard = (double)pointValue.Value;
					if (unitVolumeIndex == null) unitVolumeIndex = (int)pointValue.Units;
				}

				pointValue = this.GetNextPointValue(movementData.OpeningVolumeNetStandard, index);
				if (pointValue != null && pointValue.Value != null)
				{
					processingItem = "OpeningVolumeNetStandard";
					movementHistoryDo.CloseoutVolumeNetStandard = (double)pointValue.Value;
					if (unitVolumeIndex == null) unitVolumeIndex = (int)pointValue.Units;
				}

				pointValue = this.GetNextPointValue(movementData.OpeningVolumeRoofCorrection, index);
				if (pointValue != null && pointValue.Value != null)
				{
					processingItem = "OpeningVolumeRoofCorrection";
					movementHistoryDo.CloseoutVolumeRoofCorrection = (double)pointValue.Value;
				}

				pointValue = this.GetNextPointValue(movementData.OpeningVolumeTotalObserved, index);
				if (pointValue != null && pointValue.Value != null)
				{
					processingItem = "OpeningVolumeTotalObserved";
					movementHistoryDo.CloseoutVolumeTotalObserved = (double)pointValue.Value;
					if (unitVolumeIndex == null) unitVolumeIndex = (int)pointValue.Units;
				}

				pointValue = this.GetNextPointValue(movementData.OpeningVolumeWater, index);
				if (pointValue != null && pointValue.Value != null)
				{
					processingItem = "OpeningVolumeWater";
					movementHistoryDo.CloseoutVolumeWater = (double)pointValue.Value;
					if (unitVolumeIndex == null) unitVolumeIndex = (int)pointValue.Units;
				}

				///////////////////////////////////////////////////////////////
				// Other mappings
				///////////////////////////////////////////////////////////////
				///
				if (index == 0)
				{
                    pointValue = this.GetNextPointValue(movementData.Type, index);
                    if (pointValue != null && pointValue.Value != null)
                    {
                        processingItem = "Type";
                        movementHistoryDo.Type = ((MovementType)pointValue.Value).ToString();
                    }

                    pointValue = this.GetNextPointValue(movementData.Comment, index);
					if (pointValue != null && pointValue.Value != null)
					{
						processingItem = "Comment";
						movementHistoryDo.Comment = (string)pointValue.Value;
					}

					pointValue = this.GetNextPointValue(movementData.OrderNumber, index);
					if (pointValue != null && pointValue.Value != null)
					{
						processingItem = "OrderNumber";
						movementHistoryDo.OrderNumber = (string)pointValue.Value;
					}

					pointValue = this.GetNextPointValue(movementData.PlannedStartTime, index);
					if (pointValue != null && pointValue.Value != null)
					{
						processingItem = "PlannedStartTime";
						movementHistoryDo.PlannedStartTime = (DateTimeOffset)pointValue.Value;
					}
				}
				/////////////////////////////////////////////////////////////////
				// Start field mappings
				/////////////////////////////////////////////////////////////////
				pointValue = this.GetNextPointValue(movementData.StartTemperatureAmbient, index);
				if (pointValue != null && pointValue.Value != null)
				{
					processingItem = "StartTemperatureAmbient";
					movementHistoryDo.StartTemperatureAmbient = (double)pointValue.Value;
					if (unitTemperatureAmbientIndex == null) unitTemperatureAmbientIndex = (int)pointValue.Units;
					if (decimalPlacesTemperature == null) decimalPlacesTemperature = (int)pointValue.DecimalPlaces;
				}

				pointValue = this.GetNextPointValue(movementData.StartDensityProductObserved, index);
				if (pointValue != null && pointValue.Value != null)
				{
					processingItem = "StartDensityProductObserved";
					movementHistoryDo.StartDensityProductObserved = (double)pointValue.Value;
					if (unitDensityProductObservedIndex == null) unitDensityProductObservedIndex = (int)pointValue.Units;
					if (decimalPlacesDensity == null) decimalPlacesDensity = (int)pointValue.DecimalPlaces;
				}

				pointValue = this.GetNextPointValue(movementData.StartDensityProductinAir, index);
				if (pointValue != null && pointValue.Value != null)
				{
					processingItem = "StartDensityProductinAir";
					movementHistoryDo.StartDensityProductObservedInAir = (double)pointValue.Value;
				}

				pointValue = this.GetNextPointValue(movementData.StartDensityProductStandard, index);
				if (pointValue != null && pointValue.Value != null)
				{
					processingItem = "StartDensityProductStandard";
					movementHistoryDo.StartDensityProductStandard = (double)pointValue.Value;
					if (unitDensityProductStandardIndex == null) unitDensityProductStandardIndex = (int)pointValue.Units;
					if (decimalPlacesDensity == null) decimalPlacesDensity = (int)pointValue.DecimalPlaces;
				}

				pointValue = this.GetNextPointValue(movementData.StartDensityProductStandardinAir, index);
				if (pointValue != null && pointValue.Value != null)
				{
					processingItem = "StartDensityProductStandardinAir";
					movementHistoryDo.StartDensityProductStandardInAir = (double)pointValue.Value;
				}

				pointValue = this.GetNextPointValue(movementData.StartLevelWater, index);
				if (pointValue != null && pointValue.Value != null)
				{
					processingItem = "StartLevelWater";
					movementHistoryDo.StartLevelWater = (double)pointValue.Value;
					if (decimalPlacesLevel == null) decimalPlacesLevel = (int)pointValue.DecimalPlaces;
				}

				pointValue = this.GetNextPointValue(movementData.StartMassLiquid, index);
				if (pointValue != null && pointValue.Value != null)
				{
					processingItem = "StartMassLiquid";
					movementHistoryDo.StartMassLiquid = (double)pointValue.Value;
				}

                pointValue = this.GetNextPointValue(movementData.StartPercentBsw, index);
                if (pointValue != null && pointValue.Value != null)
                {
                    processingItem = "StartPercentBsw";
                    movementHistoryDo.StartPercentBsw = (double)pointValue.Value;
                }

                pointValue = this.GetNextPointValue(movementData.StartTankShellCorrection, index);
				if (pointValue != null && pointValue.Value != null)
				{
					processingItem = "StartTankShellCorrection";
					movementHistoryDo.StartTankShellCorrection = (double)pointValue.Value;
				}

				pointValue = this.GetNextPointValue(movementData.StartTemperatureDensity, index);
				if (pointValue != null && pointValue.Value != null)
				{
					processingItem = "StartTemperatureDensity";
					movementHistoryDo.StartTemperatureDensity = (double)pointValue.Value;
					if (unitTemperatureDensityIndex == null) unitTemperatureDensityIndex = (int)pointValue.Units;
				}

				pointValue = this.GetNextPointValue(movementData.StartTemperatureProduct, index);
				if (pointValue != null && pointValue.Value != null)
				{
					processingItem = "StartTemperatureProduct";
					movementHistoryDo.StartTemperatureProduct = (double)pointValue.Value;
					if (unitTemperatureProductIndex == null) unitTemperatureProductIndex = (int)pointValue.Units;
				}

                pointValue = this.GetNextPointValue(movementData.StartVolumeBsw, index);
                if (pointValue != null && pointValue.Value != null)
                {
                    processingItem = "StartVolumeBsw";
                    movementHistoryDo.StartVolumeBsw = (double)pointValue.Value;
                }

                pointValue = this.GetNextPointValue(movementData.StartVolumeCorrectionFactor, index);
				if (pointValue != null && pointValue.Value != null)
				{
					processingItem = "StartVolumeCorrectionFactor";
					movementHistoryDo.StartVolumeCorrectionFactor = (double)pointValue.Value;
				}

				pointValue = this.GetNextPointValue(movementData.StartVolumeRoofCorrection, index);
				if (pointValue != null && pointValue.Value != null)
				{
					processingItem = "StartVolumeRoofCorrection";
					movementHistoryDo.StartVolumeRoofCorrection = (double)pointValue.Value;
				}

				pointValue = this.GetNextPointValue(movementData.StartVolumeTotalObserved, index);
				if (pointValue != null && pointValue.Value != null)
				{
					processingItem = "StartVolumeTotalObserved";
					movementHistoryDo.StartVolumeTotalObserved = (double)pointValue.Value;
					if (unitVolumeIndex == null) unitVolumeIndex = (int)pointValue.Units;
					if (decimalPlacesVolume == null) decimalPlacesVolume = (int)pointValue.DecimalPlaces;
				}

				pointValue = this.GetNextPointValue(movementData.StartVolumeGrossStandard, index);
				if (pointValue != null && pointValue.Value != null)
				{
					processingItem = "StartVolumeGrossStandard";
					movementHistoryDo.StartVolumeGrossStandard = (double)pointValue.Value;
					if (unitVolumeIndex == null) unitVolumeIndex = (int)pointValue.Units;
					if (decimalPlacesVolume == null) decimalPlacesVolume = (int)pointValue.DecimalPlaces;
				}

				// Set all the units.
				movementHistoryDo.UnitsVolumeIndex						= unitVolumeIndex;
				movementHistoryDo.UnitsLevelProductIndex				= unitLevelProductIndex;
				movementHistoryDo.UnitsTemperatureProductIndex		= unitTemperatureProductIndex;
				movementHistoryDo.UnitsDensityProductObservedIndex  = unitDensityProductObservedIndex;
				movementHistoryDo.UnitsDensityProductStandardIndex  = unitDensityProductStandardIndex;
				movementHistoryDo.UnitsTemperatureDensityIndex		= unitTemperatureDensityIndex;
				movementHistoryDo.UnitsTemperatureAmbientIndex		= unitTemperatureAmbientIndex;
				movementHistoryDo.UnitsMassIndex						= unitMassIndex;

				// set precision for report
				movementHistoryDo.DecimalPlacesVolume					= decimalPlacesVolume;
				movementHistoryDo.DecimalPlacesLevel					= decimalPlacesLevel;
				movementHistoryDo.DecimalPlacesDensity				= decimalPlacesDensity;
				movementHistoryDo.DecimalPlacesTemperature			= decimalPlacesTemperature;
			}
			catch (Exception ex)
			{
				string msg = MovementHistoryPrefix + "Couldn't convert value from object for field: " + processingItem + ". " + ex.Message;
				this.eventLog.WriteEntry(msg, FMEventLogEntryType.Error);
				successful = false;
			}

			return successful;
		}

		/// <summary>
		/// This method will return the Point Value for a selected index.
		/// </summary>
		/// <param name="pointValueList">The point value list containing the data.</param>
		/// <param name="index">The index to retrieve the point value.</param>
		/// <returns>Returns the selected point value or null if not found.</returns>
		private PointValue GetNextPointValue(List<PointValue> pointValueList, int index)
		{
				if(pointValueList == null || pointValueList.Count == 0)
				{
					return null;
				}

				if(index >= pointValueList.Count)
				{
					string msg = MovementHistoryPrefix + "Index out of range for GetNextPointValue(). Index = " + index + ", List Count = " + pointValueList.Count + ".";
					this.eventLog.WriteEntry(msg, FMEventLogEntryType.Warning);
					return null;
				}

				return pointValueList[index];
		}

		/// <summary>
		/// This method will return the Movement ID based on the first element of the 
		/// point value list collection.
		/// </summary>
		/// <param name="pointValueList">The point value list.</param>
		/// <param name="movementId">Movement ID out value.</param>
		private void GetMovementId(List<PointValue> pointValueList, out string movementId)
		{
				if (pointValueList == null || pointValueList.Count == 0)
				{
					movementId = string.Empty;

					string msg = MovementHistoryPrefix + "Point Value List is null or empty. Method > GetMovementId().";
					this.eventLog.WriteEntry(msg, FMEventLogEntryType.Warning);
					return;
				}

				PointValue pointValue = pointValueList[0];
				movementId = pointValue.PointID;
		}

		/// <summary>
		/// This method will return true if the movement record is a midnight 
		/// record.
		/// </summary>
		/// <param name="pointValueList">The point value list.</param>
		/// <returns>Returns true if midnight record. Otherwise, returns false.</returns>
		private bool IsMidnightRecord(List<PointValue> statusPointValueList)
		{
				if(statusPointValueList == null || statusPointValueList.Count == 0)
				{
					return false;
				}

				PointValue pointValue = this.GetNextPointValue(statusPointValueList, 0);

				if (pointValue != null && pointValue.Value != null)
				{
					var movementStatus = (MovementStatus)pointValue.Value;

					// Midnight will have any status other than Inactive and Complete.
					if (movementStatus != MovementStatus.Inactive && movementStatus != MovementStatus.Complete)
					{
						return true;
					}
				}

				return false;
		}

		/// <summary>
		/// This method will return the Node ID based on the index of the element in the 
		/// point value list collection.
		/// </summary>
		/// <param name="pointValueList">The point value list.</param>
		/// <param name="index">Index into the point value list.</param>
		/// <param name="nodeGuid">Node GUID out value.</param>
		private void GetNodeId(List<PointValue> pointValueList, int index, out string nodeId)
		{
				if (pointValueList == null || pointValueList.Count == 0)
				{
					nodeId = string.Empty;

					string msg = MovementHistoryPrefix + "Point Value List is null or empty. Method > GetNodeId().";
					this.eventLog.WriteEntry(msg, FMEventLogEntryType.Warning);
					return;
				}

				if (index >= pointValueList.Count)
				{
					string msg = MovementHistoryPrefix + "Index out of range for GetNodeId(). Index = " + index + ", List Count = " + pointValueList.Count + ".";
					this.eventLog.WriteEntry(msg, FMEventLogEntryType.Warning);
					nodeId = string.Empty;
					return;
				}

				PointValue pointValue = pointValueList[index];
				nodeId = pointValue.PointID;
		}

		/// <summary>
		/// This method will return the Site Guid and Site ID based on the index of the element in the 
		/// point value list collection.
		/// </summary>
		/// <param name="pointValueList">The point value list.</param>
		/// <param name="siteId">Site ID out value.</param>
		/// <param name="siteGuid">Site GUID out value.</param>
		private void GetSiteIdAndGuid(List<PointValue> pointValueList, out string siteId, out Guid siteGuid)
		{
				if (pointValueList == null || pointValueList.Count == 0)
				{
					siteId = string.Empty;
					siteGuid = Guid.Empty;

					string msg = MovementHistoryPrefix + "Point Value List is null or empty. Method > GetSiteIdAndGuid().";
					this.eventLog.WriteEntry(msg, FMEventLogEntryType.Warning);
					return;
				}

				PointValue pointValue = pointValueList[0];
				siteGuid = pointValue.PointValueIdentifier.SiteGuid;
				siteId = pointValue.SiteID;
		}

		/// <summary>
		/// This method will return whether a movement has the handgauge settings set.
		/// </summary>
		/// <param name="security">The security object.</param>
		/// <param name="pointGuid">The movement point Guid.</param>
		/// <returns>Returns true if the handgauge is set, otherwise returns false.</returns>
		private bool IsHandgaugeSet(SecurityClass security, Guid pointGuid)
		{
				const string movementSettingId = "Movement Settings";
				PointProperties pointProperties = new PointProperties();

				Guid pointPropertyGuid = pointProperties.GetPointPropertyGuid(security, pointGuid, movementSettingId);
				PointProperty pointProperty = pointProperties.Get(security, pointPropertyGuid);

				MovementModuleSettings movementModuleSettings = pointProperty.Value as MovementModuleSettings;

				if (movementModuleSettings != null)
				{
					return movementModuleSettings.HandGaugeData;
				}

				return false;
		}

		/// <summary>
		/// This method initiates the object to its initial state.
		/// </summary>
		private void Init()
		{
				this.eventLog = new FMEventLog();
		}
		#endregion
	}
}