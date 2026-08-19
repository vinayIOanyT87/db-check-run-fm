CREATE TABLE [dbo].[tblPointServiceSchedulingTime](
                [Hostname] [nvarchar](256) NOT NULL,
                [LastSchedulingTime] [datetimeoffset](7) NOT NULL,
                [CreatedDate] [datetimeoffset](7) NOT NULL,
                [CreatedBy] [dbo].[udtUserID] NOT NULL,
                [UpdatedDate] [datetimeoffset](7) NOT NULL,
                [UpdatedBy] [dbo].[udtUserID] NOT NULL,
                [PointServiceSchedulingTimeGuid] [uniqueidentifier] NOT NULL,
                [_RowVersion] [timestamp] NOT NULL,
                [_ClusterIdx] [bigint] IDENTITY(1,1) NOT NULL,
CONSTRAINT [PK_tblPointServiceSchedulingTime_GUID] PRIMARY KEY NONCLUSTERED 
(
                [PointServiceSchedulingTimeGuid] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

CREATE UNIQUE CLUSTERED INDEX [IX_tblPointServiceSchedulingTime_ClusterIdx] ON [dbo].[tblPointServiceSchedulingTime]
(
       [_ClusterIdx] ASC
) ON [PRIMARY]
GO


ALTER TABLE [dbo].[tblPointServiceSchedulingTime] ADD  CONSTRAINT [DF_dbo_tblPointServiceSchedulingTime_GUID]  DEFAULT (newid()) FOR [PointServiceSchedulingTimeGuid]
GO

ALTER TABLE [dbo].[tblPointServiceSchedulingTime] ADD  CONSTRAINT [DF_dbo_tblPointServiceSchedulingTime_CreatedDate]  DEFAULT (sysdatetimeoffset()) FOR [CreatedDate]
GO

ALTER TABLE [dbo].[tblPointServiceSchedulingTime] ADD  CONSTRAINT [DF_dbo_tblPointServiceSchedulingTime_CreatedBy]  DEFAULT (suser_sname()) FOR [CreatedBy]
GO

ALTER TABLE [dbo].[tblPointServiceSchedulingTime] ADD  CONSTRAINT [DF_dbo_tblPointServiceSchedulingTime_UpdatedDate]  DEFAULT (sysdatetimeoffset()) FOR [UpdatedDate]
GO

ALTER TABLE [dbo].[tblPointServiceSchedulingTime] ADD  CONSTRAINT [DF_dbo_tblPointServiceSchedulingTime_UpdatedBy]  DEFAULT (suser_sname()) FOR [UpdatedBy]
GO
