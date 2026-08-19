-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblStandingOffers
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ServerSelectUpdateConflicts_tblStandingOffers]
@StandingOfferGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [dbo].[tblStandingOffers].[StandingOfferPrice],[dbo].[tblStandingOffers].[EffectiveDate],[dbo].[tblStandingOffers].[ExpirationDate],[dbo].[tblStandingOffers].[CreatedBy],[dbo].[tblStandingOffers].[CreatedDate],[dbo].[tblStandingOffers].[UpdatedBy],[dbo].[tblStandingOffers].[UpdatedDate],[dbo].[tblStandingOffers].[LowerBound],[dbo].[tblStandingOffers].[UpperBound],[dbo].[tblStandingOffers].[ReferenceNumber],[dbo].[tblStandingOffers].[StandingOfferGuid],[dbo].[tblStandingOffers].[SiteGuid],[dbo].[tblStandingOffers].[ProductGuid],[dbo].[tblStandingOffers].[SupplierCompanyGuid],[dbo].[tblStandingOffers].[LocationIATAGuid], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [dbo].[tblStandingOffers]
            INNER JOIN [track].[tblStandingOffers] CT
                ON CT.PK_StandingOfferGuid = [dbo].[tblStandingOffers].[StandingOfferGuid]
        WHERE CT.PK_StandingOfferGuid = @StandingOfferGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
