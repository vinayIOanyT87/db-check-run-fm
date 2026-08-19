/*
	DROP PROCEDURE [staging].[usp_IgnoreDimensionIntermediateRecords]

	EXEC [staging].[usp_IgnoreDimensionIntermediateRecords]
	
*/
CREATE PROCEDURE [staging].[usp_IgnoreDimensionIntermediateRecords]
AS
BEGIN
  ------------------------------------------------------------------------------------------------------
  -- Stored procedure: [staging].[usp_IgnoreDimensionIntermediateRecords]
  -- Author: Hansraj Bapoo
  -- Version/Date: 1.0.003 / 2012-07-13 14:21:10.4470770 -04:00
  -- Purpose: Sets the IgnoreRecord flag of each intermediate record that correspond to an OLAP dimension for which historical data 
  --          is not maintained.
  -- Notes:
  -- 1. Intermediate records are records other than the latest record change captured for a given entity record. An entity record can change 
  --    multiple times in between ETL runs. The fmcdc captures all the changes, not just the latest change. For non-historical tables, the 
  --    OLAP system is only interested in the latest change for each entity record, so this procedure sets the IgnoreRecord flag of all those 
  --    intermediate records.
  -- 2. This procedure is limited to non-historical OLAP tables, i.e. tables without a StartDate-EndDate field pair.
  -- 3. Intermediate records can only be introduced by the Change Data Capture (fmcdc) tables (not from the source tables) OR from being 
  --    artificially added by the ETL to help provide missing segments of an object that is constructed from multiple tables/segments 
  --    (e.g. FactTransaction).
  --    When introduced by fmcdc, the RecordUpdateDate of intermediate records are always set (non-null).
  --    When introduced by artificial addition by the ETL, the IsRecordAddedByETL = 1.
  --	  A record is only artificially added by the ETL if not already found in the staging tables, i.e. there will not be cases where a given 
  --    record exhibit multiple entries as a result of both conditions, fmcdc entries and artificial ETL entries.
  -- 4. The determination of the latest version captured for each record is performed by the order in which the record was captured, i.e. the 
  --    CDCSKey. Determining the lastest record version based on RecordUpdatedDate does not always work, as it can generate more than one record,
  --    e.g. in the case of tblUsers, where as the InactivityLockout flag is set, a trigger is fired  to update the InactivityLockoutDate 
  --    on the SAME record. This type of trigger-based successive updates to the same record was found to generate CDC records with the same 
  --    RecordUpdatedDate.
  ------------------------------------------------------------------------------------------------------------------
  SET NOCOUNT ON;
  BEGIN TRY

    -- ApplicationString
    UPDATE a
    SET a.IgnoreRecord = 1
    FROM staging.tblApplicationString a
    WHERE a.RecordUpdatedDate IS NOT NULL
    AND a.CDCSKey IS NOT NULL
    AND NOT EXISTS 
    (
        SELECT * FROM staging.tblApplicationString b
        INNER JOIN 
        (
            SELECT ApplicationStringKey, MAX(CDCSKey) [CDCSKey]
            FROM staging.tblApplicationString
            WHERE IgnoreRecord = 0
            GROUP BY ApplicationStringKey
        ) c
        ON c.ApplicationStringKey = b.ApplicationStringKey
        AND c.CDCSKey = b.CDCSKey
        WHERE b.SKey = a.SKey
    )


  -- AutoDistributionReasonCodes
    UPDATE a
    SET a.IgnoreRecord = 1
    FROM staging.tblAutoDistributionReasonCodes a
    WHERE a.RecordUpdatedDate IS NOT NULL
    AND a.CDCSKey IS NOT NULL
    AND NOT EXISTS 
    (
        SELECT * FROM staging.tblAutoDistributionReasonCodes b
        INNER JOIN 
        (
            SELECT AutoDistributionReasonCodeKey, MAX(CDCSKey) [CDCSKey]
            FROM staging.tblAutoDistributionReasonCodes
            WHERE IgnoreRecord = 0
            GROUP BY AutoDistributionReasonCodeKey
        ) c
        ON c.AutoDistributionReasonCodeKey = b.AutoDistributionReasonCodeKey
        AND c.CDCSKey = b.CDCSKey
        WHERE b.SKey = a.SKey
    )

    
    -- Station
    UPDATE a
    SET a.IgnoreRecord = 1
    FROM staging.tblStations a
    WHERE a.RecordUpdatedDate IS NOT NULL
    AND a.CDCSKey IS NOT NULL
    AND NOT EXISTS 
    (
        SELECT * FROM staging.tblStations b
        INNER JOIN 
        (
            SELECT StationKey, MAX(CDCSKey) [CDCSKey]
            FROM staging.tblStations
            WHERE IgnoreRecord = 0
            GROUP BY StationKey
        ) c
        ON c.StationKey = b.StationKey
        AND c.CDCSKey = b.CDCSKey
        WHERE b.SKey = a.SKey
    )


    -- LoadArm
    UPDATE a
    SET a.IgnoreRecord = 1
    FROM staging.tblLoadArms a
    WHERE a.RecordUpdatedDate IS NOT NULL
    AND a.CDCSKey IS NOT NULL
    AND NOT EXISTS 
    (
        SELECT * FROM staging.tblLoadArms b
        INNER JOIN 
        (
            SELECT LoadArmKey, MAX(CDCSKey) [CDCSKey]
            FROM staging.tblLoadArms
            WHERE IgnoreRecord = 0
            GROUP BY LoadArmKey
        ) c
        ON c.LoadArmKey = b.LoadArmKey
        AND c.CDCSKey = b.CDCSKey
        WHERE b.SKey = a.SKey
    )


    -- Tank
    UPDATE a
    SET a.IgnoreRecord = 1
    FROM staging.tblTanks a
    WHERE a.RecordUpdatedDate IS NOT NULL
    AND a.CDCSKey IS NOT NULL
    AND NOT EXISTS 
    (
        SELECT * FROM staging.tblTanks b
        INNER JOIN 
        (
            SELECT TankKey, MAX(CDCSKey) [CDCSKey]
            FROM staging.tblTanks
            WHERE IgnoreRecord = 0
            GROUP BY TankKey
        ) c
        ON c.TankKey = b.TankKey
        AND c.CDCSKey = b.CDCSKey
        WHERE b.SKey = a.SKey
    )


    -- Site
    UPDATE a
    SET a.IgnoreRecord = 1
    FROM staging.tblSites a
    WHERE a.RecordUpdatedDate IS NOT NULL
    AND a.CDCSKey IS NOT NULL
    AND NOT EXISTS 
    (
        SELECT * FROM staging.tblSites b
        INNER JOIN 
        (
            SELECT SiteKey, MAX(CDCSKey) [CDCSKey]
            FROM staging.tblSites
            WHERE IgnoreRecord = 0
            GROUP BY SiteKey
        ) c
        ON c.SiteKey = b.SiteKey
        AND c.CDCSKey = b.CDCSKey
        WHERE b.SKey = a.SKey
    )


    -- SitetoSite
    -- Note: In FuelsManager 8.0SP4 table dbo.tblSiteToSiteMap does not have a separate primary key (e.g. SiteToSiteIndex), but uses a compound key. This requires the code below to use the compound key as well.
    UPDATE a
    SET a.IgnoreRecord = 1
    FROM staging.tblSiteToSite a
    WHERE a.RecordUpdatedDate IS NOT NULL
    AND a.CDCSKey IS NOT NULL
    AND NOT EXISTS 
    (
        SELECT * FROM staging.tblSiteToSite b
        INNER JOIN 
        (
            SELECT ParentSiteKey, ChildSiteKey, MAX(CDCSKey) [CDCSKey]
            FROM staging.tblSiteToSite
            WHERE IgnoreRecord = 0
            GROUP BY ParentSiteKey, ChildSiteKey
        ) c
        ON c.ParentSiteKey = b.ParentSiteKey
        AND c.ChildSiteKey = b.ChildSiteKey
        AND c.CDCSKey = b.CDCSKey
        WHERE b.SKey = a.SKey
    )

    
    -- ApplicationString
    UPDATE a
    SET a.IgnoreRecord = 1
    FROM staging.tblApplicationString a
    WHERE a.RecordUpdatedDate IS NOT NULL
    AND a.CDCSKey IS NOT NULL
    AND NOT EXISTS 
    (
        SELECT * FROM staging.tblApplicationString b
        INNER JOIN 
        (
            SELECT ApplicationStringKey, MAX(CDCSKey) [CDCSKey]
            FROM staging.tblApplicationString
            WHERE IgnoreRecord = 0
            GROUP BY ApplicationStringKey
        ) c
        ON c.ApplicationStringKey = b.ApplicationStringKey
        AND c.CDCSKey = b.CDCSKey
        WHERE b.SKey = a.SKey
    )
  

    -- User
    UPDATE a
    SET a.IgnoreRecord = 1
    FROM staging.tblUsers a
    WHERE a.RecordUpdatedDate IS NOT NULL
    AND a.CDCSKey IS NOT NULL
    AND NOT EXISTS 
    (
        SELECT * FROM staging.tblUsers b
        INNER JOIN 
        (
            SELECT UserKey, MAX(CDCSKey) [CDCSKey]
            FROM staging.tblUsers
            WHERE IgnoreRecord = 0
            GROUP BY UserKey
        ) c
        ON c.UserKey = b.UserKey
        AND c.CDCSKey = b.CDCSKey
        WHERE b.SKey = a.SKey
    )


    -- CompanyToUserGroup
    UPDATE a
    SET a.IgnoreRecord = 1
    FROM staging.tblCompanyToUserGroup a
    WHERE a.RecordUpdatedDate IS NOT NULL
    AND a.CDCSKey IS NOT NULL
    AND NOT EXISTS 
    (
        SELECT * FROM staging.tblCompanyToUserGroup b
        INNER JOIN 
        (
            SELECT CompanyToUserGroupKey, MAX(CDCSKey) [CDCSKey] FROM staging.tblCompanyToUserGroup
            WHERE IgnoreRecord = 0
            GROUP BY CompanyToUserGroupKey
        ) c
        ON c.CompanyToUserGroupKey = b.CompanyToUserGroupKey
        AND c.CDCSKey = b.CDCSKey
        WHERE b.SKey = a.SKey
    )

    -- UserToUserGroup
    UPDATE a
    SET a.IgnoreRecord = 1
    FROM staging.tblUserToUserGroup a
    WHERE a.RecordUpdatedDate IS NOT NULL
    AND a.CDCSKey IS NOT NULL
    AND NOT EXISTS 
    (
        SELECT * FROM staging.tblUserToUserGroup b
        INNER JOIN 
        (
            SELECT GroupKey, UserKey, MAX(CDCSKey) [CDCSKey]
            FROM staging.tblUserToUserGroup
            WHERE IgnoreRecord = 0
            GROUP BY GroupKey, UserKey
        ) c
        ON c.GroupKey = b.GroupKey
        AND c.UserKey = b.UserKey
        AND c.CDCSKey = b.CDCSKey
        WHERE b.SKey = a.SKey
    )

    -- UserToSite
    UPDATE a
    SET a.IgnoreRecord = 1
    FROM staging.tblEntityUserToSite a
    WHERE a.RecordUpdatedDate IS NOT NULL
    AND a.CDCSKey IS NOT NULL
    AND NOT EXISTS 
    (
        SELECT * FROM staging.tblEntityUserToSite b
        INNER JOIN 
        (
            SELECT UserKey, SiteKey, MAX(CDCSKey) [CDCSKey]
            FROM staging.tblEntityUserToSite
            WHERE IgnoreRecord = 0
            GROUP BY UserKey, SiteKey
        ) c
        ON c.UserKey = b.UserKey
        AND c.SiteKey = b.SiteKey
        AND c.CDCSKey = b.CDCSKey
        WHERE b.SKey = a.SKey
    )

    -- CompanyToSite
    UPDATE a
    SET a.IgnoreRecord = 1
    FROM staging.tblEntityCompanyToSite a
    WHERE a.RecordUpdatedDate IS NOT NULL
    AND a.CDCSKey IS NOT NULL
    AND NOT EXISTS 
    (
        SELECT * FROM staging.tblEntityCompanyToSite b
        INNER JOIN 
        (
            SELECT CompanyKey, SiteKey, MAX(CDCSKey) [CDCSKey]
            FROM staging.tblEntityCompanyToSite
            WHERE IgnoreRecord = 0
            GROUP BY CompanyKey, SiteKey
        ) c
        ON c.CompanyKey = b.CompanyKey
        AND c.SiteKey = b.SiteKey
        AND c.CDCSKey = b.CDCSKey
        WHERE b.SKey = a.SKey
    )


    -- EquipmentTypes
	UPDATE a SET a.IgnoreRecord = 1 
	FROM staging.tblEquipmentTypes a
	WHERE a.RecordUpdatedDate IS NOT NULL
	AND a.CDCSKey IS NOT NULL
	AND NOT EXISTS
	(
		SELECT * FROM staging.tblEquipmentTypes b
		INNER JOIN
		(
			SELECT EquipmentTypeKey, MAX(CDCSKey) [CDCSKey] FROM staging.tblEquipmentTypes 
			WHERE IgnoreRecord = 0
			GROUP BY EquipmentTypeKey
		) c
		ON c.EquipmentTypeKey = b.EquipmentTypeKey
		AND c.CDCSKey = b.CDCSKey
		WHERE b.SKey = a.SKey								
	)

    -- EquipmentToSite
    UPDATE a
    SET a.IgnoreRecord = 1
    FROM staging.tblEntityEquipmentToSite a
    WHERE a.RecordUpdatedDate IS NOT NULL
    AND a.CDCSKey IS NOT NULL
    AND NOT EXISTS 
    (
        SELECT * FROM staging.tblEntityEquipmentToSite b
        INNER JOIN 
        (
            SELECT EquipmentKey, SiteKey, MAX(CDCSKey) [CDCSKey]
            FROM staging.tblEntityEquipmentToSite
            WHERE IgnoreRecord = 0
            GROUP BY EquipmentKey, SiteKey
        ) c
        ON c.EquipmentKey = b.EquipmentKey
        AND c.SiteKey = b.SiteKey
        AND c.CDCSKey = b.CDCSKey
        WHERE b.SKey = a.SKey
    )

    -- ProductToSite
    UPDATE a
    SET a.IgnoreRecord = 1
    FROM staging.tblEntityProductToSite a
    WHERE a.RecordUpdatedDate IS NOT NULL
    AND a.CDCSKey IS NOT NULL
    AND NOT EXISTS 
    (
        SELECT * FROM staging.tblEntityProductToSite b
        INNER JOIN 
        (
            SELECT ProductKey, SiteKey, MAX(CDCSKey) [CDCSKey]
            FROM staging.tblEntityProductToSite
            WHERE IgnoreRecord = 0
            GROUP BY ProductKey, SiteKey
        ) c
        ON c.ProductKey = b.ProductKey
        AND c.SiteKey = b.SiteKey
        AND c.CDCSKey = b.CDCSKey
        WHERE b.SKey = a.SKey
    )


    --PersonnelToSite
    UPDATE a
    SET a.IgnoreRecord = 1
    FROM staging.tblEntityPersonnelToSite a
    WHERE a.RecordUpdatedDate IS NOT NULL
    AND a.CDCSKey IS NOT NULL
    AND NOT EXISTS 
    (
        SELECT * FROM staging.tblEntityPersonnelToSite b
        INNER JOIN 
        (
            SELECT PersonnelKey, SiteKey, MAX(CDCSKey) [CDCSKey]
            FROM staging.tblEntityPersonnelToSite
            WHERE IgnoreRecord = 0
            GROUP BY PersonnelKey, SiteKey
        ) c
        ON c.PersonnelKey = b.PersonnelKey
        AND c.SiteKey = b.SiteKey
        AND c.CDCSKey = b.CDCSKey
        WHERE b.SKey = a.SKey
    )

	-- TransactionAliasToSite
	UPDATE a
    SET a.IgnoreRecord = 1
    FROM staging.tblEntityTransactionAliasToSite a
    WHERE a.RecordUpdatedDate IS NOT NULL
    AND a.CDCSKey IS NOT NULL
    AND NOT EXISTS 
    (
        SELECT * FROM staging.tblEntityTransactionAliasToSite b
        INNER JOIN 
        (
            SELECT TransactionAliasKey, SiteKey, MAX(CDCSKey) [CDCSKey]
            FROM staging.tblEntityTransactionAliasToSite
            WHERE IgnoreRecord = 0
            GROUP BY TransactionAliasKey, SiteKey
        ) c
        ON c.TransactionAliasKey = b.TransactionAliasKey
        AND c.SiteKey = b.SiteKey
        AND c.CDCSKey = b.CDCSKey
        WHERE b.SKey = a.SKey
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
    + 'Procedure Name: [staging].[usp_IgnoreDimensionIntermediateRecords]' + CHAR(13) + CHAR(10)
    + 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS varchar(20)), '') + CHAR(13) + CHAR(10);
    RAISERROR (@_ErrMessage, 16, 1);
  END CATCH

END