CREATE VIEW [dbo].[vw_CardListAll]
	AS SELECT        [CardNumber], CASE [GasboyDepartmentGuid] WHEN  '00000001-0000-0000-0000-000000000000' THEN 1 ELSE 0 END AS [Blocked], [UpdatedDate]
FROM            [dbo].[tblGasboyDevice]
