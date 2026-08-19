/****** Object:  UserDefinedFunction [dbo].[udf_GetAssignedApplicationStringCertificateListForSite]    Script Date: 4/22/2014 8:54:37 AM ******/

CREATE FUNCTION [dbo].[udf_GetAssignedApplicationStringCertificateListForSite](
@sync_context_site_guid uniqueidentifier
)
RETURNS @tblApplicationStringList TABLE
(
	[ApplicationStringGuid] [uniqueidentifier]
)
AS
BEGIN
	INSERT INTO @tblApplicationStringList 
		SELECT [dbo].[tblApplicationString].[ApplicationStringGuid]
			FROM [dbo].[tblApplicationString]
			WHERE [dbo].[tblApplicationString].[LookupApplicationStringTypeIndex] = 16 AND [dbo].[tblApplicationString].[SiteGuid] = @sync_context_site_guid

	RETURN;
END