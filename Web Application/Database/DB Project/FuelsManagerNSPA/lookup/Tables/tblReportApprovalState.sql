CREATE TABLE [lookup].[tblReportApprovalState] (
    [ReportApprovalStateIndex] INT                NOT NULL,
    [ReportApprovalStateCode]  NVARCHAR (100)     NOT NULL,
    [ReportApprovalStateName]  NVARCHAR (100)     NULL,
    [ReportApprovalStateGuid]  UNIQUEIDENTIFIER   CONSTRAINT [DF_lookup_tblReportApprovalState_GUID] DEFAULT (newid()) NOT NULL,
    [CreatedDate]              DATETIMEOFFSET (7) CONSTRAINT [DF_lookup_tblReportApprovalState_CreatedDate] DEFAULT (sysdatetimeoffset()) NULL,
    [CreatedBy]                [dbo].[udtUserID]  CONSTRAINT [DF_lookup_tblReportApprovalState_CreatedBy] DEFAULT (suser_sname()) NULL,
    [UpdatedDate]              DATETIMEOFFSET (7) CONSTRAINT [DF_lookup_tblReportApprovalState_UpdatedDate] DEFAULT (sysdatetimeoffset()) NULL,
    [UpdatedBy]                [dbo].[udtUserID]  CONSTRAINT [DF_lookup_tblReportApprovalState_UpdatedBy] DEFAULT (suser_sname()) NULL,
    [_RowVersion]              ROWVERSION         NOT NULL,
    CONSTRAINT [PK_lookup_tblReportApprovalState] PRIMARY KEY CLUSTERED ([ReportApprovalStateIndex] ASC)
);


GO
CREATE NONCLUSTERED INDEX [IXU_lookup_tblReportApprovalState_ReportApprovalStateGuid]
    ON [lookup].[tblReportApprovalState]([CreatedDate] ASC);


GO
