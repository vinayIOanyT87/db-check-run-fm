-- ========================================================================================
-- Author:		<Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : lookup.tblGasboyTwoStageDriverValidationType
-- Description:	Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ClientSelectUpdateConflicts_tblGasboyTwoStageDriverValidationType]
@GasboyTwoStageDriverValidationTypeIndex int
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [lookup].[tblGasboyTwoStageDriverValidationType].[GasboyTwoStageDriverValidationTypeIndex],[lookup].[tblGasboyTwoStageDriverValidationType].[GasboyTwoStageDriverValidationTypeCode],[lookup].[tblGasboyTwoStageDriverValidationType].[GasboyTwoStageDriverValidationTypeName],[lookup].[tblGasboyTwoStageDriverValidationType].[GasboyTwoStageDriverValidationTypeGuid],[lookup].[tblGasboyTwoStageDriverValidationType].[CreatedBy],[lookup].[tblGasboyTwoStageDriverValidationType].[CreatedDate],[lookup].[tblGasboyTwoStageDriverValidationType].[UpdatedBy],[lookup].[tblGasboyTwoStageDriverValidationType].[UpdatedDate], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [lookup].[tblGasboyTwoStageDriverValidationType]
            INNER JOIN [track].[tblGasboyTwoStageDriverValidationType] CT
                ON CT.PK_GasboyTwoStageDriverValidationTypeIndex = [lookup].[tblGasboyTwoStageDriverValidationType].[GasboyTwoStageDriverValidationTypeIndex]
        WHERE CT.PK_GasboyTwoStageDriverValidationTypeIndex = @GasboyTwoStageDriverValidationTypeIndex
    ORDER BY CT.UpdatedRowVersion ASC
END