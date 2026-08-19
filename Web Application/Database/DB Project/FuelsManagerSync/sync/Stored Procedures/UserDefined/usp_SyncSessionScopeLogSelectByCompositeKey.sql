CREATE PROCEDURE [sync].[usp_SyncSessionScopeLogSelectByCompositeKey](
    @SyncSessionLogGuid uniqueidentifier = NULL
	,@SiteGuid uniqueidentifier = NULL
	,@ScopeID nvarchar(80) = NULL
)
AS
BEGIN
	BEGIN TRY
        SELECT SyncSessionScopeLogGuid
                ,SyncSessionLogGuid
                ,SiteGuid
                ,SiteTypeIndex
                ,ScopeID
                ,SyncSessionStatusIndex
                ,SyncSessionStateIndex
                ,StartDate
                ,EndDate
				,TableCount
				,TableSuccessCount
				,TableErrorCount
				,TotalChangesCount
				,TotalChangesAppliedCount
				,TotalChangesFailedCount
				,TotalChangesPendingCount
				,TotalDeleteCount
				,TotalInsertCount
				,TotalUpdateCount
				,BatchFileName
                ,CreatedDate
                ,CreatedBy
                ,UpdatedDate
                ,UpdatedBy
                ,_RowVersion
    		FROM [sync].[tblSyncSessionScopeLog] WITH (NOLOCK)
    		WHERE (@SyncSessionLogGuid IS NOT NULL AND SyncSessionLogGuid = @SyncSessionLogGuid)
				AND (@SiteGuid IS NULL OR (@SiteGuid IS NOT NULL AND SiteGuid = @SiteGuid))
				AND (@ScopeID IS NOT NULL AND ScopeID = @ScopeID)
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
						+ 'Procedure Name: usp_usp_SyncSessionScopeLogSelectByCompositeKey' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
END
