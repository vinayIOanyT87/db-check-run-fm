-- ========================================================================================
-- Author:		<Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : lookup.tblGasboyEventObjectType
-- Description:	Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ClientSelectUpdateConflicts_tblGasboyEventObjectType]
@GasboyEventObjectTypeIndex int
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [lookup].[tblGasboyEventObjectType].[GasboyEventObjectTypeIndex],[lookup].[tblGasboyEventObjectType].[GasboyEventObjectTypeCode],[lookup].[tblGasboyEventObjectType].[GasboyEventObjectTypeName],[lookup].[tblGasboyEventObjectType].[GasboyEventObjectTypeGuid],[lookup].[tblGasboyEventObjectType].[CreatedBy],[lookup].[tblGasboyEventObjectType].[CreatedDate],[lookup].[tblGasboyEventObjectType].[UpdatedBy],[lookup].[tblGasboyEventObjectType].[UpdatedDate], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [lookup].[tblGasboyEventObjectType]
            INNER JOIN [track].[tblGasboyEventObjectType] CT
                ON CT.PK_GasboyEventObjectTypeIndex = [lookup].[tblGasboyEventObjectType].[GasboyEventObjectTypeIndex]
        WHERE CT.PK_GasboyEventObjectTypeIndex = @GasboyEventObjectTypeIndex
    ORDER BY CT.UpdatedRowVersion ASC
END