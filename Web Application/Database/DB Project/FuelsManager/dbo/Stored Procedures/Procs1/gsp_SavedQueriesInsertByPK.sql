CREATE PROCEDURE [dbo].[gsp_SavedQueriesInsertByPK]
(
		@SavedQueryGuid uniqueidentifier=NULL OUTPUT
	,	@QueryType int=NULL
	,	@QueryName nvarchar(50)=NULL
	,	@TransactionAliases nvarchar(max)=NULL
	,	@CreatedBy udtUserID=NULL
	,	@CreatedDate datetimeoffset(7)=NULL
	,	@UpdatedBy udtUserID=NULL
	,	@UpdatedDate datetimeoffset(7)=NULL
	,	@StartDate datetimeoffset(7)=NULL
	,	@EndDate datetimeoffset(7)=NULL
	,	@SiteGuid uniqueidentifier=NULL
	,	@UserGuid uniqueidentifier=NULL
	,	@_RowVersion timestamp=NULL OUTPUT
)
AS
BEGIN
	------------------------------------------------------------------------------------------------------
	-- Stored procedure: [dbo].[gsp_SavedQueriesInsertByPK] 
	-- Author: DBA - Auto generated
	-- Version/Date: 1.0.001 / 2014-02-05 15:58:32.4182767 -05:00
	-- Purpose: Insert into table [dbo].[tblSavedQueries]
	-- Notes:
	------------------------------------------------------------------------------------------------------
	SET NOCOUNT ON;
	BEGIN TRY
 
 
		SET @SavedQueryGuid=NEWID();
		SET @CreatedDate=ISNULL(@CreatedDate,sysdatetimeoffset())
 
		INSERT INTO [dbo].[tblSavedQueries] 
		(
			[SavedQueryGuid]
		,	[QueryType]
		,	[QueryName]
		,	[TransactionAliases]
		,	[CreatedBy]
		,	[CreatedDate]
		,	[UpdatedBy]
		,	[UpdatedDate]
		,	[StartDate]
		,	[EndDate]
		,	[SiteGuid]
		,	[UserGuid]
		)
		VALUES
		(
			@SavedQueryGuid
		,	@QueryType
		,	@QueryName
		,	@TransactionAliases
		,	@CreatedBy
		,	@CreatedDate
		,	@UpdatedBy
		,	@UpdatedDate
		,	@StartDate
		,	@EndDate
		,	@SiteGuid
		,	@UserGuid
		)
 
		SELECT @_RowVersion=_RowVersion        
		FROM [dbo].[tblSavedQueries]           
		WHERE SavedQueryGuid=@SavedQueryGuid;
	
 
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
						+ 'Procedure Name: gsp_SavedQueriesInsertByPK' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
END     
	
