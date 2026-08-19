CREATE TABLE [dbo].[tblEnterpriseQueue] (
    [EnterpriseQueueGuid] UNIQUEIDENTIFIER   NOT NULL,
    [SourceType]          INT                NOT NULL,
    [SourceID]            NVARCHAR (120)     NOT NULL,
    [DateAdded]           DATETIMEOFFSET (7) CONSTRAINT [DF_tblEnterpriseQueue_DateAdded] DEFAULT (sysdatetimeoffset()) NOT NULL,
    [Priority]            INT                CONSTRAINT [DF_tblEnterpriseQueue_Priority] DEFAULT ((2)) NOT NULL,
    [Status]              INT                CONSTRAINT [DF_tblEnterpriseQueue_Status] DEFAULT ((0)) NOT NULL,
    [DateUpdated]         DATETIMEOFFSET (7) CONSTRAINT [DF_tblEnterpriseQueue_DateUpdated] DEFAULT (sysdatetimeoffset()) NOT NULL,
    [CreatedBy]           [dbo].[udtUserID]  CONSTRAINT [DF_tblEnterpriseQueue_CreatedBy] DEFAULT (suser_sname()) NOT NULL,
    [UpdatedBy]           [dbo].[udtUserID]  CONSTRAINT [DF_tblEnterpriseQueue_UpdatedBy] DEFAULT (suser_sname()) NOT NULL,
    [CreatedDate]         DATETIMEOFFSET (7) CONSTRAINT [DF_tblEnterpriseQueue_CreatedDate] DEFAULT (sysdatetimeoffset()) NOT NULL,
    [UpdatedDate]         DATETIMEOFFSET (7) CONSTRAINT [DF_tblEnterpriseQueue_UpdatedDate] DEFAULT (sysdatetimeoffset()) NOT NULL,
    [_RowVersion]         ROWVERSION         NOT NULL,
	[ErrorMessage]		[nvarchar](1024) NULL,
	[Attempts]			[int] NULL CONSTRAINT [DF_tblEnterpriseQueue_attempts]  DEFAULT ((0)),
    [_ClusterIdx]         BIGINT             IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK_tblEnterpriseQueue] PRIMARY KEY NONCLUSTERED ([EnterpriseQueueGuid] ASC),
    CONSTRAINT [DF_tblEnterpriseQueue_Unique_SourceType_SourceID] UNIQUE NONCLUSTERED ([SourceType] ASC, [SourceID] ASC)
);




GO



GO
CREATE NONCLUSTERED INDEX [IX_tblEnterpriseQueue_DateAdded]
    ON [dbo].[tblEnterpriseQueue]([DateAdded] ASC);


GO
CREATE UNIQUE CLUSTERED INDEX [IX_tblEnterpriseQueue_ClusterIdx]
    ON [dbo].[tblEnterpriseQueue]([_ClusterIdx] ASC);

GO

CREATE NONCLUSTERED INDEX [IX_tblEnterpriseQueue_SourceID] 
ON [dbo].[tblEnterpriseQueue]([SourceID] ASC)
GO

CREATE INDEX [IX_tblEnterpriseQueue_SourceType_Status] ON [dbo].[tblEnterpriseQueue] 
([SourceType], [Status]) INCLUDE ([EnterpriseQueueGuid], [SourceID], [DateAdded], [Priority], [DateUpdated], [CreatedBy], [UpdatedBy], [CreatedDate], [UpdatedDate], [_RowVersion])
GO
