


CREATE FUNCTION [dbo].[udf_CreateWildcardLikeValue]
(@ColumnName NVARCHAR (30), @ProductType NVARCHAR (20), @ValueToConvert FLOAT, @SiteGuid UNIQUEIDENTIFIER)
RETURNS NVARCHAR (128)
AS
begin
	declare @ResultString nvarchar(128)
	declare @ConvertedValue float

	if(@ColumnName = 'GrossQuantity' or
	   @ColumnName = 'NetQuantity' or
	   @ColumnName = 'LineFill' or
	   @ColumnName = 'BottomVolume' or	
	   @ColumnName = 'NetCapacity' or
	   @ColumnName = 'ReceiptVariance' or
	   @ColumnName = 'LoadRackVariance' or 
	   @ColumnName = 'PresetAmount')
		begin
			set @ConvertedValue = dbo.udf_ConvertToSIUnits(@ValueToConvert,dbo.udf_ProductTypeFactor(@ProductType,(SELECT dbo.tblSites.AdditiveVolumeUnitIndex FROM dbo.tblSites WHERE dbo.tblSites.SiteGuid = @SiteGuid),(SELECT dbo.tblSites.VolumeUnitIndex FROM dbo.tblSites WHERE dbo.tblSites.SiteGuid = @SiteGuid)))			
			set @ResultString = '%' + CONVERT(nvarchar(20), @ConvertedValue) + '%'
		end
	else if(@ColumnName = 'Temperature' or
			@ColumnName = 'FreezePoint')
		begin
			set @ConvertedValue = dbo.udf_ConvertToSIUnits(@ValueToConvert,(SELECT dbo.tblSites.TemperatureUnitIndex FROM dbo.tblSites WHERE dbo.tblSites.SiteGuid = @SiteGuid))
			set @ResultString = '%' + CONVERT(nvarchar(20), @ConvertedValue) + '%'
		end
	else if(@ColumnName = 'Density')
		begin
			set @ConvertedValue = dbo.udf_ConvertToSIUnits(@ValueToConvert,(SELECT dbo.tblSites.DensityUnitIndex FROM dbo.tblSites WHERE dbo.tblSites.SiteGuid = @SiteGuid))
			set @ResultString = '%' + CONVERT(nvarchar(20), @ConvertedValue) + '%'
		end
	else if(@ColumnName = 'DifferentialPressure')
		begin
			set @ConvertedValue = dbo.udf_ConvertToSIUnits(@ValueToConvert,(SELECT dbo.tblSites.PressureUnitIndex FROM dbo.tblSites WHERE dbo.tblSites.SiteGuid = @SiteGuid))
			set @ResultString = '%' + CONVERT(nvarchar(20), @ConvertedValue) + '%'
		end
	else
		begin
			set @ResultString = ''
		end

	return @ResultString 
end