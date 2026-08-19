using FMBusinessObjects.DataObjects;


namespace FuelsManager.Areas.InventoryManagement.Controllers
{
	using FuelsManager.Areas.InventoryManagement.ViewModels;
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Web.Mvc;
	using System.Xml;
	using System.Xml.Serialization;
	using System.Web.UI.WebControls;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;

	using FuelsManager.Areas.Controllers;

	using Newtonsoft.Json;

	public class AnimationManagerController : FMBaseControllerEx
	{
		[NonAction]
		public static string SerializeModel(AnimationManagerModel model)
		{
			return JsonConvert.SerializeObject(model);
		}

		[NonAction]
		public static AnimationManagerModel DeserializeModel(string modelStr)
		{
			var jsonSerializerSettings = new JsonSerializerSettings
			{
				MissingMemberHandling = MissingMemberHandling.Ignore
			};

			var model = JsonConvert.DeserializeObject<AnimationManagerModel>(modelStr, jsonSerializerSettings);

			return model;
		}

		[NonAction]
		public static AnimationPointValueList DeserializeAnimationPointValueList(string animationPointValueListStr)
		{
			if (string.IsNullOrEmpty(animationPointValueListStr))
			{
				return null;
			}
			var jsonSerializerSettings = new JsonSerializerSettings
			{
				MissingMemberHandling = MissingMemberHandling.Ignore
			};

			var animationPointValueList = JsonConvert.DeserializeObject<AnimationPointValueList>(animationPointValueListStr, jsonSerializerSettings);

			return animationPointValueList;
		}

		protected static string GetFieldString(PointValueFieldType field)
		{
			string fieldString = "UKNOWN";
			switch (field)
			{
				case PointValueFieldType.VALUE:
					fieldString = "VALUE";
					break;
				case PointValueFieldType.ID:
					fieldString = "ID";
					break;
				case PointValueFieldType.TIMESTAMP:
					fieldString = "TIMESTAMP";
					break;
				case PointValueFieldType.UNITS:
					fieldString = "UNITS";
					break;
				case PointValueFieldType.ALARMSTATUS:
					fieldString = "ALARM STATUS";
					break;
			}
			return fieldString;
		}

		protected static List<AnimationManagerPropertyVisualState> CreateAnimationModelFromAnimationVisualStates(AnimationProperty animationProperty)
		{
			var vsList = new List<AnimationManagerPropertyVisualState>();
			foreach (var animationVs in animationProperty.VisualStates)
			{
				var visualState = new AnimationManagerPropertyVisualState
				{
					AnimationPropertyVisualStateGuid = animationVs.AnimationPropertyVisualStateGuid,
					Value = animationVs.Value
				};
				vsList.Add(visualState);
			}
			return vsList;
		}

		protected static List<AnimationManagerProperty> CreateAnimationModelFromAnimationProperties(AnimationTest animationTest)
		{
			var propertyList = new List<AnimationManagerProperty>();
			foreach (var animationProperty in animationTest.PropertyList)
			{
				var property = new AnimationManagerProperty
				{
					AnimationPropertyGuid = animationProperty.AnimationPropertyGuid,
					Name = animationProperty.Name,
					LookupName = animationProperty.LookupName,
					gojsPropertyName = animationProperty.gojsPropertyName,
					VisualStates = CreateAnimationModelFromAnimationVisualStates(animationProperty)
				};
				propertyList.Add(property);
			}

			return propertyList;
		}

		protected static List<AnimationManagerModelTest> CreateAnimationModelFromAnimationTests(AnimationTestGroup animationTestGroup)
		{
			var animationTests = new List<AnimationManagerModelTest>();
			foreach (var animeTest in animationTestGroup.TestList)
			{
				var test = new AnimationManagerModelTest
				{
					AnimationTestGuid = animeTest.AnimationTestGuid,
					TestComparisonOperator = animeTest.TestComparisonOperator,
					BitmaskOperator = animeTest.BitmaskOperator,
					ComparisonValue = animeTest.ComparisonValue,
					BitmaskStr = animeTest.BitmaskStr,
					Bitmask = animeTest.Bitmask,
					PropertyList = CreateAnimationModelFromAnimationProperties(animeTest)
				};
				animationTests.Add(test);
			}
			return animationTests;
		}

