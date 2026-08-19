/******************************************************************************

	FILE NAME:		Contrec1010RALoadArmManagerClass.cs


	PURPOSE:			Contrec1010RALoadArmManagerClass


	COMMENTS:

		Copyright (C) E+H Systems & Gauging, Inc. Norcross, GA, USA, 2000

		This file shall not be copied or reproduced in any form without
				the express written consent of Endress+Hauser.


	AUTHOR(S):	B. Schaal


	VERSION:		1.0.0  Current version



	MODIFICATION HISTORY:
		Date:			By:			Reason:
		---------	----------  -------------------------------------------
*******************************************************************************/
using Opc;
using Opc.Da;

using System;
using System.Collections;
using System.Diagnostics;
using System.Net;
using System.Runtime.InteropServices;
using System.Threading;

using FMBusinessObjects.BusinessInterfaces;
using FMBusinessObjects.ChannelFactories;
using FMBusinessObjects.DataObjects;

namespace LoadRackLibrary
{
	class Contrec1010RaLoadArmManagerClass : LoadArmManagerClass
	{
		protected ProcessVariableClass CommandFieldPV;
		protected ProcessVariableClass Arm1Status;
		protected ProcessVariableClass BatchInProgressPV;
		protected ProcessVariableClass ContrecStatusPV;
		protected double SelectedPresetQuantity = 0.0;
		protected int iContrecTransActionNumber = 0;
		protected int SelectedCompartment;
		protected bool EmergencyStopPressed = false;

		public Contrec1010RaLoadArmManagerClass(
			EventLog EventLog,
			SiteManagerClass SiteManager,
			StationManagerClass StationManager,
			LoadArmClass LoadArm,
			SecurityClass Security)
			: base(EventLog, SiteManager, StationManager, LoadArm, Security)
		{

			string LoadArmURL = LoadArm.ProcessVariableCollection[0].URL;
			string LoadArmProgID = LoadArm.ProcessVariableCollection[0].ProgID;
			string LoadArmOPCItemID = LoadArm.ProcessVariableCollection[0].OPCItemID;

		    this.iContrecTransActionNumber = 1;
		    this.SelectedCompartment = 0;

		    this.CommandFieldPV = new ProcessVariableClass(
				PROCESS_VARIABLE_TYPE.LOAD_ARM_STATE_PV,
				UNIT_TYPE.LOADARM_UNIT,
				VarEnum.VT_BSTR,
				true,
				LoadArmOPCItemID + ".Command Field",
				LoadArmURL,
				LoadArmProgID);

		    this.Arm1Status = new ProcessVariableClass(
				PROCESS_VARIABLE_TYPE.LEVEL_PV,
				UNIT_TYPE.LOADARM_UNIT,
				VarEnum.VT_BSTR,
				true,
				LoadArmOPCItemID + ".Arms.Arm 1.Arm Status",
				LoadArmURL,
				LoadArmProgID);

		    this.BatchInProgressPV = new ProcessVariableClass(
				PROCESS_VARIABLE_TYPE.BATCH_IN_PROGRESS_PV,
				UNIT_TYPE.STATION_UNIT,
				VarEnum.VT_I4,
				true,
				LoadArmOPCItemID + ".Status.Batch In Progress",
				LoadArmURL,
				LoadArmProgID);

		    this.ContrecStatusPV = new ProcessVariableClass(
				PROCESS_VARIABLE_TYPE.TRANSACTION_IN_PROGRESS_PV,
				UNIT_TYPE.LOADARM_UNIT,
				VarEnum.VT_I4,
				true,
				LoadArmOPCItemID + ".Status.1010 System Status",
				LoadArmURL,
				LoadArmProgID);

		    this.OpcServerManager.AddProcessVariable(this.CommandFieldPV);
		    this.OpcServerManager.AddProcessVariable(this.Arm1Status);
		    this.OpcServerManager.AddProcessVariable(this.BatchInProgressPV);
		    this.OpcServerManager.AddProcessVariable(this.ContrecStatusPV);
		}

		// required abstract members
		public override void Start()
		{
		}

		public override void Stop()
		{
		}

		public override bool AllocateRecipes(ulong recipeMap, ulong extendedRecipeMap)
		{
			return true;
		}

		public override void Unauthorize()
		{
			StationManagerClass stationManager = this.GetStationManager();
			if (stationManager == null)
				return;

			if (this.LoadArmState == LOADARM_STATE.INPROGRESS) this.Stop();

		    this.SetState(LOADARM_STATE.NORMAL);
		    this.CurrentLineItem = null;
		}

