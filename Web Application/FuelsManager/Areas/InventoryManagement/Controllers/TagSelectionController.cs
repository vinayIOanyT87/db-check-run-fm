
namespace FuelsManager.Areas.InventoryManagement.Controllers
{
	using System;
	using System.Linq;
	using System.Collections.Generic;
	using System.Globalization;
	using System.Web.Mvc;
	using System.Web.Script.Serialization;
	using System.Web.UI.WebControls;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.Constants;
	using FMBusinessObjects.UtilityObjects;

	using FMPointCommon;

	using FuelsManager.Areas.Controllers;
	using FuelsManager.Areas.InventoryManagement.ViewModels;

	using Varec.CommonComponents.EngineeringUnitsLibrary;
	using FMBusinessObjects.Attributes;

	public class TagSelectionController : FMBaseControllerEx
	{
		protected static Guid? MakeGuid(string guidString)
		{
			return string.IsNullOrEmpty(guidString) ? null : (Guid?)new Guid(guidString);
		}

		[HttpPost]
		[ValidateJsonAntiForgeryToken]
		public ActionResult GetPointList(bool showFields, string modelStr)
		{
			TagSelectionModel model;
			try
			{
				if (string.IsNullOrEmpty(modelStr))
				{
					model = TagSelectionController.CreateModel(this.Security, this.UseDataDictionary, true, true, false, true, showFields);
				}
				else
				{

					model = System.Web.Helpers.Json.Decode<TagSelectionModel>(modelStr);
					model.EnableFieldSelection = showFields;
					model.FilterByDataType = false;
				}
			}
			catch (Exception except)
			{
				this.OnError(except);
				return this.JsonWithErrorMessages(null);
			}

			return this.PartialViewWithErrorMessages("../TagSelection/TagSelectionView", model);
		}

		[HttpPost]
		[ValidateJsonAntiForgeryToken]
		public ActionResult GetPointListFilterByDataType(bool showFields, string modelStr, string dataTypeStr, PointValueFieldType fieldFilter)
		{
			TagSelectionModel model;
			try
			{
				if (string.IsNullOrEmpty(modelStr))
				{
					model = TagSelectionController.CreateModel(this.Security, this.UseDataDictionary, true, true, false, true, showFields);
				}
				else
				{

					model = System.Web.Helpers.Json.Decode<TagSelectionModel>(modelStr);
					model.EnableFieldSelection = showFields;
				}
				model.FilterByDataType = true;
				model.DataTypeFilter = dataTypeStr;
				model.FieldFilter = fieldFilter;
			}
			catch (Exception except)
			{
				this.OnError(except);
				return this.JsonWithErrorMessages(null);
			}

			return this.PartialViewWithErrorMessages("../TagSelection/TagSelectionView", model);
		}

		[HttpPost]
		[ValidateJsonAntiForgeryToken]
		public ActionResult GetPointListEx(	bool showValueTypes,
														bool showTags,
														bool showFields,
														bool allowMultiple,
														bool allowPoint,
														string pointId,
														string pointGuidStr,
														string valueId,
														PointValueIdentifier pointValueIdentifier,
														bool applyPointAccess)
		{
			TagSelectionModel model;

			try
			{
				model = TagSelectionController.CreateModelEx(this.Security, this.UseDataDictionary, pointId, pointGuidStr, valueId, pointValueIdentifier, showValueTypes, showTags, allowMultiple, allowPoint, showFields, applyPointAccess);
			}
			catch (Exception except)
			{
				this.OnError(except);
				return this.JsonWithErrorMessages(null);
			}

			return this.PartialViewWithErrorMessages("../TagSelection/TagSelectionView", model);
		}

		[HttpPost]
		[ValidateJsonAntiForgeryToken]
		public ActionResult GetPointListWithPanelTypeContext(bool showFields, string modelStr, string panelTypeStr, string pointTemplateGuidStr)
		{
				TagSelectionModel model;
				try
				{
					if (string.IsNullOrEmpty(modelStr))
					{
						model = TagSelectionController.CreateModel(this.Security, this.UseDataDictionary, true, true, false, true, showFields);
					}
					else
					{

						model = System.Web.Helpers.Json.Decode<TagSelectionModel>(modelStr);
						model.EnableFieldSelection = showFields;
						model.FilterByDataType = false;
				}

					PANELTYPE panelType;
					if (PANELTYPE.TryParse(panelTypeStr, out panelType))
					{
						model.PanelType = panelType;
						if (panelType == PANELTYPE.Detail)
						{
							model.PointTemplateGuid = pointTemplateGuidStr;
							model.PointTemplateTagSelectionIndicator = true;
						}
					}
					else
					{
						model.PanelType = PANELTYPE.Standard;
					}
				}
				catch (Exception except)
				{
					this.OnError(except);
					return this.JsonWithErrorMessages(null);
				}

				return this.PartialViewWithErrorMessages("../TagSelection/TagSelectionView", model);
		}

		[HttpPost]
		[ValidateJsonAntiForgeryToken]
		public ActionResult GetPointListWithPanelTypeContextFilterByDataType(bool showFields, string modelStr, string panelTypeStr, string pointTemplateGuidStr, string dataTypeStr, PointValueFieldType fieldFilter)
		{
			TagSelectionModel model;
			try
			{
				if (string.IsNullOrEmpty(modelStr))
				{
					model = TagSelectionController.CreateModel(this.Security, this.UseDataDictionary, true, true, false, true, showFields);
				}
				else
				{

					model = System.Web.Helpers.Json.Decode<TagSelectionModel>(modelStr);
					model.EnableFieldSelection = showFields;
				}

				model.FilterByDataType = true;
				model.DataTypeFilter = dataTypeStr;
				model.FieldFilter = fieldFilter;

				PANELTYPE panelType;
				if (PANELTYPE.TryParse(panelTypeStr, out panelType))
				{
					model.PanelType = panelType;
					if (panelType == PANELTYPE.Detail)
					{
						model.PointTemplateGuid = pointTemplateGuidStr;
						model.PointTemplateTagSelectionIndicator = true;
					}
				}
				else
				{
					model.PanelType = PANELTYPE.Standard;
				}
			}
			catch (Exception except)
			{
				this.OnError(except);
				return this.JsonWithErrorMessages(null);
			}

			return this.PartialViewWithErrorMessages("../TagSelection/TagSelectionView", model);
		}

