/******************************************************************************

	FILE NAME:		VarecDETStationManager.cs


	PURPOSE:			VarecDETManagerClass


	COMMENTS:

		Copyright (C) E+H Systems & Gauging, Inc. Norcross, GA, USA, 2000

		This file shall not be copied or reproduced in any form without
				the express written consent of Endress+Hauser.


	AUTHOR(S):	W. Gray


	VERSION:		1.0.0  Current version



	MODIFICATION HISTORY:
		Date:			By:			Reason:
		---------	----------  -------------------------------------------
		09/13/2006	W.Gray		7.1.0.15 - Change to IssuePleaseCardIn on MessageTimeout

		09/13/2006	W.Gray		7.0.0.15 - Change to IssuePleaseCardIn on Startup

		02/26/2008	B. Schaal	7.2.0.0 - Modified tare and exit weight scale readings to account for
												scale in motion, no scale in motion capability and failed scale in
												motion sensor. (CSI 5527)

		05/07/2008	C. Knight	7.4.0.0	- Add stack trace dump to DETScan exceptions

		12/08/2008	W.Gray		7.4.6.0 - Corrected DisplayMessage (CSI 6326)

		04/08/2009	W.Gray		7.4.6.1 - Correction to DisplayMessage to develop output lines
										based upon text with rather than length (CSI 2374)
		08/26/2010  C. Knight           WI 16478 - Create transaction and line time once we have meter start - line item will be In Progress
													and will have 0 quantity and no meter stop.
*******************************************************************************/
using System;
using System.Collections;
using System.Drawing;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Net;
using System.Data;
using System.Threading;
using System.Diagnostics;
using Opc;
using Opc.Da;

using FMBusinessObjects.BusinessInterfaces;
using FMBusinessObjects.ChannelFactories;
using FMBusinessObjects.DataObjects;
using FMBusinessObjects.ServiceRequests;
using FMBusinessObjects.UtilityObjects;

namespace LoadRackLibrary
{
	using FMBusinessObjects.Interfaces;

	/// <summary>
	/// Summary description for VarecDETStationManagerClass.
	/// </summary>
	public class VarecDETStationManagerClass : StationManagerClass
	{
		const int MAX_LINES = 8;
		protected ProcessVariableClass[] WriteLineDisplayPV = new ProcessVariableClass[MAX_LINES];
		protected ProcessVariableClass PasswordPV;
		protected ProcessVariableClass CardReaderDataPV;
		protected ProcessVariableClass KeypadDataPV;
		protected ProcessVariableClass ClearListPV;
		protected ProcessVariableClass DisplayListPV;
		protected ProcessVariableClass SelectListItemPV;
		protected ProcessVariableClass WriteListItemPV;
		protected ProcessVariableClass SelectedListItemPV;
		protected ProcessVariableClass WriteStatusLinePV;
		protected ProcessVariableClass WriteResponseLinePV;

		protected ManualResetEvent DETKillEvent;
		protected Thread DETScanThread;
		protected int MessageTimer;
		protected bool Offline = true;
		new protected double OffLoadPresetAmount;
		protected double StartMeterValue;
		protected double StopMeterValue;
		protected string SelectedProductID = "";

		private LineItemDO currentLineItem;

		public VarecDETStationManagerClass(EventLog eventLog,
			LoadRackManagerClass loadRackManager,
			StationClass station,
			SiteManagerClass siteManager,
			SecurityClass security)
			: base(eventLog, loadRackManager, station, siteManager, security)
		{
			string[] LineItemID ={  "Write First Line",
									"Write Second Line",
									"Write Third Line",
									"Write Forth Line",
									"Write Fifth Line",
									"Write Sixth Line",
									"Write Seventh Line",
									"Write Eighth Line"};

			for (int Index = 0; Index < MAX_LINES; Index++)
				this.WriteLineDisplayPV[Index] = new ProcessVariableClass(PROCESS_VARIABLE_TYPE.DISPLAY_PV,
																  UNIT_TYPE.STATION_UNIT,
																  VarEnum.VT_BSTR,
																  false,
																  this.StationPv.OPCItemID + "." + LineItemID[Index],
																  this.StationPv.URL,
																  this.StationPv.ProgID);

			this.PasswordPV = new ProcessVariableClass(
			  PROCESS_VARIABLE_TYPE.PASSWORD_PV,
			  UNIT_TYPE.STATION_UNIT,
			  VarEnum.VT_BOOL,
			  false,
			  this.StationPv.OPCItemID + ".PIN Display Mode",
			  this.StationPv.URL,
			  this.StationPv.ProgID
			  );

			this.CardReaderDataPV = new ProcessVariableClass(
			  PROCESS_VARIABLE_TYPE.CARDREADER_PV,
			  UNIT_TYPE.STATION_UNIT,
			  VarEnum.VT_BSTR,
			  true,
			  this.StationPv.OPCItemID + ".Card Reader Data",
			  this.StationPv.URL,
			  this.StationPv.ProgID
			  );

			this.KeypadDataPV = new ProcessVariableClass(
			  PROCESS_VARIABLE_TYPE.KEYPAD_DATA_PV,
			  UNIT_TYPE.STATION_UNIT,
			  VarEnum.VT_BSTR,
			  true,
			  this.StationPv.OPCItemID + ".Keypad Data",
			  this.StationPv.URL,
			  this.StationPv.ProgID
			  );

			this.ClearListPV = new ProcessVariableClass(
			  PROCESS_VARIABLE_TYPE.CLEAR_LIST_PV,
			  UNIT_TYPE.STATION_UNIT,
			  VarEnum.VT_EMPTY,
			  false,
			  this.StationPv.OPCItemID + ".Clear List",
			  this.StationPv.URL,
			  this.StationPv.ProgID
			  );

			this.DisplayListPV = new ProcessVariableClass(
			  PROCESS_VARIABLE_TYPE.DISPLAY_LIST_PV,
			  UNIT_TYPE.STATION_UNIT,
			  VarEnum.VT_EMPTY,
			  false,
			  this.StationPv.OPCItemID + ".Display List",
			  this.StationPv.URL,
			  this.StationPv.ProgID
			  );

			this.SelectListItemPV = new ProcessVariableClass(
			  PROCESS_VARIABLE_TYPE.SELECT_ITEM_PV,
			  UNIT_TYPE.STATION_UNIT,
			  VarEnum.VT_BSTR,
			  false,
			  this.StationPv.OPCItemID + ".Select List Item",
			  this.StationPv.URL,
			  this.StationPv.ProgID
			  );

			this.WriteListItemPV = new ProcessVariableClass(
			  PROCESS_VARIABLE_TYPE.WRITE_ITEM_PV,
			  UNIT_TYPE.STATION_UNIT,
			  VarEnum.VT_BSTR,
			  false,
			  this.StationPv.OPCItemID + ".Write List Item",
			  this.StationPv.URL,
			  this.StationPv.ProgID
			  );

			this.SelectedListItemPV = new ProcessVariableClass(
			  PROCESS_VARIABLE_TYPE.SELECTED_ITEM_PV,
			  UNIT_TYPE.STATION_UNIT,
			  VarEnum.VT_BSTR,
			  true,
			  this.StationPv.OPCItemID + ".Selected List Item",
			  this.StationPv.URL,
			  this.StationPv.ProgID
			  );

			this.WriteStatusLinePV = new ProcessVariableClass(
			  PROCESS_VARIABLE_TYPE.DISPLAY_PV,
			  UNIT_TYPE.STATION_UNIT,
			  VarEnum.VT_BSTR,
			  false,
			  this.StationPv.OPCItemID + "." + "Write Status Line",
			  this.StationPv.URL,
			  this.StationPv.ProgID);

			this.WriteResponseLinePV = new ProcessVariableClass(
			  PROCESS_VARIABLE_TYPE.DISPLAY_PV,
			  UNIT_TYPE.STATION_UNIT,
			  VarEnum.VT_BSTR,
			  false,
			  this.StationPv.OPCItemID + "." + "Write Response Line",
			  this.StationPv.URL,
			  this.StationPv.ProgID);


			// Launch a thread to periodically read CardReader/KeyPad/SelectedListItem
			ThreadStart DETScanStart = new ThreadStart(this.DetScan);
			this.DETKillEvent = new ManualResetEvent(false);
			this.DETScanThread = new Thread(DETScanStart);
			this.DETScanThread.Start();
			this.DETScanThread.Priority = ThreadPriority.AboveNormal;
		}

		~VarecDETStationManagerClass()
		{
			this.Dispose();
		}
		private bool UseManualMeter
		{
			get
			{
				if (this.Station.UseManualMeterData)
				{
					return true;
				}

				// Note that we assume that there may only be one load arm manager per station manager
				// This is a cheat which only works for as long as the DET is limited to supporting one arm
				foreach (VarecDETLoadArmManagerClass loadArmManager in this.LoadArmManagerCollection)
				{
					if (loadArmManager.CurrentLineItemRecipe == null)
					{
						continue;
					}

					foreach (ProcessVariableClass processVariable in loadArmManager.CurrentLineItemRecipe.ProcessVariableCollection)
					{
						if (processVariable.ProcessVariableType != PROCESS_VARIABLE_TYPE.COMPONENT_METER_FLOW_TOTAL_PV)
						{
							continue;
						}

						if (processVariable.OPCConnectionGuid.IsEmpty() || string.IsNullOrEmpty(processVariable.OPCItemID))
						{
							return true;
						}
					}
				}

				return false;
			}
		}

		public override void Dispose()
		{
			if (!this.AlreadyDisposed)
			{
				base.Dispose();

				// Terminate the Scan Thread
				this.DETKillEvent?.Set();
				this.DETScanThread?.Join();

				GC.SuppressFinalize(this);

				this.AlreadyDisposed = true;
			}
		}


		public override void ResetStationDevice()
		{
			base.ResetStationDevice();

			this.LoadArmManagerCollection.ReleaseKeyPad(this);

			foreach (VarecDETLoadArmManagerClass loadArmManager in this.LoadArmManagerCollection)
			{
				this.UpdateComponentPermissives(loadArmManager.CurrentLineItemRecipe, false);
			}

			if (this.Station.CardReader)
			{
				this.IssuePleaseCardIn();
			}
			else
			{
				this.IssueDriverIDPrompt();
			}
		}

		public override void CancelUnauthorizedTransaction()
		{
			if (this.IsTransactionInProgress())
			{
				try
				{
					this.eventLog.WriteEntry("Unauthorized Transaction in Progress: Station " + this.Station.ID, EventLogEntryType.Error);
					this.LoadArmManagerCollection.Stop(this);
					this.SendEndTransaction();
					this.ResetStationDevice();
				}
				catch (Exception e)
				{
					this.eventLog.WriteEntry("Varec DET StationManager CancelUnauthorizedTransaciton : " + e.Message, EventLogEntryType.Error);
				}
			}
		}

		public override bool SendEndOfDayOrMonthWarningMessagesDuringLoading { get { return true; } }

