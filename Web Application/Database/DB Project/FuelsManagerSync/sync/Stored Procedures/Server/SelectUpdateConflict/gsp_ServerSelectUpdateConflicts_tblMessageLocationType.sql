-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : lookup.tblMessageLocationType
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ServerSelectUpdateConflicts_tblMessageLocationType]
@MessageLocationTypeIndex int
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [lookup].[tblMessageLocationType].[MessageLocationTypeIndex],[lookup].[tblMessageLocationType].[MessageLocationTypeCode],[lookup].[tblMessageLocationType].[MessageLocationTypeName],[lookup].[tblMessageLocationType].[MessageLocationTypeGuid],[lookup].[tblMessageLocationType].[CreatedDate],[lookup].[tblMessageLocationType].[CreatedBy],[lookup].[tblMessageLocationType].[UpdatedDate],[lookup].[tblMessageLocationType].[UpdatedBy], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [lookup].[tblMessageLocationType]
            INNER JOIN [track].[tblMessageLocationType] CT
                ON CT.PK_MessageLocationTypeIndex = [lookup].[tblMessageLocationType].[MessageLocationTypeIndex]
        WHERE CT.PK_MessageLocationTypeIndex = @MessageLocationTypeIndex
    ORDER BY CT.UpdatedRowVersion ASC
END
