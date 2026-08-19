/*
	DROP TABLE [erv].[tblTempPersonnelRecordVersioningFlag]
*/
CREATE TABLE [erv].[tblTempPersonnelRecordVersioningFlag] (
    [PersonnelRVFlagGuid]            UNIQUEIDENTIFIER   CONSTRAINT [DF_tblTempPersonnelRecordVersioningFlag_GUID] DEFAULT (newid()) NOT NULL,
    [PersonnelGuid]                  UNIQUEIDENTIFIER   NULL,
    [SiteGuid]                       UNIQUEIDENTIFIER   NULL,
    [Address1_RVFlag]                BIT                NULL,
    [Address2_RVFlag]                BIT                NULL,
    [AssignedEquipmentGuid_RVFlag]   BIT                NULL,
    [AssignmentDate_RVFlag]          BIT                NULL,
    [BirthDate_RVFlag]               BIT                NULL,
    [CardedIn_RVFlag]                BIT                NULL,
    [CardNumber_RVFlag]              BIT                NULL,
    [City_RVFlag]                    BIT                NULL,
    [CompanyGuid_RVFlag]             BIT                NULL,
    [Country_RVFlag]                 BIT                NULL,
    [Department_RVFlag]              BIT                NULL,
    [Email_RVFlag]                   BIT                NULL,
    [FirstName_RVFlag]               BIT                NULL,
	[HiddenDate_RVFlag]              BIT                NULL,
	[InhibitInactivityLockout_RVFlag] BIT                NULL,
    [LaborRate1_RVFlag]              BIT                NULL,
    [LaborRate2_RVFlag]              BIT                NULL,
    [LaborRate3_RVFlag]              BIT                NULL,
    [LaborRate4_RVFlag]              BIT                NULL,
    [LastActivityDate_RVFlag]        BIT                NULL,
    [LastName_RVFlag]                BIT                NULL,
    [LockedOut_RVFlag]               BIT                NULL,
    [LockedOutDate_RVFlag]           BIT                NULL,
    [LockedOutReason_RVFlag]         BIT                NULL,
    [MiddleName_RVFlag]              BIT                NULL,
    [OnFileSignature_RVFlag]         BIT                NULL,
    [PayRate_RVFlag]                 BIT                NULL,
    [PersonID_RVFlag]                BIT                NULL,
    [Phone1_RVFlag]                  BIT                NULL,
    [Phone2_RVFlag]                  BIT                NULL,
    [PINNumber_RVFlag]               BIT                NULL,
    [PINRequired_RVFlag]             BIT                NULL,
    [ResponsibleOfficer_RVFlag]      BIT                NULL,
    [Shift_RVFlag]                   BIT                NULL,
    [ShortCardNumber_RVFlag]         BIT                NULL,
    [SSAN_RVFlag]                    BIT                NULL,
    [State_RVFlag]                   BIT                NULL,
    [Status_RVFlag]                  BIT                NULL,
    [SupervisionDate_RVFlag]         BIT                NULL,
    [SupervisorPersonnelGuid_RVFlag] BIT                NULL,
    [Title_RVFlag]                   BIT                NULL,
    [UpdatedBy_RVFlag]               BIT                NULL,
    [UpdatedDate_RVFlag]             BIT                NULL,
    [UserData1_RVFlag]               BIT                NULL,
    [UserData10_RVFlag]              BIT                NULL,
    [UserData11_RVFlag]              BIT                NULL,
    [UserData12_RVFlag]              BIT                NULL,
    [UserData13_RVFlag]              BIT                NULL,
    [UserData14_RVFlag]              BIT                NULL,
    [UserData15_RVFlag]              BIT                NULL,
    [UserData16_RVFlag]              BIT                NULL,
    [UserData17_RVFlag]              BIT                NULL,
    [UserData18_RVFlag]              BIT                NULL,
    [UserData19_RVFlag]              BIT                NULL,
    [UserData2_RVFlag]               BIT                NULL,
    [UserData20_RVFlag]              BIT                NULL,
    [UserData21_RVFlag]              BIT                NULL,
    [UserData22_RVFlag]              BIT                NULL,
    [UserData23_RVFlag]              BIT                NULL,
    [UserData24_RVFlag]              BIT                NULL,
    [UserData3_RVFlag]               BIT                NULL,
    [UserData4_RVFlag]               BIT                NULL,
    [UserData5_RVFlag]               BIT                NULL,
    [UserData6_RVFlag]               BIT                NULL,
    [UserData7_RVFlag]               BIT                NULL,
    [UserData8_RVFlag]               BIT                NULL,
    [UserData9_RVFlag]               BIT                NULL,
    [UserGuid_RVFlag]                BIT                NULL,
    [Zip_RVFlag]                     BIT                NULL,
    [_CallingReferenceGuid]          UNIQUEIDENTIFIER   NOT NULL,
    [CreatedDate]                    DATETIMEOFFSET (7) CONSTRAINT [DF_tblTempPersonnelRecordVersioningFlag_CreatedDate] DEFAULT (sysdatetimeoffset()) NOT NULL,
    [CreatedBy]                      [dbo].[udtUserID]  CONSTRAINT [DF_tblTempPersonnelRecordVersioningFlag_CreatedBy] DEFAULT ('') NOT NULL,
    [UpdatedDate]                    DATETIMEOFFSET (7) CONSTRAINT [DF_tblTempPersonnelRecordVersioningFlag_UpdatedDate] DEFAULT (sysdatetimeoffset()) NOT NULL,
    [UpdatedBy]                      [dbo].[udtUserID]  CONSTRAINT [DF_tblTempPersonnelRecordVersioningFlag_UpdatedBy] DEFAULT ('') NOT NULL,
    [_RowVersion]                    ROWVERSION         NOT NULL,    
    [_ClusterIdx]                    BIGINT             IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK_tblTempPersonnelRecordVersioningFlag] PRIMARY KEY NONCLUSTERED ([PersonnelRVFlagGuid] ASC)
);


GO
CREATE NONCLUSTERED INDEX [IX_tblTempPersonnelRecordVersioningFlag_PersonnelGuid]
    ON [erv].[tblTempPersonnelRecordVersioningFlag]([PersonnelGuid] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_tblTempPersonnelRecordVersioningFlag_CallingReferenceGuid]
    ON [erv].[tblTempPersonnelRecordVersioningFlag]([_CallingReferenceGuid] ASC);


GO
CREATE UNIQUE CLUSTERED INDEX [IX_tblTempPersonnelRecordVersioningFlag_ClusterIdx]
    ON [erv].[tblTempPersonnelRecordVersioningFlag]([_ClusterIdx] ASC);

GO
