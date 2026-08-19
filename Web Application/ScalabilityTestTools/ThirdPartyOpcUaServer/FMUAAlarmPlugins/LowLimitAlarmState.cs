
namespace FMUAAlarmPlugins
{
    using System;
    using Softing.Opc.Ua.Sdk;

    /// <summary>
    /// An exclusive limit alarm class that overrides the provided logic of the base class alarm
    /// </summary>
    class LowLimitAlarmState : ExclusiveLimitAlarmState
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MyExclusiveLimitAlarmState"/> class.
        /// </summary>
        /// <param name="parent"></param>
        internal LowLimitAlarmState(NodeState parent)
            : base(parent)
        { }

        public void UpdateState(ISystemContext context, double value, double lowLimit)
        {
            try
            {
                bool updateRequired = false;

                if (LimitState.CurrentState.Id.Value != ObjectIds.ExclusiveLimitStateMachineType_Low
                    && value <= lowLimit)
                {
                    SetLimitState(context, LimitAlarmStates.Low);
                    if (context.UserIdentity == null || string.IsNullOrEmpty(context.UserIdentity.DisplayName))
                    {
                        SetComment(context, new LocalizedText("en-US", "LowLimit exceded."), "SYSTEM");
                    }
                    else
                    {
                        SetComment(context, new LocalizedText("en-US", "LowLimit exceded."), context.UserIdentity.DisplayName);
                    }
                    SetSeverity(context, EventSeverity.High);
                    updateRequired = true;
                }
                else if (ActiveState.Id.Value != false
                    && value > lowLimit)
                {
                    SetLimitState(context, LimitAlarmStates.Inactive);
                    if (context.UserIdentity == null || string.IsNullOrEmpty(context.UserIdentity.DisplayName))
                    {
                        SetComment(context, new LocalizedText("en-US", "Alarm inactive."), "SYSTEM");
                    }
                    else
                    {
                        SetComment(context, new LocalizedText("en-US", "Alarm inactive."), context.UserIdentity.DisplayName);
                    }                  
                    SetSeverity(context, EventSeverity.Low);
                    updateRequired = true;
                }

                if (updateRequired)
                {
                    ReportExclusiveLimitAlarm(context);
                }
            }
            catch (Exception e)
            {
                System.Console.WriteLine(e.Message);
            }
        }
        /// <summary>
        /// Called after the value of the monitored variable has changed.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="value">The value of the node.</param>
        protected override void ProcessVariableChanged(ISystemContext context, object value)
        {
            double pressure = (double)value;
            if (LowLimit != null)
            {
                UpdateState(context, pressure, LowLimit.Value);
            }        
        }

        /// <summary>
        /// Reports the exclusive limit alarm.
        /// </summary>
        /// <param name="context">The context.</param>
        private void ReportExclusiveLimitAlarm(ISystemContext context)
        {
            // set event data.
            EventId.Value = Guid.NewGuid().ToByteArray();
            Time.Value = DateTime.UtcNow;
            ReceiveTime.Value = Time.Value;

            // not interested in disabled or inactive alarms.
            if (!EnabledState.Id.Value || !ActiveState.Id.Value)
            {
                Retain.Value = false;
            }
            else
            {
                Retain.Value = true;
            }

            // reset the acknowledged flag if the alarm
            SetAcknowledgedState(context, false);

            // report changes to node attributes.
            ClearChangeMasks(context, true);

            // check if events are being monitored for the source.
            if (AreEventsMonitored)
            {
                // create a snapshot.
                InstanceStateSnapshot e = new InstanceStateSnapshot();
                e.Initialize(context, this);

                // report the event.
                ReportEvent(context, e);
                this.ReportEvent(context, e);
            }
        }
    }
}

