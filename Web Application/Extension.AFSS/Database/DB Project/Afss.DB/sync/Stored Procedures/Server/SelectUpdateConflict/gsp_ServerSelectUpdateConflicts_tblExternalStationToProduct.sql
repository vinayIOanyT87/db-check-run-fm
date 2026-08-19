-- ========================================================================================
-- Author:		<Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : map.tblExternalStationToProduct
-- Description:	Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ServerSelectUpdateConflicts_tblExternalStationToProduct]
@ExternalStationToProductGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [map].[tblExternalStationToProduct].[ExternalStationToProductGuid],[map].[tblExternalStationToProduct].[ExternalStationGuid],[map].[tblExternalStationToProduct].[ExternalStationProduct],[map].[tblExternalStationToProduct].[ProductGuid],[map].[tblExternalStationToProduct].[CreatedBy],[map].[tblExternalStationToProduct].[CreatedDate],[map].[tblExternalStationToProduct].[UpdatedBy],[map].[tblExternalStationToProduct].[UpdatedDate], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [map].[tblExternalStationToProduct]
            INNER JOIN [track].[tblExternalStationToProduct] CT
                ON CT.PK_ExternalStationToProductGuid = [map].[tblExternalStationToProduct].[ExternalStationToProductGuid]
        WHERE CT.PK_ExternalStationToProductGuid = @ExternalStationToProductGuid
    ORDER BY CT.UpdatedRowVersion ASC
END