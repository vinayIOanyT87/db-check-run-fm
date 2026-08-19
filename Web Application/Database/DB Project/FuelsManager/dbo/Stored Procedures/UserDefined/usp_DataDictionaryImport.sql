CREATE PROCEDURE [dbo].[usp_DataDictionaryImport]
(
	@AddListTempTable DataDictionaryDataType READONLY,
	@ModListTempTable DataDictionaryDataType READONLY,
	@DelListTempTable DataDictionaryDataType READONLY,
	@ImportSiteGuid uniqueidentifier
)
AS
BEGIN
	SET NOCOUNT ON

	BEGIN TRY
		DECLARE @SiteGuid uniqueidentifier
		DECLARE @Key nvarchar(100) 
		DECLARE @Value nvarchar(100)
		DECLARE @CreatedDate datetimeoffset
		DECLARE @CreatedBy nvarchar(100)
		DECLARE @UpdatedDate datetimeoffset
		DECLARE @UpdatedBy nvarchar(100)
			
		DECLARE @HasAddData int
		DECLARE @HasModData int
		DECLARE @HasDelData int
		DECLARE @MappingCount int

		SELECT @HasAddData = COUNT(*) FROM @AddListTempTable
		SELECT @HasModData = COUNT(*) FROM @ModListTempTable
		SELECT @HasDelData = COUNT(*) FROM @DelListTempTable

		-- Make sure we are at the right site. A count of zero indicates
		-- that we are at the correct site.
		SELECT @MappingCount = COUNT(a.DataDictionaryToSiteGuid)
		FROM map.tblEntityDataDictionaryToSite a
			INNER JOIN tblSites b
			ON b.SiteGuid = a.MapToSiteGuid
			INNER JOIN tblSites c
			ON c.SiteGuid = a.AssignedFromSiteGuid
			INNER JOIN tblSites d
			ON d.SiteGuid = a.OwnerSiteGuid
		WHERE a.MapToSiteGuid = @ImportSiteGuid

		--===============================================
		-- Handle purging of a data dictionary entries
		--===============================================
		IF (@HasDelData > 0 AND @MappingCount = 0)
		BEGIN	
			DELETE dd FROM dbo.tblDataDictionaries dd INNER JOIN @DelListTempTable dlt ON dd.[Key] = dlt.[Key]
			WHERE dd.[Key] = dlt.[Key]
		END

		--==================================================
		-- Handle the inserting of new data dictionary keys.
		--==================================================
		IF (@HasAddData > 0)
		BEGIN
			DECLARE add_cursor CURSOR FOR
				SELECT SiteGuid, [Key], [Value], CreatedDate, CreatedBy, UpdatedDate, UpdatedBy 
				FROM @AddListTempTable

			OPEN add_cursor
			FETCH NEXT FROM add_cursor
			INTO @SiteGuid, @Key, @Value, @CreatedDate, @CreatedBy, @UpdatedDate, @UpdatedBy

			WHILE @@FETCH_STATUS = 0
			BEGIN
				INSERT INTO dbo.tblDataDictionaries
					(DataDictionaryGuid, SiteGuid, [Key], [Value], CreatedDate, CreatedBy, UpdatedDate, UpdatedBy)
					VALUES
					(NEWID(), @SiteGuid, @Key, @Value, @CreatedDate, @CreatedBy, @UpdatedDate, @UpdatedBy)

				FETCH NEXT FROM add_cursor
				INTO @SiteGuid, @Key, @Value, @CreatedDate, @CreatedBy, @UpdatedDate, @UpdatedBy
			END
			CLOSE add_cursor
			DEALLOCATE add_cursor
		END

		--==========================================================
		-- Handle the modification of existing data dictionary keys.
		--==========================================================
		IF (@HasModData > 0 AND @MappingCount = 0)
		BEGIN
			DECLARE mod_cursor CURSOR FOR
			SELECT SiteGuid, [Key], [Value], UpdatedDate, UpdatedBy 
			FROM @ModListTempTable

			OPEN mod_cursor
			FETCH NEXT FROM mod_cursor
			INTO @SiteGuid, @Key, @Value, @UpdatedDate, @UpdatedBy

			WHILE @@FETCH_STATUS = 0
			BEGIN
				UPDATE dbo.tblDataDictionaries SET [Value] = @Value, UpdatedBy = @UpdatedBy, UpdatedDate = @UpdatedDate
				WHERE [Key] = @Key

				FETCH NEXT FROM mod_cursor
				INTO @SiteGuid, @Key, @Value, @UpdatedDate, @UpdatedBy
			END
			CLOSE mod_cursor
			DEALLOCATE mod_cursor
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
						+ 'Procedure Name: usp_DataDictionaryImport' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
END
