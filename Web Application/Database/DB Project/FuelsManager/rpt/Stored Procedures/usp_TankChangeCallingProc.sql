CREATE PROCEDURE [rpt].[usp_TankChangeCallingProc]
	@SiteGuid [uniqueidentifier],
	@SiteID [nvarchar](max),
	@BeginDate [datetimeoffset](7),
	@EndDate [datetimeoffset](7),
	@SelectedType [int],
	@UserGuidStr	[uniqueidentifier],
	@UseSmallFieldNames [bit],
	@RefDataTable [nvarchar](max),
	@CassandraConfiguration [nvarchar](max),
	@CassandraUsername [nvarchar](max),
	@CassandraPassword [nvarchar](max)
AS
EXTERNAL NAME [FMDatabase.SqlServer.Clr].[FMDatabase.SqlServer.Clr.TankChangeProvider].[usp_TankChange]
GO