        /// <summary>
        /// Applies the additive profile to the preset device and updates the display name of the recipe
		/// 
		/// Does nothing as additives are not currently supported on Contrec 1010
        /// </summary>
        /// <param name="name">
        /// Recipe name to be displayed
        /// </param>
        /// <param name="recipe">
        /// Recipe to be updated
        /// </param>
        /// <param name="product">
        /// Blend or Component definition for the recipe
        /// </param>
        /// <param name="additiveProfile">
        /// Additive Profile to apply to this recipe for the current ShipTo
        /// </param>
        /// <param name="deviceRecipeNumber">
        /// Internal recipe identifier on the preset
        /// </param>
        /// <returns>
        /// True
        /// </returns>
        public override bool UpdateRecipe(
			string name,
			ProductMapClass recipe,
            ProductClass product,
			AdditiveProfileClass additiveProfile,
			int deviceRecipe)
		{
			return true;
		}

		public override int DisplayMessage(string message, int responseLength, int messageTimeout)
		{
			StationManagerClass StationManager = this.GetStationManager();
			if (StationManager == null)
				throw new OpcException("No Station Available");

			ProcessVariableClass LoadArmPV = this.LoadArm.ProcessVariableCollection[0];

			ArrayList Items = new ArrayList();

		    this.SetMessageTimeout(messageTimeout);

			// use the get answer prompt to get the response
			string Prompt = "";
			Prompt = this.FormatContrecPrompt(message, true);
			ItemValue DisplayMessageItemValue = new ItemValue(LoadArmPV.OPCItemID + ".Get Answer Prompt");
			DisplayMessageItemValue.Value = Prompt;
			Items.Add(DisplayMessageItemValue);

		    this.OpcServerManager.Write(new URL(LoadArmPV.URL), (ItemValue[])Items.ToArray(typeof(ItemValue)));

		    this.ResponsePending = true;

			return 0;
		}

		public override void DisplayMenu(DisplayMenuParameters parameters)
		{
			StationManagerClass StationManager = this.GetStationManager();
			if (StationManager == null)
				throw new OpcException("No Station Available");

			ProcessVariableClass LoadArmPV = this.LoadArm.ProcessVariableCollection[0];

			ArrayList Items = new ArrayList();

		    this.ResponsePending = true;

			// use the get answer prompt to get the response
			string Prompt = "";
			Prompt = this.FormatContrecMenuPrompt(parameters);
			ItemValue DisplayMessageItemValue = new ItemValue(LoadArmPV.OPCItemID + ".Get Answer Prompt");
			DisplayMessageItemValue.Value = Prompt;
			Items.Add(DisplayMessageItemValue);

		    this.OpcServerManager.Write(new URL(LoadArmPV.URL), (ItemValue[])Items.ToArray(typeof(ItemValue)));

		    this.CurrentMenuParameters = parameters;
			return;
		}

		public override bool IsTransactionInProgress()
		{
			StationManagerClass StationManager = this.GetStationManager();
			if (StationManager == null)
				return false;
			return false;
		}


		// specific contrec members
		public void SetMessageTimeout(int MessageTimeoutValue)
		{
			ProcessVariableClass LoadArmPV = this.LoadArm.ProcessVariableCollection[0];
			ProcessVariableClass MessageTimeout = new ProcessVariableClass();
			MessageTimeout.URL = LoadArmPV.URL;
			MessageTimeout.OPCItemID = LoadArmPV.OPCItemID + ".Set Message Time-out";
			MessageTimeout.ServerValue = MessageTimeoutValue;
		    this.OpcServerManager.Write(MessageTimeout);
		}

		public string FormatContrecPrompt(string StockMessage, bool bUseAnswerPrompt)
		{
			string Prompt = "";
			int iLoop;
			int iNumberofCharacters = 0;
			bool bStart = false;
			bool bLookForSpace = false;
			bool bLastCharacterWasSpace = false;

			// we need to reformat the message. The Contrec supports eight lines at 30 characters and we use the '|' as the line seperator
			// we need to remove the [] from the message
			Prompt = "|Varec Terminal Automation|";
			if (bUseAnswerPrompt)
				Prompt += "|";
			for (iLoop = 0; iLoop < StockMessage.Length; iLoop++)
			{
				if (iNumberofCharacters > 20)
				{
					bLookForSpace = true;
				}
				if (bLookForSpace && StockMessage[iLoop] == ' ')
				{
					bLookForSpace = false;
					iNumberofCharacters = 0;
					Prompt += '|';
				}
				if (StockMessage[iLoop] == '|')
					bStart = true;
				else if (StockMessage[iLoop] == ']' || StockMessage[iLoop] == '[')
				{
					bStart = false;
					if (bLastCharacterWasSpace == false)
					{
						Prompt += " ";
						++iNumberofCharacters;
					}
					bLastCharacterWasSpace = true;
				}
				else if (bStart)
				{
					if (bLastCharacterWasSpace &&
						StockMessage[iLoop] == ' ')
					{
					}
					else
					{
						bLastCharacterWasSpace = false;
						Prompt += StockMessage[iLoop];
						++iNumberofCharacters;
					}
				}
			}

			return Prompt;
		}

