CREATE PROCEDURE [map].[gsp_CompanyOffLoadOwnerToManagerInsertByPK]
(
		@CompanyOffLoadOwnerToManagerGuid uniqueidentifier=NULL OUTPUT
	,	@CompanyGuid uniqueidentifier=NULL
	,	@AssignedToCompanyGuid uniqueidentifier=NULL
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
	-- Stored procedure: [map].[gsp_CompanyOffLoadOwnerToManagerInsertByPK] 
	-- Author: DBA - Auto generated
	-- Version/Date: 1.0.001 / 2014-02-05 15:58:32.6602767 -05:00
	-- Purpose: Insert into table [map].[tblCompanyOffLoadOwnerToManager]
	-- Notes:
	------------------------------------------------------------------------------------------------------
	SET NOCOUNT ON;
	BEGIN TRY
 
 
		SET @CompanyOffLoadOwnerToManagerGuid=NEWID();
		SET @CreatedDate=ISNULL(@CreatedDate,sysdatetimeoffset())
 
		INSERT INTO [map].[tblCompanyOffLoadOwnerToManager] 
		(
			[CompanyOffLoadOwnerToManagerGuid]
		,	[CompanyGuid]
		,	[AssignedToCompanyGuid]
		,	[SiteGuid]
		,	[ID]
		,	[CreatedDate]
		,	[CreatedBy]
		,	[UpdatedDate]
		,	[UpdatedBy]
		)
		VALUES
		(
			@CompanyOffLoadOwnerToManagerGuid
		,	@CompanyGuid
		,	@AssignedToCompanyGuid
		,	@SiteGuid
		,	@ID
		,	@CreatedDate
		,	@CreatedBy
		,	@UpdatedDate
		,	@UpdatedBy
		)
 
		SELECT @_RowVersion=_RowVersion        
		FROM [map].[tblCompanyOffLoadOwnerToManager]           
		WHERE CompanyOffLoadOwnerToManagerGuid=@CompanyOffLoadOwnerToManagerGuid;
	
 
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
						+ 'Procedure Name: gsp_CompanyOffLoadOwnerToManagerInsertByPK' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
END     
	
