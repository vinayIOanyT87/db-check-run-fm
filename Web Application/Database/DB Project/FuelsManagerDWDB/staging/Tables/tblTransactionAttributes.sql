/*

	DROP TABLE [staging].[tblTransactionAttributes]

*/
CREATE TABLE [staging].[tblTransactionAttributes](	
    [DeleteFlag]					BIT				NULL,
	[ReversalType]					NVARCHAR (2)    NULL,
	[SubType]						NVARCHAR (20)   NULL,
	[TransactionStatusName]			NVARCHAR(100)	NULL,
	[InvalidTerminalTime]			BIT				NOT NULL DEFAULT(0),	
	[GrossQuantitySign]				NVARCHAR(10)	NULL,
	[IsRecordDeleted]				BIT				NOT NULL DEFAULT(0),

	[FactTransactionSKey]			INT NULL,
	
	[DimTransactionAttributesSKey]	INT NULL,

	[IgnoreRecord]					BIT NOT NULL DEFAULT 0,
	[SKey]							INT IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK_TransactionAttributes] PRIMARY KEY CLUSTERED ([SKey] ASC)
)