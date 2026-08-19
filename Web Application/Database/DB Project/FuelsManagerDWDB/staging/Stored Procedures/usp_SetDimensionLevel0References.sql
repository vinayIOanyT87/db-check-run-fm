/*
    DROP PROCEDURE [staging].[usp_SetDimensionLevel0References]

	EXEC [staging].[usp_SetDimensionLevel0References]
	
*/
CREATE PROCEDURE [staging].[usp_SetDimensionLevel0References]
AS
BEGIN
  ------------------------------------------------------------------------------------------------------
  -- Stored procedure: [staging].[usp_SetDimensionLevel0References]
  -- Author: Hansraj Bapoo
  -- Version/Date: 1.0.003 / 2012-07-13 14:21:10.4470770 -04:00
  -- Purpose: Sets, in staging, the Dimension fields that reflect foreign key references to all Level 0 tables, e.g. set the staging.tblProducts.SiteSKey field to point to the correct dimSites record.
  -- Notes:
  -- 1. The foreign keys are maintained in the OLAP database tables, not in the staging tables, but in order for the staging tables to be properly loaded into the 
  --    OLAP tables, the fields in the staging tables that reflect those OLAP table foreign keys have to be preset correctly.
  -- 2. For references to historical tables, the foreign key is determined by a combination of the Identity Key (e.g. ProductKey, i.e. ProductKey or ProductIndex) and the StartDate-EndDate range.
  -- 3. For references to non-historical tables, the foreign key is determined solely on the Identity Key.
  ------------------------------------------------------------------------------------------------------
  SET NOCOUNT ON;
  BEGIN TRY

    -- Site-to-Site references
    UPDATE a
    SET a.ParentSiteSKey = b.SKey,
        a.ChildSiteSKey = c.SKey
    FROM staging.tblSiteToSite a
    INNER JOIN dbo.DimSite b
    ON b.AKey = a.ParentSiteKey
    INNER JOIN dbo.DimSite c
    ON c.AKey = a.ChildSiteKey
    WHERE a.IgnoreRecord = 0

    IF 
    (
        (
            (
                SELECT COUNT(*) FROM staging.tblSiteToSite
                WHERE ParentSiteKey IS NOT NULL
                AND ParentSiteSKey IS NULL
                AND IgnoreRecord = 0
            ) > 0
        )
        OR 
        (
            (
                SELECT COUNT(*) FROM staging.tblSiteToSite
                WHERE ChildSiteKey IS NOT NULL
                AND ChildSiteSKey IS NULL
                AND IgnoreRecord = 0
            )  > 0
        )
    )
    BEGIN
      RAISERROR ('Failure to resolve Site-to-Site references', 16, 1);
      RETURN;
    END


    -- AutoDistributionReasonCodes-to-Site references
    UPDATE a
    SET a.SiteSKey = b.SKey
    FROM staging.tblAutoDistributionReasonCodes a
    INNER JOIN dbo.DimSite b
    ON b.AKey = a.SiteKey
    WHERE a.IgnoreRecord = 0
    --AND a.StartDate BETWEEN b.StartDate AND ISNULL(b.EndDate, @openEndedDate)

    IF ((SELECT COUNT(*)
      FROM staging.tblAutoDistributionReasonCodes
      WHERE SiteKey IS NOT NULL
      AND SiteSKey IS NULL
      AND IgnoreRecord = 0)
      > 0)
    BEGIN
      RAISERROR ('Failure to resolve AutoDistributionReasonCode-to-Site references', 16, 1);
      RETURN;
    END


    -- Stations-to-Site references
    UPDATE a
    SET a.SiteSKey = b.SKey
    FROM staging.tblStations a
    INNER JOIN dbo.DimSite b
    ON b.AKey = a.SiteKey
    WHERE a.IgnoreRecord = 0

    IF ((SELECT COUNT(*)
      FROM staging.tblStations
      WHERE SiteKey IS NOT NULL
      AND SiteSKey IS NULL
      AND IgnoreRecord = 0)
      > 0)
    BEGIN
      RAISERROR ('Failure to resolve Station-to-Site references', 16, 1);
      RETURN;
    END


    -- Tanks-to-Site references
    UPDATE a
    SET a.SiteSKey = b.SKey
    FROM staging.tblTanks a
    INNER JOIN dbo.DimSite b
    ON b.AKey = a.SiteKey
    WHERE a.IgnoreRecord = 0

    IF ((SELECT COUNT(*)
      FROM staging.tblTanks
      WHERE SiteKey IS NOT NULL
      AND SiteSKey IS NULL
      AND IgnoreRecord = 0)
      > 0)
    BEGIN
      RAISERROR ('Failure to resolve Tank-to-Site references', 16, 1);
      RETURN;
    END


    -- Product-to-Site references
    UPDATE a
    SET a.SiteSKey = b.SKey
    FROM staging.tblProducts a
    INNER JOIN dbo.DimSite b
    ON b.AKey = a.SiteKey
    WHERE a.IgnoreRecord = 0
    --AND a.StartDate BETWEEN b.StartDate AND ISNULL(b.EndDate, @openEndedDate)

    IF ((SELECT COUNT(*)
      FROM staging.tblProducts
      WHERE SiteKey IS NOT NULL
      AND SiteSKey IS NULL
      AND IgnoreRecord = 0)
      > 0)
    BEGIN
      RAISERROR ('Failure to resolve Product-to-Site references', 16, 1);
      RETURN;
    END


    -- ApplicationString-to-Site references
    UPDATE a
    SET a.SiteSKey = b.SKey
    FROM staging.tblApplicationString a
    INNER JOIN dbo.DimSite b
    ON b.AKey = a.SiteKey
    WHERE a.IgnoreRecord = 0

    IF ((SELECT COUNT(*)
      FROM staging.tblApplicationString
      WHERE SiteKey IS NOT NULL
      AND SiteSKey IS NULL
      AND IgnoreRecord = 0)
      > 0)
    BEGIN
      RAISERROR ('Failure to resolve ApplicationString-to-Site references', 16, 1);
      RETURN;
    END
    

    -- Company-to-Site references
    UPDATE a
    SET a.SiteSKey = b.SKey
    FROM staging.tblCompanies a
    INNER JOIN dbo.DimSite b
    ON b.AKey = a.SiteKey
    WHERE a.IgnoreRecord = 0

    IF ((SELECT COUNT(*)
      FROM staging.tblCompanies
      WHERE SiteKey IS NOT NULL
      AND SiteSKey IS NULL
      AND IgnoreRecord = 0
      AND IsRecordDeleted = 0)
      > 0)
    BEGIN
      RAISERROR ('Failure to resolve Company-to-Site references', 16, 1);
      RETURN;
    END


    -- Equipment-to-Site references
    UPDATE a
    SET a.SiteSKey = b.SKey
    FROM staging.tblEquipment a
    INNER JOIN dbo.DimSite b
    ON b.AKey = a.SiteKey
    WHERE a.IgnoreRecord = 0
    --AND a.StartDate BETWEEN b.StartDate AND ISNULL(b.EndDate, @openEndedDate)

    IF ((SELECT COUNT(*)
      FROM staging.tblEquipment
      WHERE SiteKey IS NOT NULL
      AND SiteSKey IS NULL
      AND IgnoreRecord = 0)
      > 0)
    BEGIN
      RAISERROR ('Failure to resolve Equipment-to-Site references', 16, 1);
      RETURN;
    END


    -- Personnel-to-Site references
    UPDATE a
    SET a.SiteSKey = b.SKey
    FROM staging.tblPersonnel a
    INNER JOIN dbo.DimSite b
    ON b.AKey = a.SiteKey
    WHERE a.IgnoreRecord = 0
    --AND a.StartDate BETWEEN b.StartDate AND ISNULL(b.EndDate, @openEndedDate)

    IF ((SELECT COUNT(*)
      FROM staging.tblPersonnel
      WHERE SiteKey IS NOT NULL
      AND SiteSKey IS NULL
      AND IgnoreRecord = 0)
      > 0)
    BEGIN
      RAISERROR ('Failure to resolve Personnel-to-Site references', 16, 1);
      RETURN;
    END


    -- TransactionAlias-to-Site references
    UPDATE a
    SET a.SiteSKey = b.SKey
    FROM staging.tblTransactionAliases a
    INNER JOIN dbo.DimSite b
    ON b.AKey = a.SiteKey
    WHERE a.IgnoreRecord = 0
    --AND a.StartDate BETWEEN b.StartDate AND ISNULL(b.EndDate, @openEndedDate)

    IF ((SELECT COUNT(*)
      FROM staging.tblTransactionAliases
      WHERE SiteKey IS NOT NULL
      AND SiteSKey IS NULL
      AND IgnoreRecord = 0)
      > 0)
    BEGIN
      RAISERROR ('Failure to resolve TransactionAlias-to-Site references', 16, 1);
      RETURN;
    END


    -- Equipment-to-EquipmentType references
    UPDATE a
    SET a.EquipmentTypeSKey = b.SKey
    FROM staging.tblEquipment a
    INNER JOIN dbo.DimEquipmentType b
      ON b.AKey = a.EquipmentTypeKey
    WHERE a.IgnoreRecord = 0
    --AND a.StartDate BETWEEN b.StartDate AND ISNULL(b.EndDate, @openEndedDate)

    IF ((SELECT COUNT(*)
      FROM staging.tblEquipment
      WHERE EquipmentTypeKey IS NOT NULL
      AND EquipmentTypeSKey IS NULL
      AND IgnoreRecord = 0)
      > 0)
    BEGIN
      RAISERROR ('Failure to resolve Equipment-to-EquipmentType references', 16, 1);
      RETURN;
    END


    -- TransactionAlias-to-TransactionType references
    UPDATE a
    SET a.TransactionTypeSKey = b.SKey
    FROM staging.tblTransactionAliases a
    INNER JOIN dbo.DimTransactionType b
    ON b.AKey = Convert(nvarchar(50), a.LookupTransTypeIndex)   --AKeyz
    WHERE a.IgnoreRecord = 0
    AND b.SKey > 0
    --AND a.StartDate BETWEEN b.StartDate AND ISNULL(b.EndDate, @openEndedDate)

    IF ((SELECT COUNT(*)
      FROM staging.tblTransactionAliases
      WHERE LookupTransTypeIndex IS NOT NULL
      AND TransactionTypeSKey IS NULL
      AND IgnoreRecord = 0)
      > 0)
    BEGIN
      RAISERROR ('Failure to resolve TransactionAlias-to-TransactionType references', 16, 1);
      RETURN;
    END


    /*
    -- FuelCard-to-Site references
    UPDATE a
    SET a.SiteSKey = b.SKey
    FROM staging.tblFuelCards a
    INNER JOIN dimSite b
      ON b.AKey = a.SiteKey
    WHERE a.IgnoreRecord = 0
    --AND a.StartDate BETWEEN b.StartDate AND ISNULL(b.EndDate, @openEndedDate)

    IF ((SELECT
        COUNT(*)
      FROM staging.tblFuelCards
      WHERE SiteKey IS NOT NULL
      AND SiteSKey IS NULL
      AND IgnoreRecord = 0)
      > 0)
    BEGIN
      RAISERROR ('Failure to resolve FuelCard-to-Site references', 16, 1);
      RETURN;
    END
    */

    -- CompanyToUserGroup-to-Site references
    UPDATE a
    SET a.SiteSKey = b.SKey
    FROM staging.tblCompanyToUserGroup a
    INNER JOIN dbo.DimSite b
    ON b.AKey = a.SiteKey
    WHERE a.IgnoreRecord = 0
    --AND a.StartDate BETWEEN b.StartDate AND ISNULL(b.EndDate, @openEndedDate)

    IF ((SELECT COUNT(*)
      FROM staging.tblCompanyToUserGroup
      WHERE SiteKey IS NOT NULL
      AND SiteSKey IS NULL
      AND IgnoreRecord = 0
      AND IsRecordDeleted = 0)
      > 0)
    BEGIN
      RAISERROR ('Failure to resolve CompanyToUserGroup-to-Site references', 16, 1);
      RETURN;
    END


    -- UserToUserGroup-to-Site references
    UPDATE a
    SET a.SiteSKey = b.SKey
    FROM staging.tblUserToUserGroup a
    INNER JOIN dbo.DimSite b
    ON b.AKey = a.SiteKey
    WHERE a.IgnoreRecord = 0
    --AND a.StartDate BETWEEN b.StartDate AND ISNULL(b.EndDate, @openEndedDate)

    IF ((SELECT COUNT(*)
      FROM staging.tblUserToUserGroup
      WHERE SiteKey IS NOT NULL
      AND SiteSKey IS NULL
      AND IgnoreRecord = 0
      AND IsRecordDeleted = 0)
      > 0)
    BEGIN
      RAISERROR ('Failure to resolve UserToUserGroup-to-Site references', 16, 1);
      RETURN;
    END


    -- UserToUserGroup-to-User references
    UPDATE a
    SET a.UserSKey = b.SKey
    FROM staging.tblUserToUserGroup a
    INNER JOIN dbo.DimFMUser b
    ON b.AKey = a.UserKey
    WHERE a.IgnoreRecord = 0
    --AND a.StartDate BETWEEN b.StartDate AND ISNULL(b.EndDate, @openEndedDate)		

    IF ((SELECT COUNT(*)
      FROM staging.tblUserToUserGroup
      WHERE UserKey IS NOT NULL
      AND UserSKey IS NULL
      AND IgnoreRecord = 0
      AND IsRecordDeleted = 0)
      > 0)
    BEGIN
      RAISERROR ('Failure to resolve UserToUserGroup-to-User references', 16, 1);
      RETURN;
    END


    -- UserToSite-to-Site references
    UPDATE a
    SET a.SiteSKey = b.SKey
    FROM staging.tblEntityUserToSite a
    INNER JOIN dbo.DimSite b
      ON b.AKey = a.SiteKey
    WHERE a.IgnoreRecord = 0
    --AND a.StartDate BETWEEN b.StartDate AND ISNULL(b.EndDate, @openEndedDate)

    IF ((SELECT COUNT(*)
      FROM staging.tblEntityUserToSite
      WHERE SiteKey IS NOT NULL
      AND SiteSKey IS NULL
      AND IgnoreRecord = 0
      AND IsRecordDeleted = 0)
      > 0)
    BEGIN
      RAISERROR ('Failure to resolve UserToSite-to-Site references', 16, 1);
      RETURN;
    END


    -- UserToSite-to-User references
    UPDATE a
    SET a.UserSKey = b.SKey
    FROM staging.tblEntityUserToSite a
    INNER JOIN dbo.DimFMUser b
    ON b.AKey = a.UserKey
    WHERE a.IgnoreRecord = 0
    --AND a.StartDate BETWEEN b.StartDate AND ISNULL(b.EndDate, @openEndedDate)		

    IF ((SELECT COUNT(*)
      FROM staging.tblEntityUserToSite
      WHERE UserKey IS NOT NULL
      AND UserSKey IS NULL
      AND IgnoreRecord = 0
      AND IsRecordDeleted = 0)
      > 0)
    BEGIN
      RAISERROR ('Failure to resolve UserToSite-to-User references', 16, 1);
      RETURN;
    END


    -- EntityCompanyToSite-to-Site references
    UPDATE a
    SET a.SiteSKey = b.SKey
    FROM staging.tblEntityCompanyToSite a
    INNER JOIN dbo.DimSite b
    ON b.AKey = a.SiteKey
    WHERE a.IgnoreRecord = 0
    --AND a.StartDate BETWEEN b.StartDate AND ISNULL(b.EndDate, @openEndedDate)		

    UPDATE a
    SET a.AssignedFromSiteSKey = b.SKey
    FROM staging.tblEntityCompanyToSite a
    INNER JOIN dbo.DimSite b
    ON b.AKey = a.AssignedFromSiteKey
    WHERE a.IgnoreRecord = 0
    --AND a.StartDate BETWEEN b.StartDate AND ISNULL(b.EndDate, @openEndedDate)	

    IF ((SELECT COUNT(*)
      FROM staging.tblEntityCompanyToSite
      WHERE SiteKey IS NOT NULL
      AND SiteSKey IS NULL
      AND IgnoreRecord = 0
      AND IsRecordDeleted = 0)
      > 0
      OR (SELECT COUNT(*)
      FROM staging.tblEntityCompanyToSite
      WHERE AssignedFromSiteKey IS NOT NULL
      AND AssignedFromSiteSKey IS NULL
      AND IgnoreRecord = 0
      AND IsRecordDeleted = 0)
      > 0
      )
    BEGIN
      RAISERROR ('Failure to resolve EntityCompanyToSite-to-Site references', 16, 1);
      RETURN;
    END


    -- EntityEquipmentToSite-to-Site references
    UPDATE a
    SET a.SiteSKey = b.SKey
    FROM staging.tblEntityEquipmentToSite a
    INNER JOIN dbo.DimSite b
    ON b.AKey = a.SiteKey
    WHERE a.IgnoreRecord = 0
    --AND a.StartDate BETWEEN b.StartDate AND ISNULL(b.EndDate, @openEndedDate)		

    UPDATE a
    SET a.AssignedFromSiteSKey = b.SKey
    FROM staging.tblEntityEquipmentToSite a
    INNER JOIN dbo.DimSite b
    ON b.AKey = a.AssignedFromSiteKey
    WHERE a.IgnoreRecord = 0
    --AND a.StartDate BETWEEN b.StartDate AND ISNULL(b.EndDate, @openEndedDate)

    IF ((SELECT COUNT(*)
      FROM staging.tblEntityEquipmentToSite
      WHERE SiteKey IS NOT NULL
      AND SiteSKey IS NULL
      AND IgnoreRecord = 0
      AND IsRecordDeleted = 0)
      > 0
      OR (SELECT COUNT(*)
      FROM staging.tblEntityEquipmentToSite
      WHERE AssignedFromSiteKey IS NOT NULL
      AND AssignedFromSiteSKey IS NULL
      AND IgnoreRecord = 0
      AND IsRecordDeleted = 0)
      > 0
      )
    BEGIN
      RAISERROR ('Failure to resolve EntityEquipmentToSite-to-Site references', 16, 1);
      RETURN;
    END


    -- EntityProductToSite-to-Site references
    UPDATE a
    SET a.SiteSKey = b.SKey
    FROM staging.tblEntityProductToSite a
    INNER JOIN dbo.DimSite b
    ON b.AKey = a.SiteKey
    WHERE a.IgnoreRecord = 0
    --AND a.StartDate BETWEEN b.StartDate AND ISNULL(b.EndDate, @openEndedDate)	

    UPDATE a
    SET a.AssignedFromSiteSKey = b.SKey
    FROM staging.tblEntityProductToSite a
    INNER JOIN dbo.DimSite b
    ON b.AKey = a.AssignedFromSiteKey
    WHERE a.IgnoreRecord = 0
    --AND a.StartDate BETWEEN b.StartDate AND ISNULL(b.EndDate, @openEndedDate)	

    IF ((SELECT COUNT(*)
      FROM staging.tblEntityProductToSite
      WHERE SiteKey IS NOT NULL
      AND SiteSKey IS NULL
      AND IgnoreRecord = 0
      AND IsRecordDeleted = 0)
      > 0
      OR (SELECT COUNT(*)
      FROM staging.tblEntityProductToSite
      WHERE AssignedFromSiteKey IS NOT NULL
      AND AssignedFromSiteSKey IS NULL
      AND IgnoreRecord = 0
      AND IsRecordDeleted = 0)
      > 0
      )
    BEGIN
      RAISERROR ('Failure to resolve EntityProductToSite-to-Site references', 16, 1);
      RETURN;
    END


    -- EntityPersonnelToSite-to-Site references
    UPDATE a
    SET a.SiteSKey = b.SKey
    FROM staging.tblEntityPersonnelToSite a
    INNER JOIN dbo.DimSite b
    ON b.AKey = a.SiteKey
    WHERE a.IgnoreRecord = 0
    --AND a.StartDate BETWEEN b.StartDate AND ISNULL(b.EndDate, @openEndedDate)		

    UPDATE a
    SET a.AssignedFromSiteSKey = b.SKey
    FROM staging.tblEntityPersonnelToSite a
    INNER JOIN dbo.DimSite b
    ON b.AKey = a.AssignedFromSiteKey
    WHERE a.IgnoreRecord = 0
    --AND a.StartDate BETWEEN b.StartDate AND ISNULL(b.EndDate, @openEndedDate)	

    IF ((SELECT COUNT(*)
      FROM staging.tblEntityPersonnelToSite
      WHERE SiteKey IS NOT NULL
      AND SiteSKey IS NULL
      AND IgnoreRecord = 0
      AND IsRecordDeleted = 0)
      > 0
      OR (SELECT COUNT(*)
      FROM staging.tblEntityPersonnelToSite
      WHERE AssignedFromSiteKey IS NOT NULL
      AND AssignedFromSiteSKey IS NULL
      AND IgnoreRecord = 0
      AND IsRecordDeleted = 0)
      > 0
      )
    BEGIN
      RAISERROR ('Failure to resolve EntityPersonnelToSite-to-Site references', 16, 1);
      RETURN;
    END


    -- EntityTransactionAliasToSite-to-Site references
    UPDATE a
    SET a.SiteSKey = b.SKey
    FROM staging.tblEntityTransactionAliasToSite a
    INNER JOIN dbo.DimSite b
    ON b.AKey = a.SiteKey
    WHERE a.IgnoreRecord = 0
    --AND a.StartDate BETWEEN b.StartDate AND ISNULL(b.EndDate, @openEndedDate)		

    UPDATE a
    SET a.AssignedFromSiteSKey = b.SKey
    FROM staging.tblEntityTransactionAliasToSite a
    INNER JOIN dbo.DimSite b
    ON b.AKey = a.AssignedFromSiteKey
    WHERE a.IgnoreRecord = 0
    --AND a.StartDate BETWEEN b.StartDate AND ISNULL(b.EndDate, @openEndedDate)	

    IF ((SELECT COUNT(*)
      FROM staging.tblEntityTransactionAliasToSite
      WHERE SiteKey IS NOT NULL
      AND SiteSKey IS NULL
      AND IgnoreRecord = 0
      AND IsRecordDeleted = 0)
      > 0
      OR (SELECT COUNT(*)
      FROM staging.tblEntityTransactionAliasToSite
      WHERE AssignedFromSiteKey IS NOT NULL
      AND AssignedFromSiteSKey IS NULL
      AND IgnoreRecord = 0
      AND IsRecordDeleted = 0)
      > 0
      )
    BEGIN
      RAISERROR ('Failure to resolve EntityTransactionAliasToSite-to-Site references', 16, 1);
      RETURN;
    END


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
    + 'Procedure Name: [staging].[usp_SetDimensionLevel0References]' + CHAR(13) + CHAR(10)
    + 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS varchar(20)), '') + CHAR(13) + CHAR(10);
    RAISERROR (@_ErrMessage, 16, 1);
  END CATCH

END
GO
