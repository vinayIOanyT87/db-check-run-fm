-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblProcessVariableNoAdditiveInputPermissive
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ServerSelectUpdateConflicts_tblProcessVariableNoAdditiveInputPermissive]
@ProcessVariableLoadArmGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [dbo].[tblProcessVariableNoAdditiveInputPermissive].[ProcessVariableLoadArmGuid],[dbo].[tblProcessVariableNoAdditiveInputPermissive].[LookupProcessVariableTypeIndex],[dbo].[tblProcessVariableNoAdditiveInputPermissive].[InstanceNumber],[dbo].[tblProcessVariableNoAdditiveInputPermissive].[LoadArmGuid],[dbo].[tblProcessVariableNoAdditiveInputPermissive].[OPCConnectionGuid],[dbo].[tblProcessVariableNoAdditiveInputPermissive].[OPCItemID],[dbo].[tblProcessVariableNoAdditiveInputPermissive].[DataType],[dbo].[tblProcessVariableNoAdditiveInputPermissive].[ServerEngineeringUnitsIndex],[dbo].[tblProcessVariableNoAdditiveInputPermissive].[Quality],[dbo].[tblProcessVariableNoAdditiveInputPermissive].[SIValue],[dbo].[tblProcessVariableNoAdditiveInputPermissive].[LookupSIValueVariantTypeIndex],[dbo].[tblProcessVariableNoAdditiveInputPermissive].[DateTimeStamp],[dbo].[tblProcessVariableNoAdditiveInputPermissive].[Maximum],[dbo].[tblProcessVariableNoAdditiveInputPermissive].[LookupMaximumVariantTypeIndex],[dbo].[tblProcessVariableNoAdditiveInputPermissive].[Minimum],[dbo].[tblProcessVariableNoAdditiveInputPermissive].[LookupMinimumVariantTypeIndex],[dbo].[tblProcessVariableNoAdditiveInputPermissive].[DataTypeEnabled],[dbo].[tblProcessVariableNoAdditiveInputPermissive].[Input],[dbo].[tblProcessVariableNoAdditiveInputPermissive].[InputEnabled],[dbo].[tblProcessVariableNoAdditiveInputPermissive].[MessageApplicationStringGuid],[dbo].[tblProcessVariableNoAdditiveInputPermissive].[CreatedDate],[dbo].[tblProcessVariableNoAdditiveInputPermissive].[CreatedBy],[dbo].[tblProcessVariableNoAdditiveInputPermissive].[UpdatedDate],[dbo].[tblProcessVariableNoAdditiveInputPermissive].[UpdatedBy], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [dbo].[tblProcessVariableNoAdditiveInputPermissive]
            INNER JOIN [track].[tblProcessVariableNoAdditiveInputPermissive] CT
                ON CT.PK_ProcessVariableLoadArmGuid = [dbo].[tblProcessVariableNoAdditiveInputPermissive].[ProcessVariableLoadArmGuid]
        WHERE CT.PK_ProcessVariableLoadArmGuid = @ProcessVariableLoadArmGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
