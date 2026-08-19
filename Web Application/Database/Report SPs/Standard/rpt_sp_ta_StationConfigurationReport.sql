USE ConsolidatedDB

GO
SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO

If exists (Select * From dbo.sysobjects Where id = object_id(N'dbo.rpt_sp_ta_StationConfigurationReport') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
Drop Procedure dbo.rpt_sp_ta_StationConfigurationReport
GO

CREATE PROCEDURE dbo.rpt_sp_ta_StationConfigurationReport
 /*=============================================
 Author:		Kimberly Foote   
 Create date:	6/15/2009
 Description:	Station Configuration
 Version:		7.5.1.1
 Execution:
				Execute  rpt_sp_ta_StationConfigurationReport 1,1,2,'125478',0

 Modification History:
	Date		by		Description
	12/11/2009	KF		Version 7.5.1.0
	2/17/2010	KF		Version change due to change in report

 =============================================*/

@SiteIndex int,
@LoginSiteIndex int,
@UserIndex int,
@StationIndex int,
@Header int

AS

CREATE TABLE #LoadArmsConfig (									--@Header 3
										[Index]			int,
										[StationID]		nvarchar(100),
										[LoadRackText]	nvarchar(18),
										[Enabled]		nvarchar(2),
										[SwingArm]		nvarchar(2),
										[PresetType]	int,
										[Index2]		int,
										[Arm]			int, 			--BayBArmNumber
										[LoadRackText2] nvarchar(18),
										[TabType]		nvarchar(100),
										[ProductId]		nvarchar(60),
										[MeterID]		nvarchar(40),
										[PresetNumber]	int,
										[TankID]		nvarchar(100)
								)

/*************
BEGIN General Header Info  --@Header 0
**************/
		Select 
--General	
				 s.[Index]
				,s.ID as [StationID]
				,s.[Type] as [StationType]
				,s.InterfaceType as [InterfaceType]
				,'Localhost' as [System]
				,case when s.ThirtyFiveBitCardSupport = cast(1 AS bit)  then 'x' else '' end as [35bitCardSupp]
				,case when s.CardReader = cast(1 AS bit) then 'x' else '' end as [CardReader]	
--Load Rack
				,case when s.SwingArmPosition = cast(1 AS bit) then 'A' 
					  when s.SwingArmPosition = cast(0 AS bit) then 'B' else '' end as [SwingArmPosition]
				,case when s.VaporRecovery = cast(1 AS bit) then 'x' else '' end as [Vapor Recovery]
				,case when s.SetDefaultPresetToZero = cast(1 AS bit) then 'x' else ''end as [DefaultPresetZero]
				,case when s.InhibitLoadingByLoadID = cast(1 AS bit) then 'x' else ''end as [InhibitLoadingLoadID]
				,case when s.SynchronizeReferenceDensity = cast(1 AS bit) then 'x' else ''end as [SynchronizeRefDensity]
				,case when s.InhibitSettingRecipeNames = cast(1 AS bit) then 'x' else ''end as [InhibitSetRecipeNm]
				,s.BOLPrinter as [BOLPrinter]
				,s.NumberofCopies as [NoCopies]

INTO #StationConfig

		From	 dbo.tblStations s
		Where @StationIndex = s.[Index]

		Order By [StationID]

/*************
END General Header Info
**************/

/*************
BEGIN Bay "A" and Bay "B" Load Arms  --@Header = 1
**************/

		Select		 BayAStationIndex as [Station]
					,BayAArmNumber as [BayArm]
					,LoadRackText as [LoadRack]
					
INTO #LoadArmsA

		From tblLoadArms 
		where @StationIndex = BayAStationIndex


		Select		 BayBStationIndex as [Station]
					,BayBArmNumber as [BayArm]
					,LoadRackText  as [LoadRack]
INTO #LoadArmsB

		From tblLoadArms 
		where @StationIndex = BayBStationIndex


/*************
END Bay "A" and Bay "B" Load Arms  
**************/



/*************
BEGIN General Bay "A" and Bay "B" Arm Configuration 
**************/
		Select 
				 s.[Index]
				,s.ID as [StationID]
				,la.LoadRackText as [LoadRackText]
				,case when la.[Enabled] = cast(1 AS bit) then 'x' else '' end as [Enabled]
				,case when SwingArm = cast(1 AS bit) then 'x' else '' end as [SwingArm]
				,PresetType

