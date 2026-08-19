CREATE TABLE [dbo].[tblTankQualityTagLog] (
    [TankID]                NVARCHAR (50)      CONSTRAINT [DF_tblTankQualityTagLog_TankID] DEFAULT ('') NOT NULL,
    [VesselType]            NVARCHAR (50)      CONSTRAINT [DF_tblTankQualityTagLog_VesselType] DEFAULT ('') NOT NULL,
    [QualityTagName]        NVARCHAR (50)      CONSTRAINT [DF_tblTankQualityTagLog_QualityTagName] DEFAULT ('') NOT NULL,
    [TaggedDate]            DATETIMEOFFSET (7) CONSTRAINT [DF_tblTankQualityTagLog_TaggedDate] DEFAULT (sysdatetimeoffset()) NOT NULL,
    [TaggedBy]              NVARCHAR (50)      CONSTRAINT [DF_tblTankQualityTagLog_TaggedBy] DEFAULT ('') NOT NULL,
    [Memo]                  NVARCHAR (1000)    NULL,
    [RemovedDate]           DATETIMEOFFSET (7) NULL,
    [RemovedBy]             NVARCHAR (255)     NULL,
    [DeleteFlag]            BIT                CONSTRAINT [DF_tblTankQualityTagLog_DeleteFlag] DEFAULT ((0)) NOT NULL,
    [CreatedDate]           DATETIMEOFFSET (7) CONSTRAINT [DF_tblTankQualityTagLog_CreatedDate] DEFAULT (sysdatetimeoffset()) NOT NULL,
    [CreatedBy]             [dbo].[udtUserID]  CONSTRAINT [DF_tblTankQualityTagLog_CreatedBy] DEFAULT ('') NOT NULL,
    [UpdatedDate]           DATETIMEOFFSET (7) CONSTRAINT [DF_tblTankQualityTagLog_UpdatedDate] DEFAULT (sysdatetimeoffset()) NOT NULL,
    [UpdatedBy]             [dbo].[udtUserID]  CONSTRAINT [DF_tblTankQualityTagLog_UpdatedBy] DEFAULT ('') NOT NULL,
    [TagNumber]             INT                NULL,
    [TankQualityTagLogGuid] UNIQUEIDENTIFIER   CONSTRAINT [DF_tblTankQualityTagLog_GUID] DEFAULT (newid()) NOT NULL,
    [_RowVersion]           ROWVERSION         NOT NULL,
    [SiteGuid]              UNIQUEIDENTIFIER   NOT NULL,
    [LookupVesselTypeIndex] INT                CONSTRAINT [DF_tblTankQualityTagLog_LookupVesselTypeIndex] DEFAULT ((0)) NOT NULL,
    [QualityTagGuid]        UNIQUEIDENTIFIER   NOT NULL,
    [TankGuid]              UNIQUEIDENTIFIER   NOT NULL,
    CONSTRAINT [PK_tblTankQualityTagLog_GUID] PRIMARY KEY NONCLUSTERED ([TankQualityTagLogGuid] ASC)
);


GO
CREATE CLUSTERED INDEX [IX_tblTankQualityTagLog_CreatedDate]
    ON [dbo].[tblTankQualityTagLog]([CreatedDate] ASC);

GO
