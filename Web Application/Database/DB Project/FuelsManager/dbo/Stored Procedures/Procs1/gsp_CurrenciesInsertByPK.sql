CREATE PROCEDURE [dbo].[gsp_CurrenciesInsertByPK]
(
		@CurrencyGuid uniqueidentifier=NULL OUTPUT
	,	@Country nvarchar(50)=NULL
	,	@UnitDisplayName nvarchar(50)=NULL
	,	@DisplayFlag bit=NULL
	,	@CreatedBy udtUserID=NULL
	,	@CreatedDate datetimeoffset(7)=NULL
	,	@UpdatedBy udtUserID=NULL
	,	@UpdatedDate datetimeoffset(7)=NULL
	,	@SiteGuid uniqueidentifier=NULL
	,	@LookupCurrencyUnitIndex int=NULL
	,	@_RowVersion timestamp=NULL OUTPUT
)
AS
BEGIN
	------------------------------------------------------------------------------------------------------
	-- Stored procedure: [dbo].[gsp_CurrenciesInsertByPK] 
	-- Author: DBA - Auto generated
	-- Version/Date: 1.0.001 / 2014-02-05 15:58:32.1182767 -05:00
	-- Purpose: Insert into table [dbo].[tblCurrencies]
	-- Notes:
	------------------------------------------------------------------------------------------------------
	SET NOCOUNT ON;
	BEGIN TRY
 
 
		SET @CurrencyGuid=NEWID();
		SET @CreatedDate=ISNULL(@CreatedDate,sysdatetimeoffset())
 
		INSERT INTO [dbo].[tblCurrencies] 
		(
			[CurrencyGuid]
		,	[Country]
		,	[UnitDisplayName]
		,	[DisplayFlag]
		,	[CreatedBy]
		,	[CreatedDate]
		,	[UpdatedBy]
		,	[UpdatedDate]
		,	[SiteGuid]
		,	[LookupCurrencyUnitIndex]
		)
		VALUES
		(
			@CurrencyGuid
		,	@Country
		,	@UnitDisplayName
		,	@DisplayFlag
		,	@CreatedBy
		,	@CreatedDate
		,	@UpdatedBy
		,	@UpdatedDate
		,	@SiteGuid
		,	@LookupCurrencyUnitIndex
		)
 
		SELECT @_RowVersion=_RowVersion        
		FROM [dbo].[tblCurrencies]           
		WHERE CurrencyGuid=@CurrencyGuid;
	
 
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
						+ 'Procedure Name: gsp_CurrenciesInsertByPK' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
END     
	
