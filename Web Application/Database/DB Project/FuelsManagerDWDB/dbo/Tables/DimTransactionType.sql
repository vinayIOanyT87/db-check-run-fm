/*
	DROP TABLE [dbo].[DimTransactionType]
*/
CREATE TABLE [dbo].[DimTransactionType](
	[SKey] INT IDENTITY (1, 1) NOT NULL,
	[AKey] [nvarchar](50) NULL,
	[TransactionTypeCode] NVARCHAR (100) NULL,
	[TransactionTypeName] NVARCHAR (100) NULL,    
    CONSTRAINT [PK_DimTransactionType] PRIMARY KEY CLUSTERED ([SKey] ASC)
)
GO