		protected static List<AnimationManagerModelTestGroup> CreateAnimationModelFromAnimationTestGroups(AnimationClass animation)
		{
			var animationTestGroups = new List<AnimationManagerModelTestGroup>();
			foreach(var animeTg in animation.AnimationTestGroupList)
			{
				var testGroup = new AnimationManagerModelTestGroup
				                {
					                ID = animeTg.ID,
										 DataType = animeTg.DataType,
										 PointValueAndFieldID = FMBaseController.TranslateText("UNASSIGNED"),
										 PointValueGuid = Guid.Empty,
										 PointValueIsFromTemplate = false,
										 AnimationTestGroupGuid = animeTg.AnimationTestGroupGuid,
										 TestList = CreateAnimationModelFromAnimationTests(animeTg),
										 Field = animeTg.Field,
										 ValueType = PointValueType.Tag,
										 PointID = String.Empty,
										 PointValueID = String.Empty
									};
				animationTestGroups.Add(testGroup);
			}
			return animationTestGroups;
		}

		protected static void CreateValidTestGroupDataTypeList(AnimationManagerModel animationModel)
		{
			animationModel.ValidTestGroupDataTypeList = new List<KeyValuePair<string, string>>();
			List<KeyValuePair<string, string>> dataTypes = PointTemplateTag.EnumerateTagDataTypes();

			foreach (var dataType in dataTypes)
			{
				if (dataType.Key == "System.DateTimeOffset"
					|| dataType.Key == "System.DateTime"
					|| dataType.Key == "System.TimeSpan")
					//|| dataType.Key == "FMBusinessObjects.DataObjects.PointCommandStatusListReference")
				{
					continue;
				}

				string newDataTypeText = FMBaseController.TranslateText(dataType.Value);
				var newItem = new KeyValuePair<string, string>(dataType.Key, newDataTypeText);
				animationModel.ValidTestGroupDataTypeList.Add(newItem);
			}
		}

		protected static void AddFieldTranslations(AnimationManagerModel animationModel)
		{
			animationModel.TranslatedTextForValueField = FMBaseController.TranslateText("Value");

			animationModel.TranslatedTextForIDField = FMBaseController.TranslateText("ID");

			animationModel.TranslatedTextForTimestampField = FMBaseController.TranslateText("Timestamp");

			animationModel.TranslatedTextForUnitsField = FMBaseController.TranslateText("Units");

			animationModel.TranslatedTextForAlarmStatusField = FMBaseController.TranslateText("Alarm Status");
		}

		protected static AnimationPointValueList ProcessPointValueList(AnimationPointValueList animationPointValueAssignment, AnimationManagerModel model)
		{
			if (animationPointValueAssignment != null)
			{
				if (animationPointValueAssignment.AnimationGuid != Guid.Empty
					&& animationPointValueAssignment.TestGroupPointValueInfoList != null && animationPointValueAssignment.TestGroupPointValueInfoList.Count > 0)
				{

					var animation = model.GetAnimation(animationPointValueAssignment.AnimationGuid);
					if (animation != null)
					{
						var newAnimationPointValueAssignment = new AnimationPointValueList
						{
							AnimationGuid = animationPointValueAssignment.AnimationGuid,
							AnimationID = animationPointValueAssignment.AnimationID,
							TestGroupPointValueInfoList = new List<AnimationPointValue>()
						};
						for (var i = 0; i < animationPointValueAssignment.TestGroupPointValueInfoList.Count; i++)
						{
							var testGroupPointValueInfo = animationPointValueAssignment.TestGroupPointValueInfoList[i];
							var testGroup = animation.GetAnimationTestGroup(testGroupPointValueInfo.AnimationTestGroupGuid);
							if (testGroup != null)
							{
								if (testGroup.DataType == testGroupPointValueInfo.DataType && testGroup.Field == testGroupPointValueInfo.Field
								  && testGroupPointValueInfo.PointValueGuid != Guid.Empty)
								{
									newAnimationPointValueAssignment.TestGroupPointValueInfoList.Add(testGroupPointValueInfo);
									testGroup.PointValueGuid = testGroupPointValueInfo.PointValueGuid;
									testGroup.PointGuid = testGroupPointValueInfo.PointGuid;
									testGroup.PointValueIsFromTemplate = testGroupPointValueInfo.PointValueIsFromTemplate;
									testGroup.ValueType = testGroupPointValueInfo.ValueType;
									testGroup.PointID = testGroupPointValueInfo.PointID;
									testGroup.PointValueID = testGroupPointValueInfo.PointValueID;
								}
							}
						}
						return newAnimationPointValueAssignment;
					}
				}
			}
			return null;
		}