		[HttpPost]
		[ValidateJsonAntiForgeryToken]
		public ActionResult GetPointListWithPanelTypeContextEx(bool showValueTypes, bool showTags, bool showFields, bool allowMultiple, bool allowPoint, string pointId, string pointGuidStr, string valueId, PointValueIdentifier pointValueIdentifier, string panelTypeStr, string pointTemplateGuidStr,  bool isPointDetailObject, bool isPointTrendButton, bool applyPointAccess)
		{
				TagSelectionModel model = new TagSelectionModel();
				try
				{
					
					PANELTYPE panelType;
					if (!PANELTYPE.TryParse(panelTypeStr, out panelType))
					{
						panelType = PANELTYPE.Standard;
					}

					switch (panelType)
					{
						case PANELTYPE.Standard:
						{
							model = TagSelectionController.CreateModelEx(this.Security, this.UseDataDictionary, pointId, pointGuidStr, valueId, pointValueIdentifier, showValueTypes, showTags, allowMultiple, allowPoint, showFields, applyPointAccess);
							model.PanelType = panelType;
							break;
						}

						case PANELTYPE.Detail:
						{
							if (isPointDetailObject)
							{
								model = TagSelectionController.CreateModel(
									this.Security,
									this.UseDataDictionary,
									showValueTypes,
									showTags,
									false,
									true,
									showFields);
								model.PointTemplateGuid = pointTemplateGuidStr;
								model.SelectedValues = new List<SelectionAssociations>();
								if (pointValueIdentifier != null)
								{
									model.SelectedValues.Add(new SelectionAssociations(pointValueIdentifier.ToString(), valueId));
									model.ValueType = pointValueIdentifier.PointValueType;
								}
							}
							else
							{
								model = TagSelectionController.CreateModelEx(this.Security, this.UseDataDictionary, pointId, pointGuidStr, valueId, pointValueIdentifier, showValueTypes, showTags, allowMultiple, allowPoint, showFields);
							}
							model.PointTemplateTagSelectionIndicator = isPointDetailObject;
							model.PanelType = panelType;
							model.PointTrendButton = isPointTrendButton;
							if (model.PointTrendButton)
							{
								if (model.PointTemplateList.Exists(x => x.Key == pointGuidStr))
								{
									string pointTemplateName = model.PointTemplateList.Find(x => x.Key == pointGuidStr).Value;
									model.PointTemplateList.Clear();
									model.PointTemplateList.Add(new SelectionAssociations(pointGuidStr, pointTemplateName));
								}
							}
							break;
						}
					}
				}
				catch (Exception except)
				{
					this.OnError(except);
					return this.JsonWithErrorMessages(null);
				}

				return this.PartialViewWithErrorMessages("../TagSelection/TagSelectionView", model);
		}



		public static TagSelectionModel CreateModelEx(
			SecurityClass security,
			bool useDataDictionary,
			string pointId,
			string pointGuidStr,
			string tagId,
			PointValueIdentifier pointValueIdentifier,
			bool enableValueTypeSelection = true,
			bool enableTagSelection = true,
			bool allowMultiple = true,
			bool allowPoint = true,
			bool enableFieldSelection = false,
			bool applyPointAccess = false)
		{
			var model = CreateModel(security, useDataDictionary, enableValueTypeSelection, enableTagSelection, allowMultiple, allowPoint, enableFieldSelection, applyPointAccess);
			model.SelectedValues = new List<SelectionAssociations>();
			if (pointValueIdentifier != null)
			{
				model.SelectedValues.Add(new SelectionAssociations(pointValueIdentifier.ToString(), TranslatedText(tagId, security, useDataDictionary)));
				model.ValueType = pointValueIdentifier.PointValueType;
			}
			model.PointGuid = string.IsNullOrEmpty(pointGuidStr) ? Guid.Empty : new Guid(pointGuidStr);
			model = PopulateModelForPointSelectionChange(security, model, useDataDictionary);
			return model;
		}
		
