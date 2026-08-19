-- ========================================================================================
-- Author:		<Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : lookup.tblGasboyAuthType
-- Description:	Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ClientSelectUpdateConflicts_tblGasboyAuthType]
@GasboyAuthTypeIndex int
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [lookup].[tblGasboyAuthType].[GasboyAuthTypeIndex],[lookup].[tblGasboyAuthType].[GasboyAuthTypeCode],[lookup].[tblGasboyAuthType].[GasboyAuthTypeName],[lookup].[tblGasboyAuthType].[GasboyAuthTypeGuid],[lookup].[tblGasboyAuthType].[CreatedBy],[lookup].[tblGasboyAuthType].[CreatedDate],[lookup].[tblGasboyAuthType].[UpdatedBy],[lookup].[tblGasboyAuthType].[UpdatedDate], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [lookup].[tblGasboyAuthType]
            INNER JOIN [track].[tblGasboyAuthType] CT
                ON CT.PK_GasboyAuthTypeIndex = [lookup].[tblGasboyAuthType].[GasboyAuthTypeIndex]
        WHERE CT.PK_GasboyAuthTypeIndex = @GasboyAuthTypeIndex
    ORDER BY CT.UpdatedRowVersion ASC
END