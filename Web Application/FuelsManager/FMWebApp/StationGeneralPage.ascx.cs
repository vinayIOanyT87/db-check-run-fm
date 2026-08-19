/******************************************************************************
	FILE NAME:		StationGeneralPage.ascx.cs

	PURPOSE:			Implementation of StationGeneralPage

	COMMENTS:

		Copyright (C) E+H Systems & Gauging, Inc. Norcross, GA, USA, 2002

		This file shall not be copied or reproduced in any form without
				the express written consent of Endress+Hauser.

	AUTHOR(S):	W. Gray

	VERSION:		7.4.0.0  Current version

	MODIFICATION HISTORY:
		Date:		By:			Reason:
		---------	----------  -------------------------------------------
		2007-10-11  A. Sang				Added code to handle the new Preload station type
	
		2008-04-16	C. Knight	7.4.0.0 - Added support for Signature Station
		
		2008-04-21	C. Knight	7.4.0.1 - Added support for new Meter Station - CSI 5584
		
		2008-05-13	V. Thompson	CSI 5832
								Fixed problem where the setting for 32-bit cards was not
								being saved.
								
		2008-05-22	I.Orndorff	- Removed "EnumerateHosts()" for "Page_Load()".
								- Added selectable "List", "Text" dropdownlist.
								- Added "PopulateSystemDropDownList()", which
								is only called the the SystemDropDownList is 
								visible. This fixes CSI #5907.
		
*******************************************************************************/

namespace FuelsManager.FMWebApp
{
	using System;
	using System.Collections.Generic;
	using System.Runtime.InteropServices;
	using System.Web.UI.WebControls;
	using System.Net;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;

	using OpcCom;
	using Opc;


	/// <summary>
	/// Summary description for StationGeneralPage.
	/// </summary>
	public partial class StationGeneralPage : FMUserControlBase
	{
		protected System.Web.UI.HtmlControls.HtmlInputButton StationPermissivesButton;