		protected static PointValue GetPointValueFromList(Guid pointValueGuid, PointValueType valueType, string pointValueID, List<PointValue> pointValueList )
		{
			foreach (var pointValue in pointValueList)
			{
				if (pointValue.PointValueIdentifier.IdentityGuid == pointValueGuid
				    && pointValue.PointValueIdentifier.PointValueType == valueType)
				{
					if (valueType == PointValueType.Tag)
					{
						return pointValue;
					}
					if (pointValue.PointValueIdentifier.PropertyID == pointValueID)
					{
						return pointValue;
					}
				}
			}
			return null;
		}

		protected static void HandlePointValueAndFieldID( SecurityClass security, AnimationManagerModel animationModel, AnimationPointValueList processedPointValueList)
		{
			var pointGuidList = new List<Guid>();
			var pointDictionary = new Dictionary<Guid, Point>();
			var pointTemplateGuidList = new List<Guid>();
			var pointTemplateDictionary = new Dictionary<Guid, PointTemplate>();
			foreach (var testGroupInfo in processedPointValueList.TestGroupPointValueInfoList)
			{
				if (testGroupInfo.PointGuid != Guid.Empty)
				{
					if (testGroupInfo.PointValueIsFromTemplate == false
					    && pointGuidList.Contains(testGroupInfo.PointGuid) == false)
					{
						pointGuidList.Add(testGroupInfo.PointGuid);
					}
					else if (testGroupInfo.PointValueIsFromTemplate == true && pointTemplateGuidList.Contains(testGroupInfo.PointGuid) == false)
					{
						pointTemplateGuidList.Add(testGroupInfo.PointGuid);
					}
				}
			}
			if (pointGuidList.Count > 0)
			{
				var points = FMChannelHelper.MakeCall<IPoints, PointCollection>(x => x.GetPoints(security, pointGuidList));
				foreach (var point in points)
				{
					pointDictionary.Add(point.PointGuid,point);
				}
			}
			if (pointTemplateGuidList.Count > 0)
			{
				//I had to do this looping because PointTemplates were not implemented correctly to take guid lists.  There should only be one template though for a point detail.
				foreach (var pointTemplateGuid in pointTemplateGuidList)
				{
					var pointTemplate = FMChannelHelper.MakeCall<IPointTemplates, PointTemplate>(x => x.Get(security, pointTemplateGuid));
					pointTemplateDictionary.Add(pointTemplate.PointTemplateGuid, pointTemplate);
				}
			}
			redoIteration:
			foreach (var testGroupInfo in processedPointValueList.TestGroupPointValueInfoList)
			{
				if (testGroupInfo.PointGuid != Guid.Empty)
				{
					if (testGroupInfo.PointValueIsFromTemplate == false)
					{
						try
						{
							var point = pointDictionary[testGroupInfo.PointGuid];
							testGroupInfo.PointID = point.PointId;
							if (testGroupInfo.ValueType == PointValueType.Tag)
							{
								testGroupInfo.PointValueID = point.Tags[testGroupInfo.PointValueGuid].ID;
							}
						}
						catch
						{
							processedPointValueList.TestGroupPointValueInfoList.Remove(testGroupInfo);
							goto redoIteration;
						}
					}
					else if (testGroupInfo.PointValueIsFromTemplate == true && pointTemplateGuidList.Contains(testGroupInfo.PointGuid) == false)
					{
						var pointTemplate = pointTemplateDictionary[testGroupInfo.PointGuid];
						testGroupInfo.PointID = pointTemplate.PointId;
						if (testGroupInfo.ValueType == PointValueType.Tag)
						{
							testGroupInfo.PointValueID = pointTemplate.Tags[testGroupInfo.PointValueGuid].ID;
						}
					}
				}
			}
			var animation = animationModel.GetAnimation(processedPointValueList.AnimationGuid);
			if (animation != null)
			{
				foreach (var testGroupInfo in processedPointValueList.TestGroupPointValueInfoList)
				{
					if (testGroupInfo.PointValueGuid != Guid.Empty)
					{
						var testGroup = animation.GetAnimationTestGroup(testGroupInfo.AnimationTestGroupGuid);
						if (testGroup != null)
						{
							testGroup.PointID = testGroupInfo.PointID;
							testGroup.PointValueID = testGroupInfo.PointValueID;
							testGroup.PointValueAndFieldID = testGroup.PointID + "." + testGroup.PointValueID + "."
							                                 + GetFieldString(testGroup.Field);
						}
					}
				}
			}
		}