		public string FormatContrecMenuPrompt(DisplayMenuParameters Parameters)
		{
			StationManagerClass StationManager = this.GetStationManager();
			if (StationManager == null)
				throw new OpcException("No Station Available");

		    this.SetMessageTimeout(Parameters.MenuTimeout);

			// the menu can consist of a header and up to 6 menu options.
			ProcessVariableClass LoadArmPV = this.LoadArm.ProcessVariableCollection[0];

			string Message = "|Varec Terminal Automation|";
			int iMaximumLength = 0;
			Message += FMChannelHelper.MakeCall<IDataDictionariesClass, string>(
																	 x =>
																	 x.Get(StationManager.Station.SiteGuid, Parameters.Caption)
																);

			int iCommandNumber;

			// first determine the maximum length of the menus
			for (int nLoop = 0; nLoop < Parameters.Menu.Length; ++nLoop)
			{
				string Value = Parameters.Menu[nLoop];

				if (Parameters.ApplyDataDictionary)
					Value = this.GetDataDictionaryValueByKey(StationManager.Station.SiteGuid, Value);

				if (Value.Length /*+ 4*/ > iMaximumLength)
					iMaximumLength = Value.Length /*+ 4*/;
			}

			for (int nLoop = 0; nLoop < Parameters.Menu.Length; ++nLoop)
			{
				string Value = Parameters.Menu[nLoop];

				if (Parameters.ApplyDataDictionary)
					Value = this.GetDataDictionaryValueByKey(StationManager.Station.SiteGuid, Value);

				if (Value.Length /*+ 4*/ < iMaximumLength)
				{
					for (int nLoop1 = 0; nLoop1 < (iMaximumLength - (Value.Length /*+ 4*/)); nLoop1++)
					{
						Value += " ";
					}
				}
				iCommandNumber = nLoop + 1;
				Message += '|';
				Message += iCommandNumber.ToString() + " - ";
				Message += Value;
			}

			return Message;
		}

		private string GetDataDictionaryValueByKey(Guid guid, string key)
		{
			return FMChannelHelper.MakeCall<IDataDictionariesClass, string>(
																	 x =>
																	 x.Get(guid, key)
																);
		}

		public override void ReleaseKeyPad()
		{
		}

		public override bool IssueSelectRecipePrompt(StationManagerClass StationManager, double MaximumPreset)
		{
			this.MaximumPreset = MaximumPreset;

			int NumberOfAuthorizedRecipes = 0;
			for (int Index = 0; Index < this.LoadArm.ProductRecipeCollection.Count; Index++)
			{
				if ((this.Bay(StationManager).RecipeMap & ((ulong)1 << Index)) != 0)
				{
					NumberOfAuthorizedRecipes++;
				}
			}

			if (NumberOfAuthorizedRecipes <= 0)
				return false;

			string[] Menu = new string[NumberOfAuthorizedRecipes];

			NumberOfAuthorizedRecipes = 0;
			for (int Index = 0; Index < this.LoadArm.ProductRecipeCollection.Count; Index++)
			{
				if ((this.Bay(StationManager).RecipeMap & ((ulong)1 << Index)) != 0)
				{
					//ProductMapClass Recipe = LoadArm.ProductRecipeCollection..Item(Index);
					ProductMapClass Recipe = this.LoadArm.ProductRecipeCollection[Index];
					Menu[NumberOfAuthorizedRecipes] = StationManager.GetLoadRackDisplayText(Recipe.AssignedGuid);
					NumberOfAuthorizedRecipes++;
				}
			}

			DisplayMenuParameters Parameters = new DisplayMenuParameters("LoadRack|Select Recipe", Menu, false, -1, this.PromptTimeout);

		    this.SetState(LOADARM_STATE.SELECT_RECIPE_PROMPT);

		    this.DisplayMenu(Parameters);

			return true;
		}

		protected override void ProcessSelectRecipeResponse(StationManagerClass StationManager, string Response)
		{
			int nSelection = 0;
			bool RecipeFound = false;
			Contrec1010RAStationManagerClass LocalStationManager = (Contrec1010RAStationManagerClass)this.GetStationManager();
			if (LocalStationManager == null)
				return;

			if (Response == StationManagerClass.EscapeString || Response == "0")
			{
				if (this.SiteManager.Site.PromptForShipmentNumber)
					LocalStationManager.IssueEnterShipmentNumberPrompt();
				else
					LocalStationManager.IssueLoadIDPrompt();

				return;
			}

			try
			{
				nSelection = System.Convert.ToInt32(Response);
			}
			catch
			{
			    this.IssueRecipeInvalidMessage();
				return;
			}

			if (this.CurrentMenuParameters == null
			|| nSelection > this.CurrentMenuParameters.Menu.Length)
			{
			    this.IssueRecipeInvalidMessage();
				return;
			}

			Response = this.CurrentMenuParameters.Menu[nSelection - 1];


			foreach (ProductMapClass Recipe in this.LoadArm.ProductRecipeCollection)
			{
				if (Recipe.AssignedID == Response)
				{
				    this.CurrentRecipe = Recipe;
					RecipeFound = true;
					break;
				}
			}

			if (!RecipeFound)
			{
			    this.IssueRecipeInvalidMessage();
				return;
			}

			// this will cause the Contrec to take over from this point
		    this.IssueLoadNumberTransactiont();


		}

