// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LoadArmGeneralPage.ascx.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the LoadArmGeneralPage type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FuelsManager.FMWebApp
{
	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;
	using FMControls;
	using Opc;
	using OpcCom;
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Net;
	using System.Web.UI.WebControls;
	using Convert = System.Convert;
	using Server = Opc.Server;

	/// <summary>
	///    Summary description for LoadArmGeneralPage.
	/// </summary>
	public partial class LoadArmGeneralPage : FMUserControlBase
	{
		#region Constants and Fields

		protected FMLabel Label1;

		protected FMLabel Label5;

		#endregion

		#region Properties

		private string JavascriptStartup
		{
			get
			{
				string script = @"
				<script type='text/javascript'>
				<!--
					// Set Arm Permissives and No Additive Permissives Button values according to Data Dictionary
					var LoadArmPermissivesButton=document.getElementById('LoadArmPermissivesButton');
					if(LoadArmPermissivesButton != null)
						LoadArmPermissivesButton.value='" + ((FMFormBase)this.Page).GetTranslatedText("Load Arm Permissives") + @"';
					var NoAdditivePermissivesButton=document.getElementById('NoAdditivePermissivesButton');
					if(NoAdditivePermissivesButton != null)
						NoAdditivePermissivesButton.value='" + ((FMFormBase)this.Page).GetTranslatedText("No Additive Permissives") + @"';
				//-->
				</script>
				";
				return script;
			}
		}

		#endregion

		#region Public Methods and Operators

		public void UpdateData()
		{
			var station = (StationClass)this.Session["Station"];
			var loadArm = station.LoadArmCollection[(int)this.Session["LoadArmIndex"]];

			loadArm.LoadRackText = this.LoadRackTextTextBox.Text;
			loadArm.Enabled = this.EnabledCheckBox.Checked;
			loadArm.SwingArm = this.SwingArmCheckBox.Checked;

			if (!loadArm.SwingArm)
			{
				if (station.SwingArmPosition == "A")
				{
					loadArm.BayBStationGuid = Guid.Empty;
					loadArm.BayBArmNumber = 0;
				}
				else
				{
					loadArm.BayAStationGuid = Guid.Empty;
					loadArm.BayAArmNumber = 0;
				}
			}

			ProcessVariableClass processVariable = loadArm.ProcessVariableCollection[0];
			processVariable.OPCItemID = this.OPCItemIDTextBox.Text;
		}

		#endregion

		#region Methods

		protected override void OnInit(EventArgs e)
		{
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			this.InitializeComponent();
			base.OnInit(e);
		}

		protected void Page_Load(object sender, EventArgs e)
		{
			try
			{
				StationClass currentStation = this.Session["Station"] as StationClass;
				LoadArmClass loadArm = currentStation?.LoadArmCollection[(int)this.Session["LoadArmIndex"]];

				if (!this.Page.IsPostBack)
				{
				    if (loadArm != null)
				    {
				        this.LoadRackTextTextBox.Text = loadArm.LoadRackText;
				        this.EnabledCheckBox.Checked = loadArm.Enabled;

				        // StationDropDownList
				        StationCollectionClass stationCollection =
				            FMChannelHelper.MakeCall<IStations, StationCollectionClass>(
				                x => x.EnumerateByType(this.Security, STATION_TYPE.LOAD_RACK));

				        foreach (StationClass station in stationCollection)
				        {
				            if (station.SwingArmPosition == currentStation.SwingArmPosition)
				            {
				                continue;
				            }

				            var newStationItem = new ListItem(station.ID, station.IdentityGuid.ToString());
				            foreach (ListItem existingStationItem in this.StationDropDownList.Items)
				            {
				                if (string.Compare(existingStationItem.Text, newStationItem.Text, StringComparison.Ordinal) > 0)
				                {
				                    int index = this.StationDropDownList.Items.IndexOf(existingStationItem);
				                    this.StationDropDownList.Items.Insert(index, newStationItem);
				                    if (station.SwingArmPosition == "A")
				                    {
				                        if (loadArm.BayAStationGuid == station.IdentityGuid)
				                        {
				                            this.StationDropDownList.SelectedIndex = index;
				                        }
				                    }
				                    else
				                    {
				                        if (loadArm.BayBStationGuid == station.IdentityGuid)
				                        {
				                            this.StationDropDownList.SelectedIndex = index;
				                        }
				                    }
				                    newStationItem = null;
				                    break;
				                }
				            }

				            if (newStationItem != null)
				            {
				                this.StationDropDownList.Items.Add(newStationItem);
				                if (station.SwingArmPosition == "A")
				                {
				                    if (loadArm.BayAStationGuid == station.IdentityGuid)
				                    {
				                        this.StationDropDownList.SelectedIndex = this.StationDropDownList.Items.Count - 1;
				                    }
				                }
				                else
				                {
				                    if (loadArm.BayBStationGuid == station.IdentityGuid)
				                    {
				                        this.StationDropDownList.SelectedIndex = this.StationDropDownList.Items.Count - 1;
				                    }
				                }
				            }
				        }

				        this.SwingArmCheckBox.Checked = loadArm.SwingArm;

				        // Mutiload II SMP  doesn't seem to support swing arm
				        // the configuration registry 101094ppp cannot be written
				        // and it is not preset on the configuration screens. 
				        if (this.StationDropDownList.Items.Count == 0 || loadArm.PresetType == PRESET_TYPE.MULTILOAD_II_SMP)
				        {
				            this.SwingArmCheckBox.Checked = false;
				            this.SwingArmCheckBox.Enabled = false;
				        }

				        if (!this.SwingArmCheckBox.Checked)
				        {
				            this.StationDropDownList.Visible = false;
				        }

				        // TypeDropDownList
				        for (var presetType = PRESET_TYPE.ACCULOAD2_STD; presetType < PRESET_TYPE.MAX_PRESET_TYPE; presetType++)
				        {
				            if (currentStation.InterfaceType == STATION_INTERFACE_TYPE.MANUAL && presetType != PRESET_TYPE.MANUAL)
				            {
				                continue;
				            }

				            if (currentStation.InterfaceType == STATION_INTERFACE_TYPE.ACCULOADIII_Q
				                && presetType != PRESET_TYPE.ACCULOADIII_Q)
				            {
				                continue;
				            }

				            if (currentStation.InterfaceType == STATION_INTERFACE_TYPE.ACCULOADIII_SA
				                && presetType != PRESET_TYPE.ACCULOADIII_SA)
				            {
				                continue;
				            }

				            if (currentStation.InterfaceType == STATION_INTERFACE_TYPE.MICROLOAD_NET
				                && presetType != PRESET_TYPE.MICROLOAD_NET)
				            {
				                continue;
				            }

				            if (currentStation.InterfaceType == STATION_INTERFACE_TYPE.DANLOAD6000 && presetType != PRESET_TYPE.DANLOAD6000)
				            {
				                continue;
				            }

				            if (currentStation.InterfaceType == STATION_INTERFACE_TYPE.MULTILOAD_II_SMP
				                && presetType != PRESET_TYPE.MULTILOAD_II_SMP)
				            {
				                continue;
				            }

				            if (currentStation.InterfaceType == STATION_INTERFACE_TYPE.CONTREC1010 && presetType != PRESET_TYPE.CONTREC1010)
				            {
				                continue;
				            }

				            if (currentStation.InterfaceType == STATION_INTERFACE_TYPE.MULTILOAD_II && presetType != PRESET_TYPE.MULTILOAD_II)
				            {
				                continue;
				            }

				            if (currentStation.InterfaceType == STATION_INTERFACE_TYPE.CONTREC1010_RA
				                && presetType != PRESET_TYPE.CONTREC1010_RA)
				            {
				                continue;
				            }

				            // Exclude Types not supported
				            if (presetType == PRESET_TYPE.ACCULOAD2_RBM || presetType == PRESET_TYPE.ACCULOAD2_RBU
				                || presetType == PRESET_TYPE.ACCULOAD2_SEQ || presetType == PRESET_TYPE.ACCULOAD2_SQR
				                || presetType == PRESET_TYPE.ACCULOAD2_STD || presetType == PRESET_TYPE.ACCULOAD2_STM
				                || presetType == PRESET_TYPE.ACCULOADIII_S)
				            {
				                continue;
				            }

				            var newTypeItem = new ListItem(LoadArmClass.PresetTypeID(presetType), ((int)presetType).ToString());
				            foreach (ListItem existingTypeItem in this.PresetTypeDropDownList.Items)
				            {
				                if (string.Compare(existingTypeItem.Text, newTypeItem.Text, StringComparison.Ordinal) > 0)
				                {
				                    int index = this.PresetTypeDropDownList.Items.IndexOf(existingTypeItem);
				                    this.PresetTypeDropDownList.Items.Insert(index, newTypeItem);
				                    if (loadArm.PresetType == presetType)
				                    {
				                        this.PresetTypeDropDownList.SelectedIndex = index;
				                    }
				                    newTypeItem = null;
				                    break;
				                }
				            }

				            if (newTypeItem != null)
				            {
				                this.PresetTypeDropDownList.Items.Add(newTypeItem);
				                if (loadArm.PresetType == presetType)
				                {
				                    this.PresetTypeDropDownList.SelectedIndex = this.PresetTypeDropDownList.Items.Count - 1;
				                }
				            }
				        }

				        // Certain Station Interface Types support only specific Preset Types
				        if (currentStation.InterfaceType == STATION_INTERFACE_TYPE.MANUAL
				            || currentStation.InterfaceType == STATION_INTERFACE_TYPE.ACCULOADIII_Q
				            || currentStation.InterfaceType == STATION_INTERFACE_TYPE.ACCULOADIII_SA
				            || currentStation.InterfaceType == STATION_INTERFACE_TYPE.MICROLOAD_NET
				            || currentStation.InterfaceType == STATION_INTERFACE_TYPE.DANLOAD6000
				            || currentStation.InterfaceType == STATION_INTERFACE_TYPE.MULTILOAD_II_SMP
                  		    || currentStation.InterfaceType == STATION_INTERFACE_TYPE.MULTILOAD_II
				            || currentStation.InterfaceType == STATION_INTERFACE_TYPE.CONTREC1010_RA)
				        {
				            this.PresetTypeDropDownList.Enabled = false;
				            loadArm.PresetType = (PRESET_TYPE)Convert.ToInt32(this.PresetTypeDropDownList.SelectedValue);
				        }

				        // Populate SelectSystemModeDropDownList
				        var newItem = new ListItem("List", "0");
				        this.SelectSystemModeDropDownList.Items.Add(newItem);
				        newItem = new ListItem("Text", "1");
				        this.SelectSystemModeDropDownList.Items.Add(newItem);
				        this.SelectSystemModeDropDownList.SelectedIndex = 1;
				        this.SelectSystemModeDropDownListSelectedIndexChanged(null, null);

				        ProcessVariableClass processVariable = loadArm.ProcessVariableCollection[0];
				        if (null != processVariable)
				        {
				            var url = new URL(processVariable.URL);
				            this.SystemTextBox.Text = url.HostName;
				            this.EnumerateOpcServersBySystemName(this.SystemTextBox.Text);
				            this.OPCItemIDTextBox.Text = processVariable.OPCItemID;
				        }
				    }

				    this.PresetTypeDropDownListSelectedIndexChanged(null, null);
				}
				else
				{
					// Update OPC servers based on text box during post back
					if (this.SystemTextBox.Visible)
					{
						if (0 != this.SystemTextBox.Text.Length)
						{
							this.EnumerateOpcServersBySystemName(this.SystemTextBox.Text);
						}
					}
				}

			    this.LoadArmPermissivesButton.Value = this.GetTranslatedText(this.LoadArmPermissivesButton.Value);
                this.NoAdditivePermissivesButton.Value = this.GetTranslatedText(this.NoAdditivePermissivesButton.Value);

                this.Page.ClientScript.RegisterStartupScript(
					this.GetType(), "LoadArmGeneralPageScriptBlock", this.JavascriptStartup);
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		protected void PresetTypeDropDownListSelectedIndexChanged(object sender, EventArgs e)
		{
			StationClass station = this.Session["Station"] as StationClass;
			LoadArmClass loadArm = station?.LoadArmCollection[(int)this.Session["LoadArmIndex"]];

			if (this.PresetTypeDropDownList.SelectedIndex != -1)
			{
			    if (loadArm != null)
			    {
			        loadArm.PresetType = (PRESET_TYPE)Convert.ToInt32(this.PresetTypeDropDownList.SelectedValue);
			    }
			}

			if (loadArm != null && (loadArm.PresetType == PRESET_TYPE.MULTILOAD_II_SMP || loadArm.PresetType == PRESET_TYPE.MANUAL
			                        || loadArm.PresetType == PRESET_TYPE.MICROLOAD_NET || loadArm.PresetType == PRESET_TYPE.CONTREC1010
			                        || loadArm.PresetType == PRESET_TYPE.CONTREC1010_RA))
			{
				this.SwingArmCheckBox.Enabled = false;
				this.SwingArmCheckBox.Checked = false;
			}
			else
			{
				this.SwingArmCheckBox.Enabled = true;
			}

			this.SystemDropDownListSelectedIndexChanged(null, null);
		}

		protected void SelectSystemModeDropDownListSelectedIndexChanged(object sender, EventArgs e)
		{
			if (this.SystemDropDownList.Visible && this.SystemDropDownList.SelectedIndex != -1)
			{
				this.SystemTextBox.Text = this.SystemDropDownList.SelectedItem.Text;
			}

			this.SystemDropDownList.Visible = (this.SelectSystemModeDropDownList.SelectedIndex != 1);
			this.SystemTextBox.Visible = !this.SystemDropDownList.Visible;

			// Only popluate the system drop down list when visible. 
			if (this.SystemDropDownList.Visible)
			{
				this.PopulateSystemDropDownList();
			}
		}

		protected void StationDropDownListSelectedIndexChanged(object sender, EventArgs e)
		{
			try
			{
				StationClass station = this.Session["Station"] as StationClass;
				LoadArmClass loadArm = station?.LoadArmCollection[(int)this.Session["LoadArmIndex"]];

				if (this.StationDropDownList.SelectedIndex != -1)
				{
					Guid selectedPartnerSwingArmStationGuid = Guid.Empty;

					if (station?.SwingArmPosition == "A")
					{
						loadArm.BayBStationGuid = Guid.Parse(this.StationDropDownList.SelectedValue);
						selectedPartnerSwingArmStationGuid = loadArm.BayBStationGuid;
					}
					else
					{
					    if (loadArm != null)
					    {
					        loadArm.BayAStationGuid = Guid.Parse(this.StationDropDownList.SelectedValue);
							  selectedPartnerSwingArmStationGuid = loadArm.BayAStationGuid;
						}
					}

					if (selectedPartnerSwingArmStationGuid != Guid.Empty)
					{
						bool isDynamicRecipesEnabledOnPartnerStation =
							 FMChannelHelper.MakeCall<IStations, bool>(
								  x => x.IsDynamicRecipesEnabled(this.Security, selectedPartnerSwingArmStationGuid, station.Type));

						if (isDynamicRecipesEnabledOnPartnerStation != station.EnableDynamicRecipes)
						{
							bool DisplayConfirmationPrompt = (station.Type == STATION_TYPE.LOAD_RACK);

							Button OKButton = (Button)this.Page.FindControl("OK");

							if (OKButton != null)
							{
								OKButton.OnClientClick = DisplayConfirmationPrompt ? "return OKButton_Click();" : "";
							}

							//Re-enable Load Arm form OK/Cancel buttons that were disabled on the client side
							EnableLoadArmFormControls(true);
						}
						else
						{
							// Remove any client script, if both stations have same Enable Dynamic Recipes setting
							Button OKButton = (Button)this.Page.FindControl("OK");

							if (OKButton != null)
							{
								OKButton.OnClientClick = "";
							}
						}
					}
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}
		private void EnableLoadArmFormControls(bool enable)
		{
			var loadArmForm = (LoadArmForm)this.Page;
			loadArmForm.EnableControls(enable);
		}

		protected void SwingArmCheckBoxCheckedChanged(object sender, EventArgs e)
		{
			this.StationDropDownList.Visible = this.SwingArmCheckBox.Checked;

			if (this.SwingArmCheckBox.Checked)
			{
				this.StationDropDownListSelectedIndexChanged(null, null);
			}
			else
			{
				// Remove any client script, if the arm is no longer a swing arm
				Button OKButton = (Button)this.Page.FindControl("OK");

				if (OKButton != null)
				{
					OKButton.OnClientClick = "";
				}
			}
		}

		protected void SystemDropDownListSelectedIndexChanged(object sender, EventArgs e)
		{
			if (-1 != this.SystemDropDownList.SelectedIndex)
			{
				this.EnumerateOpcServersBySystemName(this.SystemDropDownList.SelectedItem.Text);
			}
		}

		private void EnumerateOpcServersBySystemName(string systemName)
		{
			try
			{
				this.OPCServerTextBox.Text = "";

				StationClass station = this.Session["Station"] as StationClass;
				LoadArmClass loadArm = station?.LoadArmCollection[(int)this.Session["LoadArmIndex"]];
				ProcessVariableClass processVariable = loadArm?.ProcessVariableCollection[0];

				this.OPCServerTextBox.Text = "";
			    if (processVariable != null)
			    {
			        processVariable.URL = "";
			        processVariable.ProgID = "";


			        var serverEnumerator = new ServerEnumerator();
			        Server[] servers = serverEnumerator.GetAvailableServers(
			            Specification.COM_DA_20, systemName, new ConnectData(new NetworkCredential()));
			        foreach (Server server in servers)
			        {
			            server.Name = server.Name.Replace(systemName + ".", "");

			            if ((loadArm.PresetType == PRESET_TYPE.ACCULOAD2_RBM || loadArm.PresetType == PRESET_TYPE.ACCULOAD2_RBU
			                 || loadArm.PresetType == PRESET_TYPE.ACCULOAD2_SEQ || loadArm.PresetType == PRESET_TYPE.ACCULOAD2_SQR
			                 || loadArm.PresetType == PRESET_TYPE.ACCULOAD2_STD || loadArm.PresetType == PRESET_TYPE.ACCULOAD2_STM
			                 || loadArm.PresetType == PRESET_TYPE.ACCULOADIII_Q || loadArm.PresetType == PRESET_TYPE.ACCULOADIII_S
			                 || loadArm.PresetType == PRESET_TYPE.MICROLOAD_NET || loadArm.PresetType == PRESET_TYPE.ACCULOADIII_SA
			                 || loadArm.PresetType == PRESET_TYPE.MULTILOAD_II_SMP || loadArm.PresetType == PRESET_TYPE.MULTILOAD_II)
			                && server.Name != "Varec.AcculoadOPCServer")
			            {
			                continue;
			            }

			            if (loadArm.PresetType == PRESET_TYPE.DANLOAD6000 && server.Name != "Varec.DanielOPCServer")
			            {
			                continue;
			            }

			            else if ((loadArm.PresetType == PRESET_TYPE.CONTREC1010 || loadArm.PresetType == PRESET_TYPE.CONTREC1010_RA)
			                     && server.Name != "Varec.ContrecOPCServer")
			            {
			                continue;
			            }

			            else if (loadArm.PresetType == PRESET_TYPE.MANUAL)
			            {
			                continue;
			            }

				        if (server.Name == "Varec.ScullyOPCServer")
				        {
				            continue;
				        }

			            this.OPCServerTextBox.Text = server.Name;
			            processVariable.URL = server.Url.ToString();
			            processVariable.ProgID = server.Name;
			            break;
			        }
			    }
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		/// <summary>
		///    Required method for Designer support - do not modify
		///    the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
		}

		private void PopulateSystemDropDownList()
		{
			StationClass station = this.Session["Station"] as StationClass;
			LoadArmClass loadArm = station?.LoadArmCollection[(int)this.Session["LoadArmIndex"]];

			// Populate SystemDropDownList
			this.SystemDropDownList.Items.Clear();
			var newItem = new ListItem("localhost", "0");
			this.SystemDropDownList.Items.Add(newItem);
			var serverList = new List<string>();
			var domain = EnumerateLanMachines.GetDomainOrWorkgroup();
			EnumerateLanMachines.EnumerateMachines(serverList, domain);
			int item = 1;

			ProcessVariableClass processVariable = loadArm?.ProcessVariableCollection[0];
			if (null != processVariable)
			{
				var url = new URL(processVariable.URL);
				foreach (string system in serverList)
				{
					newItem = new ListItem(system, item.ToString());
					this.SystemDropDownList.Items.Add(newItem);
					if (system == url.HostName)
					{
						this.SystemDropDownList.SelectedIndex = this.SystemDropDownList.Items.Count - 1;
					}
					item++;
				}
			}
		}

		#endregion
	}
}