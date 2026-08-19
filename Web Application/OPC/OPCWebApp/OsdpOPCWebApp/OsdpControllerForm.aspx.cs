/******************************************************************************

	FILE NAME:		OptomuxControllerForm.aspx.cs


	PURPOSE:			Implementation of OptomuxControllerForm


	COMMENTS:

		Copyright (C) E+H Systems & Gauging, Inc. Norcross, GA, USA, 2002

		This file shall not be copied or reproduced in any form without
				the express written consent of Endress+HaOptomuxController.


	AUTHOR(S):	W. Gray


	VERSION:		1.0.0  Current version



	MODIFICATION HISTORY:
		Date:			By:			Reason:
		---------	----------  -------------------------------------------

*******************************************************************************/

using System;
using System.Net;
using System.Web.UI.WebControls;

using FMBusinessObjects.DataObjects;

using OsdpOPCObjectsLib;

using OsdpOPCServerLib;

namespace OPCWebApp.OsdpOPCWebApp
{
	/// <summary>
	/// Summary description for OptomuxControllerForm.
	/// </summary>
	public partial class OsdpControllerForm : OsdpFormBase
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			try
			{
				if (!this.Page.IsPostBack)
				{
					this.GetSecurity();

					this.Session.Remove("OsdpController");

					OsdpControllerClass osdpController;


					// Get Index
					if (this.Session["Index"] != null)
					{
						// Get OsdpController
						IOsdpControllers optomuxControllers = (IOsdpControllers)OpcCom.Interop.CreateInstance(new Guid("{f5e1937d-316f-4a07-a31e-77f2246a1b71}"),
																																		(string)this.Session["OsdpControllersSystem"],
																																		new NetworkCredential());

						osdpController = (OsdpControllerClass)optomuxControllers.Get(Convert.ToInt32(this.Session["Index"] as string));

						this.IDTextBox.Text = osdpController.ID;
					}
					else
						osdpController = new OsdpControllerClass();

					ListItem newItem;

					// Populate the AddressDropDownList
					// Address for OSDP is the lowest 7 bits of the Address byte, allowing a range of 00 to 7F
					// 7F is reserved for broadcast, so allow selection of 0 to 7E (0 to 126)
					for (int iAddress = 0; iAddress < 127; iAddress++)
					{
						newItem = new ListItem(iAddress.ToString("X2"), iAddress.ToString());
						this.AddressDropDownList.Items.Add(newItem);
						if (osdpController.Address == iAddress)
						{
							this.AddressDropDownList.SelectedIndex = this.AddressDropDownList.Items.Count - 1;
						}
					}

					// Populate PortDropDownList			

					IPorts ports = (IPorts)OpcCom.Interop.CreateInstance(new Guid("{61cbf9b2-19af-4532-82ae-99b6e23b6efa}"),
																							(string)this.Session["OsdpControllersSystem"],
																							new NetworkCredential());
					PortCollectionClass portCollection = (PortCollectionClass)ports.Enumerate();
					for (int item = 0; item < portCollection.Count; item++)
					{
						PortClass port = (PortClass)portCollection.Item(item);
						newItem = new ListItem(port.ID, port.Index.ToString());
						foreach (ListItem existingItem in this.PortDropDownList.Items)
						{
							if (string.Compare(existingItem.Text, newItem.Text, StringComparison.Ordinal) > 0)
							{
								int index = this.PortDropDownList.Items.IndexOf(existingItem);
								this.PortDropDownList.Items.Insert(index, newItem);
								if (osdpController.PortIndex == port.Index)
								{
									this.PortDropDownList.SelectedIndex = index;
								}
								newItem = null;
								break;
							}
						}

						if (newItem != null)
						{
							this.PortDropDownList.Items.Add(newItem);
							if (osdpController.PortIndex == port.Index)
							{
								this.PortDropDownList.SelectedIndex = this.PortDropDownList.Items.Count - 1;
							}
						}
					}

					newItem = new ListItem(this.GetDictionaryText("{None}"), "0");
					this.PortDropDownList.Items.Insert(0, newItem);

					this.Session["OsdpController"] = osdpController;

					if (!this.Security.HasRight(RIGHT.MODIFY_SITES_AND_SITE_GROUPS))
					{
						this.OKButton.Enabled = false;
					}
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}


		#region Web Form Designer generated code
		override protected void OnInit(EventArgs e)
		{
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			this.InitializeComponent();
			base.OnInit(e);
		}

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.OKButton.Command += new CommandEventHandler(this.OK_Command);
			this.CancelButton.Command += new CommandEventHandler(this.Cancel_Command);

		}
		#endregion

		private void OK_Command(object sender, CommandEventArgs e)
		{
			try
			{
				IOsdpControllers osdpControllers = (IOsdpControllers)OpcCom.Interop.CreateInstance(new Guid("{f5e1937d-316f-4a07-a31e-77f2246a1b71}"),
																																(string)this.Session["OptomuxControllersSystem"],
																																new NetworkCredential());

				OsdpControllerClass osdpController = (OsdpControllerClass)this.Session["OsdpController"];

				osdpController.ID = this.IDTextBox.Text;
				if (this.AddressDropDownList.SelectedIndex != -1)
				{
					osdpController.Address = Convert.ToByte(this.AddressDropDownList.SelectedItem.Text);
				}

				if (this.PortDropDownList.SelectedIndex != -1)
				{
					osdpController.PortIndex = Convert.ToInt32(this.PortDropDownList.SelectedItem.Value);
				}

				try
				{
					if (osdpController.Index != 0)
						osdpControllers.Modify(osdpController);
					else
						osdpControllers.Add(osdpController);
				}
				catch (System.Runtime.InteropServices.COMException ex)
				{
					if (ex.Message.Contains("duplicate key") || ex.Message.Contains("Osdp Controller Exists"))
					{
						throw new Exception("OPC Server Exists");
					}
					else
					{
						throw new Exception("Database Error");
					}
				}
			}

			catch (Exception except)
			{
				this.ErrorHandler(except);
				return;
			}
			this.Redirect("OsdpControllersForm.aspx");
			this.Session.Remove("OsdpController");
		}

		private void Cancel_Command(object sender, CommandEventArgs e)
		{
			this.Redirect("OsdpControllersForm.aspx");
			this.Session.Remove("OsdpController");
		}

	}
}
