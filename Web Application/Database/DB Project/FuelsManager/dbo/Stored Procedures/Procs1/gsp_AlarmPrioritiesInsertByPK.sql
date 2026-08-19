CREATE PROCEDURE [dbo].[gsp_AlarmPrioritiesInsertByPK]
(
		@AlarmPriorityGuid uniqueidentifier=NULL OUTPUT
	,	@ID nvarchar(32)=NULL
	,	@BackgroundSteady nvarchar(8)=NULL
	,	@BackgroundAlternate nvarchar(8)=NULL
	,	@TextSteady nvarchar(8)=NULL
	,	@TextAlternate nvarchar(8)=NULL
	,	@SoundFile nvarchar(50)=NULL
	,	@CreatedDate datetimeoffset(7)=NULL
	,	@CreatedBy udtUserID=NULL
	,	@UpdatedDate datetimeoffset(7)=NULL
	,	@UpdatedBy udtUserID=NULL
	,	@SiteGuid uniqueidentifier=NULL
	,	@Priority tinyint=NULL
	,	@_RowVersion timestamp=NULL OUTPUT
)
AS
BEGIN
	------------------------------------------------------------------------------------------------------
	-- Stored procedure: [dbo].[gsp_AlarmPrioritiesInsertByPK] 
	-- Author: DBA - Auto generated
	-- Version/Date: 1.0.001 / 2014-02-05 15:58:32.0272767 -05:00
	-- Purpose: Insert into table [dbo].[tblAlarmPriorities]
	-- Notes:
	------------------------------------------------------------------------------------------------------
	SET NOCOUNT ON;
	BEGIN TRY
 
 
		SET @AlarmPriorityGuid=NEWID();
		SET @CreatedDate=ISNULL(@CreatedDate,sysdatetimeoffset())
 
		INSERT INTO [dbo].[tblAlarmPriorities] 
		(
			[AlarmPriorityGuid]
		,	[ID]
		,	[BackgroundSteady]
		,	[BackgroundAlternate]
		,	[TextSteady]
		,	[TextAlternate]
		,	[SoundFile]
		,	[CreatedDate]
		,	[CreatedBy]
		,	[UpdatedDate]
		,	[UpdatedBy]
		,	[SiteGuid]
		,	[Priority]
		)
		VALUES
		(
			@AlarmPriorityGuid
		,	@ID
		,	@BackgroundSteady
		,	@BackgroundAlternate
		,	@TextSteady
		,	@TextAlternate
		,	@SoundFile
		,	@CreatedDate
		,	@CreatedBy
		,	@UpdatedDate
		,	@UpdatedBy
		,	@SiteGuid
		,	@Priority
		)
 
		SELECT @_RowVersion=_RowVersion        
		FROM [dbo].[tblAlarmPriorities]           
		WHERE AlarmPriorityGuid=@AlarmPriorityGuid;
	
 
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
						+ 'Procedure Name: gsp_AlarmPrioritiesInsertByPK' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
END     
	
