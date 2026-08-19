USE ConsolidatedDB
GO


IF EXISTS (SELECT * FROM sys.assemblies WHERE name = 'EngineeringUnitsLibrary')
	DROP ASSEMBLY EngineeringUnitsLibrary

USE [Master]
GO

IF EXISTS (SELECT * FROM sys.server_principals WHERE name = 'EngineeringUnitsLibraryLogin' AND type_desc = 'ASYMMETRIC_KEY_MAPPED_LOGIN')
	DROP LOGIN EngineeringUnitsLibraryLogin

IF EXISTS (SELECT * FROM sys.asymmetric_keys WHERE name = 'EngineeringUnitsLibraryKey')
	DROP ASYMMETRIC KEY EngineeringUnitsLibraryKey

IF NOT EXISTS (SELECT * FROM sys.asymmetric_keys WHERE name = 'EngineeringUnitsLibraryKey')
BEGIN
	RAISERROR('Re-creating key EngineeringUnitsLibraryKey', 10, 1) WITH NOWAIT, LOG

	CREATE ASYMMETRIC KEY EngineeringUnitsLibraryKey
	FROM EXECUTABLE FILE = 'c:\FuelsManager Primus\Web Application\EngineeringUnitsLibrary\bin\debug\EngineeringUnitsLibrary.dll'
END

IF NOT EXISTS (SELECT * FROM sys.server_principals WHERE name = 'EngineeringUnitsLibraryLogin' AND type_desc = 'ASYMMETRIC_KEY_MAPPED_LOGIN')
	CREATE LOGIN EngineeringUnitsLibraryLogin FROM ASYMMETRIC KEY EngineeringUnitsLibraryKey

GRANT UNSAFE ASSEMBLY TO EngineeringUnitsLibraryLogin

USE ConsolidatedDB
GO

IF NOT EXISTS (SELECT * FROM sys.assemblies WHERE name = 'EngineeringUnitsLibrary')
	CREATE ASSEMBLY EngineeringUnitsLibrary
	FROM 'c:\FuelsManager Primus\Web Application\EngineeringUnitsLibrary\bin\debug\EngineeringUnitsLibrary.dll' WITH PERMISSION_SET = UNSAFE

