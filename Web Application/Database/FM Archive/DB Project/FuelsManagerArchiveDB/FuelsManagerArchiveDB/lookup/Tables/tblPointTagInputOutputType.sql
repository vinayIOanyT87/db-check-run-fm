/*

	DROP TABLE [lookup].[tblPointTagInputOutputType]

*/
CREATE TABLE [lookup].[tblPointTagInputOutputType]
(
    [PointTagInputOutputTypeIndex] INT                NOT NULL,
    [PointTagInputOutputTypeCode]  NVARCHAR (100)     NOT NULL,
    [PointTagInputOutputTypeName]  NVARCHAR (100)     NULL,
    [PointTagInputOutputTypeGuid]  UNIQUEIDENTIFIER   NOT NULL,
    [CreatedDate]       DATETIMEOFFSET (7) NULL,
    [CreatedBy]         [dbo].[udtUserID]  NULL,
    [UpdatedDate]       DATETIMEOFFSET (7) NULL,
    [UpdatedBy]         [dbo].[udtUserID]  NULL,
    [_RowVersion]       ROWVERSION         NOT NULL,
	[_ClusterIdx]		BIGINT			   NOT NULL IDENTITY,
    CONSTRAINT [PK_lookup_tblPointTagInputOutputType] PRIMARY KEY NONCLUSTERED ([PointTagInputOutputTypeIndex] ASC)
)
GO
CREATE UNIQUE CLUSTERED INDEX [IX_tblPointTagInputOutputType_ClusterIdx] 
	ON [lookup].[tblPointTagInputOutputType]([_ClusterIdx]);
GO
CREATE NONCLUSTERED INDEX [IXU_lookup_tblPointTagInputOutputType_PointTagInputOutputTypeGuid]
    ON [lookup].[tblPointTagInputOutputType]([CreatedDate] ASC);