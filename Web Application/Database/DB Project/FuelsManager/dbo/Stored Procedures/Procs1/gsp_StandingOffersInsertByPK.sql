CREATE PROCEDURE [dbo].[gsp_StandingOffersInsertByPK]
(
		@StandingOfferGuid uniqueidentifier=NULL OUTPUT
	,	@StandingOfferPrice float=NULL
	,	@EffectiveDate datetimeoffset(7)=NULL
	,	@ExpirationDate datetimeoffset(7)=NULL
	,	@CreatedBy udtUserID=NULL
	,	@CreatedDate datetimeoffset(7)=NULL
	,	@UpdatedBy udtUserID=NULL
	,	@UpdatedDate datetimeoffset(7)=NULL
	,	@LowerBound int=NULL
	,	@UpperBound int=NULL
	,	@ReferenceNumber nvarchar(20)=NULL
	,	@SiteGuid uniqueidentifier=NULL
	,	@ProductGuid uniqueidentifier=NULL
	,	@SupplierCompanyGuid uniqueidentifier=NULL
	,	@LocationIATAGuid uniqueidentifier=NULL
	,	@_RowVersion timestamp=NULL OUTPUT
)
AS
BEGIN
	------------------------------------------------------------------------------------------------------
	-- Stored procedure: [dbo].[gsp_StandingOffersInsertByPK] 
	-- Author: DBA - Auto generated
	-- Version/Date: 1.0.001 / 2014-02-05 15:58:32.4802767 -05:00
	-- Purpose: Insert into table [dbo].[tblStandingOffers]
	-- Notes:
	------------------------------------------------------------------------------------------------------
	SET NOCOUNT ON;
	BEGIN TRY
 
 
		SET @StandingOfferGuid=NEWID();
		SET @CreatedDate=ISNULL(@CreatedDate,sysdatetimeoffset())
 
		INSERT INTO [dbo].[tblStandingOffers] 
		(
			[StandingOfferGuid]
		,	[StandingOfferPrice]
		,	[EffectiveDate]
		,	[ExpirationDate]
		,	[CreatedBy]
		,	[CreatedDate]
		,	[UpdatedBy]
		,	[UpdatedDate]
		,	[LowerBound]
		,	[UpperBound]
		,	[ReferenceNumber]
		,	[SiteGuid]
		,	[ProductGuid]
		,	[SupplierCompanyGuid]
		,	[LocationIATAGuid]
		)
		VALUES
		(
			@StandingOfferGuid
		,	@StandingOfferPrice
		,	@EffectiveDate
		,	@ExpirationDate
		,	@CreatedBy
		,	@CreatedDate
		,	@UpdatedBy
		,	@UpdatedDate
		,	@LowerBound
		,	@UpperBound
		,	@ReferenceNumber
		,	@SiteGuid
		,	@ProductGuid
		,	@SupplierCompanyGuid
		,	@LocationIATAGuid
		)
 
		SELECT @_RowVersion=_RowVersion        
		FROM [dbo].[tblStandingOffers]           
		WHERE StandingOfferGuid=@StandingOfferGuid;
	
 
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
						+ 'Procedure Name: gsp_StandingOffersInsertByPK' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
END     
	