		public static TagSelectionModel CreateModel(SecurityClass security, bool useDataDictionary, bool enableValueTypeSelection = true, bool enableTagSelection = true, bool allowMultiple = true, bool allowPoint = true, bool enableFieldSelection = false, bool applyPointAccess = false)
		{
			var model = new TagSelectionModel
							{
								AllowMultipleSelect = allowMultiple,
								AllowPointSelect = allowPoint,
								EnableValueTypeSelection = enableValueTypeSelection,
								EnableFieldSelection = enableFieldSelection,
								EnableTagSelection = enableTagSelection,
								ApplyPointAccess = applyPointAccess
							};

			//Site
			var site = FMChannelHelper.MakeCall<ISites, SiteClass>(x => x.Get(security, security.SiteGuid, false, false, false));

			var allTranslated = TranslatedText("{All}", security, useDataDictionary);
			var pointTemplateTypes = FMChannelHelper.MakeCall<IApplicationStrings, ApplicationStringCollectionClass>(x => x.EnumerateByType(security, STRING_TYPE.POINT_TEMPLATE_TYPE));

			model.PointTemplateTypeList = new List<SelectionAssociations>();

			// add to the point template type list
			foreach (var pointTemplateType in pointTemplateTypes)
			{
				model.PointTemplateTypeList.Add(new SelectionAssociations(pointTemplateType.IdentityGuid.ToString(), pointTemplateType.ID));
			}

			model.PointTemplateTypeList.Insert(0, new SelectionAssociations(Guid.Empty.ToString(), allTranslated));

			var pointTemplates = FMChannelHelper.MakeCall<IPointTemplates, PointTemplateCollection>(x => x.EnumerateByType(security, MakeGuid(model.PointTemplateTypeGuid)));

			model.PointTemplateList = new List<SelectionAssociations>();

			// add to the point template list
			foreach (var pointTemplate in pointTemplates)
			{
				model.PointTemplateList.Add(new SelectionAssociations(pointTemplate.IdentityGuid.ToString(), pointTemplate.ID));
			}

			// Add all entry to the beginning of the list.
			model.PointTemplateList.Insert(0, new SelectionAssociations(Guid.Empty.ToString(), allTranslated));

			var pointCategories = FMChannelHelper.MakeCall<IApplicationStrings, ApplicationStringCollectionClass>(x => x.EnumerateByType(security, STRING_TYPE.POINT_CATEGORY));

			model.PointCategoryList = new List<SelectionAssociations>();

			// add to the point category list
			foreach (var pointCategory in pointCategories)
			{
				model.PointCategoryList.Add(new SelectionAssociations(pointCategory.IdentityGuid.ToString(), pointCategory.ID));
			}

			model.PointCategoryList.Insert(0, new SelectionAssociations(Guid.Empty.ToString(), allTranslated));

			// add to the point list
			var pointList = FMChannelHelper.MakeCall<IPoints, List<KeyValuePair<Guid, string>>>
			(x => x.EnumeratePointIdListForSiteTemplateTypeTemplateCategory(security, security.SiteGuid, MakeGuid(model.PointTemplateTypeGuid), MakeGuid(model.PointTemplateGuid), MakeGuid(model.PointCategoryGuid), model.ApplyPointAccess));
			model.PointList = new List<SelectionAssociations>();
			
			foreach (var point in pointList)
			{
				model.PointList.Add(new SelectionAssociations(point.Key.ToString(), point.Value));
			}

			// Add System Data Point virtual Point and add System Data Point virtual point template.
			AddVirtualSystemDataPointInitialPointInfo(model);

			// Add empty point to the beginning of the list.
			model.PointList.Insert(0, new SelectionAssociations(Guid.Empty.ToString(), string.Empty));

			model.ValueList = new List<SelectionAssociations>();
			return model;
		}

		protected ActionResult OkButtonPressForPointTemplate(
			TagSelectionModel model,
			string pointValueIdentifierStringList,
			string pointTagFieldList)
		{
			try
			{
					// Generate Field List and Update Model
					if (!string.IsNullOrEmpty(pointTagFieldList))
					{
						model.Fields.Clear();
						var tagFields = pointTagFieldList.Split(',');
						if (tagFields.Length > 0)
						{
								foreach (var tagField in tagFields)
								{
									int tagFieldInt;
									if (int.TryParse(tagField, out tagFieldInt))
									{
										model.Fields.Add((PointValueFieldType)tagFieldInt);
									}
								}
						}
					}
					
					if (!string.IsNullOrEmpty(pointValueIdentifierStringList))
					{
						var pointValueIdentifierStrings = pointValueIdentifierStringList.Split(',');
						var pointValueIdentifiers = new List<PointValueIdentifier>();
						
						if (pointValueIdentifierStrings.Length > 0)
						{
								foreach (var pointValueIdentifierString in pointValueIdentifierStrings)
								{
									pointValueIdentifiers.Add(new PointValueIdentifier(pointValueIdentifierString));
								}
						}

						//Get the Point Template
						var pointTemplate = FMChannelHelper.MakeCall<IPointTemplates, PointTemplate>(x => x.Get(this.Security, new Guid(model.PointTemplateGuid)));
						var pointValues = pointTemplate.GetPointTemplateValueData(pointValueIdentifiers);

						model.PointValues.Clear();
						model.SelectedValues.Clear();

						if (pointValues != null && pointValues.Count != 0)
						{
								foreach (var pointValue in pointValues)
								{
									model.PointValues.Add(pointValue);
									model.SelectedValues.Add(new SelectionAssociations(new PointValueIdentifier(pointValue).ToString(), pointValue.ID));
								}
						}
						
					}
			
				}
				catch (Exception except)
				{
					this.OnError(except);
					return this.JsonWithErrorMessages(null, JsonRequestBehavior.AllowGet);
				}
				return this.JsonWithErrorMessages(model, JsonRequestBehavior.AllowGet);
		}

