CREATE TABLE [map].[tblFMAEProductID] (
    [FMAEProductIDMapGuid] UNIQUEIDENTIFIER   NOT NULL,
    [FMAEProductID]        NVARCHAR (30)      NOT NULL,
    [ProductGuid]          UNIQUEIDENTIFIER   NOT NULL,
    [CreatedDate]          DATETIMEOFFSET (7) NOT NULL,
    [CreatedBy]            [dbo].[udtUserID]  NOT NULL,
    [UpdatedDate]          DATETIMEOFFSET (7) NOT NULL,
    [UpdatedBy]            [dbo].[udtUserID]  NOT NULL,
    [_RowVersion]          ROWVERSION         NOT NULL,
    [_ClusterIdx]          BIGINT             IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK_map_tblFMAEProductID] PRIMARY KEY NONCLUSTERED ([FMAEProductIDMapGuid] ASC),
    CONSTRAINT [FK_map_tblFMAEProductID_ProductGuid] FOREIGN KEY ([ProductGuid]) REFERENCES [dbo].[tblProducts] ([ProductGuid])
);


GO
CREATE UNIQUE NONCLUSTERED INDEX [UIX_map_tblFMAEProductID_FMAEProductID]
    ON [map].[tblFMAEProductID]([FMAEProductID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_map_tblFMAEProductID_FMAEProductIDMapGuid]
    ON [map].[tblFMAEProductID]([FMAEProductIDMapGuid] ASC);


GO

GO
CREATE UNIQUE CLUSTERED INDEX [IX_tblFMAEProductID_ClusterIdx]
    ON [map].[tblFMAEProductID]([_ClusterIdx] ASC);

