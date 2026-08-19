USE ConsolidatedDB

GO
SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO

If exists (Select * From dbo.sysobjects Where id = object_id(N'dbo.[rpt_sp_ta_12HrAdditiveReport]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
Drop Procedure dbo.[rpt_sp_ta_12HrAdditiveReport]
GO

CREATE PROCEDURE [dbo].[rpt_sp_ta_12HrAdditiveReport]
 /*=============================================
 Author:				URVI PATEL
 Create date:			5/28/2009
 Description:			Additive System- 12 hr Additive Report
 Version:				7.5.3.0
 Execution:
						Execute rpt_sp_ta_12HrAdditiveReport 1,1,2,'February 2010','<All>'

 Modification History:
	Date		by		Description
	08/26/2009	kf		Correct the sp file name to correct spelling of Additive
	08/28/2009	KF		Changed [Gross Gallons] formula to include reversal and updated transactions
	3/8/2010	KF		AuthorizedCompanies replaced by rpt_fn_ta_AuthorizedCompanies
	11/20/2013	JLSII	Modified SP to use tblTransactions.TransTypeID = '5' instead of hardcoded
						tblTransactions.AliasName = 'BOL'
 =============================================*/

	@SiteIndex int,	
	@LoginSiteIndex int,
	@UserIndex int,
	@Month nvarchar(20),
	@Product nvarchar(30)


AS
BEGIN

DECLARE @AdditiveVolumeUnits int
SET @AdditiveVolumeUnits = (SELECT AdditiveVolumeUnitIndex FROM tblSites with(nolock) WHERE tblSites.SiteIndex = @SiteIndex)

DECLARE @AdditiveVolumeDecimalPlaces int
SELECT @AdditiveVolumeDecimalPlaces = (SELECT AdditiveVolumeDecimalPlaces FROM dbo.tblSites with(nolock) WHERE SiteIndex = @SiteIndex)

DECLARE @AuthorizedCompanies TABLE (CompanyID nvarchar(30))
INSERT INTO @AuthorizedCompanies SELECT ID FROM rpt_fn_ta_AuthorizedCompanies(@LoginSiteIndex, @SiteIndex, @UserIndex)

DECLARE @BeginDate datetime
DECLARE  @EndDate datetime

IF @Product = '<ALL>' SET @Product = NULL

IF @Month IS NOT NUll --OR @Month<>''
	BEGIN
		SET @BeginDate = Convert(datetime,@Month)
		SET @EndDate = DateAdd(s,-1,DateAdd(mm,1,@BeginDate)) --last day of the month
	END	

CREATE TABLE #TXLEDList(
							--TransactionDate datetime,
							FromDate date,
							FromDateTime nvarchar(8),
							ToDate date,
							ToDateTime nvarchar(8),
							[AdditiveProductCode] nvarchar(40),
							[AdditiveSystem] nvarchar(40),
							SubLineProduct nvarchar(60),
							[ProductGallons] float default 0.000,
							[ActualAdditive] float default 0.000,
							[AdditiveTreatRate] decimal(15,10),
						--	ProductType nvarchar(40),
						--	Description nvarchar(50)
							
				)

INSERT INTO #TXLEDList

SELECT FromDate, FromDateTime, ToDate, ToDateTime, [AdditiveProductCode], [AdditiveSystem], SubLineProduct, SUM(ProductGallons), SUM(ActualAdditive), [AdditiveTreatRate]
FROM 
		(
				SELECT 
					FromDate = CONVERT(date,dbo.GetLocalTime(@SiteIndex,TransDateTime),101)
					,FromDateTime= CASE
							WHEN DATEPART(hh,dbo.GetLocalTime(@SiteIndex,TransDateTime) ) < 12
							THEN '00:00:00'
							ELSE '12:00:00'
							END
					 ,ToDate = CASE 
							WHEN DATEPART(hh,dbo.GetLocalTime(@SiteIndex,TransDateTime)) >= 12
							THEN CONVERT(date,dbo.GetLocalTime(@SiteIndex,TransDateTime)+1,101)
							ELSE CONVERT(date,dbo.GetLocalTime(@SiteIndex,TransDateTime),101)
							END
					,ToDateTime = CASE
							WHEN DATEPART(hh,dbo.GetLocalTime(@SiteIndex,TransDateTime)) >= 12
							THEN '00:00:00'
							ELSE '12:00:00'
							END
					,p.productID AS [AdditiveProductCode]
					,p.productcode as [AdditiveSystem]
					,s.Product as SubLineProduct			
					,IsNull(-1*dbo.ConvertFromSIUnits(l.GrossQuantity,@AdditiveVolumeUnits,@AdditiveVolumeDecimalPlaces),0.0) as  [ProductGallons]
--					,ROUND(ABS(l.GrossQuantity * dbo.ConvertFromSIUnits (1,(SELECT VolumeUnitIndex FROM dbo.tblSites with(nolock) WHERE SiteIndex = @SiteIndex),9)), (SELECT VolumeDecimalPlaces FROM dbo.tblSites with(nolock) WHERE SiteIndex = @SiteIndex)) as [ProductGallons]  --***Removed 8/28/2009 kf
					,IsNull(-1*dbo.ConvertFromSIUnits(s.GrossQuantity,@AdditiveVolumeUnits,@AdditiveVolumeDecimalPlaces),0.0) as [ActualAdditive]
					,0 as [AdditiveTreatRate]
					,p.producttype
					,p.description
				From	
				(dbo.tblTransactions t with(nolock) 
						left join dbo.tblTransactionLineItems l with(nolock) on 
										t.TransID = l.TransID)
						left join dbo.tblTransactionSubLineItems s with(nolock) on 
										l.TransLineItemID = s.TransLineItemID
						left join dbo.tblProducts p with(nolock) on
										s.ProductIndex = p.ProductIndex
				WHERE s.ProductType = 'Additive' 
				AND  t.TransTypeID = '5'
				AND l.DeleteFlag = cast(0 as bit)
				AND	 s.DeleteFlag = cast(0 as bit)
				AND	 t.DeleteFlag = cast(0 as bit)
				AND (dbo.GetLocalTime(@SiteIndex,TransDateTime)between @BeginDate AND @EndDate)
				AND (s.Product=  isnull(@Product,s.Product))
				AND	 EXISTS (SELECT CompanyID 
								FROM @AuthorizedCompanies 
									WHERE CompanyID IN (t.CarrierID, t.ShipperID, t.ShipToID, t.SupplierID, t.ManagerID, t.OwnerID, t.BillToID)) 
			)AdditiveTotal
--WHERE (FromDate between @BeginDate AND @EndDate)
--				AND (SubLineProduct = isnull(@Product,SubLineProduct))--  or SubLineProduct = @Product)

GROUP BY FromDate, FromDateTime, ToDate, ToDateTime, [AdditiveProductCode], [AdditiveSystem], SubLineProduct, [AdditiveTreatRate]

order by [AdditiveSystem],[AdditiveProductCode],FromDate, ToDate


UPDATE #TXLEDList
SET [AdditiveTreatRate] = CASE WHEN [ProductGallons] = 0 THEN 0
								ELSE ([ActualAdditive]/[ProductGallons])*1000 END
SELECT * FROM #TXLEDList

END
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO
 

GRANT EXECUTE ON dbo.[rpt_sp_ta_12HrAdditiveReport] TO [public]
GO
