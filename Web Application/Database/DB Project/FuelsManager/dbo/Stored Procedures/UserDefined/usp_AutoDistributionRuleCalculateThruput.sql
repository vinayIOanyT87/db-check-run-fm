
-- ==================================================================================================================
-- Author:		Daniel Or
-- Create date:	6/27/2012
-- Description:	Calculate throughput of a given rule, manager and product for the given dates
-- ==================================================================================================================
CREATE PROCEDURE [dbo].[usp_AutoDistributionRuleCalculateThruput] (
       @AutoDistributionRuleGuid UNIQUEIDENTIFIER,
       @SelectedSiteGuid UNIQUEIDENTIFIER,
       @LoginSiteGuid UNIQUEIDENTIFIER,
       @ManagerGuid UNIQUEIDENTIFIER,
       @ProductGuid UNIQUEIDENTIFIER,
       @StartDate DATETIMEOFFSET,
       @EndDate DATETIMEOFFSET,

       @VolumeConversionFactor FLOAT,
       @VolumeDecimalPlaces FLOAT,

       @MassConversionFactor FLOAT,
       @MassDecimalPlaces FLOAT
)
AS
BEGIN

       SELECT
              OwnerID,
              OwnerGuid,
              SUM(ISNULL(CASE WHEN IgnoreQuantity = 0 THEN ABS(ROUND(GrossQuantity * @VolumeConversionFactor, @VolumeDecimalPlaces)) ELSE 0 END,0)) AS GrossThruput,
              SUM(ISNULL(CASE WHEN IgnoreQuantity = 0 THEN ABS(ROUND(NetQuantity * @VolumeConversionFactor, @VolumeDecimalPlaces)) ELSE 0 END,0)) AS NetThruput,
              SUM(ISNULL(CASE WHEN IgnoreQuantity = 0 THEN ABS(ROUND(MassQuantity * @MassConversionFactor, @MassDecimalPlaces)) ELSE 0 END,0)) AS MassThruput
       FROM
       (      
              SELECT
                     ALLOWNER.CompanyID AS OwnerID,
                     ALLOWNER.CompanyGuid AS OwnerGuid,
                     /* 
                           -- For now the following types are not used
                           -- when used, they need to be uncommented and tested.
                           -- this also needed to be split into 3 for gross, net and mass
                           CASE WHEN 
                           TRX.TransTypeID IN (11, 23) --T11_ConsumerTransfer, T23_StorageTransfer
                           AND  ( 
                                  TRX.ReversalType IN (''R'', ''RU'') AND TRXLINE.GrossQuantity > 0
                                  OR
                                  TRX.ReversalType NOT IN (''R'', ''RU'') AND TRXLINE.GrossQuantity < 0
                           )  
                           THEN 1 ELSE 0
                     END */
                     0 AS IgnoreQuantity,
                     TRXLINE.GrossQuantity,
                     TRXLINE.NetQuantity,
                     TRXLINE.MassQuantity
              FROM   
                     [dbo].[vw_AutoDistributionRuleOwners] ALLOWNER                
                     
                     LEFT JOIN            /* LEFT JOIN to include all owners no matter what */
                     (
                           /* get all trx with the alias specified in the rule */
                           [dbo].[tblTransactions] TRX WITH (NOLOCK)
                           INNER JOIN [map].[tblTransactionAliasToAutoDistributionRule] ALIASMAP WITH (NOLOCK)
                           ON 
                                  ALIASMAP.TransactionAliasGuid = TRX.TransactionAliasGuid

                           /* get all the line items */
                           INNER JOIN [dbo].[tblTransactionLineItems] TRXLINE WITH (NOLOCK)
                           ON 
                                  TRX.TransactionGuid = TRXLINE.TransactionGuid 
                                  AND TRXLINE.DeleteFlag = 0
                                  AND ISNULL(TRXLINE.LookupQualityIndex, 1) = 1  /* 1 is usable */
                                  AND 
                                         (
                                             /* we will also include tracking product */
                                                TRXLINE.ProductGuid = @ProductGuid
                                                OR
                                                (
                                                       TRXLINE.ProductGuid IN (
                                                             SELECT _MasterRecordGuid
                                                              FROM [dbo].[tblProducts]
                                                              WHERE TrackingProductGuid = @ProductGuid
                                                       )
                                                       AND 
                                                       TRXLINE.ProductGuid IN ( 
                                                              select p._MasterRecordGuid from tblProducts p 
															  inner join [erv].[udf_GetProductRecordVersions](@SelectedSiteGuid) rp 
															  on p.ProductGuid = rp.ProductGuid
                                                       )                                        
                                                )
                                         )             
                     )
                     ON 
                           TRX.SiteGuid =       @SelectedSiteGuid
                           AND TRX.InventoryDate BETWEEN @StartDate AND @EndDate
                           AND TRX.ManagerCompanyGuid = @ManagerGuid
                           AND TRX.OwnerCompanyGuid = ALLOWNER.CompanyGuid
                           AND TRX.LookupTransactionStatusIndex <> 15 /* TRANSACTION_STATUS_SUSPENCE*/
                           AND ALLOWNER.AutoDistributionRuleGuid = ALIASMAP.AutoDistributionRuleGuid                             
              WHERE
                     ALLOWNER.AutoDistributionRuleGuid = @AutoDistributionRuleGuid
                           
       ) TMP
       GROUP BY
              OwnerGuid, OwnerID
       ORDER BY
              OwnerID
END