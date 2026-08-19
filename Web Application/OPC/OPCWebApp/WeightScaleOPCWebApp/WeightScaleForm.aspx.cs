/******************************************************************************

	FILE NAME:		WeightScaleForm.aspx.cs


	PURPOSE:			Implementation of WeightScaleForm


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
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Net;
using WeightScaleOPCObjectsLib;
using WeightScaleOPCServerLib;
using FMBusinessObjects.DataObjects;
using FuelsManager.FMWebApp;
using System.Globalization;

namespace WeightScaleOPCWebApp
{
   /// <summary>
   /// Summary description for WeightScaleForm.
   /// </summary>
   public partial class WeightScaleForm :	FMAutoSubmitFormBase,
												IDataDictionary
	{

		string [] IDataDictionary.Keys(SecurityClass Security)
		{
			string [] Keys={	"ID",
									"Type",
									"Port"};
									
			return Keys;
		}

		protected void Page_Load(object sender, System.EventArgs e)
		{
			try
			{
                this.GetSecurity();

                if (!Page.IsPostBack) 
				{
					Session.Remove("WeightScale");

					WeightScaleClass	weightScale;

				    var weightScales =
				        (IWeightScales)OpcCom.Interop.CreateInstance(
				            new Guid("{FB4C3029-D5C9-4BB8-AC5A-1914858D79D5}"),
				            (string)Session["WeightScaleSystem"],
				            new NetworkCredential());

				    // Get Index
				    if (this.Session["Index"] != null)
				    {
				        // Get WeightScale
				        weightScale =
				            (WeightScaleClass)
				            weightScales.Get(Convert.ToInt32(Session["Index"] as string, CultureInfo.InvariantCulture));

				        this.IDTextBox.Text = weightScale.ID;
				    }
				    else
				    {
				        weightScale = new WeightScaleClass();
				    }

				    // Populate TypeDropDownList
				    for (var type = WEIGHTSCALE_TYPE.TOLEDO_8142; type < WEIGHTSCALE_TYPE.MAX_WEIGHTSCALE_TYPE; type++)
				    {
				        var newItem = new ListItem(
				            weightScale.TypeID(type),
				            ((int)type).ToString("G", CultureInfo.InvariantCulture));
				        foreach (ListItem existingItem in this.TypeDropDownList.Items)
				        {
				            if (string.Compare(existingItem.Text, newItem.Text, StringComparison.Ordinal) < 0)
				            {
				                int index = this.TypeDropDownList.Items.IndexOf(existingItem);
				                this.TypeDropDownList.Items.Insert(index, newItem);
				                if (((int)weightScale.Type).ToString("G", CultureInfo.InvariantCulture) == newItem.Value)
                                {
                                    this.TypeDropDownList.SelectedIndex = index;
                                }

				                newItem = null;
				                break;
				            }
						}

						if (newItem != null)
						{
							this.TypeDropDownList.Items.Add(newItem);
                            if (((int)weightScale.Type).ToString("G", CultureInfo.InvariantCulture) == newItem.Value)
                            {
                                this.TypeDropDownList.SelectedIndex = this.TypeDropDownList.Items.Count - 1;
                            }
						}
					}
					if (weightScale.Type == WEIGHTSCALE_TYPE.SIPELARIES_ASCII)
					{
						this.DeviceIDTextBox.Text = weightScale.DeviceID.ToString(CultureInfo.InvariantCulture);
					}
					else
					{
						this.DeviceIDTextBox.Text = "0";
					}

					// Populate PortDropDownList


					// Populate PortDropDownList from the ports table
					PortCollectionClass portCollection;

					try
					{
						var ports = (IPorts)OpcCom.Interop.CreateInstance(
							new Guid("{265331A0-40D0-4DEC-B614-2A21CDC5CC1F}"),
							Session["WeightScaleSystem"] as string,
							new NetworkCredential());

						portCollection = (PortCollectionClass)ports.Enumerate();
					}
					catch (Exception except)
					{
						this.ErrorHandler(except);
						portCollection = new PortCollectionClass();
					}

					foreach (PortClass port in portCollection)
					{
                        var newItem = new ListItem(port.ID, port.Index.ToString("G", CultureInfo.InvariantCulture));
					    this.PortDropDownList.Items.Add(newItem);
					    if (weightScale.PortIndex == port.Index)
					    {
					        int index = this.PortDropDownList.Items.IndexOf(newItem);

					        this.PortDropDownList.SelectedIndex = index;
					    }
					}

				    this.Session["WeightScale"] = weightScale;

				    if (!Security.HasRight(RIGHT.MODIFY_SITES_AND_SITE_GROUPS) || this.PortDropDownList.Items.Count <= 0)
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

		private void OK_Command(object sender, CommandEventArgs e)
		{
			try
			{
			    var weightScales =
			        (IWeightScales)OpcCom.Interop.CreateInstance(
			            new Guid("{FB4C3029-D5C9-4BB8-AC5A-1914858D79D5}"),
			            (string)Session["WeightScaleSystem"],
			            new NetworkCredential());

			    var weightScale = (WeightScaleClass)Session["WeightScale"];

			    weightScale.ID = this.IDTextBox.Text.Trim();
			    if (this.TypeDropDownList.SelectedIndex != -1)
			    {
                    weightScale.Type = (WEIGHTSCALE_TYPE)Convert.ToInt32(this.TypeDropDownList.SelectedValue, CultureInfo.InvariantCulture);
                }

                if (this.PortDropDownList.SelectedIndex != -1)
                {
                    weightScale.PortIndex = Convert.ToInt32(this.PortDropDownList.Items[this.PortDropDownList.SelectedIndex].Value, CultureInfo.InvariantCulture);
                }

				if (weightScale.Type == WEIGHTSCALE_TYPE.SIPELARIES_ASCII)
				{
					weightScale.DeviceID = Convert.ToInt32(this.DeviceIDTextBox.Text);
				}
				else
				{
					weightScale.DeviceID = 0;
				}

                try
                {
                    if (weightScale.Index != 0)
                    {
                        weightScales.Modify(weightScale);
                    }
                    else
                    {
                        weightScales.Add(weightScale);
                    }
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
				this.ErrorHandler(except);
				return;
			}

		    Response.Redirect("WeightScalesForm.aspx", false);
		    Context.ApplicationInstance.CompleteRequest();
		    Session.Remove("WeightScale");
		}

		private void Cancel_Command(object sender, CommandEventArgs e)
		{
		    Response.Redirect("WeightScalesForm.aspx", false);
		    Context.ApplicationInstance.CompleteRequest();
		    Session.Remove("WeightScale");
		}
	}
}
