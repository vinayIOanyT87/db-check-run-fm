namespace FMPointCommon
{
	using System;
	using Opc.Ua;
	using FMBusinessObjects.DataObjects;
    using InProcLogging;

	public abstract class FuelsManagerModule
	{
		public delegate void SetPointTagHandler(PointTag pointTag);

		public delegate void SetPointPropertyHandler(string pointPropertyID);

		public delegate Point GetPointHandler();

		public FuelsManagerModule()
		{
		}

		protected bool IsStatusChange(long oldStatus, long newStatus)
		{
			return (new StatusCode((uint)oldStatus).CodeBits != new StatusCode((uint)newStatus).CodeBits) ? true : false;
		}


		protected void CheckForAndSetOverUnderRange(PointTag pointTag)
		{
			var tagstatusCode = new StatusCode((uint)pointTag.Status);

			if (pointTag.Value == null)
				return;

			if ((double)pointTag.Value > pointTag.Maximum)
			{
				tagstatusCode.LimitBits = LimitBits.High;
			}
			else if ((double)pointTag.Value < pointTag.Minimum)
			{
				tagstatusCode.LimitBits = LimitBits.Low;
			}
			else
			{
				tagstatusCode.LimitBits = LimitBits.None;
			}
			pointTag.Status = (long)tagstatusCode;

		}


		protected bool IsValueGood(PointTag tagToCheck)
		{
			// simple routine to determine if the value is valid or not
			if (tagToCheck.Value == null
			|| StatusCode.IsBad(new StatusCode((uint)tagToCheck.Status))
			|| ((tagToCheck.Value is Double || tagToCheck.Value is Single) && Double.IsNaN(Convert.ToDouble(tagToCheck.Value))))
			{
				return false;
			}
			else
			{
				return true;
			}
		}

		protected bool IsValueGood(PointValue valuetoCheck)
		{
			// simple routine to determine if the value is valid or not
			if (valuetoCheck.Value == null
			|| StatusCode.IsBad(new StatusCode((uint)valuetoCheck.Status)))
			{
				return false;
			}
			else
			{
				return true;
			}
		}

		protected bool IsStatusUncertain(PointTag tagToCheck)
		{
			var tagstatusCode = new StatusCode((uint)tagToCheck.Status);

			if (tagstatusCode.LimitBits == LimitBits.High
			|| tagstatusCode.LimitBits == LimitBits.Low
			|| StatusCode.IsUncertain(tagstatusCode)
			|| tagstatusCode.SubCode == StatusCodes.GoodLocalOverride)
			{
				return true;
			}
			else
			{
				return false;
			}
		}

		protected bool IsStatusUncertain(PointValue valueToCheck)
		{
			var tagstatusCode = new StatusCode((uint)valueToCheck.Status);

			if (tagstatusCode.LimitBits == LimitBits.High
			|| tagstatusCode.LimitBits == LimitBits.Low
			|| StatusCode.IsUncertain(tagstatusCode)
			|| tagstatusCode.SubCode == StatusCodes.GoodLocalOverride)
			{
				return true;
			}
			else
			{
				return false;
			}
		}


		protected void SetTimeStamps(PointTag [] pointTagArray, PointTag outputPointTag)
		{
			foreach(var pointTag in pointTagArray)
			{
				if(pointTag.SourceTimeStamp > outputPointTag.SourceTimeStamp)
				{
					outputPointTag.SourceTimeStamp = pointTag.SourceTimeStamp;
				}

				if (pointTag.ServerTimeStamp > outputPointTag.ServerTimeStamp)
				{
					outputPointTag.ServerTimeStamp = pointTag.ServerTimeStamp;
				}

			}
		}

		protected Guid AddTimer(Guid pointGuid, DateTimeOffset timerExpiration)
		{
			Guid uniqueTimerName = Guid.NewGuid();
			SRMTimerFunctions.AddTimer(uniqueTimerName.ToString(), pointGuid, timerExpiration);
			return uniqueTimerName;
		}

		protected void RemoveTimer(Guid timerGuid)
		{
			SRMTimerFunctions.RemoveTimer(timerGuid.ToString());
		}

        protected void LogError(string message, Exception e, PointTag tag = null)
        {
			if(tag != null)
			{
                Logger.LogError(message + " Point ID:"+ tag.PointID + " StackTrace: " + e.StackTrace);
            }
			else
			{
                Logger.LogError(message + " StackTrace: " + e);
            }

        }
    }
}
