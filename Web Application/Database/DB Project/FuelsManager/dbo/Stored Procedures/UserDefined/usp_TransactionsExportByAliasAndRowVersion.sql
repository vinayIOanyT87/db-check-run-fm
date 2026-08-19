
-- This stored procedure will retrieve transactions in format for csv export with transacion notes, users data  and line items
-- original usage for Port Buffalo Niagara BOLExport

-- FM12.0-SP3_HotFix003: Added optional @ExportInterfaceName parameter which if provided is used to filter out transactions already exported by that interface

CREATE PROCEDURE [dbo].[usp_TransactionsExportByAliasAndRowVersion]
	@AliasNames NVARCHAR (200), 
	@BOLExportServiceLastRowVersion timestamp,
	@ConvertUnits bit = 0,
    @AllowedTransactionStatuses NVARCHAR(200) = NULL,
	@ExportInterfaceName NVARCHAR(150) = NULL
AS
BEGIN
		SELECT  t.TransID, 
                t.InventoryDate, 
                t.ManagerID, 
                t.OwnerID, 
                t.CarrierID, 
                t.ShipToID, 
				t.BillToID, 
                t.UpdatedBy, 
                t.UpdatedDate, 
                t.TransVersion,
                t.DocumentNumber,
				t._RowVersion AS RowVersion,
				t.AliasName,
				t.FST,  
                t.TimeEnd,  
                t.LoadID,  
                t.DestinationRegistrationID1,
				t.DeleteFlag,   
                t.LookupTransTypeIndex,
				t.OperatorID AS OperatorID, 
				tl.ArmNumber, 
				CASE WHEN @ConvertUnits = 1 THEN
							dbo.[udf_ConvertFromSIUnits](tl.GrossQuantity,dbo.udf_ProductTypeFactor(tl.ProductType,s.AdditiveVolumeUnitIndex,s.AdditiveVolumeUnitIndex),dbo.udf_ProductTypeFactor(tl.ProductType,s.AdditiveVolumeDecimalPlaces,s.VolumeDecimalPlaces))
					 ELSE 
							tl.GrossQuantity
				END AS 'GrossQuantity',
				CASE WHEN @ConvertUnits = 1 THEN
							dbo.[udf_ConvertFromSIUnits](tl.NetQuantity,dbo.udf_ProductTypeFactor(tl.ProductType,s.AdditiveVolumeUnitIndex,s.AdditiveVolumeUnitIndex),dbo.udf_ProductTypeFactor(tl.ProductType,s.AdditiveVolumeDecimalPlaces,s.VolumeDecimalPlaces))
					 ELSE 
							tl.NetQuantity
				END AS 'NetQuantity',
				CASE WHEN @ConvertUnits = 1 THEN
							dbo.[udf_ConvertFromSIUnits](tl.Temperature,s.TemperatureUnitIndex,s.TemperatureDecimalPlaces) 							
					 ELSE 
							tl.Temperature
				END AS 'Temperature', 
				CASE WHEN @ConvertUnits = 1 THEN
							dbo.[udf_ConvertFromSIUnits](tl.Density,s.DensityUnitIndex,s.DensityDecimalPlaces) 							
					 ELSE 
							tl.Density
				END AS 'Density',  
                tl.MeterStop, 
                tl.MeterStart, 
                tl.VCF, 
                tl.Product,
                tl.OperatorID AS TranLineOperatorID, 
                tl.LoadingLocationID, 
                tl.LineNumber,  
                tn.Notes,
                t.LookupTransactionStatusIndex,
				t.ReversalType
        FROM  dbo.tblTransactions t
		LEFT JOIN dbo.tblTransactionLineItems tl
		ON t.TransactionGuid = tl.TransactionGuid
		LEFT JOIN dbo.tblTransactionUserData tud
		ON tl.TransactionGuid = tud.TransactionGuid
		LEFT JOIN dbo.tblTransactionNotes tn  
		ON t.TransactionGuid = tn.TransactionGuid
		LEFT JOIN dbo.tblSites s 
		ON t.SiteGuid = s.SiteGuid
        WHERE (@AliasNames is NULL OR t.AliasName in ( SELECT * FROM [dbo].[udf_SplitString] (@AliasNames,',',0)) ) AND  
              (@BOLExportServiceLastRowVersion IS NULL OR t._RowVersion > @BOLExportServiceLastRowVersion ) AND
              (@AllowedTransactionStatuses is NULL OR t.LookupTransactionStatusIndex in ( SELECT * FROM [dbo].[udf_SplitString] (@AllowedTransactionStatuses,',',0))) AND
			  (@ExportInterfaceName is NULL OR not exists(select * from [tblExportResultDetails] join [tblExportResults] on [tblExportResults].[ExportResultGuid] = [tblExportResultDetails].[ExportResultGuid] where [tblExportResults].[InterfaceName] = @ExportInterfaceName and [tblExportResultDetails].[RecordID] = t.TransID and [tblExportResultDetails].[Fail] = 0))
		ORDER BY t.TransID, LineNumber
END
GO


