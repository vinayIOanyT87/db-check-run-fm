/******************************************************************************

	FILE NAME:		AcculoadForm.aspx.cs


	PURPOSE:			Implementation of AcculoadForm


	COMMENTS:

		Copyright (C) E+H Systems & Gauging, Inc. Norcross, GA, USA, 2002

		This file shall not be copied or reproduced in any form without
				the express written consent of Endress+HaAccuload.


	AUTHOR(S):	W. Gray


	VERSION:		1.0.0  Current version



	MODIFICATION HISTORY:
		Date:			By:			Reason:
		---------	----------  -------------------------------------------
		01/17/2008	W.Gray		7.3.2.1 - Added support for TCP/IP

*******************************************************************************/

using System;
using System.Collections;
using System.Data;
using System.Globalization;
using System.Net;
using System.Web.UI.WebControls;
using AcculoadOPCObjectsLib;
using AcculoadOPCServerLib;
using FMBusinessObjects.DataObjects;

// ReSharper disable once CheckNamespace
namespace AcculoadOPCWebApp
{
    /// <summary>
	/// Summary description for AcculoadForm.
	/// </summary>
	public class AcculoadForm : AcculoadFormBase
	{
		protected Image Image1;
		protected FMControls.FMLabel Label2;
		protected TextBox IDTextBox;
		protected FMControls.FMLabel UserNameRequiredLabel;
		protected FMControls.FMLabel Label3;
		protected DropDownList TypeDropDownList;
		protected FMControls.FMLabel Label4;
		protected DropDownList PortDropDownList;
		protected FMControls.FMDataGrid ArmDataGrid;
		protected FMControls.FMButton AddButton;
		protected FMControls.FMButton OKButton;
		protected FMControls.FMButton CancelButton;
		protected FMControls.FMLabel Label12;
		protected FMControls.FMRadioButton SerialCommunicationsRadioButton;
		protected FMControls.FMRadioButton NetworkCommunicationsRadioButton;
		protected FMControls.FMLabel Label5;
		protected TextBox IPAddressTextBox;
		protected FMControls.FMLabel Label1;

		private void UpdateArmsView()
		{
		    this.ArmDataGrid.DataSource= this.EnumerateArms();
		    this.ArmDataGrid.DataBind();
		}

        private void SetAddButtonState(bool enabled)
        {
            this.AddButton.Enabled = enabled && this.OkToAddArm();
		}

        private bool OkToAddArm()
        {
            bool enabled = true;

            var accuload = (AcculoadClass)this.Session["Accuload"];

            if ((ACCULOAD_TYPE)Convert.ToInt32(this.TypeDropDownList.SelectedValue) == ACCULOAD_TYPE.MICROLOAD_NET
                || (ACCULOAD_TYPE)Convert.ToInt32(this.TypeDropDownList.SelectedValue) == ACCULOAD_TYPE.MULTILOAD_II_SMP)
            {
                var arms = (ArmCollectionClass)accuload.Arms;

                if (arms.Count > 0)
                {
                    enabled = false;
				}
			}

			return enabled;
		}

        private ICollection EnumerateArms()
        {
            var accuload = (AcculoadClass)this.Session["Accuload"];
            var arms = (ArmCollectionClass)accuload.Arms;
            var armDataTable = new DataTable();

            armDataTable.Columns.Add("Index", typeof(int));
			armDataTable.Columns.Add("Number", typeof(int));
			armDataTable.Columns.Add("Address", typeof(int));
			armDataTable.Columns.Add("Type", typeof(string));
			armDataTable.Columns.Add("Products", typeof(int));

            for (int item = 0; item < arms.Count; item++)
            {
                DataRow armDataRow = armDataTable.NewRow();

                var arm = (ArmClass)arms.Item(item);
				armDataRow["Index"] = item;
				armDataRow["Number"] = arm.Number;
				armDataRow["Address"] = arm.Address;
				armDataRow["Type"] = this.GetDictionaryText("SmithMeter|" + arm.TypeID(arm.Type));
				armDataRow["Products"] = arm.Products;

                armDataTable.Rows.Add(armDataRow);
			}

            var acculoadDataView = new DataView(armDataTable);
            return acculoadDataView;
        }