		protected static void HandlePointValueList(SecurityClass security, AnimationManagerModel animationModel, AnimationPointValueList pointValueList)
		{
			var processedPointValueList = ProcessPointValueList(pointValueList, animationModel);
			animationModel.PointValueList = processedPointValueList;

			if (processedPointValueList == null)
			{
				if (animationModel.AnimationList.Count >= 1)
				{
					animationModel.SelectedAnimationGuid = animationModel.AnimationList[0].AnimationGuid;
				}
			}
			else
			{
				animationModel.SelectedAnimationGuid = processedPointValueList.AnimationGuid;
				if (processedPointValueList.TestGroupPointValueInfoList != null
				    && processedPointValueList.TestGroupPointValueInfoList.Count > 0)
				{
					HandlePointValueAndFieldID(security, animationModel, processedPointValueList);
				}
			}
		}

		protected static AnimationManagerModel GetModel(SecurityClass security, AnimationPointValueList pointValueList)
		{
			var animations = FMChannelHelper.MakeCall<IAnimations, Dictionary<Guid,AnimationClass>>(x => x.EnumerateAnimationsBySiteGuid(security, security.SiteGuid));
			var animationModel = new AnimationManagerModel();
			AddFieldTranslations(animationModel);
			CreateValidTestGroupDataTypeList(animationModel);
			
			animationModel.AnimationList = new List<AnimationManagerModelAnimation>();
			foreach (var anime in animations.Values)
			{
				var animationTestGroups = CreateAnimationModelFromAnimationTestGroups(anime);
				var animation = new AnimationManagerModelAnimation
				                {
					                AnimationGuid = anime.AnimationGuid,
					                ID = anime.ID,
					                UseCount = anime.UseCount,
										 AnimationTestGroups = animationTestGroups
				};
				animationModel.AnimationList.Add(animation);
			}
			HandlePointValueList(security, animationModel, pointValueList);
			animationModel.SortAnimationsAlphabetically();
			return animationModel;
		}

		protected static List<AnimationPropertyVisualState> CreateVisualStatesListFromModel(AnimationManagerProperty modelProperty)
		{
			var visualStateList = new List<AnimationPropertyVisualState>();
			foreach (var modelVisualState in modelProperty.VisualStates)
			{
				var visualState = new AnimationPropertyVisualState
				{
					AnimationPropertyVisualStateGuid = modelVisualState.AnimationPropertyVisualStateGuid,
					Value = modelVisualState.Value
				};
				visualStateList.Add(visualState);
			}
			return visualStateList;
		}

		protected static List<AnimationProperty> CreatePropertyListFromModel(AnimationManagerModelTest modelTest)
		{
			var propertyList = new List<AnimationProperty>();
			foreach (var modelProperty in modelTest.PropertyList)
			{
				var property = new AnimationProperty
				{
					AnimationPropertyGuid = modelProperty.AnimationPropertyGuid,
					Name = modelProperty.Name,
					LookupName = modelProperty.LookupName,
					gojsPropertyName = modelProperty.gojsPropertyName,
					VisualStates = CreateVisualStatesListFromModel(modelProperty)
				};
				propertyList.Add(property);
			}
			return propertyList;
		}

		protected static long ConvertBitmaskStringToLong(string bitmaskString)
		{
			//Richard you must write your conversion routine here.
			//I still believe that this should be done on the client side in the javascript and that the 
			//AnimationManagerModelTest.Bitmask should be a long
			return -1;
		}

