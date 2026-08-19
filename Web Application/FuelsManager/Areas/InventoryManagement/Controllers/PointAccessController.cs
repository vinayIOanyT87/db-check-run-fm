using System;
using System.Web;

namespace FuelsManager.Areas.InventoryManagement.Controllers
{
	using System.Collections.Generic;
	using System.Linq;
    using System.Web.Mvc;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;

	using FuelsManager.Areas.Controllers;
	using FuelsManager.Areas.InventoryManagement.ViewModels;
    using Interop.DataObjects;
    using Microsoft.Ajax.Utilities;
    using Newtonsoft.Json;

	[SessionState(System.Web.SessionState.SessionStateBehavior.ReadOnly)]
	public class PointAccessController : FMBaseControllerEx
	{

		[HttpGet]
		public ActionResult PointAccess( string id )
		{
            var model = new PointAccessModel();
            try
				{
					 

					 var pointAccessGroupDictionary = FMChannelHelper.MakeCall<IPointAccessGroups, Dictionary<Guid, PointAccessGroup>>(x => x.Enumerate(this.Security));
					 model.PointAccessGroupList = pointAccessGroupDictionary.Select(item => item.Value).ToList();

					 model.UserList = FMChannelHelper.MakeCall<IUsers, UserCollectionClass>(x => x.Enumerate(this.Security));
					 model.UserGroupList = FMChannelHelper.MakeCall<IGroups, GroupCollectionClass>(x => x.Enumerate(this.Security));
					 model.HasPointAccessModifyRight = this.Security.HasRight(RIGHT.MODIFY_POINT_ACCESS_GROUP);
					 if (id == "1") model.InitialView = PointAccessModel.PointDetailViewMode.UserGroupView;
					 if (id == "2") model.InitialView = PointAccessModel.PointDetailViewMode.UserView;

					 
				}
				catch
				(Exception ex)
				{
                this.OnError(ex);
					 if (ex.Message == "Access Denied")
						  return new HttpStatusCodeResult(System.Net.HttpStatusCode.Forbidden);
					 else 
						  return new HttpStatusCodeResult(System.Net.HttpStatusCode.InternalServerError);
            }
            return this.View(model);
        }

        [HttpGet]
		public ActionResult PointAccessGroupDetail(Guid id)
		{
			return this.PointAccessDetail(id, PointAccessDetailModel.PointDetailViewMode.PointGroupView);
		}

		[HttpGet]
		public ActionResult PointAccessUserGroupDetail(Guid id)
		{
			return this.PointAccessDetail(id, PointAccessDetailModel.PointDetailViewMode.UserGroupView);
		}

		[HttpGet]
		public ActionResult PointAccessUserDetail(Guid id)
		{
			return this.PointAccessDetail(id, PointAccessDetailModel.PointDetailViewMode.UserView);
		}

		/// <summary>
		/// This method will remove the movement templates if the movement hardware key is
		/// not set.
		/// </summary>
		/// <param name="pointTemplates">The list of templates.</param>
		private void FilterMovementsOnMovementKey(ref PointTemplateCollection pointTemplates)
		{
			bool isMovementKey = FMChannelHelper.MakeCall<IHardwareKey, bool>(x => x.IsMovementKey());

			if(isMovementKey == false && pointTemplates != null && pointTemplates.Count > 0)
            {
				PointTemplate standardMovement = pointTemplates.Find(x => x.ID.ToUpper() == "STANDARD MOVEMENT");
				PointTemplate standardMovementNode = pointTemplates.Find(x => x.ID.ToUpper() == "STANDARD MOVEMENT NODE VOL");

				if(standardMovement != null)
                {
					pointTemplates.Remove(standardMovement);
				}

				if (standardMovementNode != null)
				{
					pointTemplates.Remove(standardMovementNode);
				}
			}
		}

