using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Runtime.InteropServices;


namespace FMLegacyTestApp
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
		//[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 21)]
		//public String szName;//[21];
		//[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 21)]
		//public String szOrder;//[21];
		//[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 201)]
		//public String szComment;//[201];
		////[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 310)]
		//public IntPtr szUserDef;//[10][ 31 ];
		//[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 21)]
		//public String szGroup;//[21];
		//[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 81)]
		//public String szReportName;//[81];
		//[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 81)]
		//public String szPrinterName;//[81];
		//[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 81)]
		//public String szInputPoint;//[81];
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
		//[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 21)]
		//public String szName;//[21];
		//[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 21)]
		//public String szOrder;//[21];
		//[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 201)]
		//public String szComment;//[201];
		//[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 10)]
		//[MarshalAs(UnmanagedType, SizeConst = 10)]
		//public String[] szUserDef;//[10][ 31 ];
		//[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 21)]
		//public String szGroup;//[21];
		//[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 81)]
		//public String szReportName;//[81];
		//[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 81)]
		//public String szPrinterName;//[81];
		//[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 81)]
		//public String szInputPoint;//[81];
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

		public MOVEMENTDATA()
		{
			//szUserDef = new String[10];
		}
		public MOVEMENTDATA(MOVEINSTANCEDATA m) : this()
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

	class TestDllClass
	{
		#region Private data members	
		private const string FMLegacyRPCInterfaceDll = "C:\\repo\\fuelsmanager\\Web Application\\Utilities\\FMToLegacyInterface\\Debug\\FMToLegacyInterface.dll";
		#endregion

		[DllImport(FMLegacyRPCInterfaceDll, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
		[return: MarshalAs(UnmanagedType.U4)]
		static extern int SetMovementInstance(IntPtr ptr, uint dwMoveId);

		[DllImport(FMLegacyRPCInterfaceDll, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
		[return: MarshalAs(UnmanagedType.U4)]
		static extern int GetMovementInstance(uint dwMoveId, ref IntPtr outArray);

		[DllImport(FMLegacyRPCInterfaceDll, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
		[return: MarshalAs(UnmanagedType.U4)]
		static extern int GetDeliveryTickets(ref uint dwNumberOfTickets, ref IntPtr outArray);

		[DllImport(FMLegacyRPCInterfaceDll, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
		[return: MarshalAs(UnmanagedType.U4)]
		static extern int GetPrinters(ref uint dwNumberOfPrinters, ref IntPtr outArray);

		[DllImport(FMLegacyRPCInterfaceDll, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
		[return: MarshalAs(UnmanagedType.U4)]
		static extern int GetGroups(ref uint dwNumberOfGroups, ref IntPtr outArray);

		[DllImport(FMLegacyRPCInterfaceDll, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
		[return: MarshalAs(UnmanagedType.U4)]
		static extern int ExecuteMvmntCmd(uint dwMoveInstID, ushort wMoveNodeID, ushort wCommand);

		// define the dll functions that need to be verified
		[DllImport(FMLegacyRPCInterfaceDll, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
		[return: MarshalAs(UnmanagedType.U4)]
		public static extern int GetLeakDetectionConfigurationData(string tankName,
																						StringBuilder gaugename,
																						StringBuilder szanalyismethod,
																						StringBuilder sztankvol,
																						StringBuilder szleakunits,
																						StringBuilder szvolunits,
																						StringBuilder sztempunits,
																						StringBuilder szquality,
																						ref short psType,
																						ref double pdthreshold,
																						ref double pdcertification,
																						ref double pddeltatemp,
																						ref short psmintime);

		[DllImport(FMLegacyRPCInterfaceDll, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
		[return: MarshalAs(UnmanagedType.U4)]
		public static extern int CreateRunDestroyTankCalculator(string tankName,
														string UserName,
														double dLevel,
														double dTemperature,
														double dDensity,
														double dStdDensity,
														double dDensityTemp,
														double dAmbientTemp,
														double dWaterLevel,
														ref double RT_dStrapVolume,
														ref double RT_dWaterVolume,
														ref double RT_dGrossVolume,
														ref double RT_dDensity,
														ref double RT_dStdDensity,
														ref double RT_dVCF,
														ref double RT_dCTSh,
														ref double RT_dNetVolume,
														ref double RT_dMass);


		[DllImport(FMLegacyRPCInterfaceDll, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
		[return: MarshalAs(UnmanagedType.U4)]
		public static extern int GetHandGaugeDataStartEnd(int moveID,
			int nodeID,
			int getStartData,
			ref double RT_dLevel,
			ref long RT_LevelTime,
			ref double RT_dTemperature,
			ref long RT_TemperatureTime,
			ref double RT_dDensity,
			ref long RT_DensityTime,
			ref double RT_dDensityTemp,
			ref long RT_DensityTempTime,
			ref double RT_dAmbientTemp,
			ref long RT_AmbientTempTime,
			ref double RT_dRefHeight,
			ref long RT_RefHeightTime,
			ref double RT_dWaterLevel,
			ref long RT_WaterLevelTime,
			ref double RT_dStrapVolume,
			ref double RT_dWaterVolume,
			ref double RT_dGrossVolume,
			ref double RT_dStdDensity,
			ref double RT_dVCF,
			ref double RT_dCTSh,
			ref double RT_dNetVolume,
			ref double RT_dMass,
			ref double RT_dRoofMass,
			StringBuilder szEmployeeID);

		public void LoadDriverDll()
		{
			//GetLeakDetectionConfigurationData();
			//TestTankCalculator();
			//TestGetHandGaugeData();
			//ExecuteMovementCommand();
			//GetGroups();
			//GetDeliveryTickets();
			//GetPrinters();
			//GetMovementData();
			SetMovementData();
		}

		public const int CMD_Halt = 74; // Pause
		public const int CMD_Reset = 135;
		public const int CMD_Start = 146;
		public const int CMD_Stop = 149;

		public bool ExecuteMovementCommand()
		{
			uint dwMoveInstID = 1;
			ushort wMoveNodeID = 0;
			ushort wCommand = CMD_Stop;

			int retCode = ExecuteMvmntCmd(dwMoveInstID, wMoveNodeID, wCommand);

			return !(retCode == -1);
		}

		public bool GetGroups()
        {
			int retCode = -1;
			uint dwNumberOfGroups = 0;
			MOVEMENTGROUP[] movementGroup = { new MOVEMENTGROUP("") };
			MOVEMENTGROUP[] GroupNames = new MOVEMENTGROUP[] { };

			try
			{
				IntPtr intPtr = Marshal.AllocCoTaskMem(Marshal.SizeOf(movementGroup[0]));
				retCode = GetGroups(ref dwNumberOfGroups, ref intPtr);
				GroupNames = Marshaller.StuctArrayFromIntPtr<MOVEMENTGROUP>(intPtr, (int)dwNumberOfGroups);
			}
			catch (OutOfMemoryException outMem)
			{
			}
			return !(retCode == -1);
		}

		public bool GetDeliveryTickets()
		{
			int retCode = -1;
			uint dwNumberOfTickets = 0;
			DELIVERYTICKETNAME[] tickets = { new DELIVERYTICKETNAME() };
			DELIVERYTICKETNAME[] DeliveryTicketNames = new DELIVERYTICKETNAME[] { };

			try
			{
				IntPtr intPtr = Marshal.AllocCoTaskMem(Marshal.SizeOf(tickets[0]));
				retCode = GetDeliveryTickets(ref dwNumberOfTickets, ref intPtr);
				DeliveryTicketNames = Marshaller.StuctArrayFromIntPtr<DELIVERYTICKETNAME>(intPtr, (int)dwNumberOfTickets);
			}
			catch (OutOfMemoryException outMem)
			{
			}
			return !(retCode == -1);
		}

		public static bool GetPrinters()
		{
			int retCode = -1;
			uint dwNumberOfPrinters = 0;
			PRINTERDATA[] printers = { new PRINTERDATA() };
			PRINTERDATA[] PrinterNames = new PRINTERDATA[] { };

			try
			{
				IntPtr intPtr = Marshal.AllocCoTaskMem(Marshal.SizeOf(printers[0]));
				retCode = GetPrinters(ref dwNumberOfPrinters, ref intPtr);
				PrinterNames = Marshaller.StuctArrayFromIntPtr<PRINTERDATA>(intPtr, (int)dwNumberOfPrinters);
			}
			catch (OutOfMemoryException outMem)
			{
			}
			return !(retCode == -1);
		}

		public bool GetMovementData(out MOVEMENTDATA MovementData)
		{
			int retCode = -1;
			uint dwMoveId = 1;

			MovementData = new MOVEMENTDATA();

			try
			{
				IntPtr intPtr = Marshal.AllocCoTaskMem(Marshal.SizeOf(typeof(MOVEMENTDATA)));

				retCode = GetMovementInstance(dwMoveId, ref intPtr);

				if (retCode == 1)
				{
					MOVEINSTANCEDATA[] mvData = Marshaller.StuctArrayFromIntPtr<MOVEINSTANCEDATA>(intPtr, 1);

					if (mvData.Length > 0)
					{
						MovementData = Marshaller.BuildMovementData(mvData[0]);
					}
				}
				else
				{
					if (intPtr != IntPtr.Zero)
					{
						Marshal.FreeCoTaskMem(intPtr);
					}
				}
			}
			catch (OutOfMemoryException outMem)
			{
			}
			return (retCode == 1);
		}

		public bool SetMovementData()
		{
			int retCode = -1;
			uint dwMoveId = 1;
			
			bool bRet = GetMovementData(out MOVEMENTDATA MovementData);

			if (!bRet) return false;
			Random random = new Random();

			MovementData.bCommit = 1;
			MovementData.szGroup = "Group-" + random.Next(100, 1000);
			MovementData.szComment = "Comment-" + random.Next(100, 1000);

			Console.WriteLine("Group Name: {0}", MovementData.szGroup);
			Console.WriteLine("Comment: {0}", MovementData.szComment);

			for (int i = 0; i < 10; i++)
			{
				MovementData.szUserDef[i] = "User-" + random.Next(1, 100);
				Console.WriteLine("User Data-{1}: {0}", MovementData.szUserDef[i], (i+1));
			}

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

		public void GetLeakDetectionConfigurationData()
		{
			StringBuilder gaugename = new StringBuilder(255);
			StringBuilder szanalyismethod = new StringBuilder(255);
			StringBuilder sztankvol = new StringBuilder(255);
			StringBuilder szleakunits = new StringBuilder(255);
			StringBuilder szvolunits = new StringBuilder(255);
			StringBuilder sztempunits = new StringBuilder(255);
			StringBuilder szquality = new StringBuilder(255);
			double dthreshold = 0;
			short sType = 0;
			double dcertification = 0;
			double ddeltatemp = 0;
			short smintime = 0;

			int tt = GetLeakDetectionConfigurationData("tank1",
																	gaugename,
																	szanalyismethod,
																	sztankvol,
																	szleakunits,
																	szvolunits,
																	sztempunits,
																	szquality,
																	ref sType,
																	ref dthreshold,
																	ref dcertification,
																	ref ddeltatemp,
																	ref smintime);

			if (tt == -1)
			{
				Console.WriteLine("Error requesting leak detection config data:\n\t");
				return;
			}

			Console.WriteLine("Success in requesting leak detection config data:\n\t");

			//Console.WriteLine(":\n\t");
			Console.WriteLine("Passed in Tank: Tank1\n\t");
			Console.WriteLine("Gauge Name: " + gaugename + "\n\t");
			Console.WriteLine("Analysis Method: " + szanalyismethod + "\n\t");
			Console.WriteLine("Tank Vol: " + sztankvol + "\n\t");
			Console.WriteLine("Leak Units: " + szleakunits + "\n\t");
			Console.WriteLine("Volume Units: " + szvolunits + "\n\t");
			Console.WriteLine("Temperature Units: " + sztempunits + "\n\t");
			Console.WriteLine("Quality: " + szquality + "\n\t");
			Console.WriteLine("Type: " + sType.ToString() + "\n\t");
			Console.WriteLine("Threshold: " + dthreshold.ToString() + "\n\t");
			Console.WriteLine("Certification: " + dcertification.ToString() + "\n\t");
			Console.WriteLine("Delta Temp: " + ddeltatemp.ToString() + "\n\t");
			Console.WriteLine("Min Quiet Time: " + smintime.ToString() + "\n\t");

		}

		public void TestTankCalculator()
		{
			string UserName = "TestUser";
			
			// values to set
			double dLevel = 9.0;
			double dTemperature = 17;
			double dDensity = .76;
			double dStdDensity = .75;
			double dDensityTemp = .65;
			double dAmbientTemp = 65;
			double dWaterLevel = 0.0;

			double RT_dStrapVolume = 0.0;
			double RT_dWaterVolume = 0.0;
			double RT_dGrossVolume = 0.0;
			double RT_dDensity = 0.0;
			double RT_dStdDensity = 0.0;
			double RT_dVCF = 0.0;
			double RT_dCTSh = 0.0;
			double RT_dNetVolume = 0.0;
			double RT_dMass = 0.0;

			int tt = CreateRunDestroyTankCalculator("tank1",
													UserName,
													dLevel,
													dTemperature,
													dDensity,
													dStdDensity,
													dDensityTemp,
													dAmbientTemp,
													dWaterLevel,
													ref RT_dStrapVolume,
													ref RT_dWaterVolume,
													ref RT_dGrossVolume,
													ref RT_dDensity,
													ref RT_dStdDensity,
													ref RT_dVCF,
													ref RT_dCTSh,
													ref RT_dNetVolume,
													ref RT_dMass);
			if (tt == -1)
			{
				Console.WriteLine("Error Operating Initial Tank Calculator Creation:\n\t");
				return;
			}

			Console.WriteLine("Success in Operating Initial Tank Calculator:\n\t");

			Console.WriteLine("Passed in Tank: Tank1\n\t");

			Console.WriteLine("Strap Volume: " + RT_dStrapVolume.ToString() + "\n\t");
			Console.WriteLine("Water Volume: " + RT_dWaterVolume.ToString() + "\n\t");
			Console.WriteLine("Gross Volume: " + RT_dGrossVolume.ToString() + "\n\t");
			Console.WriteLine("Density: " + RT_dDensity.ToString() + "\n\t");
			Console.WriteLine("Std Density: " + RT_dStdDensity.ToString() + "\n\t");
			Console.WriteLine("VCF: " + RT_dVCF.ToString() + "\n\t");
			Console.WriteLine("Shell Correction: " + RT_dCTSh.ToString() + "\n\t");
			Console.WriteLine("Net Volume: " + RT_dNetVolume.ToString() + "\n\t");
			Console.WriteLine("Mass: " + RT_dMass.ToString() + "\n\t");
		}

		public void TestGetHandGaugeData()
		{
			int moveID = 2;
			int nodeID = 1;
			int getStartData = 1;
			double RT_dLevel = 0.0;
			long RT_LevelTime = 0;
			double RT_dTemperature = 0.0;
			long RT_TemperatureTime = 0;
			double RT_dDensity = 0.0;
			long RT_DensityTime = 0;
			double RT_dDensityTemp = 0.0;
			long RT_DensityTempTime = 0;
			double RT_dAmbientTemp = 0.0;
			long RT_AmbientTempTime = 0;
			double RT_dRefHeight = 0.0;
			long RT_RefHeightTime = 0;
			double RT_dWaterLevel = 0.0;
			long RT_WaterLevelTime = 0;
			double RT_dStrapVolume = 0.0;
			double RT_dWaterVolume = 0.0;
			double RT_dGrossVolume = 0.0;
			double RT_dStdDensity = 0.0;
			double RT_dVCF = 0.0;
			double RT_dCTSh = 0.0;
			double RT_dNetVolume = 0.0;
			double RT_dMass = 0.0;
			double RT_dRoofMass = 0.0;
			StringBuilder szEmployeeID = new StringBuilder(255);


			int tt = GetHandGaugeDataStartEnd(moveID,
			nodeID,
			getStartData,
			ref RT_dLevel,
			ref RT_LevelTime,
			ref RT_dTemperature,
			ref RT_TemperatureTime,
			ref RT_dDensity,
			ref RT_DensityTime,
			ref RT_dDensityTemp,
			ref RT_DensityTempTime,
			ref RT_dAmbientTemp,
			ref RT_AmbientTempTime,
			ref RT_dRefHeight,
			ref RT_RefHeightTime,
			ref RT_dWaterLevel,
			ref RT_WaterLevelTime,
			ref RT_dStrapVolume,
			ref RT_dWaterVolume,
			ref RT_dGrossVolume,
			ref RT_dStdDensity,
			ref RT_dVCF,
			ref RT_dCTSh,
			ref RT_dNetVolume,
			ref RT_dMass,
			ref RT_dRoofMass,
			szEmployeeID);
			
			if (tt == -1)
			{
				Console.WriteLine("Error GetHandGaugeStartData:\n\t");
				return;
			}

			Console.WriteLine("Success in GetHandGaugeStartData:\n\t");
			Console.WriteLine("Level: " + RT_dLevel.ToString() + " Time: " + RT_LevelTime.ToString());
			Console.WriteLine("Temperature: " + RT_dTemperature.ToString() + " Time: " + RT_TemperatureTime.ToString());

			Console.WriteLine("Density: " + RT_dDensity.ToString() + " Time: " + RT_DensityTime.ToString());

			Console.WriteLine("Density Temp: " + RT_dDensityTemp.ToString() + " Time: " + RT_DensityTempTime.ToString());

			Console.WriteLine("Ambient Temp: " + RT_dAmbientTemp.ToString() + " Time: " + RT_AmbientTempTime.ToString());

			Console.WriteLine("Ref Height: " + RT_dRefHeight.ToString() + " Time: " + RT_RefHeightTime.ToString());

			Console.WriteLine("Water Level: " + RT_dWaterLevel.ToString() + " Time: " + RT_WaterLevelTime.ToString());

			Console.WriteLine("Strap Volume: " + RT_dStrapVolume.ToString());
			Console.WriteLine("Water Volume: " + RT_dWaterVolume.ToString());
			Console.WriteLine("Gross Volume: " + RT_dGrossVolume.ToString());
			Console.WriteLine("Std Density: " + RT_dStdDensity.ToString());
			Console.WriteLine("VCF: " + RT_dVCF.ToString());
			Console.WriteLine("CTSH: " + RT_dCTSh.ToString());
			Console.WriteLine("Net Volume: " + RT_dNetVolume.ToString());
			Console.WriteLine("Mass: " + RT_dMass.ToString());
			Console.WriteLine("Roof Mass: " + RT_dRoofMass.ToString());
			Console.WriteLine("Employee: " + szEmployeeID);
		}
	}
}
