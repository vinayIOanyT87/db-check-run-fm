/*

	DROP TABLE [lookup].[tblPersonnelRole]

*/
CREATE TABLE [lookup].[tblPersonnelRole] (
    [PersonnelRoleIndex] INT                NOT NULL,
    [PersonnelRoleCode]  NVARCHAR (100)     NOT NULL,
    [PersonnelRoleName]  NVARCHAR (100)     NULL,
    [PersonnelRoleGuid]  UNIQUEIDENTIFIER   NOT NULL,
    [CreatedDate]        DATETIMEOFFSET (7) NULL,
    [CreatedBy]          [dbo].[udtUserID]  NULL,
    [UpdatedDate]        DATETIMEOFFSET (7) NULL,
    [UpdatedBy]          [dbo].[udtUserID]  NULL,
    [_RowVersion]        ROWVERSION         NOT NULL,
    [_ClusterIdx]        BIGINT             IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK_lookup_tblPersonnelRole] PRIMARY KEY NONCLUSTERED ([PersonnelRoleIndex] ASC)
);
GO
CREATE UNIQUE CLUSTERED INDEX [IX_tblPersonnelRole_ClusterIdx]
    ON [lookup].[tblPersonnelRole]([_ClusterIdx] ASC);