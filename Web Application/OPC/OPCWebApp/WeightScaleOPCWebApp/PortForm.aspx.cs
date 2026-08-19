/******************************************************************************

	FILE NAME:		PortForm.aspx.cs


	PURPOSE:			Implementation of PortForm


	COMMENTS:

		Copyright (C) E+H Systems & Gauging, Inc. Norcross, GA, USA, 2002

		This file shall not be copied or reproduced in any form without
				the express written consent of Endress+HaWeightScale.


	AUTHOR(S):	W. Gray


	VERSION:		1.0.0  Current version



	MODIFICATION HISTORY:
		Date:			By:			Reason:
		---------	----------  -------------------------------------------

*******************************************************************************/
using System;
using System.Net;
using System.Web.UI;
using System.Web.UI.WebControls;
using WeightScaleOPCObjectsLib;
using WeightScaleOPCServerLib;

using FMBusinessObjects.DataObjects;
using FuelsManager.FMWebApp;

namespace WeightScaleOPCWebApp
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
                GetSecurity();

                if (!Page.IsPostBack) 
				{
					Session.Remove("Port");

					IPorts Ports=(IPorts) OpcCom.Interop.CreateInstance(	new Guid("{265331A0-40D0-4DEC-B614-2A21CDC5CC1F}"),
																							(string) Session["WeightScaleSystem"],
																							new NetworkCredential());


					PortClass	Port = Session["Index"] != null ? (PortClass)Ports.Get(Convert.ToInt32(Session["Index"] as string)) : new PortClass();

               // Get Index


               // Populate PortDropDownList
               string[] Names=(string []) Ports.EnumeratePortIDs();

					int PortIndex=0;
					if ( Names != null )
					{
						foreach(string Name in Names)
						{
                            ListItem NewItem = new ListItem(Name, PortIndex.ToString());

							foreach(ListItem ExistingItem in PortDropDownList.Items)
							{
								if(ExistingItem.Text.CompareTo(NewItem.Text) > 0)
								{
									int Index=PortDropDownList.Items.IndexOf(ExistingItem);
									PortDropDownList.Items.Insert(Index,NewItem);
									NewItem=null;
									break;
								}
							}

							if(NewItem != null)
								PortDropDownList.Items.Add(NewItem);

							PortIndex++;
						}
					}

					if(Port.Index != 0)
					{
                        ListItem NewItem = new ListItem(Port.ID, PortIndex.ToString());

						foreach(ListItem ExistingItem in PortDropDownList.Items)
						{
							if(ExistingItem.Text.CompareTo(NewItem.Text) > 0)
							{
								int Index=PortDropDownList.Items.IndexOf(ExistingItem);
								PortDropDownList.Items.Insert(Index,NewItem);
								PortDropDownList.SelectedIndex=Index;
								NewItem=null;
								break;
							}
						}

						if(NewItem != null)
						{
							PortDropDownList.Items.Add(NewItem);
							PortDropDownList.SelectedIndex=PortDropDownList.Items.Count-1;
						}
					}
					
					// Populate BaudDropDownList
					for (WEIGHTSCALE_BAUD Baud = WEIGHTSCALE_BAUD.WEIGHTSCALE_BAUD_1200; Baud < WEIGHTSCALE_BAUD.MAX_WEIGHTSCALE_BAUD; Baud++)
					{
                        ListItem NewItem = new ListItem(Port.BaudID(Baud), ((int)Baud).ToString());
						BaudDropDownList.Items.Add(NewItem);
                        if (((int)Port.Baud).ToString() == NewItem.Value)
                        {
                            BaudDropDownList.SelectedIndex = BaudDropDownList.Items.Count - 1;
                        }
					}

					// Populate DataBitsDownList
					for (WEIGHTSCALE_DATA_BITS DataBits = WEIGHTSCALE_DATA_BITS.DATA_BITS_7; DataBits < WEIGHTSCALE_DATA_BITS.MAX_WEIGHTSCALE_DATA_BITS; DataBits++)
					{
                        ListItem NewItem = new ListItem(Port.DataBitsID(DataBits), ((int)DataBits).ToString());
						DataBitsDropDownList.Items.Add(NewItem);
                        if (((int)Port.DataBits).ToString() == NewItem.Value)
                        {
                            DataBitsDropDownList.SelectedIndex = DataBitsDropDownList.Items.Count - 1;
                        }
					}

					// Populate ParityDownList
					for (WEIGHTSCALE_PARITY Parity = WEIGHTSCALE_PARITY.WEIGHTSCALE_PARITY_NONE; Parity < WEIGHTSCALE_PARITY.MAX_WEIGHTSCALE_PARITY; Parity++)
					{
                        ListItem NewItem = new ListItem("WeightScale|" + Port.ParityID(Parity), ((int)Parity).ToString());
						foreach(ListItem ExistingItem in ParityDropDownList.Items)
						{
							if(ExistingItem.Text.CompareTo(NewItem.Text) < 0)
							{
								int Index=ParityDropDownList.Items.IndexOf(ExistingItem);
								ParityDropDownList.Items.Insert(Index,NewItem);
                                if (((int)Port.Parity).ToString() == NewItem.Value)
                                {
                                    ParityDropDownList.SelectedIndex = Index;
                                }
								NewItem=null;
								break;
							}
						}

						if(NewItem != null)
						{
							ParityDropDownList.Items.Add(NewItem);
                            if (((int)Port.Parity).ToString() == NewItem.Value)
                            {
                                ParityDropDownList.SelectedIndex = ParityDropDownList.Items.Count - 1;
                            }
						}
					}

					// Populate StopBitsDownList
					for (WEIGHTSCALE_STOP_BITS StopBits = WEIGHTSCALE_STOP_BITS.STOP_BITS_1; StopBits < WEIGHTSCALE_STOP_BITS.MAX_WEIGHTSCALE_STOP_BITS; StopBits++)
					{
                        ListItem NewItem = new ListItem(Port.StopBitsID(StopBits), ((int)StopBits).ToString());
						StopBitsDropDownList.Items.Add(NewItem);
                        if (((int)Port.StopBits).ToString() == NewItem.Value)
                        {
                            StopBitsDropDownList.SelectedIndex = StopBitsDropDownList.Items.Count - 1;
                        }
					}

					Session["Port"]=Port;

					if(!Security.HasRight(RIGHT.MODIFY_SITES_AND_SITE_GROUPS) ||
						PortDropDownList.Items.Count <= 0)
						OKButton.Enabled=false;
				}
			}
			catch (Exception except)
			{
				ErrorHandler(except);
			}
		}

		#region Web Form Designer generated code
		override protected void OnInit(EventArgs e)
		{
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			InitializeComponent();
			base.OnInit(e);
		}
		
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{    
			this.CancelButton.Command += new CommandEventHandler(this.Cancel_Command);
			this.OKButton.Command += new CommandEventHandler(this.OK_Command);

		}
		#endregion

		private void OK_Command(object sender, CommandEventArgs e)
		{
			try
			{
				IPorts Ports=(IPorts) OpcCom.Interop.CreateInstance(	new Guid("{265331A0-40D0-4DEC-B614-2A21CDC5CC1F}"),
																						(string)Session["WeightScaleSystem"],
																						new NetworkCredential());

				PortClass	Port=(PortClass) Session["Port"];

				if(PortDropDownList.SelectedIndex != -1)
					Port.ID=PortDropDownList.SelectedItem.Text;
				if(BaudDropDownList.SelectedIndex != -1)
                    Port.Baud = (WEIGHTSCALE_BAUD)Convert.ToInt32(BaudDropDownList.SelectedValue);
				if(DataBitsDropDownList.SelectedIndex != -1)
                    Port.DataBits = (WEIGHTSCALE_DATA_BITS)Convert.ToInt32(DataBitsDropDownList.SelectedValue);
				if(ParityDropDownList.SelectedIndex != -1)
                    Port.Parity = (WEIGHTSCALE_PARITY)Convert.ToInt32(ParityDropDownList.SelectedValue);
				if(StopBitsDropDownList.SelectedIndex != -1)
                    Port.StopBits = (WEIGHTSCALE_STOP_BITS)Convert.ToInt32(StopBitsDropDownList.SelectedValue);

				if(Port.Index != 0)
					Ports.Modify(Port);
				else
					Ports.Add(Port);
			}
			catch (Exception except)
			{
				ErrorHandler(except);
				return;
			}
			Response.Redirect("PortsForm.aspx");
			Session.Remove("Port");
		}

		private void Cancel_Command(object sender, CommandEventArgs e)
		{
			Response.Redirect("PortsForm.aspx");
			Session.Remove("Port");
		}
	}
}
