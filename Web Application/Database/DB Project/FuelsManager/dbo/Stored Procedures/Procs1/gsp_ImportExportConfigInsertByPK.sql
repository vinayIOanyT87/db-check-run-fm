CREATE PROCEDURE [dbo].[gsp_ImportExportConfigInsertByPK]
(
		@ImportExportConfigGuid uniqueidentifier=NULL OUTPUT
	,	@Site nvarchar(50)=NULL
	,	@ImportExportName nvarchar(50)=NULL
	,	@PluginType nvarchar(50)=NULL
	,	@ConfigName nvarchar(50)=NULL
	,	@LastExported nvarchar(50)=NULL
	,	@_RowVersion timestamp=NULL OUTPUT
)
AS
BEGIN
	------------------------------------------------------------------------------------------------------
	-- Stored procedure: [dbo].[gsp_ImportExportConfigInsertByPK] 
	-- Author: DBA - Auto generated
	-- Version/Date: 1.0.001 / 2014-02-05 15:58:32.2532767 -05:00
	-- Purpose: Insert into table [dbo].[tblImportExportConfig]
	-- Notes:
	------------------------------------------------------------------------------------------------------
	SET NOCOUNT ON;
	BEGIN TRY
 
 
		SET @ImportExportConfigGuid=NEWID();
 
		INSERT INTO [dbo].[tblImportExportConfig] 
		(
			[ImportExportConfigGuid]
		,	[Site]
		,	[ImportExportName]
		,	[PluginType]
		,	[ConfigName]
		,	[LastExported]
		)
		VALUES
		(
			@ImportExportConfigGuid
		,	@Site
		,	@ImportExportName
		,	@PluginType
		,	@ConfigName
		,	@LastExported
		)
 
		SELECT @_RowVersion=_RowVersion        
		FROM [dbo].[tblImportExportConfig]           
		WHERE ImportExportConfigGuid=@ImportExportConfigGuid;
	
 
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
						+ 'Procedure Name: gsp_ImportExportConfigInsertByPK' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
END     
	
