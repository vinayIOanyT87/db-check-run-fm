CREATE PROCEDURE [dbo].[gsp_SystemSettingsInsertByPK]
(
		@SystemSettingGuid uniqueidentifier=NULL OUTPUT
	,	@ReportServerURL nvarchar(80)=NULL
	,	@StationMessageTimeout int=NULL
	,	@StationPromptTimeout int=NULL
	,	@CreatedDate datetimeoffset(7)=NULL
	,	@CreatedBy udtUserID=NULL
	,	@UpdatedDate datetimeoffset(7)=NULL
	,	@UpdatedBy udtUserID=NULL
	,	@ReportServerUserName nvarchar(50)=NULL
	,	@ReportServerPassword varbinary=NULL
	,	@_RowVersion timestamp=NULL OUTPUT
)
AS
BEGIN
	------------------------------------------------------------------------------------------------------
	-- Stored procedure: [dbo].[gsp_SystemSettingsInsertByPK] 
	-- Author: DBA - Auto generated
	-- Version/Date: 1.0.001 / 2014-02-05 15:58:32.4912767 -05:00
	-- Purpose: Insert into table [dbo].[tblSystemSettings]
	-- Notes:
	------------------------------------------------------------------------------------------------------
	SET NOCOUNT ON;
	BEGIN TRY
 
 
		SET @SystemSettingGuid=NEWID();
		SET @CreatedDate=ISNULL(@CreatedDate,sysdatetimeoffset())
 
		INSERT INTO [dbo].[tblSystemSettings] 
		(
			[SystemSettingGuid]
		,	[ReportServerURL]
		,	[StationMessageTimeout]
		,	[StationPromptTimeout]
		,	[CreatedDate]
		,	[CreatedBy]
		,	[UpdatedDate]
		,	[UpdatedBy]
		,	[ReportServerUserName]
		,	[ReportServerPassword]
		)
		VALUES
		(
			@SystemSettingGuid
		,	@ReportServerURL
		,	@StationMessageTimeout
		,	@StationPromptTimeout
		,	@CreatedDate
		,	@CreatedBy
		,	@UpdatedDate
		,	@UpdatedBy
		,	@ReportServerUserName
		,	@ReportServerPassword
		)
 
		SELECT @_RowVersion=_RowVersion        
		FROM [dbo].[tblSystemSettings]           
		WHERE SystemSettingGuid=@SystemSettingGuid;
	
 
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
						+ 'Procedure Name: gsp_SystemSettingsInsertByPK' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
END     
	
