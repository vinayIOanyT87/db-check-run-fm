

CREATE PROCEDURE [dbo].[usp_CustomLedgerMovement]
@XMLstring NVARCHAR (MAX), @StartSiteIndex uniqueidentifier
AS
BEGIN
	SET NOCOUNT ON
	
	DECLARE @Aliases xml
	DECLARE @ProductGuid UNIQUEIDENTIFIER
	DECLARE @Tolerance float
	SET @Aliases = @XMLstring
	
	-- convert list of aliases and the data to a SQL table
	DECLARE @AliasTable TABLE (Name NVARCHAR(32), Gross FLOAT, Net FLOAT, Mass FLOAT, ProductGuid UNIQUEIDENTIFIER) 
	INSERT INTO @AliasTable (Name, Gross, Net, Mass, ProductGuid)	SELECT ParamValues.Alias.query('name').value('.','VARCHAR(32)') as Name, 
													   ParamValues.Alias.query('g').value('.','FLOAT') as Gross,
													   ParamValues.Alias.query('nt').value('.','FLOAT') as Net,
													   ParamValues.Alias.query('m').value('.','FLOAT') as Mass,
													   ParamValues.Alias.query('productGuid').value('.','UNIQUEIDENTIFIER') as ProductGuid
												FROM @Aliases.nodes('/Alias') as ParamValues(Alias)

	select @ProductGuid = ProductGuid FROM @AliasTable
	select @Tolerance = VarianceTolerance FROM tblProducts where ProductGuid = [erv].[udf_GetFirstParentRecordVersionGuid] ('Product',@ProductGuid, @StartSiteIndex)
	
	-- return the total movement based on the transaction alias type	
	SELECT CAST( SUM(a.Gross) AS FLOAT) as Gross, 
			CAST(SUM(a.Net) AS FLOAT) as Net,
			CAST(SUM(a.Mass) AS FLOAT) as Mass,
			CAST(@Tolerance as FLOAT) as Tolerance 
	FROM @AliasTable a 
	LEFT OUTER JOIN dbo.tblTransactionAliases ta ON a.Name = ta.AliasName
	LEFT OUTER JOIN [erv].[udf_GetTransactionAliasRecordVersions](@StartSiteIndex) c
	ON c.TransactionAliasGuid = ta.TransactionAliasGuid
	WHERE ta.LookupTransTypeIndex IN (5,6,25) -- Only type 5 and type 6 transactions affect inventory


END