-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : lookup.tblServiceType
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ClientSelectUpdateConflicts_tblServiceType]
@ServiceTypeIndex int
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [lookup].[tblServiceType].[ServiceTypeIndex],[lookup].[tblServiceType].[ServiceTypeCode],[lookup].[tblServiceType].[ServiceTypeName],[lookup].[tblServiceType].[ServiceTypeGuid],[lookup].[tblServiceType].[CreatedDate],[lookup].[tblServiceType].[CreatedBy],[lookup].[tblServiceType].[UpdatedDate],[lookup].[tblServiceType].[UpdatedBy], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [lookup].[tblServiceType]
            INNER JOIN [track].[tblServiceType] CT
                ON CT.PK_ServiceTypeIndex = [lookup].[tblServiceType].[ServiceTypeIndex]
        WHERE CT.PK_ServiceTypeIndex = @ServiceTypeIndex
    ORDER BY CT.UpdatedRowVersion ASC
END
