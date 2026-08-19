CREATE PROCEDURE [dbo].[gsp_FCEDeviceInsertByPK]
(
    @FCEDeviceGuid UNIQUEIDENTIFIER=NULL OUTPUT,
	@SiteGuid UNIQUEIDENTIFIER=Null,
	@ImeiNumber nchar(15)=NULL,
	@FriendlyName nchar(30)=NULL,
	@HeartbeatTimeoutProcessed Bit=NULL,
	@ConfigReady Bit=NULL,
	@MinTime	int=NULL,
	@MaxTime	int=NULL,
	@LevelDeadband float=NULL,
	@TempDeadband float=NULL,
	@Heartbeat int=NULL,
	@TLStanks smallint=NULL,
	@ModbusMap smallint=NULL,
	@MidnightOffset int=NULL,
	@ShortDeadband float=NULL,
	@ShortTime int=NULL,
	@LongDeadband float=NULL,
	@LongTime int=NULL,
	@SoftwareVersion nchar(32)=NULL,
	@CreatedDate datetimeoffset(7)=NULL,
	@CreatedBy udtUserID=NULL,
	@UpdatedDate datetimeoffset(7)=NULL,
	@UpdatedBy udtUserID=NULL,
	@_RowVersion timestamp=NULL OUTPUT,
	@ID nvarchar(35)=NULL
)
AS
BEGIN
	------------------------------------------------------------------------------------------------------
	-- Stored procedure: [dbo].[gsp_FCEDeviceInsertByPK] 
	-- Author: DBA - Auto generated
	-- Version/Date: 1.0.001 / 2014-02-05 15:58:32.0032767 -05:00
	-- Purpose: Insert into table [dbo].[tblFCEDevice]
	-- Notes:
	------------------------------------------------------------------------------------------------------

SET NOCOUNT ON;
BEGIN TRY
	IF (@FCEDeviceGuid IS NULL)
	Begin
		SET @FCEDeviceGuid=NEWID();
	END
	SET @CreatedDate=ISNULL(@CreatedDate,sysdatetimeoffset())

	INSERT INTO [dbo].[tblFCEDevice]
	(
		[FCEDeviceGuid]
	,	[SiteGuid]
	,	[ImeiNumber]
	,	[FriendlyName]
	,	[HeartbeatTimeoutProcessed]
	,	[ConfigReady]
	,	[MinTime]
	,	[MaxTime]
	,	[LevelDeadband]
	,	[TempDeadband]
	,	[Heartbeat]
	,	[TLStanks]
	,	[ModbusMap]
	,	[MidnightOffset]
	,	[ShortDeadband]
	,	[ShortTime]
	,	[LongDeadband]
	,	[LongTime]
	,	[SoftwareVersion]
	,	[CreatedDate]
	,	[CreatedBy]
	,	[UpdatedDate]
	,	[UpdatedBy]
	)
	VALUES
	(
	@FCEDeviceGuid
	,	@SiteGuid
	,	@ImeiNumber
	,	@FriendlyName
	,	@HeartbeatTimeoutProcessed
	,	@ConfigReady
	,	@MinTime
	,	@MaxTime
	,	@LevelDeadband
	,	@TempDeadband
	,	@Heartbeat
	,	@TLStanks
	,	@ModbusMap
	,	@MidnightOffset
	,	@ShortDeadband
	,	@ShortTime
	,	@LongDeadband
	,	@LongTime
	,	@SoftwareVersion
	,	@CreatedDate
	,	@CreatedBy
	,	@UpdatedDate
	,	@UpdatedBy
	)

		SELECT @_RowVersion=_RowVersion        
		FROM [dbo].[tblFCEDevice]           
		WHERE FCEDeviceGuid=@FCEDeviceGuid;
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
						+ 'Procedure Name: gsp_FCEDeviceInsertByPK' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
END     