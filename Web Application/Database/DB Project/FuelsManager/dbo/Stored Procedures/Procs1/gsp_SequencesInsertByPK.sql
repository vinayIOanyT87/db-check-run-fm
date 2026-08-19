CREATE PROCEDURE [dbo].[gsp_SequencesInsertByPK]
(
		@SequenceGuid uniqueidentifier=NULL OUTPUT
	,	@SequenceKey nvarchar(30)=NULL
	,	@SequenceValue bigint=NULL
	,	@_RowVersion timestamp=NULL OUTPUT
)
AS
BEGIN
	------------------------------------------------------------------------------------------------------
	-- Stored procedure: [dbo].[gsp_SequencesInsertByPK] 
	-- Author: DBA - Auto generated
	-- Version/Date: 1.0.001 / 2014-02-05 15:58:32.4312767 -05:00
	-- Purpose: Insert into table [dbo].[tblSequences]
	-- Notes:
	------------------------------------------------------------------------------------------------------
	SET NOCOUNT ON;
	BEGIN TRY
 
 
		SET @SequenceGuid=NEWID();
 
		INSERT INTO [dbo].[tblSequences] 
		(
			[SequenceGuid]
		,	[SequenceKey]
		,	[SequenceValue]
		)
		VALUES
		(
			@SequenceGuid
		,	@SequenceKey
		,	@SequenceValue
		)
 
		SELECT @_RowVersion=_RowVersion        
		FROM [dbo].[tblSequences]           
		WHERE SequenceGuid=@SequenceGuid;
	
 
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
						+ 'Procedure Name: gsp_SequencesInsertByPK' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
END     
	
