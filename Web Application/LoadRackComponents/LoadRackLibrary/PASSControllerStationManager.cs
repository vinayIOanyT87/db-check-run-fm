/******************************************************************************

	FILE NAME:		PASSControllerStationManager.cs


	PURPOSE:			PASSControllerStationManagerClass


	COMMENTS:

		Copyright (C) E+H Systems & Gauging, Inc. Norcross, GA, USA, 2000

		This file shall not be copied or reproduced in any form without
				the express written consent of Endress+Hauser.


	AUTHOR(S):	W. Gray


	VERSION:		1.0.0  Current version



	MODIFICATION HISTORY:
		Date:			By:			Reason:
		---------	----------  -------------------------------------------

*******************************************************************************/
using System;
using System.ServiceProcess;
using System.Runtime.InteropServices;
using System.Threading;
using System.Diagnostics;
using System.Net;
using Opc;
using Opc.Da;
using OpcCom.Da;

using FMBusinessObjects.BusinessInterfaces;
using FMBusinessObjects.ChannelFactories;
using FMBusinessObjects.DataObjects;
using FMBusinessObjects.Exceptions;
using FMBusinessObjects.ServiceRequests;
using FMBusinessObjects.UtilityObjects;

namespace LoadRackLibrary
{
	/// <summary>
	/// Summary description for PASSControllerStationManagerClass.
	/// </summary>
	public class PASSControllerStationManagerClass  :	StationManagerClass,
																		IDisposable
	{
		protected	ProcessVariableClass				FirstLineDisplayPV;
		protected	ProcessVariableClass				SecondLineDisplayPV;
		protected	ProcessVariableClass				PasswordPV;
		protected	ProcessVariableClass				CardReaderDataPV;
		protected	ProcessVariableClass				KeypadDataPV;
		protected	ProcessVariableClass				ClearListPV;
		protected	ProcessVariableClass				DisplayListPV;
		protected	ProcessVariableClass				SelectListItemPV;
		protected	ProcessVariableClass				WriteListItemPV;
		protected	ProcessVariableClass				SelectedListItemPV;
		protected	ManualResetEvent					PASSControllerKillEvent=null;
		protected	Thread								PASSControllerScanThread=null;
		protected	int									MessageTimer=0;
		protected	bool									Offline=true;

		public PASSControllerStationManagerClass(	EventLog					EventLog,
																LoadRackManagerClass	LoadRackManager,
																StationClass			Station,
																SiteManagerClass		SiteManager,
																SecurityClass			Security)
			: base(EventLog,LoadRackManager,Station,SiteManager,Security)
		{

			this.FirstLineDisplayPV =new ProcessVariableClass(
				PROCESS_VARIABLE_TYPE.DISPLAY_PV,
				UNIT_TYPE.STATION_UNIT,
				VarEnum.VT_BSTR,
				false,
				this.StationPv.OPCItemID+".Write First Line",
				this.StationPv.URL,
				this.StationPv.ProgID
				);

			this.SecondLineDisplayPV =new ProcessVariableClass(
				PROCESS_VARIABLE_TYPE.DISPLAY_PV,
				UNIT_TYPE.STATION_UNIT,
				VarEnum.VT_BSTR,
				false,
				this.StationPv.OPCItemID+".Write Second Line",
				this.StationPv.URL,
				this.StationPv.ProgID
				);

			this.PasswordPV =new ProcessVariableClass(
				PROCESS_VARIABLE_TYPE.PASSWORD_PV,
				UNIT_TYPE.STATION_UNIT,
				VarEnum.VT_BOOL,
				false,
				this.StationPv.OPCItemID+".PIN Display Mode",
				this.StationPv.URL,
				this.StationPv.ProgID
				);

			this.CardReaderDataPV =new ProcessVariableClass(
				PROCESS_VARIABLE_TYPE.CARDREADER_PV,
				UNIT_TYPE.STATION_UNIT,
				VarEnum.VT_BSTR,
				true,
				this.StationPv.OPCItemID+".Card Reader Data",
				this.StationPv.URL,
				this.StationPv.ProgID
				);

			this.KeypadDataPV = new ProcessVariableClass(
				PROCESS_VARIABLE_TYPE.KEYPAD_DATA_PV,
				UNIT_TYPE.STATION_UNIT,
				VarEnum.VT_BSTR,
				true,
				this.StationPv.OPCItemID+".Keypad Data",
				this.StationPv.URL,
				this.StationPv.ProgID
				);

			this.ClearListPV =new ProcessVariableClass(
				PROCESS_VARIABLE_TYPE.CLEAR_LIST_PV,
				UNIT_TYPE.STATION_UNIT,
				VarEnum.VT_EMPTY,
				false,
				this.StationPv.OPCItemID+".Clear List",
				this.StationPv.URL,
				this.StationPv.ProgID
				);

			this.DisplayListPV =new ProcessVariableClass(
				PROCESS_VARIABLE_TYPE.DISPLAY_LIST_PV,
				UNIT_TYPE.STATION_UNIT,
				VarEnum.VT_EMPTY,
				false,
				this.StationPv.OPCItemID+".Display List",
				this.StationPv.URL,
				this.StationPv.ProgID
				);

			this.SelectListItemPV =new ProcessVariableClass(
				PROCESS_VARIABLE_TYPE.SELECT_ITEM_PV,
				UNIT_TYPE.STATION_UNIT,
				VarEnum.VT_BSTR,
				false,
				this.StationPv.OPCItemID+".Select List Item",
				this.StationPv.URL,
				this.StationPv.ProgID
				);

			this.WriteListItemPV =new ProcessVariableClass(
				PROCESS_VARIABLE_TYPE.WRITE_ITEM_PV,
				UNIT_TYPE.STATION_UNIT,
				VarEnum.VT_BSTR,
				false,
				this.StationPv.OPCItemID+".Write List Item",
				this.StationPv.URL,
				this.StationPv.ProgID
				);

			this.SelectedListItemPV =new ProcessVariableClass(
				PROCESS_VARIABLE_TYPE.SELECTED_ITEM_PV,
				UNIT_TYPE.STATION_UNIT,
				VarEnum.VT_BSTR,
				true,
				this.StationPv.OPCItemID+".Selected List Item",
				this.StationPv.URL,
				this.StationPv.ProgID
				);

			// Launch a thread to periodically read CardReader
			ThreadStart PASSControllerScanStart = new ThreadStart(this.PASSControllerScan);

			this.PASSControllerKillEvent =new ManualResetEvent(false);

			this.PASSControllerScanThread = new Thread(PASSControllerScanStart);
			this.PASSControllerScanThread.Start();
			this.PASSControllerScanThread.Priority=ThreadPriority.AboveNormal;
		}

		~PASSControllerStationManagerClass()
		{
			this.Dispose();
		}

		public override void Dispose()
		{
			if(!this.AlreadyDisposed)
			{
				base.Dispose();

				// Terminate the Scan Thread
				if(this.PASSControllerKillEvent != null)
					this.PASSControllerKillEvent.Set();
				if(this.PASSControllerScanThread != null)
					this.PASSControllerScanThread.Join();

				GC.SuppressFinalize(this);
				this.AlreadyDisposed =true;
			}
		}

		public override bool SendEndOfDayOrMonthWarningMessagesDuringLoading{get{return true;}}

		public override int DisplayMessage(string stockMessage, string defaultResponse, int responseLength,int messageTimeout)
		{
			string Message = FMChannelHelper.MakeCall<IDataDictionariesClass, string>(
																	 x =>
																	 x.Get(this.SiteManager.Site.SiteGuid,stockMessage)
																);
			char [] Seperators={' '};
			string [] Strings=Message.Split(Seperators);
			string FirstLine="";
			string SecondLine="";
			foreach(string SubMessage in Strings)
			{
				if(SubMessage.Length > 15)
					throw new Exception("Message To Long for Pass Controller : "+Message);

				if(FirstLine == "")
					FirstLine=SubMessage;
				else if(SecondLine == "")
				{
					if(FirstLine.Length + SubMessage.Length < 15)
						FirstLine=FirstLine+" "+SubMessage;
					else
						SecondLine = SubMessage;
				}
				else if(SecondLine.Length + SubMessage.Length < 15)
					SecondLine=SecondLine+" "+SubMessage;
				else
					throw new Exception("Message To Long for Pass Controller : "+Message);
			}

			FirstLine.PadRight(15,' ');
			this.FirstLineDisplayPV.ServerValue=FirstLine;
			this.OPCServerManager.Write(this.FirstLineDisplayPV);

			SecondLine.PadRight(15,' ');
			this.SecondLineDisplayPV.ServerValue=SecondLine;
			this.OPCServerManager.Write(this.SecondLineDisplayPV);

			this.MessageTimer =messageTimeout*2;

			return 2;
		}

		public override void DisplayMenu(DisplayMenuParameters Parameters)
		{
			this.DisplayMessage(Parameters.Caption,null, 0,Parameters.MenuTimeout);

			int Item=0;
			foreach(string MenuItem in Parameters.Menu)
			{
				this.WriteListItemPV.ServerValue=Item.ToString("X2")+MenuItem;
				this.OPCServerManager.Write(this.WriteListItemPV);
			}
			this.OPCServerManager.Write(this.DisplayListPV);
		}

		protected override void PromptForPin(string stockMessage,int responseLength,int messageTimeout)
		{
			this.DisplayMessage(stockMessage,null, responseLength,messageTimeout);

			// Turn On Password Mode
			this.PasswordPV.ServerValue=true;
			this.OPCServerManager.Write(this.PasswordPV);
		}

		public override void ResetStationDevice()
		{
			base.ResetStationDevice();

			this.LoadArmManagerCollection.ReleaseKeyPad(this);

			if(this.Station.CardReader)
				this.IssuePleaseCardIn();
			else
				this.IssueDriverIDPrompt();
		}

		protected override void OpenGate()
		{
			if(this.GatePV.URL != "")
			{
				this.GatePV.ServerValue=(bool) true;
				this.OPCServerManager.Write(this.GatePV);
				this.DisplayMessage("LoadRack|Opening Gate",null,0,0);
				this.GateTimer =10;
				this.StationState =StationState.OPENING_GATE;
			}
			else
				this.DisplayMessage("LoadRack|Please Card In",null,0,0);
		}

		protected override void Unauthorize()
		{
			base.Unauthorize ();

			this.DisplayMessage("LoadRack|Please Card In",null,0,0);
		}

		public void PASSControllerScan()
		{
			try
			{
				// Instantiate the OptomuxOPCServer and get a Subscription for reading 
				Opc.Da.Server	Server=new Opc.Da.Server(new OpcCom.Factory(),new URL(this.StationPv.URL));
				NetworkCredential Credentials = null;
				Server.Connect(new ConnectData(Credentials));

				// Every 500 msec
				while(!this.PASSControllerKillEvent.WaitOne(500,true))
				{
					Monitor.Enter(this);

					try
					{
						if (this.MessageTimer > 0)
						{
							this.MessageTimer--;
							if (this.MessageTimer == 0)
								this.ProcessMessageTimeout();

						}

						if (this.GateTimer > 0)
						{
							this.GateTimer--;
							if (this.GateTimer == 0)
							{
								this.StationState = StationState.IDLE;
								this.GatePV.ServerValue = (bool)false;
								this.OPCServerManager.Write(this.GatePV);
								if (this.Station.CardReader)
									this.IssuePleaseCardIn();
								else
									this.IssueDriverIDPrompt();
							}
						}

						if (this.StationState == StationState.IDLE
						|| this.StationState == StationState.LOADID_CARD_PROMPT)
						{
							Item[] Items ={new Item(new ItemIdentifier(this.CardReaderDataPV.OPCItemID)),
												};

							ItemValueResult[] Values = Server.Read(Items);
							if (Values[0].Quality.QualityBits == qualityBits.badCommFailure)
								this.Offline = true;
							else
							{
								if (this.Offline)
								{
									this.Offline = false;

									if (this.StationState == StationState.IDLE)
										this.ResetStationDevice();
								}

								else if (Values[0].Quality == Quality.Good)
								{
									if ((string)Values[0].Value != "")
									{
										// Driver Card
										if (this.StationState == StationState.IDLE)
											this.ProcessDriverID((string)Values[0].Value);

										// LoadID Card
										else if (this.StationState == StationState.LOADID_CARD_PROMPT)
										{
											this.ProcessLoadIDCard((string)Values[0].Value);
										}
									}
								}
							}
						}
						else
						{
							Item[] Items ={new Item(new ItemIdentifier(this.KeypadDataPV.OPCItemID)),
												};

							ItemValueResult[] Values = Server.Read(Items);
							if (Values[0].Quality == Quality.Good)
								this.ProcessResponseData((string)Values[0].Value);
						}
					}
					catch (Exception e)
					{
						this.eventLog.WriteEntry(e.ToString(), EventLogEntryType.Error);
					}
					finally
					{
						Monitor.Exit(this);
					}
				}

				Server.Disconnect();
				Server.Dispose();
			}
			catch (Exception e)
			{
				this.eventLog.WriteEntry(e.ToString(),EventLogEntryType.Error);
			}
		}
	}
}