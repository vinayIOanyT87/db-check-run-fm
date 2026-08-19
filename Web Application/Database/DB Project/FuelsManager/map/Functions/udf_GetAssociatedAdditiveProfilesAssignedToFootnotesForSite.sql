CREATE FUNCTION [map].[udf_GetAssociatedAdditiveProfilesAssignedToFootnotesForSite](
@sync_context_site_guid uniqueidentifier,
@only_where_company_guid_is_null bit
)
RETURNS @tblAdditiveProfilesAssignedToFootnote TABLE
(
    [ApplicationStringToFootNoteAdditiveProfileGuid] [uniqueidentifier]
    ,[AdditiveProfileToSiteGuid] [uniqueidentifier]
    ,[AdditiveProfileGuid] [uniqueidentifier]
    ,[OwnerSiteGuid] [uniqueidentifier]
	,[FootNoteToSiteGuid] [uniqueidentifier]
)
AS
BEGIN
    -- Additive Profile guid is null when Footnote is mapped to "All" companies.
    IF (@only_where_company_guid_is_null = 0)
    BEGIN
        INSERT INTO @tblAdditiveProfilesAssignedToFootnote
            SELECT [map].[tblApplicationStringToFootNoteAdditiveProfile].ApplicationStringToFootNoteAdditiveProfileGuid, data1.[AdditiveProfileToSiteGuid], [map].[tblApplicationStringToFootNoteAdditiveProfile].AdditiveProfileGuid, tas.[SiteGuid] 'OwnerSiteGuid', MAPCT.FootNoteToSiteGuid
                FROM [map].[tblApplicationStringToFootNoteAdditiveProfile]
				JOIN tblApplicationString as tas
				ON tas.ApplicationStringGuid =  [map].[tblApplicationStringToFootNoteAdditiveProfile].ApplicationStringGuid
				INNER JOIN [map].[tblEntityFootNoteToSite] MAPCT
					ON MAPCT.ApplicationStringGuid = tas.ApplicationStringGuid
				AND MAPCT.SiteGuid = @sync_context_site_guid
                    INNER JOIN (SELECT [AdditiveProfileToSiteGuid],[AdditiveProfileGuid],[OwnerSiteGuid],[AssignedToSiteGuid] FROM [dbo].[udf_GetAssignedAdditiveProfileListForSite](@sync_context_site_guid)) data1
                        ON [map].[tblApplicationStringToFootNoteAdditiveProfile].AdditiveProfileGuid = data1.[AdditiveProfileGuid]
                    INNER JOIN [track].[tblEntityAdditiveProfileToSite] MAPCT2
                        ON MAPCT2.PK_AdditiveProfileToSiteGuid = data1.[AdditiveProfileToSiteGuid]
                WHERE [map].[tblApplicationStringToFootNoteAdditiveProfile].AdditiveProfileGuid IS NOT NULL
    END
    ELSE
    BEGIN
        INSERT INTO @tblAdditiveProfilesAssignedToFootnote
            SELECT [map].[tblApplicationStringToFootNoteAdditiveProfile].ApplicationStringToFootNoteAdditiveProfileGuid, null, [map].[tblApplicationStringToFootNoteAdditiveProfile].AdditiveProfileGuid, tas.[SiteGuid] 'OwnerSiteGuid', MAPCT.FootNoteToSiteGuid
                FROM [map].[tblApplicationStringToFootNoteAdditiveProfile]
				JOIN tblApplicationString as tas
				ON tas.ApplicationStringGuid =  [map].[tblApplicationStringToFootNoteAdditiveProfile].ApplicationStringGuid
				INNER JOIN [map].[tblEntityFootNoteToSite] MAPCT
				ON MAPCT.ApplicationStringGuid = tas.ApplicationStringGuid
				AND MAPCT.SiteGuid = @sync_context_site_guid
                WHERE [map].[tblApplicationStringToFootNoteAdditiveProfile].AdditiveProfileGuid IS NULL
    END

    RETURN;
END