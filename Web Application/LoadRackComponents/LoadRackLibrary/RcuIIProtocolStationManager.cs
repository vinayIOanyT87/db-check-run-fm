/******************************************************************************

	FILE NAME:		RcuIIProtocolStationManager.cs


	PURPOSE:			RcuIIProtocolStationManagerClass


	COMMENTS:

		Copyright (C) Varec, Inc. Norcross, GA, USA, 2009

		This file shall not be copied or reproduced in any form without
				the express written consent of Varec Inc.


	AUTHOR(S):	W. Gray


	VERSION:		1.0.0  Current version



	MODIFICATION HISTORY:
		Date:			By:			Reason:
		---------	----------  -------------------------------------------
*******************************************************************************/
using System;
using System.Collections;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;

using Opc;
using Opc.Da;

using FMBusinessObjects.DataObjects;

namespace LoadRackLibrary
{
	class RcuIIProtocolStationManagerClass:StationManagerClass
	{
		protected Thread ScanThread = null;
		protected ManualResetEvent StationKillEvent = null;
		protected ProcessVariableClass RcuStatusPV;
		protected ProcessVariableClass PowerUpPV;
		protected ProcessVariableClass HostUpPV;
		protected ProcessVariableClass CardReaderStatusPV;

		protected int MaxDisplayLineSize = 40;
		protected int MaxLines = 16;
		protected string Esc = System.Convert.ToChar(27).ToString();
		protected int MessageTimer = 0;
		protected ArrayList ProductIndexes = new ArrayList();
		protected int menuPage=0;
		protected int captionLines=0;

		public RcuIIProtocolStationManagerClass(
			EventLog EventLog,
			LoadRackManagerClass LoadRackManager,
			StationClass Station,
			SiteManagerClass SiteManager,
			SecurityClass Security)
			: base(EventLog,LoadRackManager,Station,SiteManager,Security)
		{
			RcuStatusPV = new ProcessVariableClass(
				PROCESS_VARIABLE_TYPE.STATION_PV,
				UNIT_TYPE.STATION_UNIT,
				VarEnum.VT_UI1,
				true,
				StationPv.OPCItemID + ".RCU Status",
				StationPv.URL,
				StationPv.ProgID);

			OPCServerManager.AddProcessVariable(RcuStatusPV);

			PowerUpPV = new ProcessVariableClass(
				PROCESS_VARIABLE_TYPE.LOADARM_PV,
				UNIT_TYPE.STATION_UNIT,
				VarEnum.VT_BOOL,
				true,
				StationPv.OPCItemID + ".Status.Power Up",
				StationPv.URL,
				StationPv.ProgID);

			OPCServerManager.AddProcessVariable(PowerUpPV);

			HostUpPV = new ProcessVariableClass(
				PROCESS_VARIABLE_TYPE.LOADARM_PV,
				UNIT_TYPE.STATION_UNIT,
				VarEnum.VT_BOOL,
				true,
				StationPv.OPCItemID + ".Status.Host Up",
				StationPv.URL,
				StationPv.ProgID);

			if(Station.CardReader)
			{
				CardReaderStatusPV = new ProcessVariableClass(
					PROCESS_VARIABLE_TYPE.CARDREADER_PV,
					UNIT_TYPE.STATION_UNIT,
					VarEnum.VT_UI1,
					true,
					StationPv.OPCItemID + ".Card Status",
					StationPv.URL,
					StationPv.ProgID);

				OPCServerManager.AddProcessVariable(CardReaderStatusPV);
			}           

			ThreadStart ScanStart = new ThreadStart(ScanDataThread);
			StationKillEvent = new ManualResetEvent(false);
			ScanThread = new Thread(ScanStart);
			ScanThread.Start();
			ScanThread.Priority = ThreadPriority.AboveNormal;

			try
			{


				// If the base object found a transaction in progress at the device, it will set the station
				// status to "Transaction In Progress".  In that case we do not want to reset the device as it
				// will yank the station out from under the in progress transaction
				if(StationState != StationState.TRANSACTION_IN_PROGRESS)
				{
					if(AvailableLoadArmManagers != 0)
					{
						if(Station.CardReader)
							IssuePleaseCardIn();
						else
							IssueDriverIDPrompt();
					}
				}
			}

			catch(Exception e)
			{
				EventLog.WriteEntry("RcuIIProtocolStationManager : "+e.Message,EventLogEntryType.Error);

				StationState = StationState.IDLE;
			}
		}

