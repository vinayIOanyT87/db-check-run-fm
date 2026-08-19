CREATE PROCEDURE [dbo].[gsp_B2BResultsInsertByPK]
(
		@B2BResultGuid uniqueidentifier=NULL OUTPUT
	,	@ResultsID int=NULL
	,	@TransID nvarchar(64)=NULL
	,	@Type char=NULL
	,	@Message nvarchar(100)=NULL
	,	@DataError nvarchar(15)=NULL
	,	@ErrorStatus int=NULL
	,	@Disputed int=NULL
	,	@Corrected int=NULL
	,	@ReceivedSentDate datetimeoffset(7)=NULL
	,	@_RowVersion timestamp=NULL OUTPUT
)
AS
BEGIN
	------------------------------------------------------------------------------------------------------
	-- Stored procedure: [dbo].[gsp_B2BResultsInsertByPK] 
	-- Author: DBA - Auto generated
	-- Version/Date: 1.0.001 / 2014-02-05 15:58:32.0872767 -05:00
	-- Purpose: Insert into table [dbo].[tblB2BResults]
	-- Notes:
	------------------------------------------------------------------------------------------------------
	SET NOCOUNT ON;
	BEGIN TRY
 
 
		SET @B2BResultGuid=NEWID();
 
		INSERT INTO [dbo].[tblB2BResults] 
		(
			[B2BResultGuid]
		,	[TransID]
		,	[Type]
		,	[Message]
		,	[DataError]
		,	[ErrorStatus]
		,	[Disputed]
		,	[Corrected]
		,	[ReceivedSentDate]
		)
		VALUES
		(
			@B2BResultGuid
		,	@TransID
		,	@Type
		,	@Message
		,	@DataError
		,	@ErrorStatus
		,	@Disputed
		,	@Corrected
		,	@ReceivedSentDate
		)
 
		SELECT @_RowVersion=_RowVersion        
		FROM [dbo].[tblB2BResults]           
		WHERE B2BResultGuid=@B2BResultGuid;
	
 
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
						+ 'Procedure Name: gsp_B2BResultsInsertByPK' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
END     
	
