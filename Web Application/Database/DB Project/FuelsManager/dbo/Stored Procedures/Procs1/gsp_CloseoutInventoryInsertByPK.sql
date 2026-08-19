CREATE PROCEDURE [dbo].[gsp_CloseoutInventoryInsertByPK]
(
		@CloseoutInventoryGuid uniqueidentifier=NULL OUTPUT
	,	@Site nvarchar(30)=NULL
	,	@CloseoutDate date=NULL
	,	@ProductName nvarchar(30)=NULL
	,	@ManagerName nvarchar(100)=NULL
	,	@GrossBookInventory float=NULL
	,	@NetBookInventory float=NULL
	,	@GrossPhysicalInventory float=NULL
	,	@NetPhysicalInventory float=NULL
	,	@GrossVariance float=NULL
	,	@NetVariance float=NULL
	,	@CreatedDate datetimeoffset(7)=NULL
	,	@CreatedBy udtUserID=NULL
	,	@UpdatedDate datetimeoffset(7)=NULL
	,	@UpdatedBy udtUserID=NULL
	,	@GrossBookPrice float=NULL
	,	@NetBookPrice float=NULL
	,	@GrossPhysicalPrice float=NULL
	,	@NetPhysicalPrice float=NULL
	,	@TransVersion bigint=NULL
	,	@MassBookInventory float=NULL
	,	@MassPhysicalInventory float=NULL
	,	@MassVariance float=NULL
	,	@MassBookPrice float=NULL
	,	@MassPhysicalPrice float=NULL
	,	@SiteGuid uniqueidentifier=NULL
	,	@ManagerCompanyGuid uniqueidentifier=NULL
	,	@ProductGuid uniqueidentifier=NULL
	,	@_RowVersion timestamp=NULL OUTPUT
)
AS
BEGIN
	------------------------------------------------------------------------------------------------------
	-- Stored procedure: [dbo].[gsp_CloseoutInventoryInsertByPK] 
	-- Author: DBA - Auto generated
	-- Version/Date: 1.0.001 / 2014-02-05 15:58:32.0982767 -05:00
	-- Purpose: Insert into table [dbo].[tblCloseoutInventory]
	-- Notes:
	------------------------------------------------------------------------------------------------------
	SET NOCOUNT ON;
	BEGIN TRY
 
 
		SET @CloseoutInventoryGuid=NEWID();
		SET @CreatedDate=ISNULL(@CreatedDate,sysdatetimeoffset())
 
		INSERT INTO [dbo].[tblCloseoutInventory] 
		(
			[CloseoutInventoryGuid]
		,	[Site]
		,	[CloseoutDate]
		,	[ProductName]
		,	[ManagerName]
		,	[GrossBookInventory]
		,	[NetBookInventory]
		,	[GrossPhysicalInventory]
		,	[NetPhysicalInventory]
		,	[GrossVariance]
		,	[NetVariance]
		,	[CreatedDate]
		,	[CreatedBy]
		,	[UpdatedDate]
		,	[UpdatedBy]
		,	[GrossBookPrice]
		,	[NetBookPrice]
		,	[GrossPhysicalPrice]
		,	[NetPhysicalPrice]
		,	[TransVersion]
		,	[MassBookInventory]
		,	[MassPhysicalInventory]
		,	[MassVariance]
		,	[MassBookPrice]
		,	[MassPhysicalPrice]
		,	[SiteGuid]
		,	[ManagerCompanyGuid]
		,	[ProductGuid]
		)
		VALUES
		(
			@CloseoutInventoryGuid
		,	@Site
		,	@CloseoutDate
		,	@ProductName
		,	@ManagerName
		,	@GrossBookInventory
		,	@NetBookInventory
		,	@GrossPhysicalInventory
		,	@NetPhysicalInventory
		,	@GrossVariance
		,	@NetVariance
		,	@CreatedDate
		,	@CreatedBy
		,	@UpdatedDate
		,	@UpdatedBy
		,	@GrossBookPrice
		,	@NetBookPrice
		,	@GrossPhysicalPrice
		,	@NetPhysicalPrice
		,	@TransVersion
		,	@MassBookInventory
		,	@MassPhysicalInventory
		,	@MassVariance
		,	@MassBookPrice
		,	@MassPhysicalPrice
		,	@SiteGuid
		,	@ManagerCompanyGuid
		,	@ProductGuid
		)
 
		SELECT @_RowVersion=_RowVersion        
		FROM [dbo].[tblCloseoutInventory]           
		WHERE CloseoutInventoryGuid=@CloseoutInventoryGuid;
	
 
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
						+ 'Procedure Name: gsp_CloseoutInventoryInsertByPK' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
END     
	
