-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : map.tblCompanyShipToToBillTo
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ServerSelectUpdateConflicts_tblCompanyShipToToBillTo]
@CompanyShipToToBillToGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [map].[tblCompanyShipToToBillTo].[CompanyShipToToBillToGuid],[map].[tblCompanyShipToToBillTo].[CompanyGuid],[map].[tblCompanyShipToToBillTo].[CompanyBillToToShipperGuid],[map].[tblCompanyShipToToBillTo].[SiteGuid],[map].[tblCompanyShipToToBillTo].[ID],[map].[tblCompanyShipToToBillTo].[CreatedDate],[map].[tblCompanyShipToToBillTo].[CreatedBy],[map].[tblCompanyShipToToBillTo].[UpdatedDate],[map].[tblCompanyShipToToBillTo].[UpdatedBy], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [map].[tblCompanyShipToToBillTo]
            INNER JOIN [track].[tblCompanyShipToToBillTo] CT
                ON CT.PK_CompanyShipToToBillToGuid = [map].[tblCompanyShipToToBillTo].[CompanyShipToToBillToGuid]
        WHERE CT.PK_CompanyShipToToBillToGuid = @CompanyShipToToBillToGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
