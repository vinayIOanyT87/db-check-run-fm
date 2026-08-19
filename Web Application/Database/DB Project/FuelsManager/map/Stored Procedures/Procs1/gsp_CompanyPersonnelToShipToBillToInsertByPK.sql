CREATE PROCEDURE [map].[gsp_CompanyPersonnelToShipToBillToInsertByPK]
(
		@CompanyPersonnelToShipToBillToGuid uniqueidentifier=NULL OUTPUT
	,	@PersonnelGuid uniqueidentifier=NULL
	,	@CompanyShipToToBillToGuid uniqueidentifier=NULL
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
	-- Stored procedure: [map].[gsp_CompanyPersonnelToShipToBillToInsertByPK] 
	-- Author: DBA - Auto generated
	-- Version/Date: 1.0.001 / 2014-02-05 15:58:32.6622767 -05:00
	-- Purpose: Insert into table [map].[tblCompanyPersonnelToShipToBillTo]
	-- Notes:
	------------------------------------------------------------------------------------------------------
	SET NOCOUNT ON;
	BEGIN TRY
 
 
		SET @CompanyPersonnelToShipToBillToGuid=NEWID();
		SET @CreatedDate=ISNULL(@CreatedDate,sysdatetimeoffset())
 
		INSERT INTO [map].[tblCompanyPersonnelToShipToBillTo] 
		(
			[CompanyPersonnelToShipToBillToGuid]
		,	[PersonnelGuid]
		,	[CompanyShipToToBillToGuid]
		,	[SiteGuid]
		,	[ID]
		,	[CreatedDate]
		,	[CreatedBy]
		,	[UpdatedDate]
		,	[UpdatedBy]
		)
		VALUES
		(
			@CompanyPersonnelToShipToBillToGuid
		,	@PersonnelGuid
		,	@CompanyShipToToBillToGuid
		,	@SiteGuid
		,	@ID
		,	@CreatedDate
		,	@CreatedBy
		,	@UpdatedDate
		,	@UpdatedBy
		)
 
		SELECT @_RowVersion=_RowVersion        
		FROM [map].[tblCompanyPersonnelToShipToBillTo]           
		WHERE CompanyPersonnelToShipToBillToGuid=@CompanyPersonnelToShipToBillToGuid;
	
 
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
						+ 'Procedure Name: gsp_CompanyPersonnelToShipToBillToInsertByPK' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
END     
	
