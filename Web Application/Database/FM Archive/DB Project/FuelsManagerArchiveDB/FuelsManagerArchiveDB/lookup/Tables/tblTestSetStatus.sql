/*

	DROP TABLE [lookup].[tblTestSetStatus] 

*/
CREATE TABLE [lookup].[tblTestSetStatus] (
    [TestSetStatusIndex] INT                NOT NULL,
    [TestSetStatusCode]  NVARCHAR (100)     NOT NULL,
    [TestSetStatusName]  NVARCHAR (100)     NULL,
    [TestSetStatusGuid]  UNIQUEIDENTIFIER   NOT NULL,
    [CreatedDate]        DATETIMEOFFSET (7) NULL,
    [CreatedBy]          [dbo].[udtUserID]  NULL,
    [UpdatedDate]        DATETIMEOFFSET (7) NULL,
    [UpdatedBy]          [dbo].[udtUserID]  NULL,
    [_RowVersion]        ROWVERSION         NOT NULL,
    [_ClusterIdx]        BIGINT             IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK_lookup_tblTestSetStatus] PRIMARY KEY NONCLUSTERED ([TestSetStatusIndex] ASC)
);
GO
CREATE NONCLUSTERED INDEX [IXU_lookup_tblTestSetStatus_TestSetStatusGuid]
    ON [lookup].[tblTestSetStatus]([CreatedDate] ASC);
GO
CREATE UNIQUE CLUSTERED INDEX [IX_tblTestSetStatus_ClusterIdx]
    ON [lookup].[tblTestSetStatus]([_ClusterIdx] ASC);