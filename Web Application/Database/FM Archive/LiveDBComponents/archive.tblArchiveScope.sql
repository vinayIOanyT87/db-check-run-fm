/*

	DROP TABLE [archive].[tblArchiveScope]

*/
CREATE TABLE [archive].[tblArchiveScope] (
	[ArchiveScopeGuid]		UNIQUEIDENTIFIER   NOT NULL,
    [ScopeId]               NVARCHAR (50)      NULL,    
    [IsArchivingOn]			BIT				   NOT NULL,    
	[CreatedBy]             [dbo].[udtUserID]  NULL,
    [CreatedDate]           DATETIMEOFFSET (7) NULL,
    [UpdatedBy]             [dbo].[udtUserID]  NULL,
    [UpdatedDate]           DATETIMEOFFSET (7) NULL,
    [_RowVersion]           ROWVERSION         NOT NULL,
    [_ClusterIdx]           BIGINT             IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK_tblArchiveScope] PRIMARY KEY NONCLUSTERED ([ArchiveScopeGuid] ASC)
);

GO


CREATE UNIQUE CLUSTERED INDEX [IX_tblArchiveScope_ClusterIdx] 
	ON [archive].[tblArchiveScope]([_ClusterIdx]);
GO


ALTER TABLE [archive].[tblArchiveScope] ADD  DEFAULT ((0)) FOR [IsArchivingOn]
GO