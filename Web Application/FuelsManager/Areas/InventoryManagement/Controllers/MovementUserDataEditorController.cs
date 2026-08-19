namespace FuelsManager.Areas.InventoryManagement.Controllers
{
    using FMBusinessObjects.BusinessInterfaces;
    using FMBusinessObjects.ChannelFactories;
    using FMBusinessObjects.Constants;
    using FMBusinessObjects.DataObjects;
    using FuelsManager.Areas.Controllers;
    using FuelsManager.Areas.InventoryManagement.ViewModels;
    using Newtonsoft.Json;
    using Opc.Ua;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;

    public class MovementUserDataEditorController : FMBaseControllerEx
    {
        #region Data members
        const string ErrorMsgPrefix = "MovementUserDataEditorView: ";
        List<Guid> wellKnownTagGuids;
        #endregion

        #region Constructors
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        public MovementUserDataEditorController()
        {
            wellKnownTagGuids = new List<Guid>
            {
                Guids.UserData01WellKnownGuid,
                Guids.UserData02WellKnownGuid,
                Guids.UserData03WellKnownGuid,
                Guids.UserData04WellKnownGuid,
                Guids.UserData05WellKnownGuid,
                Guids.UserData06WellKnownGuid,
                Guids.UserData07WellKnownGuid,
                Guids.UserData08WellKnownGuid,
                Guids.UserData09WellKnownGuid,
                Guids.UserData10WellKnownGuid
            };
        }
        #endregion

        #region Public static methods
        /// <summary>
        /// Identifies the data dictionary keys needed for this item.
        /// </summary>
        /// <param name="model">The model to serialize</param>
        /// <returns>Returns a string of the model.</returns>
        [NonAction]
        public static string SerializeModel(MovementUserDataEditorModel model)
        {
            return JsonConvert.SerializeObject(model);
        }

		/// <summary>
		/// Identifies the data dictionary keys needed for this item.
		/// </summary>
		/// <param name="modelStr">The model to serialize</param>
		/// <returns>Returns the movement user data editor model.</returns>
		[NonAction]
		public static MovementUserDataEditorModel DeserializeModel(string modelStr)
		{
			var jsonSerializerSettings = new JsonSerializerSettings
			{
				MissingMemberHandling = MissingMemberHandling.Ignore
			};

			var obj = JsonConvert.DeserializeObject<MovementUserDataEditorModel>(modelStr, jsonSerializerSettings);
			return obj;
		}
        #endregion

        #region Public methods
        /// <summary>
        /// This method retrieves the movement user data editor model based on the movement point GUID.
        /// </summary>
        /// <param name="movementPointGuid">The movement point GUID</param>
        /// <returns>Returns the Movement User Data Editor model.</returns>
        [HttpGet]
		public ActionResult MovementUserDataEditor(Guid movementPointGuid)
		{
			try
			{
				MovementUserDataEditorModel model = this.GetMovementUserDataEditorModel(movementPointGuid);
				return base.PartialViewWithErrorMessages("MovementUserDataEditor", model, JsonRequestBehavior.AllowGet);
			}
			catch(Exception ex)
			{
				string msgBasic = this.GetTranslatedText("Error Getting Movement User Data.");
                string msgEventLog = ErrorMsgPrefix + msgBasic + " " + ex.Message;
				FMChannelHelper.MakeCall<IFMEventLog>(x => x.WriteEntry(msgEventLog, FMBusinessObjects.Constants.FMEventLogEntryType.Error));

				base.OnError(new Exception(msgBasic));
				return base.JsonWithErrorMessages(null, JsonRequestBehavior.AllowGet);
			}
		}

        /// <summary>
        /// This method handles the save call from the UI to save the movement user data.
        /// </summary>
        /// <param name="movementUserDataEditorModelStr">The model string to save.</param>
        /// <returns>Return successful or an error.</returns>
        [HttpPost]
        public ActionResult SaveMovementUserData(string movementUserDataEditorModelStr)
        {
            string msgBasic = string.Empty;
            string msgEventLog = string.Empty;

            if (string.IsNullOrEmpty(movementUserDataEditorModelStr))
            {
                msgBasic = this.GetTranslatedText("Error, movement user data editor model is empty.");
                msgEventLog = ErrorMsgPrefix + msgBasic;
                FMChannelHelper.MakeCall<IFMEventLog>(x => x.WriteEntry(msgEventLog, FMBusinessObjects.Constants.FMEventLogEntryType.Error));

                base.OnError(new Exception(msgBasic));
                return base.JsonWithErrorMessages(null, JsonRequestBehavior.AllowGet);
            }

            MovementUserDataEditorModel model = null;

            try
            {
                var jsonSerializerSettings = new JsonSerializerSettings{  MissingMemberHandling = MissingMemberHandling.Ignore };
                model = JsonConvert.DeserializeObject<MovementUserDataEditorModel>(movementUserDataEditorModelStr, jsonSerializerSettings);
            }
            catch(Exception ex)
            {
                msgBasic = this.GetTranslatedText("Error deserializing the model string.");
                msgEventLog = ErrorMsgPrefix + msgBasic + " " + ex.Message;
                FMChannelHelper.MakeCall<IFMEventLog>(x => x.WriteEntry(msgEventLog, FMBusinessObjects.Constants.FMEventLogEntryType.Error));

                base.OnError(new Exception(msgBasic));
                return base.JsonWithErrorMessages(null, JsonRequestBehavior.AllowGet);
            }

            if(model != null)
            {
                try
                {
                    this.SaveMovementUserDataHelper(model);
                    base.AddSuccess("Saved Successful");

                    return this.JsonWithErrorMessages(null, JsonRequestBehavior.AllowGet);
                }
                catch(Exception ex)
                {
                    msgBasic = this.GetTranslatedText("Error saving the movement user data.");
                    msgEventLog = ErrorMsgPrefix + msgBasic + " " + ex.Message;
                    FMChannelHelper.MakeCall<IFMEventLog>(x => x.WriteEntry(msgEventLog, FMBusinessObjects.Constants.FMEventLogEntryType.Error));

                    base.OnError(new Exception(msgBasic));
                    return base.JsonWithErrorMessages(null, JsonRequestBehavior.AllowGet);
                }
            }

            msgBasic = this.GetTranslatedText("Error movement user data model is null.");
            msgEventLog = ErrorMsgPrefix + msgBasic;
            FMChannelHelper.MakeCall<IFMEventLog>(x => x.WriteEntry(msgEventLog, FMBusinessObjects.Constants.FMEventLogEntryType.Error));

            base.OnError(new Exception(msgBasic));
            return base.JsonWithErrorMessages(null, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Private methods
        /// <summary>
        /// This method will retrieve the movement user data information based on the movement point Guid.
        /// </summary>
        /// <param name="movementPointGuid">The movement point Guid used to retrieved the data.</param>
        /// <returns>Return a movement user data editor model.</returns>
        private MovementUserDataEditorModel GetMovementUserDataEditorModel(Guid movementPointGuid)
        {
			if(movementPointGuid == null || movementPointGuid == Guid.Empty)
            {
				return new MovementUserDataEditorModel();
            }

            var movementPoint = FMChannelHelper.MakeCall<IPoints, Point>(x => x.Get(this.Security, movementPointGuid, false));

            if (movementPoint == null)
            {
                throw new Exception("No Movement Point found.");
            }

            // Retrieve the movement user data information from the service.
            var pointPropGuid = FMChannelHelper.MakeCall<IPointProperties, Guid>(x => x.GetPointPropertyGuid(this.Security, movementPointGuid, "Movement Data"));

            if(pointPropGuid == null || pointPropGuid == Guid.Empty)
            {
                throw new Exception("No Point Property found for this Movement.");
            }

            var movementPointProp = FMChannelHelper.MakeCall<IPointProperties, PointProperty>(x => x.Get(this.Security, pointPropGuid));

            if (movementPointProp == null)
            {
                throw new Exception("No Point Property found for this Movement.");
            }

            var model = new MovementUserDataEditorModel 
            { 
                MovementPointGuid = movementPointGuid, 
                PointPropertyId = movementPointProp.ID,
                PointId = movementPoint.ID
            };

            // Get the user data point values.
            List<Guid> movementPointGuidList = new List<Guid> { movementPointGuid };
            List<PointValue> userDataPointValues = this.GetUserDataPointValues(movementPointGuidList);

            if(userDataPointValues != null && userDataPointValues.Count > 0)
            {
                foreach(PointValue pointValue in userDataPointValues)
                {
                    if (pointValue.WellKnownIdentityGuid == Guids.UserData01WellKnownGuid) model.UserData01 = this.GetUserDataValue(pointValue);
                    if (pointValue.WellKnownIdentityGuid == Guids.UserData02WellKnownGuid) model.UserData02 = this.GetUserDataValue(pointValue);
                    if (pointValue.WellKnownIdentityGuid == Guids.UserData03WellKnownGuid) model.UserData03 = this.GetUserDataValue(pointValue);
                    if (pointValue.WellKnownIdentityGuid == Guids.UserData04WellKnownGuid) model.UserData04 = this.GetUserDataValue(pointValue);
                    if (pointValue.WellKnownIdentityGuid == Guids.UserData05WellKnownGuid) model.UserData05 = this.GetUserDataValue(pointValue);
                    if (pointValue.WellKnownIdentityGuid == Guids.UserData06WellKnownGuid) model.UserData06 = this.GetUserDataValue(pointValue);
                    if (pointValue.WellKnownIdentityGuid == Guids.UserData07WellKnownGuid) model.UserData07 = this.GetUserDataValue(pointValue);
                    if (pointValue.WellKnownIdentityGuid == Guids.UserData08WellKnownGuid) model.UserData08 = this.GetUserDataValue(pointValue);
                    if (pointValue.WellKnownIdentityGuid == Guids.UserData09WellKnownGuid) model.UserData09 = this.GetUserDataValue(pointValue);
                    if (pointValue.WellKnownIdentityGuid == Guids.UserData10WellKnownGuid) model.UserData10 = this.GetUserDataValue(pointValue);
                }
            }

            return model;
        }

        /// <summary>
        /// This method will get a list of user data point values based on the movement point Guid
        /// and well know tag Guids.
        /// </summary>
        /// <param name="movementPointGuidList">The movement point Guid list to retrieve.</param>
        /// <returns>Returns a list of point values.</returns>
        private List<PointValue> GetUserDataPointValues(List<Guid> movementPointGuidList)
        {
            var pointValueIdentifierList = FMChannelHelper.MakeCall<IPointTags, List<PointValueIdentifier>>(
                x => x.EnumeratePointValueIdentifersByPointAndTagLists(this.Security, movementPointGuidList, this.wellKnownTagGuids));

            var pointValueList = FMChannelHelper.MakeCall<IPointServiceManager, List<PointValue>>(x => x.GetPointValueData(this.Security, pointValueIdentifierList, false));
            return pointValueList;
        }

        /// <summary>
        /// This method will return the point value identifier Guid associated to the user data well
        /// known Guid.
        /// </summary>
        /// <param name="pointValueIdentifierList">The point value identifier list associated to the movement.</param>
        /// <param name="wellKnownGuid">The user data well know Guid.</param>
        /// <returns>Return the point value identifier Guid.</returns>
        private Guid GetTemplateGuid(ref List<PointValueIdentifier> pointValueIdentifierList, Guid wellKnownGuid)
        {
            PointValueIdentifier valueIdentifier = pointValueIdentifierList.Find(x => x.WellKnownIdentityGuid == wellKnownGuid);
            if(valueIdentifier == null)
            {
                return Guid.Empty;
            }

            return valueIdentifier.IdentityGuid;
        }

        /// <summary>
        /// This method make that call to the service to save the movement user data.
        /// </summary>
        /// <param name="model">The model to save.</param>
        private void SaveMovementUserDataHelper(MovementUserDataEditorModel model)
        {
            List<Guid> movementPointGuidList = new List<Guid> { model.MovementPointGuid };
            List<PointValue> userDataPointValues = this.GetUserDataPointValues(movementPointGuidList);

            var site = FMChannelHelper.MakeCall<ISites, SiteClass>(x => x.Get(this.Security, this.Security.SiteGuid, false, false, false));

            if (userDataPointValues != null && userDataPointValues.Count > 0)
            {
                foreach (PointValue pointValue in userDataPointValues)
                {
                    if (pointValue.WellKnownIdentityGuid == Guids.UserData01WellKnownGuid) this.SetUserDataValue(pointValue, model.UserData01);
                    if (pointValue.WellKnownIdentityGuid == Guids.UserData02WellKnownGuid) this.SetUserDataValue(pointValue, model.UserData02);
                    if (pointValue.WellKnownIdentityGuid == Guids.UserData03WellKnownGuid) this.SetUserDataValue(pointValue, model.UserData03);
                    if (pointValue.WellKnownIdentityGuid == Guids.UserData04WellKnownGuid) this.SetUserDataValue(pointValue, model.UserData04);
                    if (pointValue.WellKnownIdentityGuid == Guids.UserData05WellKnownGuid) this.SetUserDataValue(pointValue, model.UserData05);
                    if (pointValue.WellKnownIdentityGuid == Guids.UserData06WellKnownGuid) this.SetUserDataValue(pointValue, model.UserData06);
                    if (pointValue.WellKnownIdentityGuid == Guids.UserData07WellKnownGuid) this.SetUserDataValue(pointValue, model.UserData07);
                    if (pointValue.WellKnownIdentityGuid == Guids.UserData08WellKnownGuid) this.SetUserDataValue(pointValue, model.UserData08);
                    if (pointValue.WellKnownIdentityGuid == Guids.UserData09WellKnownGuid) this.SetUserDataValue(pointValue, model.UserData09);
                    if (pointValue.WellKnownIdentityGuid == Guids.UserData10WellKnownGuid) this.SetUserDataValue(pointValue, model.UserData10);
                }

                EditValueController.SetPointValues(this.Security, site, userDataPointValues);
            }
        }

        /// <summary>
        /// This method will return the user data string from the point value.
        /// </summary>
        /// <param name="pointValue">The point value containing the data.</param>
        /// <returns>Returns a user data string or empty string.</returns>
        private string GetUserDataValue(PointValue pointValue)
        {
            if (pointValue != null && pointValue.Value != null)
            {
                return (string)pointValue.Value;
            }

            return string.Empty;
        }

        /// <summary>
        /// This method will set the user data string from the point value.
        /// </summary>
        /// <param name="pointValue">The point value containing the data.</param>
        /// <param name="userData">The user data to update.</param>
        private void SetUserDataValue(PointValue pointValue, string userData)
        {
            if (pointValue != null)
            {
                pointValue.ServerTimeStamp = DateTimeOffset.UtcNow;
                pointValue.SourceTimeStamp = DateTimeOffset.UtcNow;
                pointValue.Status = StatusCodes.Good;
                pointValue.Value = userData;
            }
        }
        #endregion
    }
}