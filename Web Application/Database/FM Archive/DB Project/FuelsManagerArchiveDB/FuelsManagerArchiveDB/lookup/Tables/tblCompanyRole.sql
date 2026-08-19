/*

	DROP TABLE [lookup].[tblCompanyRole] 

*/

CREATE TABLE [lookup].[tblCompanyRole] (
    [CompanyRoleIndex] INT                NOT NULL,
    [CompanyRoleCode]  NVARCHAR (100)     NOT NULL,
    [CompanyRoleName]  NVARCHAR (100)     NULL,
    [CompanyRoleGuid]  UNIQUEIDENTIFIER   NOT NULL,
    [CreatedDate]      DATETIMEOFFSET (7) NULL,
    [CreatedBy]        [dbo].[udtUserID]  NULL,
    [UpdatedDate]      DATETIMEOFFSET (7) NULL,
    [UpdatedBy]        [dbo].[udtUserID]  NULL,
    [_RowVersion]      ROWVERSION         NOT NULL,
    [_ClusterIdx]      BIGINT             IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK_lookup_tblCompanyRole] PRIMARY KEY NONCLUSTERED ([CompanyRoleIndex] ASC)
);
GO
CREATE UNIQUE CLUSTERED INDEX [IX_tblCompanyRole_ClusterIdx]
    ON [lookup].[tblCompanyRole]([_ClusterIdx] ASC);