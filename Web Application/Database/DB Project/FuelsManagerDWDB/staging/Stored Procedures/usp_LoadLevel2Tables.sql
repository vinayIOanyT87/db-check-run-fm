/*
 DROP PROCEDURE [Staging].[usp_LoadLevel2Tables]

EXEC [staging].[usp_LoadLevel2Tables]
	
*/
CREATE PROCEDURE [staging].[usp_LoadLevel2Tables]
AS
BEGIN
  ------------------------------------------------------------------------------------------------------
  -- Stored procedure: [staging].[usp_LoadLevel2Tables]
  -- Author: Hansraj Bapoo
  -- Version/Date: 1.0.003 / 2012-07-13 14:21:10.4470770 -04:00
  -- Purpose: Loads records from staging into level 2 tables in the OLAP database.
  -- Notes:
  -- 1. Level 2 tables are those tables that have a foreign key dependency to a level 1 table, e.g. the process of loading data into the company dimension tables relies on information from map.tblCompanyToRole.
  -- 2. The Level 1 references have to be first sorted out before Level 2 tables can be safely loaded from staging into the OLAP database.
  -- 3. The values of the ID fields are trimmed first before insertion because those ID fields are used when trying to identify the correct entities
  --    for transactions for which the entity id is available but the entity key is missing. In this case trimming avoids the insertion of new entity
  --    records that only differ by prefix or suffix whitespaces, a condition that is likely to lead to duplicate errors when processing the cube.
  ------------------------------------------------------------------------------------------------------
  SET NOCOUNT ON;
  BEGIN TRY

    --SiteToSite
    -- Identify the SiteGroups
    UPDATE a
    SET a.Processed = NULL,
        a.IsParentASiteGroup = b.SiteGroupFlag
    FROM staging.tblSiteToSite a
    INNER JOIN DimSite b
      ON b.SKey = a.ParentSiteSKey

    --Collapse the Sitegroups into a simple two-level (parent-child) hierarchy, irrespective of the depth of site-group in the actual multi-level site hierarchy.
    DECLARE @parentSiteSKey int
    WHILE ((SELECT
        COUNT(*)
      FROM staging.tblSiteToSite
      WHERE IsParentASiteGroup = 1
      AND IgnoreRecord = 0
      AND Processed IS NULL)
      > 0)
    BEGIN
      SET @parentSiteSKey = (SELECT TOP (1)
        ParentSiteSKey
      FROM staging.tblSiteToSite
      WHERE ISParentASiteGroup = 1
      AND IgnoreRecord = 0
      AND Processed IS NULL)

      INSERT INTO staging.tblSiteHierarchyBridge (ParentSiteSKey, ChildSiteSKey, RecordUpdatedDate, IsRecordDeleted)
        SELECT DISTINCT
          @parentSiteSKey,
          a.SiteSKey,
          a.RecordUpdatedDate,
          a.IsRecordDeleted
        FROM [map].[udf_GetSiteHierarchy](@parentSiteSKey, 1) a
        WHERE ISNULL(a.SiteGroupFlag, 0) <> 1
        AND NOT EXISTS (SELECT
          *
        FROM staging.tblSiteHierarchyBridge b
        WHERE b.ParentSiteSKey = @parentSiteSKey
        AND b.ChildSiteSKey = a.SiteSKey)

      UPDATE staging.tblSiteToSite
      SET Processed = 1
      WHERE ParentSiteSKey = @parentSiteSKey
    END

    --Include self-mappings for sitegroups only (not for sites)
    INSERT INTO staging.tblSiteHierarchyBridge (ParentSiteSKey, ChildSiteSKey, RecordUpdatedDate, IsRecordDeleted)
      SELECT
        a.ParentSiteSKey,
        a.ChildSiteSKey,
        a.RecordUpdatedDate,
        a.IsRecordDeleted
      FROM staging.tblSiteToSite a
      INNER JOIN dbo.DimSite b
        ON b.SKey = a.ParentSiteSKey
      WHERE a.IgnoreRecord = 0
      AND a.ChildSiteSKey = a.ParentSiteSKey
      AND ISNULL(b.SiteGroupFlag, 0) = 1
      AND NOT EXISTS (SELECT
        *
      FROM staging.tblSiteHierarchyBridge b
      WHERE b.ParentSiteSKey = a.ParentSiteSKey
      AND b.ChildSiteSKey = a.ChildSiteSKey)

    --Include all other mappings captured, excluding mappings to sitegroups and self-mappings
    INSERT INTO staging.tblSiteHierarchyBridge (ParentSiteSKey, ChildSiteSKey, RecordUpdatedDate, IsRecordDeleted)
      SELECT
        a.ParentSiteSKey,
        a.ChildSiteSKey,
        a.RecordUpdatedDate,
        a.IsRecordDeleted
      FROM staging.tblSiteToSite a
      INNER JOIN dimSite b
        ON b.SKey = a.ChildSiteSKey
      WHERE a.IgnoreRecord = 0
      AND ISNULL(b.SiteGroupFlag, 0) <> 1
      AND a.ParentSiteSKey <> a.ChildSiteSKey
      AND NOT EXISTS (SELECT
        *
      FROM staging.tblSiteHierarchyBridge b
      WHERE b.ParentSiteSKey = a.ParentSiteSKey
      AND b.ChildSiteSKey = a.ChildSiteSKey)

    -- Load the collapsed hierarchy from staging into dbo.FactSiteHierarchyBridge
    -- No historical data maintained for map.tblSitetoSite. Simply update the existing record if found, otherwise insert a new one.
    MERGE dbo.FactSiteHierarchyBridge AS tgt
    USING (SELECT DISTINCT
      [ParentSiteSKey],
      [ChildSiteSKey],
      [IsRecordDeleted],
      [RecordUpdatedDate]
    FROM staging.tblSiteHierarchyBridge
    WHERE ParentSiteSKey IS NOT NULL
    AND ChildSiteSKey IS NOT NULL
    AND IgnoreRecord = 0) AS src
    ON tgt.ParentSKey = src.ParentSiteSKey AND tgt.ChildSKey = src.ChildSiteSKey
    WHEN NOT MATCHED AND IsRecordDeleted = 0 THEN
    INSERT ([ParentSKey], [ChildSKey], [_DeletedFlag], [_RecordUpdatedDate])
    VALUES (src.[ParentSiteSKey], src.[ChildSiteSKey], src.[IsRecordDeleted], src.[RecordUpdatedDate])
    WHEN MATCHED AND src.RecordUpdatedDate > tgt._RecordUpdatedDate THEN
    UPDATE SET tgt.[_DeletedFlag] = src.[IsRecordDeleted],
    tgt.[_RecordUpdatedDate] = src.[RecordUpdatedDate];



    --CompanyToUserGroup
    -- No historical data maintained for map.CompanyToUserGroup. Simply update the existing record if found, otherwise insert a new one.					
    MERGE map.tblCompanyToUserGroup AS tgt
    USING (SELECT
      [CompanyToUserGroupKey],
      [CompanyKey],
      [CompanySKey],
      [SiteSKey],
      [GroupKey],
      TRIM([ID]) [ID],
      [CreatedBy],
      [CreatedDate],
      [UpdatedBy],
      [UpdatedDate],
      [IsRecordDeleted],
      [RecordUpdatedDate]
    FROM staging.tblCompanyToUserGroup
    WHERE CompanyToUserGroupKey IS NOT NULL
    AND IgnoreRecord = 0) AS src
    ON tgt.CompanyToUserGroupKey = src.CompanyToUserGroupKey
    WHEN NOT MATCHED AND src.IsRecordDeleted = 0 THEN
    INSERT ([CompanyToUserGroupKey], [CompanyKey], [CompanySKey], [SiteSKey], [UserGroupKey], [ID], [_DeletedFlag], [_RecordUpdatedDate])
    VALUES (src.[CompanyToUserGroupKey], src.[CompanyKey], src.[CompanySKey], src.[SiteSKey], src.[GroupKey], src.[ID], src.[IsRecordDeleted], src.[RecordUpdatedDate])
    WHEN MATCHED AND src.RecordUpdatedDate > tgt._RecordUpdatedDate THEN
    UPDATE SET tgt.[CompanyKey] = src.[CompanyKey],
    tgt.[CompanySKey] = src.[CompanySKey],
    tgt.[SiteSKey] = src.[SiteSKey],
    tgt.[UserGroupKey] = src.[GroupKey],
    tgt.[ID] = src.[ID],
    tgt.[_DeletedFlag] = src.[IsRecordDeleted],
    tgt.[_RecordUpdatedDate] = src.[RecordUpdatedDate];


    --UserToUserGroup
    -- No historical data maintained for map.tblUserToUserGroup. Simply update the existing record if found, otherwise insert a new one.
    MERGE map.tblUserToUserGroup AS tgt
    USING (SELECT
      [UserSKey],
      [GroupKey],
      [SiteSKey],
      [UserToUserGroupKey],
      [CreatedBy],
      [CreatedDate],
      [UpdatedBy],
      [UpdatedDate],
      [IsRecordDeleted],
      [RecordUpdatedDate]
    FROM staging.tblUserToUserGroup
    WHERE UserSKey IS NOT NULL
    AND GroupKey IS NOT NULL
    AND SiteSKey IS NOT NULL
    AND IgnoreRecord = 0) AS src
    ON tgt.UserSKey = src.UserSKey AND tgt.SiteSKey = src.SiteSKey AND tgt.UserGroupKey = src.GroupKey
    WHEN NOT MATCHED AND ISNULL(src.IsRecordDeleted, 0) = 0 THEN
    INSERT ([UserSKey], [UserGroupKey], [SiteSKey], [UserToUserGroupKey], [_DeletedFlag], [_RecordUpdatedDate])
    VALUES (src.[UserSKey], src.[GroupKey], src.[SiteSKey], src.[UserToUserGroupKey], src.[IsRecordDeleted], src.[RecordUpdatedDate])
    WHEN MATCHED AND src.RecordUpdatedDate > tgt._RecordUpdatedDate THEN
    UPDATE SET tgt.[_DeletedFlag] = src.[IsRecordDeleted],
    tgt.[_RecordUpdatedDate] = src.[RecordUpdatedDate];


    -- LoadArm
    -- (DimLoadArm is a Level2 table because of its StationSKey reference, which is maintained to support a Site-Station-Arm hierarchy)
    MERGE dbo.DimLoadArm AS tgt
    USING (SELECT        
        [LoadArmKey],
        [BayAStationSKey] [StationSKey],
        ISNULL([BayAArmNumber], -1) ArmNumber,
        ISNULL([SwingArm], 0) SwingArm,
        [LoadRackText],
	    'BayA' [BayId],
	    [IsRecordDeleted],
        [CombinedUpdatedDate]
    FROM staging.tblLoadArms
    WHERE BayAStationKey IS NOT NULL
    AND LoadArmKey IS NOT NULL
    AND IgnoreRecord = 0) AS src
    ON tgt.StationSKey = src.StationSKey AND tgt.ArmNumber = src.ArmNumber
    WHEN NOT MATCHED THEN
    INSERT ([AKey], [StationSKey], [ArmNumber], [SwingArm], [LoadRackText], [BayId], [_DeletedFlag], [_RecordUpdatedDate])
    VALUES (src.[LoadArmKey], src.[StationSKey], src.[ArmNumber], src.[SwingArm], src.[LoadRackText], src.[BayId], src.[IsRecordDeleted], src.[CombinedUpdatedDate])
    WHEN MATCHED AND src.CombinedUpdatedDate > tgt._RecordUpdatedDate THEN
    UPDATE SET tgt.[SwingArm] = src.[SwingArm], 
        tgt.[LoadRackText] = src.[LoadRackText], 
        tgt.[BayId] = src.[BayId],
        tgt.[_DeletedFlag] = src.[IsRecordDeleted],
        tgt.[_RecordUpdatedDate] = src.[CombinedUpdatedDate];

    MERGE dbo.DimLoadArm AS tgt
    USING (SELECT        
        [LoadArmKey],
        [BayBStationSKey] [StationSKey],
        ISNULL([BayBArmNumber], -1) ArmNumber,
        ISNULL([SwingArm], 0) SwingArm,
        [LoadRackText],
	    'BayB' [BayId],
	    [IsRecordDeleted],
        [CombinedUpdatedDate]
    FROM staging.tblLoadArms
    WHERE BayBStationKey IS NOT NULL
    AND LoadArmKey IS NOT NULL
    AND IgnoreRecord = 0) AS src
    ON tgt.StationSKey = src.StationSKey AND tgt.ArmNumber = src.ArmNumber
    WHEN NOT MATCHED THEN
    INSERT ([AKey], [StationSKey], [ArmNumber], [SwingArm], [LoadRackText], [BayId], [_DeletedFlag], [_RecordUpdatedDate])
    VALUES (src.[LoadArmKey], src.[StationSKey], src.[ArmNumber], src.[SwingArm], src.[LoadRackText], src.[BayId], src.[IsRecordDeleted], src.[CombinedUpdatedDate])
    WHEN MATCHED AND src.CombinedUpdatedDate > tgt._RecordUpdatedDate THEN
    UPDATE SET tgt.[SwingArm] = src.[SwingArm], 
        tgt.[LoadRackText] = src.[LoadRackText], 
        tgt.[BayId] = src.[BayId],
        tgt.[_DeletedFlag] = src.[IsRecordDeleted],
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
    + 'Procedure Name: [staging].[usp_LoadLevel2Tables]' + CHAR(13) + CHAR(10)
    + 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS varchar(20)), '') + CHAR(13) + CHAR(10);
    RAISERROR (@_ErrMessage, 16, 1);
  END CATCH

END
GO
