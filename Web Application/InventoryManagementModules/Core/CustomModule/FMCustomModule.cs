
/// <summary>
/// Purpose: Executes a custom script written in javascript, and updates the relevant PointTags of the Point on which the module is being executed,
/// with the results of the module execution.
/// General Rules for the Custom Javascript Code:
/// 1. The main object/class must be named CustomLogic.
/// 2. The CustomLogic javascript object must have the following code text "Calculate: function () {" declared on a separate line. 
///	This corresponds to a method called Calculate that takes no parameters. 
///	This is the entry-point method that gets executed by FMCustomModule through ClearScript.
/// 3. To reference any of the parametes (PointTags) defined for the module, the user's custom javascript code must do so by using the "this" keyword (e.g. this.pointTagx)
/// 4. ModuleExtMethods is a javascript object that provides a set of PointTag utility methods. It can be referenced directly in the user's custom javascript code (e.g. ModuleExtMethods.IsPointTagValueGood()).
/// 5. Only PointTag module parameters that are defined as Calculated tags or as OPCUA tags with an Input property of True are updated with the corresponding javascript PointTag objects at the end of a module execution.
/// </summary>

namespace CustomModule
{
	using System;
	using System.Collections.Generic;

	using System.IO;
	using System.Linq;

	using Microsoft.ClearScript.V8;
	using System.Threading;
	using System.Threading.Tasks;

	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.DataObjects.CodedVariables;
	using FMBusinessObjects.Interfaces;
	using FMPointCommon;


	public class FMCustomModule : FuelsManagerModule, IFuelsManagerModule
	{
		public const int MaxExecutionTime = 5000;

		public Point TargetPoint;		
		private Guid ModuleToPointTemplateGuid { get; set; }
		private ModuleToPointTemplateMap TargetModule { get; set; }
		private Dictionary<string, PointTag> ModuleTags { get; set; }
		private Dictionary<string, PointProperty> ModuleSettings { get; set; }
		private string ModuleLogicScript { get; set; }


		public ModuleInputOutputCollection GetInputOutputCollection(string calculationName)
		{
			var properties = new ModuleInputOutputCollection
								{
									new ModuleInputOutput
									{
											ID = "Custom Module",
											Type = typeof(double?),
											ParameterType = ModuleInputOutputType.Input
									}
								};
			return properties;
		}


		public FMCustomModule(Point point, string moduleInstanceGuid, Dictionary<Guid, ModuleToPointTemplateMap> moduleInstances, Dictionary<Guid, string> moduleLogicScript)
		{
			this.ModuleToPointTemplateGuid = Guid.Empty;
			this.TargetModule = null;
			this.ModuleLogicScript = null;
			this.ModuleTags = new Dictionary<string, PointTag>();
			this.ModuleSettings = new Dictionary<string, PointProperty>();

			this.TargetPoint = point;
			try
			{
				this.ModuleToPointTemplateGuid = Guid.Parse(moduleInstanceGuid);
			}
			catch (Exception)
			{
			}

			if ((this.ModuleToPointTemplateGuid != Guid.Empty) && (moduleInstances != null) && (moduleInstances.ContainsKey(this.ModuleToPointTemplateGuid)))
			{
				this.TargetModule = moduleInstances[this.ModuleToPointTemplateGuid];
			}

			if ((TargetModule != null) && (this.TargetModule.ModuleToPointTemplateData != null) && (this.TargetModule.ModuleToPointTemplateData.TagToModules.Length > 0))
			{
				Dictionary<Guid, PointTag> pointTags = new Dictionary<Guid, PointTag>(point.Tags.Count);
				Dictionary<Guid, PointProperty> pointProperties = new Dictionary<Guid, PointProperty>(point.Properties.Count);
				foreach (var tag in point.Tags.Values)
				{
					if(tag.PointTemplateTagGuid == Guid.Empty)
					{
						continue;
					}

					pointTags.Add(tag.PointTemplateTagGuid, tag);
				}
				foreach (var setting in point.Properties.Values)
				{
					pointProperties.Add(setting.PointTemplatePropertyGuid, setting);
				}


				for ( int i = 0; i < this.TargetModule.ModuleToPointTemplateData.TagToModules.Length; i++)
				{
					TagToModule tagToModule = this.TargetModule.ModuleToPointTemplateData.TagToModules[i];
					PointTag pointTag = null;
					pointTags.TryGetValue(tagToModule.TagGuid, out pointTag);
					this.ModuleTags.Add(tagToModule.ModuleParameter, pointTag);
				}

				for (int i = 0; i < this.TargetModule.ModuleToPointTemplateData.PropertyToModules.Length; i++)
				{
					PropertyToModule propertyToModule = this.TargetModule.ModuleToPointTemplateData.PropertyToModules[i];
					PointProperty pointProperty = null;
					pointProperties.TryGetValue(propertyToModule.PropertyGuid, out pointProperty);
					this.ModuleSettings.Add(propertyToModule.PropertyName, pointProperty);
				}
			}

			if ((TargetModule != null) && (this.TargetModule.ModuleGuid != null) && (moduleLogicScript != null) && (moduleLogicScript.ContainsKey(this.TargetModule.ModuleGuid)))
			{
				this.ModuleLogicScript = moduleLogicScript[this.TargetModule.ModuleGuid];
			}

		}


