CREATE PROCEDURE [sync].[usp_SyncSessionLogSelect](
	@StartDateTimeOffset datetimeoffset = NULL,
	@EndDateTimeOffset datetimeoffset = NULL,
	@IdentityGuid uniqueidentifier = NULL,
    @SyncNodeGuid uniqueidentifier = NULL,
	@WithConflicts bit = null
)
AS
BEGIN
	BEGIN TRY
		CREATE TABLE #ScopeLogTable (SyncSessionScopeLogGuid UNIQUEIDENTIFIER, SyncSessionLogGuid UNIQUEIDENTIFIER)
		INSERT INTO #ScopeLogTable
		SELECT sssl.SyncSessionScopeLogGuid, sl.SyncSessionLogGuid
		FROM [sync].tblSyncSessionScopeLog sssl INNER JOIN sync.tblSyncSessionLog sl ON sl.SyncSessionLogGuid = sssl.SyncSessionLogGuid

		CREATE TABLE #RecordConflictTable (SyncRecordConflictGuid UNIQUEIDENTIFIER, SyncSessionScopeLogGuid UNIQUEIDENTIFIER, SyncSessionLogGuid UNIQUEIDENTIFIER)
		INSERT INTO #RecordConflictTable
		SELECT rc.SyncRecordConflictGuid, slt.SyncSessionScopeLogGuid, slt.SyncSessionLogGuid
		FROM [sync].[tblSyncRecordConflictToSyncSessionScopeLog] rc INNER JOIN #ScopeLogTable slt ON rc.SyncSessionScopeLogGuid = slt.SyncSessionScopeLogGuid

		SELECT * FROM
			(SELECT SyncSessionLogGuid
					, SyncProfileID
					, SyncRequestTypeIndex
					, SyncTransferTypeIndex
					, SyncSessionStatusIndex
					, SyncSessionStateIndex
					, SyncDateRangeStart
					, SyncDateRangeEnd
					, StartDate
					, EndDate
					, RemoteNodeGuid
					, RemoteNodeMachineName
					, CONVERT(bigint, [SyncAnchorMax]) 'SyncAnchorMax'
					, (SELECT Count(*) FROM [sync].[tblSyncRecordConflict] rc INNER JOIN #RecordConflictTable rct ON rc.SyncRecordConflictGuid = rct.SyncRecordConflictGuid
					   WHERE rct.SyncSessionLogGuid = ssl.SyncSessionLogGuid) AS Conflicts
					, CreatedDate
					, CreatedBy
					, UpdatedDate
					, UpdatedBy
					, _RowVersion
    			FROM [sync].[tblSyncSessionLog] ssl 
    			WHERE ((@IdentityGuid IS NULL) OR (@IdentityGuid IS NOT NULL AND SyncSessionLogGuid = @IdentityGuid))
				AND ((@SyncNodeGuid IS NULL) OR (@SyncNodeGuid IS NOT NULL AND RemoteNodeGuid = @SyncNodeGuid))
				AND ((@StartDateTimeOffset IS NULL) OR (@StartDateTimeOffset IS NOT NULL AND StartDate >= @StartDateTimeOffset))
				AND ((@EndDateTimeOffset IS NULL) OR (@EndDateTimeOffset IS NOT NULL AND EndDate < @EndDateTimeOffset))) SyncSessionLogs
		WHERE ((@WithConflicts IS NULL) OR (@WithConflicts = CAST(0 AS BIT)) OR (Conflicts > 0))
		ORDER BY StartDate ASC
	
		DROP TABLE #ScopeLogTable
		DROP TABLE #RecordConflictTable
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
						+ 'Procedure Name: usp_SyncSessionLogSelect' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
END