		private ActionResult PointAccessDetail(Guid id, PointAccessDetailModel.PointDetailViewMode mode)
		{
			var model = new PointAccessDetailModel();
			model.ScreenMode = mode;

			var pointTemplates = FMChannelHelper.MakeCall<IPointTemplates, PointTemplateCollection>(x => x.EnumerateByType(this.Security, null));

			this.FilterMovementsOnMovementKey(ref pointTemplates);

			var points = FMChannelHelper.MakeCall<IPoints, PointCollection>(x => x.EnumerateForSummaryWithCategories(this.Security, this.Security.SiteGuid, false));

			model.HasPointAccessModifyRight = this.Security.HasRight(RIGHT.MODIFY_POINT_ACCESS_GROUP);

			var tagAssociatedWithDeviceAlarmMapDictionary = FMChannelHelper.MakeCall<IPointTags, Dictionary<Guid, PointTag>>(x => x.EnumerateTagsAssociatedWithDeviceAlarmMapBySiteGuid(this.Security, this.Security.SiteGuid));

			// populate the list of point template/points/settings/tags and alarms that are in the system
			this.PopulateBasicModel(model, pointTemplates, points, tagAssociatedWithDeviceAlarmMapDictionary);

			// if we are working with a Point Group 
			if (mode == PointAccessDetailModel.PointDetailViewMode.PointGroupView)
			{
				var pointAccessGroup = FMChannelHelper.MakeCall<IPointAccessGroups, PointAccessGroup>(x => x.Get(this.Security, id));
				model.IdentityGuid = id;
				model.Name = pointAccessGroup.ID;

				ApplicationStringCollectionClass categories = FMChannelHelper.MakeCall<IApplicationStrings, ApplicationStringCollectionClass>(x => x.EnumerateByType(this.Security, STRING_TYPE.POINT_CATEGORY));
                model.CategoryList = new List<PointAccessDetailCategory> ();
				foreach(var applicationStringCategory in categories) {
                    model.CategoryList.Add( new PointAccessDetailCategory
                    {
                        Id = applicationStringCategory.ID,
                        IdentityGuid = applicationStringCategory.IdentityGuid
                    });
                }

                model.UserGroupList = FMChannelHelper.MakeCall<IGroups, GroupCollectionClass>(x => x.Enumerate(this.Security));
				model.Users = FMChannelHelper.MakeCall<IUsers, UserCollectionClass>(x => x.Enumerate(this.Security));
				model.UsertoGroupMap = FMChannelHelper.MakeCall<IUserGroupMaps, UserGroupMapCollectionClass>(x => x.EnumerateBySite(this.Security, this.Security.SiteGuid));
				this.PopulatePointAccessGroupAssignments(pointAccessGroup, model);
			}
			else if (mode == PointAccessDetailModel.PointDetailViewMode.UserGroupView)
			{
				var userGroup = FMChannelHelper.MakeCall<IGroups, GroupClass>( x => x.Get(this.Security, id));
				model.Users = FMChannelHelper.MakeCall<IUsers, UserCollectionClass>(x => x.EnumerateByGroup(this.Security, id));
				model.IdentityGuid = id;
				model.Name = userGroup.ID;

                ApplicationStringCollectionClass categories = FMChannelHelper.MakeCall<IApplicationStrings, ApplicationStringCollectionClass>(x => x.EnumerateByType(this.Security, STRING_TYPE.POINT_CATEGORY));
                model.CategoryList = new List<PointAccessDetailCategory>();
                foreach (var applicationStringCategory in categories)
                {
                    model.CategoryList.Add(new PointAccessDetailCategory
                    {
                        Id = applicationStringCategory.ID,
                        IdentityGuid = applicationStringCategory.IdentityGuid
                    });
                }

                var pointAccessGroups = FMChannelHelper.MakeCall<IPointAccessGroups, List<PointAccessGroup>>(x => x.EnumerateByUserGroup(this.Security, id ));
				foreach (var pointAccessGroup in pointAccessGroups)
				{
					this.PopulatePointAccessGroupAssignments(pointAccessGroup, model);
				}
			}
			else if (mode == PointAccessDetailModel.PointDetailViewMode.UserView)
			{
				var user = FMChannelHelper.MakeCall<IUsers, UserClass>( x => x.Get(this.Security, id));
				model.IdentityGuid = id;
				model.Name = user.Name.IsNullOrWhiteSpace() ? user.ID : user.Name;
				foreach (var userGroupMap in user.UserGroupMapCollection)
				{
					var pointAccessGroupGuidList = new List<Guid>();
					var pointAccessGroups = FMChannelHelper.MakeCall<IPointAccessGroups, List<PointAccessGroup>>(x => x.EnumerateByUserGroup(this.Security, userGroupMap.GroupGuid));
					foreach (var pointAccessGroup in pointAccessGroups)
					{
						// add the point access group to the model if its not there yet
						if (model.PointAccessGroupAssignmentList.All(x => x.PointAccessGroupGuid != pointAccessGroup.PointAccessGroupGuid))
						{
							this.PopulatePointAccessGroupAssignments(pointAccessGroup, model);
						}
						pointAccessGroupGuidList.Add(pointAccessGroup.PointAccessGroupGuid);
					}
					model.UserGroupToPointAccessGroupMap.Add(userGroupMap.GroupID, pointAccessGroupGuidList);
				}

				ApplicationStringCollectionClass categories = FMChannelHelper.MakeCall<IApplicationStrings, ApplicationStringCollectionClass>(x => x.EnumerateByType(this.Security, STRING_TYPE.POINT_CATEGORY));
				model.CategoryList = new List<PointAccessDetailCategory>();
				foreach (var applicationStringCategory in categories)
				{
					model.CategoryList.Add(new PointAccessDetailCategory
					{
						Id = applicationStringCategory.ID,
						IdentityGuid = applicationStringCategory.IdentityGuid
					});
				}
			}
			return this.View("PointAccessDetail", model);
		}

