CREATE PROCEDURE [dbo].[usp_FCEDeviceGetbyIMEI]
	@IMEI nchar(15)
AS
BEGIN
	------------------------------------------------------------------------------------------------------
	-- Stored procedure: [dbo].[usp_FCEDeviceGetbyIMEI]
	-- Author: DBA - Auto generated
	-- Version/Date: 1.0.001 / 2014-02-05 15:58:32.0032767 -05:00
	------------------------------------------------------------------------------------------------------

SET NOCOUNT ON;
BEGIN TRY
	SELECT fm.[FCEDeviceGuid]
	,	fm.[SiteGuid]
	,	fm.[ImeiNumber]
	,	fm.[FriendlyName]
	,	fm.[HeartbeatTimeoutProcessed]
	,	fm.[ConfigReady]
	,	fm.[MinTime]
	,	fm.[MaxTime]
	,	fm.[LevelDeadband]
	,	fm.[TempDeadband]
	,	fm.[Heartbeat]
	,	fm.[TLStanks]
	,	fm.[ModbusMap]
	,	fm.[MidnightOffset]
	,	fm.[ShortDeadband]
	,	fm.[ShortTime]
	,	fm.[LongDeadband]
	,	fm.[LongTime] FROM [dbo].[tblFCEDevice] fm WHERE [ImeiNumber]=@IMEI
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
						+ 'Procedure Name: usp_FCEDeviceGetbyIMEI' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
END