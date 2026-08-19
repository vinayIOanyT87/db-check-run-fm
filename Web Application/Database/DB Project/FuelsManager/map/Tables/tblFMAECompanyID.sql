CREATE TABLE [map].[tblFMAECompanyID] (
    [FMAECompanyIDMapGuid] UNIQUEIDENTIFIER   NOT NULL,
    [FMAECompanyID]        NVARCHAR (100)     NOT NULL,
    [CompanyGuid]          UNIQUEIDENTIFIER   NOT NULL,
    [CreatedDate]          DATETIMEOFFSET (7) NOT NULL,
    [CreatedBy]            [dbo].[udtUserID]  NOT NULL,
    [UpdatedDate]          DATETIMEOFFSET (7) NOT NULL,
    [UpdatedBy]            [dbo].[udtUserID]  NOT NULL,
    [_RowVersion]          ROWVERSION         NOT NULL,
    [_ClusterIdx]          BIGINT             IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK_map_tblFMAECompanyID] PRIMARY KEY NONCLUSTERED ([FMAECompanyIDMapGuid] ASC),
    CONSTRAINT [FK_map_tblFMAECompanyID_CompanyGuid] FOREIGN KEY ([CompanyGuid]) REFERENCES [dbo].[tblCompanies] ([CompanyGuid])
);


GO
CREATE UNIQUE NONCLUSTERED INDEX [UIX_map_tblFMAECompanyID_FMAECompanyID]
    ON [map].[tblFMAECompanyID]([FMAECompanyID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_map_tblFMAECompanyID_FMAECompanyIDMapGuid]
    ON [map].[tblFMAECompanyID]([FMAECompanyIDMapGuid] ASC);


GO
CREATE NONCLUSTERED INDEX IX_tblFMAECompanyID_CompanyGuid ON [map].[tblFMAECompanyID]([CompanyGuid])

GO
CREATE UNIQUE CLUSTERED INDEX [IX_tblFMAECompanyID_ClusterIdx]
    ON [map].[tblFMAECompanyID]([_ClusterIdx] ASC);

