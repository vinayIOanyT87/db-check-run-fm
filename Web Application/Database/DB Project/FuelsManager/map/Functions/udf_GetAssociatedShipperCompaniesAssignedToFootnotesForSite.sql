CREATE FUNCTION [map].[udf_GetAssociatedShipperCompaniesAssignedToFootnotesForSite](
@sync_context_site_guid uniqueidentifier,
@only_where_company_guid_is_null bit
)
RETURNS @tblCompaniesAssignedToFootnote TABLE
(
    [ApplicationStringToFootNoteShipperGuid] [uniqueidentifier]
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
            SELECT [map].tblApplicationStringToFootNoteShipper.ApplicationStringToFootNoteShipperGuid, data1.[CompanyToSiteGuid], [map].tblApplicationStringToFootNoteShipper.[CompanyGuid], tas.[SiteGuid] 'OwnerSiteGuid', MAPCT.FootNoteToSiteGuid
                FROM [map].tblApplicationStringToFootNoteShipper
				JOIN tblApplicationString as tas
				ON tas.ApplicationStringGuid =  [map].tblApplicationStringToFootNoteShipper.ApplicationStringGuid
				INNER JOIN [map].[tblEntityFootNoteToSite] MAPCT
				ON MAPCT.ApplicationStringGuid = tas.ApplicationStringGuid
				AND MAPCT.SiteGuid = @sync_context_site_guid
                    INNER JOIN (SELECT [CompanyToSiteGuid],[CompanyGuid],[OwnerSiteGuid],[AssignedToSiteGuid] FROM [dbo].[udf_GetAssignedCompanyListForSite](@sync_context_site_guid)) data1
                        ON [map].tblApplicationStringToFootNoteShipper.[CompanyGuid] = data1.[CompanyGuid]
                    INNER JOIN [track].[tblEntityCompanyToSite] MAPCT2
                        ON MAPCT2.PK_CompanyToSiteGuid = data1.[CompanyToSiteGuid]
                WHERE [map].tblApplicationStringToFootNoteShipper.[CompanyGuid] IS NOT NULL
    END
    ELSE
    BEGIN
        INSERT INTO @tblCompaniesAssignedToFootnote
            SELECT [map].tblApplicationStringToFootNoteShipper.ApplicationStringToFootNoteShipperGuid, null, [map].tblApplicationStringToFootNoteShipper.[CompanyGuid], tas.[SiteGuid] 'OwnerSiteGuid', MAPCT.FootNoteToSiteGuid
                FROM [map].tblApplicationStringToFootNoteShipper
				JOIN tblApplicationString as tas
				ON tas.ApplicationStringGuid =  [map].tblApplicationStringToFootNoteShipper.ApplicationStringGuid
				INNER JOIN [map].[tblEntityFootNoteToSite] MAPCT
				ON MAPCT.ApplicationStringGuid = tas.ApplicationStringGuid
				AND MAPCT.SiteGuid = @sync_context_site_guid
                WHERE [map].tblApplicationStringToFootNoteShipper.[CompanyGuid] IS NULL
    END

    RETURN;
END