		[HttpPost, ValidateJsonAntiForgeryToken]
		public ActionResult OkButtonPress(string pointGuidString, string pointValueIdentifierStringList, string pointTagFieldList, string modelString)
		{
			TagSelectionModel model;
			try
			{
				model = System.Web.Helpers.Json.Decode<TagSelectionModel>(modelString);

				if (model.PointTemplateTagSelectionIndicator)
				{
					return this.OkButtonPressForPointTemplate(model, pointValueIdentifierStringList, pointTagFieldList);
				}

				var tags = new PointTagCollection();
				if (!string.IsNullOrEmpty(pointGuidString))
				{
					var pointGuid = new Guid(pointGuidString);
					model.PointGuid = pointGuid;
					if (!string.IsNullOrEmpty(pointTagFieldList))
					{
						model.Fields.Clear();
						var tagFields = pointTagFieldList.Split(',');
						if (tagFields.Length > 0)
						{
							foreach (var tagField in tagFields)
							{
								int tagFieldInt;
								if (int.TryParse(tagField, out tagFieldInt))
								{
									model.Fields.Add((PointValueFieldType)tagFieldInt);
								}
							}
						}
					}

					if (model.EnableTagSelection)
					{

						if (!string.IsNullOrEmpty(pointValueIdentifierStringList))
						{
							var pointValueIdentifierStrings = pointValueIdentifierStringList.Split(',');
							var pointValueIdentifiers = new List<PointValueIdentifier>();

							if (pointValueIdentifierStrings.Length == 1)
							{
								var pointValueIdentifer = new PointValueIdentifier(pointValueIdentifierStrings[0]);

								if (pointValueIdentifer.IdentityGuid == Guid.Empty)
								{
									if (model.ValueType == PointValueType.Tag)
									{
										var pointValueIdentifierDictionary = FMChannelHelper.MakeCall<IPoints, Dictionary<PointValueIdentifier, string>>(x => x.EnumeratePointValueIdentifiersForPointFilterByType(this.Security, pointGuid, PointValueType.All, model.FilterByDataType, model.DataTypeFilter, model.FieldFilter, model.ApplyPointAccess));
										pointValueIdentifiers = pointValueIdentifierDictionary.Keys.ToList<PointValueIdentifier>(); 
									}

									else if(model.ValueType == PointValueType.Setting)
									{
										var pointValueIdentifierDictionary = FMChannelHelper.MakeCall<IPoints, Dictionary<PointValueIdentifier, string>>(x => x.EnumeratePointValueIdentifiersForPointFilterByType(this.Security, pointGuid, PointValueType.Setting, model.FilterByDataType, model.DataTypeFilter, model.FieldFilter, model.ApplyPointAccess));
										pointValueIdentifiers = pointValueIdentifierDictionary.Keys.ToList<PointValueIdentifier>();
									}
									else if(model.ValueType == PointValueType.Point)
									{
										var pointValueIdentifierDictionary = FMChannelHelper.MakeCall<IPoints, Dictionary<PointValueIdentifier, string>>(x => x.EnumeratePointValueIdentifiersForPointFilterByType(this.Security, pointGuid, PointValueType.Point,model.FilterByDataType,model.DataTypeFilter, model.FieldFilter, model.ApplyPointAccess));
										pointValueIdentifiers = pointValueIdentifierDictionary.Keys.ToList<PointValueIdentifier>();
									}
								}
							}

							if (pointValueIdentifierStrings.Length > 0
							&& pointValueIdentifiers.Count == 0)
							{
								foreach (var pointValueIdentifierString in pointValueIdentifierStrings)
								{
									pointValueIdentifiers.Add(new PointValueIdentifier(pointValueIdentifierString));
								}
							}

							if (pointValueIdentifiers.Count > 0)
							{
								List<PointValue> pointValues;

								// Check for virtual system data point point.
								if (pointGuidString.Equals(SystemDataPointVirtualPoint.PointGuid.ToString()))
								{
									pointValues = this.GetVirtualPointValue(pointValueIdentifiers);									
								}
								else
								{
									pointValues = FMChannelHelper.MakeCall<IPointServiceManager, List<PointValue>>(x => x.GetPointValueData(this.Security, pointValueIdentifiers));
								}

								model.PointValues.Clear();
								model.SelectedValues.Clear();

								if (pointValues != null && pointValues.Count != 0)
								{
									foreach (var pointValue in pointValues)
									{
										if (pointValue.Value != null && (pointValue.Value.ToString() == "NaN" || pointValue.Value.ToString() == "Infinity"))
											pointValue.Value = "";
										model.PointValues.Add(pointValue);
										model.SelectedValues.Add(new SelectionAssociations(new PointValueIdentifier(pointValue).ToString(), pointValue.ID));
									}
								}
							}
						}
					}
					else
					{
						var pointValueIdentifierDictionary = FMChannelHelper.MakeCall<IPoints, Dictionary<PointValueIdentifier, string>> (x => x.EnumeratePointValueIdentifiersForPointFilterByType(this.Security, pointGuid, PointValueType.All, model.FilterByDataType, model.DataTypeFilter, model.FieldFilter, model.ApplyPointAccess));

						if (pointValueIdentifierDictionary.Count > 0)
						{
							var pointValues = FMChannelHelper.MakeCall<IPointServiceManager, List<PointValue>>(x => x.GetPointValueData(this.Security, pointValueIdentifierDictionary.Keys.ToList<PointValueIdentifier>()));

							model.PointValues.Clear();
							model.SelectedValues.Clear();

							if (pointValues != null && pointValues.Count != 0)
							{
								foreach (var pointValue in pointValues)
								{
									model.PointValues.Add(pointValue);
									model.SelectedValues.Add(new SelectionAssociations(new PointValueIdentifier(pointValue).ToString(), pointValue.ID));
								}
							}
						}
					}
				}
			}
			catch (Exception except)
			{
				this.OnError(except);
				return this.JsonWithErrorMessages(null, JsonRequestBehavior.AllowGet);
			}
			return this.JsonWithErrorMessages(model, JsonRequestBehavior.AllowGet);
		}

		[HttpPost, ValidateJsonAntiForgeryToken]
		public ActionResult PointTemplateTypeSelectionChanged(string pointTemplateTypeGuidString, string modelString)
		{
			TagSelectionModel model;
			try
			{
				model = System.Web.Helpers.Json.Decode<TagSelectionModel>(modelString);

				model.PointTemplateTypeGuid = (string.IsNullOrEmpty(pointTemplateTypeGuidString) || pointTemplateTypeGuidString == Guid.Empty.ToString()) ? null : pointTemplateTypeGuidString;

				var pointTemplates = FMChannelHelper.MakeCall<IPointTemplates, PointTemplateCollection>(x => x.EnumerateByType(this.Security, MakeGuid(model.PointTemplateTypeGuid)));

				model.PointTemplateList = new List<SelectionAssociations>();

				foreach (var pointTemplate in pointTemplates)
				{
					model.PointTemplateList.Add(new SelectionAssociations(pointTemplate.IdentityGuid.ToString(), pointTemplate.ID));
				}

				model.PointTemplateList.Insert(0, new SelectionAssociations(Guid.Empty.ToString(), this.GetTranslatedText("{All}")));

				// Add the virtual system data point point template only if the point template type is "System"
				this.AddVirtualSystemDataPointPointTemplate(model);

				if (model.PointCategoryList != null)
				{ 
					model.PointCategoryList.Clear(); 
				}

				if (model.PointList != null)
				{
					model.PointList.Clear();
				}

				if (model.ValueList != null)
				{
					model.ValueList.Clear();
				}
			}
			catch (Exception except)
			{
				this.OnError(except);
				return this.JsonWithErrorMessages(null, JsonRequestBehavior.AllowGet);
			}
			return this.JsonWithErrorMessages(model, JsonRequestBehavior.AllowGet);
		}

