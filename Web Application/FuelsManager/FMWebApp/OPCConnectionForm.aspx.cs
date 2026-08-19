// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OPCConnectionForm.aspx.cs" company="Varec, Inc.">
//	Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//	Defines the OPCConnectionForm type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FuelsManager.FMWebApp
{
	using System;
	using System.Net;
	using System.Collections.Generic;
	using System.Runtime.InteropServices;
	using System.Web.UI.WebControls;
	using System.Text;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;

	using FMControls;

	using FMCore;

	using Opc;
	using Opc.Da;

	using OpcCom;

	using Varec.CommonComponents.EngineeringUnitsLibrary;

	using Convert = System.Convert;
	using Factory = OpcCom.Factory;
	using Server = Opc.Server;
	using Type = System.Type;


	internal class NetApi
	{
		public const int ErrorSuccess = 0;

		[DllImport("Netapi32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
		public static extern int NetGetJoinInformation(string server, out IntPtr domain, out NetJoinStatus status);

		[DllImport("Netapi32.dll")]
		public static extern int NetApiBufferFree(IntPtr Buffer);

		[DllImport("Netapi32.dll", EntryPoint = "NetServerEnum", CharSet = CharSet.Ansi)]
		public static extern Int32 NetServerEnum(
				[MarshalAs(UnmanagedType.LPWStr)] String serverName,
				Int32 level,
				out IntPtr bufferPtr,
				UInt32 prefMaxLen,
				ref Int32 entriesRead,
				ref Int32 totalEntries,
				UInt32 serverType,
				[MarshalAs(UnmanagedType.LPWStr)] String domain,
				IntPtr handle);


		public enum NetJoinStatus
		{
			NetSetupUnknownStatus = 0,
			NetSetupUnjoined,
			NetSetupWorkgroupName,
			NetSetupDomainName
		}
	}

	public class EnumerateLanMachines
	{
		public const UInt32 SUCCESS = 0;
		public const UInt32 FAIL = 234;
		public const UInt32 MAX_PREFERRED_LENGTH = 0xFFFFFFFF;
		//public ArrayList machines = new ArrayList ( );

		enum ServerTypes : uint
		{
			WorkStation = 0x00000001,
			Server = 0x00000002
		}

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
		public struct MachineInfo
		{
			[MarshalAs(UnmanagedType.U4)]
			public UInt32 platformId;

			[MarshalAs(UnmanagedType.LPWStr)]
			public String serverName;
		}

		public enum Platform
		{
			PLATFORM_ID_DOS = 300,
			PLATFORM_ID_OS2 = 400,
			PLATFORM_ID_NT = 500,
			PLATFORM_ID_OSF = 600,
			PLATFORM_ID_VMS = 700
		}


		public static string GetDomainOrWorkgroup()
		{
			// Win32 Result Code Constant
			const int ErrorSuccess = 0;

			int result = 0;
			string domain = null;
			IntPtr pDomain = IntPtr.Zero;
			NetApi.NetJoinStatus status = NetApi.NetJoinStatus.NetSetupUnknownStatus;
			try
			{
				result = NetApi.NetGetJoinInformation(null, out pDomain, out status);
				if (result == ErrorSuccess)
					switch (status)
					{
						case NetApi.NetJoinStatus.NetSetupDomainName:
						case NetApi.NetJoinStatus.NetSetupWorkgroupName:
							domain = Marshal.PtrToStringAuto(pDomain);
							break;
					}
			}
			finally
			{
				if (pDomain != IntPtr.Zero)
					NetApi.NetApiBufferFree(pDomain);
			}
			if (domain == null) domain = "";
			return domain;
		}


		public static void EnumerateMachines(List<string> serverList, string domain)
		{
			IntPtr buffer = new IntPtr();
			IntPtr tmpBuffer = IntPtr.Zero;
			int totalEntries = 0;
			int entriesRead = 0;
			int result;

			try
			{
				result = NetApi.NetServerEnum(null, 100, out buffer, MAX_PREFERRED_LENGTH, ref entriesRead, ref totalEntries, (uint)0xFFFFFFFF, domain, IntPtr.Zero);

				MachineInfo machineInfo;

				if (result != FAIL)
				{
					for (int i = 0; i < entriesRead; ++i)
					{
						tmpBuffer = (IntPtr)(ulong)buffer + i * Marshal.SizeOf(typeof(MachineInfo));

						machineInfo = (MachineInfo)Marshal.PtrToStructure(tmpBuffer, typeof(MachineInfo));

						serverList.Add(machineInfo.serverName);
					}
				}
			}
			finally
			{
				NetApi.NetApiBufferFree(buffer);
			}
		}
	}


	/// <summary>
	///	Summary description for OPCConnectionForm.
	/// </summary>
	public partial class OPCConnectionForm : FMAutoSubmitFormBase
	{
		// Note: default Marshaling isn't correct so must be 1 for now

		#region Constants and Fields

		protected TextBox DecimalPlacesTextBox;

		protected FMLabel Label2;

		protected FMLabel Label9;

/*
		private const uint MAX_COUNT = 1;
*/

/*
		private const ushort OPC_READABLE = 1;
*/

/*
		private const ushort OPC_WRITABLE = 2;
*/

		private readonly VarEnum[] dataType =
			{
				VarEnum.VT_BOOL, VarEnum.VT_I1, VarEnum.VT_I2, VarEnum.VT_I4, VarEnum.VT_I8,
				VarEnum.VT_INT, VarEnum.VT_UI1, VarEnum.VT_UI2, VarEnum.VT_UI4, VarEnum.VT_UI8, VarEnum.VT_UINT, VarEnum.VT_R4,
				VarEnum.VT_R8, VarEnum.VT_DATE, VarEnum.VT_BSTR, VarEnum.VT_EMPTY, VarEnum.VT_NULL
			};

		#endregion

		#region Methods

		protected void DataTypeDropDownListSelectedIndexChanged(object sender, EventArgs e)
		{
			var pv = new ProcessVariableClass();

			SiteClass site = FMChannelHelper.MakeCall<ISites, SiteClass>(
																	x =>
																	x.Get(this.Security, this.Security.SiteGuid, false, false, true)
																);
			var processVariable = (ProcessVariableClass)this.Session["ProcessVariable"];

			EngineeringUnit units = site.GetSiteUnits(processVariable.SiteVariableType);
			byte decimalPlaces = site.GetSiteDecimalPlaces(processVariable.SiteVariableType);

			if (processVariable.DataType != this.dataType[Convert.ToInt32(this.DataTypeDropDownList.SelectedValue)])
			{
				pv.DataType = this.dataType[Convert.ToInt32(this.DataTypeDropDownList.SelectedValue)];
			}
			else
			{
				pv.DataType = processVariable.DataType;
				pv.ProcessVariableType = processVariable.ProcessVariableType;
				pv.siMinimum = processVariable.siMinimum;
				pv.siMaximum = processVariable.siMaximum;
			}

			ListItem selectedListItem = this.DataTypeDropDownList.SelectedItem;

			if (selectedListItem.Text == VarEnum.VT_BOOL.ToString() || selectedListItem.Text == VarEnum.VT_BSTR.ToString()
				|| selectedListItem.Text == VarEnum.VT_EMPTY.ToString())
			{
				this.MaximumTextBox.Text = "";
				this.MaximumTextBox.Enabled = false;
				this.MinimumTextBox.Text = "";
				this.MinimumTextBox.Enabled = false;
			}
			else
			{
				this.MaximumTextBox.Text = processVariable.Encode(
					pv.GetMaximum(units, decimalPlaces),
					Quality.Good,
					units,
					site.GetNumberFormatInfo(processVariable.SiteVariableType));
				this.MaximumTextBox.Enabled = true;
				this.MinimumTextBox.Text = processVariable.Encode(
					pv.GetMinimum(units, decimalPlaces),
					Quality.Good,
					units,
					site.GetNumberFormatInfo(processVariable.SiteVariableType));
				this.MinimumTextBox.Enabled = true;
				// check for infinity values and set defaults if they are set
				if (this.MaximumTextBox.Text == "Infinity")
				{
					this.MaximumTextBox.Text = "100";
				}
				if (this.MinimumTextBox.Text == "-Infinity")
				{
					this.MinimumTextBox.Text = "0";
				}
			}
		}

		protected void DataTypeFilterDropDownListSelectedIndexChanged(object sender, EventArgs e)
		{
			this.ItemsTreeViewSelectedNodeChanged(null, null);
		}

		protected void ItemsHeirarchicalListBoxSelectedIndexChanged(object sender, EventArgs e)
		{
			var itemsListBox = (ListBox)sender;
			ListItem selectedListItem = itemsListBox.SelectedItem;

			this.OPCItemIDTextBox.Text = selectedListItem.Value;
		}

		protected void OpcServerDropDownListSelectedIndexChanged(object sender, EventArgs e)
		{
			try
			{
				this.OPCItemIDTextBox.Text = "";
				this.ItemsTreeView.Nodes.Clear();
				this.ItemsHeirarchicalListBox.Items.Clear();

				if (this.OPCServerDropDownList.SelectedItem.Text == this.GetUnassignedTranslation())
				{
					this.ItemsTreeView.Visible = false;
					this.ItemsTreeView.Enabled = false;
					this.ItemsHeirarchicalListBox.Visible = false;
					this.ItemsHeirarchicalListBox.Enabled = false;
					this.AvailableOPCItemsLabel.Visible = false;
					this.DataTypeFilterLabel.Visible = false;
					this.DataTypeFilterDropDownList.Visible = false;
					return;
				}

				var serverEnumerator = new ServerEnumerator();

				string systemName;
				if (-1 != this.SystemDropDownList.SelectedIndex)
				{
					systemName = this.SystemDropDownList.SelectedItem.Text;
				}
				else
				{
					systemName = this.SystemTextBox.Text;
				}

				Server[] servers = serverEnumerator.GetAvailableServers(
					Specification.COM_DA_20, systemName, new ConnectData(new NetworkCredential()));

				foreach (Server server in servers)
				{
					if (server.Url.ToString() == this.OPCServerDropDownList.SelectedValue)
					{
						using (var opcServer = new Opc.Da.Server(new Factory(false), server.Url))
						{

							opcServer.Connect();

							var itemIdentifier = new ItemIdentifier("");
							var browseFilters = new BrowseFilters { BrowseFilter = browseFilter.branch };
							BrowsePosition browsePosition;

							BrowseElement[] browseElements = opcServer.Browse(itemIdentifier, browseFilters, out browsePosition);

							this.ItemsTreeView.Visible = true;
							this.ItemsTreeView.Enabled = true;
							this.ItemsHeirarchicalListBox.Visible = true;
							this.ItemsHeirarchicalListBox.Enabled = true;
							this.AvailableOPCItemsLabel.Visible = true;
							this.DataTypeFilterLabel.Visible = true;
							this.DataTypeFilterDropDownList.Visible = true;

							foreach (BrowseElement element in browseElements)
							{
								var itemTreeNode = new TreeNode
														{
																Text = element.Name,
																Value = element.ItemName,
																Expanded = false,
																SelectAction = TreeNodeSelectAction.SelectExpand
														};
								if (element.HasChildren)
								{
									BrowseElement[] subElements = opcServer.Browse(
										new ItemIdentifier(itemTreeNode.Value),
										browseFilters,
										out browsePosition);
									if (subElements != null && subElements.Length != 0)
									{
										var childItemTreeNode = new TreeNode { Expanded = false, SelectAction = TreeNodeSelectAction.SelectExpand };
										itemTreeNode.ChildNodes.Add(childItemTreeNode);
									}
								}
								this.ItemsTreeView.Nodes.Add(itemTreeNode);
							}

							opcServer.Disconnect();

							this.Session["OPCServerUrl"] = opcServer.Url;
						}

						break;
					}
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

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
				this.GetSecurity();
				bool isPageRequestForChildRecordVersion = false;
				if (this.Request.GetQueryOrFormValue("ISCHILDRECORDVERSION") != null)
				{
					isPageRequestForChildRecordVersion = Convert.ToBoolean(this.Request.GetQueryOrFormValue("ISCHILDRECORDVERSION"));
				}

				if (! this.Page.IsPostBack)
				{
					if ((!this.Security.HasRight(RIGHT.MODIFY_SITES_AND_SITE_GROUPS)) || isPageRequestForChildRecordVersion)
					{
						this.OK.Enabled = false;
					}

					this.ItemsTreeView.Visible = false;
					this.ItemsTreeView.Enabled = false;
					this.ItemsHeirarchicalListBox.Visible = false;
					this.ItemsHeirarchicalListBox.Enabled = false;
					this.AvailableOPCItemsLabel.Visible = false;
					this.DataTypeFilterLabel.Visible = false;
					this.DataTypeFilterDropDownList.Visible = false;

					var processVariable = (ProcessVariableClass)this.Session["ProcessVariable"];

					this.ProcessVariableIDTextBox.Text = ProcessVariableClass.UnitTypeID(processVariable.UnitType) + "-"
																	+ ProcessVariableClass.ProcessVariableTypeID(
																		processVariable.ProcessVariableType);

					// Populate the DataTypeDropDownList
					int iType = 0;
					while (this.dataType[iType] != VarEnum.VT_NULL)
					{
						this.DataTypeDropDownList.Items.Add(new ListItem(this.dataType[iType].ToString(), iType.ToString()));
						this.DataTypeFilterDropDownList.Items.Add(new ListItem(this.dataType[iType].ToString(), iType.ToString()));

						if (this.dataType[iType] == processVariable.DataType)
						{
							this.DataTypeFilterDropDownList.SelectedIndex = iType;
							this.DataTypeDropDownList.SelectedIndex = iType;
						}
						iType++;
					}

					this.DataTypeDropDownList.Enabled = processVariable.DataTypeEnabled;

					if (processVariable.UnitsEnabled)
					{
						EngineeringUnit lower, upper;

						switch (processVariable.UnitsType)
						{
							case EngineeringUnitType.FmuAll: // All Units
								lower = EngineeringUnit.FmtDegC;
								upper = EngineeringUnit.FmduPh;
								break;

							case EngineeringUnitType.FmuTemp: // Temperature Units
								lower = EngineeringUnit.FmtDegC;
								upper = EngineeringUnit.FmtDegR;
								break;

							case EngineeringUnitType.FmuTime: // Time Units
								lower = EngineeringUnit.FmtMsec;
								upper = EngineeringUnit.FmtYear;
								break;

							case EngineeringUnitType.FmuLength: // Length Units
								lower = EngineeringUnit.FmlFtIn8Th;
								upper = EngineeringUnit.FmlMile;
								break;

							case EngineeringUnitType.FmuArea: // Area Units
								lower = EngineeringUnit.FmaMm2;
								upper = EngineeringUnit.FmaMile2;
								break;

							case EngineeringUnitType.FmuVolume: // Volume Units
								lower = EngineeringUnit.FmvCm3;
								upper = EngineeringUnit.FmvKl;
								break;

							case EngineeringUnitType.FmuMass: // Mass/Weight Units
								lower = EngineeringUnit.FmmGram;
								upper = EngineeringUnit.FmmMlbs;
								break;

							case EngineeringUnitType.FmuPressure: // Pressure Units
								lower = EngineeringUnit.FmpPa;
								upper = EngineeringUnit.FmpAtm;
								break;

							case EngineeringUnitType.FmuVolflow: // Volumetric Flow
								lower = EngineeringUnit.FmvfCcMin;
								upper = EngineeringUnit.FmvfKlDay;
								break;

							case EngineeringUnitType.FmuMassflow: // Mass Flow
								lower = EngineeringUnit.FmmfLbSec;
								upper = EngineeringUnit.FmmfMlbDay;
								break;

							case EngineeringUnitType.FmuVelocity: // Velocity/Rate
								lower = EngineeringUnit.FmvrIps;
								upper = EngineeringUnit.FmvrMmMin;
								break;

							case EngineeringUnitType.FmuDensity: // Density Units
								lower = EngineeringUnit.FmdGcm3;
								upper = EngineeringUnit.FmdSTnYd3;
								break;

							case EngineeringUnitType.FmuEnergy: // Energy Units
								lower = EngineeringUnit.FmeBtu;
								upper = EngineeringUnit.FmeKwH;
								break;

							case EngineeringUnitType.FmuPower: // Power/Heat XFR
								lower = EngineeringUnit.FmphBtuSec;
								upper = EngineeringUnit.FmphHPower;
								break;

							case EngineeringUnitType.FmuElect: // Electrical
								lower = EngineeringUnit.FmeuMVolts;
								upper = EngineeringUnit.FmeuMho;
								break;

							case EngineeringUnitType.FmuNodim: // Dimensionless
								lower = EngineeringUnit.FmduPwrFct;
								upper = EngineeringUnit.FmduPh;
								break;

							default:
								lower = EngineeringUnit.FmtDegC;
								upper = EngineeringUnit.FmtDegC;
								break;
						}
						this.Session["EngineeringUnits"] = processVariable.ServerUnits;
						this.InitializeUnitsDropDownList(
							this.ServerEngineeringUnitsDropDownList, lower, upper, processVariable.ServerUnits);
					}
					else
					{
						this.ServerEngineeringUnitsDropDownList.Enabled = false;
					}

					// Populate SelectSystemModeDropDownList
					var newItem = new ListItem("List", "0");
					this.SelectSystemModeDropDownList.Items.Add(newItem);
					newItem = new ListItem("Text", "1");
					this.SelectSystemModeDropDownList.Items.Add(newItem);
					this.SelectSystemModeDropDownList.SelectedIndex = 1;
					this.SelectSystemModeDropDownListSelectedIndexChanged(null, null);

					var url = new URL(processVariable.URL);
					this.SystemTextBox.Text = url.HostName;
					this.EnumerateOpcServersBySystemName(this.SystemTextBox.Text);
					this.OPCItemIDTextBox.Text = processVariable.OPCItemID;

					this.DataTypeDropDownListSelectedIndexChanged(null, null);

					// Populate the MessageDropDownList
					if (processVariable.UnitType == UNIT_TYPE.STATION_INPUT_PERMISSIVE
						|| processVariable.UnitType == UNIT_TYPE.LOADARM_INPUT_PERMISSIVE
								|| processVariable.ProcessVariableType == PROCESS_VARIABLE_TYPE.GATE_CONTROL_PV
								|| processVariable.ProcessVariableType == PROCESS_VARIABLE_TYPE.SITE_PERMISSIVE_PV)
					{
						ApplicationStringCollectionClass messagesCollection = FMChannelHelper.MakeCall<IApplicationStrings, ApplicationStringCollectionClass>(
																	x =>
																	x.EnumerateByType(this.Security, STRING_TYPE.PROCESS_VARIABLE_MESSAGE)
																);
						foreach (ApplicationStringClass message in messagesCollection)
						{
							this.MessageDropDownList.Items.Add(new ListItem(message.ID, message.IdentityGuid.ToString()));
							if (processVariable.MessageApplicationStringGuid == message.IdentityGuid)
							{
								this.MessageDropDownList.SelectedIndex = this.MessageDropDownList.Items.Count - 1;
							}
						}
					}
					else
					{
						this.MessageDropDownList.Enabled = false;
					}
				}
					else
					{
						if (this.Request.Form["__EVENTTARGET"] == "HiddenOk")
						{
								this.HiddenOK_Command(null, null);
						}
					}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
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

		protected void SystemDropDownListSelectedIndexChanged(object sender, EventArgs e)
		{
			if (-1 != this.SystemDropDownList.SelectedIndex)
			{
				this.EnumerateOpcServersBySystemName(this.SystemDropDownList.SelectedItem.Text);
			}
		}

		private void CancelCommand(object sender, CommandEventArgs e)
		{
			this.Session.Remove("OPCServer");
//			this.Session.Remove("OPCServerUrl");

			var param = this.Request.GetQueryOrFormValue("Mode") == "Add" ? "?ReturnMode=CancelAdd" : string.Empty;

			this.Redirect((string)this.Session["UnitForm"] + param);
		}

/*
		private void EngineeringUnitsDropDownListSelectedIndexChanged(object sender, EventArgs e)
		{
			SiteClass Site = FMChannelHelper.MakeCall<ISites, SiteClass>(
																	x =>
																	x.Get(this.Security, this.Security.SiteGuid, false, false, true)
																);
			var ProcessVariable = new ProcessVariableClass();

			EngineeringUnit Units = Site.GetSiteUnits(ProcessVariable.SiteVariableType);
			byte DecimalPlaces = Site.GetSiteDecimalPlaces(ProcessVariable.SiteVariableType);

			ProcessVariable.DataType = this.dataType[Convert.ToInt32(this.DataTypeDropDownList.SelectedValue)];
			ProcessVariable.SetMaximum(ProcessVariable.Decode(this.MaximumTextBox.Text, Units), Units);
			ProcessVariable.SetMinimum(ProcessVariable.Decode(this.MinimumTextBox.Text, Units), Units);
			this.MaximumTextBox.Text = ProcessVariable.Encode(
				ProcessVariable.GetMaximum(Units, DecimalPlaces),
				Quality.Good,
				Units,
				Site.GetNumberFormatInfo(ProcessVariable.SiteVariableType));
			this.MinimumTextBox.Text = ProcessVariable.Encode(
				ProcessVariable.GetMinimum(Units, DecimalPlaces),
				Quality.Good,
				Units,
				Site.GetNumberFormatInfo(ProcessVariable.SiteVariableType));
		}
*/

		private void EnumerateOpcServersBySystemName(string systemName)
		{
			try
			{
				this.OPCItemIDTextBox.Text = "";
				this.ItemsTreeView.Nodes.Clear();
				this.ItemsHeirarchicalListBox.Items.Clear();

				this.ItemsTreeView.Visible = false;
				this.ItemsTreeView.Enabled = false;
				this.ItemsHeirarchicalListBox.Visible = false;
				this.ItemsHeirarchicalListBox.Enabled = false;
				this.AvailableOPCItemsLabel.Visible = false;
				this.DataTypeFilterLabel.Visible = false;
				this.DataTypeFilterDropDownList.Visible = false;

				var processVariable = (ProcessVariableClass)this.Session["ProcessVariable"];

				var serverEnumerator = new ServerEnumerator();

				Server[] servers = serverEnumerator.GetAvailableServers(
					Specification.COM_DA_20, systemName, new ConnectData(new NetworkCredential()));

				this.OPCServerDropDownList.Items.Clear();
				this.OPCServerDropDownList.Items.Add(new ListItem(this.GetUnassignedTranslation(), ""));

				foreach (Opc.Da.Server server in servers)
				{
					char[] separator = { '/' };
					string[] strings = server.Url.Path.Split(separator);
					this.OPCServerDropDownList.Items.Add(new ListItem(strings[0], server.Url.ToString()));

					if (server.Url.ToString() == processVariable.URL)
					{
						this.OPCServerDropDownList.SelectedIndex = this.OPCServerDropDownList.Items.Count - 1;
						this.OpcServerDropDownListSelectedIndexChanged(null, null);
					}
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
				var processVariable = (ProcessVariableClass)this.Session["ProcessVariable"];

				if (processVariable.URL != "")
				{
					this.OPCServerDropDownList.Items.Add(new ListItem(processVariable.ProgID, processVariable.URL));
					this.OPCServerDropDownList.SelectedIndex = this.OPCServerDropDownList.Items.Count - 1;
				}
			}
		}

		/// <summary>
		///	This method will return the translated value for "Unassigned" along with
		///	the value being within angle brackets.
		/// </summary>
		/// <returns></returns>
		private string GetUnassignedTranslation()
		{
			string translatedUnassignedText = this.GetTranslatedText("Unassigned");
			translatedUnassignedText = "<" + translatedUnassignedText + ">";

			return translatedUnassignedText;
		}

		/// <summary>
		///	Required method for Designer support - do not modify
		///	the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.OK.Command += this.OkCommand;
				this.HiddenOk.Command += new System.Web.UI.WebControls.CommandEventHandler(this.HiddenOK_Command);
			this.Cancel.Command += this.CancelCommand;
			this.ItemsTreeView.TreeNodeExpanded += this.ItemsTreeViewExpand;
			this.ItemsTreeView.SelectedNodeChanged += this.ItemsTreeViewSelectedNodeChanged;
		}

		private void ItemsTreeViewExpand(object sender, TreeNodeEventArgs e)
		{
			try
			{
				TreeNode node = e.Node;

				node.ChildNodes.Clear();

				using (var opcServer = new Opc.Da.Server(new Factory(false), this.Session["OPCServerUrl"] as URL))
				{

					opcServer.Connect();

					var itemIdentifier = new ItemIdentifier(node.Value);
					var browseFilters = new BrowseFilters { BrowseFilter = browseFilter.branch };
					BrowsePosition browsePosition;

					BrowseElement[] browseElements = opcServer.Browse(itemIdentifier, browseFilters, out browsePosition);

					if (browseElements != null)
					{
						foreach (BrowseElement element in browseElements)
						{
							var itemTreeNode = new TreeNode
													{
															Text = element.Name,
															Value = element.ItemName,
															Expanded = false,
															SelectAction = TreeNodeSelectAction.SelectExpand
													};
							if (element.HasChildren)
							{
								BrowseElement[] subElements = opcServer.Browse(
									new ItemIdentifier(itemTreeNode.Value),
									browseFilters,
									out browsePosition);
								if (subElements != null && subElements.Length != 0)
								{
									var childItemTreeNode = new TreeNode { Expanded = false, SelectAction = TreeNodeSelectAction.SelectExpand };
									itemTreeNode.ChildNodes.Add(childItemTreeNode);
								}
							}
							node.ChildNodes.Add(itemTreeNode);
						}
					}

					opcServer.Disconnect();
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		private void ItemsTreeViewSelectedNodeChanged(object sender, EventArgs e)
		{
			try
			{
				var processVariable = (ProcessVariableClass)this.Session["ProcessVariable"];

				TreeNode node = this.ItemsTreeView.SelectedNode;
				if (node == null)
				{
					throw new Exception("An element of the opc server must be selected from the tree view below!");
				}

				using (var opcServer = new Opc.Da.Server(new Factory(false), this.Session["OPCServerUrl"] as URL))
				{
					opcServer.Connect();

					var itemIdentifier = new ItemIdentifier(node.Value);
					var browseFilters = new BrowseFilters { BrowseFilter = browseFilter.item };
					PropertyID[] propertyIDs = { Property.ACCESSRIGHTS, Property.DATATYPE };
					browseFilters.PropertyIDs = propertyIDs;
					browseFilters.ReturnPropertyValues = true;
					BrowsePosition browsePosition;

					BrowseElement[] browseElements = opcServer.Browse(itemIdentifier, browseFilters, out browsePosition);

					opcServer.Disconnect();

					this.ItemsHeirarchicalListBox.Items.Clear();

					if (browseElements == null)
					{
						return;
					}

					foreach (BrowseElement element in browseElements)
					{
						if (processVariable.Input && element.Properties != null && element.Properties.Length >= 1
							&& (accessRights)element.Properties[0].Value == accessRights.writable)
						{
							continue;
						}

						if (!processVariable.Input && element.Properties != null && element.Properties.Length >= 1
							&& (accessRights)element.Properties[0].Value == accessRights.readable)
						{
							continue;
						}

						if (this.DataTypeFilterDropDownList.SelectedIndex != -1 && element.Properties != null
							&& element.Properties.Length >= 2
							&& this.dataType[Convert.ToInt32(this.DataTypeFilterDropDownList.SelectedValue)] != VarEnum.VT_EMPTY
							&& this.dataType[Convert.ToInt32(this.DataTypeFilterDropDownList.SelectedValue)]
							!= this.VarEnumType((Type)element.Properties[1].Value))
						{
							continue;
						}

						var newListItem = new ListItem(element.Name, element.ItemName);
						foreach (ListItem existingListItem in this.ItemsHeirarchicalListBox.Items)
						{
							if (string.Compare(existingListItem.Text, newListItem.Text, StringComparison.Ordinal) > 0)
							{
								int index = this.ItemsHeirarchicalListBox.Items.IndexOf(existingListItem);
								this.ItemsHeirarchicalListBox.Items.Insert(index, newListItem);
								newListItem = null;
								break;
							}
						}

						if (newListItem != null)
						{
							this.ItemsHeirarchicalListBox.Items.Add(newListItem);
						}
					}
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		private void OkCommand(object sender, CommandEventArgs e)
		{
			bool isConfirmationRequired = false;
			string confirmationMessage = string.Empty;

			try
			{
				SiteClass site = FMChannelHelper.MakeCall<ISites, SiteClass>(
																	x =>
																	x.Get(this.Security, this.Security.SiteGuid, false, false, true)
																);
				var processVariable = (ProcessVariableClass)this.Session["ProcessVariable"];
				ProcessVariableClass testVariable = new ProcessVariableClass(processVariable.ProcessVariableType,
																									processVariable.UnitType,
																									processVariable.DataType,
																									processVariable.Input,
																									this.OPCItemIDTextBox.Text,
																									this.OPCServerDropDownList.SelectedValue,
																									this.OPCServerDropDownList.SelectedItem.Text);
                 testVariable.IdentityGuid = processVariable.IdentityGuid;

                 if (!string.IsNullOrWhiteSpace(testVariable.OPCItemID) && FMChannelHelper.MakeCall<IProcessVariables, bool>(processVariables => processVariables.ProcessVariableAlreadyUsed(this.Security, testVariable)))
				{
					isConfirmationRequired = true;
					confirmationMessage = "An OPC point with tag " + testVariable.OPCItemID + " has already been defined; continue assigning the tag to this point as well?";
				}
				else
				{
					processVariable.DataType = this.dataType[Convert.ToInt32(this.DataTypeDropDownList.SelectedValue)];

					EngineeringUnit units = site.GetSiteUnits(processVariable.SiteVariableType);

					if (this.ServerEngineeringUnitsDropDownList.Enabled)
					{
						processVariable.ServerUnits =
							(EngineeringUnit)Convert.ToInt32(this.ServerEngineeringUnitsDropDownList.SelectedValue);
					}
					if (this.MaximumTextBox.Enabled)
					{
						processVariable.SetMaximum(this.MaximumTextBox.Text, units);
					}
					if (this.MinimumTextBox.Enabled)
					{
						processVariable.SetMinimum(this.MinimumTextBox.Text, units);
					}
					processVariable.OPCItemID = this.OPCItemIDTextBox.Text;

					if (this.OPCServerDropDownList.SelectedItem.Text == this.GetUnassignedTranslation())
					{
						processVariable.URL = "";
						processVariable.ProgID = "";
					}
					else
					{
						processVariable.URL = this.OPCServerDropDownList.SelectedValue;
						processVariable.ProgID = this.OPCServerDropDownList.SelectedItem.Text;
					}

					if (this.MessageDropDownList.SelectedIndex != -1)
					{
						processVariable.MessageID = this.MessageDropDownList.SelectedItem.Text;
						processVariable.MessageApplicationStringGuid = Guid.Parse(this.MessageDropDownList.SelectedValue);
					}
					else
					{
						processVariable.MessageID = "";
						processVariable.MessageApplicationStringGuid = Guid.Empty;
					}
					this.Session["ProcessVariable"] = processVariable;
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
				return;
			}

			if (isConfirmationRequired)
			{
				StringBuilder javaScript = new System.Text.StringBuilder();

				string scriptKey = "ConfirmationScript";

                javaScript.Append("var userConfirmation = window.confirm('" + confirmationMessage + "');\n");
				// Un-comment to only PostBack if user answers OK...
				javaScript.Append("if ( userConfirmation == true )\n");
				javaScript.Append("{ __doPostBack('HiddenOk','');}");

                this.ClientScript.RegisterStartupScript(this.GetType(), scriptKey, javaScript.ToString(), true);
				return;
			}

			this.Redirect((string)this.Session["UnitForm"]);
		}

		private void HiddenOK_Command(object sender, System.Web.UI.WebControls.CommandEventArgs e)
		{
				// HiddenOK gets called after a operator has confirmed that a duplicate tag is intended.  Do the same operation
				// as OK, but don't worry about the duplicate check.
				try
				{
					SiteClass Site = FMChannelHelper.MakeCall<ISites, SiteClass>( Sites => Sites.Get(this.Security, this.Security.SiteGuid, false, false, false));

					ProcessVariableClass processVariable = (ProcessVariableClass)this.Session["ProcessVariable"];

					processVariable.DataType = (VarEnum)this.dataType[System.Convert.ToInt32(this.DataTypeDropDownList.SelectedValue)];

					EngineeringUnit Units = Site.GetSiteUnits(processVariable.SiteVariableType);

					if (this.ServerEngineeringUnitsDropDownList.Enabled)
						processVariable.ServerUnits = (EngineeringUnit)System.Convert.ToInt32(this.ServerEngineeringUnitsDropDownList.SelectedValue);
					if (this.MaximumTextBox.Enabled)
						processVariable.SetMaximum(this.MaximumTextBox.Text, Units);
					if (this.MinimumTextBox.Enabled)
						processVariable.SetMinimum(this.MinimumTextBox.Text, Units);
					processVariable.OPCItemID = this.OPCItemIDTextBox.Text;

					if (this.OPCServerDropDownList.SelectedItem.Text == this.GetUnassignedTranslation())
					{
						processVariable.URL = "";
						processVariable.ProgID = "";
					}
					else
					{
						processVariable.URL = this.OPCServerDropDownList.SelectedValue;
						processVariable.ProgID = this.OPCServerDropDownList.SelectedItem.Text;
					}

					if (this.MessageDropDownList.SelectedIndex != -1)
					{
						processVariable.MessageID = this.MessageDropDownList.SelectedItem.Text;
						processVariable.MessageApplicationStringGuid = Guid.Parse(this.MessageDropDownList.SelectedValue);
					}
					else
					{
						processVariable.MessageID = "";
						processVariable.MessageApplicationStringGuid = Guid.Empty;
					}
				}
				catch (Exception except)
				{
				this.ErrorHandler(except);
					return;
				}

				this.Redirect((string)this.Session["UnitForm"]);
		}


		private void PopulateSystemDropDownList()
		{
			var processVariable = (ProcessVariableClass)this.Session["ProcessVariable"];

			// Populate SystemDropDownList
			this.SystemDropDownList.Items.Clear();
			var newItem = new ListItem("localhost", "0");
			this.SystemDropDownList.Items.Add(newItem);

			var serverList = new List<string>();
			var domain = EnumerateLanMachines.GetDomainOrWorkgroup();
			EnumerateLanMachines.EnumerateMachines(serverList, domain);

			int item = 1;
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

			if (url.HostName != this.SystemDropDownList.SelectedItem.Text)
			{
				newItem = new ListItem(url.HostName, item.ToString());
				this.SystemDropDownList.Items.Add(newItem);
				this.SystemDropDownList.SelectedIndex = this.SystemDropDownList.Items.Count - 1;
			}
		}

		private VarEnum VarEnumType(Type type)
		{
			if (type == typeof(string))
			{
				return VarEnum.VT_BSTR;
			}
			if (type == typeof(bool))
			{
				return VarEnum.VT_BOOL;
			}
			if (type == typeof(sbyte))
			{
				return VarEnum.VT_I1;
			}
			if (type == typeof(short))
			{
				return VarEnum.VT_I2;
			}
			if (type == typeof(int))
			{
				return VarEnum.VT_I4;
			}
			if (type == typeof(long))
			{
				return VarEnum.VT_I8;
			}
			if (type == typeof(int))
			{
				return VarEnum.VT_INT;
			}
			if (type == typeof(byte))
			{
				return VarEnum.VT_UI1;
			}
			if (type == typeof(ushort))
			{
				return VarEnum.VT_UI2;
			}
			if (type == typeof(uint))
			{
				return VarEnum.VT_UI4;
			}
			if (type == typeof(ulong))
			{
				return VarEnum.VT_UI8;
			}
			if (type == typeof(uint))
			{
				return VarEnum.VT_UINT;
			}
			if (type == typeof(float))
			{
				return VarEnum.VT_R4;
			}
			if (type == typeof(double))
			{
				return VarEnum.VT_R8;
			}
			if (type == typeof(DateTime))
			{
				return VarEnum.VT_DATE;
			}
			return VarEnum.VT_NULL;
		}

		#endregion
	}
}