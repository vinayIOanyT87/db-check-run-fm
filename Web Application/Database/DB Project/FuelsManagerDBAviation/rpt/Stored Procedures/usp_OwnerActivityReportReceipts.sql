-- =============================================
-- Author:		Gregory Lybanon
-- Create date: 11/09/2018
-- Description:	Gets the Receipt records for the
--				Owner Activity Report
-- =============================================
CREATE PROCEDURE [rpt].[usp_OwnerActivityReportReceipts] 
	@SiteGuid UNIQUEIDENTIFIER, 
	@StartDate DATETIMEOFFSET,
	@EndDate DATETIMEOFFSET,
	@ProductGuid NVARCHAR(MAX),
	@ManagerCompanyGuid NVARCHAR(MAX),
	@OwnerCompanyGuid NVARCHAR(MAX)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	--Site-specific settings
	DECLARE @SiteGroupLevelVolumeUnitIndex INT
	DECLARE @SiteGroupLevelVolumeDecimalPlaces INT
	DECLARE @SiteGroupLevelDensityUnitIndex INT
	DECLARE @SiteGroupLevelDensityDecimalPlaces INT

	SELECT @SiteGroupLevelVolumeUnitIndex = ISNULL(VolumeUnitIndex, 46),
		@SiteGroupLevelVolumeDecimalPlaces = ISNULL(VolumeDecimalPlaces, 0),
		@SiteGroupLevelDensityUnitIndex = ISNULL(DensityUnitIndex, 187),
		@SiteGroupLevelDensityDecimalPlaces = ISNULL(DensityDecimalPlaces, 0)
	FROM tblSites 
	WHERE SiteGuid = @SiteGuid
	--

	select T.InventoryDate,  
	dbo.udf_ConvertFromSIUnits(ABS(ISNULL(TLI.GrossQuantity,0.0)), @SiteGroupLevelVolumeUnitIndex, @SiteGroupLevelVolumeDecimalPlaces) AS GrossQuantity, 
	dbo.udf_ConvertFromSIUnits(ABS(ISNULL(TLI.NetQuantity,0.0)), @SiteGroupLevelVolumeUnitIndex, @SiteGroupLevelVolumeDecimalPlaces)  AS NetQuantity,    
	T.OwnerID,  
	T.SupplierID, 
	TLI.Product  
	from tblTransactions T 
	join tblTransactionLineItems TLI on TLI.TransactionGuid = T.TransactionGuid 
	WHERE T.SiteGuid = @SiteGuid 
	AND T.AliasName = 'Receipt'
	AND T.InventoryDate BETWEEN @StartDate AND @EndDate 
	AND TLI.ProductGuid IN (SELECT c.Guid FROM rpt.udf_GetTableFromStringList(@ProductGuid) c) 
	AND T.ManagerCompanyGuid IN (SELECT c.Guid FROM rpt.udf_GetTableFromStringList(@ManagerCompanyGuid) c) 
	AND T.OwnerCompanyGuid IN (SELECT c.Guid FROM rpt.udf_GetTableFromStringList(@OwnerCompanyGuid) c) 
	--order by InventoryDate, OwnerID 
END