CREATE PROCEDURE [dbo].[gsp_VersionInsertByPK]
(
		@VersionGuid uniqueidentifier=NULL OUTPUT
	,	@VersionIndex int=NULL
	,	@Version nvarchar(32)=NULL
	,	@PackageName nvarchar(32)=NULL
	,	@DateApplied datetimeoffset(7)=NULL
	,	@Comments nvarchar(4000)=NULL
	,	@Check1 bigint=NULL
	,	@Check2 bigint=NULL
	,	@CreatedDate datetime=NULL
	,	@CreatedBy udtUserID=NULL
	,	@UpdatedDate datetime=NULL
	,	@UpdatedBy udtUserID=NULL
	,	@SyncCompletedFlag bit=NULL
	,	@RowVersionSnapshot varbinary=NULL
	,	@_RowVersion timestamp=NULL OUTPUT
)
AS
BEGIN
	------------------------------------------------------------------------------------------------------
	-- Stored procedure: [dbo].[gsp_VersionInsertByPK] 
	-- Author: DBA - Auto generated
	-- Version/Date: 1.0.001 / 2014-02-05 15:58:32.6272767 -05:00
	-- Purpose: Insert into table [dbo].[tblVersion]
	-- Notes:
	------------------------------------------------------------------------------------------------------
	SET NOCOUNT ON;
	BEGIN TRY
 
 
		SET @VersionGuid=NEWID();
		SET @CreatedDate=ISNULL(@CreatedDate,sysdatetimeoffset())
 
		INSERT INTO [dbo].[tblVersion] 
		(
			[VersionGuid]
		,	[Version]
		,	[PackageName]
		,	[DateApplied]
		,	[Comments]
		,	[Check1]
		,	[Check2]
		,	[CreatedDate]
		,	[CreatedBy]
		,	[UpdatedDate]
		,	[UpdatedBy]
		,	[SyncCompletedFlag]
		,	[RowVersionSnapshot]
		)
		VALUES
		(
			@VersionGuid
		,	@Version
		,	@PackageName
		,	@DateApplied
		,	@Comments
		,	@Check1
		,	@Check2
		,	@CreatedDate
		,	@CreatedBy
		,	@UpdatedDate
		,	@UpdatedBy
		,	@SyncCompletedFlag
		,	@RowVersionSnapshot
		)
 
		SELECT @_RowVersion=_RowVersion        
		FROM [dbo].[tblVersion]           
		WHERE VersionGuid=@VersionGuid;
	
 
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
						+ 'Procedure Name: gsp_VersionInsertByPK' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
END     
	