		// Output upto 8 60 character lines of text provided no single string is greater than 40 characters
		public override int DisplayMessage(string stockMessage, string defaultResponse, int responseLength, int messageTimeout, bool SaveForCancelProcessing)
		{
			string Message = FMChannelHelper.MakeCall<IDataDictionariesClass, string>(
																	 x =>
																	 x.Get(this.SiteManager.Site.SiteGuid, stockMessage)
																);
			char[] Seperators = { ' ' };
			string[] Strings = Message.Split(Seperators);
			string[] Lines = new string[8];
			int LineIndex = 0;

			using (
				Font detNtFont = new Font(
					"Arial", 18F, FontStyle.Bold, GraphicsUnit.Point, ((byte)(0))),
					  detCeFont = new Font(
						  "Arial", 14F, FontStyle.Bold, GraphicsUnit.Point, ((byte)(0))))
			{
				Form detForm = new Form();
				Graphics detGraphics = Graphics.FromHwnd(detForm.Handle);

				float detNtWidth = 764;
				float detCeWidth = 601;

				foreach (string SubMessage in Strings)
				{
					float subMessageNtWidth = detGraphics.MeasureString(SubMessage, detNtFont).Width;
					float subMessageCeWidth = detGraphics.MeasureString(SubMessage, detCeFont).Width;

					if (subMessageNtWidth > detNtWidth || subMessageCeWidth > detCeWidth) break;

					if (Lines[LineIndex] == null) Lines[LineIndex] = SubMessage;

					else if (LineIndex == Lines.Length - 1 || Lines[LineIndex + 1] == null)
					{
						subMessageNtWidth = detGraphics.MeasureString(Lines[LineIndex] + " " + SubMessage, detNtFont).Width;
						subMessageCeWidth = detGraphics.MeasureString(Lines[LineIndex] + " " + SubMessage, detCeFont).Width;

						if (subMessageNtWidth < detNtWidth && subMessageCeWidth < detCeWidth) Lines[LineIndex] = Lines[LineIndex] + " " + SubMessage;
						else
						{
							LineIndex++;
							if (LineIndex > Lines.Length - 1) break;

							Lines[LineIndex] = SubMessage;
						}
					}

					else
					{
						LineIndex++;
						subMessageNtWidth = detGraphics.MeasureString(Lines[LineIndex] + " " + SubMessage, detNtFont).Width;
						subMessageCeWidth = detGraphics.MeasureString(Lines[LineIndex] + " " + SubMessage, detCeFont).Width;

						if (subMessageNtWidth < detNtWidth && subMessageCeWidth < detCeWidth) Lines[LineIndex] = Lines[LineIndex] + " " + SubMessage;
						else break;
					}
				}

				detGraphics.Dispose();
				detForm.Dispose();
			}

			ArrayList ItemValues = new ArrayList();

			ItemValue ClearList = new ItemValue(this.ClearListPV.OPCItemID);
			ItemValues.Add(ClearList);

			LineIndex = 0;
			foreach (string Line in Lines)
			{
				if (Line == null)
					break;

				ItemValue WriteLine = new ItemValue(this.WriteLineDisplayPV[LineIndex].OPCItemID)
				{
					Value = Line
				};
				ItemValues.Add(WriteLine);
				LineIndex++;
			}

			// Turn Off Password Mode which arms input
			if (responseLength > 0)
			{
				ItemValue ResponseData = new ItemValue(this.WriteResponseLinePV.OPCItemID)
				{
					Value = defaultResponse ?? ""
				};
				ItemValues.Add(ResponseData);
			}

			this.OPCServerManager.Write(new URL(this.ClearListPV.URL), (ItemValue[])ItemValues.ToArray(typeof(ItemValue)));

			this.MessageTimer = messageTimeout * 2;

			return LineIndex;
		}

		public override void DisplayMenu(DisplayMenuParameters Parameters)
		{
			this.DisplayMessage(Parameters.Caption, null, 0, Parameters.MenuTimeout);

			ArrayList ItemValues = new ArrayList();

			int Item = 0;
			foreach (string MenuItem in Parameters.Menu)
			{
				ItemValue WriteListItem = new ItemValue(this.WriteListItemPV.OPCItemID);
				if (Parameters.ApplyDataDictionary)
				{
					WriteListItem.Value = Item.ToString("X2") + this.GetDataDictionaryValueByKey(this.SiteManager.Site.SiteGuid, MenuItem);
				}
				else
				{
					WriteListItem.Value = Item.ToString("X2") + MenuItem;
				}

				ItemValues.Add(WriteListItem);
				Item++;
				if (Item > 255)
				{
					ItemValue WriteStatus = new ItemValue(this.WriteStatusLinePV.OPCItemID)
					{
						Value = this.GetDataDictionaryValueByKey(this.SiteManager.Site.SiteGuid, "LoadRack|Maximum 255 Items Exceeded")
					};
					ItemValues.Add(WriteStatus);
					break;
				}
			}

			ItemValue SelectListItem = new ItemValue(this.SelectListItemPV.OPCItemID)
			{
				Value = Parameters.DefaultItem
			};
			ItemValues.Add(SelectListItem);
			ItemValue DisplayList = new ItemValue(this.DisplayListPV.OPCItemID);
			ItemValues.Add(DisplayList);

			this.OPCServerManager.Write(new URL(this.DisplayListPV.URL), (ItemValue[])ItemValues.ToArray(typeof(ItemValue)));
		}

		protected override void PromptForPin(string stockMessage, int responseLength, int messageTimeout)
		{
			this.DisplayMessage(stockMessage, null, 0, messageTimeout);

			// Turn On Password Mode
			this.PasswordPV.ServerValue = true;
			this.OPCServerManager.Write(this.PasswordPV);
		}

		protected override void OpenGate()
		{
			this.IssueSelectGate();
		}

		private void opengate()
		{
			if (string.IsNullOrEmpty(this.GatePV.URL) == false)
			{
				try
				{
					this.GatePV.ServerValue = true;
					this.OPCServerManager.Write(this.GatePV);
					this.StationState = StationState.OPENING_GATE;
					this.DisplayMessage("LoadRack|Opening Gate", null, 0, this.MESSAGE_TIMEOUT);
					this.GateTimer = 10;
				}
				catch (Exception e)
				{
					this.eventLog.WriteEntry(e.ToString(), EventLogEntryType.Error);
					this.StationState = StationState.RESET_ON_TIMEOUT;
					this.DisplayMessage("LoadRack|Gate Open Failure", null, 0, this.MESSAGE_TIMEOUT);
				}
			}
			else
			{
				this.ResetStationDevice();
			}
		}

		protected void IssueSelectGate()
		{
			// Build initial menu parameter set
			var parameters = new DisplayMenuParameters
			{
				ApplyDataDictionary = true,
				DefaultItem = 0,
				MenuTimeout = this.PROMPT_TIMEOUT,
				Caption = "[LoadRack|Select Gate]",
			};
			var menu = new ArrayList();
			int item = 0;

			foreach (ProcessVariableClass processVariable in this.Station.ProcessVariableCollection)
			{
				if (processVariable.ProcessVariableType != PROCESS_VARIABLE_TYPE.GATE_CONTROL_PV)
				{
					continue;
				}

				if (string.IsNullOrEmpty(processVariable.URL))
				{
					continue;
				}

				item++;

				if (string.IsNullOrEmpty(processVariable.MessageID))
				{
					processVariable.MessageID = "Gate " + item.ToString();
				}
				menu.Add(processVariable.MessageID);
			}

			if (item == 1)
			{
				this.opengate();
				return;
			}
			if (item == 0)
			{
				this.DisplayMessage("LoadRack|Gate Open Failure", null, 0, this.MESSAGE_TIMEOUT);
				return;
			}
			this.StationState = StationState.SELECT_GATE;
			parameters.Menu = (string[])menu.ToArray(typeof(string));
			this.DisplayMenu(parameters);
		}

		protected override void ProcessSelectGate(string response)
		{
			if (EscapeString == response || response == "")
			{
				if (this.Station.CardReader)
				{
					this.IssuePleaseCardIn();
				}
				else if (this.Station.TouchKeyReader)
				{
					this.IssueTouchKeyPleaseCardIn();
				}
				else
				{
					this.IssueDriverIDPrompt();
				}
			}
			else
			{

				foreach (ProcessVariableClass ProcessVariable in this.Station.ProcessVariableCollection)
				{
					if (ProcessVariable.ProcessVariableType != PROCESS_VARIABLE_TYPE.GATE_CONTROL_PV)
					{
						continue;
					}
					else if (ProcessVariable.MessageID == response)
					{
						this.GatePV = ProcessVariable;
						this.opengate();
						return;
					}
				}
			}
		}

		protected override void Unauthorize()
		{
			base.Unauthorize();
		}

		public override void ReadWeight(out ItemValueResult Weight, out bool WeightScaleInMotion, out bool WeightScaleMotionReadingInValid)
		{
			WeightScaleInMotion = false;
			WeightScaleMotionReadingInValid = false;
			foreach (ProcessVariableClass PV in this.Station.ProcessVariableCollection)
			{
				if (PV.ProcessVariableType == PROCESS_VARIABLE_TYPE.WEIGHT_SCALE_PV)
				{
					string TagPrefix = PV.OPCItemID + ".";

					Item[] Items = { new Item(new ItemIdentifier(TagPrefix + "Weight")) };
					Item[] ItemMotions = { new Item(new ItemIdentifier(TagPrefix + "Scale In Motion")) };

					try
					{
						Opc.Da.Server Server = new Opc.Da.Server(new OpcCom.Factory(), new URL(PV.URL));
						NetworkCredential Credentials = null;
						Server.Connect(new ConnectData(Credentials));

						ItemValueResult[] Values = Server.Read(Items);
						ItemValueResult[] MotionValue = Server.Read(ItemMotions);

						Server.Disconnect();
						Server.Dispose();

						Weight = Values[0];

						if (MotionValue[0].ResultID == ResultID.S_OK)
						{
							if (MotionValue[0].Quality == Quality.Good)
								WeightScaleInMotion = System.Convert.ToBoolean(MotionValue[0].Value);
							else
								WeightScaleMotionReadingInValid = true;
						}
					}
					catch
					{
						Weight = new ItemValueResult(new ItemIdentifier(TagPrefix + "Weight"), new ResultID(Quality.Bad.GetCode()));
						this.eventLog.WriteEntry("Read : Connection error to " + PV.URL, EventLogEntryType.Error);
					}
					return;
				}
			}

			Weight = new ItemValueResult();
		}


