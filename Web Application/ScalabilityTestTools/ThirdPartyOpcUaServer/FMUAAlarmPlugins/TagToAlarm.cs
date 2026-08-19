
namespace FMUAAlarmPlugins
{
    using System;
    using Softing.Opc.Ua.Sdk;

    public class TagToAlarm : ExclusiveLimitAlarmState
    {
        internal TagToAlarm(ISystemContext context, NodeState parent, ushort namespaceIndex)
            : base(parent)
        {
            this.Comment = new ConditionVariableState<LocalizedText>(this);
            this.ClientUserId = new PropertyState<string>(this);
            this.AddComment = new AddCommentMethodState(this);
            this.EnabledState = new TwoStateVariableState(this);
            //this.ConfirmedState = new TwoStateVariableState(this);
            //this.Confirm = new AddCommentMethodState(this);

            // specify reference type between the source and the alarm.
            this.ReferenceTypeId = ReferenceTypeIds.HasComponent;

            // This call initializes the condition from the type model (i.e. creates all of the objects
            // and variables required to store its state). The information about the type model was 
            // incorporated into the class when the class was created.
            //
            // This method also assigns new NodeIds to all of the components by calling the INodeIdFactory.New
            // method on the INodeIdFactory object which is part of the system context. The NodeManager provides
            // the INodeIdFactory implementation used here.
            this.Create(
                context,
                null,
                new QualifiedName(parent.BrowseName.ToString() + " Alarm", namespaceIndex),
                null,
                true);

            parent.AddChild(this);
            parent.AddCondition(context, this);

            // Set input node.
            this.InputNode.Value = this.NodeId;

            // Initialize alarm information.
            this.SymbolicName = parent.BrowseName.ToString() + " Alarm";
            this.EventType.Value = this.TypeDefinitionId;
            this.ConditionName.Value = this.SymbolicName;
            this.AutoReportStateChanges = true;
            this.Time.Value = DateTime.UtcNow;
            this.ReceiveTime.Value = this.Time.Value;
            this.LocalTime.Value = Utils.GetTimeZoneInfo();
            this.BranchId.Value = null;

            // Set state values.
            this.SetEnableState(context, true);
            this.SetLimitState(context, LimitAlarmStates.Inactive);
            this.SetSuppressedState(context, false);
            this.SetAcknowledgedState(context, false);
            this.Retain.Value = false;
            //this.SetConfirmedState(context, false);

            //var parentNode = (InputTypeString)parent;
            //parentNode.Parent.AddNotifier(context, ReferenceTypeIds.HasEventSource, false, parentNode);
            //parentNode.AddNotifier(context, ReferenceTypeIds.HasEventSource, true, parentNode.Parent);

            //var parentNode = (InputTypeString)parent;
            //parentNode.Parent.AddNotifier(context, ReferenceTypeIds.HasEventSource, false, this);
            //this.AddNotifier(context, ReferenceTypeIds.HasEventSource, true, parentNode.Parent);

            var parentNode = (InputTypeString)parent;
            parentNode.Parent.AddNotifier(context, ReferenceTypeIds.HasEventSource, false, parentNode);
            parentNode.AddNotifier(context, ReferenceTypeIds.HasEventSource, true, parentNode.Parent);
            parentNode.AddNotifier(context, ReferenceTypeIds.HasEventSource, false, this);
            this.AddNotifier(context, ReferenceTypeIds.HasEventSource, true, parentNode);

        }