		protected void Page_Load(object sender, EventArgs e)
		{
			try
			{
				StationClass station = (StationClass)this.Session["Station"];
				if (!this.Page.IsPostBack)
				{
					this.IDTextBox.Text = station.ID;

					// Populate TypeDropDownList
					for (STATION_TYPE stationType = STATION_TYPE.ENTRY_GATE; stationType < STATION_TYPE.MAX_STATION_TYPE; stationType++)
					{
						ListItem NewTypeItem = new ListItem(StationClass.TypeID(stationType), ((int)stationType).ToString());
						foreach (ListItem ExistingTypeItem in this.TypeDropDownList.Items)
						{
							if (ExistingTypeItem.Text.CompareTo(NewTypeItem.Text) > 0)
							{
								int Index = this.TypeDropDownList.Items.IndexOf(ExistingTypeItem);
								this.TypeDropDownList.Items.Insert(Index, NewTypeItem);
								if (station.Type == stationType)
								{
									this.TypeDropDownList.SelectedIndex = Index;
								}

								NewTypeItem = null;
								break;
							}
						}

						if (NewTypeItem != null)
						{
							this.TypeDropDownList.Items.Add(NewTypeItem);
							if (station.Type == stationType)
							{
								this.TypeDropDownList.SelectedIndex = this.TypeDropDownList.Items.Count - 1;
							}
						}
					}

					// Populate InterfaceTypeDropDownList
					for (STATION_INTERFACE_TYPE interfaceType = STATION_INTERFACE_TYPE.ACCULOADIII_Q; interfaceType < STATION_INTERFACE_TYPE.MAX_TYPE; interfaceType++)
					{
						// Certain Interface types are suitable for certain Station Types
						// for instance and Accuload isn't suitable for Weight Scale
						if (station.Type == STATION_TYPE.EXIT_GATE
						|| station.Type == STATION_TYPE.ENTRY_GATE)
						{
							if (interfaceType != STATION_INTERFACE_TYPE.PROXIMITY_CARD_READER
							&& interfaceType != STATION_INTERFACE_TYPE.PASS_CONTROLLER
							&& interfaceType != STATION_INTERFACE_TYPE.VAREC_DET
							&& interfaceType != STATION_INTERFACE_TYPE.RCU_II_RCU
							&& interfaceType != STATION_INTERFACE_TYPE.OSDP_CARD_READER)
							{
								continue;
							}
						}

						if (station.Type == STATION_TYPE.BOL)
						{
							if (interfaceType != STATION_INTERFACE_TYPE.PROXIMITY_CARD_READER
							&& interfaceType != STATION_INTERFACE_TYPE.PASS_CONTROLLER
							&& interfaceType != STATION_INTERFACE_TYPE.VAREC_DET
									 && interfaceType != STATION_INTERFACE_TYPE.RCU_II_RCU)
							{
								continue;
							}
						}

						if (station.Type == STATION_TYPE.WEIGHT_SCALE)
						{
							if (interfaceType != STATION_INTERFACE_TYPE.PASS_CONTROLLER
							&& interfaceType != STATION_INTERFACE_TYPE.VAREC_DET
										  && interfaceType != STATION_INTERFACE_TYPE.RCU_II_RCU
										  && interfaceType != STATION_INTERFACE_TYPE.MULTILOAD_II_SMP)
							{
								continue;
							}
						}

						if (station.Type == STATION_TYPE.PRELOAD)
						{
							if (interfaceType != STATION_INTERFACE_TYPE.VAREC_DET
										  && interfaceType != STATION_INTERFACE_TYPE.RCU_II_RCU)
							{
								continue;
							}
						}

						if (station.Type == STATION_TYPE.SIGNATURE)
						{
							if (interfaceType != STATION_INTERFACE_TYPE.SIGNATURE)
							{
								continue;
							}
						}

						if (station.Type == STATION_TYPE.METER)
						{
							if (interfaceType != STATION_INTERFACE_TYPE.METER)
							{
								continue;
							}

							this.MeterReferenced.Value = "False";
							if (station.Meter.IdentityGuid != Guid.Empty)
							{
								if (FMChannelHelper.MakeCall<IMeters, bool>(x => x.HasForeignKeyReference(this.Security, station.Meter.IdentityGuid)))
								{
									this.MeterReferenced.Value = "True";
								}
							}
						}

						if (station.Type == STATION_TYPE.OFF_LOADING)
						{
							if (interfaceType != STATION_INTERFACE_TYPE.MANUAL &&
								interfaceType != STATION_INTERFACE_TYPE.CONTREC1010_RA &&
								interfaceType != STATION_INTERFACE_TYPE.VAREC_DET &&
								interfaceType != STATION_INTERFACE_TYPE.MICROLOAD_NET &&
								interfaceType != STATION_INTERFACE_TYPE.MULTILOAD_II &&
								interfaceType != STATION_INTERFACE_TYPE.RCU_II_RCU &&
								interfaceType != STATION_INTERFACE_TYPE.MULTILOAD_II_SMP &&
								interfaceType != STATION_INTERFACE_TYPE.ACCULOADIII_Q)
							{
								continue;
							}
						}

						if (station.Type == STATION_TYPE.LOAD_RACK)
						{
							if (interfaceType == STATION_INTERFACE_TYPE.METER
								|| interfaceType == STATION_INTERFACE_TYPE.CONTREC1010_RA
								|| interfaceType == STATION_INTERFACE_TYPE.PASS_CONTROLLER
								|| interfaceType == STATION_INTERFACE_TYPE.PROXIMITY_CARD_READER
								|| interfaceType == STATION_INTERFACE_TYPE.SIGNATURE
								|| interfaceType == STATION_INTERFACE_TYPE.VAREC_DET
										  || interfaceType == STATION_INTERFACE_TYPE.RCU_II_OPEN)
							{
								continue;
							}
						}

						ListItem NewTypeItem = new ListItem(StationClass.InterfaceTypeID(interfaceType), ((int)interfaceType).ToString());
						foreach (ListItem ExistingTypeItem in this.InterfaceTypeDropDownList.Items)
						{
							if (ExistingTypeItem.Text.CompareTo(NewTypeItem.Text) > 0)
							{
								int Index = this.InterfaceTypeDropDownList.Items.IndexOf(ExistingTypeItem);
								this.InterfaceTypeDropDownList.Items.Insert(Index, NewTypeItem);
								if (station.InterfaceType == interfaceType)
								{
									this.InterfaceTypeDropDownList.SelectedIndex = Index;
								}

								NewTypeItem = null;
								break;
							}
						}

						if (NewTypeItem != null)
						{
							this.InterfaceTypeDropDownList.Items.Add(NewTypeItem);
							if (station.InterfaceType == interfaceType)
							{
								this.InterfaceTypeDropDownList.SelectedIndex = this.InterfaceTypeDropDownList.Items.Count - 1;
							}
						}
					}

					if (station.Type == STATION_TYPE.SIGNATURE
					|| station.Type == STATION_TYPE.METER)
					{
						this.SelectSystemModeDropDownList.Enabled = false;
						this.SystemTextBox.Enabled = false;
						this.SystemDropDownList.Enabled = false;
						this.OPCServerTextBox.Enabled = false;
						this.OPCItemPathTextBox.Enabled = false;
					}
					else
					{
						this.SelectSystemModeDropDownList.Enabled = true;
						this.SystemTextBox.Enabled = true;
						this.SystemDropDownList.Enabled = true;
						this.OPCServerTextBox.Enabled = true;
						this.OPCItemPathTextBox.Enabled = true;

						// Populate SelectSystemModeDropDownList
						ListItem NewItem = new ListItem("List", "0");
						this.SelectSystemModeDropDownList.Items.Add(NewItem);
						NewItem = new ListItem("Text", "1");
						this.SelectSystemModeDropDownList.Items.Add(NewItem);
						this.SelectSystemModeDropDownList.SelectedIndex = 1;
						this.SelectSystemModeDropDownList_SelectedIndexChanged(null, null);

						// Find the default system stored in the process variable collection.
						ProcessVariableClass ProcessVariable = station.ProcessVariableCollection[PROCESS_VARIABLE_TYPE.STATION_PV];
						if (null != ProcessVariable)
						{
							URL Url = new URL(ProcessVariable.URL);
							this.SystemTextBox.Text = Url.HostName;
							this.EnumerateOPCServersBySystemName(this.SystemTextBox.Text);
							this.OPCItemPathTextBox.Text = ProcessVariable.OPCItemID;
						}
					}

					this.InterfaceTypeDropDownList_SelectedIndexChanged(null, null);

					this.CardReaderCheckBox.Checked = station.CardReader;
					this.ThirtyFiveBitCardsCheckBox.Checked = station.ThirtyFiveBitCardSupport;
					this.TouchKeyReaderCheckBox.Checked = station.TouchKeyReader;
					this.LogCommunicationsCheckBox.Checked = station.LogCommunications;
					this.LogCommPathTextbox.Text = station.LogCommPath;
					this.PromptTimeoutBox.Text = station.StationPromptTimeout.ToString();
					this.MessageTimeoutBox.Text = station.StationMessageTimeout.ToString();
				}
				else
				{
					station.ID = this.IDTextBox.Text;

					this.InterfaceTypeDropDownList_SelectedIndexChanged(null, null);
					ProcessVariableClass ProcessVariable = station.ProcessVariableCollection[PROCESS_VARIABLE_TYPE.STATION_PV];
					if (null != ProcessVariable)
					{
						ProcessVariable.OPCItemID = this.OPCItemPathTextBox.Text;
					}

					station.CardReader = this.CardReaderCheckBox.Checked;
					station.ThirtyFiveBitCardSupport = this.ThirtyFiveBitCardsCheckBox.Checked;
					station.TouchKeyReader = this.TouchKeyReaderCheckBox.Checked;
					station.LogCommunications = this.LogCommunicationsCheckBox.Checked;
					station.LogCommPath = this.LogCommPathTextbox.Text;
					try
					{
						station.StationPromptTimeout = System.Convert.ToInt32(this.PromptTimeoutBox.Text);
						station.StationMessageTimeout = System.Convert.ToInt32(this.MessageTimeoutBox.Text);
					}
					catch (Exception)
					{
						throw new Exception("Prompt Timeout or Message Timeout has to be a Number");
					}
				}
			}

			catch (Exception except)
			{
				this.ErrorHandler(except);
			}

		}

