CREATE FUNCTION [map].[udf_CheckUniquenessAdditiveProfile]
(@AdditiveProfileGuid uniqueidentifier, @SiteGuid uniqueidentifier)
RETURNS BIT
AS
BEGIN
	DECLARE @ID nvarchar(30)
			, @Exists bit
	SET @Exists = 1

	SET @ID = (SELECT ID FROM tblAdditiveProfiles e WHERE e.AdditiveProfileGuid = @AdditiveProfileGuid)
	IF 0 < (SELECT COUNT(*) FROM tblAdditiveProfiles e 
	RIGHT JOIN map.tblEntityAdditiveProfileToSite em ON em.SiteGuid = @SiteGuid AND em.AdditiveProfileGuid = e.AdditiveProfileGuid 
	WHERE e.AdditiveProfileGuid <> @AdditiveProfileGuid
	AND e.ID = @ID)
		SET @Exists = 0

	RETURN @Exists
END

