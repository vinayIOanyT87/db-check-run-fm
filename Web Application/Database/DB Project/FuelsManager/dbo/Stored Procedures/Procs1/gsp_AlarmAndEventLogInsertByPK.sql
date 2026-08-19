CREATE PROCEDURE [dbo].[gsp_AlarmAndEventLogInsertByPK]
(
		@AlarmAndEventLogGuid uniqueidentifier=NULL OUTPUT
	,	@SequenceNumber bigint=NULL
	,	@Source nvarchar(120)=NULL
	,	@Alarm bit=NULL
	,	@ID nvarchar(120)=NULL
	,	@AssociatedData nvarchar(max)=NULL
	,	@CategoryID nvarchar(50)=NULL
	,	@PriorityID nvarchar(50)=NULL
	,	@Acknowledged bit=NULL
	,	@CreatedDate datetimeoffset(7)=NULL
	,	@CreatedBy udtUserID=NULL
	,	@UpdatedDate datetimeoffset(7)=NULL
	,	@UpdatedBy udtUserID=NULL
	,	@SiteGuid uniqueidentifier=NULL
	,	@_RowVersion timestamp=NULL OUTPUT
)
AS
BEGIN
	------------------------------------------------------------------------------------------------------
	-- Stored procedure: [dbo].[gsp_AlarmAndEventLogInsertByPK] 
	-- Author: DBA - Auto generated
	-- Version/Date: 1.0.001 / 2014-02-05 15:58:32.0132767 -05:00
	-- Purpose: Insert into table [dbo].[tblAlarmAndEventLog]
	-- Notes:
	------------------------------------------------------------------------------------------------------
	SET NOCOUNT ON;
	BEGIN TRY
 
 
		SET @AlarmAndEventLogGuid=NEWID();
		SET @CreatedDate=ISNULL(@CreatedDate,sysdatetimeoffset())
 
		INSERT INTO [dbo].[tblAlarmAndEventLog] 
		(
			[AlarmAndEventLogGuid]
		,	[Source]
		,	[Alarm]
		,	[ID]
		,	[AssociatedData]
		,	[CategoryID]
		,	[PriorityID]
		,	[Acknowledged]
		,	[CreatedDate]
		,	[CreatedBy]
		,	[UpdatedDate]
		,	[UpdatedBy]
		,	[SiteGuid]
		)
		VALUES
		(
			@AlarmAndEventLogGuid
		,	@Source
		,	@Alarm
		,	@ID
		,	@AssociatedData
		,	@CategoryID
		,	@PriorityID
		,	@Acknowledged
		,	@CreatedDate
		,	@CreatedBy
		,	@UpdatedDate
		,	@UpdatedBy
		,	@SiteGuid
		)
 
		SELECT @_RowVersion=_RowVersion        
		FROM [dbo].[tblAlarmAndEventLog]           
		WHERE AlarmAndEventLogGuid=@AlarmAndEventLogGuid;
	
 
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
						+ 'Procedure Name: gsp_AlarmAndEventLogInsertByPK' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
END     
	
