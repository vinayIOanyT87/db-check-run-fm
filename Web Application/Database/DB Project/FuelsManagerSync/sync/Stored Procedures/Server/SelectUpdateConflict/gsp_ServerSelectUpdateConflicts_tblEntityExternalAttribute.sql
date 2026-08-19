-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : erv.tblEntityExternalAttribute
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ServerSelectUpdateConflicts_tblEntityExternalAttribute]
@EntityExternalAttributeGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [erv].[tblEntityExternalAttribute].[EntityExternalAttributeGuid],[erv].[tblEntityExternalAttribute].[EntitySegmentTemplateGuid],[erv].[tblEntityExternalAttribute].[InternalFieldName],[erv].[tblEntityExternalAttribute].[RelationshipTableName],[erv].[tblEntityExternalAttribute].[RelationshipName],[erv].[tblEntityExternalAttribute].[CreatedDate],[erv].[tblEntityExternalAttribute].[CreatedBy],[erv].[tblEntityExternalAttribute].[UpdatedDate],[erv].[tblEntityExternalAttribute].[UpdatedBy], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [erv].[tblEntityExternalAttribute]
            INNER JOIN [track].[tblEntityExternalAttribute] CT
                ON CT.PK_EntityExternalAttributeGuid = [erv].[tblEntityExternalAttribute].[EntityExternalAttributeGuid]
        WHERE CT.PK_EntityExternalAttributeGuid = @EntityExternalAttributeGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
