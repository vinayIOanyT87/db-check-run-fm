CREATE PROCEDURE [dbo].[gsp_ChangeLogInsertByPK]
(
		@ChangeLogGuid uniqueidentifier=NULL OUTPUT
	,	@TableName varchar(64)=NULL
	,	@RowID varchar(max)=NULL
	,	@DmlType char=NULL
	,	@DateEvent datetimeoffset(7)=NULL
	,	@ColumnsBefore xml=NULL
	,	@ColumnsAfter xml=NULL
	,	@UserID varchar(25)=NULL
	,	@ASPSessionID char=NULL
	,	@Token uniqueidentifier=NULL
	,	@SPID smallint=NULL
	,	@ClientDomain varchar(16)=NULL
	,	@ClientUserName varchar(10)=NULL
	,	@Workstation varchar(15)=NULL
	,	@ClientIPAddr int=NULL
	,	@AppName varchar(64)=NULL
	,	@_RowVersion timestamp=NULL OUTPUT
)
AS
BEGIN
	------------------------------------------------------------------------------------------------------
	-- Stored procedure: [dbo].[gsp_ChangeLogInsertByPK] 
	-- Author: DBA - Auto generated
	-- Version/Date: 1.0.001 / 2014-02-05 15:58:32.0932767 -05:00
	-- Purpose: Insert into table [dbo].[tblChangeLog]
	-- Notes:
	------------------------------------------------------------------------------------------------------
	SET NOCOUNT ON;
	BEGIN TRY
 
 
		SET @ChangeLogGuid=NEWID();
 
		INSERT INTO [dbo].[tblChangeLog] 
		(
			[ChangeLogGuid]
		,	[TableName]
		,	[RowID]
		,	[DmlType]
		,	[DateEvent]
		,	[ColumnsBefore]
		,	[ColumnsAfter]
		,	[UserID]
		,	[ASPSessionID]
		,	[Token]
		,	[SPID]
		,	[ClientDomain]
		,	[ClientUserName]
		,	[Workstation]
		,	[ClientIPAddr]
		,	[AppName]
		)
		VALUES
		(
			@ChangeLogGuid
		,	@TableName
		,	@RowID
		,	@DmlType
		,	@DateEvent
		,	@ColumnsBefore
		,	@ColumnsAfter
		,	@UserID
		,	@ASPSessionID
		,	@Token
		,	@SPID
		,	@ClientDomain
		,	@ClientUserName
		,	@Workstation
		,	@ClientIPAddr
		,	@AppName
		)
 
		SELECT @_RowVersion=_RowVersion        
		FROM [dbo].[tblChangeLog]           
		WHERE ChangeLogGuid=@ChangeLogGuid;
	
 
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
						+ 'Procedure Name: gsp_ChangeLogInsertByPK' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
END     
	
