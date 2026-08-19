namespace FMPointService.PointExecution
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	using System.Threading.Tasks;
	using Microsoft.ClearScript.V8;

	using FMBusinessObjects.DataObjects;
	using InProcLogging;
	using ThreadSupport;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.BusinessInterfaces;

	public abstract class PointTemplateLogic
	{
		public enum CalculationType
		{
			Standard = 0,
			Calculator = 1
		}

		protected static PointExecutionQueuer PointExecutionQueuer = new PointExecutionQueuer();
		public Point Point { get; set; }
		public Dictionary<Guid, PointTag> Tags { get; set; }
		protected Dictionary<Guid, PointProperty> Properties { get; set; }
		protected bool InitializationFailed { get; set; }

		public PointTemplateLogic(Point point)
		{
			InitializationFailed = false;
			Tags = new Dictionary<Guid, PointTag>(point.Tags.Count);
			Properties = new Dictionary<Guid, PointProperty>(point.Properties.Count);

			this.Point = point;

			foreach (var tag in point.Tags.Values)
			{
				if (tag.PointTemplateTagGuid != Guid.Empty)
				{
					this.Tags.Add(tag.PointTemplateTagGuid, tag);
				}
			}

			foreach (var property in point.Properties.Values)
			{
				if (property.PointTemplatePropertyGuid != Guid.Empty)
				{
					this.Properties.Add(property.PointTemplatePropertyGuid, property);
				}
			}
		}

		protected PointTag GetTag(string pointTemplateTagGuid)
		{
			try
			{
				return Tags[new Guid(pointTemplateTagGuid)];
			}
			catch(Exception except)
			{
				Logger.LogError("GetTag exception PointTemplateTagGuid : " + pointTemplateTagGuid + " " + except.Message);
				this.InitializationFailed = true;
			}

			return null;
		}

		protected PointProperty GetProperty(string pointTemplatePropertyGuid)
		{
			try
			{
				return Properties[new Guid(pointTemplatePropertyGuid)];
			}
			catch (Exception except)
			{
				Logger.LogError("GetProperty exception PointTemplatePropertyGuid : " + pointTemplatePropertyGuid + " " + except.Message);
				this.InitializationFailed = true;
			}

			return null;
		}

		public abstract void Execute(V8ScriptEngine v8Engine, PointTemplateLogic.CalculationType calculationType, PointCalculatorData pointCalculatorData);

        public static void TimerCallback(Guid pointGuid)
		{
			PointExecutionQueuer.QueuePointForProcessing(pointGuid);
		}

		public void SetPointTag(PointTag pointTag)
		{
			ThreadSharedData.Instance().SetPointTag(pointTag);
		}

		public void SetPointProperty(string id)
		{
			foreach(var property in this.Point.Properties.Values)
			{
				if(property.ID == id)
				{
					ThreadSharedData.Instance().SetPointProperty(property);
					var security = new SecurityClass() { UserID = "FMPointService" };
					FMChannelHelper.MakeCall<IPointProperties>(x => x.ModifyPointPropertyValue(security, property, true, true));
				}
			}
		}

		public Point GetPoint()
		{
			return this.Point;
		}
	}
}