		public void DetScan()
		{
			Opc.Da.Server Server = null;

			// Every 500 msec
			while (!this.DETKillEvent.WaitOne(500, true))
			{

				try
				{

					// Instantiate the OptomuxOPCServer 
					if (Server == null)
					{
						Server = new Opc.Da.Server(new OpcCom.Factory(), new URL(this.StationPv.URL));
						NetworkCredential Credentials = null;
						Server.Connect(new ConnectData(Credentials));
					}

					Monitor.Enter(this);

					try
					{
						if (this.MessageTimer > 0)
						{
							this.MessageTimer--;
							if (this.MessageTimer == 0)
							{
								this.ProcessMessageTimeout();
								if (this.StationState == StationState.IDLE)
								{
									if (!this.Station.CardReader)
									{
										this.IssueDriverIDPrompt();
									}
									else
									{
										this.IssuePleaseCardIn();
									}
								}
							}
						}

						if (this.GateTimer > 0)
						{
							this.GateTimer--;
							if (this.GateTimer == 0)
							{
								this.StationState = StationState.IDLE;
								this.GatePV.ServerValue = false;
								this.OPCServerManager.Write(this.GatePV);
								if (this.Station.CardReader)
								{
									this.IssuePleaseCardIn();
								}
								else
								{
									this.IssueDriverIDPrompt();
								}
							}
						}

						if (this.Station.CardReader
						&& (this.StationState == StationState.IDLE
						|| this.StationState == StationState.LOADID_CARD_PROMPT))
						{
							ItemValueResult[] Values = Server.Read(new Item[] { new Item(new ItemIdentifier(this.CardReaderDataPV.OPCItemID)) });
							if (Values[0].Quality.QualityBits == qualityBits.badCommFailure) this.Offline = true;
							else
							{
								if (this.Offline)
								{
									this.Offline = false;

									if (this.StationState == StationState.IDLE)
									{
										if (!this.Station.CardReader)
										{
											this.IssueDriverIDPrompt();
										}
										else
										{
											this.IssuePleaseCardIn();
										}
									}
								}

								else if (Values[0].Quality == Quality.Good)
								{
									if (string.IsNullOrEmpty((string)Values[0].Value) == false)
									{
										// Driver Card
										if (this.StationState == StationState.IDLE) this.ProcessDriverID((string)Values[0].Value);

										// LoadID Card
										else if (this.StationState == StationState.LOADID_CARD_PROMPT) this.ProcessLoadIDCard((string)Values[0].Value);
									}
								}
							}
						}

						else if (this.StationState == StationState.OPERATING_MODE_PROMPT
						|| this.StationState == StationState.USE_ORDER_PROMPT
						|| this.StationState == StationState.SELECT_CUSTOMER_SHIPTO_FILTER_PROMPT
						|| this.StationState == StationState.SELECT_CUSTOMER_SHIPTO_FILTER_VALUE_PROMPT
						|| this.StationState == StationState.SELECT_CUSTOMER_SHIPTO_PROMPT
						|| this.StationState == StationState.SELECT_COMPANY_HIERARCHY_PROMPT
						|| this.StationState == StationState.SELECT_ORDER_PROMPT
						|| this.StationState == StationState.SUMMARY_PROMPT
						|| this.StationState == StationState.COMPARTMENT_SUMMARY_PROMPT
						|| this.StationState == StationState.PRODUCT_PROMPT
						|| this.StationState == StationState.CAPTURE_TARE_WEIGHT_PROMPT
						|| this.StationState == StationState.ADDITIONAL_ORDERS_PROMPT
						|| this.StationState == StationState.CAPTURE_EXIT_WEIGHT_PROMPT
						|| this.StationState == StationState.ENTER_1ST_TRAILER_PROMPT
						|| this.StationState == StationState.ENTER_2ND_TRAILER_PROMPT
						|| this.StationState == StationState.SELECT_TRACTOR_PROMPT
						|| this.StationState == StationState.SELECT_TRAILER1_PROMPT
						|| this.StationState == StationState.SELECT_TRAILER2_PROMPT
						|| this.StationState == StationState.VERIFY_SHIPTO_MSG
						|| this.StationState == StationState.SELECT_SHIPMENT_LOADID_PROMPT
						|| this.StationState == StationState.VERIFY_SHIPTO_MSG_PRELOAD
						|| this.StationState == StationState.CONTAMINATION_PROMPT
						|| this.StationState == StationState.COMPARTMENTS_PREVIOUSLY_LOADED_PROMPT
						|| this.StationState == StationState.COMPARTMENTS_EMPTY_PROMPT
						|| this.StationState == StationState.MANUAL_OFFLOAD_INPROGRESS
						|| this.StationState == StationState.SELECT_OFFLOAD_PRODUCT
						|| this.StationState == StationState.VERIFY_OFFLOAD_SUPPLIER
						|| this.StationState == StationState.PROMPT_FOR_OFFLOAD_COMPLETE
						|| this.StationState == StationState.USE_SUPPLYORDER_PROMPT
						|| this.StationState == StationState.ENTER_SUPPLY_ORDER_NUMBER_LIST
						|| this.StationState == StationState.SELECT_SUPPLIER_OFFLOADID_FILTER_PROMPT
						|| this.StationState == StationState.SELECT_SUPPLIER_PROMPT
						|| this.StationState == StationState.SELECT_SUPPLIER_FILTER_VALUE_PROMPT
								|| this.StationState == StationState.SELECT_DRIVER_COMPANY
								|| this.StationState == StationState.SELECT_DESTINATION_SUPPLIER_PROMPT
								|| this.StationState == StationState.SELECT_GATE
								|| this.StationState == StationState.LINEITEM_SUMMARY_PROMPT)
						{
							ItemValueResult[] Values = Server.Read(new Item[] { new Item(new ItemIdentifier(this.SelectedListItemPV.OPCItemID)) });

							if (Values[0].Quality.QualityBits == qualityBits.badCommFailure) this.Offline = true;
							else
							{
								if (this.Offline)
								{
									this.Offline = false;

									if (!this.Station.CardReader) this.IssueDriverIDPrompt();
									else this.IssuePleaseCardIn();
								}

								if (Values[0].Quality == Quality.Good) this.ProcessResponseData((string)Values[0].Value);
							}
						}
						else if (
                               this.StationState == StationState.TRANSACTION_IN_PROGRESS // no display
							|| this.StationState == StationState.OPENING_GATE // no display or response length 0
							|| this.StationState == StationState.INVALID_PRELOAD_ORDER_SELECTION_MSG // response length 0
                            || this.StationState == StationState.INVALID_PRELOAD_TYPE_SELECTION_MSG // response length 0
                            || this.StationState == StationState.INVALID_PRELOAD_LOADID_SELECTION_MSG // response length 0
                            || this.StationState == StationState.INVALID_PRELOAD_DOCUMENT_SELECTION_MSG // response length 0
                            || this.StationState == StationState.COMPANY_INVALID // response length 0
                            || this.StationState == StationState.RESET_ON_TIMEOUT // response length 0
                            || this.StationState == StationState.INVALID_COMPANY_ON_ORDER // no display
                            || this.StationState == StationState.NO_SHOPTO_MSG // response length 0
                            || this.StationState == StationState.TRANSACTION_ALIAS_INVALID_MSG // response length 0
                            || this.StationState == StationState.CARRIER_INVALID // response length 0
                            || this.StationState == StationState.NO_PRODUCTS_MSG // response length 0
                            || this.StationState == StationState.UPDATE_DENSITY_ERROR_MSG // no display
                            || this.StationState == StationState.NO_PIDX_AUTHORIZATION_MSG // no display
                            || this.StationState == StationState.INVALID_SHIPTO_PROMPT_RESPONSE_MESSAGE // response length 0
                            || this.StationState == StationState.INVALID_ENTER_TRAILER1_PROMPT_RESPONSE_MESSAGE // response length 0
                            || this.StationState == StationState.INVALID_ENTER_TRAILER2_PROMPT_RESPONSE_MESSAGE // response length 0
                            || this.StationState == StationState.CHECK_PIDX_AUTHORIZATIONS // no display
                            || this.StationState == StationState.MANUAL_OFFLOAD_INPROGRESS // no display
                            || this.StationState == StationState.INVALID_SUPPLIER_PROMPT_RESPONSE_MESSAGE // response length 0
                            || this.StationState == StationState.INVALID_OFFLOAD_COMPLETE_TYPE_SELECTION_MSG // response length 0
                            || this.StationState == StationState.NO_SUPPLIER_MSG // response length 0
                            || this.StationState == StationState.INVALID_ENTER_TRAILER3_PROMPT_RESPONSE_MESSAGE // response length 0
                            )
                        { 
							// do nothing, wait for another state change
						}
						else
						{
							ItemValueResult[] Values = Server.Read(new Item[] { new Item(new ItemIdentifier(this.KeypadDataPV.OPCItemID)) });
							if (Values[0].Quality.QualityBits == qualityBits.badCommFailure) this.Offline = true;
							else
							{
								if (this.Offline)
								{
									this.Offline = false;

									if (!this.Station.CardReader) this.IssueDriverIDPrompt();
									else this.IssuePleaseCardIn();

								}

								if (Values[0].Quality == Quality.Good)
								{
									this.ProcessResponseData((string)Values[0].Value);
								}
							}
						}
					}

					catch (Exception e)
					{
						try
						{
							if (Server != null)
							{
								Server.Disconnect();
								Server.Dispose();
								Server = null;
							}
						}
						catch (Exception ie)
						{
							this.eventLog.WriteEntry(ie.ToString(), EventLogEntryType.Error);
						}

						this.eventLog.WriteEntry("VarecDETStationManager DETScan : " + e.TargetSite + ":" + e.Message + " : " + this.StationState.ToString() + "\n" + e.StackTrace, EventLogEntryType.Error);
						if (this.StationState != StationState.IDLE)
						{
							this.StationState = StationState.RESET_ON_TIMEOUT;
							this.DisplayMessage("LoadRack|OPC IO Error Process Terminated", null, 0, this.MESSAGE_TIMEOUT);
						}
					}
					finally
					{
						Monitor.Exit(this);
					}
				}
				catch (Exception e)
				{
					this.eventLog.WriteEntry(e.ToString() + e.TargetSite + ":" + e.Message + "\n" + e.StackTrace, EventLogEntryType.Error);
				}

			}

			try
			{
				if (Server != null)
				{
					Server.Disconnect();
					Server.Dispose();
					Server = null;
				}
			}
			catch (Exception e)
			{
				this.eventLog.WriteEntry(e.ToString(), EventLogEntryType.Error);
			}
		}

		protected override void ResetCardReaderData()
		{
			if (!this.Station.CardReader)
			{
				this.IssueDriverIDPrompt();
			}
			else
			{
				this.IssuePleaseCardIn();
			}
		}

		protected override void ProcessUnloadDensity(string Response)
		{
			if (Response == EscapeString)
			{
				if (this.Station.PromptForBOLNumber == true)
				{
					this.PromptForBOLNumber();
				}
				else if (this.Station.OffLoadByOffLoadID == true || this.UseOffLoadSupplyOrders == false)
				{
					this.DisplayOffLoadProductSelect();
				}
				else
				{
					this.DisplayVerifySupplyOrderProduct();
				}
				return;
			}
			else if (string.IsNullOrEmpty(Response) == true)
			{
				this.PromptForOffLoadDensity();
				return;
			}

			this.OffloadDensity = System.Convert.ToDouble(Response);

			this.PromptForOffLoadTemperature();
		}

		protected override void ProcessUnloadAmount(string response)
		{
			if (response == EscapeString)
			{
				if (this.Station.SynchronizeReferenceDensity == true)
				{
					this.PromptForOffLoadDensity();
				}
				else
				{
					if (this.Station.PromptForBOLNumber == true)
					{
						this.PromptForBOLNumber();
					}
					else if (this.Station.OffLoadByOffLoadID == true || this.UseOffLoadSupplyOrders == false)
					{
						this.DisplayOffLoadProductSelect();
					}
					else
					{
						this.DisplayVerifySupplyOrderProduct();
					}
				}
				return;
			}

			// verify permissive inputs
			string permissiveMessage = (this.LoadArmManagerCollection.Item(0) as VarecDETLoadArmManagerClass)?.RetrievePermissiveMessage(this) ?? "[LoadRack|Unable to verify permissives]";
			if (!string.IsNullOrEmpty(permissiveMessage))
			{
				if (this.ConsecutivePrompts >= this.SiteManager.Site._MaximumPrompts)
				{
					this.StationState = StationState.RESET_ON_TIMEOUT;
					this.DisplayMessage("LoadRack|Max Retries Exceeded", null, 0, this.MESSAGE_TIMEOUT);
					this.ConsecutivePrompts = 0;
					return;
				}
				if (this.StationState != StationState.AUTHORIZED_PERMISSIVE_PROMPT)
				{
					this.StationState = StationState.RESET_ON_TIMEOUT;
					this.DisplayMessage(permissiveMessage, null, 0, this.MESSAGE_TIMEOUT);
				}
				return;
			}

			this.StartDateTime = this.LastActivityDateTime = DateTimeOffset.Now;

			try
			{
				this.OffLoadPresetAmount = System.Convert.ToDouble(response);
			}
			catch (FormatException)
			{
				this.DisplayMessage("LoadRack|Invalid value entered for Quantity", null, 10, this.PROMPT_TIMEOUT);
				return;
			}
			catch (OverflowException)
			{
				this.DisplayMessage("LoadRack|Invalid value entered for Quantity", null, 10, this.PROMPT_TIMEOUT);
				return;
			}

			if (this.SupplyOrder != null)
			{
				// Check that supply order has sufficient quantity available.
				bool orderQuantityAvailable = false;

				foreach (object lineItemObject in this.SupplyOrder.LineItems)
				{
					var lineItem = lineItemObject as LineItemDO;
					if (lineItem == null)
					{
						continue;
					}

					if (lineItem.Product != this.SelectedProductID)
					{
						continue;
					}

					if (lineItem.NetQuantityRemaining >= this.OffLoadPresetAmount)
					{
						// Found a supply order line item with sufficient remaining product left.
						orderQuantityAvailable = true;
						break;
					}
				}

				if (orderQuantityAvailable == false)
				{
					this.DisplayMessage(
						 "LoadRack|Insufficient quantity left on this supply order", null, 10, this.PROMPT_TIMEOUT);
					return;
				}
			}

			// if setup for manual then prompt
			if (this.UseManualMeter)
			{
				this.StationState = StationState.ENTER_MANUAL_METER_START_VALUE;
				this.DisplayMessage("[LoadRack|Enter] [LoadRack|Meter Start Value]", null, 10, this.PROMPT_TIMEOUT);
			}
			else
			{
				this.ProcessManualMeterStartData("0");
			}
			// write permissive outputs

		}

