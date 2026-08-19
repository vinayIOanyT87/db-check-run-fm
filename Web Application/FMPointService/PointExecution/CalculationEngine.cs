namespace FMPointService.PointExecution
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Reflection;
	using System.ServiceModel.Configuration;

	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.Interfaces;
	using Microsoft.ClearScript.V8;

	using Logging;
	using FMBusinessObjects.UtilityObjects;

	using FMCore;

	using InProcLogging;
    using CSScriptLibrary;

    internal class CalculationEngine
	{

		public readonly EventLogger EvntLogger = new EventLogger();

		public void Calculate(PointTemplateLogic pointLogic, V8ScriptEngine v8Engine, SecurityClass security)
		{
			security.ThrowIfNull("security");
			pointLogic.ThrowIfNull("pointLogic");

			try
			{
				var alrmEngine = new AlarmEngine();

				pointLogic.Execute(v8Engine, PointTemplateLogic.CalculationType.Standard, null);

				alrmEngine.EvaluateAlarms(pointLogic.Point, security);
			}
			catch (Exception except)
			{
				// TODO: Report error to alarm system?

				// TODO: Write error on Application Event Log
				if (except.InnerException?.Data["executionContext"] != null)
				{
					 Logger.LogError("Calculation Exception: " + except.InnerException?.Data["executionContext"] + "\n" + except);
                EvntLogger.Error("Calculation Exception: " + except.InnerException?.Data["executionContext"] + "\n" + except);
            }
				else 
				{ 
					 Logger.LogError("Calculation Exception: " + except);
					 EvntLogger.Error("Calculation Exception: " + except);
				}
			}

		}

		public void PointCalculate(PointTemplateLogic pointLogic, V8ScriptEngine v8Engine, SecurityClass security)
		{
			security.ThrowIfNull("security");
			pointLogic.ThrowIfNull("pointLogic");

			try
			{
				pointLogic.Execute(v8Engine, PointTemplateLogic.CalculationType.Calculator, null);
			}
			catch (Exception except)
			{
            if (except.InnerException?.Data["executionContext"] != null)
            {
					 Logger.LogError("Calculation Exception: " + except.InnerException?.Data["executionContext"] + "\n" + except);
					 EvntLogger.Error("Calculation Exception: " + except.InnerException?.Data["executionContext"] + "\n" + except);
            }
				else
            {
                Logger.LogError("Calculation Exception: " + except);
                EvntLogger.Error("Calculation Exception: " + except);
            }
         }

		}




		//-------------------------------------------------Handling Async Method Calls
		/*
				private static bool IsTagParameter(
						string parameterName,
						List<ModuleCalculationParameter> parameterMap,
						Point point,
						out object value)
				{
					foreach (var parameter in parameterMap)
					{
						if (parameter.ParameterType == CalculationParameterType.Input
							|| parameter.ParameterType == CalculationParameterType.InOut)
						{
							if (parameter.ParameterName == parameterName)
							{
								value = point.Tags[parameter.TagKey].Value;
								return true;
							}
						}
					}
					value = null;
					return false;
				}
		*/

		private static int GetIndexOfInOutReturn(string parameterName, ParameterInfo[] parametersInfo)
		{
			for (int i = 0; i < parametersInfo.Count(); i++)
			{
				if (parametersInfo[i].Name == parameterName)
				{
					return i;
				}
			}
			return -1;
		}

		public void AsyncMethodInvoke(
		Point point,
		Guid moduleCalculationGuid,
		ParameterCollection parameters,
		SecurityClass security, ref Dictionary<Guid, PointTag> modifiedTags)
		{
			try
			{
/*
				var moduleInst = point.GetModuleInstanceForCalculationGuid(moduleCalculationGuid);
				// Process each module defined in the point.  Modules are classes in a target assembly.
				if (moduleInst != null)
				{
					var module = point.Modules[moduleInst.ModuleGuid];
					var calculation = moduleInst.Calculations[moduleCalculationGuid];
					var moduleInstance = module.Assembly.CreateInstance(module.ModuleTypeName);
					// Set the result tag value
					ModuleCalculationParameter output = null;
					foreach (var parameter in calculation.ParameterMap.Values)
					{
						if (parameter.ParameterType == CalculationParameterType.Output)
						{
							output = parameter;
						}
					}

					PointTag outputTag = null;
					if (output != null)
					{
						outputTag = point.Tags[output.TagKey];
					}

					// Do not perform the calculation if the output tag has an override value.
					if (outputTag != null
					&& outputTag.OpcStatusCodeBits == StatusCodes.GoodLocalOverride)
					{
						return;
					}

					// Build the parameter list.  The list should have at least one parameter.
					// Each parameter list contains a list of parameters of type Input or Output.
					// There should only be one Output parameter map.

					// TODO: Add support for module properties.  Must be set before calculations begin.

					// TODO: Move this class instantiation to the setup phase to save on garbage collection.

					calculation.ParameterList = SetUpParameters(
							calculation.MethodInfo.GetParameters(),
							parameters,
							calculation.ParameterMap.Values.ToList(),
							point);

					// Call the calculation
					var resultValue = calculation.MethodInfo.Invoke(moduleInstance, calculation.ParameterList);

					foreach (var parameter in calculation.ParameterMap.Values)
					{
						if (parameter.ParameterType == CalculationParameterType.InOut)
						{
							int index2 = GetIndexOfInOutReturn(parameter.ParameterName, calculation.MethodInfo.GetParameters());
							if (index2 < 0)
							{
								throw new Exception("Invalid Index returned from GetIndexOfInOutReturn!");
							}
							if (calculation.ParameterList[index2] != point.Tags[parameter.TagKey].Value)
							{
								var tag = point.Tags[parameter.TagKey];
								tag.Value = calculation.ParameterList[index2];
								tag.ServerTimeStamp = DateTimeOffset.UtcNow;
								tag.SourceTimeStamp = DateTimeOffset.UtcNow;
								if (tag.Value != null)
								{
									tag.Status = StatusCodes.Good;
								}
								else
								{
									tag.Status = StatusCodes.Bad;
								}
								modifiedTags.Add(parameter.TagKey, tag);
							}
						}
					}

					if (outputTag != null
					&& ((outputTag.Value == null && resultValue != null)
					|| (resultValue == null && outputTag.Value != null)
					|| (outputTag.Value != null && resultValue != null && !outputTag.Value.Equals(resultValue))))
					{
						outputTag.Value = resultValue;
						outputTag.ServerTimeStamp = DateTimeOffset.UtcNow;
						outputTag.SourceTimeStamp = DateTimeOffset.UtcNow;

						if (outputTag.Value != null)
						{
							outputTag.Status = StatusCodes.Good;
						}
						else
						{
							outputTag.Status = StatusCodes.Bad;
						}
						modifiedTags.Add(output.TagKey, outputTag);
					}
				}
*/
			}
			catch (Exception except)
			{
				// TODO: Report error to alarm system?

				// TODO: Write error on Application Event Log
				EvntLogger.Error("Calculation Exception: " + except);
				Logger.LogError("Calculation Exception: " + except);
			}
		}
	}
}