		private void PopulatePointAccessGroupAssignments(PointAccessGroup pointGroup, PointAccessDetailModel model)
		{
			var pointAccessGroupAssignment = new PointAccessGroupAssignment();
			pointAccessGroupAssignment.PointAccessGroupGuid = pointGroup.PointAccessGroupGuid;
			pointAccessGroupAssignment.Name = pointGroup.ID;
			pointAccessGroupAssignment.PointAccessGroupToPointTemplateAssignmentList =
				pointGroup.PointAccessGroupToPointTemplateMapList
					.Select(
						x =>
							new PointAccessGroupToPointTemplateAssignment
							{
								PointTemplateGuid = x.PointTemplateGuid,
								PointAccessGroupToPointTemplateGuid = x.PointAccessGroupToPointTemplateGuid,
								Assigned = x.Assigned,
							})
					.ToList();
			pointAccessGroupAssignment.PointAccessGroupToPointAssignmentList =
				pointGroup.PointAccessGroupToPointMapList.Select(
					y =>
						new PointAccessGroupToPointAssignment
						{
							PointGuid = y.PointGuid,
							PointTemplateGuid = y.PointTemplateGuid,
							PointAccessGroupToPointGuid = y.PointAccessGroupToPointGuid,
							Assigned = y.Assigned,
						}).ToList();

			pointAccessGroupAssignment.PointAccessGroupToSettingAssignmentList = pointGroup.PointAccessGroupToExposedSettingMapList.
								Select( x => new PointAccessGroupToSettingAssignment {
									PointAccessGroupToExposedSettingGuid = x.PointAccessGroupToExposedSettingGuid,
									PointTemplateGuid = x.PointTemplateGuid,
									ExposedSettingGuid = x.ExposedSettingGuid,
									PropertyID = x.PropertyID,
									View = x.View,
									Modify = x.Modify
								}).ToList();

			pointAccessGroupAssignment.PointAccessGroupToTagAssignmentList = pointGroup.PointAccessGroupToTagMapList.
							Select( x => new PointAccessGroupToTagAssignment {
								PointTagGuid = x.PointTemplateTagGuid,
								PointTemplateGuid = x.PointTemplateGuid,
								PointAccessGroupToTagGuid = x.PointAccessGroupToTagGuid,
								View = x.View,
								Modify = x.Modify,
								ExceedRange = x.ExceedRange,
								Override = x.Override
							}).ToList();

			pointAccessGroupAssignment.PointAccessGroupToAlarmTestAssignmentList = pointGroup.PointAccessGroupToAlarmTestMapList.
							Select( x => new PointAccessGroupToAlarmTestAssignment {
								PointAccessGroupToAlarmTestGuid = x.PointAccessGroupToAlarmTestGuid,
								PointTemplateGuid = x.PointTemplateGuid,
								AlarmTestTemplateGuid = x.AlarmTestTemplateGuid,
								View = x.View,
								Acknowledge = x.Acknowledge
							}).ToList();

			pointAccessGroupAssignment.PointAccessGroupToUserGroupAssignmentList = pointGroup.PointAccessGroupToUserGroupMapList.
						Select( x => new PointAccessGroupToUserGroupAssignment {
								UserGroupGuid = x.UserGroupGuid,
								PointAccessGroupToUserGroupGuid = x.PointAccessGroupToUserGroupGuid,
								Assigned = x.Assigned
						}).ToList();

			pointAccessGroupAssignment.PointAccessGroupToPointAlarmTestAssignmentList = pointGroup.PointAccessGroupToPointAlarmTestMapList.
						Select( x => new PointAccessGroupToPointAlarmTestAssignment {
								PointAccessGroupToPointAlarmTestGuid = x.PointAccessGroupToPointAlarmTestGuid,
								AlarmTestGuid = x.AlarmTestGuid,
								PointGuid = x.PointGuid,
								View = x.View,
								Acknowledge = x.Acknowledge
						}).ToList();

			pointAccessGroupAssignment.PointAccessGroupToPointTagAssignmentList = pointGroup.PointAccessGroupToPointTagMapList.
						Select(x => new PointAccessGroupToPointTagAssignment
						{
							PointAccessGroupToPointTagGuid = x.PointAccessGroupToPointTagGuid,
							PointTagGuid = x.PointTagGuid,
							PointGuid = x.PointGuid,
							View = x.View,
							Modify = x.Modify,
							ExceedRange = x.ExceedRange,
							Override = x.Override
						}).ToList();



			model.PointAccessGroupAssignmentList.Add(pointAccessGroupAssignment);
		}

