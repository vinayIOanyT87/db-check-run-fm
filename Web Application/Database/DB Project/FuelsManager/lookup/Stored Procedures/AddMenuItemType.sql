CREATE PROCEDURE [lookup].[AddMenuItemType]
	@menuItemIndex int,
	@menuCode nvarchar(100),
	@menuItemGuid uniqueidentifier
AS BEGIN
	IF (NOT EXISTS (SELECT * FROM lookup.tblMenuItemType WHERE MenuItemTypeIndex = @menuItemIndex))
	BEGIN
		INSERT INTO [lookup].[tblMenuItemType] (
			[MenuItemTypeIndex], 
			[MenuItemTypeCode], 
			[MenuItemTypeName], 
			[MenuItemTypeGuid], 
			[CreatedDate], 
			[CreatedBy], 
			[UpdatedDate], 
			[UpdatedBy]
		) VALUES (
			@menuItemIndex, 
			@menuCode, 
			@menuCode, 
			@menuItemGuid, 
			N'9/21/2012 4:08:03 PM -04:00', 
			N'Administrator', 
			N'9/21/2012 4:08:03 PM -04:00', 
			N'Administrator')
	END
	ELSE
	BEGIN
		UPDATE  [lookup].[tblMenuItemType] SET
			[MenuItemTypeCode] = @menuCode, 
			[MenuItemTypeName] = @menuCode, 
			[MenuItemTypeGuid] = @menuItemGuid, 
			[CreatedDate] =N'9/21/2012 4:08:03 PM -04:00' , 
			[CreatedBy] = N'Administrator', 
			[UpdatedDate] = N'9/21/2012 4:08:03 PM -04:00', 
			[UpdatedBy] = N'Administrator'
		WHERE MenuItemTypeIndex = @menuItemIndex
	END
END
