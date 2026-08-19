/*

	DROP TABLE [lookup].[tblPointServiceHealthStatus]

*/
CREATE TABLE [lookup].[tblPointServiceHealthStatus](
                [PointServiceHealthStatusIndex] [int] NOT NULL,
                [PointServiceHealthStatusCode] [nvarchar](100) NOT NULL,
                [PointServiceHealthStatusName] [nvarchar](100) NULL,
                [PointServiceHealthStatusGuid] [uniqueidentifier] NOT NULL,
                [CreatedDate] [datetimeoffset](7) NULL,
                [CreatedBy] [dbo].[udtUserID] NULL,
                [UpdatedDate] [datetimeoffset](7)  NULL,
                [UpdatedBy] [dbo].[udtUserID]  NULL,
                [_RowVersion] [timestamp] NOT NULL,
                [_ClusterIdx] [bigint] IDENTITY(1,1)  NOT NULL,
					 CONSTRAINT [PK_lookup_tblPointServiceHealthStatus] PRIMARY KEY NONCLUSTERED ([PointServiceHealthStatusIndex] ASC)
)
GO
CREATE UNIQUE CLUSTERED INDEX [IX_tblPointServiceHealthStatus_ClusterIdx] ON [lookup].[tblPointServiceHealthStatus]
(
       [_ClusterIdx] ASC
) ON [PRIMARY]