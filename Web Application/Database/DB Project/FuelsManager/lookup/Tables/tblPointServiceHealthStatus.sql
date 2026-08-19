CREATE TABLE [lookup].[tblPointServiceHealthStatus](
                [PointServiceHealthStatusIndex] [int] NOT NULL,
                [PointServiceHealthStatusCode] [nvarchar](100) NOT NULL,
                [PointServiceHealthStatusName] [nvarchar](100) NULL,
                [PointServiceHealthStatusGuid] [uniqueidentifier] CONSTRAINT [DF_lookup_tblPointServiceHealthStatus_GUID] DEFAULT (newid()) NOT NULL,
                [CreatedDate] [datetimeoffset](7) CONSTRAINT [DF_lookup_tblPointServiceHealthStatus_CreatedDate]  DEFAULT (sysdatetimeoffset()) NOT NULL,
                [CreatedBy] [dbo].[udtUserID] CONSTRAINT [DF_lookup_tblPointServiceHealthStatus_CreatedBy]  DEFAULT (suser_sname()) NULL,
                [UpdatedDate] [datetimeoffset](7)  CONSTRAINT [DF_lookup_tblPointServiceHealthStatus_UpdatedDate]  DEFAULT (sysdatetimeoffset()) NOT NULL,
                [UpdatedBy] [dbo].[udtUserID]  CONSTRAINT [DF_lookup_tblPointServiceHealthStatus_UpdatedBy]  DEFAULT (suser_sname()) NULL,
                [_RowVersion] [timestamp] NOT NULL,
                [_ClusterIdx] [bigint] IDENTITY(1,1)  NOT NULL,
					 CONSTRAINT [PK_lookup_tblPointServiceHealthStatus] PRIMARY KEY NONCLUSTERED ([PointServiceHealthStatusIndex] ASC)
)

GO

CREATE UNIQUE CLUSTERED INDEX [IX_tblPointServiceHealthStatus_ClusterIdx] ON [lookup].[tblPointServiceHealthStatus]
(
       [_ClusterIdx] ASC
) ON [PRIMARY]
GO

