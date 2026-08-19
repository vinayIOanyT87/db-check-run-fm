CREATE TABLE [dbo].[tblSequences] (
    [SequenceKey]   NVARCHAR (30)    CONSTRAINT [DF_tblSequences_SequenceKey] DEFAULT ('') NOT NULL,
    [SequenceValue] BIGINT           CONSTRAINT [DF_tblSequences_SequenceValue] DEFAULT ((0)) NOT NULL,
    [SequenceGuid]  UNIQUEIDENTIFIER CONSTRAINT [DF_tblSequences_GUID] DEFAULT (newid()) NOT NULL,
    [_RowVersion]   ROWVERSION       NOT NULL,
    [_ClusterIdx]   BIGINT           IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK_tblSequences_GUID] PRIMARY KEY NONCLUSTERED ([SequenceGuid] ASC)
);




GO



GO
CREATE NONCLUSTERED INDEX [IX_tblSequences_SequenceGuid]
    ON [dbo].[tblSequences]([SequenceGuid] ASC);


GO
CREATE UNIQUE CLUSTERED INDEX [IX_tblSequences_ClusterIdx]
    ON [dbo].[tblSequences]([_ClusterIdx] ASC);