		protected static List<AnimationTest> CreateTestListFromModel(AnimationManagerModelTestGroup modelTestGroup)
		{
			var testList = new List<AnimationTest>();
			foreach (var modelTest in modelTestGroup.TestList)
			{
				var test = new AnimationTest
				{
					AnimationTestGuid = modelTest.AnimationTestGuid,
					TestComparisonOperator = modelTest.TestComparisonOperator,
					BitmaskStr = modelTest.BitmaskStr,
					Bitmask = modelTest.Bitmask,
					BitmaskOperator = modelTest.BitmaskOperator,
					ComparisonValue = modelTest.ComparisonValue,
					PropertyList = CreatePropertyListFromModel(modelTest)
				};
				testList.Add(test);
			}
			return testList;
		}

		protected static List<AnimationTestGroup> CreateTestGroupListFromModel(AnimationManagerModelAnimation modelAnimation)
		{
			var testGroupList = new List<AnimationTestGroup>();
			foreach (var modelTestGroup in modelAnimation.AnimationTestGroups)
			{
				var testGroup = new AnimationTestGroup
				{
					AnimationTestGroupGuid = modelTestGroup.AnimationTestGroupGuid,
					ID = modelTestGroup.ID,
					DataType = modelTestGroup.DataType,
					Field = modelTestGroup.Field,
					TestList = CreateTestListFromModel(modelTestGroup)
				};
				testGroupList.Add(testGroup);
			}
			return testGroupList;
		}

		protected static AnimationClass CreateAnimationFromModel(AnimationManagerModelAnimation modelAnimation, SecurityClass security)
		{
			var animation = new AnimationClass
			                {
				                AnimationGuid = modelAnimation.AnimationGuid,
									 ID = modelAnimation.ID,
									 SiteGuid = security.SiteGuid,
									 AnimationTestGroupList = CreateTestGroupListFromModel(modelAnimation),
									 UpdatedBy = security.UserID
			                };
			return animation;
		}

		[HttpPost]
		public ActionResult AnimationManagerView(string pointValueListStr)
		{
			var pointValueList = DeserializeAnimationPointValueList(pointValueListStr);
			var model = GetModel(this.Security, pointValueList);
			model.SortAnimationsAlphabetically();
			return this.PartialViewWithErrorMessages("AnimationManagerView", model);
		}

		protected void SaveAnimation(Guid saveGuid, AnimationManagerModel model)
		{
			var animationModel = model.GetAnimation(saveGuid);
			if (animationModel == null)
			{
				this.OnError("Animation To Save Not Found!");
			}
			else
			{
				var animation = CreateAnimationFromModel(animationModel, this.Security);
				var animationList = new List<AnimationClass>();
				animationList.Add(animation);
				FMChannelHelper.MakeCall<IAnimations>(x => x.AddModifyAnimations(this.Security, animationList, true, true));
			}
		}

		[HttpPost]
		public ActionResult AnimationSelectionChangedAndSave(string animationGuidStr, string modelStr)
		{
			var model = DeserializeModel(modelStr);
			this.SaveAnimation(model.SelectedAnimationGuid, model);
			model.SelectedAnimationGuid = Guid.Parse(animationGuidStr);
			model.PointValueList = null;
			model.SortAnimationsAlphabetically();
			return this.PartialViewWithErrorMessages("AnimationManagerView", model);
		}

		[HttpPost]
		public ActionResult AnimationSelectionChanged(string animationGuidStr, string modelStr)
		{
			var model = DeserializeModel(modelStr);
			model.SelectedAnimationGuid = Guid.Parse(animationGuidStr);
			model.PointValueList = null;
			model.SortAnimationsAlphabetically();
			return this.PartialViewWithErrorMessages("AnimationManagerView", model);
		}

		[HttpPost]
		public ActionResult AnimationDelete(string animationGuidStr, string modelStr)
		{
			var model = DeserializeModel(modelStr);
			var deletedGuid = Guid.Parse(animationGuidStr);
			model.PointValueList = null;
			FMChannelHelper.MakeCall<IAnimations>(x => x.Purge(this.Security, deletedGuid));
			model.SortAnimationsAlphabetically();
			return this.PartialViewWithErrorMessages("AnimationManagerView", model);
		}

