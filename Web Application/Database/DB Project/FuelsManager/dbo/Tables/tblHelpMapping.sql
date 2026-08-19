CREATE TABLE [dbo].[tblHelpMapping] (
    [HelpMappingGuid] UNIQUEIDENTIFIER   CONSTRAINT [DF_tblHelpMapping_GUID] DEFAULT (newid()) NOT NULL,
    [HelpContextKey]  NVARCHAR (250)     NOT NULL,
    [HelpPage]        NVARCHAR (250)     NOT NULL,
    [CreatedDate]     DATETIMEOFFSET (7) CONSTRAINT [DF_tblHelpMapping_CreatedDate] DEFAULT (sysdatetimeoffset()) NOT NULL,
    [CreatedBy]       [dbo].[udtUserID]  CONSTRAINT [DF_tblHelpMapping_CreatedBy] DEFAULT (suser_sname()) NOT NULL,
    [UpdatedDate]     DATETIMEOFFSET (7) CONSTRAINT [DF_tblHelpMapping_UpdatedDate] DEFAULT (sysdatetimeoffset()) NOT NULL,
    [UpdatedBy]       [dbo].[udtUserID]  CONSTRAINT [DF_tblHelpMapping_UpdatedBy] DEFAULT (suser_sname()) NOT NULL,
    [_RowVersion]     ROWVERSION         NOT NULL,
    [_ClusterIdx]     BIGINT             IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK_tblHelpMapping_HelpMappingGuid] PRIMARY KEY NONCLUSTERED ([HelpMappingGuid] ASC)
);

GO

CREATE UNIQUE NONCLUSTERED INDEX [IX_tblHelpMapping_HelpContextKey]
    ON [dbo].[tblHelpMapping]([HelpContextKey] ASC);

GO

CREATE UNIQUE CLUSTERED INDEX [IX_tblHelpMapping_ClusterIdx]
    ON [dbo].[tblHelpMapping]([_ClusterIdx] ASC);

GO