
CREATE PROCEDURE [dbo].[usp_TransactionSubLineItemsGet]
(
	@TransactionGuid UNIQUEIDENTIFIER,
	@LevelUnitIndex INT,
	@TemperatureUnitIndex INT,
	@DensityUnitIndex INT,
	@PressureUnitIndex INT,
	@FlowUnitIndex INT,
	@VolumeUnitIndex INT,
	@AdditiveVolumeUnitIndex INT,
	@MassUnitIndex INT,
	@LevelDecimalPlaces TINYINT,
	@TemperatureDecimalPlaces TINYINT,
	@DensityDecimalPlaces TINYINT,
	@PressureDecimalPlaces TINYINT,
	@FlowDecimalPlaces TINYINT,
	@VolumeDecimalPlaces TINYINT,
	@AdditiveVolumeDecimalPlaces TINYINT,
	@MassDecimalPlaces TINYINT,
	@SiteGuid UNIQUEIDENTIFIER
)
AS
BEGIN
	SET NOCOUNT ON

	-- The use of the NOLOCK hint in this procedure is questionable at best. It should be removed and any deadlocks that result should be fixed.
	SELECT tblTransactionSubLineItems.*, 
		dbo.udf_GetUnitsIndex(p.LevelUnitIndex, @LevelUnitIndex, @LevelUnitIndex) AS LevelUnitIndex,
		dbo.udf_GetUnitsIndex(p.TemperatureUnitIndex, @TemperatureUnitIndex, @TemperatureUnitIndex) AS TemperatureUnitIndex,
		dbo.udf_GetUnitsIndex(p.DensityUnitIndex, @DensityUnitIndex, @DensityUnitIndex) AS DensityUnitIndex,
		dbo.udf_GetUnitsIndex(p.PressureUnitIndex, @PressureUnitIndex, @PressureUnitIndex) AS PressureUnitIndex,
		dbo.udf_GetUnitsIndex(p.FlowUnitIndex, @FlowUnitIndex, @FlowUnitIndex) AS FlowUnitIndex,
		VolumeUnitIndex = 
			(
				CASE WHEN p.LookupProductTypeIndex = 2 
				-- A lookupProductTypeIndex of 2 means the product is an additive.
				THEN dbo.udf_GetUnitsIndex(p.VolumeUnitIndex, @AdditiveVolumeUnitIndex, @AdditiveVolumeUnitIndex)
				ELSE dbo.udf_GetUnitsIndex(p.VolumeUnitIndex, @VolumeUnitIndex, @VolumeUnitIndex) 
				END
			),
		dbo.udf_GetUnitsIndex(p.MassUnitIndex, @MassUnitIndex, @MassUnitIndex) AS MassUnitIndex,
		dbo.udf_GetDecimalPlaces(p.LevelDecimalPlaces, @LevelDecimalPlaces, @LevelDecimalPlaces) AS LevelDecimalPlaces,
		dbo.udf_GetDecimalPlaces(p.TemperatureDecimalPlaces, @TemperatureDecimalPlaces, @TemperatureDecimalPlaces) AS TemperatureDecimalPlaces,
		dbo.udf_GetDecimalPlaces(p.DensityDecimalPlaces, @DensityDecimalPlaces, @DensityDecimalPlaces) AS DensityDecimalPlaces,
		dbo.udf_GetDecimalPlaces(p.PressureDecimalPlaces, @PressureDecimalPlaces, @PressureDecimalPlaces) AS PressureDecimalPlaces,
		dbo.udf_GetDecimalPlaces(p.FlowDecimalPlaces, @FlowDecimalPlaces, @FlowDecimalPlaces) AS FlowDecimalPlaces,
		VolumeDecimalPlaces = 
			(
				CASE WHEN p.LookupProductTypeIndex = 2 
				-- A lookupProductTypeIndex of 2 means the product is an additive.
				THEN dbo.udf_GetDecimalPlaces(p.VolumeDecimalPlaces, @AdditiveVolumeDecimalPlaces, @AdditiveVolumeDecimalPlaces)
				ELSE dbo.udf_GetDecimalPlaces(p.VolumeDecimalPlaces, @VolumeDecimalPlaces, @VolumeDecimalPlaces) 
				END
			),
		dbo.udf_GetDecimalPlaces(p.MassDecimalPlaces, @MassDecimalPlaces, @MassDecimalPlaces) AS MassDecimalPlaces,
		p.VolumePackageSize AS VolumePackageSize,
		p.MassPackageSize AS MassPackageSize,
		p.IsEthanol AS IsEthanol,
		p.VcfModuleSettings AS VcfModuleSettings
	FROM tblTransactionSubLineItems WITH(NOLOCK)
    LEFT OUTER JOIN (tblProducts p WITH(NOLOCK) INNER JOIN [erv].[udf_GetProductRecordVersions](@SiteGuid) rp on p.ProductGuid = rp.ProductGuid) ON tblTransactionSubLineItems.ProductGuid = rp.MasterRecordGuid 
	WHERE tblTransactionSubLineItems.TransactionGuid = @TransactionGuid
	ORDER BY tblTransactionSubLineItems.TransactionLineItemGuid, tblTransactionSubLineItems.SequenceID, tblTransactionSubLineItems.TransactionSubLineItemGuid

END 

