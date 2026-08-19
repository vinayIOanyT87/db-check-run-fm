-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : map.tblPIDXProfileToCompany
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ClientSelectUpdateConflicts_tblPIDXProfileToCompany]
@PIDXProfileToCompanyGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [map].[tblPIDXProfileToCompany].[PIDXProfileToCompanyGuid],[map].[tblPIDXProfileToCompany].[PIDXProfileGuid],[map].[tblPIDXProfileToCompany].[CompanyPersonnelToShipToBillToGuid],[map].[tblPIDXProfileToCompany].[SiteGuid],[map].[tblPIDXProfileToCompany].[SellerID],[map].[tblPIDXProfileToCompany].[ShipperID],[map].[tblPIDXProfileToCompany].[ConsigneeNumber],[map].[tblPIDXProfileToCompany].[DenialOverride],[map].[tblPIDXProfileToCompany].[UnavailableOverride],[map].[tblPIDXProfileToCompany].[CreatedDate],[map].[tblPIDXProfileToCompany].[CreatedBy],[map].[tblPIDXProfileToCompany].[UpdatedDate],[map].[tblPIDXProfileToCompany].[UpdatedBy], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [map].[tblPIDXProfileToCompany]
            INNER JOIN [track].[tblPIDXProfileToCompany] CT
                ON CT.PK_PIDXProfileToCompanyGuid = [map].[tblPIDXProfileToCompany].[PIDXProfileToCompanyGuid]
        WHERE CT.PK_PIDXProfileToCompanyGuid = @PIDXProfileToCompanyGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
