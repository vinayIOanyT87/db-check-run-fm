/******************************************************************************

	FILE NAME:		ScullyForm.aspx.cs


	PURPOSE:			Implementation of ScullyForm


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
using ScullyOPCObjectsLib;
using ScullyOPCServerLib;
using System.Globalization;

namespace OPCWebApp.ScullyOPCWebApp
{
	/// <summary>
	/// Summary description for ScullyForm.
	/// </summary>
	public partial class ScullyForm : ScullyFormBase
    {
		protected void Page_Load(object sender, System.EventArgs e)
		{
			try
			{
                GetSecurity();

                if (!Page.IsPostBack) 
				{
					Session.Remove("Scully");

					ScullyClass	Scully;

                    IScullys Scullys = (IScullys)OpcCom.Interop.CreateInstance(new Guid("{948DA86B-A687-494c-9B93-569B65499B36}"),
																														(string) Session["ScullySystem"],
																														new NetworkCredential());

					// Get Index
					if(Session["Index"] != null)
					{
						// Get Scully

                        Scully = (ScullyClass)Scullys.Get(System.Convert.ToInt32(Session["Index"] as string, CultureInfo.InvariantCulture));

						IDTextBox.Text=Scully.ID;
					}
					else
						Scully=new ScullyClass();

                    DeviceIDTextBox.Text = Scully.DeviceID.ToString();

					// Populate PortDropDownList from the ports table
					PortCollectionClass PortCollection;

					try
					{
						IPorts Ports = (IPorts)OpcCom.Interop.CreateInstance(
                            new Guid("{BF99140E-F916-49c2-9541-61BDD75E4531}"),
							Session["ScullySystem"] as string,
							new NetworkCredential());

						PortCollection = (PortCollectionClass)Ports.Enumerate();
					}
					catch (Exception except)
					{
						ErrorHandler(except);
						PortCollection = new PortCollectionClass();
					}

					foreach (PortClass Port in PortCollection)
					{
                        ListItem NewItem = new ListItem(Port.ID, Port.Index.ToString("G", CultureInfo.InvariantCulture));
						if (NewItem != null)
						{
							PortDropDownList.Items.Add(NewItem);
							if (Scully.PortIndex == Port.Index)
							{
								int Index = PortDropDownList.Items.IndexOf(NewItem);

								PortDropDownList.SelectedIndex = Index;
							}
						}
					}

					Session["Scully"]=Scully;

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
                IScullys Scullys = (IScullys)OpcCom.Interop.CreateInstance(new Guid("{948DA86B-A687-494c-9B93-569B65499B36}"),
																													(string) Session["ScullySystem"],
																													new NetworkCredential());

				ScullyClass	Scully=(ScullyClass) Session["Scully"];

				Scully.ID=IDTextBox.Text.Trim();
               
                if (PortDropDownList.SelectedIndex != -1)
                {
                    Scully.PortIndex = System.Convert.ToInt32(PortDropDownList.Items[PortDropDownList.SelectedIndex].Value, CultureInfo.InvariantCulture);
                }

                //Scully.DeviceID = 0;
                Scully.DeviceID = System.Convert.ToInt32(DeviceIDTextBox.Text);
                try
                {
                    if (Scully.Index != 0)
                        Scullys.Modify(Scully);
                    else
                        Scullys.Add(Scully);
                }
                catch (System.Runtime.InteropServices.COMException ex)
                {
                    if (ex.Message.Contains("duplicate key"))
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
				ErrorHandler(except);
				return;
			}
			Response.Redirect("ScullysForm.aspx");
			Session.Remove("Scully");
		}

		private void Cancel_Command(object sender, System.Web.UI.WebControls.CommandEventArgs e)
		{
			Response.Redirect("ScullysForm.aspx");
			Session.Remove("Scully");
		}
	}
}