		protected void TypeDropDownList_SelectedIndexChanged(object sender, EventArgs e)
		{
			StationClass station = (StationClass)this.Session["Station"];
			STATION_TYPE oldStationType = station.Type;
			STATION_TYPE newStationType = (STATION_TYPE)System.Convert.ToInt32(this.TypeDropDownList.SelectedValue);

			if (oldStationType != STATION_TYPE.METER)
			{
				station.Type = newStationType;
			}
			else if (this.DeleteMeter.Value == "OK")  // Delete unreferenced Meter after user has agreed
			{
				station.Type = newStationType;

				if (station.Meter.IdentityGuid != Guid.Empty)
				{
					FMChannelHelper.MakeCall<IMeters>(x => x.Purge(this.Security, station.Meter.IdentityGuid));
					station.Meter.IdentityGuid = Guid.Empty;

					if (station.IdentityGuid != Guid.Empty)
					{
						FMChannelHelper.MakeCall<IStations>(x => x.Modify(this.Security, station));
					}
				}
			}

			this.Redirect("StationForm.aspx");
		}

		private void EnumerateOPCServersBySystemName(string SystemName)
		{
			try
			{
				this.OPCServerTextBox.Text = "";

				StationClass station = (StationClass)this.Session["Station"];
				foreach (ProcessVariableClass processVariable in station.ProcessVariableCollection)
				{
					if (processVariable.ProcessVariableType != PROCESS_VARIABLE_TYPE.STATION_PV)
					{
						continue;
					}

					this.OPCServerTextBox.Text = "";
					processVariable.URL = "";
					processVariable.ProgID = "";

					ServerEnumerator serverEnumerator = new ServerEnumerator();
					Opc.Server[] servers = serverEnumerator.GetAvailableServers(Opc.Specification.COM_DA_20, SystemName, new ConnectData(new NetworkCredential()));
					foreach (Opc.Server server in servers)
					{
						server.Name = server.Name.Replace(SystemName + ".", "");

						if ((station.InterfaceType == STATION_INTERFACE_TYPE.ACCULOADIII_Q
							|| station.InterfaceType == STATION_INTERFACE_TYPE.ACCULOADIII_SA
							|| station.InterfaceType == STATION_INTERFACE_TYPE.PROXIMITY_CARD_READER
							|| station.InterfaceType == STATION_INTERFACE_TYPE.MICROLOAD_NET
							|| station.InterfaceType == STATION_INTERFACE_TYPE.MULTILOAD_II_SMP
							|| station.InterfaceType == STATION_INTERFACE_TYPE.MULTILOAD_II
							|| station.InterfaceType == STATION_INTERFACE_TYPE.RCU_II_RCU)
							&& server.Name != "Varec.AcculoadOPCServer")
						{
							continue;
						}
						else if ((station.InterfaceType == STATION_INTERFACE_TYPE.PASS_CONTROLLER
							|| station.InterfaceType == STATION_INTERFACE_TYPE.VAREC_DET)
							&& server.Name != "Varec.OptomuxOPCServer")
						{
							continue;
						}
						else if (station.InterfaceType == STATION_INTERFACE_TYPE.DANLOAD6000
							&& server.Name != "Varec.DanielOPCServer")
						{
							continue;
						}
						else if (station.InterfaceType == STATION_INTERFACE_TYPE.MANUAL)
						{
							continue;
						}
						else if (station.InterfaceType == STATION_INTERFACE_TYPE.SIGNATURE)
						{
							continue;
						}

						else if (station.InterfaceType == STATION_INTERFACE_TYPE.METER)
						{
							// allow to continue
						}

						else if (station.InterfaceType == STATION_INTERFACE_TYPE.CONTREC1010
							&& server.Name != "Varec.ContrecOPCServer")
						{
							continue;
						}
						else if (station.InterfaceType == STATION_INTERFACE_TYPE.CONTREC1010_RA
							&& server.Name != "Varec.ContrecOPCServer")
						{
							continue;
						}
						else if (station.InterfaceType == STATION_INTERFACE_TYPE.OSDP_CARD_READER
							&& server.Name != "Varec.OsdpOPCServer")
						{
							continue;
						}
						else if (station.InterfaceType == STATION_INTERFACE_TYPE.MAX_TYPE)
						{
							continue;
						}

						this.OPCServerTextBox.Text = server.Name;
						processVariable.URL = server.Url.ToString();
						processVariable.ProgID = server.Name;
						break;
					}
					break;
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		protected void SystemDropDownList_SelectedIndexChanged(object sender, EventArgs e)
		{
			// Update OPC servers based on text box during post back
			if (this.SystemTextBox.Visible)
			{
				if (0 != this.SystemTextBox.Text.Length)
				{
					this.EnumerateOPCServersBySystemName(this.SystemTextBox.Text);
				}
			}
			else if (-1 != this.SystemDropDownList.SelectedIndex)
			{
				this.EnumerateOPCServersBySystemName(this.SystemDropDownList.SelectedItem.Text);
			}
		}

		protected void InterfaceTypeDropDownList_SelectedIndexChanged(object sender, EventArgs e)
		{
			StationClass station = (StationClass)this.Session["Station"];
			STATION_INTERFACE_TYPE oldStationType = station.InterfaceType;

			if (this.InterfaceTypeDropDownList.SelectedIndex != -1)
			{
				station.InterfaceType = (STATION_INTERFACE_TYPE)System.Convert.ToInt32(this.InterfaceTypeDropDownList.SelectedValue);
			}

			this.SystemDropDownList_SelectedIndexChanged(null, null);

			if (station.InterfaceType == STATION_INTERFACE_TYPE.MANUAL)
			{
				foreach (LoadArmClass loadArm in station.LoadArmCollection)
				{
					loadArm.PresetType = PRESET_TYPE.MANUAL;
					ProcessVariableClass processVariable = loadArm.ProcessVariableCollection[0];
					processVariable.URL = "";
					processVariable.ProgID = "";
				}

				StationLoadArmsPage loadArmsPage = (StationLoadArmsPage)this.Page.FindControl("tcStation").FindControl("tpLoadArmsPage").FindControl("StationLoadArmsPage");
				loadArmsPage.UpdateView();
			}

			// turn everything on by default
			this.CardReaderCheckBox.Enabled = true;
			this.ThirtyFiveBitCardsCheckBox.Enabled = true;
			this.TouchKeyReaderCheckBox.Enabled = true;

			if (station.InterfaceType == STATION_INTERFACE_TYPE.MULTILOAD_II_SMP
				|| station.InterfaceType == STATION_INTERFACE_TYPE.MANUAL
				|| station.InterfaceType == STATION_INTERFACE_TYPE.DANLOAD6000
				|| station.InterfaceType == STATION_INTERFACE_TYPE.SIGNATURE
				|| station.InterfaceType == STATION_INTERFACE_TYPE.METER)
			{
				this.CardReaderCheckBox.Enabled = false;
				this.CardReaderCheckBox.Checked = false;
				this.ThirtyFiveBitCardsCheckBox.Enabled = false;
				this.ThirtyFiveBitCardsCheckBox.Checked = false;
				this.TouchKeyReaderCheckBox.Checked = false;
				this.TouchKeyReaderCheckBox.Enabled = false;
			}
			else if (station.InterfaceType == STATION_INTERFACE_TYPE.CONTREC1010)
			{
				this.CardReaderCheckBox.Enabled = false;
				this.CardReaderCheckBox.Checked = false;
				this.ThirtyFiveBitCardsCheckBox.Enabled = false;
				this.ThirtyFiveBitCardsCheckBox.Checked = false;
				this.TouchKeyReaderCheckBox.Enabled = true;
			}
			else if (station.InterfaceType == STATION_INTERFACE_TYPE.MULTILOAD_II)
			{
				this.ThirtyFiveBitCardsCheckBox.Enabled = false;
				this.ThirtyFiveBitCardsCheckBox.Checked = false;
				this.TouchKeyReaderCheckBox.Enabled = false;
			}
			else if (station.InterfaceType == STATION_INTERFACE_TYPE.CONTREC1010_RA)
			{
				this.CardReaderCheckBox.Enabled = false;
				this.CardReaderCheckBox.Checked = false;
				this.ThirtyFiveBitCardsCheckBox.Enabled = false;
				this.ThirtyFiveBitCardsCheckBox.Checked = false;
				this.TouchKeyReaderCheckBox.Enabled = true;
			}
			else
			{
				if (station.InterfaceType == STATION_INTERFACE_TYPE.PROXIMITY_CARD_READER)
				{
					this.CardReaderCheckBox.Checked = true;
				}

				this.CardReaderCheckBox.Enabled = true;
				this.ThirtyFiveBitCardsCheckBox.Enabled = true;
				this.TouchKeyReaderCheckBox.Checked = false;
				this.TouchKeyReaderCheckBox.Enabled = false;
			}

			if (station.Type == STATION_TYPE.OFF_LOADING &&
				((oldStationType != STATION_INTERFACE_TYPE.VAREC_DET &&
				station.InterfaceType == STATION_INTERFACE_TYPE.VAREC_DET) ||
				(oldStationType == STATION_INTERFACE_TYPE.VAREC_DET &&
				station.InterfaceType != STATION_INTERFACE_TYPE.VAREC_DET)))
			{
				this.TypeDropDownList_SelectedIndexChanged(null, null);
			}
		}

		protected void SelectSystemModeDropDownList_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (this.SystemDropDownList.Visible
				&& this.SystemDropDownList.SelectedIndex != -1)
			{
				this.SystemTextBox.Text = this.SystemDropDownList.SelectedItem.Text;
			}

			this.SystemDropDownList.Visible = this.SelectSystemModeDropDownList.SelectedIndex != 1;
			this.SystemTextBox.Visible = !this.SystemDropDownList.Visible;

			// Only popluate the system drop down list when visible. 
			if (this.SystemDropDownList.Visible)
			{
				this.PopulateSystemDropDownList();
			}
		}

		private void PopulateSystemDropDownList()
		{
			StationClass Station = (StationClass)this.Session["Station"];

			// Populate SystemDropDownList
			this.SystemDropDownList.Items.Clear();
			ListItem newItem = new ListItem("localhost", "0");
			this.SystemDropDownList.Items.Add(newItem);
			var serverList = new List<string>();
			var domain = EnumerateLanMachines.GetDomainOrWorkgroup();
			EnumerateLanMachines.EnumerateMachines(serverList, domain);

			int Item = 1;

			ProcessVariableClass processVariable = Station.ProcessVariableCollection[PROCESS_VARIABLE_TYPE.STATION_PV];
			if (null != processVariable)
			{
				URL url = new URL(processVariable.URL);

				foreach (string system in serverList)
				{
					newItem = new ListItem(system, Item.ToString());
					this.SystemDropDownList.Items.Add(newItem);
					if (system == url.HostName)
					{
						this.SystemDropDownList.SelectedIndex = this.SystemDropDownList.Items.Count - 1;
					}

					Item++;
				}

				this.OPCItemPathTextBox.Text = processVariable.OPCItemID;
			}
		}

		public static string GetDomainOrWorkgroup()
		{
			// Win32 Result Code Constant
			const int ErrorSuccess = 0;
			string domain = null;
			IntPtr pDomain = IntPtr.Zero;
			NetApi.NetJoinStatus status = NetApi.NetJoinStatus.NetSetupUnknownStatus;
			try
			{
				int result = NetApi.NetGetJoinInformation(null, out pDomain, out status);
				if (result == ErrorSuccess)
				{
					switch (status)
					{
						case NetApi.NetJoinStatus.NetSetupDomainName:
						case NetApi.NetJoinStatus.NetSetupWorkgroupName:
							domain = Marshal.PtrToStringAuto(pDomain);
							break;
					}
				}
			}
			finally
			{
				if (pDomain != IntPtr.Zero)
				{
					NetApi.NetApiBufferFree(pDomain);
				}
			}
			
			if (domain == null)
			{
				domain = "";
			}
			
			return domain;
		}


		protected void SystemTextBox_TextChanged(object sender, EventArgs e)
		{
			// Update OPC servers based on text box during post back
			if (this.SystemTextBox.Visible)
			{
				if (0 != this.SystemTextBox.Text.Length)
				{
					this.EnumerateOPCServersBySystemName(this.SystemTextBox.Text);
				}
			}
		}

		protected void OnPromptTimeoutBox_TextChanged(object sender, EventArgs e)
		{
			var station = (StationClass)this.Session["Station"];
			int value;
			try
			{
				value = System.Convert.ToInt32(this.PromptTimeoutBox.Text);
			}
			catch
			{
				value = 60;
			}

			station.StationPromptTimeout = value;
		}

		protected void OnMessageTimeoutBox_TextChanged(object sender, EventArgs e)
		{
			var station = (StationClass)this.Session["Station"];
			int value;
			try
			{
				value = System.Convert.ToInt32(this.MessageTimeoutBox.Text);
			}
			catch
			{
				value = 2;
			}

			station.StationMessageTimeout = value;
		}
	}
}