        protected ListItemCollection EnumerateArmTypes()
        {
            var arm = new ArmClass();

            var armTypeItems = new ListItemCollection();

            for (var type = ACCULOAD_ARM_TYPE.STRAIGHT;
                 type < ACCULOAD_ARM_TYPE.MAX_ACCULOAD_ARM_TYPE;
                 type++)
            {
                string itemText = this.GetDictionaryText("SmithMeter|" + arm.TypeID(type));

                armTypeItems.Add(new ListItem(itemText, ((int)type).ToString(CultureInfo.InvariantCulture)));
            }

            return armTypeItems;
		}

		protected void Page_Load(object sender, EventArgs e)
		{
			try
			{
			    this.GetSecurity();

				if (!this.Page.IsPostBack) 
				{
				    this.Session.Remove("Accuload");

					AcculoadClass	accuload;


					// Get Index
				    if (this.Session["Index"] != null)
				    {
				        // Get Accuload
				        IAcculoads acculoads =
				            (IAcculoads) OpcCom.Interop.CreateInstance(new Guid("{41D54854-8705-400A-9B22-F58B58088BE7}"),
				                (string) this.Session["SmithMeterSystem"],
				                new NetworkCredential());

				        accuload = (AcculoadClass) acculoads.Get(Convert.ToInt32(this.Session["Index"] as string));

				        this.IDTextBox.Text = accuload.ID;
				    }
				    else
				    {
				        accuload = new AcculoadClass();
				    }

				    // Populate TypeDropDownList
					ListItem newItem;

				    for (var type = ACCULOAD_TYPE.ACCULOAD_2_STD; type < ACCULOAD_TYPE.MAX_ACCULOAD_TYPE; type++)
				    {
				        // Skip Types that are not supported
				        if (type == ACCULOAD_TYPE.ACCULOAD_2_RBM 
                            || type == ACCULOAD_TYPE.ACCULOAD_2_SEQ
				            || type == ACCULOAD_TYPE.ACCULOAD_2_STD)
				        {
				            continue;
				        }

				        newItem = new ListItem(accuload.TypeID(type), ((int)type).ToString(CultureInfo.InvariantCulture));
				        foreach (ListItem existingItem in this.TypeDropDownList.Items)
				        {
							if (string.Compare(existingItem.Text, newItem.Text, StringComparison.Ordinal) < 0)
							{
							    int index = this.TypeDropDownList.Items.IndexOf(existingItem);
							    this.TypeDropDownList.Items.Insert(index, newItem);
							    if (((int)accuload.Type).ToString(CultureInfo.InvariantCulture) == newItem.Value)
							    {
							        this.TypeDropDownList.SelectedIndex = index;
							    }

							    newItem = null;
							    break;
							}
						}

						if(newItem != null)
						{
						    this.TypeDropDownList.Items.Add(newItem);
						    if (((int)accuload.Type).ToString(CultureInfo.InvariantCulture) == newItem.Value)
						    {
						        this.TypeDropDownList.SelectedIndex = this.TypeDropDownList.Items.Count - 1;
						    }
						}
					}

					// Populate PortDropDownList					

				    var ports =
				        (IPorts)
				        OpcCom.Interop.CreateInstance(
				            new Guid("{2070F4BA-651D-4268-9F5A-1EBE0A137141}"),
				            (string)this.Session["SmithMeterSystem"],
				            new NetworkCredential());
				    var portCollection = (PortCollectionClass)ports.Enumerate();
				    for (int item = 0; item < portCollection.Count; item++)
				    {
				        var port = (PortClass)portCollection.Item(item);
				        newItem = new ListItem(port.ID, port.Index.ToString(CultureInfo.InvariantCulture));
				        foreach (ListItem existingItem in this.PortDropDownList.Items)
						{
							if (string.Compare(existingItem.Text, newItem.Text, StringComparison.Ordinal) > 0)
							{
							    int index = this.PortDropDownList.Items.IndexOf(existingItem);
							    this.PortDropDownList.Items.Insert(index, newItem);
							    if (accuload.PortIndex == port.Index)
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
						    if (accuload.PortIndex == port.Index)
						    {
						        this.PortDropDownList.SelectedIndex = this.PortDropDownList.Items.Count - 1;
						    }
						}
					}

                    newItem = new ListItem(this.GetDictionaryText("{None}"), "0");
                    this.PortDropDownList.Items.Insert(0, newItem);

                    if (accuload.NetworkCommunications)
				    {
				        this.PortDropDownList.Enabled = false;
				        this.IPAddressTextBox.Text = accuload.IPAddress;
				    }
					else
				    {
				        this.IPAddressTextBox.Text = string.Empty;
				        this.IPAddressTextBox.Enabled = false;
				    }

				    this.NetworkCommunicationsRadioButton.Checked = accuload.NetworkCommunications;
				    this.SerialCommunicationsRadioButton.Checked = !accuload.NetworkCommunications;

				    this.Session["Accuload"] = accuload;

				    this.UpdateArmsView();
					this.EnableControls(true);

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

        protected void AddGridEditColumn()
		{
            string altUpdateText = this.GetDictionaryText("SmithMeter|Update this item");
            string altEditText = this.GetDictionaryText("SmithMeter|Edit this item");
            string altCancelText = this.GetDictionaryText("SmithMeter|Cancel Edit on this item");
            var editColumn = new EditCommandColumn
		                                       {
		                                           ButtonType = ButtonColumnType.LinkButton,
		                                           HeaderText = this.GetDictionaryText("SmithMeter|Edit"),
		                                           UpdateText =
		                                               @"<img src=../FMWebApp/images/Update.gif border=0 align=absmiddle alt='"
		                                               + altUpdateText + @"'>",
		                                           EditText =
		                                               @"<img src=../FMWebApp/images/Edit.gif border=0 align=absmiddle alt='"
		                                               + altEditText + @"'>",
		                                           CancelText =
		                                               @"<img src=../FMWebApp/images/Cancel.gif border=0 align=absmiddle alt='"
		                                               + altCancelText + @"'>"
		                                       };

		    editColumn.ItemStyle.HorizontalAlign	= HorizontalAlign.Center;
			editColumn.ItemStyle.Width					= Unit.Parse( "0.5in" );
			editColumn.ItemStyle.VerticalAlign		= VerticalAlign.Middle;

		    this.ArmDataGrid.Columns.AddAt( 0, editColumn );
		}

		#region Web Form Designer generated code
		override protected void OnInit(EventArgs e)
		{
			this.GetSecurity();
			this.AddGridEditColumn();

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
			this.SerialCommunicationsRadioButton.CheckedChanged += new System.EventHandler(this.SerialCommunicationsRadioButton_CheckedChanged);
			this.NetworkCommunicationsRadioButton.CheckedChanged += new System.EventHandler(this.NetworkCommunicationsRadioButton_CheckedChanged);
			this.TypeDropDownList.SelectedIndexChanged += new System.EventHandler(this.TypeDropDownList_SelectedIndexChanged);
			this.ArmDataGrid.EditCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.ArmDataGrid_EditCommand);
			this.ArmDataGrid.PageIndexChanged += new System.Web.UI.WebControls.DataGridPageChangedEventHandler(this.ArmDataGrid_PageIndexChanged);
			this.ArmDataGrid.CancelCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.ArmDataGrid_CancelCommand);
			this.ArmDataGrid.UpdateCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.ArmDataGrid_UpdateCommand);
			this.ArmDataGrid.DeleteCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.ArmDataGrid_DeleteCommand);
			this.ArmDataGrid.ItemDataBound += new System.Web.UI.WebControls.DataGridItemEventHandler(this.ArmDataGrid_ItemDataBound);
			this.AddButton.Command += new System.Web.UI.WebControls.CommandEventHandler(this.AddButton_Command);
			this.OKButton.Command += new System.Web.UI.WebControls.CommandEventHandler(this.OK_Command);
			this.CancelButton.Command += new System.Web.UI.WebControls.CommandEventHandler(this.Cancel_Command);

		}
		#endregion

