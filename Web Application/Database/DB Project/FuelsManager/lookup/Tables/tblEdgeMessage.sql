CREATE TABLE [lookup].[tblEdgeMessage]
(
    [EdgeMessageIndex] INT                NOT NULL,
    [EdgeMessageCode]  NVARCHAR (100)     NOT NULL,
    [EdgeMessageName]  NVARCHAR (100)     NULL,
    [EdgeMessageGuid]  UNIQUEIDENTIFIER   CONSTRAINT [DF_lookup_tblEdgeMessage_GUID] DEFAULT (newid()) NOT NULL,
    [CreatedDate]       DATETIMEOFFSET (7) CONSTRAINT [DF_lookup_tblEdgeMessage_CreatedDate] DEFAULT (sysdatetimeoffset()) NULL,
    [CreatedBy]         [dbo].[udtUserID]  CONSTRAINT [DF_lookup_tblEdgeMessage_CreatedBy] DEFAULT (suser_sname()) NULL,
    [UpdatedDate]       DATETIMEOFFSET (7) CONSTRAINT [DF_lookup_tblEdgeMessage_UpdatedDate] DEFAULT (sysdatetimeoffset()) NULL,
    [UpdatedBy]         [dbo].[udtUserID]  CONSTRAINT [DF_lookup_tblEdgeMessage_UpdatedBy] DEFAULT (suser_sname()) NULL,
    [_RowVersion]       ROWVERSION         NOT NULL,
    [_ClusterIdx]       BIGINT             IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK_lookup_tblEdgeMessage] PRIMARY KEY NONCLUSTERED ([EdgeMessageIndex] ASC)
);