		[HttpPost]
		public ActionResult AnimationSave(string animationGuidStr, string modelStr)
		{
			var model = DeserializeModel(modelStr);

			var saveGuid = Guid.Parse(animationGuidStr);

			this.SaveAnimation(saveGuid, model);
			model.SortAnimationsAlphabetically();
			return this.PartialViewWithErrorMessages("AnimationManagerView", model);
		}

		[HttpPost]
		public ActionResult AnimationCopyAndRename(string animationGuidStr, string modelStr)
		{
			var model = DeserializeModel(modelStr);
			var newName = animationGuidStr;

			var animationModel = model.GetAnimation(model.SelectedAnimationGuid);
			if (animationModel == null)
			{
				this.OnError("Animation To Copy And Rename Not Found!");
			}
			else
			{
				//foreach(var valueinfo in model.PointValueList.TestGroupPointValueInfoList)
					//{
					//valueinfo.PointGuid = Guid.Empty;
					//valueinfo.PointID = null;
					//valueinfo.PointValueID = null;
					//valueinfo.PointValueGuid = Guid.Empty;
				//}
				var animationModelClone = this.ShortCutCloneAnimationModel(animationModel);
				foreach (var testGroup in animationModelClone.AnimationTestGroups)
				{
					testGroup.PointGuid = Guid.Empty;
					testGroup.PointID = null;
					testGroup.PointValueID = null;
					testGroup.PointValueGuid = Guid.Empty;
				}
				this.AnimationModelNewGuidsAndName(newName, animationModelClone);
				var animation = CreateAnimationFromModel(animationModelClone, this.Security);
				var animationList = new List<AnimationClass>();
				animationList.Add(animation);
				FMChannelHelper.MakeCall<IAnimations>(x => x.AddModifyAnimations(this.Security, animationList, true, true));
				model.AnimationList.Add(animationModelClone);
				model.SelectedAnimationGuid = animationModelClone.AnimationGuid;
			}
			model.SortAnimationsAlphabetically();
			return this.PartialViewWithErrorMessages("AnimationManagerView", model);
		}

		protected void AnimationModelNewGuidsAndName(string newName, AnimationManagerModelAnimation animationModel)
		{
			if (animationModel != null)
			{
				animationModel.ID = newName;
				animationModel.AnimationGuid = Guid.NewGuid();
				animationModel.UseCount = 0;
				if (animationModel.AnimationTestGroups != null)
				{
					foreach (var animationTestGroup in animationModel.AnimationTestGroups)
					{
						animationTestGroup.AnimationTestGroupGuid = Guid.NewGuid();
						if (animationTestGroup.TestList != null)
						{
							foreach (var animationTest in animationTestGroup.TestList)
							{
								animationTest.AnimationTestGuid = Guid.NewGuid();
								if (animationTest.PropertyList != null)
								{
									foreach (var animationProperty in animationTest.PropertyList)
									{
										animationProperty.AnimationPropertyGuid = Guid.NewGuid();
										if (animationProperty.VisualStates != null)
										{
											foreach (var visualState in animationProperty.VisualStates)
											{
												visualState.AnimationPropertyVisualStateGuid = Guid.NewGuid();
											}
										}
									}
								}
							}
						}
					}
				}
			}
		}

		protected AnimationManagerModelAnimation ShortCutCloneAnimationModel(AnimationManagerModelAnimation originalManagerModelAnimation)
		{
			AnimationManagerModelAnimation ret = null;
			if (originalManagerModelAnimation == null)
			{
				return null;
			}
			else
			{
				XmlSerializer xmlserializer = new XmlSerializer(typeof(AnimationManagerModelAnimation));

				var stringWriter = new StringWriter();
				var emptyNameSpaces = new XmlSerializerNamespaces(new[] { XmlQualifiedName.Empty });
				// explicitly remove the xml declaration
				var settings = new XmlWriterSettings { Indent = true, OmitXmlDeclaration = true };
				using (var writer = XmlWriter.Create(stringWriter, settings))
				{
					xmlserializer.Serialize(writer, originalManagerModelAnimation, emptyNameSpaces);
					var xmlString = stringWriter.ToString();
					using (var tempReader = new StringReader(xmlString))
					{
						ret = (AnimationManagerModelAnimation)xmlserializer.Deserialize(tempReader);
					}
				}
			}
			return ret;
		}
	}
}