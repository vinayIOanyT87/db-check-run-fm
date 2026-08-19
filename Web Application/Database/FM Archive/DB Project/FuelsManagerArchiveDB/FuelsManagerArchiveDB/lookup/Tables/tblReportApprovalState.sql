/*

	DROP TABLE [lookup].[tblReportApprovalState]

*/
CREATE TABLE [lookup].[tblReportApprovalState](
	[ReportApprovalStateIndex] [int] NOT NULL,
	[ReportApprovalStateCode] [nvarchar](100) NOT NULL,
	[ReportApprovalStateName] [nvarchar](100) NULL,
	[ReportApprovalStateGuid] [uniqueidentifier] NOT NULL,
	[CreatedDate] [datetimeoffset](7) NULL,
	[CreatedBy] [dbo].[udtUserID] NULL,
	[UpdatedDate] [datetimeoffset](7) NULL,
	[UpdatedBy] [dbo].[udtUserID] NULL,
	[_RowVersion] [timestamp] NOT NULL,
 CONSTRAINT [PK_lookup_tblReportApprovalState] PRIMARY KEY CLUSTERED 
(
	[ReportApprovalStateIndex] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]