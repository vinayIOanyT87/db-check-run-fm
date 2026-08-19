CREATE PROCEDURE [dbo].[gsp_EnterpriseQueueInsertByPK]
(
		@EnterpriseQueueGuid uniqueidentifier=NULL OUTPUT
	,	@SourceType int=NULL
	,	@SourceID nvarchar(120)=NULL
	,	@DateAdded datetimeoffset(7)=NULL
	,	@Priority int=NULL
	,	@Status int=NULL
	,	@DateUpdated datetimeoffset(7)=NULL
	,	@CreatedBy udtUserID=NULL
	,	@UpdatedBy udtUserID=NULL
	,	@CreatedDate datetimeoffset(7)=NULL
	,	@UpdatedDate datetimeoffset(7)=NULL
	,	@_RowVersion timestamp=NULL OUTPUT
)
AS
BEGIN
	------------------------------------------------------------------------------------------------------
	-- Stored procedure: [dbo].[gsp_EnterpriseQueueInsertByPK] 
	-- Author: DBA - Auto generated
	-- Version/Date: 1.0.001 / 2014-02-05 15:58:32.1412767 -05:00
	-- Purpose: Insert into table [dbo].[tblEnterpriseQueue]
	-- Notes:
	------------------------------------------------------------------------------------------------------
	SET NOCOUNT ON;
	BEGIN TRY
 
 
		SET @EnterpriseQueueGuid=NEWID();
		SET @CreatedDate=ISNULL(@CreatedDate,sysdatetimeoffset())
 
		INSERT INTO [dbo].[tblEnterpriseQueue] 
		(
			[EnterpriseQueueGuid]
		,	[SourceType]
		,	[SourceID]
		,	[DateAdded]
		,	[Priority]
		,	[Status]
		,	[DateUpdated]
		,	[CreatedBy]
		,	[UpdatedBy]
		,	[CreatedDate]
		,	[UpdatedDate]
		)
		VALUES
		(
			@EnterpriseQueueGuid
		,	@SourceType
		,	@SourceID
		,	@DateAdded
		,	@Priority
		,	@Status
		,	@DateUpdated
		,	@CreatedBy
		,	@UpdatedBy
		,	@CreatedDate
		,	@UpdatedDate
		)
 
		SELECT @_RowVersion=_RowVersion        
		FROM [dbo].[tblEnterpriseQueue]           
		WHERE EnterpriseQueueGuid=@EnterpriseQueueGuid;
	
 
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
						+ 'Procedure Name: gsp_EnterpriseQueueInsertByPK' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
END     
	