		[HttpPost, ValidateJsonAntiForgeryToken]
		public ActionResult PointTemplateSelectionChanged(string pointTemplateGuidString, string modelString)
		{
			TagSelectionModel model;
			try
			{
				model = System.Web.Helpers.Json.Decode<TagSelectionModel>(modelString);

				model.PointTemplateGuid = (string.IsNullOrEmpty(pointTemplateGuidString) || pointTemplateGuidString == Guid.Empty.ToString()) ? null : pointTemplateGuidString;

				var pointList = FMChannelHelper.MakeCall<IPoints, List<KeyValuePair<Guid, string>>>
				(x => x.EnumeratePointIdListForSiteTemplateTypeTemplateCategory(this.Security, this.Security.SiteGuid, MakeGuid(model.PointTemplateTypeGuid), MakeGuid(model.PointTemplateGuid), MakeGuid(model.PointCategoryGuid), true));
				model.PointList = new List<SelectionAssociations>();

				foreach (var point in pointList)
				{
					model.PointList.Add(new SelectionAssociations(point.Key.ToString(), point.Value));
				}

				// Add virtual system data point point.
				this.AddVirtualSystemDataPointPoint(model);

				model.PointList.Insert(0, new SelectionAssociations(Guid.Empty.ToString(), string.Empty));

				if (model.ValueList != null)
				{
					model.ValueList.Clear();
				}
			}
			catch (Exception except)
			{
				this.OnError(except);
				return this.JsonWithErrorMessages(null, JsonRequestBehavior.AllowGet);
			}
			return this.JsonWithErrorMessages(model, JsonRequestBehavior.AllowGet);
		}

		[HttpPost]
		[ValidateJsonAntiForgeryToken]
		public ActionResult PointCategorySelectionChanged(string pointCategoryGuidString, string modelString)
		{
			TagSelectionModel model;
			try
			{
				model = System.Web.Helpers.Json.Decode<TagSelectionModel>(modelString);

				model.PointCategoryGuid = (string.IsNullOrEmpty(pointCategoryGuidString)
													|| pointCategoryGuidString == Guid.Empty.ToString())
					? null
					: pointCategoryGuidString;

				var pointList =
					FMChannelHelper.MakeCall<IPoints, List<KeyValuePair<Guid, string>>>(
						x =>
							x.EnumeratePointIdListForSiteTemplateTypeTemplateCategory(
								this.Security,
								this.Security.SiteGuid,
								MakeGuid(model.PointTemplateTypeGuid),
								MakeGuid(model.PointTemplateGuid),
								MakeGuid(model.PointCategoryGuid),
								true));
				model.PointList = new List<SelectionAssociations>();
				foreach (var point in pointList)
				{
					model.PointList.Add(new SelectionAssociations(point.Key.ToString(), point.Value));
				}
				model.PointList.Insert(0, new SelectionAssociations(Guid.Empty.ToString(), string.Empty));

				// clear the value list
				if (model.ValueList != null)
				{
					model.ValueList.Clear();
				}

			}
			catch (Exception except)
			{
				this.OnError(except);
				return this.JsonWithErrorMessages(null, JsonRequestBehavior.AllowGet);
			}
			return this.JsonWithErrorMessages(model, JsonRequestBehavior.AllowGet);
		}



