CREATE PROCEDURE [sync].[usp_SyncSessionLogCleanupActive]
AS
BEGIN
	BEGIN TRY

        DECLARE @SyncSessionLogGuid uniqueidentifier
        DECLARE @SyncSessionScopeLogGuid uniqueidentifier

        IF EXISTS (SELECT 1 FROM [sync].[tblSyncSessionLog] WITH (NOLOCK) WHERE StartDate IS NOT NULL AND (EndDate IS NULL OR SyncSessionStatusIndex in (0,1)))
        BEGIN
			-- An active session is determined simply based on whether or not the EndDate was set.
            DECLARE ActiveSyncSessionCursor CURSOR FOR 
                SELECT [SyncSessionLogGuid]
                    FROM [sync].[tblSyncSessionLog] WITH (NOLOCK)
		        WHERE StartDate IS NOT NULL 
						AND (EndDate IS NULL OR SyncSessionStatusIndex in (0,1))

            OPEN ActiveSyncSessionCursor
            FETCH NEXT FROM ActiveSyncSessionCursor INTO @SyncSessionLogGuid

            WHILE @@FETCH_STATUS = 0
            BEGIN
				-- If any of the SyncSessionScopeLog entries contain a "in progress" status, mark them as sysstop.  Do not update
				-- records that have a final status so we can identify how far into the session we stopped.
				DECLARE ActiveSyncSessionScopeCursor CURSOR FOR 
					SELECT [SyncSessionScopeLogGuid]
						FROM [sync].[tblSyncSessionScopeLog] WITH (NOLOCK)
					WHERE SyncSessionLogGuid = @SyncSessionLogGuid
							AND (EndDate IS NULL OR SyncSessionStatusIndex in (0,1))
				
				OPEN ActiveSyncSessionScopeCursor
				FETCH NEXT FROM ActiveSyncSessionScopeCursor INTO @SyncSessionScopeLogGuid

				WHILE @@FETCH_STATUS = 0
				BEGIN
					-- Although not required, we should cleanup the SyncSessionScopeLog entries so they contain a final status and EndDate (if needed)
					UPDATE [sync].[tblSyncSessionScopeLog] 
						SET SyncSessionStatusIndex = 6
							,EndDate = CASE WHEN EndDate IS NULL THEN sysdatetimeoffset() ELSE EndDate END
					WHERE SyncSessionScopeLogGuid = @SyncSessionScopeLogGuid

					FETCH NEXT FROM ActiveSyncSessionScopeCursor INTO @SyncSessionScopeLogGuid
				END

				CLOSE ActiveSyncSessionScopeCursor
				DEALLOCATE ActiveSyncSessionScopeCursor

                -- Most important action is to cleanup the SyncSessionLog entries so they contain a final status and EndDate.
                UPDATE [sync].[tblSyncSessionLog] 
					SET SyncSessionStateIndex = 18
						,SyncSessionStatusIndex = 6
						,EndDate = CASE WHEN EndDate IS NULL THEN sysdatetimeoffset() ELSE EndDate END
				WHERE SyncSessionLogGuid = @SyncSessionLogGuid

                -- Just to be safe, in case a SQL Process was mapped to a FuelsManager Session, we need to clean this up.  The SyncSessionLogGuid always corresponds to the SessionGuid.
                IF EXISTS (SELECT 1 FROM [map].[tblSessionToSqlProcess] WHERE [SessionGuid] = @SyncSessionLogGuid)
                BEGIN
                    DELETE FROM [map].[tblSessionToSqlProcess] WHERE [SessionGuid] = @SyncSessionLogGuid
                END

                -- Clean up the orphaned FuelsManager Session associated with this SyncSessionLog entry.
                IF EXISTS (SELECT 1 FROM [dbo].[tblSessions] WHERE [SessionGuid] = @SyncSessionLogGuid)
                BEGIN
                    DELETE FROM [dbo].[tblSessions] WHERE [SessionGuid] = @SyncSessionLogGuid
                END

	            FETCH NEXT FROM ActiveSyncSessionCursor INTO @SyncSessionLogGuid
            END

            CLOSE ActiveSyncSessionCursor
            DEALLOCATE ActiveSyncSessionCursor
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
						+ 'Procedure Name: usp_SyncSessionLogCleanupActive' + CHAR(13)+CHAR(10)
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);
		RAISERROR(@_ErrMessage,18,1);
	END CATCH
END