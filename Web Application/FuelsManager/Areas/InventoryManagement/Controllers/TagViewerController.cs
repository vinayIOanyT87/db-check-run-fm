

namespace FuelsManager.Areas.InventoryManagement.Controllers
{
	using System;
	using System.Linq;
	using System.Collections.Generic;
	using System.Globalization;
	using System.Web.Mvc;
	using System.Web.Script.Serialization;
	using System.Web.UI.WebControls;
	using System.Web.Compilation;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.UtilityObjects;

	using FMPointCommon;

	using FuelsManager.Areas.Controllers;
	using FuelsManager.Areas.InventoryManagement.ViewModels;

	using Varec.CommonComponents.EngineeringUnitsLibrary;
	using Opc.Ua;

	public class TagViewerController : FMBaseControllerEx
	{
		protected const string TagViewerID = "TagViewer";

		protected void SaveViewStateSettings(TagViewerModel model)
		{
			if (model != null)
			{
				var tempSite = model.Site;
				var userSettings =
					FMChannelHelper.MakeCall<IUserViewStateSettings, UserViewStateSettingCollection>(x => x.EnumerateBySiteUserClientIpAddressWindowNameAndViewID(this.Security, this.Security.SiteGuid, this.Security.UserGuid, "", TagViewerID));
				if (userSettings == null || userSettings.Count <= 0)
				{
					var userSetting = new UserViewStateSetting(this.Security);
					var tagList = new TagViewerUserViewStateSettings();
					foreach(var value in model.Values)
					{
							tagList.PointTagGuidList.Add(value.PointValueIdentifier.IdentityGuid);
					}
					userSetting.Value = tagList;
					userSetting.ViewID = TagViewerID;
					FMChannelHelper.MakeCall<IUserViewStateSettings>(x => x.Add(this.Security, userSetting));
				}
				else
				{
					var userSetting = userSettings[0];
					var tagList = new TagViewerUserViewStateSettings();
					foreach (var value in model.Values)
					{
							tagList.PointTagGuidList.Add(value.PointValueIdentifier.IdentityGuid);
					}
				if (model.SortOrder != null &&
					model.SortOrder.Count > 0)
				{
					foreach (var ssort in model.SortOrder)
					{
						tagList.SortOrder.Add(ssort);
					}
				}

				userSetting.Value = tagList;
					FMChannelHelper.MakeCall<IUserViewStateSettings>(x => x.Modify(this.Security, userSetting));
				}
			}
		}

		protected TagViewerModel ReadViewStateSettings()
		{
			var model = new TagViewerModel();
			var userSettings = FMChannelHelper.MakeCall<IUserViewStateSettings, UserViewStateSettingCollection>(x => x.EnumerateBySiteUserClientIpAddressWindowNameAndViewID(this.Security, this.Security.SiteGuid, this.Security.UserGuid, "", TagViewerID));
			if (userSettings != null && userSettings.Count > 0)
			{
				if (userSettings.Count != 1)
				{
					throw new ApplicationException(String.Format("Too many user settings records for User {0} Site {1} ID {2}", this.Security.SiteGuid, this.Security.UserGuid, TagViewerID));
				}
				var userSetting = userSettings[0];
				var tvuvss = (TagViewerUserViewStateSettings) userSetting.Value;
				var tagGuidList = tvuvss.PointTagGuidList;
				var count = tagGuidList.Count;
				tagGuidList = FMChannelHelper.MakeCall<IPointTags, List<Guid>>(x => x.EnumerateTagListByPointAccess(this.Security, tagGuidList));

				var tagDictionary = FMChannelHelper.MakeCall<IPointTags, Dictionary<Guid,PointTag>>(x => x.EnumerateByTagList(this.Security, tagGuidList));
				model.Values= new List<PointValue>();

				foreach(var tag in tagDictionary.Values)
				{
					model.Values.Add(new PointValue(tag));
				}

				foreach (var ssort in tvuvss.SortOrder)
				{
					model.SortOrder.Add(ssort);
				}

				if (tagGuidList.Count != count)
				{
					this.SaveViewStateSettings(model);
				}
			}
			return model;

		}

