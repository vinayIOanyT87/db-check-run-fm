CREATE TABLE [dbo].[tblReportApprovals] (
    [ReportName]                     NVARCHAR (75)      NOT NULL,
    [ParameterValue]                 NVARCHAR (50)      NULL,
    [MaximumRowVersionNumber]	     BIGINT             NOT NULL,
    [NextApprovalUser]               [dbo].[udtUserID]  NULL,
    [NextApprovalEmail]              NVARCHAR (50)      NULL,
    [ApprovalName]                   NVARCHAR (50)      NULL,
    [CreatedDate]                    DATETIMEOFFSET (7) CONSTRAINT [DF_tblReportApprovals_CreatedDate] DEFAULT (sysdatetimeoffset()) NOT NULL,
    [CreatedBy]                      [dbo].[udtUserID]  CONSTRAINT [DF_tblReportApprovals_CreatedBy] DEFAULT ('') NOT NULL,
	[CompanyManagerGuid]             UNIQUEIDENTIFIER   NOT NULL,
    [ReportApprovalGuid]             UNIQUEIDENTIFIER   CONSTRAINT [DF_tblReportApprovals_GUID] DEFAULT (newid()) NOT NULL,
    [_RowVersion]                    ROWVERSION         NOT NULL,
    [SiteGuid]                       UNIQUEIDENTIFIER   NOT NULL,
    [LookupReportApprovalStateIndex] INT                NOT NULL,
    CONSTRAINT [PK_tblReportApprovals_GUID] PRIMARY KEY NONCLUSTERED ([ReportApprovalGuid] ASC),
    CONSTRAINT [FK_tblReportApprovals_LookupReportApprovalStateIndex] FOREIGN KEY ([LookupReportApprovalStateIndex]) REFERENCES [lookup].[tblReportApprovalState] ([ReportApprovalStateIndex]),
    CONSTRAINT [FK_tblReportApprovals_SiteGuid] FOREIGN KEY ([SiteGuid]) REFERENCES [dbo].[tblSites] ([SiteGuid]),
	CONSTRAINT [FK_tblReportApprovals_CompanyManagerGuid] FOREIGN KEY ([CompanyManagerGuid]) REFERENCES [dbo].[tblCompanies] ([CompanyGuid])
);


GO
CREATE CLUSTERED INDEX [IX_tblReportApprovals_CreatedDate]
    ON [dbo].[tblReportApprovals]([CreatedDate] ASC);


GO