		public override bool IssuePresetPrompt(StationManagerClass StationManager, double Preset)
		{
			Contrec1010RAStationManagerClass LocalStationManager = (Contrec1010RAStationManagerClass)this.GetStationManager();
			if (LocalStationManager == null)
				return false;

		    this.SetState(LOADARM_STATE.PRESET_VOLUME_PROMPT);

		    this.DisplayMessage("[LoadRack|Enter] " + "|" + " [LoadRack|Preset]", 10, this.PromptTimeout);

		    this.ResponsePending = true;

			return true;
		}

		protected override bool IssueCompartmentPrompt(StationManagerClass stationManager)
		{
			return base.IssueCompartmentPrompt(stationManager);
		}

		public override void ProcessPresetResponse(StationManagerClass StationManager, string Response)
		{
			if (Response == StationManagerClass.EscapeString)
			{
				if (!this.IssueSelectRecipePrompt(StationManager, this.MaximumPreset))
				{
				    this.IssueSelectPrompt(StationManager);
				}
				return;
			}
			try
			{
				if (Response.ToUpper() == "YES")	// user just pressed enter so use the maximum
					Response = this.MaximumPreset.ToString();
			    this.SelectedPresetQuantity = System.Convert.ToDouble(Response);
			}
			catch
			{
			    this.IssuePresetInvalidMessage();
				return;
			}

			if (this.CurrentPreset > this.MaximumPreset)
			{
			    this.IssuePresetInvalidMessage();
				return;
			}

			if (StationManager.SiteManager.Site.PromptForReturns)
			{
				StationManager.IssuePromptForReturnsPrompt();
			}
			else
			{
			    this.IssueLoadNumberTransactiont();
				Thread.Sleep(1000);
			    this.SetPresetValue();
			}
		}

		public void SetPresetValue()
		{
			double dPresetMax = 0.0;
			StationManagerClass StationManager = this.GetStationManager();
			if (StationManager == null)
				return;

			ProcessVariableClass LoadArmPV = this.LoadArm.ProcessVariableCollection[0];
			ProcessVariableClass PresetParameters = new ProcessVariableClass();
			PresetParameters.URL = LoadArmPV.URL;
			PresetParameters.OPCItemID = LoadArmPV.OPCItemID + ".Compartment Response";

			if (StationManager.Station.Type == STATION_TYPE.OFF_LOADING)
				dPresetMax = 9999999.0;
			else
				dPresetMax = this.MaximumPreset;

			PresetParameters.ServerValue = this.SelectedPresetQuantity.ToString() + "@" + dPresetMax.ToString();

		    this.OpcServerManager.Write(PresetParameters);

		}

		public void IssueLoadNumberTransactiont()
		{
			StationManagerClass StationManager = this.GetStationManager();
			if (StationManager == null)
				return;

			// since this is asyncronous and once the authorized bit is set the contrec goes under it own control
			// we need to check them here and not authorize the contrec if they are not set to true
			if (!this.CheckLoadArmPermissives(StationManager))
			{
				return;
			}

			StationManager.UpdatePermissives(true);
			if (StationManager.StationState == StationState.RESET_ON_TIMEOUT)
			{
				// turn anything off that we may of turned on
				StationManager.UpdatePermissives(false);
				return;
			}


			StationManager.StationState = StationState.AUTHORIZED;

		    this.ClearContrecDisplay();
			ProcessVariableClass LoadArmPV = this.LoadArm.ProcessVariableCollection[0];
			ProcessVariableClass LoadNumberTransaction = new ProcessVariableClass();
			LoadNumberTransaction.URL = LoadArmPV.URL;
			LoadNumberTransaction.OPCItemID = LoadArmPV.OPCItemID + ".Load Number Response";

			LoadNumberTransaction.ServerValue = this.iContrecTransActionNumber.ToString();
			++this.iContrecTransActionNumber;

		    this.OpcServerManager.Write(LoadNumberTransaction);
		    this.Authorize(StationManager, this.CurrentPreset);
			return;
		}