		protected override void ProcessManualMeterStartData(string response)
		{
			if (response == EscapeString)
			{
				this.UpdatePermissives(false);

				foreach (VarecDETLoadArmManagerClass loadArmManager in this.LoadArmManagerCollection)
				{
					this.UpdateComponentPermissives(loadArmManager.CurrentLineItemRecipe, false);
				}

				this.StationState = StationState.ENTER_UNLOAD_AMOUNT;
				this.DisplayMessage("[LoadRack|Enter] [LoadRack|Qty On BOL]", null, 10, this.PROMPT_TIMEOUT);
				return;
			}
			if (this.UseManualMeter)
			{
				try
				{
					this.StartMeterValue = System.Convert.ToDouble(response);
				}
				catch (FormatException)
				{
					//PriorStationState = StationState;
					//StationState = STATION_STATE.INVALID_MANUAL_METER_START_VALUE;
					this.DisplayMessage("LoadRack|Invalid value entered for Meter Start", null, 10, this.PROMPT_TIMEOUT);
					return;
				}
				catch (OverflowException)
				{
					//PriorStationState = StationState;
					//StationState = STATION_STATE.INVALID_MANUAL_METER_START_VALUE;
					this.DisplayMessage("LoadRack|Invalid value entered for Meter Start", null, 10, this.PROMPT_TIMEOUT);
					return;
				}
			}
			else
			{
				this.StartMeterValue = this.ReadConfiguredMeterValue();
			}

			if (this.StartMeterValue < 0)
				return;

			// set the output permissives
			this.UpdatePermissives(true);

			foreach (VarecDETLoadArmManagerClass loadArmManager in this.LoadArmManagerCollection)
			{
				this.UpdateComponentPermissives(loadArmManager.CurrentLineItemRecipe, true);
			}

			if (this.StationState == StationState.RESET_ON_TIMEOUT)
			{
				// turn anything off that we may of turned on
				this.UpdatePermissives(false);

				foreach (VarecDETLoadArmManagerClass loadArmManager in this.LoadArmManagerCollection)
				{
					this.UpdateComponentPermissives(loadArmManager.CurrentLineItemRecipe, false);
				}

				return;
			}

			// Build initial menu parameter set
			var parameters = new DisplayMenuParameters
			{
				ApplyDataDictionary = true,
				DefaultItem = 0,
				MenuTimeout = this.SiteManager.Site._MaximumIdleTime * 60, // Maximum load time is in minutes, but prompt timeout is in seconds.
				SaveForCancelProcessing = false,
				Caption = "LoadRack|Off Load Complete?"
			};

			var menu = new ArrayList { "LoadRack|Yes", "LoadRack|Cancel" };

			// Save last station state
			this.PriorStationState = this.StationState;

			this.StationState = StationState.MANUAL_OFFLOAD_INPROGRESS;

			// add the completed line item to the transaction
			if (this.currentLineItem == null)
			{
				this.currentLineItem = new LineItemDO();
				var unitHelper = new UnitsHelperClass(this.Security, this.SiteManager.Site, this.CurrentTransactionAlias, null);
				unitHelper.SetUnits(this.currentLineItem, 0, null);
				this.currentLineItem.LineNumber = this.Transaction.LineItems.Count + 1;
			}
			this.currentLineItem.Status = TransactionStatus.InProgress;
			this.currentLineItem.Product = this.SelectedProductID;
			this.currentLineItem.MeterStartDateTime = TimeConverter.Now(this.SiteManager.Site);
			ProductClass Product = FMChannelHelper.MakeCall<IProducts, ProductClass>(
																	 x =>
																	 x.GetByID(this.Security, this.SelectedProductID)
																);

			if (Product != null)
			{
				this.currentLineItem.ProductType = ProductClass.ProductTypeID(Product.ProductType);
				this.currentLineItem.ProductCode = Product.Code;
				this.currentLineItem.ProductGuid = Product.MasterRecordGuid;
			}

			if (this.currentLineItem.Quantity == null)
			{
				this.currentLineItem.Quantity = new QuantityDO();
			}

			// although this does not make much sense. Instead of using the actual values from the meter we will
			// set the volume amounts equal to the manually entered amount
			if (this.currentLineItem.Density == null)
			{
				this.currentLineItem.Density = 0.0;
			}

			if (this.Station.SynchronizeReferenceDensity == false)
			{
				this.OffloadDensity = 1.0;
			}

			this.currentLineItem.Density = this.OffloadDensity;

			if (this.currentLineItem.MeterReading.MeterStart == null)
			{
				this.currentLineItem.MeterReading.MeterStart = 0.0;
			}

			this.currentLineItem.MeterReading.MeterStart = this.StartMeterValue;
			this.currentLineItem.MeterReading.MeterStop = null;

			if (this.SupplyOrder != null)
			{
				LineItemDO orderLineItem = this.FindMatchingOrderLineItem(this.SupplyOrder, this.SelectedProductID);

				if (orderLineItem != null)
				{
					this.currentLineItem.OrderReferenceTransactionLineItemGuid = orderLineItem.TransactionLineItemGuid;
					// store the supply order po number in the receipt transaction po number field citgo requirement
					this.Transaction.PONumber = this.SupplyOrder.PONumber;
				}

			}

			this.Transaction.LineItems.Add(this.currentLineItem);
			if (this.Station.PromptForBOLNumber)   // store the entered bol number in the document number field - citgo requirement
			{
				this.Transaction.DocumentNumber = this.SelectedBOLNumber;
			}

			this.UpdateTransaction();

			parameters.Menu = (string[])menu.ToArray(typeof(string));

			this.DisplayMenu(parameters);
		}

		protected override void ProcessOffLoadInProgress(string response)
		{
			if (response == "Cancel")
			{
				this.UpdatePermissives(false);

				foreach (VarecDETLoadArmManagerClass loadArmManager in this.LoadArmManagerCollection)
				{
					this.UpdateComponentPermissives(loadArmManager.CurrentLineItemRecipe, false);
				}

				if (this.Station.OffLoadByOffLoadID ||
			  this.UseOffLoadSupplyOrders == false) this.IssueOffLoadIDPrompt();
				else this.PromptForSupplyOrderNumber();
				return;
			}
			else if (response == EscapeString)
			{
				this.UpdatePermissives(false);

				foreach (VarecDETLoadArmManagerClass loadArmManager in this.LoadArmManagerCollection)
				{
					this.UpdateComponentPermissives(loadArmManager.CurrentLineItemRecipe, false);
				}

				if (this.UseManualMeter)
				{
					this.StationState = StationState.ENTER_MANUAL_METER_START_VALUE;
					this.DisplayMessage("[LoadRack|Enter] [LoadRack|Meter Start Value]", null, 10, this.PROMPT_TIMEOUT);
				}
				else
				{
					this.StationState = StationState.ENTER_UNLOAD_AMOUNT;
					this.DisplayMessage("[LoadRack|Enter] [LoadRack|Qty On BOL]", null, 10, this.PROMPT_TIMEOUT);
				}
				return;
			}
			if (this.UseManualMeter)
			{
				this.StationState = StationState.ENTER_MANUAL_METER_STOP_VALUE;
				this.DisplayMessage("[LoadRack|Enter] [LoadRack|Meter Stop Value]", null, 10, this.PROMPT_TIMEOUT);
			}
			else
			{
				this.ProcessManualMeterStopData("0");
			}
		}

		protected override void ProcessManualMeterStopData(string response)
		{
			this.UpdatePermissives(false);

			foreach (VarecDETLoadArmManagerClass loadArmManager in this.LoadArmManagerCollection)
			{
				this.UpdateComponentPermissives(loadArmManager.CurrentLineItemRecipe, false);
			}

			if (response == EscapeString)
			{
				// Build initial menu parameter set
				DisplayMenuParameters Parameters = new DisplayMenuParameters
				{
					ApplyDataDictionary = true,
					DefaultItem = 0,
					MenuTimeout = 0,
					SaveForCancelProcessing = false,
					Caption = "LoadRack|Off Load Complete?"
				};

				ArrayList Menu = new ArrayList
					 {
						  "LoadRack|Yes",
						  "LoadRack|Cancel"
					 };

				// Save last station state
				this.PriorStationState = this.StationState;

				this.StationState = StationState.MANUAL_OFFLOAD_INPROGRESS;

				Parameters.Menu = (string[])Menu.ToArray(typeof(string));

				this.DisplayMenu(Parameters);
				return;
			}
			else if (response == TimeoutString)
			{
				this.Transaction = null;
				this.SupplyOrder = null;
				this.Order = null;

				this.RemoteAuthorized = false;
				this.LoadArmManagerCollection.SetState(this, LOADARM_STATE.NORMAL);
				this.ResetStationDevice();
				return;
			}

			if (this.UseManualMeter)
			{
				try
				{
					this.StopMeterValue = System.Convert.ToDouble(response);
				}
				catch (FormatException)
				{
					//StationState = STATION_STATE.INVALID_MANUAL_METER_STOP_VALUE;
					this.DisplayMessage("LoadRack|Invalid value entered for Meter Stop", null, 10, this.PROMPT_TIMEOUT);
					return;
				}
				catch (OverflowException)
				{
					//StationState = STATION_STATE.INVALID_MANUAL_METER_STOP_VALUE;
					this.DisplayMessage("LoadRack|Invalid value entered for Meter Stop", null, 10, this.PROMPT_TIMEOUT);
					return;
				}
			}
			else  // read the configured meter opc point and store the data
			{
				this.StopMeterValue = this.ReadConfiguredMeterValue();
			}

			if (this.StopMeterValue < 0)
				return;

			// add the completed line item to the transaction
			if (this.currentLineItem == null)
			{
				this.currentLineItem = new LineItemDO();
				var unitHelper = new UnitsHelperClass(this.Security, this.SiteManager.Site, this.CurrentTransactionAlias, null);
				unitHelper.SetUnits(this.currentLineItem, 0, null);
				this.currentLineItem.LineNumber = this.Transaction.LineItems.Count + 1;
			}
			this.currentLineItem.Status = TransactionStatus.Completed;
			this.currentLineItem.Product = this.SelectedProductID;
			this.currentLineItem.MeterStopDateTime = TimeConverter.Now(this.SiteManager.Site);

			ProductClass Product = FMChannelHelper.MakeCall<IProducts, ProductClass>(
																	 x =>
																	 x.GetByID(this.Security, this.SelectedProductID)
																);

			if (Product != null)
			{
				this.currentLineItem.ProductType = ProductClass.ProductTypeID(Product.ProductType);
				this.currentLineItem.ProductCode = Product.Code;
				this.currentLineItem.ProductGuid = Product.MasterRecordGuid;
			}

			if (this.currentLineItem.Quantity == null)
			{
				this.currentLineItem.Quantity = new QuantityDO();
			}

			// although this does not make much sense. Instead of using the actual values from the meter we will
			// set the volume amounts equal to the manually entered amount
			this.currentLineItem.Quantity.NetInventoryChange = this.OffLoadPresetAmount;
			this.currentLineItem.Quantity.BadNetQualityLogged = false;

			this.currentLineItem.Quantity.GrossInventoryChange = this.OffLoadPresetAmount;
			this.currentLineItem.Quantity.BadGrossQualityLogged = false;

			if (this.currentLineItem.Density == null)
			{
				this.currentLineItem.Density = 0.0;
			}

			if (this.Station.SynchronizeReferenceDensity == false)
			{
				this.OffloadDensity = 1.0;
			}

			this.currentLineItem.Density = this.OffloadDensity;

			if (this.currentLineItem.MeterReading.MeterStart == null) this.currentLineItem.MeterReading.MeterStart = 0.0;
			if (this.currentLineItem.MeterReading.MeterStop == null) this.currentLineItem.MeterReading.MeterStop = 0.0;

			this.currentLineItem.MeterReading.MeterStart = this.StartMeterValue;
			this.currentLineItem.MeterReading.MeterStop = this.StopMeterValue;

			if (this.SupplyOrder != null)
			{
				LineItemDO orderLineItem = this.FindMatchingOrderLineItem(this.SupplyOrder, this.SelectedProductID);

				if (orderLineItem != null)
				{
					this.currentLineItem.OrderReferenceTransactionLineItemGuid = orderLineItem.TransactionLineItemGuid;
					// store the supply order po number in the receipt transaction po number field citgo requirement
					this.Transaction.PONumber = this.SupplyOrder.PONumber;
				}

			}

			this.UpdateTransaction();

			this.CheckForEndOfOffLoadingOperation();
		}

