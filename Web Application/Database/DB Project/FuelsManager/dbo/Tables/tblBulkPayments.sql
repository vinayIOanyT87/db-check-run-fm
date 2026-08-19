CREATE TABLE [dbo].[tblBulkPayments] (
    [Site]            NVARCHAR (60)      NOT NULL,
    [Section]         NVARCHAR (60)      NULL,
    [PaymentType]     NVARCHAR (60)      NOT NULL,
    [ForeignRate]     FLOAT (53)         NULL,
    [ForeignUnit]     NVARCHAR (60)      NULL,
    [RomanNumber]     NVARCHAR (60)      NULL,
    [DiscountRate]    FLOAT (53)         NULL,
    [PaymentDueDate]  DATETIMEOFFSET (7) NOT NULL,
    [TransactionDate] DATETIMEOFFSET (7) NOT NULL,
    [Supplier]        NVARCHAR (60)      NOT NULL,
    [CreatedBy]       [dbo].[udtUserID]  NULL,
    [CreatedDate]     DATETIMEOFFSET (7) NULL,
    [UpdatedBy]       [dbo].[udtUserID]  NULL,
    [UpdatedDate]     DATETIMEOFFSET (7) NULL,
    [BulkPaymentGuid] UNIQUEIDENTIFIER   CONSTRAINT [DF_tblBulkPayments_GUID] DEFAULT (newid()) NOT NULL,
    [_RowVersion]     ROWVERSION         NOT NULL,
    [_ClusterIdx]     BIGINT             IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK_tblBulkPayments_GUID] PRIMARY KEY NONCLUSTERED ([BulkPaymentGuid] ASC)
);




GO
CREATE NONCLUSTERED INDEX [IX_tblBulkPayments_CreatedDate]
    ON [dbo].[tblBulkPayments]([CreatedDate] ASC);




GO
CREATE UNIQUE CLUSTERED INDEX [IX_tblBulkPayments_ClusterIdx]
    ON [dbo].[tblBulkPayments]([_ClusterIdx] ASC);