	    // ReSharper disable once InconsistentNaming
		private void OK_Command(object sender, CommandEventArgs e)
		{
            try
            {
                var acculoads =
                    (IAcculoads)
                    OpcCom.Interop.CreateInstance(
                        new Guid("{41D54854-8705-400A-9B22-F58B58088BE7}"),
                        (string)this.Session["SmithMeterSystem"],
                        new NetworkCredential());

                var accuload = (AcculoadClass)this.Session["Accuload"];

                accuload.ID = this.IDTextBox.Text.Trim();
                if (this.TypeDropDownList.SelectedIndex != -1)
                {
                    accuload.Type = (ACCULOAD_TYPE)Convert.ToInt32(this.TypeDropDownList.SelectedValue);
                }

                if (this.PortDropDownList.SelectedIndex != -1)
                {
                    accuload.PortIndex = this.NetworkCommunicationsRadioButton.Checked ? 0 : Convert.ToInt32(this.PortDropDownList.SelectedItem.Value);
                }

                this.ValidateNumberOfArms();

                accuload.NetworkCommunications = this.NetworkCommunicationsRadioButton.Checked;
				accuload.IPAddress= this.IPAddressTextBox.Text.Trim();

				// Microloads the Address is the lowest octet
				if(accuload.NetworkCommunications
				&& accuload.Type == ACCULOAD_TYPE.MICROLOAD_NET)
				{
					string [] octet=accuload.IPAddress.Split('.');
					if(octet.Length != 4)
						throw new Exception("SmithMeter|Invalid IP Address lower octet > 0 and <= 99");

					byte lowestOctet=Convert.ToByte(octet[3]);
					if(lowestOctet == 0 || lowestOctet > 99)
						throw new Exception("SmithMeter|Invalid IP Address lower octet > 0 and <= 99");

                    var arms = (ArmCollectionClass)accuload.Arms;
                    arms.Item(0).Address = lowestOctet;
                }
                try
                {
                    if (accuload.Index != 0)
                    {
                        acculoads.Modify(accuload);
                    }
                    else
                    {
                        acculoads.Add(accuload);
                    }
                }
                catch (System.Runtime.InteropServices.COMException ex)
                {
                    if (ex.Message.Contains("duplicate key") || ex.Message.Contains("Accuload Exists"))
                    {

                        throw new Exception("OPC Server Exists", ex);
                    }
                    else
                    {
                        //throw new Exception("Database Error", ex);
                        throw;
                    }
                }
            }
            catch (Exception except)
            {
                this.ErrorHandler(except);
                return;
            }

		    // ReSharper disable once ArrangeThisQualifier
		    this.Response.Redirect("AcculoadsForm.aspx?" + this.Security.CSRFTokenWithParamName, endResponse: false);
		    this.Context.ApplicationInstance.CompleteRequest();
            this.Session.Remove("Accuload");
		}

