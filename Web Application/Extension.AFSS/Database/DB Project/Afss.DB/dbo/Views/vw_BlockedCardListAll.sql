CREATE VIEW [dbo].[vw_BlockedCardListAll]
	AS SELECT        CardNumber,UpdatedDate
FROM            dbo.tblGasboyDevice
WHERE        (GasboyDepartmentGuid = '00000001-0000-0000-0000-000000000000')
