-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : map.tblCompanyPersonnelToShipToBillTo
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ServerSelectUpdateConflicts_tblCompanyPersonnelToShipToBillTo]
@CompanyPersonnelToShipToBillToGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [map].[tblCompanyPersonnelToShipToBillTo].[CompanyPersonnelToShipToBillToGuid],[map].[tblCompanyPersonnelToShipToBillTo].[PersonnelGuid],[map].[tblCompanyPersonnelToShipToBillTo].[CompanyShipToToBillToGuid],[map].[tblCompanyPersonnelToShipToBillTo].[SiteGuid],[map].[tblCompanyPersonnelToShipToBillTo].[ID],[map].[tblCompanyPersonnelToShipToBillTo].[CreatedDate],[map].[tblCompanyPersonnelToShipToBillTo].[CreatedBy],[map].[tblCompanyPersonnelToShipToBillTo].[UpdatedDate],[map].[tblCompanyPersonnelToShipToBillTo].[UpdatedBy], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [map].[tblCompanyPersonnelToShipToBillTo]
            INNER JOIN [track].[tblCompanyPersonnelToShipToBillTo] CT
                ON CT.PK_CompanyPersonnelToShipToBillToGuid = [map].[tblCompanyPersonnelToShipToBillTo].[CompanyPersonnelToShipToBillToGuid]
        WHERE CT.PK_CompanyPersonnelToShipToBillToGuid = @CompanyPersonnelToShipToBillToGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
