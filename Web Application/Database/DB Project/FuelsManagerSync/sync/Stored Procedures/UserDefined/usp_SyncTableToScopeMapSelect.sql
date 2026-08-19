
CREATE PROCEDURE [sync].[usp_SyncTableToScopeMapSelect](
	@SyncScopeGuid uniqueidentifier
	,@IdentityGuid uniqueidentifier = NULL
	,@ID nvarchar(80) = NULL
)
AS
BEGIN
	BEGIN TRY
		IF (@SyncScopeGuid IS NULL AND @IdentityGuid IS NULL AND @ID IS NULL)
		BEGIN
			RAISERROR('A synchronization scope value must be provided: @SyncScopeGuid',16,1);
			RETURN;
		END

		SELECT SyncTableToScopeMapGuid
				,ID
				,SyncScopeGuid
				,SyncTableGuid
				,SyncOrder
				,SyncDirection
				,MaxBatchSegmentRowCount
				,MaxTransferSegmentKB
				,AdditionalFilterJoinClause
				,AdditionalFilterWhereClause
				,ClientTableNameOverride
				,CreatedDate
				,CreatedBy
				,UpdatedDate
				,UpdatedBy
				,_RowVersion
				,FirstTimeSyncOption
				FROM sync.tblSyncTableToScopeMap  WITH (NOLOCK)
				WHERE (@SyncScopeGuid IS NULL OR SyncScopeGuid = @SyncScopeGuid)
					AND (@IdentityGuid IS NULL OR SyncTableToScopeMapGuid = @IdentityGuid)
					AND (@ID IS NULL OR ID = @ID)
				ORDER BY SyncOrder
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
						+ 'Procedure Name: usp_SyncTableToScopeMapSelect' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
END