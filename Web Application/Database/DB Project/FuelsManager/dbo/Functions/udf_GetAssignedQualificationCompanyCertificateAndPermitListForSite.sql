CREATE FUNCTION [dbo].[udf_GetAssignedQualificationCompanyCertificateAndPermitListForSite](
@sync_context_site_guid uniqueidentifier
)
RETURNS @tblQualificationsList TABLE
(
	[CompanyCertificateAndPermitToSiteGuid] [uniqueidentifier]
	,[QualificationGuid] [uniqueidentifier]
	,[OwnerSiteGuid] [uniqueidentifier]
	,[AssignedToSiteGuid] [uniqueidentifier]
)
AS
BEGIN
	-- In the case of entity assignment, the current model always contains a self-assigned entity assignment back to the owning site so we can leverage this to find
	-- all assigned entities.
	--
	INSERT INTO @tblQualificationsList 
		SELECT [map].[tblEntityCompanyCertificateAndPermitToSite].[CompanyCertificateAndPermitToSiteGuid], [dbo].[tblQualifications].[QualificationGuid], [dbo].[tblQualifications].[SiteGuid] 'OwnerSiteGuid', [map].[tblEntityCompanyCertificateAndPermitToSite].[SiteGuid] 'AssignedToSiteGuid'
			FROM [map].[tblEntityCompanyCertificateAndPermitToSite]
				INNER JOIN [dbo].[tblQualifications]
					ON [map].[tblEntityCompanyCertificateAndPermitToSite].[QualificationGuid] = [dbo].[tblQualifications].[QualificationGuid]
			WHERE [map].[tblEntityCompanyCertificateAndPermitToSite].[SiteGuid] = @sync_context_site_guid

	RETURN;
END