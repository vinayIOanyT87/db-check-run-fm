/*

	DROP TABLE [archive].[tblArchiveScopeToTable]

*/
CREATE TABLE [archive].[tblArchiveScopeToTable] (
	[ArchiveScopeToTableGuid]	UNIQUEIDENTIFIER   NOT NULL,
	[ArchiveScopeGuid]		UNIQUEIDENTIFIER   NOT NULL,
    [SourceArchiveTable]    NVARCHAR (100)	   NULL,    
	[CreatedBy]             [dbo].[udtUserID]  NULL,
    [CreatedDate]           DATETIMEOFFSET (7) NULL,
    [UpdatedBy]             [dbo].[udtUserID]  NULL,
    [UpdatedDate]           DATETIMEOFFSET (7) NULL,
    [_RowVersion]           ROWVERSION         NOT NULL,
    [_ClusterIdx]           BIGINT             IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK_tblArchiveScopeToTable] PRIMARY KEY NONCLUSTERED ([ArchiveScopeToTableGuid] ASC)
);

GO


CREATE UNIQUE CLUSTERED INDEX [IX_tblArchiveScopeTable_ClusterIdx] 
	ON [archive].[tblArchiveScopeToTable]([_ClusterIdx]);
GO

CREATE UNIQUE NONCLUSTERED INDEX [IX_tblArchiveScopeTable_SourceArchiveTable] ON [archive].[tblArchiveScopeToTable]
(
	[SourceArchiveTable] ASC
)
GO

ALTER TABLE [archive].[tblArchiveScopeToTable]  WITH CHECK ADD  CONSTRAINT [FK_tblArchiveScopeToTable_tblArchiveScope] FOREIGN KEY([ArchiveScopeGuid])
REFERENCES [archive].[tblArchiveScope] ([ArchiveScopeGuid])
GO

ALTER TABLE [archive].[tblArchiveScopeToTable] CHECK CONSTRAINT [FK_tblArchiveScopeToTable_tblArchiveScope]
GO
