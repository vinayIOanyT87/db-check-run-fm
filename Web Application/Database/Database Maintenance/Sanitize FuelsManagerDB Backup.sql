/*
    Sanitize FuelsManagerDB Backup.sql

    SQL Server 2014-compatible backup maintenance script.

    Usage:
      - Report mode is the default and is safe to run against production or backup
        databases. It inventories every configured candidate table/column, reports
        candidate row counts for discovered tables, and marks missing tables or
        columns as skipped.
      - Sanitize mode updates only discovered candidate columns. Missing tables and
        columns are skipped by the same runtime schema discovery used by Report mode.

    Examples:
      EXEC #SanitizeFuelsManagerDBBackup @Mode = N'Report';
      EXEC #SanitizeFuelsManagerDBBackup @Mode = N'Sanitize';
*/

SET NOCOUNT ON;
GO

IF OBJECT_ID('tempdb..#SanitizeFuelsManagerDBBackup') IS NOT NULL
    DROP PROCEDURE #SanitizeFuelsManagerDBBackup;
GO

CREATE PROCEDURE #SanitizeFuelsManagerDBBackup
    @Mode NVARCHAR(20) = N'Report'
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @NormalizedMode NVARCHAR(20);
    SET @NormalizedMode = UPPER(LTRIM(RTRIM(ISNULL(@Mode, N'Report'))));

    IF @NormalizedMode NOT IN (N'REPORT', N'SANITIZE')
    BEGIN
        RAISERROR('Unsupported mode. Use Report or Sanitize.', 16, 1);
        RETURN;
    END;

    IF OBJECT_ID('tempdb..#SanitizeFuelsManagerDBBackup_Report') IS NOT NULL
        DROP TABLE #SanitizeFuelsManagerDBBackup_Report;

    CREATE TABLE #SanitizeFuelsManagerDBBackup_Report
    (
        ReportId INT IDENTITY(1,1) NOT NULL,
        Mode NVARCHAR(20) NOT NULL,
        SchemaName SYSNAME NOT NULL,
        TableName SYSNAME NOT NULL,
        ColumnName SYSNAME NOT NULL,
        TableExists BIT NOT NULL,
        ColumnExists BIT NOT NULL,
        CandidateRowCount BIGINT NULL,
        RowsChanged BIGINT NULL,
        Status NVARCHAR(40) NOT NULL,
        Detail NVARCHAR(4000) NOT NULL
    );

    DECLARE @Targets TABLE
    (
        TargetId INT IDENTITY(1,1) NOT NULL,
        SchemaName SYSNAME NOT NULL,
        TableName SYSNAME NOT NULL,
        ColumnName SYSNAME NOT NULL,
        ReplacementExpression NVARCHAR(4000) NOT NULL
    );

    INSERT INTO @Targets (SchemaName, TableName, ColumnName, ReplacementExpression)
    VALUES
        (N'dbo', N'tblUsers', N'Password', N'CONVERT(varbinary(256), 0x00)'),
        (N'dbo', N'tblUsers', N'PasswordHistory1', N'CONVERT(varbinary(256), 0x00)'),
        (N'dbo', N'tblUsers', N'PasswordHistory2', N'CONVERT(varbinary(256), 0x00)'),
        (N'dbo', N'tblUsers', N'PasswordHistory3', N'CONVERT(varbinary(256), 0x00)'),
        (N'dbo', N'tblUsers', N'PasswordHistory4', N'CONVERT(varbinary(256), 0x00)'),
        (N'dbo', N'tblUsers', N'PasswordHistory5', N'CONVERT(varbinary(256), 0x00)'),
        (N'dbo', N'tblUsers', N'PasswordHistory6', N'CONVERT(varbinary(256), 0x00)'),
        (N'dbo', N'tblUsers', N'PasswordHistory7', N'CONVERT(varbinary(256), 0x00)'),
        (N'dbo', N'tblUsers', N'PasswordHistory8', N'CONVERT(varbinary(256), 0x00)'),
        (N'dbo', N'tblUsers', N'PasswordHistory9', N'CONVERT(varbinary(256), 0x00)'),
        (N'dbo', N'tblUsers', N'PasswordHistory10', N'CONVERT(varbinary(256), 0x00)'),
        (N'dbo', N'tblUsers', N'PasswordHistory11', N'CONVERT(varbinary(256), 0x00)'),
        (N'dbo', N'tblUsers', N'PasswordHistory12', N'CONVERT(varbinary(256), 0x00)'),
        (N'dbo', N'tblUsers', N'PasswordHistory13', N'CONVERT(varbinary(256), 0x00)'),
        (N'dbo', N'tblUsers', N'PasswordHistory14', N'CONVERT(varbinary(256), 0x00)'),
        (N'dbo', N'tblUsers', N'PasswordHistory15', N'CONVERT(varbinary(256), 0x00)'),
        (N'dbo', N'tblUsers', N'PasswordHistory16', N'CONVERT(varbinary(256), 0x00)'),
        (N'dbo', N'tblUsers', N'PasswordHistory17', N'CONVERT(varbinary(256), 0x00)'),
        (N'dbo', N'tblUsers', N'PasswordHistory18', N'CONVERT(varbinary(256), 0x00)'),
        (N'dbo', N'tblUsers', N'PasswordHistory19', N'CONVERT(varbinary(256), 0x00)'),
        (N'dbo', N'tblUsers', N'PasswordHistory20', N'CONVERT(varbinary(256), 0x00)'),
        (N'dbo', N'tblUsers', N'PasswordHistory21', N'CONVERT(varbinary(256), 0x00)'),
        (N'dbo', N'tblUsers', N'PasswordHistory22', N'CONVERT(varbinary(256), 0x00)'),
        (N'dbo', N'tblUsers', N'PasswordHistory23', N'CONVERT(varbinary(256), 0x00)'),
        (N'dbo', N'tblUsers', N'PasswordHistory24', N'CONVERT(varbinary(256), 0x00)'),
        (N'dbo', N'tblUsers', N'Name', N'N''Sanitized'''),
        (N'dbo', N'tblUsers', N'EmailAddress', N'N'''''),
        (N'dbo', N'tblUsers', N'PasswordHint', N'N'''''),
        (N'dbo', N'tblUsers', N'PhoneNumber', N'N'''''),
        (N'dbo', N'tblUsers', N'UserData1', N'N'''''),
        (N'dbo', N'tblUsers', N'UserData2', N'N'''''),
        (N'dbo', N'tblUsers', N'UserData3', N'N'''''),
        (N'dbo', N'tblUsers', N'UserData4', N'N'''''),
        (N'dbo', N'tblUsers', N'UserData5', N'N'''''),
        (N'dbo', N'tblUsers', N'UserData6', N'N'''''),
        (N'dbo', N'tblUsers', N'UserData7', N'N'''''),
        (N'dbo', N'tblUsers', N'UserData8', N'N'''''),

        (N'dbo', N'tblSites', N'Address1', N'N'''''),
        (N'dbo', N'tblSites', N'Address2', N'N'''''),
        (N'dbo', N'tblSites', N'City', N'N'''''),
        (N'dbo', N'tblSites', N'State', N'N'''''),
        (N'dbo', N'tblSites', N'Zip', N'N'''''),
        (N'dbo', N'tblSites', N'Country', N'N'''''),
        (N'dbo', N'tblSites', N'Phone', N'N'''''),
        (N'dbo', N'tblSites', N'FAX', N'N'''''),
        (N'dbo', N'tblSites', N'EmailAddress', N'N'''''),
        (N'dbo', N'tblSites', N'EmergencyContact', N'N'''''),
        (N'dbo', N'tblSites', N'EmergencyPhone', N'N'''''),
        (N'dbo', N'tblSites', N'MailServer', N'N'''''),
        (N'dbo', N'tblSites', N'MailFrom', N'N'''''),
        (N'dbo', N'tblSites', N'MailUserName', N'N'''''),
        (N'dbo', N'tblSites', N'MailPassword', N'N'''''),
        (N'dbo', N'tblSites', N'DialupName', N'N'''''),
        (N'dbo', N'tblSites', N'ReportDirectory', N'N'''''),
        (N'dbo', N'tblSites', N'ManagedReportDirectory', N'N'''''),
        (N'dbo', N'tblSites', N'Contact1Name', N'N'''''),
        (N'dbo', N'tblSites', N'Contact1Address1', N'N'''''),
        (N'dbo', N'tblSites', N'Contact1Address2', N'N'''''),
        (N'dbo', N'tblSites', N'Contact1City', N'N'''''),
        (N'dbo', N'tblSites', N'Contact1State', N'N'''''),
        (N'dbo', N'tblSites', N'Contact1Zip', N'N'''''),
        (N'dbo', N'tblSites', N'Contact1Country', N'N'''''),
        (N'dbo', N'tblSites', N'Contact1PhoneOffice', N'N'''''),
        (N'dbo', N'tblSites', N'Contact1Fax', N'N'''''),
        (N'dbo', N'tblSites', N'Contact1EmailAddress', N'N'''''),
        (N'dbo', N'tblSites', N'Contact1PhoneMobile', N'N'''''),
        (N'dbo', N'tblSites', N'Contact2Name', N'N'''''),
        (N'dbo', N'tblSites', N'Contact2Address1', N'N'''''),
        (N'dbo', N'tblSites', N'Contact2Address2', N'N'''''),
        (N'dbo', N'tblSites', N'Contact2City', N'N'''''),
        (N'dbo', N'tblSites', N'Contact2State', N'N'''''),
        (N'dbo', N'tblSites', N'Contact2Zip', N'N'''''),
        (N'dbo', N'tblSites', N'Contact2Country', N'N'''''),
        (N'dbo', N'tblSites', N'Contact2PhoneOffice', N'N'''''),
        (N'dbo', N'tblSites', N'Contact2Fax', N'N'''''),
        (N'dbo', N'tblSites', N'Contact2EmailAddress', N'N'''''),
        (N'dbo', N'tblSites', N'Contact2PhoneMobile', N'N'''''),
        (N'dbo', N'tblSites', N'EnterpriseUserId', N'N'''''),
        (N'dbo', N'tblSites', N'EnterprisePassword', N'CONVERT(varbinary(256), 0x00)'),
        (N'dbo', N'tblSites', N'EnterpriseSite', N'N'''''),
        (N'dbo', N'tblSites', N'ServerEndPoint', N'N'''''),
        (N'dbo', N'tblSites', N'UserId', N'N'''''),
        (N'dbo', N'tblSites', N'UserPassword', N'N'''''),
        (N'dbo', N'tblSites', N'UserCertificatePath', N'N'''''),

        (N'dbo', N'tblCompanies', N'Name', N'N''Sanitized'''),
        (N'dbo', N'tblCompanies', N'ShortName', N'N'''''),
        (N'dbo', N'tblCompanies', N'Address1', N'N'''''),
        (N'dbo', N'tblCompanies', N'Address2', N'N'''''),
        (N'dbo', N'tblCompanies', N'City', N'N'''''),
        (N'dbo', N'tblCompanies', N'State', N'N'''''),
        (N'dbo', N'tblCompanies', N'Zip', N'N'''''),
        (N'dbo', N'tblCompanies', N'Country', N'N'''''),
        (N'dbo', N'tblCompanies', N'Phone', N'N'''''),
        (N'dbo', N'tblCompanies', N'FAX', N'N'''''),
        (N'dbo', N'tblCompanies', N'EmergencyContact', N'N'''''),
        (N'dbo', N'tblCompanies', N'EmergencyPhone', N'N'''''),
        (N'dbo', N'tblCompanies', N'FederalID', N'N'''''),
        (N'dbo', N'tblCompanies', N'FederalID2', N'N'''''),
        (N'dbo', N'tblCompanies', N'FederalID3', N'N'''''),
        (N'dbo', N'tblCompanies', N'FederalID4', N'N'''''),
        (N'dbo', N'tblCompanies', N'FederalID5', N'N'''''),
        (N'dbo', N'tblCompanies', N'StateID', N'N'''''),
        (N'dbo', N'tblCompanies', N'TaxNumber', N'N'''''),
        (N'dbo', N'tblCompanies', N'LicenseNumber', N'N'''''),
        (N'dbo', N'tblCompanies', N'InsuranceCompany', N'N'''''),
        (N'dbo', N'tblCompanies', N'InsurancePolicy', N'N'''''),
        (N'dbo', N'tblCompanies', N'AccountNumber', N'N'''''),
        (N'dbo', N'tblCompanies', N'Contact1Name', N'N'''''),
        (N'dbo', N'tblCompanies', N'Contact1Address1', N'N'''''),
        (N'dbo', N'tblCompanies', N'Contact1Address2', N'N'''''),
        (N'dbo', N'tblCompanies', N'Contact1City', N'N'''''),
        (N'dbo', N'tblCompanies', N'Contact1State', N'N'''''),
        (N'dbo', N'tblCompanies', N'Contact1Zip', N'N'''''),
        (N'dbo', N'tblCompanies', N'Contact1Country', N'N'''''),
        (N'dbo', N'tblCompanies', N'Contact1PhoneOffice', N'N'''''),
        (N'dbo', N'tblCompanies', N'Contact1Fax', N'N'''''),
        (N'dbo', N'tblCompanies', N'Contact1EmailAddress', N'N'''''),
        (N'dbo', N'tblCompanies', N'Contact1PhoneMobile', N'N'''''),
        (N'dbo', N'tblCompanies', N'Contact2Name', N'N'''''),
        (N'dbo', N'tblCompanies', N'Contact2Address1', N'N'''''),
        (N'dbo', N'tblCompanies', N'Contact2Address2', N'N'''''),
        (N'dbo', N'tblCompanies', N'Contact2City', N'N'''''),
        (N'dbo', N'tblCompanies', N'Contact2State', N'N'''''),
        (N'dbo', N'tblCompanies', N'Contact2Zip', N'N'''''),
        (N'dbo', N'tblCompanies', N'Contact2Country', N'N'''''),
        (N'dbo', N'tblCompanies', N'Contact2PhoneOffice', N'N'''''),
        (N'dbo', N'tblCompanies', N'Contact2Fax', N'N'''''),
        (N'dbo', N'tblCompanies', N'Contact2EmailAddress', N'N'''''),
        (N'dbo', N'tblCompanies', N'Contact2PhoneMobile', N'N'''''),
        (N'dbo', N'tblCompanies', N'Note', N'N'''''),

        (N'dbo', N'tblPersonnel', N'CardNumber', N'N'''''),
        (N'dbo', N'tblPersonnel', N'FirstName', N'N''Sanitized'''),
        (N'dbo', N'tblPersonnel', N'MiddleName', N'N'''''),
        (N'dbo', N'tblPersonnel', N'LastName', N'N''User'''),
        (N'dbo', N'tblPersonnel', N'Title', N'N'''''),
        (N'dbo', N'tblPersonnel', N'Department', N'N'''''),
        (N'dbo', N'tblPersonnel', N'Address1', N'N'''''),
        (N'dbo', N'tblPersonnel', N'Address2', N'N'''''),
        (N'dbo', N'tblPersonnel', N'City', N'N'''''),
        (N'dbo', N'tblPersonnel', N'State', N'N'''''),
        (N'dbo', N'tblPersonnel', N'Zip', N'N'''''),
        (N'dbo', N'tblPersonnel', N'Country', N'N'''''),
        (N'dbo', N'tblPersonnel', N'Phone1', N'N'''''),
        (N'dbo', N'tblPersonnel', N'Phone2', N'N'''''),
        (N'dbo', N'tblPersonnel', N'SSAN', N'N'''''),
        (N'dbo', N'tblPersonnel', N'Email', N'N'''''),
        (N'dbo', N'tblPersonnel', N'PINNumber', N'CONVERT(varbinary(256), 0x00)'),
        (N'dbo', N'tblPersonnel', N'LockedOutReason', N'N'''''),
        (N'dbo', N'tblPersonnel', N'ShortCardNumber', N'N'''''),
        (N'dbo', N'tblPersonnel', N'OnFileSignature', N'CONVERT(varbinary(max), 0x00)'),
        (N'dbo', N'tblPersonnel', N'UserData1', N'N'''''),
        (N'dbo', N'tblPersonnel', N'UserData2', N'N'''''),
        (N'dbo', N'tblPersonnel', N'UserData3', N'N'''''),
        (N'dbo', N'tblPersonnel', N'UserData4', N'N'''''),
        (N'dbo', N'tblPersonnel', N'UserData5', N'N'''''),
        (N'dbo', N'tblPersonnel', N'UserData6', N'N'''''),
        (N'dbo', N'tblPersonnel', N'UserData7', N'N'''''),
        (N'dbo', N'tblPersonnel', N'UserData8', N'N'''''),
        (N'dbo', N'tblPersonnel', N'UserData9', N'N'''''),
        (N'dbo', N'tblPersonnel', N'UserData10', N'N'''''),
        (N'dbo', N'tblPersonnel', N'UserData11', N'N'''''),
        (N'dbo', N'tblPersonnel', N'UserData12', N'N'''''),
        (N'dbo', N'tblPersonnel', N'UserData13', N'N'''''),
        (N'dbo', N'tblPersonnel', N'UserData14', N'N'''''),
        (N'dbo', N'tblPersonnel', N'UserData15', N'N'''''),
        (N'dbo', N'tblPersonnel', N'UserData16', N'N'''''),
        (N'dbo', N'tblPersonnel', N'UserData17', N'N'''''),
        (N'dbo', N'tblPersonnel', N'UserData18', N'N'''''),
        (N'dbo', N'tblPersonnel', N'UserData19', N'N'''''),
        (N'dbo', N'tblPersonnel', N'UserData20', N'N'''''),
        (N'dbo', N'tblPersonnel', N'UserData21', N'N'''''),
        (N'dbo', N'tblPersonnel', N'UserData22', N'N'''''),
        (N'dbo', N'tblPersonnel', N'UserData23', N'N'''''),
        (N'dbo', N'tblPersonnel', N'UserData24', N'N'''''),

        (N'dbo', N'tblFuelCards', N'Provider', N'N'''''),
        (N'dbo', N'tblFuelCards', N'Notes', N'N'''''),
        (N'dbo', N'tblFuelCards', N'PIN', N'CONVERT(varbinary(256), 0x00)'),
        (N'dbo', N'tblFuelCards', N'ProviderID', N'N''''');

    INSERT INTO @Targets (SchemaName, TableName, ColumnName, ReplacementExpression)
    SELECT AuditSchema.SchemaName, Targets.TableName, Targets.ColumnName, Targets.ReplacementExpression
    FROM @Targets Targets
    CROSS JOIN (SELECT N'fmaudit' AS SchemaName UNION ALL SELECT N'fmcdc') AuditSchema
    WHERE Targets.SchemaName = N'dbo';

    INSERT INTO @Targets (SchemaName, TableName, ColumnName, ReplacementExpression)
    VALUES
        (N'track', N'tblUsers', N'InsertedContext', N'NULL'),
        (N'track', N'tblUsers', N'UpdatedContext', N'NULL'),
        (N'track', N'tblUsers', N'DeletedContext', N'NULL'),
        (N'track', N'tblSites', N'InsertedContext', N'NULL'),
        (N'track', N'tblSites', N'UpdatedContext', N'NULL'),
        (N'track', N'tblSites', N'DeletedContext', N'NULL'),
        (N'track', N'tblCompanies', N'InsertedContext', N'NULL'),
        (N'track', N'tblCompanies', N'UpdatedContext', N'NULL'),
        (N'track', N'tblCompanies', N'DeletedContext', N'NULL'),
        (N'track', N'tblPersonnel', N'InsertedContext', N'NULL'),
        (N'track', N'tblPersonnel', N'UpdatedContext', N'NULL'),
        (N'track', N'tblPersonnel', N'DeletedContext', N'NULL'),
        (N'track', N'tblFuelCards', N'InsertedContext', N'NULL'),
        (N'track', N'tblFuelCards', N'UpdatedContext', N'NULL'),
        (N'track', N'tblFuelCards', N'DeletedContext', N'NULL'),
        (N'track', N'tblChangeTrackingSession', N'ContextName', N'N'''''),
        (N'track', N'tblChangeTrackingSession', N'BypassReason', N'N''''');

    DECLARE @TargetId INT;
    DECLARE @MaxTargetId INT;
    DECLARE @SchemaName SYSNAME;
    DECLARE @TableName SYSNAME;
    DECLARE @ColumnName SYSNAME;
    DECLARE @ReplacementExpression NVARCHAR(4000);
    DECLARE @ObjectId INT;
    DECLARE @ColumnExists BIT;
    DECLARE @CandidateRowCount BIGINT;
    DECLARE @RowsChanged BIGINT;
    DECLARE @Sql NVARCHAR(MAX);

    SELECT @TargetId = MIN(TargetId), @MaxTargetId = MAX(TargetId) FROM @Targets;

    WHILE @TargetId IS NOT NULL AND @TargetId <= @MaxTargetId
    BEGIN
        SELECT
            @SchemaName = SchemaName,
            @TableName = TableName,
            @ColumnName = ColumnName,
            @ReplacementExpression = ReplacementExpression
        FROM @Targets
        WHERE TargetId = @TargetId;

        SELECT @ObjectId = t.object_id
        FROM sys.schemas s
        INNER JOIN sys.tables t ON t.schema_id = s.schema_id
        WHERE s.name = @SchemaName
          AND t.name = @TableName;

        SET @ColumnExists = 0;
        SET @CandidateRowCount = NULL;
        SET @RowsChanged = NULL;

        IF @ObjectId IS NULL
        BEGIN
            INSERT INTO #SanitizeFuelsManagerDBBackup_Report
                (Mode, SchemaName, TableName, ColumnName, TableExists, ColumnExists, CandidateRowCount, RowsChanged, Status, Detail)
            VALUES
                (@NormalizedMode, @SchemaName, @TableName, @ColumnName, 0, 0, NULL, NULL, N'SKIPPED_ABSENT_TABLE', N'Table was not found in sys.schemas/sys.tables.');
        END
        ELSE
        BEGIN
            SELECT @ColumnExists = CASE WHEN EXISTS
            (
                SELECT 1
                FROM sys.columns c
                WHERE c.object_id = @ObjectId
                  AND c.name = @ColumnName
            ) THEN 1 ELSE 0 END;

            SET @Sql = N'SELECT @CandidateRowCount = COUNT_BIG(1) FROM ' + QUOTENAME(@SchemaName) + N'.' + QUOTENAME(@TableName) + N';';
            EXEC sp_executesql @Sql, N'@CandidateRowCount bigint OUTPUT', @CandidateRowCount = @CandidateRowCount OUTPUT;

            IF @ColumnExists = 0
            BEGIN
                INSERT INTO #SanitizeFuelsManagerDBBackup_Report
                    (Mode, SchemaName, TableName, ColumnName, TableExists, ColumnExists, CandidateRowCount, RowsChanged, Status, Detail)
                VALUES
                    (@NormalizedMode, @SchemaName, @TableName, @ColumnName, 1, 0, @CandidateRowCount, NULL, N'SKIPPED_ABSENT_COLUMN', N'Column was not found in sys.columns.');
            END
            ELSE
            BEGIN
                IF @NormalizedMode = N'SANITIZE'
                BEGIN
                    SET @Sql = N'UPDATE ' + QUOTENAME(@SchemaName) + N'.' + QUOTENAME(@TableName) +
                               N' SET ' + QUOTENAME(@ColumnName) + N' = ' + @ReplacementExpression + N'; SELECT @RowsChanged = @@ROWCOUNT;';
                    EXEC sp_executesql @Sql, N'@RowsChanged bigint OUTPUT', @RowsChanged = @RowsChanged OUTPUT;
                END;

                INSERT INTO #SanitizeFuelsManagerDBBackup_Report
                    (Mode, SchemaName, TableName, ColumnName, TableExists, ColumnExists, CandidateRowCount, RowsChanged, Status, Detail)
                VALUES
                    (@NormalizedMode, @SchemaName, @TableName, @ColumnName, 1, 1, @CandidateRowCount, @RowsChanged, N'CANDIDATE',
                     CASE WHEN @NormalizedMode = N'REPORT' THEN N'Candidate column discovered; no data changed in Report mode.' ELSE N'Candidate column sanitized.' END);
            END;
        END;

        SELECT @TargetId = MIN(TargetId) FROM @Targets WHERE TargetId > @TargetId;
    END;

    SELECT
        Mode,
        SchemaName,
        TableName,
        ColumnName,
        TableExists,
        ColumnExists,
        CandidateRowCount,
        RowsChanged,
        Status,
        Detail
    FROM #SanitizeFuelsManagerDBBackup_Report
    ORDER BY SchemaName, TableName, ColumnName;
END;
GO

EXEC #SanitizeFuelsManagerDBBackup @Mode = N'Report';
GO
