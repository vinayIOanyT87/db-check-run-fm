namespace FuelsManager.Areas.InventoryManagement.ViewModels
{
	using System;
	using System.Collections.Generic;
	using System.ComponentModel.DataAnnotations;
	using System.Globalization;
	using System.Linq;
	using System.Web.Mvc;

	using Varec.CommonComponents.EngineeringUnitsLibrary;

	using FMBusinessObjects.DataObjects;

	using FMPointCommon;
	using Areas.Controllers;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;

	using Newtonsoft.Json;

	#region Points Detail Model Class
	public class PointsDetailModel : FMBaseModel
	{
		[Required]
      [MaxLength(27, ErrorMessage = "Must not be more than 27 characters.")]
      public string ID { get; set; }

		public PointTemplateCollection Templates { get; set; }

		[Required]
		public Guid TemplateSelection { get; set; }

		[Required]
      [Range(1, 999, ErrorMessage = "Must be between 1 and 999.")] 
		public int NumberToCreate { get; set; } = 1;

      public PointsDetailModel() 
		{
         Templates = new PointTemplateCollection();

      }
   }
	#endregion

	#region Point/Template Property Model Class
	public class PropertyModel
	{
		public SiteClass Site { get; set; }
		public bool IsTemplatePoint { get; set; }
		public bool IsStandard { get; set; }
		public int DerivedPointCount { get; set;}
		public bool ModifyEnabled;
		public Guid PointGuid { get; set; }
		public Dictionary<Guid,List<SelectListItem>> PointCommandStatusListDictionary { get; set; }
		public BaseSerializedDataObject PropertyBase { get; set; }
	}
	#endregion

	#region Point Edit Detail Model Class
	[Serializable]
	public class PointEditDetailModel : FMBaseModel, IValidatableObject
	{

		public bool ModifyEnabled = true;

		public bool IsTemplatePoint = false;

		public bool HasCopyRight;

		public bool HasEnablePointRight;

		public bool HasDisablePointRight;

		public bool HasViewPCSList;

		public bool HasModifyPCSList;

		public bool HasFCEERight = false;

		public bool HasModifyFCEERight = false;

		public string OpenFormForTag;

		public string OpenFormForModule;

		public MvcHtmlString GuideOpenerScript { get; set; }

		public Point Point { get; set; }
		public PointTemplate PointTemplate { get; set; }
		public string PointType { get; set; }
		public Guid? PointTypeGuid { get; set; }
		public Guid PointGuid { get; set; }
		public Guid IdentityGuid { get; set; }
		public string Name { get; set; }
		public bool Enabled { get; set; }
		public string Description { get; set; }
		public Guid SiteGuid { get; set; }
		public string TemplateName { get; set; }
		public string Notes { get; set; }
		public Guid? ProfileImageGuid { get; set; }
		public Guid SelectedModuleInstanceGuid { get; set; }

		public EngineeringUnit LevelUnit { get; set; }
		[Required(ErrorMessage = "PointEditor|The Levels Precision is required.")]
		public int LevelDecimalPlaces { get; set; }
		[Required(ErrorMessage = "PointEditor|The Levels Minimum is required.")]
		public string LevelMinimum { get; set; }
		public double? LevelMinimumRaw { get; set; }
		[Required(ErrorMessage = "PointEditor|The Levels Maximum is required.")]
		public string LevelMaximum { get; set; }
		public double? LevelMaximumRaw { get; set; }

		public EngineeringUnit TemperatureUnit { get; set; }
		[Required(ErrorMessage = "PointEditor|The Temperatures Precision is required.")]
		public int TemperatureDecimalPlaces { get; set; }
		[Required(ErrorMessage = "PointEditor|The Temperatures Minimum is required.")]
		public string TemperatureMinimum { get; set; }
		public double? TemperatureMinimumRaw { get; set; }
		[Required(ErrorMessage = "PointEditor|The Temperatures Maximum is required.")]
		public string TemperatureMaximum { get; set; }
		public double? TemperatureMaximumRaw { get; set; }

		public EngineeringUnit DensityUnit { get; set; }
		[Required(ErrorMessage = "PointEditor|The Densities Precision is required.")]
		public int DensityDecimalPlaces { get; set; }
		[Required(ErrorMessage = "PointEditor|The Densities Minimum is required.")]
		public string DensityMinimum { get; set; }
		public double? DensityMinimumRaw { get; set; }
		[Required(ErrorMessage = "PointEditor|The Densities Maximum is required.")]
		public string DensityMaximum { get; set; }
		public double? DensityMaximumRaw { get; set; }

		public EngineeringUnit PressureUnit { get; set; }
		[Required(ErrorMessage = "PointEditor|The Pressures Precision is required.")]
		public int PressureDecimalPlaces { get; set; }
		[Required(ErrorMessage = "PointEditor|The Pressures Minimum is required.")]
		public string PressureMinimum { get; set; }
		public double? PressureMinimumRaw { get; set; }
		[Required(ErrorMessage = "PointEditor|The Pressures Maximum is required.")]
		public string PressureMaximum { get; set; }
		public double? PressureMaximumRaw { get; set; }

		public EngineeringUnit FlowUnit { get; set; }
		[Required(ErrorMessage = "PointEditor|The Volume Rates Precision is required.")]
		public int FlowDecimalPlaces { get; set; }
		[Required(ErrorMessage = "PointEditor|The Volume Rates Minimum is required.")]
		public string VolumetricFlowMinimum { get; set; }
		public double? VolumetricFlowMinimumRaw { get; set; }
		[Required(ErrorMessage = "PointEditor|The Volume Rates Maximum is required.")]
		public string VolumetricFlowMaximum { get; set; }
		public double? VolumetricFlowMaximumRaw { get; set; }

		public EngineeringUnit VolumeUnit { get; set; }
		[Required(ErrorMessage = "PointEditor|The Volumes Precision is required.")]
		public int VolumeDecimalPlaces { get; set; }
		[Required(ErrorMessage = "PointEditor|The Volumes Minimum is required.")]
		public string VolumeMinimum { get; set; }
		public double? VolumeMinimumRaw { get; set; }
		[Required(ErrorMessage = "PointEditor|The Volumes Maximum is required.")]
		public string VolumeMaximum { get; set; }
		public double? VolumeMaximumRaw { get; set; }

		public EngineeringUnit MassUnit { get; set; }
		[Required(ErrorMessage = "PointEditor|The Mass Precision is required.")]
		public int MassDecimalPlaces { get; set; }
		[Required(ErrorMessage = "PointEditor|The Mass Minimum is required.")]
		public string MassMinimum { get; set; }
		public double? MassMinimumRaw { get; set; }
		[Required(ErrorMessage = "PointEditor|The Mass Maximum is required.")]
		public string MassMaximum { get; set; }
		public double? MassMaximumRaw { get; set; }

		public EngineeringUnit VelocityUnit { get; set; }
		[Required(ErrorMessage = "PointEditor|The Level Rates Precision is required.")]
		public int VelocityDecimalPlaces { get; set; }
		[Required(ErrorMessage = "PointEditor|The Level Rates Minimum is required.")]
		public string VelocityMinimum { get; set; }
		public double? VelocityMinimumRaw { get; set; }
		[Required(ErrorMessage = "PointEditor|The Level Rates Maximum is required.")]
		public string VelocityMaximum { get; set; }
		public double? VelocityMaximumRaw { get; set; }

		public EngineeringUnit MassFlowUnit { get; set; }
		[Required(ErrorMessage = "PointEditor|The Mass Rates Precision is required.")]
		public int MassFlowDecimalPlaces { get; set; }
		[Required(ErrorMessage = "PointEditor|The Mass Rates Minimum is required.")]
		public string MassFlowMinimum { get; set; }
		public double? MassFlowMinimumRaw { get; set; }
		[Required(ErrorMessage = "PointEditor|The Mass Rates Maximum is required.")]
		public string MassFlowMaximum { get; set; }
		public double? MassFlowMaximumRaw { get; set; }

		public SiteClass Site { get; set; }

		public HashSet<Guid> AlarmTagHashSet { get; set; }

		public List<PointTagEditGridModel> Tags { get; set; }
		public List<PointPropertyEditModel> Properties { get; set; }
		public AlarmEditorModel Alarms { get; set; }
		public ApplicationStringCollectionClass Categories { get; set; }

		public List<string> ProductIdList { get; set; }
		public List<SelectListItem> ActionListCategories { get; set; }
		public List<SelectListItem> ActionListPointTypes { get; set; }
		public string PointTypeId { get; set; }
		public string CategoryId { get; set; }

		public string OverrideDefaultDrawingGuidString { get; set; }

		public List<DrawingName> AssociatedDrawings { get; set; }

		public List<KeyValuePair<Guid, string>> WellKnownTags { get; set; }

		public List<KeyValuePair<string, string>> TagDataTypes { get; set; }

		public List<SelectListItem> TagInputOutputTypes { get; set; }

		public List<SelectListItem> OutputTagChangeAgents { get; set; }

		public Dictionary<Guid, List<SelectListItem>> PointCommandStatusListDictionary { get; set; }
		  
		public Dictionary<Guid, FCEEMapping> FCEEMappings { get; set; }

		public PointEditDetailModel()
		{
				this.Tags = new List<PointTagEditGridModel>();
				this.AssociatedDrawings = new List<DrawingName>();
				this.Alarms = new AlarmEditorModel();
		}

		public PointEditDetailModel(	Point point,
												SiteClass site,
												ApplicationStringCollectionClass categories,
												List<string> productIdList,
												List<DrawingName> associatedDrawings,
												List<SelectListItem> tagInputOutputTypes,
												List<SelectListItem> outputTagChangeAgents,
												Dictionary<Guid, List<SelectListItem>> pointCommandStatusListDictonary,
												Dictionary<Guid, FCEEMapping> fceeMappings)
		{
			var numberFormatInfo = new NumberFormatInfo
										{
											NumberGroupSizes = site.GetNumberGroupSizes(),
											NumberGroupSeparator = site.NumberGroupSeparator,
											NumberDecimalSeparator = site.NumberDecimalSeparator
										};

			//TODO: remove references to model.Point, this is just temporary and MVC has issues with complex objects
			this.IsTemplatePoint = false;  // we are working with points, not point templates
			this.Point		= point;
			this.Site		= site;
			this.Categories = categories;
			this.ProductIdList = productIdList;
			this.AssociatedDrawings = associatedDrawings;
			this.TagInputOutputTypes = tagInputOutputTypes;
			this.OutputTagChangeAgents = outputTagChangeAgents;
			this.PointCommandStatusListDictionary = pointCommandStatusListDictonary;
			this.FCEEMappings = fceeMappings;

			this.PointGuid		= point.PointGuid;
			this.IdentityGuid	= point.PointGuid;

			this.Name			= point.ID;
			this.PointType		= point.PointType;
			this.PointTypeGuid = null;
			this.Description	= point.Description;
			this.Enabled		= point.Enabled;
			this.SiteGuid		= point.SiteGuid;
			this.TemplateName	= point.TemplateName;
			this.Notes			= point.Notes;
			this.ProfileImageGuid = point.ProfileImageGuid;
			this.SelectedModuleInstanceGuid = new Guid();

			this.LevelUnit							= point.LevelUnit;
			this.LevelDecimalPlaces					= point.LevelDecimalPlaces;
			numberFormatInfo.NumberDecimalDigits	= point.LevelDecimalPlaces;
			this.LevelMinimum						= PointManager.FormatValueFullPrecision(typeof(double), point.LevelUnit, numberFormatInfo, point.LevelMinimum);
			this.LevelMinimumRaw					= point.LevelMinimum;
			this.LevelMaximum						= PointManager.FormatValueFullPrecision(typeof(double), point.LevelUnit, numberFormatInfo, point.LevelMaximum);
			this.LevelMaximumRaw					= point.LevelMaximum;

			this.TemperatureUnit					= point.TemperatureUnit;
			this.TemperatureDecimalPlaces			= point.TemperatureDecimalPlaces;
			numberFormatInfo.NumberDecimalDigits = point.TemperatureDecimalPlaces;
			this.TemperatureMinimum					= PointManager.FormatValueFullPrecision(typeof(double), point.TemperatureUnit, numberFormatInfo, point.TemperatureMinimum);
			this.TemperatureMinimumRaw				= point.TemperatureMinimum;
			this.TemperatureMaximum					= PointManager.FormatValueFullPrecision(typeof(double), point.TemperatureUnit, numberFormatInfo, point.TemperatureMaximum);
			this.TemperatureMaximumRaw				= point.TemperatureMaximum;

			this.DensityUnit						= point.DensityUnit;
			this.DensityDecimalPlaces				= point.DensityDecimalPlaces;
			numberFormatInfo.NumberDecimalDigits	= point.DensityDecimalPlaces;
			this.DensityMinimum						= PointManager.FormatValueFullPrecision(typeof(double), point.DensityUnit, numberFormatInfo, point.DensityMinimum);
			this.DensityMinimumRaw					= point.DensityMinimum;
			this.DensityMaximum						= PointManager.FormatValueFullPrecision(typeof(double), point.DensityUnit, numberFormatInfo, point.DensityMaximum);
			this.DensityMaximumRaw					= point.DensityMaximum;

			this.PressureUnit						= point.PressureUnit;
			this.PressureDecimalPlaces				= point.PressureDecimalPlaces;
			numberFormatInfo.NumberDecimalDigits	= point.PressureDecimalPlaces;
			this.PressureMinimum					= PointManager.FormatValueFullPrecision(typeof(double), point.PressureUnit, numberFormatInfo, point.PressureMinimum);
			this.PressureMinimumRaw					= point.PressureMinimum;
			this.PressureMaximum					= PointManager.FormatValueFullPrecision(typeof(double), point.PressureUnit, numberFormatInfo, point.PressureMaximum);
			this.PressureMaximumRaw					= point.PressureMaximum;

			this.FlowUnit							= point.FlowUnit;
			this.FlowDecimalPlaces					= point.FlowDecimalPlaces;
			numberFormatInfo.NumberDecimalDigits	= point.FlowDecimalPlaces;
			this.VolumetricFlowMinimum				= PointManager.FormatValueFullPrecision(typeof(double), point.FlowUnit, numberFormatInfo, point.VolumetricFlowMinimum);
			this.VolumetricFlowMinimumRaw			= point.VolumetricFlowMinimum;
			this.VolumetricFlowMaximum				= PointManager.FormatValueFullPrecision(typeof(double), point.FlowUnit, numberFormatInfo, point.VolumetricFlowMaximum);
			this.VolumetricFlowMaximumRaw			= point.VolumetricFlowMaximum;

			this.VolumeUnit							= point.VolumeUnit;
			this.VolumeDecimalPlaces				= point.VolumeDecimalPlaces;
			numberFormatInfo.NumberDecimalDigits	= point.VolumeDecimalPlaces;
			this.VolumeMinimum						= PointManager.FormatValueFullPrecision(typeof(double), point.VolumeUnit, numberFormatInfo, point.VolumeMinimum);
			this.VolumeMinimumRaw					= point.VolumeMinimum;
			this.VolumeMaximum						= PointManager.FormatValueFullPrecision(typeof(double), point.VolumeUnit, numberFormatInfo, point.VolumeMaximum);
			this.VolumeMaximumRaw					= point.VolumeMaximum;

			this.MassUnit							= point.MassUnit;
			this.MassDecimalPlaces					= point.MassDecimalPlaces;
			numberFormatInfo.NumberDecimalDigits	= point.MassDecimalPlaces;
			this.MassMinimum						= PointManager.FormatValueFullPrecision(typeof(double), point.MassUnit, numberFormatInfo, point.MassMinimum);
			this.MassMinimumRaw						= point.MassMinimum;
			this.MassMaximum						= PointManager.FormatValueFullPrecision(typeof(double), point.MassUnit, numberFormatInfo, point.MassMaximum);
			this.MassMaximumRaw						= point.MassMaximum;

			this.VelocityUnit						= point.VelocityUnit;
			this.VelocityDecimalPlaces				= point.VelocityDecimalPlaces;
			numberFormatInfo.NumberDecimalDigits	= point.VelocityDecimalPlaces;
			this.VelocityMinimum					= PointManager.FormatValueFullPrecision(typeof(double), point.VelocityUnit, numberFormatInfo, point.VelocityMinimum);
			this.VelocityMinimumRaw					= point.VelocityMinimum;
			this.VelocityMaximum					= PointManager.FormatValueFullPrecision(typeof(double), point.VelocityUnit, numberFormatInfo, point.VelocityMaximum);
			this.VelocityMaximumRaw					= point.VelocityMaximum;

			this.MassFlowUnit						= point.MassFlowUnit;
			this.MassFlowDecimalPlaces				= point.MassFlowDecimalPlaces;
			numberFormatInfo.NumberDecimalDigits	= point.MassFlowDecimalPlaces;
			this.MassFlowMinimum					= PointManager.FormatValueFullPrecision(typeof(double), point.MassFlowUnit, numberFormatInfo, point.MassFlowMinimum);
			this.MassFlowMinimumRaw					= point.MassFlowMinimum;
			this.MassFlowMaximum					= PointManager.FormatValueFullPrecision(typeof(double), point.MassFlowUnit, numberFormatInfo, point.MassFlowMaximum);
			this.MassFlowMaximumRaw					= point.MassFlowMaximum;

			this.OverrideDefaultDrawingGuidString = (point.OverrideDefaultDrawingGuid != null) ? point.OverrideDefaultDrawingGuid.ToString().ToLower() : Guid.Empty.ToString().ToLower();


			this.Tags = new List<PointTagEditGridModel>();

			this.AlarmTagHashSet = new HashSet<Guid>();

			foreach (PointTag pointTag in point.Tags.Values.ToList())
			{
				numberFormatInfo.NumberDecimalDigits = pointTag.DecimalPlaces;

				var modelTag = new PointTagEditGridModel
				{
					Name = pointTag.ID,
					PointTagGuid = pointTag.PointTagGuid,
					Unit = pointTag.Units,
					DecimalPlaces = pointTag.DecimalPlaces,
					ServerUnit = pointTag.ServerUnits,
					Minimum = PointManager.FormatValueFullPrecision(typeof(double), pointTag.Units, numberFormatInfo, pointTag.Minimum),
					MinimumRaw = pointTag.Minimum,
					Maximum = PointManager.FormatValueFullPrecision(typeof(double), pointTag.Units, numberFormatInfo, pointTag.Maximum),
					MaximumRaw = pointTag.Maximum,
					InputOutputType = pointTag.InputOutputType,
					Input = pointTag.Input,
					InhibitInputOutputTypeConfiguration = pointTag.InhibitInputOutputTypeConfiguration,
					InhibitOverride = pointTag.InhibitOverride,
					Archived = pointTag.Archived, 
					ApplyPointEngineeringUnits	= pointTag.ApplyPointEngineeringUnits,
					ApplyPointDecimalPlaces		= pointTag.ApplyPointDecimalPlaces,
					ApplyPointMinimum			= pointTag.ApplyPointMinimum,
					ApplyPointMaximum			= pointTag.ApplyPointMaximum,
					EngineeringUnitsType		= pointTag.EngineeringUnitsType,
					OpcUaNamespaceUri			= pointTag.OpcUaNamespaceUri,
					OpcUaBrowsePath				= pointTag.OpcUaBrowsePath,
					OpcUaNodeId					= pointTag.OpcUaNodeId,
					OpcUaPublishingInterval		= pointTag.OpcUaPublishingInterval,
					DataType					= pointTag.ValueTypeString
				};

				// archive has not been implemented

				this.Tags.Add(modelTag);
				if (pointTag.Alarms.Any())
				{
					AlarmTagHashSet.Add(pointTag.PointTagGuid);

					foreach (var alarm in pointTag.Alarms.Values)
					{
						AlarmTagHashSet.Add(alarm.AlarmStateTagGuid);

						foreach (var alarmTest in alarm.AlarmTests.Values)
						{
							AlarmTagHashSet.Add(alarmTest.LimitTagGuid);
						}
					}
				}
			}



			this.Alarms = new AlarmEditorModel();
			this.ActionListCategories	= new List<SelectListItem>();
			this.ActionListPointTypes	= new List<SelectListItem>();
			this.PointTypeId			= string.Empty;
			this.CategoryId				= string.Empty;
				
		}

		public PointEditDetailModel(PointTemplate pointTemplate,
											SiteClass site,
											ApplicationStringCollectionClass categories,
											List<string> productIdList, List<DrawingName> associatedDrawings,
											List<KeyValuePair<Guid, string>> wellKnownTags,
											List<KeyValuePair<string, string>> tagDataTypes,
											List<SelectListItem> tagInputOutputTypes,
											List<SelectListItem> outputTagChangeAgents,
											Dictionary<Guid,List<SelectListItem>> pointCommandStatusListDictonary)
		{
			var numberFormatInfo = new NumberFormatInfo
			{
				NumberGroupSizes = site.GetNumberGroupSizes(),
				NumberGroupSeparator = site.NumberGroupSeparator,
				NumberDecimalSeparator = site.NumberDecimalSeparator
			};

			//TODO: remove references to model.Point, this is just temporary and MVC has issues with complex objects
			this.Point = new Point();
			this.PointTemplate = pointTemplate;
			this.IsTemplatePoint = true;  // we are working with point templates
			this.Site = site;
			this.Categories = categories;
			this.ProductIdList = productIdList;
			this.AssociatedDrawings = associatedDrawings;
			this.WellKnownTags = wellKnownTags;
			this.TagDataTypes = tagDataTypes;
			this.TagInputOutputTypes = tagInputOutputTypes;
			this.OutputTagChangeAgents = outputTagChangeAgents;
			this.PointCommandStatusListDictionary = pointCommandStatusListDictonary;

			this.PointGuid = pointTemplate.PointTemplateGuid;
			this.IdentityGuid = pointTemplate.PointTemplateGuid;

			this.Name = pointTemplate.ID;
			this.PointType = ""; //pointTemplate.PointTemplateTypeGuid;
			this.PointTypeGuid = pointTemplate.PointTemplateTypeGuid;
			this.Description = pointTemplate.Description;
			this.Enabled = true;
			this.SiteGuid = pointTemplate.SiteGuid;
			this.TemplateName = pointTemplate.ID;
			this.Notes = "";
			this.ProfileImageGuid = pointTemplate.ProfileImageGuid;
			this.SelectedModuleInstanceGuid = new Guid();

			this.LevelUnit = pointTemplate.LevelUnit;
			this.LevelDecimalPlaces = pointTemplate.LevelDecimalPlaces;
			numberFormatInfo.NumberDecimalDigits = pointTemplate.LevelDecimalPlaces;
			this.LevelMinimum = PointManager.FormatValueFullPrecision(typeof(double), pointTemplate.LevelUnit, numberFormatInfo, pointTemplate.LevelMinimum);
			this.LevelMinimumRaw = pointTemplate.LevelMinimum;
			this.LevelMaximum = PointManager.FormatValueFullPrecision(typeof(double), pointTemplate.LevelUnit, numberFormatInfo, pointTemplate.LevelMaximum);
			this.LevelMaximumRaw = pointTemplate.LevelMaximum;

			this.TemperatureUnit = pointTemplate.TemperatureUnit;
			this.TemperatureDecimalPlaces = pointTemplate.TemperatureDecimalPlaces;
			numberFormatInfo.NumberDecimalDigits = pointTemplate.TemperatureDecimalPlaces;
			this.TemperatureMinimum = PointManager.FormatValueFullPrecision(typeof(double), pointTemplate.TemperatureUnit, numberFormatInfo, pointTemplate.TemperatureMinimum);
			this.TemperatureMinimumRaw = pointTemplate.TemperatureMinimum;
			this.TemperatureMaximum = PointManager.FormatValueFullPrecision(typeof(double), pointTemplate.TemperatureUnit, numberFormatInfo, pointTemplate.TemperatureMaximum);
			this.TemperatureMaximumRaw = pointTemplate.TemperatureMaximum;

			this.DensityUnit = pointTemplate.DensityUnit;
			this.DensityDecimalPlaces = pointTemplate.DensityDecimalPlaces;
			numberFormatInfo.NumberDecimalDigits = pointTemplate.DensityDecimalPlaces;
			this.DensityMinimum = PointManager.FormatValueFullPrecision(typeof(double), pointTemplate.DensityUnit, numberFormatInfo, pointTemplate.DensityMinimum);
			this.DensityMinimumRaw = pointTemplate.DensityMinimum;
			this.DensityMaximum = PointManager.FormatValueFullPrecision(typeof(double), pointTemplate.DensityUnit, numberFormatInfo, pointTemplate.DensityMaximum);
			this.DensityMaximumRaw = pointTemplate.DensityMaximum;

			this.PressureUnit = pointTemplate.PressureUnit;
			this.PressureDecimalPlaces = pointTemplate.PressureDecimalPlaces;
			numberFormatInfo.NumberDecimalDigits = pointTemplate.PressureDecimalPlaces;
			this.PressureMinimum = PointManager.FormatValueFullPrecision(typeof(double), pointTemplate.PressureUnit, numberFormatInfo, pointTemplate.PressureMinimum);
			this.PressureMinimumRaw = pointTemplate.PressureMinimum;
			this.PressureMaximum = PointManager.FormatValueFullPrecision(typeof(double), pointTemplate.PressureUnit, numberFormatInfo, pointTemplate.PressureMaximum);
			this.PressureMaximumRaw = pointTemplate.PressureMaximum;

			this.FlowUnit = pointTemplate.FlowUnit;
			this.FlowDecimalPlaces = pointTemplate.FlowDecimalPlaces;
			numberFormatInfo.NumberDecimalDigits = pointTemplate.FlowDecimalPlaces;
			this.VolumetricFlowMinimum = PointManager.FormatValueFullPrecision(typeof(double), pointTemplate.FlowUnit, numberFormatInfo, pointTemplate.VolumetricFlowMinimum);
			this.VolumetricFlowMinimumRaw = pointTemplate.VolumetricFlowMinimum;
			this.VolumetricFlowMaximum = PointManager.FormatValueFullPrecision(typeof(double), pointTemplate.FlowUnit, numberFormatInfo, pointTemplate.VolumetricFlowMaximum);
			this.VolumetricFlowMaximumRaw = pointTemplate.VolumetricFlowMaximum;

			this.VolumeUnit = pointTemplate.VolumeUnit;
			this.VolumeDecimalPlaces = pointTemplate.VolumeDecimalPlaces;
			numberFormatInfo.NumberDecimalDigits = pointTemplate.VolumeDecimalPlaces;
			this.VolumeMinimum = PointManager.FormatValueFullPrecision(typeof(double), pointTemplate.VolumeUnit, numberFormatInfo, pointTemplate.VolumeMinimum);
			this.VolumeMinimumRaw = pointTemplate.VolumeMinimum;
			this.VolumeMaximum = PointManager.FormatValueFullPrecision(typeof(double), pointTemplate.VolumeUnit, numberFormatInfo, pointTemplate.VolumeMaximum);
			this.VolumeMaximumRaw = pointTemplate.VolumeMaximum;

			this.MassUnit = pointTemplate.MassUnit;
			this.MassDecimalPlaces = pointTemplate.MassDecimalPlaces;
			numberFormatInfo.NumberDecimalDigits = pointTemplate.MassDecimalPlaces;
			this.MassMinimum = PointManager.FormatValueFullPrecision(typeof(double), pointTemplate.MassUnit, numberFormatInfo, pointTemplate.MassMinimum);
			this.MassMinimumRaw = pointTemplate.MassMinimum;
			this.MassMaximum = PointManager.FormatValueFullPrecision(typeof(double), pointTemplate.MassUnit, numberFormatInfo, pointTemplate.MassMaximum);
			this.MassMaximumRaw = pointTemplate.MassMaximum;

			this.VelocityUnit = pointTemplate.VelocityUnit;
			this.VelocityDecimalPlaces = pointTemplate.VelocityDecimalPlaces;
			numberFormatInfo.NumberDecimalDigits = pointTemplate.VelocityDecimalPlaces;
			this.VelocityMinimum = PointManager.FormatValueFullPrecision(typeof(double), pointTemplate.VelocityUnit, numberFormatInfo, pointTemplate.VelocityMinimum);
			this.VelocityMinimumRaw = pointTemplate.VelocityMinimum;
			this.VelocityMaximum = PointManager.FormatValueFullPrecision(typeof(double), pointTemplate.VelocityUnit, numberFormatInfo, pointTemplate.VelocityMaximum);
			this.VelocityMaximumRaw = pointTemplate.VelocityMaximum;

			this.MassFlowUnit = pointTemplate.MassFlowUnit;
			this.MassFlowDecimalPlaces = pointTemplate.MassFlowDecimalPlaces;
			numberFormatInfo.NumberDecimalDigits = pointTemplate.MassFlowDecimalPlaces;
			this.MassFlowMinimum = PointManager.FormatValueFullPrecision(typeof(double), pointTemplate.MassFlowUnit, numberFormatInfo, pointTemplate.MassFlowMinimum);
			this.MassFlowMinimumRaw = pointTemplate.MassFlowMinimum;
			this.MassFlowMaximum = PointManager.FormatValueFullPrecision(typeof(double), pointTemplate.MassFlowUnit, numberFormatInfo, pointTemplate.MassFlowMaximum);
			this.MassFlowMaximumRaw = pointTemplate.MassFlowMaximum;

			this.OverrideDefaultDrawingGuidString = pointTemplate.DefaultDrawingGuid.ToString();


			this.Tags = new List<PointTagEditGridModel>();

			this.AlarmTagHashSet = new HashSet<Guid>();

			foreach (PointTemplateTag pointTemplateTag in pointTemplate.Tags.Values.ToList())
			{
				numberFormatInfo.NumberDecimalDigits = pointTemplateTag.DecimalPlaces;

				string valueRaw;
				var formattedValue = FormattedValue(pointTemplate, site, pointTemplateTag, numberFormatInfo, out valueRaw);

				var modelTag = new PointTagEditGridModel
				{
					Name = pointTemplateTag.ID,
					Value = pointTemplateTag.Value == null ? "" : formattedValue,
					ValueRaw = valueRaw,
					PointTagGuid = pointTemplateTag.PointTemplateTagGuid,
					Unit = pointTemplateTag.Units,
					DecimalPlaces = pointTemplateTag.DecimalPlaces,
					ServerUnit = pointTemplateTag.ServerUnits,
					Minimum = PointManager.FormatValueFullPrecision(typeof(double), pointTemplateTag.Units, numberFormatInfo, pointTemplateTag.Minimum),
					MinimumRaw = pointTemplateTag.Minimum,
					Maximum = PointManager.FormatValueFullPrecision(typeof(double), pointTemplateTag.Units, numberFormatInfo, pointTemplateTag.Maximum),
					MaximumRaw = pointTemplateTag.Maximum,
					InputOutputType = pointTemplateTag.InputOutputType,
					Input = pointTemplateTag.Input,
					InhibitInputOutputTypeConfiguration = pointTemplateTag.InhibitInputOutputTypeConfiguration,
					InhibitOverride = pointTemplateTag.InhibitOverride,
					Archived = pointTemplateTag.Archived,
					ApplyPointEngineeringUnits = pointTemplateTag.ApplyPointTemplateEngineeringUnits,
					ApplyPointDecimalPlaces = pointTemplateTag.ApplyPointTemplateDecimalPlaces,
					ApplyPointMinimum = pointTemplateTag.ApplyPointTemplateMinimum,
					ApplyPointMaximum = pointTemplateTag.ApplyPointTemplateMaximum,
					EngineeringUnitsType = pointTemplateTag.EngineeringUnitsType,
					OpcUaNamespaceUri = "",
					OpcUaBrowsePath = "",
					OpcUaNodeId = "",
					OpcUaPublishingInterval = 0,
					DataType = pointTemplateTag.ValueTypeString,
					WellKnownIdentityGuidString = pointTemplateTag.WellKnownIdentityGuid.ToString()
				};

				// archive has not been implemented

				this.Tags.Add(modelTag);

				if (pointTemplateTag.AlarmTemplates.Any())
				{
					AlarmTagHashSet.Add(pointTemplateTag.PointTemplateTagGuid);

					foreach (var alarmTemplate in pointTemplateTag.AlarmTemplates.Values)
					{
						AlarmTagHashSet.Add(alarmTemplate.AlarmStateTemplateTagGuid);

						foreach (var alarmTestTemplate in alarmTemplate.AlarmTestTemplates.Values)
						{
							AlarmTagHashSet.Add(alarmTestTemplate.LimitTemplateTagGuid);
						}
					}
				}
			}

			this.ActionListCategories = new List<SelectListItem>();
			this.ActionListPointTypes = new List<SelectListItem>();
			this.Alarms = new AlarmEditorModel();
			this.PointTypeId = string.Empty;
			this.CategoryId = string.Empty;

		}

		public static string FormattedValue(
			PointTemplate pointTemplate,
			SiteClass site,
			PointTemplateTag pointTemplateTag,
			NumberFormatInfo numberFormatInfo,
			out string valueRaw)
		{
			//Format Value for display
			string formattedValue = "";
			valueRaw = "";

			if (pointTemplateTag.Value != null)
			{
				formattedValue = PointManager.FormatValueFullPrecision(
					Type.GetType(pointTemplateTag.ValueTypeString),
					pointTemplateTag.Units,
					numberFormatInfo,
					pointTemplateTag.Value);
				valueRaw = pointTemplateTag.Value.ToString();

				if (PointTemplateTag.IsNumeric(pointTemplateTag.ValueTypeString) && (int)pointTemplateTag.Units != 27
					 && (int)pointTemplateTag.Units != 19)
				{
					var tagvalue = "";

					if (pointTemplateTag.Value != null)
					{
						tagvalue = pointTemplateTag.Value.ToString();
					}

					double newvalue =	(double)PointManager.ParseValue(Type.GetType("System.Double"), pointTemplateTag.Units, numberFormatInfo, tagvalue);
					if (pointTemplateTag.ValueTypeString == "System.Double")
					{
						// this ensures we do not write in scientific notation to the view
						string formatString = "F" + pointTemplateTag.DecimalPlaces;
						valueRaw = newvalue.ToString(formatString);
					}
					formattedValue = newvalue.ToString("N", numberFormatInfo);
				}
				else if (pointTemplateTag.ValueTypeString == "System.DateTime")
				{
					if (pointTemplateTag.Value != null)
					{
						formattedValue = ((DateTime)pointTemplateTag.Value).ToString(site.ShortDatePattern);
					}
					valueRaw = formattedValue;
				}

				else if (pointTemplateTag.ValueTypeString == "System.DateTimeOffset")
				{
					if (pointTemplateTag.Value != null)
					{
						formattedValue = ((DateTimeOffset)pointTemplateTag.Value).ToString(site.ShortDatePattern + " " + site.TimePattern);
					}
					valueRaw = formattedValue;
				}
				else if (pointTemplateTag.ValueTypeString == "FMBusinessObjects.DataObjects.PointCommandStatusListReference")
				{

					var pclr = ((FMBusinessObjects.DataObjects.PointCommandStatusListReference)pointTemplateTag.Value);
					if ((pclr.PointCommandStatusListGuid == Guid.Empty))
					{
						formattedValue = "";
					}
					else
					{
						formattedValue = "";

						var pointCommandStatusList =
							pointTemplate.PointCommandStatus.CommandStatusLists.FirstOrDefault(
								x => x.CommandStatusListGuid == pclr.PointCommandStatusListGuid);
						if (pointCommandStatusList != null)
						{
							var pointCommandStatusElement =
								pointCommandStatusList.CommandStatusList.FirstOrDefault(x => x.Value == pclr.CurrentValue);
							formattedValue = pointCommandStatusElement.Key ?? "";
						}
						valueRaw = JsonConvert.SerializeObject(pclr);
					}
				}
				else if (pointTemplateTag.ValueTypeString == "FMBusinessObjects.DataObjects.DeviceAlarmMapReference")
				{
					var damr = ((FMBusinessObjects.DataObjects.DeviceAlarmMapReference)pointTemplateTag.Value);
					if (!damr.CurrentValue.HasValue)
					{
						formattedValue = "";
					}
					else
					{
						formattedValue = damr.CurrentValue.ToString();
					}
					valueRaw = JsonConvert.SerializeObject(damr);
				}
				else if (pointTemplateTag.ValueTypeString.IndexOf(
					"FMBusinessObjects.DataObjects.CodedVariables",
					StringComparison.Ordinal) != -1)
				{
					if (pointTemplateTag.Value != null)
					{
						valueRaw = ((int)pointTemplateTag.Value).ToString();
					}
				}
			}
			return formattedValue;
		}

		public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
		{
				var ret = new List<ValidationResult>();

				if (string.IsNullOrEmpty(this.Name))
				{
					ret.Add(new ValidationResult("PointEditor|The Point Name is required.", new[] { nameof(this.Name) }));
				}

				if (this.LevelMaximumRaw < this.LevelMinimumRaw)
				{
					ret.Add(new ValidationResult("PointEditor|Levels Maximum is less than the Minimum", new[] { nameof(this.LevelMaximum), nameof(this.LevelMinimum) }));
				}

				if (this.LevelMaximumRaw == this.LevelMinimumRaw)
				{
					ret.Add(new ValidationResult("PointEditor|Levels Minimum cannot be the same as the Maximum", new[] { nameof(this.LevelMaximum), nameof(this.LevelMinimum) }));
				}

				if (this.TemperatureMaximumRaw < this.TemperatureMinimumRaw)
				{
					ret.Add(new ValidationResult("PointEditor|Temperatures Maximum is less than the Minimum", new[] { nameof(this.TemperatureMaximum), nameof(this.TemperatureMinimum) }));
				}

				if (this.TemperatureMaximumRaw == this.TemperatureMinimumRaw)
				{
					ret.Add(new ValidationResult("PointEditor|Temperatures Minimum cannot be the same as the Maximum", new[] { nameof(this.TemperatureMaximum), nameof(this.TemperatureMinimum) }));
				}

				if (this.DensityMaximumRaw < this.DensityMinimumRaw)
				{
					ret.Add(new ValidationResult("PointEditor|Densities Maximum is less than the Minimum", new[] { nameof(this.DensityMaximum), nameof(this.DensityMinimum) }));
				}

				if (this.DensityMaximumRaw == this.DensityMinimumRaw)
				{
					ret.Add(new ValidationResult("PointEditor|Densities Minimum cannot be the same as the Maximum", new[] { nameof(this.DensityMaximum), nameof(this.DensityMinimum) }));
				}

				if (this.PressureMaximumRaw < this.PressureMinimumRaw)
				{
					ret.Add(new ValidationResult("PointEditor|Pressures Maximum is less than the Minimum", new[] { nameof(this.PressureMaximum), nameof(this.PressureMinimum) }));
				}

				if (this.PressureMaximumRaw == this.PressureMinimumRaw)
				{
					ret.Add(new ValidationResult("PointEditor|Pressures Minimum cannot be the same as the Maximum", new[] { nameof(this.PressureMaximum), nameof(this.PressureMinimum) }));
				}

				if (this.VolumeMaximumRaw < this.VolumeMinimumRaw)
				{
					ret.Add(new ValidationResult("PointEditor|Volumes Maximum is less than the Minimum", new[] { nameof(this.VolumeMaximum), nameof(this.VolumeMinimum) }));
				}

				if (this.VolumeMaximumRaw == this.VolumeMinimumRaw)
				{
					ret.Add(new ValidationResult("PointEditor|Volumes Minimum cannot be the same as the Maximum", new[] { nameof(this.VolumeMaximum), nameof(this.VolumeMinimum) }));
				}

				if (this.MassMaximumRaw < this.MassMinimumRaw)
				{
					ret.Add(new ValidationResult("PointEditor|Mass Maximum is less than the Minimum", new[] { nameof(this.MassMaximum), nameof(this.MassMinimum) }));
				}

				if (this.MassMaximumRaw == this.MassMinimumRaw)
				{
					ret.Add(new ValidationResult("PointEditor|Mass Minimum cannot be the same as the Maximum", new[] { nameof(this.MassMaximum), nameof(this.MassMinimum) }));
				}

				if (this.VelocityMaximumRaw < this.VelocityMinimumRaw)
				{
					ret.Add(new ValidationResult("PointEditor|Level Rates Maximum is less than the Minimum", new[] { nameof(this.VelocityMaximum), nameof(this.VelocityMinimum) }));
				}

				if (this.VelocityMaximumRaw == this.VelocityMinimumRaw)
				{
					ret.Add(new ValidationResult("PointEditor|Level Rates Minimum cannot be the same as the Maximum", new[] { nameof(this.VelocityMaximum), nameof(this.VelocityMinimum) }));
				}

				if (this.VolumetricFlowMaximumRaw < this.VolumetricFlowMinimumRaw)
				{
					ret.Add(new ValidationResult("PointEditor|Volume Rates Maximum is less than the Minimum", new[] { nameof(this.VolumetricFlowMaximum), nameof(this.VolumetricFlowMinimum) }));
				}

				if (this.VolumetricFlowMaximumRaw == this.VolumetricFlowMinimumRaw)
				{
					ret.Add(new ValidationResult("PointEditor|Volume Rates Minimum cannot be the same as the Maximum", new[] { nameof(this.VolumetricFlowMaximum), nameof(this.VolumetricFlowMinimum) }));
				}

				if (this.MassFlowMaximumRaw < this.MassFlowMinimumRaw)
				{
					ret.Add(new ValidationResult("PointEditor|Mass Rates Maximum is less than the Minimum", new[] { nameof(this.MassFlowMaximum), nameof(this.MassFlowMinimum) }));
				}

				if (this.MassFlowMaximumRaw == this.MassFlowMinimumRaw)
				{
					ret.Add(new ValidationResult("PointEditor|Mass Rates Minimum cannot be the same as the Maximum", new[] { nameof(this.MassFlowMaximum), nameof(this.MassFlowMinimum) }));
				}

				return ret;
		}

	}
	#endregion

	#region Point setting list model class
	[Serializable]
	public class PointSettingListModel : FMBaseModel
	{
		#region Constructors
		/// <summary>
		/// This is the default constructor.
		/// </summary>
		public PointSettingListModel()
		{
			this.Init();
		}
		#endregion

		#region Properties
		public string PointName { get; set; }
		public Guid PointGuid { get; set; }
		public List<string> AssignedCategories { get; set; }
		public string PointType { get; set; }

		public string PointGuidStr
		{
			get
			{
				return this.PointGuid.ToString();
			}
			set
			{
				this.PointGuid = Guid.Empty;
				Guid newGuid;

				if (Guid.TryParse(value, out newGuid))
				{
					this.PointGuid = newGuid;
				}
			}
		}
		#endregion

		#region Private methods
		/// <summary>
		/// This method will initialize the object to its initial state.
		/// </summary>
		private void Init()
		{
			this.PointName			= string.Empty;
			this.PointGuid			= Guid.Empty;
			this.PointType			= string.Empty;
			this.AssignedCategories	= new List<string>();
		}
		#endregion
	}
	#endregion

	#region Point Tag Edit Grid Model Class
	[Serializable]
	public class PointTagEditGridModel : FMBaseModel
	{
		public Guid PointTagGuid { get; set; }
		public string Name { get; set; }
		public string Value { get; set; }
		public string ValueRaw { get; set; }
		public EngineeringUnit Unit { get; set; }
		public int? DecimalPlaces { get; set; }
		public EngineeringUnit ServerUnit { get; set; }
		public string Minimum { get; set; }
		public double MinimumRaw { get; set; }
		public string Maximum { get; set; }
		public double MaximumRaw { get; set; }
		public PointTemplateTag.PointTagInputOutputType InputOutputType { get; set; }
		public bool Input { get; set; }
		public bool InhibitInputOutputTypeConfiguration { get; set; }
		public bool InhibitOverride { get; set; }
		public bool Archived { get; set; }
		public bool ApplyPointEngineeringUnits { get; set; }
		public bool ApplyPointDecimalPlaces { get; set; }
		public bool ApplyPointMinimum { get; set; }
		public bool ApplyPointMaximum { get; set; }
		public EngineeringUnitType EngineeringUnitsType { get; set; }
		public string OpcUaNamespaceUri { get; set; }
		public string OpcUaBrowsePath { get; set; }
		public string OpcUaNodeId { get; set; }
		public int? OpcUaPublishingInterval { get; set; }
		public string DataType { get; set; }
		public string WellKnownIdentityGuidString { get; set; }
	}
	#endregion

	#region Point Property Edit 
	[Serializable]
	public class PointPropertyEditModel : FMBaseModel
	{
		public Guid PointPropertyGuid { get; set; }
		public string Name { get; set; }
		public string Value { get; set; }
	}
	#endregion


	#region Point Template Tag Resolve Grid
	[Serializable]
	public class PointTemplateTagResolveGridModel : FMBaseModel
	{
		public string ParameterName { get; set; }
		public string TagName { get; set; }
		public Guid? TagGuid { get; set; }
		public EngineeringUnitType EngineeringUnitsType { get; set; }
	}

	#endregion

	#region Point Template setting Resolve Grid
	[Serializable]
	public class PointTemplateSettingResolveGridModel : FMBaseModel
	{
		public string SettingType { get; set; }
		public string newSettingName { get; set; }
		public Guid? newSettingGuid { get; set; }
	}

	#endregion


	#region Point Default Unit Change History 
	[Serializable]
	public class PointDefaultUnitChangeHistory 
	{
		public string UnitType { get; set; }
		public Boolean PerformConversion { get; set; }
		public int OldUnit { get; set; }
		public int NewUnit { get; set; }
	}
	#endregion

	#region Point Tag Value Conversion Model Class
	[Serializable]
	public class PointTagValueConversionModel : FMBaseModel
	{
		public string id { get; set; }
		public string dataType { get; set; }
		public string value { get; set; }
		public string numDecimals { get; set; }
	}
	#endregion

	#region Point Tag Value Converted Model Class
	[Serializable]
	public class PointTagValueConvertedModel : FMBaseModel
	{
		public string id { get; set; }
		public string formattedValue { get; set; }
		public string rawValue { get; set; }
		public bool success { get; set; }
		public string errorMessage { get; set; }

	}
	#endregion

	#region Point Module Editor Model Class
	[Serializable]
	public class PointModuleEditorModel : FMBaseModel
	{
		public List<PointProperty> Properties { get; set; }
		public List<PointTag> Tags { get; set; }

		public string ID { get; set; }
		public string ModuleTemplateGuid { get; set; }

		public bool HasModifyModuleLibraryRight;
	}
	#endregion
}