		public override void Dispose()
		{
			if(!AlreadyDisposed)
			{
				base.Dispose();

				// Terminate the Scan Thread
				if (StationKillEvent != null)
				{
					StationKillEvent.Set();
				}

               // Terminate the Scan Thread
               if (this.ScanThread != null)
               {
                  try
                  {
                     if(!this.ScanThread.Join(1000))
                           this.eventLog.WriteEntry("RcuIIProtocol Station manager Thread can not stop after 1 sec.", EventLogEntryType.Error);
                  }
                  catch (Exception e)
                  {
                     this.eventLog.WriteEntry("RcuIIProtocol Station manager Thread can not stop: " + e.Message, EventLogEntryType.Error);
                  }
               }

				GC.SuppressFinalize(this);
				AlreadyDisposed = true;
			}
		}

		public void ScanDataThread()
		{
            while (!StationKillEvent.WaitOne(1000, true))
			{
				Monitor.Enter(this);

                try
                {
                    if (MessageTimer > 0)
                    {
                        MessageTimer--;
                        if (MessageTimer == 0)
                        {
                            ProcessMessageTimeout();
                            if (this.StationState == StationState.IDLE)
                            {
                                if (!Station.CardReader)
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

                    if (GateTimer > 0)
                    {
                        GateTimer--;
                        if (GateTimer == 0)
                        {
                            StationState = StationState.IDLE;
                            GatePV.ServerValue = (bool)false;
                            OPCServerManager.Write(GatePV);
							if (Station.CardReader)
							{
								IssuePleaseCardIn();
							}
							else
							{
								IssueDriverIDPrompt();
							}
                        }
                    }
                }

                catch (Exception e)
                {
                    eventLog.WriteEntry("RcuIIProtocolStationManager Scan : " + e.Message);
                }

                Monitor.Exit(this);
			}
		}

		public override void ResetStationDevice()
		{
			base.ResetStationDevice();

			if(Station.CardReader)
				IssuePleaseCardIn();
			else
				IssueDriverIDPrompt();
		}


		public override int DisplayMessage(string stockMessage,string defaultResponse,int responseLength,int messageTimeout,bool saveForCancelProcessing)
		{
			string message = this.GetDataDictionaryValueByKey(SiteManager.Site.SiteGuid,stockMessage);

			char[] Seperators = { ' ' };
			string[] Strings = message.Split(Seperators);
			ArrayList Lines = new ArrayList();

			foreach(string SubMessage in Strings)
			{
				if (SubMessage.Length > MaxDisplayLineSize)
				{
					break;
				}

				if (Lines.Count == 0)
				{
					Lines.Add(SubMessage);
				}

				else
				{
					if (((string)Lines[Lines.Count - 1]).Length + SubMessage.Length + 1 <= MaxDisplayLineSize)
					{
						Lines[Lines.Count - 1] = ((string)Lines[Lines.Count - 1]) + " " + SubMessage;
					}
					else
					{
						if (Lines.Count >= MaxLines)
						{
							break;
						}

						Lines.Add(SubMessage);
					}
				}
			}

         ItemValue TerminalCommand = new ItemValue(StationPv.OPCItemID + ".Terminal Command")
         {
            Value = Esc + "O" + Esc + "H" + Esc + "K"
         };

         string HeaderText = GetHeaderText();
			int LineNumber = 0;
			TerminalCommand.Value += Esc + "Y" + System.Convert.ToChar(0x20 + LineNumber).ToString() + " " + HeaderText;

			++LineNumber;
			foreach(string Line in Lines)
			{
				TerminalCommand.Value += Esc + "Y" + System.Convert.ToChar(0x20 + LineNumber).ToString() + " " + Line;
				LineNumber++;
			}

			if (responseLength != 0)
			{
				TerminalCommand.Value += ": " + Esc + "@" + Esc + "L" + responseLength.ToString("D2") + Esc + "E";
			}
			else
			{
				TerminalCommand.Value += Esc + "@" + Esc + "L" + responseLength.ToString("D2") + Esc + "E" + Esc + "O";
			}

			MessageTimer = messageTimeout;

			WriteLogDataToCommFile(TerminalCommand.Value.ToString(),true);

			OPCServerManager.Write(new URL(StationPv.URL),new ItemValue[] { TerminalCommand });

			return LineNumber;
		}

		protected override void PromptForPin(string stockMessage,int responseLength,int messageTimeout)
		{
			string Message = this.GetDataDictionaryValueByKey(SiteManager.Site.SiteGuid,stockMessage);

			WriteLogDataToCommFile(Message,true);

			char[] Seperators = { ' ' };
			string[] Strings = Message.Split(Seperators);
			string Esc = System.Convert.ToChar(27).ToString();
			ArrayList Lines = new ArrayList();

			foreach(string SubMessage in Strings)
			{
				if (SubMessage.Length > MaxDisplayLineSize)
				{
					break;
				}

				if (Lines.Count == 0)
				{
					Lines.Add(SubMessage);
				}
				else
				{
					if (((string)Lines[Lines.Count - 1]).Length + SubMessage.Length + 1 <= MaxDisplayLineSize)
					{
						Lines[Lines.Count - 1] = ((string)Lines[Lines.Count - 1]) + " " + SubMessage;
					}
					else
					{
						if (Lines.Count >= MaxLines)
						{
							break;
						}

						Lines.Add(SubMessage);
					}
				}
			}

         ItemValue TerminalCommand = new ItemValue(StationPv.OPCItemID + ".Terminal Command")
         {
            Value = Esc + "O" + Esc + "H" + Esc + "K"
         };

         string HeaderText = GetHeaderText();
			int LineNumber = 0;
			TerminalCommand.Value += Esc + "Y" + System.Convert.ToChar(0x20 + LineNumber).ToString() + " " + HeaderText;

			++LineNumber;
			foreach(string Line in Lines)
			{
				TerminalCommand.Value += Esc + "Y" + System.Convert.ToChar(0x20 + LineNumber).ToString() + " " + Line;
				LineNumber++;
			}

			if (responseLength != 0)
			{
				TerminalCommand.Value += " : " + Esc + "!" + Esc + "L" + responseLength.ToString("D2") + Esc + "E";
			}

			OPCServerManager.Write(new URL(StationPv.URL),new ItemValue[] { TerminalCommand });

			MessageTimer = messageTimeout;

			return;
		}

		public override void DisplayMenu(DisplayMenuParameters parameters)
		{
			int maxMessageLength=200;

			if (CurrentMenuParameters == null)
			{
				menuPage = 0;
			}

			WriteMenuLogDataToCommFile(parameters);

			string DefaultResponse = "None";

			captionLines = DisplayMessage(parameters.Caption,DefaultResponse,2,PROMPT_TIMEOUT,false);

         ItemValue TerminalCommand = new ItemValue(StationPv.OPCItemID + ".Terminal Command")
         {
            Value = Esc + "Y" + System.Convert.ToChar(0x20 + captionLines).ToString() + " "
         };

         int optionNumber=0;
			for(int Index = menuPage*(MaxLines-captionLines);Index < parameters.Menu.Length && optionNumber < (MaxLines-captionLines);Index++)
			{
				string MenuItem = parameters.Menu[Index];

				if (parameters.ApplyDataDictionary)
				{
					MenuItem = this.GetDataDictionaryValueByKey(Station.SiteGuid, MenuItem);
				}

				MenuItem = (optionNumber+1).ToString() + ". " + MenuItem;

				if (MenuItem.Length > MaxDisplayLineSize)
				{
					MenuItem = MenuItem.Substring(0, MaxDisplayLineSize);
				}

				string lineItem=MenuItem + Esc + "O" + Esc + "Y" + System.Convert.ToChar(0x20 + captionLines + optionNumber + 1).ToString() + " ";

				if ((TerminalCommand.Value as string).Length + lineItem.Length < maxMessageLength)
				{
					TerminalCommand.Value += lineItem;
				}
				else
				{
					OPCServerManager.Write(new URL(StationPv.URL), new Opc.Da.ItemValue[] { TerminalCommand });
               TerminalCommand = new ItemValue(StationPv.OPCItemID + ".Terminal Command")
               {
                  Value = Esc + "Y" + System.Convert.ToChar(0x20 + captionLines + optionNumber).ToString() + " "
               };
               TerminalCommand.Value += lineItem;
				}

				optionNumber++;
			}

			OPCServerManager.Write(new URL(StationPv.URL),new ItemValue[] { TerminalCommand });

			MessageTimer = parameters.MenuTimeout;

			CurrentMenuParameters = parameters;
		}

		protected string GetHeaderText()
		{
			string HeaderText = "Varec Terminal Automation";
			int iNumberOfSpaces = ((MaxDisplayLineSize - HeaderText.Length) / 2) + HeaderText.Length;
			HeaderText = HeaderText.PadLeft(iNumberOfSpaces,' ');
			return HeaderText;
		}

		protected override void OpenGate()
		{
            this.IssueSelectGate(); 
		}

        private void Opengate()
        {
            if (string.IsNullOrEmpty(GatePV.URL) == false)
            {
                try
                {
                    GatePV.ServerValue = (bool)true;
                    OPCServerManager.Write(GatePV);
                    //sijuan: per Jon, if BOL station is configured with Exit Gate, there will be not other Exit Gate station is enabled. 
                    if (this.Station.Type == STATION_TYPE.EXIT_GATE || this.Station.Type == STATION_TYPE.BOL)
                        CardOut();
                    StationState = StationState.OPENING_GATE;
                    DisplayMessage("LoadRack|Opening Gate", null, 0, 0);
                    GateTimer = 10;
                }
                catch (Exception e)
                {
                    eventLog.WriteEntry(e.ToString(), EventLogEntryType.Error);
                    StationState = StationState.RESET_ON_TIMEOUT;
                    DisplayMessage("LoadRack|Gate Open Failure", null, 0, MESSAGE_TIMEOUT);
                }
            }
            else
                ResetStationDevice();
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

				if (menu.Count == 1)
	         {
                this.Opengate();
                return;
            }
            if (menu.Count == 0)
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
                foreach (ProcessVariableClass ProcessVariable in Station.ProcessVariableCollection)
                {
                    if (ProcessVariable.ProcessVariableType != PROCESS_VARIABLE_TYPE.GATE_CONTROL_PV)
                    {
                        continue;
                    }
                    else if (ProcessVariable.MessageID == response)
                    {
                        GatePV = ProcessVariable;
                        this.Opengate();
                        return;
                    }
                }
            }
        }


		protected override void  EntryGateProcessing(ProcessVariableClass pv)
		{
			OnInvokeProcessing(pv);
		}

		protected override void ExitGateProcessing(ProcessVariableClass pv)
		{
            OnInvokeProcessing(pv);
		}

		protected override void BolProcessing(ProcessVariableClass pv)
		{
			OnInvokeProcessing(pv);
		}

		protected override void PreloadProcessing(ProcessVariableClass pv)
		{
			OnInvokeProcessing(pv);
		}

		protected override void WeightScaleProcessing(ProcessVariableClass pv)
		{
			OnInvokeProcessing(pv);
		}

        public override void UploadStoredTransactions()
        {
            throw new NotImplementedException();
        }

        public override bool SetDownloadDensityInUnitFlag(string density)
        {
            throw new NotImplementedException();
        }

		protected void OnInvokeProcessing(ProcessVariableClass PV)
		{
			Monitor.Enter(this);
			try
			{
				switch(PV.ProcessVariableType)
				{
					case PROCESS_VARIABLE_TYPE.STATION_PV:
					{
						if(PV.IsQualityGood)
						{
								if (StationState != StationState.IDLE)
								{
									if (System.Convert.ToChar(PV.ServerValue) == '3')
									{
										Item terminatingKey = new Item(new ItemIdentifier(StationPv.OPCItemID + ".Terminating Key"));
										ItemValueResult[] values = OPCServerManager.Read(new URL(StationPv.URL), new Opc.Da.Item[] { terminatingKey });
										if (values[0].ResultID == ResultID.S_OK)
										{
											// Exit
											if (values[0].Value as string == "C")
											{
												CurrentMenuParameters = null;
												ProcessResponseData(EscapeString);
											}

											// Enter
											else if (values[0].Value as string == "D")
											{
												Item keypadData = new Item(new ItemIdentifier(StationPv.OPCItemID + ".Keypad Data"));
												values = OPCServerManager.Read(new URL(StationPv.URL), new Item[] { keypadData });
												if (values[0].ResultID == ResultID.S_OK)
												{
													if (CurrentMenuParameters != null)
													{
														try
														{
															int index = System.Convert.ToInt32(values[0].Value as string) + menuPage * (MaxLines - captionLines);

															if (index == 0)
																values[0].Value = EscapeString;

															else if (index < 0
															|| index > CurrentMenuParameters.Menu.Length)
															{
																DisplayMessage("[LoadRack|Invalid Selection]", null, 0, MESSAGE_TIMEOUT);
																break;
															}

															else
															{
																values[0].Value = this.GetDataDictionaryValueByKey(SiteManager.Site.SiteGuid, CurrentMenuParameters.Menu[index - 1]);
															}

															CurrentMenuParameters = null;
														}
														catch (Exception)
														{
															DisplayMessage("[LoadRack|Invalid Selection]", null, 0, MESSAGE_TIMEOUT);
															break;
														}
													}
													// Sijuan: since there is no "?" key, use "#" for "?"
													if ((values[0].Value as string).TrimEnd(new char[] { ' ' }) == "#")
														values[0].Value = "?";
													ProcessResponseData((values[0].Value as string).TrimEnd(new char[] { ' ' }));
												}
											}

											// Next
											else if (values[0].Value as string == "A")
											{
												if (CurrentMenuParameters != null)
												{
													if ((menuPage + 1) * (MaxLines - captionLines) < CurrentMenuParameters.Menu.Length)
														menuPage++;

													DisplayMenu(CurrentMenuParameters);
												}
												else
													ProcessResponseData("?");
											}

											// Previous
											else if (values[0].Value as string == "B")
											{
												if (CurrentMenuParameters != null)
												{
													if (menuPage > 0)
														menuPage--;

													DisplayMenu(CurrentMenuParameters);
												}
												else
													ProcessResponseData("?");
											}

											else if (values[0].Value as string == "F")
											{
												ResetStationDevice();
											}
										}
									}
								}
								else if (!Station.CardReader)
								{
									ResetStationDevice();
								}
						}

						break;
					}


					case PROCESS_VARIABLE_TYPE.CARDREADER_PV:
					{
						if (PV.IsQualityGood)
						{
								if (System.Convert.ToChar(PV.ServerValue) == '1')
								{
									// System must be prepared for Card In
									if (StationState != StationState.IDLE)
									{
										break;
									}


									Item cardNumber = new Item(new ItemIdentifier(StationPv.OPCItemID + ".Card Number"));
									ItemValueResult[] values = OPCServerManager.Read(new URL(StationPv.URL), new Opc.Da.Item[] { cardNumber });
									if (values[0].ResultID == ResultID.S_OK)
									{
										// Test for Prox Cards 
										string[] cardData = (values[0].Value as string).Split(new char[] { '=' });
										if (cardData != null
										&& cardData.Length == 5)
										{
											ProcessDriverID(cardData[3]);
										}

										else
										{
											// Text for TWIC Cards
											cardData = (values[0].Value as string).Split(new char[] { ',' });
											if (cardData != null
											&& cardData.Length == 6)
											{
												ProcessDriverID(cardData[5].Substring(0, 10));
											}
											else
											{
												throw new Exception("RCU II Rcu Protocol Station Manager, Unrecognized Card Data - " + values[0].Value);
											}
										}
									}
								}
								else
								{
									ResetStationDevice();
								}
						}                       
						break;
					}                   

					default:
						break;
				}
			}
			catch(OpcException e)
			{
                eventLog.WriteEntry("RcuIIProtocolStationManager OnInvoke : " + e.Message, EventLogEntryType.Error);
				CommunicationsFailure = true;
			}
			catch(Exception e)
			{
                eventLog.WriteEntry("RcuIIProtocolStationManager OnInvoke : PV = " + PV.OPCItemID + " " + e.ToString(), EventLogEntryType.Error);
			}

			finally
			{
				Monitor.Exit(this);
			}
		}

      public override void ReadWeight(out ItemValueResult weight, out bool weightScaleInMotion, out bool weightScaleMotionReadingInValid)
      {
         weightScaleInMotion = false;
         weightScaleMotionReadingInValid = false;
         foreach (ProcessVariableClass pv in Station.ProcessVariableCollection)
         {
            if (pv.ProcessVariableType == PROCESS_VARIABLE_TYPE.WEIGHT_SCALE_PV)
            {
               string tagPrefix = pv.OPCItemID + ".";

               Item[] items = { new Item(new ItemIdentifier(tagPrefix + "Weight")) };
               Item[] itemMotions = { new Item(new ItemIdentifier(tagPrefix + "Scale In Motion")) };

               try
               {
                  var server = new Opc.Da.Server(new OpcCom.Factory(), new URL(pv.URL));
                  System.Net.NetworkCredential credentials = null;
                  server.Connect(new ConnectData(credentials));
                  ItemValueResult[] values = server.Read(items);
                  ItemValueResult[] motionValue = server.Read(itemMotions);

                  server.Disconnect();
                  server.Dispose();

                  weight = values[0];

                  if (motionValue[0].ResultID == ResultID.S_OK)
                  {
                     if (motionValue[0].Quality == Quality.Good)
                     {
                        weightScaleInMotion = System.Convert.ToBoolean(motionValue[0].Value);
                     }
                     else
                     {
                        weightScaleMotionReadingInValid = true;
                     }
                  }
               }
               catch
               {
                  weight = new ItemValueResult(new ItemIdentifier(tagPrefix + "Weight"), new ResultID(Quality.Bad.GetCode()));
                  eventLog.WriteEntry("Read : Connection error to " + pv.URL, EventLogEntryType.Error);
               }

               return;
            }
         }

         weight = new ItemValueResult();
      }
   }
}
