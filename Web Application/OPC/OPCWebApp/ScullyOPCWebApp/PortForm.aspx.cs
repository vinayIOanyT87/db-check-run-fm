/******************************************************************************

	FILE NAME:		PortForm.aspx.cs


	PURPOSE:			Implementation of PortForm


	COMMENTS:

		Copyright (C) E+H Systems & Gauging, Inc. Norcross, GA, USA, 2002

		This file shall not be copied or reproduced in any form without
				the express written consent of Endress+HaScully.


	AUTHOR(S):	S. Jiang


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
using ScullyOPCObjectsLib;
using ScullyOPCServerLib;
using System.Globalization;

namespace OPCWebApp.ScullyOPCWebApp
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

                    IPorts Ports = (IPorts)OpcCom.Interop.CreateInstance(new Guid("{BF99140E-F916-49c2-9541-61BDD75E4531}"),
																							(string) Session["ScullySystem"],
																							new NetworkCredential());


                    PortClass Port;

					// Get Index
                    if (Session["Index"] != null)
                    {
                        Port = (PortClass)Ports.Get(System.Convert.ToInt32(Session["Index"] as string, CultureInfo.InvariantCulture));
                    }
                    else
                    {
                        Port = new PortClass();
                    }


					// Populate PortDropDownList
					string[] Names=(string []) Ports.EnumeratePortIDs();

					int PortIndex=0;
					if ( Names != null )
					{
						foreach(string Name in Names)
						{
                            ListItem NewItem = new ListItem(Name, PortIndex.ToString("G", CultureInfo.InvariantCulture));

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
                        ListItem NewItem = new ListItem(Port.ID, PortIndex.ToString("G", CultureInfo.InvariantCulture));

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
                    for (SCULLY_BAUD Baud = SCULLY_BAUD.SCULLY_BAUD_1200; Baud < SCULLY_BAUD.MAX_SCULLY_BAUD; Baud++)
					{
                        ListItem NewItem = new ListItem(Port.BaudID(Baud), ((int)Baud).ToString("G", CultureInfo.InvariantCulture));
						BaudDropDownList.Items.Add(NewItem);
                        if (((int)Port.Baud).ToString("G", CultureInfo.InvariantCulture) == NewItem.Value)
                        {
                            BaudDropDownList.SelectedIndex = BaudDropDownList.Items.Count - 1;
                        }
					}

					// Populate DataBitsDownList
                    for (SCULLY_DATA_BITS DataBits = SCULLY_DATA_BITS.DATA_BITS_7; DataBits < SCULLY_DATA_BITS.MAX_SCULLY_DATA_BITS; DataBits++)
					{
                        ListItem NewItem = new ListItem(Port.DataBitsID(DataBits), ((int)DataBits).ToString("G", CultureInfo.InvariantCulture));
						DataBitsDropDownList.Items.Add(NewItem);
                        if (((int)Port.DataBits).ToString("G", CultureInfo.InvariantCulture) == NewItem.Value)
                        {
                            DataBitsDropDownList.SelectedIndex = DataBitsDropDownList.Items.Count - 1;
                        }
					}

					// Populate ParityDownList
                    for (SCULLY_PARITY Parity = SCULLY_PARITY.SCULLY_PARITY_NONE; Parity < SCULLY_PARITY.MAX_SCULLY_PARITY; Parity++)
					{
                        ListItem NewItem = new ListItem("Scully|" + Port.ParityID(Parity), ((int)Parity).ToString("G", CultureInfo.InvariantCulture));
						foreach(ListItem ExistingItem in ParityDropDownList.Items)
						{
							if(ExistingItem.Text.CompareTo(NewItem.Text) < 0)
							{
								int Index=ParityDropDownList.Items.IndexOf(ExistingItem);
								ParityDropDownList.Items.Insert(Index,NewItem);
                                if (((int)Port.Parity).ToString("G", CultureInfo.InvariantCulture) == NewItem.Value)
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
                            if (((int)Port.Parity).ToString("G", CultureInfo.InvariantCulture) == NewItem.Value)
                            {
                                ParityDropDownList.SelectedIndex = ParityDropDownList.Items.Count - 1;
                            }
						}
					}

					// Populate StopBitsDownList
                    for (SCULLY_STOP_BITS StopBits = SCULLY_STOP_BITS.STOP_BITS_1; StopBits < SCULLY_STOP_BITS.MAX_SCULLY_STOP_BITS; StopBits++)
					{
                        ListItem NewItem = new ListItem(Port.StopBitsID(StopBits), ((int)StopBits).ToString("G", CultureInfo.InvariantCulture));
						StopBitsDropDownList.Items.Add(NewItem);
                        if (((int)Port.StopBits).ToString("G", CultureInfo.InvariantCulture) == NewItem.Value)
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
			this.CancelButton.Command += new System.Web.UI.WebControls.CommandEventHandler(this.Cancel_Command);
			this.OKButton.Command += new System.Web.UI.WebControls.CommandEventHandler(this.OK_Command);

		}
		#endregion

		private void OK_Command(object sender, System.Web.UI.WebControls.CommandEventArgs e)
		{
			try
			{
                IPorts Ports = (IPorts)OpcCom.Interop.CreateInstance(new Guid("{BF99140E-F916-49c2-9541-61BDD75E4531}"),
																						(string)Session["ScullySystem"],
																						new NetworkCredential());

				PortClass	Port=(PortClass) Session["Port"];

				if(PortDropDownList.SelectedIndex != -1)
					Port.ID=PortDropDownList.SelectedItem.Text;
				if(BaudDropDownList.SelectedIndex != -1)
					Port.Baud = (SCULLY_BAUD)System.Convert.ToInt32(BaudDropDownList.SelectedValue, CultureInfo.InvariantCulture);
				if(DataBitsDropDownList.SelectedIndex != -1)
                    Port.DataBits = (SCULLY_DATA_BITS)System.Convert.ToInt32(DataBitsDropDownList.SelectedValue, CultureInfo.InvariantCulture);
				if(ParityDropDownList.SelectedIndex != -1)
                    Port.Parity = (SCULLY_PARITY)System.Convert.ToInt32(ParityDropDownList.SelectedValue, CultureInfo.InvariantCulture);
				if(StopBitsDropDownList.SelectedIndex != -1)
                    Port.StopBits = (SCULLY_STOP_BITS)System.Convert.ToInt32(StopBitsDropDownList.SelectedValue, CultureInfo.InvariantCulture);

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

		private void Cancel_Command(object sender, System.Web.UI.WebControls.CommandEventArgs e)
		{
			Response.Redirect("PortsForm.aspx");
			Session.Remove("Port");
		}
	}
}
