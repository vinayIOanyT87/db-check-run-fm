USE [ConsolidatedDB]
GO

SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO

If exists (Select * From dbo.sysobjects Where id = object_id(N'dbo.[rpt_sp_ta_TrailerInspectionLicenseReport]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
Drop Procedure dbo.[rpt_sp_ta_TrailerInspectionLicenseReport]
GO


CREATE PROCEDURE [dbo].[rpt_sp_ta_TrailerInspectionLicenseReport]
 /*=============================================
 Author:	 			UNKNOWN
 Create date: 			
 Description:			Main Trailer Report	
 Subreports:			rpt_sp_ta_TrailerInspecLicenTInspecSub
						rpt_sp_ta_TrailerInspecLicenTLicSub
						
 Version:				7.5.1.1
 Execution:				
				
		execute rpt_sp_ta_TrailerInspectionLicenseReport 1,1,2,'<All>',90

 Modification History:
	Date		by		Description
	4/2/2010	KF		Migrate 
						Equipment Type changed
	6/25/2010	KF		Historical Records are now being used. Need to add to where clause
						qm.HistoricalRecord = 0
 =============================================*/
	@LoginSiteIndex int,
	@SiteIndex		int,
	@UserIndex		int,
	@Carrier		nvarchar(200) = NULL,
	@Expired		int

AS 
/************
	BEGIN
	HEADER
************/

CREATE TABLE #Header(
							CarrierName			nvarchar(200),
							Company				nvarchar(60),
							[Index]				int,
							CompanyTrailerID	nvarchar(60),
							TrailerID			nvarchar(60),
							LockedOut			nvarchar(3),
							LicenseName			nvarchar(160),
							[Type]				nvarchar(13),
							Number				nvarchar(100),
							LicenseExpiration	datetime,
							diff				nvarchar(6),
							Days				nvarchar(6)
					)

INSERT INTO #Header

					select	
							 case when c.[Name] = '' then '<Not Assigned>' 
													else c.[Name] end as CarrierName
							,c.ID as Company
							,e.[Index]
							,e.CompanyEquipmentID as CompanyTrailerID
							,e.ID as TrailerID
							,case when e.LockedOut = 0 then 'No' 
													else 'Yes' end as LockedOut
							,'' as LicenseName
							,'' as [Type]
							, 0 as Number
							,'' as LicenseExpiration
							,'' as diff
							,0  as Days


					from dbo.tblEquipment e with(nolock) 
								 join dbo.tblCompanies c with(nolock) On
												e.CompanyIndex = c.CompanyIndex
								 join dbo.tblQualificationsMap qm  with (nolock) on
												e.[Index] = qm.[Index]
								 join dbo.tblQualifications q  with (nolock) on 
												q.[Index] = qm.AssignedIndex

					where 	 	 (@Carrier = '<All>' OR c.[Name] = @Carrier)
							 and ((DATEDIFF(day, getdate(), qm.ExpirationDate) <= 30 and 
								   @Expired = 30) or
								  (DATEDIFF(day, getdate(), qm.ExpirationDate) >= 30 and 
								   DATEDIFF(day, getdate(), qm.ExpirationDate) <=60 and 
								   @Expired = 60) or
								  (DATEDIFF(day, getdate(), qm.ExpirationDate) >= 60 and 
								   DATEDIFF(day, getdate(), qm.ExpirationDate) <=90 and 
								   @Expired = 90)or
								  (@Expired = 0))
							 and e.SiteIndex = @SiteIndex
--							 and e.[Type] = 0  -- TRAILER_TYPE = 0
							 and qm.HistoricalRecord = 0



					Group By c.[Name],c.ID ,e.[Index],e.CompanyEquipmentID,e.ID, e.LockedOut
										
					Order By c.[Name],c.ID

/************
	END
	HEADER
************/

/************
	BEGIN
	Equipment'S WITH 
	NO Q OR L
	(only show when Expired=0
	 for <All>)
************/
CREATE TABLE #Equipment(
							CarrierName			nvarchar(200),
							Company				nvarchar(60),
							[Index]				int,
							CompanyTrailerID	nvarchar(60),
							TrailerID			nvarchar(60),
							LockedOut			nvarchar(3),
							LicenseName			nvarchar(160),
							[Type]				nvarchar(13),
							Number				nvarchar(100),
							LicenseExpiration	datetime,
							diff				nvarchar(6),
							Days				nvarchar(6)
					)

INSERT INTO #Equipment

					select	
							 case when c.[Name] = '' then '<Not Assigned>' 
													else c.[Name] end as CarrierName
							,c.ID as Company
							,e.[Index]
							,e.CompanyEquipmentID as CompanyTrailerID
							,e.ID as TrailerID
							,case when e.LockedOut = 0 then 'No' 
													else 'Yes' end as LockedOut
							,'' as LicenseName
							,'' as [Type]
							, 0 as Number
							,'' as LicenseExpiration
							,'' as diff
							,0  as Days


				from dbo.tblEquipment e with(nolock) 
								 join dbo.tblCompanies c with(nolock) On
												e.CompanyIndex = c.CompanyIndex
	
					where 	 
								 (@Carrier = '<All>' OR c.[Name] = @Carrier)
							 and e.SiteIndex = @SiteIndex
--							 and e.[Type] = 0  -- TRAILER_TYPE = 0
							 and e.[Index] not in(select [Index] from tblQualificationsMap)
							 and @Expired = 0 


					Group By c.[Name],c.ID , e.[Index],e.CompanyEquipmentID,e.ID, e.LockedOut
										
					Order By  c.[Name],c.ID



--/************
--	END
--	Equipment'S WITH 
--	NO Q OR L
--************/		

/************
	MAIN QUERY
************/

		select * from #Header
UNION ALL
		select * from #Equipment
						




GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO
 

GRANT EXECUTE ON dbo.[rpt_sp_ta_TrailerInspectionLicenseReport] TO [public]
GO
