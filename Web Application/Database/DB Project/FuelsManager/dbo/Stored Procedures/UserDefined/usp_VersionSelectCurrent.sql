
CREATE PROCEDURE [dbo].[usp_VersionSelectCurrent]
AS
BEGIN
	BEGIN TRY
		-- Get version from database tblVersion table.  There may be many rows in the tblVersion table:
		-- we're only interested in the most recent (the highest 'VersionIndex').  This allows us to
		-- maintain a record of the versions this database has been.  Also, several different flavors
		-- of scripts can affect the schema (upgrade scripts, Deployment scripts, hotfix scripts,
		-- "Edition"-specific scripts), so we have a "PackageName" identifier to make it easier what
		-- was done to the database in what order.
        SELECT TOP 1 VersionGuid
					,VersionIndex
					,Version
					,PackageName
					,DateApplied
					,Comments
					,Check1
					,Check2
					,SyncCompletedFlag
					,RowVersionSnapshot
					,CreatedDate
					,CreatedBy
					,UpdatedDate
					,UpdatedBy
					,_RowVersion
    		FROM [dbo].[tblVersion] WITH (NOLOCK)
			ORDER BY VersionIndex DESC
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
						+ 'Procedure Name: usp_VersionSelectCurrent' + CHAR(13)+CHAR(10)
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);
		RAISERROR(@_ErrMessage,18,1);
	END CATCH
END
