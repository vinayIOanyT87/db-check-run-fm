/*

	DROP TABLE [lookup].[tblQuantityDisplay]

*/
CREATE TABLE [lookup].[tblQuantityDisplay] (
    [QuantityDisplayIndex] TINYINT            NOT NULL,
    [QuantityDisplayCode]  NVARCHAR (100)     NOT NULL,
    [QuantityDisplayName]  NVARCHAR (100)     NULL,
    [QuantityDisplayGuid]  UNIQUEIDENTIFIER   NOT NULL,
    [CreatedDate]          DATETIMEOFFSET (7) NULL,
    [CreatedBy]            [dbo].[udtUserID]  NULL,
    [UpdatedDate]          DATETIMEOFFSET (7) NULL,
    [UpdatedBy]            [dbo].[udtUserID]  NULL,
    [_RowVersion]          ROWVERSION         NOT NULL,
    [_ClusterIdx]          BIGINT             IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK_lookup_tblQuantityDisplay] PRIMARY KEY NONCLUSTERED ([QuantityDisplayIndex] ASC)
);
GO
CREATE NONCLUSTERED INDEX [IXU_lookup_tblQuantityDisplay_QuantityDisplayGuid]
    ON [lookup].[tblQuantityDisplay]([CreatedDate] ASC);
GO
CREATE UNIQUE CLUSTERED INDEX [IX_tblQuantityDisplay_ClusterIdx]
    ON [lookup].[tblQuantityDisplay]([_ClusterIdx] ASC);