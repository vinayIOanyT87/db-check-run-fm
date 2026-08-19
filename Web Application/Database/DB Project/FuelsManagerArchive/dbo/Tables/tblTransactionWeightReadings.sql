CREATE TABLE [dbo].[tblTransactionWeightReadings] (
    [CompartmentID]                NVARCHAR (30)      CONSTRAINT [DF_tblTransactionWeightReadings_CompartmentID] DEFAULT ('') NOT NULL,
    [BeginQuantityValue]           FLOAT (53)         NULL,
    [RequestedQuantityValue]       FLOAT (53)         NULL,
    [FinalQuantityValue]           FLOAT (53)         NULL,
    [CreatedBy]                    [dbo].[udtUserID]  NULL,
    [CreatedDate]                  DATETIMEOFFSET (7) NULL,
    [UpdatedBy]                    [dbo].[udtUserID]  NULL,
    [UpdatedDate]                  DATETIMEOFFSET (7) NULL,
    [TransVersion]                 BIGINT             NULL,
    [TransactionWeightReadingGuid] UNIQUEIDENTIFIER   CONSTRAINT [DF_tblTransactionWeightReadings_GUID] DEFAULT (newid()) NOT NULL,
    [_RowVersion]                  ROWVERSION         NOT NULL,
    [TransactionGuid]              UNIQUEIDENTIFIER   NOT NULL,
    [FuelsManagerVersionNumber]    INT                NOT NULL,
    [SourceVersionNumber]          INT                NULL,
    [HistoricalFlag]               BIT                NOT NULL,
    [VolumetricTopOffFlag]		   BIT				  NULL, 
    CONSTRAINT [PK_tblTransactionWeightReadings_GUID] PRIMARY KEY NONCLUSTERED ([TransactionWeightReadingGuid] ASC)
);


GO
CREATE CLUSTERED INDEX [IX_tblTransactionWeightReadings_CreatedDate]
    ON [dbo].[tblTransactionWeightReadings]([CreatedDate] ASC);

GO
