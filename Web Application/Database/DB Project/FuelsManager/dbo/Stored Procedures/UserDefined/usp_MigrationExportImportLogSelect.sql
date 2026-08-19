CREATE PROCEDURE [dbo].[usp_MigrationExportImportLogSelect](
    @IdentityGuid uniqueidentifier = NULL
	,@SiteGuid uniqueidentifier = NULL
	,@ActivityID nvarchar(30) = NULL
)
AS
BEGIN
	BEGIN TRY
        SELECT [dbo].[tblMigrationExportImportLog].[MigrationExportImportLogGuid]
				,[dbo].[tblMigrationExportImportLog].[SiteGuid]
				,[dbo].[tblMigrationExportImportLog].[ActivityID]
				,[dbo].[tblMigrationExportImportLog].[ActivityDescription]
				,[dbo].[tblMigrationExportImportLog].[ActivityStatus]
				,[dbo].[tblMigrationExportImportLog].[PerformedBy]
				,[dbo].[tblMigrationExportImportLog].[ClientIPAddress]
                ,[dbo].[tblMigrationExportImportLog].[CreatedDate]
                ,[dbo].[tblMigrationExportImportLog].[CreatedBy]
                ,[dbo].[tblMigrationExportImportLog].[UpdatedDate]
                ,[dbo].[tblMigrationExportImportLog].[UpdatedBy]
                ,[dbo].[tblMigrationExportImportLog].[_RowVersion]
				,[dbo].[tblSites].[ID] 'SiteID'
    		FROM [dbo].[tblMigrationExportImportLog] WITH (NOLOCK)
				INNER JOIN [dbo].[tblSites]
					ON [dbo].[tblMigrationExportImportLog].[SiteGuid] = [dbo].[tblSites].[SiteGuid]
    		WHERE (@IdentityGuid IS NOT NULL AND [dbo].[tblMigrationExportImportLog].[MigrationExportImportLogGuid] = @IdentityGuid)
					OR (@SiteGuid IS NOT NULL AND [dbo].[tblMigrationExportImportLog].[SiteGuid] = @SiteGuid)
					OR (@ActivityID IS NOT NULL AND [dbo].[tblMigrationExportImportLog].[ActivityID] = @ActivityID)
			ORDER BY tblMigrationExportImportLog.CreatedDate DESC
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
						+ 'Procedure Name: usp_MigrationExportImportLogSelect' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);
	END CATCH    
END
