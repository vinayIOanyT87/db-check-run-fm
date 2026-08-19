/*
	DROP TABLE [fmcdc].[tblLastRowVersionProcessed]
*/
CREATE TABLE [fmcdc].[tblLastRowVersionProcessed](
	[RowIndex]					INT IDENTITY(1,1) NOT NULL,
	[EntityName]				NVARCHAR(50) NOT NULL,
	[StagingTableName]			NVARCHAR(50) NULL,
	[ProcessName]				NVARCHAR(50) NULL,
	[LastRowVersionProcessed]	BIGINT NULL,
	[UpdatedDate]				DATETIMEOFFSET(7) NULL,

	CONSTRAINT [PK_tblLastRowVersionProcessed] PRIMARY KEY CLUSTERED 
	(
		[RowIndex] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

) ON [PRIMARY]

GO