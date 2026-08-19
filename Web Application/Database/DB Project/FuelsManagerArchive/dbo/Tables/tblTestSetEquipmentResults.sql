CREATE TABLE [dbo].[tblTestSetEquipmentResults] (
    [ResultTimeStamp]            DATETIMEOFFSET (7) CONSTRAINT [DF_tblTestSetEquipmentResults_ResultTimeStamp] DEFAULT (sysdatetimeoffset()) NOT NULL,
    [TestSetName]                NVARCHAR (80)      CONSTRAINT [DF_tblTestSetEquipmentResults_TestSetName] DEFAULT ('') NOT NULL,
    [Inspector]                  NVARCHAR (100)     CONSTRAINT [DF_tblTestSetEquipmentResults_Inspector] DEFAULT ('') NOT NULL,
    [Supervisor]                 NVARCHAR (100)     CONSTRAINT [DF_tblTestSetEquipmentResults_Supervisor] DEFAULT ('') NOT NULL,
    [EquipmentID]                NVARCHAR (50)      NOT NULL,
    [SampleNumber]               INT                CONSTRAINT [DF_tblTestSetEquipmentResults_SampleNumber] DEFAULT ((0)) NULL,
    [SampleSize]                 FLOAT (53)         CONSTRAINT [DF_tblTestSetEquipmentResults_SampleSize] DEFAULT ((0.0)) NOT NULL,
    [IsRetest]                   BIT                CONSTRAINT [DF_tblTestSetEquipmentResults_IsRetest] DEFAULT ((0)) NOT NULL,
    [PreviousSampleNumber]       INT                NULL,
    [DocumentNumber]             NVARCHAR (50)      NULL,
    [Memo]                       NVARCHAR (1000)    NULL,
    [GallonsRepresented]         FLOAT (53)         NULL,
    [Override]                   BIT                CONSTRAINT [DF_tblTestSetEquipmentResults_Override] DEFAULT ((0)) NOT NULL,
    [DeleteFlag]                 BIT                CONSTRAINT [DF_tblTestSetEquipmentResults_DeleteFlag] DEFAULT ((0)) NOT NULL,
    [CreatedDate]                DATETIMEOFFSET (7) CONSTRAINT [DF_tblTestSetEquipmentResults_CreatedDate] DEFAULT (sysdatetimeoffset()) NOT NULL,
    [CreatedBy]                  [dbo].[udtUserID]  CONSTRAINT [DF_tblTestSetEquipmentResults_CreatedBy] DEFAULT ('') NOT NULL,
    [UpdatedDate]                DATETIMEOFFSET (7) CONSTRAINT [DF_tblTestSetEquipmentResults_UpdatedDate] DEFAULT (sysdatetimeoffset()) NOT NULL,
    [UpdatedBy]                  [dbo].[udtUserID]  CONSTRAINT [DF_tblTestSetEquipmentResults_UpdatedBy] DEFAULT ('') NOT NULL,
    [TestSetEquipmentResultGuid] UNIQUEIDENTIFIER   CONSTRAINT [DF_tblTestSetEquipmentResults_GUID] DEFAULT (newid()) NOT NULL,
    [_RowVersion]                ROWVERSION         NOT NULL,
    [SiteGuid]                   UNIQUEIDENTIFIER   NOT NULL,
    [LookupTestSetStatusIndex]   INT                CONSTRAINT [DF_tblTestSetEquipmentResults_LookupTestSetStatusIndex] DEFAULT ((0)) NOT NULL,
    [EquipmentGuid]              UNIQUEIDENTIFIER   NOT NULL,
    CONSTRAINT [PK_tblTestSetEquipmentResults_GUID] PRIMARY KEY NONCLUSTERED ([TestSetEquipmentResultGuid] ASC)
);


GO
CREATE CLUSTERED INDEX [IX_tblTestSetEquipmentResults_CreatedDate]
    ON [dbo].[tblTestSetEquipmentResults]([CreatedDate] ASC);

GO
