USE [master]
GO

-- Make sure CLR support is turned on
Exec sp_configure 'clr enabled', 1
GO
RECONFIGURE
GO
sp_dbcmptlevel 'ConsolidatedDB', '90';
GO

ALTER DATABASE ConsolidatedDB SET TRUSTWORTHY ON
GO

ALTER AUTHORIZATION ON DATABASE::ConsolidatedDB TO sa
GO

USE [ConsolidatedDB]
GO

-- Drop section --
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[udf_GetLocalTime]') AND type in (N'FN', N'IF', N'TF', N'FS', N'FT'))
DROP FUNCTION [dbo].[udf_GetLocalTime]
GO

/****** Object:  UserDefinedFunction [dbo].[udf_GetUTCTime]    Script Date: 09/14/2009 17:07:19 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[udf_GetUTCTime]') AND type in (N'FN', N'IF', N'TF', N'FS', N'FT'))
DROP FUNCTION [dbo].[udf_GetUTCTime]
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetLocalOffset]') AND type in (N'FN', N'IF', N'TF', N'FS', N'FT'))
DROP FUNCTION [dbo].[GetLocalOffset]
GO

IF  EXISTS (SELECT * FROM sys.assemblies asms WHERE asms.name = N'FMCLRAssembly')
DROP ASSEMBLY [FMCLRAssembly]
GO

IF  EXISTS (SELECT * FROM sys.assemblies asms WHERE asms.name = N'EngineeringUnitsLibrary')
DROP ASSEMBLY [EngineeringUnitsLibrary]
GO

-- Create section ----
USE [master]
GO

IF NOT EXISTS (SELECT loginname FROM master.dbo.syslogins WHERE NAME = 'FMCLRAssemblyLogin')
	BEGIN
		PRINT 'Creating FMCLRAssemblyLogin...'
		
		IF NOT EXISTS (SELECT * FROM sys.asymmetric_keys WHERE name = 'FMCLRAssemblyKey')
			CREATE ASYMMETRIC KEY FMCLRAssemblyKey FROM EXECUTABLE 

		--FILE = 'C:\FuelsManager\FMCLRAssembly.dll'
		FILE = 'C:\FuelsManager Primus\Web Application\SQLCLRComponents\SQLCLRFunctions\bin\Debug\FMCLRAssembly.dll'
		CREATE LOGIN FMCLRAssemblyLogin FROM ASYMMETRIC KEY FMCLRAssemblyKey
		GRANT UNSAFE ASSEMBLY TO FMCLRAssemblyLogin
	END
ELSE
	PRINT 'FMCLRAssemblyLogin exists'
GO
	
USE ConsolidatedDB
GO

IF NOT EXISTS (SELECT NAME FROM ConsolidatedDB.sys.assemblies WHERE [name]='FMCLRAssembly')
	BEGIN
		PRINT 'Creating assembly FMCLRAssembly...'
		CREATE ASSEMBLY FMCLRAssembly 
		--FROM 'C:\FuelsManager\FMCLRAssembly.dll'
		FROM 'C:\FuelsManager Primus\Web Application\SQLCLRComponents\SQLCLRFunctions\bin\Debug\FMCLRAssembly.dll'
		WITH PERMISSION_SET = UNSAFE
		
		--ALTER ASSEMBLY FMCLRAssembly 
		--ADD FILE FROM 'C:\FuelsManager.NET\FuelsManager Wallaby\Web Application\SQLCLRComponents\SQLCLRFunctions\bin\Debug\FMCLRAssembly.pdb'
	END
GO

CREATE FUNCTION dbo.GetLocalOffset(@dateTime DateTime,@UtcDateTime bit,@StandardName nvarchar(50),@AdjustForDaylightSavings bit) 
	RETURNS int AS EXTERNAL NAME FMCLRAssembly.FMCLRFunctionClass.GetLocalOffset
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE FUNCTION [dbo].[udf_GetUTCTime] (@SiteGuid UNIQUEIDENTIFIER, @LocalTime DATETIME)
RETURNS DATETIME
WITH SCHEMABINDING
AS
BEGIN
	DECLARE @StandardName	NVARCHAR(50)
	DECLARE @AdjustForDST	BIT
	DECLARE @OffsetMinutes	INT

	IF @LocalTime IS NULL
		RETURN NULL

	SELECT @StandardName  = TimeZone                 FROM dbo.tblSites WHERE SiteGuid = @SiteGuid
	SELECT @AdjustForDST  = AdjustForDaylightSavings FROM dbo.tblSites WHERE SiteGuid = @SiteGuid
	SET    @OffsetMinutes = dbo.GetLocalOffset(@LocalTime, 0, RTRIM(@StandardName), @AdjustForDST)

	RETURN DATEADD(minute, @OffsetMinutes, @LocalTime)
END
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE FUNCTION [dbo].[udf_GetLocalTime] (
	@SiteGuid UNIQUEIDENTIFIER,
	@UTCTime datetime)
	RETURNS datetime WITH SCHEMABINDING AS
	BEGIN
	DECLARE @StandardName nvarchar(50)
	DECLARE @AdjustForDaylightSavings bit
	DECLARE @OffsetMinutes int
	DECLARE @LocalTime datetime

	IF @UTCTime IS NULL
		RETURN @LocalTime

	SET @StandardName =(SELECT TimeZone FROM dbo.tblSites WHERE SiteGuid=@SiteGuid)
	SET @AdjustForDaylightSavings = (SELECT AdjustForDaylightSavings FROM dbo.tblSites WHERE SiteGuid=@SiteGuid)

	SET @OffsetMinutes = (SELECT dbo.GetLocalOffset(@UTCTime,1,RTRIM(@StandardName),@AdjustForDaylightSavings))

	SET @LocalTime=DateAdd(minute,-@OffsetMinutes,@UTCTime)

	RETURN @LocalTime
END
GO