		protected static TagSelectionModel PopulateModelForPointSelectionChange(SecurityClass security, TagSelectionModel model, bool useDataDictionary)
		{
			if (model.PanelType == PANELTYPE.Detail && GeneralHelpers.IsStringAnNonEmptyGuid(model.PointTemplateGuid) && model.PointTemplateTagSelectionIndicator)
			{
				return PopulateModelForPointSelectionChangePointDetail(security, model, useDataDictionary);
			}
			var valueExists = false;
			var atleastOneValueFound = false;

			if (model.PointGuid != Guid.Empty)
			{
				if (model.ValueType == PointValueType.Tag)
				{
					var pointValueIdentifierDictionary = FMChannelHelper.MakeCall<IPoints, Dictionary<PointValueIdentifier, string>>(x => x.EnumeratePointValueIdentifiersForPointFilterByType(security, model.PointGuid, PointValueType.Tag, model.FilterByDataType, model.DataTypeFilter,model.FieldFilter, model.ApplyPointAccess));
					var pointValueIdentifiers = pointValueIdentifierDictionary.Keys.ToList<PointValueIdentifier>();

					// Use the well known tag data point tags for virtual system data point point.
					AddVirtualSystemDataPointPointIdentifier(model, pointValueIdentifierDictionary, pointValueIdentifiers);

					foreach (var pointValueIdentifier in pointValueIdentifiers)
					{
						valueExists = false;

						foreach (var pointValue in model.PointValues)
						{
							if (pointValue.PointValueIdentifier.IdentityGuid == pointValueIdentifier.IdentityGuid && pointValue.ID == pointValueIdentifierDictionary[pointValueIdentifier])
							{
								valueExists = true;
								break;
							}							
						}

						if (valueExists == false)
						{
							atleastOneValueFound = true;
							model.ValueList.Add(new SelectionAssociations(pointValueIdentifier.ToString(), pointValueIdentifierDictionary[pointValueIdentifier]));
						}
					}
				}

				else if(model.ValueType == PointValueType.Setting)
				{
					var pointValueIdentifierDictionary = FMChannelHelper.MakeCall<IPoints, Dictionary<PointValueIdentifier, string>>(x => x.EnumeratePointValueIdentifiersForPointFilterByType(security, model.PointGuid, PointValueType.Setting, model.FilterByDataType, model.DataTypeFilter, model.FieldFilter, model.ApplyPointAccess));
					var pointValueIdentifiers = pointValueIdentifierDictionary.Keys.ToList<PointValueIdentifier>();

					foreach(var pointValueIdentifier in pointValueIdentifiers)
					{
						valueExists = false;
						foreach (var pointValue in model.PointValues)
						{
							if (pointValue.PointValueIdentifier.IdentityGuid == pointValueIdentifier.IdentityGuid && pointValue.ID == TranslatedText(pointValueIdentifierDictionary[pointValueIdentifier], security, useDataDictionary))
							{
								valueExists = true;
								break;
							}
						}

						if (valueExists == false)
						{
							atleastOneValueFound = true;
							model.ValueList.Add(new SelectionAssociations(pointValueIdentifier.ToString(), TranslatedText(pointValueIdentifierDictionary[pointValueIdentifier], security, useDataDictionary)));
						}
					}
				}

				else if (model.ValueType == PointValueType.Point)
				{
					var pointValueIdentifierDictionary = FMChannelHelper.MakeCall<IPoints, Dictionary<PointValueIdentifier, string>>(x => x.EnumeratePointValueIdentifiersForPointFilterByType(security, model.PointGuid, PointValueType.Point, model.FilterByDataType, model.DataTypeFilter, model.FieldFilter, model.ApplyPointAccess));
					var pointValueIdentifiers = pointValueIdentifierDictionary.Keys.ToList<PointValueIdentifier>();

					foreach (var pointValueIdentifier in pointValueIdentifiers)
					{
						valueExists = false;
						foreach (var pointValue in model.PointValues)
						{
							if (pointValue.PointValueIdentifier.IdentityGuid == pointValueIdentifier.IdentityGuid && pointValue.ID == TranslatedText(pointValueIdentifierDictionary[pointValueIdentifier], security, useDataDictionary))
							{
								valueExists = true;
								break;
							}							
						}
						if (valueExists == false)
						{
							atleastOneValueFound = true;
							model.ValueList.Add(new SelectionAssociations(pointValueIdentifier.ToString(), TranslatedText(pointValueIdentifierDictionary[pointValueIdentifier], security, useDataDictionary)));
						}
					}
				}

				if (atleastOneValueFound == true && model.AllowMultipleSelect == true)
				{
					model.ValueList.Insert(0, new SelectionAssociations(new PointValueIdentifier(PointValueType.Setting).ToString(), TranslatedText("{All}", security, useDataDictionary)));
				}

				var selectedValues = model.SelectedValues;
				model.SelectedValues = new List<SelectionAssociations>();

				foreach (var selectedValue in selectedValues)
				{
					foreach (var value in model.ValueList)
					{
						if (selectedValue.Value == value.Value)
						{
							model.SelectedValues.Add(value);
							break;
						}
					}
				}
			}

			model.ValueList.Sort();

			return model;
		}

		protected static TagSelectionModel PopulateModelForPointSelectionChangePointDetail(SecurityClass security, TagSelectionModel model, bool useDataDictionary)
		{
			var valueExists = false;
			var atleastOneValueFound = false;
			var pointTemplateGuid = Guid.Empty;
			
				if(GeneralHelpers.IsStringAnNonEmptyGuid(model.PointTemplateGuid))
				{
					Guid.TryParse(model.PointTemplateGuid, out pointTemplateGuid);
				}
			

			if (pointTemplateGuid != Guid.Empty)
			{

				if (model.ValueType == PointValueType.Tag)
				{
					var pointValueIdentifierDictionary =
						FMChannelHelper.MakeCall<IPointTemplates, Dictionary<PointValueIdentifier, string>>(
							x => x.EnumeratePointValueIdentifiersForPointTemplateFilterByType(security, pointTemplateGuid, PointValueType.Tag, model.FilterByDataType, model.DataTypeFilter, model.FieldFilter));
					var pointValueIdentifiers = pointValueIdentifierDictionary.Keys.ToList<PointValueIdentifier>();

					foreach (var pointValueIdentifier in pointValueIdentifiers)
					{
						valueExists = false;
						foreach (var pointValue in model.PointValues)
						{
							if (pointValue.PointValueIdentifier.IdentityGuid == pointValueIdentifier.IdentityGuid 
								&& pointValue.ID == pointValueIdentifierDictionary[pointValueIdentifier])
							{
								valueExists = true;
								break;
							}
						}
						if (valueExists == false)
						{
							atleastOneValueFound = true;
							model.ValueList.Add(new SelectionAssociations(pointValueIdentifier.ToString(), pointValueIdentifierDictionary[pointValueIdentifier]));
						}
					}
				}

				else if (model.ValueType == PointValueType.Setting)
				{
					var pointValueIdentifierDictionary =
						FMChannelHelper.MakeCall<IPointTemplates, Dictionary<PointValueIdentifier, string>>(
							x => x.EnumeratePointValueIdentifiersForPointTemplateFilterByType(security, pointTemplateGuid, PointValueType.Setting, model.FilterByDataType, model.DataTypeFilter, model.FieldFilter));
					var pointValueIdentifiers = pointValueIdentifierDictionary.Keys.ToList<PointValueIdentifier>();

					foreach (var pointValueIdentifier in pointValueIdentifiers)
					{
						valueExists = false;
						foreach (var pointValue in model.PointValues)
						{
								if (pointValue.PointValueIdentifier.IdentityGuid == pointValueIdentifier.IdentityGuid
									&& pointValue.ID
									== TranslatedText(pointValueIdentifierDictionary[pointValueIdentifier], security, useDataDictionary))
								{
									valueExists = true;
									break;
								}
						}
						if (valueExists == false)
						{
							atleastOneValueFound = true;
							model.ValueList.Add(new SelectionAssociations(pointValueIdentifier.ToString(), TranslatedText(pointValueIdentifierDictionary[pointValueIdentifier], security, useDataDictionary)));
						}
					}
				}

				else if (model.ValueType == PointValueType.Point)
				{
					var pointValueIdentifierDictionary =
						FMChannelHelper.MakeCall<IPointTemplates, Dictionary<PointValueIdentifier, string>>(
							x => x.EnumeratePointValueIdentifiersForPointTemplateFilterByType(security, pointTemplateGuid, PointValueType.Point, model.FilterByDataType, model.DataTypeFilter, model.FieldFilter));
					var pointValueIdentifiers = pointValueIdentifierDictionary.Keys.ToList<PointValueIdentifier>();

					foreach (var pointValueIdentifier in pointValueIdentifiers)
					{
						valueExists = false;
						foreach (var pointValue in model.PointValues)
						{
								if (pointValue.PointValueIdentifier.IdentityGuid == pointValueIdentifier.IdentityGuid
									&& pointValue.ID
									== TranslatedText(pointValueIdentifierDictionary[pointValueIdentifier], security, useDataDictionary))
								{
									valueExists = true;
									break;
								}
						}

						if (valueExists == false)
						{
							atleastOneValueFound = true;
							model.ValueList.Add(new SelectionAssociations(pointValueIdentifier.ToString(), TranslatedText(pointValueIdentifierDictionary[pointValueIdentifier], security, useDataDictionary)));
						}
					}
				}


				if (atleastOneValueFound == true && model.AllowMultipleSelect == true)
				{
					model.ValueList.Insert(0, new SelectionAssociations(new PointValueIdentifier(PointValueType.Setting).ToString(), TranslatedText("{All}", security, useDataDictionary)));
				}


				var selectedValues = model.SelectedValues;
				model.SelectedValues = new List<SelectionAssociations>();
				foreach (var selectedValue in selectedValues)
				{
					selectedValue.Value = TranslatedText(selectedValue.Value, security, useDataDictionary);

					foreach (var value in model.ValueList)
					{
						if (selectedValue.Value == value.Value)
						{
							model.SelectedValues.Add(value);
							break;
						}
					}
				}
			}

			model.ValueList.Sort();

			return model;
		}

