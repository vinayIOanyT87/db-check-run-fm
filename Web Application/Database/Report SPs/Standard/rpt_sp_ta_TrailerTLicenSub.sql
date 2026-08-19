USE [ConsolidatedDB]
GO

SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO

If exists (Select * From dbo.sysobjects Where id = object_id(N'dbo.[rpt_sp_ta_TrailerInspecLicenTLicSub]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
Drop Procedure dbo.[rpt_sp_ta_TrailerInspecLicenTLicSub]
GO


CREATE PROCEDURE [dbo].[rpt_sp_ta_TrailerInspecLicenTLicSub]
 /*=============================================
 Author:	 			UNKNOWN
 Create date: 			
 Description:			Test & Inspections Subreport to Trailer Inspections and License Report
 Main Report:			rpt_sp_ta_TrailerInspectionLicenseReport
						
 Version:				7.5.1.1
 Execution:				
				execute rpt_sp_ta_TrailerInspecLicenTLicSub 1,1,2,'530',90
 Modification History:
	Date		by		Description
	4/2/2010	KF		Migrate 
	4/21/2009	KF		Equipment.Type no longer exist. Join Equipment on tblEquipmentTypes. 
						Use tblEquipmentTypes.EqTypeIndex = 1 Trailer
	6/25/2010	KF		Historical Records are now being used. Need to add to where clause
						qm.HistoricalRecord = 0
 =============================================*/
	@LoginSiteIndex int,
	@SiteIndex		int,
	@UserIndex		int,
	@EquipIndex		int,
	@Expired		int

AS 
/************
	BEGIN
	Tags & Licenses
************/

CREATE TABLE #TLicen(
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

INSERT INTO #TLicen

					select	
							 case when c.[Name] = '' then '<Not Assigned>' 
													else c.[Name] end as CarrierName
							,c.ID as Company
							,e.[Index]
							,e.CompanyEquipmentID as CompanyTrailerID
							,e.ID as TrailerID
							,case when e.LockedOut = 0 then 'No' 
													else 'Yes' end as LockedOut
							,case when q.ID = '' then 'None' 
														else q.ID end as LicenseName
							,Case when qm.type = 3 then 'Qualifications' 
														else 'License' end as [Type]
							,qm.ID as Number
							,convert(char(10),qm.ExpirationDate,110) as LicenseExpiration
							,DATEDIFF(day, getdate(), qm.ExpirationDate) as diff
							,case when DATEDIFF(day, getdate(), qm.ExpirationDate) <= 30    then 30 
								  when DATEDIFF(day, getdate(), qm.ExpirationDate) >= 30 and
									   DATEDIFF(day, getdate(), qm.ExpirationDate) <=60     then 60 
								  when DATEDIFF(day, getdate(), qm.ExpirationDate) >= 60 and
									   DATEDIFF(day, getdate(), qm.ExpirationDate) <=90     then 90 
														else 0 end as Days


				from dbo.tblEquipment e with(nolock)
								 Left Join dbo.tblEquipmentTypes et with(nolock) on
												e.EqTypeIndex = et.EqTypeIndex 
								 join dbo.tblCompanies c with(nolock) On
												e.CompanyIndex = c.CompanyIndex
								 join dbo.tblQualificationsMap qm  with (nolock) on
												e.[Index] = qm.[Index]
								 join dbo.tblQualifications q  with (nolock) on 
												q.[Index] = qm.AssignedIndex

					where 	 	  (@EquipIndex = e.[Index])
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
							 and et.EqTypeIndex = 1 -- TRAILER_TYPE = 1
							 and qm.[Type] = 2
							 and qm.HistoricalRecord = 0


				Order By c.[Name],c.ID

/************
	END
	Tags & Licenses
************/



/************
	MAIN QUERY
************/

		select * from #TLicen





GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO
 

GRANT EXECUTE ON dbo.[rpt_sp_ta_TrailerInspecLicenTLicSub] TO [public]
GO
