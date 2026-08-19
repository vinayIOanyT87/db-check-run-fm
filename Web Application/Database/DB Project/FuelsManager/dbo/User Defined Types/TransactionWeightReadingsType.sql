CREATE TYPE [dbo].[TransactionWeightReadingsType] AS TABLE (
    [TransactionGuid]        UNIQUEIDENTIFIER   NOT NULL,
    [CompartmentID]          NVARCHAR (30)      NOT NULL,
    [BeginQuantityValue]     FLOAT (53)         NULL,
    [RequestedQuantityValue] FLOAT (53)         NULL,
    [FinalQuantityValue]     FLOAT (53)         NULL,
    [SourceVersionNumber]    INT                NULL,
    [HistoricalFlag]         BIT                NOT NULL,
    [TransVersion]           BIGINT             NULL,
	VolumetricTopOffFlag     BIT				NULL,
    CreatedUpdatedBy         [dbo].[udtUserID]  NOT NULL);