		public bool CustomModuleCalculation(V8ScriptEngine v8Engine)
		{
			if (v8Engine == null)
			{
				return false;
			}

			var checkpoint = new ManualResetEventSlim(false);
			var task = Task.Run(() => CustomModuleProcessing(checkpoint, v8Engine));
			if (!checkpoint.Wait(MaxExecutionTime))
			{
				v8Engine.Interrupt();
			}
			task.Wait();
			return true;
		}

		public string AddTimer(Int16 seconds)
		{
			var timerGuid = base.AddTimer(this.TargetPoint.PointGuid, DateTimeOffset.UtcNow.AddSeconds(seconds));
			return timerGuid.ToString();
		}

		public string AddTimer(Int64 scheduleTime)
		{
			var timerGuid = base.AddTimer(this.TargetPoint.PointGuid, new DateTimeOffset((scheduleTime * 10000) + 621355968000000000, new TimeSpan()));
			return timerGuid.ToString();
		}

		public void RemoveTimer(string timerGuid)
		{
			if (!string.IsNullOrEmpty(timerGuid))
			{
				base.RemoveTimer(new Guid(timerGuid));
			}
		}

		public bool CustomModuleProcessing(ManualResetEventSlim eventSlim, V8ScriptEngine v8Engine)
		{
			if ((v8Engine == null) || (this.ModuleLogicScript == null))
				return false;

			string jsExtMethodsFilePath = @"CustomModuleExtMethods.js";  //in the FMPointService bin folder
			string jsBaseContent = File.ReadAllText(jsExtMethodsFilePath);

			string jsModuleScript = this.ModuleLogicScript;
			//string jsModuleScript = File.ReadAllText(@"CustomModuleScript5.js");

			string calculateMethodLine = @"Calculate: function () {";

			int iIndex = jsModuleScript.IndexOf(calculateMethodLine);

			//Translate all Point Tag parameters of the module as javascript objects declared as attributes of the CustomLogic javascript object/class
			string tagVarDeclaration = null;
			try { 
				foreach (KeyValuePair<string, PointTag> moduleTag in this.ModuleTags)
				{
					tagVarDeclaration += GetPointTagJavascriptDeclaration(moduleTag.Key, moduleTag.Value, true);
				}
			}
			catch (Exception ex) {
                ex.Data.Add("executionContext", this.TargetPoint.ID + ": " + this.TargetModule.ID);
                throw ex;
            }

            //Translate all the Settings of the module as javascript objects declared as attributes of the CustomLogic javascript object/class
            foreach (KeyValuePair<string, PointProperty> pointProperty in this.ModuleSettings)
			{
				// Standard Settings must be accessed as Host Objects
				if (pointProperty.Value.ValueTypeString == "FMBusinessObjects.DataObjects.QuantityModuleSettings"
				|| pointProperty.Value.ValueTypeString == "FMBusinessObjects.DataObjects.RateModuleSettings"
				|| pointProperty.Value.ValueTypeString == "FMBusinessObjects.DataObjects.StrapTable"
				|| pointProperty.Value.ValueTypeString == "FMBusinessObjects.DataObjects.TankCommandModuleSettings"
				|| pointProperty.Value.ValueTypeString == "FMBusinessObjects.DataObjects.TankTransferModuleSettings"
				|| pointProperty.Value.ValueTypeString == "FMBusinessObjects.DataObjects.VcfModuleSettings"
				|| pointProperty.Value.ValueTypeString == "FMBusinessObjects.DataObjects.Vessel"
				|| pointProperty.Value.ValueTypeString == "FMBusinessObjects.DataObjects.MovementNodeModuleSettings"
				|| pointProperty.Value.ValueTypeString == "FMBusinessObjects.DataObjects.MovementModuleSettings")
				{
					continue;
				}
				else
				{
					try { 
						tagVarDeclaration += GetModuleSettingJavascriptDeclaration(pointProperty.Key, pointProperty.Value, true);
                    }
                    catch (Exception ex)
                    {
                        ex.Data.Add("executionContext", this.TargetPoint.ID + ": " + this.TargetModule.ID);
                        throw ex;
                    }
                }
            }



			string jsContent = jsBaseContent
									+ " " + Environment.NewLine
									+ jsModuleScript.Substring(0, iIndex)
									+ " " + Environment.NewLine
									+ tagVarDeclaration 
									+ " " + Environment.NewLine 
									+ calculateMethodLine
									+ " " + Environment.NewLine
									+ jsModuleScript.Substring(iIndex + calculateMethodLine.Length);

			try
			{
				v8Engine.Execute(jsContent);
			}
			catch (Exception ex)
			{
            ex.Data.Add("executionContext", this.TargetPoint.ID + ": " + this.TargetModule.ID);
            throw ex;
			}


			v8Engine.AddHostObject("ModuleCtrl", this);

			// Add Standard Settings as host objects
			foreach (KeyValuePair<string, PointProperty> pointProperty in this.ModuleSettings)
			{
				// Standard Settings must be accessed as Host Objects
				if (pointProperty.Value.ValueTypeString == "FMBusinessObjects.DataObjects.QuantityModuleSettings"
				|| pointProperty.Value.ValueTypeString == "FMBusinessObjects.DataObjects.RateModuleSettings"
				|| pointProperty.Value.ValueTypeString == "FMBusinessObjects.DataObjects.StrapTable"
				|| pointProperty.Value.ValueTypeString == "FMBusinessObjects.DataObjects.TankCommandModuleSettings"
				|| pointProperty.Value.ValueTypeString == "FMBusinessObjects.DataObjects.TankTransferModuleSettings"
				|| pointProperty.Value.ValueTypeString == "FMBusinessObjects.DataObjects.VcfModuleSettings"
				|| pointProperty.Value.ValueTypeString == "FMBusinessObjects.DataObjects.Vessel"
				|| pointProperty.Value.ValueTypeString == "FMBusinessObjects.DataObjects.MovementNodeModuleSettings")
				{
					v8Engine.AddHostObject(pointProperty.Key, pointProperty.Value.Value);
				}
				else
				{
					continue;
				}
			}



			try
			{
				v8Engine.Script.CustomLogic.Calculate();
			}
			catch (Exception ex)
			{
                ex.Data.Add("executionContext", this.TargetPoint.ID + ": " + this.TargetModule.ID);
                throw ex;
			}
			eventSlim.Set();			

			//Retrieve the javascript object level property and use it to set the properties of the C# object to be updated by this method.
			foreach (KeyValuePair<string, PointTag> moduleTag in this.ModuleTags)
			{
				string paramName = moduleTag.Key;
				PointTag pointTag = moduleTag.Value;
				var scriptTagObject = v8Engine.Script.CustomLogic[paramName];
				if (scriptTagObject != null)
				{
					SetTargetPointTag(paramName, pointTag, scriptTagObject, v8Engine);
				}
			}
			return true;
		}



