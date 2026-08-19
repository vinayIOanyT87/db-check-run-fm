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
using OptomuxOPCObjectsLib;
using OptomuxOPCServerLib;

namespace OPCWebApp.OptomuxOPCWebApp
{
	/// <summary>
	/// Summary description for OptomuxControllerForm.
	/// </summary>
	public partial class OptomuxControllerForm : OsdpFormBase
	{
	
		protected ListItemCollection EnumerateOptomuxControllerTypes()
		{
			OptomuxControllerClass optomuxController=new OptomuxControllerClass();

			ListItemCollection	optomuxControllerTypeItems=new ListItemCollection();

			for(OPTOMUX_TYPE type=OPTOMUX_TYPE.PASSCONTROLLER_HC05;type < OPTOMUX_TYPE.MAX_OPTOMUX_TYPE;type++)
				optomuxControllerTypeItems.Add(new ListItem(optomuxController.TypeID(type),((int) type).ToString()));

			return optomuxControllerTypeItems;
		}


		protected void Page_Load(object sender, EventArgs e)
		{
			try
			{
				if (! this.Page.IsPostBack) 
				{
					this.GetSecurity();

					this.Session.Remove("OptomuxController");

					OptomuxControllerClass	optomuxController;


					// Get Index
					if(this.Session["Index"] != null)
					{
						// Get OptomuxController
						IOptomuxControllers optomuxControllers=(IOptomuxControllers) OpcCom.Interop.CreateInstance(	new Guid("{DD940B4F-C212-4361-8FDE-D4061584E4D0}"),
																																		(string) this.Session["OptomuxControllersSystem"],
																																		new NetworkCredential());

						optomuxController=(OptomuxControllerClass) optomuxControllers.Get(Convert.ToInt32(this.Session["Index"] as string));

						this.IDTextBox.Text=optomuxController.ID;
					}
					else
						optomuxController=new OptomuxControllerClass();

					// Populate TypeDropDownList
					ListItem newItem;
					for(OPTOMUX_TYPE type=OPTOMUX_TYPE.PASSCONTROLLER_HC05;type < OPTOMUX_TYPE.MAX_OPTOMUX_TYPE;type++)
					{
						newItem=new ListItem(optomuxController.TypeID(type),((int) type).ToString());
						foreach(ListItem existingItem in this.TypeDropDownList.Items)
						{
							if(string.Compare(existingItem.Text, newItem.Text, StringComparison.Ordinal) < 0)
							{
								int index=this.TypeDropDownList.Items.IndexOf(existingItem);
								this.TypeDropDownList.Items.Insert(index,newItem);
								if(((int) optomuxController.Type).ToString() == newItem.Value)
									this.TypeDropDownList.SelectedIndex=index;
								newItem=null;
								break;
							}
						}

						if(newItem != null)
						{
							this.TypeDropDownList.Items.Add(newItem);
							if(((int) optomuxController.Type).ToString() == newItem.Value)
								this.TypeDropDownList.SelectedIndex=this.TypeDropDownList.Items.Count-1;
						}
					}

					// Populate the AddressDropDownList
					for(int iAddress=0;iAddress < 256;iAddress++)
					{
						newItem=new ListItem(iAddress.ToString("X2"),iAddress.ToString());
						this.AddressDropDownList.Items.Add(newItem);
						if(optomuxController.Address == iAddress)
							this.AddressDropDownList.SelectedIndex=this.AddressDropDownList.Items.Count-1;
					}

					// Populate PortDropDownList			
					
					IPorts ports=(IPorts) OpcCom.Interop.CreateInstance(	new Guid("{D1CAA238-8AB9-4E70-A628-49AB61EC5BD1}"),
																							(string) this.Session["OptomuxControllersSystem"],
																							new NetworkCredential());
					PortCollectionClass portCollection=(PortCollectionClass) ports.Enumerate();
					for(int item=0;item < portCollection.Count;item++)
					{
						PortClass port=(PortClass) portCollection.Item(item);
						newItem=new ListItem(port.ID,port.Index.ToString());
						foreach(ListItem existingItem in this.PortDropDownList.Items)
						{
							if(string.Compare(existingItem.Text, newItem.Text, StringComparison.Ordinal) > 0)
							{
								int index=this.PortDropDownList.Items.IndexOf(existingItem);
								this.PortDropDownList.Items.Insert(index,newItem);
								if(optomuxController.PortIndex == port.Index)
									this.PortDropDownList.SelectedIndex=index;
								newItem=null;
								break;
							}
						}

						if(newItem != null)
						{
							this.PortDropDownList.Items.Add(newItem);
							if(optomuxController.PortIndex == port.Index)
								this.PortDropDownList.SelectedIndex=this.PortDropDownList.Items.Count-1;
						}
					}

                    newItem = new ListItem(this.GetDictionaryText("{None}"), "0");
                    this.PortDropDownList.Items.Insert(0, newItem);

                    if (optomuxController.NetworkCommunications)
					{
						this.PortDropDownList.Enabled=false;
						this.AddressDropDownList.Enabled=false;
						this.IPAddressTextBox.Text=optomuxController.IPAddress;
						this.PortTextBox.Text=optomuxController.Port.ToString();
					}
					else
					{
						this.IPAddressTextBox.Text="";
						this.PortTextBox.Text="";
						this.IPAddressTextBox.Enabled=false;
					}

					this.NetworkCommunicationsRadioButton.Checked=optomuxController.NetworkCommunications;
					this.SerialCommunicationsRadioButton.Checked=!optomuxController.NetworkCommunications;

					this.Module1InputRadioButton.Checked=(optomuxController.ModuleInputOutputMap & 0x01) != 0;
					this.Module2InputRadioButton.Checked=(optomuxController.ModuleInputOutputMap & 0x02) != 0;
					this.Module3InputRadioButton.Checked=(optomuxController.ModuleInputOutputMap & 0x04) != 0;
					this.Module4InputRadioButton.Checked=(optomuxController.ModuleInputOutputMap & 0x08) != 0;
					this.Module5InputRadioButton.Checked=(optomuxController.ModuleInputOutputMap & 0x10) != 0;
					this.Module6InputRadioButton.Checked=(optomuxController.ModuleInputOutputMap & 0x20) != 0;
					this.Module7InputRadioButton.Checked=(optomuxController.ModuleInputOutputMap & 0x40) != 0;
					this.Module8InputRadioButton.Checked=(optomuxController.ModuleInputOutputMap & 0x80) != 0;
					this.Module1OutputRadioButton.Checked=(optomuxController.ModuleInputOutputMap & 0x01) == 0;
					this.Module2OutputRadioButton.Checked=(optomuxController.ModuleInputOutputMap & 0x02) == 0;
					this.Module3OutputRadioButton.Checked=(optomuxController.ModuleInputOutputMap & 0x04) == 0;
					this.Module4OutputRadioButton.Checked=(optomuxController.ModuleInputOutputMap & 0x08) == 0;
					this.Module5OutputRadioButton.Checked=(optomuxController.ModuleInputOutputMap & 0x10) == 0;
					this.Module6OutputRadioButton.Checked=(optomuxController.ModuleInputOutputMap & 0x20) == 0;
					this.Module7OutputRadioButton.Checked=(optomuxController.ModuleInputOutputMap & 0x40) == 0;
					this.Module8OutputRadioButton.Checked=(optomuxController.ModuleInputOutputMap & 0x80) == 0;

					this.Session["OptomuxController"]=optomuxController;

					if(!this.Security.HasRight(RIGHT.MODIFY_SITES_AND_SITE_GROUPS))
						this.OKButton.Enabled=false;
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
				IOptomuxControllers optomuxControllers=(IOptomuxControllers) OpcCom.Interop.CreateInstance(	new Guid("{DD940B4F-C212-4361-8FDE-D4061584E4D0}"),
																																(string) this.Session["OptomuxControllersSystem"],
																																new NetworkCredential());

				OptomuxControllerClass	optomuxController=(OptomuxControllerClass) this.Session["OptomuxController"];

				optomuxController.ID=this.IDTextBox.Text;
				if(this.TypeDropDownList.SelectedIndex != -1)
					optomuxController.Type=(OPTOMUX_TYPE) Convert.ToInt32(this.TypeDropDownList.SelectedValue);
				if(this.AddressDropDownList.SelectedIndex != -1)
					optomuxController.Address=Convert.ToByte(this.AddressDropDownList.SelectedItem.Text);
				if(this.PortDropDownList.SelectedIndex != -1)
					optomuxController.PortIndex=Convert.ToInt32(this.PortDropDownList.SelectedItem.Value);

				optomuxController.NetworkCommunications=this.NetworkCommunicationsRadioButton.Checked;
				optomuxController.IPAddress=this.IPAddressTextBox.Text;
				if(optomuxController.NetworkCommunications)
					optomuxController.Port=Convert.ToInt32(this.PortTextBox.Text);

				optomuxController.ModuleInputOutputMap=(byte) (this.Module1InputRadioButton.Checked ?  0x01 : 0);
				optomuxController.ModuleInputOutputMap+=(byte) (this.Module2InputRadioButton.Checked ? 0x02 : 0);
				optomuxController.ModuleInputOutputMap+=(byte) (this.Module3InputRadioButton.Checked ? 0x04 : 0);
				optomuxController.ModuleInputOutputMap+=(byte) (this.Module4InputRadioButton.Checked ? 0x08 : 0);
				optomuxController.ModuleInputOutputMap+=(byte) (this.Module5InputRadioButton.Checked ? 0x10 : 0);
				optomuxController.ModuleInputOutputMap+=(byte) (this.Module6InputRadioButton.Checked ? 0x20 : 0);
				optomuxController.ModuleInputOutputMap+=(byte) (this.Module7InputRadioButton.Checked ? 0x40 : 0);
				optomuxController.ModuleInputOutputMap+=(byte) (this.Module8InputRadioButton.Checked ? 0x80 : 0);
                try
                {
                    if (optomuxController.Index != 0)
                        optomuxControllers.Modify(optomuxController);
                    else
                        optomuxControllers.Add(optomuxController);
                }
                catch (System.Runtime.InteropServices.COMException ex)
                {
                    if (ex.Message.Contains("duplicate key") || ex.Message.Contains("Optomux Controller Exists")) 
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
			this.Redirect("OptomuxControllersForm.aspx");
			this.Session.Remove("OptomuxController");
		}

		private void Cancel_Command(object sender, CommandEventArgs e)
		{
			this.Redirect("OptomuxControllersForm.aspx");
			this.Session.Remove("OptomuxController");
		}

		protected void SerialCommunicationsRadioButton_CheckedChanged(object sender, EventArgs e)
		{
			this.PortDropDownList.Enabled=this.SerialCommunicationsRadioButton.Checked;
			this.AddressDropDownList.Enabled=this.SerialCommunicationsRadioButton.Checked;
			this.IPAddressTextBox.Enabled=!this.SerialCommunicationsRadioButton.Checked;
			this.PortTextBox.Enabled=!this.SerialCommunicationsRadioButton.Checked;
			if(this.SerialCommunicationsRadioButton.Checked)
			{
				this.IPAddressTextBox.Text="";
				this.PortTextBox.Text="";
			}
			else
				this.PortDropDownList.SelectedIndex=0;
		}

		protected void NetworkCommunicationsRadioButton_CheckedChanged(object sender, EventArgs e)
		{
			this.PortDropDownList.Enabled=!this.NetworkCommunicationsRadioButton.Checked;
			this.AddressDropDownList.Enabled=!this.NetworkCommunicationsRadioButton.Checked;
			this.IPAddressTextBox.Enabled=this.NetworkCommunicationsRadioButton.Checked;
			this.PortTextBox.Enabled=this.NetworkCommunicationsRadioButton.Checked;
			if(!this.NetworkCommunicationsRadioButton.Checked)
			{
				this.IPAddressTextBox.Text="";
				this.PortTextBox.Text="";
			}
            else
            {
                this.PortDropDownList.SelectedIndex = this.PortDropDownList.Items.Count - 1;
            }
        }
    }
}
