-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : erv.tblEntityRecordVersioningFieldConfig
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ServerSelectUpdateConflicts_tblEntityRecordVersioningFieldConfig]
@FieldConfigGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [erv].[tblEntityRecordVersioningFieldConfig].[FieldConfigGuid],[erv].[tblEntityRecordVersioningFieldConfig].[EntitySegmentTemplateGuid],[erv].[tblEntityRecordVersioningFieldConfig].[SiteGroupGuid],[erv].[tblEntityRecordVersioningFieldConfig].[TargetField],[erv].[tblEntityRecordVersioningFieldConfig].[IsExternalAttribute],[erv].[tblEntityRecordVersioningFieldConfig].[InternalFieldName],[erv].[tblEntityRecordVersioningFieldConfig].[FilterValueGuid],[erv].[tblEntityRecordVersioningFieldConfig].[FilterValueName],[erv].[tblEntityRecordVersioningFieldConfig].[InheritedControlMode],[erv].[tblEntityRecordVersioningFieldConfig].[ForwardControlMode],[erv].[tblEntityRecordVersioningFieldConfig].[CreatedDate],[erv].[tblEntityRecordVersioningFieldConfig].[CreatedBy],[erv].[tblEntityRecordVersioningFieldConfig].[UpdatedDate],[erv].[tblEntityRecordVersioningFieldConfig].[UpdatedBy], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [erv].[tblEntityRecordVersioningFieldConfig]
            INNER JOIN [track].[tblEntityRecordVersioningFieldConfig] CT
                ON CT.PK_FieldConfigGuid = [erv].[tblEntityRecordVersioningFieldConfig].[FieldConfigGuid]
        WHERE CT.PK_FieldConfigGuid = @FieldConfigGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
