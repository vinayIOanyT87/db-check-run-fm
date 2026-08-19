-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : map.tblCompanyShipperToOwner
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ClientSelectUpdateConflicts_tblCompanyShipperToOwner]
@CompanyShipperToOwnerGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [map].[tblCompanyShipperToOwner].[CompanyShipperToOwnerGuid],[map].[tblCompanyShipperToOwner].[CompanyGuid],[map].[tblCompanyShipperToOwner].[CompanyLoadOwnerToManagerGuid],[map].[tblCompanyShipperToOwner].[SiteGuid],[map].[tblCompanyShipperToOwner].[ID],[map].[tblCompanyShipperToOwner].[CreatedDate],[map].[tblCompanyShipperToOwner].[CreatedBy],[map].[tblCompanyShipperToOwner].[UpdatedDate],[map].[tblCompanyShipperToOwner].[UpdatedBy], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [map].[tblCompanyShipperToOwner]
            INNER JOIN [track].[tblCompanyShipperToOwner] CT
                ON CT.PK_CompanyShipperToOwnerGuid = [map].[tblCompanyShipperToOwner].[CompanyShipperToOwnerGuid]
        WHERE CT.PK_CompanyShipperToOwnerGuid = @CompanyShipperToOwnerGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
