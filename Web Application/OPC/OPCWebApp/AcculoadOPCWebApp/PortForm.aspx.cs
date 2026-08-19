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
using AcculoadOPCObjectsLib;
using AcculoadOPCServerLib;
using FMBusinessObjects.DataObjects;
using FuelsManager.FMWebApp;
using FMControls;

namespace OPCWebApp.AcculoadOPCWebApp
{
	/// <summary>
	/// Summary description for PortForm.
	/// </summary>
	public class PortForm : FMAutoSubmitFormBase
	{
		protected Image Image1;
		protected FMButton CancelButton;
		protected FMButton OKButton;
		protected DropDownList StopBitsDropDownList;
		protected FMLabel Label8;
		protected DropDownList DataBitsDropDownList;
		protected FMLabel Label7;
		protected FMDropDownList ParityDropDownList;
		protected FMLabel Label6;
		protected DropDownList BaudDropDownList;
		protected FMLabel Label5;
		protected FMLabel Label2;
		protected FMLabel Label1;
		protected DropDownList PortDropDownList;
	
		protected void Page_Load(object sender, EventArgs e)
		{
			try
			{
				if (! this.Page.IsPostBack) 
				{
					this.GetSecurity();

					this.Session.Remove("Port");

					IPorts ports=(IPorts) OpcCom.Interop.CreateInstance(	new Guid("{2070F4BA-651D-4268-9F5A-1EBE0A137141}"),
																							(string) this.Session["SmithMeterSystem"],
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
					for(ACCULOAD_BAUD baud=ACCULOAD_BAUD.ACCULOAD_BAUD_1200;baud < ACCULOAD_BAUD.MAX_ACCULOAD_BAUD;baud++)
					{
						ListItem newItem=new ListItem(port.BaudID(baud),((int) baud).ToString());
						this.BaudDropDownList.Items.Add(newItem);
						if(((int) port.Baud).ToString() == newItem.Value)
							this.BaudDropDownList.SelectedIndex=this.BaudDropDownList.Items.Count-1;
					}

					// Populate DataBitsDownList
					for(ACCULOAD_DATA_BITS dataBits=ACCULOAD_DATA_BITS.DATA_BITS_7;dataBits < ACCULOAD_DATA_BITS.MAX_ACCULOAD_DATA_BITS;dataBits++)
					{
						ListItem newItem=new ListItem(port.DataBitsID(dataBits),((int) dataBits).ToString());
						this.DataBitsDropDownList.Items.Add(newItem);
						if(((int) port.DataBits).ToString() == newItem.Value)
							this.DataBitsDropDownList.SelectedIndex=this.DataBitsDropDownList.Items.Count-1;
					}

					// Populate ParityDownList
					for(ACCULOAD_PARITY parity=ACCULOAD_PARITY.ACCULOAD_PARITY_NONE;parity < ACCULOAD_PARITY.MAX_ACCULOAD_PARITY;parity++)
					{
						ListItem newItem=new ListItem( "SmithMeter|" + port.ParityID(parity),((int) parity).ToString());
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
					for(ACCULOAD_STOP_BITS stopBits=ACCULOAD_STOP_BITS.STOP_BITS_1;stopBits < ACCULOAD_STOP_BITS.MAX_ACCULOAD_STOP_BITS;stopBits++)
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
				IPorts ports=(IPorts) OpcCom.Interop.CreateInstance(	new Guid("{2070F4BA-651D-4268-9F5A-1EBE0A137141}"),
					(string) this.Session["SmithMeterSystem"],
					new NetworkCredential());

				PortClass	port=(PortClass) this.Session["Port"];

				if(this.PortDropDownList.SelectedIndex != -1)
					port.ID=this.PortDropDownList.SelectedItem.Text;
				if(this.BaudDropDownList.SelectedIndex != -1)
					port.Baud=(ACCULOAD_BAUD) Convert.ToInt32(this.BaudDropDownList.SelectedValue);
				if(this.DataBitsDropDownList.SelectedIndex != -1)
					port.DataBits=(ACCULOAD_DATA_BITS) Convert.ToInt32(this.DataBitsDropDownList.SelectedValue);
				if(this.ParityDropDownList.SelectedIndex != -1)
					port.Parity=(ACCULOAD_PARITY) Convert.ToInt32(this.ParityDropDownList.SelectedValue);
				if(this.StopBitsDropDownList.SelectedIndex != -1)
					port.StopBits=(ACCULOAD_STOP_BITS) Convert.ToInt32(this.StopBitsDropDownList.SelectedValue);

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
