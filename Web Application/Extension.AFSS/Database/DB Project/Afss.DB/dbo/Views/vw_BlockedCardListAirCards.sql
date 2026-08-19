CREATE VIEW [dbo].[vw_BlockedCardListAirCards]
	AS SELECT        CardNumber,UpdatedDate
FROM            dbo.tblGasboyDevice
WHERE        (GasboyDepartmentGuid = '00000001-0000-0000-0000-000000000000') AND (CardNumber like '789682%')