		/// <summary>
		/// Update a PointTag of the target Point with the corresponding javascript object for the PointTag
		/// </summary>
		/// <param name="pointTag"></param>
		private void SetTargetPointTag(string paramName, PointTag pointTag, dynamic scriptTagObject, V8ScriptEngine v8Engine)
		{
			try { 
				long status = Convert.ToInt64(scriptTagObject.Status);

                pointTag.AlarmsEnabled = Convert.ToBoolean(scriptTagObject.AlarmsEnabled);

				if (pointTag.Alarms.Any())
				{
					foreach (var alarm in pointTag.Alarms.Values)
					{
						var alarmName = alarm.ID.Replace(" ", "");
						var scriptAlarmObject = scriptTagObject[alarmName];
						if (scriptAlarmObject != null)
						{
							alarm.Enabled = Convert.ToBoolean(scriptAlarmObject.Enabled);
							foreach (var alarmTest in alarm.AlarmTests.Values)
							{
								var alarmTestName = alarmTest.ID.Replace(" ", "");
								var scriptAlarmTestObject = scriptAlarmObject[alarmTestName];
								if (scriptAlarmTestObject != null)
								{
									alarmTest.Enabled = Convert.ToBoolean(scriptAlarmTestObject.Enabled);
								}
							}
						}
					}
				}

				if (!IsTagWritable(pointTag))
				{
					return;
				}


				try
				{
					object value = null;
					Opc.Ua.StatusCode statusCode;

					switch (pointTag.ValueTypeString)
					{ 
						case "System.DateTime":
							if (scriptTagObject.Value != null)
							{
								value = new DateTime(Convert.ToInt64(scriptTagObject.Value) * 10000 + 621355968000000000);
							}
							break;

						case "System.DateTimeOffset":
							if (scriptTagObject.Value != null)
							{
								value = new DateTimeOffset(Convert.ToInt64(scriptTagObject.Value) * 10000 + 621355968000000000, new TimeSpan());
							}
							break;

						case "System.TimeSpan":
							if (scriptTagObject.Value != null)
							{
								value = new TimeSpan(Convert.ToInt64(scriptTagObject.Value) * 10000);
							}
							break;


						case "FMBusinessObjects.DataObjects.PointCommandStatusListReference":
							var pointCommandStatusListReference = pointTag.Value as PointCommandStatusListReference;
							if (pointCommandStatusListReference != null)
							{
								value = new PointCommandStatusListReference()
								{
									PointCommandStatusListGuid = pointCommandStatusListReference.PointCommandStatusListGuid,
									CurrentValue = (scriptTagObject.Value != null) ? Convert.ToInt32(scriptTagObject.Value) : null
								};

								if (pointCommandStatusListReference.CurrentValue == (value as PointCommandStatusListReference).CurrentValue)
								{
									(value as PointCommandStatusListReference).CurrentKey = pointCommandStatusListReference.CurrentKey;
								}
							}
							break;

						case "FMBusinessObjects.DataObjects.DeviceAlarmMapReference":
							var deviceAlarmMapReference = pointTag.Value as DeviceAlarmMapReference;
							if (deviceAlarmMapReference != null)
							{
								value = new DeviceAlarmMapReference()
								{
									DeviceAlarmMapGuid = deviceAlarmMapReference.DeviceAlarmMapGuid,
									CurrentValue = (scriptTagObject.Value != null) ? Convert.ToUInt32(scriptTagObject.Value) : null
								};
							}
							break;

				default:
						value = scriptTagObject.Value;
						statusCode = new Opc.Ua.StatusCode((uint)status);
						PointManager.ValidatePointTagValueByItsType(pointTag.ValueTypeString, ref value, ref statusCode);
						status = statusCode.Code;
						
							break;
					}

					if ((pointTag.Value == null && value != null)
					|| (pointTag.Value != null && value == null)
					|| (pointTag.Value != null && !pointTag.Value.Equals(value))
					|| IsStatusChange(pointTag.Status, status))
					{
						pointTag.Value = value;
						pointTag.Status = status;
						pointTag.SourceTimeStamp = DateTimeOffset.UtcNow;

						if (pointTag.Input
						|| (pointTag.InputOutputType == PointTemplateTag.PointTagInputOutputType.Manual && !pointTag.Input))
						{
							pointTag.ServerTimeStamp = DateTimeOffset.UtcNow;
						}
					}
				}
				catch(Exception)
				{
						
				}
            }
            catch (Exception ex)
            {
                ex.Data.Add("executionContext", this.TargetPoint.ID + ": " + this.TargetModule.ID);
                throw ex;
            }
        }