        public void UpdateState(ISystemContext context, string value)
        {
            try
            {
                bool updateRequired = false;
                var normalString = "NORMAL";
                if (LimitState.CurrentState.Id.Value != ObjectIds.ExclusiveLimitStateMachineType_High
                    && value.ToUpper() != normalString)
                {
                    SetLimitState(context, LimitAlarmStates.High);
                    if (context.UserIdentity == null || string.IsNullOrEmpty(context.UserIdentity.DisplayName))
                    {
                        SetComment(context, new LocalizedText("en-US", "HighLimit exceded."), "SYSTEM");
                    }
                    else
                    {
                        SetComment(context, new LocalizedText("en-US", "HighLimit exceded."), context.UserIdentity.DisplayName);
                    }                          
                    SetSeverity(context, EventSeverity.High);
                    updateRequired = true;
                }
                else if (value.ToUpper() == normalString)
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
            string alarmVal = (string)value;
            UpdateState(context, alarmVal);
                       
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

        //public TagToAlarm(
        //    ISystemContext context,
        //    NodeState parent)
        //    : base(parent)
        //{
        //    this.Create(context, null, new QualifiedName("TagToAlarm For Node " + parent.NodeId), null, true);
        //    this.SetupEventHandler(this.Parent);
           
        //    parent.AddNotifier(context, ReferenceTypeIds.HasNotifier, false, this);
        //    this.AddNotifier(context, ReferenceTypeIds.HasNotifier, true, parent);
        //}

        //protected void SetupEventHandler(NodeState node)
        //{
        //    if (node.GetType() == typeof(DataItemState<string>)
        //             || node.GetType().IsSubclassOf(typeof(DataItemState<string>)))
        //    {
        //        var str = ((DataItemState<string>)node);
        //        str.OnSimpleWriteValue += this.NodeValueSimpleEventHandler;
        //    }
        //    else if (node.GetType() == typeof(BaseDataVariableState<string>)
        //             || node.GetType().IsSubclassOf(typeof(BaseDataVariableState<string>)))
        //    {
        //        var str = ((BaseDataVariableState<string>)node);
        //        str.OnSimpleWriteValue += this.NodeValueSimpleEventHandler;
        //    }
        //    else if (node.GetType() == typeof(PropertyState<string>)
        //             || node.GetType().IsSubclassOf(typeof(PropertyState<string>)))
        //    {
        //        var str = ((PropertyState<string>)node);
        //        str.OnSimpleWriteValue += this.NodeValueSimpleEventHandler;
        //    }
        //}


        //public ServiceResult NodeValueSimpleEventHandler(ISystemContext context, NodeState node, ref object value)
        //{
        //    if (node.NodeId.ToString() == this.Parent.NodeId.ToString())
        //    {
        //        if (node.GetType() == typeof(DataItemState<string>)
        //                 || node.GetType().IsSubclassOf(typeof(DataItemState<string>)))
        //        {
        //            var str = ((DataItemState<string>)node);
        //            if(str.Value != (string)value)
        //            {
        //                this.ReportAlarm(context);
        //            }
        //        }
        //        else if (node.GetType() == typeof(BaseDataVariableState<string>)
        //                 || node.GetType().IsSubclassOf(typeof(BaseDataVariableState<string>)))
        //        {
        //            var str = ((BaseDataVariableState<string>)node);
        //            if (str.Value != (string)value)
        //            {
        //                this.ReportAlarm(context);
        //            }

        //        }
        //        else if (node.GetType() == typeof(PropertyState<string>)
        //                 || node.GetType().IsSubclassOf(typeof(PropertyState<string>)))
        //        {
        //            var str = ((PropertyState<string>)node);
        //            if (str.Value != (string)value)
        //            {
        //                this.ReportAlarm(context);
        //            }
        //        }
        //    }
        //    return ServiceResult.Good;
        //}


        //protected void ReportAlarm(ISystemContext context)
        //{
        //    // set event data.
        //    EventId.Value = Guid.NewGuid().ToByteArray();
        //    EventType.Value = NodeId;
        //    SourceNode.Value = Parent.NodeId;
        //    SourceName.Value = Parent.BrowseName.ToString();
        //    Time.Value = DateTime.UtcNow;
        //    ReceiveTime.Value = Time.Value;
        //    //LocalTime
        //    Message.Value = ((DataItemState<string>)Parent).Value + "Alarm";
        //    Severity.Value = 900;
        //    // report changes to node attributes.
        //    ClearChangeMasks(context, true);

        //    // check if events are being monitored for the source.
        //    if (AreEventsMonitored)
        //    {
        //        // create a snapshot.
        //        InstanceStateSnapshot e = new InstanceStateSnapshot();
        //        e.Initialize(context, this);

        //        // report the event.
        //        ReportEvent(context, e);
        //        this.ReportEvent(context, e);
        //    }
        //}
    }
}

