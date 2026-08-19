CREATE PROCEDURE [dbo].[usp_PointGroupScheduleGetByPK]
	@PointGroupScheduleGuid uniqueidentifier
AS
BEGIN
	------------------------------------------------------------------------------------------------------
	-- Stored Procedure: [dbo].[usp_PointGroupScheduleGetByPK] 
	-- Author: Francisco Martin
	-- Version/Date: 1.0
	-- Purpose: Retrieve a Point Group by Guid
	-- Notes:
	-- 1. @PointGroupScheduleGuid
	------------------------------------------------------------------------------------------------------
	BEGIN TRY	

		SELECT [PointGroupScheduleGuid]
      ,[PointGroupGuid]
      ,[UserGuid]
      ,[SiteGuid]
      ,[CronSchedule]
      ,[StartSchedule]
      ,[EndSchedule]
      ,[Printer]
      ,[EmailTo]
      ,[Layout]
		,[ExportFileFormat]
		,[CreateNewExportFile]
		,[FitToPage]
      ,[CreatedDate]
      ,[CreatedBy]
      ,[UpdatedDate]
      ,[UpdatedBy]
		FROM [dbo].[tblPointGroupSchedule] pg
		WHERE pg.[PointGroupScheduleGuid] = @PointGroupScheduleGuid


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
						+ 'Procedure Name: [dbo].[usp_PointGroupScheduleGetByPK]' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH  

END
