/*
    Sanitize FuelsManagerDB Backup.Tests.sql

    SQLCMD validation harness for the companion maintenance script. Run from the
    repository root with SQLCMD mode enabled, for example:

      sqlcmd -S <server> -E -i "Web Application/Database/Database Maintenance/Sanitize FuelsManagerDB Backup.Tests.sql"

    The harness creates and drops a disposable database named below.
*/

:setvar TestDatabase SanitizeFuelsManagerDBBackup_Test

SET NOCOUNT ON;
GO

USE [master];
GO

IF DB_ID(N'$(TestDatabase)') IS NOT NULL
BEGIN
    ALTER DATABASE [$(TestDatabase)] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE [$(TestDatabase)];
END;
GO

CREATE DATABASE [$(TestDatabase)];
GO

USE [$(TestDatabase)];
GO

:r "Web Application/Database/Database Maintenance/Sanitize FuelsManagerDB Backup.sql"
GO

IF SCHEMA_ID(N'fmaudit') IS NULL EXEC(N'CREATE SCHEMA fmaudit');
IF SCHEMA_ID(N'fmcdc') IS NULL EXEC(N'CREATE SCHEMA fmcdc');
IF SCHEMA_ID(N'track') IS NULL EXEC(N'CREATE SCHEMA track');
GO

CREATE TABLE dbo.tblUsers
(
    Password VARBINARY(256) NOT NULL,
    PasswordHistory1 VARBINARY(256) NULL,
    Name NVARCHAR(50) NULL,
    EmailAddress NVARCHAR(50) NULL,
    PasswordHint NVARCHAR(80) NULL,
    PhoneNumber NVARCHAR(20) NULL,
    UserData1 NVARCHAR(120) NULL
);

INSERT INTO dbo.tblUsers (Password, PasswordHistory1, Name, EmailAddress, PasswordHint, PhoneNumber, UserData1)
VALUES
    (0x1122, 0x3344, N'Example User', N'user1@example.invalid', N'hint', N'555-0101', N'user data'),
    (0x5566, NULL, N'Example User 2', N'user2@example.invalid', NULL, NULL, NULL);

CREATE TABLE dbo.tblSites
(
    Phone NVARCHAR(20) NULL
);

INSERT INTO dbo.tblSites (Phone) VALUES (N'555-0102');

CREATE TABLE fmaudit.tblUsers
(
    Password VARBINARY(256) NULL,
    Name NVARCHAR(50) NULL,
    EmailAddress NVARCHAR(50) NULL
);

INSERT INTO fmaudit.tblUsers (Password, Name, EmailAddress)
VALUES (0x7788, N'Audited User', N'audit@example.invalid');

CREATE TABLE fmcdc.tblPersonnel
(
    FirstName NVARCHAR(20) NULL,
    LastName NVARCHAR(30) NULL,
    Email NVARCHAR(50) NULL,
    PINNumber VARBINARY(256) NULL
);

INSERT INTO fmcdc.tblPersonnel (FirstName, LastName, Email, PINNumber)
VALUES (N'CDC', N'Person', N'cdc@example.invalid', 0x9999);

CREATE TABLE track.tblUsers
(
    ChangeIndex BIGINT NOT NULL,
    InsertedContext VARBINARY(128) NULL,
    UpdatedContext VARBINARY(128) NULL,
    DeletedContext VARBINARY(128) NULL
);

INSERT INTO track.tblUsers (ChangeIndex, InsertedContext, UpdatedContext, DeletedContext)
VALUES (1, 0x01, 0x02, 0x03);
GO

