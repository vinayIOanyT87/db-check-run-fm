-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblPersonnel
-- Description: Apply Inserts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ClientApplyIncrementalInserts_tblPersonnel]
@sync_client_id_binary binary(16),
@sync_client_id uniqueidentifier,
@sync_last_received_anchor bigint,
@sync_start_daterange datetimeoffset(7),
@sync_end_daterange datetimeoffset(7),
@sync_filter_by_daterange bit,
@PersonID nvarchar(50),
@CardNumber nvarchar(30),
@FirstName nvarchar(20),
@MiddleName nvarchar(20),
@LastName nvarchar(30),
@Title nvarchar(50),
@Department nvarchar(20),
@Address1 nvarchar(50),
@Address2 nvarchar(50),
@City nvarchar(60),
@State nvarchar(20),
@Zip nvarchar(10),
@Country nvarchar(20),
@Phone1 nvarchar(50),
@Phone2 nvarchar(50),
@AssignmentDate datetimeoffset(7),
@SupervisionDate datetimeoffset(7),
@SSAN nvarchar(11),
@BirthDate datetimeoffset(7),
@PayRate money,
@LaborRate1 float,
@LaborRate2 float,
@LaborRate3 float,
@LaborRate4 float,
@Status smallint,
@Email nvarchar(50),
@ResponsibleOfficer bit,
@Shift smallint,
@PINNumber varbinary(256),
@PINRequired bit,
@LockedOut bit,
@LockedOutReason nvarchar(80),
@LockedOutDate datetimeoffset(7),
@LastActivityDate datetimeoffset(7),
@CardedIn bit,
@ShortCardNumber nvarchar(6),
@CreatedDate datetimeoffset(7),
@CreatedBy nvarchar(100),
@UpdatedDate datetimeoffset(7),
@UpdatedBy nvarchar(100),
@OnFileSignature varbinary(max),
@UserData1 nvarchar(60),
@UserData2 nvarchar(60),
@UserData3 nvarchar(60),
@UserData4 nvarchar(60),
@UserData5 nvarchar(60),
@UserData6 nvarchar(60),
@UserData7 nvarchar(60),
@UserData8 nvarchar(60),
@UserData9 nvarchar(60),
@UserData10 nvarchar(60),
@UserData11 nvarchar(60),
@UserData12 nvarchar(60),
@UserData13 nvarchar(60),
@UserData14 nvarchar(60),
@UserData15 nvarchar(60),
@UserData16 nvarchar(60),
@UserData17 nvarchar(60),
@UserData18 nvarchar(60),
@UserData19 nvarchar(60),
@UserData20 nvarchar(60),
@UserData21 nvarchar(60),
@UserData22 nvarchar(60),
@UserData23 nvarchar(60),
@UserData24 nvarchar(60),
@InhibitInactivityLockout bit,
@PersonnelGuid uniqueidentifier,
@SiteGuid uniqueidentifier,
@CompanyGuid uniqueidentifier,
@SupervisorPersonnelGuid uniqueidentifier,
@UserGuid uniqueidentifier,
@AssignedEquipmentGuid uniqueidentifier,
@_MasterRecordGuid uniqueidentifier,
@HiddenDate datetimeoffset(7),
@sync_row_count int out,
@sync_table_name nvarchar(512)
AS
BEGIN
    DECLARE @minValidVersion BigInt 

    ;   MERGE [dbo].[tblPersonnel] AS existingData
        USING (SELECT @PersonID 'PersonID',@CardNumber 'CardNumber',@FirstName 'FirstName',@MiddleName 'MiddleName',@LastName 'LastName',@Title 'Title',@Department 'Department',@Address1 'Address1',@Address2 'Address2',@City 'City',@State 'State',@Zip 'Zip',@Country 'Country',@Phone1 'Phone1',@Phone2 'Phone2',@AssignmentDate 'AssignmentDate',@SupervisionDate 'SupervisionDate',@SSAN 'SSAN',@BirthDate 'BirthDate',@PayRate 'PayRate',@LaborRate1 'LaborRate1',@LaborRate2 'LaborRate2',@LaborRate3 'LaborRate3',@LaborRate4 'LaborRate4',@Status 'Status',@Email 'Email',@ResponsibleOfficer 'ResponsibleOfficer',@Shift 'Shift',@PINNumber 'PINNumber',@PINRequired 'PINRequired',@LockedOut 'LockedOut',@LockedOutReason 'LockedOutReason',@LockedOutDate 'LockedOutDate',@LastActivityDate 'LastActivityDate',@CardedIn 'CardedIn',@ShortCardNumber 'ShortCardNumber',@CreatedDate 'CreatedDate',@CreatedBy 'CreatedBy',@UpdatedDate 'UpdatedDate',@UpdatedBy 'UpdatedBy',@OnFileSignature 'OnFileSignature',@UserData1 'UserData1',@UserData2 'UserData2',@UserData3 'UserData3',@UserData4 'UserData4',@UserData5 'UserData5',@UserData6 'UserData6',@UserData7 'UserData7',@UserData8 'UserData8',@UserData9 'UserData9',@UserData10 'UserData10',@UserData11 'UserData11',@UserData12 'UserData12',@UserData13 'UserData13',@UserData14 'UserData14',@UserData15 'UserData15',@UserData16 'UserData16',@UserData17 'UserData17',@UserData18 'UserData18',@UserData19 'UserData19',@UserData20 'UserData20',@UserData21 'UserData21',@UserData22 'UserData22',@UserData23 'UserData23',@UserData24 'UserData24',@InhibitInactivityLockout 'InhibitInactivityLockout',@PersonnelGuid 'PersonnelGuid',@SiteGuid 'SiteGuid',@CompanyGuid 'CompanyGuid',@SupervisorPersonnelGuid 'SupervisorPersonnelGuid',@UserGuid 'UserGuid',@AssignedEquipmentGuid 'AssignedEquipmentGuid',@_MasterRecordGuid '_MasterRecordGuid',@HiddenDate 'HiddenDate'
                ) AS remoteChanges ([PersonID],[CardNumber],[FirstName],[MiddleName],[LastName],[Title],[Department],[Address1],[Address2],[City],[State],[Zip],[Country],[Phone1],[Phone2],[AssignmentDate],[SupervisionDate],[SSAN],[BirthDate],[PayRate],[LaborRate1],[LaborRate2],[LaborRate3],[LaborRate4],[Status],[Email],[ResponsibleOfficer],[Shift],[PINNumber],[PINRequired],[LockedOut],[LockedOutReason],[LockedOutDate],[LastActivityDate],[CardedIn],[ShortCardNumber],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy],[OnFileSignature],[UserData1],[UserData2],[UserData3],[UserData4],[UserData5],[UserData6],[UserData7],[UserData8],[UserData9],[UserData10],[UserData11],[UserData12],[UserData13],[UserData14],[UserData15],[UserData16],[UserData17],[UserData18],[UserData19],[UserData20],[UserData21],[UserData22],[UserData23],[UserData24],[InhibitInactivityLockout],[PersonnelGuid],[SiteGuid],[CompanyGuid],[SupervisorPersonnelGuid],[UserGuid],[AssignedEquipmentGuid],[_MasterRecordGuid],[HiddenDate])
        ON (existingData.[PersonnelGuid] = remoteChanges.[PersonnelGuid])
        WHEN MATCHED AND (remoteChanges.CreatedDate > existingData.CreatedDate OR remoteChanges.UpdatedDate > existingData.UpdatedDate) THEN
            UPDATE SET [PersonID] = remoteChanges.[PersonID]
                       ,[CardNumber] = remoteChanges.[CardNumber]
                       ,[FirstName] = remoteChanges.[FirstName]
                       ,[MiddleName] = remoteChanges.[MiddleName]
                       ,[LastName] = remoteChanges.[LastName]
                       ,[Title] = remoteChanges.[Title]
                       ,[Department] = remoteChanges.[Department]
                       ,[Address1] = remoteChanges.[Address1]
                       ,[Address2] = remoteChanges.[Address2]
                       ,[City] = remoteChanges.[City]
                       ,[State] = remoteChanges.[State]
                       ,[Zip] = remoteChanges.[Zip]
                       ,[Country] = remoteChanges.[Country]
                       ,[Phone1] = remoteChanges.[Phone1]
                       ,[Phone2] = remoteChanges.[Phone2]
                       ,[AssignmentDate] = remoteChanges.[AssignmentDate]
                       ,[SupervisionDate] = remoteChanges.[SupervisionDate]
                       ,[SSAN] = remoteChanges.[SSAN]
                       ,[BirthDate] = remoteChanges.[BirthDate]
                       ,[PayRate] = remoteChanges.[PayRate]
                       ,[LaborRate1] = remoteChanges.[LaborRate1]
                       ,[LaborRate2] = remoteChanges.[LaborRate2]
                       ,[LaborRate3] = remoteChanges.[LaborRate3]
                       ,[LaborRate4] = remoteChanges.[LaborRate4]
                       ,[Status] = remoteChanges.[Status]
                       ,[Email] = remoteChanges.[Email]
                       ,[ResponsibleOfficer] = remoteChanges.[ResponsibleOfficer]
                       ,[Shift] = remoteChanges.[Shift]
                       ,[PINNumber] = remoteChanges.[PINNumber]
                       ,[PINRequired] = remoteChanges.[PINRequired]
                       ,[LockedOut] = remoteChanges.[LockedOut]
                       ,[LockedOutReason] = remoteChanges.[LockedOutReason]
                       ,[LockedOutDate] = remoteChanges.[LockedOutDate]
                       ,[LastActivityDate] = remoteChanges.[LastActivityDate]
                       ,[CardedIn] = remoteChanges.[CardedIn]
                       ,[ShortCardNumber] = remoteChanges.[ShortCardNumber]
                       ,[CreatedDate] = remoteChanges.[CreatedDate]
                       ,[CreatedBy] = remoteChanges.[CreatedBy]
                       ,[UpdatedDate] = remoteChanges.[UpdatedDate]
                       ,[UpdatedBy] = remoteChanges.[UpdatedBy]
                       ,[OnFileSignature] = remoteChanges.[OnFileSignature]
                       ,[UserData1] = remoteChanges.[UserData1]
                       ,[UserData2] = remoteChanges.[UserData2]
                       ,[UserData3] = remoteChanges.[UserData3]
                       ,[UserData4] = remoteChanges.[UserData4]
                       ,[UserData5] = remoteChanges.[UserData5]
                       ,[UserData6] = remoteChanges.[UserData6]
                       ,[UserData7] = remoteChanges.[UserData7]
                       ,[UserData8] = remoteChanges.[UserData8]
                       ,[UserData9] = remoteChanges.[UserData9]
                       ,[UserData10] = remoteChanges.[UserData10]
                       ,[UserData11] = remoteChanges.[UserData11]
                       ,[UserData12] = remoteChanges.[UserData12]
                       ,[UserData13] = remoteChanges.[UserData13]
                       ,[UserData14] = remoteChanges.[UserData14]
                       ,[UserData15] = remoteChanges.[UserData15]
                       ,[UserData16] = remoteChanges.[UserData16]
                       ,[UserData17] = remoteChanges.[UserData17]
                       ,[UserData18] = remoteChanges.[UserData18]
                       ,[UserData19] = remoteChanges.[UserData19]
                       ,[UserData20] = remoteChanges.[UserData20]
                       ,[UserData21] = remoteChanges.[UserData21]
                       ,[UserData22] = remoteChanges.[UserData22]
                       ,[UserData23] = remoteChanges.[UserData23]
                       ,[UserData24] = remoteChanges.[UserData24]
                       ,[InhibitInactivityLockout] = remoteChanges.[InhibitInactivityLockout]
                       ,[SiteGuid] = remoteChanges.[SiteGuid]
                       ,[CompanyGuid] = remoteChanges.[CompanyGuid]
                       ,[SupervisorPersonnelGuid] = remoteChanges.[SupervisorPersonnelGuid]
                       ,[UserGuid] = remoteChanges.[UserGuid]
                       ,[AssignedEquipmentGuid] = remoteChanges.[AssignedEquipmentGuid]
                       ,[_MasterRecordGuid] = remoteChanges.[_MasterRecordGuid]
                       ,[HiddenDate] = remoteChanges.[HiddenDate]

        WHEN NOT MATCHED THEN
            INSERT ([PersonID],[CardNumber],[FirstName],[MiddleName],[LastName],[Title],[Department],[Address1],[Address2],[City],[State],[Zip],[Country],[Phone1],[Phone2],[AssignmentDate],[SupervisionDate],[SSAN],[BirthDate],[PayRate],[LaborRate1],[LaborRate2],[LaborRate3],[LaborRate4],[Status],[Email],[ResponsibleOfficer],[Shift],[PINNumber],[PINRequired],[LockedOut],[LockedOutReason],[LockedOutDate],[LastActivityDate],[CardedIn],[ShortCardNumber],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy],[OnFileSignature],[UserData1],[UserData2],[UserData3],[UserData4],[UserData5],[UserData6],[UserData7],[UserData8],[UserData9],[UserData10],[UserData11],[UserData12],[UserData13],[UserData14],[UserData15],[UserData16],[UserData17],[UserData18],[UserData19],[UserData20],[UserData21],[UserData22],[UserData23],[UserData24],[InhibitInactivityLockout],[PersonnelGuid],[SiteGuid],[CompanyGuid],[SupervisorPersonnelGuid],[UserGuid],[AssignedEquipmentGuid],[_MasterRecordGuid],[HiddenDate])
                VALUES (@PersonID,@CardNumber,@FirstName,@MiddleName,@LastName,@Title,@Department,@Address1,@Address2,@City,@State,@Zip,@Country,@Phone1,@Phone2,@AssignmentDate,@SupervisionDate,@SSAN,@BirthDate,@PayRate,@LaborRate1,@LaborRate2,@LaborRate3,@LaborRate4,@Status,@Email,@ResponsibleOfficer,@Shift,@PINNumber,@PINRequired,@LockedOut,@LockedOutReason,@LockedOutDate,@LastActivityDate,@CardedIn,@ShortCardNumber,@CreatedDate,@CreatedBy,@UpdatedDate,@UpdatedBy,@OnFileSignature,@UserData1,@UserData2,@UserData3,@UserData4,@UserData5,@UserData6,@UserData7,@UserData8,@UserData9,@UserData10,@UserData11,@UserData12,@UserData13,@UserData14,@UserData15,@UserData16,@UserData17,@UserData18,@UserData19,@UserData20,@UserData21,@UserData22,@UserData23,@UserData24,@InhibitInactivityLockout,@PersonnelGuid,@SiteGuid,@CompanyGuid,@SupervisorPersonnelGuid,@UserGuid,@AssignedEquipmentGuid,@_MasterRecordGuid,@HiddenDate)
        ;
    
    SET @sync_row_count = @@rowcount;
    
    -- If we updated / inserted a record here, go ahead and remove any pending conflict records associated with this ID because this record should contain the most recent data.
    IF (@sync_row_count > 0)
    BEGIN
        SET NOCOUNT ON
        IF EXISTS (SELECT 1 FROM [sync].[tblSyncRecordConflict] WHERE RecordKey = CONVERT(nvarchar(512), @PersonnelGuid) AND SyncConflictResolutionStatusIndex IN (0,3))
        BEGIN
            DELETE FROM [sync].[tblSyncRecordConflictToSyncSessionScopeLog] WHERE SyncRecordConflictGuid IN (SELECT SyncRecordConflictGuid FROM [sync].[tblSyncRecordConflict] WHERE RecordKey = CONVERT(nvarchar(512), @PersonnelGuid))
            DELETE FROM [sync].[tblSyncRecordConflict] WHERE RecordKey = CONVERT(nvarchar(512), @PersonnelGuid)
        END
        SET NOCOUNT OFF
    END
    

    -- One issue with the MERGE approach is that we don't know if the @@rowcount is zero because of a true conflict now or if it was simply because
    -- the CreatedDate and UpdatedDate qualifiers were not met (in which case we want to simply throw away the update)
    -- In order to determine what took place, we're going to need to perform a query if @@rowcount = 0 to see if it was caused by the additional qualifiers.
    --
    IF (@sync_row_count = 0)
    BEGIN
        IF EXISTS (SELECT 1 FROM [dbo].[tblPersonnel] WHERE PersonnelGuid = @PersonnelGuid AND (CreatedDate >= @CreatedDate OR UpdatedDate >= @UpdatedDate))
            SET @sync_row_count = 1;    
    END

    SET @minValidVersion = 0;   -- This is used to detect Change Tracking cleanup
                                -- If we support this, we should add a column to SynchronizationTable
                                -- that records the MinValidVersion after change tracking information for
                                -- a table gets cleaned up.  I don't think this will be necessary.
                                        
    IF @minValidVersion > @sync_last_received_anchor
        RAISERROR(N'(CI)Time between synchronization has exceeded the maximum amount of time for table ''%s'' (MIN: %I64d, LAST: %I64d).  To avoid data corruption and old data, the client must reinitialize its local database with a current refresh from the server.', 16, 3, @sync_table_name, @minValidVersion, @sync_last_received_anchor)
END
