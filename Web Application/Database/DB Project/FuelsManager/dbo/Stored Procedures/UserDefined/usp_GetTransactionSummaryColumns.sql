CREATE PROCEDURE [dbo].[usp_GetTransactionSummaryColumns]
(
	@SiteGuid uniqueidentifier, @AliasName nvarchar(32)
)
AS
BEGIN
BEGIN TRY

	SELECT
		lv.[ID] AS 'ListView ID'
		,taf.DbName
		,taf.DisplayName
		,field.ColumnOrder
	FROM [dbo].[tblListViews] lv
	CROSS APPLY (
		SELECT lvf.ColumnOrder, lvf.[TransactionAliasGuid], lvf.[TransactionAliasFieldGuid] FROM [dbo].[tblListViewFields] lvf WHERE lv.[ListViewGuid] = lvf.[ListViewGuid]
	) field
	CROSS APPLY (
		SELECT ta.AliasName, ta.TransactionAliasGuid FROM [dbo].[tblTransactionAliases] ta WHERE lv.[TransactionAliasGuid] = ta.[TransactionAliasGuid]
	) alias
	CROSS APPLY (
		SELECT taf.DbName, taf.DisplayName, taf.DisplayOrder FROM [dbo].[tblTransactionAliasFields] taf 
		WHERE field.[TransactionAliasFieldGuid] = taf.[TransactionAliasFieldGuid]
		AND NOT (dbName = 'LookupTransactionStatusIndex' AND LookupTransactionFieldTypeIndex = 2)
	) taf
	INNER JOIN [map].[tblEntityTransactionAliasToSite] taMap ON alias.TransactionAliasGuid = taMap.TransactionAliasGuid AND taMap.SiteGuid = @SiteGuid
	INNER JOIN [map].[tblEntityListViewToSite] lvMap ON lv.ListViewGuid = lvMap.ListViewGuid AND lvMap.SiteGuid = @SiteGuid
	INNER JOIN INFORMATION_SCHEMA.COLUMNS allCols ON taf.DbName = allCols.COLUMN_NAME AND allCols.TABLE_NAME = N'vw_TransactionSummary'
	WHERE lv.LookupListViewTypeIndex = 1 AND alias.AliasName = @AliasName
	ORDER BY lv.ID, field.ColumnOrder, taf.DbName, taf.DisplayName

END TRY
BEGIN CATCH
	DECLARE	@_ErrMessage NVARCHAR(2048)
			,@_ErrNumber INT
			,@_ErrProcName NVARCHAR(126)
			,@_ErrLineNumber INT;
	SET @_ErrMessage = ERROR_MESSAGE();
	SET @_ErrNumber = ERROR_NUMBER();
	SET @_ErrProcName= ERROR_PROCEDURE();
	SET @_ErrLineNumber = ERROR_LINE();
	SET @_ErrMessage = 'Error: ' + @_ErrMessage + CHAR(13) + CHAR(10)
				+ 'Number: ' + CAST(@_ErrNumber AS VARCHAR(20)) + CHAR(13) + CHAR(10)
				+ 'Procedure Name: [dbo].usp_GetListViewFieldsByListView' + CHAR(13) + CHAR(10)
				+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)), '') + CHAR(13) + CHAR(10);
	RAISERROR(@_ErrMessage,18,1);
END CATCH
	
END
GO