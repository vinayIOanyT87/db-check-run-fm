CREATE TABLE [dbo].[tblInvoiceQueries] (
    [Description]      NVARCHAR (512)     NULL,
    [CreatedBy]        [dbo].[udtUserID]  NULL,
    [CreatedDate]      DATETIMEOFFSET (7) NULL,
    [UpdatedBy]        [dbo].[udtUserID]  NULL,
    [UpdatedDate]      DATETIMEOFFSET (7) NULL,
    [InvoiceQueryGuid] UNIQUEIDENTIFIER   CONSTRAINT [DF_tblInvoiceQueries_GUID] DEFAULT (newid()) NOT NULL,
    [_RowVersion]      ROWVERSION         NOT NULL,
    [_ClusterIdx]      BIGINT             IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK_tblInvoiceQueries_GUID] PRIMARY KEY NONCLUSTERED ([InvoiceQueryGuid] ASC)
);




GO
CREATE NONCLUSTERED INDEX [IX_tblInvoiceQueries_CreatedDate]
    ON [dbo].[tblInvoiceQueries]([CreatedDate] ASC);




GO
CREATE UNIQUE CLUSTERED INDEX [IX_tblInvoiceQueries_ClusterIdx]
    ON [dbo].[tblInvoiceQueries]([_ClusterIdx] ASC);

