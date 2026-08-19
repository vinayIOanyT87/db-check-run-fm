USE [ConsolidatedDB]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
If exists (Select * From dbo.sysobjects Where id = object_id(N'dbo.[rpt_sp_ta_AdditiveProfile]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
Drop Procedure dbo.[rpt_sp_ta_AdditiveProfile]
GO


CREATE procedure [dbo].[rpt_sp_ta_AdditiveProfile]
 /*=============================================
 Author:	 			URVI PATEL
 Create date: 			
 Description: 			New report
 Version:				7.5.1.1
 Execution:			
		EXEC [rpt_sp_ta_AdditiveProfile] 1,1,2

 Modification History:
	Date		by		Description
	12/9/2009	KF		Version 7.5.1.0	
	3/4/2010	KF		Table change from tblapplicationstring
						to tblAdditiveProfiles
 =============================================*/

( 
	@LoginSiteIndex int,
	@SiteIndex int,
	@UserIndex int
	
)

AS 
BEGIN
DECLARE @AdditiveProfileRateUnit int
SET  @AdditiveProfileRateUnit = (SELECT AdditiveProfileRateUnitIndex FROM tblSites with(nolock) WHERE tblSites.SiteIndex = @LoginSiteIndex)

DECLARE @AdditiveProfileRateDecimalPlaces int
SET @AdditiveProfileRateDecimalPlaces = (SELECT AdditiveProfileRateDecimalPlaces FROM tblSites with(nolock) WHERE tblSites.SiteIndex = @LoginSiteIndex)


DECLARE @AdditiveProfileCycleAmountUnit int
SET @AdditiveProfileCycleAmountUnit = (SELECT AdditiveProfileCycleAmountUnitIndex FROM tblSites with(nolock) WHERE tblSites.SiteIndex = @LoginSiteIndex)

DECLARE  @AdditiveProfileCycleAmountDecimalPlaces int
SET @AdditiveProfileCycleAmountDecimalPlaces = (SELECT AdditiveProfileCycleAmountDecimalPlaces FROM tblSites with(nolock) WHERE tblSites.SiteIndex = @LoginSiteIndex)


SELECT 
	 A.ID 
	,p.ProductID
	,IsNull(1*dbo.ConvertFromSIUnits(PM.AdditiveRate,@AdditiveProfileRateUnit,@AdditiveProfileRateDecimalPlaces),0.0) as 'Rate'
	,IsNull(1*dbo.ConvertFromSIUnits(AdditiveCycleVolume,@AdditiveProfileCycleAmountUnit,@AdditiveProfileCycleAmountDecimalPlaces),0.0) as 'CycleVolume'
	,PM.Tolerance

 
FROM dbo.tblAdditiveProfiles A with (nolock)
	LEFT OUTER JOIN dbo.tblProductMap PM with (nolock) ON 
						A.[Index] = PM.AssignedToIndex
	LEFT OUTER JOIN dbo.tblProducts P with (nolock)	ON 
						PM.AssignedIndex = P.ProductIndex
WHERE 
	PM.Type = 5

ORDER BY [ID]

END









GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO
GRANT EXECUTE ON dbo.[rpt_sp_ta_AdditiveProfile] TO [public]
GO
