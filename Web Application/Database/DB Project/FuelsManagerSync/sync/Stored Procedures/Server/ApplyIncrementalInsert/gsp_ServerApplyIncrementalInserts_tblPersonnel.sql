-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblPersonnel
-- Description: Apply Inserts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ServerApplyIncrementalInserts_tblPersonnel]
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
@sync_table_name nvarchar(512),
@sync_supported_columns_tblPersonnel varchar(8000)
AS
BEGIN
    DECLARE @minValidVersion BigInt 

    ;   MERGE [dbo].[tblPersonnel] AS existingData
        USING (SELECT @PersonID 'PersonID',@CardNumber 'CardNumber',@FirstName 'FirstName',@MiddleName 'MiddleName',@LastName 'LastName',@Title 'Title',@Department 'Department',@Address1 'Address1',@Address2 'Address2',@City 'City',@State 'State',@Zip 'Zip',@Country 'Country',@Phone1 'Phone1',@Phone2 'Phone2',@AssignmentDate 'AssignmentDate',@SupervisionDate 'SupervisionDate',@SSAN 'SSAN',@BirthDate 'BirthDate',@PayRate 'PayRate',@LaborRate1 'LaborRate1',@LaborRate2 'LaborRate2',@LaborRate3 'LaborRate3',@LaborRate4 'LaborRate4',@Status 'Status',@Email 'Email',@ResponsibleOfficer 'ResponsibleOfficer',@Shift 'Shift',@PINNumber 'PINNumber',@PINRequired 'PINRequired',@LockedOut 'LockedOut',@LockedOutReason 'LockedOutReason',@LockedOutDate 'LockedOutDate',@LastActivityDate 'LastActivityDate',@CardedIn 'CardedIn',@ShortCardNumber 'ShortCardNumber',@CreatedDate 'CreatedDate',@CreatedBy 'CreatedBy',@UpdatedDate 'UpdatedDate',@UpdatedBy 'UpdatedBy',@OnFileSignature 'OnFileSignature',@UserData1 'UserData1',@UserData2 'UserData2',@UserData3 'UserData3',@UserData4 'UserData4',@UserData5 'UserData5',@UserData6 'UserData6',@UserData7 'UserData7',@UserData8 'UserData8',@UserData9 'UserData9',@UserData10 'UserData10',@UserData11 'UserData11',@UserData12 'UserData12',@UserData13 'UserData13',@UserData14 'UserData14',@UserData15 'UserData15',@UserData16 'UserData16',@UserData17 'UserData17',@UserData18 'UserData18',@UserData19 'UserData19',@UserData20 'UserData20',@UserData21 'UserData21',@UserData22 'UserData22',@UserData23 'UserData23',@UserData24 'UserData24',@InhibitInactivityLockout 'InhibitInactivityLockout',@PersonnelGuid 'PersonnelGuid',@SiteGuid 'SiteGuid',@CompanyGuid 'CompanyGuid',@SupervisorPersonnelGuid 'SupervisorPersonnelGuid',@UserGuid 'UserGuid',@AssignedEquipmentGuid 'AssignedEquipmentGuid',@_MasterRecordGuid '_MasterRecordGuid',@HiddenDate 'HiddenDate'
                ) AS remoteChanges ([PersonID],[CardNumber],[FirstName],[MiddleName],[LastName],[Title],[Department],[Address1],[Address2],[City],[State],[Zip],[Country],[Phone1],[Phone2],[AssignmentDate],[SupervisionDate],[SSAN],[BirthDate],[PayRate],[LaborRate1],[LaborRate2],[LaborRate3],[LaborRate4],[Status],[Email],[ResponsibleOfficer],[Shift],[PINNumber],[PINRequired],[LockedOut],[LockedOutReason],[LockedOutDate],[LastActivityDate],[CardedIn],[ShortCardNumber],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy],[OnFileSignature],[UserData1],[UserData2],[UserData3],[UserData4],[UserData5],[UserData6],[UserData7],[UserData8],[UserData9],[UserData10],[UserData11],[UserData12],[UserData13],[UserData14],[UserData15],[UserData16],[UserData17],[UserData18],[UserData19],[UserData20],[UserData21],[UserData22],[UserData23],[UserData24],[InhibitInactivityLockout],[PersonnelGuid],[SiteGuid],[CompanyGuid],[SupervisorPersonnelGuid],[UserGuid],[AssignedEquipmentGuid],[_MasterRecordGuid],[HiddenDate])
        ON (existingData.[PersonnelGuid] = remoteChanges.[PersonnelGuid])
        WHEN MATCHED AND (remoteChanges.CreatedDate > existingData.CreatedDate OR remoteChanges.UpdatedDate > existingData.UpdatedDate) THEN
            UPDATE SET [PersonID] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('PersonID'), @sync_supported_columns_tblPersonnel)) WHEN 0 THEN existingData.[PersonID] ELSE remoteChanges.[PersonID] END
                       ,[CardNumber] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('CardNumber'), @sync_supported_columns_tblPersonnel)) WHEN 0 THEN existingData.[CardNumber] ELSE remoteChanges.[CardNumber] END
                       ,[FirstName] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('FirstName'), @sync_supported_columns_tblPersonnel)) WHEN 0 THEN existingData.[FirstName] ELSE remoteChanges.[FirstName] END
                       ,[MiddleName] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('MiddleName'), @sync_supported_columns_tblPersonnel)) WHEN 0 THEN existingData.[MiddleName] ELSE remoteChanges.[MiddleName] END
                       ,[LastName] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('LastName'), @sync_supported_columns_tblPersonnel)) WHEN 0 THEN existingData.[LastName] ELSE remoteChanges.[LastName] END
                       ,[Title] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Title'), @sync_supported_columns_tblPersonnel)) WHEN 0 THEN existingData.[Title] ELSE remoteChanges.[Title] END
                       ,[Department] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Department'), @sync_supported_columns_tblPersonnel)) WHEN 0 THEN existingData.[Department] ELSE remoteChanges.[Department] END
                       ,[Address1] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Address1'), @sync_supported_columns_tblPersonnel)) WHEN 0 THEN existingData.[Address1] ELSE remoteChanges.[Address1] END
                       ,[Address2] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Address2'), @sync_supported_columns_tblPersonnel)) WHEN 0 THEN existingData.[Address2] ELSE remoteChanges.[Address2] END
                       ,[City] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('City'), @sync_supported_columns_tblPersonnel)) WHEN 0 THEN existingData.[City] ELSE remoteChanges.[City] END
                       ,[State] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('State'), @sync_supported_columns_tblPersonnel)) WHEN 0 THEN existingData.[State] ELSE remoteChanges.[State] END
                       ,[Zip] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Zip'), @sync_supported_columns_tblPersonnel)) WHEN 0 THEN existingData.[Zip] ELSE remoteChanges.[Zip] END
                       ,[Country] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Country'), @sync_supported_columns_tblPersonnel)) WHEN 0 THEN existingData.[Country] ELSE remoteChanges.[Country] END
                       ,[Phone1] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Phone1'), @sync_supported_columns_tblPersonnel)) WHEN 0 THEN existingData.[Phone1] ELSE remoteChanges.[Phone1] END
                       ,[Phone2] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Phone2'), @sync_supported_columns_tblPersonnel)) WHEN 0 THEN existingData.[Phone2] ELSE remoteChanges.[Phone2] END
                       ,[AssignmentDate] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('AssignmentDate'), @sync_supported_columns_tblPersonnel)) WHEN 0 THEN existingData.[AssignmentDate] ELSE remoteChanges.[AssignmentDate] END
                       ,[SupervisionDate] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('SupervisionDate'), @sync_supported_columns_tblPersonnel)) WHEN 0 THEN existingData.[SupervisionDate] ELSE remoteChanges.[SupervisionDate] END
                       ,[SSAN] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('SSAN'), @sync_supported_columns_tblPersonnel)) WHEN 0 THEN existingData.[SSAN] ELSE remoteChanges.[SSAN] END
                       ,[BirthDate] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('BirthDate'), @sync_supported_columns_tblPersonnel)) WHEN 0 THEN existingData.[BirthDate] ELSE remoteChanges.[BirthDate] END
                       ,[PayRate] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('PayRate'), @sync_supported_columns_tblPersonnel)) WHEN 0 THEN existingData.[PayRate] ELSE remoteChanges.[PayRate] END
                       ,[LaborRate1] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('LaborRate1'), @sync_supported_columns_tblPersonnel)) WHEN 0 THEN existingData.[LaborRate1] ELSE remoteChanges.[LaborRate1] END
                       ,[LaborRate2] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('LaborRate2'), @sync_supported_columns_tblPersonnel)) WHEN 0 THEN existingData.[LaborRate2] ELSE remoteChanges.[LaborRate2] END
                       ,[LaborRate3] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('LaborRate3'), @sync_supported_columns_tblPersonnel)) WHEN 0 THEN existingData.[LaborRate3] ELSE remoteChanges.[LaborRate3] END
                       ,[LaborRate4] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('LaborRate4'), @sync_supported_columns_tblPersonnel)) WHEN 0 THEN existingData.[LaborRate4] ELSE remoteChanges.[LaborRate4] END
                       ,[Status] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Status'), @sync_supported_columns_tblPersonnel)) WHEN 0 THEN existingData.[Status] ELSE remoteChanges.[Status] END
                       ,[Email] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Email'), @sync_supported_columns_tblPersonnel)) WHEN 0 THEN existingData.[Email] ELSE remoteChanges.[Email] END
                       ,[ResponsibleOfficer] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('ResponsibleOfficer'), @sync_supported_columns_tblPersonnel)) WHEN 0 THEN existingData.[ResponsibleOfficer] ELSE remoteChanges.[ResponsibleOfficer] END
                       ,[Shift] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Shift'), @sync_supported_columns_tblPersonnel)) WHEN 0 THEN existingData.[Shift] ELSE remoteChanges.[Shift] END
                       ,[PINNumber] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('PINNumber'), @sync_supported_columns_tblPersonnel)) WHEN 0 THEN existingData.[PINNumber] ELSE remoteChanges.[PINNumber] END
                       ,[PINRequired] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('PINRequired'), @sync_supported_columns_tblPersonnel)) WHEN 0 THEN existingData.[PINRequired] ELSE remoteChanges.[PINRequired] END
                       ,[LockedOut] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('LockedOut'), @sync_supported_columns_tblPersonnel)) WHEN 0 THEN existingData.[LockedOut] ELSE remoteChanges.[LockedOut] END
                       ,[LockedOutReason] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('LockedOutReason'), @sync_supported_columns_tblPersonnel)) WHEN 0 THEN existingData.[LockedOutReason] ELSE remoteChanges.[LockedOutReason] END
                       ,[LockedOutDate] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('LockedOutDate'), @sync_supported_columns_tblPersonnel)) WHEN 0 THEN existingData.[LockedOutDate] ELSE remoteChanges.[LockedOutDate] END
                       ,[LastActivityDate] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('LastActivityDate'), @sync_supported_columns_tblPersonnel)) WHEN 0 THEN existingData.[LastActivityDate] ELSE remoteChanges.[LastActivityDate] END
                       ,[CardedIn] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('CardedIn'), @sync_supported_columns_tblPersonnel)) WHEN 0 THEN existingData.[CardedIn] ELSE remoteChanges.[CardedIn] END
                       ,[ShortCardNumber] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('ShortCardNumber'), @sync_supported_columns_tblPersonnel)) WHEN 0 THEN existingData.[ShortCardNumber] ELSE remoteChanges.[ShortCardNumber] END
                       ,[CreatedDate] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('CreatedDate'), @sync_supported_columns_tblPersonnel)) WHEN 0 THEN existingData.[CreatedDate] ELSE remoteChanges.[CreatedDate] END
                       ,[CreatedBy] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('CreatedBy'), @sync_supported_columns_tblPersonnel)) WHEN 0 THEN existingData.[CreatedBy] ELSE remoteChanges.[CreatedBy] END
                       ,[UpdatedDate] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UpdatedDate'), @sync_supported_columns_tblPersonnel)) WHEN 0 THEN existingData.[UpdatedDate] ELSE remoteChanges.[UpdatedDate] END
                       ,[UpdatedBy] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UpdatedBy'), @sync_supported_columns_tblPersonnel)) WHEN 0 THEN existingData.[UpdatedBy] ELSE remoteChanges.[UpdatedBy] END
                       ,[OnFileSignature] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('OnFileSignature'), @sync_supported_columns_tblPersonnel)) WHEN 0 THEN existingData.[OnFileSignature] ELSE remoteChanges.[OnFileSignature] END
                       ,[UserData1] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UserData1'), @sync_supported_columns_tblPersonnel)) WHEN 0 THEN existingData.[UserData1] ELSE remoteChanges.[UserData1] END
                       ,[UserData2] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UserData2'), @sync_supported_columns_tblPersonnel)) WHEN 0 THEN existingData.[UserData2] ELSE remoteChanges.[UserData2] END
                       ,[UserData3] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UserData3'), @sync_supported_columns_tblPersonnel)) WHEN 0 THEN existingData.[UserData3] ELSE remoteChanges.[UserData3] END
                       ,[UserData4] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UserData4'), @sync_supported_columns_tblPersonnel)) WHEN 0 THEN existingData.[UserData4] ELSE remoteChanges.[UserData4] END
                       ,[UserData5] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UserData5'), @sync_supported_columns_tblPersonnel)) WHEN 0 THEN existingData.[UserData5] ELSE remoteChanges.[UserData5] END
                       ,[UserData6] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UserData6'), @sync_supported_columns_tblPersonnel)) WHEN 0 THEN existingData.[UserData6] ELSE remoteChanges.[UserData6] END
                       ,[UserData7] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UserData7'), @sync_supported_columns_tblPersonnel)) WHEN 0 THEN existingData.[UserData7] ELSE remoteChanges.[UserData7] END
                       ,[UserData8] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UserData8'), @sync_supported_columns_tblPersonnel)) WHEN 0 THEN existingData.[UserData8] ELSE remoteChanges.[UserData8] END
                       ,[UserData9] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UserData9'), @sync_supported_columns_tblPersonnel)) WHEN 0 THEN existingData.[UserData9] ELSE remoteChanges.[UserData9] END
                       ,[UserData10] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UserData10'), @sync_supported_columns_tblPersonnel)) WHEN 0 THEN existingData.[UserData10] ELSE remoteChanges.[UserData10] END
                       ,[UserData11] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UserData11'), @sync_supported_columns_tblPersonnel)) WHEN 0 THEN existingData.[UserData11] ELSE remoteChanges.[UserData11] END
                       ,[UserData12] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UserData12'), @sync_supported_columns_tblPersonnel)) WHEN 0 THEN existingData.[UserData12] ELSE remoteChanges.[UserData12] END
                       ,[UserData13] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UserData13'), @sync_supported_columns_tblPersonnel)) WHEN 0 THEN existingData.[UserData13] ELSE remoteChanges.[UserData13] END
                       ,[UserData14] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UserData14'), @sync_supported_columns_tblPersonnel)) WHEN 0 THEN existingData.[UserData14] ELSE remoteChanges.[UserData14] END
                       ,[UserData15] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UserData15'), @sync_supported_columns_tblPersonnel)) WHEN 0 THEN existingData.[UserData15] ELSE remoteChanges.[UserData15] END
                       ,[UserData16] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UserData16'), @sync_supported_columns_tblPersonnel)) WHEN 0 THEN existingData.[UserData16] ELSE remoteChanges.[UserData16] END
                       ,[UserData17] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UserData17'), @sync_supported_columns_tblPersonnel)) WHEN 0 THEN existingData.[UserData17] ELSE remoteChanges.[UserData17] END
                       ,[UserData18] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UserData18'), @sync_supported_columns_tblPersonnel)) WHEN 0 THEN existingData.[UserData18] ELSE remoteChanges.[UserData18] END
                       ,[UserData19] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UserData19'), @sync_supported_columns_tblPersonnel)) WHEN 0 THEN existingData.[UserData19] ELSE remoteChanges.[UserData19] END
                       ,[UserData20] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UserData20'), @sync_supported_columns_tblPersonnel)) WHEN 0 THEN existingData.[UserData20] ELSE remoteChanges.[UserData20] END
                       ,[UserData21] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UserData21'), @sync_supported_columns_tblPersonnel)) WHEN 0 THEN existingData.[UserData21] ELSE remoteChanges.[UserData21] END
                       ,[UserData22] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UserData22'), @sync_supported_columns_tblPersonnel)) WHEN 0 THEN existingData.[UserData22] ELSE remoteChanges.[UserData22] END
                       ,[UserData23] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UserData23'), @sync_supported_columns_tblPersonnel)) WHEN 0 THEN existingData.[UserData23] ELSE remoteChanges.[UserData23] END
                       ,[UserData24] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UserData24'), @sync_supported_columns_tblPersonnel)) WHEN 0 THEN existingData.[UserData24] ELSE remoteChanges.[UserData24] END
                       ,[InhibitInactivityLockout] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('InhibitInactivityLockout'), @sync_supported_columns_tblPersonnel)) WHEN 0 THEN existingData.[InhibitInactivityLockout] ELSE remoteChanges.[InhibitInactivityLockout] END
                       ,[SiteGuid] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('SiteGuid'), @sync_supported_columns_tblPersonnel)) WHEN 0 THEN existingData.[SiteGuid] ELSE remoteChanges.[SiteGuid] END
                       ,[CompanyGuid] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('CompanyGuid'), @sync_supported_columns_tblPersonnel)) WHEN 0 THEN existingData.[CompanyGuid] ELSE remoteChanges.[CompanyGuid] END
                       ,[SupervisorPersonnelGuid] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('SupervisorPersonnelGuid'), @sync_supported_columns_tblPersonnel)) WHEN 0 THEN existingData.[SupervisorPersonnelGuid] ELSE remoteChanges.[SupervisorPersonnelGuid] END
                       ,[UserGuid] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UserGuid'), @sync_supported_columns_tblPersonnel)) WHEN 0 THEN existingData.[UserGuid] ELSE remoteChanges.[UserGuid] END
                       ,[AssignedEquipmentGuid] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('AssignedEquipmentGuid'), @sync_supported_columns_tblPersonnel)) WHEN 0 THEN existingData.[AssignedEquipmentGuid] ELSE remoteChanges.[AssignedEquipmentGuid] END
                       ,[_MasterRecordGuid] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('_MasterRecordGuid'), @sync_supported_columns_tblPersonnel)) WHEN 0 THEN existingData.[_MasterRecordGuid] ELSE remoteChanges.[_MasterRecordGuid] END
                       ,[HiddenDate] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('HiddenDate'), @sync_supported_columns_tblPersonnel)) WHEN 0 THEN existingData.[HiddenDate] ELSE remoteChanges.[HiddenDate] END

        WHEN NOT MATCHED THEN
            INSERT ([PersonID],[CardNumber],[FirstName],[MiddleName],[LastName],[Title],[Department],[Address1],[Address2],[City],[State],[Zip],[Country],[Phone1],[Phone2],[AssignmentDate],[SupervisionDate],[SSAN],[BirthDate],[PayRate],[LaborRate1],[LaborRate2],[LaborRate3],[LaborRate4],[Status],[Email],[ResponsibleOfficer],[Shift],[PINNumber],[PINRequired],[LockedOut],[LockedOutReason],[LockedOutDate],[LastActivityDate],[CardedIn],[ShortCardNumber],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy],[OnFileSignature],[UserData1],[UserData2],[UserData3],[UserData4],[UserData5],[UserData6],[UserData7],[UserData8],[UserData9],[UserData10],[UserData11],[UserData12],[UserData13],[UserData14],[UserData15],[UserData16],[UserData17],[UserData18],[UserData19],[UserData20],[UserData21],[UserData22],[UserData23],[UserData24],[InhibitInactivityLockout],[PersonnelGuid],[SiteGuid],[CompanyGuid],[SupervisorPersonnelGuid],[UserGuid],[AssignedEquipmentGuid],[_MasterRecordGuid],[HiddenDate])
                VALUES (@PersonID,(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('CardNumber'), @sync_supported_columns_tblPersonnel)) WHEN 0 THEN NULL ELSE @CardNumber END),@FirstName,@MiddleName,@LastName,@Title,@Department,@Address1,@Address2,@City,@State,@Zip,@Country,@Phone1,@Phone2,@AssignmentDate,@SupervisionDate,@SSAN,@BirthDate,@PayRate,@LaborRate1,@LaborRate2,@LaborRate3,@LaborRate4,@Status,@Email,@ResponsibleOfficer,@Shift,(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('PINNumber'), @sync_supported_columns_tblPersonnel)) WHEN 0 THEN NULL ELSE @PINNumber END),@PINRequired,@LockedOut,@LockedOutReason,@LockedOutDate,@LastActivityDate,@CardedIn,(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('ShortCardNumber'), @sync_supported_columns_tblPersonnel)) WHEN 0 THEN NULL ELSE @ShortCardNumber END),@CreatedDate,@CreatedBy,@UpdatedDate,@UpdatedBy,(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('OnFileSignature'), @sync_supported_columns_tblPersonnel)) WHEN 0 THEN NULL ELSE @OnFileSignature END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UserData1'), @sync_supported_columns_tblPersonnel)) WHEN 0 THEN NULL ELSE @UserData1 END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UserData2'), @sync_supported_columns_tblPersonnel)) WHEN 0 THEN NULL ELSE @UserData2 END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UserData3'), @sync_supported_columns_tblPersonnel)) WHEN 0 THEN NULL ELSE @UserData3 END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UserData4'), @sync_supported_columns_tblPersonnel)) WHEN 0 THEN NULL ELSE @UserData4 END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UserData5'), @sync_supported_columns_tblPersonnel)) WHEN 0 THEN NULL ELSE @UserData5 END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UserData6'), @sync_supported_columns_tblPersonnel)) WHEN 0 THEN NULL ELSE @UserData6 END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UserData7'), @sync_supported_columns_tblPersonnel)) WHEN 0 THEN NULL ELSE @UserData7 END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UserData8'), @sync_supported_columns_tblPersonnel)) WHEN 0 THEN NULL ELSE @UserData8 END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UserData9'), @sync_supported_columns_tblPersonnel)) WHEN 0 THEN NULL ELSE @UserData9 END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UserData10'), @sync_supported_columns_tblPersonnel)) WHEN 0 THEN NULL ELSE @UserData10 END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UserData11'), @sync_supported_columns_tblPersonnel)) WHEN 0 THEN NULL ELSE @UserData11 END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UserData12'), @sync_supported_columns_tblPersonnel)) WHEN 0 THEN NULL ELSE @UserData12 END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UserData13'), @sync_supported_columns_tblPersonnel)) WHEN 0 THEN NULL ELSE @UserData13 END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UserData14'), @sync_supported_columns_tblPersonnel)) WHEN 0 THEN NULL ELSE @UserData14 END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UserData15'), @sync_supported_columns_tblPersonnel)) WHEN 0 THEN NULL ELSE @UserData15 END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UserData16'), @sync_supported_columns_tblPersonnel)) WHEN 0 THEN NULL ELSE @UserData16 END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UserData17'), @sync_supported_columns_tblPersonnel)) WHEN 0 THEN NULL ELSE @UserData17 END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UserData18'), @sync_supported_columns_tblPersonnel)) WHEN 0 THEN NULL ELSE @UserData18 END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UserData19'), @sync_supported_columns_tblPersonnel)) WHEN 0 THEN NULL ELSE @UserData19 END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UserData20'), @sync_supported_columns_tblPersonnel)) WHEN 0 THEN NULL ELSE @UserData20 END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UserData21'), @sync_supported_columns_tblPersonnel)) WHEN 0 THEN NULL ELSE @UserData21 END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UserData22'), @sync_supported_columns_tblPersonnel)) WHEN 0 THEN NULL ELSE @UserData22 END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UserData23'), @sync_supported_columns_tblPersonnel)) WHEN 0 THEN NULL ELSE @UserData23 END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UserData24'), @sync_supported_columns_tblPersonnel)) WHEN 0 THEN NULL ELSE @UserData24 END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('InhibitInactivityLockout'), @sync_supported_columns_tblPersonnel)) WHEN 0 THEN NULL ELSE @InhibitInactivityLockout END),@PersonnelGuid,@SiteGuid,(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('CompanyGuid'), @sync_supported_columns_tblPersonnel)) WHEN 0 THEN NULL ELSE @CompanyGuid END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('SupervisorPersonnelGuid'), @sync_supported_columns_tblPersonnel)) WHEN 0 THEN NULL ELSE @SupervisorPersonnelGuid END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UserGuid'), @sync_supported_columns_tblPersonnel)) WHEN 0 THEN NULL ELSE @UserGuid END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('AssignedEquipmentGuid'), @sync_supported_columns_tblPersonnel)) WHEN 0 THEN NULL ELSE @AssignedEquipmentGuid END),@_MasterRecordGuid,(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('HiddenDate'), @sync_supported_columns_tblPersonnel)) WHEN 0 THEN NULL ELSE @HiddenDate END))
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
        RAISERROR(N'(SI)Time between synchronization has exceeded the maximum amount of time for table ''%s'' (MIN: %I64d, LAST: %I64d).  To avoid data corruption and old data, the client must reinitialize its local database with a current refresh from the server.', 16, 3, @sync_table_name, @minValidVersion, @sync_last_received_anchor)
END

