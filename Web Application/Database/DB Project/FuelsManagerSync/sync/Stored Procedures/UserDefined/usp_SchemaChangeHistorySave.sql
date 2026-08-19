

CREATE PROCEDURE [sync].[usp_SchemaChangeHistorySave](
	@IdentityGuid uniqueidentifier = NULL
    ,@Version nvarchar(80)
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
    MERGE [sync].[tblSchemaChangeHistory] AS existing
    USING (SELECT @NewRowGuid
					,@Version
					,@CreatedDate
					,@CreatedBy
					,@UpdatedBy) AS updates (SchemaChangeHistoryGuid
											,Version
											,CreatedDate
        									,CreatedBy
        									,UpdatedBy)
    ON (existing.SchemaChangeHistoryGuid = updates.SchemaChangeHistoryGuid)
    WHEN Matched
    THEN
        UPDATE SET Version = updates.Version
					,UpdatedDate = SYSDATETIMEOFFSET()
					,UpdatedBy = updates.UpdatedBy
    WHEN Not Matched
    THEN
        INSERT (SchemaChangeHistoryGuid
                ,Version
				,CreatedDate
				,CreatedBy
				,UpdatedDate
				,UpdatedBy)
            VALUES (@NewRowGuid
                    ,@Version
					,SYSDATETIMEOFFSET()
					,CreatedBy
					,SYSDATETIMEOFFSET()
					,UpdatedBy)
	;

	RETURN;
END
