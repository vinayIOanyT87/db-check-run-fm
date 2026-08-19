

namespace FMUAAlarmPlugins
{
    using System;
    using Softing.Opc.Ua.Sdk;

    /// <summary>
    /// A monitored variable with an ExclusiveLimitAlarm attached.
    /// </summary>
    public class LowLimitAlarmMonitor : BaseDataVariableState<double>
    {
        #region Constructors
        /// <summary>
        /// Initializes the item and attaches an alarm monitor.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="parent">The parent node state.</param>
        /// <param name="namespaceIndex">Index of the namespace.</param>
        /// <param name="name">The DisplayName and BrowseName of the node.</param>
        /// <param name="alarmName">The DisplayName and BrowseName of the alarm node.</param>
        /// <param name="initialValue">The initlal value of the node.</param>
        /// <param name="highLimit">The High limit of the alarm.</param>
        /// <param name="highHighLimit">The HighHigh limit of the alarm.</param>
        /// <param name="lowLimit">The Low limit of the alarm.</param>
        /// <param name="lowLowLimit">The LowLow limit of the alarm.</param>
        internal LowLimitAlarmMonitor(
            ISystemContext context,
            NodeState parent,
            ushort namespaceIndex,
            string name,
            string alarmName,
            double initialValue,
            double lowLimit)

            : base(parent)
        {
            // Initialize the item and assign a NodeId.
            this.Create(context, null, new QualifiedName(name, namespaceIndex), null, true);

            this.ReferenceTypeId = ReferenceTypeIds.HasComponent;
            this.Value = initialValue;
            this.StatusCode = StatusCodes.Good;
            this.Timestamp = DateTime.UtcNow;

            if (parent != null)
            {
                parent.AddChild(this);

                // Define event source.
                parent.AddNotifier(context, ReferenceTypeIds.HasEventSource, false, this);
                this.AddNotifier(context, ReferenceTypeIds.HasEventSource, true, parent);
            }

            // Attach the alarm monitor.
            InitializeAlarmMonitor(
                context,
                namespaceIndex,
                alarmName,
                initialValue,
                lowLimit);
        }

        public NodeState GetLowLimitNode()
        {
            return m_alarm.LowLimit;
        }

        #endregion

        #region Private Methods

        private void InitializeAlarmMonitor(
            ISystemContext context,
            ushort namespaceIndex,
            string alarmName,
            double initialValue,
            double lowLimit)
        {

            // Create the alarm object.
            m_alarm = new LowLimitAlarmState(this);

            // declare limit components
            m_alarm.LowLimit = new PropertyState<double>(m_alarm);

            // add optional components.
            m_alarm.Comment = new ConditionVariableState<LocalizedText>(m_alarm);
            m_alarm.ClientUserId = new PropertyState<string>(m_alarm);
            m_alarm.AddComment = new AddCommentMethodState(m_alarm);
            m_alarm.EnabledState = new TwoStateVariableState(m_alarm);
            //m_alarm.ConfirmedState = new TwoStateVariableState(m_alarm);
            //m_alarm.Confirm = new AddCommentMethodState(m_alarm);

            // specify reference type between the source and the alarm.
            m_alarm.ReferenceTypeId = ReferenceTypeIds.HasComponent;

            // This call initializes the condition from the type model (i.e. creates all of the objects
            // and variables required to store its state). The information about the type model was 
            // incorporated into the class when the class was created.
            //
            // This method also assigns new NodeIds to all of the components by calling the INodeIdFactory.New
            // method on the INodeIdFactory object which is part of the system context. The NodeManager provides
            // the INodeIdFactory implementation used here.
            m_alarm.Create(
                context,
                null,
                new QualifiedName(alarmName, namespaceIndex),
                null,
                true);

            AddChild(m_alarm);
            AddCondition(context, m_alarm);

            // Set input node.
            m_alarm.InputNode.Value = this.NodeId;
            
            // Initialize alarm information.
            m_alarm.SymbolicName = alarmName;
            m_alarm.EventType.Value = m_alarm.TypeDefinitionId;
            m_alarm.ConditionName.Value = m_alarm.SymbolicName;
            m_alarm.AutoReportStateChanges = true;
            m_alarm.Time.Value = DateTime.UtcNow;
            m_alarm.ReceiveTime.Value = m_alarm.Time.Value;
            m_alarm.LocalTime.Value = Utils.GetTimeZoneInfo();
            m_alarm.BranchId.Value = null;

            // Set state values.
            m_alarm.SetEnableState(context, true);
            m_alarm.SetLimitState(context, LimitAlarmStates.Inactive);
            m_alarm.SetSuppressedState(context, false);
            m_alarm.SetAcknowledgedState(context, false);
            m_alarm.Retain.Value = false;
            //m_alarm.SetConfirmedState(context, false);

            // Define limit values.
            m_alarm.LowLimit.Value = lowLimit;
            m_alarm.LowLimit.AccessLevel = AccessLevels.CurrentReadOrWrite;
            m_alarm.LowLimit.UserAccessLevel = AccessLevels.CurrentReadOrWrite;
            m_alarm.LowLimit.OnSimpleWriteValue += this.NodeValueSimpleEventHandler;
        }


        public ServiceResult NodeValueSimpleEventHandler(ISystemContext context, NodeState node, ref object value)
        {
            this.m_alarm.UpdateState(context, this.Value, (double)value);
            return ServiceResult.Good;
        }

        #endregion

        #region Private Fields
        private LowLimitAlarmState m_alarm;
        #endregion
    }
}