        private void ValidateNumberOfArms()
        {
            var accuload = (AcculoadClass)this.Session["Accuload"];

            if (accuload.Type == ACCULOAD_TYPE.MICROLOAD_NET || accuload.Type == ACCULOAD_TYPE.MULTILOAD_II_SMP)
            {
                var arms = (ArmCollectionClass)accuload.Arms;

                if (arms.Count > 1)
                {
                    throw new Exception("Microload device only supports one arm.");
                }
            }
        }

        // ReSharper disable once InconsistentNaming
        private void Cancel_Command(object sender, CommandEventArgs e)
		{
            // ReSharper disable once ArrangeThisQualifier
            this.Response.Redirect("AcculoadsForm.aspx?" + this.Security.CSRFTokenWithParamName, false);
            this.Context.ApplicationInstance.CompleteRequest();
            this.Session.Remove("Accuload");
		}

        // ReSharper disable once InconsistentNaming
        private void AddButton_Command(object sender, CommandEventArgs e)
        {
            if (this.OkToAddArm() == false)
            {
                this.UpdateArmsView();
				this.EnableControls(true);
			}
            else
            {
                var accuload = (AcculoadClass)this.Session["Accuload"];
                var arms = (ArmCollectionClass)accuload.Arms;
                var arm = new ArmClass
                {
                    Number =
                        arms.Count > 0 ? Convert.ToByte(((ArmClass) arms.Item(arms.Count - 1)).Number + 1) : (byte) 1
                };


                arms.Add(arm);

				// If the device type is currently set to Microload
                // or Multiload II SMP, default to "straight" arm type
                if (accuload.Type == ACCULOAD_TYPE.MICROLOAD_NET || accuload.Type == ACCULOAD_TYPE.MULTILOAD_II_SMP)
                {
					arm.Type = ACCULOAD_ARM_TYPE.STRAIGHT;
				}

                this.ArmDataGrid.CurrentPageIndex = (arms.Count - 1) / this.ArmDataGrid.PageSize;
                this.ArmDataGrid.EditItemIndex = (arms.Count - 1) % this.ArmDataGrid.PageSize;
                this.EnableControls(false);
                try
				{
					this.UpdateArmsView();
				}
				catch (Exception except)
				{
				    this.ErrorHandler(except);
				    arms.Remove(arms.Count - 1);
				    if (this.ArmDataGrid.CurrentPageIndex > 0 && this.ArmDataGrid.Items.Count == 1)
				    {
				        this.ArmDataGrid.CurrentPageIndex--;
				    }

				    this.ArmDataGrid.EditItemIndex = -1;
				    this.UpdateArmsView();
				    this.EnableControls(true);
				}
			}
		}

