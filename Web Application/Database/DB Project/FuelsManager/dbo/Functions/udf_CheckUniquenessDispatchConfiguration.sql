

CREATE FUNCTION [dbo].[udf_CheckUniquenessDispatchConfiguration]
(@DispatchConfigurationGuid uniqueidentifier, @SiteGuid uniqueidentifier, @ID nvarchar(50))
RETURNS BIT
AS
BEGIN
	DECLARE @Exists bit
	SET @Exists = 1


	-- dbo.tblDispatchConfiguration
	IF 0 < (SELECT COUNT(*) FROM tblDispatchConfiguration e
	LEFT JOIN map.tblEntityDispatchConfigurationToSite em1 ON em1.DispatchConfigurationGuid = e.DispatchConfigurationGuid
	RIGHT JOIN map.tblEntityDispatchConfigurationToSite em2 ON em2.DispatchConfigurationGuid = @DispatchConfigurationGuid AND em2.SiteGuid = em1.SiteGuid
	WHERE e.DispatchConfigurationGuid <> @DispatchConfigurationGuid
	AND ID = @ID)
		SET @Exists = 0

	RETURN @Exists
END
