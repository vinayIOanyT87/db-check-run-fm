CREATE PROCEDURE [map].[gsp_EntityReportConfigurationSettingsToSiteDeleteByMapToSiteGuid](@MapToSiteGuid uniqueidentifier,@DetachOnly BIT=NULL,@SwapToGuid uniqueidentifier=NULL,@_RowVersion BINARY(8)=NULL)
AS
BEGIN 
	BEGIN TRY
		SET NOCOUNT ON; 
		IF @_RowVersion IS NOT NULL 
		BEGIN 
			 IF NOT EXISTS(SELECT 1 FROM [map].[tblEntityReportConfigurationSettingsToSite] WHERE [MapToSiteGuid] = @MapToSiteGuid AND [_RowVersion]=@_RowVersion) 
			 BEGIN 
				 RAISERROR('Attempted to delete a stale version of EntityReportConfigurationSettingsToSite.',16,1); 
				 RETURN; 
			END 
		END 
		--
		-- REPLACE MapToSiteGuid PER @SwapToGuid IF @SwapToGuid IS NOT NULL
		--
		IF NOT @SwapToGuid IS NULL
		BEGIN
			UPDATE [map].[tblEntityReportConfigurationSettingsToSite]
			SET [MapToSiteGuid]=@SwapToGuid
			WHERE [MapToSiteGuid] = @MapToSiteGuid;
		END
		ELSE
		BEGIN
			IF ISNULL(@DetachOnly,0) = 1
			BEGIN
				UPDATE [map].[tblEntityReportConfigurationSettingsToSite]
				SET [MapToSiteGuid]=NULL
				WHERE [MapToSiteGuid] = @MapToSiteGuid;
			END
			ELSE
			BEGIN
				DELETE [map].[tblEntityReportConfigurationSettingsToSite] WHERE [MapToSiteGuid] = @MapToSiteGuid; 
			END
		END
	END TRY
	BEGIN CATCH
		DECLARE @ErrMessage NVARCHAR(2048)
			,	@ErrNumber INT
			,	@ErrProcName NVARCHAR(126)
			,	@LineNumber INT
		
		SET @ErrMessage = ERROR_MESSAGE()
		SET	@ErrNumber = ERROR_NUMBER()
		SET @ErrProcName= ERROR_PROCEDURE()
		SET @LineNumber = ERROR_LINE()
		
		SET @ErrMessage =		'Error: ' + @ErrMessage + CHAR(13)+CHAR(10)
							+	'Number: ' + CAST(@ErrNumber AS VARCHAR(20)) + CHAR(13)+CHAR(10) 
							+	'Procedure Name: ' + ISNULL(@ErrProcName,OBJECT_NAME(@@PROCID)) + CHAR(13)+CHAR(10) 
							+	'Line Number: ' + ISNULL(CAST(@LineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10) 
		RAISERROR(@ErrMessage,16,1)
	END CATCH
END 
