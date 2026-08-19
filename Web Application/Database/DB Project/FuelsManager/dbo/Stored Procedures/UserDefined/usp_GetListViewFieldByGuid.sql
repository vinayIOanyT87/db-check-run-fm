

/*
	EXEC [dbo].[usp_GetListViewFieldByGuid] '00000000-0000-0000-0000-000000000001', '9A983E99-4C81-490D-ADE3-6A5D2117E7C9'

*/

CREATE PROCEDURE [dbo].[usp_GetListViewFieldByGuid]
(
	@TargetSiteGuid uniqueidentifier, @ListViewFieldGuid uniqueidentifier
)
AS
BEGIN
	------------------------------------------------------------------------------------------------------
	-- Stored Procedure: [dbo].[usp_GetListViewFieldByGuid]
	-- Author: Hansraj Bapoo
	-- Version/Date: 1.0.003 / 2012-07-13 14:21:10.4470770 -04:00
	-- Purpose: Retrieve the List View Fields of a List View for a given ListViewFieldGuid, for a given Site/SiteGroup.
	-- Notes:
	-- 1. @TargetSiteGuid: Limit results to List View that have been assigned to this site/sitegroup only
	-- 2. @ListViewFieldGuid: Limit results to List View Field that have a Guid corresponding to  @ListViewFieldGuid.
	-- 4. This stored procedure replaces the ListViewField.SelectSQL inline SQL for the case where bInTransaction is false.
	-- 6. The query examines both child record versions that are owned by the Target Site/SiteGroup (RecordVersioning ON), and record versions 
	--    that are not owned by the Target Site/SiteGroup, but that have been assigned to the Target Site/SiteGroup (RecordVersioning OFF).
	-- 7. With the exception of the Transaction Alias Guid that is retrieved specifically for the targetted site, the other Transaction Alias 
	--    external attributes (Transaction Alias Fields, Transaction Alias User Data, Transaction Alias User Data Line Item) are all retrieved
	--    for the original owner site of the List View Field, and not for the targetted site.
	------------------------------------------------------------------------------------------------------
	BEGIN TRY	

		SELECT a.*, 
		(CASE a.LookupListViewFieldTypeIndex WHEN 1 THEN b.AliasName ELSE NULL END) AS AliasName,
		(CASE a.LookupListViewFieldTypeIndex WHEN 2 THEN d.TransactionAliasGuid ELSE NULL END) AS TransactionAliasGuid,
		(CASE a.LookupListViewFieldTypeIndex WHEN 2 THEN c.DisplayName ELSE NULL END) AS AliasFieldName,
		(CASE a.LookupListViewFieldTypeIndex WHEN 2 THEN c.LookupTransactionFieldTypeIndex ELSE NULL END) AS AliasFieldType,
		(CASE a.LookupListViewFieldTypeIndex WHEN 2 THEN c.DbName ELSE NULL END) AS AliasFieldDbName,
		(CASE a.LookupListViewFieldTypeIndex WHEN 3 THEN e.DisplayName ELSE NULL END) AS UserDataName,
		(CASE a.LookupListViewFieldTypeIndex WHEN 5 THEN f.DisplayName ELSE NULL END) AS LineItemUserDataName,
		(CASE a.LookupListViewFieldTypeIndex WHEN 3 THEN e.Number ELSE NULL END) AS UserDataNumber,
		(CASE a.LookupListViewFieldTypeIndex WHEN 5 THEN f.Number ELSE NULL END) AS LineItemUserDataNumber,
		(CASE a.LookupListViewFieldTypeIndex WHEN 2 THEN c.Virtual ELSE NULL END) AS VirtualField,
		(CASE a.LookupListViewFieldTypeIndex WHEN 6 THEN g.ID ELSE NULL END) AS AggregateID
		FROM tblListViewFields a
		LEFT OUTER JOIN tblTransactionAliases b
		ON b.TransactionAliasGuid = [erv].[udf_GetFirstParentRecordVersionGuid] ('Transaction_Alias', a.TransactionAliasGuid, @TargetSiteGuid)
		LEFT OUTER JOIN tblTransactionAliasFields c
		ON c.TransactionAliasFieldGuid = a.TransactionAliasFieldGuid
		LEFT OUTER JOIN tblTransactionAliases d
		ON d.TransactionAliasGuid = [erv].[udf_GetFirstParentRecordVersionGuid] ('Transaction_Alias', c.TransactionAliasGuid, @TargetSiteGuid)
		LEFT OUTER JOIN tblUserDataFieldTransactionAlias e 
		ON e.UserDataFieldTransactionAliasGuid = a.UserDataFieldTransactionAliasGuid
		LEFT OUTER JOIN tblUserDataFieldTransactionAliasLineItem f
		ON f.UserDataFieldTransactionAliasLineItemGuid = a.UserDataFieldTransactionAliasLineItemGuid
		LEFT OUTER JOIN tblLedgerAggregateColumns g
		ON g.LedgerAggregateColumnGuid = a.LedgerAggregateColumnGuid
		WHERE a.ListViewFieldGuid = @ListViewFieldGuid

	END TRY
	BEGIN CATCH  
		DECLARE	@_ErrMessage NVARCHAR(2048)      
				, @_ErrNumber INT           
				, @_ErrProcName NVARCHAR(126)           
				, @_ErrLineNumber INT;            
		SET @_ErrMessage = ERROR_MESSAGE();        
		SET @_ErrNumber = ERROR_NUMBER();        
		SET @_ErrProcName= ERROR_PROCEDURE();        
		SET @_ErrLineNumber = ERROR_LINE();            
		SET @_ErrMessage = 'Error: ' + @_ErrMessage + CHAR(13)+CHAR(10)                 
						+ 'Number: ' + CAST(@_ErrNumber AS VARCHAR(20)) + CHAR(13)+CHAR(10)                 
						+ 'Procedure Name: [dbo].usp_GetListViewFieldByGuid' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
	
END