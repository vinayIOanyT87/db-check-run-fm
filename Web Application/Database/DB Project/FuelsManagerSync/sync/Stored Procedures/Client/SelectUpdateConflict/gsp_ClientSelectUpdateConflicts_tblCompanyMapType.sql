-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : lookup.tblCompanyMapType
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ClientSelectUpdateConflicts_tblCompanyMapType]
@CompanyMapTypeIndex int
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [lookup].[tblCompanyMapType].[CompanyMapTypeIndex],[lookup].[tblCompanyMapType].[CompanyMapTypeCode],[lookup].[tblCompanyMapType].[CompanyMapTypeName],[lookup].[tblCompanyMapType].[CompanyMapTypeGuid],[lookup].[tblCompanyMapType].[CreatedDate],[lookup].[tblCompanyMapType].[CreatedBy],[lookup].[tblCompanyMapType].[UpdatedDate],[lookup].[tblCompanyMapType].[UpdatedBy], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [lookup].[tblCompanyMapType]
            INNER JOIN [track].[tblCompanyMapType] CT
                ON CT.PK_CompanyMapTypeIndex = [lookup].[tblCompanyMapType].[CompanyMapTypeIndex]
        WHERE CT.PK_CompanyMapTypeIndex = @CompanyMapTypeIndex
    ORDER BY CT.UpdatedRowVersion ASC
END
