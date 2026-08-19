/*
  DROP PROCEDURE [staging].[usp_LoadCompanyToUserMap]

	EXEC [staging].[usp_LoadCompanyToUserMap]
	
*/
CREATE PROCEDURE [staging].[usp_LoadCompanyToUserMap]
AS
BEGIN
  ------------------------------------------------------------------------------------------------------
  -- Stored procedure: [staging].[usp_LoadCompanyToUserMap]
  -- Author: Hansraj Bapoo
  -- Version/Date: 1.0.003 / 2012-07-13 14:21:10.4470770 -04:00
  -- Purpose: Loads table FactFMUserToCompany from a combination of the Company-To-UserGroup map and the User-to-UserGroup map.
  -- Notes:
  -- 1. For the Company-To-UserGroup map, a NULL CompanyKey (CompanyIndex = 0 in older FuelsManager databases), indicates ALL companies.
  --    The use of the All Companies means that even a newly added company should be mapped to the applicable user. This Stored 
  --    Procedure refreshes the Company-to-UserGroup mappings if there has been changes to any of those tables: (i) Company table 
  --    (ii) User-to-UserGroup table (iii) Company-to-UserGroup table.
  -- 2. For the derived Company-To-User map, the retrieval of the CompanySKey is not date-bound. A given CompanyAKey is mapped to all 
  --    available versions of that company records in DimCompany. This ensures that the Company-To-User map, which is used for user 
  --    security purposes, can help give access to the users to all versions of the applicable companies, and not just ot the active 
  --    versions.
  -- 3. Table FactFMUserToCompany map is completely re-built every time this Stored Procedure is run. The table does not track/flag 
  --    deleted mappings. The table is used for user security purposes, where we are only concerned in restricting user-company access 
  --    for a user based of his/her existing company mappings. Previous company mappings of a user are of no value.
  -- 4. All users are also mapped to the dummy Company (DimCompany.SKey = 0) to allow them access to measures where the Company 
  --    attributes have not been set.
  -- 5. This procedure should be executed after missing company records have been identified and re-created from transaction records.
  ------------------------------------------------------------------------------------------------------
  SET NOCOUNT ON;
  BEGIN TRY

    DECLARE @refreshRequired bit
    SET @refreshRequired = 0

    IF
      (
      ((SELECT
        COUNT(*)
      FROM staging.tblCompanies
      WHERE IgnoreRecord = 0)
      > 0)
      OR ((SELECT
        COUNT(*)
      FROM staging.tblUserToUserGroup
      WHERE IgnoreRecord = 0)
      > 0)
      OR ((SELECT
        COUNT(*)
      FROM staging.tblCompanyToUserGroup
      WHERE IgnoreRecord = 0)
      > 0)
      )
    BEGIN
      SET @refreshRequired = 1
    END

    IF (@refreshRequired = 1)
    BEGIN
      TRUNCATE TABLE dbo.FactFMUserToCompany

      INSERT INTO dbo.FactFMUserToCompany (CompanySKey, FMUserSKey)
        SELECT DISTINCT
          x.CompanySKey,
          y.UserSKey
        FROM 
        (
            SELECT
                a.CompanySKey,
                a.UserGroupKey
                FROM map.tblCompanyToUserGroup a
                WHERE a._DeletedFlag = 0
                AND a.CompanyKey IS NOT NULL

            UNION ALL
                SELECT
                c.SKey CompanySKey,
                a.UserGroupKey
                FROM map.tblCompanyToUserGroup a
                CROSS JOIN 
                (
                    SELECT * FROM DimCompany
                    WHERE SKey > 0
                ) c  -- not date-bound to map all versions of the company records
                WHERE a._DeletedFlag = 0
                AND a.CompanyKey IS NULL  -- A Null CompanyKey indicates All companies
        ) x
        INNER JOIN map.tblUserToUserGroup y
          ON y.UserGroupKey = x.UserGroupKey
        WHERE y._DeletedFlag = 0

      IF
        (
            ((SELECT COUNT(*) FROM dbo.FactFMUserToCompany WHERE CompanySKey IS NULL) > 0)
            OR 
            ((SELECT COUNT(*) FROM dbo.FactFMUserToCompany WHERE FMUserSKey IS NULL) > 0)
        )
      BEGIN
        RAISERROR ('Failure to resolve Company-To-User references', 16, 1);
        RETURN;
      END
    END

    --Map all users to the dummy company record.
    INSERT INTO dbo.FactFMUserToCompany (CompanySKey, FMUserSKey)
    SELECT 0, a.SKey
    FROM DimFMUser a
    WHERE NOT EXISTS 
    (
        SELECT * FROM dbo.FactFMUserToCompany b
        WHERE b.FMUserSKey = a.SKey
        AND b.CompanySKey = 0
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
    + 'Procedure Name: [staging].[usp_LoadCompanyToUserMap]' + CHAR(13) + CHAR(10)
    + 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS varchar(20)), '') + CHAR(13) + CHAR(10);
    RAISERROR (@_ErrMessage, 16, 1);
  END CATCH

END