INTO #GeneralBayA

		from tblLoadArms la
			left join tblStations s on
							la.BayAStationIndex = s.[Index]

		Where		BayAStationIndex <> '' 
				and @StationIndex = s.[Index]
		Order by s.ID 


		Select 
				 s.[Index]
				,s.ID as [StationID]
				,la.LoadRackText as [LoadRackText]
				,case when la.[Enabled] = cast(1 AS bit) then 'x' else '' end as [Enabled]
				,case when SwingArm = cast(1 AS bit) then 'x' else '' end as [SwingArm]
				,PresetType

INTO #GeneralBayB

from tblLoadArms la
	left join tblStations s on
					la.BayBStationIndex = s.[Index]



		Where		BayBStationIndex <> ''
				and @StationIndex = s.[Index]
		Order by s.ID 

/*************
END General Bay "A" and Bay "B" Arm Configuration --@Header = 2
**************/


/*************
BEGIN Bay "A"  and Bay "B" Arm Configuration  --@Header = 3
**************/

Select 
				 s.[Index] as [Index2]
				,la.BayAArmNumber as [Arm]
				,la.LoadRackText as [LoadRackText2]
				,case when pm.[Type] = 3 then 'Recipes'
					  when pm.[Type] = 4 then 'Injectors'
					  when pm.[Type] = 7 then 'Tank Product'
											else '' end as [TabType]
				,ProductId
				,pm.MeterID
				,PresetNumber
				,t.TankID



INTO #BayA

		From tblProductMap pm 
				left join tblProducts p on
					pm.AssignedIndex = p.ProductIndex
				left join tblLoadArms la on
					pm.AssignedToIndex = la.[Index]
				left join tblStations s on
					la.BayAStationIndex = s.[Index]
				left join tblTanks t on
					pm.TankIndex = t.TankIndex

		Where		BayAStationIndex <> '' 
				and @StationIndex = s.[Index]
		Order by pm.AssignedToindex




		Select 
				 s.[Index] as [Index2]
				,la.BayBArmNumber as [Arm]
				,la.LoadRackText as [LoadRackText2]
				,case when pm.[Type] = 3 then 'Recipes'
					  when pm.[Type] = 4 then 'Injectors'
					  when pm.[Type] = 7 then 'Tank Product'
											else '' end as [TabType]
				,ProductId
				,pm.MeterID
				,PresetNumber
				,t.TankID



INTO #BayB

		From tblProductMap pm 
				left join tblProducts p on
					pm.AssignedIndex = p.ProductIndex
				left join tblLoadArms la on
					pm.AssignedToIndex = la.[Index]
				left join tblStations s on
					la.BayBStationIndex = s.[Index]
				left join tblTanks t on
					pm.TankIndex = t.TankIndex



		Where		BayBStationIndex <> ''
				and @StationIndex = s.[Index]
		Order by pm.AssignedToindex

/*************
END Bay "A"  and Bay "B" Arm Configuration  --@Header = 3
**************/

/*************
Main Query
**************/

IF @Header = 0 --General Header Tab
BEGIN
		Select * 
				From #StationConfig
END
	ELSE
		BEGIN
IF @Header = 1 --Load Arms Tab
BEGIN 
				
			Select * 
				From #LoadArmsA
	
				UNION ALL
			Select *
				From #LoadArmsB
END
	ELSE
		BEGIN
	
--@Header = 2	-- Load Arms Recipes, Injectors, Tank Products

			select 
				*
			INTO #temp1
			from #GeneralBayA

			UNION ALL

			select 
				*
			from #GeneralBayB



INSERT INTO #LoadArmsConfig
			select 
				 '' [Index]
				,'' [StationID]	
				,'' [LoadRackText]
				,'' [Enabled]
				,'' [SwingArm]	
				,'' [PresetType]
				,* 
			from #BayA

			UNION ALL

			select 
				 '' [Index]
				,'' [StationID]	
				,'' [LoadRackText]
				,'' [Enabled]
				,'' [SwingArm]	
				,'' [PresetType]
				,* 
			from #BayB




update #LoadArmsConfig
set [Index] =[Index2], [StationID] = a.[StationID], [Arm] = [Arm],[LoadRackText] = a.[LoadRackText],
	[Enabled] = a.[Enabled],[SwingArm] = a.[SwingArm],[PresetType] = a.[PresetType]
		from #temp1 a
		where  a.[LoadRackText] = [LoadRackText2]

select * from #LoadArmsConfig





			

END
END


Drop table #StationConfig
Drop table #LoadArmsA
Drop table #LoadArmsB
Drop table #GeneralBayA
Drop table #GeneralBayB
Drop table #BayA
Drop table #BayB
Drop table #LoadArmsConfig

/***********TEST AREA
declare @Header int
set @Header = 0

******************/


GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO
 

GRANT EXECUTE ON dbo.rpt_sp_ta_StationConfigurationReport TO [public]
GO












				


