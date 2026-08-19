-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : lookup.tblVesselType
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ServerSelectUpdateConflicts_tblVesselType]
@VesselTypeIndex int
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [lookup].[tblVesselType].[VesselTypeIndex],[lookup].[tblVesselType].[VesselTypeCode],[lookup].[tblVesselType].[VesselTypeName],[lookup].[tblVesselType].[VesselTypeGuid],[lookup].[tblVesselType].[CreatedDate],[lookup].[tblVesselType].[CreatedBy],[lookup].[tblVesselType].[UpdatedDate],[lookup].[tblVesselType].[UpdatedBy], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [lookup].[tblVesselType]
            INNER JOIN [track].[tblVesselType] CT
                ON CT.PK_VesselTypeIndex = [lookup].[tblVesselType].[VesselTypeIndex]
        WHERE CT.PK_VesselTypeIndex = @VesselTypeIndex
    ORDER BY CT.UpdatedRowVersion ASC
END
