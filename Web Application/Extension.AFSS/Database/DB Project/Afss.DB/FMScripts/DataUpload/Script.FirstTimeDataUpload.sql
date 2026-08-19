/*************************************************'
* Script.FirstTimeDataUpload.sql file
* Use this file for add scripts which insert data into an empty table or brand new table (e.g. new lookup table). This file tests and whether the table is empty in order to insert the data
* For incremental insertions, e.g. adding new records to a lookup table, use the Script.IncrementalDataMaintenance.sql file instead.
**************************************************/

/****** EXAMPLE TEMPLATE ******
IF (SELECT COUNT(*) FROM <TABLE_NAME_HERE>)=0
BEGIN
	<ADD INSERT SCRIPT HERE>
END
*/

-- Modifications on enum RIGHT may require new INSERT/UPDATE on table lookup.tblRights
-- For example, if you add a new right it must be added to lookup.tblRights
IF (NOT EXISTS (SELECT 1 FROM lookup.tblRight WHERE (RightCode = N'VIEW_EXTERNAL_STATION' OR RightCode = N'VIEW_AUTOMATED_FUEL_SERVICE_STATION') AND RightIndex = 181))
BEGIN
	PRINT 'firsttimedataupload: inserting right VIEW_AUTOMATED_FUEL_SERVICE_STATION - 181'
	INSERT INTO [lookup].[tblRight] (RightIndex,RightCode,RightName,RightGuid,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy) VALUES (181,'VIEW_AUTOMATED_FUEL_SERVICE_STATION','VIEW_AUTOMATED_FUEL_SERVICE_STATION','93E2CBFD-320B-466D-AD76-FBEB6B73FBDC',N'10/27/2015 1:49:09 PM -04:00','Administrator',N'10/27/2015 1:49:09 PM -04:00','Administrator')
END

IF (NOT EXISTS (SELECT 1 FROM lookup.tblRight WHERE (RightCode = N'MODIFY_EXTERNAL_STATION' OR RightCode = N'MODIFY_AUTOMATED_FUEL_SERVICE_STATION') AND RightIndex = 182))
BEGIN
	PRINT 'firsttimedataupload: inserting right MODIFY_AUTOMATED_FUEL_SERVICE_STATION - 182'
	INSERT INTO [lookup].[tblRight] (RightIndex,RightCode,RightName,RightGuid,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy) VALUES (182,'MODIFY_AUTOMATED_FUEL_SERVICE_STATION','MODIFY_AUTOMATED_FUEL_SERVICE_STATION','BA9D30CA-9642-4407-BCB6-0B65F4C31752',N'10/27/2015 1:49:09 PM -04:00','Administrator',N'10/27/2015 1:49:09 PM -04:00','Administrator')
END

IF (SELECT COUNT(*) FROM [lookup].[tblExternalStationType]) = 0
BEGIN
	INSERT INTO lookup.tblExternalStationType (ExternalStationTypeIndex, ExternalStationTypeCode, ExternalStationTypeName, ExternalStationTypeGuid, CreatedBy, CreatedDate, UpdatedBy, UpdatedDate) 
	VALUES (0, 'Gasboy', 'Gilbarco - Gasboy', N'835CEB34-DC3A-4ACD-B75F-D7C43D520D8D', 'Administrator', '2014-11-24 00:00:00.0000000 -04:00', 'Administrator', '2014-11-24 00:00:00.0000000 -04:00')
END

IF (SELECT COUNT(*) FROM [lookup].[tblExternalStationStatus]) = 0
BEGIN
	INSERT INTO lookup.tblExternalStationStatus (ExternalStationStatusIndex, ExternalStationStatusCode, ExternalStationStatusName, ExternalStationStatusGuid, CreatedBy, CreatedDate, UpdatedBy, UpdatedDate) 
	VALUES (0, 'Inactive', 'Inactive', N'B94B828A-86DE-4522-98E0-5AB74E0CD5BC', 'Administrator', '2014-11-24 00:00:00.0000000 -04:00', 'Administrator', '2014-11-24 00:00:00.0000000 -04:00')

	INSERT INTO lookup.tblExternalStationStatus (ExternalStationStatusIndex, ExternalStationStatusCode, ExternalStationStatusName, ExternalStationStatusGuid, CreatedBy, CreatedDate, UpdatedBy, UpdatedDate) 
	VALUES (1, 'Good', 'Good', N'AD757729-2640-486E-92EE-788B853FA344', 'Administrator', '2014-11-24 00:00:00.0000000 -04:00', 'Administrator', '2014-11-24 00:00:00.0000000 -04:00')

	INSERT INTO lookup.tblExternalStationStatus (ExternalStationStatusIndex, ExternalStationStatusCode, ExternalStationStatusName, ExternalStationStatusGuid, CreatedBy, CreatedDate, UpdatedBy, UpdatedDate) 
	VALUES (2, 'Bad', 'Bad', N'AE2245D3-9917-48C1-963E-A59845D2FA43', 'Administrator', '2014-11-24 00:00:00.0000000 -04:00', 'Administrator', '2014-11-24 00:00:00.0000000 -04:00')

	INSERT INTO lookup.tblExternalStationStatus (ExternalStationStatusIndex, ExternalStationStatusCode, ExternalStationStatusName, ExternalStationStatusGuid, CreatedBy, CreatedDate, UpdatedBy, UpdatedDate) 
	VALUES (3, 'NoCommunication', 'No Communication', N'8F4870BE-4370-443D-887F-A08687585E33', 'Administrator', '2014-11-24 00:00:00.0000000 -04:00', 'Administrator', '2014-11-24 00:00:00.0000000 -04:00')

END

IF (SELECT COUNT(*) FROM [lookup].[tblExternalStationLogType]) = 0
BEGIN
	INSERT INTO lookup.tblExternalStationLogType (ExternalStationLogTypeIndex, ExternalStationLogTypeCode, ExternalStationLogTypeName, ExternalStationLogTypeGuid, CreatedBy, CreatedDate, UpdatedBy, UpdatedDate) 
	VALUES (0, 'ConnectionFailure', 'Connection Failure', N'835CEB34-DC3A-4ACD-B75F-D7C43D520D8D', 'Administrator', '2014-11-24 00:00:00.0000000 -04:00', 'Administrator', '2014-11-24 00:00:00.0000000 -04:00')

	INSERT INTO lookup.tblExternalStationLogType (ExternalStationLogTypeIndex, ExternalStationLogTypeCode, ExternalStationLogTypeName, ExternalStationLogTypeGuid, CreatedBy, CreatedDate, UpdatedBy, UpdatedDate) 
	VALUES (1, 'ValidationFailure', 'Validation Failure', N'E9FCB81D-0DD3-4A85-A7FE-9B642DDB15B6', 'Administrator', '2014-11-24 00:00:00.0000000 -04:00', 'Administrator', '2014-11-24 00:00:00.0000000 -04:00')
END

