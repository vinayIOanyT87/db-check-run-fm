/*
	DROP TABLE [erv].[tblTempRecordVersioningField]
*/
CREATE TABLE [erv].[tblTempRecordVersioningField] (
    [VSFieldGuid]           UNIQUEIDENTIFIER   CONSTRAINT [DF_tblTempRecordVersioningField_GUID] DEFAULT (newid()) NOT NULL,
    [TargetField]           NVARCHAR (100)     NOT NULL,
    [IsExternalAttribute]   BIT                NULL,
    [InternalFieldName]     NVARCHAR (100)     NULL,
	[FieldLevelControlMode] NVARCHAR (20)	   NULL,
    [_CallingReferenceGuid] UNIQUEIDENTIFIER   NOT NULL,
    [CreatedDate]           DATETIMEOFFSET (7) CONSTRAINT [DF_tblTempRecordVersioningField_CreatedDate] DEFAULT (sysdatetimeoffset()) NOT NULL,
    [CreatedBy]             [dbo].[udtUserID]  CONSTRAINT [DF_tblTempRecordVersioningField_CreatedBy] DEFAULT ('') NOT NULL,
    [UpdatedDate]           DATETIMEOFFSET (7) CONSTRAINT [DF_tblTempRecordVersioningField_UpdatedDate] DEFAULT (sysdatetimeoffset()) NOT NULL,
    [UpdatedBy]             [dbo].[udtUserID]  CONSTRAINT [DF_tblTempRecordVersioningField_UpdatedBy] DEFAULT ('') NOT NULL,
    [_RowVersion]           ROWVERSION         NOT NULL,
    [_ClusterIdx]           BIGINT             IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK_tblRecordVersioningField] PRIMARY KEY NONCLUSTERED ([VSFieldGuid] ASC)
);
GO
CREATE NONCLUSTERED INDEX [IX_tblTempRecordVersioningField_CallingReferenceGuid]
    ON [erv].[tblTempRecordVersioningField]([_CallingReferenceGuid] ASC);
GO
CREATE NONCLUSTERED INDEX [IX_tblTempRecordVersioningField_CreatedDate]
    ON [erv].[tblTempRecordVersioningField]([CreatedDate] ASC);
GO
CREATE UNIQUE CLUSTERED INDEX [IX_tblTempRecordVersioningField_ClusterIdx]
    ON [erv].[tblTempRecordVersioningField]([_ClusterIdx] ASC);
GO
