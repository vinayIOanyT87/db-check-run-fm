CREATE PROCEDURE [dbo].[gsp_ExportPaiceTransTrackingInsertByPK]
(
		@TransID nvarchar(64)=NULL OUTPUT
	,	@TransType nvarchar(2)=NULL
	,	@SentDate datetimeoffset(7)=NULL
	,	@CreatedDate datetimeoffset(7)=NULL
	,	@CreatedBy udtUserID=NULL
	,	@UpdatedDate datetimeoffset(7)=NULL
	,	@UpdatedBy udtUserID=NULL
	,	@_RowVersion timestamp=NULL OUTPUT
)
AS
BEGIN
	------------------------------------------------------------------------------------------------------
	-- Stored procedure: [dbo].[gsp_ExportPaiceTransTrackingInsertByPK] 
	-- Author: DBA - Auto generated
	-- Version/Date: 1.0.001 / 2014-02-05 15:58:32.1952767 -05:00
	-- Purpose: Insert into table [dbo].[tblExportPaiceTransTracking]
	-- NEW FUNCTIONALITY 2015-01-23 PCarpenter
	-- Use table tblTransactions, but no change in parameters
	-- Notes:
	------------------------------------------------------------------------------------------------------
	SET NOCOUNT ON;
     
		DECLARE	@_ErrMessage NVARCHAR(2048)      
				, @_ErrNumber INT           
				, @_ErrProcName NVARCHAR(126)           
				, @_ErrLineNumber INT;            
		SET @_ErrMessage = ERROR_MESSAGE();        
		SET @_ErrNumber = ERROR_NUMBER();        
		SET @_ErrProcName= ERROR_PROCEDURE();        
		SET @_ErrLineNumber = ERROR_LINE();            
		SET @_ErrMessage = 'Error: This function is not supported, a TransID must exist to insert '  + CHAR(13)+CHAR(10)                 
						+ 'Number: ' + CAST(@_ErrNumber AS VARCHAR(20)) + CHAR(13)+CHAR(10)                 
						+ 'Procedure Name: gsp_ExportPaiceTransTrackingInsertByPK' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      

END     
	