IF (SELECT COUNT(*) FROM [lookup].[tblExternalStationSessionType])=0
BEGIN
	INSERT INTO [lookup].[tblExternalStationSessionType] ([ExternalStationSessionTypeIndex], [ExternalStationSessionTypeCode], [ExternalStationSessionTypeName], [ExternalStationSessionTypeGuid], [LongDescription], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (0, N'Manual', N'Manual Request', N'124B0CC0-92AD-4FEF-BAF9-231CC3D3C28F', N'User initiated manual communications request.', N'2015-01-01 00:00:00.0000000 -04:00', N'Administrator', N'2015-01-01 00:00:00.0000000 -04:00', N'Administrator')
	INSERT INTO [lookup].[tblExternalStationSessionType] ([ExternalStationSessionTypeIndex], [ExternalStationSessionTypeCode], [ExternalStationSessionTypeName], [ExternalStationSessionTypeGuid], [LongDescription], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (1, N'Periodic', N'Periodic Request', N'78F60D61-9539-4316-BE2D-7776CFBB7B3C', N'Automated periodic communications request.', N'2015-01-01 00:00:00.0000000 -04:00', N'Administrator', N'2015-01-01 00:00:00.0000000 -04:00', N'Administrator')
	INSERT INTO [lookup].[tblExternalStationSessionType] ([ExternalStationSessionTypeIndex], [ExternalStationSessionTypeCode], [ExternalStationSessionTypeName], [ExternalStationSessionTypeGuid], [LongDescription], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (2, N'Scheduled', N'Scheduled Request', N'4535D44F-4B8B-4BBC-8747-AFD5A572167E', N'Automated schedule-based communications request.', N'2015-01-01 00:00:00.0000000 -04:00', N'Administrator', N'2015-01-01 00:00:00.0000000 -04:00', N'Administrator')
	INSERT INTO [lookup].[tblExternalStationSessionType] ([ExternalStationSessionTypeIndex], [ExternalStationSessionTypeCode], [ExternalStationSessionTypeName], [ExternalStationSessionTypeGuid], [LongDescription], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (3, N'Setup', N'Setup Request', N'63DA50A3-486B-4C52-B2D8-B4C1137F5142', N'Initial setup of Fuel Service Station configuration data.', N'2015-01-01 00:00:00.0000000 -04:00', N'Administrator', N'2015-01-01 00:00:00.0000000 -04:00', N'Administrator')
END

IF (SELECT COUNT(*) FROM [lookup].[tblExternalStationSessionState])=0
BEGIN
	INSERT INTO [lookup].[tblExternalStationSessionState] ([ExternalStationSessionStateIndex], [ExternalStationSessionStateCode], [ExternalStationSessionStateName], [ExternalStationSessionStateGuid], [LongDescription], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (0, N'Init', N'Initializing', N'E6AA79AF-8C47-427F-B7EB-DB611CD7904D', N'Currently initializing new communications session.', N'2015-01-01 00:00:00.0000000 -04:00', N'Administrator', N'2015-01-01 00:00:00.0000000 -04:00', N'Administrator')
	INSERT INTO [lookup].[tblExternalStationSessionState] ([ExternalStationSessionStateIndex], [ExternalStationSessionStateCode], [ExternalStationSessionStateName], [ExternalStationSessionStateGuid], [LongDescription], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (1, N'Conn', N'Connecting', N'B4DE22B0-A450-45B5-BE37-61A4DB85AAEF', N'Connecting to Automated Fuel Service Station.', N'4/3/2013 3:26:11 PM -04:00', N'Administrator', N'4/3/2013 3:26:11 PM -04:00', N'Administrator')
	INSERT INTO [lookup].[tblExternalStationSessionState] ([ExternalStationSessionStateIndex], [ExternalStationSessionStateCode], [ExternalStationSessionStateName], [ExternalStationSessionStateGuid], [LongDescription], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (2, N'UploadingConfig', N'Upload Configuration', N'33B2E74B-0921-4CC8-9FCB-0EB50D35B6C8', N'Currently downloading configuration information to Automated Fuel Service Station.', N'2015-01-01 00:00:00.0000000 -04:00', N'Administrator', N'2015-01-01 00:00:00.0000000 -04:00', N'Administrator')
	INSERT INTO [lookup].[tblExternalStationSessionState] ([ExternalStationSessionStateIndex], [ExternalStationSessionStateCode], [ExternalStationSessionStateName], [ExternalStationSessionStateGuid], [LongDescription], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (3, N'DownloadTrans', N'Download Transactions', N'89972314-C3F0-4A93-A929-CF1CAA34AB8B', N'Currently downloading transactions from Automated Fuel Service Station.', N'2015-01-01 00:00:00.0000000 -04:00', N'Administrator', N'2015-01-01 00:00:00.0000000 -04:00', N'Administrator')
	INSERT INTO [lookup].[tblExternalStationSessionState] ([ExternalStationSessionStateIndex], [ExternalStationSessionStateCode], [ExternalStationSessionStateName], [ExternalStationSessionStateGuid], [LongDescription], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (4, N'ImportTrans', N'Importing Transactions', N'9D4E607D-E250-4545-A4AF-7BD5A9174ED1', N'Currently processing transactions import file generated by an offline Automated Fuel Service Station.', N'2015-01-01 00:00:00.0000000 -04:00', N'Administrator', N'2015-01-01 00:00:00.0000000 -04:00', N'Administrator')
	INSERT INTO [lookup].[tblExternalStationSessionState] ([ExternalStationSessionStateIndex], [ExternalStationSessionStateCode], [ExternalStationSessionStateName], [ExternalStationSessionStateGuid], [LongDescription], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (5, N'Close', N'Closing Session', N'22FE7BD8-60C3-4DEA-9CD7-67AFEAAA0A1A', N'Closing communications session with Automated Fuel Service Station.', N'2015-01-01 00:00:00.0000000 -04:00', N'Administrator', N'2015-01-01 00:00:00.0000000 -04:00', N'Administrator')
	INSERT INTO [lookup].[tblExternalStationSessionState] ([ExternalStationSessionStateIndex], [ExternalStationSessionStateCode], [ExternalStationSessionStateName], [ExternalStationSessionStateGuid], [LongDescription], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (6, N'End', N'Session Ended', N'213ACAF2-8823-4648-978B-5CC3DC3F1521', N'Automated Fuel Service Station communications session is no longer active.', N'2015-01-01 00:00:00.0000000 -04:00', N'Administrator', N'2015-01-01 00:00:00.0000000 -04:00', N'Administrator')
END

IF (SELECT COUNT(*) FROM [lookup].[tblExternalStationSessionStatus])=0
BEGIN
	INSERT INTO [lookup].[tblExternalStationSessionStatus] ([ExternalStationSessionStatusIndex], [ExternalStationSessionStatusCode], [ExternalStationSessionStatusName], [ExternalStationSessionStatusGuid], [LongDescription], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (0, N'New', N'New', N'893007D4-DDD7-49F8-AF0E-5F4960979339', N'Newly created synchronization session.', N'2015-01-01 00:00:00.0000000 -04:00', N'Administrator', N'2015-01-01 00:00:00.0000000 -04:00', N'Administrator')
	INSERT INTO [lookup].[tblExternalStationSessionStatus] ([ExternalStationSessionStatusIndex], [ExternalStationSessionStatusCode], [ExternalStationSessionStatusName], [ExternalStationSessionStatusGuid], [LongDescription], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (1, N'Started', N'Started', N'479FE67B-E6AD-4F9F-A5DE-D34DC60F5DD1', N'Automated Fuel Service Station communications session has started.', N'2015-01-01 00:00:00.0000000 -04:00', N'Administrator', N'2015-01-01 00:00:00.0000000 -04:00', N'Administrator')
	INSERT INTO [lookup].[tblExternalStationSessionStatus] ([ExternalStationSessionStatusIndex], [ExternalStationSessionStatusCode], [ExternalStationSessionStatusName], [ExternalStationSessionStatusGuid], [LongDescription], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (2, N'CompOK', N'Completed', N'EC51A891-5F6D-484A-96C5-EA6F01C52004', N'Session completed successfully.', N'2015-01-01 00:00:00.0000000 -04:00', N'Administrator', N'2015-01-01 00:00:00.0000000 -04:00', N'Administrator')
	INSERT INTO [lookup].[tblExternalStationSessionStatus] ([ExternalStationSessionStatusIndex], [ExternalStationSessionStatusCode], [ExternalStationSessionStatusName], [ExternalStationSessionStatusGuid], [LongDescription], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (3, N'CompErr', N'Completed w/ Errors', N'B84C224D-1A03-4923-87E1-52D78AC64B4C', N'Session completed succesfully but errors were encountered.', N'2015-01-01 00:00:00.0000000 -04:00', N'Administrator', N'2015-01-01 00:00:00.0000000 -04:00', N'Administrator')
	INSERT INTO [lookup].[tblExternalStationSessionStatus] ([ExternalStationSessionStatusIndex], [ExternalStationSessionStatusCode], [ExternalStationSessionStatusName], [ExternalStationSessionStatusGuid], [LongDescription], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (4, N'Failed', N'Failed', N'A5E1512D-EB71-400A-A2E9-2236C7BA6149', N'One or more errors prevented communications with the Automated Fuel Service Station from successfully completing.', N'2015-01-01 00:00:00.0000000 -04:00', N'Administrator', N'2015-01-01 00:00:00.0000000 -04:00', N'Administrator')
	INSERT INTO [lookup].[tblExternalStationSessionStatus] ([ExternalStationSessionStatusIndex], [ExternalStationSessionStatusCode], [ExternalStationSessionStatusName], [ExternalStationSessionStatusGuid], [LongDescription], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (5, N'UserStop', N'Stopped (User)', N'493F3E21-F0C2-4826-ABC0-BF8332B1AD48', N'Automated Fuel Service Station communications session was stopped due to a user stop request.', N'2015-01-01 00:00:00.0000000 -04:00', N'Administrator', N'2015-01-01 00:00:00.0000000 -04:00', N'Administrator')
	INSERT INTO [lookup].[tblExternalStationSessionStatus] ([ExternalStationSessionStatusIndex], [ExternalStationSessionStatusCode], [ExternalStationSessionStatusName], [ExternalStationSessionStatusGuid], [LongDescription], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (6, N'SysStop', N'Stopped (System)', N'8F1DF6B3-14BA-42ED-B609-6991FC521562', N'Automated Fuel Service Station session was stopped due to a system request.  (System Shutdown, Service Stopped, etc)', N'2015-01-01 00:00:00.0000000 -04:00', N'Administrator', N'2015-01-01 00:00:00.0000000 -04:00', N'Administrator')
END

IF (SELECT COUNT(*) FROM [lookup].[tblExternalStationTransactionStatus])=0
BEGIN
	INSERT INTO [lookup].[tblExternalStationTransactionStatus] ([ExternalStationTransactionStatusIndex], [ExternalStationTransactionStatusGuid], [ExternalStationTransactionStatusCode], [ExternalStationTransactionStatusName], [LongDescription], [DisplayOrder], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (-1, N'5C78AE99-7DDE-42E7-9894-A2CBBB6FAFB6', N'None', N'None', N'No status available.', 1, N'2015-01-01 00:00:00.0000000 -04:00', N'Administrator', N'2015-01-01 00:00:00.0000000 -04:00', N'Administrator')
	INSERT INTO [lookup].[tblExternalStationTransactionStatus] ([ExternalStationTransactionStatusIndex], [ExternalStationTransactionStatusGuid], [ExternalStationTransactionStatusCode], [ExternalStationTransactionStatusName], [LongDescription], [DisplayOrder], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (0, N'7DEDEA3A-06DB-4E0A-8151-A2BB720431E2', N'Completed', N'Completed', N'Automated Fuel Service Station transaction was successfully imported into FuelsManager.', 1, N'2015-01-01 00:00:00.0000000 -04:00', N'Administrator', N'2015-01-01 00:00:00.0000000 -04:00', N'Administrator')
	INSERT INTO [lookup].[tblExternalStationTransactionStatus] ([ExternalStationTransactionStatusIndex], [ExternalStationTransactionStatusGuid], [ExternalStationTransactionStatusCode], [ExternalStationTransactionStatusName], [LongDescription], [DisplayOrder], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (1, N'B20D5571-7EF1-4307-82A0-EA207A184D64', N'Failed', N'Failed', N'Automated Fuel Service Station transaction failed to import into FuelsManager because of one or more validation errors.', 2, N'2015-01-01 00:00:00.0000000 -04:00', N'Administrator', N'2015-01-01 00:00:00.0000000 -04:00', N'Administrator')
END

IF (SELECT COUNT(*) FROM [lookup].[tblExternalStationTransactionFailedStatus])=0
BEGIN
	INSERT INTO [lookup].[tblExternalStationTransactionFailedStatus] ([ExternalStationTransactionFailedStatusIndex], [ExternalStationTransactionFailedStatusGuid], [ExternalStationTransactionFailedStatusCode], [ExternalStationTransactionFailedStatusName], [LongDescription], [DisplayOrder], [FinalState], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (-1, N'E0AA9F05-3B64-43DA-9BD1-848EA5408843', N'None', N'None', N'No Status Available.', 1, 0, N'2015-01-01 00:00:00.0000000 -04:00', N'Administrator', N'2015-01-01 00:00:00.0000000 -04:00', N'Administrator')
	INSERT INTO [lookup].[tblExternalStationTransactionFailedStatus] ([ExternalStationTransactionFailedStatusIndex], [ExternalStationTransactionFailedStatusGuid], [ExternalStationTransactionFailedStatusCode], [ExternalStationTransactionFailedStatusName], [LongDescription], [DisplayOrder], [FinalState], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (0, N'FAA2A156-FC74-4AEE-9198-6F32513CAD62', N'Pending', N'Pending', N'Record failed to import and requires corrective action in order to process.', 1, 0, N'2015-01-01 00:00:00.0000000 -04:00', N'Administrator', N'2015-01-01 00:00:00.0000000 -04:00', N'Administrator')
	INSERT INTO [lookup].[tblExternalStationTransactionFailedStatus] ([ExternalStationTransactionFailedStatusIndex], [ExternalStationTransactionFailedStatusGuid], [ExternalStationTransactionFailedStatusCode], [ExternalStationTransactionFailedStatusName], [LongDescription], [DisplayOrder], [FinalState], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (1, N'569B342E-0779-4939-B324-5C860F851EEC', N'Reprocess', N'Reprocess', N'Corrective actions have been made and the record is ready to be reprocessed.', 2, 0, N'2015-01-01 00:00:00.0000000 -04:00', N'Administrator', N'2015-01-01 00:00:00.0000000 -04:00', N'Administrator')
	INSERT INTO [lookup].[tblExternalStationTransactionFailedStatus] ([ExternalStationTransactionFailedStatusIndex], [ExternalStationTransactionFailedStatusGuid], [ExternalStationTransactionFailedStatusCode], [ExternalStationTransactionFailedStatusName], [LongDescription], [DisplayOrder], [FinalState], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (2, N'755E5B27-35C9-4174-95DD-69C1CE27B7CB', N'AutoRetry', N'Automatic Retry', N'Application will attempt to automatically reprocess this record.', 3, 0, N'2015-01-01 00:00:00.0000000 -04:00', N'Administrator', N'2015-01-01 00:00:00.0000000 -04:00', N'Administrator')
	INSERT INTO [lookup].[tblExternalStationTransactionFailedStatus] ([ExternalStationTransactionFailedStatusIndex], [ExternalStationTransactionFailedStatusGuid], [ExternalStationTransactionFailedStatusCode], [ExternalStationTransactionFailedStatusName], [LongDescription], [DisplayOrder], [FinalState], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (3, N'5D5182E9-2ACB-4373-A60B-B02C7E95172B', N'Suppressed', N'Suppressed', N'Issue has been suppressed from further processing and retry attempts.', 4, 1, N'2015-01-01 00:00:00.0000000 -04:00', N'Administrator', N'2015-01-01 00:00:00.0000000 -04:00', N'Administrator')
	INSERT INTO [lookup].[tblExternalStationTransactionFailedStatus] ([ExternalStationTransactionFailedStatusIndex], [ExternalStationTransactionFailedStatusGuid], [ExternalStationTransactionFailedStatusCode], [ExternalStationTransactionFailedStatusName], [LongDescription], [DisplayOrder], [FinalState], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (4, N'DAD11AF3-C61A-4695-A431-181FB2222882', N'Processed', N'Processed', N'Previous transaction issues have been resolved and the record was successfully imported into FuelsManager.', 5, 1, N'2015-01-01 00:00:00.0000000 -04:00', N'Administrator', N'2015-01-01 00:00:00.0000000 -04:00', N'Administrator')
END

IF (SELECT COUNT(*) FROM [lookup].[tblGasboyEventErrorClassCode]) = 0
BEGIN
	INSERT INTO lookup.tblGasboyEventErrorClassCode (GasboyEventErrorClassCodeIndex, GasboyEventErrorClassCode, GasboyEventErrorClassCodeName, GasboyEventErrorClassCodeGuid, CreatedBy, CreatedDate, UpdatedBy, UpdatedDate) 
	VALUES (0, 'All', 'All', N'7F2A3AB8-5340-46BC-8B91-54A6BE8F9576', 'Administrator', '2015-01-26 00:00:00.0000000 -04:00', 'Administrator', '2015-01-26 00:00:00.0000000 -04:00')

	INSERT INTO lookup.tblGasboyEventErrorClassCode (GasboyEventErrorClassCodeIndex, GasboyEventErrorClassCode, GasboyEventErrorClassCodeName, GasboyEventErrorClassCodeGuid, CreatedBy, CreatedDate, UpdatedBy, UpdatedDate) 
	VALUES (1, 'Authorization', 'Authorization', N'E811D458-73A1-4787-B82B-1BFF68A69C4E', 'Administrator', '2015-01-26 00:00:00.0000000 -04:00', 'Administrator', '2015-01-26 00:00:00.0000000 -04:00')

	INSERT INTO lookup.tblGasboyEventErrorClassCode (GasboyEventErrorClassCodeIndex, GasboyEventErrorClassCode, GasboyEventErrorClassCodeName, GasboyEventErrorClassCodeGuid, CreatedBy, CreatedDate, UpdatedBy, UpdatedDate) 
	VALUES (2, 'CMSPull', 'CMS Pull', N'5478D7B8-A24C-476B-BE49-1863BF3B0CE8', 'Administrator', '2015-01-26 00:00:00.0000000 -04:00', 'Administrator', '2015-01-26 00:00:00.0000000 -04:00')

	INSERT INTO lookup.tblGasboyEventErrorClassCode (GasboyEventErrorClassCodeIndex, GasboyEventErrorClassCode, GasboyEventErrorClassCodeName, GasboyEventErrorClassCodeGuid, CreatedBy, CreatedDate, UpdatedBy, UpdatedDate) 
	VALUES (3, 'Communication', 'Communication', N'02626098-D726-400F-ACF4-1F8837722D40', 'Administrator', '2015-01-26 00:00:00.0000000 -04:00', 'Administrator', '2015-01-26 00:00:00.0000000 -04:00')

	INSERT INTO lookup.tblGasboyEventErrorClassCode (GasboyEventErrorClassCodeIndex, GasboyEventErrorClassCode, GasboyEventErrorClassCodeName, GasboyEventErrorClassCodeGuid, CreatedBy, CreatedDate, UpdatedBy, UpdatedDate) 
	VALUES (4, 'HeadOffice', 'Head Office', N'C7CB62E5-442F-43E0-B12C-FC941C7266AD', 'Administrator', '2015-01-26 00:00:00.0000000 -04:00', 'Administrator', '2015-01-26 00:00:00.0000000 -04:00')

	INSERT INTO lookup.tblGasboyEventErrorClassCode (GasboyEventErrorClassCodeIndex, GasboyEventErrorClassCode, GasboyEventErrorClassCodeName, GasboyEventErrorClassCodeGuid, CreatedBy, CreatedDate, UpdatedBy, UpdatedDate) 
	VALUES (5, 'Screens', 'Screens', N'561B82C0-84B0-4306-990E-C062F90BD19B', 'Administrator', '2015-01-26 00:00:00.0000000 -04:00', 'Administrator', '2015-01-26 00:00:00.0000000 -04:00')

	INSERT INTO lookup.tblGasboyEventErrorClassCode (GasboyEventErrorClassCodeIndex, GasboyEventErrorClassCode, GasboyEventErrorClassCodeName, GasboyEventErrorClassCodeGuid, CreatedBy, CreatedDate, UpdatedBy, UpdatedDate) 
	VALUES (6, 'Operation', 'Operation', N'2C655B50-0801-45F0-8BFD-5AA00F09081B', 'Administrator', '2015-01-26 00:00:00.0000000 -04:00', 'Administrator', '2015-01-26 00:00:00.0000000 -04:00')

	INSERT INTO lookup.tblGasboyEventErrorClassCode (GasboyEventErrorClassCodeIndex, GasboyEventErrorClassCode, GasboyEventErrorClassCodeName, GasboyEventErrorClassCodeGuid, CreatedBy, CreatedDate, UpdatedBy, UpdatedDate) 
	VALUES (7, 'System', 'System', N'AE375004-4422-471F-8A9B-4F66634D6561', 'Administrator', '2015-01-26 00:00:00.0000000 -04:00', 'Administrator', '2015-01-26 00:00:00.0000000 -04:00')

END

IF (SELECT COUNT(*) FROM [lookup].[tblGasboyEventObjectType]) = 0
BEGIN
	INSERT INTO lookup.tblGasboyEventObjectType (GasboyEventObjectTypeIndex, GasboyEventObjectTypeCode, GasboyEventObjectTypeName, GasboyEventObjectTypeGuid, CreatedBy, CreatedDate, UpdatedBy, UpdatedDate) 
	VALUES (0, 'Generic', 'Generic', N'134206D8-1A6A-45AA-929C-1FAAB097C11E', 'Administrator', '2015-01-26 00:00:00.0000000 -04:00', 'Administrator', '2015-01-26 00:00:00.0000000 -04:00')

	INSERT INTO lookup.tblGasboyEventObjectType (GasboyEventObjectTypeIndex, GasboyEventObjectTypeCode, GasboyEventObjectTypeName, GasboyEventObjectTypeGuid, CreatedBy, CreatedDate, UpdatedBy, UpdatedDate) 
	VALUES (1, 'Tank', 'Tank', N'A91B2CBD-DE41-4DA8-BD38-7E4366FDDCAD', 'Administrator', '2015-01-26 00:00:00.0000000 -04:00', 'Administrator', '2015-01-26 00:00:00.0000000 -04:00')

	INSERT INTO lookup.tblGasboyEventObjectType (GasboyEventObjectTypeIndex, GasboyEventObjectTypeCode, GasboyEventObjectTypeName, GasboyEventObjectTypeGuid, CreatedBy, CreatedDate, UpdatedBy, UpdatedDate) 
	VALUES (2, 'Probe', 'Probe', N'D4B380D5-3F2C-4CC8-95CF-8D582DE88164', 'Administrator', '2015-01-26 00:00:00.0000000 -04:00', 'Administrator', '2015-01-26 00:00:00.0000000 -04:00')

	INSERT INTO lookup.tblGasboyEventObjectType (GasboyEventObjectTypeIndex, GasboyEventObjectTypeCode, GasboyEventObjectTypeName, GasboyEventObjectTypeGuid, CreatedBy, CreatedDate, UpdatedBy, UpdatedDate) 
	VALUES (3, 'Product', 'Product', N'3AA93156-042D-4AD3-AB63-0A4E56DC03F1', 'Administrator', '2015-01-26 00:00:00.0000000 -04:00', 'Administrator', '2015-01-26 00:00:00.0000000 -04:00')

	INSERT INTO lookup.tblGasboyEventObjectType (GasboyEventObjectTypeIndex, GasboyEventObjectTypeCode, GasboyEventObjectTypeName, GasboyEventObjectTypeGuid, CreatedBy, CreatedDate, UpdatedBy, UpdatedDate) 
	VALUES (4, 'Station', 'Station', N'F8A2975D-7533-4412-A4AA-CB4984F54128', 'Administrator', '2015-01-26 00:00:00.0000000 -04:00', 'Administrator', '2015-01-26 00:00:00.0000000 -04:00')

	INSERT INTO lookup.tblGasboyEventObjectType (GasboyEventObjectTypeIndex, GasboyEventObjectTypeCode, GasboyEventObjectTypeName, GasboyEventObjectTypeGuid, CreatedBy, CreatedDate, UpdatedBy, UpdatedDate) 
	VALUES (5, 'Mean', 'Mean', N'7ACD6ACF-12B6-4CB1-B8A5-20F7E6B7E8B3', 'Administrator', '2015-01-26 00:00:00.0000000 -04:00', 'Administrator', '2015-01-26 00:00:00.0000000 -04:00')

	INSERT INTO lookup.tblGasboyEventObjectType (GasboyEventObjectTypeIndex, GasboyEventObjectTypeCode, GasboyEventObjectTypeName, GasboyEventObjectTypeGuid, CreatedBy, CreatedDate, UpdatedBy, UpdatedDate) 
	VALUES (6, 'Shift', 'Shift', N'A7E5DCB0-CA1E-4B85-9612-2ED37A11A5FA', 'Administrator', '2015-01-26 00:00:00.0000000 -04:00', 'Administrator', '2015-01-26 00:00:00.0000000 -04:00')

	INSERT INTO lookup.tblGasboyEventObjectType (GasboyEventObjectTypeIndex, GasboyEventObjectTypeCode, GasboyEventObjectTypeName, GasboyEventObjectTypeGuid, CreatedBy, CreatedDate, UpdatedBy, UpdatedDate) 
	VALUES (7, 'Bus', 'Bus', N'4EF9465D-E5BD-44A5-8D68-EFB940FA6675', 'Administrator', '2015-01-26 00:00:00.0000000 -04:00', 'Administrator', '2015-01-26 00:00:00.0000000 -04:00')

	INSERT INTO lookup.tblGasboyEventObjectType (GasboyEventObjectTypeIndex, GasboyEventObjectTypeCode, GasboyEventObjectTypeName, GasboyEventObjectTypeGuid, CreatedBy, CreatedDate, UpdatedBy, UpdatedDate) 
	VALUES (8, 'Nozzle', 'Nozzle', N'CD98512B-96C4-4BE6-989C-C30A2D2ECB9A', 'Administrator', '2015-01-26 00:00:00.0000000 -04:00', 'Administrator', '2015-01-26 00:00:00.0000000 -04:00')

	INSERT INTO lookup.tblGasboyEventObjectType (GasboyEventObjectTypeIndex, GasboyEventObjectTypeCode, GasboyEventObjectTypeName, GasboyEventObjectTypeGuid, CreatedBy, CreatedDate, UpdatedBy, UpdatedDate) 
	VALUES (9, 'Sensor', 'Sensor', N'581D98D3-D71D-49AC-9236-C424CF629217', 'Administrator', '2015-01-26 00:00:00.0000000 -04:00', 'Administrator', '2015-01-26 00:00:00.0000000 -04:00')

	INSERT INTO lookup.tblGasboyEventObjectType (GasboyEventObjectTypeIndex, GasboyEventObjectTypeCode, GasboyEventObjectTypeName, GasboyEventObjectTypeGuid, CreatedBy, CreatedDate, UpdatedBy, UpdatedDate) 
	VALUES (10, 'Input', 'Input', N'07F48E56-B269-4B4B-B6EB-1B89AFC12328', 'Administrator', '2015-01-26 00:00:00.0000000 -04:00', 'Administrator', '2015-01-26 00:00:00.0000000 -04:00')
END

IF (SELECT COUNT(*) FROM [lookup].tblGasboyVehiclePlateCheckType) = 0
BEGIN
	INSERT INTO lookup.tblGasboyVehiclePlateCheckType ( GasboyVehiclePlateCheckTypeIndex, GasboyVehiclePlateCheckTypeCode, GasboyVehiclePlateCheckTypeName, GasboyVehiclePlateCheckTypeGuid, CreatedBy, CreatedDate, UpdatedBy, UpdatedDate)
	VALUES (1, 'ValidVehicleNo' , 'Valid Vehicle No', N'4AF86C77-E685-4E0F-A796-49AC7027A26D', 'Administrator', '2015-01-26 00:00:00.0000000 -04:00', 'Administrator', '2015-01-26 00:00:00.0000000 -04:00')

	INSERT INTO lookup.tblGasboyVehiclePlateCheckType ( GasboyVehiclePlateCheckTypeIndex, GasboyVehiclePlateCheckTypeCode, GasboyVehiclePlateCheckTypeName, GasboyVehiclePlateCheckTypeGuid, CreatedBy, CreatedDate, UpdatedBy, UpdatedDate)
	VALUES (2, 'ValidDeviceName' , 'Valid Device Name No', N'4D5398F1-A339-4BEC-85FB-55E328942D1C', 'Administrator', '2015-01-26 00:00:00.0000000 -04:00', 'Administrator', '2015-01-26 00:00:00.0000000 -04:00')

	INSERT INTO lookup.tblGasboyVehiclePlateCheckType ( GasboyVehiclePlateCheckTypeIndex, GasboyVehiclePlateCheckTypeCode, GasboyVehiclePlateCheckTypeName, GasboyVehiclePlateCheckTypeGuid, CreatedBy, CreatedDate, UpdatedBy, UpdatedDate)
	VALUES (3, 'SaveAndCaptureOnly' , 'Save And Capture Only', N'41DAB1DF-C092-44BD-B49D-5669ED3F3F1D', 'Administrator', '2015-01-26 00:00:00.0000000 -04:00', 'Administrator', '2015-01-26 00:00:00.0000000 -04:00')

	INSERT INTO lookup.tblGasboyVehiclePlateCheckType ( GasboyVehiclePlateCheckTypeIndex, GasboyVehiclePlateCheckTypeCode, GasboyVehiclePlateCheckTypeName, GasboyVehiclePlateCheckTypeGuid, CreatedBy, CreatedDate, UpdatedBy, UpdatedDate)
	VALUES (4, 'ValidVehicleNoForCurrentDevice' , 'Valid Vehicle No For Current Device', N'D5B243EA-780D-419C-977B-358D65B19FF2', 'Administrator', '2015-01-26 00:00:00.0000000 -04:00', 'Administrator', '2015-01-26 00:00:00.0000000 -04:00')
END

IF (SELECT COUNT(*) FROM [lookup].tblGasboyRecordStatus) = 0
BEGIN
	INSERT INTO lookup.tblGasboyRecordStatus ( GasboyRecordStatusIndex, GasboyRecordStatusCode, GasboyRecordStatusName, GasboyRecordStatusGuid, CreatedBy, CreatedDate, UpdatedBy, UpdatedDate)
	VALUES (0, 'Deleted' , 'Deleted', N'354729C2-8117-4EB6-A0D2-4C8DB29CBE89', 'Administrator', '2015-01-26 00:00:00.0000000 -04:00', 'Administrator', '2015-01-26 00:00:00.0000000 -04:00')

	INSERT INTO lookup.tblGasboyRecordStatus ( GasboyRecordStatusIndex, GasboyRecordStatusCode, GasboyRecordStatusName, GasboyRecordStatusGuid, CreatedBy, CreatedDate, UpdatedBy, UpdatedDate)
	VALUES (1, 'Blocked' , 'Blocked', N'A30F8FDD-2B69-4A8D-BF96-9EE4AC406D76', 'Administrator', '2015-01-26 00:00:00.0000000 -04:00', 'Administrator', '2015-01-26 00:00:00.0000000 -04:00')

	INSERT INTO lookup.tblGasboyRecordStatus ( GasboyRecordStatusIndex, GasboyRecordStatusCode, GasboyRecordStatusName, GasboyRecordStatusGuid, CreatedBy, CreatedDate, UpdatedBy, UpdatedDate)
	VALUES (2, 'Active' , 'Active', N'051748F2-70B6-4AB5-A971-336A95C5A40C', 'Administrator', '2015-01-26 00:00:00.0000000 -04:00', 'Administrator', '2015-01-26 00:00:00.0000000 -04:00')
END

IF (SELECT COUNT(*) FROM [lookup].tblGasboyDeviceType) = 0
BEGIN
	INSERT INTO lookup.tblGasboyDeviceType ( GasboyDeviceTypeIndex, GasboyDeviceTypeCode, GasboyDeviceTypeName, GasboyDeviceTypeGuid, CreatedBy, CreatedDate, UpdatedBy, UpdatedDate)
	VALUES (1, 'EmployeeTag' , 'Employee Tag', N'22C57FD0-733A-4DAE-9684-5AF0DD02997D', 'Administrator', '2015-01-26 00:00:00.0000000 -04:00', 'Administrator', '2015-01-26 00:00:00.0000000 -04:00')

	INSERT INTO lookup.tblGasboyDeviceType ( GasboyDeviceTypeIndex, GasboyDeviceTypeCode, GasboyDeviceTypeName, GasboyDeviceTypeGuid, CreatedBy, CreatedDate, UpdatedBy, UpdatedDate)
	VALUES (2, 'Vehicle' , 'Vehicle', N'F16A4CCF-6354-4DE5-A469-86D15703428E', 'Administrator', '2015-01-26 00:00:00.0000000 -04:00', 'Administrator', '2015-01-26 00:00:00.0000000 -04:00')

	INSERT INTO lookup.tblGasboyDeviceType ( GasboyDeviceTypeIndex, GasboyDeviceTypeCode, GasboyDeviceTypeName, GasboyDeviceTypeGuid, CreatedBy, CreatedDate, UpdatedBy, UpdatedDate)
	VALUES (3, 'VehicleMounted' , 'Vehicle Mounted', N'CBB6130E-57CC-4049-B53F-8562FF21CD17', 'Administrator', '2015-01-26 00:00:00.0000000 -04:00', 'Administrator', '2015-01-26 00:00:00.0000000 -04:00')

	INSERT INTO lookup.tblGasboyDeviceType ( GasboyDeviceTypeIndex, GasboyDeviceTypeCode, GasboyDeviceTypeName, GasboyDeviceTypeGuid, CreatedBy, CreatedDate, UpdatedBy, UpdatedDate)
	VALUES (4, 'Driver' , 'Driver', N'AE6E75E4-83DE-446C-8CA4-9E2E265663EA', 'Administrator', '2015-01-26 00:00:00.0000000 -04:00', 'Administrator', '2015-01-26 00:00:00.0000000 -04:00')
	
	INSERT INTO lookup.tblGasboyDeviceType ( GasboyDeviceTypeIndex, GasboyDeviceTypeCode, GasboyDeviceTypeName, GasboyDeviceTypeGuid, CreatedBy, CreatedDate, UpdatedBy, UpdatedDate)
	VALUES (5, 'CustomerTag' , 'Customer Tag', N'448E2BC5-512C-463D-B5D3-2160A0325EDC', 'Administrator', '2015-01-26 00:00:00.0000000 -04:00', 'Administrator', '2015-01-26 00:00:00.0000000 -04:00')
END

IF (SELECT COUNT(*) FROM [lookup].tblGasboyHardwareType) = 0
BEGIN
	INSERT INTO lookup.tblGasboyHardwareType ( GasboyHardwareTypeIndex, GasboyHardwareTypeCode, GasboyHardwareTypeName, GasboyHardwareTypeGuid, CreatedBy, CreatedDate, UpdatedBy, UpdatedDate)
	VALUES (1, 'Tag' , 'Tag', N'AC702603-C3EF-4748-ADC3-34AF4296C6D5', 'Administrator', '2015-01-26 00:00:00.0000000 -04:00', 'Administrator', '2015-01-26 00:00:00.0000000 -04:00')

	INSERT INTO lookup.tblGasboyHardwareType ( GasboyHardwareTypeIndex, GasboyHardwareTypeCode, GasboyHardwareTypeName, GasboyHardwareTypeGuid, CreatedBy, CreatedDate, UpdatedBy, UpdatedDate)
	VALUES (6, 'Vit' , 'Vit', N'5281ADEA-3A03-426C-99D9-438A06DE70BD', 'Administrator', '2015-01-26 00:00:00.0000000 -04:00', 'Administrator', '2015-01-26 00:00:00.0000000 -04:00')
END

IF (SELECT COUNT(*) FROM [lookup].tblGasboyAuthType) = 0
BEGIN
	INSERT INTO lookup.tblGasboyAuthType ( GasboyAuthTypeIndex, GasboyAuthTypeCode, GasboyAuthTypeName, GasboyAuthTypeGuid, CreatedBy, CreatedDate, UpdatedBy, UpdatedDate)
	VALUES (1, 'Fuelopass' , 'Fuelopass', N'5AF705F6-B125-4993-9C79-ABE5DDC408DB', 'Administrator', '2015-01-26 00:00:00.0000000 -04:00', 'Administrator', '2015-01-26 00:00:00.0000000 -04:00')

	INSERT INTO lookup.tblGasboyAuthType ( GasboyAuthTypeIndex, GasboyAuthTypeCode, GasboyAuthTypeName, GasboyAuthTypeGuid, CreatedBy, CreatedDate, UpdatedBy, UpdatedDate)
	VALUES (14, 'FuelCard' , 'Fuel Card', N'D67379C6-14F6-4F8B-B76E-021D21B1CFF0', 'Administrator', '2015-01-26 00:00:00.0000000 -04:00', 'Administrator', '2015-01-26 00:00:00.0000000 -04:00')

	INSERT INTO lookup.tblGasboyAuthType ( GasboyAuthTypeIndex, GasboyAuthTypeCode, GasboyAuthTypeName, GasboyAuthTypeGuid, CreatedBy, CreatedDate, UpdatedBy, UpdatedDate)
	VALUES (21, 'ManualEntry' , 'Manual Entry', N'76A37DF6-B323-4B99-809F-AFED3BB77508', 'Administrator', '2015-01-26 00:00:00.0000000 -04:00', 'Administrator', '2015-01-26 00:00:00.0000000 -04:00')
END

IF (SELECT COUNT(*) FROM [lookup].tblGasboyEmployeeType) = 0
BEGIN
	INSERT INTO lookup.tblGasboyEmployeeType ( GasboyEmployeeTypeIndex, GasboyEmployeeTypeCode, GasboyEmployeeTypeName, GasboyEmployeeTypeGuid, CreatedBy, CreatedDate, UpdatedBy, UpdatedDate)
	VALUES (1, 'Attendant' , 'Attendant', N'E057A5CD-A437-4D7D-9993-AC679C298ACB', 'Administrator', '2015-01-26 00:00:00.0000000 -04:00', 'Administrator', '2015-01-26 00:00:00.0000000 -04:00')

	INSERT INTO lookup.tblGasboyEmployeeType ( GasboyEmployeeTypeIndex, GasboyEmployeeTypeCode, GasboyEmployeeTypeName, GasboyEmployeeTypeGuid, CreatedBy, CreatedDate, UpdatedBy, UpdatedDate)
	VALUES (2, 'ShiftManager' , 'Shift Manager', N'9D7F925E-145E-4BA4-91F0-2824E61875E6', 'Administrator', '2015-01-26 00:00:00.0000000 -04:00', 'Administrator', '2015-01-26 00:00:00.0000000 -04:00')
END

IF (SELECT COUNT(*) FROM [lookup].tblGasboyTwoStageDriverValidationType) = 0
BEGIN
	INSERT INTO lookup.tblGasboyTwoStageDriverValidationType ( GasboyTwoStageDriverValidationTypeIndex, GasboyTwoStageDriverValidationTypeCode, GasboyTwoStageDriverValidationTypeName, GasboyTwoStageDriverValidationTypeGuid, CreatedBy, CreatedDate, UpdatedBy, UpdatedDate)
	VALUES (0, 'NotSelected' , 'Not Selected', N'2274BBAB-4652-40B2-8B26-B27473824464', 'Administrator', '2015-01-26 00:00:00.0000000 -04:00', 'Administrator', '2015-01-26 00:00:00.0000000 -04:00')

	INSERT INTO lookup.tblGasboyTwoStageDriverValidationType ( GasboyTwoStageDriverValidationTypeIndex, GasboyTwoStageDriverValidationTypeCode, GasboyTwoStageDriverValidationTypeName, GasboyTwoStageDriverValidationTypeGuid, CreatedBy, CreatedDate, UpdatedBy, UpdatedDate)
	VALUES (2, 'SelectedDrivers' , 'Selected Drivers', N'DABFD363-EE74-4DF3-B772-B21B3EDB8EDD', 'Administrator', '2015-01-26 00:00:00.0000000 -04:00', 'Administrator', '2015-01-26 00:00:00.0000000 -04:00')

	INSERT INTO lookup.tblGasboyTwoStageDriverValidationType ( GasboyTwoStageDriverValidationTypeIndex, GasboyTwoStageDriverValidationTypeCode, GasboyTwoStageDriverValidationTypeName, GasboyTwoStageDriverValidationTypeGuid, CreatedBy, CreatedDate, UpdatedBy, UpdatedDate)
	VALUES (3, 'SelectedDepartments' , 'Selected Departments', N'12BC706E-8008-4807-894A-7C0A841D1819', 'Administrator', '2015-01-26 00:00:00.0000000 -04:00', 'Administrator', '2015-01-26 00:00:00.0000000 -04:00')

	INSERT INTO lookup.tblGasboyTwoStageDriverValidationType ( GasboyTwoStageDriverValidationTypeIndex, GasboyTwoStageDriverValidationTypeCode, GasboyTwoStageDriverValidationTypeName, GasboyTwoStageDriverValidationTypeGuid, CreatedBy, CreatedDate, UpdatedBy, UpdatedDate)
	VALUES (4, 'AnyDriverSameFleet' , 'Any Driver Same Fleet', N'E63FC327-4ED0-49FC-8430-ADE78531629C', 'Administrator', '2015-01-26 00:00:00.0000000 -04:00', 'Administrator', '2015-01-26 00:00:00.0000000 -04:00')

	INSERT INTO lookup.tblGasboyTwoStageDriverValidationType ( GasboyTwoStageDriverValidationTypeIndex, GasboyTwoStageDriverValidationTypeCode, GasboyTwoStageDriverValidationTypeName, GasboyTwoStageDriverValidationTypeGuid, CreatedBy, CreatedDate, UpdatedBy, UpdatedDate)
	VALUES (5, 'AnyDriverAnyFleet' , 'Any Driver Any Fleet', N'4F912F50-3D36-4A20-A052-8779B917D70D', 'Administrator', '2015-01-26 00:00:00.0000000 -04:00', 'Administrator', '2015-01-26 00:00:00.0000000 -04:00')
END

IF (SELECT COUNT(*) FROM [dbo].[tblGasboyFleet]) = 0
BEGIN
	INSERT INTO [dbo].[tblGasboyFleet] ( GasboyFleetGuid, SiteGuid, FleetCode, FleetName, LookupGasboyRecordStatusIndex, UsePINCodeFlag, PINCode, AuthPINFrom, PromptForVehiclePlateFlag, LookupGasboyVehiclePlateCheckTypeIndex, AlwaysPromptForAdditionalValidationFlag, CreatedBy, CreatedDate, UpdatedBy, UpdatedDate, FleetID)
	VALUES (N'00000009-0000-0000-0000-000000000000', N'00000000-0000-0000-0000-000000000001', 1, 'Default Fleet', 2, 0, CAST(N'9999' as VARBINARY), 1, 0, 1, 0, 'Administrator', '2017-02-02 00:00:00.0000000 -04:00', 'Administrator', '2017-02-02 00:00:00.0000000 -04:00', 900000001)
END

IF (SELECT COUNT(*) FROM [dbo].[tblGasboyDepartment]) = 0
BEGIN
	INSERT INTO [dbo].[tblGasboyDepartment] (GasboyDepartmentGuid, SiteGuid, DepartmentCode, DepartmentName, LookupGasboyRecordStatusIndex, UsePINCodeFlag, PINCode, AuthPINFrom, PromptForVehiclePlateFlag, LookupGasboyVehiclePlateCheckTypeIndex, AlwaysPromptForAdditionalValidationFlag, CreatedBy, CreatedDate, UpdatedBy, UpdatedDate, DepartmentID)
	VALUES (N'00000001-0000-0000-0000-000000000000', N'00000000-0000-0000-0000-000000000001', 9999, 'Blacklist Department', 2, 0, CONVERT(VARBINARY(25), '0x', 1), 1, 0, 3, 0, 'Administrator', '2017-02-02 00:00:00.0000000 -04:00', 'Administrator', '2017-02-02 00:00:00.0000000 -04:00', 900000003)

	INSERT INTO [dbo].[tblGasboyDepartment] (GasboyDepartmentGuid, SiteGuid, DepartmentCode, DepartmentName, LookupGasboyRecordStatusIndex, UsePINCodeFlag, PINCode, AuthPINFrom, PromptForVehiclePlateFlag, LookupGasboyVehiclePlateCheckTypeIndex, AlwaysPromptForAdditionalValidationFlag, CreatedBy, CreatedDate, UpdatedBy, UpdatedDate, DepartmentID)
	VALUES (N'00000002-0000-0000-0000-000000000000', N'00000000-0000-0000-0000-000000000001', 9998, 'Default Department', 2, 0, CONVERT(VARBINARY(25), '0x', 1), 1, 0, 3, 0, 'Administrator', '2017-02-02 00:00:00.0000000 -04:00', 'Administrator', '2017-02-02 00:00:00.0000000 -04:00', 900000002)
END

IF (SELECT COUNT(*) FROM [map].[tblGasboyDepartmentToGasboyFleet]) = 0
BEGIN
	INSERT INTO [map].[tblGasboyDepartmentToGasboyFleet] (GasboyDepartmentToGasboyFleetGuid, GasboyFleetGuid, GasboyDepartmentGuid, CreatedBy, CreatedDate, UpdatedBy, UpdatedDate)
	VALUES (N'88262C44-06B7-4048-9444-AF7B37624C5A', N'00000009-0000-0000-0000-000000000000', N'00000001-0000-0000-0000-000000000000', 'Administrator', '2017-02-02 00:00:00.0000000 -04:00', 'Administrator', '2017-02-02 00:00:00.0000000 -04:00')

	INSERT INTO [map].[tblGasboyDepartmentToGasboyFleet] (GasboyDepartmentToGasboyFleetGuid, GasboyFleetGuid, GasboyDepartmentGuid, CreatedBy, CreatedDate, UpdatedBy, UpdatedDate)
	VALUES (N'342AE092-6D22-4D4D-B5EE-A0829F8620AB', N'00000009-0000-0000-0000-000000000000', N'00000002-0000-0000-0000-000000000000', 'Administrator', '2017-02-02 00:00:00.0000000 -04:00', 'Administrator', '2017-02-02 00:00:00.0000000 -04:00')
END

IF (SELECT COUNT(*) FROM [map].[tblEntityGasboyDepartmentToSite]) = 0
BEGIN
	INSERT INTO [map].[tblEntityGasboyDepartmentToSite] (GasboyDepartmentToSiteGuid, GasboyDepartmentGuid, SiteGuid, AssignedFromSiteGuid, CreatedBy, CreatedDate, UpdatedBy,UpdatedDate)
	VALUES (N'2BC1EED4-6385-4A4E-ACF9-F633F2B006C4', N'00000001-0000-0000-0000-000000000000', N'00000000-0000-0000-0000-000000000001', N'00000000-0000-0000-0000-000000000001', 'Administrator', '2017-02-02 00:00:00.0000000 -04:00', 'Administrator', '2017-02-02 00:00:00.0000000 -04:00')

	INSERT INTO [map].[tblEntityGasboyDepartmentToSite] (GasboyDepartmentToSiteGuid, GasboyDepartmentGuid, SiteGuid, AssignedFromSiteGuid, CreatedBy, CreatedDate, UpdatedBy,UpdatedDate)
	VALUES (N'4A0121CC-F650-4688-89D5-14195B778D10', N'00000002-0000-0000-0000-000000000000', N'00000000-0000-0000-0000-000000000001', N'00000000-0000-0000-0000-000000000001', 'Administrator', '2017-02-02 00:00:00.0000000 -04:00', 'Administrator', '2017-02-02 00:00:00.0000000 -04:00')
END

IF (SELECT COUNT(*) FROM [dbo].[tblAlarmAndEvents] WHERE (Source = 'Gasboy')) = 0
BEGIN
INSERT INTO [dbo].[tblAlarmAndEvents] (Source,Alarm,ID,CategoryIndex,PriorityIndex,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy,Enabled,AlarmAndEventGuid,SiteGuid) 
VALUES ('Gasboy', 0, 'Test Connection Success', null, null, '2015-06-04 01:04:40.8974146 +00:00', 'Administrator', '2015-06-04 01:04:40.8974146 +00:00','Administrator', 1, 'F4671831-53F7-49BB-A7AE-5264C2301BDF','00000000-0000-0000-0000-000000000001')

INSERT INTO [dbo].[tblAlarmAndEvents] (Source,Alarm,ID,CategoryIndex,PriorityIndex,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy,Enabled,AlarmAndEventGuid,SiteGuid) 
VALUES ('Gasboy', 0, 'Test connection error', null, null, '2015-06-04 01:04:40.8974146 +00:00', 'Administrator', '2015-06-04 01:04:40.8974146 +00:00','Administrator', 1, '9AA35B01-6AC8-484A-8701-E7DDD0979042','00000000-0000-0000-0000-000000000001')

INSERT INTO [dbo].[tblAlarmAndEvents] (Source,Alarm,ID,CategoryIndex,PriorityIndex,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy,Enabled,AlarmAndEventGuid,SiteGuid) 
VALUES ('Gasboy', 0, 'Gasboy Station Manual Transaction Download Initiated', null, null, '2015-06-04 01:04:40.8974146 +00:00', 'Administrator', '2015-06-04 01:04:40.8974146 +00:00','Administrator', 1, '75C0AE43-48F3-4C88-B4D6-00FEDE9CAC47','00000000-0000-0000-0000-000000000001')

INSERT INTO [dbo].[tblAlarmAndEvents] (Source,Alarm,ID,CategoryIndex,PriorityIndex,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy,Enabled,AlarmAndEventGuid,SiteGuid) 
VALUES ('Gasboy', 0, 'Gasboy Station Manual Transaction Download Complete', null, null, '2015-06-04 01:04:40.8974146 +00:00', 'Administrator', '2015-06-04 01:04:40.8974146 +00:00','Administrator', 1, 'BECB25F3-D35B-4E8D-AF51-9E6D0295A8DB','00000000-0000-0000-0000-000000000001')

INSERT INTO [dbo].[tblAlarmAndEvents] (Source,Alarm,ID,CategoryIndex,PriorityIndex,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy,Enabled,AlarmAndEventGuid,SiteGuid) 
VALUES ('Gasboy', 0, 'Gasboy Station Manual Transaction Download Error', null, null, '2015-06-04 01:04:40.8974146 +00:00', 'Administrator', '2015-06-04 01:04:40.8974146 +00:00','Administrator', 1, 'DAE690EB-8D05-4355-8771-18FD46C742CA','00000000-0000-0000-0000-000000000001')

INSERT INTO [dbo].[tblAlarmAndEvents] (Source,Alarm,ID,CategoryIndex,PriorityIndex,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy,Enabled,AlarmAndEventGuid,SiteGuid) 
VALUES ('Gasboy', 0, 'Gasboy Station Device Push Initiated', null, null, '2015-06-04 01:04:40.8974146 +00:00', 'Administrator', '2015-06-04 01:04:40.8974146 +00:00','Administrator', 1, '925D70E6-3469-416E-A3A6-52E362FEA4C2','00000000-0000-0000-0000-000000000001')

INSERT INTO [dbo].[tblAlarmAndEvents] (Source,Alarm,ID,CategoryIndex,PriorityIndex,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy,Enabled,AlarmAndEventGuid,SiteGuid) 
VALUES ('Gasboy', 0, 'Gasboy Station Device Push Complete', null, null, '2015-06-04 01:04:40.8974146 +00:00', 'Administrator', '2015-06-04 01:04:40.8974146 +00:00','Administrator', 1, 'C22121EE-D3BF-442D-8A2D-41E44F7014C7','00000000-0000-0000-0000-000000000001')

INSERT INTO [dbo].[tblAlarmAndEvents] (Source,Alarm,ID,CategoryIndex,PriorityIndex,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy,Enabled,AlarmAndEventGuid,SiteGuid) 
VALUES ('Gasboy', 0, 'Gasboy Station Device Push Error', null, null, '2015-06-04 01:04:40.8974146 +00:00', 'Administrator', '2015-06-04 01:04:40.8974146 +00:00','Administrator', 1, 'F6B5F65F-0884-42B8-89C7-A2F00B73FB68','00000000-0000-0000-0000-000000000001')

INSERT INTO [dbo].[tblAlarmAndEvents] (Source,Alarm,ID,CategoryIndex,PriorityIndex,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy,Enabled,AlarmAndEventGuid,SiteGuid) 
VALUES ('Gasboy', 0, 'Gasboy Station Periodic Transaction Download Initiated', null, null, '2015-06-04 01:04:40.8974146 +00:00', 'Administrator', '2015-06-04 01:04:40.8974146 +00:00','Administrator', 1, 'CD24BB72-9958-4175-AE3C-234DAC497447','00000000-0000-0000-0000-000000000001')

INSERT INTO [dbo].[tblAlarmAndEvents] (Source,Alarm,ID,CategoryIndex,PriorityIndex,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy,Enabled,AlarmAndEventGuid,SiteGuid) 
VALUES ('Gasboy', 0, 'Gasboy Station Periodic Transaction Download Complete', null, null, '2015-06-04 01:04:40.8974146 +00:00', 'Administrator', '2015-06-04 01:04:40.8974146 +00:00','Administrator', 1, 'F51E5D92-4852-40A6-A139-EA0A7D33A7D7','00000000-0000-0000-0000-000000000001')

INSERT INTO [dbo].[tblAlarmAndEvents] (Source,Alarm,ID,CategoryIndex,PriorityIndex,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy,Enabled,AlarmAndEventGuid,SiteGuid) 
VALUES ('Gasboy', 0, 'Gasboy Station Periodic Transaction Download Error', null, null, '2015-06-04 01:04:40.8974146 +00:00', 'Administrator', '2015-06-04 01:04:40.8974146 +00:00','Administrator', 1, '755719E0-E0E7-4241-880A-672D37AA2E21','00000000-0000-0000-0000-000000000001')

INSERT INTO [dbo].[tblAlarmAndEvents] (Source,Alarm,ID,CategoryIndex,PriorityIndex,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy,Enabled,AlarmAndEventGuid,SiteGuid) 
VALUES ('Gasboy', 0, 'Gasboy Transaction Import Initiated', null, null, '2015-06-04 01:04:40.8974146 +00:00', 'Administrator', '2015-06-04 01:04:40.8974146 +00:00','Administrator', 1, '56313E0B-0962-4FBC-ADE1-DFB37674FE9F','00000000-0000-0000-0000-000000000001')

INSERT INTO [dbo].[tblAlarmAndEvents] (Source,Alarm,ID,CategoryIndex,PriorityIndex,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy,Enabled,AlarmAndEventGuid,SiteGuid) 
VALUES ('Gasboy', 0, 'Gasboy Transaction Import Completed', null, null, '2015-06-04 01:04:40.8974146 +00:00', 'Administrator', '2015-06-04 01:04:40.8974146 +00:00','Administrator', 1, '843FF7D3-4D77-401C-B640-19CF12A7CE52','00000000-0000-0000-0000-000000000001')

INSERT INTO [dbo].[tblAlarmAndEvents] (Source,Alarm,ID,CategoryIndex,PriorityIndex,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy,Enabled,AlarmAndEventGuid,SiteGuid) 
VALUES ('Gasboy', 0, 'Gasboy Transaction Import Error', null, null, '2015-06-04 01:04:40.8974146 +00:00', 'Administrator', '2015-06-04 01:04:40.8974146 +00:00','Administrator', 1, '6758A445-1AB4-4882-94EC-8B6BFFC3E617','00000000-0000-0000-0000-000000000001')

INSERT INTO [dbo].[tblAlarmAndEvents] (Source,Alarm,ID,CategoryIndex,PriorityIndex,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy,Enabled,AlarmAndEventGuid,SiteGuid) 
VALUES ('Gasboy', 0, 'Gasboy Reprocess Transaction Initiated', null, null, '2015-06-04 01:04:40.8974146 +00:00', 'Administrator', '2015-06-04 01:04:40.8974146 +00:00','Administrator', 1, '15A78335-4E59-45DB-8210-76F97D3B8412','00000000-0000-0000-0000-000000000001')

INSERT INTO [dbo].[tblAlarmAndEvents] (Source,Alarm,ID,CategoryIndex,PriorityIndex,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy,Enabled,AlarmAndEventGuid,SiteGuid) 
VALUES ('Gasboy', 0, 'Gasboy Reprocess Transaction Completed', null, null, '2015-06-04 01:04:40.8974146 +00:00', 'Administrator', '2015-06-04 01:04:40.8974146 +00:00','Administrator', 1, 'F7D18485-4ACE-4136-BE18-408E3D73CDED','00000000-0000-0000-0000-000000000001')

INSERT INTO [dbo].[tblAlarmAndEvents] (Source,Alarm,ID,CategoryIndex,PriorityIndex,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy,Enabled,AlarmAndEventGuid,SiteGuid) 
VALUES ('Gasboy', 0, 'Gasboy Reprocess Transaction Error', null, null, '2015-06-04 01:04:40.8974146 +00:00', 'Administrator', '2015-06-04 01:04:40.8974146 +00:00','Administrator', 1, 'E83323B8-4B7F-4225-A2C7-D1321037409E','00000000-0000-0000-0000-000000000001')

INSERT INTO [dbo].[tblAlarmAndEvents] (Source,Alarm,ID,CategoryIndex,PriorityIndex,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy,Enabled,AlarmAndEventGuid,SiteGuid) 
VALUES ('Gasboy', 0, 'Gasboy Online Authorization Denied', null, null, '2015-06-04 01:04:40.8974146 +00:00', 'Administrator', '2015-06-04 01:04:40.8974146 +00:00','Administrator', 1, '42ACDDED-A1A6-4BEC-B708-C905F7B24D99','00000000-0000-0000-0000-000000000001')

INSERT INTO [dbo].[tblAlarmAndEvents] (Source,Alarm,ID,CategoryIndex,PriorityIndex,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy,Enabled,AlarmAndEventGuid,SiteGuid) 
VALUES ('Gasboy', 0, 'Gasboy Online Authorization Approved', null, null, '2015-06-04 01:04:40.8974146 +00:00', 'Administrator', '2015-06-04 01:04:40.8974146 +00:00','Administrator', 1, '19B5AD55-9C07-4DAB-B9F8-110C09F239B7','00000000-0000-0000-0000-000000000001')

INSERT INTO [dbo].[tblAlarmAndEvents] (Source,Alarm,ID,CategoryIndex,PriorityIndex,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy,Enabled,AlarmAndEventGuid,SiteGuid) 
VALUES ('Gasboy', 0, 'Gasboy Event Collection Complete', null, null, '2015-06-04 01:04:40.8974146 +00:00', 'Administrator', '2015-06-04 01:04:40.8974146 +00:00','Administrator', 1, '555AB2CD-6172-4A1E-B0D2-2FCDB9F6A261','00000000-0000-0000-0000-000000000001')

INSERT INTO [dbo].[tblAlarmAndEvents] (Source,Alarm,ID,CategoryIndex,PriorityIndex,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy,Enabled,AlarmAndEventGuid,SiteGuid) 
VALUES ('Gasboy', 0, 'Gasboy Duplicate Transaction Rejected', null, null, '2015-06-04 01:04:40.8974146 +00:00', 'Administrator', '2015-06-04 01:04:40.8974146 +00:00','Administrator', 1, '007479BD-1225-46DA-9E32-454D26BA6EA1','00000000-0000-0000-0000-000000000001')

INSERT INTO [dbo].[tblAlarmAndEvents] (Source,Alarm,ID,CategoryIndex,PriorityIndex,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy,Enabled,AlarmAndEventGuid,SiteGuid) 
VALUES ('Gasboy', 0, 'Gasboy Initiated Transaction Transfer Received', null, null, '2015-06-04 01:04:40.8974146 +00:00', 'Administrator', '2015-06-04 01:04:40.8974146 +00:00','Administrator', 1, 'E08379D7-187D-4CE7-9D25-6FC7375FF67D','00000000-0000-0000-0000-000000000001')

INSERT INTO [dbo].[tblAlarmAndEvents] (Source,Alarm,ID,CategoryIndex,PriorityIndex,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy,Enabled,AlarmAndEventGuid,SiteGuid) 
VALUES ('Gasboy', 0, 'Gasboy Initiated Transaction Transfer Error', null, null, '2015-06-04 01:04:40.8974146 +00:00', 'Administrator', '2015-06-04 01:04:40.8974146 +00:00','Administrator', 1, 'E213CCFF-A0FA-4828-8FDE-10F50ADD62B5','00000000-0000-0000-0000-000000000001')

INSERT INTO [dbo].[tblAlarmAndEvents] (Source,Alarm,ID,CategoryIndex,PriorityIndex,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy,Enabled,AlarmAndEventGuid,SiteGuid) 
VALUES ('Gasboy', 0, 'Gasboy Initiated Fleet Data Transfer Complete', null, null, '2015-06-04 01:04:40.8974146 +00:00', 'Administrator', '2015-06-04 01:04:40.8974146 +00:00','Administrator', 1, '8001F905-F974-4065-8EAE-836362906BE2','00000000-0000-0000-0000-000000000001')

INSERT INTO [dbo].[tblAlarmAndEvents] (Source,Alarm,ID,CategoryIndex,PriorityIndex,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy,Enabled,AlarmAndEventGuid,SiteGuid) 
VALUES ('Gasboy', 0, 'Gasboy Initiated Department Data Transfer Complete', null, null, '2015-06-04 01:04:40.8974146 +00:00', 'Administrator', '2015-06-04 01:04:40.8974146 +00:00','Administrator', 1, '73223D7E-56E2-421E-BA36-D018777575C4','00000000-0000-0000-0000-000000000001')

INSERT INTO [dbo].[tblAlarmAndEvents] (Source,Alarm,ID,CategoryIndex,PriorityIndex,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy,Enabled,AlarmAndEventGuid,SiteGuid) 
VALUES ('Gasboy', 0, 'Gasboy Initiated Mean/Device Data Transfer Complete', null, null, '2015-06-04 01:04:40.8974146 +00:00', 'Administrator', '2015-06-04 01:04:40.8974146 +00:00','Administrator', 1, 'C0CE6303-32D0-4FB3-B934-EC736B74049D','00000000-0000-0000-0000-000000000001')

INSERT INTO [dbo].[tblAlarmAndEvents] (Source,Alarm,ID,CategoryIndex,PriorityIndex,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy,Enabled,AlarmAndEventGuid,SiteGuid) 
VALUES ('Gasboy', 0, 'Gasboy Initiated Data Transfer Error', null, null, '2015-06-04 01:04:40.8974146 +00:00', 'Administrator', '2015-06-04 01:04:40.8974146 +00:00','Administrator', 1, 'BEFABA81-F087-4765-AB2D-0A453081A769','00000000-0000-0000-0000-000000000001')

INSERT INTO [dbo].[tblAlarmAndEvents] (Source,Alarm,ID,CategoryIndex,PriorityIndex,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy,Enabled,AlarmAndEventGuid,SiteGuid) 
VALUES ('Gasboy', 0, 'Gasboy Initiated Event Data Transfer Complete', null, null, '2015-06-04 01:04:40.8974146 +00:00', 'Administrator', '2015-06-04 01:04:40.8974146 +00:00','Administrator', 1, 'E09BB24D-53F9-4EEF-ADAE-A77D1BD041C8','00000000-0000-0000-0000-000000000001')


END

IF (SELECT COUNT(*) FROM [lookup].[tblGasboyErrorCode]) = 0
BEGIN
INSERT INTO [lookup].[tblGasboyErrorCode] (GasboyErrorCodeIndex, GasboyErrorCode, GasboyErrorCodeName, GasboyErrorCodeGuid, CreatedBy, CreatedDate, UpdatedBy, UpdatedDate)
VALUES (-107, 'TimeOut', 'Station Time Out','94BE509A-BBE4-43D7-9D99-A148DDD2272C' , 'Administrator',  '2015-06-04 01:04:40.8974146 +00:00','Administrator','2015-06-04 01:04:40.8974146 +00:00')

INSERT INTO [lookup].[tblGasboyErrorCode] (GasboyErrorCodeIndex, GasboyErrorCode, GasboyErrorCodeName, GasboyErrorCodeGuid, CreatedBy, CreatedDate, UpdatedBy, UpdatedDate)
VALUES (-921, 'DriverActive', 'Driver Active','3F09E778-9DA4-4C18-8361-5E5EE860C6CC' , 'Administrator',  '2015-06-04 01:04:40.8974146 +00:00','Administrator','2015-06-04 01:04:40.8974146 +00:00')

INSERT INTO [lookup].[tblGasboyErrorCode] (GasboyErrorCodeIndex, GasboyErrorCode, GasboyErrorCodeName, GasboyErrorCodeGuid, CreatedBy, CreatedDate, UpdatedBy, UpdatedDate)
VALUES (-940, 'DeviceDefunct', 'Communication Error with Device','8B395584-E351-4CF5-916D-96753BA6FFC0' , 'Administrator',  '2015-06-04 01:04:40.8974146 +00:00','Administrator','2015-06-04 01:04:40.8974146 +00:00')

INSERT INTO [lookup].[tblGasboyErrorCode] (GasboyErrorCodeIndex, GasboyErrorCode, GasboyErrorCodeName, GasboyErrorCodeGuid, CreatedBy, CreatedDate, UpdatedBy, UpdatedDate)
VALUES (-941, 'DeviceNormal', 'Communication Restored with Device','32ED74C5-5501-480B-A330-53C921511172' , 'Administrator',  '2015-06-04 01:04:40.8974146 +00:00','Administrator','2015-06-04 01:04:40.8974146 +00:00')

INSERT INTO [lookup].[tblGasboyErrorCode] (GasboyErrorCodeIndex, GasboyErrorCode, GasboyErrorCodeName, GasboyErrorCodeGuid, CreatedBy, CreatedDate, UpdatedBy, UpdatedDate)
VALUES (-104, 'STXExpected', 'STX Expected','A9BB042F-DE17-4CA5-91DB-29B1F55982E0' , 'Administrator',  '2015-06-04 01:04:40.8974146 +00:00','Administrator','2015-06-04 01:04:40.8974146 +00:00')

INSERT INTO [lookup].[tblGasboyErrorCode] (GasboyErrorCodeIndex, GasboyErrorCode, GasboyErrorCodeName, GasboyErrorCodeGuid, CreatedBy, CreatedDate, UpdatedBy, UpdatedDate)
VALUES (-411, 'FailedAuthorize', 'Failed to Authorize','EF72B293-461C-4700-B731-09291927867C' , 'Administrator',  '2015-06-04 01:04:40.8974146 +00:00','Administrator','2015-06-04 01:04:40.8974146 +00:00')

INSERT INTO [lookup].[tblGasboyErrorCode] (GasboyErrorCodeIndex, GasboyErrorCode, GasboyErrorCodeName, GasboyErrorCodeGuid, CreatedBy, CreatedDate, UpdatedBy, UpdatedDate)
VALUES (-403, 'ShiftStart', 'Shift Start','C5B6C05D-03BD-4A99-84D3-17657B1EA812' , 'Administrator',  '2015-06-04 01:04:40.8974146 +00:00','Administrator','2015-06-04 01:04:40.8974146 +00:00')

INSERT INTO [lookup].[tblGasboyErrorCode] (GasboyErrorCodeIndex, GasboyErrorCode, GasboyErrorCodeName, GasboyErrorCodeGuid, CreatedBy, CreatedDate, UpdatedBy, UpdatedDate)
VALUES (-404, 'ShiftEnd', 'Shift End', 'BD93BE6E-B9F2-489E-A7B2-7BA0006A0354', 'Administrator',  '2015-06-04 01:04:40.8974146 +00:00','Administrator','2015-06-04 01:04:40.8974146 +00:00')

INSERT INTO [lookup].[tblGasboyErrorCode] (GasboyErrorCodeIndex, GasboyErrorCode, GasboyErrorCodeName, GasboyErrorCodeGuid, CreatedBy, CreatedDate, UpdatedBy, UpdatedDate)
VALUES (-412, 'FailedOnFuel', 'Failed to authorize fuel type check', '283FF854-BB1D-4AF4-86E1-A46B7A8EDE14', 'Administrator',  '2015-06-04 01:04:40.8974146 +00:00','Administrator','2015-06-04 01:04:40.8974146 +00:00')

INSERT INTO [lookup].[tblGasboyErrorCode] (GasboyErrorCodeIndex, GasboyErrorCode, GasboyErrorCodeName, GasboyErrorCodeGuid, CreatedBy, CreatedDate, UpdatedBy, UpdatedDate)
VALUES (-413, 'FailedOnLimit', 'Failed to authorize fuel limit for vehicle and ID', '9FDAD7B4-3C48-4B64-85AD-828DDEE9CB61', 'Administrator',  '2015-06-04 01:04:40.8974146 +00:00','Administrator','2015-06-04 01:04:40.8974146 +00:00')

INSERT INTO [lookup].[tblGasboyErrorCode] (GasboyErrorCodeIndex, GasboyErrorCode, GasboyErrorCodeName, GasboyErrorCodeGuid, CreatedBy, CreatedDate, UpdatedBy, UpdatedDate)
VALUES (-306, 'SysinitCalled', 'SysInit Called', 'C9CF911E-4F7C-4B26-8559-EDD4883C1C6F', 'Administrator',  '2015-06-04 01:04:40.8974146 +00:00','Administrator','2015-06-04 01:04:40.8974146 +00:00')

INSERT INTO [lookup].[tblGasboyErrorCode] (GasboyErrorCodeIndex, GasboyErrorCode, GasboyErrorCodeName, GasboyErrorCodeGuid, CreatedBy, CreatedDate, UpdatedBy, UpdatedDate)
VALUES (-307, 'ReloadCalled', 'Reload Called', '054CB477-1870-4A37-9E83-9CF14114C85D', 'Administrator',  '2015-06-04 01:04:40.8974146 +00:00','Administrator','2015-06-04 01:04:40.8974146 +00:00')

INSERT INTO [lookup].[tblGasboyErrorCode] (GasboyErrorCodeIndex, GasboyErrorCode, GasboyErrorCodeName, GasboyErrorCodeGuid, CreatedBy, CreatedDate, UpdatedBy, UpdatedDate)
VALUES (-308, 'InitPump', 'Pump Initialized', '3DA2F44B-B518-4A59-974D-262400A06113', 'Administrator',  '2015-06-04 01:04:40.8974146 +00:00','Administrator','2015-06-04 01:04:40.8974146 +00:00')

INSERT INTO [lookup].[tblGasboyErrorCode] (GasboyErrorCodeIndex, GasboyErrorCode, GasboyErrorCodeName, GasboyErrorCodeGuid, CreatedBy, CreatedDate, UpdatedBy, UpdatedDate)
VALUES (-405, 'InvalidProcessCode', 'Process Code is out of sync. Transaction may not have been written', '5115164C-3367-4A62-A99D-E632A3F1A312', 'Administrator',  '2015-06-04 01:04:40.8974146 +00:00','Administrator','2015-06-04 01:04:40.8974146 +00:00')

INSERT INTO [lookup].[tblGasboyErrorCode] (GasboyErrorCodeIndex, GasboyErrorCode, GasboyErrorCodeName, GasboyErrorCodeGuid, CreatedBy, CreatedDate, UpdatedBy, UpdatedDate)
VALUES (-414, 'MaxRefuelingReached', 'Mean has reached its fueling limit', '5750EAD4-741F-4F7C-8EA0-7C3B94D02FEA', 'Administrator',  '2015-06-04 01:04:40.8974146 +00:00','Administrator','2015-06-04 01:04:40.8974146 +00:00')

INSERT INTO [lookup].[tblGasboyErrorCode] (GasboyErrorCodeIndex, GasboyErrorCode, GasboyErrorCodeName, GasboyErrorCodeGuid, CreatedBy, CreatedDate, UpdatedBy, UpdatedDate)
VALUES (-407, 'OrsanCommError', 'Failed to Authorize Orsan Resson Communication', 'E424AF9C-DA82-44DE-9101-1BBE02C63C34', 'Administrator',  '2015-06-04 01:04:40.8974146 +00:00','Administrator','2015-06-04 01:04:40.8974146 +00:00')

INSERT INTO [lookup].[tblGasboyErrorCode] (GasboyErrorCodeIndex, GasboyErrorCode, GasboyErrorCodeName, GasboyErrorCodeGuid, CreatedBy, CreatedDate, UpdatedBy, UpdatedDate)
VALUES (-415, 'PriceUpdateFail', 'Price update failed', '3F4FFC8B-D272-4F66-9954-8476583BC5BF', 'Administrator',  '2015-06-04 01:04:40.8974146 +00:00','Administrator','2015-06-04 01:04:40.8974146 +00:00')

INSERT INTO [lookup].[tblGasboyErrorCode] (GasboyErrorCodeIndex, GasboyErrorCode, GasboyErrorCodeName, GasboyErrorCodeGuid, CreatedBy, CreatedDate, UpdatedBy, UpdatedDate)
VALUES (-408, 'FuelingCardAuthFail', 'Fueling Card Authorization failed', 'A3DFB37A-12C5-42E5-8345-84A12277CEA3', 'Administrator',  '2015-06-04 01:04:40.8974146 +00:00','Administrator','2015-06-04 01:04:40.8974146 +00:00')

INSERT INTO [lookup].[tblGasboyErrorCode] (GasboyErrorCodeIndex, GasboyErrorCode, GasboyErrorCodeName, GasboyErrorCodeGuid, CreatedBy, CreatedDate, UpdatedBy, UpdatedDate)
VALUES (-410, 'PriceUpdateSent', 'Price update sent', 'A2F696D7-381D-4D7A-B90D-A3CFC744BFAA', 'Administrator',  '2015-06-04 01:04:40.8974146 +00:00','Administrator','2015-06-04 01:04:40.8974146 +00:00')

INSERT INTO [lookup].[tblGasboyErrorCode] (GasboyErrorCodeIndex, GasboyErrorCode, GasboyErrorCodeName, GasboyErrorCodeGuid, CreatedBy, CreatedDate, UpdatedBy, UpdatedDate)
VALUES (-416, 'PumpAckNewPrice', 'Pump ack new price', '4501E8D1-8309-418A-BB6C-7EA84CD60820', 'Administrator',  '2015-06-04 01:04:40.8974146 +00:00','Administrator','2015-06-04 01:04:40.8974146 +00:00')

INSERT INTO [lookup].[tblGasboyErrorCode] (GasboyErrorCodeIndex, GasboyErrorCode, GasboyErrorCodeName, GasboyErrorCodeGuid, CreatedBy, CreatedDate, UpdatedBy, UpdatedDate)
VALUES (-417, 'PumpNackNewPrice', 'Pump nack new price', 'E2ABF28D-2711-4AE5-84A5-22BB6C9B4CCA', 'Administrator',  '2015-06-04 01:04:40.8974146 +00:00','Administrator','2015-06-04 01:04:40.8974146 +00:00')

INSERT INTO [lookup].[tblGasboyErrorCode] (GasboyErrorCodeIndex, GasboyErrorCode, GasboyErrorCodeName, GasboyErrorCodeGuid, CreatedBy, CreatedDate, UpdatedBy, UpdatedDate)
VALUES (-418, 'PumpPriceIncorrect', 'Pump Price Incorrect', '75DA5117-61CB-4178-95AF-B09B58E0CC55', 'Administrator',  '2015-06-04 01:04:40.8974146 +00:00','Administrator','2015-06-04 01:04:40.8974146 +00:00')

INSERT INTO [lookup].[tblGasboyErrorCode] (GasboyErrorCodeIndex, GasboyErrorCode, GasboyErrorCodeName, GasboyErrorCodeGuid, CreatedBy, CreatedDate, UpdatedBy, UpdatedDate)
VALUES (-419, 'PumpAuthSendFail', 'Pump authorize command failed', 'B551E208-850E-493B-930C-4E732FD208C2', 'Administrator',  '2015-06-04 01:04:40.8974146 +00:00','Administrator','2015-06-04 01:04:40.8974146 +00:00')

INSERT INTO [lookup].[tblGasboyErrorCode] (GasboyErrorCodeIndex, GasboyErrorCode, GasboyErrorCodeName, GasboyErrorCodeGuid, CreatedBy, CreatedDate, UpdatedBy, UpdatedDate)
VALUES (-420, 'IncorrectNozzle', 'Incorrect nozzle was lifted and transaction canceled', 'B36F38A7-F376-4BE4-87CC-6C8E64D9E373', 'Administrator',  '2015-06-04 01:04:40.8974146 +00:00','Administrator','2015-06-04 01:04:40.8974146 +00:00')

INSERT INTO [lookup].[tblGasboyErrorCode] (GasboyErrorCodeIndex, GasboyErrorCode, GasboyErrorCodeName, GasboyErrorCodeGuid, CreatedBy, CreatedDate, UpdatedBy, UpdatedDate)
VALUES (-421, 'PumpReadyTimeout', 'Pump ready status timeout', 'DB62654F-6D41-4B76-A750-B4FCB7EE734B', 'Administrator',  '2015-06-04 01:04:40.8974146 +00:00','Administrator','2015-06-04 01:04:40.8974146 +00:00')

INSERT INTO [lookup].[tblGasboyErrorCode] (GasboyErrorCodeIndex, GasboyErrorCode, GasboyErrorCodeName, GasboyErrorCodeGuid, CreatedBy, CreatedDate, UpdatedBy, UpdatedDate)
VALUES (-801, 'FHOServiceStarted', 'Fleet head office service started', '6F491ACC-CD3E-4468-B425-802FFCA46621', 'Administrator',  '2015-06-04 01:04:40.8974146 +00:00','Administrator','2015-06-04 01:04:40.8974146 +00:00')

INSERT INTO [lookup].[tblGasboyErrorCode] (GasboyErrorCodeIndex, GasboyErrorCode, GasboyErrorCodeName, GasboyErrorCodeGuid, CreatedBy, CreatedDate, UpdatedBy, UpdatedDate)
VALUES (-802, 'StationAddedFHO', 'Station was added to HeadOffice', '67173863-78C7-46F8-B3F8-DC52661B110F', 'Administrator',  '2015-06-04 01:04:40.8974146 +00:00','Administrator','2015-06-04 01:04:40.8974146 +00:00')

INSERT INTO [lookup].[tblGasboyErrorCode] (GasboyErrorCodeIndex, GasboyErrorCode, GasboyErrorCodeName, GasboyErrorCodeGuid, CreatedBy, CreatedDate, UpdatedBy, UpdatedDate)
VALUES (-803, 'StationDeletedFHO', 'Station was deleted from HeadOffice', '3688A89B-2DD0-4305-B00D-1A9748AF0B49', 'Administrator',  '2015-06-04 01:04:40.8974146 +00:00','Administrator','2015-06-04 01:04:40.8974146 +00:00')

INSERT INTO [lookup].[tblGasboyErrorCode] (GasboyErrorCodeIndex, GasboyErrorCode, GasboyErrorCodeName, GasboyErrorCodeGuid, CreatedBy, CreatedDate, UpdatedBy, UpdatedDate)
VALUES (-804, 'StationPropertyUpdated', 'Station properties updated', 'E22E6761-623A-4FF6-9491-5E0F1EBD2A55', 'Administrator',  '2015-06-04 01:04:40.8974146 +00:00','Administrator','2015-06-04 01:04:40.8974146 +00:00')

INSERT INTO [lookup].[tblGasboyErrorCode] (GasboyErrorCodeIndex, GasboyErrorCode, GasboyErrorCodeName, GasboyErrorCodeGuid, CreatedBy, CreatedDate, UpdatedBy, UpdatedDate)
VALUES (-430, 'FleetCreditExceeded', 'Fleet Credit has been exceeded', 'E1681678-4D1C-4F24-8978-24D688CD1265', 'Administrator',  '2015-06-04 01:04:40.8974146 +00:00','Administrator','2015-06-04 01:04:40.8974146 +00:00')

INSERT INTO [lookup].[tblGasboyErrorCode] (GasboyErrorCodeIndex, GasboyErrorCode, GasboyErrorCodeName, GasboyErrorCodeGuid, CreatedBy, CreatedDate, UpdatedBy, UpdatedDate)
VALUES (-431, 'VehicleBlocked', 'Vehicle blocked or not found', '50A03958-DE66-4B34-8DE0-78F66D0C5743', 'Administrator',  '2015-06-04 01:04:40.8974146 +00:00','Administrator','2015-06-04 01:04:40.8974146 +00:00')

INSERT INTO [lookup].[tblGasboyErrorCode] (GasboyErrorCodeIndex, GasboyErrorCode, GasboyErrorCodeName, GasboyErrorCodeGuid, CreatedBy, CreatedDate, UpdatedBy, UpdatedDate)
VALUES (-432, 'Range', 'Not allowed to fuel in this time range', '7907956D-A412-4A11-BAC5-6996BBAA558D', 'Administrator',  '2015-06-04 01:04:40.8974146 +00:00','Administrator','2015-06-04 01:04:40.8974146 +00:00')

INSERT INTO [lookup].[tblGasboyErrorCode] (GasboyErrorCodeIndex, GasboyErrorCode, GasboyErrorCodeName, GasboyErrorCodeGuid, CreatedBy, CreatedDate, UpdatedBy, UpdatedDate)
VALUES (-436, 'FleetBlocked', 'Fleet blocked or not found', 'B81873F2-DB78-47ED-B7C2-26093D0C0E2E', 'Administrator',  '2015-06-04 01:04:40.8974146 +00:00','Administrator','2015-06-04 01:04:40.8974146 +00:00')

INSERT INTO [lookup].[tblGasboyErrorCode] (GasboyErrorCodeIndex, GasboyErrorCode, GasboyErrorCodeName, GasboyErrorCodeGuid, CreatedBy, CreatedDate, UpdatedBy, UpdatedDate)
VALUES (-437, 'Visit', 'Number of vists has been exceeded', '7DA8377B-23E9-4FBD-9DC5-7801D98C5F70', 'Administrator',  '2015-06-04 01:04:40.8974146 +00:00','Administrator','2015-06-04 01:04:40.8974146 +00:00')

INSERT INTO [lookup].[tblGasboyErrorCode] (GasboyErrorCodeIndex, GasboyErrorCode, GasboyErrorCodeName, GasboyErrorCodeGuid, CreatedBy, CreatedDate, UpdatedBy, UpdatedDate)
VALUES (-438, 'Cluster', 'Cluster not allowed to fuel at this station', '4FD56ABF-3D11-4460-B43D-1A9458AEB5D5', 'Administrator',  '2015-06-04 01:04:40.8974146 +00:00','Administrator','2015-06-04 01:04:40.8974146 +00:00')

INSERT INTO [lookup].[tblGasboyErrorCode] (GasboyErrorCodeIndex, GasboyErrorCode, GasboyErrorCodeName, GasboyErrorCodeGuid, CreatedBy, CreatedDate, UpdatedBy, UpdatedDate)
VALUES (-439, 'RecCredit', 'Device credit has been exceeded', 'A48A6972-8C59-4810-8B0C-356AC0DAB356', 'Administrator',  '2015-06-04 01:04:40.8974146 +00:00','Administrator','2015-06-04 01:04:40.8974146 +00:00')

INSERT INTO [lookup].[tblGasboyErrorCode] (GasboyErrorCodeIndex, GasboyErrorCode, GasboyErrorCodeName, GasboyErrorCodeGuid, CreatedBy, CreatedDate, UpdatedBy, UpdatedDate)
VALUES (-440, 'FHOOffline', 'Fleet HeadOffice is offline', 'F61F1ADC-DD59-4A79-96CC-B1B1DFA62974', 'Administrator',  '2015-06-04 01:04:40.8974146 +00:00','Administrator','2015-06-04 01:04:40.8974146 +00:00')

INSERT INTO [lookup].[tblGasboyErrorCode] (GasboyErrorCodeIndex, GasboyErrorCode, GasboyErrorCodeName, GasboyErrorCodeGuid, CreatedBy, CreatedDate, UpdatedBy, UpdatedDate)
VALUES (-500, 'Trace', 'Internal Trace', 'B3CB8ADE-024C-4A12-8E03-9F8A93B693B6', 'Administrator',  '2015-06-04 01:04:40.8974146 +00:00','Administrator','2015-06-04 01:04:40.8974146 +00:00')

INSERT INTO [lookup].[tblGasboyErrorCode] (GasboyErrorCodeIndex, GasboyErrorCode, GasboyErrorCodeName, GasboyErrorCodeGuid, CreatedBy, CreatedDate, UpdatedBy, UpdatedDate)
VALUES (-442, 'TooManyDigitsFromPump', 'Too many digits from pump', '7C29073E-6961-463B-853D-D556D88A8B2A', 'Administrator',  '2015-06-04 01:04:40.8974146 +00:00','Administrator','2015-06-04 01:04:40.8974146 +00:00')

INSERT INTO [lookup].[tblGasboyErrorCode] (GasboyErrorCodeIndex, GasboyErrorCode, GasboyErrorCodeName, GasboyErrorCodeGuid, CreatedBy, CreatedDate, UpdatedBy, UpdatedDate)
VALUES (-423, 'NozzleNotLifted', 'Pump nozzle was not lifted, fueling canceled', '55A687DA-A455-404C-9C83-ADE2DFB89044', 'Administrator',  '2015-06-04 01:04:40.8974146 +00:00','Administrator','2015-06-04 01:04:40.8974146 +00:00')

INSERT INTO [lookup].[tblGasboyErrorCode] (GasboyErrorCodeIndex, GasboyErrorCode, GasboyErrorCodeName, GasboyErrorCodeGuid, CreatedBy, CreatedDate, UpdatedBy, UpdatedDate)
VALUES (-443, 'MultipleNozzlesLifted', 'Multiple Nozzles Lifted', '9E7A8AEB-E9FB-428B-B316-A35F52758A8A', 'Administrator',  '2015-06-04 01:04:40.8974146 +00:00','Administrator','2015-06-04 01:04:40.8974146 +00:00')

INSERT INTO [lookup].[tblGasboyErrorCode] (GasboyErrorCodeIndex, GasboyErrorCode, GasboyErrorCodeName, GasboyErrorCodeGuid, CreatedBy, CreatedDate, UpdatedBy, UpdatedDate)
VALUES (-444, 'NoNozzleLifted', 'No Nozzle Lifted', 'F7142510-C005-43D7-B307-ABB33EA2107F', 'Administrator',  '2015-06-04 01:04:40.8974146 +00:00','Administrator','2015-06-04 01:04:40.8974146 +00:00')

INSERT INTO [lookup].[tblGasboyErrorCode] (GasboyErrorCodeIndex, GasboyErrorCode, GasboyErrorCodeName, GasboyErrorCodeGuid, CreatedBy, CreatedDate, UpdatedBy, UpdatedDate)
VALUES (-445, 'PumpNotOpenShift', 'Pump not in Open Shift', 'DE9A7F0F-BD98-4969-8147-9CC467A7CF84', 'Administrator',  '2015-06-04 01:04:40.8974146 +00:00','Administrator','2015-06-04 01:04:40.8974146 +00:00')

INSERT INTO [lookup].[tblGasboyErrorCode] (GasboyErrorCodeIndex, GasboyErrorCode, GasboyErrorCodeName, GasboyErrorCodeGuid, CreatedBy, CreatedDate, UpdatedBy, UpdatedDate)
VALUES (-446, 'PumpNotAvailable', 'Pump not available for fueling', 'AED71189-F97E-42FE-B5BD-0C28C4B2C751', 'Administrator',  '2015-06-04 01:04:40.8974146 +00:00','Administrator','2015-06-04 01:04:40.8974146 +00:00')

INSERT INTO [lookup].[tblGasboyErrorCode] (GasboyErrorCodeIndex, GasboyErrorCode, GasboyErrorCodeName, GasboyErrorCodeGuid, CreatedBy, CreatedDate, UpdatedBy, UpdatedDate)
VALUES (-447, 'AttendantTagNotOpenShift', 'Attendant Tag not in shift', 'AB129C41-3E16-4D32-AF14-55C6E44121D2', 'Administrator',  '2015-06-04 01:04:40.8974146 +00:00','Administrator','2015-06-04 01:04:40.8974146 +00:00')

INSERT INTO [lookup].[tblGasboyErrorCode] (GasboyErrorCodeIndex, GasboyErrorCode, GasboyErrorCodeName, GasboyErrorCodeGuid, CreatedBy, CreatedDate, UpdatedBy, UpdatedDate)
VALUES (-448, 'PumpNotInOpenShift', 'Pump not in Open Shift', '998F7063-7A4D-4BBE-9E4E-D401955DBEE5', 'Administrator',  '2015-06-04 01:04:40.8974146 +00:00','Administrator','2015-06-04 01:04:40.8974146 +00:00')

INSERT INTO [lookup].[tblGasboyErrorCode] (GasboyErrorCodeIndex, GasboyErrorCode, GasboyErrorCodeName, GasboyErrorCodeGuid, CreatedBy, CreatedDate, UpdatedBy, UpdatedDate)
VALUES (-449, 'ProductNotAuthorized', 'Product not authorized', '1C2956BC-5B98-460B-BBEA-ADB351C70780', 'Administrator',  '2015-06-04 01:04:40.8974146 +00:00','Administrator','2015-06-04 01:04:40.8974146 +00:00')

INSERT INTO [lookup].[tblGasboyErrorCode] (GasboyErrorCodeIndex, GasboyErrorCode, GasboyErrorCodeName, GasboyErrorCodeGuid, CreatedBy, CreatedDate, UpdatedBy, UpdatedDate)
VALUES (-450, 'FuelingNotAuthorized', 'Fueling not authorized', '11DCD55F-1A19-4604-A1E3-B775920E65AF', 'Administrator',  '2015-06-04 01:04:40.8974146 +00:00','Administrator','2015-06-04 01:04:40.8974146 +00:00')

INSERT INTO [lookup].[tblGasboyErrorCode] (GasboyErrorCodeIndex, GasboyErrorCode, GasboyErrorCodeName, GasboyErrorCodeGuid, CreatedBy, CreatedDate, UpdatedBy, UpdatedDate)
VALUES (-451, 'NoPumpAssigned', 'No Pump Assigned', 'F6AE95C2-7CBA-4EF1-95DB-E316BF0D5A9D', 'Administrator',  '2015-06-04 01:04:40.8974146 +00:00','Administrator','2015-06-04 01:04:40.8974146 +00:00')

INSERT INTO [lookup].[tblGasboyErrorCode] (GasboyErrorCodeIndex, GasboyErrorCode, GasboyErrorCodeName, GasboyErrorCodeGuid, CreatedBy, CreatedDate, UpdatedBy, UpdatedDate)
VALUES (-452, 'PumpNotOnTagReader', 'Assigned pump not on tag reader', '8F184978-A294-4AA4-A85B-57DD88CB885B', 'Administrator',  '2015-06-04 01:04:40.8974146 +00:00','Administrator','2015-06-04 01:04:40.8974146 +00:00')

INSERT INTO [lookup].[tblGasboyErrorCode] (GasboyErrorCodeIndex, GasboyErrorCode, GasboyErrorCodeName, GasboyErrorCodeGuid, CreatedBy, CreatedDate, UpdatedBy, UpdatedDate)
VALUES (-453, 'NoMultipleFueling', 'Multiple Fueling not allowed', 'C30A4DF5-7EA9-4F59-89D8-634E23380591', 'Administrator',  '2015-06-04 01:04:40.8974146 +00:00','Administrator','2015-06-04 01:04:40.8974146 +00:00')

INSERT INTO [lookup].[tblGasboyErrorCode] (GasboyErrorCodeIndex, GasboyErrorCode, GasboyErrorCodeName, GasboyErrorCodeGuid, CreatedBy, CreatedDate, UpdatedBy, UpdatedDate)
VALUES (-454, 'UnrecognizedTag', 'Unrecognized Tag', 'A9A061F3-DE8C-4E5B-9916-D1BF1F399A02', 'Administrator',  '2015-06-04 01:04:40.8974146 +00:00','Administrator','2015-06-04 01:04:40.8974146 +00:00')

INSERT INTO [lookup].[tblGasboyErrorCode] (GasboyErrorCodeIndex, GasboyErrorCode, GasboyErrorCodeName, GasboyErrorCodeGuid, CreatedBy, CreatedDate, UpdatedBy, UpdatedDate)
VALUES (-455, 'BadTagFormat', 'Bad tag format', '8797BFA8-CE30-4223-92F9-2785D1269D17', 'Administrator',  '2015-06-04 01:04:40.8974146 +00:00','Administrator','2015-06-04 01:04:40.8974146 +00:00')

INSERT INTO [lookup].[tblGasboyErrorCode] (GasboyErrorCodeIndex, GasboyErrorCode, GasboyErrorCodeName, GasboyErrorCodeGuid, CreatedBy, CreatedDate, UpdatedBy, UpdatedDate)
VALUES (-441, 'PresetFuelingIncomplete', 'Preset Fueling incomplete', '0BA0BD20-CA90-401B-9BC6-1403E933E5AD', 'Administrator',  '2015-06-04 01:04:40.8974146 +00:00','Administrator','2015-06-04 01:04:40.8974146 +00:00')

INSERT INTO [lookup].[tblGasboyErrorCode] (GasboyErrorCodeIndex, GasboyErrorCode, GasboyErrorCodeName, GasboyErrorCodeGuid, CreatedBy, CreatedDate, UpdatedBy, UpdatedDate)
VALUES (-457, 'BlacklistedMean', 'Blacklisted Mean','94D2D57F-B9E5-463E-A1B4-82A6E4E1566A' , 'Administrator',  '2015-06-04 01:04:40.8974146 +00:00','Administrator','2015-06-04 01:04:40.8974146 +00:00')

INSERT INTO [lookup].[tblGasboyErrorCode] (GasboyErrorCodeIndex, GasboyErrorCode, GasboyErrorCodeName, GasboyErrorCodeGuid, CreatedBy, CreatedDate, UpdatedBy, UpdatedDate)
VALUES (-458, 'BlockedMean', 'Blocked mean', '607ED65D-0A1F-4BA8-BD13-BBEC4B1B5E50', 'Administrator',  '2015-06-04 01:04:40.8974146 +00:00','Administrator','2015-06-04 01:04:40.8974146 +00:00')

INSERT INTO [lookup].[tblGasboyErrorCode] (GasboyErrorCodeIndex, GasboyErrorCode, GasboyErrorCodeName, GasboyErrorCodeGuid, CreatedBy, CreatedDate, UpdatedBy, UpdatedDate)
VALUES (-459, 'BlockedFleet', 'Blocked Fleet', '8B8C3290-EDA5-4B3C-A2CB-99867E4409E0', 'Administrator',  '2015-06-04 01:04:40.8974146 +00:00','Administrator','2015-06-04 01:04:40.8974146 +00:00')

INSERT INTO [lookup].[tblGasboyErrorCode] (GasboyErrorCodeIndex, GasboyErrorCode, GasboyErrorCodeName, GasboyErrorCodeGuid, CreatedBy, CreatedDate, UpdatedBy, UpdatedDate)
VALUES (-465, 'FlowRate', 'Transaction rejected by flow rate', '7A604FB1-AAFC-464E-949F-FF14986641BB', 'Administrator',  '2015-06-04 01:04:40.8974146 +00:00','Administrator','2015-06-04 01:04:40.8974146 +00:00')

INSERT INTO [lookup].[tblGasboyErrorCode] (GasboyErrorCodeIndex, GasboyErrorCode, GasboyErrorCodeName, GasboyErrorCodeGuid, CreatedBy, CreatedDate, UpdatedBy, UpdatedDate)
VALUES (-466, 'WrongPIN', 'Wrong PIN was entered', 'EB0A926F-2B89-44A4-A70B-120EEE5ED9B6', 'Administrator',  '2015-06-04 01:04:40.8974146 +00:00','Administrator','2015-06-04 01:04:40.8974146 +00:00')

INSERT INTO [lookup].[tblGasboyErrorCode] (GasboyErrorCodeIndex, GasboyErrorCode, GasboyErrorCodeName, GasboyErrorCodeGuid, CreatedBy, CreatedDate, UpdatedBy, UpdatedDate)
VALUES (-467, 'TagBlockedByPIN', 'Tag blocked due to wrong PIN', '4A55E8FE-7B60-489E-9ED9-CB7B89566C74', 'Administrator',  '2015-06-04 01:04:40.8974146 +00:00','Administrator','2015-06-04 01:04:40.8974146 +00:00')

INSERT INTO [lookup].[tblGasboyErrorCode] (GasboyErrorCodeIndex, GasboyErrorCode, GasboyErrorCodeName, GasboyErrorCodeGuid, CreatedBy, CreatedDate, UpdatedBy, UpdatedDate)
VALUES (-468, 'PumpBusy', 'Pump is busy, cannot authorize', 'F4C61B1C-51F3-4B25-BD6D-13B1D429FE3F', 'Administrator',  '2015-06-04 01:04:40.8974146 +00:00','Administrator','2015-06-04 01:04:40.8974146 +00:00')

INSERT INTO [lookup].[tblGasboyErrorCode] (GasboyErrorCodeIndex, GasboyErrorCode, GasboyErrorCodeName, GasboyErrorCodeGuid, CreatedBy, CreatedDate, UpdatedBy, UpdatedDate)
VALUES (-959, 'BusUpdated', 'Bus updated during setup', '3AFD4726-75F7-4791-8C16-8F50EF9298A3', 'Administrator',  '2015-06-04 01:04:40.8974146 +00:00','Administrator','2015-06-04 01:04:40.8974146 +00:00')

INSERT INTO [lookup].[tblGasboyErrorCode] (GasboyErrorCodeIndex, GasboyErrorCode, GasboyErrorCodeName, GasboyErrorCodeGuid, CreatedBy, CreatedDate, UpdatedBy, UpdatedDate)
VALUES (-956, 'DeviceUpdated', 'Device updated during setup', '6D5093BB-B5F1-4555-9D66-73BF57AEBA2C', 'Administrator',  '2015-06-04 01:04:40.8974146 +00:00','Administrator','2015-06-04 01:04:40.8974146 +00:00')

INSERT INTO [lookup].[tblGasboyErrorCode] (GasboyErrorCodeIndex, GasboyErrorCode, GasboyErrorCodeName, GasboyErrorCodeGuid, CreatedBy, CreatedDate, UpdatedBy, UpdatedDate)
VALUES (-476 , 'BypassOff', 'Pump Bypass Off', '41F15C06-4C85-47BE-9089-CC322F55A176', 'Administrator',  '2015-06-04 01:04:40.8974146 +00:00','Administrator','2015-06-04 01:04:40.8974146 +00:00')

INSERT INTO [lookup].[tblGasboyErrorCode] (GasboyErrorCodeIndex, GasboyErrorCode, GasboyErrorCodeName, GasboyErrorCodeGuid, CreatedBy, CreatedDate, UpdatedBy, UpdatedDate)
VALUES (-475, 'BypassOn', 'Pump Bypass On', '61AC4EC2-23E3-4132-8EB8-5E7AF4AFCC6D', 'Administrator',  '2015-06-04 01:04:40.8974146 +00:00','Administrator','2015-06-04 01:04:40.8974146 +00:00')

END