		private void PopulateBasicModel(
			PointAccessDetailModel model,
			PointTemplateCollection pointTemplates,
			PointCollection points,
			Dictionary<Guid, PointTag> tagAssociatedWithDeviceAlarmMapDictionary)
		{
			model.PointTemplateList =
				pointTemplates.Select(
					x =>
						new PointAccessDetailPointTemplate
						{
							PointTemplateId = x.ID,
							PointTemplateGuid = x.PointTemplateGuid,
							ProfileImageGuid = x.ProfileImageGuid
						}).ToList();

			// get dictionary of Device Alarm Map Status and Limit Tags
			var damTagDictionary = new Dictionary<Guid, PointTemplateTag>();

			foreach (var pointTemplate in pointTemplates)
			{
				foreach (var tag in pointTemplate.Tags.Values)
				{
					if (tag.ValueTypeString == "FMBusinessObjects.DataObjects.DeviceAlarmMapReference")
					{
						damTagDictionary.Add(tag.PointTemplateTagGuid, pointTemplate.Tags[tag.PointTemplateTagGuid]);
						foreach (var alarm in tag.AlarmTemplates.Values)
						{
							damTagDictionary.Add(alarm.AlarmStateTemplateTagGuid, pointTemplate.Tags[alarm.AlarmStateTemplateTagGuid]);
							foreach (var alarmTest in alarm.AlarmTestTemplates.Values)
							{
								damTagDictionary.Add(alarmTest.LimitTemplateTagGuid, pointTemplate.Tags[alarmTest.LimitTemplateTagGuid]);
							}
						}

						// set an empty AlarmTemplates to exclude DAM Alarm Tests from configuration
						tag.AlarmTemplates = new Dictionary<Guid, AlarmTemplate>();
					}
				}
			}



			// get list of points
			// to get the name of the template we can join the point List with the template list
			model.PointList = (from p in points
									 join pt in pointTemplates on p.PointTemplateGuid equals pt.PointTemplateGuid
									 select
										 new PointAccessDetailPoint
										 {
											 PointGuid = p.PointGuid,
											 PointId = p.PointId,
											 ProfileImageGuid = p.ProfileImageGuid,
											 PointTemplateGuid = p.PointTemplateGuid,
											 PointTemplateId = pt.PointId,
											 Categories = ";" + string.Join(";", p.PointCategoryCollection.Select(x => x.ApplicationStringGuid.ToString())) + ";",
											 HasDeviceAccessMapTags = damTagDictionary.Values.ToList().Exists(x => x.PointTemplateGuid == p.PointTemplateGuid)
										 }).ToList();



			// get list of point template tags
			var tagList =
				pointTemplates.SelectMany(x => x.Tags.Values)
					.Where(y => !damTagDictionary.ContainsKey(y.PointTemplateTagGuid))
					.Select(
						y =>
							new PointAccessDetailTag
							{
								TagGuid = y.PointTemplateTagGuid,
								TagId = y.ID,
								PointTemplateGuid = y.PointTemplateGuid,
								PointTemplateId = "",
								IsDeviceAlarmMapTag = false,
							})
					.ToList();


			// to get the name of the template we can join the tag List with the template list
			model.TagList =
				tagList.Join(
					pointTemplates,
					tag => tag.PointTemplateGuid,
					pointTemplate => pointTemplate.PointTemplateGuid,
					(tag, pointTemplate) => new { tag, pointTemplate })
					.Select(
						x =>
							new PointAccessDetailTag
							{
								TagGuid = x.tag.TagGuid,
								TagId = x.tag.TagId,
								PointTemplateGuid = x.tag.PointTemplateGuid,
								PointTemplateId = x.pointTemplate.PointId,
								IsDeviceAlarmMapTag = x.tag.IsDeviceAlarmMapTag
							})
					.ToList();

			// Get the list of PointTags
			tagList =
				tagAssociatedWithDeviceAlarmMapDictionary.Values.Join(
					model.PointList,
					tag => tag.PointGuid,
					point => point.PointGuid,
					(tag, point) => new { tag, point })
					.Select(
						x =>
							new PointAccessDetailTag
							{
								TagGuid = x.tag.PointTagGuid,
								TagId = x.tag.ID,
								PointTemplateGuid = x.point.PointTemplateGuid,
								PointTemplateId = x.point.PointTemplateId,
								PointGuid = x.point.PointGuid,
								PointId = x.point.PointId,
								IsDeviceAlarmMapTag = true
							})
					.ToList();

			model.TagList.AddRange(tagList);

			// get the list of point template alarm tests
			var alarmTestList =
				pointTemplates.SelectMany(x => x.Tags.Values)
						.SelectMany(tag => tag.AlarmTemplates.Values)
						.SelectMany(alarmTemplates => alarmTemplates.AlarmTestTemplates.Values)
						.Select(
						alarmTest =>
							new PointAccessDetailAlarmTest
							{
								PointAlarmTestId = alarmTest.ID,
								PointAlarmTestGuid = alarmTest.AlarmTestTemplateGuid,
								TagId = "",
								TagGuid = alarmTest.PointTemplateTagGuid,
								PointTemplateId = "",
								PointTemplateGuid = alarmTest.PointTemplateGuid,
								PointId = "",
								PointGuid = new Guid("00000000-0000-0000-0000-000000000000"),
								})
					.ToList();

			// get the point template name and tag name by joining to the tag list since it has already the names resolved
			model.AlarmTestList = (from a in alarmTestList
				join t in model.TagList on new { a.TagGuid, a.PointTemplateGuid } equals
				new { t.TagGuid, t.PointTemplateGuid }
				select
				new PointAccessDetailAlarmTest
				{
					PointAlarmTestId = a.PointAlarmTestId,
					PointAlarmTestGuid = a.PointAlarmTestGuid,
					TagId = t.TagId,
					TagGuid = a.TagGuid,
					PointTemplateId = t.PointTemplateId,
					PointTemplateGuid = a.PointTemplateGuid,
					PointId = a.PointId,
					PointGuid = a.PointGuid,
					IsDeviceAlarmMapAlarmTest = t.IsDeviceAlarmMapTag
				}).Where(x => !x.IsDeviceAlarmMapAlarmTest).ToList();

			// get the point Alarm Test
			var pointAlarmTestList =
				tagAssociatedWithDeviceAlarmMapDictionary.Values.Where(x => x.ValueTypeString == "FMBusinessObjects.DataObjects.DeviceAlarmMapReference")
				.SelectMany(x => x.Alarms.Values)
				.SelectMany(alarm => alarm.AlarmTests.Values, (alarm, alarmTest) => new { alarm, alarmTest })
				.Select(
					alarmandtest =>
					new PointAccessDetailAlarmTest
					{
						PointAlarmTestId = alarmandtest.alarmTest.ID,
						PointAlarmTestGuid = alarmandtest.alarmTest.AlarmTestGuid,
						TagId = alarmandtest.alarm.InputTagID,
						TagGuid = alarmandtest.alarm.InputTagGuid,
						PointTemplateId = "",
						PointTemplateGuid = new Guid("00000000-0000-0000-0000-000000000000"),
						PointId = "",
						PointGuid = new Guid("00000000-0000-0000-0000-000000000000"),
						IsDeviceAlarmMapAlarmTest= true,
					})
					.ToList();

			pointAlarmTestList = (from a in pointAlarmTestList
								  join t in model.TagList on new { a.TagGuid } equals
												new { t.TagGuid }
								  select
									  new PointAccessDetailAlarmTest
									  {
										  PointAlarmTestId = a.PointAlarmTestId,
										  PointAlarmTestGuid = a.PointAlarmTestGuid,
										  TagId = a.TagId,
										  TagGuid = a.TagGuid,
										  PointTemplateId = t.PointTemplateId,
										  PointTemplateGuid = t.PointTemplateGuid,
										  PointId = t.PointId,
										  PointGuid = t.PointGuid,
										  IsDeviceAlarmMapAlarmTest = a.IsDeviceAlarmMapAlarmTest
									  }).ToList();

			model.AlarmTestList.AddRange(pointAlarmTestList);

			// get the list of exposed settings
			foreach (var template in pointTemplates)
			{
				var exposedPointSettings = template.EnumeratePointValueIdentifiersForPointTemplateFilterByType(
					PointValueType.Point,
					false,
					"",
					PointValueFieldType.ID);
				foreach (var exposedPointSetting in exposedPointSettings)
				{
					model.SettingList.Add(
						new PointAccessDetailSetting
						{
							PropertyID = exposedPointSetting.Key.PropertyID,
							SettingName = exposedPointSetting.Value,
							PointTemplateId = template.PointId,
							PointTemplateGuid = template.PointTemplateGuid,
							ExposedSettingGuid = template.PointTemplateGuid,
							ModuleId = "",
							ModifyDisabled = template.GetExposedSettingAttribute(exposedPointSetting.Key)?.ModifyDisabled ?? true,
						});
				}
				var exposedPropertySettings =
					template.EnumeratePointValueIdentifiersForPointTemplateFilterByType(
						PointValueType.Setting,
						false,
						"",
						PointValueFieldType.ID);
				foreach (var exposedPropertySetting in exposedPropertySettings)
				{
					model.SettingList.Add(
						new PointAccessDetailSetting
						{
							PropertyID = exposedPropertySetting.Key.PropertyID,
							SettingName = exposedPropertySetting.Value,
							ExposedSettingGuid = exposedPropertySetting.Key.IdentityGuid,
							ModuleId = template.Properties[exposedPropertySetting.Key.IdentityGuid].ID,
							PointTemplateId = template.PointId,
							PointTemplateGuid = template.PointTemplateGuid,
							ModifyDisabled = template.Properties[exposedPropertySetting.Key.IdentityGuid].GetExposedSettingAttribute(exposedPropertySetting.Key).ModifyDisabled
						});
				}
			}
		}