DECLARE @Report TABLE
(
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

INSERT INTO @Report
EXEC #SanitizeFuelsManagerDBBackup @Mode = N'Report';

IF NOT EXISTS
(
    SELECT 1
    FROM @Report
    WHERE SchemaName = N'dbo'
      AND TableName = N'tblUsers'
      AND ColumnName = N'Password'
      AND TableExists = 1
      AND ColumnExists = 1
      AND CandidateRowCount = 2
      AND RowsChanged IS NULL
      AND Status = N'CANDIDATE'
)
BEGIN
    RAISERROR('Report mode did not inventory dbo.tblUsers.Password with the expected row count and no data changes.', 16, 1);
END;

IF NOT EXISTS
(
    SELECT 1
    FROM @Report
    WHERE SchemaName = N'dbo'
      AND TableName = N'tblSites'
      AND ColumnName = N'EmailAddress'
      AND TableExists = 1
      AND ColumnExists = 0
      AND CandidateRowCount = 1
      AND Status = N'SKIPPED_ABSENT_COLUMN'
)
BEGIN
    RAISERROR('Report mode did not skip an absent column on an older-schema table.', 16, 1);
END;

IF NOT EXISTS
(
    SELECT 1
    FROM @Report
    WHERE SchemaName = N'fmaudit'
      AND TableName = N'tblSites'
      AND ColumnName = N'Phone'
      AND TableExists = 0
      AND ColumnExists = 0
      AND CandidateRowCount IS NULL
      AND Status = N'SKIPPED_ABSENT_TABLE'
)
BEGIN
    RAISERROR('Report mode did not skip an absent fmaudit table.', 16, 1);
END;

IF NOT EXISTS
(
    SELECT 1
    FROM @Report
    WHERE SchemaName = N'track'
      AND TableName = N'tblUsers'
      AND ColumnName = N'InsertedContext'
      AND TableExists = 1
      AND ColumnExists = 1
      AND CandidateRowCount = 1
      AND Status = N'CANDIDATE'
)
BEGIN
    RAISERROR('Report mode did not inventory a track schema candidate column.', 16, 1);
END;

IF EXISTS
(
    SELECT 1
    FROM @Report
    WHERE TableExists = 0
      AND Status <> N'SKIPPED_ABSENT_TABLE'
)
BEGIN
    RAISERROR('At least one absent table was not reported as SKIPPED_ABSENT_TABLE.', 16, 1);
END;

IF EXISTS
(
    SELECT 1
    FROM @Report
    WHERE TableExists = 1
      AND ColumnExists = 0
      AND Status <> N'SKIPPED_ABSENT_COLUMN'
)
BEGIN
    RAISERROR('At least one absent column was not reported as SKIPPED_ABSENT_COLUMN.', 16, 1);
END;
GO

DECLARE @SanitizeReport TABLE
(
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

INSERT INTO @SanitizeReport
EXEC #SanitizeFuelsManagerDBBackup @Mode = N'Sanitize';

IF EXISTS (SELECT 1 FROM dbo.tblUsers WHERE Password <> 0x00 OR ISNULL(EmailAddress, N'') <> N'' OR ISNULL(PhoneNumber, N'') <> N'')
BEGIN
    RAISERROR('Sanitize mode did not update discovered dbo.tblUsers candidate columns.', 16, 1);
END;

IF EXISTS (SELECT 1 FROM track.tblUsers WHERE InsertedContext IS NOT NULL OR UpdatedContext IS NOT NULL OR DeletedContext IS NOT NULL)
BEGIN
    RAISERROR('Sanitize mode did not clear discovered track context columns.', 16, 1);
END;

IF NOT EXISTS
(
    SELECT 1
    FROM @SanitizeReport
    WHERE SchemaName = N'dbo'
      AND TableName = N'tblSites'
      AND ColumnName = N'EmailAddress'
      AND Status = N'SKIPPED_ABSENT_COLUMN'
)
BEGIN
    RAISERROR('Sanitize mode did not continue to skip absent older-schema columns.', 16, 1);
END;
GO

USE [master];
GO

ALTER DATABASE [$(TestDatabase)] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
DROP DATABASE [$(TestDatabase)];
GO

PRINT 'Sanitize FuelsManagerDB Backup tests passed.';
GO
