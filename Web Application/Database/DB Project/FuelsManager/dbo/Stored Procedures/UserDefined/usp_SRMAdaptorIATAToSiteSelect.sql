
/*
=============================================
Author: Ryan Hill
Create date: 7/13/12
Description:

Read a Service Request Messaging Adaptor IATA Code to Site Mapping record
You can retrieve:
	Mappings matching the primary key 
	Mappings matching all key fields (AdaptorGuid, SiteGuid, and IATAGuid) 
	Enabled Mappings for a particular adaptor and IATA code (used for duplicate checking)
	Mappings defined for a particlar adaptor and site
	or all records
=============================================
*/
CREATE PROCEDURE [dbo].[usp_SRMAdaptorIATAToSiteSelect]
(
	@SRMAdaptorIATAToSiteGuid UNIQUEIDENTIFIER = NULL,
	@SRMAdaptorGuid UNIQUEIDENTIFIER = NULL,
	@SiteGuid UNIQUEIDENTIFIER = NULL,
	@IATAGuid UNIQUEIDENTIFIER = NULL
)
AS
BEGIN
	SET NOCOUNT ON
	
	IF(@SRMAdaptorIATAToSiteGuid IS NOT NULL)
	BEGIN
		SELECT 
			SRMAdaptorIATAToSiteGuid,
			SRMAdaptorGuid,
			map.tblSRMAdaptorIATAToSite.SiteGuid,
			SiteID = tblSites.ID,
			map.tblSRMAdaptorIATAToSite.IATAGuid,
			IATAID,
			IsEnabled,
			map.tblSRMAdaptorIATAToSite.CreatedDate,
			map.tblSRMAdaptorIATAToSite.CreatedBy,
			map.tblSRMAdaptorIATAToSite.UpdatedDate,
			map.tblSRMAdaptorIATAToSite.UpdatedBy
		FROM map.tblSRMAdaptorIATAToSite WITH (NOLOCK)	
			INNER JOIN tblSites WITH (NOLOCK) ON tblSites.SiteGuid = map.tblSRMAdaptorIATAToSite.SiteGuid 
			INNER JOIN tblIATA WITH (NOLOCK) ON tblIATA.IATAGuid = map.tblSRMAdaptorIATAToSite.IATAGUid 
		WHERE SRMAdaptorIATAToSiteGuid = @SRMAdaptorIATAToSiteGuid	
	END
	ELSE IF(@SRMAdaptorGuid IS NOT NULL AND @SiteGuid IS NOT NULL AND @IATAGuid IS NOT NULL)
	BEGIN
		SELECT 
			SRMAdaptorIATAToSiteGuid,
			SRMAdaptorGuid,
			map.tblSRMAdaptorIATAToSite.SiteGuid,
			SiteID = tblSites.ID,
			map.tblSRMAdaptorIATAToSite.IATAGuid,
			IATAID,
			IsEnabled,
			map.tblSRMAdaptorIATAToSite.CreatedDate,
			map.tblSRMAdaptorIATAToSite.CreatedBy,
			map.tblSRMAdaptorIATAToSite.UpdatedDate,
			map.tblSRMAdaptorIATAToSite.UpdatedBy
		FROM map.tblSRMAdaptorIATAToSite WITH (NOLOCK)	
			INNER JOIN tblSites WITH (NOLOCK) ON tblSites.SiteGuid = map.tblSRMAdaptorIATAToSite.SiteGuid 
			INNER JOIN tblIATA WITH (NOLOCK) ON tblIATA.IATAGuid = map.tblSRMAdaptorIATAToSite.IATAGUid 
		WHERE map.tblSRMAdaptorIATAToSite.SRMAdaptorGuid = @SRMAdaptorGuid	
			AND map.tblSRMAdaptorIATAToSite.SiteGuid = @SiteGuid
			AND map.tblSRMAdaptorIATAToSite.IATAGuid = @IATAGuid
	END
	ELSE IF(@SRMAdaptorGuid IS NOT NULL AND @IATAGuid IS NOT NULL)
	BEGIN
		SELECT 
			SRMAdaptorIATAToSiteGuid,
			SRMAdaptorGuid,
			map.tblSRMAdaptorIATAToSite.SiteGuid,
			SiteID = tblSites.ID,
			map.tblSRMAdaptorIATAToSite.IATAGuid,
			IATAID,
			IsEnabled,
			map.tblSRMAdaptorIATAToSite.CreatedDate,
			map.tblSRMAdaptorIATAToSite.CreatedBy,
			map.tblSRMAdaptorIATAToSite.UpdatedDate,
			map.tblSRMAdaptorIATAToSite.UpdatedBy
		FROM map.tblSRMAdaptorIATAToSite WITH (NOLOCK)	
			INNER JOIN tblSites WITH (NOLOCK) ON tblSites.SiteGuid = map.tblSRMAdaptorIATAToSite.SiteGuid 
			INNER JOIN tblIATA WITH (NOLOCK) ON tblIATA.IATAGuid = map.tblSRMAdaptorIATAToSite.IATAGUid 
		WHERE map.tblSRMAdaptorIATAToSite.SRMAdaptorGuid = @SRMAdaptorGuid	
			AND map.tblSRMAdaptorIATAToSite.IATAGuid = @IATAGuid
			AND IsEnabled = 1
	END
	ELSE IF(@SRMAdaptorGuid IS NOT NULL AND @SiteGuid IS NOT NULL)
	BEGIN
		SELECT 
			SRMAdaptorIATAToSiteGuid,
			SRMAdaptorGuid,
			map.tblSRMAdaptorIATAToSite.SiteGuid,
			SiteID = tblSites.ID,
			map.tblSRMAdaptorIATAToSite.IATAGuid,
			IATAID,
			IsEnabled,
			map.tblSRMAdaptorIATAToSite.CreatedDate,
			map.tblSRMAdaptorIATAToSite.CreatedBy,
			map.tblSRMAdaptorIATAToSite.UpdatedDate,
			map.tblSRMAdaptorIATAToSite.UpdatedBy
		FROM map.tblSRMAdaptorIATAToSite WITH (NOLOCK)	
			INNER JOIN tblSites WITH (NOLOCK) ON tblSites.SiteGuid = map.tblSRMAdaptorIATAToSite.SiteGuid 
			INNER JOIN tblIATA WITH (NOLOCK) ON tblIATA.IATAGuid = map.tblSRMAdaptorIATAToSite.IATAGUid 
		WHERE SRMAdaptorGuid = @SRMAdaptorGuid 
			AND (map.tblSRMAdaptorIATAToSite.SiteGuid = @SiteGuid OR map.tblSRMAdaptorIATAToSite.SiteGuid IN 
				(SELECT ChildSiteGuid FROM map.tblSiteToSite WITH (NOLOCK) WHERE ParentSiteGuid = @SiteGuid))
	END
	ELSE -- Retrieve all Service Request Messaging Adaptor IATA Code to Site Mapping records
	BEGIN
		SELECT 
			SRMAdaptorIATAToSiteGuid,
			SRMAdaptorGuid,
			map.tblSRMAdaptorIATAToSite.SiteGuid,
			SiteID = tblSites.ID,
			map.tblSRMAdaptorIATAToSite.IATAGuid,
			IATAID,
			IsEnabled,
			map.tblSRMAdaptorIATAToSite.CreatedDate,
			map.tblSRMAdaptorIATAToSite.CreatedBy,
			map.tblSRMAdaptorIATAToSite.UpdatedDate,
			map.tblSRMAdaptorIATAToSite.UpdatedBy
		FROM map.tblSRMAdaptorIATAToSite WITH (NOLOCK)	
			INNER JOIN tblSites WITH (NOLOCK) ON tblSites.SiteGuid = map.tblSRMAdaptorIATAToSite.SiteGuid 
			INNER JOIN tblIATA WITH (NOLOCK) ON tblIATA.IATAGuid = map.tblSRMAdaptorIATAToSite.IATAGUid 
	END
END