CREATE PROCEDURE [dbo].[usp_GetVersionSchemaChangeHistory](
    @IdentityGuid uniqueidentifier = NULL
	,@Version nvarchar(80) = NULL
)
AS
BEGIN
	DECLARE @SchemaChangeCount AS TABLE
	(
		SchemaChangeHistoryGuid uniqueidentifier
		,SchemaChangeDetailCount bigint
	)

	BEGIN TRY
		INSERT INTO @SchemaChangeCount SELECT DISTINCT(SchemaChangeHistoryGuid) AS 'SchemaChangeHistoryGuid', COUNT(*) AS 'SchemaChangeDetailCount' FROM [sync].[tblSchemaChangeDetail] GROUP BY SchemaChangeHistoryGuid;

        SELECT v.VersionGuid
				,v.VersionIndex
                ,v.Version
                ,v.PackageName
                ,v.DateApplied
                ,v.SyncCompletedFlag
                ,v.RowVersionSnapshot
                ,v.CreatedDate
                ,v.CreatedBy
                ,v.UpdatedDate
                ,v.UpdatedBy
				,CASE WHEN sch.[HasSchemaChangeFlag] IS NULL THEN CONVERT(bit, 0) ELSE CONVERT(bit, 1) END 'HasSchemaChangeFlag'
    		FROM [dbo].[tblVersion] v WITH (NOLOCK)
				LEFT JOIN (SELECT [sync].[tblSchemaChangeHistory].[SchemaChangeHistoryGuid] 'SchemaChangeHistoryGuid'
									,[sync].[tblSchemaChangeHistory].[Version] 'Version'
									,CASE WHEN counts.[SchemaChangeDetailCount] IS NULL THEN CONVERT(bit, 0) ELSE CONVERT(bit, 1) END 'HasSchemaChangeFlag'
    						FROM [sync].[tblSchemaChangeHistory] WITH (NOLOCK)
								LEFT JOIN (SELECT SchemaChangeHistoryGuid, SchemaChangeDetailCount FROM @SchemaChangeCount) counts
									ON [sync].[tblSchemaChangeHistory].[SchemaChangeHistoryGuid] = counts.[SchemaChangeHistoryGuid]
    						WHERE (@IdentityGuid IS NULL AND @Version IS NULL)
									OR (@IdentityGuid IS NOT NULL AND [sync].[tblSchemaChangeHistory].[SchemaChangeHistoryGuid] = @IdentityGuid)
									OR (@Version IS NOT NULL AND [sync].[tblSchemaChangeHistory].[Version] = @Version)
			) sch
				ON v.[Version] = sch.[Version]
			ORDER BY v.Version DESC, v.CreatedDate DESC
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
						+ 'Procedure Name: usp_GetVersionSchemaChangeHistory' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);
	END CATCH    
END
