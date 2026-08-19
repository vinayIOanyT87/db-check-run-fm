CREATE PROCEDURE [dbo].[usp_GasboyStationEventGet]
	@ExternalStationLogGuid UNIQUEIDENTIFIER
AS
BEGIN
	SET NOCOUNT ON

	BEGIN TRY

		SELECT 
			dbo.tblExternalStationLog.ExternalStationLogGuid,
			dbo.tblExternalStationLog.SiteGuid,  
			dbo.tblExternalStationLog.ExternalStationGuid, 
			dbo.tblExternalStationLog.LogText, 
			dbo.tblExternalStationLog.LookupExternalStationLogTypeIndex, 
			dbo.tblExternalStationLog.CreatedBy,
			dbo.tblExternalStationLog.CreatedDate,
			dbo.tblExternalStationLog.UpdatedBy,
			dbo.tblExternalStationLog.UpdatedDate,
			dbo.tblExternalStation.ID AS ExternalStationID,
			dbo.tblGasboyStationEvent.GasboyStationEventGuid,
			dbo.tblGasboyStationEvent.EventID,
			dbo.tblGasboyStationEvent.LookupGasboyEventErrorClassCodeIndex, 
			dbo.tblGasboyStationEvent.ErrorCode, 
			dbo.tblGasboyStationEvent.FleetID, 
			dbo.tblGasboyStationEvent.ObjectID, 
			dbo.tblGasboyStationEvent.LookupGasboyEventObjectTypeIndex,
			dbo.tblGasboyStationEvent.DeviceName,
			dbo.tblGasboyStationEvent.Field1,
			dbo.tblGasboyStationEvent.Field2,
			dbo.tblGasboyStationEvent.Field3,
			dbo.tblGasboyStationEvent.Field4,
			dbo.tblGasboyStationEvent.Field5,
			dbo.tblGasboyStationEvent.Field6,
			dbo.tblGasboyStationEvent.Field7,
			dbo.tblGasboyStationEvent.Field8
		FROM tblExternalStationLog 
		INNER JOIN dbo.tblExternalStation ON dbo.tblExternalStationLog.ExternalStationGuid = dbo.tblExternalStation.ExternalStationGuid
		INNER JOIN dbo.tblGasboyStationEvent ON dbo.tblGasboyStationEvent.ExternalStationLogGuid = dbo.tblExternalStationLog.ExternalStationLogGuid
		WHERE dbo.tblExternalStationLog.ExternalStationLogGuid = @ExternalStationLogGuid
	
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
						+ 'Procedure Name: usp_GasboyStationEventGet' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1); 
	END CATCH

END
	