        // ReSharper disable once InconsistentNaming
        private void ArmDataGrid_EditCommand(object source, DataGridCommandEventArgs e)
        {
            var indexLabel = (Label)e.Item.FindControl("IndexLabel");
            if (indexLabel != null)
            {
                this.ArmDataGrid.EditItemIndex = e.Item.ItemIndex;
                this.EnableControls(false);
                try
				{
					this.UpdateArmsView();
				}
				catch (Exception except)
				{
				    this.ErrorHandler(except);
				    this.ArmDataGrid.EditItemIndex = -1;
				    this.UpdateArmsView();
				    this.EnableControls(true);
				}
			}
		}

        // ReSharper disable once InconsistentNaming
        private void ArmDataGrid_CancelCommand(object source, DataGridCommandEventArgs e)
        {
            var indexLabel = (Label)e.Item.FindControl("IndexLabel");
            if (indexLabel != null)
            {
                var accuload = (AcculoadClass)this.Session["Accuload"];
                var arms = (ArmCollectionClass)accuload.Arms;
                var arm = (ArmClass)arms.Item(Convert.ToInt32(indexLabel.Text));
                if (arm.Type == ACCULOAD_ARM_TYPE.MAX_ACCULOAD_ARM_TYPE)
                {
					arms.Remove(Convert.ToInt32(indexLabel.Text));
                    if (this.ArmDataGrid.CurrentPageIndex > 0
                        && this.ArmDataGrid.CurrentPageIndex * this.ArmDataGrid.PageSize >= this.ArmDataGrid.Items.Count)
                    {
                        this.ArmDataGrid.CurrentPageIndex--;
                    }
				}

				this.ArmDataGrid.EditItemIndex = -1;
				this.UpdateArmsView();
				this.EnableControls(true);
			}
		}

        // ReSharper disable once InconsistentNaming
        private void ArmDataGrid_DeleteCommand(object source, DataGridCommandEventArgs e)
        {
            var indexLabel = (Label)e.Item.FindControl("IndexLabel");
            if (indexLabel != null)
            {
				if (this.ArmDataGrid.EditItemIndex == e.Item.ItemIndex)
				{
				    this.ArmDataGrid.EditItemIndex = -1;
				    this.EnableControls(true);
				}
				else if (this.ArmDataGrid.EditItemIndex > e.Item.ItemIndex)
				{
				    this.ArmDataGrid.EditItemIndex--;
				}

                var accuload = (AcculoadClass)this.Session["Accuload"];
                var arms = (ArmCollectionClass)accuload.Arms;
				arms.Remove(Convert.ToInt32(indexLabel.Text));

                if (this.ArmDataGrid.CurrentPageIndex > 0 && this.ArmDataGrid.Items.Count == 1)
                {
                    this.ArmDataGrid.CurrentPageIndex--;
                }

				this.UpdateArmsView();
				this.EnableControls(true);
			}
		}

