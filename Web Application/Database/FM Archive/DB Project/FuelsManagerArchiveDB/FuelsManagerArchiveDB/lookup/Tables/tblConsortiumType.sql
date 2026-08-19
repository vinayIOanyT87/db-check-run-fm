/*

	DROP TABLE [lookup].[tblConsortiumType]

*/

CREATE TABLE [lookup].[tblConsortiumType]
(    
    [ConsortiumTypeIndex] INT                     NOT NULL,
    [ConsortiumTypeCode]  NVARCHAR (100)          NOT NULL,
    [ConsortiumTypeName]  NVARCHAR (100)          NULL,
    [ConsortiumTypeIndexGuid]  UNIQUEIDENTIFIER   NOT NULL,
    [CreatedDate]              DATETIMEOFFSET (7) NULL,
    [CreatedBy]                [dbo].[udtUserID]  NULL,
    [UpdatedDate]              DATETIMEOFFSET (7) NULL,
    [UpdatedBy]                [dbo].[udtUserID]  NULL,
    [_RowVersion]              ROWVERSION         NOT NULL,
    [_ClusterIdx]              BIGINT             IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK_lookup_tblConsortiumType] PRIMARY KEY NONCLUSTERED ([ConsortiumTypeIndex] ASC)
)