		[NonAction]
		public static string SerializeModel(PointAccessDetailModel model)
		{
			return JsonConvert.SerializeObject(model.PointAccessGroupAssignmentList);
		}

		[NonAction]
		public static string SerializeUserGroupToPointAccessGroupMapModel(PointAccessDetailModel model)
		{
			return JsonConvert.SerializeObject(model.UserGroupToPointAccessGroupMap);
		}

		[HttpGet]
		public ActionResult GetListOfPointAccessGroups()
		{
			try
			{
				var pointAccessGroupDictionary = FMChannelHelper.MakeCall<IPointAccessGroups, Dictionary<Guid, PointAccessGroup>>(x => x.Enumerate(this.Security));
				var returnList = pointAccessGroupDictionary.Select(item => item.Value).ToList().Select(x => new { PointAccessGroupGuid = x.PointAccessGroupGuid, ID = x.ID});

				return this.JsonWithErrorMessages(returnList, JsonRequestBehavior.AllowGet);
			}
			catch (Exception except)
			{
				this.OnError(except);
				return this.JsonWithErrorMessages(null, JsonRequestBehavior.AllowGet);
			}
		}

		[HttpPost, ValidateJsonAntiForgeryToken]
		public ActionResult AddPointAccessGroup(string id)
		{
			try
			{
				var duplicatePointAccessGroupGuid = FMChannelHelper.MakeCall<IPointAccessGroups, Guid?>(x => x.GetDuplicate(this.Security, id, this.Security.SiteGuid));
				if (duplicatePointAccessGroupGuid == null || duplicatePointAccessGroupGuid == new Guid())
				{
					PointAccessGroup pointAccessGroup = new PointAccessGroup();
					pointAccessGroup.PointAccessGroupGuid = Guid.NewGuid();
					pointAccessGroup.ID = id;
					pointAccessGroup.SiteGuid = this.Security.SiteGuid;

					pointAccessGroup.PointAccessGroupGuid= FMChannelHelper.MakeCall<IPointAccessGroups, Guid>(x => x.Add(this.Security, pointAccessGroup));

					this.ModelState.Clear();
					this.AddSuccess(this.GetTranslatedText("Save Successful"));

					return this.JsonWithErrorMessages(new { PointAccessGroupGuid = pointAccessGroup.PointAccessGroupGuid, duplicateFound = false }, JsonRequestBehavior.AllowGet);
				}
				else
				{
					this.ModelState.Clear();
					return this.JsonWithErrorMessages(new { PointAccessGroupGuid = duplicatePointAccessGroupGuid, duplicateFound = true }, JsonRequestBehavior.AllowGet);
				}
			}
			catch (Exception except)
			{
				this.OnError(except);
				return this.JsonWithErrorMessages(null, JsonRequestBehavior.AllowGet);
			}
		}

		[HttpPost]
		[ValidateJsonAntiForgeryToken]
		public ActionResult AddPointAccessGroupToUserGroup(Guid pointAccessGroupGuid, Guid userGroupGuid)
		{
			try
			{

				var originalPointAccessGroup = FMChannelHelper.MakeCall<IPointAccessGroups, PointAccessGroup>(x => x.Get(this.Security, pointAccessGroupGuid));

				var pointAccessGroupToUserGroupMap = originalPointAccessGroup.PointAccessGroupToUserGroupMapList.FirstOrDefault(x => x.UserGroupGuid == userGroupGuid);
				if (pointAccessGroupToUserGroupMap == null)
				{
					pointAccessGroupToUserGroupMap = new PointAccessGroupToUserGroupMap();
					pointAccessGroupToUserGroupMap.PointAccessGroupGuid = pointAccessGroupGuid;
					pointAccessGroupToUserGroupMap.PointAccessGroupToUserGroupGuid = Guid.NewGuid();
					pointAccessGroupToUserGroupMap.UserGroupGuid = userGroupGuid;
					pointAccessGroupToUserGroupMap.Assigned = true;
					originalPointAccessGroup.PointAccessGroupToUserGroupMapList.Add(pointAccessGroupToUserGroupMap);
				}
				else
				{
					pointAccessGroupToUserGroupMap.Assigned = true;
				}

				FMChannelHelper.MakeCall<IPointAccessGroups>(x => x.Modify(this.Security, originalPointAccessGroup));

				var model = new PointAccessDetailModel();
				this.PopulatePointAccessGroupAssignments(originalPointAccessGroup, model);

				this.ModelState.Clear();
				return this.JsonWithErrorMessages(model.PointAccessGroupAssignmentList[0], JsonRequestBehavior.AllowGet);
			}
			catch (Exception except)
			{
				this.OnError(except);
				return this.JsonWithErrorMessages(null, JsonRequestBehavior.AllowGet);
			}
		}

