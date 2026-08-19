USE ConsolidatedDB

GO
SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO

If exists (Select * From dbo.sysobjects Where id = object_id(N'dbo.rpt_sp_ta_TankConfigurationReport') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
Drop Procedure dbo.rpt_sp_ta_TankConfigurationReport
GO

CREATE PROCEDURE dbo.rpt_sp_ta_TankConfigurationReport
 /*=============================================
 Author:		Kimberly Foote
 Create date:	6/18/2009
 Description: Tank Configuration Report
 Version:		7.5.1.0
 Execution:	
			Execute rpt_sp_ta_TankConfigurationReport 1,1,2,'3122 - CITGO Petr Corp',''
 Modification History:
	Date		by		Description
6/18/2009		KF		New standard report
12/11/2009		KF		Version 7.5.1.0
 =============================================*/

@LoginSiteIndex int,
@SiteIndex int,
@UserIndex int,
@Manager nvarchar(30),
@Tank nvarchar(30)

AS

IF @Tank = '<All>' SET @Tank = ''

DECLARE @LevelUnits int
SET @LevelUnits = (SELECT LevelUnitIndex FROM tblSites with(nolock) WHERE tblSites.SiteIndex = @LoginSiteIndex)

DECLARE @LevelDecimalPlaces int
SET @LevelDecimalPlaces = (SELECT LevelDecimalPlaces FROM tblSites with(nolock) WHERE tblSites.SiteIndex = @LoginSiteIndex)

DECLARE @TemperatureUnits int
SET @TemperatureUnits = (SELECT TemperatureUnitIndex FROM tblSites with(nolock) WHERE tblSites.SiteIndex = @LoginSiteIndex)

DECLARE @TemperatureDecimalPlaces int
SET @TemperatureDecimalPlaces = (SELECT TemperatureDecimalPlaces FROM tblSites with(nolock) WHERE tblSites.SiteIndex = @LoginSiteIndex)

DECLARE @VolumeUnits int
SET @VolumeUnits = (SELECT VolumeUnitIndex FROM tblSites with(nolock) WHERE tblSites.SiteIndex = @LoginSiteIndex)

DECLARE @VolumeDecimalPlaces int
SET @VolumeDecimalPlaces = (SELECT VolumeDecimalPlaces FROM tblSites with(nolock) WHERE tblSites.SiteIndex = @LoginSiteIndex)

DECLARE @DensityUnits int
SET @DensityUnits = (SELECT DensityUnitIndex FROM tblSites with(nolock) WHERE tblSites.SiteIndex = @LoginSiteIndex)

DECLARE @DensityDecimalPlaces int
SET @DensityDecimalPlaces = (SELECT DensityDecimalPlaces FROM tblSites with(nolock) WHERE tblSites.SiteIndex = @LoginSiteIndex)

DECLARE @MaxUnits int
SET @MaxUnits = (SELECT MassUnitIndex FROM tblSites with(nolock) WHERE tblSites.SiteIndex = @LoginSiteIndex)

DECLARE @MaxDecimalPlaces int
SET @MaxDecimalPlaces = (SELECT MassDecimalPlaces FROM tblSites with(nolock) WHERE tblSites.SiteIndex = @LoginSiteIndex)

DECLARE @PressureUnits int
SET @PressureUnits = (SELECT PressureUnitIndex FROM tblSites with(nolock) WHERE tblSites.SiteIndex = @LoginSiteIndex)

DECLARE @PressureDecimalPlaces int
SET @PressureDecimalPlaces = (SELECT PressureDecimalPlaces FROM tblSites with(nolock) WHERE tblSites.SiteIndex = @LoginSiteIndex)


Select 
		  t.TankIndex
		 ,t.TankID as [ID]
		 ,case when p.ProductType = 0 then 'Tank Product'
			   when p.ProductType = 1 then 'Blend'
			   when p.ProductType = 2 then 'Additive'
					else '' end as [ProductType] --where is location
		 ,p.ProductID as [Product]
		 ,case when VesselTypeIndex = 0 then 'Undefined'
			   when VesselTypeIndex = 1 then 'Sperical'
			   when VesselTypeIndex = 2 then 'Cylindrical'
			   when VesselTypeIndex = 3 then 'Bullet'
			   when VesselTypeIndex = 4 then 'Propane'
			   when VesselTypeIndex = 5 then 'UnderGround'
			   when VesselTypeIndex = 6 then 'Tanker'
			   when VesselTypeIndex = 7 then 'Pipeline'
			   when VesselTypeIndex = 9 then 'Other'
			   when VesselTypeIndex = 10 then 'Max'
					else '' end as [VesselType]
		,case when ProcessVariableType = 1 then 'Level'
			   when ProcessVariableType = 2 then 'Temperature'
			   when ProcessVariableType = 3 then 'Gross Volume'
			   when ProcessVariableType = 4 then 'Net Volume'
			   when ProcessVariableType = 5 then 'Density'
			   when ProcessVariableType = 6 then 'Standard Density'
			   when ProcessVariableType = 7 then 'Mass'
			   when ProcessVariableType = 28 then 'VCF'
			   when ProcessVariableType = 30 then 'Available Gross Volume'
			   when ProcessVariableType = 31 then 'Remaining Gross Volume'
			   when ProcessVariableType = 44 then 'Tank Operation'
			   when ProcessVariableType = 45 then 'Vapor Pressure'
			   when ProcessVariableType = 46 then 'Available Net Volume'
			   when ProcessVariableType = 47 then 'Remaining Net Volume'
			   when ProcessVariableType = 58 then 'Tank Status'
					else '' end as [Type]
		 ,'' as [Units]
		 ,case when ProcessVariableType = 1 then IsNull(dbo.ConvertFromSIUnits(cast(Maximum as float),@LevelUnits,@LevelDecimalPlaces),0.0) 
			   when ProcessVariableType = 2 then IsNull(dbo.ConvertFromSIUnits(cast(Maximum as float),@TemperatureUnits,@TemperatureDecimalPlaces),0.0)
			   when ProcessVariableType = 3 then IsNull(dbo.ConvertFromSIUnits(cast(Maximum as float),@VolumeUnits,@VolumeDecimalPlaces),0.0)
			   when ProcessVariableType = 4 then IsNull(dbo.ConvertFromSIUnits(cast(Maximum as float),@VolumeUnits,@VolumeDecimalPlaces),0.0)
			   when ProcessVariableType = 5 then IsNull(dbo.ConvertFromSIUnits(cast(Maximum as float),@DensityUnits,@DensityDecimalPlaces),0.0)
			   when ProcessVariableType = 6 then IsNull(dbo.ConvertFromSIUnits(cast(Maximum as float),@DensityUnits,@DensityDecimalPlaces),0.0)
			   when ProcessVariableType = 7 then IsNull(dbo.ConvertFromSIUnits(cast(Maximum as float),@MaxUnits,@MaxDecimalPlaces),0.0)
			   when ProcessVariableType = 28 then IsNull(cast(Maximum as float),0.0)
			   when ProcessVariableType = 30 then IsNull(dbo.ConvertFromSIUnits(cast(Maximum as float),@VolumeUnits,@VolumeDecimalPlaces),0.0)
			   when ProcessVariableType = 31 then IsNull(dbo.ConvertFromSIUnits(cast(Maximum as float),@VolumeUnits,@VolumeDecimalPlaces),0.0)
			   when ProcessVariableType = 44 then 0
			   when ProcessVariableType = 45 then IsNull(dbo.ConvertFromSIUnits(cast(Maximum as float),@PressureUnits,@PressureDecimalPlaces),0.0)
			   when ProcessVariableType = 46 then IsNull(dbo.ConvertFromSIUnits(cast(Maximum as float),@VolumeUnits,@VolumeDecimalPlaces),0.0)
			   when ProcessVariableType = 47 then IsNull(dbo.ConvertFromSIUnits(cast(Maximum as float),@VolumeUnits,@VolumeDecimalPlaces),0.0)
			   when ProcessVariableType = 58 then 0
					else '' end as [Maximum]
		,case when ProcessVariableType = 1 then IsNull(dbo.ConvertFromSIUnits(cast(Minimum as float),@LevelUnits,@LevelDecimalPlaces),0.0)
			  when ProcessVariableType = 2 then IsNull(dbo.ConvertFromSIUnits(cast(Minimum as float),@TemperatureUnits,@TemperatureDecimalPlaces),0.0) 
			  when ProcessVariableType = 3 then IsNull(dbo.ConvertFromSIUnits(cast(Minimum as float),@VolumeUnits,@VolumeDecimalPlaces),0.0)
			  when ProcessVariableType = 4 then IsNull(dbo.ConvertFromSIUnits(cast(Minimum as float),@VolumeUnits,@VolumeDecimalPlaces),0.0)
			  when ProcessVariableType = 5 then IsNull(dbo.ConvertFromSIUnits(cast(Minimum as float),@DensityUnits,@DensityDecimalPlaces),0.0)
			  when ProcessVariableType = 6 then IsNull(dbo.ConvertFromSIUnits(cast(Minimum as float),@DensityUnits,@DensityDecimalPlaces),0.0)
			  when ProcessVariableType = 7 then IsNull(dbo.ConvertFromSIUnits(cast(Minimum as float),@MaxUnits,@MaxDecimalPlaces),0.0)
			  when ProcessVariableType = 28 then IsNull(cast(Minimum as float),0.0)
			  when ProcessVariableType = 30 then IsNull(dbo.ConvertFromSIUnits(cast(Minimum as float),@VolumeUnits,@VolumeDecimalPlaces),0.0)
			  when ProcessVariableType = 31 then IsNull(dbo.ConvertFromSIUnits(cast(Minimum as float),@VolumeUnits,@VolumeDecimalPlaces),0.0)
			  when ProcessVariableType = 44 then 0
			  when ProcessVariableType = 45 then IsNull(dbo.ConvertFromSIUnits(cast(Minimum as float),@PressureUnits,@PressureDecimalPlaces),0.0)
			  when ProcessVariableType = 46 then IsNull(dbo.ConvertFromSIUnits(cast(Minimum as float),@VolumeUnits,@VolumeDecimalPlaces),0.0)
			  when ProcessVariableType = 47 then IsNull(dbo.ConvertFromSIUnits(cast(Minimum as float),@VolumeUnits,@VolumeDecimalPlaces),0.0)
			  when ProcessVariableType = 58 then 0
					else '' end as [Minimum]
		,'Localhost' as [System]
		,ProgID as [OPCServer]
		,OPCItemID as [ItemID]

From tblProcessVariables pv
		left join tblOPCConnections O on
			pv.OPCConnectionIndex = O.[Index]
		left join tblTanks t on
			t.TankIndex = pv.UnitIndex
		left join tblProducts p on
			t.ProductIndex = p.ProductIndex
					
Where
	 t.TankID is not null 
and (@Tank = null OR @Tank = '' OR  t.TankID = @Tank)
Order by t.TankID,pv.processvariabletype



GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO
 

GRANT EXECUTE ON dbo.rpt_sp_ta_TankConfigurationReport TO [public]
GO

