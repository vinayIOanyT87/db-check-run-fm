CREATE FUNCTION [map].[udf_GetAssociatedProductsAssignedToFootnotesForSite](
@sync_context_site_guid uniqueidentifier,
@only_where_company_guid_is_null bit
)
RETURNS @tblProductsAssignedToFootnote TABLE
(
    [ApplicationStringToFootNoteProductGuid] [uniqueidentifier]
    ,[ProductToSiteGuid] [uniqueidentifier]
    ,[ProductGuid] [uniqueidentifier]
    ,[OwnerSiteGuid] [uniqueidentifier]
	,[FootNoteToSiteGuid] [uniqueidentifier]
)
AS
BEGIN
    -- Product guid is null when Footnote is mapped to "All" companies.
    IF (@only_where_company_guid_is_null = 0)
    BEGIN
        INSERT INTO @tblProductsAssignedToFootnote
            SELECT [map].tblApplicationStringToFootNoteProduct.ApplicationStringToFootNoteProductGuid, data1.[ProductToSiteGuid], [map].tblApplicationStringToFootNoteProduct.ProductGuid, tas.[SiteGuid] 'OwnerSiteGuid', MAPCT.FootNoteToSiteGuid
                FROM [map].tblApplicationStringToFootNoteProduct
				JOIN tblApplicationString as tas
				ON tas.ApplicationStringGuid =  [map].tblApplicationStringToFootNoteProduct.ApplicationStringGuid
				INNER JOIN [map].[tblEntityFootNoteToSite] MAPCT
				ON MAPCT.ApplicationStringGuid = tas.ApplicationStringGuid
				AND MAPCT.SiteGuid = @sync_context_site_guid
                    INNER JOIN (SELECT [ProductToSiteGuid],[ProductGuid],[OwnerSiteGuid],[AssignedToSiteGuid] FROM [dbo].[udf_GetAssignedProductListForSite](@sync_context_site_guid)) data1
                        ON [map].tblApplicationStringToFootNoteProduct.[ProductGuid] = data1.[ProductGuid]
                    INNER JOIN [track].[tblEntityProductToSite] MAPCT2
                        ON MAPCT2.PK_ProductToSiteGuid = data1.[ProductToSiteGuid]
                WHERE [map].tblApplicationStringToFootNoteProduct.[ProductGuid] IS NOT NULL
    END
    ELSE
    BEGIN
        INSERT INTO @tblProductsAssignedToFootnote
            SELECT [map].tblApplicationStringToFootNoteProduct.ApplicationStringToFootNoteProductGuid, null, [map].tblApplicationStringToFootNoteProduct.ProductGuid, tas.[SiteGuid] 'OwnerSiteGuid', MAPCT.FootNoteToSiteGuid
                FROM [map].tblApplicationStringToFootNoteProduct
				JOIN tblApplicationString as tas
				ON tas.ApplicationStringGuid =  [map].tblApplicationStringToFootNoteProduct.ApplicationStringGuid
				INNER JOIN [map].[tblEntityFootNoteToSite] MAPCT
				ON MAPCT.ApplicationStringGuid = tas.ApplicationStringGuid
				AND MAPCT.SiteGuid = @sync_context_site_guid
                WHERE [map].tblApplicationStringToFootNoteProduct.[ProductGuid] IS NULL
    END

    RETURN;
END