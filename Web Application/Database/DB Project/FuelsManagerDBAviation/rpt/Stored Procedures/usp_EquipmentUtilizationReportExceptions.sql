-- =============================================
-- Author:		Gregory Lybanon
-- Create date: 10/18/2018
-- Description:	Stored procedure for use with Equipment Utilization SSRS Report
-- =============================================
CREATE PROCEDURE [rpt].[usp_EquipmentUtilizationReportExceptions]
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

    -- Insert statements for procedure here
	select T.InventoryDate, TLI.MeterID, 
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
	and T.[Site] = @Site 
	and TLI.[Product] = @Product 
	and (TLI.MeterStartDateTime IS NULL AND TLI.MeterStopDateTime IS NULL)
	and T.InventoryDate BETWEEN CAST(@StartTime AS DATE) AND CAST(@StopTime AS DATE) 
	order by T.CreatedDate
END