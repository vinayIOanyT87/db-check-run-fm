CREATE PROCEDURE [dbo].[gsp_OwnerCloseoutInsertByPK]
(
		@OwnerCloseoutGuid uniqueidentifier=NULL OUTPUT
	,	@Site nvarchar(30)=NULL
	,	@ManagerName nvarchar(100)=NULL
	,	@ProductName nvarchar(30)=NULL
	,	@CloseoutDate date=NULL
	,	@OwnerName nvarchar(100)=NULL
	,	@GrossBookInventory float=NULL
	,	@NetBookInventory float=NULL
	,	@CreatedDate datetimeoffset(7)=NULL
	,	@CreatedBy udtUserID=NULL
	,	@UpdatedDate datetimeoffset(7)=NULL
	,	@UpdatedBy udtUserID=NULL
	,	@GrossBookPrice float=NULL
	,	@NetBookPrice float=NULL
	,	@TransVersion bigint=NULL
	,	@MassBookInventory float=NULL
	,	@MassBookPrice float=NULL
	,	@SiteGuid uniqueidentifier=NULL
	,	@ManagerCompanyGuid uniqueidentifier=NULL
	,	@OwnerCompanyGuid uniqueidentifier=NULL
	,	@ProductGuid uniqueidentifier=NULL
	,	@_RowVersion timestamp=NULL OUTPUT
)
AS
BEGIN
	------------------------------------------------------------------------------------------------------
	-- Stored procedure: [dbo].[gsp_OwnerCloseoutInsertByPK] 
	-- Author: DBA - Auto generated
	-- Version/Date: 1.0.001 / 2014-02-05 15:58:32.3092767 -05:00
	-- Purpose: Insert into table [dbo].[tblOwnerCloseout]
	-- Notes:
	------------------------------------------------------------------------------------------------------
	SET NOCOUNT ON;
	BEGIN TRY
 
 
		SET @OwnerCloseoutGuid=NEWID();
		SET @CreatedDate=ISNULL(@CreatedDate,sysdatetimeoffset())
 
		INSERT INTO [dbo].[tblOwnerCloseout] 
		(
			[OwnerCloseoutGuid]
		,	[Site]
		,	[ManagerName]
		,	[ProductName]
		,	[CloseoutDate]
		,	[OwnerName]
		,	[GrossBookInventory]
		,	[NetBookInventory]
		,	[CreatedDate]
		,	[CreatedBy]
		,	[UpdatedDate]
		,	[UpdatedBy]
		,	[GrossBookPrice]
		,	[NetBookPrice]
		,	[TransVersion]
		,	[MassBookInventory]
		,	[MassBookPrice]
		,	[SiteGuid]
		,	[ManagerCompanyGuid]
		,	[OwnerCompanyGuid]
		,	[ProductGuid]
		)
		VALUES
		(
			@OwnerCloseoutGuid
		,	@Site
		,	@ManagerName
		,	@ProductName
		,	@CloseoutDate
		,	@OwnerName
		,	@GrossBookInventory
		,	@NetBookInventory
		,	@CreatedDate
		,	@CreatedBy
		,	@UpdatedDate
		,	@UpdatedBy
		,	@GrossBookPrice
		,	@NetBookPrice
		,	@TransVersion
		,	@MassBookInventory
		,	@MassBookPrice
		,	@SiteGuid
		,	@ManagerCompanyGuid
		,	@OwnerCompanyGuid
		,	@ProductGuid
		)
 
		SELECT @_RowVersion=_RowVersion        
		FROM [dbo].[tblOwnerCloseout]           
		WHERE OwnerCloseoutGuid=@OwnerCloseoutGuid;
	
 
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
						+ 'Procedure Name: gsp_OwnerCloseoutInsertByPK' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
END     
	
