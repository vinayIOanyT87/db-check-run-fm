
CREATE PROCEDURE [sync].[usp_SyncTableToScopeMapColumnSave](
	@IdentityGuid uniqueidentifier = NULL
	,@SyncTableToScopeMapGuid uniqueidentifier = NULL
	,@ColumnName nvarchar(512) = NULL
	,@ColumnIndex int
	,@ColumnType nvarchar(256) = NULL
	,@ColumnSize int
	,@ColumnPrecision int
	,@ColumnScale int
	,@IsNullableFlag bit
	,@IsPrimaryKeyMemberFlag bit
	,@IsIdentityColumnFlag bit
	,@CreatedBy udtUserID
	,@UpdatedBy udtUserID
	,@NewRowGuid uniqueidentifier out
)
AS
BEGIN
	SET @NewRowGuid = NULL;
	
	IF (@IdentityGuid IS NULL)
		SET @NewRowGuid = newid();
	ELSE
		SET @NewRowGuid = @IdentityGuid;
	
    ; MERGE [sync].[tblSyncTableToScopeMapColumn] AS existing
    USING (SELECT @NewRowGuid
					,@SyncTableToScopeMapGuid
					,@ColumnName
					,@ColumnIndex
					,@ColumnType
					,@ColumnSize
					,@ColumnPrecision
					,@ColumnScale
					,@IsNullableFlag
					,@IsPrimaryKeyMemberFlag
					,@IsIdentityColumnFlag
					,@CreatedBy
					,@UpdatedBy
            ) AS updates (SyncTableToScopeMapColumnGuid
							,SyncTableToScopeMapGuid
							,ColumnName
							,ColumnIndex
							,ColumnType
							,ColumnSize
							,ColumnPrecision
							,ColumnScale
							,IsNullableFlag
							,IsPrimaryKeyMemberFlag
							,IsIdentityColumnFlag
							,CreatedBy
							,UpdatedBy)
    ON (existing.SyncTableToScopeMapColumnGuid = updates.SyncTableToScopeMapColumnGuid)
    WHEN Matched
    THEN
        UPDATE SET SyncTableToScopeMapGuid = updates.SyncTableToScopeMapGuid
					,ColumnName = updates.ColumnName
					,ColumnIndex = updates.ColumnIndex
					,ColumnType = updates.ColumnType
					,ColumnSize = updates.ColumnSize
					,ColumnPrecision = updates.ColumnPrecision
					,ColumnScale = updates.ColumnScale
					,IsNullableFlag = updates.IsNullableFlag
					,IsPrimaryKeyMemberFlag = updates.IsPrimaryKeyMemberFlag
					,IsIdentityColumnFlag = updates.IsIdentityColumnFlag
					,UpdatedDate = SYSDATETIMEOFFSET()
					,UpdatedBy = updates.UpdatedBy
    WHEN Not Matched
    THEN
        INSERT (SyncTableToScopeMapColumnGuid
				,SyncTableToScopeMapGuid
				,ColumnName
				,ColumnIndex
				,ColumnType
				,ColumnSize
				,ColumnPrecision
				,ColumnScale
				,IsNullableFlag
				,IsPrimaryKeyMemberFlag
				,IsIdentityColumnFlag
				,CreatedDate
				,CreatedBy
				,UpdatedDate
				,UpdatedBy)
            VALUES (@NewRowGuid
					,@SyncTableToScopeMapGuid
					,@ColumnName
					,@ColumnIndex
					,@ColumnType
					,@ColumnSize
					,@ColumnPrecision
					,@ColumnScale
					,@IsNullableFlag
					,@IsPrimaryKeyMemberFlag
					,@IsIdentityColumnFlag
					,SYSDATETIMEOFFSET()
					,CreatedBy
					,SYSDATETIMEOFFSET()
					,UpdatedBy)
    ;
	
	RETURN;
END