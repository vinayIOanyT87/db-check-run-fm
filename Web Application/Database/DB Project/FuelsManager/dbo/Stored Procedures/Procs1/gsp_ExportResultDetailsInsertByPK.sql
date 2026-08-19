CREATE PROCEDURE [dbo].[gsp_ExportResultDetailsInsertByPK]
(
		@ExportResultDetailGuid uniqueidentifier=NULL OUTPUT
	,	@RecordID nvarchar(64)=NULL
	,	@Fail bit=NULL
	,	@TransVersion bigint=NULL
	,	@CreatedDate datetimeoffset(7)=NULL
	,	@CreatedBy udtUserID=NULL
	,	@UpdatedDate datetimeoffset(7)=NULL
	,	@UpdatedBy udtUserID=NULL
	,	@Error nvarchar(250)=NULL
	,	@ExportResultGuid uniqueidentifier=NULL
	,	@InterfaceData01 nvarchar(100)=NULL
	,	@InterfaceData02 nvarchar(100)=NULL
	,	@InterfaceData03 nvarchar(100)=NULL
	,	@InterfaceData04 nvarchar(100)=NULL
	,	@InterfaceData05 nvarchar(100)=NULL
	,	@InterfaceData06 nvarchar(100)=NULL
	,	@InterfaceData07 nvarchar(100)=NULL
	,	@InterfaceData08 nvarchar(100)=NULL
	,	@_RowVersion timestamp=NULL OUTPUT
)
AS
BEGIN
	------------------------------------------------------------------------------------------------------
	-- Stored procedure: [dbo].[gsp_ExportResultDetailsInsertByPK] 
	-- Author: DBA - Auto generated
	-- Version/Date: 1.0.001 / 2014-02-05 15:58:32.2112767 -05:00
	-- Purpose: Insert into table [dbo].[tblExportResultDetails]
	-- Notes:
	------------------------------------------------------------------------------------------------------
	SET NOCOUNT ON;
	BEGIN TRY
 
 
		SET @ExportResultDetailGuid=NEWID();
		SET @CreatedDate=ISNULL(@CreatedDate,sysdatetimeoffset())
 
		INSERT INTO [dbo].[tblExportResultDetails] 
		(
			[ExportResultDetailGuid]
		,	[RecordID]
		,	[Fail]
		,	[TransVersion]
		,	[CreatedDate]
		,	[CreatedBy]
		,	[UpdatedDate]
		,	[UpdatedBy]
		,	[Error]
		,	[ExportResultGuid]
		,	[InterfaceData01]
		,	[InterfaceData02]
		,	[InterfaceData03]
		,	[InterfaceData04]
		,	[InterfaceData05]
		,	[InterfaceData06]
		,	[InterfaceData07]
		,	[InterfaceData08]
		)
		VALUES
		(
			@ExportResultDetailGuid
		,	@RecordID
		,	@Fail
		,	@TransVersion
		,	@CreatedDate
		,	@CreatedBy
		,	@UpdatedDate
		,	@UpdatedBy
		,	@Error
		,	@ExportResultGuid
		,	@InterfaceData01
		,	@InterfaceData02
		,	@InterfaceData03
		,	@InterfaceData04
		,	@InterfaceData05
		,	@InterfaceData06
		,	@InterfaceData07
		,	@InterfaceData08
		)
 
		SELECT @_RowVersion=_RowVersion        
		FROM [dbo].[tblExportResultDetails]           
		WHERE ExportResultDetailGuid=@ExportResultDetailGuid;
	
 
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
						+ 'Procedure Name: gsp_ExportResultDetailsInsertByPK' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
END     
	
