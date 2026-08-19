CREATE TABLE [dbo].[tblEquipmentQualityTagLog] (
    [QualityTagName]             NVARCHAR (50)      CONSTRAINT [DF_tblEquipmentQualityTagLog_QualityTagName] DEFAULT ('') NOT NULL,
    [EquipmentID]                NVARCHAR (50)      CONSTRAINT [DF_tblEquipmentQualityTagLog_EquipmentID] DEFAULT ('') NOT NULL,
    [EquipmentType]              NVARCHAR (50)      CONSTRAINT [DF_tblEquipmentQualityTagLog_EquipmentType] DEFAULT ('') NOT NULL,
    [TaggedDate]                 DATETIMEOFFSET (7) CONSTRAINT [DF_tblEquipmentQualityTagLog_TaggedDate] DEFAULT (sysdatetimeoffset()) NOT NULL,
    [TaggedBy]                   NVARCHAR (50)      CONSTRAINT [DF_tblEquipmentQualityTagLog_TaggedBy] DEFAULT ('') NOT NULL,
    [Memo]                       NVARCHAR (1000)    NULL,
    [RemovedDate]                DATETIMEOFFSET (7) NULL,
    [RemovedBy]                  NVARCHAR (255)     NULL,
    [DeleteFlag]                 BIT                CONSTRAINT [DF_tblEquipmentQualityTagLog_DeleteFlag] DEFAULT ((0)) NOT NULL,
    [CreatedDate]                DATETIMEOFFSET (7) CONSTRAINT [DF_tblEquipmentQualityTagLog_CreatedDate] DEFAULT (sysdatetimeoffset()) NOT NULL,
    [CreatedBy]                  [dbo].[udtUserID]  CONSTRAINT [DF_tblEquipmentQualityTagLog_CreatedBy] DEFAULT ('') NOT NULL,
    [UpdatedDate]                DATETIMEOFFSET (7) CONSTRAINT [DF_tblEquipmentQualityTagLog_UpdatedDate] DEFAULT (sysdatetimeoffset()) NOT NULL,
    [UpdatedBy]                  [dbo].[udtUserID]  CONSTRAINT [DF_tblEquipmentQualityTagLog_UpdatedBy] DEFAULT ('') NOT NULL,
    [TagNumber]                  INT                NULL,
    [EquipmentQualityTagLogGuid] UNIQUEIDENTIFIER   CONSTRAINT [DF_tblEquipmentQualityTagLog_GUID] DEFAULT (newid()) NOT NULL,
    [_RowVersion]                ROWVERSION         NOT NULL,
    [SiteGuid]                   UNIQUEIDENTIFIER   NOT NULL,
    [EquipmentGuid]              UNIQUEIDENTIFIER   NOT NULL,
    [QualityTagGuid]             UNIQUEIDENTIFIER   NULL,
    CONSTRAINT [PK_tblEquipmentQualityTagLog_GUID] PRIMARY KEY NONCLUSTERED ([EquipmentQualityTagLogGuid] ASC)
);


GO
CREATE CLUSTERED INDEX [IX_tblEquipmentQualityTagLog_CreatedDate]
    ON [dbo].[tblEquipmentQualityTagLog]([CreatedDate] ASC);

GO
