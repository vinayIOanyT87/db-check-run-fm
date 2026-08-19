
CREATE PROCEDURE [dbo].[gsp_ModuleInsertByPK]
(
		@ModuleGuid uniqueidentifier=NULL OUTPUT
	,	@ID nvarchar(30)=NULL
	,	@Description nvarchar(50)=NULL
	,	@Standard bit=NULL
	,	@ModuleCalculation nvarchar(250)=NULL
	,	@ModuleTypeName nvarchar(250)=NULL
	,	@ModuleData xml=NULL
	,	@ModuleScript nvarchar(MAX)=null
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
	-- Stored procedure: [dbo].[gsp_ModuleInsertByPK] 
	-- Author: DBA - Auto generated
	-- Version/Date: 1.0.001 / 2014-02-05 15:58:32.0032767 -05:00
	-- Purpose: Insert into table [dbo].[tblAirplaneTank]
	-- Notes:
	------------------------------------------------------------------------------------------------------
	SET NOCOUNT ON;
	BEGIN TRY
 
 
 		IF ( @ModuleGuid IS NULL )
		BEGIN
				SET @ModuleGuid=NEWID();
		END
		SET @CreatedDate=ISNULL(@CreatedDate,sysdatetimeoffset())
 
		INSERT INTO [dbo].[tblModule] 
		(
			[ModuleGuid]
		,	[ID]
		,	[Description]
		,	[Standard]
		,	[ModuleCalculation]
		,	[ModuleTypeName]
		,	[ModuleData]
		,	[ModuleScript]
		,	[CreatedDate]
		,	[CreatedBy]
		,	[UpdatedDate]
		,	[UpdatedBy]
		,	[SiteGuid]
		)
		VALUES
		(
			@ModuleGuid
		,	@ID
		,	@Description
		,	@Standard
		,	@ModuleCalculation
		,	@ModuleTypeName
		,	@ModuleData
		,	@ModuleScript
		,	@CreatedDate
		,	@CreatedBy
		,	@UpdatedDate
		,	@UpdatedBy
		,	@SiteGuid
		)
 
		SELECT @_RowVersion=_RowVersion        
		FROM [dbo].[tblModule]           
		WHERE ModuleGuid=@ModuleGuid;
	
 
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
						+ 'Procedure Name: gsp_ModuleInsertByPK' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
END     
	
