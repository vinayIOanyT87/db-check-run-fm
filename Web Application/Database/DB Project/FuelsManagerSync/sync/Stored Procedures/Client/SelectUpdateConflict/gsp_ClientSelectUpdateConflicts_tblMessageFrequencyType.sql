-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : lookup.tblMessageFrequencyType
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ClientSelectUpdateConflicts_tblMessageFrequencyType]
@MessageFrequencyTypeIndex int
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [lookup].[tblMessageFrequencyType].[MessageFrequencyTypeIndex],[lookup].[tblMessageFrequencyType].[MessageFrequencyTypeCode],[lookup].[tblMessageFrequencyType].[MessageFrequencyTypeName],[lookup].[tblMessageFrequencyType].[MessageFrequencyTypeGuid],[lookup].[tblMessageFrequencyType].[CreatedDate],[lookup].[tblMessageFrequencyType].[CreatedBy],[lookup].[tblMessageFrequencyType].[UpdatedDate],[lookup].[tblMessageFrequencyType].[UpdatedBy], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [lookup].[tblMessageFrequencyType]
            INNER JOIN [track].[tblMessageFrequencyType] CT
                ON CT.PK_MessageFrequencyTypeIndex = [lookup].[tblMessageFrequencyType].[MessageFrequencyTypeIndex]
        WHERE CT.PK_MessageFrequencyTypeIndex = @MessageFrequencyTypeIndex
    ORDER BY CT.UpdatedRowVersion ASC
END
