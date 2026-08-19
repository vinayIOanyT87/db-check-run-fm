CREATE TABLE [dbo].[tblTestTankResults] (
    [TestName]                 NVARCHAR (80)      CONSTRAINT [DF_tblTestTankResults_TestName] DEFAULT ('') NOT NULL,
    [Measurement]              NVARCHAR (50)      NULL,
    [TestDate]                 DATETIMEOFFSET (7) NULL,
    [DeleteFlag]               BIT                CONSTRAINT [DF_tblTestTankResults_DeleteFlag] DEFAULT ((0)) NOT NULL,
    [CreatedDate]              DATETIMEOFFSET (7) CONSTRAINT [DF_tblTestTankResults_CreatedDate] DEFAULT (sysdatetimeoffset()) NOT NULL,
    [CreatedBy]                [dbo].[udtUserID]  CONSTRAINT [DF_tblTestTankResults_CreatedBy] DEFAULT ('') NOT NULL,
    [UpdatedDate]              DATETIMEOFFSET (7) CONSTRAINT [DF_tblTestTankResults_UpdatedDate] DEFAULT (sysdatetimeoffset()) NOT NULL,
    [UpdatedBy]                [dbo].[udtUserID]  CONSTRAINT [DF_tblTestTankResults_UpdatedBy] DEFAULT ('') NOT NULL,
    [PerformedBy]              NVARCHAR (100)     NULL,
    [Supervisor]               NVARCHAR (100)     NULL,
    [TestTankResultGuid]       UNIQUEIDENTIFIER   CONSTRAINT [DF_tblTestTankResults_GUID] DEFAULT (newid()) NOT NULL,
    [_RowVersion]              ROWVERSION         NOT NULL,
    [LookupTestSetStatusIndex] INT                CONSTRAINT [DF_tblTestTankResults_LookupTestSetStatusIndex] DEFAULT ((0)) NOT NULL,
    [TestSetTankResultGuid]    UNIQUEIDENTIFIER   NOT NULL,
    CONSTRAINT [PK_tblTestTankResults_GUID] PRIMARY KEY NONCLUSTERED ([TestTankResultGuid] ASC)
);


GO
CREATE CLUSTERED INDEX [IX_tblTestTankResults_CreatedDate]
    ON [dbo].[tblTestTankResults]([CreatedDate] ASC);

GO
