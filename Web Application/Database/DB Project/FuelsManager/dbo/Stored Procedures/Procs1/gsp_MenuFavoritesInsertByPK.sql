CREATE PROCEDURE [dbo].[gsp_MenuFavoritesInsertByPK]
(
		@MenuFavoriteGuid uniqueidentifier=NULL OUTPUT
	,	@UserGuid uniqueidentifier=NULL
	,	@IsQuickLink bit=NULL
	,	@CustomName nvarchar(100)=NULL
	,	@DisplayOrder int=NULL
	,	@MenuItemType int=NULL
	,	@DynamicMenuItemGuid uniqueidentifier=NULL
	,	@CreatedDate datetimeoffset(7)=NULL
	,	@CreatedBy udtUserID=NULL
	,	@UpdatedDate datetimeoffset(7)=NULL
	,	@UpdatedBy udtUserID=NULL
	,	@_RowVersion timestamp=NULL OUTPUT
)
AS
BEGIN
	------------------------------------------------------------------------------------------------------
	-- Stored procedure: [dbo].[gsp_MenuFavoritesInsertByPK] 
	-- Author: DBA - Auto generated
	-- Version/Date: 1.0.001 / 2014-02-05 15:58:32.2722767 -05:00
	-- Purpose: Insert into table [dbo].[tblMenuFavorites]
	-- Notes:
	------------------------------------------------------------------------------------------------------
	SET NOCOUNT ON;
	BEGIN TRY
 
 
		SET @MenuFavoriteGuid=NEWID();
		SET @CreatedDate=ISNULL(@CreatedDate,sysdatetimeoffset())
 
		INSERT INTO [dbo].[tblMenuFavorites] 
		(
			[MenuFavoriteGuid]
		,	[UserGuid]
		,	[IsQuickLink]
		,	[CustomName]
		,	[DisplayOrder]
		,	[MenuItemType]
		,	[DynamicMenuItemGuid]
		,	[CreatedDate]
		,	[CreatedBy]
		,	[UpdatedDate]
		,	[UpdatedBy]
		)
		VALUES
		(
			@MenuFavoriteGuid
		,	@UserGuid
		,	@IsQuickLink
		,	@CustomName
		,	@DisplayOrder
		,	@MenuItemType
		,	@DynamicMenuItemGuid
		,	@CreatedDate
		,	@CreatedBy
		,	@UpdatedDate
		,	@UpdatedBy
		)
 
		SELECT @_RowVersion=_RowVersion        
		FROM [dbo].[tblMenuFavorites]           
		WHERE MenuFavoriteGuid=@MenuFavoriteGuid;
	
 
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
						+ 'Procedure Name: gsp_MenuFavoritesInsertByPK' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
END     
	
