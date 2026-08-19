/*
	DROP PROCEDURE [staging].[usp_SetDimensionAlternateKeys]

	EXEC [staging].[usp_SetDimensionAlternateKeys]
	
*/
CREATE PROCEDURE [staging].[usp_SetDimensionAlternateKeys]
AS
BEGIN
  ------------------------------------------------------------------------------------------------------
  -- Stored procedure: [staging].[usp_SetDimensionAlternateKeys]
  -- Author: Hansraj Bapoo
  -- Version/Date: 1.0.003 / 2012-07-13 14:21:10.4470770 -04:00
  -- Purpose: For all the staging tables used to populate the OLAP dimension tables, set the alternate key (nvarchar(50)) field of each record of in staging to match the Identity Key of the source record.  
  -- Notes:
  -- 1. This includes both the original identity key of the record itself, and any foreign key that the record maintains to the identity keys of 
  --    other tables, and that is pertinent/maintained in the OLAP database.
  -- 2. The IdentityKey reflects either the IdentityIndex(int) field as used by FuelsManager 8.0 SP4, or the IdentityGuid (uniqueidentifier) field,
  --    as used by FuelsManager Cirrus.
  -- 3. The alternate key field (e.g. SiteKey, ProductKey, etc.) effectively helps make it transparent for the rest of the OLAP system as to what version of the OLTP 
  --    FuelsManager is being used (FuelsManager 8.0 SP4 or FuelsManager Cirrus).
  -- 4. The Staging Transaction tables have been excluded from this process because of their large sizes. Setting the alternate keys for the staging Transactin tables is handled in separate Store Procedures.
  ------------------------------------------------------------------------------------------------------
  SET NOCOUNT ON;
  BEGIN TRY

    -- ApplicationString
    UPDATE staging.tblApplicationString
    SET ApplicationStringKey = CONVERT(nvarchar(50), ApplicationStringGuid),
    SiteKey = CONVERT(nvarchar(50), SiteGuid)
    WHERE IgnoreRecord = 0

    -- AutoDistributionReasonCodes
    UPDATE staging.tblAutoDistributionReasonCodes
    SET AutoDistributionReasonCodeKey = CONVERT(nvarchar(50), AutoDistributionReasonCodeGuid),
    SiteKey = CONVERT(nvarchar(50), SiteGuid)
    WHERE IgnoreRecord = 0

    -- Site
    UPDATE staging.tblSites
    SET SiteKey = CONVERT(nvarchar(50), SiteGuid)
    WHERE IgnoreRecord = 0

    -- SitetoSiteMap
    UPDATE staging.tblSiteToSite
    SET ParentSiteKey = CONVERT(nvarchar(50), ParentSiteGuid),
        ChildSiteKey = CONVERT(nvarchar(50), ChildSiteGuid)
    WHERE IgnoreRecord = 0

     -- EquipmentType
    UPDATE staging.tblEquipmentTypes
    SET EquipmentTypeKey = CONVERT(nvarchar(50), EquipmentTypeGuid),
    SiteKey = CONVERT(nvarchar(50), SiteGuid)
    WHERE IgnoreRecord = 0

    -- Station
    UPDATE staging.tblStations
    SET StationKey = CONVERT(nvarchar(50), StationGuid),
    SiteKey = CONVERT(nvarchar(50), SiteGuid)
    WHERE IgnoreRecord = 0

    -- LoadArms
    UPDATE staging.tblLoadArms
    SET LoadArmKey = CONVERT(nvarchar(50), LoadArmGuid),
    BayAStationKey = CONVERT(nvarchar(50), BayAStationGuid),
    BayBStationKey = CONVERT(nvarchar(50), BayBStationGuid)
    WHERE IgnoreRecord = 0

    -- Tank
    UPDATE staging.tblTanks
    SET TankKey = CONVERT(nvarchar(50), TankGuid),
    SiteKey = CONVERT(nvarchar(50), SiteGuid)
    WHERE IgnoreRecord = 0

    -- OwnerCloseout
    UPDATE staging.tblOwnerCloseout
    SET OwnerCloseoutKey = CONVERT(nvarchar(50), OwnerCloseoutGuid),
    ManagerCompanyKey = CONVERT(nvarchar(50), ManagerCompanyGuid),
    OwnerCompanyKey = CONVERT(nvarchar(50), OwnerCompanyGuid),
    ProductKey = CONVERT(nvarchar(50), ProductGuid)
    WHERE IgnoreRecord = 0


    -- Product
    UPDATE staging.tblProducts
    SET ProductKey = CONVERT(nvarchar(50), ProductGuid),
        SiteKey = CONVERT(nvarchar(50), SiteGuid),
        TrackingProductKey = CONVERT(nvarchar(50), TrackingProductGuid),
        MasterRecordKey = CONVERT(nvarchar(50), MasterRecordGuid)
    WHERE IgnoreRecord = 0

    -- Personnel
    UPDATE staging.tblPersonnel
    SET PersonnelKey = CONVERT(nvarchar(50), PersonnelGuid),
        SiteKey = CONVERT(nvarchar(50), SiteGuid),
        MasterRecordKey = CONVERT(nvarchar(50), MasterRecordGuid)
    WHERE IgnoreRecord = 0

    -- Company
    UPDATE staging.tblCompanies
    SET CompanyKey = CONVERT(nvarchar(50), CompanyGuid),
        SiteKey = CONVERT(nvarchar(50), SiteGuid),
        MasterRecordKey = CONVERT(nvarchar(50), MasterRecordGuid)
    WHERE IgnoreRecord = 0

    -- Equipment
    UPDATE staging.tblEquipment
    SET EquipmentKey = CONVERT(nvarchar(50), EquipmentGuid),
        SiteKey = CONVERT(nvarchar(50), SiteGuid),
        EquipmentTypeKey = CONVERT(nvarchar(50), EquipmentTypeGuid),
        MasterRecordKey = CONVERT(nvarchar(50), MasterRecordGuid)
    WHERE IgnoreRecord = 0

    -- TransactionAlias
    UPDATE staging.tblTransactionAliases
    SET TransactionAliasKey = CONVERT(nvarchar(50), TransactionAliasGuid),
        SiteKey = CONVERT(nvarchar(50), SiteGuid),
        MasterRecordKey = CONVERT(nvarchar(50), MasterRecordGuid)
    WHERE IgnoreRecord = 0


    -- User
    UPDATE staging.tblUsers
    SET UserKey = CONVERT(nvarchar(50), UserGuid),
        SiteKey = CONVERT(nvarchar(50), SiteGuid)
    WHERE IgnoreRecord = 0

    -- UserToUserGroup
    UPDATE staging.tblUserToUserGroup
    SET GroupKey = CONVERT(nvarchar(50), GroupGuid),
        SiteKey = CONVERT(nvarchar(50), SiteGuid),
        UserKey = CONVERT(nvarchar(50), UserGuid)
    WHERE IgnoreRecord = 0

    -- CompanyToUserGroup
    UPDATE staging.tblCompanyToUserGroup
    SET CompanyToUserGroupKey = CONVERT(nvarchar(50), CompanyToUserGroupGuid),
        CompanyKey = CONVERT(nvarchar(50), CompanyGuid),
        SiteKey = CONVERT(nvarchar(50), SiteGuid),
        GroupKey = CONVERT(nvarchar(50), GroupGuid)
    WHERE IgnoreRecord = 0

    -- UserToSite
    UPDATE staging.tblEntityUserToSite
    SET UserToSiteKey = CONVERT(nvarchar(50), UserToSiteGuid),
        SiteKey = CONVERT(nvarchar(50), SiteGuid),
        UserKey = CONVERT(nvarchar(50), UserGuid)
    WHERE IgnoreRecord = 0

    -- CompanyToSite
    UPDATE staging.tblEntityCompanyToSite
    SET CompanyKey = CONVERT(nvarchar(50), CompanyGuid),
        SiteKey = CONVERT(nvarchar(50), SiteGuid),
        AssignedFromSiteKey = CONVERT(nvarchar(50), AssignedFromSiteGuid)
    WHERE IgnoreRecord = 0

    -- EquipmentToSite
    UPDATE staging.tblEntityEquipmentToSite
    SET EquipmentKey = CONVERT(nvarchar(50), EquipmentGuid),
        SiteKey = CONVERT(nvarchar(50), SiteGuid),
        AssignedFromSiteKey = CONVERT(nvarchar(50), AssignedFromSiteGuid)
    WHERE IgnoreRecord = 0

    -- ProductToSite
    UPDATE staging.tblEntityProductToSite
    SET ProductKey = CONVERT(nvarchar(50), ProductGuid),
        SiteKey = CONVERT(nvarchar(50), SiteGuid),
        AssignedFromSiteKey = CONVERT(nvarchar(50), AssignedFromSiteGuid)
    WHERE IgnoreRecord = 0

    -- PersonnelToSite
    UPDATE staging.tblEntityPersonnelToSite
    SET PersonnelKey = CONVERT(nvarchar(50), PersonnelGuid),
        SiteKey = CONVERT(nvarchar(50), SiteGuid),
        AssignedFromSiteKey = CONVERT(nvarchar(50), AssignedFromSiteGuid)
    WHERE IgnoreRecord = 0

    -- TransactionAliasToSite
    UPDATE staging.tblEntityTransactionAliasToSite
    SET TransactionAliasKey = CONVERT(nvarchar(50), TransactionAliasGuid),
        SiteKey = CONVERT(nvarchar(50), SiteGuid),
        AssignedFromSiteKey = CONVERT(nvarchar(50), AssignedFromSiteGuid)
    WHERE IgnoreRecord = 0


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
    + 'Procedure Name: [staging].[usp_SetDimensionAlternateKeys]' + CHAR(13) + CHAR(10)
    + 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS varchar(20)), '') + CHAR(13) + CHAR(10);
    RAISERROR (@_ErrMessage, 16, 1);
  END CATCH

END
GO