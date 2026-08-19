CREATE TABLE [dbo].[tblTestSetTankResults] (
    [ResultTimeStamp]          DATETIMEOFFSET (7) CONSTRAINT [DF_tblTestSetTankResults_ResultTimeStamp] DEFAULT (sysdatetimeoffset()) NOT NULL,
    [TestSetName]              NVARCHAR (80)      CONSTRAINT [DF_tblTestSetTankResults_TestSetName] DEFAULT ('') NOT NULL,
    [Inspector]                NVARCHAR (100)     CONSTRAINT [DF_tblTestSetTankResults_Inspector] DEFAULT ('') NOT NULL,
    [Supervisor]               NVARCHAR (100)     CONSTRAINT [DF_tblTestSetTankResults_Supervisor] DEFAULT ('') NOT NULL,
    [TankID]                   NVARCHAR (50)      NOT NULL,
    [SampleNumber]             INT                NULL,
    [SampleSize]               FLOAT (53)         CONSTRAINT [DF_tblTestSetTankResults_SampleSize] DEFAULT ((0.0)) NOT NULL,
    [IsRetest]                 BIT                CONSTRAINT [DF_tblTestSetTankResults_IsRetest] DEFAULT ((0)) NOT NULL,
    [PreviousSampleNumber]     INT                NULL,
    [DocumentNumber]           NVARCHAR (50)      NULL,
    [Memo]                     NVARCHAR (1000)    NULL,
    [GallonsRepresented]       FLOAT (53)         NULL,
    [Override]                 BIT                CONSTRAINT [DF_tblTestSetTankResults_Override] DEFAULT ((0)) NOT NULL,
    [DeleteFlag]               BIT                CONSTRAINT [DF_tblTestSetTankResults_DeleteFlag] DEFAULT ((0)) NOT NULL,
    [CreatedDate]              DATETIMEOFFSET (7) CONSTRAINT [DF_tblTestSetTankResults_CreatedDate] DEFAULT (sysdatetimeoffset()) NOT NULL,
    [CreatedBy]                [dbo].[udtUserID]  CONSTRAINT [DF_tblTestSetTankResults_CreatedBy] DEFAULT ('') NOT NULL,
    [UpdatedDate]              DATETIMEOFFSET (7) CONSTRAINT [DF_tblTestSetTankResults_UpdatedDate] DEFAULT (sysdatetimeoffset()) NOT NULL,
    [UpdatedBy]                [dbo].[udtUserID]  CONSTRAINT [DF_tblTestSetTankResults_UpdatedBy] DEFAULT ('') NOT NULL,
    [TestSetTankResultGuid]    UNIQUEIDENTIFIER   CONSTRAINT [DF_tblTestSetTankResults_GUID] DEFAULT (newid()) NOT NULL,
    [_RowVersion]              ROWVERSION         NOT NULL,
    [SiteGuid]                 UNIQUEIDENTIFIER   NOT NULL,
    [LookupTestSetStatusIndex] INT                CONSTRAINT [DF_tblTestSetTankResults_LookupTestSetStatusIndex] DEFAULT ((0)) NOT NULL,
    [TankGuid]                 UNIQUEIDENTIFIER   NOT NULL,
    CONSTRAINT [PK_tblTestSetTankResults_GUID] PRIMARY KEY NONCLUSTERED ([TestSetTankResultGuid] ASC)
);


GO
CREATE CLUSTERED INDEX [IX_tblTestSetTankResults_CreatedDate]
    ON [dbo].[tblTestSetTankResults]([CreatedDate] ASC);

GO
