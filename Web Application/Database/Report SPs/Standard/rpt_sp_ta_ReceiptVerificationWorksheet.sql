USE [ConsolidatedDB]
GO

SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO

If exists (Select * From dbo.sysobjects Where id = object_id(N'dbo.rpt_sp_ta_ReceiptVerificationWorksheet') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
Drop Procedure dbo.rpt_sp_ta_ReceiptVerificationWorksheet
GO


CREATE PROCEDURE dbo.rpt_sp_ta_ReceiptVerificationWorksheet

/*********************************************************************
 Author:		Urvi Patel
 Create date:	6/23/2009
 Description: 
 Version:		7.5.1.2
 Execution: 
	EXEC rpt_sp_ta_ReceiptVerificationWorksheet 1,1,2,'12/1/2009 00:00 ','12/7/2009 10:00',26837,'3112- CITGO Petroleum Corp', 100, 2000

	
 Modification History:
	Date			By		Description
	6/23/2009		UP		NEW
	12/10/2009		KF		Version 7.5.1.0
	2/9/2010		KF		Version changed due to change in report.
	3/8/2010		KF		AuthorizedCompanies replaced by rpt_fn_ta_AuthorizedCompanies
	
********************************************************************************/

(	@LoginSiteIndex int,
	@SiteIndex int,
	@UserIndex int,
	@FromDate datetime,
	@ToDate datetime,
	@Document nvarchar(30),
	@Manager nvarchar(50),
	@StartTankVol float,
	@StopTankVol float
)

AS

IF  @Manager = '<ALL>' SET @Manager = NULL

DECLARE @AuthorizedCompanies TABLE (CompanyID nvarchar(30))
INSERT INTO @AuthorizedCompanies SELECT ID FROM rpt_fn_ta_AuthorizedCompanies(@LoginSiteIndex, @SiteIndex, @UserIndex)

DECLARE @VolumeUnits int
SET @VolumeUnits = (SELECT VolumeUnitIndex FROM tblSites with(nolock) WHERE tblSites.SiteIndex = @LoginSiteIndex)

DECLARE @VolumeDecimalPlaces int
SET @VolumeDecimalPlaces = (SELECT VolumeDecimalPlaces FROM tblSites with(nolock) WHERE tblSites.SiteIndex = @LoginSiteIndex)


SELECT dbo.GetLocalTime(@SiteIndex,t.TransDateTime) AS TransDateTime
		,t.documentnumber
		,t.aliasname
		,l.product
		,l.netquantity
		,ReceivedVol = IsNull(dbo.ConvertFromSIUnits(l.NetQuantity,@VolumeUnits,@VolumeDecimalPlaces),0.0)
		,StartVol = @StartTankVol--  case when u.UserData2 is null then 0 else u.UserData2 end
		,StopVol = @StopTankVol ----case when u.UserData3 is null then 0 else u.UserData3 end
		,l.storagelocationindex
		,OwnerID
		,ManagerID
	INTO #tmpReceipt
	from tbltransactions t
	JOIN tbltransactionlineitems l 
		ON t.transid = l.transid

WHERE (dbo.GetLocalTime(@SiteIndex,TransDateTime)>=@FromDate AND dbo.GetLocalTime(@SiteIndex,TransDateTime)<=@ToDate) 
			AND	t.documentnumber = @Document
			AND t.ManagerID = ISNULL(@Manager, ManagerID)
			AND aliasname = 'RECEIPT'  
			AND EXISTS (SELECT CompanyID 
								FROM @AuthorizedCompanies 
								WHERE CompanyID IN (T.CarrierID, T.ShipperID, T.ShipToID, T.SupplierID, T.ManagerID, T.OwnerID, T.BillToID)) 
			AND T.DeleteFlag = cast(0 as bit)
			AND L.DeleteFlag = cast(0 as bit)
			 
	
SELECT  TransDateTime
		,documentnumber
		,product
		,sum(ReceivedVol) AS ReceivedVol
		,StartVol 
		,StopVol 
		,storagelocationindex
		,OwnerID
		,ManagerID
INTO #tmpSum
FROM #tmpReceipt
GROUP BY  TransDateTime	,documentnumber	,product,StartVol ,StopVol ,storagelocationindex,OwnerID,ManagerID


--select * from #tmpSum

SELECT	r.TransDateTime
		,r.documentnumber
		,r.product
		,ReceivedVol
		,r.StartVol 
		,r.StopVol 
		,IssuedVolume = sum(IsNull(-1*dbo.ConvertFromSIUnits(l.NetQuantity,@VolumeUnits,@VolumeDecimalPlaces),0.0) )
		,r.storagelocationIndex AS ReceiptTank
		,l.storagelocationIndex AS IssuedTank
INTO #Master
FROM tbltransactions t
JOIN tbltransactionlineitems l 
	ON t.transid = l.transid
JOIN #tmpSum r
ON r.storagelocationindex = l.storagelocationindex
AND r.ManagerID = t.ManagerID
WHERE (dbo.GetLocalTime(@SiteIndex,MeterStartDateTime) >= @FromDate and dbo.GetLocalTime(@SiteIndex,MeterStartDateTime) <= @ToDate)
AND Aliasname = 'BOL'
AND t.ManagerID = ISNULL(@Manager, t.ManagerID)
GROUP BY l.product,l.storagelocationindex,r.TransDateTime	,r.documentnumber,ReceivedVol	,r.product,r.StartVol ,r.StopVol,r.storagelocationIndex,l.storagelocationIndex

--SELECT * FROM #Master

SELECT	TransDateTime
		,documentnumber
		,product
		,ReceivedVol
		,StartVol 
		,StopVol 
		,IssuedVolume 
		,CalculatedVol = (StopVol - StartVol + IssuedVolume)
		,CalDifference = ReceivedVol - (StopVol - StartVol + IssuedVolume)
		,ReceiptTank
		,IssuedTank
FROM #Master

DROP TABLE #tmpReceipt
DROP TABLE #tmpSum
DROP TABLE #Master

GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO
 

GRANT EXECUTE ON dbo.rpt_sp_ta_ReceiptVerificationWorksheet TO [public]
GO