		protected override void ProcessOffLoadProductSelect(string response)
		{
			bool productFound = false;
			if (response == EscapeString)
			{
				if (this.Station.OffLoadByOffLoadID == false &&
					 this.UseOffLoadSupplyOrders == true) this.PromptForSupplyOrderNumber();
				else
				{
					this.StationState = StationState.OFFLOADID_PROMPT;
					this.DisplayMessage("[LoadRack|Enter] [LoadRack|Off Load ID]", null, PromptLength, this.PROMPT_TIMEOUT);
				}
				return;
			}

			if (this.Station.OffLoadByOffLoadID || this.UseOffLoadSupplyOrders == false)
			{
				foreach (VarecDETLoadArmManagerClass loadArmManager in this.LoadArmManagerCollection)
				{
					foreach (ProductMapClass productMap in loadArmManager.LoadArm.OffloadExternalProductCollection)
					{
						if (productMap.AssignedID == response)
						{
							this.SelectedProductID = productMap.AssignedID;
							loadArmManager.CurrentLineItemProduct = FMChannelHelper.MakeCall<IProducts, ProductClass>(
																				  x =>
																				  x.GetByID(this.Security, this.SelectedProductID)
																			);
							loadArmManager.CurrentLineItemRecipe = productMap;
							if (loadArmManager.CurrentLineItemProduct != null)
							{
								productFound = true;
							}
							break;
						}
					}
				}
			}
			else
			{
				if (this.SupplyOrder.LineItems.Count > 0)
				{
					foreach (VarecDETLoadArmManagerClass loadArmManager in this.LoadArmManagerCollection)
					{
						// check for different products in the line items and present the user with a selection
						foreach (object lineItemObject in this.SupplyOrder.LineItems)
						{
							var lineItem = (LineItemDO)lineItemObject;
							if (lineItem.Product == response)
							{
								this.SelectedProductID = lineItem.Product;
								ProductMapClass productMap = null;
								foreach (ProductMapClass testedProductMap in loadArmManager.LoadArm.OffloadExternalProductCollection)
								{
									if (testedProductMap.AssignedID == this.SelectedProductID)
									{
										productMap = testedProductMap;
										break;
									}
								}

								if (productMap == null)
								{
									continue;
								}

								loadArmManager.CurrentLineItemProduct = FMChannelHelper.MakeCall<IProducts, ProductClass>(
																				 x =>
																				 x.GetByID(this.Security, this.SelectedProductID)
																		  );
								if (loadArmManager.CurrentLineItemProduct != null)
								{
									productFound = true;
									loadArmManager.CurrentLineItemRecipe = productMap;
								}

								break;
							}
						}
					}
				}
			}

			if (productFound == false)
			{
				this.DisplayOffLoadProductSelect();
				return;
			}

			this.PromptForBOLNumber();
		}

		protected double ReadConfiguredMeterValue()
		{
			double dReturnValue = 0.0;

			foreach (VarecDETLoadArmManagerClass loadArmManager in this.LoadArmManagerCollection)
			{
				if (loadArmManager.CurrentLineItemRecipe != null)
				{
					foreach (ProcessVariableClass processVariable in loadArmManager.CurrentLineItemRecipe.ProcessVariableCollection)
					{
						if (processVariable.ProcessVariableType != PROCESS_VARIABLE_TYPE.COMPONENT_METER_FLOW_TOTAL_PV)
						{
							continue;
						}

						this.OPCServerManager.Read(processVariable);

						// ReSharper disable ImpureMethodCallOnReadonlyValueField
						if (processVariable.OPCQuality != Quality.Bad.GetCode())
						{
							dReturnValue = System.Convert.ToDouble(processVariable.ServerValue);
						}
						else
						{
							this.AddAlarmAndEventLogs(this.Security, this.Station.ErrorReadingMeterEvent(this.Station.ID));
							this.LoadRackManager.EventOrAlarmEvent.Set();

							this.StationState = StationState.RESET_ON_TIMEOUT;
							this.DisplayMessage("LoadRack|Error Reading Meter", null, 0, this.MESSAGE_TIMEOUT);
							return -1;
						}
						// ReSharper restore ImpureMethodCallOnReadonlyValueField
					}
				}

				break;
			}

			return dReturnValue;
		}

		protected override void CheckForEndOfOffLoadingOperation()
		{
			if (this.Station.PromptForBOLNumber)
			{
				// store the entered bol number in the document number field - citgo requirement
				this.Transaction.DocumentNumber = this.SelectedBOLNumber;
			}

			this.UpdatePermissives(false);

			// Per J. Ussery, DET Offload should go straight to ending the offload.
			this.CompleteOffLoadingTransaction();

			if (this.Station.CardReader)
			{
				this.IssuePleaseCardIn();
			}
			else
			{
				this.IssueDriverIDPrompt();
			}
		}

		protected override void LoadRackProcessing(ProcessVariableClass PV)
		{
			if (this.StationState == StationState.ENTER_DRIVER_ID_PROMPT)
				return;

			switch (PV.ProcessVariableType)
			{
				case PROCESS_VARIABLE_TYPE.INPUT_PERMISSIVE_PV:
					{
						PermissivesClass Permissives = PV.Parent as PermissivesClass;
						if (Permissives == null)
							break;

						Permissives.Update();

						this.OPCServerManager.Update(true);

						if (!PV.IsQualityGood
						|| !((bool)PV.ServerValue)) this.IssuePermissiveMessage(this);

						break;
					}

				default:
					this.eventLog.WriteEntry("StationManager OnInvoke : Unknown PV : " + PV.OPCItemID);
					break;
			}
		}

		public bool VerifyPermissiveInputs()
		{
			foreach (ProcessVariableClass pv in this.Station.StationPermissives.Inputs)
			{
				switch (pv.ProcessVariableType)
				{
					case PROCESS_VARIABLE_TYPE.INPUT_PERMISSIVE_PV:
						{
							PermissivesClass permissives = pv.Parent;
							if (permissives == null)
								break;

							permissives.Update();

							this.OPCServerManager.Update(true);

							if (!pv.IsQualityGood)
							{
								return false;
							}
							else if (!((bool)pv.ServerValue))
							{
								this.ConsecutivePrompts++;
								this.IssuePermissiveMessage(this);
								return false;
							}
							else
							{
								this.ConsecutivePrompts = 0;
							}
							break;
						}

					default:
						this.eventLog.WriteEntry("StationManager OnInvoke : Unknown PV : " + pv.OPCItemID);
						return false;
				}
			}
			return true;
		}

		public void IssuePermissiveMessage(VarecDETStationManagerClass stationManager)
		{
			if (stationManager == null)
				return;

			string Message = this.GetPermissiveMessage(stationManager);

			if (string.IsNullOrEmpty(Message) == false)
			{
				this.PriorStationState = this.StationState;
				this.StationState = StationState.AUTHORIZED_PERMISSIVE_PROMPT;
				this.DisplayMessage(Message + " " + stationManager.AcknowledgementMessage, null, 0, this.PROMPT_TIMEOUT);
			}
		}

		protected string GetPermissiveMessage(StationManagerClass stationManager)
		{
			string Message = "";
			bool FailedPermissive = false;

			// Check Site Permissive
			foreach (ProcessVariableClass PV in stationManager.SiteManager.Site.ProcessVariableCollection)
			{
				if (PV.ProcessVariableType == PROCESS_VARIABLE_TYPE.SITE_PERMISSIVE_PV
				&& (!PV.IsQualityGood
				|| !((bool)PV.ServerValue)))
				{
					Message = PV.MessageID;
					FailedPermissive = true;
					break;
				}
			}

			// Check Station Permissives
			if (!FailedPermissive)
			{
				foreach (ProcessVariableClass PV in stationManager.Station.StationPermissives.Inputs)
				{
					if (!PV.IsQualityGood
					|| !((bool)PV.ServerValue))
					{
						Message = PV.MessageID;
						FailedPermissive = true;
						break;
					}
				}
			}

			return Message;
		}

		protected override void ProcessPermissiveMessageAcknowledge(string Response)
		{
			if (this.StationState == StationState.INPROGRESS_PERMISSIVE_PROMPT)
			{
				this.ResetStationDevice();
			}
			else
			{
				this.StationState = this.PriorStationState;
				// recheck the permissive
				if (!this.VerifyPermissiveInputs())
				{
					if (this.ConsecutivePrompts >= this.SiteManager.Site._MaximumPrompts)
					{
						this.StationState = StationState.RESET_ON_TIMEOUT;
						this.DisplayMessage("LoadRack|Max Retries Exceeded", null, 0, this.MESSAGE_TIMEOUT);
						this.ConsecutivePrompts = 0;
						return;
					}
					if (this.StationState != StationState.AUTHORIZED_PERMISSIVE_PROMPT)
					{
						this.StationState = StationState.RESET_ON_TIMEOUT;
						this.DisplayMessage("LoadRack|Permissive Failure Process Aborted", null, 0, this.MESSAGE_TIMEOUT);
					}
					return;
				}

				DisplayMenuParameters Parameters;
				switch (this.StationState)
				{
					case StationState.ENTER_UNLOAD_AMOUNT:
						this.DisplayMessage("[LoadRack|Enter] [LoadRack|Qty On BOL]", null, 10, this.PROMPT_TIMEOUT);
						break;
					case StationState.ENTER_MANUAL_METER_START_VALUE:
						this.DisplayMessage("[LoadRack|Enter] [LoadRack|Meter Start Value]", null, 10, this.PROMPT_TIMEOUT);
						break;
					case StationState.MANUAL_OFFLOAD_INPROGRESS:
						Parameters = new DisplayMenuParameters
						{
							ApplyDataDictionary = true,
							DefaultItem = 0,
							MenuTimeout = 0,
							SaveForCancelProcessing = false,
							Caption = "LoadRack|Off Load Complete?"
						};

						ArrayList Menu = new ArrayList
						{
								"LoadRack|Yes",
								"LoadRack|Cancel"
						};

						Parameters.Menu = (string[])Menu.ToArray(typeof(string));

						this.DisplayMenu(Parameters);
						break;
					case StationState.ENTER_MANUAL_METER_STOP_VALUE:
						this.DisplayMessage("[LoadRack|Enter] [LoadRack|Meter Stop Value]", null, 10, this.PROMPT_TIMEOUT);
						break;
					case StationState.PROMPT_FOR_OFFLOAD_COMPLETE:
						Parameters = new DisplayMenuParameters
						{
							ApplyDataDictionary = true,
							DefaultItem = 0,
							MenuTimeout = 999,

							Caption = "LoadRack|Select",

							Menu = new string[2]
						};
						Parameters.Menu[0] = "LoadRack|Off Load New Batch";
						Parameters.Menu[1] = "LoadRack|Finished Off Loading";

						this.DisplayMenu(Parameters);
						break;
					default:
						this.ResetStationDevice();
						break;
				}
			}
		}

