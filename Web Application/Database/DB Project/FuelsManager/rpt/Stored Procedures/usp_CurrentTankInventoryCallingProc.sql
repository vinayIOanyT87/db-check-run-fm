CREATE PROCEDURE [rpt].[usp_CurrentTankInventoryCallingProc]
	@SiteGuid UNIQUEIDENTIFIER,
	@SiteID nvarchar(max),
	@BeginDate DATETIMEOFFSET,
	@UseSmallFieldNames BIT,
	@useDateOnly BIT,
	@UserGuidStr	UNIQUEIDENTIFIER,
	@RefDataTable nvarchar(max),
	@CassandraConfiguration NVARCHAR(MAX),
	@CassandraUsername NVARCHAR(MAX),
	@CassandraPassword NVARCHAR(MAX)
AS
EXTERNAL NAME [FMDatabase.SqlServer.Clr].[FMDatabase.SqlServer.Clr.CurrentTankInventoryProvider].[usp_CurrentTankInventory]
GO