		public void ClearContrecDisplay()
		{
			Contrec1010RAStationManagerClass StationManager = (Contrec1010RAStationManagerClass)this.GetStationManager();
			if (StationManager == null)
				throw new OpcException("No Station Available");

			ProcessVariableClass LoadArmPV = this.LoadArm.ProcessVariableCollection[0];

			ProcessVariableClass ClearDisplay = new ProcessVariableClass();
			ClearDisplay.URL = LoadArmPV.URL;
			ClearDisplay.OPCItemID = LoadArmPV.OPCItemID + ".Clear Display";
			ClearDisplay.ServerValue = true;
		    this.OpcServerManager.Write(ClearDisplay);
			StationManager.ReadyToLoad = true;
		}

		public override bool Authorize(StationManagerClass stationManager, double preset)
		{
		    this.SetState(LOADARM_STATE.AUTHORIZED);
			return true;
		}

		public override void CaptureMeterValues()
		{
			ProcessVariableClass LoadArmPV = this.LoadArm.ProcessVariableCollection[0];

			Opc.Da.Server Server = new Opc.Da.Server(new OpcCom.Factory(), new URL(LoadArmPV.URL));
			NetworkCredential Credentials = null;
			Server.Connect(new ConnectData(Credentials));

			ItemValueResult[] NonResettableTotal;

		    this.ReadNonResettableTotals(
				Server,
				out NonResettableTotal);


			// Component
			if (NonResettableTotal[0].Quality != Quality.Good) this.eventLog.WriteEntry("CaptureMeterValues : Non-Resettable Gross Volume OPC Quality Bad " + NonResettableTotal[0].ItemName, EventLogEntryType.Error);
			else
				//LoadArm.ComponentCollection.Item(0).MeterValue = System.Convert.ToDouble(NonResettableTotal[0].key);
			    this.LoadArm.ComponentCollection[0].MeterValue = System.Convert.ToDouble(NonResettableTotal[0].Value);

			// Additives
			int Item = 0;
			foreach (ProductMapClass AdditiveInjector in this.LoadArm.AdditiveInjectorCollection)
			{
				Item++;
				if (NonResettableTotal[Item].Quality != Quality.Good) this.eventLog.WriteEntry("CaptureMeterValues : Non-Resettable Gross Volume OPC Quality Bad " + NonResettableTotal[Item].ItemName, EventLogEntryType.Error);
				else
					AdditiveInjector.MeterValue = System.Convert.ToDouble(NonResettableTotal[Item].Value);
			}

			Server.Disconnect();
			Server.Dispose();
		}

		public void ReadNonResettableTotals(
			Opc.Da.Server Server,
			out ItemValueResult[] NonResettableGrossVolumes)
		{
			Contrec1010RAStationManagerClass StationManager = (Contrec1010RAStationManagerClass)this.GetStationManager();

			ProcessVariableClass LoadArmPV = this.LoadArm.ProcessVariableCollection[0];

			ArrayList Items = new ArrayList();

			for (int iArm = 1; iArm <= StationManager.NumberOfArmsConfigured; iArm++)
			{
				string TagPrefix = LoadArmPV.OPCItemID + ".Arm Totals.Arm " + iArm.ToString();

				string OPCTagPrefix = TagPrefix + ".Accumulated Gross Volume";

				Items.Add(new Item(new ItemIdentifier(OPCTagPrefix)));
			}

			foreach (Item item in Items)
				item.MaxAgeSpecified = true;

			((Item)Items[0]).MaxAgeSpecified = false;

			ItemValueResult[] Values = Server.Read((Item[])Items.ToArray(typeof(Item)));

			NonResettableGrossVolumes = Values;
		}

