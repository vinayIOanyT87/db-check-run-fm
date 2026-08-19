
CREATE PROCEDURE [sync].[usp_SyncTableSave](
	@IdentityGuid uniqueidentifier = NULL
	,@SyncDependencyGroupGuid uniqueidentifier
	,@TableName nvarchar(1024)
	,@LastSchemaDate DateTimeOffset(7)
	,@IsSiteFilteredFlag bit
	,@IsSiteFilteredOnDeleteFlag bit
	,@ParentSyncTableGuid uniqueidentifier
	,@ParentForeignKeyColumnName nvarchar(512)
	,@CreatedBy nvarchar(100)
	,@UpdatedBy nvarchar(100)
	,@NewRowGuid uniqueidentifier out
)
AS
BEGIN
	DECLARE @nextSeq int
	
	SET @NewRowGuid = NULL;
	
	IF (@IdentityGuid IS NULL)
		SET @NewRowGuid = newid();
	ELSE
		SET @NewRowGuid = @IdentityGuid;
	
    ;   
    MERGE [sync].[tblSyncTable] AS existing
    USING (SELECT @NewRowGuid
					,@SyncDependencyGroupGuid
					,@TableName
					,@LastSchemaDate
					,@IsSiteFilteredFlag
					,@IsSiteFilteredOnDeleteFlag
					,@ParentSyncTableGuid
					,@ParentForeignKeyColumnName
					,@CreatedBy
					,@UpdatedBy
            ) AS updates (SyncTableGuid
							,SyncDependencyGroupGuid
							,TableName
							,LastSchemaDate
							,IsSiteFilteredFlag
							,IsSiteFilteredOnDeleteFlag
							,ParentSyncTableGuid
							,ParentForeignKeyColumnName
							,CreatedBy
							,UpdatedBy)
    ON (existing.SyncTableGuid = updates.SyncTableGuid)
    WHEN Matched
    THEN
        UPDATE SET SyncDependencyGroupGuid = updates.SyncDependencyGroupGuid
					,TableName = updates.TableName
					,LastSchemaDate = updates.LastSchemaDate
					,IsSiteFilteredFlag = updates.IsSiteFilteredFlag
					,IsSiteFilteredOnDeleteFlag = updates.IsSiteFilteredOnDeleteFlag
					,ParentSyncTableGuid = updates.ParentSyncTableGuid
					,ParentForeignKeyColumnName = updates.ParentForeignKeyColumnName
					,UpdatedDate = SYSDATETIMEOFFSET()
					,UpdatedBy = updates.UpdatedBy
    WHEN Not Matched
    THEN
        INSERT (SyncTableGuid
				,SyncDependencyGroupGuid
				,TableName
				,LastSchemaDate
				,IsSiteFilteredFlag
				,IsSiteFilteredOnDeleteFlag
				,ParentSyncTableGuid
				,ParentForeignKeyColumnName
				,CreatedDate
				,CreatedBy
				,UpdatedDate
				,UpdatedBy)
            VALUES (@NewRowGuid
					,@SyncDependencyGroupGuid
					,@TableName
					,@LastSchemaDate
					,@IsSiteFilteredFlag
					,@IsSiteFilteredOnDeleteFlag
					,@ParentSyncTableGuid
					,@ParentForeignKeyColumnName
					,SYSDATETIMEOFFSET()
					,CreatedBy
					,SYSDATETIMEOFFSET()
					,UpdatedBy)
    ;
	
	RETURN;
END