		[HttpPost]
		[ValidateJsonAntiForgeryToken]
		public ActionResult PointOrValueTypeSelectionChanged(string pointGuidString, string valueType, string modelString)
		{

			TagSelectionModel model;
			try
			{
				model = System.Web.Helpers.Json.Decode<TagSelectionModel>(modelString);

				model.PointGuid = (!GeneralHelpers.IsStringAnNonEmptyGuid(pointGuidString))
					? Guid.Empty
					: new Guid(pointGuidString);
				model.PointTemplateGuid = Guid.Empty.ToString();
				model.ValueType = (PointValueType) Enum.Parse(typeof(PointValueType), valueType);
				model.ValueList = new List<SelectionAssociations>();
				model = PopulateModelForPointSelectionChange(this.Security, model, this.UseDataDictionary);

			}
			catch (Exception except)
			{
				this.OnError(except);
				return this.JsonWithErrorMessages(null, JsonRequestBehavior.AllowGet);
			}
			return this.JsonWithErrorMessages(model, JsonRequestBehavior.AllowGet);
		}

		[HttpPost]
		[ValidateJsonAntiForgeryToken]
		public ActionResult PointTemplateCheckBoxOrValueTypeSelectionChanged(string pointTemplateGuidString, string valueType, string modelString)
		{

				TagSelectionModel model;
				try
				{
					model = System.Web.Helpers.Json.Decode<TagSelectionModel>(modelString);

					model.PointTemplateGuid = (!GeneralHelpers.IsStringAnNonEmptyGuid(pointTemplateGuidString))
						? Guid.Empty.ToString()
						: new Guid(pointTemplateGuidString).ToString();
					model.PointGuid = Guid.Empty;
					model.ValueType = (PointValueType)Enum.Parse(typeof(PointValueType), valueType);
					model.ValueList = new List<SelectionAssociations>();
					model = PopulateModelForPointSelectionChange(this.Security, model, this.UseDataDictionary);

				}
				catch (Exception except)
				{
					this.OnError(except);
					return this.JsonWithErrorMessages(null, JsonRequestBehavior.AllowGet);
				}
				return this.JsonWithErrorMessages(model, JsonRequestBehavior.AllowGet);
		}