		// GET: InventoryManagement/TagViewer
		[HttpGet]
		public ActionResult TagViewer()
		{
			var model = ReadViewStateSettings();
			this.Session[TagViewerModel.SessionKey] = model;
			model.Site = FMChannelHelper.MakeCall<ISites, SiteClass>(x => x.Get(this.Security, this.Security.SiteGuid, false, false, false));
			model.Format = new NumberFormatInfo
			{
				NumberGroupSizes = model.Site.GetNumberGroupSizes(),
				NumberGroupSeparator = model.Site.NumberGroupSeparator,
				NumberDecimalSeparator = model.Site.NumberDecimalSeparator,
			};


			model.ShortDatePattern = model.Site.ShortDatePattern;
			model.TimePattern = model.Site.TimePattern;


			var pointValueIdentifierList = new List<PointValueIdentifier>(model.Values.Count);

			foreach (var value in model.Values)
			{
				pointValueIdentifierList.Add(value.PointValueIdentifier);
			}

			List< PointValue> pointValueList = null;

			try
			{
				pointValueList = FMChannelHelper.MakeCall<IPointServiceManager, List<PointValue>>(x => x.GetPointValueData(this.Security, pointValueIdentifierList));
			}
			catch
			{
			}

			int index = 0;
			foreach(var value in model.Values)
			{
				if (pointValueList == null || pointValueList[index] == null)
				{
					value.Value = null;
					value.Status = StatusCodes.Bad;
					value.ServerTimeStamp = DateTimeOffset.UtcNow;
					value.SourceTimeStamp = DateTimeOffset.UtcNow;
					value.Access = new PointValueAccess() { View = true, Modify = false, ExceedRange = false, Override = false };
				}

				else
				{
					value.Value = pointValueList[index].Value;
					value.Status = pointValueList[index].Status;
					value.ServerTimeStamp = pointValueList[index].ServerTimeStamp;
					value.SourceTimeStamp = pointValueList[index].SourceTimeStamp;
					value.Access = pointValueList[index].Access;
				}

				index++;
			}

			return this.View(model);
		}

