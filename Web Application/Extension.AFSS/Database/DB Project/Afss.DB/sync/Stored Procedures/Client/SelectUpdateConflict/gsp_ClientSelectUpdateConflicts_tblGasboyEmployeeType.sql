-- ========================================================================================
-- Author:		<Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : lookup.tblGasboyEmployeeType
-- Description:	Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ClientSelectUpdateConflicts_tblGasboyEmployeeType]
@GasboyEmployeeTypeIndex int
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [lookup].[tblGasboyEmployeeType].[GasboyEmployeeTypeIndex],[lookup].[tblGasboyEmployeeType].[GasboyEmployeeTypeCode],[lookup].[tblGasboyEmployeeType].[GasboyEmployeeTypeName],[lookup].[tblGasboyEmployeeType].[GasboyEmployeeTypeGuid],[lookup].[tblGasboyEmployeeType].[CreatedBy],[lookup].[tblGasboyEmployeeType].[CreatedDate],[lookup].[tblGasboyEmployeeType].[UpdatedBy],[lookup].[tblGasboyEmployeeType].[UpdatedDate], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [lookup].[tblGasboyEmployeeType]
            INNER JOIN [track].[tblGasboyEmployeeType] CT
                ON CT.PK_GasboyEmployeeTypeIndex = [lookup].[tblGasboyEmployeeType].[GasboyEmployeeTypeIndex]
        WHERE CT.PK_GasboyEmployeeTypeIndex = @GasboyEmployeeTypeIndex
    ORDER BY CT.UpdatedRowVersion ASC
END