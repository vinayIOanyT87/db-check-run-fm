-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : map.tblQualificationPersonQualificationToStation
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ServerSelectUpdateConflicts_tblQualificationPersonQualificationToStation]
@QualificationPersonQualificationToStationGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [map].[tblQualificationPersonQualificationToStation].[QualificationPersonQualificationToStationGuid],[map].[tblQualificationPersonQualificationToStation].[QualificationGuid],[map].[tblQualificationPersonQualificationToStation].[StationGuid],[map].[tblQualificationPersonQualificationToStation].[Sequence],[map].[tblQualificationPersonQualificationToStation].[Instructor],[map].[tblQualificationPersonQualificationToStation].[DateCompleted],[map].[tblQualificationPersonQualificationToStation].[DateDue],[map].[tblQualificationPersonQualificationToStation].[ExpirationDate],[map].[tblQualificationPersonQualificationToStation].[ID],[map].[tblQualificationPersonQualificationToStation].[Rating],[map].[tblQualificationPersonQualificationToStation].[HistoricalRecord],[map].[tblQualificationPersonQualificationToStation].[CreatedDate],[map].[tblQualificationPersonQualificationToStation].[CreatedBy],[map].[tblQualificationPersonQualificationToStation].[UpdatedDate],[map].[tblQualificationPersonQualificationToStation].[UpdatedBy], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [map].[tblQualificationPersonQualificationToStation]
            INNER JOIN [track].[tblQualificationPersonQualificationToStation] CT
                ON CT.PK_QualificationPersonQualificationToStationGuid = [map].[tblQualificationPersonQualificationToStation].[QualificationPersonQualificationToStationGuid]
        WHERE CT.PK_QualificationPersonQualificationToStationGuid = @QualificationPersonQualificationToStationGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
