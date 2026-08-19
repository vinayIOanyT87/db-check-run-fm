CREATE TABLE [map].[tblPointToPointService](
		[PointToPointServiceGuid] [uniqueidentifier] CONSTRAINT [DF_map_tblPointToPointService_PointToPointServiceGUID]  DEFAULT (newid()) NOT NULL,
		[PointGuid] [uniqueidentifier] NOT NULL,
		[PointServiceGuid] [uniqueidentifier] NOT NULL,
		[TimeAssigned] [datetimeoffset](7) NULL,
		[CreatedDate] [datetimeoffset](7) CONSTRAINT [DF_map_tblPointToPointService_CreatedDate]  DEFAULT (sysdatetimeoffset()) NULL,
		[CreatedBy] [dbo].[udtUserID] CONSTRAINT [DF_map_tblPointToPointService_CreatedBy]  DEFAULT (suser_sname()) NULL,
		[UpdatedDate] [datetimeoffset](7) CONSTRAINT [DF_map_tblPointToPointService_UpdatedDate]  DEFAULT (sysdatetimeoffset()) NULL,
		[UpdatedBy] [dbo].[udtUserID] CONSTRAINT [DF_map_tblPointToPointService_UpdatedBy]  DEFAULT (suser_sname()) NULL,
		[_RowVersion] [timestamp] NOT NULL,
		[_ClusterIdx] [bigint] IDENTITY(1,1) NOT NULL,
		CONSTRAINT [PK_map_tblPointToPointService_GUID] PRIMARY KEY NONCLUSTERED ([PointToPointServiceGuid] ASC)
)
GO

CREATE UNIQUE CLUSTERED INDEX [IX_tblPointToPointService_ClusterIdx] ON [map].[tblPointToPointService]
(
	[_ClusterIdx] ASC
)
GO

CREATE UNIQUE INDEX [IX_tblPointToPointService_PointGuid] ON [map].[tblPointToPointService]
(
	[PointGuid] ASC
) 
GO


CREATE NONCLUSTERED   INDEX [IX_tblPointToPointService_PointServiceGuid]
ON [map].[tblPointToPointService] ([PointServiceGuid])
INCLUDE ([PointGuid]);
GO



ALTER TABLE [map].[tblPointToPointService]  ADD  CONSTRAINT [FK_map_tblPointToPointService_PointGuid] FOREIGN KEY([PointGuid])
REFERENCES [dbo].[tblPoint] ([PointGuid]) ON DELETE CASCADE
GO

ALTER TABLE [map].[tblPointToPointService] CHECK CONSTRAINT [FK_map_tblPointToPointService_PointGuid]
GO

ALTER TABLE [map].[tblPointToPointService]  ADD  CONSTRAINT [FK_map_tblPointToPointService_PointServiceGuid] FOREIGN KEY([PointServiceGuid])
REFERENCES [dbo].[tblPointService] ([PointServiceGuid]) ON DELETE CASCADE
GO

ALTER TABLE [map].[tblPointToPointService] CHECK CONSTRAINT [FK_map_tblPointToPointService_PointServiceGuid]
GO