		public override void UpdatePermissives(bool authorized)
		{
			foreach (ProcessVariableClass pv in this.Station.StationPermissives.Outputs)
			{
				switch (pv.ProcessVariableType)
				{
					case PROCESS_VARIABLE_TYPE.OUTPUT_PERMISSIVE_PV:
						{
							PermissivesClass permissives = pv.Parent;
							if (permissives == null)
								break;

							pv.ServerValue = (bool)authorized;

							this.OPCServerManager.Update(true);

							if (!pv.IsQualityGood
							|| ((bool)pv.ServerValue) != authorized)
							{
								this.AddAlarmAndEventLogs(this.Security, this.Station.ErrorSettingPermissiveEvent(this.Station.ID, pv.OPCItemID));
								this.LoadRackManager.EventOrAlarmEvent.Set();

								this.StationState = StationState.RESET_ON_TIMEOUT;
								this.DisplayMessage("LoadRack|Error Setting Permissive" + " " + pv.OPCItemID, null, 0, this.MESSAGE_TIMEOUT);
								return;
							}

							break;
						}

					default:
						this.eventLog.WriteEntry("StationManager OnInvoke : Unknown PV : " + pv.OPCItemID);
						break;
				}
			}

		}

		private void UpdateTransaction()
		{
			if (this.Transaction != null)
			{
				this.Transaction.Status = TransactionStatus.InProgress;

				DateTimeOffset siteTimeNow = TimeConverter.Now(this.SiteManager.Site);
				this.Transaction.TimeEnd = siteTimeNow;
				this.Transaction.TimeOut = siteTimeNow;

				this.SaveTransaction();
			}
		}

		public override void CompleteOffLoadingTransaction()
		{
			base.CompleteOffLoadingTransaction();

			this.currentLineItem = null;
		}

		public override void DisplayOffLoadProductSelect()
		{
			// for the DET we need to populate the menu with the configured phantom arm products
			if (this.AvailableLoadArmManagers == 0)
			{
				this.StationState = StationState.RESET_ON_TIMEOUT;
				this.DisplayMessage("LoadRack|No Products Available", null, 0, this.MESSAGE_TIMEOUT);
				return;
			}

			// check that the supplier has authorized products configured
			if (this.Supplier.SupplierAuthorizedProductCollection.Count == 0)
			{
				this.StationState = StationState.RESET_ON_TIMEOUT;
				this.DisplayMessage("LoadRack|No Products Available", null, 0, this.MESSAGE_TIMEOUT);
				return;
			}

			// Build menu parameter set
			var parameters = new DisplayMenuParameters
			{
				ApplyDataDictionary = true,
				DefaultItem = 0,
				MenuTimeout = this.PROMPT_TIMEOUT,
				SaveForCancelProcessing = false,
				Caption = "LoadRack|Select Off Load Product"
			};

			var menu = new ArrayList();

			// Save last station state
			this.PriorStationState = this.StationState;

			foreach (LoadArmManagerClass loadArmManager in this.LoadArmManagerCollection)
			{
				loadArmManager.OpcServerManager.Update(false);

				foreach (ProductMapClass productMap in loadArmManager.LoadArm.OffloadExternalProductCollection)
				{
					foreach (ProductMapClass supplierProduct in this.Supplier.SupplierAuthorizedProductCollection)
					{
						if (supplierProduct.AssignedID == productMap.AssignedID && this.IsProductMapPermissed(productMap, this.Manager))
						{
							menu.Add(productMap.AssignedID);
						}
					}
				}
			}

			if (menu.Count == 0)
			{
				this.StationState = StationState.RESET_ON_TIMEOUT;
				this.DisplayMessage("LoadRack|No Products Available", null, 0, this.MESSAGE_TIMEOUT);
				return;
			}

			parameters.Menu = (string[])menu.ToArray(typeof(string));

			this.StationState = StationState.SELECT_OFFLOAD_PRODUCT;

			this.DisplayMenu(parameters);

			this.currentLineItem = null;
		}

		public override void DisplayVerifySupplyOrderProduct()
		{
			// SelectedSupplyOrder
			bool productFound = false;

			// CardID=Response;
			// Check for preloads for the current driver
			var getTransactionSR = new GetTransactionSR
			{
				Security = this.Security,
				Request =
													 GetTransactionRequest.SITE_TYPEID_ALIAS_DOCUMENTNUMBER,
				Site = this.SiteManager.Site.ID,
				TransTypeID = TransactionTypes.T18_SupplyOrder,
				Status = ((int)TransactionStatus.Scheduled).ToString(),
				DocumentNumber = this.SelectedSupplyOrder
			};


			GetTransactionDO getTransactionDO = FMChannelHelper.MakeCall<IGetTransactionProcessor, GetTransactionDO>(
																						x =>
																						x.Process(getTransactionSR)
																				 );
			// Build menu parameter set
			var parameters = new DisplayMenuParameters
			{
				ApplyDataDictionary = true,
				DefaultItem = 0,
				MenuTimeout = this.PROMPT_TIMEOUT,
				SaveForCancelProcessing = false,
				Caption = "LoadRack|Select Off Load Product"
			};

			var menu = new ArrayList();

			// Save last station state
			this.PriorStationState = this.StationState;

			if (getTransactionDO?.TransactionDataSet != null && getTransactionDO.TransactionDataSet.Tables.Count != 0 && getTransactionDO.TransactionDataSet.Tables[0].Rows.Count != 0)
			{
				foreach (DataRow row in getTransactionDO.TransactionDataSet.Tables[0].Rows)
				{
					var documentNumber = row["TransID"] as string;
					if (documentNumber != string.Empty)
					{
						this.SupplyOrder = this.GetTransaction(documentNumber);

						// check for multiple line items to very product
						if (this.SupplyOrder.LineItems.Count > 0)
						{
							// check for different products in the line items and present the user with a selection
							foreach (LineItemDO lineItem in this.SupplyOrder.LineItems)
							{
								if (lineItem.Product != null && lineItem.Status == TransactionStatus.Scheduled)
								{
									// make sure this product is in the arm or the transaction will not be saved
									foreach (LoadArmManagerClass loadArmManager in this.LoadArmManagerCollection)
									{
										foreach (ProductMapClass productMap in loadArmManager.LoadArm.OffloadExternalProductCollection)
										{
											if (lineItem.Product == productMap.AssignedID && this.IsProductMapPermissed(productMap, this.Manager))
											{
												menu.Add(productMap.AssignedID);
												productFound = true;
												parameters.Menu = (string[])menu.ToArray(typeof(string));
											}
										}
									}
								}
							}
						}

						if (productFound == false)
						{
							this.StationState = StationState.RESET_ON_TIMEOUT;
							this.DisplayMessage("LoadRack|No Products Available", null, 0, this.MESSAGE_TIMEOUT);
							return;
						}

						if (this.SupplyOrder.ShipToCompanyGuid != Guid.Empty)
						{
							this.ShipTo = this.GetCompany(this.Security, this.SupplyOrder.ShipToCompanyGuid);
						}
						if (this.SupplyOrder.BillToCompanyGuid != Guid.Empty)
						{
							this.BillTo = this.GetCompany(this.Security, this.SupplyOrder.BillToCompanyGuid);
						}
						if (this.SupplyOrder.ShipperCompanyGuid != Guid.Empty)
						{
							this.Shipper = this.GetCompany(this.Security, this.SupplyOrder.ShipperCompanyGuid);
						}
						if (this.SupplyOrder.OwnerCompanyGuid != Guid.Empty)
						{
							this.Owner = this.GetCompany(this.Security, this.SupplyOrder.OwnerCompanyGuid);
						}
						if (this.SupplyOrder.ManagerCompanyGuid != Guid.Empty)
						{
							this.Manager = this.GetCompany(this.Security, this.SupplyOrder.ManagerCompanyGuid);
						}
						if (this.SupplyOrder.SupplierCompanyGuid != Guid.Empty)
						{
							this.Supplier = this.GetCompany(this.Security, this.SupplyOrder.SupplierCompanyGuid);
						}

						this.StationState = StationState.SELECT_OFFLOAD_PRODUCT;

						this.DisplayMenu(parameters);
						return;
					}
				}

				// default error message if any of the above does not complete
				this.DisplayMessage("[LoadRack|Invalid Selection]", null, 0, this.MESSAGE_TIMEOUT);
				this.StationState = StationState.INVALID_SUPPLIER_PROMPT_RESPONSE_MESSAGE;
			}

			this.currentLineItem = null;
		}
		public bool IsProductMapPermissed(ProductMapClass productMap, CompanyClass manager)
		{
			if (productMap == null)
			{
				this.eventLog.WriteEntry("Null productmap in IsProductMapPermissed", EventLogEntryType.Error);
				return false;
			}

			if (!productMap.Permissives.Permitted)
			{
				this.eventLog.WriteEntry("Offload Disabled by Permissives : " + productMap.AssignedID, EventLogEntryType.Error);
				return false;
			}

			TankClass tank = this.SiteManager.GetTank(productMap, manager);
			if (tank == null)
			{
				this.eventLog.WriteEntry("No Tank for Offload Product : " + productMap.AssignedID, EventLogEntryType.Error);
				return false;
			}

			// We may want to modify and use below code in the future to prevent loading if BOL amount would overfill tank
			////ProcessVariableClass PV;
			////if (!SiteManager.Site.LoadByNet)
			////    PV = tank.ProcessVariableCollection[PROCESS_VARIABLE_TYPE.AVAILABLE_GROSS_VOLUME_PV];
			////else
			////    PV = tank.ProcessVariableCollection[PROCESS_VARIABLE_TYPE.AVAILABLE_NET_VOLUME_PV];

			////if (PV == null)
			////{
			////    EventLog.WriteEntry("No Tank Available Volume Process Variable for Tank : " + tank.ID, EventLogEntryType.Error);
			////    return false;
			////}

			////if ((!SiteManager.Site.UseLastKnownGoodTankData
			////    && !PV.IsQualityGood)
			////    || !typeof(double).IsInstanceOfType(PV.SIValue))
			////{
			////    EventLog.WriteEntry("Available Gross Volume OPC Quality Bad for Tank : " + tank.ID, EventLogEntryType.Error);
			////    return false;
			////}

			////CU_UNIT units = (currentTransactionAlias.VolumeUnits != 0) ? currentTransactionAlias.VolumeUnits : SiteManager.Site.VolumeUnits;
			////byte decimalPlaces = (currentTransactionAlias.VolumeUnits != 0) ? currentTransactionAlias._VolumeDecimalPlaces : SiteManager.Site._VolumeDecimalPlaces;

			////double MaximumAvailable = (double)PV.GetValue(units, decimalPlaces);

			////if (MaximumLoadAmount > MaximumAvailable)
			////    return false;

			return true;
		}
		private void UpdateComponentPermissives(ProductMapClass productMap, bool authorized)
		{
			foreach (object loadArmManagerObject in this.LoadArmManagerCollection)
			{
				var loadArmManager = loadArmManagerObject as VarecDETLoadArmManagerClass;
				loadArmManager?.UpdateOffloadProductPermissives(productMap, authorized);
			}
		}

