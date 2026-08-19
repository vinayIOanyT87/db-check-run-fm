using System;
using System.Runtime.InteropServices;

namespace LegacyMovementRPC
{

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public class MOVEMENTGROUP
    {
        public String Name;
        public MOVEMENTGROUP(String name)
        {
            Name = name;
        }
        public MOVEMENTGROUP()
        {
            Name = String.Empty;
        }
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public class PRINTERDATA
    {
        public byte byLen;
        public byte byDefault;
        public String Name;

        public PRINTERDATA(String name, byte len, byte def)
        {
            Name = name;
            byLen = len;
            byDefault = def;
        }
        public PRINTERDATA()
        {
            Name = String.Empty;
            byLen = byDefault = 0;
        }
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public class DELIVERYTICKETNAME
    {
        public String Name;
        public DELIVERYTICKETNAME(String name)
        {
            Name = name;
        }
        public DELIVERYTICKETNAME()
        {
            Name = String.Empty;
        }
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public class NODEINSTANCEDATA
    {
        public String szName;//[37];
        public String szNameOld;//[37];
        public ushort wNodeID;
        public byte bType;
        public byte bSource;
        public byte bSetNamePerm;
        public byte bSetXfrModePerm;
        public byte bSetXfrModeInactivePerm;
        public byte bSetSetpointPerm;
        public byte bSetSetpointPercentPerm;
        public byte bCombined;
        public byte bSetpointDataValid;
        public byte bRangeDataValid;
        public byte bPercentDataValid;
        public byte bReferenceGrossValid;
        public byte bReferenceMassValid;
        public ushort wXfrMode;
        public ushort wXfrModeOld;
        public double dXfrSetpoint;
        public double dXfrSetpointOld;
        public double dXfrSetpointInPercent;
        public double dXfrSetpointInPercentOld;
        public byte bXfrSetpointUnits;
        public byte bXfrSetpointStyle;
        public double dXfrSetpointMax;
        public double dXfrSetpointMin;
        public double dXfrSetpointInPercentMax;
        public double dXfrSetpointInPercentMin;
        public double dXfrReferenceGross;
        public double dXfrReferenceMass;
        public byte bStatus;
        public String szTankDataBaseReference;//[129];
        public String szMeterGrossReference;//[129];
        public String szMeterNetReference;//[129];
        public String szMeterMassReference;//[129];
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public class MOVEINSTANCEDATA
    {
        public String szName;//[21];
        public String szOrder;//[21];
        public String szComment;//[201];
        public IntPtr szUserDef;//[10][ 31 ];
        public String szGroup;//[21];
        public String szReportName;//[81];
        public String szPrinterName;//[81];
        public String szInputPoint;//[81];
        public int lPlannedStartTime;
        public int bPlannedStartTimeOperational;//byte
        public int wPlannedStartTimeStatus;
        public int lAutoStartTime;
        public int bAutoStartTimeActive;
        public int wAutoStartTimeStatus;
        public int lAutoStopTime;
        public int bAutoStopTimeActive;
        public int wAutoStopTimeStatus;
        public ushort wZeroFlowHoldOffMinutes;
        public byte bType;
        public byte bCommit;
        public byte bOkPerm;
        public byte bOrderPerm;
        public byte bSourceSetpointsInPercentPerm;
        public byte bSourceSetpointsInPercent;
        public byte bSourceSetpointsInPercentOld;
        public byte bAutoDelete;
        public byte bStartOnNonZeroFlow;
        public byte bStopOnZeroFlow;
        public byte bInterlockSetpoints;
        public byte bIncludeHandValues;
        public byte bLineupActionSequence;
        public byte bLineupActionSequencePerm;
        public byte bHaltOnCompletion;
        public byte bInhibitSetpointOverrange;
        public byte bInhibitMovementType;
        public byte bIndividualNodeControl;
        public byte bUsePendingOperation;
        public byte bUseInputPoint;
        public byte bSendMvmntToSnapIn;
        public byte bMvmntToSnapInAvailable;
        public uint dwInitiationCount;
        public int tInitiationTime;
        public ushort wNumberOfNodes;
        public IntPtr pNodeInstanceData;
        public MOVEINSTANCEDATA() { }
        public MOVEINSTANCEDATA(MOVEMENTDATA m)
        {
            this.szName = String.Copy(m.szName);
            this.szOrder = String.Copy(m.szOrder);
            this.szComment = String.Copy(m.szComment);
            this.szGroup = String.Copy(m.szGroup);
            this.szReportName = String.Copy(m.szReportName);
            this.szPrinterName = String.Copy(m.szPrinterName);
            this.szInputPoint = String.Copy(m.szInputPoint);
            this.lPlannedStartTime = m.lPlannedStartTime;
            this.bPlannedStartTimeOperational = m.bPlannedStartTimeOperational;
            this.wPlannedStartTimeStatus = m.wPlannedStartTimeStatus;
            this.lAutoStartTime = m.lAutoStartTime;
            this.bAutoStartTimeActive = m.bAutoStartTimeActive;
            this.wAutoStartTimeStatus = m.wAutoStartTimeStatus;
            this.lAutoStopTime = m.lAutoStopTime;
            this.bAutoStopTimeActive = m.bAutoStopTimeActive;
            this.wAutoStopTimeStatus = m.wAutoStopTimeStatus;
            this.wZeroFlowHoldOffMinutes = m.wZeroFlowHoldOffMinutes;
            this.bType = m.bType;
            this.bCommit = m.bCommit;
            this.bOkPerm = m.bOkPerm;
            this.bOrderPerm = m.bOrderPerm;
            this.bSourceSetpointsInPercentPerm = m.bSourceSetpointsInPercentPerm;
            this.bSourceSetpointsInPercent = m.bSourceSetpointsInPercent;
            this.bSourceSetpointsInPercentOld = m.bSourceSetpointsInPercentOld;
            this.bAutoDelete = m.bAutoDelete;
            this.bStartOnNonZeroFlow = m.bStartOnNonZeroFlow;
            this.bStopOnZeroFlow = m.bStopOnZeroFlow;
            this.bInterlockSetpoints = m.bInterlockSetpoints;
            this.bIncludeHandValues = m.bIncludeHandValues;
            this.bLineupActionSequence = m.bLineupActionSequence;
            this.bLineupActionSequencePerm = m.bLineupActionSequencePerm;
            this.bHaltOnCompletion = m.bHaltOnCompletion;
            this.bInhibitSetpointOverrange = m.bInhibitSetpointOverrange;
            this.bInhibitMovementType = m.bInhibitMovementType;
            this.bIndividualNodeControl = m.bIndividualNodeControl;
            this.bUsePendingOperation = m.bUsePendingOperation;
            this.bUseInputPoint = m.bUseInputPoint;
            this.bSendMvmntToSnapIn = m.bSendMvmntToSnapIn;
            this.bMvmntToSnapInAvailable = m.bMvmntToSnapInAvailable;
            this.dwInitiationCount = m.dwInitiationCount;
            this.tInitiationTime = m.tInitiationTime;
            this.wNumberOfNodes = m.wNumberOfNodes;
        }
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public class MOVEMENTDATA
    {
        public String szName;//[21];
        public String szOrder;//[21];
        public String szComment;//[201];
        public String[] szUserDef;//[10][ 31 ];
        public String szGroup;//[21];
        public String szReportName;//[81];
        public String szPrinterName;//[81];
        public String szInputPoint;//[81];
        public int lPlannedStartTime;
        public int bPlannedStartTimeOperational;//byte
        public int wPlannedStartTimeStatus;
        public int lAutoStartTime;
        public int bAutoStartTimeActive;
        public int wAutoStartTimeStatus;
        public int lAutoStopTime;
        public int bAutoStopTimeActive;
        public int wAutoStopTimeStatus;
        public ushort wZeroFlowHoldOffMinutes;
        public byte bType;
        public byte bCommit;
        public byte bOkPerm;
        public byte bOrderPerm;
        public byte bSourceSetpointsInPercentPerm;
        public byte bSourceSetpointsInPercent;
        public byte bSourceSetpointsInPercentOld;
        public byte bAutoDelete;
        public byte bStartOnNonZeroFlow;
        public byte bStopOnZeroFlow;
        public byte bInterlockSetpoints;
        public byte bIncludeHandValues;
        public byte bLineupActionSequence;
        public byte bLineupActionSequencePerm;
        public byte bHaltOnCompletion;
        public byte bInhibitSetpointOverrange;
        public byte bInhibitMovementType;
        public byte bIndividualNodeControl;
        public byte bUsePendingOperation;
        public byte bUseInputPoint;
        public byte bSendMvmntToSnapIn;
        public byte bMvmntToSnapInAvailable;
        public uint dwInitiationCount;
        public int tInitiationTime;
        public ushort wNumberOfNodes;
        public NODEINSTANCEDATA[] NodeInstanceData;

        public MOVEMENTDATA() { }
        public MOVEMENTDATA(MOVEINSTANCEDATA m)
        {
            this.szName = m.szName;
            this.szOrder = m.szOrder;
            this.szComment = m.szComment;
            this.szGroup = m.szGroup;
            this.szReportName = m.szReportName;
            this.szPrinterName = m.szPrinterName;
            this.szInputPoint = m.szInputPoint;
            this.lPlannedStartTime = m.lPlannedStartTime;
            this.bPlannedStartTimeOperational = m.bPlannedStartTimeOperational;
            this.wPlannedStartTimeStatus = m.wPlannedStartTimeStatus;
            this.lAutoStartTime = m.lAutoStartTime;
            this.bAutoStartTimeActive = m.bAutoStartTimeActive;
            this.wAutoStartTimeStatus = m.wAutoStartTimeStatus;
            this.lAutoStopTime = m.lAutoStopTime;
            this.bAutoStopTimeActive = m.bAutoStopTimeActive;
            this.wAutoStopTimeStatus = m.wAutoStopTimeStatus;
            this.wZeroFlowHoldOffMinutes = m.wZeroFlowHoldOffMinutes;
            this.bType = m.bType;
            this.bCommit = m.bCommit;
            this.bOkPerm = m.bOkPerm;
            this.bOrderPerm = m.bOrderPerm;
            this.bSourceSetpointsInPercentPerm = m.bSourceSetpointsInPercentPerm;
            this.bSourceSetpointsInPercent = m.bSourceSetpointsInPercent;
            this.bSourceSetpointsInPercentOld = m.bSourceSetpointsInPercentOld;
            this.bAutoDelete = m.bAutoDelete;
            this.bStartOnNonZeroFlow = m.bStartOnNonZeroFlow;
            this.bStopOnZeroFlow = m.bStopOnZeroFlow;
            this.bInterlockSetpoints = m.bInterlockSetpoints;
            this.bIncludeHandValues = m.bIncludeHandValues;
            this.bLineupActionSequence = m.bLineupActionSequence;
            this.bLineupActionSequencePerm = m.bLineupActionSequencePerm;
            this.bHaltOnCompletion = m.bHaltOnCompletion;
            this.bInhibitSetpointOverrange = m.bInhibitSetpointOverrange;
            this.bInhibitMovementType = m.bInhibitMovementType;
            this.bIndividualNodeControl = m.bIndividualNodeControl;
            this.bUsePendingOperation = m.bUsePendingOperation;
            this.bUseInputPoint = m.bUseInputPoint;
            this.bSendMvmntToSnapIn = m.bSendMvmntToSnapIn;
            this.bMvmntToSnapInAvailable = m.bMvmntToSnapInAvailable;
            this.dwInitiationCount = m.dwInitiationCount;
            this.tInitiationTime = m.tInitiationTime;
            this.wNumberOfNodes = m.wNumberOfNodes;
        }
    }

    public class MovementRPC
    {
        private const string FMLegacyRPCInterfaceDll = "FMToLegacyInterface.dll";

        public const int CMD_Halt = 74;	// Pause
        public const int CMD_Reset = 135;
        public const int CMD_Start = 146;
        public const int CMD_Stop = 149;

        [DllImport(FMLegacyRPCInterfaceDll, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.U4)]
        static extern int ExecuteMvmntCmd(uint dwMoveInstID, ushort wMoveNodeID, ushort wCommand);

        [DllImport(FMLegacyRPCInterfaceDll, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.U4)]
        static extern int GetGroups(ref uint dwNumberOfGroups, ref IntPtr outArray);

        [DllImport(FMLegacyRPCInterfaceDll, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.U4)]
        static extern int GetPrinters(ref uint dwNumberOfPrinters, ref IntPtr outArray);

        [DllImport(FMLegacyRPCInterfaceDll, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.U4)]
        static extern int GetDeliveryTickets(ref uint dwNumberOfTickets, ref IntPtr outArray);

        [DllImport(FMLegacyRPCInterfaceDll, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.U4)]
        static extern int GetMovementInstance(uint dwMoveId, ref IntPtr outArray);

        [DllImport(FMLegacyRPCInterfaceDll, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.U4)]
        static extern int SetMovementInstance(IntPtr ptr, uint dwMoveId);

        public static bool ExecuteMovementCommand(uint dwMoveInstID, ushort wMoveNodeID, ushort wCommand)
        {
            int retCode = ExecuteMvmntCmd(dwMoveInstID, wMoveNodeID, wCommand);

            return (retCode == 1);
        }

        public static bool GetServerGroups(out MOVEMENTGROUP[] GroupNames)
        {
            int retCode = -1;
            uint dwNumberOfGroups = 0;
            MOVEMENTGROUP[] movementGroup = { new MOVEMENTGROUP("") };
            GroupNames = new MOVEMENTGROUP[] { };

            try
            {
                IntPtr intPtr = Marshal.AllocCoTaskMem(Marshal.SizeOf(movementGroup[0]));
                retCode = GetGroups(ref dwNumberOfGroups, ref intPtr);
                GroupNames = Marshaller.StuctArrayFromIntPtr<MOVEMENTGROUP>(intPtr, (int)dwNumberOfGroups);
            }
            catch(OutOfMemoryException outMem)
            {
            }
            return (retCode == 1);
        }
        public static bool GetServerPrinters(out PRINTERDATA[] PrinterNames)
        {
            int retCode = -1;
            uint dwNumberOfPrinters = 0;
            PRINTERDATA[] printers = { new PRINTERDATA() };
            PrinterNames = new PRINTERDATA[] { };

            try
            {
                IntPtr intPtr = Marshal.AllocCoTaskMem(Marshal.SizeOf(printers[0]));
                retCode = GetPrinters(ref dwNumberOfPrinters, ref intPtr);
                PrinterNames = Marshaller.StuctArrayFromIntPtr<PRINTERDATA>(intPtr, (int)dwNumberOfPrinters);
            }
            catch (OutOfMemoryException outMem)
            {
            }
            return (retCode == 1);
        }
        public static bool GetServerDeliveryTickets(out DELIVERYTICKETNAME[] DeliveryTicketNames)
        {
            int retCode = -1;
            uint dwNumberOfTickets = 0;
            DELIVERYTICKETNAME[] printers = { new DELIVERYTICKETNAME() };
            DeliveryTicketNames = new DELIVERYTICKETNAME[] { };

            try
            {
                IntPtr intPtr = Marshal.AllocCoTaskMem(Marshal.SizeOf(printers[0]));
                retCode = GetDeliveryTickets(ref dwNumberOfTickets, ref intPtr);
                DeliveryTicketNames = Marshaller.StuctArrayFromIntPtr<DELIVERYTICKETNAME>(intPtr, (int)dwNumberOfTickets);
            }
            catch (OutOfMemoryException outMem)
            {
            }
            return (retCode == 1);
        }

        public static bool GetMovementData(uint dwMoveId, out MOVEMENTDATA MovementData)
        {
            int retCode = -1;
            MovementData = new MOVEMENTDATA();

            try
            {
                IntPtr intPtr = Marshal.AllocCoTaskMem(Marshal.SizeOf(MovementData));

                retCode = GetMovementInstance(dwMoveId, ref intPtr);

                MOVEINSTANCEDATA[] mvData = Marshaller.StuctArrayFromIntPtr<MOVEINSTANCEDATA>(intPtr, 1);

                if (mvData.Length > 0)
                {
                    MovementData = Marshaller.BuildMovementData(mvData[0]);
                }
            }
            catch (OutOfMemoryException outMem)
            {
            }
            return (retCode == 1);
        }

        public bool SetMovementData(uint dwMoveId, MOVEMENTDATA MovementData)
        {
            int retCode = -1;

            MOVEINSTANCEDATA[] mvData = Marshaller.BuildMovementData(MovementData);

            try
            {
                IntPtr intPtr = Marshaller.IntPtrFromStuctArray<MOVEINSTANCEDATA>(mvData);

                retCode = SetMovementInstance(intPtr, dwMoveId);
            }
            catch (OutOfMemoryException outMem)
            {
            }
            return (retCode == 1);
        }
    }
}