		[HttpGet, ValidateJsonAntiForgeryToken]
		public ActionResult AddPointTags()
		{
			var model = this.Session[TagViewerModel.SessionKey] as TagViewerModel;
			try
			{
				if (model == null)
				{
					throw new Exception("No Model in Session");
				}

				var pointTemplateTypes =
					FMChannelHelper.MakeCall<IApplicationStrings, ApplicationStringCollectionClass>(
						x => x.EnumerateByType(this.Security, STRING_TYPE.POINT_TEMPLATE_TYPE));

				model.PointTemplateTypeList = new List<KeyValuePair<string, string>>();

				// add to the point template type list
				foreach (var pointTemplateType in pointTemplateTypes)
				{
					model.PointTemplateTypeList.Add(
						new KeyValuePair<string, string>(pointTemplateType.IdentityGuid.ToString(), pointTemplateType.ID));
				}

				model.PointTemplateTypeList.Insert(
					0,
					new KeyValuePair<string, string>(Guid.Empty.ToString(), this.GetTranslatedText("{All}")));

				var pointTemplates =
					FMChannelHelper.MakeCall<IPointTemplates, PointTemplateCollection>(
						x => x.EnumerateByType(this.Security, model.PointTemplateTypeGuid));

				model.PointTemplateList = new List<KeyValuePair<Guid, string>>();

				// add to the point template list
				foreach (var pointTemplate in pointTemplates)
				{
					model.PointTemplateList.Add(new KeyValuePair<Guid, string>(pointTemplate.IdentityGuid, pointTemplate.ID));
				}

				model.PointTemplateList.Insert(0, new KeyValuePair<Guid, string>(Guid.Empty, this.GetTranslatedText("{All}")));

				var pointCategories =
					FMChannelHelper.MakeCall<IApplicationStrings, ApplicationStringCollectionClass>(
						x => x.EnumerateByType(this.Security, STRING_TYPE.POINT_CATEGORY));

				model.PointCategoryList = new List<KeyValuePair<Guid, string>>();

				// add to the point category list
				foreach (var pointCategory in pointCategories)
				{
					model.PointCategoryList.Add(new KeyValuePair<Guid, string>(pointCategory.IdentityGuid, pointCategory.ID));
				}

				model.PointCategoryList.Insert(0, new KeyValuePair<Guid, string>(Guid.Empty, this.GetTranslatedText("{All}")));

				// add to the point list
				model.PointList =
					FMChannelHelper.MakeCall<IPoints, List<KeyValuePair<Guid, string>>>(
						x =>
							x.EnumeratePointIdListForSiteTemplateTypeTemplateCategory(
								this.Security,
								this.Security.SiteGuid,
								model.PointTemplateTypeGuid,
								model.PointTemplateGuid,
								model.PointCategoryGuid,
								true));
				model.PointList.Insert(0, new KeyValuePair<Guid, string>(Guid.Empty, string.Empty));

				model.PointTagList = new List<KeyValuePair<Guid, string>>();
				if (model.PointGuid != Guid.Empty)
				{
					var TagExists = false;
					var AtleastOneTagFound = false;
					var pointTagIdDictionary = FMChannelHelper.MakeCall<IPointTags, Dictionary<Guid, string>>(x => x.EnumerateIdByPointGuid(this.Security, model.PointGuid));
					foreach (var pointTagGuid in pointTagIdDictionary.Keys)
					{
						TagExists = false;

						// make sure that the tag is not already in the list before adding to the list of options
						foreach (var value in model.Values)
						{
							if (value.PointGuid == model.PointGuid && value.PointValueIdentifier.IdentityGuid == pointTagGuid)
							{
								TagExists = true;
								break;
							}
						}
						if (TagExists == false)
						{
							AtleastOneTagFound = true;
							model.PointTagList.Add(new KeyValuePair<Guid, string>(pointTagGuid, pointTagIdDictionary[pointTagGuid]));
						}
					}

					if (AtleastOneTagFound == true) model.PointTagList.Insert(0, new KeyValuePair<Guid, string>(Guid.Empty, this.GetTranslatedText("{All}")));
				}
			}
			catch (Exception except)
			{
				this.OnError(except);
				return this.JsonWithErrorMessages(null, JsonRequestBehavior.AllowGet);
			}
			return this.PartialViewWithErrorMessages("AddPointTags", model, JsonRequestBehavior.AllowGet);
		}



		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult ButtonSubmit( string command )
		{
			var model = this.Session[TagViewerModel.SessionKey] as TagViewerModel;

			if (model == null)
			{
				throw new Exception("No Model in Session");
			}

			if (command == "addButton")
			{
				var pointTagGuidStringList = this.Request.Params["PointTagDropDownList"];
				if(!string.IsNullOrEmpty(pointTagGuidStringList))
				{
					var pointTagGuidStrings = pointTagGuidStringList.Split(',');
					if (pointTagGuidStrings.Length > 0)
					{
						var pointTagGuids = new List<Guid>();
						foreach (var pointTagGuidString in pointTagGuidStrings)
						{
							if (pointTagGuidString == Guid.Empty.ToString())
							{
								var pointTagIdDictionary = FMChannelHelper.MakeCall<IPointTags, Dictionary<Guid, string>>(x => x.EnumerateIdByPointGuid(this.Security, model.PointGuid));
								foreach (var tagGuid in pointTagIdDictionary.Keys)
								{
									pointTagGuids.Add(tagGuid);
								}
							}
							else
							{
								pointTagGuids.Add(new Guid(pointTagGuidString));
							}
						}

						if (pointTagGuids.Count > 0)
						{
							var pointValueIdentiferList = new List<PointValueIdentifier>();

							foreach (var pointTagGuid in pointTagGuids)
							{
								pointValueIdentiferList.Add(new PointValueIdentifier() { IdentityGuid = pointTagGuid, PointValueType = PointValueType.Tag, PropertyID = null });
							}

							var pointValueList = FMChannelHelper.MakeCall<IPointServiceManager, List<PointValue>>(x => x.GetPointValueData(this.Security, pointValueIdentiferList));

							foreach (var newPointValue in pointValueList)
							{
								var TagExists = false;

								// only add tags that do not exist
								foreach (var value in model.Values)
								{
									if (value.PointGuid == model.PointGuid
									&&	value.PointValueIdentifier.IdentityGuid == newPointValue.PointValueIdentifier.IdentityGuid)
									{
										TagExists = true;
										break;
									}
								}
								if (TagExists == false)
								{
									model.Values.Add(newPointValue);
								}
							}
						}
					}
				}



				var modStr = this.Request.Params["modStr"];
				if (modStr != null)
				{
					var mod = System.Web.Helpers.Json.Decode<TagViewerModel>(modStr);
					model.SortOrder = mod.SortOrder;
					model.lastScrollPosition = mod.lastScrollPosition;
					model.edititemrowposition = mod.edititemrowposition;

				}
			}

			else if(command == "removeButton")
			{
				foreach (string param in this.Request.Params.Keys)
				{
					if (param == "DeleteAllCheckBox")
					{
						// do nothing under this condition
						// the user can select the deleteall checkbox
						// and then unselect certain rows one at a time so we need to just fall through
						// and only delete the ones that are checked
					}

					else if (param.StartsWith("Delete_"))
					{
						if (this.Request.Params[param] == "true")
						{
							var pointTagGuid = new Guid(param.Replace("Delete_", ""));
							var index = model.Values.FindIndex(x => x.PointValueIdentifier.IdentityGuid == pointTagGuid);
							if (index != -1)
							{
								model.Values.RemoveAt(index);
							}
						}
					}
				}
			}

			SaveViewStateSettings(model);
			return this.View("TagViewer", model);
		}



