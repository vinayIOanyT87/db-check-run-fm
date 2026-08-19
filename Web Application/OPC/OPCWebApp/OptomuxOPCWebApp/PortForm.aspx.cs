/******************************************************************************

	FILE NAME:		PortForm.aspx.cs


	PURPOSE:			Implementation of PortForm


	COMMENTS:

		Copyright (C) E+H Systems & Gauging, Inc. Norcross, GA, USA, 2002

		This file shall not be copied or reproduced in any form without
				the express written consent of Endress+HaAccuload.


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

using FuelsManager.FMWebApp;

using OptomuxOPCObjectsLib;

using OptomuxOPCServerLib;

namespace OPCWebApp.OptomuxOPCWebApp
{
   /// <summary>
   /// Summary description for PortForm.
   /// </summary>
   public partial class PortForm : FMAutoSubmitFormBase
	{
	
		protected void Page_Load(object sender, EventArgs e)
		{
			try
			{
				if (!this.Page.IsPostBack) 
				{
				    this.GetSecurity();

				    this.Session.Remove("Port");

					IPorts ports=(IPorts) OpcCom.Interop.CreateInstance(	new Guid("{D1CAA238-8AB9-4E70-A628-49AB61EC5BD1}"),
																							(string) this.Session["OptomuxControllersSystem"],
																							new NetworkCredential());


					PortClass	port;

					// Get Index
					if(this.Session["Index"] != null)
						port=(PortClass) ports.Get(Convert.ToInt32(this.Session["Index"] as string));
					else
						port=new PortClass();


					// Populate PortDropDownList
					string[] names=(string []) ports.EnumeratePortIDs();

					int portIndex=0;
					if ( names != null )
					{
						foreach(string name in names)
						{
							ListItem newItem=new ListItem(name,portIndex.ToString());

							foreach(ListItem existingItem in this.PortDropDownList.Items)
							{
								if(string.Compare(existingItem.Text, newItem.Text, StringComparison.Ordinal) > 0)
								{
									int index=this.PortDropDownList.Items.IndexOf(existingItem);
									this.PortDropDownList.Items.Insert(index,newItem);
									newItem=null;
									break;
								}
							}

							if(newItem != null)
								this.PortDropDownList.Items.Add(newItem);

							portIndex++;
						}
					}

					if(port.Index != 0)
					{
						ListItem newItem=new ListItem(port.ID,portIndex.ToString());

						foreach(ListItem existingItem in this.PortDropDownList.Items)
						{
							if(string.Compare(existingItem.Text, newItem.Text, StringComparison.Ordinal) > 0)
							{
								int index=this.PortDropDownList.Items.IndexOf(existingItem);
								this.PortDropDownList.Items.Insert(index,newItem);
								this.PortDropDownList.SelectedIndex=index;
								newItem=null;
								break;
							}
						}

						if(newItem != null)
						{
							this.PortDropDownList.Items.Add(newItem);
							this.PortDropDownList.SelectedIndex=this.PortDropDownList.Items.Count-1;
						}
					}
					
					// Populate BaudDropDownList
					for(OPTOMUX_BAUD baud=OPTOMUX_BAUD.OPTOMUX_BAUD_1200;baud < OPTOMUX_BAUD.MAX_OPTOMUX_BAUD;baud++)
					{
						ListItem newItem=new ListItem(port.BaudID(baud),((int) baud).ToString());
						this.BaudDropDownList.Items.Add(newItem);
						if(((int) port.Baud).ToString() == newItem.Value)
							this.BaudDropDownList.SelectedIndex=this.BaudDropDownList.Items.Count-1;
					}

					// Populate DataBitsDownList
					for(OPTOMUX_DATA_BITS dataBits=OPTOMUX_DATA_BITS.DATA_BITS_7;dataBits < OPTOMUX_DATA_BITS.MAX_OPTOMUX_DATA_BITS;dataBits++)
					{
						ListItem newItem=new ListItem(port.DataBitsID(dataBits),((int) dataBits).ToString());
						this.DataBitsDropDownList.Items.Add(newItem);
						if(((int) port.DataBits).ToString() == newItem.Value)
							this.DataBitsDropDownList.SelectedIndex=this.DataBitsDropDownList.Items.Count-1;
					}

					// Populate ParityDownList
					for(OPTOMUX_PARITY parity=OPTOMUX_PARITY.OPTOMUX_PARITY_NONE;parity < OPTOMUX_PARITY.MAX_OPTOMUX_PARITY;parity++)
					{
						ListItem newItem=new ListItem("Optomux|"+port.ParityID(parity),((int) parity).ToString());
						foreach(ListItem existingItem in this.ParityDropDownList.Items)
						{
							if(string.Compare(existingItem.Text, newItem.Text, StringComparison.Ordinal) < 0)
							{
								int index=this.ParityDropDownList.Items.IndexOf(existingItem);
								this.ParityDropDownList.Items.Insert(index,newItem);
								if(((int) port.Parity).ToString() == newItem.Value)
									this.ParityDropDownList.SelectedIndex=index;
								newItem=null;
								break;
							}
						}

						if(newItem != null)
						{
							this.ParityDropDownList.Items.Add(newItem);
							if(((int) port.Parity).ToString() == newItem.Value)
								this.ParityDropDownList.SelectedIndex=this.ParityDropDownList.Items.Count-1;
						}
					}

					// Populate StopBitsDownList
					for(OPTOMUX_STOP_BITS stopBits=OPTOMUX_STOP_BITS.STOP_BITS_1;stopBits < OPTOMUX_STOP_BITS.MAX_OPTOMUX_STOP_BITS;stopBits++)
					{
						ListItem newItem=new ListItem(port.StopBitsID(stopBits),((int) stopBits).ToString());
						this.StopBitsDropDownList.Items.Add(newItem);
						if(((int) port.StopBits).ToString() == newItem.Value)
							this.StopBitsDropDownList.SelectedIndex=this.StopBitsDropDownList.Items.Count-1;
					}

				    this.Session["Port"]=port;

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
			this.CancelButton.Command += new System.Web.UI.WebControls.CommandEventHandler(this.Cancel_Command);
			this.OKButton.Command += new System.Web.UI.WebControls.CommandEventHandler(this.OK_Command);

		}
		#endregion

		private void OK_Command(object sender, CommandEventArgs e)
		{
			try
			{
				IPorts ports=(IPorts) OpcCom.Interop.CreateInstance(	new Guid("{D1CAA238-8AB9-4E70-A628-49AB61EC5BD1}"),
																						(string) this.Session["OptomuxControllersSystem"],
																						new NetworkCredential());

				PortClass	port=(PortClass) this.Session["Port"];

				if(this.PortDropDownList.SelectedIndex != -1)
					port.ID=this.PortDropDownList.SelectedItem.Text;
				if(this.BaudDropDownList.SelectedIndex != -1)
					port.Baud=(OPTOMUX_BAUD) Convert.ToInt32(this.BaudDropDownList.SelectedValue);
				if(this.DataBitsDropDownList.SelectedIndex != -1)
					port.DataBits=(OPTOMUX_DATA_BITS) Convert.ToInt32(this.DataBitsDropDownList.SelectedValue);
				if(this.ParityDropDownList.SelectedIndex != -1)
					port.Parity=(OPTOMUX_PARITY) Convert.ToInt32(this.ParityDropDownList.SelectedValue);
				if(this.StopBitsDropDownList.SelectedIndex != -1)
					port.StopBits=(OPTOMUX_STOP_BITS) Convert.ToInt32(this.StopBitsDropDownList.SelectedValue);

				if(port.Index != 0)
					ports.Modify(port);
				else
					ports.Add(port);
			}
			catch (Exception except)
			{
			    this.ErrorHandler(except);
				return;
			}
			this.Redirect("PortsForm.aspx");
		    this.Session.Remove("Port");
		}

		private void Cancel_Command(object sender, CommandEventArgs e)
		{
			this.Redirect("PortsForm.aspx");
		    this.Session.Remove("Port");
		}
	}
}
