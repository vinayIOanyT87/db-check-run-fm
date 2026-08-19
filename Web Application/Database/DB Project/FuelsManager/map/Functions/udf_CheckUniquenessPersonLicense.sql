

CREATE FUNCTION [map].[udf_CheckUniquenessPersonLicense]
(@QualificationGuid uniqueidentifier, @SiteGuid uniqueidentifier)
RETURNS BIT
AS
BEGIN
	DECLARE @ID nvarchar(80)
			, @Exists bit
	SET @Exists = 1

	SET @ID = (SELECT ID FROM [dbo].[tblQualifications] e WHERE e.QualificationGuid = @QualificationGuid AND e.LookupQualificationTypeIndex = 4)
	IF 0 < (SELECT COUNT(*) FROM [dbo].[tblQualifications] e 
	RIGHT JOIN map.tblEntityPersonnelLicenseToSite em ON em.SiteGuid = @SiteGuid AND em.QualificationGuid = e.QualificationGuid 
	WHERE e.QualificationGuid <> @QualificationGuid
	AND e.ID = @ID AND e.LookupQualificationTypeIndex = 4)
		SET @Exists = 0

	RETURN @Exists
END
