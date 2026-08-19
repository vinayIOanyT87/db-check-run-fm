CREATE TABLE [dbo].[tblTestEquipmentResults] (
    [TestName]                   NVARCHAR (80)      CONSTRAINT [DF_tblTestEquipmentResults_TestName] DEFAULT ('') NOT NULL,
    [Measurement]                NVARCHAR (50)      NULL,
    [TestDate]                   DATETIMEOFFSET (7) NULL,
    [DeleteFlag]                 BIT                CONSTRAINT [DF_tblTestEquipmentResults_DeleteFlag] DEFAULT ((0)) NOT NULL,
    [CreatedDate]                DATETIMEOFFSET (7) CONSTRAINT [DF_tblTestEquipmentResults_CreatedDate] DEFAULT (sysdatetimeoffset()) NOT NULL,
    [CreatedBy]                  [dbo].[udtUserID]  CONSTRAINT [DF_tblTestEquipmentResults_CreatedBy] DEFAULT ('') NOT NULL,
    [UpdatedDate]                DATETIMEOFFSET (7) CONSTRAINT [DF_tblTestEquipmentResults_UpdatedDate] DEFAULT (sysdatetimeoffset()) NOT NULL,
    [UpdatedBy]                  [dbo].[udtUserID]  CONSTRAINT [DF_tblTestEquipmentResults_UpdatedBy] DEFAULT ('') NOT NULL,
    [PerformedBy]                NVARCHAR (100)     NULL,
    [Supervisor]                 NVARCHAR (100)     NULL,
    [TestEquipmentResultGuid]    UNIQUEIDENTIFIER   CONSTRAINT [DF_tblTestEquipmentResults_GUID] DEFAULT (newid()) NOT NULL,
    [_RowVersion]                ROWVERSION         NOT NULL,
    [LookupTestSetStatusIndex]   INT                CONSTRAINT [DF_tblTestEquipmentResults_LookupTestSetStatusIndex] DEFAULT ((0)) NOT NULL,
    [TestSetEquipmentResultGuid] UNIQUEIDENTIFIER   NOT NULL,
    CONSTRAINT [PK_tblTestEquipmentResults_GUID] PRIMARY KEY NONCLUSTERED ([TestEquipmentResultGuid] ASC)
);


GO

CREATE CLUSTERED INDEX [IX_tblTestEquipmentResults_CreatedDate]
    ON [dbo].[tblTestEquipmentResults]([CreatedDate] ASC);

GO
