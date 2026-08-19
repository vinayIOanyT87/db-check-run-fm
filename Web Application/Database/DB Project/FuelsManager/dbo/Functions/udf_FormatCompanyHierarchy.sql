


CREATE FUNCTION [dbo].[udf_FormatCompanyHierarchy]
(@Level INT, @CompanyMapGuid UNIQUEIDENTIFIER)
RETURNS NVARCHAR(410)
--WITH SCHEMABINDING
AS
BEGIN 
	DECLARE @Result NVARCHAR(410)
	
	IF @Level = 0
	BEGIN
		IF EXISTS (SELECT CompanyLoadOwnerToManagerGuid FROM [map].[tblCompanyLoadOwnerToManager] WHERE CompanyLoadOwnerToManagerGuid = @CompanyMapGuid)
			SET @Result = (SELECT DISTINCT(c2.ID + ' -> ' + c1.ID)
							FROM  [map].[tblCompanyLoadOwnerToManager] a
							LEFT JOIN [dbo].[tblCompanies] c1 ON c1.CompanyGuid = a.CompanyGuid
							LEFT JOIN [dbo].[tblCompanies] c2 ON c2.CompanyGuid = a.AssignedToCompanyGuid
							WHERE CompanyLoadOwnerToManagerGuid = @CompanyMapGuid)
		ELSE
			SET @Result = (SELECT DISTINCT(coalesce(c2.ID, ca2.ID, '[N/A]') + ' -> '
							+ coalesce(c1.ID, ca1.ID, '[N/A]'))
							FROM  [fmaudit].[map_tblCompanyLoadOwnerToManager] a
							LEFT JOIN [dbo].[tblCompanies] c1 ON c1.CompanyGuid = a.CompanyGuid
							LEFT JOIN [fmaudit].[tblCompanies] ca1 ON ca1.CompanyGuid = a.CompanyGuid AND ca1._AuditEventType = 'D'
							LEFT JOIN [dbo].[tblCompanies] c2 ON c2.CompanyGuid = a.AssignedToCompanyGuid
							LEFT JOIN [fmaudit].[tblCompanies] ca2 ON ca2.CompanyGuid = a.AssignedToCompanyGuid AND ca2._AuditEventType = 'D'
							WHERE CompanyLoadOwnerToManagerGuid = @CompanyMapGuid AND a._AuditEventType = 'D')

	END
	ELSE IF @Level = 1
	BEGIN
		IF EXISTS (SELECT CompanyShipperToOwnerGuid FROM [map].tblCompanyShipperToOwner WHERE CompanyShipperToOwnerGuid = @CompanyMapGuid)
			SET @Result = (SELECT DISTINCT(c3.ID + ' -> ' + c2.ID + ' -> ' + c1.ID)
							FROM  [map].[tblCompanyShipperToOwner] a
							LEFT JOIN [dbo].[tblCompanies] c1 ON c1.CompanyGuid = a.CompanyGuid
							LEFT JOIN [map].[tblCompanyLoadOwnerToManager] h1 ON h1.CompanyLoadOwnerToManagerGuid = a.CompanyLoadOwnerToManagerGuid
							LEFT JOIN [dbo].[tblCompanies] c2 ON c2.CompanyGuid = h1.CompanyGuid
							LEFT JOIN [dbo].[tblCompanies] c3 ON c3.CompanyGuid = h1.AssignedToCompanyGuid
							WHERE CompanyShipperToOwnerGuid = @CompanyMapGuid)
		ELSE
			SET @Result = (SELECT DISTINCT(coalesce(c3.ID, ca3.ID, '[N/A]') + ' -> '
							+ coalesce(c2.ID, ca2.ID, '[N/A]') + ' -> '
 							+ coalesce(c1.ID, ca1.ID, '[N/A]'))
							FROM  [fmaudit].[map_tblCompanyShipperToOwner] a
							LEFT JOIN [dbo].[tblCompanies] c1 ON c1.CompanyGuid = a.CompanyGuid
							LEFT JOIN [fmaudit].[tblCompanies] ca1 ON ca1.CompanyGuid = a.CompanyGuid AND ca1._AuditEventType = 'D'
							LEFT JOIN [map].[tblCompanyLoadOwnerToManager] h1 ON h1.CompanyLoadOwnerToManagerGuid = a.CompanyLoadOwnerToManagerGuid
							LEFT JOIN [fmaudit].[map_tblCompanyLoadOwnerToManager] ha1 ON ha1.CompanyLoadOwnerToManagerGuid = a.CompanyLoadOwnerToManagerGuid AND ha1._AuditEventType = 'D'
							LEFT JOIN [dbo].[tblCompanies] c2 ON c2.CompanyGuid = h1.CompanyGuid
							LEFT JOIN [fmaudit].[tblCompanies] ca2 ON ca2.CompanyGuid = ha1.CompanyGuid AND ca2._AuditEventType = 'D'
							LEFT JOIN [dbo].[tblCompanies] c3 ON c3.CompanyGuid = h1.AssignedToCompanyGuid
							LEFT JOIN [fmaudit].[tblCompanies] ca3 ON ca3.CompanyGuid = ha1.AssignedToCompanyGuid AND ca3._AuditEventType = 'D'
							WHERE CompanyShipperToOwnerGuid = @CompanyMapGuid)
	END
	ELSE IF @Level = 2
	BEGIN
		IF EXISTS (SELECT CompanyBillToToShipperGuid FROM [map].tblCompanyBillToToShipper WHERE CompanyBillToToShipperGuid = @CompanyMapGuid)
			SET @Result = (SELECT DISTINCT(c4.ID + ' -> ' + c3.ID + ' -> ' + c2.ID + ' -> ' + c1.ID)
							FROM  [map].[tblCompanyBillToToShipper] a
							LEFT JOIN [dbo].[tblCompanies] c1 ON c1.CompanyGuid = a.CompanyGuid
							LEFT JOIN [map].[tblCompanyShipperToOwner] h1 ON h1.CompanyShipperToOwnerGuid = a.CompanyShipperToOwnerGuid
							LEFT JOIN [dbo].[tblCompanies] c2 ON c2.CompanyGuid = h1.CompanyGuid
							LEFT JOIN [map].[tblCompanyLoadOwnerToManager] h2 ON h2.CompanyLoadOwnerToManagerGuid = h1.CompanyLoadOwnerToManagerGuid
							LEFT JOIN [dbo].[tblCompanies] c3 ON c3.CompanyGuid = h2.CompanyGuid
							LEFT JOIN [dbo].[tblCompanies] c4 ON c4.CompanyGuid = h2.AssignedToCompanyGuid
							WHERE CompanyBillToToShipperGuid = @CompanyMapGuid)
		ELSE
			SET @Result = (SELECT DISTINCT(coalesce(c4.ID, ca4.ID, '[N/A]') + ' -> '
							+ coalesce(c3.ID, ca3.ID, '[N/A]') + ' -> '
							+ coalesce(c2.ID, ca2.ID, '[N/A]') + ' -> '
 							+ coalesce(c1.ID, ca1.ID, '[N/A]'))
							FROM  [fmaudit].[map_tblCompanyBillToToShipper] a
							LEFT JOIN [dbo].[tblCompanies] c1 ON c1.CompanyGuid = a.CompanyGuid
							LEFT JOIN [fmaudit].[tblCompanies] ca1 ON ca1.CompanyGuid = a.CompanyGuid AND ca1._AuditEventType = 'D'
							LEFT JOIN [map].[tblCompanyShipperToOwner] h1 ON h1.CompanyShipperToOwnerGuid = a.CompanyShipperToOwnerGuid
							LEFT JOIN [fmaudit].[map_tblCompanyShipperToOwner] ha1 ON ha1.CompanyShipperToOwnerGuid = a.CompanyShipperToOwnerGuid AND ha1._AuditEventType = 'D'
							LEFT JOIN [dbo].[tblCompanies] c2 ON c2.CompanyGuid = h1.CompanyGuid
							LEFT JOIN [fmaudit].[tblCompanies] ca2 ON ca2.CompanyGuid = ha1.CompanyGuid AND ca2._AuditEventType = 'D'
							LEFT JOIN [map].[tblCompanyLoadOwnerToManager] h2 ON h2.CompanyLoadOwnerToManagerGuid = h1.CompanyLoadOwnerToManagerGuid
							LEFT JOIN [fmaudit].[map_tblCompanyLoadOwnerToManager] ha2 ON ha2.CompanyLoadOwnerToManagerGuid = ha1.CompanyLoadOwnerToManagerGuid AND ha2._AuditEventType = 'D'
							LEFT JOIN [dbo].[tblCompanies] c3 ON c3.CompanyGuid = h2.CompanyGuid
							LEFT JOIN [fmaudit].[tblCompanies] ca3 ON ca3.CompanyGuid = ha2.CompanyGuid AND ca3._AuditEventType = 'D'
							LEFT JOIN [dbo].[tblCompanies] c4 ON c4.CompanyGuid = h2.AssignedToCompanyGuid
							LEFT JOIN [fmaudit].[tblCompanies] ca4 ON ca4.CompanyGuid = ha2.AssignedToCompanyGuid AND ca4._AuditEventType = 'D'
							WHERE CompanyBillToToShipperGuid = @CompanyMapGuid)
	END
	ELSE IF @Level = 3
	BEGIN
		IF EXISTS (SELECT CompanyShipToToBillToGuid FROM [map].tblCompanyShipToToBillTo WHERE CompanyShipToToBillToGuid = @CompanyMapGuid)
			SET @Result = (SELECT DISTINCT(c5.ID + ' -> ' + c4.ID + ' -> ' + c3.ID + ' -> ' + c2.ID + ' -> '+ c1.ID)
							FROM  [map].[tblCompanyShipToToBillTo] a
							LEFT JOIN [dbo].[tblCompanies] c1 ON c1.CompanyGuid = a.CompanyGuid
							LEFT JOIN [map].[tblCompanyBillToToShipper] h1 ON h1.CompanyBillToToShipperGuid = a.CompanyBillToToShipperGuid
							LEFT JOIN [dbo].[tblCompanies] c2 ON c2.CompanyGuid = h1.CompanyGuid
							LEFT JOIN [map].[tblCompanyShipperToOwner] h2 ON h2.CompanyShipperToOwnerGuid = h1.CompanyShipperToOwnerGuid
							LEFT JOIN [dbo].[tblCompanies] c3 ON c3.CompanyGuid = h2.CompanyGuid
							LEFT JOIN [map].[tblCompanyLoadOwnerToManager] h3 ON h3.CompanyLoadOwnerToManagerGuid = h2.CompanyLoadOwnerToManagerGuid
							LEFT JOIN [dbo].[tblCompanies] c4 ON c4.CompanyGuid = h3.CompanyGuid
							LEFT JOIN [dbo].[tblCompanies] c5 ON c5.CompanyGuid = h3.AssignedToCompanyGuid
							WHERE CompanyShipToToBillToGuid = @CompanyMapGuid)
		ELSE
			SET @Result = (SELECT DISTINCT(coalesce(c5.ID, ca5.ID, '[N/A]') + ' -> '
							+ coalesce(c4.ID, ca4.ID, '[N/A]') + ' -> '
							+ coalesce(c3.ID, ca3.ID, '[N/A]') + ' -> '
							+ coalesce(c2.ID, ca2.ID, '[N/A]') + ' -> '
							+ coalesce(c1.ID, ca1.ID, '[N/A]'))
							FROM  [fmaudit].[map_tblCompanyShipToToBillTo] a
							LEFT JOIN [dbo].[tblCompanies] c1 ON c1.CompanyGuid = a.CompanyGuid
							LEFT JOIN [fmaudit].[tblCompanies] ca1 ON ca1.CompanyGuid = a.CompanyGuid AND ca1._AuditEventType = 'D'
							LEFT JOIN [map].[tblCompanyBillToToShipper] h1 ON h1.CompanyBillToToShipperGuid = a.CompanyBillToToShipperGuid
							LEFT JOIN [fmaudit].[map_tblCompanyBillToToShipper] ha1 ON ha1.CompanyBillToToShipperGuid = a.CompanyBillToToShipperGuid AND ha1._AuditEventType = 'D'
							LEFT JOIN [dbo].[tblCompanies] c2 ON c2.CompanyGuid = h1.CompanyGuid
							LEFT JOIN [fmaudit].[tblCompanies] ca2 ON ca2.CompanyGuid = ha1.CompanyGuid AND ca2._AuditEventType = 'D'
							LEFT JOIN [map].[tblCompanyShipperToOwner] h2 ON h2.CompanyShipperToOwnerGuid = h1.CompanyShipperToOwnerGuid
							LEFT JOIN [fmaudit].[map_tblCompanyShipperToOwner] ha2 ON ha2.CompanyShipperToOwnerGuid = ha1.CompanyShipperToOwnerGuid AND ha2._AuditEventType = 'D'
							LEFT JOIN [dbo].[tblCompanies] c3 ON c3.CompanyGuid = h2.CompanyGuid
							LEFT JOIN [fmaudit].[tblCompanies] ca3 ON ca3.CompanyGuid = ha2.CompanyGuid AND ca3._AuditEventType = 'D'
							LEFT JOIN [map].[tblCompanyLoadOwnerToManager] h3 ON h3.CompanyLoadOwnerToManagerGuid = h2.CompanyLoadOwnerToManagerGuid
							LEFT JOIN [fmaudit].[map_tblCompanyLoadOwnerToManager] ha3 ON ha3.CompanyLoadOwnerToManagerGuid = ha2.CompanyLoadOwnerToManagerGuid AND ha3._AuditEventType = 'D'
							LEFT JOIN [dbo].[tblCompanies] c4 ON c4.CompanyGuid = h3.CompanyGuid
							LEFT JOIN [fmaudit].[tblCompanies] ca4 ON ca4.CompanyGuid = ha3.CompanyGuid AND ca4._AuditEventType = 'D'
							LEFT JOIN [dbo].[tblCompanies] c5 ON c5.CompanyGuid = h3.AssignedToCompanyGuid
							LEFT JOIN [fmaudit].[tblCompanies] ca5 ON ca5.CompanyGuid = ha3.AssignedToCompanyGuid AND ca5._AuditEventType = 'D'
							WHERE CompanyShipToToBillToGuid = @CompanyMapGuid)
	END

	ELSE IF @Level = 4
	BEGIN
		IF EXISTS (SELECT CompanyOffLoadOwnerToManagerGuid FROM [map].tblCompanyOffLoadOwnerToManager WHERE CompanyOffLoadOwnerToManagerGuid = @CompanyMapGuid)
			SET @Result = (SELECT DISTINCT(c2.ID + ' -> ' + c1.ID)
							FROM  [map].[tblCompanyOffLoadOwnerToManager] a
							LEFT JOIN [dbo].[tblCompanies] c1 ON c1.CompanyGuid = a.CompanyGuid
							LEFT JOIN [dbo].[tblCompanies] c2 ON c2.CompanyGuid = a.AssignedToCompanyGuid
							WHERE CompanyOffLoadOwnerToManagerGuid = @CompanyMapGuid)
		ELSE
			SET @Result = (SELECT DISTINCT(coalesce(c2.ID, ca2.ID, '[N/A]') + ' -> '
							+ coalesce(c1.ID, ca1.ID, '[N/A]'))
							FROM  [fmaudit].[map_tblCompanyOffLoadOwnerToManager] a
							LEFT JOIN [dbo].[tblCompanies] c1 ON c1.CompanyGuid = a.CompanyGuid
							LEFT JOIN [fmaudit].[tblCompanies] ca1 ON ca1.CompanyGuid = a.CompanyGuid AND ca1._AuditEventType = 'D'
							LEFT JOIN [dbo].[tblCompanies] c2 ON c2.CompanyGuid = a.AssignedToCompanyGuid
							LEFT JOIN [fmaudit].[tblCompanies] ca2 ON ca2.CompanyGuid = a.AssignedToCompanyGuid AND ca2._AuditEventType = 'D'
							WHERE CompanyOffLoadOwnerToManagerGuid = @CompanyMapGuid)
	END
	ELSE IF @Level = 5
	BEGIN
		IF EXISTS (SELECT CompanySupplierToOwnerGuid FROM [map].tblCompanySupplierToOwner WHERE CompanySupplierToOwnerGuid = @CompanyMapGuid)
			SET @Result = (SELECT DISTINCT(c3.ID + ' -> ' + c2.ID + ' -> ' + c1.ID)
							FROM  [map].[tblCompanySupplierToOwner] a
							LEFT JOIN [dbo].[tblCompanies] c1 ON c1.CompanyGuid = a.CompanyGuid
							LEFT JOIN [map].[tblCompanyOffLoadOwnerToManager] h1 ON h1.CompanyOffLoadOwnerToManagerGuid = a.CompanyOffLoadOwnerToManagerGuid
							LEFT JOIN [dbo].[tblCompanies] c2 ON c2.CompanyGuid = h1.CompanyGuid
							LEFT JOIN [dbo].[tblCompanies] c3 ON c3.CompanyGuid = h1.AssignedToCompanyGuid
							WHERE CompanySupplierToOwnerGuid = @CompanyMapGuid)
		ELSE
			SET @Result = (SELECT DISTINCT(coalesce(c3.ID, ca3.ID, '[N/A]') + ' -> '
							+ coalesce(c2.ID, ca2.ID, '[N/A]') + ' -> '
 							+ coalesce(c1.ID, ca1.ID, '[N/A]'))
							FROM  [fmaudit].[map_tblCompanySupplierToOwner] a
							LEFT JOIN [dbo].[tblCompanies] c1 ON c1.CompanyGuid = a.CompanyGuid
							LEFT JOIN [fmaudit].[tblCompanies] ca1 ON ca1.CompanyGuid = a.CompanyGuid AND ca1._AuditEventType = 'D'
							LEFT JOIN [map].[tblCompanyOffLoadOwnerToManager] h1 ON h1.CompanyOffLoadOwnerToManagerGuid = a.CompanyOffLoadOwnerToManagerGuid
							LEFT JOIN [fmaudit].[map_tblCompanyOffLoadOwnerToManager] ha1 ON ha1.CompanyOffLoadOwnerToManagerGuid = a.CompanyOffLoadOwnerToManagerGuid AND ha1._AuditEventType = 'D'
							LEFT JOIN [dbo].[tblCompanies] c2 ON c2.CompanyGuid = h1.CompanyGuid
							LEFT JOIN [fmaudit].[tblCompanies] ca2 ON ca2.CompanyGuid = ha1.CompanyGuid AND ca2._AuditEventType = 'D'
							LEFT JOIN [dbo].[tblCompanies] c3 ON c3.CompanyGuid = h1.AssignedToCompanyGuid
							LEFT JOIN [fmaudit].[tblCompanies] ca3 ON ca3.CompanyGuid = ha1.AssignedToCompanyGuid AND ca3._AuditEventType = 'D'
							WHERE CompanySupplierToOwnerGuid = @CompanyMapGuid)
	END
	ELSE
		SET @Result = 'Unknown Hierarchy Level'

	RETURN @Result 
END