        // ReSharper disable once InconsistentNaming
        private void ArmDataGrid_UpdateCommand(object source, DataGridCommandEventArgs e)
        {
            var indexLabel = (Label)e.Item.FindControl("IndexLabel");
            if (indexLabel != null)
            {
                var accuload = (AcculoadClass)this.Session["Accuload"];
                var arms = (ArmCollectionClass)accuload.Arms;
                var arm = (ArmClass)arms.Item(Convert.ToInt32(indexLabel.Text));

                var armTextBox = (TextBox)e.Item.FindControl("ArmTextBox");
                arm.Number = Convert.ToByte(armTextBox.Text);

                var addressTextBox = (TextBox)e.Item.FindControl("AddressTextBox");
                arm.Address = Convert.ToByte(addressTextBox.Text);

                var armTypesDropDownList = (DropDownList)e.Item.FindControl("ArmTypesDropDownList");
                arm.Type = (ACCULOAD_ARM_TYPE)Convert.ToInt32(armTypesDropDownList.SelectedValue);

                var productsTextBox = (TextBox)e.Item.FindControl("ProductsTextBox");
                arm.Products = Convert.ToByte(productsTextBox.Text);

                this.ArmDataGrid.EditItemIndex = -1;
                this.UpdateArmsView();
                this.EnableControls(true);
			}		
		}

        // ReSharper disable once InconsistentNaming
        private void ArmDataGrid_ItemDataBound(object sender, DataGridItemEventArgs e)
        {
            var indexLabel = (Label)e.Item.FindControl("IndexLabel");
            if (indexLabel != null)
            {
                var armTypesDropDownList = (DropDownList)e.Item.FindControl("ArmTypesDropDownList");
                if (armTypesDropDownList != null)
                {
                    var accuload = (AcculoadClass)this.Session["Accuload"];
                    var arms = (ArmCollectionClass)accuload.Arms;
                    var arm = (ArmClass)arms.Item(Convert.ToInt32(indexLabel.Text));

                    if (arm.Type != ACCULOAD_ARM_TYPE.MAX_ACCULOAD_ARM_TYPE)
                    {
                        ListItemCollection items = armTypesDropDownList.Items;
                        int index = items.IndexOf(items.FindByValue(((int)arm.Type).ToString(CultureInfo.InvariantCulture)));
                        armTypesDropDownList.SelectedIndex = index;
                    }
                }
			}

            // Delete Button
            this.UpdateDeleteButton((LinkButton)e.Item.FindControl("DeleteButton"));
        }

        // ReSharper disable once InconsistentNaming
        private void ArmDataGrid_PageIndexChanged(object source, DataGridPageChangedEventArgs e)
		{
			// if we are editing do not allow a page change
            if (this.ArmDataGrid.EditItemIndex > -1)
            {
                return;
            }

			this.ArmDataGrid.CurrentPageIndex = e.NewPageIndex;
			this.UpdateArmsView();
		}

        private void EnableControls(bool enable)
		{
			this.OKButton.Enabled				= enable;
			this.CancelButton.Enabled			= enable;
			this.TypeDropDownList.Enabled	= enable;
			this.PortDropDownList.Enabled	= enable;
			this.IDTextBox.Enabled			= enable;

            this.SetAddButtonState(enable);
		}

        // ReSharper disable once InconsistentNaming
        private void TypeDropDownList_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.SetAddButtonState(true);
        }

	    // ReSharper disable once InconsistentNaming
		private void SerialCommunicationsRadioButton_CheckedChanged(object sender, EventArgs e)
		{
		    this.PortDropDownList.Enabled= this.SerialCommunicationsRadioButton.Checked;
		    this.IPAddressTextBox.Enabled= !this.SerialCommunicationsRadioButton.Checked;
			if(this.SerialCommunicationsRadioButton.Checked)
			{
			    this.IPAddressTextBox.Text=string.Empty;
			}
			else this.PortDropDownList.SelectedIndex=0;
		}

	    // ReSharper disable once InconsistentNaming
		private void NetworkCommunicationsRadioButton_CheckedChanged(object sender, EventArgs e)
		{
		    this.PortDropDownList.Enabled= !this.NetworkCommunicationsRadioButton.Checked;
		    this.IPAddressTextBox.Enabled= this.NetworkCommunicationsRadioButton.Checked;
		    if (!this.NetworkCommunicationsRadioButton.Checked)
		    {
		        this.IPAddressTextBox.Text = string.Empty;
		    }
		    else
		    {
		        this.PortDropDownList.SelectedIndex=this.PortDropDownList.Items.Count - 1;
		    }
		}
	}
}
