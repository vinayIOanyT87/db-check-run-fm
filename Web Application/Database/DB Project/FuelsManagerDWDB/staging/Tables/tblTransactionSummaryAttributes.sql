/*

	DROP TABLE [staging].[tblTransactionSummaryAttributes]

*/
CREATE TABLE [staging].[tblTransactionSummaryAttributes](	
    [DeleteFlag]					BIT				NULL,
	[ReversalType]					NVARCHAR (2)    NULL,
	[SubType]						NVARCHAR (20)   NULL,
	[TransactionStatusName]			NVARCHAR(100)	NULL,
	[InvalidTerminalTime]			BIT				NOT NULL DEFAULT(0),	
	[IsRecordDeleted]				BIT				NOT NULL DEFAULT(0),

	[FactTransactionSummarySKey]	INT NULL,
	
	[DimTransactionAttributesSKey]	INT NULL,

	[IgnoreRecord]					BIT NOT NULL DEFAULT 0,
	[SKey]							INT IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK_TransactionSummaryAttributes] PRIMARY KEY CLUSTERED ([SKey] ASC)
)