CREATE PROCEDURE [dbo].[gsp_AllocationsInsertByPK]
(
		@AllocationGuid uniqueidentifier=NULL OUTPUT
	,	@EffectiveDate datetimeoffset(7)=NULL
	,	@ExpirationDate datetimeoffset(7)=NULL
	,	@LoadWarning float=NULL
	,	@LoadDenial float=NULL
	,	@ContractNumber nvarchar(10)=NULL
	,	@AllocationGroupIndex int=NULL
	,	@LastAllocationResetDate datetimeoffset(7)=NULL
	,	@CreatedDate datetimeoffset(7)=NULL
	,	@CreatedBy udtUserID=NULL
	,	@UpdatedDate datetimeoffset(7)=NULL
	,	@UpdatedBy udtUserID=NULL
	,	@CompanyBillToToShipperGuid uniqueidentifier=NULL
	,	@CompanyLoadOwnerToManagerGuid uniqueidentifier=NULL
	,	@CompanyOffLoadOwnerToManagerGuid uniqueidentifier=NULL
	,	@CompanyShipperToOwnerGuid uniqueidentifier=NULL
	,	@CompanyShipToToBillToGuid uniqueidentifier=NULL
	,	@CompanySupplierToOwnerGuid uniqueidentifier=NULL
	,	@SiteGuid uniqueidentifier=NULL
	,	@LookupCompanyMapTypeIndex int=NULL
	,	@AllocationGroupApplicationStringGuid uniqueidentifier=NULL
	,	@_RowVersion timestamp=NULL OUTPUT
)
AS
BEGIN
	------------------------------------------------------------------------------------------------------
	-- Stored procedure: [dbo].[gsp_AllocationsInsertByPK] 
	-- Author: DBA - Auto generated
	-- Version/Date: 1.0.001 / 2014-02-05 15:58:32.0432767 -05:00
	-- Purpose: Insert into table [dbo].[tblAllocations]
	-- Notes:
	------------------------------------------------------------------------------------------------------
	SET NOCOUNT ON;
	BEGIN TRY
 
 
		SET @AllocationGuid=NEWID();
		SET @CreatedDate=ISNULL(@CreatedDate,sysdatetimeoffset())
 
		INSERT INTO [dbo].[tblAllocations] 
		(
			[AllocationGuid]
		,	[EffectiveDate]
		,	[ExpirationDate]
		,	[LoadWarning]
		,	[LoadDenial]
		,	[ContractNumber]
		,	[AllocationGroupIndex]
		,	[LastAllocationResetDate]
		,	[CreatedDate]
		,	[CreatedBy]
		,	[UpdatedDate]
		,	[UpdatedBy]
		,	[CompanyBillToToShipperGuid]
		,	[CompanyLoadOwnerToManagerGuid]
		,	[CompanyOffLoadOwnerToManagerGuid]
		,	[CompanyShipperToOwnerGuid]
		,	[CompanyShipToToBillToGuid]
		,	[CompanySupplierToOwnerGuid]
		,	[SiteGuid]
		,	[LookupCompanyMapTypeIndex]
		,	[AllocationGroupApplicationStringGuid]
		)
		VALUES
		(
			@AllocationGuid
		,	@EffectiveDate
		,	@ExpirationDate
		,	@LoadWarning
		,	@LoadDenial
		,	@ContractNumber
		,	@AllocationGroupIndex
		,	@LastAllocationResetDate
		,	@CreatedDate
		,	@CreatedBy
		,	@UpdatedDate
		,	@UpdatedBy
		,	@CompanyBillToToShipperGuid
		,	@CompanyLoadOwnerToManagerGuid
		,	@CompanyOffLoadOwnerToManagerGuid
		,	@CompanyShipperToOwnerGuid
		,	@CompanyShipToToBillToGuid
		,	@CompanySupplierToOwnerGuid
		,	@SiteGuid
		,	@LookupCompanyMapTypeIndex
		,	@AllocationGroupApplicationStringGuid
		)
 
		SELECT @_RowVersion=_RowVersion        
		FROM [dbo].[tblAllocations]           
		WHERE AllocationGuid=@AllocationGuid;
	
 
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
						+ 'Procedure Name: gsp_AllocationsInsertByPK' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
END     
	
