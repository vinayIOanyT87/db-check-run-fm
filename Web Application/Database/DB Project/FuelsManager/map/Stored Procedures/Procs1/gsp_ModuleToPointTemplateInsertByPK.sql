CREATE PROCEDURE [map].[gsp_ModuleToPointTemplateInsertByPK]
(
		@ModuleToPointTemplateGuid uniqueidentifier=NULL OUTPUT
	,	@ID nvarchar(30)
	,	@Order int
	,	@ModuleToPointTemplateData xml=NULL
	,	@CreatedDate datetimeoffset(7)=NULL
	,	@CreatedBy udtUserID=NULL
	,	@UpdatedDate datetimeoffset(7)=NULL
	,	@UpdatedBy udtUserID=NULL
	,	@PointTemplateGuid uniqueidentifier=NULL
	,	@ModuleGuid uniqueidentifier=NULL
	,	@_RowVersion timestamp=NULL OUTPUT
)
AS
BEGIN
	------------------------------------------------------------------------------------------------------
	-- Stored procedure: [map].[gsp_ModuleToPointTemplateInsertByPK] 
	-- Author: DBA - Auto generated
	-- Version/Date: 1.0.001 / 2014-11-24 14:41:58.8722400 -05:00
	-- Purpose: Insert into table [map].[tblModuleToPointTemplate]
	-- Notes:
	------------------------------------------------------------------------------------------------------
	SET NOCOUNT ON;
	BEGIN TRY
 
 
 		IF ( @ModuleToPointTemplateGuid IS NULL )
		BEGIN
				SET @ModuleToPointTemplateGuid=NEWID();
		END
		SET @CreatedDate=ISNULL(@CreatedDate,sysdatetimeoffset())
 
		INSERT INTO [map].[tblModuleToPointTemplate] 
		(
			[ModuleToPointTemplateGuid]
		,	[ID]
		,	[Order]
		,	[ModuleToPointTemplateData]
		,	[CreatedDate]
		,	[CreatedBy]
		,	[UpdatedDate]
		,	[UpdatedBy]
		,	[PointTemplateGuid]
		,	[ModuleGuid]
		)
		VALUES
		(
			@ModuleToPointTemplateGuid
		,	@ID
		,	@Order
		,	@ModuleToPointTemplateData
		,	@CreatedDate
		,	@CreatedBy
		,	@UpdatedDate
		,	@UpdatedBy
		,	@PointTemplateGuid
		,	@ModuleGuid
		)
 
		SELECT @_RowVersion=_RowVersion        
		FROM [map].[tblModuleToPointTemplate]           
		WHERE ModuleToPointTemplateGuid=@ModuleToPointTemplateGuid;
	
 
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
						+ 'Procedure Name: gsp_ModuleToPointTemplateInsertByPK' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
END