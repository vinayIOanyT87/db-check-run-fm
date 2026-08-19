CREATE PROCEDURE [map].[gsp_PIDXProfileToCompanyInsertByPK]
(
		@PIDXProfileToCompanyGuid uniqueidentifier=NULL OUTPUT
	,	@PIDXProfileGuid uniqueidentifier=NULL
	,	@CompanyPersonnelToShipToBillToGuid uniqueidentifier=NULL
	,	@SiteGuid uniqueidentifier=NULL
	,	@SellerID nvarchar(3)=NULL
	,	@ShipperID nvarchar(3)=NULL
	,	@ConsigneeNumber nvarchar(14)=NULL
	,	@DenialOverride bit=NULL
	,	@UnavailableOverride bit=NULL
	,	@CreatedDate datetimeoffset(7)=NULL
	,	@CreatedBy udtUserID=NULL
	,	@UpdatedDate datetimeoffset(7)=NULL
	,	@UpdatedBy udtUserID=NULL
	,	@_RowVersion timestamp=NULL OUTPUT
)
AS
BEGIN
	------------------------------------------------------------------------------------------------------
	-- Stored procedure: [map].[gsp_PIDXProfileToCompanyInsertByPK] 
	-- Author: DBA - Auto generated
	-- Version/Date: 1.0.001 / 2014-02-05 15:58:32.7022767 -05:00
	-- Purpose: Insert into table [map].[tblPIDXProfileToCompany]
	-- Notes:
	------------------------------------------------------------------------------------------------------
	SET NOCOUNT ON;
	BEGIN TRY
 
 
		SET @PIDXProfileToCompanyGuid=NEWID();
		SET @CreatedDate=ISNULL(@CreatedDate,sysdatetimeoffset())
 
		INSERT INTO [map].[tblPIDXProfileToCompany] 
		(
			[PIDXProfileToCompanyGuid]
		,	[PIDXProfileGuid]
		,	[CompanyPersonnelToShipToBillToGuid]
		,	[SiteGuid]
		,	[SellerID]
		,	[ShipperID]
		,	[ConsigneeNumber]
		,	[DenialOverride]
		,	[UnavailableOverride]
		,	[CreatedDate]
		,	[CreatedBy]
		,	[UpdatedDate]
		,	[UpdatedBy]
		)
		VALUES
		(
			@PIDXProfileToCompanyGuid
		,	@PIDXProfileGuid
		,	@CompanyPersonnelToShipToBillToGuid
		,	@SiteGuid
		,	@SellerID
		,	@ShipperID
		,	@ConsigneeNumber
		,	@DenialOverride
		,	@UnavailableOverride
		,	@CreatedDate
		,	@CreatedBy
		,	@UpdatedDate
		,	@UpdatedBy
		)
 
		SELECT @_RowVersion=_RowVersion        
		FROM [map].[tblPIDXProfileToCompany]           
		WHERE PIDXProfileToCompanyGuid=@PIDXProfileToCompanyGuid;
	
 
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
						+ 'Procedure Name: gsp_PIDXProfileToCompanyInsertByPK' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
END     
	
