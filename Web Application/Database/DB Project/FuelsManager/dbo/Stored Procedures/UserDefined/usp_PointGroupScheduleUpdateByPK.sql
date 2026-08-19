CREATE PROCEDURE [dbo].[usp_PointGroupScheduleUpdateByPK]
	@PointGroupScheduleGuid UNIQUEIDENTIFIER,
	@PointGroupGuid UNIQUEIDENTIFIER,
	@SiteGuid UNIQUEIDENTIFIER,
	@UserGuid UNIQUEIDENTIFIER,
	@CronSchedule nvarchar(100),
	@StartSchedule datetime,
	@EndSchedule nvarchar(100),
	@Printer nvarchar(100),
	@EmailTo nvarchar(100),
	@Layout int,
	@ExportFileFormat int,
	@CreateNewExportFile bit,
	@FitToPage bit,
	@ID nvarchar(100),
	@UpdatedDate datetimeoffset,
	@UpdatedBy dbo.udtUserID
AS
BEGIN
	SET NOCOUNT ON

	BEGIN TRY

		UPDATE tblPointGroupSchedule
		SET PointGroupGuid = @PointGroupGuid,
			CronSchedule = @CronSchedule,
			StartSchedule = @StartSchedule,
			EndSchedule = @EndSchedule,
			Printer = @Printer,
			EmailTo = @EmailTo,
			Layout = @Layout,
			ExportFileFormat = @ExportFileFormat,
			CreateNewExportFile = @CreateNewExportFile,
			FitToPage = @FitToPage,
			UserGuid = @UserGuid,
			UpdatedBy = @UpdatedBy,
			UpdatedDate = @UpdatedDate,
			SiteGuid = @SiteGuid
			WHERE PointGroupScheduleGuid = @PointGroupScheduleGuid

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
						+ 'Procedure Name: usp_PointGroupUpdateByPK' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1); 
	END CATCH

END
GO