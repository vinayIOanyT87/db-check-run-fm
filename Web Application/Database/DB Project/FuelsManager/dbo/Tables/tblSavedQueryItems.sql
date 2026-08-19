CREATE TABLE [dbo].[tblSavedQueryItems] (
    [QueryIndex]         INT                CONSTRAINT [DF_tblSavedQueryItems_QueryIndex] DEFAULT ((0)) NOT NULL,
    [FieldID]            NVARCHAR (30)      CONSTRAINT [DF_tblSavedQueryItems_FieldID] DEFAULT ('') NOT NULL,
    [ModifierID]         INT                CONSTRAINT [DF_tblSavedQueryItems_ModifierID] DEFAULT ((0)) NOT NULL,
    [Value]              NVARCHAR (50)      NULL,
    [JoinTypeID]         INT                CONSTRAINT [DF_tblSavedQueryItems_JoinTypeID] DEFAULT ((0)) NOT NULL,
    [ItemSequence]       INT                CONSTRAINT [DF_tblSavedQueryItems_ItemSequence] DEFAULT ((0)) NOT NULL,
    [CreatedBy]          [dbo].[udtUserID]  NULL,
    [CreatedDate]        DATETIMEOFFSET (7) NULL,
    [UpdatedBy]          [dbo].[udtUserID]  NULL,
    [UpdatedDate]        DATETIMEOFFSET (7) NULL,
    [SavedQueryItemGuid] UNIQUEIDENTIFIER   CONSTRAINT [DF_tblSavedQueryItems_GUID] DEFAULT (newid()) NOT NULL,
    [_RowVersion]        ROWVERSION         NOT NULL,
    [_ClusterIdx]        BIGINT             IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK_tblSavedQueryItems_GUID] PRIMARY KEY NONCLUSTERED ([SavedQueryItemGuid] ASC)
);




GO
CREATE NONCLUSTERED INDEX [IX_tblSavedQueryItems_CreatedDate]
    ON [dbo].[tblSavedQueryItems]([CreatedDate] ASC);




GO
CREATE UNIQUE CLUSTERED INDEX [IX_tblSavedQueryItems_ClusterIdx]
    ON [dbo].[tblSavedQueryItems]([_ClusterIdx] ASC);

