CREATE PROCEDURE [map].[gsp_CompanySupplierToOwnerInsertByPK]
(
		@CompanySupplierToOwnerGuid uniqueidentifier=NULL OUTPUT
	,	@CompanyGuid uniqueidentifier=NULL
	,	@CompanyOffLoadOwnerToManagerGuid uniqueidentifier=NULL
	,	@SiteGuid uniqueidentifier=NULL
	,	@ID nvarchar(30)=NULL
	,	@CreatedDate datetimeoffset(7)=NULL
	,	@CreatedBy udtUserID=NULL
	,	@UpdatedDate datetimeoffset(7)=NULL
	,	@UpdatedBy udtUserID=NULL
	,	@_RowVersion timestamp=NULL OUTPUT
)
AS
BEGIN
	------------------------------------------------------------------------------------------------------
	-- Stored procedure: [map].[gsp_CompanySupplierToOwnerInsertByPK] 
	-- Author: DBA - Auto generated
	-- Version/Date: 1.0.001 / 2014-02-05 15:58:32.6692767 -05:00
	-- Purpose: Insert into table [map].[tblCompanySupplierToOwner]
	-- Notes:
	------------------------------------------------------------------------------------------------------
	SET NOCOUNT ON;
	BEGIN TRY
 
 
		SET @CompanySupplierToOwnerGuid=NEWID();
		SET @CreatedDate=ISNULL(@CreatedDate,sysdatetimeoffset())
 
		INSERT INTO [map].[tblCompanySupplierToOwner] 
		(
			[CompanySupplierToOwnerGuid]
		,	[CompanyGuid]
		,	[CompanyOffLoadOwnerToManagerGuid]
		,	[SiteGuid]
		,	[ID]
		,	[CreatedDate]
		,	[CreatedBy]
		,	[UpdatedDate]
		,	[UpdatedBy]
		)
		VALUES
		(
			@CompanySupplierToOwnerGuid
		,	@CompanyGuid
		,	@CompanyOffLoadOwnerToManagerGuid
		,	@SiteGuid
		,	@ID
		,	@CreatedDate
		,	@CreatedBy
		,	@UpdatedDate
		,	@UpdatedBy
		)
 
		SELECT @_RowVersion=_RowVersion        
		FROM [map].[tblCompanySupplierToOwner]           
		WHERE CompanySupplierToOwnerGuid=@CompanySupplierToOwnerGuid;
	
 
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
						+ 'Procedure Name: gsp_CompanySupplierToOwnerInsertByPK' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
END     
	
