/*
	DROP PROCEDURE [dbo].[usp_ResetStaticTables]
 
	EXEC [dbo].[usp_ResetStaticTables]
 
*/
CREATE PROCEDURE [dbo].[usp_ResetStaticTables]
AS
BEGIN
------------------------------------------------------------------------------------------------------
-- Stored procedure: [dbo].[usp_ResetStaticTables]
-- Author: Hansraj Bapoo
-- Version/Date: 1.0.003 / 2012-07-13 14:21:10.4470770 -04:00
-- Purpose: Truncate all the static tables.
-- Notes:
-- 1. The static tables are those tables in the OLTP database that are hardly ever updated and that are referenced by the dymanic tables, 
--    such as the Entity tables and the Transaction tables.
------------------------------------------------------------------------------------------------------
	SET NOCOUNT ON;
	BEGIN TRY
		TRUNCATE TABLE [dbo].[tblAlarmAndEvents]
		TRUNCATE TABLE [lookup].[tblActivationStatus]
		TRUNCATE TABLE [lookup].[tblAggregateField]
		TRUNCATE TABLE [lookup].[tblAirplaneTankLocation]
		TRUNCATE TABLE [lookup].[tblAirplaneTankToleranceType]
		TRUNCATE TABLE [lookup].[tblAllocationType]
		TRUNCATE TABLE [lookup].[tblApplicationStringType]
		TRUNCATE TABLE [lookup].[tblAssetTrackingDeviceType]
		TRUNCATE TABLE [lookup].[tblAssetTrackingMessageState]
		TRUNCATE TABLE [lookup].[tblAssetTrackingPayloadType]
		TRUNCATE TABLE [lookup].[tblChangeQueueRecordType]
		TRUNCATE TABLE [lookup].[tblCompanyCrossReferenceType]
		TRUNCATE TABLE [lookup].[tblCompanyMapType]
		TRUNCATE TABLE [lookup].[tblCompanyRole]
		TRUNCATE TABLE [lookup].[tblConsortiumType]
		TRUNCATE TABLE [lookup].[tblCurrencyUnit]
		TRUNCATE TABLE [lookup].[tblCustomToolbarCommandType]
		TRUNCATE TABLE [lookup].[tblCustomToolbarType]
		TRUNCATE TABLE [lookup].[tblDayOfWeek]
		TRUNCATE TABLE [lookup].[tblDeviceTankType]
		TRUNCATE TABLE [lookup].[tblDispatchGridColumnType]
		TRUNCATE TABLE [lookup].[tblDispatchGridType]
		TRUNCATE TABLE [lookup].[tblEngineeringUnit]
		TRUNCATE TABLE [lookup].[tblEquipmentType]
		TRUNCATE TABLE [lookup].[tblExportResultType]
		TRUNCATE TABLE [lookup].[tblFillMethod]
		TRUNCATE TABLE [lookup].[tblFilterField]
		TRUNCATE TABLE [lookup].[tblFuelCardLimitPeriod]
		TRUNCATE TABLE [lookup].[tblListViewFieldType]
		TRUNCATE TABLE [lookup].[tblListViewStandardType]
		TRUNCATE TABLE [lookup].[tblListViewType]
		TRUNCATE TABLE [lookup].[tblMailServerConnectMode]
		TRUNCATE TABLE [lookup].[tblMajorCorrectionType]
		TRUNCATE TABLE [lookup].[tblMapSource]
		TRUNCATE TABLE [lookup].[tblMenuItemType]
		TRUNCATE TABLE [lookup].[tblMessageFrequencyType]
		TRUNCATE TABLE [lookup].[tblMessageLocationType]
		TRUNCATE TABLE [lookup].[tblMinorCorrectionType]
		TRUNCATE TABLE [lookup].[tblNumberGroupSizesType]
		TRUNCATE TABLE [lookup].[tblPersonnelRole]
		TRUNCATE TABLE [lookup].[tblPointServiceHealthStatus]
		TRUNCATE TABLE [lookup].[tblPointTagInputOutputType]
		TRUNCATE TABLE [lookup].[tblPresetType]
		TRUNCATE TABLE [lookup].[tblProcessVariableType]
		TRUNCATE TABLE [lookup].[tblProductType]
		TRUNCATE TABLE [lookup].[tblQualificationType]
		TRUNCATE TABLE [lookup].[tblQuantityDisplay]
		TRUNCATE TABLE [lookup].[tblReportApprovalState]
		TRUNCATE TABLE [lookup].[tblResetMethod]
		TRUNCATE TABLE [lookup].[tblResetPeriod]
		TRUNCATE TABLE [lookup].[tblRight]
		TRUNCATE TABLE [lookup].[tblScheduleType]
		TRUNCATE TABLE [lookup].[tblServiceType]
		TRUNCATE TABLE [lookup].[tblSRMAdaptorFilterType]
		TRUNCATE TABLE [lookup].[tblStandardFieldType]
		TRUNCATE TABLE [lookup].[tblStationInterfaceType]
		TRUNCATE TABLE [lookup].[tblStationType]
		TRUNCATE TABLE [lookup].[tblSyncConflictResolutionStatus]
		TRUNCATE TABLE [lookup].[tblSyncConflictType]
		TRUNCATE TABLE [lookup].[tblSyncControllerStep]
		TRUNCATE TABLE [lookup].[tblSyncRequestType]
		TRUNCATE TABLE [lookup].[tblSyncSessionState]
		TRUNCATE TABLE [lookup].[tblSyncSessionStatus]
		TRUNCATE TABLE [lookup].[tblSyncTransferType]
		TRUNCATE TABLE [lookup].[tblTestSetStatus]
		TRUNCATE TABLE [lookup].[tblTimeZone]
		TRUNCATE TABLE [lookup].[tblTransactionFieldType]
		TRUNCATE TABLE [lookup].[tblTransactionOrigin]
		TRUNCATE TABLE [lookup].[tblTransactionQuality]
		TRUNCATE TABLE [lookup].[tblTransactionStatus]
		TRUNCATE TABLE [lookup].[tblTransactionTypes]
		TRUNCATE TABLE [lookup].[tblUserDataType]
		TRUNCATE TABLE [lookup].[tblVariantType]
		TRUNCATE TABLE [lookup].[tblVesselType]
		TRUNCATE TABLE [lookup].[tblWatchdogMode]
	END TRY
	BEGIN CATCH
		DECLARE @_ErrMessage nvarchar(2048),
		@_ErrNumber int,
		@_ErrProcName nvarchar(126),
		@_ErrLineNumber int;
		SET @_ErrMessage = ERROR_MESSAGE();
		SET @_ErrNumber = ERROR_NUMBER();
		SET @_ErrProcName = ERROR_PROCEDURE();
		SET @_ErrLineNumber = ERROR_LINE();
		SET @_ErrMessage = 'Error: ' + @_ErrMessage + CHAR(13) + CHAR(10)
		+ 'Number: ' + CAST(@_ErrNumber AS varchar(20)) + CHAR(13) + CHAR(10)
		+ 'Procedure Name: [dbo].[usp_ResetStaticTables]' + CHAR(13) + CHAR(10)
		+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS varchar(20)), '') + CHAR(13) + CHAR(10);
		RAISERROR (@_ErrMessage, 16, 1);
	END CATCH
END
