CREATE PROCEDURE [sync].[usp_SyncTableToScopeMapSelectByTableListForProfile](
	@SyncProfileGuid uniqueidentifier = NULL
	,@TableNames nvarchar(max)
)
AS
BEGIN
	BEGIN TRY
		IF (@SyncProfileGuid IS NULL AND @TableNames IS NULL)
		BEGIN
			RAISERROR('The SyncProfileGuid and Table Names must be provided: @SyncProfileGuid, @TableNames',16,1);
			RETURN;
		END

		DECLARE @TableName AS TABLE
		(
			TableName nvarchar(512)
		)

		SET NOCOUNT ON;
		INSERT INTO @TableName SELECT * FROM [dbo].[udf_SplitString](@TableNames, ',', 0)
		SET NOCOUNT OFF;

		SELECT [sync].[tblSyncTableToScopeMap].[SyncTableToScopeMapGuid] 'SyncTableToScopeMapGuid'
			  ,[sync].[tblSyncTableToScopeMap].[ID] 'ID'
			  ,[sync].[tblSyncTableToScopeMap].[SyncScopeGuid] 'SyncScopeGuid'
			  ,[sync].[tblSyncTableToScopeMap].[SyncTableGuid] 'SyncTableGuid'
			  ,[sync].[tblSyncTableToScopeMap].[SyncOrder] 'SyncOrder'
			  ,[sync].[tblSyncTableToScopeMap].[SyncDirection] 'SyncDirection'
			  ,[sync].[tblSyncTableToScopeMap].[MaxBatchSegmentRowCount] 'MaxBatchSegmentRowCount'
			  ,[sync].[tblSyncTableToScopeMap].[MaxTransferSegmentKB] 'MaxTransferSegmentKB'
			  ,[sync].[tblSyncTableToScopeMap].[AdditionalFilterJoinClause] 'AdditionalFilterJoinClause'
			  ,[sync].[tblSyncTableToScopeMap].[AdditionalFilterWhereClause] 'AdditionalFilterWhereClause'
			  ,[sync].[tblSyncTableToScopeMap].[CreatedDate] 'CreatedDate'
			  ,[sync].[tblSyncTableToScopeMap].[CreatedBy] 'CreatedBy'
			  ,[sync].[tblSyncTableToScopeMap].[UpdatedDate] 'UpdatedDate'
			  ,[sync].[tblSyncTableToScopeMap].[UpdatedBy] 'UpdatedBy'
			  ,[sync].[tblSyncTableToScopeMap].[_RowVersion] '_RowVersion'
			  ,[sync].[tblSyncTableToScopeMap].[ClientTableNameOverride] 'ClientTableNameOverride'
			  ,[sync].[tblSyncTableToScopeMap].[FirstTimeSyncOption] 'FirstTimeSyncOption'
		  FROM [sync].[tblSyncTableToScopeMap]
				INNER JOIN [sync].[tblSyncScope]
					ON [sync].[tblSyncTableToScopeMap].[SyncScopeGuid] = [sync].[tblSyncScope].[SyncScopeGuid]
				INNER JOIN [sync].[tblSyncTable]
					ON [sync].[tblSyncTableToScopeMap].[SyncTableGuid] = [sync].[tblSyncTable].[SyncTableGuid]
				INNER JOIN @TableName t
					ON [sync].[tblSyncTable].[TableName] = t.TableName
			WHERE [sync].[tblSyncScope].[SyncProfileGuid] = @syncProfileGuid
			ORDER BY [sync].[tblSyncScope].[SyncOrder], [sync].[tblSyncTableToScopeMap].[SyncOrder]
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
						+ 'Procedure Name: usp_SyncTableToScopeMapSelectByTableListForProfile' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,16,1);      
	END CATCH    
END