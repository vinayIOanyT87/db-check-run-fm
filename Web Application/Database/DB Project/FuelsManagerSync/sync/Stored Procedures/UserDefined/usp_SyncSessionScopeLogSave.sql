CREATE PROCEDURE [sync].[usp_SyncSessionScopeLogSave](
	@IdentityGuid uniqueidentifier = NULL
    ,@SyncSessionLogGuid uniqueidentifier
    ,@SiteGuid uniqueidentifier
    ,@SiteTypeIndex bigint
	,@ScopeID nvarchar(80)
    ,@SyncSessionStatusIndex bigint
    ,@SyncSessionStateIndex bigint
    ,@StartDate datetimeoffset
    ,@EndDate datetimeoffset
	,@TableCount int
	,@TableSuccessCount int
	,@TableErrorCount int
	,@TotalChangesCount int
	,@TotalChangesAppliedCount int
	,@TotalChangesFailedCount int
	,@TotalChangesPendingCount int
	,@TotalDeleteCount int
	,@TotalInsertCount int
	,@TotalUpdateCount int
	,@BatchFileName nvarchar(384)
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
	
    ;   
    MERGE [sync].[tblSyncSessionScopeLog] AS existing
    USING (SELECT @NewRowGuid
                    ,@SyncSessionLogGuid
                    ,@SiteGuid
                    ,@SiteTypeIndex
                    ,@ScopeID
                    ,@SyncSessionStatusIndex
                    ,@SyncSessionStateIndex
                    ,@StartDate
                    ,@EndDate
					,@TableCount
					,@TableSuccessCount
					,@TableErrorCount
					,@TotalChangesCount
					,@TotalChangesAppliedCount
					,@TotalChangesFailedCount
					,@TotalChangesPendingCount
					,@TotalDeleteCount
					,@TotalInsertCount
					,@TotalUpdateCount
					,@BatchFileName
					,@CreatedBy
					,@UpdatedBy
            ) AS updates (SyncSessionScopeLogGuid
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
        					,CreatedBy
        					,UpdatedBy)
    ON (existing.SyncSessionScopeLogGuid = updates.SyncSessionScopeLogGuid)
    WHEN Matched
    THEN
        UPDATE SET SyncSessionLogGuid = updates.SyncSessionLogGuid
                    ,SiteGuid = updates.SiteGuid
                    ,SiteTypeIndex = updates.SiteTypeIndex
                    ,ScopeID = updates.ScopeID
                    ,SyncSessionStatusIndex = updates.SyncSessionStatusIndex
                    ,SyncSessionStateIndex = updates.SyncSessionStateIndex
                    ,StartDate = updates.StartDate
                    ,EndDate = updates.EndDate
					,TableCount = updates.TableCount
					,TableSuccessCount = updates.TableSuccessCount
					,TableErrorCount = updates.TableErrorCount
					,TotalChangesCount = updates.TotalChangesCount
					,TotalChangesAppliedCount = updates.TotalChangesAppliedCount
					,TotalChangesFailedCount = updates.TotalChangesFailedCount
					,TotalChangesPendingCount = updates.TotalChangesPendingCount
					,TotalDeleteCount = updates.TotalDeleteCount
					,TotalInsertCount = updates.TotalInsertCount
					,TotalUpdateCount = updates.TotalUpdateCount
					,BatchFileName = updates.BatchFileName
					,UpdatedDate = SYSDATETIMEOFFSET()
					,UpdatedBy = updates.UpdatedBy
    WHEN Not Matched
    THEN
        INSERT (SyncSessionScopeLogGuid
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
				,UpdatedBy)
            VALUES (@NewRowGuid
                    ,@SyncSessionLogGuid
                    ,@SiteGuid
                    ,@SiteTypeIndex
                    ,@ScopeID
                    ,@SyncSessionStatusIndex
                    ,@SyncSessionStateIndex
                    ,@StartDate
                    ,@EndDate
					,@TableCount
					,@TableSuccessCount
					,@TableErrorCount
					,@TotalChangesCount
					,@TotalChangesAppliedCount
					,@TotalChangesFailedCount
					,@TotalChangesPendingCount
					,@TotalDeleteCount
					,@TotalInsertCount
					,@TotalUpdateCount
					,@BatchFileName
					,SYSDATETIMEOFFSET()
					,CreatedBy
					,SYSDATETIMEOFFSET()
					,UpdatedBy)
    ;
	
	RETURN;
END