		[HttpGet, ValidateJsonAntiForgeryToken]
		public  ActionResult PointTemplateTypeSelectionChanged(string pointTemplateTypeGuidString)
		{
			var model = this.Session[TagViewerModel.SessionKey] as TagViewerModel;
			try
			{
				if (model == null)
				{
					throw new Exception("No Model in Session");
				}

				model.PointTemplateTypeGuid = (string.IsNullOrEmpty(pointTemplateTypeGuidString)
														 || pointTemplateTypeGuidString == Guid.Empty.ToString())
					? null
					: (Guid?)new Guid(pointTemplateTypeGuidString);

				var pointTemplates =
					FMChannelHelper.MakeCall<IPointTemplates, PointTemplateCollection>(
						x => x.EnumerateByType(this.Security, model.PointTemplateTypeGuid));

				model.PointTemplateList = new List<KeyValuePair<Guid, string>>();

				foreach (var pointTemplate in pointTemplates)
				{
					model.PointTemplateList.Add(new KeyValuePair<Guid, string>(pointTemplate.IdentityGuid, pointTemplate.ID));
				}

				model.PointTemplateList.Insert(0, new KeyValuePair<Guid, string>(Guid.Empty, this.GetTranslatedText("{All}")));

				if (model.PointCategoryList != null) model.PointCategoryList.Clear();
				if (model.PointList != null) model.PointList.Clear();
				if (model.PointTagList != null) model.PointTagList.Clear();


				SaveViewStateSettings(model);
			}
			catch (Exception except)
			{
				this.OnError(except);
				return this.JsonWithErrorMessages(null, JsonRequestBehavior.AllowGet);
			}
			return JsonWithErrorMessages(model.PointTemplateList, JsonRequestBehavior.AllowGet);
		}

