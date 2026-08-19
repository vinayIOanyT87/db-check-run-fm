
CREATE PROCEDURE [sync].[usp_SyncTableToScopeMapColumnSelect](
	@SyncTableToScopeMapGuid uniqueidentifier = NULL
	,@IdentityGuid uniqueidentifier = NULL
	,@ColumnName nvarchar(512) = NULL
)
AS
BEGIN
	BEGIN TRY
		IF (@IdentityGuid IS NULL AND @SyncTableToScopeMapGuid IS NULL AND @ColumnName IS NOT NULL)
		BEGIN
			RAISERROR('A synchronization scope mapping value must be provided when a ColumnName is specified: @SyncTableToScopeMapGuid',16,1);
			RETURN;
		END

		IF (@IdentityGuid IS NOT NULL)
		BEGIN
			SELECT SyncTableToScopeMapColumnGuid
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
					,UpdatedBy
					,_RowVersion
					FROM sync.tblSyncTableToScopeMapColumn  WITH (NOLOCK)
					WHERE SyncTableToScopeMapColumnGuid = @IdentityGuid
					ORDER BY ColumnIndex
			END
			ELSE 
			BEGIN
				SELECT SyncTableToScopeMapColumnGuid
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
						,UpdatedBy
						,_RowVersion
						FROM sync.tblSyncTableToScopeMapColumn  WITH (NOLOCK)
						WHERE SyncTableToScopeMapGuid = @SyncTableToScopeMapGuid
						AND (@ColumnName IS NULL OR ColumnName = @ColumnName)
						ORDER BY ColumnIndex
			END
	END TRY
	BEGIN CATCH        
		DECLARE	@_ErrMessage NVARCHAR(2048)      
				, @_ErrNumber INT           
				, @_ErrProcName NVARCHAR(126)           
				, @_ErrLineNumber INT;            
		SET @_ErrMessage = ERROR_MESSAGE();        
		SET @_ErrNumber = ERROR_NUMBER();        
		SET @_ErrProcName= ERROR_PROCEDURE();        
		SET @_ErrLineNumber = ERROR_LINE();            
		SET @_ErrMessage = 'Error: ' + @_ErrMessage + CHAR(13)+CHAR(10)                 
						+ 'Number: ' + CAST(@_ErrNumber AS VARCHAR(20)) + CHAR(13)+CHAR(10)                 
						+ 'Procedure Name: usp_SyncTableToScopeMapColumnSelect' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
END