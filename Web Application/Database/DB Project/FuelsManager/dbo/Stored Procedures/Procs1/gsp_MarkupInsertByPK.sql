CREATE PROCEDURE [dbo].[gsp_MarkupInsertByPK]
(
		@MarkupGuid uniqueidentifier=NULL OUTPUT
	,	@PurchasingEntity nvarchar(50)=NULL
	,	@MarkupRate float=NULL
	,	@SOFA_FEA bit=NULL
	,	@Quantities float=NULL
	,	@CreatedBy udtUserID=NULL
	,	@CreatedDate datetimeoffset(7)=NULL
	,	@UpdatedBy udtUserID=NULL
	,	@UpdatedDate datetimeoffset(7)=NULL
	,	@LookupServiceTypeIndex int=NULL
	,	@_RowVersion timestamp=NULL OUTPUT
)
AS
BEGIN
	------------------------------------------------------------------------------------------------------
	-- Stored procedure: [dbo].[gsp_MarkupInsertByPK] 
	-- Author: DBA - Auto generated
	-- Version/Date: 1.0.001 / 2014-02-05 15:58:32.2702767 -05:00
	-- Purpose: Insert into table [dbo].[tblMarkup]
	-- Notes:
	------------------------------------------------------------------------------------------------------
	SET NOCOUNT ON;
	BEGIN TRY
 
 
		SET @MarkupGuid=NEWID();
		SET @CreatedDate=ISNULL(@CreatedDate,sysdatetimeoffset())
 
		INSERT INTO [dbo].[tblMarkup] 
		(
			[MarkupGuid]
		,	[PurchasingEntity]
		,	[MarkupRate]
		,	[SOFA_FEA]
		,	[Quantities]
		,	[CreatedBy]
		,	[CreatedDate]
		,	[UpdatedBy]
		,	[UpdatedDate]
		,	[LookupServiceTypeIndex]
		)
		VALUES
		(
			@MarkupGuid
		,	@PurchasingEntity
		,	@MarkupRate
		,	@SOFA_FEA
		,	@Quantities
		,	@CreatedBy
		,	@CreatedDate
		,	@UpdatedBy
		,	@UpdatedDate
		,	@LookupServiceTypeIndex
		)
 
		SELECT @_RowVersion=_RowVersion        
		FROM [dbo].[tblMarkup]           
		WHERE MarkupGuid=@MarkupGuid;
	
 
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
						+ 'Procedure Name: gsp_MarkupInsertByPK' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
END     
	
