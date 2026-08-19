CREATE PROCEDURE [dbo].[gsp_ChangesQueueInsertByPK]
(
		@ChangesQueueGuid uniqueidentifier=NULL OUTPUT
	,	@EventIndex bigint=NULL
	,	@EventType char=NULL
	,	@Completed bit=NULL
	,	@RecordID nvarchar(64)=NULL
	,	@CreatedDate datetimeoffset(7)=NULL
	,	@CreatedBy udtUserID=NULL
	,	@UpdatedDate datetimeoffset(7)=NULL
	,	@UpdatedBy udtUserID=NULL
	,	@SiteGuid uniqueidentifier=NULL
	,	@LookupChangeQueueRecordTypeIndex int=NULL
	,	@RecordGuid uniqueidentifier=NULL
	,	@_RowVersion timestamp=NULL OUTPUT
)
AS
BEGIN
	------------------------------------------------------------------------------------------------------
	-- Stored procedure: [dbo].[gsp_ChangesQueueInsertByPK] 
	-- Author: DBA - Auto generated
	-- Version/Date: 1.0.001 / 2014-02-05 15:58:32.0962767 -05:00
	-- Purpose: Insert into table [dbo].[tblChangesQueue]
	-- Notes:
	------------------------------------------------------------------------------------------------------
	SET NOCOUNT ON;
	BEGIN TRY
 
 
		SET @ChangesQueueGuid=NEWID();
		SET @CreatedDate=ISNULL(@CreatedDate,sysdatetimeoffset())
 
		INSERT INTO [dbo].[tblChangesQueue] 
		(
			[ChangesQueueGuid]
		,	[EventType]
		,	[Completed]
		,	[RecordID]
		,	[CreatedDate]
		,	[CreatedBy]
		,	[UpdatedDate]
		,	[UpdatedBy]
		,	[SiteGuid]
		,	[LookupChangeQueueRecordTypeIndex]
		,	[RecordGuid]
		)
		VALUES
		(
			@ChangesQueueGuid
		,	@EventType
		,	@Completed
		,	@RecordID
		,	@CreatedDate
		,	@CreatedBy
		,	@UpdatedDate
		,	@UpdatedBy
		,	@SiteGuid
		,	@LookupChangeQueueRecordTypeIndex
		,	@RecordGuid
		)
 
		SELECT @_RowVersion=_RowVersion        
		FROM [dbo].[tblChangesQueue]           
		WHERE ChangesQueueGuid=@ChangesQueueGuid;
	
 
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
						+ 'Procedure Name: gsp_ChangesQueueInsertByPK' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
END     
	
