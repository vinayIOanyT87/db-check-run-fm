-- ========================================================================================
-- Author:		<Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : lookup.tblExternalStationLogType
-- Description:	Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ServerSelectUpdateConflicts_tblExternalStationLogType]
@ExternalStationLogTypeIndex int
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [lookup].[tblExternalStationLogType].[ExternalStationLogTypeIndex],[lookup].[tblExternalStationLogType].[ExternalStationLogTypeCode],[lookup].[tblExternalStationLogType].[ExternalStationLogTypeName],[lookup].[tblExternalStationLogType].[ExternalStationLogTypeGuid],[lookup].[tblExternalStationLogType].[CreatedBy],[lookup].[tblExternalStationLogType].[CreatedDate],[lookup].[tblExternalStationLogType].[UpdatedBy],[lookup].[tblExternalStationLogType].[UpdatedDate], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [lookup].[tblExternalStationLogType]
            INNER JOIN [track].[tblExternalStationLogType] CT
                ON CT.PK_ExternalStationLogTypeIndex = [lookup].[tblExternalStationLogType].[ExternalStationLogTypeIndex]
        WHERE CT.PK_ExternalStationLogTypeIndex = @ExternalStationLogTypeIndex
    ORDER BY CT.UpdatedRowVersion ASC
END