		protected override void ProcessProduct(string response)
		{
			// DET menu selection response is the full menu item text; no requirement to parse into an index and 
			// find menu item by that index
			if (response == EscapeString)
			{
				if (this.AvailableCompartments(this.CurrentEquipment) > 1)
				{
					this.IssueCompartmentSummaryPrompt();
				}
				else
				{
					if (this.AvailableCompartments(this.CurrentEquipment) > 1)
					{
						this.IssueCompartmentSummaryPrompt();
					}

					else if (this.LoadSummaryIssued)
					{
						this.IssueLoadSummaryPrompt();
					}

					else if (this.CurrentEquipment.MasterRecordGuid == this.Trailer3.MasterRecordGuid
						 && this.AvailableCompartments(this.Trailer2) > 0)
					{
						this.CurrentEquipment = this.Trailer2;
						this.IssueCompartmentSummaryPrompt();
					}

					else if (this.CurrentEquipment.MasterRecordGuid == this.Trailer2.MasterRecordGuid
						 && this.AvailableCompartments(this.Trailer1) > 0)
					{
						this.CurrentEquipment = this.Trailer1;
						this.IssueCompartmentSummaryPrompt();
					}

					else if (this.CurrentEquipment.MasterRecordGuid == this.Trailer1.MasterRecordGuid
						 && this.AvailableCompartments(this.TractorOrTanker) > 0)
					{
						this.CurrentEquipment = this.TractorOrTanker;
						this.IssueCompartmentSummaryPrompt();
					}

					else if (this.Order != null)
					{
						if (this.SiteManager.Site.PromptForShipmentNumber)
						{
							this.IssueEnterShipmentNumberPrompt();
						}
						else
						{
							this.IssueEnterOrderNumberPrompt();
						}
					}

					else
					{
						this.IssueLoadIDPrompt();
					}
				}
			}
			else
			{
				LineItemDO lineItem;
				if (this.Station.Type == STATION_TYPE.MANUAL_BOL)
				{
					if (this.CurrentLineItemBaseIndex == -1)
					{
						lineItem = new LineItemDO
						{
							LoadingLocationID = this.Station.ID,
							LoadingLocationStationGuid = this.Station.IdentityGuid
						};
						this.Transaction.LineItems.Add(lineItem);
						this.CurrentLineItemBaseIndex = this.Transaction.LineItems.Count - 1;
					}
					else
					{
						lineItem = this.Transaction.LineItems[this.CurrentLineItemBaseIndex];
					}
				}
				else
				{
					lineItem = this.CurrentLineItem;
				}

				if (response == FMChannelHelper.MakeCall<IDataDictionariesClass, string>(x => x.Get(this.SiteManager.Site.SiteGuid, "LoadRack|None")))
				{
					lineItem.Product = null;
					lineItem.ProductGuid = Guid.Empty;
					lineItem.ProductCode = null;
					lineItem.ProductType = null;
					lineItem.CustomerProductName = null;
					lineItem.CustomerProductCode = null;
					lineItem.PresetAmount = null;
					lineItem.StorageLocationID = null;
					lineItem.StorageLocationTankGuid = Guid.Empty;
					lineItem.Density = null;
					lineItem.VCF = null;
					lineItem.OrderReferenceTransactionLineItemGuid = Guid.Empty;

					if (!this.ProductsConfigured)
					{
						this.ByWeight = false;
						this.ByWeightProduct = string.Empty;
					}

					if (this.Station.Type == STATION_TYPE.MANUAL_BOL)
					{
						this.IssueLineItemSummaryPrompt();
					}
					else if (this.AvailableCompartments(this.CurrentEquipment) > 1)
					{
						this.IssueCompartmentSummaryPrompt();
					}
					else
					{
						this.IssueLoadSummaryPrompt();
					}
				}
				else
				{
					var unitsHelper = new UnitsHelperClass(this.Security, this.SiteManager.Site, this.CurrentTransactionAlias, null);

					foreach (ProductMapClass authorizedProduct in this.ShipTo.AuthorizedProductCollection)
					{
						if (response != GetLoadRackDisplayText(authorizedProduct))
						{
							continue;
						}

						ProductClass product = this.GetByProductAuthorizedCompanies(
							 this.Security, authorizedProduct.AssignedGuid, false);
						unitsHelper.Product = product;

						this.CurrentTransactionAlias = this.GetTransactionAlias(
							 this.Security,
							 (product.LoadByWeight)
								  ? this.Station.IssueByWeightTransactionAliasGuid
								  : this.Station.IssueByVolumeTransactionAliasGuid,
							 false);

						AdditiveProfileClass additiveProfile = null;
						if (authorizedProduct.AdditiveProfileGuid != Guid.Empty)
						{
							additiveProfile = this.GetAdditiveProfiles(this.Security, authorizedProduct.AdditiveProfileGuid);
						}

						lineItem.Product = authorizedProduct.AssignedID;
						lineItem.ProductCode = authorizedProduct.AssignedCode;
						lineItem.ProductType = ProductClass.ProductTypeID(authorizedProduct.AssignedProductType);
						lineItem.CustomerProductName = authorizedProduct.ShipToProductID;
						lineItem.CustomerProductCode = authorizedProduct.ShipToProductCode;
						lineItem.ProductGuid = FMChannelHelper.MakeCall<IProducts, Guid>(x => x.GetMasterRecordGuid(this.Security, authorizedProduct.AssignedGuid));

						unitsHelper.SetUnits(lineItem, 0, product);

						// Density and VCF are set to default values.  When Tank is determined
						// these are reset to values from SCADA.
						var standardDensity = new SIDouble
						{
							Units = lineItem.DensityUnits,
							SIValue = product._StandardDensity.SIValue
						};
						// units;
						lineItem.Density = standardDensity.Value;

						lineItem.VCF = 1.0;

						var standardTemperature = new SIDouble
						{
							Units = lineItem.TemperatureUnits,
							SIValue = (double)product._VcfModuleSettings.BaseTemperature.Value
							//SIValue = product._StandardTemperature.SIValue
						};
						// units;
						lineItem.Temperature = standardTemperature.Value;

						IQualityAssurance qaInterface = this.GetQualityAssuranceInterface();

						bool productAvailable = false;
						bool additiveProfileAvailable = (additiveProfile == null);
						bool tankCertified = false;
						bool certificateOfAnalysis = false;

						if (product.ProductType == ProductType.ComponentProduct)
						{
							foreach (StationManagerClass stationManager in this.SiteManager.StationManagerCollection)
							{
								bool stationProductAvailable = false;
								bool stationAdditiveProfileAvailable = (additiveProfile == null);
								bool stationTankCertified = false;
								bool stationCertificateOfAnalysis = false;

								if (!stationManager.Station.Enabled)
								{
									continue;
								}

								if (stationManager.Station.Type != STATION_TYPE.LOAD_RACK)
								{
									continue;
								}

								if (product.LoadByWeight)
								{
									// This is a bit confusing because in SiteLoadRackPage the alias is stored in the IssueByVolumeTransactionAliasIndex
									// even though the station may be a manual station created to represent a Load By Weight Station
									if (stationManager.Station.IssueByVolumeTransactionAliasGuid != this.Station.IssueByWeightTransactionAliasGuid)
									{
										continue;
									}
								}
								else
								{
									if (stationManager.Station.IssueByVolumeTransactionAliasGuid != this.Station.IssueByVolumeTransactionAliasGuid)
									{
										continue;
									}
								}

								TankClass tank = null;

								foreach (LoadArmManagerClass loadArmManager in stationManager.LoadArmManagerCollection)
								{
									stationProductAvailable = false;
									stationAdditiveProfileAvailable = (additiveProfile == null);
									stationTankCertified = false;
									stationCertificateOfAnalysis = false;

									if (stationManager != loadArmManager.GetStationManager())
									{
										continue;
									}

									if (!loadArmManager.IsProductServedByLoadArm(product))
									{
										continue;
									}

									ProductMapClass loadArmComponent = loadArmManager.GetComponent(product.MasterRecordGuid);
									if (loadArmComponent == null)
									{
										continue;
									}

									tank = this.SiteManager.GetTank(loadArmComponent, this.Manager);
									if (tank == null)
									{
										continue;
									}

									try
									{
										var productDensity = new SIDouble
										{
											Units = lineItem.DensityUnits,
											SIValue = this.GetTankValue(tank, PROCESS_VARIABLE_TYPE.DENSITY_PV)
										};

										double productVcf = this.GetTankValue(tank, PROCESS_VARIABLE_TYPE.VCF_PV);

										double productPressure = this.GetTankValue(tank, PROCESS_VARIABLE_TYPE.VAPOR_PRESSURE_PV);

										var productTemperature = new SIDouble
										{
											Units = lineItem.TemperatureUnits,
											SIValue =
																					this.GetTankValue(tank, PROCESS_VARIABLE_TYPE.TEMPERATURE_PV)
										};
										// units;

										lineItem.Density = productDensity.Value;
										lineItem.VCF = productVcf;
										lineItem.Temperature = productTemperature.Value;
										this.CurrentMaximum = this.GetMaximum(product, lineItem.Density, lineItem.VCF, productPressure, null, lineItem);

										if (
											 !stationManager.IsProductAvailable(
												  product, loadArmManager, this.CurrentMaximum, this.CurrentTransactionAlias))
										{
											continue;
										}

										productAvailable = true;
										stationProductAvailable = true;

										if (!loadArmManager.IsAdditiveProfileServedByLoadArm(additiveProfile))
										{
											continue;
										}

										additiveProfileAvailable = true;
										stationAdditiveProfileAvailable = true;
									}
									catch (Exception e)
									{
										this.eventLog.WriteEntry("StationManager ProcessProduct : " + e.Message, EventLogEntryType.Error);
										continue;
									}

									if (qaInterface == null)
									{
										tankCertified = true;
										certificateOfAnalysis = true;
										stationTankCertified = true;
										stationCertificateOfAnalysis = true;
									}
									else
									{
										if (!qaInterface.GetTankCertification(this.Security, tank.IdentityGuid, product.IdentityGuid))
										{
											continue;
										}

										tankCertified = true;
										stationTankCertified = true;

										FailedTestItem[] testItems;
										if (
											 !qaInterface.GetCertificateOfAnalysis(
												  this.Security,
												  tank.IdentityGuid,
												  product.IdentityGuid,
												  this.Owner.MasterRecordGuid,
												  this.BillTo.MasterRecordGuid,
												  this.ShipTo.MasterRecordGuid,
												  out testItems))
										{
											continue;
										}

										certificateOfAnalysis = true;
										stationCertificateOfAnalysis = true;
									}

									break;
								}

								// Break if Load Arm Identified for Loading
								if (stationProductAvailable && stationAdditiveProfileAvailable && stationTankCertified
									  && stationCertificateOfAnalysis)
								{
									// If Loading By Weight Record the Storage Location
									if (product.LoadByWeight && tank != null)
									{
										lineItem.StorageLocationID = tank.ID;
										lineItem.StorageLocationTankGuid = tank.IdentityGuid;
									}

									break;
								}
							}

							// if product fails on all load arms
							if (!productAvailable)
							{
								this.AddAlarmAndEventLogs(this.Security, this.Station.ProductUnavailableAlarm(product.ID));
							}

							else if (!additiveProfileAvailable)
							{
								this.AddAlarmAndEventLogs(this.Security, this.Station.AdditiveProfileUnavailableAlarm(additiveProfile.ID));
							}

							else if (!tankCertified)
							{
								this.AddAlarmAndEventLogs(this.Security, this.Station.NoTankCertificationAlarm(product.ID));
							}

							else if (!certificateOfAnalysis)
							{
								this.AddAlarmAndEventLogs(this.Security, this.Station.FailedCertificateOfAnalysisAlarm(this.ShipTo.ID, product.ID));
							}
						}

						// For Blends see if product is available as a Splash Blend
						else if (product.ProductType == ProductType.BlendProduct)
						{
							// For Blends see if product is available as a Splash Blend
							productAvailable = true;
							additiveProfileAvailable = true;
							tankCertified = true;
							certificateOfAnalysis = true;
							bool blendComponentsCoa = true;
							if (qaInterface != null)
							{
								blendComponentsCoa = qaInterface.BlendComponentsCOA(this.Security, product.MasterRecordGuid);
							}

							var productDensity = new SIDouble { Units = lineItem.DensityUnits, SIValue = 0 };
							double productVcf = 0;

							var productTemperature = new SIDouble { Units = lineItem.TemperatureUnits, SIValue = 0 };

							if (!blendComponentsCoa)
							{
								FailedTestItem[] testItems;
								certificateOfAnalysis = qaInterface.GetCertificateOfAnalysis(this.Security,
																													 Guid.Empty,
																													 product.MasterRecordGuid,
																													 this.Owner.MasterRecordGuid,
																													 this.BillTo.MasterRecordGuid,
																													 this.ShipTo.MasterRecordGuid,
																													 out testItems);
								if (!certificateOfAnalysis)
								{
									FMChannelHelper.MakeCall<IAlarmAndEventLogs>(x => x.Add(this.Security, this.Station.FailedCertificateOfAnalysisAlarm(this.ShipTo.ID, product.ID)));
									this.LoadRackManager.EventOrAlarmEvent.Set();
								}
							}

							this.CurrentMaximum = 0;

							foreach (ProductMapClass component in product.ComponentCollection)
							{
								bool componentProductAvailable = false;
								bool componentAdditiveProfileAvailable = additiveProfile == null;
								bool componentTankCertified = false;
								bool componentCertificateOfAnalysis = false;

								ProductClass componentProduct = FMChannelHelper.MakeCall<IProducts, ProductClass>(x => x.Get(this.Security, component.AssignedGuid));

								foreach (StationManagerClass stationManager in this.SiteManager.StationManagerCollection)
								{
									if (component.LockedOut)
									{
										continue;
									}

									if (!stationManager.Station.Enabled)
									{
										continue;
									}

									if (stationManager.Station.Type != STATION_TYPE.LOAD_RACK)
									{
										continue;
									}

									if (product.LoadByWeight)
									{
										// This is a bit confusing because in SiteLoadRackPage the alias is stored in the IssueByVolumeTransactionAliasIndex
										// even though the station may be a manual station created to represent a Load By Weight Station
										if (stationManager.Station.IssueByVolumeTransactionAliasGuid
											 != this.Station.IssueByWeightTransactionAliasGuid)
										{
											continue;
										}
									}
									else
									{
										if (stationManager.Station.IssueByVolumeTransactionAliasGuid
											 != this.Station.IssueByVolumeTransactionAliasGuid)
										{
											continue;
										}
									}

									bool loadAsRecipe = stationManager.IsProductAvailable(additiveProfile, authorizedProduct);

									foreach (LoadArmManagerClass loadArmManager in stationManager.LoadArmManagerCollection)
									{
										if (stationManager != loadArmManager.GetStationManager())
										{
											continue;
										}

										if (loadAsRecipe && !loadArmManager.IsProductServedByLoadArm(product))
										{
											continue;
										}

										if (!loadAsRecipe && !loadArmManager.IsProductServedByLoadArm(componentProduct))
										{
											continue;
										}

										ProductMapClass loadArmComponent = loadArmManager.GetComponent(componentProduct.IdentityGuid);
										if (loadArmComponent == null)
										{
											continue;
										}

										TankClass tank = this.SiteManager.GetTank(loadArmComponent, this.Manager);
										if (tank == null)
										{
											continue;
										}

										var componentDensity = new SIDouble();
										double componentVcf;
										var componentTemperature = new SIDouble();

										try
										{
											componentDensity.Units = lineItem.DensityUnits;
											componentDensity.SIValue = this.GetTankValue(tank, PROCESS_VARIABLE_TYPE.DENSITY_PV);

											componentVcf = this.GetTankValue(tank, PROCESS_VARIABLE_TYPE.VCF_PV);

											componentTemperature.Units = lineItem.TemperatureUnits;
											componentTemperature.SIValue = this.GetTankValue(tank, PROCESS_VARIABLE_TYPE.TEMPERATURE_PV);

											double componentPressure = this.GetTankValue(tank, PROCESS_VARIABLE_TYPE.VAPOR_PRESSURE_PV);
											double componentMaximum = this.GetMaximum(componentProduct, componentDensity.Value, componentVcf, componentPressure, component, lineItem);

											if (
												 !stationManager.IsProductAvailable(
													  componentProduct, loadArmManager, componentMaximum, this.CurrentTransactionAlias))
											{
												continue;
											}

											componentProductAvailable = true;

											if (!loadArmManager.IsAdditiveProfileServedByLoadArm(additiveProfile))
											{
												continue;
											}

											componentAdditiveProfileAvailable = true;
										}
										catch (Exception e)
										{
											this.eventLog.WriteEntry("StationManager ProcessProduct : " + e.Message, EventLogEntryType.Error);
											continue;
										}

										if (qaInterface == null)
										{
											componentTankCertified = true;
											componentCertificateOfAnalysis = true;
										}
										else
										{
											if (!qaInterface.GetTankCertification(this.Security, tank.IdentityGuid, componentProduct.MasterRecordGuid))
											{
												continue;
											}

											componentTankCertified = true;

											if (blendComponentsCoa)
											{
												FailedTestItem[] testItems;
												if (
													 !qaInterface.GetCertificateOfAnalysis(
														  this.Security,
														  tank.IdentityGuid,
														  componentProduct.MasterRecordGuid,
														  this.Owner.MasterRecordGuid,
														  this.BillTo.MasterRecordGuid,
														  this.ShipTo.MasterRecordGuid,
														  out testItems))
												{
													continue;
												}
											}

											componentCertificateOfAnalysis = true;
										}

										productDensity.SIValue += componentDensity.SIValue * component.BlendPercentage / 100;
										productVcf += componentVcf * component.BlendPercentage / 100;
										productTemperature.SIValue += componentTemperature.SIValue * component.BlendPercentage / 100;
										break;
									}

									// Break if component succeeeds on any load arm
									if (componentProductAvailable && componentAdditiveProfileAvailable && componentTankCertified
										 && componentCertificateOfAnalysis)
									{
										break;
									}
								}

								// if component fails on all load arms
								if (!componentProductAvailable)
								{
									FMChannelHelper.MakeCall<IAlarmAndEventLogs>(
										 x =>
											  x.Add(
													this.Security,
													this.Station.ProductUnavailableAlarm(
														 componentProduct.ID,
														 this.Driver.ID)));
									this.LoadRackManager.EventOrAlarmEvent.Set();
									productAvailable = false;
								}
								else if (!componentAdditiveProfileAvailable)
								{
									FMChannelHelper.MakeCall<IAlarmAndEventLogs>(x => x.Add(this.Security, this.Station.AdditiveProfileUnavailableAlarm(additiveProfile.ID)));
									this.LoadRackManager.EventOrAlarmEvent.Set();
									additiveProfileAvailable = false;
								}
								else if (!componentTankCertified)
								{
									FMChannelHelper.MakeCall<IAlarmAndEventLogs>(x => x.Add(this.Security, this.Station.NoTankCertificationAlarm(componentProduct.ID)));
									this.LoadRackManager.EventOrAlarmEvent.Set();
									tankCertified = false;
								}
								else if (!componentCertificateOfAnalysis)
								{
									FMChannelHelper.MakeCall<IAlarmAndEventLogs>(x => x.Add(this.Security, this.Station.FailedCertificateOfAnalysisAlarm(this.ShipTo.ID, componentProduct.ID)));
									this.LoadRackManager.EventOrAlarmEvent.Set();
									certificateOfAnalysis = false;
								}
							}

							if (productAvailable)
							{
								lineItem.Density = productDensity.Value;
								lineItem.VCF = productVcf;
								lineItem.Temperature = productTemperature.Value;
							}
						}

						if (product.LoadByWeight)
						{
							this.ByWeight = true;
							this.ByWeightProduct = response;
						}

						if (!productAvailable)
						{
							this.StationState = StationState.PRODUCT_UNAVAILABLE_MESSAGE;
							this.DisplayMessageWithAcknowledge("[LoadRack|Product Unavailable], ");
							return;
						}

						else if (!additiveProfileAvailable)
						{
							this.StationState = StationState.ADDITIVE_PROFILE_UNAVAILABLE_MESSAGE;
							this.DisplayMessageWithAcknowledge("[LoadRack|Additive Profile Unavailable], ");
							return;
						}

						else if (!tankCertified)
						{
							this.StationState = StationState.TANK_NOT_CERTIFIED_MSG;
							this.DisplayMessageWithAcknowledge("[LoadRack|Tank is not Certified], ");
							return;
						}

						else if (!certificateOfAnalysis)
						{
							this.StationState = StationState.FAILED_CERTIFICATE_OF_ANALYSIS_MSG;
							this.DisplayMessageWithAcknowledge("[LoadRack|Failed Certificate of Analysis], ");
							return;
						}

						if (product.LoadByWeight)
						{
							if ((this.Mode == OperatingMode.Loading && this.Station.IssueByWeightTransactionAliasGuid.IsEmpty())
								  || (this.Mode == OperatingMode.Unloading && this.Station.ReceiptByWeightTransactionAliasGuid.IsEmpty()))
							{
								this.ByWeight = false;
								this.ByWeightProduct = "";

								this.StationState = StationState.TRANSACTION_ALIAS_INVALID_MSG;
								this.DisplayMessage("LoadRack|Transaction Alias Invalid", null, 0, this.MESSAGE_TIMEOUT);
								return;
							}

							if (this.AvailableCompartments(this.CurrentEquipment) > 1)
							{
								this.IssueCompartmentSummaryPrompt();
							}

							else if (this.LoadSummaryIssued)
							{
								this.IssueLoadSummaryPrompt();
							}

							else if (this.CurrentEquipment.MasterRecordGuid == this.TractorOrTanker.MasterRecordGuid && this.AvailableCompartments(this.Trailer1) > 0)
							{
								this.CurrentEquipment = this.Trailer1;
								this.IssueCompartmentSummaryPrompt();
							}

							else if (this.CurrentEquipment.MasterRecordGuid == this.Trailer1.MasterRecordGuid && this.AvailableCompartments(this.Trailer2) > 0)
							{
								this.CurrentEquipment = this.Trailer2;
								this.IssueCompartmentSummaryPrompt();
							}

							else if (this.CurrentEquipment.MasterRecordGuid == this.Trailer2.MasterRecordGuid && this.AvailableCompartments(this.Trailer3) > 0)
							{
								this.CurrentEquipment = this.Trailer3;
								this.IssueCompartmentSummaryPrompt();
							}

							else
							{
								this.IssueLoadSummaryPrompt();
							}

							return;
						}

						if ((this.Mode == OperatingMode.Loading && this.Station.IssueByVolumeTransactionAliasGuid.IsEmpty())
							 || (this.Mode == OperatingMode.Unloading && this.Station.ReceiptByVolumeTransactionAliasGuid.IsEmpty()))
						{
							this.StationState = StationState.TRANSACTION_ALIAS_INVALID_MSG;
							this.DisplayMessage("LoadRack|Transaction Alias Invalid", null, 0, this.MESSAGE_TIMEOUT);
							return;
						}

						this.IssuePresetPrompt();
						return;
					}

					this.StationState = StationState.PRODUCT_UNAVAILABLE_MESSAGE;
					this.DisplayMessageWithAcknowledge("[LoadRack|Product Unavailable], ");
				}
			}
		}
	}
}
