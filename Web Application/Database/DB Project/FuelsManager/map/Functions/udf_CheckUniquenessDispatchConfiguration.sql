

CREATE FUNCTION [map].[udf_CheckUniquenessDispatchConfiguration]
(@DispatchConfigurationGuid uniqueidentifier, @SiteGuid uniqueidentifier)
RETURNS BIT
AS
BEGIN
	DECLARE @ID nvarchar(50)
			, @Exists bit
	SET @Exists = 1

	SET @ID = (SELECT ID FROM tblDispatchConfiguration e WHERE e.DispatchConfigurationGuid = @DispatchConfigurationGuid)
	IF 0 < (SELECT COUNT(*) FROM tblDispatchConfiguration e 
	RIGHT JOIN map.tblEntityDispatchConfigurationToSite em ON em.SiteGuid = @SiteGuid AND em.DispatchConfigurationGuid = e.DispatchConfigurationGuid 
	WHERE e.DispatchConfigurationGuid <> @DispatchConfigurationGuid
	AND e.ID = @ID)
		SET @Exists = 0

	RETURN @Exists
END