        private static bool IsTagWritable(PointTag tag)
		{
			return (tag.InputOutputType == PointTemplateTag.PointTagInputOutputType.Calculated
			|| (tag.InputOutputType == PointTemplateTag.PointTagInputOutputType.Manual && !tag.Input)) ? true : false;
		}


		/// <summary>
		/// Translate a C# PointTag parameter into a Javascript object parameter, either as a class attribute or as a method variable
		/// </summary>
		/// <param name="paramName"></param>
		/// <param name="pointTag"></param>
		/// <param name="classLevelDeclaration"></param>
		/// <returns></returns>
		private string GetPointTagJavascriptDeclaration(string paramName, PointTag pointTag, Boolean classLevelDeclaration)
		{
			string varDeclaration = Environment.NewLine + "var " + paramName + " = {";
			if (classLevelDeclaration)
				varDeclaration = Environment.NewLine + paramName + ": {";

			string valueString = GetJavascriptValueString(pointTag.Value, pointTag.ValueTypeString) ;

			varDeclaration += " PointTagGuid: '" + pointTag.PointTagGuid.ToString() + "',"
									+ " Value:" + valueString + ","
									+ " Status:" + Convert.ToString(pointTag.Status) + ","
									+ " AlarmsEnabled:" + pointTag.AlarmsEnabled.ToString().ToLower() + ","
									+ " DecimalPlaces:" + Convert.ToString(pointTag.DecimalPlaces) + ","
									+ " EngineeringUnitsType:'" + pointTag.EngineeringUnitsType.ToString() + "',"
									+ " Units:'" + pointTag.Units.ToString() + "',"
                           + " ServerTimestamp:'" + pointTag.ServerTimeStamp.ToString() + "',"
                           + " SourceTimestamp:'" + pointTag.SourceTimeStamp.ToString() + "',"
                           + " DataSource:'" + pointTag.InputOutputType + "'"; 

			if(pointTag.Alarms.Any())
			{
				varDeclaration += ",AlarmState:'" + pointTag.AlarmState + "',"
									+ " Acknowledged:" + (pointTag.Acknowledged ? "true" : "false");

				foreach (var alarm in pointTag.Alarms.Values)
				{
					varDeclaration += ", " + alarm.ID.Replace(" ","") + ": { "
										+ " Enabled:" + alarm.Enabled.ToString().ToLower();

					foreach (var alarmTest in alarm.AlarmTests.Values)
					{
						var pointTagAlarmStatus = alarm.AlarmStatus.Values.ToList().Single(x => x.AlarmTestGuid == alarmTest.AlarmTestGuid);

						varDeclaration += ", " + alarmTest.ID.Replace(" ", "") + ": {"
											+ " Enabled:" + alarmTest.Enabled.ToString().ToLower() + ","
											+ " AlarmTestFailed:" + pointTagAlarmStatus.AlarmTestFailed.ToString().ToLower() + ","
											+ " Acknowledged:" + pointTagAlarmStatus.Acknowledged.ToString().ToLower();
						varDeclaration += "}";
					}
					varDeclaration += "}";
				}
			}

			varDeclaration += "}";

			if (classLevelDeclaration)
				varDeclaration += ", ";
			else
				varDeclaration += "; ";

			return varDeclaration;
		}