		protected override void OnInvoke(ProcessVariableClass pv)
		{
			Contrec1010RAStationManagerClass StationManager = (Contrec1010RAStationManagerClass)this.GetStationManager();
			if (StationManager == null)
			{
				return;
			}
			Monitor.Enter(StationManager);

			try
			{
				switch (pv.ProcessVariableType)
				{
					case PROCESS_VARIABLE_TYPE.LOAD_ARM_STATE_PV:
						{
							if (pv.IsQualityGood)
							{
								if (pv.ServerValue.ToString() == "AA" && this.ResponsePending == true)
								{
									ProcessVariableClass LoadArmPV = this.LoadArm.ProcessVariableCollection[0];

									Item[] Items ={	new Item(new ItemIdentifier(LoadArmPV.OPCItemID+".Entered Data")),
														};

									ItemValueResult[] Values = this.OpcServerManager.Read(new URL(LoadArmPV.URL), Items);
									if (Values[0].Quality == Quality.Good)
									{
										string KeypadData = Values[0].Value.ToString();
										if (KeypadData == "")
											break;
									    this.ResponsePending = false;

										if (this.LoadArmState == LOADARM_STATE.SELECT_PROMPT)
										{
											if (KeypadData[0] == '2') // terminate loading
											{
											    this.ClearContrecDisplay();
											}
										}

										if (KeypadData.ToString().ToUpper() == "YES")
											KeypadData = "Yes";
										else if (KeypadData.ToString().ToUpper() == "NO")
										{
											if (StationManager.StationState == StationState.VERIFY_SHIPTO_MSG ||
												StationManager.StationState == StationState.PROMPT_FOR_RETURNS)
												KeypadData = "No";
											else
												KeypadData = StationManagerClass.EscapeString;
										}

									    this.ProcessResponseData(StationManager, KeypadData);
									}
								}
								else if (pv.ServerValue.ToString() == "RL" &&
									StationManager.StationState == StationState.AUTHORIZED)
								{
								    this.IssueLoadNumberTransactiont();
								}
								else if (pv.ServerValue.ToString() == "RC" &&
									StationManager.StationState == StationState.AUTHORIZED)
								{
								    this.SetPresetValue();
								}
								else if (pv.ServerValue.ToString() == "PL")	// send transaction complete
								{
									StationManager.SetTransactionNumber();
								    this.SendTransactionComplete();
								    this.CheckForSingleLineitem(StationManager);
								}

							}
							break;
						}
					case PROCESS_VARIABLE_TYPE.LEVEL_PV:
						{
							if (pv.IsQualityGood)
							{
								if (pv.ServerValue.ToString() == "Batch Complete" ||
									pv.ServerValue.ToString() == "Batch Error")
								{
									if (pv.ServerValue.ToString() == "Batch Error")
										Thread.Sleep(10000);
									if (this.LoadArmState == LOADARM_STATE.INPROGRESS)
									{
										LineItemDO LineItem = StationManager.GetLineItem(this.LoadArm.IdentityGuid);

										if (LineItem != null)
										{
											StationManager.UpdateLineItem(LineItem);
											StationManager.CloseOutLineItem(LineItem);
											if (StationManager.InRecircMode == false)
												StationManager.SaveTransaction();
										}
									    this.SetState(LOADARM_STATE.FINISHED);
									    this.SendTransactionComplete();
										Thread.Sleep(3000); // delay required to allow nvram to update per contrec
									    this.ClearContrecDisplay();
									}
								}
								else if (pv.ServerValue.ToString() == "Loading")
								{
									if (this.LoadArmState != LOADARM_STATE.INPROGRESS) this.SetState(LOADARM_STATE.INPROGRESS);
								}
							}
							break;
						}
					case PROCESS_VARIABLE_TYPE.BATCH_IN_PROGRESS_PV:
						{
							int iServerIntValue = 0;
							if (pv.IsQualityGood)
							{
								if (pv.ServerValue.ToString() != "1" &&
									pv.ServerValue.ToString() != "2" &&
									pv.ServerValue.ToString() != "3" &&
									pv.ServerValue.ToString() != "4")
									break;
								if (StationManager.StationState == StationState.AUTHORIZING
								|| StationManager.StationState == StationState.AUTHORIZED
								|| StationManager.StationState == StationState.TRANSACTION_IN_PROGRESS)
								{
									iServerIntValue = System.Convert.ToInt32(pv.ServerValue.ToString());
									//									DetermineArmandPresetAmount(iServerIntValue);

									int ArmNumber = this.GetArmNumber(StationManager);
									LineItemDO LineItem = StationManager.GetLineItem(this.LoadArm.IdentityGuid);

									if (LineItem == null)
									{
									    this.SetState(LOADARM_STATE.AUTHORIZED);
										if (this.LoadArmState == LOADARM_STATE.AUTHORIZED
										|| this.LoadArmState == LOADARM_STATE.AUTHORIZED_PERMISSIVE_PROMPT
										|| this.LoadArmState == LOADARM_STATE.PRESET_VOLUME_PROMPT)
										{
										    this.SetState(LOADARM_STATE.INPROGRESS);

											if (StationManager.AddLineItem(this.LoadArm.IdentityGuid) != null &&
												StationManager.InRecircMode == false)
												StationManager.SaveTransaction();
										}
									}
									else if (this.LoadArmState != LOADARM_STATE.INPROGRESS) this.SetState(LOADARM_STATE.INPROGRESS);
								}
							}
							break;
						}
					case PROCESS_VARIABLE_TYPE.TRANSACTION_IN_PROGRESS_PV:
						{
							if (pv.IsQualityGood)
							{
								int iTempValue = System.Convert.ToInt32(pv.ServerValue);

								if ((iTempValue & 0x40) == 0x40) this.EmergencyStopPressed = true;
								else if ((iTempValue & 0x40) != 0x40 && this.EmergencyStopPressed == true)
								{
								    this.EmergencyStopPressed = false;
									StationManager.ResetStationDevice();
								}
								if ((iTempValue & 0x80) == 0 &&
									StationManager.StationState != StationState.IDLE &&
									StationManager.StationState != StationState.ENTER_ORDER_PROMPT)
								{
									if (this.LoadArmState != LOADARM_STATE.SELECT_PROMPT)
									{
										StationManager.ResetStationDevice();
									}
								}
							}
							break;
						}
					default:
						base.OnInvoke(pv);
						break;
				}
			}
			catch (OpcException e)
			{
			    this.eventLog.WriteEntry("Contrec LoadArmManager OnInvoke : " + e.Message, EventLogEntryType.Error);
			}

			catch (Exception e)
			{
			    this.eventLog.WriteEntry("Contrec LoadArmManager OnInvoke : PV Type = " + pv.ProcessVariableType.ToString() + " " + e.ToString(), EventLogEntryType.Error);
			}
			finally
			{
				Monitor.Exit(StationManager);
			}
		}

