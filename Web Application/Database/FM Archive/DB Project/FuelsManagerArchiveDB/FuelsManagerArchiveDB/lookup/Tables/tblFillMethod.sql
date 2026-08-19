/*

	DROP TABLE [lookup].[tblFillMethod]

*/
CREATE TABLE [lookup].[tblFillMethod] (
    [FillMethodIndex] TINYINT            NOT NULL,
    [FillMethodCode]  NVARCHAR (100)     NOT NULL,
    [FillMethodName]  NVARCHAR (100)     NULL,
    [FillMethodGuid]  UNIQUEIDENTIFIER   NOT NULL,
    [CreatedDate]     DATETIMEOFFSET (7) NULL,
    [CreatedBy]       [dbo].[udtUserID]  NULL,
    [UpdatedDate]     DATETIMEOFFSET (7) NULL,
    [UpdatedBy]       [dbo].[udtUserID]  NULL,
    [_RowVersion]     ROWVERSION         NOT NULL,
    [_ClusterIdx]     BIGINT             IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK_lookup_tblFillMethod] PRIMARY KEY NONCLUSTERED ([FillMethodIndex] ASC)
);
GO
CREATE NONCLUSTERED INDEX [IXU_lookup_tblFillMethod_FillMethodGuid]
    ON [lookup].[tblFillMethod]([CreatedDate] ASC);
GO
CREATE UNIQUE CLUSTERED INDEX [IX_tblFillMethod_ClusterIdx]
    ON [lookup].[tblFillMethod]([_ClusterIdx] ASC);