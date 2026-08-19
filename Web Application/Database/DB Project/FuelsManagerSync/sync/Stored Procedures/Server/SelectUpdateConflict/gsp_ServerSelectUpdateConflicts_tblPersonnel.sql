-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblPersonnel
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ServerSelectUpdateConflicts_tblPersonnel]
@PersonnelGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [dbo].[tblPersonnel].[PersonID],[dbo].[tblPersonnel].[CardNumber],[dbo].[tblPersonnel].[FirstName],[dbo].[tblPersonnel].[MiddleName],[dbo].[tblPersonnel].[LastName],[dbo].[tblPersonnel].[Title],[dbo].[tblPersonnel].[Department],[dbo].[tblPersonnel].[Address1],[dbo].[tblPersonnel].[Address2],[dbo].[tblPersonnel].[City],[dbo].[tblPersonnel].[State],[dbo].[tblPersonnel].[Zip],[dbo].[tblPersonnel].[Country],[dbo].[tblPersonnel].[Phone1],[dbo].[tblPersonnel].[Phone2],[dbo].[tblPersonnel].[AssignmentDate],[dbo].[tblPersonnel].[SupervisionDate],[dbo].[tblPersonnel].[SSAN],[dbo].[tblPersonnel].[BirthDate],[dbo].[tblPersonnel].[PayRate],[dbo].[tblPersonnel].[LaborRate1],[dbo].[tblPersonnel].[LaborRate2],[dbo].[tblPersonnel].[LaborRate3],[dbo].[tblPersonnel].[LaborRate4],[dbo].[tblPersonnel].[Status],[dbo].[tblPersonnel].[Email],[dbo].[tblPersonnel].[ResponsibleOfficer],[dbo].[tblPersonnel].[Shift],[dbo].[tblPersonnel].[PINNumber],[dbo].[tblPersonnel].[PINRequired],[dbo].[tblPersonnel].[LockedOut],[dbo].[tblPersonnel].[LockedOutReason],[dbo].[tblPersonnel].[LockedOutDate],[dbo].[tblPersonnel].[LastActivityDate],[dbo].[tblPersonnel].[CardedIn],[dbo].[tblPersonnel].[ShortCardNumber],[dbo].[tblPersonnel].[CreatedDate],[dbo].[tblPersonnel].[CreatedBy],[dbo].[tblPersonnel].[UpdatedDate],[dbo].[tblPersonnel].[UpdatedBy],[dbo].[tblPersonnel].[OnFileSignature],[dbo].[tblPersonnel].[UserData1],[dbo].[tblPersonnel].[UserData2],[dbo].[tblPersonnel].[UserData3],[dbo].[tblPersonnel].[UserData4],[dbo].[tblPersonnel].[UserData5],[dbo].[tblPersonnel].[UserData6],[dbo].[tblPersonnel].[UserData7],[dbo].[tblPersonnel].[UserData8],[dbo].[tblPersonnel].[UserData9],[dbo].[tblPersonnel].[UserData10],[dbo].[tblPersonnel].[UserData11],[dbo].[tblPersonnel].[UserData12],[dbo].[tblPersonnel].[UserData13],[dbo].[tblPersonnel].[UserData14],[dbo].[tblPersonnel].[UserData15],[dbo].[tblPersonnel].[UserData16],[dbo].[tblPersonnel].[UserData17],[dbo].[tblPersonnel].[UserData18],[dbo].[tblPersonnel].[UserData19],[dbo].[tblPersonnel].[UserData20],[dbo].[tblPersonnel].[UserData21],[dbo].[tblPersonnel].[UserData22],[dbo].[tblPersonnel].[UserData23],[dbo].[tblPersonnel].[UserData24],[dbo].[tblPersonnel].[InhibitInactivityLockout],[dbo].[tblPersonnel].[PersonnelGuid],[dbo].[tblPersonnel].[SiteGuid],[dbo].[tblPersonnel].[CompanyGuid],[dbo].[tblPersonnel].[SupervisorPersonnelGuid],[dbo].[tblPersonnel].[UserGuid],[dbo].[tblPersonnel].[AssignedEquipmentGuid],[dbo].[tblPersonnel].[_MasterRecordGuid],[dbo].[tblPersonnel].[HiddenDate], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [dbo].[tblPersonnel]
            INNER JOIN [track].[tblPersonnel] CT
                ON CT.PK_PersonnelGuid = [dbo].[tblPersonnel].[PersonnelGuid]
        WHERE CT.PK_PersonnelGuid = @PersonnelGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
