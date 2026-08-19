namespace FMPointService.PointExecution
{
	using FMBusinessObjects.DataObjects;
	using FMCore;
	using Logging;
	using Microsoft.ClearScript.V8;
	using System;
	using System.Collections.Concurrent;
	using System.Collections.Generic;
	using System.Configuration;
	using System.Linq;
	using System.Threading.Tasks;
	using ThreadSupport;

   internal class PointExecutor
	{
		public readonly CalculationEngine CalculationEngine = new CalculationEngine();

		public StatisticsLogger StatisticsLogger = new StatisticsLogger();

		public EventLogger EventLogger = new EventLogger();

		public CalculatedValuesSaver CalculatedValuesSaver = new CalculatedValuesSaver();

		public int? PointProcessingThreads = null;

		public bool? ForceGarbageCollection = null;

		private ConcurrentQueue<V8ScriptEngine> _v8EngineQueue;


		public PointExecutor()
		{
			this._v8EngineQueue = new ConcurrentQueue<V8ScriptEngine>();

			InitJavascriptEngine();
		}

		private void InitJavascriptEngine()
		{
			if (PointProcessingThreads == null)
			{
				PointProcessingThreads = int.Parse(ConfigurationManager.AppSettings["PointProcessingThreads"]);
			}

			for (int i = 0; i < PointProcessingThreads; i++)
			{
				V8ScriptEngine v8Engine = new V8ScriptEngine();
				this._v8EngineQueue.Enqueue(v8Engine);
			}
		}

		private void CollectGarbageJavascriptEngine()
		{
			foreach(var v8Engine in this._v8EngineQueue)
			{
				v8Engine.CollectGarbage(true);
			}

			GC.Collect();
		}


		public void ExecutePoints(SecurityClass security, IEnumerable<Guid> pointGuids)
		{
			security.ThrowIfNull("security");
			pointGuids.ThrowIfNull("points");
			var pointLogicList = ThreadSharedData.Instance().GetPointsClearNeedsCalculation(pointGuids);
			if (PointProcessingThreads == null)
			{
            int appSettingPointProcessingThreads;
				if(int.TryParse(ConfigurationManager.AppSettings["PointProcessingThreads"], out appSettingPointProcessingThreads))
				{
               PointProcessingThreads = appSettingPointProcessingThreads;
				}
				else
				{
					PointProcessingThreads = 8;
            }
			}
            if (ForceGarbageCollection == null)
            {
                bool appSettingForceGarbageCollection;
                if (bool.TryParse(ConfigurationManager.AppSettings["ForceGarbageCollection"], out appSettingForceGarbageCollection))
                {
                    ForceGarbageCollection = appSettingForceGarbageCollection;
                }
                else
                {
                    ForceGarbageCollection = false;
                }
            }


            if (pointLogicList.Any())
			{
				Parallel.ForEach(pointLogicList, new ParallelOptions { MaxDegreeOfParallelism = (int)this.PointProcessingThreads }, pointLogic => this.ExecutePoint(security, pointLogic));



            if (ForceGarbageCollection == true)
            CollectGarbageJavascriptEngine();

         }
		}

		public void ExecutePoints(SecurityClass security, IEnumerable<Point> points)
		{
			security.ThrowIfNull("security");
			points.ThrowIfNull("points");

			List<Guid> pointGuids = new List<Guid>(42);

			foreach (var point in points)
			{
				pointGuids.Add(point.PointGuid);
			}

			this.ExecutePoints(security, pointGuids);
		}


		public void ExecutePoint( SecurityClass security, PointTemplateLogic pointLogic )
		{
			security.ThrowIfNull( "security" );
			pointLogic.ThrowIfNull("pointLogic");

			try
			{
				var timer = StatisticsLogger.Start("Execute " + pointLogic.Point.TemplateName);

				V8ScriptEngine v8Engine = null;
				if (!this._v8EngineQueue.IsEmpty)
					this._v8EngineQueue.TryDequeue(out v8Engine);

				CalculationEngine.Calculate(pointLogic, v8Engine, security);

				if (v8Engine != null)
					this._v8EngineQueue.Enqueue(v8Engine);

				CalculatedValuesSaver.SaveChangedPointTags(pointLogic.Point);

				StatisticsLogger.Stop(timer);
			}
			catch (Exception except)
			{
				EventLogger.Error(string.Format("Point Guid: {0}, PointExecutor.ExecutePoint: {1}", pointLogic?.Point?.IdentityGuid,  except));
			}
		}
	}
}
