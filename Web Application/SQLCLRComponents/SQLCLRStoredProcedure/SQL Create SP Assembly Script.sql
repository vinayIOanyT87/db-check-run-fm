USE [master]
GO

-- Make sure CLR support is turned on
Exec sp_configure 'clr enabled', 1
GO
reconfigure
GO
sp_dbcmptlevel 'ConsolidatedDB', '90';
GO

ALTER DATABASE ConsolidatedDB
SET TRUSTWORTHY ON
GO

USE ConsolidatedDB
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[fm_Ledger]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[fm_Ledger]
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[xsp_LedgerCalculator]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[xsp_LedgerCalculator]
GO

IF EXISTS (SELECT NAME FROM ConsolidatedDB.sys.assemblies WHERE [name]='FMCLRStoredProcedureAssembly')
	BEGIN
		DROP ASSEMBLY FMCLRStoredProcedureAssembly
	END

USE [master]
GO

IF NOT EXISTS (SELECT loginname FROM master.dbo.syslogins WHERE NAME = 'FMCLRAssemblyLogin')
	BEGIN
		PRINT 'Creating FMCLRAssemblyLogin...'
		
		IF NOT EXISTS (SELECT * FROM sys.asymmetric_keys WHERE name = 'FMCLRAssemblyKey')
			CREATE ASYMMETRIC KEY FMCLRAssemblyKey FROM EXECUTABLE 
		
		--FILE = 'C:\FuelsManager\FMCLRStoredProcedureAssembly.dll'
		FILE = 'C:\FuelsManager Primus\Web Application\SQLCLRComponents\SQLCLRStoredProcedure\bin\Debug\FMCLRStoredProcedureAssembly.dll'
		CREATE LOGIN FMCLRAssemblyLogin FROM ASYMMETRIC KEY FMCLRAssemblyKey
		GRANT UNSAFE ASSEMBLY TO FMCLRAssemblyLogin
	END
ELSE
	PRINT 'FMCLRAssemblyLogin exists'
GO
	
USE ConsolidatedDB
GO

IF NOT EXISTS (SELECT NAME FROM ConsolidatedDB.sys.assemblies WHERE [name]='FMCLRStoredProcedureAssembly')
	BEGIN
		PRINT 'Creating assembly FMCLRStoredProcedureAssembly...'
		CREATE ASSEMBLY FMCLRStoredProcedureAssembly 
		--FROM 'C:\FuelsManager\FMCLRStoredProcedureAssembly.dll'
		FROM 'C:\FuelsManager Primus\Web Application\SQLCLRComponents\SQLCLRStoredProcedure\bin\Debug\FMCLRStoredProcedureAssembly.dll'
		WITH PERMISSION_SET = UNSAFE
		
		--ALTER ASSEMBLY FMCLRStoredProcedureAssembly 
		--ADD FILE FROM 'C:\FuelsManager.NET\FuelsManager Primus\Web Application\SQLCLRComponents\SQLCLRStoredProcedure\bin\Debug\FMCLRStoredProcedureAssembly.pdb'
	END
GO


CREATE PROCEDURE [dbo].[xsp_LedgerCalculator](@BeginDate [smalldatetime], 
										     @EndDate [smalldatetime],
								             @ProductGuid [Guid],
								             @ManagerGuid [Guid],
								             @OwnerGuid [Guid],
								             @LoginSiteGuid [Guid],
								             @SelectedSiteGuid [Guid],
								             @UserGuid [Guid],
								             @LedgerRequest [int],
								             @ReportLedger [int],
								             @TankGuid [Guid],
								             @SystemEdition [int])
AS EXTERNAL NAME [FMCLRStoredProcedureAssembly].[FMCLRStoredProcedureClass].[xsp_LedgerCalculator]
GO
