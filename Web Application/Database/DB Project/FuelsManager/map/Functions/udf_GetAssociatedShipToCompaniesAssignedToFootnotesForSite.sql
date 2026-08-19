CREATE FUNCTION [map].[udf_GetAssociatedShipToCompaniesAssignedToFootnotesForSite](
@sync_context_site_guid uniqueidentifier,
@only_where_company_guid_is_null bit
)
RETURNS @tblCompaniesAssignedToFootnote TABLE
(
    [ApplicationStringToFootNoteShipToGuid] [uniqueidentifier]
    ,[CompanyToSiteGuid] [uniqueidentifier]
    ,[CompanyGuid] [uniqueidentifier]
    ,[OwnerSiteGuid] [uniqueidentifier]
	,[FootNoteToSiteGuid] [uniqueidentifier]
)
AS
BEGIN
    -- Company guid is null when Footnote is mapped to "All" companies.
    IF (@only_where_company_guid_is_null = 0)
    BEGIN
        INSERT INTO @tblCompaniesAssignedToFootnote
            SELECT [map].tblApplicationStringToFootNoteShipTo.ApplicationStringToFootNoteShipToGuid, data1.[CompanyToSiteGuid], [map].tblApplicationStringToFootNoteShipTo.[CompanyGuid], tas.[SiteGuid] 'OwnerSiteGuid', MAPCT.FootNoteToSiteGuid
                FROM [map].tblApplicationStringToFootNoteShipTo
				JOIN tblApplicationString as tas
				ON tas.ApplicationStringGuid =  [map].tblApplicationStringToFootNoteShipTo.ApplicationStringGuid
				INNER JOIN [map].[tblEntityFootNoteToSite] MAPCT
				ON MAPCT.ApplicationStringGuid = tas.ApplicationStringGuid
				AND MAPCT.SiteGuid = @sync_context_site_guid
                    INNER JOIN (SELECT [CompanyToSiteGuid],[CompanyGuid],[OwnerSiteGuid],[AssignedToSiteGuid] FROM [dbo].[udf_GetAssignedCompanyListForSite](@sync_context_site_guid)) data1
                        ON [map].tblApplicationStringToFootNoteShipTo.[CompanyGuid] = data1.[CompanyGuid]
                    INNER JOIN [track].[tblEntityCompanyToSite] MAPCT2
                        ON MAPCT2.PK_CompanyToSiteGuid = data1.[CompanyToSiteGuid]
                WHERE [map].tblApplicationStringToFootNoteShipTo.[CompanyGuid] IS NOT NULL
    END
    ELSE
    BEGIN
        INSERT INTO @tblCompaniesAssignedToFootnote
            SELECT [map].tblApplicationStringToFootNoteShipTo.ApplicationStringToFootNoteShipToGuid, null, [map].tblApplicationStringToFootNoteShipTo.[CompanyGuid], tas.[SiteGuid] 'OwnerSiteGuid', MAPCT.FootNoteToSiteGuid
                FROM [map].tblApplicationStringToFootNoteShipTo
				JOIN tblApplicationString as tas
				ON tas.ApplicationStringGuid =  [map].tblApplicationStringToFootNoteShipTo.ApplicationStringGuid
				INNER JOIN [map].[tblEntityFootNoteToSite] MAPCT
				ON MAPCT.ApplicationStringGuid = tas.ApplicationStringGuid
				AND MAPCT.SiteGuid = @sync_context_site_guid
                WHERE [map].tblApplicationStringToFootNoteShipTo.[CompanyGuid] IS NULL
    END

    RETURN;
END