		[HttpGet, ValidateJsonAntiForgeryToken]
		public ActionResult PointTemplateSelectionChanged(string pointTemplateGuidString)
		{
			var model = this.Session[TagViewerModel.SessionKey] as TagViewerModel;
			try
			{
				if (model == null)
				{
					throw new Exception("No Model in Session");
				}


				model.PointTemplateGuid = (string.IsNullOrEmpty(pointTemplateGuidString)
													|| pointTemplateGuidString == Guid.Empty.ToString())
					? null
					: (Guid?)new Guid(pointTemplateGuidString);

				model.PointList =
					FMChannelHelper.MakeCall<IPoints, List<KeyValuePair<Guid, string>>>(
						x =>
							x.EnumeratePointIdListForSiteTemplateTypeTemplateCategory(
								this.Security,
								this.Security.SiteGuid,
								model.PointTemplateTypeGuid,
								model.PointTemplateGuid,
								model.PointCategoryGuid,
								true));
				model.PointList.Insert(0, new KeyValuePair<Guid, string>(Guid.Empty, string.Empty));

				if (model.PointTagList != null) model.PointTagList.Clear();

				SaveViewStateSettings(model);
			}
			catch (Exception except)
			{
				this.OnError(except);
				return this.JsonWithErrorMessages(null, JsonRequestBehavior.AllowGet);
			}
			return JsonWithErrorMessages(model.PointList, JsonRequestBehavior.AllowGet);
		}

		[HttpGet, ValidateJsonAntiForgeryToken]
		public ActionResult PointCategorySelectionChanged(string pointCategoryGuidString)
		{
			var model = this.Session[TagViewerModel.SessionKey] as TagViewerModel;
			try
			{
				if (model == null)
			{
				throw new Exception("No Model in Session");
			}

			model.PointCategoryGuid = (string.IsNullOrEmpty(pointCategoryGuidString) || pointCategoryGuidString == Guid.Empty.ToString()) ? null : (Guid?)new Guid(pointCategoryGuidString);

			model.PointList = FMChannelHelper.MakeCall<IPoints, List<KeyValuePair<Guid, string>>>
			(x => x.EnumeratePointIdListForSiteTemplateTypeTemplateCategory(this.Security, this.Security.SiteGuid, model.PointTemplateTypeGuid, model.PointTemplateGuid, model.PointCategoryGuid, true));
			model.PointList.Insert(0,new KeyValuePair<Guid, string>(Guid.Empty, string.Empty));

			// clear the tag list
			if (model.PointTagList != null)
				model.PointTagList.Clear();

			SaveViewStateSettings(model);
			}
			catch (Exception except)
			{
				this.OnError(except);
				return this.JsonWithErrorMessages(null, JsonRequestBehavior.AllowGet);
			}
			return this.JsonWithErrorMessages(model.PointList, JsonRequestBehavior.AllowGet);
		}

		[HttpGet, ValidateJsonAntiForgeryToken]
		public ActionResult PointSelectionChanged(string pointGuidString, string PointID)
		{
			var model = this.Session[TagViewerModel.SessionKey] as TagViewerModel;
			try
			{
				if (model == null)
				{
					throw new Exception("No Model in Session");
				}


				model.PointID = PointID;
				model.PointGuid = (string.IsNullOrEmpty(pointGuidString)) ? Guid.Empty : new Guid(pointGuidString);
				model.PointTagList = new List<KeyValuePair<Guid, string>>();
				if (model.PointGuid != Guid.Empty)
				{
					var TagExists = false;
					var AtleastOneTagFound = false;
					var pointTagIdDictionary = FMChannelHelper.MakeCall<IPointTags, Dictionary<Guid, string>>(x => x.EnumerateIdByPointGuid(this.Security, model.PointGuid));
					foreach (var pointTagGuid in pointTagIdDictionary.Keys)
					{
						TagExists = false;

						// make sure that the tag is not already in the list before adding to the list of options
						foreach (var value in model.Values)
						{
							if (value.PointGuid == model.PointGuid && value.PointValueIdentifier.IdentityGuid == pointTagGuid)
							{
								TagExists = true;
								break;
							}
						}
						if (TagExists == false)
						{
							AtleastOneTagFound = true;
							model.PointTagList.Add(new KeyValuePair<Guid, string>(pointTagGuid, pointTagIdDictionary[pointTagGuid]));
						}
					}

					if (AtleastOneTagFound == true) model.PointTagList.Insert(0, new KeyValuePair<Guid, string>(Guid.Empty, this.GetTranslatedText("{All}")));
				}

				SaveViewStateSettings(model);
			}
			catch (Exception except)
			{
				this.OnError(except);
				return this.JsonWithErrorMessages(null, JsonRequestBehavior.AllowGet);
			}
			return this.JsonWithErrorMessages(model.PointTagList, JsonRequestBehavior.AllowGet);
		}


