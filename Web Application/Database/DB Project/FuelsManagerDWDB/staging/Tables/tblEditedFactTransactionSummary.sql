/*
	DROP TABLE [staging].[tblEditedFactTransactionSummary]
*/
CREATE TABLE [staging].[tblEditedFactTransactionSummary](
	[SKey] [int] IDENTITY(1,1) NOT NULL,
	[FactTransactionSummarySKey] [int] NOT NULL,
    CONSTRAINT [PK_tblEditedFactTransactionSummary] PRIMARY KEY CLUSTERED ([SKey] ASC)
)