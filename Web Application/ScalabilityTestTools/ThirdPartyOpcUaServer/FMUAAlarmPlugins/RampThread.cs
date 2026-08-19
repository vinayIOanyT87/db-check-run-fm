using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMUAAlarmPlugins
{
	using FMUAAlarmPlugins;

	using Softing.Opc.Ua.Sdk;
	using Softing.Opc.Ua.Sdk.Server;

	public class RampThread : SrmThread
	{

		protected static RampThread mInst = null;

		protected List<InputTypeDouble> RegisteredDoubleNodes = new List<InputTypeDouble>();

		protected List<InputTypeFloat> RegisteredFloatNodes = new List<InputTypeFloat>();

		protected List<InputTypeString> RegisteredStringNodes = new List<InputTypeString>();

		protected object lockObj = new object();

		protected ServerSystemContext SystemContext;

		protected double MinRampValue = 51.00;

		protected double MaxRampValue = 449.00;

		protected int RampUpdateRateInSeconds = 1;

		protected double RampIncrement = 1;

		protected bool IncreasingRamp = true;

		protected RampThread(ServerSystemContext systemContext, double minRampValue, double maxRampValue, int rampUpdateRateInSeconds, double rampIncrement, bool increasingRamp)
		{
			MinRampValue = minRampValue;
			MaxRampValue = maxRampValue;
			RampUpdateRateInSeconds = rampUpdateRateInSeconds;
			SystemContext = systemContext;
			this.RampIncrement = rampIncrement;
			this.IncreasingRamp = increasingRamp;
			this.Start();
		}

		public void RegisterDouble(InputTypeDouble d)
		{
			lock (lockObj)
			{
				this.RegisteredDoubleNodes.Add(d);
			}
		}

		public void RegisterFloat(InputTypeFloat f)
		{
			lock (lockObj)
			{
				this.RegisteredFloatNodes.Add(f);
			}
		}

		public void RegisterString(InputTypeString s)
		{
			lock (lockObj)
			{
				this.RegisteredStringNodes.Add(s);
			}
		}

		protected List<InputTypeDouble> GetRegisteredDoubleNodes()
		{
			List<InputTypeDouble> ret = new List<InputTypeDouble>();
			lock (lockObj)
			{
				foreach (var node in this.RegisteredDoubleNodes)
				{
					ret.Add(node);
				}
			}
			return ret;
		}

		protected List<InputTypeFloat> GetRegisteredFloatNodes()
		{
			List<InputTypeFloat> ret = new List<InputTypeFloat>();
			lock (lockObj)
			{
				foreach (var node in this.RegisteredFloatNodes)
				{
					ret.Add(node);
				}
			}
			return ret;
		}

		protected List<InputTypeString> GetRegisteredStringNodes()
		{
			List<InputTypeString> ret = new List<InputTypeString>();
			lock (lockObj)
			{
				foreach (var node in this.RegisteredStringNodes)
				{
					ret.Add(node);
				}
			}
			return ret;
		}
		public static RampThread Initialize(ServerSystemContext systemContext, double minRampValue, double maxRampValue, int rampUpdateRateInSeconds, double rampIncrement, bool increasingRamp)
		{
			if (mInst == null)
			{
				mInst = new RampThread(systemContext, minRampValue, maxRampValue, rampUpdateRateInSeconds, rampIncrement, increasingRamp);
			}
			return mInst;
		}

		private double SetNextValue(double val)
		{
			double tempVal = val + this.RampIncrement;
			if (!IncreasingRamp)
			{
				tempVal = val - this.RampIncrement;
			}
			if (tempVal > MaxRampValue)
			{
				return MinRampValue;
			}
			else
			{
				if (val < MinRampValue)
				{
					return MaxRampValue;
				}
				return tempVal;
			}
		}

		protected void UpdateValues(DateTime timeStamp)
		{
			var dNodes = this.GetRegisteredDoubleNodes();
			foreach (var node in dNodes)
			{
				node.Value = this.SetNextValue(node.Value);
				node.Timestamp = timeStamp;
				node.StatusCode = StatusCodes.Good;
				node.ClearChangeMasks(SystemContext, false);
			}

			var fNodes = this.GetRegisteredFloatNodes();
			foreach (var node in fNodes)
			{
				node.Value = (float)this.SetNextValue((double)node.Value);
				node.Timestamp = timeStamp;
				node.StatusCode = StatusCodes.Good;
				node.ClearChangeMasks(SystemContext, false);
			}

			const string StringStatus = "Valid";
			var sNodes = this.GetRegisteredStringNodes();
			foreach (var node in sNodes)
			{
				if (string.IsNullOrEmpty(node.Value) || node.Value != StringStatus)
				{
					node.Value = StringStatus;
					node.Timestamp = timeStamp;
					node.StatusCode = StatusCodes.Good;
					node.ClearChangeMasks(SystemContext, false);
				}
			}
		}

		public override void Run()
		{
			long startTime = HighPerformanceTimer.Now;
			DateTime currentTime = DateTime.Now;
			long rampUpdateRateInTicks = HighPerformanceTimer.convertToTicks((double)this.RampUpdateRateInSeconds);
			long twoMillisecondMargin = HighPerformanceTimer.convertToTicks(0.002);
			long nextTarget = startTime + rampUpdateRateInTicks;
			while (true)
			{

				try
				{
					long lNow = HighPerformanceTimer.Now;
					if (lNow > nextTarget)
					{
						System.Console.WriteLine("Ramp Thread Not Able To Keep Up!  Target Time {0} Actual Time {1}", HighPerformanceTimer.convertToSeconds(nextTarget), HighPerformanceTimer.convertToSeconds(lNow));
					}
					else
					{
						if (lNow + twoMillisecondMargin < nextTarget)
						{
							HighPerformanceTimer.Wait(nextTarget - lNow - twoMillisecondMargin);
						}
						this.UpdateValues(currentTime);
					}

				}
				catch (Exception e)
				{

					EventLog eventLog = new EventLog("Application", ".", "FMUAAlarmService");
					eventLog.WriteEntry("RampThread : " + e.Message, EventLogEntryType.Error);
				}
				nextTarget = nextTarget + rampUpdateRateInTicks;
				currentTime = currentTime.AddSeconds(this.RampUpdateRateInSeconds);
			}
		}
	}
}
