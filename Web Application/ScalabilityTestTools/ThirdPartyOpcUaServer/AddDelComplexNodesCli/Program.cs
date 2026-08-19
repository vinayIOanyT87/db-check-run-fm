using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AddDelComplexNodesCli
{
	using FMBusinessObjects.DataObjects;

	using FMUAAlarmPluginInterface;

    using FMUAAlarmPlugins;

    using Softing.Opc.Ua.Sdk;

    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            //WriteExampleXMLFile();
            //WriteExampleXMLFile2();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }

        static string CreateFolder(string parentNodeId, string name, ushort namespaceIndex, ref AddNodeCollectionClass coll)
        {
            string nodeID = new NodeId(Guid.NewGuid().ToString(), namespaceIndex).ToString();
            var n1 = new AddNodeClass { NodeName = name, ParentNodeID = parentNodeId };
            var n1Request = new AddNodeRequestClass
                            {
                                Sender = ShawnSenderStr,
                                DynamicEntityType = new FolderFactory().GetDynamicEntityTypeName()
                            };
            var inputParams = new ParameterCollection();
            inputParams[FolderFactory.NameKey] = n1.NodeName;
            inputParams[FolderFactory.NodeIDKey] = nodeID;
            n1Request.InputParameters = inputParams;
            n1.NodeXML = n1Request.ToXML();
            coll.Add(n1);
            return nodeID;
        }

        static string CreateDouble(string parentNodeId, string name, double value, ushort namespaceIndex, ref AddNodeCollectionClass coll)
        {
            string nodeID = new NodeId(Guid.NewGuid().ToString(), namespaceIndex).ToString();
            var n1 = new AddNodeClass { NodeName = name, ParentNodeID = parentNodeId };
            var n1Request = new AddNodeRequestClass
            {
                Sender = ShawnSenderStr,
                DynamicEntityType = new InputTypeDoubleFactory().GetDynamicEntityTypeName()
            };
            var inputParams = new ParameterCollection();
            inputParams[InputTypeStringFactory.NameKey] = name;
            inputParams[InputTypeStringFactory.ValueKey] = value;
            inputParams[InputTypeStringFactory.NodeIdKey] = nodeID;
            n1Request.InputParameters = inputParams;
            n1.NodeXML = n1Request.ToXML();
            coll.Add(n1);
            return nodeID;
        }

        static string CreateString(string parentNodeId, string name, string value, ushort namespaceIndex, ref AddNodeCollectionClass coll)
        {
            string nodeID = new NodeId(Guid.NewGuid().ToString(), namespaceIndex).ToString();
            var n1 = new AddNodeClass { NodeName = name, ParentNodeID = parentNodeId };
            var n1Request = new AddNodeRequestClass
            {
                Sender = ShawnSenderStr,
                DynamicEntityType = new InputTypeStringFactory().GetDynamicEntityTypeName()
            };
            var inputParams = new ParameterCollection();
            inputParams[InputTypeStringFactory.NameKey] = name;
            inputParams[InputTypeStringFactory.ValueKey] = value;
            inputParams[InputTypeStringFactory.NodeIdKey] = nodeID;
            n1Request.InputParameters = inputParams;
            n1.NodeXML = n1Request.ToXML();
            coll.Add(n1);
            return nodeID;
        }

        static void CreateConnctedHighLimitAlarm(
            string parentNodeId,
            string name,
            double initialValue,
            double highLimit,
            string valueTagNodeId,
            string highLimitTagNodeId,
            ref AddNodeCollectionClass coll)
        {
            var n1 = new AddNodeClass { NodeName = name, ParentNodeID = parentNodeId };
            var n1Request = new AddNodeRequestClass
            {
                Sender = ShawnSenderStr,
                DynamicEntityType = new ConnectedHighLimitAlarmFactory().GetDynamicEntityTypeName()
            };
            var inputParams = new ParameterCollection();
            inputParams[ConnectedHighLimitAlarmFactory.PointGuidKey] = Guid.NewGuid().ToString();
            inputParams[ConnectedHighLimitAlarmFactory.PointIDKey] = "Tank";
            inputParams[ConnectedHighLimitAlarmFactory.ModuleNameKey] = "HighLimit";
            inputParams[ConnectedHighLimitAlarmFactory.ModuleGuidKey] = Guid.NewGuid().ToString();
            inputParams[ConnectedHighLimitAlarmFactory.ModuleCalculationGuidKey] = Guid.NewGuid().ToString();
            inputParams[ConnectedHighLimitAlarmFactory.MethodNameKey] = "Alarm";
            inputParams[ConnectedHighLimitAlarmFactory.HighLimitKey] = highLimit;
            inputParams[ConnectedHighLimitAlarmFactory.HighLimitNodeIDKey] = highLimitTagNodeId;
            inputParams[ConnectedHighLimitAlarmFactory.ValueKey] = initialValue;
            inputParams[ConnectedHighLimitAlarmFactory.ValueNodeIDKey] = valueTagNodeId;
            n1Request.InputParameters = inputParams;
            n1.NodeXML = n1Request.ToXML();
            coll.Add(n1);
        }

        static void CreateConnctedLowLimitAlarm(
            string parentNodeId,
            string name,
            double initialValue,
            double lowLimit,
            string valueTagNodeId,
            string lowLimitTagNodeId,
            ref AddNodeCollectionClass coll)
        {
            var n1 = new AddNodeClass { NodeName = name, ParentNodeID = parentNodeId };
            var n1Request = new AddNodeRequestClass
            {
                Sender = ShawnSenderStr,
                DynamicEntityType = new ConnectedLowLimitAlarmFactory().GetDynamicEntityTypeName()
            };
            var inputParams = new ParameterCollection();
            inputParams[ConnectedLowLimitAlarmFactory.PointGuidKey] = Guid.NewGuid().ToString();
            inputParams[ConnectedLowLimitAlarmFactory.PointIDKey] = "Tank";
            inputParams[ConnectedLowLimitAlarmFactory.ModuleNameKey] = "LowLimit";
            inputParams[ConnectedLowLimitAlarmFactory.ModuleGuidKey] = Guid.NewGuid().ToString();
            inputParams[ConnectedLowLimitAlarmFactory.ModuleCalculationGuidKey] = Guid.NewGuid().ToString();
            inputParams[ConnectedLowLimitAlarmFactory.MethodNameKey] = "Alarm";
            inputParams[ConnectedLowLimitAlarmFactory.LowLimitKey] = lowLimit;
            inputParams[ConnectedLowLimitAlarmFactory.LowLimitNodeIDKey] = lowLimitTagNodeId;
            inputParams[ConnectedLowLimitAlarmFactory.ValueKey] = initialValue;
            inputParams[ConnectedLowLimitAlarmFactory.ValueNodeIDKey] = valueTagNodeId;
            n1Request.InputParameters = inputParams;
            n1.NodeXML = n1Request.ToXML();
            coll.Add(n1);
        }

        public static void CreateSignalSelector(
            string parentNodeId,
            string name,
            string signal1NodeId,
            string signal2NodeId,
            string signal3NodeId,
            string signal4NodeId,
            string highSignalNodeId,
            string lowSignalNodeId,
            ref AddNodeCollectionClass coll)
        {
            var n1 = new AddNodeClass { NodeName = name, ParentNodeID = parentNodeId };
            var n1Request = new AddNodeRequestClass
            {
                Sender = ShawnSenderStr,
                DynamicEntityType = new SignalSelectorFactory().GetDynamicEntityTypeName()
            };
            var inputParams = new ParameterCollection();
            inputParams[SignalSelectorFactory.NameKey] = name;
            inputParams[SignalSelectorFactory.Signal1NodeIdKey] = signal1NodeId;
            inputParams[SignalSelectorFactory.Signal2NodeIdKey] = signal2NodeId;
            inputParams[SignalSelectorFactory.Signal3NodeIdKey] = signal3NodeId;
            inputParams[SignalSelectorFactory.Signal4NodeIdKey] = signal4NodeId;
            inputParams[SignalSelectorFactory.LowSignalNodeIdKey] = lowSignalNodeId;
            inputParams[SignalSelectorFactory.HighSignalNodeIdKey] = highSignalNodeId;
            n1Request.InputParameters = inputParams;
            n1.NodeXML = n1Request.ToXML();
            coll.Add(n1);
        }

        public static string CreateTank(
            string parentNodeId,
            string name,
            ushort namespaceindex,
            ref AddNodeCollectionClass coll)
        {
            string tankNodeID = CreateFolder(parentNodeId, name, namespaceindex, ref coll);
            string signal1NodeId = CreateDouble(tankNodeID, "Signal1", 100, namespaceindex, ref coll);
            string signal2NodeId = CreateDouble(tankNodeID, "Signal2", 200, namespaceindex, ref coll);
            string signal3NodeId = CreateDouble(tankNodeID, "Signal3", 300, namespaceindex, ref coll);
            string signal4NodeId = CreateDouble(tankNodeID, "Signal4", 400, namespaceindex, ref coll);
            string highSignalNodeId = CreateDouble(tankNodeID, "HighSignal", 0, namespaceindex, ref coll);
            string lowSignalNodeId = CreateDouble(tankNodeID, "LowSignal", 0, namespaceindex, ref coll);
            string highLimitNodeId = CreateDouble(tankNodeID, "HighLimit", 450, namespaceindex, ref coll);
            string lowLimitNodeId = CreateDouble(tankNodeID, "LowLimit", 50, namespaceindex, ref coll);
            CreateSignalSelector(tankNodeID, "SignalSelector", signal1NodeId, signal2NodeId, signal3NodeId, signal4NodeId, highSignalNodeId, lowSignalNodeId,ref coll);
            CreateConnctedHighLimitAlarm(tankNodeID, "Tank High Limit Alarm", 0, 100, highSignalNodeId, highLimitNodeId,ref coll);
            CreateConnctedLowLimitAlarm(tankNodeID, "Tank Low Limit Alarm", 100, 0, lowSignalNodeId, lowLimitNodeId, ref coll);
            return tankNodeID;
        }

        public const string ShawnSenderStr = "shawnSender"; 

        static void WriteExampleXMLFile()
        {
            const int NumSites = 3500;
            const int NumDoubles = 50;
            const int NumStrings = 50;
            const string TopLevelNodeId = "ns=2;i=3";
            const ushort Namespaceindex = 2;
            var coll = new AddNodeCollectionClass();
            for (int i = 0; i < NumSites; i++)
            {
                string folderNodeID = CreateFolder(TopLevelNodeId, "Site" + i, Namespaceindex, ref coll);
                for (int j = 0; j < NumDoubles; j++)
                {
                    string doubleNodeId = CreateDouble(folderNodeID, "D" + j, (double)j * 100, Namespaceindex, ref coll);
                }
                for (int k = 0; k < NumStrings; k++)
                {
                    string doubleNodeId = CreateString(folderNodeID, "S" + k, (k * 100).ToString(), Namespaceindex, ref coll);
                }
            }
            coll.ToFile("NodeConfig.xml");
        }

        static void WriteExampleXMLFile2()
        {
            const int NumSites = 2000;
            const int NumTanks = 25;
            const string TopLevelNodeId = "ns=2;i=3";
            const ushort Namespaceindex = 2;
            var coll = new AddNodeCollectionClass();
            for (int i = 0; i < NumSites; i++)
            {
                string folderNodeID = CreateFolder(TopLevelNodeId, "Site" + i, Namespaceindex, ref coll);
                for (int j = 0; j < NumTanks; j++)
                {
                    string tankNodeId = CreateTank(folderNodeID, "Tank" + j,Namespaceindex, ref coll);
                }
            }
            coll.ToFile("NodeConfig.xml");
        }
    }
}
