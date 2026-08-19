/*

	DROP PROCEDURE [Staging].[usp_SetNonHistoricalDimensionUpdatedDate]

	EXEC [staging].[usp_SetNonHistoricalDimensionUpdatedDate]
	
*/
CREATE PROCEDURE [staging].[usp_SetNonHistoricalDimensionUpdatedDate]
AS
BEGIN
  ------------------------------------------------------------------------------------------------------
  -- Stored procedure: [staging].[usp_SetNonHistoricalUpdatedDate]
  -- Author: Hansraj Bapoo
  -- Version/Date: 1.0.003 / 2012-07-13 14:21:10.4470770 -04:00
  -- Purpose: Sets the CombinedUpdatedDate field for each non-historical record captured in staging.
  -- Notes:
  -- 1. This process is limited to tables for which non-historical records are captured on the OLTP database.
  -- 2. The updated date of a record can come from two sources: The UpdatedDate field of the source record, or the RecordUpdatedDate field of the fmcdc record entry for the record.
  --    In the case of historical records, those two date sources are combined into a single value when setting the StartDate.
  --    This Stored Procedure does the same thing for non-historical records, which do not have a StartDate.
  ------------------------------------------------------------------------------------------------------
  SET NOCOUNT ON;
  BEGIN TRY

    -- ApplicationString
    UPDATE staging.tblApplicationString
    SET CombinedUpdatedDate = COALESCE(RecordUpdatedDate, UpdatedDate)

   -- AutoDistributionReasonCodes
    UPDATE staging.tblAutoDistributionReasonCodes
    SET CombinedUpdatedDate = COALESCE(RecordUpdatedDate, UpdatedDate)

    -- Station	
    UPDATE staging.tblStations
    SET CombinedUpdatedDate = COALESCE(RecordUpdatedDate, UpdatedDate)

    -- LoadArm
    UPDATE staging.tblLoadArms
    SET CombinedUpdatedDate = COALESCE(RecordUpdatedDate, UpdatedDate)

    -- Tank	
    UPDATE staging.tblTanks
    SET CombinedUpdatedDate = COALESCE(RecordUpdatedDate, UpdatedDate)

    -- Site	
    UPDATE staging.tblSites
    SET CombinedUpdatedDate = COALESCE(RecordUpdatedDate, UpdatedDate)

    -- User
    UPDATE staging.tblUsers
    SET CombinedUpdatedDate = COALESCE(RecordUpdatedDate, UpdatedDate)

    -- CompanyToRole
    UPDATE staging.tblCompanyToRole
    SET CombinedUpdatedDate = COALESCE(RecordUpdatedDate, UpdatedDate)

    -- CompanyToUserGroup
    UPDATE staging.tblCompanyToUserGroup
    SET CombinedUpdatedDate = COALESCE(RecordUpdatedDate, UpdatedDate)

    -- UserToUserGroup
    UPDATE staging.tblUserToUserGroup
    SET CombinedUpdatedDate = COALESCE(RecordUpdatedDate, UpdatedDate)

    -- UserToSite
    UPDATE staging.tblEntityUserToSite
    SET CombinedUpdatedDate = COALESCE(RecordUpdatedDate, UpdatedDate)

    -- CompanyToSite
    UPDATE staging.tblEntityCompanyToSite
    SET CombinedUpdatedDate = COALESCE(RecordUpdatedDate, UpdatedDate)

    -- EquipmentToSite
    UPDATE staging.tblEntityEquipmentToSite
    SET CombinedUpdatedDate = COALESCE(RecordUpdatedDate, UpdatedDate)

    -- ProductToSite
    UPDATE staging.tblEntityProductToSite
    SET CombinedUpdatedDate = COALESCE(RecordUpdatedDate, UpdatedDate)

    -- PersonnelToSite
    UPDATE staging.tblEntityPersonnelToSite
    SET CombinedUpdatedDate = COALESCE(RecordUpdatedDate, UpdatedDate)

    -- TransactionAliasToSite
    UPDATE staging.tblEntityTransactionAliasToSite
    SET CombinedUpdatedDate = COALESCE(RecordUpdatedDate, UpdatedDate)

    -- EquipmentTypes	
    UPDATE staging.tblEquipmentTypes
    SET CombinedUpdatedDate = COALESCE(RecordUpdatedDate, UpdatedDate)

    --SiteToSite
    UPDATE staging.tblSiteToSite
    SET CombinedUpdatedDate = COALESCE(RecordUpdatedDate, UpdatedDate)


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
    + 'Procedure Name: [staging].[usp_SetNonHistoricalDimensionUpdatedDate]' + CHAR(13) + CHAR(10)
    + 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS varchar(20)), '') + CHAR(13) + CHAR(10);
    RAISERROR (@_ErrMessage, 16, 1);
  END CATCH

END