		[HttpPost]
		[ValidateJsonAntiForgeryToken]
		public ActionResult DeleteUserGroupFromPointAccessGroup(Guid pointAccessGroupGuid, Guid userGroupGuid)
		{
			try
			{

				var originalPointAccessGroup = FMChannelHelper.MakeCall<IPointAccessGroups, PointAccessGroup>(x => x.Get(this.Security, pointAccessGroupGuid));

				originalPointAccessGroup.PointAccessGroupToUserGroupMapList.RemoveAll(x => x.UserGroupGuid == userGroupGuid);

				FMChannelHelper.MakeCall<IPointAccessGroups>(x => x.Modify(this.Security, originalPointAccessGroup));

				var model = new PointAccessDetailModel();
				this.PopulatePointAccessGroupAssignments(originalPointAccessGroup, model);

				this.ModelState.Clear();
				this.AddSuccess(this.GetTranslatedText("Point Access Group successfully removed."));
				return this.JsonWithErrorMessages(model.PointAccessGroupAssignmentList[0], JsonRequestBehavior.AllowGet);
			}
			catch (Exception except)
			{
				this.OnError(except);
				return this.JsonWithErrorMessages(null, JsonRequestBehavior.AllowGet);
			}
		}

		[HttpPost, ValidateJsonAntiForgeryToken]
		public ActionResult DeletePointAccessGroup(Guid pointAccessGroupGuid)
		{
			var results = new List<KeyValuePair<string, string>>();
			try
			{
				FMChannelHelper.MakeCall<IPointAccessGroups>(x => x.Purge(this.Security, pointAccessGroupGuid));
				this.AddSuccess("Point Access Group Successfully Removed.");

				return this.JsonWithErrorMessages(null, JsonRequestBehavior.AllowGet);
			}
			catch (Exception except)
			{
				this.OnError(except);
				return this.JsonWithErrorMessages(null, JsonRequestBehavior.AllowGet);
			}
		}

		[HttpPost, ValidateJsonAntiForgeryToken]
		public ActionResult ModifyPointAccessGroup(PointAccessGroupAssignment pointAccessGroupAssignment)
		{
			try
			{
				var pointAccessGroupGuid = pointAccessGroupAssignment.PointAccessGroupGuid;
				var originalPointAccessGroup = FMChannelHelper.MakeCall<IPointAccessGroups, PointAccessGroup>(x => x.Get(this.Security, pointAccessGroupGuid));

				ResolvePointAccessGroupAssignments(pointAccessGroupAssignment, originalPointAccessGroup, pointAccessGroupGuid);

				FMChannelHelper.MakeCall<IPointAccessGroups>(x => x.Modify(this.Security, originalPointAccessGroup));

				this.ModelState.Clear();
				this.AddSuccess(this.GetTranslatedText("Save Successful"));
				return this.JsonWithErrorMessages(null, JsonRequestBehavior.AllowGet);
			}
			catch (Exception except)
			{
				this.OnError(except);
				return this.JsonWithErrorMessages(null, JsonRequestBehavior.AllowGet);
			}
		}

		[HttpPost, ValidateJsonAntiForgeryToken]
		public ActionResult ModifyPointAccessGroups(List<PointAccessGroupAssignment> pointAccessGroupAssignmentList)
		{
			try
			{
				if (pointAccessGroupAssignmentList == null)
				{
					this.ModelState.Clear();
					this.AddSuccess(this.GetTranslatedText("There are no changes to save."));
					return this.JsonWithErrorMessages(null, JsonRequestBehavior.AllowGet);
				}

				var pointAccessGroupToModify = new List<PointAccessGroup>();
				foreach (var pointAccessGroupAssignment in pointAccessGroupAssignmentList)
				{
					var pointAccessGroupGuid = pointAccessGroupAssignment.PointAccessGroupGuid;
					var originalPointAccessGroup = FMChannelHelper.MakeCall<IPointAccessGroups, PointAccessGroup>(x => x.Get(this.Security, pointAccessGroupGuid));

					ResolvePointAccessGroupAssignments(pointAccessGroupAssignment, originalPointAccessGroup, pointAccessGroupGuid);
					pointAccessGroupToModify.Add(originalPointAccessGroup);
				}

				FMChannelHelper.MakeCall<IPointAccessGroups>(x => x.ModifyByList(this.Security, pointAccessGroupToModify));

				this.ModelState.Clear();
				this.AddSuccess(this.GetTranslatedText("Save Successful"));
				return this.JsonWithErrorMessages(null, JsonRequestBehavior.AllowGet);
			}
			catch (Exception except)
			{
				this.OnError(except);
				return this.JsonWithErrorMessages(null, JsonRequestBehavior.AllowGet);
			}
		}


