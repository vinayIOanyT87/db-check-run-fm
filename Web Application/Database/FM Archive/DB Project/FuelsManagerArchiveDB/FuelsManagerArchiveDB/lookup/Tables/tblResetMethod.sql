/*

	DROP TABLE [lookup].[tblResetMethod]

*/
CREATE TABLE [lookup].[tblResetMethod] (
    [ResetMethodIndex] INT                NOT NULL,
    [ResetMethodCode]  NVARCHAR (100)     NOT NULL,
    [ResetMethodName]  NVARCHAR (100)     NULL,
    [ResetMethodGuid]  UNIQUEIDENTIFIER   NOT NULL,
    [CreatedDate]      DATETIMEOFFSET (7) NULL,
    [CreatedBy]        [dbo].[udtUserID]  NULL,
    [UpdatedDate]      DATETIMEOFFSET (7) NULL,
    [UpdatedBy]        [dbo].[udtUserID]  NULL,
    [_RowVersion]      ROWVERSION         NOT NULL,
    [_ClusterIdx]      BIGINT             IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK_lookup_tblResetMethod] PRIMARY KEY NONCLUSTERED ([ResetMethodIndex] ASC)
);
GO
CREATE NONCLUSTERED INDEX [IXU_lookup_tblResetMethod_ResetMethodGuid]
    ON [lookup].[tblResetMethod]([CreatedDate] ASC);
GO
CREATE UNIQUE CLUSTERED INDEX [IX_tblResetMethod_ClusterIdx]
    ON [lookup].[tblResetMethod]([_ClusterIdx] ASC);