		public void SendTransactionComplete()
		{
			ProcessVariableClass LoadArmPV = this.LoadArm.ProcessVariableCollection[0];
			ProcessVariableClass TransactionComplete = new ProcessVariableClass();
			TransactionComplete.URL = LoadArmPV.URL;
			TransactionComplete.OPCItemID = LoadArmPV.OPCItemID + ".Transaction Complete";
			TransactionComplete.ServerValue = true;
		    this.OpcServerManager.Write(TransactionComplete);
		    this.SetState(LOADARM_STATE.NORMAL);
		}

		public void CheckForSingleLineitem(Contrec1010RAStationManagerClass StationManager)
		{
		    this.SetFinishedLoading();
		    this.ClearContrecDisplay();
		}

		public void ReadComponentData(
			Opc.Da.Server Server,
			out ItemValueResult GrossVolume,
			out ItemValueResult NetVolume,
			out ItemValueResult AverageTemperature,
			out ItemValueResult AverageDensity)
		{
			StationManagerClass StationManager = this.GetStationManager();
			ProcessVariableClass LoadArmPV = this.LoadArm.ProcessVariableCollection[0];

			string TagPrefix = "";// = LoadArmPV.OPCItemID + ".Arm Totals.Arm " + iLoadArmInUse.ToString();
			int ArmNumber = this.GetArmNumber(StationManager);
			if (ArmNumber == 1)
				TagPrefix = LoadArmPV.OPCItemID + ".Arm Totals.Arm " + "1";
			else if (ArmNumber == 2)
				TagPrefix = LoadArmPV.OPCItemID + ".Arm Totals.Arm " + "2";
			else if (ArmNumber == 3)
				TagPrefix = LoadArmPV.OPCItemID + ".Arm Totals.Arm " + "3";
			else if (ArmNumber == 4)
				TagPrefix = LoadArmPV.OPCItemID + ".Arm Totals.Arm " + "4";



			ArrayList Items = new ArrayList();

			Items.Add(new Item(new ItemIdentifier(TagPrefix + ".Batch Gross Volume")));
			Items.Add(new Item(new ItemIdentifier(TagPrefix + ".Batch Net Volume")));
			Items.Add(new Item(new ItemIdentifier(TagPrefix + ".Batch Average Temperature")));
			Items.Add(new Item(new ItemIdentifier(TagPrefix + ".Batch Product Density")));

			foreach (Item item in Items)
				item.MaxAgeSpecified = true;

			((Item)Items[0]).MaxAgeSpecified = false;

			ItemValueResult[] Values = Server.Read((Item[])Items.ToArray(typeof(Item)));

			GrossVolume = Values[0];
			NetVolume = Values[1];
			AverageTemperature = Values[2];
			AverageDensity = Values[3];
		}

		public int GetCompartmentNumber()
		{
			// Validate in the context of the selected equipment
			if (this.SelectedCompartment == 0) this.NonPreloadCompartmentSelection = -1;
			else this.NonPreloadCompartmentSelection = this.SelectedCompartment;
			if (this.NonPreloadEquipmentSelection != "" && this.NonPreloadCompartmentSelection != -1)
			{
				CompartmentInfo Info = this.GetCompartmentIfValid(this.NonPreloadEquipmentSelection, this.SelectedCompartment);
				if (Info != null)
				{
					Info.Loaded = true;
				}
			}
			return this.SelectedCompartment;
		}

