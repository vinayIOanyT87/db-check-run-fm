USE ConsolidatedDB
GO
SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO

If exists (Select * From dbo.sysobjects Where id = object_id(N'dbo.rpt_sp_ta_ShipmentReport') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
Drop Procedure dbo.rpt_sp_ta_ShipmentReport
GO



CREATE PROCEDURE dbo.[rpt_sp_ta_ShipmentReport]

 /*=============================================
 Author:	 			UNKNOWN
 Create date: 			
 Description: 	
 Version:				7.5.1.3
 Execution:
				Execute rpt_sp_ta_ShipmentReport 1,1,2,'12/1/2009','12/7/2009'	
	
 Modification History:
	Date		by		Description
	4/29/09		UP		Rename from [fm_ShipmentVolumesReport] to [rpt_sp_ta_ShipmentVolumesReport]
	8/5/09		KF		Rename from rpt_sp_ta_ShipmentVolumesReport to rpt_sp_ta_ShipmentReport due to 
						Report Title.
	12/10/2009	KF		Version 7.5.1.0
	1/21/2009	KF		DeleteFlag = cast(0 as bit) add to where clause.
	3/8/2010	KF		AuthorizedCompanies replaced by rpt_fn_ta_AuthorizedCompanies
	4/15/2010	KF		Removed Abs from Net field. Need to show positive and negative amounts.
	
 =============================================*/
	@SiteIndex int,
	@LoginSiteIndex int,
	@UserIndex int,
	@StartDate datetime,
	@EndDate datetime


AS BEGIN

DECLARE @VolumeFactor float
SELECT @VolumeFactor = dbo.ConvertFromSIUnits (1,(SELECT VolumeUnitIndex FROM dbo.tblSites with (NoLock) WHERE SiteIndex = @SiteIndex),9)

DECLARE @VolumeDecimal int
SELECT @VolumeDecimal = (SELECT VolumeDecimalPlaces FROM dbo.tblSites with (NoLock) WHERE SiteIndex = @SiteIndex)

DECLARE @AuthorizedCompanies TABLE
(
	Company nvarchar (30)
)

INSERT INTO @AuthorizedCompanies SELECT * FROM rpt_fn_ta_AuthorizedCompanies(@LoginSiteIndex,@SiteIndex,@UserIndex)

SELECT
	B.ProductCode,
	A.DestinationCompanyEquipmentID1,
	A.DocumentNumber,
	A.TimeEnd,
	A.OwnerID,
	B.StorageLocationID,
	ROUND((B.NetQuantity * @VolumeFactor), @VolumeDecimal)AS Net

FROM 
	tblTransactions A with (NoLock)
		LEFT OUTER JOIN tblTransactionLineItems B with (NoLock) on A.TransID = B.TransID

WHERE 
	A.SiteIndex = @SiteIndex
	AND ( A.InventoryDate >= @StartDate AND A.InventoryDate <= @EndDate )
	AND A.AliasName = 'Shipment'
	AND EXISTS (SELECT Company
			FROM @AuthorizedCompanies
			WHERE Company IN (A.ShipToID, A.SupplierID, A.ShipperID, A.OwnerID, A.ManagerID, A.CarrierID, A.BillToID))
	AND A.DeleteFlag = cast(0 as bit)
	AND B.DeleteFlag = cast(0 as bit)

ORDER BY
	B.ProductCode,
	A.TimeEnd

END


GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO
GRANT EXECUTE ON dbo.rpt_sp_ta_ShipmentReport TO [public]
GO
