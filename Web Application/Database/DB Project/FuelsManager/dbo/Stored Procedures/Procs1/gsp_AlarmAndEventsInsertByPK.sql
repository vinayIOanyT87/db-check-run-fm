CREATE PROCEDURE [dbo].[gsp_AlarmAndEventsInsertByPK]
(
		@AlarmAndEventGuid uniqueidentifier=NULL OUTPUT
	,	@Source nvarchar(120)=NULL
	,	@Alarm bit=NULL
	,	@ID nvarchar(120)=NULL
	,	@CategoryIndex int=NULL
	,	@PriorityIndex int=NULL
	,	@CreatedDate datetimeoffset(7)=NULL
	,	@CreatedBy udtUserID=NULL
	,	@UpdatedDate datetimeoffset(7)=NULL
	,	@UpdatedBy udtUserID=NULL
	,	@Enabled bit=NULL
	,	@SiteGuid uniqueidentifier=NULL
	,	@CategoryGuid uniqueidentifier=NULL
	,	@PriorityGuid uniqueidentifier=NULL
	,	@_RowVersion timestamp=NULL OUTPUT
)
AS
BEGIN
	------------------------------------------------------------------------------------------------------
	-- Stored procedure: [dbo].[gsp_AlarmAndEventsInsertByPK] 
	-- Author: DBA - Auto generated
	-- Version/Date: 1.0.001 / 2014-02-05 15:58:32.0202767 -05:00
	-- Purpose: Insert into table [dbo].[tblAlarmAndEvents]
	-- Notes:
	------------------------------------------------------------------------------------------------------
	SET NOCOUNT ON;
	BEGIN TRY
 
 
		SET @AlarmAndEventGuid=NEWID();
		SET @CreatedDate=ISNULL(@CreatedDate,sysdatetimeoffset())
 
		INSERT INTO [dbo].[tblAlarmAndEvents] 
		(
			[AlarmAndEventGuid]
		,	[Source]
		,	[Alarm]
		,	[ID]
		,	[CategoryIndex]
		,	[PriorityIndex]
		,	[CreatedDate]
		,	[CreatedBy]
		,	[UpdatedDate]
		,	[UpdatedBy]
		,	[Enabled]
		,	[SiteGuid]
		,	[CategoryGuid]
		,	[PriorityGuid]
		)
		VALUES
		(
			@AlarmAndEventGuid
		,	@Source
		,	@Alarm
		,	@ID
		,	@CategoryIndex
		,	@PriorityIndex
		,	@CreatedDate
		,	@CreatedBy
		,	@UpdatedDate
		,	@UpdatedBy
		,	@Enabled
		,	@SiteGuid
		,	@CategoryGuid
		,	@PriorityGuid
		)
 
		SELECT @_RowVersion=_RowVersion        
		FROM [dbo].[tblAlarmAndEvents]           
		WHERE AlarmAndEventGuid=@AlarmAndEventGuid;
	
 
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
						+ 'Procedure Name: gsp_AlarmAndEventsInsertByPK' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
END     
	
