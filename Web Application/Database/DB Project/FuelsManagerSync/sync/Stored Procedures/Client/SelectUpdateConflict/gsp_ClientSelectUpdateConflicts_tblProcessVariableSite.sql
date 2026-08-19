-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblProcessVariableSite
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ClientSelectUpdateConflicts_tblProcessVariableSite]
@ProcessVariableSiteGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [dbo].[tblProcessVariableSite].[ProcessVariableSiteGuid],[dbo].[tblProcessVariableSite].[LookupProcessVariableTypeIndex],[dbo].[tblProcessVariableSite].[InstanceNumber],[dbo].[tblProcessVariableSite].[SiteGuid],[dbo].[tblProcessVariableSite].[OPCConnectionGuid],[dbo].[tblProcessVariableSite].[OPCItemID],[dbo].[tblProcessVariableSite].[DataType],[dbo].[tblProcessVariableSite].[ServerEngineeringUnitsIndex],[dbo].[tblProcessVariableSite].[Quality],[dbo].[tblProcessVariableSite].[SIValue],[dbo].[tblProcessVariableSite].[LookupSIValueVariantTypeIndex],[dbo].[tblProcessVariableSite].[DateTimeStamp],[dbo].[tblProcessVariableSite].[Maximum],[dbo].[tblProcessVariableSite].[LookupMaximumVariantTypeIndex],[dbo].[tblProcessVariableSite].[Minimum],[dbo].[tblProcessVariableSite].[LookupMinimumVariantTypeIndex],[dbo].[tblProcessVariableSite].[DataTypeEnabled],[dbo].[tblProcessVariableSite].[Input],[dbo].[tblProcessVariableSite].[InputEnabled],[dbo].[tblProcessVariableSite].[MessageApplicationStringGuid],[dbo].[tblProcessVariableSite].[CreatedDate],[dbo].[tblProcessVariableSite].[CreatedBy],[dbo].[tblProcessVariableSite].[UpdatedDate],[dbo].[tblProcessVariableSite].[UpdatedBy], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [dbo].[tblProcessVariableSite]
            INNER JOIN [track].[tblProcessVariableSite] CT
                ON CT.PK_ProcessVariableSiteGuid = [dbo].[tblProcessVariableSite].[ProcessVariableSiteGuid]
        WHERE CT.PK_ProcessVariableSiteGuid = @ProcessVariableSiteGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
