/*
	DROP TABLE [staging].[tblEditedFactTransaction]
*/
CREATE TABLE [staging].[tblEditedFactTransaction](
	[SKey] [int] IDENTITY(1,1) NOT NULL,
	[FactTransactionSKey] [int] NOT NULL,
    CONSTRAINT [PK_tblEditedFactTransaction] PRIMARY KEY CLUSTERED ([SKey] ASC)
)