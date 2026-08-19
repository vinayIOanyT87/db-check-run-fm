CREATE PROCEDURE [dbo].[gsp_ImportExportPluginsInsertByPK]
(
		@ImportExportPluginGuid uniqueidentifier=NULL OUTPUT
	,	@PluginType nvarchar(50)=NULL
	,	@ConfigURL nvarchar(250)=NULL
	,	@RunURL nvarchar(250)=NULL
	,	@Import bit=NULL
	,	@Export bit=NULL
	,	@_RowVersion timestamp=NULL OUTPUT
)
AS
BEGIN
	------------------------------------------------------------------------------------------------------
	-- Stored procedure: [dbo].[gsp_ImportExportPluginsInsertByPK] 
	-- Author: DBA - Auto generated
	-- Version/Date: 1.0.001 / 2014-02-05 15:58:32.2562767 -05:00
	-- Purpose: Insert into table [dbo].[tblImportExportPlugins]
	-- Notes:
	------------------------------------------------------------------------------------------------------
	SET NOCOUNT ON;
	BEGIN TRY
 
 
		SET @ImportExportPluginGuid=NEWID();
 
		INSERT INTO [dbo].[tblImportExportPlugins] 
		(
			[ImportExportPluginGuid]
		,	[PluginType]
		,	[ConfigURL]
		,	[RunURL]
		,	[Import]
		,	[Export]
		)
		VALUES
		(
			@ImportExportPluginGuid
		,	@PluginType
		,	@ConfigURL
		,	@RunURL
		,	@Import
		,	@Export
		)
 
		SELECT @_RowVersion=_RowVersion        
		FROM [dbo].[tblImportExportPlugins]           
		WHERE ImportExportPluginGuid=@ImportExportPluginGuid;
	
 
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
						+ 'Procedure Name: gsp_ImportExportPluginsInsertByPK' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
END     
	
