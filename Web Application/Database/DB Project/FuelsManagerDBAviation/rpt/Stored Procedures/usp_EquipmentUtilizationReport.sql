-- =============================================
-- Author:		Gregory Lybanon
-- Create date: 10/18/2018
-- Description:	Stored procedure for use with Equipment Utilization SSRS Report
-- =============================================
CREATE PROCEDURE [rpt].[usp_EquipmentUtilizationReport] 
	-- Add the parameters for the stored procedure here
	@Site nvarchar(max), 
	@Product nvarchar(max), 
	@StartTime datetimeoffset,
	@StopTime datetimeoffset
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	--Used for volume conversion from SI to site-specific 
	DECLARE @SiteGroupLevelVolumeUnitIndex INT
	DECLARE @SiteGroupLevelVolumeDecimalPlaces INT

	SELECT @SiteGroupLevelVolumeUnitIndex = ISNULL(VolumeUnitIndex, 46),
		@SiteGroupLevelVolumeDecimalPlaces = ISNULL(VolumeDecimalPlaces, 0)
	FROM tblSites 
	WHERE [ID] = @Site

	--Get Timezone offset for site
	DECLARE @SiteDefaultOffsetMinutes INT
	DECLARE @MeterStartOffsetMinutes INT
	DECLARE @MeterStopOffsetMinutes INT

	--Get Site Standard
	select @SiteDefaultOffsetMinutes = TZ.OffsetMinutes 
	from lookup.tblTimeZone TZ 
	JOIN dbo.tblSites S on S.TimeZone = TZ.TimeZoneName 
	where S.ID = @Site

	--Try to get start and stop offsets regardless of whether it is *ST or *DT
	select top 1 @MeterStartOffsetMinutes = DATEPART(TZOFFSET, TLI.MeterStartDateTime)
	from [dbo].[tblTransactions] T 
	JOIN [dbo].[tblTransactionLineItems] TLI on TLI.TransactionGuid = T.TransactionGuid 
	where T.AliasName = 'Issue' 
	and T.[Site] =  @Site 
	and TLI.[Product] = @Product 
	and T.InventoryDate = CAST(@StartTime as DATE) 
	and TLI.MeterStartDateTime IS NOT NULL 
	--and (TLI.MeterStartDateTime BETWEEN @StartTime AND @StopTime 
	--OR (TLI.MeterStartDateTime < @StartTime AND TLI.MeterStopDateTime > @StartTime)) 
	ORDER BY TLI.MeterStartDateTime

	select top 1 @MeterStopOffsetMinutes = DATEPART(TZ, TLI.MeterStopDateTime)
	from [dbo].[tblTransactions] T 
	JOIN [dbo].[tblTransactionLineItems] TLI on TLI.TransactionGuid = T.TransactionGuid 
	where T.AliasName = 'Issue' 
	and T.[Site] =  @Site 
	and TLI.[Product] = @Product 
	and T.InventoryDate = CAST(@StopTime as DATE) 
	and TLI.MeterStopDateTime IS NOT NULL 
	--and (TLI.MeterStartDateTime BETWEEN @StartTime AND @StopTime 
	--OR (TLI.MeterStartDateTime < @StartTime AND TLI.MeterStopDateTime > @StartTime)) 
	ORDER BY TLI.MeterStopDateTime DESC

	SET @StartTime = DATEADD(mi,ABS(COALESCE(@MeterStartOffsetMinutes,@SiteDefaultOffsetMinutes)), @StartTime)
	SET @StopTime = DATEADD(mi,ABS(COALESCE(@MeterStopOffsetMinutes,@SiteDefaultOffsetMinutes)), @StopTime)

    -- Insert statements for procedure here
	select T.Site, T.InventoryDate, TLI.MeterID, 
	TLI.MeterStart, TLI.MeterStop, 
	dbo.udf_ConvertFromSIUnits(ABS(ISNULL(TLI.GrossQuantity,0.0)), @SiteGroupLevelVolumeUnitIndex, @SiteGroupLevelVolumeDecimalPlaces) AS GrossQuantity, 
	T.AliasName, TLI.Product, 
	T.SourceRegistrationID1, T.SourceSerialNumber1, T.SourceEquipmentType1, T.ManagerID, 
	T.CreatedDate, TLI.MeterStartDateTime, tli.MeterStopDateTime, ET.EqTypeName 
	from [dbo].[tblTransactions] T 
	JOIN [dbo].[tblTransactionLineItems] TLI on TLI.TransactionGuid = T.TransactionGuid 
	join tblEquipment E on E.EquipmentGUID = T.Source1EquipmentGUID 
	join tblEquipmentTypes ET on ET.EquipmentTypeGUID = E.EquipmentTypeGuid 
	where T.AliasName = 'Issue' 
	and T.[Site] =  @Site 
	and TLI.[Product] = @Product 
	and TLI.MeterStartDateTime IS NOT NULL 
	and TLI.MeterStopDateTime IS NOT NULL 
	and T.InventoryDate BETWEEN CAST(DATEADD(dd,-1,@StartTime) as DATE) AND CAST(DATEADD(dd,1,@StopTime) as DATE) 
	and (TLI.MeterStartDateTime >= @StartTime AND TLI.MeterStartDateTime < @StopTime 
	OR (TLI.MeterStartDateTime < @StartTime AND TLI.MeterStopDateTime > @StartTime))
	order by ET.EqTypeName, T.SourceRegistrationID1  
END