      #region Private System Data Point methods
      /// <summary>
      /// This method will return a list of point values for a virtual point value identifiers.
      /// </summary>
      /// <param name="pointValueIdentifiers"></param>
      /// <returns>Returns a list of point values.</returns>
      private List<PointValue> GetVirtualPointValue(List<PointValueIdentifier> pointValueIdentifiers)
		{
			var pointValues = new List<PointValue>();

			foreach (PointValueIdentifier pointValueIdentifier in pointValueIdentifiers)
			{
				if (pointValueIdentifier.IdentityGuid == SystemDataPointVirtualPoint.TagSiteDataGuid)
				{
					var pointValue = new PointValue
					{
						PointGuid					= SystemDataPointVirtualPoint.PointGuid
						, PointID					= SystemDataPointVirtualPoint.PointId
						, PointTemplateTagGuid	= SystemDataPointVirtualPoint.TagSiteDataGuid
						, PointValueIdentifier	= pointValueIdentifier
						, ID							= SystemDataPointVirtualPoint.TagSiteDataId
						, ValueTypeString			= "System.String"
					};

					pointValues.Add(pointValue);
				}

				if (pointValueIdentifier.IdentityGuid == SystemDataPointVirtualPoint.TagUserDataGuid)
				{
					var pointValue = new PointValue
					{
						PointGuid					= SystemDataPointVirtualPoint.PointGuid
						, PointID					= SystemDataPointVirtualPoint.PointId
						, PointTemplateTagGuid	= SystemDataPointVirtualPoint.TagUserDataGuid
						, PointValueIdentifier	= pointValueIdentifier
						, ID							= SystemDataPointVirtualPoint.TagUserDataId
						, ValueTypeString			= "System.String"
					};

					pointValues.Add(pointValue);
				}

				if (pointValueIdentifier.IdentityGuid == SystemDataPointVirtualPoint.TagDateTimeDataGuid)
				{
					var pointValue = new PointValue
					{
						PointGuid					= SystemDataPointVirtualPoint.PointGuid
						, PointID					= SystemDataPointVirtualPoint.PointId
						, PointTemplateTagGuid	= SystemDataPointVirtualPoint.TagDateTimeDataGuid
						, PointValueIdentifier	= pointValueIdentifier
						, ID							= SystemDataPointVirtualPoint.TagDateTimeDataId
						, ValueTypeString			= "System.String"
					};

					pointValues.Add(pointValue);
				}

                if (pointValueIdentifier.IdentityGuid == SystemDataPointVirtualPoint.TaglicenseExpiryDataGuid)
                {
                    var pointValue = new PointValue
                    {
                        PointGuid = SystemDataPointVirtualPoint.PointGuid
                        ,
                        PointID = SystemDataPointVirtualPoint.PointId
                        ,
                        PointTemplateTagGuid = SystemDataPointVirtualPoint.TaglicenseExpiryDataGuid
                        ,
                        PointValueIdentifier = pointValueIdentifier
                        ,
                        ID = SystemDataPointVirtualPoint.TagLicenseExpiryDataId
                        ,
                        ValueTypeString = "System.String"
                    };

                    pointValues.Add(pointValue);
                }
            }

			return pointValues;
		}

		/// <summary>
		/// This method will add a virtual System Data Point point to the model if the point template
		/// type Guid is equal to to the virtual point template type Guid.
		/// </summary>
		/// <param name="model">The tag selection model to be updated.</param>
		private void AddVirtualSystemDataPointPoint(TagSelectionModel model)
      {
			// Add virtual system data point point.
			if (string.IsNullOrEmpty(model.PointTemplateGuid) || model.PointTemplateGuid.Equals(SystemDataPointVirtualPoint.PointTemplateGuid.ToString()))
			{
				model.PointList.Add(new SelectionAssociations(SystemDataPointVirtualPoint.PointGuid.ToString(), SystemDataPointVirtualPoint.PointId));
			}
		}

		/// <summary>
		/// This method will add a virtual System Data Point point template to the mode if the point template
		/// type is equal to the virtual System Data Point point template type Guid.
		/// </summary>
		/// <param name="model">The tag selection model to be updated.</param>
		private void AddVirtualSystemDataPointPointTemplate(TagSelectionModel model)
      {
			// Add the virtual system data point point template only if the point template type is "System"
			if (string.IsNullOrEmpty(model.PointTemplateTypeGuid) == false && model.PointTemplateTypeGuid.Equals(SystemDataPointVirtualPoint.PointTemplateTypeGuid.ToString()))
			{
				model.PointTemplateList.Add(new SelectionAssociations(SystemDataPointVirtualPoint.PointTemplateGuid.ToString(), SystemDataPointVirtualPoint.PointTemplateId));
			}
		}

		/// <summary>
		/// This method will add the initial virtual System Data Point point template Guid and point Guid to the model.
		/// </summary>
		/// <param name="model">The tag selection model to be updated.</param>
		private static void AddVirtualSystemDataPointInitialPointInfo(TagSelectionModel model)
      {
			// Add System Data Point virtual point template.
			model.PointTemplateList.Add(new SelectionAssociations(SystemDataPointVirtualPoint.PointTemplateGuid.ToString(), SystemDataPointVirtualPoint.PointTemplateId));

			// Add System Data Point virtual Point
			model.PointList.Add(new SelectionAssociations(SystemDataPointVirtualPoint.PointGuid.ToString(), SystemDataPointVirtualPoint.PointId));
		}

		/// <summary>
		/// This method will add a virtual System Data Point point value identifier if the point is equal
		/// to the System Data Point virtual point Guid.
		/// </summary>
		/// <param name="model">The tag selection model.</param>
		/// <param name="pointValueIdentifierDictionary">The point value identifier dictionary to be populated.</param>
		/// <param name="pointValueIdentifiers">The point value identifier to be populated.</param>
		private static void AddVirtualSystemDataPointPointIdentifier(TagSelectionModel model 
																						, Dictionary<PointValueIdentifier, string> pointValueIdentifierDictionary
																						, List<PointValueIdentifier> pointValueIdentifiers)
		{
			// Use the well known tag data point tags for virtual system data point point.
			if (model.PointGuid == SystemDataPointVirtualPoint.PointGuid)
			{
				var pointValueIdentifier = new PointValueIdentifier { IdentityGuid = SystemDataPointVirtualPoint.TagSiteDataGuid };
				pointValueIdentifierDictionary.Add(pointValueIdentifier, SystemDataPointVirtualPoint.TagSiteDataId);
				pointValueIdentifiers.Add(pointValueIdentifier);

				pointValueIdentifier = new PointValueIdentifier { IdentityGuid = SystemDataPointVirtualPoint.TagUserDataGuid };
				pointValueIdentifierDictionary.Add(pointValueIdentifier, SystemDataPointVirtualPoint.TagUserDataId);
				pointValueIdentifiers.Add(pointValueIdentifier);

				pointValueIdentifier = new PointValueIdentifier { IdentityGuid = SystemDataPointVirtualPoint.TagDateTimeDataGuid };
				pointValueIdentifierDictionary.Add(pointValueIdentifier, SystemDataPointVirtualPoint.TagDateTimeDataId);
				pointValueIdentifiers.Add(pointValueIdentifier);
			}
		}
      #endregion
   }
}