		private static void ResolvePointAccessGroupAssignments(
			PointAccessGroupAssignment pointAccessGroupAssignment,
			PointAccessGroup originalPointAccessGroup,
			Guid pointAccessGroupGuid)
		{
			// resolve the point template assignments ( add the new access records and remove things deleted in the UI ) 
			var pointTemplateAssignmentDictionary = new Dictionary<Guid, PointAccessGroupToPointTemplateMap>();
			foreach (var pointtemplateAssignment in pointAccessGroupAssignment.PointAccessGroupToPointTemplateAssignmentList)
			{
				if (pointtemplateAssignment.Assigned == true)
				{
					// found record in the existing PointGroupAccess
					var templateMapRecord =
						originalPointAccessGroup.PointAccessGroupToPointTemplateMapList.FirstOrDefault(
							x => x.PointTemplateGuid == pointtemplateAssignment.PointTemplateGuid);
					if (templateMapRecord == null)
					{
						templateMapRecord = new PointAccessGroupToPointTemplateMap();
					}
					templateMapRecord.Assigned = true;
					templateMapRecord.PointAccessGroupGuid = pointAccessGroupGuid;
					templateMapRecord.PointTemplateGuid = pointtemplateAssignment.PointTemplateGuid;
					if (templateMapRecord.PointAccessGroupToPointTemplateGuid == Guid.Empty)
					{
						templateMapRecord.PointAccessGroupToPointTemplateGuid = Guid.NewGuid();
					}
					pointTemplateAssignmentDictionary.Add(templateMapRecord.PointAccessGroupToPointTemplateGuid, templateMapRecord);

					// delete all entries for the points if the template is selected
					pointAccessGroupAssignment.PointAccessGroupToPointAssignmentList.RemoveAll(
						x => x.PointTemplateGuid == pointtemplateAssignment.PointTemplateGuid);
				}
			}
			originalPointAccessGroup.PointAccessGroupToPointTemplateMapList = pointTemplateAssignmentDictionary.Values.ToList();

			// resolve the point assignments ( add the new access records and remove things deleted in the UI ) 
			var pointAssignmentDictionary = new List<PointAccessGroupToPointMap>();
			foreach (var pointAssignment in pointAccessGroupAssignment.PointAccessGroupToPointAssignmentList)
			{
				if (pointAssignment.Assigned == true)
				{
					// found record in the existing PointGroupAccess
					var pointMapRecord =
						originalPointAccessGroup.PointAccessGroupToPointMapList.FirstOrDefault(
							x => x.PointGuid == pointAssignment.PointGuid);
					if (pointMapRecord == null)
					{
						pointMapRecord = new PointAccessGroupToPointMap();
					}
					pointMapRecord.PointAccessGroupGuid = pointAccessGroupGuid;
					pointMapRecord.PointGuid = pointAssignment.PointGuid;
					pointMapRecord.PointTemplateGuid = pointAssignment.PointTemplateGuid;
					pointMapRecord.Assigned = true;
					if (pointMapRecord.PointAccessGroupToPointGuid == Guid.Empty)
					{
						pointMapRecord.PointAccessGroupToPointGuid = Guid.NewGuid();
					}
					pointAssignmentDictionary.Add(pointMapRecord);
				}
			}
			originalPointAccessGroup.PointAccessGroupToPointMapList = pointAssignmentDictionary;

			// resolve the user settings assignments ( add the new access records and remove things deleted in the UI ) 
			var exposedSettingAssignmentList = new List<PointAccessGroupToExposedSettingMap>();
			foreach (var settingGroupAssignment in pointAccessGroupAssignment.PointAccessGroupToSettingAssignmentList)
			{
				if (settingGroupAssignment.View == false || settingGroupAssignment.Modify == false)
				{
					// found record in the existing UserGroupGroupAccess
					var settingGroupMapRecord =
						originalPointAccessGroup.PointAccessGroupToExposedSettingMapList.FirstOrDefault(
							x =>
								x.ExposedSettingGuid == settingGroupAssignment.ExposedSettingGuid
								&& x.PropertyID == settingGroupAssignment.PropertyID);
					if (settingGroupMapRecord == null)
					{
						settingGroupMapRecord = new PointAccessGroupToExposedSettingMap();
					}
					settingGroupMapRecord.ExposedSettingGuid = settingGroupAssignment.ExposedSettingGuid;
					settingGroupMapRecord.PropertyID = settingGroupAssignment.PropertyID;
					settingGroupMapRecord.PointTemplateGuid = settingGroupAssignment.PointTemplateGuid;
					if (settingGroupMapRecord.ExposedSettingGuid == settingGroupMapRecord.PointTemplateGuid)
					{
						settingGroupMapRecord.ValueType = PointValueType.Point;
					}
					else
					{
						settingGroupMapRecord.ValueType = PointValueType.Setting;
					}

					settingGroupMapRecord.View = settingGroupAssignment.View;
					settingGroupMapRecord.Modify = settingGroupAssignment.Modify;
					settingGroupMapRecord.PointAccessGroupGuid = pointAccessGroupGuid;
					if (settingGroupMapRecord.PointAccessGroupToExposedSettingGuid == Guid.Empty)
					{
						settingGroupMapRecord.PointAccessGroupToExposedSettingGuid = Guid.NewGuid();
					}
					exposedSettingAssignmentList.Add(settingGroupMapRecord);
				}
			}
			originalPointAccessGroup.PointAccessGroupToExposedSettingMapList = exposedSettingAssignmentList;

			// resolve the tag assignments ( add the new access records and remove things deleted in the UI ) 
			var tagAssignmentList = new List<PointAccessGroupToTagMap>();
			foreach (var tagAssignment in pointAccessGroupAssignment.PointAccessGroupToTagAssignmentList)
			{
				if (tagAssignment.View == false || tagAssignment.Modify == false || tagAssignment.ExceedRange == false
				    || tagAssignment.Override == false)
				{
					// found record in the existing UserGroupGroupAccess
					var tagMapRecord =
						originalPointAccessGroup.PointAccessGroupToTagMapList.FirstOrDefault(
							x => x.PointTemplateTagGuid == tagAssignment.PointTagGuid);
					if (tagMapRecord == null)
					{
						tagMapRecord = new PointAccessGroupToTagMap();
					}
					tagMapRecord.PointTemplateTagGuid = tagAssignment.PointTagGuid;
					tagMapRecord.View = tagAssignment.View;
					tagMapRecord.Modify = tagAssignment.Modify;
					tagMapRecord.ExceedRange = tagAssignment.ExceedRange;
					tagMapRecord.Override = tagAssignment.Override;

					tagMapRecord.PointAccessGroupGuid = pointAccessGroupGuid;
					if (tagMapRecord.PointAccessGroupToTagGuid == Guid.Empty)
					{
						tagMapRecord.PointAccessGroupToTagGuid = Guid.NewGuid();
					}
					tagAssignmentList.Add(tagMapRecord);
				}
			}
			originalPointAccessGroup.PointAccessGroupToTagMapList = tagAssignmentList;

			// resolve the point tag assignments ( add the new access records and remove things deleted in the UI ) 
			var pointTagAssignmentList = new List<PointAccessGroupToPointTagMap>();
			foreach (var pointTagAssignment in pointAccessGroupAssignment.PointAccessGroupToPointTagAssignmentList)
			{
				if (pointTagAssignment.View == false || pointTagAssignment.Modify == false || pointTagAssignment.ExceedRange == false
					 || pointTagAssignment.Override == false)
				{
					// found record in the existing UserGroupGroupAccess
					var pointTagMapRecord =
						originalPointAccessGroup.PointAccessGroupToPointTagMapList.FirstOrDefault(
							x => x.PointTagGuid == pointTagAssignment.PointTagGuid);
					if (pointTagMapRecord == null)
					{
						pointTagMapRecord = new PointAccessGroupToPointTagMap();
					}
					pointTagMapRecord.PointTagGuid = pointTagAssignment.PointTagGuid;
					pointTagMapRecord.View = pointTagAssignment.View;
					pointTagMapRecord.Modify = pointTagAssignment.Modify;
					pointTagMapRecord.ExceedRange = pointTagAssignment.ExceedRange;
					pointTagMapRecord.Override = pointTagAssignment.Override;

					pointTagMapRecord.PointAccessGroupGuid = pointAccessGroupGuid;
					if (pointTagMapRecord.PointAccessGroupToPointTagGuid == Guid.Empty)
					{
						pointTagMapRecord.PointAccessGroupToPointTagGuid = Guid.NewGuid();
					}
					pointTagAssignmentList.Add(pointTagMapRecord);
				}
			}
			originalPointAccessGroup.PointAccessGroupToPointTagMapList = pointTagAssignmentList;


			// resolve the alarm test assignments ( add the new access records and remove things deleted in the UI ) 
			var alarmTestAssignmentList = new List<PointAccessGroupToAlarmTestMap>();
			foreach (var tagAssignment in pointAccessGroupAssignment.PointAccessGroupToAlarmTestAssignmentList)
			{
				if (tagAssignment.View == false || tagAssignment.Acknowledge == false)
				{
					// found record in the existing UserGroupGroupAccess
					var tagMapRecord =
						originalPointAccessGroup.PointAccessGroupToAlarmTestMapList.FirstOrDefault(
							x => x.AlarmTestTemplateGuid == tagAssignment.AlarmTestTemplateGuid);
					if (tagMapRecord == null)
					{
						tagMapRecord = new PointAccessGroupToAlarmTestMap();
					}
					tagMapRecord.AlarmTestTemplateGuid = tagAssignment.AlarmTestTemplateGuid;
					tagMapRecord.View = tagAssignment.View;
					tagMapRecord.Acknowledge = tagAssignment.Acknowledge;
					tagMapRecord.PointAccessGroupGuid = pointAccessGroupGuid;
					if (tagMapRecord.PointAccessGroupToAlarmTestGuid == Guid.Empty)
					{
						tagMapRecord.PointAccessGroupToAlarmTestGuid = Guid.NewGuid();
					}
					alarmTestAssignmentList.Add(tagMapRecord);
				}
			}
			originalPointAccessGroup.PointAccessGroupToAlarmTestMapList = alarmTestAssignmentList;

			// resolve the alarm test assignments ( add the new access records and remove things deleted in the UI ) 
			var pointAlarmTestAssignmentList = new List<PointAccessGroupToPointAlarmTestMap>();
			foreach (var tagAssignment in pointAccessGroupAssignment.PointAccessGroupToPointAlarmTestAssignmentList)
			{
				if (tagAssignment.View == false || tagAssignment.Acknowledge == false)
				{
					// found record in the existing UserGroupGroupAccess
					var tagMapRecord =
					originalPointAccessGroup.PointAccessGroupToPointAlarmTestMapList.FirstOrDefault(
					x => x.AlarmTestGuid == tagAssignment.AlarmTestGuid && x.PointGuid == tagAssignment.PointGuid);
					if (tagMapRecord == null)
					{
						tagMapRecord = new PointAccessGroupToPointAlarmTestMap();
					}
					tagMapRecord.AlarmTestGuid = tagAssignment.AlarmTestGuid;
					tagMapRecord.PointGuid = tagAssignment.PointGuid;
					tagMapRecord.View = tagAssignment.View;
					tagMapRecord.Acknowledge = tagAssignment.Acknowledge;
					tagMapRecord.PointAccessGroupGuid = pointAccessGroupGuid;
					if (tagMapRecord.PointAccessGroupToPointAlarmTestGuid == Guid.Empty)
					{
						tagMapRecord.PointAccessGroupToPointAlarmTestGuid = Guid.NewGuid();
					}
					pointAlarmTestAssignmentList.Add(tagMapRecord);
				}
			}
			originalPointAccessGroup.PointAccessGroupToPointAlarmTestMapList = pointAlarmTestAssignmentList;

			// resolve the user group assignments ( add the new access records and remove things deleted in the UI ) 
			var userGroupAssignmentList = new List<PointAccessGroupToUserGroupMap>();
			foreach (var userGroupAssignment in pointAccessGroupAssignment.PointAccessGroupToUserGroupAssignmentList)
			{
				if (userGroupAssignment.Assigned == true)
				{
					// found record in the existing UserGroupGroupAccess
					var userGroupMapRecord =
					originalPointAccessGroup.PointAccessGroupToUserGroupMapList.FirstOrDefault(
					x => x.UserGroupGuid == userGroupAssignment.UserGroupGuid);
					if (userGroupMapRecord == null)
					{
						userGroupMapRecord = new PointAccessGroupToUserGroupMap();
					}
					userGroupMapRecord.UserGroupGuid = userGroupAssignment.UserGroupGuid;
					userGroupMapRecord.Assigned = true;
					userGroupMapRecord.PointAccessGroupGuid = pointAccessGroupGuid;
					if (userGroupMapRecord.PointAccessGroupToUserGroupGuid == Guid.Empty)
					{
						userGroupMapRecord.PointAccessGroupToUserGroupGuid = Guid.NewGuid();
					}
					userGroupAssignmentList.Add(userGroupMapRecord);
				}
			}
			originalPointAccessGroup.PointAccessGroupToUserGroupMapList = userGroupAssignmentList;
		}
	}
}
