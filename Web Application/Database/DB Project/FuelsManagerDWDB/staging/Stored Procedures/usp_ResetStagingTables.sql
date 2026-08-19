/*
	DROP PROCEDURE [staging].[usp_ResetStagingTables]

	EXEC [staging].[usp_ResetStagingTables]
	
*/
CREATE PROCEDURE [staging].[usp_ResetStagingTables]
AS
BEGIN
  ------------------------------------------------------------------------------------------------------
  -- Stored procedure: [staging].[usp_ResetStagingTables]
  -- Author: Hansraj Bapoo
  -- Version/Date: 1.0.003 / 2012-07-13 14:21:10.4470770 -04:00
  -- Purpose: Deletes all the records from the staging tables.
  -- Notes:
  -- 1. 
  ------------------------------------------------------------------------------------------------------
  SET NOCOUNT ON;
  BEGIN TRY

    TRUNCATE TABLE [staging].[tblApplicationString]
    TRUNCATE TABLE [staging].[tblAutoDistributionReasonCodes]
    TRUNCATE TABLE [staging].[tblCompanies]
    TRUNCATE TABLE [staging].[tblCompanyComparisonTemp]
    TRUNCATE TABLE [staging].[tblCompanyMasterRecordToDateRange]
    TRUNCATE TABLE [staging].[tblCompanyToRole]
    TRUNCATE TABLE [staging].[tblCompanyToSiteRecordVersion]
    TRUNCATE TABLE [staging].[tblCompanyToSiteRecordVersionTemp]
    TRUNCATE TABLE [staging].[tblCompanyToUserGroup]
    TRUNCATE TABLE [staging].[tblConjoinTransactionLineItems]
    TRUNCATE TABLE [staging].[tblConjoinTransactions]
    TRUNCATE TABLE [staging].[tblConjoinTransactionSubLineItems]
    TRUNCATE TABLE [staging].[tblEditedFactTransaction]
    TRUNCATE TABLE [staging].[tblEditedFactTransactionSummary]
    TRUNCATE TABLE [staging].[tblEntityChecksum]
    TRUNCATE TABLE [staging].[tblEntityCompanyToSite]
    TRUNCATE TABLE [staging].[tblEntityDateRange]
    TRUNCATE TABLE [staging].[tblEntityEquipmentToSite]
    TRUNCATE TABLE [staging].[tblEntityPersonnelToSite]
    TRUNCATE TABLE [staging].[tblEntityProductToSite]
    TRUNCATE TABLE [staging].[tblEntityTransactionAliasToSite]
    TRUNCATE TABLE [staging].[tblEntityUserToSite]
    TRUNCATE TABLE [staging].[tblEquipment]
    TRUNCATE TABLE [staging].[tblEquipmentComparisonTemp]
    TRUNCATE TABLE [staging].[tblEquipmentMasterRecordToDateRange]
    TRUNCATE TABLE [staging].[tblEquipmentToSiteRecordVersion]
    TRUNCATE TABLE [staging].[tblEquipmentToSiteRecordVersionTemp]
    TRUNCATE TABLE [staging].[tblEquipmentTypes]
    TRUNCATE TABLE [staging].[tblInsertedLineItems]
    TRUNCATE TABLE [staging].[tblInsertedRecordsTemp]
    TRUNCATE TABLE [staging].[tblLoadArms]
    TRUNCATE TABLE [staging].[tblMissingEntitiesTempOne]
    TRUNCATE TABLE [staging].[tblMissingEntitiesTempTwo]
    TRUNCATE TABLE [staging].[tblOwnerCloseout]
    TRUNCATE TABLE [staging].[tblPersonnel]
    TRUNCATE TABLE [staging].[tblPersonnelComparisonTemp]
    TRUNCATE TABLE [staging].[tblPersonnelMasterRecordToDateRange]
    TRUNCATE TABLE [staging].[tblPersonnelToSiteRecordVersion]
    TRUNCATE TABLE [staging].[tblPersonnelToSiteRecordVersionTemp]
    TRUNCATE TABLE [staging].[tblProcessedInventoryYears]
    TRUNCATE TABLE [staging].[tblProductComparisonTemp]
    TRUNCATE TABLE [staging].[tblProductMasterRecordToDateRange]
    TRUNCATE TABLE [staging].[tblProducts]
    TRUNCATE TABLE [staging].[tblProductToSiteRecordVersion]
    TRUNCATE TABLE [staging].[tblProductToSiteRecordVersionTemp]
    TRUNCATE TABLE [staging].[tblSiteHierarchyBridge]
    TRUNCATE TABLE [staging].[tblSites]
    TRUNCATE TABLE [staging].[tblSiteToSite]
    TRUNCATE TABLE [staging].[tblStations]
    TRUNCATE TABLE [staging].[tblTanks]
    TRUNCATE TABLE [staging].[tblTransactionAliasComparisonTemp]
    TRUNCATE TABLE [staging].[tblTransactionAliases]
    TRUNCATE TABLE [staging].[tblTransactionAliasMasterRecordToDateRange]
    TRUNCATE TABLE [staging].[tblTransactionAliasToSiteRecordVersion]
    TRUNCATE TABLE [staging].[tblTransactionAliasToSiteRecordVersionTemp]
    TRUNCATE TABLE [staging].[tblTransactionAttributes]
    TRUNCATE TABLE [staging].[tblTransactionLineItems]
    TRUNCATE TABLE [staging].[tblTransactionLineItemUserData]
    TRUNCATE TABLE [staging].[tblTransactionNotes]
    TRUNCATE TABLE [staging].[tblTransactions]
    TRUNCATE TABLE [staging].[tblTransactionSubLineItems]
    TRUNCATE TABLE [staging].[tblTransactionSummary]
    TRUNCATE TABLE [staging].[tblTransactionSummaryAttributes]
    TRUNCATE TABLE [staging].[tblTransactionUserData]
    TRUNCATE TABLE [staging].[tblUpdatedRecordsTemp]
    TRUNCATE TABLE [staging].[tblUsers]
    TRUNCATE TABLE [staging].[tblUserToUserGroup]
    TRUNCATE TABLE [staging].[tblPartialTransactionSegment]


    UPDATE [staging].[tblETLTempVariables]
	SET VariableValue = NULL

    --TRUNCATE TABLE [staging].[tblApplicationString]
    --TRUNCATE TABLE [staging].[tblFuelCards]
    --TRUNCATE TABLE [staging].[tblMissingTransactions]
	--TRUNCATE TABLE [staging].[tblFuelCardComparisonTemp]

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
    + 'Procedure Name: [staging].[usp_ResetStagingTables]' + CHAR(13) + CHAR(10)
    + 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS varchar(20)), '') + CHAR(13) + CHAR(10);
    RAISERROR (@_ErrMessage, 16, 1);
  END CATCH

END