CREATE PROCEDURE [dbo].[usp_GasboyStationEventInsert]
	@GasboyStationEvents dbo.GasboyStationEventType READONLY
AS
BEGIN
	SET NOCOUNT ON

	BEGIN TRY

		INSERT INTO tblGasboyStationEvent
		(
			GasboyStationEventGuid, 
			ExternalStationLogGuid,
			EventID,
			LookupGasboyEventErrorClassCodeIndex, 
			ErrorCode, 
			FleetID, 
			ObjectID, 
			LookupGasboyEventObjectTypeIndex,
			DeviceName,
			Field1,
			Field2,
			Field3,
			Field4,
			Field5,
			Field6,
			Field7,
			Field8,
			CreatedBy,
			CreatedDate,
			UpdatedBy,
			UpdatedDate
		)
		SELECT
			GasboyStationEventGuid, 
			ExternalStationLogGuid,
			EventID,
			LookupGasboyEventErrorClassCodeIndex, 
			ErrorCode, 
			FleetID, 
			ObjectID, 
			LookupGasboyEventObjectTypeIndex,
			DeviceName,
			Field1,
			Field2,
			Field3,
			Field4,
			Field5,
			Field6,
			Field7,
			Field8,
			CreatedUpdatedBy,
			SYSDATETIMEOFFSET(),
			CreatedUpdatedBy,
			SYSDATETIMEOFFSET()
		FROM @GasboyStationEvents

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
						+ 'Procedure Name: usp_GasboyStationEventInsert' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1); 
	END CATCH

END