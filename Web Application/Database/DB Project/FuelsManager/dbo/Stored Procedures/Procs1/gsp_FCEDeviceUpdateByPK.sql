CREATE PROCEDURE [dbo].[gsp_FCEDeviceUpdateByPK]
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
	@NullOverrideSiteGuid BIT=0,
	@NullOverrideImeiNumber BIT=0,
	@NullOverrideFriendlyName BIT=0,
	@NullOverrideHeartbeatTimeoutProcessed BIT=0,
	@NullOverrideConfigReady BIT=0,
	@NullOverrideMinTime	BIT=0,
	@NullOverrideMaxTime	BIT=0,
	@NullOverrideLevelDeadband BIT=0,
	@NullOverrideTempDeadband BIT=0,
	@NullOverrideHeartbeat BIT=0,
	@NullOverrideTLStanks BIT=0,
	@NullOverrideModbusMap BIT=0,
	@NullOverrideMidnightOffset BIT=0,
	@NullOverrideShortDeadband BIT=0,
	@NullOverrideShortTime BIT=0,
	@NullOverrideLongDeadband BIT=0,
	@NullOverrideLongTime BIT=0,
	@NullOverrideUpdatedDate BIT=0,
	@ID nvarchar(35)=NULL
)

AS
BEGIN

	------------------------------------------------------------------------------------------------------
	-- Stored procedure: [dbo].[gsp_FCEDeviceUpdateByPK] 
	-- Author: DBA - Auto generated
	-- Version/Date: 1.0.001 / 2014-02-05 15:58:32.0032767 -05:00
	-- Purpose: Update into table [dbo].[tblFCEEMapping]
	-- Notes:
	------------------------------------------------------------------------------------------------------

SET NOCOUNT ON;
BEGIN TRY
		IF @_RowVersion IS NOT NULL AND NOT EXISTS(SELECT 1 FROM [dbo].[tblFCEDevice] WHERE @FCEDeviceGuid=@FCEDeviceGuid AND _RowVersion=@_RowVersion)
		BEGIN
			RAISERROR('Attempted to modify a stale copy of the record',16,1);
			RETURN;
		END

		UPDATE [dbo].[tblFCEDevice] SET
			[SiteGuid]=(CASE ISNULL(@NullOverrideSiteGuid,0) WHEN 1 THEN @SiteGuid ELSE ISNULL(@SiteGuid,[SiteGuid]) END)
		,	[ImeiNumber]=(CASE ISNULL(@NullOverrideImeiNumber,0) WHEN 1 THEN @ImeiNumber ELSE ISNULL(@ImeiNumber,[ImeiNumber]) END)
		,	[FriendlyName]=(CASE ISNULL(@NullOverrideFriendlyName,0) WHEN 1 THEN @FriendlyName ELSE ISNULL(@FriendlyName,[FriendlyName]) END)
		,	[HeartbeatTimeoutProcessed]=(CASE ISNULL(@NullOverrideHeartbeatTimeoutProcessed,0) WHEN 1 THEN @HeartbeatTimeoutProcessed ELSE ISNULL(@HeartbeatTimeoutProcessed,[HeartbeatTimeoutProcessed]) END)
		,	[ConfigReady]=(CASE ISNULL(@NullOverrideConfigReady,0) WHEN 1 THEN @ConfigReady ELSE ISNULL(@ConfigReady,[ConfigReady]) END)
		,	[MinTime]=(CASE ISNULL(@NullOverrideMinTime,0) WHEN 1 THEN @MinTime ELSE ISNULL(@MinTime,[MinTime]) END)
		,	[MaxTime]=(CASE ISNULL(@NullOverrideMaxTime,0) WHEN 1 THEN @MaxTime ELSE ISNULL(@MaxTime,[MaxTime]) END)
		,	[LevelDeadband]=(CASE ISNULL(@NullOverrideLevelDeadband,0) WHEN 1 THEN @LevelDeadband ELSE ISNULL(@LevelDeadband,[LevelDeadband]) END)
		,	[TempDeadband]=(CASE ISNULL(@NullOverrideTempDeadband,0) WHEN 1 THEN @TempDeadband ELSE ISNULL(@TempDeadband,[TempDeadband]) END)
		,	[Heartbeat]=(CASE ISNULL(@NullOverrideHeartbeat,0) WHEN 1 THEN @Heartbeat ELSE ISNULL(@Heartbeat,[Heartbeat]) END)
		,	[TLStanks]=(CASE ISNULL(@NullOverrideTLStanks,0) WHEN 1 THEN @TLStanks ELSE ISNULL(@TLStanks,[TLStanks]) END)
		,	[ModbusMap]=(CASE ISNULL(@NullOverrideModbusMap,0) WHEN 1 THEN @ModbusMap ELSE ISNULL(@ModbusMap,[ModbusMap]) END)
		,	[MidnightOffset]=(CASE ISNULL(@NullOverrideMidnightOffset,0) WHEN 1 THEN @MidnightOffset ELSE ISNULL(@MidnightOffset,[MidnightOffset]) END)
		,	[ShortDeadband]=(CASE ISNULL(@NullOverrideShortDeadband,0) WHEN 1 THEN @ShortDeadband ELSE ISNULL(@ShortDeadband,[ShortDeadband]) END)
		,	[ShortTime]=(CASE ISNULL(@NullOverrideShortTime,0) WHEN 1 THEN @ShortTime ELSE ISNULL(@ShortTime,[ShortTime]) END)
		,	[LongDeadband]=(CASE ISNULL(@NullOverrideLongDeadband,0) WHEN 1 THEN @LongDeadband ELSE ISNULL(@LongDeadband,[LongDeadband]) END)
		,	[LongTime]=(CASE ISNULL(@NullOverrideLongTime,0) WHEN 1 THEN @LongTime ELSE ISNULL(@LongTime,[LongTime]) END)
		,	[SoftwareVersion]=(CASE ISNULL(@NullOverrideLongTime,0) WHEN 1 THEN @SoftwareVersion ELSE ISNULL(@SoftwareVersion,[SoftwareVersion]) END)
		,	[UpdatedDate]=(CASE ISNULL(@NullOverrideUpdatedDate,0) WHEN 1 THEN @UpdatedDate ELSE ISNULL(@UpdatedDate,[UpdatedDate]) END)
		,	[UpdatedBy]= ISNULL(@UpdatedBy,SUSER_SNAME())
		WHERE FCEDeviceGuid=@FCEDeviceGuid

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
						+ 'Procedure Name: [gsp_FCEDeviceUpdateByPK]' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
END     
	
GO