		public void DetermineArmandPresetAmount(int iServerIntValue)
		{
			string LoadArmURL = this.LoadArm.ProcessVariableCollection[0].URL;
			string LoadArmProgID = this.LoadArm.ProcessVariableCollection[0].ProgID;
			string LoadArmOPCItemID = this.LoadArm.ProcessVariableCollection[0].OPCItemID;

			ProcessVariableClass LoadArmPV = this.LoadArm.ProcessVariableCollection[0];

			Item[] Items ={new Item(new ItemIdentifier(LoadArmPV.OPCItemID+".Arms.Arm 1.Preset Amount")),
				new Item(new ItemIdentifier(LoadArmPV.OPCItemID+".Arms.Arm 1.Arm Status"))
				};

			ItemValueResult[] Values = this.OpcServerManager.Read(new URL(LoadArmPV.URL), Items);

			Contrec1010RAStationManagerClass StationManager = (Contrec1010RAStationManagerClass)this.GetStationManager();
			if (StationManager == null)
				return;

			int ArmNumber = this.GetArmNumber(StationManager);
			// determine the load arms currently being used
			if ((iServerIntValue & 0x01) != 0)
			{
			}

		    this.CurrentPreset = System.Convert.ToInt32(Values[0].Value.ToString());

		}

		public override void ReadBatchRecipe(
			string BatchNumber,
			Opc.Da.Server Server,
			out ItemValueResult Recipe)
		{
			ItemValueResult Value = new ItemValueResult();

			Value.Quality = Quality.Good;
			Value.Value = this.CurrentRecipe.PresetNumber;

			Recipe = Value;
		}

		public override void ReadPresetAmount(
		Opc.Da.Server Server,
		out ItemValueResult PresetAmount)
		{
			ProcessVariableClass LoadArmPV = this.LoadArm.ProcessVariableCollection[0];

			Item[] Items = { new Item(new ItemIdentifier(LoadArmPV.OPCItemID + ".Arms.Arm 1.Preset Amount")) };

			ItemValueResult[] Values = Server.Read(Items);

			PresetAmount = Values[0];

		}

		protected bool CheckLoadArmPermissives(StationManagerClass StationManager)
		{
			int retrycounter = 3;
			foreach (ProcessVariableClass PV in this.LoadArm.LoadArmPermissives.Inputs)
			{
				switch (PV.ProcessVariableType)
				{
					case PROCESS_VARIABLE_TYPE.INPUT_PERMISSIVE_PV:
						{
							PermissivesClass Permissives = PV.Parent as PermissivesClass;
							if (Permissives == null)
								break;

							Permissives.Update();

							retrycounter = 3;
							while (retrycounter > 0)
							{
							    this.OpcServerManager.Update(true);
								if (PV.IsQualityGood)
									break;
								--retrycounter;
								Thread.Sleep(100);
							}

							if (!PV.IsQualityGood
							|| !((bool)PV.ServerValue))
							{
							    this.IssueContrecPermissiveMessage(StationManager);
								return false;
							}

							break;
						}

					default:
				        this.eventLog.WriteEntry("StationManager OnInvoke : Unknown PV : " + PV.OPCItemID);
						break;
				}
			}
			// check the station input permissives

			foreach (ProcessVariableClass PV in StationManager.Station.StationPermissives.Inputs)
			{
				switch (PV.ProcessVariableType)
				{
					case PROCESS_VARIABLE_TYPE.INPUT_PERMISSIVE_PV:
						{
							PermissivesClass Permissives = PV.Parent as PermissivesClass;
							if (Permissives == null)
								break;

							Permissives.Update();

							retrycounter = 3;
							while (retrycounter > 0)
							{
							    this.OpcServerManager.Update(true);
								if (PV.IsQualityGood)
									break;
								--retrycounter;
								Thread.Sleep(100);
							}

							if (!PV.IsQualityGood
							|| !((bool)PV.ServerValue))
							{
							    this.IssueContrecPermissiveMessage(StationManager);
								return false;
							}

							break;
						}

					default:
				        this.eventLog.WriteEntry("StationManager OnInvoke : Unknown PV : " + PV.OPCItemID);
						break;
				}
			}
			return true;
		}

		public void IssueContrecPermissiveMessage(StationManagerClass StationManager)
		{
			if (StationManager == null)
				return;

			string Message = this.GetPermissiveMessage(StationManager);

			if (Message != "")
			{
				// format the message for the contrec
			    this.DisplayMessage("[LoadRack|" + Message + "] " + " " + StationManager.AcknowledgementMessage, StationManager.AcknowledgementResponseLength, 999);
			    this.SetState(LOADARM_STATE.AUTHORIZED_PERMISSIVE_PROMPT);
			}
		}

		public override void ProcessPermissiveMessageAcknowledge(StationManagerClass stationManager, string Response)
		{
			if (Response == StationManagerClass.EscapeString || Response == "0")
			{
				stationManager.ResetStationDevice();
				return;
			}
			// make sure the permissives have been cleared
			if (!CheckLoadArmPermissives(stationManager))
				return;
			stationManager.StationState = StationState.AUTHORIZED;

			Authorize(stationManager, CurrentPreset);
		}

	}
}
