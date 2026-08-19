CREATE TABLE [dbo].[tblPointService](
                [Hostname] [nvarchar](256) NOT NULL,
                [LastPingTime] [datetimeoffset](7) NOT NULL,
                [PingIntervalInSeconds] [int] NOT NULL,
                [HealthStatusIndex] [int] NOT NULL,
				[MaxNumberOfPoints] [int] CONSTRAINT [DF_tblPointService_MaxNumberOfPoints] DEFAULT (0) NOT NULL,
                [PercentCpuUtilization] [float] NOT NULL,
                [PercentCpuUtilizationThrottleLevel] [float] NOT NULL,
                [PercentMemoryUtilization] [float] NOT NULL,
                [PercentMemoryUtilizationThrottleLevel] [float] NOT NULL,
                [CreatedDate] [datetimeoffset](7) CONSTRAINT [DF_dbo_tblPointService_CreatedDate]  DEFAULT (sysdatetimeoffset()) NOT NULL,
                [CreatedBy] [dbo].[udtUserID] CONSTRAINT [DF_dbo_tblPointService_CreatedBy]  DEFAULT (suser_sname()) NOT NULL,
                [UpdatedDate] [datetimeoffset](7) CONSTRAINT [DF_dbo_tblPointService_UpdatedDate]  DEFAULT (sysdatetimeoffset()) NOT NULL,
                [UpdatedBy] [dbo].[udtUserID] CONSTRAINT [DF_dbo_tblPointService_UpdatedBy]  DEFAULT (suser_sname()) NOT NULL,
                [PointServiceGuid] [uniqueidentifier] CONSTRAINT [DF_dbo_tblPointService_GUID]  DEFAULT (newid()) NOT NULL,
                [_RowVersion] [timestamp] NOT NULL,
                [_ClusterIdx] [bigint] IDENTITY(1,1) NOT NULL,
					CONSTRAINT [PK_tblPointService_GUID] PRIMARY KEY NONCLUSTERED ([PointServiceGuid] ASC)
);

GO
CREATE UNIQUE CLUSTERED INDEX [IX_tblPointService_ClusterIdx] ON [dbo].[tblPointService]
(
       [_ClusterIdx] ASC
) ON [PRIMARY]

GO
CREATE UNIQUE INDEX [IX_tblPointService_Hostname] ON [dbo].[tblPointService]
(
       [Hostname] ASC
) ON [PRIMARY]

GO
ALTER TABLE [dbo].[tblPointService]  ADD  CONSTRAINT [FK_tblPointService_HealthStatusIndex] FOREIGN KEY([HealthStatusIndex])
REFERENCES [lookup].[tblPointServiceHealthStatus] ([PointServiceHealthStatusIndex])
GO

ALTER TABLE [dbo].[tblPointService] CHECK CONSTRAINT [FK_tblPointService_HealthStatusIndex]
GO
