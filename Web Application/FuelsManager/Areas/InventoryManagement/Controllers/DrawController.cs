namespace FuelsManager.Areas.InventoryManagement.Controllers
{
	using System;
	using System.Collections.Generic;
	using System.Drawing;
	using System.IO;
	using System.Linq;
	using System.Web;
	using System.Web.Mvc;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;

	using FMPointCommon;

	using FuelsManager.Areas.Controllers;
	using FuelsManager.Areas.InventoryManagement.ViewModels;

	//using EngineeringUnitsLibrary;
	using Varec.CommonComponents.EngineeringUnitsLibrary;
	using Newtonsoft.Json.Linq;
    using Newtonsoft.Json;
	using FMBusinessObjects.Attributes;

    public class DrawController : FMBaseControllerEx
	{
		protected const string DrawViewerID = "DrawViewer";

		[HttpGet]
		public ActionResult DrawIndex(string id)
		{
			var context = this.Session[DrawContext.SessionKey] as DrawContext;
			var model = new DrawModel(context);

			try
			{
				// try to load the drawing state from the previous session
				if (string.IsNullOrEmpty(id) == false)
				{
					var drawingGuid = new Guid(id);
					model.Drawing = FMChannelHelper.MakeCall<IDrawings, Drawing>(x => x.Get(this.Security, drawingGuid));
				}
				else
				{
					model.Drawing = new Drawing();
				}
				var site = FMChannelHelper.MakeCall<ISites, SiteClass>(x => x.Get(this.Security, this.Security.SiteGuid, false, false, false));

				var snfi = new FMNumberFormatInfo
							{
								NegativeSign				= "-",
								NumberDecimalDigits		= 0,
								NumberDecimalSeparator	= site.NumberDecimalSeparator,
								NumberGroupSeparator		= site.NumberGroupSeparator,
								NumberGroupSizes			= site.GetNumberGroupSizes()[0],
								NumberNegativePattern	= 1,
								ShortDatePattern			= site.ShortDatePattern
							};

				model.SiteNumFormatInfo = snfi;
				model.DateTimeFormatInfo = site.GetDateTimeFormatInfo();
				this.PopulateTagUnitToUnitTypeList(model);

				// Create the properties for the properties menu.
				this.CreateCommonProperties(model);
				this.CreateRectangleProperties(model);

				this.Session[DrawContext.SessionKey] = new DrawContext(model);
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}

			return this.View(model);
		}

		[HttpGet, ValidateJsonAntiForgeryToken]
		public ActionResult GetDrawingNames()
		{
			List<DrawingName> availableNames;

			try
			{
				availableNames = FMChannelHelper.MakeCall<IDrawings, List<DrawingName>>(x => x.EnumerateAvailableDrawingNames(this.Security));
			}
			catch (Exception except)
			{
				this.OnError(except);
				return this.JsonWithErrorMessages(null, JsonRequestBehavior.AllowGet);
			}

			return this.JsonWithErrorMessages(availableNames, JsonRequestBehavior.AllowGet);

		}

		[HttpGet, ValidateJsonAntiForgeryToken]
		public ActionResult GePointTemplates()
		{
			List<Tuple<Guid,string>> pointTemplateList = new List<Tuple<Guid, string>>();

			try
			{
				var pointTemplates = FMChannelHelper.MakeCall<IPointTemplates, PointTemplateCollection>(x => x.EnumerateByType(this.Security, null));

				foreach (var pointTemplate in pointTemplates)
				{
					pointTemplateList.Add(new Tuple<Guid, string>(pointTemplate.PointTemplateGuid,pointTemplate.ID));
				}

			}
			catch (Exception except)
			{
				this.OnError(except);
				return this.JsonWithErrorMessages(null, JsonRequestBehavior.AllowGet);
			}

			return this.JsonWithErrorMessages(pointTemplateList, JsonRequestBehavior.AllowGet);

		}

		[HttpGet, ValidateJsonAntiForgeryToken]
		public ActionResult GePointTemplateIdAndType(string pointTemplateGuidStr)
		{
			var pointTemplateIdAndType = new Tuple<string, string>(null,null);

			try
			{
				var pointTemplateGuid = new Guid(pointTemplateGuidStr);
				var pointTemplate = FMChannelHelper.MakeCall<IPointTemplates, PointTemplate>(x => x.Get(this.Security, pointTemplateGuid));
				string pointTemplateTypeStr = null;
				if (pointTemplate.PointTemplateTypeGuid != null)
				{
					var pointTemplateType =
						FMChannelHelper.MakeCall<IApplicationStrings, ApplicationStringClass>(
							x => x.Get(this.Security, (Guid)pointTemplate.PointTemplateTypeGuid));
					pointTemplateTypeStr = pointTemplateType.ID;
				}
				pointTemplateIdAndType = new Tuple<string, string>(pointTemplate.ID, pointTemplateTypeStr);
			}
			catch (Exception except)
			{
				this.OnError(except);
				return this.JsonWithErrorMessages(null, JsonRequestBehavior.AllowGet);
			}

			return this.JsonWithErrorMessages(pointTemplateIdAndType, JsonRequestBehavior.AllowGet);

		}

		[HttpGet, ValidateJsonAntiForgeryToken]
		public ActionResult GetDrawingNamesByPanelType(string panelTypesToFilter)
		{
				List<DrawingName> availableNames;
				try
				{
					var panelTypes = new List<PANELTYPE>();
					foreach (var type in panelTypesToFilter.Split(",".ToCharArray()))
					{
						PANELTYPE panelTypeEnum;
						if (Enum.TryParse(type, out panelTypeEnum))
						{
								panelTypes.Add(panelTypeEnum);
						}
					}
					availableNames = FMChannelHelper.MakeCall<IDrawings, List<DrawingName>>(x => x.EnumerateAvailableDrawingNamesByPanelType(this.Security,panelTypes));
				}
				catch (Exception except)
				{
					this.OnError(except);
					return this.JsonWithErrorMessages(null, JsonRequestBehavior.AllowGet);
				}

				return this.JsonWithErrorMessages(availableNames, JsonRequestBehavior.AllowGet);

		}

		[HttpGet, ValidateJsonAntiForgeryToken]
		public ActionResult GetDrawingNamesByPointTemplate(string pointTemplateGuidString)
		{
				List<DrawingName> availableNames;

				try
				{
					Guid pointTemplateGuid = Guid.Parse(pointTemplateGuidString);
					availableNames = FMChannelHelper.MakeCall<IDrawings, List<DrawingName>>(x => x.EnumerateAvailableDrawingNamesByPointTemplate(this.Security, pointTemplateGuid));
				}
				catch (Exception except)
				{
					this.OnError(except);
					return this.JsonWithErrorMessages(null, JsonRequestBehavior.AllowGet);
				}

				return this.JsonWithErrorMessages(availableNames, JsonRequestBehavior.AllowGet);

		}

		[HttpGet, ValidateJsonAntiForgeryToken]
		public JsonResult GetDrawing(string id)
		{
			try
			{
				var drawingGuid = new Guid(id);
				var drawing = FMChannelHelper.MakeCall<IDrawings, Drawing>(x => x.Get(this.Security, drawingGuid));
				var localImage = drawing.Image;

				// save the drawing guid to the list in the model
				var context = this.Session[DrawContext.SessionKey] as DrawContext;
				var model = new DrawModel(context);
				//model.AddDrawingToList(drawingGuid);

				return this.JsonWithErrorMessages(localImage, JsonRequestBehavior.AllowGet);
			}
			catch (Exception except)
			{
				this.OnError(except);
				return this.JsonWithErrorMessages(null, JsonRequestBehavior.AllowGet);
			}
		}

		[HttpGet, ValidateJsonAntiForgeryToken]
		public JsonResult DeleteDrawing(string id)
		{
			try
			{
				var drawingGuid = new Guid(id);
				FMChannelHelper.MakeCall<IDrawings>(x => x.Purge(this.Security, drawingGuid));

				return this.JsonWithErrorMessages(null, JsonRequestBehavior.AllowGet);
			}
			catch (Exception except)
			{
				this.OnError(except);
				return this.JsonWithErrorMessages(null, JsonRequestBehavior.AllowGet);
			}
		}



		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult SaveDrawing(string name, string description, string image, string panelTypeString, string pointTemplateGuidString, bool published, List<string> animationGuidList)
		{
			string serializedModel;

			try
			{
				// as requested by SI when we save the graphic set the position at 0,0 and the scale at 100%
				// if we ever do implement the ability to save views from a larger file this is where we would make the change
				string[] splitstringarray = image.Split(',');

				if(splitstringarray.Length > 0)
				{
					for(int loop = 0;loop < splitstringarray.Length;loop++)
					{
						if (splitstringarray[loop].IndexOf("\"position\":\"") >= 0)
						{
							splitstringarray[loop] = "\"position\":\"0 0\"";
						}
						else if (splitstringarray[loop].IndexOf("\"scale\":") >= 0)
						{
							splitstringarray[loop] = "\"scale\":1.00";
						}
					}
				}
				// rebuild the image string
				image = string.Empty;
				for (int loop = 0; loop < splitstringarray.Length; loop++)
				{
					if (loop > 0)
					{
						image += ',';
					}
					image += splitstringarray[loop];
				}


				var context = this.Session[DrawContext.SessionKey] as DrawContext;
				var model = new DrawModel(context);

				if (model.Drawing == null)
				{
					model.Drawing = new Drawing
					{
						ID = name,
						Description = description,
						Image = image,
						Published = published,
					};
				}
				else
				{
					model.Drawing.ID = name;
					model.Drawing.Description = description;
					model.Drawing.Image = image;
					model.Drawing.Published = published;
				}

				var panelType = PANELTYPE.Standard;
				if(!PANELTYPE.TryParse(panelTypeString, out panelType))
				{
					throw new FormatException("Panel Type " + panelTypeString + " is not supported");
				}
				else
				{
					model.Drawing.PanelType = panelType;
				}
				var pointTemplateGuid = Guid.Empty;
				if(panelType == PANELTYPE.Detail && Guid.TryParse(pointTemplateGuidString,out pointTemplateGuid))
				{
					model.Drawing.PointTemplateGuid = pointTemplateGuid;
				}
				else
					model.Drawing.PointTemplateGuid = null;

				model.Drawing.DrawingGuid = FMChannelHelper.MakeCall<IDrawings, Guid>(x => x.GetIdentityGuid(this.Security, model.Drawing.ID));

				List<Guid> animationGuids = new List<Guid>();
				if (animationGuidList != null && animationGuidList.Count > 0)
				{
					foreach (var guidString in animationGuidList)
					{
						var animationGuid = new Guid(guidString);
						animationGuids.Add(animationGuid);
					}
				}
				model.Drawing.AnimationGuidList = animationGuids;


				if (model.Drawing.DrawingGuid != Guid.Empty)
				{
					FMChannelHelper.MakeCall<IDrawings>(x => x.Modify(this.Security, model.Drawing));
				}
				else
				{
					model.Drawing = FMChannelHelper.MakeCall<IDrawings, Drawing>(x => x.Add(this.Security, model.Drawing));
				}

				serializedModel = model.Drawing.DrawingGuid.ToString();

				// save the drawing to the list
				//model.AddDrawingToList(model.Drawing.DrawingGuid);
				this.Session[DrawContext.SessionKey] = new DrawContext(model);
			}
			catch (Exception except)
			{
				this.OnError(except);
				return this.JsonWithErrorMessages(null, JsonRequestBehavior.AllowGet);
			}

			this.AddSuccess( "Save successful.");
			return this.JsonWithErrorMessages(serializedModel, JsonRequestBehavior.AllowGet);
		}

		[HttpGet, ValidateJsonAntiForgeryToken]
		public ActionResult ImageHashExists(string imageHash)
		{
			string results;

			try
			{
				//Retrive PictureGuid
				var pictureGuid = FMChannelHelper.MakeCall<IPictures, Guid>(x => x.GetPictureGuidByImageHash(this.Security, imageHash));
				results = (pictureGuid == Guid.Empty) ? string.Empty : pictureGuid.ToString();
			}
			catch (Exception except)
			{
				this.OnError(except);
				return this.JsonWithErrorMessages(null, JsonRequestBehavior.AllowGet);
			}

			return this.JsonWithErrorMessages(results, JsonRequestBehavior.AllowGet);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult SaveImage(string name, string type, string imageString)
		{
			var results = new string[2];

			try
			{
				int indexOfFirstComma = imageString.IndexOf(",", 0);

				if (indexOfFirstComma == -1)
				{
					throw new Exception("Improper Image String Format");
				}

				byte[] imageBytes;

				// Convert Base64 String to byte[]
				try
				{
					imageBytes = Convert.FromBase64String(imageString.Substring(indexOfFirstComma + 1));
				}
				catch
				{
					throw new Exception("Unable to decode image string");
				}

				var picture = new Picture
				{
					ImageStream = imageBytes,
					ID = name,
					Description = "Uploaded file",
					ContentType = type
				};

				// Save it in the database
				var pictureGuid = FMChannelHelper.MakeCall<IPictures, Guid>(x => x.Add(this.Security, picture));

				results[0] = pictureGuid.ToString();
				results[1] = picture.ImageHash;
			}
			catch (Exception except)
			{
				this.OnError(except);
				return this.JsonWithErrorMessages(null, JsonRequestBehavior.AllowGet);
			}
			return this.JsonWithErrorMessages(results);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public JsonResult ButtonTagEdit(string guid, string value)
		{

			PointTag tag = new PointTag { PointTagGuid = Guid.Parse(guid), Value = double.Parse(value) };

			try
			{
				FMChannelHelper.MakeCall<IPointServiceManager>(x => x.SetPointValueData(this.Security, new List<PointValue>() { new PointValue(tag) }, false));
				return this.Json("Point Value guid " + guid + " changed to " + value);
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
				return this.Json(except);
			}
		}

		[HttpGet]
		public FileContentResult Export(string id)
		{
			SiteClass site = null;
			var drawing = new Drawing();
			string animationsString = null;
			string picturesString = null;
			List<Picture> pictureList = new List<Picture>();
  
			try
			{
				site = FMChannelHelper.MakeCall<ISites, SiteClass>(x => x.Get(this.Security, this.Security.SiteGuid, false, false, false));
                // try to load the drawing state from the previous session
                if (string.IsNullOrEmpty(id) == false)
                {
                    var drawingGuid = new Guid(id);

					//Animation handling
                    List<Guid> animationGuidList = new List<Guid>();
                    drawing = FMChannelHelper.MakeCall<IDrawings, Drawing>(x => x.Get(this.Security, drawingGuid));
                    Dictionary<Guid, AnimationToDrawingMapClass> animationMappingGuidList =
                            FMChannelHelper.MakeCall<IAnimationDrawingMaps, Dictionary<Guid, AnimationToDrawingMapClass>>(x => x.EnumerateByDrawingGuids(this.Security, new List<Guid> {drawingGuid} ));
                    foreach (KeyValuePair<Guid,AnimationToDrawingMapClass> mapPair in animationMappingGuidList)
                    {
                        animationGuidList.Add(mapPair.Value.AnimationGuid);
                    }
                    animationsString = this.GetAnimationString(animationGuidList);

					//Picture handling
					JObject graphicImageJSON = JObject.Parse(drawing.Image);
					var ListofGraphics = graphicImageJSON["model"]["nodeDataArray"].Where(s => (string)s["category"] == "picture").Select(x => x["imageGuid"]).ToList();
					foreach( var pictureguid in ListofGraphics)
					{
						var picture = FMChannelHelper.MakeCall<IPictures, Picture>(x => x.Get(this.Security, (Guid)pictureguid));

						pictureList.Add(picture);

					}
					picturesString = this.GetPictureString(pictureList);
				}
				else
				{
					drawing = new Drawing();
					throw new Exception("Invalid Drawing");
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
			Response.AddHeader("Content-Disposition", "attachment; filename=" + drawing.ID + ".fmg");

			// build the object to save to the file
			string json = @"{
						version: '1.0',
						name: 'FuelsManager Graphic Export',
						pointTemplateGuid: '" + drawing.PointTemplateGuid + "'," +
						"graphic: " + drawing.Image + "," +
						"pictures: " + picturesString + "," +
						"animations: " + animationsString + "," +
				"}";

			JObject o = JObject.Parse(json);

			return new FileContentResult(System.Text.Encoding.UTF8.GetBytes(o.ToString()), System.Net.Mime.MediaTypeNames.Application.Octet);
		}

        private string GetAnimationString(List<Guid> AnimationGuidList)
        {
            var animations = FMChannelHelper.MakeCall<IAnimations, Dictionary<Guid, AnimationClass>>(x => x.EnumerateByAnimationGuids(this.Security, AnimationGuidList));

            var animationModel = new AnimationManagerModel();

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
            animationModel.SortAnimationsAlphabetically();
            return JsonConvert.SerializeObject(animationModel.AnimationList);        
        }

        protected static List<AnimationManagerModelTestGroup> CreateAnimationModelFromAnimationTestGroups(AnimationClass animation)
        {
            var animationTestGroups = new List<AnimationManagerModelTestGroup>();
            foreach (var animeTg in animation.AnimationTestGroupList)
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

		private string GetPictureString(List <Picture> pictureList)
		{
			List<PointDetailPictures> ListOfPictures = new List<PointDetailPictures>();
			foreach (var pic in pictureList)
			{
				var picture = new PointDetailPictures
				{
					PictureGuid = pic.PictureGuid,
					ID			= pic.ID,
					ImageStream = pic.ImageStream,
					Description = pic.Description,
					ContentType = pic.ContentType,
					IsSystemImage = pic.IsSystemImage,
					ImageHash	= pic.ImageHash
				};
				ListOfPictures.Add(picture);
			}
			return JsonConvert.SerializeObject(ListOfPictures);
		}

		[HttpPost, ValidateJsonAntiForgeryToken]
		public ActionResult Import(Guid pointTagTemplateGuid, string pointDetailName, bool published, string pointDetailGraphic, string pointDetailPictures, string pointDetailAnimations)
		{
			SiteClass site = null;
			string drawingModel = string.Empty;
			var drawing = new Drawing();
			var isNewAnimation = false;
			try
			{
				site = FMChannelHelper.MakeCall<ISites, SiteClass>(x => x.Get(this.Security, this.Security.SiteGuid, false, false, false));

				// process the images (if an image with the same Hash exists it will not create a new one, it will use the existing one)
				JObject pictureObj = new JObject();
				pictureObj = JObject.Parse("{ pictures:" + pointDetailPictures + "}");
				foreach (var picture in pictureObj["pictures"])
				{
					var pictureGuid = (Guid)picture["PictureGuid"];
					var ImageHash = (string)picture["ImageHash"];
					var id = (string)picture["ID"];
					var description = (string)picture["Description"];
					var imageStream = (byte[])picture["ImageStream"];
					var contentType = (string)picture["ContentType"];
					var isSystemImage = (bool)picture["IsSystemImage"];

					var pictureRecord = FMChannelHelper.MakeCall<IPictures, Picture>(x => x.Get(this.Security, pictureGuid));
					if ((pictureRecord != null && pictureRecord.PictureGuid == Guid.Empty) ||
						(pictureRecord.SiteGuid != this.Security.SiteGuid && pictureRecord.SiteGuid != FMBusinessObjects.Constants.Guids.SiteAdminGuid))
					{
						pictureRecord.PictureGuid = Guid.NewGuid();
						pictureRecord.IdentityGuid = pictureRecord.PictureGuid;
						pictureRecord.ID = id;
						pictureRecord.Description = description;
						pictureRecord.ImageStream = imageStream;
						pictureRecord.IsSystemImage = isSystemImage;
						pictureRecord.ContentType = contentType;
						pictureRecord.SiteGuid = this.Security.SiteGuid;
						pictureRecord.Deleted = false;
						var newPictureGuid = FMChannelHelper.MakeCall<IPictures, Guid>(x => x.Add(this.Security, pictureRecord));
						// if it didn't create a new picture because it already existed but with a different Guid we need to use the new guid
						if (newPictureGuid != pictureGuid)
						{
							pointDetailGraphic = pointDetailGraphic.Replace(pictureGuid.ToString(), newPictureGuid.ToString());
						}
					}

				}

				// process the animations
				JObject animationObj = new JObject();
				animationObj = JObject.Parse("{ animations:" + pointDetailAnimations + "}");
				foreach(var animation in animationObj["animations"])
				{
					var animationGuid = (Guid)animation["AnimationGuid"];
					if (animationGuid.Equals(Guid.Empty))
					{
						animationGuid = Guid.NewGuid();
					}
					var id = (string)animation["ID"];
					var animationTestGroups = animation["AnimationTestGroups"];

					var animationRecord = FMChannelHelper.MakeCall<IAnimations, AnimationClass>(x => x.Get(this.Security, animationGuid));
					if (animationRecord == null)
					{
						isNewAnimation = true;
						animationRecord = new AnimationClass();
						animationRecord.AnimationGuid = animationGuid;
						animationRecord.IdentityGuid = animationGuid;
						animationRecord.UseCount = 1;
						animationRecord.SiteGuid = this.Security.SiteGuid;
						animationRecord.ID = id;

					} else {
						isNewAnimation = false;
					}

						animationRecord.AnimationTestGroupList = new List<AnimationTestGroup>();
						foreach( var animationTestGroup in animationTestGroups)
						{
							var newAnimationTestGroup = new AnimationTestGroup();
							newAnimationTestGroup.AnimationTestGroupGuid = (Guid)animationTestGroup["AnimationTestGroupGuid"];
							newAnimationTestGroup.ID = (string)animationTestGroup["ID"];
							newAnimationTestGroup.DataType = (string)animationTestGroup["DataType"];
							newAnimationTestGroup.Field = (PointValueFieldType)Enum.Parse(typeof(PointValueFieldType), animationTestGroup["Field"].ToString());
							newAnimationTestGroup.TestList = new List<AnimationTest>();
							foreach (var animationTest in animationTestGroup["TestList"])
							{
								var newAnimationTest = new AnimationTest();
								newAnimationTest.AnimationTestGuid = (Guid)animationTest["AnimationTestGuid"];
								newAnimationTest.TestComparisonOperator = (EAnimationTestComparisonOperators)Enum.Parse(typeof(EAnimationTestComparisonOperators), animationTest["TestComparisonOperator"].ToString());
								newAnimationTest.BitmaskStr = (string)animationTest["BitmaskStr"];
								newAnimationTest.Bitmask = (long)animationTest["Bitmask"];
								newAnimationTest.BitmaskOperator = (EAnimationTestBitmaskOperators)Enum.Parse(typeof(EAnimationTestBitmaskOperators), animationTest["BitmaskOperator"].ToString());
								newAnimationTest.ComparisonValue = (string)animationTest["ComparisonValue"];
								newAnimationTest.PropertyList = new List<AnimationProperty>();
								foreach (var property in animationTest["PropertyList"])
								{
									var newProperty = new AnimationProperty();
									newProperty.AnimationPropertyGuid = (Guid)property["AnimationPropertyGuid"];
									newProperty.Name = (string)property["Name"];
									newProperty.LookupName = (string)property["LookupName"];
									newProperty.gojsPropertyName = (string)property["gojsPropertyName"];
									newProperty.VisualStates = new List<AnimationPropertyVisualState>();
									foreach (var visualState in property["VisualStates"])
									{
										var newVisualState = new AnimationPropertyVisualState();
										newVisualState.AnimationPropertyVisualStateGuid = (Guid)visualState["AnimationPropertyVisualStateGuid"];
										newVisualState.Value = (string)visualState["Value"];
										newProperty.VisualStates.Add(newVisualState);
									}
									newAnimationTest.PropertyList.Add(newProperty);
								}
								newAnimationTestGroup.TestList.Add(newAnimationTest);
							}
							animationRecord.AnimationTestGroupList.Add(newAnimationTestGroup);
						}

					if ( isNewAnimation == true ) { 
						var newAnimationGuid = FMChannelHelper.MakeCall<IAnimations, Guid>(x => x.Add(this.Security, animationRecord));
					} else {
						FMChannelHelper.MakeCall<IAnimations>(x => x.Modify(this.Security, animationRecord));
					}

				}


				drawing.AnimationGuidList = new List<Guid>();
					drawing.Description = "Imported";
					drawing.DrawingGuid = Guid.NewGuid();
					drawing.IdentityGuid = drawing.DrawingGuid;
					drawing.Published = published;
					drawing.Image = pointDetailGraphic;
					drawing.PanelType = PANELTYPE.Detail;
					drawing.PointTemplateGuid = pointTagTemplateGuid;  // standard template
					drawing.SiteGuid = this.Security.SiteGuid;
					drawing.ID = pointDetailName;
					// drawing name has to be unique
					var drawingList = FMChannelHelper.MakeCall<IDrawings, List<DrawingName>>(x => x.EnumerateAvailableDrawingNames(this.Security));

					if (!drawingList.Any(x => x.ID == pointDetailName))
					{
						drawing = FMChannelHelper.MakeCall<IDrawings, Drawing>(x => x.Add(this.Security, drawing));
					}
					else
					{
						this.OnError("A Point Detail with the same name already exists!");
						return this.JsonWithErrorMessages(null);
					}


				this.AddSuccess("Import successful");
			}
			catch (Exception except)
			{
				this.OnError(except);
				return this.JsonWithErrorMessages(null);
			}
			return this.JsonWithErrorMessages(drawing.DrawingGuid);
		}

		// get the list of ValuePointIdentifiers for PointTemplates
		[HttpPost, ValidateJsonAntiForgeryToken]
		public ActionResult GetImportWizardMappingOptions(Guid pointTemplateGuid, string pointDetailAnimations)
		{
			var pointValueIdentifiers = new List<pointValueIdentifierNamedWitDataType>();

			try
			{

				PointTemplate pointTemplate = FMChannelHelper.MakeCall<IPointTemplates, PointTemplate>(x => x.Get(this.Security, pointTemplateGuid));
				// Tags
				foreach (var pointTemplateTag in pointTemplate.Tags.Values)
				{
					pointValueIdentifiers.Add(new pointValueIdentifierNamedWitDataType
					{
						ID = pointTemplateTag.ID,
						pointValueIdentifier = new PointValueIdentifier(pointTemplateTag),
						DataType = pointTemplateTag.ValueTypeString,
						UnitType = pointTemplateTag.EngineeringUnitsType.ToString()
					});
				}


				// settings
				foreach (var pointTemplateProperty in pointTemplate.Properties.Values)
				{
					var propertyType = pointTemplateProperty.Value.GetType();
					var propertyInfos = propertyType.GetProperties();

					foreach (var propertyInfo in propertyInfos)
					{
						var fmExposedSettings = propertyInfo.GetCustomAttributes(typeof(FMExposedSetting), false);
						if (fmExposedSettings.Length == 0)
						{
							continue;
						}
						var pointValueIdentifier = new PointValueIdentifier()
						{
							IdentityGuid = pointTemplateProperty.PointTemplatePropertyGuid,
							PointValueType = PointValueType.Setting,
							PropertyID = propertyInfo.Name
						};


						var fmExposedSetting = fmExposedSettings[0] as FMExposedSetting;
						var localId = "";
						var localDataType = "";
						var localUnitType = "";
						if (fmExposedSetting != null)
						{
							localId = fmExposedSetting.ID;
							localUnitType = fmExposedSetting.EngineeringUnitsType.ToString();
							localDataType = propertyInfo.PropertyType.ToString();
						}

						pointValueIdentifiers.Add(new pointValueIdentifierNamedWitDataType
						{
							ID = localId,
							pointValueIdentifier = pointValueIdentifier,
							DataType = localDataType,
							UnitType = localUnitType
						});
					}

				}

				// point
				var point = new FMBusinessObjects.DataObjects.Point();
				foreach (var pointValueIdentifier in point.GetExposedSettingPointValueIdentifiers())
				{
					var pointValueId = point.GetExposedSettingIDFilterByType(pointValueIdentifier, false, "", PointValueFieldType.ID);
					if (pointValueId != string.Empty)
					{
						pointValueIdentifier.IdentityGuid = pointTemplate.PointTemplateGuid;
						pointValueIdentifiers.Add(new pointValueIdentifierNamedWitDataType
						{
							ID = pointValueId,
							pointValueIdentifier = pointValueIdentifier,
							DataType = "System.String",
							UnitType = "FmuNodim"
						});

					}
				}

				// Get the list of points for the Template
				var points = FMChannelHelper.MakeCall<IPoints, PointCollection>(x => x.EnumerateBySite(this.Security, this.Security.SiteGuid)); 
				var pointNameList = points.Where( x =>x.PointTemplateGuid == pointTemplateGuid)
														.Select(x => new { x.PointGuid, x.PointId })
														.AsEnumerable()
														.Select(o => new KeyValuePair<Guid, string>(o.PointGuid, o.PointId))
														.ToList();

				// Get the graphics for the site
				var drawings = FMChannelHelper.MakeCall<IDrawings, List<DrawingName>>(x => x.EnumerateAvailableDrawingNamesByPanelType(this.Security, new List<PANELTYPE> { PANELTYPE.Standard }));
				var drawingsList = drawings.Select(x => new { x.DrawingGuid, x.ID }).AsEnumerable().Select(o => new KeyValuePair<Guid, string>(o.DrawingGuid, o.ID)).ToList();

				var wizardOptions = new wizardOptions();
				wizardOptions.pointValueIdentifiers = pointValueIdentifiers;
				wizardOptions.pointNameList = pointNameList;
				wizardOptions.drawingsList = drawingsList;
				wizardOptions.animationList = new List<KeyValuePair<Guid, string>>();

				// Look into the animations
				JObject animationObj = new JObject();
				animationObj = JObject.Parse("{ animations:" + pointDetailAnimations + "}");
				foreach (var animation in animationObj["animations"])
				{
					var animationGuid = (Guid)animation["AnimationGuid"];
					var id = (string)animation["ID"];
					var animationTestGroups = animation["AnimationTestGroups"];
					AnimationClass existingAnimationRecord = null;

//					existingAnimationRecord = FMChannelHelper.MakeCall<IAnimations, AnimationClass>(x => x.Get(this.Security, animationGuid));

					var siteAnimations = FMChannelHelper.MakeCall<IAnimations, Dictionary<Guid,AnimationClass>>(x => x.EnumerateAnimationsBySiteGuid(this.Security, this.Security.SiteGuid));
					if (siteAnimations.ContainsKey(animationGuid) )
					{
						existingAnimationRecord = siteAnimations[animationGuid];
					} else
					{
						var existingWithDifferentID = siteAnimations.Values.FirstOrDefault(x => x.ID == id);
						// we found the animation but has a different guid
						if (existingWithDifferentID != null && existingWithDifferentID.AnimationGuid != animationGuid )
						{
							animationGuid = existingWithDifferentID.AnimationGuid;
							existingAnimationRecord = existingWithDifferentID;
						}
					}


					// if animation does not exists we can safely create it
					if (existingAnimationRecord == null)
					{
						wizardOptions.animationList.Add(new KeyValuePair<Guid,string>(animationGuid, "NEW"));

					} else { 
						// if the animation already exists we need to check if they are the same or it has been updated
						// we need to build the animation object to be able to compare it
						var animationRecord = new AnimationClass();
						animationRecord.AnimationGuid = animationGuid;
						animationRecord.IdentityGuid = animationGuid;
						animationRecord.ID = id;
						animationRecord.UseCount = 1;
						animationRecord.SiteGuid = this.Security.SiteGuid;
						animationRecord.AnimationTestGroupList = new List<AnimationTestGroup>();
						foreach (var animationTestGroup in animationTestGroups)
						{
							var newAnimationTestGroup = new AnimationTestGroup();
							newAnimationTestGroup.AnimationTestGroupGuid = (Guid)animationTestGroup["AnimationTestGroupGuid"];
							newAnimationTestGroup.ID = (string)animationTestGroup["ID"];
							newAnimationTestGroup.DataType = (string)animationTestGroup["DataType"];
							newAnimationTestGroup.Field = (PointValueFieldType)Enum.Parse(typeof(PointValueFieldType), animationTestGroup["Field"].ToString());
							newAnimationTestGroup.TestList = new List<AnimationTest>();
							foreach (var animationTest in animationTestGroup["TestList"])
							{
								var newAnimationTest = new AnimationTest();
								newAnimationTest.AnimationTestGuid = (Guid)animationTest["AnimationTestGuid"];
								newAnimationTest.TestComparisonOperator = (EAnimationTestComparisonOperators)Enum.Parse(typeof(EAnimationTestComparisonOperators), animationTest["TestComparisonOperator"].ToString());
								newAnimationTest.BitmaskStr = (string)animationTest["BitmaskStr"];
								newAnimationTest.Bitmask = (long)animationTest["Bitmask"];
								newAnimationTest.BitmaskOperator = (EAnimationTestBitmaskOperators)Enum.Parse(typeof(EAnimationTestBitmaskOperators), animationTest["BitmaskOperator"].ToString());
								newAnimationTest.ComparisonValue = (string)animationTest["ComparisonValue"];
								newAnimationTest.PropertyList = new List<AnimationProperty>();
								foreach (var property in animationTest["PropertyList"])
								{
									var newProperty = new AnimationProperty();
									newProperty.AnimationPropertyGuid = (Guid)property["AnimationPropertyGuid"];
									newProperty.Name = (string)property["Name"];
									newProperty.LookupName = (string)property["LookupName"];
									newProperty.gojsPropertyName = (string)property["gojsPropertyName"];
									newProperty.VisualStates = new List<AnimationPropertyVisualState>();
									foreach (var visualState in property["VisualStates"])
									{
										var newVisualState = new AnimationPropertyVisualState();
										newVisualState.AnimationPropertyVisualStateGuid = (Guid)visualState["AnimationPropertyVisualStateGuid"];
										newVisualState.Value = (string)visualState["Value"];
										newProperty.VisualStates.Add(newVisualState);
									}
									newAnimationTest.PropertyList.Add(newProperty);
								}
								newAnimationTestGroup.TestList.Add(newAnimationTest);
							}
							animationRecord.AnimationTestGroupList.Add(newAnimationTestGroup);
						}
						// once we have build the animation object we can compare the animation in the system with the one in the export.
						var importAnimationJson = JsonConvert.SerializeObject(animationRecord.AnimationTestGroupList, Formatting.None);
						var ExistingAnimationJson = JsonConvert.SerializeObject(existingAnimationRecord.AnimationTestGroupList, Formatting.None);

						if (ExistingAnimationJson.ToString().Equals(importAnimationJson.ToString())) {
							wizardOptions.animationList.Add(new KeyValuePair<Guid, string>(animationGuid, "SAME"));
						} else {
							wizardOptions.animationList.Add(new KeyValuePair<Guid, string>(animationGuid, "DIFFERENT"));
						}
					}

				}


				return this.JsonWithErrorMessages(wizardOptions, JsonRequestBehavior.AllowGet);
			}
			catch (Exception except)
			{
				this.OnError(except);
				return this.JsonWithErrorMessages(null, JsonRequestBehavior.AllowGet);
			}

		}

		// get the list of PointTemplates
		[HttpGet, ValidateJsonAntiForgeryToken]
		public ActionResult GetListPointTemplates()
		{
			try
			{
				//Retrieve Point Templates for site
				var pointTemplates = FMChannelHelper.MakeCall<IPointTemplates, PointTemplateCollection>(x => x.EnumerateByType(this.Security, null));
				var results = pointTemplates.Select(x => new { x.PointTemplateGuid, x.ID}).ToList();
				return this.JsonWithErrorMessages(results, JsonRequestBehavior.AllowGet);
			}
			catch (Exception except)
			{
				this.OnError(except);
				return this.JsonWithErrorMessages(null, JsonRequestBehavior.AllowGet);
			}
		}

		#region Private methods
		/// <summary>
		/// This method will create the common properties that is used to build the
		/// property menu.
		/// </summary>
		/// <param name="drawModel">The draw model</param>
		private void CreateCommonProperties(DrawModel drawModel)
		{
			List<DrawPropertyMenuRecord> commonPropertyList = new List<DrawPropertyMenuRecord>();

			//================================================================
			// Main Common section
			//================================================================
			var propertyMenuRecord = new DrawPropertyMenuRecord
			{
				PropertyName = "MAINPROPDIVIDER",
				ControlType = DrawPropertyMenuRecord.PropertyControlTypes.Divider,
				Hide = false,
				SectionName = DrawPropertyMenuRecord.SectionNameSectionMain,
				SectionLabelName = "Main Section"
			};
			commonPropertyList.Add(propertyMenuRecord);

			propertyMenuRecord = new DrawPropertyMenuRecord
			{
				PropertyName = "TOP",
				PropertyLabelName = "Top (coordinate)",
				ControlType = DrawPropertyMenuRecord.PropertyControlTypes.Textbox,
				Hide = false,
				SectionName = DrawPropertyMenuRecord.SectionNameSectionMain
			};
			commonPropertyList.Add(propertyMenuRecord);

			propertyMenuRecord = new DrawPropertyMenuRecord
			{
				PropertyName = "LEFT",
				PropertyLabelName = "Left (coordinate)",
				ControlType = DrawPropertyMenuRecord.PropertyControlTypes.Textbox,
				Hide = false,
				SectionName = DrawPropertyMenuRecord.SectionNameSectionMain
			};
			commonPropertyList.Add(propertyMenuRecord);

			propertyMenuRecord = new DrawPropertyMenuRecord
			{
				PropertyName = "HEIGHT",
				PropertyLabelName = "Height",
				ControlType = DrawPropertyMenuRecord.PropertyControlTypes.Textbox,
				Hide = false,
				SectionName = DrawPropertyMenuRecord.SectionNameSectionMain
			};
			commonPropertyList.Add(propertyMenuRecord);

			propertyMenuRecord = new DrawPropertyMenuRecord
			{
				PropertyName = "WIDTH",
				PropertyLabelName = "Width",
				ControlType = DrawPropertyMenuRecord.PropertyControlTypes.Textbox,
				Hide = false,
				SectionName = DrawPropertyMenuRecord.SectionNameSectionMain
			};
			commonPropertyList.Add(propertyMenuRecord);

			propertyMenuRecord = new DrawPropertyMenuRecord
			{
				PropertyName = "ANGLE",
				PropertyLabelName = "Angle",
				ControlType = DrawPropertyMenuRecord.PropertyControlTypes.Textbox,
				Hide = false,
				SectionName = DrawPropertyMenuRecord.SectionNameSectionMain
			};
			commonPropertyList.Add(propertyMenuRecord);

				propertyMenuRecord = new DrawPropertyMenuRecord
				{
					PropertyName = "LAYER",
					PropertyLabelName = "Layer",
					ControlType = DrawPropertyMenuRecord.PropertyControlTypes.Dropdown,
					DropdownWidth = "140px",
					Hide = false,
				SectionName = DrawPropertyMenuRecord.SectionNameSectionMain
			};
				commonPropertyList.Add(propertyMenuRecord);

				propertyMenuRecord = new DrawPropertyMenuRecord
			{
				PropertyName = "ZORDER",
				PropertyLabelName = "Z-Order",
				ControlType = DrawPropertyMenuRecord.PropertyControlTypes.Textbox,
				Hide = false,
				SectionName = DrawPropertyMenuRecord.SectionNameSectionMain
			};
			commonPropertyList.Add(propertyMenuRecord);

			//================================================================
			// Fill Color section
			//================================================================
			propertyMenuRecord = new DrawPropertyMenuRecord
			{
				PropertyName = "FILLCOLORDIVIDER",
				ControlType = DrawPropertyMenuRecord.PropertyControlTypes.Divider,
				Hide = false,
				SectionName = DrawPropertyMenuRecord.SectionNameSectionFillColor,
				SectionLabelName = "Fill Section"
			};
			commonPropertyList.Add(propertyMenuRecord);

			propertyMenuRecord = new DrawPropertyMenuRecord
			{
				PropertyName = "FILLCOLOR",
				PropertyLabelName = "Fill Color",
				ControlType = DrawPropertyMenuRecord.PropertyControlTypes.FillColor,
				Hide = false,
				FillColorHex = "#ffffff",
				SectionName = DrawPropertyMenuRecord.SectionNameSectionFillColor
			};
			commonPropertyList.Add(propertyMenuRecord);

			propertyMenuRecord = new DrawPropertyMenuRecord
			{
				PropertyName = "FILLCOLORSPECTRUM",
				PropertyLabelName = "Fill Color spectrum",
				ControlType = DrawPropertyMenuRecord.PropertyControlTypes.FillColorSpectrum,
				Hide = true,
				SectionName = DrawPropertyMenuRecord.SectionNameSectionFillColor
			};
			commonPropertyList.Add(propertyMenuRecord);

			propertyMenuRecord = new DrawPropertyMenuRecord
			{
				PropertyName = "PATTERNCOLOR",
				PropertyLabelName = "Pattern Color",
				ControlType = DrawPropertyMenuRecord.PropertyControlTypes.FillColor,
				Hide = false,
				SectionName = DrawPropertyMenuRecord.SectionNameSectionFillColor
			};
			commonPropertyList.Add(propertyMenuRecord);

			propertyMenuRecord = new DrawPropertyMenuRecord
			{
				PropertyName = "PATTERNCOLORSPECTRUM",
				PropertyLabelName = "Fill Color spectrum",
				ControlType = DrawPropertyMenuRecord.PropertyControlTypes.FillColorSpectrum,
				Hide = true,
				SectionName = DrawPropertyMenuRecord.SectionNameSectionFillColor
			};
			commonPropertyList.Add(propertyMenuRecord);

			propertyMenuRecord = new DrawPropertyMenuRecord
			{
				PropertyName = "BGFILLCOLOR",
				PropertyLabelName = "Background Fill Color",
				ControlType = DrawPropertyMenuRecord.PropertyControlTypes.FillColor,
				Hide = true,
				FillColorHex = "#ffffff",
				SectionName = DrawPropertyMenuRecord.SectionNameSectionFillColor
			};
			commonPropertyList.Add(propertyMenuRecord);

			propertyMenuRecord = new DrawPropertyMenuRecord
			{
				PropertyName = "BGFILLCOLORSPECTRUM",
				PropertyLabelName = "Fill Color spectrum",
				ControlType = DrawPropertyMenuRecord.PropertyControlTypes.FillColorSpectrum,
				Hide = true,
				SectionName = DrawPropertyMenuRecord.SectionNameSectionFillColor
			};
			commonPropertyList.Add(propertyMenuRecord);

			propertyMenuRecord = new DrawPropertyMenuRecord
			{
				PropertyName = "FILLPATTERN",
				PropertyLabelName = "Fill Pattern",
				ControlType = DrawPropertyMenuRecord.PropertyControlTypes.FillPattern,
				Hide = false,
				SectionName = DrawPropertyMenuRecord.SectionNameSectionFillColor
			};
			commonPropertyList.Add(propertyMenuRecord);

			propertyMenuRecord = new DrawPropertyMenuRecord
			{
				PropertyName = "FILLPATTERNPALETTE",
				PropertyLabelName = "Choose Pattern",
				ControlType = DrawPropertyMenuRecord.PropertyControlTypes.FillPatternPalette,
				ControlSubType = DrawPropertyMenuRecord.PropertyControlSubTypes.FillPatternType,
				Hide = true,
				SectionName = DrawPropertyMenuRecord.SectionNameSectionFillColor
			};
			this.GetFillPatternNumbers(propertyMenuRecord);
			commonPropertyList.Add(propertyMenuRecord);

			propertyMenuRecord = new DrawPropertyMenuRecord
			{
				PropertyName = "TRANSPARENCY",
				PropertyLabelName = "Fill Transparency (%)",
				ControlType = DrawPropertyMenuRecord.PropertyControlTypes.Textbox,
				Hide = false,
				SectionName = DrawPropertyMenuRecord.SectionNameSectionFillColor
			};
			commonPropertyList.Add(propertyMenuRecord);

			propertyMenuRecord = new DrawPropertyMenuRecord
			{
				PropertyName = "BGTRANSPARENCY",
				PropertyLabelName = "Background Transparency (%)",
				ControlType = DrawPropertyMenuRecord.PropertyControlTypes.Textbox,
				Hide = false,
				SectionName = DrawPropertyMenuRecord.SectionNameSectionFillColor
			};
			commonPropertyList.Add(propertyMenuRecord);

			//================================================================
			// Line section
			//================================================================
			propertyMenuRecord = new DrawPropertyMenuRecord
			{
				PropertyName = "LINEPROPDIVIDER",
				ControlType = DrawPropertyMenuRecord.PropertyControlTypes.Divider,
				Hide = false,
				SectionName = DrawPropertyMenuRecord.SectionNameSectionLine,
				SectionLabelName = "Line Section"
			};
			commonPropertyList.Add(propertyMenuRecord);

			propertyMenuRecord = new DrawPropertyMenuRecord
			{
				PropertyName = "LINESIZE",
				PropertyLabelName = "Line Size",
				ControlType = DrawPropertyMenuRecord.PropertyControlTypes.Dropdown,
				Hide = false,
				SectionName = DrawPropertyMenuRecord.SectionNameSectionLine
			};
			this.GetLineSizeDropdownList(propertyMenuRecord);
			commonPropertyList.Add(propertyMenuRecord);

			propertyMenuRecord = new DrawPropertyMenuRecord
			{
				PropertyName = "LINECOLOR",
				PropertyLabelName = "Line Color",
				ControlType = DrawPropertyMenuRecord.PropertyControlTypes.FillColor,
				Hide = false,
				FillColorHex = "#ffffff",
				SectionName = DrawPropertyMenuRecord.SectionNameSectionLine
			};
			commonPropertyList.Add(propertyMenuRecord);

			propertyMenuRecord = new DrawPropertyMenuRecord
			{
				PropertyName = "LINECOLORSPECTRUM",
				PropertyLabelName = "Line Color spectrum",
				ControlType = DrawPropertyMenuRecord.PropertyControlTypes.FillColorSpectrum,
				Hide = true,
				SectionName = DrawPropertyMenuRecord.SectionNameSectionLine
			};
			commonPropertyList.Add(propertyMenuRecord);

			propertyMenuRecord = new DrawPropertyMenuRecord
			{
				PropertyName = "LINESTYLE",
				PropertyLabelName = "Line Style",
				ControlType = DrawPropertyMenuRecord.PropertyControlTypes.FillPattern,
				Hide = false,
				SectionName = DrawPropertyMenuRecord.SectionNameSectionLine
			};
			commonPropertyList.Add(propertyMenuRecord);

			propertyMenuRecord = new DrawPropertyMenuRecord
			{
				PropertyName = "LINESTYLEPALETTE",
				PropertyLabelName = "Choose Style",
				ControlType = DrawPropertyMenuRecord.PropertyControlTypes.FillPatternPalette,
				ControlSubType = DrawPropertyMenuRecord.PropertyControlSubTypes.LineStyleType,
				Hide = true,
				SectionName = DrawPropertyMenuRecord.SectionNameSectionLine
			};
			this.GetLineStyleNumbers(propertyMenuRecord);
			commonPropertyList.Add(propertyMenuRecord);

			propertyMenuRecord = new DrawPropertyMenuRecord
			{
				PropertyName = "LINESTYLETRANSPARENCY",
				PropertyLabelName = "Line Transparency (%)",
				ControlType = DrawPropertyMenuRecord.PropertyControlTypes.Textbox,
				Hide = false,
				SectionName = DrawPropertyMenuRecord.SectionNameSectionLine
			};
			commonPropertyList.Add(propertyMenuRecord);

			propertyMenuRecord = new DrawPropertyMenuRecord
			{
				PropertyName = "LINEFROMARROW",
				PropertyLabelName = "Begin Arrow",
				ControlType = DrawPropertyMenuRecord.PropertyControlTypes.FillPattern,
				Hide = false,
				SectionName = DrawPropertyMenuRecord.SectionNameSectionLine
			};
			commonPropertyList.Add(propertyMenuRecord);

			propertyMenuRecord = new DrawPropertyMenuRecord
			{
				PropertyName = "LINEFROMARROWPALETTE",
				PropertyLabelName = "Choose Arrow",
				ControlType = DrawPropertyMenuRecord.PropertyControlTypes.FillPatternPalette,
				ControlSubType = DrawPropertyMenuRecord.PropertyControlSubTypes.LineArrowType,
				Hide = true,
				SectionName = DrawPropertyMenuRecord.SectionNameSectionLine
			};
			this.GetLineArrowNumbers(propertyMenuRecord);
			commonPropertyList.Add(propertyMenuRecord);

			propertyMenuRecord = new DrawPropertyMenuRecord
			{
				PropertyName = "LINETOARROW",
				PropertyLabelName = "End Arrow",
				ControlType = DrawPropertyMenuRecord.PropertyControlTypes.FillPattern,
				Hide = false,
				SectionName = DrawPropertyMenuRecord.SectionNameSectionLine
			};
			commonPropertyList.Add(propertyMenuRecord);

			propertyMenuRecord = new DrawPropertyMenuRecord
			{
				PropertyName = "LINETOARROWPALETTE",
				PropertyLabelName = "Choose Arrow",
				ControlType = DrawPropertyMenuRecord.PropertyControlTypes.FillPatternPalette,
				ControlSubType = DrawPropertyMenuRecord.PropertyControlSubTypes.LineArrowType,
				Hide = true,
				SectionName = DrawPropertyMenuRecord.SectionNameSectionLine
			};
			this.GetLineArrowNumbers(propertyMenuRecord);
			commonPropertyList.Add(propertyMenuRecord);

			//================================================================
			// Text section
			//================================================================
			propertyMenuRecord = new DrawPropertyMenuRecord
			{
				PropertyName = "TEXTPROPDIVIDER",
				ControlType = DrawPropertyMenuRecord.PropertyControlTypes.Divider,
				Hide = false,
				SectionName = DrawPropertyMenuRecord.SectionNameSectionText,
				SectionLabelName = "Text Section"
			};
			commonPropertyList.Add(propertyMenuRecord);

			propertyMenuRecord = new DrawPropertyMenuRecord
			{
				PropertyName = "TEXTFONT",
				PropertyLabelName = "Text Font",
				ControlType = DrawPropertyMenuRecord.PropertyControlTypes.Dropdown,
				Hide = false,
				DropdownWidth = "130px",
				SectionName = DrawPropertyMenuRecord.SectionNameSectionText
			};
			this.GetTextFontDropdownList(propertyMenuRecord);
			commonPropertyList.Add(propertyMenuRecord);

			propertyMenuRecord = new DrawPropertyMenuRecord
			{
				PropertyName = "TEXTSIZE",
				PropertyLabelName = "Text Size",
				ControlType = DrawPropertyMenuRecord.PropertyControlTypes.Dropdown,
				Hide = false,
				SectionName = DrawPropertyMenuRecord.SectionNameSectionText
			};
			this.GetTextSizeDropdownList(propertyMenuRecord);
			commonPropertyList.Add(propertyMenuRecord);

			propertyMenuRecord = new DrawPropertyMenuRecord
			{
				PropertyName = "TEXTSTYLE",
				PropertyLabelName = "Text Style",
				ControlType = DrawPropertyMenuRecord.PropertyControlTypes.Dropdown,
				Hide = false,
				DropdownWidth = "80px",
				SectionName = DrawPropertyMenuRecord.SectionNameSectionText
			};
			this.GetTextStyleDropdownList(propertyMenuRecord);
			commonPropertyList.Add(propertyMenuRecord);

			propertyMenuRecord = new DrawPropertyMenuRecord
			{
				PropertyName = "TEXTUNDERLINE",
				PropertyLabelName = "Text Underline",
				ControlType = DrawPropertyMenuRecord.PropertyControlTypes.Dropdown,
				Hide = false,
				SectionName = DrawPropertyMenuRecord.SectionNameSectionText
			};
			this.GetTextUnderlineDropdownList(propertyMenuRecord);
			commonPropertyList.Add(propertyMenuRecord);

			propertyMenuRecord = new DrawPropertyMenuRecord
			{
				PropertyName = "TEXTALIGNMENT",
				PropertyLabelName = "Text Justification",
				ControlType = DrawPropertyMenuRecord.PropertyControlTypes.Dropdown,
				Hide = false,
				DropdownWidth = "60px",
				SectionName = DrawPropertyMenuRecord.SectionNameSectionText
			};
			this.GetTextAlignmentDropdownList(propertyMenuRecord);
			commonPropertyList.Add(propertyMenuRecord);

			propertyMenuRecord = new DrawPropertyMenuRecord
			{
				PropertyName = "TEXTPOSITION",
				PropertyLabelName = "Text Block Position",
				ControlType = DrawPropertyMenuRecord.PropertyControlTypes.Dropdown,
				Hide = false,
				DropdownWidth = "60px",
				SectionName = DrawPropertyMenuRecord.SectionNameSectionText
			};
			this.GetTextPositionDropdownList(propertyMenuRecord);
			commonPropertyList.Add(propertyMenuRecord);

			propertyMenuRecord = new DrawPropertyMenuRecord
			{
				PropertyName = "TEXTBLOCKALIGNMENT",
				PropertyLabelName = "Text Block Alignment",
				ControlType = DrawPropertyMenuRecord.PropertyControlTypes.Dropdown,
				Hide = false,
				DropdownWidth = "60px",
				SectionName = DrawPropertyMenuRecord.SectionNameSectionText
			};
			this.GetTextBlockAlignmentDropdownList(propertyMenuRecord);
			commonPropertyList.Add(propertyMenuRecord);

			propertyMenuRecord = new DrawPropertyMenuRecord
			{
				PropertyName = "TEXTCOLOR",
				PropertyLabelName = "Text Color",
				ControlType = DrawPropertyMenuRecord.PropertyControlTypes.FillColor,
				Hide = false,
				FillColorHex = "#ffffff",
				SectionName = DrawPropertyMenuRecord.SectionNameSectionText
			};
			commonPropertyList.Add(propertyMenuRecord);

			propertyMenuRecord = new DrawPropertyMenuRecord
			{
				PropertyName = "TEXTCOLORSPECTRUM",
				PropertyLabelName = "Text Color spectrum",
				ControlType = DrawPropertyMenuRecord.PropertyControlTypes.FillColorSpectrum,
				Hide = true,
				SectionName = DrawPropertyMenuRecord.SectionNameSectionText
			};
			commonPropertyList.Add(propertyMenuRecord);

			//================================================================
			// Control section
			//================================================================
			propertyMenuRecord = new DrawPropertyMenuRecord
			{
				PropertyName = "DEMOVALUEPERCENTDIVIDER",
				ControlType = DrawPropertyMenuRecord.PropertyControlTypes.Divider,
				Hide = false,
				SectionName = DrawPropertyMenuRecord.SectionNameSectionControl,
				SectionLabelName = "Control Section"
			};
			commonPropertyList.Add(propertyMenuRecord);

			propertyMenuRecord = new DrawPropertyMenuRecord
			{
				PropertyName = "BARTYPE",
				PropertyLabelName = "Bar Type",
				ControlType = DrawPropertyMenuRecord.PropertyControlTypes.Dropdown,
				DropdownWidth = "80px",
				Hide = true,
				SectionName = DrawPropertyMenuRecord.SectionNameSectionControl
			};
			this.GetBarTypeDropdownList(propertyMenuRecord);
			commonPropertyList.Add(propertyMenuRecord);

			propertyMenuRecord = new DrawPropertyMenuRecord
			{
				PropertyName = "DEMOVALUEPERCENT",
				PropertyLabelName = "Demo Percent",
				ControlType = DrawPropertyMenuRecord.PropertyControlTypes.Dropdown,
				Hide = true,
				SectionName = DrawPropertyMenuRecord.SectionNameSectionControl
			};
			this.GetDemoValuePercentDropdownList(propertyMenuRecord);
			commonPropertyList.Add(propertyMenuRecord);

			propertyMenuRecord = new DrawPropertyMenuRecord
			{
				PropertyName = "POINTANDTAGID",
				PropertyLabelName = "Point and Value ID",
					AlternateLabelName = "Point Template and Value ID",
				ControlType = DrawPropertyMenuRecord.PropertyControlTypes.Textbox,
				//Readonly = true,
				TextboxWidth = "130px",
				Hide = true,
				SectionName = DrawPropertyMenuRecord.SectionNameSectionControl
			};
			commonPropertyList.Add(propertyMenuRecord);

			propertyMenuRecord = new DrawPropertyMenuRecord
			{
				PropertyName = "POINTID",
				PropertyLabelName = "Point ID",
				ControlType = DrawPropertyMenuRecord.PropertyControlTypes.Textbox,
				//Readonly = true,
				TextboxWidth = "130px",
				Hide = true,
				SectionName = DrawPropertyMenuRecord.SectionNameSectionControl
			};
			commonPropertyList.Add(propertyMenuRecord);
				
			//===========================================================
			// Button section
			//===========================================================
			var propertyMenuRecordActionButton = new DrawPropertyMenuRecord
				{
					PropertyName = "BUTTONACTIONTYPE",
					PropertyLabelName = "Button Action Type",
					DropdownWidth = "130px",
					ControlType = DrawPropertyMenuRecord.PropertyControlTypes.Dropdown,
					Hide = true,
				SectionName = DrawPropertyMenuRecord.SectionNameSectionControl
			};
				this.GetButtonActionDropdownList(propertyMenuRecordActionButton);
				commonPropertyList.Add(propertyMenuRecordActionButton);

				propertyMenuRecord = new DrawPropertyMenuRecord
				{
					PropertyName = "BUTTONACTIONTARGET",
					PropertyLabelName = "Button Action Target",
					ControlType = DrawPropertyMenuRecord.PropertyControlTypes.TextboxWithButton,
					Hide = true,
					TextboxWidth = "130px",
					ButtonActionValue = "FMDrawPropertyMenu.InvokeButtonActionConfiguration('" + propertyMenuRecordActionButton.DropdownId + "')",
				SectionName = DrawPropertyMenuRecord.SectionNameSectionControl
			};
				commonPropertyList.Add(propertyMenuRecord);

			propertyMenuRecord = new DrawPropertyMenuRecord
			{
				PropertyName = "USETAGLIMITS",
				PropertyLabelName = "Use Value Limits",
				ControlType = DrawPropertyMenuRecord.PropertyControlTypes.Dropdown,
				Hide = true,
				SectionName = DrawPropertyMenuRecord.SectionNameSectionControl
			};
			var dropdownItem = new DrawPropertyMenuDropdown { Text = "true", ValueAttribute = "true", DataValueAttribute = "true" };
			propertyMenuRecord.DropdownAdd(dropdownItem);
			dropdownItem = new DrawPropertyMenuDropdown { Text = "false", ValueAttribute = "false", DataValueAttribute = "false" };
			propertyMenuRecord.DropdownAdd(dropdownItem);
			commonPropertyList.Add(propertyMenuRecord);

			propertyMenuRecord = new DrawPropertyMenuRecord
			{
				PropertyName = "MINVALUE",
				PropertyLabelName = "Minimum Value",
				ControlType = DrawPropertyMenuRecord.PropertyControlTypes.Textbox,
				Hide = true,
				SectionName = DrawPropertyMenuRecord.SectionNameSectionControl
			};
			commonPropertyList.Add(propertyMenuRecord);

			propertyMenuRecord = new DrawPropertyMenuRecord
			{
				PropertyName = "MAXVALUE",
				PropertyLabelName = "Maximum Value",
				ControlType = DrawPropertyMenuRecord.PropertyControlTypes.Textbox,
				Hide = true,
				SectionName = DrawPropertyMenuRecord.SectionNameSectionControl
			};
			commonPropertyList.Add(propertyMenuRecord);

			propertyMenuRecord = new DrawPropertyMenuRecord
			{
				PropertyName = "USEPRODUCTCOLOR",
				PropertyLabelName = "Use Product Color",
				ControlType = DrawPropertyMenuRecord.PropertyControlTypes.Dropdown,
				Hide = false,
				DropdownWidth = "60px",
				SectionName = DrawPropertyMenuRecord.SectionNameSectionControl
			};
			this.GetTrueFalseDropDownList(propertyMenuRecord, false);
			commonPropertyList.Add(propertyMenuRecord);

			propertyMenuRecord = new DrawPropertyMenuRecord
			{
				PropertyName = "USEALARMLEVEL",
				PropertyLabelName = "Use Alarm Level",
				ControlType = DrawPropertyMenuRecord.PropertyControlTypes.Dropdown,
				Hide = false,
				DropdownWidth = "60px",
				SectionName = DrawPropertyMenuRecord.SectionNameSectionControl
			};
			this.GetTrueFalseDropDownList(propertyMenuRecord, false);
			commonPropertyList.Add(propertyMenuRecord);

			propertyMenuRecord = new DrawPropertyMenuRecord
			{
				PropertyName = "TAGUNITS",
				PropertyLabelName = "Value Units",
				ControlType = DrawPropertyMenuRecord.PropertyControlTypes.Dropdown,
				Hide = true,
				DropdownWidth = "100px",
				SectionName = DrawPropertyMenuRecord.SectionNameSectionControl
			};
			this.GetEngineeringUnitDropdownList(propertyMenuRecord);
			commonPropertyList.Add(propertyMenuRecord);

			propertyMenuRecord = new DrawPropertyMenuRecord
			{
				PropertyName = "TAGWIDTH",
				PropertyLabelName = "Value Width",
				ControlType = DrawPropertyMenuRecord.PropertyControlTypes.Textbox,
				Hide = true,
				SectionName = DrawPropertyMenuRecord.SectionNameSectionControl
			};
			commonPropertyList.Add(propertyMenuRecord);

			propertyMenuRecord = new DrawPropertyMenuRecord
			{
				PropertyName = "TAGPRECISION",
				PropertyLabelName = "Value Precision",
				ControlType = DrawPropertyMenuRecord.PropertyControlTypes.Textbox,
				Hide = true,
				SectionName = DrawPropertyMenuRecord.SectionNameSectionControl
			};
			commonPropertyList.Add(propertyMenuRecord);

			propertyMenuRecord = new DrawPropertyMenuRecord
			{
				PropertyName = "TAGFIELD",
				PropertyLabelName = "Value Field",
				ControlType = DrawPropertyMenuRecord.PropertyControlTypes.Dropdown,
				Hide = true,
				DropdownWidth = "100px",
				SectionName = DrawPropertyMenuRecord.SectionNameSectionControl
			};
			this.GetTagFieldDropdownList(propertyMenuRecord);
			commonPropertyList.Add(propertyMenuRecord);

			propertyMenuRecord = new DrawPropertyMenuRecord
			{
				PropertyName = "TAGALARMANNUNCIATION",
				PropertyLabelName = "Annunciate Alarm",
				ControlType = DrawPropertyMenuRecord.PropertyControlTypes.Dropdown,
				Hide = true,
				DropdownWidth = "60px",
				SectionName = DrawPropertyMenuRecord.SectionNameSectionControl
			};
			this.GetTrueFalseDropDownList(propertyMenuRecord, false);
			commonPropertyList.Add(propertyMenuRecord);

			propertyMenuRecord = new DrawPropertyMenuRecord
			{
				PropertyName = "TAGSHOWSTATUS",
				PropertyLabelName = "Show Quality",
				ControlType = DrawPropertyMenuRecord.PropertyControlTypes.Dropdown,
				Hide = true,
				DropdownWidth = "60px",
				SectionName = DrawPropertyMenuRecord.SectionNameSectionControl
			};
			this.GetTrueFalseDropDownList(propertyMenuRecord, true);
			commonPropertyList.Add(propertyMenuRecord);

			propertyMenuRecord = new DrawPropertyMenuRecord
			{
				PropertyName = "TAGSHOWWEIGHTSANDMEASURES",
				PropertyLabelName = "Show W&M",
				ControlType = DrawPropertyMenuRecord.PropertyControlTypes.Dropdown,
				Hide = true,
				DropdownWidth = "60px",
				SectionName = DrawPropertyMenuRecord.SectionNameSectionControl
			};
			this.GetTrueFalseDropDownList(propertyMenuRecord, true);
			commonPropertyList.Add(propertyMenuRecord);

			propertyMenuRecord = new DrawPropertyMenuRecord
			{
				PropertyName = "ANIMATIONBUTTON",
				PropertyLabelName = "Animation Create/Edit",
				ControlType = DrawPropertyMenuRecord.PropertyControlTypes.Textbox,
				//Readonly = true,
				TextboxWidth = "130px",
				Hide = true,
				SectionName = DrawPropertyMenuRecord.SectionNameSectionControl
			};
			commonPropertyList.Add(propertyMenuRecord);

			// Add the common properties to the model.
			drawModel.CommonPropertyList = commonPropertyList;
		}

		/// <summary>
		/// This method will create the rectangle properties that is used to build the
		/// property menu.
		/// </summary>
		/// <param name="drawModel">The draw model</param>
		private void CreateRectangleProperties(DrawModel drawModel)
		{
			List<DrawPropertyMenuRecord> rectanglePropertyList = new List<DrawPropertyMenuRecord>();

			//var propertyMenuRecord = new DrawPropertyMenuRecord
			//						{
			//							PropertyName		= "WIDTH",
			//							PropertyLabelName	= "Width",
			//							ControlType			= DrawPropertyMenuRecord.PropertyControlTypes.Textbox,
			//							Hide				= false
			//						};
			//rectanglePropertyList.Add(propertyMenuRecord);

			//propertyMenuRecord = new DrawPropertyMenuRecord
			//						{
			//							PropertyName		= "HEIGHT",
			//							PropertyLabelName	= "Height",
			//							ControlType			= DrawPropertyMenuRecord.PropertyControlTypes.Textbox,
			//							Hide				= false
			//						};
			//rectanglePropertyList.Add(propertyMenuRecord);

			// Add the rectangle properties to the model.
			drawModel.RectanglePropertyList = rectanglePropertyList;
		}

		/// <summary>
		/// This method will load the items for the line size dropdown.
		/// </summary>
		/// <param name="propertyMenuRecord"></param>
		private void GetLineSizeDropdownList(DrawPropertyMenuRecord propertyMenuRecord)
		{
			var dropdownItem = new DrawPropertyMenuDropdown { Text = "None", ValueAttribute = "-1", DataValueAttribute = "NONE", Selected = true };
			propertyMenuRecord.DropdownAdd(dropdownItem);

			for (int nextPoint = 1; nextPoint < 25; nextPoint++)
			{
				string textStr = nextPoint + "pt";
				dropdownItem = new DrawPropertyMenuDropdown { Text = textStr, ValueAttribute = nextPoint.ToString(), DataValueAttribute = nextPoint.ToString() };
				propertyMenuRecord.DropdownAdd(dropdownItem);
			}
		}

		/// <summary>
		/// This method will load the items for the text size dropdown.
		/// </summary>
		private void GetTextSizeDropdownList(DrawPropertyMenuRecord propertyMenuRecord)
		{
			var dropdownItem = new DrawPropertyMenuDropdown { Text = "None", ValueAttribute = "16", DataValueAttribute = "NONE", Selected = true };
			propertyMenuRecord.DropdownAdd(dropdownItem);

			dropdownItem = new DrawPropertyMenuDropdown { Text = "6pt", ValueAttribute = "1", DataValueAttribute = "6" };
			propertyMenuRecord.DropdownAdd(dropdownItem);

			dropdownItem = new DrawPropertyMenuDropdown { Text = "8pt", ValueAttribute = "2", DataValueAttribute = "8" };
			propertyMenuRecord.DropdownAdd(dropdownItem);

			dropdownItem = new DrawPropertyMenuDropdown { Text = "9pt", ValueAttribute = "3", DataValueAttribute = "9" };
			propertyMenuRecord.DropdownAdd(dropdownItem);

			dropdownItem = new DrawPropertyMenuDropdown { Text = "10pt", ValueAttribute = "4", DataValueAttribute = "10" };
			propertyMenuRecord.DropdownAdd(dropdownItem);

			dropdownItem = new DrawPropertyMenuDropdown { Text = "11pt", ValueAttribute = "5", DataValueAttribute = "11" };
			propertyMenuRecord.DropdownAdd(dropdownItem);

			dropdownItem = new DrawPropertyMenuDropdown { Text = "12pt", ValueAttribute = "6", DataValueAttribute = "12" };
			propertyMenuRecord.DropdownAdd(dropdownItem);

			dropdownItem = new DrawPropertyMenuDropdown { Text = "13pt", ValueAttribute = "7", DataValueAttribute = "13" };
			propertyMenuRecord.DropdownAdd(dropdownItem);

			dropdownItem = new DrawPropertyMenuDropdown { Text = "14pt", ValueAttribute = "8", DataValueAttribute = "14" };
			propertyMenuRecord.DropdownAdd(dropdownItem);

			dropdownItem = new DrawPropertyMenuDropdown { Text = "16pt", ValueAttribute = "9", DataValueAttribute = "16" };
			propertyMenuRecord.DropdownAdd(dropdownItem);

			dropdownItem = new DrawPropertyMenuDropdown { Text = "18pt", ValueAttribute = "10", DataValueAttribute = "18" };
			propertyMenuRecord.DropdownAdd(dropdownItem);

			dropdownItem = new DrawPropertyMenuDropdown { Text = "24pt", ValueAttribute = "11", DataValueAttribute = "24" };
			propertyMenuRecord.DropdownAdd(dropdownItem);

			dropdownItem = new DrawPropertyMenuDropdown { Text = "30pt", ValueAttribute = "12", DataValueAttribute = "30" };
			propertyMenuRecord.DropdownAdd(dropdownItem);

			dropdownItem = new DrawPropertyMenuDropdown { Text = "36pt", ValueAttribute = "13", DataValueAttribute = "36" };
			propertyMenuRecord.DropdownAdd(dropdownItem);

			dropdownItem = new DrawPropertyMenuDropdown { Text = "48pt", ValueAttribute = "14", DataValueAttribute = "48" };
			propertyMenuRecord.DropdownAdd(dropdownItem);

			dropdownItem = new DrawPropertyMenuDropdown { Text = "60pt", ValueAttribute = "15", DataValueAttribute = "60" };
			propertyMenuRecord.DropdownAdd(dropdownItem);
		}

		/// <summary>
		/// This method will load the items for the text style dropdown.
		/// </summary>
		private void GetTextStyleDropdownList(DrawPropertyMenuRecord propertyMenuRecord)
		{
			var dropdownItem = new DrawPropertyMenuDropdown { Text = "None", ValueAttribute = "5", DataValueAttribute = "NONE", Selected = true };
			propertyMenuRecord.DropdownAdd(dropdownItem);

			dropdownItem = new DrawPropertyMenuDropdown { Text = "Regular", ValueAttribute = "1", DataValueAttribute = "Regular" };
			propertyMenuRecord.DropdownAdd(dropdownItem);

			dropdownItem = new DrawPropertyMenuDropdown { Text = "Bold", ValueAttribute = "2", DataValueAttribute = "Bold" };
			propertyMenuRecord.DropdownAdd(dropdownItem);

			dropdownItem = new DrawPropertyMenuDropdown { Text = "Italic", ValueAttribute = "3", DataValueAttribute = "Italic" };
			propertyMenuRecord.DropdownAdd(dropdownItem);

			dropdownItem = new DrawPropertyMenuDropdown { Text = "Bold Italic", ValueAttribute = "4", DataValueAttribute = "BoldItalic" };
			propertyMenuRecord.DropdownAdd(dropdownItem);
		}

		/// <summary>
		/// This method will load the items for the text underline dropdown.
		/// </summary>
		private void GetTextUnderlineDropdownList(DrawPropertyMenuRecord propertyMenuRecord)
		{
			var dropdownItem = new DrawPropertyMenuDropdown { Text = "None", ValueAttribute = "3", DataValueAttribute = "NONE", Selected = true };
			propertyMenuRecord.DropdownAdd(dropdownItem);

			dropdownItem = new DrawPropertyMenuDropdown { Text = "True", ValueAttribute = "1", DataValueAttribute = "TRUE" };
			propertyMenuRecord.DropdownAdd(dropdownItem);

			dropdownItem = new DrawPropertyMenuDropdown { Text = "False", ValueAttribute = "2", DataValueAttribute = "FALSE" };
			propertyMenuRecord.DropdownAdd(dropdownItem);
		}

		/// <summary>
		/// This method will populate the dropdown with true and false.  The Defaulting setting
		/// will select either the True or False option.
		/// </summary>
		/// <param name="propertyMenuRecord">The property menu record object.</param>
		/// <param name="defaultSetting">Default setting to select either the True or False option.</param>
		private void GetTrueFalseDropDownList(DrawPropertyMenuRecord propertyMenuRecord, bool defaultSetting)
		{
			DrawPropertyMenuDropdown dropdownItem;

			if (defaultSetting)
			{
				dropdownItem = new DrawPropertyMenuDropdown { Text = "False", ValueAttribute = "false", DataValueAttribute = "false" };
				propertyMenuRecord.DropdownAdd(dropdownItem);

				dropdownItem = new DrawPropertyMenuDropdown { Text = "True", ValueAttribute = "true", DataValueAttribute = "true", Selected = true };
				propertyMenuRecord.DropdownAdd(dropdownItem);
			}
			else
			{
				dropdownItem = new DrawPropertyMenuDropdown { Text = "False", ValueAttribute = "false", DataValueAttribute = "false", Selected = true };
				propertyMenuRecord.DropdownAdd(dropdownItem);

				dropdownItem = new DrawPropertyMenuDropdown { Text = "True", ValueAttribute = "true", DataValueAttribute = "true" };
				propertyMenuRecord.DropdownAdd(dropdownItem);
			}		
		}

		

		/// <summary>
		/// This method will load the items for the text alignment dropdown.
		/// </summary>
		private void GetTextAlignmentDropdownList(DrawPropertyMenuRecord propertyMenuRecord)
		{
			var dropdownItem = new DrawPropertyMenuDropdown { Text = "None", ValueAttribute = "4", DataValueAttribute = "NONE", Selected = true };
			propertyMenuRecord.DropdownAdd(dropdownItem);

			dropdownItem = new DrawPropertyMenuDropdown { Text = "Left", ValueAttribute = "1", DataValueAttribute = "start" };
			propertyMenuRecord.DropdownAdd(dropdownItem);

			dropdownItem = new DrawPropertyMenuDropdown { Text = "Center", ValueAttribute = "2", DataValueAttribute = "center" };
			propertyMenuRecord.DropdownAdd(dropdownItem);

			dropdownItem = new DrawPropertyMenuDropdown { Text = "Right", ValueAttribute = "3", DataValueAttribute = "end" };
			propertyMenuRecord.DropdownAdd(dropdownItem);
		}

		/// <summary>
		/// This method will load the items for the text position dropdown.
		/// </summary>
		private void GetTextPositionDropdownList(DrawPropertyMenuRecord propertyMenuRecord)
		{
			var dropdownItem = new DrawPropertyMenuDropdown { Text = "None", ValueAttribute = "4", DataValueAttribute = "NONE" };
			propertyMenuRecord.DropdownAdd(dropdownItem);

			dropdownItem = new DrawPropertyMenuDropdown { Text = "Top", ValueAttribute = "1", DataValueAttribute = "TOP" };
			propertyMenuRecord.DropdownAdd(dropdownItem);

			dropdownItem = new DrawPropertyMenuDropdown { Text = "Middle", ValueAttribute = "2", DataValueAttribute = "CENTER" };
			propertyMenuRecord.DropdownAdd(dropdownItem);

			dropdownItem = new DrawPropertyMenuDropdown { Text = "Bottom", ValueAttribute = "3", DataValueAttribute = "BOTTOM", Selected = true };
			propertyMenuRecord.DropdownAdd(dropdownItem);
		}

		private void UnitsHelperFunction(List<TagUnitToUnitType> tagUnitToUnitTypeList, EngineeringUnitType type)
		{
			var unitList = EngineeringUnits.GetUnitsByType(type);
			foreach (var unit in unitList)
			{
				tagUnitToUnitTypeList.Add(new TagUnitToUnitType
				{
					Unit = (int)unit,
					UnitStr = unit.ToString(),
					UnitType = (int)type,
					UnitTypeStr = type.ToString(),
					UnitDescription = EngineeringUnits.GetUnitString(unit),
					UnitAbbreviation = EngineeringUnits.GetUnitAbbreviation(unit)
				});
			}
		}

		/// <summary>
		/// This method will load the items for the Engineering Unit dropdown.
		/// </summary>
		private void PopulateTagUnitToUnitTypeList(DrawModel model)
		{
			EngineeringUnit unit;
			model.TagUnitToUnitTypeList = new List<TagUnitToUnitType>();
			for (EngineeringUnitType unitType = EngineeringUnitType.FmuAll + 1; unitType < EngineeringUnitType.FmuNone; unitType++)
			{
				if (Enum.IsDefined(typeof(EngineeringUnitType), unitType))
				{
					if (unitType == EngineeringUnitType.FmuNodim)
					{
						unit = EngineeringUnit.FmuNone;
						model.TagUnitToUnitTypeList.Add(new TagUnitToUnitType
						{
							Unit = (int)unit,
							UnitStr = unit.ToString(),
							UnitType = (int)unitType,
							UnitTypeStr = unitType.ToString(),
							UnitDescription = "None",
							UnitAbbreviation = "None"
						});
					}
					else
					{
						this.UnitsHelperFunction(model.TagUnitToUnitTypeList, unitType);
					}
				}
			}
			unit = EngineeringUnit.FmSiteUnits;
			model.TagUnitToUnitTypeList.Add(new TagUnitToUnitType
			{
				Unit = (int)unit,
				UnitStr = unit.ToString(),
				UnitType = -1,
				UnitTypeStr = "PointUnits",
				UnitDescription = "{Value Units}",
				UnitAbbreviation = "{Value Units}"
			});
		}
		/// <summary>
		/// This method will load the items for the Engineering Unit dropdown.
		/// </summary>
		private void GetEngineeringUnitDropdownList(DrawPropertyMenuRecord propertyMenuRecord)
		{
			for (EngineeringUnit unit = EngineeringUnit.FmSiteUnits; unit <= EngineeringUnit.FmuNone; unit++)
			{
				if (Enum.IsDefined(typeof(EngineeringUnit), unit))
				{
					int unitInt = (int)unit;
					var dropdownItem = new DrawPropertyMenuDropdown { Text = unit.ToString(), ValueAttribute = unitInt.ToString(), DataValueAttribute = unit.ToString(), Selected = (unit == EngineeringUnit.FmuNone) };
					propertyMenuRecord.DropdownAdd(dropdownItem);
				}
			}
		}

		/// <summary>
		/// This method will load the items for the text block alignment dropdown.
		/// </summary>
		private void GetTextBlockAlignmentDropdownList(DrawPropertyMenuRecord propertyMenuRecord)
		{
			var dropdownItem = new DrawPropertyMenuDropdown { Text = "None", ValueAttribute = "4", DataValueAttribute = "NONE", Selected = true };
			propertyMenuRecord.DropdownAdd(dropdownItem);

			dropdownItem = new DrawPropertyMenuDropdown { Text = "Left", ValueAttribute = "1", DataValueAttribute = "Left" };
			propertyMenuRecord.DropdownAdd(dropdownItem);

			dropdownItem = new DrawPropertyMenuDropdown { Text = "Center", ValueAttribute = "2", DataValueAttribute = "Center" };
			propertyMenuRecord.DropdownAdd(dropdownItem);

			dropdownItem = new DrawPropertyMenuDropdown { Text = "Right", ValueAttribute = "3", DataValueAttribute = "Right" };
			propertyMenuRecord.DropdownAdd(dropdownItem);
		}

		/// <summary>
		/// This method will load the items for the text position dropdown.
		/// </summary>
		private void GetTextFontDropdownList(DrawPropertyMenuRecord propertyMenuRecord)
		{
			int valueCount = 1;
			DrawPropertyMenuDropdown dropdownItem;

			foreach (FontFamily font in FontFamily.Families)
			{
				dropdownItem = new DrawPropertyMenuDropdown { Text = font.Name, ValueAttribute = valueCount.ToString(), DataValueAttribute = font.Name };
				propertyMenuRecord.DropdownAdd(dropdownItem);

				if (font.Name.Equals("Arial"))
				{
					dropdownItem.Selected = true;
				}

				valueCount++;
			}

			// Add sans-serif since it is not present.
			dropdownItem = new DrawPropertyMenuDropdown { Text = "sans-serif", ValueAttribute = valueCount.ToString(), DataValueAttribute = "sans-serif" };
			valueCount++;
			propertyMenuRecord.DropdownAdd(dropdownItem);
			propertyMenuRecord.Sort();

			dropdownItem = new DrawPropertyMenuDropdown { Text = "None", ValueAttribute = valueCount.ToString(), DataValueAttribute = "NONE" };
			propertyMenuRecord.DropdownInsertAt(dropdownItem, 0);
		}

		/// <summary>
		/// This method will load the items for the bar type percent dropdown.
		/// </summary>
		private void GetBarTypeDropdownList(DrawPropertyMenuRecord propertyMenuRecord)
		{
			var dropdownItem = new DrawPropertyMenuDropdown { Text = "Standard", ValueAttribute = "Standard", DataValueAttribute = "Standard", Selected = true };
			propertyMenuRecord.DropdownAdd(dropdownItem);

			dropdownItem = new DrawPropertyMenuDropdown { Text = "Deviation", ValueAttribute = "Deviation", DataValueAttribute = "Deviation" };
			propertyMenuRecord.DropdownAdd(dropdownItem);
		}

		/// <summary>
		/// This method will load the items for the Demo Value percent dropdown.
		/// </summary>
		private void GetDemoValuePercentDropdownList(DrawPropertyMenuRecord propertyMenuRecord)
		{
			var dropdownItem = new DrawPropertyMenuDropdown { Text = "100%", ValueAttribute = "100", DataValueAttribute = "100", Selected = true };
			propertyMenuRecord.DropdownAdd(dropdownItem);

			dropdownItem = new DrawPropertyMenuDropdown { Text = "90%", ValueAttribute = "90", DataValueAttribute = "90" };
			propertyMenuRecord.DropdownAdd(dropdownItem);

			dropdownItem = new DrawPropertyMenuDropdown { Text = "75%", ValueAttribute = "75", DataValueAttribute = "75" };
			propertyMenuRecord.DropdownAdd(dropdownItem);

			dropdownItem = new DrawPropertyMenuDropdown { Text = "50%", ValueAttribute = "50", DataValueAttribute = "50" };
			propertyMenuRecord.DropdownAdd(dropdownItem);

			dropdownItem = new DrawPropertyMenuDropdown { Text = "25%", ValueAttribute = "25", DataValueAttribute = "25" };
			propertyMenuRecord.DropdownAdd(dropdownItem);

			dropdownItem = new DrawPropertyMenuDropdown { Text = "10%", ValueAttribute = "10", DataValueAttribute = "10" };
			propertyMenuRecord.DropdownAdd(dropdownItem);

			dropdownItem = new DrawPropertyMenuDropdown { Text = "0%", ValueAttribute = "0", DataValueAttribute = "0" };
			propertyMenuRecord.DropdownAdd(dropdownItem);
		}

		/// <summary>
		/// This method will load the items for the Demo Value percent dropdown.
		/// </summary>
		private void GetTagFieldDropdownList(DrawPropertyMenuRecord propertyMenuRecord)
		{
			var dropdownItem = new DrawPropertyMenuDropdown { Text = "VALUE", ValueAttribute = "0", DataValueAttribute = "VALUE", Selected = true };
			propertyMenuRecord.DropdownAdd(dropdownItem);

			dropdownItem = new DrawPropertyMenuDropdown { Text = "ID", ValueAttribute = "1", DataValueAttribute = "ID" };
			propertyMenuRecord.DropdownAdd(dropdownItem);

			dropdownItem = new DrawPropertyMenuDropdown { Text = "TIMESTAMP", ValueAttribute = "2", DataValueAttribute = "TIMESTAMP" };
			propertyMenuRecord.DropdownAdd(dropdownItem);

			dropdownItem = new DrawPropertyMenuDropdown { Text = "UNITS", ValueAttribute = "3", DataValueAttribute = "UNITS" };
			propertyMenuRecord.DropdownAdd(dropdownItem);

			dropdownItem = new DrawPropertyMenuDropdown { Text = "ALARM STATUS", ValueAttribute = "4", DataValueAttribute = "ALARM STATUS" };
			propertyMenuRecord.DropdownAdd(dropdownItem);
		}

		/// <summary>
		/// This method will populate the pattern numbers.
		/// </summary>
		/// <param name="propertyMenuRecord">The property menu record.</param>
		private void GetFillPatternNumbers(DrawPropertyMenuRecord propertyMenuRecord)
		{
			var patternList = new List<DrawPropertyMenuPattern>();

			for (int nextPattern = 1; nextPattern <= 7; nextPattern++)
			{
				var pattern = new DrawPropertyMenuPattern
				{
					CanvasTagId = DrawPropertyMenuPattern.CanvasPalettePrefixId + propertyMenuRecord.PropertyName + "-" + nextPattern,
					PatternNumber = nextPattern
				};
				patternList.Add(pattern);
			}

			propertyMenuRecord.PatternList = patternList;
		}

		/// <summary>
		/// This method will populate the line style numbers.
		/// </summary>
		/// <param name="propertyMenuRecord">The property menu record.</param>
		private void GetLineStyleNumbers(DrawPropertyMenuRecord propertyMenuRecord)
		{
			var patternList = new List<DrawPropertyMenuPattern>();

			for (int nextPattern = 1; nextPattern <= 5; nextPattern++)
			{
				var pattern = new DrawPropertyMenuPattern
				{
					CanvasTagId = DrawPropertyMenuPattern.CanvasPalettePrefixId + propertyMenuRecord.PropertyName + "-" + nextPattern,
					PatternNumber = nextPattern
				};
				patternList.Add(pattern);
			}

			propertyMenuRecord.PatternList = patternList;
		}

		/// <summary>
		/// This method will populate the line arrow numbers.
		/// </summary>
		/// <param name="propertyMenuRecord">The property menu record.</param>
		private void GetLineArrowNumbers(DrawPropertyMenuRecord propertyMenuRecord)
		{
			var patternList = new List<DrawPropertyMenuPattern>();

			for (int nextPattern = 1; nextPattern <= 5; nextPattern++)
			{
				var pattern = new DrawPropertyMenuPattern
				{
					CanvasTagId = DrawPropertyMenuPattern.CanvasPalettePrefixId + propertyMenuRecord.PropertyName + "-" + nextPattern,
					PatternNumber = nextPattern
				};
				patternList.Add(pattern);
			}

			propertyMenuRecord.PatternList = patternList;
		}

		/// <summary>
		/// This method will load the items for the Demo Value percent dropdown.
		/// </summary>
		private void GetButtonActionDropdownList(DrawPropertyMenuRecord propertyMenuRecord)
		{
			var dropdownItem = new DrawPropertyMenuDropdown { Text = "None", ValueAttribute = "BUTTON_ACTION_NONE", DataValueAttribute = "BUTTON_ACTION_NONE", Selected = true };
			propertyMenuRecord.DropdownAdd(dropdownItem);

			dropdownItem = new DrawPropertyMenuDropdown { Text = "Command", ValueAttribute = "BUTTON_ACTION_COMMAND", DataValueAttribute = "BUTTON_ACTION_COMMAND" };
			propertyMenuRecord.DropdownAdd(dropdownItem);

			dropdownItem = new DrawPropertyMenuDropdown { Text = "Graphic", ValueAttribute = "BUTTON_ACTION_GRAPHIC", DataValueAttribute = "BUTTON_ACTION_GRAPHIC" };
			propertyMenuRecord.DropdownAdd(dropdownItem);

			dropdownItem = new DrawPropertyMenuDropdown { Text = "Point Trend", ValueAttribute = "BUTTON_POINT_TREND", DataValueAttribute = "BUTTON_POINT_TREND" };
			propertyMenuRecord.DropdownAdd(dropdownItem);

			dropdownItem = new DrawPropertyMenuDropdown { Text = "Point Detail", ValueAttribute = "BUTTON_ACTION_DETAIL", DataValueAttribute = "BUTTON_ACTION_DETAIL" };
			propertyMenuRecord.DropdownAdd(dropdownItem);

            dropdownItem = new DrawPropertyMenuDropdown { Text = "Point History", ValueAttribute = "BUTTON_ACTION_POINT_HISTORY", DataValueAttribute = "BUTTON_ACTION_POINT_HISTORY" };
            propertyMenuRecord.DropdownAdd(dropdownItem);
            
			/*dropdownItem = new DrawPropertyMenuDropdown { Text = "Linked Graphic", ValueAttribute = "BUTTON_ACTION_LINKED_GRAPHIC", DataValueAttribute = "BUTTON_ACTION_LINKED_GRAPHIC" };
			propertyMenuRecord.DropdownAdd(dropdownItem);

			dropdownItem = new DrawPropertyMenuDropdown { Text = "Report", ValueAttribute = "BUTTON_ACTION_REPORT", DataValueAttribute = "BUTTON_ACTION_REPORT" };
			propertyMenuRecord.DropdownAdd(dropdownItem);

			dropdownItem = new DrawPropertyMenuDropdown { Text = "Help", ValueAttribute = "BUTTON_ACTION_HELP", DataValueAttribute = "BUTTON_ACTION_HELP" };
			propertyMenuRecord.DropdownAdd(dropdownItem);

			dropdownItem = new DrawPropertyMenuDropdown { Text = "Template", ValueAttribute = "BUTTON_ACTION_TEMPLATE", DataValueAttribute = "BUTTON_ACTION_TEMPLATE" };
			propertyMenuRecord.DropdownAdd(dropdownItem);

			dropdownItem = new DrawPropertyMenuDropdown { Text = "Linked Template", ValueAttribute = "BUTTON_ACTION_LINKEDTEMPLATE", DataValueAttribute = "BUTTON_ACTION_LINKEDTEMPLATE" };
			propertyMenuRecord.DropdownAdd(dropdownItem);

			dropdownItem = new DrawPropertyMenuDropdown { Text = "URL Link", ValueAttribute = "BUTTON_ACTION_URLLINK", DataValueAttribute = "BUTTON_ACTION_URLLINK" };
			propertyMenuRecord.DropdownAdd(dropdownItem);*/
        }

        [HttpGet, ValidateJsonAntiForgeryToken]
		public ActionResult ReadDrawViewStateSettings()
		{
			string results; // this is where we store the data to return which in this case is the left,top position of the toolbar

			results = "-1;1";
			var userSettings = FMChannelHelper.MakeCall<IUserViewStateSettings, UserViewStateSettingCollection>(x => x.EnumerateBySiteUserClientIpAddressWindowNameAndViewID(this.Security, this.Security.SiteGuid, this.Security.UserGuid, "", DrawViewerID));
			if (userSettings != null && userSettings.Count > 0)
			{
				if (userSettings.Count != 1)
				{
					throw new ApplicationException(String.Format("Too many user settings records for User {0} Site {1} ID {2}", this.Security.SiteGuid, this.Security.UserGuid, DrawViewerID));
				}
				var userSetting = userSettings[0];
				var drawuvss = (DrawUserViewStateSettings)userSetting.Value;
				results = drawuvss.DrawToolbarTopPosition.ToString() + ";" + drawuvss.DrawToolbarLeftPosition.ToString();
				int y = 0;
				++y;
			}
			return this.JsonWithErrorMessages(results, JsonRequestBehavior.AllowGet);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult SaveDrawViewStateSettings(string ToolbarLeftCoord, string ToolbarTopCoord)
		{
			bool success = true;
			double TopCoord = System.Convert.ToDouble(ToolbarTopCoord);
			// if the toolbar is somehow placed off screen, reset the position
			// -45 is the height of the top bar
		   if (TopCoord < -45) 
				TopCoord = 0;
         double LeftCoord = System.Convert.ToDouble(ToolbarLeftCoord);
			if (LeftCoord < 0) 
				LeftCoord = 0;
			var context = this.Session[DrawContext.SessionKey] as DrawContext;
			var model = new DrawModel(context);

			if (model != null)
			{
				//var tempSite = model.Site;
				var userSettings =
					FMChannelHelper.MakeCall<IUserViewStateSettings, UserViewStateSettingCollection>(x => x.EnumerateBySiteUserClientIpAddressWindowNameAndViewID(this.Security, this.Security.SiteGuid, this.Security.UserGuid, "", DrawViewerID));
				if (userSettings == null || userSettings.Count <= 0)
				{
					var userSetting = new UserViewStateSetting(this.Security);
					var tagList = new DrawUserViewStateSettings();

					tagList.DrawToolbarLeftPosition = System.Convert.ToInt32(LeftCoord);
					tagList.DrawToolbarTopPosition = System.Convert.ToInt32(TopCoord);
					
					userSetting.Value = tagList;
					userSetting.ViewID = DrawViewerID;
					FMChannelHelper.MakeCall<IUserViewStateSettings>(x => x.Add(this.Security, userSetting));
				}
				else
				{
					var userSetting = userSettings[0];
					var tagList = new DrawUserViewStateSettings();

					tagList.DrawToolbarLeftPosition = System.Convert.ToInt32(LeftCoord);
					tagList.DrawToolbarTopPosition = System.Convert.ToInt32(TopCoord);
					userSetting.Value = tagList;
					userSetting.ViewID = DrawViewerID;
					FMChannelHelper.MakeCall<IUserViewStateSettings>(x => x.Modify(this.Security, userSetting));
				}
			}
			return this.JsonWithErrorMessages(success, JsonRequestBehavior.AllowGet);
		}

		#endregion
	}
}
