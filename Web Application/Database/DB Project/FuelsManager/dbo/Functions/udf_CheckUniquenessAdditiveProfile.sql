CREATE FUNCTION [dbo].[udf_CheckUniquenessAdditiveProfile]
(@AdditiveProfileGuid uniqueidentifier, @SiteGuid uniqueidentifier, @ID nvarchar(30))
RETURNS BIT
AS
BEGIN
	DECLARE @Exists bit
	SET @Exists = 1


	-- dbo.tblAdditiveProfile
	IF 0 < (SELECT COUNT(*) FROM tblAdditiveProfiles e
	LEFT JOIN map.tblEntityAdditiveProfileToSite em1 ON em1.AdditiveProfileGuid = e.AdditiveProfileGuid
	RIGHT JOIN map.tblEntityAdditiveProfileToSite em2 ON em2.AdditiveProfileGuid = @AdditiveProfileGuid AND em2.SiteGuid = em1.SiteGuid
	WHERE e.AdditiveProfileGuid <> @AdditiveProfileGuid
	AND ID = @ID)
		SET @Exists = 0

	RETURN @Exists
END

