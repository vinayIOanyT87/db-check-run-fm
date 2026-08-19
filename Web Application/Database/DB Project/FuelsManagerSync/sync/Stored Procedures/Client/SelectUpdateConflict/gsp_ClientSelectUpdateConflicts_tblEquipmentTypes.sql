-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblEquipmentTypes
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ClientSelectUpdateConflicts_tblEquipmentTypes]
@EquipmentTypeGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [dbo].[tblEquipmentTypes].[EqTypeName],[dbo].[tblEquipmentTypes].[EqTypeDescription],[dbo].[tblEquipmentTypes].[Capacity],[dbo].[tblEquipmentTypes].[SafeFill],[dbo].[tblEquipmentTypes].[Make],[dbo].[tblEquipmentTypes].[Model],[dbo].[tblEquipmentTypes].[Year],[dbo].[tblEquipmentTypes].[CreatedDate],[dbo].[tblEquipmentTypes].[CreatedBy],[dbo].[tblEquipmentTypes].[UpdatedDate],[dbo].[tblEquipmentTypes].[UpdatedBy],[dbo].[tblEquipmentTypes].[DeleteFlag],[dbo].[tblEquipmentTypes].[IssPt],[dbo].[tblEquipmentTypes].[MultiCompartment],[dbo].[tblEquipmentTypes].[EquipmentTypeGuid],[dbo].[tblEquipmentTypes].[SiteGuid],[dbo].[tblEquipmentTypes].[LookupEquipmentTypeIndex],[dbo].[tblEquipmentTypes].[ProductGuid],[dbo].[tblEquipmentTypes].[CustomerDesignator],[dbo].[tblEquipmentTypes].[ServiceTime],[dbo].[tblEquipmentTypes].[VolumeUnits],[dbo].[tblEquipmentTypes].[VolumeDecimalPlaces],[dbo].[tblEquipmentTypes].[MassUnits],[dbo].[tblEquipmentTypes].[MassDecimalPlaces],[dbo].[tblEquipmentTypes].[WingToWingToleranceType],[dbo].[tblEquipmentTypes].[WingToWingToleranceValue],[dbo].[tblEquipmentTypes].[TankToTankToleranceType],[dbo].[tblEquipmentTypes].[TankToTankToleranceValue],[dbo].[tblEquipmentTypes].[FuelServiceToleranceType],[dbo].[tblEquipmentTypes].[FuelServiceToleranceValue],[dbo].[tblEquipmentTypes].[FuelServiceToleranceMaxType],[dbo].[tblEquipmentTypes].[FuelServiceToleranceMaxValue],[dbo].[tblEquipmentTypes].[AllowFuelingByWeight],[dbo].[tblEquipmentTypes].[LookupCompanyRoleIndex], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [dbo].[tblEquipmentTypes]
            INNER JOIN [track].[tblEquipmentTypes] CT
                ON CT.PK_EquipmentTypeGuid = [dbo].[tblEquipmentTypes].[EquipmentTypeGuid]
        WHERE CT.PK_EquipmentTypeGuid = @EquipmentTypeGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
