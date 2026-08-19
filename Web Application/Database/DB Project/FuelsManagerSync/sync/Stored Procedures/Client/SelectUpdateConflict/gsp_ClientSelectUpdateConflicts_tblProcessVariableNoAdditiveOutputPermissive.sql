-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblProcessVariableNoAdditiveOutputPermissive
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ClientSelectUpdateConflicts_tblProcessVariableNoAdditiveOutputPermissive]
@ProcessVariableLoadArmGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [dbo].[tblProcessVariableNoAdditiveOutputPermissive].[ProcessVariableLoadArmGuid],[dbo].[tblProcessVariableNoAdditiveOutputPermissive].[LookupProcessVariableTypeIndex],[dbo].[tblProcessVariableNoAdditiveOutputPermissive].[InstanceNumber],[dbo].[tblProcessVariableNoAdditiveOutputPermissive].[LoadArmGuid],[dbo].[tblProcessVariableNoAdditiveOutputPermissive].[OPCConnectionGuid],[dbo].[tblProcessVariableNoAdditiveOutputPermissive].[OPCItemID],[dbo].[tblProcessVariableNoAdditiveOutputPermissive].[DataType],[dbo].[tblProcessVariableNoAdditiveOutputPermissive].[ServerEngineeringUnitsIndex],[dbo].[tblProcessVariableNoAdditiveOutputPermissive].[Quality],[dbo].[tblProcessVariableNoAdditiveOutputPermissive].[SIValue],[dbo].[tblProcessVariableNoAdditiveOutputPermissive].[LookupSIValueVariantTypeIndex],[dbo].[tblProcessVariableNoAdditiveOutputPermissive].[DateTimeStamp],[dbo].[tblProcessVariableNoAdditiveOutputPermissive].[Maximum],[dbo].[tblProcessVariableNoAdditiveOutputPermissive].[LookupMaximumVariantTypeIndex],[dbo].[tblProcessVariableNoAdditiveOutputPermissive].[Minimum],[dbo].[tblProcessVariableNoAdditiveOutputPermissive].[LookupMinimumVariantTypeIndex],[dbo].[tblProcessVariableNoAdditiveOutputPermissive].[DataTypeEnabled],[dbo].[tblProcessVariableNoAdditiveOutputPermissive].[Input],[dbo].[tblProcessVariableNoAdditiveOutputPermissive].[InputEnabled],[dbo].[tblProcessVariableNoAdditiveOutputPermissive].[MessageApplicationStringGuid],[dbo].[tblProcessVariableNoAdditiveOutputPermissive].[CreatedDate],[dbo].[tblProcessVariableNoAdditiveOutputPermissive].[CreatedBy],[dbo].[tblProcessVariableNoAdditiveOutputPermissive].[UpdatedDate],[dbo].[tblProcessVariableNoAdditiveOutputPermissive].[UpdatedBy], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [dbo].[tblProcessVariableNoAdditiveOutputPermissive]
            INNER JOIN [track].[tblProcessVariableNoAdditiveOutputPermissive] CT
                ON CT.PK_ProcessVariableLoadArmGuid = [dbo].[tblProcessVariableNoAdditiveOutputPermissive].[ProcessVariableLoadArmGuid]
        WHERE CT.PK_ProcessVariableLoadArmGuid = @ProcessVariableLoadArmGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
