CREATE PROCEDURE [dbo].[gsp_PointPropertyInsertByPK]
(
		@PointPropertyGuid uniqueidentifier=NULL OUTPUT
	,	@ID nvarchar(50)=NULL
	,	@ValueType nvarchar(max)=NULL
	,	@Value xml=NULL
	,	@CreatedDate datetimeoffset(7)=NULL
	,	@CreatedBy udtUserID=NULL
	,	@UpdatedDate datetimeoffset(7)=NULL
	,	@UpdatedBy udtUserID=NULL
	,	@PointGuid uniqueidentifier=NULL
	,	@PointTemplatePropertyGuid uniqueidentifier=NULL
	,	@_ClusterIdx bigint=NULL OUTPUT
	,	@_RowVersion timestamp=NULL OUTPUT
)
AS
BEGIN
	------------------------------------------------------------------------------------------------------
	-- Stored procedure: [dbo].[gsp_PointPropertyInsertByPK] 
	-- Author: DBA - Auto generated
	-- Version/Date: 1.0.001 / 2016-02-06 16:55:13.9872767 -05:00
	-- Purpose: Insert into table [dbo].[tblPointProperty]
	-- Notes:
	------------------------------------------------------------------------------------------------------
	SET NOCOUNT ON;
	BEGIN TRY
 
 
		SET @PointPropertyGuid=NEWID();
		SET @CreatedDate=ISNULL(@CreatedDate,sysdatetimeoffset())
 
		INSERT INTO [dbo].[tblPointProperty] 
		(
			[PointPropertyGuid]
		,	[ID]
		,	[ValueType]
		,	[Value]
		,	[CreatedDate]
		,	[CreatedBy]
		,	[UpdatedDate]
		,	[UpdatedBy]
		,	[PointGuid]
		,	[PointTemplatePropertyGuid]
		)
		VALUES
		(
			@PointPropertyGuid
		,	@ID
		,	@ValueType
		,	@Value
		,	@CreatedDate
		,	@CreatedBy
		,	@UpdatedDate
		,	@UpdatedBy
		,	@PointGuid
		,	@PointTemplatePropertyGuid
		)
 
		SELECT @_RowVersion = _RowVersion,@_ClusterIdx = _ClusterIdx        
		FROM [dbo].[tblPointProperty]           
		WHERE PointPropertyGuid=@PointPropertyGuid;
	
 
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
						+ 'Procedure Name: gsp_ModulePointPropertyInsertByPK' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
END     
	
GO