		[HttpGet, ValidateJsonAntiForgeryToken]
		public ActionResult Refresh(bool initialRefresh)
		{
			var tagList = new List<ListItem>();

			try
			{
				var restricted = this.GetTranslatedText("Restricted");

				var model = this.Session[TagViewerModel.SessionKey] as TagViewerModel;
				if (model == null)
				{
					throw new Exception("No Model in Session");
				}

				var numberFormatInfo = new NumberFormatInfo
											{
												NumberGroupSizes = model.Site.GetNumberGroupSizes(),
												NumberGroupSeparator = model.Site.NumberGroupSeparator,
												NumberDecimalSeparator = model.Site.NumberDecimalSeparator
											};

				List<PointValueIdentifier> pointValueIdntifierList = new List<PointValueIdentifier>(model.Values.Count);
				foreach (var value in model.Values)
				{
					pointValueIdntifierList.Add(value.PointValueIdentifier);
				}

				var pointValues = FMChannelHelper.MakeCall<IPointServiceManager, List<PointValue>>(x => x.GetPointValueData(this.Security, pointValueIdntifierList));
				int index = 0;

				foreach (var value in model.Values)
				{
					// Row is being edited
					if (value.PointValueIdentifier.IdentityGuid == Guid.Empty)
					{
						index++;
						continue;
					}

					// Tag cannot be read
					if (pointValues == null
					|| pointValues.Count <= index
					||	pointValues[index] == null
					|| pointValues[index].PointGuid == Guid.Empty)
					{
						if (value.Status != StatusCodes.Bad)
						{
							value.Value = null;
							value.Status = StatusCodes.Bad;
							value.ServerTimeStamp = DateTimeOffset.UtcNow;
							value.SourceTimeStamp = DateTimeOffset.UtcNow;
							var limitStatus = "";
							var statusCode = new StatusCode((uint)value.Status);

							tagList.Add(
								new ListItem(
									"Override_" + value.PointValueIdentifier.IdentityGuid.ToString(),
									(value.OpcStatusCodeBits == StatusCodes.GoodLocalOverride).ToString()));

							ListItem OpcStatus = new ListItem("OpcStatus_" + value.PointValueIdentifier.IdentityGuid.ToString(), StatusCode.LookupSymbolicId((uint)value.Status) + limitStatus);
							OpcStatus.Attributes.Add("#FF4040", "#FF4040");
							tagList.Add(OpcStatus);

							tagList.Add(
								new ListItem(
									"Timestamp_" + value.PointValueIdentifier.IdentityGuid.ToString(),
									value.ServerTimeStamp.ToString(model.Site.GetDateTimeFormatInfo())));

							numberFormatInfo.NumberDecimalDigits = value.DecimalPlaces;
							tagList.Add(
								new ListItem("Value_" + value.PointValueIdentifier.IdentityGuid.ToString(), PointManager.FormatValue(Type.GetType(value.ValueTypeString), value.Units, numberFormatInfo, value.Value)));

						}
					}

					else
					{
						if ((pointValues[index].Status != value.Status) || initialRefresh)
						{
							string statusColor = "#9697FA";
							value.Status = pointValues[index].Status;

							tagList.Add(
								new ListItem(
									"Override_" + value.PointValueIdentifier.IdentityGuid.ToString(),
									(value.OpcStatusCodeBits == StatusCodes.GoodLocalOverride).ToString()));

							var limitStatus = "";
							var statusCode = new StatusCode((uint)value.Status);
							if (StatusCode.IsBad(statusCode))
							{
								statusColor = "#FF4040";
							}
							else if (StatusCode.IsGood(statusCode))
							{
								statusColor = "#27AE60";
							}

							if (value.Value is double && (double)value.Value < value.Minimum)
							{
								statusCode.LimitBits = LimitBits.Low;
							}
							else if (value.Value is double && (double)value.Value > value.Maximum)
							{
								statusCode.LimitBits = LimitBits.High;
							}
							value.Status = (long)statusCode;

							if (statusCode.LimitBits == LimitBits.High)
							{
								limitStatus = " Over Range";
							}
							else if (statusCode.LimitBits == LimitBits.Low)
							{
								limitStatus = " Under Range";
							}

							ListItem OpcStatus = new ListItem("OpcStatus_" + value.PointValueIdentifier.IdentityGuid.ToString(), StatusCode.LookupSymbolicId((uint)value.Status) + limitStatus);
							OpcStatus.Attributes.Add(statusColor, statusColor);
							tagList.Add(OpcStatus);
						}

						if ((pointValues[index].ServerTimeStamp != value.ServerTimeStamp) || initialRefresh)
						{
							value.ServerTimeStamp = pointValues[index].ServerTimeStamp;
							value.SourceTimeStamp = pointValues[index].SourceTimeStamp;
							tagList.Add(
								new ListItem(
									"Timestamp_" + value.PointValueIdentifier.IdentityGuid.ToString(),
									value.ServerTimeStamp.ToString(model.Site.GetDateTimeFormatInfo())));
						}

						if ((pointValues[index].Value == null && value.Value != null)
						|| (pointValues[index].Value != null && (value.Value == null || !pointValues[index].Value.Equals(value.Value)))
						|| pointValues[index].DecimalPlaces != value.DecimalPlaces
						|| pointValues[index].Units != value.Units
						|| pointValues[index].Access.View != value.Access.View
						|| pointValues[index].Access.Modify != value.Access.Modify
						|| initialRefresh)
						{
							value.Value = pointValues[index].Value;
							value.DecimalPlaces = pointValues[index].DecimalPlaces;
							value.Units = pointValues[index].Units;
							value.Access.View = pointValues[index].Access.View;
							numberFormatInfo.NumberDecimalDigits = value.DecimalPlaces;

							if (value.Value != null)
							{
								if (value.ValueTypeString.IndexOf("FMBusinessObjects.DataObjects.CodedVariables") > -1)
								{
									tagList.Add(new ListItem("Value_" + value.PointValueIdentifier.IdentityGuid.ToString(), this.GetTranslatedText(FMBusinessObjects.DataObjects.CodedVariables.SelectList.CreateUIString((Enum)value.Value))));
								}
								else if (value.ValueTypeString == "FMBusinessObjects.DataObjects.PointCommandStatusListReference")
								{
									tagList.Add(new ListItem("Value_" + value.PointValueIdentifier.IdentityGuid.ToString(), PointManager.GetCommandStatusKey(value.Value)));
								}
								else if (value.ValueTypeString == "FMBusinessObjects.DataObjects.DeviceAlarmMapReference")
								{
									tagList.Add(new ListItem("Value_" + value.PointValueIdentifier.IdentityGuid.ToString(), PointManager.FormatValue(BuildManager.GetType(value.ValueTypeString, false), value.Units, numberFormatInfo, ((DeviceAlarmMapReference) value.Value).CurrentValue)));
								}
								else if (value.ValueTypeString == "System.DateTime")
								{
									tagList.Add(new ListItem("Value_" + value.PointValueIdentifier.IdentityGuid.ToString(), (value.Value == null) ? "" : ((DateTime)value.Value).ToString(model.Site.ShortDatePattern)));
								}
								else if (value.ValueTypeString == "System.DateTimeOffset")
								{
									var siteTimeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(model.Site.TimeZone);
									value.Value = TimeZoneInfo.ConvertTime((DateTimeOffset)value.Value, siteTimeZoneInfo);
									tagList.Add(new ListItem("Value_" + value.PointValueIdentifier.IdentityGuid.ToString(), (value.Value == null) ? "" : ((DateTimeOffset)value.Value).ToString(model.Site.ShortDatePattern + " " + model.Site.TimePattern)));
								}
								else
								{
									tagList.Add(new ListItem("Value_" + value.PointValueIdentifier.IdentityGuid.ToString(), PointManager.FormatValue(BuildManager.GetType(value.ValueTypeString, false), value.Units, numberFormatInfo, value.Value)));
								}
							}
							else
							{
								if (!pointValues[index].Access.View
								&& !pointValues[index].Access.Modify)
								{
									tagList.Add(new ListItem("Value_" + value.PointValueIdentifier.IdentityGuid.ToString(), restricted));
								}
								else
								{
									tagList.Add(new ListItem("Value_" + value.PointValueIdentifier.IdentityGuid.ToString(), PointManager.FormatValue(BuildManager.GetType(value.ValueTypeString, false), value.Units, numberFormatInfo, value.Value)));
								}
							}

							tagList.Add(new ListItem("Units_" + value.PointValueIdentifier.IdentityGuid.ToString(), EngineeringUnits.GetUnitString(value.Units)));

						}

						if( pointValues[index].Access.Modify != value.Access.Modify
						|| pointValues[index].Access.Override != value.Access.Override
						|| pointValues[index].InputOutputType != value.InputOutputType
						|| initialRefresh)
						{
							value.Access.Modify = pointValues[index].Access.Modify;
							value.Access.Override = pointValues[index].Access.Override;

							var editClass = ((pointValues[index].InputOutputType == PointTemplateTag.PointTagInputOutputType.UnAssigned
												|| !value.Access.Modify
												|| ((!value.Access.Override
												|| value.InhibitOverride)
												&& (value.InputOutputType == PointTemplateTag.PointTagInputOutputType.Calculated
												|| value.InputOutputType == PointTemplateTag.PointTagInputOutputType.OpcUa))) ? "editLinkClassDisabled" : "editLinkClass");
							tagList.Add(new ListItem("Edit_" + value.PointValueIdentifier.IdentityGuid.ToString(), editClass));
						}

						if (pointValues[index].InputOutputType != value.InputOutputType
						|| initialRefresh)
						{
							value.InputOutputType = pointValues[index].InputOutputType;
							tagList.Add(
								new ListItem(
									"Type_" + value.PointValueIdentifier.IdentityGuid.ToString(),
									value.InputOutputType.ToString()));

						}

						if (pointValues[index].Units != value.Units
						|| initialRefresh)
						{
							value.Units = pointValues[index].Units;
							tagList.Add(
								new ListItem(
									"Units_" + value.PointValueIdentifier.IdentityGuid.ToString(),
									EngineeringUnits.GetUnitString(value.Units)));

						}

						// check the other important variables that are missing
						if (pointValues[index].Minimum != value.Minimum ||
							initialRefresh)
						{
							value.Minimum = pointValues[index].Minimum;
						}
						if (pointValues[index].Maximum != value.Maximum ||
							initialRefresh)
						{
							value.Maximum = pointValues[index].Maximum;
						}
					}

					index++;
				}
				// store the data back if it has changed
				this.Session[TagViewerModel.SessionKey] = model;
			}
			catch (Exception except)
			{
				this.OnError(except);
				return this.JsonWithErrorMessages(null, JsonRequestBehavior.AllowGet);
			}

			return this.JsonWithErrorMessages(tagList, JsonRequestBehavior.AllowGet);

		}


		[HttpPost, ValidateJsonAntiForgeryToken]
		public ActionResult SetDatagridSortOrder(string modstring)
		{
			var mod = System.Web.Helpers.Json.Decode<TagViewerModel>(modstring);
			var model = this.Session[TagViewerModel.SessionKey] as TagViewerModel;
			try
			{
				if (mod == null)
				{
					throw new Exception("No Column Sort Information");
				}


				if (model == null)
				{
					throw new Exception("No Model in Session");
				}

				model.SortOrder = mod.SortOrder;
				SaveViewStateSettings(model);
			}
			catch (Exception except)
			{
				this.OnError(except);
				return this.JsonWithErrorMessages(null, JsonRequestBehavior.AllowGet);
			}
			return this.JsonWithErrorMessages(null, JsonRequestBehavior.AllowGet);
		}
	}
}