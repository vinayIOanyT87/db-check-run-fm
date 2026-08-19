-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblProducts
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ClientSelectUpdateConflicts_tblProducts]
@ProductGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [dbo].[tblProducts].[ProductID],[dbo].[tblProducts].[Description],[dbo].[tblProducts].[GenericType],[dbo].[tblProducts].[StockResetDate],[dbo].[tblProducts].[StockTrack],[dbo].[tblProducts].[DensityHighLimit],[dbo].[tblProducts].[DensityLowLimit],[dbo].[tblProducts].[DensityDeadband],[dbo].[tblProducts].[TemperatureHiHiLimit],[dbo].[tblProducts].[TemperatureHighLimit],[dbo].[tblProducts].[TemperatureLowLimit],[dbo].[tblProducts].[TemperatureLoLoLimit],[dbo].[tblProducts].[TemperatureDeadband],[dbo].[tblProducts].[Bonded],[dbo].[tblProducts].[LowStockWarning],[dbo].[tblProducts].[GroundFuel],[dbo].[tblProducts].[ProductCode],[dbo].[tblProducts].[Price],[dbo].[tblProducts].[AviationFuelFlag],[dbo].[tblProducts].[StandardDensity],[dbo].[tblProducts].[ApplyVolumeCorrection],[dbo].[tblProducts].[ApplyStandardDensity],[dbo].[tblProducts].[ApplyDensityLimits],[dbo].[tblProducts].[ApplyTemperatureLimits],[dbo].[tblProducts].[VolumeUnitIndex],[dbo].[tblProducts].[TemperatureUnitIndex],[dbo].[tblProducts].[DensityUnitIndex],[dbo].[tblProducts].[VolumeDecimalPlaces],[dbo].[tblProducts].[TemperatureDecimalPlaces],[dbo].[tblProducts].[DensityDecimalPlaces],[dbo].[tblProducts].[Capitalize],[dbo].[tblProducts].[OctaneNumber],[dbo].[tblProducts].[ReidVaporPressure],[dbo].[tblProducts].[HazardousMaterial],[dbo].[tblProducts].[RegulatoryClass],[dbo].[tblProducts].[LoadRackDisplayText],[dbo].[tblProducts].[ComponentTolerance],[dbo].[tblProducts].[VaporRecovery],[dbo].[tblProducts].[LockedOut],[dbo].[tblProducts].[LockedOutReason],[dbo].[tblProducts].[LockedOutDate],[dbo].[tblProducts].[VarianceTolerance],[dbo].[tblProducts].[DielectricTolerance],[dbo].[tblProducts].[LoadByWeight],[dbo].[tblProducts].[PIDXCode],[dbo].[tblProducts].[ContaminationPromptLoadRackText],[dbo].[tblProducts].[InhibitAccounting],[dbo].[tblProducts].[UserData1],[dbo].[tblProducts].[UserData2],[dbo].[tblProducts].[UserData3],[dbo].[tblProducts].[UserData4],[dbo].[tblProducts].[UserData5],[dbo].[tblProducts].[UserData6],[dbo].[tblProducts].[UserData7],[dbo].[tblProducts].[UserData8],[dbo].[tblProducts].[CreatedDate],[dbo].[tblProducts].[CreatedBy],[dbo].[tblProducts].[UpdatedDate],[dbo].[tblProducts].[UpdatedBy],[dbo].[tblProducts].[MassUnitIndex],[dbo].[tblProducts].[LevelUnitIndex],[dbo].[tblProducts].[FlowUnitIndex],[dbo].[tblProducts].[PressureUnitIndex],[dbo].[tblProducts].[MassDecimalPlaces],[dbo].[tblProducts].[LevelDecimalPlaces],[dbo].[tblProducts].[FlowDecimalPlaces],[dbo].[tblProducts].[PressureDecimalPlaces],[dbo].[tblProducts].[VolumePackageSize],[dbo].[tblProducts].[MassPackageSize],[dbo].[tblProducts].[ProductGuid],[dbo].[tblProducts].[SiteGuid],[dbo].[tblProducts].[LookupProductTypeIndex],[dbo].[tblProducts].[TrackingProductGuid],[dbo].[tblProducts].[TaxCode],[dbo].[tblProducts].[VcfModuleSettings],[dbo].[tblProducts].[ProductColor],[dbo].[tblProducts].[PatternColor],[dbo].[tblProducts].[PatternNumber],[dbo].[tblProducts].[_MasterRecordGuid],[dbo].[tblProducts].[HiddenDate],[dbo].[tblProducts].[AutomaticCloseout],[dbo].[tblProducts].[PIDXFamilyCode],[dbo].[tblProducts].[IsEthanol], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [dbo].[tblProducts]
            INNER JOIN [track].[tblProducts] CT
                ON CT.PK_ProductGuid = [dbo].[tblProducts].[ProductGuid]
        WHERE CT.PK_ProductGuid = @ProductGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
