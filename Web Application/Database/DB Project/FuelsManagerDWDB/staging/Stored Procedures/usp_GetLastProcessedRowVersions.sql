/*
	DROP PROCEDURE [Staging].[usp_GetLastProcessedRowVersions]

	EXEC [staging].[usp_GetLastProcessedRowVersions]
	
*/
CREATE PROCEDURE [staging].[usp_GetLastProcessedRowVersions]
AS
BEGIN
  ------------------------------------------------------------------------------------------------------
  -- Stored procedure: [staging].[usp_GetLastProcessedRowVersions]
  -- Author: Hansraj Bapoo
  -- Version/Date: 1.0.003 / 2012-07-13 14:21:10.4470770 -04:00
  -- Purpose: Retrieve the Last RowVersion of the record changes processed by Staging, for each of the entities.
  -- Notes:
  -- 1. The RowVersion that is returned is the RowVersion of the record as issued from the OLTP database itself, in the Data Capture Tables,
  --    and as retrieved and copied into the Staging tables in the OLAP database.
  ------------------------------------------------------------------------------------------------------
  SET NOCOUNT ON;
  BEGIN TRY

    DECLARE @tblEntityLastRowVersion TABLE (
      EntityName nvarchar(50),
      StagingTableName nvarchar(50),
      LastRowVersion bigint
    )

    -- ApplicationString
    INSERT INTO @tblEntityLastRowVersion (EntityName, StagingTableName, LastRowVersion)
      SELECT
        'ApplicationString',
        'staging.tblApplicationString',
        MAX(CDCRowVersion) MaxSourceRowVer
      FROM Staging.tblApplicationString

    -- AutoDistributionReasonCodes
    INSERT INTO @tblEntityLastRowVersion (EntityName, StagingTableName, LastRowVersion)
      SELECT
        'AutoDistributionReasonCode',
        'staging.tblAutoDistributionReasonCodes',
        MAX(CDCRowVersion) MaxSourceRowVer
      FROM staging.tblAutoDistributionReasonCodes

     -- Company
    INSERT INTO @tblEntityLastRowVersion (EntityName, StagingTableName, LastRowVersion)
      SELECT
        'Company',
        'staging.tblCompanies',
        MAX(CDCRowVersion) MaxSourceRowVer
      FROM staging.tblCompanies

    -- Equipment
    INSERT INTO @tblEntityLastRowVersion (EntityName, StagingTableName, LastRowVersion)
      SELECT
        'Equipment',
        'staging.tblEquipment',
        MAX(CDCRowVersion) MaxSourceRowVer
      FROM staging.tblEquipment

    -- EquipmentType
    INSERT INTO @tblEntityLastRowVersion (EntityName, StagingTableName, LastRowVersion)
      SELECT
        'EquipmentType',
        'staging.tblEquipmentTypes',
        MAX(CDCRowVersion) MaxSourceRowVer
      FROM staging.tblEquipmentTypes

    -- Personnel
    INSERT INTO @tblEntityLastRowVersion (EntityName, StagingTableName, LastRowVersion)
      SELECT
        'Personnel',
        'staging.tblPersonnel',
        MAX(CDCRowVersion) MaxSourceRowVer
      FROM staging.tblPersonnel

   -- Product
    INSERT INTO @tblEntityLastRowVersion (EntityName, StagingTableName, LastRowVersion)
      SELECT
        'Product',
        'staging.tblProducts',
        MAX(CDCRowVersion) MaxSourceRowVer
      FROM staging.tblProducts

 -- Station
    INSERT INTO @tblEntityLastRowVersion (EntityName, StagingTableName, LastRowVersion)
      SELECT
        'Station',
        'staging.tblStations',
        MAX(CDCRowVersion) MaxSourceRowVer
      FROM staging.tblStations

 -- LoadArm
    INSERT INTO @tblEntityLastRowVersion (EntityName, StagingTableName, LastRowVersion)
      SELECT
        'LoadArm',
        'staging.tblLoadArms',
        MAX(CDCRowVersion) MaxSourceRowVer
      FROM staging.tblLoadArms

 -- Tank
    INSERT INTO @tblEntityLastRowVersion (EntityName, StagingTableName, LastRowVersion)
      SELECT
        'Tank',
        'staging.tblTanks',
        MAX(CDCRowVersion) MaxSourceRowVer
      FROM staging.tblTanks

   -- Site
    INSERT INTO @tblEntityLastRowVersion (EntityName, StagingTableName, LastRowVersion)
      SELECT
        'Site',
        'staging.tblSites',
        MAX(CDCRowVersion) MaxSourceRowVer
      FROM staging.tblSites

    -- Transaction Alias
    INSERT INTO @tblEntityLastRowVersion (EntityName, StagingTableName, LastRowVersion)
      SELECT
        'TransactionAlias',
        'staging.tblTransactionAliases',
        MAX(CDCRowVersion) MaxSourceRowVer
      FROM staging.tblTransactionAliases

    -- User
    INSERT INTO @tblEntityLastRowVersion (EntityName, StagingTableName, LastRowVersion)
      SELECT
        'User',
        'staging.tblUsers',
        MAX(CDCRowVersion) MaxSourceRowVer
      FROM staging.tblUsers
      


    -- TransactionHeader
    INSERT INTO @tblEntityLastRowVersion (EntityName, StagingTableName, LastRowVersion)
      SELECT
        'TransactionHeader',
        'staging.tblTransactions',
        MAX(CDCRowVersion) MaxSourceRowVer
      FROM staging.tblTransactions

    -- TransactionLineItem
    INSERT INTO @tblEntityLastRowVersion (EntityName, StagingTableName, LastRowVersion)
      SELECT
        'TransactionLineItem',
        'staging.tblTransactionLineItems',
        MAX(CDCRowVersion) MaxSourceRowVer
      FROM staging.tblTransactionLineItems

    -- TransactionSubLineItem
    INSERT INTO @tblEntityLastRowVersion (EntityName, StagingTableName, LastRowVersion)
      SELECT
        'TransactionSubLineItem',
        'staging.tblTransactionSubLineItems',
        MAX(CDCRowVersion) MaxSourceRowVer
      FROM staging.tblTransactionSubLineItems

    -- TransactionUserData
    INSERT INTO @tblEntityLastRowVersion (EntityName, StagingTableName, LastRowVersion)
      SELECT
        'TransactionUserData',
        'staging.tblTransactionUserData',
        MAX(CDCRowVersion) MaxSourceRowVer
      FROM staging.tblTransactionUserData

    -- TransactionLineItemUserData
    INSERT INTO @tblEntityLastRowVersion (EntityName, StagingTableName, LastRowVersion)
      SELECT
        'TransactionLineItemUserData',
        'staging.tblTransactionLineItemUserData',
        MAX(CDCRowVersion) MaxSourceRowVer
      FROM staging.tblTransactionLineItemUserData



    -- CompanyToUserGroup
    INSERT INTO @tblEntityLastRowVersion (EntityName, StagingTableName, LastRowVersion)
      SELECT
        'CompanyToUserGroup',
        'staging.tblCompanyToUserGroup',
        MAX(CDCRowVersion) MaxSourceRowVer
      FROM staging.tblCompanyToUserGroup

    /*
    -- CompanyToRole
    INSERT INTO @tblEntityLastRowVersion (EntityName, StagingTableName, LastRowVersion)
      SELECT
        'CompanyToRole',
        'staging.tblCompanyToRole',
        MAX(CDCRowVersion) MaxSourceRowVer
      FROM staging.tblCompanyToRole
    */

    -- UserToUserGroup
    INSERT INTO @tblEntityLastRowVersion (EntityName, StagingTableName, LastRowVersion)
      SELECT
        'UserToUserGroup',
        'staging.tblUserToUserGroup',
        MAX(CDCRowVersion) MaxSourceRowVer
      FROM staging.tblUserToUserGroup

    -- MapSitetoSite
    INSERT INTO @tblEntityLastRowVersion (EntityName, StagingTableName, LastRowVersion)
      SELECT
        'SiteToSite',
        'staging.tblSiteToSite',
        MAX(CDCRowVersion) MaxSourceRowVer
      FROM staging.tblSiteToSite

    -- EntityUserToSite
    INSERT INTO @tblEntityLastRowVersion (EntityName, StagingTableName, LastRowVersion)
      SELECT
        'EntityUserToSite',
        'staging.tblEntityUserToSite',
        MAX(CDCRowVersion) MaxSourceRowVer
      FROM staging.tblEntityUserToSite

    -- EntityCompanyToSite
    INSERT INTO @tblEntityLastRowVersion (EntityName, StagingTableName, LastRowVersion)
      SELECT
        'EntityCompanyToSite',
        'staging.tblEntityCompanyToSite',
        MAX(CDCRowVersion) MaxSourceRowVer
      FROM staging.tblEntityCompanyToSite

    -- EntityEquipmentToSite
    INSERT INTO @tblEntityLastRowVersion (EntityName, StagingTableName, LastRowVersion)
      SELECT
        'EntityEquipmentToSite',
        'staging.tblEntityEquipmentToSite',
        MAX(CDCRowVersion) MaxSourceRowVer
      FROM staging.tblEntityEquipmentToSite

    -- EntityProductToSite
    INSERT INTO @tblEntityLastRowVersion (EntityName, StagingTableName, LastRowVersion)
      SELECT
        'EntityProductToSite',
        'staging.tblEntityProductToSite',
        MAX(CDCRowVersion) MaxSourceRowVer
      FROM staging.tblEntityProductToSite

    -- EntityPersonnelToSite
    INSERT INTO @tblEntityLastRowVersion (EntityName, StagingTableName, LastRowVersion)
      SELECT
        'EntityPersonnelToSite',
        'staging.tblEntityPersonnelToSite',
        MAX(CDCRowVersion) MaxSourceRowVer
      FROM staging.tblEntityPersonnelToSite

    -- EntityTransactionAliasToSite
    INSERT INTO @tblEntityLastRowVersion (EntityName, StagingTableName, LastRowVersion)
      SELECT
        'EntityTransactionAliasToSite',
        'staging.tblEntityTransactionAliasToSite',
        MAX(CDCRowVersion) MaxSourceRowVer
      FROM staging.tblEntityTransactionAliasToSite


    SELECT * FROM @tblEntityLastRowVersion


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
    + 'Procedure Name: [staging].[usp_GetLastProcessedRowVersions]' + CHAR(13) + CHAR(10)
    + 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS varchar(20)), '') + CHAR(13) + CHAR(10);
    RAISERROR (@_ErrMessage, 16, 1);
  END CATCH

END