CREATE PROCEDURE [dbo].[gsp_ConfigurationSettingInsertByPK]
(
		@ConfigurationSettingGuid uniqueidentifier=NULL OUTPUT
	,	@KeyType nvarchar(8)=NULL
	,	@SettingKey nvarchar(50)=NULL
	,	@SettingValue nvarchar(1000)=NULL
	,	@CreatedDate datetimeoffset(7)=NULL
	,	@CreatedBy nvarchar(50)=NULL
	,	@UpdatedDate datetimeoffset(7)=NULL
	,	@UpdatedBy nvarchar(50)=NULL
	,	@_RowVersion timestamp=NULL OUTPUT
)
AS
BEGIN
	------------------------------------------------------------------------------------------------------
	-- Stored procedure: [dbo].[gsp_ConfigurationSettingInsertByPK] 
	-- Author: DBA - Auto generated
	-- Version/Date: 1.0.001 / 2014-02-05 15:58:32.1142767 -05:00
	-- Purpose: Insert into table [dbo].[tblConfigurationSetting]
	-- Notes:
	------------------------------------------------------------------------------------------------------
	SET NOCOUNT ON;
	BEGIN TRY
 
 
		SET @ConfigurationSettingGuid=NEWID();
		SET @CreatedDate=ISNULL(@CreatedDate,sysdatetimeoffset())
 
		INSERT INTO [dbo].[tblConfigurationSetting] 
		(
			[ConfigurationSettingGuid]
		,	[KeyType]
		,	[SettingKey]
		,	[SettingValue]
		,	[CreatedDate]
		,	[CreatedBy]
		,	[UpdatedDate]
		,	[UpdatedBy]
		)
		VALUES
		(
			@ConfigurationSettingGuid
		,	@KeyType
		,	@SettingKey
		,	@SettingValue
		,	@CreatedDate
		,	@CreatedBy
		,	@UpdatedDate
		,	@UpdatedBy
		)
 
		SELECT @_RowVersion=_RowVersion        
		FROM [dbo].[tblConfigurationSetting]           
		WHERE ConfigurationSettingGuid=@ConfigurationSettingGuid;
	
 
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
						+ 'Procedure Name: gsp_ConfigurationSettingInsertByPK' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
END     
	
