/*
	DROP PROCEDURE [Staging].[usp_LoadLevel1Tables]

	EXEC [staging].[usp_LoadLevel1Tables]
	
*/
CREATE PROCEDURE [staging].[usp_LoadLevel1Tables]
AS
BEGIN
  ------------------------------------------------------------------------------------------------------
  -- Stored procedure: [staging].[usp_LoadLevel1Tables]
  -- Author: Hansraj Bapoo
  -- Version/Date: 1.0.003 / 2012-07-13 14:21:10.4470770 -04:00
  -- Purpose: Loads records from staging into level 1 tables in the DW database.
  -- Notes:
  -- 1. Level 1 tables are those tables that have a foreign key dependency to a level 0 table, e.g. staging.tblCompanies has a reference to staging.ApplicationString
  -- 2. The Level 0 references have to be first sorted out before Level 1 tables can be safely loaded from staging into the OLAP database.
  -- 3. All NULL SKey references to external entities are adjusted to point to the SKey = 0 ('<Not Available>') record of that entity.
  -- 4. All null-value fields that are used as a Dimension Attribute are reset to a non-null dummy value (e.g. '<NOT AVAILABLE>').
  --    This is to avoid a Duplicate Attribute Error during the cube deployment.
  -- 5. The values of the ID fields are trimmed first before insertion because those ID fields are used when trying to identify the correct entities
  --    for transactions for which the entity id is available but the entity key is missing. In this case trimming avoids the insertion of new entity
  --    records that only differ by prefix or suffix whitespaces, a condition that is likely to lead to duplicate errors when processing the cube.
  ------------------------------------------------------------------------------------------------------
  SET NOCOUNT ON;
  BEGIN TRY

    DECLARE @dummyId varchar(15) = '<NOT AVAILABLE>'
    DECLARE @shortDummyId varchar(4) = '<NA>'


    --Product
    -- Insert all end-dated, historical records in the order the change history was recorded in the OLTP database
    INSERT INTO dbo.DimProduct 
    (
        [AKey], 
        [MasterRecordKey], 
        [SiteSKey], 
        [ProductID],         
        [ProductCode], 
        [Description], 
        [ProductTypeName], 
        [TrackingProductSKey], 
        [TrackingProductID], 
        [VolumeDecimalPlaces],
        [AviationFuelFlag],
        [GroundFuel],
        [LockedOut],
        [LockedOutReason],
        [LockedOutDate],
        [VarianceTolerance],
        [StartDate], 
        [EndDate]
    )  
    SELECT
        [ProductKey],
        [MasterRecordKey],
        [SiteSKey],
        TRIM([ProductID]) [ProductID],        
        ISNULL([ProductCode], @dummyId),
        [Description],
        [ProductTypeName],
		ISNULL([TrackingProductSKey], 0), 
		ISNULL([TrackingProductID], @dummyId), 
		[VolumeDecimalPlaces],
        ISNULL([AviationFuelFlag], 0),
        ISNULL([GroundFuel], 0),
        ISNULL([LockedOut], 0),
        [LockedOutReason],
        [LockedOutDate],
        ISNULL([VarianceTolerance], 0),
        [StartDate],
        [EndDate]
      FROM staging.tblProducts
      WHERE EndDate IS NOT NULL
      AND IsRecordDeleted = 0
      AND IgnoreRecord = 0
      ORDER BY ProductKey, COALESCE(RecordUpdatedDate, UpdatedDate)

    -- EndDate records for which a later revision has been recorded
    -- Create a new open-ended record for each end-dated record for which there is a new revision and for each new record for which there are no revisions on record.
    INSERT INTO dbo.DimProduct 
    (
        [AKey], 
        [MasterRecordKey], 
        [SiteSKey], 
        [ProductID],         
        [ProductCode], 
        [Description], 
        [ProductTypeName], 
        [TrackingProductSKey], 
        [TrackingProductID], 
        [VolumeDecimalPlaces],
        [AviationFuelFlag],
        [GroundFuel],
        [LockedOut],
        [LockedOutReason],
        [LockedOutDate],
        [VarianceTolerance],
        [StartDate], 
        [EndDate]
    )
    SELECT
        [ProductKey],
        [MasterRecordKey],
        [SiteSKey],
        [ProductID],        
        [ProductCode],
        [Description],
        [ProductTypeName],
		[TrackingProductSKey],
		[TrackingProductID],
		[VolumeDecimalPlaces],
        [AviationFuelFlag],
        [GroundFuel],
        [LockedOut],
        [LockedOutReason],
        [LockedOutDate],
        [VarianceTolerance],
        [StartDate],
        [EndDate]
      FROM (
      MERGE dbo.DimProduct AS tgt
      USING (SELECT
        [ProductKey],
        [MasterRecordKey],
        [SiteSKey],
        TRIM([ProductID]) [ProductID],        
        ISNULL([ProductCode], @dummyId) [ProductCode],
        [Description],
        [ProductTypeName],
		ISNULL([TrackingProductSKey], 0) [TrackingProductSKey], 
		ISNULL([TrackingProductID], @dummyId) [TrackingProductID], 
		[VolumeDecimalPlaces],
        ISNULL([AviationFuelFlag], 0) [AviationFuelFlag],
        ISNULL([GroundFuel], 0) [GroundFuel],
        ISNULL([LockedOut], 0) [LockedOut],
        [LockedOutReason],
        [LockedOutDate],
        ISNULL([VarianceTolerance], 0) [VarianceTolerance],
        [StartDate],
        [EndDate],
        [IsRecordDeleted]
      FROM staging.tblProducts
      WHERE EndDate IS NULL
      AND ProductKey IS NOT NULL
      AND IsRecordDeleted = 0
      AND IgnoreRecord = 0) AS src
      ON tgt.AKey = src.ProductKey AND tgt.EndDate IS NULL
      WHEN NOT MATCHED THEN
      INSERT ([AKey], [MasterRecordKey], [SiteSKey], [ProductID], [ProductCode], [Description], [ProductTypeName], [TrackingProductSKey], [TrackingProductID], [VolumeDecimalPlaces], [AviationFuelFlag], [GroundFuel], [LockedOut], [LockedOutReason], [LockedOutDate], [VarianceTolerance], [StartDate], [EndDate])
      VALUES (src.[ProductKey], src.[MasterRecordKey], src.[SiteSKey], src.[ProductID], src.[ProductCode], src.[Description], src.[ProductTypeName], src.[TrackingProductSKey], src.[TrackingProductID], src.[VolumeDecimalPlaces], src.[AviationFuelFlag], src.[GroundFuel], src.[LockedOut], src.[LockedOutReason], src.[LockedOutDate], src.[VarianceTolerance], src.[StartDate], NULL)
      WHEN MATCHED AND src.StartDate > tgt.StartDate THEN
      UPDATE SET tgt.EndDate = src.StartDate
      OUTPUT $ACTION Action_Out, src.[ProductKey], src.[MasterRecordKey], src.[SiteSKey], src.[ProductID], src.[ProductCode], src.[Description], src.[ProductTypeName], src.[TrackingProductSKey], src.[TrackingProductID], src.[VolumeDecimalPlaces], src.[AviationFuelFlag], src.[GroundFuel], src.[LockedOut], src.[LockedOutReason], src.[LockedOutDate], src.[VarianceTolerance], src.[StartDate], src.[EndDate], src.[IsRecordDeleted]
      ) AS Merge_out
      WHERE Merge_Out.Action_Out = 'UPDATE'
      AND ISNULL(Merge_out.IsRecordDeleted, 0) = 0;

    -- EndDate records which have been deleted
    UPDATE a
    SET a.EndDate = b.RecordUpdatedDate  
    FROM dbo.DimProduct a
    INNER JOIN staging.tblProducts b
    ON b.ProductKey = a.AKey
    WHERE b.IsRecordDeleted = 1
    AND b.ProductKey IS NOT NULL
    AND b.IgnoreRecord = 0
    AND a.EndDate IS NULL



    --Company
    -- Insert all end-dated, historical records in the order the change history was recorded in the OLTP database
    INSERT INTO dbo.DimCompany 
    (
        [AKey],
        [MasterRecordKey],
        [SiteSKey],
        [CompanyId],
        [Name],
        [Code],
        [Address1],
        [Address2],
        [City],
        [State],
        [Zip],
        [Country],
        [Phone],
        [EmergencyContact],
        [EmergencyPhone],
        [LockedOut],
        [LockedOutReason],
        [LockedOutDate],
        [StartDate],
        [EndDate]
    )  
    SELECT
        [CompanyKey],
        [MasterRecordKey],
        [SiteSKey],
        TRIM([ID]) [CompanyID],  
        [Name],
        [Code],
        [Address1],
        [Address2],
        [City],
        [State],
        [Zip],
        [Country],
        [Phone],
        [EmergencyContact],
        [EmergencyPhone],
        ISNULL([LockedOut], 0) [LockedOut],
        [LockedOutReason],
        [LockedOutDate],
        [StartDate],
        [EndDate]
      FROM staging.tblCompanies
      WHERE EndDate IS NOT NULL
      AND IsRecordDeleted = 0
      AND IgnoreRecord = 0
      ORDER BY CompanyKey, COALESCE(RecordUpdatedDate, UpdatedDate)

    -- EndDate records for which a later revision has been recorded
    -- Create a new open-ended record for each end-dated record for which there is a new revision and for each new record for which there are no revisions on record.
    INSERT INTO dbo.DimCompany
    (
        [AKey],
        [MasterRecordKey],
        [SiteSKey],
        [CompanyId],
        [Name],
        [Code],
        [Address1],
        [Address2],
        [City],
        [State],
        [Zip],
        [Country],
        [Phone],
        [EmergencyContact],
        [EmergencyPhone],
        [LockedOut],
        [LockedOutReason],
        [LockedOutDate],
        [StartDate],
        [EndDate]
    )
    SELECT
        [CompanyKey],
        [MasterRecordKey],
        [SiteSKey],
        [CompanyId],
        [Name],
        [Code],
        [Address1],
        [Address2],
        [City],
        [State],
        [Zip],
        [Country],
        [Phone],
        [EmergencyContact],
        [EmergencyPhone],
        [LockedOut],
        [LockedOutReason],
        [LockedOutDate],
        [StartDate],
        [EndDate]
      FROM (
      MERGE dbo.DimCompany AS tgt
      USING (SELECT
        [CompanyKey],
        [MasterRecordKey],
        [SiteSKey],
        TRIM([ID]) [CompanyID],  
        [Name],
        [Code],
        [Address1],
        [Address2],
        [City],
        [State],
        [Zip],
        [Country],
        [Phone],
        [EmergencyContact],
        [EmergencyPhone],
        ISNULL([LockedOut], 0) [LockedOut],
        [LockedOutReason],
        [LockedOutDate],
        [StartDate],
        [EndDate],
        [IsRecordDeleted]
      FROM staging.tblCompanies
      WHERE EndDate IS NULL
      AND CompanyKey IS NOT NULL
      AND IsRecordDeleted = 0
      AND IgnoreRecord = 0) AS src
      ON tgt.AKey = src.CompanyKey AND tgt.EndDate IS NULL
      WHEN NOT MATCHED THEN
      INSERT ([AKey], [MasterRecordKey], [SiteSKey], [CompanyId], [Name], [Code], [Address1], [Address2], [City], [State], [Zip], [Country], [Phone], [EmergencyContact], [EmergencyPhone], [LockedOut], [LockedOutReason], [LockedOutDate], [StartDate], [EndDate])
      VALUES (src.[CompanyKey], src.[MasterRecordKey], src.[SiteSKey], src.[CompanyId], src.[Name], src.[Code], src.[Address1], src.[Address2], src.[City], src.[State], src.[Zip], src.[Country], src.[Phone], src.[EmergencyContact], src.[EmergencyPhone], src.[LockedOut], src.[LockedOutReason], src.[LockedOutDate], src.[StartDate], NULL)
      WHEN MATCHED  AND src.StartDate > tgt.StartDate THEN
      UPDATE SET tgt.EndDate = src.StartDate
      OUTPUT $ACTION Action_Out, src.[CompanyKey], src.[MasterRecordKey], src.[SiteSKey], src.[CompanyId], src.[Name], src.[Code], src.[Address1], src.[Address2], src.[City], src.[State], src.[Zip], src.[Country], src.[Phone], src.[EmergencyContact], src.[EmergencyPhone], src.[LockedOut], src.[LockedOutReason], src.[LockedOutDate], src.[StartDate], src.[EndDate], src.[IsRecordDeleted]
      ) AS Merge_out
      WHERE Merge_Out.Action_Out = 'UPDATE'
      AND ISNULL(Merge_out.IsRecordDeleted, 0) = 0;

    -- EndDate records which have been deleted
    UPDATE a
    SET a.EndDate = b.RecordUpdatedDate
    FROM dbo.DimCompany a
    INNER JOIN staging.tblCompanies b
    ON b.CompanyKey = a.AKey
    WHERE b.IsRecordDeleted = 1
    AND b.CompanyKey IS NOT NULL
    AND b.IgnoreRecord = 0
    AND a.EndDate IS NULL



    --Equipment
    -- Insert all end-dated, historical records in the order the change history was recorded in the OLTP database
    INSERT INTO dbo.DimEquipment 
    (
        [AKey],
        [MasterRecordKey],
        [SiteSKey],
        [EquipmentId],
        [EquipmentTypeSKey],
        [Description],
        [Make],
        [Model],
        [InUse],
        [SerialNumber],
        [StartDate],
        [EndDate]
    )  
    SELECT
        [EquipmentKey],
        [MasterRecordKey],
        [SiteSKey],
        TRIM([ID]) [EquipmentId],
        ISNULL([EquipmentTypeSKey], 0) [EquipmentTypeSKey],
        [Description],
        (CASE WHEN ([Make] IS NULL OR (LEN(TRIM([Make])) = 0)) THEN @dummyId ELSE [Make] END) [Make],
        (CASE WHEN ([Model] IS NULL OR (LEN(TRIM([Model])) = 0)) THEN @dummyId ELSE [Model] END) [Model],
        ISNULL([InUse], 0) [InUse],
        (CASE WHEN ([SerialNumber] IS NULL OR (LEN(TRIM([SerialNumber])) = 0)) THEN @shortDummyId ELSE [SerialNumber] END) [SerialNumber],
        [StartDate],
        [EndDate]
      FROM staging.tblEquipment
      WHERE EndDate IS NOT NULL
      AND IsRecordDeleted = 0
      AND IgnoreRecord = 0
      ORDER BY EquipmentKey, COALESCE(RecordUpdatedDate, UpdatedDate)

    -- EndDate records for which a later revision has been recorded
    -- Create a new open-ended record for each end-dated record for which there is a new revision and for each new record for which there are no revisions on record.
    INSERT INTO dbo.DimEquipment
    (
        [AKey],
        [MasterRecordKey],
        [SiteSKey],
        [EquipmentId],
        [EquipmentTypeSKey],
        [Description],
        [Make],
        [Model],
        [InUse],
        [SerialNumber],
        [StartDate],
        [EndDate]
    )
    SELECT
        [EquipmentKey],
        [MasterRecordKey],
        [SiteSKey],
        [EquipmentId],
        [EquipmentTypeSKey],
        [Description],
        [Make],
        [Model],
        [InUse],
        [SerialNumber],
        [StartDate],
        [EndDate]
      FROM (
      MERGE dbo.DimEquipment AS tgt
      USING (SELECT
        [EquipmentKey],
        [MasterRecordKey],
        [SiteSKey],
        TRIM([ID]) [EquipmentId],
        ISNULL([EquipmentTypeSKey], 0) [EquipmentTypeSKey],
        [Description],
        (CASE WHEN ([Make] IS NULL OR (LEN(TRIM([Make])) = 0)) THEN @dummyId ELSE [Make] END) [Make],
        (CASE WHEN ([Model] IS NULL OR (LEN(TRIM([Model])) = 0)) THEN @dummyId ELSE [Model] END) [Model],
        ISNULL([InUse], 0) [InUse],
        (CASE WHEN ([SerialNumber] IS NULL OR (LEN(TRIM([SerialNumber])) = 0)) THEN @shortDummyId ELSE [SerialNumber] END) [SerialNumber],        
        [StartDate],
        [EndDate],
        [IsRecordDeleted]
      FROM staging.tblEquipment
      WHERE EndDate IS NULL
      AND EquipmentKey IS NOT NULL
      AND IsRecordDeleted = 0
      AND IgnoreRecord = 0) AS src
      ON tgt.AKey = src.EquipmentKey AND tgt.EndDate IS NULL
      WHEN NOT MATCHED THEN
      INSERT ([AKey], [MasterRecordKey], [SiteSKey], [EquipmentId], [EquipmentTypeSKey], [Description], [Make], [Model], [InUse], [SerialNumber], [StartDate], [EndDate])
      VALUES (src.[EquipmentKey], src.[MasterRecordKey], src.[SiteSKey], src.[EquipmentId], src.[EquipmentTypeSKey], src.[Description], src.[Make], src.[Model], src.[InUse], src.[SerialNumber], src.[StartDate], NULL)
      WHEN MATCHED  AND src.StartDate > tgt.StartDate THEN
      UPDATE SET tgt.EndDate = src.StartDate
      OUTPUT $ACTION Action_Out, src.[EquipmentKey], src.[MasterRecordKey], src.[SiteSKey], src.[EquipmentId], src.[EquipmentTypeSKey], src.[Description], src.[Make], src.[Model], src.[InUse], src.[SerialNumber], src.[StartDate], src.[EndDate], src.[IsRecordDeleted]
      ) AS Merge_out
      WHERE Merge_Out.Action_Out = 'UPDATE'
      AND ISNULL(Merge_out.IsRecordDeleted, 0) = 0;

    -- EndDate records which have been deleted
    UPDATE a
    SET a.EndDate = b.RecordUpdatedDate
    FROM dbo.DimEquipment a
    INNER JOIN staging.tblEquipment b
    ON b.EquipmentKey = a.AKey
    WHERE b.IsRecordDeleted = 1
    AND b.EquipmentKey IS NOT NULL
    AND b.IgnoreRecord = 0
    AND a.EndDate IS NULL



    --Personnel
    -- Insert all end-dated, historical records in the order the change history was recorded in the OLTP database
    INSERT INTO dbo.DimPersonnel 
    (
        [AKey],
        [MasterRecordKey],
        [SiteSKey],
        [PersonID],
        [FirstName],
        [MiddleName],
        [LastName],
        [LockedOut],
        [LockedOutReason],
        [LockedOutDate],
        [StartDate],
        [EndDate]
    )  
    SELECT
        [PersonnelKey],
        [MasterRecordKey],
        [SiteSKey],
        TRIM([PersonID]) [PersonID],
        [FirstName],
        [MiddleName],
        [LastName],
        ISNULL([LockedOut], 0) [LockedOut],
        [LockedOutReason],
        [LockedOutDate],
        [StartDate],
        [EndDate]
      FROM staging.tblPersonnel
      WHERE EndDate IS NOT NULL
      AND IsRecordDeleted = 0
      AND IgnoreRecord = 0
      ORDER BY PersonnelKey, COALESCE(RecordUpdatedDate, UpdatedDate)

    -- EndDate records for which a later revision has been recorded
    -- Create a new open-ended record for each end-dated record for which there is a new revision and for each new record for which there are no revisions on record.
    INSERT INTO dbo.DimPersonnel
    (
        [AKey],
        [MasterRecordKey],
        [SiteSKey],
        [PersonID],
        [FirstName],
        [MiddleName],
        [LastName],
        [LockedOut],
        [LockedOutReason],
        [LockedOutDate],
        [StartDate],
        [EndDate]
    )
    SELECT
        [PersonnelKey],
        [MasterRecordKey],
        [SiteSKey],
        [PersonID],
        [FirstName],
        [MiddleName],
        [LastName],
        [LockedOut],
        [LockedOutReason],
        [LockedOutDate],
        [StartDate],
        [EndDate]
      FROM (
      MERGE dbo.DimPersonnel AS tgt
      USING (SELECT
        [PersonnelKey],
        [MasterRecordKey],
        [SiteSKey],
        TRIM([PersonID]) [PersonID],
        [FirstName],
        [MiddleName],
        [LastName],
        ISNULL([LockedOut], 0) [LockedOut],
        [LockedOutReason],
        [LockedOutDate],
        [StartDate],
        [EndDate],
        [IsRecordDeleted]
      FROM staging.tblPersonnel
      WHERE EndDate IS NULL
      AND PersonnelKey IS NOT NULL
      AND IsRecordDeleted = 0
      AND IgnoreRecord = 0) AS src
      ON tgt.AKey = src.PersonnelKey AND tgt.EndDate IS NULL
      WHEN NOT MATCHED THEN
      INSERT ([AKey],[MasterRecordKey], [SiteSKey], [PersonID], [FirstName], [MiddleName], [LastName], [LockedOut], [LockedOutReason], [LockedOutDate], [StartDate], [EndDate])
      VALUES (src.[PersonnelKey],src.[MasterRecordKey], src.[SiteSKey], src.[PersonID], src.[FirstName], src.[MiddleName], src.[LastName], src.[LockedOut], src.[LockedOutReason], src.[LockedOutDate], src.[StartDate], NULL)
      WHEN MATCHED  AND src.StartDate > tgt.StartDate THEN
      UPDATE SET tgt.EndDate = src.StartDate
      OUTPUT $ACTION Action_Out, src.[PersonnelKey],src.[MasterRecordKey], src.[SiteSKey], src.[PersonID], src.[FirstName], src.[MiddleName], src.[LastName], src.[LockedOut], src.[LockedOutReason], src.[LockedOutDate], src.[StartDate], src.[EndDate], src.[IsRecordDeleted]
      ) AS Merge_out
      WHERE Merge_Out.Action_Out = 'UPDATE'
      AND ISNULL(Merge_out.IsRecordDeleted, 0) = 0;

    -- EndDate records which have been deleted
    UPDATE a
    SET a.EndDate = b.RecordUpdatedDate
    FROM dbo.DimPersonnel a
    INNER JOIN staging.tblPersonnel b
    ON b.PersonnelKey = a.AKey
    WHERE b.IsRecordDeleted = 1
    AND b.PersonnelKey IS NOT NULL
    AND b.IgnoreRecord = 0
    AND a.EndDate IS NULL



    --TransactionAlias
    -- Insert all end-dated, historical records in the order the change history was recorded in the OLTP database
    INSERT INTO dbo.DimTransactionAlias
    (
        [AKey],
        [MasterRecordKey],
        [SiteSKey],
        [AliasName],
        [TransactionTypeSKey],
        [StartDate],
        [EndDate]
    )  
    SELECT
        [TransactionAliasKey],
        [MasterRecordKey],
        [SiteSKey],
        TRIM([AliasName]) [AliasName],
        ISNULL([TransactionTypeSKey], 0) [TransactionTypeSKey],
        [StartDate],
        [EndDate]
      FROM staging.tblTransactionAliases
      WHERE EndDate IS NOT NULL
      AND IsRecordDeleted = 0
      AND IgnoreRecord = 0
      ORDER BY TransactionAliasKey, COALESCE(RecordUpdatedDate, UpdatedDate)

    -- EndDate records for which a later revision has been recorded
    -- Create a new open-ended record for each end-dated record for which there is a new revision and for each new record for which there are no revisions on record.
    INSERT INTO dbo.DimTransactionAlias
    (
        [AKey],
        [MasterRecordKey],
        [SiteSKey],
        [AliasName],
        [TransactionTypeSKey],
        [StartDate],
        [EndDate]
    )
    SELECT
        [TransactionAliasKey],
        [MasterRecordKey],
        [SiteSKey],
        [AliasName],
        [TransactionTypeSKey],
        [StartDate],
        [EndDate]
      FROM (
      MERGE dbo.DimTransactionAlias AS tgt
      USING (SELECT
        [TransactionAliasKey],
        [MasterRecordKey],
        [SiteSKey],
        TRIM([AliasName]) [AliasName],
        ISNULL([TransactionTypeSKey], 0) [TransactionTypeSKey],
        [StartDate],
        [EndDate],
        [IsRecordDeleted]
      FROM staging.tblTransactionAliases
      WHERE EndDate IS NULL
      AND TransactionAliasKey IS NOT NULL
      AND IsRecordDeleted = 0
      AND IgnoreRecord = 0) AS src
      ON tgt.AKey = src.TransactionAliasKey AND tgt.EndDate IS NULL
      WHEN NOT MATCHED THEN
      INSERT ([AKey], [MasterRecordKey], [SiteSKey], [AliasName], [TransactionTypeSKey], [StartDate], [EndDate])
      VALUES (src.[TransactionAliasKey], src.[MasterRecordKey], src.[SiteSKey], src.[AliasName], src.[TransactionTypeSKey], src.[StartDate], NULL)
      WHEN MATCHED  AND src.StartDate > tgt.StartDate THEN
      UPDATE SET tgt.EndDate = src.StartDate
      OUTPUT $ACTION Action_Out, src.[TransactionAliasKey], src.[MasterRecordKey], src.[SiteSKey], src.[AliasName], src.[TransactionTypeSKey], src.[StartDate], src.[EndDate], src.[IsRecordDeleted]
      ) AS Merge_out
      WHERE Merge_Out.Action_Out = 'UPDATE'
      AND ISNULL(Merge_out.IsRecordDeleted, 0) = 0;

    -- EndDate records which have been deleted
    UPDATE a
    SET a.EndDate = b.RecordUpdatedDate
    FROM dbo.DimTransactionAlias a
    INNER JOIN staging.tblTransactionAliases b
    ON b.TransactionAliasKey = a.AKey
    WHERE b.IsRecordDeleted = 1
    AND b.TransactionAliasKey IS NOT NULL
    AND b.IgnoreRecord = 0
    AND a.EndDate IS NULL




    --SiteToSite
    -- No historical data maintained for map.tblSitetoSite. Simply update the existing record if found, otherwise insert a new one.
    MERGE map.tblSiteToSite AS tgt
    USING (SELECT
      [ParentSiteSKey],
      [ChildSiteSKey],
      [SitetoSiteKey],
      [IsRecordDeleted],
      [CombinedUpdatedDate]
    FROM staging.tblSiteToSite
    WHERE ParentSiteSKey IS NOT NULL
    AND ChildSiteSKey IS NOT NULL
    AND IgnoreRecord = 0) AS src
    ON tgt.ParentSiteSKey = src.ParentSiteSKey AND tgt.ChildSiteSKey = src.ChildSiteSKey
    WHEN NOT MATCHED AND src.IsRecordDeleted = 0 THEN
    INSERT ([ParentSiteSKey], [ChildSiteSKey], [SitetoSiteKey], [_DeletedFlag], [_RecordUpdatedDate])
    VALUES (src.[ParentSiteSKey], src.[ChildSiteSKey], src.[SitetoSiteKey], src.[IsRecordDeleted], src.[CombinedUpdatedDate])
    WHEN MATCHED AND src.CombinedUpdatedDate > tgt._RecordUpdatedDate THEN
    UPDATE SET tgt.[_DeletedFlag] = src.[IsRecordDeleted],
    tgt.[_RecordUpdatedDate] = src.[CombinedUpdatedDate];



    --UserToSite
    -- No historical data maintained for FactFMUserToSiteGroup. Simply update the existing record if found, otherwise insert a new one.
    MERGE FactFMUserToSite AS tgt
    USING (SELECT
      [UserSKey],
      [SiteSKey],
      [IsRecordDeleted],
      [CombinedUpdatedDate]
    FROM staging.tblEntityUserToSite
    WHERE UserKey IS NOT NULL
    AND SiteKey IS NOT NULL
    AND IgnoreRecord = 0) AS src
    ON tgt.FMUserSKey = src.UserSKey AND tgt.SiteSKey = src.SiteSKey
    WHEN NOT MATCHED AND IsRecordDeleted = 0 THEN
    INSERT ([FMUserSKey], [SiteSKey], [_DeletedFlag], [_RecordUpdatedDate])
    VALUES (src.[UserSKey], src.[SiteSKey], src.[IsRecordDeleted], src.[CombinedUpdatedDate])
    WHEN MATCHED AND src.CombinedUpdatedDate > tgt._RecordUpdatedDate THEN
    UPDATE SET tgt.[_DeletedFlag] = src.[IsRecordDeleted],
    tgt.[_RecordUpdatedDate] = src.[CombinedUpdatedDate];



    -- Station
    -- (DimStation is a Level1 table because of its SiteSKey reference, which is maintained to support a Site-Station-Arm hierarchy)
    MERGE dbo.DimStation AS tgt
    USING (SELECT
        [StationKey],
        [SiteSKey],
	    [ID],	    
	    [StationInterfaceTypeCode],
	    [IsRecordDeleted],
        [CombinedUpdatedDate]
    FROM staging.tblStations
    WHERE StationKey IS NOT NULL
    AND IgnoreRecord = 0) AS src
    ON tgt.StationId = src.ID
    WHEN NOT MATCHED THEN
    INSERT ([AKey], [SiteSKey], [StationID], [StationInterfaceTypeCode], [_DeletedFlag], [_RecordUpdatedDate])
    VALUES (src.[StationKey], src.[SiteSKey], src.[ID], src.[StationInterfaceTypeCode], src.[IsRecordDeleted], src.[CombinedUpdatedDate])
    WHEN MATCHED AND src.CombinedUpdatedDate > tgt._RecordUpdatedDate THEN
    UPDATE SET tgt.[_DeletedFlag] = src.[IsRecordDeleted],
        tgt.[_RecordUpdatedDate] = src.[CombinedUpdatedDate];



    -- Tank
    -- (DimTank is treated as a Level1 table because the Company references of tblTanks have been excluded in DimTank. The company references to Tanks can be indirectly extracted from the OLAP cube from the FactTransaction.)
    MERGE dbo.DimTank AS tgt
    USING (SELECT
        [TankKey],
        [SiteSKey],
	    [TankID],	    
	    [VesselTypeName],
	    [IsRecordDeleted],
        [CombinedUpdatedDate]
    FROM staging.tblTanks
    WHERE TankKey IS NOT NULL
    AND IgnoreRecord = 0) AS src
    ON tgt.TankId = src.TankId
    WHEN NOT MATCHED THEN
    INSERT ([AKey], [SiteSKey], [TankID], [VesselTypeName], [_DeletedFlag], [_RecordUpdatedDate])
    VALUES (src.[TankKey], src.[SiteSKey], src.[TankId], src.[VesselTypeName], src.[IsRecordDeleted], src.[CombinedUpdatedDate])
    WHEN MATCHED AND src.CombinedUpdatedDate > tgt._RecordUpdatedDate THEN
    UPDATE SET tgt.[_DeletedFlag] = src.[IsRecordDeleted],
        tgt.[_RecordUpdatedDate] = src.[CombinedUpdatedDate];


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
    + 'Procedure Name: [staging].[usp_LoadLevel1Tables]' + CHAR(13) + CHAR(10)
    + 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS varchar(20)), '') + CHAR(13) + CHAR(10);
    RAISERROR (@_ErrMessage, 16, 1);
  END CATCH

END