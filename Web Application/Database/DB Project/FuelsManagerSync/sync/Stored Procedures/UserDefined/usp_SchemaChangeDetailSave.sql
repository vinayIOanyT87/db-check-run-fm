

CREATE PROCEDURE [sync].[usp_SchemaChangeDetailSave](
	@IdentityGuid uniqueidentifier = NULL
    ,@SchemaChangeHistoryGuid uniqueidentifier
    ,@SchemaObjectTypeIndex bigint
    ,@SchemaName nvarchar(64)
    ,@ObjectName nvarchar(512)
    ,@CreatedDate datetimeoffset(7)
	,@CreatedBy nvarchar(100)
	,@UpdatedBy nvarchar(100)
	,@NewRowGuid uniqueidentifier out
)
AS
BEGIN
	SET @NewRowGuid = NULL;

	IF (@IdentityGuid IS NULL)
		SET @NewRowGuid = newid();
	ELSE
		SET @NewRowGuid = @IdentityGuid;

    ;   
    MERGE [sync].[tblSchemaChangeDetail] AS existing
    USING (SELECT @NewRowGuid
					,@SchemaChangeHistoryGuid
					,@SchemaObjectTypeIndex
					,@SchemaName
					,@ObjectName
					,@CreatedDate
					,@CreatedBy
					,@UpdatedBy) AS updates (SchemaChangeDetailGuid
											,SchemaChangeHistoryGuid
											,SchemaObjectTypeIndex
											,SchemaName
											,ObjectName
											,CreatedDate
        									,CreatedBy
        									,UpdatedBy)
    ON (existing.SchemaChangeDetailGuid = updates.SchemaChangeDetailGuid)
    WHEN Matched
    THEN
        UPDATE SET SchemaChangeHistoryGuid = updates.SchemaChangeHistoryGuid
					,SchemaObjectTypeIndex = updates.SchemaObjectTypeIndex
					,SchemaName = updates.SchemaName
					,ObjectName = updates.ObjectName
					,UpdatedDate = SYSDATETIMEOFFSET()
					,UpdatedBy = updates.UpdatedBy
    WHEN Not Matched
    THEN
        INSERT (SchemaChangeDetailGuid
                ,SchemaChangeHistoryGuid
				,SchemaObjectTypeIndex
				,SchemaName
				,ObjectName
				,CreatedDate
				,CreatedBy
				,UpdatedDate
				,UpdatedBy)
            VALUES (@NewRowGuid
					,@SchemaChangeHistoryGuid
					,@SchemaObjectTypeIndex
					,@SchemaName
					,@ObjectName
					,SYSDATETIMEOFFSET()
					,CreatedBy
					,SYSDATETIMEOFFSET()
					,UpdatedBy)
	;	

	RETURN;
END