		/// <summary>
		/// Translate a C# PointProperty parameter into a Javascript object parameter, either as a class attribute or as a method variable
		/// </summary>
		/// <param name="paramName"></param>
		/// <param name="pointTag"></param>
		/// <param name="classLevelDeclaration"></param>
		/// <returns></returns>
		private string GetModuleSettingJavascriptDeclaration(string settingName, PointProperty pointProperty, Boolean classLevelDeclaration)
		{
			string varDeclaration = Environment.NewLine + "var " + settingName+ " = {";
			if (classLevelDeclaration)
				varDeclaration = Environment.NewLine + settingName + ": {";

			string valueString = GetJavascriptValueString(pointProperty.Value, pointProperty.ValueTypeString);
			
			varDeclaration += " PointPropertyGuid: '" + pointProperty.PointPropertyGuid.ToString() + "',"
				+ " Value:" + valueString
				+ "}";

			if (classLevelDeclaration)
				varDeclaration += ", ";
			else
				varDeclaration += "; ";

			return varDeclaration;
		}


		private string GetJavascriptValueString(object value, string valueTypeString)
		{
			string targetValue = "null";
			if (value != null)
			{
				switch (valueTypeString)
				{
					case "System.String":
					case "FMBusinessObjects.DataObjects.CodedVariables.TankCommands":
					case "FMBusinessObjects.DataObjects.CodedVariables.TankStatuses":
					case "FMBusinessObjects.DataObjects.CodedVariables.TransferStatuses":
					case "FMBusinessObjects.DataObjects.CodedVariables.TransferModes":
					case "FMBusinessObjects.DataObjects.CodedVariables.TankTransferMode":
					case "FMBusinessObjects.DataObjects.CodedVariables.VolumeTransferMode":
					case "FMBusinessObjects.DataObjects.CodedVariables.TankOperationalMode":
					case "FMBusinessObjects.DataObjects.CodedVariables.MovementNodeCommand":
					case "FMBusinessObjects.DataObjects.CodedVariables.MovementCommand":
					case "FMBusinessObjects.DataObjects.CodedVariables.MovementStatus":
               case "FMBusinessObjects.DataObjects.CodedVariables.StrapTableSelect":
               case "FMBusinessObjects.DataObjects.CodedVariables.Reset":
					case "FMBusinessObjects.DataObjects.CodedVariables.RoofTypeEnum":
						targetValue = "'" + Convert.ToString(value) + "'";
						break;


					case "System.Boolean":
						targetValue = Convert.ToString(value).ToLower();
						break;

					case "FMBusinessObjects.DataObjects.PointCommandStatusListReference":
						var pointCommandStatusListReference = value as FMBusinessObjects.DataObjects.PointCommandStatusListReference;

						if (pointCommandStatusListReference != null
						&& pointCommandStatusListReference.CurrentValue.HasValue)
						{
							targetValue = Convert.ToString(pointCommandStatusListReference.CurrentValue.Value);
						}
						break;

					case "FMBusinessObjects.DataObjects.DeviceAlarmMapReference":
						var deviceAlarmMapReference = value as FMBusinessObjects.DataObjects.DeviceAlarmMapReference;

						if (deviceAlarmMapReference != null
						&& deviceAlarmMapReference.CurrentValue.HasValue)
						{
							targetValue = Convert.ToString(deviceAlarmMapReference.CurrentValue.Value);
						}
						break;


					case "System.DateTime":
						targetValue = ((((DateTime)value).Ticks - 621355968000000000) / 10000).ToString();
						break;

					case "System.DateTimeOffset":
						targetValue = ((((DateTimeOffset)value).UtcDateTime.Ticks - 621355968000000000) / 10000).ToString();
						break;

					case "System.TimeSpan":
						targetValue = (((TimeSpan)value).Ticks / 10000).ToString();
						break;

					default:	
						targetValue = Convert.ToString(value);
						break;
				}					
			}
			return targetValue;
		}


		public void LogMessage(object obj)
		{
			PointTag pointTag = obj as PointTag;
			if (pointTag != null)
			{

			}
			else
			{
				double? d = obj as double?;
				if (d != null)
				{

				}
			}
		}
	}
}
