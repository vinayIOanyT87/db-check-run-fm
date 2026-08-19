USE ConsolidatedDB

GO
SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO

If exists (Select * From dbo.sysobjects Where id = object_id(N'dbo.rpt_sp_ta_DriverActivityReport') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
Drop Procedure dbo.rpt_sp_ta_DriverActivityReport
GO


CREATE PROCEDURE dbo.rpt_sp_ta_DriverActivityReport
 /*=================================================================================================
 Author:				Kimberly Foote
 Create date:			12/29/2008	
 Version:				7.5.1.2
 Description:			Driver Detail Report
 Execution:				
		EXEC rpt_sp_ta_DriverActivityReport '12/1/2009','12/7/2009', 1, 1, 2, '<ALL>', '<ALL>','3122 - CITGO Petroleum Corp', '<ALL>','BOl'

 Modification History:
 
 Date		by		Description
 1/27/09	KF		Changed @Driver and @Carrier to 'All' or single
					selection.
					Add Company Header Info
 1/28/09	KF		add columns and changed dates 
					[Card In DET] = TimeIn
					[Load Start] = FST
					[Load End] =TimeEnd
					[BOL Printed] = TimeOut
 1/30/09	KF		changed [Diff] = abs(DATEDIFF(minute,[BOL Printed], [Load Start])) to
					[Diff] = abs(DATEDIFF(minute,[Load End], [Load Start]))
 2/10/09	KF		Add @Owner using as Stockholder in report.
 2/19/09	KF		Add columns for Percentages, Time At Rack, Time In Terminal.
 3/25/09	KF		Add @AuthorizedCompanies to create Userdefine functionality. 
 5/6/09		KF		Removed function @UtcBeginDate for the @BeginDate and @EndDate. 
					Replaced with (dbo.GetLocalTime(@SiteIndex,TransDateTime)>=@BeginDate AND 
					dbo.GetLocalTime(@SiteIndex,TransDateTime)<=@EndDate). This enables the report 
					to pull date and time.
6/12/2009	UP		Rename from rptTA_st_sp_Driver_Activity_Report to rpt_sp_ta_DriverActivityReport
8/10/2009	KF		Set @BeginDate to 12:00am and @EndDate to 12:00 midnight
					Parameters where to large @Carrier, @Driver, @Owner
8/13/2009	KF		Add @AliasName so that the hardcoded "BOL" can be a parameter to select which
					AliasName to use per site.
8/19/2009	KF		Fixed daterange. Had @EndData in both places for Time set.
12/9/2009	KF		Version 7.5.1.0
2/9/2010	KF		Version change due to change in report.
3/8/2010	KF		AuthorizedCompanies replaced by rpt_fn_ta_AuthorizedCompanies
 =================================================================================================*/


@BeginDate datetime,
@EndDate datetime,
@LoginSiteIndex int,
@SiteIndex int,
@UserIndex int,
@Carrier nvarchar(200) = NULL,
@Driver nvarchar(100) = NULL,
@Manager nvarchar(30),
@Owner nvarchar(60),
@AliasName nvarchar(64)


AS

Set @BeginDate  = convert(char(10),@BeginDate,110) + ' 00:00:00'
Set @EndDate  = convert(char(10),@EndDate,110) + ' 23:59:59'


DECLARE @VolumeFactor float
SELECT @VolumeFactor = 0.003785412

DECLARE @MassFactor float
SELECT @MassFactor = 2.20462262  -- 1kg = 2.20462262lb

DECLARE @AdditiveVolumeFactor float
SELECT @AdditiveVolumeFactor = 1000000 --1 cubic meter = 1000000 cubic cm

DECLARE @VolumeUnits int
SET @VolumeUnits = (SELECT tblSites.VolumeUnitIndex FROM tblSites with (nolock) WHERE tblSites.SiteIndex = @SiteIndex)

DECLARE @VolumeDecimalPlaces int
SET @VolumeDecimalPlaces = (SELECT tblSites.VolumeDecimalPlaces FROM tblSites with (nolock) WHERE tblSites.SiteIndex = @SiteIndex)

-- Get the Authorized Companies
DECLARE @AuthorizedCompanies TABLE (CompanyID nvarchar(30))
INSERT INTO @AuthorizedCompanies SELECT ID FROM rpt_fn_ta_AuthorizedCompanies(@LoginSiteIndex, @SiteIndex, @UserIndex)

SELECT	ID AS Site,
		SiteIndex
INTO	#Site
FROM	tblSites with (nolock),tblSiteToSiteMap  with (nolock)
WHERE	ParentSiteIndex = @SiteIndex 
		AND ChildSiteIndex = tblSites.SiteIndex --???
		AND SiteGroupFlag = 0

/******************

Sum Temp Table

********************/

Select
	 t.TransID
	,t.documentnumber
	,[Card In DET] = dbo.GetLocalTime(@SiteIndex,TimeIn)
	,[Load Start] = CASE WHEN FST IS NULL 
						THEN dbo.GetLocalTime(@SiteIndex,TimeIn)
							ELSE dbo.GetLocalTime(@SiteIndex,FST)
															END 
	,[Load End]  = dbo.GetLocalTime(@SiteIndex,TimeEnd)
	,[BOL Printed] = dbo.GetLocalTime(@SiteIndex,TimeOut)
	,ROUND(ABS(GrossQuantity * dbo.ConvertFromSIUnits (1,(SELECT VolumeUnitIndex FROM dbo.tblSites with (nolock) WHERE SiteIndex = @SiteIndex),9)), (SELECT VolumeDecimalPlaces FROM dbo.tblSites with (nolock) WHERE SiteIndex = @SiteIndex)) as [Gallons Loaded]

	

INTO #Temp

From   tblTransactions t with (nolock) LEFT OUTER JOIN tblTransactionLineItems l with (nolock) ON t.TransID = l.TransID

Where 
 (dbo.GetLocalTime(@SiteIndex,TransDateTime)>=@BeginDate AND 
  dbo.GetLocalTime(@SiteIndex,TransDateTime)<=@EndDate) 
and EXISTS (SELECT CompanyID 
            FROM @AuthorizedCompanies 
	        WHERE CompanyID IN (t.CarrierID, t.ShipperID, t.ShipToID, t.SupplierID, t.ManagerID, t.OwnerID, t.BillToID)) 		
and t.DeleteFlag = cast(0 as bit) 
and t.AliasName = @AliasName 

order by t.documentnumber

select 
 TransId
,[BOL Number] = documentnumber
,[Gallons Loaded] = sum([Gallons Loaded])
,[Card In DET]
,[Load Start]
,[Load End]
,[BOL Printed] 
,[Diff] = abs(DATEDIFF(minute,[Load End], [Load Start]))
,[Time In Terminal] = abs(DATEDIFF(minute,[BOL Printed], [Card In DET]))
,[Document] =count( distinct documentnumber)

INTO #Sums
from #Temp

Group by documentnumber,[Load Start],[BOL Printed],TransId,[Card In DET],[Load End]
Order by documentnumber

/******************
Sum Temp Table END
********************/

/*******************
	Main Table
**********************/


	CREATE TABLE #DriverInfo  --12
	 (
		[ManagerCompanyName] nvarchar (60),
		[Card In DET]		 datetime,
		[Load Start]		 datetime ,
		[Load End]			 datetime,
		[BOL Printed]		 datetime,
		[BOLNumber]		     nvarchar (60),
		[Gallons Loaded]	 int	  ,
		[CarrierId]			 int,
		[Carrier]			 nvarchar (60),
		[DriverNumber]		 nvarchar (100),
		[DriverName]		 nvarchar (50),
		[Owner]				 nvarchar(60),
		[TransDateTime]		 datetime,
		[Transid]			 nvarchar(128),
		[Diff]				 int,
		[Time In Terminal]	 int,
		[Document]			 int
	) 


	INSERT INTO  #DriverInfo
	SELECT	
			 ManagerID
			,sm.[Card In DET]		
			,sm.[Load Start]		
			,sm.[Load End]			
			,sm.[BOL Printed]		 
			,t.documentnumber as [BOLNumber]
			,sm.[Gallons Loaded] as [Gallons Loaded]
			,t.CarrierIndex as [CarrierId]
			,case when c.[Name] = '' 
					then '<Not Assigned>' else c.[Name] 
							end as [Carrier]
			,t.OperatorIndex as [DriverNumber]
			,p.PersonId as [DriverName]
			,t.OwnerID as [Owner]
			,dbo.GetLocalTime(@SiteIndex,T.TransDateTime) as TransDateTime 
			,t.Transid
			,sm.[Diff] as [Diff]
			,sm.[Time In Terminal] as [Time In Terminal]
			,sm.[Document] as [Document]
		
			
	FROM   tblTransactions t with (nolock)
     Left Outer Join tblTransactionLineItems l  with (nolock) on
						t.TransID = l.TransID
				Join tblPersonnel p  with (nolock) on
						p.PersonIndex =  t.OperatorIndex
				Join tblCompanies c  with (nolock) on
						c.CompanyIndex = t.CarrierIndex
				Join #Sums Sm on
						t.TransID = sm.TransID
	Where 	
 (dbo.GetLocalTime(@SiteIndex,TransDateTime)>=@BeginDate AND 
  dbo.GetLocalTime(@SiteIndex,TransDateTime)<=@EndDate) 
		and Site in (
						SELECT 	[ID] 
						FROM 	tblSites S  with (nolock), 
							tblSiteToSiteMap M  with (nolock)
						WHERE 	M.ParentSiteIndex = @SiteIndex 
							AND M.ChildSiteIndex = S.SiteIndex
						) 
		and EXISTS (SELECT CompanyID 
            FROM @AuthorizedCompanies 
	        WHERE CompanyID IN (t.CarrierID, t.ShipperID, t.ShipToID, t.SupplierID, t.ManagerID, t.OwnerID, t.BillToID)) 		
		and t.DeleteFlag = cast(0 as bit) 

		and t.SiteIndex IN(SELECT SiteIndex FROM #Site)
		and (@Driver = '<All>' OR (p.PersonId = @Driver))
		and (@Carrier = '<All>' OR (c.[Name] = @Carrier))
		and (@Owner = '<All>' OR (t.OwnerID = @Owner))
		and t.SiteIndex IN (SELECT SiteIndex FROM #Site)
		and t.DeleteFlag = cast(0 as bit) 
		and t.AliasName = @AliasName 

   Group by 
			 ManagerID
			,t.OwnerID
			,c.[Name]
			,PersonId
			,t.documentnumber
			,sm.[Card In DET]		
			,sm.[Load Start]		
			,sm.[Load End]			
			,sm.[BOL Printed]	
			,sm.[Gallons Loaded] 
			,t.CarrierIndex 
			,t.OperatorIndex 
			,T.TransDateTime
			,t.Transid
			,sm.[Diff]
			,sm.[Time In Terminal]
			,sm.[Document]


   order by t.DocumentNumber

/*****
Master Query

******/

SELECT 
			  d.*
			, @VolumeDecimalPlaces as DecimalPlaces
			,case when [Diff] >= 10 then 1 else 0 end as [TAR>10]
			,case when [Diff] >= 20 then 1 else 0 end as [TAR>20]
			,case when [Diff] >= 30 then 1 else 0 end as [TAR>30]
			,case when [Time In Terminal] >= 45 then 1 else 0 end as [TIT>45]
			,case when [Time In Terminal] >= 60 then 1 else 0 end as [TIT>60]
			,case when [Time In Terminal] >= 90 then 1 else 0 end as [TIT>90]
			,case when [Time In Terminal] < 45 then 1 else 0 end as [TIT<45]
			,case when [Time In Terminal] < 60 then 1 else 0 end as [TIT<60]
			,case when [Time In Terminal] < 90 then 1 else 0 end as [TIT<90]

			
FROM [dbo].[#DriverInfo] d


DELETE FROM  [dbo].[#DriverInfo] 

  

--drop table #DriverInfo
--drop table #Site
--drop table #temp
--drop table #Sums




/****TEST AREA

declare	@BeginDate datetime
declare	@EndDate datetime
declare @LoginSiteIndex int
declare	@SiteIndex int
declare @UserIndex int
declare @Carrier nvarchar(4000)
declare @Driver nvarchar(4000)
declare	@Manager nvarchar(30)

set	@BeginDate = '11/1/2008'
set	@EndDate = '11/1/2008'
set @LoginSiteIndex = 1
set	@SiteIndex = 1 
set @UserIndex = 2
--set @Carrier = 'DEAD RIVER CO/TRANSPORT (ME)'
set @Carrier = '<All>'
set @Driver = '<All>'
--set @Driver = 'MARK CRANDELL'
set	@Manager = '3122 - CITGO Petroleum Corp'

	Select t.operatorindex,* from tbltransactions t  where t.operatorindex = '1505AARON FREY' order by t.operatorindex

***/


GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO
 

GRANT EXECUTE ON dbo.rpt_sp_ta_DriverActivityReport TO [public]
GO