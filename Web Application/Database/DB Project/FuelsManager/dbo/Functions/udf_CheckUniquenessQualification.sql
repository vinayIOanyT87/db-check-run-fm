

CREATE FUNCTION [dbo].[udf_CheckUniquenessQualification]
(@QualificationGuid uniqueidentifier, @SiteGuid uniqueidentifier, @ID nvarchar(80), @LookupQualificationTypeIndex int)
RETURNS BIT
AS
BEGIN
	DECLARE @Exists bit
	SET @Exists = 1


	IF @LookupQualificationTypeIndex = 0
	BEGIN
		IF 0 < (SELECT COUNT(*) FROM [dbo].[tblQualifications] e
		LEFT JOIN map.tblEntityCompanyCertificateAndPermitToSite em1 ON em1.QualificationGuid = e.QualificationGuid
		RIGHT JOIN map.tblEntityCompanyCertificateAndPermitToSite em2 ON em2.QualificationGuid = @QualificationGuid AND em2.SiteGuid = em1.SiteGuid
		WHERE e.QualificationGuid <> @QualificationGuid
		AND ID = @ID
		AND LookupQualificationTypeIndex = @LookupQualificationTypeIndex)
			SET @Exists = 0
	END

	ELSE IF @LookupQualificationTypeIndex = 1
	BEGIN
		IF 0 < (SELECT COUNT(*) FROM [dbo].[tblQualifications] e
		LEFT JOIN map.tblEntityEquipmentTestAndInspectionToSite em1 ON em1.QualificationGuid = e.QualificationGuid
		RIGHT JOIN map.tblEntityEquipmentTestAndInspectionToSite em2 ON em2.QualificationGuid = @QualificationGuid AND em2.SiteGuid = em1.SiteGuid
		WHERE e.QualificationGuid <> @QualificationGuid
		AND ID = @ID
		AND LookupQualificationTypeIndex = @LookupQualificationTypeIndex)
			SET @Exists = 0
	END

	ELSE IF @LookupQualificationTypeIndex = 2
	BEGIN
		IF 0 < (SELECT COUNT(*) FROM [dbo].[tblQualifications] e
		LEFT JOIN map.tblEntityEquipmentTagAndLicenseToSite em1 ON em1.QualificationGuid = e.QualificationGuid
		RIGHT JOIN map.tblEntityEquipmentTagAndLicenseToSite em2 ON em2.QualificationGuid = @QualificationGuid AND em2.SiteGuid = em1.SiteGuid
		WHERE e.QualificationGuid <> @QualificationGuid
		AND ID = @ID
		AND LookupQualificationTypeIndex = @LookupQualificationTypeIndex)
			SET @Exists = 0
	END

	ELSE IF @LookupQualificationTypeIndex = 3
	BEGIN
		IF 0 < (SELECT COUNT(*) FROM [dbo].[tblQualifications] e
		LEFT JOIN map.tblEntityPersonnelQualificationToSite em1 ON em1.QualificationGuid = e.QualificationGuid
		RIGHT JOIN map.tblEntityPersonnelQualificationToSite em2 ON em2.QualificationGuid = @QualificationGuid AND em2.SiteGuid = em1.SiteGuid
		WHERE e.QualificationGuid <> @QualificationGuid
		AND ID = @ID
		AND LookupQualificationTypeIndex = @LookupQualificationTypeIndex)
			SET @Exists = 0
	END

	ELSE IF @LookupQualificationTypeIndex = 4
	BEGIN
		IF 0 < (SELECT COUNT(*) FROM [dbo].[tblQualifications] e
		LEFT JOIN map.tblEntityPersonnelLicenseToSite em1 ON em1.QualificationGuid = e.QualificationGuid
		RIGHT JOIN map.tblEntityPersonnelLicenseToSite em2 ON em2.QualificationGuid = @QualificationGuid AND em2.SiteGuid = em1.SiteGuid
		WHERE e.QualificationGuid <> @QualificationGuid
		AND ID = @ID
		AND LookupQualificationTypeIndex = @LookupQualificationTypeIndex)
			SET @Exists = 0
	END

	ELSE IF @LookupQualificationTypeIndex = 5
	BEGIN
		IF 0 < (SELECT COUNT(*) FROM [dbo].[tblQualifications] e
		LEFT JOIN map.tblEntityPersonnelTrainingToSite em1 ON em1.QualificationGuid = e.QualificationGuid
		RIGHT JOIN map.tblEntityPersonnelTrainingToSite em2 ON em2.QualificationGuid = @QualificationGuid AND em2.SiteGuid = em1.SiteGuid
		WHERE e.QualificationGuid <> @QualificationGuid
		AND ID = @ID
		AND LookupQualificationTypeIndex = @LookupQualificationTypeIndex)
			SET @Exists = 0
	END

	RETURN @Exists
END
