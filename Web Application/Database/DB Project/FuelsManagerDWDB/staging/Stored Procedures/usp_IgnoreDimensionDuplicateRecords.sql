/*

	DROP PROCEDURE [staging].[usp_IgnoreDimensionDuplicateRecords]

	EXEC [staging].[usp_IgnoreDimensionDuplicateRecords]
	
*/
CREATE PROCEDURE [staging].[usp_IgnoreDimensionDuplicateRecords]
AS
BEGIN
  ------------------------------------------------------------------------------------------------------
  -- Stored procedure: [staging].[usp_IgnoreDimensionDuplicateRecords]
  -- Author: Hansraj Bapoo
  -- Version/Date: 1.0.003 / 2012-07-13 14:21:10.4470770 -04:00
  -- Purpose: Sets the IgnoreRecord flag of each Dimension record in staging that comes directly from the source OLTP record when one or more fmcdc record has also been captured for the same entity record.
  -- Notes:
  -- 1. All record changes in the OLTP database are captured by the custom Change Data Capture system (fmcdc).
  -- 2. Usually the ETL process only extracts data from the fmcdc tables, but in the case of a manual run, e.g. during an intial data loading, the ETL process also retrieves data directly from the source tables.
  --    When this happens, there is a possibility that two records will be captured in staging for a given record, one directly from the source table, and one (or more) from the fmcdc table. If an fmcdc record exist 
  --    for the entity record, then the staging record for the one captured directly from the source table can safely be ignored. 
  ------------------------------------------------------------------------------------------------------
  SET NOCOUNT ON;
  BEGIN TRY

    -- Set the IgnoreRecord flag of all non-trigger entered records for which there is a corresponding trigger-entered record (based on Identity Key matching)

    -- ApplicationString
    UPDATE a
    SET a.IgnoreRecord = 1
    FROM staging.tblApplicationString a
    WHERE a.RecordUpdatedDate IS NULL
    AND EXISTS 
    (
        SELECT * FROM staging.tblApplicationString b
        WHERE b.ApplicationStringKey = a.ApplicationStringKey
        AND b.RecordUpdatedDate IS NOT NULL
    )

    -- AutoDistributionReasonCodes
    UPDATE a
    SET a.IgnoreRecord = 1
    FROM staging.tblAutoDistributionReasonCodes a
    WHERE a.RecordUpdatedDate IS NULL
    AND EXISTS 
    (
        SELECT * FROM staging.tblAutoDistributionReasonCodes b
        WHERE b.AutoDistributionReasonCodeKey = a.AutoDistributionReasonCodeKey
        AND b.RecordUpdatedDate IS NOT NULL
    )

    -- Company
    UPDATE a
    SET a.IgnoreRecord = 1
    FROM staging.tblCompanies a
    WHERE a.RecordUpdatedDate IS NULL
    AND EXISTS 
    (
        SELECT * FROM staging.tblCompanies b
        WHERE b.CompanyKey = a.CompanyKey
        AND b.RecordUpdatedDate IS NOT NULL
    )

    -- Station
    UPDATE a
    SET a.IgnoreRecord = 1
    FROM staging.tblStations a
    WHERE a.RecordUpdatedDate IS NULL
    AND EXISTS 
    (
        SELECT * FROM staging.tblStations b
        WHERE b.StationKey = a.StationKey
        AND b.RecordUpdatedDate IS NOT NULL
    )

    -- LoarArm
    UPDATE a
    SET a.IgnoreRecord = 1
    FROM staging.tblLoadArms a
    WHERE a.RecordUpdatedDate IS NULL
    AND EXISTS 
    (
        SELECT * FROM staging.tblLoadArms b
        WHERE b.LoadArmKey = a.LoadArmKey
        AND b.RecordUpdatedDate IS NOT NULL
    )

    -- Tank
    UPDATE a
    SET a.IgnoreRecord = 1
    FROM staging.tblTanks a
    WHERE a.RecordUpdatedDate IS NULL
    AND EXISTS 
    (
        SELECT * FROM staging.tblTanks b
        WHERE b.TankKey = a.TankKey
        AND b.RecordUpdatedDate IS NOT NULL
    )

    -- Site
    UPDATE a
    SET a.IgnoreRecord = 1
    FROM staging.tblSites a
    WHERE a.RecordUpdatedDate IS NULL
    AND EXISTS 
    (
        SELECT * FROM staging.tblSites b
        WHERE b.SiteKey = a.SiteKey
        AND b.RecordUpdatedDate IS NOT NULL
    )

     -- Personnel
    UPDATE a
    SET a.IgnoreRecord = 1
    FROM staging.tblPersonnel a
    WHERE a.RecordUpdatedDate IS NULL
    AND EXISTS 
    (
        SELECT * FROM staging.tblPersonnel b
        WHERE b.PersonnelKey = a.PersonnelKey
        AND b.RecordUpdatedDate IS NOT NULL
    )

    -- Product
    UPDATE a
    SET a.IgnoreRecord = 1
    FROM staging.tblProducts a
    WHERE a.RecordUpdatedDate IS NULL
    AND EXISTS 
    (
        SELECT * FROM staging.tblProducts b
        WHERE b.ProductKey = a.ProductKey
        AND b.RecordUpdatedDate IS NOT NULL
    )

    -- Equipment
    UPDATE a
    SET a.IgnoreRecord = 1
    FROM staging.tblEquipment a
    WHERE a.RecordUpdatedDate IS NULL
    AND EXISTS 
    (
        SELECT * FROM staging.tblEquipment b
        WHERE b.EquipmentKey = a.EquipmentKey
        AND b.RecordUpdatedDate IS NOT NULL
    )

    -- EquipmentType
    UPDATE a
    SET a.IgnoreRecord = 1
    FROM staging.tblEquipmentTypes a
    WHERE a.RecordUpdatedDate IS NULL
    AND EXISTS 
    (
        SELECT * FROM staging.tblEquipmentTypes b
        WHERE b.EquipmentTypeKey = a.EquipmentTypeKey
        AND b.RecordUpdatedDate IS NOT NULL
    )

     -- TransactionAlias
    UPDATE a
    SET a.IgnoreRecord = 1
    FROM staging.tblTransactionAliases a
    WHERE a.RecordUpdatedDate IS NULL
    AND EXISTS 
    (
        SELECT * FROM staging.tblTransactionAliases b
        WHERE b.TransactionAliasKey = a.TransactionAliasKey
        AND b.RecordUpdatedDate IS NOT NULL
    )

    -- User
    UPDATE a
    SET a.IgnoreRecord = 1
    FROM staging.tblUsers a
    WHERE a.RecordUpdatedDate IS NULL
    AND EXISTS 
    (
        SELECT * FROM staging.tblUsers b
        WHERE b.UserKey = a.UserKey
        AND b.RecordUpdatedDate IS NOT NULL
    )



     -- CompanyToRole
    UPDATE a
    SET a.IgnoreRecord = 1
    FROM staging.tblCompanyToRole a
    WHERE a.RecordUpdatedDate IS NULL
    AND EXISTS 
    (
        SELECT * FROM staging.tblCompanyToRole b
        WHERE b.CompanyToRoleKey = a.CompanyToRoleKey
        AND b.RecordUpdatedDate IS NOT NULL
    )

    -- CompanyToUserGroup
    UPDATE a
    SET a.IgnoreRecord = 1
    FROM staging.tblCompanyToUserGroup a
    WHERE a.RecordUpdatedDate IS NULL
    AND EXISTS 
    (
        SELECT * FROM staging.tblCompanyToUserGroup b
        WHERE b.CompanyToUserGroupKey = a.CompanyToUserGroupKey
        AND b.RecordUpdatedDate IS NOT NULL
    )


    -- UserToUserGroup		
    UPDATE a
    SET a.IgnoreRecord = 1
    FROM staging.tblUserToUserGroup a
    WHERE a.RecordUpdatedDate IS NULL
    AND EXISTS 
    (
        SELECT * FROM staging.tblUserToUserGroup b
        WHERE b.GroupKey = a.GroupKey
        AND b.UserKey = a.UserKey
        AND b.RecordUpdatedDate IS NOT NULL
    )

    -- CompanyToSite
    UPDATE a
    SET a.IgnoreRecord = 1
    FROM staging.tblEntityCompanyToSite a
    WHERE a.RecordUpdatedDate IS NULL
    AND EXISTS 
    (
        SELECT * FROM staging.tblEntityCompanyToSite b
        WHERE b.CompanyToSiteKey = a.CompanyToSiteKey
        AND b.RecordUpdatedDate IS NOT NULL
    )

    -- EquipmentToSite
    UPDATE a
    SET a.IgnoreRecord = 1
    FROM staging.tblEntityEquipmentToSite a
    WHERE a.RecordUpdatedDate IS NULL
    AND EXISTS 
    (
        SELECT * FROM staging.tblEntityEquipmentToSite b
        WHERE b.EquipmentToSiteKey = a.EquipmentToSiteKey
        AND b.RecordUpdatedDate IS NOT NULL
    )

    -- PersonnelToSite
    UPDATE a
    SET a.IgnoreRecord = 1
    FROM staging.tblEntityPersonnelToSite a
    WHERE a.RecordUpdatedDate IS NULL
    AND EXISTS 
    (
        SELECT * FROM staging.tblEntityPersonnelToSite b
        WHERE b.PersonnelToSiteKey = a.PersonnelToSiteKey
        AND b.RecordUpdatedDate IS NOT NULL
    )

     -- ProductToSite
    UPDATE a
    SET a.IgnoreRecord = 1
    FROM staging.tblEntityProductToSite a
    WHERE a.RecordUpdatedDate IS NULL
    AND EXISTS 
    (
        SELECT * FROM staging.tblEntityProductToSite b
        WHERE b.ProductToSiteKey = a.ProductToSiteKey
        AND b.RecordUpdatedDate IS NOT NULL
    )

     -- TransactionAliasToSite
    UPDATE a
    SET a.IgnoreRecord = 1
    FROM staging.tblEntityTransactionAliasToSite a
    WHERE a.RecordUpdatedDate IS NULL
    AND EXISTS 
    (
        SELECT * FROM staging.tblEntityTransactionAliasToSite b
        WHERE b.TransactionAliasToSiteKey = a.TransactionAliasToSiteKey
        AND b.RecordUpdatedDate IS NOT NULL
    )

    --SiteToSite
    UPDATE a
    SET a.IgnoreRecord = 1
    FROM staging.tblSiteToSite a
    WHERE a.RecordUpdatedDate IS NULL
    AND EXISTS 
    (
        SELECT * FROM staging.tblSiteToSite b
        WHERE b.ParentSiteKey = a.ParentSiteKey
        AND b.ChildSiteKey = a.ChildSiteKey
        AND b.RecordUpdatedDate IS NOT NULL
    )


    -- UserToSite
    UPDATE a
    SET a.IgnoreRecord = 1
    FROM staging.tblEntityUserToSite a
    WHERE a.RecordUpdatedDate IS NULL
    AND EXISTS 
    (
        SELECT * FROM staging.tblEntityUserToSite b
        WHERE b.UserKey = a.UserKey
        AND b.SiteKey = a.SiteKey
        AND b.RecordUpdatedDate IS NOT NULL
    )


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
    + 'Procedure Name: [staging].[usp_IgnoreDimensionDuplicateRecords]' + CHAR(13) + CHAR(10)
    + 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS varchar(20)), '') + CHAR(13) + CHAR(10);
    RAISERROR (@_ErrMessage, 16, 1);
  END CATCH

END