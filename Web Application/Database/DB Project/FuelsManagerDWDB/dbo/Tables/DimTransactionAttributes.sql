/*

	DROP TABLE [dbo].[DimTransactionAttributes]

*/
CREATE TABLE [dbo].[DimTransactionAttributes] (
    [DeleteFlag]				BIT				NOT NULL,
	[ReversalType]              NVARCHAR (2)    NOT NULL,
	[SubType]                   NVARCHAR (20)   NOT NULL,
	[TransactionStatusName]		NVARCHAR(100)	NOT NULL,
	[InvalidTerminalTime]		BIT				NOT NULL DEFAULT(0),
	[GrossQuantitySign]			NVARCHAR(10)	NOT NULL,
	[IsRecordDeleted]			BIT				NOT NULL DEFAULT(0),

	[_RecordUpdatedDate]		DATETIMEOFFSET(7) NULL,
    [_DeletedFlag]				BIT				NULL,
    [SKey]						INT				IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK_DimTransactionAttributes] PRIMARY KEY CLUSTERED ([SKey] ASC) WITH (FILLFACTOR = 100)
);